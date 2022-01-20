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
    public abstract class RegisterTypeProvider<T, TEnum> : DbProvider<T> 
        where T : RegistryBase, new()
        where TEnum : struct, IConvertible
    {
        public RegisterTypeProvider(ILogger logger, IDbContextFactory<BotContext> dbContextFactory) :base(dbContextFactory)
        {
            if (!typeof(TEnum).IsEnum)
            {
                throw new ArgumentException("TEnum must be an enumerated type");
            }
        }

        public async Task<ulong?> GetRegisterByType(TEnum registerType)
        {
            using (var dbContext = await dbContextFactory.CreateDbContextAsync())
            {
                var foundRegister = await dbContext.Set<T>().SingleOrDefaultAsync(register => register.Type == registerType.ToString());
                return foundRegister?.RegisterId;
            }
        }

        public async Task AddOrOverwriteRegister(ulong registerId, TEnum type)
        {
            using (var dbContext = await dbContextFactory.CreateDbContextAsync())
            {
                T registerInUse = await dbContext.Set<T>().SingleOrDefaultAsync(register => register.RegisterId == registerId);
                if (registerInUse != null)
                {
                    dbContext.Remove(registerInUse);
                }
                var existingRecord = await dbContext.Set<T>().SingleOrDefaultAsync(register => register.Type == type.ToString());
                if (existingRecord != null)
                {
                    existingRecord.RegisterId = registerId;
                }
                else
                {
                    var newRecord = new T() { RegisterId = registerId, Type = type.ToString() };
                    dbContext.Add(newRecord);
                }
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task<List<T>> GetRegistrations()
        {
            using (var dbContext = await dbContextFactory.CreateDbContextAsync())
            {
                return await dbContext.Set<T>().ToListAsync(); 
            }
        }
    }
}
