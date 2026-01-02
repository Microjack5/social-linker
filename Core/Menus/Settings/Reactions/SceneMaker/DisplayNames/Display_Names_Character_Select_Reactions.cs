using Discord.WebSocket;
using SocialLinker.Core.LocalStorageTables;
using SocialLinker.Core.Menus.Settings.Main.SceneMaker.DisplayNames;
using SocialLinker.Core.SceneMaker;
using System.Linq;
using System.Threading.Tasks;

namespace SocialLinker.Core.Menus.Settings.Reactions.SceneMaker.DisplayNames
{
    internal class Display_Names_Character_Select_Reactions
    {
        public static Task Nav_Display_Names_Character_Select_Main(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "display-names-character-select-modal-open":
                    _ = Display_Names_Character_Select_Menu.Display_Names_Character_Select_Modal(component);
                    break;

                case "return":
                    _ = Display_Names_Title_Select_Menu.Display_Names_Title_Select(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static async Task Nav_Display_Names_Character_Select_Modal(SocketModal modal, MenuIdStructure menuSession)
        {
            var account = menuSession.Account;

            var sprite_set_name = modal.Data.Components
            .FirstOrDefault(x => x.CustomId == "name")?.Value;

            await Nav_Display_Names_Character_Select_Main_Received(sprite_set_name, menuSession);
            return;
        }

        public static Task Nav_Display_Names_Character_Select_Error(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "return":
                    _ = Display_Names_Character_Select_Menu.Display_Names_Character_Select_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        // Methods that activate on the MessageReceived event.
        public static Task Nav_Display_Names_Character_Select_Main_Received(string sprite_set_name, MenuIdStructure menuSession)
        {
            var naming_session = Global.DisplayNameTempList.SingleOrDefault(x => x.User_ID == $"{menuSession.User.Id}");

            var account = menuSession.Account;
            string input_string = sprite_set_name;

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
                _ = Display_Names_Character_Select_Menu.Display_Names_Character_Select_Error(menuSession, input_string);
                return Task.CompletedTask;
            }

            naming_session.Sprite_Set = sprite_set_info;

            _ = Display_Names_Sprite_Select_Menu.Display_Names_Sprite_Select_Main(menuSession);
            return Task.CompletedTask;
        }
    }
}
