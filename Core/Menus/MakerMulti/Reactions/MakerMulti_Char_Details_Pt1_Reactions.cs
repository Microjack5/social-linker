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
        public static Task Nav_MakerMulti_1Char_Details_Main(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            menuSession.MenuTimer.Stop();

            switch (component.Data.CustomId)
            {
                case "makermulti-1char-details-modal-open":
                    _ = MakerMulti_Char_Details_Pt1_Menu.MakerMulti_1Char_Details_Modal(component);
                    break;

                case "back-to-makermulti-bbtag-layout-select":
                    component.DeferAsync(ephemeral: true);
                    _ = MakerMulti_BBTAG_Layout_Menu.MakerMulti_BBTAG_Layout_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_MakerMulti_2Char_Details_Main(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            menuSession.MenuTimer.Stop();

            switch (component.Data.CustomId)
            {
                case "makermulti-2char-details-modal-open":
                    _ = MakerMulti_Char_Details_Pt1_Menu.MakerMulti_2Char_Details_Modal(component);
                    break;

                case "back-to-makermulti-title-select":
                    component.DeferAsync(ephemeral: true);
                    _ = MakerMulti_Title_Select_Menu.MakerMulti_Main_Menu(menuSession.User, menuSession.MenuMessage);
                    break;

                case "back-to-makermulti-bbtag-layout-select":
                    component.DeferAsync(ephemeral: true);
                    _ = MakerMulti_BBTAG_Layout_Menu.MakerMulti_BBTAG_Layout_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static async Task Nav_MakerMulti_1Char_Details_Modal(SocketModal modal, MenuIdStructure menuSession)
        {
            var multimaker_session = Global.MultiMaker_Session_List.SingleOrDefault(x => x.User.Id == menuSession.User.Id);
            var account = UserInfoClasses.GetAccount(menuSession.User);

            var character_1 = modal.Data.Components
            .FirstOrDefault(x => x.CustomId == "character_1")?.Value;

            var sprite_1 = modal.Data.Components
            .FirstOrDefault(x => x.CustomId == "sprite_1")?.Value;

            if (!await Utility.Process_Character(multimaker_session, menuSession, account, modal, character_1, sprite_1, 1))
            {
                return;
            }

            menuSession.MenuTimer.Stop();
            await modal.DeferAsync(ephemeral: true);

            if (multimaker_session.MakerCommand.Template == "BBTAG")
            {
                _ = MakerMulti_BBTAG_Speaker_Menu.MakerMulti_BBTAG_Speaker_Main(menuSession);
            }
            else
            {
                _ = MakerMulti_Dialogue_Entry_Menu.MakerMulti_Display_Name_and_Dialogue_Entry_Main(menuSession);
            }

            return;
        }

        public static async Task Nav_MakerMulti_2Char_Details_Modal(SocketModal modal, MenuIdStructure menuSession)
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

            if (!await Utility.Process_Character(multimaker_session, menuSession, account, modal, character_1, sprite_1, 1))
            {
                return;
            }

            if (!await Utility.Process_Character(multimaker_session, menuSession, account, modal, character_2, sprite_2, 2))
            {
                return;
            }

            menuSession.MenuTimer.Stop();
            await modal.DeferAsync(ephemeral: true);

            // BBTAG Handling
            if (multimaker_session.MakerCommand.Template == "BBTAG")
            {
                switch (multimaker_session.MakerCommand.BBTAG_Specific_Data.Layout)
                {
                    case "1":
                    case "2":
                    case "3":
                    case "4":
                    case "5":
                    case "6":
                    case "7":
                        _ = MakerMulti_BBTAG_Speaker_Menu.MakerMulti_BBTAG_Speaker_Main(menuSession);
                        break;
                    case "8":
                    case "9":
                        _ = MakerMulti_Char_Details_Pt2_Menu.MakerMulti_3Char_Details_Main(menuSession);
                        break;
                    case "10":
                        _ = MakerMulti_Char_Details_Pt2_Menu.MakerMulti_4Char_Details_Main(menuSession);
                        break;
                }

                return;
            }

            // Else, go to dialogue menu
            _ = MakerMulti_Dialogue_Entry_Menu.MakerMulti_Display_Name_and_Dialogue_Entry_Main(menuSession);
            return;
        }

        public static Task Return_To_Char_Details_Pt1(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            //if 

            if (component.Data.CustomId == "back-to-makermulti-2char-details")
            {
                menuSession.MenuTimer.Stop();
                component.DeferAsync(ephemeral: true);
                _ = MakerMulti_Char_Details_Pt1_Menu.MakerMulti_2Char_Details_Main(menuSession);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }
    }
}
