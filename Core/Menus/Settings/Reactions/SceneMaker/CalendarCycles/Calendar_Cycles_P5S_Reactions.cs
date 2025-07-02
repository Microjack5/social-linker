using Discord.WebSocket;
using SocialLinker.Core.Menus.Settings.Main.SceneMaker.CalendarCycles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialLinker.Core.Menus.Settings.Reactions.SceneMaker.CalendarCycles
{
    class Calendar_Cycles_P5S_Reactions
    {
        public static Task Nav_Calendar_Cycles_PS5_Reactions(SocketReaction reaction, MenuIdStructure menuSession)
        {
            if (reaction.Emote.Name == "↩️")
            {
                menuSession.MenuTimer.Stop();

                _ = Calendar_Cycles_Menu.Calendar_Cycles_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }
            else if (reaction.Emote.Name == "🗓️")
            {
                menuSession.MenuTimer.Stop();

                _ = Calendar_Cycles_P5S_Menu.Calendar_Cycles_P5S_Month_Day(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }
            else if (reaction.Emote.Name == "🕗")
            {
                menuSession.MenuTimer.Stop();

                _ = Calendar_Cycles_P5S_Menu.Calendar_Cycles_P5S_Day_Of_Week(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }
            else if (reaction.Emote.Name == "⛅")
            {
                menuSession.MenuTimer.Stop();

                _ = Calendar_Cycles_P5S_Menu.Calendar_Cycles_P5S_TOD(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }
            else if (reaction.Emote.Name == "🔄")
            {
                menuSession.MenuTimer.Stop();

                //_ = Template_Layout_VC_Menu.Template_Layout_VC_P5_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }
    }
}
