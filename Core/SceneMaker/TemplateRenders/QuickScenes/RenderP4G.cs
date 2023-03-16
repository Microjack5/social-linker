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
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using System.Collections.Generic;
using SocialLinker.Core.SceneMaker.Data.Bustup;
using System.Globalization;
using SocialLinker.Core.Menus;

namespace SocialLinker.Core.SceneMaker.TemplateRenders.QuickScenes
{
    public class RenderP4G : ModuleBase<SocketCommandContext>
    {
        public static async Task Render_Quick_Scene_P4G(SocialLinkerCommand sl_command, OfficialSetData set_data, MakerCommandData command_data)
        {
            // Create variables to store the width and height of the template.
            int template_width = 1920;
            int template_height = 1080;

            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = sl_command.User;
            SocketTextChannel channel = (SocketTextChannel)sl_command.Channel;

            // Send a loading message to the channel while the sprite sheet is being made.
            RestUserMessage loader = await channel.SendMessageAsync("", false, P4G_Loading_Message().Build());

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
                Bitmap layer_1 = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P4G//Main//layer_1.png");
                Bitmap layer_2 = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P4G//Main//layer_2.png");
                Bitmap cursor = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P4G//Main//cursor.png");

                // Draw the layer with the user's colored default background if it exists.
                graphics.DrawImage(colored_background_bitmap, 0, 0, template_width, template_height);

                // Draw the user's background to the base template.
                graphics.DrawImage(background, 0, 0, template_width, template_height);

                // Draw the first textbox layer to the base template.
                graphics.DrawImage(layer_1, 0, 0, template_width, template_height);

                // Draw the character bust-up to the template if the base sprite number is not '0'.
                if (command_data.Base_Sprite != 0)
                {
                    graphics.DrawImage(bustup, bustup_data.P4G_Coord_X, bustup_data.P4G_Coord_Y, bustup_data.P4G_Scale_Width, bustup_data.P4G_Scale_Height);
                }

                // Draw the brown textbox layer and cursor to the template last.
                graphics.DrawImage(layer_2, 0, 0, template_width, template_height);
                graphics.DrawImage(cursor, 0, 0, template_width, template_height);

                // If the user has the HUD enabled, render it to the template as well.
                if (account.P4G_TS_HUD != "None")
                {
                    graphics.DrawImage(Render_Calendar_HUD(sl_command, account), 0, 0, template_width, template_height);
                }

                // Now, it's time to render the text.
                // Render the character's name to the template first.
                string display_name = OfficialSetMethods.GetDisplayName(account, command_data, set_data, bustup_data);
                graphics.DrawImage(Text_To_Brown(Render_Name(display_name)), 0, 0, template_width, template_height);

                List<string>[] parsed_lines = OfficialSetMethods.Line_Parser(sl_command, "P4G", command_data.Dialogue, 3, 1450);

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

        public static async Task Render_System_Message(SocialLinkerCommand sl_command, MakerCommandData command_data)
        {
            // Create variables to store the width and height of the template.
            int template_width = 1920;
            int template_height = 1080;

            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = sl_command.User;
            SocketTextChannel channel = (SocketTextChannel)sl_command.Channel;

            // Send a loading message to the channel while the sprite sheet is being made.
            RestUserMessage loader = await channel.SendMessageAsync("", false, P4G_Loading_Message().Build());

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
                Bitmap system_textbox = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P4G//Main//system.png");
                Bitmap cursor = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P4G//Main//cursor.png");

                // Draw the layer with the user's colored default background if it exists.
                graphics.DrawImage(colored_background_bitmap, 0, 0, template_width, template_height);

                // Draw the user's background to the base template.
                graphics.DrawImage(background, 0, 0, template_width, template_height);

                // Draw the textbox layer and cursor to the template last.
                graphics.DrawImage(system_textbox, 0, 0, template_width, template_height);
                graphics.DrawImage(cursor, 0, 0, template_width, template_height);

                // If the user has the HUD enabled, render it to the template as well.
                if (account.P4G_TS_HUD != "None")
                {
                    graphics.DrawImage(Render_Calendar_HUD(sl_command, account), 0, 0, template_width, template_height);
                }

                // Now, it's time to render the text.
                // Draw the input dialogue to the template.
                List<string>[] parsed_lines = OfficialSetMethods.Line_Parser(sl_command, "P4G", command_data.Dialogue, 3, 1450);
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

        public static Bitmap Render_Name(string display_name)
        {
            // Create a bitmap as large as the template.
            Bitmap base_template = new Bitmap(1920, 1080);

            // Establish an int for the width and height glyphs should be rendered at.
            int multiplier = 64;

            // Load the bitmap font.
            string font_sheet = $@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P4G//Font//p4g_font_sheet.png";

            // Create a variable to iterate through sections of the bitmap font.
            Bitmap current_glyph;

            // Specify X and Y coordinates for where the glyphs should start rendering on the template.
            int render_position_x = 102;
            int render_position_y = 748;

            char[] charArr = display_name.ToCharArray();

            for (int i = 0; i < charArr.Length; i++)
            {
                // Retrieve glyph information from the JSON file.
                var glyph = ParsingMethods.Get_P4G_Glyph(charArr[i]);

                // Check if the character is a line break.
                if (charArr[i] == '\u000a')
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

                            // Draw the glyph to the base bitmap.
                            graphics.DrawImage(current_glyph, (render_position_x - glyph.LeftCut), render_position_y, multiplier, multiplier);
                        }
                    }

                    // Set the next X value at the end of the current glyph's right width.
                    render_position_x += (glyph.RightCut - glyph.LeftCut);

                    // Check if the current iterated index is less than the number of indicies available.
                    if (i < charArr.Length - 1)
                    {
                        // If so, edit the position of the X coordinate according to specific kerning pairs.
                        if (charArr[i] == 'Y' && Char.IsLower(charArr[i + 1]))
                        {
                            render_position_x += -6;
                        }
                        else if (charArr[i] == 'v' && Char.IsLower(charArr[i + 1]))
                        {
                            render_position_x += -1;
                        }
                        else if (charArr[i] == 'T' && Char.IsLower(charArr[i + 1]) && charArr[i + 1] != 'h')
                        {
                            render_position_x += -6;
                        }
                    }
                }
            }

            return base_template;
        }

        public static Bitmap Render_Dialogue(List<string>[] dialogue_lines)
        {
            //Create a bitmap as large as the template
            Bitmap bitmap = new Bitmap(1920, 1080);

            // Create an int to keep track of rendering errors. This is neccessary to inform the user of any potential issues.
            int error_counter = 0;

            //Establish an int for the width and height glyphs should be rendered at
            int multiplier = 64;

            string font_sheet = $@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P4G//Font//p4g_font_sheet.png";
            Bitmap current_glyph;

            for (int i = 0; i < dialogue_lines.Length; i++)
            {
                //Establish variables for where the glyphs should be rendered on the template
                int render_position_x = 124;
                int render_position_y = 836 + (68 * i);

                char[] char_array = String_List_To_String(dialogue_lines[i]).ToCharArray();

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
                                //Copy the section of the bitmap font needed
                                Rectangle crop = new Rectangle(x, y, multiplier, multiplier);
                                current_glyph = originalImage.Clone(crop, originalImage.PixelFormat);

                                //Draw the glyph to the base bitmap
                                graphics.DrawImage(current_glyph, (render_position_x - glyph.LeftCut), render_position_y, multiplier, multiplier);
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
                                render_position_x += -6;
                            }
                            else if (char_array[j] == 'v' && Char.IsLower(char_array[j + 1]))
                            {
                                render_position_x += -1;
                            }
                            else if (char_array[j] == 'T' && (Char.IsLower(char_array[j + 1]) && char_array[j + 1] != 'h'))
                            {
                                render_position_x += -6;
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

            // Create another int to count the number of times a character comes up null from the font sheet.
            // We'll want to keep track of this number so we can ensure there's only one error message sent.
            int error_counter = 0;

            // Take the input string and turn it into a char array.
            char[] char_array = input_word.ToCharArray();

            // Now, let's iterate through the char array.
            for (int i = 0; i < char_array.Length; i++)
            {
                // Retrieve glyph information for the current character from the JSON file.
                var glyph = ParsingMethods.Get_P4G_Glyph(char_array[i]);

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
                                pixel_counter += -6;
                            }
                            else if (char_array[i] == 'v' && Char.IsLower(char_array[i + 1]))
                            {
                                pixel_counter += -1;
                            }
                            else if (char_array[i] == 'T' && Char.IsLower(char_array[i + 1]) && (char_array[i + 1] != 'h'))
                            {
                                pixel_counter += -6;
                            }
                        }

                        // Set the pixel counter to the appropriate width of the string so far.
                        pixel_counter += glyph.RightCut - glyph.LeftCut;
                    }
                }
                // If the character returns null, it's not supported by the template's font set.
                // Send a warning message to the user.
                else
                {
                    // Increase the error counter by one.
                    error_counter++;

                    // If the error counter is at exactly 1, send a warning message to the user.
                    if (error_counter == 1)
                    {
                        _ = ErrorHandling.Unsupported_Character(sl_command);
                    }
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

        public static Bitmap Render_Calendar_HUD(SocialLinkerCommand sl_command, UserInfoFields account)
        {
            // Create variables to store the width and height of the template.
            int template_width = 1920;
            int template_height = 1080;

            // Establish needed bitmap variables for the assets.
            Bitmap date_container = new Bitmap(2, 2);
            Bitmap weather_container = new Bitmap(2, 2);
            Bitmap hud = new Bitmap(2, 2);
            Bitmap corner_glow = new Bitmap(2, 2);

            switch (account.P4G_TS_HUD)
            {
                case "Normal":
                    corner_glow = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P4G//Main//corner_glow.png");
                    hud = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P4G//Main//hud_normal.png");
                    date_container = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P4G//Main//date_container.png");
                    weather_container = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P4G//Main//weather_container.png");
                    break;

                case "TV World":
                    corner_glow = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P4G//Main//corner_glow.png");
                    hud = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P4G//Main//hud_tv.png");
                    date_container = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P4G//Main//date_container.png");
                    weather_container = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P4G//Main//weather_container.png");
                    break;

                case "None":
                    break;
            }

            // Create a new bitmap with the width and height values specified earlier.
            Bitmap base_template = new Bitmap(template_width, template_height);

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
                Bitmap date_slash = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P4G//Main//Calendar//slash.png");

                Bitmap weather = null;

                // Get the user's current date and time according to their settings.
                DateTime user_time = Get_Date(sl_command, account);

                // Use the user's date and time to determine which assets to use.
                // Months
                char[] month = user_time.ToString("MM").ToCharArray();

                month_tens = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P4G//Main//Calendar//Month//Tens_Place//{month[0]}.png");
                month_ones = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P4G//Main//Calendar//Month//Ones_Place//{month[1]}.png");

                // Days
                char[] day = user_time.ToString("dd").ToCharArray();
                day_tens = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P4G//Main//Calendar//Day//Tens_Place//{day[0]}.png");
                day_ones = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P4G//Main//Calendar//Day//Ones_Place//{day[1]}.png");

                day_of_week = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P4G//Main//Calendar//Weekday//{user_time.ToString("dddd").ToLower()}.png");
                time_of_day = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P4G//Main//Calendar//Time_of_Day//{Get_Time_of_Day(user_time)}.png");

                weather = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P4G//Main//Weather//{Get_Weather(account)}.png");

                // Color the day of week bitmap depending on what day it currently is.
                if (Holiday_Check(user_time) == true)
                {
                    day_of_week = Day_Of_Week_To_Off_Day_Color_Scheme(day_of_week);
                }
                if (user_time.ToString("dddd").ToLower() == "saturday")
                {
                    day_of_week = Day_Of_Week_To_Saturday_Color_Scheme(day_of_week);
                }
                else if (user_time.ToString("dddd").ToLower() == "sunday")
                {
                    day_of_week = Day_Of_Week_To_Off_Day_Color_Scheme(day_of_week);
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
                    json_location = new TimedWebClient { Timeout = 5000 }.DownloadString($"http://api.weatherapi.com/v1/current.json?key={WeatherAPIConfig.weather_api_account.accountKey}&q={account.City}");
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
                    json_location = new TimedWebClient { Timeout = 5000 }.DownloadString($"http://api.weatherapi.com/v1/current.json?key={WeatherAPIConfig.weather_api_account.accountKey}&q={account.City}");
                }

                // Deserialize the JSON object and store it in a variable.
                var dataObject = JsonConvert.DeserializeObject<dynamic>(json_location);

                string current_condition = dataObject.current.condition.text.ToString();

                if (current_condition == "Sunny")
                {
                    return "sun";
                }
                else if (
                    current_condition == "Cloudy" ||
                    current_condition == "Partly cloudy" ||
                    current_condition == "Overcast" ||
                    current_condition == "Mist" ||
                    current_condition == "Fog" ||
                    current_condition == "Freezing fog" ||
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
                    current_condition == "Torrential rain shower")
                {
                    return "rain";
                }
                else if (
                    current_condition == "Thundery outbreaks possible" ||
                    current_condition == "Patchy light rain with thunder" ||
                    current_condition == "Moderate or heavy rain with thunder" ||
                    current_condition == "Patchy light snow with thunder" ||
                    current_condition == "Moderate or heavy snow with thunder")
                {
                    return "thunderstorm";
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
            // If it's a weekday, set it to After School.
            // If it's a Sunday or during school vacation, set it to Daytime.
            else if (hour < evening && hour >= after_school)
            {
                if (DateTime.Now.ToString("ddd") == "Sun")
                {
                    tod = "daytime";
                }
                else
                {
                    tod = "after_school";
                }
            }
            // If the current hour is before 1PM and after or on 12PM, set the time depending on the day.
            // If it's a weekday, set it to Lunchtime.
            // If it's a Sunday or during school vacation, set it to Daytime.
            else if (hour < after_school && hour >= lunchtime)
            {
                if (DateTime.Now.ToString("ddd") == "Sun")
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
            for (int i = 90; i < 1320; i++)
            {
                // Create a nested for loop to iterate over the Y values where the name could be rendered.
                for (int j = 735; j < 810; j++)
                {
                    // Get the current pixel from the input bitmap.
                    actual_color = input_bitmap.GetPixel(i, j);

                    // Color in the pixel with the new color while keeping its current alpha value.
                    System.Drawing.Color new_color = System.Drawing.Color.FromArgb(actual_color.A, 77, 40, 13);
                    new_bitmap.SetPixel(i, j, new_color);
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
            for (int x = 1655; x < 1755; x++)
            {
                // Create a nested for loop to iterate over the Y values of the bitmap to be changed.
                for (int y = 35; y < 75; y++)
                {
                    // Get the current pixel from the input bitmap.
                    actual_color = input_bitmap.GetPixel(x, y);

                    // Color in the pixel with the new color while keeping its current alpha value.
                    System.Drawing.Color new_color = System.Drawing.Color.FromArgb(actual_color.A, 209, 225, 250);
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
            for (int x = 1655; x < 1755; x++)
            {
                // Create a nested for loop to iterate over the Y values of the bitmap to be changed.
                for (int y = 35; y < 75; y++)
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

        // Calendar checks
        public static bool Holiday_Check(DateTime user_time)
        {
            try
            {
                // Establish the directory of the file and then search for all JSON documents that start with "holiday_calendar_". This should only bring in one result.
                string holiday_calendar_path = $@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}\SceneMaker\Data\Calendar_Data";
                string[] file_search = Directory.GetFiles(holiday_calendar_path, $"holiday_calendar_*.json");

                // Read in all the text of the file.
                string json_text = File.ReadAllText(file_search[0]);

                // Deserialize the JSON object and store it in a variable.
                var dataObject = JsonConvert.DeserializeObject<dynamic>(json_text);

                // Iterate through each item of the JSON object.
                foreach (var item in dataObject)
                {
                    // If the JSON contains an entry with the same month and day as the user's current time, return true.
                    if (item.Month == user_time.ToString("MMMM") && item.Day == user_time.ToString("dd"))
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return false;
        }

        public static bool School_Vacation_Check(DateTime user_time)
        {
            try
            {
                // Establish the directory of the file and then search for all JSON documents that start with "academic_calendar_". This should only bring in one result.
                string holiday_calendar_path = $@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}\SceneMaker\Data\Calendar_Data";
                string[] file_search = Directory.GetFiles(holiday_calendar_path, $"academic_calendar_*.json");

                // Read in all the text of the file.
                string json_text = File.ReadAllText(file_search[0]);

                // Deserialize the JSON object and store it in a variable.
                var dataObject = JsonConvert.DeserializeObject<dynamic>(json_text);

                string stored_condition = "";

                // Iterate through each item of the JSON object.
                foreach (var item in dataObject)
                {
                    // Get the info of the current item and create a DateTime object from it. We'll use this to compare to the user's current time.
                    DateTime current_item = new DateTime(Int32.Parse(item.Year.ToString()), DateTime.ParseExact(item.Month.ToString(), "MMMM", CultureInfo.InvariantCulture).Month, Int32.Parse(item.Day.ToString()), 0, 0, 0);

                    // If the user's time is after the current item's time, store the condition of the current item in the stored condition variable.
                    if (user_time >= current_item)
                    {
                        stored_condition = item.Condition;
                    }
                    // If the user's time is BEFORE the current item's time, we stop here and compare!
                    // Take a look at the stored condition's value.
                    // Since the item values alternate between opening and closing days, the user's time will be between these periods.
                    else
                    {
                        if (stored_condition == "First Day of School" && item.Condition == "Closing Ceremony")
                        {
                            return false;
                        }
                        else if (stored_condition == "Closing Ceremony" && item.Condition == "First Day of School")
                        {
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return false;
        }

        // Loading message
        public static EmbedBuilder P4G_Loading_Message()
        {
            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Generating Scene...",
                IconUrl = EmbedSettings.Get_Game_Logo("P4G")
            };

            embed.WithAuthor(author);
            embed.WithColor(EmbedSettings.Get_Game_Color("P4G", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Loading_Icon("P4G", null));
            embed.WithDescription("This may take a few seconds!");

            return embed;
        }
    }

    // Class from https://stackoverflow.com/questions/12878857/how-to-limit-the-time-downloadstringurl-allowed-by-500-milliseconds
    public class TimedWebClient : WebClient
    {
        // Timeout in milliseconds, default = 600,000 msec
        public int Timeout { get; set; }

        public TimedWebClient()
        {
            this.Timeout = 600000;
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var objWebRequest = base.GetWebRequest(address);
            objWebRequest.Timeout = this.Timeout;
            return objWebRequest;
        }
    }
}
