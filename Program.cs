using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using SocialLinker.Config;

namespace SocialLinker
{
    class Program
    {
        DiscordShardedClient _client;
        CommandHandler _handler;

        static void Main(string[] args)
        => new Program().StartAsync().GetAwaiter().GetResult();

        public async Task StartAsync()
        {
            if (BotConfig.bot.token == "" || BotConfig.bot.token == null) return;
            _client = new DiscordShardedClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose,
                TotalShards = 1
            });
            _client.Log += Log;
            _client.ReactionAdded += Core.Menus.MenuDirectory.ReactionAddedIndex;
            _client.ReactionRemoved += Core.Menus.MenuDirectory.ReactionRemovedIndex;
            _client.MessageReceived += Core.Menus.MenuDirectory.MessageReceivedIndex;
            await _client.LoginAsync(TokenType.Bot, BotConfig.bot.token);
            await _client.StartAsync();
            await _client.SetGameAsync("......");
            _handler = new CommandHandler();
            await _handler.InitializeAsync(_client);
            await Task.Delay(-1);
        }

        private async Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.Message);
            await Task.CompletedTask;
        }
    }
}
