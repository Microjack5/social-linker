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
    class MakerMulti_Data_Entry_Reactions
    {
        public static Task Nav_MakerMulti_Data_Entry_Main(SocketMessageComponent component)
        {
            if (component.Data.CustomId == "multi-char-modal")
            {
                Console.WriteLine("YEEHAW 2!!!");
                // Go to a new menu.
                _ = MakerMulti_Data_Entry_Menu.MakerMulti_Data_Entry_Modal(component);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }
    }
}
