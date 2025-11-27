using Discord.WebSocket;
using SocialLinker.Core.Menus.Help.Main;
using System.Threading.Tasks;

namespace SocialLinker.Core.Menus.Help.Reactions
{
    class Help_Reactions
    {
        public static Task Nav_Help_Main_Menu(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "status-screen-tutorial":
                    _ = Status_Tutorial_Menu.Status_Tutorial_Page_1(menuSession);
                    break;

                case "scene-maker-tutorial":
                    _ = SM_Tutorial_Select_Menu.SM_Tutorial_Select_Main(menuSession);
                    break;

                case "legal-notices":
                    _ = Legal_Notices_Menu.Legal_Notices_Main(menuSession);
                    break;

                case "credits":
                    _ = Credits_Menu.Credits_Page_1(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }
    }
}
