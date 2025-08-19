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
    class MakerMulti_Dialogue_Entry_Reactions
    {
        public static Task Nav_MakerMulti_Dialogue_Entry_Main(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            if (component.Data.CustomId == "makermulti-dialogue-entry-modal-open")
            {
                // Go to a new menu.
                _ = MakerMulti_Dialogue_Entry_Menu.MakerMulti_Dialogue_Details_Modal(component);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_MakerMulti_Dialogue_Details_Modal(SocketModal modal, MenuIdStructure menuSession)
        {
            var multimaker_session = Global.MultiMaker_Session_List.SingleOrDefault(x => x.User.Id == menuSession.User.Id);
            var account = UserInfoClasses.GetAccount(menuSession.User);

            multimaker_session.MakerCommand.Display_Name = modal.Data.Components
            .FirstOrDefault(x => x.CustomId == "display_name")?.Value;

            multimaker_session.MakerCommand.Dialogue = modal.Data.Components
            .FirstOrDefault(x => x.CustomId == "dialogue")?.Value;

            modal.DeferAsync(ephemeral: true);

            menuSession.MenuMessage.DeleteAsync();

            switch (multimaker_session.MakerCommand.Template)
            {
                case "P3P":
                    SceneMaker.TemplateRenders.MakerMulti.RenderMultiCharP3P p3p_render = new SceneMaker.TemplateRenders.MakerMulti.RenderMultiCharP3P();
                    _ = p3p_render.Render_Multi_Character_Scene_P3P(multimaker_session);
                    break;

                case "P4AU":
                    SceneMaker.TemplateRenders.MakerMulti.RenderMultiCharP4AU p4au_render = new SceneMaker.TemplateRenders.MakerMulti.RenderMultiCharP4AU();
                    _ = p4au_render.Render_Multi_Character_Scene_P4AU(multimaker_session);
                    break;

                case "P4D":
                    SceneMaker.TemplateRenders.MakerMulti.RenderMultiCharP4D p4d_render = new SceneMaker.TemplateRenders.MakerMulti.RenderMultiCharP4D();
                    _ = p4d_render.Render_Multi_Character_Scene_P4D(multimaker_session);
                    break;

                case "BBTAG":
                    SceneMaker.TemplateRenders.MakerMulti.RenderMultiCharBBTAG bbtag_render = new SceneMaker.TemplateRenders.MakerMulti.RenderMultiCharBBTAG();
                    _ = bbtag_render.Render_Multi_Character_Scene_BBTAG(multimaker_session);
                    break;
            }

            return Task.CompletedTask;
        }
    }
}
