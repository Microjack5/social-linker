using System;
using System.Threading.Tasks;
using Discord.WebSocket;
using SocialLinker.Core.CloudStorageTables;
using SocialLinker.Core.Menus.Settings.Main.General;

namespace SocialLinker.Core.Menus.Settings.Reactions.General
{
    class Rank_Up_Notifications_Reactions
    {
        public static Task Nav_Rank_Up_Notifications_Main(SocketReaction reaction, MenuIdStructure menuSession)
        {
            if (reaction.Emote.Name == "↩️")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = General_Settings_Menu.General_Settings_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            // Keycap One
            else if (reaction.Emote.Name == "\u0031\ufe0f\u20e3")
            {
                // Get the account information of the user.
                var account = UserInfoClasses.GetAccount(menuSession.User);

                // Change the user's rank up notification setting to "On".
                account.Rank_Up_Notifications = "On";

                //Update the user's account.
                UserInfoClasses.UpdateAccount(account);

                // Stop the timeout timer associated with the menu
                menuSession.MenuTimer.Stop();

                // Go to a new menu
                _ = Rank_Up_Notifications_Menu.Rank_Up_Notifications_Confirm(menuSession.User, menuSession.MenuMessage);

                return Task.CompletedTask;
            }

            // Keycap Two
            else if (reaction.Emote.Name == "\u0032\ufe0f\u20e3")
            {
                // Get the account information of the user.
                var account = UserInfoClasses.GetAccount(menuSession.User);

                // Change the user's rank up notification setting to "Off".
                account.Rank_Up_Notifications = "Off";

                //Update the user's account.
                UserInfoClasses.UpdateAccount(account);

                // Stop the timeout timer associated with the menu
                menuSession.MenuTimer.Stop();

                // Go to a new menu
                _ = Rank_Up_Notifications_Menu.Rank_Up_Notifications_Confirm(menuSession.User, menuSession.MenuMessage);

                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Rank_Up_Notifications_Confirm(SocketReaction reaction, MenuIdStructure menuSession)
        {
            if (reaction.Emote.Name == "💠")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = General_Settings_Menu.General_Settings_Main(menuSession.User, menuSession.MenuMessage);
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
