﻿using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Kdrama.Services;

namespace Kdrama
{
    class Program
    {
        static void Main(string[] args)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("config.json")
                .Build();

            var host = CreateHostBuilder(args, configuration).Build();
            new Program().MainAsync(host).GetAwaiter().GetResult();
        }


        public async Task MainAsync(IHost host)
        {
            var runner = host.Services.GetRequiredService<DiscordRunner>();

            await host.StartAsync();
            await runner.StartClient(host.Services);

            Console.WriteLine("Press ANY key to exit");
            Console.ReadLine();
        }

        public static IHostBuilder CreateHostBuilder(string[] args, IConfiguration configuration) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddScoped<IPermService, PermService>();
                
                services.AddSingleton<IConfiguration>(configuration);
                services.AddSingleton<DiscordRunner>();
                
            });
    }
}
