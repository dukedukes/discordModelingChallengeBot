using Microsoft.Extensions.Logging;
using ModelChallengeBot.Models.Queue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelChallengeBot.Queues
{
    public class ActionQueue : BaseQueue<ActionQueueModel>, IActionQueue
    {
        public ActionQueue(ILogger<ActionQueue> logger) : base(logger)
        {
        }
    }
}
