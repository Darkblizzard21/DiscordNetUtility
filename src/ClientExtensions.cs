using System.Collections.Generic;
using System.Linq;
using Discord.WebSocket;

namespace DNU
{
    public static class ClientExtensions
    {
        public static IEnumerable<SocketGuild> FetchGuilds(this DiscordSocketClient client, List<ulong> guildIds) =>
            guildIds.Select(client.GetGuild);
    }
}