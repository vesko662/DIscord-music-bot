using Discord;
using Discord.Commands;
using Discord.WebSocket;
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

        [Command("leave")]
        public async Task Leave()
        {
            if (!lavaNode.HasPlayer(Context.Guild))
            {
                await ReplyAsync("I'm not connected to a voice channel!");
                return;
            }

            var voiceState = Context.User as IVoiceState;

            if (voiceState?.VoiceChannel is null)
            {
                await ReplyAsync("You are not connected to a voice channel!");
                return;
            }

            try
            {
                await lavaNode.LeaveAsync(voiceState.VoiceChannel);
            }
            catch (Exception exception)
            {
                await ReplyAsync(exception.Message);
            }

        }

        [Command("p")]
        public async Task Play([Remainder] string searchQuery = null)
        {
            if (!lavaNode.HasPlayer(Context.Guild))
            {
                await Join();
            }

            if (string.IsNullOrWhiteSpace(searchQuery))
            {
                await ReplyAsync("Please provide search terms.");
                return;
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

        [Command("skip")]
        public async Task Skip()
        {
            var voiceState = Context.User as IVoiceState;

            if (voiceState?.VoiceChannel is null)
            {
                await ReplyAsync("You must be connected to a voice channel!");
                return;
            }

            if (!lavaNode.HasPlayer(Context.Guild))
            {
                await ReplyAsync("You are not connected to a voice channel!");
                return;
            }

            var player = lavaNode.GetPlayer(Context.Guild);

            if (voiceState.VoiceChannel != player.VoiceChannel)
            {
                await ReplyAsync("You need to be in the same voice channel as me!");
                return;
            }

            if (player.Queue.Count == 0)
            {
                await ReplyAsync("There are no more song in the queue!");
                return;
            }

            await player.SkipAsync();

            await ReplyAsync($"Now Playing: {player.Track.Title}");
        }

        [Command("pause")]
        public async Task Pause()
        {
            var voiceState = Context.User as IVoiceState;

            if (voiceState?.VoiceChannel is null)
            {
                await ReplyAsync("You must be connected to a voice channel!");
                return;
            }

            if (!lavaNode.HasPlayer(Context.Guild))
            {
                await ReplyAsync("You are not connected to a voice channel!");
                return;
            }

            var player = lavaNode.GetPlayer(Context.Guild);

            if (voiceState.VoiceChannel != player.VoiceChannel)
            {
                await ReplyAsync("You need to be in the same voice channel as me!");
                return;
            }

            if (player.PlayerState == PlayerState.Paused || player.PlayerState == PlayerState.Stopped)
            {
                await ReplyAsync("The music is already paused!");
                return;
            }

            await player.PauseAsync();

            await ReplyAsync("Paused the music!");
        }

        [Command("resume")]
        public async Task Resume()
        {
            var voiceState = Context.User as IVoiceState;

            if (voiceState?.VoiceChannel is null)
            {
                await ReplyAsync("You must be connected to a voice channel!");
                return;
            }

            if (!lavaNode.HasPlayer(Context.Guild))
            {
                await ReplyAsync("You are not connected to a voice channel!");
                return;
            }

            var player = lavaNode.GetPlayer(Context.Guild);

            if (voiceState.VoiceChannel != player.VoiceChannel)
            {
                await ReplyAsync("You need to be in the same voice channel as me!");
                return;
            }

            if (player.PlayerState == PlayerState.Playing)
            {
                await ReplyAsync("The music is already playing!");
                return;
            }

            await player.ResumeAsync();

            await ReplyAsync("Resumed the music!");
        }
    }
}
