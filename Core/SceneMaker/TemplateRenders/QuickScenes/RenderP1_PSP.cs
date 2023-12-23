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
using System.Drawing.Drawing2D;
using SocialLinker.Core.Menus;
using System.Timers;

namespace SocialLinker.Core.SceneMaker.TemplateRenders.QuickScenes
{
    public class RenderP1_PSP
    {
        int template_width = 480;
        int template_height = 272;
        int max_line_length = 320;

        public async Task Render_Quick_Scene_P1_PSP(SocialLinkerCommand sl_command)
        {
            SocketUser user = sl_command.User;

            OfficialSetData set_data = sl_command.MakerCommand.Character_Data_1.Set_Data;
            MakerCommandData maker_command_data = sl_command.MakerCommand;

            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            // Get the data for the chosen bustup.
            sl_command.MakerCommand.Character_Data_1.Bustup_Data = BustupDataMethods.Get_Bustup_Data(account, set_data, sl_command.MakerCommand.Character_Data_1);
            BustupData bustup_data = sl_command.MakerCommand.Character_Data_1.Bustup_Data;

            // The P1-PS1 template has a unique function where display names are not rendered if the same character is used in succession.
            // We'll call this "context switch". Get or create an active context switch object that stores data for this.
            PlacementSwitchData active_session = PlacementSwitchMethods.Get_Active_Session((SocketGuildUser)user, set_data);

            // Check if the list of active characters contains the current data set. If not, we'll want to add the set to the list.
            if (!active_session.Active_Characters.Contains(set_data))
            {
                // Check if the number of active characters in the list is two, which is the max number allowed.
                if (active_session.Active_Characters.Count == 3)
                {
                    // If so, replace the set in the first index with the current set.
                    active_session.Active_Characters[active_session.Recently_Used_Character_List[0]] = set_data;
                }
                // If the number is less than three, add the set data to the list.
                else
                {
                    active_session.Active_Characters.Add(set_data);
                }
            }

            // Create a new int variable that stores the INDEX of the current set data in the session list.
            int char_index = active_session.Active_Characters.IndexOf(set_data);

            // Render the image.
            await Render_Offload(sl_command, account, active_session, set_data, bustup_data, maker_command_data);

            // Here, we're at the step where we've already rendered and sent the images we needed.
            // For the Context Switch feature, we'll want to keep track of the past three unique characters used.
            // Check if the current Active_Characters index is in the Recently_Used_Character_List.
            // Remember that the Active_Characters list is always kept at a max of three indicies, so you don't have to worry about overflow here.
            if (active_session.Recently_Used_Character_List.Contains(char_index))
            {
                // If the set's index is found, remove it from its current position in the list and add it back to the end.
                active_session.Recently_Used_Character_List.Remove(char_index);
                active_session.Recently_Used_Character_List.Add(char_index);
            }
            // If the index isn't found in the list (meaning less than three characters have been used in this session), add it to the list.
            else
            {
                active_session.Recently_Used_Character_List.Add(char_index);
            }

            // Make the Recently_Used_Index the same as the current char_index so we can compare it the next time this template is used.
            active_session.Recently_Used_Index = char_index;
        }

        public async Task Render_Offload(SocialLinkerCommand sl_command, UserInfoFields account, PlacementSwitchData active_session, OfficialSetData set_data, BustupData bustup_data, MakerCommandData maker_command_data)
        {
            SocketTextChannel channel = (SocketTextChannel)sl_command.Channel;
            RestUserMessage loader = await channel.SendMessageAsync("", false, P1_PSP_Loading_Message().Build());

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

            if (maker_command_data.Character_Data_1.Base_Sprite != 0)
            {
                bustup = OfficialSetMethods.Bustup_Selection(sl_command, account, sl_command.MakerCommand.Character_Data_1);
            }

            if (bustup == null)
            {
                await loader.DeleteAsync();
                return;
            }

            // Time to put it all together!
            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                Random rnd = new Random();
                Bitmap message_window = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PSP//Main//message_window.png");
                Bitmap cursor = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PSP//Main//cursor.png");
                Bitmap bg_shadow = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PSP//Main//shadow.png");

                graphics.DrawImage(colored_background_bitmap, 0, 0, template_width, template_height);
                graphics.DrawImage(background, 0, 0, template_width, template_height);

                if (account.P1_PSP_TS_Moon_HUD == "On")
                {
                    Bitmap moon_hud = Render_Moon_HUD(account);
                    graphics.DrawImage(moon_hud, 0, 0, template_width, template_height);
                }

                if (account.P1_PSP_TS_BG_Darken == "On")
                {
                    graphics.DrawImage(bg_shadow, 0, 0, template_width, template_height);
                }

                graphics.DrawImage(message_window, 0, 0, template_width, template_height);

                if (maker_command_data.Character_Data_1.Base_Sprite != 0)
                {
                    Bitmap placed_bustup = Set_Bustup_Placement(account, bustup, bustup_data, active_session, set_data);
                    graphics.DrawImage(placed_bustup, 0, 0, template_width, template_height);
                }

                int cursor_y_position = rnd.Next(0, 8);
                graphics.DrawImage(cursor, 0, cursor_y_position, template_width, template_height);
            }

            string display_name = OfficialSetMethods.GetDisplayName(account, sl_command.MakerCommand.Character_Data_1);
            display_name = OfficialSetMethods.Validate_Input(sl_command, "P1-PSP", "Name", display_name);
            Bitmap display_name_layer = Render_Name(display_name);
            
            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                maker_command_data.Dialogue = OfficialSetMethods.Validate_Input(sl_command, "P1-PSP", "Dialogue", maker_command_data.Dialogue);
                List<string>[] parsed_lines = OfficialSetMethods.Line_Parser(sl_command, "P1-PSP", maker_command_data.Dialogue, 2, max_line_length);
                graphics.DrawImage(display_name_layer, 0, 0, template_width, template_height);
                graphics.DrawImage(Render_Dialogue(parsed_lines, false), 0, 0, template_width, template_height);
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
            SocketUser user = sl_command.User;
            SocketTextChannel channel = (SocketTextChannel)sl_command.Channel;
            RestUserMessage loader = await channel.SendMessageAsync("", false, P1_PSP_Loading_Message().Build());
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
                Random rnd = new Random();
                command_data.Dialogue = OfficialSetMethods.Validate_Input(sl_command, "P1-PSP", "Dialogue", command_data.Dialogue);
                List<string>[] parsed_lines = OfficialSetMethods.Line_Parser(sl_command, "P1-PSP", command_data.Dialogue, 3, max_line_length);
                Bitmap message_window = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PSP//Main//message_window.png");
                Bitmap cursor = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PSP//Main//cursor.png");
                Bitmap bg_shadow = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PSP//Main//shadow.png");

                graphics.DrawImage(colored_background_bitmap, 0, 0, template_width, template_height);
                graphics.DrawImage(background, 0, 0, template_width, template_height);

                if (account.P1_PSP_TS_Moon_HUD == "On")
                {
                    Bitmap moon_hud = Render_Moon_HUD(account);
                    graphics.DrawImage(moon_hud, 0, 0, template_width, template_height);
                }

                if (account.P1_PSP_TS_BG_Darken == "On")
                {
                    graphics.DrawImage(bg_shadow, 0, 0, template_width, template_height);
                }

                graphics.DrawImage(message_window, 0, 0, template_width, template_height);

                int cursor_y_position = rnd.Next(0, 8);
                graphics.DrawImage(cursor, 0, cursor_y_position, template_width, template_height);

                
                graphics.DrawImage(Render_Dialogue(parsed_lines, true), 0, 0, template_width, template_height);
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

        public Bitmap Set_Bustup_Placement(UserInfoFields account, Bitmap bustup, BustupData bustup_data, PlacementSwitchData active_session, OfficialSetData set_data)
        {
            // Create a starting base bitmap to render all graphics on.
            Bitmap base_template = new Bitmap(template_width, template_height);

            // Switch the rendering position of the bustup depending on the user's settings.
            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                switch (account.P1_PSP_TS_Position)
                {
                    case "Left":
                        graphics.DrawImage(bustup, bustup_data.P1_PSP_Left_Coord_X, bustup_data.P1_PSP_Left_Coord_Y, bustup_data.P1_PSP_Scale_Width, bustup_data.P1_PSP_Scale_Height);
                        break;

                    case "Right":
                        graphics.DrawImage(bustup, bustup_data.P1_PSP_Right_Coord_X, bustup_data.P1_PSP_Right_Coord_Y, bustup_data.P1_PSP_Scale_Width, bustup_data.P1_PSP_Scale_Height);
                        break;

                    case "Center":
                        graphics.DrawImage(bustup, bustup_data.P1_PSP_Center_Coord_X, bustup_data.P1_PSP_Center_Coord_Y, bustup_data.P1_PSP_Scale_Width, bustup_data.P1_PSP_Scale_Height);
                        break;

                    case "Switch":
                        switch (active_session.Active_Characters.IndexOf(set_data))
                        {
                            case 0:
                                graphics.DrawImage(bustup, bustup_data.P1_PSP_Left_Coord_X, bustup_data.P1_PSP_Left_Coord_Y, bustup_data.P1_PSP_Scale_Width, bustup_data.P1_PSP_Scale_Height);
                                break;

                            case 1:
                                graphics.DrawImage(bustup, bustup_data.P1_PSP_Right_Coord_X, bustup_data.P1_PSP_Right_Coord_Y, bustup_data.P1_PSP_Scale_Width, bustup_data.P1_PSP_Scale_Height);
                                break;

                            case 2:
                                graphics.DrawImage(bustup, bustup_data.P1_PSP_Center_Coord_X, bustup_data.P1_PSP_Center_Coord_Y, bustup_data.P1_PSP_Scale_Width, bustup_data.P1_PSP_Scale_Height);
                                break;
                        }
                        break;
                }
            }

            return base_template;
        }

        public Bitmap Render_Name(string display_name)
        {
            Bitmap base_template = new Bitmap(template_width, template_height);
            System.Drawing.Color display_name_color = System.Drawing.Color.FromArgb(158, 239, 22);
            Rectangle display_name_area = new Rectangle(0, 196, 480, 72);

            // Establish an int for the width and height glyphs should be rendered at.
            int multiplier = 16;

            // Load the bitmap font.
            string font_sheet = $@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PSP//Font//p1-psp_font_sheet.png";

            // Create a variable to iterate through sections of the bitmap font.
            Bitmap current_glyph;

            // Specify X and Y coordinates for where the glyphs should start rendering on the template.
            int render_position_x = 46;
            int render_position_y = 206;

            // Thake the sprite's display name and convert it into a char array.
            char[] char_array = display_name.ToCharArray();

            // Iterate through each character of the array.
            for (int i = 0; i < char_array.Length; i++)
            {
                // Retrieve glyph information from the JSON file.
                var glyph = ParsingMethods.Get_P1_PSP_Glyph(char_array[i]);

                // Check if the character is a line break.
                if (char_array[i] == '\u000a')
                {
                    // Set the X coordinate to the start of the row.
                    render_position_x = 46;
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

            base_template = Bitmap_To_Color(base_template, display_name_color, display_name_area);

            return base_template;
        }

        public Bitmap Render_Dialogue(List<string>[] dialogue_lines, bool system_message_check)
        {
            Bitmap bitmap = new Bitmap(template_width, template_height);

            // Create an int to keep track of rendering errors. This is neccessary to inform the user of any potential issues.
            int error_counter = 0;

            //Establish an int for the width and height glyphs should be rendered at
            int multiplier = 16;

            string font_sheet = $@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PSP//Font//p1-psp_font_sheet.png";
            Bitmap current_glyph;

            // Iterate over each line of the dialogue string list with a loop.
            for (int i = 0; i < dialogue_lines.Length; i++)
            {
                // Specify X and Y coordinates for where the glyphs should start rendering on the template.
                int render_position_x = 0;
                int render_position_y = 0;

                switch (system_message_check)
                {
                    case true:
                        render_position_x = 46;
                        render_position_y = 206 + (18 * i);
                        break;

                    case false:
                        render_position_x = 54;
                        render_position_y = 224 + (18 * i);
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

        public Bitmap Render_Moon_HUD(UserInfoFields account)
        {
            Bitmap hud_bar = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PSP//Main//hud_bar.png");
            Bitmap moon_phase_glow = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PSP//Main//Moon//Phases//Glow//moon_glow.png");
            Bitmap moon_phase = new Bitmap(template_width, template_height);
            Bitmap counter_numerator = new Bitmap(template_width, template_height);
            Bitmap counter_slash = new Bitmap(template_width, template_height);
            Bitmap counter_denominator = new Bitmap(template_width, template_height);
            Bitmap moon_text = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PSP//Main//Moon//Counter//moon.png");
            
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

            // Store the moon's illumination percentage in a double. We'll use this to determine what phase it's currently in alongside using the age.
            double illumination = Math.Round(result.Visibility, 2);

            // Create a new bitmap with the width and height variables.
            Bitmap base_template = new Bitmap(template_width, template_height);

            // Now, let's use a graphics object to draw to the base template.
            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                // Here is where the calculation on which moon phase to display begins.
                // The cycle begins with a new moon, so we'll use the current cycle's age and divide it into two halfs to determine whether it's waxing or waning.
                // Waxing phases
                if (cycle_age <= 14.76)
                {
                    // New moon
                    if ((illumination >= 0) && (illumination < 12.5))
                    {
                        moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PSP//Main//Moon//Phases//1_new.png");
                        counter_numerator = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PSP//Main//Moon//Counter//Numerator//new.png");
                    }
                    // Waxing crescent 1
                    else if ((illumination >= 12.5) && (illumination < 25))
                    {
                        moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PSP//Main//Moon//Phases//2_waxing_crescent.png");
                        counter_numerator = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PSP//Main//Moon//Counter//Numerator//1.png");
                        counter_slash = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PSP//Main//Moon//Counter//Denominator//slash.png");
                        counter_denominator = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PSP//Main//Moon//Counter//Denominator//8.png");
                    }
                    // Waxing crescent 2
                    else if ((illumination >= 25) && (illumination < 37.5))
                    {
                        moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PSP//Main//Moon//Phases//3_waxing_crescent.png");
                        counter_numerator = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PSP//Main//Moon//Counter//Numerator//2.png");
                        counter_slash = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PSP//Main//Moon//Counter//Denominator//slash.png");
                        counter_denominator = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PSP//Main//Moon//Counter//Denominator//8.png");
                    }
                    // Waxing crescent 3
                    else if ((illumination >= 37.5) && (illumination < 50))
                    {
                        moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PSP//Main//Moon//Phases//4_waxing_crescent.png");
                        counter_numerator = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PSP//Main//Moon//Counter//Numerator//3.png");
                        counter_slash = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PSP//Main//Moon//Counter//Denominator//slash.png");
                        counter_denominator = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PSP//Main//Moon//Counter//Denominator//8.png");
                    }
                    // Waxing half
                    else if ((illumination >= 50) && (illumination < 62.5))
                    {
                        moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PSP//Main//Moon//Phases//5_waxing_half.png");
                        counter_numerator = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PSP//Main//Moon//Counter//Numerator//half.png");
                    }
                    // Waxing gibbous 1
                    else if ((illumination >= 62.5) && (illumination < 75))
                    {
                        moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PSP//Main//Moon//Phases//6_waxing_gibbous.png");
                        counter_numerator = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PSP//Main//Moon//Counter//Numerator//5.png");
                        counter_slash = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PSP//Main//Moon//Counter//Denominator//slash.png");
                        counter_denominator = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PSP//Main//Moon//Counter//Denominator//8.png");
                    }
                    // Waxing gibbous 2
                    else if ((illumination >= 75) && (illumination < 87.5))
                    {
                        moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PSP//Main//Moon//Phases//7_waxing_gibbous.png");
                        counter_numerator = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PSP//Main//Moon//Counter//Numerator//6.png");
                        counter_slash = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PSP//Main//Moon//Counter//Denominator//slash.png");
                        counter_denominator = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PSP//Main//Moon//Counter//Denominator//8.png");
                    }
                    // Waxing gibbous 3
                    else if ((illumination >= 87.5) && (illumination < 100))
                    {
                        moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PSP//Main//Moon//Phases//8_waxing_gibbous.png");
                        counter_numerator = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PSP//Main//Moon//Counter//Numerator//7.png");
                        counter_slash = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PSP//Main//Moon//Counter//Denominator//slash.png");
                        counter_denominator = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PSP//Main//Moon//Counter//Denominator//8.png");
                    }
                    // Full moon
                    else if (illumination == 100)
                    {
                        moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PSP//Main//Moon//Phases//9_full.png");
                        counter_numerator = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PSP//Main//Moon//Counter//Numerator//full.png");
                    }
                }
                // Waning phases
                else if (cycle_age > 14.76)
                {
                    // Full moon
                    if (illumination == 100)
                    {
                        moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PSP//Main//Moon//Phases//9_full.png");
                        counter_numerator = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PSP//Main//Moon//Counter//Numerator//full.png");
                    }
                    // Waning gibbous 1
                    else if ((illumination >= 87.5) && (illumination < 100))
                    {
                        moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PSP//Main//Moon//Phases//10_waning_gibbous.png");
                        counter_numerator = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PSP//Main//Moon//Counter//Numerator//7.png");
                        counter_slash = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PSP//Main//Moon//Counter//Denominator//slash.png");
                        counter_denominator = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PSP//Main//Moon//Counter//Denominator//8.png");
                    }
                    // Waning gibbous 2
                    else if ((illumination >= 75) && (illumination < 87.5))
                    {
                        moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PSP//Main//Moon//Phases//11_waning_gibbous.png");
                        counter_numerator = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PSP//Main//Moon//Counter//Numerator//6.png");
                        counter_slash = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PSP//Main//Moon//Counter//Denominator//slash.png");
                        counter_denominator = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PSP//Main//Moon//Counter//Denominator//8.png");
                    }
                    // Waning gibbous 3
                    else if ((illumination >= 62.5) && (illumination < 75))
                    {
                        moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PSP//Main//Moon//Phases//12_waning_gibbous.png");
                        counter_numerator = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PSP//Main//Moon//Counter//Numerator//5.png");
                        counter_slash = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PSP//Main//Moon//Counter//Denominator//slash.png");
                        counter_denominator = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PSP//Main//Moon//Counter//Denominator//8.png");
                    }
                    // Waning half
                    else if ((illumination >= 50) && (illumination < 62.5))
                    {
                        moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PSP//Main//Moon//Phases//13_waning_half.png");
                        counter_numerator = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PSP//Main//Moon//Counter//Numerator//half.png");
                    }
                    // Waning crescent 1
                    else if ((illumination >= 37.5) && (illumination < 50))
                    {
                        moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PSP//Main//Moon//Phases//14_waning_crescent.png");
                        counter_numerator = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PSP//Main//Moon//Counter//Numerator//3.png");
                        counter_slash = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PSP//Main//Moon//Counter//Denominator//slash.png");
                        counter_denominator = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PSP//Main//Moon//Counter//Denominator//8.png");
                    }
                    // Waning crescent 2
                    else if ((illumination >= 25) && (illumination < 37.5))
                    {
                        moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PSP//Main//Moon//Phases//15_waning_crescent.png");
                        counter_numerator = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PSP//Main//Moon//Counter//Numerator//2.png");
                        counter_slash = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PSP//Main//Moon//Counter//Denominator//slash.png");
                        counter_denominator = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PSP//Main//Moon//Counter//Denominator//8.png");
                    }
                    // Waning crescent 3
                    else if ((illumination >= 12.5) && (illumination < 25))
                    {
                        moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PSP//Main//Moon//Phases//16_waning_crescent.png");
                        counter_numerator = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PSP//Main//Moon//Counter//Numerator//1.png");
                        counter_slash = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PSP//Main//Moon//Counter//Denominator//slash.png");
                        counter_denominator = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PSP//Main//Moon//Counter//Denominator//8.png");
                    }
                    // New moon
                    else if ((illumination >= 0) && (illumination < 12.5))
                    {
                        moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PSP//Main//Moon//Phases//1_new.png");
                        counter_numerator = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PSP//Main//Moon//Counter//Numerator//new.png");
                    }
                }

                graphics.DrawImage(hud_bar, 0, 0, template_width, template_height);
                graphics.DrawImage(moon_phase_glow, 0, 0, template_width, template_height);
                graphics.DrawImage(moon_phase, 0, 0, template_width, template_height);
                graphics.DrawImage(counter_numerator, 0, 0, template_width, template_height);
                graphics.DrawImage(counter_slash, 0, 0, template_width, template_height);
                graphics.DrawImage(counter_denominator, 0, 0, template_width, template_height);
                graphics.DrawImage(moon_text, 0, 0, template_width, template_height);
            }

            return base_template;
        }

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
                var glyph = ParsingMethods.Get_P1_PSP_Glyph(char_array[i]);

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

        public static string Get_Hemisphere(UserInfoFields account)
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
                return "Northern";
            }
        }

        public Bitmap Scale_Template(UserInfoFields account, Bitmap input_template)
        {
            var scaled_bitmap = new Bitmap(2, 2);
            int scaled_width = template_width;
            int scaled_height = template_height;

            if (account.P1_PSP_Resolution == "480 × 272")
            {
                // Do nothing if setting is at default resolution
            }
            else
            {
                if (account.P1_PSP_Resolution == "1920 × 1088")
                {
                    scaled_width = 1920;
                    scaled_height = 1088;
                }

                var copied_input = new Bitmap(input_template);
                scaled_bitmap = new Bitmap(scaled_width, scaled_height);

                using (Graphics graphics = Graphics.FromImage(scaled_bitmap))
                {
                    switch (account.P1_PSP_Scale)
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

        public static EmbedBuilder P1_PSP_Loading_Message()
        {
            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = $"Generating Scene...",
                IconUrl = EmbedSettings.Get_Game_Logo("P1-PSP")
            };

            embed.WithAuthor(author);
            embed.WithColor(EmbedSettings.Get_Game_Color("P1-PSP", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Loading_Icon("P1-PSP", null));
            embed.WithDescription("This may take a few seconds!");

            return embed;
        }
    }

    public class PlacementSwitchMethods
    {
        public static PlacementSwitchData Get_Active_Session(SocketGuildUser user, OfficialSetData set_data)
        {
            // Find the session associated with both the current user and command type.
            var active_session = Global.P1_PSP_Usage_List.SingleOrDefault(x => (x.User.Id == user.Id));

            // If the session doesn't exist, create one and set it to the session variable.
            if (active_session == null)
            {
                Create_Active_Session(user, set_data);
                active_session = Global.P1_PSP_Usage_List.SingleOrDefault(x => (x.User.Id == user.Id));
            }

            // Reset the timer to expire in five minutes.
            active_session.Active_Timer = new Timer()
            {
                Interval = 300000,
                AutoReset = false,
                Enabled = true
            };

            return active_session;
        }

        public static void Create_Active_Session(SocketGuildUser user, OfficialSetData set_data)
        {
            // Create a new session for the command user.
            var active_session = new PlacementSwitchData()
            {
                User = user,
                Active_Characters = new List<OfficialSetData> { set_data },
                Recently_Used_Index = 100,
                Recently_Used_Character_List = new List<int>(),
                Active_Timer = new Timer()
                {
                    // Create a timer that expires as a "time out" duration for the user's session.
                    Interval = 300000,
                    AutoReset = false,
                    Enabled = true
                },
            };

            Global.P1_PSP_Usage_List.Add(active_session);

            // If the timer runs out, activate a function.
            active_session.Active_Timer.Elapsed += (sender, e) => Timer_Elapsed(sender, e, user);
        }

        private static void Timer_Elapsed(object sender, ElapsedEventArgs e, SocketGuildUser user)
        {
            // Find the usage session associated with the current user.
            var active_session = Global.P1_PSP_Usage_List.SingleOrDefault(x => (x.User.Id == user.Id));

            // Remove the usage session from the global list.
            Global.P1_PSP_Usage_List.Remove(active_session);
        }
    }

    public class PlacementSwitchData
    {
        public SocketGuildUser User { get; set; }
        public List<OfficialSetData> Active_Characters { get; set; }
        public int Recently_Used_Index { get; set; }
        public List<int> Recently_Used_Character_List { get; set; }
        public Timer Active_Timer { get; set; }
    }
}
