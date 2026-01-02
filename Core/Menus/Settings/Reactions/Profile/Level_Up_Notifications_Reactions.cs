using Discord.WebSocket;
using SocialLinker.Core.CloudStorageTables;
using SocialLinker.Core.Menus.Settings.Main.Profile;
using System.Threading.Tasks;

namespace SocialLinker.Core.Menus.Settings.Reactions.Profile
{
    class Level_Up_Notifications_Reactions
    {
        public static Task Nav_Level_Up_Notifications_Main(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            var account = menuSession.Account;

            switch (component.Data.CustomId)
            {
                case "return":
                    _ = Profile_Settings_Menu.Profile_Settings_Main(menuSession);
                    break;

                case "on":
                    account.Level_Up_Notifications = "On";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Level_Up_Notifications_Menu.Level_Up_Notifications_Confirm(menuSession);
                    break;

                case "off":
                    account.Level_Up_Notifications = "Off";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Level_Up_Notifications_Menu.Level_Up_Notifications_Confirm(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Level_Up_Notifications_Confirm(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "back-to-general-settings":
                    _ = Profile_Settings_Menu.Profile_Settings_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }
    }
}
