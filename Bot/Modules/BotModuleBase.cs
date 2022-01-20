using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using ModelChallengeBot.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelChallengeBot.Bot.Modules
{
    public abstract class BotModuleBase : ModuleBase<SocketCommandContext>
    {
        private readonly ILogger logger;

        public BotModuleBase(ILogger logger)
        {
            this.logger = logger;
        }

        protected async Task ReportAndThrowInternalError(string error)
        {
            var correlationId = StringUtil.GenerateShortUniqueName();
            var correlationString = $"correlation id: {correlationId}";
            await ReplyAsync($"internal error. {correlationString}");
            throw new Exception($"{error} {correlationString}");
        }

        protected async Task ReportError(string error)
        {
            var errorResponse = string.Concat($"{MentionUtils.MentionUser(Context.User.Id)}: ", error);
            await ReplyAsync(errorResponse);
        }

        protected async Task ReplyUser(string message)
        {
            var messageResponse = string.Concat($"{MentionUtils.MentionUser(Context.User.Id)}: ", message);
            await ReplyAsync(messageResponse);
        }

        protected async Task<bool> IsAdministrator()
        {
            var socketUser = Context.User as SocketGuildUser;
            if (socketUser == null)
            {
                await ReportAndThrowInternalError("failed to cast user to SocketGuildUser");
            }
            return socketUser.GuildPermissions.Administrator;
        }
    }
}
