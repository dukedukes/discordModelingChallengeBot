using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelChallengeBot.EF.Models
{
    public class ChallengeImage : ModelBase
    {
        public int ChallengeImageId { get; set; }
        public string ImagePath { get; set; }
    }
}
