using System.IO;
using Newtonsoft.Json;

namespace SocialLinker.Config
{
    class AssetDirectoryConfig
    {
        private const string configFolder = "Resources";
        private const string configFile = "AssetDirectory.json";

        public static AssetDirectory assetDirectory;

        static AssetDirectoryConfig()
        {
            if (!Directory.Exists(configFolder))
            {
                Directory.CreateDirectory(configFolder);
            }

            if (!File.Exists(configFolder + "/" + configFile))
            {
                assetDirectory = new AssetDirectory();
                string json = JsonConvert.SerializeObject(assetDirectory, Formatting.Indented);
                File.WriteAllText(configFolder + "/" + configFile, json);
            }
            else
            {
                string json = File.ReadAllText(configFolder + "/" + configFile);
                assetDirectory = JsonConvert.DeserializeObject<AssetDirectory>(json);
            }
        }
    }

    public struct AssetDirectory
    {
        public string assetFolderPath;
    }
}
