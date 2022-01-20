using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelChallengeBot.Attributes
{
   [AttributeUsage(AttributeTargets.Class)]
   public class SectionAttribute : Attribute
   {
      public string Section { get; set; }

      public SectionAttribute(string section)
      {
         this.Section = section;
      }
   }
}
