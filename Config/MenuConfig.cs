using System.IO;
using Newtonsoft.Json;

namespace SocialLinker.Config
{
    class MenuConfig
    {
        private const string configFolder = "Resources";
        private const string configFile = "MenuConfig.json";

        public static MenuConfiguration menu;

        static MenuConfig()
        {
            if (!Directory.Exists(configFolder))
            {
                Directory.CreateDirectory(configFolder);
            }

            if (!File.Exists(configFolder + "/" + configFile))
            {
                menu = new MenuConfiguration();
                string json = JsonConvert.SerializeObject(menu, Formatting.Indented);
                File.WriteAllText(configFolder + "/" + configFile, json);
            }
            else
            {
                string json = File.ReadAllText(configFolder + "/" + configFile);
                menu = JsonConvert.DeserializeObject<MenuConfiguration>(json);
            }
        }
    }

    public struct MenuConfiguration
    {
        public int reactionAddedDelay;
        public int timerDuration;
    }
}
