using Discord;
using Discord.Rest;
using Discord.WebSocket;
using SocialLinker.Config;
using SocialLinker.Core.CloudStorageTables;
using SocialLinker.Core.Menus.Help.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace SocialLinker.Core.Menus.InitialUsage.Main
{
    class First_Use_Content_Filter_Menu
    {
        public static async Task First_Use_Intro_Initialize(SocialLinkerCommand command)
        {
            // Create two variables to check if there is a menu list entry with either the current channel ID or current user ID.
            var channelSearch = Global.MenuIdList.SingleOrDefault(x => x.MenuMessage.Channel.Id == command.Channel.Id);
            var userSearch = Global.MenuIdList.SingleOrDefault(x => x.User.Id == command.User.Id);
            var filterSession = Global.ContentFilterList.SingleOrDefault(x => x.User.Id == command.User.Id);

            // If the channel entry exists and the user is not the same, create a new menu.
            if (channelSearch != null && channelSearch.User.Id != command.User.Id)
            {
                // Case 1: Search by channel successful, user ID does not match. Create new entry for new user.
                // Create a new menu in the current channel.
                await First_Use_Intro_Load((SocketTextChannel)command.Channel, (SocketGuildUser)command.User);
                return;
            }
            // Else, if the channel entry exists and the user is the same, assume they want to reset the menu and delete the previous entry.
            else if (channelSearch != null && channelSearch.User.Id == command.User.Id)
            {
                // Case 2: Search by channel successful, user ID matches. Resetting menu in same channel.
                // Attempt deleting the message if it hasn't been deleted by the user yet.
                try
                {
                    // Delete the currently active menu.
                    await channelSearch.MenuMessage.DeleteAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                // Stop the timeout timer associated with the menu.
                channelSearch.MenuTimer.Stop();

                // Remove the menu entry from the global list.
                Global.MenuIdList.Remove(channelSearch);
                Global.ContentFilterList.Remove(filterSession);

                // Create a new menu in the current channel.
                await First_Use_Intro_Load((SocketTextChannel)command.Channel, (SocketGuildUser)command.User);
                return;
            }
            // Else, if an entry exists where the user is found but they're in a different channel now, delete previous entry and reset the menu.
            else if (userSearch != null && userSearch.MenuMessage.Channel.Id != command.Channel.Id)
            {
                // Case 3: Search by user successful, channel ID does not match. Resetting menu in new channel.
                // Attempt deleting the message if it hasn't been deleted by the user yet.
                try
                {
                    // Delete the currently active menu.
                    await userSearch.MenuMessage.DeleteAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                // Stop the timeout timer associated with the menu.
                userSearch.MenuTimer.Stop();

                // Remove the menu entry from the global list.
                Global.MenuIdList.Remove(userSearch);
                Global.ContentFilterList.Remove(filterSession);

                // Create a new menu in the current channel.
                await First_Use_Intro_Load((SocketTextChannel)command.Channel, (SocketGuildUser)command.User);
                return;
            }
            // For any other condition (if one should exist and not be handled here), create a new menu entry.
            else
            {
                // Case 4: No previous entry found. Create new entry.
                // Create a new menu in the current channel.
                await First_Use_Intro_Load((SocketTextChannel)command.Channel, (SocketGuildUser)command.User);
                return;
            }
        }

        public static async Task First_Use_Intro_Load(SocketTextChannel channel, SocketGuildUser user)
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

            // Create a new menu identifier entry for this current message and user to keep track of the overall menu status.
            var menuSession = new MenuIdStructure()
            {
                User = user,
                MenuMessage = message,
                CurrentMenu = "First_Use_Intro_Load",
                MenuTimer = new Timer()
                {
                    // Create a timer that expires as a "time out" duration for the user.
                    Interval = MenuConfig.menu.timerDuration,
                    AutoReset = false,
                    Enabled = true
                }
            };

            // Add the menu entry to the global list.
            Global.MenuIdList.Add(menuSession);

            // Create a new content filter identifier entry for this current session and user to keep track of the overall status.
            var filterSession = new ContentFilter()
            {
                User = user
            };

            // Add the filter session to the global list.
            Global.ContentFilterList.Add(filterSession);

            // Create a new menu in the current channel.
            await First_Use_Intro_Main(menuSession.User, menuSession.MenuMessage);
        }

        //public static async Task First_Use_Content_Filter_Main(SocketGuildUser user, RestUserMessage message)
        //{
        //    // Get the account information of the command's user.
        //    var account = UserInfoClasses.GetAccount(user);

        //    // Find the menu session associated with the current user.
        //    var menuSession = Global.MenuIdList.SingleOrDefault(x => x.User.Id == user.Id);

        //    // Find the filter session associated with the current user.
        //    var filterSession = Global.ContentFilterList.SingleOrDefault(x => x.User.Id == user.Id);

        //    // In case the user backtracks to this menu, set the values to activate all the other interactive menus and title options to false.
        //    filterSession.P1_Select = false;
        //    filterSession.P2IS_Select = false;
        //    filterSession.P2EP_Select = false;
        //    filterSession.P3_Select = false;
        //    filterSession.P4_Select = false;
        //    filterSession.P4AU_Select = false;
        //    filterSession.P4D_Select = false;
        //    filterSession.P5_Select = false;
        //    filterSession.BBTAG_Select = false;
        //    filterSession.P5S_Select = false;

        //    var embed = new EmbedBuilder();
        //    var author = new EmbedAuthorBuilder
        //    {
        //        Name = "First time using Social Linker?",
        //        IconUrl = user.GetAvatarUrl()
        //    };

        //    embed.WithAuthor(author);

        //    var footer = new EmbedFooterBuilder
        //    {
        //        Text = "✅ Confirm"
        //    };

        //    embed.WithFooter(footer);

        //    embed.WithThumbnailUrl("https://i.imgur.com/L0K5pNh.png");

        //    embed.WithDescription("" +
        //        ":warning: **__PLEASE READ CAREFULLY BEFORE CONTINUING!__** :warning:\n" +
        //        "\n" +
        //        "Social Linker contains content from all across the Persona series, so it might be easy to spoil yourself if you’re actively avoiding certain titles.\n" +
        //        "\n" +
        //        "**Select the games you want to __avoid spoilers for__ by reacting with their icons below.**\n" +
        //        "You’ll receive a warning message whenever related content is accessed to prevent you from accidentally viewing it. " +
        //        "This won’t prevent other users around you from accessing such content, however.\n" +
        //        "\n" +
        //        "Once you’ve finished selecting unwanted titles (or if you wish to choose nothing), select ✅ to continue.\n" +
        //        "You can change these settings later on whenever you wish to.\n" +
        //        "\n" +
        //        "<:P1:751133115531133112> **Persona**\n" +
        //        "<:P2IS:788950080396328990> **Persona 2: Innocent Sin**\n" +
        //        "<:P2EP:788950163363463172> **Persona 2: Eternal Punishment**\n" +
        //        "<:P3:751133114918633483> **Persona 3**\n" +
        //        "<:P4:751133120530612274> **Persona 4**\n" +
        //        "<:P4AU:751133122342420572> **Persona 4 Arena Ultimax**\n" +
        //        "<:P4D:751133120346062859> **Persona 4: Dancing All Night**\n" +
        //        "<:P5:751133123861020742> **Persona 5**\n" +
        //        "<:P5S:852644176188669972> **Persona 5 Strikers**\n" +
        //        "<:BBTAG:751133123013771617> **BlazBlue: Cross Tag Battle**\n");

        //    // Attempt editing the message if it hasn't been deleted by the user yet.
        //    // If it has, catch the exception, remove the menu entry from the global list, and return.
        //    try
        //    {
        //        // Remove all reactions from the current message.
        //        await message.RemoveAllReactionsAsync();

        //        // Edit the current active message by replacing it with the recently created embed.
        //        await message.ModifyAsync(x => {
        //            x.Embed = embed.Build();
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex);
        //        await message.DeleteAsync();
        //        await ErrorHandling.PermissionCheck(message);

        //        // Remove the menu and filter entries from the global list.
        //        Global.MenuIdList.Remove(menuSession);
        //        Global.ContentFilterList.Remove(filterSession);

        //        return;
        //    }

        //    // Edit the menu session according to the current message.
        //    menuSession.CurrentMenu = "First_Use_Content_Filter_Main";
        //    menuSession.MenuTimer = new Timer()
        //    {
        //        // Create a timer that expires as a "time out" duration for the user.
        //        Interval = MenuConfig.menu.timerDuration,
        //        AutoReset = false,
        //        Enabled = true
        //    };

        //    // If the menu timer runs out, activate a function.
        //    menuSession.MenuTimer.Elapsed += (sender, e) => MenuTimer_Elapsed(sender, e, menuSession);

        //    // Create an empty list for reactions.
        //    List<IEmote> reaction_list = new List<IEmote> { };

        //    // Add needed emote reactions for the menu.
        //    reaction_list.Add(Emote.Parse("<:P1:751133115531133112>"));
        //    reaction_list.Add(Emote.Parse("<:P2IS:788950080396328990>"));
        //    reaction_list.Add(Emote.Parse("<:P2EP:788950163363463172>"));
        //    reaction_list.Add(Emote.Parse("<:P3:751133114918633483>"));
        //    reaction_list.Add(Emote.Parse("<:P4:751133120530612274>"));
        //    reaction_list.Add(Emote.Parse("<:P4AU:751133122342420572>"));
        //    reaction_list.Add(Emote.Parse("<:P4D:751133120346062859>"));
        //    reaction_list.Add(Emote.Parse("<:P5:751133123861020742>"));
        //    reaction_list.Add(Emote.Parse("<:P5S:852644176188669972>"));
        //    reaction_list.Add(Emote.Parse("<:BBTAG:751133123013771617>"));
        //    reaction_list.Add(new Emoji("✅"));

        //    // Add the reactions to the message.
        //    _ = ReactionHandling.AddReactionsToMenu(message, reaction_list);
        //}

        public static async Task First_Use_Intro_Main(SocketGuildUser user, RestUserMessage message)
        {
            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            // Find the menu session associated with the current user.
            var menuSession = Global.MenuIdList.SingleOrDefault(x => x.User.Id == user.Id);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "First time using Social Linker?",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            embed.WithThumbnailUrl("https://i.imgur.com/L0K5pNh.png");

            embed.WithDescription("" +
                "Here's a quick guide to start!\n" +
                "\n" +
                "🔷 Use the **`help`** command for guides on how to use features like the scene maker and status screens.\n" +
                "\n" +
                "🔷 Use the **`settings`** command to customize your Social Linker profile and change how your scene maker images appear.\n" +
                "\n" +
                "🔷 Avoiding spoilers? Go to [Profile Settings] under the **`settings`** menu to filter out content for specific titles.\n" +
                "\n" +
                "That's all! Press the button to unlock Social Linker and start using commands!\n");

            var component = new ComponentBuilder()
                .WithButton("Confirm", customId: "first-use-intro-confirm", ButtonStyle.Primary);

            try
            {
                // Remove all reactions from the current message.
                await message.RemoveAllReactionsAsync();

                // Edit the current active message by replacing it with the recently created embed.
                await message.ModifyAsync(x => {
                    x.Embed = embed.Build();
                    x.Components = component.Build();
                });
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

            // Edit the menu session according to the current message.
            menuSession.CurrentMenu = "First_Use_Intro_Main";
            menuSession.MenuTimer = new Timer()
            {
                // Create a timer that expires as a "time out" duration for the user.
                Interval = MenuConfig.menu.timerDuration,
                AutoReset = false,
                Enabled = true
            };

            // If the menu timer runs out, activate a function.
            menuSession.MenuTimer.Elapsed += (sender, e) => MenuTimer_Elapsed(sender, e, menuSession);
        }

        public static async Task First_Use_Intro_Confirm(MenuIdStructure menuSession)
        {
            var account = UserInfoClasses.GetAccount(menuSession.User);
            var message = menuSession.MenuMessage;
            var user = menuSession.User;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Welcome to Social Linker!",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            embed.WithThumbnailUrl("https://i.imgur.com/L0K5pNh.png");

            embed.WithDescription("" +
                "You're all set! Have fun!\n");

            var component = new ComponentBuilder()
                .WithButton("Help Menu", customId: "go-to-help-menu", ButtonStyle.Secondary)
                .WithButton("Settings Menu", customId: "go-to-settings-menu", ButtonStyle.Secondary)
                .WithButton("Set Profile Theme", customId: "go-to-set-profile-theme", ButtonStyle.Secondary);

            try
            {
                // Remove all reactions from the current message.
                await message.RemoveAllReactionsAsync();

                // Edit the current active message by replacing it with the recently created embed.
                await message.ModifyAsync(x => {
                    x.Embed = embed.Build();
                    x.Components = component.Build();
                });
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

            // Edit the menu session according to the current message.
            menuSession.CurrentMenu = "First_Use_Intro_Confirm";
            menuSession.MenuTimer = new Timer()
            {
                // Create a timer that expires as a "time out" duration for the user.
                Interval = MenuConfig.menu.timerDuration,
                AutoReset = false,
                Enabled = true
            };

            // If the menu timer runs out, activate a function.
            menuSession.MenuTimer.Elapsed += (sender, e) => MenuTimer_Elapsed(sender, e, menuSession);
        }

        //public static async Task First_Use_Intro_Confirm(MenuIdStructure menuSession)
        //{
        //    await menuSession.MenuMessage.DeleteAsync();
        //    await Help_Menu.Help_Start((SocketTextChannel)menuSession.MenuMessage.Channel, menuSession.User);
        //    return;
        //}

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

            embed.WithDescription("Use a Social Linker command again to set up your first time usage settings.");
            return embed;
        }
    }
}
