using Microsoft.Extensions.Logging;
using ModelChallengeBot.Models.Queue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelChallengeBot.Queues
{
    public class RenderQueue : BaseQueue<RenderQueueModel>, IRenderQueue
    {
        private readonly ILogger<RenderQueue> logger;

        public RenderQueue(ILogger<RenderQueue> logger) : base(logger)
        {
            this.logger = logger;
        }
    }
}
