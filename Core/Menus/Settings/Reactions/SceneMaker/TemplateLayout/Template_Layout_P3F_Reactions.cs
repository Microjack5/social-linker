using Discord.WebSocket;
using SocialLinker.Core.CloudStorageTables;
using SocialLinker.Core.Menus.Settings.Main.SceneMaker.TemplateLayout;
using System.Linq;
using System.Threading.Tasks;

namespace SocialLinker.Core.Menus.Settings.Reactions.SceneMaker.TemplateLayout
{
    class Template_Layout_P3F_Reactions
    {
        public static Task Nav_Template_Layout_P3F_Main(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            string selected = component.Data.Values.First();

            switch (selected)
            {
                case "1":
                    _ = Template_Layout_P3F_Menu.Template_Layout_P3F_Date_Moon(menuSession);
                    break;
                case "2":
                    _ = Template_Layout_P3F_Menu.Template_Layout_P3F_Navigatior_Window(menuSession);
                    break;
                case "return":
                    _ = Template_Layout_VC_Menu.Template_Layout_VC_P3_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P3F_Date_Moon(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            string selected = component.Data.Values.First();
            var account = menuSession.Account;

            switch (selected)
            {
                case "1":
                    account.P3F_TS_HUD = "Display All";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P3F_Menu.Template_Layout_P3F_Date_Moon_Confirm(menuSession);
                    break;
                case "2":
                    account.P3F_TS_HUD = "Countdown Off";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P3F_Menu.Template_Layout_P3F_Date_Moon_Confirm(menuSession);
                    break;
                case "3":
                    account.P3F_TS_HUD = "Date Only";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P3F_Menu.Template_Layout_P3F_Date_Moon_Confirm(menuSession);
                    break;
                case "4":
                    account.P3F_TS_HUD = "None";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P3F_Menu.Template_Layout_P3F_Date_Moon_Confirm(menuSession);
                    break;
                case "return":
                    _ = Template_Layout_P3F_Menu.Template_Layout_P3F_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P3F_Navigatior_Window(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            var account = menuSession.Account;

            switch (component.Data.CustomId)
            {
                case "return":
                    _ = Template_Layout_P3F_Menu.Template_Layout_P3F_Main(menuSession);
                    break;

                case "on":
                    account.P3F_TS_Nav = "On";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P3F_Menu.Template_Layout_P3F_Navigatior_Window_Confirm(menuSession);
                    break;

                case "off":
                    account.P3F_TS_Nav = "Off";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P3F_Menu.Template_Layout_P3F_Navigatior_Window_Confirm(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P3F_Date_Moon_Confirm(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "back-to-p3f-template-settings":
                    _ = Template_Layout_P3F_Menu.Template_Layout_P3F_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P3F_Navigatior_Window_Confirm(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "back-to-p3f-template-settings":
                    _ = Template_Layout_P3F_Menu.Template_Layout_P3F_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }
    }
}
