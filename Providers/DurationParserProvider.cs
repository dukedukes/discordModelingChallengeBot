using Microsoft.Extensions.Logging;
using ModelChallengeBot.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ModelChallengeBot.Providers
{
    public class DurationParserProvider : IDurationParserProvider
    {
        private readonly ILogger<DurationParserProvider> logger;

        public DurationParserProvider(ILogger<DurationParserProvider> logger)
        {
            this.logger = logger;
        }
        public Task<string> ValidateDurationString(string durationToValidate)
        {
            string errorResponse = null;
            if (string.IsNullOrEmpty(durationToValidate))
            {
                errorResponse = "is empty";
            }
            else if (durationToValidate.Length > 9)
            {
                errorResponse = "has too many characters";
            }
            else if (!Regex.Match(durationToValidate, "^[0-9][dhm0-9]+").Success || !Regex.Match(durationToValidate, "[dhm]").Success)
            {
                errorResponse = "has an invalid format";
            }
            else if ("dhm".Any((timeClassifier) => durationToValidate.Count( durationCharacter => durationCharacter == timeClassifier) > 1))
            {
                errorResponse = "contains multiple of the same time classifier";
            }
            if (errorResponse != null)
            {
                logger.LogInformation($"duration string: {durationToValidate} {errorResponse}");
            }

            return Task.FromResult(errorResponse);
        }

        public Task<TimeSpan?> ParseDuration(string durationToParse)
        {
            var duration = DurationParser.ParseDuration(durationToParse);

            return Task.FromResult(duration);
        }
    }
    internal class DurationParser
    {
        private string parseString;
        private string TimeClassifier => "dhm";
        private class DurationModel
        {
            public int DurationDay { get; set; }
            public int DurationHour { get; set; }
            public int DurationMinute { get; set; }

            public TimeSpan BuildTimeSpan()
            {
                TimeSpan timeSpan = TimeSpan.Zero;
                timeSpan += TimeSpan.FromDays(DurationDay);
                timeSpan += TimeSpan.FromHours(DurationHour);
                timeSpan += TimeSpan.FromMinutes(DurationMinute);
                return timeSpan;
            }
        }
        public static TimeSpan? ParseDuration(string durationToParse)
        {
            return new DurationParser(durationToParse).Parse();
        }
        private DurationParser(string parseString)
        {
            this.parseString = parseString;
        }
        private TimeSpan? Parse()
        {
            DurationModel durationModel = new DurationModel();
            TimeSpan duration = TimeSpan.Zero;

            string currentNumeric = "";
            int loops = 0;
            while (HasRemainingDataToParse())
            {
                loops++;
                char nextCharacter = Pop();
                if (IsNumeric(nextCharacter))
                {
                    currentNumeric += nextCharacter;
                }
                else if(IsTimeClassifier(nextCharacter))
                {
                    PopulateDurationModel(durationModel, currentNumeric, nextCharacter);
                    currentNumeric = "";
                }

                // Should loop max 9 times or so
                if (loops > 100)
                {
                    throw new Exception($"potential stack overflow parsing duration string. remaining string {parseString}");
                }
            }

            return durationModel.BuildTimeSpan();
        }

        private void PopulateDurationModel(DurationModel durationModel, string durationNumericString, char timeSpanClassifier)
        {
            int durationNumeric = 0;
            if (!string.IsNullOrEmpty(durationNumericString))
            {
                durationNumeric = int.Parse(durationNumericString);
            }
            
            switch (timeSpanClassifier)
            {
                case 'd':
                    durationModel.DurationDay = durationNumeric;
                    break;
                case 'h':
                    durationModel.DurationHour = durationNumeric;
                    break;
                case 'm':
                    durationModel.DurationMinute = durationNumeric;
                    break;
            }
        }

        private bool IsNumeric(char nextChar)
        {
            return (nextChar >= '0' && nextChar <= '9');
        }

        private bool IsTimeClassifier(char nextChar)
        {
            return TimeClassifier.Contains(nextChar);
        }

        private bool HasRemainingDataToParse()
        {
            return !string.IsNullOrEmpty(parseString);
        }

        private char Peek()
        {
            return parseString[0];
        }
        private char Pop()
        {
            var characterToReturn = Peek();
            parseString = parseString.Remove(0, 1);
            return characterToReturn;
        }

        private void Match(char characterToMatch)
        {
            if (Peek() == characterToMatch)
            {
                parseString = parseString.Remove(0, 1);
            }
            else
            {
                throw new Exception($"expected {Peek()} to match {characterToMatch} parsing duration string");
            }
        }
    }
}
