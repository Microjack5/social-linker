using Newtonsoft.Json;
using SocialLinker.Config;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
using System.Globalization;

namespace SocialLinker.Core.SceneMaker.Data.Calendar
{
    class AcademicDataMethods
    {
        private static List<AcademicData> academic_data_list;
        string data_folder = $@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Data//Calendar_Data";
        string data_sheet = $"";

        public AcademicDataMethods(DateTime user_time)
        {
            data_sheet = $"academic_calendar_{user_time.Year}.json";

            if (!Directory.Exists(data_folder))
            {
                Directory.CreateDirectory(data_folder);
            }

            if (File.Exists(data_folder + "/" + data_sheet))
            {
                academic_data_list = Load_Academic_Data_List(data_folder + "/" + data_sheet).ToList();
            }
            else
            {
                academic_data_list = new List<AcademicData>();
                Save_Academic_List(academic_data_list, data_folder + "/" + data_sheet);
            }
        }

        public static IEnumerable<AcademicData> Load_Academic_Data_List(string filePath)
        {
            // If the path specified doesn't exist, return null.
            if (!File.Exists(filePath)) return null;

            // Otherwise, deserialize the list within the JSON file and return.
            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<AcademicData>>(json);
        }

        public static void Save_Academic_List(IEnumerable<AcademicData> academic_list, string filePath)
        {
            string json = JsonConvert.SerializeObject(academic_list, Formatting.Indented, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore
            });
            File.WriteAllText(filePath, json);
        }

        public bool Is_School_Term(DateTime user_time)
        {
            string last_term_notification = "";

            foreach (AcademicData date in academic_data_list)
            {
                DateTime academic_date = new DateTime(user_time.Year, DateTime.ParseExact(date.Month, "MMMM", CultureInfo.CurrentCulture).Month, date.Day);
                DateTime user_date = new DateTime(user_time.Year, user_time.Month, user_time.Day);

                if (user_date >= academic_date)
                {
                    last_term_notification = date.Condition.ToString();
                }
            }

            switch (last_term_notification)
            {
                case "First Day of School":
                    return true;

                case "Closing Ceremony":
                    return false;

                default:
                    return true;
            }
        }
    }
}
