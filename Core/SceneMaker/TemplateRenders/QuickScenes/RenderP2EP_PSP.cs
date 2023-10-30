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
using System.Collections.Generic;
using SocialLinker.Core.SceneMaker.Data.Bustup;
using System.Drawing.Drawing2D;
using SocialLinker.Core.Menus;
using Color = System.Drawing.Color;
using System.Drawing.Imaging;

namespace SocialLinker.Core.SceneMaker.TemplateRenders.QuickScenes
{
    internal class RenderP2EP_PSP
    {
        int template_width = 480;
        int template_height = 272;

        public async Task Render_Quick_Scene_P2EP_PSP(SocialLinkerCommand sl_command)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = sl_command.User;
            SocketTextChannel channel = (SocketTextChannel)sl_command.Channel;

            OfficialSetData set_data = sl_command.MakerCommand.Character_Data.Set_Data;
            MakerCommandData maker_command_data = sl_command.MakerCommand;

            // Send a loading message to the channel while the sprite sheet is being made.
            RestUserMessage loader = await channel.SendMessageAsync("", false, P2EP_PSP_Loading_Message().Build());

            // Get the account information of the command's user.
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

            // Next, time for the conversation portrait! Create and initialize a new bitmap variable for it.
            Bitmap bustup = new Bitmap(2, 2);

            // Check if the base sprite number is something other than zero.
            // If it is zero, we have nothing to render. Otherwise, retrieve the bustup.
            if (maker_command_data.Character_Data.Base_Sprite != 0)
            {
                bustup = OfficialSetMethods.Bustup_Selection(sl_command, account, set_data, bustup_data, maker_command_data);
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
                string window_color = "";

                switch(account.P2EP_PSP_TS_Window_Color)
                {
                    case "Type 1":
                        window_color = "type_1";
                        break;

                    case "Type 2":
                        window_color = "type_2";
                        break;

                    case "Type 3":
                        window_color = "type_3";
                        break;

                    case "Type 4":
                        window_color = "type_4";
                        break;

                    case "Type 5":
                        window_color = "type_5";
                        break;

                    case "Type 6":
                        window_color = "type_6";
                        break;
                }

                // Create and assign bitmap variables for the assets needed.
                Bitmap message_window = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P2EP-PSP//{window_color}.png");

                // Draw the layer with the user's colored default background if it exists.
                graphics.DrawImage(colored_background_bitmap, 0, 0, template_width, template_height);

                // Draw the user's background to the base template.
                graphics.DrawImage(background, 0, 0, template_width, template_height);

                // Draw the first textbox layer to the base template.
                graphics.DrawImage(message_window, 0, 0, template_width, template_height);

                // Draw the character bust-up to the template if the base sprite number is not '0'.
                if (maker_command_data.Character_Data.Base_Sprite != 0)
                {
                    Bitmap placed_bustup = Set_Bustup_Placement(sl_command, account, bustup, bustup_data, set_data, maker_command_data);
                    graphics.DrawImage(placed_bustup, 0, 0, template_width, template_height);
                }
            }

            System.Drawing.Color display_name_color = System.Drawing.Color.FromArgb(246, 243, 66);

            string display_name = OfficialSetMethods.GetDisplayName(account, maker_command_data);
            display_name = OfficialSetMethods.Validate_Input(sl_command, "P2EP-PSP", "Name", display_name);

            Bitmap display_name_layer = Render_Name(display_name);
            Rectangle display_name_area = new Rectangle(25, 200, 455, 16);

            // Create another graphics object for the base template.
            // We'll start rendering our needed text here.
            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                // Check if the base sprite number is something other than zero. If so, render the display name of the chosen sprite to the template.
                if (maker_command_data.Character_Data.Base_Sprite != 0)
                {
                    graphics.DrawImage(Bitmap_To_Color(display_name_layer, display_name_color, display_name_area), 0, 0, template_width, template_height);
                }
                // If the base sprite number IS zero, we need a sprite to actually retrieve a display name from.
                else
                {
                    // Change the base sprite number from the command data to one.
                    // This way, we can get the bustup data for the first sprite to retrieve its display name.
                    maker_command_data.Character_Data.Base_Sprite = 1;

                    // Get the bustup data for the first sprite and render the display name to the template.
                    bustup_data = BustupDataMethods.Get_Bustup_Data(account, set_data, maker_command_data);
                    graphics.DrawImage(Bitmap_To_Color(display_name_layer, display_name_color, display_name_area), 0, 0, template_width, template_height);
                }

                // Draw the input dialogue to the template.
                maker_command_data.Dialogue = OfficialSetMethods.Validate_Input(sl_command, "P2EP-PSP", "Dialogue", maker_command_data.Dialogue);
                List<string>[] dialogue_lines = OfficialSetMethods.Line_Parser(sl_command, "P2EP-PSP", maker_command_data.Dialogue, 3, 370);
                graphics.DrawImage(Render_Dialogue(dialogue_lines, false), 0, 0, template_width, template_height);

                Bitmap cursor = Render_Cursor(account);
                graphics.DrawImage(cursor, 0, 0, cursor.Width, cursor.Height);
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

        public async Task Render_System_Message(SocialLinkerCommand sl_command, MakerCommandData command_data)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = sl_command.User;
            SocketTextChannel channel = (SocketTextChannel)sl_command.Channel;

            // Send a loading message to the channel while the sprite sheet is being made.
            RestUserMessage loader = await channel.SendMessageAsync("", false, P2EP_PSP_Loading_Message().Build());

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
                string window_color = "";

                switch (account.P2EP_PSP_TS_Window_Color)
                {
                    case "Type 1":
                        window_color = "type_1";
                        break;

                    case "Type 2":
                        window_color = "type_2";
                        break;

                    case "Type 3":
                        window_color = "type_3";
                        break;

                    case "Type 4":
                        window_color = "type_4";
                        break;

                    case "Type 5":
                        window_color = "type_5";
                        break;

                    case "Type 6":
                        window_color = "type_6";
                        break;
                }

                // Create and assign bitmap variables for the assets needed.
                Bitmap message_window = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P2EP-PSP//{window_color}.png");

                // Draw the layer with the user's colored default background if it exists.
                graphics.DrawImage(colored_background_bitmap, 0, 0, template_width, template_height);

                // Draw the user's background to the base template.
                graphics.DrawImage(background, 0, 0, template_width, template_height);

                // Draw the first textbox layer to the base template.
                graphics.DrawImage(message_window, 0, 0, template_width, template_height);
            }

            // Create another graphics object for the base template.
            // We'll start rendering our needed text here.
            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                // Draw the input dialogue to the template.
                command_data.Dialogue = OfficialSetMethods.Validate_Input(sl_command, "P2EP-PSP", "Dialogue", command_data.Dialogue);
                List<string>[] dialogue_lines = OfficialSetMethods.Line_Parser(sl_command, "P2EP-PSP", command_data.Dialogue, 4, 370);
                graphics.DrawImage(Render_Dialogue(dialogue_lines, true), 0, 0, template_width, template_height);

                Bitmap cursor = Render_Cursor(account);
                graphics.DrawImage(cursor, 0, 0, cursor.Width, cursor.Height);
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

        public Bitmap Set_Bustup_Placement(SocialLinkerCommand sl_command, UserInfoFields account, Bitmap bustup, BustupData bustup_data, OfficialSetData set_data, MakerCommandData command_data)
        {
            Bitmap base_template = new Bitmap(template_width, template_height);

            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                if (account.P2EP_PSP_TS_Invert == "On")
                {
                    bustup = Invert_Bitmap(bustup);
                }

                if (account.P2EP_PSP_TS_Sprite_Flip == "On")
                {
                    bustup.RotateFlip(RotateFlipType.Rotate180FlipY);
                }

                switch (account.P2EP_PSP_TS_Position)
                {
                    case "Default":
                        switch (bustup_data.P2EP_PSP_Default_Position)
                        {
                            case "Left":
                                graphics.DrawImage(bustup, bustup_data.P2EP_PSP_Left_Coord_X, bustup_data.P2EP_PSP_Left_Coord_Y, bustup_data.P2EP_PSP_Scale_Width, bustup_data.P2EP_PSP_Scale_Height);
                                break;

                            case "Right":
                                graphics.DrawImage(bustup, bustup_data.P2EP_PSP_Right_Coord_X, bustup_data.P2EP_PSP_Right_Coord_Y, bustup_data.P2EP_PSP_Scale_Width, bustup_data.P2EP_PSP_Scale_Height);
                                break;
                        }
                        break;

                    case "Left":
                        graphics.DrawImage(bustup, bustup_data.P2EP_PSP_Left_Coord_X, bustup_data.P2EP_PSP_Left_Coord_Y, bustup_data.P2EP_PSP_Scale_Width, bustup_data.P2EP_PSP_Scale_Height);
                        break;

                    case "Right":
                        graphics.DrawImage(bustup, bustup_data.P2EP_PSP_Right_Coord_X, bustup_data.P2EP_PSP_Right_Coord_Y, bustup_data.P2EP_PSP_Scale_Width, bustup_data.P2EP_PSP_Scale_Height);
                        break;
                }
            }

            return base_template;
        }

        public Bitmap Render_Name(string display_name)
        {
            Bitmap base_template = new Bitmap(template_width, template_height);

            // Establish an int for the width and height glyphs should be rendered at.
            int multiplier = 16;

            // Load the bitmap font.
            string font_sheet = $@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P2EP-PSP//Font//p2ep-psp_font_sheet.png";

            // Create a variable to iterate through sections of the bitmap font.
            Bitmap current_glyph;

            // Specify X and Y coordinates for where the glyphs should start rendering on the template.
            int render_position_x = 25;
            int render_position_y = 200;

            // Thake the sprite's display name and convert it into a char array.
            char[] char_array = display_name.ToCharArray();

            // Iterate through each character of the array.
            for (int i = 0; i < char_array.Length; i++)
            {
                // Retrieve glyph information from the JSON file.
                var glyph = ParsingMethods.Get_P2IS_PSP_Glyph(char_array[i]);

                // Check if the character is a line break.
                if (char_array[i] == '\u000a')
                {
                    // Set the X coordinate to the start of the row.
                    render_position_x = 33;
                    // Move the Y coordinate down to the next line.
                    render_position_y += 18;
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
                    render_position_x += (glyph.RightCut - glyph.LeftCut) + 1;
                }
            }

            return base_template;
        }

        public Bitmap Render_Dialogue(List<string>[] dialogue_lines, bool system_message_check)
        {
            Bitmap bitmap = new Bitmap(template_width, template_height);

            // Create an int to keep track of rendering errors. This is neccessary to inform the user of any potential issues.
            int error_counter = 0;

            //Establish an int for the width and height glyphs should be rendered at
            int multiplier = 16;

            string font_sheet = $@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P2EP-PSP//Font//p2ep-psp_font_sheet.png";
            Bitmap current_glyph;

            // Iterate over each line of the dialogue string list with a loop.
            for (int i = 0; i < dialogue_lines.Length; i++)
            {
                // Specify X and Y coordinates for where the glyphs should start rendering on the template.
                int render_position_x = 33;
                int render_position_y = 0;

                switch (system_message_check)
                {
                    case true:
                        render_position_y = 200 + (18 * i);
                        break;

                    case false:
                        render_position_y = 218 + (18 * i);
                        break;
                }

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

                                // Draw the glyph to the base bitmap.
                                graphics.DrawImage(current_glyph, (render_position_x - glyph.LeftCut), render_position_y, multiplier, multiplier);
                            }
                        }

                        //Set the next X value at the end of the current glyph's right width
                        render_position_x += (glyph.RightCut - glyph.LeftCut) + 1;
                    }
                }
            }

            return bitmap;
        }

        public Bitmap Render_Cursor(UserInfoFields account)
        {
            Bitmap base_template = new Bitmap(template_width, template_height);
            Bitmap bright_square = new Bitmap(3, 3);
            Bitmap dark_square = new Bitmap(3, 3);

            Color bright_square_color = default;
            Color dark_square_color = default;

            switch (account.P2EP_PSP_TS_Window_Color)
            {
                case "Type 1":
                    bright_square_color = Color.FromArgb(200, 200, 200);
                    dark_square_color = Color.FromArgb(67, 67, 67);
                    break;

                case "Type 2":
                    bright_square_color = Color.FromArgb(216, 16, 16);
                    dark_square_color = Color.FromArgb(79, 7, 6);
                    break;

                case "Type 3":
                    bright_square_color = Color.FromArgb(108, 204, 252);
                    dark_square_color = Color.FromArgb(37, 68, 89);
                    break;

                case "Type 4":
                    bright_square_color = Color.FromArgb(255, 228, 85);
                    dark_square_color = Color.FromArgb(84, 76, 27);
                    break;

                case "Type 5":
                    bright_square_color = Color.FromArgb(252, 168, 183);
                    dark_square_color = Color.FromArgb(98, 8, 10);
                    break;

                case "Type 6":
                    bright_square_color = Color.FromArgb(207, 105, 251);
                    dark_square_color = Color.FromArgb(71, 34, 88);
                    break;
            }

            using (Graphics graphics = Graphics.FromImage(bright_square))
            {
                graphics.Clear(bright_square_color);
            }

            using (Graphics graphics = Graphics.FromImage(dark_square))
            {
                graphics.Clear(dark_square_color);
            }

            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                Random rnd = new Random();
                int cursor_version = rnd.Next(1, 5);

                graphics.DrawImage(bright_square, 457, 260, 3, 3);
                graphics.DrawImage(bright_square, 461, 260, 3, 3);
                graphics.DrawImage(bright_square, 457, 264, 3, 3);
                graphics.DrawImage(bright_square, 461, 264, 3, 3);

                switch (cursor_version)
                {
                    case 1:
                        graphics.DrawImage(dark_square, 457, 260, 3, 3);
                        break;

                    case 2:
                        graphics.DrawImage(dark_square, 461, 260, 3, 3);
                        break;

                    case 3:
                        graphics.DrawImage(dark_square, 457, 264, 3, 3);
                        break;

                    case 4:
                        graphics.DrawImage(dark_square, 461, 264, 3, 3);
                        break;
                }
            }

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
                var glyph = ParsingMethods.Get_P2EP_PSP_Glyph(char_array[i]);

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

        public Bitmap Scale_Template(UserInfoFields account, Bitmap input_template)
        {
            var scaled_bitmap = new Bitmap(2, 2);
            int scaled_width = template_width;
            int scaled_height = template_height;

            if (account.P2IS_PSP_Resolution == "480 × 272")
            {
                // Do nothing if setting is at default resolution
            }
            else
            {
                if (account.P2IS_PSP_Resolution == "1920 × 1088")
                {
                    scaled_width = 1920;
                    scaled_height = 1088;
                }

                var copied_input = new Bitmap(input_template);
                scaled_bitmap = new Bitmap(scaled_width, scaled_height);

                using (Graphics graphics = Graphics.FromImage(scaled_bitmap))
                {
                    switch (account.P2EP_PSP_Scale)
                    {
                        case "Bicubic":
                            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            break;

                        case "Nearest Neighbor":
                            graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                            break;
                    }

                    graphics.CompositingQuality = CompositingQuality.HighQuality;
                    graphics.DrawImage(copied_input, 0, 0, scaled_width + 2, scaled_height + 2);
                }

                input_template = scaled_bitmap;
            }

            return input_template;
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
                        Color inv = input_bitmap.GetPixel(x, y);
                        inv = Color.FromArgb(inv.A, (255 - inv.R), (255 - inv.G), (255 - inv.B));
                        base_template.SetPixel(x, y, inv);
                    }
                }
            }

            return base_template;
        }

        // Loading message
        public static EmbedBuilder P2EP_PSP_Loading_Message()
        {
            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = $"Generating Scene...",
                IconUrl = EmbedSettings.Get_Game_Logo("P2EP-PSP")
            };

            embed.WithAuthor(author);
            embed.WithColor(EmbedSettings.Get_Game_Color("P2EP-PSP", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Loading_Icon("P2EP-PSP", null));
            embed.WithDescription("This may take a few seconds!");

            return embed;
        }
    }
}
