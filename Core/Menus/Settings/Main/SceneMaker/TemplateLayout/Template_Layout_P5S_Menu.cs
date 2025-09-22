using System.Threading.Tasks;
using Discord;

namespace SocialLinker.Core.Menus.Settings.Main.SceneMaker.TemplateLayout
{
    class Template_Layout_P5S_Menu
    {
        public static async Task Template_Layout_P5S_Main(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Template Settings - Persona 5 Strikers",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            embed.WithColor(EmbedSettings.Get_Game_Color("P5S", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P5S"));

            embed.WithDescription("" +
                "**Select a setting to edit.**\n" +
                "\n" +
                ":one: Controller Type\n" +
                ":two: Skip Button\n" +
                ":three: Auto-Advance\n" +
                ":four: Scene Border\n" +
                ":five: Date & Location Layout\n" +
                ":six: Location Icon\n" +
                ":seven: Screenshot Watermark\n");

            menuSession.CurrentMenu = "Template_Layout_P5S_Main";

            var selectMenu = new SelectMenuBuilder()
                    .WithPlaceholder("Select an option")
                    .WithCustomId(menuSession.CurrentMenu)
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .AddOption("Controller", "1", null, new Emoji("1️⃣"))
                    .AddOption("Skip Button", "2", null, new Emoji("2️⃣"))
                    .AddOption("Auto-Advance", "3", null, new Emoji("3️⃣"))
                    .AddOption("Scene Border", "4", null, new Emoji("4️⃣"))
                    .AddOption("Date & Location Layout", "5", null, new Emoji("5️⃣"))
                    .AddOption("Location Icon", "6", null, new Emoji("6️⃣"))
                    .AddOption("Screenshot Watermark", "7", null, new Emoji("7️⃣"))
                    .AddOption("Return to Template Layout Menu", "return", null, new Emoji("↩️"));

            var component = new ComponentBuilder()
                .WithSelectMenu(selectMenu);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_P5S_Controller_Type(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Controller Type",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            embed.WithColor(EmbedSettings.Get_Game_Color("P5S", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P5S"));

            embed.WithDescription("" +
                "Change button icons between PlayStation® 4, Nintendo Switch, Xbox One, and keyboard displays.\n" +
                "\n" +
                $"⚙️ **Current setting:** **`{account.P5S_TS_Controller_Type}`**\n" +
                "\n" +
                ":one: PlayStation® 4\n" +
                ":two: Nintendo Switch\n" +
                ":three: Xbox One\n" +
                ":four: Keyboard\n");

            embed.WithImageUrl("https://i.imgur.com/NvXYNq0.png");

            menuSession.CurrentMenu = "Template_Layout_P5S_Controller_Type";

            var selectMenu = new SelectMenuBuilder()
                    .WithPlaceholder("Select an option")
                    .WithCustomId(menuSession.CurrentMenu)
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .AddOption("PlayStation® 4", "1", null, new Emoji("1️⃣"))
                    .AddOption("Nintendo Switch", "2", null, new Emoji("2️⃣"))
                    .AddOption("Xbox One", "3", null, new Emoji("3️⃣"))
                    .AddOption("Keyboard", "4", null, new Emoji("4️⃣"))
                    .AddOption("Return to P5S Template Settings", "return", null, new Emoji("↩️"));

            var component = new ComponentBuilder()
                .WithSelectMenu(selectMenu);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_P5S_Skip_Button(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Skip Button",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            var footer = new EmbedFooterBuilder
            {
                Text = "↩️ Return to P5S Template Settings"
            };

            embed.WithFooter(footer);

            embed.WithColor(EmbedSettings.Get_Game_Color("P5S", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P5S"));

            embed.WithDescription("" +
                "Toggle the skip button of the control panel on and off.\n" +
                "\n" +
                $"⚙️ **Current setting:** **`{account.P5S_TS_Skip_Button}`**\n");

            embed.WithImageUrl("https://i.imgur.com/8hTZlYS.png");

            menuSession.CurrentMenu = "Template_Layout_P5S_Skip_Button";

            var component = new ComponentBuilder()
                .WithButton("↩️", customId: "return", ButtonStyle.Secondary)
                .WithButton("✅ On", customId: "on", ButtonStyle.Secondary)
                .WithButton("❌ Off", customId: "off", ButtonStyle.Secondary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_P5S_Auto_Advance(MenuIdStructure menuSession)
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
                Text = "↩️ Return to P5S Template Settings"
            };

            embed.WithFooter(footer);

            embed.WithColor(EmbedSettings.Get_Game_Color("P5S", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P5S"));

            embed.WithDescription("" +
                "Toggle the auto advance icon of the control panel on and off.\n" +
                "\n" +
                $"⚙️ **Current setting:** **`{account.P5S_TS_Auto_Advance}`**\n");

            menuSession.CurrentMenu = "Template_Layout_P5S_Auto_Advance";

            var component = new ComponentBuilder()
                .WithButton("↩️", customId: "return", ButtonStyle.Secondary)
                .WithButton("✅ On", customId: "on", ButtonStyle.Secondary)
                .WithButton("❌ Off", customId: "off", ButtonStyle.Secondary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_P5S_Scene_Border(MenuIdStructure menuSession)
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

            var footer = new EmbedFooterBuilder
            {
                Text = "↩️ Return to P5S Template Settings"
            };

            embed.WithFooter(footer);

            embed.WithColor(EmbedSettings.Get_Game_Color("P5S", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P5S"));

            embed.WithDescription("" +
                "Toggle the scene border on and off.\n" +
                "\n" +
                $"⚙️ **Current setting:** **`{account.P5S_TS_Scene_Border}`**\n");

            embed.WithImageUrl("https://i.imgur.com/9GbmoSC.png");

            menuSession.CurrentMenu = "Template_Layout_P5S_Scene_Border";

            var component = new ComponentBuilder()
                .WithButton("↩️", customId: "return", ButtonStyle.Secondary)
                .WithButton("✅ On", customId: "on", ButtonStyle.Secondary)
                .WithButton("❌ Off", customId: "off", ButtonStyle.Secondary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_P5S_Date_Location_Layout(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Date & Location Layout",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            embed.WithColor(EmbedSettings.Get_Game_Color("P5S", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P5S"));

            embed.WithDescription("" +
                "Toggle parts of the date & location HUD on and off.\n" +
                "\n" +
                $"⚙️ **Current setting:** **`{account.P5S_TS_Date_Location_Layout}`**\n" +
                "\n" +
                ":one: Display All\n" +
                ":two: Date Only\n" +
                ":three: None");

            embed.WithImageUrl("https://i.imgur.com/eLBUlGC.png");

            menuSession.CurrentMenu = "Template_Layout_P5S_Date_Location_Layout";

            var selectMenu = new SelectMenuBuilder()
                    .WithPlaceholder("Select an option")
                    .WithCustomId(menuSession.CurrentMenu)
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .AddOption("Display All", "1", null, new Emoji("1️⃣"))
                    .AddOption("Date Only", "2", null, new Emoji("2️⃣"))
                    .AddOption("None", "3", null, new Emoji("3️⃣"))
                    .AddOption("Return to P5S Template Settings", "return", null, new Emoji("↩️"));

            var component = new ComponentBuilder()
                .WithSelectMenu(selectMenu);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_P5S_Location_Icon(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Location Icon",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            embed.WithColor(EmbedSettings.Get_Game_Color("P5S", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P5S"));

            embed.WithDescription("" +
                "Change the displayed location icon.\n" +
                "\n" +
                $"⚙️ **Current setting:** **`{account.P5S_TS_Location_Icon}`**\n" +
                "\n" +
                ":one: Yongen-Jaya\n" +
                ":two: Shibuya\n" +
                ":three: Sendai\n" +
                ":four: Sapporo\n" +
                ":five: Okinawa\n" +
                ":six: Fukuoka\n" +
                ":seven: Kyoto\n" +
                ":eight: Osaka\n" +
                ":nine: Yokohama\n" +
                ":keycap_ten: Shiba Park\n" +
                ":blue_car: RV Travel");

            embed.WithImageUrl("https://i.imgur.com/GRFozcV.png");

            menuSession.CurrentMenu = "Template_Layout_P5S_Location_Icon";

            var selectMenu = new SelectMenuBuilder()
                    .WithPlaceholder("Select an option")
                    .WithCustomId(menuSession.CurrentMenu)
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .AddOption("Yongen-Jaya", "1", null, new Emoji("1️⃣"))
                    .AddOption("Shibuya", "2", null, new Emoji("2️⃣"))
                    .AddOption("Sendai", "3", null, new Emoji("3️⃣"))
                    .AddOption("Sapporo", "4", null, new Emoji("4️⃣"))
                    .AddOption("Okinawa", "5", null, new Emoji("5️⃣"))
                    .AddOption("Fukuoka", "6", null, new Emoji("6️⃣"))
                    .AddOption("Kyoto", "7", null, new Emoji("7️⃣"))
                    .AddOption("Osaka", "8", null, new Emoji("8️⃣"))
                    .AddOption("Yokohama", "9", null, new Emoji("9️⃣"))
                    .AddOption("Shiba Park", "10", null, new Emoji("🔟"))
                    .AddOption("RV Travel", "car", null, new Emoji("🚙"))
                    .AddOption("Return to P5S Template Settings", "return", null, new Emoji("↩️"));

            var component = new ComponentBuilder()
                .WithSelectMenu(selectMenu);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_P5S_Watermark(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Screenshot Watermark",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            var footer = new EmbedFooterBuilder
            {
                Text = "↩️ Return to P5S Template Settings"
            };

            embed.WithFooter(footer);

            embed.WithColor(EmbedSettings.Get_Game_Color("P5S", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P5S"));

            embed.WithDescription("" +
                "Toggle the screenshot watermark on and off.\n" +
                "\n" +
                $"⚙️ **Current setting:** **`{account.P5S_TS_Watermark}`**\n");

            embed.WithImageUrl("https://i.imgur.com/6rjnLTE.png");

            menuSession.CurrentMenu = "Template_Layout_P5S_Watermark";

            var component = new ComponentBuilder()
                .WithButton("↩️", customId: "return", ButtonStyle.Secondary)
                .WithButton("✅ On", customId: "on", ButtonStyle.Secondary)
                .WithButton("❌ Off", customId: "off", ButtonStyle.Secondary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_P5S_Controller_Type_Confirm(MenuIdStructure menuSession)
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

            embed.WithColor(EmbedSettings.Get_Game_Color("P5S", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P5S"));

            embed.WithDescription("" +
                $"The controller type has been set to **`{account.P5S_TS_Controller_Type}`**.\n");

            menuSession.CurrentMenu = "Template_Layout_P5S_Controller_Type_Confirm";

            var component = new ComponentBuilder()
                .WithButton("💠 P5S Template Settings", customId: "back-to-p5s-template-settings", ButtonStyle.Primary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_P5S_Skip_Button_Confirm(MenuIdStructure menuSession)
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

            embed.WithColor(EmbedSettings.Get_Game_Color("P5S", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P5S"));

            embed.WithDescription("" +
                $"The skip button has been set to **`{account.P5S_TS_Skip_Button}`**.\n");

            menuSession.CurrentMenu = "Template_Layout_P5S_Skip_Button_Confirm";

            var component = new ComponentBuilder()
                .WithButton("💠 P5S Template Settings", customId: "back-to-p5s-template-settings", ButtonStyle.Primary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_P5S_Auto_Advance_Confirm(MenuIdStructure menuSession)
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

            embed.WithColor(EmbedSettings.Get_Game_Color("P5S", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P5S"));

            embed.WithDescription("" +
                $"The auto advance icon has been set to **`{account.P5S_TS_Auto_Advance}`**.\n");

            menuSession.CurrentMenu = "Template_Layout_P5S_Auto_Advance_Confirm";

            var component = new ComponentBuilder()
                .WithButton("💠 P5S Template Settings", customId: "back-to-p5s-template-settings", ButtonStyle.Primary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_P5S_Scene_Border_Confirm(MenuIdStructure menuSession)
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

            embed.WithColor(EmbedSettings.Get_Game_Color("P5S", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P5S"));

            embed.WithDescription("" +
                $"The scene border has been set to **`{account.P5S_TS_Scene_Border}`**.\n");

            menuSession.CurrentMenu = "Template_Layout_P5S_Scene_Border_Confirm";

            var component = new ComponentBuilder()
                .WithButton("💠 P5S Template Settings", customId: "back-to-p5s-template-settings", ButtonStyle.Primary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_P5S_Date_Location_Layout_Confirm(MenuIdStructure menuSession)
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

            embed.WithColor(EmbedSettings.Get_Game_Color("P5S", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P5S"));

            embed.WithDescription("" +
                $"The date & location layout has been set to **`{account.P5S_TS_Date_Location_Layout}`**.\n");

            menuSession.CurrentMenu = "Template_Layout_P5S_Date_Location_Layout_Confirm";

            var component = new ComponentBuilder()
                .WithButton("💠 P5S Template Settings", customId: "back-to-p5s-template-settings", ButtonStyle.Primary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_P5S_Location_Icon_Confirm(MenuIdStructure menuSession)
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

            embed.WithColor(EmbedSettings.Get_Game_Color("P5S", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P5S"));

            embed.WithDescription("" +
                $"The location icon has been set to **`{account.P5S_TS_Location_Icon}`**.\n");

            menuSession.CurrentMenu = "Template_Layout_P5S_Location_Icon_Confirm";

            var component = new ComponentBuilder()
                .WithButton("💠 P5S Template Settings", customId: "back-to-p5s-template-settings", ButtonStyle.Primary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_P5S_Watermark_Confirm(MenuIdStructure menuSession)
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

            embed.WithColor(EmbedSettings.Get_Game_Color("P5S", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P5S"));

            embed.WithDescription("" +
                $"The screenshot watermark has been set to **`{account.P5S_TS_Watermark}`**.\n");

            menuSession.CurrentMenu = "Template_Layout_P5S_Watermark_Confirm";

            var component = new ComponentBuilder()
                .WithButton("💠 P5S Template Settings", customId: "back-to-p5s-template-settings", ButtonStyle.Primary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }
    }
}
