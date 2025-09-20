using System.Threading.Tasks;
using Discord;

namespace SocialLinker.Core.Menus.Settings.Main.SceneMaker
{
    class SM_Settings_Menu
    {
        public static async Task SM_Settings_Main(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Scene Maker Settings",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            // Determine the color and thumbnail for the embeded message
            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Profile_Config_Thumbnail(account));

            //embed.AddField(":three: Calendar Cycles",
            //    "Manually set the date, time of day, moon phases, and more.");

            embed.AddField(":one: Version Control",
                "Change the default templates and sprite sets available for different versions of the same title.");
            embed.AddField(":two: Template Layout",
                "Change select visual elements while using various templates.");
            embed.AddField(":three: Display Names",
                "Change the displayed names of various characters and sprite sets.");
            embed.AddField(":four: Sprite Sheet Order",
                "Change whether sprite sets are ordered by outfit or by expression.");
            embed.AddField(":five: Backgrounds",
                "Determine how background images are rendered within the scene maker.");
            embed.AddField(":six: Resolution & Scaling",
                "Change output resolutions and scaling methods per template.");
            embed.AddField(":seven: Auto-Delete",
                "Toggle auto-deletion of error messages and scene maker commands.");

            var selectMenu = new SelectMenuBuilder()
                    .WithPlaceholder("Select an option")
                    .WithCustomId("sm-settings-main-select")
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .AddOption("Version Control", "1", null, new Emoji("1️⃣"))
                    .AddOption("Template Layout", "2", null, new Emoji("2️⃣"))
                    .AddOption("Display Names", "3", null, new Emoji("3️⃣"))
                    .AddOption("Sprite Sheet Order", "4", null, new Emoji("4️⃣"))
                    .AddOption("Backgrounds", "5", null, new Emoji("5️⃣"))
                    .AddOption("Resolution & Scaling", "6", null, new Emoji("6️⃣"))
                    .AddOption("Auto-Delete", "7", null, new Emoji("7️⃣"))
                    .AddOption("Return to Settings Main Menu", "return", null, new Emoji("↩️"));

            var component = new ComponentBuilder()
                .WithSelectMenu(selectMenu);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);

            menuSession.CurrentMenu = "SM_Settings_Menu";
        }
    }
}
