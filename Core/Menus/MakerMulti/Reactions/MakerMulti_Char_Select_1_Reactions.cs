using Discord.WebSocket;
using SocialLinker.Core.CloudStorageTables;
using SocialLinker.Core.LocalStorageTables;
using SocialLinker.Core.Menus.MakerMulti.Main;
using SocialLinker.Core.Menus.Settings.Main.SceneMaker.DisplayNames;
using SocialLinker.Core.SceneMaker;
using SocialLinker.Core.SceneMaker.Data.Bustup;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using static SocialLinker.Core.SceneMaker.Moon;

namespace SocialLinker.Core.Menus.MakerMulti.Reactions
{
    class MakerMulti_Char_Select_1_Reactions
    {
        public static Task Nav_MakerMulti_Character_Select_1_Main(SocketReaction reaction, MenuIdStructure menuSession)
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

        public static Task Nav_MakerMulti_Character_Select_1_Invalid_Character(SocketReaction reaction, MenuIdStructure menuSession)
        {
            if (reaction.Emote.Name == "↩️")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = MakerMulti_Char_Select_1_Menu.MakerMulti_Character_Select_1_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_MakerMulti_Character_Select_1_Error_Sprite(SocketReaction reaction, MenuIdStructure menuSession)
        {
            if (reaction.Emote.Name == "↩️")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = MakerMulti_Char_Select_1_Menu.MakerMulti_Character_Select_1_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        // Methods that activate on the MessageReceived event.

        public static Task Nav_MakerMulti_Character_Select_1_Main_Received(SocketMessage message, MenuIdStructure menuSession)
        {
            var multimaker_session = Global.MultiMaker_Session_List.SingleOrDefault(x => x.User.Id == menuSession.User.Id);

            var account = UserInfoClasses.GetAccount(message.Author);
            string input_string = message.Content;
            input_string = Global.RemoveBotMention(input_string).Trim();

            // break word into array
            // check if last index is a number
            // if number, assign sprite number and skip next menu
            // if not, continue as normal

            MakerCommandData maker_command = new MakerCommandData()
            {
                Character_Data_1 = new MakerCharacterData()
                {
                    Sprite_Set_Version = multimaker_session.MakerCommand.Template,
                    Character_Keyword = input_string
                }
            };

            string[] input_as_array = input_string.Split(' ');

            int sprite_number;

            if (int.TryParse(input_as_array[input_as_array.Length - 1], out sprite_number))
            {
                return ParseAsCharacterAndSpriteNumber(menuSession, multimaker_session, account, input_as_array);
            }
            else
            {
                return ParseAsCharacter(menuSession, multimaker_session, account, input_string);
            }
        }

        public static Task ParseAsCharacter(MenuIdStructure menuSession, SocialLinkerCommand multimaker_session, UserInfoFields account, string input_string)
        {
            MakerCommandData maker_command = new MakerCommandData()
            {
                Character_Data_1 = new MakerCharacterData()
                {
                    Sprite_Set_Version = multimaker_session.MakerCommand.Template,
                    Character_Keyword = input_string
                }
            };

            OfficialSetData sprite_set_info = OfficialSetMethods.GetSpriteSetInfo(account, maker_command);

            if (sprite_set_info == null)
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = MakerMulti_Char_Select_1_Menu.MakerMulti_Character_Select_1_Invalid_Character(menuSession.User, menuSession.MenuMessage, input_string);
                return Task.CompletedTask;
            }

            multimaker_session.MakerCommand.Character_Data_1.Set_Data = sprite_set_info;

            // Stop the timeout timer associated with the menu.
            menuSession.MenuTimer.Stop();

            // Go to a new menu.
            _ = MakerMulti_Sprite_Select_1_Menu.MakerMulti_Sprite_Select_1_Main(menuSession.User, menuSession.MenuMessage);
            return Task.CompletedTask;
        }

        public static Task ParseAsCharacterAndSpriteNumber(
            MenuIdStructure menuSession, 
            SocialLinkerCommand multimaker_session, 
            UserInfoFields account, 
            string[] input_as_array)
        {
            // parse as character and sprite
            // else, parse as character
            string altered_input_character = "";

            for (int i = 0; i < input_as_array.Length - 1; i++)
            {
                altered_input_character += input_as_array[i];
            }

            MakerCommandData maker_command = new MakerCommandData()
            {
                Character_Data_1 = new MakerCharacterData()
                {
                    Sprite_Set_Version = multimaker_session.MakerCommand.Template,
                    Character_Keyword = altered_input_character
                }
            };

            OfficialSetData sprite_set_info = OfficialSetMethods.GetSpriteSetInfo(account, maker_command);

            if (sprite_set_info == null)
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = MakerMulti_Char_Select_1_Menu.MakerMulti_Character_Select_1_Invalid_Character(menuSession.User, menuSession.MenuMessage, altered_input_character);
                return Task.CompletedTask;
            }

            multimaker_session.MakerCommand.Character_Data_1.Set_Data = sprite_set_info;

            // Sprite Number Comparison
            // input_as_array[input_as_array.Length - 1] is the sprite number
            string result = Utility.Sprite_Number_Parser(input_as_array[input_as_array.Length - 1], multimaker_session, 1);

            switch (result)
            {
                case "Success":
                    break;

                case "Too_Many_Animation_Frames":
                    // Stop the timeout timer associated with the menu.
                    menuSession.MenuTimer.Stop();

                    // Go to a new menu.
                    _ = MakerMulti_Sprite_Select_1_Menu.MakerMulti_Sprite_Select_1_Error_Too_Many_Animation_Frames(menuSession.User, menuSession.MenuMessage);
                    return Task.CompletedTask;

                case "Non_Digit_In_Sprite_Number":
                    // Stop the timeout timer associated with the menu.
                    menuSession.MenuTimer.Stop();

                    // Go to a new menu.
                    _ = MakerMulti_Sprite_Select_1_Menu.MakerMulti_Sprite_Select_1_Error_Non_Digit_In_Sprite_Number(menuSession.User, menuSession.MenuMessage);
                    return Task.CompletedTask;
            }

            if (Utility.Base_Sprite_Validity_Check(multimaker_session.MakerCommand.Character_Data_1) == false)
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = MakerMulti_Sprite_Select_1_Menu.MakerMulti_Sprite_Select_1_Invalid_Base_Sprite(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }
            if ((multimaker_session.MakerCommand.Character_Data_1.Base_Sprite == 0) && ((multimaker_session.MakerCommand.Character_Data_1.Eye_Frame != default) || (multimaker_session.MakerCommand.Character_Data_1.Mouth_Frame != default)))
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = MakerMulti_Sprite_Select_1_Menu.MakerMulti_Sprite_Select_1_Error_Animation_Frame_With_Blank_Sprite(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            if (multimaker_session.MakerCommand.Character_Data_1.Base_Sprite != 0)
            {
                var bustup = Utility.Bustup_Selection(menuSession, account, multimaker_session.MakerCommand.Character_Data_1, 1); // Just a validity check

                if (bustup == null) // In case the validity check fails
                {
                    return Task.CompletedTask;
                }

                multimaker_session.MakerCommand.Character_Data_1.Bustup_Data = BustupDataMethods.Get_Bustup_Data(account, multimaker_session.MakerCommand.Character_Data_1.Set_Data, multimaker_session.MakerCommand.Character_Data_1);
            }

            menuSession.MenuTimer.Stop();
            _ = MakerMulti_Char_Select_2_Menu.MakerMulti_Character_Select_2_Main(menuSession.User, menuSession.MenuMessage);

            return Task.CompletedTask;
        }

        //public static Task Nav_MakerMulti_Character_Select_1_Main_Received(SocketMessage message, MenuIdStructure menuSession)
        //{
        //    var multimaker_session = Global.MultiMaker_Session_List.SingleOrDefault(x => x.User.Id == menuSession.User.Id);

        //    var account = UserInfoClasses.GetAccount(message.Author);
        //    string input_string = message.Content;
        //    input_string = Global.RemoveBotMention(input_string).Trim();

        //    // break word into array
        //    // check if last index is a number
        //    // if number, assign sprite number and skip next menu
        //    // if not, continue as normal

        //    MakerCommandData maker_command = new MakerCommandData()
        //    {
        //        Character_Data_1 = new MakerCharacterData()
        //        {
        //            Sprite_Set_Version = multimaker_session.MakerCommand.Template,
        //            Character_Keyword = input_string
        //        }
        //    };

        //    string[] input_as_array = input_string.Split(' ');

        //    int sprite_number;

        //    if (int.TryParse(input_as_array[input_as_array.Length - 1], out sprite_number))
        //    {
        //        string altered_input_character = "";

        //        for (int i = 0; i < input_as_array.Length - 1; i++)
        //        {
        //            altered_input_character += input_as_array[i];

        //            maker_command.Character_Data_1 = new MakerCharacterData()
        //            {
        //                Sprite_Set_Version = multimaker_session.MakerCommand.Template,
        //                Character_Keyword = altered_input_character
        //            };
        //        }
        //    }



        //    OfficialSetData sprite_set_info = OfficialSetMethods.GetSpriteSetInfo(account, maker_command);

        //    if (sprite_set_info == null)
        //    {
        //        // Stop the timeout timer associated with the menu.
        //        menuSession.MenuTimer.Stop();

        //        // Go to a new menu.
        //        _ = MakerMulti_Char_Select_1_Menu.MakerMulti_Character_Select_1_Invalid_Character(menuSession.User, menuSession.MenuMessage, input_string);
        //        return Task.CompletedTask;
        //    }

        //    multimaker_session.MakerCommand.Character_Data_1.Set_Data = sprite_set_info;

        //    // Stop the timeout timer associated with the menu.
        //    menuSession.MenuTimer.Stop();

        //    // Go to a new menu.
        //    _ = MakerMulti_Sprite_Select_1_Menu.MakerMulti_Sprite_Select_1_Main(menuSession.User, menuSession.MenuMessage);
        //    return Task.CompletedTask;
        //}
    }
}
