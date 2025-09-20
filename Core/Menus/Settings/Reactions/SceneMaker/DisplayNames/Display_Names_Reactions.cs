using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.WebSocket;
using SocialLinker.Core.Menus.Settings.Main.SceneMaker;
using SocialLinker.Core.Menus.Settings.Main.SceneMaker.DisplayNames;

namespace SocialLinker.Core.Menus.Settings.Reactions.SceneMaker.DisplayNames
{
    internal class Display_Names_Reactions
    {
        public static Task Nav_Display_Names_Main(SocketReaction reaction, MenuIdStructure menuSession)
        {
            // Search for an item list that corresponds to the user's ID. If a menu entry was found, this should also exist alongside it.
            var itemSession = Global.ItemIdList.SingleOrDefault(x => x.User.Id == reaction.UserId);

            // Perform various actions based on what type of reaction was given to the message.
            if (reaction.Emote.Name == "↩️")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = SM_Settings_Menu.SM_Settings_Main(menuSession);
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
                    _ = Display_Names_Menu.Display_Names_Main(menuSession.User, menuSession.MenuMessage);
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
                    _ = Display_Names_Menu.Display_Names_Main(menuSession.User, menuSession.MenuMessage);
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
                _ = Display_Names_Edit_Menu.Display_Names_Edit_Main(menuSession.User, menuSession.MenuMessage, itemSession.ItemIndexBase);
                return Task.CompletedTask;
            }

            // Keycap Two
            else if (reaction.Emote.Name == "\u0032\ufe0f\u20e3")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Display_Names_Edit_Menu.Display_Names_Edit_Main(menuSession.User, menuSession.MenuMessage, itemSession.ItemIndexBase + 1);
                return Task.CompletedTask;
            }

            // Keycap Three
            else if (reaction.Emote.Name == "\u0033\ufe0f\u20e3")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Display_Names_Edit_Menu.Display_Names_Edit_Main(menuSession.User, menuSession.MenuMessage, itemSession.ItemIndexBase + 2);
                return Task.CompletedTask;
            }

            // Keycap Four
            else if (reaction.Emote.Name == "\u0034\ufe0f\u20e3")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Display_Names_Edit_Menu.Display_Names_Edit_Main(menuSession.User, menuSession.MenuMessage, itemSession.ItemIndexBase + 3);
                return Task.CompletedTask;
            }

            // Keycap Five
            else if (reaction.Emote.Name == "\u0035\ufe0f\u20e3")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Display_Names_Edit_Menu.Display_Names_Edit_Main(menuSession.User, menuSession.MenuMessage, itemSession.ItemIndexBase + 4);
                return Task.CompletedTask;
            }

            else if (reaction.Emote.Name == "➕")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Display_Names_Title_Select_Menu.Display_Names_Title_Select(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            else if (reaction.Emote.Name == "⚙️")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Display_Names_Sort_Menu.Display_Names_Sort(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }
    }
}
