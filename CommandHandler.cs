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
using System.Timers;

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

        public static SocialLinkerCommand ContextCommandConverter(SocketMessage message)
        {
            List<string> input_substring;

            char[] delimiterChars = { ' ' };

            input_substring = message.Content.Split(delimiterChars).ToList();

            int prefix_length = $"{BotConfig.bot.cmdPrefix}".Length;

            string parsed_command_name = input_substring[0].Substring(prefix_length);

            SocialLinkerCommand context_to_command = new SocialLinkerCommand
            {
                CommandType = "Context",
                CommandName = parsed_command_name,
                User = message.Author,
                Channel = message.Channel,
                MentionedUser = message.MentionedUsers.FirstOrDefault(),
                Attachments = message.Attachments,
                Message = (SocketUserMessage)message
            };

            return context_to_command;
        }
    }

    class CommandHandler
    {
        DiscordShardedClient _client;
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
            _client.ShardReady += Hug;
            _client.ShardReady += Pat;
            _client.ShardReady += Punch;
            _client.ShardReady += Slap;
            _client.SlashCommandExecuted += SlashAnnex;
            
            await Task.CompletedTask;
        }

        private async Task SlashAnnex(SocketSlashCommand slash_command)
        {
            try
            {
                SocialLinkerCommand sl_command = CommandConverter.SlashCommandConverter(slash_command);
                await CommandIndex(sl_command);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

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

            int argPos = 0;
            if (msg.HasStringPrefix(BotConfig.bot.cmdPrefix, ref argPos))
            {
                try
                {
                    var converted_sl_command = CommandConverter.ContextCommandConverter(msg);
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
            var commandChannel = (SocketGuildChannel)command.Channel;
            ulong[] allowed_servers = new ulong[] { 543226698238394378, 488920941041025025, 981870056688480266 };

            if (allowed_servers.Contains(commandChannel.Guild.Id))
            {
                // Do nothing
            }
            else
            {
                await command.Channel.SendMessageAsync("Global slash commands are temporarily disabled for this server.");
                return;
            }

            if (command.CommandType == "Slash")
            {
                await command.SlashCommand.RespondAsync(embed: Slash_Command_Response().Build(), ephemeral: false);

                Timer notice_deletion_timer = new Timer()
                {
                    // Create a timer that expires as a "time out" duration for the user.
                    Interval = 5000,
                    AutoReset = false,
                    Enabled = true
                };

                // If the timer runs out, activate a function.
                notice_deletion_timer.Elapsed += (sender, e) => Timer_Elapsed(sender, e, command);
            }

            switch (command.CommandName)
            {
                case "status":
                    await Commands.Status.ContentCheck(command);
                    break; 

                case "shop":
                    await Commands.Shop.StartShop(command);
                    break;

                case "settings":
                    await Commands.Settings.SettingsMenu(command);
                    break;

                case "help":
                    await Commands.Help.HelpMenu(command);
                    break;

                case "hug":
                    await Commands.Hug.HugCommand(command);
                    break;

                case "pat":
                    await Commands.Pat.PatCommand(command);
                    break;

                case "punch":
                    await Commands.Punch.PunchCommand(command);
                    break;

                case "slap":
                    await Commands.Slap.SlapCommand(command);
                    break;

                case "maker":
                    await Commands.Maker.MakerCommandParser(command);
                    break;

                case "maker_list":
                    command.MakerCommand = SL_To_Maker_Command(command);
                    await Commands.Maker.MakerCommandParser(command);
                    break;

                case "maker_sheet":
                    command.MakerCommand = SL_To_Maker_Command(command);
                    await Commands.Maker.MakerCommandParser(command);
                    break;

                case "maker_create":
                    command.MakerCommand = SL_To_Maker_Command(command);
                    await Commands.Maker.MakerCommandParser(command);
                    break;
            }
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
                    .AddChoice("P1-PS1", 1)
                    .AddChoice("P1-PSP", 2)
                    .AddChoice("P2IS-PS1", 3)
                    .AddChoice("P2IS-PSP", 4)
                    .AddChoice("P2EP-PS1", 5)
                    .AddChoice("P2EP-PSP", 6)
                    .AddChoice("P3F", 7)
                    .AddChoice("P3P", 8)
                    .AddChoice("P4-PS2", 9)
                    .AddChoice("P4G", 10)
                    .AddChoice("P4AU", 11)
                    .AddChoice("P4D", 12)
                    .AddChoice("P5-PS4", 13)
                    .AddChoice("P5R", 14)
                    .AddChoice("P5S", 15)
                    .AddChoice("BBTAG", 16)
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
                .AddOption("character_version", ApplicationCommandOptionType.String, "Specifies the game a character's sprite sheet comes from.", isRequired: false)
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
                    .AddChoice("P1-PS1", 1)
                    .AddChoice("P1-PSP", 2)
                    .AddChoice("P2IS-PS1", 3)
                    .AddChoice("P2IS-PSP", 4)
                    .AddChoice("P2EP-PS1", 5)
                    .AddChoice("P2EP-PSP", 6)
                    .AddChoice("P3F", 7)
                    .AddChoice("P3P", 8)
                    .AddChoice("P4-PS2", 9)
                    .AddChoice("P4G", 10)
                    .AddChoice("P4AU", 11)
                    .AddChoice("P4D", 12)
                    .AddChoice("P5-PS4", 13)
                    .AddChoice("P5R", 14)
                    .AddChoice("P5S", 15)
                    .AddChoice("BBTAG", 16)
                    .WithType(ApplicationCommandOptionType.Integer)
                    )
                .AddOption("sprite_number", ApplicationCommandOptionType.Integer, "The specific sprite from the character's sprite sheet to use.", isRequired: true)
                .AddOption("eye_frame", ApplicationCommandOptionType.Integer, "Use an eye frame linked to the character's sprite.", isRequired: false)
                .AddOption("mouth_frame", ApplicationCommandOptionType.Integer, "Use a mouth frame linked to the character's sprite.", isRequired: false)
                .AddOption("dialogue", ApplicationCommandOptionType.String, "The character's spoken text.", isRequired: true);

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
                Character_Keyword = "",
                Sprite_Set_Version = "",
                Base_Sprite = default,
                Eye_Frame = default,
                Mouth_Frame = default,
                Dialogue = ""
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
                        maker_command_data.Character_Keyword = slash_command_data_options_list[i].Value.ToString();
                        break;

                    case "character_version":
                        maker_command_data.Sprite_Set_Version = Value_To_Template(slash_command_data_options_list[i].Value.ToString());
                        break;

                    case "sprite_number":
                        maker_command_data.Base_Sprite = Convert.ToInt32(slash_command_data_options_list[i].Value.ToString());
                        break;

                    case "eye_frame":
                        maker_command_data.Eye_Frame = Convert.ToInt32(slash_command_data_options_list[i].Value.ToString());
                        break;

                    case "mouth_frame":
                        maker_command_data.Mouth_Frame = Convert.ToInt32(slash_command_data_options_list[i].Value.ToString());
                        break;

                    case "dialogue":
                        maker_command_data.Dialogue = slash_command_data_options_list[i].Value.ToString();
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
                    template = "P1-PS1";
                    break;

                case "2":
                    template = "P1-PSP";
                    break;

                case "3":
                    template = "P2IS-PS1";
                    break;

                case "4":
                    template = "P2IS-PSP";
                    break;

                case "5":
                    template = "P2EP-PS1";
                    break;

                case "6":
                    template = "P2EP-PSP";
                    break;

                case "7":
                    template = "P3F";
                    break;

                case "8":
                    template = "P3P";
                    break;

                case "9":
                    template = "P4-PS2";
                    break;

                case "10":
                    template = "P4G";
                    break;

                case "11":
                    template = "P4AU";
                    break;

                case "12":
                    template = "P4D";
                    break;

                case "13":
                    template = "P5-PS4";
                    break;

                case "14":
                    template = "P5R";
                    break;

                case "15":
                    template = "P5S";
                    break;

                case "16":
                    template = "BBTAG";
                    break;
            }

            return template;
        }

        public static EmbedBuilder Slash_Command_Response()
        {
            var embed = new EmbedBuilder();

            embed.WithColor(0, 207, 41);
            embed.WithDescription("**Slash command processing** <a:Loading:983845611482783814>");

            return embed;
        }

        public static async void Timer_Elapsed(object sender, ElapsedEventArgs e, SocialLinkerCommand command)
        {
            try
            {
                await command.SlashCommand.DeleteOriginalResponseAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return;
            }
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
