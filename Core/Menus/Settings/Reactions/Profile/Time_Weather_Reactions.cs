using System;
using System.Net;
using System.Threading.Tasks;
using Discord.WebSocket;
using Newtonsoft.Json;
using SocialLinker.Config;
using SocialLinker.Core.CloudStorageTables;
using SocialLinker.Core.Menus.Settings.Main.Profile;

namespace SocialLinker.Core.Menus.Settings.Reactions.Profile
{
    class Time_Weather_Reactions
    {
        public static Task Nav_Time_Weather_Main(SocketReaction reaction, MenuIdStructure menuSession)
        {
            if (reaction.Emote.Name == "↩️")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Profile_Settings_Menu.Profile_Settings_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Time_Weather_Error(SocketReaction reaction, MenuIdStructure menuSession)
        {
            if (reaction.Emote.Name == "↩️")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Time_Weather_Menu.Time_Weather_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Time_Weather_Confirm(SocketReaction reaction, MenuIdStructure menuSession)
        {
            if (reaction.Emote.Name == "💠")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Profile_Settings_Menu.Profile_Settings_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            else if (reaction.Emote.Name == "❌")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Attempt to delete the menu message from the channel if it hasn't been deleted by the user yet. If this fails, catch the exception.
                try
                {
                    _ = menuSession.MenuMessage.DeleteAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                // If the menu session is not null, remove it from the global list.
                if (menuSession != null)
                {
                    Global.MenuIdList.Remove(menuSession);
                }
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        // Methods that activate on the MessageReceived event.
        public static Task Nav_Time_Weather_Main_Received(SocketMessage message, MenuIdStructure menuSession)
        {
            // Store the text of the message in a string and convert all letters to uppercase.
            string input_string = message.Content.ToUpper();

            input_string = Global.RemoveBotMention(input_string).Trim();

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
                _ = Time_Weather_Menu.Time_Weather_Error(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            // Deserialize the JSON object and store it in a variable.
            var dataObject = JsonConvert.DeserializeObject<dynamic>(json_location);

            // Get the account information of the user.
            var account = UserInfoClasses.GetAccount(menuSession.User);

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
            _ = Time_Weather_Menu.Time_Weather_Confirm(menuSession.User, menuSession.MenuMessage);
            return Task.CompletedTask;
        }
    }
}
