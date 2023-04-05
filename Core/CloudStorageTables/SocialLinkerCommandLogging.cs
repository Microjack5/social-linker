using System;
using Discord.WebSocket;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using SocialLinker.Config;

namespace SocialLinker.Core.CloudStorageTables
{
    internal class SocialLinkerCommandLogging
    {
        public static string logging_table = "SocialLinkerCommandLogs";

        static SocialLinkerCommandLogging()
        {
            //Log into account and specify table to work on
            var storageAccount = new CloudStorageAccount(new StorageCredentials(AzureConfig.azureAccount.accountName, AzureConfig.azureAccount.accountKey), true);
            var tableClient = storageAccount.CreateCloudTableClient();
            var commandLogTable = tableClient.GetTableReference(logging_table);

            //Create table if it does not exist
            commandLogTable.CreateIfNotExists();
        }

        public static void LogData(SocialLinkerCommand sl_command_data)
        {
            string mentioned_user = "";

            if (sl_command_data.MentionedUser != null)
            {
                mentioned_user = sl_command_data.MentionedUser.Id.ToString();
            }

            SLCommandLogData table_submission = new SLCommandLogData()
            {
                PartitionKey = sl_command_data.User.Id.ToString(),
                RowKey = string.Format("{0:D19}", DateTime.MaxValue.Ticks - DateTime.UtcNow.Ticks),
                Guild_ID = ((SocketGuildChannel)sl_command_data.Channel).Guild.Id.ToString(),
                Channel_ID = sl_command_data.Channel.Id.ToString(),
                Command_Type = sl_command_data.CommandType,
                Command_Name = sl_command_data.CommandName,
                Mentioned_User_ID = mentioned_user,
            };

            var storageAccount = new CloudStorageAccount(new StorageCredentials(AzureConfig.azureAccount.accountName, AzureConfig.azureAccount.accountKey), true);
            var tableClient = storageAccount.CreateCloudTableClient();
            var commandLogTable = tableClient.GetTableReference(logging_table);

            commandLogTable.Execute(TableOperation.InsertOrReplace(table_submission));
        }
    }

    public class SLCommandLogData : TableEntity
    {
        public string User_ID => PartitionKey;
        public string Time_Created => RowKey; // Reverse ticks, solved by: return new DateTime(DateTime.MaxValue.Ticks - long.Parse(timestamp), DateTimeKind.Utc);
        public string Guild_ID { get; set; }
        public string Channel_ID { get; set; }
        public string Command_Type { get; set; }
        public string Command_Name { get; set; }
        public string Mentioned_User_ID { get; set; }
    }
}
