using System.Threading.Tasks;

namespace ModelChallengeBot.Providers
{
    public interface IProcessExecutionProvider
    {
        Task<bool> RunProcess(string path, string args);
    }
}