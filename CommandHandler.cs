using System;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using SocialLinker.Config;
using SocialLinker.Core.LevelSystem;
using Fergun.Interactive;
using System.Collections.Generic;
using System.Linq;
using Discord;
using Discord.Interactions;
using Newtonsoft.Json;
using Discord.Net;

namespace SocialLinker
{
    public class SocialLinkerCommand
    {
        public string CommandType { get; set; }
        public string CommandName { get; set; }
        public SocketUser User { get; set; }
        public ISocketMessageChannel Channel { get; set; }
        public SocketUser MentionedUser { get; set; }
        public string Content { get; set; }
        public IReadOnlyCollection<Attachment> Attachments { get; set; }
        public SocketUserMessage Message { get; set; }
        public SocketSlashCommand SlashCommand { get; set; }
    }

    public class CommandConverter : InteractionModuleBase<SocketInteractionContext>
    {
        public static SocialLinkerCommand SlashCommandConverter(SocketSlashCommand command)
        {
            SocketUser data_to_mentioneduser;
            string data_to_string;

            if ((command.Data != null)) // Maker, help, settings, shop, status
            {
                data_to_mentioneduser = (SocketUser)command.Data.Options.First().Value;
                data_to_string = null;
            }
            else
            {
                data_to_mentioneduser = null;
                data_to_string = command.Data.Options.ToString();
            }

            SocialLinkerCommand slash_to_command = new SocialLinkerCommand
            {
                CommandType = "Slash",
                CommandName = command.CommandName,
                User = command.User,
                Channel = command.Channel,
                MentionedUser = data_to_mentioneduser,
                Content = data_to_string,
                Message = null,
                SlashCommand = command
            };

            return slash_to_command;
        }

        public static SocialLinkerCommand ContextCommandConverter(SocketMessage message)
        {
            List<string> input_substring;

            char[] delimiterChars = { ' ' };

            input_substring = message.Content.Split(delimiterChars).ToList();

            int prefix_length = $"{BotConfig.bot.cmdPrefix}".Length;

            string parsed_command_name = input_substring[0].Substring(prefix_length);

            if (input_substring.Count > 1)
            {
                input_substring.RemoveAt(0);
            }

            string parsed_content = String_List_To_String(input_substring);

            SocialLinkerCommand context_to_command = new SocialLinkerCommand
            {
                CommandType = "Context",
                CommandName = parsed_command_name,
                User = message.Author,
                Channel = message.Channel,
                //MentionedUser = message.MentionedUsers.First() ?? null,
                Content = parsed_content,
                Attachments = message.Attachments,
                Message = (SocketUserMessage)message
            };

            return context_to_command;
        }

        public static string String_List_To_String(List<string> input_list)
        {
            // Create an empty string variable.
            string output_string = "";

            // Iterate through each index of the list and add it to the string variable.
            for (int i = 0; i < input_list.Count; i++)
            {
                output_string += input_list[i];
            }

            // Return the string variable.
            return output_string;
        }
    }

    class CommandHandler
    {
        DiscordShardedClient _client;
        Commands.InteractionHandler _interactionHandler;
        CommandService _service;
        public IServiceProvider _Services;

        public async Task InitializeAsync(DiscordShardedClient client)
        {
            _client = client;
            _service = new CommandService();
            _Services = ConfigureServices();
            await _service.AddModulesAsync(Assembly.GetEntryAssembly(), _Services);
            _client.MessageReceived += HandleCommandAsync;

            _client.ShardReady += Status;
            /*_client.ShardReady += Shop;
            _client.ShardReady += Settings;
            _client.ShardReady += Help;
            _client.ShardReady += Hug;
            _client.ShardReady += Pat;
            _client.ShardReady += Punch;
            _client.ShardReady += Slap; */
            await Task.CompletedTask;

            _interactionHandler = new Commands.InteractionHandler();
            await _interactionHandler.InitializeAsync(_client);
            
        }

        /*private async Task HandleCommandAsync(SocketMessage s)
        {
            var msg = s as SocketUserMessage;
            if (msg == null) return;
            var context = new ShardedCommandContext(_client, msg);
            if (context.User.IsBot) return;

            // If the message is a direct message, return immediately.
            if (msg.Channel.GetType() == typeof(SocketDMChannel))
            {
                return;
            }

            //If the user is in a time out status, do nothing and return
            if (TimeOut.TimeOutStatus(msg) == "Yes") return;

            int argPos = 0;
            if (msg.HasStringPrefix(BotConfig.bot.cmdPrefix, ref argPos))
            {
                var result = await _service.ExecuteAsync(context, argPos, _Services);
                if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
                {
                    Console.WriteLine(result.ErrorReason);
                } 

                //Add Proficiency to the user's account whenever a command is successfully used
                SocialStats.AddProficiency(msg);
            }

            //Calculate if the user gains Diligence for this message
            SocialStats.AddDiligence(msg);

            //Leveling up manages the user's time caps, so make sure it comes after AddProficiency and AddDiligence have ran
            Leveling.UserSentMessage(msg);
        } */

        private async Task HandleCommandAsync(SocketMessage s)
        {
            var msg = s as SocketUserMessage;
            if (msg == null) return;
            var context = new ShardedCommandContext(_client, msg);
            if (context.User.IsBot) return;

            // If the message is a direct message, return immediately.
            if (msg.Channel.GetType() == typeof(SocketDMChannel))
            {
                return;
            }

            //If the user is in a time out status, do nothing and return
            if (TimeOut.TimeOutStatus(msg) == "Yes") return;

            Console.WriteLine("Here 0");

            int argPos = 0;
            if (msg.HasStringPrefix(BotConfig.bot.cmdPrefix, ref argPos))
            {
                Console.WriteLine("Here 1");
                try
                {
                    var converted_sl_command = CommandConverter.ContextCommandConverter(msg);
                    Console.WriteLine("Here 2");
                    Console.WriteLine(converted_sl_command.CommandName);
                    await CommandIndex(converted_sl_command);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                //Add Proficiency to the user's account whenever a command is successfully used
                //SocialStats.AddProficiency(msg);
            }

            //Calculate if the user gains Diligence for this message
            //SocialStats.AddDiligence(msg);

            //Leveling up manages the user's time caps, so make sure it comes after AddProficiency and AddDiligence have ran
            //Leveling.UserSentMessage(msg);
        }

        private async Task CommandIndex(SocialLinkerCommand command)
        {
            if (command.CommandType == "Slash")
            {
                await command.SlashCommand.RespondAsync(embed: Slash_Command_Successful().Build(), ephemeral: true);
            }

            switch (command.CommandName)
            {
                case "status":

                    break;

                case "shop":
                    break;

                case "settings":
                    break;

                case "help":
                    await Commands.Help.HelpMenu(command);
                    break;

                case "hug":
                    break;

                case "pat":
                    break;

                case "punch":
                    break;

                case "slap":
                    break;

                case "maker":
                    break;
            }
        }

        public static async Task Status(DiscordSocketClient client)
        {
            ulong guildId = 543226698238394378;
            //await client.GetGuild(543226698238394378).DeleteApplicationCommandsAsync();

            var guildCommand = new SlashCommandBuilder()
                .WithName("status")
                .WithDescription("View your status screen, or specify a user to view theirs.")
                .AddOption("user", ApplicationCommandOptionType.User, "The user whose status you want to view.", isRequired: false);

            try
            {
                await client.Rest.CreateGuildCommand(guildCommand.Build(), guildId);
            }
            catch (HttpException exception)
            {
                var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);
                Console.WriteLine(json);
            }
        }

        public static async Task Shop(DiscordSocketClient client)
        {
            ulong guildId = 543226698238394378;

            var guildCommand = new SlashCommandBuilder()
                .WithName("shop")
                .WithDescription("Browse the décor shop.");

            try
            {
                await client.Rest.CreateGuildCommand(guildCommand.Build(), guildId);
            }
            catch (HttpException exception)
            {
                var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);
                Console.WriteLine(json);
            }
        }

        public static async Task Settings(DiscordSocketClient client)
        {
            ulong guildId = 543226698238394378;

            var guildCommand = new SlashCommandBuilder()
                .WithName("settings")
                .WithDescription("Change and customize user settings.");

            try
            {
                await client.Rest.CreateGuildCommand(guildCommand.Build(), guildId);
            }
            catch (HttpException exception)
            {
                var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);
                Console.WriteLine(json);
            }
        }

        public static async Task Help(DiscordSocketClient client)
        {
            ulong guildId = 543226698238394378;

            var guildCommand = new SlashCommandBuilder()
                .WithName("help")
                .WithDescription("Learn how to use Social Linker.");

            try
            {
                await client.Rest.CreateGuildCommand(guildCommand.Build(), guildId);
            }
            catch (HttpException exception)
            {
                var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);
                Console.WriteLine(json);
            }
        }

        public static async Task Hug(DiscordSocketClient client)
        {
            ulong guildId = 543226698238394378;

            var guildCommand = new SlashCommandBuilder()
                .WithName("hug")
                .WithDescription("Give a user a hug.")
                .AddOption("user", ApplicationCommandOptionType.User, "The user you want to hug.", isRequired: true);

            try
            {
                await client.Rest.CreateGuildCommand(guildCommand.Build(), guildId);
            }
            catch (HttpException exception)
            {
                var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);
                Console.WriteLine(json);
            }
        }

        public static async Task Pat(DiscordSocketClient client)
        {
            ulong guildId = 543226698238394378;

            var guildCommand = new SlashCommandBuilder()
                .WithName("pat")
                .WithDescription("Give a user a pat.")
                .AddOption("user", ApplicationCommandOptionType.User, "The user you want to pat.", isRequired: true);

            try
            {
                await client.Rest.CreateGuildCommand(guildCommand.Build(), guildId);
            }
            catch (HttpException exception)
            {
                var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);
                Console.WriteLine(json);
            }
        }

        public static async Task Punch(DiscordSocketClient client)
        {
            ulong guildId = 543226698238394378;

            var guildCommand = new SlashCommandBuilder()
                .WithName("punch")
                .WithDescription("Give out unbridled violence.")
                .AddOption("user", ApplicationCommandOptionType.User, "The user you want to punch.", isRequired: true);

            try
            {
                await client.Rest.CreateGuildCommand(guildCommand.Build(), guildId);
            }
            catch (HttpException exception)
            {
                var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);
                Console.WriteLine(json);
            }
        }

        public static async Task Slap(DiscordSocketClient client)
        {
            ulong guildId = 543226698238394378;

            var guildCommand = new SlashCommandBuilder()
                .WithName("slap")
                .WithDescription("Hand out a high five. In the face.")
                .AddOption("user", ApplicationCommandOptionType.User, "The user you want to slap.", isRequired: true);

            try
            {
                await client.Rest.CreateGuildCommand(guildCommand.Build(), guildId);
            }
            catch (HttpException exception)
            {
                var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);
                Console.WriteLine(json);
            }
        }

        public static EmbedBuilder Slash_Command_Successful()
        {
            var embed = new EmbedBuilder();

            embed.WithColor(0, 207, 41);
            embed.WithDescription("**Slash command successful** :white_check_mark:");

            return embed;
        }

        private IServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandler>()
                .AddSingleton<InteractiveService>()
                .BuildServiceProvider();
        }
    }
}
