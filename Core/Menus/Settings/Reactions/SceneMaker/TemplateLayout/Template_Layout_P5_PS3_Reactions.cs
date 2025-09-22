using Discord.WebSocket;
using SocialLinker.Core.CloudStorageTables;
using SocialLinker.Core.Menus.Settings.Main.SceneMaker.TemplateLayout;
using System.Linq;
using System.Threading.Tasks;

namespace SocialLinker.Core.Menus.Settings.Reactions.SceneMaker.TemplateLayout
{
    class Template_Layout_P5_PS3_Reactions
    {
        public static Task Nav_Template_Layout_P5_PS3_Main(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            string selected = component.Data.Values.First();

            switch (selected)
            {
                case "1":
                    _ = Template_Layout_P5_PS3_Menu.Template_Layout_P5_PS3_Date_Weather(menuSession);
                    break;
                case "2":
                    _ = Template_Layout_P5_PS3_Menu.Template_Layout_P5_PS3_Scene_Border(menuSession);
                    break;
                case "3":
                    _ = Template_Layout_P5_PS3_Menu.Template_Layout_P5_PS3_Cursor_Panel(menuSession);
                    break;
                case "return":
                    _ = Template_Layout_VC_Menu.Template_Layout_VC_P5_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P5_PS3_Date_Weather(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            var account = menuSession.Account;

            switch (component.Data.CustomId)
            {
                case "return":
                    _ = Template_Layout_P5_PS3_Menu.Template_Layout_P5_PS3_Main(menuSession);
                    break;

                case "on":
                    account.P5_PS4_TS_HUD = "On";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P5_PS3_Menu.Template_Layout_P5_PS3_Date_Weather_Confirm(menuSession);
                    break;

                case "off":
                    account.P5_PS4_TS_HUD = "Off";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P5_PS3_Menu.Template_Layout_P5_PS3_Date_Weather_Confirm(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P5_PS3_Scene_Border(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            string selected = component.Data.Values.First();
            var account = menuSession.Account;

            switch (selected)
            {
                case "1":
                    account.P5_PS4_TS_Border = "Event";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P5_PS3_Menu.Template_Layout_P5_PS3_Scene_Border_Confirm(menuSession);
                    break;
                case "2":
                    account.P5_PS4_TS_Border = "Interaction";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P5_PS3_Menu.Template_Layout_P5_PS3_Scene_Border_Confirm(menuSession);
                    break;
                case "3":
                    account.P5_PS4_TS_Border = "None";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P5_PS3_Menu.Template_Layout_P5_PS3_Scene_Border_Confirm(menuSession);
                    break;
                case "return":
                    _ = Template_Layout_P5_PS3_Menu.Template_Layout_P5_PS3_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P5_PS3_Cursor_Panel(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            string selected = component.Data.Values.First();
            var account = menuSession.Account;

            switch (selected)
            {
                case "1":
                    account.P5_PS4_TS_Panel = "Manual (with Control Panel)";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P5_PS3_Menu.Template_Layout_P5_PS3_Cursor_Panel_Confirm(menuSession);
                    break;
                case "2":
                    account.P5_PS4_TS_Panel = "Manual (without Control Panel)";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P5_PS3_Menu.Template_Layout_P5_PS3_Cursor_Panel_Confirm(menuSession);
                    break;
                case "3":
                    account.P5_PS4_TS_Panel = "Auto-Advance";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P5_PS3_Menu.Template_Layout_P5_PS3_Cursor_Panel_Confirm(menuSession);
                    break;
                case "return":
                    _ = Template_Layout_P5_PS3_Menu.Template_Layout_P5_PS3_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P5_PS3_Date_Weather_Confirm(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "back-to-p5-template-settings":
                    _ = Template_Layout_P5_PS3_Menu.Template_Layout_P5_PS3_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P5_PS3_Scene_Border_Confirm(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "back-to-p5-template-settings":
                    _ = Template_Layout_P5_PS3_Menu.Template_Layout_P5_PS3_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P5_PS3_Cursor_Panel_Confirm(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "back-to-p5-template-settings":
                    _ = Template_Layout_P5_PS3_Menu.Template_Layout_P5_PS3_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }
    }
}
