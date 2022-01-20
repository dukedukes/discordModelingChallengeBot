using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelChallengeBot.EF.Models
{
    public class Submission : ModelBase
    {
        public int SubmissionId { get; set; }
        public ulong Submitter { get; set; }
        public TimeSpan TimeTaken { get; set; }
        public string FilePath { get; set; }
    }
}
