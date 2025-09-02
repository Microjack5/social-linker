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
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using SocialLinker.Core.Menus;
using System.Timers;
using System.Security.Principal;

namespace SocialLinker.Core.SceneMaker.TemplateRenders.QuickScenes
{
    public class RenderP2EP_PS1 : ModuleBase<SocketCommandContext>
    {
        int template_width = 320;
        int template_height = 240;

        System.Drawing.Color ep_yellow = System.Drawing.Color.FromArgb(222, 222, 74);
        System.Drawing.Color ep_dark_yellow = System.Drawing.Color.FromArgb(49, 49, 24);
        System.Drawing.Color ep_green = System.Drawing.Color.FromArgb(132, 230, 132);
        System.Drawing.Color ep_black = System.Drawing.Color.FromArgb(24, 24, 24);

        Bitmap font_sheet = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P2EP-PS1//Font//p2ep-ps1_font_sheet.png");
        Bitmap ba_gua_bitmap = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P2EP-PS1//Font//p2ep-ps1_font_sheet_2.png");
        Bitmap heart_bitmap = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P2EP-PS1//Font//p2ep-ps1_font_sheet_3.png");

        char[] hearts = { '♥', '♡', '❣', '❤' };
        char[] ba_gua = { '☰', '☱', '☲', '☳', '☴', '☵', '☶', '☷' };
        char[] abnormals = { 'g', 'j', 'p', 'q', 'y' };

        public async Task Render_Quick_Scene_P2EP_PS1(SocialLinkerCommand sl_command)
        {
            SocketUser user = sl_command.User;
            SocketTextChannel channel = (SocketTextChannel)sl_command.Channel;

            OfficialSetData set_data = sl_command.MakerCommand.Character_Data_1.Set_Data;
            MakerCommandData maker_command_data = sl_command.MakerCommand;

            RestUserMessage loader = await channel.SendMessageAsync("", false, P2EP_PS1_Loading_Message().Build());

            var account = UserInfoClasses.GetAccount(user);

            sl_command.MakerCommand.Character_Data_1.Bustup_Data = BustupDataMethods.Get_Bustup_Data(account, set_data, sl_command.MakerCommand.Character_Data_1);
            BustupData bustup_data = sl_command.MakerCommand.Character_Data_1.Bustup_Data;

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
            if (maker_command_data.Character_Data_1.Base_Sprite != 0)
            {
                bustup = OfficialSetMethods.Bustup_Selection(sl_command, account, sl_command.MakerCommand.Character_Data_1);
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
                graphics.DrawImage(colored_background_bitmap, 0, 0, template_width, template_height);
                graphics.DrawImage(background, 0, 0, template_width, template_height);

                if (maker_command_data.Character_Data_1.Base_Sprite != 0)
                {
                    Bitmap placed_bustup = Set_Bustup_Placement(account, bustup, bustup_data);
                    graphics.DrawImage(placed_bustup, 0, 0, template_width, template_height);
                }

                graphics.DrawImage(Render_Message_Window(account), 0, 0, template_width, template_height);

                string display_name = OfficialSetMethods.GetDisplayName(account, sl_command.MakerCommand.Character_Data_1);
                display_name = OfficialSetMethods.Validate_Input(sl_command, "P2EP-PS1", "Name", display_name);

                maker_command_data.Dialogue = OfficialSetMethods.Validate_Input(sl_command, "P2EP-PS1", "Dialogue", maker_command_data.Dialogue);
                List<string>[] dialogue_lines = OfficialSetMethods.Line_Parser(sl_command, "P2EP-PS1", maker_command_data.Dialogue, 3, 230);

                graphics.DrawImage(Combined_Text_Layers(display_name, dialogue_lines), 0, 0, template_width, template_height);
                graphics.DrawImage(Render_Cursor(), 0, 0, template_width, template_height);
            }

            base_template = Scale_Template(account, base_template);

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

            RestUserMessage loader = await channel.SendMessageAsync("", false, P2EP_PS1_Loading_Message().Build());

            var account = UserInfoClasses.GetAccount(user);

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

            command_data.Dialogue = OfficialSetMethods.Validate_Input(sl_command, "P2EP-PS1", "Dialogue", command_data.Dialogue);
            List<string>[] dialogue_lines = OfficialSetMethods.Line_Parser(sl_command, "P2EP-PS1", command_data.Dialogue, 4, 225);

            // Time to put it all together!
            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                graphics.DrawImage(colored_background_bitmap, 0, 0, template_width, template_height);
                graphics.DrawImage(background, 0, 0, template_width, template_height);
                graphics.DrawImage(Render_Message_Window(account), 0, 0, template_width, template_height);
                graphics.DrawImage(Combined_Text_Layers_System(dialogue_lines), 0, 0, template_width, template_height);
                graphics.DrawImage(Render_Cursor(), 0, 0, template_width, template_height);
            }

            base_template = Scale_Template(account, base_template);

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

        public Bitmap Set_Bustup_Placement(UserInfoFields account, Bitmap bustup, BustupData bustup_data)
        {
            Bitmap base_template = new Bitmap(template_width, template_height);

            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                if (account.P2EP_PSX_TS_Invert == "On")
                {
                    bustup = Invert_Bitmap(bustup);
                }

                if (account.P2EP_PSX_TS_Sprite_Flip == "On")
                {
                    bustup.RotateFlip(RotateFlipType.Rotate180FlipY);
                }

                switch (account.P2EP_PSX_TS_Position)
                {
                    case "Default":
                        switch (bustup_data.P2EP_PSX_Default_Position)
                        {
                            case "Left":
                                graphics.DrawImage(bustup, bustup_data.P2EP_PSX_Left_Coord_X, bustup_data.P2EP_PSX_Left_Coord_Y, bustup_data.P2EP_PSX_Scale_Width, bustup_data.P2EP_PSX_Scale_Height);
                                break;

                            case "Right":
                                graphics.DrawImage(bustup, bustup_data.P2EP_PSX_Right_Coord_X, bustup_data.P2EP_PSX_Right_Coord_Y, bustup_data.P2EP_PSX_Scale_Width, bustup_data.P2EP_PSX_Scale_Height);
                                break;
                        }
                        break;

                    case "Left":
                        graphics.DrawImage(bustup, bustup_data.P2EP_PSX_Left_Coord_X, bustup_data.P2EP_PSX_Left_Coord_Y, bustup_data.P2EP_PSX_Scale_Width, bustup_data.P2EP_PSX_Scale_Height);
                        break;

                    case "Right":
                        graphics.DrawImage(bustup, bustup_data.P2EP_PSX_Right_Coord_X, bustup_data.P2EP_PSX_Right_Coord_Y, bustup_data.P2EP_PSX_Scale_Width, bustup_data.P2EP_PSX_Scale_Height);
                        break;
                }
            }

            return base_template;
        }

        public Bitmap Render_Message_Window(UserInfoFields account)
        {
            int template_width = 320;
            int template_height = 240;

            Bitmap base_template = new Bitmap(template_width, template_height);
            Bitmap window_frame = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P2EP-PS1//frame.png");
            Bitmap wallpaper = new Bitmap(template_width, template_height);

            switch (account.P2EP_PSX_TS_Wallpaper)
            {
                case "Blue Tone":
                    wallpaper = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P2EP-PS1//Wallpaper//1.png");
                    break;

                case "Sepia Tone":
                    wallpaper = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P2EP-PS1//Wallpaper//2.png");
                    break;

                case "Purple Tone":
                    wallpaper = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P2EP-PS1//Wallpaper//3.png");
                    break;

                case "Seventh":
                    wallpaper = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P2EP-PS1//Wallpaper//4.png");
                    break;

                case "Baofu":
                    wallpaper = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P2EP-PS1//Wallpaper//5.png");
                    break;

                case "NWO":
                    wallpaper = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P2EP-PS1//Wallpaper//6.png");
                    break;

                case "Dragon":
                    wallpaper = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P2EP-PS1//Wallpaper//7.png");
                    break;

                case "Jack Frost":
                    wallpaper = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P2EP-PS1//Wallpaper//8.png");
                    break;

                case "Grid":
                    wallpaper = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P2EP-PS1//Wallpaper//9.png");
                    break;

                case "Star":
                    wallpaper = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P2EP-PS1//Wallpaper//10.png");
                    break;
            }

            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                wallpaper = SetImageOpacity(wallpaper, (float)0.5);

                graphics.DrawImage(wallpaper, 0, 0, wallpaper.Width, wallpaper.Height);
                graphics.DrawImage(window_frame, 0, 0, window_frame.Width, window_frame.Height);
            }

            return base_template;
        }

        public Bitmap Combined_Text_Layers(string display_name, List<string>[] dialogue_lines)
        {
            Bitmap base_template = new Bitmap(template_width, template_height);

            Bitmap rendered_name = Render_Name(display_name);
            Bitmap rendered_dialogue = Render_Dialogue(dialogue_lines, false);

            Bitmap dialogue_front = rendered_dialogue;
            Bitmap dialogue_back = Bitmap_To_Color(rendered_dialogue, ep_black, new Rectangle(0, 0, template_width, template_height));

            Bitmap display_name_front = Bitmap_To_Color(rendered_name, ep_yellow, new Rectangle(0, 0, template_width, template_height));
            Bitmap display_name_back = Bitmap_To_Color(rendered_name, ep_dark_yellow, new Rectangle(0, 0, template_width, template_height));

            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                graphics.DrawImage(display_name_back, 1, 1, template_width, template_height);
                graphics.DrawImage(display_name_front, 0, 0, template_width, template_height);

                graphics.DrawImage(dialogue_back, 1, 1, template_width, template_height);
                graphics.DrawImage(dialogue_front, 0, 0, template_width, template_height);
            }

            return base_template;
        }

        public Bitmap Combined_Text_Layers_System(List<string>[] dialogue_lines)
        {
            Bitmap base_template = new Bitmap(template_width, template_height);

            Bitmap rendered_dialogue = Render_Dialogue(dialogue_lines, true);

            Bitmap dialogue_front = Bitmap_To_Color(rendered_dialogue, ep_green, new Rectangle(0, 0, template_width, template_height));
            Bitmap dialogue_back = Bitmap_To_Color(rendered_dialogue, ep_black, new Rectangle(0, 0, template_width, template_height));

            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                graphics.DrawImage(dialogue_back, 1, 1, template_width, template_height);
                graphics.DrawImage(dialogue_front, 0, 0, template_width, template_height);
            }

            return base_template;
        }

        public Bitmap Render_Name(string display_name)
        {
            Bitmap base_template = new Bitmap(template_width, template_height);

            // Establish ints for the width and height of glyphs.
            int x_multiplier = 8;
            int y_multiplier = 12;

            // Create a variable to iterate through sections of the bitmap font.
            Bitmap current_glyph;

            // Specify X and Y coordinates for where the glyphs should start rendering on the template.
            int render_position_x = 16;
            int render_position_y = 171;

            // Take the sprite's display name and convert it into a char array.
            char[] char_array = display_name.ToCharArray();

            // Iterate through each character of the array.
            for (int i = 0; i < char_array.Length; i++)
            {
                // Retrieve glyph information from the JSON file.
                var glyph = ParsingMethods.Get_P2EP_PS1_Glyph(char_array[i]);

                // Check if the character is a line break.
                if (char_array[i] == '\u000a')
                {
                    // Set the X coordinate to the start of the row.
                    render_position_x = 16;
                    // Move the Y coordinate down to the next line.
                    render_position_y += 14;
                }
                else if (glyph != null)
                {
                    int x = x_multiplier * glyph.Column;
                    int y = y_multiplier * glyph.Row;

                    using (Graphics graphics = Graphics.FromImage(base_template))
                    {
                        // Copy the section of the bitmap font needed.
                        Rectangle cropped_section = new Rectangle(x, y, x_multiplier, y_multiplier);
                        current_glyph = font_sheet.Clone(cropped_section, font_sheet.PixelFormat);

                        // Draw the glyph to the base bitmap. Some hanging letters will need to be drawn a bit lower to appear natural.
                        if (abnormals.Contains(char_array[i]))
                        {
                            graphics.DrawImage(current_glyph, (render_position_x - glyph.LeftCut), render_position_y + 1, x_multiplier, y_multiplier);
                        }
                        else
                        {
                            graphics.DrawImage(current_glyph, (render_position_x - glyph.LeftCut), render_position_y, x_multiplier, y_multiplier);
                        }
                    }

                    // Set the next X value at the end of the current glyph's right width.
                    render_position_x += (glyph.RightCut - glyph.LeftCut) + 1;
                }
                // Exception for heart symbol which is separated from font sheet
                else if (glyph == null && hearts.Contains(char_array[i]))
                {
                    using (Graphics graphics = Graphics.FromImage(base_template))
                    {
                        graphics.DrawImage(heart_bitmap, (render_position_x - 0), render_position_y, heart_bitmap.Width, heart_bitmap.Height);
                    }

                    render_position_x += 11 + 1;
                }
                // Exception for ba gua symbols which are separated from font sheet
                else if (glyph == null && ba_gua.Contains(char_array[i]))
                {
                    Rectangle crop = new Rectangle(0, 0, 2, 2);

                    switch (char_array[i])
                    {
                        case '☰':
                            crop = new Rectangle(0, 0, 10, 11);
                            break;

                        case '☱':
                            crop = new Rectangle(12, 0, 10, 11);
                            break;

                        case '☲':
                            crop = new Rectangle(24, 0, 10, 11);
                            break;

                        case '☳':
                            crop = new Rectangle(36, 0, 10, 11);
                            break;

                        case '☴':
                            crop = new Rectangle(0, 14, 10, 11);
                            break;

                        case '☵':
                            crop = new Rectangle(12, 14, 10, 11);
                            break;

                        case '☶':
                            crop = new Rectangle(24, 14, 10, 11);
                            break;

                        case '☷':
                            crop = new Rectangle(36, 14, 10, 11);
                            break;
                    }

                    using (Graphics graphics = Graphics.FromImage(base_template))
                    {
                        Bitmap ba_bua_symbol = ba_gua_bitmap.Clone(crop, ba_gua_bitmap.PixelFormat);
                        graphics.DrawImage(ba_bua_symbol, (render_position_x - 0), render_position_y, ba_bua_symbol.Width, ba_bua_symbol.Height);
                    }

                    render_position_x += 10 + 1;
                }
            }

            return base_template;
        }

        public Bitmap Render_Dialogue(List<string>[] dialogue_lines, bool system_message_check)
        {
            Bitmap base_template = new Bitmap(template_width, template_height);

            // Establish ints for the width and height of glyphs.
            int x_multiplier = 8;
            int y_multiplier = 12;

            Bitmap current_glyph;

            // Iterate over each line of the dialogue string list with a loop.
            for (int i = 0; i < dialogue_lines.Length; i++)
            {
                // Specify X and Y coordinates for where the glyphs should start rendering on the template.
                int render_position_x = 21;
                int render_position_y = 0;

                switch (system_message_check)
                {
                    case true:
                        render_position_y = 171 + (14 * i);
                        break;

                    case false:
                        render_position_y = 185 + (14 * i);
                        break;
                }

                // Take the input dialogue and convert it into a char array.
                char[] char_array = String_List_To_String(dialogue_lines[i]).ToCharArray();

                // Iterate through each character of the array.
                for (int j = 0; j < char_array.Length; j++)
                {
                    //Retrieve glyph information from the JSON file
                    var glyph = ParsingMethods.Get_P2EP_PS1_Glyph(char_array[j]);

                    // If the glyph info returns null, we have a rendering error.
                    // A warning message should have already been sent to the user in the Measure_Word_Pixel_Length method.
                    if (glyph == null)
                    {
                        // Do nothing
                    }

                    if (glyph != null)
                    {
                        int x = x_multiplier * glyph.Column;
                        int y = y_multiplier * glyph.Row;

                        using (Graphics graphics = Graphics.FromImage(base_template))
                        {
                            // Copy the section of the bitmap font needed.
                            Rectangle cropped_section = new Rectangle(x, y, x_multiplier, y_multiplier);
                            current_glyph = font_sheet.Clone(cropped_section, font_sheet.PixelFormat);

                            // Draw the glyph to the base bitmap. Some hanging letters will need to be drawn a bit lower to appear natural.
                            if (abnormals.Contains(char_array[j]))
                            {
                                graphics.DrawImage(current_glyph, (render_position_x - glyph.LeftCut), render_position_y + 1, x_multiplier, y_multiplier);
                            }
                            else
                            {
                                graphics.DrawImage(current_glyph, (render_position_x - glyph.LeftCut), render_position_y, x_multiplier, y_multiplier);
                            }
                        }

                        // Set the next X value at the end of the current glyph's right width, plus 1 for spacing.
                        render_position_x += (glyph.RightCut - glyph.LeftCut) + 1;
                    }
                    // Exception for heart symbol which is separated from font sheet
                    else if (glyph == null && hearts.Contains(char_array[j]))
                    {
                        using (Graphics graphics = Graphics.FromImage(base_template))
                        {
                            graphics.DrawImage(heart_bitmap, (render_position_x - 0), render_position_y, heart_bitmap.Width, heart_bitmap.Height);
                        }

                        render_position_x += 11 + 1;
                    }
                    // Exception for ba gua symbols which are separated from font sheet
                    else if (glyph == null && ba_gua.Contains(char_array[j]))
                    {
                        Rectangle crop = new Rectangle(0, 0, 2, 2);

                        switch (char_array[j])
                        {
                            case '☰':
                                crop = new Rectangle(0, 0, 10, 11);
                                break;

                            case '☱':
                                crop = new Rectangle(12, 0, 10, 11);
                                break;

                            case '☲':
                                crop = new Rectangle(24, 0, 10, 11);
                                break;

                            case '☳':
                                crop = new Rectangle(36, 0, 10, 11);
                                break;

                            case '☴':
                                crop = new Rectangle(0, 14, 10, 11);
                                break;

                            case '☵':
                                crop = new Rectangle(12, 14, 10, 11);
                                break;

                            case '☶':
                                crop = new Rectangle(24, 14, 10, 11);
                                break;

                            case '☷':
                                crop = new Rectangle(36, 14, 10, 11);
                                break;
                        }

                        using (Graphics graphics = Graphics.FromImage(base_template))
                        {
                            Bitmap ba_bua_symbol = ba_gua_bitmap.Clone(crop, ba_gua_bitmap.PixelFormat);
                            graphics.DrawImage(ba_bua_symbol, (render_position_x - 0), render_position_y, ba_bua_symbol.Width, ba_bua_symbol.Height);
                        }

                        render_position_x += 10 + 1;
                    }
                }
            }

            return base_template;
        }

        public Bitmap Render_Cursor()
        {
            Random rnd = new Random();
            Bitmap base_template = new Bitmap(template_width, template_height);

            int weighted_random = rnd.Next(1, 48);

            int cursor_frame = 0;

            if (weighted_random >= 10)
            {
                cursor_frame = weighted_random / 4;
            }
            else
            {
                cursor_frame = 1;
            }

            Bitmap cursor = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P2EP-PS1//Cursor//{cursor_frame}.png");
            
            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                graphics.DrawImage(cursor, 0, 0, template_width, template_height);
            }

            return base_template;
        }

        public int Measure_Word_Pixel_Length(SocialLinkerCommand sl_command, string input_word)
        {
            // Create an int to keep track of how many pixels a glyph is wide in.
            int pixel_counter = 0;

            // Take the input string and turn it into a char array.
            char[] char_array = input_word.ToCharArray();

            // Now, let's iterate through the char array.
            for (int i = 0; i < char_array.Length; i++)
            {
                // Retrieve glyph information for the current character from the JSON file.
                var glyph = ParsingMethods.Get_P2EP_PS1_Glyph(char_array[i]);

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
                // Exception for heart symbol which is separated from font sheet
                else if (glyph == null && hearts.Contains(char_array[i]))
                {
                    pixel_counter += 11; // Logic: Right cut (11) - Left cut (0)
                }
                // Exception for ba gua symbols which are separated from font sheet
                else if (glyph == null && ba_gua.Contains(char_array[i]))
                {
                    pixel_counter += 10; // Logic: Right cut (10) - Left cut (0)
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
            string output_string = "";

            for (int i = 0; i < input_list.Count; i++)
            {
                output_string += input_list[i];
            }

            return output_string;
        }

        // Coloring Bitmaps
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

        public static Bitmap SetImageOpacity(Bitmap input_bitmap, float opacity)
        {
            try
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
            catch (Exception ex)
            {
                Console.WriteLine($"Here\n {ex}");
                return input_bitmap;
            }
        }

        // Method from https://stackoverflow.com/questions/33024881/invert-image-faster-in-c-sharp
        public Bitmap Invert_Bitmap(Bitmap input_bitmap)
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

        public Bitmap Scale_Template(UserInfoFields account, Bitmap input_template)
        {
            var scaled_bitmap = new Bitmap(2, 2);
            int scaled_width = template_width;
            int scaled_height = template_height;

            if (account.P2EP_PSX_Resolution == "320 × 240")
            {
                // Do nothing if setting is at default resolution
            }
            else
            {
                if (account.P2EP_PSX_Resolution == "1440 × 1080")
                {
                    scaled_width = 1440;
                    scaled_height = 1080;
                }

                var copied_input = new Bitmap(input_template);
                scaled_bitmap = new Bitmap(scaled_width, scaled_height);

                using (Graphics graphics = Graphics.FromImage(scaled_bitmap))
                {
                    switch (account.P2EP_PSX_Scale)
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

        public EmbedBuilder P2EP_PS1_Loading_Message()
        {
            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = $"Generating Scene...",
                IconUrl = EmbedSettings.Get_Game_Logo("P2EP-PS1")
            };

            embed.WithAuthor(author);
            embed.WithColor(EmbedSettings.Get_Game_Color("P2EP-PS1", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Loading_Icon("P2EP-PS1", null));
            embed.WithDescription("This may take a few seconds!");

            return embed;
        }
    }
}
