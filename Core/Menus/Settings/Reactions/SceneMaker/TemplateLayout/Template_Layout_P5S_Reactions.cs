using Discord.WebSocket;
using SocialLinker.Core.CloudStorageTables;
using SocialLinker.Core.Menus.Settings.Main.SceneMaker;
using SocialLinker.Core.Menus.Settings.Main.SceneMaker.TemplateLayout;
using System.Linq;
using System.Threading.Tasks;

namespace SocialLinker.Core.Menus.Settings.Reactions.SceneMaker.TemplateLayout
{
    class Template_Layout_P5S_Reactions
    {
        public static Task Nav_Template_Layout_P5S_Main(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            string selected = component.Data.Values.First();

            switch (selected)
            {
                case "1":
                    _ = Template_Layout_P5S_Menu.Template_Layout_P5S_Controller_Type(menuSession);
                    break;
                case "2":
                    _ = Template_Layout_P5S_Menu.Template_Layout_P5S_Skip_Button(menuSession);
                    break;
                case "3":
                    _ = Template_Layout_P5S_Menu.Template_Layout_P5S_Auto_Advance(menuSession);
                    break;
                case "4":
                    _ = Template_Layout_P5S_Menu.Template_Layout_P5S_Scene_Border(menuSession);
                    break;
                case "5":
                    _ = Template_Layout_P5S_Menu.Template_Layout_P5S_Date_Location_Layout(menuSession);
                    break;
                case "6":
                    _ = Template_Layout_P5S_Menu.Template_Layout_P5S_Location_Icon(menuSession);
                    break;
                case "7":
                    _ = Template_Layout_P5S_Menu.Template_Layout_P5S_Watermark(menuSession);
                    break;
                case "return":
                    _ = Template_Layout_Menu.Template_Layout_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P5S_Controller_Type(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            string selected = component.Data.Values.First();
            var account = menuSession.Account;

            switch (selected)
            {
                case "1":
                    account.P5S_TS_Controller_Type = "PlayStation® 4";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P5S_Menu.Template_Layout_P5S_Controller_Type_Confirm(menuSession);
                    break;
                case "2":
                    account.P5S_TS_Controller_Type = "Nintendo Switch";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P5S_Menu.Template_Layout_P5S_Controller_Type_Confirm(menuSession);
                    break;
                case "3":
                    account.P5S_TS_Controller_Type = "Xbox One";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P5S_Menu.Template_Layout_P5S_Controller_Type_Confirm(menuSession);
                    break;
                case "4":
                    account.P5S_TS_Controller_Type = "Keyboard";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P5S_Menu.Template_Layout_P5S_Controller_Type_Confirm(menuSession);
                    break;
                case "return":
                    _ = Template_Layout_P5S_Menu.Template_Layout_P5S_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P5S_Skip_Button(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            var account = menuSession.Account;

            switch (component.Data.CustomId)
            {
                case "return":
                    _ = Template_Layout_P5S_Menu.Template_Layout_P5S_Main(menuSession);
                    break;

                case "on":
                    account.P5S_TS_Skip_Button = "On";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P5S_Menu.Template_Layout_P5S_Skip_Button_Confirm(menuSession);
                    break;

                case "off":
                    account.P5S_TS_Skip_Button = "Off";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P5S_Menu.Template_Layout_P5S_Skip_Button_Confirm(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P5S_Auto_Advance(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            var account = menuSession.Account;

            switch (component.Data.CustomId)
            {
                case "return":
                    _ = Template_Layout_P5S_Menu.Template_Layout_P5S_Main(menuSession);
                    break;

                case "on":
                    account.P5S_TS_Auto_Advance = "On";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P5S_Menu.Template_Layout_P5S_Auto_Advance_Confirm(menuSession);
                    break;

                case "off":
                    account.P5S_TS_Auto_Advance = "Off";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P5S_Menu.Template_Layout_P5S_Auto_Advance_Confirm(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P5S_Scene_Border(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            var account = menuSession.Account;

            switch (component.Data.CustomId)
            {
                case "return":
                    _ = Template_Layout_P5S_Menu.Template_Layout_P5S_Main(menuSession);
                    break;

                case "on":
                    account.P5S_TS_Scene_Border = "On";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P5S_Menu.Template_Layout_P5S_Scene_Border_Confirm(menuSession);
                    break;

                case "off":
                    account.P5S_TS_Scene_Border = "Off";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P5S_Menu.Template_Layout_P5S_Scene_Border_Confirm(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P5S_Date_Location_Layout(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            string selected = component.Data.Values.First();
            var account = menuSession.Account;

            switch (selected)
            {
                case "1":
                    account.P5S_TS_Date_Location_Layout = "Display All";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P5S_Menu.Template_Layout_P5S_Date_Location_Layout_Confirm(menuSession);
                    break;
                case "2":
                    account.P5S_TS_Date_Location_Layout = "Date Only";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P5S_Menu.Template_Layout_P5S_Date_Location_Layout_Confirm(menuSession);
                    break;
                case "3":
                    account.P5S_TS_Date_Location_Layout = "None";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P5S_Menu.Template_Layout_P5S_Date_Location_Layout_Confirm(menuSession);
                    break;
                case "return":
                    _ = Template_Layout_P5S_Menu.Template_Layout_P5S_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P5S_Location_Icon(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            string selected = component.Data.Values.First();
            var account = menuSession.Account;

            switch (selected)
            {
                case "1":
                    account.P5S_TS_Location_Icon = "Yongen-Jaya";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P5S_Menu.Template_Layout_P5S_Location_Icon_Confirm(menuSession);
                    break;
                case "2":
                    account.P5S_TS_Location_Icon = "Shibuya";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P5S_Menu.Template_Layout_P5S_Location_Icon_Confirm(menuSession);
                    break;
                case "3":
                    account.P5S_TS_Location_Icon = "Sendai";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P5S_Menu.Template_Layout_P5S_Location_Icon_Confirm(menuSession);
                    break;
                case "4":
                    account.P5S_TS_Location_Icon = "Sapporo";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P5S_Menu.Template_Layout_P5S_Location_Icon_Confirm(menuSession);
                    break;
                case "5":
                    account.P5S_TS_Location_Icon = "Okinawa";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P5S_Menu.Template_Layout_P5S_Location_Icon_Confirm(menuSession);
                    break;
                case "6":
                    account.P5S_TS_Location_Icon = "Fukuoka";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P5S_Menu.Template_Layout_P5S_Location_Icon_Confirm(menuSession);
                    break;
                case "7":
                    account.P5S_TS_Location_Icon = "Kyoto";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P5S_Menu.Template_Layout_P5S_Location_Icon_Confirm(menuSession);
                    break;
                case "8":
                    account.P5S_TS_Location_Icon = "Osaka";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P5S_Menu.Template_Layout_P5S_Location_Icon_Confirm(menuSession);
                    break;
                case "9":
                    account.P5S_TS_Location_Icon = "Yokohama";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P5S_Menu.Template_Layout_P5S_Location_Icon_Confirm(menuSession);
                    break;
                case "10":
                    account.P5S_TS_Location_Icon = "Shiba Park";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P5S_Menu.Template_Layout_P5S_Location_Icon_Confirm(menuSession);
                    break;
                case "car":
                    account.P5S_TS_Location_Icon = "RV Travel";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P5S_Menu.Template_Layout_P5S_Location_Icon_Confirm(menuSession);
                    break;
                case "return":
                    _ = Template_Layout_P5S_Menu.Template_Layout_P5S_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P5S_Watermark(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            var account = menuSession.Account;

            switch (component.Data.CustomId)
            {
                case "return":
                    _ = Template_Layout_P5S_Menu.Template_Layout_P5S_Main(menuSession);
                    break;

                case "on":
                    account.P5S_TS_Watermark = "On";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P5S_Menu.Template_Layout_P5S_Watermark_Confirm(menuSession);
                    break;

                case "off":
                    account.P5S_TS_Watermark = "Off";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Template_Layout_P5S_Menu.Template_Layout_P5S_Watermark_Confirm(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P5S_Controller_Type_Confirm(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "back-to-p5s-template-settings":
                    _ = Template_Layout_P5S_Menu.Template_Layout_P5S_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P5S_Skip_Button_Confirm(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "back-to-p5s-template-settings":
                    _ = Template_Layout_P5S_Menu.Template_Layout_P5S_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P5S_Auto_Advance_Confirm(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "back-to-p5s-template-settings":
                    _ = Template_Layout_P5S_Menu.Template_Layout_P5S_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P5S_Scene_Border_Confirm(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "back-to-p5s-template-settings":
                    _ = Template_Layout_P5S_Menu.Template_Layout_P5S_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P5S_Date_Location_Layout_Confirm(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "back-to-p5s-template-settings":
                    _ = Template_Layout_P5S_Menu.Template_Layout_P5S_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P5S_Location_Icon_Confirm(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "back-to-p5s-template-settings":
                    _ = Template_Layout_P5S_Menu.Template_Layout_P5S_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Template_Layout_P5S_Watermark_Confirm(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "back-to-p5s-template-settings":
                    _ = Template_Layout_P5S_Menu.Template_Layout_P5S_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }
    }
}
