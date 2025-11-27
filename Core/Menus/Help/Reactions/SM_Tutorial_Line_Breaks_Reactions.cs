using Discord.WebSocket;
using SocialLinker.Core.Menus.Help.Main;
using System.Threading.Tasks;

namespace SocialLinker.Core.Menus.Help.Reactions
{
    class SM_Tutorial_Line_Breaks_Reactions
    {
        public static Task Nav_SM_Tutorial_Line_Breaks_Page_1(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "return":
                    _ = SM_Tutorial_Select_Menu.SM_Tutorial_Select_Advanced(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }
    }
}
