using Discord.WebSocket;
using SocialLinker.Core.CloudStorageTables;
using SocialLinker.Core.Menus.Settings.Main.Profile;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SocialLinker.Core.Menus.Settings.Reactions.Profile
{
    class Status_Decor_Reactions
    {
        public static Task Nav_Status_Decor_Main(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            // Search for an item list that corresponds to the user's ID. If a menu entry was found, this should also exist alongside it.
            var itemSession = Global.ItemIdList.SingleOrDefault(x => x.User.Id == menuSession.User.Id);
            var account = menuSession.Account;

            switch (component.Data.CustomId)
            {
                case "return":
                    _ = Status_Decor_Menu.Status_Decor_Exit(menuSession);
                    break;

                case "previous-page":
                    itemSession.ItemIndexBase -= itemSession.MaxItemsDisplayed;
                    itemSession.CurrentPage--;
                    _ = Status_Decor_Menu.Status_Decor_Main(menuSession);
                    break;

                case "next-page":
                    itemSession.ItemIndexBase += itemSession.MaxItemsDisplayed;
                    itemSession.CurrentPage++;
                    _ = Status_Decor_Menu.Status_Decor_Main(menuSession);
                    break;

                case "1":
                    _ = Status_Decor_Menu.Set_Decor_Preview(menuSession, itemSession.ItemIndexBase);
                    break;

                case "2":
                    _ = Status_Decor_Menu.Set_Decor_Preview(menuSession, itemSession.ItemIndexBase + 1);
                    break;

                case "3":
                    _ = Status_Decor_Menu.Set_Decor_Preview(menuSession, itemSession.ItemIndexBase + 2);
                    break;

                case "4":
                    _ = Status_Decor_Menu.Set_Decor_Preview(menuSession, itemSession.ItemIndexBase + 3);
                    break;

                case "5":
                    _ = Status_Decor_Menu.Set_Decor_Preview(menuSession, itemSession.ItemIndexBase + 4);
                    break;

                case "6":
                    _ = Status_Decor_Menu.Set_Decor_Preview(menuSession, itemSession.ItemIndexBase + 5);
                    break;

                case "sort":
                    _ = Status_Decor_Menu.Decor_Sort(menuSession);
                    break;

                case "default":
                    account.Decor_Setting = "";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Status_Decor_Menu.Set_Decor_Confirm(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Set_Decor_Preview(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            var itemSession = Global.ItemIdList.SingleOrDefault(x => x.User.Id == menuSession.User.Id);
            var account = menuSession.Account;

            switch (component.Data.CustomId)
            {
                case "return":
                    _ = Status_Decor_Menu.Status_Decor_Main(menuSession);
                    break;

                case "confirm":
                    account.Decor_Setting = itemSession.SelectedItem;
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Status_Decor_Menu.Set_Decor_Confirm(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Set_Decor_Confirm(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "decor-settings":
                    _ = Status_Decor_Menu.Status_Decor_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Decor_Sort(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            var account = menuSession.Account;

            switch (component.Data.CustomId)
            {
                case "return":
                    _ = Status_Decor_Menu.Status_Decor_Main(menuSession);
                    break;

                case "1":
                    account.Shop_Sort = "title_a_z";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Status_Decor_Menu.Decor_Sort_Confirm(menuSession);
                    break;

                case "2":
                    account.Shop_Sort = "title_z_a";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Status_Decor_Menu.Decor_Sort_Confirm(menuSession);
                    break;

                case "3":
                    account.Shop_Sort = "cost_low_high";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Status_Decor_Menu.Decor_Sort_Confirm(menuSession);
                    break;

                case "4":
                    account.Shop_Sort = "cost_high_low";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Status_Decor_Menu.Decor_Sort_Confirm(menuSession);
                    break;

                case "5":
                    account.Shop_Sort = "release_old_new";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Status_Decor_Menu.Decor_Sort_Confirm(menuSession);
                    break;

                case "6":
                    account.Shop_Sort = "release_new_old";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Status_Decor_Menu.Decor_Sort_Confirm(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Decor_Sort_Confirm(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            // Search for an item list that corresponds to the user's ID. If a menu entry was found, this should also exist alongside it.
            var itemSession = Global.ItemIdList.SingleOrDefault(x => x.User.Id == menuSession.User.Id);

            switch (component.Data.CustomId)
            {
                case "back-to-decor-settings":
                    if (menuSession != null)
                    {
                        Global.ItemIdList.Remove(itemSession);
                    }

                    _ = Status_Decor_Menu.Status_Decor_Start(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }
    }
}
