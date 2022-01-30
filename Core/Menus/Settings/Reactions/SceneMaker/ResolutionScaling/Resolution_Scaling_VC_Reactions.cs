using System.Threading.Tasks;
using Discord.WebSocket;
using SocialLinker.Core.Menus.Settings.Main.SceneMaker.ResolutionScaling;

namespace SocialLinker.Core.Menus.Settings.Reactions.SceneMaker.ResolutionScaling
{
    class Resolution_Scaling_VC_Reactions
    {
        public static Task Nav_Resolution_Scaling_VC_P1_Main(SocketReaction reaction, MenuIdStructure menuSession)
        {
            if (reaction.Emote.Name == "↩️")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Resolution_Scaling_Menu.Resolution_Scaling_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            // Keycap One
            else if (reaction.Emote.Name == "\u0031\ufe0f\u20e3")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Resolution_Scaling_P1_PS1_Menu.Resolution_Scaling_P1_PS1_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            // Keycap Two
            else if (reaction.Emote.Name == "\u0032\ufe0f\u20e3")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Resolution_Scaling_P1_PSP_Menu.Resolution_Scaling_P1_PSP_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Resolution_Scaling_VC_P2IS_Main(SocketReaction reaction, MenuIdStructure menuSession)
        {
            if (reaction.Emote.Name == "↩️")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Resolution_Scaling_Menu.Resolution_Scaling_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            // Keycap One
            else if (reaction.Emote.Name == "\u0031\ufe0f\u20e3")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Resolution_Scaling_P2IS_PS1_Menu.Resolution_Scaling_P2IS_PS1_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            // Keycap Two
            else if (reaction.Emote.Name == "\u0032\ufe0f\u20e3")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Resolution_Scaling_P2IS_PSP_Menu.Resolution_Scaling_P2IS_PSP_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Resolution_Scaling_VC_P2EP_Main(SocketReaction reaction, MenuIdStructure menuSession)
        {
            if (reaction.Emote.Name == "↩️")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Resolution_Scaling_Menu.Resolution_Scaling_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            // Keycap One
            else if (reaction.Emote.Name == "\u0031\ufe0f\u20e3")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Resolution_Scaling_P2EP_PS1_Menu.Resolution_Scaling_P2EP_PS1_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            // Keycap Two
            else if (reaction.Emote.Name == "\u0032\ufe0f\u20e3")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Resolution_Scaling_P2EP_PSP_Menu.Resolution_Scaling_P2EP_PSP_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Resolution_Scaling_VC_P3_Main(SocketReaction reaction, MenuIdStructure menuSession)
        {
            if (reaction.Emote.Name == "↩️")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Resolution_Scaling_Menu.Resolution_Scaling_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            // Keycap One
            else if (reaction.Emote.Name == "\u0031\ufe0f\u20e3")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Resolution_Scaling_P3F_Menu.Resolution_Scaling_P3F_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            // Keycap Two
            else if (reaction.Emote.Name == "\u0032\ufe0f\u20e3")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Resolution_Scaling_P3P_Menu.Resolution_Scaling_P3P_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }
    }
}
