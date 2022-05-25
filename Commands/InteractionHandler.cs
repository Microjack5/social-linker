using Discord;
using Discord.Interactions;
using Discord.Net;
using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialLinker.Commands
{
    public class InteractionHandler : InteractionModuleBase<SocketInteractionContext>
    {
        DiscordShardedClient _client;

        public async Task InitializeAsync(DiscordShardedClient client)
        {
            _client = client;
            _client.ShardReady += Status;
            /*_client.ShardReady += Shop;
            _client.ShardReady += Settings;
            _client.ShardReady += Help;
            _client.ShardReady += Hug;
            _client.ShardReady += Pat;
            _client.ShardReady += Punch;
            _client.ShardReady += Slap; */
            //_client.SlashCommandExecuted += SlashCommandIndex;
            await Task.CompletedTask;
        }

        /*private async Task SlashCommandIndex(SocketSlashCommand command)
        {
            try
            {
                switch (command.CommandName)
                {
                    case "status":
                        await command.RespondAsync(embed: Slash_Command_Successful().Build(), ephemeral: true);
                        SocialLinker.Commands.Slash.Status status_class = new SocialLinker.Commands.Slash.Status();
                        _ = status_class.StatusCommandParser1(command);
                        break;

                    case "shop":
                        await command.RespondAsync(embed: Slash_Command_Successful().Build(), ephemeral: true);
                        SocialLinker.Commands.Slash.Shop shop_class = new SocialLinker.Commands.Slash.Shop();
                        _ = shop_class.StartShop(command);
                        break;

                    case "settings":
                        break;

                    case "help":
                        await command.RespondAsync(embed: Slash_Command_Successful().Build(), ephemeral: true);
                        SocialLinker.Commands.Slash.Help help_class = new SocialLinker.Commands.Slash.Help();
                        _ = help_class.HelpMenu(command);
                        break;

                    case "hug":
                        await command.RespondAsync(embed: Slash_Command_Successful().Build(), ephemeral: true);
                        SocialLinker.Commands.Slash.Hug hug_class = new SocialLinker.Commands.Slash.Hug();
                        _ = hug_class.HugCommand(command);
                        break;

                    case "pat":
                        //await command.RespondAsync(embed: Slash_Command_Successful().Build(), ephemeral: true);
                        await command.RespondAsync("** **", ephemeral: true);
                        SocialLinker.Commands.Slash.Pat pat_class = new SocialLinker.Commands.Slash.Pat();
                        _ = pat_class.PatCommand(command);
                        break;

                    case "punch":
                        await command.RespondAsync(embed: Slash_Command_Successful().Build(), ephemeral: true);
                        SocialLinker.Commands.Slash.Punch punch_class = new SocialLinker.Commands.Slash.Punch();
                        _ = punch_class.PunchCommand(command);
                        break;

                    case "slap":
                        await command.RespondAsync(embed: Slash_Command_Successful().Build(), ephemeral: true);
                        SocialLinker.Commands.Slash.Slap slap_class = new SocialLinker.Commands.Slash.Slap();
                        _ = slap_class.SlapCommand(command);
                        break;

                    case "maker":
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            
            await Task.CompletedTask;
        } */

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
    }
}
