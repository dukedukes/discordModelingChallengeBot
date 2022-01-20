using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using ModelChallengeBot.Utils;
using ModelChallengeBot.Models.Queue;
using ModelChallengeBot.Providers;
using ModelChallengeBot.Providers.Discord;
using ModelChallengeBot.Queues;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ModelChallengeBot.Workers.Queue
{
    public class RenderQueueWorker : BaseQueueWorker<RenderQueueModel>
    {
        private readonly ILogger<RenderQueueWorker> logger;
        private readonly IDiscordUserProvider discordUserProvider;
        private readonly IDownloadProvider downloadProvider;
        private readonly IRenderProvider renderProvider;
        private readonly DiscordSocketClient discordSocketClient;

        public RenderQueueWorker(ILogger<RenderQueueWorker> logger, IDiscordUserProvider discordUserProvider, IRenderQueue renderQueue, IDownloadProvider downloadProvider, IRenderProvider renderProvider, DiscordSocketClient discordSocketClient) : base(logger, renderQueue)
        {
            this.logger = logger;
            this.discordUserProvider = discordUserProvider;
            this.downloadProvider = downloadProvider;
            this.renderProvider = renderProvider;
            this.discordSocketClient = discordSocketClient;
        }

        public override int TickDelayMilliseconds => 1000;

        protected override Task<bool> CanProcess()
        {
            return Task.FromResult(discordSocketClient.ConnectionState == Discord.ConnectionState.Connected);
        }

        protected override async Task ProcessQueueItem(RenderQueueModel renderItem)
        {
            var channel = await discordSocketClient.GetChannelAsync(renderItem.Instigator.ChannelId, new Discord.RequestOptions() { AuditLogReason = "User requested file render" });
            if (channel == null)
            {
                logger.LogError($"channel {renderItem.Instigator.ChannelId} does not exist");
                await discordUserProvider.SendUserDM(renderItem.Instigator.UserId, "Render failed, unable to find target channel");
                return;
            }
            var socketMessageChannel = channel as ISocketMessageChannel;
            if (socketMessageChannel == null)
            {
                logger.LogError("IChannel can't be casted to ISocketMessageChannel");
                return;
            }
            
            var filePath = await downloadProvider.Download(renderItem.Url);
            var renderResultPath = await renderProvider.Render(filePath, $"{renderItem.Instigator.Username}-{StringUtil.GenerateShortUniqueName()}");
            await socketMessageChannel.SendFileAsync(renderResultPath, $"Uploaded by {MentionUtils.MentionUser(renderItem.Instigator.UserId)}");
            File.Delete(renderResultPath);
            if (renderItem.Instigator.NotificationMessageId.HasValue)
            {
                await socketMessageChannel.DeleteMessageAsync(renderItem.Instigator.NotificationMessageId.Value);
            }
            await socketMessageChannel.DeleteMessageAsync(renderItem.Instigator.MessageId);
        }
    }
}
