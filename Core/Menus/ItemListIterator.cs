using System.Collections.Generic;
using Discord.WebSocket;
using SocialLinker.Core.CloudStorageTables;

namespace SocialLinker.Core.Menus
{
    public class ItemListIterator
    {
        public SocketGuildUser User { get; set; }
        public List<string> ItemList { get; set; }
        public List<DisplayNameTableData> DisplayNameItemList { get; set; } // Replace with generic when possible
        public int ItemIndexBase { get; set; }
        public int MaxItemsDisplayed { get; set; }
        public int CurrentPage { get; set; }
        public string SelectedItem { get; set; }
        public DisplayNameTableData SelectedDisplayName { get; set; }
        public int CurrentMenuItem { get; set; }
    }
}
