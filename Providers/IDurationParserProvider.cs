using System;
using System.Threading.Tasks;

namespace ModelChallengeBot.Providers
{
    public interface IDurationParserProvider
    {
        Task<TimeSpan?> ParseDuration(string durationToParse);
        Task<string> ValidateDurationString(string durationToValidate);
    }
}