using System.Threading.Tasks;

namespace ModelChallengeBot.Providers
{
    public interface IRenderProvider
    {
        Task DeleteRenderOutput(string file);
        Task<string> Render(string blendFile);
        Task<string> Render(string blendFile, string newFileName);
    }
}