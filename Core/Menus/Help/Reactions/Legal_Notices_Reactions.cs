using Discord.WebSocket;
using SocialLinker.Core.Menus.Help.Main;
using System.Threading.Tasks;

namespace SocialLinker.Core.Menus.Help.Reactions
{
    class Legal_Notices_Reactions
    {
        public static Task Nav_Legal_Notices_Main(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "return":
                    _ = Help_Menu.Help_Main_Menu(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }
    }
}
