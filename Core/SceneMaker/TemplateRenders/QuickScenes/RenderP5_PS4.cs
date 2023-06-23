using System;
using System.Drawing;
using System.Threading.Tasks;
using Discord.Commands;
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
    public class RenderP5_PS4 : ModuleBase<SocketCommandContext>
    {
        public const int template_width = 1920;
        public const int template_height = 1080;
        Random rnd = new Random();

        // The specific point the message window vector is rotated at.
        Point message_window_point_of_rotation = new Point(0, 0);

        // The specific point the spriteless nametag vector is rotated at.
        Point spriteless_nametag_point_of_rotation = new Point(0, 0);

        // The random angle the message window is rotated at from its top leftmost point.
        float message_window_rotation_angle = 0;

        // The X coordinate of the cursor. Changes with the length of the message window.
        int cursor_x_coord = 0;

        Bitmap rotated_void_layer = new Bitmap(2, 2);
        Bitmap scene_border = new Bitmap(2, 2);
        bool is_spriteless = false;

        public async Task Render_Quick_Scene_P5_PS4(SocialLinkerCommand sl_command, OfficialSetData set_data, MakerCommandData command_data)
        {
            SocketUser user = sl_command.User;
            SocketTextChannel channel = (SocketTextChannel)sl_command.Channel;

            RestUserMessage loader = await channel.SendMessageAsync("", false, P5_PS4_Loading_Message().Build());

            var account = UserInfoClasses.GetAccount(user);

            if (command_data.Base_Sprite == 0)
            {
                is_spriteless = true;
            }

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

            string display_name = OfficialSetMethods.GetDisplayName(account, command_data, set_data, bustup_data);
            display_name = OfficialSetMethods.Validate_Input(sl_command, "P5-PS4", "Name", display_name);

            Bitmap calendar = new Bitmap(2, 2);

            command_data.Dialogue = OfficialSetMethods.Validate_Input(sl_command, "P5-PS4", "Dialogue", command_data.Dialogue);
            List<string>[] parsed_lines = OfficialSetMethods.Line_Parser(sl_command, "P5-PS4", command_data.Dialogue, 3, 820);

            // Textbox layers MUST be rendered here
            Bitmap dialogue_layers = new Bitmap(2, 2);
            if (is_spriteless == false)
            {
                dialogue_layers = Combine_Normal_Textbox_Layers(account, parsed_lines, display_name);
            }
            else
            {
                dialogue_layers = Combine_Spriteless_Textbox_Layers(account, parsed_lines, display_name);
            }

            // Time to put it all together!
            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                graphics.DrawImage(background, 0, 0, template_width, template_height);
                graphics.DrawImage(scene_border, 0, 0, template_width, template_height);

                if (command_data.Base_Sprite != 0)
                {
                    Bitmap bustup_layer = Render_Bustup(account, bustup, bustup_data);
                    graphics.DrawImage(bustup_layer, 0, 0, bustup_layer.Width, bustup_layer.Height);
                }

                if (account.P5_PS4_TS_HUD != "None")
                {
                    calendar = Construct_Calendar(sl_command, account);
                    graphics.DrawImage(calendar, 0, 0, template_width, template_height);
                }

                graphics.DrawImage(dialogue_layers, 0, 0, template_width, template_height);
                graphics.DrawImage(Combine_Cursor_And_Control_Panel_Layers(account), 0, 0, template_width, template_height);
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
            SocketUser user = sl_command.User;
            SocketTextChannel channel = (SocketTextChannel)sl_command.Channel;

            RestUserMessage loader = await channel.SendMessageAsync("", false, P5_PS4_Loading_Message().Build());

            var account = UserInfoClasses.GetAccount(user);

            if (command_data.Base_Sprite == 0)
            {
                is_spriteless = true;
            }

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

            Bitmap calendar = new Bitmap(2, 2);

            command_data.Dialogue = OfficialSetMethods.Validate_Input(sl_command, "P5-PS4", "Dialogue", command_data.Dialogue);
            List<string>[] parsed_lines = OfficialSetMethods.Line_Parser(sl_command, "P5-PS4", command_data.Dialogue, 3, 820);

            // Textbox layers MUST be rendered here
            Bitmap dialogue_layers = Combine_System_Textbox_Layers(account, parsed_lines);

            // Time to put it all together!
            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                graphics.DrawImage(background, 0, 0, template_width, template_height);
                graphics.DrawImage(scene_border, 0, 0, template_width, template_height);

                if (account.P5_PS4_TS_HUD != "None")
                {
                    calendar = Construct_Calendar(sl_command, account);
                    graphics.DrawImage(calendar, 0, 0, template_width, template_height);
                }

                graphics.DrawImage(dialogue_layers, 0, 0, template_width, template_height);
                graphics.DrawImage(Combine_Cursor_And_Control_Panel_Layers(account), 0, 0, template_width, template_height);
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

        public Bitmap Render_Bustup(UserInfoFields account, Bitmap bustup, BustupData bustup_data)
        {
            Bitmap base_template = new Bitmap(template_width, template_height);
            Bitmap drop_shadow = new Bitmap(2, 2);

            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                drop_shadow = Create_Bustup_Drop_Shadow(bustup);
                drop_shadow = (Bitmap)Set_Image_Opacity(drop_shadow, (float)0.8);
                graphics.DrawImage(drop_shadow, bustup_data.P5_PS4_Coord_X - 30, bustup_data.P5_PS4_Coord_Y + 30, bustup_data.P5_PS4_Scale_Width, bustup_data.P5_PS4_Scale_Height);
                graphics.DrawImage(bustup, bustup_data.P5_PS4_Coord_X, bustup_data.P5_PS4_Coord_Y, bustup_data.P5_PS4_Scale_Width, bustup_data.P5_PS4_Scale_Height);
            }

            return base_template;
        }

        // Assemble Layers
        public Bitmap Combine_Normal_Textbox_Layers(UserInfoFields account, List<string>[] parsed_dialogue, string display_name)
        {
            int number_of_lines = Get_Number_Of_Lines(parsed_dialogue);
            int length_of_longest_line = Get_Max_Line_Length(parsed_dialogue);
            Bitmap base_template = new Bitmap(template_width, template_height);

            Bitmap nametag = Render_Nametag_Window(display_name);
            Bitmap message_window = Render_Message_Window(account, parsed_dialogue);

            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                // Message Window
                double rotation_angle = rnd.NextDouble(0, 3.2);
                message_window = Rotate_Image_On_Point(message_window, (float)rotation_angle, message_window_point_of_rotation.X, message_window_point_of_rotation.Y, false);
                rotated_void_layer = Rotate_Image_On_Point(rotated_void_layer, (float)rotation_angle, message_window_point_of_rotation.X, message_window_point_of_rotation.Y, false);

                message_window_rotation_angle = (float)rotation_angle;

                if (account.P5_PS4_TS_Border != "None")
                {
                    Rectangle cropped_area = new Rectangle(0, 0, 1920, 1080);
                    scene_border = Render_Screen_Border(account);
                    scene_border = Delete_Pixel_Overlap(scene_border, rotated_void_layer, cropped_area);
                }

                graphics.DrawImage(message_window, 0, 0, template_width, template_height);

                // Nametag
                if (number_of_lines > 1)
                {
                    graphics.DrawImage(nametag, 13, -13, nametag.Width, nametag.Height);
                }
                else
                {
                    graphics.DrawImage(nametag, 0, 0, nametag.Width, nametag.Height);
                }

                // Cursor
                if (length_of_longest_line >= 750)
                {
                    cursor_x_coord -= 43;
                }

                graphics.DrawImage(Render_Dialogue(parsed_dialogue), 0, 0, template_width, template_height);
            }

            return base_template;
        }

        public Bitmap Combine_Spriteless_Textbox_Layers(UserInfoFields account, List<string>[] parsed_dialogue, string display_name)
        {
            int number_of_lines = Get_Number_Of_Lines(parsed_dialogue);
            int length_of_longest_line = Get_Max_Line_Length(parsed_dialogue);
            Bitmap base_template = new Bitmap(template_width, template_height);

            Bitmap nametag = Render_Spriteless_Nametag_Window(display_name);

            Bitmap message_window = Render_Spriteless_Message_Window(account, parsed_dialogue);

            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                // Message Window
                double rotation_angle = rnd.NextDouble(0, 3.2);
                message_window = Rotate_Image_On_Point(message_window, (float)rotation_angle, message_window_point_of_rotation.X, message_window_point_of_rotation.Y, false);
                rotated_void_layer = Rotate_Image_On_Point(rotated_void_layer, (float)rotation_angle, message_window_point_of_rotation.X, message_window_point_of_rotation.Y, false);

                message_window_rotation_angle = (float)rotation_angle;

                if (account.P5_PS4_TS_Border != "None")
                {
                    Rectangle cropped_area = new Rectangle(0, 0, 1920, 1080);
                    scene_border = Render_Screen_Border(account);
                    scene_border = Delete_Pixel_Overlap(scene_border, rotated_void_layer, cropped_area);
                }

                graphics.DrawImage(message_window, 0, 0, template_width, template_height);

                // Nametag
                if (number_of_lines > 1)
                {
                    nametag = Rotate_And_Place_Spriteless_Nametag(nametag, spriteless_nametag_point_of_rotation, -19, -14); // Point of rotation set in Render_Spriteless_Nametag_Window method
                    graphics.DrawImage(nametag, 12, 54, nametag.Width, nametag.Height);
                }
                else
                {
                    nametag = Rotate_And_Place_Spriteless_Nametag(nametag, spriteless_nametag_point_of_rotation, -19, -12); // Point of rotation set in Render_Spriteless_Nametag_Window method
                    graphics.DrawImage(nametag, -5, 71, nametag.Width, nametag.Height);
                }

                // Cursor
                if (length_of_longest_line >= 750)
                {
                    cursor_x_coord -= 43;
                }

                graphics.DrawImage(Render_Dialogue(parsed_dialogue), -84, 1, template_width, template_height);
            }

            return base_template;
        }

        public Bitmap Combine_System_Textbox_Layers(UserInfoFields account, List<string>[] parsed_dialogue)
        {
            int length_of_longest_line = Get_Max_Line_Length(parsed_dialogue);
            Bitmap base_template = new Bitmap(template_width, template_height);

            Bitmap message_window = Render_Spriteless_Message_Window(account, parsed_dialogue);

            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                // Message Window
                double rotation_angle = rnd.NextDouble(0, 3.2);
                message_window = Rotate_Image_On_Point(message_window, (float)rotation_angle, message_window_point_of_rotation.X, message_window_point_of_rotation.Y, false);
                rotated_void_layer = Rotate_Image_On_Point(rotated_void_layer, (float)rotation_angle, message_window_point_of_rotation.X, message_window_point_of_rotation.Y, false);

                message_window_rotation_angle = (float)rotation_angle;

                if (account.P5_PS4_TS_Border != "None")
                {
                    Rectangle cropped_area = new Rectangle(0, 0, 1920, 1080);
                    scene_border = Render_Screen_Border(account);
                    scene_border = Delete_Pixel_Overlap(scene_border, rotated_void_layer, cropped_area);
                }

                graphics.DrawImage(message_window, 0, 0, template_width, template_height);

                // Cursor
                if (length_of_longest_line >= 750)
                {
                    cursor_x_coord -= 43;
                }

                graphics.DrawImage(Render_Dialogue(parsed_dialogue), -84, 1, template_width, template_height);
            }

            return base_template;
        }

        public Bitmap Combine_Cursor_And_Control_Panel_Layers(UserInfoFields account)
        {
            Bitmap base_template = new Bitmap(template_width, template_height);

            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                switch (account.P5_PS4_TS_Panel)
                {
                    case "Manual (with Control Panel)":
                        graphics.DrawImage(Render_Manual_Advance_Tick(), cursor_x_coord, 0, template_width, template_height);
                        graphics.DrawImage(Render_Control_Panel(account), 0, 0, template_width, template_height);
                        break;

                    case "Manual (without Control Panel)":
                        graphics.DrawImage(Render_Manual_Advance_Tick(), cursor_x_coord, 0, template_width, template_height);
                        break;

                    case "Auto-Advance":
                        graphics.DrawImage(Render_Control_Panel(account), 0, 0, template_width, template_height);
                        break;
                }
            }

            return base_template;
        }

        // Text Rendering
        public Bitmap Render_Name(string display_name)
        {
            // The pixel spacing between characters in nametags are the original kerning (minus 2 due to out measuring method that condenses the spacing), plus 3 pixels to make it wider, totalling 5 pixels.
            int name_spacer = 5;

            // We want to subtract the adjusted spacing amount at the end to account for the last character in the string which shouldn't have this additional spacing after it.
            int display_name_length = Measure_Word_Pixel_Length(null, display_name) + (display_name.Length * name_spacer) - name_spacer; 
            
            // To give room to potential boxed characters at the start or end of the string to render, we pad the beginning and end with 5 pixels, meaning 10 pixels in total.
            display_name_length += 10;

            Bitmap base_template = new Bitmap(display_name_length, 54);
            int index_counter = -1;
            int boxed_char_count = 0;
            List<char> edited_display_name_list = display_name.ToCharArray().ToList();

            edited_display_name_list.RemoveAll(character => char.IsLetterOrDigit(character) == false);

            string edited_display_name = Char_List_To_String(edited_display_name_list);

            List<string>[] display_name_list = new List<string>[] { new List<string> { display_name } };

            // Establish an int for the width and height glyphs should be rendered at.
            int multiplier = 48;

            string font_sheet = $@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5-PS4//Font//p5-ps4_font_sheet.png";
            Bitmap current_glyph;

            for (int i = 0; i < display_name_list.Length; i++)
            {
                // Start the rendering 5 pixels in to give room to a potential boxed character at the beginning of the string.
                int render_position_x = 5;
                int render_position_y = 0 + (68 * i);

                char[] char_array = String_List_To_String(display_name_list[i]).ToCharArray();

                for (int j = 0; j < char_array.Length; j++)
                {
                    // Retrieve glyph information from the JSON file
                    var glyph = ParsingMethods.GetGlyph(char_array[j]);

                    if (glyph != null)
                    {
                        int x = multiplier * glyph.Column;
                        int y = multiplier * glyph.Row;

                        using (Graphics graphics = Graphics.FromImage(base_template))
                        {
                            using (var originalImage = new Bitmap(font_sheet))
                            {
                                // Copy the section of the loaded bitmap font needed.
                                Rectangle crop = new Rectangle(x, y, multiplier, multiplier);
                                current_glyph = originalImage.Clone(crop, originalImage.PixelFormat);

                                if (char.IsLetterOrDigit(char_array[j]))
                                {
                                    index_counter++;

                                    if (Is_Boxed_Letter(edited_display_name.Length, index_counter))
                                    {
                                        boxed_char_count++;

                                        var tilt_degree_values = new[] { -4, 5 };
                                        int random_tilt_degree = tilt_degree_values[rnd.Next(tilt_degree_values.Length)];

                                        current_glyph = Render_Boxed_Letter(current_glyph, glyph, char_array[j], boxed_char_count);
                                        current_glyph = Rotate_Image_On_Point(current_glyph, random_tilt_degree, current_glyph.Width / 2, current_glyph.Height / 2, false);
                                        graphics.DrawImage(current_glyph, (render_position_x - glyph.LeftCut), render_position_y, 48, 54);
                                    }
                                    else
                                    {
                                        Bitmap black_glyph = Bitmap_To_Color(current_glyph, System.Drawing.Color.Black, new Rectangle(0, 0, current_glyph.Width, current_glyph.Height));
                                        graphics.DrawImage(black_glyph, (render_position_x - glyph.LeftCut), render_position_y, 48, 48);
                                    }
                                }
                                else
                                {
                                    Bitmap black_glyph = Bitmap_To_Color(current_glyph, System.Drawing.Color.Black, new Rectangle(0, 0, current_glyph.Width, current_glyph.Height));
                                    graphics.DrawImage(black_glyph, (render_position_x - glyph.LeftCut), render_position_y, 48, 48);
                                }
                            }
                        }

                        if (j < char_array.Length - 1)
                        {
                            render_position_x += (glyph.RightCut - glyph.LeftCut);
                            render_position_x += Get_Kerning_Adjustment(char_array, j);
                            render_position_x += name_spacer;
                        }
                        
                    }
                }
            }

            // Shrink the bitmap so it's identical in size to how nametags are rendered in-game.
            base_template = ScaleImage(base_template, 5000, 47);

            return base_template;
        }

        public Bitmap Render_Dialogue(List<string>[] dialogue_lines)
        {
            int start_point_x = 688;
            int start_point_y = 905;
            int number_of_lines = Get_Number_Of_Lines(dialogue_lines);

            switch (number_of_lines)
            {
                case 1:
                    // Do nothing
                    break;

                case 2:
                    start_point_y -= 24;
                    break;

                case 3:
                    start_point_y -= 45;
                    break;
            }

            // Rotation start
            float radian = (float)(message_window_rotation_angle * Math.PI / 180) / 4;

            start_point_x = (int)(start_point_x * Math.Cos(radian) - start_point_y * Math.Sin(radian)) + (int)(1.5 * message_window_rotation_angle);
            start_point_y = (int)(start_point_x * Math.Sin(radian) + start_point_y * Math.Cos(radian));
            // Rotation end

            //Create a bitmap as large as the template
            Bitmap bitmap = new Bitmap(template_width, template_height);

            //Establish an int for the width and height glyphs should be rendered at
            int multiplier = 48;

            string font_sheet = $@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5-PS4//Font//p5-ps4_font_sheet.png";
            Bitmap current_glyph;

            for (int i = 0; i < dialogue_lines.Length; i++)
            {
                //Establish variables for where the glyphs should be rendered on the template
                int render_position_x = start_point_x;
                int render_position_y = start_point_y + (45 * i);

                char[] char_array = String_List_To_String(dialogue_lines[i]).ToCharArray();

                for (int j = 0; j < char_array.Length; j++)
                {
                    //Retrieve glyph information from the JSON file
                    var glyph = ParsingMethods.GetGlyph(char_array[j]);

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
                                graphics.DrawImage(current_glyph, (render_position_x - glyph.LeftCut), render_position_y, 42, 42);
                            }
                        }

                        //Set the next X value at the end of the current glyph's right width
                        render_position_x += (glyph.RightCut - glyph.LeftCut);

                        // Check if the current iterated index is less than the number of indicies available.
                        if (j < char_array.Length - 1)
                        {
                            // If so, edit the position of the X coordinate according to specific kerning pairs.

                            render_position_x += Get_Kerning_Adjustment(char_array, j);
                        }
                    }
                }
            }

            return bitmap;
        }

        public static int Get_Kerning_Adjustment(char[] char_array, int current_index)
        {
            int render_position_x = -2;

            if (char_array[current_index] == ' ')
            {
                render_position_x += -12;
            }

            if (char_array[current_index] == '.')
            {
                render_position_x += 3;
            }

            if (char_array[current_index] == ',')
            {
                render_position_x += 3;
            }

            if (char_array[current_index + 1] == '\'')
            {
                render_position_x += 1;
            }

            if (char_array[current_index] == '\'')
            {
                render_position_x += 2;
            }

            if (char_array[current_index] == 'B')
            {
                render_position_x += -1;
            }

            if (char_array[current_index] == 'l')
            {
                render_position_x += 2;
            }

            if (char_array[current_index] == 'i')
            {
                render_position_x += 2;
            }

            if (char_array[current_index] == 'm')
            {
                render_position_x += -2;
            }

            if (char_array[current_index] == 'y')
            {
                render_position_x += -2;
            }

            return render_position_x;
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
                var glyph = ParsingMethods.Get_P5_PS4_Glyph(char_array[i]);

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
                        // Set the pixel counter to the appropriate width of the string so far.
                        pixel_counter += glyph.RightCut - glyph.LeftCut;

                        // Check if the current iterated index is less than the number of indicies available.
                        if (i < char_array.Length - 1)
                        {
                            pixel_counter += Get_Kerning_Adjustment(char_array, i);
                        }
                    }
                }
                else if (char_array[i] == '\ufe0f')
                {
                    // Do nothing, emoji variation selector
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

        public static string Char_List_To_String(List<char> input_list)
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

        // Vector Rendering
        public Bitmap Render_Nametag_Window(string display_name)
        {
            Bitmap base_template = new Bitmap(template_width, template_height);
            Bitmap black_layer = new Bitmap(template_width, template_height);
            Bitmap white_layer = new Bitmap(template_width, template_height);

            Bitmap display_name_layer = Render_Name(display_name);

            // Create a brush for the color white.
            SolidBrush blackBrush = new SolidBrush(System.Drawing.Color.Black);

            // Create a brush for the color white.
            SolidBrush whiteBrush = new SolidBrush(System.Drawing.Color.White);

            // How the vectors are rendered is strongly determined
            int default_line_length = 220;

            // Create multiple variables for the potential min and max values of the thirteen black outer points of the message window.
            int black_point_1_x_min = 386;
            int black_point_1_x_max = 387;
            int black_point_1_y_min = 757;
            int black_point_1_y_max = 759;

            int black_point_2_x_min = 487;
            int black_point_2_x_max = 490;
            int black_point_2_y_min = 730;
            int black_point_2_y_max = 735;

            int black_point_3_x_min = 472;
            int black_point_3_x_max = 486;
            int black_point_3_y_min = 700;
            int black_point_3_y_max = 703;

            int black_point_4_x_min = 783;
            int black_point_4_x_max = 790;
            int black_point_4_y_min = 693;
            int black_point_4_y_max = 693;

            int black_point_5_x_min = 810;
            int black_point_5_x_max = 819;
            int black_point_5_y_min = 786;
            int black_point_5_y_max = 788;

            int black_point_6_x_min = 548;
            int black_point_6_x_max = 556;
            int black_point_6_y_min = 762;
            int black_point_6_y_max = 764;

            int black_point_7_x_min = 550;
            int black_point_7_x_max = 559;
            int black_point_7_y_min = 791;
            int black_point_7_y_max = 796;

            int name_render_point_x = 0;
            if (display_name_layer.Width <= default_line_length)
            {
                name_render_point_x = black_point_1_x_min + 258 - (display_name_layer.Width / 2);
            }
            else
            {
                name_render_point_x = black_point_1_x_min + 140;

                black_point_4_x_min = name_render_point_x + display_name_layer.Width + 15;
                black_point_4_x_max = black_point_4_x_min + 7;

                black_point_5_x_min = black_point_4_x_min + 27;
                black_point_5_x_max = black_point_5_x_min + 9;
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

            // This conditional helps expand and angle black point #5 properly, close to how it's seen in-game.
            // Based on the positioning of black point #6.
            if (display_name_layer.Width <= default_line_length)
            {
                // Do nothing
            }
            else
            {
                black_point_5_x = rnd.Next(black_point_5_x_min, black_point_5_x_max + 1);
                black_point_5_y = rnd.Next(black_point_6_y_min + ((black_point_5_x - black_point_6_x) / 11), black_point_6_y_max + ((black_point_5_x - black_point_6_x) / 11) + 1);
            }

            // Randomly set the X and Y values of the thirteen points of the inner white vector based on the set black point X & Y values.
            int white_point_1_x = rnd.Next(black_point_1_x + 18, black_point_1_x + 21);
            int white_point_1_y = rnd.Next(black_point_1_y - 2, black_point_1_y + 2);

            int white_point_2_x = rnd.Next(black_point_2_x + 10, black_point_2_x + 20);
            int white_point_2_y = rnd.Next(black_point_2_y + 4, black_point_2_y + 12);

            int white_point_3_x = rnd.Next(black_point_3_x + 9, black_point_3_x + 15);
            int white_point_3_y = rnd.Next(black_point_3_y + 5, black_point_3_y + 8);

            int white_point_4_x = rnd.Next(black_point_4_x - 11, black_point_4_x - 7);
            int white_point_4_y = rnd.Next(black_point_4_y + 7, black_point_4_y + 13);

            int white_point_5_x = rnd.Next(black_point_5_x - 22, black_point_5_x - 16);
            int white_point_5_y = rnd.Next(black_point_5_y - 18, black_point_5_y - 14); //22

            int white_point_6_x = rnd.Next(black_point_6_x - 17, black_point_6_x - 5);
            int white_point_6_y = rnd.Next(black_point_6_y - 5, black_point_6_y - 2);

            int white_point_7_x = rnd.Next(black_point_7_x - 12, black_point_7_x - 8);
            int white_point_7_y = rnd.Next(black_point_7_y - 12, black_point_7_y - 9);

            // Create the thirteen points of the black vector from the randomly chosen values.
            Point black_point_1 = new Point(black_point_1_x, black_point_1_y);
            Point black_point_2 = new Point(black_point_2_x, black_point_2_y);
            Point black_point_3 = new Point(black_point_3_x, black_point_3_y);
            Point black_point_4 = new Point(black_point_4_x, black_point_4_y);
            Point black_point_5 = new Point(black_point_5_x, black_point_5_y);
            Point black_point_6 = new Point(black_point_6_x, black_point_6_y);
            Point black_point_7 = new Point(black_point_7_x, black_point_7_y);

            // Create the thirteen points of the white vector from the randomly chosen values.
            Point white_point_1 = new Point(white_point_1_x, white_point_1_y);
            Point white_point_2 = new Point(white_point_2_x, white_point_2_y);
            Point white_point_3 = new Point(white_point_3_x, white_point_3_y);
            Point white_point_4 = new Point(white_point_4_x, white_point_4_y);
            Point white_point_5 = new Point(white_point_5_x, white_point_5_y);
            Point white_point_6 = new Point(white_point_6_x, white_point_6_y);
            Point white_point_7 = new Point(white_point_7_x, white_point_7_y);

            // Add all the points for the outer black vector into a point array.
            Point[] black_poly_points = {
                    black_point_1,
                    black_point_2,
                    black_point_3,
                    black_point_4,
                    black_point_5,
                    black_point_6,
                    black_point_7 };

            // Add all the points for the inner white vector into a point array.
            Point[] white_poly_points = {
                    white_point_1,
                    white_point_2,
                    white_point_3,
                    white_point_4,
                    white_point_5,
                    white_point_6,
                    white_point_7 };

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

            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                // Set the graphics rendering to have antialiasing.
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                black_layer = (Bitmap)Set_Image_Opacity(black_layer, (float)0.85);

                graphics.DrawImage(black_layer, 0, 0, template_width, template_height);
                graphics.DrawImage(white_layer, 0, 0, template_width, template_height);
            }

            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                // Set the graphics rendering to have antialiasing.
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                graphics.DrawImage(display_name_layer, name_render_point_x, black_point_1_y_min - 46, display_name_layer.Width, display_name_layer.Height);
            }

            base_template = Rotate_And_Place_Nametag(base_template, white_point_1);

            // Return the base template.
            return base_template;
        }

        public Bitmap Render_Message_Window(UserInfoFields account, List<string>[] input_list_array)
        {
            // NOTE: Points are determined from the top leftmost point of the message window, going counterclockwise

            int number_of_lines = Get_Number_Of_Lines(input_list_array);
            int length_of_longest_line = Get_Max_Line_Length(input_list_array);

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

            // How the vectors are rendered is strongly determined by the amount of text the user inputs.
            int default_line_length = 400;
            int base_gap_length_past_default = 302;
            int starting_dialogue_position = 686;

            // Create multiple variables for the potential min and max values of the thirteen black outer points of the message window.
            int black_point_1_x_min = 454;
            int black_point_1_x_max = 460;
            int black_point_1_y_min = 908;
            int black_point_1_y_max = 913;

            // black_point_2 is dependent on black_point_1

            int black_point_3_x_min = 547;
            int black_point_3_x_max = 556;
            int black_point_3_y_min = 990;
            int black_point_3_y_max = 993;

            int black_point_4_x_min = 567;
            int black_point_4_x_max = 577;
            int black_point_4_y_min = 969;
            int black_point_4_y_max = 973;

            int black_point_5_x_min = 642;
            int black_point_5_x_max = 647;
            int black_point_5_y_min = 1020;
            int black_point_5_y_max = 1024;

            int black_point_6_x_min = 671;
            int black_point_6_x_max = 677;
            int black_point_6_y_min = 997;
            int black_point_6_y_max = 999;

            int black_point_7_x_min = 1384;
            int black_point_7_x_max = 1384;
            int black_point_7_y_min = 1043;
            int black_point_7_y_max = 1043;

            int black_point_8_x_min = 1431;
            int black_point_8_x_max = 1431;
            int black_point_8_y_min = 1034;
            int black_point_8_y_max = 1034;

            int black_point_9_x_min = 1385;
            int black_point_9_x_max = 1385;
            int black_point_9_y_min = 803;
            int black_point_9_y_max = 805;

            int black_point_10_x_min = 1384;
            int black_point_10_x_max = 1384;
            int black_point_10_y_min = 767;
            int black_point_10_y_max = 767;

            int black_point_11_x_min = 1300;
            int black_point_11_x_max = 1300;
            int black_point_11_y_min = 782;
            int black_point_11_y_max = 782;

            int black_point_12_x_min = 602;
            int black_point_12_x_max = 602;
            int black_point_12_y_min = 854;
            int black_point_12_y_max = 854;

            int black_point_13_x_min = 584;
            int black_point_13_x_max = 592;
            int black_point_13_y_min = 907;
            int black_point_13_y_max = 912;

            int black_point_14_x_min = 527;
            int black_point_14_x_max = 540;
            int black_point_14_y_min = 872;
            int black_point_14_y_max = 878;

            int black_point_15_x_min = 526;
            int black_point_15_x_max = 532;
            int black_point_15_y_min = 895;
            int black_point_15_y_max = 909;

            int black_point_16_x_min = 518;
            int black_point_16_x_max = 524;
            int black_point_16_y_min = 907;
            int black_point_16_y_max = 920;

            if (length_of_longest_line > default_line_length)
            {
                double[] gap_factors = { 0.3, 0.35, 0.4, 0.45, 0.5 };
                double gap_multiplier = gap_factors[rnd.Next(0, gap_factors.Length)];

                if (length_of_longest_line >= 700)
                {
                    gap_multiplier = 0.5;
                }

                int adjusted_black_point_10_x = starting_dialogue_position + length_of_longest_line + (int)(base_gap_length_past_default - ((length_of_longest_line - default_line_length) * gap_multiplier));

                black_point_7_x_min = adjusted_black_point_10_x;
                black_point_7_x_max = adjusted_black_point_10_x;
                black_point_7_y_min = 1043;
                black_point_7_y_max = 1043;

                black_point_8_x_min = adjusted_black_point_10_x + 47;
                black_point_8_x_max = adjusted_black_point_10_x + 47;
                black_point_8_y_min = 1034;
                black_point_8_y_max = 1034;

                black_point_9_x_min = adjusted_black_point_10_x + 1;
                black_point_9_x_max = adjusted_black_point_10_x + 1;
                black_point_9_y_min = 803;
                black_point_9_y_max = 805;

                black_point_10_x_min = adjusted_black_point_10_x;
                black_point_10_x_max = adjusted_black_point_10_x;
                black_point_10_y_min = 767;
                black_point_10_y_max = 767;

                black_point_11_x_min = adjusted_black_point_10_x - 84;
                black_point_11_x_max = adjusted_black_point_10_x - 84;
                black_point_11_y_min = 782;
                black_point_11_y_max = 782;
            }
            else
            {
                // Do nothing
            }

            // Randomly set the X and Y values of the outer thirteen points of the vector using the min and max values.
            int black_point_1_x = rnd.Next(black_point_1_x_min, black_point_1_x_max + 1);
            int black_point_1_y = rnd.Next(black_point_1_y_min, black_point_1_y_max + 1);

            int black_point_2_x = black_point_1_x;
            int black_point_2_y = black_point_1_y + 9;

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

            int black_point_14_x = rnd.Next(black_point_14_x_min, black_point_14_x_max + 1);
            int black_point_14_y = rnd.Next(black_point_14_y_min, black_point_14_y_max + 1);

            int black_point_15_x = rnd.Next(black_point_15_x_min, black_point_15_x_max + 1);
            int black_point_15_y = rnd.Next(black_point_15_y_min, black_point_15_y_max + 1);

            int black_point_16_x = rnd.Next(black_point_16_x_min, black_point_16_x_max + 1);
            int black_point_16_y = rnd.Next(black_point_16_y_min, black_point_16_y_max + 1);

            if (number_of_lines > 1)
            {
                black_point_2_y += 6;
                black_point_3_y += 23;
                black_point_4_y += 22;
                black_point_5_y += 33;
                black_point_6_y += 26;
                black_point_7_y += 46;
                black_point_8_y += 46;
                black_point_9_y -= 14;
                black_point_10_y -= 23;
                black_point_11_y -= 23;
                black_point_12_y -= 10;
            }

            // Randomly set the X and Y values of the thirteen points of the inner white vector based on the set black point X & Y values.
            int white_point_1_x = rnd.Next(black_point_1_x + 15, black_point_1_x + 16);
            int white_point_1_y = rnd.Next(black_point_1_y + 4, black_point_1_y + 5);

            int white_point_2_x = rnd.Next(black_point_3_x + 3, black_point_3_x + 6);
            int white_point_2_y = rnd.Next(black_point_3_y - 17, black_point_3_y - 13);

            int white_point_3_x = rnd.Next(black_point_4_x + 4, black_point_4_x + 7);
            int white_point_3_y = rnd.Next(black_point_4_y - 15, black_point_4_y - 10);

            int white_point_4_x = rnd.Next(black_point_5_x - 3, black_point_5_x + 3);
            int white_point_4_y = rnd.Next(black_point_5_y - 15, black_point_5_y - 10);

            int white_point_5_x = rnd.Next(black_point_6_x - 10, black_point_6_x - 6);
            int white_point_5_y = rnd.Next(black_point_6_y - 8, black_point_6_y - 5);

            int white_point_6_x = rnd.Next(black_point_8_x - 17, black_point_8_x - 12);
            int white_point_6_y = rnd.Next(black_point_8_y - 1, black_point_8_y - 1);

            int white_point_7_x = rnd.Next(black_point_10_x - 10, black_point_10_x - 8);
            int white_point_7_y = rnd.Next(black_point_10_y + 14, black_point_10_y + 16);

            int white_point_8_x = rnd.Next(black_point_12_x + 5, black_point_12_x + 8);
            int white_point_8_y = rnd.Next(black_point_12_y + 7, black_point_12_y + 9);

            int white_point_9_x = rnd.Next(black_point_13_x + 12, black_point_13_x + 16);
            int white_point_9_y = rnd.Next(black_point_13_y + 15, black_point_13_y + 17);

            int white_point_10_x = rnd.Next(black_point_14_x + 6, black_point_14_x + 13);
            int white_point_10_y = rnd.Next(black_point_14_y + 8, black_point_14_y + 20);

            int white_point_11_x = rnd.Next(black_point_16_x + 13, black_point_16_x + 22);
            int white_point_11_y = rnd.Next(black_point_16_y + 12, black_point_16_y + 15);

            // Randomly set the X and Y values of the thirteen points of the innermost black vector (we'll call it 'void' here) based on the set white point X & Y values.
            int void_point_1_x = rnd.Next(white_point_1_x + 32, white_point_1_x + 38);
            int void_point_1_y = rnd.Next(white_point_1_y + 16, white_point_1_y + 20);

            int void_point_2_x = rnd.Next(white_point_2_x - 4, white_point_2_x + 2);
            int void_point_2_y = rnd.Next(white_point_2_y - 15, white_point_2_y - 12);

            int void_point_3_x = rnd.Next(white_point_3_x - 3, white_point_3_x + 0);
            int void_point_3_y = rnd.Next(white_point_3_y - 17, white_point_3_y - 16);

            int void_point_4_x = rnd.Next(white_point_4_x - 9, white_point_4_x - 4);
            int void_point_4_y = rnd.Next(white_point_4_y - 18, white_point_4_y - 14);

            int void_point_5_x = rnd.Next(white_point_5_x - 7, white_point_5_x - 3);
            int void_point_5_y = rnd.Next(white_point_5_y - 14, white_point_5_y - 12);

            int void_point_6_x = rnd.Next(white_point_6_x - 9, white_point_6_x - 7);
            int void_point_6_y = rnd.Next(white_point_6_y - 6, white_point_6_y - 4);

            int void_point_7_x = rnd.Next(white_point_7_x - 14, white_point_7_x - 11);
            int void_point_7_y = rnd.Next(white_point_7_y + 12, white_point_7_y + 15);

            int void_point_8_x = rnd.Next(white_point_8_x + 6, white_point_8_x + 8);
            int void_point_8_y = rnd.Next(white_point_8_y + 8, white_point_8_y + 10);

            int void_point_9_x = rnd.Next(white_point_9_x + 6, white_point_9_x + 10);
            int void_point_9_y = rnd.Next(white_point_9_y + 8, white_point_9_y + 11);

            int void_point_10_x = rnd.Next(white_point_10_x + 8, white_point_10_x + 11);
            int void_point_10_y = rnd.Next(white_point_10_y + 18, white_point_10_y + 25);

            int void_point_11_x = rnd.Next(white_point_11_x + 3, white_point_11_x + 8);
            int void_point_11_y = rnd.Next(white_point_11_y + 13, white_point_11_y + 18);

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
            Point black_point_14 = new Point(black_point_14_x, black_point_14_y);
            Point black_point_15 = new Point(black_point_15_x, black_point_15_y);
            Point black_point_16 = new Point(black_point_16_x, black_point_16_y);

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
                    black_point_13,
                    black_point_14,
                    black_point_15,
                    black_point_16 };

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
                    white_point_11 };

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

            rotated_void_layer = void_layer;

            // Let's merge the black and white layers into one bitmap.
            using (Graphics graphics = Graphics.FromImage(black_white_layer))
            {
                // Set the graphics rendering to have antialiasing.
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                black_layer = (Bitmap)Set_Image_Opacity(black_layer, (float)0.85);

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

            message_window_point_of_rotation = black_point_12;
            cursor_x_coord = black_point_10.X - 1384;

            // Return the base template.
            return base_template;
        }

        public Bitmap Render_Spriteless_Nametag_Window(string display_name)
        {
            Bitmap base_template = new Bitmap(template_width, template_height);
            Bitmap black_layer = new Bitmap(template_width, template_height);
            Bitmap white_layer = new Bitmap(template_width, template_height);

            Bitmap display_name_layer = Render_Name(display_name);

            // Create a brush for the color white.
            SolidBrush blackBrush = new SolidBrush(System.Drawing.Color.Black);

            // Create a brush for the color white.
            SolidBrush whiteBrush = new SolidBrush(System.Drawing.Color.White);

            // How the vectors are rendered is strongly determined
            int default_line_length = 220;

            // Create multiple variables for the potential min and max values of the 13 black outer points of the nametag window.
            int black_point_1_x_min = 303;
            int black_point_1_x_max = 317;
            int black_point_1_y_min = 683;
            int black_point_1_y_max = 686;

            int black_point_2_x_min = 615;
            int black_point_2_x_max = 621;
            int black_point_2_y_min = 675;
            int black_point_2_y_max = 675;

            int black_point_3_x_min = 642;
            int black_point_3_x_max = 654;
            int black_point_3_y_min = 769;
            int black_point_3_y_max = 770;

            int black_point_4_x_min = 519;
            int black_point_4_x_max = 523;
            int black_point_4_y_min = 757;
            int black_point_4_y_max = 759;

            int black_point_5_x_min = 520;
            int black_point_5_x_max = 525;
            int black_point_5_y_min = 762;
            int black_point_5_y_max = 767;

            int black_point_6_x_min = 465;
            int black_point_6_x_max = 466;
            int black_point_6_y_min = 756;
            int black_point_6_y_max = 764;

            int black_point_7_x_min = 491;
            int black_point_7_x_max = 492;
            int black_point_7_y_min = 800;
            int black_point_7_y_max = 806;

            int black_point_8_x_min = 453;
            int black_point_8_x_max = 455;
            int black_point_8_y_min = 783;
            int black_point_8_y_max = 788;

            int black_point_9_x_min = 472;
            int black_point_9_x_max = 473;
            int black_point_9_y_min = 830;
            int black_point_9_y_max = 832;

            int black_point_10_x_min = 403;
            int black_point_10_x_max = 404;
            int black_point_10_y_min = 752;
            int black_point_10_y_max = 760;

            int black_point_11_x_min = 412;
            int black_point_11_x_max = 415;
            int black_point_11_y_min = 752;
            int black_point_11_y_max = 757;

            int black_point_12_x_min = 406;
            int black_point_12_x_max = 412;
            int black_point_12_y_min = 747;
            int black_point_12_y_max = 750;

            int black_point_13_x_min = 326;
            int black_point_13_x_max = 327;
            int black_point_13_y_min = 740;
            int black_point_13_y_max = 743;

            int name_render_point_x = 0;

            if (display_name_layer.Width <= default_line_length)
            {
                name_render_point_x = black_point_1_x_min + 177 - (display_name_layer.Width / 2);
            }
            else
            {
                name_render_point_x = black_point_1_x_min + 57;

                black_point_2_x_min = name_render_point_x + display_name_layer.Width + 10;
                black_point_2_x_max = black_point_2_x_min + 6;

                black_point_3_x_min = black_point_2_x_min + 25;
                black_point_3_x_max = black_point_3_x_min + 12;
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
            //int black_point_9_x = black_point_9_x_max + 1;
            //int black_point_9_y = black_point_9_y_max + 1;

            int black_point_10_x = rnd.Next(black_point_10_x_min, black_point_10_x_max + 1);
            int black_point_10_y = rnd.Next(black_point_10_y_min, black_point_10_y_max + 1);

            int black_point_11_x = rnd.Next(black_point_11_x_min, black_point_11_x_max + 1);
            int black_point_11_y = rnd.Next(black_point_11_y_min, black_point_11_y_max + 1);

            int black_point_12_x = rnd.Next(black_point_12_x_min, black_point_12_x_max + 1);
            int black_point_12_y = rnd.Next(black_point_12_y_min, black_point_12_y_max + 1);

            int black_point_13_x = rnd.Next(black_point_13_x_min, black_point_13_x_max + 1);
            int black_point_13_y = rnd.Next(black_point_13_y_min, black_point_13_y_max + 1);

            // This conditional helps expand and angle black point #3 properly, close to how it's seen in-game.
            // Based on the positioning of black point #4.
            if (display_name_layer.Width <= default_line_length)
            {
                // Do nothing
            }
            else
            {
                black_point_3_x = rnd.Next(black_point_3_x_min, black_point_3_x_max + 1);
                black_point_3_y = rnd.Next(black_point_4_y_min + ((black_point_3_x - black_point_4_x) / 11), black_point_4_y_max + ((black_point_3_x - black_point_4_x) / 11) + 1);
            }

            // Randomly set the X and Y values of the thirteen points of the inner white vector based on the set black point X & Y values.
            int white_point_1_x = rnd.Next(black_point_1_x + 10, black_point_1_x + 15);
            int white_point_1_y = rnd.Next(black_point_1_y + 6, black_point_1_y + 10);

            int white_point_2_x = rnd.Next(black_point_2_x - 13, black_point_2_x - 8);
            int white_point_2_y = rnd.Next(black_point_2_y + 8, black_point_2_y + 13);

            int white_point_3_x = rnd.Next(black_point_3_x - 22, black_point_3_x - 17);
            int white_point_3_y = rnd.Next(black_point_3_y - 18, black_point_3_y - 15);

            int white_point_4_x = rnd.Next(black_point_4_x - 22, black_point_4_x - 18);
            int white_point_4_y = rnd.Next(black_point_4_y - 13, black_point_4_y - 10);

            int white_point_5_x = rnd.Next(black_point_5_x - 25, black_point_5_x - 20);
            int white_point_5_y = rnd.Next(black_point_5_y - 7, black_point_5_y - 5);

            int white_point_6_x = rnd.Next(black_point_6_x - 12, black_point_6_x - 11);
            int white_point_6_y = rnd.Next(black_point_6_y - 10, black_point_6_y - 9);

            int white_point_7_x = rnd.Next(black_point_7_x - 15, black_point_7_x - 11);
            int white_point_7_y = rnd.Next(black_point_7_y - 17, black_point_7_y - 14);

            int white_point_8_x = rnd.Next(black_point_8_x - 13, black_point_8_x - 10);
            int white_point_8_y = rnd.Next(black_point_8_y - 14, black_point_8_y - 12);

            int white_point_9_x = rnd.Next(black_point_9_x - 15, black_point_9_x - 11);
            int white_point_9_y = rnd.Next(black_point_9_y - 23, black_point_9_y - 14);

            int white_point_10_x = rnd.Next(black_point_10_x + 11, black_point_10_x + 13);
            int white_point_10_y = rnd.Next(black_point_10_y + 6, black_point_10_y + 9);

            int white_point_11_x = rnd.Next(black_point_11_x + 13, black_point_11_x + 15);
            int white_point_11_y = rnd.Next(black_point_11_y + 7, black_point_11_y + 10);

            int white_point_12_x = rnd.Next(black_point_12_x + 1, black_point_12_x + 5);
            int white_point_12_y = rnd.Next(black_point_12_y - 8, black_point_12_y - 6);

            int white_point_13_x = rnd.Next(black_point_13_x + 10, black_point_13_x + 22);
            int white_point_13_y = rnd.Next(black_point_13_y - 5, black_point_13_y - 4);

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

            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                // Set the graphics rendering to have antialiasing.
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                black_layer = (Bitmap)Set_Image_Opacity(black_layer, (float)0.85);

                graphics.DrawImage(black_layer, 0, 0, template_width, template_height);
                graphics.DrawImage(white_layer, 0, 0, template_width, template_height);
            }

            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                // Set the graphics rendering to have antialiasing.
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                graphics.DrawImage(display_name_layer, name_render_point_x, black_point_1_y_min + 10, display_name_layer.Width, display_name_layer.Height);
            }

            spriteless_nametag_point_of_rotation = white_point_1;

            // Return the base template.
            return base_template;
        }

        public Bitmap Render_Spriteless_Message_Window(UserInfoFields account, List<string>[] input_list_array)
        {
            // NOTE: Points are determined from the top leftmost point of the message window, going clockwise

            int number_of_lines = Get_Number_Of_Lines(input_list_array);
            int length_of_longest_line = Get_Max_Line_Length(input_list_array);

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

            // How the vectors are rendered is strongly determined by the amount of text the user inputs.
            int default_line_length = 400;
            int base_gap_length_past_default = 302;
            int starting_dialogue_position = 686;

            // Create multiple variables for the potential min and max values of the seven black outer points of the message window.
            int black_point_1_x_min = 587;
            int black_point_1_x_max = 593;
            int black_point_1_y_min = 984;
            int black_point_1_y_max = 986;

            int black_point_2_x_min = 1384;
            int black_point_2_x_max = 1384;
            int black_point_2_y_min = 1043;
            int black_point_2_y_max = 1043;

            int black_point_3_x_min = 1431;
            int black_point_3_x_max = 1431;
            int black_point_3_y_min = 1034;
            int black_point_3_y_max = 1034;

            int black_point_4_x_min = 1385;
            int black_point_4_x_max = 1385;
            int black_point_4_y_min = 803;
            int black_point_4_y_max = 805;

            int black_point_5_x_min = 1384;
            int black_point_5_x_max = 1384;
            int black_point_5_y_min = 767;
            int black_point_5_y_max = 767;

            int black_point_6_x_min = 1300;
            int black_point_6_x_max = 1300;
            int black_point_6_y_min = 782;
            int black_point_6_y_max = 782;

            int black_point_7_x_min = 599;
            int black_point_7_x_max = 599;
            int black_point_7_y_min = 848;
            int black_point_7_y_max = 848;

            if (length_of_longest_line > default_line_length)
            {
                double[] gap_factors = { 0.3, 0.35, 0.4, 0.45, 0.5 };
                double gap_multiplier = gap_factors[rnd.Next(0, gap_factors.Length)];

                if (length_of_longest_line >= 700)
                {
                    gap_multiplier = 0.5;
                }

                int adjusted_black_point_5_x = starting_dialogue_position + length_of_longest_line + (int)(base_gap_length_past_default - ((length_of_longest_line - default_line_length) * gap_multiplier));

                black_point_2_x_min = adjusted_black_point_5_x;
                black_point_2_x_max = adjusted_black_point_5_x;
                black_point_2_y_min = 1043;
                black_point_2_y_max = 1043;

                black_point_3_x_min = adjusted_black_point_5_x + 47;
                black_point_3_x_max = adjusted_black_point_5_x + 47;
                black_point_3_y_min = 1034;
                black_point_3_y_max = 1034;

                black_point_4_x_min = adjusted_black_point_5_x + 1;
                black_point_4_x_max = adjusted_black_point_5_x + 1;
                black_point_4_y_min = 803;
                black_point_4_y_max = 805;

                black_point_5_x_min = adjusted_black_point_5_x;
                black_point_5_x_max = adjusted_black_point_5_x;
                black_point_5_y_min = 767;
                black_point_5_y_max = 767;

                black_point_6_x_min = adjusted_black_point_5_x - 84;
                black_point_6_x_max = adjusted_black_point_5_x - 84;
                black_point_6_y_min = 782;
                black_point_6_y_max = 782;
            }
            else
            {
                // Do nothing
            }

            int alt_x = -74;
            int alt_y = 10;

            // Randomly set the X and Y values of the outer seven points of the vector using the min and max values.
            int black_point_1_x = rnd.Next(black_point_1_x_min, black_point_1_x_max + 1) + alt_x;
            int black_point_1_y = rnd.Next(black_point_1_y_min, black_point_1_y_max + 1) + alt_y;

            int black_point_2_x = rnd.Next(black_point_2_x_min, black_point_2_x_max + 1) + alt_x;
            int black_point_2_y = rnd.Next(black_point_2_y_min, black_point_2_y_max + 1) + alt_y;

            int black_point_3_x = rnd.Next(black_point_3_x_min, black_point_3_x_max + 1) + alt_x;
            int black_point_3_y = rnd.Next(black_point_3_y_min, black_point_3_y_max + 1) + alt_y;

            int black_point_4_x = rnd.Next(black_point_4_x_min, black_point_4_x_max + 1) + alt_x;
            int black_point_4_y = rnd.Next(black_point_4_y_min, black_point_4_y_max + 1) + alt_y;

            int black_point_5_x = rnd.Next(black_point_5_x_min, black_point_5_x_max + 1) + alt_x;
            int black_point_5_y = rnd.Next(black_point_5_y_min, black_point_5_y_max + 1) + alt_y;

            int black_point_6_x = rnd.Next(black_point_6_x_min, black_point_6_x_max + 1) + alt_x;
            int black_point_6_y = rnd.Next(black_point_6_y_min, black_point_6_y_max + 1) + alt_y;

            int black_point_7_x = rnd.Next(black_point_7_x_min, black_point_7_x_max + 1) + alt_x;
            int black_point_7_y = rnd.Next(black_point_7_y_min, black_point_7_y_max + 1) + alt_y;

            if (number_of_lines > 1)
            {
                black_point_1_y += 26;
                black_point_2_y += 46;
                black_point_3_y += 46;
                black_point_4_y -= 14;
                black_point_5_y -= 23;
                black_point_6_y -= 23;
                black_point_7_y -= 10;
            }

            // Randomly set the X and Y values of the thirteen points of the inner white vector based on the set black point X & Y values.
            int white_point_1_x = rnd.Next(black_point_1_x + 8, black_point_1_x + 12);
            int white_point_1_y = rnd.Next(black_point_1_y - 2, black_point_1_y - 1);

            int white_point_2_x = rnd.Next(black_point_3_x - 17, black_point_3_x - 12);
            int white_point_2_y = rnd.Next(black_point_3_y - 1, black_point_3_y - 1);

            int white_point_3_x = rnd.Next(black_point_5_x - 10, black_point_5_x - 8);
            int white_point_3_y = rnd.Next(black_point_5_y + 14, black_point_5_y + 16);

            int white_point_4_x = rnd.Next(black_point_7_x + 5, black_point_7_x + 8);
            int white_point_4_y = rnd.Next(black_point_7_y + 7, black_point_7_y + 9);

            // Randomly set the X and Y values of the thirteen points of the innermost black vector (we'll call it 'void' here) based on the set white point X & Y values.
            int void_point_1_x = rnd.Next(white_point_1_x + 7, white_point_1_x + 11);
            int void_point_1_y = rnd.Next(white_point_1_y - 14, white_point_1_y - 12);

            int void_point_2_x = rnd.Next(white_point_2_x - 9, white_point_2_x - 7);
            int void_point_2_y = rnd.Next(white_point_2_y - 6, white_point_2_y - 4);

            int void_point_3_x = rnd.Next(white_point_3_x - 14, white_point_3_x - 11);
            int void_point_3_y = rnd.Next(white_point_3_y + 12, white_point_3_y + 15);

            int void_point_4_x = rnd.Next(white_point_4_x + 6, white_point_4_x + 8);
            int void_point_4_y = rnd.Next(white_point_4_y + 8, white_point_4_y + 10);

            // Create the thirteen points of the black vector from the randomly chosen values.
            Point black_point_1 = new Point(black_point_1_x, black_point_1_y);
            Point black_point_2 = new Point(black_point_2_x, black_point_2_y);
            Point black_point_3 = new Point(black_point_3_x, black_point_3_y);
            Point black_point_4 = new Point(black_point_4_x, black_point_4_y);
            Point black_point_5 = new Point(black_point_5_x, black_point_5_y);
            Point black_point_6 = new Point(black_point_6_x, black_point_6_y);
            Point black_point_7 = new Point(black_point_7_x, black_point_7_y);

            // Create the thirteen points of the white vector from the randomly chosen values.
            Point white_point_1 = new Point(white_point_1_x, white_point_1_y);
            Point white_point_2 = new Point(white_point_2_x, white_point_2_y);
            Point white_point_3 = new Point(white_point_3_x, white_point_3_y);
            Point white_point_4 = new Point(white_point_4_x, white_point_4_y);

            // Create the thirteen points of the void vector from the randomly chosen values.
            Point void_point_1 = new Point(void_point_1_x, void_point_1_y);
            Point void_point_2 = new Point(void_point_2_x, void_point_2_y);
            Point void_point_3 = new Point(void_point_3_x, void_point_3_y);
            Point void_point_4 = new Point(void_point_4_x, void_point_4_y);

            // Add all the points for the outer black vector into a point array.
            Point[] black_poly_points = {
                    black_point_1,
                    black_point_2,
                    black_point_3,
                    black_point_4,
                    black_point_5,
                    black_point_6,
                    black_point_7 };

            // Add all the points for the inner white vector into a point array.
            Point[] white_poly_points = {
                    white_point_1,
                    white_point_2,
                    white_point_3,
                    white_point_4 };

            // Add all the points for the innermost void vector into a point array.
            Point[] void_poly_points = {
                    void_point_1,
                    void_point_2,
                    void_point_3,
                    void_point_4 };

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

            rotated_void_layer = void_layer;

            // Let's merge the black and white layers into one bitmap.
            using (Graphics graphics = Graphics.FromImage(black_white_layer))
            {
                // Set the graphics rendering to have antialiasing.
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                black_layer = (Bitmap)Set_Image_Opacity(black_layer, (float)0.85);

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

            message_window_point_of_rotation = new Point(black_point_7.X, black_point_7.Y);
            cursor_x_coord = black_point_5.X - 1384;

            // Return the base template.
            return base_template;
        }

        public static Bitmap Render_Boxed_Letter(Bitmap glyph, ParsingFields glyph_info, char input_char, int boxed_char_count)
        {
            Bitmap base_template = new Bitmap(48, 54);
            char[] narrow_chars = new char[] { 'i', 'l', 'r', 'u' };

            System.Drawing.Color box_bg_color = default;

            if (boxed_char_count == 2)
            {
                box_bg_color = System.Drawing.Color.FromArgb(51, 51, 51);
            }
            else
            {
                box_bg_color = System.Drawing.Color.Black;
            }

            SolidBrush box_brush = new SolidBrush(box_bg_color);

            Point box_point_1 = new Point(glyph_info.LeftCut - 2, 2);
            Point box_point_2 = new Point(glyph_info.RightCut + 2, 2);
            Point box_point_3 = new Point(glyph_info.RightCut + 2, 52);
            Point box_point_4 = new Point(glyph_info.LeftCut - 2, 52);

            if (narrow_chars.Contains(input_char))
            {
                box_point_2 = new Point(glyph_info.RightCut + 1, 2);
                box_point_3 = new Point(glyph_info.RightCut + 1, 52);
            }

            Point[] box_poly_points = {
                    box_point_1,
                    box_point_2,
                    box_point_3,
                    box_point_4 };

            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                graphics.FillPolygon(box_brush, box_poly_points);

                graphics.DrawImage(glyph, 0, 0, 48, 48);
            }

            return base_template;
        }

        public Bitmap Render_Manual_Advance_Tick()
        {
            // We'll need to create three layers: A base one, a layer for the white vector, and a layer for the black vector.
            Bitmap base_template = new Bitmap(template_width, template_height);
            Bitmap white_layer = new Bitmap(template_width, template_height);
            Bitmap black_layer = new Bitmap(template_width, template_height);
            Bitmap merged_layer = new Bitmap(template_width, template_height);

            // Create a brush for the color white.
            SolidBrush blackBrush = new SolidBrush(System.Drawing.Color.Black);

            // Create a brush for the color white.
            SolidBrush whiteBrush = new SolidBrush(System.Drawing.Color.White);

            int scale_factor = rnd.Next(0, 13);

            // Create the four points of the black vector from the randomly chosen values.
            Point black_point_1 = new Point(1469 - (scale_factor * 5), (int)(826 + (scale_factor * 5)));
            Point black_point_2 = new Point((int)(1577 - (scale_factor * 7.5)), (int)(929 + (scale_factor * 2.75)));
            Point black_point_3 = new Point((int)(1300 - (scale_factor * 0.6)), (int)(1016 + (scale_factor * 0.6)));
            Point black_point_4 = new Point((int)(1278 - (scale_factor * 0.8)), (int)(1012 + (scale_factor * 0.8)));

            // Create the four points of the white vector from the randomly chosen values.
            Point white_point_1 = new Point((int)(black_point_1.X + 4 - (scale_factor * 0.17)), (int)(black_point_1.Y + 19 - (scale_factor * 0.5)));
            Point white_point_2 = new Point((int)(black_point_2.X - 36 + (scale_factor * 1.2)), (int)(black_point_2.Y - 6 + (scale_factor * 0.12)));
            Point white_point_3 = new Point((int)(black_point_3.X + 15 - (scale_factor * 0.5)), (int)(black_point_3.Y - 10 + (scale_factor * 0.17)));
            Point white_point_4 = new Point((int)(black_point_4.X + 15 - (scale_factor * 0.5)), (int)(black_point_4.Y - 8 + (scale_factor * 0.25)));

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
            using (Graphics graphics = Graphics.FromImage(merged_layer))
            {
                // Set the graphics rendering to have antialiasing.
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                black_layer = (Bitmap)Set_Image_Opacity(black_layer, (float)0.85);

                // Draw the two layers to the template.
                graphics.DrawImage(black_layer, 0, 0, template_width, template_height);
                graphics.DrawImage(white_layer, 0, 0, template_width, template_height);
            }

            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                int rotation_angle = rnd.Next(-2, 3);
                merged_layer = Rotate_Image_On_Point(merged_layer, rotation_angle, white_point_4.X, white_point_4.Y, false);

                int choice = rnd.Next(0, 10);

                if (scale_factor == 0 && choice >= 7)
                {
                    graphics.DrawImage(merged_layer, -30, 30, template_width, template_height);
                }
                else
                {
                    graphics.DrawImage(merged_layer, 0, 0, template_width, template_height);
                }
            }

            // Return the new bitmap.
            return base_template;
        }

        public Bitmap Rotate_And_Place_Nametag(Bitmap window_layer, Point point_of_rotation)
        {
            Bitmap base_template = new Bitmap(template_width, template_height);

            int randomized_int = rnd.Next(-19, -12);
            double[] adjustment_array = new double[] { 0, 0.25, 0.50, 0.75, 1 };
            int double_index = rnd.Next(0, adjustment_array.Length);

            float randomized_angle = (float)(randomized_int + adjustment_array[double_index]);

            window_layer = Rotate_Image_On_Point(window_layer, randomized_angle, point_of_rotation.X, point_of_rotation.Y, false);

            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                graphics.DrawImage(window_layer, 78 + 3, 144 - 3, window_layer.Width, window_layer.Height);
            }

            return base_template;
        }

        public Bitmap Rotate_And_Place_Spriteless_Nametag(Bitmap window_layer, Point point_of_rotation, int rotation_min, int rotation_max)
        {
            Bitmap base_template = new Bitmap(template_width, template_height);

            int randomized_int = rnd.Next(rotation_min, rotation_max);
            double[] adjustment_array = new double[] { 0, 0.25, 0.50, 0.75, 1 };
            int double_index = rnd.Next(0, adjustment_array.Length);

            float randomized_angle = (float)(randomized_int + adjustment_array[double_index]);

            //randomized_angle = (float)-15;

            window_layer = Rotate_Image_On_Point(window_layer, randomized_angle, point_of_rotation.X, point_of_rotation.Y, false);

            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                graphics.DrawImage(window_layer, 66, 88, window_layer.Width, window_layer.Height);
            }

            return base_template;
        }

        // Helper Methods
        public static bool Is_Boxed_Letter(int name_length, int current_index)
        {
            switch (name_length)
            {
                case 0:
                    return false;

                case 1:
                    return false;

                case 2:
                    if (current_index == 1)
                    {
                        return true;
                    }
                    break;

                case 3:
                    if (current_index == 1)
                    {
                        return true;
                    }
                    break;

                case 4:
                    if (current_index == 1)
                    {
                        return true;
                    }
                    break;

                case 5:
                    if (current_index == 1)
                    {
                        return true;
                    }
                    break;

                case 6:
                    if (current_index == 1)
                    {
                        return true;
                    }
                    break;

                case 7:
                    if (current_index == 2)
                    {
                        return true;
                    }
                    break;

                case 8:
                    if (current_index == 2 || current_index == 7)
                    {
                        return true;
                    }
                    break;

                case 9:
                    if (current_index == 2 || current_index == 7)
                    {
                        return true;
                    }
                    break;

                case 10:
                    if (current_index == 2 || current_index == 7)
                    {
                        return true;
                    }
                    break;

                case 11:
                    if (current_index == 0 || current_index == 5)
                    {
                        return true;
                    }
                    break;

                case 12:
                    if (current_index == 0 || current_index == 5)
                    {
                        return true;
                    }
                    break;

                case 13:
                    if (current_index == 0 || current_index == 5)
                    {
                        return true;
                    }
                    break;

                case 14:
                    if (current_index == 0 || current_index == 5 || current_index == 13)
                    {
                        return true;
                    }
                    break;

                case 15:
                    if (current_index == 0 || current_index == 5 || current_index == 13)
                    {
                        return true;
                    }
                    break;

                case 16:
                    if (current_index == 0 || current_index == 5 || current_index == 13)
                    {
                        return true;
                    }
                    break;

                case 17:
                    if (current_index == 0 || current_index == 5 || current_index == 13)
                    {
                        return true;
                    }
                    break;

                case 18:
                    if (current_index == 0 || current_index == 5 || current_index == 13)
                    {
                        return true;
                    }
                    break;

                case 19:
                    if (current_index == 0 || current_index == 5 || current_index == 13)
                    {
                        return true;
                    }
                    break;

                case 20:
                    if (current_index == 0 || current_index == 5 || current_index == 13 || current_index == 19)
                    {
                        return true;
                    }
                    break;

                case 21:
                    if (current_index == 1 || current_index == 9)
                    {
                        return true;
                    }
                    break;

                case 22:
                    if (current_index == 1 || current_index == 9)
                    {
                        return true;
                    }
                    break;

                case 23:
                    if (current_index == 1 || current_index == 9)
                    {
                        return true;
                    }
                    break;

                case 24:
                    if (current_index == 1 || current_index == 9 || current_index == 23)
                    {
                        return true;
                    }
                    break;

                case 25:
                    if (current_index == 1 || current_index == 9 || current_index == 23)
                    {
                        return true;
                    }
                    break;

                case 26:
                    if (current_index == 1 || current_index == 9 || current_index == 23)
                    {
                        return true;
                    }
                    break;

                default:
                    if (current_index == 1 || current_index == 9 || current_index == 23)
                    {
                        return true;
                    }
                    break;
            }

            return false;
        }

        public static int Get_Number_Of_Lines(List<string>[] input_list_array)
        {
            int line_counter = 0;

            for (int line_index = 0; line_index < input_list_array.Length; line_index++)
            {
                string current_list = String_List_To_String(input_list_array[line_index]);

                if (!string.IsNullOrEmpty(current_list))
                {
                    line_counter++;
                }
            }

            return line_counter;
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

                int current_string_length = Measure_Word_Pixel_Length(null, current_list);

                if (current_string_length > max_line_length)
                {
                    max_line_length = current_string_length;
                }
            }

            return max_line_length;
        }

        // Method from https://stackoverflow.com/questions/58086523/rotate-bitmap-around-point-and-make-that-point-the-new-center
        Bitmap Rotate_Image_On_Point(Bitmap img, float angle, int cx, int cy, bool is_centered_at_point) // cx & cy = point to rotate around
        {
            Bitmap result = new Bitmap(img.Width, img.Height);
            int mx = img.Width / 2,
                my = img.Height / 2;
            using (Graphics g = Graphics.FromImage(result))
            {
                g.TranslateTransform(cx, cy);
                g.RotateTransform(angle);
                g.TranslateTransform(-cx, -cy);

                if (is_centered_at_point == true)
                {
                    g.TranslateTransform(mx - cx, my - cy, MatrixOrder.Append);
                }

                g.DrawImage(img, new Point(0, 0));
            }
            return result;
        }

        // Border Rendering
        public Bitmap Render_Screen_Border(UserInfoFields account)
        {
            Bitmap base_template = new Bitmap(template_width, template_height);

            Bitmap border_main = new Bitmap(2, 2);
            Bitmap border_secondary = new Bitmap(2, 2);

            switch (account.P5_PS4_TS_Border)
            {
                case "Event":
                    border_main = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5-PS4//Border//event_main.png");
                    border_secondary = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5-PS4//Border//event_secondary.png");
                    break;

                case "Interaction":
                    border_main = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5-PS4//Border//interaction_main.png");
                    border_secondary = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5-PS4//Border//interaction_secondary.png");
                    break;
            }

            Bitmap shading = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5-PS4//Border//border_shading.png");
            Bitmap star_layer = Render_Star_Layer();

            using (Graphics graphics = Graphics.FromImage(star_layer))
            {
                graphics.DrawImage(shading, 0, 0, template_width, template_height);
            }

            star_layer = Keep_Pixel_Overlap_Stars(border_main, star_layer, new Rectangle(0, 0, template_width, template_height));


            // Now, time to put the template together!
            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                graphics.DrawImage(border_main, 0, 0, template_width, template_height);
                graphics.DrawImage(star_layer, 0, 0, template_width, template_height);
                graphics.DrawImage(border_secondary, 0, 0, template_width, template_height);
            }

            return base_template;
        }

        public static Bitmap Render_Star(double scale_factor, System.Drawing.Color star_color)
        {
            int template_width = 8000;
            int template_height = 8000;

            // Make a new bitmap large enough for a working space.
            Bitmap new_bitmap = new Bitmap(template_width, template_height);

            // Use a graphics object to edit the bitmap.
            using (Graphics graphics = Graphics.FromImage(new_bitmap))
            {
                int alter = 15;

                // Establish the center point of the star.
                Point center_point = new Point(template_width / 2, template_height / 2); // 449, 471

                // Create an array of ints that will establish the X and Y values of each angle of the star. Even array indexes are X values, odd indexes are Y values.
                int[] star_points = new int[] { 0, -227, 58 + alter, -80 - alter, 216, -70, 95 + alter, 32 + alter, 134, 184, 0, 100 + alter, -132, 184, -94 - alter, 32 + alter, -216, -70, -58 - alter, -80 - alter };

                // Edit each array index by multiplying them by the scaling factor.
                for (int i = 0; i < star_points.Length; i++)
                {
                    star_points[i] = (int)(star_points[i] * (scale_factor / 1.5));
                }

                // Create points for the star by adding on the star_point indexes to the center_point coordinates.
                Point point_1 = new Point(center_point.X + star_points[0], center_point.Y + star_points[1]);
                Point point_2 = new Point(center_point.X + star_points[2], center_point.Y + star_points[3]);
                Point point_3 = new Point(center_point.X + star_points[4], center_point.Y + star_points[5]);
                Point point_4 = new Point(center_point.X + star_points[6], center_point.Y + star_points[7]);
                Point point_5 = new Point(center_point.X + star_points[8], center_point.Y + star_points[9]);
                Point point_6 = new Point(center_point.X + star_points[10], center_point.Y + star_points[11]);
                Point point_7 = new Point(center_point.X + star_points[12], center_point.Y + star_points[13]);
                Point point_8 = new Point(center_point.X + star_points[14], center_point.Y + star_points[15]);
                Point point_9 = new Point(center_point.X + star_points[16], center_point.Y + star_points[17]);
                Point point_10 = new Point(center_point.X + star_points[18], center_point.Y + star_points[19]);

                // Create a color for the star to be filled with.
                SolidBrush colorBrush = new SolidBrush(star_color);

                // Add all the points into a point array.
                Point[] polyPoints = { point_1, point_2, point_3, point_4, point_5, point_6, point_7, point_8, point_9, point_10 };

                // Use the point array to create a polygon by connecting all the points together and filling it with color.
                graphics.FillPolygon(colorBrush, polyPoints);
            }

            // Return the new bitmap.
            return new_bitmap;
        }

        public Bitmap Render_Recursive_Star()
        {
            int template_width = 8000;
            int template_height = 8000;

            double start_size = rnd.NextDouble(19.0, 22.0);

            // Make a new bitmap large enough for a working space.
            Bitmap new_bitmap = new Bitmap(template_width, template_height);

            // Use a graphics object to edit the bitmap.
            using (Graphics graphics = Graphics.FromImage(new_bitmap))
            {
                // Create another graphics object. This will establish a cropping region in the shape of a star (for the star itself) to give a greater visual effect.
                using (Graphics region_crop = Graphics.FromImage(new_bitmap))
                {
                    int alter = 15;

                    // Establish the center point of the star.
                    Point center_point = new Point(template_width / 2, template_height / 2);

                    // Create an array of ints that will establish the X and Y values of each angle of the star. Even array indexes are X values, odd indexes are Y values.
                    int[] star_points = new int[] { 0, -227, 58 + alter, -80 - alter, 216, -70, 95 + alter, 32 + alter, 134, 184, 0, 100 + alter, -132, 184, -94 - alter, 32 + alter, -216, -70, -58 - alter, -80 - alter };
                    //int[] star_points = new int[] { 0, -227, 58, -80, 216, -70, 95, 32, 134, 184, 0, 100, -132, 184, -94, 32, -216, -70, -58, -80 };

                    // Edit each array index by multiplying them by 24. Again, 24 must be the lowest point to get eight layers of stars minimum since the stars will be made in decrements of three. 24 divided by 3 is eight.
                    for (int i = 0; i < star_points.Length; i++)
                    {
                        star_points[i] = (int)(star_points[i] * start_size);
                    }

                    // Create points for the star by adding on the star_point indexes to the center_point coordinates.
                    Point point_1 = new Point(center_point.X + star_points[0], center_point.Y + star_points[1]);
                    Point point_2 = new Point(center_point.X + star_points[2], center_point.Y + star_points[3]);
                    Point point_3 = new Point(center_point.X + star_points[4], center_point.Y + star_points[5]);
                    Point point_4 = new Point(center_point.X + star_points[6], center_point.Y + star_points[7]);
                    Point point_5 = new Point(center_point.X + star_points[8], center_point.Y + star_points[9]);
                    Point point_6 = new Point(center_point.X + star_points[10], center_point.Y + star_points[11]);
                    Point point_7 = new Point(center_point.X + star_points[12], center_point.Y + star_points[13]);
                    Point point_8 = new Point(center_point.X + star_points[14], center_point.Y + star_points[15]);
                    Point point_9 = new Point(center_point.X + star_points[16], center_point.Y + star_points[17]);
                    Point point_10 = new Point(center_point.X + star_points[18], center_point.Y + star_points[19]);

                    // Add all the points into a point array.
                    Point[] polyPoints = { point_1, point_2, point_3, point_4, point_5, point_6, point_7, point_8, point_9, point_10 };

                    // Use the point array to create a path and connect the points together
                    GraphicsPath path = new GraphicsPath();
                    path.AddPolygon(polyPoints);

                    // Construct a region based on the path
                    Region region = new Region(path);

                    // Set the clipping region of the Graphics object
                    region_crop.SetClip(region, CombineMode.Replace);

                    // Now, we start creating the layers of the star itself. 
                    // Based on the random size determined earlier, create stars of alternating colors while decrementing in size.
                    for (double i = start_size; i > 0; i = i - 3) //3
                    {
                        // start_point_int casts the current double to an int for rounding purposes.
                        double start_point_int = i;

                        // If the double is even, color the star either black or gray depinding on the star type specified. If it's odd, color it white.
                        if ((int)start_point_int % 2 == 0)
                        {
                            region_crop.DrawImage(Render_Star(i, System.Drawing.Color.Black), 0, 0, template_width, template_height);
                        }
                        else
                        {
                            region_crop.DrawImage(Render_Star(i, System.Drawing.Color.FromArgb(16, 16, 16)), 0, 0, template_width, template_height);
                        }
                    }
                }
            }

            Bitmap smaller_template = new Bitmap(600, 600);

            using (Graphics graphics = Graphics.FromImage(smaller_template))
            {
                graphics.DrawImage(new_bitmap, 0, 0, smaller_template.Width, smaller_template.Height);
            }

            new_bitmap = smaller_template;

            Bitmap star_base = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5-PS4//Border//star_base.png");

            new_bitmap = Keep_Pixel_Overlap_Stars(star_base, new_bitmap, new Rectangle(0, 0, star_base.Width, star_base.Height));

            // Return the new bitmap.
            return new_bitmap;
        }

        public Bitmap Render_Star_Layer()
        {
            Bitmap base_template = new Bitmap(template_width, template_height);

            Bitmap star = Render_Recursive_Star();

            // Top layer
            Bitmap star_1 = Rotate_Image_On_Point(star, -28.50f, star.Width / 2, star.Height / 2, false);
            Bitmap star_2 = Rotate_Image_On_Point(star, -180.00f, star.Width / 2, star.Height / 2, false);
            Bitmap star_3 = Rotate_Image_On_Point(star, 45.00f, star.Width / 2, star.Height / 2, false);
            Bitmap star_4 = Rotate_Image_On_Point(star, -12.00f, star.Width / 2, star.Height / 2, false);
            Bitmap star_5 = Rotate_Image_On_Point(star, -20.00f, star.Width / 2, star.Height / 2, false);
            Bitmap star_6 = Rotate_Image_On_Point(star, 32.00f, star.Width / 2, star.Height / 2, false);
            Bitmap star_7 = Rotate_Image_On_Point(star, -175.00f, star.Width / 2, star.Height / 2, false);
            Bitmap star_8 = Rotate_Image_On_Point(star, -10.00f, star.Width / 2, star.Height / 2, false);
            Bitmap star_9 = Rotate_Image_On_Point(star, -10.00f, star.Width / 2, star.Height / 2, false);
            Bitmap star_10 = Rotate_Image_On_Point(star, 30.00f, star.Width / 2, star.Height / 2, false);
            Bitmap star_11 = Rotate_Image_On_Point(star, 13.00f, star.Width / 2, star.Height / 2, false);
            Bitmap star_12 = Rotate_Image_On_Point(star, -10.00f, star.Width / 2, star.Height / 2, false);
            Bitmap star_13 = Rotate_Image_On_Point(star, 0f, star.Width / 2, star.Height / 2, false);
            Bitmap star_14 = Rotate_Image_On_Point(star, 0f, star.Width / 2, star.Height / 2, false);

            // Bottom layer
            Bitmap star_15 = Rotate_Image_On_Point(star, -25.00f, star.Width / 2, star.Height / 2, false);
            Bitmap star_16 = Rotate_Image_On_Point(star, 177.00f, star.Width / 2, star.Height / 2, false);
            Bitmap star_17 = Rotate_Image_On_Point(star, -15.00f, star.Width / 2, star.Height / 2, false);
            Bitmap star_18 = Rotate_Image_On_Point(star, -14.00f, star.Width / 2, star.Height / 2, false);
            Bitmap star_19 = Rotate_Image_On_Point(star, -15.00f, star.Width / 2, star.Height / 2, false);
            Bitmap star_20 = Rotate_Image_On_Point(star, -175.00f, star.Width / 2, star.Height / 2, false);

            Bitmap star_21 = Rotate_Image_On_Point(star, 21.00f, star.Width / 2, star.Height / 2, false);
            Bitmap star_22 = Rotate_Image_On_Point(star, 16.50f, star.Width / 2, star.Height / 2, false);
            Bitmap star_23 = Rotate_Image_On_Point(star, -30.00f, star.Width / 2, star.Height / 2, false);
            Bitmap star_24 = Rotate_Image_On_Point(star, 24.00f, star.Width / 2, star.Height / 2, false);

            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                // Top layer
                graphics.DrawImage(star_14, (template_width / 2) - 842, (template_height / 2) - 732, star_14.Width, star_14.Height);
                graphics.DrawImage(star_13, (template_width / 2) + 108, (template_height / 2) - 631, star_13.Width, star_13.Height);
                graphics.DrawImage(star_12, (template_width / 2) - 1170, (template_height / 2) - 915, star_12.Width, star_12.Height);
                graphics.DrawImage(star_10, (template_width / 2) - 1209, (template_height / 2) - 688, star_10.Width, star_10.Height);
                graphics.DrawImage(star_7, (template_width / 2) - 616, (template_height / 2) - 860, star_7.Width, star_7.Height);
                graphics.DrawImage(star_5, (template_width / 2) - 273, (template_height / 2) - 838, star_5.Width, star_5.Height);
                graphics.DrawImage(star_3, (template_width / 2) + 347, (template_height / 2) - 868, star_3.Width, star_3.Height);
                graphics.DrawImage(star_1, (template_width / 2) + 560, (template_height / 2) - 760, star_1.Width, star_1.Height);
                graphics.DrawImage(star_11, (template_width / 2) - 110, (template_height / 2) - 811, star_11.Width, star_11.Height);
                graphics.DrawImage(star_4, (template_width / 2) - 60, (template_height / 2) - 739, star_4.Width, star_4.Height);
                graphics.DrawImage(star_2, (template_width / 2) + 226, (template_height / 2) - 732, star_2.Width, star_2.Height);
                graphics.DrawImage(star_6, (template_width / 2) - 439, (template_height / 2) - 755, star_6.Width, star_6.Height);
                graphics.DrawImage(star_9, (template_width / 2) - 1030, (template_height / 2) - 779, star_9.Width, star_9.Height);
                graphics.DrawImage(star_8, (template_width / 2) - 839, (template_height / 2) - 915, star_8.Width, star_8.Height);

                // Bottom layer
                graphics.DrawImage(star_15, (template_width / 2) + 580, (template_height / 2) + 178, star_15.Width, star_15.Height);
                graphics.DrawImage(star_16, (template_width / 2) + 136, (template_height / 2) + 229, star_16.Width, star_16.Height);
                graphics.DrawImage(star_17, (template_width / 2) - 1050, (template_height / 2) + 246, star_17.Width, star_17.Height);
                graphics.DrawImage(star_18, (template_width / 2) + 269, (template_height / 2) + 295, star_18.Width, star_18.Height);
                graphics.DrawImage(star_19, (template_width / 2) - 565, (template_height / 2) + 192, star_19.Width, star_19.Height);
                graphics.DrawImage(star_20, (template_width / 2) - 370, (template_height / 2) + 243, star_20.Width, star_20.Height);
                graphics.DrawImage(star_21, (template_width / 2) - 1201, (template_height / 2) + 266, star_21.Width, star_21.Height);
                graphics.DrawImage(star_22, (template_width / 2) - 858, (template_height / 2) + 195, star_22.Width, star_22.Height);
                graphics.DrawImage(star_23, (template_width / 2) - 130, (template_height / 2) + 178, star_23.Width, star_23.Height);
                graphics.DrawImage(star_24, (template_width / 2) + 414, (template_height / 2) + 166, star_24.Width, star_24.Height);
            }

            return base_template;
        }

        public static Bitmap Render_Control_Panel(UserInfoFields account)
        {
            Bitmap base_bitmap = new Bitmap(template_width, template_height);

            Bitmap ffwd_button = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5-PS4//Control_Panel//ffwd.png");
            Bitmap log_button = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5-PS4//Control_Panel//log.png");

            using (Graphics graphics = Graphics.FromImage(base_bitmap))
            {
                graphics.DrawImage(ffwd_button, 0, 0, template_width, template_height);
                graphics.DrawImage(log_button, 0, 0, template_width, template_height);
            }

            return base_bitmap;
        }

        // Calendar Rendering
        public static Bitmap Construct_Calendar(SocialLinkerCommand sl_command, UserInfoFields account)
        {
            // Get the user's current date and time according to their settings.
            DateTime user_time = Get_Date(sl_command, account);

            // Basic calendar
            return Construct_Basic_Calendar(user_time, account);
        }

        public static Bitmap Construct_Basic_Calendar(DateTime user_time, UserInfoFields account)
        {
            // Make an empty bitmap.
            Bitmap bitmap = new Bitmap(template_width, template_height);

            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                // Establish all variables needed and set them to null; they will be assigned to later.
                System.Drawing.Image monthBottom = null;
                System.Drawing.Image monthMiddle = null;
                System.Drawing.Image monthTop = null;

                System.Drawing.Image dayBottom = null;
                System.Drawing.Image dayMiddle = null;
                System.Drawing.Image dayTop = null;

                System.Drawing.Image dayBottom_tens = null;
                System.Drawing.Image dayBottom_ones = null;
                System.Drawing.Image dayMiddle_tens = null;
                System.Drawing.Image dayMiddle_ones = null;
                System.Drawing.Image dayMiddle_filler = null;
                System.Drawing.Image dayTop_tens = null;
                System.Drawing.Image dayTop_ones = null;

                System.Drawing.Image weekdayBottom = null;
                System.Drawing.Image weekdayMiddle = null;
                System.Drawing.Image weekdayTop = null;

                System.Drawing.Image weatherBox = null;
                System.Drawing.Image weatherIcon = null;

                System.Drawing.Image timeOfDay = null;

                // Assign variables that are immediately needed.
                monthBottom = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5-PS4//Calendar//Calendar_Bottom//Month//{user_time.Month}.png");
                monthMiddle = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5-PS4//Calendar//Calendar_Middle//Month//{user_time.Month}.png");
                monthTop = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5-PS4//Calendar//Calendar_Top//Month//{user_time.Month}.png");

                weekdayBottom = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5-PS4//Calendar//Calendar_Bottom//Weekday//{user_time.DayOfWeek}.png");
                weekdayMiddle = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5-PS4//Calendar//Calendar_Middle//Weekday//{user_time.DayOfWeek}.png");
                weekdayTop = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5-PS4//Calendar//Calendar_Top//Weekday//{user_time.DayOfWeek}.png");

                // Use a random variable for the weather icon so it can change in each scene.
                Random w = new Random();
                int wInt = w.Next(1, 4);

                // Assign paths for the weather and time of day variables.
                weatherBox = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5-PS4//Calendar//Weather//weather_box.png");
                weatherIcon = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5-PS4//Calendar//Weather//{Get_Weather(account)}//{wInt}.png");
                timeOfDay = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5-PS4//Calendar//Time_of_Day//{Get_Time_of_Day(user_time)}.png");

                // If the day is less than 10, only use one digit for the day.
                if (user_time.Day < 10)
                {
                    dayBottom = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5-PS4//Calendar//Calendar_Bottom//Day//Single_Digit//{user_time.Day}.png");
                    dayMiddle = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5-PS4//Calendar//Calendar_Middle//Day//Single_Digit//{user_time.Day}.png");
                    dayTop = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5-PS4//Calendar//Calendar_Top//Day//Single_Digit//{user_time.Day}.png");
                }
                // If the day is ten or more, we need two digits for the day.
                else if (user_time.Day >= 10)
                {
                    char[] day = user_time.Day.ToString().ToCharArray();

                    dayBottom_tens = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5-PS4//Calendar//Calendar_Bottom//Day//Double_Digit//Tens_Place//{day[0]}.png");
                    dayBottom_ones = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5-PS4//Calendar//Calendar_Bottom//Day//Double_Digit//Ones_Place//{day[1]}.png");

                    dayMiddle_tens = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5-PS4//Calendar//Calendar_Middle//Day//Double_Digit//Tens_Place//{day[0]}.png");
                    dayMiddle_ones = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5-PS4//Calendar//Calendar_Middle//Day//Double_Digit//Ones_Place//{day[1]}.png");
                    dayMiddle_filler = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5-PS4//Calendar//Calendar_Middle//Day//Double_Digit//middle_filler.png");

                    dayTop_tens = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5-PS4//Calendar//Calendar_Top//Day//Double_Digit//Tens_Place//{day[0]}.png");
                    dayTop_ones = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5-PS4//Calendar//Calendar_Top//Day//Double_Digit//Ones_Place//{day[1]}.png");
                }

                // Create multiple bitmap layers for the calendar.
                Bitmap bottom_layer = new Bitmap(template_width, template_height);
                Bitmap middle_layer = new Bitmap(template_width, template_height);
                Bitmap top_layer = new Bitmap(template_width, template_height);
                Bitmap merged_layer = new Bitmap(template_width, template_height);

                using (Graphics calendar_bottom = Graphics.FromImage(bottom_layer))
                {
                    if (user_time.Day < 10)
                    {
                        calendar_bottom.DrawImage(monthBottom, 0, 0, template_width, template_height);
                        calendar_bottom.DrawImage(dayBottom, 0, 0, template_width, template_height);
                        calendar_bottom.DrawImage(weatherBox, -15, 0, template_width, template_height);
                    }
                    else
                    {
                        calendar_bottom.DrawImage(monthBottom, -30, 0, template_width, template_height);
                        calendar_bottom.DrawImage(dayBottom_ones, 0, 0, template_width, template_height);
                        calendar_bottom.DrawImage(dayBottom_tens, 0, 0, template_width, template_height);
                        calendar_bottom.DrawImage(weatherBox, 0, 0, template_width, template_height);
                    }

                    calendar_bottom.DrawImage(weekdayBottom, 0, 0, template_width, template_height);
                }

                using (Graphics calendar_middle = Graphics.FromImage(middle_layer))
                {
                    if (user_time.Day < 10)
                    {
                        calendar_middle.DrawImage(monthMiddle, 0, 0, template_width, template_height);
                        calendar_middle.DrawImage(dayMiddle, 0, 0, template_width, template_height);
                        calendar_middle.DrawImage(weatherIcon, -15, 0, template_width, template_height);
                    }
                    else
                    {
                        calendar_middle.DrawImage(monthMiddle, -30, 0, template_width, template_height);
                        calendar_middle.DrawImage(dayMiddle_ones, 0, 0, template_width, template_height);
                        calendar_middle.DrawImage(dayMiddle_tens, 0, 0, template_width, template_height);
                        calendar_middle.DrawImage(dayMiddle_filler, 0, 0, template_width, template_height);
                        calendar_middle.DrawImage(weatherIcon, 0, 0, template_width, template_height);
                    }

                    calendar_middle.DrawImage(weekdayMiddle, 0, 0, template_width, template_height);
                }

                middle_layer = Color_Calendar_Black(middle_layer);

                using (Graphics calendar_top = Graphics.FromImage(top_layer))
                {
                    if (user_time.Day < 10)
                    {
                        calendar_top.DrawImage(monthTop, 0, 0, template_width, template_height);
                        calendar_top.DrawImage(dayTop, 0, 0, template_width, template_height);
                    }
                    else
                    {
                        calendar_top.DrawImage(monthTop, -30, 0, template_width, template_height);
                        calendar_top.DrawImage(dayTop_ones, 0, 0, template_width, template_height);
                        calendar_top.DrawImage(dayTop_tens, 0, 0, template_width, template_height);
                    }
                }

                // Use the merged_layer to merge all bottom, middle, and top layers together.
                using (Graphics merged_calendar = Graphics.FromImage(merged_layer))
                {
                    merged_calendar.DrawImage(bottom_layer, 0, 0, template_width, template_height);
                    merged_calendar.DrawImage(middle_layer, 0, 0, template_width, template_height);
                    merged_calendar.DrawImage(top_layer, 0, 0, template_width, template_height);

                    // Turn the transparent pixels on the top weekday and time of day layers to solid.
                    weekdayTop = Weekday_To_No_Alpha((Bitmap)weekdayTop);
                    timeOfDay = Weekday_To_No_Alpha((Bitmap)timeOfDay);

                    // If the day is a Saturday, color the white pixels on the weekday to blue.
                    if (user_time.DayOfWeek.ToString().ToLower() == "saturday")
                    {
                        weekdayTop = White_To_Blue((Bitmap)weekdayTop);
                    }
                    // If the day is a Sunday, color the white pixels on the weekday to red.
                    else if (user_time.DayOfWeek.ToString().ToLower() == "sunday")
                    {
                        weekdayTop = White_To_Red((Bitmap)weekdayTop);
                    }

                    // Draw them to the merged layer.
                    merged_calendar.DrawImage(weekdayTop, 0, 0, template_width, template_height);
                    merged_calendar.DrawImage(timeOfDay, 0, 0, template_width, template_height);
                }

                // Now that the layers are all merged, turn the black pixels transparent.
                merged_layer = Black_To_Opaque(merged_layer);

                // Draw the merged layer to the final bitmap.
                graphics.DrawImage(merged_layer, 0, 0, template_width, template_height);
            }

            return bitmap;
        }

        public static Bitmap Keep_Pixel_Overlap_Stars(Bitmap bitmap_to_keep, Bitmap bitmap_to_compare, Rectangle affected_area)
        {
            System.Drawing.Color pixel_to_compare;
            System.Drawing.Color pixel_to_keep;

            Bitmap base_template = new Bitmap(bitmap_to_keep.Width, bitmap_to_keep.Height);

            for (int i = affected_area.X; i < (affected_area.X + affected_area.Width); i++)
            {
                for (int j = affected_area.Y; j < (affected_area.Y + affected_area.Height); j++)
                {
                    pixel_to_compare = bitmap_to_keep.GetPixel(i, j);
                    pixel_to_keep = bitmap_to_compare.GetPixel(i, j);

                    if (pixel_to_compare.A > 0 && pixel_to_keep.A > 0)
                    {
                        base_template.SetPixel(i, j, System.Drawing.Color.FromArgb(pixel_to_compare.A, pixel_to_keep.R, pixel_to_keep.G, pixel_to_keep.B));
                    }
                }
            }

            return base_template;
        }

        public static Bitmap Delete_Pixel_Overlap(Bitmap bitmap_to_keep, Bitmap bitmap_to_compare, Rectangle cropped_area)
        {
            System.Drawing.Color pixel_to_compare;
            System.Drawing.Color pixel_to_keep;
            //Make an empty bitmap the same size as scrBitmap
            Bitmap newBitmap = new Bitmap(bitmap_to_keep.Width, bitmap_to_keep.Height);
            for (int i = cropped_area.X; i < (cropped_area.X + cropped_area.Width); i++)
            {
                for (int j = cropped_area.Y; j < (cropped_area.Y + cropped_area.Height); j++)
                {
                    //Get the pixel from the scrBitmap image
                    pixel_to_compare = bitmap_to_compare.GetPixel(i, j);
                    pixel_to_keep = bitmap_to_keep.GetPixel(i, j);

                    if (pixel_to_compare.A > 20 && pixel_to_keep.A > 20)
                    {
                        //Don't draw the pixel; it needs to be transparent
                    }
                    else
                    {
                        //Draw the top layer's pixel if both layers don't overlap
                        newBitmap.SetPixel(i, j, pixel_to_keep);
                    }
                }
            }

            return newBitmap;
        }

        // Getter Methods
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
                    return "Sun";
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
                    return "Cloud";
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
                    return "Rain";
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
                    return "Snow";
                }
                else
                {
                    return "Cloud";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                // Return a default condition.
                return "Cloud";
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

        // Coloring Bitmaps
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

        public static Bitmap Invert_Calendar(Bitmap input_bitmap)
        {
            System.Drawing.Color actualColor;

            // Make an empty bitmap the same size as the input bitmap.
            Bitmap newBitmap = new Bitmap(input_bitmap.Width, input_bitmap.Height);
            for (int i = 0; i < 435; i++)
            {
                for (int j = 0; j < 330; j++)
                {
                    // Get the pixel from the input image.
                    actualColor = input_bitmap.GetPixel(i, j);

                    if (actualColor.A <= 5)
                    {
                        // Don't draw the pixel; it needs to be transparent.
                    }
                    else
                    {
                        System.Drawing.Color newColor = System.Drawing.Color.FromArgb(actualColor.ToArgb() ^ 0xffffff);
                        newBitmap.SetPixel(i, j, newColor);
                    }
                }
            }

            //newBitmap = (Bitmap)SetImageOpacity(newBitmap, (float)0.80);
            return newBitmap;
        }

        public static Bitmap Color_Calendar_Black(Bitmap scrBitmap)
        {
            System.Drawing.Color actualColor;
            //Make an empty bitmap the same size as scrBitmap
            Bitmap newBitmap = new Bitmap(scrBitmap.Width, scrBitmap.Height);
            for (int i = 0; i < 435; i++)
            {
                for (int j = 0; j < 330; j++)
                {
                    //Get the pixel from the scrBitmap image
                    actualColor = scrBitmap.GetPixel(i, j);

                    if (actualColor.A <= 5)
                    {
                        //Don't draw the pixel; it needs to be transparent
                    }
                    else
                    {
                        System.Drawing.Color newColor = System.Drawing.Color.FromArgb(actualColor.A, 0, 0, 0);
                        newBitmap.SetPixel(i, j, newColor);
                    }
                }
            }

            //newBitmap = (Bitmap)SetImageOpacity(newBitmap, (float)0.80);
            return newBitmap;
        }

        public static Bitmap Weekday_To_No_Alpha(Bitmap scrBitmap)
        {
            System.Drawing.Color actualColor;
            System.Drawing.Color blackPixel = System.Drawing.Color.FromArgb(0, 0, 0);
            //Make an empty bitmap the same size as scrBitmap
            Bitmap newBitmap = new Bitmap(scrBitmap.Width, scrBitmap.Height);
            for (int i = 0; i < 435; i++)
            {
                for (int j = 0; j < 330; j++)
                {
                    //Get the pixel from the scrBitmap image
                    actualColor = scrBitmap.GetPixel(i, j);

                    if ((actualColor.R <= 100 && actualColor.G <= 100 && actualColor.B <= 100) && (actualColor.A > 5))
                    {
                        System.Drawing.Color newColor = System.Drawing.Color.FromArgb(255, actualColor.R, actualColor.G, actualColor.B);
                        newBitmap.SetPixel(i, j, newColor);
                    }
                    else
                    {
                        newBitmap.SetPixel(i, j, actualColor);
                    }
                }
            }

            //newBitmap = (Bitmap)SetImageOpacity(newBitmap, (float)0.80);
            return newBitmap;
        }

        public static Bitmap Black_To_Opaque(Bitmap scrBitmap)
        {
            System.Drawing.Color actualColor;
            System.Drawing.Color blackPixel = System.Drawing.Color.FromArgb(0, 0, 0);
            //Make an empty bitmap the same size as scrBitmap
            Bitmap newBitmap = new Bitmap(scrBitmap.Width, scrBitmap.Height);
            for (int i = 0; i < 435; i++)
            {
                for (int j = 0; j < 330; j++)
                {
                    //Get the pixel from the scrBitmap image
                    actualColor = scrBitmap.GetPixel(i, j);

                    if ((actualColor.R <= 150 && actualColor.G <= 150 && actualColor.B <= 150) && (actualColor.A > 179))
                    {
                        System.Drawing.Color newColor = System.Drawing.Color.FromArgb(179, actualColor.R, actualColor.G, actualColor.B);
                        newBitmap.SetPixel(i, j, newColor);
                    }
                    else
                    {
                        newBitmap.SetPixel(i, j, actualColor);
                    }
                }
            }

            //newBitmap = (Bitmap)SetImageOpacity(newBitmap, (float)0.80);
            return newBitmap;
        }

        public static Bitmap White_To_Red(Bitmap scrBitmap)
        {
            System.Drawing.Color actualColor;
            //Make an empty bitmap the same size as scrBitmap
            Bitmap newBitmap = new Bitmap(scrBitmap.Width, scrBitmap.Height);
            for (int i = 0; i < 435; i++)
            {
                for (int j = 0; j < 330; j++)
                {
                    //Get the pixel from the scrBitmap image
                    actualColor = scrBitmap.GetPixel(i, j);

                    System.Drawing.Color newColor = System.Drawing.Color.FromArgb(actualColor.ToArgb() ^ 0x00ffff);
                    newColor = System.Drawing.Color.FromArgb(actualColor.A, newColor.R, newColor.G, newColor.B);

                    if (newColor.G > 0 || newColor.B > 0)
                    {
                        newColor = System.Drawing.Color.FromArgb(actualColor.A, newColor.R, 0, 0);
                    }

                    newBitmap.SetPixel(i, j, newColor);
                }
            }

            return newBitmap;
        }

        public static Bitmap White_To_Blue(Bitmap scrBitmap)
        {
            System.Drawing.Color actualColor;
            //Make an empty bitmap the same size as scrBitmap
            Bitmap newBitmap = new Bitmap(scrBitmap.Width, scrBitmap.Height);
            for (int i = 0; i < 435; i++)
            {
                for (int j = 0; j < 330; j++)
                {
                    //Get the pixel from the scrBitmap image
                    actualColor = scrBitmap.GetPixel(i, j);

                    System.Drawing.Color newColor = System.Drawing.Color.FromArgb(actualColor.ToArgb() ^ 0xff0000);
                    newColor = System.Drawing.Color.FromArgb(actualColor.A, newColor.R, newColor.G, newColor.B);

                    if (newColor.R > 0 || newColor.B > 0)
                    {
                        newColor = System.Drawing.Color.FromArgb(actualColor.A, 0, newColor.G, newColor.B);
                    }

                    newBitmap.SetPixel(i, j, newColor);
                }
            }

            return newBitmap;
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

        // Calendar Checks
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

        // Method from https://efundies.com/scale-an-image-in-c-sharp-preserving-aspect-ratio/
        public static Bitmap ScaleImage(Bitmap bmp, int maxWidth, int maxHeight)
        {
            var ratioX = (double)maxWidth / bmp.Width;
            var ratioY = (double)maxHeight / bmp.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(bmp.Width * ratio);
            var newHeight = (int)(bmp.Height * ratio);

            var newImage = new Bitmap(newWidth, newHeight);

            using (var graphics = Graphics.FromImage(newImage))
                graphics.DrawImage(bmp, 0, 0, newWidth, newHeight);

            return newImage;
        }

        // Loading message
        public static EmbedBuilder P5_PS4_Loading_Message()
        {
            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Generating Scene...",
                IconUrl = EmbedSettings.Get_Game_Logo("P5-PS4")
            };

            embed.WithAuthor(author);
            embed.WithColor(EmbedSettings.Get_Game_Color("P5-PS4", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Loading_Icon("P5-PS4", null));
            embed.WithDescription("This may take a few seconds!");

            return embed;
        }
    }
}

