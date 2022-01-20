using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelChallengeBot.Models.Queue
{
    public class ActionQueueModel : BaseQueueModel
    {
        public ActionQueueModel(Func<DiscordSocketClient, Task> action)
        {
            this.Action = action;
        }
        public Func<DiscordSocketClient, Task> Action { get; }
    }
}
