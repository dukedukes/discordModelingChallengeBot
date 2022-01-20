using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelChallengeBot.Models
{
    public class Instigator
    {
        public Instigator(ulong channelId, ulong userId, string username, ulong messageId)
        {
            ChannelId = channelId;
            UserId = userId;
            Username = username;
            MessageId = messageId;
        }

        public ulong ChannelId { get; set; }
        public ulong UserId { get; set; }
        public string Username { get; set; }
        public ulong MessageId { get; set; }
        public ulong? NotificationMessageId { get; set; }
    }
}
