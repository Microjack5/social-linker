using System.Threading.Tasks;
using Discord;

namespace SocialLinker.Core.Menus.Help.Main
{
    class SM_Tutorial_Basics_Menu
    {
        public static async Task SM_Tutorial_Basics_Page_1(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Persona Scene Maker",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            var footer = new EmbedFooterBuilder
            {
                Text = "Page 1 / 5"
            };

            embed.WithFooter(footer);

            // Determine the color and thumbnail for the embeded message
            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Profile_Help_Thumbnail(account));

            embed.WithDescription("" +
                "The scene maker lets you create your own realistic screenshots from all across the Persona series!\n" +
                "\n" +
                $"You can make your own \"scenes\" a number of ways using multiple commands. Let's learn a few!\n");

            embed.WithImageUrl("https://i.imgur.com/03maXp5.png");

            menuSession.CurrentMenu = "SM_Tutorial_Basics_Page_1";

            var component = new ComponentBuilder()
                .WithButton("↩️ Return", customId: "return", ButtonStyle.Secondary)
                .WithButton("▶️ Next Page", customId: "next-page", ButtonStyle.Secondary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task SM_Tutorial_Basics_Page_2(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Character Lists and Game Styles",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            var footer = new EmbedFooterBuilder
            {
                Text = "Page 2 / 5"
            };

            embed.WithFooter(footer);

            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Profile_Help_Thumbnail(account));

            embed.WithDescription("" +
                "There are a few game styles to choose from based on multiple titles:\n" +
                "\n" +
                "<:P1:751133115531133112> `P1` - **Persona**\n" +
                "<:P2IS:788950080396328990> `P2IS` - **Persona 2: Innocent Sin**\n" +
                "<:P2EP:788950163363463172> `P2EP` - **Persona 2: Eternal Punishment**\n" +
                "<:P3:751133114918633483> `P3` - **Persona 3**\n" +
                "<:P4:751133120530612274> `P4` - **Persona 4**\n" +
                "<:P4AU:751133122342420572> `P4AU` - **Persona 4 Arena Ultimax**\n" +
                "<:P4D:751133120346062859> `P4D` - **Persona 4: Dancing All Night**\n" +
                "<:P5:751133123861020742> `P5` - **Persona 5**\n" +
                "<:P5S:852644176188669972> `P5S` - **Persona 5 Strikers**\n" +
                "<:BBTAG:751133123013771617> `BBTAG` - **BlazBlue: Cross Tag Battle**\n" +
                "\n" +
                "Use **`maker_list`** to view the names of characters available for a given title.\n");

            embed.WithImageUrl("https://i.imgur.com/hhnDVlB.png");

            menuSession.CurrentMenu = "SM_Tutorial_Basics_Page_2";

            var component = new ComponentBuilder()
                .WithButton("◀️ Previous Page", customId: "previous-page", ButtonStyle.Secondary)
                .WithButton("▶️ Next Page", customId: "next-page", ButtonStyle.Secondary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task SM_Tutorial_Basics_Page_3(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Using Sprite Sheets",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            var footer = new EmbedFooterBuilder
            {
                Text = "" +
                "◀️ Previous Page | ▶️ Next Page\n" +
                "Page 3 / 5"
            };

            embed.WithFooter(footer);

            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Profile_Help_Thumbnail(account));

            embed.WithDescription("" +
                $"When you find a character you want, use **`maker_sheet`** and type the character's name to view their sprite sheet.\n" +
                $"\n" +
                $"If the character appears in more than one title, this will only show their sprite sheet from the first game they appeared in.\n" +
                $"\n" +
                $"To specify their sprite sheet from another title, use the **`character_version`** option and choose one of the game styles from the list.");

            embed.WithImageUrl("https://i.imgur.com/Z0L6z5x.png");

            menuSession.CurrentMenu = "SM_Tutorial_Basics_Page_3";

            var component = new ComponentBuilder()
                .WithButton("◀️ Previous Page", customId: "previous-page", ButtonStyle.Secondary)
                .WithButton("▶️ Next Page", customId: "next-page", ButtonStyle.Secondary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task SM_Tutorial_Basics_Page_4(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Creating a Scene",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            var footer = new EmbedFooterBuilder
            {
                Text = "" +
                "◀️ Previous Page | ▶️ Next Page\n" +
                "Page 4 / 5"
            };

            embed.WithFooter(footer);

            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Profile_Help_Thumbnail(account));

            embed.WithDescription("" +
                $"To create a scene, use **`maker_create`** to type in a character's name, one of their sprite numbers, and the dialogue you want them to say.\n" +
                "\n" +
                $"You'll create a scene based on your current scene maker settings, which you can change by using the **`settings`** command and choosing [Scene Maker Settings].");

            embed.WithImageUrl("https://i.imgur.com/qIaEJeK.gif");

            menuSession.CurrentMenu = "SM_Tutorial_Basics_Page_4";

            var component = new ComponentBuilder()
                .WithButton("◀️ Previous Page", customId: "previous-page", ButtonStyle.Secondary)
                .WithButton("▶️ Next Page", customId: "next-page", ButtonStyle.Secondary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task SM_Tutorial_Basics_Page_5(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Backgrounds & Deleting Scenes",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            var footer = new EmbedFooterBuilder
            {
                Text = "Page 5 / 5"
            };

            embed.WithFooter(footer);

            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Profile_Help_Thumbnail(account));

            embed.WithDescription("" +
                "To use a background, upload an image alongside your command when creating a scene.\n" +
                "\n" +
                "You can also delete scenes you’ve already made by reacting to them with the :x: emote.");

            embed.WithImageUrl("https://i.imgur.com/YgYLPXW.png");

            menuSession.CurrentMenu = "SM_Tutorial_Basics_Page_5";

            var component = new ComponentBuilder()
                .WithButton("◀️ Previous Page", customId: "previous-page", ButtonStyle.Secondary)
                .WithButton("💠 Return to Tutorial Menu", customId: "back-to-tutorial-menu", ButtonStyle.Secondary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }
    }
}
