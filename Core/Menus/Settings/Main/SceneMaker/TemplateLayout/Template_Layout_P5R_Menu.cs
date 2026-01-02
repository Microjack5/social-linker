using System.Threading.Tasks;
using Discord;

namespace SocialLinker.Core.Menus.Settings.Main.SceneMaker.TemplateLayout
{
    class Template_Layout_P5R_Menu
    {
        public static async Task Template_Layout_P5R_Main(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Template Settings - Persona 5 Royal",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            embed.WithColor(EmbedSettings.Get_Game_Color("P5R", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P5R"));

            embed.WithDescription("" +
                "Select a setting to edit.\n" +
                "\n" +
                ":one: Date & Weather\n" +
                ":two: Scene Border\n" +
                ":three: Cursor & Control Panel\n" +
                ":four: Phone Calls");

            menuSession.CurrentMenu = "Template_Layout_P5R_Main";

            var selectMenu = new SelectMenuBuilder()
                    .WithPlaceholder("Select an option")
                    .WithCustomId(menuSession.CurrentMenu)
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .AddOption("Date & Weather", "1", null, new Emoji("1️⃣"))
                    .AddOption("Scene Border", "2", null, new Emoji("2️⃣"))
                    .AddOption("Cursor & Control Panel", "3", null, new Emoji("3️⃣"))
                    .AddOption("Phone Calls", "4", null, new Emoji("4️⃣"))
                    .AddOption("Return to Version Select", "return", null, new Emoji("↩️"));

            var component = new ComponentBuilder()
                .WithSelectMenu(selectMenu);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_P5R_Date_Weather(MenuIdStructure menuSession)
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

            embed.WithColor(EmbedSettings.Get_Game_Color("P5R", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P5R"));

            embed.WithDescription("" +
                "Toggle the date & weather HUD on and off.\n" +
                "\n" +
                $"⚙️ **Current setting:** **`{account.P5R_TS_HUD}`**\n" +
                "\n" +
                ":one: Normal\n" +
                ":two: Inverted\n" +
                ":three: None\n");

            embed.WithImageUrl("https://i.imgur.com/SC29WmH.png");

            menuSession.CurrentMenu = "Template_Layout_P5R_Date_Weather";

            var selectMenu = new SelectMenuBuilder()
                    .WithPlaceholder("Select an option")
                    .WithCustomId(menuSession.CurrentMenu)
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .AddOption("Normal", "1", null, new Emoji("1️⃣"))
                    .AddOption("Inverted", "2", null, new Emoji("2️⃣"))
                    .AddOption("None", "3", null, new Emoji("3️⃣"))
                    .AddOption("Return to P5R Template Settings", "return", null, new Emoji("↩️"));

            var component = new ComponentBuilder()
                .WithSelectMenu(selectMenu);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_P5R_Scene_Border(MenuIdStructure menuSession)
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

            embed.WithColor(EmbedSettings.Get_Game_Color("P5R", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P5R"));

            embed.WithDescription("" +
                "Toggle between scene borders used in different contexts.\n" +
                "\n" +
                $"⚙️ **Current setting:** **`{account.P5R_TS_Border}`**\n" +
                "\n" +
                ":one: Event\n" +
                ":two: Interaction\n" +
                ":three: None");

            embed.WithImageUrl("https://i.imgur.com/ppcK1C8.png");

            menuSession.CurrentMenu = "Template_Layout_P5R_Scene_Border";

            var selectMenu = new SelectMenuBuilder()
                    .WithPlaceholder("Select an option")
                    .WithCustomId(menuSession.CurrentMenu)
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .AddOption("Event", "1", null, new Emoji("1️⃣"))
                    .AddOption("Interaction", "2", null, new Emoji("2️⃣"))
                    .AddOption("None", "3", null, new Emoji("3️⃣"))
                    .AddOption("Return to P5R Template Settings", "return", null, new Emoji("↩️"));

            var component = new ComponentBuilder()
                .WithSelectMenu(selectMenu);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_P5R_Cursor_Panel(MenuIdStructure menuSession)
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

            embed.WithColor(EmbedSettings.Get_Game_Color("P5R", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P5R"));

            embed.WithDescription("" +
                "Change how the message window's cursor and control panel are displayed.\n" +
                "\n" +
                $"⚙️ **Current setting:** **`{account.P5R_TS_Panel}`**\n" +
                "\n" +
                ":one: Manual (with Control Panel)\n" +
                ":two: Manual (without Control Panel)\n" +
                ":three: Auto-Advance");

            embed.WithImageUrl("https://i.imgur.com/GgGyvo7.png");

            menuSession.CurrentMenu = "Template_Layout_P5R_Cursor_Panel";

            var selectMenu = new SelectMenuBuilder()
                    .WithPlaceholder("Select an option")
                    .WithCustomId(menuSession.CurrentMenu)
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .AddOption("Manual (with Control Panel)", "1", null, new Emoji("1️⃣"))
                    .AddOption("Manual (without Control Panel)", "2", null, new Emoji("2️⃣"))
                    .AddOption("Auto-Advance", "3", null, new Emoji("3️⃣"))
                    .AddOption("Return to P5R Template Settings", "return", null, new Emoji("↩️"));

            var component = new ComponentBuilder()
                .WithSelectMenu(selectMenu);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_P5R_Phone_Calls_Main(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Phone Calls",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            embed.WithColor(EmbedSettings.Get_Game_Color("P5R", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P5R"));

            embed.WithDescription("" +
                "Select a setting to edit.\n" +
                "\n" +
                ":one: Caller Toggle\n" +
                ":two: Caller Location\n");

            menuSession.CurrentMenu = "Template_Layout_P5R_Phone_Calls_Main";

            var selectMenu = new SelectMenuBuilder()
                    .WithPlaceholder("Select an option")
                    .WithCustomId(menuSession.CurrentMenu)
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .AddOption("Caller Toggle", "1", null, new Emoji("1️⃣"))
                    .AddOption("Caller Location", "2", null, new Emoji("2️⃣"))
                    .AddOption("Return to P5R Template Settings", "return", null, new Emoji("↩️"));

            var component = new ComponentBuilder()
                .WithSelectMenu(selectMenu);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_P5R_Phone_Calls_Toggle(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Caller Toggle",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            var footer = new EmbedFooterBuilder
            {
                Text = "↩️ Return to Phone Calls Menu"
            };

            embed.WithFooter(footer);

            // Assign a color and thumbnail to the embeded message based on the title being edited.
            embed.WithColor(EmbedSettings.Get_Game_Color("P5R", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P5R"));

            embed.WithDescription("" +
                "Toggle phone call settings for character sprites on and off.\n" +
                "\n" +
                $"⚙️ **Current setting:** **`{account.P5R_TS_Caller_Toggle}`**\n");

            embed.WithImageUrl("https://i.imgur.com/thCJrMc.png");

            menuSession.CurrentMenu = "Template_Layout_P5R_Phone_Calls_Toggle";

            var component = new ComponentBuilder()
                .WithButton("↩️", customId: "return", ButtonStyle.Secondary)
                .WithButton("✅ On", customId: "on", ButtonStyle.Secondary)
                .WithButton("❌ Off", customId: "off", ButtonStyle.Secondary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_P5R_Phone_Calls_Location(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Caller Location",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            var footer = new EmbedFooterBuilder
            {
                Text = "↩️ Return to Phone Calls Menu"
            };

            embed.WithFooter(footer);

            // Assign a color and thumbnail to the embeded message based on the title being edited.
            embed.WithColor(EmbedSettings.Get_Game_Color("P5R", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P5R"));

            embed.WithDescription("" +
                "Toggle between normal and Velvet Room calling locations.\n" +
                "\n" +
                $"⚙️ **Current setting:** **`{account.P5R_TS_Caller_Location}`**\n" +
                "\n" +
                ":one: Dynamic - *Dynamically change the background depending on time of day and characters.*\n" +
                ":two: Dynamic (Normals Only) - *Dynamically change the background depending on time of day. Velvet Room background excluded.*\n" +
                ":three: Velvet Room - *Statically set the background to Velvet Room for all characters.*");

            menuSession.CurrentMenu = "Template_Layout_P5R_Phone_Calls_Location";

            var selectMenu = new SelectMenuBuilder()
                    .WithPlaceholder("Select an option")
                    .WithCustomId(menuSession.CurrentMenu)
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .AddOption("Dynamic", "1", null, new Emoji("1️⃣"))
                    .AddOption("Dynamic (Normals Only)", "2", null, new Emoji("2️⃣"))
                    .AddOption("Velvet Room", "3", null, new Emoji("3️⃣"))
                    .AddOption("Return to Phone Calls Menu", "return", null, new Emoji("↩️"));

            var component = new ComponentBuilder()
                .WithSelectMenu(selectMenu);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_P5R_Date_Weather_Confirm(MenuIdStructure menuSession)
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

            embed.WithColor(EmbedSettings.Get_Game_Color("P5R", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P5R"));

            embed.WithDescription("" +
                $"The date & weather HUD has been set to **`{account.P5R_TS_HUD}`**.\n");

            menuSession.CurrentMenu = "Template_Layout_P5R_Date_Weather_Confirm";

            var component = new ComponentBuilder()
                .WithButton("💠 P5R Template Settings", customId: "back-to-p5r-template-settings", ButtonStyle.Primary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_P5R_Scene_Border_Confirm(MenuIdStructure menuSession)
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

            embed.WithColor(EmbedSettings.Get_Game_Color("P5R", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P5R"));

            embed.WithDescription("" +
                $"The scene border has been set to **`{account.P5R_TS_Border}`**.\n");

            menuSession.CurrentMenu = "Template_Layout_P5R_Scene_Border_Confirm";

            var component = new ComponentBuilder()
                .WithButton("💠 P5R Template Settings", customId: "back-to-p5r-template-settings", ButtonStyle.Primary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_P5R_Cursor_Panel_Confirm(MenuIdStructure menuSession)
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

            embed.WithColor(EmbedSettings.Get_Game_Color("P5R", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P5R"));

            embed.WithDescription("" +
                $"The cursor & control panel have been set to **`{account.P5R_TS_Panel}`**.\n");

            menuSession.CurrentMenu = "Template_Layout_P5R_Cursor_Panel_Confirm";

            var component = new ComponentBuilder()
                .WithButton("💠 P5R Template Settings", customId: "back-to-p5r-template-settings", ButtonStyle.Primary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_P5R_Phone_Calls_Toggle_Confirm(MenuIdStructure menuSession)
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

            embed.WithColor(EmbedSettings.Get_Game_Color("P5R", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P5R"));

            embed.WithDescription("" +
                $"The phone call setting has been set to **`{account.P5R_TS_Caller_Toggle}`**.\n");

            menuSession.CurrentMenu = "Template_Layout_P5R_Phone_Calls_Toggle_Confirm";

            var component = new ComponentBuilder()
                .WithButton("💠 P5R Template Settings", customId: "back-to-p5r-template-settings", ButtonStyle.Primary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_P5R_Phone_Calls_Location_Confirm(MenuIdStructure menuSession)
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

            embed.WithColor(EmbedSettings.Get_Game_Color("P5R", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P5R"));

            embed.WithDescription("" +
                $"The caller location has been set to **`{account.P5R_TS_Caller_Location}`**.\n");

            menuSession.CurrentMenu = "Template_Layout_P5R_Phone_Calls_Location_Confirm";

            var component = new ComponentBuilder()
                .WithButton("💠 P5R Template Settings", customId: "back-to-p5r-template-settings", ButtonStyle.Primary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }
    }
}
