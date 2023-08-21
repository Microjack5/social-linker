using System;
using System.Collections.Generic;
using System.Linq;
using Discord;
using Discord.WebSocket;
using SocialLinker.Config;
using SocialLinker.Core.CloudStorageTables;

namespace SocialLinker.Core.LevelSystem
{
    internal static class Leveling
    {
        internal static void UserSentMessage(SocialLinkerCommand sl_command)
        {
            try
            {
                // Get the account information of the message's author.
                var account = UserInfoClasses.GetAccount(sl_command.User);
                DateTime current_time = DateTime.UtcNow;

                // If the user's message cooldown has not passed a minute, ignore their message.
                TimeSpan time_since_last_message = (TimeSpan)(current_time - account.Last_Message_Cooldown);
                if (time_since_last_message.TotalSeconds < 60)
                {
                    //Keep in case of debugging.
                    //Console.WriteLine($"Seconds since last counted message: {time_since_last_message.TotalSeconds}");

                    return;
                }

                // If the daily loop point has not passed and the daily EXP cap has been reached, ignore their message.
                TimeSpan time_since_last_day = (TimeSpan)(current_time - account.Loop_Point_Day);
                if ((time_since_last_day.TotalHours < 24) && (account.Daily_Exp_Gained >= 24000))
                {
                    //Keep in case of debugging.
                    //Console.WriteLine($"Daily EXP cap reached.");

                    return;
                }

                // If the hourly loop point has not passed and the hourly EXP cap has already been reached, ignore their message.
                TimeSpan time_since_last_hour = (TimeSpan)(current_time - account.Loop_Point_Hour);
                if ((time_since_last_hour.TotalHours < 1) && (account.Hourly_Exp_Gained >= 1200))
                {
                    //Keep in case of debugging.
                    //Console.WriteLine($"Hourly EXP cap reached.");

                    return;
                }
                // If the hourly loop point has passed, reset the hourly parameters.
                else if (time_since_last_hour.TotalHours >= 1)
                {
                    account.Loop_Point_Hour = current_time;
                    account.Hourly_Exp_Gained = 0;
                }

                // Determine if the user is currently in a Tired state.
                // If the Hourly_Cap_Counter is 12 and 8 hours have not passed yet, Tired state is active.
                if (account.Hourly_Cap_Counter == 12 && time_since_last_day.TotalHours < 8)
                {
                    // Do nothing
                }
                // If the Hourly_Cap_Counter is 12 and 8 hours have passed, reset the counters.
                else if (account.Hourly_Cap_Counter == 12 && time_since_last_day.TotalHours >= 8)
                {
                    account.Hourly_Cap_Counter = 0;
                    account.Loop_Point_Day = current_time;
                    account.Daily_Exp_Gained = 0;
                }

                // Determine if the user is currently in a Sick state.
                // If the Hourly_Cap_Counter is more than 12 and 24 hours have not passed yet, Sick state is active.
                if (account.Hourly_Cap_Counter > 12 && time_since_last_day.TotalHours < 24)
                {
                    // Do nothing
                }
                // If the Hourly_Cap_Counter is more than 12 and 24 hours have passed, reset the counters.
                else if (account.Hourly_Cap_Counter > 12 && time_since_last_day.TotalHours >= 24)
                {
                    account.Hourly_Cap_Counter = 0;
                    account.Loop_Point_Day = current_time;
                    account.Daily_Exp_Gained = 0;
                }

                // If the user is in a normal state, reset the counters after a day has passed.
                if (account.Hourly_Cap_Counter < 12 && time_since_last_day.TotalHours >= 24)
                {
                    account.Hourly_Cap_Counter = 0;
                    account.Loop_Point_Day = current_time;
                    account.Daily_Exp_Gained = 0;
                }

                // Calculate the amount of EXP the user should earn from this message.
                int gained_exp = CalculateExpEarned(sl_command);

                //Before overwriting the user's total EXP value, store the old one
                int old_exp = account.Total_Exp;

                // Store the new EXP total in both Total_EXP and Hourly_Exp_Gained fields.
                // If the user has reached Level 99, no more EXP is added to their account and the function returns.
                if (account.Level == Global.Max_Level)
                {
                    // Do nothing
                    return;
                }
                // If the hourly EXP cap has been reached 12 times, the user earns half EXP.
                else if (account.Hourly_Cap_Counter == 12)
                {
                    account.Total_Exp += gained_exp / 2;
                    account.Hourly_Exp_Gained += gained_exp / 2;
                    account.Daily_Exp_Gained += gained_exp / 2;
                }
                // If the hourly EXP cap has been reached more than 12 times, the user earns 1 EXP.
                else if (account.Hourly_Cap_Counter > 12)
                {
                    account.Total_Exp += 1;
                    account.Hourly_Exp_Gained += 1;
                    account.Daily_Exp_Gained += 1;
                }
                else
                {
                    account.Total_Exp += gained_exp;
                    account.Hourly_Exp_Gained += gained_exp;
                    account.Daily_Exp_Gained += gained_exp;
                }

                // Determine what level the user is at now.
                int oldLevel = account.Level;
                int newLevel = CalculateLevel(account.Total_Exp);

                // Compare oldLevel to newLevel. If the values are different, the user leveled up.
                if (oldLevel != newLevel)
                {
                    // If newLevel is less than 10, override previous stored value and cap EXP at the start of the next level.
                    if (newLevel < 10)
                    {
                        newLevel = oldLevel + 1;
                        account.Total_Exp = CalculateExp(newLevel);
                    }

                    // Update the user's level with newLevel.
                    account.Level = newLevel;

                    // Calculate the amount of P-Medals the user gains on level up.
                    // Social Bonus is the amount of P-Medals affected by the user's social stats.
                    int social_bonus = (account.Proficiency_Rank - 1) + (account.Diligence_Rank - 1) + (account.Expression_Rank - 1);
                    int gained_pmedals = (social_bonus * 2) + 1;

                    // Calculate the amount of P-Medals the user keeps before reaching the P-Medal cap.
                    gained_pmedals = CalculateMedalsKept(sl_command, gained_pmedals);
                    account.P_Medals += gained_pmedals;

                    // If the user's account is actiated, has a profile theme set, and has notifications set to on, send a level up message.
                    if (account.Account_Activated == "Yes" && account.Profile_Theme != "" && account.Level_Up_Notifications == "On")
                    {
                        LevelUpMessage(sl_command, newLevel, gained_pmedals);

                        // If this is the first time the user is receiving a level up message, set the First_Level_Msg_Sent field to "yes" after the message is sent.
                        if (account.First_Level_Msg_Sent == "No")
                        {
                            account.First_Level_Msg_Sent = "Yes";
                        }
                    }

                    // If the user has leveled up to Level 99 for the first time, activated their account, has a profile theme set, and has notifications set to on, send a notification.
                    if (newLevel == Global.Max_Level && account.Account_Activated == "Yes" && account.Profile_Theme != "" && account.Level_Up_Notifications == "On")
                    {
                        MaxLevelMessage(sl_command);
                    }
                }

                // Check if 75% of the hourly EXP cap has been reached with this message.
                if (old_exp < 4500 && account.Hourly_Exp_Gained >= 4500)
                {
                    // If old_exp was below the threshhold and EXP gained in the hour is above the threshhold, increase the Hourly_Cap_Counter.
                    account.Hourly_Cap_Counter += 1;

                    // If Hourly_Cap_Counter has reached 12, set the Loop_Point_Day to the current time. The user has either entered a "Tired" or "Sick" state.
                    if (account.Hourly_Cap_Counter >= 12)
                    {
                        account.Loop_Point_Day = current_time;
                    }
                }

                // Create a new message cooldown from this point.
                account.Last_Message_Cooldown = current_time;

                // Update user information with new data.
                UserInfoClasses.UpdateAccount(account);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        internal static double CharacterCount(SocialLinkerCommand sl_command)
        {
            Random rnd = new Random();
            int return_count = rnd.Next(10, 2001);

            if (sl_command.ValidCommand == true)
            {
                return_count = return_count * 2;
            }

            return return_count;
        }

        internal static int CalculateExpEarned(SocialLinkerCommand sl_command)
        {
            // Get the account information of the message's author.
            var account = UserInfoClasses.GetAccount(sl_command.User);

            // Create an int variable and initialize it to 0. This will represent the user's gained experience points.
            int gained_exp = 0;

            // Calculate the amount of EXP the user should earn from this message.
            // This is done by dividing the character count of a message by a certain amount, depending on which tier of levels the user is on.

            // Check if the user's level is between 1 and 11.
            if (account.Level >= 1 && account.Level <= 11)
            {
                // If so, earned EXP equals the ceiling of message's character count divided by ten.
                gained_exp = (int)Math.Ceiling(CharacterCount(sl_command) / 10);
            }
            // Check if the user's level is between 12 and 22.
            else if (account.Level >= 12 && account.Level <= 22)
            {
                // If so, earned EXP equals the ceiling of message's character count divided by nine.
                gained_exp = (int)Math.Ceiling(CharacterCount(sl_command) / 8);
            }
            // Check if the user's level is between 23 and 33.
            else if (account.Level >= 23 && account.Level <= 33)
            {
                // If so, earned EXP equals the ceiling of message's character count divided by eight.
                gained_exp = (int)Math.Ceiling(CharacterCount(sl_command) / 7);
            }
            // Check if the user's level is between 34 and 44.
            else if (account.Level >= 34 && account.Level <= 44)
            {
                // If so, earned EXP equals the ceiling of message's character count divided by seven.
                gained_exp = (int)Math.Ceiling(CharacterCount(sl_command) / 6);
            }
            // Check if the user's level is between 45 and 55.
            else if (account.Level >= 45 && account.Level <= 55)
            {
                // If so, earned EXP equals the ceiling of message's character count divided by six.
                gained_exp = (int)Math.Ceiling(CharacterCount(sl_command) / 5);
            }
            // Check if the user's level is between 56 and 66.
            else if (account.Level >= 56 && account.Level <= 66)
            {
                // If so, earned EXP equals the ceiling of message's character count divided by five.
                gained_exp = (int)Math.Ceiling(CharacterCount(sl_command) / 4);
            }
            // Check if the user's level is between 67 and 77.
            else if (account.Level >= 67 && account.Level <= 77)
            {
                // If so, earned EXP equals the ceiling of message's character count divided by four.
                gained_exp = (int)Math.Ceiling(CharacterCount(sl_command) / 3);
            }
            // Check if the user's level is between 78 and 88.
            else if (account.Level >= 78 && account.Level <= 88)
            {
                // If so, earned EXP equals the ceiling of message's character count divided by three.
                gained_exp = (int)Math.Ceiling(CharacterCount(sl_command) / 2);
            }
            // Check if the user's level is between 89 and 99.
            else if (account.Level >= 89 && account.Level <= 99)
            {
                // If so, earned EXP equals the ceiling of message's character count divided by two.
                gained_exp = (int)Math.Ceiling(CharacterCount(sl_command) / 1);
            }

            return gained_exp;
        }

        internal static int CalculateLevel(int input_exp)
        {
            // Create variables.
            int answer = 0;
            int level_to_exp;

            for (int i = 1; i <= 99; i++)
            {
                // Total Exp for Level i = 1/12 (n^4 + 4n^3 + 53n^2 - 58n)
                level_to_exp = (((int)Math.Pow(i, 4)) + (4 * ((int)Math.Pow(i, 3))) + (53 * ((int)Math.Pow(i, 2))) - (58 * i)) / 12;

                if (input_exp < level_to_exp)
                {
                    // If the input EXP is less than the equation's answer, it belongs to the previous level.
                    answer = i - 1;
                    break;
                }
                else if (input_exp == level_to_exp)
                {
                    // If the input EXP is equal to the equation's answer, they are at the same level.
                    answer = i;
                    break;
                }
            }

            return answer;
        }

        internal static int CalculateExp(int input_level)
        {
            // Total Exp for Level n = 1/12 (n^4 + 4n^3 + 53n^2 - 58n)
            int current_total_exp = (((int)Math.Pow(input_level, 4)) + (4 * ((int)Math.Pow(input_level, 3))) + (53 * ((int)Math.Pow(input_level, 2))) - (58 * input_level)) / 12;

            return current_total_exp;
        }

        internal static int CalculateMedalsKept(SocialLinkerCommand sl_command, int gained_pmedals)
        {
            var account = UserInfoClasses.GetAccount(sl_command.User);
            int amount_kept = 0;

            // If the amount of P-Medals gained is greater than the max amount the user can hold, return only what will bring the user to the cap.
            if (gained_pmedals > (Global.Max_PMedals - account.P_Medals))
            {
                amount_kept = Global.Max_PMedals - account.P_Medals;
            }
            // If not, return all the P-Medals gained.
            else
            {
                amount_kept = gained_pmedals;
            }

            return amount_kept;
        }

        internal static async void LevelUpMessage(SocialLinkerCommand sl_command, int new_level, int gained_pmedals)
        {
            var user = sl_command.User;
            var channel = sl_command.Channel;

            var account = UserInfoClasses.GetAccount(user);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "LEVEL UP!!",
                IconUrl = user.GetAvatarUrl()
            };

            // If the user sees the level up message for the first time, display a notification for the settings menu.
            if (account.First_Level_Msg_Sent == "No")
            {
                var footer = new EmbedFooterBuilder
                {
                    Text = $"You can disable level up messages like these from the {BotConfig.bot.cmdPrefix}settings menu by choosing [Profile Settings]."
                };

                embed.WithFooter(footer);
            }

            // Determine color for embeded message.
            if (account.Profile_Theme == "P3")
            {
                embed.WithColor(37, 149, 255);
                embed.WithThumbnailUrl("https://i.imgur.com/UQPUuLL.png");
            }
            else if (account.Profile_Theme == "P4")
            {
                embed.WithColor(255, 229, 49);
                embed.WithThumbnailUrl("https://i.imgur.com/4A3OGYw.png");
            }
            else if (account.Profile_Theme == "P5")
            {
                embed.WithColor(213, 27, 4);
                embed.WithThumbnailUrl("https://i.imgur.com/93rrrmR.png");
            }

            embed.WithAuthor(author);
            embed.WithDescription(user.Username + " has leveled up!");

            // Create a string for the P-Medal section to account for plurals.
            string pmedal_string = "";
            if (gained_pmedals == 1)
            {
                pmedal_string = $"+{gained_pmedals} P-Medal";
            }
            else
            {
                pmedal_string = $"+{gained_pmedals} P-Medals";
            }
            
            // If the user has reset their level before, display a star emote next to the Level area depending on how many times.
            // No levels are gained on Star Level Rank 3, so only ranks 1 & 2 are accounted for.
            if (account.Level_Resets == 2)
            {
                embed.AddField(":star2: LEVEL", new_level, true);
            }
            else if (account.Level_Resets == 1)
            {
                embed.AddField(":star: LEVEL", new_level, true);
            }
            // If not, display the default emote.
            else if (account.Level_Resets == 0)
            {
                embed.AddField("<:exp_shine:672641450319675392> LEVEL", new_level, true);
            }
            
            embed.AddField("<:PMedals:672637091171139615> MONEY", pmedal_string, true);

            await channel.SendMessageAsync("", false, embed.Build());
        }

        internal static async void MaxLevelMessage(SocialLinkerCommand sl_command)
        {
            var user = sl_command.User;
            var channel = sl_command.Channel;

            var account = UserInfoClasses.GetAccount(user);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "MAX LEVEL!!",
                IconUrl = user.GetAvatarUrl()
            };

            // Determine color for embeded message.
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

            embed.WithAuthor(author);

            // If the user has reset their level before, display a message for current Star Level users depending on how many times their level has been reset.
            if (account.Level_Resets == 2)
            {
                embed.WithDescription($"You've reached Level 99 three times! The option **Star Level** has reappeared for the final time in the **`{BotConfig.bot.cmdPrefix}settings`** menu under [Profile Settings].");
            }
            else if (account.Level_Resets == 1)
            {
                embed.WithDescription($"You've reached Level 99 twice! The option **Star Level** has reappeared in the **`{BotConfig.bot.cmdPrefix}settings`** menu under [Profile Settings].");
            }
            // If not, introduce the concept of Star Levels to the user.
            else if (account.Level_Resets == 0)
            {
                embed.WithDescription($"You've reached Level 99! A new option called **Star Level** has been unlocked in the **`{BotConfig.bot.cmdPrefix}settings`** menu under [Profile Settings].");
            }
            
            await channel.SendMessageAsync("", false, embed.Build());
        }
    }
}
