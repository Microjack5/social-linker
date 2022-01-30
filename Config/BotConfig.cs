using System.IO;
using Newtonsoft.Json;

namespace SocialLinker.Config
{
    class BotConfig
    {
        private const string configFolder = "Resources";
        private const string configFile = "Config.json";

        public static BotConfiguration bot;

        static BotConfig()
        {
            if (!Directory.Exists(configFolder))
            {
                Directory.CreateDirectory(configFolder);
            }

            if (!File.Exists(configFolder + "/" + configFile))
            {
                bot = new BotConfiguration();
                string json = JsonConvert.SerializeObject(bot, Formatting.Indented);
                File.WriteAllText(configFolder + "/" + configFile, json);
            }
            else
            {
                string json = File.ReadAllText(configFolder + "/" + configFile);
                bot = JsonConvert.DeserializeObject<BotConfiguration>(json);
            }
        }
    }

    public struct BotConfiguration
    {
        public string token;
        public string cmdPrefix;
        public ulong id;
    }
}
