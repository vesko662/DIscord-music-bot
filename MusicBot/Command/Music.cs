using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicBot.Command
{
    public class Music: ModuleBase
    {
        [Command("p")]
        public async Task Play()
        {
            await Context.Channel.SendMessageAsync("playing");
        }

        [Command("join")]
        public async Task Join()
        {

        }
    }
}
