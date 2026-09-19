using Discord.WebSocket;
using SocialLinker.Core.CloudStorageTables;
using SocialLinker.Core.Menus.Settings.Main.SceneMaker;
using SocialLinker.Core.Menus.Settings.Main.SceneMaker.TemplateLayout;
using System.Linq;
using System.Threading.Tasks;

namespace SocialLinker.Core.Menus.Settings.Reactions.SceneMaker.TemplateLayout
{
    class Template_Layout_P3R_Reactions
    {
        public static Task Nav_Template_Layout_P3R_Main(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            string selected = component.Data.Values.First();

            switch (selected)
            {
                case "1":
                    _ = Template_Layout_P3R_Menu.Template_Layout_P3R_Date_Moon(menuSession);
                    break;
                case "2":
                    _ = Template_Layout_P3R_Menu.Template_Layout_P3R_Portrait_Lighting(menuSession);
                    break;
                case "3":
                    _ = Template_Layout_P3R_Menu.Template_Layout_P3R_Control_Panel(menuSession);
                    break;
                case "4":
                    _ = Template_Layout_P3R_Menu.Template_Layout_P3R_Auto_Advance(menuSession);
                    break;
                case "5":
                    _ = Template_Layout_P3R_Menu.Template_Layout_P3R_Low_Latency(menuSession);
                    break;
                case "return":
                    _ = Template_Layout_VC_Menu.Template_Layout_VC_P3_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P3R_Date_Moon(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            string selected = component.Data.Values.First();
            var account = menuSession.Account;

            switch (selected)
            {
                case "1":
                    account.P3R_TS_HUD = "Display All";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P3R_Menu.Template_Layout_P3R_Date_Moon_Confirm(menuSession);
                    break;
                case "2":
                    account.P3R_TS_HUD = "Countdown Off";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P3R_Menu.Template_Layout_P3R_Date_Moon_Confirm(menuSession);
                    break;
                case "3":
                    account.P3R_TS_HUD = "None";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P3R_Menu.Template_Layout_P3R_Date_Moon_Confirm(menuSession);
                    break;
                case "return":
                    _ = Template_Layout_P3R_Menu.Template_Layout_P3R_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P3R_Portrait_Lighting(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            string selected = component.Data.Values.First();
            var account = menuSession.Account;

            switch (selected)
            {
                case "1":
                    account.P3R_TS_Portrait_Lighting_Type = "Default";
                    account.P3R_TS_Low_Latency = "Off";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P3R_Menu.Template_Layout_P3R_Portrait_Lighting_Confirm(menuSession);
                    break;
                case "2":
                    account.P3R_TS_Portrait_Lighting_Type = "Background-Based";
                    account.P3R_TS_Low_Latency = "Off";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P3R_Menu.Template_Layout_P3R_Portrait_Lighting_Confirm(menuSession);
                    break;
                case "3":
                    _ = Template_Layout_P3R_Menu.Template_Layout_P3R_Custom_Lighting(menuSession);
                    break;
                case "4":
                    account.P3R_TS_Portrait_Lighting_Type = "None";
                    account.P3R_TS_Low_Latency = "On";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P3R_Menu.Template_Layout_P3R_Portrait_Lighting_Confirm(menuSession);
                    break;
                case "return":
                    _ = Template_Layout_P3R_Menu.Template_Layout_P3R_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P3R_Custom_Lighting(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            string selected = component.Data.Values.First();
            var account = menuSession.Account;

            switch (selected)
            {
                case "1":
                    _ = Template_Layout_P3R_Menu.Template_Layout_P3R_Custom_Lighting_Toggle(menuSession);
                    break;
                case "2":
                    _ = Template_Layout_P3R_Menu.Template_Layout_P3R_Custom_Lighting_Colors(menuSession);
                    break;
                case "return":
                    _ = Template_Layout_P3R_Menu.Template_Layout_P3R_Portrait_Lighting(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P3R_Custom_Lighting_Toggle(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            var account = menuSession.Account;

            switch (component.Data.CustomId)
            {
                case "return":
                    _ = Template_Layout_P3R_Menu.Template_Layout_P3R_Custom_Lighting(menuSession);
                    break;

                case "on":
                    account.P3R_TS_Portrait_Lighting_Type = "Custom";
                    account.P3R_TS_Low_Latency = "Off";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P3R_Menu.Template_Layout_P3R_Custom_Lighting_Toggle_Confirm(menuSession);
                    break;

                case "off":
                    account.P3R_TS_Portrait_Lighting_Type = "Default";
                    account.P3R_TS_Low_Latency = "Off";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P3R_Menu.Template_Layout_P3R_Custom_Lighting_Toggle_Confirm(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P3R_Custom_Lighting_Colors(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            var account = menuSession.Account;

            switch (component.Data.CustomId)
            {
                case "base-color-code-modal-open":
                    _ = Template_Layout_P3R_Menu.Template_Layout_P3R_Custom_Lighting_Colors_Base_Modal(component);
                    break;

                case "rim-color-code-modal-open":
                    _ = Template_Layout_P3R_Menu.Template_Layout_P3R_Custom_Lighting_Colors_Rim_Modal(component);
                    break;

                case "reset-colors":
                    account.P3R_TS_Portrait_Lighting_Custom_Base = "#FFFFFF";
                    account.P3R_TS_Portrait_Lighting_Custom_Rim = "#FFFFFF";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P3R_Menu.Template_Layout_P3R_Custom_Lighting_Colors_Reset_Confirm(menuSession);
                    break;

                case "back-to-p3r-custom-lighting":
                    _ = Template_Layout_P3R_Menu.Template_Layout_P3R_Custom_Lighting(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static async Task Nav_Template_Layout_P3R_Custom_Lighting_Colors_Base_Modal(SocketModal modal, MenuIdStructure menuSession)
        {
            var account = UserInfoClasses.GetAccount(menuSession.User);

            var color_code = modal.Data.Components
            .FirstOrDefault(x => x.CustomId == "base_color_code")?.Value;

            await Nav_Template_Layout_P3R_Custom_Lighting_Colors_Logic(menuSession, color_code, "base_color_code");
            return;
        }

        public static async Task Nav_Template_Layout_P3R_Custom_Lighting_Colors_Rim_Modal(SocketModal modal, MenuIdStructure menuSession)
        {
            var account = UserInfoClasses.GetAccount(menuSession.User);

            var color_code = modal.Data.Components
            .FirstOrDefault(x => x.CustomId == "rim_color_code")?.Value;

            await Nav_Template_Layout_P3R_Custom_Lighting_Colors_Logic(menuSession, color_code, "rim_color_code");
            return;
        }

        public static Task Nav_Template_Layout_P3R_Custom_Lighting_Colors_Error(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "retry":
                    _ = Template_Layout_P3R_Menu.Template_Layout_P3R_Custom_Lighting_Colors(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P3R_Control_Panel(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            string selected = component.Data.Values.First();
            var account = menuSession.Account;

            switch (selected)
            {
                case "1":
                    account.P3R_TS_Panel = "Xbox Series X|S";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P3R_Menu.Template_Layout_P3R_Control_Panel_Confirm(menuSession);
                    break;
                case "2":
                    account.P3R_TS_Panel = "PlayStation®️ 5";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P3R_Menu.Template_Layout_P3R_Control_Panel_Confirm(menuSession);
                    break;
                case "3":
                    account.P3R_TS_Panel = "PlayStation®️ 4";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P3R_Menu.Template_Layout_P3R_Control_Panel_Confirm(menuSession);
                    break;
                case "4":
                    account.P3R_TS_Panel = "Nintendo Switch 2";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P3R_Menu.Template_Layout_P3R_Control_Panel_Confirm(menuSession);
                    break;
                case "5":
                    account.P3R_TS_Panel = "Keyboard";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P3R_Menu.Template_Layout_P3R_Control_Panel_Confirm(menuSession);
                    break;
                case "6":
                    account.P3R_TS_Panel = "None";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P3R_Menu.Template_Layout_P3R_Control_Panel_Confirm(menuSession);
                    break;
                case "return":
                    _ = Template_Layout_P3R_Menu.Template_Layout_P3R_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P3R_Auto_Advance(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            var account = menuSession.Account;

            switch (component.Data.CustomId)
            {
                case "return":
                    _ = Template_Layout_P3R_Menu.Template_Layout_P3R_Main(menuSession);
                    break;

                case "on":
                    account.P3R_TS_Auto_Advance = "On";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P3R_Menu.Template_Layout_P3R_Auto_Advance_Confirm(menuSession);
                    break;

                case "off":
                    account.P3R_TS_Auto_Advance = "Off";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P3R_Menu.Template_Layout_P3R_Auto_Advance_Confirm(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P3R_Low_Latency(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            var account = menuSession.Account;

            switch (component.Data.CustomId)
            {
                case "return":
                    _ = Template_Layout_P3R_Menu.Template_Layout_P3R_Main(menuSession);
                    break;

                case "on":
                    account.P3R_TS_Low_Latency = "On";
                    account.P3R_TS_Portrait_Lighting_Type = "None";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P3R_Menu.Template_Layout_P3R_Low_Latency_Confirm(menuSession);
                    break;

                case "off":
                    account.P3R_TS_Low_Latency = "Off";
                    account.P3R_TS_Portrait_Lighting_Type = "Default";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P3R_Menu.Template_Layout_P3R_Low_Latency_Confirm(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P3R_Date_Moon_Confirm(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "back-to-p3r-template-settings":
                    _ = Template_Layout_P3R_Menu.Template_Layout_P3R_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P3R_Portrait_Lighting_Confirm(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "back-to-p3r-template-settings":
                    _ = Template_Layout_P3R_Menu.Template_Layout_P3R_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P3R_Custom_Lighting_Toggle_Confirm(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "back-to-p3r-custom-lighting":
                    _ = Template_Layout_P3R_Menu.Template_Layout_P3R_Custom_Lighting(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P3R_Custom_Lighting_Colors_Base_Confirm(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "back-to-p3r-custom-lighting-colors":
                    _ = Template_Layout_P3R_Menu.Template_Layout_P3R_Custom_Lighting_Colors(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P3R_Custom_Lighting_Colors_Rim_Confirm(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "back-to-p3r-custom-lighting-colors":
                    _ = Template_Layout_P3R_Menu.Template_Layout_P3R_Custom_Lighting_Colors(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P3R_Custom_Lighting_Colors_Reset_Confirm(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "back-to-p3r-custom-lighting-colors":
                    _ = Template_Layout_P3R_Menu.Template_Layout_P3R_Custom_Lighting_Colors(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P3R_Control_Panel_Confirm(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "back-to-p3r-template-settings":
                    _ = Template_Layout_P3R_Menu.Template_Layout_P3R_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P3R_Auto_Advance_Confirm(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "back-to-p3r-template-settings":
                    _ = Template_Layout_P3R_Menu.Template_Layout_P3R_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P3R_Low_Latency_Confirm(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "back-to-p3r-template-settings":
                    _ = Template_Layout_P3R_Menu.Template_Layout_P3R_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        // Utility
        public static Task Nav_Template_Layout_P3R_Custom_Lighting_Colors_Logic(MenuIdStructure menuSession, string color_code, string customId)
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
            if (Utility.CheckValidFormatHtmlColor(color_code) == true)
            {
                switch (customId)
                {
                    case "base_color_code":
                        account.P3R_TS_Portrait_Lighting_Custom_Base = color_code;
                        UserInfoClasses.UpdateAccount(account);

                        menuSession.Account = account;
                        _ = Template_Layout_P3R_Menu.Template_Layout_P3R_Custom_Lighting_Colors_Base_Confirm(menuSession);
                        break;

                    case "rim_color_code":
                        account.P3R_TS_Portrait_Lighting_Custom_Rim = color_code;
                        UserInfoClasses.UpdateAccount(account);

                        menuSession.Account = account;
                        _ = Template_Layout_P3R_Menu.Template_Layout_P3R_Custom_Lighting_Colors_Rim_Confirm(menuSession);
                        break;
                }
            }
            else if (Utility.CheckValidFormatHtmlColor(color_code) == false)
            {
                _ = Template_Layout_P3R_Menu.Template_Layout_P3R_Custom_Lighting_Colors_Error(menuSession);
            }

            return Task.CompletedTask;
        }
    }
}
