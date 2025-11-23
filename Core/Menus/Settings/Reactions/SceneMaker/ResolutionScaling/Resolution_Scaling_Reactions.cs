using Discord.WebSocket;
using SocialLinker.Core.Menus.Settings.Main.SceneMaker;
using SocialLinker.Core.Menus.Settings.Main.SceneMaker.ResolutionScaling;
using System.Linq;
using System.Threading.Tasks;

namespace SocialLinker.Core.Menus.Settings.Reactions.SceneMaker.ResolutionScaling
{
    class Resolution_Scaling_Reactions
    {
        public static Task Nav_Resolution_Scaling_Main(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            string selected = component.Data.Values.First();

            switch (selected)
            {
                case "P1":
                    _ = Resolution_Scaling_VC_Menu.Resolution_Scaling_VC_P1_Main(menuSession);
                    break;
                case "P2IS":
                    _ = Resolution_Scaling_VC_Menu.Resolution_Scaling_VC_P2IS_Main(menuSession);
                    break;
                case "P2EP":
                    _ = Resolution_Scaling_VC_Menu.Resolution_Scaling_VC_P2EP_Main(menuSession);
                    break;
                case "P3":
                    _ = Resolution_Scaling_VC_Menu.Resolution_Scaling_VC_P3_Main(menuSession);
                    break;
                case "P4":
                    _ = Resolution_Scaling_P4_PS2_Menu.Resolution_Scaling_P4_PS2_Main(menuSession);
                    break;
                case "P4AU":
                    _ = Resolution_Scaling_P4AU_Menu.Resolution_Scaling_P4AU_Main(menuSession);
                    break;
                case "return":
                    _ = SM_Settings_Menu.SM_Settings_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }
    }
}
