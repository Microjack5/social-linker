using System.Threading.Tasks;
using Discord;

namespace SocialLinker.Core.Menus.Help.Main
{
    class SM_Tutorial_VC_Menu
    {
        public static async Task SM_Tutorial_VC_Main(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Scene Maker Tutorials: Version Control",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Profile_Help_Thumbnail(account));

            embed.AddField(":one: Basic Tutorial",
                "What is version control?");
            embed.AddField(":two: Auto-switching",
                "Switch versions based on character.");
            embed.AddField(":three: Bypass Version Control",
                "Switch between game versions instantly.");

            menuSession.CurrentMenu = "SM_Tutorial_VC_Main";

            var selectMenu = new SelectMenuBuilder()
                    .WithPlaceholder("Select an option")
                    .WithCustomId(menuSession.CurrentMenu)
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .AddOption("Basic Tutorial", "1", null, new Emoji("1️⃣"))
                    .AddOption("Auto-switching", "2", null, new Emoji("2️⃣"))
                    .AddOption("Bypass Version Control", "3", null, new Emoji("3️⃣"))
                    .AddOption("Return to Help Menu", "return", null, new Emoji("↩️"));

            var component = new ComponentBuilder()
                .WithSelectMenu(selectMenu);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task SM_Tutorial_VC_Basic_Page_1(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "What is Version Control?",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            var footer = new EmbedFooterBuilder
            {
                Text = "Page 1 / 2"
            };

            embed.WithFooter(footer);

            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Profile_Help_Thumbnail(account));

            embed.WithDescription("" +
                "Version Control lets you set which version of a game your scene maker commands use by default.\n" +
                "\n" +
                "For example, \"Persona 3 FES\" and \"Persona 3 Portable\" might be two versions of \"Persona 3\", but there are lots of differences between them.");

            embed.WithImageUrl("https://i.imgur.com/wbs5bfg.png");

            menuSession.CurrentMenu = "SM_Tutorial_VC_Basic_Page_1";

            var component = new ComponentBuilder()
                .WithButton("↩️ Return", customId: "return", ButtonStyle.Secondary)
                .WithButton("▶️ Next Page", customId: "next-page", ButtonStyle.Secondary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task SM_Tutorial_VC_Basic_Page_2(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Version Control Settings",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            var footer = new EmbedFooterBuilder
            {
                Text = "Page 2 / 2"
            };

            embed.WithFooter(footer);

            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Profile_Help_Thumbnail(account));

            embed.WithDescription("" +
                $"Many titles in the series have different Version Control settings! You can choose the versions you want from the **`settings`** menu and choosing [Scene Maker Settings] > [Version Control].\n");

            embed.WithImageUrl("https://i.imgur.com/PdJAyYJ.gif");

            menuSession.CurrentMenu = "SM_Tutorial_VC_Basic_Page_2";

            var component = new ComponentBuilder()
                .WithButton("◀️ Previous Page", customId: "previous-page", ButtonStyle.Secondary)
                .WithButton("⏭️ Next Tutorial", customId: "next-tutorial", ButtonStyle.Secondary)
                .WithButton("💠 Return to Version Control Tutorials", customId: "back-to-vc-tutorials", ButtonStyle.Secondary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task SM_Tutorial_VC_Auto_Page_1(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Auto-Switching",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            var footer = new EmbedFooterBuilder
            {
                Text = "Page 1 / 2"
            };

            embed.WithFooter(footer);

            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Profile_Help_Thumbnail(account));

            embed.WithDescription("" +
                "Some characters might only appear in one version of a game.\n" +
                "\n" +
                "If a character you want to use doesn't match up with your Version Control settings, the scene you make will automatically use the same game style as the version they’re from. No need to change your settings!");

            embed.WithImageUrl("https://i.imgur.com/nlROP3q.png");

            menuSession.CurrentMenu = "SM_Tutorial_VC_Auto_Page_1";

            var component = new ComponentBuilder()
                .WithButton("↩️ Return", customId: "return", ButtonStyle.Secondary)
                .WithButton("▶️ Next Page", customId: "next-page", ButtonStyle.Secondary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task SM_Tutorial_VC_Auto_Page_2(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Auto-Switching",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            var footer = new EmbedFooterBuilder
            {
                Text = "Page 2 / 2"
            };

            embed.WithFooter(footer);

            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Profile_Help_Thumbnail(account));

            embed.WithDescription("" +
                $"If you want to keep using their game style for other characters, make sure to change your Version Control settings appropriately from the **`settings`** menu by choosing [Scene Maker Settings] > [Version Control].\n");

            embed.WithImageUrl("https://i.imgur.com/b9buvHy.png");

            menuSession.CurrentMenu = "SM_Tutorial_VC_Auto_Page_2";

            var component = new ComponentBuilder()
                .WithButton("◀️ Previous Page", customId: "previous-page", ButtonStyle.Secondary)
                .WithButton("⏭️ Next Tutorial", customId: "next-tutorial", ButtonStyle.Secondary)
                .WithButton("💠 Return to Version Control Tutorials", customId: "back-to-vc-tutorials", ButtonStyle.Secondary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task SM_Tutorial_VC_Bypass_Page_1(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Bypassing Version Control",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            var footer = new EmbedFooterBuilder
            {
                Text = "Page 1 / 2"
            };

            embed.WithFooter(footer);

            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Profile_Help_Thumbnail(account));

            embed.WithDescription("" +
                "Although game style keywords usually follow your Version Control settings, you can bypass this via message commands with an expanded list of keywords that specifies each version for all compatible titles.\n");

            embed.AddField("Persona Keywords", "" +
                "<:P1_PS1:824469261316915220> `P1-PS1` - **Revelations: Persona**\n" +
                "<:P1:751133115531133112> `P1-PSP` - **Persona (PSP®️)**");

            embed.AddField("Persona 2: Innocent Sin Keywords", "" +
                "<:P2IS:788950080396328990> `P2IS-PS1` - **Persona 2: Innocent Sin (PlayStation®️)**\n" +
                "<:P2IS:788950080396328990> `P2IS-PSP` - **Persona 2: Innocent Sin (PSP®️)**");

            embed.AddField("Persona 2: Eternal Punishment Keywords", "" +
                "<:P2EP:788950163363463172> `P2EP-PS1` - **Persona 2: Eternal Punishment (PlayStation®️)**\n" +
                "<:P2EP:788950163363463172> `P2EP-PSP` - **Persona 2: Eternal Punishment (PSP®️)**");

            embed.AddField("Persona 3 Keywords", "" +
                "<:P3F:1096338540369039413> `P3F` - **Persona 3 FES**\n" +
                "<:P3P:1096338602046267392> `P3P` - **Persona 3 Portable**");

            embed.AddField("Persona 4 Keywords", "" +
                "<:P4:751133120530612274> `P4-PS2` - **Persona 4 (PlayStation®️ 2)**\n" +
                "<:P4G:751133123479207956> `P4G` - **Persona 4 Golden**");

            embed.AddField("Persona 5 Keywords", "" +
                "<:P5:751133123861020742> `P5-PS4` - **Persona 5 (PlayStation®️ 4)**\n" +
                "<:P5R:751133123617488937> `P5R` - **Persona 5 Royal**");

            embed.AddField("Single Version Keywords", "" +
                "<:P4AU:751133122342420572> `P4AU` - **Persona 4 Arena Ultimax**\n" +
                "<:P4D:751133120346062859> `P4D` - **Persona 4: Dancing All Night**\n" +
                "<:P5S:852644176188669972> `P5S` - **Persona 5 Strikers**\n" +
                "<:BBTAG:751133123013771617> `BBTAG` - **BlazBlue: Cross Tag Battle**");

            menuSession.CurrentMenu = "SM_Tutorial_VC_Bypass_Page_1";

            var component = new ComponentBuilder()
                .WithButton("↩️ Return", customId: "return", ButtonStyle.Secondary)
                .WithButton("▶️ Next Page", customId: "next-page", ButtonStyle.Secondary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task SM_Tutorial_VC_Bypass_Page_2(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Using Expanded Game Style Keywords",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            var footer = new EmbedFooterBuilder
            {
                Text = "Page 2 / 2"
            };

            embed.WithFooter(footer);

            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Profile_Help_Thumbnail(account));

            embed.WithDescription("" +
                "Like usual, you can also use these keywords to access version-specific character lists and sprite sheets.\n" +
                "\n" +
                "Master their usage and swap versions on the fly!");

            embed.WithImageUrl("https://i.imgur.com/OtjgqkC.png");

            menuSession.CurrentMenu = "SM_Tutorial_VC_Bypass_Page_2";

            var component = new ComponentBuilder()
                .WithButton("◀️ Previous Page", customId: "previous-page", ButtonStyle.Secondary)
                .WithButton("💠 Return to Version Control Tutorials", customId: "back-to-vc-tutorials", ButtonStyle.Secondary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }
    }
}
