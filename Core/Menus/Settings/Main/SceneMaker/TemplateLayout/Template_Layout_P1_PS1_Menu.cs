using System.Threading.Tasks;
using Discord;

namespace SocialLinker.Core.Menus.Settings.Main.SceneMaker.TemplateLayout
{
    class Template_Layout_P1_PS1_Menu
    {
        public static async Task Template_Layout_P1_PS1_Main(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Template Settings - Revelations: Persona",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            embed.WithColor(EmbedSettings.Get_Game_Color("P1-PS1", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P1-PS1"));

            embed.WithDescription("" +
                "Select a setting to edit.\n" +
                "\n" +
                ":one: Wallpaper\n" +
                ":two: Moon Phases\n" +
                ":three: Sprite Placement\n" +
                ":four: Darken Background\n" +
                ":five: Consistent Display Names\n" +
                ":six: Localized Display Names");

            var selectMenu = new SelectMenuBuilder()
                    .WithPlaceholder("Select an option")
                    .WithCustomId("template-layout-p1-ps1-main")
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .AddOption("Wallpaper", "1", null, new Emoji("1️⃣"))
                    .AddOption("Moon Phases", "2", null, new Emoji("2️⃣"))
                    .AddOption("Sprite Placement", "3", null, new Emoji("3️⃣"))
                    .AddOption("Darken Background", "4", null, new Emoji("4️⃣"))
                    .AddOption("Consistent Display Names", "5", null, new Emoji("5️⃣"))
                    .AddOption("Localized Display Names", "6", null, new Emoji("6️⃣"))
                    .AddOption("Return to Version Select", "return", null, new Emoji("↩️"));

            var component = new ComponentBuilder()
                .WithSelectMenu(selectMenu);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);

            menuSession.CurrentMenu = "Template_Layout_P1_PS1_Main";
        }

        public static async Task Template_Layout_P1_PS1_Wallpaper(MenuIdStructure menuSession)
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

            embed.WithColor(EmbedSettings.Get_Game_Color("P1-PS1", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P1-PS1"));

            embed.WithDescription("" +
                "Change the background of the message window. Pick the one that suits you.\n" +
                "\n" +
                $"⚙️ **Current setting:** **`{account.P1_PSX_TS_Wallpaper}`**\n" +
                "\n" +
                ":one: Type 1\n" +
                ":two: Type 2\n" +
                ":three: Type 3\n" +
                ":four: Type 4\n" +
                ":five: Type 5\n" +
                ":six: Type 6\n" +
                ":seven: Type 7\n" +
                ":eight: Type 8\n");

            embed.WithImageUrl("https://i.imgur.com/SE0PWwd.png");

            var selectMenu = new SelectMenuBuilder()
                    .WithPlaceholder("Select an option")
                    .WithCustomId("template-layout-p1-ps1-wallpaper")
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .AddOption("Type 1", "1", null, new Emoji("1️⃣"))
                    .AddOption("Type 2", "2", null, new Emoji("2️⃣"))
                    .AddOption("Type 3", "3", null, new Emoji("3️⃣"))
                    .AddOption("Type 4", "4", null, new Emoji("4️⃣"))
                    .AddOption("Type 5", "5", null, new Emoji("5️⃣"))
                    .AddOption("Type 6", "6", null, new Emoji("6️⃣"))
                    .AddOption("Type 7", "7", null, new Emoji("7️⃣"))
                    .AddOption("Type 8", "8", null, new Emoji("8️⃣"))
                    .AddOption("Return to Version Select", "return", null, new Emoji("↩️"));

            var component = new ComponentBuilder()
                .WithSelectMenu(selectMenu);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);

            menuSession.CurrentMenu = "Template_Layout_P1_PS1_Wallpaper";
        }

        public static async Task Template_Layout_P1_PS1_Moon_Phases(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Moon Phases",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            var footer = new EmbedFooterBuilder
            {
                Text = "↩️ Return to Revelations: Persona Template Settings"
            };

            embed.WithFooter(footer);

            embed.WithColor(EmbedSettings.Get_Game_Color("P1-PS1", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P1-PS1"));

            embed.WithDescription("" +
                "Toggle the moon phases on and off in created scenes.\n" +
                "\n" +
                $"⚙️ **Current setting:** **`{account.P1_PSX_TS_Moon_HUD}`**\n");

            embed.WithImageUrl("https://i.imgur.com/xbEo69c.png");

            var component = new ComponentBuilder()
                .WithButton("↩️", customId: "return", ButtonStyle.Secondary)
                .WithButton("✅ On", customId: "on", ButtonStyle.Secondary)
                .WithButton("❌ Off", customId: "off", ButtonStyle.Secondary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);

            menuSession.CurrentMenu = "Template_Layout_P1_PS1_Moon_Phases";
        }

        public static async Task Template_Layout_P1_PS1_Placement(MenuIdStructure menuSession)
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

            embed.WithColor(EmbedSettings.Get_Game_Color("P1-PS1", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P1-PS1"));

            embed.WithDescription("" +
                "Choose the default position character sprites are rendered at.\n" +
                "\n" +
                $"⚙️ **Current setting:** **`{account.P1_PSX_TS_Position}`**\n" +
                "\n" +
                ":one: Left\n" +
                ":two: Center\n" +
                ":three: Right\n" +
                ":four: Switch - *Contextually switch between left, right, and center placements for up to three used characters in a conversation.*\n");

            embed.WithImageUrl("https://i.imgur.com/OsvwaV7.png");

            var selectMenu = new SelectMenuBuilder()
                    .WithPlaceholder("Select an option")
                    .WithCustomId("template-layout-p1-ps1-placement")
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .AddOption("Left", "left", null, new Emoji("1️⃣"))
                    .AddOption("Right", "right", null, new Emoji("2️⃣"))
                    .AddOption("Center", "center", null, new Emoji("3️⃣"))
                    .AddOption("Switch", "switch", null, new Emoji("4️⃣"))
                    .AddOption("Return to Revelations: Persona Template Settings", "return", null, new Emoji("↩️"));

            var component = new ComponentBuilder()
                .WithSelectMenu(selectMenu);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);

            menuSession.CurrentMenu = "Template_Layout_P1_PS1_Placement";
        }

        public static async Task Template_Layout_P1_PS1_BG_Darken(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Darken Background",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            var footer = new EmbedFooterBuilder
            {
                Text = "↩️ Return to Revelations: Persona Template Settings"
            };

            embed.WithFooter(footer);

            embed.WithColor(EmbedSettings.Get_Game_Color("P1-PS1", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P1-PS1"));

            embed.WithDescription("" +
                "Toggle a filter to darken background elements on and off.\n" +
                "\n" +
                $"⚙️ **Current setting:** **`{account.P1_PSX_TS_BG_Darken}`**\n");

            embed.WithImageUrl("https://i.imgur.com/zxMGylO.png");

            var component = new ComponentBuilder()
                .WithButton("↩️", customId: "return", ButtonStyle.Secondary)
                .WithButton("✅ On", customId: "on", ButtonStyle.Secondary)
                .WithButton("❌ Off", customId: "off", ButtonStyle.Secondary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);

            menuSession.CurrentMenu = "Template_Layout_P1_PS1_BG_Darken";
        }

        public static async Task Template_Layout_P1_PS1_Consistent_Names(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Consistent Display Names",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            var footer = new EmbedFooterBuilder
            {
                Text = "↩️ Return to Revelations: Persona Template Settings"
            };

            embed.WithFooter(footer);

            embed.WithColor(EmbedSettings.Get_Game_Color("P1-PS1", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P1-PS1"));

            embed.WithDescription("" +
                "Set the toggle on to always display a character's name, or off to hide their name for consecutive scenes they speak in.\n" +
                "\n" +
                $"⚙️ **Current setting:** **`{account.P1_PSX_TS_Consistent_Names}`**\n");

            embed.WithImageUrl("https://i.imgur.com/1v4HjkQ.png");

            var component = new ComponentBuilder()
                .WithButton("↩️", customId: "return", ButtonStyle.Secondary)
                .WithButton("✅ On", customId: "on", ButtonStyle.Secondary)
                .WithButton("❌ Off", customId: "off", ButtonStyle.Secondary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);

            menuSession.CurrentMenu = "Template_Layout_P1_PS1_Consistent_Names";
        }

        public static async Task Template_Layout_P1_PS1_Localized_Names(MenuIdStructure menuSession)
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
                Text = "↩️ Return to Revelations: Persona Template Settings"
            };

            embed.WithFooter(footer);

            embed.WithColor(EmbedSettings.Get_Game_Color("P1-PS1", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P1-PS1"));

            embed.WithDescription("" +
                "Set characters to have their localized display names on or off. This can still be overridden with custom display names.\n" +
                "\n" +
                $"⚙️ **Current setting:** **`{account.P1_PSX_TS_Localized_Revelations_Names}`**\n");

            embed.WithImageUrl("https://i.imgur.com/opEvVqv.png");

            var component = new ComponentBuilder()
                .WithButton("↩️", customId: "return", ButtonStyle.Secondary)
                .WithButton("✅ On", customId: "on", ButtonStyle.Secondary)
                .WithButton("❌ Off", customId: "off", ButtonStyle.Secondary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);

            menuSession.CurrentMenu = "Template_Layout_P1_PS1_Localized_Names";
        }

        public static async Task Template_Layout_P1_PS1_Wallpaper_Confirm(MenuIdStructure menuSession)
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

            embed.WithColor(EmbedSettings.Get_Game_Color("P1-PS1", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P1-PS1"));

            embed.WithDescription("" +
                $"Your wallpaper has been set to **`{account.P1_PSX_TS_Wallpaper}`**.\n");

            var component = new ComponentBuilder()
                .WithButton("💠 Revelations: Persona Template Settings", customId: "back-to-p1-ps1-template-settings", ButtonStyle.Primary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);

            menuSession.CurrentMenu = "Template_Layout_P1_PS1_Wallpaper_Confirm";
        }

        public static async Task Template_Layout_P1_PS1_Moon_Phases_Confirm(MenuIdStructure menuSession)
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

            embed.WithColor(EmbedSettings.Get_Game_Color("P1-PS1", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P1-PS1"));

            embed.WithDescription("" +
                $"Moon phases have been set to **`{account.P1_PSX_TS_Moon_HUD}`**.\n");

            var component = new ComponentBuilder()
                .WithButton("💠 Revelations: Persona Template Settings", customId: "back-to-p1-ps1-template-settings", ButtonStyle.Primary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);

            menuSession.CurrentMenu = "Template_Layout_P1_PS1_Moon_Phases_Confirm";
        }

        public static async Task Template_Layout_P1_PS1_Placement_Confirm(MenuIdStructure menuSession)
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

            embed.WithColor(EmbedSettings.Get_Game_Color("P1-PS1", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P1-PS1"));

            embed.WithDescription("" +
                $"Sprite placements have been set to **`{account.P1_PSX_TS_Position}`**.\n");

            var component = new ComponentBuilder()
                .WithButton("💠 Revelations: Persona Template Settings", customId: "back-to-p1-ps1-template-settings", ButtonStyle.Primary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);

            menuSession.CurrentMenu = "Template_Layout_P1_PS1_Placement_Confirm";
        }

        public static async Task Template_Layout_P1_PS1_BG_Darken_Confirm(MenuIdStructure menuSession)
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

            embed.WithColor(EmbedSettings.Get_Game_Color("P1-PS1", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P1-PS1"));

            embed.WithDescription("" +
                $"The Darken Background toggle has been set to **`{account.P1_PSX_TS_BG_Darken}`**.\n");

            var component = new ComponentBuilder()
                .WithButton("💠 Revelations: Persona Template Settings", customId: "back-to-p1-ps1-template-settings", ButtonStyle.Primary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);

            menuSession.CurrentMenu = "Template_Layout_P1_PS1_BG_Darken_Confirm";
        }

        public static async Task Template_Layout_P1_PS1_Consistent_Names_Confirm(MenuIdStructure menuSession)
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

            var footer = new EmbedFooterBuilder
            {
                Text = "💠 Revelations: Persona Template Settings | ❌ Close Menu"
            };

            embed.WithFooter(footer);

            embed.WithColor(EmbedSettings.Get_Game_Color("P1-PS1", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P1-PS1"));

            embed.WithDescription("" +
                $"Consistent display names have been set to **`{account.P1_PSX_TS_Consistent_Names}`**.\n");

            var component = new ComponentBuilder()
                .WithButton("💠 Revelations: Persona Template Settings", customId: "back-to-p1-ps1-template-settings", ButtonStyle.Primary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);

            menuSession.CurrentMenu = "Template_Layout_P1_PS1_Consistent_Names_Confirm";
        }

        public static async Task Template_Layout_P1_PS1_Localized_Names_Confirm(MenuIdStructure menuSession)
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

            embed.WithColor(EmbedSettings.Get_Game_Color("P1-PS1", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P1-PS1"));

            embed.WithDescription("" +
                $"Localized display names have been set to **`{account.P1_PSX_TS_Localized_Revelations_Names}`**.\n");

            var component = new ComponentBuilder()
                .WithButton("💠 Revelations: Persona Template Settings", customId: "back-to-p1-ps1-template-settings", ButtonStyle.Primary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);

            menuSession.CurrentMenu = "Template_Layout_P1_PS1_Localized_Names_Confirm";
        }
    }
}
