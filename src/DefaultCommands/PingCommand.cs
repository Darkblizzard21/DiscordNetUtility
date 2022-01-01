using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace DNU.DefaultCommands
{
    public class PingCommand : SlashCommand
    {
        public override string Name => "ping";
        public override string Description => "Measures ping of the bot.";

        public override async Task HandleCommand(SocketSlashCommand command)
        {
            await command.RespondAsync("⌛ measuring ping ... ");
            var x = command.CreatedAt;
            var y = command.GetOriginalResponseAsync().Result.CreatedAt;
            await command.ModifyOriginalResponseAsync(properties =>
                properties.Content =
                    $"🏓 {(command.Channel is IDMChannel ? "Global" : "")} Slash Command Ping: {(y - x).Seconds}.{(y - x).Milliseconds % 1000} s");
        }

        public override bool IsGlobal => true;
    }
}