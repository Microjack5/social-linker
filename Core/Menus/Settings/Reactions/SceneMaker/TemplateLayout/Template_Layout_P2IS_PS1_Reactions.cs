using Discord.WebSocket;
using SocialLinker.Core.CloudStorageTables;
using SocialLinker.Core.Menus.Settings.Main.SceneMaker.TemplateLayout;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace SocialLinker.Core.Menus.Settings.Reactions.SceneMaker.TemplateLayout
{
    class Template_Layout_P2IS_PS1_Reactions
    {
        public static Task Nav_Template_Layout_P2IS_PS1_Main(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            string selected = component.Data.Values.First();

            switch (selected)
            {
                case "1":
                    _ = Template_Layout_P2IS_PS1_Menu.Template_Layout_P2IS_PS1_Wallpaper(menuSession);
                    break;
                case "2":
                    _ = Template_Layout_P2IS_PS1_Menu.Template_Layout_P2IS_PS1_Inverted_Filter(menuSession);
                    break;
                case "3":
                    _ = Template_Layout_P2IS_PS1_Menu.Template_Layout_P2IS_PS1_Placement(menuSession);
                    break;
                case "4":
                    _ = Template_Layout_P2IS_PS1_Menu.Template_Layout_P2IS_PS1_Sprite_Flip(menuSession);
                    break;
                case "5":
                    _ = Template_Layout_P2IS_PS1_Menu.Template_Layout_P2IS_PS1_Localized_Names(menuSession);
                    break;
                case "return":
                    _ = Template_Layout_VC_Menu.Template_Layout_VC_P2IS_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P2IS_PS1_Wallpaper(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            string selected = component.Data.Values.First();
            var account = menuSession.Account;

            switch (selected)
            {
                case "1":
                    account.P2IS_PSX_TS_Wallpaper = "Blue Tone";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P2IS_PS1_Menu.Template_Layout_P2IS_PS1_Wallpaper_Confirm(menuSession);
                    break;
                case "2":
                    account.P2IS_PSX_TS_Wallpaper = "Sepia Tone";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P2IS_PS1_Menu.Template_Layout_P2IS_PS1_Wallpaper_Confirm(menuSession);
                    break;
                case "3":
                    account.P2IS_PSX_TS_Wallpaper = "Purple Tone";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P2IS_PS1_Menu.Template_Layout_P2IS_PS1_Wallpaper_Confirm(menuSession);
                    break;
                case "4":
                    account.P2IS_PSX_TS_Wallpaper = "Jack Frost";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P2IS_PS1_Menu.Template_Layout_P2IS_PS1_Wallpaper_Confirm(menuSession);
                    break;
                case "5":
                    account.P2IS_PSX_TS_Wallpaper = "Star";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P2IS_PS1_Menu.Template_Layout_P2IS_PS1_Wallpaper_Confirm(menuSession);
                    break;
                case "6":
                    account.P2IS_PSX_TS_Wallpaper = "Punched Metal";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P2IS_PS1_Menu.Template_Layout_P2IS_PS1_Wallpaper_Confirm(menuSession);
                    break;
                case "7":
                    account.P2IS_PSX_TS_Wallpaper = "Seventh";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P2IS_PS1_Menu.Template_Layout_P2IS_PS1_Wallpaper_Confirm(menuSession);
                    break;
                case "8":
                    account.P2IS_PSX_TS_Wallpaper = "Cuss High";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P2IS_PS1_Menu.Template_Layout_P2IS_PS1_Wallpaper_Confirm(menuSession);
                    break;
                case "9":
                    account.P2IS_PSX_TS_Wallpaper = "Butterfly";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P2IS_PS1_Menu.Template_Layout_P2IS_PS1_Wallpaper_Confirm(menuSession);
                    break;
                case "10":
                    account.P2IS_PSX_TS_Wallpaper = "Grid";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P2IS_PS1_Menu.Template_Layout_P2IS_PS1_Wallpaper_Confirm(menuSession);
                    break;
                case "return":
                    _ = Template_Layout_P2IS_PS1_Menu.Template_Layout_P2IS_PS1_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P2IS_PS1_Inverted_Filter(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            var account = menuSession.Account;

            switch (component.Data.CustomId)
            {
                case "return":
                    _ = Template_Layout_P2IS_PS1_Menu.Template_Layout_P2IS_PS1_Main(menuSession);
                    break;

                case "on":
                    account.P2IS_PSX_TS_Invert = "On";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P2IS_PS1_Menu.Template_Layout_P2IS_PS1_Inverted_Filter_Confirm(menuSession);
                    break;

                case "off":
                    account.P2IS_PSX_TS_Invert = "Off";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P2IS_PS1_Menu.Template_Layout_P2IS_PS1_Inverted_Filter_Confirm(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P2IS_PS1_Placement(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            string selected = component.Data.Values.First();
            var account = menuSession.Account;

            switch (selected)
            {
                case "1":
                    account.P2IS_PSX_TS_Position = "Default";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P2IS_PS1_Menu.Template_Layout_P2IS_PS1_Placement_Confirm(menuSession);
                    break;
                case "2":
                    account.P2IS_PSX_TS_Position = "Left";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P2IS_PS1_Menu.Template_Layout_P2IS_PS1_Placement_Confirm(menuSession);
                    break;
                case "3":
                    account.P2IS_PSX_TS_Position = "Right";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P2IS_PS1_Menu.Template_Layout_P2IS_PS1_Placement_Confirm(menuSession);
                    break;
                case "return":
                    _ = Template_Layout_P2IS_PS1_Menu.Template_Layout_P2IS_PS1_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P2IS_PS1_Sprite_Flip(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            var account = menuSession.Account;

            switch (component.Data.CustomId)
            {
                case "return":
                    _ = Template_Layout_P2IS_PS1_Menu.Template_Layout_P2IS_PS1_Main(menuSession);
                    break;

                case "on":
                    account.P2IS_PSX_TS_Sprite_Flip = "On";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P2IS_PS1_Menu.Template_Layout_P2IS_PS1_Sprite_Flip_Confirm(menuSession);
                    break;

                case "off":
                    account.P2IS_PSX_TS_Sprite_Flip = "Off";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P2IS_PS1_Menu.Template_Layout_P2IS_PS1_Sprite_Flip_Confirm(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P2IS_PS1_Localized_Names(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            var account = menuSession.Account;

            switch (component.Data.CustomId)
            {
                case "return":
                    _ = Template_Layout_P2IS_PS1_Menu.Template_Layout_P2IS_PS1_Main(menuSession);
                    break;

                case "on":
                    account.P2IS_PSX_TS_Localized_Revelations_Names = "On";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P2IS_PS1_Menu.Template_Layout_P2IS_PS1_Localized_Names_Confirm(menuSession);
                    break;

                case "off":
                    account.P2IS_PSX_TS_Localized_Revelations_Names = "Off";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P2IS_PS1_Menu.Template_Layout_P2IS_PS1_Localized_Names_Confirm(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P2IS_PS1_Wallpaper_Confirm(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "back-to-p2is-ps1-template-settings":
                    _ = Template_Layout_P2IS_PS1_Menu.Template_Layout_P2IS_PS1_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P2IS_PS1_Inverted_Filter_Confirm(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "back-to-p2is-ps1-template-settings":
                    _ = Template_Layout_P2IS_PS1_Menu.Template_Layout_P2IS_PS1_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P2IS_PS1_Placement_Confirm(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "back-to-p2is-ps1-template-settings":
                    _ = Template_Layout_P2IS_PS1_Menu.Template_Layout_P2IS_PS1_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P2IS_PS1_Sprite_Flip_Confirm(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "back-to-p2is-ps1-template-settings":
                    _ = Template_Layout_P2IS_PS1_Menu.Template_Layout_P2IS_PS1_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P2IS_PS1_Localized_Names_Confirm(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "back-to-p2is-ps1-template-settings":
                    _ = Template_Layout_P2IS_PS1_Menu.Template_Layout_P2IS_PS1_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }
    }
}
