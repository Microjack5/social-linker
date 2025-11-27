using System.Threading.Tasks;
using Discord;

namespace SocialLinker.Core.Menus.Help.Main
{
    class SM_Tutorial_Spriteless_Menu
    {
        public static async Task SM_Tutorial_Spriteless_Main(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Spriteless Scenes",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);
            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Profile_Help_Thumbnail(account));

            embed.AddField(":one: Characters", "" +
                "Learn how to create spriteless scenes with characters.");
            embed.AddField(":two: System Messages", "" +
                "Learn how to create spriteless scenes with system messages.");

            menuSession.CurrentMenu = "SM_Tutorial_Spriteless_Main";

            var selectMenu = new SelectMenuBuilder()
                    .WithPlaceholder("Select an option")
                    .WithCustomId(menuSession.CurrentMenu)
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .AddOption("Characters", "1", null, new Emoji("1️⃣"))
                    .AddOption("System Messages", "2", null, new Emoji("2️⃣"))
                    .AddOption("Return to Tips & Tricks Menu", "return", null, new Emoji("↩️"));

            var component = new ComponentBuilder()
                .WithSelectMenu(selectMenu);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task SM_Tutorial_Spriteless_Chara_Page_1(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Creating Spriteless Scenes",
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
                "When creating a scene, type `0` as the sprite number to not have a character sprite appear. This will only leave the character’s name displayed.");

            embed.WithImageUrl("https://i.imgur.com/6NoOby5.png");

            menuSession.CurrentMenu = "SM_Tutorial_Spriteless_Chara_Page_1";

            var component = new ComponentBuilder()
                .WithButton("↩️ Return", customId: "return", ButtonStyle.Secondary)
                .WithButton("▶️ Next Page", customId: "next-page", ButtonStyle.Secondary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task SM_Tutorial_Spriteless_Chara_Page_2(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Spriteless NPCs",
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
                $"Combined with the Display Names menu in **`settings`** > [Scene Maker Settings], you could even act out spriteless NPCs!");

            embed.WithImageUrl("https://i.imgur.com/gQuvGNE.png");

            menuSession.CurrentMenu = "SM_Tutorial_Spriteless_Chara_Page_2";

            var component = new ComponentBuilder()
                .WithButton("◀️ Previous Page", customId: "previous-page", ButtonStyle.Secondary)
                .WithButton("⏭️ Next Tutorial", customId: "next-tutorial", ButtonStyle.Secondary)
                .WithButton("💠 Return to Spriteless Scenes Menu", customId: "back-to-spriteless-tutorials", ButtonStyle.Secondary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task SM_Tutorial_Spriteless_System_Page_1(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "System Messages",
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
                "Use the character keyword `System` with a game style keyword to create system messages in that template.\n" +
                "\n" +
                "Like when creating scenes with spriteless characters, the sprite number will be `0`.");

            embed.WithImageUrl("https://i.imgur.com/usrLoth.png");

            menuSession.CurrentMenu = "SM_Tutorial_Spriteless_System_Page_1";

            var component = new ComponentBuilder()
                .WithButton("↩️ Return", customId: "return", ButtonStyle.Secondary)
                .WithButton("▶️ Next Page", customId: "next-page", ButtonStyle.Secondary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task SM_Tutorial_Spriteless_System_Page_2(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Cross-Dimensional Observation System No. XX",
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
                "BBTAG’s System acts as a unique character with multiple textboxes to choose from.\n" +
                "\n" +
                "The default form can still be selected by setting the sprite number to `0`, or you can set the sprite number to `1` for her sentient form.");

            embed.WithImageUrl("https://i.imgur.com/gZ7elqR.gif");

            menuSession.CurrentMenu = "SM_Tutorial_Spriteless_System_Page_2";

            var component = new ComponentBuilder()
                .WithButton("◀️ Previous Page", customId: "previous-page", ButtonStyle.Secondary)
                .WithButton("💠 Return to Spriteless Scenes Menu", customId: "back-to-spriteless-tutorials", ButtonStyle.Secondary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }
    }
}
