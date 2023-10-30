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
using System.Collections.Generic;
using SocialLinker.Core.SceneMaker.Data.Bustup;
using SocialLinker.Core.Menus;
using System.Drawing.Drawing2D;

namespace SocialLinker.Core.SceneMaker.TemplateRenders.QuickScenes
{
    public class RenderP4D : ModuleBase<SocketCommandContext>
    {
        int template_width = 1920;
        int template_height = 1080;
        Random rnd = new Random();

        public async Task Render_Quick_Scene_P4D(SocialLinkerCommand sl_command)
        {
            SocketUser user = sl_command.User;
            SocketTextChannel channel = (SocketTextChannel)sl_command.Channel;

            OfficialSetData set_data = sl_command.MakerCommand.Character_Data.Set_Data;
            MakerCommandData maker_command_data = sl_command.MakerCommand;

            RestUserMessage loader = await channel.SendMessageAsync("", false, P4D_Loading_Message().Build());
            var account = UserInfoClasses.GetAccount(user);
            sl_command.MakerCommand.Character_Data.Bustup_Data = BustupDataMethods.Get_Bustup_Data(account, set_data, maker_command_data);
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

            Bitmap bustup = new Bitmap(2, 2);

            if (maker_command_data.Character_Data.Base_Sprite != 0)
            {
                bustup = OfficialSetMethods.Bustup_Selection(sl_command, account, set_data, bustup_data, maker_command_data);
            }

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

                // Draw the character bust-up to the template if the base sprite number is not '0'.
                if (maker_command_data.Character_Data.Base_Sprite != 0)
                {
                    Bitmap placed_bustup = Set_Bustup_Placement(sl_command, account, bustup, bustup_data, set_data, maker_command_data);
                    graphics.DrawImage(placed_bustup, 0, 0, template_width, template_height);
                }

                Bitmap text_overlay = new Bitmap(2, 2);

                maker_command_data.Dialogue = OfficialSetMethods.Validate_Input(sl_command, "P4D", "Dialogue", maker_command_data.Dialogue);

                switch (account.P4D_TS_Scene_Type)
                {
                    case "Dialogue":
                        text_overlay = Render_Dialogue_Overlay(sl_command, account, set_data, maker_command_data, bustup_data);
                        break;

                    case "Narration":
                        text_overlay = Render_Narration_Overlay(sl_command, account, maker_command_data, false);
                        break;
                }

                graphics.DrawImage(text_overlay, 0, 0, template_width, template_height);
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
            if (account.Auto_Delete_Commands == "On" && sl_command.CommandType == "Context")
            {
                await sl_command.Message.DeleteAsync();
            }
        }

        public async Task Render_System_Message(SocialLinkerCommand sl_command, MakerCommandData command_data)
        {
            SocketUser user = sl_command.User;
            SocketTextChannel channel = (SocketTextChannel)sl_command.Channel;
            RestUserMessage loader = await channel.SendMessageAsync("", false, P4D_Loading_Message().Build());
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

                Bitmap text_overlay = new Bitmap(2, 2);

                text_overlay = Render_Narration_Overlay(sl_command, account, command_data, true);

                graphics.DrawImage(text_overlay, 0, 0, template_width, template_height);
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
            if (account.Auto_Delete_Commands == "On" && sl_command.CommandType == "Context")
            {
                await sl_command.Message.DeleteAsync();
            }
        }

        public Bitmap Set_Bustup_Placement(SocialLinkerCommand sl_command, UserInfoFields account, Bitmap bustup, BustupData bustup_data, OfficialSetData set_data, MakerCommandData command_data)
        {
            // Create a starting base bitmap to render all graphics on.
            Bitmap base_template = new Bitmap(template_width, template_height);

            // Switch the rendering position of the bustup depending on the user's settings.
            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                switch (account.P4D_TS_Position)
                {
                    case "Left":
                        bustup = OfficialSetMethods.Reverse_Bustup_Selection(sl_command, set_data, bustup, bustup_data, command_data);

                        if (bustup_data.P4D_Dual_Flip == true)
                        {
                            bustup.RotateFlip(RotateFlipType.RotateNoneFlipX);
                        }

                        graphics.DrawImage(bustup, bustup_data.P4D_Left_Coord_X, bustup_data.P4D_Left_Coord_Y, bustup_data.P4D_Scale_Width, bustup_data.P4D_Scale_Height);
                        break;

                    case "Right":
                        graphics.DrawImage(bustup, bustup_data.P4D_Right_Coord_X, bustup_data.P4D_Right_Coord_Y, bustup_data.P4D_Scale_Width, bustup_data.P4D_Scale_Height);
                        break;

                    case "Center":
                        graphics.DrawImage(bustup, bustup_data.P4D_Center_Coord_X, bustup_data.P4D_Center_Coord_Y, bustup_data.P4D_Scale_Width, bustup_data.P4D_Scale_Height);
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
                Bitmap dialogue_layer_1 = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P4D//Main//dialogue_layer_1.png");
                Bitmap dialogue_layer_2 = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P4D//Main//dialogue_layer_2.png");
                Bitmap button_guide = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P4D//Main//button_guide.png");

                graphics.DrawImage(dialogue_layer_1, 0, 0, template_width, template_height);
                graphics.DrawImage(Render_Textbox_Glow(), 0, 0, template_width, template_height);
                graphics.DrawImage(dialogue_layer_2, 0, 0, template_width, template_height);
                graphics.DrawImage(button_guide, 0, 0, template_width, template_height);

                string display_name = OfficialSetMethods.GetDisplayName(account, command_data);
                display_name = OfficialSetMethods.Validate_Input(sl_command, "P4D", "Name", display_name);

                Bitmap rendered_name = Render_Name(display_name);
                Bitmap colored_name = Bitmap_To_Color(rendered_name, System.Drawing.Color.FromArgb(0, 141, 255), new Rectangle(120, 734, 1680, 80));
                graphics.DrawImage(colored_name, 0, 0, colored_name.Width, colored_name.Height);

                List<string>[] parsed_lines = OfficialSetMethods.Line_Parser(sl_command, "P4D", command_data.Dialogue, 3, 785);
                Bitmap rendered_dialogue = Render_Dialogue(parsed_lines);
                graphics.DrawImage(rendered_dialogue, 0, 0, rendered_dialogue.Width, rendered_dialogue.Height);
                graphics.DrawImage(Render_Cursor(account, 0, false), 0, 0, template_width, template_height);
            }

            return base_template;
        }

        public Bitmap Render_Narration_Overlay(SocialLinkerCommand sl_command, UserInfoFields account, MakerCommandData command_data, bool system_message)
        {
            Bitmap base_template = new Bitmap(template_width, template_height);

            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                Bitmap narrative_layer = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P4D//Main//narrative_layer.png");
                Bitmap button_guide = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P4D//Main//button_guide.png");

                List<string>[] parsed_lines = OfficialSetMethods.Line_Parser(sl_command, "P4D", command_data.Dialogue, 9, 785);
                int number_of_lines = Get_Number_of_Rendered_Lines(parsed_lines);

                graphics.DrawImage(narrative_layer, 0, 0, template_width, template_height);
                graphics.DrawImage(button_guide, 0, 0, template_width, template_height);

                graphics.DrawImage(Render_Cursor(account, number_of_lines, system_message), 0, 0, template_width, template_height); 

                Bitmap rendered_dialogue = Render_Narration_Dialogue(parsed_lines);
                graphics.DrawImage(rendered_dialogue, 0, 0, rendered_dialogue.Width, rendered_dialogue.Height);
            }

            return base_template;
        }

        public Bitmap Render_Name(string display_name)
        {
            //Create a bitmap as large as the template
            Bitmap bitmap = new Bitmap(960, 544);

            //Establish an int for the width and height glyphs should be rendered at
            int multiplier = 32;

            //Establish variables for where the glyphs should be rendered on the template
            int render_position_x = 64;
            int render_position_y = 373;

            string font_sheet = $@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P4D//Font//p4d_font_sheet.png";
            Bitmap current_glyph;

            char[] char_array = display_name.ToCharArray();

            for (int i = 0; i < char_array.Length; i++)
            {
                //Retrieve glyph information from the JSON file
                var glyph = ParsingMethods.Get_P4D_Glyph(char_array[i]);

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
                            graphics.DrawImage(current_glyph, (render_position_x - glyph.LeftCut), render_position_y, 30, 30);
                        }
                    }

                    //Set the next X value at the end of the current glyph's right width
                    render_position_x += (glyph.RightCut - glyph.LeftCut);
                }
            }

            // Resize for HD format
            int rescaled_width = 1920;
            int rescaled_height = 1088;
            var rescaled_bitmap = new Bitmap(rescaled_width, rescaled_height);

            using (Graphics graphics = Graphics.FromImage(rescaled_bitmap))
            {
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.DrawImage(bitmap, 0, 0, rescaled_width, rescaled_height);
            }
            bitmap = rescaled_bitmap;

            return bitmap;
        }

        public Bitmap Render_Dialogue(List<string>[] dialogue_lines)
        {
            //Create a bitmap as large as the template
            Bitmap bitmap = new Bitmap(960, 544);

            //Establish an int for the width and height glyphs should be rendered at
            int multiplier = 32;

            string font_sheet = $@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P4D//Font//p4d_font_sheet.png";
            Bitmap current_glyph;

            for (int i = 0; i < dialogue_lines.Length; i++)
            {
                //Establish variables for where the glyphs should be rendered on the template
                int render_position_x = 74;
                int render_position_y = 407 + (35 * i);

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
                                graphics.DrawImage(current_glyph, (render_position_x - glyph.LeftCut), render_position_y, 32, 32);
                            }
                        }

                        //Set the next X value at the end of the current glyph's right width
                        render_position_x += (glyph.RightCut - glyph.LeftCut);
                    }
                }
            }

            // Resize for HD format
            int rescaled_width = 1920;
            int rescaled_height = 1088;
            var rescaled_bitmap = new Bitmap(rescaled_width, rescaled_height);

            using (Graphics graphics = Graphics.FromImage(rescaled_bitmap))
            {
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.DrawImage(bitmap, 0, 0, rescaled_width, rescaled_height);
            }
            bitmap = rescaled_bitmap;

            return bitmap;
        }

        public Bitmap Render_Narration_Dialogue(List<string>[] dialogue_lines)
        {
            //Create a bitmap as large as the template
            Bitmap bitmap = new Bitmap(960, 544);

            //Establish an int for the width and height glyphs should be rendered at
            int multiplier = 32;

            string font_sheet = $@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P4D//Font//p4d_font_sheet.png";
            Bitmap current_glyph;

            for (int i = 0; i < dialogue_lines.Length; i++)
            {
                //Establish variables for where the glyphs should be rendered on the template
                int render_position_x = 88;
                int render_position_y = 118 + (34 * i);

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
                                graphics.DrawImage(current_glyph, (render_position_x - glyph.LeftCut), render_position_y, 32, 32);
                            }
                        }

                        //Set the next X value at the end of the current glyph's right width
                        render_position_x += (glyph.RightCut - glyph.LeftCut);
                    }
                }
            }

            // Resize for HD format
            int rescaled_width = 1920;
            int rescaled_height = 1088;
            var rescaled_bitmap = new Bitmap(rescaled_width, rescaled_height);

            using (Graphics graphics = Graphics.FromImage(rescaled_bitmap))
            {
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.DrawImage(bitmap, 0, 0, rescaled_width, rescaled_height);
            }
            bitmap = rescaled_bitmap;

            return bitmap;
        }

        public Bitmap Render_Textbox_Glow()
        {
            Bitmap base_template = new Bitmap(template_width, template_height);
            Bitmap dialogue_layer_1 = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P4D//Main//dialogue_layer_1.png");
            Bitmap gradient_1 = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P4D//Main//gradient_1.png");
            Bitmap gradient_2 = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P4D//Main//gradient_2.png");

            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                int x_min_1 = -1921;
                int y_min_1 = -136;
                int x_max_1 = 1803;
                int y_max_1 = 884;

                int random_x_1 = rnd.Next(x_min_1, x_max_1);
                int random_y_1 = rnd.Next(y_min_1, y_max_1);

                int x_min_2 = -1411;
                int y_min_2 = -134;
                int x_max_2 = 1803;
                int y_max_2 = 884;

                int random_x_2 = rnd.Next(x_min_2, x_max_2);
                int random_y_2 = rnd.Next(y_min_2, y_max_2);

                graphics.DrawImage(gradient_1, random_x_1, random_y_1, gradient_1.Width, gradient_1.Height);
                graphics.DrawImage(gradient_2, random_x_2, random_y_2, gradient_2.Width, gradient_2.Height);
            }

            base_template = Keep_Pixel_Overlap(dialogue_layer_1, base_template, false);

            return base_template;
        }

        public Bitmap Render_Cursor(UserInfoFields account, int number_of_lines, bool system_message)
        {
            Bitmap base_template = new Bitmap(960, 544);
            Bitmap cursor = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P4D//Main//continue_mark_narration.png");

            System.Drawing.Color cursor_color = default;

            switch (account.P4D_TS_Auto_Advance)
            {
                case "On":
                    cursor_color = System.Drawing.Color.FromArgb(255, 39, 239);
                    break;

                case "Off":
                    cursor_color = System.Drawing.Color.FromArgb(72, 105, 218);
                    break;
            }

            cursor = Bitmap_To_Color(cursor, cursor_color, new Rectangle(0, 0, cursor.Width, cursor.Height));

            int render_position_x = 0;
            int render_position_y = 0;

            if (system_message == true)
            {
                render_position_x = 869;
                render_position_y = 118 + (34 * (number_of_lines)) - 3;
            }
            else
            {
                switch (account.P4D_TS_Scene_Type)
                {
                    case "Dialogue":
                        render_position_x = 869;
                        render_position_y = 470;
                        break;

                    case "Narration":
                        render_position_x = 869;
                        render_position_y = 118 + (34 * (number_of_lines)) - 3;
                        break;
                }
            }

            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                graphics.DrawImage(cursor, render_position_x, render_position_y, 32, 32);
            }

            // Resize for HD format
            int rescaled_width = 1920;
            int rescaled_height = 1088;
            var rescaled_bitmap = new Bitmap(rescaled_width, rescaled_height);

            using (Graphics graphics = Graphics.FromImage(rescaled_bitmap))
            {
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.DrawImage(base_template, 0, 0, rescaled_width, rescaled_height);
            }
            base_template = rescaled_bitmap;

            return base_template;
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
                var glyph = ParsingMethods.Get_P4D_Glyph(char_array[i]);

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

        public static int Get_Number_of_Rendered_Lines(List<string>[] input_list_array)
        {
            // Initialize an int variable to hold the number of rendered lines.
            int number_of_lines = 0;

            // Take each index of the string list array, convert the list to a string, then analyze the string to determine if it's empty or not.
            // If it IS empty, that line won't be rendered.
            // Count the number of lines that will actually be rendered to the screen.

            for (int i = 0; i < input_list_array.Length; i++)
            {
                if (String_List_To_String(input_list_array[i]) != "")
                {
                    number_of_lines++;
                }
            }

            return number_of_lines;
        }

        // Coloring bitmaps
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

        public static Bitmap Keep_Pixel_Overlap(Bitmap bottom_bitmap, Bitmap top_bitmap, bool transfer_bottom_alpha)
        {
            System.Drawing.Color bottom_pixel_color;
            System.Drawing.Color top_pixel_color;
            
            Bitmap newBitmap = new Bitmap(bottom_bitmap.Width, bottom_bitmap.Height);
            for (int x = 0; x < top_bitmap.Width; x++)
            {
                for (int y = 0; y < top_bitmap.Height; y++)
                {
                    //Get the pixel from the scrBitmap image
                    bottom_pixel_color = bottom_bitmap.GetPixel(x, y);
                    top_pixel_color = top_bitmap.GetPixel(x, y);

                    if (bottom_pixel_color.A > 0 && top_pixel_color.A > 0)
                    {
                        if (transfer_bottom_alpha == true)
                        {
                            newBitmap.SetPixel(x, y, System.Drawing.Color.FromArgb(bottom_pixel_color.A, top_pixel_color.R, top_pixel_color.G, top_pixel_color.B));
                        }
                        else
                        {
                            newBitmap.SetPixel(x, y, System.Drawing.Color.FromArgb(top_pixel_color.A, top_pixel_color.R, top_pixel_color.G, top_pixel_color.B));
                        }
                        
                    }
                }
            }

            return newBitmap;
        }

        // Loading message
        public static EmbedBuilder P4D_Loading_Message()
        {
            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Generating Scene...",
                IconUrl = EmbedSettings.Get_Game_Logo("P4D")
            };

            embed.WithAuthor(author);
            embed.WithColor(EmbedSettings.Get_Game_Color("P4D", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Loading_Icon("P4D", null));
            embed.WithDescription("This may take a few seconds!");

            return embed;
        }
    }
}
