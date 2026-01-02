using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.WebSocket;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage;
using SocialLinker.Config;
using SocialLinker.Core.CloudStorageTables;
using SocialLinker.Core.Menus.Settings.Main.SceneMaker.DisplayNames;
using System.Collections.Generic;

namespace SocialLinker.Core.Menus.Settings.Reactions.SceneMaker.DisplayNames
{
    internal class Display_Names_Custom_Input_Reactions
    {
        public static Task Nav_Display_Names_Custom_Input_Main(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "display-names-custom-input-modal-open":
                    _ = Display_Names_Custom_Input_Menu.Display_Names_Custom_Input_Modal(component);
                    break;

                case "return":
                    _ = Display_Names_Sprite_Select_Menu.Display_Names_Sprite_Select_Main(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static async Task Nav_Display_Names_Custom_Input_Modal(SocketModal modal, MenuIdStructure menuSession)
        {
            var account = menuSession.Account;

            var display_name = modal.Data.Components
            .FirstOrDefault(x => x.CustomId == "name")?.Value;

            await Nav_Display_Names_Custom_Input_Main_Received(display_name, menuSession);
            return;
        }

        public static Task Nav_Display_Names_Custom_Input_Error(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "return":
                    _ = Display_Names_Sprite_Select_Menu.Display_Names_Sprite_Select_Error_1(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        // Methods that activate on the MessageReceived event.
        public static Task Nav_Display_Names_Custom_Input_Main_Received(string display_name, MenuIdStructure menuSession)
        {
            var naming_session = Global.DisplayNameTempList.SingleOrDefault(x => x.User_ID == $"{menuSession.User.Id}");

            var account = menuSession.Account;
            string input_string = display_name;

            if (input_string.Length > 32)
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                naming_session.Display_Name = input_string;

                // Go to a new menu.
                _ = Display_Names_Custom_Input_Menu.Display_Names_Custom_Input_Error_1(menuSession);
                return Task.CompletedTask;
            }

            try
            {
                var existing_name_list = DisplayNameLogging.GetCustomNameList(Convert.ToUInt64(naming_session.User_ID));
                existing_name_list = existing_name_list.OrderBy(s => s.Entry_ID).ToList();
                int new_id = 1;

                if (existing_name_list.Count > 0)
                {
                    char[] delimiterChars = { '_' };
                    List<string> latest_name_entry = existing_name_list[existing_name_list.Count - 1].Entry_ID.Split(delimiterChars).ToList();

                    new_id = Int32.Parse(latest_name_entry[1]) + 1;
                }

                // Save to temp for results screen
                naming_session.Display_Name = input_string;

                DisplayNameTableData table_submission = new DisplayNameTableData()
                {
                    PartitionKey = naming_session.User_ID,
                    RowKey = $"{naming_session.User_ID}_{new_id}",
                    Display_Name = input_string,
                    Game = naming_session.Game,
                    Character_ID = naming_session.Sprite_Set.ID,
                    Sprites_Affected = naming_session.Sprites_Affected,
                    Spriteless_Included = naming_session.Spriteless_Included,
                };

                var storageAccount = new CloudStorageAccount(new StorageCredentials(AzureConfig.azureAccount.accountName, AzureConfig.azureAccount.accountKey), true);
                var tableClient = storageAccount.CreateCloudTableClient();
                var displayNamesTable = tableClient.GetTableReference("CustomDisplayNames");

                displayNamesTable.Execute(TableOperation.InsertOrReplace(table_submission));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            // Stop the timeout timer associated with the menu.
            menuSession.MenuTimer.Stop();

            // Go to a new menu.
            _ = Display_Names_Confirm_Menu.Display_Name_Confirm_Main(menuSession.User, menuSession.MenuMessage);
            return Task.CompletedTask;
        }
    }
}
