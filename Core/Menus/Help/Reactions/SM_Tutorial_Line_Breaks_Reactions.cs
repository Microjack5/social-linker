using Discord.WebSocket;
using SocialLinker.Core.Menus.Help.Main;
using System.Threading.Tasks;

namespace SocialLinker.Core.Menus.Help.Reactions
{
    class SM_Tutorial_Line_Breaks_Reactions
    {
        public static Task Nav_SM_Tutorial_Line_Breaks_Page_1(SocketReaction reaction, MenuIdStructure menuSession)
        {
            if (reaction.Emote.Name == "↩️")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = SM_Tutorial_Select_Menu.SM_Tutorial_Select_Advanced(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }
    }
}
