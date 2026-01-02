using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using SocialLinker.Config;
using SocialLinker.Core.CloudStorageTables;
using SocialLinker.Core.LocalStorageTables;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace SocialLinker.Core.Menus.Shop.Main
{
    public class ShopMenu : ModuleBase<SocketCommandContext>
    {
        public static async Task ShopStart(SocketTextChannel channel, SocketGuildUser user)
        {
            //Get the account information of the command's target
            var account = UserInfoClasses.GetAccount(user);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Now Loading...",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            //Determine color for embeded message
            if (account.Profile_Theme == "P3")
            {
                embed.WithColor(37, 149, 255);
            }
            else if (account.Profile_Theme == "P4")
            {
                embed.WithColor(255, 229, 49);
            }
            else if (account.Profile_Theme == "P5")
            {
                embed.WithColor(213, 27, 4);
            }

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

            // Create a string list variable.
            List<string> original_decor_list;

            // Depending on the user's settings, fill the string list with an assortment of décor according to how the user wishes for it to be organized.
            original_decor_list = DecorInfoMethods.CreateSortSettingList(account.Shop_Sort);

            // Create an empty string list.
            List<string> owned_decor = new List<string> { };

            // Check if the user owns any décor.
            if (account.Decor_Owned != "")
            {
                // If so, convert their Decor_Owned value into a string list and assign it to the owned_decor string list.
                owned_decor = DecorInfoMethods.StringToStringArray(account.Decor_Owned);
            }

            // Start comparing the user's owned_decor list to the created decor_list for the shop.
            // If the user owns any décor from the decor_list or has content blocked, remove the matching entry from the list
            var new_decor_list = original_decor_list.Except(owned_decor).ToList();

            // Create another empty string list.
            List<string> user_content_filter = new List<string> { };
            
            // Check if the user has any titles listed in their content filter.
            if (account.Content_Filter != "")
            {
                // If so, convert their Content_Filter value into a string list and assign it to the user_content_filter string list.
                user_content_filter = DecorInfoMethods.StringToStringArray(account.Content_Filter);
            }

            // Create a new list by removing any décor that contains content specified in the user's content filter and assign it to the new_decor_list variable.
            new_decor_list = DecorInfoMethods.RemoveBlockedContentFromList(new_decor_list, user_content_filter);

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
                ItemList = new_decor_list,
                ItemIndexBase = 0,
                MaxItemsDisplayed = 6,
                CurrentPage = 1
            };

            // Add the item entry to the global list.
            Global.ItemIdList.Add(itemSession);

            // Create a new menu identifier entry for this current message and user to keep track of the overall menu status.
            var idTracker = new MenuIdStructure()
            {
                User = user,
                Account = account,
                MenuMessage = message,
                CurrentMenu = "Shop_Start",
                MenuTimer = new Timer()
                {
                    // Create a timer that expires as a "time out" duration for the user.
                    Interval = MenuConfig.menu.timerDuration,
                    AutoReset = false,
                    Enabled = true
                }
            };

            // Add the menu entry to the global list.
            Global.MenuIdList.Add(idTracker);

            // Create a new menu in the current channel.
            await ShopMainMenu(idTracker.User, idTracker.MenuMessage);
        }

        public static async Task ShopMainMenu(SocketGuildUser user, RestUserMessage message)
        {
            //Get the account information of the command's target
            var account = UserInfoClasses.GetAccount(user);

            // Find both the menu session and item session associated with the current user and store them in variables.
            var menuSession = Global.MenuIdList.SingleOrDefault(x => x.User.Id == user.Id);
            var itemSession = Global.ItemIdList.SingleOrDefault(x => x.User.Id == user.Id);

            var selectMenu = new SelectMenuBuilder()
                    .WithPlaceholder("Select Décor")
                    .WithCustomId("shop-menu-main")
                    .WithMinValues(1)
                    .WithMaxValues(1);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Décor Shop",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            //Determine color for embeded message
            if (account.Profile_Theme == "P3")
            {
                embed.WithColor(37, 149, 255);
            }
            else if (account.Profile_Theme == "P4")
            {
                embed.WithColor(255, 229, 49);
            }
            else if (account.Profile_Theme == "P5")
            {
                embed.WithColor(213, 27, 4);
            }

            embed.AddField("Wallet", $"<:PMedals:672637091171139615> **{account.P_Medals}**");

            // Create a string variable to store the text that will be displayed on the message's body.
            string displayed_shop_list = "";

            // Create an int variable from the number of items in the list minus the starting index to count from.
            // Since the ItemIndexBase should always initially start at zero, nothing will be subtracted at first but will adjust as the index moves when the page changes.
            int remaining_list_length = itemSession.ItemList.Count - itemSession.ItemIndexBase;

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

                // Get the information of the current décor iteration.
                var decor_info = DecorInfoMethods.GetDecorInfo(itemSession.ItemList[i]);

                // Add the entry to the displayed_shop_list string.
                displayed_shop_list += $":{DecorInfoMethods.NumberToWords(displayed_list_counter)}: {decor_info.Title} - <:cost:780352551945895936> **{decor_info.Price}**\n";

                selectMenu.AddOption($"{DecorInfoMethods.NumberToKeycapEmoji(displayed_list_counter)} {decor_info.Title}", $"{displayed_list_counter}");
            }

            // Add the displayed_shop_list as a new field to the embed.
            embed.AddField("What would you like to purchase? Select a number you wish to view.", $"{displayed_shop_list}");

            // Attach a locally generated image to the embed. This image hasn't been created yet, so the filename is just a placeholder for now.
            embed.WithImageUrl($"attachment://preview.png");

            // Create a new stream. We'll use this to create the locally generated image.
            MemoryStream memoryStream = new MemoryStream();

            // Generate a bitmap comprised of thumbnail previews of the décor being listed on the current page.
            Bitmap decor_preview = DecorInfoMethods.DecorPreviews(itemSession, sublist_length);

            // Save the décor preview bitmap to the stream as a PNG.
            decor_preview.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);

            // Ensure the stream is set to the beginning of itself.
            memoryStream.Seek(0, SeekOrigin.Begin);

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

            // If the bot lacks permission to attach files, catch the exception, send an error message, and return.
            try
            {
                // Reassign the menu session's message to a new message generated from the created embed and preview image.
                menuSession.MenuMessage = (RestUserMessage)await message.Channel.SendFileAsync(memoryStream, "preview.png", "", false, embed.Build());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                await message.DeleteAsync();
                await ErrorHandling.PermissionCheck(message);

                // Remove the menu entry from the global list.
                Global.MenuIdList.Remove(menuSession);

                return;
            }

            // Set the "message" variable to the menu session's message.
            message = menuSession.MenuMessage;

            // Edit the menu session according to the current message.
            menuSession.CurrentMenu = "Shop_Main_Menu";

            var component = new ComponentBuilder()
                .WithSelectMenu(selectMenu);

            // Add needed emote reactions for the menu.
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

            // Add two more reactions to the end of the message.
            component.WithButton("⚙️ Sort", customId: "sort", ButtonStyle.Secondary);
            component.WithButton("❌ Exit", customId: "exit", ButtonStyle.Secondary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task ShopDecorPreview(SocketGuildUser user, RestUserMessage message, int item_index)
        {
            // Get the account information of the command's target
            var account = UserInfoClasses.GetAccount(user);

            // Find both the menu session and item session associated with the current user and store them in variables.
            var menuSession = Global.MenuIdList.SingleOrDefault(x => x.User.Id == user.Id);
            var itemSession = Global.ItemIdList.SingleOrDefault(x => x.User.Id == user.Id);

            // Get the information of the chosen décor index.
            var decor_info = DecorInfoMethods.GetDecorInfo(itemSession.ItemList[item_index]);

            // Create a new embed that will be displayed in the message.
            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Décor Preview",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            // Create an empty string variable for the description text.
            string description_text = "";

            // Create an empty string list.
            List<string> owned_decor = new List<string> { };

            // Check if the user owns any décor.
            if (account.Decor_Owned != "")
            {
                // If so, convert their Decor_Owned value into a string list and assign it to the owned_decor string list.
                owned_decor = DecorInfoMethods.StringToStringArray(account.Decor_Owned);
            }

            // Perform a check to see if the user has enough money to purchase the décor. If so, state the cost.
            if (account.P_Medals >= decor_info.Price)
            {
                description_text = $"Purchase this décor for <:cost:780352551945895936> **{decor_info.Price}**?";
            }
            // If the user does not have enough money, prevent them from purchasing the décor.
            else
            {
                description_text = $"You do not have enough P-Medals to purchase this décor.";
            }

            // Add the description to the embed.
            embed.WithDescription(description_text);

            embed.WithThumbnailUrl($"{decor_info.Thumbnail_Link}");

            embed.AddField("Wallet", $"<:PMedals:672637091171139615> **{account.P_Medals}**");
            embed.AddField("Title", $"{decor_info.Title}", true);
            embed.AddField("Game", $"{decor_info.Game}", true);
            embed.AddField("Designer", $"[{decor_info.Designer_Name}]({decor_info.Designer_Link})", true);

            // If a description exists for the décor itself, add it as a field.
            if (decor_info.Description != null)
            {
                embed.AddField("Description", $"{decor_info.Description}", false);
            }

            // Set the color of the embed by converting the décor's stored hex value to a usable format.
            embed.WithColor((Discord.Color)System.Drawing.ColorTranslator.FromHtml($"{decor_info.Embed_Color}"));

            // Attach a locally generated image to the embed. This image hasn't been created yet, so the filename is just a placeholder for now.
            embed.WithImageUrl($"attachment://{decor_info.Decor_ID}_preview.png");

            // Create a new stream. We'll use this to create the locally generated image.
            MemoryStream memoryStream = new MemoryStream();

            // Generate a bitmap comprised of the thumbnail of the current décor.
            Bitmap decor_preview = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//Profile//StatusScreens//Decor//{decor_info.Decor_ID}//_Thumbnails//preview_2.png");

            // Save the décor preview bitmap to the stream as a PNG.
            decor_preview.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);

            // Ensure the stream is set to the beginning of itself.
            memoryStream.Seek(0, SeekOrigin.Begin);

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

            // If the bot lacks permission to attach files, catch the exception, send an error message, and return.
            try
            {
                // Reassign the menu session's message to a new message generated from the created embed and preview image.
                menuSession.MenuMessage = (RestUserMessage)await message.Channel.SendFileAsync(memoryStream, $"{decor_info.Decor_ID}_preview.png", "", false, embed.Build());
            }
            catch (Exception ex)
            {
                await ErrorHandling.AttachFilesError((SocketTextChannel)message.Channel);
                Console.WriteLine(ex);
                return;
            }

            // Set the "message" variable to the menu session's message.
            message = menuSession.MenuMessage;

            // Edit the menu session according to the current message.
            menuSession.CurrentMenu = "Shop_Decor_Preview";

            // Edit the item session to save the selected décor's ID to a variable.
            // If the user chooses to buy it, we will be able to pass its information to other methods.
            itemSession.SelectedItem = decor_info.Decor_ID;

            var component = new ComponentBuilder();

            component.WithButton("↩️ Back", customId: "return", ButtonStyle.Secondary);

            // If the user does not own this décor and has enough money to purchase it, add a checkmark reaction to the message.
            if (owned_decor.Contains(decor_info.Decor_ID) == false && account.P_Medals >= decor_info.Price)
            {
                component
                .WithButton("✅ Confirm", customId: "confirm", ButtonStyle.Secondary);
            }

            // Add the reactions to the message.
            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task ShopDecorPurchased(SocketGuildUser user, RestUserMessage message)
        {
            // Get the account information of the command's target
            var account = UserInfoClasses.GetAccount(user);

            // Find both the menu session and item session associated with the current user and store them in variables.
            var menuSession = Global.MenuIdList.SingleOrDefault(x => x.User.Id == user.Id);
            var itemSession = Global.ItemIdList.SingleOrDefault(x => x.User.Id == user.Id);

            // Get the information of the chosen décor index.
            var decor_info = DecorInfoMethods.GetDecorInfo(itemSession.SelectedItem);

            // Create a new embed that will be displayed in the message.
            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Purchase Complete!",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            // Determine the color and thumbnail for the embeded message.
            if (account.Profile_Theme == "P3")
            {
                embed.WithColor(37, 149, 255);
            }
            else if (account.Profile_Theme == "P4")
            {
                embed.WithColor(255, 229, 49);
            }
            else if (account.Profile_Theme == "P5")
            {
                embed.WithColor(213, 27, 4);
            }

            embed.WithDescription($"Do you want to set `{decor_info.Title}` as your décor right now?");

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
            menuSession.CurrentMenu = "Shop_Decor_Purchased";

            var component = new ComponentBuilder();

            component
                .WithButton("❌ No", customId: "no", ButtonStyle.Secondary)
                .WithButton("✅ Yes", customId: "yes", ButtonStyle.Secondary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task ShopDecorPurchaseSet(SocketGuildUser user, RestUserMessage message)
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
                Name = "Décor Set!",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            embed.WithDescription($"" +
                $"You can access your new décor at any time by using the **`status`** command." +
                $"\n" +
                $"To change décor, visit the **`settings`** menu and choose [Profile Settings] > [Status Screen Décor].");

            // Determine the color and thumbnail for the embeded message.
            if (account.Profile_Theme == "P3")
            {
                embed.WithColor(37, 149, 255);
            }
            else if (account.Profile_Theme == "P4")
            {
                embed.WithColor(255, 229, 49);
            }
            else if (account.Profile_Theme == "P5")
            {
                embed.WithColor(213, 27, 4);
            }

            // Edit the menu session according to the current message.
            menuSession.CurrentMenu = "Shop_Decor_Purchase_Set";

            var component = new ComponentBuilder();

            component
                .WithButton("💠 Return to Shop", customId: "return", ButtonStyle.Secondary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task ShopDecorPurchaseNotSet(SocketGuildUser user, RestUserMessage message)
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
                Name = "Décor Not Set",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            embed.WithDescription($"You can set your new décor at any time from the **`settings`** menu by choosing [Profile Settings] > [Status Screen Décor].");

            // Determine the color and thumbnail for the embeded message.
            if (account.Profile_Theme == "P3")
            {
                embed.WithColor(37, 149, 255);
            }
            else if (account.Profile_Theme == "P4")
            {
                embed.WithColor(255, 229, 49);
            }
            else if (account.Profile_Theme == "P5")
            {
                embed.WithColor(213, 27, 4);
            }

            // Edit the menu session according to the current message.
            menuSession.CurrentMenu = "Shop_Decor_Purchase_Not_Set";

            var component = new ComponentBuilder();

            component
                .WithButton("💠 Return to Shop", customId: "return", ButtonStyle.Secondary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task ShopSort(SocketGuildUser user, RestUserMessage message)
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
                Name = "Sort Décor",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            embed.AddField("Choose a method to sort décor entries by.", $"" +
                $"⚙️ **Current Setting:** **`{DecorInfoMethods.SortSettingToString(account.Shop_Sort)}`**\n" +
                $"\n" +
                $":one: By Title (A - Z)\n" +
                $":two: By Title (Z - A)\n" +
                $":three: By Cost (Low - High)\n" +
                $":four: By Cost (High - Low)\n" +
                $":five: By Release Order (Old - New)\n" +
                $":six: By Release Order (New - Old)");

            // Determine the color and thumbnail for the embeded message.
            if (account.Profile_Theme == "P3")
            {
                embed.WithColor(37, 149, 255);
            }
            else if (account.Profile_Theme == "P4")
            {
                embed.WithColor(255, 229, 49);
            }
            else if (account.Profile_Theme == "P5")
            {
                embed.WithColor(213, 27, 4);
            }

            menuSession.CurrentMenu = "Shop_Sort";

            var selectMenu = new SelectMenuBuilder()
                    .WithPlaceholder("Select an option")
                    .WithCustomId("shop-sort-select")
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .AddOption("By Title (A - Z)", "1", null, new Emoji("1️⃣"))
                    .AddOption("By Title (Z - A)", "2", null, new Emoji("2️⃣"))
                    .AddOption("By Cost (Low - High)", "3", null, new Emoji("3️⃣"))
                    .AddOption("By Cost (High - Low)", "4", null, new Emoji("4️⃣"))
                    .AddOption("By Release Order (Old - New)", "5", null, new Emoji("5️⃣"))
                    .AddOption("By Release Order (New - Old)", "6", null, new Emoji("6️⃣"));

            var component = new ComponentBuilder()
                .WithSelectMenu(selectMenu)
                .WithButton("↩️ Return", customId: "return", ButtonStyle.Secondary);

            await Utility.CleanMessage(menuSession, embed, component, true);
            Utility.NewTimer(menuSession);
        }

        public static async Task ShopSortConfirm(SocketGuildUser user, RestUserMessage message)
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

            embed.WithAuthor(author);

            embed.WithDescription($"Shop décor will now be sorted **`{DecorInfoMethods.SortSettingToString(account.Shop_Sort)}`**.");

            // Determine the color and thumbnail for the embeded message.
            if (account.Profile_Theme == "P3")
            {
                embed.WithColor(37, 149, 255);
            }
            else if (account.Profile_Theme == "P4")
            {
                embed.WithColor(255, 229, 49);
            }
            else if (account.Profile_Theme == "P5")
            {
                embed.WithColor(213, 27, 4);
            }

            menuSession.CurrentMenu = "Shop_Sort_Confirm";

            var component = new ComponentBuilder();

            component
                .WithButton("💠 Return to Shop", customId: "return", ButtonStyle.Secondary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }
    }
}
