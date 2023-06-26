using Newtonsoft.Json;
using SocialLinker.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SocialLinker.Core.SceneMaker.Data.Calendar
{
    class HolidayDataMethods
    {
        private static List<HolidayData> holiday_data_list;
        string data_folder = $@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Data//Calendar_Data";
        string data_sheet = $"";

        public HolidayDataMethods(DateTime user_time)
        {
            data_sheet = $"holiday_calendar_{user_time.Year}.json";

            if (!Directory.Exists(data_folder))
            {
                Directory.CreateDirectory(data_folder);
            }

            if (File.Exists(data_folder + "/" + data_sheet))
            {
                holiday_data_list = Load_Holiday_Data_List(data_folder + "/" + data_sheet).ToList();
            }
            else
            {
                holiday_data_list = new List<HolidayData>();
                Save_Holiday_List(holiday_data_list, data_folder + "/" + data_sheet);
            }
        }

        public static IEnumerable<HolidayData> Load_Holiday_Data_List(string filePath)
        {
            // If the path specified doesn't exist, return null.
            if (!File.Exists(filePath)) return null;

            // Otherwise, deserialize the list within the JSON file and return.
            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<HolidayData>>(json);
        }

        public static void Save_Holiday_List(IEnumerable<HolidayData> holiday_list, string filePath)
        {
            string json = JsonConvert.SerializeObject(holiday_list, Formatting.Indented, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore
            });
            File.WriteAllText(filePath, json);
        }

        public bool Is_Holiday(DateTime user_time)
        {
            foreach (HolidayData date in holiday_data_list)
            {
                if (date.Month == user_time.ToString("MMMM") && date.Day == user_time.Day)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
