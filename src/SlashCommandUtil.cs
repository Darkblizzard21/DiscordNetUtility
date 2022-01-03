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
                try
                {
                    if (slashCommand.IsGlobal)
                    {
                        currentGlobalCommands.Add(commandProperties);
                    }
                    else
                    {
                        foreach (var socketGuild in guilds)
                        {
                            await socketGuild.CreateApplicationCommandAsync(commandProperties);
                        }
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
                catch (HttpException exception)
                {
                    // If our command was invalid, we should catch an ApplicationCommandException.
                    // This exception contains the path of the error as well as the error message.
                    // You can serialize the Error field in the exception to get a visual of where your error is.
                    var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);

                    // You can send this error somewhere or just print it to the console, for this example we're just going to print it.
                    Console.WriteLine(json + $"\n\nCommand {slashCommand.Name} was not added.");
                }
            }

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