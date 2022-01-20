using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using ModelChallengeBot.EF.Models;
using Serilog;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelChallengeBot.EF
{
    public class BotContext : DbContext
    {
        public DbSet<ChannelRegistry> ChannelRegistry { get; set; }
        public DbSet<RoleRegistry> RoleRegistry { get; set; }
        public DbSet<ModelingChallenge> ModelingChallenge { get; set; }
        public DbSet<ChallengeImage> ChallengeImage { get; set; }

        public BotContext(DbContextOptions<BotContext> context) : base(context)
        {
           
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ConfigureWarnings((warningsBuilder) =>
            {
                warningsBuilder.Log((RelationalEventId.CommandExecuting, LogLevel.Debug));
                warningsBuilder.Log((RelationalEventId.CommandExecuted, LogLevel.Debug));
                warningsBuilder.Log((CoreEventId.ContextInitialized, LogLevel.Debug));
            });
        }
    }
}
