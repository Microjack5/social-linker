using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using SocialLinker.Config;
using SocialLinker.Core.CloudStorageTables;
using Discord.Rest;

namespace SocialLinker.Core.Menus.Settings.Main.Profile
{
    class Profile_Theme_Menu
    {
        public static async Task Profile_Theme_Main(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            // Create a list variable containing the content filter of the command user.
            List<string> user_filter = ContentFilterMethods.ParseContentFilter(account);

            // Create bool values for each of the profile themes.
            // These are meant to check whether or not all versions of a title are completely blocked in the user's content filter.
            // False indicates at least one version is allowed, True indicates no versions are allowed. The condition is False by default.
            bool p3_filter_check = false;
            bool p4_filter_check = false;
            bool p5_filter_check = false;

            // If both versions of P3 are blocked in the user's content filter, set p3_filter_check to true.
            if (user_filter.Contains("P3F") == true && user_filter.Contains("P3P") == true)
            {
                p3_filter_check = true;
            }

            // If both versions of P4 are blocked in the user's content filter, set p4_filter_check to true.
            if (user_filter.Contains("P4-PS2") == true && user_filter.Contains("P4G") == true)
            {
                p4_filter_check = true;
            }

            // If both versions of P5 are blocked in the user's content filter, set p5_filter_check to true.
            if (user_filter.Contains("P5-PS4") == true && user_filter.Contains("P5R") == true)
            {
                p5_filter_check = true;
            }

            // Start building the embeded message.
            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Profile Theme",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            // Determine the color and thumbnail for the embeded message.
            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Profile_Config_Thumbnail(account));

            // Create an empty string variable.
            string title = "";

            // Based on the user's profile theme, assign proper titles to the set acronym.
            if (account.Profile_Theme == "P3")
            {
                title = "Persona 3";
            }
            else if (account.Profile_Theme == "P4")
            {
                title = "Persona 4";
            }
            else if (account.Profile_Theme == "P5")
            {
                title = "Persona 5";
            }
            else
            {
                title = "None";
            }

            // Create a default string to be used as the description's text. This can change depending on the circumstances.
            string description_text = "" +
                "Choose a profile theme. The appearance of your commands will change based on each one.\n\n";

            // If at least one title is completely filtered out but some are still remaining, add on to the end of the default description text.
            if (p3_filter_check == true || p4_filter_check == true || p5_filter_check == true)
            {
                description_text += ":warning: Some options are unavailable due to your content filter.\n\n";
            }

            // Append the currently set profile theme to the description text.
            description_text += $"⚙️ **Current Setting:** **`{title}`**\n\n";

            // If all versions for all profile themes are filtered out, replace the default description text and add a footer.
            if (p3_filter_check == true && p4_filter_check == true && p5_filter_check == true)
            {
                description_text = "" +
                    "Profile themes based on certain Persona titles can be chosen to customize your experience.\n" +
                    "\n" +
                    "The appearance of your commands will change based on each one, and you can switch to a different profile theme at any time.\n" +
                    "\n" +
                    ":warning: No profile themes can be chosen due to your content filter. " +
                    $"You can edit your content filter at any time from the **`settings`** menu by choosing [Profile Settings] > [Content Filter].";
            }

            embed.WithDescription(description_text);

            menuSession.CurrentMenu = "Profile_Theme_Main";

            var component = new ComponentBuilder()
                .WithButton("↩️ Return", customId: "return", ButtonStyle.Secondary);

            if (p3_filter_check == false)
            {
                component.WithButton("Persona 3", customId: "p3", ButtonStyle.Secondary, Emote.Parse(Global.GetGameEmote("P3")));
            }

            if (p4_filter_check == false)
            {
                component.WithButton("Persona 4", customId: "p4", ButtonStyle.Secondary, Emote.Parse(Global.GetGameEmote("P4")));
            }

            if (p5_filter_check == false)
            {
                component.WithButton("Persona 5", customId: "p5", ButtonStyle.Secondary, Emote.Parse(Global.GetGameEmote("P5")));
            }

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Profile_Theme_Confirm(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            // Create a new embed that will be displayed in the message.
            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Settings Saved",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            string game_title = "";
            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));

            if (account.Profile_Theme == "P3")
            {
                game_title = "Persona 3";
                embed.WithThumbnailUrl("https://i.imgur.com/trtPflx.png");
            }
            else if (account.Profile_Theme == "P4")
            {
                game_title = "Persona 4";
                embed.WithThumbnailUrl("https://i.imgur.com/8Qs9g1d.png");
            }
            else if (account.Profile_Theme == "P5")
            {
                game_title = "Persona 5";
                embed.WithThumbnailUrl("https://i.imgur.com/1jk1MZw.png");
            }

            embed.WithDescription($"" +
                $"Your profile theme has been set to **`{game_title}`**.");

            menuSession.CurrentMenu = "Profile_Theme_Confirm";

            var component = new ComponentBuilder()
                .WithButton("💠 Return to Profile Settings", customId: "back-to-profile-settings", ButtonStyle.Secondary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }
    }
}
