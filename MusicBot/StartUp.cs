using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MusicBot.Service;
using System;
using System.Threading.Tasks;

namespace MusicBot
{
    public class StartUp
    {
        public IConfigurationRoot Configuration { get; }

        public StartUp(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddYamlFile("config.yml");

            Configuration = builder.Build();
        }

        public static async Task RunAsync(string[] args)
        {
            var srartUp = new StartUp(args);
            await srartUp.RunAsync();
        }

        public async Task RunAsync()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);

            var provider = services.BuildServiceProvider();
            provider.GetRequiredService<CommandHandler>();

            await provider.GetRequiredService<StartUpService>().StartAsync();
            await Task.Delay(-1);
        }

        private void ConfigureServices(ServiceCollection services)
        {
            services.AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = Discord.LogSeverity.Debug,
                MessageCacheSize = 1000,
            }))
            .AddSingleton(new CommandService(new CommandServiceConfig
            {
                LogLevel = Discord.LogSeverity.Debug,
                DefaultRunMode = RunMode.Async,
                CaseSensitiveCommands = false,
            }))
            .AddSingleton<CommandHandler>()
            .AddSingleton<StartUpService>()
            .AddSingleton(Configuration);
        }
    }
}
