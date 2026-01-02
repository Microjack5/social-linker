using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using SocialLinker.Core.LocalStorageTables;

namespace SocialLinker.Core.Menus.Settings.Main.SceneMaker.DisplayNames
{
    internal class Display_Names_Sprite_Select_Menu
    {
        public static async Task Display_Names_Sprite_Select_Main(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var new_name_data = Global.DisplayNameTempList.SingleOrDefault(x => x.User_ID == $"{menuSession.User.Id}");
            OfficialSetData set_data = new_name_data.Sprite_Set;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = $"Range Selection - {set_data.Name}",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            embed.WithColor(EmbedSettings.Get_Game_Color(new_name_data.Game, account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo(new_name_data.Game));

            embed.WithDescription("" +
                $"Select [Enter Sprite Range] and specify the sprite numbers of {set_data.Name}'s sprite set you'd like to change the display names for.\n" +
                $"\n" +
                $"You can type in individual numbers separated by a comma or space, select a range by placing a hyphen between two numbers, or react with 🔄 to select the entire set.\n");

            menuSession.CurrentMenu = "Display_Names_Sprite_Select_Main";

            var component = new ComponentBuilder()
                .WithButton("Enter Sprite Range", customId: "display-names-sprite-select-modal-open", ButtonStyle.Primary)
                .WithButton("🔄 Entire Set", customId: "entire-set", ButtonStyle.Secondary)
                .WithButton("↩️ Return", customId: "return", ButtonStyle.Secondary);

            await Utility.CleanMessage(menuSession, embed, component, true);
            Utility.NewTimer(menuSession);
        }

        public static async Task Display_Names_Sprite_Select_Modal(SocketMessageComponent component)
        {
            if (component.Data.CustomId == "display-names-sprite-select-modal-open")
            {
                try
                {
                    var modal = new ModalBuilder()
                    .WithTitle("Range Entry")
                    .WithCustomId("display-names-sprite-select-modal-submit")
                    .AddTextInput("Sprite Numbers", "range");

                    await component.RespondWithModalAsync(modal.Build());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public static async Task Display_Names_Sprite_Select_Error_1(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var new_name_data = Global.DisplayNameTempList.SingleOrDefault(x => x.User_ID == $"{menuSession.User.Id}");

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = $"Unexpected Input",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Profile_Config_Thumbnail(account));

            OfficialSetData set_data = new_name_data.Sprite_Set;

            embed.WithDescription("" +
                "An unexpected character was found while reading your input. Press ↩️ to try again.\n");

            menuSession.CurrentMenu = "Display_Names_Sprite_Select_Error_1";

            var component = new ComponentBuilder()
                .WithButton("↩️ Retry", customId: "return", ButtonStyle.Secondary);

            await Utility.CleanMessage(menuSession, embed, component, true);
            Utility.NewTimer(menuSession);
        }

        public static async Task Display_Names_Sprite_Select_Error_2(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var new_name_data = Global.DisplayNameTempList.SingleOrDefault(x => x.User_ID == $"{menuSession.User.Id}");

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = $"Invalid Range",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Profile_Config_Thumbnail(account));

            embed.WithDescription("" +
                "Invalid range. Press ↩️ to try again.\n");

            menuSession.CurrentMenu = "Display_Names_Sprite_Select_Error_2";

            var component = new ComponentBuilder()
                .WithButton("↩️ Retry", customId: "return", ButtonStyle.Secondary);

            await Utility.CleanMessage(menuSession, embed, component, true);
            Utility.NewTimer(menuSession);
        }

        public static async Task Display_Names_Sprite_Select_Error_3(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var new_name_data = Global.DisplayNameTempList.SingleOrDefault(x => x.User_ID == $"{menuSession.User.Id}");

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = $"Overlapping Sprites",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Profile_Config_Thumbnail(account));

            embed.WithDescription("" +
                "A custom display name that overlaps with the sprite range you've chosen already exists. Press ↩️ to select a different range.\n");

            menuSession.CurrentMenu = "Display_Names_Sprite_Select_Error_3";

            var component = new ComponentBuilder()
                .WithButton("↩️ Retry", customId: "return", ButtonStyle.Secondary);

            await Utility.CleanMessage(menuSession, embed, component, true);
            Utility.NewTimer(menuSession);
        }
    }
}
