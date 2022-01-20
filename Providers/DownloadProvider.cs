using Microsoft.Extensions.Logging;
using ModelChallengeBot.Configuration;
using ModelChallengeBot.Utils;
using ModelChallengeBot.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ModelChallengeBot.Providers
{
    public class DownloadProvider : IDownloadProvider
    {
        private readonly ILogger<DownloadProvider> logger;
        private readonly WorkspaceSettings workspaceSettings;

        public DownloadProvider(ILogger<DownloadProvider> logger, WorkspaceSettings workspaceSettings)
        {
            this.logger = logger;
            this.workspaceSettings = workspaceSettings;
        }
        public async Task<string> Download(string url)
        {
            return await Download(url, "");
        }
        private async Task<string> Download(string url, string requiredMediaType = "")
        {
            var extension = url.Substring(url.LastIndexOf('.'));
            string filePath = workspaceSettings.DownloadDirectory + "\\" + StringUtil.GenerateUniqueName() + extension;
            using (HttpClient client = new HttpClient())
            {
                var resp = await client.GetAsync(url);
                string mediaType = resp.Content.Headers.ContentType.MediaType;
                if (!string.IsNullOrEmpty(requiredMediaType))
                {
                    throw new NotImplementedException("required media type unimplemented");
                }
                using (FileStream fileStream = new FileStream(filePath, FileMode.CreateNew))
                {
                    await resp.Content.CopyToAsync(fileStream);
                }
            }

            return filePath;
        }
    }
}
