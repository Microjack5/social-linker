using Discord.WebSocket;
using SocialLinker.Core.CloudStorageTables;
using SocialLinker.Core.Menus.Settings.Main.SceneMaker;
using SocialLinker.Core.Menus.Settings.Main.SceneMaker.Backgrounds;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SocialLinker.Core.Menus.Settings.Reactions.SceneMaker.Backgrounds
{
    class Backgrounds_Reactions
    {
        public static Task Nav_Backgrounds_Main(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            string selected = component.Data.Values.First();

            switch (selected)
            {
                case "1":
                    _ = Backgrounds_Menu.Backgrounds_Default_Color(menuSession);
                    break;
                case "2":
                    _ = Backgrounds_Menu.Backgrounds_Upload_Settings(menuSession);
                    break;
                case "return":
                    _ = SM_Settings_Menu.SM_Settings_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Backgrounds_Default_Color(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            var account = menuSession.Account;

            switch (component.Data.CustomId)
            {
                case "color-code-modal-open":
                    _ = Backgrounds_Menu.Backgrounds_Default_Color_Modal(component);
                    break;

                case "reset-background":
                    account.Setting_BG_Color = "Transparent";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Backgrounds_Menu.Backgrounds_Default_Color_Confirm(menuSession);
                    break;

                case "back-to-background-settings":
                    _ = Backgrounds_Menu.Backgrounds_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static async Task Nav_Backgrounds_Default_Color_Modal(SocketModal modal, MenuIdStructure menuSession)
        {
            var account = UserInfoClasses.GetAccount(menuSession.User);

            var color_code = modal.Data.Components
            .FirstOrDefault(x => x.CustomId == "color_code")?.Value;

            await Nav_Backgrounds_Default_Color_Logic(menuSession, color_code);
            return;
        }

        public static Task Nav_Backgrounds_Upload_Settings(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            string selected = component.Data.Values.First();
            var account = menuSession.Account;

            switch (selected)
            {
                case "1":
                    account.Setting_BG_Upload = "Scale to Width";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Backgrounds_Menu.Backgrounds_Upload_Settings_Confirm(menuSession);
                    break;
                case "2":
                    account.Setting_BG_Upload = "Scale to Height";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Backgrounds_Menu.Backgrounds_Upload_Settings_Confirm(menuSession);
                    break;
                case "3":
                    account.Setting_BG_Upload = "Scale to Fit";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Backgrounds_Menu.Backgrounds_Upload_Settings_Confirm(menuSession);
                    break;
                case "4":
                    account.Setting_BG_Upload = "Scale to Fill";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Backgrounds_Menu.Backgrounds_Upload_Settings_Confirm(menuSession);
                    break;
                case "5":
                    account.Setting_BG_Upload = "Stretch to Fill";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Backgrounds_Menu.Backgrounds_Upload_Settings_Confirm(menuSession);
                    break;
                case "return":
                    _ = Backgrounds_Menu.Backgrounds_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Backgrounds_Default_Color_Error(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "retry":
                    _ = Backgrounds_Menu.Backgrounds_Default_Color(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Backgrounds_Default_Color_Confirm(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "back-to-background-settings":
                    _ = Backgrounds_Menu.Backgrounds_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Backgrounds_Upload_Settings_Confirm(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "back-to-background-settings":
                    _ = Backgrounds_Menu.Backgrounds_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        // Utility method
        public static Task Nav_Backgrounds_Default_Color_Logic(MenuIdStructure menuSession, string color_code)
        {
            var account = UserInfoClasses.GetAccount(menuSession.User);

            color_code = color_code.ToUpper();

            string first_char = color_code.Substring(0, 1);

            // If the first character is not the pound symbol, add it to the beginning of the input string.
            if (first_char != "#")
            {
                color_code = "#" + color_code;
            }

            // Check if the input string is a valid HTML color.
            if (CheckValidFormatHtmlColor(color_code) == true)
            {
                account.Setting_BG_Color = color_code;
                UserInfoClasses.UpdateAccount(account);

                menuSession.Account = account;
                _ = Backgrounds_Menu.Backgrounds_Default_Color_Confirm(menuSession);
            }
            else if (CheckValidFormatHtmlColor(color_code) == false)
            {
                _ = Backgrounds_Menu.Backgrounds_Default_Color_Error(menuSession);
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
