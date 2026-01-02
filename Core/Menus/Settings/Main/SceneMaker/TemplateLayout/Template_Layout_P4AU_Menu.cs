using System.Threading.Tasks;
using Discord;

namespace SocialLinker.Core.Menus.Settings.Main.SceneMaker.TemplateLayout
{
    class Template_Layout_P4AU_Menu
    {
        public static async Task Template_Layout_P4AU_Main(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Template Settings - Persona 4 Arena Ultimax",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            embed.WithColor(EmbedSettings.Get_Game_Color("P4AU", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P4AU"));

            embed.WithDescription("" +
                "Select a setting to edit.\n" +
                "\n" +
                ":one: Scene Type\n" +
                ":two: Auto Read\n" +
                ":three: Control Panel\n" +
                ":four: Sprite Placement\n" +
                ":five: Protagonist Highlight\n");

            menuSession.CurrentMenu = "Template_Layout_P4AU_Main";

            var selectMenu = new SelectMenuBuilder()
                    .WithPlaceholder("Select an option")
                    .WithCustomId(menuSession.CurrentMenu)
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .AddOption("Scene Type", "1", null, new Emoji("1️⃣"))
                    .AddOption("Auto Read", "2", null, new Emoji("2️⃣"))
                    .AddOption("Control Panel", "3", null, new Emoji("3️⃣"))
                    .AddOption("Sprite Placement", "4", null, new Emoji("4️⃣"))
                    .AddOption("Protagonist Highlight", "5", null, new Emoji("5️⃣"))
                    .AddOption("Return to Template Layout Menu", "return", null, new Emoji("↩️"));

            var component = new ComponentBuilder()
                .WithSelectMenu(selectMenu);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_P4AU_Scene_Type(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Scene Type",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            embed.WithColor(EmbedSettings.Get_Game_Color("P4AU", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P4AU"));

            embed.WithDescription("" +
                "Toggle between dialogue and narration formats for created scenes.\n" +
                "\n" +
                $"⚙️ **Current setting:** **`{account.P4AU_TS_Scene_Type}`**\n" +
                "\n" +
                ":one: Dialogue\n" +
                ":two: Narration\n");

            embed.WithImageUrl("https://i.imgur.com/Nj5I4Ov.png");

            menuSession.CurrentMenu = "Template_Layout_P4AU_Scene_Type";

            var selectMenu = new SelectMenuBuilder()
                    .WithPlaceholder("Select an option")
                    .WithCustomId(menuSession.CurrentMenu)
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .AddOption("Dialogue", "1", null, new Emoji("1️⃣"))
                    .AddOption("Narration", "2", null, new Emoji("2️⃣"))
                    .AddOption("Return to P4AU Template Settings", "return", null, new Emoji("↩️"));

            var component = new ComponentBuilder()
                .WithSelectMenu(selectMenu);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_P4AU_Auto_Advance(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Auto Read",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            var footer = new EmbedFooterBuilder
            {
                Text = "↩️ Return to P4AU Template Settings"
            };

            embed.WithFooter(footer);

            embed.WithColor(EmbedSettings.Get_Game_Color("P4AU", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P4AU"));

            embed.WithDescription("" +
                "Toggle the cursor's auto read color scheme on and off.\n" +
                "\n" +
                $"⚙️ **Current setting:** **`{account.P4AU_TS_Auto_Advance}`**\n");

            embed.WithImageUrl("https://i.imgur.com/8iSnblW.png");

            menuSession.CurrentMenu = "Template_Layout_P4AU_Auto_Advance";

            var component = new ComponentBuilder()
                .WithButton("↩️", customId: "return", ButtonStyle.Secondary)
                .WithButton("✅ On", customId: "on", ButtonStyle.Secondary)
                .WithButton("❌ Off", customId: "off", ButtonStyle.Secondary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_P4AU_Control_Panel(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Control Panel",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            embed.WithColor(EmbedSettings.Get_Game_Color("P4AU", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P4AU"));

            embed.WithDescription("" +
                "Toggle between multiple control guide layouts.\n" +
                "\n" +
                $"⚙️ **Current setting:** **`{account.P4AU_TS_Panel}`**\n" +
                "\n" +
                ":one: PlayStation®️ 3\n" +
                ":two: PlayStation®️ 4\n" +
                ":three: PlayStation®️ 4 (PC Layout)\n" +
                ":four: Xbox 360\n" +
                ":five: Xbox One (PC Layout)\n" +
                ":six: Nintendo Switch\n" +
                ":seven: Nintendo Switch (PC Layout)\n" +
                ":eight: Keyboard\n" +
                ":nine: None");

            embed.WithImageUrl("https://i.imgur.com/0hvNpig.png");

            menuSession.CurrentMenu = "Template_Layout_P4AU_Control_Panel";

            var selectMenu = new SelectMenuBuilder()
                    .WithPlaceholder("Select an option")
                    .WithCustomId(menuSession.CurrentMenu)
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .AddOption("PlayStation®️ 3", "1", null, new Emoji("1️⃣"))
                    .AddOption("PlayStation®️ 4", "2", null, new Emoji("2️⃣"))
                    .AddOption("PlayStation®️ 4 (PC Layout)", "3", null, new Emoji("3️⃣"))
                    .AddOption("Xbox 360", "4", null, new Emoji("4️⃣"))
                    .AddOption("Xbox One (PC Layout)", "5", null, new Emoji("5️⃣"))
                    .AddOption("Nintendo Switch", "6", null, new Emoji("6️⃣"))
                    .AddOption("Nintendo Switch (PC Layout)", "7", null, new Emoji("7️⃣"))
                    .AddOption("Keyboard", "8", null, new Emoji("8️⃣"))
                    .AddOption("None", "9", null, new Emoji("9️⃣"))
                    .AddOption("Return to P4AU Template Settings", "return", null, new Emoji("↩️"));

            var component = new ComponentBuilder()
                .WithSelectMenu(selectMenu);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_P4AU_Sprite_Placement(MenuIdStructure menuSession)
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

            embed.WithColor(EmbedSettings.Get_Game_Color("P4AU", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P4AU"));

            embed.WithDescription("" +
                "Choose the default position character sprites are rendered at.\n" +
                "\n" +
                $"⚙️ **Current setting:** **`{account.P4AU_TS_Position}`**\n" +
                "\n" +
                ":one: Left\n" +
                ":two: Center\n" +
                ":three: Right\n");

            embed.WithImageUrl("https://i.imgur.com/4H6iZF6.png");

            menuSession.CurrentMenu = "Template_Layout_P4AU_Sprite_Placement";

            var selectMenu = new SelectMenuBuilder()
                    .WithPlaceholder("Select an option")
                    .WithCustomId(menuSession.CurrentMenu)
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .AddOption("Left", "1", null, new Emoji("1️⃣"))
                    .AddOption("Center", "2", null, new Emoji("2️⃣"))
                    .AddOption("Right", "3", null, new Emoji("3️⃣"))
                    .AddOption("Return to P4AU Template Settings", "return", null, new Emoji("↩️"));

            var component = new ComponentBuilder()
                .WithSelectMenu(selectMenu);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_P4AU_Highlight(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Protagonist Highlight",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            var footer = new EmbedFooterBuilder
            {
                Text = "↩️ Return to P4AU Template Settings"
            };

            embed.WithFooter(footer);

            embed.WithColor(EmbedSettings.Get_Game_Color("P4AU", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P4AU"));

            embed.WithDescription("" +
                "Toggle the highlight for the scene’s protagonist on and off when speaking.\n" +
                "\n" +
                $"⚙️ **Current setting:** **`{account.P4AU_TS_Highlight}`**\n");

            embed.WithImageUrl("https://i.imgur.com/hhOHIKH.png");

            menuSession.CurrentMenu = "Template_Layout_P4AU_Highlight";

            var component = new ComponentBuilder()
                .WithButton("↩️", customId: "return", ButtonStyle.Secondary)
                .WithButton("✅ On", customId: "on", ButtonStyle.Secondary)
                .WithButton("❌ Off", customId: "off", ButtonStyle.Secondary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_P4AU_Scene_Type_Confirm(MenuIdStructure menuSession)
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

            embed.WithColor(EmbedSettings.Get_Game_Color("P4AU", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P4AU"));

            embed.WithDescription("" +
                $"The scene type has been set to **`{account.P4AU_TS_Scene_Type}`**.\n");

            menuSession.CurrentMenu = "Template_Layout_P4AU_Scene_Type_Confirm";

            var component = new ComponentBuilder()
                .WithButton("💠 P4AU Template Settings", customId: "back-to-p4au-template-settings", ButtonStyle.Primary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_P4AU_Auto_Advance_Confirm(MenuIdStructure menuSession)
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

            embed.WithColor(EmbedSettings.Get_Game_Color("P4AU", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P4AU"));

            embed.WithDescription("" +
                $"The auto read cursor has been set to **`{account.P4AU_TS_Auto_Advance}`**.\n");

            menuSession.CurrentMenu = "Template_Layout_P4AU_Auto_Advance_Confirm";

            var component = new ComponentBuilder()
                .WithButton("💠 P4AU Template Settings", customId: "back-to-p4au-template-settings", ButtonStyle.Primary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_P4AU_Control_Panel_Confirm(MenuIdStructure menuSession)
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

            embed.WithColor(EmbedSettings.Get_Game_Color("P4AU", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P4AU"));

            embed.WithDescription("" +
                $"The control panel has been set to **`{account.P4AU_TS_Panel}`**.\n");

            menuSession.CurrentMenu = "Template_Layout_P4AU_Control_Panel_Confirm";

            var component = new ComponentBuilder()
                .WithButton("💠 P4AU Template Settings", customId: "back-to-p4au-template-settings", ButtonStyle.Primary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_P4AU_Sprite_Placement_Confirm(MenuIdStructure menuSession)
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

            embed.WithColor(EmbedSettings.Get_Game_Color("P4AU", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P4AU"));

            embed.WithDescription("" +
                $"Sprite placements have been set to **`{account.P4AU_TS_Position}`**.\n");

            menuSession.CurrentMenu = "Template_Layout_P4AU_Sprite_Placement_Confirm";

            var component = new ComponentBuilder()
                .WithButton("💠 P4AU Template Settings", customId: "back-to-p4au-template-settings", ButtonStyle.Primary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_P4AU_Highlight_Confirm(MenuIdStructure menuSession)
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

            embed.WithColor(EmbedSettings.Get_Game_Color("P4AU", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P4AU"));

            embed.WithDescription("" +
                $"Highlights for right-sided sprites have been set to **`{account.P4AU_TS_Highlight}`** when speaking.\n");

            menuSession.CurrentMenu = "Template_Layout_P4AU_Highlight_Confirm";

            var component = new ComponentBuilder()
                .WithButton("💠 P4AU Template Settings", customId: "back-to-p4au-template-settings", ButtonStyle.Primary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }
    }
}
