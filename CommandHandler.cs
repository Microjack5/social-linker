using System;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using SocialLinker.Config;
using SocialLinker.Core.LevelSystem;
using Fergun.Interactive;

namespace SocialLinker
{
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

            _interactionHandler = new Commands.InteractionHandler();
            await _interactionHandler.InitializeAsync(_client);
            
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
