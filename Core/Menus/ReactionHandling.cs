using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using SocialLinker.Config;
using Discord.Rest;

namespace SocialLinker.Core.Menus
{
    public class ReactionHandling
    {
        public static async Task AddReactionsToMenu(RestUserMessage message, List<IEmote> reaction_list)
        {
            // Establish a loop that iterates through every index of the list of reactions.
            for (int i = 0; i < reaction_list.Count; i++)
            {
                // Attempt to add the current iterated reaction to the message.
                try
                {
                    System.Threading.Thread.Sleep(MenuConfig.menu.reactionAddedDelay);
                    await message.AddReactionAsync(reaction_list[i]);
                }
                // If it fails, catch the exception and try to determine what the issue is.
                catch (Exception ex)
                {
                    // Get the channel the message belongs to.
                    var channel = message.Channel as SocketGuildChannel;

                    // Get the bot's user information.
                    var bot = channel.GetUser(BotConfig.bot.id);

                    // Check if the bot has permissions to add reactions in the channel.
                    if (bot.GetPermissions(channel).AddReactions == false)
                    {
                        // If not, send an error message to the channel.
                        await ErrorHandling.AddReactionsError((SocketTextChannel)message.Channel);
                    }
                    // If so, the failure must have come from the message being deleted.
                    else
                    {
                        // Do nothing
                    }

                    Console.WriteLine(ex);

                    // Exit the loop.
                    break;
                }
            }
        }
    }
}
