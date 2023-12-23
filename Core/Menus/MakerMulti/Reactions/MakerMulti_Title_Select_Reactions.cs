using Discord.WebSocket;
using SocialLinker.Core.Menus.MakerMulti.Main;
using SocialLinker.Core.Menus.Settings.Main.Profile;
using SocialLinker.Core.Menus.Settings.Main.SceneMaker;
using SocialLinker.Core.Menus.Settings.Main.SceneMaker.TemplateLayout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialLinker.Core.Menus.MakerMulti.Reactions
{
    class MakerMulti_Title_Select_Reactions
    {
        public static Task Nav_MakerMulti_Main_Menu(SocketReaction reaction, MenuIdStructure menuSession)
        {
            if (reaction.Emote.Name == "P2IS")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = MakerMulti_Title_Select_Menu.MakerMulti_VC_P2IS_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            else if (reaction.Emote.Name == "P2EP")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = MakerMulti_Title_Select_Menu.MakerMulti_VC_P2EP_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            else if (reaction.Emote.Name == "P3P")
            {
                var multimaker_session = Global.MultiMaker_Session_List.SingleOrDefault(x => x.User.Id == menuSession.User.Id);

                multimaker_session.MakerCommand.Template = "P3P";
                multimaker_session.MakerCommand.Expected_Characters = 2;

                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = MakerMulti_Char_Select_1_Menu.MakerMulti_Character_Select_1_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            else if (reaction.Emote.Name == "P4AU")
            {
                var multimaker_session = Global.MultiMaker_Session_List.SingleOrDefault(x => x.User.Id == menuSession.User.Id);

                multimaker_session.MakerCommand.Template = "P4AU";
                multimaker_session.MakerCommand.Expected_Characters = 2;

                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = MakerMulti_Char_Select_1_Menu.MakerMulti_Character_Select_1_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            else if (reaction.Emote.Name == "P4D")
            {
                var multimaker_session = Global.MultiMaker_Session_List.SingleOrDefault(x => x.User.Id == menuSession.User.Id);

                multimaker_session.MakerCommand.Template = "P4D";
                multimaker_session.MakerCommand.Expected_Characters = 2;

                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = MakerMulti_Layout_Select_Menu.MakerMulti_Layout_Select_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            else if (reaction.Emote.Name == "BBTAG")
            {
                var multimaker_session = Global.MultiMaker_Session_List.SingleOrDefault(x => x.User.Id == menuSession.User.Id);
                multimaker_session.MakerCommand.Template = "BBTAG";

                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = MakerMulti_Layout_Select_Menu.MakerMulti_Layout_Select_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            else if (reaction.Emote.Name == "❌")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Attempt to delete the menu message from the channel if it hasn't been deleted by the user yet. If this fails, catch the exception.
                try
                {
                    _ = menuSession.MenuMessage.DeleteAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                // If the menu session is not null, remove it from the global list.
                if (menuSession != null)
                {
                    Global.MenuIdList.Remove(menuSession);
                }
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_MakerMulti_VC_P2IS_Main(SocketReaction reaction, MenuIdStructure menuSession)
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
                multimaker_session.MakerCommand.Template = "P2IS-PS1";

                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = _ = MakerMulti_Layout_Select_Menu.MakerMulti_Layout_Select_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            else if (reaction.Emote.Name == "\u0032\ufe0f\u20e3")
            {
                var multimaker_session = Global.MultiMaker_Session_List.SingleOrDefault(x => x.User.Id == menuSession.User.Id);
                multimaker_session.MakerCommand.Template = "P2IS-PSP";

                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = _ = MakerMulti_Layout_Select_Menu.MakerMulti_Layout_Select_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_MakerMulti_VC_P2EP_Main(SocketReaction reaction, MenuIdStructure menuSession)
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
                multimaker_session.MakerCommand.Template = "P2EP-PS1";

                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = _ = MakerMulti_Layout_Select_Menu.MakerMulti_Layout_Select_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            else if (reaction.Emote.Name == "\u0032\ufe0f\u20e3")
            {
                var multimaker_session = Global.MultiMaker_Session_List.SingleOrDefault(x => x.User.Id == menuSession.User.Id);
                multimaker_session.MakerCommand.Template = "P2EP-PSP";

                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = _ = MakerMulti_Layout_Select_Menu.MakerMulti_Layout_Select_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }
    }
}
