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
using SocialLinker.Core.LocalStorageTables;
using System.Drawing;
using System.IO;
using SocialLinker.Core.Menus.Settings.Reactions.SceneMaker.DisplayNames;

namespace SocialLinker.Core.Menus.Settings.Main.SceneMaker.DisplayNames
{
    internal class Display_Names_Sort_Menu
    {
        public static async Task Display_Names_Sort(SocketGuildUser user, RestUserMessage message)
        {
            // Get the account information of the command's target
            var account = UserInfoClasses.GetAccount(user);

            // Find both the menu session and item session associated with the current user and store them in variables.
            var menuSession = Global.MenuIdList.SingleOrDefault(x => x.User.Id == user.Id);
            var itemSession = Global.ItemIdList.SingleOrDefault(x => x.User.Id == user.Id);

            // Create a new embed that will be displayed in the message.
            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Sort Display Names",
                IconUrl = user.GetAvatarUrl()
            };

            var footer = new EmbedFooterBuilder
            {
                Text = "↩️ Back"
            };

            embed.WithAuthor(author);
            embed.WithFooter(footer);

            embed.WithDescription("" +
                "**Choose a method to sort display names entries by.**\n" +
                "\n"+
                $"⚙️ **Current Setting:** **`{Display_Names_Sort_Reactions.SortSettingToString(account.Display_Names_Sort)}`**\n" +
                $"\n" +
                $":one: By Oldest to Newest\n" +
                $":two: By Newest to Oldest\n" +
                $":three: By Display Name (A - Z)\n" +
                $":four: By Display Name (Z - A)\n" +
                $":five: By Title\n");

            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));

            // Attempt deleting the message if it hasn't been deleted by the user yet.
            try
            {
                // Delete the current message from the channel.
                await message.DeleteAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            // If the bot lacks permission to send messages, catch the exception and return.
            try
            {
                // Reassign the menu session's message to a new message generated from the created embed.
                menuSession.MenuMessage = (RestUserMessage)await message.Channel.SendMessageAsync("", false, embed.Build());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return;
            }

            // Set the "message" variable to the menu session's message.
            message = menuSession.MenuMessage;

            // Edit the menu session according to the current message.
            menuSession.CurrentMenu = "Display_Names_Sort";
            menuSession.MenuTimer = new Timer()
            {
                // Create a timer that expires as a "time out" duration for the user.
                Interval = MenuConfig.menu.timerDuration,
                AutoReset = false,
                Enabled = true
            };

            // If the timer runs out, activate a function.
            menuSession.MenuTimer.Elapsed += (sender, e) => MenuTimer_Elapsed(sender, e, menuSession, itemSession);

            // Create an empty list for reactions.
            List<IEmote> reaction_list = new List<IEmote> { };

            // Add needed emote reactions for the menu.
            reaction_list.Add(new Emoji("↩️"));
            reaction_list.Add(new Emoji("\u0031\ufe0f\u20e3"));
            reaction_list.Add(new Emoji("\u0032\ufe0f\u20e3"));
            reaction_list.Add(new Emoji("\u0033\ufe0f\u20e3"));
            reaction_list.Add(new Emoji("\u0034\ufe0f\u20e3"));
            reaction_list.Add(new Emoji("\u0035\ufe0f\u20e3"));

            // Add the reactions to the message.
            _ = ReactionHandling.AddReactionsToMenu(message, reaction_list);
        }

        public static async Task Display_Names_Sort_Confirm(SocketGuildUser user, RestUserMessage message)
        {
            // Get the account information of the command's target
            var account = UserInfoClasses.GetAccount(user);

            // Find both the menu session and item session associated with the current user and store them in variables.
            var menuSession = Global.MenuIdList.SingleOrDefault(x => x.User.Id == user.Id);
            var itemSession = Global.ItemIdList.SingleOrDefault(x => x.User.Id == user.Id);

            // Create a new embed that will be displayed in the message.
            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Settings Saved",
                IconUrl = user.GetAvatarUrl()
            };

            var footer = new EmbedFooterBuilder
            {
                Text = "💠 Return to Display Names Menu | ❌ Close Menu"
            };

            embed.WithAuthor(author);
            embed.WithFooter(footer);

            embed.WithDescription($"Display names will now be sorted **`{Display_Names_Sort_Reactions.SortSettingToString(account.Display_Names_Sort)}`**.");

            // Determine the color and thumbnail for the embeded message.
            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));

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
            menuSession.CurrentMenu = "Display_Names_Sort_Confirm";
            menuSession.MenuTimer = new Timer()
            {
                // Create a timer that expires as a "time out" duration for the user.
                Interval = MenuConfig.menu.timerDuration,
                AutoReset = false,
                Enabled = true
            };

            // If the timer runs out, activate a function.
            menuSession.MenuTimer.Elapsed += (sender, e) => MenuTimer_Elapsed(sender, e, menuSession, itemSession);

            // Create an empty list for reactions.
            List<IEmote> reaction_list = new List<IEmote> { };

            // Add needed emote reactions for the menu.
            reaction_list.Add(new Emoji("💠"));
            reaction_list.Add(new Emoji("❌"));

            // Add the reactions to the message.
            _ = ReactionHandling.AddReactionsToMenu(message, reaction_list);
        }

        private static async void MenuTimer_Elapsed(object sender, ElapsedEventArgs e, MenuIdStructure idTracker, ItemListIterator itemSession)
        {
            // Attempt deleting the message if it hasn't been deleted by the user yet.
            // If it has, catch the exception, remove the menu and item entries from the global list, and return.
            try
            {
                // Delete the current message from the channel.
                await idTracker.MenuMessage.DeleteAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                // Remove the menu and item entries from the global list.
                Global.MenuIdList.Remove(idTracker);
                Global.ItemIdList.Remove(itemSession);

                return;
            }

            // Reassign the menu session's message to a new message generated from the created embed.
            idTracker.MenuMessage = (RestUserMessage)await idTracker.MenuMessage.Channel.SendMessageAsync("", false, MenuTimedOut(idTracker.User).Build());

            // Remove the menu and item entries from the global list
            Global.MenuIdList.Remove(idTracker);
            Global.ItemIdList.Remove(itemSession);
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

            embed.WithDescription($"You can view and add new custom display names at any time from the **`settings`** menu by choosing [Scene Maker Settings] > [Display Names].");
            return embed;
        }

        
    }
}
