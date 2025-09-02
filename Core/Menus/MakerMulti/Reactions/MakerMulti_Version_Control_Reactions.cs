using Discord.WebSocket;
using SocialLinker.Core.Menus.MakerMulti.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialLinker.Core.Menus.MakerMulti.Reactions
{
    class MakerMulti_Version_Control_Reactions
    {
        public static Task Nav_MakerMulti_VC_P2IS_Main(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            var multimaker_session = Global.MultiMaker_Session_List.SingleOrDefault(x => x.User.Id == menuSession.User.Id);
            string selected = component.Data.Values.First();

            switch (selected)
            {
                case "P2IS-PS1":
                    multimaker_session.MakerCommand.Template = "P2IS-PS1";
                    break;

                case "P2IS-PSP":
                    multimaker_session.MakerCommand.Template = "P2IS-PSP";
                    break;
            }

            menuSession.MenuTimer.Stop();
            _ = MakerMulti_Char_Details_Pt1_Menu.MakerMulti_Char_Details_Pt1_Main(menuSession);

            return Task.CompletedTask;
        }

        public static Task Nav_MakerMulti_VC_P2EP_Main(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            var multimaker_session = Global.MultiMaker_Session_List.SingleOrDefault(x => x.User.Id == menuSession.User.Id);
            string selected = component.Data.Values.First();

            switch (selected)
            {
                case "P2EP-PS1":
                    multimaker_session.MakerCommand.Template = "P2EP-PS1";
                    break;

                case "P2EP-PSP":
                    multimaker_session.MakerCommand.Template = "P2EP-PSP";
                    break;
            }

            menuSession.MenuTimer.Stop();
            _ = MakerMulti_Char_Details_Pt1_Menu.MakerMulti_Char_Details_Pt1_Main(menuSession);

            return Task.CompletedTask;
        }
    }
}
