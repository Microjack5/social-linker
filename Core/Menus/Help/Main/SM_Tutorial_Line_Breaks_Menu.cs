using Discord.Rest;
using Discord.WebSocket;
using Discord;
using SocialLinker.Config;
using SocialLinker.Core.CloudStorageTables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace SocialLinker.Core.Menus.Help.Main
{
    class SM_Tutorial_Line_Breaks_Menu
    {
        public static async Task SM_Tutorial_Line_Breaks_Page_1(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Line Breaks",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            var footer = new EmbedFooterBuilder
            {
                Text = "Page 1 / 1"
            };

            embed.WithFooter(footer);

            // Determine the color and thumbnail for the embeded message
            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Profile_Help_Thumbnail(account));

            embed.WithDescription("" +
                "Message-based commands have a distinct advantage of being able to input text on multiple lines.\n" +
                "\n" +
                "To insert a manual line break in your scene, simply input a line break into your message as normal.\n");

            embed.WithImageUrl("https://i.imgur.com/miDb0ar.png");

            menuSession.CurrentMenu = "SM_Tutorial_Line_Breaks_Page_1";

            var component = new ComponentBuilder()
                .WithButton("↩️ Return", customId: "return", ButtonStyle.Secondary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }
    }
}
