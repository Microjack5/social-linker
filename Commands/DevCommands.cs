using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using SocialLinker.Core.CloudStorageTables;
using SocialLinker.Core.SceneMaker;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrayNotify;

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

                    account.Setting_BG_Upload = "Scale to Fill";

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

        public static async Task P3RE_Bustup_Test(SocialLinkerCommand command)
        {
            Bitmap bustup = new Bitmap(2, 2);
            var attachment = command.Attachments.FirstOrDefault();

            if (attachment != null)
            {
                // Here, we'll want to try and retrieve the user's input image.
                try
                {
                    // Declare variables for a web request to retrieve the image.
                    System.Net.HttpWebRequest webRequest = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(attachment.Url);
                    webRequest.AllowWriteStreamBuffering = true;
                    webRequest.Timeout = 30000;

                    // Create a stream and download the image to it.
                    System.Net.WebResponse webResponse = webRequest.GetResponse();
                    System.IO.Stream stream = webResponse.GetResponseStream();

                    // Copy the stream's contents to the background bitmap variable.
                    bustup = (Bitmap)System.Drawing.Image.FromStream(stream);

                    webResponse.Close();
                }
                // If an exception occurs here, the filetype is likely incompatible.
                // Send an error message, delete the loading message, and return.
                catch (System.ArgumentException e)
                {
                    Console.WriteLine(e);
                    throw new ArgumentException();
                }
            }
            else
            {
                await command.Channel.SendMessageAsync("No image found.");
                return;
            }

            Bitmap base_template = new Bitmap(bustup.Width, bustup.Height);
            Bitmap highlight_layer = new Bitmap(bustup.Width, bustup.Height);

            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                System.Drawing.Color current_pixel;
                int new_alpha = 0;

                // Base sprite
                for (int i = 0; i < bustup.Width; i++)
                {
                    for (int j = 0; j < bustup.Height; j++)
                    {
                        current_pixel = bustup.GetPixel(i, j);

                        if (current_pixel.A >= 50)
                        {
                            new_alpha = current_pixel.A * 2;

                            if (new_alpha > 250)
                            {
                                new_alpha = 255;
                            }

                            base_template.SetPixel(i, j, Color.FromArgb(new_alpha, current_pixel.R, current_pixel.G, current_pixel.B));
                        }
                    }
                }
            }

            //using (Graphics graphics = Graphics.FromImage(base_template))
            //{
            //    System.Drawing.Color current_pixel;

            //    // Base sprite
            //    for (int i = 0; i < bustup.Width; i++)
            //    {
            //        for (int j = 0; j < bustup.Height; j++)
            //        {
            //            current_pixel = bustup.GetPixel(i, j);

            //            if (current_pixel.A > 10)
            //            {
            //                base_template.SetPixel(i, j, Color.FromArgb(255, current_pixel.R, current_pixel.G, current_pixel.B));
            //            }
            //        }
            //    }
            //}

            //using (Graphics graphics = Graphics.FromImage(highlight_layer))
            //{
            //    System.Drawing.Color current_pixel;
            //    System.Drawing.Color highlight = Color.FromArgb(255, 255, 255);

            //    // Highlight
            //    for (int i = 0; i < bustup.Width; i++)
            //    {
            //        for (int j = 0; j < bustup.Height; j++)
            //        {
            //            current_pixel = bustup.GetPixel(i, j);

            //            if (current_pixel.A > 150)
            //            {
            //                highlight_layer.SetPixel(i, j, Color.FromArgb(current_pixel.A, highlight.R, highlight.G, highlight.B));
            //            }
            //        }
            //    }
            //}

            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                graphics.DrawImage(highlight_layer, 0, 0, highlight_layer.Width, highlight_layer.Height);
            }

            MemoryStream memoryStream = new MemoryStream();
            base_template.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
            memoryStream.Seek(0, SeekOrigin.Begin);

            try
            {
                // Send the image.
                await command.Channel.SendFileAsync(memoryStream, $"scene_{command.User.Id}_{DateTime.UtcNow}.png", "Result:");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

                // Send an error message to the user if the image upload fails.
                _ = ErrorHandling.Image_Upload_Failed(command);

                // Clean up resources used by the stream, delete the loading message, and return.
                memoryStream.Dispose();
                return;
            }
        }

        public static async Task Organize_P3RE(SocialLinkerCommand command)
        {
            //if (command.User.Id != 222504679878164481)
            //{
            //    return;
            //}

            //string char_id = "B2";
            //List<string> pose_a_frames = new List<string>() { "F00", "F01", "F02", "F03", "F05", "F10", "F64" };
            //List<string> pose_b_frames = new List<string>() { "F04", "F06", "F08", "F11", "F61" };
            //List<string> pose_c_frames = new List<string>();
            //List<string> pose_d_frames = new List<string>();
            //List<string> pose_p_frames = new List<string>();
            //List<string> outfit_list = new List<string>() { "C002", "C051", "C052", "C201", "C001", "C006", "C005", "C159", "C154", "C102", "C156", "C106", "C155", "C157" };

            //int eye_pose_a_x_coord = 668;
            //int eye_pose_a_y_coord = 1072;
            //int eye_pose_b_x_coord = 761;
            //int eye_pose_b_y_coord = 1049;
            //int eye_pose_c_x_coord = 0;
            //int eye_pose_c_y_coord = 0;
            //int eye_pose_d_x_coord = 0;
            //int eye_pose_d_y_coord = 0;

            //int mouth_pose_a_x_coord = 668;
            //int mouth_pose_a_y_coord = 1310;
            //int mouth_pose_b_x_coord = 761;
            //int mouth_pose_b_y_coord = 1287;
            //int mouth_pose_c_x_coord = 0;
            //int mouth_pose_c_y_coord = 0;
            //int mouth_pose_d_x_coord = 0;
            //int mouth_pose_d_y_coord = 0;

            //string base_path = $@"C:\Users\Alice\Desktop\Social Linker\SocialLinker\Assets\SceneMaker\Templates";
            //string source_framepath = $@"{base_path}\P3R\Bustup\{char_id}";
            //string export_framepath = $@"{base_path}\P3R (Export)\Bustup\{char_id}";
            //string sprite_sheet_framepath = $@"{base_path}\P3R (Sprite Sheet)\Bustup\{char_id}";

            // START -----------------------------------

            string char_id = "B2";
            List<string> pose_a_frames = new List<string>() { "F00", "F01", "F03", "F05", "F10", "F64" };
            List<string> pose_b_frames = new List<string>() { "F04", "F06", "F08", "F11", "F61" };
            List<string> pose_c_frames = new List<string>();
            List<string> pose_d_frames = new List<string>();
            List<string> pose_p_frames = new List<string>();
            List<string> outfit_list = new List<string>();

            int eye_pose_a_x_coord = 668;
            int eye_pose_a_y_coord = 1072;
            int eye_pose_b_x_coord = 761;
            int eye_pose_b_y_coord = 1049;
            int eye_pose_c_x_coord = 0;
            int eye_pose_c_y_coord = 0;
            int eye_pose_d_x_coord = 0;
            int eye_pose_d_y_coord = 0;

            int mouth_pose_a_x_coord = 668;
            int mouth_pose_a_y_coord = 1310;
            int mouth_pose_b_x_coord = 761;
            int mouth_pose_b_y_coord = 1287;
            int mouth_pose_c_x_coord = 0;
            int mouth_pose_c_y_coord = 0;
            int mouth_pose_d_x_coord = 0;
            int mouth_pose_d_y_coord = 0;

            string base_path = $@"F:\Projects\Modding\Persona 3 Reload\_Waiting Room\Automation";
            string source_framepath = $@"{base_path}\1. Source\{char_id}";
            string export_framepath = $@"{base_path}\2. Export\{char_id}";
            string sprite_sheet_framepath = $@"{base_path}\3. Sprite Sheet\{char_id}";

            // END -------------------------------------

            string eyes_folder = "Eyes";
            string mouth_folder = "Mouth";

            await Organize_P3RE_Workload(
            command,
            char_id,
            pose_a_frames,
            pose_b_frames,
            pose_c_frames,
            pose_d_frames,
            pose_p_frames,
            outfit_list,
            eye_pose_a_x_coord,
            eye_pose_a_y_coord,
            eye_pose_b_x_coord,
            eye_pose_b_y_coord,
            eye_pose_c_x_coord,
            eye_pose_c_y_coord,
            eye_pose_d_x_coord,
            eye_pose_d_y_coord,
            mouth_pose_a_x_coord,
            mouth_pose_a_y_coord,
            mouth_pose_b_x_coord,
            mouth_pose_b_y_coord,
            mouth_pose_c_x_coord,
            mouth_pose_c_y_coord,
            mouth_pose_d_x_coord,
            mouth_pose_d_y_coord,
            source_framepath,
            export_framepath,
            sprite_sheet_framepath,
            eyes_folder,
            mouth_folder);
        }

        public static async Task Organize_P3RE_Workload(
            SocialLinkerCommand command,
            string char_id,
            List<string> pose_a_frames,
            List<string> pose_b_frames,
            List<string> pose_c_frames,
            List<string> pose_d_frames,
            List<string> pose_p_frames,
            List<string> outfit_list,
            int eye_pose_a_x_coord,
            int eye_pose_a_y_coord,
            int eye_pose_b_x_coord,
            int eye_pose_b_y_coord,
            int eye_pose_c_x_coord,
            int eye_pose_c_y_coord,
            int eye_pose_d_x_coord,
            int eye_pose_d_y_coord,
            int mouth_pose_a_x_coord,
            int mouth_pose_a_y_coord,
            int mouth_pose_b_x_coord,
            int mouth_pose_b_y_coord,
            int mouth_pose_c_x_coord,
            int mouth_pose_c_y_coord,
            int mouth_pose_d_x_coord,
            int mouth_pose_d_y_coord,
            string source_framepath,
            string export_framepath,
            string sprite_sheet_framepath,
            string eyes_folder,
            string mouth_folder)
        {
            List<string> expression_list = new List<string>();

            string[] all_base_sprites = Directory.GetFiles(source_framepath, $"*.png");

            int base_sprite_filecount = all_base_sprites.Length;

            char[] delimiters = { '_', '.' };
            
            await command.Channel.SendMessageAsync($"" +
                $"Character ID: {char_id}\n" +
                $"Number of base sprites: {base_sprite_filecount}\n" +
                $"Beginning to sort eye frames. Check the console for progress.");

            for (int i = 0; i < base_sprite_filecount; i++)
            {
                Console.WriteLine($"Organizing eye frames on {i + 1}/{base_sprite_filecount}...");
                List<string> destructured_filename = Path.GetFileName(all_base_sprites[i]).Split(delimiters).ToList();

                string current_character_code = $"{destructured_filename[0]}_{destructured_filename[1]}_{destructured_filename[2]}";
                string current_base_pose_code = $"{destructured_filename[3]}";
                string current_base_outfit_code = $"{destructured_filename[4]}";

                if (!outfit_list.Contains($"{current_base_outfit_code}"))
                {
                    await command.Channel.SendMessageAsync($"Current outfit code not found in list: {current_base_outfit_code}. Adding...");
                    outfit_list.Add(current_base_outfit_code);
                }

                Bitmap current_base_sprite = (Bitmap)System.Drawing.Image.FromFile($@"{all_base_sprites[i]}");

                Bitmap base_sprite_sheet_copy = P3RE_Bitmap_to_Opaque(current_base_sprite);

                string new_pose_code = "";

                switch (current_base_pose_code)
                {
                    case "PoseA":
                        new_pose_code = "a";
                        break;

                    case "PoseB":
                        new_pose_code = "b";
                        break;

                    case "PoseC":
                        new_pose_code = "c";
                        break;

                    case "PoseD":
                        new_pose_code = "d";
                        break;

                    case "PoseP":
                        new_pose_code = "p";
                        break;
                }

                string exported_base_sprite_filename = $"{char_id.ToLower()}_0_{outfit_list.IndexOf(current_base_outfit_code) + 1}{new_pose_code}";

                if (!File.Exists($@"{export_framepath}\{exported_base_sprite_filename}.png"))
                {
                    current_base_sprite.Save($@"{export_framepath}\{exported_base_sprite_filename}.png", System.Drawing.Imaging.ImageFormat.Png);
                }

                if (Directory.Exists($@"{source_framepath}\{eyes_folder}"))
                {
                    string[] all_eye_frames = Directory.GetFiles($@"{source_framepath}\{eyes_folder}", $"*.png");

                    int eye_frame_filecount = all_eye_frames.Length;

                    for (int j = 0; j < eye_frame_filecount; j++)
                    {
                        List<string> destructured_eye_filename = Path.GetFileName(all_eye_frames[j]).Split(delimiters).ToList();

                        string current_character_eye_code = $"{destructured_eye_filename[0]}_{destructured_eye_filename[1]}_{destructured_eye_filename[2]}";
                        string current_expression_code = $"{destructured_eye_filename[3]}";
                        string current_eye_outfit_code = $"{destructured_eye_filename[4]}";
                        string current_frame_code = $"{destructured_eye_filename[5]}";

                        bool special_frame_check = false;

                        foreach (var frame in all_eye_frames)
                        {
                            if (frame.Contains(current_base_outfit_code))
                            {
                                special_frame_check = true;
                            }
                        }

                        if (!expression_list.Contains($"{current_expression_code}"))
                        {
                            expression_list.Add(current_expression_code);
                        }

                        using (Graphics graphics = Graphics.FromImage(base_sprite_sheet_copy))
                        {
                            Bitmap current_eye_sprite = (Bitmap)System.Drawing.Image.FromFile($@"{all_eye_frames[j]}");
                            Bitmap eye_sprite_sheet_copy = P3RE_Bitmap_to_Opaque(current_eye_sprite);
                            string new_eye_filename = "";

                            if ((pose_a_frames.Contains(current_expression_code) && (current_base_pose_code == "PoseA")) ||
                                (pose_b_frames.Contains(current_expression_code) && (current_base_pose_code == "PoseB")) ||
                                (pose_c_frames.Contains(current_expression_code) && (current_base_pose_code == "PoseC")) ||
                                (pose_d_frames.Contains(current_expression_code) && (current_base_pose_code == "PoseD")) ||
                                (pose_p_frames.Contains(current_expression_code) && (current_base_pose_code == "PoseP")))
                            {
                                if (current_eye_outfit_code == "C900" && !special_frame_check)
                                {
                                    new_eye_filename = $"{char_id.ToLower()}_{expression_list.IndexOf(current_expression_code) + 1}_0{new_pose_code}_{current_frame_code.ToLower()}";

                                    string sprite_sheet_eye_folder = $@"{sprite_sheet_framepath}\{eyes_folder}";

                                    if (!File.Exists($@"{sprite_sheet_eye_folder}\{new_eye_filename}.png"))
                                    {
                                        eye_sprite_sheet_copy.Save($@"{sprite_sheet_eye_folder}\{new_eye_filename}.png", System.Drawing.Imaging.ImageFormat.Png);

                                        current_eye_sprite.Save($@"{export_framepath}\{eyes_folder}\{new_eye_filename}.png", System.Drawing.Imaging.ImageFormat.Png);
                                    }

                                    if (current_frame_code == "E1")
                                    {
                                        string sprite_sheet_filename = $"{char_id.ToLower()}_{expression_list.IndexOf(current_expression_code) + 1}_{outfit_list.IndexOf(current_base_outfit_code) + 1}{new_pose_code}";

                                        if (File.Exists($@"{sprite_sheet_framepath}\{sprite_sheet_filename}.png"))
                                        {
                                            Console.WriteLine("Existing filename detected! It isn't supposed to be here...");
                                        }

                                        switch (current_base_pose_code)
                                        {
                                            case "PoseA":
                                                graphics.DrawImage(eye_sprite_sheet_copy, eye_pose_a_x_coord, eye_pose_a_y_coord, eye_sprite_sheet_copy.Width, eye_sprite_sheet_copy.Height);
                                                break;

                                            case "PoseB":
                                                graphics.DrawImage(eye_sprite_sheet_copy, eye_pose_b_x_coord, eye_pose_b_y_coord, eye_sprite_sheet_copy.Width, eye_sprite_sheet_copy.Height);
                                                break;

                                            case "PoseC":
                                                graphics.DrawImage(eye_sprite_sheet_copy, eye_pose_c_x_coord, eye_pose_c_y_coord, eye_sprite_sheet_copy.Width, eye_sprite_sheet_copy.Height);
                                                break;

                                            case "PoseD":
                                                graphics.DrawImage(eye_sprite_sheet_copy, eye_pose_d_x_coord, eye_pose_d_y_coord, eye_sprite_sheet_copy.Width, eye_sprite_sheet_copy.Height);
                                                break;

                                            case "PoseP":
                                                // Do nothing
                                                break;
                                        }

                                        base_sprite_sheet_copy.Save($@"{sprite_sheet_framepath}\{sprite_sheet_filename}.png", System.Drawing.Imaging.ImageFormat.Png);
                                    }
                                }
                                else if (current_eye_outfit_code == current_base_outfit_code)
                                {
                                    new_eye_filename = $"{char_id.ToLower()}_{expression_list.IndexOf(current_expression_code) + 1}_{outfit_list.IndexOf(current_base_outfit_code) + 1}{new_pose_code}_{current_frame_code.ToLower()}";

                                    string sprite_sheet_eye_folder = $@"{sprite_sheet_framepath}\{eyes_folder}";

                                    if (!File.Exists($@"{sprite_sheet_eye_folder}\{new_eye_filename}.png"))
                                    {
                                        eye_sprite_sheet_copy.Save($@"{sprite_sheet_eye_folder}\{new_eye_filename}.png", System.Drawing.Imaging.ImageFormat.Png);

                                        current_eye_sprite.Save($@"{export_framepath}\{eyes_folder}\{new_eye_filename}.png", System.Drawing.Imaging.ImageFormat.Png);
                                    }

                                    if (current_frame_code == "E1")
                                    {
                                        string sprite_sheet_filename = $"{char_id.ToLower()}_{expression_list.IndexOf(current_expression_code) + 1}_{outfit_list.IndexOf(current_base_outfit_code) + 1}{new_pose_code}";

                                        if (File.Exists($@"{sprite_sheet_framepath}\{sprite_sheet_filename}.png"))
                                        {
                                            Console.WriteLine("Existing filename detected! It isn't supposed to be here...");
                                        }

                                        switch (current_base_pose_code)
                                        {
                                            case "PoseA":
                                                graphics.DrawImage(eye_sprite_sheet_copy, eye_pose_a_x_coord, eye_pose_a_y_coord, eye_sprite_sheet_copy.Width, eye_sprite_sheet_copy.Height);
                                                break;

                                            case "PoseB":
                                                graphics.DrawImage(eye_sprite_sheet_copy, eye_pose_b_x_coord, eye_pose_b_y_coord, eye_sprite_sheet_copy.Width, eye_sprite_sheet_copy.Height);
                                                break;

                                            case "PoseC":
                                                graphics.DrawImage(eye_sprite_sheet_copy, eye_pose_c_x_coord, eye_pose_c_y_coord, eye_sprite_sheet_copy.Width, eye_sprite_sheet_copy.Height);
                                                break;

                                            case "PoseD":
                                                graphics.DrawImage(eye_sprite_sheet_copy, eye_pose_d_x_coord, eye_pose_d_y_coord, eye_sprite_sheet_copy.Width, eye_sprite_sheet_copy.Height);
                                                break;

                                            case "PoseP":
                                                // Do nothing
                                                break;
                                        }

                                        base_sprite_sheet_copy.Save($@"{sprite_sheet_framepath}\{sprite_sheet_filename}.png", System.Drawing.Imaging.ImageFormat.Png);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            await command.Channel.SendMessageAsync($"" +
                $"Beginning to sort mouth frames. Check the console for progress.");

            for (int i = 0; i < base_sprite_filecount; i++)
            {
                Console.WriteLine($"Organizing mouth frames on {i + 1}/{base_sprite_filecount}...");
                List<string> destructured_filename = Path.GetFileName(all_base_sprites[i]).Split(delimiters).ToList();

                string current_character_code = $"{destructured_filename[0]}_{destructured_filename[1]}_{destructured_filename[2]}";
                string current_base_pose_code = $"{destructured_filename[3]}";
                string current_base_outfit_code = $"{destructured_filename[4]}";

                if (!outfit_list.Contains($"{current_base_outfit_code}"))
                {
                    await command.Channel.SendMessageAsync($"Current outfit code not found in list: {current_base_outfit_code}. Adding...");
                    outfit_list.Add(current_base_outfit_code);
                }

                string new_pose_code = "";

                switch (current_base_pose_code)
                {
                    case "PoseA":
                        new_pose_code = "a";
                        break;

                    case "PoseB":
                        new_pose_code = "b";
                        break;

                    case "PoseC":
                        new_pose_code = "c";
                        break;

                    case "PoseD":
                        new_pose_code = "d";
                        break;

                    case "PoseP":
                        new_pose_code = "p";
                        break;
                }

                if (Directory.Exists($@"{source_framepath}\{mouth_folder}"))
                {
                    string[] all_mouth_frames = Directory.GetFiles($@"{source_framepath}\{mouth_folder}", $"*.png");

                    int mouth_frame_filecount = all_mouth_frames.Length;

                    for (int j = 0; j < mouth_frame_filecount; j++)
                    {
                        List<string> destructured_mouth_filename = Path.GetFileName(all_mouth_frames[j]).Split(delimiters).ToList();

                        string current_character_mouth_code = $"{destructured_mouth_filename[0]}_{destructured_mouth_filename[1]}_{destructured_mouth_filename[2]}";
                        string current_expression_code = $"{destructured_mouth_filename[3]}";
                        string current_mouth_outfit_code = $"{destructured_mouth_filename[4]}";
                        string current_frame_code = $"{destructured_mouth_filename[5]}";

                        bool special_frame_check = false;

                        foreach (var frame in all_mouth_frames)
                        {
                            if (frame.Contains(current_base_outfit_code))
                            {
                                special_frame_check = true;
                            }
                        }

                        if (!expression_list.Contains($"{current_expression_code}"))
                        {
                            Console.WriteLine("Warning!! An expression code seems to have gone missing.");
                            expression_list.Add(current_expression_code);
                        }

                        string sprite_sheet_filename = $"{char_id.ToLower()}_{expression_list.IndexOf(current_expression_code) + 1}_{outfit_list.IndexOf(current_base_outfit_code) + 1}{new_pose_code}";

                        // We need to only overwrite existing files here to avoid accidentally creating new ones
                        if (File.Exists($@"{sprite_sheet_framepath}\{sprite_sheet_filename}.png"))
                        {
                            Bitmap original = (Bitmap)System.Drawing.Image.FromFile($@"{sprite_sheet_framepath}\{sprite_sheet_filename}.png");
                            Bitmap clone = new Bitmap(original);
                            original.Dispose();

                            using (Graphics graphics = Graphics.FromImage(clone))
                            {
                                Bitmap current_mouth_sprite = (Bitmap)System.Drawing.Image.FromFile($@"{all_mouth_frames[j]}");
                                Bitmap mouth_sprite_sheet_copy = P3RE_Bitmap_to_Opaque(current_mouth_sprite);
                                string new_mouth_filename = "";

                                if ((pose_a_frames.Contains(current_expression_code) && (current_base_pose_code == "PoseA")) ||
                                    (pose_b_frames.Contains(current_expression_code) && (current_base_pose_code == "PoseB")) ||
                                    (pose_c_frames.Contains(current_expression_code) && (current_base_pose_code == "PoseC")) ||
                                    (pose_d_frames.Contains(current_expression_code) && (current_base_pose_code == "PoseD")) ||
                                    (pose_p_frames.Contains(current_expression_code) && (current_base_pose_code == "PoseP")))
                                {
                                    if (current_mouth_outfit_code == "C900" && !special_frame_check)
                                    {
                                        new_mouth_filename = $"{char_id.ToLower()}_{expression_list.IndexOf(current_expression_code) + 1}_0{new_pose_code}_{current_frame_code.ToLower()}";

                                        string sprite_sheet_mouth_folder = $@"{sprite_sheet_framepath}\{mouth_folder}";

                                        if (!File.Exists($@"{sprite_sheet_mouth_folder}\{new_mouth_filename}.png"))
                                        {
                                            mouth_sprite_sheet_copy.Save($@"{sprite_sheet_mouth_folder}\{new_mouth_filename}.png", System.Drawing.Imaging.ImageFormat.Png);

                                            current_mouth_sprite.Save($@"{export_framepath}\{mouth_folder}\{new_mouth_filename}.png", System.Drawing.Imaging.ImageFormat.Png);
                                        }

                                        if (current_frame_code == "M1")
                                        {
                                            Console.WriteLine($"Overwriting {sprite_sheet_filename}.png...");

                                            switch (current_base_pose_code)
                                            {
                                                case "PoseA":
                                                    graphics.DrawImage(mouth_sprite_sheet_copy, mouth_pose_a_x_coord, mouth_pose_a_y_coord, mouth_sprite_sheet_copy.Width, mouth_sprite_sheet_copy.Height);
                                                    break;

                                                case "PoseB":
                                                    graphics.DrawImage(mouth_sprite_sheet_copy, mouth_pose_b_x_coord, mouth_pose_b_y_coord, mouth_sprite_sheet_copy.Width, mouth_sprite_sheet_copy.Height);
                                                    break;

                                                case "PoseC":
                                                    graphics.DrawImage(mouth_sprite_sheet_copy, mouth_pose_c_x_coord, mouth_pose_c_y_coord, mouth_sprite_sheet_copy.Width, mouth_sprite_sheet_copy.Height);
                                                    break;

                                                case "PoseD":
                                                    graphics.DrawImage(mouth_sprite_sheet_copy, mouth_pose_d_x_coord, mouth_pose_d_y_coord, mouth_sprite_sheet_copy.Width, mouth_sprite_sheet_copy.Height);
                                                    break;

                                                case "PoseP":
                                                    // Do nothing
                                                    break;
                                            }

                                            clone.Save($@"{sprite_sheet_framepath}\{sprite_sheet_filename}.png", System.Drawing.Imaging.ImageFormat.Png);
                                            clone.Dispose();
                                        }
                                    }
                                    else if (current_mouth_outfit_code == current_base_outfit_code)
                                    {
                                        new_mouth_filename = $"{char_id.ToLower()}_{expression_list.IndexOf(current_expression_code) + 1}_{outfit_list.IndexOf(current_base_outfit_code) + 1}{new_pose_code}_{current_frame_code.ToLower()}";

                                        string sprite_sheet_mouth_folder = $@"{sprite_sheet_framepath}\{mouth_folder}";

                                        if (!File.Exists($@"{sprite_sheet_mouth_folder}\{new_mouth_filename}.png"))
                                        {
                                            mouth_sprite_sheet_copy.Save($@"{sprite_sheet_mouth_folder}\{new_mouth_filename}.png", System.Drawing.Imaging.ImageFormat.Png);

                                            current_mouth_sprite.Save($@"{export_framepath}\{mouth_folder}\{new_mouth_filename}.png", System.Drawing.Imaging.ImageFormat.Png);
                                        }

                                        if (current_frame_code == "M1")
                                        {
                                            Console.WriteLine($"Overwriting {sprite_sheet_filename}.png...");

                                            switch (current_base_pose_code)
                                            {
                                                case "PoseA":
                                                    graphics.DrawImage(mouth_sprite_sheet_copy, mouth_pose_a_x_coord, mouth_pose_a_y_coord, mouth_sprite_sheet_copy.Width, mouth_sprite_sheet_copy.Height);
                                                    break;

                                                case "PoseB":
                                                    graphics.DrawImage(mouth_sprite_sheet_copy, mouth_pose_b_x_coord, mouth_pose_b_y_coord, mouth_sprite_sheet_copy.Width, mouth_sprite_sheet_copy.Height);
                                                    break;

                                                case "PoseC":
                                                    graphics.DrawImage(mouth_sprite_sheet_copy, mouth_pose_c_x_coord, mouth_pose_c_y_coord, mouth_sprite_sheet_copy.Width, mouth_sprite_sheet_copy.Height);
                                                    break;

                                                case "PoseD":
                                                    graphics.DrawImage(mouth_sprite_sheet_copy, mouth_pose_d_x_coord, mouth_pose_d_y_coord, mouth_sprite_sheet_copy.Width, mouth_sprite_sheet_copy.Height);
                                                    break;

                                                case "PoseP":
                                                    // Do nothing
                                                    break;
                                            }

                                            clone.Save($@"{sprite_sheet_framepath}\{sprite_sheet_filename}.png", System.Drawing.Imaging.ImageFormat.Png);
                                            clone.Dispose();
                                        }
                                    }
                                }
                                clone.Dispose();
                            }
                        }

                        
                    }
                }
            }

            Console.WriteLine($"{char_id} organized!");
            await command.Channel.SendMessageAsync("Sorting finished.");
        }

        public static Bitmap P3RE_Bitmap_to_Opaque(Bitmap input_bitmap)
        {
            Bitmap output_bitmap = new Bitmap(input_bitmap.Width, input_bitmap.Height);

            using (Graphics graphics = Graphics.FromImage(output_bitmap))
            {
                System.Drawing.Color current_pixel;
                int new_alpha = 0;

                // Base sprite
                for (int x_pixel = 0; x_pixel < input_bitmap.Width; x_pixel++)
                {
                    for (int y_pixel = 0; y_pixel < input_bitmap.Height; y_pixel++)
                    {
                        current_pixel = input_bitmap.GetPixel(x_pixel, y_pixel);

                        if (current_pixel.A >= 50)
                        {
                            new_alpha = current_pixel.A * 2;

                            if (new_alpha > 250)
                            {
                                new_alpha = 255;
                            }

                            output_bitmap.SetPixel(x_pixel, y_pixel, Color.FromArgb(new_alpha, current_pixel.R, current_pixel.G, current_pixel.B));
                        }
                    }
                }
            }

            return output_bitmap;
        }
    }
}
