using Microsoft.Extensions.Logging;
using ModelChallengeBot.EF.Models;
using ModelChallengeBot.Providers.Discord;
using ModelChallengeBot.Providers.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelChallengeBot.Providers
{
    public class SubmitChallengeProvider : ISubmitChallengeProvider
    {
        private readonly ILogger<SubmitChallengeProvider> logger;
        private readonly IModelingChallengeProvider modelingChallengeProvider;
        private readonly IDiscordUserProvider discordUserProvider;
        private readonly IDownloadProvider downloadProvider; 
        private static readonly List<string> BLEND_FILE_EXTENSIONS = new List<string> { ".blend", ".blend1" };

        public SubmitChallengeProvider(ILogger<SubmitChallengeProvider> logger, IModelingChallengeProvider modelingChallengeProvider, IDiscordUserProvider discordUserProvider, IDownloadProvider downloadProvider)
        {
            this.logger = logger;
            this.modelingChallengeProvider = modelingChallengeProvider;
            this.discordUserProvider = discordUserProvider;
            this.downloadProvider = downloadProvider;
        }

        public async Task SubmitChallenge(ulong userId, string attachmentUrl)
        {
            var activeChallenge = await modelingChallengeProvider.GetActiveChallengeByUser(userId);
            if (activeChallenge == null)
            {
                logger.LogInformation($"User {userId} tried to submit challenge without having an active challenge");
                return;
            }
            var downloadPath = await downloadProvider.Download(attachmentUrl);
            var extension = attachmentUrl.Substring(attachmentUrl.LastIndexOf('.'));
            if (!BLEND_FILE_EXTENSIONS.Contains(extension))
            {
                await discordUserProvider.SendUserDM(userId, "Attachment is not a blend file");
                return;
            }
            var challengeAcceptor = activeChallenge.ChallengeAcceptors.Single(acceptors => acceptors.ChallengeAcceptor == userId);
            var timeTaken = DateTime.UtcNow - challengeAcceptor.AcceptedTime;
            activeChallenge.Submissions.Add(new Submission() { FilePath = downloadPath, Submitter = userId, TimeTaken = timeTaken});
            await modelingChallengeProvider.Update(activeChallenge);
            await discordUserProvider.SendUserDM(userId, "File received");
        }
    }
}
