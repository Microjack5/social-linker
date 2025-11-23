using System.Threading.Tasks;
using Discord;

namespace SocialLinker.Core.Menus.Settings.Main.SceneMaker.ResolutionScaling
{
    class Resolution_Scaling_P4AU_Menu
    {
        public static async Task Resolution_Scaling_P4AU_Main(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Resolution & Scaling - Persona 4 Arena Ultimax",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);
            embed.WithColor(EmbedSettings.Get_Game_Color("P4AU", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P4AU"));

            embed.WithDescription("" +
                "Select a setting to edit.\n");

            menuSession.CurrentMenu = "Resolution_Scaling_P4AU_Main";

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

        public static async Task Resolution_Scaling_P4AU_Output_Resolution(MenuIdStructure menuSession)
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
            embed.WithColor(EmbedSettings.Get_Game_Color("P4AU", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P4AU"));

            embed.WithDescription("" +
                "Choose a resolution to output your scenes in.\n" +
                "\n" +
                $"⚙️ **Current setting:** **`{account.P4AU_Resolution}`**\n");

            menuSession.CurrentMenu = "Resolution_Scaling_P4AU_Output_Resolution";

            var selectMenu = new SelectMenuBuilder()
                    .WithPlaceholder("Select an option")
                    .WithCustomId(menuSession.CurrentMenu)
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .AddOption("1280 × 720", "1", "Original PlayStation® 3 output resolution.", new Emoji("1️⃣"))
                    .AddOption("1920 × 1080", "2", "Scaled HD resolution.", new Emoji("2️⃣"))
                    .AddOption("P4AU Resolution & Scaling Menu", "return", null, new Emoji("↩️"));

            var component = new ComponentBuilder()
                .WithSelectMenu(selectMenu);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Resolution_Scaling_P4AU_Scaling_Method(MenuIdStructure menuSession)
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
            embed.WithColor(EmbedSettings.Get_Game_Color("P4AU", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P4AU"));

            embed.WithDescription("" +
                "Choose a scaling method for your scenes. (This will not affect output resolutions on the lowest setting.)\n" +
                "\n" +
                $"⚙️ **Current setting:** **`{account.P4AU_Scale}`**\n");

            menuSession.CurrentMenu = "Resolution_Scaling_P4AU_Scaling_Method";

            var selectMenu = new SelectMenuBuilder()
                    .WithPlaceholder("Select an option")
                    .WithCustomId(menuSession.CurrentMenu)
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .AddOption("Bicubic", "1", null, new Emoji("1️⃣"))
                    .AddOption("Nearest Neighbor", "2", null, new Emoji("2️⃣"))
                    .AddOption("P4AU Resolution & Scaling Menu", "return", null, new Emoji("↩️"));

            var component = new ComponentBuilder()
                .WithSelectMenu(selectMenu);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Resolution_Scaling_P4AU_Output_Resolution_Confirm(MenuIdStructure menuSession)
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
            embed.WithColor(EmbedSettings.Get_Game_Color("P4AU", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P4AU"));

            embed.WithDescription("" +
                $"Scenes will be output at a **`{account.P4AU_Resolution}`** resolution.\n");

            menuSession.CurrentMenu = "Resolution_Scaling_P4AU_Output_Resolution_Confirm";

            var component = new ComponentBuilder()
                .WithButton("💠 P4AU Resolution & Scaling Menu", customId: "back-to-p4au-resolution-scaling", ButtonStyle.Primary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Resolution_Scaling_P4AU_Scaling_Method_Confirm(MenuIdStructure menuSession)
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
            embed.WithColor(EmbedSettings.Get_Game_Color("P4AU", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P4AU"));

            embed.WithDescription("" +
                $"Scenes will be scaled with **`{account.P4AU_Scale}`** interpolation.\n");

            menuSession.CurrentMenu = "Resolution_Scaling_P4AU_Scaling_Method_Confirm";

            var component = new ComponentBuilder()
                .WithButton("💠 P4AU Resolution & Scaling Menu", customId: "back-to-p4au-resolution-scaling", ButtonStyle.Primary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }
    }
}
