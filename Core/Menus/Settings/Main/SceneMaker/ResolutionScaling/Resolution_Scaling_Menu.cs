using System.Threading.Tasks;
using Discord;

namespace SocialLinker.Core.Menus.Settings.Main.SceneMaker.ResolutionScaling
{
    class Resolution_Scaling_Menu
    {
        public static async Task Resolution_Scaling_Main(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Resolution & Scaling",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            // Determine the color and thumbnail for the embeded message
            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Profile_Config_Thumbnail(account));

            embed.WithDescription("" +
                "Titles not originally rendered in 1080p can have their output resolutions and scaling methods adjusted.\n" +
                "\n" +
                "Choose a compatible title to adjust resolution & scaling settings for.\n");

            // Edit the menu session according to the current message.
            menuSession.CurrentMenu = "Resolution_Scaling_Main";

            var selectMenu = new SelectMenuBuilder()
                    .WithPlaceholder("Select an option")
                    .WithCustomId(menuSession.CurrentMenu)
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .AddOption("Persona", "P1", emote: Emote.Parse(Global.GetGameEmote("P1")))
                    .AddOption("Persona 2: Innocent Sin", "P2IS", emote: Emote.Parse(Global.GetGameEmote("P2IS")))
                    .AddOption("Persona 2: Eternal Punishment", "P2EP", emote: Emote.Parse(Global.GetGameEmote("P2EP")))
                    .AddOption("Persona 3", "P3", emote: Emote.Parse(Global.GetGameEmote("P3")))
                    .AddOption("Persona 4", "P4", emote: Emote.Parse(Global.GetGameEmote("P4")))
                    .AddOption("Persona 4 Arena Ultimax", "P4AU", emote: Emote.Parse(Global.GetGameEmote("P4AU")))
                    .AddOption("Return to Scene Maker Settings", "return", null, new Emoji("↩️"));

            var component = new ComponentBuilder()
                .WithSelectMenu(selectMenu);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }
    }
}
