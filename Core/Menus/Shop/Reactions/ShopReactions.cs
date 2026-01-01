using Discord.WebSocket;
using SocialLinker.Core.CloudStorageTables;
using SocialLinker.Core.LocalStorageTables;
using SocialLinker.Core.Menus.Shop.Main;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace SocialLinker.Core.Menus.Shop.Reactions
{
    class ShopReactions
    {
        public static Task Nav_ShopMainMenu(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            // Search for an item list that corresponds to the user's ID. If a menu entry was found, this should also exist alongside it.
            var itemSession = Global.ItemIdList.SingleOrDefault(x => x.User.Id == menuSession.User.Id);
            var account = menuSession.Account;

            switch (component.Data.CustomId)
            {
                case "previous-page":
                    // Decrease the item index by the maximum number of items that should be displayed to the user at once.
                    itemSession.ItemIndexBase -= itemSession.MaxItemsDisplayed;

                    // Decrease the page counter by one.
                    itemSession.CurrentPage--;

                    // Go to a new menu.
                    _ = ShopMenu.ShopMainMenu(menuSession.User, menuSession.MenuMessage);
                    break;

                case "next-page":
                    // Increase the item index by the maximum number of items that should be displayed to the user at once.
                    itemSession.ItemIndexBase += itemSession.MaxItemsDisplayed;

                    // Increase the page counter by one.
                    itemSession.CurrentPage++;

                    // Go to a new menu.
                    _ = ShopMenu.ShopMainMenu(menuSession.User, menuSession.MenuMessage);
                    break;

                case "sort":
                    _ = ShopMenu.ShopSort(menuSession.User, menuSession.MenuMessage);
                    break;

                case "exit":
                    // Attempt to delete the menu message from the channel if it hasn't been deleted by the user yet. If this fails, catch the exception.
                    try
                    {
                        _ = menuSession.MenuMessage.DeleteAsync();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }

                    // If the menu session is not null, remove it and the item entries from the global list.
                    if (menuSession != null)
                    {
                        Global.MenuIdList.Remove(menuSession);
                        Global.ItemIdList.Remove(itemSession);
                    }
                    break;
            }

            string selected = component.Data.Values.First();

            switch (selected)
            {
                case "1":
                    itemSession.SelectedItem = itemSession.ItemList[itemSession.ItemIndexBase];
                    _ = ShopMenu.ShopDecorPreview(menuSession.User, menuSession.MenuMessage, itemSession.ItemIndexBase);
                    break;
                case "2":
                    itemSession.SelectedItem = itemSession.ItemList[itemSession.ItemIndexBase + 1];
                    _ = ShopMenu.ShopDecorPreview(menuSession.User, menuSession.MenuMessage, itemSession.ItemIndexBase + 1);
                    break;
                case "3":
                    itemSession.SelectedItem = itemSession.ItemList[itemSession.ItemIndexBase + 2];
                    _ = ShopMenu.ShopDecorPreview(menuSession.User, menuSession.MenuMessage, itemSession.ItemIndexBase + 2);
                    break;
                case "4":
                    itemSession.SelectedItem = itemSession.ItemList[itemSession.ItemIndexBase + 3];
                    _ = ShopMenu.ShopDecorPreview(menuSession.User, menuSession.MenuMessage, itemSession.ItemIndexBase + 3);
                    break;
                case "5":
                    itemSession.SelectedItem = itemSession.ItemList[itemSession.ItemIndexBase + 4];
                    _ = ShopMenu.ShopDecorPreview(menuSession.User, menuSession.MenuMessage, itemSession.ItemIndexBase + 4);
                    break;
                case "6":
                    itemSession.SelectedItem = itemSession.ItemList[itemSession.ItemIndexBase + 5];
                    _ = ShopMenu.ShopDecorPreview(menuSession.User, menuSession.MenuMessage, itemSession.ItemIndexBase + 5);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_ShopDecorPreview(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "return":
                    _ = ShopMenu.ShopMainMenu(menuSession.User, menuSession.MenuMessage);
                    break;

                case "confirm":
                    var account = menuSession.Account;

                    // Search for an item list that corresponds to the user's ID. If a menu entry was found, this should also exist alongside it.
                    var itemSession = Global.ItemIdList.SingleOrDefault(x => x.User.Id == menuSession.User.Id);

                    // Get the information of the chosen décor index.
                    var decor_info = DecorInfoMethods.GetDecorInfo(itemSession.SelectedItem);

                    // If the user has not maxed out their Star Level rank, deduct the cost of the purchased décor from the user's P-Medal value.
                    if (account.Level_Resets < 3)
                    {
                        account.P_Medals -= decor_info.Price;
                    }

                    // Add the purchased item to the user's list of owned décor.
                    account.Decor_Owned += $"{itemSession.SelectedItem};";

                    //Update the user's account.
                    UserInfoClasses.UpdateAccount(account);

                    // Go to a new menu
                    _ = ShopMenu.ShopDecorPurchased(menuSession.User, menuSession.MenuMessage);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_ShopDecorPurchased(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "no":
                    _ = ShopMenu.ShopDecorPurchaseNotSet(menuSession.User, menuSession.MenuMessage);
                    break;

                case "yes":
                    var account = menuSession.Account;
                    var itemSession = Global.ItemIdList.SingleOrDefault(x => x.User.Id == menuSession.User.Id);

                    // Change the user's Decor_Setting setting to the SelectedItem.
                    account.Decor_Setting = itemSession.SelectedItem;

                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;

                    _ = ShopMenu.ShopDecorPurchaseSet(menuSession.User, menuSession.MenuMessage);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_ShopDecorPurchaseSet(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            var itemSession = Global.ItemIdList.SingleOrDefault(x => x.User.Id == menuSession.User.Id);

            switch (component.Data.CustomId)
            {
                case "return":
                    var channel = (SocketTextChannel)menuSession.MenuMessage.Channel;
                    var user = menuSession.User;

                    _ = menuSession.MenuMessage.DeleteAsync();

                    // If the menu session is not null, remove it and the item entries from the global list.
                    if (menuSession != null)
                    {
                        Global.MenuIdList.Remove(menuSession);
                        Global.ItemIdList.Remove(itemSession);
                    }

                    // Go to a new menu
                    _ = ShopMenu.ShopStart(channel, user);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_ShopDecorPurchaseNotSet(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            var itemSession = Global.ItemIdList.SingleOrDefault(x => x.User.Id == menuSession.User.Id);

            switch (component.Data.CustomId)
            {
                case "return":
                    var channel = (SocketTextChannel)menuSession.MenuMessage.Channel;
                    var user = menuSession.User;

                    _ = menuSession.MenuMessage.DeleteAsync();

                    // If the menu session is not null, remove it and the item entries from the global list.
                    if (menuSession != null)
                    {
                        Global.MenuIdList.Remove(menuSession);
                        Global.ItemIdList.Remove(itemSession);
                    }

                    // Go to a new menu
                    _ = ShopMenu.ShopStart(channel, user);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_ShopSort(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            var account = menuSession.Account;

            switch (component.Data.CustomId)
            {
                case "return":
                    _ = ShopMenu.ShopMainMenu(menuSession.User, menuSession.MenuMessage);
                    break;
            }

            string selected = component.Data.Values.First();

            switch (selected)
            {
                case "1":
                    account.Shop_Sort = "title_a_z";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = ShopMenu.ShopSortConfirm(menuSession.User, menuSession.MenuMessage);
                    break;
                case "2":
                    account.Shop_Sort = "title_z_a";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = ShopMenu.ShopSortConfirm(menuSession.User, menuSession.MenuMessage);
                    break;
                case "3":
                    account.Shop_Sort = "cost_low_high";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = ShopMenu.ShopSortConfirm(menuSession.User, menuSession.MenuMessage);
                    break;
                case "4":
                    account.Shop_Sort = "cost_high_low";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = ShopMenu.ShopSortConfirm(menuSession.User, menuSession.MenuMessage);
                    break;
                case "5":
                    account.Shop_Sort = "release_old_new";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = ShopMenu.ShopSortConfirm(menuSession.User, menuSession.MenuMessage);
                    break;
                case "6":
                    account.Shop_Sort = "release_new_old";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = ShopMenu.ShopSortConfirm(menuSession.User, menuSession.MenuMessage);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_ShopSortConfirm(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            // Search for an item list that corresponds to the user's ID. If a menu entry was found, this should also exist alongside it.
            var itemSession = Global.ItemIdList.SingleOrDefault(x => x.User.Id == menuSession.User.Id);

            switch (component.Data.CustomId)
            {
                case "return":
                    var channel = (SocketTextChannel)menuSession.MenuMessage.Channel;
                    var user = menuSession.User;

                    // Delete the menu message from the channel.
                    _ = menuSession.MenuMessage.DeleteAsync();

                    // If the menu session is not null, remove it and the item entries from the global list.
                    if (menuSession != null)
                    {
                        Global.MenuIdList.Remove(menuSession);
                        Global.ItemIdList.Remove(itemSession);
                    }

                    // Go to a new menu
                    _ = ShopMenu.ShopStart(channel, user);
                    break;
            }

            return Task.CompletedTask;
        }
    }
}
