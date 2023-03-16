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

namespace SocialLinker.Core.SceneMaker.TemplateRenders.QuickScenes
{
    public class RenderP1_PS1 : ModuleBase<SocketCommandContext>
    {
        int template_width = 320;
        int template_height = 240;

        public async Task Render_Quick_Scene_P1_PS1(SocialLinkerCommand sl_command, OfficialSetData set_data, MakerCommandData command_data)
        {
            try
            {
                SocketUser user = sl_command.User;
                SocketTextChannel channel = (SocketTextChannel)sl_command.Channel;

                var account = UserInfoClasses.GetAccount(user);

                BustupData bustup_data = BustupDataMethods.Get_Bustup_Data(account, set_data, command_data);

                // The P1-PS1 template has a unique function where display names are not rendered if the same character is used in succession.
                // We'll call this "context switch". Get or create an active context switch object that stores data for this.
                ContextSwitchData active_session = ContextSwitchMethods.Get_Active_Session((SocketGuildUser)user, set_data);

                string display_name = "";

                if (command_data.Base_Sprite == 0)
                {
                    display_name = OfficialSetMethods.GetDisplayName(account, command_data, set_data, bustup_data);
                    command_data.Base_Sprite = 0;
                }
                else
                {
                    display_name = OfficialSetMethods.GetDisplayName(account, command_data, set_data, bustup_data);
                }

                // Check if the list of active characters contains the current data set.
                if (active_session.Active_Characters.Contains(set_data))
                {
                    // If so, check if the most recently used index of the list is not the same index the matching set data is in.
                    if (active_session.Recently_Used_Index != active_session.Active_Characters.IndexOf(set_data))
                    {
                        // Append the character's display name to their dialogue.
                        command_data.Dialogue = $"{display_name}:  {command_data.Dialogue}";
                    }
                    // Append the character's display name if they have Consistent Display Names set to "On".
                    else if (account.P1_PSX_TS_Consistent_Names == "On")
                    {
                        command_data.Dialogue = $"{display_name}:  {command_data.Dialogue}";
                    }
                }
                // If not, we'll want to add the set to the list.
                else
                {
                    // Append the character's display name to their dialogue.
                    command_data.Dialogue = $"{display_name}:  {command_data.Dialogue}";

                    // Check if the number of active characters in the list is three, which is the max number allowed.
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

                List<string>[] dialogue_lines = OfficialSetMethods.Line_Parser(sl_command, "P1-PS1", command_data.Dialogue, 4, 240);

                // The string array list typically has a constant number of indicies when created, so get the number of lines that'll actually be rendered. 
                int number_of_rendered_lines = Get_Number_of_Rendered_Lines(dialogue_lines);

                // Check if the number of rendered lines is greater than three.
                // If so, we'll have to send two images to simulate text scrolling for this template.
                if (number_of_rendered_lines > 3)
                {
                    // Isolate the first three lines of dialogue into a new string array list.
                    List<string>[] dialogue_lines_pt_1 = new List<string>[] { dialogue_lines[0], dialogue_lines[1], dialogue_lines[2] };

                    // Send a loading message for the first image.
                    RestUserMessage loader = await channel.SendMessageAsync("", false, P1_PS1_Multi_Scene_Loading_Message(1, 2).Build());

                    // Render the first image.
                    await Render_Offload(sl_command, account, active_session, set_data, bustup_data, command_data, dialogue_lines_pt_1, loader);

                    // Move down one line and isolate another three lines of dialogue into a new string array list. This will imitate the text scrolling.
                    List<string>[] dialogue_lines_pt_2 = new List<string>[] { dialogue_lines[1], dialogue_lines[2], dialogue_lines[3] };

                    // Send a loading message for the second image.
                    loader = await channel.SendMessageAsync("", false, P1_PS1_Multi_Scene_Loading_Message(2, 2).Build());

                    // Render the second image.
                    await Render_Offload(sl_command, account, active_session, set_data, bustup_data, command_data, dialogue_lines_pt_2, loader);
                }
                // If the number of rendered lines is exactly three or less, we'll only need to send one image.
                else
                {
                    // Send a loading message.
                    RestUserMessage loader = await channel.SendMessageAsync("", false, P1_PS1_Loading_Message().Build());

                    // Render the image.
                    await Render_Offload(sl_command, account, active_session, set_data, bustup_data, command_data, dialogue_lines, loader);
                }

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
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public async Task Render_Offload(SocialLinkerCommand sl_command, UserInfoFields account, ContextSwitchData active_session, OfficialSetData set_data, BustupData bustup_data, MakerCommandData command_data, List<string>[] dialogue_lines, RestUserMessage loader)
        {
            // Background rendering
            Bitmap base_template = new Bitmap(template_width, template_height);
            Bitmap colored_background_bitmap = OfficialSetMethods.Render_Colored_Background(account, template_width, template_height);
            Bitmap background = new Bitmap(2, 2);
            Bitmap bg_shadow = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PS1//Main//shadow.png");

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
                graphics.DrawImage(colored_background_bitmap, 0, 0, template_width, template_height);
                graphics.DrawImage(background, 0, 0, template_width, template_height);

                if (account.P1_PSX_TS_BG_Darken == "On")
                {
                    graphics.DrawImage(bg_shadow, 0, 0, template_width, template_height);
                }

                if (command_data.Base_Sprite != 0)
                {
                    Bitmap placed_bustup = Set_Bustup_Placement(account, bustup, bustup_data, active_session, set_data);
                    graphics.DrawImage(placed_bustup, 0, 0, template_width, template_height);
                }
                
                graphics.DrawImage(Generate_Message_Window(account), 0, 0, template_width, template_height);

                if (account.P1_PSX_TS_Moon_HUD == "On")
                {
                    graphics.DrawImage(Generate_Moon_HUD(account), 0, 0, template_width, template_height);
                }

                Bitmap rendered_dialogue = Render_Dialogue(dialogue_lines);
                graphics.DrawImage(rendered_dialogue, 0, 0, template_width, template_height);

                Random rnd = new Random();
                Bitmap cursor = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PS1//Main//Cursor//{rnd.Next(1, 16)}.png");
                graphics.DrawImage(cursor, 0, 0, template_width, template_height);
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

        public async Task Render_System_Message(SocialLinkerCommand sl_command,  MakerCommandData command_data)
        {
            SocketUser user = sl_command.User;
            SocketTextChannel channel = (SocketTextChannel)sl_command.Channel;

            var account = UserInfoClasses.GetAccount(user);

            List<string>[] dialogue_lines = OfficialSetMethods.Line_Parser(sl_command, "P1-PS1", command_data.Dialogue, 4, 240);

            // The string array list typically has a constant number of indicies when created, so get the number of lines that'll actually be rendered. 
            int number_of_rendered_lines = Get_Number_of_Rendered_Lines(dialogue_lines);

            // Check if the number of rendered lines is greater than three.
            // If so, we'll have to send two images to simulate text scrolling for this template.
            if (number_of_rendered_lines > 3)
            {
                // Isolate the first three lines of dialogue into a new string array list.
                List<string>[] dialogue_lines_pt_1 = new List<string>[] { dialogue_lines[0], dialogue_lines[1], dialogue_lines[2] };

                // Send a loading message for the first image.
                RestUserMessage loader = await channel.SendMessageAsync("", false, P1_PS1_Multi_Scene_Loading_Message(1, 2).Build());

                // Render the first image.
                await Render_System_Message_Offload(sl_command, account, command_data, dialogue_lines_pt_1, loader);

                // Move down one line and isolate another three lines of dialogue into a new string array list. This will imitate the text scrolling.
                List<string>[] dialogue_lines_pt_2 = new List<string>[] { dialogue_lines[1], dialogue_lines[2], dialogue_lines[3] };

                // Send a loading message for the second image.
                loader = await channel.SendMessageAsync("", false, P1_PS1_Multi_Scene_Loading_Message(2, 2).Build());

                // Render the second image.
                await Render_System_Message_Offload(sl_command, account, command_data, dialogue_lines_pt_2, loader);
            }
            // If the number of rendered lines is exactly three or less, we'll only need to send one image.
            else
            {
                // Send a loading message.
                RestUserMessage loader = await channel.SendMessageAsync("", false, P1_PS1_Loading_Message().Build());

                // Render the image.
                await Render_System_Message_Offload(sl_command, account, command_data, dialogue_lines, loader);
            }

        }

        public async Task Render_System_Message_Offload(SocialLinkerCommand sl_command, UserInfoFields account, MakerCommandData command_data, List<string>[] dialogue_lines, RestUserMessage loader)
        {
            // Background rendering
            Bitmap base_template = new Bitmap(template_width, template_height);
            Bitmap colored_background_bitmap = OfficialSetMethods.Render_Colored_Background(account, template_width, template_height);
            Bitmap background = new Bitmap(2, 2);
            Bitmap bg_shadow = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PS1//Main//shadow.png");

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
                graphics.DrawImage(colored_background_bitmap, 0, 0, template_width, template_height);
                graphics.DrawImage(background, 0, 0, template_width, template_height);

                if (account.P1_PSX_TS_BG_Darken == "On")
                {
                    graphics.DrawImage(bg_shadow, 0, 0, template_width, template_height);
                }

                graphics.DrawImage(Generate_Message_Window(account), 0, 0, template_width, template_height);

                if (account.P1_PSX_TS_Moon_HUD == "On")
                {
                    graphics.DrawImage(Generate_Moon_HUD(account), 0, 0, template_width, template_height);
                }

                Bitmap rendered_dialogue = Render_Dialogue(dialogue_lines);
                graphics.DrawImage(rendered_dialogue, 0, 0, template_width, template_height);

                Random rnd = new Random();
                Bitmap cursor = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PS1//Main//Cursor//{rnd.Next(1, 16)}.png");
                graphics.DrawImage(cursor, 0, 0, template_width, template_height);
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

        public Bitmap Set_Bustup_Placement(UserInfoFields account, Bitmap bustup, BustupData bustup_data, ContextSwitchData active_session, OfficialSetData set_data)
        {
            // Create a starting base bitmap to render all graphics on.
            Bitmap base_template = new Bitmap(template_width, template_height);

            // Switch the rendering position of the bustup depending on the user's settings.
            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                switch (account.P1_PSX_TS_Position)
                {
                    case "Left":
                        graphics.DrawImage(bustup, bustup_data.P1_PSX_Left_Coord_X, bustup_data.P1_PSX_Left_Coord_Y, bustup_data.P1_PSX_Scale_Width, bustup_data.P1_PSX_Scale_Height);
                        break;

                    case "Right":
                        graphics.DrawImage(bustup, bustup_data.P1_PSX_Right_Coord_X, bustup_data.P1_PSX_Right_Coord_Y, bustup_data.P1_PSX_Scale_Width, bustup_data.P1_PSX_Scale_Height);
                        break;

                    case "Center":
                        graphics.DrawImage(bustup, bustup_data.P1_PSX_Center_Coord_X, bustup_data.P1_PSX_Center_Coord_Y, bustup_data.P1_PSX_Scale_Width, bustup_data.P1_PSX_Scale_Height);
                        break;

                    case "Switch":
                        switch (active_session.Active_Characters.IndexOf(set_data))
                        {
                            case 0:
                                graphics.DrawImage(bustup, bustup_data.P1_PSX_Left_Coord_X, bustup_data.P1_PSX_Left_Coord_Y, bustup_data.P1_PSX_Scale_Width, bustup_data.P1_PSX_Scale_Height);
                                break;

                            case 1:
                                graphics.DrawImage(bustup, bustup_data.P1_PSX_Right_Coord_X, bustup_data.P1_PSX_Right_Coord_Y, bustup_data.P1_PSX_Scale_Width, bustup_data.P1_PSX_Scale_Height);
                                break;

                            case 2:
                                graphics.DrawImage(bustup, bustup_data.P1_PSX_Center_Coord_X, bustup_data.P1_PSX_Center_Coord_Y, bustup_data.P1_PSX_Scale_Width, bustup_data.P1_PSX_Scale_Height);
                                break;
                        }
                        break;
                }
            }

            return base_template;
        }

        public Bitmap Generate_Message_Window(UserInfoFields account)
        {
            Bitmap base_template = new Bitmap(template_width, template_height);
            string wallpaper_type = "";

            switch (account.P1_PSX_TS_Wallpaper)
            {
                case "Type 1":
                    wallpaper_type = "type_1";
                    break;

                case "Type 2":
                    wallpaper_type = "type_2";
                    break;

                case "Type 3":
                    wallpaper_type = "type_3";
                    break;

                case "Type 4":
                    wallpaper_type = "type_4";
                    break;

                case "Type 5":
                    wallpaper_type = "type_5";
                    break;

                case "Type 6":
                    wallpaper_type = "type_6";
                    break;

                case "Type 7":
                    wallpaper_type = "type_7";
                    break;

                case "Type 8":
                    wallpaper_type = "type_8";
                    break;
            }

            Bitmap wallpaper = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PS1//Main//Wallpaper//{wallpaper_type}.png");
            Bitmap message_window = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PS1//Main//message_window.png");

            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                wallpaper = SetImageOpacity(wallpaper, (float)0.5);

                graphics.DrawImage(wallpaper, 0, 0, template_width, template_height);
                graphics.DrawImage(message_window, 0, 0, template_width, template_height);
            }

            return base_template;
        }

        // Text rendering tools
        public Bitmap Render_Dialogue(List<string>[] dialogue_lines)
        {
            // Create a starting base bitmap to render all graphics on.
            Bitmap bitmap = new Bitmap(template_width, template_height);

            //Establish an int for the width and height glyphs should be rendered at
            int multiplier = 16;

            string font_sheet = $@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PS1//Font//p1-ps1_font_sheet.png";
            Bitmap current_glyph;

            // Iterate over each line of the dialogue string list with a loop.
            for (int i = 0; i < dialogue_lines.Length; i++)
            {
                // Specify X and Y coordinates for where the glyphs should start rendering on the template.
                int render_position_x = 40;
                int render_position_y = 164 + (16 * i);

                // Take the input dialogue and convert it into a char array.
                char[] char_array = String_List_To_String(dialogue_lines[i]).ToCharArray();

                // Iterate through each character of the array.
                for (int j = 0; j < char_array.Length; j++)
                {
                    // Retrieve glyph information from the appropriate JSON file.
                    var glyph = ParsingMethods.Get_P1_PS1_Glyph(char_array[j]);

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

                        // Set the next X value at the end of the current glyph's right width.
                        render_position_x += (glyph.RightCut - glyph.LeftCut);
                    }
                }
            }

            return bitmap;
        }

        public static int Measure_String_Pixel_Length(SocialLinkerCommand sl_command, string input_word)
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
                var glyph = ParsingMethods.Get_P1_PS1_Glyph(char_array[i]);

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

        // Getter methods
        public Bitmap Generate_Moon_HUD(UserInfoFields account)
        {
            // Create a new bitmap with the width and height variables.
            Bitmap base_template = new Bitmap(template_width, template_height);

            // Create new bitmap variables for the assets we'll need throughout the method.
            // We'll assign them proper values soon depending on the moon phase.
            Bitmap moon_window = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PS1//Main//Moon//moon_window.png");
            Bitmap base_text = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PS1//Main//Moon//Phases//Text//base_text.png");
            Bitmap phase_text = new Bitmap(template_width, template_height);
            Bitmap phase_texture = new Bitmap(template_width, template_height);
            Bitmap phase_covering = new Bitmap(template_width, template_height);
            Bitmap slash = new Bitmap(template_width, template_height);
            Bitmap denominator = new Bitmap(template_width, template_height);

            // Create a random variable.
            Random rnd = new Random();

            phase_texture = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PS1//Main//Moon//Phases//Images//Textures//{rnd.Next(1, 17)}.png");

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

            // Here is where the calculation on which moon phase to display begins.
            // The cycle begins with a new moon, so we'll use the current cycle's age and divide it into two halfs to determine whether it's waxing or waning.
            // Waxing phases
            if (cycle_age <= 14.76)
            {
                // New moon
                if ((illumination >= 0) && (illumination < 12.5))
                {
                    phase_text = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PS1//Main//Moon//Phases//Text//new.png");
                    phase_covering = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PS1//Main//Moon//Phases//Images//Coverings//1_new.png");
                }
                // Waxing crescent 1
                else if ((illumination >= 12.5) && (illumination < 25))
                {
                    phase_text = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PS1//Main//Moon//Phases//Text//1.png");
                    slash = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PS1//Main//Moon//Phases//Text//slash.png");
                    denominator = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PS1//Main//Moon//Phases//Text//denominator.png");
                    phase_covering = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PS1//Main//Moon//Phases//Images//Coverings//2_waxing_crescent.png");
                }
                // Waxing crescent 2
                else if ((illumination >= 25) && (illumination < 37.5))
                {
                    phase_text = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PS1//Main//Moon//Phases//Text//2.png");
                    slash = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PS1//Main//Moon//Phases//Text//slash.png");
                    denominator = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PS1//Main//Moon//Phases//Text//denominator.png");
                    phase_covering = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PS1//Main//Moon//Phases//Images//Coverings//3_waxing_crescent.png");
                }
                // Waxing crescent 3
                else if ((illumination >= 37.5) && (illumination < 50))
                {
                    phase_text = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PS1//Main//Moon//Phases//Text//3.png");
                    slash = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PS1//Main//Moon//Phases//Text//slash.png");
                    denominator = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PS1//Main//Moon//Phases//Text//denominator.png");
                    phase_covering = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PS1//Main//Moon//Phases//Images//Coverings//4_waxing_crescent.png");
                }
                // Waxing half
                else if ((illumination >= 50) && (illumination < 62.5))
                {
                    phase_text = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PS1//Main//Moon//Phases//Text//half.png");
                    phase_covering = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PS1//Main//Moon//Phases//Images//Coverings//5_waxing_half.png");
                }
                // Waxing gibbous 1
                else if ((illumination >= 62.5) && (illumination < 75))
                {
                    phase_text = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PS1//Main//Moon//Phases//Text//5.png");
                    slash = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PS1//Main//Moon//Phases//Text//slash.png");
                    denominator = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PS1//Main//Moon//Phases//Text//denominator.png");
                    phase_covering = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PS1//Main//Moon//Phases//Images//Coverings//6_waxing_gibbous.png");
                }
                // Waxing gibbous 2
                else if ((illumination >= 75) && (illumination < 87.5))
                {
                    phase_text = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PS1//Main//Moon//Phases//Text//6.png");
                    slash = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PS1//Main//Moon//Phases//Text//slash.png");
                    denominator = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PS1//Main//Moon//Phases//Text//denominator.png");
                    phase_covering = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PS1//Main//Moon//Phases//Images//Coverings//7_waxing_gibbous.png");
                }
                // Waxing gibbous 3
                else if ((illumination >= 87.5) && (illumination < 100))
                {
                    phase_text = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PS1//Main//Moon//Phases//Text//7.png");
                    slash = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PS1//Main//Moon//Phases//Text//slash.png");
                    denominator = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PS1//Main//Moon//Phases//Text//denominator.png");
                    phase_covering = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PS1//Main//Moon//Phases//Images//Coverings//8_waxing_gibbous.png");
                }
                // Full moon
                else if (illumination >= 100)
                {
                    phase_text = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PS1//Main//Moon//Phases//Text//full.png");
                    phase_covering = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PS1//Main//Moon//Phases//Images//Coverings//9_full.png");
                }
            }
            // Waning phases
            else if (cycle_age > 14.76)
            {
                // Full moon
                if (illumination >= 100)
                {
                    phase_text = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PS1//Main//Moon//Phases//Text//full.png");
                    phase_covering = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PS1//Main//Moon//Phases//Images//Coverings//9_full.png");
                }
                // Waning gibbous 1
                else if ((illumination >= 87.5) && (illumination < 100))
                {
                    phase_text = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PS1//Main//Moon//Phases//Text//7.png");
                    slash = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PS1//Main//Moon//Phases//Text//slash.png");
                    denominator = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PS1//Main//Moon//Phases//Text//denominator.png");
                    phase_covering = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PS1//Main//Moon//Phases//Images//Coverings//10_waning_gibbous.png");
                }
                // Waning gibbous 2
                else if ((illumination >= 75) && (illumination < 87.5))
                {
                    phase_text = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PS1//Main//Moon//Phases//Text//6.png");
                    slash = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PS1//Main//Moon//Phases//Text//slash.png");
                    denominator = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PS1//Main//Moon//Phases//Text//denominator.png");
                    phase_covering = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PS1//Main//Moon//Phases//Images//Coverings//11_waning_gibbous.png");
                }
                // Waning gibbous 3
                else if ((illumination >= 62.5) && (illumination < 75))
                {
                    phase_text = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PS1//Main//Moon//Phases//Text//5.png");
                    slash = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PS1//Main//Moon//Phases//Text//slash.png");
                    denominator = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PS1//Main//Moon//Phases//Text//denominator.png");
                    phase_covering = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PS1//Main//Moon//Phases//Images//Coverings//12_waning_gibbous.png");
                }
                // Waning half
                else if ((illumination >= 50) && (illumination < 62.5))
                {
                    phase_text = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PS1//Main//Moon//Phases//Text//half.png");
                    phase_covering = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PS1//Main//Moon//Phases//Images//Coverings//13_waning_half.png");
                }
                // Waning crescent 1
                else if ((illumination >= 37.5) && (illumination < 50))
                {
                    phase_text = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PS1//Main//Moon//Phases//Text//3.png");
                    slash = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PS1//Main//Moon//Phases//Text//slash.png");
                    denominator = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PS1//Main//Moon//Phases//Text//denominator.png");
                    phase_covering = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PS1//Main//Moon//Phases//Images//Coverings//14_waning_crescent.png");
                }
                // Waning crescent 2
                else if ((illumination >= 25) && (illumination < 37.5))
                {
                    phase_text = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PS1//Main//Moon//Phases//Text//2.png");
                    slash = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PS1//Main//Moon//Phases//Text//slash.png");
                    denominator = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PS1//Main//Moon//Phases//Text//denominator.png");
                    phase_covering = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PS1//Main//Moon//Phases//Images//Coverings//14_waning_crescent.png");
                }
                // Waning crescent 3
                else if ((illumination >= 12.5) && (illumination < 25))
                {
                    phase_text = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PS1//Main//Moon//Phases//Text//1.png");
                    slash = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PS1//Main//Moon//Phases//Text//slash.png");
                    denominator = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PS1//Main//Moon//Phases//Text//denominator.png");
                    phase_covering = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PS1//Main//Moon//Phases//Images//Coverings//15_waning_crescent.png");
                }
                // New moon
                else if ((illumination >= 0) && (illumination < 12.5))
                {
                    phase_text = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PS1//Main//Moon//Phases//Text//new.png");
                    phase_covering = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PS1//Main//Moon//Phases//Images//Coverings//1_new.png");
                }
            }

            // Now, let's use a graphics object to draw to the base template and render them all!
            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                Bitmap moon_phase = Create_Moon_Shading(phase_texture, phase_covering);

                graphics.DrawImage(moon_phase, 227, 14, moon_phase.Width, moon_phase.Height);
                graphics.DrawImage(moon_window, 0, 0, template_width, template_height);
                graphics.DrawImage(phase_text, 0, 0, template_width, template_height);
                graphics.DrawImage(slash, 0, 0, template_width, template_height);
                graphics.DrawImage(denominator, 0, 0, template_width, template_height);
                graphics.DrawImage(base_text, 0, 0, template_width, template_height);
            }

            return base_template;
        }

        public static int Get_Number_of_Rendered_Lines(List<string>[] input_list_array)
        {
            // Initialize an int variable to hold the number of rendered lines.
            int number_of_lines = 0;

            // Take each index of the string list array, convert the list to a string, then analyze the string to determine if it's empty or not.
            // If it IS empty, that line won't be rendered.
            // Count the number of lines that will actually be rendered to the screen.
            if (String_List_To_String(input_list_array[3]) != "")
            {
                number_of_lines = 4;
            }
            else if (String_List_To_String(input_list_array[2]) != "")
            {
                number_of_lines = 3;
            }
            else if (String_List_To_String(input_list_array[1]) != "")
            {
                number_of_lines = 2;
            }
            else
            {
                number_of_lines = 1;
            }

            return number_of_lines;
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
                    json_result = client.DownloadString($"http://api.weatherapi.com/v1/current.json?key={WeatherAPIConfig.weather_api_account.accountKey}&q={account.City}");
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

        // Method from https://www.codeproject.com/Tips/201129/Change-Opacity-of-Image-in-C
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

        public Bitmap Scale_Template(UserInfoFields account, Bitmap input_template)
        {
            var scaled_bitmap = new Bitmap(2, 2);
            int scaled_width = template_width;
            int scaled_height = template_height;

            if (account.P1_PSX_Resolution == "320 × 240")
            {
                // Do nothing if setting is at default resolution
            }
            else
            {
                if (account.P1_PSX_Resolution == "1440 × 1080")
                {
                    scaled_width = 1440;
                    scaled_height = 1080;
                }

                var copied_input = new Bitmap(input_template);
                scaled_bitmap = new Bitmap(scaled_width, scaled_height);

                using (Graphics graphics = Graphics.FromImage(scaled_bitmap))
                {
                    switch (account.P1_PSX_Scale)
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

        public Bitmap Create_Moon_Shading(Bitmap moon_phase, Bitmap beige_cover)
        {
            Bitmap base_template = new Bitmap(moon_phase.Width, moon_phase.Height);
            System.Drawing.Color altered_pixel = default;

            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                for (int x = 0; x < moon_phase.Width; x++)
                {
                    for (int y = 0; y < moon_phase.Height; y++)
                    {
                        System.Drawing.Color beige_cover_color = beige_cover.GetPixel(x, y);
                        System.Drawing.Color moon_phase_color = moon_phase.GetPixel(x, y);

                        int r_value = moon_phase_color.R - beige_cover_color.R;
                        int g_value = moon_phase_color.G - beige_cover_color.G;
                        int b_value = moon_phase_color.B - beige_cover_color.B;

                        if (r_value < 0)
                        {
                            r_value = 0;
                        }
                        if (g_value < 0)
                        {
                            g_value = 0;
                        }
                        if (b_value < 0)
                        {
                            b_value = 0;
                        }

                        altered_pixel = System.Drawing.Color.FromArgb(moon_phase_color.A, r_value, g_value, b_value);
                        base_template.SetPixel(x, y, altered_pixel);
                    }
                }
            }

            return base_template;
        }

        // Loading messages
        public static EmbedBuilder P1_PS1_Loading_Message()
        {
            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = $"Generating Scene...",
                IconUrl = EmbedSettings.Get_Game_Logo("P1-PS1")
            };

            embed.WithAuthor(author);
            embed.WithColor(EmbedSettings.Get_Game_Color("P1-PS1", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Loading_Icon("P1-PS1", null));
            embed.WithDescription("This may take a few seconds!");

            return embed;
        }

        public static EmbedBuilder P1_PS1_Multi_Scene_Loading_Message(int passthrough, int number_of_scenes)
        {
            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = $"Generating Scene... (Part {passthrough} / {number_of_scenes})",
                IconUrl = EmbedSettings.Get_Game_Logo("P1-PS1")
            };

            embed.WithAuthor(author);
            embed.WithColor(EmbedSettings.Get_Game_Color("P1-PS1", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Loading_Icon("P1-PS1", null));
            embed.WithDescription("This may take a few seconds!");

            return embed;
        }
    }

    public class ContextSwitchMethods
    {
        public static ContextSwitchData Get_Active_Session(SocketGuildUser user, OfficialSetData set_data)
        {
            // Find the session associated with both the current user and command type.
            var active_session = Global.P1_PS1_Usage_List.SingleOrDefault(x => (x.User.Id == user.Id));

            // If the session doesn't exist, create one and set it to the session variable.
            if (active_session == null)
            {
                Create_Active_Session(user, set_data);
                active_session = Global.P1_PS1_Usage_List.SingleOrDefault(x => (x.User.Id == user.Id));
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
            var active_session = new ContextSwitchData()
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

            Global.P1_PS1_Usage_List.Add(active_session);

            // If the timer runs out, activate a function.
            active_session.Active_Timer.Elapsed += (sender, e) => Timer_Elapsed(sender, e, user);
        }

        private static void Timer_Elapsed(object sender, ElapsedEventArgs e, SocketGuildUser user)
        {
            // Find the usage session associated with the current user.
            var active_session = Global.P1_PS1_Usage_List.SingleOrDefault(x => (x.User.Id == user.Id));

            // Remove the usage session from the global list.
            Global.P1_PS1_Usage_List.Remove(active_session);
        }
    }

    public class ContextSwitchData
    {
        public SocketGuildUser User { get; set; }
        public List<OfficialSetData> Active_Characters { get; set; }
        public int Recently_Used_Index { get; set; }
        public List<int> Recently_Used_Character_List { get; set; }
        public Timer Active_Timer { get; set; }
    }
}
