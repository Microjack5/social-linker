using Discord;
using Discord.Rest;
using Discord.WebSocket;
using SocialLinker.Core.CloudStorageTables;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace SocialLinker.Cooldown
{
    public class UserCooldownMethods
    {
        public static async Task<bool> IsCooldownActive(SocketMessage message, string command_type)
        {
            // Create variables for the command user and the channel being used.
            SocketGuildUser user = (SocketGuildUser)message.Author;
            ISocketMessageChannel channel = message.Channel;

            // Find the cooldown session associated with both the current user and command type.
            var cooldownSession = Global.CooldownList.SingleOrDefault(x => (x.User.Id == user.Id) && (x.CommandType == command_type));

            // If the session exists, perform an action.
            if (cooldownSession != null)
            {
                // Check for the following conditions:
                // If the command type is "menu" and the usage count is at least 1, the cooldown period is still active.
                // If the command type is "social" and the usage count is at least 3, the cooldown period is still active.
                // If the command type is "status" and the usage count is at least 1, the cooldown period is still active.
                // If the command type is "scene" and the usage count is at least 1, the cooldown period is still active.
                if ((command_type == "menu" && cooldownSession.UsageCount >= 1) ||
                    (command_type == "social" && cooldownSession.UsageCount >= 3) ||
                    (command_type == "status" && cooldownSession.UsageCount >= 1) ||
                    (command_type == "scene" && cooldownSession.UsageCount >= 1))
                {
                    // Check if a cooldown message has been sent for this session.
                    if (cooldownSession.MessageSent == false)
                    {
                        // If not, send an embeded message to the channel and set the "MessageSent" variable to "true".
                        cooldownSession.CooldownMessage = await channel.SendMessageAsync("", false, CooldownEmbed(user, CheckTimeRemaining(cooldownSession)).Build());
                        cooldownSession.MessageSent = true;
                    }
                    
                    // Return the method as "true".
                    return true;
                }
            }
            // If the session does not exist, create one for the command type.
            else
            {
                // Create a new entry and return false.
                CreateCooldownSession(message, command_type);
                return false;
            }

            // Increase the usage count by 1 on the non-active cooldown session.
            cooldownSession.UsageCount += 1;

            // Return the method as "false", indicating there is not a cooldown session active.
            return false;
        }

        public static void CreateCooldownSession(SocketMessage message, string command_type)
        {
            // Create a variable for the command user.
            SocketGuildUser user = (SocketGuildUser)message.Author;

            // Create an int initialized at zero.
            int timer_duration = 0;

            // Depending on the type of command used, set the recently created int variable to a different value.
            // These values will represent milliseconds for a timer object.
            switch (command_type)
            {
                // If the command type is "menu", set timer_duration to 15 seconds.
                case "menu":
                    timer_duration = 15000;
                    break;

                // If the command type is "social", set timer_duration to 5 seconds.
                case "social":
                    timer_duration = 5000;
                    break;

                // If the command type is "status", set timer_duration to 5 seconds.
                case "status":
                    timer_duration = 5000;
                    break;

                // If the command type is "scene", set timer_duration to 5 seconds.
                case "scene":
                    timer_duration = 5000;
                    break;
            }

            // Create a new cooldown session for the command user.
            var cooldown_session = new UserCooldownFields()
            {
                User = user,
                CommandType = command_type,
                UsageCount = 1,
                CooldownTimer = new Timer()
                {
                    // Create a timer that expires as a "time out" duration for the user.
                    Interval = timer_duration,
                    AutoReset = false,
                    Enabled = true
                },
                MessageSent = false,
                ExpirationTime = DateTime.UtcNow.AddMilliseconds(timer_duration)
            };

            // Add the cooldown entry to the global list.
            Global.CooldownList.Add(cooldown_session);

            // If the cooldown timer runs out, activate a function.
            cooldown_session.CooldownTimer.Elapsed += (sender, e) => CooldownTimer_Elapsed(sender, e, message, command_type);
        }

        public static int CheckTimeRemaining(UserCooldownFields cooldown_session)
        {
            // Create a timespan for how much time is left in the active cooldown session.
            TimeSpan time_remaining = cooldown_session.ExpirationTime - DateTime.UtcNow;

            // Cast the result to an int and return.
            return (int)time_remaining.TotalSeconds;
        }

        private static void CooldownTimer_Elapsed(object sender, ElapsedEventArgs e, SocketMessage message, string command_type)
        {
            // Create a variable for the command user.
            SocketGuildUser user = (SocketGuildUser)message.Author;

            // Find the cooldown session associated with both the current user and command type.
            var cooldownSession = Global.CooldownList.SingleOrDefault(x => (x.User.Id == user.Id) && (x.CommandType == command_type));

            // If the cooldown message is not null, try deleting it from the channel if it hasn't been deleted by the user yet.
            if (cooldownSession.CooldownMessage != null)
            {
                try
                {
                    cooldownSession.CooldownMessage.DeleteAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            
            // Remove the cooldown session from the global list.
            Global.CooldownList.Remove(cooldownSession);
        }

        public static EmbedBuilder CooldownEmbed(SocketUser user, int time_remaining)
        {
            // Grab the user's account information.
            var account = UserInfoClasses.GetAccount(user);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Command Cooldown",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            // Determine the color and thumbnail for the embeded message.
            if (account.Profile_Theme == "P3")
            {
                embed.WithColor(37, 149, 255);
                embed.WithThumbnailUrl("https://i.imgur.com/CguM1ql.png");
            }
            else if (account.Profile_Theme == "P4")
            {
                embed.WithColor(255, 229, 49);
                embed.WithThumbnailUrl("https://i.imgur.com/PW7VtuB.png");
            }
            else if (account.Profile_Theme == "P5")
            {
                embed.WithColor(213, 27, 4);
                embed.WithThumbnailUrl("https://i.imgur.com/tubdL8K.png");
            }

            // Change the embeded description depending on how much time is remaining.
            if (time_remaining == 0)
            {
                embed.WithDescription($"Please wait a moment to use this command again.");
            }
            else if (time_remaining == 1)
            {
                embed.WithDescription($"Please wait {time_remaining} second to use this command again.");
            }
            else
            {
                embed.WithDescription($"Please wait {time_remaining} seconds to use this command again.");
            }

            return embed;
        }
    }
}
