using Discord.WebSocket;
using SocialLinker.Core.Menus.Settings.Main.SceneMaker.DisplayNames;
using System.Threading.Tasks;

namespace SocialLinker.Core.Menus.Settings.Reactions.SceneMaker.DisplayNames
{
    internal class Display_Name_Confirm_Reactions
    {
        public static Task Nav_Display_Names_Confirm_Main(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "display-names":
                    _ = Display_Names_Menu.Display_Names_Start(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }
    }
}
