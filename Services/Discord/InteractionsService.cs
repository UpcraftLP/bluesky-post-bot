using System.Reflection;
using Discord;
using Discord.Addons.Hosting;
using Discord.Addons.Hosting.Util;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Options;
using DiscordConfig = Up.Bsky.PostBot.Util.Config.DiscordConfig;
using IResult = Discord.Interactions.IResult;

namespace Up.Bsky.PostBot.Services.Discord;

public class InteractionsService : DiscordClientService
{
    private readonly DiscordConfig _config;
    private readonly InteractionService _interactionService;

    private readonly IServiceProvider _services;

    public InteractionsService(DiscordSocketClient client, ILogger<InteractionsService> logger, IServiceProvider services, InteractionService interactionService, IOptions<DiscordConfig> config) : base(client, logger)
    {
        _services = services;
        _interactionService = interactionService;
        _config = config.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Process the InteractionCreated payloads to execute Interactions commands
        Client.InteractionCreated += HandleInteraction;

        // Process the command execution results 
        _interactionService.SlashCommandExecuted += SlashCommandExecuted;
        _interactionService.ContextCommandExecuted += ContextCommandExecuted;
        _interactionService.ComponentCommandExecuted += ComponentCommandExecuted;

        // Add the public modules that inherit InteractionModuleBase<T> to the InteractionService
        await _interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

        await Client.WaitForReadyAsync(stoppingToken);
        // await _interactionService.RegisterCommandsToGuildAsync(_config.GuildId);

        // to delete commands: ! comment out the AddModulesAsync() call !
        // await _interactionService.AddCommandsGloballyAsync(true);
        // await _interactionService.RegisterCommandsToGuildAsync(_config.GuildId);


    }

    # region Execution

    private async Task HandleInteraction(SocketInteraction arg)
    {
        try
        {
            // Create an execution context that matches the generic type parameter of your InteractionModuleBase<T> modules
            var ctx = new SocketInteractionContext(Client, arg);
            await _interactionService.ExecuteCommandAsync(ctx, _services);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Unable to handle socket interaction");

            // If a Slash Command execution fails it is most likely that the original interaction acknowledgement will persist. It is a good idea to delete the original
            // response, or at least let the user know that something went wrong during the command execution.
            if (arg.Type == InteractionType.ApplicationCommand)
            {
                await arg.GetOriginalResponseAsync().ContinueWith(async msg => await msg.Result.DeleteAsync());
            }
        }
    }

    # endregion

    # region Error Handling

    private Task ComponentCommandExecuted(ComponentCommandInfo arg1, IInteractionContext arg2, IResult arg3)
    {
        if (!arg3.IsSuccess)
        {
            switch (arg3.Error)
            {
                case InteractionCommandError.UnmetPrecondition:
                    // implement
                    break;
                case InteractionCommandError.UnknownCommand:
                    // implement
                    break;
                case InteractionCommandError.BadArgs:
                    // implement
                    break;
                case InteractionCommandError.Exception:
                    // implement
                    break;
                case InteractionCommandError.Unsuccessful:
                    // implement
                    break;
            }
        }

        return Task.CompletedTask;
    }

    private Task ContextCommandExecuted(ContextCommandInfo arg1, IInteractionContext arg2, IResult arg3)
    {
        if (!arg3.IsSuccess)
        {
            switch (arg3.Error)
            {
                case InteractionCommandError.UnmetPrecondition:
                    // implement
                    break;
                case InteractionCommandError.UnknownCommand:
                    // implement
                    break;
                case InteractionCommandError.BadArgs:
                    // implement
                    break;
                case InteractionCommandError.Exception:
                    // implement
                    break;
                case InteractionCommandError.Unsuccessful:
                    // implement
                    break;
            }
        }

        return Task.CompletedTask;
    }

    private Task SlashCommandExecuted(SlashCommandInfo arg1, IInteractionContext arg2, IResult arg3)
    {
        if (!arg3.IsSuccess)
        {
            switch (arg3.Error)
            {
                case InteractionCommandError.UnmetPrecondition:
                    // implement
                    break;
                case InteractionCommandError.UnknownCommand:
                    // implement
                    break;
                case InteractionCommandError.BadArgs:
                    // implement
                    break;
                case InteractionCommandError.Exception:
                    // implement
                    break;
                case InteractionCommandError.Unsuccessful:
                    // implement
                    break;
            }
        }

        return Task.CompletedTask;
    }

    # endregion
}
