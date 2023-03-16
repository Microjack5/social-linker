using System;
using Discord;
using Discord.WebSocket;
using SocialLinker.Config;
using SocialLinker.Core.CloudStorageTables;

namespace SocialLinker.Core.LevelSystem
{
    internal static class SocialStats
    {
        // Add stats to accounts
        internal static void AddProficiency(SocialLinkerCommand sl_command)
        {
            var account = UserInfoClasses.GetAccount(sl_command.User);
            DateTime current_time = DateTime.UtcNow;

            // Create a variable for max Proficiency value.
            int max_proficiency = SocialStatRanks.proficiency_rank_5_max;

            // If the daily loop point has not passed and the daily Proficiency cap has been reached, skip this step.
            TimeSpan time_since_last_day = (TimeSpan)(current_time - account.Loop_Point_Day);
            if ((time_since_last_day.TotalHours < 24) && (account.Daily_Proficiency_Gained >= 10))
            {
                return;
            }
            // If the daily loop point has passed, reset the daily parameter.
            else if (time_since_last_day.TotalHours >= 24)
            {
                account.Daily_Proficiency_Gained = 0;
            }

            // If the user has already reached the Proficiency cap, return the function.
            if (account.Proficiency == max_proficiency)
            {
                return;
            }

            // Add Proficiency to user account.
            account.Proficiency += 2;
            account.Daily_Proficiency_Gained += 2;

            // Check if the user has crossed the Proficiency cap. If so, set Proficiency exactly at the cap.
            if (account.Proficiency > max_proficiency)
            {
                account.Proficiency = max_proficiency;
            }

            // Establish variables for keeping track of the user's rank before and after adding Proficiency.
            int old_rank = account.Proficiency_Rank;
            int new_rank = CalculateProficiencyRank(account.Proficiency);

            // Compare old_rank to new_rank. If the values are different, the user ranked up.
            if (old_rank != new_rank)
            {
                // Replace previous stored value with new one.
                account.Proficiency_Rank = new_rank;

                // If the user's account is actiated, has a profile theme set, and has notifications set to on, send a rank up message.
                if (account.Account_Activated == "Yes" && account.Profile_Theme != "" && account.Rank_Up_Notifications == "On")
                {
                    ProficiencyRankUpMessage(sl_command, new_rank);

                    // If this is the first time the user is receiving a rank up message, set the First_Rank_Msg_Sent field to "yes" after the message is sent.
                    if (account.First_Rank_Msg_Sent == "No")
                    {
                        account.First_Rank_Msg_Sent = "Yes";
                    }
                }
            }

            // Update user information with new data.
            UserInfoClasses.UpdateAccount(account);

            // Check if the user has maxed out all social stats.
            AllRanksMaxedCheck(sl_command);
        }

        internal static void AddDiligence(SocialLinkerCommand sl_command)
        {
            var account = UserInfoClasses.GetAccount(sl_command.User);
            DateTime current_time = DateTime.UtcNow;

            // Create a variable for max Diligence value.
            int max_diligence = SocialStatRanks.diligence_rank_5_max;

            // If the daily loop point has not passed and the daily Diligence cap has been reached, return.
            TimeSpan time_since_last_day = (TimeSpan)(current_time - account.Loop_Point_Day);
            if ((time_since_last_day.TotalHours < 24) && (account.Daily_Diligence_Gained == "Yes"))
            {
                return;
            }
            // If the amount of time that has passed is between 24 and 48 hours since the daily loop point, reset the daily parameter.
            else if ((time_since_last_day.TotalHours >= 24) && (time_since_last_day.TotalHours <= 48))
            {
                account.Daily_Diligence_Gained = "No";
            }
            // If the amount of time that has passed is over 48 hours, set the Diligence multiplier back to 10, add a P-Medal, then return. The user does not gain Diligence for the day.
            else if (time_since_last_day.TotalHours > 48)
            {
                account.Diligence_Multiplier = 10;
                account.P_Medals += 1;
                return;
            }

            // If the user has already reached the Diligence cap, return the function.
            if (account.Diligence == max_diligence)
            {
                return;
            }

            // Add Diligence and a P-Medal to the user, then set the daily cap value.
            account.Diligence += account.Diligence_Multiplier;

            // Calculate the amount of P-Medals the user keeps before reaching the P-Medal cap.
            account.P_Medals += LevelSystem.Leveling.CalculateMedalsKept(sl_command, 1);

            // Daily_Diligence_Gained is set to "yes" to signify the day has been counted for any amount.
            account.Daily_Diligence_Gained = "Yes";

            // Increase the Diligence multiplier for the next day if its cap hasn't been reached.
            if (account.Diligence_Multiplier < 20)
            {
                account.Diligence_Multiplier += 1;
            }

            // Check if the user has crossed the Diligence cap. If so, set Diligence exactly at the cap.
            if (account.Diligence > max_diligence)
            {
                account.Diligence = max_diligence;
            }

            // Establish variables for keeping track of the user's rank before and after adding Diligence.
            int old_rank = account.Diligence_Rank;
            int new_rank = CalculateDiligenceRank(account.Diligence);

            // Compare old_rank to new_rank. If the values are different, the user ranked up.
            if (old_rank != new_rank)
            {
                // Replace previous stored value with new one.
                account.Diligence_Rank = new_rank;

                // If the user's account is actiated, has a profile theme set, and has notifications set to on, send a rank up message.
                if (account.Account_Activated == "Yes" && account.Profile_Theme != "" && account.Rank_Up_Notifications == "On")
                {
                    DiligenceRankUpMessage(sl_command, new_rank);

                    // If this is the first time the user is receiving a rank up message, set the First_Rank_Msg_Sent field to "yes" after the message is sent.
                    if (account.First_Rank_Msg_Sent == "No")
                    {
                        account.First_Rank_Msg_Sent = "Yes";
                    }
                }
            }

            //Update user information with new data
            UserInfoClasses.UpdateAccount(account);

            //Check if the user has maxed out all social stats.
            AllRanksMaxedCheck(sl_command);
        }

        internal static void AddExpression(SocialLinkerCommand sl_command, SocketUser user)
        {
            var account = UserInfoClasses.GetAccount(user);
            DateTime current_time = DateTime.UtcNow;

            // Create a variable for max Expression value.
            int max_expression = SocialStatRanks.expression_rank_5_max;

            // If the daily loop point has not passed and the daily Expression cap has been reached, skip this step.
            TimeSpan time_since_last_day = (TimeSpan)(current_time - account.Loop_Point_Day);
            if ((time_since_last_day.TotalHours < 24) && (account.Daily_Expression_Gained >= 10))
            {
                return;
            }
            // If the daily loop point has passed, reset the daily parameter.
            else if (time_since_last_day.TotalHours >= 24)
            {
                account.Daily_Expression_Gained = 0;
            }

            // If the user has already reached the Expression cap, return the function.
            if (account.Expression == max_expression)
            {
                return;
            }

            // Add Expression to user account.
            account.Expression += 2;
            account.Daily_Expression_Gained += 2;

            // Check if the user has crossed the Expression cap. If so, set Expression exactly at the cap.
            if (account.Expression > max_expression)
            {
                account.Expression = max_expression;
            }

            // Establish variables for keeping track of the user's rank before and after adding Expression.
            int old_rank = account.Expression_Rank;
            int new_rank = CalculateExpressionRank(account.Expression);

            // Compare old_rank to new_rank. If the values are different, the user ranked up.
            if (old_rank != new_rank)
            {
                // Replace previous stored value with new one.
                account.Expression_Rank = new_rank;

                // If the user's account is actiated, has a profile theme set, and has notifications set to on, send a rank up message.
                if (account.Account_Activated == "Yes" && account.Profile_Theme != "" && account.Rank_Up_Notifications == "On")
                {
                    ExpressionRankUpMessage(sl_command, new_rank);

                    // If this is the first time the user is receiving a rank up message, set the First_Rank_Msg_Sent field to "yes" after the message is sent.
                    if (account.First_Rank_Msg_Sent == "No")
                    {
                        account.First_Rank_Msg_Sent = "Yes";
                    }
                }
            }

            // Update user information with new data.
            UserInfoClasses.UpdateAccount(account);

            // Check if the user has maxed out all social stats.
            AllRanksMaxedCheck(sl_command);
        }

        // Utility methods
        internal static int CalculateProficiencyRank(double input_value)
        {
            //Create variable
            int answer = 0;

            //Comparison values for Proficiency ranks
            if (input_value <= SocialStatRanks.proficiency_rank_1_max)
            {
                answer = 1;
            }
            else if (input_value <= SocialStatRanks.proficiency_rank_2_max)
            {
                answer = 2;
            }
            else if (input_value <= SocialStatRanks.proficiency_rank_3_max)
            {
                answer = 3;
            }
            else if (input_value <= SocialStatRanks.proficiency_rank_4_max)
            {
                answer = 4;
            }
            else if (input_value >= SocialStatRanks.proficiency_rank_5_max)
            {
                answer = 5;
            }

            return answer;
        }

        internal static int CalculateDiligenceRank(double input_value)
        {
            //Create variable
            int answer = 0;

            //Comparison values for Diligence ranks
            if (input_value <= SocialStatRanks.diligence_rank_1_max)
            {
                answer = 1;
            }
            else if (input_value <= SocialStatRanks.diligence_rank_2_max)
            {
                answer = 2;
            }
            else if (input_value <= SocialStatRanks.diligence_rank_3_max)
            {
                answer = 3;
            }
            else if (input_value <= SocialStatRanks.diligence_rank_4_max)
            {
                answer = 4;
            }
            else if (input_value >= SocialStatRanks.diligence_rank_5_max)
            {
                answer = 5;
            }

            return answer;
        }

        internal static int CalculateExpressionRank(double input_value)
        {
            //Create variable
            int answer = 0;

            //Comparison values for Expression ranks
            if (input_value <= SocialStatRanks.expression_rank_1_max)
            {
                answer = 1;
            }
            else if (input_value <= SocialStatRanks.expression_rank_2_max)
            {
                answer = 2;
            }
            else if (input_value <= SocialStatRanks.expression_rank_3_max)
            {
                answer = 3;
            }
            else if (input_value <= SocialStatRanks.expression_rank_4_max)
            {
                answer = 4;
            }
            else if (input_value >= SocialStatRanks.expression_rank_5_max)
            {
                answer = 5;
            }

            return answer;
        }

        public static decimal SocialStatToDecimal(int input_integer)
        {
            return decimal.Round((decimal)input_integer / 10, 2, MidpointRounding.AwayFromZero);
        }

        internal static string ProficiencyRankTitle(int rank)
        {
            string title = "";

            if (rank == 1)
            {
                title = "Bumbling";
            }
            else if (rank == 2)
            {
                title = "Decent";
            }
            else if (rank == 3)
            {
                title = "Skilled";
            }
            else if (rank == 4)
            {
                title = "Masterful";
            }
            else if (rank == 5)
            {
                title = "Transcendent";
            }

            return title;
        }

        internal static string DiligenceRankTitle(int rank)
        {
            string title = "";

            if (rank == 1)
            {
                title = "Callow";
            }
            else if (rank == 2)
            {
                title = "Persistent";
            }
            else if (rank == 3)
            {
                title = "Strong";
            }
            else if (rank == 4)
            {
                title = "Thorough";
            }
            else if (rank == 5)
            {
                title = "Rock Solid";
            }

            return title;
        }

        internal static string ExpressionRankTitle(int rank)
        {
            string title = "";

            if (rank == 1)
            {
                title = "Rough";
            }
            else if (rank == 2)
            {
                title = "Eloquent";
            }
            else if (rank == 3)
            {
                title = "Persuasive";
            }
            else if (rank == 4)
            {
                title = "Touching";
            }
            else if (rank == 5)
            {
                title = "Enthralling";
            }

            return title;
        }

        // Rank messages
        internal static async void ProficiencyRankUpMessage(SocialLinkerCommand sl_command, int new_rank)
        {
            var user = sl_command.User;
            var channel = sl_command.Channel;

            var account = UserInfoClasses.GetAccount(user);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "RANK UP!!",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            // If the user sees a rank up message for the first time, display a notification for the settings menu.
            if (account.First_Rank_Msg_Sent == "No")
            {
                var footer = new EmbedFooterBuilder
                {
                    Text = $"You can disable rank up messages like these from the {BotConfig.bot.cmdPrefix}settings menu by choosing [General Settings]."
                };

                embed.WithFooter(footer);
            }

            // Determine details that are specific to each profile theme.
            if (account.Profile_Theme == "P3")
            {
                //Color for embeded message
                embed.WithColor(37, 149, 255);

                //Description and thumbnail for embeded message
                if (new_rank == 2)
                {
                    embed.WithThumbnailUrl("https://i.imgur.com/1vmnjkY.png");
                    embed.WithDescription(user.Username + "'s Proficiency has changed from **Bumbling** to **Decent**!");
                }
                else if (new_rank == 3)
                {
                    embed.WithThumbnailUrl("https://i.imgur.com/zJ7sWPx.png");
                    embed.WithDescription(user.Username + "'s Proficiency has changed from **Decent** to **Skilled**!");
                }
                else if (new_rank == 4)
                {
                    embed.WithThumbnailUrl("https://i.imgur.com/xduToPC.png");
                    embed.WithDescription(user.Username + "'s Proficiency has changed from **Skilled** to **Masterful**!");
                }
                else if (new_rank == 5)
                {
                    embed.WithThumbnailUrl("https://i.imgur.com/BBeo5Bf.png");
                    embed.WithDescription(user.Username + "'s Proficiency has **maxed out**!");
                }
            }
            else if (account.Profile_Theme == "P4")
            {
                //Color for embeded message
                embed.WithColor(255, 229, 49);

                //Description and thumbnail for embeded message
                if (new_rank == 2)
                {
                    embed.WithThumbnailUrl("https://i.imgur.com/G2h2phf.png");
                    embed.WithDescription(user.Username + "'s Proficiency has changed from **Bumbling** to **Decent**!");
                }
                else if (new_rank == 3)
                {
                    embed.WithThumbnailUrl("https://i.imgur.com/smfKtru.png");
                    embed.WithDescription(user.Username + "'s Proficiency has changed from **Decent** to **Skilled**!");
                }
                else if (new_rank == 4)
                {
                    embed.WithThumbnailUrl("https://i.imgur.com/Piym7Hu.png");
                    embed.WithDescription(user.Username + "'s Proficiency has changed from **Skilled** to **Masterful**!");
                }
                else if (new_rank == 5)
                {
                    embed.WithThumbnailUrl("https://i.imgur.com/rFbyXdI.png");
                    embed.WithDescription(user.Username + "'s Proficiency has **maxed out**!");
                }
            }
            else if (account.Profile_Theme == "P5")
            {
                //Color for embeded message
                embed.WithColor(213, 27, 4);

                //Description and thumbnail for embeded message
                if (new_rank == 2)
                {
                    embed.WithThumbnailUrl("https://i.imgur.com/kvJtft2.png");
                    embed.WithDescription(user.Username + "'s Proficiency has increased to **Decent**!");
                }
                else if (new_rank == 3)
                {
                    embed.WithThumbnailUrl("https://i.imgur.com/A94X9UD.png");
                    embed.WithDescription(user.Username + "'s Proficiency has increased to **Skilled**!");
                }
                else if (new_rank == 4)
                {
                    embed.WithThumbnailUrl("https://i.imgur.com/vmeNzVn.png");
                    embed.WithDescription(user.Username + "'s Proficiency has increased to **Masterful**!");
                }
                else if (new_rank == 5)
                {
                    embed.WithThumbnailUrl("https://i.imgur.com/XaqjLWC.png");
                    embed.WithDescription(user.Username + "'s Proficiency has **maxed out**!");
                }
            }

            await channel.SendMessageAsync("", false, embed.Build());
        }

        internal static async void DiligenceRankUpMessage(SocialLinkerCommand sl_command, int new_rank)
        {
            var user = sl_command.User;
            var channel = sl_command.Channel;

            var account = UserInfoClasses.GetAccount(user);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "RANK UP!!",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            //If the user sees a rank up message for the first time, display a notification for the settings menu.
            if (account.First_Rank_Msg_Sent == "No")
            {
                var footer = new EmbedFooterBuilder
                {
                    Text = $"You can disable rank up messages like these from the {BotConfig.bot.cmdPrefix}settings menu by choosing [General Settings]."
                };

                embed.WithFooter(footer);
            }

            //Determine details that are specific to each profile theme
            if (account.Profile_Theme == "P3")
            {
                //Color for embeded message
                embed.WithColor(37, 149, 255);

                //Description and thumbnail for embeded message
                if (new_rank == 2)
                {
                    embed.WithThumbnailUrl("https://i.imgur.com/gnp8Sb1.png");
                    embed.WithDescription(user.Username + "'s Diligence has changed from **Callow** to **Persistent**!");
                }
                else if (new_rank == 3)
                {
                    embed.WithThumbnailUrl("https://i.imgur.com/VZcDdhm.png");
                    embed.WithDescription(user.Username + "'s Diligence has changed from **Persistent** to **Strong**!");
                }
                else if (new_rank == 4)
                {
                    embed.WithThumbnailUrl("https://i.imgur.com/AbotQvE.png");
                    embed.WithDescription(user.Username + "'s Diligence has changed from **Strong** to **Thorough**!");
                }
                else if (new_rank == 5)
                {
                    embed.WithThumbnailUrl("https://i.imgur.com/tU8DoUC.png");
                    embed.WithDescription(user.Username + "'s Diligence has **maxed out**!");
                }
            }
            else if (account.Profile_Theme == "P4")
            {
                //Color for embeded message
                embed.WithColor(255, 229, 49);

                //Description and thumbnail for embeded message
                if (new_rank == 2)
                {
                    embed.WithThumbnailUrl("https://i.imgur.com/qDysKKu.png");
                    embed.WithDescription(user.Username + "'s Diligence has changed from **Callow** to **Persistent**!");
                }
                else if (new_rank == 3)
                {
                    embed.WithThumbnailUrl("https://i.imgur.com/tIlZX2I.png");
                    embed.WithDescription(user.Username + "'s Diligence has changed from **Persistent** to **Strong**!");
                }
                else if (new_rank == 4)
                {
                    embed.WithThumbnailUrl("https://i.imgur.com/dtjdeeG.png");
                    embed.WithDescription(user.Username + "'s Diligence has changed from **Strong** to **Thorough**!");
                }
                else if (new_rank == 5)
                {
                    embed.WithThumbnailUrl("https://i.imgur.com/h3wtkot.png");
                    embed.WithDescription(user.Username + "'s Diligence has **maxed out**!");
                }
            }
            else if (account.Profile_Theme == "P5")
            {
                //Color for embeded message
                embed.WithColor(213, 27, 4);

                //Description and thumbnail for embeded message
                if (new_rank == 2)
                {
                    embed.WithThumbnailUrl("https://i.imgur.com/O9g4vsY.png");
                    embed.WithDescription(user.Username + "'s Diligence has increased to **Persistent**!");
                }
                else if (new_rank == 3)
                {
                    embed.WithThumbnailUrl("https://i.imgur.com/XXeil53.png");
                    embed.WithDescription(user.Username + "'s Diligence has increased to **Strong**!");
                }
                else if (new_rank == 4)
                {
                    embed.WithThumbnailUrl("https://i.imgur.com/mKKsnzm.png");
                    embed.WithDescription(user.Username + "'s Diligence has increased to **Thorough**!");
                }
                else if (new_rank == 5)
                {
                    embed.WithThumbnailUrl("https://i.imgur.com/GVx284g.png");
                    embed.WithDescription(user.Username + "'s Diligence has **maxed out**!");
                }
            }

            await channel.SendMessageAsync("", false, embed.Build());
        }

        internal static async void ExpressionRankUpMessage(SocialLinkerCommand sl_command, int new_rank)
        {
            var user = sl_command.User;
            var channel = sl_command.Channel;

            var account = UserInfoClasses.GetAccount(user);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "RANK UP!!",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            //If the user sees a rank up message for the first time, display a notification for the settings menu.
            if (account.First_Rank_Msg_Sent == "No")
            {
                var footer = new EmbedFooterBuilder
                {
                    Text = $"You can disable rank up messages like these from the {BotConfig.bot.cmdPrefix}settings menu by choosing [General Settings]."
                };

                embed.WithFooter(footer);
            }

            //Determine color for embeded message
            if (account.Profile_Theme == "P3")
            {
                //Color for embeded message
                embed.WithColor(37, 149, 255);

                //Description and thumbnail for embeded message
                if (new_rank == 2)
                {
                    embed.WithThumbnailUrl("https://i.imgur.com/pX0dmWm.png");
                    embed.WithDescription(user.Username + "'s Expression has changed from **Rough** to **Eloquent**!");
                }
                else if (new_rank == 3)
                {
                    embed.WithThumbnailUrl("https://i.imgur.com/m4Uwh1b.png");
                    embed.WithDescription(user.Username + "'s Expression has changed from **Eloquent** to **Persuasive**!");
                }
                else if (new_rank == 4)
                {
                    embed.WithThumbnailUrl("https://i.imgur.com/e2hiyeZ.png");
                    embed.WithDescription(user.Username + "'s Expression has changed from **Persuasive** to **Touching**!");
                }
                else if (new_rank == 5)
                {
                    embed.WithThumbnailUrl("https://i.imgur.com/ioJGB7f.png");
                    embed.WithDescription(user.Username + "'s Expression has **maxed out**!");
                }
            }
            else if (account.Profile_Theme == "P4")
            {
                //Color for embeded message
                embed.WithColor(255, 229, 49);

                //Description and thumbnail for embeded message
                if (new_rank == 2)
                {
                    embed.WithThumbnailUrl("https://i.imgur.com/DGyo7Gq.png");
                    embed.WithDescription(user.Username + "'s Expression has changed from **Rough** to **Eloquent**!");
                }
                else if (new_rank == 3)
                {
                    embed.WithThumbnailUrl("https://i.imgur.com/8ZlESIt.png");
                    embed.WithDescription(user.Username + "'s Expression has changed from **Eloquent** to **Persuasive**!");
                }
                else if (new_rank == 4)
                {
                    embed.WithThumbnailUrl("https://i.imgur.com/0FaH1Ai.png");
                    embed.WithDescription(user.Username + "'s Expression has changed from **Persuasive** to **Touching**!");
                }
                else if (new_rank == 5)
                {
                    embed.WithThumbnailUrl("https://i.imgur.com/MF6p0oL.png");
                    embed.WithDescription(user.Username + "'s Expression has **maxed out**!");
                }
            }
            else if (account.Profile_Theme == "P5")
            {
                //Color for embeded message
                embed.WithColor(213, 27, 4);

                //Description and thumbnail for embeded message
                if (new_rank == 2)
                {
                    embed.WithThumbnailUrl("https://i.imgur.com/aM39SzA.png");
                    embed.WithDescription(user.Username + "'s Expression has increased to **Eloquent**!");
                }
                else if (new_rank == 3)
                {
                    embed.WithThumbnailUrl("https://i.imgur.com/HKei8f1.png");
                    embed.WithDescription(user.Username + "'s Expression has increased to **Persuasive**!");
                }
                else if (new_rank == 4)
                {
                    embed.WithThumbnailUrl("https://i.imgur.com/qmPnuGE.png");
                    embed.WithDescription(user.Username + "'s Expression has increased to **Touching**!");
                }
                else if (new_rank == 5)
                {
                    embed.WithThumbnailUrl("https://i.imgur.com/jJMuSB2.png");
                    embed.WithDescription(user.Username + "'s Expression has **maxed out**!");
                }
            }

            await channel.SendMessageAsync("", false, embed.Build());
        }

        internal static void AllRanksMaxedCheck(SocialLinkerCommand sl_command)
        {
            var user = sl_command.User;

            var account = UserInfoClasses.GetAccount(user);

            // Check if all three social stats have reached rank 5 and the user hasn't received a notification acknowledging it yet.
            // Also check if the user's account is actiated, has a profile theme set, and has notifications set to on.
            if ((account.Proficiency_Rank == 5 && account.Diligence_Rank == 5 && account.Expression_Rank == 5) && account.All_Ranks_Maxed_Msg_Sent == "No"
                && account.Account_Activated == "Yes" && account.Profile_Theme != "" && account.Rank_Up_Notifications == "On")
            {
                // If all these conditions are fulfilled, send a notification to the user notifying them that all three social stats are maxed.
                AllRanksMaxedMessage(sl_command);

                // Set the All_Ranks_Maxed_Msg_Sent field to "yes" after the message is sent.
                account.All_Ranks_Maxed_Msg_Sent = "Yes";

                // Update user information with new data.
                UserInfoClasses.UpdateAccount(account);
            }
        }

        internal static async void AllRanksMaxedMessage(SocialLinkerCommand sl_command)
        {
            var user = sl_command.User;
            var channel = sl_command.Channel;

            var account = UserInfoClasses.GetAccount(user);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "ALL RANKS MAXED!!",
                IconUrl = user.GetAvatarUrl()
            };

            //Determine color for embeded message
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

            embed.WithDescription("You've maxed out all social stats!");

            await channel.SendMessageAsync("", false, embed.Build());
        }
    }

    public class SocialStatRanks
    {
        // Proficiency
        public const int proficiency_rank_1_min = 0;
        public const int proficiency_rank_2_min = 240;
        public const int proficiency_rank_3_min = 680;
        public const int proficiency_rank_4_min = 1200;
        public const int proficiency_rank_5_min = 1730;

        public const int proficiency_rank_1_max = 239;
        public const int proficiency_rank_2_max = 679;
        public const int proficiency_rank_3_max = 1199;
        public const int proficiency_rank_4_max = 1729;
        public const int proficiency_rank_5_max = 1730;

        // Diligence
        public const int diligence_rank_1_min = 0;
        public const int diligence_rank_2_min = 640;
        public const int diligence_rank_3_min = 1600;
        public const int diligence_rank_4_min = 3200;
        public const int diligence_rank_5_min = 5600;

        public const int diligence_rank_1_max = 639;
        public const int diligence_rank_2_max = 1599;
        public const int diligence_rank_3_max = 3199;
        public const int diligence_rank_4_max = 5599;
        public const int diligence_rank_5_max = 5600;

        // Expression
        public const int expression_rank_1_min = 0;
        public const int expression_rank_2_min = 300;
        public const int expression_rank_3_min = 660;
        public const int expression_rank_4_min = 1060;
        public const int expression_rank_5_min = 1700;

        public const int expression_rank_1_max = 259;
        public const int expression_rank_2_max = 659;
        public const int expression_rank_3_max = 1059;
        public const int expression_rank_4_max = 1699;
        public const int expression_rank_5_max = 1700;
    }
}
