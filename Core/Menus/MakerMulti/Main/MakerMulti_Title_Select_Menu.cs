using Discord.Rest;
using Discord;
using Discord.WebSocket;
using SocialLinker.Config;
using SocialLinker.Core.CloudStorageTables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using SocialLinker.Core.SceneMaker;

namespace SocialLinker.Core.Menus.MakerMulti.Main
{
    class MakerMulti_Title_Select_Menu
    {
        public static async Task MakerMulti_Start(SocialLinkerCommand sl_command)
        {
            var channel = (SocketTextChannel)sl_command.Channel;
            var user = (SocketGuildUser)sl_command.User;

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
            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));

            var multimaker_session = Global.MultiMaker_Session_List.SingleOrDefault(x => x.User.Id == user.Id);

            if (multimaker_session != null)
            {
                Global.MultiMaker_Session_List.Remove(multimaker_session);
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

            // Create a new menu identifier entry for this current message and user to keep track of the overall menu status.
            var menuSession = new MenuIdStructure()
            {
                User = user,
                MenuMessage = message,
                CurrentMenu = "MakerMulti_Start",
                MenuTimer = new Timer()
                {
                    // Create a timer that expires as a "time out" duration for the user.
                    Interval = MenuConfig.menu.timerDuration,
                    AutoReset = false,
                    Enabled = true
                }
            };

            MakerCommandData maker_command_data = new MakerCommandData()
            {
                Template = "",
                Character_Data_1 = new MakerCharacterData(),
                Character_Data_2 = new MakerCharacterData(),
                Character_Data_3 = new MakerCharacterData(),
                Character_Data_4 = new MakerCharacterData(),
                Dialogue = "",
                Background = null,
                Expected_Characters = 0,
                Display_Name = "",
                Display_Name_Has_Invalid_Char = false,
                Dialogue_Has_Invalid_Char = false
            };

            sl_command.MakerCommand = maker_command_data;

            // Add the menu entry to the global list.
            Global.MenuIdList.Add(menuSession);
            Global.MultiMaker_Session_List.Add(sl_command);

            // Create a new menu in the current channel.
            await MakerMulti_Main_Menu(menuSession.User, menuSession.MenuMessage);
        }

        public static async Task MakerMulti_Main_Menu(SocketGuildUser user, RestUserMessage message)
        {
            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            // Find the menu session associated with the current user.
            var menuSession = Global.MenuIdList.SingleOrDefault(x => x.User.Id == user.Id);
            var multimaker_session = Global.MultiMaker_Session_List.SingleOrDefault(x => x.User.Id == user.Id);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Multi-character Scene Maker",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            var footer = new EmbedFooterBuilder
            {
                Text = "❌ Close Menu"
            };

            embed.WithFooter(footer);

            embed.WithDescription("" +
                "Quickly create a scene with multiple characters based on your existing scene maker settings. Choose a compatible title to start.\n" +
                "\n" +
                "<:P2IS:788950080396328990> **Persona 2: Innocent Sin**\n" +
                "<:P2EP:788950163363463172> **Persona 2: Eternal Punishment**\n" +
                "<:P3P:1096338602046267392> **Persona 3 Portable**\n" +
                "<:P4AU:751133122342420572> **Persona 4 Arena Ultimax**\n" +
                "<:P4D:751133120346062859> **Persona 4: Dancing All Night**\n" +
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

                // Remove the menu entry from the global list.
                Global.MenuIdList.Remove(menuSession);

                return;
            }

            // Edit the menu session according to the current message.
            menuSession.CurrentMenu = "MakerMulti_Main_Menu";
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
            reaction_list.Add(Emote.Parse("<:P2IS:788950080396328990>"));
            reaction_list.Add(Emote.Parse("<:P2EP:788950163363463172>"));
            reaction_list.Add(Emote.Parse("<:P3P:1096338602046267392>"));
            reaction_list.Add(Emote.Parse("<:P4AU:751133122342420572>"));
            reaction_list.Add(Emote.Parse("<:P4D:751133120346062859>"));
            reaction_list.Add(Emote.Parse("<:BBTAG:751133123013771617>"));
            reaction_list.Add(new Emoji("❌"));

            // Add the reactions to the message.
            _ = ReactionHandling.AddReactionsToMenu(message, reaction_list);
        }

        public static async Task MakerMulti_VC_P2IS_Main(SocketGuildUser user, RestUserMessage message)
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
                Text = "↩️ Return to Previous Menu"
            };

            embed.WithFooter(footer);

            // Assign a color and thumbnail to the embeded message based on the title being edited.
            embed.WithColor(EmbedSettings.Get_Game_Color("P2IS-PSP", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P2IS-PSP"));

            embed.WithDescription("" +
                "**Which version of Persona 2: Innocent Sin would you like to use?**\n" +
                "\n" +
                ":one: Persona 2: Innocent Sin (PlayStation®️)\n" +
                ":two: Persona 2: Innocent Sin (Remake)");

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
            menuSession.CurrentMenu = "MakerMulti_VC_P2IS_Main";
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

        public static async Task MakerMulti_VC_P2EP_Main(SocketGuildUser user, RestUserMessage message)
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
                Text = "↩️ Return to Previous Menu"
            };

            embed.WithFooter(footer);

            // Assign a color and thumbnail to the embeded message based on the title being edited.
            embed.WithColor(EmbedSettings.Get_Game_Color("P2EP-PSP", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P2EP-PSP"));

            embed.WithDescription("" +
                "**Which version of Persona 2: Eternal Punishment would you like to use?**\n" +
                "\n" +
                ":one: Persona 2: Eternal Punishment (PlayStation®️)\n" +
                ":two: Persona 2: Eternal Punishment (Remake)");

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
            menuSession.CurrentMenu = "MakerMulti_VC_P2EP_Main";
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
