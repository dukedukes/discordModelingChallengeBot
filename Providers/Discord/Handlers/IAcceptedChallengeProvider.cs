using Discord.WebSocket;
using System.Threading.Tasks;

namespace ModelChallengeBot.Providers.Discord.Handlers
{
    public interface IAcceptedChallengeProvider
    {
        Task HandleAcceptedChallenge(SocketMessageComponent socketMessageComponent);
    }
}