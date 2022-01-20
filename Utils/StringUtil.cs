using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelChallengeBot.Utils
{
    public static class StringUtil
    {
        public static string GenerateUniqueName()
        {
            return Guid.NewGuid().ToString();
        }

        public static string GenerateShortUniqueName()
        {
            return Guid.NewGuid().ToString().Substring(0,10).Replace("-","");
        }
    }
}
