using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using ModelChallengeBot.EF.Models;
using ModelChallengeBot.Enums;
using ModelChallengeBot.Extensions;
using ModelChallengeBot.Providers.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelChallengeBot.Providers.Discord.Handlers
{
    public class AcceptedChallengeProvider : IAcceptedChallengeProvider
    {
        private readonly ILogger<AcceptedChallengeProvider> logger;
        private readonly DiscordSocketClient client;
        private readonly IModelingChallengeProvider modelingChallengeProvider;
        private readonly IDiscordUserProvider discordUserProvider;

        public AcceptedChallengeProvider(ILogger<AcceptedChallengeProvider> logger, DiscordSocketClient client, IModelingChallengeProvider modelingChallengeProvider, IDiscordUserProvider discordUserProvider)
        {
            this.logger = logger;
            this.client = client;
            this.modelingChallengeProvider = modelingChallengeProvider;
            this.discordUserProvider = discordUserProvider;
        }

        public async Task HandleAcceptedChallenge(SocketMessageComponent socketMessageComponent)
        {
            if (!await discordUserProvider.UserHasRole(socketMessageComponent.User, ChallengeRoleType.Participant))
            {
                logger.LogInformation($"User {socketMessageComponent.User.Username} tried to enter challenge while not being a participant");
                await discordUserProvider.SendUserDM(socketMessageComponent.Id, "You are not a participant");
                return;
            }
            var activeChallenge = await modelingChallengeProvider.GetChallengeByListing(socketMessageComponent.Message.Id);
            if (activeChallenge == null)
            {
                logger.LogError($"User {socketMessageComponent.User.Username} tried to enter challenge with listing id {socketMessageComponent.Message.Id} which didn't exist as a listing");
                await discordUserProvider.SendUserDM(socketMessageComponent.Id, "Failed to find challenge");
                return;
            }
            if (activeChallenge.ChallengeFinished)
            {
                logger.LogError($"User {socketMessageComponent.User.Username} tried to enter challenge which was already over");
                await discordUserProvider.SendUserDM(socketMessageComponent.User.Id, "This challenge is already over");
                return;
            }
            var existingAcceptor = activeChallenge.ChallengeAcceptors.SingleOrDefault(acceptor => acceptor.ChallengeAcceptor == socketMessageComponent.User.Id);
            if (existingAcceptor != null)
            {
                logger.LogInformation($"User {socketMessageComponent.User.Username} tried to reenter challenge");
                await discordUserProvider.SendUserDM(socketMessageComponent.User.Id, "You have already accepted this challenge");
                return;
            }
            var existingActiveChallenge = await modelingChallengeProvider.GetActiveChallengeByUser(socketMessageComponent.User.Id);
            if (existingActiveChallenge != null)
            {
                logger.LogInformation($"User {socketMessageComponent.User.Username} tried to enter a challenge while already in an active challenge. challenge id {existingActiveChallenge.ModelingChallengeId}");
                await discordUserProvider.SendUserDM(socketMessageComponent.User.Id, $"You are already participating in a challenge named '{existingActiveChallenge.ChallengeName}', you can't enter another challenge yet.");
                return;
            }
            
            DateTime challengeStartTime = DateTime.UtcNow;
            activeChallenge.ChallengeAcceptors.Add(new AcceptedChallenge() 
            {
                AcceptedTime = challengeStartTime,
                ChallengeAcceptor = socketMessageComponent.User.Id,
            });
            await modelingChallengeProvider.Update(activeChallenge);
            var challengeMessage = BuildChallengeMessage(activeChallenge, challengeStartTime);
            if (activeChallenge.Images.Any())
            {
                await discordUserProvider.SendUserDM(socketMessageComponent.User.Id, challengeMessage, activeChallenge.Images.Select(image => image.ImagePath).ToList());

            }
            else
            {
                await discordUserProvider.SendUserDM(socketMessageComponent.User.Id, challengeMessage);
            }
        }

        private string BuildChallengeMessage(ModelingChallenge modelingChallenge, DateTime startTime)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(".");
            stringBuilder.AppendLine($"Challenge '{modelingChallenge.ChallengeName}' begins now.");
            stringBuilder.AppendLine($"What you need to make: {modelingChallenge.ChallengeDescription}");
            stringBuilder.AppendLine($"Download the template.blend file here: https://discord.com/channels/931249644028186634/933162010785484852/933171038848692334 to work in if you haven't already. It's needed to render your output properly.");
            stringBuilder.AppendLine($"Your deadline is in: <t:{(startTime +  modelingChallenge.ChallengeDuration).ConvertToTimestamp()}:R>");
            stringBuilder.AppendLine($"Upload your completed challenge here");


            return stringBuilder.ToString();
        }
    }
}
