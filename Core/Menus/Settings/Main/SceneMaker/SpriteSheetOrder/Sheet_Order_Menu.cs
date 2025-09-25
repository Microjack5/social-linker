using System.Threading.Tasks;
using Discord;

namespace SocialLinker.Core.Menus.Settings.Main.SceneMaker.SpriteSheetOrder
{
    class Sheet_Order_Menu
    {
        public static async Task Sheet_Order_Main(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Sprite Sheet Order",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            var footer = new EmbedFooterBuilder
            {
                Text = "↩️ Return to Scene Maker Settings"
            };

            embed.WithFooter(footer);

            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Profile_Config_Thumbnail(account));

            embed.WithDescription("" +
                "Sprite sheets for some titles can be organized by grouping similar costumes or expressions together.\n" +
                "\n" +
                $"⚙️ **Current setting:** **`{account.Setting_Sheet_Order}`**\n");

            embed.WithImageUrl("https://i.imgur.com/ZiCkdCc.png");

            menuSession.CurrentMenu = "Sheet_Order_Main";

            var component = new ComponentBuilder()
                .WithButton("↩️", customId: "return", ButtonStyle.Secondary)
                .WithButton("Order by Outfit", customId: "outfit", ButtonStyle.Secondary)
                .WithButton("Order by Expression", customId: "expression", ButtonStyle.Secondary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Sheet_Order_Confirm(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Settings Saved",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Profile_Config_Thumbnail(account));

            embed.WithDescription("" +
                $"Your sprite sheet order has changed to **`{account.Setting_Sheet_Order}`**.");

            menuSession.CurrentMenu = "Sheet_Order_Confirm";

            var component = new ComponentBuilder()
                .WithButton("💠 Scene Maker Settings", customId: "back-to-scene-maker-settings", ButtonStyle.Primary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }
    }
}
