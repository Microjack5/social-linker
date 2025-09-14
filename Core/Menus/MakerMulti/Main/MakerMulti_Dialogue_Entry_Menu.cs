using Discord;
using Discord.WebSocket;
using SocialLinker.Config;
using SocialLinker.Core.CloudStorageTables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace SocialLinker.Core.Menus.MakerMulti.Main
{
    class MakerMulti_Dialogue_Entry_Menu
    {
        public static async Task MakerMulti_Display_Name_and_Dialogue_Entry_Main(MenuIdStructure menuSession)
        {
            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(menuSession.User);
            var message = menuSession.MenuMessage;
            var user = menuSession.User;

            var multimaker_session = Global.MultiMaker_Session_List.SingleOrDefault(x => x.User.Id == menuSession.User.Id);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Display Name & Dialogue",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            // Determine the color and thumbnail for the embeded message
            embed.WithColor(EmbedSettings.Get_Game_Color(multimaker_session.MakerCommand.Template, null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo(multimaker_session.MakerCommand.Template));

            embed.WithDescription("" +
                $"Next, Select [Enter Display Name & Dialogue] to specify the display name and what you want them to say.");

            var component = new ComponentBuilder()
                .WithButton("Enter Display Name & Dialogue", customId: "makermulti-display-name-and-dialogue-entry-modal-open", ButtonStyle.Primary);

            if (multimaker_session.MakerCommand.BBTAG_Specific_Data.Speaker == "offscreen")
            {
                component = component.WithButton("↩️ Return", customId: "back-to-multimaker-bbtag-offscreen-speaker-series", ButtonStyle.Secondary);
            }
            else
            {
                switch (multimaker_session.MakerCommand.Expected_Characters)
                {
                    case 1:
                        component = component.WithButton("↩️ Return", customId: "back-to-makermulti-1char-details", ButtonStyle.Secondary);
                        break;

                    case 2:
                        component = component.WithButton("↩️ Return", customId: "back-to-makermulti-2char-details", ButtonStyle.Secondary);
                        break;

                    case 3:
                        component = component.WithButton("↩️ Return", customId: "back-to-makermulti-3char-details", ButtonStyle.Secondary);
                        break;

                    case 4:
                        component = component.WithButton("↩️ Return", customId: "back-to-makermulti-4char-details", ButtonStyle.Secondary);
                        break;
                }
            }

            // Attempt editing the message if it hasn't been deleted by the user yet.
            // If it has, catch the exception, remove the menu entry from the global list, and return.
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
            menuSession.CurrentMenu = "MakerMulti_Display_Name_and_Dialogue_Entry_Main";
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

            // Add the reactions to the message.
            _ = ReactionHandling.AddReactionsToMenu(message, reaction_list);
        }

        public static async Task MakerMulti_Dialogue_Only_Entry_Main(MenuIdStructure menuSession)
        {
            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(menuSession.User);
            var message = menuSession.MenuMessage;
            var user = menuSession.User;

            var multimaker_session = Global.MultiMaker_Session_List.SingleOrDefault(x => x.User.Id == menuSession.User.Id);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Dialogue",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            // Determine the color and thumbnail for the embeded message
            embed.WithColor(EmbedSettings.Get_Game_Color(multimaker_session.MakerCommand.Template, null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo(multimaker_session.MakerCommand.Template));

            embed.WithDescription("" +
                $"Next, Select [Enter Dialogue] to specify what you want the character to say.");

            var component = new ComponentBuilder()
                .WithButton("Enter Dialogue", customId: "makermulti-dialogue-only-entry-modal-open", ButtonStyle.Primary)
                .WithButton("↩️ Return", customId: "back-to-makermulti-bbtag-speaker-select", ButtonStyle.Secondary);

            // Attempt editing the message if it hasn't been deleted by the user yet.
            // If it has, catch the exception, remove the menu entry from the global list, and return.
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
            menuSession.CurrentMenu = "MakerMulti_Dialogue_Only_Entry_Main";
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

            // Add the reactions to the message.
            _ = ReactionHandling.AddReactionsToMenu(message, reaction_list);
        }

        public static async Task MakerMulti_Display_Name_and_Dialogue_Details_Modal(SocketMessageComponent component)
        {
            // Get the account information of the command's user.
            if (component.Data.CustomId == "makermulti-display-name-and-dialogue-entry-modal-open")
            {
                try
                {
                    var modal = new ModalBuilder()
                    .WithTitle("Display Name & Dialogue")
                    .WithCustomId("makermulti-display-name-and-dialogue-entry-modal-submit")
                    .AddTextInput("Display Name", "display_name")
                    .AddTextInput("Dialogue", "dialogue");

                    await component.RespondWithModalAsync(modal.Build());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public static async Task MakerMulti_Dialogue_Only_Details_Modal(SocketMessageComponent component)
        {
            // Get the account information of the command's user.
            if (component.Data.CustomId == "makermulti-dialogue-only-entry-modal-open")
            {
                try
                {
                    var modal = new ModalBuilder()
                    .WithTitle("Dialogue")
                    .WithCustomId("makermulti-dialogue-only-entry-modal-submit")
                    .AddTextInput("Dialogue", "dialogue");

                    await component.RespondWithModalAsync(modal.Build());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
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
                    x.Components = new ComponentBuilder().Build();
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
