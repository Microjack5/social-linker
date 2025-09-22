using Discord.WebSocket;
using SocialLinker.Core.Menus.Settings.Main.SceneMaker;
using SocialLinker.Core.Menus.Settings.Main.SceneMaker.TemplateLayout;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace SocialLinker.Core.Menus.Settings.Reactions.SceneMaker.TemplateLayout
{
    class Template_Layout_VC_Reactions
    {
        public static Task Nav_Template_Layout_VC_P1_Main(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            string selected = component.Data.Values.First();

            switch (selected)
            {
                case "P1-PS1":
                    _ = Template_Layout_P1_PS1_Menu.Template_Layout_P1_PS1_Main(menuSession);
                    break;
                case "P1-PSP":
                    _ = Template_Layout_P1_PSP_Menu.Template_Layout_P1_PSP_Main(menuSession);
                    break;
                case "return":
                    _ = Template_Layout_Menu.Template_Layout_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_VC_P2IS_Main(SocketReaction reaction, MenuIdStructure menuSession)
        {
            if (reaction.Emote.Name == "↩️")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Template_Layout_Menu.Template_Layout_Main(menuSession);
                return Task.CompletedTask;
            }

            // Keycap One
            else if (reaction.Emote.Name == "\u0031\ufe0f\u20e3")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Template_Layout_P2IS_PS1_Menu.Template_Layout_P2IS_PS1_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            // Keycap Two
            else if (reaction.Emote.Name == "\u0032\ufe0f\u20e3")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Template_Layout_P2IS_PSP_Menu.Template_Layout_P2IS_PSP_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_VC_P2EP_Main(SocketReaction reaction, MenuIdStructure menuSession)
        {
            if (reaction.Emote.Name == "↩️")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Template_Layout_Menu.Template_Layout_Main(menuSession);
                return Task.CompletedTask;
            }

            // Keycap One
            else if (reaction.Emote.Name == "\u0031\ufe0f\u20e3")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Template_Layout_P2EP_PS1_Menu.Template_Layout_P2EP_PS1_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            // Keycap Two
            else if (reaction.Emote.Name == "\u0032\ufe0f\u20e3")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Template_Layout_P2EP_PSP_Menu.Template_Layout_P2EP_PSP_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_VC_P3_Main(SocketReaction reaction, MenuIdStructure menuSession)
        {
            if (reaction.Emote.Name == "↩️")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Template_Layout_Menu.Template_Layout_Main(menuSession);
                return Task.CompletedTask;
            }

            // Keycap One
            else if (reaction.Emote.Name == "\u0031\ufe0f\u20e3")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Template_Layout_P3F_Menu.Template_Layout_P3F_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            // Keycap Two
            else if (reaction.Emote.Name == "\u0032\ufe0f\u20e3")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Template_Layout_P3P_Menu.Template_Layout_P3P_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_VC_P4_Main(SocketReaction reaction, MenuIdStructure menuSession)
        {
            if (reaction.Emote.Name == "↩️")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Template_Layout_Menu.Template_Layout_Main(menuSession);
                return Task.CompletedTask;
            }

            // Keycap One
            else if (reaction.Emote.Name == "\u0031\ufe0f\u20e3")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Template_Layout_P4_PS2_Menu.Template_Layout_P4_PS2_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            // Keycap Two
            else if (reaction.Emote.Name == "\u0032\ufe0f\u20e3")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Template_Layout_P4G_Menu.Template_Layout_P4G_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_VC_P5_Main(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            string selected = component.Data.Values.First();

            switch (selected)
            {
                case "P5-PS4":
                    _ = Template_Layout_P5_PS3_Menu.Template_Layout_P5_PS3_Main(menuSession);
                    break;
                case "P5R":
                    _ = Template_Layout_P5R_Menu.Template_Layout_P5R_Main(menuSession);
                    break;
                case "return":
                    _ = Template_Layout_Menu.Template_Layout_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }
    }
}
