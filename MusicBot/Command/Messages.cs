using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicBot.Command
{
    public class Messages : ModuleBase
    {
        public readonly IConfiguration config;

        public Messages(IConfiguration config)
        {
            this.config = config;
        }

        [Command("clean")]
        public async Task Clean()
        {
            var messages = await Context.Channel.GetMessagesAsync().FlattenAsync();

            var messagesToDelete = messages
                .Where(x => DateTime.Compare(x.CreatedAt.UtcDateTime.AddDays(14), DateTime.UtcNow) == 1)
                .Where(x => x.Author.Id == 884410899572588544 ||
                 x.Author.IsBot == false && x.Content.StartsWith(config["prefix"]))
                .ToList();

            await (Context.Channel as SocketTextChannel).DeleteMessagesAsync(messagesToDelete);

            var informationMessage = await Context
                .Channel
                .SendMessageAsync($"{messagesToDelete.Count} messages deleted successfuly!");

            await Task.Delay(2500);
            await informationMessage.DeleteAsync();
        }
    }
}
