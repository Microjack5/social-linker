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

namespace SocialLinker.Core.Menus.Settings.Main.SceneMaker.DisplayNames
{
    internal class Display_Names_Edit_Menu
    {
        public static async Task Display_Names_Edit_Main(SocketGuildUser user, RestUserMessage message, int item_menu_index)
        {
            // Get the account information of the command's target
            var account = UserInfoClasses.GetAccount(user);

            // Find both the menu session and item session associated with the current user and store them in variables.
            var menuSession = Global.MenuIdList.SingleOrDefault(x => x.User.Id == user.Id);
            var itemSession = Global.ItemIdList.SingleOrDefault(x => x.User.Id == user.Id);

            var name_info = GetNameInfo(itemSession, item_menu_index);
            itemSession.SelectedDisplayName = name_info;
            itemSession.CurrentMenuItem = item_menu_index;
            OfficialSetData current_set_data = OfficialSetMethods.Search_By_Title_And_ID(name_info.Game, name_info.Character_ID);

            // Create a new embed that will be displayed in the message.
            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Edit Display Names",
                IconUrl = user.GetAvatarUrl()
            };

            var footer = new EmbedFooterBuilder
            {
                Text = "↩️ Back"
            };

            embed.WithAuthor(author);
            embed.WithFooter(footer);

            embed.WithDescription("**What would you like to do?**\n" +
                $"\n" +
                $"**Display Name:** `{name_info.Display_Name}`\n" +
                $"**Character:** `{current_set_data.Name}`\n" +
                $"**Game:** `{name_info.Game}`\n" +
                $"**Sprite Numbers Affected:** `{DisplayNameLogging.String_Range_To_Int_Range(account, current_set_data, DisplayNameLogging.String_To_String_List(name_info.Sprites_Affected), name_info)}`\n" +
                $"**Spriteless Affected:** `{name_info.Spriteless_Included}`\n" +
                $"\n" +
                $":one: Delete Display Name\n");

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
            menuSession.CurrentMenu = "Display_Names_Edit_Main";
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

            // Add the reactions to the message.
            _ = ReactionHandling.AddReactionsToMenu(message, reaction_list);
        }

        public static async Task Display_Names_Delete_Confirmation(SocketGuildUser user, RestUserMessage message, int item_menu_index)
        {
            // Get the account information of the command's target
            var account = UserInfoClasses.GetAccount(user);

            // Find both the menu session and item session associated with the current user and store them in variables.
            var menuSession = Global.MenuIdList.SingleOrDefault(x => x.User.Id == user.Id);
            var itemSession = Global.ItemIdList.SingleOrDefault(x => x.User.Id == user.Id);

            var name_info = itemSession.SelectedDisplayName;
            OfficialSetData current_set_data = OfficialSetMethods.Search_By_Title_And_ID(name_info.Game, name_info.Character_ID);

            // Create a new embed that will be displayed in the message.
            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Confirm Deletion",
                IconUrl = user.GetAvatarUrl()
            };

            var footer = new EmbedFooterBuilder
            {
                Text = "↩️ Back | ✅ Confirm"
            };

            embed.WithAuthor(author);
            embed.WithFooter(footer);

            embed.WithDescription("**Are you sure you want to delete this custom display name?**\n" +
                $"\n" +
                $"**Display Name:** `{name_info.Display_Name}`\n" +
                $"**Character:** `{current_set_data.Name}`\n" +
                $"**Game:** `{name_info.Game}`\n" +
                $"**Sprite Numbers Affected:** `{DisplayNameLogging.String_Range_To_Int_Range(account, current_set_data, DisplayNameLogging.String_To_String_List(name_info.Sprites_Affected), name_info)}`\n" +
                $"**Spriteless Affected:** `{name_info.Spriteless_Included}`\n");

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
            menuSession.CurrentMenu = "Display_Names_Delete_Confirmation";
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
            reaction_list.Add(new Emoji("✅"));

            // Add the reactions to the message.
            _ = ReactionHandling.AddReactionsToMenu(message, reaction_list);
        }

        // Utility
        public static DisplayNameTableData GetNameInfo(ItemListIterator itemSession, int item_menu_index)
        {
            var result = itemSession.DisplayNameItemList[item_menu_index];

            return result;
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
