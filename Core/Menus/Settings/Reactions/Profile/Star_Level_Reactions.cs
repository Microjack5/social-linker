using System;
using System.Threading.Tasks;
using Discord.WebSocket;
using SocialLinker.Core.CloudStorageTables;
using SocialLinker.Core.Menus.Settings.Main.Profile;

namespace SocialLinker.Core.Menus.Settings.Reactions.Profile
{
    class Star_Level_Reactions
    {
        public static Task Nav_Star_Level_Main(SocketReaction reaction, MenuIdStructure menuSession)
        {
            if (reaction.Emote.Name == "↩️")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Profile_Settings_Menu.Profile_Settings_Main(menuSession);
                return Task.CompletedTask;
            }

            else if (reaction.Emote.Name == "✅")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Star_Level_Menu.Star_Level_Check(menuSession.User, menuSession.MenuMessage);

                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Star_Level_Check(SocketReaction reaction, MenuIdStructure menuSession)
        {
            if (reaction.Emote.Name == "💠")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Profile_Settings_Menu.Profile_Settings_Main(menuSession);
                return Task.CompletedTask;
            }

            else if (reaction.Emote.Name == "⭐" || reaction.Emote.Name == "🌟" || reaction.Emote.Name == "✨")
            {
                // Get the account information of the user.
                var account = UserInfoClasses.GetAccount(menuSession.User);

                // If the user is below Star Level Rank 2, reset their level and total EXP to their base values. 
                if (account.Level_Resets < 2)
                {
                    account.Level = 1;
                    account.Total_Exp = 0;
                }

                // If the user is currently at Star Level Rank 2, max out their P-Medal value.
                if (account.Level_Resets == 2)
                {
                    account.P_Medals = 999;
                }

                // Increase the user's level reset value by 1.
                account.Level_Resets += 1;

                //Update the user's account.
                UserInfoClasses.UpdateAccount(account);

                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Star_Level_Menu.Star_Level_Confirm(menuSession.User, menuSession.MenuMessage);

                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Star_Level_Confirm(SocketReaction reaction, MenuIdStructure menuSession)
        {
            if (reaction.Emote.Name == "💠")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Profile_Settings_Menu.Profile_Settings_Main(menuSession);
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
