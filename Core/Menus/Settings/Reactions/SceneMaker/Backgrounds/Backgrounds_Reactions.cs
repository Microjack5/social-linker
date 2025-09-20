using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord.WebSocket;
using SocialLinker.Core.CloudStorageTables;
using SocialLinker.Core.Menus.Settings.Main.SceneMaker;
using SocialLinker.Core.Menus.Settings.Main.SceneMaker.Backgrounds;

namespace SocialLinker.Core.Menus.Settings.Reactions.SceneMaker.Backgrounds
{
    class Backgrounds_Reactions
    {
        // Methods that activate on the ReactionAdded event.
        public static Task Nav_Backgrounds_Main(SocketReaction reaction, MenuIdStructure menuSession)
        {
            if (reaction.Emote.Name == "↩️")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = SM_Settings_Menu.SM_Settings_Main(menuSession);
                return Task.CompletedTask;
            }

            // Keycap One
            else if (reaction.Emote.Name == "\u0031\ufe0f\u20e3")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Backgrounds_Menu.Backgrounds_Default_Color(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            // Keycap Two
            else if (reaction.Emote.Name == "\u0032\ufe0f\u20e3")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Backgrounds_Menu.Backgrounds_Upload_Settings(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Backgrounds_Default_Color(SocketReaction reaction, MenuIdStructure menuSession)
        {
            if (reaction.Emote.Name == "↩️")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Backgrounds_Menu.Backgrounds_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            else if (reaction.Emote.Name == "🔳")
            {
                // Get the account information of the user.
                var account = UserInfoClasses.GetAccount(menuSession.User);

                // Assign the chosen setting to the user's account.
                account.Setting_BG_Color = "Transparent";

                //Update the user's account.
                UserInfoClasses.UpdateAccount(account);

                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Backgrounds_Menu.Backgrounds_Default_Color_Confirm(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Backgrounds_Upload_Settings(SocketReaction reaction, MenuIdStructure menuSession)
        {
            if (reaction.Emote.Name == "↩️")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Backgrounds_Menu.Backgrounds_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            // Keycap One
            else if (reaction.Emote.Name == "\u0031\ufe0f\u20e3")
            {
                // Get the account information of the user.
                var account = UserInfoClasses.GetAccount(menuSession.User);

                // Assign the chosen setting to the user's account.
                account.Setting_BG_Upload = "Scale to Width";

                //Update the user's account.
                UserInfoClasses.UpdateAccount(account);

                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Backgrounds_Menu.Backgrounds_Upload_Settings_Confirm(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            // Keycap Two
            else if (reaction.Emote.Name == "\u0032\ufe0f\u20e3")
            {
                // Get the account information of the user.
                var account = UserInfoClasses.GetAccount(menuSession.User);

                // Assign the chosen setting to the user's account.
                account.Setting_BG_Upload = "Scale to Height";

                //Update the user's account.
                UserInfoClasses.UpdateAccount(account);

                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Backgrounds_Menu.Backgrounds_Upload_Settings_Confirm(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            // Keycap Three
            else if (reaction.Emote.Name == "\u0033\ufe0f\u20e3")
            {
                // Get the account information of the user.
                var account = UserInfoClasses.GetAccount(menuSession.User);

                // Assign the chosen setting to the user's account.
                account.Setting_BG_Upload = "Scale to Fit";

                //Update the user's account.
                UserInfoClasses.UpdateAccount(account);

                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Backgrounds_Menu.Backgrounds_Upload_Settings_Confirm(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            // Keycap Four
            else if (reaction.Emote.Name == "\u0034\ufe0f\u20e3")
            {
                // Get the account information of the user.
                var account = UserInfoClasses.GetAccount(menuSession.User);

                // Assign the chosen setting to the user's account.
                account.Setting_BG_Upload = "Scale to Fill";

                //Update the user's account.
                UserInfoClasses.UpdateAccount(account);

                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Backgrounds_Menu.Backgrounds_Upload_Settings_Confirm(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            // Keycap Five
            else if (reaction.Emote.Name == "\u0035\ufe0f\u20e3")
            {
                // Get the account information of the user.
                var account = UserInfoClasses.GetAccount(menuSession.User);

                // Assign the chosen setting to the user's account.
                account.Setting_BG_Upload = "Stretch to Fill";

                //Update the user's account.
                UserInfoClasses.UpdateAccount(account);

                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Backgrounds_Menu.Backgrounds_Upload_Settings_Confirm(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Backgrounds_Default_Color_Error(SocketReaction reaction, MenuIdStructure menuSession)
        {
            if (reaction.Emote.Name == "↩️")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Backgrounds_Menu.Backgrounds_Default_Color(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Backgrounds_Default_Color_Confirm(SocketReaction reaction, MenuIdStructure menuSession)
        {
            if (reaction.Emote.Name == "💠")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Backgrounds_Menu.Backgrounds_Main(menuSession.User, menuSession.MenuMessage);
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

        public static Task Nav_Backgrounds_Upload_Settings_Confirm(SocketReaction reaction, MenuIdStructure menuSession)
        {
            if (reaction.Emote.Name == "💠")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Backgrounds_Menu.Backgrounds_Main(menuSession.User, menuSession.MenuMessage);
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

        // Methods that activate on the MessageReceived event.
        public static Task Nav_Backgrounds_Default_Color_Message_Received(SocketMessage message, MenuIdStructure menuSession)
        {
            // Store the text of the message in a string and convert all letters to uppercase.
            string input_string = message.Content.ToUpper();

            input_string = Global.RemoveBotMention(input_string).Trim();

            // Take a substring consisting of the first character of the input string.
            string first_char = input_string.Substring(0, 1);

            // If the first character is not the pound symbol, add it to the beginning of the input string.
            if (first_char != "#")
            {
                input_string = "#" + input_string;
            }

            // Check if the input string is a valid HTML color.
            if (CheckValidFormatHtmlColor(input_string) == true)
            {
                // Get the account information of the user.
                var account = UserInfoClasses.GetAccount(menuSession.User);

                // Assign the chosen setting to the user's account.
                account.Setting_BG_Color = input_string;

                //Update the user's account.
                UserInfoClasses.UpdateAccount(account);

                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Backgrounds_Menu.Backgrounds_Default_Color_Confirm(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }
            // If the input string is not a valid HTML color, go to an error menu.
            else if (CheckValidFormatHtmlColor(input_string) == false)
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Backgrounds_Menu.Backgrounds_Default_Color_Error(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        // Methods that suppliment the functionality of the menus.
        // Method from https://stackoverflow.com/a/13035186/7138583
        protected static bool CheckValidFormatHtmlColor(string inputColor)
        {
            //regex from http://stackoverflow.com/a/1636354/2343
            if (Regex.Match(inputColor, "^#(?:[0-9a-fA-F]{3}){1,2}$").Success)
                return true;

            var result = System.Drawing.Color.FromName(inputColor);
            return result.IsKnownColor;
        }
    }
}
