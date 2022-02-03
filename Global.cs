using System.Collections.Generic;
using SocialLinker.Cooldown;
using SocialLinker.Core.Menus;
using SocialLinker.Core.SceneMaker.TemplateRenders.QuickScenes;

namespace SocialLinker
{
    internal static class Global
    {
        internal static List<MenuIdStructure> MenuIdList { get; set; } = new List<MenuIdStructure>();
        internal static List<ItemListIterator> ItemIdList { get; set; } = new List<ItemListIterator>();
        internal static List<UserCooldownFields> CooldownList { get; set; } = new List<UserCooldownFields>();
        internal static List<ContentFilter> ContentFilterList { get; set; } = new List<ContentFilter>();
        internal static List<ContextSwitchData> P1_PS1_Usage_List { get; set; } = new List<ContextSwitchData>();
    }
}
