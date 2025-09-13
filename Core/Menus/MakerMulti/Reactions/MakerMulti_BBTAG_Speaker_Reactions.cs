using Discord.WebSocket;
using SocialLinker.Core.Menus.MakerMulti.Main;
using System.Linq;
using System.Threading.Tasks;

namespace SocialLinker.Core.Menus.MakerMulti.Reactions
{
    class MakerMulti_BBTAG_Speaker_Reactions
    {
        public static Task Nav_MakerMulti_BBTAG_Speaker_Main(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            string selected = component.Data.Values.First();
            var multimaker_session = Global.MultiMaker_Session_List.SingleOrDefault(x => x.User.Id == menuSession.User.Id);

            multimaker_session.MakerCommand.BBTAG_Specific_Data.Speaker = selected;

            menuSession.MenuTimer.Stop();

            switch (selected)
            {
                case "char_1":
                    multimaker_session.MakerCommand.BBTAG_Specific_Data.Speaker_Series = multimaker_session.MakerCommand.Character_Data_1.Set_Data.Series;
                    multimaker_session.MakerCommand.BBTAG_Specific_Data.Speaker_Exists_In_Game = true;

                    if (multimaker_session.MakerCommand.Character_Data_1.Base_Sprite == 0)
                    {
                        multimaker_session.MakerCommand.BBTAG_Specific_Data.Speaker_Is_Spriteless = true;
                    }
                    break;

                case "char_2":
                    multimaker_session.MakerCommand.BBTAG_Specific_Data.Speaker_Series = multimaker_session.MakerCommand.Character_Data_2.Set_Data.Series;
                    multimaker_session.MakerCommand.BBTAG_Specific_Data.Speaker_Exists_In_Game = true;

                    if (multimaker_session.MakerCommand.Character_Data_2.Base_Sprite == 0)
                    {
                        multimaker_session.MakerCommand.BBTAG_Specific_Data.Speaker_Is_Spriteless = true;
                    }
                    break;

                case "char_3":
                    multimaker_session.MakerCommand.BBTAG_Specific_Data.Speaker_Series = multimaker_session.MakerCommand.Character_Data_3.Set_Data.Series;
                    multimaker_session.MakerCommand.BBTAG_Specific_Data.Speaker_Exists_In_Game = true;

                    if (multimaker_session.MakerCommand.Character_Data_3.Base_Sprite == 0)
                    {
                        multimaker_session.MakerCommand.BBTAG_Specific_Data.Speaker_Is_Spriteless = true;
                    }
                    break;

                case "char_4":
                    multimaker_session.MakerCommand.BBTAG_Specific_Data.Speaker_Series = multimaker_session.MakerCommand.Character_Data_4.Set_Data.Series;
                    multimaker_session.MakerCommand.BBTAG_Specific_Data.Speaker_Exists_In_Game = true;

                    if (multimaker_session.MakerCommand.Character_Data_4.Base_Sprite == 0)
                    {
                        multimaker_session.MakerCommand.BBTAG_Specific_Data.Speaker_Is_Spriteless = true;
                    }
                    break;

                case "system_1":
                case "system_2":
                    // Skip to dialogue-only input since we don't need a display name
                    _ = MakerMulti_Dialogue_Entry_Menu.MakerMulti_Dialogue_Only_Entry_Main(menuSession);
                    return Task.CompletedTask;

                case "offscreen":
                    // Skip to new menu to ask what series the off-screen character belongs to, THEN we'll go to dialogue
                    _ = MakerMulti_BBTAG_Speaker_Menu.MakerMulti_BBTAG_Offscreen_Speaker_Main(menuSession);
                    return Task.CompletedTask;
            }

            _ = MakerMulti_Dialogue_Entry_Menu.MakerMulti_Display_Name_and_Dialogue_Entry_Main(menuSession);

            return Task.CompletedTask;

            //switch (selected)
            //{
            //    case "char_1":
            //        multimaker_session.MakerCommand.BBTAG_Speaker = "char_1";
            //        break;

            //    case "char_2":
            //        break;

            //    case "char_3":
            //        break;

            //    case "char_4":
            //        break;

            //    case "system_1":
            //        break;

            //    case "system_2":
            //        break;

            //    case "offscreen":
            //        break;
            //}

            //if (selected == "char_1")
            //{
            //    var multimaker_session = Global.MultiMaker_Session_List.SingleOrDefault(x => x.User.Id == menuSession.User.Id);

            //    multimaker_session.MakerCommand.Template = "P3P";

            //    menuSession.MenuTimer.Stop();

            //    _ = MakerMulti_Char_Details_Pt1_Menu.MakerMulti_2Char_Details_Main(menuSession);
            //    return Task.CompletedTask;
            //}

            //else if (selected == "P4AU")
            //{
            //    var multimaker_session = Global.MultiMaker_Session_List.SingleOrDefault(x => x.User.Id == menuSession.User.Id);

            //    multimaker_session.MakerCommand.Template = "P4AU";

            //    menuSession.MenuTimer.Stop();

            //    _ = MakerMulti_Char_Details_Pt1_Menu.MakerMulti_2Char_Details_Main(menuSession);
            //    return Task.CompletedTask;
            //}

            //else if (selected == "P4D")
            //{
            //    var multimaker_session = Global.MultiMaker_Session_List.SingleOrDefault(x => x.User.Id == menuSession.User.Id);

            //    multimaker_session.MakerCommand.Template = "P4D";

            //    menuSession.MenuTimer.Stop();

            //    _ = MakerMulti_Char_Details_Pt1_Menu.MakerMulti_2Char_Details_Main(menuSession);
            //    return Task.CompletedTask;
            //}

            //else if (selected == "BBTAG")
            //{
            //    var multimaker_session = Global.MultiMaker_Session_List.SingleOrDefault(x => x.User.Id == menuSession.User.Id);
            //    multimaker_session.MakerCommand.Template = "BBTAG";

            //    menuSession.MenuTimer.Stop();

            //    _ = MakerMulti_BBTAG_Layout_Menu.MakerMulti_BBTAG_Layout_Main(menuSession);
            //    return Task.CompletedTask;
            //}

            //return Task.CompletedTask;
        }

        public static Task Nav_MakerMulti_BBTAG_Offscreen_Speaker_Main(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            string selected = component.Data.Values.First();
            var multimaker_session = Global.MultiMaker_Session_List.SingleOrDefault(x => x.User.Id == menuSession.User.Id);

            multimaker_session.MakerCommand.BBTAG_Specific_Data.Speaker_Series = selected;

            menuSession.MenuTimer.Stop();

            _ = MakerMulti_Dialogue_Entry_Menu.MakerMulti_Display_Name_and_Dialogue_Entry_Main(menuSession);

            return Task.CompletedTask;
        }
    }
}
