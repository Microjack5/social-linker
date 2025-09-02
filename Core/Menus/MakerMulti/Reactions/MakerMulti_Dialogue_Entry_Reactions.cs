using Discord.WebSocket;
using SocialLinker.Core.CloudStorageTables;
using SocialLinker.Core.Menus.MakerMulti.Main;
using System.Linq;
using System.Threading.Tasks;

namespace SocialLinker.Core.Menus.MakerMulti.Reactions
{
    class MakerMulti_Dialogue_Entry_Reactions
    {
        public static Task Nav_MakerMulti_Dialogue_Entry_Main(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            menuSession.MenuTimer.Stop();

            switch (component.Data.CustomId)
            {
                case "makermulti-dialogue-entry-modal-open":
                    _ = MakerMulti_Dialogue_Entry_Menu.MakerMulti_Dialogue_Details_Modal(component);
                    break;

                case "back-to-makermulti-char-details-pt1":
                    component.DeferAsync(ephemeral: true);
                    _ = MakerMulti_Char_Details_Pt1_Menu.MakerMulti_Char_Details_Pt1_Main(menuSession);
                    break;
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

            switch (multimaker_session.MakerCommand.Template)
            {
                case "P2IS-PS1":
                    menuSession.MenuTimer.Stop();
                    menuSession.MenuMessage.DeleteAsync();
                    SceneMaker.TemplateRenders.MakerMulti.RenderMultiCharP2IS_PS1 p2is_ps1_render = new SceneMaker.TemplateRenders.MakerMulti.RenderMultiCharP2IS_PS1();
                    _ = p2is_ps1_render.Render_Multi_Character_Scene_P2IS_PS1(multimaker_session);
                    break;

                case "P2IS-PSP":
                    menuSession.MenuTimer.Stop();
                    menuSession.MenuMessage.DeleteAsync();
                    SceneMaker.TemplateRenders.MakerMulti.RenderMultiCharP2IS_PSP p2is_psp_render = new SceneMaker.TemplateRenders.MakerMulti.RenderMultiCharP2IS_PSP();
                    _ = p2is_psp_render.Render_Multi_Character_Scene_P2IS_PSP(multimaker_session);
                    break;

                case "P2EP-PS1":
                    menuSession.MenuTimer.Stop();
                    menuSession.MenuMessage.DeleteAsync();
                    SceneMaker.TemplateRenders.MakerMulti.RenderMultiCharP2EP_PS1 p2ep_ps1_render = new SceneMaker.TemplateRenders.MakerMulti.RenderMultiCharP2EP_PS1();
                    _ = p2ep_ps1_render.Render_Multi_Character_Scene_P2EP_PS1(multimaker_session);
                    break;

                case "P2EP-PSP":
                    menuSession.MenuTimer.Stop();
                    menuSession.MenuMessage.DeleteAsync();
                    SceneMaker.TemplateRenders.MakerMulti.RenderMultiCharP2EP_PSP p2ep_psp_render = new SceneMaker.TemplateRenders.MakerMulti.RenderMultiCharP2EP_PSP();
                    _ = p2ep_psp_render.Render_Multi_Character_Scene_P2EP_PSP(multimaker_session);
                    break;

                case "P3P":
                    menuSession.MenuTimer.Stop();
                    menuSession.MenuMessage.DeleteAsync();
                    SceneMaker.TemplateRenders.MakerMulti.RenderMultiCharP3P p3p_render = new SceneMaker.TemplateRenders.MakerMulti.RenderMultiCharP3P();
                    _ = p3p_render.Render_Multi_Character_Scene_P3P(multimaker_session);
                    break;

                case "P4AU":
                    _ = MakerMulti_P4AU_Protag_Highlight_Menu.MakerMulti_P4AU_Protag_Highlight_Main(menuSession);
                    break;

                case "P4D":
                    menuSession.MenuTimer.Stop();
                    menuSession.MenuMessage.DeleteAsync();
                    SceneMaker.TemplateRenders.MakerMulti.RenderMultiCharP4D p4d_render = new SceneMaker.TemplateRenders.MakerMulti.RenderMultiCharP4D();
                    _ = p4d_render.Render_Multi_Character_Scene_P4D(multimaker_session);
                    break;

                case "BBTAG":
                    menuSession.MenuTimer.Stop();
                    menuSession.MenuMessage.DeleteAsync();
                    SceneMaker.TemplateRenders.MakerMulti.RenderMultiCharBBTAG bbtag_render = new SceneMaker.TemplateRenders.MakerMulti.RenderMultiCharBBTAG();
                    _ = bbtag_render.Render_Multi_Character_Scene_BBTAG(multimaker_session);
                    break;
            }

            return Task.CompletedTask;
        }
    }
}
