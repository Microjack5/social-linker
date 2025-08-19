using Discord;
using Discord.WebSocket;
using SocialLinker.Core.CloudStorageTables;
using SocialLinker.Core.LocalStorageTables;
using SocialLinker.Core.Menus.MakerMulti.Main;
using SocialLinker.Core.SceneMaker;
using SocialLinker.Core.SceneMaker.Data.Bustup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SocialLinker.Core.Menus.MakerMulti.Reactions
{
    class MakerMulti_Char_Entry_1_Reactions
    {
        public static Task Nav_MakerMulti_Char_Entry_1_Main(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            if (component.Data.CustomId == "makermulti-char-entry-1-modal-open")
            {
                // Go to a new menu.
                _ = MakerMulti_Char_Entry_1_Menu.MakerMulti_Char_Details_Modal(component);
                return Task.CompletedTask;
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

            // Process Character #1
            Console.WriteLine("Process Character #1");
            var char_1_set_info = Utility.ValidateCharacter(multimaker_session, account, character_1);

            if (char_1_set_info == null)
            {
                menuSession.MenuTimer.Stop();
                _ = MakerMulti_Char_Select_1_Menu.MakerMulti_Character_Select_1_Invalid_Character(menuSession.User, menuSession.MenuMessage, character_1);
                return Task.CompletedTask;
            }

            multimaker_session.MakerCommand.Character_Data_1.Set_Data = char_1_set_info;

            // Process Character #1 Sprite Number
            Console.WriteLine("Process Character #1 Sprite Number");
            string result_1 = Utility.Sprite_Number_Parser(sprite_1, multimaker_session, 1);

            switch (result_1)
            {
                case "Success":
                    break;

                case "Too_Many_Animation_Frames":
                    menuSession.MenuTimer.Stop();
                    _ = MakerMulti_Sprite_Select_1_Menu.MakerMulti_Sprite_Select_1_Error_Too_Many_Animation_Frames(menuSession.User, menuSession.MenuMessage);
                    return Task.CompletedTask;

                case "Non_Digit_In_Sprite_Number":
                    menuSession.MenuTimer.Stop();
                    _ = MakerMulti_Sprite_Select_1_Menu.MakerMulti_Sprite_Select_1_Error_Non_Digit_In_Sprite_Number(menuSession.User, menuSession.MenuMessage);
                    return Task.CompletedTask;
            }

            if (Utility.Base_Sprite_Validity_Check(multimaker_session.MakerCommand.Character_Data_1) == false)
            {
                menuSession.MenuTimer.Stop();
                modal.DeferAsync(ephemeral: true);
                _ = MakerMulti_Sprite_Select_1_Menu.MakerMulti_Sprite_Select_1_Invalid_Base_Sprite(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }
            if ((multimaker_session.MakerCommand.Character_Data_1.Base_Sprite == 0) && ((multimaker_session.MakerCommand.Character_Data_1.Eye_Frame != default) || (multimaker_session.MakerCommand.Character_Data_1.Mouth_Frame != default)))
            {
                menuSession.MenuTimer.Stop();
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

            // Process Character #2
            Console.WriteLine("Process Character #2");
            var char_2_set_info = Utility.ValidateCharacter(multimaker_session, account, character_2);

            if (char_2_set_info == null)
            {
                menuSession.MenuTimer.Stop();
                _ = MakerMulti_Char_Select_2_Menu.MakerMulti_Character_Select_2_Invalid_Character(menuSession.User, menuSession.MenuMessage, character_2);
                return Task.CompletedTask;
            }

            multimaker_session.MakerCommand.Character_Data_2.Set_Data = char_2_set_info;

            // Process Character #2 Sprite Number
            string result_2 = Utility.Sprite_Number_Parser(sprite_2, multimaker_session, 2);

            Console.WriteLine("Process Character #2 Sprite Number");

            switch (result_2)
            {
                case "Success":
                    break;

                case "Too_Many_Animation_Frames":
                    menuSession.MenuTimer.Stop();
                    _ = MakerMulti_Sprite_Select_2_Menu.MakerMulti_Sprite_Select_2_Error_Too_Many_Animation_Frames(menuSession.User, menuSession.MenuMessage);
                    return Task.CompletedTask;

                case "Non_Digit_In_Sprite_Number":
                    menuSession.MenuTimer.Stop();
                    _ = MakerMulti_Sprite_Select_2_Menu.MakerMulti_Sprite_Select_2_Error_Non_Digit_In_Sprite_Number(menuSession.User, menuSession.MenuMessage);
                    return Task.CompletedTask;
            }

            if (Utility.Base_Sprite_Validity_Check(multimaker_session.MakerCommand.Character_Data_2) == false)
            {
                menuSession.MenuTimer.Stop();
                _ = MakerMulti_Sprite_Select_2_Menu.MakerMulti_Sprite_Select_2_Invalid_Base_Sprite(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }
            if ((multimaker_session.MakerCommand.Character_Data_2.Base_Sprite == 0) && ((multimaker_session.MakerCommand.Character_Data_2.Eye_Frame != default) || (multimaker_session.MakerCommand.Character_Data_2.Mouth_Frame != default)))
            {
                menuSession.MenuTimer.Stop();
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

            Console.WriteLine($"\n" +
                $"~Printed Values~\n" +
                $"Char #1: {character_1}\n" +
                $"Sprite #1: {sprite_1}\n" +
                $"Char #2: {character_2}\n" +
                $"Sprite #2: {sprite_2}\n" +
                $"");

            multimaker_session.MakerCommand.Display_Name = "Both of them";
            multimaker_session.MakerCommand.Dialogue = "Testing, 1, 2, 3...";

            modal.DeferAsync(ephemeral: true);
            _ = MakerMulti_Dialogue_Entry_Menu.MakerMulti_Dialogue_Entry_Main(menuSession);

            //SceneMaker.TemplateRenders.MakerMulti.RenderMultiCharP3P p3p_render = new SceneMaker.TemplateRenders.MakerMulti.RenderMultiCharP3P();
            //_ = p3p_render.Render_Multi_Character_Scene_P3P(multimaker_session);

            return Task.CompletedTask;
        }
    }
}
