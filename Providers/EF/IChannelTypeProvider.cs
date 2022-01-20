using ModelChallengeBot.EF.Models;
using ModelChallengeBot.Enums;
using ModelChallengeBot.Providers.EF;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ModelChallengeBot.Providers.EF
{
    public interface IChannelTypeProvider : IRegisterTypeProvider<ChannelRegistry, ChallengeChannelType>
    {
    }
}