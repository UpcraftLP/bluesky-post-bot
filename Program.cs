using Discord;
using Discord.Addons.Hosting;
using Discord.WebSocket;
using FishyFlip;
using GraphQL.AspNet.Configuration;
using Microsoft.EntityFrameworkCore;
using Up.Bsky.PostBot.Database;
using Up.Bsky.PostBot.Services.Bluesky;
using Up.Bsky.PostBot.Services.Discord;
using Up.Bsky.PostBot.Util.Auth;
using Up.Bsky.PostBot.Util.Config;
using DiscordConfig = Up.Bsky.PostBot.Util.Config.DiscordConfig;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddLogging();
builder.Services.AddHttpClient();
builder.Services.AddGraphQL(options => { options.ExecutionOptions.MaxQueryDepth = 20; });
builder.Services.AddAuthorization();
builder.Services.AddSingleton<ApiKeyAuthorizationFilter>();
builder.Services.AddSingleton<IApiKeyValidator, ApiKeyValidator>();
builder.Services.AddSingleton<ATProtocol>(serviceProvider =>
{
    var config = builder.Configuration.GetSection(AtProtoConfig.SectionName).Get<AtProtoConfig>() ?? throw new Exception("No ATProto config found!");
    var logger = serviceProvider.GetRequiredService<ILogger<ATProtocol>>();
    var client = new ATProtocolBuilder()
        .EnableAutoRenewSession(true)
        .WithInstanceUrl(new Uri(config.ServiceUrl))
        .WithLogger(logger)
        .Build();
    client.Server.CreateSessionAsync(config.LoginIdentifier, config.LoginToken).Wait();
    return client;
});
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseLazyLoadingProxies();
    var connString = builder.Configuration.GetConnectionString("Postgres") ?? throw new Exception("No connection string found!");
    if (builder.Environment.IsDevelopment() && !connString.Contains("Include Error Detail"))
    {
        if (!connString.EndsWith(';'))
        {
            connString += ';';
        }
        connString += "Include Error Detail=true;";
    }
    var contextBuilder = options.UseNpgsql(connString);
    if (builder.Environment.IsDevelopment())
    {
        contextBuilder.EnableSensitiveDataLogging();
    }
});
builder.Services.AddScoped<IFetchPostsService, FetchPostsService>();
builder.Host.ConfigureDiscordHost((ctx, config) =>
{
    config.SocketConfig = new DiscordSocketConfig
    {
        LogLevel = LogSeverity.Warning,
        AlwaysDownloadUsers = false,
        MessageCacheSize = 50,
        GatewayIntents = GatewayIntents.Guilds,
    };
    config.LogFormat = (msg, ex) => $"{msg.Source}: {msg.Message}";
    var dcConfig = ctx.Configuration.GetSection(DiscordConfig.SectionName).Get<DiscordConfig>();
    config.Token = dcConfig?.Token ?? throw new ApplicationException("no discord bot token provided!");
});
builder.Host.UseInteractionService((_, config) =>
{
    config.LogLevel = LogSeverity.Warning;
    config.UseCompiledLambda = true;
    config.AutoServiceScopes = true;
});
builder.Services.AddScoped<IUpdateStatusService, UpdateStatusService>();

builder.Services.AddHostedService<FetchPostsBackgroundService>();
builder.Services.AddHostedService<OnlineStatusService>();
builder.Services.AddHostedService<InteractionsService>();
builder.Services.AddHostedService<JoinMessageService>();

var app = builder.Build();
app.UseAuthorization();
app.UseGraphQL();
Init(app).Wait();
app.Run();
return;

async Task Init(IHost host)
{
    using var scope = host.Services.CreateScope();

    // run pending database migrations
    await scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.MigrateAsync();
}