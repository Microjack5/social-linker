using System.Threading.Tasks;
using Discord;

namespace SocialLinker.Core.Menus.Settings.Main.SceneMaker.TemplateLayout
{
    class Template_Layout_P1_PSP_Menu
    {
        public static async Task Template_Layout_P1_PSP_Main(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Template Settings - Persona (PSP®️)",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            embed.WithColor(EmbedSettings.Get_Game_Color("P1-PSP", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P1-PSP"));

            embed.WithDescription("" +
                "**Select a setting to edit.**\n" +
                "\n" +
                ":one: Moon Phases\n" +
                ":two: Sprite Placement\n" +
                ":three: Darken Background");

            var selectMenu = new SelectMenuBuilder()
                    .WithPlaceholder("Select an option")
                    .WithCustomId("template-layout-p1-psp-main")
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .AddOption("Moon Phases", "1", null, new Emoji("1️⃣"))
                    .AddOption("Sprite Placement", "2", null, new Emoji("2️⃣"))
                    .AddOption("Darken Background", "3", null, new Emoji("3️⃣"))
                    .AddOption("Return to Version Select", "return", null, new Emoji("↩️"));

            var component = new ComponentBuilder()
                .WithSelectMenu(selectMenu);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);

            menuSession.CurrentMenu = "Template_Layout_P1_PSP_Main";
        }

        public static async Task Template_Layout_P1_PSP_Moon_Phases(MenuIdStructure menuSession)
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

            embed.WithColor(EmbedSettings.Get_Game_Color("P1-PSP", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P1-PSP"));

            embed.WithDescription("" +
                "**Toggle the moon phases on and off in created scenes.**\n" +
                "\n" +
                $"⚙️ **Current setting:** **`{account.P1_PSP_TS_Moon_HUD}`**\n");

            embed.WithImageUrl("https://i.imgur.com/pLWOAWZ.png");

            var component = new ComponentBuilder()
                .WithButton("↩️", customId: "return", ButtonStyle.Secondary)
                .WithButton("✅ On", customId: "on", ButtonStyle.Secondary)
                .WithButton("❌ Off", customId: "off", ButtonStyle.Secondary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);

            menuSession.CurrentMenu = "Template_Layout_P1_PSP_Moon_Phases";
        }

        public static async Task Template_Layout_P1_PSP_Placement(MenuIdStructure menuSession)
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

            embed.WithColor(EmbedSettings.Get_Game_Color("P1-PSP", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P1-PSP"));

            embed.WithDescription("" +
                "**Choose the default position character sprites are rendered at.**\n" +
                "\n" +
                $"⚙️ **Current setting:** **`{account.P1_PSP_TS_Position}`**\n" +
                "\n" +
                ":one: Left\n" +
                ":two: Center\n" +
                ":three: Right\n" +
                ":four: Switch - *Contextually switch between left, right, and center placements for up to three used characters in a conversation.*\n");

            embed.WithImageUrl("https://i.imgur.com/ZcJ6qEe.png");

            var selectMenu = new SelectMenuBuilder()
                    .WithPlaceholder("Select an option")
                    .WithCustomId("template-layout-p1-psp-placement")
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .AddOption("Left", "left", null, new Emoji("1️⃣"))
                    .AddOption("Right", "right", null, new Emoji("2️⃣"))
                    .AddOption("Center", "center", null, new Emoji("3️⃣"))
                    .AddOption("Switch", "switch", null, new Emoji("4️⃣"))
                    .AddOption("Return to Persona (PSP®️) Template Settings", "return", null, new Emoji("↩️"));

            var component = new ComponentBuilder()
                .WithSelectMenu(selectMenu);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);

            menuSession.CurrentMenu = "Template_Layout_P1_PSP_Placement";
        }

        public static async Task Template_Layout_P1_PSP_BG_Darken(MenuIdStructure menuSession)
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

            embed.WithColor(EmbedSettings.Get_Game_Color("P1-PSP", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P1-PSP"));

            embed.WithDescription("" +
                "**Toggle a filter to darken background elements on and off.**\n" +
                "\n" +
                $"⚙️ **Current setting:** **`{account.P1_PSP_TS_BG_Darken}`**\n" +
                "\n" +
                ":one: On\n" +
                ":two: Off\n");

            embed.WithImageUrl("https://i.imgur.com/zxMGylO.png");

            var component = new ComponentBuilder()
                .WithButton("↩️", customId: "return", ButtonStyle.Secondary)
                .WithButton("✅ On", customId: "on", ButtonStyle.Secondary)
                .WithButton("❌ Off", customId: "off", ButtonStyle.Secondary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);

            menuSession.CurrentMenu = "Template_Layout_P1_PSP_BG_Darken";
        }

        public static async Task Template_Layout_P1_PSP_Moon_Phases_Confirm(MenuIdStructure menuSession)
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

            embed.WithColor(EmbedSettings.Get_Game_Color("P1-PSP", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P1-PSP"));

            embed.WithDescription("" +
                $"Moon phases have been set to **`{account.P1_PSP_TS_Moon_HUD}`**.\n");

            var component = new ComponentBuilder()
                .WithButton("💠 Persona (PSP®️) Template Settings", customId: "back-to-p1-psp-template-settings", ButtonStyle.Primary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);

            menuSession.CurrentMenu = "Template_Layout_P1_PSP_Moon_Phases_Confirm";
        }

        public static async Task Template_Layout_P1_PSP_Placement_Confirm(MenuIdStructure menuSession)
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

            embed.WithColor(EmbedSettings.Get_Game_Color("P1-PSP", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P1-PSP"));

            embed.WithDescription("" +
                $"Sprite placements have been set to **`{account.P1_PSP_TS_Position}`**.\n");

            var component = new ComponentBuilder()
                .WithButton("💠 Persona (PSP®️) Template Settings", customId: "back-to-p1-psp-template-settings", ButtonStyle.Primary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);

            menuSession.CurrentMenu = "Template_Layout_P1_PSP_Placement_Confirm";
        }

        public static async Task Template_Layout_P1_PSP_BG_Darken_Confirm(MenuIdStructure menuSession)
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

            embed.WithColor(EmbedSettings.Get_Game_Color("P1-PSP", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P1-PSP"));

            embed.WithDescription("" +
                $"The Darken Background toggle has been set to **`{account.P1_PSP_TS_BG_Darken}`**.\n");

            var component = new ComponentBuilder()
                .WithButton("💠 Persona (PSP®️) Template Settings", customId: "back-to-p1-psp-template-settings", ButtonStyle.Primary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);

            menuSession.CurrentMenu = "Template_Layout_P1_PSP_BG_Darken_Confirm";
        }
    }
}
