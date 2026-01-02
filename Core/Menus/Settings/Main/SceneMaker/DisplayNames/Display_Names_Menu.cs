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
using SocialLinker.Core.Menus.Settings.Reactions.SceneMaker.DisplayNames;

namespace SocialLinker.Core.Menus.Settings.Main.SceneMaker.DisplayNames
{
    internal class Display_Names_Menu
    {
        public static async Task Display_Names_Start(MenuIdStructure menuSession)
        {
            try
            {
                var account = menuSession.Account;
                var user = menuSession.User;
                var message = menuSession.MenuMessage;

                var embed = new EmbedBuilder();
                var author = new EmbedAuthorBuilder
                {
                    Name = "Now Loading...",
                    IconUrl = user.GetAvatarUrl()
                };

                embed.WithAuthor(author);

                // Determine the color and thumbnail for the embeded message.
                embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));

                // Search for an item list that corresponds to the user's ID.
                var itemSession = Global.ItemIdList.SingleOrDefault(x => x.User.Id == user.Id);

                // If an item session already exists, remove it from the global list.
                if (itemSession != null)
                {
                    Global.ItemIdList.Remove(itemSession);
                }

                // Create a new item identifier entry for this current user to keep track of the position of décor on the menu.
                itemSession = new ItemListIterator()
                {
                    User = user,
                    DisplayNameItemList = Display_Names_Sort_Reactions.CreateSortSettingList(DisplayNameLogging.GetCustomNameList(user.Id), account.Display_Names_Sort),
                    ItemIndexBase = 0,
                    MaxItemsDisplayed = 5,
                    CurrentPage = 1
                };

                // Add the item entry to the global list.
                Global.ItemIdList.Add(itemSession);

                var naming_session = Global.DisplayNameTempList.SingleOrDefault(x => x.User_ID == $"{menuSession.User.Id}");

                if (naming_session != null)
                {
                    Global.DisplayNameTempList.Remove(naming_session);
                }

                // Create a new naming session identifier entry for this current session and user to keep track of the overall status.
                naming_session = new DisplayNameInternalData()
                {
                    User_ID = $"{user.Id}",
                };

                // Add the filter session to the global list.
                Global.DisplayNameTempList.Add(naming_session);

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
                menuSession.CurrentMenu = "Display_Names_Start";
                menuSession.MenuTimer = new Timer()
                {
                    // Create a timer that expires as a "time out" duration for the user.
                    Interval = MenuConfig.menu.timerDuration,
                    AutoReset = false,
                    Enabled = true
                };

                // Create a new menu in the current channel.
                await Display_Names_Main(menuSession);
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static async Task Display_Names_Main(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var itemSession = Global.ItemIdList.SingleOrDefault(x => x.User.Id == user.Id);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Display Names",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            // Determine the color and thumbnail for the embeded message.
            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Profile_Config_Thumbnail(account));

            // Create a string variable to store the text that will be displayed on the message's body.
            string displayed_name_list = "";

            // Create an int variable from the number of items in the list minus the starting index to count from.
            // Since the ItemIndexBase should always initially start at zero, nothing will be subtracted at first but will adjust as the index moves when the page changes.
            int remaining_list_length = itemSession.DisplayNameItemList.Count - itemSession.ItemIndexBase;

            // Create another int variable that will indicate a subset of the item list that the user is currently viewing.
            int sublist_length = 0;

            // If the remaining number of items in the list is greater than or equal to the max amount of items that should be displayed, make the sublist_length int also equal to max_items_displayed.
            if (remaining_list_length >= itemSession.MaxItemsDisplayed)
            {
                sublist_length = itemSession.MaxItemsDisplayed;
            }
            // Else, if the number of remaining items is less than max_items_displayed, make sublist_length equal to the remaining number of items.
            else
            {
                sublist_length = remaining_list_length;
            }

            // Create an int to properly display the needed emotes when iterating through the item list.
            int displayed_list_counter = 0;

            // Iterate through the item list starting from the ItemBaseIndex and up until sublist_length.
            for (int i = itemSession.ItemIndexBase; i < (itemSession.ItemIndexBase + sublist_length); i++)
            {
                // Increase the displayed_list_counter by one.
                displayed_list_counter += 1;

                OfficialSetData current_set_data = OfficialSetMethods.Search_By_Title_And_ID(itemSession.DisplayNameItemList[i].Game, itemSession.DisplayNameItemList[i].Character_ID);

                // Add the entry to the displayed_shop_list string.
                displayed_name_list += $"" +
                    $":{DecorInfoMethods.NumberToWords(displayed_list_counter)}: **Display Name:** {itemSession.DisplayNameItemList[i].Display_Name}\n" +
                    $"**Character:** {current_set_data.Name}\n" +
                    $"**Game:** {itemSession.DisplayNameItemList[i].Game}\n" +
                    $"**Sprite Numbers Affected:** {DisplayNameLogging.String_Range_To_Int_Range(account, current_set_data, DisplayNameLogging.String_To_String_List(itemSession.DisplayNameItemList[i].Sprites_Affected), itemSession.DisplayNameItemList[i])}\n" +
                    $"**Spriteless Affected:** {itemSession.DisplayNameItemList[i].Spriteless_Included}\n" +
                    $"\n";
            }

            // Depending on whether or not the user owns or can set any décor, perform different actions.
            if (displayed_name_list.Length > 0)
            {
                embed.WithDescription($"{displayed_name_list}");
            }
            else
            {
                embed.WithDescription("You don't have any custom display names.");
            }

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
            menuSession.CurrentMenu = "Display_Names_Main";

            var selectMenu = new SelectMenuBuilder()
                    .WithPlaceholder("Select Display Name")
                    .WithCustomId("display-names-main")
                    .WithMinValues(1)
                    .WithMaxValues(1);

            displayed_list_counter = 0;

            for (int i = 0; i < sublist_length; i++)
            {
                displayed_list_counter += 1;
                selectMenu.AddOption($"{DecorInfoMethods.NumberToKeycapEmoji(displayed_list_counter)} {itemSession.DisplayNameItemList[i].Display_Name}", $"{displayed_list_counter}");
            }

            var component = new ComponentBuilder();

            if (sublist_length > 0)
            {
                component.WithSelectMenu(selectMenu);
            }

            component.WithButton("↩️ Scene Maker Settings", customId: "return", ButtonStyle.Secondary);

            // Check if the starting item index is greater than or equal to max_items_displayed.
            if (itemSession.ItemIndexBase >= itemSession.MaxItemsDisplayed)
            {
                // If so, there will be a "Previous Page" button added as a reaction.
                component.WithButton("◀️ Previous Page", customId: "previous-page", ButtonStyle.Secondary);
            }

            // Check if the number of items in the list minus the starting item index is more than max_items_displayed.
            if (remaining_list_length > itemSession.MaxItemsDisplayed)
            {
                // If so, there will be a "Next Page" button added as a reaction.
                component.WithButton("▶️ Next Page", customId: "next-page", ButtonStyle.Secondary);
            }

            component.WithButton("➕ Add New Entry", customId: "add", ButtonStyle.Secondary);

            // If the user owns any décor, add a gear reaction in order to sort entries.
            if (displayed_name_list.Length > 0)
            {
                component.WithButton("⚙️ Sort", customId: "sort", ButtonStyle.Secondary);
            }

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }
    }
}
