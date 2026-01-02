using Discord.WebSocket;
using SocialLinker.Core.CloudStorageTables;
using SocialLinker.Core.Menus.Settings.Main.SceneMaker.TemplateLayout;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SocialLinker.Core.Menus.Settings.Reactions.SceneMaker.TemplateLayout
{
    class Template_Layout_P4G_Reactions
    {
        public static Task Nav_Template_Layout_P4G_Main(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            string selected = component.Data.Values.First();

            switch (selected)
            {
                case "1":
                    _ = Template_Layout_P4G_Menu.Template_Layout_P4G_Date_Weather(menuSession);
                    break;
                case "return":
                    _ = Template_Layout_VC_Menu.Template_Layout_VC_P4_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P4G_Date_Weather(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            string selected = component.Data.Values.First();
            var account = menuSession.Account;

            switch (selected)
            {
                case "1":
                    account.P4G_TS_HUD = "Normal";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P4G_Menu.Template_Layout_P4G_Date_Weather_Confirm(menuSession);
                    break;
                case "2":
                    account.P4G_TS_HUD = "TV World";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P4G_Menu.Template_Layout_P4G_Date_Weather_Confirm(menuSession);
                    break;
                case "3":
                    account.P4G_TS_HUD = "None";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P4G_Menu.Template_Layout_P4G_Date_Weather_Confirm(menuSession);
                    break;
                case "return":
                    _ = Template_Layout_P4G_Menu.Template_Layout_P4G_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P4G_Date_Weather_Confirm(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "back-to-p4g-template-settings":
                    _ = Template_Layout_P4G_Menu.Template_Layout_P4G_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }
    }
}
