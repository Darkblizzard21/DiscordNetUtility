using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace DiscordNetUtility
{
    public abstract class SlashCommand
    {
        public abstract Tuple<ulong, SlashCommandProperties> BuildCommand();

        public async Task HandleCommand(SocketSlashCommand command)
        {
        }
    }
}