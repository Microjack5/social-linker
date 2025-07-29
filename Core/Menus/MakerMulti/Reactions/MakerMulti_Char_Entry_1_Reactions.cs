using Discord;
using Discord.WebSocket;
using SocialLinker.Core.Menus.MakerMulti.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialLinker.Core.Menus.MakerMulti.Reactions
{
    class MakerMulti_Char_Entry_1_Reactions
    {
        public static Task Nav_MakerMulti_Char_Entry_1_Main(SocketMessageComponent component)
        {
            if (component.Data.CustomId == "makermulti-char-entry-1-modal-open")
            {
                // Go to a new menu.
                _ = MakerMulti_Char_Entry_1_Menu.MakerMulti_Char_Details_Modal(component);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }
    }
}
