using System.Threading.Tasks;
using Discord;

namespace SocialLinker.Core.Menus.Help.Main
{
    class Status_Tutorial_Menu
    {
        public static async Task Status_Tutorial_Page_1(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Status Screens",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            var footer = new EmbedFooterBuilder
            {
                Text = "Page 1 / 5"
            };

            embed.WithFooter(footer);

            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Profile_Help_Thumbnail(account));

            embed.WithDescription("Status screens keep track of your various Discord activities. Depending on which theme you set your profile to, they can take on different appearances.");

            embed.WithImageUrl("https://i.imgur.com/jglT9wW.png");

            menuSession.CurrentMenu = "Status_Tutorial_Page_1";

            var component = new ComponentBuilder()
                .WithButton("↩️ Return", customId: "return", ButtonStyle.Secondary)
                .WithButton("▶️ Next Page", customId: "next-page", ButtonStyle.Secondary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Status_Tutorial_Page_2(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Social Stats",
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
                "Social stats are determined by how interactive you are with Social Linker. There’s a daily limit for points earned with each one, so come back often to see them grow.\n" +
                "\n" +
                "**• Proficiency:** Increases through use of Social Linker commands.\n" +
                "**• Diligence:** Increases through daily Discord activity.\n" +
                "**• Expression:** Increases through usage of social commands by you or by others.");

            embed.WithImageUrl("https://i.imgur.com/roeFJq8.png");

            menuSession.CurrentMenu = "Status_Tutorial_Page_2";

            var component = new ComponentBuilder()
                .WithButton("◀️ Previous Page", customId: "previous-page", ButtonStyle.Secondary)
                .WithButton("▶️ Next Page", customId: "next-page", ButtonStyle.Secondary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Status_Tutorial_Page_3(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Leveling Up",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            var footer = new EmbedFooterBuilder
            {
                Text = "Page 3 / 5"
            };

            embed.WithFooter(footer);

            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Profile_Help_Thumbnail(account));

            embed.WithDescription("" +
                "Levels are gained by earning experience points with messages you send. When you level up, you can obtain P-Medals to spend on unique décor for customizing your status screen.");

            embed.WithImageUrl("https://i.imgur.com/H9WEZnU.png");

            menuSession.CurrentMenu = "Status_Tutorial_Page_3";

            var component = new ComponentBuilder()
                .WithButton("◀️ Previous Page", customId: "previous-page", ButtonStyle.Secondary)
                .WithButton("▶️ Next Page", customId: "next-page", ButtonStyle.Secondary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Status_Tutorial_Page_4(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Earning P-Medals",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            var footer = new EmbedFooterBuilder
            {
                Text = "Page 4 / 5"
            };

            embed.WithFooter(footer);

            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Profile_Help_Thumbnail(account));

            embed.WithDescription("" +
                "Your social stat ranks and level determine how many P-Medals are gained on level up. P-Medals can also be gained once per day for simply being active.");

            menuSession.CurrentMenu = "Status_Tutorial_Page_4";

            var component = new ComponentBuilder()
                .WithButton("◀️ Previous Page", customId: "previous-page", ButtonStyle.Secondary)
                .WithButton("▶️ Next Page", customId: "next-page", ButtonStyle.Secondary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Status_Tutorial_Page_5(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Settings and Commands",
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
                $"You can set your profile theme at any time from the **`settings`** menu by choosing [Profile Theme Settings].\n" +
                "Use the following commands to access these main features:\n");

            embed.AddField($"Command List", 
                $"> **`status`**\n" +
                $"Check your status screen.\n" +
                $"\n" +
                $"> **`shop`**\n" +
                $"Access the status screen Décor Shop.");

            embed.WithImageUrl("https://i.imgur.com/ezUiOxl.png");

            menuSession.CurrentMenu = "Status_Tutorial_Page_5";

            var component = new ComponentBuilder()
                .WithButton("◀️ Previous Page", customId: "previous-page", ButtonStyle.Secondary)
                .WithButton("💠 Return to Help Menu", customId: "back-to-help-menu", ButtonStyle.Secondary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }
    }
}
