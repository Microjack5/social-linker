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
    class Profile_Settings_Menu
    {
        public static async Task Profile_Settings_Main(SocketGuildUser user, RestUserMessage message)
        {
            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            // Find the menu session associated with the current user.
            var menuSession = Global.MenuIdList.SingleOrDefault(x => x.User.Id == user.Id);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Profile Settings",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            var footer = new EmbedFooterBuilder
            {
                Text = "↩️ Return to Settings Main Menu"
            };

            embed.WithFooter(footer);

            // Determine the color and thumbnail for the embeded message
            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Profile_Config_Thumbnail(account));

            embed.AddField(":one: Profile Theme",
                "Adjust profile theme settings.");
            embed.AddField(":two: Status Screen Décor",
                "Adjust status screen décor settings.");
            embed.AddField(":three: Time Zone & Weather",
                "Adjust time zone and weather settings.");
            embed.AddField(":four: Level Up Notifications",
                "Adjust level up notification settings.");
            embed.AddField(":five: Rank Up Notifications",
                "Adjust rank up notification settings.");
            embed.AddField(":six: Content Filter",
                "Adjust content filter settings.");

            // If the user is at level 99 and has reset their level less than three times, add a "Star Level" option to the menu.
            if (account.Level == 99 && account.Level_Resets == 0)
            {
                embed.AddField(":star: Star Level",
                "Reach a new profile rank and continue earning P-Medals.");
            }
            else if (account.Level == 99 && account.Level_Resets == 1)
            {
                embed.AddField(":star2: Star Level",
                "Reach Star Level Rank 2 and continue earning P-Medals!");
            }
            else if (account.Level == 99 && account.Level_Resets == 2)
            {
                embed.AddField(":sparkles: Star Level",
                "Reach the final Star Level rank!");
            }

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
            menuSession.CurrentMenu = "Profile_Settings_Menu";
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
            reaction_list.Add(new Emoji("↩️"));
            reaction_list.Add(new Emoji("\u0031\ufe0f\u20e3"));
            reaction_list.Add(new Emoji("\u0032\ufe0f\u20e3"));
            reaction_list.Add(new Emoji("\u0033\ufe0f\u20e3"));
            reaction_list.Add(new Emoji("\u0034\ufe0f\u20e3"));
            reaction_list.Add(new Emoji("\u0035\ufe0f\u20e3"));
            reaction_list.Add(new Emoji("\u0036\ufe0f\u20e3"));

            // If the user is at level 99, add a star reaction to the menu depending on how many times they reset their level.
            if (account.Level == 99 && account.Level_Resets == 0)
            {
                reaction_list.Add(new Emoji("⭐"));
            }
            else if (account.Level == 99 && account.Level_Resets == 1)
            {
                reaction_list.Add(new Emoji("🌟"));
            }
            else if (account.Level == 99 && account.Level_Resets == 2)
            {
                reaction_list.Add(new Emoji("✨"));
            }

            // Add the reactions to the message.
            _ = ReactionHandling.AddReactionsToMenu(message, reaction_list);
        }

        public static async Task Content_Filter_Main(SocketGuildUser user, RestUserMessage message)
        {
            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            // Find the menu session associated with the current user.
            var menuSession = Global.MenuIdList.SingleOrDefault(x => x.User.Id == user.Id);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Content Filter",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            var footer = new EmbedFooterBuilder
            {
                Text = "↩️ Return to General Settings Menu"
            };

            embed.WithFooter(footer);

            // Determine the color and thumbnail for the embeded message
            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Profile_Config_Thumbnail(account));

            embed.WithDescription("" +
                "Choose to hide content related to certain titles. Select the titles you wish to filter out and end with ✅ to change these settings.\n" +
                "\n" +
                $"⚙️ **Current Setting:** `{account.Content_Filter}`\n");

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
            menuSession.CurrentMenu = "Content_Filter_Main";
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
            reaction_list.Add(new Emoji("↩️"));
            reaction_list.Add(Emote.Parse("<:P1:751133115531133112>"));
            reaction_list.Add(Emote.Parse("<:P2IS:788950080396328990>"));
            reaction_list.Add(Emote.Parse("<:P2EP:788950163363463172>"));
            reaction_list.Add(Emote.Parse("<:P3:751133114918633483>"));
            reaction_list.Add(Emote.Parse("<:P4:751133120530612274>"));
            reaction_list.Add(Emote.Parse("<:P4AU:751133122342420572>"));
            reaction_list.Add(Emote.Parse("<:P4D:751133120346062859>"));
            reaction_list.Add(Emote.Parse("<:P5:751133123861020742>"));
            reaction_list.Add(Emote.Parse("<:BBTAG:751133123013771617>"));

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

            embed.WithDescription($"You can view and change your user settings at any time with the **`settings`** command.");
            return embed;
        }
    }
}
