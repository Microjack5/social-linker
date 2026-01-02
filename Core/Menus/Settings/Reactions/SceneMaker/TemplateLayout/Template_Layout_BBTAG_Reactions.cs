using Discord.WebSocket;
using SocialLinker.Core.CloudStorageTables;
using SocialLinker.Core.Menus.Settings.Main.SceneMaker;
using SocialLinker.Core.Menus.Settings.Main.SceneMaker.TemplateLayout;
using System.Linq;
using System.Threading.Tasks;

namespace SocialLinker.Core.Menus.Settings.Reactions.SceneMaker.TemplateLayout
{
    class Template_Layout_BBTAG_Reactions
    {
        public static Task Nav_Template_Layout_BBTAG_Main(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            string selected = component.Data.Values.First();

            switch (selected)
            {
                case "1":
                    _ = Template_Layout_BBTAG_Menu.Template_Layout_BBTAG_Header(menuSession);
                    break;
                case "2":
                    _ = Template_Layout_BBTAG_Menu.Template_Layout_BBTAG_Sprite_Placement(menuSession);
                    break;
                case "3":
                    _ = Template_Layout_BBTAG_Menu.Template_Layout_BBTAG_Background_Blur(menuSession);
                    break;
                case "return":
                    _ = Template_Layout_Menu.Template_Layout_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_BBTAG_Header(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            string selected = component.Data.Values.First();
            var account = menuSession.Account;

            switch (selected)
            {
                case "1":
                    account.BBTAG_TS_Header = "Prologue";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_BBTAG_Menu.Template_Layout_BBTAG_Header_Confirm(menuSession);
                    break;
                case "2":
                    account.BBTAG_TS_Header = "Episode BlazBlue";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_BBTAG_Menu.Template_Layout_BBTAG_Header_Confirm(menuSession);
                    break;
                case "3":
                    account.BBTAG_TS_Header = "Episode P4A";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_BBTAG_Menu.Template_Layout_BBTAG_Header_Confirm(menuSession);
                    break;
                case "4":
                    account.BBTAG_TS_Header = "Episode Under Night In-Birth";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_BBTAG_Menu.Template_Layout_BBTAG_Header_Confirm(menuSession);
                    break;
                case "5":
                    account.BBTAG_TS_Header = "Episode RWBY";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_BBTAG_Menu.Template_Layout_BBTAG_Header_Confirm(menuSession);
                    break;
                case "6":
                    account.BBTAG_TS_Header = "Episode Extra";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_BBTAG_Menu.Template_Layout_BBTAG_Header_Confirm(menuSession);
                    break;
                case "return":
                    _ = Template_Layout_BBTAG_Menu.Template_Layout_BBTAG_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_BBTAG_Sprite_Placement(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            string selected = component.Data.Values.First();
            var account = menuSession.Account;

            switch (selected)
            {
                case "1":
                    account.BBTAG_TS_Position = "Left";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_BBTAG_Menu.Template_Layout_BBTAG_Sprite_Placement_Confirm(menuSession);
                    break;
                case "2":
                    account.BBTAG_TS_Position = "Center";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_BBTAG_Menu.Template_Layout_BBTAG_Sprite_Placement_Confirm(menuSession);
                    break;
                case "3":
                    account.BBTAG_TS_Position = "Right";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_BBTAG_Menu.Template_Layout_BBTAG_Sprite_Placement_Confirm(menuSession);
                    break;
                case "return":
                    _ = Template_Layout_BBTAG_Menu.Template_Layout_BBTAG_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_BBTAG_Background_Blur(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            var account = menuSession.Account;

            switch (component.Data.CustomId)
            {
                case "return":
                    _ = Template_Layout_BBTAG_Menu.Template_Layout_BBTAG_Main(menuSession);
                    break;

                case "on":
                    account.BBTAG_TS_BG_Blur = "On";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_BBTAG_Menu.Template_Layout_BBTAG_Background_Blur_Confirm(menuSession);
                    break;

                case "off":
                    account.BBTAG_TS_BG_Blur = "Off";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_BBTAG_Menu.Template_Layout_BBTAG_Background_Blur_Confirm(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_BBTAG_Header_Confirm(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "back-to-bbtag-template-settings":
                    _ = Template_Layout_BBTAG_Menu.Template_Layout_BBTAG_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_BBTAG_Sprite_Placement_Confirm(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "back-to-bbtag-template-settings":
                    _ = Template_Layout_BBTAG_Menu.Template_Layout_BBTAG_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_BBTAG_Background_Blur_Confirm(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "back-to-bbtag-template-settings":
                    _ = Template_Layout_BBTAG_Menu.Template_Layout_BBTAG_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }
    }
}
