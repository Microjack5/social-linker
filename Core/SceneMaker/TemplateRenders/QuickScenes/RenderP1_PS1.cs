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
        public static async Task Render_Quick_Scene_P1_PS1(SocialLinkerCommand sl_command, OfficialSetData set_data, MakerCommandData command_data)
        {
            try
            {
                // Create two variables for the command user and the command channel, derived from the message object taken in.
                SocketUser user = sl_command.User;
                SocketTextChannel channel = (SocketTextChannel)sl_command.Channel;

                // Get the account information of the command's user.
                var account = UserInfoClasses.GetAccount(user);

                // Get the data for the chosen bustup.
                BustupData bustup_data = BustupDataMethods.Get_Bustup_Data(account, set_data, command_data);

                // The P1-PS1 template has a unique function where display names are not rendered if the same character is used in succession.
                // We'll call this "context switch". Get or create an active context switch object that stores data for this.
                ContextSwitchData active_session = ContextSwitchMethods.Get_Active_Session((SocketGuildUser)user, set_data);

                // Check if the list of active characters contains the current data set.
                if (active_session.Active_Characters.Contains(set_data))
                {
                    // If so, check if the most recently used index of the list is not the same index the matching set data is in.
                    if (active_session.Recently_Used_Index != active_session.Active_Characters.IndexOf(set_data))
                    {
                        // Append the character's display name to their dialogue.
                        command_data.Dialogue = $"{bustup_data.Default_Name_EN}: {command_data.Dialogue}";
                    }
                }
                // If not, we'll want to add the set to the list.
                else
                {
                    // Append the character's display name to their dialogue.
                    command_data.Dialogue = $"{bustup_data.Default_Name_EN}: {command_data.Dialogue}";

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

                // Parse the dialogue into lines that'll fit on the template and store it in a string array list.
                List<string>[] dialogue_lines = Line_Parser(sl_command, bustup_data, command_data.Dialogue);

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

        public static async Task Render_Offload(SocialLinkerCommand sl_command, UserInfoFields account, ContextSwitchData active_session, OfficialSetData set_data, BustupData bustup_data, MakerCommandData command_data, List<string>[] dialogue_lines, RestUserMessage loader)
        {
            // Create variables to store the width and height of the template.
            int template_width = 320;
            int template_height = 240;

            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = sl_command.User;
            SocketTextChannel channel = (SocketTextChannel)sl_command.Channel;

            // Create a starting base bitmap to render all graphics on.
            Bitmap base_template = new Bitmap(template_width, template_height);

            // Create another bitmap the same size.
            // In case the user has set a colored bitmap in their settings, we'll need to use this to render it.
            Bitmap colored_background_bitmap = new Bitmap(template_width, template_height);

            // Here, we want to grab any images attached to the message to use it as a background.
            // Create a variable for the message attachment.
            var attachments = sl_command.Attachments;

            // Create an empty string variable to hold the URL of the attachment.
            string url = "";

            // If there are no attachments on the message, set the URL string to "None".
            if (attachments == default || attachments.LongCount() == 0)
            {
                url = "None";
            }
            // Else, assign the URL of the attachment to the URL string.
            else
            {
                url = attachments.ElementAt(0).Url;
            }

            // Initialize a bitmap object for the user's background. It's small now because we'll reassign it depending on our circumstances.
            Bitmap background = new Bitmap(2, 2);

            // If a URL for a message attachment exists, download it and copy its contents to the bitmap variable we just created.
            if (url != "None")
            {
                // Here, we'll want to try and retrieve the user's input image.
                try
                {
                    // Declare variables for a web request to retrieve the image.
                    System.Net.HttpWebRequest webRequest = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(url);
                    webRequest.AllowWriteStreamBuffering = true;
                    webRequest.Timeout = 30000;

                    // Create a stream and download the image to it.
                    System.Net.WebResponse webResponse = webRequest.GetResponse();
                    System.IO.Stream stream = webResponse.GetResponseStream();

                    // Copy the stream's contents to the background bitmap variable.
                    background = (Bitmap)System.Drawing.Image.FromStream(stream);

                    webResponse.Close();
                }
                // If an exception occurs here, the filetype is likely incompatible.
                // Send an error message, delete the loading message, and return.
                catch (System.ArgumentException e)
                {
                    Console.WriteLine(e);
                    await loader.DeleteAsync();
                    _ = ErrorHandling.Incompatible_File_Type(sl_command);
                    return;
                }
            }

            // Render the uploaded image based on the user's background settings.
            switch (account.Setting_BG_Upload)
            {
                case "Maintain Aspect Ratio":
                    background = Center_Image(background);
                    break;

                case "Stretch to Fit":
                    background = Stretch_To_Fit(background);
                    break;
            }

            // The user may have a custom mono-colored background designated in their settings. Let's handle that now.
            // Check if the user's background color setting is set to something other than "Transparent".
            // If so, we have a color to render for the background!
            if (account.Setting_BG_Color != "Transparent")
            {
                // Convert the user's HTML color setting to one we can use and assign it to a color variable.
                System.Drawing.Color user_background_color = System.Drawing.ColorTranslator.FromHtml(account.Setting_BG_Color);

                // Color the entirety of the background bitmap the user's selected color.
                using (Graphics graphics = Graphics.FromImage(colored_background_bitmap))
                {
                    graphics.Clear(user_background_color);
                }
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
                // Draw the layer with the user's colored default background if it exists.
                graphics.DrawImage(colored_background_bitmap, 0, 0, template_width, template_height);

                // Draw the user's background to the base template.
                graphics.DrawImage(background, 0, 0, template_width, template_height);

                // Get the chosen bustup, placed in the correct spot.
                Bitmap placed_bustup = Set_Bustup_Placement(account, bustup, bustup_data, active_session, set_data);

                // Draw the bustup to the template.
                graphics.DrawImage(placed_bustup, 0, 0, placed_bustup.Width, placed_bustup.Height);

                // Draw the message window to the template.
                graphics.DrawImage(Generate_Message_Window(account), 0, 0, template_width, template_height);

                // If the user has it enabled, draw the moon HUD to the template.
                if (account.P1_PSP_TS_Moon_HUD == "On")
                {
                    graphics.DrawImage(Generate_Moon_HUD(account), 0, 0, template_width, template_height);
                }

                // Render the input dialogue to a bitmap.
                Bitmap rendered_dialogue = Render_Dialogue(dialogue_lines);

                // Draw the input dialogue to the template.
                graphics.DrawImage(rendered_dialogue, 0, 0, template_width, template_height);
            }

            // The user could choose to output the image at different resolutions, so let's handle that point now.
            // If the user's output setting is at the default resolution, do nothing.
            if (account.P1_PSX_Resolution == "320 × 240")
            {
                // Do nothing
            }
            // If the user's output setting is NOT at the default resolution, however, we need to do some work.
            else if (account.P1_PSX_Resolution == "1440 × 1080")
            {
                // Change the template width and height variables based on the user's output settings.
                template_width = 1440;
                template_height = 1080;

                // Now, we'll want to make a new bitmap that matches these sizes.
                // Create a copy of the template so far.
                var copied_source = new Bitmap(base_template);

                // Create a new empty bitmap with the adjusted dimensions.
                var scaled_bitmap = new Bitmap(template_width, template_height);

                // Create a new graphics object so we can render on the empty bitmap.
                using (Graphics graphics = Graphics.FromImage(scaled_bitmap))
                {
                    // Set the scaling method to the user's choice of Bicubic and Nearest Neighbor.
                    switch (account.P1_PSX_Scale)
                    {
                        case "Bicubic":
                            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            break;

                        case "Nearest Neighbor":
                            graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                            break;
                    }

                    // Set the rendering quality to high.
                    graphics.CompositingQuality = CompositingQuality.HighQuality;

                    // Draw the copy of the template to the empty bitmap while fitting to size.
                    graphics.DrawImage(copied_source, 0, 0, template_width, template_height);
                }

                // Copy the contents of the new bitmap to the base template variable.
                base_template = scaled_bitmap;
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
                _ = ErrorHandling.Scene_Upload_Failed(sl_command);

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

        public static Bitmap Set_Bustup_Placement(UserInfoFields account, Bitmap bustup, BustupData bustup_data, ContextSwitchData active_session, OfficialSetData set_data)
        {
            // Create variables to store the width and height of the template.
            int template_width = 320;
            int template_height = 240;

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

        public static Bitmap Generate_Message_Window(UserInfoFields account)
        {
            // Create variables to store the width and height of the template.
            int template_width = 320;
            int template_height = 240;

            // Create an empty string to store the wallpaper type in.
            string wallpaper_type = "";

            // Create a starting base bitmap to render all graphics on.
            Bitmap base_template = new Bitmap(template_width, template_height);

            // Set the wallpaper type based on the user's account settings.
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

            // Get the appropriate wallpaper bitmap alongside the message window.
            Bitmap wallpaper = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PS1//Main//Wallpaper//{wallpaper_type}.png");
            Bitmap message_window = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PS1//Main//message_window.png");

            // Now, time to put the template together!
            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                graphics.DrawImage(wallpaper, 0, 0, template_width, template_height);
                graphics.DrawImage(message_window, 0, 0, template_width, template_height);
            }

            return base_template;
        }

        // Text rendering tools
        public static Bitmap Render_Dialogue(List<string>[] dialogue_lines)
        {
            // Create variables to store the width and height of the template.
            int template_width = 320;
            int template_height = 240;

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

        public static List<string>[] Line_Parser(SocialLinkerCommand sl_command, BustupData bustup_data, string dialogue)
        {
            // First, let's establish some values.
            // The max pixel length of a line.
            int max_line_length = 232;

            // The number of pixels in a line remaining. This will gradually decrease as the pixel length of characters are subtracted from it.
            int line_length_remaining = max_line_length;

            // The maximum number of lines on the template. 
            int max_lines = 4;

            // Completed word string. Characters will be added to this string one-by-one until a space, line break, or end-of-input is encountered.
            string completed_word = "";

            // Create an array of string lists and initialize them.
            // These are where our dialogue input will be organized.
            List<string>[] dialogue_list = new List<string>[max_lines];

            // Initialize each index of the string array list.
            for (int i = 0; i < max_lines; i++)
            {
                dialogue_list[i] = new List<string>();
            }

            // Now that we have our string lists created, we need a variable to dynamically change which line we're currently on.
            // For that, create an int variable and initialize it to zero for starting on the first line.
            int current_line = 0;

            // Take the input dialogue and convert it into a char array. This is how we'll iterate through the dialogue character-by-character.
            char[] dialogue_array = dialogue.ToCharArray();

            // Create a for loop meant to iterate through the dialogue array.
            for (int i = 0; i < dialogue_array.Length; i++)
            {
                // Check if the completed word string is empty, the remaining pixel length of the current line is at the max value, and if the current iterated character is a space.
                if ((completed_word == "") && (line_length_remaining == max_line_length) && (dialogue_array[i] == ' '))
                {
                    // We want to skip any spaces that appear at the start of a line, so do nothing here.
                }
                // Check if the contents of the current index is not a space, not a line break, and not the end of the array.
                else if ((dialogue_array[i] != ' ') && (dialogue_array[i] != '\u000a') && (i != dialogue_array.Length - 1))
                {
                    // If so, add the currently iterated char to the completed word string.
                    completed_word += dialogue_array[i];
                }
                // Next, check if the contents of the current index IS a space, IS a line break, or IS the end of the array.
                else if ((dialogue_array[i] == ' ') || (dialogue_array[i] == '\u000a') || (i == dialogue_array.Length - 1))
                {
                    // If so, add the currently iterated char to the completed word string.
                    completed_word += dialogue_array[i];

                    // Now that we have our word, measure the pixel length of the completed string.
                    int completed_word_length = Measure_String_Pixel_Length(sl_command, completed_word);

                    // Check if the completed word is under the current line's allowed length.
                    // This is done by subtracting the completed word string's length from the remaining length of the line.
                    // If the result is greater than zero, it's a perfect fit.
                    if ((line_length_remaining - completed_word_length > 0) && (dialogue_array[i] != '\u000a'))
                    {
                        // Subtract the completed word's pixel length from the remaining pixel length of the current line.
                        line_length_remaining = line_length_remaining - completed_word_length;

                        // Add the completed word to the current line.
                        dialogue_list[current_line].Add(completed_word);

                        // Reset the completed word variable to an empty string.
                        completed_word = "";
                    }

                    // Else, check if all three of the following conditions are met:
                    // If there is no more room to add the completed word to the current line.
                    // The completed word's length is less than or equal to a line itself.
                    // The current iterated character is NOT a line break.
                    else if ((line_length_remaining - completed_word_length < 0) && (completed_word_length <= max_line_length) && (dialogue_array[i] != '\u000a'))
                    {
                        // Check if the current line number is less than the max number of lines available.
                        if (current_line < max_lines - 1)
                        {
                            // Increase the current line number.
                            current_line++;

                            // Add the completed word string to the current line.
                            dialogue_list[current_line].Add(completed_word);

                            // Reset the remaining pixel length variable to the start and subtract the pixel length of the completed word string.
                            // This is done because we moved to a new line.
                            line_length_remaining = max_line_length - completed_word_length;

                            // Reset the completed word variable to an empty string.
                            completed_word = "";
                        }
                        // Else, check if the current line number is greater than or equal to the max number of lines available.
                        else if (current_line >= max_lines - 1)
                        {
                            // If so, there is no more room to render text.
                            // Break from the for loop.
                            break;
                        }
                    }

                    // Else, check if all three of the following conditions are met:
                    // If there IS room to add the completed word to the current line.
                    // The completed word's length is less than or equal to the length of a line itself.
                    // The current iterated character IS a line break.
                    else if ((line_length_remaining - completed_word_length >= 0) && (completed_word_length <= max_line_length) && (dialogue_array[i] == '\u000a'))
                    {
                        // Check if the current line number is less than the max number of lines available.
                        if (current_line < max_lines - 1)
                        {
                            // Since there is room, add the completed word string to the current line.
                            dialogue_list[current_line].Add(completed_word);

                            // Increase the current line number.
                            current_line++;

                            // Reset the remaining pixel length variable to the max value.
                            // This is done because we moved to a new line.
                            line_length_remaining = max_line_length;

                            // Reset the completed word variable to an empty string.
                            completed_word = "";
                        }
                        // Else, check if the current line number is greater than to the max number of lines available.
                        else if (current_line > max_lines - 1)
                        {
                            // If so, there is no more room to render text.
                            // Break from the for loop.
                            break;
                        }
                    }

                    // Else, check if there is no more room to add the completed word to the current line AND the completed word's length is greater than the length of a line itself.
                    // This means that we'll need to split the string up on different lines.
                    else if (line_length_remaining - completed_word_length < 0 && completed_word_length > max_line_length)
                    {
                        // Take the completed word and turn it into a char array.
                        // We'll use this to iterate through the word character-by-character to decide where to split the string.
                        char[] completed_word_array = completed_word.ToCharArray();

                        // Create a new string variable and initialize it to an empty string.
                        // Similar to the completed word variable, this string will contain characters that will fit on a single line.
                        // Because we know the word will be split into multiple lines, this will only contain part of the full string at any given time, hence "substring".
                        string substring = "";

                        // Create an int variable and initialize it to zero.
                        // This will contain the pixel length of our substring variable once we measure it.
                        int substring_length = 0;

                        // Create a for loop to iterate through the completed word array.
                        for (int j = 0; j < completed_word_array.Length; j++)
                        {
                            // Add the currently iterated character to the substring.
                            substring += completed_word_array[j];

                            // Measure the pixel length of the substring so far.
                            substring_length = Measure_String_Pixel_Length(sl_command, substring);

                            // Check if there is no more room to add another character to the current line, OR if the current character is a line break.
                            // Since we are iterating through the string character-by-character, this should trigger the moment the length hits the line boundary.
                            if ((line_length_remaining - substring_length <= 0) || (completed_word_array[j] == '\u000a')) // || (completed_word_array[j] == '\u000a')
                            {
                                // Check if the current line number is less than the max number of lines available.
                                if (current_line < max_lines)
                                {
                                    // Add the substring to the current line.
                                    dialogue_list[current_line].Add(substring);

                                    // Since there is absolutely no more room on the current line left, increase the current line value.
                                    current_line++;

                                    // Reset the remaining pixel length variable to the max value.
                                    // This is done because we moved to a new line.
                                    line_length_remaining = max_line_length;

                                    // Reset the substring variable to an empty string.
                                    substring = "";
                                }
                            }
                            // Else, check if the last index of the completed word array has been reached.
                            else if (j == completed_word_array.Length - 1)
                            {
                                // Add the substring to the current line.
                                dialogue_list[current_line].Add(substring);

                                // Subtract the completed word's pixel length from the remaining pixel length of the current line.
                                line_length_remaining = line_length_remaining - substring_length;

                                // Reset the substring variable to an empty string.
                                substring = "";
                            }
                        }

                        // Reset the completed word string to an empty string.
                        completed_word = "";
                    }
                }
            }

            return dialogue_list;
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

        // Background rendering
        public static Bitmap Center_Image(Bitmap input_bitmap)
        {
            float width = 1920;
            float height = 1080;
            var brush = new SolidBrush(System.Drawing.Color.Black);

            var image = new Bitmap(input_bitmap);

            float scale = Math.Min(width / image.Width, height / image.Height);

            var bmp = new Bitmap((int)width, (int)height);
            var graph = Graphics.FromImage(bmp);

            // uncomment for higher quality output
            graph.InterpolationMode = InterpolationMode.High;
            graph.CompositingQuality = CompositingQuality.HighQuality;
            graph.SmoothingMode = SmoothingMode.AntiAlias;

            bmp.SetResolution(96, 96);

            var scaleWidth = (int)(image.Width * scale);
            var scaleHeight = (int)(image.Height * scale);

            //graph.FillRectangle(brush, new RectangleF(0, 0, width, height));
            graph.DrawImage(image, ((int)width - scaleWidth) / 2, ((int)height - scaleHeight) / 2, scaleWidth, scaleHeight);

            return bmp;
        }

        public static Bitmap Stretch_To_Fit(Bitmap input_bitmap)
        {
            // Set the width and height of the bitmap to be created
            float width = 1920;
            float height = 1080;

            // Copy the input bitmap to a new variable.
            var bitmap_copy = new Bitmap(input_bitmap);

            // Create a brand new bitmap with the specified dimensions from earlier.
            var new_bitmap = new Bitmap((int)width, (int)height);

            // Create a graphics object so we can edit this new bitmap.
            var graphics = Graphics.FromImage(new_bitmap);

            // uncomment for higher quality output
            graphics.InterpolationMode = InterpolationMode.High;
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            new_bitmap.SetResolution(96, 96);

            // Draw the copy of the input bitmap to the new bitmap.
            graphics.DrawImage(bitmap_copy, 0, 0, width, height);

            return new_bitmap;
        }

        // Getter methods
        public static Bitmap Generate_Moon_HUD(UserInfoFields account)
        {
            // Create variables to store the width and height of the template.
            int template_width = 320;
            int template_height = 240;

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
                else if (illumination == 100)
                {
                    phase_text = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PS1//Main//Moon//Phases//Text//full.png");
                    phase_covering = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PS1//Main//Moon//Phases//Images//Coverings//9_full.png");
                }
            }
            // Waning phases
            else if (cycle_age > 14.76)
            {
                // Full moon
                if (illumination == 100)
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

            //phase_covering = Covering_To_Transparent(phase_covering);
            //phase_covering = (Bitmap)Set_Image_Opacity(phase_covering, (float)0.9);

            // Now, let's use a graphics object to draw to the base template and render them all!
            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                graphics.DrawImage(phase_texture, 227, 14, phase_texture.Width, phase_texture.Height);
                graphics.DrawImage(phase_covering, 227, 14, phase_covering.Width, phase_covering.Height);
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

        public static Bitmap Covering_To_Transparent(Bitmap input_bitmap)
        {
            // Create a color variable. We'll use this to iterate through the input bitmap and store each pixel's color values here.
            System.Drawing.Color actual_color;

            // Make an empty bitmap the same size as the input bitmap.
            Bitmap new_bitmap = new Bitmap(input_bitmap.Width, input_bitmap.Height);

            // Create a for loop to iterate over the X values of the bitmap to be changed.
            for (int x = 0; x < input_bitmap.Width; x++)
            {
                // Create a nested for loop to iterate over the Y values of the bitmap to be changed.
                for (int y = 0; y < input_bitmap.Height; y++)
                {
                    // Get the current pixel from the input bitmap.
                    actual_color = input_bitmap.GetPixel(x, y);

                    if (actual_color.R > 5 && actual_color.G > 5 && actual_color.B > 5)
                    {
                        // Do nothing
                    }
                    else
                    {
                        new_bitmap.SetPixel(x, y, actual_color);
                    }
                    
                }
            }

            return new_bitmap;
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

        // Loading messages
        public static EmbedBuilder P1_PS1_Loading_Message()
        {
            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = $"Generating Scene...",
                IconUrl = EmbedSettings.Get_Game_Thumbnail("P1-PS1")
            };

            embed.WithAuthor(author);
            embed.WithColor(EmbedSettings.Get_Game_Color("P1-PS1", null));
            embed.WithThumbnailUrl("https://i.imgur.com/Lv794ze.png");
            embed.WithDescription("This may take a few seconds!");

            return embed;
        }

        public static EmbedBuilder P1_PS1_Multi_Scene_Loading_Message(int passthrough, int number_of_scenes)
        {
            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = $"Generating Scene... (Part {passthrough} / {number_of_scenes})",
                IconUrl = EmbedSettings.Get_Game_Thumbnail("P1-PS1")
            };

            embed.WithAuthor(author);
            embed.WithColor(EmbedSettings.Get_Game_Color("P1-PS1", null));
            embed.WithThumbnailUrl("https://i.imgur.com/Lv794ze.png");
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
