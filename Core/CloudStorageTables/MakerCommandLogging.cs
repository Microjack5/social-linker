using System;
using Discord.WebSocket;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using SocialLinker.Config;
using SocialLinker.Core.SceneMaker;

namespace SocialLinker.Core.CloudStorageTables
{
    internal class MakerCommandLogging
    {
        public static string logging_table = "MakerCommandLogs";

        static MakerCommandLogging()
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
            MakerCommandData maker_command_data = sl_command_data.MakerCommand;
            bool bg_bool = false;

            if (maker_command_data.Background != default)
            {
                bg_bool = true;
            }

            if (maker_command_data.Template == default)
            {
                maker_command_data.Template = "None";
            }

            MakerCommandLogData table_submission = new MakerCommandLogData()
            {
                PartitionKey = sl_command_data.User.Id.ToString(),
                RowKey = string.Format("{0:D19}", DateTime.MaxValue.Ticks - DateTime.UtcNow.Ticks),
                Guild_ID = ((SocketGuildChannel)sl_command_data.Channel).Guild.Id.ToString(),
                Channel_ID = sl_command_data.Channel.Id.ToString(),
                Template = maker_command_data.Template,
                Character_Keyword = maker_command_data.Character_Keyword,
                Sprite_Set_Version = maker_command_data.Sprite_Set_Version,
                Base_Sprite = maker_command_data.Base_Sprite,
                Eye_Frame = maker_command_data.Eye_Frame,
                Mouth_Frame = maker_command_data.Mouth_Frame,
                Dialogue = maker_command_data.Dialogue,
                Background_Used = bg_bool
            };

            var storageAccount = new CloudStorageAccount(new StorageCredentials(AzureConfig.azureAccount.accountName, AzureConfig.azureAccount.accountKey), true);
            var tableClient = storageAccount.CreateCloudTableClient();
            var commandLogTable = tableClient.GetTableReference(logging_table);

            commandLogTable.Execute(TableOperation.InsertOrReplace(table_submission));
        }
    }

    public class MakerCommandLogData : TableEntity
    {
        public string User_ID => PartitionKey;
        public string Time_Created => RowKey; // Reverse ticks, solved by: return new DateTime(DateTime.MaxValue.Ticks - long.Parse(timestamp), DateTimeKind.Utc);
        public string Guild_ID { get; set; }
        public string Channel_ID { get; set; }
        public string Template { get; set; }
        public string Character_Keyword { get; set; }
        public string Sprite_Set_Version { get; set; }
        public int Base_Sprite { get; set; }
        public int Eye_Frame { get; set; }
        public int Mouth_Frame { get; set; }
        public string Dialogue { get; set; }
        public bool Background_Used { get; set; }
    }
}
