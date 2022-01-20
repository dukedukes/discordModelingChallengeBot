using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelChallengeBot.EF.Models
{
    public class AcceptedChallenge : ModelBase
    {
        public int AcceptedChallengeId { get; set; }
        public ulong ChallengeAcceptor { get; set; }
        public DateTime AcceptedTime { get; set; }
        public bool NotifiedTimeLeft { get; set; }
    }
}
