using System.Threading.Tasks;
using Discord.WebSocket;
using SocialLinker.Core.Menus.Help.Main;

namespace SocialLinker.Core.Menus.Help.Reactions
{
    class SM_Tutorial_VC_Reactions
    {
        public static Task Nav_SM_Tutorial_VC_Main(SocketReaction reaction, MenuIdStructure menuSession)
        {
            if (reaction.Emote.Name == "↩️")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = SM_Tutorial_Select_Menu.SM_Tutorial_Select_Advanced(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            // Keycap One
            else if (reaction.Emote.Name == "\u0031\ufe0f\u20e3")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = SM_Tutorial_VC_Menu.SM_Tutorial_VC_Basic_Page_1(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            // Keycap Two
            else if (reaction.Emote.Name == "\u0032\ufe0f\u20e3")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = SM_Tutorial_VC_Menu.SM_Tutorial_VC_Auto_Page_1(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            // Keycap Three
            else if (reaction.Emote.Name == "\u0033\ufe0f\u20e3")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = SM_Tutorial_VC_Menu.SM_Tutorial_VC_Cross_Page_1(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_SM_Tutorial_VC_Basic_Page_1(SocketReaction reaction, MenuIdStructure menuSession)
        {
            if (reaction.Emote.Name == "↩️")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = SM_Tutorial_VC_Menu.SM_Tutorial_VC_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            else if (reaction.Emote.Name == "▶️")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = SM_Tutorial_VC_Menu.SM_Tutorial_VC_Basic_Page_2(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_SM_Tutorial_VC_Basic_Page_2(SocketReaction reaction, MenuIdStructure menuSession)
        {
            if (reaction.Emote.Name == "◀️")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = SM_Tutorial_VC_Menu.SM_Tutorial_VC_Basic_Page_1(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            else if (reaction.Emote.Name == "💠")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = SM_Tutorial_VC_Menu.SM_Tutorial_VC_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_SM_Tutorial_VC_Auto_Page_1(SocketReaction reaction, MenuIdStructure menuSession)
        {
            if (reaction.Emote.Name == "↩️")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = SM_Tutorial_VC_Menu.SM_Tutorial_VC_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            else if (reaction.Emote.Name == "▶️")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = SM_Tutorial_VC_Menu.SM_Tutorial_VC_Auto_Page_2(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_SM_Tutorial_VC_Auto_Page_2(SocketReaction reaction, MenuIdStructure menuSession)
        {
            if (reaction.Emote.Name == "◀️")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = SM_Tutorial_VC_Menu.SM_Tutorial_VC_Auto_Page_1(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            else if (reaction.Emote.Name == "💠")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = SM_Tutorial_VC_Menu.SM_Tutorial_VC_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_SM_Tutorial_VC_Cross_Page_1(SocketReaction reaction, MenuIdStructure menuSession)
        {
            if (reaction.Emote.Name == "↩️")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = SM_Tutorial_VC_Menu.SM_Tutorial_VC_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            else if (reaction.Emote.Name == "▶️")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = SM_Tutorial_VC_Menu.SM_Tutorial_VC_Cross_Page_2(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_SM_Tutorial_VC_Cross_Page_2(SocketReaction reaction, MenuIdStructure menuSession)
        {
            if (reaction.Emote.Name == "◀️")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = SM_Tutorial_VC_Menu.SM_Tutorial_VC_Cross_Page_1(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            else if (reaction.Emote.Name == "▶️")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = SM_Tutorial_VC_Menu.SM_Tutorial_VC_Cross_Page_3(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_SM_Tutorial_VC_Cross_Page_3(SocketReaction reaction, MenuIdStructure menuSession)
        {
            if (reaction.Emote.Name == "◀️")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = SM_Tutorial_VC_Menu.SM_Tutorial_VC_Cross_Page_2(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            else if (reaction.Emote.Name == "💠")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = SM_Tutorial_VC_Menu.SM_Tutorial_VC_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }
    }
}
