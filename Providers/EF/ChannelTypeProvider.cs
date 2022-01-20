using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ModelChallengeBot.EF;
using ModelChallengeBot.EF.Models;
using ModelChallengeBot.Enums;
using ModelChallengeBot.Providers.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelChallengeBot.Providers.EF
{
    public class ChannelTypeProvider : RegisterTypeProvider<ChannelRegistry, ChallengeChannelType>, IChannelTypeProvider
    {
        public ChannelTypeProvider(ILogger<ChannelTypeProvider> logger, IDbContextFactory<BotContext> dbContextFactory) : base(logger, dbContextFactory)
        {
        }
    }
}
