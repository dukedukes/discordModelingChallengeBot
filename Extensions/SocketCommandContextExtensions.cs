using Discord.Commands;
using ModelChallengeBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelChallengeBot.Extensions
{
    public static class SocketCommandContextExtensions
    {
        public static Instigator BuildInstigator(this SocketCommandContext context)
        {
            return new Instigator(context.Message.Channel.Id, context.User.Id, context.User.Username, context.Message.Id);
        }
    }
}
