using System.Threading.Tasks;
using Discord;

namespace SocialLinker.Core.Menus.Settings.Main.SceneMaker.TemplateLayout
{
    class Template_Layout_P4G_Menu
    {
        public static async Task Template_Layout_P4G_Main(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Template Settings - Persona 4 Golden",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            embed.WithColor(EmbedSettings.Get_Game_Color("P4G", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P4G"));

            embed.WithDescription("" +
                "Select a setting to edit.\n" +
                "\n" +
                ":one: Date & Weather\n");

            menuSession.CurrentMenu = "Template_Layout_P4G_Main";

            var selectMenu = new SelectMenuBuilder()
                    .WithPlaceholder("Select an option")
                    .WithCustomId(menuSession.CurrentMenu)
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .AddOption("Date & Weather", "1", null, new Emoji("1️⃣"))
                    .AddOption("Return to P4 Version Select", "return", null, new Emoji("↩️"));

            var component = new ComponentBuilder()
                .WithSelectMenu(selectMenu);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_P4G_Date_Weather(MenuIdStructure menuSession)
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

            embed.WithColor(EmbedSettings.Get_Game_Color("P4G", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P4G"));

            embed.WithDescription("" +
                "Toggle the date & weather HUD between normal and TV World versions, or hide entirely.\n" +
                "\n" +
                $"⚙️ **Current setting:** **`{account.P4G_TS_HUD}`**\n" +
                "\n" +
                ":one: Normal\n" +
                ":two: TV World\n" +
                ":three: None\n");

            embed.WithImageUrl("https://i.imgur.com/GaxIMvp.png");

            menuSession.CurrentMenu = "Template_Layout_P4G_Date_Weather";

            var selectMenu = new SelectMenuBuilder()
                    .WithPlaceholder("Select an option")
                    .WithCustomId(menuSession.CurrentMenu)
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .AddOption("Normal", "1", null, new Emoji("1️⃣"))
                    .AddOption("TV World", "2", null, new Emoji("2️⃣"))
                    .AddOption("None", "3", null, new Emoji("3️⃣"))
                    .AddOption("Return to P4G Template Settings", "return", null, new Emoji("↩️"));

            var component = new ComponentBuilder()
                .WithSelectMenu(selectMenu);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_P4G_Date_Weather_Confirm(MenuIdStructure menuSession)
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

            embed.WithColor(EmbedSettings.Get_Game_Color("P4G", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P4G"));

            embed.WithDescription("" +
                $"The date & weather HUD has been set to **`{account.P4G_TS_HUD}`**.\n");

            menuSession.CurrentMenu = "Template_Layout_P4G_Date_Weather_Confirm";

            var component = new ComponentBuilder()
                .WithButton("💠 P4G Template Settings", customId: "back-to-p4g-template-settings", ButtonStyle.Primary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }
    }
}
