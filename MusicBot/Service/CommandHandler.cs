using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace MusicBot.Service
{
    public class CommandHandler
    {
        public static IServiceProvider _provider;
        public static DiscordSocketClient _discord;
        public static CommandService _command;
        public static IConfigurationRoot _config;

        public CommandHandler(IServiceProvider provider,
            DiscordSocketClient discord,
            CommandService command,
            IConfigurationRoot config)
        {
            _provider = provider;
            _discord = discord;
            _command = command;
            _config = config;

            _discord.Ready += OnReady;
        }

        private Task OnReady()
        {
            Console.WriteLine("Ready to use");
            return Task.CompletedTask;
        }
    }
}
