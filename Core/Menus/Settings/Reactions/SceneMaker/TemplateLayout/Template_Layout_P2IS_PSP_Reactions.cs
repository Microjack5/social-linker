using Discord.WebSocket;
using SocialLinker.Core.CloudStorageTables;
using SocialLinker.Core.Menus.Settings.Main.SceneMaker.TemplateLayout;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SocialLinker.Core.Menus.Settings.Reactions.SceneMaker.TemplateLayout
{
    class Template_Layout_P2IS_PSP_Reactions
    {
        public static Task Nav_Template_Layout_P2IS_PSP_Main(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            string selected = component.Data.Values.First();

            switch (selected)
            {
                case "1":
                    _ = Template_Layout_P2IS_PSP_Menu.Template_Layout_P2IS_PSP_Inverted_Filter(menuSession);
                    break;
                case "2":
                    _ = Template_Layout_P2IS_PSP_Menu.Template_Layout_P2IS_PSP_Placement(menuSession);
                    break;
                case "3":
                    _ = Template_Layout_P2IS_PSP_Menu.Template_Layout_P2IS_PSP_Sprite_Flip(menuSession);
                    break;
                case "return":
                    _ = Template_Layout_VC_Menu.Template_Layout_VC_P2IS_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P2IS_PSP_Inverted_Filter(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            var account = menuSession.Account;

            switch (component.Data.CustomId)
            {
                case "return":
                    _ = Template_Layout_P2IS_PSP_Menu.Template_Layout_P2IS_PSP_Main(menuSession);
                    break;

                case "on":
                    account.P2IS_PSP_TS_Invert = "On";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P2IS_PSP_Menu.Template_Layout_P2IS_PSP_Inverted_Filter_Confirm(menuSession);
                    break;

                case "off":
                    account.P2IS_PSP_TS_Invert = "Off";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P2IS_PSP_Menu.Template_Layout_P2IS_PSP_Inverted_Filter_Confirm(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P2IS_PSP_Placement(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            string selected = component.Data.Values.First();
            var account = menuSession.Account;

            switch (selected)
            {
                case "1":
                    account.P2IS_PSP_TS_Position = "Default";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P2IS_PSP_Menu.Template_Layout_P2IS_PSP_Placement_Confirm(menuSession);
                    break;
                case "2":
                    account.P2IS_PSP_TS_Position = "Left";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P2IS_PSP_Menu.Template_Layout_P2IS_PSP_Placement_Confirm(menuSession);
                    break;
                case "3":
                    account.P2IS_PSP_TS_Position = "Right";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P2IS_PSP_Menu.Template_Layout_P2IS_PSP_Placement_Confirm(menuSession);
                    break;
                case "return":
                    _ = Template_Layout_P2IS_PSP_Menu.Template_Layout_P2IS_PSP_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P2IS_PSP_Sprite_Flip(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            var account = menuSession.Account;

            switch (component.Data.CustomId)
            {
                case "return":
                    _ = Template_Layout_P2IS_PSP_Menu.Template_Layout_P2IS_PSP_Main(menuSession);
                    break;

                case "on":
                    account.P2IS_PSP_TS_Sprite_Flip = "On";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P2IS_PSP_Menu.Template_Layout_P2IS_PSP_Sprite_Flip_Confirm(menuSession);
                    break;

                case "off":
                    account.P2IS_PSP_TS_Sprite_Flip = "Off";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P2IS_PSP_Menu.Template_Layout_P2IS_PSP_Sprite_Flip_Confirm(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P2IS_PSP_Inverted_Filter_Confirm(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "back-to-p2is-psp-template-settings":
                    _ = Template_Layout_P2IS_PSP_Menu.Template_Layout_P2IS_PSP_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P2IS_PSP_Placement_Confirm(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "back-to-p2is-psp-template-settings":
                    _ = Template_Layout_P2IS_PSP_Menu.Template_Layout_P2IS_PSP_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P2IS_PSP_Sprite_Flip_Confirm(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "back-to-p2is-psp-template-settings":
                    _ = Template_Layout_P2IS_PSP_Menu.Template_Layout_P2IS_PSP_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }
    }
}
