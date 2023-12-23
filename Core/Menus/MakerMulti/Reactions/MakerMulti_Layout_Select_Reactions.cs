using Discord.WebSocket;
using SocialLinker.Core.Menus.MakerMulti.Main;
using System.Linq;
using System.Threading.Tasks;

namespace SocialLinker.Core.Menus.MakerMulti.Reactions
{
    class MakerMulti_Layout_Select_Reactions
    {
        public static Task Nav_MakerMulti_Layout_Select_Main(SocketReaction reaction, MenuIdStructure menuSession)
        {
            if (reaction.Emote.Name == "↩️")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = MakerMulti_Title_Select_Menu.MakerMulti_Main_Menu(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            else if (reaction.Emote.Name == "\u0031\ufe0f\u20e3")
            {
                var multimaker_session = Global.MultiMaker_Session_List.SingleOrDefault(x => x.User.Id == menuSession.User.Id);
                multimaker_session.MakerCommand.Expected_Characters = 2;

                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = MakerMulti_Char_Select_1_Menu.MakerMulti_Character_Select_1_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            else if (reaction.Emote.Name == "\u0032\ufe0f\u20e3")
            {
                var multimaker_session = Global.MultiMaker_Session_List.SingleOrDefault(x => x.User.Id == menuSession.User.Id);
                multimaker_session.MakerCommand.Expected_Characters = 3;

                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = MakerMulti_Char_Select_1_Menu.MakerMulti_Character_Select_1_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            else if (reaction.Emote.Name == "\u0033\ufe0f\u20e3")
            {
                var multimaker_session = Global.MultiMaker_Session_List.SingleOrDefault(x => x.User.Id == menuSession.User.Id);
                multimaker_session.MakerCommand.Expected_Characters = 4;

                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = MakerMulti_Char_Select_1_Menu.MakerMulti_Character_Select_1_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }
    }
}
