using System.Threading.Tasks;
using Discord;

namespace SocialLinker.Core.Menus.Help.Main
{
    class Legal_Notices_Menu
    {
        public static async Task Legal_Notices_Main(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Legal Notices",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Profile_Help_Thumbnail(account));

            embed.WithDescription(
                "**Content: :copyright: ATLUS / SEGA / KOEI TECMO GAMES / ARC SYSTEM WORKS / FRENCH-BREAD / Rooster Teeth Productions, LLC. / Team ARCANA / Marvelous, Inc. / SUBTLE STYLE**\n" +
                "\n" +
                "\"PlayStation\", \"PS\", \"PS2\", \"PS3\", \"PS4\", \"PS5\", \"PSP\", \"PSVITA\", and the \"PS\" Family logos are either registered trademarks or trademarks of Sony Interactive Entertainment Inc.\n" +
                "\n" +
                "Xbox 360, Xbox One, Xbox Series X|S, and the Xbox logos are either registered trademarks or trademarks of the Microsoft group of companies.\n" +
                "\n" +
                "Wii U and Nintendo Switch are trademarks of Nintendo.\n" +
                "\n" +
                "The ratings icon is a registered trademark of the Entertainment Software Association.\n" +
                "\n" +
                "All other trademarks are property of their respective owners.\n" +
                "\n" +
                "```Social Linker is not affiliated, associated, authorized, maintained, sponsored, endorsed by, or in any way officially connected with these trademark and copyright holders. All content present is intended to fall under fair use.```");

            menuSession.CurrentMenu = "Legal_Notices_Main";

            var component = new ComponentBuilder()
                .WithButton("↩️ Return", customId: "return", ButtonStyle.Secondary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }
    }
}
