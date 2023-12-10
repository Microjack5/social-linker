using System;
using System.Drawing;
using System.Threading.Tasks;
using Discord.Commands;
using Fergun.Interactive;
using SocialLinker.Core.SceneMaker.GlyphParsing;
using System.IO;
using Discord.WebSocket;
using SocialLinker.Config;
using SocialLinker.Core.LocalStorageTables;
using Discord;
using Discord.Rest;
using SocialLinker.Core.CloudStorageTables;
using System.Net;
using Newtonsoft.Json;
using System.Collections.Generic;
using SocialLinker.Core.SceneMaker.Data.Bustup;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using SocialLinker.Core.Menus;

namespace SocialLinker.Core.SceneMaker.TemplateRenders.QuickScenes
{
    public class RenderP5S : ModuleBase<SocketCommandContext>
    {
        int template_width = 1920;
        int template_height = 1080;

        public async Task Render_Quick_Scene_P5S(SocialLinkerCommand sl_command)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = sl_command.User;
            SocketTextChannel channel = (SocketTextChannel)sl_command.Channel;

            OfficialSetData set_data = sl_command.MakerCommand.Character_Data.Set_Data;
            MakerCommandData maker_command_data = sl_command.MakerCommand;

            // Send a loading message to the channel while the sprite sheet is being made.
            RestUserMessage loader = await channel.SendMessageAsync("", false, P5S_Loading_Message().Build());

            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            sl_command.MakerCommand.Character_Data.Bustup_Data = BustupDataMethods.Get_Bustup_Data(account, set_data, sl_command.MakerCommand.Character_Data);
            BustupData bustup_data = sl_command.MakerCommand.Character_Data.Bustup_Data;

            // Background rendering
            Bitmap base_template = new Bitmap(template_width, template_height);
            Bitmap colored_background_bitmap = OfficialSetMethods.Render_Colored_Background(account, template_width, template_height);
            Bitmap background = new Bitmap(2, 2);

            try
            {
                background = OfficialSetMethods.Render_Background(sl_command, template_width, template_height);
            }
            catch (System.ArgumentException e)
            {
                Console.WriteLine(e);
                await loader.DeleteAsync();
                _ = ErrorHandling.Incompatible_File_Type(sl_command);
                return;
            }

            // Next, time for the conversation portrait! Create and initialize a new bitmap variable for it.
            Bitmap bustup = new Bitmap(2, 2);

            // Check if the base sprite number is something other than zero.
            // If it is zero, we have nothing to render. Otherwise, retrieve the bustup.
            if (maker_command_data.Character_Data.Base_Sprite != 0)
            {
                bustup = OfficialSetMethods.Bustup_Selection(sl_command, account, sl_command.MakerCommand.Character_Data);
            }

            // If the bustup returns as null, however, something went wrong with rendering the animation frames.
            // An error message has already been sent in the frame rendering method, so delete the loading message and return.
            if (bustup == null)
            {
                await loader.DeleteAsync();
                return;
            }

            // Time to put it all together!
            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                // Draw the layer with the user's colored default background if it exists.
                graphics.DrawImage(colored_background_bitmap, 0, 0, template_width, template_height);

                // Draw the user's background to the base template.
                graphics.DrawImage(background, 0, 0, template_width, template_height);

                // If the user has scene borders enabled, render it to the template.
                if (account.P5S_TS_Scene_Border != "Off")
                {
                    Bitmap border = Render_Scene_Border();

                    graphics.DrawImage(border, 0, 0, template_width, template_height);
                    graphics.DrawImage(Render_Border_Squares(border), 0, 0, template_width, template_height);
                }

                // If the user has the control panel enabled, render it to the template.
                if (account.P5S_TS_Controller_Type != "None")
                {
                    graphics.DrawImage(Render_Control_Panel(account), 0, 0, template_width, template_height);
                }

                // Draw the character bust-up to the template if the base sprite number is not '0'.
                if (maker_command_data.Character_Data.Base_Sprite != 0)
                {
                    // Make a drop shadow of the bustup first and render it to the template before the main image.
                    Bitmap drop_shadow = Create_Bustup_Drop_Shadow(bustup);
                    graphics.DrawImage(drop_shadow, bustup_data.P5S_Coord_X - 20, bustup_data.P5S_Coord_Y + 20, bustup_data.P5S_Scale_Width, bustup_data.P5S_Scale_Height);

                    // Render the main bustup next.
                    graphics.DrawImage(bustup, bustup_data.P5S_Coord_X, bustup_data.P5S_Coord_Y, bustup_data.P5S_Scale_Width, bustup_data.P5S_Scale_Height);
                }

                // If the user has the HUD enabled, render it to the template as well.
                if (account.P5S_TS_Date_Location_Layout != "None")
                {
                    switch (account.P5S_TS_Date_Location_Layout)
                    {
                        case "Display All":
                            graphics.DrawImage(Render_Location_Icon(sl_command, account), 0, 0, template_width, template_height);
                            graphics.DrawImage(Render_Calendar_HUD(sl_command, account), 0, 0, template_width, template_height);
                            break;

                        case "Date Only":
                            graphics.DrawImage(Render_Calendar_HUD(sl_command, account), -100, 0, template_width, template_height);
                            break;
                    }
                }

                // Here's an important step: Rendering all the text and vectors to the template.
                // First, let's established a needed variable: The lines of dialogue needed to be rendered, parsed into an array of string lists.
                maker_command_data.Dialogue = OfficialSetMethods.Validate_Input(sl_command, "P5S", "Dialogue", maker_command_data.Dialogue);
                List<string>[] dialogue_lines = OfficialSetMethods.Line_Parser(sl_command, "P5S", maker_command_data.Dialogue, 3, 750);

                // Using that string array list, let's generate all the vectors and text in one go!
                Bitmap merged_vectors_bitmap = new Bitmap(template_width, template_height);

                if (maker_command_data.Character_Data.Base_Sprite != 0)
                {
                    merged_vectors_bitmap = Combine_Vector_Bitmaps(account, dialogue_lines, false, false);
                }
                else
                {
                    merged_vectors_bitmap = Combine_Vector_Bitmaps(account, dialogue_lines, false, true);
                }

                string display_name = OfficialSetMethods.GetDisplayName(account, sl_command.MakerCommand.Character_Data);
                display_name = OfficialSetMethods.Validate_Input(sl_command, "P5S", "Name", display_name);

                Bitmap merged_text_bitmap = Combine_Text_Bitmaps(display_name, dialogue_lines);

                // Draw the vectors and text to the template.
                graphics.DrawImage(merged_vectors_bitmap, 0, 0, template_width, template_height);
                graphics.DrawImage(merged_text_bitmap, 0, 0, template_width, template_height);

                if (account.P5S_TS_Watermark == "On")
                {
                    Bitmap watermark = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5S//Main//copyright.png");
                    graphics.DrawImage(watermark, 0, 0, template_width, template_height);
                }
            }

            // Save the entire base template to a data stream.
            MemoryStream memoryStream = new MemoryStream();
            base_template.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
            memoryStream.Seek(0, SeekOrigin.Begin);

            try
            {
                await sl_command.Channel.SendFileAsync(memoryStream, $"scene_{sl_command.User.Id}_{DateTime.UtcNow}.png");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

                // Send an error message to the user if the image upload fails.
                _ = ErrorHandling.Image_Upload_Failed(sl_command);

                // Clean up resources used by the stream, delete the loading message, and return.
                memoryStream.Dispose();
                await loader.DeleteAsync();
                return;
            }

            // Clean up resources used by the stream and delete the loading message.
            memoryStream.Dispose();
            await loader.DeleteAsync();

            // If the user has auto-delete for their commands set to on, delete their command as well.
            if (account.Auto_Delete_Commands == "On")
            {
                await sl_command.Message.DeleteAsync();
            }
        }

        public async Task Render_System_Message(SocialLinkerCommand sl_command, MakerCommandData command_data)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = sl_command.User;
            SocketTextChannel channel = (SocketTextChannel)sl_command.Channel;

            // Send a loading message to the channel while the sprite sheet is being made.
            RestUserMessage loader = await channel.SendMessageAsync("", false, P5S_Loading_Message().Build());

            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            // Background rendering
            Bitmap base_template = new Bitmap(template_width, template_height);
            Bitmap colored_background_bitmap = OfficialSetMethods.Render_Colored_Background(account, template_width, template_height);
            Bitmap background = new Bitmap(2, 2);

            try
            {
                background = OfficialSetMethods.Render_Background(sl_command, template_width, template_height);
            }
            catch (System.ArgumentException e)
            {
                Console.WriteLine(e);
                await loader.DeleteAsync();
                _ = ErrorHandling.Incompatible_File_Type(sl_command);
                return;
            }

            // Time to put it all together!
            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                // Draw the layer with the user's colored default background if it exists.
                graphics.DrawImage(colored_background_bitmap, 0, 0, template_width, template_height);

                // Draw the user's background to the base template.
                graphics.DrawImage(background, 0, 0, template_width, template_height);

                // If the user has scene borders enabled, render it to the template.
                if (account.P5S_TS_Scene_Border != "Off")
                {
                    Bitmap border = Render_Scene_Border();

                    graphics.DrawImage(border, 0, 0, template_width, template_height);
                    graphics.DrawImage(Render_Border_Squares(border), 0, 0, template_width, template_height);
                }

                // If the user has the control panel enabled, render it to the template.
                if (account.P5S_TS_Controller_Type != "None")
                {
                    graphics.DrawImage(Render_Control_Panel(account), 0, 0, template_width, template_height);
                }

                // If the user has the HUD enabled, render it to the template as well.
                if (account.P5S_TS_Date_Location_Layout != "None")
                {
                    switch (account.P5S_TS_Date_Location_Layout)
                    {
                        case "Display All":
                            graphics.DrawImage(Render_Location_Icon(sl_command, account), 0, 0, template_width, template_height);
                            graphics.DrawImage(Render_Calendar_HUD(sl_command, account), 0, 0, template_width, template_height);
                            break;

                        case "Date Only":
                            graphics.DrawImage(Render_Calendar_HUD(sl_command, account), -100, 0, template_width, template_height);
                            break;
                    }
                }

                // Here's an important step: Rendering all the text and vectors to the template.
                // First, let's established a needed variable: The lines of dialogue needed to be rendered, parsed into an array of string lists.
                command_data.Dialogue = OfficialSetMethods.Validate_Input(sl_command, "P5S", "Dialogue", command_data.Dialogue);
                List<string>[] dialogue_lines = OfficialSetMethods.Line_Parser(sl_command, "P5S", command_data.Dialogue, 3, 750);

                // Using that string array list, let's generate all the vectors and text in one go!
                Bitmap merged_vectors_bitmap = Combine_Vector_Bitmaps(account, dialogue_lines, true, false);
                Bitmap merged_text_bitmap = Combine_Text_Bitmaps(null, dialogue_lines);

                // Draw the vectors and text to the template.
                graphics.DrawImage(merged_vectors_bitmap, 0, 0, template_width, template_height);
                graphics.DrawImage(merged_text_bitmap, 0, 0, template_width, template_height);

                if (account.P5S_TS_Watermark == "On")
                {
                    Bitmap watermark = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5S//Main//copyright.png");
                    graphics.DrawImage(watermark, 0, 0, template_width, template_height);
                }
            }

            // Save the entire base template to a data stream.
            MemoryStream memoryStream = new MemoryStream();
            base_template.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
            memoryStream.Seek(0, SeekOrigin.Begin);

            try
            {
                await sl_command.Channel.SendFileAsync(memoryStream, $"scene_{sl_command.User.Id}_{DateTime.UtcNow}.png");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

                // Send an error message to the user if the image upload fails.
                _ = ErrorHandling.Image_Upload_Failed(sl_command);

                // Clean up resources used by the stream, delete the loading message, and return.
                memoryStream.Dispose();
                await loader.DeleteAsync();
                return;
            }

            // Clean up resources used by the stream and delete the loading message.
            memoryStream.Dispose();
            await loader.DeleteAsync();

            // If the user has auto-delete for their commands set to on, delete their command as well.
            if (account.Auto_Delete_Commands == "On")
            {
                await sl_command.Message.DeleteAsync();
            }
        }

        // Vector rendering
        public Bitmap Combine_Vector_Bitmaps(UserInfoFields account, List<string>[] dialogue_lines, bool system_message_check, bool spriteless_message_check)
        {
            // Create a starting base bitmap to render all graphics on.
            Bitmap base_template = new Bitmap(template_width, template_height);

            // First, let's established some needed variables:
            // 1. The number of lines there actually are in that string list array
            // 2. The measured pixel length of the longest line in the string list array
            // 3. The default pixel length of a line, which will be a constant value.
            int number_of_lines = Get_Number_of_Rendered_Lines(dialogue_lines);
            int max_line_length = Get_Max_Line_Length(dialogue_lines);
            int default_line_length = 536;

            // Next, let's create bitmaps for each of the assets we need.
            // The manual advance tick is the only setting that's optional, so hold off on calling that method for now to save resources.
            Bitmap message_window = new Bitmap(template_width, template_height);
            Bitmap nametag_window = new Bitmap(template_width, template_height);
            Bitmap manual_advance;

            if (system_message_check)
            {
                message_window = Render_System_Message_Window(number_of_lines, max_line_length);
            }
            else if (spriteless_message_check)
            {
                nametag_window = Render_Nametag_Window();
                message_window = Render_System_Message_Window(number_of_lines, max_line_length);
            }
            else
            {
                nametag_window = Render_Nametag_Window();
                message_window = Render_Message_Window(number_of_lines, max_line_length);
            }

            // Depending on how many line of dialogue there are, we may need to move the nametag window up a bit.
            // The X coordinates should remain the same, but the Y ones will change.
            // Create an int variable to change the Y values of the vectors' coordinate points later on.
            int nametag_y_shift = 0;

            // Let's decide the value of the y_axis_shift depending on how many lines are there.
            // If there is only one line, do nothing; the default Y value is already set for this.
            if (number_of_lines <= 1)
            {
                // Do nothing
            }
            // If there are two or three lines, however, we want to move the Y values up by 20.
            else
            {
                nametag_y_shift = -20;
            }

            // Likewise, the manual advance tick bitmap may move depending on the length of the text box.
            // Set the default X coordinate to zero by creating an int variable to store the value.
            int manual_advance_x_shift = 0;

            // If the pixel length of the longest line is greater than the default pixel length of the textbox, we'll need to move the bitmap accordingly.
            if (max_line_length > default_line_length)
            {
                // Get the pixel length difference between the two; that'll be out new X coordinate.
                manual_advance_x_shift = max_line_length - default_line_length;
            }

            // Let's put all the layers together!
            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                // Set the graphics rendering to have antialiasing.
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                // Render the message and nametag windows to the template.
                graphics.DrawImage(message_window, 0, 0, template_width, template_height);
                graphics.DrawImage(nametag_window, 0, nametag_y_shift, template_width, template_height);

                // If the user has auto advance set to off, finally create the manual advance tick bitmap and render it to the template as well.
                if (account.P5S_TS_Auto_Advance == "Off")
                {
                    manual_advance = Render_Manual_Advance_Tick();
                    graphics.DrawImage(manual_advance, manual_advance_x_shift, 0, template_width, template_height);
                }
            }

            return base_template;
        }

        public Bitmap Render_Message_Window(int number_of_lines, int max_line_length)
        {
            // How the vectors are rendered is strongly determined
            int default_line_length = 536;
            int starting_dialogue_position = 672;

            int end_of_line = 0;

            if (max_line_length > default_line_length)
            {
                end_of_line = starting_dialogue_position + max_line_length;
            }
            else
            {
                end_of_line = starting_dialogue_position + default_line_length;
            }

            // We'll need to create four layers:
            // - Base layer
            // - Outer black vector layer
            // - White vector layer
            // - A layer for merging the black and white vectors
            // - Inner transparent black layer (We'll call this one a 'void layer' for short)
            Bitmap base_template = new Bitmap(template_width, template_height);
            Bitmap black_layer = new Bitmap(template_width, template_height);
            Bitmap white_layer = new Bitmap(template_width, template_height);
            Bitmap black_white_layer = new Bitmap(template_width, template_height);
            Bitmap void_layer = new Bitmap(template_width, template_height);

            // Create a brush for the color white.
            SolidBrush blackBrush = new SolidBrush(System.Drawing.Color.Black);

            // Create a brush for the color white.
            SolidBrush whiteBrush = new SolidBrush(System.Drawing.Color.White);

            // Create a new random variable.
            Random rnd = new Random();

            // Create multiple variables for the potential min and max values of the thirteen black outer points of the message window.
            int black_point_1_x_min = 479;
            int black_point_1_x_max = 479;
            int black_point_1_y_min = 909;
            int black_point_1_y_max = 909;

            int black_point_2_x_min = 549;
            int black_point_2_x_max = 570;
            int black_point_2_y_min = 1034;
            int black_point_2_y_max = 1062;

            int black_point_3_x_min = 575;
            int black_point_3_x_max = 584;
            int black_point_3_y_min = 1004;
            int black_point_3_y_max = 1013;

            int black_point_4_x_min = 624;
            int black_point_4_x_max = 625;
            int black_point_4_y_min = 1066;
            int black_point_4_y_max = 1073;

            int black_point_5_x_min = 652;
            int black_point_5_x_max = 653;
            int black_point_5_y_min = 1037;
            int black_point_5_y_max = 1041;

            int black_point_6_x_min = 671;
            int black_point_6_x_max = 677;
            int black_point_6_y_min = 1047;
            int black_point_6_y_max = 1049;

            int black_point_7_x_min = end_of_line + 51; // 1259
            int black_point_7_x_max = end_of_line + 52; // 1260
            int black_point_7_y_min = 1024;
            int black_point_7_y_max = 1026;

            int black_point_8_x_min = end_of_line + 151; // 1359
            int black_point_8_x_max = end_of_line + 152; // 1361
            int black_point_8_y_min = 891;
            int black_point_8_y_max = 892;

            int black_point_9_x_min = end_of_line + 26; // 1234
            int black_point_9_x_max = end_of_line + 35; // 1243
            int black_point_9_y_min = 779;
            int black_point_9_y_max = 784;

            int black_point_10_x_min = 612;
            int black_point_10_x_max = 617;
            int black_point_10_y_min = 838;
            int black_point_10_y_max = 843;

            int black_point_11_x_min = 602;
            int black_point_11_x_max = 608;
            int black_point_11_y_min = 890;
            int black_point_11_y_max = 897;

            int black_point_12_x_min = 561;
            int black_point_12_x_max = 571;
            int black_point_12_y_min = 849;
            int black_point_12_y_max = 872;

            int black_point_13_x_min = 541;
            int black_point_13_x_max = 546;
            int black_point_13_y_min = 913;
            int black_point_13_y_max = 918;

            switch (number_of_lines)
            {
                case 2:
                    black_point_6_y_min = black_point_6_y_min + 19;
                    black_point_6_y_max = black_point_6_y_max + 19;

                    black_point_7_y_min = black_point_7_y_min + 19;
                    black_point_7_y_max = black_point_7_y_max + 19;

                    black_point_8_y_min = black_point_8_y_min - 8;
                    black_point_8_y_max = black_point_8_y_max - 8;

                    black_point_9_y_min = black_point_9_y_min - 22;
                    black_point_9_y_max = black_point_9_y_max - 22;

                    black_point_10_y_min = black_point_10_y_min - 22;
                    black_point_10_y_max = black_point_10_y_max - 22;
                    break;

                case 3:
                    black_point_6_y_min = black_point_6_y_min + 38;
                    black_point_6_y_max = black_point_6_y_max + 38;

                    black_point_7_y_min = black_point_7_y_min + 38;
                    black_point_7_y_max = black_point_7_y_max + 38;

                    black_point_8_y_min = black_point_8_y_min - 16;
                    black_point_8_y_max = black_point_8_y_max - 16;

                    black_point_9_y_min = black_point_9_y_min - 44;
                    black_point_9_y_max = black_point_9_y_max - 44;

                    black_point_10_y_min = black_point_10_y_min - 44;
                    black_point_10_y_max = black_point_10_y_max - 44;
                    break;

                default:
                    // Do nothing
                    break;
            }

            // Randomly set the X and Y values of the outer thirteen points of the vector using the min and max values.
            int black_point_1_x = rnd.Next(black_point_1_x_min, black_point_1_x_max + 1);
            int black_point_1_y = rnd.Next(black_point_1_y_min, black_point_1_y_max + 1);

            int black_point_2_x = rnd.Next(black_point_2_x_min, black_point_2_x_max + 1);
            int black_point_2_y = rnd.Next(black_point_2_y_min, black_point_2_y_max + 1);

            int black_point_3_x = rnd.Next(black_point_3_x_min, black_point_3_x_max + 1);
            int black_point_3_y = rnd.Next(black_point_3_y_min, black_point_3_y_max + 1);

            int black_point_4_x = rnd.Next(black_point_4_x_min, black_point_4_x_max + 1);
            int black_point_4_y = rnd.Next(black_point_4_y_min, black_point_4_y_max + 1);

            int black_point_5_x = rnd.Next(black_point_5_x_min, black_point_5_x_max + 1);
            int black_point_5_y = rnd.Next(black_point_5_y_min, black_point_5_y_max + 1);

            int black_point_6_x = rnd.Next(black_point_6_x_min, black_point_6_x_max + 1);
            int black_point_6_y = rnd.Next(black_point_6_y_min, black_point_6_y_max + 1);

            int black_point_7_x = rnd.Next(black_point_7_x_min, black_point_7_x_max + 1);
            int black_point_7_y = rnd.Next(black_point_7_y_min, black_point_7_y_max + 1);

            int black_point_8_x = rnd.Next(black_point_8_x_min, black_point_8_x_max + 1);
            int black_point_8_y = rnd.Next(black_point_8_y_min, black_point_8_y_max + 1);

            int black_point_9_x = rnd.Next(black_point_9_x_min, black_point_9_x_max + 1);
            int black_point_9_y = rnd.Next(black_point_9_y_min, black_point_9_y_max + 1);

            int black_point_10_x = rnd.Next(black_point_10_x_min, black_point_10_x_max + 1);
            int black_point_10_y = rnd.Next(black_point_10_y_min, black_point_10_y_max + 1);

            int black_point_11_x = rnd.Next(black_point_11_x_min, black_point_11_x_max + 1);
            int black_point_11_y = rnd.Next(black_point_11_y_min, black_point_11_y_max + 1);

            int black_point_12_x = rnd.Next(black_point_12_x_min, black_point_12_x_max + 1);
            int black_point_12_y = rnd.Next(black_point_12_y_min, black_point_12_y_max + 1);

            int black_point_13_x = rnd.Next(black_point_13_x_min, black_point_13_x_max + 1);
            int black_point_13_y = rnd.Next(black_point_13_y_min, black_point_13_y_max + 1);

            // Randomly set the X and Y values of the thirteen points of the inner white vector based on the set black point X & Y values.
            int white_point_1_x = rnd.Next(black_point_1_x + 15, black_point_1_x + 16);
            int white_point_1_y = rnd.Next(black_point_1_y + 14, black_point_1_y + 15);

            int white_point_2_x = rnd.Next(black_point_2_x - 4, black_point_2_x + 3);
            int white_point_2_y = rnd.Next(black_point_2_y - 31, black_point_2_y - 26);

            int white_point_3_x = rnd.Next(black_point_3_x - 7, black_point_3_x - 1);
            int white_point_3_y = rnd.Next(black_point_3_y - 23, black_point_3_y - 19);

            int white_point_4_x = rnd.Next(black_point_4_x - 4, black_point_4_x + 0);
            int white_point_4_y = rnd.Next(black_point_4_y - 29, black_point_4_y - 20);

            int white_point_5_x = rnd.Next(black_point_5_x - 6, black_point_5_x - 1);
            int white_point_5_y = rnd.Next(black_point_5_y - 29, black_point_5_y - 23);

            int white_point_6_x = rnd.Next(black_point_6_x - 2, black_point_6_x + 3);
            int white_point_6_y = rnd.Next(black_point_6_y - 21, black_point_6_y - 18);

            int white_point_7_x = rnd.Next(black_point_7_x - 8, black_point_7_x - 5);
            int white_point_7_y = rnd.Next(black_point_7_y - 17, black_point_7_y - 13);

            int white_point_8_x = rnd.Next(black_point_8_x - 17, black_point_8_x - 14);
            int white_point_8_y = rnd.Next(black_point_8_y + 4, black_point_8_y + 7);

            int white_point_9_x = rnd.Next(black_point_9_x - 10, black_point_9_x - 3);
            int white_point_9_y = rnd.Next(black_point_9_y + 14, black_point_9_y + 19);

            int white_point_10_x = rnd.Next(black_point_10_x + 14, black_point_10_x + 20);
            int white_point_10_y = rnd.Next(black_point_10_y + 13, black_point_10_y + 19);

            int white_point_11_x = rnd.Next(black_point_11_x + 4, black_point_11_x + 12);
            int white_point_11_y = rnd.Next(black_point_11_y + 26, black_point_11_y + 33);

            int white_point_12_x = rnd.Next(black_point_12_x - 2, black_point_12_x + 12);
            int white_point_12_y = rnd.Next(black_point_12_y + 21, black_point_12_y + 29);

            int white_point_13_x = rnd.Next(black_point_13_x + 3, black_point_13_x + 10);
            int white_point_13_y = rnd.Next(black_point_13_y + 18, black_point_13_y + 30);

            // Randomly set the X and Y values of the thirteen points of the innermost black vector (we'll call it 'void' here) based on the set white point X & Y values.
            int void_point_1_x = rnd.Next(white_point_1_x + 19, white_point_1_x + 20);
            int void_point_1_y = rnd.Next(white_point_1_y + 20, white_point_1_y + 21);

            int void_point_2_x = rnd.Next(white_point_2_x - 3, white_point_2_x + 3);
            int void_point_2_y = rnd.Next(white_point_2_y - 36, white_point_2_y - 21);

            int void_point_3_x = rnd.Next(white_point_3_x - 4, white_point_3_x - 1);
            int void_point_3_y = rnd.Next(white_point_3_y - 26, white_point_3_y - 22);

            int void_point_4_x = rnd.Next(white_point_4_x - 2, white_point_4_x + 1);
            int void_point_4_y = rnd.Next(white_point_4_y - 30, white_point_4_y - 21);

            int void_point_5_x = rnd.Next(white_point_5_x - 2, white_point_5_x + 0);
            int void_point_5_y = rnd.Next(white_point_5_y - 25, white_point_5_y - 21);

            int void_point_6_x = rnd.Next(white_point_6_x + 0, white_point_6_x + 7);
            int void_point_6_y = rnd.Next(white_point_6_y - 16, white_point_6_y - 13);

            int void_point_7_x = rnd.Next(white_point_7_x - 15, white_point_7_x - 4);
            int void_point_7_y = rnd.Next(white_point_7_y - 10, white_point_7_y - 7);

            int void_point_8_x = rnd.Next(white_point_8_x - 19, white_point_8_x - 16);
            int void_point_8_y = rnd.Next(white_point_8_y - 1, white_point_8_y + 5);

            int void_point_9_x = rnd.Next(white_point_9_x - 6, white_point_9_x + 4);
            int void_point_9_y = rnd.Next(white_point_9_y + 15, white_point_9_y + 19);

            int void_point_10_x = rnd.Next(white_point_10_x + 14, white_point_10_x + 18);
            int void_point_10_y = rnd.Next(white_point_10_y + 10, white_point_10_y + 16);

            int void_point_11_x = rnd.Next(white_point_11_x + 6, white_point_11_x + 10);
            int void_point_11_y = rnd.Next(white_point_11_y + 23, white_point_11_y + 30);

            int void_point_12_x = rnd.Next(white_point_12_x + 4, white_point_12_x + 9);
            int void_point_12_y = rnd.Next(white_point_12_y + 18, white_point_12_y + 29);

            int void_point_13_x = rnd.Next(white_point_13_x + 3, white_point_13_x + 9);
            int void_point_13_y = rnd.Next(white_point_13_y + 16, white_point_13_y + 22);

            // Create the thirteen points of the black vector from the randomly chosen values.
            Point black_point_1 = new Point(black_point_1_x, black_point_1_y);
            Point black_point_2 = new Point(black_point_2_x, black_point_2_y);
            Point black_point_3 = new Point(black_point_3_x, black_point_3_y);
            Point black_point_4 = new Point(black_point_4_x, black_point_4_y);
            Point black_point_5 = new Point(black_point_5_x, black_point_5_y);
            Point black_point_6 = new Point(black_point_6_x, black_point_6_y);
            Point black_point_7 = new Point(black_point_7_x, black_point_7_y);
            Point black_point_8 = new Point(black_point_8_x, black_point_8_y);
            Point black_point_9 = new Point(black_point_9_x, black_point_9_y);
            Point black_point_10 = new Point(black_point_10_x, black_point_10_y);
            Point black_point_11 = new Point(black_point_11_x, black_point_11_y);
            Point black_point_12 = new Point(black_point_12_x, black_point_12_y);
            Point black_point_13 = new Point(black_point_13_x, black_point_13_y);

            // Create the thirteen points of the white vector from the randomly chosen values.
            Point white_point_1 = new Point(white_point_1_x, white_point_1_y);
            Point white_point_2 = new Point(white_point_2_x, white_point_2_y);
            Point white_point_3 = new Point(white_point_3_x, white_point_3_y);
            Point white_point_4 = new Point(white_point_4_x, white_point_4_y);
            Point white_point_5 = new Point(white_point_5_x, white_point_5_y);
            Point white_point_6 = new Point(white_point_6_x, white_point_6_y);
            Point white_point_7 = new Point(white_point_7_x, white_point_7_y);
            Point white_point_8 = new Point(white_point_8_x, white_point_8_y);
            Point white_point_9 = new Point(white_point_9_x, white_point_9_y);
            Point white_point_10 = new Point(white_point_10_x, white_point_10_y);
            Point white_point_11 = new Point(white_point_11_x, white_point_11_y);
            Point white_point_12 = new Point(white_point_12_x, white_point_12_y);
            Point white_point_13 = new Point(white_point_13_x, white_point_13_y);

            // Create the thirteen points of the void vector from the randomly chosen values.
            Point void_point_1 = new Point(void_point_1_x, void_point_1_y);
            Point void_point_2 = new Point(void_point_2_x, void_point_2_y);
            Point void_point_3 = new Point(void_point_3_x, void_point_3_y);
            Point void_point_4 = new Point(void_point_4_x, void_point_4_y);
            Point void_point_5 = new Point(void_point_5_x, void_point_5_y);
            Point void_point_6 = new Point(void_point_6_x, void_point_6_y);
            Point void_point_7 = new Point(void_point_7_x, void_point_7_y);
            Point void_point_8 = new Point(void_point_8_x, void_point_8_y);
            Point void_point_9 = new Point(void_point_9_x, void_point_9_y);
            Point void_point_10 = new Point(void_point_10_x, void_point_10_y);
            Point void_point_11 = new Point(void_point_11_x, void_point_11_y);
            Point void_point_12 = new Point(void_point_12_x, void_point_12_y);
            Point void_point_13 = new Point(void_point_13_x, void_point_13_y);

            // Add all the points for the outer black vector into a point array.
            Point[] black_poly_points = {
                    black_point_1,
                    black_point_2,
                    black_point_3,
                    black_point_4,
                    black_point_5,
                    black_point_6,
                    black_point_7,
                    black_point_8,
                    black_point_9,
                    black_point_10,
                    black_point_11,
                    black_point_12,
                    black_point_13 };

            // Add all the points for the inner white vector into a point array.
            Point[] white_poly_points = {
                    white_point_1,
                    white_point_2,
                    white_point_3,
                    white_point_4,
                    white_point_5,
                    white_point_6,
                    white_point_7,
                    white_point_8,
                    white_point_9,
                    white_point_10,
                    white_point_11,
                    white_point_12,
                    white_point_13 };

            // Add all the points for the innermost void vector into a point array.
            Point[] void_poly_points = {
                    void_point_1,
                    void_point_2,
                    void_point_3,
                    void_point_4,
                    void_point_5,
                    void_point_6,
                    void_point_7,
                    void_point_8,
                    void_point_9,
                    void_point_10,
                    void_point_11,
                    void_point_12,
                    void_point_13 };

            // First, put together the black layer.
            using (Graphics graphics = Graphics.FromImage(black_layer))
            {
                // Set the graphics rendering to have antialiasing.
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                // Use the black_poly_points array to create a polygon and fill it with black color.
                graphics.FillPolygon(blackBrush, black_poly_points);
            }

            // Next, put together the white layer.
            using (Graphics graphics = Graphics.FromImage(white_layer))
            {
                // Set the graphics rendering to have antialiasing.
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                // Use the white_poly_points array to create a polygon and fill it with white color.
                graphics.FillPolygon(whiteBrush, white_poly_points);
            }

            // Void layer next...
            using (Graphics graphics = Graphics.FromImage(void_layer))
            {
                // Set the graphics rendering to have antialiasing.
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                // Use the white_poly_points array to create a polygon and fill it with white color.
                graphics.FillPolygon(blackBrush, white_poly_points);
            }

            // Let's merge the black and white layers into one bitmap.
            using (Graphics graphics = Graphics.FromImage(black_white_layer))
            {
                // Set the graphics rendering to have antialiasing.
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                // Draw the two layers to the template.
                graphics.DrawImage(black_layer, 0, 0, template_width, template_height);
                graphics.DrawImage(white_layer, 0, 0, template_width, template_height);
            }

            // Now, using the merged layer, let's cut out a section for the transparent void layer to appear in.
            // We'll use a custom function for this to get proper antiailiasing.
            black_white_layer = Custom_Antiailiasing(black_white_layer, void_poly_points);

            // Lastly, let's put the merged and void layers together!
            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                // Set the graphics rendering to have antialiasing.
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                // Before we draw the void layer here, lower its opacity.
                void_layer = (Bitmap)Set_Image_Opacity(void_layer, (float)0.85);

                // Draw the two layers to the template.
                graphics.DrawImage(void_layer, 0, 0, template_width, template_height);
                graphics.DrawImage(black_white_layer, 0, 0, template_width, template_height);
            }

            // Return the base template.
            return base_template;
        }

        public Bitmap Render_System_Message_Window(int number_of_lines, int max_line_length)
        {
            // How the vectors are rendered is strongly determined
            int default_line_length = 536;
            int starting_dialogue_position = 672;

            int end_of_line = 0;

            if (max_line_length > default_line_length)
            {
                end_of_line = starting_dialogue_position + max_line_length;
            }
            else
            {
                end_of_line = starting_dialogue_position + default_line_length;
            }

            // We'll need to create four layers:
            // - Base layer
            // - Outer black vector layer
            // - White vector layer
            // - A layer for merging the black and white vectors
            // - Inner transparent black layer (We'll call this one a 'void layer' for short)
            Bitmap base_template = new Bitmap(template_width, template_height);
            Bitmap black_layer = new Bitmap(template_width, template_height);
            Bitmap white_layer = new Bitmap(template_width, template_height);
            Bitmap black_white_layer = new Bitmap(template_width, template_height);
            Bitmap void_layer = new Bitmap(template_width, template_height);

            // Create a brush for the color white.
            SolidBrush blackBrush = new SolidBrush(System.Drawing.Color.Black);

            // Create a brush for the color white.
            SolidBrush whiteBrush = new SolidBrush(System.Drawing.Color.White);

            // Create a new random variable.
            Random rnd = new Random();

            // Create multiple variables for the potential min and max values of the thirteen black outer points of the message window.
            int black_point_6_x_min = 666;
            int black_point_6_x_max = 671;
            int black_point_6_y_min = 1040;
            int black_point_6_y_max = 1044;

            int black_point_7_x_min = end_of_line + 51; // 1259
            int black_point_7_x_max = end_of_line + 52; // 1260
            int black_point_7_y_min = 1024;
            int black_point_7_y_max = 1026;

            int black_point_8_x_min = end_of_line + 151; // 1359
            int black_point_8_x_max = end_of_line + 152; // 1361
            int black_point_8_y_min = 891;
            int black_point_8_y_max = 892;

            int black_point_9_x_min = end_of_line + 26; // 1234
            int black_point_9_x_max = end_of_line + 35; // 1243
            int black_point_9_y_min = 779;
            int black_point_9_y_max = 784;

            int black_point_10_x_min = 612;
            int black_point_10_x_max = 617;
            int black_point_10_y_min = 838;
            int black_point_10_y_max = 843;

            int black_point_11_x_min = 568;
            int black_point_11_x_max = 577;
            int black_point_11_y_min = 967;
            int black_point_11_y_max = 972;

            switch (number_of_lines)
            {
                case 2:
                    black_point_6_y_min = black_point_6_y_min + 19;
                    black_point_6_y_max = black_point_6_y_max + 19;

                    black_point_7_y_min = black_point_7_y_min + 19;
                    black_point_7_y_max = black_point_7_y_max + 19;

                    black_point_8_y_min = black_point_8_y_min - 8;
                    black_point_8_y_max = black_point_8_y_max - 8;

                    black_point_9_y_min = black_point_9_y_min - 22;
                    black_point_9_y_max = black_point_9_y_max - 22;

                    black_point_10_y_min = black_point_10_y_min - 22;
                    black_point_10_y_max = black_point_10_y_max - 22;
                    break;

                case 3:
                    black_point_6_y_min = black_point_6_y_min + 38;
                    black_point_6_y_max = black_point_6_y_max + 38;

                    black_point_7_y_min = black_point_7_y_min + 38;
                    black_point_7_y_max = black_point_7_y_max + 38;

                    black_point_8_y_min = black_point_8_y_min - 16;
                    black_point_8_y_max = black_point_8_y_max - 16;

                    black_point_9_y_min = black_point_9_y_min - 44;
                    black_point_9_y_max = black_point_9_y_max - 44;

                    black_point_10_y_min = black_point_10_y_min - 44;
                    black_point_10_y_max = black_point_10_y_max - 44;
                    break;

                default:
                    // Do nothing
                    break;
            }

            // Randomly set the X and Y values of the outer thirteen points of the vector using the min and max values.
            int black_point_6_x = rnd.Next(black_point_6_x_min, black_point_6_x_max + 1);
            int black_point_6_y = rnd.Next(black_point_6_y_min, black_point_6_y_max + 1);

            int black_point_7_x = rnd.Next(black_point_7_x_min, black_point_7_x_max + 1);
            int black_point_7_y = rnd.Next(black_point_7_y_min, black_point_7_y_max + 1);

            int black_point_8_x = rnd.Next(black_point_8_x_min, black_point_8_x_max + 1);
            int black_point_8_y = rnd.Next(black_point_8_y_min, black_point_8_y_max + 1);

            int black_point_9_x = rnd.Next(black_point_9_x_min, black_point_9_x_max + 1);
            int black_point_9_y = rnd.Next(black_point_9_y_min, black_point_9_y_max + 1);

            int black_point_10_x = rnd.Next(black_point_10_x_min, black_point_10_x_max + 1);
            int black_point_10_y = rnd.Next(black_point_10_y_min, black_point_10_y_max + 1);

            int black_point_11_x = rnd.Next(black_point_11_x_min, black_point_11_x_max + 1);
            int black_point_11_y = rnd.Next(black_point_11_y_min, black_point_11_y_max + 1);

            // Randomly set the X and Y values of the thirteen points of the inner white vector based on the set black point X & Y values.
            int white_point_6_x = rnd.Next(black_point_6_x + 2, black_point_6_x + 8);
            int white_point_6_y = rnd.Next(black_point_6_y - 18, black_point_6_y - 15);

            int white_point_7_x = rnd.Next(black_point_7_x - 8, black_point_7_x - 5);
            int white_point_7_y = rnd.Next(black_point_7_y - 17, black_point_7_y - 13);

            int white_point_8_x = rnd.Next(black_point_8_x - 17, black_point_8_x - 14);
            int white_point_8_y = rnd.Next(black_point_8_y + 4, black_point_8_y + 7);

            int white_point_9_x = rnd.Next(black_point_9_x - 10, black_point_9_x - 3);
            int white_point_9_y = rnd.Next(black_point_9_y + 14, black_point_9_y + 19);

            int white_point_10_x = rnd.Next(black_point_10_x + 14, black_point_10_x + 20);
            int white_point_10_y = rnd.Next(black_point_10_y + 13, black_point_10_y + 19);

            // white_point_11_x & white_point_11_y need the data of points 10 and 12 first

            int white_point_12_x = rnd.Next(black_point_11_x + 19, black_point_11_x + 23);
            int white_point_12_y = rnd.Next(black_point_11_y - 6, black_point_11_y - 1);

            // Here are white_point_11_x & white_point_11_y
            int white_point_11_x_midpoint = (white_point_10_x + white_point_12_x) / 2;
            int white_point_11_x = rnd.Next(white_point_11_x_midpoint, white_point_11_x_midpoint + 8);
            int white_point_11_y = (white_point_10_y + white_point_12_y) / 2;

            // Randomly set the X and Y values of the thirteen points of the innermost black vector (we'll call it 'void' here) based on the set white point X & Y values.
            int void_point_6_x = rnd.Next(white_point_6_x + 5, white_point_6_x + 12);
            int void_point_6_y = rnd.Next(white_point_6_y - 14, white_point_6_y - 8);

            int void_point_7_x = rnd.Next(white_point_7_x - 15, white_point_7_x - 4);
            int void_point_7_y = rnd.Next(white_point_7_y - 10, white_point_7_y - 7);

            int void_point_8_x = rnd.Next(white_point_8_x - 19, white_point_8_x - 16);
            int void_point_8_y = rnd.Next(white_point_8_y - 1, white_point_8_y + 5);

            int void_point_9_x = rnd.Next(white_point_9_x - 6, white_point_9_x + 4);
            int void_point_9_y = rnd.Next(white_point_9_y + 15, white_point_9_y + 19);

            int void_point_10_x = rnd.Next(white_point_10_x + 14, white_point_10_x + 18);
            int void_point_10_y = rnd.Next(white_point_10_y + 10, white_point_10_y + 16);

            int void_point_11_x = rnd.Next(white_point_12_x + 17, white_point_12_x + 24);
            int void_point_11_y = rnd.Next(white_point_12_y - 4, white_point_12_y - 1);

            // Create the thirteen points of the black vector from the randomly chosen values.
            Point black_point_6 = new Point(black_point_6_x, black_point_6_y);
            Point black_point_7 = new Point(black_point_7_x, black_point_7_y);
            Point black_point_8 = new Point(black_point_8_x, black_point_8_y);
            Point black_point_9 = new Point(black_point_9_x, black_point_9_y);
            Point black_point_10 = new Point(black_point_10_x, black_point_10_y);
            Point black_point_11 = new Point(black_point_11_x, black_point_11_y);

            // Create the thirteen points of the white vector from the randomly chosen values.
            Point white_point_6 = new Point(white_point_6_x, white_point_6_y);
            Point white_point_7 = new Point(white_point_7_x, white_point_7_y);
            Point white_point_8 = new Point(white_point_8_x, white_point_8_y);
            Point white_point_9 = new Point(white_point_9_x, white_point_9_y);
            Point white_point_10 = new Point(white_point_10_x, white_point_10_y);
            Point white_point_11 = new Point(white_point_11_x, white_point_11_y);
            Point white_point_12 = new Point(white_point_12_x, white_point_12_y);

            // Create the thirteen points of the void vector from the randomly chosen values.
            Point void_point_6 = new Point(void_point_6_x, void_point_6_y);
            Point void_point_7 = new Point(void_point_7_x, void_point_7_y);
            Point void_point_8 = new Point(void_point_8_x, void_point_8_y);
            Point void_point_9 = new Point(void_point_9_x, void_point_9_y);
            Point void_point_10 = new Point(void_point_10_x, void_point_10_y);
            Point void_point_11 = new Point(void_point_11_x, void_point_11_y);

            // Add all the points for the outer black vector into a point array.
            Point[] black_poly_points = {
                    black_point_6,
                    black_point_7,
                    black_point_8,
                    black_point_9,
                    black_point_10,
                    black_point_11};

            // Add all the points for the inner white vector into a point array.
            Point[] white_poly_points = {
                    white_point_6,
                    white_point_7,
                    white_point_8,
                    white_point_9,
                    white_point_10,
                    white_point_11,
                    white_point_12 };

            // Add all the points for the innermost void vector into a point array.
            Point[] void_poly_points = {
                    void_point_6,
                    void_point_7,
                    void_point_8,
                    void_point_9,
                    void_point_10,
                    void_point_11 };

            // First, put together the black layer.
            using (Graphics graphics = Graphics.FromImage(black_layer))
            {
                // Set the graphics rendering to have antialiasing.
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                // Use the black_poly_points array to create a polygon and fill it with black color.
                graphics.FillPolygon(blackBrush, black_poly_points);
            }

            // Next, put together the white layer.
            using (Graphics graphics = Graphics.FromImage(white_layer))
            {
                // Set the graphics rendering to have antialiasing.
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                // Use the white_poly_points array to create a polygon and fill it with white color.
                graphics.FillPolygon(whiteBrush, white_poly_points);
            }

            // Void layer next...
            using (Graphics graphics = Graphics.FromImage(void_layer))
            {
                // Set the graphics rendering to have antialiasing.
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                // Use the white_poly_points array to create a polygon and fill it with white color.
                graphics.FillPolygon(blackBrush, white_poly_points);
            }

            // Let's merge the black and white layers into one bitmap.
            using (Graphics graphics = Graphics.FromImage(black_white_layer))
            {
                // Set the graphics rendering to have antialiasing.
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                // Draw the two layers to the template.
                graphics.DrawImage(black_layer, 0, 0, template_width, template_height);
                graphics.DrawImage(white_layer, 0, 0, template_width, template_height);
            }

            // Now, using the merged layer, let's cut out a section for the transparent void layer to appear in.
            // We'll use a custom function for this to get proper antiailiasing.
            black_white_layer = Custom_Antiailiasing(black_white_layer, void_poly_points);

            // Lastly, let's put the merged and void layers together!
            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                // Set the graphics rendering to have antialiasing.
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                // Before we draw the void layer here, lower its opacity.
                void_layer = (Bitmap)Set_Image_Opacity(void_layer, (float)0.85);

                // Draw the two layers to the template.
                graphics.DrawImage(void_layer, 0, 0, template_width, template_height);
                graphics.DrawImage(black_white_layer, 0, 0, template_width, template_height);
            }

            // Return the base template.
            return base_template;
        }

        public static Bitmap Render_Nametag_Window()
        {
            // Create variables to store the width and height of the template.
            int template_width = 1920;
            int template_height = 1080;

            // We'll need to create three layers: A base one, a layer for the white vector, and a layer for the black vector.
            Bitmap base_template = new Bitmap(template_width, template_height);
            Bitmap white_layer = new Bitmap(template_width, template_height);
            Bitmap black_layer = new Bitmap(template_width, template_height);

            // Create a brush for the color white.
            SolidBrush blackBrush = new SolidBrush(System.Drawing.Color.Black);

            // Create a brush for the color white.
            SolidBrush whiteBrush = new SolidBrush(System.Drawing.Color.White);

            // Create a new random variable. We'll use this to randomize the vector points within a certain range later.
            Random rnd = new Random();

            // Create multiple variables for the potential min and max values of the five white outer points of the nametag.
            int white_point_1_x_min = 962;
            int white_point_1_x_max = 963;
            int white_point_1_y_min = 853;
            int white_point_1_y_max = 854;

            int white_point_2_x_min = 1033;
            int white_point_2_x_max = 1044;
            int white_point_2_y_min = 743;
            int white_point_2_y_max = 757;

            int white_point_3_x_min = 598;
            int white_point_3_x_max = 614;
            int white_point_3_y_min = 806;
            int white_point_3_y_max = 812;

            int white_point_4_x_min = 574;
            int white_point_4_x_max = 579;
            int white_point_4_y_min = 841;
            int white_point_4_y_max = 847;

            int white_point_5_x_min = 596;
            int white_point_5_x_max = 602;
            int white_point_5_y_min = 873;
            int white_point_5_y_max = 879;

            // Randomly set the X and Y values of the outer five points of the nametag using the min and max values.
            int white_point_1_x = rnd.Next(white_point_1_x_min, white_point_1_x_max);
            int white_point_1_y = rnd.Next(white_point_1_y_min, white_point_1_y_max);

            int white_point_2_x = rnd.Next(white_point_2_x_min, white_point_2_x_max);
            int white_point_2_y = rnd.Next(white_point_2_y_min, white_point_2_y_max);

            int white_point_3_x = rnd.Next(white_point_3_x_min, white_point_3_x_max);
            int white_point_3_y = rnd.Next(white_point_3_y_min, white_point_3_y_max);

            int white_point_4_x = rnd.Next(white_point_4_x_min, white_point_4_x_max);
            int white_point_4_y = rnd.Next(white_point_4_y_min, white_point_4_y_max);

            int white_point_5_x = rnd.Next(white_point_5_x_min, white_point_5_x_max);
            int white_point_5_y = rnd.Next(white_point_5_y_min, white_point_5_y_max);

            // Now, let's focus on the black vector.
            // Randomly set the X and Y values of the five points of the outer black vector based on the set white point X & Y values.
            int black_point_1_x = rnd.Next(white_point_1_x - 2, white_point_1_x + 7);
            int black_point_1_y = rnd.Next(white_point_1_y + 12, white_point_1_y + 23);

            int black_point_2_x = rnd.Next(white_point_2_x + 28, white_point_2_x + 46);
            int black_point_2_y = rnd.Next(white_point_2_y - 25, white_point_2_y - 16);

            int black_point_3_x = rnd.Next(white_point_3_x - 12, white_point_3_x - 6);
            int black_point_3_y = rnd.Next(white_point_3_y - 12, white_point_3_y - 8);

            int black_point_4_x = rnd.Next(white_point_4_x - 19, white_point_4_x - 13);
            int black_point_4_y = rnd.Next(white_point_4_y - 4, white_point_4_y + 2);

            int black_point_5_x = rnd.Next(white_point_5_x - 13, white_point_5_x + 0);
            int black_point_5_y = rnd.Next(white_point_5_y + 6, white_point_5_y + 13);

            // Create the five points of the black vector from the randomly chosen values.
            Point black_point_1 = new Point(black_point_1_x, black_point_1_y);
            Point black_point_2 = new Point(black_point_2_x, black_point_2_y);
            Point black_point_3 = new Point(black_point_3_x, black_point_3_y);
            Point black_point_4 = new Point(black_point_4_x, black_point_4_y);
            Point black_point_5 = new Point(black_point_5_x, black_point_5_y);

            // Create the five points of the white vector from the randomly chosen values.
            Point white_point_1 = new Point(white_point_1_x, white_point_1_y);
            Point white_point_2 = new Point(white_point_2_x, white_point_2_y);
            Point white_point_3 = new Point(white_point_3_x, white_point_3_y);
            Point white_point_4 = new Point(white_point_4_x, white_point_4_y);
            Point white_point_5 = new Point(white_point_5_x, white_point_5_y);

            // Add all the points for the outer black vector into a point array.
            Point[] black_poly_points = {
                    black_point_1,
                    black_point_2,
                    black_point_3,
                    black_point_4,
                    black_point_5 };

            // Add all the points for the inner white vector into a point array.
            Point[] white_poly_points = {
                    white_point_1,
                    white_point_2,
                    white_point_3,
                    white_point_4,
                    white_point_5 };

            // Now, let's put all the points together and make polygons!
            // We'll need to make three graphics objects:
            // - One for the black layer
            // - One for the white layer
            // - And one for putting the two layers together

            // First, put together the black layer.
            using (Graphics graphics = Graphics.FromImage(black_layer))
            {
                // Set the graphics rendering to have antialiasing.
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                // Use the black_poly_points array to create a polygon and fill it with black color.
                graphics.FillPolygon(blackBrush, black_poly_points);
            }

            // Next, put together the white layer.
            using (Graphics graphics = Graphics.FromImage(white_layer))
            {
                // Set the graphics rendering to have antialiasing.
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                // Use the white_poly_points array to create a polygon and fill it with white color.
                graphics.FillPolygon(whiteBrush, white_poly_points);
            }

            // Lastly, let's merge the black and white layers into one bitmap.
            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                // Set the graphics rendering to have antialiasing.
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                // Before we draw the black layer here, lower its opacity.
                black_layer = (Bitmap)Set_Image_Opacity(black_layer, (float)0.7);

                // Draw the two layers to the template.
                graphics.DrawImage(black_layer, 0, 0, template_width, template_height);
                graphics.DrawImage(white_layer, 0, 0, template_width, template_height);
            }

            // While we're done with the nametag vector's structure overall, there's a gray vector moving within it we need to add.
            // Create one more bitmap for the gray layer.
            Bitmap gray_layer = new Bitmap(template_width, template_height);

            // Create a unique brush for the light gray color.
            SolidBrush gray_brush = new SolidBrush(System.Drawing.Color.FromArgb(218, 218, 218));

            // Create multiple variables for the potential min and max values of the gray vector.
            // While there'll be five points to the vector in total, two of the points will be fixed and invisible outside of the nametag vector.
            int inner_arrow_point_1_x_min = 853;
            int inner_arrow_point_1_x_max = 854;
            int inner_arrow_point_1_y_min = 747;
            int inner_arrow_point_1_y_max = 761;

            int inner_arrow_point_2_x_min = 921;
            int inner_arrow_point_2_x_max = 942;
            int inner_arrow_point_2_y_min = 808;
            int inner_arrow_point_2_y_max = 823;

            int inner_arrow_point_3_x_min = 853;
            int inner_arrow_point_3_x_max = 854;
            int inner_arrow_point_3_y_min = 864;
            int inner_arrow_point_3_y_max = 878;

            // Randomly set the X and Y values of the three points of the gray vector.
            // Again, this is just for the first three points of the vector.
            int inner_arrow_point_1_x = rnd.Next(inner_arrow_point_1_x_min, inner_arrow_point_1_x_max);
            int inner_arrow_point_1_y = rnd.Next(inner_arrow_point_1_y_min, inner_arrow_point_1_y_max);

            int inner_arrow_point_2_x = rnd.Next(inner_arrow_point_2_x_min, inner_arrow_point_2_x_max);
            int inner_arrow_point_2_y = rnd.Next(inner_arrow_point_2_y_min, inner_arrow_point_2_y_max);

            int inner_arrow_point_3_x = rnd.Next(inner_arrow_point_3_x_min, inner_arrow_point_3_x_max);
            int inner_arrow_point_3_y = rnd.Next(inner_arrow_point_3_y_min, inner_arrow_point_3_y_max);

            // Solidify the five points of the gray vector. The last two points have fixed values.
            Point inner_arrow_point_1 = new Point(inner_arrow_point_1_x, inner_arrow_point_1_y);
            Point inner_arrow_point_2 = new Point(inner_arrow_point_2_x, inner_arrow_point_2_y);
            Point inner_arrow_point_3 = new Point(inner_arrow_point_3_x, inner_arrow_point_3_y);
            Point inner_arrow_point_4 = new Point(1052, 895);
            Point inner_arrow_point_5 = new Point(1052, 696);

            // Add all the points for the gray vector into a point array.
            Point[] inner_arrow_poly_points = {
                    inner_arrow_point_1,
                    inner_arrow_point_2,
                    inner_arrow_point_3,
                    inner_arrow_point_4,
                    inner_arrow_point_5 };

            // Now, let's draw the gray vector to a bitmap.
            using (Graphics graphics = Graphics.FromImage(gray_layer))
            {
                // Set the graphics rendering to have antialiasing.
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                // Use the inner_arrow_poly_points array to create a polygon and fill it with gray color.
                graphics.FillPolygon(gray_brush, inner_arrow_poly_points);
            }

            // Lastly, merge the gray layer with the base template while only displaying the portion that overlaps with the white layer of the nametag vector.
            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                // Set the graphics rendering to have antialiasing.
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                // Alter the gray layer bitmap so only pixels that overlap with the white layer created earlier remain.
                gray_layer = Keep_Pixel_Overlap(white_layer, gray_layer, 852, 1052, 650, 895);

                // Draw the new gray layer to the base template.
                graphics.DrawImage(gray_layer, 0, 0, template_width, template_height);
            }

            // Return the new bitmap.
            return base_template;
        }

        public static Bitmap Render_Manual_Advance_Tick()
        {
            // Create variables to store the width and height of the template.
            int template_width = 1920;
            int template_height = 1080;

            // We'll need to create three layers: A base one, a layer for the white vector, and a layer for the black vector.
            Bitmap base_template = new Bitmap(template_width, template_height);
            Bitmap white_layer = new Bitmap(template_width, template_height);
            Bitmap black_layer = new Bitmap(template_width, template_height);

            // Create a brush for the color white.
            SolidBrush blackBrush = new SolidBrush(System.Drawing.Color.Black);

            // Create a brush for the color white.
            SolidBrush whiteBrush = new SolidBrush(System.Drawing.Color.White);

            // Create a new random variable. We'll use this to randomize the vector points within a certain range later.
            Random rnd = new Random();

            int point_1_increase = rnd.Next(0, 12);
            int variant = rnd.Next(1, 3);

            // Create the four points of the black vector from the randomly chosen values.
            Point black_point_1 = default;
            Point black_point_2 = default;
            Point black_point_3 = default;
            Point black_point_4 = default;

            switch (variant)
            {
                case 1:
                    black_point_1 = new Point(1246 - point_1_increase, 908);
                    black_point_2 = new Point(1269, 913);
                    black_point_3 = new Point(1462 + (point_1_increase * 4), 847 - point_1_increase);
                    black_point_4 = new Point(1403 + (point_1_increase * 3), 770 - (point_1_increase * 3));
                    break;

                case 2:
                    black_point_1 = new Point(1246 - point_1_increase, 911);
                    black_point_2 = new Point(1268, 914);
                    black_point_3 = new Point(1463 + (point_1_increase * 4), 843 - point_1_increase);
                    black_point_4 = new Point(1398 + (point_1_increase * 3), 765 - (point_1_increase * 3));
                    break;
            }

            // Create the four points of the white vector from the randomly chosen values.
            Point white_point_1 = new Point(black_point_1.X + 15, black_point_1.Y - 5);
            Point white_point_2 = new Point(black_point_2.X + 14, white_point_1.Y);
            Point white_point_3 = new Point(black_point_3.X - 10 - (4 * (point_1_increase / 11)), black_point_3.Y - 2);
            Point white_point_4 = new Point(black_point_4.X - 2, black_point_4.Y + 15 + (2 * (point_1_increase / 11)));

            // Add all the points for the outer black vector into a point array.
            Point[] black_poly_points = {
                    black_point_1,
                    black_point_2,
                    black_point_3,
                    black_point_4 };

            // Add all the points for the inner white vector into a point array.
            Point[] white_poly_points = {
                    white_point_1,
                    white_point_2,
                    white_point_3,
                    white_point_4 };

            // Now, let's put all the points together and make polygons!
            // We'll need to make three graphics objects:
            // - One for the black layer
            // - One for the white layer
            // - And one for putting the two layers together

            // First, put together the black layer.
            using (Graphics graphics = Graphics.FromImage(black_layer))
            {
                // Set the graphics rendering to have antialiasing.
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                // Use the black_poly_points array to create a polygon and fill it with black color.
                graphics.FillPolygon(blackBrush, black_poly_points);
            }

            // Next, put together the white layer.
            using (Graphics graphics = Graphics.FromImage(white_layer))
            {
                // Set the graphics rendering to have antialiasing.
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                // Use the white_poly_points array to create a polygon and fill it with white color.
                graphics.FillPolygon(whiteBrush, white_poly_points);
            }

            // Lastly, let's merge the black and white layers into one bitmap.
            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                // Set the graphics rendering to have antialiasing.
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                // Draw the two layers to the template.
                graphics.DrawImage(black_layer, 0, 0, template_width, template_height);
                graphics.DrawImage(white_layer, 0, 0, template_width, template_height);
            }

            // Return the new bitmap.
            return base_template;
        }

        // Dialogue rendering
        public Bitmap Combine_Text_Bitmaps(string display_name, List<string>[] dialogue_lines)
        {
            // Create a starting base bitmap to render all graphics on.
            Bitmap base_template = new Bitmap(template_width, template_height);

            // Get the number of lines to actually be rendered in the string array list and store it in an int variable.
            int number_of_lines = Get_Number_of_Rendered_Lines(dialogue_lines);

            // Now, let's get the text!
            // We'll need the dialogue and the character's name, but also another bitmap where the character name is rotated.
            Bitmap dialogue = Render_Dialogue(dialogue_lines);
            Bitmap rotated_nametag = new Bitmap(template_width, template_height);

            if (display_name != null)
            {
                Bitmap nametag = Render_Nametag(display_name);
                rotated_nametag = Render_Rotated_Nametag(nametag, number_of_lines);
            }

            // Let's put all the layers together!
            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                // Set the graphics rendering to have antialiasing.
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                // Render the layers to the template.
                graphics.DrawImage(dialogue, 0, 0, template_width, template_height);
                graphics.DrawImage(rotated_nametag, 0, 0, template_width, template_height);
            }

            return base_template;
        }

        public Bitmap Render_Dialogue(List<string>[] dialogue_lines)
        {
            // Create a working space bitmap.
            Bitmap base_template = new Bitmap(template_width, template_height);

            // Create an int to keep track of rendering errors. This is neccessary to inform the user of any potential issues.
            int error_counter = 0;

            //Establish an int for the width and height glyphs should be rendered at
            int multiplier = 37;

            // Create another bitmap variable. This will be reserved for characters cropped from the bitmap font and rendered to the screen.
            Bitmap current_glyph;

            // We'll need a source for the bitmap font, of course, so get the path to it.
            string font_sheet = $@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5S//Font//p5s_font_sheet.png";

            char[] temp_char_array = String_List_To_String(dialogue_lines[0]).ToCharArray();
            var first_glyph = ParsingMethods.GetGlyph(temp_char_array[0]);

            // Now, let's start iterating through the lines of dialouge so we can render them to the base template.
            for (int line_index = 0; line_index < dialogue_lines.Length; line_index++)
            {
                // Create a default value for the initial Y position for dialogue to start rendering at.
                int initial_line_height = 904;

                // To determine the true Y positioning of dialogue, we need to check how many lines there are.
                switch (Get_Number_of_Rendered_Lines(dialogue_lines))
                {
                    case 3:
                        initial_line_height = initial_line_height - 36;
                        break;

                    case 2:
                        initial_line_height = initial_line_height - 22;
                        break;

                    case 1:
                        // Do not change the initial line height
                        break;
                }

                // Using the value we just calculated for the Y coordinate, create the X & Y pairs we need to tell us where to start rendering on the template.
                int render_position_x = 667;
                render_position_x = render_position_x + first_glyph.LeftCut;
                int render_position_y = initial_line_height + (44 * line_index);

                // To render the contents of the string list array, we're going to need to break down each line.
                // On the current iterated line, convert the string list to one cohesive string.
                // Then, convert that string into a char array.
                // This way, we can grab every single character in order to render them.
                char[] char_array = String_List_To_String(dialogue_lines[line_index]).ToCharArray();

                // Create a loop to iterate through the newly created char array.
                for (int char_index = 0; char_index < char_array.Length; char_index++)
                {
                    // Retrieve glyph information from the JSON file.
                    var glyph = ParsingMethods.GetGlyph(char_array[char_index]);

                    // If the glyph info returns null, we have a rendering error.
                    // If this occurs and the error counter is at zero, increase the error counter and send a message to the user.
                    if (glyph == null && error_counter == 0)
                    {
                        error_counter++;
                    }

                    if (glyph != null)
                    {
                        int x = multiplier * glyph.Column;
                        int y = multiplier * glyph.Row;

                        using (Graphics graphics = Graphics.FromImage(base_template))
                        {
                            using (var originalImage = new Bitmap(font_sheet))
                            {
                                // Copy the section of the bitmap font needed
                                Rectangle crop = new Rectangle(x, y, multiplier, multiplier);
                                current_glyph = originalImage.Clone(crop, originalImage.PixelFormat);

                                // Draw the glyph to the base bitmap
                                graphics.DrawImage(current_glyph, (render_position_x - glyph.LeftCut), render_position_y, multiplier, multiplier);
                            }
                        }

                        //Set the next X value at the end of the current glyph's right width
                        render_position_x += (glyph.RightCut - glyph.LeftCut);

                        // Check if the current iterated index is less than the number of indicies available.
                        if (char_index < char_array.Length - 1)
                        {
                            // If so, edit the position of the X coordinate according to specific kerning pairs.
                            render_position_x += Get_Kerning_Adjustment(char_array, char_index);
                        }
                    }
                }
            }

            return base_template;
        }

        public static Bitmap Render_Nametag(string display_name)
        {
            // Establish an int for the width and height glyphs should be rendered at.
            // Glyphs are rendered in squares, so the width and height will be the same number.
            int multiplier = 37;

            // We'll want a max pixel length for the nametag this time, so keep the value here.
            int max_nametag_length = 365;

            // Now we have the data to construct the bitmap we'll be returning!
            Bitmap base_template = new Bitmap(max_nametag_length, multiplier);

            // Load the path to the bitmap font so we can crop out the glyphs.
            string font_sheet = $@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5S//Font//p5s_font_sheet.png";

            // Now, let's separate the character's display name into a char array to make it easier to iterate over.
            char[] char_array = display_name.ToCharArray();

            // Since we want the display name to be centered, we need to figure out where to start rendering it on the base template.
            // First, determine the pixel length of the display name.
            int name_length = Measure_String_Pixel_Length(null, display_name);

            // Then, we'll want to subtract the display name's pixel length from the max length of the nametag region.
            // This will give us how many pixels of blank space are left width-wise after the name is rendered.
            int free_space = max_nametag_length - name_length;

            // If we divide the free space value in half, we'll get the exact X value we'll need to start rendering at for centered text.
            // So with that, specify X and Y coordinates for where the glyphs should start rendering on the template.
            int render_position_x = free_space / 2;
            int render_position_y = 0;
            
            // Now, let's start iterating through the char array.
            for (int current_char_index = 0; current_char_index < char_array.Length; current_char_index++)
            {
                // Retrieve glyph information from the JSON file.
                var glyph = ParsingMethods.Get_P5S_Glyph(char_array[current_char_index]);

                // Check if the character is a line break.
                if (char_array[current_char_index] == '\u000a')
                {
                    // If it is, do nothing. Display names are only shown on one line.
                }
                else
                {
                    int x = multiplier * glyph.Column;
                    int y = multiplier * glyph.Row;

                    using (Graphics graphics = Graphics.FromImage(base_template))
                    {
                        using (var originalImage = new Bitmap(font_sheet))
                        {
                            // Copy the section of the bitmap font needed.
                            Rectangle crop = new Rectangle(x, y, multiplier, multiplier);
                            Bitmap current_glyph = originalImage.Clone(crop, originalImage.PixelFormat);

                            // Draw the glyph to the base bitmap.
                            graphics.DrawImage(current_glyph, (render_position_x - glyph.LeftCut), render_position_y, multiplier, multiplier);
                        }
                    }

                    // Set the next X value at the end of the current glyph's right width.
                    render_position_x += (glyph.RightCut - glyph.LeftCut);

                    // Check if the current iterated index is less than the number of indicies available.
                    if (current_char_index < char_array.Length - 1)
                    {
                        // If so, edit the position of the X coordinate according to specific kerning pairs.
                        render_position_x += Get_Kerning_Adjustment(char_array, current_char_index);
                    }
                }
            }

            base_template = Text_To_Black(base_template);

            return base_template;
        }

        public static Bitmap Render_Rotated_Nametag(Bitmap input_bitmap, int number_of_lines)
        {
            // Create variables to store the width and height of the template.
            int template_width = 1920;
            int template_height = 1080;

            // Create a starting base bitmap to render all graphics on.
            Bitmap base_template = new Bitmap(template_width, template_height);

            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                // Depending on how many line of dialogue there are, we may need to move the nametag window up a bit.
                // The X coordinates should remain the same, but the Y ones will change.
                // Create an int variable to change the Y values of the vectors' coordinate points later on.
                int y_axis_shift = 0;

                // Let's decide the value of the y_axis_shift depending on how many lines are there.
                // If there is only one line, do nothing; the default Y value is already set for this.
                if (number_of_lines == 1)
                {
                    // Do nothing
                }
                // If there are two or three lines, however, we want to move the Y values up by 20.
                else
                {
                    y_axis_shift = -20;
                }

                // We'll also want to generate the nametag of the character to display.
                Bitmap display_name = input_bitmap;
                display_name = RotateImage(display_name, -5);
                graphics.DrawImage(display_name, 514, 797 + y_axis_shift, display_name.Width, display_name.Height);
            }

            return base_template;
        }

        // Text rendering tools
        public static int Measure_String_Pixel_Length(SocialLinkerCommand sl_command, string input_word)
        {
            // Create an int to keep track of how many pixels a glyph is wide in.
            int pixel_counter = 0;

            // Take the input string and turn it into a char array.
            char[] char_array = input_word.ToCharArray();

            // Now, let's iterate through the char array.
            for (int i = 0; i < char_array.Length; i++)
            {
                // Retrieve glyph information for the current character from the JSON file.
                var glyph = ParsingMethods.Get_P5S_Glyph(char_array[i]);

                // Make sure that the glyph info doesn't return null.
                if (glyph != null)
                {
                    // Check if the current character is a line break.
                    // If it is, do nothing. Line breaks take up no pixel width space.
                    if (char_array[i] == '\u000a')
                    {
                        // Do nothing
                    }
                    else
                    {
                        // Check if the current iterated index is less than the number of indicies available.
                        if (i < char_array.Length - 1)
                        {
                            // If so, edit the position of the X coordinate according to specific kerning pairs.
                            pixel_counter += Get_Kerning_Adjustment(char_array, i);
                        }

                        // Set the pixel counter to the appropriate width of the string so far.
                        pixel_counter += glyph.RightCut - glyph.LeftCut;
                    }
                }
                else if (char_array[i] == '\ufe0f')
                {
                    // Do nothing, emoji variation selector
                }
                // If the character returns null, it's not supported by the template's font set.
                else if (sl_command != null) // This is a possibility
                {
                    sl_command.MakerCommand.Dialogue_Has_Invalid_Char = true;
                }
            }

            // Since this Persona 5 Strikers template is using a shrunken version of the Persona 5 Royal font when rendered, we need to alter the final count a bit.
            // The original width for glyphs was universally 48px wide in Royal, but shrunken down to 37px wide in Strikers. That's 77.08% of the original Royal size.
            // Let's alter the final pixel count to reflect that change.
            //pixel_counter = (int)(pixel_counter * 0.77);

            return pixel_counter;
        }

        public static string String_List_To_String(List<string> input_list)
        {
            // Create an empty string variable.
            string output_string = "";

            // Iterate through each index of the list and add it to the string variable.
            for (int i = 0; i < input_list.Count; i++)
            {
                output_string += input_list[i];
            }

            // Return the string variable.
            return output_string;
        }

        public static int Get_Kerning_Adjustment(char[] char_array, int current_index)
        {
            int render_position_x = 0;

            render_position_x += -2;

            if (char_array[current_index] == 'y')
            {
                render_position_x += -3;
            }
            else if (char_array[current_index] == ',')
            {
                render_position_x += -2;
            }
            else if (char_array[current_index] == 'w')
            {
                render_position_x += -3;
            }
            else if (char_array[current_index] == 'e' && char_array[current_index + 1] == ' ')
            {
                render_position_x += -4;
            }
            else if (char_array[current_index] == 'o' && char_array[current_index + 1] == 'm')
            {
                render_position_x += -1;
            }
            else if (char_array[current_index] == '.')
            {
                render_position_x += +2;
            }
            else if (char_array[current_index] == 'm')
            {
                render_position_x += -2;
            }
            else if (char_array[current_index] == 'r' && char_array[current_index + 1] == 'o')
            {
                render_position_x += -2;
            }
            // "Okay, next I'm scheduled aaat..."
            else if (char_array[current_index] == 'k' && char_array[current_index + 1] == 'a')
            {
                render_position_x += -2;
            }
            else if (char_array[current_index] == 'a' && char_array[current_index + 1] == 'y')
            {
                render_position_x += -2;
            }
            else if (char_array[current_index + 1] == '\'')
            {
                render_position_x += +3;
            }
            else if (char_array[current_index] == 'a' && char_array[current_index + 1] == 'a')
            {
                render_position_x += -2;
            }
            // "Well, you're lookin' sharp."
            else if (char_array[current_index] == 'W' && char_array[current_index + 1] == 'e')
            {
                render_position_x += -4;
            }
            /*else if (char_array[current_index] == 'e' && char_array[current_index + 1] == 'l')
            {
                render_position_x += +1;
            } */
            /*else if (char_array[current_index] == 'l' && char_array[current_index + 1] == 'l')
            {
                render_position_x += +3;
            } */
            // Shadow Akane
            else if (char_array[current_index] == 'o' && char_array[current_index + 1] == 'w')
            {
                render_position_x += -3;
            }
            // "Yay, we won."
            else if (char_array[current_index] == 'Y')
            {
                render_position_x += -2;
            }
            // "Huh? Oh, right, right. Get it together-"
            else if (char_array[current_index] == 'o' && char_array[current_index + 1] == 'g')
            {
                render_position_x += -3;
            }
            else if (char_array[current_index] == 'g' && char_array[current_index + 1] == 'e')
            {
                render_position_x += -3;
            }
            // "Deal! Now, Thieves-to the hideout!"
            else if (char_array[current_index] == 'D')
            {
                render_position_x += -2;
            }
            else if (char_array[current_index] == '!')
            {
                render_position_x += 4;
            }
            else if (char_array[current_index] == 'v' && char_array[current_index + 1] == 'e')
            {
                render_position_x += -3;
            }
            /*else if (char_array[current_index] == 'l' && char_array[current_index + 1] == '!')
            {
                render_position_x += 1;
            } */
            else if (char_array[current_index] == 'e' && char_array[current_index + 1] == 'v')
            {
                render_position_x += -1;
            }
            // "Bullcrap. I won't let anyone fool me again."
            else if (char_array[current_index] == 'g')
            {
                render_position_x += -2;
            }
            else if (char_array[current_index + 1] == 'g')
            {
                render_position_x += -2;
            }
            /*else if (char_array[current_index] == 'u' && char_array[current_index + 1] == 'l')
            {
                render_position_x += 2;
            } */
            else if (char_array[current_index] == 'r' && char_array[current_index + 1] == 'a')
            {
                render_position_x += -1;
            }
            else if (char_array[current_index] == 'a' && char_array[current_index + 1] == 'p')
            {
                render_position_x += -1;
            }
            /*else if (char_array[current_index] == 'l' && char_array[current_index + 1] == 'c')
            {
                render_position_x += 1;
            }*/
            // "Aw, thanks! Then I have a surprise for you..."
            else if (char_array[current_index] == 'A' && char_array[current_index + 1] == 'w')
            {
                render_position_x += -4;
            }
            else if (char_array[current_index] == 'T' && char_array[current_index + 1] == 'h')
            {
                render_position_x += -1;
            }
            else if (char_array[current_index] == 'a' && char_array[current_index + 1] == 'v')
            {
                render_position_x += -3;
            }
            else if (char_array[current_index + 1] == '.')
            {
                render_position_x += 2;
            }
            // "Now everyone, let's clean Sapporo up!"
            else if (char_array[current_index] == 'r' && char_array[current_index + 1] == 'y')
            {
                render_position_x += -2;
            }
            // "Now that I think about it, we just took a boat
            // trip, went swimming, cooked a whole feast...
            // and then dove straight into a Jail."
            else if (char_array[current_index] == 's' && char_array[current_index + 1] == 'w')
            {
                render_position_x += -1;
            }
            else if (char_array[current_index] == 'w' && char_array[current_index + 1] == 'h')
            {
                render_position_x += 1;
            }
            // "Nice to meet you, Zenkichi Hasegawa."
            else if (char_array[current_index] == 'a' && char_array[current_index + 1] == 'w')
            {
                render_position_x += -3;
            }
            // Other
            else if (char_array[current_index] == ' ')
            {
                render_position_x += -1;
            }
            else if (char_array[current_index] == 'a' && char_array[current_index + 1] == 'm')
            {
                render_position_x += -1;
            }
            else if (char_array[current_index] == 'k' && char_array[current_index + 1] == 'e')
            {
                render_position_x += -1;
            }

            if (char_array[current_index] == 'M')
            {
                render_position_x += -4;
            }
            if (char_array[current_index + 1] == 'l')
            {
                render_position_x += 2;
            }
            if (char_array[current_index] == 'l')
            {
                render_position_x += 2;
            }

            return render_position_x;
        }

        // Calendar rendering
        public static Bitmap Render_Location_Icon(SocialLinkerCommand sl_command, UserInfoFields account)
        {
            // Create variables to store the width and height of the template.
            int template_width = 1920;
            int template_height = 1080;

            // Create a new bitmap with the width and height values specified earlier.
            Bitmap base_template = new Bitmap(template_width, template_height);

            Bitmap location_icon = new Bitmap(2, 2);
            string location_icon_path = $@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5S//Main//Cities";

            switch (account.P5S_TS_Location_Icon)
            {
                case "Yongen-Jaya":
                    location_icon = (Bitmap)System.Drawing.Image.FromFile($@"{location_icon_path}//yongen-jaya.png");
                    break;

                case "Shibuya":
                    location_icon = (Bitmap)System.Drawing.Image.FromFile($@"{location_icon_path}//shibuya.png");
                    break;

                case "Sendai":
                    location_icon = (Bitmap)System.Drawing.Image.FromFile($@"{location_icon_path}//sendai.png");
                    break;

                case "Sapporo":
                    location_icon = (Bitmap)System.Drawing.Image.FromFile($@"{location_icon_path}//sapporo.png");
                    break;

                case "Okinawa":
                    location_icon = (Bitmap)System.Drawing.Image.FromFile($@"{location_icon_path}//okinawa.png");
                    break;

                case "Fukuoka":
                    location_icon = (Bitmap)System.Drawing.Image.FromFile($@"{location_icon_path}//fukuoka.png");
                    break;

                case "Kyoto":
                    location_icon = (Bitmap)System.Drawing.Image.FromFile($@"{location_icon_path}//kyoto.png");
                    break;

                case "Osaka":
                    location_icon = (Bitmap)System.Drawing.Image.FromFile($@"{location_icon_path}//osaka.png");
                    break;

                case "Yokohama":
                    location_icon = (Bitmap)System.Drawing.Image.FromFile($@"{location_icon_path}//yokohama.png");
                    break;

                case "Shiba Park":
                    location_icon = (Bitmap)System.Drawing.Image.FromFile($@"{location_icon_path}//shiba_park.png");
                    break;

                case "RV Travel":
                    location_icon = (Bitmap)System.Drawing.Image.FromFile($@"{location_icon_path}//on_the_road.png");
                    break;
            }

            return location_icon;
        }

        public static Bitmap Render_Calendar_HUD(SocialLinkerCommand sl_command, UserInfoFields account)
        {
            // Create variables to store the width and height of the template.
            int template_width = 1920;
            int template_height = 1080;

            // Create a new bitmap with the width and height values specified earlier.
            Bitmap base_template = new Bitmap(template_width, template_height);

            string calendar_assets_path = $@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5S//Main//Calendar";

            // Now, time to put the template together!
            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                // Establish all variables needed and set them to null; they will be assigned to soon.
                Bitmap date_container = new Bitmap(2, 2);

                Bitmap month = new Bitmap(2, 2);

                Bitmap day_tens = new Bitmap(2, 2);
                Bitmap day_ones = new Bitmap(2, 2);
                Bitmap day_ones_shadow = new Bitmap(2, 2);

                Bitmap day_of_week = new Bitmap(2, 2);
                Bitmap time_of_day = new Bitmap(2, 2);

                Bitmap star_decoration = new Bitmap(2, 2);

                // Get the user's current date and time according to their settings.
                DateTime user_time = Get_Date(sl_command, account);

                date_container = (Bitmap)System.Drawing.Image.FromFile($@"{calendar_assets_path}//date_container.png");

                // Use the user's date and time to determine which assets to use.
                // Months
                month = (Bitmap)System.Drawing.Image.FromFile($@"{calendar_assets_path}//Month//{user_time.Month}.png");

                // Days
                char[] day = user_time.ToString("dd").ToCharArray();

                if (day[0] != '0')
                {
                    day_tens = (Bitmap)System.Drawing.Image.FromFile($@"{calendar_assets_path}//Day//Double_Digit//Tens_Place//{day[0]}.png");
                    day_ones = (Bitmap)System.Drawing.Image.FromFile($@"{calendar_assets_path}//Day//Double_Digit//Ones_Place//{day[1]}.png");
                    day_ones_shadow = (Bitmap)System.Drawing.Image.FromFile($@"{calendar_assets_path}//Day//Double_Digit//Ones_Place//Shadow//{day[1]}.png");
                }
                else
                {
                    day_ones = (Bitmap)System.Drawing.Image.FromFile($@"{calendar_assets_path}//Day//Single_Digit//{day[1]}.png");
                }
                
                day_of_week = (Bitmap)System.Drawing.Image.FromFile($@"{calendar_assets_path}//Day_of_Week//{user_time.ToString("dddd").ToLower()}.png");
                time_of_day = (Bitmap)System.Drawing.Image.FromFile($@"{calendar_assets_path}//Time_of_Day//{Get_Time_of_Day(user_time)}.png");

                // The time of day contains a randomized element, so let's create a random variable here.
                Random rnd = new Random();

                // There are three variants a star on the time of day bitmap could have, so select a random number between 1 and 3.
                int star_selection = rnd.Next(1, 4);

                // Get the proper bitmap based on the time of day and the randomized int.
                star_decoration = (Bitmap)System.Drawing.Image.FromFile($@"{calendar_assets_path}//Time_of_Day//Stars//{Get_Time_of_Day(user_time)}_{star_selection}.png");

                // Draw all the assets to the template.
                graphics.DrawImage(date_container, 0, 0, template_width, template_height);

                graphics.DrawImage(month, 0, 0, template_width, template_height);

                graphics.DrawImage(day_ones_shadow, 0, 0, template_width, template_height);
                graphics.DrawImage(day_tens, 0, 0, template_width, template_height);
                graphics.DrawImage(day_ones, 0, 0, template_width, template_height);

                graphics.DrawImage(day_of_week, 0, 0, template_width, template_height);
                graphics.DrawImage(time_of_day, 0, 0, template_width, template_height);

                graphics.DrawImage(star_decoration, 0, 0, template_width, template_height);
            }

            return base_template;
        }

        // Border rendering
        public static Bitmap Render_Scene_Border()
        {
            // Create variables to store the width and height of the template.
            int template_width = 1920;
            int template_height = 1080;

            // Make an empty bitmap.
            Bitmap base_bitmap = new Bitmap(template_width, template_height);

            // Create needed bitmap variables for needed assets. We'll initialize them to small bitmaps for now.
            Bitmap border_top = new Bitmap(2, 2);
            Bitmap border_bottom = new Bitmap(2, 2);

            // Here, we'll assign the border graphics based on the user's settings.
            border_top = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5S//Main//Border//border_top.png");
            border_bottom = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5S//Main//Border//border_bottom.png");

            border_top = (Bitmap)Set_Image_Opacity(border_top, (float)0.8);
            border_bottom = (Bitmap)Set_Image_Opacity(border_bottom, (float)0.8);

            // Draw the assets to the template.
            using (Graphics graphics = Graphics.FromImage(base_bitmap))
            {
                graphics.DrawImage(border_top, 0, 0, template_width, template_height);
                graphics.DrawImage(border_bottom, 0, 0, template_width, template_height);
            }

            return base_bitmap;
        }

        public static Bitmap Render_Border_Squares(Bitmap input_bitmap)
        {
            // Create variables to store the width and height of the template.
            int template_width = 1920;
            int template_height = 1080;

            // Make an empty bitmap.
            Bitmap base_template = new Bitmap(template_width, template_height);

            // Assign needed assets to bitmap variables. These are the squares we'll be using.
            Bitmap white_1 = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5S//Main//Border//Squares//white_1.png");
            Bitmap white_2 = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5S//Main//Border//Squares//white_2.png");
            Bitmap black_1 = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5S//Main//Border//Squares//black_1.png");
            Bitmap black_2 = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5S//Main//Border//Squares//black_2.png");

            // In-game, black and white layers have four different possible levels of opacity.
            // Let's create two double arrays with the respective values for black and white layers.
            // We'll have a randomizer that'll decide which values to use later.
            double[] white_opacity_values = new double[] { 0.0, 0.02, 0.04, 0.06 };
            double[] black_opacity_values = new double[] { 0.0, 0.05, 0.10, 0.15 };

            // Speaking of, create a new random variable.
            // We'll rely heavily on randomization from here on, so this variable is essential.
            Random rnd = new Random();

            // We'll need to store opacity values for each layer soon, so initialize those variables now.
            float white_1_opacity = default;
            float white_2_opacity = default;
            float black_1_opacity = default;
            float black_2_opacity = default;

            // Here comes the start of randomization.
            // First question: How many layers should be selected to have full opacity?
            // Create a randomized int variable to pick a number between 1 and 4, the number of layers we have.
            int layer_selector = rnd.Next(1, 5);

            // Now, WHICH layers should be selected?
            // Let's create an int list to make it easy to keep track of which layers are randomly chosen.
            // We can remove whichever int was chosen from the list to avoid repeated choices.
            List<int> layer_list = new List<int> { 1, 2, 3, 4 };

            // Lastly, let's make a bool array with four indicies to log which layers were selected.
            // When a layer is selected, we'll make the corresponding index true.
            bool[] ledger = new bool[] { false, false, false, false };

            //Console.WriteLine($"Number of layers selected: {layer_selector}");

            // Depending on how many layers were randomly selected earlier to have full opacity, let's create a loop that iterates for that many times.
            for (int i = 0; i < layer_selector; i++)
            {
                // Pick a random value ranging from 0 to the number of total values in the layer list.
                // This chosen value will represent a random index in the list.
                int current_pick = rnd.Next(0, layer_list.Count);

                // We'll want the "real value" of our choice, meaning the int being held in the index.
                int real_value = layer_list[current_pick];

                //Console.WriteLine($"Layer picked: {real_value}");

                // Now that we have our chosen layer, mark off its corresponding index on the ledger to keep track of which one was selected.
                ledger[real_value - 1] = true;

                // Remove the value we picked in the layer list from the list itself.
                // This will shorten the length of the list and avoid repeats.
                layer_list.Remove(real_value);

                //Console.WriteLine($"Number of indicies left: {layer_list.Count}");
            }

            // Based on the notes we left in the ledger, change the opacity values for each layer to either be at the max value or randomized to a lower opacity value.
            for (int i = 0; i < ledger.Length; i++)
            {
                switch (i)
                {
                    case 0:
                        if (ledger[0] == true)
                        {
                            white_1_opacity = (float)white_opacity_values[3];
                        }
                        else
                        {
                            white_1_opacity = (float)white_opacity_values[rnd.Next(0, white_opacity_values.Length - 1)];
                        }
                        break;

                    case 1:
                        if (ledger[1] == true)
                        {
                            white_2_opacity = (float)white_opacity_values[3];
                        }
                        else
                        {
                            white_2_opacity = (float)white_opacity_values[rnd.Next(0, white_opacity_values.Length - 1)];
                        }
                        break;

                    case 2:
                        if (ledger[2] == true)
                        {
                            black_1_opacity = (float)black_opacity_values[3];
                        }
                        else
                        {
                            black_1_opacity = (float)black_opacity_values[rnd.Next(0, black_opacity_values.Length - 1)];
                        }
                        break;

                    case 3:
                        if (ledger[3] == true)
                        {
                            black_2_opacity = (float)black_opacity_values[3];
                        }
                        else
                        {
                            black_2_opacity = (float)black_opacity_values[rnd.Next(0, black_opacity_values.Length - 1)];
                        }
                        break;
                }
            }

            /*Console.WriteLine($"White opacity 1: {white_1_opacity}");
            Console.WriteLine($"White opacity 2: {white_2_opacity}");
            Console.WriteLine($"Black opacity 1: {black_1_opacity}");
            Console.WriteLine($"Black opacity 2: {black_2_opacity}"); */

            // Now! After all that randomization just for the opacity, apply the chosen opacity values to the layers themselves.
            white_1 = (Bitmap)Set_Image_Opacity(white_1, white_1_opacity);
            white_2 = (Bitmap)Set_Image_Opacity(white_2, white_2_opacity);
            black_1 = (Bitmap)Set_Image_Opacity(black_1, black_1_opacity);
            black_2 = (Bitmap)Set_Image_Opacity(black_2, black_2_opacity); 

            // Randomize the Y coordinate that each layer will start off at within a given range appropriate for each of their heights.
            // Let's start with the top border.
            int y_top_render_distance_white_1 = rnd.Next(-220, 132);
            int y_top_render_distance_white_2 = rnd.Next(-128, 132);
            int y_top_render_distance_black_1 = rnd.Next(-131, 132);
            int y_top_render_distance_black_2 = rnd.Next(-203, 132);

            // Do the same for the bottom border as well.
            int y_bottom_render_distance_white_1 = rnd.Next(680, 1081);
            int y_bottom_render_distance_white_2 = rnd.Next(772, 1081);
            int y_bottom_render_distance_black_1 = rnd.Next(769, 1081);
            int y_bottom_render_distance_black_2 = rnd.Next(697, 1081);

            // Now, let's draw the assets to the template.
            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                // Top border
                graphics.DrawImage(black_2, 0, y_top_render_distance_black_2, black_2.Width, black_2.Height);
                graphics.DrawImage(black_1, 0, y_top_render_distance_black_1, black_1.Width, black_1.Height);
                graphics.DrawImage(white_2, 0, y_top_render_distance_white_2, white_2.Width, white_2.Height);
                graphics.DrawImage(white_1, 0, y_top_render_distance_white_1, white_1.Width, white_1.Height);

                // Top border, double printing
                graphics.DrawImage(black_2, 0, y_top_render_distance_black_2 + black_2.Height, black_2.Width, black_2.Height);
                graphics.DrawImage(black_1, 0, y_top_render_distance_black_1 + black_1.Height, black_1.Width, black_1.Height);
                graphics.DrawImage(white_2, 0, y_top_render_distance_white_2 + white_2.Height, white_2.Width, white_2.Height);
                graphics.DrawImage(white_1, 0, y_top_render_distance_white_1 + white_1.Height, white_1.Width, white_1.Height);

                // Bottom border
                graphics.DrawImage(black_2, 0, y_bottom_render_distance_black_2, black_2.Width, black_2.Height);
                graphics.DrawImage(black_1, 0, y_bottom_render_distance_black_1, black_1.Width, black_1.Height);
                graphics.DrawImage(white_2, 0, y_bottom_render_distance_white_2, white_2.Width, white_2.Height);
                graphics.DrawImage(white_1, 0, y_bottom_render_distance_white_1, white_1.Width, white_1.Height);

                // Bottom border, double printing
                graphics.DrawImage(black_2, 0, y_bottom_render_distance_black_2 - black_2.Height, black_2.Width, black_2.Height);
                graphics.DrawImage(black_1, 0, y_bottom_render_distance_black_1 - black_1.Height, black_1.Width, black_1.Height);
                graphics.DrawImage(white_2, 0, y_bottom_render_distance_white_2 - white_2.Height, white_2.Width, white_2.Height);
                graphics.DrawImage(white_1, 0, y_bottom_render_distance_white_1 - white_1.Height, white_1.Width, white_1.Height);
            }

            // To ONLY keep pixel overlap between the squares and the border, return a new bitmap that only keeps overlapping pixels from the squares.
            base_template = Keep_Border_Pixel_Overlap(input_bitmap, base_template, 0, 1920, 0, 1080);

            return base_template;
        }

        public static Bitmap Render_Control_Panel(UserInfoFields account)
        {
            // Create variables to store the width and height of the template.
            int template_width = 1920;
            int template_height = 1080;

            // Make an empty bitmap.
            Bitmap base_bitmap = new Bitmap(template_width, template_height);

            // Create needed bitmap variables for needed assets. We'll initialize them to small bitmaps for now.
            Bitmap skip_button = new Bitmap(2, 2);
            Bitmap auto_button = new Bitmap(2, 2);
            
            Bitmap log_button = new Bitmap(2, 2);
            Bitmap ffwd_button = new Bitmap(2, 2);
            Bitmap hold_icon = new Bitmap(2, 2);

            string control_panel_path = $@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5S//Main//Control_Panel//Buttons";

            // Start assigning assets to variables that will be constant on either user setting.
            Bitmap skip_text = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5S//Main//Control_Panel//Text//skip.png");
            Bitmap log_text = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5S//Main//Control_Panel//Text//log.png");
            Bitmap ffwd_text = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5S//Main//Control_Panel//Text//ffwd.png");

            // Save the variables for the auto assets for later since another setting will decide their appearance.
            Bitmap auto_text_default = new Bitmap(2, 2);
            Bitmap auto_off_text_black = new Bitmap(2, 2);
            Bitmap auto_off_text_white = new Bitmap(2, 2);
            Bitmap auto_wheel = new Bitmap(2, 2);

            switch (account.P5S_TS_Controller_Type)
            {
                case "PlayStation® 4":
                    skip_button = (Bitmap)System.Drawing.Image.FromFile($@"{control_panel_path}//PS4//options_button.png");
                    auto_button = (Bitmap)System.Drawing.Image.FromFile($@"{control_panel_path}//PS4//l3_button.png");
                    log_button = (Bitmap)System.Drawing.Image.FromFile($@"{control_panel_path}//PS4//square_button.png");
                    ffwd_button = (Bitmap)System.Drawing.Image.FromFile($@"{control_panel_path}//PS4//triangle_button.png");
                    hold_icon = (Bitmap)System.Drawing.Image.FromFile($@"{control_panel_path}//PS4//hold.png");
                    break;

                case "Nintendo Switch":
                    skip_button = (Bitmap)System.Drawing.Image.FromFile($@"{control_panel_path}//Switch//plus_button.png");
                    auto_button = (Bitmap)System.Drawing.Image.FromFile($@"{control_panel_path}//Switch//l_stick.png");
                    log_button = (Bitmap)System.Drawing.Image.FromFile($@"{control_panel_path}//Switch//y_button.png");
                    ffwd_button = (Bitmap)System.Drawing.Image.FromFile($@"{control_panel_path}//Switch//x_button.png");
                    hold_icon = (Bitmap)System.Drawing.Image.FromFile($@"{control_panel_path}//Switch//hold.png");
                    break;

                case "Xbox One":
                    skip_button = (Bitmap)System.Drawing.Image.FromFile($@"{control_panel_path}//Xbox//options_button.png");
                    auto_button = (Bitmap)System.Drawing.Image.FromFile($@"{control_panel_path}//Xbox//l_stick.png");
                    log_button = (Bitmap)System.Drawing.Image.FromFile($@"{control_panel_path}//Xbox//x_button.png");
                    ffwd_button = (Bitmap)System.Drawing.Image.FromFile($@"{control_panel_path}//Xbox//y_button.png");
                    hold_icon = (Bitmap)System.Drawing.Image.FromFile($@"{control_panel_path}//Xbox//hold.png");
                    break;

                case "Keyboard":
                    skip_button = (Bitmap)System.Drawing.Image.FromFile($@"{control_panel_path}//PC//key_2.png");
                    auto_button = (Bitmap)System.Drawing.Image.FromFile($@"{control_panel_path}//PC//key_0.png");
                    log_button = (Bitmap)System.Drawing.Image.FromFile($@"{control_panel_path}//PC//key_b.png");
                    ffwd_button = (Bitmap)System.Drawing.Image.FromFile($@"{control_panel_path}//PC//key_h.png");
                    hold_icon = (Bitmap)System.Drawing.Image.FromFile($@"{control_panel_path}//PC//hold.png");
                    break;
            }

            // Skip button
            switch (account.P5S_TS_Skip_Button)
            {
                case "On":
                    // Do nothing
                    break;

                case "Off":
                    skip_button = new Bitmap(2, 2);
                    skip_text = new Bitmap(2, 2);
                    break;
            }

            // Here, we'll assign the auto graphics based on the user's settings.
            switch (account.P5S_TS_Auto_Advance)
            {
                case "On":
                    // Use a random variable for the auto wheel icon so it can change in each scene.
                    Random w = new Random();
                    int wInt = w.Next(1, 5);

                    auto_off_text_black = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5S//Main//Control_Panel//Text//auto_off_black.png");
                    auto_off_text_white = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5S//Main//Control_Panel//Text//auto_off_white.png");
                    auto_wheel = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5S//Main//Control_Panel//Text//Auto_Wheel//auto_wheel_{wInt}.png");
                    break;

                case "Off":
                    auto_text_default = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5S//Main//Control_Panel//Text//auto.png");
                    break;
            }

            // Draw the assets to the template.
            using (Graphics graphics = Graphics.FromImage(base_bitmap))
            {
                // Text
                graphics.DrawImage(skip_text, 0, 0, template_width, template_height);
                graphics.DrawImage(auto_text_default, 0, 0, template_width, template_height);

                // Auto-advance on
                graphics.DrawImage(auto_wheel, 0, 0, template_width, template_height);
                graphics.DrawImage(auto_off_text_black, 0, 0, template_width, template_height);
                graphics.DrawImage(auto_off_text_white, 0, 0, template_width, template_height);

                // Text (cont.)
                graphics.DrawImage(log_text, 0, 0, template_width, template_height);
                graphics.DrawImage(ffwd_text, 0, 0, template_width, template_height);

                // Buttons
                graphics.DrawImage(skip_button, 0, 0, template_width, template_height);
                graphics.DrawImage(auto_button, 0, 0, template_width, template_height);
                graphics.DrawImage(log_button, 0, 0, template_width, template_height);
                graphics.DrawImage(hold_icon, 0, 0, template_width, template_height);
                graphics.DrawImage(ffwd_button, 0, 0, template_width, template_height);

                
            }

            return base_bitmap;
        }

        // Coloring bitmaps
        public static Bitmap Create_Bustup_Drop_Shadow(Bitmap input_bitmap)
        {
            // Create a color variable to store the color value of a pixel on the input bitmap later.
            System.Drawing.Color actual_color;

            // Make an empty bitmap the same size as the input bitmap.
            Bitmap new_bitmap = new Bitmap(input_bitmap.Width, input_bitmap.Height);

            // Create a for loop to iterate over the X values of the entire bitmap.
            for (int x = 0; x < input_bitmap.Width; x++)
            {
                // Create a for loop to iterate over the Y values of the entire bitmap.
                for (int y = 0; y < input_bitmap.Height; y++)
                {
                    // Get the current pixel from the input bitmap.
                    actual_color = input_bitmap.GetPixel(x, y);

                    // Color in the pixel with the new color while keeping its current alpha value.
                    System.Drawing.Color new_color = System.Drawing.Color.FromArgb(actual_color.A, 0, 0, 0);
                    new_bitmap.SetPixel(x, y, new_color);
                }
            }

            return new_bitmap;
        }

        public static Bitmap Text_To_Black(Bitmap input_bitmap)
        {
            // Create a color variable to store the color value of a pixel on the input bitmap later.
            System.Drawing.Color actual_color;

            // Make an empty bitmap the same size as the input bitmap.
            Bitmap new_bitmap = new Bitmap(input_bitmap.Width, input_bitmap.Height);

            // Create a for loop to iterate over the X values where the name could be rendered.
            for (int x = 0; x < input_bitmap.Width; x++)
            {
                // Create a nested for loop to iterate over the Y values where the name could be rendered.
                for (int y = 0; y < input_bitmap.Height; y++)
                {
                    // Get the current pixel from the input bitmap.
                    actual_color = input_bitmap.GetPixel(x, y);

                    // Color in the pixel with the new color while keeping its current alpha value.
                    System.Drawing.Color new_color = System.Drawing.Color.FromArgb(actual_color.A, 0, 0, 0);
                    new_bitmap.SetPixel(x, y, new_color);
                }
            }

            return new_bitmap;
        }

        public static Bitmap Keep_Pixel_Overlap(Bitmap bottom_bitmap, Bitmap top_bitmap, int x_start, int x_end, int y_start, int y_end)
        {
            // First, let's set up variables for the pixels we'll be iterating over on both bitmaps.
            System.Drawing.Color bottom_pixel_color;
            System.Drawing.Color top_pixel_color;

            // We'll also need a new color that has components of both pixel colors, so establish a variable for that here.
            System.Drawing.Color new_color;

            // Create a new bitmap to return that's the same size of the bottom bitmap.
            Bitmap base_template = new Bitmap(bottom_bitmap.Width, bottom_bitmap.Height);

            // Now, let's use a loop to start iterating through the X and Y values we need to work on.
            for (int x = x_start; x < x_end; x++)
            {
                for (int y = y_start; y < y_end; y++)
                {
                    // Get the colors of the pixels at the current X and Y coordinate for both the bottom and top bitmaps.
                    bottom_pixel_color = bottom_bitmap.GetPixel(x, y);
                    top_pixel_color = top_bitmap.GetPixel(x, y);

                    // Check if the alpha values of both pixels are greater than zero.
                    if (bottom_pixel_color.A > 0 && top_pixel_color.A > 0)
                    {
                        // If so, create a new color with the alpha value of the bottom pixel and RGB values of the top pixel and draw it to the new bitmap.
                        new_color = System.Drawing.Color.FromArgb(bottom_pixel_color.A, top_pixel_color.R, top_pixel_color.G, top_pixel_color.B);
                        base_template.SetPixel(x, y, new_color);
                    }

                }
            }

            return base_template;
        }

        public static Bitmap Keep_Border_Pixel_Overlap(Bitmap bottom_bitmap, Bitmap top_bitmap, int x_start, int x_end, int y_start, int y_end)
        {
            // First, let's set up variables for the pixels we'll be iterating over on both bitmaps.
            System.Drawing.Color bottom_pixel_color;
            System.Drawing.Color top_pixel_color;

            // We'll also need a new color that has components of both pixel colors, so establish a variable for that here.
            System.Drawing.Color new_color;

            // Create a new bitmap to return that's the same size of the bottom bitmap.
            Bitmap base_template = new Bitmap(bottom_bitmap.Width, bottom_bitmap.Height);

            // Now, let's use a loop to start iterating through the X and Y values we need to work on.
            for (int x = x_start; x < x_end; x++)
            {
                for (int y = y_start; y < y_end; y++)
                {
                    // Get the colors of the pixels at the current X and Y coordinate for both the bottom and top bitmaps.
                    bottom_pixel_color = bottom_bitmap.GetPixel(x, y);
                    top_pixel_color = top_bitmap.GetPixel(x, y);

                    // Check if the alpha values of both pixels are greater than zero.
                    if (bottom_pixel_color.A > 0 && top_pixel_color.A > 0)
                    {
                        // If so, create a new color with the alpha value of the bottom pixel and RGB values of the top pixel and draw it to the new bitmap.
                        new_color = System.Drawing.Color.FromArgb(top_pixel_color.A, top_pixel_color.R, top_pixel_color.G, top_pixel_color.B);
                        base_template.SetPixel(x, y, new_color);
                    }

                }
            }

            return base_template;
        }

        // Method from https://www.codeproject.com/Tips/201129/Change-Opacity-of-Image-in-C
        public static System.Drawing.Image Set_Image_Opacity(System.Drawing.Image image, float opacity)
        {
            try
            {
                //create a Bitmap the size of the image provided  
                Bitmap bmp = new Bitmap(image.Width, image.Height);

                //create a graphics object from the image  
                using (Graphics gfx = Graphics.FromImage(bmp))
                {
                    //create a color matrix object  
                    ColorMatrix matrix = new ColorMatrix();

                    //set the opacity  
                    matrix.Matrix33 = opacity;

                    //create image attributes  
                    ImageAttributes attributes = new ImageAttributes();

                    //set the color(opacity) of the image  
                    attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                    //now draw the image  
                    gfx.DrawImage(image, new Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attributes);
                }
                return bmp;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        public static Bitmap RotateImage(Bitmap rotateMe, float angle)
        {
            //First, re-center the image in a larger image that has a margin/frame
            //to compensate for the rotated image's increased size

            var bmp = new Bitmap(rotateMe.Width + (rotateMe.Width / 2), rotateMe.Height + (rotateMe.Height / 2));

            using (Graphics g = Graphics.FromImage(bmp))
                g.DrawImage(rotateMe, (rotateMe.Width / 4), (rotateMe.Height / 4), rotateMe.Width, rotateMe.Height);

            rotateMe = bmp;

            //Now, actually rotate the image
            Bitmap rotatedImage = new Bitmap(rotateMe.Width, rotateMe.Height);

            using (Graphics g = Graphics.FromImage(rotatedImage))
            {
                g.TranslateTransform(rotateMe.Width / 2, rotateMe.Height / 2);   //set the rotation point as the center into the matrix
                g.RotateTransform(angle);                                        //rotate
                g.TranslateTransform(-rotateMe.Width / 2, -rotateMe.Height / 2); //restore rotation point into the matrix
                g.DrawImage(rotateMe, new Point(0, 0));                          //draw the image on the new bitmap
            }

            return rotatedImage;
        }

        // Method from https://www.codeproject.com/Articles/9184/Custom-AntiAliasing-with-GDI
        public static Bitmap Custom_Antiailiasing(Bitmap input_bitmap, Point[] input_array)
        {
            // Make a 4X offscreen bitmap, power of 2's are important because 
            // interpolating other size images takes significantly longer.
            Bitmap scaled_bitmap = new Bitmap(input_bitmap.Width * 4, input_bitmap.Height * 4);
            using (Graphics graphics = Graphics.FromImage(scaled_bitmap))
            {
                graphics.DrawImage(input_bitmap, 0, 0, scaled_bitmap.Width, scaled_bitmap.Height);
            }
            Bitmap base_template = new Bitmap(input_bitmap.Width, input_bitmap.Height);

            // Update transform for additional pixels
            Matrix myMatrix = new Matrix();
            myMatrix.Scale(4, 4, MatrixOrder.Append);
            myMatrix.TransformPoints(input_array);

            using (Graphics graphics = Graphics.FromImage(scaled_bitmap))
            {
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                graphics.FillPolygon(new SolidBrush(System.Drawing.Color.Transparent), input_array);
            }

            // Stretch blit the rendered image to the actual image
            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.DrawImage(scaled_bitmap, 0, 0, base_template.Width, base_template.Height);
            }

            return base_template;
        } 

        // Getter methods
        public static int Get_Number_of_Rendered_Lines(List<string>[] input_list_array)
        {
            // Initialize an int variable to hold the number of rendered lines.
            int number_of_lines = 0;

            // Take each index of the string list array, convert the list to a string, then analyze the string to determine if it's empty or not.
            // If it IS empty, that line won't be rendered.
            // Count the number of lines that will actually be rendered to the screen.
            if (String_List_To_String(input_list_array[2]) != "")
            {
                number_of_lines = 3;
            }
            else if (String_List_To_String(input_list_array[1]) != "")
            {
                number_of_lines = 2;
            }
            else
            {
                number_of_lines = 1;
            }

            return number_of_lines;
        }

        public static int Get_Max_Line_Length(List<string>[] input_list_array)
        {
            // Initialize an int variable to hold the max line length as measured in pixels.
            int max_line_length = 0;

            // Now, let's iterate through the string list array to find out which line is the longest.
            // Take each index of the string list array, convert the current iterated list to a string, then measure the string's pixel length.
            // If the pixel length is longer than the number held in max_line_length, store that new number in the max_line_length variable instead.
            for (int line_index = 0; line_index < input_list_array.Length; line_index++)
            {
                string current_list = String_List_To_String(input_list_array[line_index]);

                int current_string_length = Measure_String_Pixel_Length(null, current_list);

                if (current_string_length > max_line_length)
                {
                    max_line_length = current_string_length;
                }
            }

            return max_line_length;
        }

        public static DateTime Get_Date(SocialLinkerCommand sl_command, UserInfoFields account)
        {
            // Create an empty string variable. This is where the API request will be stored.
            string json_location = "";

            // Create a default DateTime variable.
            // We'll use this to store the user's set time later.
            DateTime user_time = default;

            // Encapsulate the API request in a try-catch block. If the user inputs an invalid parameter, an exception will be thrown.
            try
            {
                // Make an API request with the account key and user input as parameters.
                using (WebClient client = new WebClient())
                {
                    json_location = new TimedWebClient { Timeout = Global.API_Timeout }.DownloadString($"http://api.weatherapi.com/v1/current.json?key={WeatherAPIConfig.weather_api_account.accountKey}&q={account.City}");
                }

                // Deserialize the JSON object and store it in a variable.
                var dataObject = JsonConvert.DeserializeObject<dynamic>(json_location);

                // Set the user's time to set location.
                user_time = dataObject.location.localtime;
            }
            catch (Exception ex)
            {
                // Log the error to the console.
                Console.WriteLine(ex);

                // Send a warning message to the user.
                _ = ErrorHandling.API_Timeout(sl_command);

                // Set the user's time to the current UTC time.
                user_time = DateTime.UtcNow;
            }

            return user_time;
        }

        public static string Get_Time_of_Day(DateTime input_time)
        {
            // Get the current hour of the user.
            TimeSpan hour = input_time.TimeOfDay;

            // Create an empty string variable to store the returned time of day value later on.
            string tod = "";

            // Lastly, establish the starting times for each time of day.
            TimeSpan morning = new TimeSpan(6, 0, 0);
            TimeSpan noon = new TimeSpan(12, 0, 0);
            TimeSpan evening = new TimeSpan(18, 0, 0);
            TimeSpan night = new TimeSpan(20, 0, 0);

            TimeSpan before_midnight = new TimeSpan(23, 59, 59);
            TimeSpan after_midnight = new TimeSpan(0, 0, 0);

            // Now, let's find out the current value.
            // If the current hour is before 12AM and after or on 8PM, set the time to Night.
            if (hour < before_midnight && hour >= night)
            {
                tod = "night";
            }
            // If the current hour is before 6AM and after or on 12AM, set the time to Night.
            else if (hour < morning && hour >= after_midnight)
            {
                tod = "night";
            }
            // If the current hour is before 12PM and after or on 6AM, set the time to Morning.
            else if (hour < noon && hour >= morning)
            {
                tod = "morning";
            }
            // If the current hour is before 6PM and after or on 12PM, set the time to Early Morning.
            else if (hour < evening && hour >= noon)
            {
                tod = "noon";
            }
            // If the current hour is before 8PM and after or on 6PM, set the time to Evening.
            else if (hour < night && hour >= evening)
            {
                tod = "evening";
            }
            else
            {
                tod = "null";
            }

            return tod;
        }

        public static EmbedBuilder P5S_Loading_Message()
        {
            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Generating Scene...",
                IconUrl = EmbedSettings.Get_Game_Logo("P5S")
            };

            embed.WithAuthor(author);
            embed.WithColor(EmbedSettings.Get_Game_Color("P5S", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Loading_Icon("P5S", null));
            embed.WithDescription("This may take a few seconds!");

            return embed;
        }
    }
}
