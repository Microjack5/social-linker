using Discord.WebSocket;
using SocialLinker.Core.Menus.MakerMulti.Main;
using System.Linq;
using System.Threading.Tasks;

namespace SocialLinker.Core.Menus.MakerMulti.Reactions
{
    class MakerMulti_Speaker_Select_Reactions
    {
        public static Task Nav_MakerMulti_Speaker_Select_Main(SocketReaction reaction, MenuIdStructure menuSession)
        {
            if (reaction.Emote.Name == "↩️")
            {
                var multimaker_session = Global.MultiMaker_Session_List.SingleOrDefault(x => x.User.Id == menuSession.User.Id);
                
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                if (multimaker_session.MakerMultiCommand.Expected_Characters == 2)
                {
                    _ = MakerMulti_Char_Select_2_Menu.MakerMulti_Character_Select_2_Main(menuSession.User, menuSession.MenuMessage);
                }
                else if (multimaker_session.MakerMultiCommand.Expected_Characters == 3)
                {
                    _ = MakerMulti_Char_Select_3_Menu.MakerMulti_Character_Select_3_Main(menuSession.User, menuSession.MenuMessage);
                }
                else if (multimaker_session.MakerMultiCommand.Expected_Characters == 4)
                {
                    _ = MakerMulti_Char_Select_4_Menu.MakerMulti_Character_Select_4_Main(menuSession.User, menuSession.MenuMessage);
                }

                return Task.CompletedTask;
            }

            else if (reaction.Emote.Name == "\u0031\ufe0f\u20e3")
            {
                var multimaker_session = Global.MultiMaker_Session_List.SingleOrDefault(x => x.User.Id == menuSession.User.Id);
                multimaker_session.MakerMultiCommand.Display_Name = multimaker_session.MakerMultiCommand.Character_Data_1.Bustup_Data.Default_Name_EN;

                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = MakerMulti_Dialogue_Select_Menu.MakerMulti_Dialogue_Select_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            else if (reaction.Emote.Name == "\u0032\ufe0f\u20e3")
            {
                var multimaker_session = Global.MultiMaker_Session_List.SingleOrDefault(x => x.User.Id == menuSession.User.Id);
                multimaker_session.MakerMultiCommand.Display_Name = multimaker_session.MakerMultiCommand.Character_Data_2.Bustup_Data.Default_Name_EN;

                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = MakerMulti_Dialogue_Select_Menu.MakerMulti_Dialogue_Select_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            else if (reaction.Emote.Name == "\u0033\ufe0f\u20e3")
            {
                var multimaker_session = Global.MultiMaker_Session_List.SingleOrDefault(x => x.User.Id == menuSession.User.Id);
                multimaker_session.MakerMultiCommand.Display_Name = multimaker_session.MakerMultiCommand.Character_Data_3.Bustup_Data.Default_Name_EN;

                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = MakerMulti_Dialogue_Select_Menu.MakerMulti_Dialogue_Select_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            else if (reaction.Emote.Name == "\u0034\ufe0f\u20e3")
            {
                var multimaker_session = Global.MultiMaker_Session_List.SingleOrDefault(x => x.User.Id == menuSession.User.Id);
                multimaker_session.MakerMultiCommand.Display_Name = multimaker_session.MakerMultiCommand.Character_Data_4.Bustup_Data.Default_Name_EN;

                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = MakerMulti_Dialogue_Select_Menu.MakerMulti_Dialogue_Select_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        // Methods that activate on the MessageReceived event.
        public static Task Nav_MakerMulti_Speaker_Select_Main_Received(SocketMessage message, MenuIdStructure menuSession)
        {
            var multimaker_session = Global.MultiMaker_Session_List.SingleOrDefault(x => x.User.Id == menuSession.User.Id);

            string input_string = message.Content;
            input_string = Global.RemoveBotMention(input_string).Trim();

            // Check string length here later

            multimaker_session.MakerMultiCommand.Display_Name = input_string;

            // Stop the timeout timer associated with the menu.
            menuSession.MenuTimer.Stop();

            // Go to a new menu.
            _ = MakerMulti_Dialogue_Select_Menu.MakerMulti_Dialogue_Select_Main(menuSession.User, menuSession.MenuMessage);
            return Task.CompletedTask;
        }
    }
}
