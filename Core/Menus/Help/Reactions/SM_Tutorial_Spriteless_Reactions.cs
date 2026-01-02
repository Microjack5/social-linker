using Discord.WebSocket;
using SocialLinker.Core.Menus.Help.Main;
using System.Linq;
using System.Threading.Tasks;

namespace SocialLinker.Core.Menus.Help.Reactions
{
    class SM_Tutorial_Spriteless_Reactions
    {
        public static Task Nav_SM_Tutorial_Spriteless_Main(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            string selected = component.Data.Values.First();

            switch (selected)
            {
                case "1":
                    _ = SM_Tutorial_Spriteless_Menu.SM_Tutorial_Spriteless_Chara_Page_1(menuSession);
                    break;
                case "2":
                    _ = SM_Tutorial_Spriteless_Menu.SM_Tutorial_Spriteless_System_Page_1(menuSession);
                    break;
                case "return":
                    _ = SM_Tutorial_Select_Menu.SM_Tutorial_Select_Advanced(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_SM_Tutorial_Spriteless_Chara_Page_1(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "return":
                    _ = SM_Tutorial_Spriteless_Menu.SM_Tutorial_Spriteless_Main(menuSession);
                    break;

                case "next-page":
                    _ = SM_Tutorial_Spriteless_Menu.SM_Tutorial_Spriteless_Chara_Page_2(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_SM_Tutorial_Spriteless_Chara_Page_2(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "previous-page":
                    _ = SM_Tutorial_Spriteless_Menu.SM_Tutorial_Spriteless_Chara_Page_1(menuSession);
                    break;

                case "next-tutorial":
                    _ = SM_Tutorial_Spriteless_Menu.SM_Tutorial_Spriteless_System_Page_1(menuSession);
                    break;

                case "back-to-spriteless-tutorials":
                    _ = SM_Tutorial_Spriteless_Menu.SM_Tutorial_Spriteless_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_SM_Tutorial_Spriteless_System_Page_1(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "return":
                    _ = SM_Tutorial_Spriteless_Menu.SM_Tutorial_Spriteless_Main(menuSession);
                    break;

                case "next-page":
                    _ = SM_Tutorial_Spriteless_Menu.SM_Tutorial_Spriteless_System_Page_2(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_SM_Tutorial_Spriteless_System_Page_2(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "previous-page":
                    _ = SM_Tutorial_Spriteless_Menu.SM_Tutorial_Spriteless_System_Page_1(menuSession);
                    break;

                case "back-to-spriteless-tutorials":
                    _ = SM_Tutorial_Spriteless_Menu.SM_Tutorial_Spriteless_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }
    }
}
