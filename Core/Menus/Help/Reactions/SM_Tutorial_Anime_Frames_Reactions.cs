using System.Threading.Tasks;
using Discord.WebSocket;
using SocialLinker.Core.Menus.Help.Main;

namespace SocialLinker.Core.Menus.Help.Reactions
{
    class SM_Tutorial_Anime_Frames_Reactions
    {
        public static Task Nav_SM_Tutorial_Anime_Frames_Page_1(SocketReaction reaction, MenuIdStructure menuSession)
        {
            if (reaction.Emote.Name == "↩️")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = SM_Tutorial_Select_Menu.SM_Tutorial_Select_Advanced(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            else if (reaction.Emote.Name == "▶️")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = SM_Tutorial_Anime_Frames_Menu.SM_Tutorial_Anime_Frames_Page_2(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_SM_Tutorial_Anime_Frames_Page_2(SocketReaction reaction, MenuIdStructure menuSession)
        {
            if (reaction.Emote.Name == "◀️")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = SM_Tutorial_Anime_Frames_Menu.SM_Tutorial_Anime_Frames_Page_1(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            else if (reaction.Emote.Name == "▶️")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = SM_Tutorial_Anime_Frames_Menu.SM_Tutorial_Anime_Frames_Page_3(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_SM_Tutorial_Anime_Frames_Page_3(SocketReaction reaction, MenuIdStructure menuSession)
        {
            if (reaction.Emote.Name == "◀️")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = SM_Tutorial_Anime_Frames_Menu.SM_Tutorial_Anime_Frames_Page_2(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            else if (reaction.Emote.Name == "💠")
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
