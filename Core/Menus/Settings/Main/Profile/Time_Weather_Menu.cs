using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json;
using SocialLinker.Config;
using System;
using System.Net;
using System.Threading.Tasks;

namespace SocialLinker.Core.Menus.Settings.Main.Profile
{
    class Time_Weather_Menu : ModuleBase<SocketCommandContext>
    {
        public static async Task Time_Weather_Main(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Time Zone & Weather",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            // Determine the color and thumbnail for the embeded message
            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Profile_Config_Thumbnail(account));

            // Get the date, time, and weather information for the user's location and store it in a variable.
            var dataObject = Get_Weather_API_Info(account.City);

            embed.WithDescription("" +
                "Set a city to automatically configure the date, times of day, and weather readings for your user profile and scene maker images.\n" +
                "\n" +
                $"⚙️ **Current Setting:**\n" +
                $"\n" +
                $"Location: **`{dataObject.location.name.ToString()}`**\n" +
                $"Region: **`{dataObject.location.region.ToString()}`**\n" +
                $"Country: **`{dataObject.location.country.ToString()}`**\n" +
                $"\n" +
                $"Type in a city or postal code you’d like to use with Social Linker.\n" +
                $"\n" +
                $":warning: Changing this setting in private is recommended if using local locations.");

            menuSession.CurrentMenu = "Time_Weather_Main";

            var component = new ComponentBuilder()
                .WithButton("Enter Location", customId: "time-weather-modal-open", ButtonStyle.Primary)
                .WithButton("↩️ Return", customId: "return", ButtonStyle.Secondary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Time_Weather_Modal(SocketMessageComponent component)
        {
            if (component.Data.CustomId == "time-weather-modal-open")
            {
                try
                {
                    var modal = new ModalBuilder()
                    .WithTitle("Location Entry")
                    .WithCustomId("time-weather-modal-submit")
                    .AddTextInput("City or Postal Code", "location");

                    await component.RespondWithModalAsync(modal.Build());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public static async Task Time_Weather_Error(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Location Not Found",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

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

            var component = new ComponentBuilder()
                .WithButton("↩️ Return", customId: "return", ButtonStyle.Secondary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Time_Weather_Confirm(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Settings Saved",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

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

            menuSession.CurrentMenu = "Time_Weather_Confirm";

            var component = new ComponentBuilder()
                .WithButton("💠 Profile Settings", customId: "profile-settings", ButtonStyle.Secondary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
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
