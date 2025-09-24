using System.Threading.Tasks;
using Discord;

namespace SocialLinker.Core.Menus.Settings.Main.SceneMaker.TemplateLayout
{
    class Template_Layout_BBTAG_Menu
    {
        public static async Task Template_Layout_BBTAG_Main(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Template Settings - BlazBlue: Cross Tag Battle",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            embed.WithColor(EmbedSettings.Get_Game_Color("BBTAG", account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("BBTAG"));

            embed.WithDescription("" +
                "Select a setting to edit.\n" +
                "\n" +
                ":one: Episode Header\n" +
                ":two: Sprite Placement\n" +
                ":three: Background Blur\n");

            menuSession.CurrentMenu = "Template_Layout_BBTAG_Main";

            var selectMenu = new SelectMenuBuilder()
                    .WithPlaceholder("Select an option")
                    .WithCustomId(menuSession.CurrentMenu)
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .AddOption("Episode Header", "1", null, new Emoji("1️⃣"))
                    .AddOption("Sprite Placement", "2", null, new Emoji("2️⃣"))
                    .AddOption("Background Blur", "3", null, new Emoji("3️⃣"))
                    .AddOption("Return to Template Layout Menu", "return", null, new Emoji("↩️"));

            var component = new ComponentBuilder()
                .WithSelectMenu(selectMenu);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_BBTAG_Header(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Episode Header",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            embed.WithColor(EmbedSettings.Get_Game_Color("BBTAG", account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("BBTAG"));

            embed.WithDescription("" +
                "Choose an episode header to display. The chapters will change throughout the week.\n" +
                "\n" +
                $"⚙️ **Current setting:** **`{account.BBTAG_TS_Header}`**\n" +
                "\n" +
                ":one: Prologue\n" +
                ":two: Episode BlazBlue\n" +
                ":three: Episode P4A\n" +
                ":four: Episode Under Night In-Birth\n" +
                ":five: Episode RWBY\n" +
                ":six: Episode Extra\n");

            embed.WithImageUrl("https://i.imgur.com/eMdAI12.png");

            menuSession.CurrentMenu = "Template_Layout_BBTAG_Header";

            var selectMenu = new SelectMenuBuilder()
                    .WithPlaceholder("Select an option")
                    .WithCustomId(menuSession.CurrentMenu)
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .AddOption("Prologue", "1", null, new Emoji("1️⃣"))
                    .AddOption("Episode BlazBlue", "2", null, new Emoji("2️⃣"))
                    .AddOption("Episode P4A", "3", null, new Emoji("3️⃣"))
                    .AddOption("Episode Under Night In-Birth", "4", null, new Emoji("4️⃣"))
                    .AddOption("Episode RWBY", "5", null, new Emoji("5️⃣"))
                    .AddOption("Episode Extra", "6", null, new Emoji("6️⃣"))
                    .AddOption("Return to BBTAG Template Settings", "return", null, new Emoji("↩️"));

            var component = new ComponentBuilder()
                .WithSelectMenu(selectMenu);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_BBTAG_Sprite_Placement(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Sprite Placement",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            embed.WithColor(EmbedSettings.Get_Game_Color("BBTAG", account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("BBTAG"));

            embed.WithDescription("" +
                "Choose the default position character sprites are rendered at.\n" +
                "\n" +
                $"⚙️ **Current setting:** **`{account.BBTAG_TS_Position}`**\n" +
                "\n" +
                ":one: Left\n" +
                ":two: Center\n" +
                ":three: Right\n");

            embed.WithImageUrl("https://i.imgur.com/CP6SYHc.png");

            menuSession.CurrentMenu = "Template_Layout_BBTAG_Sprite_Placement";

            var selectMenu = new SelectMenuBuilder()
                    .WithPlaceholder("Select an option")
                    .WithCustomId(menuSession.CurrentMenu)
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .AddOption("Left", "1", null, new Emoji("1️⃣"))
                    .AddOption("Center", "2", null, new Emoji("2️⃣"))
                    .AddOption("Right", "3", null, new Emoji("3️⃣"))
                    .AddOption("Return to BBTAG Template Settings", "return", null, new Emoji("↩️"));

            var component = new ComponentBuilder()
                .WithSelectMenu(selectMenu);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_BBTAG_Background_Blur(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Background Blur",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            var footer = new EmbedFooterBuilder
            {
                Text = "↩️ Return to BBTAG Template Settings"
            };

            embed.WithFooter(footer);

            embed.WithColor(EmbedSettings.Get_Game_Color("BBTAG", account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("BBTAG"));

            embed.WithDescription("" +
                "Choose to toggle background blur on or off. Will slightly increase load times.\n" +
                "\n" +
                $"⚙️ **Current setting:** **`{account.BBTAG_TS_BG_Blur}`**\n");

            embed.WithImageUrl("https://i.imgur.com/kha38dk.png");

            menuSession.CurrentMenu = "Template_Layout_BBTAG_Background_Blur";

            var component = new ComponentBuilder()
                .WithButton("↩️", customId: "return", ButtonStyle.Secondary)
                .WithButton("✅ On", customId: "on", ButtonStyle.Secondary)
                .WithButton("❌ Off", customId: "off", ButtonStyle.Secondary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_BBTAG_Header_Confirm(MenuIdStructure menuSession)
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

            embed.WithColor(EmbedSettings.Get_Game_Color("BBTAG", account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("BBTAG"));

            embed.WithDescription("" +
                $"The episode header has been set to **`{account.BBTAG_TS_Header}`**.\n");

            menuSession.CurrentMenu = "Template_Layout_BBTAG_Header_Confirm";

            var component = new ComponentBuilder()
                .WithButton("💠 BBTAG Template Settings", customId: "back-to-bbtag-template-settings", ButtonStyle.Primary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_BBTAG_Sprite_Placement_Confirm(MenuIdStructure menuSession)
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

            embed.WithColor(EmbedSettings.Get_Game_Color("BBTAG", account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("BBTAG"));

            embed.WithDescription("" +
                $"Sprite placements have been set to **`{account.BBTAG_TS_Position}`**.\n");

            menuSession.CurrentMenu = "Template_Layout_BBTAG_Sprite_Placement_Confirm";

            var component = new ComponentBuilder()
                .WithButton("💠 BBTAG Template Settings", customId: "back-to-bbtag-template-settings", ButtonStyle.Primary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_BBTAG_Background_Blur_Confirm(MenuIdStructure menuSession)
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

            embed.WithColor(EmbedSettings.Get_Game_Color("BBTAG", account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("BBTAG"));

            embed.WithDescription("" +
                $"Background blur has been set to **`{account.BBTAG_TS_BG_Blur}`**.\n");

            menuSession.CurrentMenu = "Template_Layout_BBTAG_Background_Blur_Confirm";

            var component = new ComponentBuilder()
                .WithButton("💠 BBTAG Template Settings", customId: "back-to-bbtag-template-settings", ButtonStyle.Primary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }
    }
}
