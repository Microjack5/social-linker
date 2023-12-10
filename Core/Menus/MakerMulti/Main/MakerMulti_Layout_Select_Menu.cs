using Discord.Rest;
using Discord.WebSocket;
using Discord;
using SocialLinker.Config;
using SocialLinker.Core.CloudStorageTables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace SocialLinker.Core.Menus.MakerMulti.Main
{
    class MakerMulti_Layout_Select_Menu
    {
        public static async Task MakerMulti_Layout_Select_Main(SocketGuildUser user, RestUserMessage message)
        {
            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            // Find the menu session associated with the current user.
            var menuSession = Global.MenuIdList.SingleOrDefault(x => x.User.Id == user.Id);
            var multimaker_session = Global.MultiMaker_Session_List.SingleOrDefault(x => x.User.Id == menuSession.User.Id);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Choose a Layout",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            var footer = new EmbedFooterBuilder
            {
                Text = "↩️ Return to Previous Menu"
            };

            embed.WithFooter(footer);

            // Determine the color and thumbnail for the embeded message
            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Profile_Config_Thumbnail(account));

            List<string> two_character_templates = new List<string>() { "P2IS-PS1", "PSIS-PSP", "P2EP-PS1", "PSEP-PSP", "P3P", "P4AU", "P4D", "BBTAG" };
            List<string> three_character_templates = new List<string>() { "P2IS-PS1", "PSIS-PSP", "P2EP-PS1", "PSEP-PSP", "BBTAG" };
            List<string> four_character_templates = new List<string>() { "P2IS-PS1", "PSIS-PSP", "P2EP-PS1", "PSEP-PSP", "BBTAG" };

            string results_list = "";

            results_list += "" +
                "Select how many characters you'd like to use in your scene." +
                $"\n";

            var template = multimaker_session.MakerMultiCommand.Template;

            if (two_character_templates.Any(s => template.Contains(s)))
            {
                results_list += $":two: Two characters\n";
            }
            if (three_character_templates.Any(s => template.Contains(s)))
            {
                results_list += $":three: Three characters\n";
            }
            if (four_character_templates.Any(s => template.Contains(s)))
            {
                results_list += $":four: Four characters\n";
            };

            embed.WithDescription(results_list);

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
            menuSession.CurrentMenu = "MakerMulti_Layout_Select_Main";
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

            if (two_character_templates.Any(s => template.Contains(s)))
            {
                reaction_list.Add(new Emoji("\u0032\ufe0f\u20e3"));
            }
            if (three_character_templates.Any(s => template.Contains(s)))
            {
                reaction_list.Add(new Emoji("\u0033\ufe0f\u20e3"));
            }
            if (four_character_templates.Any(s => template.Contains(s)))
            {
                reaction_list.Add(new Emoji("\u0034\ufe0f\u20e3"));
            }

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
