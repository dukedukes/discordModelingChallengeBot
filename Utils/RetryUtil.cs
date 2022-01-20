using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelChallengeBot.Utils
{
    public class RetryUtil
    {
        public static async Task<T> RetryResultRequest<T>(Func<Task<T>> actionToRetry, int retryAttempts = 3, int delayMilliseconds = 1000) where T : class
        {
            int currentAttempt = 0;
            T result = null;
            while (currentAttempt++ < retryAttempts && result == null)
            {
                result = await actionToRetry();
                if (result == null)
                {
                    await Task.Delay(delayMilliseconds);
                }
            }
            return result;
        }

        public static async Task<bool> RetryConditionRequest(Func<Task<bool>> conditionToMeet, int retryAttempts = 3, int delayMilliseconds = 1000)
        {
            int currentAttempt = 0;
            bool conditionMet = false;
            while (currentAttempt++ < retryAttempts && !conditionMet)
            {
                conditionMet = await conditionToMeet();
                if (!conditionMet)
                {
                    await Task.Delay(delayMilliseconds);
                }
            }
            return conditionMet;
        }
    }
}
