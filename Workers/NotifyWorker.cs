using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using ModelChallengeBot.Configuration;
using ModelChallengeBot.Extensions;
using ModelChallengeBot.Providers.Discord;
using ModelChallengeBot.Providers.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelChallengeBot.Workers
{
    public class NotifyWorker : WorkerBase
    {
        private readonly DiscordSocketClient discordSocketClient;
        private readonly IModelingChallengeProvider modelingChallengeProvider;
        private readonly IDiscordUserProvider discordUserProvider;
        private readonly ChallengeSettings challengeSettings;

        public NotifyWorker(ILogger<NotifyWorker> logger, DiscordSocketClient discordSocketClient, IModelingChallengeProvider modelingChallengeProvider, IDiscordUserProvider discordUserProvider, ChallengeSettings challengeSettings) : base(logger)
        {
            this.discordSocketClient = discordSocketClient;
            this.modelingChallengeProvider = modelingChallengeProvider;
            this.discordUserProvider = discordUserProvider;
            this.challengeSettings = challengeSettings;
        }

        public override int TickDelayMilliseconds => 1000 * 60;

        protected override Task Initialize()
        {
            return Task.CompletedTask;
        }

        protected async override Task Tick()
        {
            if (discordSocketClient.ConnectionState != Discord.ConnectionState.Connected)
            {
                return;
            }
            await NotifyTimeRemaining();
        }

        private async Task NotifyTimeRemaining()
        {
            var activeChallenges = await modelingChallengeProvider.GetAllActiveChallenges();
            var timeLeftWarning = TimeSpan.FromMinutes(challengeSettings.TimeLeftWarningMinutes);
            foreach (var activeChallenge in activeChallenges)
            {
                if (!activeChallenge.ChallengeAcceptors.Any())
                {
                    return;
                }

                foreach (var challengeAcceptor in activeChallenge.ChallengeAcceptors)
                {
                    if (challengeAcceptor.NotifiedTimeLeft)
                    {
                        continue;
                    }
                    var timeLeft = activeChallenge.ChallengeDuration - (DateTime.UtcNow - challengeAcceptor.AcceptedTime);
                    if (timeLeft > TimeSpan.Zero && timeLeftWarning > timeLeft)
                    {
                        await discordUserProvider.SendUserDM(challengeAcceptor.ChallengeAcceptor, $"You have {timeLeftWarning.ToReadableFormat()} left in the challenge");
                        challengeAcceptor.NotifiedTimeLeft = true;
                        await modelingChallengeProvider.Update(activeChallenge);
                    }
                }
            }
        }
    }
}
