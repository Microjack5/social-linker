using Discord.WebSocket;
using SocialLinker.Core.Menus.Settings.Main.SceneMaker;
using SocialLinker.Core.Menus.Settings.Main.SceneMaker.TemplateLayout;
using System.Linq;
using System.Threading.Tasks;

namespace SocialLinker.Core.Menus.Settings.Reactions.SceneMaker
{
    class Template_Layout_Reactions
    {
        public static Task Nav_Template_Layout_Main(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            string selected = component.Data.Values.First();

            switch (selected)
            {
                case "P1":
                    _ = Template_Layout_VC_Menu.Template_Layout_VC_P1_Main(menuSession);
                    break;
                case "P2IS":
                    _ = Template_Layout_VC_Menu.Template_Layout_VC_P2IS_Main(menuSession);
                    break;
                case "P2EP":
                    _ = Template_Layout_VC_Menu.Template_Layout_VC_P2EP_Main(menuSession);
                    break;
                case "P3":
                    _ = Template_Layout_VC_Menu.Template_Layout_VC_P3_Main(menuSession);
                    break;
                case "P4":
                    _ = Template_Layout_VC_Menu.Template_Layout_VC_P4_Main(menuSession);
                    break;
                case "P4AU":
                    _ = Template_Layout_P4AU_Menu.Template_Layout_P4AU_Main(menuSession.User, menuSession.MenuMessage);
                    break;
                case "P4D":
                    _ = Template_Layout_P4D_Menu.Template_Layout_P4D_Main(menuSession);
                    break;
                case "P5":
                    _ = Template_Layout_VC_Menu.Template_Layout_VC_P5_Main(menuSession);
                    break;
                case "P5S":
                    _ = Template_Layout_P5S_Menu.Template_Layout_P5S_Main(menuSession);
                    break;
                case "BBTAG":
                    _ = Template_Layout_BBTAG_Menu.Template_Layout_BBTAG_Main(menuSession.User, menuSession.MenuMessage);
                    break;
                case "return":
                    _ = SM_Settings_Menu.SM_Settings_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }
    }
}
