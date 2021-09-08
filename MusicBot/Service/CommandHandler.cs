using Discord;
using Discord.Addons.Hosting;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Victoria;

namespace MusicBot.Service
{
    public class CommandHandler: InitializedService
    {
        public readonly IServiceProvider provider;
        public readonly DiscordSocketClient client;
        public readonly CommandService command;
        public readonly IConfiguration config;
        private readonly LavaNode lavaNode;

        public CommandHandler(IServiceProvider provider,
            DiscordSocketClient client, 
            CommandService command, 
            IConfiguration config,
            LavaNode lavaNode)
        {
            this.provider = provider;
            this.client = client;
            this.command = command;
            this.config = config;
            this.lavaNode = lavaNode;
        }

        public override async Task InitializeAsync(CancellationToken cancellationToken)
        {
            client.MessageReceived += OnMessageRecieved;
            command.CommandExecuted += OnCommandExecute;
            client.Ready += OnReadyAsync;

            await command.AddModulesAsync(Assembly.GetEntryAssembly(), provider);
        }

        private async Task OnCommandExecute(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
           if (command.IsSpecified && !result.IsSuccess)
                 Console.WriteLine($"Error:{result}");
        }

        private async Task OnReadyAsync()
        {
            if (!lavaNode.IsConnected)
            {
               await lavaNode.ConnectAsync();
            }
        }

        private async Task OnMessageRecieved(SocketMessage arg)
        {
            if (!(arg is SocketUserMessage message))
                return;

            if (message.Source != MessageSource.User)
                return;

            var prefix = config["prefix"];
            var argPos = 0;

            if (!message.HasStringPrefix(prefix, ref argPos) && !message.HasMentionPrefix(client.CurrentUser, ref argPos))
                return;

            var context = new SocketCommandContext(client, message);

            await command.ExecuteAsync(context, argPos, provider);
        }
    }
}
