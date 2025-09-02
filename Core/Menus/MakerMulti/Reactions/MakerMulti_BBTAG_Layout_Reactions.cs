using Discord.WebSocket;
using SocialLinker.Core.Menus.MakerMulti.Main;
using System.Linq;
using System.Threading.Tasks;

namespace SocialLinker.Core.Menus.MakerMulti.Reactions
{
    class MakerMulti_BBTAG_Layout_Reactions
    {
        public static Task Nav_MakerMulti_BBTAG_Layout_Main(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            string selected = component.Data.Values.First();

            var multimaker_session = Global.MultiMaker_Session_List.SingleOrDefault(x => x.User.Id == menuSession.User.Id);

            multimaker_session.MakerCommand.Template = selected;

            switch (selected)
            {
                case "1":
                case "2":
                case "3":
                    _ = MakerMulti_Char_Details_Pt1_Menu.MakerMulti_BBTAG_1Char_Details_Main(menuSession);
                    break;

                default:
                    _ = MakerMulti_Char_Details_Pt1_Menu.MakerMulti_Char_Details_Pt1_Main(menuSession);
                    break;
            }

            menuSession.MenuTimer.Stop();

            _ = MakerMulti_Char_Details_Pt1_Menu.MakerMulti_Char_Details_Pt1_Main(menuSession);
            return Task.CompletedTask;
        }
    }
}
