using Discord.WebSocket;
using SocialLinker.Core.CloudStorageTables;
using SocialLinker.Core.Menus.MakerMulti.Main;
using SocialLinker.Core.SceneMaker.Data.Bustup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialLinker.Core.Menus.MakerMulti.Reactions
{
    class MakerMulti_Sprite_Select_4_Reactions
    {
        public static Task Nav_MakerMulti_Sprite_Select_4_Main(SocketReaction reaction, MenuIdStructure menuSession)
        {
            if (reaction.Emote.Name == "↩️")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = MakerMulti_Layout_Select_Menu.MakerMulti_Layout_Select_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_MakerMulti_Sprite_Select_4_Invalid_Base_Sprite(SocketReaction reaction, MenuIdStructure menuSession)
        {
            if (reaction.Emote.Name == "↩️")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = MakerMulti_Sprite_Select_4_Menu.MakerMulti_Sprite_Select_4_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_MakerMulti_Sprite_Select_4_Error_Too_Many_Animation_Frames(SocketReaction reaction, MenuIdStructure menuSession)
        {
            if (reaction.Emote.Name == "↩️")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = MakerMulti_Sprite_Select_4_Menu.MakerMulti_Sprite_Select_4_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_MakerMulti_Sprite_Select_4_Error_Non_Digit_In_Sprite_Number(SocketReaction reaction, MenuIdStructure menuSession)
        {
            if (reaction.Emote.Name == "↩️")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = MakerMulti_Sprite_Select_4_Menu.MakerMulti_Sprite_Select_4_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_MakerMulti_Sprite_Select_4_Error_Animation_Frame_With_Blank_Sprite(SocketReaction reaction, MenuIdStructure menuSession)
        {
            if (reaction.Emote.Name == "↩️")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = MakerMulti_Sprite_Select_4_Menu.MakerMulti_Sprite_Select_4_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_MakerMulti_Sprite_Select_4_Error_Eye_Frame_Not_Found(SocketReaction reaction, MenuIdStructure menuSession)
        {
            if (reaction.Emote.Name == "↩️")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = MakerMulti_Sprite_Select_4_Menu.MakerMulti_Sprite_Select_4_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_MakerMulti_Sprite_Select_4_Error_Mouth_Frame_Not_Found(SocketReaction reaction, MenuIdStructure menuSession)
        {
            if (reaction.Emote.Name == "↩️")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = MakerMulti_Sprite_Select_4_Menu.MakerMulti_Sprite_Select_4_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        // Methods that activate on the MessageReceived event.
        public static Task Nav_MakerMulti_Sprite_Select_4_Main_Received(SocketMessage message, MenuIdStructure menuSession)
        {
            var multimaker_session = Global.MultiMaker_Session_List.SingleOrDefault(x => x.User.Id == menuSession.User.Id);

            var account = UserInfoClasses.GetAccount(message.Author);
            string input_string = message.Content;
            input_string = Global.RemoveBotMention(input_string).Trim();

            string result = MakerMulti_Sprite_Select_1_Reactions.Sprite_Number_Parser(input_string, multimaker_session, 4);

            switch (result)
            {
                case "Success":
                    break;

                case "Too_Many_Animation_Frames":
                    // Stop the timeout timer associated with the menu.
                    menuSession.MenuTimer.Stop();

                    // Go to a new menu.
                    _ = MakerMulti_Sprite_Select_4_Menu.MakerMulti_Sprite_Select_4_Error_Too_Many_Animation_Frames(menuSession.User, menuSession.MenuMessage);
                    return Task.CompletedTask;

                case "Non_Digit_In_Sprite_Number":
                    // Stop the timeout timer associated with the menu.
                    menuSession.MenuTimer.Stop();

                    // Go to a new menu.
                    _ = MakerMulti_Sprite_Select_4_Menu.MakerMulti_Sprite_Select_4_Error_Non_Digit_In_Sprite_Number(menuSession.User, menuSession.MenuMessage);
                    return Task.CompletedTask;
            }

            if (MakerMulti_Sprite_Select_1_Reactions.Base_Sprite_Validity_Check(multimaker_session.MakerCommand.Character_Data_4) == false)
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = MakerMulti_Sprite_Select_4_Menu.MakerMulti_Sprite_Select_4_Invalid_Base_Sprite(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }
            if ((multimaker_session.MakerCommand.Character_Data_4.Base_Sprite == 0) && ((multimaker_session.MakerCommand.Character_Data_4.Eye_Frame != default) || (multimaker_session.MakerCommand.Character_Data_4.Mouth_Frame != default)))
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = MakerMulti_Sprite_Select_4_Menu.MakerMulti_Sprite_Select_4_Error_Animation_Frame_With_Blank_Sprite(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            if (multimaker_session.MakerCommand.Character_Data_4.Base_Sprite != 0)
            {
                var bustup = MakerMulti_Sprite_Select_1_Reactions.Bustup_Selection(menuSession, account, multimaker_session.MakerCommand.Character_Data_4, 4); // Just a validity check

                if (bustup == null) // In case the validity check fails
                {
                    return Task.CompletedTask;
                }

                multimaker_session.MakerCommand.Character_Data_4.Bustup_Data = BustupDataMethods.Get_Bustup_Data(account, multimaker_session.MakerCommand.Character_Data_4.Set_Data, multimaker_session.MakerCommand.Character_Data_4);
            }

            menuSession.MenuTimer.Stop();

            _ = MakerMulti_Speaker_Select_Menu.MakerMulti_Speaker_Select_Main(menuSession.User, menuSession.MenuMessage);

            return Task.CompletedTask;
        }
    }
}
