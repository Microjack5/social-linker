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
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using SocialLinker.Core.Menus;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace SocialLinker.Core.SceneMaker.TemplateRenders.QuickScenes
{
    public class RenderP5R : ModuleBase<SocketCommandContext>
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
        DateTime user_time = default;
        int max_line_length = 810;
        int max_line_length_before_box_stagnates = 700;

        public async Task Render_Quick_Scene_P5R(SocialLinkerCommand sl_command, OfficialSetData set_data, MakerCommandData command_data)
        {
            SocketUser user = sl_command.User;
            SocketTextChannel channel = (SocketTextChannel)sl_command.Channel;

            RestUserMessage loader = await channel.SendMessageAsync("", false, P5R_Loading_Message().Build());

            var account = UserInfoClasses.GetAccount(user);
            user_time = Get_Date(sl_command, account);

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
            display_name = OfficialSetMethods.Validate_Input(sl_command, "P5R", "Name", display_name);

            Bitmap calendar = new Bitmap(2, 2);

            command_data.Dialogue = OfficialSetMethods.Validate_Input(sl_command, "P5R", "Dialogue", command_data.Dialogue);
            List<string>[] parsed_lines = OfficialSetMethods.Line_Parser(sl_command, "P5R", command_data.Dialogue, 3, max_line_length);

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
                    Bitmap bustup_layer = Render_Bustup(account, user_time, set_data, bustup_data, bustup);
                    graphics.DrawImage(bustup_layer, 0, 0, bustup_layer.Width, bustup_layer.Height);
                }

                if (account.P5R_TS_HUD != "None")
                {
                    calendar = Construct_Calendar(sl_command, account, user_time);
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

            RestUserMessage loader = await channel.SendMessageAsync("", false, P5R_Loading_Message().Build());

            var account = UserInfoClasses.GetAccount(user);
            user_time = Get_Date(sl_command, account);

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

            command_data.Dialogue = OfficialSetMethods.Validate_Input(sl_command, "P5R", "Dialogue", command_data.Dialogue);
            List<string>[] parsed_lines = OfficialSetMethods.Line_Parser(sl_command, "P5R", command_data.Dialogue, 3, max_line_length);

            // Textbox layers MUST be rendered here
            Bitmap dialogue_layers = Combine_System_Textbox_Layers(account, parsed_lines);

            // Time to put it all together!
            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                graphics.DrawImage(background, 0, 0, template_width, template_height);
                graphics.DrawImage(scene_border, 0, 0, template_width, template_height);

                if (account.P5R_TS_HUD != "None")
                {
                    calendar = Construct_Calendar(sl_command, account, user_time);
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

        public Bitmap Render_Bustup(UserInfoFields account, DateTime user_time, OfficialSetData set_data, BustupData bustup_data, Bitmap bustup)
        {
            Bitmap base_template = new Bitmap(template_width, template_height);
            Bitmap character_with_background = new Bitmap(template_width, template_height);
            Bitmap drop_shadow = new Bitmap(2, 2);

            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                drop_shadow = Create_Bustup_Drop_Shadow(bustup);
                drop_shadow = (Bitmap)Set_Image_Opacity(drop_shadow, (float)0.8);

                switch (account.P5R_TS_Caller_Toggle)
                {
                    case "On":
                        string phone_time_of_day = Get_Phone_Time_Of_Day(account, set_data, user_time);
                        Bitmap phone_bg = Get_Phone_Background(account, set_data, phone_time_of_day);

                        using (Graphics char_and_bg = Graphics.FromImage(character_with_background))
                        {
                            char_and_bg.DrawImage(phone_bg, -6, 410, 768, 768);
                            char_and_bg.DrawImage(drop_shadow, bustup_data.P5R_Phone_Coord_X - 30, bustup_data.P5R_Phone_Coord_Y + 30, bustup_data.P5R_Phone_Scale_Width, bustup_data.P5R_Phone_Scale_Height);
                            char_and_bg.DrawImage(bustup, bustup_data.P5R_Phone_Coord_X, bustup_data.P5R_Phone_Coord_Y, bustup_data.P5R_Phone_Scale_Width, bustup_data.P5R_Phone_Scale_Height);
                        }

                        Bitmap phone_tint = Get_Phone_Tint(character_with_background, phone_time_of_day);

                        graphics.DrawImage(Render_Phone_Call(phone_tint), 0, 0, template_width, template_height);
                        break;

                    case "Off":
                        graphics.DrawImage(drop_shadow, bustup_data.P5R_Coord_X - 30, bustup_data.P5R_Coord_Y + 30, bustup_data.P5R_Scale_Width, bustup_data.P5R_Scale_Height);
                        graphics.DrawImage(bustup, bustup_data.P5R_Coord_X, bustup_data.P5R_Coord_Y, bustup_data.P5R_Scale_Width, bustup_data.P5R_Scale_Height);
                        break;
                }
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

                if (account.P5R_TS_Border != "None")
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
                if (length_of_longest_line >= max_line_length_before_box_stagnates)
                {
                    //cursor_x_coord -= 43;
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

                if (account.P5R_TS_Border != "None")
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
                if (length_of_longest_line >= max_line_length_before_box_stagnates)
                {
                    //cursor_x_coord -= 43;
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

                if (account.P5R_TS_Border != "None")
                {
                    Rectangle cropped_area = new Rectangle(0, 0, 1920, 1080);
                    scene_border = Render_Screen_Border(account);
                    scene_border = Delete_Pixel_Overlap(scene_border, rotated_void_layer, cropped_area);
                }

                graphics.DrawImage(message_window, 0, 0, template_width, template_height);

                // Cursor
                if (length_of_longest_line >= max_line_length_before_box_stagnates)
                {
                    //cursor_x_coord -= 43;
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
                switch (account.P5R_TS_Panel)
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
            int name_spacer = 4;

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

            string font_sheet = $@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Font//p5r_font_sheet.png";
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
            int start_point_x = 686;
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

            string font_sheet = $@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Font//p5r_font_sheet.png";
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
            int render_position_x = -1;

            if (char_array[current_index] == 'i')
            {
                render_position_x += -2;
            }

            if (char_array[current_index] == 't')
            {
                render_position_x += 1;
            }

            if (char_array[current_index] == '.')
            {
                render_position_x += 1;
            }

            if (char_array[current_index + 1] == '!')
            {
                render_position_x += 2;
            }
            if (char_array[current_index] == '!')
            {
                render_position_x += 3;
            }

            if (char_array[current_index] == 'B')
            {
                render_position_x += 1;
            }

            if (char_array[current_index] == 'H')
            {
                render_position_x += -1;
            }

            if (char_array[current_index + 1] == 'w')
            {
                render_position_x += -1;
            }
            if (char_array[current_index] == 'w')
            {
                render_position_x += -1;
            }

            if (char_array[current_index + 1] == 'g')
            {
                render_position_x += -1;
            }

            if (char_array[current_index + 1] == 'm')
            {
                render_position_x += -1;
            }

            if (char_array[current_index + 1] == 'j')
            {
                render_position_x += 1;
            }
            if (char_array[current_index] == 'j')
            {
                render_position_x += 1;
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
                var glyph = ParsingMethods.Get_P5R_Glyph(char_array[i]);

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

            //pixel_counter = (int)(pixel_counter * 0.875);

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

                if (length_of_longest_line >= max_line_length_before_box_stagnates)
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

                if (length_of_longest_line >= max_line_length_before_box_stagnates)
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

            Point box_point_1 = new Point(glyph_info.LeftCut - 2, 0);
            Point box_point_2 = new Point(glyph_info.RightCut + 2, 0);
            Point box_point_3 = new Point(glyph_info.RightCut + 2, 54);
            Point box_point_4 = new Point(glyph_info.LeftCut - 2, 54);

            if (narrow_chars.Contains(input_char))
            {
                box_point_2 = new Point(glyph_info.RightCut + 1, 0);
                box_point_3 = new Point(glyph_info.RightCut + 1, 54);
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

        // Phone Call Rendering
        public Bitmap Render_Phone_Call(Bitmap char_and_bg) // Working!
        {
            Bitmap base_template = new Bitmap(template_width, template_height);
            Bitmap white_layer = new Bitmap(template_width, template_height);
            Bitmap black_outer_layer = new Bitmap(template_width, template_height);
            Bitmap black_inner_layer = new Bitmap(template_width, template_height);
            Bitmap void_layer = new Bitmap(template_width, template_height);

            SolidBrush blackBrush = new SolidBrush(System.Drawing.Color.Black);
            SolidBrush whiteBrush = new SolidBrush(System.Drawing.Color.White);

            int black_point_1_x_min = -17;
            int black_point_1_x_max = -13;
            int black_point_1_y_min = 640;
            int black_point_1_y_max = 648;

            int black_point_2_x_min = 518;
            int black_point_2_x_max = 524;
            int black_point_2_y_min = 531;
            int black_point_2_y_max = 536;

            int black_point_3_x_min = 676;
            int black_point_3_x_max = 680;
            int black_point_3_y_min = 721;
            int black_point_3_y_max = 732;

            int black_point_4_x_min = 664;
            int black_point_4_x_max = 671;
            int black_point_4_y_min = 872;
            int black_point_4_y_max = 879;

            int black_point_5_x_min = 324;
            int black_point_5_x_max = 331;
            int black_point_5_y_min = 1088;
            int black_point_5_y_max = 1096;

            int black_point_6_x_min = 104;
            int black_point_6_x_max = 111;
            int black_point_6_y_min = 1017;
            int black_point_6_y_max = 1022;

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

            Point black_point_1 = new Point(black_point_1_x, black_point_1_y);
            Point black_point_2 = new Point(black_point_2_x, black_point_2_y);
            Point black_point_3 = new Point(black_point_3_x, black_point_3_y);
            Point black_point_4 = new Point(black_point_4_x, black_point_4_y);
            Point black_point_5 = new Point(black_point_5_x, black_point_5_y);
            Point black_point_6 = new Point(black_point_6_x, black_point_6_y);

            Point white_point_1 = new Point(black_point_1_x + 48, black_point_1_y + 4);
            Point white_point_2 = new Point(black_point_2_x - 3, black_point_2_y + 10);
            Point white_point_3 = new Point(black_point_3_x - 7, black_point_3_y + 9);
            Point white_point_4 = new Point(black_point_4_x - 15, black_point_4_y - 19);
            Point white_point_5 = new Point(black_point_5_x + 5, black_point_5_y - 37);
            Point white_point_6 = new Point(black_point_6_x + 13, black_point_6_y - 15);

            Point black_inner_point_1 = new Point(white_point_1.X + 17, white_point_1.Y + 10);
            Point black_inner_point_2 = new Point(white_point_2.X - 5, white_point_2.Y + 6);
            Point black_inner_point_3 = new Point(white_point_3.X - 14, white_point_3.Y - 1);
            Point black_inner_point_4 = new Point(white_point_4.X - 13, white_point_4.Y - 1);
            Point black_inner_point_5 = new Point(white_point_5.X + 4, white_point_5.Y - 15);
            Point black_inner_point_6 = new Point(white_point_6.X + 26, white_point_6.Y - 20);

            Point void_point_1 = new Point(black_inner_point_1.X + 14, black_inner_point_1.Y + 5);
            Point void_point_2 = new Point(black_inner_point_2.X - 6, black_inner_point_2.Y + 15);
            Point void_point_3 = new Point(black_inner_point_3.X - 11, black_inner_point_3.Y - 1);
            Point void_point_4 = new Point(black_inner_point_4.X - 8, black_inner_point_4.Y - 3);
            Point void_point_5 = new Point(black_inner_point_5.X - 3, black_inner_point_5.Y - 16);
            Point void_point_6 = new Point(black_inner_point_6.X + 6, black_inner_point_6.Y - 5);

            Point[] black_poly_points = {
                    black_point_1,
                    black_point_2,
                    black_point_3,
                    black_point_4,
                    black_point_5,
                    black_point_6 };

            Point[] white_poly_points = {
                    white_point_1,
                    white_point_2,
                    white_point_3,
                    white_point_4,
                    white_point_5,
                    white_point_6 };

            Point[] black_inner_poly_points = {
                    black_inner_point_1,
                    black_inner_point_2,
                    black_inner_point_3,
                    black_inner_point_4,
                    black_inner_point_5,
                    black_inner_point_6 };

            Point[] void_poly_points = {
                    void_point_1,
                    void_point_2,
                    void_point_3,
                    void_point_4,
                    void_point_5,
                    void_point_6 };

            using (Graphics graphics = Graphics.FromImage(black_outer_layer))
            {
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                graphics.FillPolygon(blackBrush, black_poly_points);
            }

            using (Graphics graphics = Graphics.FromImage(white_layer))
            {
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                graphics.FillPolygon(whiteBrush, white_poly_points);
            }

            using (Graphics graphics = Graphics.FromImage(black_inner_layer))
            {
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                graphics.FillPolygon(blackBrush, black_inner_poly_points);
            }

            using (Graphics graphics = Graphics.FromImage(void_layer))
            {
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                graphics.FillPolygon(whiteBrush, void_poly_points);
            }

            char_and_bg = Keep_Pixel_Overlap_General(char_and_bg, void_layer, new Rectangle(0, 0, template_width, template_height));

            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                graphics.DrawImage(black_outer_layer, 0, 0, template_width, template_height);
                graphics.DrawImage(white_layer, 0, 0, template_width, template_height);
                graphics.DrawImage(black_inner_layer, 0, 0, template_width, template_height);
                graphics.DrawImage(char_and_bg, 0, 0, template_width, template_height);
            }

            return base_template;
        }

        public Bitmap Render_Phone_Call_alt(Bitmap char_and_bg) // Alternate method for generating phone call animations
        {
            Bitmap base_template = new Bitmap(template_width, template_height);
            Bitmap white_layer = new Bitmap(template_width, template_height);
            Bitmap black_outer_layer = new Bitmap(template_width, template_height);
            Bitmap black_inner_layer = new Bitmap(template_width, template_height);
            Bitmap void_layer = new Bitmap(template_width, template_height);

            SolidBrush blackBrush = new SolidBrush(System.Drawing.Color.Black);
            SolidBrush whiteBrush = new SolidBrush(System.Drawing.Color.White);

            int black_point_1_x_min = -17;
            int black_point_1_x_max = -13;
            int black_point_1_y_min = 640;
            int black_point_1_y_max = 648;

            int black_point_2_x_min = 518;
            int black_point_2_x_max = 524;
            int black_point_2_y_min = 531;
            int black_point_2_y_max = 536;

            int black_point_3_x_min = 676;
            int black_point_3_x_max = 680;
            int black_point_3_y_min = 721;
            int black_point_3_y_max = 732;

            int black_point_4_x_min = 664;
            int black_point_4_x_max = 671;
            int black_point_4_y_min = 872;
            int black_point_4_y_max = 879;

            int black_point_5_x_min = 324;
            int black_point_5_x_max = 331;
            int black_point_5_y_min = 1088;
            int black_point_5_y_max = 1096;

            int black_point_6_x_min = 104;
            int black_point_6_x_max = 111;
            int black_point_6_y_min = 1017;
            int black_point_6_y_max = 1022;

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

            Point black_point_1 = new Point(black_point_1_x, black_point_1_y);
            Point black_point_2 = new Point(black_point_2_x, black_point_2_y);
            Point black_point_3 = new Point(black_point_3_x, black_point_3_y);
            Point black_point_4 = new Point(black_point_4_x, black_point_4_y);
            Point black_point_5 = new Point(black_point_5_x, black_point_5_y);
            Point black_point_6 = new Point(black_point_6_x, black_point_6_y);

            Point white_point_1 = new Point(black_point_1_x + 48, black_point_1_y + 4);
            Point white_point_2 = new Point(black_point_2_x - 3, black_point_2_y + 10);
            Point white_point_3 = new Point(black_point_3_x - 7, black_point_3_y + 9);
            Point white_point_4 = new Point(black_point_4_x - 15, black_point_4_y - 19);
            Point white_point_5 = new Point(black_point_5_x + 5, black_point_5_y - 37);
            Point white_point_6 = new Point(black_point_6_x + 13, black_point_6_y - 15);

            Point black_inner_point_1 = new Point(white_point_1.X + 17, white_point_1.Y + 10);
            Point black_inner_point_2 = new Point(white_point_2.X - 5, white_point_2.Y + 6);
            Point black_inner_point_3 = new Point(white_point_3.X - 14, white_point_3.Y - 1);
            Point black_inner_point_4 = new Point(white_point_4.X - 13, white_point_4.Y - 1);
            Point black_inner_point_5 = new Point(white_point_5.X + 4, white_point_5.Y - 15);
            Point black_inner_point_6 = new Point(white_point_6.X + 26, white_point_6.Y - 20);

            Point void_point_1 = new Point(black_inner_point_1.X + 14, black_inner_point_1.Y + 5);
            Point void_point_2 = new Point(black_inner_point_2.X - 6, black_inner_point_2.Y + 15);
            Point void_point_3 = new Point(black_inner_point_3.X - 11, black_inner_point_3.Y - 1);
            Point void_point_4 = new Point(black_inner_point_4.X - 8, black_inner_point_4.Y - 3);
            Point void_point_5 = new Point(black_inner_point_5.X - 3, black_inner_point_5.Y - 16);
            Point void_point_6 = new Point(black_inner_point_6.X + 6, black_inner_point_6.Y - 5);

            Point[] black_poly_points = {
                    black_point_1,
                    black_point_2,
                    black_point_3,
                    black_point_4,
                    black_point_5,
                    black_point_6 };

            Point[] white_poly_points = {
                    white_point_1,
                    white_point_2,
                    white_point_3,
                    white_point_4,
                    white_point_5,
                    white_point_6 };

            Point[] black_inner_poly_points = {
                    black_inner_point_1,
                    black_inner_point_2,
                    black_inner_point_3,
                    black_inner_point_4,
                    black_inner_point_5,
                    black_inner_point_6 };

            Point[] void_poly_points = {
                    void_point_1,
                    void_point_2,
                    void_point_3,
                    void_point_4,
                    void_point_5,
                    void_point_6 };

            // Lines
            var black_inner_poly_1_line_1 = new Line(black_point_1, black_point_2);
            var black_inner_poly_1_line_2 = new Line(black_point_2, black_point_3);
            var black_inner_poly_1_line_3 = new Line(black_point_3, black_point_4);
            var black_inner_poly_1_line_4 = new Line(black_point_4, black_point_5);
            var black_inner_poly_1_line_5 = new Line(black_point_5, black_point_6);
            var black_inner_poly_1_line_6 = new Line(black_point_6, black_point_1);

            var white_inner_poly_1_line_1 = new Line(white_point_1, white_point_2);
            var white_inner_poly_1_line_2 = new Line(white_point_2, white_point_3);
            var white_inner_poly_1_line_3 = new Line(white_point_3, white_point_4);
            var white_inner_poly_1_line_4 = new Line(white_point_4, white_point_5);
            var white_inner_poly_1_line_5 = new Line(white_point_5, white_point_6);
            var white_inner_poly_1_line_6 = new Line(white_point_6, white_point_1);

            var black_inner_poly_2_line_1 = new Line(black_inner_point_1, black_inner_point_2);
            var black_inner_poly_2_line_2 = new Line(black_inner_point_2, black_inner_point_3);
            var black_inner_poly_2_line_3 = new Line(black_inner_point_3, black_inner_point_4);
            var black_inner_poly_2_line_4 = new Line(black_inner_point_4, black_inner_point_5);
            var black_inner_poly_2_line_5 = new Line(black_inner_point_5, black_inner_point_6);
            var black_inner_poly_2_line_6 = new Line(black_inner_point_6, black_inner_point_1);

            var void_poly_line_1 = new Line(void_point_1, void_point_2);
            var void_poly_line_2 = new Line(void_point_2, void_point_3);
            var void_poly_line_3 = new Line(void_point_3, void_point_4);
            var void_poly_line_4 = new Line(void_point_4, void_point_5);
            var void_poly_line_5 = new Line(void_point_5, void_point_6);
            var void_poly_line_6 = new Line(void_point_6, void_point_1);

            // Points
            int number_of_points = 40;

            var black_inner_poly_1_line_1_points = black_inner_poly_1_line_1.getPoints(number_of_points);
            var black_inner_poly_1_line_2_points = black_inner_poly_1_line_2.getPoints(number_of_points);
            var black_inner_poly_1_line_3_points = black_inner_poly_1_line_3.getPoints(number_of_points);
            var black_inner_poly_1_line_4_points = black_inner_poly_1_line_4.getPoints(number_of_points);
            var black_inner_poly_1_line_5_points = black_inner_poly_1_line_5.getPoints(number_of_points);
            var black_inner_poly_1_line_6_points = black_inner_poly_1_line_6.getPoints(number_of_points);

            black_inner_poly_1_line_1_points[10] = new Point(black_inner_poly_1_line_1_points[10].X, black_inner_poly_1_line_1_points[10].Y - 50);
            black_inner_poly_1_line_1_points[11] = new Point(black_inner_poly_1_line_1_points[11].X, black_inner_poly_1_line_1_points[11].Y + 25);

            var white_inner_poly_1_line_1_points = new Point[number_of_points];
            var white_inner_poly_1_line_2_points = new Point[number_of_points];
            var white_inner_poly_1_line_3_points = new Point[number_of_points];
            var white_inner_poly_1_line_4_points = new Point[number_of_points];
            var white_inner_poly_1_line_5_points = new Point[number_of_points];
            var white_inner_poly_1_line_6_points = new Point[number_of_points];

            for (int i = 0; i < number_of_points; i++)
            {
                white_inner_poly_1_line_1_points[i] = new Point(black_inner_poly_1_line_1_points[i].X + 48, black_inner_poly_1_line_1_points[i].Y + 4);
                white_inner_poly_1_line_2_points[i] = new Point(black_inner_poly_1_line_2_points[i].X - 3, black_inner_poly_1_line_2_points[i].Y + 10);
                white_inner_poly_1_line_3_points[i] = new Point(black_inner_poly_1_line_3_points[i].X - 7, black_inner_poly_1_line_3_points[i].Y + 9);
                white_inner_poly_1_line_4_points[i] = new Point(black_inner_poly_1_line_4_points[i].X - 15, black_inner_poly_1_line_4_points[i].Y - 19);
                white_inner_poly_1_line_5_points[i] = new Point(black_inner_poly_1_line_5_points[i].X + 5, black_inner_poly_1_line_5_points[i].Y - 37);
                white_inner_poly_1_line_6_points[i] = new Point(black_inner_poly_1_line_6_points[i].X + 13, black_inner_poly_1_line_6_points[i].Y - 15);
            }

            var black_inner_poly_2_line_1_points = new Point[number_of_points];
            var black_inner_poly_2_line_2_points = new Point[number_of_points];
            var black_inner_poly_2_line_3_points = new Point[number_of_points];
            var black_inner_poly_2_line_4_points = new Point[number_of_points];
            var black_inner_poly_2_line_5_points = new Point[number_of_points];
            var black_inner_poly_2_line_6_points = new Point[number_of_points];

            for (int i = 0; i < number_of_points; i++)
            {
                black_inner_poly_2_line_1_points[i] = new Point(white_inner_poly_1_line_1_points[i].X + 17, white_inner_poly_1_line_1_points[i].Y + 10);
                black_inner_poly_2_line_2_points[i] = new Point(white_inner_poly_1_line_2_points[i].X - 5, white_inner_poly_1_line_2_points[i].Y + 6);
                black_inner_poly_2_line_3_points[i] = new Point(white_inner_poly_1_line_3_points[i].X - 14, white_inner_poly_1_line_3_points[i].Y - 1);
                black_inner_poly_2_line_4_points[i] = new Point(white_inner_poly_1_line_4_points[i].X - 13, white_inner_poly_1_line_4_points[i].Y - 1);
                black_inner_poly_2_line_5_points[i] = new Point(white_inner_poly_1_line_5_points[i].X + 4, white_inner_poly_1_line_5_points[i].Y - 15);
                black_inner_poly_2_line_6_points[i] = new Point(white_inner_poly_1_line_6_points[i].X + 26, white_inner_poly_1_line_6_points[i].Y - 20);
            }

            //var void_poly_line_1_points = new Point[20];
            //var void_poly_line_2_points = new Point[20];
            //var void_poly_line_3_points = new Point[20];
            //var void_poly_line_4_points = new Point[20];
            //var void_poly_line_5_points = new Point[20];
            //var void_poly_line_6_points = new Point[20];

            //for (int i = 0; i < number_of_points; i++)
            //{
            //    void_poly_line_1_points[i] = new Point(black_inner_poly_2_line_1_points[i].X + 14, black_inner_poly_2_line_1_points[i].Y + 5);
            //    void_poly_line_2_points[i] = new Point(black_inner_poly_2_line_2_points[i].X - 6, black_inner_poly_2_line_2_points[i].Y + 15);
            //    void_poly_line_3_points[i] = new Point(black_inner_poly_2_line_3_points[i].X - 11, black_inner_poly_2_line_3_points[i].Y - 1);
            //    void_poly_line_4_points[i] = new Point(black_inner_poly_2_line_4_points[i].X - 8, black_inner_poly_2_line_4_points[i].Y - 3);
            //    void_poly_line_5_points[i] = new Point(black_inner_poly_2_line_5_points[i].X - 3, black_inner_poly_2_line_5_points[i].Y - 16);
            //    void_poly_line_6_points[i] = new Point(black_inner_poly_2_line_6_points[i].X + 6, black_inner_poly_2_line_6_points[i].Y - 5);
            //}

            Point[] black_inner_poly_1 =
                black_inner_poly_1_line_1_points
                .Concat(black_inner_poly_1_line_2_points)
                .Concat(black_inner_poly_1_line_3_points)
                .Concat(black_inner_poly_1_line_4_points)
                .Concat(black_inner_poly_1_line_5_points)
                .Concat(black_inner_poly_1_line_6_points).ToArray();

            Point[] white_inner_poly_1 =
                white_inner_poly_1_line_1_points
                .Concat(white_inner_poly_1_line_2_points)
                .Concat(white_inner_poly_1_line_3_points)
                .Concat(white_inner_poly_1_line_4_points)
                .Concat(white_inner_poly_1_line_5_points)
                .Concat(white_inner_poly_1_line_6_points).ToArray();

            Point[] black_inner_poly_2 =
                black_inner_poly_2_line_1_points
                .Concat(black_inner_poly_2_line_2_points)
                .Concat(black_inner_poly_2_line_3_points)
                .Concat(black_inner_poly_2_line_4_points)
                .Concat(black_inner_poly_2_line_5_points)
                .Concat(black_inner_poly_2_line_6_points).ToArray();

            //Point[] void_poly_1 =
            //    void_poly_line_1_points
            //    .Concat(void_poly_line_2_points)
            //    .Concat(void_poly_line_3_points)
            //    .Concat(void_poly_line_4_points)
            //    .Concat(void_poly_line_5_points)
            //    .Concat(void_poly_line_6_points).ToArray();

            Bitmap test_bitmap = new Bitmap(template_width, template_height);

            using (Graphics graphics = Graphics.FromImage(test_bitmap))
            {
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                graphics.FillPolygon(blackBrush, black_inner_poly_1);
                graphics.FillPolygon(whiteBrush, white_inner_poly_1);
                graphics.FillPolygon(blackBrush, black_inner_poly_2);
                //graphics.FillPolygon(whiteBrush, void_poly_1);
            }

            // =================================

            //using (Graphics graphics = Graphics.FromImage(black_outer_layer))
            //{
            //    graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            //    graphics.FillPolygon(blackBrush, black_poly_points);
            //}

            //using (Graphics graphics = Graphics.FromImage(white_layer))
            //{
            //    graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            //    graphics.FillPolygon(whiteBrush, white_poly_points);
            //}

            //using (Graphics graphics = Graphics.FromImage(black_inner_layer))
            //{
            //    graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            //    graphics.FillPolygon(blackBrush, black_inner_poly_points);
            //}

            //using (Graphics graphics = Graphics.FromImage(void_layer))
            //{
            //    graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            //    graphics.FillPolygon(whiteBrush, void_poly_points);
            //}

            //char_and_bg = Keep_Pixel_Overlap_General(char_and_bg, void_layer, new Rectangle(0, 0, template_width, template_height));

            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                //graphics.DrawImage(black_outer_layer, 0, 0, template_width, template_height);
                //graphics.DrawImage(white_layer, 0, 0, template_width, template_height);
                //graphics.DrawImage(black_inner_layer, 0, 0, template_width, template_height);
                //graphics.DrawImage(char_and_bg, 0, 0, template_width, template_height);
                graphics.DrawImage(test_bitmap, 0, 0, template_width, template_height);
            }

            return base_template;
        }

        public string Get_Phone_Time_Of_Day(UserInfoFields account, OfficialSetData set_data, DateTime user_time)
        {
            string time_of_day = "";
            string[] p5r_vr_resident_id = new string[] { "B11", "B12", "B13", "B25" };

            if (account.P5R_TS_Caller_Location == "Dynamic" || account.P5R_TS_Caller_Location == "Dynamic (Normals Only)")
            {
                switch (Get_Time_of_Day(user_time))
                {
                    case "early_morning":
                        time_of_day = "day";
                        break;

                    case "morning":
                        time_of_day = "day";
                        break;

                    case "daytime":
                        time_of_day = "after";
                        break;

                    case "lunchtime":
                        time_of_day = "after";
                        break;

                    case "after_school":
                        time_of_day = "after";
                        break;

                    case "evening":
                        time_of_day = "night";
                        break;
                }

                if (account.P5R_TS_Caller_Location == "Dynamic")
                {
                    if (set_data.Origin == "P5R" && p5r_vr_resident_id.Contains(set_data.ID))
                    {
                        time_of_day = "vr";
                    }
                }
            }
            else if (account.P5R_TS_Caller_Location == "Velvet Room")
            {
                time_of_day = "vr";
            }

            return time_of_day;
        }

        public Bitmap Get_Phone_Background(UserInfoFields account, OfficialSetData set_data, string phone_time_of_day)
        {
            int choice = 0;

            if (phone_time_of_day == "vr")
            {
                choice = 1;
            }
            else
            {
                choice = user_time.Day % 5;

                if (choice == 0)
                {
                    choice = 5;
                }
            }

            return (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Phone_BG//mw_{phone_time_of_day}_0{choice}.png");
        }

        public Bitmap Get_Phone_Tint(Bitmap input_bitmap, string phone_time_of_day)
        {
            int added_red_value = 0;
            int added_green_value = 0;
            int added_blue_value = 0;
            Bitmap tint_layer = new Bitmap(template_width, template_height);

            switch (phone_time_of_day)
            {
                case "day":
                    added_red_value = 0;
                    added_green_value = 0;
                    added_blue_value = 0;
                    break;

                case "after":
                    added_red_value = 20;
                    added_green_value = 10;
                    added_blue_value = 5;
                    break;

                case "night":
                    added_red_value = 0;
                    added_green_value = 10;
                    added_blue_value = 20;
                    break;

                case "vr":
                    added_red_value = 0;
                    added_green_value = 0;
                    added_blue_value = 50;
                    break;
            }

            for (int x = 0; x < 764; x++)
            {
                for (int y = 408; y < 1080; y++)
                {
                    System.Drawing.Color original_pixel_color = input_bitmap.GetPixel(x, y);

                    int new_red_value = original_pixel_color.R + added_red_value;
                    int new_green_value = original_pixel_color.G + added_green_value;
                    int new_blue_value = original_pixel_color.B + added_blue_value;

                    if (new_red_value > 255)
                    {
                        new_red_value = 255;
                    }
                    if (new_green_value > 255)
                    {
                        new_green_value = 255;
                    }
                    if (new_blue_value > 255)
                    {
                        new_blue_value = 255;
                    }

                    System.Drawing.Color tint_color = System.Drawing.Color.FromArgb(new_red_value, new_green_value, new_blue_value);

                    tint_layer.SetPixel(x, y, tint_color);
                }
            }

            return tint_layer;
        }

        // Rotate Methods
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
            string[] prerendered_star_layers_path = Directory.GetFiles($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Border//Prerendered//Event", "*.png");

            switch (account.P5R_TS_Border)
            {
                case "Event":
                    prerendered_star_layers_path = Directory.GetFiles($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Border//Prerendered//Event", "*.png");
                    break;

                case "Interaction":
                    prerendered_star_layers_path = Directory.GetFiles($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Border//Prerendered//Interaction", "*.png");
                    break;
            }

            var chosen_star_layer_path = prerendered_star_layers_path[rnd.Next(prerendered_star_layers_path.Length)];

            return (Bitmap)System.Drawing.Image.FromFile(chosen_star_layer_path);
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
            try
            {
                int template_width = 8000;
                int template_height = 8000;

                double start_size = rnd.NextDouble(19.0, 21.0);

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

                Bitmap star_base = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Border//star_base.png");

                new_bitmap = Keep_Pixel_Overlap_General(new_bitmap, star_base, new Rectangle(0, 0, star_base.Width, star_base.Height));

                // Return the new bitmap.
                return new_bitmap;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new Bitmap(2, 2);
            }
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
            // Make an empty bitmap.
            Bitmap base_bitmap = new Bitmap(template_width, template_height);

            // Create needed bitmap variables for needed assets. We'll initialize them to small bitmaps for now.
            Bitmap auto_toggle = new Bitmap(2, 2);
            Bitmap auto_toggle_bg = new Bitmap(2, 2);
            Bitmap auto_wheel = new Bitmap(2, 2);
            Bitmap ffwd_button = new Bitmap(2, 2);
            Bitmap log_button = new Bitmap(2, 2);

            // Start assigning assets to variables that will be constant on either user setting.
            ffwd_button = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Control_Panel//ffwd.png");
            log_button = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Control_Panel//log.png");

            // Here, we'll assign the auto graphics based on the user's settings.
            switch (account.P5R_TS_Panel)
            {
                case "Auto-Advance":
                    // Use a random variable for the auto wheel icon so it can change in each scene.
                    Random w = new Random();
                    int wInt = w.Next(1, 5);

                    auto_toggle = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Control_Panel//auto_toggle_active.png");
                    auto_toggle_bg = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Control_Panel//auto_toggle_active_bg.png");
                    auto_wheel = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Control_Panel//auto_wheel_{wInt}.png");
                    break;

                default:
                    auto_toggle = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Control_Panel//auto_toggle_default.png");
                    break;
            }

            // Draw the assets to the template.
            using (Graphics graphics = Graphics.FromImage(base_bitmap))
            {
                graphics.DrawImage(ffwd_button, 0, 0, template_width, template_height);
                graphics.DrawImage(auto_toggle_bg, 0, 0, template_width, template_height);
                graphics.DrawImage(auto_wheel, 0, 0, template_width, template_height);
                graphics.DrawImage(auto_toggle, 0, 0, template_width, template_height);
                graphics.DrawImage(log_button, 0, 0, template_width, template_height);
            }

            return base_bitmap;
        }

        // Calendar Rendering
        public Bitmap Construct_Calendar(SocialLinkerCommand sl_command, UserInfoFields account, DateTime user_time)
        {
            // Halloween calendar
            if (user_time.Month == 10 && user_time.Day == 31)
            {
                return Construct_Halloween_Calendar(user_time, account);
            }
            // Christmas calendar
            if (user_time.Month == 12 && (user_time.Day == 24 || user_time.Day == 25))
            {
                return Construct_Christmas_Calendar(user_time, account);
            }
            // New Year's calendar
            if (user_time.Month == 1 && (user_time.Day == 1 || user_time.Day == 2))
            {
                return Construct_New_Year_Calendar(user_time, account);
            }
            // Valentine's Day calendar
            if (user_time.Month == 2 && user_time.Day == 14)
            {
                return Construct_Valentine_Calendar(user_time, account);
            }
            // White Day calendar
            if (user_time.Month == 3 && user_time.Day == 14)
            {
                return Construct_White_Calendar(user_time, account);
            }
            // Harvest festival calendar
            if (user_time.Month == 3 && user_time.Day == 15)
            {
                return Construct_Harvest_Calendar(user_time, account);
            }

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
                monthBottom = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Calendar_Bottom//Month//{user_time.Month}.png");
                monthMiddle = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Calendar_Middle//Month//{user_time.Month}.png");
                monthTop = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Calendar_Top//Month//{user_time.Month}.png");

                weekdayBottom = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Calendar_Bottom//Weekday//{user_time.DayOfWeek}.png");
                weekdayMiddle = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Calendar_Middle//Weekday//{user_time.DayOfWeek}.png");
                weekdayTop = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Calendar_Top//Weekday//{user_time.DayOfWeek}.png");

                // Use a random variable for the weather icon so it can change in each scene.
                Random w = new Random();
                int wInt = w.Next(1, 4);

                // Assign paths for the weather and time of day variables.
                weatherBox = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Weather//weather_box.png");
                weatherIcon = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Weather//{Get_Weather(account)}//{wInt}.png");
                timeOfDay = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Time_of_Day//{Get_Time_of_Day(user_time)}.png");

                // If the day is less than 10, only use one digit for the day.
                if (user_time.Day < 10)
                {
                    dayBottom = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Calendar_Bottom//Day//Single_Digit//{user_time.Day}.png");
                    dayMiddle = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Calendar_Middle//Day//Single_Digit//{user_time.Day}.png");
                    dayTop = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Calendar_Top//Day//Single_Digit//{user_time.Day}.png");
                }
                // If the day is ten or more, we need two digits for the day.
                else if (user_time.Day >= 10)
                {
                    char[] day = user_time.Day.ToString().ToCharArray();

                    dayBottom_tens = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Calendar_Bottom//Day//Double_Digit//Tens_Place//{day[0]}.png");
                    dayBottom_ones = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Calendar_Bottom//Day//Double_Digit//Ones_Place//{day[1]}.png");

                    dayMiddle_tens = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Calendar_Middle//Day//Double_Digit//Tens_Place//{day[0]}.png");
                    dayMiddle_ones = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Calendar_Middle//Day//Double_Digit//Ones_Place//{day[1]}.png");
                    dayMiddle_filler = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Calendar_Middle//Day//Double_Digit//middle_filler.png");

                    dayTop_tens = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Calendar_Top//Day//Double_Digit//Tens_Place//{day[0]}.png");
                    dayTop_ones = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Calendar_Top//Day//Double_Digit//Ones_Place//{day[1]}.png");
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

                switch (account.P5R_TS_HUD)
                {
                    case "Normal":
                        // Do nothing here if the calendar is set to normal.
                        break;

                    case "Inverted":
                        // If the calendar is inverted, color the bottom layer black.
                        bottom_layer = Color_Calendar_Black(bottom_layer);
                        break;
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

                switch (account.P5R_TS_HUD)
                {
                    // If the calendar is normal, color the middle layer black.
                    case "Normal":
                        middle_layer = Color_Calendar_Black(middle_layer);
                        break;

                    // Otherwise, do nothing.
                    case "Inverted":
                        break;
                }

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

                switch (account.P5R_TS_HUD)
                {
                    case "Normal":
                        // Do nothing here if the calendar is set to normal.
                        break;

                    case "Inverted":
                        // If the calendar is set to Inverted, color the top layer black.
                        top_layer = Color_Calendar_Black(top_layer);
                        break;
                }

                // Use the merged_layer to merge all bottom, middle, and top layers together.
                using (Graphics merged_calendar = Graphics.FromImage(merged_layer))
                {
                    merged_calendar.DrawImage(bottom_layer, 0, 0, template_width, template_height);
                    merged_calendar.DrawImage(middle_layer, 0, 0, template_width, template_height);
                    merged_calendar.DrawImage(top_layer, 0, 0, template_width, template_height);

                    if (account.P5R_TS_HUD == "Normal")
                    {
                        // Turn the transparent pixels on the top weekday and time of day layers to solid.
                        weekdayTop = Weekday_To_No_Alpha((Bitmap)weekdayTop);
                        timeOfDay = Weekday_To_No_Alpha((Bitmap)timeOfDay);

                        // If the day is a Sunday or holiday, color the white pixels on the weekday to red.
                        if (user_time.DayOfWeek.ToString().ToLower() == "sunday" || OfficialSetMethods.Is_Holiday(user_time))
                        {
                            weekdayTop = White_To_Red((Bitmap)weekdayTop);
                        }
                        // If the day is a plain Saturday, color the white pixels on the weekday to blue.
                        else if (user_time.DayOfWeek.ToString().ToLower() == "saturday")
                        {
                            weekdayTop = White_To_Blue((Bitmap)weekdayTop);
                        }

                        // Draw them to the merged layer.
                        merged_calendar.DrawImage(weekdayTop, 0, 0, template_width, template_height);
                        merged_calendar.DrawImage(timeOfDay, 0, 0, template_width, template_height);
                    }
                    else if (account.P5R_TS_HUD == "Inverted")
                    {
                        // Turn the transparent pixels on the top weekday and time of day layers to solid.
                        weekdayTop = Weekday_To_No_Alpha((Bitmap)weekdayTop);
                        timeOfDay = Weekday_To_No_Alpha((Bitmap)timeOfDay);

                        weekdayTop = Invert_Calendar((Bitmap)weekdayTop);
                        timeOfDay = Invert_Calendar((Bitmap)timeOfDay);

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

                        // Invert their colors, then draw them to the merged layer.
                        merged_calendar.DrawImage(weekdayTop, 0, 0, template_width, template_height);
                        merged_calendar.DrawImage(timeOfDay, 0, 0, template_width, template_height);
                    }
                }

                // Now that the layers are all merged, turn the black pixels transparent.
                merged_layer = Black_To_Opaque(merged_layer);

                // Draw the merged layer to the final bitmap.
                graphics.DrawImage(merged_layer, 0, 0, template_width, template_height);
            }

            return bitmap;
        }

        public static Bitmap Construct_Halloween_Calendar(DateTime user_time, UserInfoFields account)
        {
            // Create an empty bitmap.
            Bitmap bitmap = new Bitmap(template_width, template_height);

            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                // Establish all variables needed and set them to null; they will be assigned to later.
                System.Drawing.Image monthBottom = null;
                System.Drawing.Image monthMiddle = null;
                System.Drawing.Image monthTop = null;

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

                System.Drawing.Image decoration_1 = null;
                System.Drawing.Image decoration_2 = null;
                System.Drawing.Image decoration_3 = null;
                System.Drawing.Image decoration_4 = null;

                // Assign variables that are immediately needed.
                monthBottom = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Calendar_Bottom//Month//10.png");
                monthMiddle = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Calendar_Middle//Month//10.png");
                monthTop = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Holiday//Halloween//Calendar//Month//10.png");

                weekdayBottom = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Calendar_Bottom//Weekday//{user_time.DayOfWeek.ToString().ToLower()}.png");
                weekdayMiddle = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Calendar_Middle//Weekday//{user_time.DayOfWeek.ToString().ToLower()}.png");
                weekdayTop = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Calendar_Top//Weekday//{user_time.DayOfWeek.ToString().ToLower()}.png");

                decoration_1 = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Holiday//Halloween//Decoration//decoration_1.png");
                decoration_2 = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Holiday//Halloween//Decoration//decoration_2.png");
                decoration_3 = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Holiday//Halloween//Decoration//decoration_3.png");

                // Use a random variable for the spider so it can change in each scene.
                Random s = new Random();
                int sInt = s.Next(1, 4);

                decoration_4 = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Holiday//Halloween//Decoration//Spider//spider_{sInt}.png");

                // Use a random variable for the weather icon so it can change in each scene.
                Random w = new Random();
                int wInt = w.Next(1, 4);

                // Assign paths for the weather and time of day variables.
                weatherBox = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Weather//weather_box.png");
                weatherIcon = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Weather//{Get_Weather(account)}//{wInt}.png");
                timeOfDay = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Time_of_Day//{Get_Time_of_Day(user_time)}.png");

                // Since it's Halloween, the day will always be the 31st.
                dayBottom_tens = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Calendar_Bottom//Day//Double_Digit//Tens_Place//3.png");
                dayBottom_ones = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Calendar_Bottom//Day//Double_Digit//Ones_Place//1.png");

                dayMiddle_tens = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Calendar_Middle//Day//Double_Digit//Tens_Place//3.png");
                dayMiddle_ones = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Calendar_Middle//Day//Double_Digit//Ones_Place//1.png");
                dayMiddle_filler = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Calendar_Middle//Day//Double_Digit//middle_filler.png");

                dayTop_tens = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Holiday//Halloween//Calendar//Day//3.png");
                dayTop_ones = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Holiday//Halloween//Calendar//Day//1.png");

                // Create multiple bitmap layers for the calendar.
                Bitmap bottom_layer = new Bitmap(template_width, template_height);
                Bitmap middle_layer = new Bitmap(template_width, template_height);
                Bitmap top_layer = new Bitmap(template_width, template_height);
                Bitmap merged_layer = new Bitmap(template_width, template_height);
                Bitmap decoration_layer = new Bitmap(template_width, template_height);

                using (Graphics calendar_bottom = Graphics.FromImage(bottom_layer))
                {
                    calendar_bottom.DrawImage(monthBottom, -30, 0, template_width, template_height);
                    calendar_bottom.DrawImage(dayBottom_ones, 0, 0, template_width, template_height);
                    calendar_bottom.DrawImage(dayBottom_tens, 0, 0, template_width, template_height);
                    calendar_bottom.DrawImage(weatherBox, 0, 0, template_width, template_height);

                    calendar_bottom.DrawImage(weekdayBottom, 0, 0, template_width, template_height);

                    // Draw the spider decorations here so it can change colors if inverted.
                    calendar_bottom.DrawImage(decoration_3, 0, 0, template_width, template_height);
                    calendar_bottom.DrawImage(decoration_4, 0, 0, template_width, template_height);
                }

                switch (account.P5R_TS_HUD)
                {
                    case "Normal":
                        // Do nothing here if the calendar is set to normal.
                        break;

                    case "Inverted":
                        // If the calendar is inverted, color the bottom layer black.
                        bottom_layer = Color_Calendar_Black(bottom_layer);
                        break;
                }

                using (Graphics calendar_middle = Graphics.FromImage(middle_layer))
                {
                    calendar_middle.DrawImage(monthMiddle, -30, 0, template_width, template_height);
                    calendar_middle.DrawImage(dayMiddle_ones, 0, 0, template_width, template_height);
                    calendar_middle.DrawImage(dayMiddle_tens, 0, 0, template_width, template_height);
                    calendar_middle.DrawImage(dayMiddle_filler, 0, 0, template_width, template_height);
                    calendar_middle.DrawImage(weatherIcon, 0, 0, template_width, template_height);

                    calendar_middle.DrawImage(weekdayMiddle, 0, 0, template_width, template_height);
                }

                switch (account.P5R_TS_HUD)
                {
                    // If the calendar is normal, color the middle layer black.
                    case "Normal":
                        middle_layer = Color_Calendar_Black(middle_layer);
                        break;

                    // Otherwise, do nothing.
                    case "Inverted":
                        break;
                }

                using (Graphics calendar_top = Graphics.FromImage(top_layer))
                {
                    calendar_top.DrawImage(monthTop, -30, 0, template_width, template_height);
                    calendar_top.DrawImage(dayTop_ones, 0, 0, template_width, template_height);
                    calendar_top.DrawImage(dayTop_tens, 0, 0, template_width, template_height);
                }

                switch (account.P5R_TS_HUD)
                {
                    case "Normal":
                        // Do nothing here if the calendar is set to normal.
                        break;

                    case "Inverted":
                        // If the calendar is set to Inverted, color the top layer black.
                        top_layer = Color_Calendar_Black(top_layer);
                        break;
                }

                // Use the merged_layer to merge all bottom, middle, and top layers together.
                using (Graphics merged_calendar = Graphics.FromImage(merged_layer))
                {
                    merged_calendar.DrawImage(bottom_layer, 0, 0, template_width, template_height);
                    merged_calendar.DrawImage(middle_layer, 0, 0, template_width, template_height);
                    merged_calendar.DrawImage(top_layer, 0, 0, template_width, template_height);

                    if (account.P5R_TS_HUD == "Normal")
                    {
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
                    else if (account.P5R_TS_HUD == "Inverted")
                    {
                        // Turn the transparent pixels on the top weekday and time of day layers to solid.
                        weekdayTop = Weekday_To_No_Alpha((Bitmap)weekdayTop);
                        timeOfDay = Weekday_To_No_Alpha((Bitmap)timeOfDay);

                        weekdayTop = Invert_Calendar((Bitmap)weekdayTop);
                        timeOfDay = Invert_Calendar((Bitmap)timeOfDay);

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

                        // Invert their colors, then draw them to the merged layer.
                        merged_calendar.DrawImage(weekdayTop, 0, 0, template_width, template_height);
                        merged_calendar.DrawImage(timeOfDay, 0, 0, template_width, template_height);
                    }
                }

                // Now that the layers are all merged, turn the black pixels transparent.
                merged_layer = Black_To_Opaque(merged_layer);

                // Since it's a specialized date, we need a new layer for the decorations.
                using (Graphics calendar_decorations = Graphics.FromImage(decoration_layer))
                {
                    calendar_decorations.DrawImage(decoration_1, -30, 0, template_width, template_height);
                    calendar_decorations.DrawImage(decoration_2, -30, 0, template_width, template_height);
                }

                // Draw the merged layer to the final bitmap.
                graphics.DrawImage(merged_layer, 0, 0, template_width, template_height);
                graphics.DrawImage(decoration_layer, 0, 0, template_width, template_height);
            }

            return bitmap;
        }

        public static Bitmap Construct_Christmas_Calendar(DateTime user_time, UserInfoFields account)
        {
            // Make an empty bitmap.
            Bitmap bitmap = new Bitmap(template_width, template_height);

            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                // Establish all variables needed and set them to null; they will be assigned to later.
                System.Drawing.Image monthBottom = null;
                System.Drawing.Image monthMiddle = null;
                System.Drawing.Image monthTop = null;

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

                System.Drawing.Image decoration_1 = null;
                System.Drawing.Image decoration_2 = null;
                System.Drawing.Image decoration_3 = null;
                System.Drawing.Image decoration_4 = null;
                System.Drawing.Image decoration_5 = null;
                System.Drawing.Image decoration_6 = null;

                // Assign variables that are immediately needed.
                monthBottom = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Calendar_Bottom//Month//12.png");
                monthMiddle = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Calendar_Middle//Month//12.png");
                monthTop = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Holiday//Christmas//Calendar//Month//12.png");

                weekdayBottom = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Calendar_Bottom//Weekday//{user_time.DayOfWeek.ToString().ToLower()}.png");
                weekdayMiddle = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Calendar_Middle//Weekday//{user_time.DayOfWeek.ToString().ToLower()}.png");
                weekdayTop = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Calendar_Top//Weekday//{user_time.DayOfWeek.ToString().ToLower()}.png");

                decoration_1 = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Holiday//Christmas//Decoration//decoration_1.png");
                decoration_2 = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Holiday//Christmas//Decoration//decoration_2.png");
                decoration_3 = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Holiday//Christmas//Decoration//decoration_3.png");
                decoration_4 = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Holiday//Christmas//Decoration//decoration_4.png");
                decoration_5 = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Holiday//Christmas//Decoration//decoration_5.png");
                decoration_6 = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Holiday//Christmas//Decoration//decoration_6.png");

                // Use a random variable for the weather icon so it can change in each scene.
                Random w = new Random();
                int wInt = w.Next(1, 4);

                //Assign paths for the weather and time of day variables
                weatherBox = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Weather//weather_box.png");
                weatherIcon = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Weather//{Get_Weather(account)}//{wInt}.png");
                timeOfDay = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Time_of_Day//{Get_Time_of_Day(user_time)}.png");

                // Since it's a specialized date, the day will always be either the 24th or the 25th.
                char[] day = DateTime.Now.Day.ToString().ToCharArray();

                dayBottom_tens = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Calendar_Bottom//Day//Double_Digit//Tens_Place//{day[0]}.png");
                dayBottom_ones = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Calendar_Bottom//Day//Double_Digit//Ones_Place//{day[1]}.png");

                dayMiddle_tens = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Calendar_Middle//Day//Double_Digit//Tens_Place//{day[0]}.png");
                dayMiddle_ones = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Calendar_Middle//Day//Double_Digit//Ones_Place//{day[1]}.png");
                dayMiddle_filler = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Calendar_Middle//Day//Double_Digit//middle_filler.png");

                dayTop_tens = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Calendar_Top//Day//Double_Digit//Tens_Place//{day[0]}.png");
                dayTop_ones = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Calendar_Top//Day//Double_Digit//Ones_Place//{day[1]}.png");

                // Create multiple bitmap layers for the calendar.
                Bitmap bottom_layer = new Bitmap(template_width, template_height);
                Bitmap middle_layer = new Bitmap(template_width, template_height);
                Bitmap top_layer = new Bitmap(template_width, template_height);
                Bitmap merged_layer = new Bitmap(template_width, template_height);
                Bitmap decoration_layer = new Bitmap(template_width, template_height);

                using (Graphics calendar_bottom = Graphics.FromImage(bottom_layer))
                {
                    calendar_bottom.DrawImage(monthBottom, -30, 0, template_width, template_height);
                    calendar_bottom.DrawImage(dayBottom_ones, 0, 0, template_width, template_height);
                    calendar_bottom.DrawImage(dayBottom_tens, 0, 0, template_width, template_height);
                    calendar_bottom.DrawImage(weatherBox, 0, 0, template_width, template_height);

                    calendar_bottom.DrawImage(weekdayBottom, 0, 0, template_width, template_height);
                }

                switch (account.P5R_TS_HUD)
                {
                    case "Normal":
                        // Do nothing here if the calendar is set to normal.
                        break;

                    case "Inverted":
                        // If the calendar is inverted, color the bottom layer black.
                        bottom_layer = Color_Calendar_Black(bottom_layer);
                        break;
                }

                using (Graphics calendar_middle = Graphics.FromImage(middle_layer))
                {
                    calendar_middle.DrawImage(monthMiddle, -30, 0, template_width, template_height);
                    calendar_middle.DrawImage(dayMiddle_ones, 0, 0, template_width, template_height);
                    calendar_middle.DrawImage(dayMiddle_tens, 0, 0, template_width, template_height);
                    calendar_middle.DrawImage(dayMiddle_filler, 0, 0, template_width, template_height);
                    calendar_middle.DrawImage(weatherIcon, 0, 0, template_width, template_height);

                    calendar_middle.DrawImage(weekdayMiddle, 0, 0, template_width, template_height);
                }

                switch (account.P5R_TS_HUD)
                {
                    // If the calendar is normal, color the middle layer black.
                    case "Normal":
                        middle_layer = Color_Calendar_Black(middle_layer);
                        break;

                    // Otherwise, do nothing.
                    case "Inverted":
                        break;
                }

                using (Graphics calendar_top = Graphics.FromImage(top_layer))
                {
                    calendar_top.DrawImage(monthTop, -30, 0, template_width, template_height);
                    calendar_top.DrawImage(dayTop_ones, 0, 0, template_width, template_height);
                    calendar_top.DrawImage(dayTop_tens, 0, 0, template_width, template_height);
                }

                switch (account.P5R_TS_HUD)
                {
                    case "Normal":
                        // Do nothing here if the calendar is set to normal.
                        break;

                    case "Inverted":
                        // If the calendar is set to Inverted, color the top layer black.
                        top_layer = Color_Calendar_Black(top_layer);
                        break;
                }

                // Use the merged_layer to merge all bottom, middle, and top layers together.
                using (Graphics merged_calendar = Graphics.FromImage(merged_layer))
                {
                    merged_calendar.DrawImage(bottom_layer, 0, 0, template_width, template_height);
                    merged_calendar.DrawImage(middle_layer, 0, 0, template_width, template_height);
                    merged_calendar.DrawImage(top_layer, 0, 0, template_width, template_height);

                    if (account.P5R_TS_HUD == "Normal")
                    {
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
                    else if (account.P5R_TS_HUD == "Inverted")
                    {
                        // Turn the transparent pixels on the top weekday and time of day layers to solid.
                        weekdayTop = Weekday_To_No_Alpha((Bitmap)weekdayTop);
                        timeOfDay = Weekday_To_No_Alpha((Bitmap)timeOfDay);

                        weekdayTop = Invert_Calendar((Bitmap)weekdayTop);
                        timeOfDay = Invert_Calendar((Bitmap)timeOfDay);

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

                        // Invert their colors, then draw them to the merged layer.
                        merged_calendar.DrawImage(weekdayTop, 0, 0, template_width, template_height);
                        merged_calendar.DrawImage(timeOfDay, 0, 0, template_width, template_height);
                    }
                }

                // Now that the layers are all merged, turn the black pixels transparent.
                merged_layer = Black_To_Opaque(merged_layer);

                // Since it's a specialized date, we need a new layer for the decorations.
                using (Graphics calendar_decorations = Graphics.FromImage(decoration_layer))
                {
                    calendar_decorations.DrawImage(decoration_1, -30, 0, template_width, template_height);
                    calendar_decorations.DrawImage(decoration_2, -30, 0, template_width, template_height);
                    calendar_decorations.DrawImage(decoration_3, -30, 0, template_width, template_height);
                    calendar_decorations.DrawImage(decoration_4, -30, 0, template_width, template_height);
                    calendar_decorations.DrawImage(decoration_5, -30, 0, template_width, template_height);
                    calendar_decorations.DrawImage(decoration_6, -30, 0, template_width, template_height);
                }

                // Draw the merged layer to the final bitmap.
                graphics.DrawImage(merged_layer, 0, 0, template_width, template_height);
                graphics.DrawImage(decoration_layer, 0, 0, template_width, template_height);
            }

            return bitmap;
        }

        public static Bitmap Construct_New_Year_Calendar(DateTime user_time, UserInfoFields account)
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

                System.Drawing.Image weekdayBottom = null;
                System.Drawing.Image weekdayMiddle = null;
                System.Drawing.Image weekdayTop = null;

                System.Drawing.Image weatherBox = null;
                System.Drawing.Image weatherIcon = null;

                System.Drawing.Image timeOfDay = null;

                System.Drawing.Image decoration_1 = null;
                System.Drawing.Image decoration_2 = null;
                System.Drawing.Image decoration_3 = null;
                System.Drawing.Image decoration_4 = null;
                System.Drawing.Image decoration_5 = null;

                // Weather
                string user_weather = Get_Weather(account);

                // Assign variables that are immediately needed.
                monthBottom = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Calendar_Bottom//Month//1.png");
                monthMiddle = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Calendar_Middle//Month//1.png");
                monthTop = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Holiday//New_Year//Calendar//Month//1.png");

                weekdayBottom = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Calendar_Bottom//Weekday//{user_time.DayOfWeek.ToString().ToLower()}.png");
                weekdayMiddle = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Calendar_Middle//Weekday//{user_time.DayOfWeek.ToString().ToLower()}.png");
                weekdayTop = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Calendar_Top//Weekday//{user_time.DayOfWeek.ToString().ToLower()}.png");

                decoration_1 = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Holiday//New_Year//Decoration//decoration_1.png");
                decoration_2 = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Holiday//New_Year//Decoration//decoration_2.png");
                decoration_3 = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Holiday//New_Year//Decoration//decoration_3.png");
                decoration_4 = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Holiday//New_Year//Decoration//decoration_4.png");
                decoration_5 = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Holiday//New_Year//Decoration//decoration_5.png");

                // Use a random variable for the weather icon so it can change in each scene.
                Random w = new Random();
                int wInt = w.Next(1, 4);

                // Assign paths for the weather and time of day variables.
                weatherBox = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Weather//weather_box.png");

                if (user_weather == "cloud" || user_weather == "sun")
                {
                    weatherIcon = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Holiday//New_Year//Weather//{user_weather}//{wInt}.png");
                }
                else
                {
                    weatherIcon = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Weather//{user_weather}//{wInt}.png");
                }

                timeOfDay = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Time_of_Day//{Get_Time_of_Day(user_time)}.png");

                // Since the New Year's specialized date only covers a couple of days, we only need single digits.
                dayBottom = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Calendar_Bottom//Day//Single_Digit//{user_time.Day}.png");
                dayMiddle = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Calendar_Middle//Day//Single_Digit//{user_time.Day}.png");
                dayTop = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Holiday//New_Year//Calendar//Day//{user_time.Day}.png");

                // Create multiple bitmap layers for the calendar.
                Bitmap bottom_layer = new Bitmap(template_width, template_height);
                Bitmap middle_layer = new Bitmap(template_width, template_height);
                Bitmap top_layer = new Bitmap(template_width, template_height);
                Bitmap merged_layer = new Bitmap(template_width, template_height);
                Bitmap decoration_layer = new Bitmap(template_width, template_height);

                using (Graphics calendar_bottom = Graphics.FromImage(bottom_layer))
                {
                    calendar_bottom.DrawImage(monthBottom, 0, 0, template_width, template_height);
                    calendar_bottom.DrawImage(dayBottom, 0, 0, template_width, template_height);
                    calendar_bottom.DrawImage(weatherBox, -15, 0, template_width, template_height);

                    calendar_bottom.DrawImage(weekdayBottom, 0, 0, template_width, template_height);
                }

                switch (account.P5R_TS_HUD)
                {
                    case "Normal":
                        // Do nothing here if the calendar is set to normal.
                        break;

                    case "Inverted":
                        // If the calendar is inverted, color the bottom layer black.
                        bottom_layer = Color_Calendar_Black(bottom_layer);
                        break;
                }

                using (Graphics calendar_middle = Graphics.FromImage(middle_layer))
                {
                    calendar_middle.DrawImage(monthMiddle, 0, 0, template_width, template_height);
                    calendar_middle.DrawImage(dayMiddle, 0, 0, template_width, template_height);
                    calendar_middle.DrawImage(weatherIcon, 0, 0, template_width, template_height);

                    calendar_middle.DrawImage(weekdayMiddle, 0, 0, template_width, template_height);
                }

                switch (account.P5R_TS_HUD)
                {
                    // If the calendar is normal, color the middle layer black.
                    case "Normal":
                        middle_layer = Color_Calendar_Black(middle_layer);
                        break;

                    // Otherwise, do nothing.
                    case "Inverted":
                        break;
                }

                using (Graphics calendar_top = Graphics.FromImage(top_layer))
                {
                    calendar_top.DrawImage(monthTop, 0, 0, template_width, template_height);
                    calendar_top.DrawImage(dayTop, 0, 0, template_width, template_height);
                }

                switch (account.P5R_TS_HUD)
                {
                    case "Normal":
                        // Do nothing here if the calendar is set to normal.
                        break;

                    case "Inverted":
                        // If the calendar is set to Inverted, color the top layer black.
                        top_layer = Color_Calendar_Black(top_layer);
                        break;
                }

                // Use the merged_layer to merge all bottom, middle, and top layers together.
                using (Graphics merged_calendar = Graphics.FromImage(merged_layer))
                {
                    merged_calendar.DrawImage(bottom_layer, 0, 0, template_width, template_height);
                    merged_calendar.DrawImage(middle_layer, 0, 0, template_width, template_height);
                    merged_calendar.DrawImage(top_layer, 0, 0, template_width, template_height);

                    if (account.P5R_TS_HUD == "Normal")
                    {
                        // Turn the transparent pixels on the top weekday and time of day layers to solid.
                        weekdayTop = Weekday_To_No_Alpha((Bitmap)weekdayTop);
                        timeOfDay = Weekday_To_No_Alpha((Bitmap)timeOfDay);

                        // If the day is a Saturday, color the white pixels on the weekday to blue.
                        if (user_time.DayOfWeek.ToString().ToLower() == "saturday")
                        {
                            weekdayTop = White_To_Blue((Bitmap)weekdayTop);
                        }
                        // Else, the day should be red since New Year's is a holiday.
                        else
                        {
                            weekdayTop = White_To_Red((Bitmap)weekdayTop);
                        }

                        // Draw them to the merged layer.
                        merged_calendar.DrawImage(weekdayTop, 0, 0, template_width, template_height);
                        merged_calendar.DrawImage(timeOfDay, 0, 0, template_width, template_height);
                    }
                    else if (account.P5R_TS_HUD == "Inverted")
                    {
                        // Turn the transparent pixels on the top weekday and time of day layers to solid.
                        weekdayTop = Weekday_To_No_Alpha((Bitmap)weekdayTop);
                        timeOfDay = Weekday_To_No_Alpha((Bitmap)timeOfDay);

                        weekdayTop = Invert_Calendar((Bitmap)weekdayTop);
                        timeOfDay = Invert_Calendar((Bitmap)timeOfDay);

                        // If the day is a Saturday, color the white pixels on the weekday to blue.
                        if (user_time.DayOfWeek.ToString().ToLower() == "saturday")
                        {
                            weekdayTop = White_To_Blue((Bitmap)weekdayTop);
                        }
                        // Else, the day should be red since New Year's is a holiday.
                        else
                        {
                            weekdayTop = White_To_Red((Bitmap)weekdayTop);
                        }

                        // Invert their colors, then draw them to the merged layer.
                        merged_calendar.DrawImage(weekdayTop, 0, 0, template_width, template_height);
                        merged_calendar.DrawImage(timeOfDay, 0, 0, template_width, template_height);
                    }
                }

                // Now that the layers are all merged, turn the black pixels transparent.
                merged_layer = Black_To_Opaque(merged_layer);

                // Since it's a specialized date, we need a new layer for the decorations.
                using (Graphics calendar_decorations = Graphics.FromImage(decoration_layer))
                {
                    calendar_decorations.DrawImage(decoration_1, 0, 0, template_width, template_height);
                    calendar_decorations.DrawImage(decoration_2, 0, 0, template_width, template_height);
                    calendar_decorations.DrawImage(decoration_3, 0, 0, template_width, template_height);
                    calendar_decorations.DrawImage(decoration_4, 0, 0, template_width, template_height);
                    calendar_decorations.DrawImage(decoration_5, 0, 0, template_width, template_height);
                }

                // Draw the merged layer to the final bitmap.
                graphics.DrawImage(merged_layer, 0, 0, template_width, template_height);
                graphics.DrawImage(decoration_layer, 0, 0, template_width, template_height);
            }

            return bitmap;
        }

        public static Bitmap Construct_Valentine_Calendar(DateTime user_time, UserInfoFields account)
        {
            // Make an empty bitmap.
            Bitmap bitmap = new Bitmap(template_width, template_height);

            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                // Establish all variables needed and set them to null; they will be assigned to later.
                System.Drawing.Image monthBottom = null;
                System.Drawing.Image monthMiddle = null;
                System.Drawing.Image monthTop = null;

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

                System.Drawing.Image decoration_1 = null;
                System.Drawing.Image decoration_2 = null;
                System.Drawing.Image decoration_3 = null;
                System.Drawing.Image decoration_4 = null;
                System.Drawing.Image decoration_5 = null;
                System.Drawing.Image decoration_6 = null;
                System.Drawing.Image decoration_7 = null;
                System.Drawing.Image decoration_8 = null;

                // Assign variables that are immediately needed.
                monthBottom = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Calendar_Bottom//Month//2.png");
                monthMiddle = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Calendar_Middle//Month//2.png");
                monthTop = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Calendar_Top//Month//2.png");

                weekdayBottom = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Calendar_Bottom//Weekday//{user_time.DayOfWeek.ToString().ToLower()}.png");
                weekdayMiddle = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Calendar_Middle//Weekday//{user_time.DayOfWeek.ToString().ToLower()}.png");
                weekdayTop = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Holiday//Valentine//Calendar//Weekday//{user_time.DayOfWeek.ToString().ToLower()}.png");

                decoration_1 = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Holiday//Valentine//Decoration//decoration_1.png");
                decoration_2 = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Holiday//Valentine//Decoration//decoration_2.png");
                decoration_3 = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Holiday//Valentine//Decoration//decoration_3.png");
                decoration_4 = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Holiday//Valentine//Decoration//decoration_4.png");
                decoration_5 = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Holiday//Valentine//Decoration//decoration_5.png");
                decoration_6 = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Holiday//Valentine//Decoration//decoration_6.png");
                decoration_7 = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Holiday//Valentine//Decoration//decoration_7.png");
                decoration_8 = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Holiday//Valentine//Decoration//decoration_8.png");

                // Use a random variable for the weather icon so it can change in each scene.
                Random w = new Random();
                int wInt = w.Next(1, 4);

                // Assign paths for the weather and time of day variables.
                weatherBox = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Weather//weather_box.png");
                weatherIcon = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Weather//{Get_Weather(account)}//{wInt}.png");
                timeOfDay = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Time_of_Day//{Get_Time_of_Day(user_time)}.png");

                // Since it's Valentine's Day, the day will always be the 14th.
                dayBottom_tens = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Calendar_Bottom//Day//Double_Digit//Tens_Place//1.png");
                dayBottom_ones = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Calendar_Bottom//Day//Double_Digit//Ones_Place//4.png");

                dayMiddle_tens = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Calendar_Middle//Day//Double_Digit//Tens_Place//1.png");
                dayMiddle_ones = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Calendar_Middle//Day//Double_Digit//Ones_Place//4.png");
                dayMiddle_filler = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Calendar_Middle//Day//Double_Digit//middle_filler.png");

                dayTop_tens = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Calendar_Top//Day//Double_Digit//Tens_Place//1.png");
                dayTop_ones = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Calendar_Top//Day//Double_Digit//Ones_Place//4.png");

                // Create multiple bitmap layers for the calendar.
                Bitmap bottom_layer = new Bitmap(template_width, template_height);
                Bitmap middle_layer = new Bitmap(template_width, template_height);
                Bitmap top_layer = new Bitmap(template_width, template_height);
                Bitmap merged_layer = new Bitmap(template_width, template_height);
                Bitmap decoration_layer = new Bitmap(template_width, template_height);

                using (Graphics calendar_bottom = Graphics.FromImage(bottom_layer))
                {
                    calendar_bottom.DrawImage(monthBottom, -30, 0, template_width, template_height);
                    calendar_bottom.DrawImage(dayBottom_ones, 0, 0, template_width, template_height);
                    calendar_bottom.DrawImage(dayBottom_tens, 0, 0, template_width, template_height);
                    calendar_bottom.DrawImage(weatherBox, 0, 0, template_width, template_height);

                    calendar_bottom.DrawImage(weekdayBottom, 0, 0, template_width, template_height);
                }

                switch (account.P5R_TS_HUD)
                {
                    case "Normal":
                        // Do nothing here if the calendar is set to normal.
                        break;

                    case "Inverted":
                        // If the calendar is inverted, color the bottom layer black.
                        bottom_layer = Color_Calendar_Black(bottom_layer);
                        break;
                }

                using (Graphics calendar_middle = Graphics.FromImage(middle_layer))
                {
                    calendar_middle.DrawImage(monthMiddle, -30, 0, template_width, template_height);
                    calendar_middle.DrawImage(dayMiddle_ones, 0, 0, template_width, template_height);
                    calendar_middle.DrawImage(dayMiddle_tens, 0, 0, template_width, template_height);
                    calendar_middle.DrawImage(dayMiddle_filler, 0, 0, template_width, template_height);
                    calendar_middle.DrawImage(weatherIcon, 0, 0, template_width, template_height);

                    calendar_middle.DrawImage(weekdayMiddle, 0, 0, template_width, template_height);
                }

                switch (account.P5R_TS_HUD)
                {
                    // If the calendar is normal, color the middle layer black.
                    case "Normal":
                        middle_layer = Color_Calendar_Black(middle_layer);
                        break;

                    // Otherwise, do nothing.
                    case "Inverted":
                        break;
                }

                using (Graphics calendar_top = Graphics.FromImage(top_layer))
                {
                    calendar_top.DrawImage(monthTop, -30, 0, template_width, template_height);
                    calendar_top.DrawImage(dayTop_ones, 0, 0, template_width, template_height);
                    calendar_top.DrawImage(dayTop_tens, 0, 0, template_width, template_height);
                }

                switch (account.P5R_TS_HUD)
                {
                    case "Normal":
                        // Do nothing here if the calendar is set to normal.
                        break;

                    case "Inverted":
                        // If the calendar is set to Inverted, color the top layer black.
                        top_layer = Color_Calendar_Black(top_layer);
                        break;
                }

                // Use the merged_layer to merge all bottom, middle, and top layers together.
                using (Graphics merged_calendar = Graphics.FromImage(merged_layer))
                {
                    merged_calendar.DrawImage(bottom_layer, 0, 0, template_width, template_height);
                    merged_calendar.DrawImage(middle_layer, 0, 0, template_width, template_height);

                    if (account.P5R_TS_HUD == "Inverted")
                    {
                        // Before the decorations are overwritten, draw what's supposed to go over the middle layer filler.
                        merged_calendar.DrawImage(Keep_Pixel_Overlap_Calendar((Bitmap)dayMiddle_filler, (Bitmap)decoration_2), 0, 0, template_width, template_height);

                        // Alter the chocolate layers so that only the pixels where it overlaps with the middle day appears.
                        decoration_1 = Keep_Pixel_Overlap_Calendar((Bitmap)dayMiddle_tens, (Bitmap)decoration_1);
                        decoration_2 = Keep_Pixel_Overlap_Calendar((Bitmap)dayMiddle_ones, (Bitmap)decoration_2);

                        merged_calendar.DrawImage(decoration_1, 0, 0, template_width, template_height);
                        merged_calendar.DrawImage(decoration_2, 0, 0, template_width, template_height);
                    }

                    merged_calendar.DrawImage(top_layer, 0, 0, template_width, template_height);

                    if (account.P5R_TS_HUD == "Normal")
                    {
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
                    else if (account.P5R_TS_HUD == "Inverted")
                    {
                        // Turn the transparent pixels on the top weekday and time of day layers to solid.
                        weekdayTop = Weekday_To_No_Alpha((Bitmap)weekdayTop);
                        timeOfDay = Weekday_To_No_Alpha((Bitmap)timeOfDay);

                        weekdayTop = Invert_Calendar((Bitmap)weekdayTop);
                        timeOfDay = Invert_Calendar((Bitmap)timeOfDay);

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

                        // Invert their colors, then draw them to the merged layer.
                        merged_calendar.DrawImage(weekdayTop, 0, 0, template_width, template_height);
                        merged_calendar.DrawImage(timeOfDay, 0, 0, template_width, template_height);
                    }
                }

                //Now that the layers are all merged, turn the black pixels transparent
                merged_layer = Black_To_Opaque(merged_layer);

                //Since it's a specialized date, we need a new layer for the decorations
                using (Graphics calendar_decorations = Graphics.FromImage(decoration_layer))
                {
                    if (account.P5R_TS_HUD == "Normal")
                    {
                        //Alter the chocolate layers so that only the pixels where it overlaps with the day appears
                        calendar_decorations.DrawImage(Keep_Pixel_Overlap_Calendar((Bitmap)dayTop_tens, (Bitmap)decoration_1), 0, 0, template_width, template_height);
                        calendar_decorations.DrawImage(Keep_Pixel_Overlap_Calendar((Bitmap)dayTop_ones, (Bitmap)decoration_2), 0, 0, template_width, template_height);
                    }

                    calendar_decorations.DrawImage(decoration_3, 0, 0, template_width, template_height);
                    calendar_decorations.DrawImage(decoration_4, 0, 0, template_width, template_height);
                    calendar_decorations.DrawImage(decoration_5, 0, 0, template_width, template_height);
                    calendar_decorations.DrawImage(decoration_6, 0, 0, template_width, template_height);
                    calendar_decorations.DrawImage(decoration_7, 0, 0, template_width, template_height);
                    calendar_decorations.DrawImage(decoration_8, 0, 0, template_width, template_height);
                }

                //Draw the merged layer to the final bitmap
                graphics.DrawImage(merged_layer, 0, 0, template_width, template_height);

                if (account.P5R_TS_HUD == "Inverted")
                {
                    //The dark parts of the chocolate turned transparent if the calendar's inverted, so let's draw another chocolate layer on top of it
                    Bitmap chocolate_invert = new Bitmap(template_width, template_height);

                    //Alter the chocolate layers so that only the pixels where it overlaps with the day appears
                    graphics.DrawImage(Keep_Chocolate_Pixel_Overlap(merged_layer, (Bitmap)decoration_1), 0, 0, template_width, template_height);
                    graphics.DrawImage(Keep_Chocolate_Pixel_Overlap(merged_layer, (Bitmap)decoration_2), 0, 0, template_width, template_height);
                }

                //Draw the decoration layer to the final bitmap
                graphics.DrawImage(decoration_layer, 0, 0, template_width, template_height);
            }

            return bitmap;
        }

        public Bitmap Construct_White_Calendar(DateTime user_time, UserInfoFields account)
        {
            // Make an empty bitmap.
            Bitmap bitmap = new Bitmap(template_width, template_height);

            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                // Establish all variables needed and set them to null; they will be assigned to later.
                System.Drawing.Image monthBottom = null;
                System.Drawing.Image monthMiddle = null;
                System.Drawing.Image monthTop = null;

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

                System.Drawing.Image decoration_1 = null;
                System.Drawing.Image decoration_2 = null;
                System.Drawing.Image decoration_3 = null;
                System.Drawing.Image decoration_4 = null;
                System.Drawing.Image decoration_5 = null;
                System.Drawing.Image decoration_6 = null;

                // Assign variables that are immediately needed.
                monthBottom = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Calendar_Bottom//Month//3.png");
                monthMiddle = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Calendar_Middle//Month//3.png");
                monthTop = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Calendar_Top//Month//3.png");

                weekdayBottom = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Calendar_Bottom//Weekday//{user_time.DayOfWeek}.png");
                weekdayMiddle = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Calendar_Middle//Weekday//{user_time.DayOfWeek}.png");
                weekdayTop = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Calendar_Top//Weekday//{user_time.DayOfWeek}.png");

                decoration_1 = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Holiday//White//Decoration//decoration_1.png");
                decoration_2 = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Holiday//White//Decoration//decoration_2.png");
                decoration_3 = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Holiday//White//Decoration//decoration_3.png");
                decoration_4 = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Holiday//White//Decoration//decoration_4.png");
                decoration_5 = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Holiday//White//Decoration//decoration_5.png");
                decoration_6 = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Holiday//White//Decoration//decoration_shine.png");

                // Assign paths for the weather and time of day variables.
                weatherBox = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Weather//weather_box.png");
                weatherIcon = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Weather//{Get_Weather(account)}//{rnd.Next(1, 4)}.png");
                timeOfDay = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Time_of_Day//{Get_Time_of_Day(user_time)}.png");

                // Since it's a specialized date, the day will always be the 14th.
                char[] day = DateTime.Now.Day.ToString().ToCharArray();

                dayBottom_tens = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Calendar_Bottom//Day//Double_Digit//Tens_Place//1.png");
                dayBottom_ones = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Calendar_Bottom//Day//Double_Digit//Ones_Place//4.png");

                dayMiddle_tens = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Calendar_Middle//Day//Double_Digit//Tens_Place//1.png");
                dayMiddle_ones = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Calendar_Middle//Day//Double_Digit//Ones_Place//4.png");
                dayMiddle_filler = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Calendar_Middle//Day//Double_Digit//middle_filler.png");

                dayTop_tens = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Calendar_Top//Day//Double_Digit//Tens_Place//1.png");
                dayTop_ones = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Calendar_Top//Day//Double_Digit//Ones_Place//4.png");

                // Create multiple bitmap layers for the calendar.
                Bitmap bottom_layer = new Bitmap(template_width, template_height);
                Bitmap middle_layer = new Bitmap(template_width, template_height);
                Bitmap top_layer = new Bitmap(template_width, template_height);
                Bitmap merged_layer = new Bitmap(template_width, template_height);
                Bitmap decoration_layer_1 = new Bitmap(template_width, template_height);
                Bitmap decoration_layer_2 = new Bitmap(template_width, template_height);

                // Since it's a specialized date, we need a new layer for the decorations. White Day needs a layer behind the date.
                using (Graphics calendar_decorations = Graphics.FromImage(decoration_layer_1))
                {
                    calendar_decorations.DrawImage(decoration_1, 0, 0, template_width, template_height);
                }
                graphics.DrawImage(decoration_layer_1, 0, 0, template_width, template_height);

                using (Graphics calendar_bottom = Graphics.FromImage(bottom_layer))
                {
                    calendar_bottom.DrawImage(monthBottom, -30, 0, template_width, template_height);
                    calendar_bottom.DrawImage(dayBottom_ones, 0, 0, template_width, template_height);
                    calendar_bottom.DrawImage(dayBottom_tens, 0, 0, template_width, template_height);
                    calendar_bottom.DrawImage(weatherBox, 0, 0, template_width, template_height);

                    calendar_bottom.DrawImage(weekdayBottom, 0, 0, template_width, template_height);
                }

                switch (account.P5R_TS_HUD)
                {
                    case "Normal":
                        // Do nothing here if the calendar is set to normal.
                        break;

                    case "Inverted":
                        // If the calendar is inverted, color the bottom layer black.
                        bottom_layer = Color_Calendar_Black(bottom_layer);
                        break;
                }

                using (Graphics calendar_middle = Graphics.FromImage(middle_layer))
                {
                    calendar_middle.DrawImage(monthMiddle, -30, 0, template_width, template_height);
                    calendar_middle.DrawImage(dayMiddle_ones, 0, 0, template_width, template_height);
                    calendar_middle.DrawImage(dayMiddle_tens, 0, 0, template_width, template_height);
                    calendar_middle.DrawImage(dayMiddle_filler, 0, 0, template_width, template_height);
                    calendar_middle.DrawImage(weatherIcon, 0, 0, template_width, template_height);

                    calendar_middle.DrawImage(weekdayMiddle, 0, 0, template_width, template_height);
                }

                switch (account.P5R_TS_HUD)
                {
                    // If the calendar is normal, color the middle layer black.
                    case "Normal":
                        middle_layer = Color_Calendar_Black(middle_layer);
                        break;

                    // Otherwise, do nothing.
                    case "Inverted":
                        break;
                }

                using (Graphics calendar_top = Graphics.FromImage(top_layer))
                {
                    calendar_top.DrawImage(monthTop, -30, 0, template_width, template_height);
                    calendar_top.DrawImage(dayTop_ones, 0, 0, template_width, template_height);
                    calendar_top.DrawImage(dayTop_tens, 0, 0, template_width, template_height);
                }

                switch (account.P5R_TS_HUD)
                {
                    case "Normal":
                        // Do nothing here if the calendar is set to normal.
                        break;

                    case "Inverted":
                        // If the calendar is set to Inverted, color the top layer black.
                        top_layer = Color_Calendar_Black(top_layer);
                        break;
                }

                // Use the merged_layer to merge all bottom, middle, and top layers together.
                using (Graphics merged_calendar = Graphics.FromImage(merged_layer))
                {
                    merged_calendar.DrawImage(bottom_layer, 0, 0, template_width, template_height);
                    merged_calendar.DrawImage(middle_layer, 0, 0, template_width, template_height);
                    merged_calendar.DrawImage(top_layer, 0, 0, template_width, template_height);

                    if (account.P5R_TS_HUD == "Normal")
                    {
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
                    else if (account.P5R_TS_HUD == "Inverted")
                    {
                        // Turn the transparent pixels on the top weekday and time of day layers to solid.
                        weekdayTop = Weekday_To_No_Alpha((Bitmap)weekdayTop);
                        timeOfDay = Weekday_To_No_Alpha((Bitmap)timeOfDay);

                        weekdayTop = Invert_Calendar((Bitmap)weekdayTop);
                        timeOfDay = Invert_Calendar((Bitmap)timeOfDay);

                        //If the day is a Saturday, color the white pixels on the weekday to blue
                        if (user_time.DayOfWeek.ToString().ToLower() == "saturday")
                        {
                            weekdayTop = White_To_Blue((Bitmap)weekdayTop);
                        }
                        //If the day is a Sunday, color the white pixels on the weekday to red
                        else if (user_time.DayOfWeek.ToString().ToLower() == "sunday")
                        {
                            weekdayTop = White_To_Red((Bitmap)weekdayTop);
                        }

                        //Invert their colors, then draw them to the merged layer
                        merged_calendar.DrawImage(weekdayTop, 0, 0, template_width, template_height);
                        merged_calendar.DrawImage(timeOfDay, 0, 0, template_width, template_height);
                    }
                }

                // Now that the layers are all merged, turn the black pixels transparent.
                merged_layer = Black_To_Opaque(merged_layer);

                // Since it's a specialized date, we need a new layer for the decorations
                using (Graphics calendar_decorations = Graphics.FromImage(decoration_layer_2))
                {
                    // Call the KeepPixelOverlap function on decoration_2 to make sure it's wrapped around the merged_layer correctly.
                    calendar_decorations.DrawImage(Keep_Pixel_Overlap_Calendar(merged_layer, (Bitmap)decoration_2), 0, 0, template_width, template_height);
                    calendar_decorations.DrawImage(decoration_3, 0, 0, template_width, template_height);
                    calendar_decorations.DrawImage(decoration_4, 0, 0, template_width, template_height);
                    calendar_decorations.DrawImage(decoration_5, 0, 0, template_width, template_height);

                    // There is a 1 in 3 chance of a shine appearing on the decoration.
                    if (rnd.Next(1, 4) == 3)
                    {
                        // Form a bitmap for the shine texture to be drawn onto before cropping.
                        Bitmap shine = new Bitmap(template_width, template_height);

                        // Choose a random number betwwn 1 and 4 for the animation frames.
                        int frame = rnd.Next(1, 5);

                        // If one of the four animation variations is chosen, render the shine texture a different way.
                        switch (frame)
                        {
                            case 1:
                                {
                                    using (Graphics shine_to_fullscreen = Graphics.FromImage(shine))
                                    {
                                        shine_to_fullscreen.DrawImage(decoration_6, 41, 89, 106, 117);
                                    }

                                    calendar_decorations.DrawImage(Keep_Shine_Pixel_Overlap((Bitmap)decoration_5, (Bitmap)shine), 0, 0, template_width, template_height);
                                }
                                break;
                            case 2:
                                {
                                    using (Graphics shine_to_fullscreen = Graphics.FromImage(shine))
                                    {
                                        shine_to_fullscreen.DrawImage(decoration_6, 66, 116, 106, 117);
                                    }

                                    calendar_decorations.DrawImage(Keep_Shine_Pixel_Overlap((Bitmap)decoration_5, (Bitmap)shine), 0, 0, template_width, template_height);
                                }
                                break;
                            case 3:
                                {
                                    using (Graphics shine_to_fullscreen = Graphics.FromImage(shine))
                                    {
                                        shine_to_fullscreen.DrawImage(decoration_6, 90, 132, 106, 117);
                                    }

                                    calendar_decorations.DrawImage(Keep_Shine_Pixel_Overlap((Bitmap)decoration_5, (Bitmap)shine), 0, 0, template_width, template_height);
                                }
                                break;
                            case 4:
                                {
                                    using (Graphics shine_to_fullscreen = Graphics.FromImage(shine))
                                    {
                                        shine_to_fullscreen.DrawImage(decoration_6, 108, 146, 106, 117);
                                    }

                                    calendar_decorations.DrawImage(Keep_Shine_Pixel_Overlap((Bitmap)decoration_5, (Bitmap)shine), 0, 0, template_width, template_height);
                                }
                                break;
                            default:
                                {
                                    //Do nothing
                                }
                                break;
                        }
                    }
                }

                // Draw the merged layer to the final bitmap.
                graphics.DrawImage(merged_layer, 0, 0, template_width, template_height);
                graphics.DrawImage(decoration_layer_2, 0, 0, template_width, template_height);
            }

            return bitmap;
        }

        public Bitmap Construct_Harvest_Calendar(DateTime user_time, UserInfoFields account)
        {
            // Make an empty bitmap.
            Bitmap bitmap = new Bitmap(template_width, template_height);

            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                // Establish all variables needed and set them to null; they will be assigned to later.
                System.Drawing.Image monthBottom = null;
                System.Drawing.Image monthMiddle = null;
                System.Drawing.Image monthTop = null;

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

                System.Drawing.Image decoration_1 = null;
                System.Drawing.Image decoration_2 = null;
                System.Drawing.Image decoration_3 = null;
                System.Drawing.Image decoration_4 = null;
                System.Drawing.Image decoration_5 = null;
                System.Drawing.Image decoration_6 = null;
                System.Drawing.Image decoration_7 = null;
                System.Drawing.Image decoration_8 = null;
                System.Drawing.Image decoration_9 = null;

                // Assign variables that are immediately needed.
                monthBottom = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Calendar_Bottom//Month//3.png");
                monthMiddle = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Calendar_Middle//Month//3.png");
                monthTop = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Calendar_Top//Month//3.png");

                weekdayBottom = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Calendar_Bottom//Weekday//{user_time.DayOfWeek}.png");
                weekdayMiddle = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Calendar_Middle//Weekday//{user_time.DayOfWeek}.png");
                weekdayTop = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Calendar_Top//Weekday//{user_time.DayOfWeek}.png");

                decoration_1 = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Holiday//Harvest//Decoration//decoration_1.png");
                decoration_2 = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Holiday//Harvest//Decoration//decoration_2.png");
                decoration_3 = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Holiday//Harvest//Decoration//decoration_3.png");
                decoration_4 = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Holiday//Harvest//Decoration//decoration_4.png");
                decoration_5 = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Holiday//Harvest//Decoration//decoration_5.png");
                decoration_6 = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Holiday//Harvest//Decoration//decoration_6.png");
                decoration_7 = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Holiday//Harvest//Decoration//decoration_7.png");
                decoration_8 = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Holiday//Harvest//Decoration//decoration_8.png");
                decoration_9 = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Holiday//Harvest//Decoration//decoration_9.png");

                // Use a random variable for the weather icon so it can change in each scene.
                Random w = new Random();
                int wInt = w.Next(1, 4);

                // Assign paths for the weather and time of day variables.
                weatherBox = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Weather//weather_box.png");
                weatherIcon = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Weather//{Get_Weather(account)}//{wInt}.png");
                timeOfDay = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Time_of_Day//{Get_Time_of_Day(user_time)}.png");

                // Since it's a specialized date, the day for the Harvest festival will always be the 15th.
                char[] day = DateTime.Now.Day.ToString().ToCharArray();

                dayBottom_tens = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Calendar_Bottom//Day//Double_Digit//Tens_Place//1.png");
                dayBottom_ones = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Calendar_Bottom//Day//Double_Digit//Ones_Place//5.png");

                dayMiddle_tens = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Calendar_Middle//Day//Double_Digit//Tens_Place//1.png");
                dayMiddle_ones = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Calendar_Middle//Day//Double_Digit//Ones_Place//5.png");
                dayMiddle_filler = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Calendar_Middle//Day//Double_Digit//middle_filler.png");

                dayTop_tens = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Calendar_Top//Day//Double_Digit//Tens_Place//1.png");
                dayTop_ones = System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Calendar_Top//Day//Double_Digit//Ones_Place//5.png");

                // Create multiple bitmap layers for the calendar.
                Bitmap bottom_layer = new Bitmap(template_width, template_height);
                Bitmap middle_layer = new Bitmap(template_width, template_height);
                Bitmap top_layer = new Bitmap(template_width, template_height);
                Bitmap merged_layer = new Bitmap(template_width, template_height);
                Bitmap decoration_layer = new Bitmap(template_width, template_height);

                using (Graphics calendar_bottom = Graphics.FromImage(bottom_layer))
                {
                    calendar_bottom.DrawImage(monthBottom, -30, 0, template_width, template_height);
                    calendar_bottom.DrawImage(dayBottom_ones, 0, 0, template_width, template_height);
                    calendar_bottom.DrawImage(dayBottom_tens, 0, 0, template_width, template_height);
                    calendar_bottom.DrawImage(weatherBox, 0, 0, template_width, template_height);

                    calendar_bottom.DrawImage(weekdayBottom, 0, 0, template_width, template_height);
                }

                switch (account.P5R_TS_HUD)
                {
                    case "Normal":
                        // Do nothing here if the calendar is set to normal.
                        break;

                    case "Inverted":
                        // If the calendar is inverted, color the bottom layer black.
                        bottom_layer = Color_Calendar_Black(bottom_layer);
                        break;
                }

                using (Graphics calendar_middle = Graphics.FromImage(middle_layer))
                {
                    calendar_middle.DrawImage(monthMiddle, -30, 0, template_width, template_height);
                    calendar_middle.DrawImage(dayMiddle_ones, 0, 0, template_width, template_height);
                    calendar_middle.DrawImage(dayMiddle_tens, 0, 0, template_width, template_height);
                    calendar_middle.DrawImage(dayMiddle_filler, 0, 0, template_width, template_height);
                    calendar_middle.DrawImage(weatherIcon, 0, 0, template_width, template_height);

                    calendar_middle.DrawImage(weekdayMiddle, 0, 0, template_width, template_height);
                }

                switch (account.P5R_TS_HUD)
                {
                    // If the calendar is normal, color the middle layer black.
                    case "Normal":
                        middle_layer = Color_Calendar_Black(middle_layer);
                        break;

                    // Otherwise, do nothing.
                    case "Inverted":
                        break;
                }

                using (Graphics calendar_top = Graphics.FromImage(top_layer))
                {
                    calendar_top.DrawImage(monthTop, -30, 0, template_width, template_height);
                    calendar_top.DrawImage(dayTop_ones, 0, 0, template_width, template_height);
                    calendar_top.DrawImage(dayTop_tens, 0, 0, template_width, template_height);
                }

                switch (account.P5R_TS_HUD)
                {
                    case "Normal":
                        // Do nothing here if the calendar is set to normal.
                        break;

                    case "Inverted":
                        // If the calendar is set to Inverted, color the top layer black.
                        top_layer = Color_Calendar_Black(top_layer);
                        break;
                }

                // Use the merged_layer to merge all bottom, middle, and top layers together.
                using (Graphics merged_calendar = Graphics.FromImage(merged_layer))
                {
                    merged_calendar.DrawImage(bottom_layer, 0, 0, template_width, template_height);
                    merged_calendar.DrawImage(middle_layer, 0, 0, template_width, template_height);
                    merged_calendar.DrawImage(top_layer, 0, 0, template_width, template_height);

                    if (account.P5R_TS_HUD == "Normal")
                    {
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
                    else if (account.P5R_TS_HUD == "Inverted")
                    {
                        // Turn the transparent pixels on the top weekday and time of day layers to solid.
                        weekdayTop = Weekday_To_No_Alpha((Bitmap)weekdayTop);
                        timeOfDay = Weekday_To_No_Alpha((Bitmap)timeOfDay);

                        weekdayTop = Invert_Calendar((Bitmap)weekdayTop);
                        timeOfDay = Invert_Calendar((Bitmap)timeOfDay);

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

                        // Invert their colors, then draw them to the merged layer.
                        merged_calendar.DrawImage(weekdayTop, 0, 0, template_width, template_height);
                        merged_calendar.DrawImage(timeOfDay, 0, 0, template_width, template_height);
                    }
                }

                // Now that the layers are all merged, turn the black pixels transparent.
                merged_layer = Black_To_Opaque(merged_layer);

                // Since it's a specialized date, we need a new layer for the decorations.
                using (Graphics calendar_decorations = Graphics.FromImage(decoration_layer))
                {
                    calendar_decorations.DrawImage(decoration_1, 0, 0, template_width, template_height);
                    calendar_decorations.DrawImage(decoration_2, 0, 0, template_width, template_height);
                    calendar_decorations.DrawImage(decoration_3, 0, 0, template_width, template_height);
                    calendar_decorations.DrawImage(decoration_4, 0, 0, template_width, template_height);
                    calendar_decorations.DrawImage(decoration_5, 0, 0, template_width, template_height);
                    calendar_decorations.DrawImage(decoration_6, 0, 0, template_width, template_height);
                    calendar_decorations.DrawImage(decoration_7, 0, 0, template_width, template_height);
                    calendar_decorations.DrawImage(decoration_8, 0, 0, template_width, template_height);
                    calendar_decorations.DrawImage(decoration_9, 0, 0, template_width, template_height);
                }

                // Draw the merged layer to the final bitmap.
                graphics.DrawImage(merged_layer, 0, 0, template_width, template_height);

                // Randomly generate petals and color the returned bitmap from white to pink.
                Bitmap petal_layer = White_To_Pink(Generate_Petals());

                // Draw the petal layer where there are white pixels.
                graphics.DrawImage(Keep_Petal_Pixel_Overlap(merged_layer, petal_layer), 0, 0, template_width, template_height);
                //graphics.DrawImage(petal_layer, 0, 0, 435, 330);

                graphics.DrawImage(decoration_layer, 0, 0, template_width, template_height);

            }

            return bitmap;
        }

        public static Bitmap Rotate_Image(Bitmap rotateMe, float angle)
        {
            //First, re-center the image in a larger image that has a margin/frame
            //to compensate for the rotated image's increased size

            var bmp = new Bitmap(rotateMe.Width + (rotateMe.Width / 2), rotateMe.Height + (rotateMe.Height / 2));

            using (Graphics g = Graphics.FromImage(bmp))
                g.DrawImageUnscaled(rotateMe, (rotateMe.Width / 4), (rotateMe.Height / 4), bmp.Width, bmp.Height);

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

        public Bitmap Generate_Petals()
        {
            int width = 435;
            int height = 330;

            string petal_path = @$"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Calendar//Holiday//Harvest//Decoration//decoration_petal.png";
            Bitmap petal = (Bitmap)System.Drawing.Image.FromFile(petal_path);

            Bitmap bitmap = new Bitmap(width, height);

            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                for (int i = 0; i < rnd.Next(25, 51); i++) //15, 26
                {
                    // Create a range for the petals to be placed in.
                    int placement_x = rnd.Next(width);
                    int placement_y = rnd.Next(height);

                    if (placement_x >= 235 && placement_y <= 65)
                    {
                        // To mimic the game, don't draw anything if the placement coordinantes are within this range/
                    }
                    else
                    {
                        // Rotate the petal between a range of -90 and 90 degrees.
                        Bitmap rotated_petal = Rotate_Image(petal, rnd.Next(-90, 90));

                        // Create a variable to resize the petal between 40% and 100%.
                        int resize_percentage = rnd.Next(4, 11);

                        // When the petal is drawn, it will be randomly placed within the specified range and randomly resized within a specified range.
                        graphics.DrawImage(rotated_petal, placement_x, placement_y, (rotated_petal.Width * resize_percentage / 10), (rotated_petal.Height * resize_percentage / 10));
                    }
                }
            }

            return bitmap;
        }

        public static Bitmap Keep_Pixel_Overlap_Calendar(Bitmap bottom_bitmap, Bitmap top_bitmap)
        {
            System.Drawing.Color bottom_pixel_color;
            System.Drawing.Color top_pixel_color;
            //Make an empty bitmap the same size as scrBitmap
            Bitmap newBitmap = new Bitmap(bottom_bitmap.Width, bottom_bitmap.Height);
            for (int i = 0; i < 435; i++)
            {
                for (int j = 0; j < 330; j++)
                {
                    //Get the pixel from the scrBitmap image
                    bottom_pixel_color = bottom_bitmap.GetPixel(i, j);
                    top_pixel_color = top_bitmap.GetPixel(i, j);

                    if (bottom_pixel_color.A > 100 && top_pixel_color.A > 100)
                    {
                        //Draw the top layer's pixel if both layers overlap
                        newBitmap.SetPixel(i, j, top_pixel_color);
                    }
                }
            }

            return newBitmap;
        }

        public static Bitmap Keep_Pixel_Overlap_General(Bitmap bitmap_to_keep, Bitmap bitmap_to_compare, Rectangle affected_area)
        {
            System.Drawing.Color pixel_to_keep;
            System.Drawing.Color pixel_to_compare;

            Bitmap base_template = new Bitmap(bitmap_to_keep.Width, bitmap_to_keep.Height);

            for (int i = affected_area.X; i < (affected_area.X + affected_area.Width); i++)
            {
                for (int j = affected_area.Y; j < (affected_area.Y + affected_area.Height); j++)
                {
                    pixel_to_keep = bitmap_to_keep.GetPixel(i, j);
                    pixel_to_compare = bitmap_to_compare.GetPixel(i, j);

                    if (pixel_to_keep.A > 0 && pixel_to_compare.A > 0)
                    {
                        base_template.SetPixel(i, j, System.Drawing.Color.FromArgb(pixel_to_compare.A, pixel_to_keep.R, pixel_to_keep.G, pixel_to_keep.B));
                    }
                }
            }

            return base_template;
        }

        public static Bitmap Keep_Chocolate_Pixel_Overlap(Bitmap bottom_bitmap, Bitmap top_bitmap)
        {
            System.Drawing.Color bottom_pixel_color;
            System.Drawing.Color top_pixel_color;
            //Make an empty bitmap the same size as scrBitmap
            Bitmap newBitmap = new Bitmap(bottom_bitmap.Width, bottom_bitmap.Height);
            for (int i = 0; i < 435; i++)
            {
                for (int j = 0; j < 330; j++)
                {
                    //Get the pixel from the scrBitmap image
                    bottom_pixel_color = bottom_bitmap.GetPixel(i, j);
                    top_pixel_color = top_bitmap.GetPixel(i, j);

                    //Draw the top layer's pixel if both layers overlap and are nearly the same pixel colors. Top layer's values are reduced by 50 for effectiveness.
                    if (bottom_pixel_color.R >= (top_pixel_color.R - 50) && bottom_pixel_color.G >= (top_pixel_color.G - 50) && bottom_pixel_color.B >= (top_pixel_color.B - 50))
                    {
                        newBitmap.SetPixel(i, j, top_pixel_color);
                    }
                }
            }

            return newBitmap;
        }

        public static Bitmap Keep_Shine_Pixel_Overlap(Bitmap bottom_bitmap, Bitmap top_bitmap)
        {
            System.Drawing.Color bottom_pixel_color;
            System.Drawing.Color top_pixel_color;
            //Make an empty bitmap the same size as scrBitmap
            Bitmap newBitmap = new Bitmap(bottom_bitmap.Width, bottom_bitmap.Height);
            for (int i = 0; i < 435; i++)
            {
                for (int j = 0; j < 330; j++)
                {
                    //Get the pixel from the scrBitmap image
                    bottom_pixel_color = bottom_bitmap.GetPixel(i, j);
                    top_pixel_color = top_bitmap.GetPixel(i, j);

                    System.Drawing.Color newColor = System.Drawing.Color.FromArgb(179, top_pixel_color.R, top_pixel_color.G, top_pixel_color.B);

                    if (bottom_pixel_color.A > 100 && top_pixel_color.A > 100)
                    {
                        //Draw the top layer's pixel if both layers overlap
                        newBitmap.SetPixel(i, j, newColor);
                    }
                }
            }

            return newBitmap;
        }

        public static Bitmap Keep_Petal_Pixel_Overlap(Bitmap bottom_bitmap, Bitmap top_bitmap)
        {
            System.Drawing.Color bottom_pixel_color;
            System.Drawing.Color top_pixel_color;
            //Make an empty bitmap the same size as scrBitmap
            Bitmap newBitmap = new Bitmap(bottom_bitmap.Width, bottom_bitmap.Height);
            for (int i = 0; i < 435; i++)
            {
                for (int j = 0; j < 330; j++)
                {
                    //Get the pixel from the scrBitmap image
                    bottom_pixel_color = bottom_bitmap.GetPixel(i, j);
                    top_pixel_color = top_bitmap.GetPixel(i, j);

                    //Draw the top layer's pixel if the bottom layer's pixel is white. Top layer's values are reduced by 50 for effectiveness.
                    if (bottom_pixel_color.R >= (255 - 50) && bottom_pixel_color.G >= (255 - 50) && bottom_pixel_color.B >= (255 - 50))
                    {
                        newBitmap.SetPixel(i, j, top_pixel_color);
                    }
                }
            }

            return newBitmap;
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

        public static Bitmap White_To_Pink(Bitmap scrBitmap)
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
                        System.Drawing.Color newColor = System.Drawing.Color.FromArgb(actualColor.A, 254, 130, 167);
                        newBitmap.SetPixel(i, j, newColor);
                    }
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
        public static EmbedBuilder P5R_Loading_Message()
        {
            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Generating Scene...",
                IconUrl = EmbedSettings.Get_Game_Logo("P5R")
            };

            embed.WithAuthor(author);
            embed.WithColor(EmbedSettings.Get_Game_Color("P5R", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Loading_Icon("P5R", null));
            embed.WithDescription("This may take a few seconds!");

            return embed;
        }
    }

    //Class from https://stackoverflow.com/questions/1064901/random-number-between-2-double-numbers
    public static class RandomExtensions
    {
        public static double NextDouble(
            this Random random,
            double minValue,
            double maxValue)
        {
            return random.NextDouble() * (maxValue - minValue) + minValue;
        }
    }

    // Class from https://stackoverflow.com/questions/16028752/how-do-i-get-all-the-points-between-two-point-objects
    public class Line
    {
        public Point p1, p2;

        public Line(Point p1, Point p2)
        {
            this.p1 = p1;
            this.p2 = p2;
        }

        public Point[] getPoints(int quantity)
        {
            var points = new Point[quantity];
            int ydiff = p2.Y - p1.Y, xdiff = p2.X - p1.X;
            double slope = (double)(p2.Y - p1.Y) / (p2.X - p1.X);
            double x, y;

            --quantity;

            for (double i = 0; i < quantity; i++)
            {
                y = slope == 0 ? 0 : ydiff * (i / quantity);
                x = slope == 0 ? xdiff * (i / quantity) : y / slope;
                points[(int)i] = new Point((int)Math.Round(x) + p1.X, (int)Math.Round(y) + p1.Y);
            }

            points[quantity] = p2;
            return points;
        }
    }
}
