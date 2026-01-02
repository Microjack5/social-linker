using Discord.WebSocket;
using SocialLinker.Core.CloudStorageTables;
using SocialLinker.Core.Menus.Settings.Main.SceneMaker.TemplateLayout;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SocialLinker.Core.Menus.Settings.Reactions.SceneMaker.TemplateLayout
{
    class Template_Layout_P3P_Reactions
    {
        public static Task Nav_Template_Layout_P3P_Main(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            string selected = component.Data.Values.First();

            switch (selected)
            {
                case "1":
                    _ = Template_Layout_P3P_Menu.Template_Layout_P3P_Color_Scheme(menuSession);
                    break;
                case "2":
                    _ = Template_Layout_P3P_Menu.Template_Layout_P3P_Date_Moon(menuSession);
                    break;
                case "3":
                    _ = Template_Layout_P3P_Menu.Template_Layout_P3P_Sprite_Placement(menuSession);
                    break;
                case "return":
                    _ = Template_Layout_VC_Menu.Template_Layout_VC_P3_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P3P_Color_Scheme(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            string selected = component.Data.Values.First();
            var account = menuSession.Account;

            switch (selected)
            {
                case "1":
                    account.P3P_TS_Color = "Male Protagonist";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P3P_Menu.Template_Layout_P3P_Color_Scheme_Confirm(menuSession);
                    break;
                case "2":
                    account.P3P_TS_Color = "Female Protagonist";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P3P_Menu.Template_Layout_P3P_Color_Scheme_Confirm(menuSession);
                    break;
                case "return":
                    _ = Template_Layout_P3P_Menu.Template_Layout_P3P_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P3P_Date_Moon(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            string selected = component.Data.Values.First();
            var account = menuSession.Account;

            switch (selected)
            {
                case "1":

                    account.P3P_TS_HUD = "Display All";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P3P_Menu.Template_Layout_P3P_Date_Moon_Confirm(menuSession);
                    break;
                case "2":
                    account.P3P_TS_HUD = "Countdown Off";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P3P_Menu.Template_Layout_P3P_Date_Moon_Confirm(menuSession);
                    break;
                case "3":
                    account.P3P_TS_HUD = "None";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P3P_Menu.Template_Layout_P3P_Date_Moon_Confirm(menuSession);
                    break;
                case "return":
                    _ = Template_Layout_P3P_Menu.Template_Layout_P3P_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P3P_Sprite_Placement(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            string selected = component.Data.Values.First();
            var account = menuSession.Account;

            switch (selected)
            {
                case "1":
                    account.P3P_TS_Position = "Left";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P3P_Menu.Template_Layout_P3P_Sprite_Placement_Confirm(menuSession);
                    break;
                case "2":
                    account.P3P_TS_Position = "Center";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P3P_Menu.Template_Layout_P3P_Sprite_Placement_Confirm(menuSession);
                    break;
                case "3":
                    account.P3P_TS_Position = "Right";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P3P_Menu.Template_Layout_P3P_Sprite_Placement_Confirm(menuSession);
                    break;
                case "return":
                    _ = Template_Layout_P3P_Menu.Template_Layout_P3P_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P3P_Color_Scheme_Confirm(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "back-to-p3p-template-settings":
                    _ = Template_Layout_P3P_Menu.Template_Layout_P3P_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P3P_Date_Moon_Confirm(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "back-to-p3p-template-settings":
                    _ = Template_Layout_P3P_Menu.Template_Layout_P3P_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P3P_Sprite_Placement_Confirm(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "back-to-p3p-template-settings":
                    _ = Template_Layout_P3P_Menu.Template_Layout_P3P_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }
    }
}
