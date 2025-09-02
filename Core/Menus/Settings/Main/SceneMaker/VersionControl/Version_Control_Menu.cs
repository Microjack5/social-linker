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

namespace SocialLinker.Core.Menus.Settings.Main.SceneMaker
{
    class Version_Control_Menu
    {
        public static async Task Version_Control_Main(SocketGuildUser user, RestUserMessage message)
        {
            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            // Find the menu session associated with the current user.
            var menuSession = Global.MenuIdList.SingleOrDefault(x => x.User.Id == user.Id);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Version Control",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            var footer = new EmbedFooterBuilder
            {
                Text = "↩️ Return to Scene Maker Settings"
            };

            embed.WithFooter(footer);

            // Determine the color and thumbnail for the embeded message
            EmbedSettings.Get_Profile_Embed_Color(account);
            EmbedSettings.Get_Profile_Config_Thumbnail(account);

            embed.WithDescription("" +
                "Choose a title to view the version control settings for.\n" +
                "\n" +
                "<:P1:751133115531133112> **Persona**\n" +
                "<:P2IS:788950080396328990> **Persona 2: Innocent Sin**\n" +
                "<:P2EP:788950163363463172> **Persona 2: Eternal Punishment**\n" +
                "<:P3:751133114918633483> **Persona 3**\n" +
                "<:P4:751133120530612274> **Persona 4**\n" +
                "<:P5:751133123861020742> **Persona 5**\n");

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
            menuSession.CurrentMenu = "Version_Control_Main";
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
            reaction_list.Add(Emote.Parse("<:P2IS:788950080396328990>"));
            reaction_list.Add(Emote.Parse("<:P2EP:788950163363463172>"));
            reaction_list.Add(Emote.Parse("<:P3:751133114918633483>"));
            reaction_list.Add(Emote.Parse("<:P4:751133120530612274>"));
            reaction_list.Add(Emote.Parse("<:P5:751133123861020742>"));

            // Add the reactions to the message.
            _ = ReactionHandling.AddReactionsToMenu(message, reaction_list);
        }

        public static async Task Version_Control_P1(SocketGuildUser user, RestUserMessage message)
        {
            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            // Find the menu session associated with the current user.
            var menuSession = Global.MenuIdList.SingleOrDefault(x => x.User.Id == user.Id);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Version Control - Persona",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            var footer = new EmbedFooterBuilder
            {
                Text = "↩️ Return to Version Control Settings"
            };

            embed.WithFooter(footer);

            // Assign a color and thumbnail to the embeded message based on the title being edited.
            embed.WithColor(EmbedSettings.Get_Game_Color("P1-PSP", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P1-PSP"));

            // Create an empty string variable.
            string version_title = "";

            // Depending on the user's version control settings, assign the string variable with a proper version title.
            if (account.VC_P1 == "P1-PS1")
            {
                version_title = "Revelations: Persona";
            }
            else if (account.VC_P1 == "P1-PSP")
            {
                version_title = "Persona (PSP™)";
            }

            // Use the version_title string in the embed's description.
            embed.WithDescription("" +
                "Select the default version you would like to use.\n" +
                "\n" +
                $"⚙️ **Current Setting:** **`{version_title}`**\n" +
                $"\n" +
                $":one: **Revelations: Persona**\n" +
                $":two: **Persona (PSP™)**");

            // Embed the user tutorial image for the menu.
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
            menuSession.CurrentMenu = "Version_Control_P1";
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

        public static async Task Version_Control_P2IS(SocketGuildUser user, RestUserMessage message)
        {
            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            // Find the menu session associated with the current user.
            var menuSession = Global.MenuIdList.SingleOrDefault(x => x.User.Id == user.Id);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Version Control - Persona 2: Innocent Sin",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            var footer = new EmbedFooterBuilder
            {
                Text = "↩️ Return to Version Control Settings"
            };

            embed.WithFooter(footer);

            // Assign a color and thumbnail to the embeded message based on the title being edited.
            embed.WithColor(EmbedSettings.Get_Game_Color("P2IS-PSP", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P2IS-PSP"));

            // Create an empty string variable.
            string version_title = "";

            // Depending on the user's version control settings, assign the string variable with a proper version title.
            if (account.VC_P2IS == "P2IS-PS1")
            {
                version_title = "Persona 2: Innocent Sin (PlayStation®️)";
            }
            else if (account.VC_P2IS == "P2IS-PSP")
            {
                version_title = "Persona 2: Innocent Sin (PSP™)";
            }

            // Use the version_title string in the embed's description.
            embed.WithDescription("" +
                "Select the default version you would like to use.\n" +
                "\n" +
                $"⚙️ **Current Setting:** **`{version_title}`**\n" +
                $"\n" +
                $":one: Persona 2: Innocent Sin (PlayStation®️)\n" +
                $":two: Persona 2: Innocent Sin (PSP™)");

            // Embed the user tutorial image for the menu.
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
            menuSession.CurrentMenu = "Version_Control_P2IS";
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

        public static async Task Version_Control_P2EP(SocketGuildUser user, RestUserMessage message)
        {
            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            // Find the menu session associated with the current user.
            var menuSession = Global.MenuIdList.SingleOrDefault(x => x.User.Id == user.Id);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Version Control - Persona 2: Eternal Punishment",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            var footer = new EmbedFooterBuilder
            {
                Text = "↩️ Return to Version Control Settings"
            };

            embed.WithFooter(footer);

            // Assign a color and thumbnail to the embeded message based on the title being edited.
            embed.WithColor(EmbedSettings.Get_Game_Color("P2EP-PSP", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P2EP-PSP"));

            // Create an empty string variable.
            string version_title = "";

            // Depending on the user's version control settings, assign the string variable with a proper version title.
            if (account.VC_P2EP == "P2EP-PS1")
            {
                version_title = "Persona 2: Eternal Punishment (PlayStation®️)";
            }
            else if (account.VC_P2EP == "P2EP-PSP")
            {
                version_title = "Persona 2: Eternal Punishment (PSP™)";
            }

            // Use the version_title string in the embed's description.
            embed.WithDescription("" +
                "Select the default version you would like to use.\n" +
                "\n" +
                $"⚙️ **Current Setting:** **`{version_title}`**\n" +
                $"\n" +
                $":one: Persona 2: Eternal Punishment (PlayStation®️)\n" +
                $":two: Persona 2: Eternal Punishment (PSP™)");

            // Embed the user tutorial image for the menu.
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
            menuSession.CurrentMenu = "Version_Control_P2EP";
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

        public static async Task Version_Control_P3(SocketGuildUser user, RestUserMessage message)
        {
            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            // Find the menu session associated with the current user.
            var menuSession = Global.MenuIdList.SingleOrDefault(x => x.User.Id == user.Id);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Version Control - Persona 3",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            var footer = new EmbedFooterBuilder
            {
                Text = "↩️ Return to Version Control Settings"
            };

            embed.WithFooter(footer);

            // Assign a color and thumbnail to the embeded message based on the title being edited.
            embed.WithColor(EmbedSettings.Get_Game_Color("P3F", null));
            embed.WithThumbnailUrl("https://i.imgur.com/trtPflx.png");

            // Create an empty string variable.
            string version_title = "";

            // Depending on the user's version control settings, assign the string variable with a proper version title.
            if (account.VC_P3 == "P3F")
            {
                version_title = "Persona 3 FES";
            }
            else if (account.VC_P3 == "P3P")
            {
                version_title = "Persona 3 Portable";
            }

            // Use the version_title string in the embed's description.
            embed.WithDescription("" +
                "Select the default version you would like to use.\n" +
                "\n" +
                $"⚙️ **Current Setting:** **`{version_title}`**\n" +
                $"\n" +
                $":one: Persona 3 FES\n" +
                $":two: Persona 3 Portable");

            // Embed the user tutorial image for the menu.
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
            menuSession.CurrentMenu = "Version_Control_P3";
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

        public static async Task Version_Control_P4(SocketGuildUser user, RestUserMessage message)
        {
            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            // Find the menu session associated with the current user.
            var menuSession = Global.MenuIdList.SingleOrDefault(x => x.User.Id == user.Id);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Version Control - Persona 4",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            var footer = new EmbedFooterBuilder
            {
                Text = "↩️ Return to Version Control Settings"
            };

            embed.WithFooter(footer);

            // Assign a color and thumbnail to the embeded message based on the title being edited.
            embed.WithColor(EmbedSettings.Get_Game_Color("P4-PS2", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P4-PS2"));

            // Create an empty string variable.
            string version_title = "";

            // Depending on the user's version control settings, assign the string variable with a proper version title.
            if (account.VC_P4 == "P4-PS2")
            {
                version_title = "Persona 4 (PlayStation®️ 2)";
            }
            else if (account.VC_P4 == "P4G")
            {
                version_title = "Persona 4 Golden";
            }

            // Use the version_title string in the embed's description.
            embed.WithDescription("" +
                "Select the default version you would like to use.\n" +
                "\n" +
                $"⚙️ **Current Setting:** **`{version_title}`**\n" +
                $"\n" +
                $":one: Persona 4 (PlayStation®️ 2)\n" +
                $":two: Persona 4 Golden");

            // Embed the user tutorial image for the menu.
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
            menuSession.CurrentMenu = "Version_Control_P4";
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

        public static async Task Version_Control_P5(SocketGuildUser user, RestUserMessage message)
        {
            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            // Find the menu session associated with the current user.
            var menuSession = Global.MenuIdList.SingleOrDefault(x => x.User.Id == user.Id);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Version Control - Persona 5",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            var footer = new EmbedFooterBuilder
            {
                Text = "↩️ Return to Version Control Settings"
            };

            embed.WithFooter(footer);

            // Assign a color and thumbnail to the embeded message based on the title being edited.
            embed.WithColor(EmbedSettings.Get_Game_Color("P5-PS4", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P5-PS4"));

            // Create an empty string variable.
            string version_title = "";

            // Depending on the user's version control settings, assign the string variable with a proper version title.
            if (account.VC_P5 == "P5-PS4")
            {
                version_title = "Persona 5 (PlayStation®️ 4)";
            }
            else if (account.VC_P5 == "P5R")
            {
                version_title = "Persona 5 Royal";
            }

            // Use the version_title string in the embed's description.
            embed.WithDescription("" +
                "Select the default version you would like to use.\n" +
                "\n" +
                $"⚙️ **Current Setting:** **`{version_title}`**\n" +
                $"\n" +
                $":one: Persona 5 (PlayStation®️ 4)\n" +
                $":two: Persona 5 Royal");

            // Embed the user tutorial image for the menu.
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
            menuSession.CurrentMenu = "Version_Control_P5";
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

        public static async Task Version_Control_P1_Confirm(SocketGuildUser user, RestUserMessage message)
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
                Text = "💠 Return to Version Control Settings | ❌ Close Menu"
            };

            embed.WithFooter(footer);

            // Assign a color to the embeded message based on the title being edited.
            embed.WithColor(EmbedSettings.Get_Game_Color("P1-PSP", null));

            // Create an empty string variable.
            string version_title = "";

            // Depending on the user's version control settings, assign the string variable with a proper version title and decide the thumbnail to be attached.
            if (account.VC_P1 == "P1-PS1")
            {
                version_title = "Revelations: Persona";
                embed.WithThumbnailUrl("https://i.imgur.com/VDFzfIt.jpg");
            }
            else if (account.VC_P1 == "P1-PSP")
            {
                version_title = "Persona (PSP™)";
                embed.WithThumbnailUrl("https://i.imgur.com/V3GQl38.jpg");
            }

            // Use the version_title string in the embed's description.
            embed.WithDescription("" +
                $"Your version control settings for Persona has been set to **`{version_title}`**.");

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
            menuSession.CurrentMenu = "Version_Control_P1_Confirm";
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

        public static async Task Version_Control_P2IS_Confirm(SocketGuildUser user, RestUserMessage message)
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
                Text = "💠 Return to Version Control Settings | ❌ Close Menu"
            };

            embed.WithFooter(footer);

            // Assign a color to the embeded message based on the title being edited.
            embed.WithColor(EmbedSettings.Get_Game_Color("P2IS-PSP", null));

            // Create an empty string variable.
            string version_title = "";

            // Depending on the user's version control settings, assign the string variable with a proper version title and decide the thumbnail to be attached.
            if (account.VC_P2IS == "P2IS-PS1")
            {
                version_title = "Persona 2: Innocent Sin (PlayStation®️)";
                embed.WithThumbnailUrl("https://i.imgur.com/PZAMFPy.jpg");
            }
            else if (account.VC_P2IS == "P2IS-PSP")
            {
                version_title = "Persona 2: Innocent Sin (PSP™)";
                embed.WithThumbnailUrl("https://i.imgur.com/yMcba0F.jpg");
            }

            // Use the version_title string in the embed's description.
            embed.WithDescription("" +
                $"Your version control settings for Persona 2: Innocent Sin has been set to **`{version_title}`**.");

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
            menuSession.CurrentMenu = "Version_Control_P2IS_Confirm";
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

        public static async Task Version_Control_P2EP_Confirm(SocketGuildUser user, RestUserMessage message)
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
                Text = "💠 Return to Version Control Settings | ❌ Close Menu"
            };

            embed.WithFooter(footer);

            // Assign a color to the embeded message based on the title being edited.
            embed.WithColor(EmbedSettings.Get_Game_Color("P2EP-PSP", null));

            // Create an empty string variable.
            string version_title = "";

            // Depending on the user's version control settings, assign the string variable with a proper version title and decide the thumbnail to be attached.
            if (account.VC_P2EP == "P2EP-PS1")
            {
                version_title = "Persona 2: Eternal Punishment (PlayStation®️)";
                embed.WithThumbnailUrl("https://i.imgur.com/KVj8Tqt.jpg");
            }
            else if (account.VC_P2EP == "P2EP-PSP")
            {
                version_title = "Persona 2: Eternal Punishment (PSP™)";
                embed.WithThumbnailUrl("https://i.imgur.com/x04Ldby.jpg");
            }

            // Use the version_title string in the embed's description.
            embed.WithDescription("" +
                $"Your version control settings for Persona 2: Eternal Punishment has been set to **`{version_title}`**.");

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
            menuSession.CurrentMenu = "Version_Control_P2EP_Confirm";
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

        public static async Task Version_Control_P3_Confirm(SocketGuildUser user, RestUserMessage message)
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
                Text = "💠 Return to Version Control Settings | ❌ Close Menu"
            };

            embed.WithFooter(footer);

            // Create an empty string variable.
            string version_title = "";

            // Depending on the user's version control settings, assign the string variable with a proper version title and decide the thumbnail to be attached.
            if (account.VC_P3 == "P3F")
            {
                version_title = "Persona 3 FES";
                embed.WithThumbnailUrl("https://i.imgur.com/LX1PSeP.jpg");
                embed.WithColor(EmbedSettings.Get_Game_Color("P3F", null));
            }
            else if (account.VC_P3 == "P3P")
            {
                version_title = "Persona 3 Portable";
                embed.WithThumbnailUrl("https://i.imgur.com/sNrtgFX.jpg");
                embed.WithColor(EmbedSettings.Get_Game_Color("P3P", account));
            }

            // Use the version_title string in the embed's description.
            embed.WithDescription("" +
                $"Your version control settings for Persona 3 has been set to **`{version_title}`**.");

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
            menuSession.CurrentMenu = "Version_Control_P3_Confirm";
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

        public static async Task Version_Control_P4_Confirm(SocketGuildUser user, RestUserMessage message)
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
                Text = "💠 Return to Version Control Settings | ❌ Close Menu"
            };

            embed.WithFooter(footer);

            // Assign a color to the embeded message based on the title being edited.
            embed.WithColor(EmbedSettings.Get_Game_Color("P4G", null));

            // Create an empty string variable.
            string version_title = "";

            // Depending on the user's version control settings, assign the string variable with a proper version title and decide the thumbnail to be attached.
            if (account.VC_P4 == "P4-PS2")
            {
                version_title = "Persona 4 (PlayStation®️ 2)";
                embed.WithThumbnailUrl("https://i.imgur.com/mXi0b8j.jpg");
            }
            else if (account.VC_P4 == "P4G")
            {
                version_title = "Persona 4 Golden";
                embed.WithThumbnailUrl("https://i.imgur.com/bVbgkST.png");
            }

            // Use the version_title string in the embed's description.
            embed.WithDescription("" +
                $"Your version control settings for Persona 4 has been set to **`{version_title}`**.");

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
            menuSession.CurrentMenu = "Version_Control_P4_Confirm";
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

        public static async Task Version_Control_P5_Confirm(SocketGuildUser user, RestUserMessage message)
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
                Text = "💠 Return to Version Control Settings | ❌ Close Menu"
            };

            embed.WithFooter(footer);

            // Assign a color to the embeded message based on the title being edited.
            embed.WithColor(EmbedSettings.Get_Game_Color("P5-PS4", null));

            // Create an empty string variable.
            string version_title = "";

            // Depending on the user's version control settings, assign the string variable with a proper version title and decide the thumbnail to be attached.
            if (account.VC_P5 == "P5-PS4")
            {
                version_title = "Persona 5 (PlayStation®️ 4)";
                embed.WithThumbnailUrl("https://i.imgur.com/K78my5p.jpg");
            }
            else if (account.VC_P5 == "P5R")
            {
                version_title = "Persona 5 Royal";
                embed.WithThumbnailUrl("https://i.imgur.com/auKURsG.jpg");
            }

            // Use the version_title string in the embed's description.
            embed.WithDescription("" +
                $"Your version control settings for Persona 5 has been set to **`{version_title}`**.");

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
            menuSession.CurrentMenu = "Version_Control_P5_Confirm";
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

            embed.WithDescription($"You can edit your version control settings at any time from the **`settings`** menu by choosing [Scene Maker Settings] > [Version Control].");
            return embed;
        }
    }
}
