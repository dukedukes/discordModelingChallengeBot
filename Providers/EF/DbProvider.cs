using Microsoft.EntityFrameworkCore;
using ModelChallengeBot.EF;
using ModelChallengeBot.EF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelChallengeBot.Providers.EF
{
    public abstract class DbProvider<T> : IDbProvider<T> where T : class, IDbModel
    {
        protected readonly IDbContextFactory<BotContext> dbContextFactory;

        public DbProvider(IDbContextFactory<BotContext> dbContextFactory)
        {
            this.dbContextFactory = dbContextFactory;
        }

        public virtual async Task<T> Get(int identifier)
        {
            using (var dbContext = dbContextFactory.CreateDbContext())
            {
                return await dbContext.Set<T>().FindAsync(identifier);
            }
        }

        public virtual async Task<T> Update(T entity)
        {
            using (var dbContext = dbContextFactory.CreateDbContext())
            {
                dbContext.Set<T>().Update(entity);
                await dbContext.SaveChangesAsync();
                return entity;
            }
        }
    }
}
