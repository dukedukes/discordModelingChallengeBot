using ModelChallengeBot.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelChallengeBot.Extensions
{
    public static class DatabaseSettingsExtensions
    {
        public static string BuildConnectionString(this DatabaseSettings settings, string workspace)
        {
            string path = Path.Join(workspace, settings.FileName);

            return string.Format(settings.ConnectionString, path);
        }
    }
}
