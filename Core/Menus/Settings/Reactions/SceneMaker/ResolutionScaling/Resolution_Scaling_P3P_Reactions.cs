using Discord.WebSocket;
using SocialLinker.Core.CloudStorageTables;
using SocialLinker.Core.Menus.Settings.Main.SceneMaker.ResolutionScaling;
using System.Linq;
using System.Threading.Tasks;

namespace SocialLinker.Core.Menus.Settings.Reactions.SceneMaker.ResolutionScaling
{
    class Resolution_Scaling_P3P_Reactions
    {
        public static Task Nav_Resolution_Scaling_P3P_Main(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            string selected = component.Data.Values.First();

            switch (selected)
            {
                case "1":
                    _ = Resolution_Scaling_P3P_Menu.Resolution_Scaling_P3P_Output_Resolution(menuSession);
                    break;
                case "2":
                    _ = Resolution_Scaling_P3P_Menu.Resolution_Scaling_P3P_Scaling_Method(menuSession);
                    break;
                case "return":
                    _ = Resolution_Scaling_VC_Menu.Resolution_Scaling_VC_P3_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Resolution_Scaling_P3P_Output_Resolution(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            var account = menuSession.Account;

            switch (component.Data.CustomId)
            {
                case "return":
                    _ = Resolution_Scaling_P3P_Menu.Resolution_Scaling_P3P_Main(menuSession);
                    break;

                case "1":
                    account.P3P_Resolution = "480 × 272";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Resolution_Scaling_P3P_Menu.Resolution_Scaling_P3P_Output_Resolution_Confirm(menuSession);
                    break;

                case "2":
                    account.P3P_Resolution = "1920 × 1088";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Resolution_Scaling_P3P_Menu.Resolution_Scaling_P3P_Output_Resolution_Confirm(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Resolution_Scaling_P3P_Scaling_Method(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            var account = menuSession.Account;

            switch (component.Data.CustomId)
            {
                case "return":
                    _ = Resolution_Scaling_P3P_Menu.Resolution_Scaling_P3P_Main(menuSession);
                    break;

                case "1":
                    account.P3P_Scale = "Bicubic";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Resolution_Scaling_P3P_Menu.Resolution_Scaling_P3P_Scaling_Method_Confirm(menuSession);
                    break;

                case "2":
                    account.P3P_Scale = "Nearest Neighbor";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Resolution_Scaling_P3P_Menu.Resolution_Scaling_P3P_Scaling_Method_Confirm(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Resolution_Scaling_P3P_Output_Resolution_Confirm(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "back-to-p3p-resolution-scaling":
                    _ = Resolution_Scaling_P3P_Menu.Resolution_Scaling_P3P_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Resolution_Scaling_P3P_Scaling_Method_Confirm(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "back-to-p3p-resolution-scaling":
                    _ = Resolution_Scaling_P3P_Menu.Resolution_Scaling_P3P_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }
    }
}
