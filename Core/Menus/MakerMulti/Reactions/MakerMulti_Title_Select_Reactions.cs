using Discord.WebSocket;
using SocialLinker.Core.Menus.MakerMulti.Main;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SocialLinker.Core.Menus.MakerMulti.Reactions
{
    class MakerMulti_Title_Select_Reactions
    {
        public static Task Nav_MakerMulti_Main_Menu(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            string selected = component.Data.Values.First();

            if (selected == "P2IS")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = MakerMulti_Version_Control_Menu.MakerMulti_VC_P2IS_Main(menuSession);
                return Task.CompletedTask;
            }

            else if (selected == "P2EP")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = MakerMulti_Version_Control_Menu.MakerMulti_VC_P2EP_Main(menuSession);
                return Task.CompletedTask;
            }

            else if (selected == "P3P")
            {
                var multimaker_session = Global.MultiMaker_Session_List.SingleOrDefault(x => x.User.Id == menuSession.User.Id);

                multimaker_session.MakerCommand.Template = "P3P";
                multimaker_session.MakerCommand.Expected_Characters = 2;

                menuSession.MenuTimer.Stop();

                _ = MakerMulti_Char_Details_Pt1_Menu.MakerMulti_Char_Details_Pt1_Main(menuSession);
                return Task.CompletedTask;
            }

            else if (selected == "P4AU")
            {
                var multimaker_session = Global.MultiMaker_Session_List.SingleOrDefault(x => x.User.Id == menuSession.User.Id);

                multimaker_session.MakerCommand.Template = "P4AU";
                multimaker_session.MakerCommand.Expected_Characters = 2;

                menuSession.MenuTimer.Stop();

                _ = MakerMulti_Char_Details_Pt1_Menu.MakerMulti_Char_Details_Pt1_Main(menuSession);
                return Task.CompletedTask;
            }

            else if (selected == "P4D")
            {
                var multimaker_session = Global.MultiMaker_Session_List.SingleOrDefault(x => x.User.Id == menuSession.User.Id);

                multimaker_session.MakerCommand.Template = "P4D";
                multimaker_session.MakerCommand.Expected_Characters = 2;

                menuSession.MenuTimer.Stop();

                _ = MakerMulti_Char_Details_Pt1_Menu.MakerMulti_Char_Details_Pt1_Main(menuSession);
                return Task.CompletedTask;
            }

            else if (selected == "BBTAG")
            {
                var multimaker_session = Global.MultiMaker_Session_List.SingleOrDefault(x => x.User.Id == menuSession.User.Id);
                multimaker_session.MakerCommand.Template = "BBTAG";

                menuSession.MenuTimer.Stop();

                _ = MakerMulti_BBTAG_Layout_Menu.MakerMulti_BBTAG_Layout_Main(menuSession);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }
    }
}
