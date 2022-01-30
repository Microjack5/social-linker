using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.WebSocket;
using SocialLinker.Core.CloudStorageTables;
using SocialLinker.Core.Menus.Settings.Main.Profile;

namespace SocialLinker.Core.Menus.Settings.Reactions.Profile
{
    class Status_Decor_Reactions
    {
        public static Task Nav_Status_Decor_Main(SocketReaction reaction, MenuIdStructure menuSession)
        {
            // Search for an item list that corresponds to the user's ID. If a menu entry was found, this should also exist alongside it.
            var itemSession = Global.ItemIdList.SingleOrDefault(x => x.User.Id == reaction.UserId);

            // Perform various actions based on what type of reaction was given to the message.
            if (reaction.Emote.Name == "↩️")
            {
                // Stop the timeout timer associated with the menu
                menuSession.MenuTimer.Stop();

                // Go to a new menu
                _ = Status_Decor_Menu.Status_Decor_Exit(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            // Previous Page
            else if (reaction.Emote.Name == "◀️")
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
                    _ = Status_Decor_Menu.Status_Decor_Main(menuSession.User, menuSession.MenuMessage);
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
                    _ = Status_Decor_Menu.Status_Decor_Main(menuSession.User, menuSession.MenuMessage);
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
                _ = Status_Decor_Menu.Set_Decor_Preview(menuSession.User, menuSession.MenuMessage, itemSession.ItemIndexBase);
                return Task.CompletedTask;
            }

            // Keycap Two
            else if (reaction.Emote.Name == "\u0032\ufe0f\u20e3")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Status_Decor_Menu.Set_Decor_Preview(menuSession.User, menuSession.MenuMessage, itemSession.ItemIndexBase + 1);
                return Task.CompletedTask;
            }

            // Keycap Three
            else if (reaction.Emote.Name == "\u0033\ufe0f\u20e3")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Status_Decor_Menu.Set_Decor_Preview(menuSession.User, menuSession.MenuMessage, itemSession.ItemIndexBase + 2);
                return Task.CompletedTask;
            }

            // Keycap Four
            else if (reaction.Emote.Name == "\u0034\ufe0f\u20e3")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Status_Decor_Menu.Set_Decor_Preview(menuSession.User, menuSession.MenuMessage, itemSession.ItemIndexBase + 3);
                return Task.CompletedTask;
            }

            // Keycap Five
            else if (reaction.Emote.Name == "\u0035\ufe0f\u20e3")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Status_Decor_Menu.Set_Decor_Preview(menuSession.User, menuSession.MenuMessage, itemSession.ItemIndexBase + 4);
                return Task.CompletedTask;
            }

            // Keycap Six
            else if (reaction.Emote.Name == "\u0036\ufe0f\u20e3")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Status_Decor_Menu.Set_Decor_Preview(menuSession.User, menuSession.MenuMessage, itemSession.ItemIndexBase + 5);
                return Task.CompletedTask;
            }

            // Sort Shop
            else if (reaction.Emote.Name == "⚙️")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Status_Decor_Menu.Decor_Sort(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            // If the user reacts with the box emote, we want to remove the current décor.
            else if (reaction.Emote.Name == "🔳")
            {
                // Get the account information of the user.
                var account = UserInfoClasses.GetAccount(menuSession.User);

                // Change the user's Decor_Setting setting to an empty string.
                account.Decor_Setting = "";

                // Update the user's account.
                UserInfoClasses.UpdateAccount(account);

                // Stop the timeout timer associated with the menu
                menuSession.MenuTimer.Stop();

                // Go to a new menu
                _ = Status_Decor_Menu.Set_Decor_Confirm(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Set_Decor_Preview(SocketReaction reaction, MenuIdStructure menuSession)
        {
            if (reaction.Emote.Name == "↩️")
            {
                // Stop the timeout timer associated with the menu
                menuSession.MenuTimer.Stop();

                // Go to a new menu
                _ = Status_Decor_Menu.Status_Decor_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            // If the user reacts with the checkmark emote, it's time to set the décor!
            else if (reaction.Emote.Name == "✅")
            {
                // Get the account information of the user.
                var account = UserInfoClasses.GetAccount(menuSession.User);

                // Search for an item list that corresponds to the user's ID. If a menu entry was found, this should also exist alongside it.
                var itemSession = Global.ItemIdList.SingleOrDefault(x => x.User.Id == reaction.UserId);

                // Change the user's Decor_Setting setting to the SelectedItem.
                account.Decor_Setting = itemSession.SelectedItem;

                // Update the user's account.
                UserInfoClasses.UpdateAccount(account);

                // Stop the timeout timer associated with the menu
                menuSession.MenuTimer.Stop();

                // Go to a new menu
                _ = Status_Decor_Menu.Set_Decor_Confirm(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Set_Decor_Confirm(SocketReaction reaction, MenuIdStructure menuSession)
        {
            if (reaction.Emote.Name == "💠")
            {
                // Stop the timeout timer associated with the menu
                menuSession.MenuTimer.Stop();

                // Go to a new menu
                _ = Status_Decor_Menu.Status_Decor_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }
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

                // If the menu session is not null, remove it from the global list.
                if (menuSession != null)
                {
                    Global.MenuIdList.Remove(menuSession);
                }
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Decor_Sort(SocketReaction reaction, MenuIdStructure menuSession)
        {
            if (reaction.Emote.Name == "↩️")
            {
                // Stop the timeout timer associated with the menu
                menuSession.MenuTimer.Stop();

                // Go to a new menu
                _ = Status_Decor_Menu.Status_Decor_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            // Keycap one
            else if (reaction.Emote.Name == "\u0031\ufe0f\u20e3") 
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
                _ = Status_Decor_Menu.Decor_Sort_Confirm(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            // Keycap two
            else if (reaction.Emote.Name == "\u0032\ufe0f\u20e3") 
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
                _ = Status_Decor_Menu.Decor_Sort_Confirm(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            // Keycap three
            else if (reaction.Emote.Name == "\u0033\ufe0f\u20e3") 
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
                _ = Status_Decor_Menu.Decor_Sort_Confirm(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            // Keycap four
            else if (reaction.Emote.Name == "\u0034\ufe0f\u20e3") 
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
                _ = Status_Decor_Menu.Decor_Sort_Confirm(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            // Keycap five
            else if (reaction.Emote.Name == "\u0035\ufe0f\u20e3") 
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
                _ = Status_Decor_Menu.Decor_Sort_Confirm(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            // Keycap six
            else if (reaction.Emote.Name == "\u0036\ufe0f\u20e3") 
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
                _ = Status_Decor_Menu.Decor_Sort_Confirm(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Decor_Sort_Confirm(SocketReaction reaction, MenuIdStructure menuSession)
        {
            // Search for an item list that corresponds to the user's ID. If a menu entry was found, this should also exist alongside it.
            var itemSession = Global.ItemIdList.SingleOrDefault(x => x.User.Id == reaction.UserId);

            if (reaction.Emote.Name == "💠")
            {
                // If the item session is not null, remove it from the global list.
                if (menuSession != null)
                {
                    Global.ItemIdList.Remove(itemSession);
                }

                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Status_Decor_Menu.Status_Decor_Start(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }
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

                // If the menu session is not null, remove it from the global list.
                if (menuSession != null)
                {
                    Global.MenuIdList.Remove(menuSession);
                }
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }
    }
}
