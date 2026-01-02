using System.Threading.Tasks;
using Discord;

namespace SocialLinker.Core.Menus.Settings.Main.SceneMaker.ResolutionScaling
{
    class Resolution_Scaling_P2IS_PSP_Menu
    {
        public static async Task Resolution_Scaling_P2IS_PSP_Main(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Resolution & Scaling - Persona 2: Innocent Sin (PSP®️)",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);
            embed.WithColor(EmbedSettings.Get_Game_Color("P2IS-PSP", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P2IS-PSP"));

            embed.WithDescription("" +
                "Select a setting to edit.\n");

            menuSession.CurrentMenu = "Resolution_Scaling_P2IS_PSP_Main";

            var selectMenu = new SelectMenuBuilder()
                    .WithPlaceholder("Select an option")
                    .WithCustomId(menuSession.CurrentMenu)
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .AddOption("Output Resolution", "1", null, new Emoji("1️⃣"))
                    .AddOption("Scaling Method", "2", null, new Emoji("2️⃣"))
                    .AddOption("Return to Version Select", "return", null, new Emoji("↩️"));

            var component = new ComponentBuilder()
                .WithSelectMenu(selectMenu);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Resolution_Scaling_P2IS_PSP_Output_Resolution(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Output Resolution",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);
            embed.WithColor(EmbedSettings.Get_Game_Color("P2IS-PSP", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P2IS-PSP"));

            embed.WithDescription("" +
                "Choose a resolution to output your scenes in.\n" +
                "\n" +
                $"⚙️ **Current setting:** **`{account.P2IS_PSP_Resolution}`**\n");

            menuSession.CurrentMenu = "Resolution_Scaling_P2IS_PSP_Output_Resolution";

            var selectMenu = new SelectMenuBuilder()
                    .WithPlaceholder("Select an option")
                    .WithCustomId(menuSession.CurrentMenu)
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .AddOption("480 × 272", "1", "Original PlayStation® Portable output resolution.", new Emoji("1️⃣"))
                    .AddOption("1920 × 1088", "2", "Scaled HD resolution.", new Emoji("2️⃣"))
                    .AddOption("P2IS (PSP®️) Resolution & Scaling Menu", "return", null, new Emoji("↩️"));

            var component = new ComponentBuilder()
                .WithSelectMenu(selectMenu);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Resolution_Scaling_P2IS_PSP_Scaling_Method(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Scaling Method",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);
            embed.WithColor(EmbedSettings.Get_Game_Color("P2IS-PSP", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P2IS-PSP"));

            embed.WithDescription("" +
                "Choose a scaling method for your scenes. (This will not affect output resolutions on the lowest setting.)\n" +
                "\n" +
                $"⚙️ **Current setting:** **`{account.P2IS_PSP_Scale}`**\n");

            menuSession.CurrentMenu = "Resolution_Scaling_P2IS_PSP_Scaling_Method";

            var selectMenu = new SelectMenuBuilder()
                    .WithPlaceholder("Select an option")
                    .WithCustomId(menuSession.CurrentMenu)
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .AddOption("Bicubic", "1", null, new Emoji("1️⃣"))
                    .AddOption("Nearest Neighbor", "2", null, new Emoji("2️⃣"))
                    .AddOption("P2IS (PSP®️) Resolution & Scaling Menu", "return", null, new Emoji("↩️"));

            var component = new ComponentBuilder()
                .WithSelectMenu(selectMenu);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Resolution_Scaling_P2IS_PSP_Output_Resolution_Confirm(MenuIdStructure menuSession)
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
            embed.WithColor(EmbedSettings.Get_Game_Color("P2IS-PSP", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P2IS-PSP"));

            embed.WithDescription("" +
                $"Scenes will be output at a **`{account.P2IS_PSP_Resolution}`** resolution.\n");

            menuSession.CurrentMenu = "Resolution_Scaling_P2IS_PSP_Output_Resolution_Confirm";

            var component = new ComponentBuilder()
                .WithButton("💠 P2IS (PSP®️) Resolution & Scaling Menu", customId: "back-to-p2is-psp-resolution-scaling", ButtonStyle.Primary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Resolution_Scaling_P2IS_PSP_Scaling_Method_Confirm(MenuIdStructure menuSession)
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
            embed.WithColor(EmbedSettings.Get_Game_Color("P2IS-PSP", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P2IS-PSP"));

            embed.WithDescription("" +
                $"Scenes will be scaled with **`{account.P2IS_PSP_Scale}`** interpolation.\n");

            menuSession.CurrentMenu = "Resolution_Scaling_P2IS_PSP_Scaling_Method_Confirm";

            var component = new ComponentBuilder()
                .WithButton("💠 P2IS (PSP®️) Resolution & Scaling Menu", customId: "back-to-p2is-psp-resolution-scaling", ButtonStyle.Primary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }
    }
}
