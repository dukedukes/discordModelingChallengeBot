using Discord;
using Microsoft.Extensions.Logging;
using ModelChallengeBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelChallengeBot.Extensions
{
    public static class LogMessageExtensions
    {
        public static SerilogMessage ConvertToSerilogMessage(this LogMessage logMessage)
        {
            return new SerilogMessage()
            {
                LogLevel = ConvertSeverityToLogLevel(logMessage.Severity),
                Message = logMessage.Message,

            };
        }

        private static LogLevel ConvertSeverityToLogLevel(LogSeverity severity)
        {
            LogLevel logLevel;
            switch (severity)
            {
                case LogSeverity.Error:
                    logLevel = LogLevel.Error;
                    break;
                case LogSeverity.Warning:
                    logLevel = LogLevel.Warning;
                    break;
                case LogSeverity.Info:
                    logLevel = LogLevel.Information;
                    break;
                case LogSeverity.Verbose:
                    logLevel = LogLevel.Trace;
                    break;
                case LogSeverity.Critical:
                    logLevel = LogLevel.Critical;
                    break;
                case LogSeverity.Debug:
                    logLevel = LogLevel.Debug;
                    break;
                default:
                    logLevel = LogLevel.Warning;
                    break;
            }
            return logLevel;
        }
    }
}
