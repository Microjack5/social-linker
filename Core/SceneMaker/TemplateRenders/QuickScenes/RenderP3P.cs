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
using System.Net;
using Newtonsoft.Json;
using System.Collections.Generic;
using SocialLinker.Core.SceneMaker.Data.Bustup;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using SocialLinker.Core.Menus;

namespace SocialLinker.Core.SceneMaker.TemplateRenders.QuickScenes
{
    public class RenderP3P
    {
        int template_width = 480;
        int template_height = 272;

        System.Drawing.Color color_hud_bg_blue = System.Drawing.Color.FromArgb(97, 174, 253);
        System.Drawing.Color color_date_blue = System.Drawing.Color.FromArgb(35, 97, 133);
        System.Drawing.Color color_tod_blue = System.Drawing.Color.FromArgb(65, 125, 173);
        System.Drawing.Color color_countdown_blue = System.Drawing.Color.FromArgb(192, 255, 255);

        System.Drawing.Color color_hud_bg_pink = System.Drawing.Color.FromArgb(251, 152, 180);
        System.Drawing.Color color_date_pink = System.Drawing.Color.FromArgb(150, 42, 57);
        System.Drawing.Color color_tod_pink = System.Drawing.Color.FromArgb(143, 44, 64);
        System.Drawing.Color color_countdown_pink = System.Drawing.Color.FromArgb(245, 195, 222);

        System.Drawing.Color color_hud_bg_green = System.Drawing.Color.FromArgb(58, 168, 97);
        System.Drawing.Color color_date_green = System.Drawing.Color.FromArgb(7, 40, 10);
        System.Drawing.Color color_tod_green = System.Drawing.Color.FromArgb(7, 40, 10);
        System.Drawing.Color color_countdown_green = System.Drawing.Color.FromArgb(121, 255, 141);

        System.Drawing.Color color_saturday_blue = System.Drawing.Color.FromArgb(58, 15, 104);
        System.Drawing.Color color_sunday_red = System.Drawing.Color.FromArgb(123, 27, 55);

        System.Drawing.Color color_moon_yellow = System.Drawing.Color.FromArgb(183, 150, 81);

        int max_line_length = 360;
        int error_counter = 0;

        public async Task Render_Quick_Scene_P3P(SocialLinkerCommand sl_command, OfficialSetData set_data, MakerCommandData command_data)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = sl_command.User;
            SocketTextChannel channel = (SocketTextChannel)sl_command.Channel;

            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            // Send a loading message to the channel while the sprite sheet is being made.
            RestUserMessage loader = await channel.SendMessageAsync("", false, P3P_Loading_Message(account).Build());

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

            if (bustup == null)
            {
                await loader.DeleteAsync();
                return;
            }

            // Time to put it all together!
            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                try
                {
                    // Create and assign bitmap variables for the assets needed.
                    Bitmap message_window = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3P//Main//message_window.png");
                    Bitmap cursor = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3P//Main//cursor.png");

                    // Draw the layer with the user's colored default background if it exists.
                    graphics.DrawImage(colored_background_bitmap, 0, 0, template_width, template_height);

                    // Draw the user's background to the base template.
                    graphics.DrawImage(background, 0, 0, template_width, template_height);

                    // Draw the character bust-up to the template if the base sprite number is not '0'.
                    if (command_data.Base_Sprite != 0)
                    {
                        Bitmap placed_bustup = Set_Bustup_Placement(account, bustup, bustup_data, set_data);
                        graphics.DrawImage(placed_bustup, 0, 0, template_width, template_height);
                    }

                    // Draw the message window layer to the base template.
                    message_window = Tint_Message_Window(message_window);
                    graphics.DrawImage(message_window, 0, 0, template_width, template_height);

                    // Draw the cursor layer to the base template.
                    cursor = Color_Cursor(cursor, account.P3P_TS_Color);
                    graphics.DrawImage(cursor, 0, 0, template_width, template_height);

                    // If the user has the HUD enabled, render it to the template as well.
                    if (account.P3P_TS_HUD != "None")
                    {
                        graphics.DrawImage(Render_Calendar_HUD(account), 0, 0, template_width, template_height);
                        graphics.DrawImage(Render_Moon_HUD(account), 0, 0, template_width, template_height);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            // Create another graphics object for the base template.
            // We'll start rendering our needed text here.
            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                System.Drawing.Color name_dark_blue = System.Drawing.Color.FromArgb(29, 0, 92);
                Rectangle name_area = new Rectangle(0, 190, 480, 30);

                string display_name = OfficialSetMethods.GetDisplayName(account, command_data, set_data, bustup_data);
                display_name = OfficialSetMethods.Validate_Input(sl_command, "P3P", "Name", display_name);

                Bitmap rendered_name = Render_Name(display_name);
                Bitmap colored_rendered_name = Bitmap_To_Color(rendered_name, name_dark_blue, name_area);
                graphics.DrawImage(colored_rendered_name, 0, 0, template_width, template_height);

                System.Drawing.Color dialogue_gray = System.Drawing.Color.FromArgb(72, 72, 72);
                Rectangle dialogue_area = new Rectangle(0, 190, 480, 82);

                command_data.Dialogue = OfficialSetMethods.Validate_Input(sl_command, "P3P", "Dialogue", command_data.Dialogue);
                List<string>[] parsed_lines = OfficialSetMethods.Line_Parser(sl_command, "P3P", command_data.Dialogue, 3, max_line_length);

                Bitmap rendered_dialogue = Render_Dialogue(parsed_lines);
                Bitmap colored_dialogue = Bitmap_To_Color(rendered_dialogue, dialogue_gray, dialogue_area);

                // Draw the input dialogue to the template.
                graphics.DrawImage(colored_dialogue, 0, 0, template_width, template_height);
            }

            base_template = Scale_Template(account, base_template);

            if (error_counter > 0)
            {
                _ = ErrorHandling.API_Timeout(sl_command);
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

            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            // Send a loading message to the channel while the sprite sheet is being made.
            RestUserMessage loader = await channel.SendMessageAsync("", false, P3P_Loading_Message(account).Build());

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
                try
                {
                    // Create and assign bitmap variables for the assets needed.
                    Bitmap message_window = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3P//Main//message_window.png");
                    Bitmap cursor = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3P//Main//cursor.png");

                    // Draw the layer with the user's colored default background if it exists.
                    graphics.DrawImage(colored_background_bitmap, 0, 0, template_width, template_height);

                    // Draw the user's background to the base template.
                    graphics.DrawImage(background, 0, 0, template_width, template_height);

                    // Draw the message window layer to the base template.
                    message_window = Tint_Message_Window(message_window);
                    graphics.DrawImage(message_window, 0, 0, template_width, template_height);

                    // Draw the cursor layer to the base template.
                    cursor = Color_Cursor(cursor, account.P3P_TS_Color);
                    graphics.DrawImage(cursor, 0, 0, template_width, template_height);

                    // If the user has the HUD enabled, render it to the template as well.
                    if (account.P3P_TS_HUD != "None")
                    {
                        graphics.DrawImage(Render_Calendar_HUD(account), 0, 0, template_width, template_height);
                        graphics.DrawImage(Render_Moon_HUD(account), 0, 0, template_width, template_height);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            // Create another graphics object for the base template.
            // We'll start rendering our needed text here.
            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                System.Drawing.Color name_dark_blue = System.Drawing.Color.FromArgb(29, 0, 92);
                Rectangle name_area = new Rectangle(0, 190, 480, 30);

                System.Drawing.Color dialogue_gray = System.Drawing.Color.FromArgb(72, 72, 72);
                Rectangle dialogue_area = new Rectangle(0, 190, 480, 82);

                command_data.Dialogue = OfficialSetMethods.Validate_Input(sl_command, "P3P", "Dialogue", command_data.Dialogue);
                List<string>[] parsed_lines = OfficialSetMethods.Line_Parser(sl_command, "P3P", command_data.Dialogue, 3, max_line_length);

                Bitmap rendered_dialogue = Render_Dialogue(parsed_lines);
                Bitmap colored_dialogue = Bitmap_To_Color(rendered_dialogue, dialogue_gray, dialogue_area);

                // Draw the input dialogue to the template.
                graphics.DrawImage(colored_dialogue, 0, 0, template_width, template_height);
            }

            base_template = Scale_Template(account, base_template);

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

        public Bitmap Set_Bustup_Placement(UserInfoFields account, Bitmap bustup, BustupData bustup_data, OfficialSetData set_data)
        {
            // Create a starting base bitmap to render all graphics on.
            Bitmap base_template = new Bitmap(template_width, template_height);

            // Switch the rendering position of the bustup depending on the user's settings.
            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                switch (account.P3P_TS_Position)
                {
                    case "Left":
                        graphics.DrawImage(bustup, bustup_data.P3P_Left_Coord_X, bustup_data.P3P_Left_Coord_Y, bustup_data.P3P_Scale_Width, bustup_data.P3P_Scale_Height);
                        break;

                    case "Right":
                        graphics.DrawImage(bustup, bustup_data.P3P_Right_Coord_X, bustup_data.P3P_Right_Coord_Y, bustup_data.P3P_Scale_Width, bustup_data.P3P_Scale_Height);
                        break;

                    case "Center":
                        graphics.DrawImage(bustup, bustup_data.P3P_Center_Coord_X, bustup_data.P3P_Center_Coord_Y, bustup_data.P3P_Scale_Width, bustup_data.P3P_Scale_Height);
                        break;
                }
            }

            return base_template;
        }

        public Bitmap Render_Name(string display_name)
        {
            Bitmap base_template = new Bitmap(template_width, template_height);

            // Establish an int for the width and height glyphs should be rendered at.
            int multiplier = 32;

            // Load the bitmap font.
            string font_sheet = $@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3P//Font//p3p_font_sheet.png";

            // Create a variable to iterate through sections of the bitmap font.
            Bitmap current_glyph;

            // Specify X and Y coordinates for where the glyphs should start rendering on the template.
            int render_position_x = 40;
            int render_position_y = 192;

            // Thake the sprite's display name and convert it into a char array.
            char[] char_array = display_name.ToCharArray();

            // Iterate through each character of the array.
            for (int i = 0; i < char_array.Length; i++)
            {
                // Retrieve glyph information from the JSON file.
                var glyph = ParsingMethods.Get_P3P_Glyph(char_array[i]);

                // Check if the character is a line break.
                if (char_array[i] == '\u000a')
                {
                    // Do nothing
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

                            //Draw the glyph to the base bitmap
                            graphics.DrawImage(current_glyph, (render_position_x - glyph.LeftCut), render_position_y, multiplier, multiplier);
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
                            render_position_x += -2;
                        }
                    }
                }
            }

            return base_template;
        }

        public Bitmap Render_Dialogue(List<string>[] dialogue_lines)
        {
            Bitmap bitmap = new Bitmap(template_width, template_height);

            // Create an int to keep track of rendering errors. This is neccessary to inform the user of any potential issues.
            int error_counter = 0;

            //Establish an int for the width and height glyphs should be rendered at
            int multiplier = 32;

            string font_sheet = $@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3P//Font//p3p_font_sheet.png";
            Bitmap current_glyph;

            for (int i = 0; i < dialogue_lines.Length; i++)
            {
                //Establish variables for where the glyphs should be rendered on the template
                int render_position_x = 52;
                int render_position_y = 208 + (15 * i);

                char[] char_array = String_List_To_String(dialogue_lines[i]).ToCharArray();

                for (int j = 0; j < char_array.Length; j++)
                {
                    //Retrieve glyph information from the JSON file
                    var glyph = ParsingMethods.Get_P3P_Glyph(char_array[j]);

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
                                //Copy the section of the bitmap font needed
                                Rectangle crop = new Rectangle(x, y, multiplier, multiplier);
                                current_glyph = originalImage.Clone(crop, originalImage.PixelFormat);

                                graphics.DrawImage(current_glyph, (render_position_x - glyph.LeftCut), render_position_y, multiplier, multiplier);
                            }
                        }

                        //Set the next X value at the end of the current glyph's right width
                        render_position_x += (glyph.RightCut - glyph.LeftCut);
                    }
                }
            }

            return bitmap;
        }

        public static int Measure_Word_Pixel_Length(SocialLinkerCommand sl_command, string input_word)
        {
            // Create an int variable to keep track of the pixel length of a word.
            int pixel_counter = 0;

            // Take the input string and convert it into a char array.
            char[] char_array = input_word.ToCharArray();

            // Here, we'll process the char array by iterating through each index.
            for (int i = 0; i < char_array.Length; i++)
            {
                // Retrieve glyph information from the P3F JSON file.
                var glyph = ParsingMethods.Get_P3P_Glyph(char_array[i]);

                // Confirm that the glyph taken in is catologued in the JSON. If not, the character is unsupported.
                if (glyph != null)
                {
                    // Check if the character is a line break. Strings with line breaks shouldn't make it to this method, but this is a failsafe just in case.
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
                                pixel_counter += -2;
                            }
                            else if (char_array[i] == 'T' && Char.IsLower(char_array[i + 1]) && char_array[i + 1] != 'h')
                            {
                                pixel_counter += -3;
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

        public Bitmap Render_Calendar_HUD(UserInfoFields account)
        {
            // Get the user's current time and store it in a variable.
            DateTime user_time = Get_Date(account);

            // Create a new bitmap with the width and height variables.
            Bitmap base_template = new Bitmap(template_width, template_height);

            // Now, let's use a graphics object to draw to the base template.
            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                // Establish all bitmap variables needed. Ones needed for the date and time of day will be initialized as new bitmaps and reassigned to later.
                Bitmap hud_top = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3P//Main//hud_1.png");
                Bitmap hud_bottom = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3P//Main//hud_2.png");

                Bitmap month_tens = new Bitmap(template_width, template_height);
                Bitmap month_ones = new Bitmap(template_width, template_height);

                Bitmap day_tens = new Bitmap(template_width, template_height);
                Bitmap day_ones = new Bitmap(template_width, template_height);

                Bitmap day_of_week = new Bitmap(template_width, template_height);
                Bitmap time_of_day = new Bitmap(template_width, template_height);
                Bitmap time_of_day_shadow = new Bitmap(template_width, template_height);
                Bitmap date_slash = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3P//Main//Calendar//slash.png");
                Bitmap date_dot = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3P//Main//Calendar//day_dot.png");

                Bitmap moon_phase_text = new Bitmap(template_width, template_height);
                Bitmap moon_phase_digit_tens = new Bitmap(template_width, template_height);
                Bitmap moon_phase_digit_ones = new Bitmap(template_width, template_height);
                Bitmap moon_phase_image_normal = new Bitmap(template_width, template_height);
                Bitmap moon_phase_image_glow = new Bitmap(template_width, template_height);

                // Get the user's current month and convert it to a char array.
                char[] month = user_time.ToString("MM").ToCharArray();

                // If the month is not a single digit, get the appropriate bitmap for the tens place of the month.
                if (month[0] != '0')
                {
                    month_tens = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3P//Main//Calendar//Month//Tens_Place//{month[0]}.png");
                }

                // Regardless, get the appropriate bitmap for the ones place of the month.
                month_ones = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3P//Main//Calendar//Month//Ones_Place//{month[1]}.png");

                // Get the user's current day and convert it to a char array.
                char[] day = user_time.ToString("dd").ToCharArray();

                // If the day is not a single digit, get the appropriate bitmap for the tens place of the day.
                if (day[0] != '0')
                {
                    day_tens = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3P//Main//Calendar//Day//Tens_Place//{day[0]}.png");
                }

                // Regardless, get the appropriate bitmap for the ones place of the day.
                day_ones = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3P//Main//Calendar//Day//Ones_Place//{day[1]}.png");

                // Get the appropriate bitmaps for the weekday and time of day for the user.
                day_of_week = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3P//Main//Calendar//Day_of_Week//{user_time.ToString("dddd").ToLower()}.png");
                time_of_day = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3P//Main//Calendar//Time_of_Day//{Get_Time_of_Day(user_time)}.png");

                Rectangle hud_area = new Rectangle(331, 0, 149, 48);
                Rectangle calendar_area = new Rectangle(365, 1, 111, 35);

                // Color the assets depending on whether the time is the "Dark Hour" for the user or not.
                if (Get_Time_of_Day(user_time) == "dark_hour")
                {
                    hud_top = Bitmap_To_Color(hud_top, color_hud_bg_green, hud_area);
                    month_tens = Bitmap_To_Color(month_tens, color_date_green, calendar_area);
                    month_ones = Bitmap_To_Color(month_ones, color_date_green, calendar_area);
                    date_slash = Bitmap_To_Color(date_slash, color_date_green, calendar_area);
                    day_tens = Bitmap_To_Color(day_tens, color_date_green, calendar_area);
                    day_ones = Bitmap_To_Color(day_ones, color_date_green, calendar_area);
                    date_dot = Bitmap_To_Color(date_dot, color_date_green, calendar_area);
                    day_of_week = Bitmap_To_Color(day_of_week, color_date_green, calendar_area);
                    time_of_day_shadow = Bitmap_To_Color(time_of_day, color_tod_green, calendar_area);
                }
                else
                {
                    switch (account.P3P_TS_Color)
                    {
                        case "Male Protagonist":
                            hud_top = Bitmap_To_Color(hud_top, color_hud_bg_blue, hud_area);
                            month_tens = Bitmap_To_Color(month_tens, color_date_blue, calendar_area);
                            month_ones = Bitmap_To_Color(month_ones, color_date_blue, calendar_area);
                            date_slash = Bitmap_To_Color(date_slash, color_date_blue, calendar_area);
                            day_tens = Bitmap_To_Color(day_tens, color_date_blue, calendar_area);
                            day_ones = Bitmap_To_Color(day_ones, color_date_blue, calendar_area);
                            date_dot = Bitmap_To_Color(date_dot, color_date_blue, calendar_area);
                            time_of_day_shadow = Bitmap_To_Color(time_of_day, color_tod_blue, calendar_area);

                            if (OfficialSetMethods.Is_Holiday(user_time))
                            {
                                day_of_week = Bitmap_To_Color(day_of_week, color_date_blue, calendar_area);
                            }
                            else if (user_time.ToString("dddd").ToLower() == "saturday")
                            {
                                day_of_week = Bitmap_To_Color(day_of_week, color_saturday_blue, calendar_area);
                            }
                            else if (user_time.ToString("dddd").ToLower() == "sunday")
                            {
                                day_of_week = Bitmap_To_Color(day_of_week, color_sunday_red, calendar_area);
                            }
                            else
                            {
                                day_of_week = Bitmap_To_Color(day_of_week, color_date_blue, calendar_area);
                            }
                            break;

                        case "Female Protagonist":
                            hud_top = Bitmap_To_Color(hud_top, color_hud_bg_pink, hud_area);
                            month_tens = Bitmap_To_Color(month_tens, color_date_pink, calendar_area);
                            month_ones = Bitmap_To_Color(month_ones, color_date_pink, calendar_area);
                            date_slash = Bitmap_To_Color(date_slash, color_date_pink, calendar_area);
                            day_tens = Bitmap_To_Color(day_tens, color_date_pink, calendar_area);
                            day_ones = Bitmap_To_Color(day_ones, color_date_pink, calendar_area);
                            date_dot = Bitmap_To_Color(date_dot, color_date_pink, calendar_area);
                            time_of_day_shadow = Bitmap_To_Color(time_of_day, color_tod_pink, calendar_area);

                            // Color the day of week bitmap depending on what day it currently is.
                            if (OfficialSetMethods.Is_Holiday(user_time))
                            {
                                day_of_week = Bitmap_To_Color(day_of_week, color_date_pink, calendar_area);
                            }
                            else if (user_time.ToString("dddd").ToLower() == "saturday")
                            {
                                day_of_week = Bitmap_To_Color(day_of_week, color_date_pink, calendar_area);
                            }
                            else if (user_time.ToString("dddd").ToLower() == "sunday")
                            {
                                day_of_week = Bitmap_To_Color(day_of_week, color_sunday_red, calendar_area);
                            }
                            else
                            {
                                day_of_week = Bitmap_To_Color(day_of_week, color_date_pink, calendar_area);
                            }
                            break;

                        default:
                            hud_top = Bitmap_To_Color(hud_top, color_hud_bg_blue, hud_area);
                            month_tens = Bitmap_To_Color(month_tens, color_date_blue, calendar_area);
                            month_ones = Bitmap_To_Color(month_ones, color_date_blue, calendar_area);
                            date_slash = Bitmap_To_Color(date_slash, color_date_blue, calendar_area);
                            day_tens = Bitmap_To_Color(day_tens, color_date_blue, calendar_area);
                            day_ones = Bitmap_To_Color(day_ones, color_date_blue, calendar_area);
                            date_dot = Bitmap_To_Color(date_dot, color_date_blue, calendar_area);

                            // Color the day of week bitmap depending on what day it currently is.
                            if (OfficialSetMethods.Is_Holiday(user_time))
                            {
                                day_of_week = Bitmap_To_Color(day_of_week, color_date_blue, calendar_area);
                            }
                            if (user_time.ToString("dddd").ToLower() == "saturday")
                            {
                                day_of_week = Bitmap_To_Color(day_of_week, color_saturday_blue, calendar_area);
                            }
                            else if (user_time.ToString("dddd").ToLower() == "sunday")
                            {
                                day_of_week = Bitmap_To_Color(day_of_week, color_sunday_red, calendar_area);
                            }
                            else
                            {
                                day_of_week = Bitmap_To_Color(day_of_week, color_date_blue, calendar_area);
                            }
                            break;
                    }
                }

                // Draw all the assets to the template.
                graphics.DrawImage(hud_top, 0, 0, template_width, template_height);
                graphics.DrawImage(hud_bottom, 0, 0, template_width, template_height);

                graphics.DrawImage(month_tens, 0, 0, template_width, template_height);
                graphics.DrawImage(month_ones, 0, 0, template_width, template_height);
                graphics.DrawImage(date_slash, 0, 0, template_width, template_height);
                graphics.DrawImage(day_tens, 0, 0, template_width, template_height);
                graphics.DrawImage(day_ones, 0, 0, template_width, template_height);
                graphics.DrawImage(date_dot, 0, 0, template_width, template_height);

                graphics.DrawImage(day_of_week, 0, 0, template_width, template_height);
                
                graphics.DrawImage(time_of_day_shadow, 2, 2, template_width, template_height);
                graphics.DrawImage(time_of_day, 0, 0, template_width, template_height);
            }

            return base_template;
        }

        public Bitmap Render_Moon_HUD(UserInfoFields account)
        {
            // Create new bitmap variables for the assets we'll need throughout the method.
            // We'll assign them proper values soon depending on the moon phase.
            // For the countdown text, create and initialize two. One will be a mainstay while the other only appears during new and half moons.
            Bitmap countdown_text = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3P//Main//Moon//Countdown//Text//next.png");

            Bitmap countdown_tens = new Bitmap(template_width, template_height);
            Bitmap countdown_ones = new Bitmap(template_width, template_height);

            Bitmap countdown_slash = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3P//Main//Moon//Countdown//slash.png");

            Bitmap moon_background = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3P//Main//Moon//moon_background.png");
            Bitmap moon_phase = new Bitmap(template_width, template_height);
            Bitmap moon_phase_glow = new Bitmap(template_width, template_height);

            // Create a variable to store the moon phase result and initialize it to null.
            Moon.PhaseResult result = null;

            // Determine whether the user's set location is in the northern or southern hemisphere.
            if (Get_Hemisphere(account) == "Northern")
            {
                result = Moon.Now(Earth.Hemispheres.Northern);
            }
            else if (Get_Hemisphere(account) == "Southern")
            {
                result = Moon.Now(Earth.Hemispheres.Southern);
            }

            // Create a variable for the current cycle's age.
            double cycle_age = result.DaysIntoCycle;

            // Using that age, determine how many days are left until the next full moon.
            int full_moon_countdown = Get_Full_Moon_Countdown(cycle_age);

            // Store the moon's illumination percentage in a double. We'll use this to determine what phase it's currently in alongside using the age.
            double illumination = Math.Round(result.Visibility, 2);

            // Create a new bitmap with the width and height variables.
            Bitmap base_template = new Bitmap(template_width, template_height);

            // Now, let's use a graphics object to draw to the base template.
            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                // Convert the full moon countdown value to a two-index char array.
                char[] countdown_array = full_moon_countdown.ToString("00").ToCharArray();

                // Check if the first index is not a zero. If it is, the countdown digit is a single number and we can ignore the tens place.
                // Else, we need to assign a proper value to the tens place bitmap variable.
                if (countdown_array[0] != '0')
                {
                    countdown_tens = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3P//Main//Moon//Countdown//Digits//Tens_Place//{countdown_array[0]}.png");
                }
                // There will always be a digit in the ones place unless the moon is full, so assign a proper value here too.
                countdown_ones = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3P//Main//Moon//Countdown//Digits//Ones_Place//{countdown_array[1]}.png");

                // Displayed moon phases have a dark background, so assign that value here.
                moon_background = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3P//Main//Moon//moon_background.png");

                // Here is where the calculation on which moon phase to display begins.
                // The cycle begins with a new moon, so we'll use the current cycle's age and divide it into two halfs to determine whether it's waxing or waning.
                // Waxing phases
                if (cycle_age <= 14.76)
                {
                    // New moon
                    if ((illumination >= 0) && (illumination < 6.25))
                    {
                        moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3P//Main//Moon//Phases//1_new.png");
                        countdown_text = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3P//Main//Moon//Countdown//Text//new.png");
                        countdown_slash = new Bitmap(template_width, template_height);
                        countdown_tens = new Bitmap(template_width, template_height);
                        countdown_ones = new Bitmap(template_width, template_height);
                    }
                    // Waxing crescent 1
                    else if ((illumination >= 6.25) && (illumination < 12.5))
                    {
                        moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3P//Main//Moon//Phases//2_waxing_crescent.png");
                        moon_phase_glow = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3P//Main//Moon//Phases//Glow//2_waxing_crescent.png");
                    }
                    // Waxing crescent 2
                    else if ((illumination >= 12.5) && (illumination < 18.75))
                    {
                        moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3P//Main//Moon//Phases//3_waxing_crescent.png");
                        moon_phase_glow = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3P//Main//Moon//Phases//Glow//3_waxing_crescent.png");
                    }
                    // Waxing crescent 3
                    else if ((illumination >= 18.75) && (illumination < 25))
                    {
                        moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3P//Main//Moon//Phases//4_waxing_crescent.png");
                        moon_phase_glow = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3P//Main//Moon//Phases//Glow//4_waxing_crescent.png");
                    }
                    // Waxing crescent 4
                    else if ((illumination >= 25) && (illumination < 31.25))
                    {
                        moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3P//Main//Moon//Phases//5_waxing_crescent.png");
                        moon_phase_glow = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3P//Main//Moon//Phases//Glow//5_waxing_crescent.png");
                    }
                    // Waxing crescent 5
                    else if ((illumination >= 31.25) && (illumination < 37.5))
                    {
                        moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3P//Main//Moon//Phases//6_waxing_crescent.png");
                        moon_phase_glow = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3P//Main//Moon//Phases//Glow//6_waxing_crescent.png");
                    }
                    // Waxing crescent 6
                    else if ((illumination >= 37.5) && (illumination < 43.75))
                    {
                        moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3P//Main//Moon//Phases//7_waxing_crescent.png");
                        moon_phase_glow = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3P//Main//Moon//Phases//Glow//7_waxing_crescent.png");
                    }
                    // Waxing half
                    else if ((illumination >= 43.75) && (illumination < 50))
                    {
                        moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3P//Main//Moon//Phases//8_waxing_half.png");
                        moon_phase_glow = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3P//Main//Moon//Phases//Glow//8_waxing_half.png");
                        countdown_text = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3P//Main//Moon//Countdown//Text//half.png");
                        countdown_slash = new Bitmap(template_width, template_height);
                        countdown_tens = new Bitmap(template_width, template_height);
                        countdown_ones = new Bitmap(template_width, template_height);
                    }
                    // Waxing half
                    else if ((illumination >= 50) && (illumination < 55))
                    {
                        moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3P//Main//Moon//Phases//8_waxing_half.png");
                        moon_phase_glow = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3P//Main//Moon//Phases//Glow//8_waxing_half.png");
                        countdown_text = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3P//Main//Moon//Countdown//Text//half.png");
                        countdown_slash = new Bitmap(template_width, template_height);
                        countdown_tens = new Bitmap(template_width, template_height);
                        countdown_ones = new Bitmap(template_width, template_height);
                    }
                    // Waxing gibbous 1
                    else if ((illumination >= 55) && (illumination < 60))
                    {
                        moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3P//Main//Moon//Phases//9_waxing_gibbous.png");
                        moon_phase_glow = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3P//Main//Moon//Phases//Glow//9_waxing_gibbous.png");
                    }
                    // Waxing gibbous 2
                    else if ((illumination >= 60) && (illumination < 65))
                    {
                        moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3P//Main//Moon//Phases//10_waxing_gibbous.png");
                        moon_phase_glow = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3P//Main//Moon//Phases//Glow//10_waxing_gibbous.png");
                    }
                    // Waxing gibbous 3
                    else if ((illumination >= 65) && (illumination < 70))
                    {
                        moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3P//Main//Moon//Phases//11_waxing_gibbous.png");
                        moon_phase_glow = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3P//Main//Moon//Phases//Glow//11_waxing_gibbous.png");
                    }
                    // Waxing gibbous 4
                    else if ((illumination >= 70) && (illumination < 75))
                    {
                        moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3P//Main//Moon//Phases//12_waxing_gibbous.png");
                        moon_phase_glow = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3P//Main//Moon//Phases//Glow//12_waxing_gibbous.png");
                    }
                    // Waxing gibbous 5
                    else if ((illumination >= 75) && (illumination < 80))
                    {
                        moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3P//Main//Moon//Phases//13_waxing_gibbous.png");
                        moon_phase_glow = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3P//Main//Moon//Phases//Glow//13_waxing_gibbous.png");
                    }
                    // Waxing gibbous 6
                    else if ((illumination >= 80) && (illumination < 85))
                    {
                        moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3P//Main//Moon//Phases//14_waxing_gibbous.png");
                        moon_phase_glow = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3P//Main//Moon//Phases//Glow//14_waxing_gibbous.png");
                    }
                    // Waxing gibbous 7
                    else if ((illumination >= 85) && (illumination < 90))
                    {
                        moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3P//Main//Moon//Phases//15_waxing_gibbous.png");
                        moon_phase_glow = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3P//Main//Moon//Phases//Glow//15_waxing_gibbous.png");
                    }
                    // Waxing gibbous 8
                    else if ((illumination >= 90) && (illumination < 95))
                    {
                        moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3P//Main//Moon//Phases//16_waxing_gibbous.png");
                        moon_phase_glow = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3P//Main//Moon//Phases//Glow//16_waxing_gibbous.png");
                    }
                    // Full moon
                    else if ((illumination >= 95) && (illumination < 100))
                    {
                        moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3P//Main//Moon//Phases//17_full.png");
                        moon_phase_glow = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3P//Main//Moon//Phases//Glow//17_full.png");
                        countdown_text = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3P//Main//Moon//Countdown//Text//full.png");
                        countdown_slash = new Bitmap(template_width, template_height);
                        countdown_tens = new Bitmap(template_width, template_height);
                        countdown_ones = new Bitmap(template_width, template_height);
                    }
                    // Full moon
                    else if (illumination >= 100)
                    {
                        moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3P//Main//Moon//Phases//17_full.png");
                        moon_phase_glow = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3P//Main//Moon//Phases//Glow//17_full.png");
                        countdown_text = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3P//Main//Moon//Countdown//Text//full.png");
                        countdown_slash = new Bitmap(template_width, template_height);
                        countdown_tens = new Bitmap(template_width, template_height);
                        countdown_ones = new Bitmap(template_width, template_height);
                    }
                }

                // Waning phases
                else if (cycle_age > 14.76)
                {
                    // Full moon
                    if (illumination >= 100)
                    {
                        moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3P//Main//Moon//Phases//17_full.png");
                        moon_phase_glow = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3P//Main//Moon//Phases//Glow//17_full.png");
                        countdown_text = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3P//Main//Moon//Countdown//Text//full.png");
                        countdown_slash = new Bitmap(template_width, template_height);
                        countdown_tens = new Bitmap(template_width, template_height);
                        countdown_ones = new Bitmap(template_width, template_height);
                    }
                    // Full moon
                    else if ((illumination >= 95) && (illumination < 100))
                    {
                        moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3P//Main//Moon//Phases//17_full.png");
                        moon_phase_glow = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3P//Main//Moon//Phases//Glow//17_full.png");
                        countdown_text = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3P//Main//Moon//Countdown//Text//full.png");
                        countdown_slash = new Bitmap(template_width, template_height);
                        countdown_tens = new Bitmap(template_width, template_height);
                        countdown_ones = new Bitmap(template_width, template_height);
                    }
                    // Waning gibbous 1
                    else if ((illumination >= 90) && (illumination < 95))
                    {
                        moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3P//Main//Moon//Phases//18_waning_gibbous.png");
                    }
                    // Waning gibbous 2
                    else if ((illumination >= 85) && (illumination < 90))
                    {
                        moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3P//Main//Moon//Phases//19_waning_gibbous.png");
                    }
                    // Waning gibbous 3
                    else if ((illumination >= 80) && (illumination < 85))
                    {
                        moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3P//Main//Moon//Phases//20_waning_gibbous.png");
                    }
                    // Waning gibbous 4
                    else if ((illumination >= 75) && (illumination < 80))
                    {
                        moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3P//Main//Moon//Phases//21_waning_gibbous.png");
                    }
                    // Waning gibbous 5
                    else if ((illumination >= 70) && (illumination < 75))
                    {
                        moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3P//Main//Moon//Phases//22_waning_gibbous.png");
                    }
                    // Waning gibbous 6
                    else if ((illumination >= 65) && (illumination < 70))
                    {
                        moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3P//Main//Moon//Phases//23_waning_gibbous.png");
                    }
                    // Waning gibbous 7
                    else if ((illumination >= 60) && (illumination < 65))
                    {
                        moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3P//Main//Moon//Phases//24_waning_gibbous.png");
                    }
                    // Waning gibbous 8
                    else if ((illumination >= 55) && (illumination < 60))
                    {
                        moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3P//Main//Moon//Phases//25_waning_gibbous.png");
                    }
                    // Waning half
                    else if ((illumination >= 50) && (illumination < 55))
                    {
                        moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3P//Main//Moon//Phases//26_waning_half.png");
                        countdown_text = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3P//Main//Moon//Countdown//Text//half.png");
                        countdown_slash = new Bitmap(template_width, template_height);
                        countdown_tens = new Bitmap(template_width, template_height);
                        countdown_ones = new Bitmap(template_width, template_height);
                    }
                    // Waning half
                    else if ((illumination >= 43.75) && (illumination < 50))
                    {
                        moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3P//Main//Moon//Phases//26_waning_half.png");
                        countdown_text = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3P//Main//Moon//Countdown//Text//half.png");
                        countdown_slash = new Bitmap(template_width, template_height);
                        countdown_tens = new Bitmap(template_width, template_height);
                        countdown_ones = new Bitmap(template_width, template_height);
                    }
                    // Waning crescent 2
                    else if ((illumination >= 37.5) && (illumination < 43.75))
                    {
                        moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3P//Main//Moon//Phases//27_waning_crescent.png");
                    }
                    // Waning crescent 3
                    else if ((illumination >= 31.25) && (illumination < 37.5))
                    {
                        moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3P//Main//Moon//Phases//28_waning_crescent.png");
                    }
                    // Waning crescent 4
                    else if ((illumination >= 25) && (illumination < 31.25))
                    {
                        moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3P//Main//Moon//Phases//29_waning_crescent.png");
                    }
                    // Waning crescent 5
                    else if ((illumination >= 18.75) && (illumination < 25))
                    {
                        moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3P//Main//Moon//Phases//30_waning_crescent.png");
                    }
                    // Waning crescent 6
                    else if ((illumination >= 12.5) && (illumination < 18.75))
                    {
                        moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3P//Main//Moon//Phases//31_waning_crescent.png");
                    }
                    // Waning crescent 7
                    else if ((illumination >= 6.25) && (illumination < 12.5))
                    {
                        moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3P//Main//Moon//Phases//32_waning_crescent.png");
                    }
                    // New moon
                    else if ((illumination >= 0) && (illumination < 6.25))
                    {
                        moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3P//Main//Moon//Phases//1_new.png");
                        countdown_text = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3P//Main//Moon//Countdown//Text//new.png");
                        countdown_slash = new Bitmap(template_width, template_height);
                        countdown_tens = new Bitmap(template_width, template_height);
                        countdown_ones = new Bitmap(template_width, template_height);
                    }
                }

                Rectangle countdown_area = new Rectangle(410, 48, 28, 14);
                Rectangle phase_area = new Rectangle(456, 49, 19, 19);

                // Depending on the time of day, color the HUD either blue or green.
                if (Get_Time_of_Day(Get_Date(account)) == "dark_hour")
                {
                    countdown_tens = Bitmap_To_Color(countdown_tens, color_countdown_green, countdown_area);
                    countdown_ones = Bitmap_To_Color(countdown_ones, color_countdown_green, countdown_area);
                }
                else
                {
                    switch (account.P3P_TS_Color)
                    {
                        case "Male Protagonist":
                            countdown_tens = Bitmap_To_Color(countdown_tens, color_countdown_blue, countdown_area);
                            countdown_ones = Bitmap_To_Color(countdown_ones, color_countdown_blue, countdown_area);
                            break;

                        case "Female Protagonist":
                            countdown_tens = Bitmap_To_Color(countdown_tens, color_countdown_pink, countdown_area);
                            countdown_ones = Bitmap_To_Color(countdown_ones, color_countdown_pink, countdown_area);
                            break;

                        default:
                            countdown_tens = Bitmap_To_Color(countdown_tens, color_countdown_blue, countdown_area);
                            countdown_ones = Bitmap_To_Color(countdown_ones, color_countdown_blue, countdown_area);
                            break;
                    }
                }

                // Lastly, we'll want to adjust the glow of waxing moon phases if an asset has been assigned to it.
                // Create a random variable.
                Random rnd = new Random();

                // Increase the brightness and contrast of the glowing bitmap.
                moon_phase_glow = Increase_Brightness_Contrast(moon_phase_glow);

                // Use the random variable to randomize the opacity of the glow.
                moon_phase_glow = SetImageOpacity(moon_phase_glow, (float)rnd.NextDouble());

                moon_phase = Bitmap_To_Color(moon_phase, color_moon_yellow, phase_area);

                countdown_text = SetImageOpacity(countdown_text, (float)0.5);
                countdown_slash = SetImageOpacity(countdown_slash, (float)0.5);

                // Draw all the assets to the template.
                if (account.P3P_TS_HUD != "Countdown Off")
                {
                    graphics.DrawImage(countdown_text, 0, 0, template_width, template_height);
                    graphics.DrawImage(countdown_tens, 0, 0, template_width, template_height);
                    graphics.DrawImage(countdown_ones, 0, 0, template_width, template_height);
                    graphics.DrawImage(countdown_slash, 0, 0, template_width, template_height);
                }
                graphics.DrawImage(moon_background, 0, 0, template_width, template_height);
                graphics.DrawImage(moon_phase, 0, 0, template_width, template_height);
                graphics.DrawImage(moon_phase_glow, 0, 0, template_width, template_height);
            }

            return base_template;
        }

        public static Bitmap Bitmap_To_Color(Bitmap input_bitmap, System.Drawing.Color input_color, Rectangle edit_area)
        {
            Bitmap base_bitmap = new Bitmap(input_bitmap.Width, input_bitmap.Height);

            for (int x = edit_area.X; x < edit_area.Right; x++)
            {
                for (int y = edit_area.Y; y < edit_area.Bottom; y++)
                {
                    System.Drawing.Color original_color = input_bitmap.GetPixel(x, y);
                    System.Drawing.Color new_color = System.Drawing.Color.FromArgb(original_color.A, input_color.R, input_color.G, input_color.B);

                    base_bitmap.SetPixel(x, y, new_color);
                }
            }

            return base_bitmap;
        }

        public static Bitmap Tint_Message_Window(Bitmap input_bitmap)
        {
            // Create a color variable. We'll use this to iterate through the input bitmap and store each pixel's color values here.
            System.Drawing.Color original_color;

            // Make an empty bitmap the same size as the input bitmap.
            Bitmap new_bitmap = new Bitmap(input_bitmap.Width, input_bitmap.Height);

            // Create a for loop to iterate over the X values of the bitmap to be changed.
            for (int x = 0; x < 480; x++)
            {
                // Create a nested for loop to iterate over the Y values of the bitmap to be changed.
                for (int y = 190; y < 272; y++)
                {
                    // Get the current pixel from the input bitmap.
                    original_color = input_bitmap.GetPixel(x, y);

                    int new_b_value = original_color.B - 27;

                    if (new_b_value < 0)
                    {
                        new_b_value = 0;
                    }

                    System.Drawing.Color new_color = System.Drawing.Color.FromArgb(original_color.A, original_color.R, original_color.G, new_b_value);
                    new_bitmap.SetPixel(x, y, new_color);
                }
            }

            return new_bitmap;
        }

        public static Bitmap Color_Cursor(Bitmap input_bitmap, string user_setting)
        {
            // Create a color variable. We'll use this to iterate through the input bitmap and store each pixel's color values here.
            System.Drawing.Color actual_color;

            // Make an empty bitmap the same size as the input bitmap.
            Bitmap new_bitmap = new Bitmap(input_bitmap.Width, input_bitmap.Height);

            System.Drawing.Color new_color = default;

            // Create a for loop to iterate over the X values of the bitmap to be changed.
            for (int i = 425; i < 445; i++)
            {
                // Create a nested for loop to iterate over the Y values of the bitmap to be changed.
                for (int j = 240; j < 265; j++)
                {
                    // Get the current pixel from the input bitmap.
                    actual_color = input_bitmap.GetPixel(i, j);

                    // Color in the pixel with the new color while keeping its current alpha value.
                    if (user_setting == "Male Protagonist")
                    {
                        new_color = System.Drawing.Color.FromArgb(actual_color.A, 25, 143, 255);
                    }
                    else if (user_setting == "Female Protagonist")
                    {
                        new_color = System.Drawing.Color.FromArgb(actual_color.A, 255, 91, 167);
                    }

                    new_bitmap.SetPixel(i, j, new_color);
                }
            }

            return new_bitmap;
        }

        public DateTime Get_Date(UserInfoFields account)
        {
            // Create an empty string variable. This is where the API request will be stored.
            string json_result = "";

            // Encapsulate the API request in a try-catch block. If the user inputs an invalid parameter, an exception will be thrown.
            try
            {
                // Make an API request with the account key and user input as parameters.
                using (WebClient client = new WebClient())
                {
                    json_result = new TimedWebClient { Timeout = Global.API_Timeout }.DownloadString($"http://api.weatherapi.com/v1/current.json?key={WeatherAPIConfig.weather_api_account.accountKey}&q={account.City}");
                }

                // Deserialize the JSON object and store it in a variable.
                var dataObject = JsonConvert.DeserializeObject<dynamic>(json_result);

                // Read the localtime variable of the data object.
                DateTime user_time = dataObject.location.localtime;

                // Return the localtime variable.
                return user_time;
            }
            // If an exception is thrown, return a default value.
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                error_counter++;
                return DateTime.UtcNow;
            }
        }

        public string Get_Hemisphere(UserInfoFields account)
        {
            // Create an empty string variable. This is where the API request will be stored.
            string json_result = "";

            // Encapsulate the API request in a try-catch block. If the user inputs an invalid parameter, an exception will be thrown.
            try
            {
                // Make an API request with the account key and user input as parameters.
                using (WebClient client = new WebClient())
                {
                    json_result = new TimedWebClient { Timeout = Global.API_Timeout }.DownloadString($"http://api.weatherapi.com/v1/current.json?key={WeatherAPIConfig.weather_api_account.accountKey}&q={account.City}");
                }

                // Deserialize the JSON object and store it in a variable.
                var dataObject = JsonConvert.DeserializeObject<dynamic>(json_result);

                // Create a double that stores the user's latitude value.
                double user_latitude = dataObject.location.lat;

                // We'll also create an empty string that we'll store the user's hemisphere in shortly.
                string user_hemisphere = "";

                // Determine the user's hemisphere based on the latitude value.
                if (user_latitude > 0)
                {
                    user_hemisphere = "Northern";
                }
                else
                {
                    user_hemisphere = "Southern";
                }

                return user_hemisphere;
            }
            // If an exception is thrown, return a default value.
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                error_counter++;
                return "Northern";
            }
        }

        public static string Get_Time_of_Day(DateTime input_time)
        {
            // Get the current hour of the user.
            TimeSpan current_hour = input_time.TimeOfDay;

            // Create an empty string variable to store the returned time of day value later on.
            string time_of_day = "";

            // Lastly, establish the starting times for each time of day.
            TimeSpan early_morning = new TimeSpan(6, 0, 0);
            TimeSpan morning = new TimeSpan(8, 0, 0);
            TimeSpan lunchtime = new TimeSpan(12, 0, 0);
            TimeSpan afternoon = new TimeSpan(13, 0, 0);
            TimeSpan after_school = new TimeSpan(15, 0, 0);
            TimeSpan evening = new TimeSpan(18, 0, 0);
            TimeSpan late_night = new TimeSpan(22, 0, 0);
            TimeSpan before_midnight = new TimeSpan(23, 59, 59);
            TimeSpan dark_hour = new TimeSpan(0, 0, 0);
            TimeSpan after_midnight = new TimeSpan(1, 0, 0);

            // Now, let's find out the current value.
            // If the current hour is before 1AM and after or on 12AM, set the time to Dark Hour.
            if (current_hour < after_midnight && current_hour >= dark_hour)
            {
                time_of_day = "dark_hour";
            }
            // If the current hour is before 11:59PM and after or on 10PM, set the time to Late Night.
            else if (current_hour < before_midnight && current_hour >= late_night)
            {
                time_of_day = "late_night";
            }
            // If the current hour is before 10PM and after or on 6PM, set the time to Evening.
            else if (current_hour < late_night && current_hour >= evening)
            {
                time_of_day = "evening";
            }
            // If the current hour is before 6PM and after or on 3PM, set the time depending on the day.
            // If it's a Sunday or outside of a school term, set it to Daytime.
            // If it's a weekday, set it to After School.
            else if (current_hour < evening && current_hour >= after_school)
            {
                if (DateTime.Now.ToString("ddd") == "Sun" || !OfficialSetMethods.Is_School_Term(input_time))
                {
                    time_of_day = "daytime";
                }
                else
                {
                    time_of_day = "after_school";
                }
            }
            // If the current hour is before 3PM and after or on 1PM, set the time to Afternoon.
            else if (current_hour < after_school && current_hour >= afternoon)
            {
                time_of_day = "afternoon";
            }
            // If the current hour is before 1PM and after or on 12PM, set the time depending on the day.
            // If it's a Sunday or outside of a school term, set it to Daytime.
            // If it's a weekday, set it to Lunchtime.
            else if (current_hour < afternoon && current_hour >= lunchtime)
            {
                if ((DateTime.Now.ToString("ddd") == "Sun") || !OfficialSetMethods.Is_School_Term(input_time))
                {
                    time_of_day = "daytime";
                }
                else
                {
                    time_of_day = "lunchtime";
                }
            }
            // If the current hour is before 12PM and after or on 8AM, set the time to Morning.
            else if (current_hour < lunchtime && current_hour >= morning)
            {
                time_of_day = "morning";
            }
            // If the current hour is before 8AM and after or on 6AM, set the time to Early Morning.
            else if (current_hour < morning && current_hour >= early_morning)
            {
                time_of_day = "early_morning";
            }
            // If the current hour is before 6AM and after or on 1AM, set the time to Late Night.
            else if (current_hour < early_morning && current_hour >= after_midnight)
            {
                time_of_day = "late_night";
            }
            else
            {
                time_of_day = "null";
            }

            return time_of_day;
        }

        public static int Get_Full_Moon_Countdown(double age)
        {
            // Create a default return value. This is an unrealistic number for the countdown, but will not cause rendering issues if used.
            int countdownInt = 39;

            // Calculate how many days are left until the next full moon.
            // This is done by taking the day value of the cycle and seeing how many days are left until the next halfpoint is reached.
            if (age < 14.76)
            {
                age = 14.76 - age;
            }
            else if (age >= 14.76)
            {
                age = (29.53 + 14.76) - age;
            }

            // Round the answer to the nearest integer.
            countdownInt = (int)Math.Round(age);

            return countdownInt;
        }

        // Method from https://stackoverflow.com/questions/15408607/adjust-brightness-contrast-and-gamma-of-an-image
        public static Bitmap Increase_Brightness_Contrast(Bitmap input_bitmap)
        {
            Bitmap adjustedImage = new Bitmap(input_bitmap.Width, input_bitmap.Height);
            float brightness = 1.2f; // 1.2 times the brightness
            float contrast = 1.8f; // 1.8 times the contrast
            float gamma = 1.0f; // no change in gamma

            float adjustedBrightness = brightness - 1.0f;
            // create matrix that will brighten and contrast the image
            float[][] ptsArray =
            {
                new float[] {contrast, 0, 0, 0, 0}, // scale red
                new float[] {0, contrast, 0, 0, 0}, // scale green
                new float[] {0, 0, contrast, 0, 0}, // scale blue
                new float[] {0, 0, 0, 1.0f, 0}, // don't scale alpha
                new float[] {adjustedBrightness, adjustedBrightness, adjustedBrightness, 0, 1}
            };

            ImageAttributes imageAttributes = new ImageAttributes();
            imageAttributes.ClearColorMatrix();
            imageAttributes.SetColorMatrix(new ColorMatrix(ptsArray), ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
            imageAttributes.SetGamma(gamma, ColorAdjustType.Bitmap);
            Graphics g = Graphics.FromImage(adjustedImage);
            g.DrawImage(input_bitmap, new Rectangle(0, 0, adjustedImage.Width, adjustedImage.Height)
                , 0, 0, input_bitmap.Width, input_bitmap.Height,
                GraphicsUnit.Pixel, imageAttributes);

            return adjustedImage;
        }

        public static Bitmap SetImageOpacity(Bitmap input_bitmap, float opacity)
        {
            //create a Bitmap the size of the image provided  
            Bitmap base_template = new Bitmap(input_bitmap.Width, input_bitmap.Height);

            //create a graphics object from the image  
            using (Graphics graphics = Graphics.FromImage(base_template))
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
                graphics.DrawImage(input_bitmap, new Rectangle(0, 0, base_template.Width, base_template.Height), 0, 0, input_bitmap.Width, input_bitmap.Height, GraphicsUnit.Pixel, attributes);
            }
            return base_template;
        }

        public Bitmap Scale_Template(UserInfoFields account, Bitmap input_template)
        {
            var scaled_bitmap = new Bitmap(2, 2);
            int scaled_width = template_width;
            int scaled_height = template_height;

            if (account.P3P_Resolution == "320 × 240")
            {
                // Do nothing if setting is at default resolution
            }
            else
            {
                if (account.P3P_Resolution == "1440 × 1088")
                {
                    scaled_width = 1440;
                    scaled_height = 1088;
                }

                var copied_input = new Bitmap(input_template);
                scaled_bitmap = new Bitmap(scaled_width, scaled_height);

                using (Graphics graphics = Graphics.FromImage(scaled_bitmap))
                {
                    switch (account.P3P_Scale)
                    {
                        case "Bicubic":
                            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            break;

                        case "Nearest Neighbor":
                            graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                            break;
                    }

                    graphics.CompositingQuality = CompositingQuality.HighQuality;
                    graphics.DrawImage(copied_input, 0, 0, scaled_width, scaled_height);
                }

                input_template = scaled_bitmap;
            }

            return input_template;
        }

        public static EmbedBuilder P3P_Loading_Message(UserInfoFields account)
        {
            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Generating Scene...",
                IconUrl = EmbedSettings.Get_Game_Logo("P3P")
            };

            embed.WithAuthor(author);

            // Assign a color based on the user's color setting for the P3P template.
            embed.WithColor(EmbedSettings.Get_Game_Color("P3P", account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Loading_Icon("P3P", account));

            embed.WithDescription("This may take a few seconds!");

            return embed;
        }
    }
}
