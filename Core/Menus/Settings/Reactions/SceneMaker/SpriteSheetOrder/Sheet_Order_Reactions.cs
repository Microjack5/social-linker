using Discord.WebSocket;
using SocialLinker.Core.CloudStorageTables;
using SocialLinker.Core.Menus.Settings.Main.SceneMaker;
using SocialLinker.Core.Menus.Settings.Main.SceneMaker.SpriteSheetOrder;
using SocialLinker.Core.Menus.Settings.Main.SceneMaker.TemplateLayout;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace SocialLinker.Core.Menus.Settings.Reactions.SceneMaker.SpriteSheetOrder
{
    class Sheet_Order_Reactions
    {
        public static Task Nav_Sheet_Order_Main(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            var account = menuSession.Account;

            switch (component.Data.CustomId)
            {
                case "return":
                    _ = SM_Settings_Menu.SM_Settings_Main(menuSession);
                    break;

                case "outfit":
                    account.Setting_Sheet_Order = "Order by Outfit";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Sheet_Order_Menu.Sheet_Order_Confirm(menuSession);
                    break;

                case "expression":
                    account.Setting_Sheet_Order = "Order by Expression";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Sheet_Order_Menu.Sheet_Order_Confirm(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Sheet_Order_Confirm(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "back-to-scene-maker-settings":
                    _ = SM_Settings_Menu.SM_Settings_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }
    }
}
