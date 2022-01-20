using ModelChallengeBot.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelChallengeBot.Configuration
{
   [Section("Discord")]
   public class DiscordSettings : Settings
   {
        public string CommandPrefix { get; set; }
        public string Secret { get; set; }
        public ulong Admin { get; set; }
        public ulong Guild { get; set; }
   }
}
