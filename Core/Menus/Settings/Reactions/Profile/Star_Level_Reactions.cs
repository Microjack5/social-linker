using System.Threading.Tasks;
using Discord.WebSocket;
using SocialLinker.Core.CloudStorageTables;
using SocialLinker.Core.Menus.Settings.Main.Profile;

namespace SocialLinker.Core.Menus.Settings.Reactions.Profile
{
    class Star_Level_Reactions
    {
        public static Task Nav_Star_Level_Main(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "return":
                    _ = Profile_Settings_Menu.Profile_Settings_Main(menuSession);
                    break;

                case "confirm":
                    _ = Star_Level_Menu.Star_Level_Check(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Star_Level_Check(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "profile-settings":
                    _ = Profile_Settings_Menu.Profile_Settings_Main(menuSession);
                    break;

                case "confirm":
                    var account = menuSession.Account;

                    // If the user is below Star Level Rank 2, reset their level and total EXP to their base values. 
                    if (account.Level_Resets < 2)
                    {
                        account.Level = 1;
                        account.Total_Exp = 0;
                    }

                    // If the user is currently at Star Level Rank 2, max out their P-Medal value.
                    if (account.Level_Resets == 2)
                    {
                        account.P_Medals = 999;
                    }

                    // Increase the user's level reset value by 1.
                    account.Level_Resets += 1;

                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;

                    _ = Star_Level_Menu.Star_Level_Confirm(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Star_Level_Confirm(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "profile-settings":
                    _ = Profile_Settings_Menu.Profile_Settings_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }
    }
}
