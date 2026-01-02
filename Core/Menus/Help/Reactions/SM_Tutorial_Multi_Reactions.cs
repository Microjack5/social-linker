using Discord.WebSocket;
using SocialLinker.Core.Menus.Help.Main;
using System.Threading.Tasks;

namespace SocialLinker.Core.Menus.Help.Reactions
{
    class SM_Tutorial_Multi_Reactions
    {
        public static Task Nav_SM_Tutorial_Multi_Chara_Page_1(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "return":
                    _ = SM_Tutorial_Select_Menu.SM_Tutorial_Select_Advanced(menuSession);
                    break;

                case "next-page":
                    _ = SM_Tutorial_Multi_Menu.SM_Tutorial_Multi_Chara_Page_2(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_SM_Tutorial_Multi_Chara_Page_2(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "previous-page":
                    _ = SM_Tutorial_Multi_Menu.SM_Tutorial_Multi_Chara_Page_1(menuSession);
                    break;

                case "back-to-tips-menu":
                    _ = SM_Tutorial_Select_Menu.SM_Tutorial_Select_Advanced(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }
    }
}
