using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ModelChallengeBot.Providers;
using Serilog;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using ModelChallengeBot.EF;
using Microsoft.EntityFrameworkCore;
using ModelChallengeBot.Extensions;
using ModelChallengeBot.Configuration;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using ModelChallengeBot.Providers.Discord;
using ModelChallengeBot.Providers.Discord.Handlers;

namespace ModelChallengeBot
{
    public class Program
    {
        public static Task Main(string[] args)
        {
            try
            {
                return CreateHostBuilder(args).Build().RunAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Task.CompletedTask;
            }
        }

        static IHostBuilder CreateHostBuilder(string[] args) =>
          Host.CreateDefaultBuilder(args)
              .ConfigureAppConfiguration((hostingContext, configuration) =>
              {
                  configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                  configuration.AddJsonFile("appsettings.development.json", optional: true, reloadOnChange: false);
                  Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(configuration.Build(), "Serilog").CreateLogger();
              })
              .ConfigureServices((hostBuilderContext, services) =>
              {
                  var assemblies = Assembly.GetExecutingAssembly().GetTypes().ToList();
                  services
                     .RegisterQueues(assemblies)//Queues should be tables if execution ending abruptly is problematic
                     .RegisterConfigurationSettings(hostBuilderContext.Configuration, assemblies)
                     .AddDbContextFactory<BotContext>(options => options
                        .UseSqlite(
                            hostBuilderContext.Configuration.GetSettings<DatabaseSettings>()
                            .BuildConnectionString(hostBuilderContext.Configuration.GetSettings<WorkspaceSettings>().Directory)))
                     .RegisterDbProviders(assemblies)
                     .AddTransient<IDownloadProvider, DownloadProvider>()
                     .AddTransient<IProcessExecutionProvider, ProcessExecutionProvider>()
                     .AddTransient<IRenderProvider, BlenderRenderProvider>()
                     .AddTransient<IDiscordUserProvider, DiscordUserProvider>()
                     .AddTransient<IDurationParserProvider, DurationParserProvider>()
                     .AddTransient<IAcceptedChallengeProvider, AcceptedChallengeProvider>()
                     .AddTransient<ISubmitChallengeProvider, SubmitChallengeProvider>()
                     .RegisterDiscordNetTypes()
                     .RegisterWorkers(assemblies);
              });


    }
}