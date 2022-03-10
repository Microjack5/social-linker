using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Fergun.Interactive;
using SocialLinker.Core.CloudStorageTables;
using SocialLinker.Core.StatusScreens;
using System.Reflection;
using SocialLinker.Cooldown;
using SocialLinker.Core.Menus.InitialUsage.Main;

namespace SocialLinker.Commands
{
    public class Status : ModuleBase<SocketCommandContext>
    {
        [Command("status")]
        public async Task StatusCommandParser1()
        {
            // If there is a cooldown session active for the command type "status", return the method immediately.
            if (await UserCooldownMethods.IsCooldownActive(Context.Message, "status") == true)
            {
                return;
            }

            // Get the account information of the command's user.
            var command_user_account = UserInfoClasses.GetAccount(Context.User);

            // Check if the user's account has been activated. If not, send them to the initial usage setup menu.
            if (command_user_account.Account_Activated == "No")
            {
                await First_Use_Content_Filter_Menu.First_Use_Content_Filter_Start((SocketTextChannel)Context.Channel, (SocketGuildUser)Context.User);
                return;
            }

            // End of initial usage and cooldown checks.

            //Since there are no parameters, invoke StatusScreen for the command user
            await StatusScreen();
        }

        [Command("status", RunMode = RunMode.Async)]
        public async Task StatusCommandParser2(string param)
        {
            // If there is a cooldown session active for the command type "status", return the method immediately.
            if (await UserCooldownMethods.IsCooldownActive(Context.Message, "status") == true)
            {
                return;
            }

            // Get the account information of the command's user.
            var command_user_account = UserInfoClasses.GetAccount(Context.User);

            // Check if the user's account has been activated. If not, send them to the initial usage setup menu.
            if (command_user_account.Account_Activated == "No")
            {
                await First_Use_Content_Filter_Menu.First_Use_Content_Filter_Start((SocketTextChannel)Context.Channel, (SocketGuildUser)Context.User);
                return;
            }

            // End of initial usage and cooldown checks.

            //Create a variable for a potential mentioned user
            var mentionedUser = Context.Message.MentionedUsers.FirstOrDefault();

            //If the mentioned user is not null, invoke StatusScreen for the mentioned user
            if (mentionedUser != null)
            {
                await StatusScreen(param);
            }
            //Else, if the entered parameter is the word "detail", invoke StatusDetails for the command user
            else if (param.ToLower() == "detail")
            {
                await StatusDetails(param);
            }
        }

        [Command("status", RunMode = RunMode.Async)]
        public async Task StatusCommandParser3(string param1, string param2)
        {
            // If there is a cooldown session active for the command type "status", return the method immediately.
            if (await UserCooldownMethods.IsCooldownActive(Context.Message, "status") == true)
            {
                return;
            }

            // Get the account information of the command's user.
            var command_user_account = UserInfoClasses.GetAccount(Context.User);

            // Check if the user's account has been activated. If not, send them to the initial usage setup menu.
            if (command_user_account.Account_Activated == "No")
            {
                await First_Use_Content_Filter_Menu.First_Use_Content_Filter_Start((SocketTextChannel)Context.Channel, (SocketGuildUser)Context.User);
                return;
            }

            // End of initial usage and cooldown checks.

            //Create a variable for a potential mentioned user
            var mentionedUser = Context.Message.MentionedUsers.FirstOrDefault();

            //If the first parameter is the word "detail" and the second parameter is a mentioned user, invoke StatusDetails for the mentioned user
            if (param1.ToLower() == "detail" && mentionedUser != null)
            {
                await StatusDetails(param2);
            }
        }

        public async Task StatusScreen([Remainder] string arg = "")
        {
            //Establish variables for the command user and the command's target
            SocketUser commandTarget = null;
            SocketUser commandUser = null;

            //Create a variable for a potential mentioned user
            var mentionedUser = Context.Message.MentionedUsers.FirstOrDefault();

            //If there is a mentioned user, they become the command's target. If not, the command's user is also the target.
            commandTarget = mentionedUser ?? Context.User;
            commandUser = Context.User;

            //Get the account information of the command's target
            var account = UserInfoClasses.GetAccount(commandTarget);

            //If a user is mentioned and they're not the command user and not a bot, add Expression to both users
            if ((mentionedUser != null) && (mentionedUser != Context.User) && (mentionedUser.IsBot == false))
            {
                Core.LevelSystem.SocialStats.AddExpression(Context.Message, commandUser);
                Core.LevelSystem.SocialStats.AddExpression(Context.Message, commandTarget);
            }

            // Call different status screen functions depending on the account's profile theme and decor settings.
            if (account.Decor_Setting != "")
            {
                // Attempt to render the user's set décor.
                try
                {
                    // Use the string taken from the user's Décor setting to find the class that generates it.
                    Type type = Type.GetType($"SocialLinker.Core.StatusScreens.Decor.{account.Decor_Setting}");

                    // Specify the RenderImage method of whatever class is chosen to invoke. Every décor class should have this method to construct its image.
                    MethodInfo methodInfo = type.GetMethod("RenderImage");

                    // Store the typical parameters for a RenderImage method within an object array.
                    object[] parametersArray = new object[] { commandTarget, Context.Channel };

                    // Call the method to render the user's set décor.
                    methodInfo.Invoke(this, parametersArray);
                }
                // If something goes awry in rendering the décor, send an error message to the user and return.
                catch (Exception ex)
                {
                    await Context.Channel.SendMessageAsync($":warning: Oh no! It looks like a mistake was made. Please visit the support server to report this issue and gain access to the décor.");
                    Console.WriteLine(ex);
                    return;
                }

            }
            else if (account.Profile_Theme == "P3")
            {
                StatusScreenP3.RenderImage(commandTarget, Context.Channel);
            }
            else if (account.Profile_Theme == "P4")
            {
                StatusScreenP4.RenderImage(commandTarget, Context.Channel);
            }
            else if (account.Profile_Theme == "P5")
            {
                StatusScreenP5.RenderImage(commandTarget, Context.Channel);
            }
            else if (account.Profile_Theme == "")
            {
                // If the user doesn't have a profile theme set, send a message to do so.
                _ = StartThemeMenu();
            }

            await Task.CompletedTask;
        }

        public async Task StartThemeMenu()
        {
            // If there is a cooldown session active for the command type "menu", return the method immediately.
            if (await UserCooldownMethods.IsCooldownActive(Context.Message, "menu") == true)
            {
                return;
            }

            // Create two variables to check if there is a menu list entry with either the current channel ID or current user ID.
            var channelSearch = Global.MenuIdList.SingleOrDefault(x => x.MenuMessage.Channel.Id == Context.Channel.Id);
            var userSearch = Global.MenuIdList.SingleOrDefault(x => x.User.Id == Context.User.Id);

            // If the channel entry exists and the user is not the same, send an error message.
            if (channelSearch != null && channelSearch.User.Id != Context.User.Id)
            {
                // Case 1: Search by channel successful, user ID does not match. Create new entry for new user.
                // Create a new menu in the current channel.
                await SetFirstTheme_Menu.SetFirstThemeMain((SocketTextChannel)Context.Channel, (SocketGuildUser)Context.User);
                return;
            }
            // Else, if the channel entry exists and the user is the same, assume they want to reset the menu and delete the previous entry.
            else if (channelSearch != null && channelSearch.User.Id == Context.User.Id)
            {
                // Case 2: Search by channel successful, user ID matches. Resetting menu in same channel.
                // Attempt deleting the message if it hasn't been deleted by the user yet.
                try
                {
                    // Delete the currently active menu.
                    await channelSearch.MenuMessage.DeleteAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                // Stop the timeout timer associated with the menu.
                channelSearch.MenuTimer.Stop();

                // Remove the menu entry from the global list.
                Global.MenuIdList.Remove(channelSearch);

                // Create a new menu in the current channel.
                await SetFirstTheme_Menu.SetFirstThemeMain((SocketTextChannel)Context.Channel, (SocketGuildUser)Context.User);
                return;
            }
            // Else, if an entry exists where the user is found but they're in a different channel now, delete previous entry and reset the menu.
            else if (userSearch != null && userSearch.MenuMessage.Channel.Id != Context.Channel.Id)
            {
                // Case 3: Search by user successful, channel ID does not match. Resetting menu in new channel.
                // Attempt deleting the message if it hasn't been deleted by the user yet.
                try
                {
                    // Delete the currently active menu.
                    await userSearch.MenuMessage.DeleteAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                // Stop the timeout timer associated with the menu.
                userSearch.MenuTimer.Stop();

                // Remove the menu entry from the global list.
                Global.MenuIdList.Remove(userSearch);

                // Create a new menu in the current channel.
                await SetFirstTheme_Menu.SetFirstThemeMain((SocketTextChannel)Context.Channel, (SocketGuildUser)Context.User);
                return;
            }
            // For any other condition (if one should exist and not be handled here), create a new menu entry.
            else
            {
                // Case 4: No previous entry found. Create new entry.
                // Create a new menu in the current channel.
                await SetFirstTheme_Menu.SetFirstThemeMain((SocketTextChannel)Context.Channel, (SocketGuildUser)Context.User);
                return;
            }
        }

        public async Task StatusDetails([Remainder]string arg = "")
        {
            // Establish variables for the command user and the command's target.
            SocketUser commandTarget = null;
            SocketUser commandUser = null;

            // Create a variable for a potential mentioned user.
            var mentionedUser = Context.Message.MentionedUsers.FirstOrDefault();

            // If there is a mentioned user, they become the command's target. If not, the command's user is also the target.
            commandTarget = mentionedUser ?? Context.User;
            commandUser = Context.User;

            // Get the account information of the command's target.
            var account = UserInfoClasses.GetAccount(commandTarget);

            // If a user is mentioned and they're not the command user and not a bot, add Expression to both users.
            if ((mentionedUser != null) && (mentionedUser != Context.User) && (mentionedUser.IsBot == false))
            {
                Core.LevelSystem.SocialStats.AddExpression(Context.Message, commandUser);
                Core.LevelSystem.SocialStats.AddExpression(Context.Message, commandTarget);
            }

            // Construct embeded message.
            var embed = new EmbedBuilder();

            // Determine color for embeded message.
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

            // Create a pleasant-looking string for Profile Theme section.
            string userProfileTheme = "";
            if (account.Profile_Theme == "")
            {
                userProfileTheme = "None";
            }
            else
            {
                userProfileTheme = account.Profile_Theme.ToUpper();
            }

            // Determine the Next Exp value.
            int next_exp = 0;
            if (account.Level != 99)
            {
                next_exp = Core.LevelSystem.Leveling.CalculateExp(account.Level + 1) - account.Total_Exp;
            }

            embed.WithTitle($"__{commandTarget.Username}'s Status__");
            embed.WithThumbnailUrl($"{commandTarget.GetAvatarUrl()}");

            // Divide the actual stats by 10 for the user view.
            decimal represented_proficiency = decimal.Round((decimal)account.Proficiency / 10, 2, MidpointRounding.AwayFromZero);
            decimal represented_diligence = decimal.Round((decimal)account.Diligence / 10, 2, MidpointRounding.AwayFromZero);
            decimal represented_expression = decimal.Round((decimal)account.Expression / 10, 2, MidpointRounding.AwayFromZero);

            // Create a string variable for the embed's description.
            string description_text = "" +
                $"**Level:** {account.Level}\n" +
                $"**Total Exp:** {account.Total_Exp}\n" +
                $"**Next Exp:** {next_exp}\n" +
                $"\n" +
                $"**Proficiency:** Rank {account.Proficiency_Rank} - *({represented_proficiency}/173)*\n" +
                $"**Diligence:** Rank {account.Diligence_Rank} - *({represented_diligence}/280)*\n" +
                $"**Expression:** Rank {account.Expression_Rank} - *({represented_expression}/170)*\n" +
                $"\n" +
                $"**Theme:** {userProfileTheme}\n" +
                $"**P-Medals:** {account.P_Medals}\n";

            // Check if the user has reset their level before.
            if (account.Level_Resets > 0)
            {
                // Append a new section to the end of the description text depending on how many times the user has reset their level.
                if (account.Level_Resets == 1)
                {
                    description_text += $":star: **Star Level Rank:** {account.Level_Resets}";
                }
                else if (account.Level_Resets == 2)
                {
                    description_text += $":star2: **Star Level Rank:** {account.Level_Resets}";
                }
                else if (account.Level_Resets == 3)
                {
                    description_text += $":sparkles: **Star Level Rank:** {account.Level_Resets}";
                }
            }

            // Add the description text to the embed.
            embed.WithDescription(description_text);

            await Context.Channel.SendMessageAsync("", false, embed.Build());
        }
    }
}
