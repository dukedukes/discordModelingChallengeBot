using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelChallengeBot.Attributes
{
    public class ConsumesAttachmentAttribute : Attribute
    {
        public ConsumesAttachmentAttribute(string attachmentType)
        {
            AttachmentType = attachmentType;
        }

        public string AttachmentType { get; }
    }
}
