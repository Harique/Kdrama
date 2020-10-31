﻿using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using snipetrain_bot.Services;

namespace snipetrain_bot
{
    class Program
    {
        static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("config.json")
                .Build();

            var servicesProvider = BuildDi(configuration);

            var runner = servicesProvider.GetRequiredService<DiscordRunner>();

            await runner.StartClient(servicesProvider, configuration);

            Console.WriteLine("Press ANY key to exit");
            Console.ReadLine();
        }

        private static IServiceProvider BuildDi(IConfiguration configuration)
        {
            var services = new ServiceCollection();

            services.AddTransient<DiscordRunner>();

            services.AddSingleton<IConfiguration>(configuration);

            return services.BuildServiceProvider();
        }
    }
}
