using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using SocialLinker.Core.CloudStorageTables;

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

                    account.P5R_TS_Caller_Location = "Dynamic";

                    //if (account.P5_PS4_TS_Panel == "Manual (with Control Panel)")
                    //{
                    //    account.Setting_BG_Upload = "Scale to Fit";
                    //}
                    //else if (account.Setting_BG_Upload == "Stretch to Fit")
                    //{
                    //    account.Setting_BG_Upload = "Stretch to Fill";
                    //}

                    UserInfoClasses.UpdateAccount(account);
                    counter++;
                }

                //foreach (var account in accounts)
                //{
                //    Console.WriteLine($"Updating account {counter} out of {accounts.Count}\n" +
                //        $"ID: {account.RowKey}\n");

                //    account.Level_Up_Notifications = "Off";
                //    account.Rank_Up_Notifications = "Off";
                //    //account.Content_Filter = "";
                //    account.VC_P1 = "P1-PSP";
                //    account.VC_P2IS = "P2IS-PSP";
                //    account.VC_P2EP = "P2EP-PSP";
                //    account.VC_P3 = "P3P";
                //    account.VC_P4 = "P4G";
                //    account.VC_P5 = "P5R";
                //    account.CustomSpriteSets = "";
                //    account.P1_PSX_TS_Wallpaper = "Type 1";
                //    account.P1_PSX_TS_Moon_HUD = "On";
                //    account.P1_PSX_TS_Position = "Switch";
                //    account.P1_PSX_TS_BG_Darken = "Off";
                //    account.P1_PSX_TS_Consistent_Names = "On";
                //    account.P1_PSP_TS_Moon_HUD = "On";
                //    account.P1_PSP_TS_Position = "Switch";
                //    account.P1_PSP_TS_BG_Darken = "Off";
                //    account.P2IS_PSX_TS_Wallpaper = "Blue Tone";
                //    account.P2IS_PSX_TS_Invert = "Off";
                //    account.P2IS_PSX_TS_Position = "Default";
                //    account.P2IS_PSX_TS_Sprite_Flip = "Off";
                //    account.P2IS_PSP_TS_Invert = "Off";
                //    account.P2IS_PSP_TS_Position = "Default";
                //    account.P2IS_PSP_TS_Sprite_Flip = "Off";
                //    account.P2EP_PSX_TS_Wallpaper = "Blue Tone";
                //    account.P2EP_PSX_TS_Invert = "Off";
                //    account.P2EP_PSX_TS_Position = "Default";
                //    account.P2EP_PSX_TS_Sprite_Flip = "Off";
                //    account.P2EP_PSP_TS_Window_Color = "Type 1";
                //    account.P2EP_PSP_TS_Invert = "Off";
                //    account.P2EP_PSP_TS_Position = "Default";
                //    account.P2EP_PSP_TS_Sprite_Flip = "Off";
                //    account.P3F_TS_HUD = "Display All";
                //    account.P3F_TS_Nav = "Off";
                //    account.P3P_TS_Color = "Male Protagonist";
                //    account.P3P_TS_HUD = "Display All";
                //    account.P3P_TS_Position = "Center";
                //    account.P3P_TS_Dual = "Normal";
                //    account.P4_PS2_TS_HUD = "Normal";
                //    account.P4G_TS_HUD = "Normal";
                //    account.P4AU_TS_Scene_Type = "Dialogue";
                //    account.P4AU_TS_Auto_Advance = "Off";
                //    account.P4AU_TS_Position = "Right";
                //    account.P4AU_TS_Panel = "PlayStation®️ 3";
                //    account.P4AU_TS_Dual = "Normal";
                //    account.P4AU_TS_Nav_BG = 1;
                //    account.P4AU_TS_Phone_BG = "Junes Food Court";
                //    account.P4AU_TS_Highlight = "On";
                //    account.P4D_TS_Scene_Type = "Dialogue";
                //    account.P4D_TS_Auto_Advance = "Off";
                //    account.P4D_TS_Position = "Center";
                //    account.P4D_TS_Dual = "Normal";
                //    account.P4D_TS_Nav_Call_Location = 1;
                //    account.P5_PS4_TS_HUD = "Normal";
                //    account.P5_PS4_TS_Border = "Event";
                //    account.P5_PS4_TS_Panel = "Manual";
                //    account.P5R_TS_HUD = "Normal";
                //    account.P5R_TS_Border = "Event";
                //    account.P5R_TS_Panel = "Manual";
                //    account.P5R_TS_Caller_Toggle = "Off";
                //    account.P5R_TS_Caller_Location = "Normal";
                //    account.P5S_TS_Controller_Type = "PlayStation® 4";
                //    account.P5S_TS_Skip_Button = "On";
                //    account.P5S_TS_Auto_Advance = "Off";
                //    account.P5S_TS_Scene_Border = "On";
                //    account.P5S_TS_Date_Location_Layout = "Display All";
                //    account.P5S_TS_Location_Icon = "RV Travel";
                //    account.P5S_TS_Watermark = "Off";
                //    account.BBTAG_TS_Header = "Episode Extra";
                //    account.BBTAG_TS_Position = "Center";
                //    account.BBTAG_TS_BG_Blur = "Off";
                //    account.Display_Names_Sort = "entry_new_old";
                //    account.Setting_Sheet_Order = "Order by Outfit";
                //    account.Setting_BG_Color = "Transparent";
                //    account.Setting_BG_Upload = "Maintain Aspect Ratio";
                //    account.P1_PSX_Resolution = "320 × 240";
                //    account.P1_PSX_Scale = "Nearest Neighbor";
                //    account.P1_PSP_Resolution = "480 × 272";
                //    account.P1_PSP_Scale = "Nearest Neighbor";
                //    account.P2IS_PSX_Resolution = "320 × 240";
                //    account.P2IS_PSX_Scale = "Nearest Neighbor";
                //    account.P2IS_PSP_Resolution = "480 × 272";
                //    account.P2IS_PSP_Scale = "Nearest Neighbor";
                //    account.P2EP_PSX_Resolution = "320 × 240";
                //    account.P2EP_PSX_Scale = "Nearest Neighbor";
                //    account.P2EP_PSP_Resolution = "480 × 272";
                //    account.P2EP_PSP_Scale = "Nearest Neighbor";
                //    account.P3F_Resolution = "640 × 480";
                //    account.P3F_Scale = "Nearest Neighbor";
                //    account.P3P_Resolution = "480 × 272";
                //    account.P3P_Scale = "Nearest Neighbor";
                //    account.P4_PS2_Resolution = "640 × 480";
                //    account.P4_PS2_Scale = "Nearest Neighbor";
                //    account.P4AU_Resolution = "1280 × 720";
                //    account.P4AU_Scale = "Nearest Neighbor";
                //    account.P4D_Resolution = "960 × 544";
                //    account.P4D_Scale = "Nearest Neighbor";
                //    account.Auto_Delete_Commands = "On";
                //    account.Auto_Delete_Error_Messages = "Off";

                //    UserInfoClasses.UpdateAccount(account);
                //    counter++;
                //}

                await command.Channel.SendMessageAsync("Account updates complete.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                await command.Channel.SendMessageAsync("Update failed. Check console for details.");
            }

            await Task.CompletedTask;
        }

        public static async Task ExpCalculator(SocialLinkerCommand command)
        {
            if (command.User.Id != 222504679878164481)
            {
                return;
            }

            char[] delimiters = { ' ' };
            List<string> temp = command.Message.Content.Split(delimiters).ToList();
            int n = Int32.Parse(temp[1]);

            //Total Exp for Level n = 1/12 (n^4 + 4n^3 + 53n^2 - 58n)
            int current_total_exp = (((int)Math.Pow(n, 4)) + (4 * ((int)Math.Pow(n, 3))) + (53 * ((int)Math.Pow(n, 2))) - (58 * n)) / 12;

            //Next Exp for Level n = 1/6 (2n^3 + 9n^2 + 61n)
            int next_exp = ((2 * ((int)Math.Pow(n, 3))) + (9 * ((int)Math.Pow(n, 2))) + (61 * n)) / 6;

            if (n >= Global.Max_Level)
            {
                next_exp = 0;
            }

            await command.Message.Channel.SendMessageAsync($"" +
                $"Total Exp at Level {n}: {current_total_exp}\n" +
                $"Next Exp for Level {n + 1}: {next_exp}");
        }

        public static async Task LevelCalculator(SocialLinkerCommand command)
        {
            if (command.User.Id != 222504679878164481)
            {
                return;
            }

            char[] delimiters = { ' ' };
            List<string> temp = command.Message.Content.Split(delimiters).ToList();
            int input_exp = Int32.Parse(temp[1]);

            //Create variables
            int answer = 0;
            int next_exp = 0;
            int level_to_exp = 0;

            for (int i = 1; i <= Global.Max_Level; i++)
            {
                //Total Exp for Level i = 1/12 (n^4 + 4n^3 + 53n^2 - 58n)
                level_to_exp = (((int)Math.Pow(i, 4)) + (4 * ((int)Math.Pow(i, 3))) + (53 * ((int)Math.Pow(i, 2))) - (58 * i)) / 12;

                if (input_exp < level_to_exp)
                {
                    //If the input EXP is less than the equation's answer, it belongs to the previous level
                    answer = i - 1;
                    break;
                }
                else if (input_exp == level_to_exp)
                {
                    //If the input EXP is equal to the equation's answer, they are at the same level
                    answer = i;
                    break;
                }
            }

            //Next, calculate how much EXP is needed to level up
            int nextLevelBase = (((int)Math.Pow((answer + 1), 4)) + (4 * ((int)Math.Pow((answer + 1), 3))) + (53 * ((int)Math.Pow((answer + 1), 2))) - (58 * (answer + 1))) / 12;
            next_exp = nextLevelBase - input_exp;

            await command.Message.Channel.SendMessageAsync($"" +
                $"{input_exp} EXP is in the range of Level {answer}\n" +
                $"Remaining EXP until the next level: {next_exp}");
        }

        public static void CorrectMouthFrames(SocialLinkerCommand command)
        {
            if (command.User.Id != 222504679878164481)
            {
                return;
            }

            string framepath = $@"C:\Users\Alice\Desktop\New folder (4)";

            string[] allFiles = Directory.GetFiles(framepath, $"*.png");

            int filecount = allFiles.Length;

            for ( int i = 0; i < filecount; i++ )
            {
                Console.WriteLine($"Fixing {allFiles[i]}...");

                Bitmap current_frame = (Bitmap)System.Drawing.Image.FromFile($@"{allFiles[i]}");

                Bitmap new_bitmap = new Bitmap(2, 2);

                //if (allFiles[i].Contains("b3")) // Mona
                //{
                //    new_bitmap = new Bitmap(current_frame.Width - 2, current_frame.Height);
                //}
                //if (allFiles[i].Contains("b11")) // Lavenza
                //{
                //    new_bitmap = new Bitmap(current_frame.Width, current_frame.Height - 2);
                //}
                if (allFiles[i].Contains("b39")) // Maruki
                {
                    new_bitmap = new Bitmap(current_frame.Width, current_frame.Height - 2);
                }
                //if (allFiles[i].Contains("b45")) // Rumi
                //{
                //    new_bitmap = new Bitmap(current_frame.Width - 2, current_frame.Height);
                //}
                //if (allFiles[i].Contains("b47")) // Inui
                //{
                //    new_bitmap = new Bitmap(current_frame.Width, current_frame.Height - 3);
                //}
                //if (allFiles[i].Contains("b49")) // Chouno
                //{
                //    new_bitmap = new Bitmap(current_frame.Width, current_frame.Height - 3);
                //}

                using (Graphics graphics = Graphics.FromImage(new_bitmap))
                {
                    graphics.DrawImage(current_frame, 0, 0, current_frame.Width, current_frame.Height);
                }

                new_bitmap.Save($@"C:\Users\Alice\Desktop\New folder (4)\Fixed\{Path.GetFileName(allFiles[i])}", System.Drawing.Imaging.ImageFormat.Png);

                Console.WriteLine($"{allFiles[i]} saved!");
            }
        }
    }
}
