using Discord.WebSocket;
using SocialLinker.Core.CloudStorageTables;
using SocialLinker.Core.Menus.Settings.Main.SceneMaker.TemplateLayout;
using System.Linq;
using System.Threading.Tasks;

namespace SocialLinker.Core.Menus.Settings.Reactions.SceneMaker.TemplateLayout
{
    class Template_Layout_P1_PS1_Reactions
    {
        public static Task Nav_Template_Layout_P1_PS1_Main(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            string selected = component.Data.Values.First();

            switch (selected)
            {
                case "1":
                    _ = Template_Layout_P1_PS1_Menu.Template_Layout_P1_PS1_Wallpaper(menuSession);
                    break;
                case "2":
                    _ = Template_Layout_P1_PS1_Menu.Template_Layout_P1_PS1_Moon_Phases(menuSession);
                    break;
                case "3":
                    _ = Template_Layout_P1_PS1_Menu.Template_Layout_P1_PS1_Placement(menuSession);
                    break;
                case "4":
                    _ = Template_Layout_P1_PS1_Menu.Template_Layout_P1_PS1_BG_Darken(menuSession);
                    break;
                case "5":
                    _ = Template_Layout_P1_PS1_Menu.Template_Layout_P1_PS1_Consistent_Names(menuSession);
                    break;
                case "6":
                    _ = Template_Layout_P1_PS1_Menu.Template_Layout_P1_PS1_Localized_Names(menuSession);
                    break;
                case "return":
                    _ = Template_Layout_VC_Menu.Template_Layout_VC_P1_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P1_PS1_Wallpaper(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            string selected = component.Data.Values.First();
            var account = menuSession.Account;

            switch (selected)
            {
                case "1":
                    account.P1_PSX_TS_Wallpaper = "Type 1";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P1_PS1_Menu.Template_Layout_P1_PS1_Wallpaper_Confirm(menuSession);
                    break;
                case "2":
                    account.P1_PSX_TS_Wallpaper = "Type 2";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P1_PS1_Menu.Template_Layout_P1_PS1_Wallpaper_Confirm(menuSession);
                    break;
                case "3":
                    account.P1_PSX_TS_Wallpaper = "Type 3";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P1_PS1_Menu.Template_Layout_P1_PS1_Wallpaper_Confirm(menuSession);
                    break;
                case "4":
                    account.P1_PSX_TS_Wallpaper = "Type 4";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P1_PS1_Menu.Template_Layout_P1_PS1_Wallpaper_Confirm(menuSession);
                    break;
                case "5":
                    account.P1_PSX_TS_Wallpaper = "Type 5";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P1_PS1_Menu.Template_Layout_P1_PS1_Wallpaper_Confirm(menuSession);
                    break;
                case "6":
                    account.P1_PSX_TS_Wallpaper = "Type 6";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P1_PS1_Menu.Template_Layout_P1_PS1_Wallpaper_Confirm(menuSession);
                    break;
                case "7":
                    account.P1_PSX_TS_Wallpaper = "Type 7";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P1_PS1_Menu.Template_Layout_P1_PS1_Wallpaper_Confirm(menuSession);
                    break;
                case "8":
                    account.P1_PSX_TS_Wallpaper = "Type 8";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P1_PS1_Menu.Template_Layout_P1_PS1_Wallpaper_Confirm(menuSession);
                    break;
                case "return":
                    _ = Template_Layout_P1_PS1_Menu.Template_Layout_P1_PS1_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P1_PS1_Moon_Phases(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            var account = menuSession.Account;

            switch (component.Data.CustomId)
            {
                case "return":
                    _ = Template_Layout_P1_PS1_Menu.Template_Layout_P1_PS1_Main(menuSession);
                    break;

                case "on":
                    account.P1_PSX_TS_Moon_HUD = "On";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P1_PS1_Menu.Template_Layout_P1_PS1_Moon_Phases_Confirm(menuSession);
                    break;

                case "off":
                    account.P1_PSX_TS_Moon_HUD = "Off";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P1_PS1_Menu.Template_Layout_P1_PS1_Moon_Phases_Confirm(menuSession);
                    break;
            }

            return Task.CompletedTask;

        }

        public static Task Nav_Template_Layout_P1_PS1_Placement(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            string selected = component.Data.Values.First();
            var account = menuSession.Account;

            switch (selected)
            {
                case "1":
                    account.P1_PSX_TS_Position = "Left";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P1_PS1_Menu.Template_Layout_P1_PS1_Placement_Confirm(menuSession);
                    break;
                case "2":
                    account.P1_PSX_TS_Position = "Center";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P1_PS1_Menu.Template_Layout_P1_PS1_Placement_Confirm(menuSession);
                    break;
                case "3":
                    account.P1_PSX_TS_Position = "Right";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P1_PS1_Menu.Template_Layout_P1_PS1_Placement_Confirm(menuSession);
                    break;
                case "4":
                    account.P1_PSX_TS_Position = "Switch";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P1_PS1_Menu.Template_Layout_P1_PS1_Placement_Confirm(menuSession);
                    break;
                case "return":
                    _ = Template_Layout_P1_PS1_Menu.Template_Layout_P1_PS1_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P1_PS1_BG_Darken(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            var account = menuSession.Account;

            switch (component.Data.CustomId)
            {
                case "return":
                    _ = Template_Layout_P1_PS1_Menu.Template_Layout_P1_PS1_Main(menuSession);
                    break;

                case "on":
                    account.P1_PSX_TS_BG_Darken = "On";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P1_PS1_Menu.Template_Layout_P1_PS1_BG_Darken_Confirm(menuSession);
                    break;

                case "off":
                    account.P1_PSX_TS_BG_Darken = "Off";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P1_PS1_Menu.Template_Layout_P1_PS1_BG_Darken_Confirm(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P1_PS1_Consistent_Names(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            var account = menuSession.Account;

            switch (component.Data.CustomId)
            {
                case "return":
                    _ = Template_Layout_P1_PS1_Menu.Template_Layout_P1_PS1_Main(menuSession);
                    break;

                case "on":
                    account.P1_PSX_TS_Consistent_Names = "On";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P1_PS1_Menu.Template_Layout_P1_PS1_Consistent_Names_Confirm(menuSession);
                    break;

                case "off":
                    account.P1_PSX_TS_Consistent_Names = "Off";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P1_PS1_Menu.Template_Layout_P1_PS1_Consistent_Names_Confirm(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P1_PS1_Localized_Names(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            var account = menuSession.Account;

            switch (component.Data.CustomId)
            {
                case "return":
                    _ = Template_Layout_P1_PS1_Menu.Template_Layout_P1_PS1_Main(menuSession);
                    break;

                case "on":
                    account.P1_PSX_TS_Localized_Revelations_Names = "On";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P1_PS1_Menu.Template_Layout_P1_PS1_Localized_Names_Confirm(menuSession);
                    break;

                case "off":
                    account.P1_PSX_TS_Localized_Revelations_Names = "Off";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P1_PS1_Menu.Template_Layout_P1_PS1_Localized_Names_Confirm(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P1_PS1_Wallpaper_Confirm(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "back-to-p1-ps1-template-settings":
                    _ = Template_Layout_P1_PS1_Menu.Template_Layout_P1_PS1_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P1_PS1_Moon_Phases_Confirm(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "back-to-p1-ps1-template-settings":
                    _ = Template_Layout_P1_PS1_Menu.Template_Layout_P1_PS1_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P1_PS1_Placement_Confirm(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "back-to-p1-ps1-template-settings":
                    _ = Template_Layout_P1_PS1_Menu.Template_Layout_P1_PS1_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P1_PS1_BG_Darken_Confirm(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "back-to-p1-ps1-template-settings":
                    _ = Template_Layout_P1_PS1_Menu.Template_Layout_P1_PS1_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P1_PS1_Consistent_Names_Confirm(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "back-to-p1-ps1-template-settings":
                    _ = Template_Layout_P1_PS1_Menu.Template_Layout_P1_PS1_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P1_PS1_Localized_Names_Confirm(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "back-to-p1-ps1-template-settings":
                    _ = Template_Layout_P1_PS1_Menu.Template_Layout_P1_PS1_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }
    }
}
