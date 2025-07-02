using Discord.Rest;
using Discord.WebSocket;
using Discord;
using SocialLinker.Config;
using SocialLinker.Core.CloudStorageTables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace SocialLinker.Core.Menus.Settings.Main.SceneMaker.CalendarCycles
{
    class Calendar_Cycles_Menu
    {
        public static async Task Calendar_Cycles_Main(SocketGuildUser user, RestUserMessage message)
        {
            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            // Find the menu session associated with the current user.
            var menuSession = Global.MenuIdList.SingleOrDefault(x => x.User.Id == user.Id);

            // Find a filter session associated with the current user.
            var filterSession = Global.ContentFilterList.SingleOrDefault(x => x.User.Id == user.Id);

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

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Calendar Cycles",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            var footer = new EmbedFooterBuilder
            {
                Text = "↩️ Scene Maker Settings"
            };

            embed.WithFooter(footer);

            // Determine the color and thumbnail for the embeded message.
            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Profile_Config_Thumbnail(account));

            embed.WithDescription("" +
                "Choose a title with a changeable cycle to edit.\n" +
                "\n" +
                $"⚙️ **Currently Edited Cycles:**\n" +
                $"\n" +
                $"{filter_text}" +
                "\n" +
                "<:P1:751133115531133112> **Persona**\n" +
                "Edit moon phases.\n" +
                "\n" +
                "<:P3:751133114918633483> **Persona 3**\n" +
                "Edit date, time of day, and moon phases.\n" +
                "\n" +
                "<:P4:751133120530612274> **Persona 4**\n" +
                "Edit date, time of day, and weather.\n" +
                "\n" +
                "<:P5:751133123861020742> **Persona 5**\n" +
                "Edit date, time of day, and weather.\n" +
                "\n" +
                "<:P5S:852644176188669972> **Persona 5 Strikers**\n" +
                "Edit date and time of day.\n");

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
            menuSession.CurrentMenu = "Calendar_Cycles_Main";
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
            reaction_list.Add(Emote.Parse("<:P3:751133114918633483>"));
            reaction_list.Add(Emote.Parse("<:P4:751133120530612274>"));
            reaction_list.Add(Emote.Parse("<:P5:751133123861020742>"));
            reaction_list.Add(Emote.Parse("<:P5S:852644176188669972>"));

            // Add the reactions to the message.
            _ = ReactionHandling.AddReactionsToMenu(message, reaction_list);
        }

        private static async void MenuTimer_Elapsed(object sender, ElapsedEventArgs e, MenuIdStructure menuSession)
        {
            // Search for a content filter list that corresponds to the user's ID.
            var filterSession = Global.ContentFilterList.SingleOrDefault(x => x.User.Id == menuSession.User.Id);

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

                // Remove the content filter entry from the global list.
                Global.ContentFilterList.Remove(filterSession);

                return;
            }

            // Remove the menu entry from the global list.
            Global.MenuIdList.Remove(menuSession);

            // Remove the content filter entry from the global list.
            Global.ContentFilterList.Remove(filterSession);
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

            embed.WithDescription($"You can edit your content settings at any time from the **`settings`** menu by choosing [Profile Settings] > [Content Filter].");
            return embed;
        }
    }
}
