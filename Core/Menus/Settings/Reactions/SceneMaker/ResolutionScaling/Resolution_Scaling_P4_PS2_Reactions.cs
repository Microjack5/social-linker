using Discord.WebSocket;
using SocialLinker.Core.CloudStorageTables;
using SocialLinker.Core.Menus.Settings.Main.SceneMaker.ResolutionScaling;
using System.Linq;
using System.Threading.Tasks;

namespace SocialLinker.Core.Menus.Settings.Reactions.SceneMaker.ResolutionScaling
{
    class Resolution_Scaling_P4_PS2_Reactions
    {
        public static Task Nav_Resolution_Scaling_P4_PS2_Main(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            string selected = component.Data.Values.First();

            switch (selected)
            {
                case "1":
                    _ = Resolution_Scaling_P4_PS2_Menu.Resolution_Scaling_P4_PS2_Output_Resolution(menuSession);
                    break;
                case "2":
                    _ = Resolution_Scaling_P4_PS2_Menu.Resolution_Scaling_P4_PS2_Scaling_Method(menuSession);
                    break;
                case "return":
                    _ = Resolution_Scaling_Menu.Resolution_Scaling_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Resolution_Scaling_P4_PS2_Output_Resolution(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            string selected = component.Data.Values.First();
            var account = menuSession.Account;

            switch (selected)
            {
                case "1":
                    account.P4_PS2_Resolution = "640 × 448";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Resolution_Scaling_P4_PS2_Menu.Resolution_Scaling_P4_PS2_Output_Resolution_Confirm(menuSession);
                    break;
                case "2":
                    account.P4_PS2_Resolution = "640 × 480";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Resolution_Scaling_P4_PS2_Menu.Resolution_Scaling_P4_PS2_Output_Resolution_Confirm(menuSession);
                    break;
                case "3":
                    account.P4_PS2_Resolution = "1440 × 1080";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Resolution_Scaling_P4_PS2_Menu.Resolution_Scaling_P4_PS2_Output_Resolution_Confirm(menuSession);
                    break;
                case "return":
                    _ = Resolution_Scaling_P4_PS2_Menu.Resolution_Scaling_P4_PS2_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Resolution_Scaling_P4_PS2_Scaling_Method(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            string selected = component.Data.Values.First();
            var account = menuSession.Account;

            switch (selected)
            {
                case "1":
                    account.P4_PS2_Scale = "Bicubic";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Resolution_Scaling_P4_PS2_Menu.Resolution_Scaling_P4_PS2_Scaling_Method_Confirm(menuSession);
                    break;
                case "2":
                    account.P4_PS2_Scale = "Nearest Neighbor";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Resolution_Scaling_P4_PS2_Menu.Resolution_Scaling_P4_PS2_Scaling_Method_Confirm(menuSession);
                    break;
                case "return":
                    _ = Resolution_Scaling_P4_PS2_Menu.Resolution_Scaling_P4_PS2_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Resolution_Scaling_P4_PS2_Output_Resolution_Confirm(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "back-to-p4-ps2-resolution-scaling":
                    _ = Resolution_Scaling_P4_PS2_Menu.Resolution_Scaling_P4_PS2_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Resolution_Scaling_P4_PS2_Scaling_Method_Confirm(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "back-to-p4-ps2-resolution-scaling":
                    _ = Resolution_Scaling_P4_PS2_Menu.Resolution_Scaling_P4_PS2_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }
    }
}
