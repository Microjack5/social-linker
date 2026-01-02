using Discord.WebSocket;
using SocialLinker.Core.CloudStorageTables;
using SocialLinker.Core.Menus.Settings.Main.SceneMaker;
using SocialLinker.Core.Menus.Settings.Main.SceneMaker.AutoDelete;
using System.Linq;
using System.Threading.Tasks;

namespace SocialLinker.Core.Menus.Settings.Reactions.SceneMaker.AutoDelete
{
    class Auto_Delete_Reactions
    {
        public static Task Nav_Auto_Delete_Main(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            string selected = component.Data.Values.First();

            switch (selected)
            {
                case "1":
                    _ = Auto_Delete_Menu.Auto_Delete_Commands(menuSession);
                    break;
                case "2":
                    _ = Auto_Delete_Menu.Auto_Delete_Error_Messages(menuSession);
                    break;
                case "return":
                    _ = SM_Settings_Menu.SM_Settings_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Auto_Delete_Commands(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            var account = menuSession.Account;

            switch (component.Data.CustomId)
            {
                case "return":
                    _ = Auto_Delete_Menu.Auto_Delete_Main(menuSession);
                    break;

                case "on":
                    account.Auto_Delete_Commands = "On";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Auto_Delete_Menu.Auto_Delete_Commands_Confirm(menuSession);
                    break;

                case "off":
                    account.Auto_Delete_Commands = "Off";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Auto_Delete_Menu.Auto_Delete_Commands_Confirm(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Auto_Delete_Error_Messages(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            var account = menuSession.Account;

            switch (component.Data.CustomId)
            {
                case "return":
                    _ = Auto_Delete_Menu.Auto_Delete_Main(menuSession);
                    break;

                case "on":
                    account.Auto_Delete_Error_Messages = "On";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Auto_Delete_Menu.Auto_Delete_Error_Messages_Confirm(menuSession);
                    break;

                case "off":
                    account.Auto_Delete_Error_Messages = "Off";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Auto_Delete_Menu.Auto_Delete_Error_Messages_Confirm(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Auto_Delete_Commands_Confirm(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "back-to-auto-delete-settings":
                    _ = Auto_Delete_Menu.Auto_Delete_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Auto_Delete_Error_Messages_Confirm(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "back-to-auto-delete-settings":
                    _ = Auto_Delete_Menu.Auto_Delete_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }
    }
}
