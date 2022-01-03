using System.Threading.Tasks;
using Discord.WebSocket;

namespace DNU
{
    public static class UserInteraction
    {
        public static bool IsAdmin(SocketGuildUser user)
        {
            return user.GuildPermissions.Administrator;
        }

        public static async Task<bool> CheckAdmin(this SocketSlashCommand command, SocketGuildUser user)
        {
            var res = IsAdmin(user);
            if(!res) await command.RespondAsync($"Command can only be invoked by admins", ephemeral: true);
            return res;
        }
    }
}