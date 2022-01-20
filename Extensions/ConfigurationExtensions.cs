using Microsoft.Extensions.Configuration;
using ModelChallengeBot.Attributes;
using ModelChallengeBot.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelChallengeBot.Extensions
{
    public static class ConfigurationExtensions
    {
        public static object GetSettings(this IConfiguration configuration, Type t) 
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            var sectionAttribute = ReadSectionAttribute(t);
            if (sectionAttribute == null)
            {
                throw new Exception($"Failed to find SectionAttribute for type {t}");
            }
            var configurationSection = configuration.GetSection(sectionAttribute);
            if (configurationSection == null)
            {
                throw new Exception($"Failed to find configuration section for type {t}");
            }
            return configurationSection.Get(t);
        }
        public static T GetSettings<T>(this IConfiguration configuration) where T : Settings
        {
            return (T)GetSettings(configuration, typeof(T));
        }

        private static string ReadSectionAttribute(Type type)
        {
            var attribute = type.GetCustomAttributes(typeof(SectionAttribute), true).SingleOrDefault();
            if (attribute == null)
            {
                throw new Exception($"Setting {type.Name} is missing SectionAttribute decorator");
            }
            return ((SectionAttribute)attribute).Section;
        }
    }
}
