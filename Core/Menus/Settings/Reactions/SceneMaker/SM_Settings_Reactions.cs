using Discord.WebSocket;
using SocialLinker.Core.Menus.Settings.Main;
using SocialLinker.Core.Menus.Settings.Main.SceneMaker;
using SocialLinker.Core.Menus.Settings.Main.SceneMaker.Backgrounds;
using SocialLinker.Core.Menus.Settings.Main.SceneMaker.DisplayNames;
using SocialLinker.Core.Menus.Settings.Main.SceneMaker.ResolutionScaling;
using SocialLinker.Core.Menus.Settings.Main.SceneMaker.SpriteSheetOrder;
using System.Linq;
using System.Threading.Tasks;

namespace SocialLinker.Core.Menus.Settings.Reactions.SceneMaker
{
    class SM_Settings_Reactions
    {
        public static Task Nav_SM_Settings_Main(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            string selected = component.Data.Values.First();

            switch (selected)
            {
                case "1":
                    _ = Version_Control_Menu.Version_Control_Main(menuSession);
                    break;
                case "2":
                    _ = Template_Layout_Menu.Template_Layout_Main(menuSession);
                    break;
                case "3":
                    _ = Display_Names_Menu.Display_Names_Start(menuSession.User, menuSession.MenuMessage);
                    break;
                case "4":
                    _ = Display_Names_Menu.Display_Names_Start(menuSession.User, menuSession.MenuMessage);
                    break;
                case "5":
                    _ = Sheet_Order_Menu.Sheet_Order_Main(menuSession.User, menuSession.MenuMessage);
                    break;
                case "6":
                    _ = Backgrounds_Menu.Backgrounds_Main(menuSession.User, menuSession.MenuMessage);
                    break;
                case "7":
                    _ = Resolution_Scaling_Menu.Resolution_Scaling_Main(menuSession.User, menuSession.MenuMessage);
                    break;
                case "return":
                    _ = Settings_Menu.Settings_Main_Menu(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }
    }
}
