using Discord.WebSocket;
using SocialLinker.Core.Menus.MakerMulti.Main;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SocialLinker.Core.Menus.MakerMulti.Reactions
{
    class MakerMulti_Dialogue_Select_Reactions
    {
        public static Task Nav_MakerMulti_Dialogue_Select_Main(SocketReaction reaction, MenuIdStructure menuSession)
        {
            if (reaction.Emote.Name == "↩️")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = MakerMulti_Speaker_Select_Menu.MakerMulti_Speaker_Select_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        // Methods that activate on the MessageReceived event.
        public static Task Nav_MakerMulti_Dialogue_Select_Main_Received(SocketMessage message, MenuIdStructure menuSession)
        {
            try
            {
                var multimaker_session = Global.MultiMaker_Session_List.SingleOrDefault(x => x.User.Id == menuSession.User.Id);

                string input_string = message.Content;
                input_string = Global.RemoveBotMention(input_string).Trim();

                multimaker_session.MakerMultiCommand.Dialogue = input_string;

                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = MakerMulti_Confirm_Details_Menu.MakerMulti_Confirm_Details_Main(menuSession.User, menuSession.MenuMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return Task.CompletedTask;
        }
    }
}
