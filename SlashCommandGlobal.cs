using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace DiscordNetUtility
{
    public abstract class SlashCommandGlobal
    {
        public abstract SlashCommandProperties BuildCommand();

        public async Task HandleCommand(SocketSlashCommand command)
        {
        }
    }
}