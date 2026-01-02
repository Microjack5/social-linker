using System.Threading.Tasks;
using Discord;

namespace SocialLinker.Core.Menus.Settings.Main.SceneMaker.TemplateLayout
{
    class Template_Layout_P3F_Menu
    {
        public static async Task Template_Layout_P3F_Main(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Template Settings - Persona 3 FES",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            embed.WithColor(EmbedSettings.Get_Game_Color("P3F", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P3F"));

            embed.WithDescription("" +
                "Select a setting to edit.\n" +
                "\n" +
                ":one: Date & Moon Phases\n" +
                //":two: Navigator Window\n" +
                "");

            menuSession.CurrentMenu = "Template_Layout_P3F_Main";

            var selectMenu = new SelectMenuBuilder()
                    .WithPlaceholder("Select an option")
                    .WithCustomId(menuSession.CurrentMenu)
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .AddOption("Date & Moon Phases", "1", null, new Emoji("1️⃣"))
                    .AddOption("Return to Version Select", "return", null, new Emoji("↩️"));

            var component = new ComponentBuilder()
                .WithSelectMenu(selectMenu);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_P3F_Date_Moon(MenuIdStructure menuSession)
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

            embed.WithColor(EmbedSettings.Get_Game_Color("P3F", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P3F"));

            embed.WithDescription("" +
                "Toggle parts of the date & moon HUD on and off.\n" +
                "\n" +
                $"⚙️ **Current setting:** **`{account.P3F_TS_HUD}`**\n" +
                "\n" +
                ":one: Display All\n" +
                ":two: Countdown Off\n" +
                ":three: Date Only\n" +
                ":four: None");

            embed.WithImageUrl("https://i.imgur.com/eFQ6c8U.png");

            menuSession.CurrentMenu = "Template_Layout_P3F_Date_Moon";

            var selectMenu = new SelectMenuBuilder()
                    .WithPlaceholder("Select an option")
                    .WithCustomId(menuSession.CurrentMenu)
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .AddOption("Display All", "1", null, new Emoji("1️⃣"))
                    .AddOption("Countdown Off", "2", null, new Emoji("2️⃣"))
                    .AddOption("Date Only", "3", null, new Emoji("3️⃣"))
                    .AddOption("Return to Version Select", "return", null, new Emoji("↩️"));

            var component = new ComponentBuilder()
                .WithSelectMenu(selectMenu);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_P3F_Navigatior_Window(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Navigator Window",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            embed.WithColor(EmbedSettings.Get_Game_Color("P3F", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P3F"));

            embed.WithDescription("" +
                "Choose to display character sprites within navigator windows.\n" +
                "(This is automatically applied for some cross-compatible sprites regardless of the setting.)\n" +
                "\n" +
                $"⚙️ **Current setting:** **`{account.P3F_TS_Nav}`**\n" +
                "\n" +
                ":one: On\n" +
                ":two: Off\n");

            menuSession.CurrentMenu = "Template_Layout_P3F_Navigatior_Window";

            var component = new ComponentBuilder()
                .WithButton("↩️", customId: "return", ButtonStyle.Secondary)
                .WithButton("✅ On", customId: "on", ButtonStyle.Secondary)
                .WithButton("❌ Off", customId: "off", ButtonStyle.Secondary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_P3F_Date_Moon_Confirm(MenuIdStructure menuSession)
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

            embed.WithColor(EmbedSettings.Get_Game_Color("P3F", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P3F"));

            embed.WithDescription("" +
                $"Date & moon phases have been set to **`{account.P3F_TS_HUD}`**.\n");

            menuSession.CurrentMenu = "Template_Layout_P3F_Date_Moon_Confirm";

            var component = new ComponentBuilder()
                .WithButton("💠 P3F Template Settings", customId: "back-to-p3f-template-settings", ButtonStyle.Primary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_P3F_Navigatior_Window_Confirm(MenuIdStructure menuSession)
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

            embed.WithColor(EmbedSettings.Get_Game_Color("P3F", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P3F"));

            embed.WithDescription("" +
                $"The navigator window has been set to **`{account.P3F_TS_Nav}`**.\n");

            menuSession.CurrentMenu = "Template_Layout_P3F_Navigatior_Window_Confirm";

            var component = new ComponentBuilder()
                .WithButton("💠 P3F Template Settings", customId: "back-to-p3f-template-settings", ButtonStyle.Primary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }
    }
}
