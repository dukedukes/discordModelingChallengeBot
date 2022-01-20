using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using ModelChallengeBot.Configuration;
using ModelChallengeBot.EF.Models;
using ModelChallengeBot.Extensions;
using ModelChallengeBot.Providers;
using ModelChallengeBot.Providers.Discord;
using ModelChallengeBot.Providers.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelChallengeBot.Workers
{
    public class ChallengeWorker : WorkerBase
    {
        private readonly ILogger<ChallengeWorker> logger;
        private readonly DiscordSocketClient discordSocketClient;
        private readonly IModelingChallengeProvider modelingChallengeProvider;
        private readonly IDiscordUserProvider discordUserProvider;
        private readonly IChannelTypeProvider channelTypeProvider;
        private readonly IRenderProvider renderProvider;
        private readonly DiscordSettings discordSettings;

        public ChallengeWorker(ILogger<ChallengeWorker> logger,
                               DiscordSocketClient discordSocketClient,
                               IModelingChallengeProvider modelingChallengeProvider,
                               IDiscordUserProvider discordUserProvider,
                               IChannelTypeProvider channelTypeProvider,
                               IRenderProvider renderProvider,
                               DiscordSettings discordSettings) : base(logger)
        {
            this.logger = logger;
            this.discordSocketClient = discordSocketClient;
            this.modelingChallengeProvider = modelingChallengeProvider;
            this.discordUserProvider = discordUserProvider;
            this.channelTypeProvider = channelTypeProvider;
            this.renderProvider = renderProvider;
            this.discordSettings = discordSettings;
        }

        public override int TickDelayMilliseconds => 5000;
        
        protected override Task Initialize()
        {
            return Task.CompletedTask;
        }

        protected async override Task Tick()
        {
            if (discordSocketClient.ConnectionState != ConnectionState.Connected)
            {
                return;
            }
            var modelingChallengesToRemove = await modelingChallengeProvider.GetChallengesToComplete();
            var unlistedChallenges = await modelingChallengeProvider.GetAllUnlistedChallenges();
            if (!modelingChallengesToRemove.Any() && !unlistedChallenges.Any())
            {
                logger.LogDebug("No modeling challenges to update");
                return;
            }
            var channelId = await channelTypeProvider.GetRegisterByType(Enums.ChallengeChannelType.Challenges);
            if (channelId == null)
            {
                logger.LogDebug("Challenge worker has no channel to post to");
                return;
            }
            var channel = await discordSocketClient.GetChannelAsync(channelId.Value) as ISocketMessageChannel;
            if (channel == null)
            {
                logger.LogError("Failed to retrieve challenge channel");
                return;
            }

            await ProcessUnlistedChallenges(channel, unlistedChallenges);
            await CompleteEndedChallenges(channel, modelingChallengesToRemove);
        }


        private async Task CompleteEndedChallenges(ISocketMessageChannel channel, List<ModelingChallenge> challengesToComplete)
        {
            foreach (var challengeToComplete in challengesToComplete)
            {
                await PostCompletedChallenge(challengeToComplete);
                await RemoveFromActiveChallenges(channel, challengeToComplete);
            }
        }

        private async Task PostCompletedChallenge(ModelingChallenge challengeToPost)
        {
            var submissions = challengeToPost.Submissions;
            if (!submissions.Any())
            {
                challengeToPost.ChallengeFinished = true;
                await modelingChallengeProvider.Update(challengeToPost);
                return;
            }

            IThreadChannel threadChannel = await GetOrCreateFinishedThread(challengeToPost);
            if (submissions.Any())
            {
                await threadChannel.SendMessageAsync(BuildChallengeText(challengeToPost));
            }

            var socketChannel = threadChannel as ISocketMessageChannel;
            foreach (var submission in submissions)
            {
                try
                {
                    var resultPath = await renderProvider.Render(submission.FilePath);

                    await socketChannel.SendFileAsync(resultPath, $"Submitted by {MentionUtils.MentionUser(submission.Submitter)}. Challenge duration {submission.TimeTaken.ToReadableFormat()}");
                }
                catch (Exception ex)
                {
                    string errorMessage = $"failed to render user file for challenge: {challengeToPost.ModelingChallengeId} file: {submission.FilePath} exception {ex}";
                    logger.LogError(errorMessage);
                    await discordUserProvider.SendUserDM(discordSettings.Admin, errorMessage);
                    continue;
                }
            }
        }

        private async Task<IThreadChannel> GetOrCreateFinishedThread(ModelingChallenge challengeToPost)
        {
            if (challengeToPost.ChallengeFinishedThreadId != null)
            {
                
                var guild = discordSocketClient.GetGuild(discordSettings.Guild);
                return guild.GetThreadChannel(challengeToPost.ChallengeFinishedThreadId.Value);
            }

            var resultChannel = await channelTypeProvider.GetRegisterByType(Enums.ChallengeChannelType.Results);
            if (resultChannel == null)
            {
                throw new Exception("No result channel to post results in");
            }
            var channel = await discordSocketClient.GetChannelAsync(resultChannel.Value) as ITextChannel;

            var threadChannel = await channel.CreateThreadAsync($"Challenge {challengeToPost.ModelingChallengeId}");
            challengeToPost.ChallengeFinishedThreadId = threadChannel.Id;
            await modelingChallengeProvider.Update(challengeToPost);
            return threadChannel;
        }

        private async Task RemoveFromActiveChallenges(ISocketMessageChannel channel, ModelingChallenge challenge)
        {
            await channel.DeleteMessageAsync(challenge.ListingId.Value);
            challenge.ChallengeFinished = true;
            await modelingChallengeProvider.Update(challenge);
        }

        private async Task ProcessUnlistedChallenges(ISocketMessageChannel channel, List<ModelingChallenge> unlistedChallenges)
        {
            foreach (var unlistedChallenge in unlistedChallenges)
            {
                if (unlistedChallenge.EndDate < DateTime.UtcNow)
                {
                    await RemoveFromActiveChallenges(channel, unlistedChallenge);
                    continue;
                }

                await PostChallenge(channel, unlistedChallenge);
            }
        }

        private async Task PostChallenge(ISocketMessageChannel channel, ModelingChallenge modelingChallenge)
        {
            var builder = new ComponentBuilder()
                .WithButton("Start", "start-challenge");
            var message = await channel.SendMessageAsync(BuildChallengeText(modelingChallenge), components: builder.Build());
            modelingChallenge.ListingId = message.Id;
            await modelingChallengeProvider.Update(modelingChallenge);
        }

        private string BuildChallengeText(ModelingChallenge modelingChallenge)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(".");
            stringBuilder.AppendLine($"Name: {modelingChallenge.ChallengeName}");
            stringBuilder.AppendLine($"Duration: {modelingChallenge.ChallengeDuration.ToReadableFormat()}");
            stringBuilder.AppendLine($"End Date: <t:{modelingChallenge.EndDate.ConvertToTimestamp()}:R>");

            return stringBuilder.ToString();
        }
    }
}
