using Discord.WebSocket;
using SocialLinker.Core.Menus.MakerMulti.Main;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace SocialLinker.Core.Menus.MakerMulti.Reactions
{
    class MakerMulti_BBTAG_Layout_Reactions
    {
        public static Task Nav_MakerMulti_BBTAG_Layout_Main(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            try
            {
                string selected = component.Data.Values.First();

                var multimaker_session = Global.MultiMaker_Session_List.SingleOrDefault(x => x.User.Id == menuSession.User.Id);

                multimaker_session.MakerCommand.BBTAG_Specific_Data.Layout = selected;

                menuSession.MenuTimer.Stop();

                switch (selected)
                {
                    case "1":
                    case "2":
                    case "3":
                        multimaker_session.MakerCommand.Expected_Characters = 1;
                        break;
                    case "4":
                    case "5":
                    case "6":
                    case "7":
                        multimaker_session.MakerCommand.Expected_Characters = 2;
                        break;
                    case "8":
                    case "9":
                        multimaker_session.MakerCommand.Expected_Characters = 3;
                        break;
                    case "10":
                        multimaker_session.MakerCommand.Expected_Characters = 4;
                        break;
                }

                switch (selected)
                {
                    case "1":
                    case "2":
                    case "3":
                        _ = MakerMulti_Char_Details_Pt1_Menu.MakerMulti_1Char_Details_Main(menuSession);
                        break;

                    default:
                        _ = MakerMulti_Char_Details_Pt1_Menu.MakerMulti_2Char_Details_Main(menuSession);
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return Task.CompletedTask;
        }
    }
}
