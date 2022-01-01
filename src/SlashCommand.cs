using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace DNU
{
    public abstract class SlashCommand
    {
        public abstract string Name { get; }
        public abstract string Description { get; }
        
        public virtual Optional<List<SlashCommandOptionBuilder>> GetOptions() =>
            Optional<List<SlashCommandOptionBuilder>>.Unspecified;

        public virtual async Task HandleCommand(SocketSlashCommand command)
        {
            await command.RespondAsync($"You executed {command.Data.Name}. (this is the default implementation)");
        }
        
        public abstract bool IsGlobal { get; }
    }
}