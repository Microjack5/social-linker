using System;
using System.Timers;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Fergun.Interactive;
using Discord.Rest;
using SocialLinker.Core.CloudStorageTables;
using Discord;
using SocialLinker.Core.LocalStorageTables;
using SocialLinker.Config;
using SocialLinker.Core.Menus;

namespace SocialLinker.Core.StatusScreens
{
    class ErrorHandling : ModuleBase<SocketCommandContext>
    {
        public static async Task Image_Upload_Failed(SocketUser user, ISocketMessageChannel channel)
        {
            // Get the account information of the command's target
            var account = UserInfoClasses.GetAccount(user);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Image Upload Failed",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            // Determine the color and thumbnail for the embeded message.
            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Profile_Help_Thumbnail(account));

            // Write an appropriate description for the error.
            embed.WithDescription("Something went wrong while trying to upload the image. Try again soon.");

            var error_message = await channel.SendMessageAsync("", false, embed.Build());

            Timer error_timer = new Timer()
            {
                // Create a timer that expires as a "time out" duration for the user.
                Interval = Global.error_duration,
                AutoReset = false,
                Enabled = true
            };

            // If the timer runs out, activate a function.
            error_timer.Elapsed += (sender, e) => ErrorTimer_Elapsed(sender, e, error_message, account);
        }

        public static async void ErrorTimer_Elapsed(object sender, ElapsedEventArgs e, RestUserMessage error_message, UserInfoFields account)
        {
            // If the user has their auto-delete settings for error messages set to on, attempt deleting the message if it hasn't been deleted by the user yet.
            // If it has, catch the exception and return.
            if (account.Auto_Delete_Error_Messages == "On")
            {
                try
                {
                    // Delete the current message from the channel.
                    await error_message.DeleteAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return;
                }
            }
        }
    }
}
