using System.Threading.Tasks;

namespace ModelChallengeBot.Providers
{
    public interface ISubmitChallengeProvider
    {
        Task SubmitChallenge(ulong userId, string attachmentUrl);
    }
}