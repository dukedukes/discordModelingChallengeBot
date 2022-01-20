using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelChallengeBot.Attributes;
using ModelChallengeBot.Bot;
using ModelChallengeBot.Configuration;
using ModelChallengeBot.Enums;
using ModelChallengeBot.Extensions;
using ModelChallengeBot.Providers.Discord.Handlers;
using ModelChallengeBot.Providers.EF;
using ModelChallengeBot.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ModelChallengeBot.Workers
{
    public class StatusWorker : WorkerBase
    {
        private readonly ILogger<StatusWorker> logger;
        private readonly DiscordSocketClient discordSocketClient;
        private readonly IChannelTypeProvider channelTypeProvider;
        private readonly IAcceptedChallengeProvider acceptedChallengeProvider;
        private readonly IRoleTypeProvider roleTypeProvider;
        private readonly DiscordSettings discordSettings;
        private readonly CommandHandler commandHandler;
        private string LastStatusMessageCache;

        public StatusWorker(ILogger<StatusWorker> logger,
                            IHostApplicationLifetime applicationLifetime,
                            DiscordSocketClient discordSocketClient,
                            IChannelTypeProvider channelTypeProvider,
                            IAcceptedChallengeProvider acceptedChallengeProvider,
                            IRoleTypeProvider roleTypeProvider,
                            DiscordSettings discordSettings,
                            CommandHandler commandHandler) : base(logger)
        {
            applicationLifetime.ApplicationStopping.Register(OnShutdown);
            this.logger = logger;
            this.discordSocketClient = discordSocketClient;
            this.channelTypeProvider = channelTypeProvider;
            this.acceptedChallengeProvider = acceptedChallengeProvider;
            this.roleTypeProvider = roleTypeProvider;
            this.discordSettings = discordSettings;
            this.commandHandler = commandHandler;
        }

        private void OnShutdown()
        {
            ReportOfflineStatus().GetAwaiter().GetResult();
        }

        public override int TickDelayMilliseconds => 1000 * 60;

        protected async override Task Initialize()
        {
            try
            {
                await commandHandler.InstallCommandAsync();
                discordSocketClient.Log += LogDiscordMessage;
                discordSocketClient.ButtonExecuted += acceptedChallengeProvider.HandleAcceptedChallenge;//we only got one button so this is alright
                await discordSocketClient.LoginAsync(TokenType.Bot, discordSettings.Secret);
                await discordSocketClient.StartAsync();
                var isConnected = await RetryUtil.RetryConditionRequest(() => Task.FromResult(discordSocketClient.ConnectionState == ConnectionState.Connected));
                if (!isConnected)
                {
                    logger.LogWarning("Not connected to discord after retry attempts");
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Exception occurred: {ex}");
            }
        }

        private Task LogDiscordMessage(LogMessage logMessage)
        {
            var serilogMessage = logMessage.ConvertToSerilogMessage();
            logger.Log(serilogMessage.LogLevel, serilogMessage.Exception, serilogMessage.Message);
            return Task.CompletedTask;
        }

        protected async override Task Tick()
        {
            await ReportStatus();
        }

        private async Task ReportOfflineStatus()
        {
            logger.LogInformation("detected shutdown");
            StringBuilder statusMessage = new StringBuilder();
            statusMessage.AppendLine(".");
            statusMessage.AppendLine("**Status**");
            statusMessage.AppendLine("> Offline");
            await UpdateStatus(statusMessage.ToString());
            await discordSocketClient.StopAsync();
        }

        private async Task ReportStatus()
        {
            if (!(await HasStatusChannel()))
            {
                return;
            }

            StringBuilder statusMessage = new StringBuilder();
            statusMessage.AppendLine(".");
            statusMessage.AppendLine("**Status**");
            statusMessage.AppendLine("> Online");
            await AppendChannelStatusSection(statusMessage);
            await AppendRoleStatusSection(statusMessage);
            await AppendCommandStatusSection(statusMessage);

            var statusMessageString = statusMessage.ToString();
            if (!statusMessageString.Equals(LastStatusMessageCache))
            {
                await UpdateStatus(statusMessageString);
                LastStatusMessageCache = statusMessageString;
            }
        }

        private async Task UpdateStatus(string status)
        {
            if (discordSocketClient.ConnectionState == ConnectionState.Connected)
            {
                var statusChannel = await GetStatusChannel();
                ISocketMessageChannel channel = await RetryUtil.RetryResultRequest(
                    async () => (await discordSocketClient.GetChannelAsync(statusChannel.Value)) as ISocketMessageChannel);
                if (channel == null)
                {
                    logger.LogWarning($"Failed to find status channel after retries");
                    return;
                }
                var messages = await GetChannelMessages(channel);
                if (messages.Any())
                {
                    var firstMessage = messages.First();
                    await channel.ModifyMessageAsync(firstMessage.Id, (properties) =>
                    {
                        properties.Content = status;
                    });
                }
                else
                {   
                    await channel.SendMessageAsync(status);
                }
            }
            else
            {
                logger.LogInformation($"Can't update status. discord client state: {discordSocketClient.ConnectionState}");
            }
        }

        private static async Task<IEnumerable<IMessage>> GetChannelMessages(ISocketMessageChannel channel)
        {
            var asyncMessages = channel.GetMessagesAsync();
            return await AsyncEnumerableExtensions.FlattenAsync(asyncMessages);
        }

        private async Task AppendChannelStatusSection(StringBuilder statusMessage)
        {
            statusMessage.AppendLine("**Channel Registry**");
            var registeredChannels = await channelTypeProvider.GetRegistrations();
            var fullList = Enum.GetValues<ChallengeChannelType>().Select(e => e.ToString()).ToList();
            registeredChannels.ForEach(registeredChannel =>
            {
                if (!fullList.Contains(registeredChannel.Type))
                {
                    logger.LogError($"List desync: DB has unknown type {registeredChannel.Type}");
                }
                else
                {
                    fullList.Remove(registeredChannel.Type);
                    statusMessage.AppendLine($"> {registeredChannel.Type}: {MentionUtils.MentionChannel(registeredChannel.RegisterId)}");
                }
            });
            fullList.ForEach(unregistedChannel =>
            {
                statusMessage.AppendLine($"{unregistedChannel}: Unregistered");
            });
        }
        private async Task AppendRoleStatusSection(StringBuilder statusMessage)
        {
            statusMessage.AppendLine("**Role Registry**");
            var roleTypeRegistrations = await roleTypeProvider.GetRegistrations();
            var fullList = Enum.GetValues<ChallengeRoleType>().Select(e => e.ToString()).ToList();
            roleTypeRegistrations.ForEach(registeredRole =>
            {
                if (!fullList.Contains(registeredRole.Type))
                {
                    logger.LogError($"List desync: DB has unknown role type {registeredRole.Type}");
                }
                else
                {
                    fullList.Remove(registeredRole.Type);
                }
            });
            if (fullList.Any())
            {
                fullList.ForEach(role =>
                {
                    statusMessage.AppendLine($"> Role {role} is unregistered");
                });
            }
            else
            {
                statusMessage.AppendLine("> All roles registered");
            }
        }

        private Task AppendCommandStatusSection(StringBuilder statusMessage)
        {
            statusMessage.AppendLine("**Commands**");
            foreach (var assembly in Assembly.GetExecutingAssembly().GetTypes().Where(assembly => assembly.IsAssignableTo(typeof(ModuleBase<SocketCommandContext>))))
            {
                MethodInfo[] methods = assembly.GetMethods(BindingFlags.Instance | BindingFlags.Public);
                foreach (var method in methods)
                {
                    var commandAttribute = method.GetCustomAttribute<CommandAttribute>();
                    if (commandAttribute == null)
                    {
                        continue;
                    }
                    var attachmentAttribute = method.GetCustomAttribute<ConsumesAttachmentAttribute>();
                    statusMessage.Append($"> {discordSettings.CommandPrefix}{commandAttribute.Text}");
                    var commandName = commandAttribute.Text;
                    var parameters = method.GetParameters();

                    if (parameters.Length == 0 && attachmentAttribute == null)
                    {
                        statusMessage.AppendLine();
                        continue;
                    }
                    List<string> parameterDescriptions = new List<string>();
                    foreach (var parameter in parameters)
                    {
                        statusMessage.Append($" {parameter.Name}(*{parameter.ParameterType.Name}*)");
                    }
                    if (attachmentAttribute != null)
                    {
                        statusMessage.Append($" Attachment(*{attachmentAttribute.AttachmentType}*)");
                    }
                    statusMessage.AppendLine();
                }
            }
            return Task.CompletedTask;
        }
        private async Task<bool> HasStatusChannel()
        {
            return (await GetStatusChannel()) != null;
        }

        private async Task<ulong?> GetStatusChannel()
        {
            var statusChannel = await channelTypeProvider.GetRegisterByType(ChallengeChannelType.Status);
            if (statusChannel == null)
            {
                var admin = await discordSocketClient.GetUserAsync(discordSettings.Admin);
                var dmChannel = await admin.CreateDMChannelAsync();
                await dmChannel.SendMessageAsync("register a status channel: !RegisterChannel @channel status");
                return null;
            }

            return statusChannel;
        }
    }
}
