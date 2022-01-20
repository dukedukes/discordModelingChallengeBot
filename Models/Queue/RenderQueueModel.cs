using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelChallengeBot.Models.Queue
{
    public class RenderQueueModel : BaseQueueModel
    {
        public RenderQueueModel(string url, Instigator instigator)
        {
            Url = url;
            Instigator = instigator;
        }
        public string Url { get;}
        public Instigator Instigator { get; }
    }
}
