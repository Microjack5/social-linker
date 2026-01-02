using System.Threading.Tasks;
using Discord.WebSocket;
using SocialLinker.Core.Menus.Help.Main;

namespace SocialLinker.Core.Menus.Help.Reactions
{
    class Credits_Reactions
    {
        public static Task Nav_Credits_Page_1(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "return":
                    _ = Help_Menu.Help_Main_Menu(menuSession);
                    break;

                case "next-page":
                    _ = Credits_Menu.Credits_Page_2(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Credits_Page_2(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "previous-page":
                    _ = Credits_Menu.Credits_Page_1(menuSession);
                    break;

                case "back-to-help-menu":
                    _ = Help_Menu.Help_Main_Menu(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }
    }
}
