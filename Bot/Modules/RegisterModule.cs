using Discord;
using Discord.Commands;
using Microsoft.Extensions.Logging;
using ModelChallengeBot.Enums;
using ModelChallengeBot.Providers;
using ModelChallengeBot.Providers.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelChallengeBot.Bot.Modules
{
    public class RegisterModule : BotModuleBase
    {
        private readonly ILogger<RegisterModule> logger;
        private readonly IChannelTypeProvider channelTypeProvider;
        private readonly IRoleTypeProvider roleTypeProvider;

        public RegisterModule(ILogger<RegisterModule> logger, IChannelTypeProvider channelTypeProvider, IRoleTypeProvider roleTypeProvider) : base(logger)
        {
            this.logger = logger;
            this.channelTypeProvider = channelTypeProvider;
            this.roleTypeProvider = roleTypeProvider;
        }

        [Command("RegisterChannel")]
        public async Task RegisterChannel(string channel, string type)
        {
            if (!(await IsAdministrator()))
            {
                return;
            }

            if (!MentionUtils.TryParseChannel(channel, out ulong channelId))
            {
                await ReportError($"failed to parse channel {channel}");
                return;
            }
            if (!Enum.TryParse(type, ignoreCase: true, out ChallengeChannelType parsedChannelType))
            {
                await ReportError($"failed to parse channel type {type}");
                return;
            }

            await channelTypeProvider.AddOrOverwriteRegister(channelId, parsedChannelType);

            await ReplyUser($"Channel {MentionUtils.MentionChannel(channelId)} registered as {type}.");
        }

        [Command("RegisterRole")]
        public async Task RegisterRole(string role, string type) 
        {
            if (!(await IsAdministrator()))
            {
                return;
            }

            if (!MentionUtils.TryParseRole(role, out ulong roleId))
            {
                await ReportError($"failed to parse role {role}");
                return;
            }
            if (!Enum.TryParse(type, ignoreCase: true, out ChallengeRoleType parsedRoleType))
            {
                await ReportError($"failed to parse role type {type}");
                return;
            }

            await roleTypeProvider.AddOrOverwriteRegister(roleId, parsedRoleType);

            await ReplyUser($"Role {role} registered as {type}.");
        }
    }
}
