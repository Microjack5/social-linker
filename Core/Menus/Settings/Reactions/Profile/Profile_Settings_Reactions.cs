using Discord.WebSocket;
using SocialLinker.Core.Menus.Settings.Main;
using SocialLinker.Core.Menus.Settings.Main.Profile;
using System.Linq;
using System.Threading.Tasks;

namespace SocialLinker.Core.Menus.Settings.Reactions.Profile
{
    class Profile_Settings_Reactions
    {
        public static Task Nav_Profile_Settings_Main(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            string selected = component.Data.Values.First();

            switch (selected)
            {
                case "1":
                    _ = Profile_Theme_Menu.Profile_Theme_Main(menuSession);
                    break;
                case "2":
                    _ = Status_Decor_Menu.Status_Decor_Start(menuSession);
                    break;
                case "3":
                    _ = Time_Weather_Menu.Time_Weather_Main(menuSession.User, menuSession.MenuMessage);
                    break;
                case "4":
                    _ = Level_Up_Notifications_Menu.Level_Up_Notifications_Main(menuSession);
                    break;
                case "5":
                    _ = Rank_Up_Notifications_Menu.Rank_Up_Notifications_Main(menuSession);
                    break;
                case "6":
                    _ = Content_Filter_Menu.Content_Filter_Main(menuSession);
                    break;
                case "star1":
                case "star2":
                case "star3":
                    _ = Star_Level_Menu.Star_Level_Main(menuSession.User, menuSession.MenuMessage);
                    break;
                case "return":
                    _ = Settings_Menu.Settings_Main_Menu(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }
    }
}
