using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ModelChallengeBot.Workers
{
    public abstract class WorkerBase : BackgroundService, IWorker
    {
        private readonly ILogger logger;

        public abstract int TickDelayMilliseconds { get; }
        public WorkerBase(ILogger logger)
        {
            this.logger = logger;
        }

        protected sealed override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Initialize();
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await Tick();
                }
                catch (Exception ex)
                {
                    logger.LogError($"Exception running Tick. Exception: {ex}");
                }
                finally
                {
                    await Task.Delay(TickDelayMilliseconds);
                }
            }
            if (stoppingToken.IsCancellationRequested)
            {
                await Shutdown();
            }
        }
        protected abstract Task Initialize();
        protected abstract Task Tick();
        protected virtual Task Shutdown()
        {
            return Task.CompletedTask;
        }
    }
}
