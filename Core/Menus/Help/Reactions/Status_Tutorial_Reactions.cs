using Discord.WebSocket;
using SocialLinker.Core.Menus.Help.Main;
using System.Threading.Tasks;

namespace SocialLinker.Core.Menus.Help.Reactions
{
    class Status_Tutorial_Reactions
    {
        public static Task Nav_Status_Tutorial_Page_1(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "return":
                    _ = Help_Menu.Help_Main_Menu(menuSession);
                    break;

                case "next-page":
                    _ = Status_Tutorial_Menu.Status_Tutorial_Page_2(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Status_Tutorial_Page_2(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "previous-page":
                    _ = Status_Tutorial_Menu.Status_Tutorial_Page_1(menuSession);
                    break;

                case "next-page":
                    _ = Status_Tutorial_Menu.Status_Tutorial_Page_3(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Status_Tutorial_Page_3(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "previous-page":
                    _ = Status_Tutorial_Menu.Status_Tutorial_Page_2(menuSession);
                    break;

                case "next-page":
                    _ = Status_Tutorial_Menu.Status_Tutorial_Page_4(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Status_Tutorial_Page_4(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "previous-page":
                    _ = Status_Tutorial_Menu.Status_Tutorial_Page_3(menuSession);
                    break;

                case "next-page":
                    _ = Status_Tutorial_Menu.Status_Tutorial_Page_5(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Status_Tutorial_Page_5(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "previous-page":
                    _ = Status_Tutorial_Menu.Status_Tutorial_Page_4(menuSession);
                    break;

                case "back-to-help-menu":
                    _ = Help_Menu.Help_Main_Menu(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }
    }
}
