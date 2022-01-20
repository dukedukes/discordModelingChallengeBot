using Microsoft.Extensions.Logging;
using ModelChallengeBot.Models;
using ModelChallengeBot.Models.Queue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelChallengeBot.Queues
{
    public abstract class BaseQueue<T> : IQueue<T> where T: IQueueModel
    {
        private readonly ILogger logger;

        protected Queue<T> Queue { get; set; } = new Queue<T>();

        protected BaseQueue(ILogger logger)
        {
            this.logger = logger;
        }

        public Task Enqueue(T model)
        {
            ValidateModel(model);
            lock (Queue)
            {
                Queue.Enqueue(model);
                logger.LogInformation($"Queued render for {model.Id}");
            }
            return Task.CompletedTask;
        }

        public Task<T> Dequeue()
        {
            return Task<T>.Factory.StartNew(() =>
            {
                lock (Queue)
                {
                    var model = Queue.Dequeue();
                    logger.LogInformation($"Dequeued render for {model.Id}");
                    return model;
                }
            });
        }

        public Task<int> GetQueueLength()
        {
            return Task.FromResult(Queue.Count);
        }

        private void ValidateModel(T model)
        {
            //todo validate model
        }
    }
}
