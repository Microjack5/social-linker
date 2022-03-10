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
            _client.ShardReady += Shop;
            _client.ShardReady += Settings;
            _client.ShardReady += Help;
            _client.SlashCommandExecuted += SlashCommandIndex;
            await Task.CompletedTask;
        }

        private async Task SlashCommandIndex(SocketSlashCommand command)
        {
            await command.RespondAsync($"You executed {command.Data.Name}");
        }

        public static async Task Status(DiscordSocketClient client)
        {
            ulong guildId = 543226698238394378;

            var guildCommand = new SlashCommandBuilder()
                .WithName("status")
                .WithDescription("View your status screen or someone else's.")
                .AddOption("user", ApplicationCommandOptionType.User, "The user whose status screen you want to view.", isRequired: false);

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
    }
}
