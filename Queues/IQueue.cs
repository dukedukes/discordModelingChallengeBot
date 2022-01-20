using ModelChallengeBot.Models;
using ModelChallengeBot.Models.Queue;
using System.Threading.Tasks;

namespace ModelChallengeBot.Queues
{
    public interface IQueue<T>: IQueue where T : IQueueModel
    {
        Task<T> Dequeue();
        Task Enqueue(T model);
        Task<int> GetQueueLength();
    }
    // Just a marker class for easy type discovery
    public interface IQueue
    {
    }
}