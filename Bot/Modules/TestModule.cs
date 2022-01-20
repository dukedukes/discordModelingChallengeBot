using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using ModelChallengeBot.Models.Queue;
using ModelChallengeBot.Queues;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ModelChallengeBot.Extensions;
using ModelChallengeBot.Attributes;
using ModelChallengeBot.Providers.EF;

namespace ModelChallengeBot.Bot.Modules
{
    public class TestModule : BotModuleBase
    {
        private readonly ILogger<TestModule> logger;
        private readonly IRenderQueue renderQueue;
        private readonly IActionQueue actionQueue;
        private readonly IRoleTypeProvider roleTypeProvider;
        private static readonly List<string> BLEND_FILE_EXTENSIONS = new List<string> { ".blend", ".blend1" };

        public TestModule(ILogger<TestModule> logger, IRenderQueue renderQueue, IActionQueue actionQueue, IRoleTypeProvider roleTypeProvider) : base(logger)
        {
            this.logger = logger;
            this.renderQueue = renderQueue;
            this.actionQueue = actionQueue;
            this.roleTypeProvider = roleTypeProvider;
        }

        [Command("Purge")]
        public async Task Purge()
        {
            logger.LogInformation($"user {Context.User.Id} - {Context.User.Username} requesting purge");
            if(!(await IsAdministrator()))
            {
                await Context.Channel.DeleteMessageAsync(Context.Message);
                var dmChannel = await Context.User.CreateDMChannelAsync();
                await dmChannel.SendMessageAsync("You don't have permission to purge a channel");
            }

            var channelId = Context.Channel.Id;
            await actionQueue.Enqueue(new ActionQueueModel(async (discordClient) =>
            {
                var channelToPurge = channelId;
                var channel = await discordClient.GetChannelAsync(channelToPurge) as ISocketMessageChannel;
                if (channel == null)
                {
                    logger.LogError($"can't find channel to purge. channel is {channelToPurge}");
                }
                IEnumerable<IMessage> messages = null;
                Func<Task> getMessages = async () =>
                {
                    var asyncMessages = channel.GetMessagesAsync();
                    messages = await AsyncEnumerableExtensions.FlattenAsync(asyncMessages);
                };
                await getMessages();
                while (messages != null && messages.Any())
                {
                    foreach(var message in messages)
                    {
                        await channel.DeleteMessageAsync(message);
                    }
                    await getMessages();
                }
            }));            
        }

        [ConsumesAttachment("Blend file")]
        [Command("TestRender")]
        public async Task TestRender()
        {
            var roleTypes = await roleTypeProvider.GetRegistrations();
            var roleTypeIds = roleTypes.Select(rt => rt.RegisterId).ToList();
            var guildUser = Context.User as SocketGuildUser;
            if (!guildUser.Roles.Any(role => roleTypeIds.Contains(role.Id)))
            {
                await Context.Message.Channel.DeleteMessageAsync(Context.Message.Id);
                await ReportError("You must be an organizer to use this command");
                
                return;
            }

            logger.LogInformation($"received TestRender value from user {Context.User.Id} - {Context.User.Username}");
            var attachment = Context.Message.Attachments.FirstOrDefault();
            var extension = attachment.Url.Substring(attachment.Url.LastIndexOf('.'));
            if (attachment == null)
            {
                await ReportError("You must attach a .blend file to perform a test render");
                await Context.Message.Channel.DeleteMessageAsync(Context.Message.Id);
                return;
            }
            else if (!BLEND_FILE_EXTENSIONS.Contains(extension))
            {
                await ReportError("Attachment is not a blend file");
                await Context.Message.Channel.DeleteMessageAsync(Context.Message.Id);
                return;
            }
            var notificationMessage = await ReplyAsync("Processing file...");

            var instigator = Context.BuildInstigator();
            instigator.NotificationMessageId = notificationMessage.Id;
            await renderQueue.Enqueue(new RenderQueueModel(attachment.Url, instigator));
        }
    }
}
