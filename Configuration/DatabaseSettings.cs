using ModelChallengeBot.Attributes;
using ModelChallengeBot.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelChallengeBot.Configuration
{
    [Section("Database")]
    public class DatabaseSettings : Settings
    {
        public string ConnectionString { get; set; }
        public string FileName { get; set; }
    }
}
