using ModelChallengeBot.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelChallengeBot.Configuration
{
    [Section("Blender")]
    public class BlenderSettings : Settings
    {
        public string Path { get; set; }
        public string RenderArgs { get; set; }
        public string OutputSuffix { get; set; }
    }
}
