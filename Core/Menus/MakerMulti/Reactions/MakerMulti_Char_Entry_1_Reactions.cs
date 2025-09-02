using Discord;
using Discord.WebSocket;
using SocialLinker.Core.CloudStorageTables;
using SocialLinker.Core.LocalStorageTables;
using SocialLinker.Core.Menus.MakerMulti.Main;
using SocialLinker.Core.SceneMaker;
using SocialLinker.Core.SceneMaker.Data.Bustup;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static SocialLinker.Core.Menus.MakerMulti.Reactions.Utility;

namespace SocialLinker.Core.Menus.MakerMulti.Reactions
{
    class MakerMulti_Char_Entry_1_Reactions
    {
        public static Task Nav_MakerMulti_Char_Entry_1_Main(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            menuSession.MenuTimer.Stop();

            switch (component.Data.CustomId)
            {
                case "makermulti-char-entry-1-modal-open":
                    _ = MakerMulti_Char_Entry_1_Menu.MakerMulti_Char_Details_Modal(component);
                    break;

                case "back-to-makermulti-title-select":
                    component.DeferAsync(ephemeral: true);
                    _ = MakerMulti_Title_Select_Menu.MakerMulti_Main_Menu(menuSession.User, menuSession.MenuMessage);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_MakerMulti_Char_Entry_1_Modal(SocketModal modal, MenuIdStructure menuSession)
        {
            var multimaker_session = Global.MultiMaker_Session_List.SingleOrDefault(x => x.User.Id == menuSession.User.Id);
            var account = UserInfoClasses.GetAccount(menuSession.User);

            var character_1 = modal.Data.Components
            .FirstOrDefault(x => x.CustomId == "character_1")?.Value;

            var sprite_1 = modal.Data.Components
            .FirstOrDefault(x => x.CustomId == "sprite_1")?.Value;

            var character_2 = modal.Data.Components
            .FirstOrDefault(x => x.CustomId == "character_2")?.Value;

            var sprite_2 = modal.Data.Components
            .FirstOrDefault(x => x.CustomId == "sprite_2")?.Value;

            Process_Character(multimaker_session, menuSession, account, modal, character_1, sprite_1, 1);
            Process_Character(multimaker_session, menuSession, account, modal, character_2, sprite_2, 2);

            menuSession.MenuTimer.Stop();
            modal.DeferAsync(ephemeral: true);
            _ = MakerMulti_Dialogue_Entry_Menu.MakerMulti_Dialogue_Entry_Main(menuSession);
            return Task.CompletedTask;

            //var char_1_set_info = Utility.ValidateCharacter(multimaker_session, account, character_1);

            //if (char_1_set_info == null)
            //{
            //    menuSession.MenuTimer.Stop();
            //    modal.DeferAsync(ephemeral: true);
            //    _ = MakerMulti_Char_Entry_1_Menu.MakerMulti_Char_Entry_1_Invalid_Character(menuSession.User, menuSession.MenuMessage, character_1);
            //    return Task.CompletedTask;
            //}

            //multimaker_session.MakerCommand.Character_Data_1.Set_Data = char_1_set_info;

            //// Process Character #1 Sprite Number
            //Console.WriteLine("Process Character #1 Sprite Number");
            //string result_1 = Utility.Sprite_Number_Parser(sprite_1, multimaker_session, 1);

            //switch (result_1)
            //{
            //    case "Success":
            //        break;

            //    case "Too_Many_Animation_Frames":
            //        menuSession.MenuTimer.Stop();
            //        modal.DeferAsync(ephemeral: true);
            //        _ = MakerMulti_Char_Entry_1_Menu.MakerMulti_Char_Entry_1_Sprite_Select_Too_Many_Animation_Frames(menuSession.User, menuSession.MenuMessage, multimaker_session.MakerCommand.Character_Data_1);
            //        return Task.CompletedTask;

            //    case "Non_Digit_In_Sprite_Number":
            //        menuSession.MenuTimer.Stop();
            //        modal.DeferAsync(ephemeral: true);
            //        _ = MakerMulti_Char_Entry_1_Menu.MakerMulti_Char_Entry_1_Sprite_Select_Non_Digit_In_Sprite_Number(menuSession.User, menuSession.MenuMessage, multimaker_session.MakerCommand.Character_Data_1);
            //        return Task.CompletedTask;
            //}

            //if (Utility.Base_Sprite_Validity_Check(multimaker_session.MakerCommand.Character_Data_1) == false)
            //{
            //    menuSession.MenuTimer.Stop();
            //    modal.DeferAsync(ephemeral: true);
            //    _ = MakerMulti_Char_Entry_1_Menu.MakerMulti_Char_Entry_1_Invalid_Base_Sprite(menuSession.User, menuSession.MenuMessage, multimaker_session.MakerCommand.Character_Data_1);
            //    return Task.CompletedTask;
            //}
            //if ((multimaker_session.MakerCommand.Character_Data_1.Base_Sprite == 0) && ((multimaker_session.MakerCommand.Character_Data_1.Eye_Frame != default) || (multimaker_session.MakerCommand.Character_Data_1.Mouth_Frame != default)))
            //{
            //    menuSession.MenuTimer.Stop();
            //    modal.DeferAsync(ephemeral: true);
            //    _ = MakerMulti_Char_Entry_1_Menu.MakerMulti_Char_Entry_1_Sprite_Select_Animation_Frame_With_Blank_Sprite(menuSession.User, menuSession.MenuMessage, multimaker_session.MakerCommand.Character_Data_1);
            //    return Task.CompletedTask;
            //}

            //if (multimaker_session.MakerCommand.Character_Data_1.Base_Sprite != 0)
            //{
            //    try
            //    {
            //        var bustup = Utility.Bustup_Selection(menuSession, account, multimaker_session.MakerCommand.Character_Data_1, 1);
            //        multimaker_session.MakerCommand.Character_Data_1.Bustup_Data = BustupDataMethods.Get_Bustup_Data(account, multimaker_session.MakerCommand.Character_Data_1.Set_Data, multimaker_session.MakerCommand.Character_Data_1);
            //    }
            //    catch (EyeFrameNotFoundException)
            //    {
            //        menuSession.MenuTimer.Stop();
            //        modal.DeferAsync(ephemeral: true);
            //        _ = MakerMulti_Char_Entry_1_Menu.MakerMulti_Char_Entry_1_Sprite_Select_Eye_Frame_Not_Found(menuSession.User, menuSession.MenuMessage, multimaker_session.MakerCommand.Character_Data_1);
            //        return Task.CompletedTask;
            //    }
            //    catch (MouthFrameNotFoundException)
            //    {
            //        menuSession.MenuTimer.Stop();
            //        modal.DeferAsync(ephemeral: true);
            //        _ = MakerMulti_Char_Entry_1_Menu.MakerMulti_Char_Entry_1_Sprite_Select_Mouth_Frame_Not_Found(menuSession.User, menuSession.MenuMessage, multimaker_session.MakerCommand.Character_Data_1);
            //        return Task.CompletedTask;
            //    }
            //}

            //// Process Character #2
            //Console.WriteLine("Process Character #2");
            //var char_2_set_info = Utility.ValidateCharacter(multimaker_session, account, character_2);

            //if (char_2_set_info == null)
            //{
            //    menuSession.MenuTimer.Stop();
            //    modal.DeferAsync(ephemeral: true);
            //    _ = MakerMulti_Char_Entry_1_Menu.MakerMulti_Char_Entry_1_Invalid_Character(menuSession.User, menuSession.MenuMessage, character_2);
            //    return Task.CompletedTask;
            //}

            //multimaker_session.MakerCommand.Character_Data_2.Set_Data = char_2_set_info;

            //// Process Character #2 Sprite Number
            //string result_2 = Utility.Sprite_Number_Parser(sprite_2, multimaker_session, 2);

            //Console.WriteLine("Process Character #2 Sprite Number");

            //switch (result_2)
            //{
            //    case "Success":
            //        break;

            //    case "Too_Many_Animation_Frames":
            //        menuSession.MenuTimer.Stop();
            //        modal.DeferAsync(ephemeral: true);
            //        _ = MakerMulti_Char_Entry_1_Menu.MakerMulti_Char_Entry_1_Sprite_Select_Too_Many_Animation_Frames(menuSession.User, menuSession.MenuMessage, multimaker_session.MakerCommand.Character_Data_2);
            //        return Task.CompletedTask;

            //    case "Non_Digit_In_Sprite_Number":
            //        menuSession.MenuTimer.Stop();
            //        modal.DeferAsync(ephemeral: true);
            //        _ = MakerMulti_Char_Entry_1_Menu.MakerMulti_Char_Entry_1_Sprite_Select_Non_Digit_In_Sprite_Number(menuSession.User, menuSession.MenuMessage, multimaker_session.MakerCommand.Character_Data_2);
            //        return Task.CompletedTask;
            //}

            //if (Utility.Base_Sprite_Validity_Check(multimaker_session.MakerCommand.Character_Data_2) == false)
            //{
            //    menuSession.MenuTimer.Stop();
            //    modal.DeferAsync(ephemeral: true);
            //    _ = MakerMulti_Char_Entry_1_Menu.MakerMulti_Char_Entry_1_Invalid_Base_Sprite(menuSession.User, menuSession.MenuMessage, multimaker_session.MakerCommand.Character_Data_2);
            //    return Task.CompletedTask;
            //}
            //if ((multimaker_session.MakerCommand.Character_Data_2.Base_Sprite == 0) && ((multimaker_session.MakerCommand.Character_Data_2.Eye_Frame != default) || (multimaker_session.MakerCommand.Character_Data_2.Mouth_Frame != default)))
            //{
            //    menuSession.MenuTimer.Stop();
            //    modal.DeferAsync(ephemeral: true);
            //    _ = MakerMulti_Char_Entry_1_Menu.MakerMulti_Char_Entry_1_Sprite_Select_Animation_Frame_With_Blank_Sprite(menuSession.User, menuSession.MenuMessage, multimaker_session.MakerCommand.Character_Data_2);
            //    return Task.CompletedTask;
            //}

            //if (multimaker_session.MakerCommand.Character_Data_2.Base_Sprite != 0)
            //{
            //    try
            //    {
            //        var bustup = Utility.Bustup_Selection(menuSession, account, multimaker_session.MakerCommand.Character_Data_2, 2);
            //        multimaker_session.MakerCommand.Character_Data_2.Bustup_Data = BustupDataMethods.Get_Bustup_Data(account, multimaker_session.MakerCommand.Character_Data_2.Set_Data, multimaker_session.MakerCommand.Character_Data_2);
            //    }
            //    catch (EyeFrameNotFoundException)
            //    {
            //        menuSession.MenuTimer.Stop();
            //        modal.DeferAsync(ephemeral: true);
            //        _ = MakerMulti_Char_Entry_1_Menu.MakerMulti_Char_Entry_1_Sprite_Select_Eye_Frame_Not_Found(menuSession.User, menuSession.MenuMessage, multimaker_session.MakerCommand.Character_Data_2);
            //        return Task.CompletedTask;
            //    }
            //    catch (MouthFrameNotFoundException)
            //    {
            //        menuSession.MenuTimer.Stop();
            //        modal.DeferAsync(ephemeral: true);
            //        _ = MakerMulti_Char_Entry_1_Menu.MakerMulti_Char_Entry_1_Sprite_Select_Mouth_Frame_Not_Found(menuSession.User, menuSession.MenuMessage, multimaker_session.MakerCommand.Character_Data_2);
            //        return Task.CompletedTask;
            //    }
            //}

            //menuSession.MenuTimer.Stop();
            //modal.DeferAsync(ephemeral: true);
            //_ = MakerMulti_Dialogue_Entry_Menu.MakerMulti_Dialogue_Entry_Main(menuSession);

            //return Task.CompletedTask;
        }

        public static Task Return_To_Char_Entry_1(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            if (component.Data.CustomId == "back-to-makermulti-char-entry-1")
            {
                menuSession.MenuTimer.Stop();
                component.DeferAsync(ephemeral: true);
                _ = MakerMulti_Char_Entry_1_Menu.MakerMulti_Char_Entry_1_Main(menuSession);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        public static Task Process_Character(SocialLinkerCommand multimaker_session, MenuIdStructure menuSession, UserInfoFields account, SocketModal modal, string character_input, string sprite_input, int current_character)
        {
            var character_set_data = Utility.ValidateCharacter(multimaker_session, account, character_input);

            if (character_set_data == null)
            {
                menuSession.MenuTimer.Stop();
                modal.DeferAsync(ephemeral: true);

                switch (current_character)
                {
                    case 1:
                    case 2:
                        _ = MakerMulti_Char_Entry_1_Menu.MakerMulti_Char_Entry_1_Invalid_Character(menuSession.User, menuSession.MenuMessage, character_input);
                        break;

                    case 3:
                    case 4:
                        //_ = MakerMulti_Char_Entry_2_Menu.MakerMulti_Char_Entry_2_Invalid_Character(menuSession.User, menuSession.MenuMessage, character_input);
                        break;
                }
                
                return Task.CompletedTask;
            }

            MakerCharacterData general_character_data = null;

            switch (current_character)
            {
                case 1:
                    multimaker_session.MakerCommand.Character_Data_1.Set_Data = character_set_data;
                    general_character_data = multimaker_session.MakerCommand.Character_Data_1;
                    break;

                case 2:
                    multimaker_session.MakerCommand.Character_Data_2.Set_Data = character_set_data;
                    general_character_data = multimaker_session.MakerCommand.Character_Data_2;
                    break;

                case 3:
                    multimaker_session.MakerCommand.Character_Data_3.Set_Data = character_set_data;
                    general_character_data = multimaker_session.MakerCommand.Character_Data_3;
                    break;

                case 4:
                    multimaker_session.MakerCommand.Character_Data_4.Set_Data = character_set_data;
                    general_character_data = multimaker_session.MakerCommand.Character_Data_4;
                    break;
            }

            //switch (current_character)
            //{
            //    case 1:
            //        break;

            //    case 2:
            //        break;

            //    case 3:
            //        break;

            //    case 4:
            //        break;
            //}



            // Process Character Sprite Number
            string parsed_sprite_number = Utility.Sprite_Number_Parser(sprite_input, multimaker_session, current_character);

            switch (parsed_sprite_number)
            {
                case "Success":
                    break;

                case "Too_Many_Animation_Frames":
                    menuSession.MenuTimer.Stop();
                    modal.DeferAsync(ephemeral: true);
                    switch (current_character)
                    {
                        case 1:
                        case 2:
                            _ = MakerMulti_Char_Entry_1_Menu.MakerMulti_Char_Entry_1_Sprite_Select_Too_Many_Animation_Frames(menuSession.User, menuSession.MenuMessage, general_character_data);
                            break;

                        case 3:
                        case 4:
                            //_ = MakerMulti_Char_Entry_2_Menu.MakerMulti_Char_Entry_2_Sprite_Select_Too_Many_Animation_Frames(menuSession.User, menuSession.MenuMessage, general_character_data);
                            break;
                    }
                    _ = MakerMulti_Char_Entry_1_Menu.MakerMulti_Char_Entry_1_Sprite_Select_Too_Many_Animation_Frames(menuSession.User, menuSession.MenuMessage, general_character_data);
                    return Task.CompletedTask;

                case "Non_Digit_In_Sprite_Number":
                    menuSession.MenuTimer.Stop();
                    modal.DeferAsync(ephemeral: true);
                    _ = MakerMulti_Char_Entry_1_Menu.MakerMulti_Char_Entry_1_Sprite_Select_Non_Digit_In_Sprite_Number(menuSession.User, menuSession.MenuMessage, general_character_data);
                    return Task.CompletedTask;
            }

            if (Utility.Base_Sprite_Validity_Check(general_character_data) == false)
            {
                menuSession.MenuTimer.Stop();
                modal.DeferAsync(ephemeral: true);
                _ = MakerMulti_Char_Entry_1_Menu.MakerMulti_Char_Entry_1_Invalid_Base_Sprite(menuSession.User, menuSession.MenuMessage, general_character_data);
                return Task.CompletedTask;
            }
            if ((general_character_data.Base_Sprite == 0) && ((general_character_data.Eye_Frame != default) || (general_character_data.Mouth_Frame != default)))
            {
                menuSession.MenuTimer.Stop();
                modal.DeferAsync(ephemeral: true);
                _ = MakerMulti_Char_Entry_1_Menu.MakerMulti_Char_Entry_1_Sprite_Select_Animation_Frame_With_Blank_Sprite(menuSession.User, menuSession.MenuMessage, general_character_data);
                return Task.CompletedTask;
            }

            if (general_character_data.Base_Sprite != 0)
            {
                try
                {
                    switch (current_character)
                    {
                        case 1:
                            multimaker_session.MakerCommand.Character_Data_1.Bustup_Data = BustupDataMethods.Get_Bustup_Data(account, general_character_data.Set_Data, general_character_data);
                            break;

                        case 2:
                            multimaker_session.MakerCommand.Character_Data_2.Bustup_Data = BustupDataMethods.Get_Bustup_Data(account, general_character_data.Set_Data, general_character_data);
                            break;

                        case 3:
                            multimaker_session.MakerCommand.Character_Data_3.Bustup_Data = BustupDataMethods.Get_Bustup_Data(account, general_character_data.Set_Data, general_character_data);
                            break;

                        case 4:
                            multimaker_session.MakerCommand.Character_Data_4.Bustup_Data = BustupDataMethods.Get_Bustup_Data(account, general_character_data.Set_Data, general_character_data);
                            break;
                    }
                }
                catch (EyeFrameNotFoundException)
                {
                    menuSession.MenuTimer.Stop();
                    modal.DeferAsync(ephemeral: true);
                    
                    switch (current_character)
                    {
                        case 1:
                        case 2:
                            _ = MakerMulti_Char_Entry_1_Menu.MakerMulti_Char_Entry_1_Sprite_Select_Eye_Frame_Not_Found(menuSession.User, menuSession.MenuMessage, general_character_data);
                            break;

                        case 3:
                        case 4:
                            //_ = MakerMulti_Char_Entry_2_Menu.MakerMulti_Char_Entry_2_Sprite_Select_Eye_Frame_Not_Found(menuSession.User, menuSession.MenuMessage, general_character_data);
                            break;
                    }

                    return Task.CompletedTask;
                }
                catch (MouthFrameNotFoundException)
                {
                    menuSession.MenuTimer.Stop();
                    modal.DeferAsync(ephemeral: true);
                    
                    switch (current_character)
                    {
                        case 1:
                        case 2:
                            _ = MakerMulti_Char_Entry_1_Menu.MakerMulti_Char_Entry_1_Sprite_Select_Mouth_Frame_Not_Found(menuSession.User, menuSession.MenuMessage, general_character_data);
                            break;

                        case 3:
                        case 4:
                            //_ = MakerMulti_Char_Entry_2_Menu.MakerMulti_Char_Entry_2_Sprite_Select_Mouth_Frame_Not_Found(menuSession.User, menuSession.MenuMessage, general_character_data);
                            break;
                    }

                    return Task.CompletedTask;
                }
            }

            return Task.CompletedTask;
        }
    }
}
