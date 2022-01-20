using Discord.WebSocket;
using ModelChallengeBot.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ModelChallengeBot.Providers.Discord
{
    public interface IDiscordUserProvider
    {
        Task SendUserDM(ulong userId, string message);
        Task SendUserDM(ulong userId, string message, List<string> attachmentFilePaths);
        Task<bool> UserHasRole(SocketUser user, ChallengeRoleType challengeRoleType);
    }
}