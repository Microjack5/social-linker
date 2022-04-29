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
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace SocialLinker.Core.SceneMaker.TemplateRenders.QuickScenes
{
    public class RenderP5R : ModuleBase<SocketCommandContext>
    {
        public const int template_width = 1920;
        public const int template_height = 1080;

        public static async Task Render_Quick_Scene_P5R(SocketMessage message, OfficialSetData set_data, MakerCommandData command_data)
        {
            try
            {
                // Create two variables for the command user and the command channel, derived from the message object taken in.
                SocketUser user = message.Author;
                SocketTextChannel channel = (SocketTextChannel)message.Channel;

                // Send a loading message to the channel while the sprite sheet is being made.
                RestUserMessage loader = await channel.SendMessageAsync("", false, P5R_Loading_Message().Build());

                // Get the account information of the command's user.
                var account = UserInfoClasses.GetAccount(user);

                BustupData bustup_data = BustupDataMethods.Get_Bustup_Data(account, set_data, command_data);

                // Create a starting base bitmap to render all graphics on.
                Bitmap base_template = new Bitmap(template_width, template_height);

                // Create another bitmap the same size.
                // In case the user has set a colored bitmap in their settings, we'll need to use this to render it.
                Bitmap colored_background_bitmap = new Bitmap(template_width, template_height);

                // Here, we want to grab any images attached to the message to use it as a background.
                // Create a variable for the message attachment.
                var attachments = message.Attachments;

                // Create an empty string variable to hold the URL of the attachment.
                string url = "";

                // If there are no attachments on the message, set the URL string to "None".
                if (attachments.LongCount() == 0)
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
                        _ = ErrorHandling.Incompatible_File_Type(message);
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
                    bustup = OfficialSetMethods.Bustup_Selection(message, account, set_data, bustup_data, command_data);
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
                    // Draw the user's background to the base template.
                    graphics.DrawImage(background, 0, 0, template_width, template_height);

                    // If the user has scene borders enabled, render it to the template.
                    if (account.P5R_TS_Border != "None")
                    {
                        graphics.DrawImage(Render_Scene_Border(account), 0, 0, template_width, template_height);
                    }

                    // If the user has the control panel enabled, render it to the template.
                    if (account.P5R_TS_Panel != "None")
                    {
                        graphics.DrawImage(Render_Control_Panel(account), 0, 0, template_width, template_height);
                    }

                    // Draw the character bust-up to the template if the base sprite number is not '0'.
                    if (command_data.Base_Sprite != 0)
                    {
                        // Make a drop shadow of the bustup first and render it to the template before the main image.
                        Bitmap drop_shadow = Create_Bustup_Drop_Shadow(bustup);
                        drop_shadow = (Bitmap)Set_Image_Opacity(drop_shadow, (float)0.8);
                        graphics.DrawImage(drop_shadow, bustup_data.P5R_Coord_X - 30, bustup_data.P5R_Coord_Y + 30, bustup_data.P5R_Scale_Width, bustup_data.P5R_Scale_Height);

                        // Render the main bustup nest.
                        graphics.DrawImage(bustup, bustup_data.P5R_Coord_X, bustup_data.P5R_Coord_Y, bustup_data.P5R_Scale_Width, bustup_data.P5R_Scale_Height);
                    }

                    // If the user has the HUD enabled, render it to the template as well.
                    if (account.P5R_TS_HUD != "None")
                    {
                        graphics.DrawImage(Construct_Calendar(message, account), 0, 0, template_width, template_height);
                    }

                    // Draw the input dialogue to the template.
                    graphics.DrawImage(Render_Dialogue(Line_Parser(message, command_data.Dialogue)), 0, 0, template_width, template_height);
                }

                // Save the entire base template to a data stream.
                MemoryStream memoryStream = new MemoryStream();
                base_template.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
                //RenderRecursiveStar(System.Drawing.Color.Black).Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
                memoryStream.Seek(0, SeekOrigin.Begin);

                try
                {
                    // Send the image.
                    await message.Channel.SendFileAsync(memoryStream, $"scene_{message.Author.Id}_{DateTime.UtcNow}.png");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);

                    // Send an error message to the user if the image upload fails.
                    _ = ErrorHandling.Scene_Upload_Failed(message);

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
                    await message.DeleteAsync();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static Bitmap Render_Dialogue(List<string>[] dialogue_lines)
        {
            //Create a bitmap as large as the template
            Bitmap bitmap = new Bitmap(template_width, template_height);

            // Create an int to keep track of rendering errors. This is neccessary to inform the user of any potential issues.
            int error_counter = 0;

            //Establish an int for the width and height glyphs should be rendered at
            int multiplier = 48;

            string font_sheet = $@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Font//p5r_font_sheet.png";
            Bitmap current_glyph;

            for (int i = 0; i < dialogue_lines.Length; i++)
            {
                //Establish variables for where the glyphs should be rendered on the template
                int render_position_x = 680;
                int render_position_y = 914 + (68 * i);

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
                                graphics.DrawImage(current_glyph, (render_position_x - glyph.LeftCut), render_position_y, 42, 42);
                            }
                        }

                        //Set the next X value at the end of the current glyph's right width
                        render_position_x += (glyph.RightCut - glyph.LeftCut);

                        // Check if the current iterated index is less than the number of indicies available.
                        if (j < char_array.Length - 1)
                        {
                            // If so, edit the position of the X coordinate according to specific kerning pairs.

                            /*if (char_array[j] == 'w')
                            {
                                render_position_x += -1;
                            } */

                            render_position_x += -1;

                            if (char_array[j] == 'h' && char_array[j + 1] == 'i')
                            {
                                render_position_x += +1;
                            }
                            else if (char_array[j] == 't' && char_array[j + 1] == 'h')
                            {
                                render_position_x += +1;
                            }
                            else if (char_array[j] == 'o' && char_array[j + 1] == 'm')
                            {
                                render_position_x += -1;
                            }
                            else if (char_array[j] == 'l' && char_array[j + 1] == 'i')
                            {
                                render_position_x += +1;
                            }
                            else if (char_array[j] == 'r' && char_array[j + 1] == '!')
                            {
                                render_position_x += +2;
                            }
                            // Next
                            else if (char_array[j] == 'D' && char_array[j + 1] == 'o')
                            {
                                render_position_x += -1;
                            }
                            else if (char_array[j] == 's' && char_array[j + 1] == 'o')
                            {
                                render_position_x += -1;
                            }
                            else if (char_array[j] == 'm' && char_array[j + 1] == 'e')
                            {
                                render_position_x += -1;
                            }
                            // Next
                            else if (char_array[j] == 't' && char_array[j + 1] == 'o')
                            {
                                render_position_x += +1;
                            }
                            else if (char_array[j] == 'f' && char_array[j + 1] == 'i')
                            {
                                render_position_x += +1;
                            }
                            else if (char_array[j] == 'c' && char_array[j + 1] == 'i')
                            {
                                render_position_x += +1;
                            }
                            else if (char_array[j] == 'W' && char_array[j + 1] == 'e')
                            {
                                render_position_x += -2;
                            }
                            else if (char_array[j] == 't' && char_array[j + 1] == 'e')
                            {
                                render_position_x += +1;
                            }
                            // Next
                            else if (char_array[j] == 'H' && char_array[j + 1] == 'e')
                            {
                                render_position_x += -1;
                            }
                            else if (char_array[j] == 'e' && char_array[j + 1] == 'h')
                            {
                                render_position_x += +1;
                            }
                            else if (char_array[j] == 'h' && char_array[j + 1] == 'e')
                            {
                                render_position_x += -1;
                            }
                            else if (char_array[j] == 'e' && char_array[j + 1] == ',')
                            {
                                render_position_x += +1;
                            }
                            else if (char_array[j] == 'l' && char_array[j + 1] == 'l')
                            {
                                render_position_x += +1;
                            }
                            else if (char_array[j] == 'm' && char_array[j + 1] == 'e')
                            {
                                render_position_x += -1;
                            }
                            else if (char_array[j] == 'y' && char_array[j + 1] == '.')
                            {
                                render_position_x += +1;
                            }
                            else if (char_array[j] == '.' && char_array[j + 1] == '\"')
                            {
                                render_position_x += +1;
                            }
                            // Next
                            else if (char_array[j] == 'l' && char_array[j + 1] == 'o')
                            {
                                render_position_x += +1;
                            }
                            else if (char_array[j] == 'd' && char_array[j + 1] == 'e')
                            {
                                render_position_x += +1;
                            }
                            else if (char_array[j] == 'r' && char_array[j + 1] == 'd')
                            {
                                render_position_x += -1;
                            }
                            else if (char_array[j] == '.' && char_array[j + 1] == '.')
                            {
                                render_position_x += +1;
                            }
                            // Next
                            else if (char_array[j] == 'w' && char_array[j + 1] == 'h')
                            {
                                render_position_x += -1;
                            }
                            else if (char_array[j] == 'h' && char_array[j + 1] == 'i')
                            {
                                render_position_x += +1;
                            }
                            else if (char_array[j] == 'i' && char_array[j + 1] == 'l')
                            {
                                render_position_x += +1;
                            }
                            else if (char_array[j] == 't' && char_array[j + 1] == 'u')
                            {
                                render_position_x += +1;
                            }
                            else if (char_array[j] == 'y' && char_array[j + 1] == 'i')
                            {
                                render_position_x += +1;
                            }
                            else if (char_array[j] == 'n' && char_array[j + 1] == 'i')
                            {
                                render_position_x += +1;
                            }
                            /*else if (char_array[j] == 'g' && char_array[j + 1] == 'h')
                            {
                                render_position_x += +1;
                            } */
                            else if (char_array[j] == 't' && char_array[j + 1] == '.')
                            {
                                render_position_x += +1;
                            }
                            // Next
                            else if (char_array[j] == 'M' && char_array[j + 1] == 'y')
                            {
                                render_position_x += -1;
                            }
                            else if (char_array[j] == 'a' && char_array[j + 1] == 'm')
                            {
                                render_position_x += -1;
                            }
                            else if (char_array[j] == 'm' && char_array[j + 1] == 'e')
                            {
                                render_position_x += -1;
                            }
                            else if (char_array[j] == 'W' && char_array[j + 1] == 'i')
                            {
                                render_position_x += -1;
                            }
                            // Next
                            else if (char_array[j] == 'H' && char_array[j + 1] == 'o')
                            {
                                render_position_x += -1;
                            }
                            else if (char_array[j] == 'o' && char_array[j + 1] == 'w')
                            {
                                render_position_x += -1;
                            }
                            else if (char_array[j] == 'm' && char_array[j + 1] == 'a')
                            {
                                render_position_x += -1;
                            }
                            else if (char_array[j] == 'f' && char_array[j + 1] == 'l')
                            {
                                render_position_x += +1;
                            }
                            else if (char_array[j] == 'w' && char_array[j + 1] == 'n')
                            {
                                render_position_x += -1;
                            }
                            else if (char_array[j] == 'i' && char_array[j + 1] == 's')
                            {
                                render_position_x += -1;
                            }
                            else if (char_array[j] == 'p' && char_array[j + 1] == 'l')
                            {
                                render_position_x += +1;
                            }
                            else if (char_array[j] == 'g' && char_array[j + 1] == 'o')
                            {
                                render_position_x += -1;
                            }
                            // Next
                            else if (char_array[j] == 'm' && char_array[j + 1] == 'u')
                            {
                                render_position_x += -1;
                            }
                            // Next
                            else if (char_array[j] == 't' && char_array[j + 1] == 'h')
                            {
                                render_position_x += +1;
                            }
                            else if (char_array[j] == 'l' && char_array[j + 1] == 'i')
                            {
                                render_position_x += +1;
                            }
                            else if (char_array[j] == 'i' && char_array[j + 1] == 'k')
                            {
                                render_position_x += +1;
                            }
                            else if (char_array[j] == 'i' && char_array[j + 1] == 's')
                            {
                                render_position_x += +1;
                            }
                            // Next
                            else if (char_array[j] == 't' && char_array[j + 1] == ',')
                            {
                                render_position_x += +1;
                            }
                            else if (char_array[j] == 'w' && char_array[j + 1] == 'a')
                            {
                                render_position_x += -1;
                            }
                            else if (char_array[j] == 't' && char_array[j + 1] == 'e')
                            {
                                render_position_x += +1;
                            }
                        }
                    }
                }
            }

            return bitmap;
        }

        public static List<string>[] Line_Parser(SocketMessage message, string dialogue)
        {
            // First, let's establish some values.
            // The max pixel length of a line.
            int max_line_length = 660;

            // The number of pixels in a line remaining. This will gradually decrease as the pixel length of characters are subtracted from it.
            int line_length_remaining = max_line_length;

            // The maximum number of lines on the template. 
            int max_lines = 3;

            // Completed word string. Characters will be added to this string one-by-one until a space, line break, or end-of-input is encountered.
            string completed_word = "";

            // Create an array of three string lists and initialize them.
            // These are where our dialogue input will be organized.
            List<string>[] dialogue_list = new List<string>[3];

            dialogue_list[0] = new List<string>();
            dialogue_list[1] = new List<string>();
            dialogue_list[2] = new List<string>();

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
                    int completed_word_length = Measure_Word_Pixel_Length(message, completed_word);

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
                            substring_length = Measure_Word_Pixel_Length(message, substring);

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

        public static int Measure_Word_Pixel_Length(SocketMessage message, string input_word)
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
                        // Check if the current iterated index is less than the number of indicies available.
                        if (i < char_array.Length - 1)
                        {
                            // If so, edit the position of the X coordinate according to specific kerning pairs.
                            /*if (char_array[i] == 'Y' && Char.IsLower(char_array[i + 1]))
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
                            } */
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
                        _ = ErrorHandling.Unsupported_Character(message);
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

        public static Bitmap Render_Screen_Border(SocketMessage message, UserInfoFields account)
        {
            Bitmap base_template = new Bitmap(template_width, template_height);

            Bitmap border_main = new Bitmap(2, 2);
            Bitmap border_secondary = new Bitmap(2, 2);

            switch (account.P5R_TS_Border)
            {
                case "Event":
                    border_main = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Border//event_main.png");
                    border_secondary = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Border//event_secondary.png");
                    break;

                case "Interaction":
                    border_main = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Border//interaction_main.png");
                    border_secondary = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Border//interaction_secondary.png");
                    break;
            }

            // Now, time to put the template together!
            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                graphics.DrawImage(border_main, 0, 0, template_width, template_height);
                graphics.DrawImage(border_secondary, 0, 0, template_width, template_height);
            }

            return base_template;
        }

        // Vector rendering
        public static Bitmap Render_Message_Window(int number_of_lines, int max_line_length)
        {
            // How the vectors are rendered is strongly determined
            int default_line_length = 365;
            int starting_dialogue_position = 672;

            int end_of_line = 0;

            if (max_line_length > default_line_length)
            {
                end_of_line = starting_dialogue_position + max_line_length;
            }
            else
            {
                end_of_line = starting_dialogue_position + default_line_length;
            }

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

            // Create a new random variable.
            Random rnd = new Random();

            // Create multiple variables for the potential min and max values of the thirteen black outer points of the message window.
            int black_point_1_x_min = 448;
            int black_point_1_x_max = 456;
            int black_point_1_y_min = 902;
            int black_point_1_y_max = 912;

            int black_point_2_x_min = 448;
            int black_point_2_x_max = 460;
            int black_point_2_y_min = 912;
            int black_point_2_y_max = 921;

            int black_point_3_x_min = 541;
            int black_point_3_x_max = 555;
            int black_point_3_y_min = 990;
            int black_point_3_y_max = 994;

            int black_point_4_x_min = 557;
            int black_point_4_x_max = 576;
            int black_point_4_y_min = 969;
            int black_point_4_y_max = 973;

            int black_point_5_x_min = 633;
            int black_point_5_x_max = 643;
            int black_point_5_y_min = 1025;
            int black_point_5_y_max = 1029;

            int black_point_6_x_min = 662;
            int black_point_6_x_max = 675;
            int black_point_6_y_min = 999;
            int black_point_6_y_max = 1005;

            // Sect 1

            int black_point_7_x_min = end_of_line + 0;
            int black_point_7_x_max = end_of_line + 0;
            int black_point_7_y_min = 0;
            int black_point_7_y_max = 0;

            int black_point_8_x_min = end_of_line + 0;
            int black_point_8_x_max = end_of_line + 0;
            int black_point_8_y_min = 0;
            int black_point_8_y_max = 0;

            int black_point_9_x_min = end_of_line + 0;
            int black_point_9_x_max = end_of_line + 0;
            int black_point_9_y_min = 0;
            int black_point_9_y_max = 0;

            int black_point_10_x_min = end_of_line + 0;
            int black_point_10_x_max = end_of_line + 0;
            int black_point_10_y_min = 0;
            int black_point_10_y_max = 0;

            int black_point_11_x_min = 0;
            int black_point_11_x_max = 0;
            int black_point_11_y_min = 0;
            int black_point_11_y_max = 0;

            int black_point_12_x_min = 598;
            int black_point_12_x_max = 605;
            int black_point_12_y_min = 854;
            int black_point_12_y_max = 859;

            int black_point_13_x_min = 578;
            int black_point_13_x_max = 590;
            int black_point_13_y_min = 907;
            int black_point_13_y_max = 914;

            int black_point_14_x_min = 0;
            int black_point_14_x_max = 0;
            int black_point_14_y_min = 0;
            int black_point_14_y_max = 0;

            int black_point_15_x_min = 0;
            int black_point_15_x_max = 0;
            int black_point_15_y_min = 0;
            int black_point_15_y_max = 0;

            int black_point_16_x_min = 507;
            int black_point_16_x_max = 525;
            int black_point_16_y_min = 906;
            int black_point_16_y_max = 919;

            switch (number_of_lines)
            {
                case 2:
                    black_point_6_y_min = black_point_6_y_min + 19;
                    black_point_6_y_max = black_point_6_y_max + 19;

                    black_point_7_y_min = black_point_7_y_min + 19;
                    black_point_7_y_max = black_point_7_y_max + 19;

                    black_point_8_y_min = black_point_8_y_min - 8;
                    black_point_8_y_max = black_point_8_y_max - 8;

                    black_point_9_y_min = black_point_9_y_min - 22;
                    black_point_9_y_max = black_point_9_y_max - 22;

                    black_point_10_y_min = black_point_10_y_min - 22;
                    black_point_10_y_max = black_point_10_y_max - 22;
                    break;

                case 3:
                    black_point_6_y_min = black_point_6_y_min + 38;
                    black_point_6_y_max = black_point_6_y_max + 38;

                    black_point_7_y_min = black_point_7_y_min + 38;
                    black_point_7_y_max = black_point_7_y_max + 38;

                    black_point_8_y_min = black_point_8_y_min - 16;
                    black_point_8_y_max = black_point_8_y_max - 16;

                    black_point_9_y_min = black_point_9_y_min - 44;
                    black_point_9_y_max = black_point_9_y_max - 44;

                    black_point_10_y_min = black_point_10_y_min - 44;
                    black_point_10_y_max = black_point_10_y_max - 44;
                    break;

                default:
                    // Do nothing
                    break;
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

            int black_point_10_x = rnd.Next(black_point_10_x_min, black_point_10_x_max + 1);
            int black_point_10_y = rnd.Next(black_point_10_y_min, black_point_10_y_max + 1);

            int black_point_11_x = rnd.Next(black_point_11_x_min, black_point_11_x_max + 1);
            int black_point_11_y = rnd.Next(black_point_11_y_min, black_point_11_y_max + 1);

            int black_point_12_x = rnd.Next(black_point_12_x_min, black_point_12_x_max + 1);
            int black_point_12_y = rnd.Next(black_point_12_y_min, black_point_12_y_max + 1);

            int black_point_13_x = rnd.Next(black_point_13_x_min, black_point_13_x_max + 1);
            int black_point_13_y = rnd.Next(black_point_13_y_min, black_point_13_y_max + 1);

            // Randomly set the X and Y values of the thirteen points of the inner white vector based on the set black point X & Y values.
            int white_point_1_x = rnd.Next(black_point_1_x + 15, black_point_1_x + 16);
            int white_point_1_y = rnd.Next(black_point_1_y + 14, black_point_1_y + 15);

            int white_point_2_x = rnd.Next(black_point_2_x - 4, black_point_2_x + 3);
            int white_point_2_y = rnd.Next(black_point_2_y - 31, black_point_2_y - 26);

            int white_point_3_x = rnd.Next(black_point_3_x - 7, black_point_3_x - 1);
            int white_point_3_y = rnd.Next(black_point_3_y - 23, black_point_3_y - 19);

            int white_point_4_x = rnd.Next(black_point_4_x - 4, black_point_4_x + 0);
            int white_point_4_y = rnd.Next(black_point_4_y - 29, black_point_4_y - 20);

            int white_point_5_x = rnd.Next(black_point_5_x - 6, black_point_5_x - 1);
            int white_point_5_y = rnd.Next(black_point_5_y - 29, black_point_5_y - 23);

            int white_point_6_x = rnd.Next(black_point_6_x - 2, black_point_6_x + 3);
            int white_point_6_y = rnd.Next(black_point_6_y - 21, black_point_6_y - 18);

            int white_point_7_x = rnd.Next(black_point_7_x - 8, black_point_7_x - 5);
            int white_point_7_y = rnd.Next(black_point_7_y - 17, black_point_7_y - 13);

            int white_point_8_x = rnd.Next(black_point_8_x - 17, black_point_8_x - 14);
            int white_point_8_y = rnd.Next(black_point_8_y + 4, black_point_8_y + 7);

            int white_point_9_x = rnd.Next(black_point_9_x - 10, black_point_9_x - 3);
            int white_point_9_y = rnd.Next(black_point_9_y + 14, black_point_9_y + 19);

            int white_point_10_x = rnd.Next(black_point_10_x + 14, black_point_10_x + 20);
            int white_point_10_y = rnd.Next(black_point_10_y + 13, black_point_10_y + 19);

            int white_point_11_x = rnd.Next(black_point_11_x + 4, black_point_11_x + 12);
            int white_point_11_y = rnd.Next(black_point_11_y + 26, black_point_11_y + 33);

            int white_point_12_x = rnd.Next(black_point_12_x - 2, black_point_12_x + 12);
            int white_point_12_y = rnd.Next(black_point_12_y + 21, black_point_12_y + 29);

            int white_point_13_x = rnd.Next(black_point_13_x + 3, black_point_13_x + 10);
            int white_point_13_y = rnd.Next(black_point_13_y + 18, black_point_13_y + 30);

            // Randomly set the X and Y values of the thirteen points of the innermost black vector (we'll call it 'void' here) based on the set white point X & Y values.
            int void_point_1_x = rnd.Next(white_point_1_x + 19, white_point_1_x + 20);
            int void_point_1_y = rnd.Next(white_point_1_y + 20, white_point_1_y + 21);

            int void_point_2_x = rnd.Next(white_point_2_x - 3, white_point_2_x + 3);
            int void_point_2_y = rnd.Next(white_point_2_y - 36, white_point_2_y - 21);

            int void_point_3_x = rnd.Next(white_point_3_x - 4, white_point_3_x - 1);
            int void_point_3_y = rnd.Next(white_point_3_y - 26, white_point_3_y - 22);

            int void_point_4_x = rnd.Next(white_point_4_x - 2, white_point_4_x + 1);
            int void_point_4_y = rnd.Next(white_point_4_y - 30, white_point_4_y - 21);

            int void_point_5_x = rnd.Next(white_point_5_x - 2, white_point_5_x + 0);
            int void_point_5_y = rnd.Next(white_point_5_y - 25, white_point_5_y - 21);

            int void_point_6_x = rnd.Next(white_point_6_x + 0, white_point_6_x + 7);
            int void_point_6_y = rnd.Next(white_point_6_y - 16, white_point_6_y - 13);

            int void_point_7_x = rnd.Next(white_point_7_x - 15, white_point_7_x - 4);
            int void_point_7_y = rnd.Next(white_point_7_y - 10, white_point_7_y - 7);

            int void_point_8_x = rnd.Next(white_point_8_x - 19, white_point_8_x - 16);
            int void_point_8_y = rnd.Next(white_point_8_y - 1, white_point_8_y + 5);

            int void_point_9_x = rnd.Next(white_point_9_x - 6, white_point_9_x + 4);
            int void_point_9_y = rnd.Next(white_point_9_y + 15, white_point_9_y + 19);

            int void_point_10_x = rnd.Next(white_point_10_x + 14, white_point_10_x + 18);
            int void_point_10_y = rnd.Next(white_point_10_y + 10, white_point_10_y + 16);

            int void_point_11_x = rnd.Next(white_point_11_x + 6, white_point_11_x + 10);
            int void_point_11_y = rnd.Next(white_point_11_y + 23, white_point_11_y + 30);

            int void_point_12_x = rnd.Next(white_point_12_x + 4, white_point_12_x + 9);
            int void_point_12_y = rnd.Next(white_point_12_y + 18, white_point_12_y + 29);

            int void_point_13_x = rnd.Next(white_point_13_x + 3, white_point_13_x + 9);
            int void_point_13_y = rnd.Next(white_point_13_y + 16, white_point_13_y + 22);

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
            Point void_point_12 = new Point(void_point_12_x, void_point_12_y);
            Point void_point_13 = new Point(void_point_13_x, void_point_13_y);

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
                    void_point_11,
                    void_point_12,
                    void_point_13 };

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

            // Let's merge the black and white layers into one bitmap.
            using (Graphics graphics = Graphics.FromImage(black_white_layer))
            {
                // Set the graphics rendering to have antialiasing.
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

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

            // Return the base template.
            return base_template;
        }

        public static Bitmap RenderStar(double scale_factor, System.Drawing.Color star_color)
        {
            int template_width = 8000;
            int template_height = 8000;

            // Make a new bitmap large enough for a working space.
            Bitmap new_bitmap = new Bitmap(template_width, template_height);

            // Use a graphics object to edit the bitmap.
            using (Graphics graphics = Graphics.FromImage(new_bitmap))
            {
                // Establish the center point of the star.
                Point center_point = new Point(template_width / 2, template_height / 2); // 449, 471

                // Create an array of ints that will establish the X and Y values of each angle of the star. Even array indexes are X values, odd indexes are Y values.
                int[] star_points = new int[] { 0, -227, 58, -80, 216, -70, 95, 32, 134, 184, 0, 100, -132, 184, -94, 32, -216, -70, -58, -80 };

                // Edit each array index by multiplying them by the scaling factor.
                for (int i = 0; i < star_points.Length; i++)
                {
                    star_points[i] = (int)(star_points[i] * scale_factor);
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

        public static Bitmap RenderRecursiveStar(System.Drawing.Color star_type)
        {
            int template_width = 8000;
            int template_height = 8000;

            // Make a new bitmap large enough for a working space.
            Bitmap new_bitmap = new Bitmap(template_width, template_height);

            // Use a graphics object to edit the bitmap.
            using (Graphics graphics = Graphics.FromImage(new_bitmap))
            {
                // Create a new random variable.
                Random rnd = new Random();

                // Randomly determine the maximum size of the star between two points. 24 must be the lowest point to get eight layers of stars minimum (24 divided by 3).
                double start_size = rnd.NextDouble(24.0, 29.9);

                // Create another graphics object. This will establish a cropping region in the shape of a star (for the star itself) to give a greater visual effect.
                using (Graphics region_crop = Graphics.FromImage(new_bitmap))
                {
                    // Establish the center point of the star.
                    Point center_point = new Point(template_width / 2, template_height / 2);

                    // Create an array of ints that will establish the X and Y values of each angle of the star. Even array indexes are X values, odd indexes are Y values.
                    int[] star_points = new int[] { 0, -227, 58, -80, 216, -70, 95, 32, 134, 184, 0, 100, -132, 184, -94, 32, -216, -70, -58, -80 };

                    // Edit each array index by multiplying them by 24. Again, 24 must be the lowest point to get eight layers of stars minimum since the stars will be made in decrements of three. 24 divided by 3 is eight.
                    for (int i = 0; i < star_points.Length; i++)
                    {
                        star_points[i] = (int)(star_points[i] * 24);
                    }

                    // Create points for the cropping reigon by adding on the star_point indexes to the center_point coordinates.
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
                    for (double i = start_size; i > 0; i = i - 3)
                    {
                        // start_point_int casts the current double to an int for rounding purposes.
                        int start_point_int = (int)i;

                        // If the double is even, color the star either black or gray depinding on the star type specified. If it's odd, color it white.
                        if (start_point_int % 2 == 0)
                        {
                            region_crop.DrawImage(RenderStar(i, star_type), 0, 0, template_width, template_height);
                        }
                        else
                        {
                            region_crop.DrawImage(RenderStar(i, System.Drawing.Color.White), 0, 0, template_width, template_height);
                        }
                    }



                }
            }

            Bitmap smaller_template = new Bitmap(900, 900);

            using (Graphics graphics = Graphics.FromImage(smaller_template))
            {
                graphics.DrawImage(new_bitmap, 0, 0, 900, 900);
            }

            new_bitmap = smaller_template;

            // Return the new bitmap.
            return new_bitmap;
        }

        // Border rendering
        public static Bitmap Render_Scene_Border(UserInfoFields account)
        {
            // Make an empty bitmap.
            Bitmap base_bitmap = new Bitmap(template_width, template_height);

            // Create needed bitmap variables for needed assets. We'll initialize them to small bitmaps for now.
            Bitmap border_main = new Bitmap(2, 2);
            Bitmap border_secondary = new Bitmap(2, 2);

            // Here, we'll assign the border graphics based on the user's settings.
            switch (account.P5R_TS_Border)
            {
                case "Event":
                    border_main = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Border//event_main.png");
                    border_secondary = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Border//event_secondary.png");
                    break;

                case "Interaction":
                    border_main = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Border//interaction_main.png");
                    border_secondary = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Border//interaction_secondary.png");
                    break;

                default:
                    break;
            }

            // Draw the assets to the template.
            using (Graphics graphics = Graphics.FromImage(base_bitmap))
            {
                graphics.DrawImage(border_main, 0, 0, template_width, template_height);
                graphics.DrawImage(border_secondary, 0, 0, template_width, template_height);
            }

            return base_bitmap;
        }

        public static Bitmap Render_Control_Panel(UserInfoFields account)
        {
            // Make an empty bitmap.
            Bitmap base_bitmap = new Bitmap(template_width, template_height);

            // Create needed bitmap variables for needed assets. We'll initialize them to small bitmaps for now.
            Bitmap auto_toggle = new Bitmap(2, 2);
            Bitmap auto_wheel = new Bitmap(2, 2);
            Bitmap ffwd_button = new Bitmap(2, 2);
            Bitmap log_button = new Bitmap(2, 2);

            // Start assigning assets to variables that will be constant on either user setting.
            ffwd_button = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Control_Panel//ffwd.png");
            log_button = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Control_Panel//log.png");

            // Here, we'll assign the auto graphics based on the user's settings.
            switch (account.P5R_TS_Panel)
            {
                case "Manual":
                    auto_toggle = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Control_Panel//auto_toggle_default.png");
                    break;

                case "Auto-advance":
                    // Use a random variable for the auto wheel icon so it can change in each scene.
                    Random w = new Random();
                    int wInt = w.Next(1, 5);

                    auto_toggle = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Control_Panel//auto_toggle_active.png");
                    auto_wheel = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Control_Panel//auto_wheel_{wInt}.png");
                    break;

                default:
                    break;
            }

            // Draw the assets to the template.
            using (Graphics graphics = Graphics.FromImage(base_bitmap))
            {
                graphics.DrawImage(ffwd_button, 0, 0, template_width, template_height);
                graphics.DrawImage(auto_wheel, 0, 0, template_width, template_height);
                graphics.DrawImage(auto_toggle, 0, 0, template_width, template_height);
                graphics.DrawImage(log_button, 0, 0, template_width, template_height);
            }

            return base_bitmap;
        }

        // Calendar rendering
        public static Bitmap Construct_Calendar(SocketMessage message, UserInfoFields account)
        {
            // Get the user's current date and time according to their settings.
            DateTime user_time = Get_Date(message, account);

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
                    dayTop = System.Drawing.Image.FromFile($@"C:\Users\Microjack5\Desktop\Public Test Storage\calendar_top\day\single_digit\{user_time.Day}.png");
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
                        merged_calendar.DrawImage(Keep_Pixel_Overlap((Bitmap)dayMiddle_filler, (Bitmap)decoration_2), 0, 0, template_width, template_height);

                        // Alter the chocolate layers so that only the pixels where it overlaps with the middle day appears.
                        decoration_1 = Keep_Pixel_Overlap((Bitmap)dayMiddle_tens, (Bitmap)decoration_1);
                        decoration_2 = Keep_Pixel_Overlap((Bitmap)dayMiddle_ones, (Bitmap)decoration_2);

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
                        calendar_decorations.DrawImage(Keep_Pixel_Overlap((Bitmap)dayTop_tens, (Bitmap)decoration_1), 0, 0, template_width, template_height);
                        calendar_decorations.DrawImage(Keep_Pixel_Overlap((Bitmap)dayTop_ones, (Bitmap)decoration_2), 0, 0, template_width, template_height);
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

        public static Bitmap Construct_White_Calendar(DateTime user_time, UserInfoFields account)
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

                // Use a random variable for the weather icon so it can change in each scene.
                Random rnd = new Random();

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
                    calendar_decorations.DrawImage(Keep_Pixel_Overlap(merged_layer, (Bitmap)decoration_2), 0, 0, template_width, template_height);
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

        public static Bitmap Construct_Harvest_Calendar(DateTime user_time, UserInfoFields account)
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

        public static Bitmap Generate_Petals()
        {
            int width = 435;
            int height = 330;
            Random rnd = new Random();

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

        public static Bitmap Keep_Pixel_Overlap(Bitmap bottom_bitmap, Bitmap top_bitmap)
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

        public static Bitmap Delete_Pixel_Overlap(Bitmap bottom_bitmap, Bitmap top_bitmap)
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

                    if (bottom_pixel_color.A > 20 && top_pixel_color.A > 20)
                    {
                        //Don't draw the pixel; it needs to be transparent
                    }
                    else
                    {
                        //Draw the top layer's pixel if both layers don't overlap
                        newBitmap.SetPixel(i, j, top_pixel_color);
                    }
                }
            }

            return newBitmap;
        }

        // Getter methods
        public static DateTime Get_Date(SocketMessage message, UserInfoFields account)
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
                _ = ErrorHandling.API_Timeout(message);

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

        // Coloring bitmaps
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

        // Calendar checks
        public static bool Holiday_Check(DateTime user_time)
        {
            try
            {
                // Establish the directory of the file and then search for all JSON documents that start with "holiday_calendar_". This should only bring in one result.
                string holiday_calendar_path = $@"C:\Users\Microjack5\Documents\Social_Linker_Final\SocialLinker\Assets\SceneMaker\Data\Calendar_Data";
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
                string holiday_calendar_path = $@"C:\Users\Microjack5\Documents\Social_Linker_Final\SocialLinker\Assets\SceneMaker\Data\Calendar_Data";
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

        // Loading message
        public static EmbedBuilder P5R_Loading_Message()
        {
            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Generating Scene...",
                IconUrl = "https://i.imgur.com/WV32GRK.png"
            };

            embed.WithAuthor(author);
            embed.WithColor(213, 27, 4);
            embed.WithThumbnailUrl("https://i.imgur.com/PYMB6XG.gif");
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
}
