using System.IO;
using Newtonsoft.Json;

namespace SocialLinker.Config
{
    class WeatherAPIConfig
    {
        private const string configFolder = "Resources";
        private const string configFile = "WeatherAPIConfig.json";

        public static WeatherAPIConfiguration weather_api_account;

        static WeatherAPIConfig()
        {
            if (!Directory.Exists(configFolder))
            {
                Directory.CreateDirectory(configFolder);
            }

            if (!File.Exists(configFolder + "/" + configFile))
            {
                weather_api_account = new WeatherAPIConfiguration();
                string json = JsonConvert.SerializeObject(weather_api_account, Formatting.Indented);
                File.WriteAllText(configFolder + "/" + configFile, json);
            }
            else
            {
                string json = File.ReadAllText(configFolder + "/" + configFile);
                weather_api_account = JsonConvert.DeserializeObject<WeatherAPIConfiguration>(json);
            }
        }
    }

    public struct WeatherAPIConfiguration
    {
        public string accountKey;
    }
}
