using System.Threading.Tasks;
using Discord.WebSocket;
using SocialLinker.Core.Menus.Help.Main;

namespace SocialLinker.Core.Menus.Help.Reactions
{
    class SM_Tutorial_Basics_Reactions
    {
        public static Task Nav_SM_Tutorial_Basics_Page_1(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "return":
                    _ = SM_Tutorial_Select_Menu.SM_Tutorial_Select_Main(menuSession);
                    break;

                case "next-page":
                    _ = SM_Tutorial_Basics_Menu.SM_Tutorial_Basics_Page_2(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_SM_Tutorial_Basics_Page_2(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "previous-page":
                    _ = SM_Tutorial_Basics_Menu.SM_Tutorial_Basics_Page_1(menuSession);
                    break;

                case "next-page":
                    _ = SM_Tutorial_Basics_Menu.SM_Tutorial_Basics_Page_3(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_SM_Tutorial_Basics_Page_3(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "previous-page":
                    _ = SM_Tutorial_Basics_Menu.SM_Tutorial_Basics_Page_2(menuSession);
                    break;

                case "next-page":
                    _ = SM_Tutorial_Basics_Menu.SM_Tutorial_Basics_Page_4(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_SM_Tutorial_Basics_Page_4(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "previous-page":
                    _ = SM_Tutorial_Basics_Menu.SM_Tutorial_Basics_Page_3(menuSession);
                    break;

                case "next-page":
                    _ = SM_Tutorial_Basics_Menu.SM_Tutorial_Basics_Page_5(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_SM_Tutorial_Basics_Page_5(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "previous-page":
                    _ = SM_Tutorial_Basics_Menu.SM_Tutorial_Basics_Page_4(menuSession);
                    break;

                case "back-to-tutorial-menu":
                    _ = SM_Tutorial_Select_Menu.SM_Tutorial_Select_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }
    }
}
