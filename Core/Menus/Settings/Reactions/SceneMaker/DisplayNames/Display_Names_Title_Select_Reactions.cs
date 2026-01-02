using Discord.WebSocket;
using SocialLinker.Core.Menus.Settings.Main.SceneMaker.DisplayNames;
using System.Linq;
using System.Threading.Tasks;

namespace SocialLinker.Core.Menus.Settings.Reactions.SceneMaker.DisplayNames
{
    internal class Display_Names_Title_Select_Reactions
    {
        public static Task Nav_Display_Names_Title_Select(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            var naming_session = Global.DisplayNameTempList.SingleOrDefault(x => x.User_ID == $"{menuSession.User.Id}");

            string selected = component.Data.Values.First();

            switch (selected)
            {
                case "P1":
                    _ = Display_Names_Title_Select_Menu.Display_Names_Title_Select_VC_P1_Main(menuSession);
                    break;
                case "P2IS":
                    _ = Display_Names_Title_Select_Menu.Display_Names_Title_Select_VC_P2IS_Main(menuSession);
                    break;
                case "P2EP":
                    _ = Display_Names_Title_Select_Menu.Display_Names_Title_Select_VC_P2EP_Main(menuSession);
                    break;
                case "P3":
                    _ = Display_Names_Title_Select_Menu.Display_Names_Title_Select_VC_P3_Main(menuSession);
                    break;
                case "P4":
                    _ = Display_Names_Title_Select_Menu.Display_Names_Title_Select_VC_P4_Main(menuSession);
                    break;
                case "P4AU":
                    naming_session.Game = "P4AU";
                    _ = Display_Names_Character_Select_Menu.Display_Names_Character_Select_Main(menuSession);
                    break;
                case "P4D":
                    naming_session.Game = "P4D";
                    _ = Display_Names_Character_Select_Menu.Display_Names_Character_Select_Main(menuSession);
                    break;
                case "P5":
                    _ = Display_Names_Title_Select_Menu.Display_Names_Title_Select_VC_P5_Main(menuSession);
                    break;
                case "P5S":
                    naming_session.Game = "P5S";
                    _ = Display_Names_Character_Select_Menu.Display_Names_Character_Select_Main(menuSession);
                    break;
                case "BBTAG":
                    naming_session.Game = "BBTAG";
                    _ = Display_Names_Character_Select_Menu.Display_Names_Character_Select_Main(menuSession);
                    break;
                case "return":
                    _ = Display_Names_Menu.Display_Names_Start(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Display_Names_Title_Select_VC_P1_Main(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            var naming_session = Global.DisplayNameTempList.SingleOrDefault(x => x.User_ID == $"{menuSession.User.Id}");

            string selected = component.Data.Values.First();

            switch (selected)
            {
                case "P1-PS1":
                    naming_session.Game = "P1-PS1";
                    _ = Display_Names_Character_Select_Menu.Display_Names_Character_Select_Main(menuSession);
                    break;
                case "P1-PSP":
                    naming_session.Game = "P1-PSP";
                    _ = Display_Names_Character_Select_Menu.Display_Names_Character_Select_Main(menuSession);
                    break;
                case "return":
                    _ = Display_Names_Title_Select_Menu.Display_Names_Title_Select(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Display_Names_Title_Select_VC_P2IS_Main(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            var naming_session = Global.DisplayNameTempList.SingleOrDefault(x => x.User_ID == $"{menuSession.User.Id}");

            string selected = component.Data.Values.First();

            switch (selected)
            {
                case "P2IS-PS1":
                    naming_session.Game = "P2IS-PS1";
                    _ = Display_Names_Character_Select_Menu.Display_Names_Character_Select_Main(menuSession);
                    break;
                case "P2IS-PSP":
                    naming_session.Game = "P2IS-PSP";
                    _ = Display_Names_Character_Select_Menu.Display_Names_Character_Select_Main(menuSession);
                    break;
                case "return":
                    _ = Display_Names_Title_Select_Menu.Display_Names_Title_Select(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Display_Names_Title_Select_VC_P2EP_Main(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            var naming_session = Global.DisplayNameTempList.SingleOrDefault(x => x.User_ID == $"{menuSession.User.Id}");

            string selected = component.Data.Values.First();

            switch (selected)
            {
                case "P2EP-PS1":
                    naming_session.Game = "P2EP-PS1";
                    _ = Display_Names_Character_Select_Menu.Display_Names_Character_Select_Main(menuSession);
                    break;
                case "P2EP-PSP":
                    naming_session.Game = "P2EP-PSP";
                    _ = Display_Names_Character_Select_Menu.Display_Names_Character_Select_Main(menuSession);
                    break;
                case "return":
                    _ = Display_Names_Title_Select_Menu.Display_Names_Title_Select(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Display_Names_Title_Select_VC_P3_Main(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            var naming_session = Global.DisplayNameTempList.SingleOrDefault(x => x.User_ID == $"{menuSession.User.Id}");

            string selected = component.Data.Values.First();

            switch (selected)
            {
                case "P3F":
                    naming_session.Game = "P3F";
                    _ = Display_Names_Character_Select_Menu.Display_Names_Character_Select_Main(menuSession);
                    break;
                case "P3P":
                    naming_session.Game = "P3P";
                    _ = Display_Names_Character_Select_Menu.Display_Names_Character_Select_Main(menuSession);
                    break;
                case "return":
                    _ = Display_Names_Title_Select_Menu.Display_Names_Title_Select(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Display_Names_Title_Select_VC_P4_Main(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            var naming_session = Global.DisplayNameTempList.SingleOrDefault(x => x.User_ID == $"{menuSession.User.Id}");

            string selected = component.Data.Values.First();

            switch (selected)
            {
                case "P4-PS2":
                    naming_session.Game = "P4-PS2";
                    _ = Display_Names_Character_Select_Menu.Display_Names_Character_Select_Main(menuSession);
                    break;
                case "P4G":
                    naming_session.Game = "P4G";
                    _ = Display_Names_Character_Select_Menu.Display_Names_Character_Select_Main(menuSession);
                    break;
                case "return":
                    _ = Display_Names_Title_Select_Menu.Display_Names_Title_Select(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Display_Names_Title_Select_VC_P5_Main(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            var naming_session = Global.DisplayNameTempList.SingleOrDefault(x => x.User_ID == $"{menuSession.User.Id}");

            string selected = component.Data.Values.First();

            switch (selected)
            {
                case "P5-PS4":
                    naming_session.Game = "P5-PS4";
                    _ = Display_Names_Character_Select_Menu.Display_Names_Character_Select_Main(menuSession);
                    break;
                case "P5R":
                    naming_session.Game = "P5R";
                    _ = Display_Names_Character_Select_Menu.Display_Names_Character_Select_Main(menuSession);
                    break;
                case "return":
                    _ = Display_Names_Title_Select_Menu.Display_Names_Title_Select(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }
    }
}
