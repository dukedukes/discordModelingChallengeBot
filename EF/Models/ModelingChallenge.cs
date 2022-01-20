using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelChallengeBot.EF.Models
{
    public class ModelingChallenge : ModelBase
    {
        public int ModelingChallengeId { get; set; }
        public string ChallengeName { get; set; }
        public string ChallengeDescription { get; set; }
        public int ChallengeDurationMinutes { get; set; }
        public DateTime EndDate { get; set; }
        public List<ChallengeImage> Images { get; set; } = new List<ChallengeImage>();
        public List<AcceptedChallenge> ChallengeAcceptors { get; set; } = new List<AcceptedChallenge>();
        public List<Submission> Submissions { get; set; } = new List<Submission>();
        public ulong? ListingId { get; set; }
        public bool ChallengeFinished { get; set; }
        public ulong? ChallengeFinishedThreadId { get; set; }

        public TimeSpan ChallengeDuration => TimeSpan.FromMinutes(ChallengeDurationMinutes);
    }
}
