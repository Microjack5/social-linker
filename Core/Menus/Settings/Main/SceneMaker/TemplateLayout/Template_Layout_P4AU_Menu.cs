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

namespace SocialLinker.Core.Menus.Settings.Main.SceneMaker.TemplateLayout
{
    class Template_Layout_P4AU_Menu
    {
        public static async Task Template_Layout_P4AU_Main(SocketGuildUser user, RestUserMessage message)
        {
            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            // Find the menu session associated with the current user.
            var menuSession = Global.MenuIdList.SingleOrDefault(x => x.User.Id == user.Id);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Template Settings - Persona 4 Arena Ultimax",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            var footer = new EmbedFooterBuilder
            {
                Text = "↩️ Return to Template Layout Menu"
            };

            embed.WithFooter(footer);

            // Assign a color and thumbnail to the embeded message based on the title being edited.
            embed.WithColor(EmbedSettings.Get_Game_Color("P4AU", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P4AU"));

            embed.WithDescription("" +
                "**Select a setting to edit.**\n" +
                "\n" +
                ":one: Scene Type\n" +
                ":two: Auto Read\n" +
                ":three: Control Panel\n" +
                ":four: Sprite Placement\n" +
                ":five: Protagonist Highlight\n");

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
            menuSession.CurrentMenu = "Template_Layout_P4AU_Main";
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

            // Add the reactions to the message.
            _ = ReactionHandling.AddReactionsToMenu(message, reaction_list);
        }

        public static async Task Template_Layout_P4AU_Scene_Type(SocketGuildUser user, RestUserMessage message)
        {
            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            // Find the menu session associated with the current user.
            var menuSession = Global.MenuIdList.SingleOrDefault(x => x.User.Id == user.Id);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Scene Type",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            var footer = new EmbedFooterBuilder
            {
                Text = "↩️ Return to P4AU Template Settings"
            };

            embed.WithFooter(footer);

            // Assign a color and thumbnail to the embeded message based on the title being edited.
            embed.WithColor(EmbedSettings.Get_Game_Color("P4AU", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P4AU"));

            embed.WithDescription("" +
                "**Toggle between dialogue and narration formats for created scenes.**\n" +
                "\n" +
                $"⚙️ **Current setting:** **`{account.P4AU_TS_Scene_Type}`**\n" +
                "\n" +
                ":one: Dialogue\n" +
                ":two: Narration\n");

            embed.WithImageUrl("https://i.imgur.com/Nj5I4Ov.png");

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
            menuSession.CurrentMenu = "Template_Layout_P4AU_Scene_Type";
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

            // Add the reactions to the message.
            _ = ReactionHandling.AddReactionsToMenu(message, reaction_list);
        }

        public static async Task Template_Layout_P4AU_Auto_Advance(SocketGuildUser user, RestUserMessage message)
        {
            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            // Find the menu session associated with the current user.
            var menuSession = Global.MenuIdList.SingleOrDefault(x => x.User.Id == user.Id);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Auto Read",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            var footer = new EmbedFooterBuilder
            {
                Text = "↩️ Return to P4AU Template Settings"
            };

            embed.WithFooter(footer);

            // Assign a color and thumbnail to the embeded message based on the title being edited.
            embed.WithColor(EmbedSettings.Get_Game_Color("P4AU", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P4AU"));

            embed.WithDescription("" +
                "**Toggle the cursor's auto read color scheme on and off.**\n" +
                "\n" +
                $"⚙️ **Current setting:** **`{account.P4AU_TS_Auto_Advance}`**\n" +
                "\n" +
                ":one: On\n" +
                ":two: Off\n");

            embed.WithImageUrl("https://i.imgur.com/8iSnblW.png");

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
            menuSession.CurrentMenu = "Template_Layout_P4AU_Auto_Advance";
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

            // Add the reactions to the message.
            _ = ReactionHandling.AddReactionsToMenu(message, reaction_list);
        }

        public static async Task Template_Layout_P4AU_Control_Panel(SocketGuildUser user, RestUserMessage message)
        {
            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            // Find the menu session associated with the current user.
            var menuSession = Global.MenuIdList.SingleOrDefault(x => x.User.Id == user.Id);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Control Panel",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            var footer = new EmbedFooterBuilder
            {
                Text = "↩️ Return to P4AU Template Settings"
            };

            embed.WithFooter(footer);

            // Assign a color and thumbnail to the embeded message based on the title being edited.
            embed.WithColor(EmbedSettings.Get_Game_Color("P4AU", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P4AU"));

            embed.WithDescription("" +
                "**Toggle between multiple control guide layouts.**\n" +
                "\n" +
                $"⚙️ **Current setting:** **`{account.P4AU_TS_Panel}`**\n" +
                "\n" +
                ":one: PlayStation®️ 3\n" +
                ":two: PlayStation®️ 4\n" +
                ":three: PlayStation®️ 4 (PC Layout)\n" +
                ":four: Xbox 360\n" +
                ":five: Xbox One (PC Layout)\n" +
                ":six: Nintendo Switch\n" +
                ":seven: Nintendo Switch (PC Layout)\n" +
                ":eight: Keyboard\n" +
                ":nine: None");

            embed.WithImageUrl("https://i.imgur.com/0hvNpig.png");

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
            menuSession.CurrentMenu = "Template_Layout_P4AU_Control_Panel";
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
            reaction_list.Add(new Emoji("\u0037\ufe0f\u20e3"));
            reaction_list.Add(new Emoji("\u0038\ufe0f\u20e3"));
            reaction_list.Add(new Emoji("\u0039\ufe0f\u20e3"));

            // Add the reactions to the message.
            _ = ReactionHandling.AddReactionsToMenu(message, reaction_list);
        }

        public static async Task Template_Layout_P4AU_Sprite_Placement(SocketGuildUser user, RestUserMessage message)
        {
            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            // Find the menu session associated with the current user.
            var menuSession = Global.MenuIdList.SingleOrDefault(x => x.User.Id == user.Id);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Sprite Placement",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            var footer = new EmbedFooterBuilder
            {
                Text = "↩️ Return to P4AU Template Settings"
            };

            embed.WithFooter(footer);

            // Assign a color and thumbnail to the embeded message based on the title being edited.
            embed.WithColor(EmbedSettings.Get_Game_Color("P4AU", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P4AU"));

            embed.WithDescription("" +
                "**Choose the default position character sprites are rendered at.**\n" +
                "\n" +
                $"⚙️ **Current setting:** **`{account.P4AU_TS_Position}`**\n" +
                "\n" +
                ":one: Left\n" +
                ":two: Center\n" +
                ":three: Right\n");

            embed.WithImageUrl("https://i.imgur.com/4H6iZF6.png");

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
            menuSession.CurrentMenu = "Template_Layout_P4AU_Sprite_Placement";
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

            // Add the reactions to the message.
            _ = ReactionHandling.AddReactionsToMenu(message, reaction_list);
        }

        public static async Task Template_Layout_P4AU_Dual_Character(SocketGuildUser user, RestUserMessage message)
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
                Text = "↩️ Return to P4AU Template Settings"
            };

            embed.WithFooter(footer);

            // Assign a color and thumbnail to the embeded message based on the title being edited.
            embed.WithColor(EmbedSettings.Get_Game_Color("P4AU", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P4AU"));

            embed.WithDescription("" +
                "**Change how left-sided sprites are displayed in dual character scenes.**\n" +
                "\n" +
                $"⚙️ **Current setting:** **`{account.P4AU_TS_Dual}`**\n" +
                "\n" +
                ":one: Normal\n" +
                ":two: Navigator\n" +
                ":three: Television\n" +
                ":four: Phone Call");

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
            menuSession.CurrentMenu = "Template_Layout_P4AU_Dual_Character";
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

            // Add the reactions to the message.
            _ = ReactionHandling.AddReactionsToMenu(message, reaction_list);
        }

        public static async Task Template_Layout_P4AU_Navigator_Location(SocketGuildUser user, RestUserMessage message)
        {
            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            // Find the menu session associated with the current user.
            var menuSession = Global.MenuIdList.SingleOrDefault(x => x.User.Id == user.Id);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Navigator Location",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            // Assign a color and thumbnail to the embeded message based on the title being edited.
            embed.WithColor(EmbedSettings.Get_Game_Color("P4AU", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P4AU"));

            // Search for an item list that corresponds to the user's ID.
            var itemSession = Global.ItemIdList.SingleOrDefault(x => x.User.Id == user.Id);

            // If the item session is null, create a new one.
            if (itemSession == null)
            {
                // Create a new item identifier entry for this current user to keep track of the position of décor on the menu.
                itemSession = new ItemListIterator()
                {
                    User = user,
                    ItemList = new string[75].ToList(),
                    ItemIndexBase = 0,
                    MaxItemsDisplayed = 6,
                    CurrentPage = 1
                };

                // Add the item entry to the global list.
                Global.ItemIdList.Add(itemSession);
            }

            // Create a string variable to store the text that will be displayed on the message's body.
            string displayed_location_list = "";

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

                // Add the entry to the displayed_shop_list string.
                displayed_location_list += $":{DecorInfoMethods.NumberToWords(displayed_list_counter)}: Location {itemSession.ItemIndexBase + displayed_list_counter}\n";
            }

            embed.WithDescription("" +
                "**Choose a location to set the background for navigators.**\n" +
                "\n" +
                $"⚙️ **Current setting:** **`Location {account.P4AU_TS_Nav_BG}`**\n" +
                $"\n" +
                $"{displayed_location_list}");

            // Create a string variable to store text for the footer. This will change depending on the state of the menu.
            string footer_text = "💠 Return to P4AU Template Settings ";

            // Check if the starting item index is greater than or equal to max_items_displayed.
            if (itemSession.ItemIndexBase >= itemSession.MaxItemsDisplayed)
            {
                // If so, there will be a "Previous Page" button displayed on the footer.
                footer_text += "| ◀️ Previous Page ";
            }
            // Check if the number of items in the list minus the starting item index is more than max_items_displayed.
            if (remaining_list_length > itemSession.MaxItemsDisplayed)
            {
                // If so, there will be a "Next Page" button on the footer.
                footer_text += "| ▶️ Next Page";
            }

            // Calculate the amount of pages there will be in total and store it in a variable.
            int pageCount = (itemSession.ItemList.Count + itemSession.MaxItemsDisplayed - 1) / itemSession.MaxItemsDisplayed;

            // Add two icons to the end of footer_text regardless of the state, plus a page counter on a new line.
            footer_text += $"\nPage {itemSession.CurrentPage} / {pageCount}";

            // Create the footer object for the embed.
            var footer = new EmbedFooterBuilder
            {
                Text = footer_text
            };

            // Add the footer to the embed.
            embed.WithFooter(footer);

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
            menuSession.CurrentMenu = "Template_Layout_P4AU_Navigator_Location";
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

            // Add the reactions to the message.
            _ = ReactionHandling.AddReactionsToMenu(message, reaction_list);
        }

        public static async Task Template_Layout_P4AU_Caller_Location(SocketGuildUser user, RestUserMessage message)
        {
            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            // Find the menu session associated with the current user.
            var menuSession = Global.MenuIdList.SingleOrDefault(x => x.User.Id == user.Id);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Caller Location",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            var footer = new EmbedFooterBuilder
            {
                Text = "💠 Return to P4AU Template Settings"
            };

            embed.WithFooter(footer);

            // Assign a color and thumbnail to the embeded message based on the title being edited.
            embed.WithColor(EmbedSettings.Get_Game_Color("P4AU", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P4AU"));

            embed.WithDescription("" +
                "**Choose a location to set the background for phone calls.**\n" +
                "\n" +
                $"⚙️ **Current setting:** **`{account.P4AU_TS_Phone_BG}`**\n" +
                $"\n" +
                $":one: Dojima Residence, Bedroom\n" +
                $":two: Yosuke’s Room\n" +
                $":three: Chie’s Room\n" +
                $":four: Airport\n" +
                $":five: Amagi Inn, Kitchen\n" +
                $":six: Limousine\n" +
                $":seven: Junes Food Court\n" +
                $":eight: Junes TV Department\n" +
                $":nine: Hotel\n");

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
            menuSession.CurrentMenu = "Template_Layout_P4AU_Caller_Location";
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
            reaction_list.Add(new Emoji("\u0031\ufe0f\u20e3"));
            reaction_list.Add(new Emoji("\u0032\ufe0f\u20e3"));
            reaction_list.Add(new Emoji("\u0033\ufe0f\u20e3"));
            reaction_list.Add(new Emoji("\u0034\ufe0f\u20e3"));
            reaction_list.Add(new Emoji("\u0035\ufe0f\u20e3"));
            reaction_list.Add(new Emoji("\u0036\ufe0f\u20e3"));
            reaction_list.Add(new Emoji("\u0037\ufe0f\u20e3"));
            reaction_list.Add(new Emoji("\u0038\ufe0f\u20e3"));
            reaction_list.Add(new Emoji("\u0039\ufe0f\u20e3"));

            // Add the reactions to the message.
            _ = ReactionHandling.AddReactionsToMenu(message, reaction_list);
        }

        public static async Task Template_Layout_P4AU_Highlight(SocketGuildUser user, RestUserMessage message)
        {
            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            // Find the menu session associated with the current user.
            var menuSession = Global.MenuIdList.SingleOrDefault(x => x.User.Id == user.Id);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Protagonist Highlight",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            var footer = new EmbedFooterBuilder
            {
                Text = "↩️ Return to P4AU Template Settings"
            };

            embed.WithFooter(footer);

            // Assign a color and thumbnail to the embeded message based on the title being edited.
            embed.WithColor(EmbedSettings.Get_Game_Color("P4AU", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P4AU"));

            embed.WithDescription("" +
                "**Toggle the highlight for the scene’s protagonist on and off when speaking.**\n" +
                "\n" +
                $"⚙️ **Current setting:** **`{account.P4AU_TS_Highlight}`**\n" +
                "\n" +
                ":one: On\n" +
                ":two: Off\n");

            embed.WithImageUrl("https://i.imgur.com/hhOHIKH.png");

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
            menuSession.CurrentMenu = "Template_Layout_P4AU_Highlight";
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

            // Add the reactions to the message.
            _ = ReactionHandling.AddReactionsToMenu(message, reaction_list);
        }

        public static async Task Template_Layout_P4AU_Scene_Type_Confirm(SocketGuildUser user, RestUserMessage message)
        {
            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            // Find the menu session associated with the current user.
            var menuSession = Global.MenuIdList.SingleOrDefault(x => x.User.Id == user.Id);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Settings Saved",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            var footer = new EmbedFooterBuilder
            {
                Text = "💠 P4AU Template Settings | ❌ Close Menu"
            };

            embed.WithFooter(footer);

            // Assign a color and thumbnail to the embeded message based on the title being edited.
            embed.WithColor(EmbedSettings.Get_Game_Color("P4AU", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P4AU"));

            embed.WithDescription("" +
                $"The scene type has been set to **`{account.P4AU_TS_Scene_Type}`**.\n");

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
            menuSession.CurrentMenu = "Template_Layout_P4AU_Scene_Type_Confirm";
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

        public static async Task Template_Layout_P4AU_Auto_Advance_Confirm(SocketGuildUser user, RestUserMessage message)
        {
            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            // Find the menu session associated with the current user.
            var menuSession = Global.MenuIdList.SingleOrDefault(x => x.User.Id == user.Id);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Settings Saved",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            var footer = new EmbedFooterBuilder
            {
                Text = "💠 P4AU Template Settings | ❌ Close Menu"
            };

            embed.WithFooter(footer);

            // Assign a color and thumbnail to the embeded message based on the title being edited.
            embed.WithColor(EmbedSettings.Get_Game_Color("P4AU", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P4AU"));

            embed.WithDescription("" +
                $"The auto read cursor has been set to **`{account.P4AU_TS_Auto_Advance}`**.\n");

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
            menuSession.CurrentMenu = "Template_Layout_P4AU_Auto_Advance_Confirm";
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

        public static async Task Template_Layout_P4AU_Control_Panel_Confirm(SocketGuildUser user, RestUserMessage message)
        {
            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            // Find the menu session associated with the current user.
            var menuSession = Global.MenuIdList.SingleOrDefault(x => x.User.Id == user.Id);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Settings Saved",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            var footer = new EmbedFooterBuilder
            {
                Text = "💠 P4AU Template Settings | ❌ Close Menu"
            };

            embed.WithFooter(footer);

            // Assign a color and thumbnail to the embeded message based on the title being edited.
            embed.WithColor(EmbedSettings.Get_Game_Color("P4AU", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P4AU"));

            embed.WithDescription("" +
                $"The control panel has been set to **`{account.P4AU_TS_Panel}`**.\n");

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
            menuSession.CurrentMenu = "Template_Layout_P4AU_Control_Panel_Confirm";
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

        public static async Task Template_Layout_P4AU_Sprite_Placement_Confirm(SocketGuildUser user, RestUserMessage message)
        {
            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            // Find the menu session associated with the current user.
            var menuSession = Global.MenuIdList.SingleOrDefault(x => x.User.Id == user.Id);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Settings Saved",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            var footer = new EmbedFooterBuilder
            {
                Text = "💠 P4AU Template Settings | ❌ Close Menu"
            };

            embed.WithFooter(footer);

            // Assign a color and thumbnail to the embeded message based on the title being edited.
            embed.WithColor(EmbedSettings.Get_Game_Color("P4AU", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P4AU"));

            embed.WithDescription("" +
                $"Sprite placements have been set to **`{account.P4AU_TS_Position}`**.\n");

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
            menuSession.CurrentMenu = "Template_Layout_P4AU_Sprite_Placement_Confirm";
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

        public static async Task Template_Layout_P4AU_Dual_Character_Default_Confirm(SocketGuildUser user, RestUserMessage message)
        {
            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            // Find the menu session associated with the current user.
            var menuSession = Global.MenuIdList.SingleOrDefault(x => x.User.Id == user.Id);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Settings Saved",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            var footer = new EmbedFooterBuilder
            {
                Text = "💠 P4AU Template Settings | ❌ Close Menu"
            };

            embed.WithFooter(footer);

            // Assign a color and thumbnail to the embeded message based on the title being edited.
            embed.WithColor(EmbedSettings.Get_Game_Color("P4AU", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P4AU"));

            embed.WithDescription("" +
                $"Left-sided sprites in dual character scenes have been set to **`{account.P4AU_TS_Dual}`**.\n");

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
            menuSession.CurrentMenu = "Template_Layout_P4AU_Dual_Character_Default_Confirm";
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

        public static async Task Template_Layout_P4AU_Dual_Character_Navigator_Confirm(SocketGuildUser user, RestUserMessage message)
        {
            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            // Find the menu session associated with the current user.
            var menuSession = Global.MenuIdList.SingleOrDefault(x => x.User.Id == user.Id);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Settings Saved",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            var footer = new EmbedFooterBuilder
            {
                Text = "💠 P4AU Template Settings | ✅ Confirm | ❌ Close Menu"
            };

            embed.WithFooter(footer);

            // Assign a color and thumbnail to the embeded message based on the title being edited.
            embed.WithColor(EmbedSettings.Get_Game_Color("P4AU", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P4AU"));

            embed.WithDescription("" +
                $"Left-sided sprites in dual character scenes have been set to **`{account.P4AU_TS_Dual}`**.\n" +
                $"\n" +
                $"Navigators use different backgrounds in addition to the main one in a scene. View navigator location background settings?\n");

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
            menuSession.CurrentMenu = "Template_Layout_P4AU_Dual_Character_Navigator_Confirm";
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
            reaction_list.Add(new Emoji("✅"));
            reaction_list.Add(new Emoji("❌"));

            // Add the reactions to the message.
            _ = ReactionHandling.AddReactionsToMenu(message, reaction_list);
        }

        public static async Task Template_Layout_P4AU_Dual_Character_Caller_Confirm(SocketGuildUser user, RestUserMessage message)
        {
            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            // Find the menu session associated with the current user.
            var menuSession = Global.MenuIdList.SingleOrDefault(x => x.User.Id == user.Id);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Settings Saved",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            var footer = new EmbedFooterBuilder
            {
                Text = "💠 P4AU Template Settings | ✅ Confirm | ❌ Close Menu"
            };

            embed.WithFooter(footer);

            // Assign a color and thumbnail to the embeded message based on the title being edited.
            embed.WithColor(EmbedSettings.Get_Game_Color("P4AU", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P4AU"));

            embed.WithDescription("" +
                $"Left-sided sprites in dual character scenes have been set to **`{account.P4AU_TS_Dual}`**.\n" +
                $"\n" +
                $"Phone calls use different backgrounds in addition to the main one in a scene. View caller location background settings?\n");

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
            menuSession.CurrentMenu = "Template_Layout_P4AU_Dual_Character_Caller_Confirm";
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
            reaction_list.Add(new Emoji("✅"));
            reaction_list.Add(new Emoji("❌"));

            // Add the reactions to the message.
            _ = ReactionHandling.AddReactionsToMenu(message, reaction_list);
        }

        public static async Task Template_Layout_P4AU_Navigator_Location_Confirm(SocketGuildUser user, RestUserMessage message)
        {
            // Search for an item list that corresponds to the user's ID.
            var itemSession = Global.ItemIdList.SingleOrDefault(x => x.User.Id == user.Id);

            // If the item session is not null, remove it from the global list.
            if (itemSession != null)
            {
                Global.ItemIdList.Remove(itemSession);
            }

            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            // Find the menu session associated with the current user.
            var menuSession = Global.MenuIdList.SingleOrDefault(x => x.User.Id == user.Id);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Settings Saved",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            var footer = new EmbedFooterBuilder
            {
                Text = "💠 P4AU Template Settings | ❌ Close Menu"
            };

            embed.WithFooter(footer);

            // Assign a color and thumbnail to the embeded message based on the title being edited.
            embed.WithColor(EmbedSettings.Get_Game_Color("P4AU", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P4AU"));

            embed.WithDescription("" +
                $"The navigator location has been set to **`Location {account.P4AU_TS_Nav_BG}`**.\n");

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
            menuSession.CurrentMenu = "Template_Layout_P4AU_Navigator_Location_Confirm";
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

        public static async Task Template_Layout_P4AU_Caller_Location_Confirm(SocketGuildUser user, RestUserMessage message)
        {
            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            // Find the menu session associated with the current user.
            var menuSession = Global.MenuIdList.SingleOrDefault(x => x.User.Id == user.Id);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Settings Saved",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            var footer = new EmbedFooterBuilder
            {
                Text = "💠 P4AU Template Settings | ❌ Close Menu"
            };

            embed.WithFooter(footer);

            // Assign a color and thumbnail to the embeded message based on the title being edited.
            embed.WithColor(EmbedSettings.Get_Game_Color("P4AU", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P4AU"));

            embed.WithDescription("" +
                $"The caller location has been set to **`{account.P4AU_TS_Phone_BG}`**.\n");

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
            menuSession.CurrentMenu = "Template_Layout_P4AU_Caller_Location_Confirm";
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

        public static async Task Template_Layout_P4AU_Highlight_Confirm(SocketGuildUser user, RestUserMessage message)
        {
            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            // Find the menu session associated with the current user.
            var menuSession = Global.MenuIdList.SingleOrDefault(x => x.User.Id == user.Id);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Settings Saved",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            var footer = new EmbedFooterBuilder
            {
                Text = "💠 P4AU Template Settings | ❌ Close Menu"
            };

            embed.WithFooter(footer);

            // Assign a color and thumbnail to the embeded message based on the title being edited.
            embed.WithColor(EmbedSettings.Get_Game_Color("P4AU", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P4AU"));

            embed.WithDescription("" +
                $"Highlights for right-sided sprites have been set to **`{account.P4AU_TS_Highlight}`** when speaking.\n");

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
            menuSession.CurrentMenu = "Template_Layout_P4AU_Highlight_Confirm";
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

            embed.WithDescription($"You can adjust your template settings at any time from the **`settings`** menu by choosing [Scene Maker Settings] > [Template Layout].");
            return embed;
        }
    }
}
