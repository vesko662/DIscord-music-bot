using Discord;
using Discord.Addons.Hosting;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace MusicBot.Service
{
    public class CommandHandler: InitializedService
    {
        public static IServiceProvider _provider;
        public static DiscordSocketClient _client;
        public static CommandService _command;
        public static IConfiguration _config;

        public CommandHandler(IServiceProvider provider,
            DiscordSocketClient client,
            CommandService command,
            IConfiguration config)
        {
            _provider = provider;
            _client = client;
            _command = command;
            _config = config;
        }

        public override async Task InitializeAsync(CancellationToken cancellationToken)
        {
            _client.MessageReceived += OnMessageRecieved;
            _command.CommandExecuted += OnCommandExecute;
            await _command.AddModulesAsync(Assembly.GetEntryAssembly(), _provider);
        }

        private async Task OnCommandExecute(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            if (command.IsSpecified && !result.IsSuccess)
                Console.WriteLine($"Error:{result}");
        }

        private async Task OnMessageRecieved(SocketMessage arg)
        {
            if (!(arg is SocketUserMessage message))
                return;

            if (message.Source != MessageSource.User)
                return;
            var prefix = _config["prefix"];
            var argPos = 0;

            if (!message.HasStringPrefix(prefix, ref argPos) && !message.HasMentionPrefix(_client.CurrentUser, ref argPos))
                return;

            var context = new SocketCommandContext(_client, message);
            await _command.ExecuteAsync(context, argPos, _provider);
        }
    }
}
