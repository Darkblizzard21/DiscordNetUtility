using System.Threading.Tasks;
using Discord.WebSocket;

namespace DNU
{
    public static class UserInteraction
    {
        private static ulong? _maintainerId = null;

        public static ulong MaintainerId
        {
            set => _maintainerId = value;
            get => _maintainerId ?? 0;
        }
        public static bool IsAdmin(SocketGuildUser user)
        {
            return user.GuildPermissions.Administrator || (_maintainerId.HasValue && user.Id == _maintainerId);
        }

        public static async Task<bool> CheckAdmin(this SocketSlashCommand command, SocketGuildUser user)
        {
            var res = IsAdmin(user);
            if(!res) await command.RespondAsync($"Command can only be invoked by admins", ephemeral: true);
            return res;
        }
    }
}