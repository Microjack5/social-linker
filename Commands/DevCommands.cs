using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Fergun.Interactive;
using SocialLinker.Core.CloudStorageTables;
using SocialLinker.Core.Menus;

namespace SocialLinker.Commands
{
    public class DevCommands : ModuleBase<SocketCommandContext>
    {
        public static async Task UpdatePreReleaseAccounts(SocialLinkerCommand command)
        {
            if (command.User.Id != 222504679878164481)
            {
                return;
            }

            try
            {
                var accounts = UserInfoClasses.GetAllAccounts();
                int counter = 1;

                await command.Channel.SendMessageAsync("Starting account updates. Check console to watch progress.");

                foreach (var account in accounts)
                {
                    Console.WriteLine($"Updating account {counter} out of {accounts.Count}\n" +
                        $"ID: {account.RowKey}\n");

                    account.Level_Up_Notifications = "Off";
                    account.Rank_Up_Notifications = "Off";
                    //account.Content_Filter = "";
                    account.VC_P1 = "P1-PSP";
                    account.VC_P2IS = "P2IS-PSP";
                    account.VC_P2EP = "P2EP-PSP";
                    account.VC_P3 = "P3P";
                    account.VC_P4 = "P4G";
                    account.VC_P5 = "P5R";
                    account.CustomSpriteSets = "";
                    account.P1_PSX_TS_Wallpaper = "Type 1";
                    account.P1_PSX_TS_Moon_HUD = "On";
                    account.P1_PSX_TS_Position = "Switch";
                    account.P1_PSX_TS_BG_Darken = "Off";
                    account.P1_PSX_TS_Consistent_Names = "On";
                    account.P1_PSP_TS_Moon_HUD = "On";
                    account.P1_PSP_TS_Position = "Switch";
                    account.P1_PSP_TS_BG_Darken = "Off";
                    account.P2IS_PSX_TS_Wallpaper = "Blue Tone";
                    account.P2IS_PSX_TS_Invert = "Off";
                    account.P2IS_PSX_TS_Position = "Default";
                    account.P2IS_PSX_TS_Sprite_Flip = "Off";
                    account.P2IS_PSP_TS_Invert = "Off";
                    account.P2IS_PSP_TS_Position = "Default";
                    account.P2IS_PSP_TS_Sprite_Flip = "Off";
                    account.P2EP_PSX_TS_Wallpaper = "Blue Tone";
                    account.P2EP_PSX_TS_Invert = "Off";
                    account.P2EP_PSX_TS_Position = "Default";
                    account.P2EP_PSX_TS_Sprite_Flip = "Off";
                    account.P2EP_PSP_TS_Window_Color = "Type 1";
                    account.P2EP_PSP_TS_Invert = "Off";
                    account.P2EP_PSP_TS_Position = "Default";
                    account.P2EP_PSP_TS_Sprite_Flip = "Off";
                    account.P3F_TS_HUD = "Display All";
                    account.P3F_TS_Nav = "Off";
                    account.P3P_TS_Color = "Male Protagonist";
                    account.P3P_TS_HUD = "Display All";
                    account.P3P_TS_Position = "Center";
                    account.P3P_TS_Dual = "Normal";
                    account.P4_PS2_TS_HUD = "Normal";
                    account.P4G_TS_HUD = "Normal";
                    account.P4AU_TS_Scene_Type = "Dialogue";
                    account.P4AU_TS_Auto_Advance = "Off";
                    account.P4AU_TS_Position = "Right";
                    account.P4AU_TS_Panel = "PlayStation®️ 3";
                    account.P4AU_TS_Dual = "Normal";
                    account.P4AU_TS_Nav_BG = 1;
                    account.P4AU_TS_Phone_BG = "Junes Food Court";
                    account.P4AU_TS_Highlight = "On";
                    account.P4D_TS_Scene_Type = "Dialogue";
                    account.P4D_TS_Auto_Advance = "Off";
                    account.P4D_TS_Position = "Center";
                    account.P4D_TS_Dual = "Normal";
                    account.P4D_TS_Nav_Call_Location = 1;
                    account.P5_PS4_TS_HUD = "Normal";
                    account.P5_PS4_TS_Border = "Event";
                    account.P5_PS4_TS_Panel = "Manual";
                    account.P5R_TS_HUD = "Normal";
                    account.P5R_TS_Border = "Event";
                    account.P5R_TS_Panel = "Manual";
                    account.P5R_TS_Caller_Toggle = "Off";
                    account.P5R_TS_Caller_Location = "Normal";
                    account.P5S_TS_Controller_Type = "PlayStation® 4";
                    account.P5S_TS_Skip_Button = "On";
                    account.P5S_TS_Auto_Advance = "Off";
                    account.P5S_TS_Scene_Border = "On";
                    account.P5S_TS_Date_Location_Layout = "Display All";
                    account.P5S_TS_Location_Icon = "RV Travel";
                    account.P5S_TS_Watermark = "Off";
                    account.BBTAG_TS_Header = "Episode Extra";
                    account.BBTAG_TS_Position = "Center";
                    account.BBTAG_TS_BG_Blur = "Off";
                    account.Display_Names_Sort = "entry_new_old";
                    account.Setting_Sheet_Order = "Order by Outfit";
                    account.Setting_BG_Color = "Transparent";
                    account.Setting_BG_Upload = "Maintain Aspect Ratio";
                    account.P1_PSX_Resolution = "320 × 240";
                    account.P1_PSX_Scale = "Nearest Neighbor";
                    account.P1_PSP_Resolution = "480 × 272";
                    account.P1_PSP_Scale = "Nearest Neighbor";
                    account.P2IS_PSX_Resolution = "320 × 240";
                    account.P2IS_PSX_Scale = "Nearest Neighbor";
                    account.P2IS_PSP_Resolution = "480 × 272";
                    account.P2IS_PSP_Scale = "Nearest Neighbor";
                    account.P2EP_PSX_Resolution = "320 × 240";
                    account.P2EP_PSX_Scale = "Nearest Neighbor";
                    account.P2EP_PSP_Resolution = "480 × 272";
                    account.P2EP_PSP_Scale = "Nearest Neighbor";
                    account.P3F_Resolution = "640 × 480";
                    account.P3F_Scale = "Nearest Neighbor";
                    account.P3P_Resolution = "480 × 272";
                    account.P3P_Scale = "Nearest Neighbor";
                    account.P4_PS2_Resolution = "640 × 480";
                    account.P4_PS2_Scale = "Nearest Neighbor";
                    account.P4AU_Resolution = "1280 × 720";
                    account.P4AU_Scale = "Nearest Neighbor";
                    account.P4D_Resolution = "960 × 544";
                    account.P4D_Scale = "Nearest Neighbor";
                    account.Auto_Delete_Commands = "On";
                    account.Auto_Delete_Error_Messages = "Off";

                    UserInfoClasses.UpdateAccount(account);
                    counter++;
                }

                await command.Channel.SendMessageAsync("Account updates complete.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                await command.Channel.SendMessageAsync("Update failed. Check console for details.");
            }

            await Task.CompletedTask;
        }
    }
}
