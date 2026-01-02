using System.Threading.Tasks;
using Discord;

namespace SocialLinker.Core.Menus.Settings.Main.SceneMaker.TemplateLayout
{
    class Template_Layout_P3P_Menu
    {
        public static async Task Template_Layout_P3P_Main(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Template Settings - Persona 3 Portable",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            embed.WithColor(EmbedSettings.Get_Game_Color("P3P", account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P3P"));

            embed.WithDescription("" +
                "Select a setting to edit.\n" +
                "\n" +
                ":one: Color Scheme\n" +
                ":two: Date & Moon Phases\n" +
                ":three: Sprite Placement\n");

            menuSession.CurrentMenu = "Template_Layout_P3P_Main";

            var selectMenu = new SelectMenuBuilder()
                    .WithPlaceholder("Select an option")
                    .WithCustomId(menuSession.CurrentMenu)
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .AddOption("Color Scheme", "1", null, new Emoji("1️⃣"))
                    .AddOption("Date & Moon Phases", "2", null, new Emoji("2️⃣"))
                    .AddOption("Sprite Placement", "3", null, new Emoji("3️⃣"))
                    .AddOption("Return to Version Select", "return", null, new Emoji("↩️"));

            var component = new ComponentBuilder()
                .WithSelectMenu(selectMenu);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_P3P_Color_Scheme(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Color Scheme",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            embed.WithColor(EmbedSettings.Get_Game_Color("P3P", account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P3P"));

            embed.WithDescription("" +
                "Change the color scheme of the template.\n" +
                "\n" +
                $"⚙️ **Current setting:** **`{account.P3P_TS_Color}`**\n" +
                "\n" +
                ":one: Male Protagonist\n" +
                ":two: Female Protagonist\n");

            embed.WithImageUrl("https://i.imgur.com/M6H08x4.png");

            menuSession.CurrentMenu = "Template_Layout_P3P_Color_Scheme";

            var selectMenu = new SelectMenuBuilder()
                    .WithPlaceholder("Select an option")
                    .WithCustomId(menuSession.CurrentMenu)
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .AddOption("Male Protagonist", "1", null, new Emoji("1️⃣"))
                    .AddOption("Female Protagonist", "2", null, new Emoji("2️⃣"))
                    .AddOption("Return to P3P Template Settings", "return", null, new Emoji("↩️"));

            var component = new ComponentBuilder()
                .WithSelectMenu(selectMenu);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_P3P_Date_Moon(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Date & Moon Phases",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            embed.WithColor(EmbedSettings.Get_Game_Color("P3P", account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P3P"));

            embed.WithDescription("" +
                "Toggle parts of the date & moon HUD on and off.\n" +
                "\n" +
                $"⚙️ **Current setting:** **`{account.P3P_TS_HUD}`**\n" +
                "\n" +
                ":one: Display All\n" +
                ":two: Countdown Off\n" +
                ":three: None\n");

            embed.WithImageUrl("https://i.imgur.com/JYVDhCO.png");

            menuSession.CurrentMenu = "Template_Layout_P3P_Date_Moon";

            var selectMenu = new SelectMenuBuilder()
                    .WithPlaceholder("Select an option")
                    .WithCustomId(menuSession.CurrentMenu)
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .AddOption("Display All", "1", null, new Emoji("1️⃣"))
                    .AddOption("Countdown Off", "2", null, new Emoji("2️⃣"))
                    .AddOption("None", "3", null, new Emoji("3️⃣"))
                    .AddOption("Return to P3P Template Settings", "return", null, new Emoji("↩️"));

            var component = new ComponentBuilder()
                .WithSelectMenu(selectMenu);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_P3P_Sprite_Placement(MenuIdStructure menuSession)
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

            embed.WithColor(EmbedSettings.Get_Game_Color("P3P", account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P3P"));

            embed.WithDescription("" +
                "Choose the default position character sprites are rendered at.\n" +
                "\n" +
                $"⚙️ **Current setting:** **`{account.P3P_TS_Position}`**\n" +
                "\n" +
                ":one: Left\n" +
                ":two: Center\n" +
                ":three: Right\n");

            embed.WithImageUrl("https://i.imgur.com/5BoO0yM.png");

            menuSession.CurrentMenu = "Template_Layout_P3P_Sprite_Placement";

            var selectMenu = new SelectMenuBuilder()
                    .WithPlaceholder("Select an option")
                    .WithCustomId(menuSession.CurrentMenu)
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .AddOption("Left", "1", null, new Emoji("1️⃣"))
                    .AddOption("Center", "2", null, new Emoji("2️⃣"))
                    .AddOption("Right", "3", null, new Emoji("3️⃣"))
                    .AddOption("Return to P3P Template Settings", "return", null, new Emoji("↩️"));

            var component = new ComponentBuilder()
                .WithSelectMenu(selectMenu);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_P3P_Color_Scheme_Confirm(MenuIdStructure menuSession)
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

            embed.WithColor(EmbedSettings.Get_Game_Color("P3P", account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P3P"));

            embed.WithDescription("" +
                $"The color scheme has been set to **`{account.P3P_TS_Color}`**.\n");

            menuSession.CurrentMenu = "Template_Layout_P3P_Color_Scheme_Confirm";

            var component = new ComponentBuilder()
                .WithButton("💠 P3P Template Settings", customId: "back-to-p3p-template-settings", ButtonStyle.Primary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_P3P_Date_Moon_Confirm(MenuIdStructure menuSession)
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

            embed.WithColor(EmbedSettings.Get_Game_Color("P3P", account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P3P"));

            embed.WithDescription("" +
                $"Date & moon phases have been set to **`{account.P3P_TS_HUD}`**.\n");

            menuSession.CurrentMenu = "Template_Layout_P3P_Date_Moon_Confirm";

            var component = new ComponentBuilder()
                .WithButton("💠 P3P Template Settings", customId: "back-to-p3p-template-settings", ButtonStyle.Primary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_P3P_Sprite_Placement_Confirm(MenuIdStructure menuSession)
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

            embed.WithColor(EmbedSettings.Get_Game_Color("P3P", account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P3P"));

            embed.WithDescription("" +
                $"Sprite placements have been set to **`{account.P3P_TS_Position}`**.\n");

            menuSession.CurrentMenu = "Template_Layout_P3P_Sprite_Placement_Confirm";

            var component = new ComponentBuilder()
                .WithButton("💠 P3P Template Settings", customId: "back-to-p3p-template-settings", ButtonStyle.Primary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }
    }
}
