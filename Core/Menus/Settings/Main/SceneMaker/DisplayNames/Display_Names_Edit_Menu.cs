using System.Linq;
using System.Threading.Tasks;
using Discord;
using SocialLinker.Core.CloudStorageTables;
using SocialLinker.Core.LocalStorageTables;

namespace SocialLinker.Core.Menus.Settings.Main.SceneMaker.DisplayNames
{
    internal class Display_Names_Edit_Menu
    {
        public static async Task Display_Names_Edit_Main(MenuIdStructure menuSession, int item_menu_index)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var itemSession = Global.ItemIdList.SingleOrDefault(x => x.User.Id == user.Id);

            var name_info = GetNameInfo(itemSession, item_menu_index);
            itemSession.SelectedDisplayName = name_info;
            itemSession.CurrentMenuItem = item_menu_index;
            OfficialSetData current_set_data = OfficialSetMethods.Search_By_Title_And_ID(name_info.Game, name_info.Character_ID);

            // Create a new embed that will be displayed in the message.
            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Edit Display Names",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            embed.WithDescription("What would you like to do?\n" +
                $"\n" +
                $"**Display Name:** `{name_info.Display_Name}`\n" +
                $"**Character:** `{current_set_data.Name}`\n" +
                $"**Game:** `{name_info.Game}`\n" +
                $"**Sprite Numbers Affected:** `{DisplayNameLogging.String_Range_To_Int_Range(account, current_set_data, DisplayNameLogging.String_To_String_List(name_info.Sprites_Affected), name_info)}`\n" +
                $"**Spriteless Affected:** `{name_info.Spriteless_Included}`\n");

            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));

            menuSession.CurrentMenu = "Display_Names_Edit_Main";

            var component = new ComponentBuilder()
                .WithButton("↩️ Return", customId: "return", ButtonStyle.Secondary)
                .WithButton("🗑️ Delete Display Name", customId: "delete", ButtonStyle.Secondary);

            await Utility.CleanMessage(menuSession, embed, component, true);
            Utility.NewTimer(menuSession);
        }

        public static async Task Display_Names_Delete_Confirmation(MenuIdStructure menuSession, int item_menu_index)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var itemSession = Global.ItemIdList.SingleOrDefault(x => x.User.Id == user.Id);

            var name_info = itemSession.SelectedDisplayName;
            OfficialSetData current_set_data = OfficialSetMethods.Search_By_Title_And_ID(name_info.Game, name_info.Character_ID);

            // Create a new embed that will be displayed in the message.
            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Confirm Deletion",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            embed.WithDescription("**Are you sure you want to delete this custom display name?**\n" +
                $"\n" +
                $"**Display Name:** `{name_info.Display_Name}`\n" +
                $"**Character:** `{current_set_data.Name}`\n" +
                $"**Game:** `{name_info.Game}`\n" +
                $"**Sprite Numbers Affected:** `{DisplayNameLogging.String_Range_To_Int_Range(account, current_set_data, DisplayNameLogging.String_To_String_List(name_info.Sprites_Affected), name_info)}`\n" +
                $"**Spriteless Affected:** `{name_info.Spriteless_Included}`\n");

            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));

            menuSession.CurrentMenu = "Display_Names_Delete_Confirmation";

            var component = new ComponentBuilder()
                .WithButton("↩️ Return", customId: "return", ButtonStyle.Secondary)
                .WithButton("✅ Confirm", customId: "confirm", ButtonStyle.Secondary);

            await Utility.CleanMessage(menuSession, embed, component, true);
            Utility.NewTimer(menuSession);
        }

        // Utility
        public static DisplayNameTableData GetNameInfo(ItemListIterator itemSession, int item_menu_index)
        {
            var result = itemSession.DisplayNameItemList[item_menu_index];

            return result;
        }
    }
}
