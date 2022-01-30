using System;
using System.Timers;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Discord.Addons.Interactive;
using SocialLinker.Config;
using Discord.Rest;

namespace SocialLinker.Core.Menus
{
    public class ErrorHandling : InteractiveBase<SocketCommandContext>
    {
        public static async Task AttachFilesError(SocketTextChannel channel)
        {
            var message = await channel.SendMessageAsync(":warning: Social Linker needs the **`Attach Files`** permission for this channel in order to use this menu.");

            Timer error_timer = new Timer()
            {
                // Create a timer that expires as a "time out" duration for the user.
                Interval = MenuConfig.menu.timerDuration,
                AutoReset = false,
                Enabled = true
            };

            // If the timer runs out, activate a function.
            error_timer.Elapsed += (sender, e) => ErrorHandling.ErrorTimer_Elapsed(sender, e, message);
        }

        public static async Task AddReactionsError(SocketTextChannel channel)
        {
            await channel.SendMessageAsync(":warning: Social Linker needs the **`Add Reactions`** permission for this channel in order to use this menu.");
        }

        public static async Task MissingMessageError(SocketTextChannel channel)
        {
            await channel.SendMessageAsync(":warning: It looks like the menu message has been deleted.");
        }

        public static async void ErrorTimer_Elapsed(object sender, ElapsedEventArgs e, RestUserMessage error_message)
        {
            // Attempt deleting the message if it hasn't been deleted by the user yet.
            // If it has, catch the exception and return.
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
