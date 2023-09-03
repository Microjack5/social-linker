using Discord;
using Discord.WebSocket;
using SocialLinker.Config;
using System.Threading.Tasks;

namespace SocialLinker.Core.Menus.InitialUsage
{
    internal class InviteMessage
    {
        public static async Task SendInviteMessage(SocketGuild guild)
        {
            ISocketMessageChannel channel = guild.SystemChannel;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = $"Thank You for Adding Social Linker!",
                IconUrl = "https://i.imgur.com/HKfTInD.png"
            };
            embed.Author = author;

            embed.WithDescription("" +
                "Social Linker is a fan project in the works since 2019 based on the Persona franchise from Atlus Co., Ltd.! " +
                "Track your Discord activity with social stats and create realistic screenshots from various games across the series.\n" +
                "\n" +
                "If you’ve used Social Linker in another server, you can pick up your progress right from where you left off.\n" +
                "\n" +
                $"Use the **`help`** slash command to get started.\n" +
                "\n" +
                "**Content: :copyright: ATLUS / SEGA / KOEI TECMO GAMES / ARC SYSTEM WORKS / FRENCH-BREAD / Rooster Teeth Productions, LLC. / Team ARCANA / Marvelous, Inc. / SUBTLE STYLE**\n" +
                "```Social Linker is not affiliated, associated, authorized, maintained, sponsored, endorsed by, or in any way officially connected with these trademark and copyright holders. All content present is intended to fall under fair use.```");

            embed.WithColor(82, 236, 243);
            embed.WithImageUrl("https://i.imgur.com/XpKdixY.png");

            await channel.SendMessageAsync("", false, embed.Build());
        }

    }
}
