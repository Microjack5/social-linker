using Discord.WebSocket;
using SocialLinker.Core.CloudStorageTables;
using SocialLinker.Core.Menus.Settings.Main.SceneMaker;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SocialLinker.Core.Menus.Settings.Reactions.SceneMaker
{
    class Version_Control_Reactions
    {
        public static Task Nav_Version_Control_Main(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            string selected = component.Data.Values.First();

            switch (selected)
            {
                case "p1":
                    _ = Version_Control_Menu.Version_Control_P1(menuSession);
                    break;
                case "p2is":
                    _ = Version_Control_Menu.Version_Control_P2IS(menuSession);
                    break;
                case "p2ep":
                    _ = Version_Control_Menu.Version_Control_P2EP(menuSession);
                    break;
                case "p3":
                    _ = Version_Control_Menu.Version_Control_P3(menuSession);
                    break;
                case "p4":
                    _ = Version_Control_Menu.Version_Control_P4(menuSession);
                    break;
                case "p5":
                    _ = Version_Control_Menu.Version_Control_P5(menuSession);
                    break;
                case "return":
                    _ = SM_Settings_Menu.SM_Settings_Main(menuSession.User, menuSession.MenuMessage);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Version_Control_P1(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            string selected = component.Data.Values.First();
            var account = menuSession.Account;

            switch (selected)
            {
                case "p1-ps1":
                    account.VC_P1 = "P1-PS1";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Version_Control_Menu.Version_Control_P1_Confirm(menuSession);
                    break;
                case "p1-psp":
                    account.VC_P1 = "P1-PSP";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Version_Control_Menu.Version_Control_P1_Confirm(menuSession);
                    break;
                case "return":
                    _ = Version_Control_Menu.Version_Control_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Version_Control_P2IS(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            string selected = component.Data.Values.First();
            var account = menuSession.Account;

            switch (selected)
            {
                case "p2is-ps1":
                    account.VC_P2IS = "P2IS-PS1";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Version_Control_Menu.Version_Control_P2IS_Confirm(menuSession);
                    break;
                case "p2is-psp":
                    account.VC_P2IS = "P2IS-PSP";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Version_Control_Menu.Version_Control_P2IS_Confirm(menuSession);
                    break;
                case "return":
                    _ = Version_Control_Menu.Version_Control_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Version_Control_P2EP(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            string selected = component.Data.Values.First();
            var account = menuSession.Account;

            switch (selected)
            {
                case "p2ep-ps1":
                    account.VC_P2EP = "P2EP-PS1";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Version_Control_Menu.Version_Control_P2EP_Confirm(menuSession);
                    break;
                case "p2ep-psp":
                    account.VC_P2EP = "P2EP-PSP";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Version_Control_Menu.Version_Control_P2EP_Confirm(menuSession);
                    break;
                case "return":
                    _ = Version_Control_Menu.Version_Control_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Version_Control_P3(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            string selected = component.Data.Values.First();
            var account = menuSession.Account;

            switch (selected)
            {
                case "p3f":
                    account.VC_P3 = "P3F";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Version_Control_Menu.Version_Control_P3_Confirm(menuSession);
                    break;
                case "p3p":
                    account.VC_P3 = "P3P";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Version_Control_Menu.Version_Control_P3_Confirm(menuSession);
                    break;
                case "return":
                    _ = Version_Control_Menu.Version_Control_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Version_Control_P4(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            string selected = component.Data.Values.First();
            var account = menuSession.Account;

            switch (selected)
            {
                case "p4-ps2":
                    account.VC_P4 = "P4-PS2";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Version_Control_Menu.Version_Control_P4_Confirm(menuSession);
                    break;
                case "p4g":
                    account.VC_P4 = "P4G";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Version_Control_Menu.Version_Control_P4_Confirm(menuSession);
                    break;
                case "return":
                    _ = Version_Control_Menu.Version_Control_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Version_Control_P5(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            string selected = component.Data.Values.First();
            var account = menuSession.Account;

            switch (selected)
            {
                case "p5-ps4":
                    account.VC_P5 = "P5-PS4";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Version_Control_Menu.Version_Control_P5_Confirm(menuSession);
                    break;
                case "p5r":
                    account.VC_P5 = "P5R";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Version_Control_Menu.Version_Control_P5_Confirm(menuSession);
                    break;
                case "return":
                    _ = Version_Control_Menu.Version_Control_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Version_Control_P1_Confirm(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "return":
                    _ = Version_Control_Menu.Version_Control_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Version_Control_P2IS_Confirm(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "return":
                    _ = Version_Control_Menu.Version_Control_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Version_Control_P2EP_Confirm(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "return":
                    _ = Version_Control_Menu.Version_Control_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Version_Control_P3_Confirm(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "return":
                    _ = Version_Control_Menu.Version_Control_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Version_Control_P4_Confirm(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "return":
                    _ = Version_Control_Menu.Version_Control_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Version_Control_P5_Confirm(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "return":
                    _ = Version_Control_Menu.Version_Control_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }
    }
}
