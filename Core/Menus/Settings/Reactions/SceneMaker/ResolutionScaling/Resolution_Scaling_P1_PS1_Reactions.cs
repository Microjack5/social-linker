using Discord.WebSocket;
using SocialLinker.Core.CloudStorageTables;
using SocialLinker.Core.Menus.Settings.Main.SceneMaker.ResolutionScaling;
using System.Linq;
using System.Threading.Tasks;

namespace SocialLinker.Core.Menus.Settings.Reactions.SceneMaker.ResolutionScaling
{
    class Resolution_Scaling_P1_PS1_Reactions
    {
        public static Task Nav_Resolution_Scaling_P1_PS1_Main(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            string selected = component.Data.Values.First();
            var account = menuSession.Account;

            switch (selected)
            {
                case "1":
                    _ = Resolution_Scaling_P1_PS1_Menu.Resolution_Scaling_P1_PS1_Output_Resolution(menuSession);
                    break;
                case "2":
                    _ = Resolution_Scaling_P1_PS1_Menu.Resolution_Scaling_P1_PS1_Scaling_Method(menuSession);
                    break;
                case "return":
                    _ = Resolution_Scaling_VC_Menu.Resolution_Scaling_VC_P1_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Resolution_Scaling_P1_PS1_Output_Resolution(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            string selected = component.Data.Values.First();
            var account = menuSession.Account;

            switch (selected)
            {
                case "1":
                    account.P1_PSX_Resolution = "320 × 240";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Resolution_Scaling_P1_PS1_Menu.Resolution_Scaling_P1_PS1_Output_Resolution_Confirm(menuSession);
                    break;
                case "2":
                    account.P1_PSX_Resolution = "1440 × 1080";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Resolution_Scaling_P1_PS1_Menu.Resolution_Scaling_P1_PS1_Output_Resolution_Confirm(menuSession);
                    break;
                case "return":
                    _ = Resolution_Scaling_P1_PS1_Menu.Resolution_Scaling_P1_PS1_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Resolution_Scaling_P1_PS1_Scaling_Method(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            string selected = component.Data.Values.First();
            var account = menuSession.Account;

            switch (selected)
            {
                case "1":
                    account.P1_PSX_Scale = "Bicubic";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Resolution_Scaling_P1_PS1_Menu.Resolution_Scaling_P1_PS1_Scaling_Method_Confirm(menuSession);
                    break;
                case "2":
                    account.P1_PSX_Scale = "Nearest Neighbor";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Resolution_Scaling_P1_PS1_Menu.Resolution_Scaling_P1_PS1_Scaling_Method_Confirm(menuSession);
                    break;
                case "return":
                    _ = Resolution_Scaling_P1_PS1_Menu.Resolution_Scaling_P1_PS1_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Resolution_Scaling_P1_PS1_Output_Resolution_Confirm(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "back-to-p1-ps1-resolution-scaling":
                    _ = Resolution_Scaling_P1_PS1_Menu.Resolution_Scaling_P1_PS1_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Resolution_Scaling_P1_PS1_Scaling_Method_Confirm(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "back-to-p1-ps1-resolution-scaling":
                    _ = Resolution_Scaling_P1_PS1_Menu.Resolution_Scaling_P1_PS1_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }
    }
}
