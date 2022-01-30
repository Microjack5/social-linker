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
using System.Drawing.Imaging;
using System.Globalization;
using System.Runtime.InteropServices;

namespace SocialLinker.Core.SceneMaker.TemplateRenders.QuickScenes
{
    public static class RenderP3F
    {
        public static async Task Render_Quick_Scene_P3F(SocketMessage message, OfficialSetData set_data, MakerCommandData command_data)
        {
            // Create variables to store the width and height of the template.
            int template_width = 640;
            int template_height = 448;

            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = message.Author;
            SocketTextChannel channel = (SocketTextChannel)message.Channel;

            // Send a loading message to the channel while the sprite sheet is being made.
            RestUserMessage loader = await channel.SendMessageAsync("", false, P3F_Loading_Message().Build());

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
                // Create and assign bitmap variables for the assets needed.
                Bitmap message_window = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3F//Main//message_window.png");
                Bitmap cursor = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3F//Main//cursor.png");

                // Draw the layer with the user's colored default background if it exists.
                graphics.DrawImage(colored_background_bitmap, 0, 0, template_width, template_height);

                // Draw the user's background to the base template.
                graphics.DrawImage(background, 0, 0, template_width, template_height);

                // Draw the character bust-up to the template if the base sprite number is not '0'.
                if (command_data.Base_Sprite != 0)
                {
                    graphics.DrawImage(bustup, bustup_data.P3F_Coord_X, bustup_data.P3F_Coord_Y, bustup_data.P3F_Scale_Width, bustup_data.P3F_Scale_Height);
                }

                // Draw the message window layer to the base template.
                graphics.DrawImage(message_window, 0, 0, template_width, template_height);

                // Draw the cursor layer to the base template.
                graphics.DrawImage(cursor, 0, 0, template_width, template_height);

                // If the user has the HUD enabled, render it to the template as well.
                if (account.P3F_TS_HUD != "None")
                {
                    graphics.DrawImage(Render_Calendar_HUD(account), 0, 0, template_width, template_height);
                }
            }

            // The user could choose to output the image at different resolutions, so let's handle that point now.
            // This is done before the text is rendered to the template.
            // If the user's output setting is at the default resolution, do nothing.
            if (account.P3F_Resolution == "640 × 448")
            {
                // Do nothing
            }
            // If the user's output setting is NOT at the default resolution, however, we need to do some work.
            else
            {
                // Change the template width and height variables based on the user's output settings.
                if (account.P3F_Resolution == "640 × 480")
                {
                    template_width = 640;
                    template_height = 480;
                }
                else if (account.P3F_Resolution == "1440 × 1080")
                {
                    template_width = 1440;
                    template_height = 1080;
                }

                // Now, we'll want to make a new bitmap that matches these sizes.
                // Create a copy of the template so far.
                var image = new Bitmap(base_template);

                // Create a new empty bitmap with the adjusted dimensions.
                var scaled_bitmap = new Bitmap(template_width, template_height);

                // Create a new graphics object so we can render on the empty bitmap.
                using (Graphics graphics = Graphics.FromImage(scaled_bitmap))
                {
                    // If the user's setting is at Full HD, set the scaling method to their choice of Bicubic and Nearest Neighbor.
                    if (account.P3F_Resolution == "1440 × 1080")
                    {
                        switch (account.P3F_Scale)
                        {
                            case "Bicubic":
                                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                                break;

                            case "Nearest Neighbor":
                                graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                                break;
                        }
                    }
                    // Otherwise, set the method to Bicubic.
                    else
                    {
                        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    }
                    
                    // Set the rendering quality to high.
                    graphics.CompositingQuality = CompositingQuality.HighQuality;

                    // Draw the copy of the template to the empty bitmap while fitting to size.
                    graphics.DrawImage(image, 0, 0, template_width, template_height);
                }

                // Copy the contents of the new bitmap to the base template variable.
                base_template = scaled_bitmap;
            }

            // Create another graphics object for the base template.
            // We'll start rendering our needed text here.
            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                // Check if the base sprite number is something other than zero. If so, render the display name of the chosen sprite to the template.
                if (command_data.Base_Sprite != 0)
                {
                    graphics.DrawImage(Text_To_Red(Render_Name(bustup_data)), 0, 0, template_width, template_height);
                }
                // If the base sprite number IS zero, we need a sprite to actually retrieve a display name from.
                else
                {
                    // Change the base sprite number from the command data to one.
                    // This way, we can get the bustup data for the first sprite to retrieve its display name.
                    command_data.Base_Sprite = 1;

                    // Get the bustup data for the first sprite and render the display name to the template.
                    bustup_data = BustupDataMethods.Get_Bustup_Data(account, set_data, command_data);
                    graphics.DrawImage(Text_To_Red(Render_Name(bustup_data)), 0, 0, template_width, template_height);
                }

                // Draw the input dialogue to the template.
                graphics.DrawImage(Text_To_Gray(Render_Dialogue(Line_Parser(message, command_data.Dialogue))), 0, 0, template_width, template_height);
            }

            // Save the entire base template to a data stream.
            MemoryStream memoryStream = new MemoryStream();
            base_template.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
            memoryStream.Seek(0, SeekOrigin.Begin);

            // Send the image.
            await message.Channel.SendFileAsync(memoryStream, $"scene_{message.Author.Id}_{DateTime.UtcNow}.png");

            // Delete the loading message.
            await loader.DeleteAsync();

            // If the user has auto-delete for their commands set to on, delete their command as well.
            if (account.Auto_Delete_Commands == "On")
            {
                await message.DeleteAsync();
            }
        }

        public static async Task Render_System_Message(SocketMessage message, MakerCommandData command_data)
        {
            // Create variables to store the width and height of the template.
            int template_width = 640;
            int template_height = 448;

            // Before any rendering occurs, amend the dialogue so that an arrow is placed in front of it.
            command_data.Dialogue = $"> {command_data.Dialogue}";

            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = message.Author;
            SocketTextChannel channel = (SocketTextChannel)message.Channel;

            // Send a loading message to the channel while the sprite sheet is being made.
            RestUserMessage loader = await channel.SendMessageAsync("", false, P3F_Loading_Message().Build());

            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

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

            // Time to put it all together!
            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                try
                {
                    // Create and assign bitmap variables for the assets needed.
                    Bitmap message_window = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3F//Main//message_window.png");
                    Bitmap cursor = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3F//Main//cursor.png");

                    // Draw the layer with the user's colored default background if it exists.
                    graphics.DrawImage(colored_background_bitmap, 0, 0, template_width, template_height);

                    // Draw the user's background to the base template.
                    graphics.DrawImage(background, 0, 0, template_width, template_height);

                    // Draw the message window layer to the base template.
                    graphics.DrawImage(message_window, 0, 0, template_width, template_height);

                    // Draw the cursor layer to the base template.
                    graphics.DrawImage(cursor, 0, 0, template_width, template_height);

                    // If the user has the HUD enabled, render it to the template as well.
                    if (account.P3F_TS_HUD != "None")
                    {
                        graphics.DrawImage(Render_Calendar_HUD(account), 0, 0, template_width, template_height);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            // The user could choose to output the image at different resolutions, so let's handle that point now.
            // This is done before the text is rendered to the template.
            // If the user's output setting is at the default resolution, do nothing.
            if (account.P3F_Resolution == "640 × 448")
            {
                // Do nothing
            }
            // If the user's output setting is NOT at the default resolution, however, we need to do some work.
            else
            {
                // Change the template width and height variables based on the user's output settings.
                if (account.P3F_Resolution == "640 × 480")
                {
                    template_width = 640;
                    template_height = 480;
                }
                else if (account.P3F_Resolution == "1440 × 1080")
                {
                    template_width = 1440;
                    template_height = 1080;
                }

                // Now, we'll want to make a new bitmap that matches these sizes.
                // Create a copy of the template so far.
                var image = new Bitmap(base_template);

                // Create a new empty bitmap with the adjusted dimensions.
                var scaled_bitmap = new Bitmap(template_width, template_height);

                // Create a new graphics object so we can render on the empty bitmap.
                using (Graphics graphics = Graphics.FromImage(scaled_bitmap))
                {
                    // If the user's setting is at Full HD, set the scaling method to their choice of Bicubic and Nearest Neighbor.
                    if (account.P3F_Resolution == "1440 × 1080")
                    {
                        switch (account.P3F_Scale)
                        {
                            case "Bicubic":
                                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                                break;

                            case "Nearest Neighbor":
                                graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                                break;
                        }
                    }
                    // Otherwise, set the method to Bicubic.
                    else
                    {
                        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    }

                    // Set the rendering quality to high.
                    graphics.CompositingQuality = CompositingQuality.HighQuality;

                    // Draw the copy of the template to the empty bitmap while fitting to size.
                    graphics.DrawImage(image, 0, 0, template_width, template_height);
                }

                // Copy the contents of the new bitmap to the base template variable.
                base_template = scaled_bitmap;
            }

            // Create another graphics object for the base template.
            // We'll start rendering our needed text here.
            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                // Draw the input dialogue to the template.
                graphics.DrawImage(Text_To_Gray(Render_Dialogue(Line_Parser(message, command_data.Dialogue))), 0, 0, template_width, template_height);
            }

            // Save the entire base template to a data stream.
            MemoryStream memoryStream = new MemoryStream();
            base_template.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
            memoryStream.Seek(0, SeekOrigin.Begin);

            // Send the image.
            await message.Channel.SendFileAsync(memoryStream, $"scene_{message.Author.Id}_{DateTime.UtcNow}.png");

            // Delete the loading message.
            await loader.DeleteAsync();

            // If the user has auto-delete for their commands set to on, delete their command as well.
            if (account.Auto_Delete_Commands == "On")
            {
                await message.DeleteAsync();
            }
        }

        public static Bitmap Render_Name(BustupData bustup_data)
        {
            // Create a 640 x 480 bitmap.
            // This is larger than the template's defauly 640 x 448 size, but P3F's font must be rendered with this 640 x 480 dimension in mind.
            Bitmap base_template = new Bitmap(640, 480);

            // Establish an int for the width and height glyphs should be rendered at.
            int multiplier = 32;

            // Load the bitmap font.
            string font_sheet = $@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3F//Font//p3f_font_sheet.png";

            // Create a variable to iterate through sections of the bitmap font.
            Bitmap current_glyph;

            // Specify X and Y coordinates for where the glyphs should start rendering on the template.
            int render_position_x = 42;
            int render_position_y = 354;

            // Take the sprite's display name and convert it into a char array.
            char[] char_array = bustup_data.Default_Name_EN.ToCharArray();

            // Iterate through each character of the array.
            for (int i = 0; i < char_array.Length; i++)
            {
                // Retrieve glyph information from the JSON file.
                var glyph = ParsingMethods.Get_P3F_Glyph(char_array[i]);

                // Check if the character is a line break.
                if (char_array[i] == '\u000a')
                {
                    // Set the X coordinate to the start of the row.
                    render_position_x = 42;
                    // Move the Y coordinate down to the next line.
                    render_position_y += 22;
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

                            // Draw the glyph to the base bitmap. Some hanging letters will need to be drawn a bit lower to appear natural.
                            if (char_array[i] == 'g' || char_array[i] == 'j' || char_array[i] == 'p' || char_array[i] == 'q' || char_array[i] == 'y')
                            {
                                graphics.DrawImage(current_glyph, (render_position_x - glyph.LeftCut), render_position_y + 2, multiplier, multiplier);
                            }
                            else
                            {
                                graphics.DrawImage(current_glyph, (render_position_x - glyph.LeftCut), render_position_y, multiplier, multiplier);
                            }
                        }
                    }

                    // Set the next X value at the end of the current glyph's right width.
                    render_position_x += (glyph.RightCut - glyph.LeftCut);

                    // Check if the current iterated index is less than the number of indicies available.
                    if (i < char_array.Length - 1)
                    {
                        // If so, edit the position of the X coordinate according to specific kerning pairs.
                        if (char_array[i] == 'Y' && Char.IsLower(char_array[i + 1]))
                        {
                            render_position_x += -2;
                        }
                        else if (char_array[i] == 'T' && Char.IsLower(char_array[i + 1]) && char_array[i + 1] != 'h')
                        {
                            render_position_x += -2;
                        }
                    }
                }
            }

            return base_template;
        }

        public static Bitmap Render_Dialogue(List<string>[] dialogue_lines)
        {
            // Create a 640 x 480 bitmap.
            // This is larger than the template's defauly 640 x 448 size, but P3F's font must be rendered with this 640 x 480 dimension in mind.
            Bitmap bitmap = new Bitmap(640, 480);

            // Create an int to keep track of rendering errors. This is neccessary to inform the user of any potential issues.
            int error_counter = 0;

            //Establish an int for the width and height glyphs should be rendered at
            int multiplier = 32;

            string font_sheet = $@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3F//Font//p3f_font_sheet.png";
            Bitmap current_glyph;

            // Iterate over each line of the dialogue string list with a loop.
            for (int i = 0; i < dialogue_lines.Length; i++)
            {
                // Specify X and Y coordinates for where the glyphs should start rendering on the template.
                int render_position_x = 55;
                int render_position_y = 380 + (22 * i);

                // Take the input dialogue and convert it into a char array.
                char[] char_array = String_List_To_String(dialogue_lines[i]).ToCharArray();

                // Iterate through each character of the array.
                for (int j = 0; j < char_array.Length; j++)
                {
                    //Retrieve glyph information from the JSON file
                    var glyph = ParsingMethods.Get_P3F_Glyph(char_array[j]);

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

                                // Draw the glyph to the base bitmap. Some hanging letters will need to be drawn a bit lower to appear natural.
                                if (char_array[j] == 'g' || char_array[j] == 'j' || char_array[j] == 'p' || char_array[j] == 'q' || char_array[j] == 'y')
                                {
                                    graphics.DrawImage(current_glyph, (render_position_x - glyph.LeftCut), render_position_y + 2, multiplier, multiplier);
                                }
                                else
                                {
                                    graphics.DrawImage(current_glyph, (render_position_x - glyph.LeftCut), render_position_y, multiplier, multiplier);
                                }
                            }
                        }

                        // Set the next X value at the end of the current glyph's right width.
                        render_position_x += (glyph.RightCut - glyph.LeftCut);

                        // Check if the current iterated index is less than the number of indicies available.
                        if (j < char_array.Length - 1)
                        {
                            // If so, edit the position of the X coordinate according to specific kerning pairs.
                            if (char_array[j] == 'Y' && Char.IsLower(char_array[j + 1]))
                            {
                                render_position_x += -2;
                            }
                            else if (char_array[j] == 'T' && Char.IsLower(char_array[j + 1]) && char_array[j + 1] != 'h')
                            {
                                render_position_x += -3;
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
            int max_line_length = 510;

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
                    int completed_word_length = Measure_Word_Pixel_Length(completed_word);

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
                            substring_length = Measure_Word_Pixel_Length(substring);

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

        public static int Measure_Word_Pixel_Length(string input_word)
        {
            // Create an int variable to keep track of the pixel length of a word.
            int pixel_counter = 0;

            // Take the input string and convert it into a char array.
            char[] char_array = input_word.ToCharArray();

            // Here, we'll process the char array by iterating through each index.
            for (int i = 0; i < char_array.Length; i++)
            {
                // Retrieve glyph information from the P3F JSON file.
                var glyph = ParsingMethods.Get_P3F_Glyph(char_array[i]);

                // Confirm that the glyph taken in is catologued in the JSON. If not, the character is unsupported.
                if (glyph != null)
                {
                    // Check if the character is a line break. Strings with line breaks shouldn't make it to this method, but this is a failsafe just in case.
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
                                pixel_counter += -2;
                            }
                            else if (char_array[i] == 'T' && Char.IsLower(char_array[i + 1]) && char_array[i + 1] != 'h')
                            {
                                pixel_counter += -3;
                            }
                        }

                        // Set the pixel counter to the appropriate width of the string so far.
                        pixel_counter += glyph.RightCut - glyph.LeftCut;
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

        public static Bitmap Render_Calendar_HUD(UserInfoFields account)
        {
            // Create variables to store the width and height of the template.
            int template_width = 640;
            int template_height = 448;

            // Get the user's current time and store it in a variable.
            DateTime user_time = Get_Date(account);

            // Create a new bitmap with the width and height variables.
            Bitmap base_template = new Bitmap(template_width, template_height);

            // Now, let's use a graphics object to draw to the base template.
            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                // Establish all bitmap variables needed. Ones needed for the date and time of day will be initialized as new bitmaps and reassigned to later.
                Bitmap hud_top = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3F//Main//hud_1.png");
                Bitmap hud_bottom = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3F//Main//hud_2.png");

                Bitmap month_tens = new Bitmap(template_width, template_height);
                Bitmap month_ones = new Bitmap(template_width, template_height);

                Bitmap day_tens = new Bitmap(template_width, template_height);
                Bitmap day_ones = new Bitmap(template_width, template_height);

                Bitmap day_of_week = new Bitmap(template_width, template_height);
                Bitmap time_of_day = new Bitmap(template_width, template_height);
                Bitmap date_slash = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3F//Main//Calendar//slash.png");

                Bitmap moon_phase_text = new Bitmap(template_width, template_height);
                Bitmap moon_phase_digit_tens = new Bitmap(template_width, template_height);
                Bitmap moon_phase_digit_ones = new Bitmap(template_width, template_height);
                Bitmap moon_phase_image_normal = new Bitmap(template_width, template_height);
                Bitmap moon_phase_image_glow = new Bitmap(template_width, template_height);

                // Get the user's current month and convert it to a char array.
                char[] month = user_time.ToString("MM").ToCharArray();

                // If the month is not a single digit, get the appropriate bitmap for the tens place of the month.
                if (month[0] != '0')
                {
                    month_tens = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3F//Main//Calendar//Month//Tens_Place//{month[0]}.png");
                }

                // Regardless, get the appropriate bitmap for the ones place of the month.
                month_ones = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3F//Main//Calendar//Month//Ones_Place//{month[1]}.png");

                // Get the user's current day and convert it to a char array.
                char[] day = user_time.ToString("dd").ToCharArray();

                // If the day is not a single digit, get the appropriate bitmap for the tens place of the day.
                if (day[0] != '0')
                {
                    day_tens = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3F//Main//Calendar//Day//Tens_Place//{day[0]}.png");
                }

                // Regardless, get the appropriate bitmap for the ones place of the day.
                day_ones = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3F//Main//Calendar//Day//Ones_Place//{day[1]}.png");

                // Get the appropriate bitmaps for the weekday and time of day for the user.
                day_of_week = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3F//Main//Calendar//Weekday//{user_time.ToString("dddd").ToLower()}.png");
                time_of_day = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3F//Main//Calendar//Time_of_Day//{Get_Time_of_Day(user_time)}.png");

                // Color the assets depending on whether the time is the "Dark Hour" for the user or not.
                if (Get_Time_of_Day(user_time) == "dark_hour")
                {
                    hud_top = HUD_To_Green(hud_top);
                    month_tens = Date_To_Dark_Green(month_tens);
                    month_ones = Date_To_Dark_Green(month_ones);
                    date_slash = Date_To_Dark_Green(date_slash);
                    day_tens = Date_To_Dark_Green(day_tens);
                    day_ones = Date_To_Dark_Green(day_ones);
                }
                else
                {
                    hud_top = HUD_To_Blue(hud_top);
                    month_tens = Date_To_Dark_Blue(month_tens);
                    month_ones = Date_To_Dark_Blue(month_ones);
                    date_slash = Date_To_Dark_Blue(date_slash);
                    day_tens = Date_To_Dark_Blue(day_tens);
                    day_ones = Date_To_Dark_Blue(day_ones);
                }

                // Color the day of week bitmap depending on what day it currently is.
                if (Holiday_Check(user_time) == true)
                {
                    day_of_week = Day_Of_Week_To_Off_Day_Color_Scheme(day_of_week);
                }
                if (user_time.ToString("dddd").ToLower() == "saturday")
                {
                    day_of_week = Day_Of_Week_To_Saturday_Color_Scheme(day_of_week);
                }
                else if (user_time.ToString("dddd").ToLower() == "sunday")
                {
                    day_of_week = Day_Of_Week_To_Off_Day_Color_Scheme(day_of_week);
                }
                else 
                {
                    day_of_week = Day_Of_Week_To_Weekday_Color_Scheme(day_of_week);
                }

                // Draw all the assets to the template.
                graphics.DrawImage(hud_top, 0, 0, template_width, template_height);
                graphics.DrawImage(hud_bottom, 0, 0, template_width, template_height);

                graphics.DrawImage(month_tens, 0, 0, template_width, template_height);
                graphics.DrawImage(month_ones, 0, 0, template_width, template_height);
                graphics.DrawImage(date_slash, 0, 0, template_width, template_height);
                graphics.DrawImage(day_tens, 0, 0, template_width, template_height);
                graphics.DrawImage(day_ones, 0, 0, template_width, template_height);

                graphics.DrawImage(day_of_week, 0, 0, template_width, template_height);
                graphics.DrawImage(time_of_day, 0, 0, template_width, template_height);

                // Lastly, render the moon HUD to the template as well.
                graphics.DrawImage(Render_Moon_HUD(account), 0, 0, template_width, template_height);
            }

            return base_template;
        }

        public static Bitmap Render_Moon_HUD(UserInfoFields account)
        {
            // Create variables to store the width and height of the template.
            int template_width = 640;
            int template_height = 448;

            // Create new bitmap variables for the assets we'll need throughout the method.
            // We'll assign them proper values soon depending on the moon phase.
            // For the countdown text, create and initialize two. One will be a mainstay while the other only appears during new and half moons.
            Bitmap countdown_text_main = new Bitmap(template_width, template_height);
            Bitmap countdown_text_special = new Bitmap(template_width, template_height);

            Bitmap countdown_tens = new Bitmap(template_width, template_height);
            Bitmap countdown_ones = new Bitmap(template_width, template_height);

            Bitmap moon_background = new Bitmap(template_width, template_height);
            Bitmap moon_phase = new Bitmap(template_width, template_height);
            Bitmap moon_phase_glow = new Bitmap(template_width, template_height);

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

            // Using that age, determine how many days are left until the next full moon.
            int full_moon_countdown = Get_Full_Moon_Countdown(cycle_age);

            // Store the moon's illumination percentage in a double. We'll use this to determine what phase it's currently in alongside using the age.
            double illumination = Math.Round(result.Visibility, 2);

            // Create a new bitmap with the width and height variables.
            Bitmap base_template = new Bitmap(template_width, template_height);

            // Now, let's use a graphics object to draw to the base template.
            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                // Convert the full moon countdown value to a two-index char array.
                char[] countdown_array = full_moon_countdown.ToString("00").ToCharArray();

                // Check if the first index is not a zero. If it is, the countdown digit is a single number and we can ignore the tens place.
                // Else, we need to assign a proper value to the tens place bitmap variable.
                if (countdown_array[0] != '0')
                {
                    countdown_tens = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3F//Main//Moon//Countdown//Digits//Tens_Place//{countdown_array[0]}.png");
                }
                // There will always be a digit in the ones place unless the moon is full, so assign a proper value here too.
                countdown_ones = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3F//Main//Moon//Countdown//Digits//Ones_Place//{countdown_array[1]}.png");

                // Assign the countdown text a default value. This could change depending on endpoint phases (new and half), but for the most part, it will remain on "Next".
                countdown_text_main = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3F//Main//Moon//Countdown//Text//next.png");

                // Displayed moon phases have a dark background, so assign that value here.
                moon_background = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3F//Main//Moon//Phases//moon_background.png");

                // Here is where the calculation on which moon phase to display begins.
                // The cycle begins with a new moon, so we'll use the current cycle's age and divide it into two halfs to determine whether it's waxing or waning.
                // Waxing phases
                if (cycle_age <= 14.76)
                {
                    // New moon
                    if ((illumination >= 0) && (illumination < 12.5))
                    {
                        moon_background = new Bitmap(template_width, template_height);
                        moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3F//Main//Moon//Phases//1_new.png");
                        countdown_text_special = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3F//Main//Moon//Countdown//Text//new.png");
                    }
                    // Waxing crescent 1
                    else if ((illumination >= 12.5) && (illumination < 25))
                    {
                        moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3F//Main//Moon//Phases//2_waxing_crescent.png");
                        moon_phase_glow = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3F//Main//Moon//Phases//Glow//2_waxing_crescent.png");
                    }
                    // Waxing crescent 2
                    else if ((illumination >= 25) && (illumination < 37.5))
                    {
                        moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3F//Main//Moon//Phases//3_waxing_crescent.png");
                        moon_phase_glow = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3F//Main//Moon//Phases//Glow//3_waxing_crescent.png");
                    }
                    // Waxing crescent 3
                    else if ((illumination >= 37.5) && (illumination < 50))
                    {
                        moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3F//Main//Moon//Phases//4_waxing_crescent.png");
                        moon_phase_glow = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3F//Main//Moon//Phases//Glow//4_waxing_crescent.png");
                    }
                    // Waxing half
                    else if ((illumination >= 50) && (illumination < 62.5))
                    {
                        moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3F//Main//Moon//Phases//5_waxing_half.png");
                        moon_phase_glow = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3F//Main//Moon//Phases//Glow//5_waxing_half.png");
                        countdown_text_special = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3F//Main//Moon//Countdown//Text//half.png");
                    }
                    // Waxing gibbous 1
                    else if ((illumination >= 62.5) && (illumination < 75))
                    {
                        moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3F//Main//Moon//Phases//6_waxing_gibbous.png");
                        moon_phase_glow = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3F//Main//Moon//Phases//Glow//6_waxing_gibbous.png");
                    }
                    // Waxing gibbous 2
                    else if ((illumination >= 75) && (illumination < 87.5))
                    {
                        moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3F//Main//Moon//Phases//7_waxing_gibbous.png");
                        moon_phase_glow = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3F//Main//Moon//Phases//Glow//7_waxing_gibbous.png");
                    }
                    // Waxing gibbous 3
                    else if ((illumination >= 87.5) && (illumination < 100))
                    {
                        moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3F//Main//Moon//Phases//8_waxing_gibbous.png");
                        moon_phase_glow = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3F//Main//Moon//Phases//Glow//8_waxing_gibbous.png");
                    }
                    // Full moon
                    else if (illumination == 100)
                    {
                        countdown_tens = new Bitmap(template_width, template_height);
                        countdown_ones = new Bitmap(template_width, template_height);
                        moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3F//Main//Moon//Phases//9_full.png");
                        moon_phase_glow = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3F//Main//Moon//Phases//Glow//9_full.png");
                        countdown_text_special = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3F//Main//Moon//Countdown//Text//full.png");
                    }
                }
                // Waning phases
                else if (cycle_age > 14.76)
                {
                    // Full moon
                    if (illumination == 100)
                    {
                        countdown_tens = new Bitmap(template_width, template_height);
                        countdown_ones = new Bitmap(template_width, template_height);
                        moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3F//Main//Moon//Phases//9_full.png");
                        moon_phase_glow = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3F//Main//Moon//Phases//Glow//9_full.png");
                        countdown_text_special = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3F//Main//Moon//Countdown//Text//full.png");
                    }
                    // Waning gibbous 1
                    else if ((illumination >= 87.5) && (illumination < 100))
                    {
                        moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3F//Main//Moon//Phases//10_waning_gibbous.png");
                    }
                    // Waning gibbous 2
                    else if ((illumination >= 75) && (illumination < 87.5))
                    {
                        moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3F//Main//Moon//Phases//11_waning_gibbous.png");
                    }
                    // Waning gibbous 3
                    else if ((illumination >= 62.5) && (illumination < 75))
                    {
                        moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3F//Main//Moon//Phases//12_waning_gibbous.png");
                    }
                    // Waning half
                    else if ((illumination >= 50) && (illumination < 62.5))
                    {
                        moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3F//Main//Moon//Phases//13_waning_half.png");
                        countdown_text_special = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3F//Main//Moon//Countdown//Text//half.png");
                    }
                    // Waning crescent 1
                    else if ((illumination >= 37.5) && (illumination < 50))
                    {
                        moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3F//Main//Moon//Phases//14_waning_crescent.png");
                    }
                    // Waning crescent 2
                    else if ((illumination >= 25) && (illumination < 37.5))
                    {
                        moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3F//Main//Moon//Phases//15_waning_crescent.png");
                    }
                    // Waning crescent 3
                    else if ((illumination >= 12.5) && (illumination < 25))
                    {
                        moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3F//Main//Moon//Phases//16_waning_crescent.png");
                    }
                    // New moon
                    else if ((illumination >= 0) && (illumination < 12.5))
                    {
                        moon_background = new Bitmap(template_width, template_height);
                        moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3F//Main//Moon//Phases//1_new.png");
                        countdown_text_special = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3F//Main//Moon//Countdown//Text//new.png");
                    }
                }
                
                // Depending on the time of day, color the HUD either blue or green.
                if (Get_Time_of_Day(Get_Date(account)) == "dark_hour")
                {
                    countdown_tens = HUD_To_Green(countdown_tens);
                    countdown_ones = HUD_To_Green(countdown_ones);
                }
                else
                {
                    countdown_tens = HUD_To_Blue(countdown_tens);
                    countdown_ones = HUD_To_Blue(countdown_ones);
                }

                // Color the countdown text to white.
                countdown_text_main = Countdown_Text_To_White(countdown_text_main);
                countdown_text_special = Countdown_Text_To_White(countdown_text_special);

                // Change the opacity of countdown assets.
                float opacity = (float)0.8;
                countdown_tens = (Bitmap)SetImageOpacity(countdown_tens, opacity);
                countdown_ones = (Bitmap)SetImageOpacity(countdown_ones, opacity);
                countdown_text_main = (Bitmap)SetImageOpacity(countdown_text_main, opacity);
                countdown_text_special = (Bitmap)SetImageOpacity(countdown_text_special, opacity);

                // Lastly, we'll want to adjust the glow of waxing moon phases if an asset has been assigned to it.
                // Create a random variable.
                Random rnd = new Random();

                // Increase the brightness and contrast of the glowing bitmap.
                moon_phase_glow = Increase_Brightness_Contrast(moon_phase_glow);

                // Use the random variable to randomize the opacity of the glow.
                moon_phase_glow = (Bitmap)SetImageOpacity(moon_phase_glow, (float)rnd.NextDouble());

                // Draw all the assets to the template.
                // The main countdown text is drawn in a different position depending on the countdown's value. 
                if (full_moon_countdown < 10)
                {
                    graphics.DrawImage(countdown_text_main, 17, 0, template_width, template_height);
                }
                else
                {
                    graphics.DrawImage(countdown_text_main, 0, 0, template_width, template_height);
                }

                graphics.DrawImage(countdown_text_special, 0, 0, template_width, template_height);
                graphics.DrawImage(countdown_tens, 0, 0, template_width, template_height);
                graphics.DrawImage(countdown_ones, 0, 0, template_width, template_height);
                graphics.DrawImage(moon_background, 0, 0, template_width, template_height);
                graphics.DrawImage(moon_phase, 0, 0, template_width, template_height);
                graphics.DrawImage(moon_phase_glow, 0, 0, template_width, template_height);
            }

            return base_template;
        }

        public static Bitmap HUD_To_Blue(Bitmap input_bitmap)
        {
            // Create a color variable. We'll use this to iterate through the input bitmap and store each pixel's color values here.
            System.Drawing.Color actual_color;

            // Make an empty bitmap the same size as the input bitmap.
            Bitmap new_bitmap = new Bitmap(input_bitmap.Width, input_bitmap.Height);

            // Create a for loop to iterate over the X values of the bitmap to be changed.
            for (int i = 393; i < 640; i++)
            {
                // Create a nested for loop to iterate over the Y values of the bitmap to be changed.
                for (int j = 0; j < 150; j++)
                {
                    // Get the current pixel from the input bitmap.
                    actual_color = input_bitmap.GetPixel(i, j);

                    // Color in the pixel with the new color while keeping its current alpha value.
                    System.Drawing.Color new_color = System.Drawing.Color.FromArgb(actual_color.A, 74, 152, 255);
                    new_bitmap.SetPixel(i, j, new_color);
                }
            }

            return new_bitmap;
        }

        public static Bitmap HUD_To_Green(Bitmap input_bitmap)
        {
            // Create a color variable. We'll use this to iterate through the input bitmap and store each pixel's color values here.
            System.Drawing.Color actual_color;

            // Make an empty bitmap the same size as the input bitmap.
            Bitmap new_bitmap = new Bitmap(input_bitmap.Width, input_bitmap.Height);

            // Create a for loop to iterate over the X values of the bitmap to be changed.
            for (int i = 393; i < 640; i++)
            {
                // Create a nested for loop to iterate over the Y values of the bitmap to be changed.
                for (int j = 0; j < 150; j++)
                {
                    // Get the current pixel from the input bitmap.
                    actual_color = input_bitmap.GetPixel(i, j);

                    // Color in the pixel with the new color while keeping its current alpha value.
                    System.Drawing.Color new_color = System.Drawing.Color.FromArgb(actual_color.A, 121, 254, 141);
                    new_bitmap.SetPixel(i, j, new_color);
                }
            }

            return new_bitmap;
        }

        public static Bitmap Date_To_Dark_Blue(Bitmap input_bitmap) // Date
        {
            // Create a color variable. We'll use this to iterate through the input bitmap and store each pixel's color values here.
            System.Drawing.Color actual_color;

            // Make an empty bitmap the same size as the input bitmap.
            Bitmap new_bitmap = new Bitmap(input_bitmap.Width, input_bitmap.Height);

            // Create a for loop to iterate over the X values of the bitmap to be changed.
            for (int i = 393; i < 640; i++)
            {
                // Create a nested for loop to iterate over the Y values of the bitmap to be changed.
                for (int j = 0; j < 72; j++)
                {
                    // Get the current pixel from the input bitmap.
                    actual_color = input_bitmap.GetPixel(i, j);

                    // Color in the pixel with the new color while keeping its current alpha value.
                    System.Drawing.Color new_color = System.Drawing.Color.FromArgb(actual_color.A, 13, 33, 37);
                    new_bitmap.SetPixel(i, j, new_color);
                }
            }

            return new_bitmap;
        }

        public static Bitmap Date_To_Dark_Green(Bitmap input_bitmap) // Date (Dark Hour)
        {
            // Create a color variable. We'll use this to iterate through the input bitmap and store each pixel's color values here.
            System.Drawing.Color actual_color;

            // Make an empty bitmap the same size as the input bitmap.
            Bitmap new_bitmap = new Bitmap(input_bitmap.Width, input_bitmap.Height);

            // Create a for loop to iterate over the X values of the bitmap to be changed.
            for (int i = 393; i < 640; i++)
            {
                // Create a nested for loop to iterate over the Y values of the bitmap to be changed.
                for (int j = 0; j < 72; j++)
                {
                    // Get the current pixel from the input bitmap.
                    actual_color = input_bitmap.GetPixel(i, j);

                    // Color in the pixel with the new color while keeping its current alpha value.
                    System.Drawing.Color new_color = System.Drawing.Color.FromArgb(actual_color.A, 15, 42, 18);
                    new_bitmap.SetPixel(i, j, new_color);
                }
            }

            return new_bitmap;
        }

        public static Bitmap Day_Of_Week_To_Weekday_Color_Scheme(Bitmap input_bitmap) // Weekdays
        {
            // Create a color variable. We'll use this to iterate through the input bitmap and store each pixel's color values here.
            System.Drawing.Color actual_color;

            // Make an empty bitmap the same size as the input bitmap.
            Bitmap new_bitmap = new Bitmap(input_bitmap.Width, input_bitmap.Height);

            // Create a for loop to iterate over the X values of the bitmap to be changed.
            for (int i = 393; i < 640; i++)
            {
                // Create a nested for loop to iterate over the Y values of the bitmap to be changed.
                for (int j = 0; j < 72; j++)
                {
                    // Get the current pixel from the input bitmap.
                    actual_color = input_bitmap.GetPixel(i, j);

                    // Color in the pixel with the new color while keeping its current alpha value.
                    System.Drawing.Color new_color = System.Drawing.Color.FromArgb(actual_color.A, 53, 74, 94);
                    new_bitmap.SetPixel(i, j, new_color);
                }
            }

            return new_bitmap;
        }

        public static Bitmap Day_Of_Week_To_Saturday_Color_Scheme(Bitmap input_bitmap) // Saturdays
        {
            // Create a color variable. We'll use this to iterate through the input bitmap and store each pixel's color values here.
            System.Drawing.Color actual_color;

            // Make an empty bitmap the same size as the input bitmap.
            Bitmap new_bitmap = new Bitmap(input_bitmap.Width, input_bitmap.Height);

            // Create a for loop to iterate over the X values of the bitmap to be changed.
            for (int i = 393; i < 640; i++)
            {
                // Create a nested for loop to iterate over the Y values of the bitmap to be changed.
                for (int j = 0; j < 72; j++)
                {
                    // Get the current pixel from the input bitmap.
                    actual_color = input_bitmap.GetPixel(i, j);

                    // Check for pixels consistent with the color of the dot. 
                    if (actual_color.R == 49 && actual_color.G == 84 && actual_color.B == 102)
                    {
                        // Color in the pixel with the new color while keeping its current alpha value.
                        System.Drawing.Color new_color = System.Drawing.Color.FromArgb(actual_color.A, 12, 43, 78);
                        new_bitmap.SetPixel(i, j, new_color);
                    }
                    // Check for pixels consistent with the white color of the letters.
                    else if (actual_color.R == 255 && actual_color.G == 255 && actual_color.B == 255)
                    {
                        // Color in the pixel with the new color while keeping its current alpha value.
                        System.Drawing.Color new_color = System.Drawing.Color.FromArgb(actual_color.A, 15, 86, 142);
                        new_bitmap.SetPixel(i, j, new_color);
                    }
                    // Otherwise, copy the same pixel over to the new bitmap.
                    else
                    {
                        new_bitmap.SetPixel(i, j, actual_color);
                    }
                }
            }

            return new_bitmap;
        }

        public static Bitmap Day_Of_Week_To_Off_Day_Color_Scheme(Bitmap input_bitmap) // Sundays and Holidays
        {
            // Create a color variable. We'll use this to iterate through the input bitmap and store each pixel's color values here.
            System.Drawing.Color actual_color;

            // Make an empty bitmap the same size as the input bitmap.
            Bitmap new_bitmap = new Bitmap(input_bitmap.Width, input_bitmap.Height);

            // Create a for loop to iterate over the X values of the bitmap to be changed.
            for (int i = 393; i < 640; i++)
            {
                // Create a nested for loop to iterate over the Y values of the bitmap to be changed.
                for (int j = 0; j < 72; j++)
                {
                    // Get the current pixel from the input bitmap.
                    actual_color = input_bitmap.GetPixel(i, j);

                    // Check for pixels consistent with the color of the dot. 
                    if (actual_color.R == 49 && actual_color.G == 84 && actual_color.B == 102)
                    {
                        // Color in the pixel with the new color while keeping its current alpha value.
                        System.Drawing.Color new_color = System.Drawing.Color.FromArgb(actual_color.A, 27, 34, 64);
                        new_bitmap.SetPixel(i, j, new_color);
                    }
                    // Check for pixels consistent with the white color of the letters.
                    else if (actual_color.R == 255 && actual_color.G == 255 && actual_color.B == 255)
                    {
                        // Color in the pixel with the new color while keeping its current alpha value.
                        System.Drawing.Color new_color = System.Drawing.Color.FromArgb(actual_color.A, 90, 57, 105);
                        new_bitmap.SetPixel(i, j, new_color);
                    }
                    // Otherwise, copy the same pixel over to the new bitmap.
                    else
                    {
                        new_bitmap.SetPixel(i, j, actual_color);
                    }
                }
            }

            return new_bitmap;
        }

        public static Bitmap Countdown_Text_To_White(Bitmap input_bitmap) // Phase descriptors
        {
            // Create a color variable. We'll use this to iterate through the input bitmap and store each pixel's color values here.
            System.Drawing.Color actual_color;

            // Make an empty bitmap the same size as the input bitmap.
            Bitmap new_bitmap = new Bitmap(input_bitmap.Width, input_bitmap.Height);

            // Create a for loop to iterate over the X values of the bitmap to be changed.
            for (int i = 393; i < 640; i++)
            {
                // Create a nested for loop to iterate over the Y values of the bitmap to be changed.
                for (int j = 0; j < 150; j++)
                {
                    // Get the current pixel from the input bitmap.
                    actual_color = input_bitmap.GetPixel(i, j);

                    // Color in the pixel with the new color while keeping its current alpha value.
                    System.Drawing.Color new_color = System.Drawing.Color.FromArgb(actual_color.A, 255, 255, 255);
                    new_bitmap.SetPixel(i, j, new_color);
                }
            }

            return new_bitmap;
        }

        public static Bitmap Text_To_Red(Bitmap input_bitmap) // Display names
        {
            // Establish the width and height of the template you want to render to.
            // In the case of P3 FES, font is made to be rendered on a 640 x 480 bitmap.
            int template_width = 640;
            int template_height = 480;

            // Create a color variable. We'll use this to iterate through the input bitmap and store each pixel's color values here.
            System.Drawing.Color actual_color;

            // Make an empty bitmap the same size as the input bitmap.
            Bitmap new_bitmap = new Bitmap(input_bitmap.Width, input_bitmap.Height);

            // Create a for loop to iterate over the X values of the bitmap to be changed.
            for (int x = 0; x < template_width; x++)
            {
                // Create a nested for loop to iterate over the Y values of the bitmap to be changed.
                for (int y = 0; y < template_height; y++)
                {
                    // Get the current pixel from the input bitmap.
                    actual_color = input_bitmap.GetPixel(x, y);

                    // Color in the pixel with the new color while keeping its current alpha value.
                    System.Drawing.Color new_color = System.Drawing.Color.FromArgb(actual_color.A, 88, 24, 29);
                    new_bitmap.SetPixel(x, y, new_color);
                }
            }

            return new_bitmap;
        }

        public static Bitmap Text_To_Gray(Bitmap input_bitmap) // Dialogue
        {
            // Establish the width and height of the template you want to render to.
            // In the case of P3 FES, font is made to be rendered on a 640 x 480 bitmap.
            int template_width = 640;
            int template_height = 480;

            // Create a color variable. We'll use this to iterate through the input bitmap and store each pixel's color values here.
            System.Drawing.Color actual_color;

            // Make an empty bitmap the same size as the input bitmap.
            Bitmap new_bitmap = new Bitmap(input_bitmap.Width, input_bitmap.Height);

            // Create a for loop to iterate over the X values of the bitmap to be changed.
            for (int x = 0; x < template_width; x++)
            {
                // Create a nested for loop to iterate over the Y values of the bitmap to be changed.
                for (int y = 0; y < template_height; y++)
                {
                    // Get the current pixel from the input bitmap.
                    actual_color = input_bitmap.GetPixel(x, y);

                    // Color in the pixel with the new color while keeping its current alpha value.
                    System.Drawing.Color new_color = System.Drawing.Color.FromArgb(actual_color.A, 82, 83, 93);
                    new_bitmap.SetPixel(x, y, new_color);
                }
            }

            return new_bitmap;
        }

        public static DateTime Get_Date(UserInfoFields account)
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

                // Read the localtime variable of the data object.
                DateTime user_time = dataObject.location.localtime;

                // Return the localtime variable.
                return user_time;
            }
            // If an exception is thrown, return a default value.
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return DateTime.UtcNow;
            }
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

        public static string Get_Time_of_Day(DateTime input_time)
        {
            // Get the current hour of the user.
            TimeSpan current_hour = input_time.TimeOfDay;

            // Create an empty string variable to store the returned time of day value later on.
            string time_of_day = "";

            // Lastly, establish the starting times for each time of day.
            TimeSpan early_morning = new TimeSpan(6, 0, 0);
            TimeSpan morning = new TimeSpan(8, 0, 0);
            TimeSpan lunchtime = new TimeSpan(12, 0, 0);
            TimeSpan afternoon = new TimeSpan(13, 0, 0);
            TimeSpan after_school = new TimeSpan(15, 0, 0);
            TimeSpan evening = new TimeSpan(18, 0, 0);
            TimeSpan late_night = new TimeSpan(22, 0, 0);
            TimeSpan before_midnight = new TimeSpan(23, 59, 59);
            TimeSpan dark_hour = new TimeSpan(0, 0, 0);
            TimeSpan after_midnight = new TimeSpan(1, 0, 0);

            // Now, let's find out the current value.
            // If the current hour is before 1AM and after or on 12AM, set the time to Dark Hour.
            if (current_hour < after_midnight && current_hour >= dark_hour)
            {
                time_of_day = "dark_hour";
            }
            // If the current hour is before 11:59PM and after or on 10PM, set the time to Late Night.
            else if (current_hour < before_midnight && current_hour >= late_night)
            {
                time_of_day = "late_night";
            }
            // If the current hour is before 10PM and after or on 6PM, set the time to Evening.
            else if (current_hour < late_night && current_hour >= evening)
            {
                time_of_day = "evening";
            }
            // If the current hour is before 6PM and after or on 3PM, set the time depending on the day.
            // If it's a weekday, set it to After School.
            // If it's a Sunday or during school vacation, set it to Daytime.
            else if (current_hour < evening && current_hour >= after_school)
            {
                if (DateTime.Now.ToString("ddd") == "Sun" || School_Vacation_Check(input_time) == true)
                {
                    time_of_day = "daytime";
                }
                else
                {
                    time_of_day = "after_school";
                }
            }
            // If the current hour is before 3PM and after or on 1PM, set the time to Afternoon.
            else if (current_hour < after_school && current_hour >= afternoon)
            {
                time_of_day = "afternoon";
            }
            // If the current hour is before 1PM and after or on 12PM, set the time depending on the day.
            // If it's a weekday, set it to Lunchtime.
            // If it's a Sunday or during school vacation, set it to Daytime.
            else if (current_hour < afternoon && current_hour >= lunchtime)
            {
                if ((DateTime.Now.ToString("ddd") == "Sun") || (School_Vacation_Check(input_time) == true))
                {
                    time_of_day = "daytime";
                }
                else
                {
                    time_of_day = "lunchtime";
                }
            }
            // If the current hour is before 12PM and after or on 8AM, set the time to Morning.
            else if (current_hour < lunchtime && current_hour >= morning)
            {
                time_of_day = "morning";
            }
            // If the current hour is before 8AM and after or on 6AM, set the time to Early Morning.
            else if (current_hour < morning && current_hour >= early_morning)
            {
                time_of_day = "early_morning";
            }
            // If the current hour is before 6AM and after or on 1AM, set the time to Late Night.
            else if (current_hour < early_morning && current_hour >= after_midnight)
            {
                time_of_day = "late_night";
            }
            else
            {
                time_of_day = "null";
            }

            return time_of_day;
        }

        public static int Get_Full_Moon_Countdown(double age)
        {
            // Create a default return value. This is an unrealistic number for the countdown, but will not cause rendering issues if used.
            int countdownInt = 39;

            // Calculate how many days are left until the next full moon.
            // This is done by taking the day value of the cycle and seeing how many days are left until the next halfpoint is reached.
            if (age < 14.76)
            {
                age = 14.76 - age;
            }
            else if (age >= 14.76)
            {
                age = (29.53 + 14.76) - age;
            }

            // Round the answer to the nearest integer.
            countdownInt = (int)Math.Round(age);

            return countdownInt;
        }

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

        public static Bitmap Center_Image(Bitmap input_bitmap)
        {
            // Specify the width and height of the template we'll be drawing to.
            float width = 640;
            float height = 448;

            // Copy the input bitmap to a new bitmap variable.
            var image = new Bitmap(input_bitmap);

            // Create a number to scale the image by on the template.
            float scale = Math.Min(width / image.Width, height / image.Height);

            // Create a new bitmap with the specified width and height variables.
            var centered_bitmap = new Bitmap((int)width, (int)height);

            // Create a new graphics object so we can render the image to the new bitmap.
            var graphics = Graphics.FromImage(centered_bitmap);

            // uncomment for higher quality output
            graphics.InterpolationMode = InterpolationMode.High;
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // Set the pixel density of the image.
            centered_bitmap.SetResolution(96, 96);

            // Create the new width and height of the image.
            var scaleWidth = (int)(image.Width * scale);
            var scaleHeight = (int)(image.Height * scale);

            // Finally, draw the image!
            graphics.DrawImage(image, ((int)width - scaleWidth) / 2, ((int)height - scaleHeight) / 2, scaleWidth, scaleHeight);

            return centered_bitmap;
        }

        public static Bitmap Stretch_To_Fit(Bitmap input_bitmap)
        {
            // Set the width and height of the bitmap to be created
            float width = 640;
            float height = 448;

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

            // Set the pixel density of the image.
            new_bitmap.SetResolution(96, 96);

            // Draw the copy of the input bitmap to the new bitmap.
            graphics.DrawImage(bitmap_copy, 0, 0, width, height);

            return new_bitmap;
        }

        // Method from https://stackoverflow.com/questions/15408607/adjust-brightness-contrast-and-gamma-of-an-image
        public static Bitmap Increase_Brightness_Contrast(Bitmap input_bitmap)
        {
            //Bitmap originalImage = new Bitmap(input_bitmap.Width, input_bitmap.Height); ;
            Bitmap adjustedImage = new Bitmap(input_bitmap.Width, input_bitmap.Height);
            float brightness = 1.0f; // no change in brightness
            float contrast = 2.0f; // twice the contrast
            float gamma = 1.0f; // no change in gamma

            float adjustedBrightness = brightness - 1.0f;
            // create matrix that will brighten and contrast the image
            float[][] ptsArray = 
            {
                new float[] {contrast, 0, 0, 0, 0}, // scale red
                new float[] {0, contrast, 0, 0, 0}, // scale green
                new float[] {0, 0, contrast, 0, 0}, // scale blue
                new float[] {0, 0, 0, 1.0f, 0}, // don't scale alpha
                new float[] {adjustedBrightness, adjustedBrightness, adjustedBrightness, 0, 1}
            };

            ImageAttributes imageAttributes = new ImageAttributes();
            imageAttributes.ClearColorMatrix();
            imageAttributes.SetColorMatrix(new ColorMatrix(ptsArray), ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
            imageAttributes.SetGamma(gamma, ColorAdjustType.Bitmap);
            Graphics g = Graphics.FromImage(adjustedImage);
            g.DrawImage(input_bitmap, new Rectangle(0, 0, adjustedImage.Width, adjustedImage.Height)
                , 0, 0, input_bitmap.Width, input_bitmap.Height,
                GraphicsUnit.Pixel, imageAttributes);

            return adjustedImage;
        }

        // Method from https://www.codeproject.com/Tips/201129/Change-Opacity-of-Image-in-C
        public static System.Drawing.Image SetImageOpacity(System.Drawing.Image image, float opacity)
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

        // Methods from https://softwarebydefault.com/2013/03/03/colomatrix-image-filters/
        private static Bitmap GetArgbCopy(System.Drawing.Image sourceImage)
        {
            Bitmap bmpNew = new Bitmap(sourceImage.Width, sourceImage.Height, PixelFormat.Format32bppArgb);

            using (Graphics graphics = Graphics.FromImage(bmpNew))
            {
                graphics.DrawImage(sourceImage, new Rectangle(0, 0, bmpNew.Width, bmpNew.Height), new Rectangle(0, 0, bmpNew.Width, bmpNew.Height), GraphicsUnit.Pixel);
                graphics.Flush();
            }

            return bmpNew;
        }

        private static Bitmap ApplyColorMatrix(System.Drawing.Image sourceImage, ColorMatrix colorMatrix)
        {
            Bitmap bmp32BppSource = GetArgbCopy(sourceImage);
            Bitmap bmp32BppDest = new Bitmap(bmp32BppSource.Width, bmp32BppSource.Height, PixelFormat.Format32bppArgb);

            using (Graphics graphics = Graphics.FromImage(bmp32BppDest))
            {
                ImageAttributes bmpAttributes = new ImageAttributes();
                bmpAttributes.SetColorMatrix(colorMatrix);

                graphics.DrawImage(bmp32BppSource, new Rectangle(0, 0, bmp32BppSource.Width, bmp32BppSource.Height),
                                    0, 0, bmp32BppSource.Width, bmp32BppSource.Height, GraphicsUnit.Pixel, bmpAttributes);
            }

            bmp32BppSource.Dispose();

            return bmp32BppDest;
        }

        public static Bitmap DrawAsNegative(this System.Drawing.Image sourceImage)
        {
            ColorMatrix colorMatrix = new ColorMatrix(new float[][]
            {
                new float[]{-1, 0, 0, 0, 0},
                new float[]{0, -1, 0, 0, 0},
                new float[]{0, 0, -1, 0, 0},
                new float[]{0, 0, 0, 1, 0},
                new float[]{1, 1, 1, 1, 1}
            });

            return ApplyColorMatrix(sourceImage, colorMatrix);
        }

        public static Bitmap ColorTint(this Bitmap sourceBitmap, float blueTint, float greenTint, float redTint)
        {
            BitmapData sourceData = sourceBitmap.LockBits(new Rectangle(0, 0,
                                    sourceBitmap.Width, sourceBitmap.Height),
                                    ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);


            byte[] pixelBuffer = new byte[sourceData.Stride * sourceData.Height];


            Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length);


            sourceBitmap.UnlockBits(sourceData);


            float blue = 0;
            float green = 0;
            float red = 0;


            for (int k = 0; k + 4 < pixelBuffer.Length; k += 4)
            {
                blue = pixelBuffer[k] + (255 - pixelBuffer[k]) * blueTint;
                green = pixelBuffer[k + 1] + (255 - pixelBuffer[k + 1]) * greenTint;
                red = pixelBuffer[k + 2] + (255 - pixelBuffer[k + 2]) * redTint;


                if (blue > 255)
                { blue = 255; }


                if (green > 255)
                { green = 255; }


                if (red > 255)
                { red = 255; }


                pixelBuffer[k] = (byte)blue;
                pixelBuffer[k + 1] = (byte)green;
                pixelBuffer[k + 2] = (byte)red;


            }


            Bitmap resultBitmap = new Bitmap(sourceBitmap.Width, sourceBitmap.Height);


            BitmapData resultData = resultBitmap.LockBits(new Rectangle(0, 0,
                                    resultBitmap.Width, resultBitmap.Height),
                                    ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);


            Marshal.Copy(pixelBuffer, 0, resultData.Scan0, pixelBuffer.Length);
            resultBitmap.UnlockBits(resultData);


            return resultBitmap;
        }

        public static EmbedBuilder P3F_Loading_Message()
        {
            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Generating Scene...",
                IconUrl = "https://i.imgur.com/HlBRK9l.png"
            };

            embed.WithAuthor(author);
            embed.WithColor(37, 149, 255);
            embed.WithThumbnailUrl("https://i.imgur.com/VwI3i20.gif");
            embed.WithDescription("This may take a few seconds!");

            return embed;
        }
    }
}
