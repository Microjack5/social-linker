using System;
using System.Drawing;
using System.Threading.Tasks;
using SocialLinker.Core.SceneMaker.GlyphParsing;
using System.IO;
using Discord.WebSocket;
using SocialLinker.Config;
using SocialLinker.Core.LocalStorageTables;
using Discord;
using Discord.Rest;
using SocialLinker.Core.CloudStorageTables;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using System.Collections.Generic;
using SocialLinker.Core.SceneMaker.Data.Bustup;
using System.Globalization;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using SocialLinker.Core.Menus;

namespace SocialLinker.Core.SceneMaker.TemplateRenders.QuickScenes
{
    public class RenderP4_PS2
    {
        public int template_width = 640;
        public int template_height = 448;

        public int working_template_width = 640;
        public int working_template_height = 448;

        public int max_line_length = 500;

        public async Task Render_Quick_Scene_P4_PS2(SocialLinkerCommand sl_command, OfficialSetData set_data, MakerCommandData command_data)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = sl_command.User;
            SocketTextChannel channel = (SocketTextChannel)sl_command.Channel;

            // Send a loading message to the channel while the sprite sheet is being made.
            RestUserMessage loader = await channel.SendMessageAsync("", false, P4_PS2_Loading_Message().Build());

            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            BustupData bustup_data = BustupDataMethods.Get_Bustup_Data(account, set_data, command_data);

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
            if (command_data.Base_Sprite != 0)
            {
                bustup = OfficialSetMethods.Bustup_Selection(sl_command, account, set_data, bustup_data, command_data);
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
                // Create and assign bitmap variables for the assets needed.
                Bitmap layer_1 = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P4-PS2//Main//layer_1.png");
                Bitmap layer_2 = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P4-PS2//Main//layer_2.png");
                Bitmap cursor = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P4-PS2//Main//cursor.png");

                // Draw the layer with the user's colored default background if it exists.
                graphics.DrawImage(colored_background_bitmap, 0, 0, template_width, template_height);

                // Draw the user's background to the base template.
                graphics.DrawImage(background, 0, 0, template_width, template_height);

                // Draw the first textbox layer to the base template.
                graphics.DrawImage(layer_1, 0, 0, template_width, template_height);

                // Draw the character bust-up to the template if the base sprite number is not '0'.
                if (command_data.Base_Sprite != 0)
                {
                    graphics.DrawImage(bustup, bustup_data.P4_PS2_Coord_X, bustup_data.P4_PS2_Coord_Y, bustup_data.P4_PS2_Scale_Width, bustup_data.P4_PS2_Scale_Height);
                }

                // Draw the brown textbox layer to the template last.
                graphics.DrawImage(layer_2, 0, 0, template_width, template_height);

                // Take the cursor bitmap and color it orange.
                cursor = Cursor_To_Orange(cursor);

                // Draw the cursor to the template.
                graphics.DrawImage(cursor, 0, 0, template_width, template_height);

                // If the user has the HUD enabled, render it to the template as well.
                if (account.P4_PS2_TS_HUD != "None")
                {
                    graphics.DrawImage(Render_Calendar_HUD(sl_command, account), 0, 0, template_width, template_height);
                }
            }

            base_template = Scale_Template(account, base_template);

            // Create another graphics object for the base template.
            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                // Render the character's name to the template first.
                string display_name = OfficialSetMethods.GetDisplayName(account, command_data, set_data, bustup_data);
                display_name = OfficialSetMethods.Validate_Input(sl_command, "P4-PS2", "Name", display_name);

                graphics.DrawImage(Text_To_Brown(Render_Name(display_name)), 0, 0, template_width, template_height);

                command_data.Dialogue = OfficialSetMethods.Validate_Input(sl_command, "P4-PS2", "Dialogue", command_data.Dialogue);
                List<string>[] parsed_lines = OfficialSetMethods.Line_Parser(sl_command, "P4-PS2", command_data.Dialogue, 3, max_line_length);

                // Draw the input dialogue to the template.
                graphics.DrawImage(Render_Dialogue(parsed_lines), 0, 0, template_width, template_height);
            }

            // Save the entire base template to a data stream.
            MemoryStream memoryStream = new MemoryStream();
            base_template.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
            memoryStream.Seek(0, SeekOrigin.Begin);

            try
            {
                // Send the image.
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
            RestUserMessage loader = await channel.SendMessageAsync("", false, P4_PS2_Loading_Message().Build());

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
                // Create and assign bitmap variables for the assets needed.
                Bitmap layer_1 = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P4-PS2//Main//layer_1.png");
                Bitmap layer_2 = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P4-PS2//Main//layer_2.png");
                Bitmap cursor = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P4-PS2//Main//cursor.png");

                // Draw the layer with the user's colored default background if it exists.
                graphics.DrawImage(colored_background_bitmap, 0, 0, template_width, template_height);

                // Draw the user's background to the base template.
                graphics.DrawImage(background, 0, 0, template_width, template_height);

                // Draw the first textbox layer to the base template.
                graphics.DrawImage(layer_1, 0, 0, template_width, template_height);

                // Draw the brown textbox layer to the template last.
                graphics.DrawImage(layer_2, 0, 0, template_width, template_height);

                // Take the cursor bitmap and color it orange.
                cursor = Cursor_To_Orange(cursor);

                // Draw the cursor to the template.
                graphics.DrawImage(cursor, 0, 0, template_width, template_height);

                // If the user has the HUD enabled, render it to the template as well.
                if (account.P4_PS2_TS_HUD != "None")
                {
                    graphics.DrawImage(Render_Calendar_HUD(sl_command, account), 0, 0, template_width, template_height);
                }
            }

            base_template = Scale_Template(account, base_template);

            // Create another graphics object for the base template.
            // We'll start rendering our needed text here.
            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                command_data.Dialogue = OfficialSetMethods.Validate_Input(sl_command, "P4-PS2", "Dialogue", command_data.Dialogue);
                List<string>[] parsed_lines = OfficialSetMethods.Line_Parser(sl_command, "P4-PS2", command_data.Dialogue, 3, max_line_length);

                // Draw the input dialogue to the template.
                graphics.DrawImage(Render_Dialogue(parsed_lines), 0, 0, template_width, template_height);
            }

            // Save the entire base template to a data stream.
            MemoryStream memoryStream = new MemoryStream();
            base_template.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
            memoryStream.Seek(0, SeekOrigin.Begin);

            // Send the image.
            await sl_command.Channel.SendFileAsync(memoryStream, $"scene_{sl_command.User.Id}_{DateTime.UtcNow}.png");

            // Delete the loading message.
            await loader.DeleteAsync();

            // If the user has auto-delete for their commands set to on, delete their command as well.
            if (account.Auto_Delete_Commands == "On")
            {
                await sl_command.Message.DeleteAsync();
            }
        }

        public Bitmap Render_Name(string display_name)
        {
            // Create a 640 x 448 bitmap.
            Bitmap base_template = new Bitmap(640, 448);

            // Establish an int for the width and height glyphs should be rendered at.
            int multiplier = 32;

            // Load the bitmap font.
            string font_sheet = $@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P4-PS2//Font//p4-ps2_font_sheet.png";

            // Create a variable to iterate through sections of the bitmap font.
            Bitmap current_glyph;

            // Specify X and Y coordinates for where the glyphs should start rendering on the template.
            int render_position_x = 43;
            int render_position_y = 306;

            // Thake the sprite's display name and convert it into a char array.
            char[] char_array = display_name.ToCharArray();

            // Iterate through each character of the array.
            for (int i = 0; i < char_array.Length; i++)
            {
                // Retrieve glyph information from the JSON file.
                var glyph = ParsingMethods.Get_P4_PS2_Glyph(char_array[i]);

                // Check if the character is a line break.
                if (char_array[i] == '\u000a')
                {
                    // Set the X coordinate to the start of the row.
                    render_position_x = 124;
                    // Move the Y coordinate down to the next line.
                    render_position_y += 68;
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
                            current_glyph = originalImage.Clone(crop, originalImage.PixelFormat);

                            // Draw the glyph to the base bitmap. Some hanging letters will need to be drawn a bit lower to appear natural.
                            if (char_array[i] == 'g' || char_array[i] == 'j' || char_array[i] == 'p' || char_array[i] == 'q' || char_array[i] == 'y')
                            {
                                graphics.DrawImage(current_glyph, (render_position_x - glyph.LeftCut), render_position_y + 2, multiplier, multiplier);
                            }
                            else
                            {
                                graphics.DrawImage(current_glyph, (render_position_x - glyph.LeftCut), render_position_y, multiplier, multiplier);
                            }
                        }
                    }

                    // Set the next X value at the end of the current glyph's right width.
                    render_position_x += (glyph.RightCut - glyph.LeftCut);

                    // Check if the current iterated index is less than the number of indicies available.
                    if (i < char_array.Length - 1)
                    {
                        // If so, edit the position of the X coordinate according to specific kerning pairs.
                        if (char_array[i] == 'Y' && Char.IsLower(char_array[i + 1]))
                        {
                            render_position_x += -4;
                        }
                        else if (char_array[i] == 'T' && Char.IsLower(char_array[i + 1]) && char_array[i + 1] != 'h')
                        {
                            render_position_x += -3;
                        }
                        // If no kerning pairs are matched, create a universal rule that moves the X cursor by -2 pixels.
                        // This is specific to the P4-PS2 font.
                        else
                        {
                            render_position_x += -2;
                        }
                    }
                }
            }

            return base_template;
        }

        public Bitmap Render_Dialogue(List<string>[] dialogue_lines)
        {
            // Create a 640 x 448 bitmap.
            Bitmap bitmap = new Bitmap(working_template_width, working_template_height);

            // Create an int to keep track of rendering errors. This is neccessary to inform the user of any potential issues.
            int error_counter = 0;

            //Establish an int for the width and height glyphs should be rendered at
            int multiplier = 32;

            string font_sheet = $@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P4-PS2//Font//p4-ps2_font_sheet.png";
            Bitmap current_glyph;

            // Iterate over each line of the dialogue string list with a loop.
            for (int i = 0; i < dialogue_lines.Length; i++)
            {
                // Specify X and Y coordinates for where the glyphs should start rendering on the template.
                int render_position_x = 56;
                int render_position_y = 338 + (25 * i);

                // Take the current line of input dialogue and convert it into a char array.
                char[] char_array = String_List_To_String(dialogue_lines[i]).ToCharArray();

                // Iterate through each character of the array.
                for (int j = 0; j < char_array.Length; j++)
                {
                    //Retrieve glyph information from the JSON file
                    var glyph = ParsingMethods.GetGlyph(char_array[j]);

                    // If the glyph info returns null, we have a rendering error.
                    // If this occurs and the error counter is at zero, increase the error counter and send a message to the user.
                    if (glyph == null && error_counter == 0)
                    {
                        error_counter++;
                        //message.Channel.SendMessageAsync(":warning: One or more of the characters entered is not supported by this template's font set and will not be rendered.");
                    }

                    if (glyph != null)
                    {
                        int x = multiplier * glyph.Column;
                        int y = multiplier * glyph.Row;

                        using (Graphics graphics = Graphics.FromImage(bitmap))
                        {
                            using (var originalImage = new Bitmap(font_sheet))
                            {
                                // Copy the section of the bitmap font needed.
                                Rectangle crop = new Rectangle(x, y, multiplier, multiplier);
                                current_glyph = originalImage.Clone(crop, originalImage.PixelFormat);

                                // Draw the glyph to the base bitmap. Some hanging letters will need to be drawn a bit lower to appear natural.
                                if (char_array[j] == 'g' || char_array[j] == 'j' || char_array[j] == 'p' || char_array[j] == 'q' || char_array[j] == 'y')
                                {
                                    graphics.DrawImage(current_glyph, (render_position_x - glyph.LeftCut), render_position_y + 2, multiplier, multiplier);
                                }
                                else
                                {
                                    graphics.DrawImage(current_glyph, (render_position_x - glyph.LeftCut), render_position_y, multiplier, multiplier);
                                }
                            }
                        }

                        //Set the next X value at the end of the current glyph's right width
                        render_position_x += (glyph.RightCut - glyph.LeftCut);

                        // Check if the current iterated index is less than the number of indicies available.
                        if (j < char_array.Length - 1)
                        {
                            // If so, edit the position of the X coordinate according to specific kerning pairs.
                            if (char_array[j] == 'Y' && Char.IsLower(char_array[j + 1]))
                            {
                                render_position_x += -4;
                            }
                            else if (char_array[j] == 'T' && Char.IsLower(char_array[j + 1]) && char_array[j + 1] != 'h')
                            {
                                render_position_x += -3;
                            }
                            // If no kerning pairs are matched, create a universal rule that moves the X cursor by -2 pixels.
                            // This is specific to the P4-PS2 font.
                            else
                            {
                                render_position_x += -2;
                            } 
                        }
                    }
                }
            }

            return bitmap;
        }

        public static int Measure_Word_Pixel_Length(SocialLinkerCommand sl_command, string input_word)
        {
            // Create an int to keep track of how many pixels a glyph is wide in.
            int pixel_counter = 0;

            // Take the input string and turn it into a char array.
            char[] char_array = input_word.ToCharArray();

            // Now, let's iterate through the char array.
            for (int i = 0; i < char_array.Length; i++)
            {
                // Retrieve glyph information for the current character from the JSON file.
                var glyph = ParsingMethods.Get_P4_PS2_Glyph(char_array[i]);

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
                            if (char_array[i] == 'Y' && Char.IsLower(char_array[i + 1]))
                            {
                                pixel_counter += -4;
                            }
                            else if (char_array[i] == 'T' && Char.IsLower(char_array[i + 1]) && (char_array[i + 1] != 'h'))
                            {
                                pixel_counter += -3;
                            }
                            // If no kerning pairs are matched, create a universal rule that moves the X cursor by -2 pixels.
                            // This is specific to the P4-PS2 font.
                            else
                            {
                                pixel_counter += -2;
                            }
                        }

                        // Set the pixel counter to the appropriate width of the string so far.
                        pixel_counter += glyph.RightCut - glyph.LeftCut;
                    }
                }
                // If the character returns null, it's not supported by the template's font set.
                else
                {
                    sl_command.MakerCommand.Dialogue_Has_Invalid_Char = true;
                }
            }

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

        public Bitmap Render_Calendar_HUD(SocialLinkerCommand sl_command, UserInfoFields account)
        {
            // Establish needed bitmap variables for the assets.
            Bitmap date_container = new Bitmap(2, 2);
            Bitmap weather_container = new Bitmap(2, 2);
            Bitmap hud = new Bitmap(2, 2);
            Bitmap corner_glow = new Bitmap(2, 2);

            // Create a new bitmap with the width and height values specified earlier.
            Bitmap base_template = new Bitmap(working_template_width, working_template_height);

            // Now, time to put the template together!
            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                // Establish all variables needed and set them to null; they will be assigned to soon.
                Bitmap month_tens = null;
                Bitmap month_ones = null;

                Bitmap day_tens = null;
                Bitmap day_ones = null;

                Bitmap day_of_week = null;
                Bitmap time_of_day = null;
                Bitmap date_slash = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P4-PS2//Main//Calendar//slash.png");

                Bitmap weather = new Bitmap(2, 2);

                // Get the user's current date and time according to their settings.
                DateTime user_time = Get_Date(sl_command, account);

                // Use the user's date and time to determine which assets to use.
                // Months
                char[] month = user_time.ToString("MM").ToCharArray();

                month_tens = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P4-PS2//Main//Calendar//Month//Tens_Place//{month[0]}.png");
                month_ones = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P4-PS2//Main//Calendar//Month//Ones_Place//{month[1]}.png");

                // Days
                char[] day = user_time.ToString("dd").ToCharArray();
                day_tens = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P4-PS2//Main//Calendar//Day//Tens_Place//{day[0]}.png");
                day_ones = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P4-PS2//Main//Calendar//Day//Ones_Place//{day[1]}.png");

                day_of_week = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P4-PS2//Main//Calendar//Weekday//{user_time.ToString("dddd").ToLower()}.png");

                // Get the user's time of day and store it in a string variable.
                // We'll be using this to retrieve the proper time of day bitmap for the template.
                string tod_string = Get_Time_of_Day(user_time);

                // If the HUD template setting is not set to "None", assign some common assets to the bitmap variables.
                if (account.P4_PS2_TS_HUD != "None")
                {
                    corner_glow = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P4-PS2//Main//corner_glow.png");
                    date_container = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P4-PS2//Main//date_container.png");
                }

                // Check if the user's HUD settings for the template is set to "TV World".
                if (account.P4_PS2_TS_HUD == "TV World")
                {
                    // If so, check if the time of day is currently "After School" or "Daytime".
                    // These are the two time periods that have TV World versions in game, so we'll want to use those if so.
                    if (tod_string == "after_school" || tod_string == "daytime")
                    {
                        time_of_day = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P4-PS2//Main//Calendar//Time_of_Day//TV//{tod_string}.png");
                    }
                    // If not, we'll have to take the normal variant of the current time period and make it negative.
                    else
                    {
                        // Grab the respective time of day bitmap for the user.
                        time_of_day = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P4-PS2//Main//Calendar//Time_of_Day//Normal//{tod_string}.png");

                        // Take the time of day bitmap and invert the colors by making it negative.
                        time_of_day = Invert_Bitmap(time_of_day);
                    }

                    // Assign the TV World HUD to a bitmap variable.
                    hud = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P4-PS2//Main//hud_tv.png");
                }
                // Else, check if the user's HUD settings for the template is set to "Normal" instead.
                else if (account.P4_PS2_TS_HUD == "Normal")
                {
                    // Grab the respective time of day bitmap for the user.
                    time_of_day = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P4-PS2//Main//Calendar//Time_of_Day//Normal//{tod_string}.png");
                    
                    // Also grab other assets exclusive to the Normal template setting: The normal HUD, weather container, and the appropriate weather asset.
                    hud = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P4-PS2//Main//hud_normal.png");
                    weather_container = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P4-PS2//Main//weather_container.png");
                    weather = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P4-PS2//Main//Weather//{Get_Weather(account)}.png");
                }

                // Take the date container bitmap and color it black.
                date_container = Date_Container_To_Black(date_container);

                // Color the day of week bitmap depending on what day it currently is.
                if (user_time.ToString("dddd").ToLower() == "sunday" || OfficialSetMethods.Is_Holiday(user_time))
                {
                    day_of_week = Day_Of_Week_To_Off_Day_Color_Scheme(day_of_week);
                }
                else if (user_time.ToString("dddd").ToLower() == "saturday")
                {
                    day_of_week = Day_Of_Week_To_Saturday_Color_Scheme(day_of_week);
                }

                // Draw all the assets to the template.
                graphics.DrawImage(corner_glow, 0, 0, template_width, template_height);
                graphics.DrawImage(hud, 0, 0, template_width, template_height);

                graphics.DrawImage(date_container, 0, 0, template_width, template_height);
                graphics.DrawImage(weather_container, 0, 0, template_width, template_height);

                graphics.DrawImage(month_tens, 0, 0, template_width, template_height);
                graphics.DrawImage(month_ones, 0, 0, template_width, template_height);
                graphics.DrawImage(date_slash, 0, 0, template_width, template_height);
                graphics.DrawImage(day_tens, 0, 0, template_width, template_height);
                graphics.DrawImage(day_ones, 0, 0, template_width, template_height);

                graphics.DrawImage(day_of_week, 0, 0, template_width, template_height);
                graphics.DrawImage(time_of_day, 0, 0, template_width, template_height);

                graphics.DrawImage(weather, 0, 0, template_width, template_height);
            }

            return base_template;
        }

        // Getter methods
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

        public static string Get_Weather(UserInfoFields account)
        {
            // Create an empty string variable. This is where the API request will be stored.
            string json_location = "";

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

                string current_condition = dataObject.current.condition.text.ToString();

                if (current_condition == "Sunny")
                {
                    return "sun";
                }
                else if (
                    current_condition == "Mist" ||
                    current_condition == "Fog" ||
                    current_condition == "Freezing fog")
                {
                    return "fog";
                }
                else if (
                    current_condition == "Cloudy" ||
                    current_condition == "Partly cloudy" ||
                    current_condition == "Overcast" ||
                    current_condition == "Clear")
                {
                    return "cloud";
                }
                else if (
                    current_condition == "Patchy rain possible" ||
                    current_condition == "Patchy freezing drizzle possible" ||
                    current_condition == "Patchy light drizzle" ||
                    current_condition == "Light drizzle" ||
                    current_condition == "Freezing drizzle" ||
                    current_condition == "Heavy freezing drizzle" ||
                    current_condition == "Patchy light rain" ||
                    current_condition == "Light rain" ||
                    current_condition == "Moderate rain at times" ||
                    current_condition == "Moderate rain" ||
                    current_condition == "Heavy rain at times" ||
                    current_condition == "Heavy rain" ||
                    current_condition == "Light freezing rain" ||
                    current_condition == "Moderate or heavy freezing rain" ||
                    current_condition == "Light rain shower" ||
                    current_condition == "Moderate or heavy rain shower" ||
                    current_condition == "Torrential rain shower" ||
                    current_condition == "Thundery outbreaks possible" ||
                    current_condition == "Patchy light rain with thunder" ||
                    current_condition == "Moderate or heavy rain with thunder" ||
                    current_condition == "Patchy light snow with thunder" ||
                    current_condition == "Moderate or heavy snow with thunder")
                {
                    return "rain";
                }
                else if (
                    current_condition == "Patchy snow possible" ||
                    current_condition == "Patchy sleet possible" ||
                    current_condition == "Blowing snow" ||
                    current_condition == "Blizzard" ||
                    current_condition == "Light sleet" ||
                    current_condition == "Moderate or heavy sleet" ||
                    current_condition == "Patchy light snow" ||
                    current_condition == "Light snow" ||
                    current_condition == "Patchy moderate snow" ||
                    current_condition == "Moderate snow" ||
                    current_condition == "Patchy heavy snow" ||
                    current_condition == "Heavy snow" ||
                    current_condition == "Ice pellets" ||
                    current_condition == "Light sleet showers" ||
                    current_condition == "Moderate or heavy sleet showers" ||
                    current_condition == "Light snow showers" ||
                    current_condition == "Moderate or heavy snow showers" ||
                    current_condition == "Light showers of ice pellets" ||
                    current_condition == "Moderate or heavy showers of ice pellets")
                {
                    return "snow";
                }
                else
                {
                    return "cloud";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                // Return a default condition.
                return "cloud";
            }
        }

        public static string Get_Time_of_Day(DateTime input_time)
        {
            // Get the current hour of the user.
            TimeSpan hour = input_time.TimeOfDay;

            // Create an empty string variable to store the returned time of day value later on.
            string tod = "";

            // Lastly, establish the starting times for each time of day.
            TimeSpan early_morning = new TimeSpan(6, 0, 0);
            TimeSpan morning = new TimeSpan(8, 0, 0);
            TimeSpan lunchtime = new TimeSpan(12, 0, 0);
            TimeSpan afternoon = new TimeSpan(12, 0, 0);
            TimeSpan after_school = new TimeSpan(15, 0, 0);
            TimeSpan evening = new TimeSpan(18, 0, 0);

            TimeSpan before_midnight = new TimeSpan(23, 59, 59);
            TimeSpan after_midnight = new TimeSpan(0, 0, 0);

            // Now, let's find out the current value.
            // If the current hour is before 12AM and after or on 6PM, set the time to Evening.
            if (hour < before_midnight && hour >= evening)
            {
                tod = "evening";
            }
            // If the current hour is before 6AM and after or on 12AM, set the time to Evening.
            else if (hour < early_morning && hour >= after_midnight)
            {
                tod = "evening";
            }
            // If the current hour is before 6PM and after or on 3PM, set the time depending on the day.
            // If it's a Sunday or outside of a school term, set it to Daytime.
            // If it's a weekday, set it to After School.
            else if (hour < evening && hour >= after_school)
            {
                if (DateTime.Now.ToString("ddd") == "Sun" || !OfficialSetMethods.Is_School_Term(input_time))
                {
                    tod = "daytime";
                }
                else
                {
                    tod = "after_school";
                }
            }
            // If the current hour is before 1PM and after or on 12PM, set the time depending on the day.
            // If it's a Sunday or outside of a school term, set it to Daytime.
            // If it's a weekday, set it to Lunchtime.
            else if (hour < after_school && hour >= lunchtime)
            {
                if (DateTime.Now.ToString("ddd") == "Sun" || !OfficialSetMethods.Is_School_Term(input_time))
                {
                    tod = "daytime";
                }
                else
                {
                    tod = "lunchtime";
                }
            }
            // If the current hour is before 12PM and after or on 8AM, set the time to Morning.
            else if (hour < lunchtime && hour >= morning)
            {
                tod = "morning";
            }
            // If the current hour is before 8AM and after or on 6AM, set the time to Early Morning.
            else if (hour < morning && hour >= early_morning)
            {
                tod = "early_morning";
            }
            else
            {
                tod = "null";
            }

            return tod;
        }

        // Coloring bitmaps
        public static Bitmap Text_To_Brown(Bitmap input_bitmap)
        {
            // Create a color variable to store the color value of a pixel on the input bitmap later.
            System.Drawing.Color actual_color;

            // Make an empty bitmap the same size as the input bitmap.
            Bitmap new_bitmap = new Bitmap(input_bitmap.Width, input_bitmap.Height);

            // Create a for loop to iterate over the X values where the name could be rendered.
            for (int x = 37; x < 480; x++)
            {
                // Create a nested for loop to iterate over the Y values where the name could be rendered.
                for (int y = 310; y < 338; y++)
                {
                    // Get the current pixel from the input bitmap.
                    actual_color = input_bitmap.GetPixel(x, y);

                    // Color in the pixel with the new color while keeping its current alpha value.
                    System.Drawing.Color new_color = System.Drawing.Color.FromArgb(actual_color.A, 25, 24, 25);
                    new_bitmap.SetPixel(x, y, new_color);
                }
            }

            return new_bitmap;
        }

        public static Bitmap Cursor_To_Orange(Bitmap input_bitmap)
        {
            // Create a color variable to store the color value of a pixel on the input bitmap later.
            System.Drawing.Color actual_color;

            // Make an empty bitmap the same size as the input bitmap.
            Bitmap new_bitmap = new Bitmap(input_bitmap.Width, input_bitmap.Height);

            // Create a for loop to iterate over the X values where the name could be rendered.
            for (int x = 593; x < 620; x++)
            {
                // Create a nested for loop to iterate over the Y values where the name could be rendered.
                for (int y = 395; y < 422; y++)
                {
                    // Get the current pixel from the input bitmap.
                    actual_color = input_bitmap.GetPixel(x, y);

                    // Color in the pixel with the new color while keeping its current alpha value.
                    System.Drawing.Color new_color = System.Drawing.Color.FromArgb(actual_color.A, 255, 157, 3);
                    new_bitmap.SetPixel(x, y, new_color);
                }
            }

            return new_bitmap;
        }

        public static Bitmap Date_Container_To_Black(Bitmap input_bitmap)
        {
            // Create a color variable to store the color value of a pixel on the input bitmap later.
            System.Drawing.Color actual_color;

            // Make an empty bitmap the same size as the input bitmap.
            Bitmap new_bitmap = new Bitmap(input_bitmap.Width, input_bitmap.Height);

            // Create a for loop to iterate over the X values where the name could be rendered.
            for (int x = 445; x < 605; x++)
            {
                // Create a nested for loop to iterate over the Y values where the name could be rendered.
                for (int y = 15; y < 35; y++)
                {
                    // Get the current pixel from the input bitmap.
                    actual_color = input_bitmap.GetPixel(x, y);

                    // Color in the pixel with the new color while keeping its current alpha value.
                    System.Drawing.Color new_color = System.Drawing.Color.FromArgb(actual_color.A, 21, 21, 21);
                    new_bitmap.SetPixel(x, y, new_color);
                }
            }

            return new_bitmap;
        }

        public static Bitmap Day_Of_Week_To_Saturday_Color_Scheme(Bitmap input_bitmap) // Saturdays
        {
            // Create a color variable. We'll use this to iterate through the input bitmap and store each pixel's color values here.
            System.Drawing.Color actual_color;

            // Make an empty bitmap the same size as the input bitmap.
            Bitmap new_bitmap = new Bitmap(input_bitmap.Width, input_bitmap.Height);

            // Create a for loop to iterate over the X values of the bitmap to be changed.
            for (int x = 445; x < 605; x++)
            {
                // Create a nested for loop to iterate over the Y values of the bitmap to be changed.
                for (int y = 15; y < 35; y++)
                {
                    // Get the current pixel from the input bitmap.
                    actual_color = input_bitmap.GetPixel(x, y);

                    // Color in the pixel with the new color while keeping its current alpha value.
                    System.Drawing.Color new_color = System.Drawing.Color.FromArgb(actual_color.A, 153, 156, 223);
                    new_bitmap.SetPixel(x, y, new_color);
                }
            }

            return new_bitmap;
        }

        public static Bitmap Day_Of_Week_To_Off_Day_Color_Scheme(Bitmap input_bitmap) // Sundays and Holidays
        {
            // Create a color variable. We'll use this to iterate through the input bitmap and store each pixel's color values here.
            System.Drawing.Color actual_color;

            // Make an empty bitmap the same size as the input bitmap.
            Bitmap new_bitmap = new Bitmap(input_bitmap.Width, input_bitmap.Height);

            // Create a for loop to iterate over the X values of the bitmap to be changed.
            for (int x = 445; x < 605; x++)
            {
                // Create a nested for loop to iterate over the Y values of the bitmap to be changed.
                for (int y = 15; y < 35; y++)
                {
                    // Get the current pixel from the input bitmap.
                    actual_color = input_bitmap.GetPixel(x, y);

                    // Color in the pixel with the new color while keeping its current alpha value.
                    System.Drawing.Color new_color = System.Drawing.Color.FromArgb(actual_color.A, 247, 184, 179);
                    new_bitmap.SetPixel(x, y, new_color);
                }
            }

            return new_bitmap;
        }

        public Bitmap Scale_Template(UserInfoFields account, Bitmap input_template)
        {
            var scaled_bitmap = new Bitmap(2, 2);

            if (account.P4_PS2_Resolution == "640 × 448")
            {
                // Do nothing if setting is at default resolution
            }
            else
            {
                if (account.P4_PS2_Resolution == "640 × 480")
                {
                    template_width = 640;
                    template_height = 480;
                }
                else if (account.P4_PS2_Resolution == "1440 × 1080")
                {
                    template_width = 1440;
                    template_height = 1080;
                }

                var copied_input = new Bitmap(input_template);
                scaled_bitmap = new Bitmap(template_width, template_height);

                using (Graphics graphics = Graphics.FromImage(scaled_bitmap))
                {
                    // If the user's setting is at Full HD, set the scaling method to their choice of Bicubic and Nearest Neighbor.
                    if (account.P4_PS2_Resolution == "1440 × 1080")
                    {
                        switch (account.P3F_Scale)
                        {
                            case "Bicubic":
                                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                                break;

                            case "Nearest Neighbor":
                                graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                                break;
                        }
                    }
                    // Otherwise, set the method to Bicubic.
                    else
                    {
                        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    }

                    graphics.CompositingQuality = CompositingQuality.HighQuality;
                    graphics.DrawImage(copied_input, 0, 0, template_width, template_height);
                }

                input_template = scaled_bitmap;
            }

            return input_template;
        }

        // Loading message
        public static EmbedBuilder P4_PS2_Loading_Message()
        {
            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Generating Scene...",
                IconUrl = EmbedSettings.Get_Game_Logo("P4-PS2")
            };

            embed.WithAuthor(author);
            embed.WithColor(EmbedSettings.Get_Game_Color("P4-PS2", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Loading_Icon("P4-PS2", null));
            embed.WithDescription("This may take a few seconds!");

            return embed;
        }

        // Methods from https://softwarebydefault.com/2013/03/03/colomatrix-image-filters/
        private static Bitmap GetArgbCopy(System.Drawing.Image sourceImage)
        {
            Bitmap bmpNew = new Bitmap(sourceImage.Width, sourceImage.Height, PixelFormat.Format32bppArgb);

            using (Graphics graphics = Graphics.FromImage(bmpNew))
            {
                graphics.DrawImage(sourceImage, new Rectangle(0, 0, bmpNew.Width, bmpNew.Height), new Rectangle(0, 0, bmpNew.Width, bmpNew.Height), GraphicsUnit.Pixel);
                graphics.Flush();
            }

            return bmpNew;
        }

        private static Bitmap ApplyColorMatrix(System.Drawing.Image sourceImage, ColorMatrix colorMatrix)
        {
            Bitmap bmp32BppSource = GetArgbCopy(sourceImage);
            Bitmap bmp32BppDest = new Bitmap(bmp32BppSource.Width, bmp32BppSource.Height, PixelFormat.Format32bppArgb);

            using (Graphics graphics = Graphics.FromImage(bmp32BppDest))
            {
                ImageAttributes bmpAttributes = new ImageAttributes();
                bmpAttributes.SetColorMatrix(colorMatrix);

                graphics.DrawImage(bmp32BppSource, new Rectangle(0, 0, bmp32BppSource.Width, bmp32BppSource.Height),
                                    0, 0, bmp32BppSource.Width, bmp32BppSource.Height, GraphicsUnit.Pixel, bmpAttributes);
            }

            bmp32BppSource.Dispose();

            return bmp32BppDest;
        }

        // Method from https://stackoverflow.com/questions/33024881/invert-image-faster-in-c-sharp
        public static Bitmap Invert_Bitmap(Bitmap input_bitmap)
        {
            Bitmap base_template = new Bitmap(input_bitmap.Width, input_bitmap.Height);

            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                for (int y = 0; (y <= (input_bitmap.Height - 1)); y++)
                {
                    for (int x = 0; (x <= (input_bitmap.Width - 1)); x++)
                    {
                        System.Drawing.Color inv = input_bitmap.GetPixel(x, y);
                        inv = System.Drawing.Color.FromArgb(inv.A, (255 - inv.R), (255 - inv.G), (255 - inv.B));
                        base_template.SetPixel(x, y, inv);
                    }
                }
            }

            return base_template;
        }

        //public static Bitmap DrawAsNegative(this System.Drawing.Image sourceImage)
        //{
        //    ColorMatrix colorMatrix = new ColorMatrix(new float[][]
        //    {
        //        new float[]{-1, 0, 0, 0, 0},
        //        new float[]{0, -1, 0, 0, 0},
        //        new float[]{0, 0, -1, 0, 0},
        //        new float[]{0, 0, 0, 1, 0},
        //        new float[]{1, 1, 1, 0, 1}
        //    });

        //    return ApplyColorMatrix(sourceImage, colorMatrix);
        //}
    }
}
