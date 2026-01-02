using System.Threading.Tasks;
using Discord;

namespace SocialLinker.Core.Menus.Settings.Main.SceneMaker.ResolutionScaling
{
    class Resolution_Scaling_VC_Menu
    {
        public static async Task Resolution_Scaling_VC_P1_Main(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Version Select",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);
            embed.WithColor(EmbedSettings.Get_Game_Color("P1-PSP", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P1-PSP"));

            embed.WithDescription("" +
                "Which version would you like to edit?\n");

            embed.WithImageUrl("https://i.imgur.com/bCWThuf.png");

            menuSession.CurrentMenu = "Resolution_Scaling_VC_P1_Main";

            var selectMenu = new SelectMenuBuilder()
                    .WithPlaceholder("Select a version")
                    .WithCustomId("resolution-scaling-vc-p1-main")
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .AddOption("Revelations: Persona", "P1-PS1", emote: Emote.Parse(Global.GetGameEmote("P1-PS1")))
                    .AddOption("Persona (PSP®️)", "P1-PSP", emote: Emote.Parse(Global.GetGameEmote("P1-PSP")))
                    .AddOption("Return to Resolution & Scaling Main Menu", "return", null, new Emoji("↩️"));

            var component = new ComponentBuilder()
                .WithSelectMenu(selectMenu);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Resolution_Scaling_VC_P2IS_Main(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Version Select",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);
            embed.WithColor(EmbedSettings.Get_Game_Color("P2IS-PSP", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P2IS-PSP"));

            embed.WithDescription("" +
                "Which version would you like to edit?\n");

            embed.WithImageUrl("https://i.imgur.com/6Utgced.png");

            menuSession.CurrentMenu = "Resolution_Scaling_VC_P2IS_Main";

            var selectMenu = new SelectMenuBuilder()
                    .WithPlaceholder("Select a version")
                    .WithCustomId("resolution-scaling-vc-p2is-main")
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .AddOption("Persona 2: Innocent Sin (PlayStation®️)", "P2IS-PS1", emote: Emote.Parse(Global.GetGameEmote("P2IS-PS1")))
                    .AddOption("Persona 2: Innocent Sin (PSP®️)", "P2IS-PSP", emote: Emote.Parse(Global.GetGameEmote("P2IS-PSP")))
                    .AddOption("Return to Resolution & Scaling Main Menu", "return", null, new Emoji("↩️"));

            var component = new ComponentBuilder()
                .WithSelectMenu(selectMenu);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Resolution_Scaling_VC_P2EP_Main(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Version Select",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);
            embed.WithColor(EmbedSettings.Get_Game_Color("P2EP-PSP", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P2EP-PSP"));

            embed.WithDescription("" +
                "Which version would you like to edit?\n");

            embed.WithImageUrl("https://i.imgur.com/JAZN3dP.png");

            menuSession.CurrentMenu = "Resolution_Scaling_VC_P2EP_Main";

            var selectMenu = new SelectMenuBuilder()
                    .WithPlaceholder("Select a version")
                    .WithCustomId("resolution-scaling-vc-p2ep-main")
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .AddOption("Persona 2: Eternal Punishment (PlayStation®️)", "P2EP-PS1", emote: Emote.Parse(Global.GetGameEmote("P2EP-PS1")))
                    .AddOption("Persona 2: Eternal Punishment (PSP®️)", "P2EP-PSP", emote: Emote.Parse(Global.GetGameEmote("P2EP-PSP")))
                    .AddOption("Return to Resolution & Scaling Main Menu", "return", null, new Emoji("↩️"));

            var component = new ComponentBuilder()
                .WithSelectMenu(selectMenu);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Resolution_Scaling_VC_P3_Main(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Version Select",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);
            embed.WithColor(EmbedSettings.Get_Game_Color("P3F", null));
            embed.WithThumbnailUrl("https://i.imgur.com/trtPflx.png");

            embed.WithDescription("" +
                "Which version would you like to edit?\n");

            embed.WithImageUrl("https://i.imgur.com/hZJTcx4.png");

            menuSession.CurrentMenu = "Resolution_Scaling_VC_P3_Main";

            var selectMenu = new SelectMenuBuilder()
                    .WithPlaceholder("Select a version")
                    .WithCustomId("resolution-scaling-vc-p3-main")
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .AddOption("Persona 3 FES", "P3F", emote: Emote.Parse(Global.GetGameEmote("P3F")))
                    .AddOption("Persona 3 Portable", "P3P", emote: Emote.Parse(Global.GetGameEmote("P3P")))
                    .AddOption("Return to Resolution & Scaling Main Menu", "return", null, new Emoji("↩️"));

            var component = new ComponentBuilder()
                .WithSelectMenu(selectMenu);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }
    }
}
