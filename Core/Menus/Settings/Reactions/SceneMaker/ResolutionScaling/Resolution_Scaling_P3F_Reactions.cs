using Discord.WebSocket;
using SocialLinker.Core.CloudStorageTables;
using SocialLinker.Core.Menus.Settings.Main.SceneMaker.ResolutionScaling;
using SocialLinker.Core.Menus.Settings.Main.SceneMaker.TemplateLayout;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace SocialLinker.Core.Menus.Settings.Reactions.SceneMaker.ResolutionScaling
{
    class Resolution_Scaling_P3F_Reactions
    {
        public static Task Nav_Resolution_Scaling_P3F_Main(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            string selected = component.Data.Values.First();

            switch (selected)
            {
                case "1":
                    _ = Resolution_Scaling_P3F_Menu.Resolution_Scaling_P3F_Output_Resolution(menuSession);
                    break;
                case "2":
                    _ = Resolution_Scaling_P3F_Menu.Resolution_Scaling_P3F_Scaling_Method(menuSession);
                    break;
                case "return":
                    _ = Resolution_Scaling_VC_Menu.Resolution_Scaling_VC_P3_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Resolution_Scaling_P3F_Output_Resolution(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            string selected = component.Data.Values.First();
            var account = menuSession.Account;

            switch (selected)
            {
                case "1":
                    account.P3F_Resolution = "640 × 448";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Resolution_Scaling_P3F_Menu.Resolution_Scaling_P3F_Output_Resolution_Confirm(menuSession);
                    break;
                case "2":
                    account.P3F_Resolution = "640 × 480";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Resolution_Scaling_P3F_Menu.Resolution_Scaling_P3F_Output_Resolution_Confirm(menuSession);
                    break;
                case "3":
                    account.P3F_Resolution = "1440 × 1080";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Resolution_Scaling_P3F_Menu.Resolution_Scaling_P3F_Output_Resolution_Confirm(menuSession);
                    break;
                case "return":
                    _ = Resolution_Scaling_P3F_Menu.Resolution_Scaling_P3F_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Resolution_Scaling_P3F_Scaling_Method(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            string selected = component.Data.Values.First();
            var account = menuSession.Account;

            switch (selected)
            {
                case "1":
                    account.P3F_Scale = "Bicubic";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Resolution_Scaling_P3F_Menu.Resolution_Scaling_P3F_Scaling_Method_Confirm(menuSession);
                    break;
                case "2":
                    account.P3F_Scale = "Nearest Neighbor";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Resolution_Scaling_P3F_Menu.Resolution_Scaling_P3F_Scaling_Method_Confirm(menuSession);
                    break;
                case "return":
                    _ = Resolution_Scaling_P3F_Menu.Resolution_Scaling_P3F_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Resolution_Scaling_P3F_Output_Resolution_Confirm(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "back-to-p3f-resolution-scaling":
                    _ = Resolution_Scaling_P3F_Menu.Resolution_Scaling_P3F_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Resolution_Scaling_P3F_Scaling_Method_Confirm(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "back-to-p3f-resolution-scaling":
                    _ = Resolution_Scaling_P3F_Menu.Resolution_Scaling_P3F_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }
    }
}
