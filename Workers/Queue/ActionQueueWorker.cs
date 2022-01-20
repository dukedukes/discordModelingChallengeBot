using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using ModelChallengeBot.Models.Queue;
using ModelChallengeBot.Queues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelChallengeBot.Workers.Queue
{
    public class ActionQueueWorker : BaseQueueWorker<ActionQueueModel>
    {
        private readonly DiscordSocketClient discordSocketClient;

        public ActionQueueWorker(ILogger<ActionQueueWorker> logger, DiscordSocketClient discordSocketClient, IActionQueue actionQueue) : base(logger, actionQueue)
        {
            this.discordSocketClient = discordSocketClient;
        }

        public override int TickDelayMilliseconds => 2000;

        protected async override Task ProcessQueueItem(ActionQueueModel queueItem)
        {
            await queueItem.Action(discordSocketClient);
        }

    }
}
