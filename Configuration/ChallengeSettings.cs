using ModelChallengeBot.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelChallengeBot.Configuration
{
    [Section("Challenge")]
    public class ChallengeSettings : Settings
    {
        public int MaxDurationDays { get; set; }
        public int MaxTimeLimitHours { get; set; }
        public int TimeLeftWarningMinutes { get; set; }
        public int CharacterLimit { get; set; }
    }
}
