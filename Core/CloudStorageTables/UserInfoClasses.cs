using System;
using System.Collections.Generic;
using System.Linq;
using Discord.WebSocket;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using SocialLinker.Config;

namespace SocialLinker.Core.CloudStorageTables
{
    public static class UserInfoClasses
    {
        private static string accountsTable = "UserInformation"; //"UserInformationDev"

        static UserInfoClasses()
        {
            //Log into account and specify table to work on
            var storageAccount = new CloudStorageAccount(new StorageCredentials(AzureConfig.azureAccount.accountName, AzureConfig.azureAccount.accountKey), true);
            var tableClient = storageAccount.CreateCloudTableClient();
            var userInfoTable = tableClient.GetTableReference(accountsTable);

            //Create table if it does not exist
            userInfoTable.CreateIfNotExists();
        }

        public static UserInfoFields GetAccount(SocketUser user)
        {
            return GetOrCreateAccount(user.Id);
        }

        public static void UpdateAccount(UserInfoFields alteredAccount)
        {
            //Specify table information
            var storageAccount = new CloudStorageAccount(new StorageCredentials(AzureConfig.azureAccount.accountName, AzureConfig.azureAccount.accountKey), true);
            var tableClient = storageAccount.CreateCloudTableClient();
            var userInfoTable = tableClient.GetTableReference(accountsTable);

            //Update user fields with new data
            userInfoTable.Execute(TableOperation.Merge(alteredAccount));
        }

        private static UserInfoFields GetOrCreateAccount(ulong id)
        {
            //Specify table information
            var storageAccount = new CloudStorageAccount(new StorageCredentials(AzureConfig.azureAccount.accountName, AzureConfig.azureAccount.accountKey), true);
            var tableClient = storageAccount.CreateCloudTableClient();
            var userInfoTable = tableClient.GetTableReference(accountsTable);

            //Retrieve user information
            var tableResult = userInfoTable.Execute(TableOperation.Retrieve<UserInfoFields>("Discord", id.ToString()));
            var account = (UserInfoFields)tableResult.Result;

            //If user information does not exist, create it
            if (account == null) account = CreateUserAccount(id);
            return account;
        }

        public static List<UserInfoFields> GetAllAccounts()
        {
            //Specify table information
            var storageAccount = new CloudStorageAccount(new StorageCredentials(AzureConfig.azureAccount.accountName, AzureConfig.azureAccount.accountKey), true);
            var tableClient = storageAccount.CreateCloudTableClient();
            var userInfoTable = tableClient.GetTableReference(accountsTable);

            //Retrieve user information
            var filter_1 = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "Discord");
            var query = new TableQuery<UserInfoFields>().Where(filter_1);
            var results_list = userInfoTable.ExecuteQuery(query).ToList();

            return results_list;
        }

        private static UserInfoFields CreateUserAccount(ulong id)
        {
            var newAccount = new UserInfoFields()
            {
                PartitionKey = "Discord",
                RowKey = id.ToString(),
                Date_Started = DateTime.UtcNow,
                Account_Activated = "No",
                Profile_Theme = "",
                Decor_Owned = "",
                Decor_Setting = "",
                Shop_Sort = "release_old_new",
                Level = 1,
                Total_Exp = 0,
                Last_Message_Cooldown = DateTime.UtcNow,
                Loop_Point_Hour = DateTime.UtcNow,
                Loop_Point_Day = DateTime.UtcNow,
                Hourly_Exp_Gained = 0,
                Daily_Exp_Gained = 0,
                Hourly_Cap_Counter = 0,
                Time_Out_Start = DateTime.UtcNow,
                Time_Out_Duration = 0,
                First_Level_Msg_Sent = "No",
                First_Rank_Msg_Sent = "No",
                All_Ranks_Maxed_Msg_Sent = "No",
                Level_Resets = 0,
                Proficiency = 0,
                Diligence = 0,
                Expression = 0,
                Proficiency_Rank = 1,
                Diligence_Rank = 1,
                Expression_Rank = 1,
                Daily_Proficiency_Gained = 0,
                Daily_Diligence_Gained = "Yes",
                Daily_Expression_Gained = 0,
                Diligence_Multiplier = 10,
                P_Medals = 100,
                City = "Tokyo",
                Level_Up_Notifications = "Off",
                Rank_Up_Notifications = "Off",
                Content_Filter = "",
                VC_P1 = "P1-PSP",
                VC_P2IS = "P2IS-PSP",
                VC_P2EP = "P2EP-PSP",
                VC_P3 = "P3P",
                VC_P4 = "P4G",
                VC_P5 = "P5R",
                CustomSpriteSets = "",
                P1_PSX_TS_Wallpaper = "Type 1",
                P1_PSX_TS_Moon_HUD = "On",
                P1_PSX_TS_Position = "Switch",
                P1_PSX_TS_BG_Darken = "Off",
                P1_PSX_TS_Consistent_Names = "On",
                P1_PSX_TS_Localized_Revelations_Names = "On",
                P1_PSP_TS_Moon_HUD = "On",
                P1_PSP_TS_Position = "Switch",
                P1_PSP_TS_BG_Darken = "Off",
                P2IS_PSX_TS_Wallpaper = "Blue Tone",
                P2IS_PSX_TS_Invert = "Off",
                P2IS_PSX_TS_Position = "Default",
                P2IS_PSX_TS_Sprite_Flip = "Off",
                P2IS_PSX_TS_Localized_Revelations_Names = "On",
                P2IS_PSP_TS_Invert = "Off",
                P2IS_PSP_TS_Position = "Default",
                P2IS_PSP_TS_Sprite_Flip = "Off",
                P2EP_PSX_TS_Wallpaper = "Blue Tone",
                P2EP_PSX_TS_Invert = "Off",
                P2EP_PSX_TS_Position = "Default",
                P2EP_PSX_TS_Sprite_Flip = "Off",
                P2EP_PSX_TS_Localized_Revelations_Names = "On",
                P2EP_PSP_TS_Window_Color = "Type 1",
                P2EP_PSP_TS_Invert = "Off",
                P2EP_PSP_TS_Position = "Default",
                P2EP_PSP_TS_Sprite_Flip = "Off",
                P3F_TS_HUD = "Display All",
                P3F_TS_Nav = "Off",
                P3P_TS_Color = "Male Protagonist",
                P3P_TS_HUD = "Display All",
                P3P_TS_Position = "Center",
                P3P_TS_Dual = "Normal",
                P4_PS2_TS_HUD = "Normal",
                P4G_TS_HUD = "Normal",
                P4AU_TS_Scene_Type = "Dialogue",
                P4AU_TS_Auto_Advance = "Off",
                P4AU_TS_Position = "Right",
                P4AU_TS_Panel = "PlayStation®️ 3",
                P4AU_TS_Dual = "Normal",
                P4AU_TS_Nav_BG = 1,
                P4AU_TS_Phone_BG = "Junes Food Court",
                P4AU_TS_Highlight = "On",
                P4D_TS_Scene_Type = "Dialogue",
                P4D_TS_Auto_Advance = "Off",
                P4D_TS_Position = "Center",
                P4D_TS_Dual = "Normal",
                P4D_TS_Nav_Call_Location = 1,
                P5_PS4_TS_HUD = "Normal",
                P5_PS4_TS_Border = "Event",
                P5_PS4_TS_Panel = "Manual (with Control Panel)",
                P5R_TS_HUD = "Normal",
                P5R_TS_Border = "Event",
                P5R_TS_Panel = "Manual (with Control Panel)",
                P5R_TS_Caller_Toggle = "Off",
                P5R_TS_Caller_Location = "Dynamic",
                P5S_TS_Controller_Type = "PlayStation® 4",
                P5S_TS_Skip_Button = "On",
                P5S_TS_Auto_Advance = "Off",
                P5S_TS_Scene_Border = "On",
                P5S_TS_Date_Location_Layout = "Display All",
                P5S_TS_Location_Icon = "RV Travel",
                P5S_TS_Watermark = "Off",
                BBTAG_TS_Header = "Episode Extra",
                BBTAG_TS_Position = "Center",
                BBTAG_TS_BG_Blur = "Off",
                Display_Names_Sort = "entry_new_old",
                Setting_Sheet_Order = "Order by Outfit",
                Setting_BG_Color = "Transparent",
                Setting_BG_Upload = "Scale to Fill",
                P1_PSX_Resolution = "320 × 240",
                P1_PSX_Scale = "Nearest Neighbor",
                P1_PSP_Resolution = "480 × 272",
                P1_PSP_Scale = "Nearest Neighbor",
                P2IS_PSX_Resolution = "320 × 240",
                P2IS_PSX_Scale = "Nearest Neighbor",
                P2IS_PSP_Resolution = "480 × 272",
                P2IS_PSP_Scale = "Nearest Neighbor",
                P2EP_PSX_Resolution = "320 × 240",
                P2EP_PSX_Scale = "Nearest Neighbor",
                P2EP_PSP_Resolution = "480 × 272",
                P2EP_PSP_Scale = "Nearest Neighbor",
                P3F_Resolution = "640 × 480",
                P3F_Scale = "Nearest Neighbor",
                P3P_Resolution = "480 × 272",
                P3P_Scale = "Nearest Neighbor",
                P4_PS2_Resolution = "640 × 480",
                P4_PS2_Scale = "Nearest Neighbor",
                P4AU_Resolution = "1280 × 720",
                P4AU_Scale = "Nearest Neighbor",
                P4D_Resolution = "960 × 544",
                P4D_Scale = "Nearest Neighbor",
                Auto_Delete_Commands = "On",
                Auto_Delete_Error_Messages = "Off"
            };

            //Specify table information
            var storageAccount = new CloudStorageAccount(new StorageCredentials(AzureConfig.azureAccount.accountName, AzureConfig.azureAccount.accountKey), true);
            var tableClient = storageAccount.CreateCloudTableClient();
            var userInfoTable = tableClient.GetTableReference(accountsTable);

            //Insert entity
            userInfoTable.Execute(TableOperation.InsertOrReplace(newAccount));

            //Return new info
            return newAccount;
        }
    }
}
