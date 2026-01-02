using System.Threading.Tasks;
using Discord;

namespace SocialLinker.Core.Menus.Settings.Main.SceneMaker.ResolutionScaling
{
    class Resolution_Scaling_P3F_Menu
    {
        public static async Task Resolution_Scaling_P3F_Main(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Resolution & Scaling - Persona 3 FES",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);
            embed.WithColor(EmbedSettings.Get_Game_Color("P3F", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P3F"));

            embed.WithDescription("" +
                "Select a setting to edit.\n");

            menuSession.CurrentMenu = "Resolution_Scaling_P3F_Main";

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

        public static async Task Resolution_Scaling_P3F_Output_Resolution(MenuIdStructure menuSession)
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
            embed.WithColor(EmbedSettings.Get_Game_Color("P3F", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P3F"));

            embed.WithDescription("" +
                "Choose a resolution to output your scenes in.\n" +
                "\n" +
                $"⚙️ **Current setting:** **`{account.P3F_Resolution}`**\n");

            menuSession.CurrentMenu = "Resolution_Scaling_P3F_Output_Resolution";

            var selectMenu = new SelectMenuBuilder()
                    .WithPlaceholder("Select an option")
                    .WithCustomId(menuSession.CurrentMenu)
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .AddOption("640 × 448", "1", "Original PlayStation® 2 internal resolution.", new Emoji("1️⃣"))
                    .AddOption("640 × 480", "2", "Original PlayStation® 2 output resolution.", new Emoji("2️⃣"))
                    .AddOption("1440 × 1080", "3", "Scaled HD resolution.", new Emoji("3️⃣"))
                    .AddOption("P3F Resolution & Scaling Menu", "return", null, new Emoji("↩️"));

            var component = new ComponentBuilder()
                .WithSelectMenu(selectMenu);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Resolution_Scaling_P3F_Scaling_Method(MenuIdStructure menuSession)
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
            embed.WithColor(EmbedSettings.Get_Game_Color("P3F", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P3F"));

            embed.WithDescription("" +
                "Choose a scaling method for your scenes. (This will not affect output resolutions on the lowest setting.)\n" +
                "\n" +
                $"⚙️ **Current setting:** **`{account.P3F_Scale}`**\n" +
                "\n" +
                ":one: Bicubic\n" +
                ":two: Nearest Neighbor\n");

            menuSession.CurrentMenu = "Resolution_Scaling_P3F_Scaling_Method";

            var selectMenu = new SelectMenuBuilder()
                    .WithPlaceholder("Select an option")
                    .WithCustomId(menuSession.CurrentMenu)
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .AddOption("Bicubic", "1", null, new Emoji("1️⃣"))
                    .AddOption("Nearest Neighbor", "2", null, new Emoji("2️⃣"))
                    .AddOption("P3F Resolution & Scaling Menu", "return", null, new Emoji("↩️"));

            var component = new ComponentBuilder()
                .WithSelectMenu(selectMenu);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Resolution_Scaling_P3F_Output_Resolution_Confirm(MenuIdStructure menuSession)
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
                $"Scenes will be output at a **`{account.P3F_Resolution}`** resolution.\n");

            menuSession.CurrentMenu = "Resolution_Scaling_P3F_Output_Resolution_Confirm";

            var component = new ComponentBuilder()
                .WithButton("💠 P3F Resolution & Scaling Menu", customId: "back-to-p3f-resolution-scaling", ButtonStyle.Primary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Resolution_Scaling_P3F_Scaling_Method_Confirm(MenuIdStructure menuSession)
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
                $"Scenes will be scaled with **`{account.P3F_Scale}`** interpolation.\n");

            menuSession.CurrentMenu = "Resolution_Scaling_P3F_Scaling_Method_Confirm";

            var component = new ComponentBuilder()
                .WithButton("💠 P3F Resolution & Scaling Menu", customId: "back-to-p3f-resolution-scaling", ButtonStyle.Primary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }
    }
}
