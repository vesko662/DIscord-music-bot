using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace MusicBot.Service
{
    public class StartUpService
    {
        public static IServiceProvider _provider;
        private readonly DiscordSocketClient discord;
        private readonly CommandService command;
        public readonly IConfigurationRoot config;

        public StartUpService(IServiceProvider provider,
            DiscordSocketClient discord, 
            CommandService command, 
            IConfigurationRoot config)
        {
            _provider = provider;
            this.discord = discord;
            this.command = command;
            this.config = config;
        }

        public async Task StartAsync()
        {
            string token = config["token"];

            if (string.IsNullOrEmpty(token))
            {
                Console.WriteLine("Please provide your Discord token in the config.yml file");
                return;
            }

            await discord.LoginAsync(TokenType.Bot, token);
            await discord.StartAsync();
            await command.AddModulesAsync(Assembly.GetEntryAssembly(), _provider);
        }
    }
}
