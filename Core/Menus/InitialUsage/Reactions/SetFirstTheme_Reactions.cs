using Discord.WebSocket;
using SocialLinker.Core.CloudStorageTables;
using SocialLinker.Core.Menus.InitialUsage.Main;
using System;
using System.Threading.Tasks;

namespace SocialLinker.Core.Menus.InitialUsage.Reactions
{
    public class SetFirstTheme_Reactions
    {
        public static Task Nav_SetFirstThemeMain(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            var account = menuSession.Account;

            switch (component.Data.CustomId)
            {
                case "p3":
                    account.Profile_Theme = "P3";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = SetFirstTheme_Menu.SetFirstThemeConfirm(menuSession);
                    break;

                case "p4":
                    account.Profile_Theme = "P4";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = SetFirstTheme_Menu.SetFirstThemeConfirm(menuSession);
                    break;

                case "p5":
                    account.Profile_Theme = "P5";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = SetFirstTheme_Menu.SetFirstThemeConfirm(menuSession);
                    break;

                case "close":
                    // Stop the timeout timer associated with the menu.
                    menuSession.MenuTimer.Stop();

                    // Attempt to delete the menu message from the channel if it hasn't been deleted by the user yet. If this fails, catch the exception.
                    try
                    {
                        _ = menuSession.MenuMessage.DeleteAsync();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }

                    // If the menu session is not null, remove it from the global list.
                    if (menuSession != null)
                    {
                        Global.MenuIdList.Remove(menuSession);
                    }
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_SetFirstThemeConfirm(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            var account = menuSession.Account;

            switch (component.Data.CustomId)
            {
                case "close":
                    // Stop the timeout timer associated with the menu.
                    menuSession.MenuTimer.Stop();

                    // Attempt to delete the menu message from the channel if it hasn't been deleted by the user yet. If this fails, catch the exception.
                    try
                    {
                        _ = menuSession.MenuMessage.DeleteAsync();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }

                    // If the menu session is not null, remove it from the global list.
                    if (menuSession != null)
                    {
                        Global.MenuIdList.Remove(menuSession);
                    }
                    break;
            }

            return Task.CompletedTask;
        }
    }
}