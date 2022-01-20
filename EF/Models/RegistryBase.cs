using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelChallengeBot.EF.Models
{
    public abstract class RegistryBase : ModelBase
    {
        public ulong RegisterId { get; set; }
        public string Type { get; set; }
    }
}
