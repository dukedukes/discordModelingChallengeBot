using ModelChallengeBot.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelChallengeBot.Configuration
{
    [Section("Workspace")]
    public class WorkspaceSettings : Settings
    {
        public string Directory { get; set; }
        public string RenderFolder { get; set; }
        public string DownloadFolder { get; set; }
        public string RenderDirectory => Combine(RenderFolder);
        public string DownloadDirectory => Combine(DownloadFolder);

        private string Combine(string folder)
        {
            return Path.Combine(Directory, folder);
        }
    }
}
