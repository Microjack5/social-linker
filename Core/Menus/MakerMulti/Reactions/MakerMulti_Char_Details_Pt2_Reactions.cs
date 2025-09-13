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
    class MakerMulti_Char_Details_Pt2_Reactions
    {
        public static Task Nav_MakerMulti_3Char_Details_Main(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            menuSession.MenuTimer.Stop();

            switch (component.Data.CustomId)
            {
                case "makermulti-3char-details-modal-open":
                    _ = MakerMulti_Char_Details_Pt2_Menu.MakerMulti_3Char_Details_Modal(component);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_MakerMulti_4Char_Details_Main(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            menuSession.MenuTimer.Stop();

            switch (component.Data.CustomId)
            {
                case "makermulti-4char-details-modal-open":
                    _ = MakerMulti_Char_Details_Pt2_Menu.MakerMulti_4Char_Details_Modal(component);
                    break;
            }

            return Task.CompletedTask;
        }

        public static async Task Nav_MakerMulti_3Char_Details_Modal(SocketModal modal, MenuIdStructure menuSession)
        {
            var multimaker_session = Global.MultiMaker_Session_List.SingleOrDefault(x => x.User.Id == menuSession.User.Id);
            var account = UserInfoClasses.GetAccount(menuSession.User);

            var character_3 = modal.Data.Components
            .FirstOrDefault(x => x.CustomId == "character_3")?.Value;

            var sprite_3 = modal.Data.Components
            .FirstOrDefault(x => x.CustomId == "sprite_3")?.Value;


            if (!await Utility.Process_Character(multimaker_session, menuSession, account, modal, character_3, sprite_3, 3))
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

        public static async Task Nav_MakerMulti_4Char_Details_Modal(SocketModal modal, MenuIdStructure menuSession)
        {
            var multimaker_session = Global.MultiMaker_Session_List.SingleOrDefault(x => x.User.Id == menuSession.User.Id);
            var account = UserInfoClasses.GetAccount(menuSession.User);

            var character_3 = modal.Data.Components
            .FirstOrDefault(x => x.CustomId == "character_3")?.Value;

            var sprite_3 = modal.Data.Components
            .FirstOrDefault(x => x.CustomId == "sprite_3")?.Value;

            var character_4 = modal.Data.Components
            .FirstOrDefault(x => x.CustomId == "character_4")?.Value;

            var sprite_4 = modal.Data.Components
            .FirstOrDefault(x => x.CustomId == "sprite_4")?.Value;

            if (!await Utility.Process_Character(multimaker_session, menuSession, account, modal, character_3, sprite_3, 3))
            {
                return;
            }

            if (!await Utility.Process_Character(multimaker_session, menuSession, account, modal, character_4, sprite_4, 4))
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

        public static Task Return_To_Char_Details_Pt2(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            menuSession.MenuTimer.Stop();
            component.DeferAsync(ephemeral: true);

            switch (component.Data.CustomId)
            {
                case "back-to-makermulti-3char-details":
                    _ = MakerMulti_Char_Details_Pt2_Menu.MakerMulti_3Char_Details_Main(menuSession);
                    break;

                case "back-to-makermulti-4char-details":
                    _ = MakerMulti_Char_Details_Pt2_Menu.MakerMulti_4Char_Details_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }
    }
}
