using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ModelChallengeBot.EF;
using ModelChallengeBot.EF.Models;
using ModelChallengeBot.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelChallengeBot.Providers.EF
{
    public class RoleTypeProvider : RegisterTypeProvider<RoleRegistry, ChallengeRoleType>, IRoleTypeProvider
    {
        public RoleTypeProvider(ILogger<RoleTypeProvider> logger, IDbContextFactory<BotContext> dbContextFactory) : base(logger, dbContextFactory)
        {
        }
    }
}
