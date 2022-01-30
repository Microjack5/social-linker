using System.Threading.Tasks;
using Discord.WebSocket;
using SocialLinker.Core.Menus.Help.Main;


namespace SocialLinker.Core.Menus.Help.Reactions
{
    class Legal_Notices_Reactions
    {
        public static Task Nav_Legal_Notices_Main(SocketReaction reaction, MenuIdStructure menuSession)
        {
            if (reaction.Emote.Name == "↩️")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Help_Menu.Help_Main_Menu(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }
    }
}
