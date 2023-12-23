using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.WebSocket;
using SocialLinker.Core.CloudStorageTables;
using SocialLinker.Core.LocalStorageTables;
using SocialLinker.Core.Menus.Settings.Main.SceneMaker.DisplayNames;
using SocialLinker.Core.SceneMaker;

namespace SocialLinker.Core.Menus.Settings.Reactions.SceneMaker.DisplayNames
{
    internal class Display_Names_Character_Select_Reactions
    {
        public static Task Nav_Display_Names_Character_Select_Main(SocketReaction reaction, MenuIdStructure menuSession)
        {
            if (reaction.Emote.Name == "↩️")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Display_Names_Title_Select_Menu.Display_Names_Title_Select(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Display_Names_Character_Select_Error(SocketReaction reaction, MenuIdStructure menuSession)
        {
            if (reaction.Emote.Name == "↩️")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Display_Names_Character_Select_Menu.Display_Names_Character_Select_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        // Methods that activate on the MessageReceived event.
        public static Task Nav_Display_Names_Character_Select_Main_Received(SocketMessage message, MenuIdStructure menuSession)
        {
            var naming_session = Global.DisplayNameTempList.SingleOrDefault(x => x.User_ID == $"{menuSession.User.Id}");

            var account = UserInfoClasses.GetAccount(message.Author);
            string input_string = message.Content;
            input_string = Global.RemoveBotMention(input_string).Trim();

            MakerCommandData maker_command = new MakerCommandData()
            {
                Character_Data_1 = new MakerCharacterData()
                {
                    Sprite_Set_Version = naming_session.Game,
                    Character_Keyword = input_string
                }
            };

            OfficialSetData sprite_set_info = OfficialSetMethods.GetSpriteSetInfo(account, maker_command);

            if (sprite_set_info == null)
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Display_Names_Character_Select_Menu.Display_Names_Character_Select_Error(menuSession.User, menuSession.MenuMessage, input_string);
                return Task.CompletedTask;
            }

            naming_session.Sprite_Set = sprite_set_info;

            // Stop the timeout timer associated with the menu.
            menuSession.MenuTimer.Stop();

            // Go to a new menu.
            _ = Display_Names_Sprite_Select_Menu.Display_Names_Sprite_Select_Main(menuSession.User, menuSession.MenuMessage);
            return Task.CompletedTask;
        }
    }
}
