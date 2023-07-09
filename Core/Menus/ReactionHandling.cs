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
                    // Check if the bot has permissions to manage messages in the channel.
                    await ErrorHandling.PermissionCheck(message);

                    Console.WriteLine(ex);

                    // Exit the loop.
                    break;
                }
            }
        }
    }
}
