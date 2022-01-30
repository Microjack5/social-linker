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

namespace SocialLinker.Core.Menus.Help.Main
{
    class SM_Tutorial_Dual_Menu
    {
        public static async Task SM_Tutorial_Dual_Chara_Page_1(SocketGuildUser user, RestUserMessage message)
        {
            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            // Find the menu session associated with the current user.
            var menuSession = Global.MenuIdList.SingleOrDefault(x => x.User.Id == user.Id);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Dual Character Scenes",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            var footer = new EmbedFooterBuilder
            {
                Text = "" +
                "↩️ Return to Tips & Tricks Menu | ▶️ Next Page\n" +
                "Page 1 / 2"
            };

            embed.WithFooter(footer);

            // Determine the color and thumbnail for the embeded message
            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Profile_Help_Thumbnail(account));

            embed.WithDescription("" +
                $"Type **`{BotConfig.bot.cmdPrefix}maker dual`** to start creating a scene with two characters. This is only possible with a select few templates, so choose the one you think fits best and follow the instructions provided.");

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
            menuSession.CurrentMenu = "SM_Tutorial_Dual_Chara_Page_1";
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
            reaction_list.Add(new Emoji("▶️"));

            // Add the reactions to the message.
            _ = ReactionHandling.AddReactionsToMenu(message, reaction_list);
        }

        public static async Task SM_Tutorial_Dual_Chara_Page_2(SocketGuildUser user, RestUserMessage message)
        {
            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            // Find the menu session associated with the current user.
            var menuSession = Global.MenuIdList.SingleOrDefault(x => x.User.Id == user.Id);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Forcing Character Positions",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            var footer = new EmbedFooterBuilder
            {
                Text = "" +
                "◀️ Previous Page | 💠 Return to Tips & Tricks Menu\n" +
                "Page 2 / 2"
            };

            embed.WithFooter(footer);

            // Determine the color and thumbnail for the embeded message
            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Profile_Help_Thumbnail(account));

            embed.WithDescription("" +
                "By setting one of the character’s sprite numbers to `0`, you can force the other character to appear on a side they normally wouldn’t be on alone.");

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
            menuSession.CurrentMenu = "SM_Tutorial_Dual_Chara_Page_2";
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
            reaction_list.Add(new Emoji("◀️"));
            reaction_list.Add(new Emoji("💠"));

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

            embed.WithDescription($"You can view the list of scene maker tutorials at any time from the **`{BotConfig.bot.cmdPrefix}help`** menu by choosing [Scene Maker].");
            return embed;
        }
    }
}
