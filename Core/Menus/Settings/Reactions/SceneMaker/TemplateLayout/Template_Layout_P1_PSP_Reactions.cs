using System;
using System.Threading.Tasks;
using Discord.WebSocket;
using SocialLinker.Core.CloudStorageTables;
using SocialLinker.Core.Menus.Settings.Main;
using SocialLinker.Core.Menus.Settings.Main.SceneMaker;
using SocialLinker.Core.Menus.Settings.Main.SceneMaker.TemplateLayout;

namespace SocialLinker.Core.Menus.Settings.Reactions.SceneMaker.TemplateLayout
{
    class Template_Layout_P1_PSP_Reactions
    {
        public static Task Nav_Template_Layout_P1_PSP_Main(SocketReaction reaction, MenuIdStructure menuSession)
        {
            if (reaction.Emote.Name == "↩️")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Template_Layout_VC_Menu.Template_Layout_VC_P1_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            // Keycap One
            else if (reaction.Emote.Name == "\u0031\ufe0f\u20e3")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Template_Layout_P1_PSP_Menu.Template_Layout_P1_PSP_Moon_Phases(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            // Keycap Two
            else if (reaction.Emote.Name == "\u0032\ufe0f\u20e3")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Template_Layout_P1_PSP_Menu.Template_Layout_P1_PSP_Placement(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P1_PSP_Moon_Phases(SocketReaction reaction, MenuIdStructure menuSession)
        {
            if (reaction.Emote.Name == "↩️")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Template_Layout_P1_PSP_Menu.Template_Layout_P1_PSP_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            // Keycap One
            else if (reaction.Emote.Name == "\u0031\ufe0f\u20e3")
            {
                // Get the account information of the user.
                var account = UserInfoClasses.GetAccount(menuSession.User);

                // Assign the chosen setting to the user's account.
                account.P1_PSP_TS_Moon_HUD = "On";

                //Update the user's account.
                UserInfoClasses.UpdateAccount(account);

                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Template_Layout_P1_PSP_Menu.Template_Layout_P1_PSP_Moon_Phases_Confirm(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            // Keycap Two
            else if (reaction.Emote.Name == "\u0032\ufe0f\u20e3")
            {
                // Get the account information of the user.
                var account = UserInfoClasses.GetAccount(menuSession.User);

                // Assign the chosen setting to the user's account.
                account.P1_PSP_TS_Moon_HUD = "Off";

                //Update the user's account.
                UserInfoClasses.UpdateAccount(account);

                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Template_Layout_P1_PSP_Menu.Template_Layout_P1_PSP_Moon_Phases_Confirm(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P1_PSP_Placement(SocketReaction reaction, MenuIdStructure menuSession)
        {
            if (reaction.Emote.Name == "↩️")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Template_Layout_P1_PSP_Menu.Template_Layout_P1_PSP_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            // Keycap One
            else if (reaction.Emote.Name == "\u0031\ufe0f\u20e3")
            {
                // Get the account information of the user.
                var account = UserInfoClasses.GetAccount(menuSession.User);

                // Assign the chosen setting to the user's account.
                account.P1_PSP_TS_Position = "Left";

                //Update the user's account.
                UserInfoClasses.UpdateAccount(account);

                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Template_Layout_P1_PSP_Menu.Template_Layout_P1_PSP_Placement_Confirm(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            // Keycap Two
            else if (reaction.Emote.Name == "\u0032\ufe0f\u20e3")
            {
                // Get the account information of the user.
                var account = UserInfoClasses.GetAccount(menuSession.User);

                // Assign the chosen setting to the user's account.
                account.P1_PSP_TS_Position = "Center";

                //Update the user's account.
                UserInfoClasses.UpdateAccount(account);

                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Template_Layout_P1_PSP_Menu.Template_Layout_P1_PSP_Placement_Confirm(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            // Keycap Three
            else if (reaction.Emote.Name == "\u0033\ufe0f\u20e3")
            {
                // Get the account information of the user.
                var account = UserInfoClasses.GetAccount(menuSession.User);

                // Assign the chosen setting to the user's account.
                account.P1_PSP_TS_Position = "Right";

                //Update the user's account.
                UserInfoClasses.UpdateAccount(account);

                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Template_Layout_P1_PSP_Menu.Template_Layout_P1_PSP_Placement_Confirm(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            // Keycap Four
            else if (reaction.Emote.Name == "\u0034\ufe0f\u20e3")
            {
                // Get the account information of the user.
                var account = UserInfoClasses.GetAccount(menuSession.User);

                // Assign the chosen setting to the user's account.
                account.P1_PSP_TS_Position = "Switch";

                //Update the user's account.
                UserInfoClasses.UpdateAccount(account);

                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Template_Layout_P1_PSP_Menu.Template_Layout_P1_PSP_Placement_Confirm(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P1_PSP_Moon_Phases_Confirm(SocketReaction reaction, MenuIdStructure menuSession)
        {
            if (reaction.Emote.Name == "💠")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Template_Layout_P1_PSP_Menu.Template_Layout_P1_PSP_Main(menuSession.User, menuSession.MenuMessage);
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

        public static Task Nav_Template_Layout_P1_PSP_Placement_Confirm(SocketReaction reaction, MenuIdStructure menuSession)
        {
            if (reaction.Emote.Name == "💠")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Template_Layout_P1_PSP_Menu.Template_Layout_P1_PSP_Main(menuSession.User, menuSession.MenuMessage);
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
