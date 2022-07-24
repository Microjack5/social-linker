using System;
using System.Drawing;
using System.Threading.Tasks;
using Discord.Commands;
using Fergun.Interactive;
using SocialLinker.Core.SceneMaker.GlyphParsing;
using System.IO;
using Discord.WebSocket;
using SocialLinker.Config;
using SocialLinker.Core.LocalStorageTables;
using Discord;
using Discord.Rest;
using SocialLinker.Core.CloudStorageTables;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using System.Collections.Generic;
using SocialLinker.Core.SceneMaker.Data.Bustup;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using SocialLinker.Core.Menus;

namespace SocialLinker.Core.SceneMaker.TemplateRenders.QuickScenes
{
    public class RenderBBTAG : ModuleBase<SocketCommandContext>
    {
        public static EmbedBuilder BBTAG_Loading_Message(string series)
        {
            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = $"Generating Scene...",
                IconUrl = EmbedSettings.Get_Game_Logo("BBTAG")
            };

            embed.WithAuthor(author);
            embed.WithColor(EmbedSettings.Get_BBTAG_Series_Color(series));
            embed.WithThumbnailUrl(EmbedSettings.Get_Loading_Icon("BBTAG", null));
            embed.WithDescription("This may take a few seconds!");

            return embed;
        }
    }
}
