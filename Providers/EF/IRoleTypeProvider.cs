using ModelChallengeBot.EF.Models;
using ModelChallengeBot.Enums;

namespace ModelChallengeBot.Providers.EF
{
    public interface IRoleTypeProvider : IRegisterTypeProvider<RoleRegistry, ChallengeRoleType>
    {
    }
}