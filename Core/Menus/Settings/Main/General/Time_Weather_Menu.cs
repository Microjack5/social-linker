using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using SocialLinker.Config;
using SocialLinker.Core.CloudStorageTables;
using Discord.Rest;
using Discord.Commands;
using Discord.Addons.Interactive;
using System.Net;
using Newtonsoft.Json;

namespace SocialLinker.Core.Menus.Settings.Main.General
{
    class Time_Weather_Menu : InteractiveBase<SocketCommandContext>
    {
        public static async Task Time_Weather_Main(SocketGuildUser user, RestUserMessage message)
        {
            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            // Find the menu session associated with the current user.
            var menuSession = Global.MenuIdList.SingleOrDefault(x => x.User.Id == user.Id);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Time Zone & Weather",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            var footer = new EmbedFooterBuilder
            {
                Text = "↩️ Return to General Settings Menu"
            };

            embed.WithFooter(footer);

            // Determine the color and thumbnail for the embeded message
            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Profile_Config_Thumbnail(account));

            // Get the date, time, and weather information for the user's location and store it in a variable.
            var dataObject = Get_Weather_API_Info(account.City);

            embed.WithDescription("" +
                "Set a city to automatically configure the date, times of day, and weather readings for your user profile.\n" +
                "\n" +
                $"⚙️ **Current Setting:**\n" +
                $"\n" +
                $"Location: **`{dataObject.location.name.ToString()}`**\n" +
                $"Region: **`{dataObject.location.region.ToString()}`**\n" +
                $"Country: **`{dataObject.location.country.ToString()}`**\n" +
                $"\n" +
                $"Type in a city you’d like to use with Social Linker, or react with ↩️ to cancel.");

            // Attempt editing the message if it hasn't been deleted by the user yet.
            // If it has, catch the exception, remove the menu entry from the global list, and return.
            try
            {
                // Remove all reactions from the current message.
                await message.RemoveAllReactionsAsync();

                // Edit the current active message by replacing it with the recently created embed.
                await message.ModifyAsync(x => {
                    x.Embed = embed.Build();
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                // Remove the menu entry from the global list.
                Global.MenuIdList.Remove(menuSession);

                return;
            }

            // Edit the menu session according to the current message.
            menuSession.CurrentMenu = "Time_Weather_Main";
            menuSession.MenuTimer = new Timer()
            {
                // Create a timer that expires as a "time out" duration for the user.
                Interval = MenuConfig.menu.timerDuration,
                AutoReset = false,
                Enabled = true
            };

            // If the menu timer runs out, activate a function.
            menuSession.MenuTimer.Elapsed += (sender, e) => MenuTimer_Elapsed(sender, e, menuSession);

            // Create an empty list for reactions.
            List<IEmote> reaction_list = new List<IEmote> { };

            // Add needed emote reactions for the menu.
            reaction_list.Add(new Emoji("↩️"));

            // Add the reactions to the message.
            _ = ReactionHandling.AddReactionsToMenu(message, reaction_list);
        }

        public static async Task Time_Weather_Error(SocketGuildUser user, RestUserMessage message)
        {
            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            // Find the menu session associated with the current user.
            var menuSession = Global.MenuIdList.SingleOrDefault(x => x.User.Id == user.Id);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Location Not Found",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            var footer = new EmbedFooterBuilder
            {
                Text = "↩️ Retry"
            };

            embed.WithFooter(footer);

            // Determine the color and thumbnail for the embeded message.
            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Profile_Config_Thumbnail(account));

            embed.WithDescription("" +
                "It looks like an invalid city was typed in. React with ↩️ to try again.\n");

            embed.AddField("Tips", "" +
                "Try using the names of major cities or other well-known locations.");

            // Attempt editing the message if it hasn't been deleted by the user yet.
            // If it has, catch the exception, remove the menu entry from the global list, and return.
            try
            {
                // Remove all reactions from the current message.
                await message.RemoveAllReactionsAsync();

                // Edit the current active message by replacing it with the recently created embed.
                await message.ModifyAsync(x => {
                    x.Embed = embed.Build();
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                // Remove the menu entry from the global list.
                Global.MenuIdList.Remove(menuSession);

                return;
            }

            // Edit the menu session according to the current message.
            menuSession.CurrentMenu = "Time_Weather_Error";
            menuSession.MenuTimer = new Timer()
            {
                // Create a timer that expires as a "time out" duration for the user.
                Interval = MenuConfig.menu.timerDuration,
                AutoReset = false,
                Enabled = true
            };

            // If the menu timer runs out, activate a function.
            menuSession.MenuTimer.Elapsed += (sender, e) => MenuTimer_Elapsed(sender, e, menuSession);

            // Create an empty list for reactions.
            List<IEmote> reaction_list = new List<IEmote> { };

            // Add needed emote reactions for the menu.
            reaction_list.Add(new Emoji("↩️"));

            // Add the reactions to the message.
            _ = ReactionHandling.AddReactionsToMenu(message, reaction_list);
        }

        public static async Task Time_Weather_Confirm(SocketGuildUser user, RestUserMessage message)
        {
            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            // Find the menu session associated with the current user.
            var menuSession = Global.MenuIdList.SingleOrDefault(x => x.User.Id == user.Id);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Settings Saved",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            var footer = new EmbedFooterBuilder
            {
                Text = "💠 General Settings | ❌ Close Menu"
            };

            embed.WithFooter(footer);

            // Determine the color and thumbnail for the embeded message
            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Profile_Config_Thumbnail(account));

            // Get the date, time, and weather information for the user's location and store it in a variable.
            var dataObject = Get_Weather_API_Info(account.City);

            // Parse the data object's local time variable string into a DateTime object.
            DateTime local_time = DateTime.Parse(dataObject.location.localtime.ToString());

            embed.WithDescription("" +
                $"Your location has been set.\n" +
                $"\n" +
                $"Location: **`{dataObject.location.name.ToString()}`**\n" +
                $"Region: **`{dataObject.location.region.ToString()}`**\n" +
                $"Country: **`{dataObject.location.country.ToString()}`**\n" +
                $"\n" +
                $"Date: **`{local_time.ToString("dddd, MMMM dd yyyy")}`**\n" +
                $"Time: **`{local_time.ToString("HH:mm")}`**\n" +
                $"Weather: **`{dataObject.current.condition.text.ToString()}`**\n");

            // Attempt editing the message if it hasn't been deleted by the user yet.
            // If it has, catch the exception, remove the menu entry from the global list, and return.
            try
            {
                // Remove all reactions from the current message.
                await message.RemoveAllReactionsAsync();

                // Edit the current active message by replacing it with the recently created embed.
                await message.ModifyAsync(x => {
                    x.Embed = embed.Build();
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                // Remove the menu entry from the global list.
                Global.MenuIdList.Remove(menuSession);

                return;
            }

            // Edit the menu session according to the current message.
            menuSession.CurrentMenu = "Time_Weather_Confirm";
            menuSession.MenuTimer = new Timer()
            {
                // Create a timer that expires as a "time out" duration for the user.
                Interval = MenuConfig.menu.timerDuration,
                AutoReset = false,
                Enabled = true
            };

            // If the menu timer runs out, activate a function.
            menuSession.MenuTimer.Elapsed += (sender, e) => MenuTimer_Elapsed(sender, e, menuSession);

            // Create an empty list for reactions.
            List<IEmote> reaction_list = new List<IEmote> { };

            // Add needed emote reactions for the menu.
            reaction_list.Add(new Emoji("💠"));
            reaction_list.Add(new Emoji("❌"));

            // Add the reactions to the message.
            _ = ReactionHandling.AddReactionsToMenu(message, reaction_list);
        }

        private static async void MenuTimer_Elapsed(object sender, ElapsedEventArgs e, MenuIdStructure menuSession)
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
                await message.ModifyAsync(x => {
                    x.Embed = MenuTimedOut(menuSession.User).Build();
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

        public static EmbedBuilder MenuTimedOut(SocketGuildUser user)
        {
            // Get the account information of the command's target
            var account = UserInfoClasses.GetAccount(user);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Inactive Menu",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            // Determine the color and thumbnail for the embeded message
            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Profile_Help_Thumbnail(account));

            embed.WithDescription($"You can set your time zone & weather at any time from the **`{BotConfig.bot.cmdPrefix}settings`** menu by choosing [General Settings] > [Time Zone & Weather].");
            return embed;
        }

        // Methods that suppliment the functionality of the menus.
        public static dynamic Get_Weather_API_Info(string location)
        {
            // Create an empty string variable. This is where the API request will be stored.
            string json_location = "";

            // Encapsulate the API request in a try-catch block. If the user inputs an invalid parameter, an exception will be thrown.
            try
            {
                // Make an API request with the account key and user input as parameters.
                using (WebClient client = new WebClient())
                {
                    json_location = client.DownloadString($"http://api.weatherapi.com/v1/current.json?key={WeatherAPIConfig.weather_api_account.accountKey}&q={location}");
                }
            }
            catch (Exception ex)
            {
                // Write the exception to the console.
                Console.WriteLine(ex);
            }

            // Deserialize the JSON object and store it in a variable.
            var dataObject = JsonConvert.DeserializeObject<dynamic>(json_location);

            // Return the data object.
            return dataObject;
        }
    }
}
