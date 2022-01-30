using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Rest;
using Discord.WebSocket;
using SocialLinker.Cooldown;
using SocialLinker.Core.Menus;

namespace SocialLinker
{
    internal static class Global
    {
        internal static List<MenuIdStructure> MenuIdList { get; set; } = new List<MenuIdStructure>();
        internal static List<ItemListIterator> ItemIdList { get; set; } = new List<ItemListIterator>();
        internal static List<UserCooldownFields> CooldownList { get; set; } = new List<UserCooldownFields>();
        internal static List<ContentFilter> ContentFilterList { get; set; } = new List<ContentFilter>();
    }
}
