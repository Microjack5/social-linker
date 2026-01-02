using Discord.WebSocket;
using Newtonsoft.Json;
using SocialLinker.Config;
using SocialLinker.Core.CloudStorageTables;
using SocialLinker.Core.Menus.Settings.Main.Profile;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SocialLinker.Core.Menus.Settings.Reactions.Profile
{
    class Time_Weather_Reactions
    {
        public static Task Nav_Time_Weather_Main(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "time-weather-modal-open":
                    _ = Time_Weather_Menu.Time_Weather_Modal(component);
                    break;

                case "return":
                    _ = Profile_Settings_Menu.Profile_Settings_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static async Task Nav_Time_Weather_Modal(SocketModal modal, MenuIdStructure menuSession)
        {
            var account = menuSession.Account;

            var location = modal.Data.Components
            .FirstOrDefault(x => x.CustomId == "location")?.Value;

            await Nav_Time_Weather_Main_Received(location, menuSession);
            return;
        }

        public static Task Nav_Time_Weather_Error(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "return":
                    _ = Time_Weather_Menu.Time_Weather_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Time_Weather_Confirm(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "profile-settings":
                    _ = Profile_Settings_Menu.Profile_Settings_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        // Utility methods for data prossessing
        public static Task Nav_Time_Weather_Main_Received(string location, MenuIdStructure menuSession)
        {
            var account = menuSession.Account;

            Console.WriteLine($"Location is {location}");

            // Store the text of the message in a string and convert all letters to uppercase.
            string input_string = location.ToUpper();

            // Create an empty string variable. This is where the API request will be stored.
            string json_location = "";

            // Encapsulate the API request in a try-catch block. If the user inputs an invalid parameter, an exception will be thrown.
            try
            {
                // Make an API request with the account key and user input as parameters.
                using (WebClient client = new WebClient())
                {
                    json_location = client.DownloadString($"http://api.weatherapi.com/v1/current.json?key={WeatherAPIConfig.weather_api_account.accountKey}&q={input_string}");
                }
            }
            catch (Exception ex)
            {
                // Write the exception to the console.
                Console.WriteLine(ex);

                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Time_Weather_Menu.Time_Weather_Error(menuSession);
                return Task.CompletedTask;
            }

            // Deserialize the JSON object and store it in a variable.
            var dataObject = JsonConvert.DeserializeObject<dynamic>(json_location);

            // Check if the location name contains any part of the user's input string.
            // The lowercase forms are compared to check for this match.
            if (dataObject.location.name.ToString().ToLower().Contains(input_string.ToLower()))
            {
                // If so, assign the proper location name to the user's account.
                account.City = $"{dataObject.location.name.ToString()}";
            }
            // If not, assign the user's input to their account settings.
            else
            {
                account.City = input_string;
            }

            //Update the user's account.
            UserInfoClasses.UpdateAccount(account);

            // Stop the timeout timer associated with the menu.
            menuSession.MenuTimer.Stop();

            // Go to a new menu.
            _ = Time_Weather_Menu.Time_Weather_Confirm(menuSession);
            return Task.CompletedTask;
        }
    }
}
