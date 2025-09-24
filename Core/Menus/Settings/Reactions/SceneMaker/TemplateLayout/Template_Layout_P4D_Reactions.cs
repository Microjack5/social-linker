using System.Linq;
using System.Threading.Tasks;
using Discord.WebSocket;
using SocialLinker.Core.CloudStorageTables;
using SocialLinker.Core.Menus.Settings.Main.SceneMaker;
using SocialLinker.Core.Menus.Settings.Main.SceneMaker.TemplateLayout;

namespace SocialLinker.Core.Menus.Settings.Reactions.SceneMaker.TemplateLayout
{
    class Template_Layout_P4D_Reactions
    {
        public static Task Nav_Template_Layout_P4D_Main(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            string selected = component.Data.Values.First();

            switch (selected)
            {
                case "1":
                    _ = Template_Layout_P4D_Menu.Template_Layout_P4D_Scene_Type(menuSession);
                    break;
                case "2":
                    _ = Template_Layout_P4D_Menu.Template_Layout_P4D_Auto_Advance(menuSession);
                    break;
                case "3":
                    _ = Template_Layout_P4D_Menu.Template_Layout_P4D_Sprite_Placement(menuSession);
                    break;
                case "return":
                    _ = Template_Layout_Menu.Template_Layout_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P4D_Scene_Type(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            string selected = component.Data.Values.First();
            var account = menuSession.Account;

            switch (selected)
            {
                case "1":
                    account.P4D_TS_Scene_Type = "Dialogue";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P4D_Menu.Template_Layout_P4D_Scene_Type_Confirm(menuSession);
                    break;
                case "2":
                    account.P4D_TS_Scene_Type = "Narration";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P4D_Menu.Template_Layout_P4D_Scene_Type_Confirm(menuSession);
                    break;
                case "return":
                    _ = Template_Layout_P4D_Menu.Template_Layout_P4D_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P4D_Auto_Advance(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            var account = menuSession.Account;

            switch (component.Data.CustomId)
            {
                case "return":
                    _ = Template_Layout_P4D_Menu.Template_Layout_P4D_Main(menuSession);
                    break;

                case "on":
                    account.P4D_TS_Auto_Advance = "On";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P4D_Menu.Template_Layout_P4D_Auto_Advance_Confirm(menuSession);
                    break;

                case "off":
                    account.P4D_TS_Auto_Advance = "Off";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P4D_Menu.Template_Layout_P4D_Auto_Advance_Confirm(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P4D_Sprite_Placement(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            string selected = component.Data.Values.First();
            var account = menuSession.Account;

            switch (selected)
            {
                case "1":
                    account.P4D_TS_Position = "Left";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P4D_Menu.Template_Layout_P4D_Sprite_Placement_Confirm(menuSession);
                    break;
                case "2":
                    account.P4D_TS_Position = "Center";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P4D_Menu.Template_Layout_P4D_Sprite_Placement_Confirm(menuSession);
                    break;
                case "3":
                    account.P4D_TS_Position = "Right";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P4D_Menu.Template_Layout_P4D_Sprite_Placement_Confirm(menuSession);
                    break;
                case "return":
                    _ = Template_Layout_P4D_Menu.Template_Layout_P4D_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P4D_Scene_Type_Confirm(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "back-to-p4d-template-settings":
                    _ = Template_Layout_P4D_Menu.Template_Layout_P4D_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P4D_Auto_Advance_Confirm(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "back-to-p4d-template-settings":
                    _ = Template_Layout_P4D_Menu.Template_Layout_P4D_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P4D_Sprite_Placement_Confirm(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "back-to-p4d-template-settings":
                    _ = Template_Layout_P4D_Menu.Template_Layout_P4D_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }
    }
}
