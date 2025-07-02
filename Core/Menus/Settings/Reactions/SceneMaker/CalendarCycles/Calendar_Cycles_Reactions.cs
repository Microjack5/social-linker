using Discord.WebSocket;
using SocialLinker.Core.Menus.Settings.Main.Profile;
using SocialLinker.Core.Menus.Settings.Main.SceneMaker.TemplateLayout;
using SocialLinker.Core.Menus.Settings.Main.SceneMaker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocialLinker.Core.Menus.Settings.Main.SceneMaker.CalendarCycles;

namespace SocialLinker.Core.Menus.Settings.Reactions.SceneMaker.CalendarCycles
{
    class Calendar_Cycles_Reactions
    {
        public static Task Nav_Calendar_Cycles_Main(SocketReaction reaction, MenuIdStructure menuSession)
        {
            if (reaction.Emote.Name == "↩️")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                //_ = SM_Settings_Menu.SM_Settings_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }
            else if (reaction.Emote.Name == "P1")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                //_ = Template_Layout_VC_Menu.Template_Layout_VC_P1_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }
            else if (reaction.Emote.Name == "P3")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                //_ = Template_Layout_VC_Menu.Template_Layout_VC_P3_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }
            else if (reaction.Emote.Name == "P4")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                //_ = Template_Layout_VC_Menu.Template_Layout_VC_P4_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }
            else if (reaction.Emote.Name == "P5")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                //_ = Template_Layout_VC_Menu.Template_Layout_VC_P5_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }
            else if (reaction.Emote.Name == "P5S")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Calendar_Cycles_P5S_Menu.Calendar_Cycles_P5S_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }
    }
}
