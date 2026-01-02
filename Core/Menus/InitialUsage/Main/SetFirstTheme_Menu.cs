using System;
using System.Collections.Generic;
using System.Timers;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using SocialLinker.Config;
using SocialLinker.Core.CloudStorageTables;
using Discord.Rest;

namespace SocialLinker.Core.Menus.InitialUsage.Main
{
    class SetFirstTheme_Menu
    {
        public static async Task SetFirstThemeMain(SocketTextChannel channel, SocketGuildUser user)
        {
            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

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
                Name = "Setting a Theme",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            embed.WithThumbnailUrl("https://i.imgur.com/DJyqN5w.png");

            // Create a default string to be used as the description's text. This can change depending on the circumstances.
            string description_text = "" +
                "Set your profile theme by reacting to one of the icons below.\n" +
                "\n" +
                "The appearance of your commands will change based on each one, and you can switch to a different profile theme at any time.\n\n";

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
            // Else, if at least one title is completely filtered out but some are still remaining, add on to the end of the default description text.
            else if (p3_filter_check == true || p4_filter_check == true || p5_filter_check == true)
            {
                description_text += ":warning: Some options are unavailable due to your content filter.";
            }

            // Add the description text to the embeded message.
            embed.WithDescription(description_text);

            // Create a null variable for the message.
            RestUserMessage message = null;

            // Try to send a message to the channel. If the bot lacks permissions, catch the exception and return.
            try
            {
                message = await channel.SendMessageAsync("", false, embed.Build());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return;
            }

            // Create a new menu identifier entry for this current message and user to keep track of the overall menu status.
            var menuSession = new MenuIdStructure()
            {
                User = user,
                Account = account,
                MenuMessage = message,
                CurrentMenu = "Set_First_Theme_Main",
                MenuTimer = new Timer()
                {
                    // Create a timer that expires as a "time out" duration for the user.
                    Interval = MenuConfig.menu.timerDuration,
                    AutoReset = false,
                    Enabled = true
                }
            };

            // Add the menu entry to the global list.
            Global.MenuIdList.Add(menuSession);

            var component = new ComponentBuilder();

            // Depending on the user's content filter settings, add needed emote reactions to the menu.
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

            if (p3_filter_check == true && p4_filter_check == true && p5_filter_check == true)
            {
                component.WithButton("❌ Close", customId: "close", ButtonStyle.Secondary);
            }

            await Utility.CleanMessage(menuSession, embed, component, true);
            Utility.NewTimer(menuSession);
        }

        public static async Task SetFirstThemeConfirm(MenuIdStructure menuSession)
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

            // Determine the color and thumbnail for the embeded message.
            if (account.Profile_Theme == "P3")
            {
                game_title = "Persona 3";
                embed.WithThumbnailUrl("https://i.imgur.com/trtPflx.png");
            }
            else if (account.Profile_Theme == "P4")
            {
                game_title = "Persona 4";
                embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P4-PS2"));
            }
            else if (account.Profile_Theme == "P5")
            {
                game_title = "Persona 5";
                embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P5-PS4"));
            }

            embed.WithDescription($"" +
                $"Your profile theme has been set to `{game_title}`.\n\n" +
                $"You can change your profile theme at any time from the **`settings`** menu by choosing [Profile Theme Settings].");

            menuSession.CurrentMenu = "Set_First_Theme_Confirm";

            var component = new ComponentBuilder()
                .WithButton("❌ Close", customId: "close", ButtonStyle.Secondary);

            await Utility.CleanMessage(menuSession, embed, component, true);
            Utility.NewTimer(menuSession);
        }
    }
}
