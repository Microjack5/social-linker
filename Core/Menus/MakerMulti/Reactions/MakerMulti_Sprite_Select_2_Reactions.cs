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
    class MakerMulti_Sprite_Select_2_Reactions
    {
        public static Task Nav_MakerMulti_Sprite_Select_2_Main(SocketReaction reaction, MenuIdStructure menuSession)
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

        public static Task Nav_MakerMulti_Sprite_Select_2_Invalid_Base_Sprite(SocketReaction reaction, MenuIdStructure menuSession)
        {
            if (reaction.Emote.Name == "↩️")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = MakerMulti_Sprite_Select_2_Menu.MakerMulti_Sprite_Select_2_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_MakerMulti_Sprite_Select_2_Error_Too_Many_Animation_Frames(SocketReaction reaction, MenuIdStructure menuSession)
        {
            if (reaction.Emote.Name == "↩️")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = MakerMulti_Sprite_Select_2_Menu.MakerMulti_Sprite_Select_2_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_MakerMulti_Sprite_Select_2_Error_Non_Digit_In_Sprite_Number(SocketReaction reaction, MenuIdStructure menuSession)
        {
            if (reaction.Emote.Name == "↩️")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = MakerMulti_Sprite_Select_2_Menu.MakerMulti_Sprite_Select_2_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_MakerMulti_Sprite_Select_2_Error_Animation_Frame_With_Blank_Sprite(SocketReaction reaction, MenuIdStructure menuSession)
        {
            if (reaction.Emote.Name == "↩️")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = MakerMulti_Sprite_Select_2_Menu.MakerMulti_Sprite_Select_2_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_MakerMulti_Sprite_Select_2_Error_Eye_Frame_Not_Found(SocketReaction reaction, MenuIdStructure menuSession)
        {
            if (reaction.Emote.Name == "↩️")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = MakerMulti_Sprite_Select_2_Menu.MakerMulti_Sprite_Select_2_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_MakerMulti_Sprite_Select_2_Error_Mouth_Frame_Not_Found(SocketReaction reaction, MenuIdStructure menuSession)
        {
            if (reaction.Emote.Name == "↩️")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = MakerMulti_Sprite_Select_2_Menu.MakerMulti_Sprite_Select_2_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        // Methods that activate on the MessageReceived event.
        public static Task Nav_MakerMulti_Sprite_Select_2_Main_Received(SocketMessage message, MenuIdStructure menuSession)
        {
            try
            {
                var multimaker_session = Global.MultiMaker_Session_List.SingleOrDefault(x => x.User.Id == menuSession.User.Id);

                var account = UserInfoClasses.GetAccount(message.Author);
                string input_string = message.Content;
                input_string = Global.RemoveBotMention(input_string).Trim();

                string result = Utility.Sprite_Number_Parser(input_string, multimaker_session, 2);

                switch (result)
                {
                    case "Success":
                        break;

                    case "Too_Many_Animation_Frames":
                        // Stop the timeout timer associated with the menu.
                        menuSession.MenuTimer.Stop();

                        // Go to a new menu.
                        _ = MakerMulti_Sprite_Select_2_Menu.MakerMulti_Sprite_Select_2_Error_Too_Many_Animation_Frames(menuSession.User, menuSession.MenuMessage);
                        return Task.CompletedTask;

                    case "Non_Digit_In_Sprite_Number":
                        // Stop the timeout timer associated with the menu.
                        menuSession.MenuTimer.Stop();

                        // Go to a new menu.
                        _ = MakerMulti_Sprite_Select_2_Menu.MakerMulti_Sprite_Select_2_Error_Non_Digit_In_Sprite_Number(menuSession.User, menuSession.MenuMessage);
                        return Task.CompletedTask;
                }

                if (Utility.Base_Sprite_Validity_Check(multimaker_session.MakerCommand.Character_Data_2) == false)
                {
                    // Stop the timeout timer associated with the menu.
                    menuSession.MenuTimer.Stop();

                    // Go to a new menu.
                    _ = MakerMulti_Sprite_Select_2_Menu.MakerMulti_Sprite_Select_2_Invalid_Base_Sprite(menuSession.User, menuSession.MenuMessage);
                    return Task.CompletedTask;
                }
                if ((multimaker_session.MakerCommand.Character_Data_2.Base_Sprite == 0) && ((multimaker_session.MakerCommand.Character_Data_2.Eye_Frame != default) || (multimaker_session.MakerCommand.Character_Data_2.Mouth_Frame != default)))
                {
                    // Stop the timeout timer associated with the menu.
                    menuSession.MenuTimer.Stop();

                    // Go to a new menu.
                    _ = MakerMulti_Sprite_Select_2_Menu.MakerMulti_Sprite_Select_2_Error_Animation_Frame_With_Blank_Sprite(menuSession.User, menuSession.MenuMessage);
                    return Task.CompletedTask;
                }

                if (multimaker_session.MakerCommand.Character_Data_2.Base_Sprite != 0)
                {
                    var bustup = Utility.Bustup_Selection(menuSession, account, multimaker_session.MakerCommand.Character_Data_2, 2); // Just a validity check

                    if (bustup == null) // In case the validity check fails
                    {
                        return Task.CompletedTask;
                    }

                    multimaker_session.MakerCommand.Character_Data_2.Bustup_Data = BustupDataMethods.Get_Bustup_Data(account, multimaker_session.MakerCommand.Character_Data_2.Set_Data, multimaker_session.MakerCommand.Character_Data_2);
                }

                menuSession.MenuTimer.Stop();

                if (multimaker_session.MakerCommand.Expected_Characters > 2)
                {
                    _ = MakerMulti_Char_Select_3_Menu.MakerMulti_Character_Select_3_Main(menuSession.User, menuSession.MenuMessage);
                }
                else
                {
                    if ((multimaker_session.MakerCommand.Template == "P4AU" && account.P4AU_TS_Scene_Type == "Narration") ||
                        (multimaker_session.MakerCommand.Template == "P4D" && account.P4D_TS_Scene_Type == "Narration"))
                    {
                        _ = MakerMulti_Dialogue_Select_Menu.MakerMulti_Dialogue_Select_Main(menuSession.User, menuSession.MenuMessage);
                    }
                    else
                    {
                        _ = MakerMulti_Speaker_Select_Menu.MakerMulti_Speaker_Select_Main(menuSession.User, menuSession.MenuMessage);
                    }
                }

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return Task.CompletedTask;
        }
    }
}
