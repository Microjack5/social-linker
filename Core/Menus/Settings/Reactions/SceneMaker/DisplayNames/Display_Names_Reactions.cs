using Discord.WebSocket;
using SocialLinker.Core.Menus.Settings.Main.Profile;
using SocialLinker.Core.Menus.Settings.Main.SceneMaker;
using SocialLinker.Core.Menus.Settings.Main.SceneMaker.DisplayNames;
using System.Linq;
using System.Threading.Tasks;

namespace SocialLinker.Core.Menus.Settings.Reactions.SceneMaker.DisplayNames
{
    internal class Display_Names_Reactions
    {
        public static Task Nav_Display_Names_Main(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            // Search for an item list that corresponds to the user's ID. If a menu entry was found, this should also exist alongside it.
            var itemSession = Global.ItemIdList.SingleOrDefault(x => x.User.Id == menuSession.User.Id);
            var account = menuSession.Account;

            switch (component.Data.CustomId)
            {
                case "return":
                    _ = SM_Settings_Menu.SM_Settings_Main(menuSession);
                    break;

                case "previous-page":
                    itemSession.ItemIndexBase -= itemSession.MaxItemsDisplayed;
                    itemSession.CurrentPage--;
                    _ = Display_Names_Menu.Display_Names_Main(menuSession);
                    break;

                case "next-page":
                    itemSession.ItemIndexBase += itemSession.MaxItemsDisplayed;
                    itemSession.CurrentPage++;
                    _ = Display_Names_Menu.Display_Names_Main(menuSession);
                    break;

                case "add":
                    _ = Display_Names_Title_Select_Menu.Display_Names_Title_Select(menuSession);
                    break;

                case "sort":
                    _ = Display_Names_Sort_Menu.Display_Names_Sort(menuSession);
                    break;
            }

            string selected = component.Data.Values.First();

            switch (selected)
            {
                case "1":
                    //itemSession.SelectedItem = itemSession.ItemList[itemSession.ItemIndexBase];
                    _ = Display_Names_Edit_Menu.Display_Names_Edit_Main(menuSession, itemSession.ItemIndexBase);
                    break;
                case "2":
                    //itemSession.SelectedItem = itemSession.ItemList[itemSession.ItemIndexBase + 1];
                    _ = Display_Names_Edit_Menu.Display_Names_Edit_Main(menuSession, itemSession.ItemIndexBase + 1);
                    break;
                case "3":
                    //itemSession.SelectedItem = itemSession.ItemList[itemSession.ItemIndexBase + 2];
                    _ = Display_Names_Edit_Menu.Display_Names_Edit_Main(menuSession, itemSession.ItemIndexBase + 2);
                    break;
                case "4":
                    //itemSession.SelectedItem = itemSession.ItemList[itemSession.ItemIndexBase + 3];
                    _ = Display_Names_Edit_Menu.Display_Names_Edit_Main(menuSession, itemSession.ItemIndexBase + 3);
                    break;
                case "5":
                    //itemSession.SelectedItem = itemSession.ItemList[itemSession.ItemIndexBase + 4];
                    _ = Display_Names_Edit_Menu.Display_Names_Edit_Main(menuSession, itemSession.ItemIndexBase + 4);
                    break;
                case "6":
                    //itemSession.SelectedItem = itemSession.ItemList[itemSession.ItemIndexBase + 5];
                    _ = Display_Names_Edit_Menu.Display_Names_Edit_Main(menuSession, itemSession.ItemIndexBase + 5);
                    break;
            }

            return Task.CompletedTask;
        }
    }
}
