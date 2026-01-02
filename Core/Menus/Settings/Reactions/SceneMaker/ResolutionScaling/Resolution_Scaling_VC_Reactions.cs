using Discord.WebSocket;
using SocialLinker.Core.Menus.Settings.Main.SceneMaker.ResolutionScaling;
using System.Linq;
using System.Threading.Tasks;

namespace SocialLinker.Core.Menus.Settings.Reactions.SceneMaker.ResolutionScaling
{
    class Resolution_Scaling_VC_Reactions
    {
        public static Task Nav_Resolution_Scaling_VC_P1_Main(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            string selected = component.Data.Values.First();
            var account = menuSession.Account;

            switch (selected)
            {
                case "P1-PS1":
                    _ = Resolution_Scaling_P1_PS1_Menu.Resolution_Scaling_P1_PS1_Main(menuSession);
                    break;
                case "P1-PSP":
                    _ = Resolution_Scaling_P1_PSP_Menu.Resolution_Scaling_P1_PSP_Main(menuSession);
                    break;
                case "return":
                    _ = Resolution_Scaling_Menu.Resolution_Scaling_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Resolution_Scaling_VC_P2IS_Main(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            string selected = component.Data.Values.First();
            var account = menuSession.Account;

            switch (selected)
            {
                case "P2IS-PS1":
                    _ = Resolution_Scaling_P2IS_PS1_Menu.Resolution_Scaling_P2IS_PS1_Main(menuSession);
                    break;
                case "P2IS-PSP":
                    _ = Resolution_Scaling_P2IS_PSP_Menu.Resolution_Scaling_P2IS_PSP_Main(menuSession);
                    break;
                case "return":
                    _ = Resolution_Scaling_Menu.Resolution_Scaling_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Resolution_Scaling_VC_P2EP_Main(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            string selected = component.Data.Values.First();
            var account = menuSession.Account;

            switch (selected)
            {
                case "P2EP-PS1":
                    _ = Resolution_Scaling_P2EP_PS1_Menu.Resolution_Scaling_P2EP_PS1_Main(menuSession);
                    break;
                case "P2EP-PSP":
                    _ = Resolution_Scaling_P2EP_PSP_Menu.Resolution_Scaling_P2EP_PSP_Main(menuSession);
                    break;
                case "return":
                    _ = Resolution_Scaling_Menu.Resolution_Scaling_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Resolution_Scaling_VC_P3_Main(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            string selected = component.Data.Values.First();
            var account = menuSession.Account;

            switch (selected)
            {
                case "P3F":
                    _ = Resolution_Scaling_P3F_Menu.Resolution_Scaling_P3F_Main(menuSession);
                    break;
                case "P3P":
                    _ = Resolution_Scaling_P3P_Menu.Resolution_Scaling_P3P_Main(menuSession);
                    break;
                case "return":
                    _ = Resolution_Scaling_Menu.Resolution_Scaling_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }
    }
}
