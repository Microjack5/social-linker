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
    class Star_Level_Menu
    {
        public static async Task Star_Level_Main(SocketGuildUser user, RestUserMessage message)
        {
            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            // Find the menu session associated with the current user.
            var menuSession = Global.MenuIdList.SingleOrDefault(x => x.User.Id == user.Id);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Star Level",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            var footer = new EmbedFooterBuilder
            {
                Text = "↩️ Back | ✅ Confirm"
            };

            embed.WithFooter(footer);

            // Determine the color and thumbnail for the embeded message
            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Profile_Config_Thumbnail(account));

            string description_text = "";

            if (account.Level_Resets == 0)
            {
                description_text = "" +
                    "Star Level is a special rank that lets you keep earning P-Medals past Level 99!\n" +
                    "Your profile will be reset to Level 1, but a signature mark will appear on your status screen and all your social stats stay intact for P-Medal bonuses.\n" +
                    "\n" +
                    "Would you like to reach to Star Level Rank 1?";
            }
            else if (account.Level_Resets == 1)
            {
                description_text = "" +
                    "You've maxed out your level twice! This is the final stretch...\n" +
                    "You can reach Star Level Rank 2 by resetting your level again while gaining another star mark and keeping your social stats intact.\n" +
                    "\n" +
                    "Would you like to reach Star Level Rank 2?";
            }
            else if (account.Level_Resets == 2)
            {
                description_text = "" +
                    "You've made it! This is the end goal of Social Linker.\n" +
                    "This time, there are no more level resets. All levels and social stats remain completely intact going forward.\n" +
                    "\n" +
                    "Would you like to reach Star Level Rank 3?";
            }

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
            menuSession.CurrentMenu = "Star_Level_Main";
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
            reaction_list.Add(new Emoji("✅"));

            // Add the reactions to the message.
            _ = ReactionHandling.AddReactionsToMenu(message, reaction_list);
        }

        public static async Task Star_Level_Check(SocketGuildUser user, RestUserMessage message)
        {
            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            // Find the menu session associated with the current user.
            var menuSession = Global.MenuIdList.SingleOrDefault(x => x.User.Id == user.Id);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Are You Sure?",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            // Create an empty string variable. This will store part of the footer's text.
            string confirm_text = "";

            // Add a specific star icon to the confirm_text variable depending on how many times the user's level has been reset.
            if (account.Level == 99 && account.Level_Resets == 0)
            {
                confirm_text = "⭐ Confirm";
            }
            else if (account.Level == 99 && account.Level_Resets == 1)
            {
                confirm_text = "🌟 Confirm";
            }
            else if (account.Level == 99 && account.Level_Resets == 2)
            {
                confirm_text = "✨ Confirm";
            }

            // Create and add the footer to the embeded message.
            var footer = new EmbedFooterBuilder
            {
                Text = $"💠 Return to Profile Settings Menu | {confirm_text}"
            };

            embed.WithFooter(footer);

            // Determine the color and thumbnail for the embeded message
            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Profile_Config_Thumbnail(account));

            if (account.Level_Resets == 0)
            {
                embed.WithDescription($"Are you sure you want to gain Star Level rank and reset your level? This cannot be undone.");
            }
            else if (account.Level_Resets == 1)
            {
                embed.WithDescription($"Are you sure you want to gain another Star Level rank and reset your level? This cannot be undone.");
            }
            else if (account.Level_Resets == 2)
            {
                embed.WithDescription($"Are you sure? No turning back!");
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
            menuSession.CurrentMenu = "Star_Level_Check";
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

            // Add a star reaction to the menu depending on how many times the user's level has been reset.
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

        public static async Task Star_Level_Confirm(SocketGuildUser user, RestUserMessage message)
        {
            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            // Find the menu session associated with the current user.
            var menuSession = Global.MenuIdList.SingleOrDefault(x => x.User.Id == user.Id);

            var embed = new EmbedBuilder();

            string header_text = "";
            string description_text = "";

            if (account.Level_Resets == 1)
            {
                header_text = "Welcome to Star Level!";
                description_text = "" +
                    "Your Star Level is now at Rank 1! A rank above the rest!\n" +
                    "\n" +
                    "There are three possible ranks you can reach in Star Level. Maybe a special surprise awaits at the final one...?\n" +
                    "\n" +
                    "Only one way to find out! Go out and show off your new rank with pride!";
            }
            else if (account.Level_Resets == 2)
            {
                header_text = "Star Level Rank Up!!";
                description_text = "" +
                    "Your Star Level is now at Rank 2! You've come an amazingly long way. Congratulations!";
            }
            else if (account.Level_Resets == 3)
            {
                header_text = "STAR LEVEL RANK MAX!!";
                description_text = "" +
                    "You've completely maxed out your Star Level! There are no other greater accomplishments past this point.\n" +
                    "\n" +
                    "As a special gift, all décor in the Décor Shop are now completely free for you from this point onwards. Thank you for being active and using Social Linker so much over the past few years!";
            }

            var author = new EmbedAuthorBuilder
            {
                Name = header_text,
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            var footer = new EmbedFooterBuilder
            {
                Text = "💠 Return to Profile Settings Menu | ❌ Close Menu"
            };

            embed.WithFooter(footer);

            // Determine the color and thumbnail for the embeded message
            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Profile_Config_Thumbnail(account));

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
            menuSession.CurrentMenu = "Star_Level_Confirm";
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

            embed.WithDescription($"You can set your profile theme at any time from the **`{BotConfig.bot.cmdPrefix}settings`** menu by choosing [Profile Settings] > [Star Level].");
            return embed;
        }
    }
}
