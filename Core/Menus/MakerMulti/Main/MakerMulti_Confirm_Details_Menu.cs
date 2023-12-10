using Discord.Rest;
using Discord.WebSocket;
using Discord;
using SocialLinker.Config;
using SocialLinker.Core.CloudStorageTables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace SocialLinker.Core.Menus.MakerMulti.Main
{
    class MakerMulti_Confirm_Details_Menu
    {
        public static async Task MakerMulti_Confirm_Details_Main(SocketGuildUser user, RestUserMessage message)
        {
            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            // Find the menu and item sessions associated with the current user.
            var menu_session = Global.MenuIdList.SingleOrDefault(x => x.User.Id == user.Id);
            var multimaker_session = Global.MultiMaker_Session_List.SingleOrDefault(x => x.User.Id == menu_session.User.Id);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Confirm Details",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            var footer = new EmbedFooterBuilder
            {
                Text = "↩️ Return to Previous Menu | ✅ Continue"
            };

            embed.WithFooter(footer);

            // Determine the color and thumbnail for the embeded message
            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Profile_Config_Thumbnail(account));

            string results_list = "";

            results_list += "" +
                "Are the details correct? Upload an image as a background or select ✅ to create your scene.\n" +
                "\n" +
                $"**Game Style:** {multimaker_session.MakerMultiCommand.Template}\n" +
                $"**Character #1:** {multimaker_session.MakerMultiCommand.Character_Data_1.Bustup_Data.Default_Name_EN}\n" +
                $"**Character #2:** {multimaker_session.MakerMultiCommand.Character_Data_2.Bustup_Data.Default_Name_EN}\n";

            if (multimaker_session.MakerMultiCommand.Character_Data_3.Character_Keyword != default)
            {
                results_list += $"**Character #3:** {multimaker_session.MakerMultiCommand.Character_Data_3.Bustup_Data.Default_Name_EN}\n";
            }
            if (multimaker_session.MakerMultiCommand.Character_Data_4.Character_Keyword != default)
            {
                results_list += $"**Character #4:** {multimaker_session.MakerMultiCommand.Character_Data_4.Bustup_Data.Default_Name_EN}\n";
            }

            results_list += "" +
                $"**Display Name:** {multimaker_session.MakerMultiCommand.Display_Name}\n" +
                $"**Dialogue:** {multimaker_session.MakerMultiCommand.Dialogue}\n";

            embed.WithDescription(results_list);

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
                await message.DeleteAsync();
                await ErrorHandling.PermissionCheck(message);

                // Remove the menu entry from the global list.
                Global.MenuIdList.Remove(menu_session);

                return;
            }

            // Edit the menu session according to the current message.
            menu_session.CurrentMenu = "MakerMulti_Confirm_Details_Main";
            menu_session.MenuTimer = new Timer()
            {
                // Create a timer that expires as a "time out" duration for the user.
                Interval = MenuConfig.menu.timerDuration,
                AutoReset = false,
                Enabled = true
            };

            // If the menu timer runs out, activate a function.
            menu_session.MenuTimer.Elapsed += (sender, e) => MenuTimer_Elapsed(sender, e, menu_session);

            // Create an empty list for reactions.
            List<IEmote> reaction_list = new List<IEmote> { };

            // Add needed emote reactions for the menu.
            reaction_list.Add(new Emoji("↩️"));
            reaction_list.Add(new Emoji("✅"));

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

            embed.WithDescription($"You can create scene maker images with two or more characters at any time with the **`MakerMulti`** command.");
            return embed;
        }
    }
}
