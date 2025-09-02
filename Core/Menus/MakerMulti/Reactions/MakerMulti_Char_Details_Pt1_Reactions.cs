using Discord.WebSocket;
using SocialLinker.Core.CloudStorageTables;
using SocialLinker.Core.Menus.MakerMulti.Main;
using SocialLinker.Core.SceneMaker;
using SocialLinker.Core.SceneMaker.Data.Bustup;
using System.Linq;
using System.Threading.Tasks;
using static SocialLinker.Core.Menus.MakerMulti.Reactions.Utility;

namespace SocialLinker.Core.Menus.MakerMulti.Reactions
{
    class MakerMulti_Char_Details_Pt1_Reactions
    {
        public static Task Nav_MakerMulti_Char_Details_Pt1_Main(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            menuSession.MenuTimer.Stop();

            switch (component.Data.CustomId)
            {
                case "makermulti-char-entry-1-modal-open":
                    _ = MakerMulti_Char_Details_Pt1_Menu.MakerMulti_Char_Details_Pt1_Modal(component);
                    break;

                case "back-to-makermulti-title-select":
                    component.DeferAsync(ephemeral: true);
                    _ = MakerMulti_Title_Select_Menu.MakerMulti_Main_Menu(menuSession.User, menuSession.MenuMessage);
                    break;
            }

            return Task.CompletedTask;
        }

        public static async Task Nav_MakerMulti_Char_Details_Pt1_Modal(SocketModal modal, MenuIdStructure menuSession)
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

            if (!await Process_Character(multimaker_session, menuSession, account, modal, character_1, sprite_1, 1))
            {
                return;
            }

            if (!await Process_Character(multimaker_session, menuSession, account, modal, character_2, sprite_2, 2))
            {
                return;
            }

            menuSession.MenuTimer.Stop();
            await modal.DeferAsync(ephemeral: true);
            _ = MakerMulti_Dialogue_Entry_Menu.MakerMulti_Dialogue_Entry_Main(menuSession);
            return;
        }

        public static Task Return_To_Char_Details_Pt1(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            if (component.Data.CustomId == "back-to-makermulti-char-details-pt1")
            {
                menuSession.MenuTimer.Stop();
                component.DeferAsync(ephemeral: true);
                _ = MakerMulti_Char_Details_Pt1_Menu.MakerMulti_Char_Details_Pt1_Main(menuSession);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        public static Task<bool> Process_Character(SocialLinkerCommand multimaker_session, MenuIdStructure menuSession, UserInfoFields account, SocketModal modal, string character_input, string sprite_input, int current_character)
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
                        _ = MakerMulti_Char_Details_Pt1_Menu.MakerMulti_Char_Details_Pt1_Invalid_Character(menuSession.User, menuSession.MenuMessage, character_input);
                        break;

                    case 3:
                    case 4:
                        //_ = MakerMulti_Char_Entry_2_Menu.MakerMulti_Char_Entry_2_Invalid_Character(menuSession.User, menuSession.MenuMessage, character_input);
                        break;
                }
                
                return Task.FromResult(false);
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
                            _ = MakerMulti_Char_Details_Pt1_Menu.MakerMulti_Char_Details_Pt1_Sprite_Select_Too_Many_Animation_Frames(menuSession.User, menuSession.MenuMessage, general_character_data);
                            break;

                        case 3:
                        case 4:
                            //_ = MakerMulti_Char_Entry_2_Menu.MakerMulti_Char_Entry_2_Sprite_Select_Too_Many_Animation_Frames(menuSession.User, menuSession.MenuMessage, general_character_data);
                            break;
                    }
                    return Task.FromResult(false);

                case "Non_Digit_In_Sprite_Number":
                    menuSession.MenuTimer.Stop();
                    modal.DeferAsync(ephemeral: true);
                    _ = MakerMulti_Char_Details_Pt1_Menu.MakerMulti_Char_Details_Pt1_Sprite_Select_Non_Digit_In_Sprite_Number(menuSession.User, menuSession.MenuMessage, general_character_data);
                    return Task.FromResult(false);
            }

            if (Utility.Base_Sprite_Validity_Check(general_character_data) == false)
            {
                menuSession.MenuTimer.Stop();
                modal.DeferAsync(ephemeral: true);
                _ = MakerMulti_Char_Details_Pt1_Menu.MakerMulti_Char_Details_Pt1_Invalid_Base_Sprite(menuSession.User, menuSession.MenuMessage, general_character_data);
                return Task.FromResult(false);
            }
            if ((general_character_data.Base_Sprite == 0) && ((general_character_data.Eye_Frame != default) || (general_character_data.Mouth_Frame != default)))
            {
                menuSession.MenuTimer.Stop();
                modal.DeferAsync(ephemeral: true);
                _ = MakerMulti_Char_Details_Pt1_Menu.MakerMulti_Char_Details_Pt1_Sprite_Select_Animation_Frame_With_Blank_Sprite(menuSession.User, menuSession.MenuMessage, general_character_data);
                return Task.FromResult(false);
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
                            _ = MakerMulti_Char_Details_Pt1_Menu.MakerMulti_Char_Details_Pt1_Sprite_Select_Eye_Frame_Not_Found(menuSession.User, menuSession.MenuMessage, general_character_data);
                            break;

                        case 3:
                        case 4:
                            //_ = MakerMulti_Char_Entry_2_Menu.MakerMulti_Char_Entry_2_Sprite_Select_Eye_Frame_Not_Found(menuSession.User, menuSession.MenuMessage, general_character_data);
                            break;
                    }

                    return Task.FromResult(false);
                }
                catch (MouthFrameNotFoundException)
                {
                    menuSession.MenuTimer.Stop();
                    modal.DeferAsync(ephemeral: true);
                    
                    switch (current_character)
                    {
                        case 1:
                        case 2:
                            _ = MakerMulti_Char_Details_Pt1_Menu.MakerMulti_Char_Details_Pt1_Sprite_Select_Mouth_Frame_Not_Found(menuSession.User, menuSession.MenuMessage, general_character_data);
                            break;

                        case 3:
                        case 4:
                            //_ = MakerMulti_Char_Entry_2_Menu.MakerMulti_Char_Entry_2_Sprite_Select_Mouth_Frame_Not_Found(menuSession.User, menuSession.MenuMessage, general_character_data);
                            break;
                    }

                    return Task.FromResult(false);
                }
            }

            return Task.FromResult(true);
        }
    }
}
