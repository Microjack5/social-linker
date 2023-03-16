using System.Linq;
using System.Threading.Tasks;
using Discord.WebSocket;
using SocialLinker.Core.CloudStorageTables;
using SocialLinker.Core.Menus.Settings.Main.SceneMaker.DisplayNames;

namespace SocialLinker.Core.Menus.Settings.Reactions.SceneMaker.DisplayNames
{
    internal class Display_Names_Edit_Reactions
    {
        public static Task Nav_Display_Names_Edit_Main(SocketReaction reaction, MenuIdStructure menuSession)
        {
            // Search for an item list that corresponds to the user's ID. If a menu entry was found, this should also exist alongside it.
            var itemSession = Global.ItemIdList.SingleOrDefault(x => x.User.Id == reaction.UserId);

            // Perform various actions based on what type of reaction was given to the message.
            if (reaction.Emote.Name == "↩️")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Display_Names_Menu.Display_Names_Start(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            // Keycap One
            else if (reaction.Emote.Name == "\u0031\ufe0f\u20e3")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Display_Names_Edit_Menu.Display_Names_Delete_Confirmation(menuSession.User, menuSession.MenuMessage, itemSession.ItemIndexBase);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Display_Names_Delete_Confirmation(SocketReaction reaction, MenuIdStructure menuSession)
        {
            // Search for an item list that corresponds to the user's ID. If a menu entry was found, this should also exist alongside it.
            var itemSession = Global.ItemIdList.SingleOrDefault(x => x.User.Id == reaction.UserId);

            // Perform various actions based on what type of reaction was given to the message.
            if (reaction.Emote.Name == "↩️")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Display_Names_Edit_Menu.Display_Names_Edit_Main(menuSession.User, menuSession.MenuMessage, itemSession.CurrentMenuItem);
                return Task.CompletedTask;
            }
            else if (reaction.Emote.Name == "✅")
            {
                DisplayNameLogging.DeleteCustomName(itemSession.SelectedDisplayName);

                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Display_Names_Menu.Display_Names_Start(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }
    }
}
