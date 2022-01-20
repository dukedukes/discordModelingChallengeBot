using ModelChallengeBot.EF.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ModelChallengeBot.Providers.EF
{
    public interface IModelingChallengeProvider : IDbProvider<ModelingChallenge>
    {
        Task Add(ModelingChallenge modelingChallenge);
        Task Delete(int modelingChallengeId);
        Task<ModelingChallenge> GetActiveChallengeByUser(ulong userId);
        Task<List<ModelingChallenge>> GetAllActiveChallenges();
        Task<List<ModelingChallenge>> GetAllUnlistedChallenges();
        Task<ModelingChallenge> GetChallengeByListing(ulong listingId);
        Task<List<ModelingChallenge>> GetChallengesToComplete();
    }
}