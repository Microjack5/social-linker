using System.Threading.Tasks;
using Discord;

namespace SocialLinker.Core.Menus.Help.Main
{
    class SM_Tutorial_Anime_Frames_Menu
    {
        public static async Task SM_Tutorial_Anime_Frames_Page_1(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Viewing Animation Frames",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            var footer = new EmbedFooterBuilder
            {
                Text = "Page 1 / 3"
            };

            embed.WithFooter(footer);

            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Profile_Help_Thumbnail(account));

            embed.WithDescription("" +
                "Some sprites come with animation frames you can use to make their expressions more dynamic!\n" +
                "\n" +
                $"Use **`maker_sheet`** and select the `sprite_number` option after choosing a character to view the animation frames for them.\n");

            embed.WithImageUrl("https://i.imgur.com/C80ESvK.png");

            menuSession.CurrentMenu = "SM_Tutorial_Anime_Frames_Page_1";

            var component = new ComponentBuilder()
                .WithButton("↩️ Return", customId: "return", ButtonStyle.Secondary)
                .WithButton("▶️ Next Page", customId: "next-page", ButtonStyle.Secondary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task SM_Tutorial_Anime_Frames_Page_2(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Using Animation Frames",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            var footer = new EmbedFooterBuilder
            {
                Text = "Page 2 / 3"
            };

            embed.WithFooter(footer);

            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Profile_Help_Thumbnail(account));

            embed.WithDescription("" +
                "To use these animation frames in a scene, use **`maker_create`** and specify the `eye_frame` and `mouth_frame` options.\n");

            embed.WithImageUrl("https://i.imgur.com/pmrYI4b.png");

            menuSession.CurrentMenu = "SM_Tutorial_Anime_Frames_Page_2";

            var component = new ComponentBuilder()
                .WithButton("◀️ Previous Page", customId: "previous-page", ButtonStyle.Secondary)
                .WithButton("▶️ Next Page", customId: "next-page", ButtonStyle.Secondary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task SM_Tutorial_Anime_Frames_Page_3(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Using Animation Frames",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            var footer = new EmbedFooterBuilder
            {
                Text = "Page 3 / 3"
            };

            embed.WithFooter(footer);

            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Profile_Help_Thumbnail(account));

            embed.WithDescription("" +
                "If the sprite only has eye animations, just specifying the base sprite and eye frame numbers will work as well. " +
                "You can also type `0` for either frame if the sprite doesn't have one to choose or you don't want to use one.");

            embed.WithImageUrl("https://i.imgur.com/HrLAzlx.png");

            menuSession.CurrentMenu = "SM_Tutorial_Anime_Frames_Page_3";

            var component = new ComponentBuilder()
                .WithButton("◀️ Previous Page", customId: "previous-page", ButtonStyle.Secondary)
                .WithButton("💠 Return to Tips & Tricks Menu", customId: "back-to-advanced-tutorials", ButtonStyle.Secondary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }
    }
}
