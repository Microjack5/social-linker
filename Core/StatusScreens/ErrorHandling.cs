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

namespace SocialLinker.Core.StatusScreens
{
    class ErrorHandling : ModuleBase<SocketCommandContext>
    {
        public static double error_duration = 60000;

        public static async Task Scene_Upload_Failed(SocketUser user, ISocketMessageChannel channel)
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
            embed.WithColor(Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(Get_Profile_Help_Thumbnail(account));

            // Write an appropriate description for the error.
            embed.WithDescription("Something went wrong while trying to upload the image. Try again soon.");

            var error_message = await channel.SendMessageAsync("", false, embed.Build());

            Timer error_timer = new Timer()
            {
                // Create a timer that expires as a "time out" duration for the user.
                Interval = error_duration,
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

        //Misc
        public static Color Get_Profile_Embed_Color(UserInfoFields account)
        {
            // Based on the account's settings, return a color to be used on embedded menu messages.
            switch (account.Profile_Theme)
            {
                case "P3":
                    return new Color(37, 149, 255);

                case "P4":
                    return new Color(255, 229, 49);

                case "P5":
                    return new Color(213, 27, 4);

                default:
                    return new Color(0, 0, 0);
            }
        }

        public static string Get_Profile_Help_Thumbnail(UserInfoFields account)
        {
            // Based on the account's settings, return a thumbnail to be used on embedded menu messages.
            switch (account.Profile_Theme)
            {
                case "P3":
                    return "https://i.imgur.com/CguM1ql.png";

                case "P4":
                    return "https://i.imgur.com/PW7VtuB.png";

                case "P5":
                    return "https://i.imgur.com/tubdL8K.png";

                default:
                    return "";
            }
        }


    }
}
