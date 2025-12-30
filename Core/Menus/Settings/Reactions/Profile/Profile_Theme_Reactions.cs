using Discord.WebSocket;
using SocialLinker.Core.CloudStorageTables;
using SocialLinker.Core.Menus.Settings.Main.Profile;
using System.Threading.Tasks;

namespace SocialLinker.Core.Menus.Settings.Reactions.Profile
{
    class Profile_Theme_Reactions
    {
        public static Task Nav_Profile_Theme_Main(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            var account = menuSession.Account;

            switch (component.Data.CustomId)
            {
                case "return":
                    _ = Profile_Settings_Menu.Profile_Settings_Main(menuSession);
                    break;

                case "p3":
                    account.Profile_Theme = "P3";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Profile_Theme_Menu.Profile_Theme_Confirm(menuSession);
                    break;

                case "p4":
                    account.Profile_Theme = "P4";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Profile_Theme_Menu.Profile_Theme_Confirm(menuSession);
                    break;

                case "p5":
                    account.Profile_Theme = "P5";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Profile_Theme_Menu.Profile_Theme_Confirm(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Profile_Theme_Confirm(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "back-to-profile-settings":
                    _ = Profile_Settings_Menu.Profile_Settings_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }
    }
}
