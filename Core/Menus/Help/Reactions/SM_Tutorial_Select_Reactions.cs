using Discord.WebSocket;
using SocialLinker.Core.Menus.Help.Main;
using System.Linq;
using System.Threading.Tasks;

namespace SocialLinker.Core.Menus.Help.Reactions
{
    class SM_Tutorial_Select_Reactions
    {
        public static Task Nav_SM_Tutorial_Select_Main(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            string selected = component.Data.Values.First();

            switch (selected)
            {
                case "1":
                    _ = SM_Tutorial_Basics_Menu.SM_Tutorial_Basics_Page_1(menuSession);
                    break;
                case "2":
                    _ = SM_Tutorial_Select_Menu.SM_Tutorial_Select_Advanced(menuSession);
                    break;
                case "return":
                    _ = Help_Menu.Help_Main_Menu(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_SM_Tutorial_Select_Advanced(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            string selected = component.Data.Values.First();

            switch (selected)
            {
                case "1":
                    _ = SM_Tutorial_VC_Menu.SM_Tutorial_VC_Main(menuSession);
                    break;
                case "2":
                    _ = SM_Tutorial_Spriteless_Menu.SM_Tutorial_Spriteless_Main(menuSession);
                    break;
                case "3":
                    _ = SM_Tutorial_Multi_Menu.SM_Tutorial_Multi_Chara_Page_1(menuSession);
                    break;
                case "4":
                    _ = SM_Tutorial_Anime_Frames_Menu.SM_Tutorial_Anime_Frames_Page_1(menuSession);
                    break;
                case "5":
                    _ = SM_Tutorial_Line_Breaks_Menu.SM_Tutorial_Line_Breaks_Page_1(menuSession);
                    break;
                case "return":
                    _ = Help_Menu.Help_Main_Menu(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }
    }
}
