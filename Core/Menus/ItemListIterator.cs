using System.Collections.Generic;
using Discord.WebSocket;

namespace SocialLinker.Core.Menus
{
    public class ItemListIterator
    {
        public SocketGuildUser User { get; set; }
        public List<string> ItemList { get; set; }
        public int ItemIndexBase { get; set; }
        public int MaxItemsDisplayed { get; set; }
        public int CurrentPage { get; set; }
        public string SelectedItem { get; set; }
    }
}
