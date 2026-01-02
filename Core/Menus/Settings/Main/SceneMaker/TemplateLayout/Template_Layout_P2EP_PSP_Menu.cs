using System.Threading.Tasks;
using Discord;

namespace SocialLinker.Core.Menus.Settings.Main.SceneMaker.TemplateLayout
{
    class Template_Layout_P2EP_PSP_Menu
    {
        public static async Task Template_Layout_P2EP_PSP_Main(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Template Settings - Persona 2: Eternal Punishment (PSP®️)",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            embed.WithColor(EmbedSettings.Get_Game_Color("P2EP-PSP", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P2EP-PSP"));

            embed.WithDescription("" +
                "Select a setting to edit.\n" +
                "\n" +
                ":one: Message Window Color\n" +
                ":two: Inverted Filter\n" +
                ":three: Sprite Placement\n" +
                ":four: Sprite Flip");

            menuSession.CurrentMenu = "Template_Layout_P2EP_PSP_Main";

            var selectMenu = new SelectMenuBuilder()
                    .WithPlaceholder("Select an option")
                    .WithCustomId(menuSession.CurrentMenu)
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .AddOption("Message Window Color", "1", null, new Emoji("1️⃣"))
                    .AddOption("Inverted Filter", "2", null, new Emoji("2️⃣"))
                    .AddOption("Sprite Placement", "3", null, new Emoji("3️⃣"))
                    .AddOption("Sprite Flip", "4", null, new Emoji("4️⃣"))
                    .AddOption("Return to Version Select", "return", null, new Emoji("↩️"));

            var component = new ComponentBuilder()
                .WithSelectMenu(selectMenu);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_P2EP_PSP_Window_Color(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Message Window Color",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            embed.WithColor(EmbedSettings.Get_Game_Color("P2EP-PSP", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P2EP-PSP"));

            embed.WithDescription("" +
                "Choose the message window's color.\n" +
                "\n" +
                $"⚙️ **Current setting:** **`{account.P2EP_PSP_TS_Window_Color}`**\n" +
                "\n" +
                ":one: Type 1\n" +
                ":two: Type 2\n" +
                ":three: Type 3\n" +
                ":four: Type 4\n" +
                ":five: Type 5\n" +
                ":six: Type 6\n");

            embed.WithImageUrl("https://i.imgur.com/mjx7gQ6.png");

            menuSession.CurrentMenu = "Template_Layout_P2EP_PSP_Window_Color";

            var selectMenu = new SelectMenuBuilder()
                    .WithPlaceholder("Select an option")
                    .WithCustomId(menuSession.CurrentMenu)
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .AddOption("Type 1", "1", null, new Emoji("1️⃣"))
                    .AddOption("Type 2", "2", null, new Emoji("2️⃣"))
                    .AddOption("Type 3", "3", null, new Emoji("3️⃣"))
                    .AddOption("Type 4", "4", null, new Emoji("4️⃣"))
                    .AddOption("Type 5", "5", null, new Emoji("5️⃣"))
                    .AddOption("Type 6", "6", null, new Emoji("6️⃣"))
                    .AddOption("Return to P2EP Template Settings", "return", null, new Emoji("↩️"));

            var component = new ComponentBuilder()
                .WithSelectMenu(selectMenu);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_P2EP_PSP_Inverted_Filter(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Inverted Filter",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            var footer = new EmbedFooterBuilder
            {
                Text = "↩️ Return to P2EP Template Settings"
            };

            embed.WithFooter(footer);

            embed.WithColor(EmbedSettings.Get_Game_Color("P2EP-PSP", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P2EP-PSP"));

            embed.WithDescription("" +
                "Toggle inverted colors for character sprites on and off.\n" +
                "\n" +
                $"⚙️ **Current setting:** **`{account.P2EP_PSP_TS_Invert}`**\n");

            embed.WithImageUrl("https://i.imgur.com/epDRucE.png");

            menuSession.CurrentMenu = "Template_Layout_P2EP_PSP_Inverted_Filter";

            var component = new ComponentBuilder()
                .WithButton("↩️", customId: "return", ButtonStyle.Secondary)
                .WithButton("✅ On", customId: "on", ButtonStyle.Secondary)
                .WithButton("❌ Off", customId: "off", ButtonStyle.Secondary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_P2EP_PSP_Placement(MenuIdStructure menuSession)
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

            embed.WithColor(EmbedSettings.Get_Game_Color("P2EP-PSP", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P2EP-PSP"));

            embed.WithDescription("" +
                "Choose the default position character sprites are rendered at.\n" +
                "\n" +
                $"⚙️ **Current setting:** **`{account.P2EP_PSP_TS_Position}`**\n" +
                "\n" +
                ":one: Default - *Use the set default positions for each sprite.*\n" +
                ":two: Left\n" +
                ":three: Right\n");

            embed.WithImageUrl("https://i.imgur.com/tb5FK67.png");

            menuSession.CurrentMenu = "Template_Layout_P2EP_PSP_Placement";

            var selectMenu = new SelectMenuBuilder()
                    .WithPlaceholder("Select an option")
                    .WithCustomId(menuSession.CurrentMenu)
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .AddOption("Default", "1", null, new Emoji("1️⃣"))
                    .AddOption("Left", "2", null, new Emoji("2️⃣"))
                    .AddOption("Right", "3", null, new Emoji("3️⃣"))
                    .AddOption("Return to P2EP Template Settings", "return", null, new Emoji("↩️"));

            var component = new ComponentBuilder()
                .WithSelectMenu(selectMenu);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_P2EP_PSP_Sprite_Flip(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Sprite Flip",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            embed.WithColor(EmbedSettings.Get_Game_Color("P2EP-PSP", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P2EP-PSP"));

            embed.WithDescription("" +
                "Choose to flip sprites horizontally from their default orientation.\n" +
                "\n" +
                $"⚙️ **Current setting:** **`{account.P2EP_PSP_TS_Sprite_Flip}`**\n");

            embed.WithImageUrl("https://i.imgur.com/soTGtFv.png");

            menuSession.CurrentMenu = "Template_Layout_P2EP_PSP_Sprite_Flip";

            var component = new ComponentBuilder()
                .WithButton("↩️", customId: "return", ButtonStyle.Secondary)
                .WithButton("✅ On", customId: "on", ButtonStyle.Secondary)
                .WithButton("❌ Off", customId: "off", ButtonStyle.Secondary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_P2EP_PSP_Window_Color_Confirm(MenuIdStructure menuSession)
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

            embed.WithColor(EmbedSettings.Get_Game_Color("P2EP-PSP", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P2EP-PSP"));

            embed.WithDescription("" +
                $"The message window's color been set to **`{account.P2EP_PSP_TS_Window_Color}`**.\n");

            menuSession.CurrentMenu = "Template_Layout_P2EP_PSP_Window_Color_Confirm";

            var component = new ComponentBuilder()
                .WithButton("💠 P2EP Template Settings", customId: "back-to-p2ep-psp-template-settings", ButtonStyle.Primary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_P2EP_PSP_Inverted_Filter_Confirm(MenuIdStructure menuSession)
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

            embed.WithColor(EmbedSettings.Get_Game_Color("P2EP-PSP", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P2EP-PSP"));

            embed.WithDescription("" +
                $"Inverted colors for character sprites have been set to **`{account.P2EP_PSP_TS_Invert}`**.\n");

            menuSession.CurrentMenu = "Template_Layout_P2EP_PSP_Inverted_Filter_Confirm";

            var component = new ComponentBuilder()
                .WithButton("💠 P2EP Template Settings", customId: "back-to-p2ep-psp-template-settings", ButtonStyle.Primary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_P2EP_PSP_Placement_Confirm(MenuIdStructure menuSession)
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

            embed.WithColor(EmbedSettings.Get_Game_Color("P2EP-PSP", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P2EP-PSP"));

            embed.WithDescription("" +
                $"Sprite placements have been set to **`{account.P2EP_PSP_TS_Position}`**.\n");

            menuSession.CurrentMenu = "Template_Layout_P2EP_PSP_Placement_Confirm";

            var component = new ComponentBuilder()
                .WithButton("💠 P2EP Template Settings", customId: "back-to-p2ep-psp-template-settings", ButtonStyle.Primary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_P2EP_PSP_Sprite_Flip_Confirm(MenuIdStructure menuSession)
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

            embed.WithColor(EmbedSettings.Get_Game_Color("P2EP-PSP", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P2EP-PSP"));

            embed.WithDescription("" +
                $"Sprite flip has been set to **`{account.P2EP_PSP_TS_Sprite_Flip}`**.\n");

            menuSession.CurrentMenu = "Template_Layout_P2EP_PSP_Sprite_Flip_Confirm";

            var component = new ComponentBuilder()
                .WithButton("💠 P2EP Template Settings", customId: "back-to-p2ep-psp-template-settings", ButtonStyle.Primary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }
    }
}
