using Discord.WebSocket;
using SocialLinker.Core.CloudStorageTables;
using SocialLinker.Core.Menus.Help.Main;
using SocialLinker.Core.Menus.InitialUsage.Main;
using SocialLinker.Core.Menus.Settings.Main;
using System.Security.Principal;
using System.Threading.Tasks;

namespace SocialLinker.Core.Menus.InitialUsage.Reactions
{
    class First_Use_Content_Filter_Reactions
    {
        public static Task Nav_First_Use_Intro_Main(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            menuSession.MenuTimer.Stop();

            switch (component.Data.CustomId)
            {
                case "first-use-intro-confirm":
                    var account = UserInfoClasses.GetAccount(menuSession.User);

                    account.Account_Activated = "Yes";
                    UserInfoClasses.UpdateAccount(account);

                    component.DeferAsync(ephemeral: true);
                    _ = First_Use_Content_Filter_Menu.First_Use_Intro_Confirm(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_First_Use_Intro_Confirm(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            menuSession.MenuTimer.Stop();

            switch (component.Data.CustomId)
            {
                case "go-to-help-menu":
                    component.DeferAsync(ephemeral: true);
                    _ = Help_Menu.Help_Main_Menu(menuSession.User, menuSession.MenuMessage);
                    break;

                case "go-to-settings-menu":
                    component.DeferAsync(ephemeral: true);
                    _ = Settings_Menu.Settings_Main_Menu(menuSession.User, menuSession.MenuMessage);
                    break;

                case "go-to-set-profile-theme":
                    component.DeferAsync(ephemeral: true);
                    _ = SetFirstTheme_Menu.SetFirstThemeMain((SocketTextChannel)menuSession.MenuMessage.Channel, menuSession.User);
                    break;
            }

            return Task.CompletedTask;
        }

    }
}
