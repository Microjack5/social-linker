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

namespace SocialLinker.Core.Menus.InitialUsage.Main
{
    class First_Use_Content_Filter_Menu
    {
        public static async Task First_Use_Content_Filter_Initialize(SocialLinkerCommand command)
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
                await First_Use_Content_Filter_Load((SocketTextChannel)command.Channel, (SocketGuildUser)command.User);
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
                await First_Use_Content_Filter_Load((SocketTextChannel)command.Channel, (SocketGuildUser)command.User);
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
                await First_Use_Content_Filter_Load((SocketTextChannel)command.Channel, (SocketGuildUser)command.User);
                return;
            }
            // For any other condition (if one should exist and not be handled here), create a new menu entry.
            else
            {
                // Case 4: No previous entry found. Create new entry.
                // Create a new menu in the current channel.
                await First_Use_Content_Filter_Load((SocketTextChannel)command.Channel, (SocketGuildUser)command.User);
                return;
            }
        }

        public static async Task First_Use_Content_Filter_Load(SocketTextChannel channel, SocketGuildUser user)
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
                CurrentMenu = "First_Use_Content_Filter_Load",
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
            await First_Use_Content_Filter_Main(menuSession.User, menuSession.MenuMessage);
        }

        public static async Task First_Use_Content_Filter_Main(SocketGuildUser user, RestUserMessage message)
        {
            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            // Find the menu session associated with the current user.
            var menuSession = Global.MenuIdList.SingleOrDefault(x => x.User.Id == user.Id);

            // Find the filter session associated with the current user.
            var filterSession = Global.ContentFilterList.SingleOrDefault(x => x.User.Id == user.Id);

            // In case the user backtracks to this menu, set the values to activate all the other interactive menus and title options to false.
            filterSession.P1_Select = false;
            filterSession.P2IS_Select = false;
            filterSession.P2EP_Select = false;
            filterSession.P3_Select = false;
            filterSession.P4_Select = false;
            filterSession.P4AU_Select = false;
            filterSession.P4D_Select = false;
            filterSession.P5_Select = false;
            filterSession.BBTAG_Select = false;
            filterSession.P5S_Select = false;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "First time using Social Linker?",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            var footer = new EmbedFooterBuilder
            {
                Text = "✅ Confirm"
            };

            embed.WithFooter(footer);

            embed.WithThumbnailUrl("https://i.imgur.com/L0K5pNh.png");

            embed.WithDescription("" +
                ":warning: **__PLEASE READ CAREFULLY BEFORE CONTINUING!__** :warning:\n" +
                "\n" +
                "Social Linker contains content from all across the Persona series, so it might be easy to spoil yourself if you’re actively avoiding certain titles.\n" +
                "\n" +
                "**Select the games you want to __avoid spoilers for__ by reacting with their icons below.**\n" +
                "You’ll receive a warning message whenever related content is accessed to prevent you from accidentally viewing it. " +
                "This won’t prevent other users around you from accessing such content, however.\n" +
                "\n" +
                "Once you’ve finished selecting unwanted titles (or if you wish to choose nothing), select ✅ to continue.\n" +
                "You can change these settings later on whenever you wish to.\n" +
                "\n" +
                "<:P1:751133115531133112> **Persona**\n" +
                "<:P2IS:788950080396328990> **Persona 2: Innocent Sin**\n" +
                "<:P2EP:788950163363463172> **Persona 2: Eternal Punishment**\n" +
                "<:P3:751133114918633483> **Persona 3**\n" +
                "<:P4:751133120530612274> **Persona 4**\n" +
                "<:P4AU:751133122342420572> **Persona 4 Arena Ultimax**\n" +
                "<:P4D:751133120346062859> **Persona 4: Dancing All Night**\n" +
                "<:P5:751133123861020742> **Persona 5**\n" +
                "<:P5S:852644176188669972> **Persona 5 Strikers**\n" +
                "<:BBTAG:751133123013771617> **BlazBlue: Cross Tag Battle**\n");

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

                // Remove the menu and filter entries from the global list.
                Global.MenuIdList.Remove(menuSession);
                Global.ContentFilterList.Remove(filterSession);

                return;
            }

            // Edit the menu session according to the current message.
            menuSession.CurrentMenu = "First_Use_Content_Filter_Main";
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
            reaction_list.Add(Emote.Parse("<:P1:751133115531133112>"));
            reaction_list.Add(Emote.Parse("<:P2IS:788950080396328990>"));
            reaction_list.Add(Emote.Parse("<:P2EP:788950163363463172>"));
            reaction_list.Add(Emote.Parse("<:P3:751133114918633483>"));
            reaction_list.Add(Emote.Parse("<:P4:751133120530612274>"));
            reaction_list.Add(Emote.Parse("<:P4AU:751133122342420572>"));
            reaction_list.Add(Emote.Parse("<:P4D:751133120346062859>"));
            reaction_list.Add(Emote.Parse("<:P5:751133123861020742>"));
            reaction_list.Add(Emote.Parse("<:P5S:852644176188669972>"));
            reaction_list.Add(Emote.Parse("<:BBTAG:751133123013771617>"));
            reaction_list.Add(new Emoji("✅"));

            // Add the reactions to the message.
            _ = ReactionHandling.AddReactionsToMenu(message, reaction_list);
        }

        public static async Task First_Use_Content_Filter_VC_P1_Main(SocketGuildUser user, RestUserMessage message)
        {
            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            // Find the menu session associated with the current user.
            var menuSession = Global.MenuIdList.SingleOrDefault(x => x.User.Id == user.Id);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Version Select",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            var footer = new EmbedFooterBuilder
            {
                Text = "↩️ Return to Previous Menu | ✅ Confirm"
            };

            embed.WithFooter(footer);

            // Assign a color and thumbnail to the embeded message based on the title being edited.
            embed.WithColor(EmbedSettings.Get_Game_Color("P1-PSP", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P1-PSP"));

            embed.WithDescription("" +
                "**Which version of Persona would you like to block? Select all that apply, then react with ✅ to continue.**\n" +
                "\n" +
                ":one: Revelations: Persona\n" +
                ":two: Persona (PSP™)");

            embed.WithImageUrl("https://i.imgur.com/t4YH4rN.png");

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
            menuSession.CurrentMenu = "First_Use_Content_Filter_VC_P1_Main";
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
            reaction_list.Add(new Emoji("✅"));

            // Add the reactions to the message.
            _ = ReactionHandling.AddReactionsToMenu(message, reaction_list);
        }

        public static async Task First_Use_Content_Filter_VC_P2IS_Main(SocketGuildUser user, RestUserMessage message)
        {
            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            // Find the menu session associated with the current user.
            var menuSession = Global.MenuIdList.SingleOrDefault(x => x.User.Id == user.Id);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Version Select",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            var footer = new EmbedFooterBuilder
            {
                Text = "↩️ Return to Previous Menu | ✅ Confirm"
            };

            embed.WithFooter(footer);

            // Assign a color and thumbnail to the embeded message based on the title being edited.
            embed.WithColor(EmbedSettings.Get_Game_Color("P2IS-PSP", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P2IS-PSP"));

            embed.WithDescription("" +
                "**Which version of Persona 2: Innocent Sin would you like to block? Select all that apply, then react with ✅ to continue.**\n" +
                "\n" +
                ":one: Persona 2: Innocent Sin (PlayStation®️)\n" +
                ":two: Persona 2: Innocent Sin (PSP™)");

            embed.WithImageUrl("https://i.imgur.com/7oh20qY.png");

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
            menuSession.CurrentMenu = "First_Use_Content_Filter_VC_P2IS_Main";
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
            reaction_list.Add(new Emoji("✅"));

            // Add the reactions to the message.
            _ = ReactionHandling.AddReactionsToMenu(message, reaction_list);
        }

        public static async Task First_Use_Content_Filter_VC_P2EP_Main(SocketGuildUser user, RestUserMessage message)
        {
            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            // Find the menu session associated with the current user.
            var menuSession = Global.MenuIdList.SingleOrDefault(x => x.User.Id == user.Id);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Version Select",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            var footer = new EmbedFooterBuilder
            {
                Text = "↩️ Return to Previous Menu | ✅ Confirm"
            };

            embed.WithFooter(footer);

            // Assign a color and thumbnail to the embeded message based on the title being edited.
            embed.WithColor(EmbedSettings.Get_Game_Color("P2EP-PSP", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P2EP-PSP"));

            embed.WithDescription("" +
                "**Which version of Persona 2: Eternal Punishment would you like to block? Select all that apply, then react with ✅ to continue.**\n" +
                "\n" +
                ":one: Persona 2: Eternal Punishment (PlayStation®️)\n" +
                ":two: Persona 2: Eternal Punishment (PSP™)");

            embed.WithImageUrl("https://i.imgur.com/QPpK2TO.png");

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
            menuSession.CurrentMenu = "First_Use_Content_Filter_VC_P2EP_Main";
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
            reaction_list.Add(new Emoji("✅"));

            // Add the reactions to the message.
            _ = ReactionHandling.AddReactionsToMenu(message, reaction_list);
        }

        public static async Task First_Use_Content_Filter_VC_P3_Main(SocketGuildUser user, RestUserMessage message)
        {
            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            // Find the menu session associated with the current user.
            var menuSession = Global.MenuIdList.SingleOrDefault(x => x.User.Id == user.Id);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Version Select",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            var footer = new EmbedFooterBuilder
            {
                Text = "↩️ Return to Previous Menu | ✅ Confirm"
            };

            embed.WithFooter(footer);

            // Assign a color and thumbnail to the embeded message based on the title being edited.
            embed.WithColor(EmbedSettings.Get_Game_Color("P3F", null));
            embed.WithThumbnailUrl("https://i.imgur.com/trtPflx.png");

            embed.WithDescription("" +
                "**Which version of Persona 3 would you like to block? Select all that apply, then react with ✅ to continue.**\n" +
                "\n" +
                ":one: Persona 3 FES\n" +
                ":two: Persona 3 Portable");

            embed.WithImageUrl("https://i.imgur.com/hZJTcx4.png");

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
            menuSession.CurrentMenu = "First_Use_Content_Filter_VC_P3_Main";
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
            reaction_list.Add(new Emoji("✅"));

            // Add the reactions to the message.
            _ = ReactionHandling.AddReactionsToMenu(message, reaction_list);
        }

        public static async Task First_Use_Content_Filter_VC_P4_Main(SocketGuildUser user, RestUserMessage message)
        {
            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            // Find the menu session associated with the current user.
            var menuSession = Global.MenuIdList.SingleOrDefault(x => x.User.Id == user.Id);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Version Select",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            var footer = new EmbedFooterBuilder
            {
                Text = "↩️ Return to Previous Menu | ✅ Confirm"
            };

            embed.WithFooter(footer);

            // Assign a color and thumbnail to the embeded message based on the title being edited.
            embed.WithColor(EmbedSettings.Get_Game_Color("P4-PS2", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P4-PS2"));

            embed.WithDescription("" +
                "**Which version of Persona 4 would you like to block? Select all that apply, then react with ✅ to continue.**\n" +
                "\n" +
                ":one: Persona 4 (PlayStation®️ 2)\n" +
                ":two: Persona 4 Golden");

            embed.WithImageUrl("https://i.imgur.com/ZVldBKO.png");

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
            menuSession.CurrentMenu = "First_Use_Content_Filter_VC_P4_Main";
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
            reaction_list.Add(new Emoji("✅"));

            // Add the reactions to the message.
            _ = ReactionHandling.AddReactionsToMenu(message, reaction_list);
        }

        public static async Task First_Use_Content_Filter_VC_P5_Main(SocketGuildUser user, RestUserMessage message)
        {
            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            // Find the menu session associated with the current user.
            var menuSession = Global.MenuIdList.SingleOrDefault(x => x.User.Id == user.Id);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Version Select",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            var footer = new EmbedFooterBuilder
            {
                Text = "↩️ Return to Previous Menu | ✅ Confirm"
            };

            embed.WithFooter(footer);

            // Assign a color and thumbnail to the embeded message based on the title being edited.
            embed.WithColor(EmbedSettings.Get_Game_Color("P5-PS4", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P5-PS4"));

            embed.WithDescription("" +
                "**Which version of Persona 5 would you like to block? Select all that apply, then react with ✅ to continue.**\n" +
                "\n" +
                ":one: Persona 5 (PlayStation®️ 4)\n" +
                ":two: Persona 5 Royal");

            embed.WithImageUrl("https://i.imgur.com/7PMim5v.png");

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
            menuSession.CurrentMenu = "First_Use_Content_Filter_VC_P5_Main";
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
            reaction_list.Add(new Emoji("✅"));

            // Add the reactions to the message.
            _ = ReactionHandling.AddReactionsToMenu(message, reaction_list);
        }

        public static async Task First_Use_Content_Filter_Confirm(SocketGuildUser user, RestUserMessage message)
        {
            // Get the account information of the command's target
            var account = UserInfoClasses.GetAccount(user);

            // Find the menu session associated with the current user.
            var menuSession = Global.MenuIdList.SingleOrDefault(x => x.User.Id == user.Id);

            // Search for a content filter list that corresponds to the user's ID.
            var filterSession = Global.ContentFilterList.SingleOrDefault(x => x.User.Id == menuSession.User.Id);

            // Create a new embed that will be displayed in the message.
            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Settings Saved",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            embed.WithThumbnailUrl("https://i.imgur.com/L0K5pNh.png");

            embed.WithDescription($"" +
                $"You can adjust your content filter settings at any time from the **`settings`** menu by choosing [Profile Settings] > [Content Filter].");

            embed.AddField("Getting Started", "" +
                $"Use the **`help`** slash command to view various other commands and tutorials showing how to use Social Linker.");

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

            // Remove the menu entry from the global list.
            Global.MenuIdList.Remove(menuSession);

            // Remove the content filter entry from the global list.
            Global.ContentFilterList.Remove(filterSession);
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

            embed.WithDescription("Use a Social Linker command again to set up your first time usage settings.");
            return embed;
        }
    }
}
