using System.Threading.Tasks;
using Discord;

namespace SocialLinker.Core.Menus.Help.Main
{
    class SM_Tutorial_Select_Menu
    {
        public static async Task SM_Tutorial_Select_Main(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Scene Maker Tutorials",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Profile_Help_Thumbnail(account));

            embed.AddField(":one: Basic Tutorial",
                "Learn the basic essentials of how to use the scene maker.");
            embed.AddField(":two: Tips & Tricks",
                "Learn advanced scene maker features and techniques.");

            menuSession.CurrentMenu = "SM_Tutorial_Select_Main";

            var selectMenu = new SelectMenuBuilder()
                    .WithPlaceholder("Select an option")
                    .WithCustomId(menuSession.CurrentMenu)
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .AddOption("Basic Tutorial", "1", null, new Emoji("1️⃣"))
                    .AddOption("Tips & Tricks", "2", null, new Emoji("2️⃣"))
                    .AddOption("Return to Help Menu", "return", null, new Emoji("↩️"));

            var component = new ComponentBuilder()
                .WithSelectMenu(selectMenu);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task SM_Tutorial_Select_Advanced(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Scene Maker Tutorials: Tips & Tricks",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Profile_Help_Thumbnail(account));

            embed.AddField(":one: Version Control",
                "Use templates and sprites based on different release versions.");

            embed.AddField(":two: Spriteless Scenes",
                "Create scenes without character sprites.");

            embed.AddField(":three: Multi-Character Scenes",
                "Place two or more characters in a single scene.");

            embed.AddField(":four: Animation Frames",
                "Use eye and mouth animation frames on character sprites.");

            embed.AddField(":five: Line Breaks",
                "Insert manual line breaks at any time.");
            /*embed.AddField(":five: Cut-ins",
                "Use unique character sprites in certain templates."); */

            menuSession.CurrentMenu = "SM_Tutorial_Select_Advanced";

            var selectMenu = new SelectMenuBuilder()
                    .WithPlaceholder("Select an option")
                    .WithCustomId(menuSession.CurrentMenu)
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .AddOption("Version Control", "1", null, new Emoji("1️⃣"))
                    .AddOption("Spriteless Scenes", "2", null, new Emoji("2️⃣"))
                    .AddOption("Multi-Character Scenes", "3", null, new Emoji("3️⃣"))
                    .AddOption("Animation Frames", "4", null, new Emoji("4️⃣"))
                    .AddOption("Line Breaks", "5", null, new Emoji("5️⃣"))
                    .AddOption("Return to Scene Maker Tutorials", "return", null, new Emoji("↩️"));

            var component = new ComponentBuilder()
                .WithSelectMenu(selectMenu);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }
    }
}
