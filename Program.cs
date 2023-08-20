using GraphQL.AspNet.Configuration;
using Microsoft.EntityFrameworkCore;
using Up.Bsky.PostBot.Database;
using Up.Bsky.PostBot.Util.Auth;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddLogging();
builder.Services.AddHttpClient();
builder.Services.AddGraphQL(options =>
{
    options.ExecutionOptions.MaxQueryDepth = 20;
});
builder.Services.AddAuthorization();
builder.Services.AddSingleton<ApiKeyAuthorizationFilter>();
builder.Services.AddSingleton<IApiKeyValidator, ApiKeyValidator>();
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
