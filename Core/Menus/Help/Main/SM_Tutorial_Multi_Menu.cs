using Discord;
using System.Threading.Tasks;

namespace SocialLinker.Core.Menus.Help.Main
{
    class SM_Tutorial_Multi_Menu
    {
        public static async Task SM_Tutorial_Multi_Chara_Page_1(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Multi-Character Scenes",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            var footer = new EmbedFooterBuilder
            {
                Text = "Page 1 / 2"
            };

            embed.WithFooter(footer);

            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Profile_Help_Thumbnail(account));

            embed.WithDescription("" +
                $"Use **`maker_multi`** to start creating a scene with multiple characters. This is only possible with a select few game styles, so choose the one you think fits best and follow the instructions provided.");

            menuSession.CurrentMenu = "SM_Tutorial_Multi_Chara_Page_1";

            var component = new ComponentBuilder()
                .WithButton("↩️ Return", customId: "return", ButtonStyle.Secondary)
                .WithButton("▶️ Next Page", customId: "next-page", ButtonStyle.Secondary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task SM_Tutorial_Multi_Chara_Page_2(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Forcing Character Positions",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            var footer = new EmbedFooterBuilder
            {
                Text = "Page 2 / 2"
            };

            embed.WithFooter(footer);

            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Profile_Help_Thumbnail(account));

            embed.WithDescription("" +
                "By setting one of the character’s sprite numbers to `0`, you can force the other character to appear on a side they normally wouldn’t be on alone.");

            menuSession.CurrentMenu = "SM_Tutorial_Multi_Chara_Page_2";

            var component = new ComponentBuilder()
                .WithButton("◀️ Previous Page", customId: "previous-page", ButtonStyle.Secondary)
                .WithButton("💠 Return to Tips & Tricks Menu", customId: "back-to-tips-menu", ButtonStyle.Secondary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }
    }
}
