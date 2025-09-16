using Discord.WebSocket;
using SocialLinker.Core.Menus.MakerMulti.Main;
using SocialLinker.Core.Menus.Settings.Main.Profile;
using SocialLinker.Core.Menus.Settings.Main.SceneMaker;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace SocialLinker.Core.Menus.Settings.Reactions
{
    class Settings_Reactions
    {
        public static Task Nav_Settings_Main_Menu(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "profile-settings":
                    _ = Profile_Settings_Menu.Profile_Settings_Main(menuSession.User, menuSession.MenuMessage);
                    break;

                case "scene-maker-settings":
                    component.DeferAsync(ephemeral: true);
                    _ = SM_Settings_Menu.SM_Settings_Main(menuSession.User, menuSession.MenuMessage);
                    break;
            }

            return Task.CompletedTask;
        }
    }
}
