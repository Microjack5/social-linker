using Discord;
using SocialLinker.Config;
using SocialLinker.Core.CloudStorageTables;
using System;
using System.Threading.Tasks;
using System.Timers;

namespace SocialLinker.Core.Menus
{
    class Utility
    {
        public static void NewTimer(MenuIdStructure menuSession)
        {
            menuSession.MenuTimer = new Timer()
            {
                // Create a timer that expires as a "time out" duration for the user.
                Interval = MenuConfig.menu.timerDuration,
                AutoReset = false,
                Enabled = true
            };

            // If the menu timer runs out, activate a function.
            menuSession.MenuTimer.Elapsed += (sender, e) => Utility.MenuTimer_Elapsed(sender, e, menuSession);
        }

        public static async void MenuTimer_Elapsed(object sender, ElapsedEventArgs e, MenuIdStructure menuSession)
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
                await message.ModifyAsync(x =>
                {
                    x.Embed = MenuTimedOut(menuSession).Build();
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

        public static EmbedBuilder MenuTimedOut(MenuIdStructure menuSession)
        {
            // Get the account information of the command's target
            var account = UserInfoClasses.GetAccount(menuSession.User);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Inactive Menu",
                IconUrl = menuSession.User.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            // Determine the color and thumbnail for the embeded message
            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Profile_Help_Thumbnail(account));

            embed.WithDescription(menuSession.InactiveMessage);
            return embed;
        }

        public static async Task CleanMessage(MenuIdStructure menuSession, EmbedBuilder embed, ComponentBuilder component, bool removeAttachment = false)
        {
            // Attempt editing the message if it hasn't been deleted by the user yet.
            // If it has, catch the exception, remove the menu entry from the global list, and return.
            var message = menuSession.MenuMessage;

            try
            {
                // Remove all reactions from the current message.
                await message.RemoveAllReactionsAsync();

                // Edit the current active message by replacing it with the recently created embed.
                await message.ModifyAsync(x =>
                {
                    if (removeAttachment)
                    {
                        x.Attachments = Array.Empty<FileAttachment>();
                    }
                    x.Embed = embed.Build();

                    if (component != null)
                    {
                        x.Components = component.Build();
                    }
                    else
                    {
                        x.Components = new ComponentBuilder().Build();
                    }
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
        }
    }
}
