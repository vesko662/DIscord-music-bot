using Discord;
using Discord.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;
using Victoria;
using Victoria.Enums;

namespace MusicBot.Command
{
    public class Music : ModuleBase
    {
        private readonly LavaNode lavaNode;

        public Music(LavaNode lavaNode)
        {
            this.lavaNode = lavaNode;
        }

        [Command("join")]
        public async Task Join()
        {
            if (lavaNode.HasPlayer(Context.Guild))
            {
                await ReplyAsync("I'm already connected to a voice channel!");
                return;
            }

            var voiceState = Context.User as IVoiceState;

            if (voiceState?.VoiceChannel is null)
            {
                await ReplyAsync("You must be connected to a voice channel!");
                return;
            }

            try
            {
                await lavaNode.JoinAsync(voiceState.VoiceChannel, Context.Channel as ITextChannel);
                await ReplyAsync($"Joined {voiceState.VoiceChannel.Name}!");
            }
            catch (Exception exception)
            {
                await ReplyAsync(exception.Message);
            }

        }

        [Command("p")]
        public async Task Play([Remainder] string searchQuery = null)
        {
            if (string.IsNullOrWhiteSpace(searchQuery))
            {
                await ReplyAsync("Please provide search terms.");
                return;
            }

            if (!lavaNode.HasPlayer(Context.Guild))
            {
                await Join();
            }

            var searchResponse = await lavaNode.SearchYouTubeAsync(searchQuery);

            var player = lavaNode.GetPlayer(Context.Guild);

            var track = searchResponse.Tracks.FirstOrDefault();

            if (player.PlayerState == PlayerState.Playing || player.PlayerState == PlayerState.Paused)
            {
                player.Queue.Enqueue(track);
                await ReplyAsync($"Enqueued: {track.Title}");
            }
            else
            {
                await player.PlayAsync(track);
                await ReplyAsync($"Now Playing: {track.Title}");
            }

        }
    }
}
