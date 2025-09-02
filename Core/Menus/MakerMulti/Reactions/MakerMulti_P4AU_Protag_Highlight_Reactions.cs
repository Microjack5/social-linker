using Discord;
using Discord.WebSocket;
using SocialLinker.Core.CloudStorageTables;
using System.Linq;
using System.Threading.Tasks;

namespace SocialLinker.Core.Menus.MakerMulti.Reactions
{
    class MakerMulti_P4AU_Protag_Highlight_Reactions
    {
        public static Task Nav_MakerMulti_P4AU_Protag_Highlight_Main(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            var multimaker_session = Global.MultiMaker_Session_List.SingleOrDefault(x => x.User.Id == menuSession.User.Id);
            var account = UserInfoClasses.GetAccount(menuSession.User);

            switch (component.Data.CustomId)
            {
                case "makermulti-p4au-protag-highlight-on":
                    multimaker_session.MakerCommand.P4AU_Multi_Char_Protag_Highlight_Toggle = true;
                    break;

                case "makermulti-p4au-protag-highlight-off":
                    multimaker_session.MakerCommand.P4AU_Multi_Char_Protag_Highlight_Toggle = false;
                    break;
            }

            menuSession.MenuMessage.DeleteAsync();
            SceneMaker.TemplateRenders.MakerMulti.RenderMultiCharP4AU p4au_render = new SceneMaker.TemplateRenders.MakerMulti.RenderMultiCharP4AU();
            _ = p4au_render.Render_Multi_Character_Scene_P4AU(multimaker_session);

            return Task.CompletedTask;
        }
    }
}
