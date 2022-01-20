using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ModelChallengeBot.Bot;
using ModelChallengeBot.Configuration;
using ModelChallengeBot.Providers.EF;
using ModelChallengeBot.Queues;
using ModelChallengeBot.Workers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ModelChallengeBot.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterConfigurationSettings(this IServiceCollection serviceCollection, IConfiguration configuration, List<Type> assemblies)
        {
            foreach (var foundSettingType in GetConcreteTypesByCommonType(assemblies, typeof(Settings)))
            {
                serviceCollection.AddSingleton(foundSettingType, configuration.GetSettings(foundSettingType));
            }
            return serviceCollection;
        }

        public static IServiceCollection RegisterQueues(this IServiceCollection serviceCollection, List<Type> assemblies)
        {
            return RegisterByNonGenericInterface(serviceCollection, assemblies, typeof(IQueue));
        }

        public static IServiceCollection RegisterDbProviders(this IServiceCollection serviceCollection, List<Type> assemblies)
        {
            return RegisterByNonGenericInterface(serviceCollection, assemblies, typeof(IDbProvider));
        }

        public static IServiceCollection RegisterDiscordNetTypes(this IServiceCollection serviceCollection)
        {
            return serviceCollection.AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandler>();
        }

        private static IServiceCollection RegisterByNonGenericInterface(IServiceCollection serviceCollection, List<Type> assemblies, Type discoveryType)
        {
            return RegisterByNonGenericInterface(serviceCollection, assemblies, discoveryType, new List<Type>());
        }

        private static IServiceCollection RegisterByNonGenericInterface(IServiceCollection serviceCollection, List<Type> assemblies, Type discoveryType, List<Type> excludedInterfaces)
        {
            foreach (var foundConcreteType in GetConcreteTypesByCommonType(assemblies, discoveryType))
            {
                List<Type> interfaces = foundConcreteType.GetInterfaces().ToList();
                interfaces.Remove(discoveryType);
                for (int i = interfaces.Count - 1; i >= 0; i--)
                {
                    Type foundInterface = interfaces[i];
                    if (foundInterface.IsConstructedGenericType || excludedInterfaces.Contains(foundInterface))
                    {
                        interfaces.RemoveAt(i);
                    }
                }
                
                Type immediateInterface;
                try
                {
                    immediateInterface = interfaces.Single();
                }
                catch (Exception)
                {
                    throw new Exception("to be discovered types must have a single non-generic interface");
                }
                serviceCollection.AddSingleton(immediateInterface, foundConcreteType);
            }
            return serviceCollection;
        }

        public static IServiceCollection RegisterWorkers(this IServiceCollection serviceCollection, List<Type> assemblies)
        {
            GetConcreteTypesByCommonType(assemblies, typeof(IWorker)).ForEach(workerType => 
            {
                serviceCollection.AddSingleton(typeof(IHostedService), workerType);
            });
            return serviceCollection;
        }

        private static List<Type> GetConcreteTypesByCommonType(List<Type> assemblies, Type concreteTypesInterface)
        {
            return assemblies.Where(t => !t.IsAbstract && t.IsAssignableTo(concreteTypesInterface)).ToList();
        }
    }
}
