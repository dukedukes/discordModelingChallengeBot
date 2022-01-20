using ModelChallengeBot.EF.Models;
using System.Threading.Tasks;

namespace ModelChallengeBot.Providers.EF
{
    public interface IDbProvider<T> : IDbProvider where T : IDbModel
    {
        Task<T> Get(int identifier);
        Task<T> Update(T entity);
    }

    //Marker interface for discovery
    public interface IDbProvider
    {

    }
}