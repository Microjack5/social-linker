using System.Threading.Tasks;
using Discord;

namespace SocialLinker.Core.Menus.Settings.Main.SceneMaker.TemplateLayout
{
    class Template_Layout_P2IS_PS1_Menu
    {
        public static async Task Template_Layout_P2IS_PS1_Main(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Template Settings - Persona 2: Innocent Sin (PlayStation®️)",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            embed.WithColor(EmbedSettings.Get_Game_Color("P2IS-PS1", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P2IS-PS1"));

            embed.WithDescription("" +
                "Select a setting to edit.\n" +
                "\n" +
                ":one: Wallpaper\n" +
                ":two: Inverted Filter\n" +
                ":three: Sprite Placement\n" +
                ":four: Sprite Flip\n" +
                ":five: Localized Display Names");

            menuSession.CurrentMenu = "Template_Layout_P2IS_PS1_Main";

            var selectMenu = new SelectMenuBuilder()
                    .WithPlaceholder("Select an option")
                    .WithCustomId(menuSession.CurrentMenu)
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .AddOption("Wallpaper", "1", null, new Emoji("1️⃣"))
                    .AddOption("Inverted Filter", "2", null, new Emoji("2️⃣"))
                    .AddOption("Sprite Placement", "3", null, new Emoji("3️⃣"))
                    .AddOption("Sprite Flip", "4", null, new Emoji("4️⃣"))
                    .AddOption("Localized Display Names", "5", null, new Emoji("5️⃣"))
                    .AddOption("Return to P2IS Template Settings", "return", null, new Emoji("↩️"));

            var component = new ComponentBuilder()
                .WithSelectMenu(selectMenu);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_P2IS_PS1_Wallpaper(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Wallpaper",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            embed.WithColor(EmbedSettings.Get_Game_Color("P2IS-PS1", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P2IS-PS1"));

            embed.WithDescription("" +
                "Change the background of the message window. Pick the one that suits you.\n" +
                "\n" +
                $"⚙️ **Current setting:** **`{account.P2IS_PSX_TS_Wallpaper}`**\n" +
                "\n" +
                ":one: Blue Tone\n" +
                ":two: Sepia Tone\n" +
                ":three: Purple Tone\n" +
                ":four: Jack Frost\n" +
                ":five: Star\n" +
                ":six: Punched Metal\n" +
                ":seven: Seventh\n" +
                ":eight: Cuss High\n" +
                ":nine: Butterfly\n" +
                ":keycap_ten: Grid\n");

            embed.WithImageUrl("https://i.imgur.com/HhSv6zr.png");

            menuSession.CurrentMenu = "Template_Layout_P2IS_PS1_Wallpaper";

            var selectMenu = new SelectMenuBuilder()
                    .WithPlaceholder("Select an option")
                    .WithCustomId(menuSession.CurrentMenu)
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .AddOption("Blue Tone", "1", null, new Emoji("1️⃣"))
                    .AddOption("Sepia Tone", "2", null, new Emoji("2️⃣"))
                    .AddOption("Purple Tone", "3", null, new Emoji("3️⃣"))
                    .AddOption("Jack Frost", "4", null, new Emoji("4️⃣"))
                    .AddOption("Star", "5", null, new Emoji("5️⃣"))
                    .AddOption("Punched Metal", "6", null, new Emoji("6️⃣"))
                    .AddOption("Seventh", "7", null, new Emoji("7️⃣"))
                    .AddOption("Cuss High", "8", null, new Emoji("8️⃣"))
                    .AddOption("Butterfly", "9", null, new Emoji("9️⃣"))
                    .AddOption("Grid", "10", null, new Emoji("🔟"))
                    .AddOption("Return to P2IS Template Settings", "return", null, new Emoji("↩️"));

            var component = new ComponentBuilder()
                .WithSelectMenu(selectMenu);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_P2IS_PS1_Inverted_Filter(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Inverted Filter",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            var footer = new EmbedFooterBuilder
            {
                Text = "↩️ Return to P2IS Template Settings"
            };

            embed.WithFooter(footer);

            embed.WithColor(EmbedSettings.Get_Game_Color("P2IS-PS1", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P2IS-PS1"));

            embed.WithDescription("" +
                "Toggle inverted colors for character sprites on and off.\n" +
                "\n" +
                $"⚙️ **Current setting:** **`{account.P2IS_PSX_TS_Invert}`**\n");

            embed.WithImageUrl("https://i.imgur.com/UswByeQ.png");

            menuSession.CurrentMenu = "Template_Layout_P2IS_PS1_Inverted_Filter";

            var component = new ComponentBuilder()
                .WithButton("↩️", customId: "return", ButtonStyle.Secondary)
                .WithButton("✅ On", customId: "on", ButtonStyle.Secondary)
                .WithButton("❌ Off", customId: "off", ButtonStyle.Secondary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_P2IS_PS1_Placement(MenuIdStructure menuSession)
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

            embed.WithColor(EmbedSettings.Get_Game_Color("P2IS-PS1", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P2IS-PS1"));

            embed.WithDescription("" +
                "Choose the default position character sprites are rendered at.\n" +
                "\n" +
                $"⚙️ **Current setting:** **`{account.P2IS_PSX_TS_Position}`**\n" +
                "\n" +
                ":one: Default - *Use the set default positions for each sprite.*\n" +
                ":two: Left\n" +
                ":three: Right\n");

            embed.WithImageUrl("https://i.imgur.com/RNvrNdF.png");

            menuSession.CurrentMenu = "Template_Layout_P2IS_PS1_Placement";

            var selectMenu = new SelectMenuBuilder()
                    .WithPlaceholder("Select an option")
                    .WithCustomId(menuSession.CurrentMenu)
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .AddOption("Default", "1", null, new Emoji("1️⃣"))
                    .AddOption("Left", "2", null, new Emoji("2️⃣"))
                    .AddOption("Right", "3", null, new Emoji("3️⃣"))
                    .AddOption("Return to P2IS Template Settings", "return", null, new Emoji("↩️"));

            var component = new ComponentBuilder()
                .WithSelectMenu(selectMenu);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_P2IS_PS1_Sprite_Flip(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Sprite Flip",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            var footer = new EmbedFooterBuilder
            {
                Text = "↩️ Return to P2IS Template Settings"
            };

            embed.WithFooter(footer);

            embed.WithColor(EmbedSettings.Get_Game_Color("P2IS-PS1", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P2IS-PS1"));

            embed.WithDescription("" +
                "Choose to flip sprites horizontally from their default orientation.\n" +
                "\n" +
                $"⚙️ **Current setting:** **`{account.P2IS_PSX_TS_Sprite_Flip}`**\n");

            embed.WithImageUrl("https://i.imgur.com/ianHbiO.png");

            menuSession.CurrentMenu = "Template_Layout_P2IS_PS1_Sprite_Flip";

            var component = new ComponentBuilder()
                .WithButton("↩️", customId: "return", ButtonStyle.Secondary)
                .WithButton("✅ On", customId: "on", ButtonStyle.Secondary)
                .WithButton("❌ Off", customId: "off", ButtonStyle.Secondary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_P2IS_PS1_Localized_Names(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Localized Display Names",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            var footer = new EmbedFooterBuilder
            {
                Text = "↩️ Return to P2IS Template Settings"
            };

            embed.WithFooter(footer);

            embed.WithColor(EmbedSettings.Get_Game_Color("P2IS-PS1", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P2IS-PS1"));

            embed.WithDescription("" +
                "Set returning characters from Revelations: Persona to have their localized display names on or off. This can still be overridden with custom display names.\n" +
                "\n" +
                $"⚙️ **Current setting:** **`{account.P2IS_PSX_TS_Localized_Revelations_Names}`**\n");

            menuSession.CurrentMenu = "Template_Layout_P2IS_PS1_Localized_Names";

            var component = new ComponentBuilder()
                .WithButton("↩️", customId: "return", ButtonStyle.Secondary)
                .WithButton("✅ On", customId: "on", ButtonStyle.Secondary)
                .WithButton("❌ Off", customId: "off", ButtonStyle.Secondary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_P2IS_PS1_Wallpaper_Confirm(MenuIdStructure menuSession)
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

            embed.WithColor(EmbedSettings.Get_Game_Color("P2IS-PS1", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P2IS-PS1"));

            embed.WithDescription("" +
                $"Your wallpaper has been set to **`{account.P2IS_PSX_TS_Wallpaper}`**.\n");

            menuSession.CurrentMenu = "Template_Layout_P2IS_PS1_Wallpaper_Confirm";

            var component = new ComponentBuilder()
                .WithButton("💠 P2IS Template Settings", customId: "back-to-p2is-ps1-template-settings", ButtonStyle.Primary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_P2IS_PS1_Inverted_Filter_Confirm(MenuIdStructure menuSession)
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

            embed.WithColor(EmbedSettings.Get_Game_Color("P2IS-PS1", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P2IS-PS1"));

            embed.WithDescription("" +
                $"Inverted colors for character sprites have been set to **`{account.P2IS_PSX_TS_Invert}`**.\n");

            menuSession.CurrentMenu = "Template_Layout_P2IS_PS1_Inverted_Filter_Confirm";

            var component = new ComponentBuilder()
                .WithButton("💠 P2IS Template Settings", customId: "back-to-p2is-ps1-template-settings", ButtonStyle.Primary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_P2IS_PS1_Placement_Confirm(MenuIdStructure menuSession)
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

            embed.WithColor(EmbedSettings.Get_Game_Color("P2IS-PS1", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P2IS-PS1"));

            embed.WithDescription("" +
                $"Sprite placements have been set to **`{account.P2IS_PSX_TS_Position}`**.\n");

            menuSession.CurrentMenu = "Template_Layout_P2IS_PS1_Placement_Confirm";

            var component = new ComponentBuilder()
                .WithButton("💠 P2IS Template Settings", customId: "back-to-p2is-ps1-template-settings", ButtonStyle.Primary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_P2IS_PS1_Sprite_Flip_Confirm(MenuIdStructure menuSession)
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

            embed.WithColor(EmbedSettings.Get_Game_Color("P2IS-PS1", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P2IS-PS1"));

            embed.WithDescription("" +
                $"Sprite flip has been set to **`{account.P2IS_PSX_TS_Sprite_Flip}`**.\n");

            menuSession.CurrentMenu = "Template_Layout_P2IS_PS1_Sprite_Flip_Confirm";

            var component = new ComponentBuilder()
                .WithButton("💠 P2IS Template Settings", customId: "back-to-p2is-ps1-template-settings", ButtonStyle.Primary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_P2IS_PS1_Localized_Names_Confirm(MenuIdStructure menuSession)
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

            embed.WithColor(EmbedSettings.Get_Game_Color("P2IS-PS1", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P2IS-PS1"));

            embed.WithDescription("" +
                $"Localized display names have been set to **`{account.P2IS_PSX_TS_Localized_Revelations_Names}`**.\n");

            menuSession.CurrentMenu = "Template_Layout_P2IS_PS1_Localized_Names_Confirm";

            var component = new ComponentBuilder()
                .WithButton("💠 P2IS Template Settings", customId: "back-to-p2is-ps1-template-settings", ButtonStyle.Primary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }
    }
}
