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
        public static async Task Profile_Theme_Main(SocketGuildUser user, RestUserMessage message)
        {
            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            // Create a list variable containing the content filter of the command user.
            List<string> user_filter = ContentFilterMethods.ParseContentFilter(account);

            // Find the menu session associated with the current user.
            var menuSession = Global.MenuIdList.SingleOrDefault(x => x.User.Id == user.Id);

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

            var footer = new EmbedFooterBuilder
            {
                Text = "↩️ Return to Profile Settings"
            };

            embed.WithFooter(footer);

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

            // Based on the user's content filter, append possible profile theme choices to the end of the description text.
            if (p3_filter_check == false)
            {
                description_text += "<:P3:751133114918633483> **Persona 3**\n";
            }

            if (p4_filter_check == false)
            {
                description_text += "<:P4:751133120530612274> **Persona 4**\n";
            }

            if (p5_filter_check == false)
            {
                description_text += "<:P5:751133123861020742> **Persona 5**\n";
            }

            // If all versions for all profile themes are filtered out, replace the default description text and add a footer.
            if (p3_filter_check == true && p4_filter_check == true && p5_filter_check == true)
            {
                description_text = "" +
                    "Profile themes based on certain Persona titles can be chosen to customize your experience.\n" +
                    "\n" +
                    "The appearance of your commands will change based on each one, and you can switch to a different profile theme at any time.\n" +
                    "\n" +
                    ":warning: No profile themes can be chosen due to your content filter. " +
                    $"You can edit your content filter at any time from the **`{BotConfig.bot.cmdPrefix}settings`** menu by choosing [General Settings] > [Content Filter].";
            }

            // Add the description text to the embeded message.
            embed.WithDescription(description_text);

            // Attempt editing the message if it hasn't been deleted by the user yet.
            // If it has, catch the exception, remove the menu entry from the global list, and return.
            try
            {
                // Remove all reactions from the current message.
                await message.RemoveAllReactionsAsync();

                // Edit the current active message by replacing it with the recently created embed.
                await message.ModifyAsync(x => {
                    x.Embed = embed.Build();
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                // Remove the menu entry from the global list.
                Global.MenuIdList.Remove(menuSession);

                return;
            }

            // Edit the menu session according to the current message.
            menuSession.CurrentMenu = "Profile_Theme_Main";
            menuSession.MenuTimer = new Timer()
            {
                // Create a timer that expires as a "time out" duration for the user.
                Interval = MenuConfig.menu.timerDuration,
                AutoReset = false,
                Enabled = true
            };

            // If the menu timer runs out, activate a function.
            menuSession.MenuTimer.Elapsed += (sender, e) => MenuTimer_Elapsed(sender, e, menuSession);

            // Create an empty list for reactions.
            List<IEmote> reaction_list = new List<IEmote> { };

            // Depending on the user's content filter settings, add needed emote reactions to the menu.
            reaction_list.Add(new Emoji("↩️"));

            if (p3_filter_check == false)
            {
                reaction_list.Add(Emote.Parse("<:P3:751133114918633483>"));
            }

            if (p4_filter_check == false)
            {
                reaction_list.Add(Emote.Parse("<:P4:751133120530612274>"));
            }

            if (p5_filter_check == false)
            {
                reaction_list.Add(Emote.Parse("<:P5:751133123861020742>"));
            }

            // Add the reactions to the message.
            _ = ReactionHandling.AddReactionsToMenu(message, reaction_list);
        }

        public static async Task Profile_Theme_Confirm(SocketGuildUser user, RestUserMessage message)
        {
            // Get the account information of the command's target
            var account = UserInfoClasses.GetAccount(user);

            // Find the menu session associated with the current user.
            var menuSession = Global.MenuIdList.SingleOrDefault(x => x.User.Id == user.Id);

            // Create a new embed that will be displayed in the message.
            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Settings Saved",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            // Create an empty string variable.
            string game_title = "";
            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));

            // Assign a value to the string and determine the color and thumbnail for the embeded message.
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

            var footer = new EmbedFooterBuilder
            {
                Text = "💠 Return to Profile Settings | ❌ Close Menu"
            };

            // Add the footer to the embed.
            embed.WithFooter(footer);

            // Attempt editing the message if it hasn't been deleted by the user yet. If it has, catch the exception, send an error message, and return.
            try
            {
                // Remove all reactions from the current message.
                await message.RemoveAllReactionsAsync();

                // Edit the current active message by replacing it with the recently created embed.
                await message.ModifyAsync(x => {
                    x.Embed = embed.Build();
                });
            }
            catch (Exception ex)
            {
                await ErrorHandling.MissingMessageError((SocketTextChannel)message.Channel);
                Console.WriteLine(ex);
                return;
            }

            // Edit the menu session according to the current message.
            menuSession.CurrentMenu = "Profile_Theme_Confirm";
            menuSession.MenuTimer = new Timer()
            {
                // Create a timer that expires as a "time out" duration for the user.
                Interval = MenuConfig.menu.timerDuration,
                AutoReset = false,
                Enabled = true
            };

            // If the menu timer runs out, activate a function.
            menuSession.MenuTimer.Elapsed += (sender, e) => MenuTimer_Elapsed(sender, e, menuSession);

            // Create an empty list for reactions.
            List<IEmote> reaction_list = new List<IEmote> { };

            // Add needed emote reactions for the menu.
            reaction_list.Add(new Emoji("💠"));
            reaction_list.Add(new Emoji("❌"));

            // Add the reactions to the message.
            _ = ReactionHandling.AddReactionsToMenu(message, reaction_list);
        }

        private static async void MenuTimer_Elapsed(object sender, ElapsedEventArgs e, MenuIdStructure menuSession)
        {
            // Assign the menu session's message to another variable.
            var message = menuSession.MenuMessage;

            // Attempt editing the message if it hasn't been deleted by the user yet.
            // If it has, catch the exception, remove the menu entry from the global list, and return.
            try
            {
                // Remove all reactions from the current message.
                await message.RemoveAllReactionsAsync();

                // Edit the current active message by replacing it with the recently created embed.
                await message.ModifyAsync(x => {
                    x.Embed = MenuTimedOut(menuSession.User).Build();
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                // Remove the menu entry from the global list.
                Global.MenuIdList.Remove(menuSession);

                return;
            }

            // Remove the menu entry from the global list.
            Global.MenuIdList.Remove(menuSession);
        }

        public static EmbedBuilder MenuTimedOut(SocketGuildUser user)
        {
            // Get the account information of the command's target
            var account = UserInfoClasses.GetAccount(user);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Inactive Menu",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            // Determine the color and thumbnail for the embeded message
            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Profile_Help_Thumbnail(account));

            embed.WithDescription($"You can set your profile theme at any time from the **`{BotConfig.bot.cmdPrefix}settings`** menu by choosing [Profile Settings] > [Profile Theme].");
            return embed;
        }
    }
}
