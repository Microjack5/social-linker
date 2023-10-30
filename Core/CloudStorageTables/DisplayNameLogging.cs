using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using SocialLinker.Config;
using SocialLinker.Core.LocalStorageTables;
using SocialLinker.Core.SceneMaker;
using SocialLinker.Core.SceneMaker.Data.Bustup;

namespace SocialLinker.Core.CloudStorageTables
{
    public static class DisplayNameLogging
    {
        public static string logging_table = "CustomDisplayNames";

        static DisplayNameLogging()
        {
            //Log into account and specify table to work on
            var storageAccount = new CloudStorageAccount(new StorageCredentials(AzureConfig.azureAccount.accountName, AzureConfig.azureAccount.accountKey), true);
            var tableClient = storageAccount.CreateCloudTableClient();
            var displayNameTable = tableClient.GetTableReference(logging_table);

            //Create table if it does not exist
            displayNameTable.CreateIfNotExists();
        }

        public static DisplayNameTableData GetCustomName(ulong id, MakerCommandData maker_command_data, OfficialSetData set_data, BustupData bustup_data)
        {
            var storageAccount = new CloudStorageAccount(new StorageCredentials(AzureConfig.azureAccount.accountName, AzureConfig.azureAccount.accountKey), true);
            var tableClient = storageAccount.CreateCloudTableClient();
            var customNameTable = tableClient.GetTableReference(logging_table);

            var filter_1 = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, id.ToString()); // Match User ID
            var filter_2 = TableQuery.GenerateFilterCondition("Game", QueryComparisons.Equal, set_data.Origin);

            var name_filter = TableQuery.CombineFilters(filter_1, TableOperators.And, filter_2);

            var query = new TableQuery<DisplayNameTableData>().Where(name_filter);
            var results_list = customNameTable.ExecuteQuery(query).ToList();

            for (int i = 0; i < results_list.Count; i++)
            {
                if (maker_command_data.Character_Data.Base_Sprite == 0 || results_list[i].Sprites_Affected.Contains(bustup_data.Filename))
                {
                    return results_list[i];
                }
            }

            return null;
        }

        public static List<DisplayNameTableData> GetCustomNameList(ulong id)
        {
            var storageAccount = new CloudStorageAccount(new StorageCredentials(AzureConfig.azureAccount.accountName, AzureConfig.azureAccount.accountKey), true);
            var tableClient = storageAccount.CreateCloudTableClient();
            var customNameTable = tableClient.GetTableReference(logging_table);
            var id_search = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, id.ToString()); // Match User ID
            var query = new TableQuery<DisplayNameTableData>().Where(id_search);
            var result = customNameTable.ExecuteQuery(query).ToList();

            return result;
        }

        public static void DeleteCustomName(DisplayNameTableData custom_name)
        {
            var storageAccount = new CloudStorageAccount(new StorageCredentials(AzureConfig.azureAccount.accountName, AzureConfig.azureAccount.accountKey), true);
            var tableClient = storageAccount.CreateCloudTableClient();
            var customNameTable = tableClient.GetTableReference(logging_table);

            var filter_1 = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, custom_name.PartitionKey); // Match User ID
            var filter_2 = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, $"{custom_name.Entry_ID}");

            var name_filter_1 = TableQuery.CombineFilters(filter_1, TableOperators.And, filter_2);

            var query = new TableQuery<DisplayNameTableData>().Where(name_filter_1);
            var result = customNameTable.ExecuteQuery(query).FirstOrDefault();

            customNameTable.Execute(TableOperation.Delete(result));
        }

        public static bool Check_If_Sprites_Overlap(DisplayNameInternalData new_name_data)
        {
            var storageAccount = new CloudStorageAccount(new StorageCredentials(AzureConfig.azureAccount.accountName, AzureConfig.azureAccount.accountKey), true);
            var tableClient = storageAccount.CreateCloudTableClient();
            var customNameTable = tableClient.GetTableReference(logging_table);

            var filter_1 = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, new_name_data.User_ID); // Match User ID
            var filter_2 = TableQuery.GenerateFilterCondition("Game", QueryComparisons.Equal, new_name_data.Game);
            var filter_3 = TableQuery.GenerateFilterCondition("Character_ID", QueryComparisons.Equal, new_name_data.Sprite_Set.ID);

            var name_filter_1 = TableQuery.CombineFilters(filter_1, TableOperators.And, filter_2);
            var name_filter_2 = TableQuery.CombineFilters(name_filter_1, TableOperators.And, filter_3);

            var query = new TableQuery<DisplayNameTableData>().Where(name_filter_2);
            var existing_result = customNameTable.ExecuteQuery(query).FirstOrDefault();

            if (existing_result == null)
            {
                return false;
            }

            List<string> existing_sprite_list = String_To_String_List(existing_result.Sprites_Affected);
            List<string> new_sprite_list = String_To_String_List(new_name_data.Sprites_Affected);

            if (new_name_data.Spriteless_Included == "Yes" && existing_result.Spriteless_Included == "Yes")
            {
                return true;
            }
            else if (existing_sprite_list.Any(x => new_sprite_list.Any(y => y == x)))
            {
                return true;
            }

            return false;
        }

        // Utilities
        public static string NameListToString(List<DisplayNameTableData> name_list)
        {
            string list_string = "";
            int displayed_list_counter = 0;

            foreach (var name in name_list)
            {
                displayed_list_counter += 1;
                list_string += $"" +
                    $"{DecorInfoMethods.NumberToWords(displayed_list_counter)} **Display Name:** {name.Display_Name}\n" +
                    $"**Character:** \n" +
                    $"**Game:** {name.Game}\n" +
                    $"**Sprites Affected:** {name.Sprites_Affected}";
            }

            return list_string;
        }

        public static List<string> String_To_String_List(string input_string)
        {
            char[] delimiterChars = { ';' };
            List<string> string_list = input_string.Split(delimiterChars).ToList();
            string_list.RemoveAll(x => x.Length == 0); // Get rid of empty spaces in created list
            return string_list;
        }

        // Display Name Temp Data
        public static string String_Range_To_Int_Range(UserInfoFields account, OfficialSetData set_data, List<string> string_range, DisplayNameInternalData new_name_data)
        {
            string bustup_string = "";
            string set_path = $@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//{new_name_data.Sprite_Set.Origin}//Bustup//{new_name_data.Sprite_Set.ID}";
            int filecount = AttachmentCountItemDirectory(set_path);

            if (string_range.Count == filecount)
            {
                return "All";
            }
            else if (string_range.Count == 0)
            {
                return "None";
            }
            else
            {
                for (int i = 0; i < string_range.Count; i++)
                {
                    bustup_string += Bustup_Filename_To_Number(string_range[i], account, set_data);

                    if (i != string_range.Count - 1)
                    {
                        bustup_string += ", ";
                    }
                }

                return bustup_string;
            }
        }

        // Display Name Table Data
        public static string String_Range_To_Int_Range(UserInfoFields account, OfficialSetData set_data, List<string> string_range, DisplayNameTableData new_name_data)
        {
            string bustup_string = "";
            string set_path = $@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//{new_name_data.Game}//Bustup//{new_name_data.Character_ID}";
            int filecount = AttachmentCountItemDirectory(set_path);

            if (string_range.Count == filecount)
            {
                return "All";
            }
            else if (string_range.Count == 0)
            {
                return "None";
            }
            else
            {
                for (int i = 0; i < string_range.Count; i++)
                {
                    bustup_string += Bustup_Filename_To_Number(string_range[i], account, set_data);

                    if (i != string_range.Count - 1)
                    {
                        bustup_string += ", ";
                    }
                }

                return bustup_string;
            }
        }

        public static int Bustup_Filename_To_Number(string searched_filename, UserInfoFields account, OfficialSetData set_data)
        {
            string set_path = $@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//{set_data.Origin}//Bustup//{set_data.ID}";
            int filecount = OfficialSetMethods.AttachmentCountItemDirectory(set_path);
            int base_sprite_number = -1;

            if (Directory.Exists(set_path))
            {
                int counter = 0;

                if (account.Setting_Sheet_Order == "Order by Outfit")
                {
                    for (int outfit = 1; outfit <= filecount; outfit++)
                    {
                        for (int expression = 1; expression <= filecount; expression++)
                        {
                            string current_filename = $"{set_data.ID.ToLower()}_{expression}_{outfit}.png";

                            if (File.Exists($"{set_path}//{current_filename}"))
                            {
                                counter++;

                                if (current_filename == searched_filename)
                                {
                                    base_sprite_number = counter;
                                    break;
                                }
                            }
                        }

                        // Check if the filename variable for the base sprite is not default value set earlier.
                        if (base_sprite_number != -1)
                        {
                            break;
                        }
                    }
                }
                // Second case, Order by Expression.
                else if (account.Setting_Sheet_Order == "Order by Expression")
                {
                    for (int expression = 1; expression <= filecount; expression++)
                    {
                        for (int outfit = 1; outfit <= filecount; outfit++)
                        {
                            string current_filename = $"{set_data.ID.ToLower()}_{expression}_{outfit}.png";

                            if (File.Exists($"{set_path}//{current_filename}"))
                            {
                                counter++;

                                if (current_filename == searched_filename)
                                {
                                    base_sprite_number = counter;
                                    break;
                                }
                            }
                        }

                        // Check if the filename variable for the base sprite is not default value set earlier.
                        if (base_sprite_number != -1)
                        {
                            break;
                        }
                    }
                }
            }

            return base_sprite_number;
        }

        private static int AttachmentCountItemDirectory(string set_path)
        {
            string[] attExt = { ".png" };
            return Directory.EnumerateFiles(set_path)
              .Count(f => attExt.Contains(Path.GetExtension(f), StringComparer.InvariantCultureIgnoreCase));
        }
    }

    public class DisplayNameTableData : TableEntity
    {
        public string User_ID => PartitionKey;
        public string Entry_ID => RowKey;
        public string Display_Name { get; set; }
        public string Game { get; set; }
        public string Character_ID { get; set; }
        public string Sprites_Affected { get; set; }
        public string Spriteless_Included { get; set; }
    }

    public class DisplayNameInternalData
    {
        public string User_ID { get; set; }
        public string Display_Name { get; set; }
        public string Game { get; set; }
        public string Character_ID { get; set; }
        public string Sprites_Affected { get; set; }
        public string Spriteless_Included { get; set; }
        public OfficialSetData Sprite_Set { get; set; }
        public int Sprite_Count { get; set; }
    }
}
