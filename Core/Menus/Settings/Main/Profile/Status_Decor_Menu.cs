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
using System.IO;
using System.Drawing;

namespace SocialLinker.Core.Menus.Settings.Main.Profile
{
    class Status_Decor_Menu
    {
        public static async Task Status_Decor_Start(SocketGuildUser user, RestUserMessage message)
        {
            //Get the account information of the command's target
            var account = UserInfoClasses.GetAccount(user);

            // Find the menu session associated with the current user.
            var menuSession = Global.MenuIdList.SingleOrDefault(x => x.User.Id == user.Id);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Now Loading...",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            // Determine the color and thumbnail for the embeded message.
            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));

            // Create an empty string list.
            List<string> owned_decor = new List<string> { };

            // Check if the user owns any décor.
            if (account.Decor_Owned != "")
            {
                // If so, convert their Decor_Owned value into a string list and assign it to the owned_decor string list.
                owned_decor = DecorInfoMethods.StringToStringArray(account.Decor_Owned);
            }

            // Create another empty string list.
            List<string> user_content_filter = new List<string> { };

            // Check if the user has any titles listed in their content filter.
            if (account.Content_Filter != "")
            {
                // If so, convert their Content_Filter value into a string list and assign it to the user_content_filter string list.
                user_content_filter = DecorInfoMethods.StringToStringArray(account.Content_Filter);
            }

            // Create a new list by removing any décor that contains content specified in the user's content filter and assign it to the owned_decor variable.
            owned_decor = DecorInfoMethods.RemoveBlockedContentFromList(owned_decor, user_content_filter);

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
                ItemList = owned_decor,
                ItemIndexBase = 0,
                MaxItemsDisplayed = 6,
                CurrentPage = 1
            };

            // Add the item entry to the global list.
            Global.ItemIdList.Add(itemSession);

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
            menuSession.CurrentMenu = "Status_Decor_Start";
            menuSession.MenuTimer = new Timer()
            {
                // Create a timer that expires as a "time out" duration for the user.
                Interval = MenuConfig.menu.timerDuration,
                AutoReset = false,
                Enabled = true
            };

            // Create a new menu in the current channel.
            await Status_Decor_Main(menuSession.User, menuSession.MenuMessage);
        }

        public static async Task Status_Decor_Main(SocketGuildUser user, RestUserMessage message)
        {
            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            // Find both the menu session and item session associated with the current user and store them in variables.
            var menuSession = Global.MenuIdList.SingleOrDefault(x => x.User.Id == user.Id);
            var itemSession = Global.ItemIdList.SingleOrDefault(x => x.User.Id == user.Id);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Status Screen Décor",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            // Determine the color and thumbnail for the embeded message.
            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Profile_Config_Thumbnail(account));

            // Create a string variable to store the text that will be displayed on the message's body.
            string displayed_decor_list = "";

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
                displayed_decor_list += $":{DecorInfoMethods.NumberToWords(displayed_list_counter)}: {decor_info.Title}\n";
            }

            // Create a string variable to store text for the footer. This will change depending on the state of the menu.
            string footer_text = "";

            // Depending on whether or not the user owns or can set any décor, perform different actions.
            if (displayed_decor_list.Length > 0)
            {
                // Add a "Back" button to be displayed on the footer.
                footer_text += "↩️ Profile Settings | ";

                // Check if the starting item index is greater than or equal to max_items_displayed.
                if (itemSession.ItemIndexBase >= itemSession.MaxItemsDisplayed)
                {
                    // If so, there will be a "Previous Page" button displayed on the footer.
                    footer_text += "◀️ Previous Page | ";
                }
                // Check if the number of items in the list minus the starting item index is more than max_items_displayed.
                if (remaining_list_length > itemSession.MaxItemsDisplayed)
                {
                    // If so, there will be a "Next Page" button on the footer.
                    footer_text += "▶️ Next Page | ";
                }

                // Calculate the amount of pages there will be in total and store it in a variable.
                int pageCount = (itemSession.ItemList.Count + itemSession.MaxItemsDisplayed - 1) / itemSession.MaxItemsDisplayed;

                // Add two icons to the end of footer_text regardless of the state, plus a page counter on a new line.
                footer_text += $"⚙️ Sort\nPage {itemSession.CurrentPage} / {pageCount}";

                // Create the footer object for the embed.
                var footer = new EmbedFooterBuilder
                {
                    Text = footer_text
                };

                // Add the footer to the embed.
                embed.WithFooter(footer);

                // Create an empty string variable. This will hold part of the embeded message's description
                string description_text = "";

                // If the user has a décor and a profile theme currently set, create a description text explaining how to remove the currently set décor.
                if (account.Decor_Setting != "" && account.Profile_Theme != "")
                {
                    description_text = "" +
                        "**Select a décor to view.**\n" +
                        "**To remove your current décor and set the default one for your profile theme, select :white_square_button:.**";
                }
                // If not, create a default description text instructing to select a décor.
                else
                {
                    description_text = "**Select a décor to view.**";
                }

                // Create a string variable that will hold the title of the user's currently set décor.
                string set_decor_title = DecorInfoMethods.GetDecorTitle(user);

                embed.WithDescription("" +
                    $"{description_text}\n" +
                    "\n" +
                    $"⚙️ **Current setting:** **`{set_decor_title}`**\n" +
                    "\n" +
                    $"{displayed_decor_list}");

                // Attach a locally generated image to the embed.
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
                    await ErrorHandling.AttachFilesError((SocketTextChannel)message.Channel);
                    Console.WriteLine(ex);
                    return;
                }

                // Set the "message" variable to the menu session's message.
                message = menuSession.MenuMessage;
            }
            else
            {
                // Add a "Back" button to be displayed on the footer.
                footer_text += "↩️ Profile Settings";

                // Create the footer object for the embed.
                var footer = new EmbedFooterBuilder
                {
                    Text = footer_text
                };

                // Add the footer to the embed.
                embed.WithFooter(footer);

                embed.WithDescription($"You don't have any other décor to set. Visit the Décor Shop with the **`{BotConfig.bot.cmdPrefix}shop`** command to browse and buy décor for your collection.");

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
            }

            // Edit the menu session according to the current message.
            menuSession.CurrentMenu = "Status_Decor_Main";
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

            // Check if the starting item index is greater than or equal to max_items_displayed.
            if (itemSession.ItemIndexBase >= itemSession.MaxItemsDisplayed)
            {
                // If so, there will be a "Previous Page" button added as a reaction.
                reaction_list.Add(new Emoji("◀️"));
            }

            // Check if the number of items in the list minus the starting item index is more than max_items_displayed.
            if (remaining_list_length > itemSession.MaxItemsDisplayed)
            {
                // If so, there will be a "Next Page" button added as a reaction.
                reaction_list.Add(new Emoji("▶️"));
            }

            // Reset the displayed_list_counter to zero.
            displayed_list_counter = 0;

            for (int i = 0; i < sublist_length; i++)
            {
                // Increase the displayed_list_counter by one.
                displayed_list_counter += 1;

                // For each loop iteration, add a keycap emote representing an item entry being displayed to the user.
                reaction_list.Add(new Emoji($"{DecorInfoMethods.NumberToKeycapEmoji(displayed_list_counter)}"));
            }

            // If the user owns any décor, add a gear reaction in order to sort entries.
            if (displayed_decor_list.Length > 0)
            {
                reaction_list.Add(new Emoji("⚙️"));
            }

            // If the user has a décor and a profile theme currently set, add a box reaction. This gives the option to remove the set décor and return to the default one.
            if (account.Decor_Setting != "" && account.Profile_Theme != "")
            {
                reaction_list.Add(new Emoji("🔳"));
            }

            // Add the reactions to the message.
            _ = ReactionHandling.AddReactionsToMenu(message, reaction_list);
        }

        public static async Task Set_Decor_Preview(SocketGuildUser user, RestUserMessage message, int item_index)
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

            embed.WithThumbnailUrl($"{decor_info.Thumbnail_Link}");

            // Add a description to the embed.
            embed.WithDescription("Set this décor?");

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

            // Add a footer to the embed.
            var footer = new EmbedFooterBuilder
            {
                Text = "↩️ Back | ✅ Confirm"
            };

            embed.WithFooter(footer);

            // Attach a locally generated image to the embed. This image hasn't been created yet, so the filename is just a placeholder for now.
            embed.WithImageUrl($"attachment://{decor_info.Decor_ID}_preview.png");

            // Create a new stream. We'll use this to create the locally generated image.
            MemoryStream memoryStream = new MemoryStream();

            // Generate a bitmap comprised of the thumbnail of the current décor.
            Bitmap decor_preview = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//Profile//StatusScreens//Decor//{decor_info.Decor_ID}//_Thumbnails//preview_1.png");

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
            menuSession.CurrentMenu = "Set_Decor_Preview";
            menuSession.MenuTimer = new Timer()
            {
                // Create a timer that expires as a "time out" duration for the user.
                Interval = MenuConfig.menu.timerDuration,
                AutoReset = false,
                Enabled = true
            };

            // Edit the item session to save the selected décor's ID to a variable.
            // If the user chooses to buy it, we will be able to pass its information to other methods.
            itemSession.SelectedItem = decor_info.Decor_ID;

            // If the menu timer runs out, activate a function.
            menuSession.MenuTimer.Elapsed += (sender, e) => MenuTimer_Elapsed(sender, e, menuSession, itemSession);

            // Create an empty list for reactions.
            List<IEmote> reaction_list = new List<IEmote> { };

            // Add needed emote reactions for the menu.
            reaction_list.Add(new Emoji("↩️"));
            reaction_list.Add(new Emoji("✅"));

            // Add the reactions to the message.
            _ = ReactionHandling.AddReactionsToMenu(message, reaction_list);
        }

        public static async Task Set_Decor_Confirm(SocketGuildUser user, RestUserMessage message)
        {
            // Get the account information of the command's target
            var account = UserInfoClasses.GetAccount(user);

            // Find both the menu session and item session associated with the current user and store them in variables.
            var menuSession = Global.MenuIdList.SingleOrDefault(x => x.User.Id == user.Id);
            var itemSession = Global.ItemIdList.SingleOrDefault(x => x.User.Id == user.Id);

            // Create a string variable that will hold the title of the user's currently set décor.
            string set_decor_title = DecorInfoMethods.GetDecorTitle(user);

            // Create a new embed that will be displayed in the message.
            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Settings Saved",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            // Determine the color and thumbnail for the embeded message.
            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Profile_Config_Thumbnail(account));

            embed.WithDescription($"Your décor has been set to **`{set_decor_title}`**.");

            var footer = new EmbedFooterBuilder
            {
                Text = "💠 Décor Settings | ❌ Close Menu"
            };

            // Add the footer to the embed.
            embed.WithFooter(footer);

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
            menuSession.CurrentMenu = "Set_Decor_Confirm";
            menuSession.MenuTimer = new Timer()
            {
                // Create a timer that expires as a "time out" duration for the user.
                Interval = MenuConfig.menu.timerDuration,
                AutoReset = false,
                Enabled = true
            };

            // If the menu timer runs out, activate a function.
            menuSession.MenuTimer.Elapsed += (sender, e) => MenuTimer_Elapsed(sender, e, menuSession, itemSession);

            // Create an empty list for reactions.
            List<IEmote> reaction_list = new List<IEmote> { };

            // Add needed emote reactions for the menu.
            reaction_list.Add(new Emoji("💠"));
            reaction_list.Add(new Emoji("❌"));
            
            // Add the reactions to the message.
            _ = ReactionHandling.AddReactionsToMenu(message, reaction_list);
        }

        public static async Task Decor_Sort(SocketGuildUser user, RestUserMessage message)
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

            var footer = new EmbedFooterBuilder
            {
                Text = "↩️ Back"
            };

            embed.WithAuthor(author);
            embed.WithFooter(footer);

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
            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Profile_Config_Thumbnail(account));

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
            menuSession.CurrentMenu = "Decor_Sort";
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
            reaction_list.Add(new Emoji("\u0036\ufe0f\u20e3"));

            // Add the reactions to the message.
            _ = ReactionHandling.AddReactionsToMenu(message, reaction_list);
        }

        public static async Task Decor_Sort_Confirm(SocketGuildUser user, RestUserMessage message)
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
                Text = "💠 Décor Settings | ❌ Close Menu"
            };

            embed.WithAuthor(author);
            embed.WithFooter(footer);

            embed.WithDescription($"Décor will now be sorted **`{DecorInfoMethods.SortSettingToString(account.Shop_Sort)}`**.");

            // Determine the color and thumbnail for the embeded message.
            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Profile_Config_Thumbnail(account));

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
            menuSession.CurrentMenu = "Decor_Sort_Confirm";
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

        public static async Task Status_Decor_Exit(SocketGuildUser user, RestUserMessage message)
        {
            //Get the account information of the command's target
            var account = UserInfoClasses.GetAccount(user);

            // Find the menu session associated with the current user and store it in a variable.
            var menuSession = Global.MenuIdList.SingleOrDefault(x => x.User.Id == user.Id);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Now Loading...",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            //Determine color for embeded message
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
            menuSession.CurrentMenu = "Status_Decor_Exit";
            menuSession.MenuTimer = new Timer()
            {
                // Create a timer that expires as a "time out" duration for the user.
                Interval = MenuConfig.menu.timerDuration,
                AutoReset = false,
                Enabled = true
            };

            // Create a new menu in the current channel.
            await Profile_Settings_Menu.Profile_Settings_Main(menuSession.User, menuSession.MenuMessage);
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

            embed.WithDescription($"You can set your status screen Décor at any time from the **`{BotConfig.bot.cmdPrefix}settings`** menu by choosing [Profile Settings] > [Status Screen Décor].");
            return embed;
        }
    }
}
