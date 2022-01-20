using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using ModelChallengeBot.Attributes;
using ModelChallengeBot.Configuration;
using ModelChallengeBot.EF.Models;
using ModelChallengeBot.Extensions;
using ModelChallengeBot.Providers;
using ModelChallengeBot.Providers.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelChallengeBot.Bot.Modules
{
    public class OrganizerModule : BotModuleBase
    {
        private readonly ILogger<OrganizerModule> logger;
        private readonly IModelingChallengeProvider modelingChallengeProvider;
        private readonly IDurationParserProvider durationParserProvider;
        private readonly ChallengeSettings challengeSettings;
        private readonly IDownloadProvider downloadProvider;
        private readonly IRoleTypeProvider roleTypeProvider;
        private readonly IChannelTypeProvider channelTypeProvider;

        public OrganizerModule(ILogger<OrganizerModule> logger,
            IModelingChallengeProvider modelingChallengeProvider,
            IDurationParserProvider durationParserProvider,
            ChallengeSettings challengeSettings,
            IDownloadProvider downloadProvider,
            IRoleTypeProvider roleTypeProvider,
            IChannelTypeProvider channelTypeProvider) : base(logger)
        {
            this.logger = logger;
            this.modelingChallengeProvider = modelingChallengeProvider;
            this.durationParserProvider = durationParserProvider;
            this.challengeSettings = challengeSettings;
            this.downloadProvider = downloadProvider;
            this.roleTypeProvider = roleTypeProvider;
            this.channelTypeProvider = channelTypeProvider;
            
        }

        [Command("CreateChallenge")]
        [ConsumesAttachment("Images")]
        public async Task CreateChallenge(string challengePublicName, string challengeDescription, string timeLimitExpression, string challengeDurationExpression)
        {
            if (!(await IsAuthorized()))
            {
                return;
            }
            if (!(await IsUploadChannel()))
            {
                return;
            }
            if (challengePublicName.Length > challengeSettings.CharacterLimit)
            {
                await ReportError($"challenge public name must be under {challengeSettings.CharacterLimit} characters long");
                return;
            }
            if (challengeDescription.Length > challengeSettings.CharacterLimit)
            {
                await ReportError($"challenge description must be under {challengeSettings.CharacterLimit} characters long");
                return;
            }

            var challenge = new ModelingChallenge()
            {
                ChallengeName = challengePublicName,
                ChallengeDescription = challengeDescription,
            };
            var timeLimitResult = await ProcessDurationExpression(timeLimitExpression, TimeSpan.FromHours(challengeSettings.MaxTimeLimitHours), "Time Limit");
            if (!timeLimitResult.success)
            {
                return;
            }
            var challengeDurationResult = await ProcessDurationExpression(challengeDurationExpression, TimeSpan.FromDays(challengeSettings.MaxDurationDays), "Challenge Duration");
            if (!challengeDurationResult.success)
            {
                return;
            }

            challenge.ChallengeDurationMinutes = (int)timeLimitResult.result.TotalMinutes;
            challenge.EndDate = DateTime.UtcNow + challengeDurationResult.result;
            foreach (var attachment in Context.Message.Attachments)
            {
                var downloadPath = await downloadProvider.Download(attachment.Url);
                challenge.Images.Add(new ChallengeImage() { ImagePath = downloadPath });
            }
            await modelingChallengeProvider.Add(challenge);
            
            await ReplyAsync($"Challenge created. Id is {challenge.ModelingChallengeId}");
        }

        [Command("RemoveChallenge")]
        public async Task RemoveChallenge(int challengeId)
        {
            if (!await IsAuthorized())
            {
                return;
            }
            var modelingChallengeToDelete = await modelingChallengeProvider.Get(challengeId);
            if (modelingChallengeToDelete == null)
            {
                await ReportError("Challenge either does not exist or is not active");
                return;
            }
            if (modelingChallengeToDelete.ListingId != null)// clean up its listing
            {
                var channelId = await channelTypeProvider.GetRegisterByType(Enums.ChallengeChannelType.Challenges);
                var channel = await Context.Client.GetChannelAsync(channelId.Value) as ISocketMessageChannel;
                if (channel != null)
                {
                    await channel.DeleteMessageAsync(modelingChallengeToDelete.ListingId.Value);
                }
            }
            await modelingChallengeProvider.Delete(challengeId);
            await ReplyUser("Challenge deleted");
        }

        [Command("GetActiveChallenges")]
        public async Task GetActiveChallenges()
        {
            if (!(await IsAuthorized()))
            {
                return;
            }
            var modelingChallenges = await modelingChallengeProvider.GetAllActiveChallenges();
            if (modelingChallenges == null || modelingChallenges.Count == 0)
            {
                await ReplyUser("No active challenges");
                return;
            }
            StringBuilder responseMessage = new StringBuilder();
            responseMessage.AppendLine("Active Challenges");
            foreach (var modelingChallenge in modelingChallenges)
            {
                responseMessage.AppendLine($"> Id: {modelingChallenge.ModelingChallengeId} Name: {modelingChallenge.ChallengeName}");
            }
            await ReplyAsync(responseMessage.ToString());
        }

        private async Task<bool> IsUploadChannel()
        {
            bool canContinue = true;
            var challengeUploadChannelId = await channelTypeProvider.GetRegisterByType(Enums.ChallengeChannelType.ChallengeUpload);
            if (Context.Message.Channel.Id != challengeUploadChannelId)
            {
                await Context.Message.Channel.DeleteMessageAsync(Context.Message.Id);
                await ReportError("Send this message to the challenge upload channel");
                canContinue = false;
            }
            return canContinue;
        }

        private async Task<bool> IsAuthorized()
        {
            bool canContinue = true;
            var guildUser = Context.User as SocketGuildUser;
            var organizerRoleId = await roleTypeProvider.GetRegisterByType(Enums.ChallengeRoleType.Organizer);
            if (!guildUser.Roles.Any(role => role.Id == organizerRoleId))
            {
                await Context.Message.Channel.DeleteMessageAsync(Context.Message.Id);
                await ReportError("You must be an organizer to use this command");
                canContinue = false;
                return canContinue;
            }
            return canContinue;
        }

        private async Task<(bool success, TimeSpan result)> ProcessDurationExpression(string durationExpression, TimeSpan maxDuration, string descriptor)
        {
            bool success = true;
            TimeSpan result = TimeSpan.Zero;

            var durationValidationResult = await durationParserProvider.ValidateDurationString(durationExpression);
            if (durationValidationResult != null)
            {
                await ReportError($"{descriptor} {durationValidationResult}");
                success = false;
                return (success, result);
            }
            var timeLimit = await durationParserProvider.ParseDuration(durationExpression);
            if (timeLimit == null)
            {
                await ReportAndThrowInternalError($"failed to parse duration expression. value {durationExpression}");
            }
            result = timeLimit.Value;

            if (result >= maxDuration)
            {
                await ReportError($"{descriptor} is too long. Must be under {maxDuration.ToReadableFormat()}");
                success = false;
                return (success, result);
            }

            return (success, result);
        }
    }
}
