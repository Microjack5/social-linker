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
using SocialLinker.Core.SceneMaker;
using SocialLinker.Core.CloudStorageTables;

namespace SocialLinker
{
    public class SocialLinkerCommand
    {
        public string CommandType { get; set; }
        public string CommandName { get; set; }
        public SocketUser User { get; set; }
        public ISocketMessageChannel Channel { get; set; }
        public SocketUser MentionedUser { get; set; }
        public IReadOnlyCollection<Attachment> Attachments { get; set; }
        public SocketUserMessage Message { get; set; }
        public SocketSlashCommand SlashCommand { get; set; }
        public MakerCommandData MakerCommand { get; set; }
        public MakerMultiCommandData MakerMultiCommand { get; set; }
        public bool ValidCommand { get; set; }
    }

    public class CommandConverter : InteractionModuleBase<SocketInteractionContext>
    {
        public static SocialLinkerCommand SlashCommandConverter(SocketSlashCommand command)
        {
            SocketUser data_to_mentioneduser;
            bool social_bool = false;

            switch (command.CommandName)
            {
                case "status":
                    if (command.Data.Options.FirstOrDefault() != default)
                    {
                        social_bool = true;
                    }
                    break;

                case "hug":
                    social_bool = true;
                    break;

                case "pat":
                    social_bool = true;
                    break;

                case "punch":
                    social_bool = true;
                    break;

                case "slap":
                    social_bool = true;
                    break;

                default:
                    break;
            }

            if (social_bool == true)
            {
                data_to_mentioneduser = (SocketUser)command.Data.Options.FirstOrDefault().Value;
            }
            else
            {
                data_to_mentioneduser = null;
            }

            SocialLinkerCommand slash_to_command = new SocialLinkerCommand
            {
                CommandType = "Slash",
                CommandName = command.CommandName,
                User = command.User,
                Channel = command.Channel,
                MentionedUser = data_to_mentioneduser,
                Attachments = default,
                Message = null,
                SlashCommand = command
            };

            return slash_to_command;
        }

        public static SocialLinkerCommand ContextCommandConverter(SocketUserMessage message, DiscordShardedClient client)
        {
            List<string> input_substring;

            char[] delimiterChars = { ' ' };

            input_substring = message.Content.Split(delimiterChars).ToList();

            string parsed_command_name = "";

            int argPos = 0;
            if (message.HasMentionPrefix(client.CurrentUser, ref argPos))
            {
                parsed_command_name = input_substring[1];
            }
            else
            {
                parsed_command_name = "None";
            }

            SocialLinkerCommand context_to_command = new SocialLinkerCommand
            {
                CommandType = "Context",
                CommandName = parsed_command_name,
                User = message.Author,
                Channel = message.Channel,
                MentionedUser = message.MentionedUsers.Skip(1).FirstOrDefault(),
                Attachments = message.Attachments,
                Message = message
            };

            return context_to_command;
        }
    }

    class CommandHandler
    {
        public DiscordShardedClient _client;
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
            _client.ShardReady += Shop;
            _client.ShardReady += Settings;
            _client.ShardReady += Help;
            _client.ShardReady += Maker_List;
            _client.ShardReady += Maker_Sheet;
            _client.ShardReady += Maker_Create;
            _client.ShardReady += Maker_Multi;
            _client.ShardReady += Hug;
            _client.ShardReady += Pat;
            _client.ShardReady += Punch;
            _client.ShardReady += Slap;
            _client.SlashCommandExecuted += SlashAnnex;
            
            await Task.CompletedTask;
        }

        private async Task SlashAnnex(SocketSlashCommand slash_command)
        {
            SocialLinkerCommand sl_command = CommandConverter.SlashCommandConverter(slash_command);

            SocialStats.AddProficiency(sl_command);
            SocialStats.AddDiligence(sl_command);
            Leveling.UserSentMessage(sl_command);

            await CommandIndex(sl_command);
        }

        private Task HandleCommandAsync(SocketMessage s)
        {
            try
            {
                _ = Task.Run(async () =>
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

                    var converted_sl_command = CommandConverter.ContextCommandConverter(msg, _client);

                    int argPos = 0;
                    if (msg.HasMentionPrefix(_client.CurrentUser, ref argPos))
                    {
                        try
                        {
                            // Process the command if there's actually a prefix. Otherwise, treat it as a normal message.
                            await CommandIndex(converted_sl_command);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }

                        //Add Proficiency to the user's account whenever a command is successfully used
                        SocialStats.AddProficiency(converted_sl_command);
                    }

                    //Calculate if the user gains Diligence for this message
                    SocialStats.AddDiligence(converted_sl_command);

                    //Leveling up manages the user's time caps, so make sure it comes after AddProficiency and AddDiligence have ran
                    Leveling.UserSentMessage(converted_sl_command);
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            
            return Task.CompletedTask;
        }

        private async Task CommandIndex(SocialLinkerCommand command)
        {
            var commandChannel = (SocketGuildChannel)command.Channel;
            bool validityCheck = true;

            if (command.CommandType == "Slash")
            {
                await command.SlashCommand.DeferAsync();
                await command.SlashCommand.DeleteOriginalResponseAsync();
            }

            switch (command.CommandName.ToLower())
            {
                // "status" and "profile" lead to the same command
                case "status":
                    await Commands.Status.ContentCheck(command);
                    SocialLinkerCommandLogging.LogData(command);
                    break;

                case "profile":
                    await Commands.Status.ContentCheck(command);
                    SocialLinkerCommandLogging.LogData(command);
                    break;

                case "shop":
                    await Commands.Shop.StartShop(command);
                    SocialLinkerCommandLogging.LogData(command);
                    break;

                // "settings" and "setting" lead to the same command
                case "settings":
                    await Commands.Settings.SettingsMenu(command);
                    SocialLinkerCommandLogging.LogData(command);
                    break;

                case "setting":
                    await Commands.Settings.SettingsMenu(command);
                    SocialLinkerCommandLogging.LogData(command);
                    break;

                case "config":
                    await Commands.Settings.SettingsMenu(command);
                    SocialLinkerCommandLogging.LogData(command);
                    break;

                case "help":
                    await Commands.Help.HelpMenu(command);
                    SocialLinkerCommandLogging.LogData(command);
                    break;

                case "menu":
                    await Commands.Help.HelpMenu(command);
                    SocialLinkerCommandLogging.LogData(command);
                    break;

                case "hug":
                    await Commands.Hug.HugCommand(command);
                    SocialLinkerCommandLogging.LogData(command);
                    break;

                case "pat":
                    await Commands.Pat.PatCommand(command);
                    SocialLinkerCommandLogging.LogData(command);
                    break;

                case "punch":
                    await Commands.Punch.PunchCommand(command);
                    SocialLinkerCommandLogging.LogData(command);
                    break;

                case "slap":
                    await Commands.Slap.SlapCommand(command);
                    SocialLinkerCommandLogging.LogData(command);
                    break;

                case "maker":
                    await Commands.Maker.MakerCommandParser(command);
                    SocialLinkerCommandLogging.LogData(command);
                    break;

                case "maker_list":
                    command.MakerCommand = SL_To_Maker_Command(command);
                    await Commands.Maker.MakerCommandParser(command);
                    SocialLinkerCommandLogging.LogData(command);
                    break;

                case "maker_sheet":
                    command.MakerCommand = SL_To_Maker_Command(command);
                    await Commands.Maker.MakerCommandParser(command);
                    SocialLinkerCommandLogging.LogData(command);
                    break;

                case "maker_create":
                    command.MakerCommand = SL_To_Maker_Command(command);
                    await Commands.Maker.MakerCommandParser(command);
                    SocialLinkerCommandLogging.LogData(command);
                    break;

                case "maker_multi":
                    await Commands.MakerMulti.MakerMultiMenu(command);
                    SocialLinkerCommandLogging.LogData(command);
                    break;

                case "update":
                    await Commands.DevCommands.UpdatePreReleaseAccounts(command);
                    break;

                case "calcexp":
                    await Commands.DevCommands.ExpCalculator(command);
                    break;

                case "calclevel":
                    await Commands.DevCommands.LevelCalculator(command);
                    break;

                case "fix":
                    Commands.DevCommands.CorrectMouthFrames(command);
                    break;

                default:
                    validityCheck = false;
                    break;
            }

            command.ValidCommand = validityCheck;
        }

        public static async Task Status(DiscordSocketClient client)
        {
            //await client.Rest.DeleteAllGlobalCommandsAsync();

            var guildCommand = new SlashCommandBuilder()
                .WithName("status")
                .WithDescription("View your status screen, or specify a user to view theirs.")
                .AddOption("user", ApplicationCommandOptionType.User, "The user whose status you want to view.", isRequired: false);

            try
            {
                await client.Rest.CreateGlobalCommand(guildCommand.Build());
            }
            catch (HttpException exception)
            {
                var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);
                Console.WriteLine(json);
            }
        }

        public static async Task Shop(DiscordSocketClient client)
        {
            var guildCommand = new SlashCommandBuilder()
                .WithName("shop")
                .WithDescription("Browse the décor shop.");

            try
            {
                await client.Rest.CreateGlobalCommand(guildCommand.Build());
            }
            catch (HttpException exception)
            {
                var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);
                Console.WriteLine(json);
            }
        }

        public static async Task Settings(DiscordSocketClient client)
        {
            var guildCommand = new SlashCommandBuilder()
                .WithName("settings")
                .WithDescription("Change and customize user settings.");

            try
            {
                await client.Rest.CreateGlobalCommand(guildCommand.Build());
            }
            catch (HttpException exception)
            {
                var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);
                Console.WriteLine(json);
            }
        }

        public static async Task Help(DiscordSocketClient client)
        {
            var guildCommand = new SlashCommandBuilder()
                .WithName("help")
                .WithDescription("Learn how to use Social Linker.");

            try
            {
                await client.Rest.CreateGlobalCommand(guildCommand.Build());
            }
            catch (HttpException exception)
            {
                var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);
                Console.WriteLine(json);
            }
        }

        public static async Task Hug(DiscordSocketClient client)
        {
            var guildCommand = new SlashCommandBuilder()
                .WithName("hug")
                .WithDescription("Give a user a hug.")
                .AddOption("user", ApplicationCommandOptionType.User, "The user you want to hug.", isRequired: true);

            try
            {
                await client.Rest.CreateGlobalCommand(guildCommand.Build());
            }
            catch (HttpException exception)
            {
                var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);
                Console.WriteLine(json);
            }
        }

        public static async Task Pat(DiscordSocketClient client)
        {
            var guildCommand = new SlashCommandBuilder()
                .WithName("pat")
                .WithDescription("Give a user a pat.")
                .AddOption("user", ApplicationCommandOptionType.User, "The user you want to pat.", isRequired: true);

            try
            {
                await client.Rest.CreateGlobalCommand(guildCommand.Build());
            }
            catch (HttpException exception)
            {
                var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);
                Console.WriteLine(json);
            }
        }

        public static async Task Punch(DiscordSocketClient client)
        {
            var guildCommand = new SlashCommandBuilder()
                .WithName("punch")
                .WithDescription("Give out unbridled violence.")
                .AddOption("user", ApplicationCommandOptionType.User, "The user you want to punch.", isRequired: true);

            try
            {
                await client.Rest.CreateGlobalCommand(guildCommand.Build());
            }
            catch (HttpException exception)
            {
                var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);
                Console.WriteLine(json);
            }
        }

        public static async Task Slap(DiscordSocketClient client)
        {
            var guildCommand = new SlashCommandBuilder()
                .WithName("slap")
                .WithDescription("Hand out a high five. In the face.")
                .AddOption("user", ApplicationCommandOptionType.User, "The user you want to slap.", isRequired: true);

            try
            {
                await client.Rest.CreateGlobalCommand(guildCommand.Build());
            }
            catch (HttpException exception)
            {
                var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);
                Console.WriteLine(json);
            }
        }

        public static async Task Maker_List(DiscordSocketClient client)
        {
            var guildCommand = new SlashCommandBuilder()
                .WithName("maker_list")
                .WithDescription("View the list of usable characters from a certain game.")
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("game")
                    .WithDescription("The game you wish to choose.")
                    .WithRequired(true)
                    .AddChoice("P1", 1)
                    .AddChoice("P2IS", 2)
                    .AddChoice("P2EP", 3)
                    .AddChoice("P3", 4)
                    .AddChoice("P4", 5)
                    .AddChoice("P4AU", 6)
                    .AddChoice("P4D", 7)
                    .AddChoice("P5", 8)
                    .AddChoice("P5S", 9)
                    .AddChoice("BBTAG", 10)
                    .WithType(ApplicationCommandOptionType.Integer)
                    );

            try
            {
                await client.Rest.CreateGlobalCommand(guildCommand.Build());
            }
            catch (HttpException exception)
            {
                var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);
                Console.WriteLine(json);
            }
        }

        public static async Task Maker_Sheet(DiscordSocketClient client)
        {
            var guildCommand = new SlashCommandBuilder()
                .WithName("maker_sheet")
                .WithDescription("View the sprite sheet for a character with optional animation frames.")
                .AddOption("character", ApplicationCommandOptionType.String, "Name of the character you wish to view.", isRequired: true)
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("character_version")
                    .WithDescription("Specifies the game a character's sprite sheet comes from.")
                    .WithRequired(false)
                    .AddChoice("P1", 1)
                    .AddChoice("P2IS", 2)
                    .AddChoice("P2EP", 3)
                    .AddChoice("P3", 4)
                    .AddChoice("P4", 5)
                    .AddChoice("P4AU", 6)
                    .AddChoice("P4D", 7)
                    .AddChoice("P5", 8)
                    .AddChoice("P5S", 9)
                    .AddChoice("BBTAG", 10)
                    .WithType(ApplicationCommandOptionType.Integer)
                    )
                .AddOption("sprite_number", ApplicationCommandOptionType.Integer, "View animation frames for a character's specific sprite.", isRequired: false);

            try
            {
                await client.Rest.CreateGlobalCommand(guildCommand.Build());
            }
            catch (HttpException exception)
            {
                var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);
                Console.WriteLine(json);
            }
        }

        public static async Task Maker_Create(DiscordSocketClient client)
        {
            try
            {
                var guildCommand = new SlashCommandBuilder()
                .WithName("maker_create")
                .WithDescription("Create a realistic screenshot from various Persona titles.")
                .AddOption("character", ApplicationCommandOptionType.String, "Name of the character you wish to use.", isRequired: true)
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("character_version")
                    .WithDescription("Specifies the game a character's sprite sheet comes from.")
                    .WithRequired(false)
                    .AddChoice("P1", 1)
                    .AddChoice("P2IS", 2)
                    .AddChoice("P2EP", 3)
                    .AddChoice("P3", 4)
                    .AddChoice("P4", 5)
                    .AddChoice("P4AU", 6)
                    .AddChoice("P4D", 7)
                    .AddChoice("P5", 8)
                    .AddChoice("P5S", 9)
                    .AddChoice("BBTAG", 10)
                    .WithType(ApplicationCommandOptionType.Integer)
                    )
                .AddOption("sprite_number", ApplicationCommandOptionType.Integer, "The specific sprite from the character's sprite sheet to use.", isRequired: true)
                .AddOption("eye_frame", ApplicationCommandOptionType.Integer, "Use an eye frame linked to the character's sprite.", isRequired: false)
                .AddOption("mouth_frame", ApplicationCommandOptionType.Integer, "Use a mouth frame linked to the character's sprite.", isRequired: false)
                .AddOption("dialogue", ApplicationCommandOptionType.String, "The character's spoken text.", isRequired: true)
                .AddOption("background", ApplicationCommandOptionType.Attachment, "Upload an image to use as a background.", isRequired: false);

                await client.Rest.CreateGlobalCommand(guildCommand.Build());
            }
            catch (HttpException exception)
            {
                var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);
                Console.WriteLine(json);
            }
        }

        public static async Task Maker_Multi(DiscordSocketClient client)
        {
            var guildCommand = new SlashCommandBuilder()
                .WithName("maker_multi")
                .WithDescription("Quickly create a scene with multiple characters.");

            try
            {
                await client.Rest.CreateGlobalCommand(guildCommand.Build());
            }
            catch (HttpException exception)
            {
                var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);
                Console.WriteLine(json);
            }
        }

        public static MakerCommandData SL_To_Maker_Command(SocialLinkerCommand sl_command)
        {
            MakerCommandData maker_command_data = new MakerCommandData()
            {
                Template = "",
                Character_Data = new MakerCharacterData()
                {
                    Character_Keyword = "",
                    Sprite_Set_Version = "",
                    Base_Sprite = default,
                    Eye_Frame = default,
                    Mouth_Frame = default,
                },
                Dialogue = "",
                Background = null
            };

            List<SocketSlashCommandDataOption> slash_command_data_options_list = sl_command.SlashCommand.Data.Options.ToList();

            for (int i = 0; i < slash_command_data_options_list.Count; i++)
            {
                switch (slash_command_data_options_list[i].Name)
                {
                    case "game":
                        maker_command_data.Template = Value_To_Template(slash_command_data_options_list[i].Value.ToString());
                        break;

                    case "character":
                        maker_command_data.Character_Data.Character_Keyword = slash_command_data_options_list[i].Value.ToString();
                        break;

                    case "character_version":
                        maker_command_data.Character_Data.Sprite_Set_Version = Value_To_Template(slash_command_data_options_list[i].Value.ToString());
                        break;

                    case "sprite_number":
                        maker_command_data.Character_Data.Base_Sprite = Convert.ToInt32(slash_command_data_options_list[i].Value.ToString());
                        break;

                    case "eye_frame":
                        maker_command_data.Character_Data.Eye_Frame = Convert.ToInt32(slash_command_data_options_list[i].Value.ToString());
                        break;

                    case "mouth_frame":
                        maker_command_data.Character_Data.Mouth_Frame = Convert.ToInt32(slash_command_data_options_list[i].Value.ToString());
                        break;

                    case "dialogue":
                        maker_command_data.Dialogue = slash_command_data_options_list[i].Value.ToString();
                        break;

                    case "background":
                        maker_command_data.Background = slash_command_data_options_list[i].Value as IAttachment;
                        break;
                } 
            }

            return maker_command_data;
        }

        public static string Value_To_Template(string value)
        {
            string template = "";

            switch (value)
            {
                case "1":
                    template = "P1";
                    break;

                case "2":
                    template = "P2IS";
                    break;

                case "3":
                    template = "P2EP";
                    break;

                case "4":
                    template = "P3";
                    break;

                case "5":
                    template = "P4";
                    break;

                case "6":
                    template = "P4AU";
                    break;

                case "7":
                    template = "P4D";
                    break;

                case "8":
                    template = "P5";
                    break;

                case "9":
                    template = "P5S";
                    break;

                case "10":
                    template = "BBTAG";
                    break;
            }

            return template;
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
