using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Fergun.Interactive;
using SocialLinker.Config;
using SocialLinker.Core.CloudStorageTables;
using SocialLinker.Cooldown;
using SocialLinker.Core.Menus.InitialUsage.Main;

namespace SocialLinker.Commands
{
    public class Punch : ModuleBase<SocketCommandContext>
    {
        public static async Task PunchCommand(SocialLinkerCommand command)
        {
            // If there is a cooldown session active for the command type "social", return the method immediately.
            if (await UserCooldownMethods.IsCooldownActive(command, "social") == true)
            {
                return;
            }

            // Get the account information of the command's user.
            var command_user_account = UserInfoClasses.GetAccount(command.User);

            // Check if the user's account has been activated. If not, send them to the initial usage setup menu.
            if (command_user_account.Account_Activated == "No")
            {
                await First_Use_Content_Filter_Menu.First_Use_Content_Filter_Start((SocketTextChannel)command.Channel, (SocketGuildUser)command.User);
                return;
            }

            // End of initial usage and cooldown checks.

            //Establish variables for both the user of the command and the user who is pinged
            SocketUser commandTarget = null;
            SocketUser commandUser = null;

            //Retreive the first mentioned user of the message if there is one
            var mentionedUser = command.Message.MentionedUsers.FirstOrDefault();

            //If a mentioned user exists, assign them to commandTarget. If not, set commandTarget to the command user.
            commandTarget = mentionedUser ?? command.User;
            commandUser = command.User;

            //Check if the mentioned user is null. If so, send an error-tutorial message.
            if (mentionedUser == null)
            {
                PunchError(command.Message);
                return;
            }
            //If the mentioned user is the command user, send a special message and return
            else if (mentionedUser == commandUser)
            {
                PunchSelf(command.Message);
                return;
            }
            //If the mentioned user is the bot itself, send a special message and return
            else if (mentionedUser.Id == BotConfig.bot.id)
            {
                PunchBot(command.Message);
                return;
            }

            //If the previous conditions are false, get the command user's account information
            var account = UserInfoClasses.GetAccount(commandTarget);

            //If a user is mentioned and they're not the command user and not a bot, add Expression to both users
            if ((mentionedUser != null) && (mentionedUser != command.User) && (mentionedUser.IsBot == false))
            {
                Core.LevelSystem.SocialStats.AddExpression(command.Message, commandUser);
                Core.LevelSystem.SocialStats.AddExpression(command.Message, commandTarget);
            }

            //Send a punch message to the mentioned user
            PunchUser(command.Message, commandTarget);

            await Task.CompletedTask;
        }

        public static async void PunchUser(SocketMessage message, SocketUser command_target)
        {
            var command_user = message.Author;
            var channel = message.Channel;

            // Retrieve the account information of both the command's user and the command's target.
            var command_user_account = UserInfoClasses.GetAccount(command_user);
            var command_target_account = UserInfoClasses.GetAccount(command_target);

            // Create an embeded message.
            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = $"{command_user.Username} punched {command_target.Username}!",
                IconUrl = command_user.GetAvatarUrl()
            };

            // Determine color for embeded message based on the command user's profile.
            if (command_user_account.Profile_Theme == "P3")
            {
                embed.WithColor(37, 149, 255);
            }
            else if (command_user_account.Profile_Theme == "P4")
            {
                embed.WithColor(255, 229, 49);
            }
            else if (command_user_account.Profile_Theme == "P5")
            {
                embed.WithColor(213, 27, 4);
            }

            embed.WithAuthor(author);

            // Create a randomized URL based on the command user and command target's content filters.
            string randomized_image = RandomizePunchGif(command_user, command_target);

            // If the command user has a set profile theme and the randomized image URL is empty, OR the command target doesn't have an activated account, add a notification to the embed.
            if ((command_user_account.Profile_Theme != "" && randomized_image == "") || command_target_account.Account_Activated == "No")
            {
                var footer = new EmbedFooterBuilder
                {
                    Text = $"{command_user_account.Profile_Theme} images are filtered out for this user."
                };
                embed.WithFooter(footer);
            }
            // Else, if the command user has a profile theme set, choose a random GIF to display based on it
            else if (command_user_account.Profile_Theme != "")
            {
                embed.WithImageUrl($"{randomized_image}");
            }
            // Else, if the command user doesn't have a profile theme set, add a different notification to the embed instead.
            else if (command_user_account.Profile_Theme == "")
            {
                embed.WithDescription($"You can add GIFs to your social commands by visiting the **`{BotConfig.bot.cmdPrefix}settings`** menu and choosing [Profile Settings] > [Profile Theme].");
            }

            await channel.SendMessageAsync("", false, embed.Build());
        }

        public static async void PunchError(SocketMessage message)
        {
            var user = message.Author;
            var channel = message.Channel;

            //Retrieve the account information of the command's user
            var account = UserInfoClasses.GetAccount(user);

            //Create an embeded message and declare the title
            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Social Command: Punch",
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
            embed.WithDescription("Mention a user while using this command to punch them.");

            await channel.SendMessageAsync("", false, embed.Build());
        }

        public static async void PunchSelf(SocketMessage message)
        {
            var user = message.Author;
            var channel = message.Channel;

            //Retrieve the account information of the command's user
            var account = UserInfoClasses.GetAccount(user);

            //Create an embeded message and declare the title
            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = $"Oh no! Social Linker gave {user.Username} a hug instead.",
                IconUrl = user.GetAvatarUrl()
            };

            //Determine color for embeded message
            if (account.Profile_Theme == "P3")
            {
                embed.WithColor(37, 149, 255);
            }
            else if (account.Profile_Theme == "P4")
            {
                embed.WithColor(255, 229, 49);
            }
            else if (account.Profile_Theme == "P5")
            {
                embed.WithColor(213, 27, 4);
            }

            embed.WithAuthor(author);

            if (account.Profile_Theme != "")
            {
                embed.WithImageUrl($"{Commands.Hug.RandomizeHugGif(user, user)}");
            }
            
            await channel.SendMessageAsync("", false, embed.Build());
        }

        public static async void PunchBot(SocketMessage message)
        {
            var user = message.Author;
            var channel = message.Channel;

            var account = UserInfoClasses.GetAccount(user);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Ow!!!",
                IconUrl = user.GetAvatarUrl()
            };

            //Determine color for embeded message
            if (account.Profile_Theme == "P3")
            {
                embed.WithColor(37, 149, 255);
            }
            else if (account.Profile_Theme == "P4")
            {
                embed.WithColor(255, 229, 49);
            }
            else if (account.Profile_Theme == "P5")
            {
                embed.WithColor(213, 27, 4);
            }

            embed.WithAuthor(author);
            embed.WithDescription("Social Linker needs a few minutes to recover...");

            await channel.SendMessageAsync("", false, embed.Build());

            //If the user punches Social Linker, their messages will be ignored for 3 minutes
            Core.LevelSystem.TimeOut.SetTimeOut(user, 3);
        }

        public static string RandomizePunchGif(SocketUser command_user, SocketUser command_target)
        {
            // Retrieve the account information of the both the command's user and the command's target.
            // These two may be the same account in some cases.
            var command_user_account = UserInfoClasses.GetAccount(command_user);
            var command_target_account = UserInfoClasses.GetAccount(command_target);

            // Create a random variable.
            Random r = new Random();

            // Create an empty string variable that will return as the final answer.
            string imgurl = "";

            // Social command GIFs can be divided into subcategories for each title depending on the version, so create separate string arrays for each category.
            // First Title: Persona 3.
            // General P3 GIFs are scenes that can be applied to both versions of P3 or various P3-related media.
            string[] p3_general_punch = new string[]
            {
                "https://i.imgur.com/b2LGALR.gif",
                "https://i.imgur.com/93KPORd.gif",
                "https://i.imgur.com/kK9Wg28.gif",
                "https://i.imgur.com/04aeEXU.gif",
                "https://i.imgur.com/5u6H5PY.gif",
                "https://i.imgur.com/djkmWRN.gif"
            };

            // P3F GIFs are scenes that exclusively apply to the FES version of P3.
            string[] p3f_punch = new string[]
            {
                "https://i.imgur.com/m0YLLUO.gif"
            };

            // P3P GIFs are scenes that exclusively apply to the Portable version of P3.
            string[] p3p_punch = new string[]
            {
                // There are no P3P-specific GIFs to add.
            };

            // Second Title: Persona 4.
            // General P4 GIFs are scenes that can be applied to both versions of P4 or various P4-related media.
            string[] p4_general_punch = new string[]
            {
                "https://i.imgur.com/0YIz2C6.gif",
                "https://i.imgur.com/cQSw5YM.gif",
                "https://i.imgur.com/jjiHb5s.gif"
            };

            // P4-PS2 GIFs are scenes that exclusively apply to the PlayStation 2 version of P4.
            string[] p4_ps2_punch = new string[]
            {
                // There are no P4-PS2-specific GIFs to add.
            };

            // P4G GIFs are scenes that exclusively apply to the Golden version of P4.
            string[] p4g_punch = new string[]
            {
                "https://i.imgur.com/T00RYts.gif",
                "https://i.imgur.com/t2Dg70R.gif",
                "https://i.imgur.com/ExSXdlV.gif",
                "https://i.imgur.com/SIx9Int.gif"
            };

            // Third Title: Persona 5.
            // General P5 GIFs are scenes that can be applied to both versions of P5 or various P5-related media.
            string[] p5_general_punch = new string[]
            {
                "https://i.imgur.com/AZ75vbH.gif"
            };

            // P5-PS4 GIFs are scenes that exclusively apply to the PlayStation 4 version of P5.
            string[] p5_ps4_punch = new string[]
            {
                // There are no P5-PS4-specific GIFs to add.
            };

            // P5R GIFs are scenes that exclusively apply to the Royal version of P5.
            string[] p5r_punch = new string[]
            {
                "https://i.imgur.com/RIYttUf.gif",
                "https://i.imgur.com/TRUiJMu.gif",
                "https://i.imgur.com/cyUnU4O.gif",
                "https://i.imgur.com/aG7hQbu.gif",
                "https://i.imgur.com/qXhfwuD.gif",
                "https://i.imgur.com/eynit0u.gif"
            };

            // Create two list variables containing the content filters of both the command's user and the command's target.
            // These two may be the same list in some cases.
            List<string> command_user_filter = ParseContentFilter(command_user_account);
            List<string> command_target_filter = ParseContentFilter(command_target_account);

            // Create string lists for each of the profile themes that will store the final selection of GIFs to be chosen from.
            List<string> p3_selection_list = new List<string>();
            List<string> p4_selection_list = new List<string>();
            List<string> p5_selection_list = new List<string>();

            // If both the command user and command target allows either P3F and P3P content, add general P3 GIFs to the p3_selection_list.
            if ((command_user_filter.Contains("P3F") == false || command_user_filter.Contains("P3P") == false) &&
                (command_target_filter.Contains("P3F") == false || command_target_filter.Contains("P3P") == false))
            {
                p3_selection_list.AddRange(p3_general_punch);
            }

            // If the command user and command target allows P3F content, add P3F-specific GIFs to the p3_selection_list.
            if (command_user_filter.Contains("P3F") == false && command_target_filter.Contains("P3F") == false)
            {
                p3_selection_list.AddRange(p3f_punch);
            }

            // If the command user and command target allows P3P content, add P3P-specific GIFs to the p3_selection_list.
            if (command_user_filter.Contains("P3P") == false && command_target_filter.Contains("P3P") == false)
            {
                p3_selection_list.AddRange(p3p_punch);
            }

            // If both the command user and command target allows either P4-PS2 and P4G content, add general P4 GIFs to the p4_selection_list.
            if ((command_user_filter.Contains("P4-PS2") == false || command_user_filter.Contains("P4G") == false) &&
                (command_target_filter.Contains("P4-PS2") == false || command_target_filter.Contains("P4G") == false))
            {
                p4_selection_list.AddRange(p4_general_punch);
            }

            // If the command user and command target allows P4-PS2 content, add P4-PS2-specific GIFs to the p4_selection_list.
            if (command_user_filter.Contains("P4-PS2") == false && command_target_filter.Contains("P4-PS2") == false)
            {
                p4_selection_list.AddRange(p4_ps2_punch);
            }

            // If the command user and command target allows P4G content, add P4G-specific GIFs to the p4_selection_list.
            if (command_user_filter.Contains("P4G") == false && command_target_filter.Contains("P4G") == false)
            {
                p4_selection_list.AddRange(p4g_punch);
            }

            // If both the command user and command target allows either P5-PS4 and P5R content, add general P5 GIFs to the p5_selection_list.
            if ((command_user_filter.Contains("P5-PS4") == false || command_user_filter.Contains("P5R") == false) &&
                (command_target_filter.Contains("P5-PS4") == false || command_target_filter.Contains("P5R") == false))
            {
                p5_selection_list.AddRange(p5_general_punch);
            }

            // If the command user and command target allows P5-PS4 content, add P5-PS4-specific GIFs to the p5_selection_list.
            if (command_user_filter.Contains("P5-PS4") == false && command_target_filter.Contains("P5-PS4") == false)
            {
                p5_selection_list.AddRange(p5_ps4_punch);
            }

            // If the command user and command target allows P5R content, add P5R-specific GIFs to the p5_selection_list.
            if (command_user_filter.Contains("P5R") == false && command_target_filter.Contains("P5R") == false)
            {
                p5_selection_list.AddRange(p5r_punch);
            }

            // Using the created selection lists, get a random GIF based on the command user's profile theme.
            if (command_user_account.Profile_Theme == "P3" && p3_selection_list.Count != 0)
            {
                imgurl = p3_selection_list[r.Next(0, p3_selection_list.Count)];
            }
            if (command_user_account.Profile_Theme == "P4" && p4_selection_list.Count != 0)
            {
                imgurl = p4_selection_list[r.Next(0, p4_selection_list.Count)];
            }
            if (command_user_account.Profile_Theme == "P5" && p5_selection_list.Count != 0)
            {
                imgurl = p5_selection_list[r.Next(0, p5_selection_list.Count)];
            }

            return imgurl;
        }

        public static List<string> ParseContentFilter(UserInfoFields account)
        {
            //Create a list variable to return
            List<string> input_substring;

            //Specify the characters to divide the incoming string by
            char[] delimiterChars = { ';' };

            //Assign the return value to the input account's content filter string with its entries split into a list
            input_substring = account.Content_Filter.Split(delimiterChars).ToList();

            return input_substring;
        }
    }
}
