using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ModelChallengeBot.EF;
using ModelChallengeBot.EF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelChallengeBot.Providers.EF
{
    public class ModelingChallengeProvider : DbProvider<ModelingChallenge>, IModelingChallengeProvider
    {
        private readonly ILogger<ModelingChallengeProvider> logger;

        public ModelingChallengeProvider(ILogger<ModelingChallengeProvider> logger, IDbContextFactory<BotContext> dbContextFactory) : base(dbContextFactory)
        {
            this.logger = logger;
        }

        public async Task Add(ModelingChallenge modelingChallenge) 
        {
            using (var dbContext = dbContextFactory.CreateDbContext())
            {
                await dbContext.AddAsync(modelingChallenge);
                await dbContext.SaveChangesAsync();
            }
        }
        public  async override Task<ModelingChallenge> Get(int identifier)
        {
            var challenge = await base.Get(identifier);
            if (challenge != null && IsActiveChallenge(challenge))
            {
                return challenge;
            }
            return null;
        }

        public async Task<ModelingChallenge> GetChallengeByListing(ulong listingId)
        {
            using (var dbContext = await dbContextFactory.CreateDbContextAsync())
            {
                return await dbContext.ModelingChallenge
                    .Include(challenge=>challenge.Images)
                    .Include(challenge=>challenge.ChallengeAcceptors)
                    .AsSplitQuery()
                    .SingleOrDefaultAsync(modelingChallenge => modelingChallenge.ListingId == listingId);
            }
        }

        public async Task<List<ModelingChallenge>> GetAllActiveChallenges()
        {
            using (var dbContext = dbContextFactory.CreateDbContext())
            {
                return await dbContext.ModelingChallenge.Include(challenge => challenge.ChallengeAcceptors).Where(modelingChallenge => modelingChallenge.EndDate > DateTime.UtcNow).ToListAsync();
            }
        }

        public async Task<List<ModelingChallenge>> GetAllUnlistedChallenges()
        {
            using (var dbContext = dbContextFactory.CreateDbContext())
            {
                return await dbContext.ModelingChallenge.Where(modelingChallenge => modelingChallenge.EndDate > DateTime.UtcNow && modelingChallenge.ListingId == null).ToListAsync();
            }
        }

        public async Task<ModelingChallenge> GetActiveChallengeByUser(ulong userId)
        {
            using (var dbContext = await  dbContextFactory.CreateDbContextAsync())
            {
                return await dbContext.ModelingChallenge
                    .Include(challenge => challenge.Submissions)
                    .Include(challenge => challenge.ChallengeAcceptors)
                    .AsSplitQuery()
                    .SingleOrDefaultAsync(challenge => 
                        !challenge.ChallengeFinished 
                        && challenge.ChallengeAcceptors.Any(acceptors => acceptors.ChallengeAcceptor == userId)
                        && !challenge.Submissions.Any(submissions => submissions.Submitter == userId));
            }
        }

        public async Task<List<ModelingChallenge>> GetChallengesToComplete()
        {
            using (var dbContext = dbContextFactory.CreateDbContext())
            {
                return await dbContext.ModelingChallenge.Include(challenge => challenge.Submissions).Where(modelingChallenge => modelingChallenge.EndDate < DateTime.UtcNow && modelingChallenge.ListingId != null && !modelingChallenge.ChallengeFinished).ToListAsync();
            }
        }

        private bool IsActiveChallenge(ModelingChallenge modelingChallenge)
        {
            return modelingChallenge.EndDate > DateTime.UtcNow;
        }

        public async Task Delete(int modelingChallengeId)
        {
            using (var dbContext = dbContextFactory.CreateDbContext())
            {
                var modelingChallenge = await dbContext.ModelingChallenge.FindAsync(modelingChallengeId);
                dbContext.ModelingChallenge.Remove(modelingChallenge);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
