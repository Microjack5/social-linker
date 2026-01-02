using System.Threading.Tasks;
using Discord;

namespace SocialLinker.Core.Menus.Settings.Main.SceneMaker
{
    class Template_Layout_Menu
    {
        public static async Task Template_Layout_Main(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Template Layout",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Profile_Config_Thumbnail(account));

            embed.WithDescription("" +
                "Choose a title to view the template settings for.\n");

            var selectMenu = new SelectMenuBuilder()
                    .WithPlaceholder("Select a title")
                    .WithCustomId("template-layout-main")
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .AddOption("Persona", "P1", emote: Emote.Parse(Global.GetGameEmote("P1")))
                    .AddOption("Persona 2: Innocent Sin", "P2IS", emote: Emote.Parse(Global.GetGameEmote("P2IS")))
                    .AddOption("Persona 2: Eternal Punishment", "P2EP", emote: Emote.Parse(Global.GetGameEmote("P2EP")))
                    .AddOption("Persona 3", "P3", emote: Emote.Parse(Global.GetGameEmote("P3")))
                    .AddOption("Persona 4", "P4", emote: Emote.Parse(Global.GetGameEmote("P4")))
                    .AddOption("Persona 4 Arena Ultimax", "P4AU", emote: Emote.Parse(Global.GetGameEmote("P4AU")))
                    .AddOption("Persona 4: Dancing All Night", "P4D", emote: Emote.Parse(Global.GetGameEmote("P4D")))
                    .AddOption("Persona 5", "P5", emote: Emote.Parse(Global.GetGameEmote("P5")))
                    .AddOption("Persona 5 Strikers", "P5S", emote: Emote.Parse(Global.GetGameEmote("P5S")))
                    .AddOption("BlazBlue: Cross Tag Battle", "BBTAG", emote: Emote.Parse(Global.GetGameEmote("BBTAG")))
                    .AddOption("Return to Scene Maker Settings", "return", null, new Emoji("↩️"));

            var component = new ComponentBuilder()
                .WithSelectMenu(selectMenu);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);

            menuSession.CurrentMenu = "Template_Layout_Main";
        }
    }
}
