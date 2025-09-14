using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using SocialLinker.Cooldown;
using SocialLinker.Core.Menus.Settings.Main;
using SocialLinker.Core.Menus.InitialUsage.Main;
using SocialLinker.Core.CloudStorageTables;

namespace SocialLinker.Commands
{
    public class Settings : ModuleBase<SocketCommandContext>
    {
        public static async Task SettingsMenu(SocialLinkerCommand command)
        {
            // If there is a cooldown session active for the command type "menu", return the method immediately.
            if (await UserCooldownMethods.IsCooldownActive(command, "menu") == true)
            {
                return;
            }

            // Get the account information of the command's user.
            var command_user_account = UserInfoClasses.GetAccount(command.User);

            // Check if the user's account has been activated. If not, send them to the initial usage setup menu.
            if (command_user_account.Account_Activated == "No")
            {
                await First_Use_Content_Filter_Menu.First_Use_Intro_Initialize(command);
                return;
            }

            // End of initial usage and cooldown checks.

            // Create two variables to check if there is a menu list entry with either the current channel ID or current user ID.
            var channelSearch = Global.MenuIdList.SingleOrDefault(x => x.MenuMessage.Channel.Id == command.Channel.Id);
            var userSearch = Global.MenuIdList.SingleOrDefault(x => x.User.Id == command.User.Id);

            // If the channel entry exists and the user is not the same, send an error message.
            if (channelSearch != null && channelSearch.User.Id != command.User.Id)
            {
                // Case 1: Search by channel successful, user ID does not match. Create new entry for new user.
                // Create a new menu in the current channel.
                await Settings_Menu.Settings_Start((SocketTextChannel)command.Channel, (SocketGuildUser)command.User);
                return;
            }
            // Else, if the channel entry exists and the user is the same, assume they want to reset the menu and delete the previous entry.
            else if (channelSearch != null && channelSearch.User.Id == command.User.Id)
            {
                // Case 2: Search by channel successful, user ID matches. Resetting menu in same channel.
                // Attempt deleting the message if it hasn't been deleted by the user yet.
                try
                {
                    // Delete the currently active menu.
                    await channelSearch.MenuMessage.DeleteAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                // Stop the timeout timer associated with the menu.
                channelSearch.MenuTimer.Stop();

                // Remove the menu entry from the global list.
                Global.MenuIdList.Remove(channelSearch);

                // Create a new menu in the current channel.
                await Settings_Menu.Settings_Start((SocketTextChannel)command.Channel, (SocketGuildUser)command.User);
                return;
            }
            // Else, if an entry exists where the user is found but they're in a different channel now, delete previous entry and reset the menu.
            else if (userSearch != null && userSearch.MenuMessage.Channel.Id != command.Channel.Id)
            {
                // Case 3: Search by user successful, channel ID does not match. Resetting menu in new channel.
                // Attempt deleting the message if it hasn't been deleted by the user yet.
                try
                {
                    // Delete the currently active menu.
                    await userSearch.MenuMessage.DeleteAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                // Stop the timeout timer associated with the menu.
                userSearch.MenuTimer.Stop();

                // Remove the menu entry from the global list.
                Global.MenuIdList.Remove(userSearch);

                // Create a new menu in the current channel.
                await Settings_Menu.Settings_Start((SocketTextChannel)command.Channel, (SocketGuildUser)command.User);
                return;
            }
            // For any other condition (if one should exist and not be handled here), create a new menu entry.
            else
            {
                // Case 4: No previous entry found. Create new entry.
                // Create a new menu in the current channel.
                await Settings_Menu.Settings_Start((SocketTextChannel)command.Channel, (SocketGuildUser)command.User);
                return;
            }
        }
    }
}
