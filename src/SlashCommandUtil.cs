using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Net;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace DNU
{
    public static class SlashCommandUtil
    {
        private static readonly Dictionary<string, SlashCommand> Commands = new Dictionary<string, SlashCommand>();
        private static IEnumerable<SlashCommand> _commands = new List<SlashCommand>();

        public static void AddCommand(IEnumerable<SlashCommand> additionalCommands)
        {
            _commands = _commands.Concat(additionalCommands);
        }

        public static void AddCommand(SlashCommand additionalCommand)
        {
            _commands = _commands.Concat(new[] {additionalCommand});
        }

        public static async Task RegisterCommands(
            this DiscordSocketClient client,
            List<SocketGuild> guilds)
        {
            var currentGlobalCommands = new List<SlashCommandProperties>();
            var currentGuildCommands = new List<SlashCommandProperties>();
            foreach (var slashCommand in _commands)
            {
                var command = new SlashCommandBuilder();
                command.WithName(slashCommand.Name);
                command.WithDescription(slashCommand.Description);

                if (slashCommand.GetOptions().IsSpecified)
                {
                    var options = slashCommand.GetOptions().Value;
                    foreach (var slashCommandOptionBuilder in options)
                    {
                        command.AddOption(slashCommandOptionBuilder);
                    }
                }


                var commandProperties = command.Build();

                if (slashCommand.IsGlobal)
                {
                    currentGlobalCommands.Add(commandProperties);
                }
                else
                {
                    currentGuildCommands.Add(commandProperties);
                }

                if (!Commands.ContainsKey(slashCommand.Name))
                {
                    Commands.Add(slashCommand.Name, slashCommand);
                }
                else
                {
                    Console.WriteLine(
                        $"\n\nCommand {slashCommand.Name} was not added. Because Command with that Name already exits");
                }
            }

            // Global Commands
            var onlineCommands = await client.GetGlobalApplicationCommandsAsync();
            var toRemove = onlineCommands
                .Where(sac => currentGlobalCommands
                    .Any(properties => !(properties.Name.Value.Equals(sac.Name)
                                         && properties.Name.Value.Equals(sac.Name)))
                );

            foreach (var socketApplicationCommand in toRemove)
            {
                await socketApplicationCommand.DeleteAsync();
            }

            foreach (var slashCommandProperties in currentGlobalCommands)
            {
                await client.CreateGlobalApplicationCommandAsync(slashCommandProperties);
            }
            
            //GuildCommands
            foreach (var socketGuild in guilds)
            {
                var onlineGuildCommands = await socketGuild.GetApplicationCommandsAsync();
                var toRemoveGuild = onlineGuildCommands
                    .Where(sac => currentGuildCommands
                        .Any(properties => !(properties.Name.Value.Equals(sac.Name)
                                             && properties.Name.Value.Equals(sac.Name)))
                    );

                foreach (var socketApplicationCommand in toRemoveGuild)
                {
                    await socketApplicationCommand.DeleteAsync();
                }

                foreach (var slashCommandProperties in currentGuildCommands)
                {
                    await socketGuild.CreateApplicationCommandAsync(slashCommandProperties);
                }
            }
        }


        public static async Task SlashCommandHandler(SocketSlashCommand command)
        {
            if (Commands.TryGetValue(command.Data.Name, out var invokedCommand))
            {
                await invokedCommand.HandleCommand(command);
            }
            else
            {
                await command.RespondAsync($"No command with Name \"{command.Data.Name}\" found.");
            }
        }
    }
}