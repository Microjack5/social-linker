using System;
using System.Threading.Tasks;
using Discord.WebSocket;
using SocialLinker.Core.CloudStorageTables;
using SocialLinker.Core.Menus.Settings.Main.Profile;

namespace SocialLinker.Core.Menus.Settings.Reactions.Profile
{
    class Profile_Theme_Reactions
    {
        public static Task Nav_Profile_Theme_Main(SocketReaction reaction, MenuIdStructure menuSession)
        {
            if (reaction.Emote.Name == "↩️")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Profile_Settings_Menu.Profile_Settings_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            else if (reaction.Emote.Name == "P3")
            {
                // Get the account information of the user.
                var account = UserInfoClasses.GetAccount(menuSession.User);

                // Assign the chosen profile theme to the user's account.
                account.Profile_Theme = "P3";

                //Update the user's account.
                UserInfoClasses.UpdateAccount(account);

                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Profile_Theme_Menu.Profile_Theme_Confirm(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            else if (reaction.Emote.Name == "P4")
            {
                // Get the account information of the user.
                var account = UserInfoClasses.GetAccount(menuSession.User);

                // Assign the chosen profile theme to the user's account.
                account.Profile_Theme = "P4";

                //Update the user's account.
                UserInfoClasses.UpdateAccount(account);

                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Profile_Theme_Menu.Profile_Theme_Confirm(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            else if (reaction.Emote.Name == "P5")
            {
                // Get the account information of the user.
                var account = UserInfoClasses.GetAccount(menuSession.User);

                // Assign the chosen profile theme to the user's account.
                account.Profile_Theme = "P5";

                //Update the user's account.
                UserInfoClasses.UpdateAccount(account);

                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Profile_Theme_Menu.Profile_Theme_Confirm(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Profile_Theme_Confirm(SocketReaction reaction, MenuIdStructure menuSession)
        {
            if (reaction.Emote.Name == "💠")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Profile_Settings_Menu.Profile_Settings_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            else if (reaction.Emote.Name == "❌")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Attempt to delete the menu message from the channel if it hasn't been deleted by the user yet. If this fails, catch the exception.
                try
                {
                    _ = menuSession.MenuMessage.DeleteAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                // If the menu session is not null, remove it from the global list.
                if (menuSession != null)
                {
                    Global.MenuIdList.Remove(menuSession);
                }
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }
    }
}
