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
    public abstract class BaseQueueWorker<T> : WorkerBase where T: IQueueModel
    {
        private readonly ILogger logger;
        private IQueue<T> baseQueue;
        protected BaseQueueWorker(ILogger logger, IQueue<T> baseQueue) : base(logger)
        {
            this.baseQueue = baseQueue;
            this.logger = logger;
        }

        protected override Task Initialize()
        {
            return Task.CompletedTask;
        }
        protected virtual Task<bool> CanProcess() 
        {
            return Task.FromResult(true); 
        }
        protected override async Task Tick()
        {
            if (await CanProcess())
            {
                await ProcessQueue();
            }
            else
            {
                logger.LogDebug("Discord client is disconnected");
            }
        }

        /// <summary>
        /// Processes a single record from queue
        /// </summary>
        /// <returns></returns>
        private async Task ProcessQueue()
        {
            int queueLength = await baseQueue.GetQueueLength();
            if (queueLength > 0)
            {
                var queueItem = await baseQueue.Dequeue();
                logger.LogInformation($"Queue working: {queueLength} items in queue. Processing item {queueItem.Id}");

                await ProcessQueueItem(queueItem);
            }
        }

        protected abstract Task ProcessQueueItem(T queueItem);
    }
}
