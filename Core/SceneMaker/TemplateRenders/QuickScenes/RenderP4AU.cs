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
using System.Collections.Generic;
using SocialLinker.Core.SceneMaker.Data.Bustup;
using System.Drawing.Drawing2D;
using SocialLinker.Core.Menus;

namespace SocialLinker.Core.SceneMaker.TemplateRenders.QuickScenes
{
    internal class RenderP4AU
    {
        int template_width = 1280;
        int template_height = 720;
        Random rnd = new Random();

        public async Task Render_Quick_Scene_P4AU(SocialLinkerCommand sl_command, OfficialSetData set_data, MakerCommandData command_data)
        {
            SocketUser user = sl_command.User;
            SocketTextChannel channel = (SocketTextChannel)sl_command.Channel;

            RestUserMessage loader = await channel.SendMessageAsync("", false, P4AU_Loading_Message().Build());

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
                Bitmap layer_1 = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P4AU//Main//layer_1.png");
                Bitmap layer_2 = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P4AU//Main//layer_2.png");
                
                graphics.DrawImage(colored_background_bitmap, 0, 0, template_width, template_height);
                graphics.DrawImage(background, 0, 0, template_width, template_height);
                
                // Draw the character bust-up to the template if the base sprite number is not '0'.
                if (command_data.Base_Sprite != 0)
                {
                    Bitmap placed_bustup = Set_Bustup_Placement(sl_command, account, bustup, bustup_data, set_data, command_data);
                    graphics.DrawImage(placed_bustup, 0, 0, template_width, template_height);
                }

                Bitmap text_overlay = new Bitmap(2, 2);

                switch (account.P4AU_TS_Scene_Type)
                {
                    case "Dialogue":
                        text_overlay = Render_Dialogue_Overlay(sl_command, account, set_data, command_data, bustup_data);
                        break;

                    case "Narration":
                        text_overlay = Render_Narration_Overlay(sl_command, account, command_data);
                        break;
                }

                graphics.DrawImage(text_overlay, 0, 0, template_width, template_height);

                Bitmap control_guide = Render_Control_Guide(account);

                graphics.DrawImage(control_guide, 0, 0, control_guide.Width, control_guide.Height);
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
                _ = ErrorHandling.Image_Upload_Failed(sl_command);
                memoryStream.Dispose();
                await loader.DeleteAsync();
                return;
            }

            memoryStream.Dispose();
            await loader.DeleteAsync();

            if (account.Auto_Delete_Commands == "On")
            {
                await sl_command.Message.DeleteAsync();
            }
        }

        public async Task Render_System_Message(SocialLinkerCommand sl_command, MakerCommandData command_data)
        {
            SocketUser user = sl_command.User;
            SocketTextChannel channel = (SocketTextChannel)sl_command.Channel;

            RestUserMessage loader = await channel.SendMessageAsync("", false, P4AU_Loading_Message().Build());

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
                Bitmap layer_1 = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P4AU//Main//layer_1.png");
                Bitmap layer_2 = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P4AU//Main//layer_2.png");

                graphics.DrawImage(colored_background_bitmap, 0, 0, template_width, template_height);
                graphics.DrawImage(background, 0, 0, template_width, template_height);

                Bitmap text_overlay = new Bitmap(2, 2);

                text_overlay = Render_Narration_Overlay(sl_command, account, command_data);

                graphics.DrawImage(text_overlay, 0, 0, template_width, template_height);

                Bitmap control_guide = Render_Control_Guide(account);

                graphics.DrawImage(control_guide, 0, 0, control_guide.Width, control_guide.Height);
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
                _ = ErrorHandling.Image_Upload_Failed(sl_command);
                memoryStream.Dispose();
                await loader.DeleteAsync();
                return;
            }

            memoryStream.Dispose();
            await loader.DeleteAsync();

            if (account.Auto_Delete_Commands == "On")
            {
                await sl_command.Message.DeleteAsync();
            }
        }

        public Bitmap Set_Bustup_Placement(SocialLinkerCommand sl_command, UserInfoFields account, Bitmap bustup, BustupData bustup_data, OfficialSetData set_data, MakerCommandData command_data)
        {
            // Create a starting base bitmap to render all graphics on.
            Bitmap base_template = new Bitmap(template_width, template_height);
            Bitmap bustup_yellow = new Bitmap(2, 2);
            Bitmap bustup_white = new Bitmap(2, 2);

            // Switch the rendering position of the bustup depending on the user's settings.
            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                switch (account.P4AU_TS_Position)
                {
                    case "Left":
                        bustup = OfficialSetMethods.Reverse_Bustup_Selection(sl_command, set_data, bustup, bustup_data, command_data);

                        if (bustup_data.P4AU_Dual_Flip == true)
                        {
                            bustup.RotateFlip(RotateFlipType.RotateNoneFlipX);
                        }

                        bustup_yellow = Bitmap_To_Color(bustup, System.Drawing.Color.FromArgb(240, 253, 39), new Rectangle(0, 0, bustup.Width, bustup.Height));
                        bustup_white = Bitmap_To_Color(bustup, System.Drawing.Color.White, new Rectangle(0, 0, bustup.Width, bustup.Height));

                        if (account.P4AU_TS_Scene_Type == "Dialogue" && (account.P4AU_TS_Highlight == "On"))
                        {
                            graphics.DrawImage(bustup_yellow, bustup_data.P4AU_Left_Coord_X - 16, bustup_data.P4AU_Left_Coord_Y, bustup_data.P4AU_Scale_Width, bustup_data.P4AU_Scale_Height);
                            graphics.DrawImage(bustup_white, bustup_data.P4AU_Left_Coord_X - 6, bustup_data.P4AU_Left_Coord_Y, bustup_data.P4AU_Scale_Width, bustup_data.P4AU_Scale_Height);
                        }

                        graphics.DrawImage(bustup, bustup_data.P4AU_Left_Coord_X, bustup_data.P4AU_Left_Coord_Y, bustup_data.P4AU_Scale_Width, bustup_data.P4AU_Scale_Height);
                        break;

                    case "Right":
                        bustup_yellow = Bitmap_To_Color(bustup, System.Drawing.Color.FromArgb(240, 253, 39), new Rectangle(0, 0, bustup.Width, bustup.Height));
                        bustup_white = Bitmap_To_Color(bustup, System.Drawing.Color.White, new Rectangle(0, 0, bustup.Width, bustup.Height));

                        if (account.P4AU_TS_Scene_Type == "Dialogue" && (account.P4AU_TS_Highlight == "On"))
                        {
                            graphics.DrawImage(bustup_yellow, bustup_data.P4AU_Right_Coord_X - 16, bustup_data.P4AU_Right_Coord_Y, bustup_data.P4AU_Scale_Width, bustup_data.P4AU_Scale_Height);
                            graphics.DrawImage(bustup_white, bustup_data.P4AU_Right_Coord_X - 6, bustup_data.P4AU_Right_Coord_Y, bustup_data.P4AU_Scale_Width, bustup_data.P4AU_Scale_Height);
                        }

                        graphics.DrawImage(bustup, bustup_data.P4AU_Right_Coord_X, bustup_data.P4AU_Right_Coord_Y, bustup_data.P4AU_Scale_Width, bustup_data.P4AU_Scale_Height);
                        break;

                    case "Center":
                        bustup_yellow = Bitmap_To_Color(bustup, System.Drawing.Color.FromArgb(240, 253, 39), new Rectangle(0, 0, bustup.Width, bustup.Height));
                        bustup_white = Bitmap_To_Color(bustup, System.Drawing.Color.White, new Rectangle(0, 0, bustup.Width, bustup.Height));

                        if (account.P4AU_TS_Scene_Type == "Dialogue" && (account.P4AU_TS_Highlight == "On"))
                        {
                            graphics.DrawImage(bustup_yellow, bustup_data.P4AU_Center_Coord_X - 16, bustup_data.P4AU_Center_Coord_Y, bustup_data.P4AU_Scale_Width, bustup_data.P4AU_Scale_Height);
                            graphics.DrawImage(bustup_white, bustup_data.P4AU_Center_Coord_X - 6, bustup_data.P4AU_Center_Coord_Y, bustup_data.P4AU_Scale_Width, bustup_data.P4AU_Scale_Height);
                        }

                        graphics.DrawImage(bustup, bustup_data.P4AU_Center_Coord_X, bustup_data.P4AU_Center_Coord_Y, bustup_data.P4AU_Scale_Width, bustup_data.P4AU_Scale_Height);
                        break;
                }
            }

            return base_template;
        }

        public Bitmap Render_Dialogue_Overlay(SocialLinkerCommand sl_command, UserInfoFields account, OfficialSetData set_data, MakerCommandData command_data, BustupData bustup_data)
        {
            Bitmap base_template = new Bitmap(template_width, template_height);

            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                Bitmap layer_1 = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P4AU//Main//layer_1.png");
                Bitmap layer_2 = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P4AU//Main//layer_2.png");

                graphics.DrawImage(layer_1, 0, 0, template_width, template_height);
                graphics.DrawImage(layer_2, 0, 0, template_width, template_height);

                List<string>[] parsed_lines = OfficialSetMethods.Line_Parser(sl_command, "P4AU", command_data.Dialogue, 3, 850);
                graphics.DrawImage(Render_Dialogue(parsed_lines, 149, 529, account), 0, 0, template_width, template_height);

                string display_name = OfficialSetMethods.GetDisplayName(account, command_data, set_data, bustup_data);
                Bitmap rendered_display_name = Bitmap_To_Color(Render_Name(display_name), System.Drawing.Color.Black, new Rectangle(142, 478, 600, 49));
                graphics.DrawImage(rendered_display_name, 0, 0, template_width, template_height);

                graphics.DrawImage(Render_Arena_Lines(), 0, 0, template_width, template_height);
            }

            return base_template;
        }

        public Bitmap Render_Narration_Overlay(SocialLinkerCommand sl_command, UserInfoFields account, MakerCommandData command_data)
        {
            Bitmap base_template = new Bitmap(template_width, template_height);

            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                Bitmap layer_1 = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P4AU//Main//narration_bg.png");

                graphics.DrawImage(layer_1, 0, 0, template_width, template_height);

                List<string>[] parsed_lines = OfficialSetMethods.Line_Parser(sl_command, "P4AU", command_data.Dialogue, 6, 1050);
                graphics.DrawImage(Render_Dialogue(parsed_lines, 127, 143, account), 0, 0, template_width, template_height);
            }

            return base_template;
        }

        public Bitmap Render_Name(string display_name)
        {
            // Create a bitmap as large as the template.
            Bitmap base_template = new Bitmap(template_width, template_height);

            // Establish an int for the width and height glyphs should be rendered at.
            int multiplier = 49;

            // Load the bitmap font.
            string font_sheet = $@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P4AU//Font//p4au_font_sheet.png";

            // Create a variable to iterate through sections of the bitmap font.
            Bitmap current_glyph;

            // Specify X and Y coordinates for where the glyphs should start rendering on the template.
            int render_position_x = 142;
            int render_position_y = 478;

            char[] charArr = display_name.ToCharArray();

            for (int i = 0; i < charArr.Length; i++)
            {
                // Retrieve glyph information from the JSON file.
                var glyph = ParsingMethods.Get_P4AU_Glyph(charArr[i]);

                // Check if the character is a line break.
                if (charArr[i] == '\u000a')
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

                            // Draw the glyph to the base bitmap.
                            graphics.DrawImage(current_glyph, (render_position_x - glyph.LeftCut), render_position_y, multiplier, multiplier);
                        }
                    }

                    // Set the next X value at the end of the current glyph's right width.
                    render_position_x += (glyph.RightCut - glyph.LeftCut);
                }
            }

            return base_template;
        }

        public Bitmap Render_Dialogue(List<string>[] dialogue_lines, int start_pos_x, int start_pos_y, UserInfoFields account)
        {
            //Create a bitmap as large as the template
            Bitmap bitmap = new Bitmap(template_width, template_height);

            //Establish an int for the width and height glyphs should be rendered at
            int multiplier = 49;

            string font_sheet = $@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P4AU//Font//p4au_font_sheet.png";
            Bitmap current_glyph;

            int cursor_position_x = 0;
            int cursor_position_y = 0;

            for (int i = 0; i < dialogue_lines.Length; i++)
            {
                //Establish variables for where the glyphs should be rendered on the template
                int render_position_x = start_pos_x;
                int render_position_y = start_pos_y + (42 * i);

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
                                graphics.DrawImage(current_glyph, (render_position_x - glyph.LeftCut), render_position_y, multiplier, multiplier);
                            }
                        }

                        //Set the next X value at the end of the current glyph's right width
                        render_position_x += (glyph.RightCut - glyph.LeftCut);

                        cursor_position_x = render_position_x;
                        cursor_position_y = render_position_y;
                    }
                }
            }

            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                Bitmap cursor = Render_Cursor(account);
                graphics.DrawImage(cursor, cursor_position_x + 8, cursor_position_y, multiplier, multiplier);
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
                var glyph = ParsingMethods.Get_P4AU_Glyph(char_array[i]);

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

        public Bitmap Render_Cursor(UserInfoFields account)
        {
            Bitmap cursor_outer = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P4AU//Main//maru_2.png");
            Bitmap cursor_inner = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P4AU//Main//maru_1.png");
            Bitmap base_template = new Bitmap(49, 49);
            
            int weighted_random = rnd.Next(1, 11);

            int rotation_angle = 0;

            if (weighted_random > 3)
            {
                rotation_angle = rnd.Next(340, 360);
            }
            else
            {
                rotation_angle = rnd.Next(1, 340);
            }

            int center_point_x = 0;
            int center_point_y = 0;

            // Changes point of rotation based on chosen degree. Can remove once a more consistent rotation method is found.
            if (rotation_angle >= 315)
            {
                center_point_x = 16;
                center_point_y = 16;
            }
            else if (rotation_angle >= 270)
            {
                center_point_x = 15;
                center_point_y = 16;
            }
            else if (rotation_angle >= 180)
            {
                center_point_x = 15;
                center_point_y = 15;
            }
            else if (rotation_angle >= 90)
            {
                center_point_x = 16;
                center_point_y = 15;
            }
            else if (rotation_angle >= 0)
            {
                center_point_x = 16;
                center_point_y = 16;
            }

            Console.WriteLine("Rotation: " + rotation_angle);

            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                graphics.DrawImage(cursor_inner, 8, 12, cursor_inner.Width, cursor_inner.Height);
                graphics.DrawImage(Rotate_Image(cursor_outer, rotation_angle, center_point_x, center_point_y), 8, 12, cursor_inner.Width, cursor_inner.Height);
            }

            switch (account.P4AU_TS_Auto_Advance)
            {
                case "On":
                    base_template = Bitmap_To_Color(base_template, System.Drawing.Color.FromArgb(255, 119, 0), new Rectangle(8, 13, 32, 32));
                    break;

                case "Off":
                    // Do nothing
                    break;
            }

            return base_template;
        }

        public Bitmap Render_Control_Guide(UserInfoFields account)
        {
            switch (account.P4AU_TS_Panel)
            {
                case "PlayStation®️ 3":
                    return (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P4AU//Main//Control_Panel//ps3.png");

                case "PlayStation®️ 4":
                    return (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P4AU//Main//Control_Panel//ps4.png");

                case "PlayStation®️ 4 (PC Layout)":
                    return (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P4AU//Main//Control_Panel//ps4_pc.png");

                case "Xbox 360":
                    return (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P4AU//Main//Control_Panel//xbox_360.png");

                case "Xbox One (PC Layout)":
                    return (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P4AU//Main//Control_Panel//xbox_one_pc.png");

                case "Nintendo Switch":
                    return (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P4AU//Main//Control_Panel//switch.png");

                case "Nintendo Switch (PC Layout)":
                    return (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P4AU//Main//Control_Panel//switch_pc.png");

                case "Keyboard":
                    return (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P4AU//Main//Control_Panel//keyboard.png");

                case "None":
                    return new Bitmap(2,2);

                default:
                    return new Bitmap(2, 2);
            }
        }

        public Bitmap Render_Arena_Lines()
        {
            Bitmap base_template = new Bitmap(template_width, template_height);

            Point line_1_top_default = new Point(134, 480);
            Point line_1_bottom_default = new Point(134, 720);
            Point line_1_center_point = new Point(134, 596);
            int line_1_angle = rnd.Next(-12, 4);

            Point line_2_top_default = new Point(1182, 480);
            Point line_2_bottom_default = new Point(1182, 720);
            Point line_2_center_point = new Point(1182, 557);
            int line_2_angle = rnd.Next(-26, -5);

            Point line_3_top_default = new Point(1182, 480);
            Point line_3_bottom_default = new Point(1182, 720);
            Point line_3_center_point = new Point(1182, 557);
            int line_3_angle = rnd.Next(-9, 17);

            Point line_1_top = RotatePoint(line_1_top_default, line_1_center_point, line_1_angle);
            Point line_1_bottom = RotatePoint(line_1_bottom_default, line_1_center_point, line_1_angle);

            Point line_2_top = RotatePoint(line_2_top_default, line_2_center_point, line_2_angle);
            Point line_2_bottom = RotatePoint(line_2_bottom_default, line_2_center_point, line_2_angle);

            Point line_3_top = RotatePoint(line_3_top_default, line_3_center_point, line_3_angle);
            Point line_3_bottom = RotatePoint(line_3_bottom_default, line_3_center_point, line_3_angle);

            System.Drawing.Color arena_yellow = System.Drawing.Color.FromArgb(241, 216, 29);

            Pen line_pen = new Pen(arena_yellow, 2);

            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                graphics.SmoothingMode = SmoothingMode.AntiAlias;

                graphics.DrawLine(line_pen, line_1_top, line_1_bottom);
                graphics.DrawLine(line_pen, line_2_top, line_2_bottom);
                graphics.DrawLine(line_pen, line_3_top, line_3_bottom);
            }

            Bitmap textbox = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P4AU//Main//layer_2.png");

            base_template = Keep_Pixel_Overlap(textbox, base_template);

            return base_template;
        }

        // Method from https://stackoverflow.com/questions/13695317/rotate-a-point-around-another-point
        static Point RotatePoint(Point pointToRotate, Point centerPoint, double angleInDegrees)
        {
            double angleInRadians = angleInDegrees * (Math.PI / 180);
            double cosTheta = Math.Cos(angleInRadians);
            double sinTheta = Math.Sin(angleInRadians);
            return new Point
            {
                X =
                    (int)
                    (cosTheta * (pointToRotate.X - centerPoint.X) -
                    sinTheta * (pointToRotate.Y - centerPoint.Y) + centerPoint.X),
                Y =
                    (int)
                    (sinTheta * (pointToRotate.X - centerPoint.X) +
                    cosTheta * (pointToRotate.Y - centerPoint.Y) + centerPoint.Y)
            };
        }

        // Utility
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

        public static Bitmap Keep_Pixel_Overlap(Bitmap bottom_bitmap, Bitmap top_bitmap)
        {
            System.Drawing.Color bottom_pixel_color;
            System.Drawing.Color top_pixel_color;
            //Make an empty bitmap the same size as scrBitmap
            Bitmap newBitmap = new Bitmap(bottom_bitmap.Width, bottom_bitmap.Height);
            for (int x = 55; x < 1275; x++)
            {
                for (int y = 521; y < 720; y++)
                {
                    //Get the pixel from the scrBitmap image
                    bottom_pixel_color = bottom_bitmap.GetPixel(x, y);
                    top_pixel_color = top_bitmap.GetPixel(x, y);

                    if (bottom_pixel_color.A > 0 && top_pixel_color.A > 0)
                    {
                        //Draw the top layer's pixel if both layers overlap
                        newBitmap.SetPixel(x, y, top_pixel_color);
                    }
                }
            }

            return newBitmap;
        }

        // Method from https://stackoverflow.com/questions/58086523/rotate-bitmap-around-point-and-make-that-point-the-new-center
        Bitmap Rotate_Image(Bitmap img, float angle, int cx, int cy)
        {
            Bitmap result = new Bitmap(img.Width, img.Height);
            int mx = img.Width / 2,
                my = img.Height / 2;
            using (Graphics g = Graphics.FromImage(result))
            {
                g.TranslateTransform(cx, cy);
                g.RotateTransform(angle);
                g.TranslateTransform(-cx, -cy);
                g.TranslateTransform(mx - cx, my - cy, MatrixOrder.Append);
                g.DrawImage(img, new Point(0, 0));
            }
            return result;
        }

        public static EmbedBuilder P4AU_Loading_Message()
        {
            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Generating Scene...",
                IconUrl = EmbedSettings.Get_Game_Logo("P4AU")
            };

            embed.WithAuthor(author);
            embed.WithColor(EmbedSettings.Get_Game_Color("P4AU", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Loading_Icon("P4AU", null));
            embed.WithDescription("This may take a few seconds!");

            return embed;
        }
    }
}
