using System.IO;
using Newtonsoft.Json;

namespace SocialLinker.Config
{
    class AzureConfig
    {
        private const string configFolder = "Resources";
        private const string configFile = "AzureConfig.json";

        public static AzureConfiguration azureAccount;

        static AzureConfig()
        {
            if (!Directory.Exists(configFolder))
            {
                Directory.CreateDirectory(configFolder);
            }

            if (!File.Exists(configFolder + "/" + configFile))
            {
                azureAccount = new AzureConfiguration();
                string json = JsonConvert.SerializeObject(azureAccount, Formatting.Indented);
                File.WriteAllText(configFolder + "/" + configFile, json);
            }
            else
            {
                string json = File.ReadAllText(configFolder + "/" + configFile);
                azureAccount = JsonConvert.DeserializeObject<AzureConfiguration>(json);
            }
        }
    }

    public struct AzureConfiguration
    {
        public string accountName;
        public string accountKey;
    }
}
