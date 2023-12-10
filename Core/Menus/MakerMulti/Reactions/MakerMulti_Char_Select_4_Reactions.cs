using Discord.WebSocket;
using SocialLinker.Core.CloudStorageTables;
using System.Linq;
using System.Threading.Tasks;
using SocialLinker.Core.LocalStorageTables;
using SocialLinker.Core.Menus.MakerMulti.Main;
using SocialLinker.Core.SceneMaker;

namespace SocialLinker.Core.Menus.MakerMulti.Reactions
{
    class MakerMulti_Char_Select_4_Reactions
    {
        public static Task Nav_MakerMulti_Character_Select_4_Main(SocketReaction reaction, MenuIdStructure menuSession)
        {
            if (reaction.Emote.Name == "↩️")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = MakerMulti_Char_Select_3_Menu.MakerMulti_Character_Select_3_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_MakerMulti_Character_Select_4_Invalid_Character(SocketReaction reaction, MenuIdStructure menuSession)
        {
            if (reaction.Emote.Name == "↩️")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = MakerMulti_Char_Select_4_Menu.MakerMulti_Character_Select_4_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_MakerMulti_Character_Select_4_Error_Sprite(SocketReaction reaction, MenuIdStructure menuSession)
        {
            if (reaction.Emote.Name == "↩️")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = MakerMulti_Char_Select_4_Menu.MakerMulti_Character_Select_4_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        // Methods that activate on the MessageReceived event.
        public static Task Nav_MakerMulti_Character_Select_4_Main_Received(SocketMessage message, MenuIdStructure menuSession)
        {
            var multimaker_session = Global.MultiMaker_Session_List.SingleOrDefault(x => x.User.Id == menuSession.User.Id);

            var account = UserInfoClasses.GetAccount(message.Author);
            string input_string = message.Content;
            input_string = Global.RemoveBotMention(input_string).Trim();

            MakerCommandData maker_command = new MakerCommandData()
            {
                Character_Data = new MakerCharacterData()
                {
                    Sprite_Set_Version = multimaker_session.MakerMultiCommand.Template,
                    Character_Keyword = input_string
                }
            };

            OfficialSetData sprite_set_info = OfficialSetMethods.GetSpriteSetInfo(account, maker_command);

            if (sprite_set_info == null)
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = MakerMulti_Char_Select_4_Menu.MakerMulti_Character_Select_4_Invalid_Character(menuSession.User, menuSession.MenuMessage, input_string);
                return Task.CompletedTask;
            }

            multimaker_session.MakerMultiCommand.Character_Data_4.Set_Data = sprite_set_info;

            // Stop the timeout timer associated with the menu.
            menuSession.MenuTimer.Stop();

            // Go to a new menu.
            _ = MakerMulti_Sprite_Select_4_Menu.MakerMulti_Sprite_Select_4_Main(menuSession.User, menuSession.MenuMessage);
            return Task.CompletedTask;
        }
    }
}
