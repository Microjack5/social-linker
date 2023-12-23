using Discord.WebSocket;
using SocialLinker.Core.CloudStorageTables;
using SocialLinker.Core.LocalStorageTables;
using SocialLinker.Core.Menus.MakerMulti.Main;
using SocialLinker.Core.Menus.Settings.Main.SceneMaker.DisplayNames;
using SocialLinker.Core.SceneMaker;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace SocialLinker.Core.Menus.MakerMulti.Reactions
{
    class MakerMulti_Char_Select_1_Reactions
    {
        public static Task Nav_MakerMulti_Character_Select_1_Main(SocketReaction reaction, MenuIdStructure menuSession)
        {
            if (reaction.Emote.Name == "↩️")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = MakerMulti_Layout_Select_Menu.MakerMulti_Layout_Select_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_MakerMulti_Character_Select_1_Invalid_Character(SocketReaction reaction, MenuIdStructure menuSession)
        {
            if (reaction.Emote.Name == "↩️")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = MakerMulti_Char_Select_1_Menu.MakerMulti_Character_Select_1_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_MakerMulti_Character_Select_1_Error_Sprite(SocketReaction reaction, MenuIdStructure menuSession)
        {
            if (reaction.Emote.Name == "↩️")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = MakerMulti_Char_Select_1_Menu.MakerMulti_Character_Select_1_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        // Methods that activate on the MessageReceived event.
        public static Task Nav_MakerMulti_Character_Select_1_Main_Received(SocketMessage message, MenuIdStructure menuSession)
        {
            var multimaker_session = Global.MultiMaker_Session_List.SingleOrDefault(x => x.User.Id == menuSession.User.Id);

            var account = UserInfoClasses.GetAccount(message.Author);
            string input_string = message.Content;
            input_string = Global.RemoveBotMention(input_string).Trim();

            MakerCommandData maker_command = new MakerCommandData()
            {
                Character_Data_1 = new MakerCharacterData()
                {
                    Sprite_Set_Version = multimaker_session.MakerCommand.Template,
                    Character_Keyword = input_string
                }
            };

            OfficialSetData sprite_set_info = OfficialSetMethods.GetSpriteSetInfo(account, maker_command);

            if (sprite_set_info == null)
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = MakerMulti_Char_Select_1_Menu.MakerMulti_Character_Select_1_Invalid_Character(menuSession.User, menuSession.MenuMessage, input_string);
                return Task.CompletedTask;
            }

            multimaker_session.MakerCommand.Character_Data_1.Set_Data = sprite_set_info;

            // Stop the timeout timer associated with the menu.
            menuSession.MenuTimer.Stop();

            // Go to a new menu.
            _ = MakerMulti_Sprite_Select_1_Menu.MakerMulti_Sprite_Select_1_Main(menuSession.User, menuSession.MenuMessage);
            return Task.CompletedTask;
        }
    }
}
