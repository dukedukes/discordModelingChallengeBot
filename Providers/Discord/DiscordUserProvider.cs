using Discord;
using Discord.WebSocket;
using ModelChallengeBot.Enums;
using ModelChallengeBot.Providers.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelChallengeBot.Providers.Discord
{
    public class DiscordUserProvider : IDiscordUserProvider
    {
        private readonly DiscordSocketClient discordSocketClient;
        private readonly IRoleTypeProvider roleTypeProvider;

        public DiscordUserProvider(DiscordSocketClient discordSocketClient, IRoleTypeProvider roleTypeProvider)
        {
            this.discordSocketClient = discordSocketClient;
            this.roleTypeProvider = roleTypeProvider;
        }

        public async Task SendUserDM(ulong userId, string message)
        {
            var user = await discordSocketClient.GetUserAsync(userId);
            var dmChannel = await user.CreateDMChannelAsync();
            await dmChannel.SendMessageAsync(message);
        }

        public async Task SendUserDM(ulong userId, string message, List<string> attachmentFilePaths)
        {
            var attachments = attachmentFilePaths.Select(filePath => new FileAttachment(filePath));
            var user = await discordSocketClient.GetUserAsync(userId);
            var dmChannel = await user.CreateDMChannelAsync();
            await dmChannel.SendFilesAsync(attachments, message);
        }

        public async Task<bool> UserHasRole(SocketUser user, ChallengeRoleType challengeRoleType)
        {
            var guildUser = user as SocketGuildUser;
            var participantRoleId = await roleTypeProvider.GetRegisterByType(challengeRoleType);
            return guildUser.Roles.Any(role => role.Id == participantRoleId);
        }
    }
}
