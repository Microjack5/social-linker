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
using SocialLinker.Core.Menus;

namespace SocialLinker.Commands
{
    public class Hug : ModuleBase<SocketCommandContext>
    {
        public static async Task HugCommand(SocialLinkerCommand command)
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
                await First_Use_Content_Filter_Menu.First_Use_Content_Filter_Initialize(command);
                return;
            }

            // End of initial usage and cooldown checks.

            // Establish variables for both the user of the command and the user who is pinged.
            SocketUser command_target = null;
            SocketUser command_user = null;

            // Retreive the first mentioned user of the message if there is one.
            var mentionedUser = command.MentionedUser;

            // If a mentioned user exists, assign them to commandTarget. If not, set commandTarget to the command user.
            command_target = mentionedUser ?? command.User;
            command_user = command.User;

            // Check if the mentioned user is null. If so, send an error-tutorial message.
            if (mentionedUser == null)
            {
                HugError(command);
                return;
            }
            // If the mentioned user is the command user, send a special message and return.
            else if (mentionedUser == command_user)
            {
                HugSelf(command);
                return;
            }
            // If the mentioned user is the bot itself, send a special message and return.
            else if (mentionedUser.Id == BotConfig.bot.id)
            {
                HugBot(command);
                return;
            }

            // If the previous conditions are false, get the command user's account information.
            var account = UserInfoClasses.GetAccount(command_target);

            // If a user is mentioned and they're not the command user and not a bot, add Expression to both users.
            if ((mentionedUser != null) && (mentionedUser != command.User) && (mentionedUser.IsBot == false))
            {
                //Core.LevelSystem.SocialStats.AddExpression(command.Message, command_user);
                //Core.LevelSystem.SocialStats.AddExpression(command.Message, command_target);
            }

            // Send a hug message to the mentioned user.
            HugUser(command, command_target);

            await Task.CompletedTask;
        }

        public static async void HugUser(SocialLinkerCommand command, SocketUser command_target)
        {
            var command_user = command.User;
            var channel = command.Channel;

            // Retrieve the account information of both the command's user and the command's target.
            var command_user_account = UserInfoClasses.GetAccount(command_user);
            var command_target_account = UserInfoClasses.GetAccount(command_target);

            // Create an embeded message.
            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = $"{command_user.Username} gave {command_target.Username} a hug!",
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
            string randomized_image = RandomizeHugGif(command_user, command_target);

            if (randomized_image == "")
            {
                var footer = new EmbedFooterBuilder
                {
                    Text = $"...but there's no hug to show!"
                };
                embed.WithFooter(footer);
            }
            else
            {
                embed.WithImageUrl($"{randomized_image}");
            }

            await channel.SendMessageAsync("", false, embed.Build());
        }

        public static async void HugError(SocialLinkerCommand sl_command)
        {
            var user = sl_command.User;
            var channel = sl_command.Channel;

            //Retrieve the account information of the command's user
            var account = UserInfoClasses.GetAccount(user);

            //Create an embeded message and declare the title
            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Social Command: Hug",
                IconUrl = user.GetAvatarUrl()
            };

            // Determine the color and thumbnail for the embeded message
            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Profile_Help_Thumbnail(account));

            embed.WithAuthor(author);
            embed.WithDescription("Mention a user while using this command to give them a hug.");

            await channel.SendMessageAsync("", false, embed.Build());
        }

        public static async void HugSelf(SocialLinkerCommand sl_command)
        {
            var user = sl_command.User;
            var channel = sl_command.Channel;

            await channel.SendMessageAsync($"*Gives {user.Username} a super special hug*");
        }

        public static async void HugBot(SocialLinkerCommand sl_command)
        {
            var user = sl_command.User;
            var channel = sl_command.Channel;

            var account = UserInfoClasses.GetAccount(user);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Social Linker loves you!",
                IconUrl = user.GetAvatarUrl()
            };

            //Determine color for embeded message
            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));

            embed.WithAuthor(author);
            embed.WithDescription("❤️ ❤️ ❤️ ❤️ ❤️");

            await channel.SendMessageAsync("", false, embed.Build());
        }

        public static string RandomizeHugGif(SocketUser command_user, SocketUser command_target)
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
            string[] p3_general_hugs = new string[]
            {
                "https://i.imgur.com/2q6stI4.gif",
                "https://i.imgur.com/zWr1uW0.gif",
                "https://i.imgur.com/jLTzSHG.gif",
                "https://i.imgur.com/Fg3odW3.gif",
                "https://i.imgur.com/SVBwiHF.gif",
                "https://i.imgur.com/ARtqRET.gif",
                "https://i.imgur.com/jNIUoHy.gif",
                "https://i.imgur.com/Moln9na.gif"
            };

            // P3F GIFs are scenes that exclusively apply to the FES version of P3.
            string[] p3f_hugs = new string[]
            {
                "https://i.imgur.com/dlu1LyB.gif"
            };

            // P3P GIFs are scenes that exclusively apply to the Portable version of P3.
            string[] p3p_hugs = new string[]
            {
                // There are no P3P-specific GIFs to add.
            };

            // Second Title: Persona 4.
            // General P4 GIFs are scenes that can be applied to both versions of P4 or various P4-related media.
            string[] p4_general_hugs = new string[]
            {
                "https://i.imgur.com/CAR53Tz.gif",
                "https://i.imgur.com/vYZicIG.gif",
                "https://i.imgur.com/yggwHKG.gif",
                "https://i.imgur.com/kBKgSxC.gif"
            };

            // P4-PS2 GIFs are scenes that exclusively apply to the PlayStation 2 version of P4.
            string[] p4_ps2_hugs = new string[]
            {
                // There are no P4-PS2-specific GIFs to add.
            };

            // P4G GIFs are scenes that exclusively apply to the Golden version of P4.
            string[] p4g_hugs = new string[]
            {
                "https://i.imgur.com/G82Q9nZ.gif",
                "https://i.imgur.com/WTDyXRL.gif",
                "https://i.imgur.com/uat3man.gif",
                "https://i.imgur.com/uBIlZzW.gif",
                "https://i.imgur.com/mSnxV59.gif",
                "https://i.imgur.com/qjo9zKB.gif",
                "https://i.imgur.com/DCpRfSx.gif",
                "https://i.imgur.com/q9mEGaq.gif",
                "https://i.imgur.com/Wse8tiB.gif",
                "https://i.imgur.com/bMIqHIM.gif"
            };

            // Third Title: Persona 5.
            // General P5 GIFs are scenes that can be applied to both versions of P5 or various P5-related media.
            string[] p5_general_hugs = new string[]
            {
                "https://i.imgur.com/e0Z8pMg.gif",
                "https://i.imgur.com/7FcjIlQ.gif"
            };

            // P5-PS4 GIFs are scenes that exclusively apply to the PlayStation 3 version of P5.
            string[] p5_ps4_hugs = new string[]
            {
                // There are no P5-PS4-specific GIFs to add.
            };

            // P5R GIFs are scenes that exclusively apply to the Royal version of P5.
            string[] p5r_hugs = new string[]
            {
                "https://i.imgur.com/PffVoP3.gif",
                "https://i.imgur.com/TzkB3CX.gif",
                "https://i.imgur.com/fYdjOaa.gif",
                "https://i.imgur.com/IQvVEKQ.gif",
                "https://i.imgur.com/sa72Ltk.gif"
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
                p3_selection_list.AddRange(p3_general_hugs);
            }

            // If the command user and command target allows P3F content, add P3F-specific GIFs to the p3_selection_list.
            if (command_user_filter.Contains("P3F") == false && command_target_filter.Contains("P3F") == false)
            {
                p3_selection_list.AddRange(p3f_hugs);
            }

            // If the command user and command target allows P3P content, add P3P-specific GIFs to the p3_selection_list.
            if (command_user_filter.Contains("P3P") == false && command_target_filter.Contains("P3P") == false)
            {
                p3_selection_list.AddRange(p3p_hugs);
            }

            // If both the command user and command target allows either P4-PS2 and P4G content, add general P4 GIFs to the p4_selection_list.
            if ((command_user_filter.Contains("P4-PS2") == false || command_user_filter.Contains("P4G") == false) &&
                (command_target_filter.Contains("P4-PS2") == false || command_target_filter.Contains("P4G") == false))
            {
                p4_selection_list.AddRange(p4_general_hugs);
            }

            // If the command user and command target allows P4-PS2 content, add P4-PS2-specific GIFs to the p4_selection_list.
            if (command_user_filter.Contains("P4-PS2") == false && command_target_filter.Contains("P4-PS2") == false)
            {
                p4_selection_list.AddRange(p4_ps2_hugs);
            }

            // If the command user and command target allows P4G content, add P4G-specific GIFs to the p4_selection_list.
            if (command_user_filter.Contains("P4G") == false && command_target_filter.Contains("P4G") == false)
            {
                p4_selection_list.AddRange(p4g_hugs);
            }

            // If both the command user and command target allows either P5-PS4 and P5R content, add general P5 GIFs to the p5_selection_list.
            if ((command_user_filter.Contains("P5-PS4") == false || command_user_filter.Contains("P5R") == false) &&
                (command_target_filter.Contains("P5-PS4") == false || command_target_filter.Contains("P5R") == false))
            {
                p5_selection_list.AddRange(p5_general_hugs);
            }

            // If the command user and command target allows P5-PS4 content, add P5-PS4-specific GIFs to the p5_selection_list.
            if (command_user_filter.Contains("P5-PS4") == false && command_target_filter.Contains("P5-PS4") == false)
            {
                p5_selection_list.AddRange(p5_ps4_hugs);
            }

            // If the command user and command target allows P5R content, add P5R-specific GIFs to the p5_selection_list.
            if (command_user_filter.Contains("P5R") == false && command_target_filter.Contains("P5R") == false)
            {
                p5_selection_list.AddRange(p5r_hugs);
            }

            // Using the created selection lists, get a random GIF based on the command user's profile theme.
            if (command_user_account.Profile_Theme == "P3" && p3_selection_list.Count != 0)
            {
                imgurl = p3_selection_list[r.Next(0, p3_selection_list.Count)];
            }
            else if (command_user_account.Profile_Theme == "P4" && p4_selection_list.Count != 0)
            {
                imgurl = p4_selection_list[r.Next(0, p4_selection_list.Count)];
            }
            else if (command_user_account.Profile_Theme == "P5" && p5_selection_list.Count != 0)
            {
                imgurl = p5_selection_list[r.Next(0, p5_selection_list.Count)];
            }
            // If the user does not have a profile theme set, take all GIFs and combine them into one list to choose from.
            else if (command_user_account.Profile_Theme == "")
            {
                List<string> all_selection_list = new List<string>();

                all_selection_list.AddRange(p3_selection_list);
                all_selection_list.AddRange(p4_selection_list);
                all_selection_list.AddRange(p5_selection_list);

                imgurl = all_selection_list[r.Next(0, all_selection_list.Count)];
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
