using System.Threading.Tasks;
using Discord;

namespace SocialLinker.Core.Menus.Settings.Main.SceneMaker.TemplateLayout
{
    class Template_Layout_P5_PS3_Menu
    {
        public static async Task Template_Layout_P5_PS3_Main(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Template Settings - Persona 5 (PlayStation®️ 4)",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            embed.WithColor(EmbedSettings.Get_Game_Color("P5-PS4", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P5-PS4"));

            embed.WithDescription("" +
                "**Select a setting to edit.**\n" +
                "\n" +
                ":one: Date & Weather\n" +
                ":two: Scene Border\n" +
                ":three: Cursor & Control Panel\n");

            menuSession.CurrentMenu = "Template_Layout_P5_PS3_Main";

            var selectMenu = new SelectMenuBuilder()
                    .WithPlaceholder("Select an option")
                    .WithCustomId(menuSession.CurrentMenu)
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .AddOption("Date & Weather", "1", null, new Emoji("1️⃣"))
                    .AddOption("Scene Border", "2", null, new Emoji("2️⃣"))
                    .AddOption("Cursor & Control Panel", "3", null, new Emoji("3️⃣"))
                    .AddOption("Return to Version Select", "return", null, new Emoji("↩️"));

            var component = new ComponentBuilder()
                .WithSelectMenu(selectMenu);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_P5_PS3_Date_Weather(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Date & Weather",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            var footer = new EmbedFooterBuilder
            {
                Text = "↩️ Return to P5 Template Settings"
            };

            embed.WithFooter(footer);

            embed.WithColor(EmbedSettings.Get_Game_Color("P5-PS4", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P5-PS4"));

            embed.WithDescription("" +
                "Toggle the date & weather HUD on and off.\n" +
                "\n" +
                $"⚙️ **Current setting:** **`{account.P5_PS4_TS_HUD}`**\n");

            embed.WithImageUrl("https://i.imgur.com/Bqd0Cxv.png");

            menuSession.CurrentMenu = "Template_Layout_P5_PS3_Date_Weather";

            var component = new ComponentBuilder()
                .WithButton("↩️", customId: "return", ButtonStyle.Secondary)
                .WithButton("✅ On", customId: "on", ButtonStyle.Secondary)
                .WithButton("❌ Off", customId: "off", ButtonStyle.Secondary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_P5_PS3_Scene_Border(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Scene Border",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            embed.WithColor(EmbedSettings.Get_Game_Color("P5-PS4", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P5-PS4"));

            embed.WithDescription("" +
                "Toggle between scene borders used in different contexts.\n" +
                "\n" +
                $"⚙️ **Current setting:** **`{account.P5_PS4_TS_Border}`**\n" +
                "\n" +
                ":one: Event\n" +
                ":two: Interaction\n" +
                ":three: None");

            embed.WithImageUrl("https://i.imgur.com/9zAoeTL.png");

            menuSession.CurrentMenu = "Template_Layout_P5_PS3_Scene_Border";

            var selectMenu = new SelectMenuBuilder()
                    .WithPlaceholder("Select an option")
                    .WithCustomId(menuSession.CurrentMenu)
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .AddOption("Event", "1", null, new Emoji("1️⃣"))
                    .AddOption("Interaction", "2", null, new Emoji("2️⃣"))
                    .AddOption("None", "3", null, new Emoji("3️⃣"))
                    .AddOption("Return to P5 Template Settings", "return", null, new Emoji("↩️"));

            var component = new ComponentBuilder()
                .WithSelectMenu(selectMenu);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_P5_PS3_Cursor_Panel(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Cursor & Control Panel",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            embed.WithColor(EmbedSettings.Get_Game_Color("P5-PS4", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P5-PS4"));

            embed.WithDescription("" +
                "Change how the message window's cursor and control panel are displayed.\n" +
                "\n" +
                $"⚙️ **Current setting:** **`{account.P5_PS4_TS_Panel}`**\n" +
                "\n" +
                ":one: Manual (with Control Panel)\n" +
                ":two: Manual (without Control Panel)\n" +
                ":three: Auto-Advance");

            embed.WithImageUrl("https://i.imgur.com/tPUsAWH.png");

            menuSession.CurrentMenu = "Template_Layout_P5_PS3_Cursor_Panel";

            var selectMenu = new SelectMenuBuilder()
                    .WithPlaceholder("Select an option")
                    .WithCustomId(menuSession.CurrentMenu)
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .AddOption("Manual (with Control Panel)", "1", null, new Emoji("1️⃣"))
                    .AddOption("Manual (without Control Panel)", "2", null, new Emoji("2️⃣"))
                    .AddOption("Auto-Advance", "3", null, new Emoji("3️⃣"))
                    .AddOption("Return to P5 Template Settings", "return", null, new Emoji("↩️"));

            var component = new ComponentBuilder()
                .WithSelectMenu(selectMenu);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_P5_PS3_Date_Weather_Confirm(MenuIdStructure menuSession)
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

            embed.WithColor(EmbedSettings.Get_Game_Color("P5-PS4", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P5-PS4"));

            embed.WithDescription("" +
                $"The date & weather HUD has been set to **`{account.P5_PS4_TS_HUD}`**.\n");

            menuSession.CurrentMenu = "Template_Layout_P5_PS3_Date_Weather_Confirm";

            var component = new ComponentBuilder()
                .WithButton("💠 P5 Template Settings", customId: "back-to-p5-template-settings", ButtonStyle.Primary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_P5_PS3_Scene_Border_Confirm(MenuIdStructure menuSession)
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

            embed.WithColor(EmbedSettings.Get_Game_Color("P5-PS4", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P5-PS4"));

            embed.WithDescription("" +
                $"The scene border has been set to **`{account.P5_PS4_TS_Border}`**.\n");

            menuSession.CurrentMenu = "Template_Layout_P5_PS3_Scene_Border_Confirm";

            var component = new ComponentBuilder()
                .WithButton("💠 P5 Template Settings", customId: "back-to-p5-template-settings", ButtonStyle.Primary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_P5_PS3_Cursor_Panel_Confirm(MenuIdStructure menuSession)
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

            embed.WithColor(EmbedSettings.Get_Game_Color("P5-PS4", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P5-PS4"));

            embed.WithDescription("" +
                $"The cursor & control panel have been set to **`{account.P5_PS4_TS_Panel}`**.\n");

            menuSession.CurrentMenu = "Template_Layout_P5_PS3_Cursor_Panel_Confirm";

            var component = new ComponentBuilder()
                .WithButton("💠 P5 Template Settings", customId: "back-to-p5-template-settings", ButtonStyle.Primary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }
    }
}
