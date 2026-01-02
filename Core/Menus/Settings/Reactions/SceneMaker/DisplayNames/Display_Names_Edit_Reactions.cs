using Discord.WebSocket;
using SocialLinker.Core.CloudStorageTables;
using SocialLinker.Core.Menus.Settings.Main.SceneMaker.DisplayNames;
using System.Linq;
using System.Threading.Tasks;

namespace SocialLinker.Core.Menus.Settings.Reactions.SceneMaker.DisplayNames
{
    internal class Display_Names_Edit_Reactions
    {
        public static Task Nav_Display_Names_Edit_Main(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            // Search for an item list that corresponds to the user's ID. If a menu entry was found, this should also exist alongside it.
            var itemSession = Global.ItemIdList.SingleOrDefault(x => x.User.Id == menuSession.User.Id);

            switch (component.Data.CustomId)
            {
                case "return":
                    _ = Display_Names_Menu.Display_Names_Start(menuSession);
                    break;

                case "delete":
                    _ = Display_Names_Edit_Menu.Display_Names_Delete_Confirmation(menuSession, itemSession.ItemIndexBase);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Display_Names_Delete_Confirmation(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            // Search for an item list that corresponds to the user's ID. If a menu entry was found, this should also exist alongside it.
            var itemSession = Global.ItemIdList.SingleOrDefault(x => x.User.Id == menuSession.User.Id);

            switch (component.Data.CustomId)
            {
                case "return":
                    _ = Display_Names_Edit_Menu.Display_Names_Edit_Main(menuSession, itemSession.CurrentMenuItem);
                    break;

                case "confirm":
                    DisplayNameLogging.DeleteCustomName(itemSession.SelectedDisplayName);
                    _ = Display_Names_Menu.Display_Names_Start(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }
    }
}
