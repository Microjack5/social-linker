using System.Linq;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using SocialLinker.Config;

namespace SocialLinker.Core.CloudStorageTables
{
    public static class CalendarCycles
    {
        public static string logging_table = "CalendarCycles";

        static CalendarCycles()
        {
            //Log into account and specify table to work on
            var storageAccount = new CloudStorageAccount(new StorageCredentials(AzureConfig.azureAccount.accountName, AzureConfig.azureAccount.accountKey), true);
            var tableClient = storageAccount.CreateCloudTableClient();
            var displayNameTable = tableClient.GetTableReference(logging_table);

            //Create table if it does not exist
            displayNameTable.CreateIfNotExists();
        }

        public static CalendarCycleFields GetUserCalendarCycles(ulong id)
        {
            var storageAccount = new CloudStorageAccount(new StorageCredentials(AzureConfig.azureAccount.accountName, AzureConfig.azureAccount.accountKey), true);
            var tableClient = storageAccount.CreateCloudTableClient();
            var customNameTable = tableClient.GetTableReference(logging_table);

            var id_filter = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, id.ToString()); // Match User ID

            var query = new TableQuery<CalendarCycleFields>().Where(id_filter).FirstOrDefault();

            return query;
        }

        public class CalendarCycleFields : TableEntity
        {
            public string Platform => PartitionKey;
            public string User_ID => RowKey;
            public string P1_PSX_Calendar_Cycle_Override { get; set; }
            public string P1_PSX_Calendar_Cycle_Moon_Phase { get; set; }
            public string P1_PSP_Calendar_Cycle_Override { get; set; }
            public string P1_PSP_Calendar_Cycle_Moon_Phase { get; set; }
            public string P3F_Calendar_Cycle_Override { get; set; }
            public string P3F_Calendar_Cycle_Moon_Phase { get; set; }
            public string P3P_Calendar_Cycle_Override { get; set; }
            public string P3R_Calendar_Cycle_Override { get; set; }
            public string P4_PS2_Calendar_Cycle_Override { get; set; }
            public string P4G_Calendar_Cycle_Override { get; set; }
            public string P5_PS3_Calendar_Cycle_Override { get; set; }
            public string P5R_Calendar_Cycle_Override { get; set; }
            public string P5R_Calendar_Cycle_Month { get; set; }
            public string P5R_Calendar_Cycle_Day { get; set; }
            public string P5R_Calendar_Cycle_Day_of_Week { get; set; }
            public string P5R_Calendar_Cycle_Time_of_Day { get; set; }
            public string P5S_Calendar_Cycle_Cycle_Override { get; set; }
            public string P5S_Calendar_Cycle_Month { get; set; }
            public string P5S_Calendar_Cycle_Day { get; set; }
            public string P5S_Calendar_Cycle_Day_of_Week { get; set; }
            public string P5S_Calendar_Cycle_Time_of_Day { get; set; }
        }
    }
}
