using System.Threading.Tasks;
using Discord;

namespace SocialLinker.Core.Menus.Settings.Main.SceneMaker.TemplateLayout
{
    class Template_Layout_P4D_Menu
    {
        public static async Task Template_Layout_P4D_Main(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Template Settings - Persona 4: Dancing All Night",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            embed.WithColor(EmbedSettings.Get_Game_Color("P4D", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P4D"));

            embed.WithDescription("" +
                "Select a setting to edit.\n" +
                "\n" +
                ":one: Scene Type\n" +
                ":two: Auto Advance\n" +
                ":three: Sprite Placement\n");

            menuSession.CurrentMenu = "Template_Layout_P4D_Main";

            var selectMenu = new SelectMenuBuilder()
                    .WithPlaceholder("Select an option")
                    .WithCustomId(menuSession.CurrentMenu)
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .AddOption("Scene Type", "1", null, new Emoji("1️⃣"))
                    .AddOption("Auto Advance", "2", null, new Emoji("2️⃣"))
                    .AddOption("Sprite Placement", "3", null, new Emoji("3️⃣"))
                    .AddOption("Return to Template Layout Menu", "return", null, new Emoji("↩️"));

            var component = new ComponentBuilder()
                .WithSelectMenu(selectMenu);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_P4D_Scene_Type(MenuIdStructure menuSession)
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

            embed.WithColor(EmbedSettings.Get_Game_Color("P4D", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P4D"));

            embed.WithDescription("" +
                "Toggle between dialogue and narration formats for created scenes.\n" +
                "\n" +
                $"⚙️ **Current setting:** **`{account.P4D_TS_Scene_Type}`**\n" +
                "\n" +
                ":one: Dialogue\n" +
                ":two: Narration\n");

            embed.WithImageUrl("https://i.imgur.com/sw6lGdd.png");

            menuSession.CurrentMenu = "Template_Layout_P4D_Scene_Type";

            var selectMenu = new SelectMenuBuilder()
                    .WithPlaceholder("Select an option")
                    .WithCustomId(menuSession.CurrentMenu)
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .AddOption("Dialogue", "1", null, new Emoji("1️⃣"))
                    .AddOption("Narration", "2", null, new Emoji("2️⃣"))
                    .AddOption("Return to P4D Template Settings", "return", null, new Emoji("↩️"));

            var component = new ComponentBuilder()
                .WithSelectMenu(selectMenu);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_P4D_Auto_Advance(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Auto Advance",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            var footer = new EmbedFooterBuilder
            {
                Text = "↩️ Return to P4D Template Settings"
            };

            embed.WithFooter(footer);

            embed.WithColor(EmbedSettings.Get_Game_Color("P4D", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P4D"));

            embed.WithDescription("" +
                "Toggle the cursor's auto advance color scheme on and off.\n" +
                "\n" +
                $"⚙️ **Current setting:** **`{account.P4D_TS_Auto_Advance}`**\n");

            embed.WithImageUrl("https://i.imgur.com/sqvuS8v.png");

            menuSession.CurrentMenu = "Template_Layout_P4D_Auto_Advance";

            var component = new ComponentBuilder()
                .WithButton("↩️", customId: "return", ButtonStyle.Secondary)
                .WithButton("✅ On", customId: "on", ButtonStyle.Secondary)
                .WithButton("❌ Off", customId: "off", ButtonStyle.Secondary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_P4D_Sprite_Placement(MenuIdStructure menuSession)
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

            embed.WithColor(EmbedSettings.Get_Game_Color("P4D", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P4D"));

            embed.WithDescription("" +
                "Choose the default position character sprites are rendered at.\n" +
                "\n" +
                $"⚙️ **Current setting:** **`{account.P4D_TS_Position}`**\n" +
                "\n" +
                ":one: Left\n" +
                ":two: Center\n" +
                ":three: Right\n");

            embed.WithImageUrl("https://i.imgur.com/e3grgJw.png");

            menuSession.CurrentMenu = "Template_Layout_P4D_Sprite_Placement";

            var selectMenu = new SelectMenuBuilder()
                    .WithPlaceholder("Select an option")
                    .WithCustomId(menuSession.CurrentMenu)
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .AddOption("Left", "1", null, new Emoji("1️⃣"))
                    .AddOption("Center", "2", null, new Emoji("2️⃣"))
                    .AddOption("Right", "3", null, new Emoji("3️⃣"))
                    .AddOption("Return to P4D Template Settings", "return", null, new Emoji("↩️"));

            var component = new ComponentBuilder()
                .WithSelectMenu(selectMenu);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_P4D_Scene_Type_Confirm(MenuIdStructure menuSession)
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

            embed.WithColor(EmbedSettings.Get_Game_Color("P4D", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P4D"));

            embed.WithDescription("" +
                $"The scene type has been set to **`{account.P4D_TS_Scene_Type}`**.\n");

            menuSession.CurrentMenu = "Template_Layout_P4D_Scene_Type_Confirm";

            var component = new ComponentBuilder()
                .WithButton("💠 P4D Template Settings", customId: "back-to-p4d-template-settings", ButtonStyle.Primary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_P4D_Auto_Advance_Confirm(MenuIdStructure menuSession)
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

            embed.WithColor(EmbedSettings.Get_Game_Color("P4D", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P4D"));

            embed.WithDescription("" +
                $"The auto advance cursor has been set to **`{account.P4D_TS_Auto_Advance}`**.\n");

            menuSession.CurrentMenu = "Template_Layout_P4D_Auto_Advance_Confirm";

            var component = new ComponentBuilder()
                .WithButton("💠 P4D Template Settings", customId: "back-to-p4d-template-settings", ButtonStyle.Primary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_P4D_Sprite_Placement_Confirm(MenuIdStructure menuSession)
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

            embed.WithColor(EmbedSettings.Get_Game_Color("P4D", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P4D"));

            embed.WithDescription("" +
                $"Sprite placements have been set to **`{account.P4D_TS_Position}`**.\n");

            menuSession.CurrentMenu = "Template_Layout_P4D_Sprite_Placement_Confirm";

            var component = new ComponentBuilder()
                .WithButton("💠 P4D Template Settings", customId: "back-to-p4d-template-settings", ButtonStyle.Primary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }
    }
}
