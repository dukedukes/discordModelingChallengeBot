using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelChallengeBot.Models.Queue
{
    public class BaseQueueModel : IQueueModel
    {
        public string Id => Guid.NewGuid().ToString();
    }
}
