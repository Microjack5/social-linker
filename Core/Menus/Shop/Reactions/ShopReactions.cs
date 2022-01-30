using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.WebSocket;
using SocialLinker.Core.CloudStorageTables;
using SocialLinker.Core.LocalStorageTables;
using SocialLinker.Core.Menus.Shop.Main;

namespace SocialLinker.Core.Menus.Shop.Reactions
{
    class ShopReactions
    {
        public static Task Nav_ShopMainMenu(SocketReaction reaction, MenuIdStructure menuSession)
        {
            // Search for an item list that corresponds to the user's ID. If a menu entry was found, this should also exist alongside it.
            var itemSession = Global.ItemIdList.SingleOrDefault(x => x.User.Id == reaction.UserId);

            // Perform various actions based on what type of reaction was given to the message.
            // Previous Page
            if (reaction.Emote.Name == "◀️")
            {
                try
                {
                    // Decrease the item index by the maximum number of items that should be displayed to the user at once.
                    itemSession.ItemIndexBase -= itemSession.MaxItemsDisplayed;

                    // Decrease the page counter by one.
                    itemSession.CurrentPage--;

                    // Stop the timeout timer associated with the menu.
                    menuSession.MenuTimer.Stop();

                    // Go to a new menu.
                    _ = ShopMenu.ShopMainMenu(menuSession.User, menuSession.MenuMessage);
                    return Task.CompletedTask;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

            }

            // Next Page
            else if (reaction.Emote.Name == "▶️")
            {
                try
                {
                    // Increase the item index by the maximum number of items that should be displayed to the user at once.
                    itemSession.ItemIndexBase += itemSession.MaxItemsDisplayed;

                    // Increase the page counter by one.
                    itemSession.CurrentPage++;

                    // Stop the timeout timer associated with the menu.
                    menuSession.MenuTimer.Stop();

                    // Go to a new menu.
                    _ = ShopMenu.ShopMainMenu(menuSession.User, menuSession.MenuMessage);
                    return Task.CompletedTask;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

            }

            // Keycap One
            else if (reaction.Emote.Name == "\u0031\ufe0f\u20e3")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = ShopMenu.ShopDecorPreview(menuSession.User, menuSession.MenuMessage, itemSession.ItemIndexBase);
                return Task.CompletedTask;
            }

            // Keycap Two
            else if (reaction.Emote.Name == "\u0032\ufe0f\u20e3")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = ShopMenu.ShopDecorPreview(menuSession.User, menuSession.MenuMessage, itemSession.ItemIndexBase + 1);
                return Task.CompletedTask;
            }

            // Keycap Three
            else if (reaction.Emote.Name == "\u0033\ufe0f\u20e3")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = ShopMenu.ShopDecorPreview(menuSession.User, menuSession.MenuMessage, itemSession.ItemIndexBase + 2);
                return Task.CompletedTask;
            }

            // Keycap Four
            else if (reaction.Emote.Name == "\u0034\ufe0f\u20e3")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = ShopMenu.ShopDecorPreview(menuSession.User, menuSession.MenuMessage, itemSession.ItemIndexBase + 3);
                return Task.CompletedTask;
            }

            // Keycap Five
            else if (reaction.Emote.Name == "\u0035\ufe0f\u20e3")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = ShopMenu.ShopDecorPreview(menuSession.User, menuSession.MenuMessage, itemSession.ItemIndexBase + 4);
                return Task.CompletedTask;
            }

            // Keycap Six
            else if (reaction.Emote.Name == "\u0036\ufe0f\u20e3")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = ShopMenu.ShopDecorPreview(menuSession.User, menuSession.MenuMessage, itemSession.ItemIndexBase + 5);
                return Task.CompletedTask;
            }

            // Sort Shop
            else if (reaction.Emote.Name == "⚙️")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = ShopMenu.ShopSort(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            // Exit Shop
            else if (reaction.Emote.Name == "❌")
            {
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

                // If the menu session is not null, remove it and the item entries from the global list.
                if (menuSession != null)
                {
                    Global.MenuIdList.Remove(menuSession);
                    Global.ItemIdList.Remove(itemSession);
                }
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_ShopDecorPreview(SocketReaction reaction, MenuIdStructure menuSession)
        {
            if (reaction.Emote.Name == "↩️")
            {
                // Stop the timeout timer associated with the menu
                menuSession.MenuTimer.Stop();

                // Go to a new menu
                _ = ShopMenu.ShopMainMenu(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            // If the user reacts with the checkmark emote, it's time to purchase the décor!
            else if (reaction.Emote.Name == "✅")
            {
                // Get the account information of the user.
                var account = UserInfoClasses.GetAccount(menuSession.User);

                // Search for an item list that corresponds to the user's ID. If a menu entry was found, this should also exist alongside it.
                var itemSession = Global.ItemIdList.SingleOrDefault(x => x.User.Id == reaction.UserId);

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

                // Stop the timeout timer associated with the menu
                menuSession.MenuTimer.Stop();

                // Go to a new menu
                _ = ShopMenu.ShopDecorPurchased(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_ShopDecorPurchased(SocketReaction reaction, MenuIdStructure menuSession)
        {
            if (reaction.Emote.Name == "❌")
            {
                // Stop the timeout timer associated with the menu
                menuSession.MenuTimer.Stop();

                // Go to a new menu
                _ = ShopMenu.ShopDecorPurchaseNotSet(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            else if (reaction.Emote.Name == "✅")
            {
                // Get the account information of the user.
                var account = UserInfoClasses.GetAccount(menuSession.User);

                // Search for an item list that corresponds to the user's ID. If a menu entry was found, this should also exist alongside it.
                var itemSession = Global.ItemIdList.SingleOrDefault(x => x.User.Id == reaction.UserId);

                // Change the user's Decor_Setting setting to the SelectedItem.
                account.Decor_Setting = itemSession.SelectedItem;

                //Update the user's account.
                UserInfoClasses.UpdateAccount(account);

                // Stop the timeout timer associated with the menu
                menuSession.MenuTimer.Stop();

                // Go to a new menu
                _ = ShopMenu.ShopDecorPurchaseSet(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_ShopDecorPurchaseSet(SocketReaction reaction, MenuIdStructure menuSession)
        {
            // Search for an item list that corresponds to the user's ID. If a menu entry was found, this should also exist alongside it.
            var itemSession = Global.ItemIdList.SingleOrDefault(x => x.User.Id == reaction.UserId);

            if (reaction.Emote.Name == "💠")
            {
                var channel = (SocketTextChannel)menuSession.MenuMessage.Channel;
                var user = menuSession.User;

                // Stop the timeout timer associated with the menu
                menuSession.MenuTimer.Stop();

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
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_ShopDecorPurchaseNotSet(SocketReaction reaction, MenuIdStructure menuSession)
        {
            // Search for an item list that corresponds to the user's ID. If a menu entry was found, this should also exist alongside it.
            var itemSession = Global.ItemIdList.SingleOrDefault(x => x.User.Id == reaction.UserId);

            if (reaction.Emote.Name == "💠")
            {
                var channel = (SocketTextChannel)menuSession.MenuMessage.Channel;
                var user = menuSession.User;

                // Stop the timeout timer associated with the menu
                menuSession.MenuTimer.Stop();

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
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_ShopSort(SocketReaction reaction, MenuIdStructure menuSession)
        {
            if (reaction.Emote.Name == "↩️")
            {
                // Stop the timeout timer associated with the menu
                menuSession.MenuTimer.Stop();

                // Go to a new menu
                _ = ShopMenu.ShopMainMenu(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            else if (reaction.Emote.Name == "\u0031\ufe0f\u20e3") // Keycap one
            {
                // Get the account information of the user.
                var account = UserInfoClasses.GetAccount(menuSession.User);

                // Change the user's Shop_Sort setting to the chosen value.
                account.Shop_Sort = "title_a_z";

                //Update the user's account.
                UserInfoClasses.UpdateAccount(account);

                // Stop the timeout timer associated with the menu
                menuSession.MenuTimer.Stop();

                // Go to a new menu
                _ = ShopMenu.ShopSortConfirm(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            else if (reaction.Emote.Name == "\u0032\ufe0f\u20e3") // Keycap two
            {
                // Get the account information of the user.
                var account = UserInfoClasses.GetAccount(menuSession.User);

                // Change the user's Shop_Sort setting to the chosen value.
                account.Shop_Sort = "title_z_a";

                //Update the user's account.
                UserInfoClasses.UpdateAccount(account);

                // Stop the timeout timer associated with the menu
                menuSession.MenuTimer.Stop();

                // Go to a new menu
                _ = ShopMenu.ShopSortConfirm(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            else if (reaction.Emote.Name == "\u0033\ufe0f\u20e3") // Keycap three
            {
                // Get the account information of the user.
                var account = UserInfoClasses.GetAccount(menuSession.User);

                // Change the user's Shop_Sort setting to the chosen value.
                account.Shop_Sort = "cost_low_high";

                //Update the user's account.
                UserInfoClasses.UpdateAccount(account);

                // Stop the timeout timer associated with the menu
                menuSession.MenuTimer.Stop();

                // Go to a new menu
                _ = ShopMenu.ShopSortConfirm(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            else if (reaction.Emote.Name == "\u0034\ufe0f\u20e3") // Keycap four
            {
                // Get the account information of the user.
                var account = UserInfoClasses.GetAccount(menuSession.User);

                // Change the user's Shop_Sort setting to the chosen value.
                account.Shop_Sort = "cost_high_low";

                //Update the user's account.
                UserInfoClasses.UpdateAccount(account);

                // Stop the timeout timer associated with the menu
                menuSession.MenuTimer.Stop();

                // Go to a new menu
                _ = ShopMenu.ShopSortConfirm(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            else if (reaction.Emote.Name == "\u0035\ufe0f\u20e3") // Keycap five
            {
                // Get the account information of the user.
                var account = UserInfoClasses.GetAccount(menuSession.User);

                // Change the user's Shop_Sort setting to the chosen value.
                account.Shop_Sort = "release_old_new";

                //Update the user's account.
                UserInfoClasses.UpdateAccount(account);

                // Stop the timeout timer associated with the menu
                menuSession.MenuTimer.Stop();

                // Go to a new menu
                _ = ShopMenu.ShopSortConfirm(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            else if (reaction.Emote.Name == "\u0036\ufe0f\u20e3") // Keycap six
            {
                // Get the account information of the user.
                var account = UserInfoClasses.GetAccount(menuSession.User);

                // Change the user's Shop_Sort setting to the chosen value.
                account.Shop_Sort = "release_new_old";

                //Update the user's account.
                UserInfoClasses.UpdateAccount(account);

                // Stop the timeout timer associated with the menu
                menuSession.MenuTimer.Stop();

                // Go to a new menu
                _ = ShopMenu.ShopSortConfirm(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_ShopSortConfirm(SocketReaction reaction, MenuIdStructure menuSession)
        {
            // Search for an item list that corresponds to the user's ID. If a menu entry was found, this should also exist alongside it.
            var itemSession = Global.ItemIdList.SingleOrDefault(x => x.User.Id == reaction.UserId);

            if (reaction.Emote.Name == "💠")
            {
                var channel = (SocketTextChannel)menuSession.MenuMessage.Channel;
                var user = menuSession.User;

                // Stop the timeout timer associated with the menu
                menuSession.MenuTimer.Stop();

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
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }
    }
}
