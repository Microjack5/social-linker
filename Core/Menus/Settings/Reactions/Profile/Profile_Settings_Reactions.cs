using System.Threading.Tasks;
using Discord.WebSocket;
using SocialLinker.Core.Menus.Settings.Main;
using SocialLinker.Core.Menus.Settings.Main.Profile;

namespace SocialLinker.Core.Menus.Settings.Reactions.Profile
{
    class Profile_Settings_Reactions
    {
        public static Task Nav_Profile_Settings_Main(SocketReaction reaction, MenuIdStructure menuSession)
        {
            if (reaction.Emote.Name == "↩️")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Settings_Menu.Settings_Main_Menu(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            // Keycap One
            else if (reaction.Emote.Name == "\u0031\ufe0f\u20e3")
            {
                // Stop the timeout timer associated with the menu
                menuSession.MenuTimer.Stop();

                // Go to a new menu
                _ = Profile_Theme_Menu.Profile_Theme_Main(menuSession.User, menuSession.MenuMessage);

                return Task.CompletedTask;
            }

            // Keycap Two
            else if (reaction.Emote.Name == "\u0032\ufe0f\u20e3")
            {
                // Stop the timeout timer associated with the menu
                menuSession.MenuTimer.Stop();

                // Go to a new menu
                _ = Status_Decor_Menu.Status_Decor_Start(menuSession.User, menuSession.MenuMessage);

                return Task.CompletedTask;
            }

            else if (reaction.Emote.Name == "⭐" || reaction.Emote.Name == "🌟" || reaction.Emote.Name == "✨")
            {
                // Stop the timeout timer associated with the menu
                menuSession.MenuTimer.Stop();

                // Go to a new menu
                _ = Star_Level_Menu.Star_Level_Main(menuSession.User, menuSession.MenuMessage);

                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }
    }
}
