using System.Linq;
using System.Threading.Tasks;
using Discord.WebSocket;
using SocialLinker.Core.CloudStorageTables;
using SocialLinker.Core.Menus.Settings.Main.SceneMaker;
using SocialLinker.Core.Menus.Settings.Main.SceneMaker.TemplateLayout;

namespace SocialLinker.Core.Menus.Settings.Reactions.SceneMaker.TemplateLayout
{
    class Template_Layout_P4AU_Reactions
    {
        public static Task Nav_Template_Layout_P4AU_Main(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            string selected = component.Data.Values.First();

            switch (selected)
            {
                case "1":
                    _ = Template_Layout_P4AU_Menu.Template_Layout_P4AU_Scene_Type(menuSession);
                    break;
                case "2":
                    _ = Template_Layout_P4AU_Menu.Template_Layout_P4AU_Auto_Advance(menuSession);
                    break;
                case "3":
                    _ = Template_Layout_P4AU_Menu.Template_Layout_P4AU_Control_Panel(menuSession);
                    break;
                case "4":
                    _ = Template_Layout_P4AU_Menu.Template_Layout_P4AU_Sprite_Placement(menuSession);
                    break;
                case "5":
                    _ = Template_Layout_P4AU_Menu.Template_Layout_P4AU_Highlight(menuSession);
                    break;
                case "return":
                    _ = Template_Layout_Menu.Template_Layout_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P4AU_Scene_Type(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            string selected = component.Data.Values.First();
            var account = menuSession.Account;

            switch (selected)
            {
                case "1":
                    account.P4AU_TS_Scene_Type = "Dialogue";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P4AU_Menu.Template_Layout_P4AU_Scene_Type_Confirm(menuSession);
                    break;
                case "2":
                    account.P4AU_TS_Scene_Type = "Narration";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P4AU_Menu.Template_Layout_P4AU_Scene_Type_Confirm(menuSession);
                    break;
                case "return":
                    _ = Template_Layout_P4AU_Menu.Template_Layout_P4AU_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P4AU_Auto_Advance(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            var account = menuSession.Account;

            switch (component.Data.CustomId)
            {
                case "return":
                    _ = Template_Layout_P4AU_Menu.Template_Layout_P4AU_Main(menuSession);
                    break;

                case "on":
                    account.P4AU_TS_Auto_Advance = "On";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P4AU_Menu.Template_Layout_P4AU_Auto_Advance_Confirm(menuSession);
                    break;

                case "off":
                    account.P4AU_TS_Auto_Advance = "Off";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P4AU_Menu.Template_Layout_P4AU_Auto_Advance_Confirm(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P4AU_Control_Panel(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            string selected = component.Data.Values.First();
            var account = menuSession.Account;

            switch (selected)
            {
                case "1":
                    account.P4AU_TS_Panel = "PlayStation®️ 3";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P4AU_Menu.Template_Layout_P4AU_Control_Panel_Confirm(menuSession);
                    break;
                case "2":
                    account.P4AU_TS_Panel = "PlayStation®️ 4";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P4AU_Menu.Template_Layout_P4AU_Control_Panel_Confirm(menuSession);
                    break;
                case "3":
                    account.P4AU_TS_Panel = "PlayStation®️ 4 (PC Layout)";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P4AU_Menu.Template_Layout_P4AU_Control_Panel_Confirm(menuSession);
                    break;
                case "4":
                    account.P4AU_TS_Panel = "Xbox 360";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P4AU_Menu.Template_Layout_P4AU_Control_Panel_Confirm(menuSession);
                    break;
                case "5":
                    account.P4AU_TS_Panel = "Xbox One (PC Layout)";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P4AU_Menu.Template_Layout_P4AU_Control_Panel_Confirm(menuSession);
                    break;
                case "6":
                    account.P4AU_TS_Panel = "Nintendo Switch";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P4AU_Menu.Template_Layout_P4AU_Control_Panel_Confirm(menuSession);
                    break;
                case "7":
                    account.P4AU_TS_Panel = "Nintendo Switch (PC Layout)";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P4AU_Menu.Template_Layout_P4AU_Control_Panel_Confirm(menuSession);
                    break;
                case "8":
                    account.P4AU_TS_Panel = "Keyboard";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P4AU_Menu.Template_Layout_P4AU_Control_Panel_Confirm(menuSession);
                    break;
                case "9":
                    account.P4AU_TS_Panel = "None";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P4AU_Menu.Template_Layout_P4AU_Control_Panel_Confirm(menuSession);
                    break;
                case "return":
                    _ = Template_Layout_P4AU_Menu.Template_Layout_P4AU_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P4AU_Sprite_Placement(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            string selected = component.Data.Values.First();
            var account = menuSession.Account;

            switch (selected)
            {
                case "1":
                    account.P4AU_TS_Position = "Left";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P4AU_Menu.Template_Layout_P4AU_Sprite_Placement_Confirm(menuSession);
                    break;
                case "2":
                    account.P4AU_TS_Position = "Center";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P4AU_Menu.Template_Layout_P4AU_Sprite_Placement_Confirm(menuSession);
                    break;
                case "3":
                    account.P4AU_TS_Position = "Right";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P4AU_Menu.Template_Layout_P4AU_Sprite_Placement_Confirm(menuSession);
                    break;
                case "return":
                    _ = Template_Layout_P4AU_Menu.Template_Layout_P4AU_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P4AU_Highlight(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            var account = menuSession.Account;

            switch (component.Data.CustomId)
            {
                case "return":
                    _ = Template_Layout_P4AU_Menu.Template_Layout_P4AU_Main(menuSession);
                    break;

                case "on":
                    account.P4AU_TS_Highlight = "On";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P4AU_Menu.Template_Layout_P4AU_Highlight_Confirm(menuSession);
                    break;

                case "off":
                    account.P4AU_TS_Highlight = "Off";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P4AU_Menu.Template_Layout_P4AU_Highlight_Confirm(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P4AU_Scene_Type_Confirm(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "back-to-p4au-template-settings":
                    _ = Template_Layout_P4AU_Menu.Template_Layout_P4AU_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P4AU_Auto_Advance_Confirm(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "back-to-p4au-template-settings":
                    _ = Template_Layout_P4AU_Menu.Template_Layout_P4AU_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P4AU_Control_Panel_Confirm(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "back-to-p4au-template-settings":
                    _ = Template_Layout_P4AU_Menu.Template_Layout_P4AU_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P4AU_Sprite_Placement_Confirm(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "back-to-p4au-template-settings":
                    _ = Template_Layout_P4AU_Menu.Template_Layout_P4AU_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P4AU_Highlight_Confirm(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "back-to-p4au-template-settings":
                    _ = Template_Layout_P4AU_Menu.Template_Layout_P4AU_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }
    }
}
