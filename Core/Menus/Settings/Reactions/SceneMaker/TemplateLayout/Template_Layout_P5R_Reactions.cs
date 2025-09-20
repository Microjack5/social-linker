using Discord.WebSocket;
using SocialLinker.Core.CloudStorageTables;
using SocialLinker.Core.Menus.Settings.Main.SceneMaker.TemplateLayout;
using System.Linq;
using System.Threading.Tasks;

namespace SocialLinker.Core.Menus.Settings.Reactions.SceneMaker.TemplateLayout
{
    class Template_Layout_P5R_Reactions
    {
        public static Task Nav_Template_Layout_P5R_Main(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            string selected = component.Data.Values.First();

            switch (selected)
            {
                case "1":
                    _ = Template_Layout_P5R_Menu.Template_Layout_P5R_Date_Weather(menuSession);
                    break;
                case "2":
                    _ = Template_Layout_P5R_Menu.Template_Layout_P5R_Scene_Border(menuSession);
                    break;
                case "3":
                    _ = Template_Layout_P5R_Menu.Template_Layout_P5R_Cursor_Panel(menuSession);
                    break;
                case "4":
                    _ = Template_Layout_P5R_Menu.Template_Layout_P5R_Phone_Calls_Main(menuSession);
                    break;
                case "return":
                    _ = Template_Layout_VC_Menu.Template_Layout_VC_P5_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P5R_Date_Weather(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            string selected = component.Data.Values.First();
            var account = menuSession.Account;

            switch (selected)
            {
                case "1":
                    account.P5R_TS_HUD = "Normal";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P5R_Menu.Template_Layout_P5R_Date_Weather_Confirm(menuSession);
                    break;
                case "2":
                    account.P5R_TS_HUD = "Inverted";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P5R_Menu.Template_Layout_P5R_Date_Weather_Confirm(menuSession);
                    break;
                case "3":
                    account.P5R_TS_HUD = "None";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P5R_Menu.Template_Layout_P5R_Date_Weather_Confirm(menuSession);
                    break;
                case "return":
                    _ = Template_Layout_P5R_Menu.Template_Layout_P5R_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P5R_Scene_Border(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            string selected = component.Data.Values.First();
            var account = menuSession.Account;

            switch (selected)
            {
                case "1":
                    account.P5R_TS_Border = "Event";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P5R_Menu.Template_Layout_P5R_Scene_Border_Confirm(menuSession);
                    break;
                case "2":
                    account.P5R_TS_Border = "Interaction";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P5R_Menu.Template_Layout_P5R_Scene_Border_Confirm(menuSession);
                    break;
                case "3":
                    account.P5R_TS_Border = "None";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P5R_Menu.Template_Layout_P5R_Scene_Border_Confirm(menuSession);
                    break;
                case "return":
                    _ = Template_Layout_P5R_Menu.Template_Layout_P5R_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P5R_Cursor_Panel(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            string selected = component.Data.Values.First();
            var account = menuSession.Account;

            switch (selected)
            {
                case "1":
                    account.P5R_TS_Panel = "Manual (with Control Panel)";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P5R_Menu.Template_Layout_P5R_Cursor_Panel_Confirm(menuSession);
                    break;
                case "2":
                    account.P5R_TS_Panel = "Manual (without Control Panel)";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P5R_Menu.Template_Layout_P5R_Cursor_Panel_Confirm(menuSession);
                    break;
                case "3":
                    account.P5R_TS_Panel = "Auto-Advance";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P5R_Menu.Template_Layout_P5R_Cursor_Panel_Confirm(menuSession);
                    break;
                case "return":
                    _ = Template_Layout_P5R_Menu.Template_Layout_P5R_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P5R_Phone_Calls_Main(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            string selected = component.Data.Values.First();

            switch (selected)
            {
                case "1":
                    _ = Template_Layout_P5R_Menu.Template_Layout_P5R_Phone_Calls_Toggle(menuSession);
                    break;
                case "2":
                    _ = Template_Layout_P5R_Menu.Template_Layout_P5R_Phone_Calls_Location(menuSession);
                    break;
                case "return":
                    _ = Template_Layout_P5R_Menu.Template_Layout_P5R_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P5R_Phone_Calls_Toggle(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            var account = menuSession.Account;

            switch (component.Data.CustomId)
            {
                case "return":
                    _ = Template_Layout_P5R_Menu.Template_Layout_P5R_Phone_Calls_Main(menuSession);
                    break;

                case "on":
                    account.P5R_TS_Caller_Toggle = "On";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P5R_Menu.Template_Layout_P5R_Phone_Calls_Toggle_Confirm(menuSession);
                    break;

                case "off":
                    account.P5R_TS_Caller_Toggle = "Off";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P5R_Menu.Template_Layout_P5R_Phone_Calls_Toggle_Confirm(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P5R_Phone_Calls_Location(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            string selected = component.Data.Values.First();
            var account = menuSession.Account;

            switch (selected)
            {
                case "return":
                    _ = Template_Layout_P5R_Menu.Template_Layout_P5R_Phone_Calls_Main(menuSession);
                    break;

                case "1":
                    account.P5R_TS_Caller_Location = "Dynamic";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P5R_Menu.Template_Layout_P5R_Phone_Calls_Location_Confirm(menuSession);
                    break;

                case "2":
                    account.P5R_TS_Caller_Location = "Dynamic (Normals Only)";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P5R_Menu.Template_Layout_P5R_Phone_Calls_Location_Confirm(menuSession);
                    break;

                case "3":
                    account.P5R_TS_Caller_Location = "Velvet Room";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P5R_Menu.Template_Layout_P5R_Phone_Calls_Location_Confirm(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P5R_Date_Weather_Confirm(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "back-to-p5r-template-settings":
                    _ = Template_Layout_P5R_Menu.Template_Layout_P5R_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P5R_Scene_Border_Confirm(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "back-to-p5r-template-settings":
                    _ = Template_Layout_P5R_Menu.Template_Layout_P5R_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P5R_Cursor_Panel_Confirm(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "back-to-p5r-template-settings":
                    _ = Template_Layout_P5R_Menu.Template_Layout_P5R_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P5R_Phone_Calls_Toggle_Confirm(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "back-to-p5r-template-settings":
                    _ = Template_Layout_P5R_Menu.Template_Layout_P5R_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P5R_Phone_Calls_Location_Confirm(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "back-to-p5r-template-settings":
                    _ = Template_Layout_P5R_Menu.Template_Layout_P5R_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }
    }
}
