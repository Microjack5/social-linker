using System.Threading.Tasks;
using Discord;

namespace SocialLinker.Core.Menus.Settings.Main.SceneMaker.ResolutionScaling
{
    class Resolution_Scaling_P4_PS2_Menu
    {
        public static async Task Resolution_Scaling_P4_PS2_Main(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Resolution & Scaling - Persona 4 (PlayStation® 2)",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);
            embed.WithColor(EmbedSettings.Get_Game_Color("P4-PS2", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P4-PS2"));

            embed.WithDescription("" +
                "Select a setting to edit.\n");

            menuSession.CurrentMenu = "Resolution_Scaling_P4_PS2_Main";

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

        public static async Task Resolution_Scaling_P4_PS2_Output_Resolution(MenuIdStructure menuSession)
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
            embed.WithColor(EmbedSettings.Get_Game_Color("P4-PS2", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P4-PS2"));

            embed.WithDescription("" +
                "Choose a resolution to output your scenes in.\n" +
                "\n" +
                $"⚙️ **Current setting:** **`{account.P4_PS2_Resolution}`**\n");

            menuSession.CurrentMenu = "Resolution_Scaling_P4_PS2_Output_Resolution";

            var selectMenu = new SelectMenuBuilder()
                    .WithPlaceholder("Select an option")
                    .WithCustomId(menuSession.CurrentMenu)
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .AddOption("640 × 448", "1", "Original PlayStation® 2 internal resolution.", new Emoji("1️⃣"))
                    .AddOption("640 × 480", "2", "Original PlayStation® 2 output resolution.", new Emoji("2️⃣"))
                    .AddOption("1440 × 1080", "3", "Scaled HD resolution.", new Emoji("3️⃣"))
                    .AddOption("P4 (PlayStation® 2) Resolution & Scaling Menu", "return", null, new Emoji("↩️"));

            var component = new ComponentBuilder()
                .WithSelectMenu(selectMenu);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Resolution_Scaling_P4_PS2_Scaling_Method(MenuIdStructure menuSession)
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
            embed.WithColor(EmbedSettings.Get_Game_Color("P4-PS2", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P4-PS2"));

            embed.WithDescription("" +
                "Choose a scaling method for your scenes. (This will not affect output resolutions on the lowest setting.)\n" +
                "\n" +
                $"⚙️ **Current setting:** **`{account.P4_PS2_Scale}`**\n");

            menuSession.CurrentMenu = "Resolution_Scaling_P4_PS2_Scaling_Method";

            var selectMenu = new SelectMenuBuilder()
                    .WithPlaceholder("Select an option")
                    .WithCustomId(menuSession.CurrentMenu)
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .AddOption("Bicubic", "1", null, new Emoji("1️⃣"))
                    .AddOption("Nearest Neighbor", "2", null, new Emoji("2️⃣"))
                    .AddOption("P4 (PlayStation® 2) Resolution & Scaling Menu", "return", null, new Emoji("↩️"));

            var component = new ComponentBuilder()
                .WithSelectMenu(selectMenu);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Resolution_Scaling_P4_PS2_Output_Resolution_Confirm(MenuIdStructure menuSession)
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
            embed.WithColor(EmbedSettings.Get_Game_Color("P4-PS2", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P4-PS2"));

            embed.WithDescription("" +
                $"Scenes will be output at a **`{account.P4_PS2_Resolution}`** resolution.\n");

            menuSession.CurrentMenu = "Resolution_Scaling_P4_PS2_Output_Resolution_Confirm";

            var component = new ComponentBuilder()
                .WithButton("💠 P4 (PlayStation® 2) Resolution & Scaling Menu", customId: "back-to-p4-ps2-resolution-scaling", ButtonStyle.Primary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Resolution_Scaling_P4_PS2_Scaling_Method_Confirm(MenuIdStructure menuSession)
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
            embed.WithColor(EmbedSettings.Get_Game_Color("P4-PS2", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P4-PS2"));

            embed.WithDescription("" +
                $"Scenes will be scaled with **`{account.P4_PS2_Scale}`** interpolation.\n");

            menuSession.CurrentMenu = "Resolution_Scaling_P4_PS2_Scaling_Method_Confirm";

            var component = new ComponentBuilder()
                .WithButton("💠 P4 (PlayStation® 2) Resolution & Scaling Menu", customId: "back-to-p4-ps2-resolution-scaling", ButtonStyle.Primary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }
    }
}
