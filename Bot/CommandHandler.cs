using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using ModelChallengeBot.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ModelChallengeBot.Bot
{
    public class CommandHandler
    {
        private readonly ILogger<CommandHandler> logger;
        private readonly DiscordSocketClient discordClient;
        private readonly CommandService commandService;
        private readonly IServiceProvider serviceProvider;
        private readonly ISubmitChallengeProvider submitChallengeProvider;

        public CommandHandler(ILogger<CommandHandler> logger, DiscordSocketClient discordClient, CommandService commandService, IServiceProvider serviceProvider, ISubmitChallengeProvider submitChallengeProvider)
        {
            this.logger = logger;
            this.discordClient = discordClient;
            this.commandService = commandService;
            this.serviceProvider = serviceProvider;
            this.submitChallengeProvider = submitChallengeProvider;
        }

        public async Task InstallCommandAsync()
        {
            discordClient.MessageReceived += HandleCommandAsync;
            await commandService.AddModulesAsync(Assembly.GetEntryAssembly(), serviceProvider);
        }

        //https://discordnet.dev/guides/text_commands/intro.html
        private async Task HandleCommandAsync(SocketMessage messageParam)
        {
            
            // Don't process the command if it was a system message
            var message = messageParam as SocketUserMessage;
            if (message == null) return;

            // Create a number to track where the prefix ends and the command begins
            int argPos = 0;

            // Determine if the message is a command based on the prefix and make sure no bots trigger commands
            if (message.HasMentionPrefix(discordClient.CurrentUser, ref argPos) ||
               message.Author.IsBot)
            {
                return;
            }

            var channelAsPrivate = messageParam.Channel as IPrivateChannel;
            if (channelAsPrivate != null)
            {
                await HandlePrivateMessage(message);
                return;
            }

            if (!message.HasCharPrefix('!', ref argPos))
            {
                return;
            }
            // Create a WebSocket-based command context based on the message
            var context = new SocketCommandContext(discordClient, message);

            // Execute the command with the command context we just
            // created, along with the service provider for precondition checks.
            try
            {
                await commandService.ExecuteAsync(
                    context: context,
                    argPos: argPos,
                    services: serviceProvider);
            }
            catch (Exception ex)
            {
                logger.LogError($"Exception running command. Ex {ex}");
            }
        }

        // Only accept uploads as submissions. make a CommandService specific to private messages if more messages are needed
        private async Task HandlePrivateMessage(SocketUserMessage message)
        {
            try
            {
                var context = new SocketCommandContext(discordClient, message);

                var attachment = context.Message.Attachments.SingleOrDefault();
                if (attachment != null)
                {
                    await submitChallengeProvider.SubmitChallenge(context.User.Id, attachment.Url);
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Exception handling challenge submission Exception: {ex}");
            }
        }
    }
}
