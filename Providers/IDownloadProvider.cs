using ModelChallengeBot.Models;
using System.Threading.Tasks;

namespace ModelChallengeBot.Providers
{
    public interface IDownloadProvider
    {
        Task<string> Download(string url);
    }
}