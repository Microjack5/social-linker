using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;

namespace SocialLinker.Core.Menus.Settings.Main.Profile
{
    class Content_Filter_Menu
    {
        public static async Task Content_Filter_Main(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            // Find a filter session associated with the current user.
            var filterSession = Global.ContentFilterList.SingleOrDefault(x => x.User.Id == user.Id);

            // Check if the filter session is null.
            if (filterSession != null)
            {
                // If not, remove the content filter entry from the global list.
                Global.ContentFilterList.Remove(filterSession);
            }

            // Create a new content filter identifier entry for this current session and user to keep track of the overall status.
            filterSession = new ContentFilter()
            {
                User = user
            };

            // Add the filter session to the global list.
            Global.ContentFilterList.Add(filterSession);

            // Create a list variable containing the content filter of the command user.
            List<string> user_filter = ContentFilterMethods.ParseContentFilter(account);

            // Using the newly created content filter list, create a new list that converts all the game acronyms into proper titles.
            List<string> filter_titles = ContentFilterMethods.AcronymToTitle(user_filter);

            // Create an empty string variable.
            string filter_text = "";

            // Iterating through the title list, add each entry to the string variable.
            for (int i = 0; i < filter_titles.Count; i++)
            {
                filter_text += $"**`{filter_titles[i]}`**\n";
            }

            // If the string variable is still empty afterwards (meaning the user had no titles filtered), assign "None" to it.
            if (filter_text == "")
            {
                filter_text = "**`None`**\n";
            }

            // In case the user backtracks to this menu, set the values to activate all the other interactive menus and title options to false.
            filterSession.P1_Select = false;
            filterSession.P2IS_Select = false;
            filterSession.P2EP_Select = false;
            filterSession.P3_Select = false;
            filterSession.P4_Select = false;
            filterSession.P4AU_Select = false;
            filterSession.P4D_Select = false;
            filterSession.P5_Select = false;
            filterSession.BBTAG_Select = false;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Content Filter",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Profile_Config_Thumbnail(account));

            embed.WithDescription("" +
                "Choose to hide content related to certain titles. Select any titles you wish to block (or select nothing to reset), then press ✅ to continue.\n" +
                "\n" +
                $"⚙️ **Currently Filtered Titles:**\n" +
                $"\n" +
                $"{filter_text}");

            menuSession.CurrentMenu = "Content_Filter_Main";

            var selectMenu = new SelectMenuBuilder()
                    .WithPlaceholder("Select title(s)")
                    .WithCustomId("content-filter-main")
                    .WithMaxValues(10)
                    .AddOption("Persona", "p1", emote: Emote.Parse(Global.GetGameEmote("P1")))
                    .AddOption("Persona 2: Innocent Sin", "p2is", emote: Emote.Parse(Global.GetGameEmote("P2IS")))
                    .AddOption("Persona 2: Eternal Punishment", "p2ep", emote: Emote.Parse(Global.GetGameEmote("P2EP")))
                    .AddOption("Persona 3", "p3", emote: Emote.Parse(Global.GetGameEmote("P3")))
                    .AddOption("Persona 4", "p4", emote: Emote.Parse(Global.GetGameEmote("P4")))
                    .AddOption("Persona 4 Arena Ultimax", "p4au", emote: Emote.Parse(Global.GetGameEmote("P4AU")))
                    .AddOption("Persona 4: Dancing All Night", "p4d", emote: Emote.Parse(Global.GetGameEmote("P4D")))
                    .AddOption("Persona 5", "p5", emote: Emote.Parse(Global.GetGameEmote("P5")))
                    .AddOption("Persona 5 Strikers", "p5s", emote: Emote.Parse(Global.GetGameEmote("P5S")))
                    .AddOption("BlazBlue: Cross Tag Battle", "bbtag", emote: Emote.Parse(Global.GetGameEmote("BBTAG")));

            var component = new ComponentBuilder()
                .WithSelectMenu(selectMenu)
                .WithButton("↩️ Return", customId: "return", ButtonStyle.Secondary)
                .WithButton("✅ Confirm", customId: "confirm", ButtonStyle.Secondary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Content_Filter_VC_P1_Main(MenuIdStructure menuSession)
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
                "Which version of Persona would you like to block? Select all that apply, then react with ✅ to continue.");

            embed.WithImageUrl("https://i.imgur.com/bCWThuf.png");

            menuSession.CurrentMenu = "Content_Filter_VC_P1_Main";

            var selectMenu = new SelectMenuBuilder()
                    .WithPlaceholder("Select version(s)")
                    .WithCustomId("content-filter-vc-p1")
                    .WithMaxValues(2)
                    .AddOption("Revelations: Persona", "p1-ps1", emote: Emote.Parse(Global.GetGameEmote("P1-PS1")))
                    .AddOption("Persona (PSP®️)", "p1-psp", emote: Emote.Parse(Global.GetGameEmote("P1-PSP")));

            var component = new ComponentBuilder()
                .WithSelectMenu(selectMenu)
                .WithButton("↩️ Return", customId: "return", ButtonStyle.Secondary)
                .WithButton("✅ Confirm", customId: "confirm", ButtonStyle.Secondary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Content_Filter_VC_P2IS_Main(MenuIdStructure menuSession)
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
                "Which version of Persona 2: Innocent Sin would you like to block? Select all that apply, then react with ✅ to continue.");

            embed.WithImageUrl("https://i.imgur.com/JAZN3dP.png");

            menuSession.CurrentMenu = "Content_Filter_VC_P2IS_Main";

            var selectMenu = new SelectMenuBuilder()
                    .WithPlaceholder("Select version(s)")
                    .WithCustomId("content-filter-vc-p2is")
                    .WithMaxValues(2)
                    .AddOption("Persona 2: Innocent Sin (PlayStation®️)", "p2is-ps1", emote: Emote.Parse(Global.GetGameEmote("P2IS-PS1")))
                    .AddOption("Persona 2: Innocent Sin (PSP®️)", "p2is-psp", emote: Emote.Parse(Global.GetGameEmote("P2IS-PSP")));

            var component = new ComponentBuilder()
                .WithSelectMenu(selectMenu)
                .WithButton("↩️ Return", customId: "return", ButtonStyle.Secondary)
                .WithButton("✅ Confirm", customId: "confirm", ButtonStyle.Secondary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Content_Filter_VC_P2EP_Main(MenuIdStructure menuSession)
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
                "Which version of Persona 2: Eternal Punishment would you like to block? Select all that apply, then react with ✅ to continue.");

            embed.WithImageUrl("https://i.imgur.com/6Utgced.png");

            menuSession.CurrentMenu = "Content_Filter_VC_P2EP_Main";

            var selectMenu = new SelectMenuBuilder()
                    .WithPlaceholder("Select version(s)")
                    .WithCustomId("content-filter-vc-p2ep")
                    .WithMaxValues(2)
                    .AddOption("Persona 2: Eternal Punishment (PlayStation®️)", "p2ep-ps1", emote: Emote.Parse(Global.GetGameEmote("P2EP-PS1")))
                    .AddOption("Persona 2: Eternal Punishment (PSP®️)", "p2ep-psp", emote: Emote.Parse(Global.GetGameEmote("P2EP-PSP")));

            var component = new ComponentBuilder()
                .WithSelectMenu(selectMenu)
                .WithButton("↩️ Return", customId: "return", ButtonStyle.Secondary)
                .WithButton("✅ Confirm", customId: "confirm", ButtonStyle.Secondary); ;

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Content_Filter_VC_P3_Main(MenuIdStructure menuSession)
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
                "Which version of Persona 3 would you like to block? Select all that apply, then react with ✅ to continue.");

            embed.WithImageUrl("https://i.imgur.com/hZJTcx4.png");

            menuSession.CurrentMenu = "Content_Filter_VC_P3_Main";

            var selectMenu = new SelectMenuBuilder()
                    .WithPlaceholder("Select version(s)")
                    .WithCustomId("content-filter-vc-p3")
                    .WithMaxValues(2)
                    .AddOption("Persona 3 FES", "p3f", emote: Emote.Parse(Global.GetGameEmote("P3F")))
                    .AddOption("Persona 3 Portable", "p3p", emote: Emote.Parse(Global.GetGameEmote("P3P")));

            var component = new ComponentBuilder()
                .WithSelectMenu(selectMenu)
                .WithButton("↩️ Return", customId: "return", ButtonStyle.Secondary)
                .WithButton("✅ Confirm", customId: "confirm", ButtonStyle.Secondary); ;

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Content_Filter_VC_P4_Main(MenuIdStructure menuSession)
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

            embed.WithColor(EmbedSettings.Get_Game_Color("P4-PS2", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P4-PS2"));

            embed.WithDescription("" +
                "Which version of Persona 4 would you like to block? Select all that apply, then react with ✅ to continue.");

            embed.WithImageUrl("https://i.imgur.com/ZVldBKO.png");

            menuSession.CurrentMenu = "Content_Filter_VC_P4_Main";

            var selectMenu = new SelectMenuBuilder()
                .WithPlaceholder("Select version(s)")
                .WithCustomId("content-filter-vc-p4")
                .WithMaxValues(2)
                .AddOption("Persona 4 (PlayStation®️ 2)", "p4-ps2", emote: Emote.Parse(Global.GetGameEmote("P4-PS2")))
                .AddOption("Persona 4 Golden", "p4g", emote: Emote.Parse(Global.GetGameEmote("P4G")));

            var component = new ComponentBuilder()
                .WithSelectMenu(selectMenu)
                .WithButton("↩️ Return", customId: "return", ButtonStyle.Secondary)
                .WithButton("✅ Confirm", customId: "confirm", ButtonStyle.Secondary); ;

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Content_Filter_VC_P5_Main(MenuIdStructure menuSession)
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

            embed.WithColor(EmbedSettings.Get_Game_Color("P5-PS4", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P5-PS4"));

            embed.WithDescription("" +
                "Which version of Persona 5 would you like to block? Select all that apply, then react with ✅ to continue.");

            embed.WithImageUrl("https://i.imgur.com/7PMim5v.png");

            menuSession.CurrentMenu = "Content_Filter_VC_P5_Main";

            var selectMenu = new SelectMenuBuilder()
                .WithPlaceholder("Select version(s)")
                .WithCustomId("content-filter-vc-p5")
                .WithMaxValues(2)
                .AddOption("Persona 5 (PlayStation®️ 4)", "p5-ps4", emote: Emote.Parse(Global.GetGameEmote("P5-PS4")))
                .AddOption("Persona 5 Royal", "p5r", emote: Emote.Parse(Global.GetGameEmote("P5R")));

            var component = new ComponentBuilder()
                .WithSelectMenu(selectMenu)
                .WithButton("↩️ Return", customId: "return", ButtonStyle.Secondary)
                .WithButton("✅ Confirm", customId: "confirm", ButtonStyle.Secondary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Content_Filter_Confirm(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            // Search for a content filter list that corresponds to the user's ID.
            var filterSession = Global.ContentFilterList.SingleOrDefault(x => x.User.Id == menuSession.User.Id);

            // Create a list variable containing the content filter of the command user.
            List<string> user_filter = ContentFilterMethods.ParseContentFilter(account);

            // Using the newly created content filter list, create a new list that converts all the game acronyms into proper titles.
            List<string> filter_titles = ContentFilterMethods.AcronymToTitle(user_filter);

            // Create an empty string variable.
            string filter_text = "";

            // Iterating through the title list, add each entry to the string variable.
            for (int i = 0; i < filter_titles.Count; i++)
            {
                filter_text += $"**`{filter_titles[i]}`**\n";
            }

            // Create a new embed that will be displayed in the message.
            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Settings Saved",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            // Determine the color and thumbnail for the embeded message.
            if (account.Profile_Theme == "P3")
            {
                embed.WithColor(37, 149, 255);
                embed.WithThumbnailUrl("https://i.imgur.com/7xnoaQ7.png");
            }
            else if (account.Profile_Theme == "P4")
            {
                embed.WithColor(255, 229, 49);
                embed.WithThumbnailUrl("https://i.imgur.com/4vtG4On.png");
            }
            else if (account.Profile_Theme == "P5")
            {
                embed.WithColor(213, 27, 4);
                embed.WithThumbnailUrl("https://i.imgur.com/bVSsGsA.png");
            }

            // Create different descriptions depending on whether or not there are titles in the user's content filter.
            if (filter_text == "")
            {
                embed.WithDescription("No titles are currently being filtered out.");
            }
            else
            {
                embed.WithDescription("" +
                    "The following titles will be filtered out:\n" +
                    "\n" +
                    $"{filter_text}");
            };

            Global.ContentFilterList.Remove(filterSession);

            menuSession.CurrentMenu = "Content_Filter_Confirm";

            var component = new ComponentBuilder()
                .WithButton("💠 Profile Settings", customId: "profile-settings", ButtonStyle.Secondary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }
    }
}
