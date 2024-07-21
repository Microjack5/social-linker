using Discord;
using Discord.Rest;
using Discord.WebSocket;
using SocialLinker.Config;
using SocialLinker.Core.CloudStorageTables;
using SocialLinker.Core.LocalStorageTables;
using SocialLinker.Core.Menus;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SocialLinker.Core.SceneMaker
{
    public class BustupFrameSheets
    {
        // Sprite sheet formation
        public static Bitmap Generate_Standard_Bustup_Frame_Sheet(SocialLinkerCommand sl_command, OfficialSetData set_data, MakerCharacterData maker_character_data)
        {
            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(sl_command.User);

            // Establish the directory of the specified sprite set.
            string set_path = $@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//{set_data.Origin}//Bustup//{set_data.ID}";

            // Using the sprite set's path, create path variables for Eye and Mouth frame folders.
            // These paths aren't guaranteed to exist, but we'll handle that validity check later.
            string eye_frame_path = $@"{set_path}//Eyes";
            string mouth_frame_path = $@"{set_path}//Mouth";

            // Create a filename for the bitmap that will be generated.
            var fileName = $"{sl_command.User.Id}_{DateTime.UtcNow.ToString("yyyyMMdd_HH_mm_ss_fff")}.png";

            // Get a count of how many files are in the sprite set's directory.
            int filecount = OfficialSetMethods.AttachmentCountItemDirectory(set_path);

            // Create a variable for the base sprite's filename. We'll go searching for it in a few moments.
            string base_sprite_filename = "";

            // Create variables that keep track of how many frames of each frame type are present for the base sprite.
            // This will help us perform the math needed to form the sprite sheet.
            int eye_frame_count = 0;
            int mouth_frame_count = 0;

            base_sprite_filename = OfficialSetMethods.Get_Standard_Bustup_Filename_From_Sprite_Number(sl_command, set_data, maker_character_data);

            // At this point, we should have the file name for the base sprite.
            // Check if the path for the set's eye frames exists.
            // Not all sets will have eye frames, so this block doesn't always need to execute.
            if (Directory.Exists(eye_frame_path))
            {
                // Get a count of how many files are in the sprite set's directory.
                filecount = OfficialSetMethods.AttachmentCountItemDirectory(eye_frame_path);

                // Form an array of file names for the sprite's eye frames
                string[] allFiles = Directory.GetFiles(eye_frame_path, $"{base_sprite_filename}_e*.png");

                // Assign the array length to the eye frame count
                eye_frame_count = allFiles.Length;
            }

            // Check if the path for the set's mouth frames exists.
            // Not all sets will have mouth frames, so this block doesn't always need to execute.
            if (Directory.Exists(mouth_frame_path))
            {
                // Get a count of how many files are in the sprite set's directory.
                filecount = OfficialSetMethods.AttachmentCountItemDirectory(mouth_frame_path);

                // Form an array of file names for the sprite's mouth frames.
                string[] allFiles = Directory.GetFiles(mouth_frame_path, $"{base_sprite_filename}_m*.png");

                // Assign the array length to the mouth frame count.
                mouth_frame_count = allFiles.Length;
            }

            // Time to put together the final bitmap! Create the base that the other layers will go on.
            Bitmap base_template = new Bitmap(1000, 1000);

            // Here, start drawing on the base bitmap.
            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                // Depending on what types and how many frames the sprite has, we want to render the frame sheet in different manners.
                // Check for the case that the sprite has no eye or mouth frames.
                if (eye_frame_count == 0 && mouth_frame_count == 0)
                {
                    // Since there are no frames, create a bustup section that fills the entire frame sheet.
                    Bitmap bustup_section = Create_Base_Bustup_Bitmap((Bitmap)System.Drawing.Image.FromFile($"{set_path}//{base_sprite_filename}.png"), set_data, 0);

                    // Draw the bustup section to the base template.
                    graphics.DrawImage(bustup_section, 100, 0, bustup_section.Width, bustup_section.Height);
                }
                // Check for the case that the sprite has eye frames AND mouth frames.
                else if (eye_frame_count > 0 && mouth_frame_count > 0)
                {
                    // Create a bustup section that accounts for frames of both types present on the sheet.
                    Bitmap bustup_section = Create_Base_Bustup_Bitmap((Bitmap)System.Drawing.Image.FromFile($"{set_path}//{base_sprite_filename}.png"), set_data, 2);

                    // Draw the bustup section to the base template.
                    graphics.DrawImage(bustup_section, 100, 0, bustup_section.Width, bustup_section.Height);

                    // Create two lists of bitmaps to contain the frames for the sprite.
                    List<Bitmap> eye_frame_list = new List<Bitmap>();
                    List<Bitmap> mouth_frame_list = new List<Bitmap>();

                    // Add each eye frame for the sprite to the list.
                    // This assumes the file names are correct while iterating to add them instantly.
                    for (int i = 0; i < eye_frame_count; i++)
                    {
                        eye_frame_list.Add((Bitmap)System.Drawing.Image.FromFile($"{eye_frame_path}//{base_sprite_filename}_e{i + 1}.png"));
                    }

                    // Add each mouth frame for the sprite to the list.
                    // This assumes the file names are correct while iterating to add them instantly.
                    for (int i = 0; i < mouth_frame_count; i++)
                    {
                        mouth_frame_list.Add((Bitmap)System.Drawing.Image.FromFile($"{mouth_frame_path}//{base_sprite_filename}_m{i + 1}.png"));
                    }

                    // Create frame bitmaps for the eye and mouth sections.
                    Bitmap eye_frame_section = Create_Standard_Frame_Bitmap(eye_frame_list.ToArray());
                    Bitmap mouth_frame_section = Create_Standard_Frame_Bitmap(mouth_frame_list.ToArray());

                    // Draw the frame bitmaps to the base template.
                    graphics.DrawImage(eye_frame_section, 100, 400, eye_frame_section.Width, eye_frame_section.Height);
                    graphics.DrawImage(mouth_frame_section, 100, 700, mouth_frame_section.Width, mouth_frame_section.Height);
                }
                // Check for the case that the sprite either has eye frames but no mouth frames, or mouth frames but no eye frames.
                else if (eye_frame_count > 0 || mouth_frame_count > 0)
                {
                    // Create a bustup section that accounts for one type of frame present on the sheet.
                    Bitmap bustup_section = Create_Base_Bustup_Bitmap((Bitmap)System.Drawing.Image.FromFile($"{set_path}//{base_sprite_filename}.png"), set_data, 1);
                    graphics.DrawImage(bustup_section, 100, 0, bustup_section.Width, bustup_section.Height);

                    // Now, let's check for which frame type does exist for the sprite.
                    // If any eye frames exist, create an eye frame panel.
                    if (eye_frame_count > 0)
                    {
                        List<Bitmap> eye_frame_list = new List<Bitmap>();

                        for (int i = 0; i < eye_frame_count; i++)
                        {
                            eye_frame_list.Add((Bitmap)System.Drawing.Image.FromFile($"{eye_frame_path}//{base_sprite_filename}_e{i + 1}.png"));
                        }

                        Bitmap eye_frame_section = Create_Standard_Frame_Bitmap(eye_frame_list.ToArray());
                        graphics.DrawImage(eye_frame_section, 100, 700, eye_frame_section.Width, eye_frame_section.Height);
                    }
                    // If any mouth frames exist, create a mouth frame panel.
                    else if (mouth_frame_count > 0)
                    {
                        List<Bitmap> mouth_frame_list = new List<Bitmap>();

                        for (int i = 0; i < mouth_frame_count; i++)
                        {
                            mouth_frame_list.Add((Bitmap)System.Drawing.Image.FromFile($"{mouth_frame_path}//{base_sprite_filename}_m{i + 1}.png"));
                        }

                        Bitmap mouth_frame_section = Create_Standard_Frame_Bitmap(mouth_frame_list.ToArray());
                        graphics.DrawImage(mouth_frame_section, 100, 700, mouth_frame_section.Width, mouth_frame_section.Height);
                    }
                }

                // Now, we should create an overlay that will assist the user's viewing of the frame sheet.
                // Create a "black bar" bitmap that will contain information on the side of the frame sheet.
                Bitmap black_bar = new Bitmap(100, 1000);

                // We'll also want to create a "white bar" bitmap to separate sections on the frame sheet.
                Bitmap white_bar = new Bitmap(1000, 6);

                // Fill the black_bar bitmap with the color black.
                using (Graphics overlay_object = Graphics.FromImage(black_bar))
                {
                    overlay_object.Clear(System.Drawing.Color.Black);
                }

                // Fill the white_bar bitmap with the color white.
                using (Graphics overlay_object = Graphics.FromImage(white_bar))
                {
                    overlay_object.Clear(System.Drawing.Color.White);
                }

                // Draw the black bar to the base template. We'll handle drawing the white bars later.
                graphics.DrawImage(black_bar, 0, 0, black_bar.Width, black_bar.Height);

                // Now, let's start rendering text for user readability.
                //Set text rendering to have antialiasing.
                graphics.TextRenderingHint = TextRenderingHint.AntiAlias;

                // Create three rectangle variables to represent text boxes that may be rendered to the template.
                // Depending on the frames available, only one or two of these variables may be used.
                Rectangle base_text_box = new Rectangle(0, 0, 0, 0);
                Rectangle eyes_text_box = new Rectangle(0, 0, 0, 0);
                Rectangle mouth_text_box = new Rectangle(0, 0, 0, 0);

                // Create a font object to draw text to the base template.
                using (Font frame_font = new Font("Eurostar Black Extended", 35))
                {
                    // Format strings so that their placement is at the center of the text box.
                    StringFormat stringFormat = new StringFormat();
                    stringFormat.Alignment = StringAlignment.Center;
                    stringFormat.LineAlignment = StringAlignment.Center;

                    // Check if the number of eye and mouth frames present is zero.
                    if (eye_frame_count == 0 && mouth_frame_count == 0)
                    {
                        // Rotate the graphics object by -90 degrees so that the text will appear on its side.
                        graphics.RotateTransform(-90);

                        // Redefine the text box for the base sprite to take up the entire sheet since there will be no frames to show and draw the text to the template.
                        // The X coordinate starts at -1000 to compensate for the -90 degree rotation.
                        base_text_box = new Rectangle(-1000 + 0, 0, 1000, 100);
                        graphics.DrawString("NO FRAMES", frame_font, System.Drawing.Brushes.White, base_text_box, stringFormat);
                    }
                    // Check if there are both eye frames and mouth frames present.
                    else if (eye_frame_count > 0 && mouth_frame_count > 0)
                    {
                        // Draw two white bars to the template so they will appear as dividers for each section.
                        graphics.DrawImage(white_bar, 0, 397, white_bar.Width, white_bar.Height);
                        graphics.DrawImage(white_bar, 0, 697, white_bar.Width, white_bar.Height);

                        // Rotate the graphics object by -90 degrees so that the text will appear on its side.
                        graphics.RotateTransform(-90);

                        // Redefine the text boxes for the base, eye, and mouth sections and draw their text to the template.
                        // The X coordinates start at -1000 to compensate for the -90 degree rotation.
                        base_text_box = new Rectangle(-1000 + 600, 0, 400, 100);
                        graphics.DrawString("BASE", frame_font, System.Drawing.Brushes.White, base_text_box, stringFormat);

                        eyes_text_box = new Rectangle(-1000 + 300, 0, 300, 100);
                        graphics.DrawString("EYES", frame_font, System.Drawing.Brushes.White, eyes_text_box, stringFormat);

                        mouth_text_box = new Rectangle(-1000 + 0, 0, 300, 100);
                        graphics.DrawString("MOUTH", frame_font, System.Drawing.Brushes.White, mouth_text_box, stringFormat);
                    }
                    // Check if there are either eye frames OR mouth frames present.
                    else if (eye_frame_count > 0 || mouth_frame_count > 0)
                    {
                        // Draw a white bar to the template so it will appear as a divider for each section.
                        graphics.DrawImage(white_bar, 0, 697, white_bar.Width, white_bar.Height);

                        // Rotate the graphics object by -90 degrees so that the text will appear on its side.
                        graphics.RotateTransform(-90);

                        // Redefine the text box for the base sprite and draw the text for it to the template.
                        // The X coordinates start at -1000 to compensate for the -90 degree rotation.
                        base_text_box = new Rectangle(-1000 + 300, 0, 700, 100);
                        graphics.DrawString("BASE", frame_font, System.Drawing.Brushes.White, base_text_box, stringFormat);

                        // Since we've confirmed there are either eye frames or mouth frames present, do a comparison to see which type it is and draw the appropriate text to the template.
                        // The X coordinates start at -1000 to compensate for the -90 degree rotation.
                        if (eye_frame_count > 0)
                        {
                            eyes_text_box = new Rectangle(-1000 + 0, 0, 300, 100);
                            graphics.DrawString("EYES", frame_font, System.Drawing.Brushes.White, eyes_text_box, stringFormat);
                        }
                        else if (mouth_frame_count > 0)
                        {
                            mouth_text_box = new Rectangle(-1000 + 0, 0, 300, 100);
                            graphics.DrawString("MOUTH", frame_font, System.Drawing.Brushes.White, mouth_text_box, stringFormat);
                        }
                    }
                }
            }

            // Return the base template.
            return base_template;
        }

        public static Bitmap Create_Base_Bustup_Bitmap(Bitmap scrBitmap, OfficialSetData set_data, int frame_type)
        {
            float width = 900;
            float height = 0;

            if (frame_type == 0) // '0' denotes no frame types present (no eyes or mouth).
            {
                height = 1000;
            }
            else if (frame_type == 1) // '1' denotes only one frame type present (eyes or mouth).
            {
                height = 700;
            }
            else if (frame_type == 2) // '2' denotes two (both) frame types present (eyes and mouth).
            {
                height = 400;
            }

            var image = new Bitmap(scrBitmap);

            float scale = Math.Min(width / image.Width, height / image.Height);

            var bmp = new Bitmap((int)width, (int)height);
            var graph = Graphics.FromImage(bmp);

            if (set_data.Origin == "P1-PS1" ||
                set_data.Origin == "P1-PSP" ||
                set_data.Origin == "P2IS-PS1" ||
                set_data.Origin == "P2IS-PSP" ||
                set_data.Origin == "P2EP-PS1" ||
                set_data.Origin == "P2EP-PSP" ||
                set_data.Origin == "P3P")
            {
                // Set the scaling mode for any rendered images to nearest neighbor.
                graph.InterpolationMode = InterpolationMode.NearestNeighbor;
            }

            bmp.SetResolution(96, 96);

            var scaleWidth = (int)(image.Width * scale);
            var scaleHeight = (int)(image.Height * scale);

            graph.DrawImage(image, ((int)width - scaleWidth) / 2, ((int)height - scaleHeight) / 2, scaleWidth, scaleHeight);

            return bmp;
        }

        public static Bitmap Create_Standard_Frame_Bitmap(Bitmap[] frame_array)
        {
            // Create width and height variables for the base bitmap.
            int width = 900;
            int height = 300;

            // Using the width and height specified, create a new bitmap.
            Bitmap base_template = new Bitmap(width, height);

            // Time to edit the base bitmap! Create a graphics object.
            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                // Set the DPI of the bitmap for consistent rendering.
                base_template.SetResolution(96, 96);

                // To create the full frame panel, we'll want to divide the area into easier-to-manage "sub-panels" that can be rendered and placed side-by-side next to each other.
                // First, let's determine the width of a single sub-panel so we can have equal spacing. We can do this by dividing the base template's width by the amount of frames we need to render.
                int sub_panel_width = width / frame_array.Length;

                // Next, time for a loop!
                // For as many entries are in the frame array, we'll want to create a sub-panel for each one and center the frame within it.
                for (int i = 0; i < frame_array.Length; i++)
                {
                    // Create the sub-panel bitmap with the dimensions we now have.
                    Bitmap sub_panel = new Bitmap(sub_panel_width, height);

                    // Since we want the frame to fit nicely in the sub-panel, we should check whether the frame is too small or too large for the space.
                    // First, check if the frame is too small to view in the sub-panel.
                    if (frame_array[i].Height < (height / 4))
                    {
                        // If so, double its dimensions.
                        frame_array[i] = new Bitmap(frame_array[i], new Size((frame_array[i].Width * 2), (frame_array[i].Height * 2)));
                    }

                    // Next, check if the frame is too large to fit in the sub-panel.
                    while (frame_array[i].Height > (height / 2))
                    {
                        // If so, divide the dimensions by 25%.
                        frame_array[i] = new Bitmap(frame_array[i], new Size((int)(frame_array[i].Width / 1.5), (int)(frame_array[i].Height / 1.5)));
                    }

                    // Assign the currently iterated frame in the frame array to a bitmap variable.
                    Bitmap current_frame = frame_array[i];

                    // Create another graphics object to edit the sub-panel.
                    using (Graphics panel_render = Graphics.FromImage(sub_panel))
                    {
                        // Determine where the frame should be placed on the Y axis by centering it within the sub-panel based on its height.
                        int frame_y_position = (height - current_frame.Height) / 2;

                        // Render the frame on the sub-panel.
                        panel_render.DrawImage(current_frame, (sub_panel_width - current_frame.Width) / 2, frame_y_position, current_frame.Width, current_frame.Height);

                        // Lastly, let's number the frame in the sub-panel.
                        using (Font rockwell = new Font("Rockwell", 45, FontStyle.Bold))
                        {
                            // Create a GraphicsPath object.
                            GraphicsPath myPath = new GraphicsPath();

                            // Set up all the string parameters.
                            string stringText = $"{i + 1}";

                            System.Drawing.FontFamily family = new System.Drawing.FontFamily("Rockwell");
                            int fontStyle = (int)FontStyle.Bold;
                            int emSize = 23;
                            Point origin = new Point((sub_panel_width * i) + (sub_panel_width / 2), frame_y_position + current_frame.Height + 40);

                            StringFormat stringFormat = new StringFormat();
                            stringFormat.Alignment = StringAlignment.Center;
                            stringFormat.LineAlignment = StringAlignment.Center;

                            // Add the string to the path.
                            myPath.AddString(stringText,
                                family,
                                fontStyle,
                                emSize = (int)graphics.DpiY * 60 / 72,
                                origin,
                                stringFormat);

                            //Draw the path to the screen.
                            graphics.SmoothingMode = SmoothingMode.AntiAlias;
                            graphics.FillPath(System.Drawing.Brushes.White, myPath);
                            graphics.DrawPath(new System.Drawing.Pen(System.Drawing.Brushes.Black, 4), myPath);
                            graphics.FillPath(System.Drawing.Brushes.White, myPath);
                            graphics.DrawPath(new System.Drawing.Pen(System.Drawing.Brushes.Black, 0), myPath);
                        }
                    }

                    // Render the sub-panel on the base template based on the current iteration of the frame array loop.
                    graphics.DrawImage(sub_panel, (sub_panel_width * i), 0, sub_panel.Width, sub_panel.Height);
                }
            }

            // Return the base template.
            return base_template;
        }

        // Methods made specifically for P4D
        public static Bitmap Generate_P4D_Bustup_Frame_Sheet(SocialLinkerCommand sl_command, OfficialSetData set_data, MakerCommandData maker_command_data)
        {
            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(sl_command.User);

            // Establish the directory of the specified sprite set.
            string set_path = $@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//{set_data.Origin}//Bustup//{set_data.ID}";

            // Using the sprite set's path, create path variables for Eye and Mouth frame folders.
            // These paths aren't guaranteed to exist, but we'll handle that validity check later.
            string eye_frame_path = $@"{set_path}//Eyes";
            string mouth_frame_path = $@"{set_path}//Mouth";

            // Create a filename for the bitmap that will be generated.
            var fileName = $"{sl_command.User.Id}_{DateTime.UtcNow.ToString("yyyyMMdd_HH_mm_ss_fff")}.png";

            // Get a count of how many files are in the sprite set's directory.
            int filecount = OfficialSetMethods.AttachmentCountItemDirectory(set_path);

            // Create a variable for the base sprite's filename. We'll go searching for it in a few moments.
            string base_sprite_filename = "";

            // Create variables that keep track of how many frames of each frame type are present for the base sprite.
            // This will help us perform the math needed to form the sprite sheet.
            int eye_frame_count = 0;
            int mouth_frame_count = 0;

            // Check if the sprite set's directory exists.
            if (Directory.Exists(set_path))
            {
                // If so, it's time to find the filename for the user's selected sprite so we can retrieve the frames associated with it.
                // We can do this by creating a counter starting from zero that will increment by one until it reaches the sprite numer the user specified.
                // Once it reaches that number, the iterated filename will be saved and we can use that to find its associated frames.
                int counter = 0;
                int base_sprite_number = maker_command_data.Character_Data_1.Base_Sprite;

                // The manner of iteration will change based on the user's settings.
                // First, Order by Outfit.
                if (account.Setting_Sheet_Order == "Order by Outfit")
                {
                    // Create a loop starting at 1 meant to iterate though every file in the directory.
                    // Outfit numbers always start at 1, so we'll begin there.
                    for (int outfit = 1; outfit <= filecount; outfit++)
                    {
                        // Inside, create a secondary loop also meant to iterate though every file in the directory.
                        // This loop is searching for expressions, which start at 1.
                        for (int expression = 1; expression <= filecount; expression++)
                        {
                            // Here, we're going to create a file path that could potentially exist given the combination of expression and outfit numbers.
                            // Check if the created file path string exists.
                            if (File.Exists($"{set_path}//{set_data.ID.ToLower()}_{expression}_{outfit}.png"))
                            {
                                // If the file does exist, increment the counter by one.
                                counter++;

                                // Check if the counter matches the same number of the chosen sprite number.
                                if (counter == base_sprite_number)
                                {
                                    // If it does, we found our sprite! Save the filename to the variable created earlier so we can reference it later.
                                    base_sprite_filename = $"{set_data.ID.ToLower()}_{expression}_{outfit}";

                                    // Break out of the current loop.
                                    break;
                                }
                            }
                        }

                        // Check if the filename variable for the base sprite is not empty.
                        if (base_sprite_filename != "")
                        {
                            // If so, we already found our filename! Break out of the outer loop.
                            break;
                        }
                    }
                }
                // Second case, Order by Expression.
                else if (account.Setting_Sheet_Order == "Order by Expression")
                {
                    // Create a loop starting at 1 meant to iterate though every file in the directory.
                    // Expression numbers always start at 1, so we'll begin there.
                    for (int expression = 1; expression <= filecount; expression++)
                    {
                        // Inside, create a secondary loop also meant to iterate though every file in the directory.
                        // This loop is searching for outfits, which start at 1.
                        for (int outfit = 1; outfit <= filecount; outfit++)
                        {
                            // Here, we're going to create a file path that could potentially exist given the combination of expression and outfit numbers.
                            // Check if the created file path string exists.
                            if (File.Exists($"{set_path}//{set_data.ID.ToLower()}_{expression}_{outfit}.png"))
                            {
                                // If the file does exist, increment the counter by one.
                                counter++;

                                // Check if the counter matches the same number of the chosen sprite number.
                                if (counter == base_sprite_number)
                                {
                                    // If it does, we found our sprite! Save the filename to the variable created earlier so we can reference it later.
                                    base_sprite_filename = $"{set_data.ID.ToLower()}_{expression}_{outfit}";

                                    // Break out of the current loop.
                                    break;
                                }
                            }
                        }

                        // Check if the filename variable for the base sprite is not empty.
                        if (base_sprite_filename != "")
                        {
                            // If so, we already found our filename! Break out of the outer loop.
                            break;
                        }
                    }
                }
            }

            // At this point, we should have the file name for the base sprite.
            // Check if the path for the set's eye frames exists.
            // Not all sets will have eye frames, so this block doesn't always need to execute.
            if (Directory.Exists(eye_frame_path))
            {
                // Get a count of how many files are in the sprite set's directory.
                filecount = OfficialSetMethods.AttachmentCountItemDirectory(eye_frame_path);

                // Form an array of file names for the sprite's eye frames
                string[] allFiles = Directory.GetFiles(eye_frame_path, $"{base_sprite_filename}_e*.png");

                // Assign the array length to the eye frame count
                eye_frame_count = allFiles.Length;
            }

            // Check if the path for the set's mouth frames exists.
            // Not all sets will have mouth frames, so this block doesn't always need to execute.
            if (Directory.Exists(mouth_frame_path))
            {
                // Get a count of how many files are in the sprite set's directory.
                filecount = OfficialSetMethods.AttachmentCountItemDirectory(mouth_frame_path);

                // Form an array of file names for the sprite's mouth frames.
                string[] allFiles = Directory.GetFiles(mouth_frame_path, $"{base_sprite_filename}_m*.png");

                // Assign the array length to the mouth frame count.
                mouth_frame_count = allFiles.Length;
            }

            // Time to put together the final bitmap! Create the base that the other layers will go on.
            Bitmap base_template = new Bitmap(1000, 1000);

            // Here, start drawing on the base bitmap.
            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                // Depending on what types and how many frames the sprite has, we want to render the frame sheet in different manners.
                // Check for the case that the sprite has no eye or mouth frames.
                if (eye_frame_count == 0 && mouth_frame_count == 0)
                {
                    // Since there are no frames, create a bustup section that fills the entire frame sheet.
                    Bitmap bustup_section = Create_P4D_Base_Bustup_Bitmap((Bitmap)System.Drawing.Image.FromFile($"{set_path}//{base_sprite_filename}.png"), set_data, 0);

                    // Draw the bustup section to the base template.
                    graphics.DrawImage(bustup_section, 100, 0, bustup_section.Width, bustup_section.Height);
                }
                // Check for the case that the sprite has eye frames AND mouth frames.
                else if (eye_frame_count > 0 && mouth_frame_count > 0)
                {
                    // Create a bustup section that accounts for frames of both types present on the sheet.
                    Bitmap bustup_section = Create_P4D_Base_Bustup_Bitmap((Bitmap)System.Drawing.Image.FromFile($"{set_path}//{base_sprite_filename}.png"), set_data, 2);

                    // Draw the bustup section to the base template.
                    graphics.DrawImage(bustup_section, 100, 0, bustup_section.Width, bustup_section.Height);

                    // Create two lists of bitmaps to contain the frames for the sprite.
                    List<Bitmap> eye_frame_list = new List<Bitmap>();
                    List<Bitmap> mouth_frame_list = new List<Bitmap>();

                    // Add each eye frame for the sprite to the list.
                    // This assumes the file names are correct while iterating to add them instantly.
                    for (int i = 0; i < eye_frame_count; i++)
                    {
                        Bitmap cropped_frame = Crop_Alpha_From_Bitmap((Bitmap)System.Drawing.Image.FromFile($"{eye_frame_path}//{base_sprite_filename}_e{i + 1}.png"), "Eyes");
                        eye_frame_list.Add(cropped_frame);
                    }

                    // Add each mouth frame for the sprite to the list.
                    // This assumes the file names are correct while iterating to add them instantly.
                    for (int i = 0; i < mouth_frame_count; i++)
                    {
                        Bitmap cropped_frame = Crop_Alpha_From_Bitmap((Bitmap)System.Drawing.Image.FromFile($"{mouth_frame_path}//{base_sprite_filename}_m{i + 1}.png"), "Mouth");
                        mouth_frame_list.Add(cropped_frame);
                    }

                    // Create frame bitmaps for the eye and mouth sections.
                    Bitmap eye_frame_section = Create_P4D_Frame_Bitmap(eye_frame_list.ToArray());
                    Bitmap mouth_frame_section = Create_P4D_Frame_Bitmap(mouth_frame_list.ToArray());

                    // Draw the frame bitmaps to the base template.
                    graphics.DrawImage(eye_frame_section, 100, 400, eye_frame_section.Width, eye_frame_section.Height);
                    graphics.DrawImage(mouth_frame_section, 100, 700, mouth_frame_section.Width, mouth_frame_section.Height);
                }
                // Check for the case that the sprite either has eye frames but no mouth frames, or mouth frames but no eye frames.
                else if (eye_frame_count > 0 || mouth_frame_count > 0)
                {
                    // Create a bustup section that accounts for one type of frame present on the sheet.
                    Bitmap bustup_section = Create_P4D_Base_Bustup_Bitmap((Bitmap)System.Drawing.Image.FromFile($"{set_path}//{base_sprite_filename}.png"), set_data, 1);
                    graphics.DrawImage(bustup_section, 100, 0, bustup_section.Width, bustup_section.Height);

                    // Now, let's check for which frame type does exist for the sprite.
                    // If any eye frames exist, create an eye frame panel.
                    if (eye_frame_count > 0)
                    {
                        List<Bitmap> eye_frame_list = new List<Bitmap>();

                        for (int i = 0; i < eye_frame_count; i++)
                        {
                            Bitmap cropped_frame = Crop_Alpha_From_Bitmap((Bitmap)System.Drawing.Image.FromFile($"{eye_frame_path}//{base_sprite_filename}_e{i + 1}.png"), "Eyes");
                            eye_frame_list.Add(cropped_frame);
                        }

                        Bitmap eye_frame_section = Create_P4D_Frame_Bitmap(eye_frame_list.ToArray());
                        graphics.DrawImage(eye_frame_section, 100, 700, eye_frame_section.Width, eye_frame_section.Height);
                    }
                    // If any mouth frames exist, create a mouth frame panel.
                    else if (mouth_frame_count > 0)
                    {
                        List<Bitmap> mouth_frame_list = new List<Bitmap>();

                        for (int i = 0; i < mouth_frame_count; i++)
                        {
                            Bitmap cropped_frame = Crop_Alpha_From_Bitmap((Bitmap)System.Drawing.Image.FromFile($"{mouth_frame_path}//{base_sprite_filename}_m{i + 1}.png"), "Mouth");
                            mouth_frame_list.Add(cropped_frame);
                        }

                        Bitmap mouth_frame_section = Create_P4D_Frame_Bitmap(mouth_frame_list.ToArray());
                        graphics.DrawImage(mouth_frame_section, 100, 700, mouth_frame_section.Width, mouth_frame_section.Height);
                    }
                }

                // Now, we should create an overlay that will assist the user's viewing of the frame sheet.
                // Create a "black bar" bitmap that will contain information on the side of the frame sheet.
                Bitmap black_bar = new Bitmap(100, 1000);

                // We'll also want to create a "white bar" bitmap to separate sections on the frame sheet.
                Bitmap white_bar = new Bitmap(1000, 6);

                // Fill the black_bar bitmap with the color black.
                using (Graphics overlay_object = Graphics.FromImage(black_bar))
                {
                    overlay_object.Clear(System.Drawing.Color.Black);
                }

                // Fill the white_bar bitmap with the color white.
                using (Graphics overlay_object = Graphics.FromImage(white_bar))
                {
                    overlay_object.Clear(System.Drawing.Color.White);
                }

                // Draw the black bar to the base template. We'll handle drawing the white bars later.
                graphics.DrawImage(black_bar, 0, 0, black_bar.Width, black_bar.Height);

                // Now, let's start rendering text for user readability.
                //Set text rendering to have antialiasing.
                graphics.TextRenderingHint = TextRenderingHint.AntiAlias;

                // Create three rectangle variables to represent text boxes that may be rendered to the template.
                // Depending on the frames available, only one or two of these variables may be used.
                Rectangle base_text_box = new Rectangle(0, 0, 0, 0);
                Rectangle eyes_text_box = new Rectangle(0, 0, 0, 0);
                Rectangle mouth_text_box = new Rectangle(0, 0, 0, 0);

                // Create a font object to draw text to the base template.
                using (Font frame_font = new Font("Eurostar Black Extended", 35))
                {
                    // Format strings so that their placement is at the center of the text box.
                    StringFormat stringFormat = new StringFormat();
                    stringFormat.Alignment = StringAlignment.Center;
                    stringFormat.LineAlignment = StringAlignment.Center;

                    // Check if the number of eye and mouth frames present is zero.
                    if (eye_frame_count == 0 && mouth_frame_count == 0)
                    {
                        // Rotate the graphics object by -90 degrees so that the text will appear on its side.
                        graphics.RotateTransform(-90);

                        // Redefine the text box for the base sprite to take up the entire sheet since there will be no frames to show and draw the text to the template.
                        // The X coordinate starts at -1000 to compensate for the -90 degree rotation.
                        base_text_box = new Rectangle(-1000 + 0, 0, 1000, 100);
                        graphics.DrawString("NO FRAMES", frame_font, System.Drawing.Brushes.White, base_text_box, stringFormat);
                    }
                    // Check if there are both eye frames and mouth frames present.
                    else if (eye_frame_count > 0 && mouth_frame_count > 0)
                    {
                        // Draw two white bars to the template so they will appear as dividers for each section.
                        graphics.DrawImage(white_bar, 0, 397, white_bar.Width, white_bar.Height);
                        graphics.DrawImage(white_bar, 0, 697, white_bar.Width, white_bar.Height);

                        // Rotate the graphics object by -90 degrees so that the text will appear on its side.
                        graphics.RotateTransform(-90);

                        // Redefine the text boxes for the base, eye, and mouth sections and draw their text to the template.
                        // The X coordinates start at -1000 to compensate for the -90 degree rotation.
                        base_text_box = new Rectangle(-1000 + 600, 0, 400, 100);
                        graphics.DrawString("BASE", frame_font, System.Drawing.Brushes.White, base_text_box, stringFormat);

                        eyes_text_box = new Rectangle(-1000 + 300, 0, 300, 100);
                        graphics.DrawString("EYES", frame_font, System.Drawing.Brushes.White, eyes_text_box, stringFormat);

                        mouth_text_box = new Rectangle(-1000 + 0, 0, 300, 100);
                        graphics.DrawString("MOUTH", frame_font, System.Drawing.Brushes.White, mouth_text_box, stringFormat);
                    }
                    // Check if there are either eye frames OR mouth frames present.
                    else if (eye_frame_count > 0 || mouth_frame_count > 0)
                    {
                        // Draw a white bar to the template so it will appear as a divider for each section.
                        graphics.DrawImage(white_bar, 0, 697, white_bar.Width, white_bar.Height);

                        // Rotate the graphics object by -90 degrees so that the text will appear on its side.
                        graphics.RotateTransform(-90);

                        // Redefine the text box for the base sprite and draw the text for it to the template.
                        // The X coordinates start at -1000 to compensate for the -90 degree rotation.
                        base_text_box = new Rectangle(-1000 + 300, 0, 700, 100);
                        graphics.DrawString("BASE", frame_font, System.Drawing.Brushes.White, base_text_box, stringFormat);

                        // Since we've confirmed there are either eye frames or mouth frames present, do a comparison to see which type it is and draw the appropriate text to the template.
                        // The X coordinates start at -1000 to compensate for the -90 degree rotation.
                        if (eye_frame_count > 0)
                        {
                            eyes_text_box = new Rectangle(-1000 + 0, 0, 300, 100);
                            graphics.DrawString("EYES", frame_font, System.Drawing.Brushes.White, eyes_text_box, stringFormat);
                        }
                        else if (mouth_frame_count > 0)
                        {
                            mouth_text_box = new Rectangle(-1000 + 0, 0, 300, 100);
                            graphics.DrawString("MOUTH", frame_font, System.Drawing.Brushes.White, mouth_text_box, stringFormat);
                        }
                    }
                }
            }

            // Return the base template.
            return base_template;
        }

        public static Bitmap Create_P4D_Base_Bustup_Bitmap(Bitmap input_bitmap, OfficialSetData set_data, int frame_type)
        {
            // First, establish the dimensions for the section.
            // We know the width will be 900 pixels, but the height could vary depending on how many frames are available. So, we initialize that to zero for now.
            float width = 900;
            float height = 0;

            // Here, we'll determine the height of the section.
            if (frame_type == 0) // '0' denotes no frame types present (no eyes or mouth).
            {
                height = 1000;
            }
            else if (frame_type == 1) // '1' denotes only one frame type present (eyes or mouth).
            {
                height = 700;
            }
            else if (frame_type == 2) // '2' denotes two (both) frame types present (eyes and mouth).
            {
                height = 400;
            }

            // Copy the input bitmap to a new bitmap variable.
            var image = new Bitmap(input_bitmap);

            // Check if the image is not a square and the height is greater than 1024 (the average height of a P4D bustup).
            if ((image.Width != image.Height) && (image.Height > 1024))
            {
                // If so, we'll have to crop the bustup for the frame sheet.
                // Create a new bitmap at a 1024 x 1024 resolution.
                Bitmap cropped_image = new Bitmap(1024, 1024);

                // Using a new graphics object, draw the bustup to the newly created bitmap, effectively cropping it.
                using (Graphics graphics = Graphics.FromImage(cropped_image))
                {
                    graphics.DrawImage(image, 0, -(image.Height - 1024), image.Width, image.Height);
                }

                // Copy the cropped_image bitmap to the image variable.
                image = cropped_image;
            }

            float scale = Math.Min(width / image.Width, height / image.Height);

            var bmp = new Bitmap((int)width, (int)height);
            var graph = Graphics.FromImage(bmp);

            bmp.SetResolution(96, 96);

            var scaleWidth = (int)(image.Width * scale);
            var scaleHeight = (int)(image.Height * scale);

            graph.DrawImage(image, ((int)width - scaleWidth) / 2, ((int)height - scaleHeight) / 2, scaleWidth, scaleHeight);

            return bmp;
        }

        public static Bitmap Create_P4D_Frame_Bitmap(Bitmap[] frame_array)
        {
            // Create width and height variables for the base bitmap.
            int width = 900;
            int height = 300;

            // Using the width and height specified, create a new bitmap.
            Bitmap base_template = new Bitmap(width, height);

            // Time to edit the base bitmap! Create a graphics object.
            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                // Set the DPI of the bitmap for consistent rendering.
                base_template.SetResolution(96, 96);

                // To create the full frame panel, we'll want to divide the area into easier-to-manage "sub-panels" that can be rendered and placed side-by-side next to each other.
                // First, let's determine the width of a single sub-panel so we can have equal spacing. We can do this by dividing the base template's width by the amount of frames we need to render.
                int sub_panel_width = width / frame_array.Length;

                // Create the sub-panel bitmap with the dimensions we now have.
                Bitmap sub_panel = new Bitmap(sub_panel_width, height);

                // Next, time for a loop!
                // For as many entries are in the frame array, we'll want to create a sub-panel for each one and center the frame within it.
                for (int i = 0; i < frame_array.Length; i++)
                {
                    // Since we want the frame to fit nicely in the sub-panel, we should check whether the frame is too small or too large for the space.
                    // First, check if the frame is too small to view in the sub-panel.
                    if (frame_array[i].Height < (height / 4))
                    {
                        // If so, double its dimensions.
                        frame_array[i] = new Bitmap(frame_array[i], new Size((frame_array[i].Width * 2), (frame_array[i].Height * 2)));
                    }

                    // Next, check if the frame is too large to fit in the sub-panel.
                    while (frame_array[i].Height > (height / 2))
                    {
                        // If so, divide the dimensions by 20%.
                        frame_array[i] = new Bitmap(frame_array[i], new Size((int)(frame_array[i].Width / 1.2), (int)(frame_array[i].Height / 1.2)));
                    } 

                    // Assign the currently iterated frame in the frame array to a bitmap variable.
                    Bitmap current_frame = frame_array[i];

                    // Create another graphics object to edit the sub-panel.
                    using (Graphics panel_render = Graphics.FromImage(sub_panel))
                    {
                        // Determine where the frame should be placed on the Y axis by centering it within the sub-panel based on its height.
                        int frame_y_position = (height - current_frame.Height) / 2;

                        // Render the frame on the sub-panel.
                        panel_render.DrawImage(current_frame, (sub_panel_width - current_frame.Width) / 2, frame_y_position, current_frame.Width, current_frame.Height);

                        // Lastly, let's number the frame in the sub-panel.
                        using (Font rockwell = new Font("Rockwell", 45, FontStyle.Bold))
                        {
                            // Create a GraphicsPath object.
                            GraphicsPath myPath = new GraphicsPath();

                            // Set up all the string parameters.
                            string stringText = $"{i + 1}";

                            System.Drawing.FontFamily family = new System.Drawing.FontFamily("Rockwell");
                            int fontStyle = (int)FontStyle.Bold;
                            int emSize = 23;
                            Point origin = new Point((sub_panel_width * i) + (sub_panel_width / 2), frame_y_position + current_frame.Height + 40);

                            StringFormat stringFormat = new StringFormat();
                            stringFormat.Alignment = StringAlignment.Center;
                            stringFormat.LineAlignment = StringAlignment.Center;

                            // Add the string to the path.
                            myPath.AddString(stringText,
                                family,
                                fontStyle,
                                emSize = (int)graphics.DpiY * 60 / 72,
                                origin,
                                stringFormat);

                            //Draw the path to the screen.
                            graphics.SmoothingMode = SmoothingMode.AntiAlias;
                            graphics.FillPath(System.Drawing.Brushes.White, myPath);
                            graphics.DrawPath(new System.Drawing.Pen(System.Drawing.Brushes.Black, 4), myPath);
                            graphics.FillPath(System.Drawing.Brushes.White, myPath);
                            graphics.DrawPath(new System.Drawing.Pen(System.Drawing.Brushes.Black, 0), myPath);
                        }
                    }

                    // Render the sub-panel on the base template based on the current iteration of the frame array loop.
                    graphics.DrawImage(sub_panel, (sub_panel_width * i), 0, sub_panel.Width, sub_panel.Height);
                }
            }

            // Return the base template.
            return base_template;
        }

        public static Bitmap Crop_Alpha_From_Bitmap(Bitmap input_bitmap, string frame_type)
        {
            // Declare a bitmap variable that will be returned at the end of the function.
            Bitmap cropped_frame;

            // Crop the input bitmap in different ways depending on what type of frame it is.
            // First, let's start with Eye bitmaps.
            if (frame_type == "Eyes")
            {
                // Find the height of the bitmap we should crop to.
                int true_bitmap_height = Find_True_Eye_Frame_Height(input_bitmap);

                // Create a new bitmap using the width of the input bitmap and the true height value we just calculated.
                // If the "true height" calculated was 0 however, that means there was no empty space on the frame to crop.
                // In that case, the entire input bitmap can be copied over.
                if (true_bitmap_height != 0)
                {
                    cropped_frame = new Bitmap(input_bitmap.Width, true_bitmap_height);
                }
                else
                {
                    cropped_frame = new Bitmap(input_bitmap.Width, input_bitmap.Height);
                }

                // Create a graphics object and draw the input bitmap to fit inside the newly made shortened bitmap so it appears cropped.
                using (Graphics graphics = Graphics.FromImage(cropped_frame))
                {
                    graphics.DrawImage(input_bitmap, 0, 0, input_bitmap.Width, input_bitmap.Height);
                }

                // Return the final bitmap.
                return cropped_frame;
            }
            else if (frame_type == "Mouth")
            {
                // Find the height of the bitmap we should crop to.
                int true_bitmap_height = Find_True_Mouth_Frame_Height(input_bitmap);

                // Create a new bitmap using the width of the input bitmap and the true height value we just calculated.
                // If the "true height" calculated was 0 however, that means there was no empty space on the frame to crop.
                // In that case, the entire input bitmap can be copied over.
                if (true_bitmap_height != 0)
                {
                    cropped_frame = new Bitmap(input_bitmap.Width, true_bitmap_height);
                }
                else
                {
                    cropped_frame = new Bitmap(input_bitmap.Width, input_bitmap.Height);
                }

                // Create a graphics object and draw the input bitmap to fit inside the newly made shortened bitmap so it appears cropped.
                using (Graphics graphics = Graphics.FromImage(cropped_frame))
                {
                    graphics.DrawImage(input_bitmap, 0, -true_bitmap_height, input_bitmap.Width, input_bitmap.Height);
                }

                // Return the final bitmap.
                return cropped_frame;
            }

            return null;
        }

        public static int Find_True_Base_Bitmap_Height(Bitmap input_bitmap)
        {
            // Create an int value to return at the end of the function and initialize it to zero.
            // This will be our counter to store the Y value of where the base sprite truly starts.
            int counter = 0;

            // Create a variable for the X coordinate that is placed at the center of the bitmap.
            int x_coord = input_bitmap.Width / 2;

            // Create a color variable. We'll use this to analyze the color of the pixel being iterated over in a loop.
            System.Drawing.Color current_pixel;

            // Create a loop that starts at 0 and lasts until the input bitmap's height value.
            for (int i = 0; i < input_bitmap.Height; i++)
            {
                // Get the color of the pixel currently being iterated over.
                current_pixel = input_bitmap.GetPixel(x_coord, i);
                
                // Check if the pixel's alpha value is greater than ten.
                // This signifies that the contents of the frame have been reached and we are no longer in empty space.
                if (current_pixel.A > 10)
                {
                    // If so, assign the current 'i' value to the counter variable and break from the loop.
                    counter = i;
                    break;
                }
            }

            // Return the counter variable.
            return counter;
        }

        public static int Find_True_Eye_Frame_Height(Bitmap input_bitmap)
        {
            // Create an int value to return at the end of the function and initialize it to the height value of the bitmap.
            // This will be our counter to store the Y value of where the base sprite truly starts.
            int counter = input_bitmap.Height;

            // Create a variable for the X coordinate that is placed at the center of the bitmap.
            int x_coord = input_bitmap.Width / 2;

            // Create a color variable. We'll use this to analyze the color of the pixel being iterated over in a loop.
            System.Drawing.Color current_pixel;

            // Create a loop that starts at the bitmap's height value and decrements until zero.
            for (int i = input_bitmap.Height - 1; i > 0; i--)
            {
                // Get the color of the pixel currently being iterated over.
                current_pixel = input_bitmap.GetPixel(x_coord, i);

                // Check if the pixel's alpha value is greater than ten.
                // This signifies that the contents of the frame have been reached and we are no longer in empty space.
                if (current_pixel.A > 10)
                {
                    // If so, assign the current 'i' value to the counter variable and break from the loop.
                    counter = i;
                    break;
                }
            }

            // Return the counter variable.
            return counter;
        }

        public static int Find_True_Mouth_Frame_Height(Bitmap input_bitmap)
        {
            // Create an int value to return at the end of the function and initialize it to zero.
            // This will be our counter to store the Y value of where the base sprite truly starts.
            int counter = 0;

            // Create a variable for the X coordinate that is placed at the center of the bitmap.
            int x_coord = input_bitmap.Width / 2;

            // Create a color variable. We'll use this to analyze the color of the pixel being iterated over in a loop.
            System.Drawing.Color current_pixel;

            // Create a loop that starts at 0 and lasts until the input bitmap's height value.
            for (int i = 0; i < input_bitmap.Height; i++)
            {
                // Get the color of the pixel currently being iterated over.
                current_pixel = input_bitmap.GetPixel(x_coord, i);

                // Check if the pixel's alpha value is greater than ten.
                // This signifies that the contents of the frame have been reached and we are no longer in empty space.
                if (current_pixel.A > 10)
                {
                    // If so, assign the current 'i' value to the counter variable and break from the loop.
                    counter = i;
                    break;
                }
            }

            // Return the counter variable.
            return counter;
        }

        public static string Create_Sprite_Sheet_Footer(UserInfoFields account, OfficialSetData set_data)
        {
            // Create a string variable for text that will be displayed in the footer.
            string footer_text = "";

            // Deserialize the Title_Appearances field of the set data object into a string array.
            string[] appearances = Newtonsoft.Json.JsonConvert.DeserializeObject<string[]>(set_data.Title_Appearances);

            // If the character in the set appears in more than one game, add a "Version" section to the string variable.
            if (appearances.Length > 1)
            {
                footer_text += $"Version: {set_data.Origin}\n";
            }

            // ---------------------------------------------------------------------------------------

            // Finally, let's add a section that shows the user what other titles the character featured in the set is in.
            // First, let's again make sure the character in the set appears in more than one game.
            if (appearances.Length > 1)
            {
                // Create a new string list that we'll use to generate the user's displayed list of titles.
                List<string> displayed_appearances = new List<string>();

                // For each title with multiple versions, check if the sprite set comes from either version.
                // If it does, we want to leave those titles out of the list.
                // Otherwise, check if the character appears in both versions of a title.
                // If they do, add the generic game abbreviation for both versions to the string list.
                // If not, add the specific version the character appears in to the string list.

                // Persona 1
                if (set_data.Origin != "P1-PS1" && set_data.Origin != "P1-PSP")
                {
                    if (appearances.Contains("P1-PS1") && appearances.Contains("P1-PSP"))
                    {
                        displayed_appearances.Add("P1");
                    }
                    else
                    {
                        if (appearances.Contains("P1-PS1"))
                        {
                            displayed_appearances.Add("P1-PS1");
                        }

                        if (appearances.Contains("P1-PSP"))
                        {
                            displayed_appearances.Add("P1-PSP");
                        }
                    }
                }

                // Persona 2: Innocent Sin
                if (set_data.Origin != "P2IS-PS1" && set_data.Origin != "P2IS-PSP")
                {
                    if (appearances.Contains("P2IS-PS1") && appearances.Contains("P2IS-PSP"))
                    {
                        displayed_appearances.Add("P2IS");
                    }
                    else
                    {
                        if (appearances.Contains("P2IS-PS1"))
                        {
                            displayed_appearances.Add("P2IS-PS1");
                        }

                        if (appearances.Contains("P2IS-PSP"))
                        {
                            displayed_appearances.Add("P2IS-PSP");
                        }
                    }
                }

                // Persona 2: Eternal Punishment
                if (set_data.Origin != "P2EP-PS1" && set_data.Origin != "P2EP-PSP")
                {
                    if (appearances.Contains("P2EP-PS1") && appearances.Contains("P2EP-PSP"))
                    {
                        displayed_appearances.Add("P2EP");
                    }
                    else
                    {
                        if (appearances.Contains("P2EP-PS1"))
                        {
                            displayed_appearances.Add("P2EP-PS1");
                        }

                        if (appearances.Contains("P2EP-PSP"))
                        {
                            displayed_appearances.Add("P2EP-PSP");
                        }
                    }
                }

                // Persona 3
                if (set_data.Origin != "P3F" && set_data.Origin != "P3P" && set_data.Origin != "P3R")
                {
                    if (appearances.Contains("P3F") && appearances.Contains("P3P") && appearances.Contains("P3R"))
                    {
                        displayed_appearances.Add("P3");
                    }
                    else
                    {
                        if (appearances.Contains("P3F"))
                        {
                            displayed_appearances.Add("P3F");
                        }

                        if (appearances.Contains("P3P"))
                        {
                            displayed_appearances.Add("P3P");
                        }

                        if (appearances.Contains("P3R"))
                        {
                            displayed_appearances.Add("P3R");
                        }
                    }
                }

                // Persona 4
                if (set_data.Origin != "P4-PS2" && set_data.Origin != "P4G")
                {
                    if (appearances.Contains("P4-PS2") && appearances.Contains("P4G"))
                    {
                        displayed_appearances.Add("P4");
                    }
                    else
                    {
                        if (appearances.Contains("P4-PS2"))
                        {
                            displayed_appearances.Add("P4-PS2");
                        }

                        if (appearances.Contains("P4G"))
                        {
                            displayed_appearances.Add("P4G");
                        }
                    }
                }

                // Persona 4 Arena Ultimax
                if (appearances.Contains("P4AU") && set_data.Origin != "P4AU")
                {
                    displayed_appearances.Add("P4AU");
                }

                // Persona 4: Dancing All Night
                if (appearances.Contains("P4D") && set_data.Origin != "P4D")
                {
                    displayed_appearances.Add("P4D");
                }

                // Persona 5
                if (set_data.Origin != "P5-PS4" && set_data.Origin != "P5R")
                {
                    if (appearances.Contains("P5-PS4") && appearances.Contains("P5R"))
                    {
                        displayed_appearances.Add("P5");
                    }
                    else
                    {
                        if (appearances.Contains("P5-PS4"))
                        {
                            displayed_appearances.Add("P5-PS4");
                        }

                        if (appearances.Contains("P5R"))
                        {
                            displayed_appearances.Add("P5R");
                        }
                    }
                }

                // Persona 5 Strikers
                if (appearances.Contains("P5S") && set_data.Origin != "P5S")
                {
                    displayed_appearances.Add("P5S");
                }

                // BlazBlue: Cross Tag Battle
                if (appearances.Contains("BBTAG") && set_data.Origin != "BBTAG")
                {
                    displayed_appearances.Add("BBTAG");
                }

                // Now that we've iterated through all titles, it's time to construct our final string.
                // Start a new section of the footer text for listing other titles the character appeared in.
                if (displayed_appearances.Count != 0)
                {
                    footer_text += "Other Appearances: ";
                }

                // Iterate through each index of the formed string list.
                for (int i = 0; i < displayed_appearances.Count; i++)
                {
                    // Add the contents of the currently iterated index to the footer text.
                    footer_text += displayed_appearances[i];

                    // If the current index is not the last one in the list, add a comma and space to separate the next entry.
                    if (i != displayed_appearances.Count - 1)
                    {
                        footer_text += ", ";
                    }
                }
            }

            return footer_text;
        }

        // Methods made specifically for P3R
        public static Bitmap Generate_P3R_Bustup_Frame_Sheet(SocialLinkerCommand sl_command, OfficialSetData set_data, MakerCommandData maker_command_data)
        {
            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(sl_command.User);

            // Establish the directory of the specified sprite set.
            string set_path_raw = $@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//{set_data.Origin}//Bustup//{set_data.ID}";
            string set_path_preview = $@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//{set_data.Origin}//Bustup_Preview//{set_data.ID}";

            // Using the sprite set's path, create path variables for Eye and Mouth frame folders.
            // These paths aren't guaranteed to exist, but we'll handle that validity check later.
            string eye_frame_path_raw = $@"{set_path_raw}//Eyes";
            string mouth_frame_path_raw = $@"{set_path_raw}//Mouth";

            string eye_frame_path_preview = $@"{set_path_preview}//Eyes";
            string mouth_frame_path_preview = $@"{set_path_preview}//Mouth";

            // Create a filename for the bitmap that will be generated.
            var fileName = $"{sl_command.User.Id}_{DateTime.UtcNow.ToString("yyyyMMdd_HH_mm_ss_fff")}.png";

            // Get a count of how many files are in the sprite set's directory.
            int bustup_filecount = OfficialSetMethods.AttachmentCountItemDirectory(set_path_raw);
            int frame_filecount = Directory.EnumerateFiles(eye_frame_path_raw).Where(f => f.Contains("e1")).Count();

            // Create a variable for the base sprite's filename. We'll go searching for it in a few moments.
            string base_sprite_filename = "";

            // Create variables that keep track of how many frames of each frame type are present for the base sprite.
            // This will help us perform the math needed to form the sprite sheet.
            int eye_frame_count = 0;
            int mouth_frame_count = 0;

            char[] poses = new char[] { 'a', 'b', 'c', 'd', 'p'};

            string frame_filename_specific = "";
            string frame_filename_generic = "";

            // Check if the sprite set's directory exists.
            base_sprite_filename = OfficialSetMethods.Get_P3R_Bustup_Filename_From_Sprite_Number(sl_command, set_data, maker_command_data);

            // At this point, we should have the file name for the base sprite.
            // Check if the path for the set's eye frames exists.
            // Not all sets will have eye frames, so this block doesn't always need to execute.
            if (Directory.Exists(eye_frame_path_raw))
            {
                // Get a count of how many files are in the sprite set's directory.
                bustup_filecount = OfficialSetMethods.AttachmentCountItemDirectory(eye_frame_path_raw);

                // Form an array of file names for the sprite's eye frames
                string[] allFiles = { };

                if (File.Exists($"{eye_frame_path_raw}//{frame_filename_specific}_e1.png"))
                {
                    allFiles = Directory.GetFiles(eye_frame_path_raw, $"{frame_filename_specific}_e*.png");
                }
                else if (File.Exists($"{eye_frame_path_raw}//{frame_filename_generic}_e1.png"))
                {
                    allFiles = Directory.GetFiles(eye_frame_path_raw, $"{frame_filename_generic}_e*.png");
                }

                // Assign the array length to the eye frame count
                eye_frame_count = allFiles.Length;
            }

            // Check if the path for the set's mouth frames exists.
            // Not all sets will have mouth frames, so this block doesn't always need to execute.
            if (Directory.Exists(mouth_frame_path_raw))
            {
                // Get a count of how many files are in the sprite set's directory.
                bustup_filecount = OfficialSetMethods.AttachmentCountItemDirectory(mouth_frame_path_raw);

                // Form an array of file names for the sprite's mouth frames
                string[] allFiles = { };

                if (File.Exists($"{mouth_frame_path_raw}//{frame_filename_specific}_m1.png"))
                {
                    allFiles = Directory.GetFiles(mouth_frame_path_raw, $"{frame_filename_specific}_m*.png");
                }
                else if (File.Exists($"{mouth_frame_path_raw}//{frame_filename_generic}_m1.png"))
                {
                    allFiles = Directory.GetFiles(mouth_frame_path_raw, $"{frame_filename_generic}_m*.png");
                }

                // Assign the array length to the mouth frame count.
                mouth_frame_count = allFiles.Length;
            }

            // Time to put together the final bitmap! Create the base that the other layers will go on.
            Bitmap base_template = new Bitmap(1000, 1000);

            // Here, start drawing on the base bitmap.
            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                // Depending on what types and how many frames the sprite has, we want to render the frame sheet in different manners.
                // Check for the case that the sprite has no eye or mouth frames.
                if (eye_frame_count == 0 && mouth_frame_count == 0)
                {
                    // Since there are no frames, create a bustup section that fills the entire frame sheet.
                    Bitmap bustup_section = Create_Base_Bustup_Bitmap((Bitmap)System.Drawing.Image.FromFile($"{set_path_preview}//{base_sprite_filename}.png"), set_data, 0);

                    // Draw the bustup section to the base template.
                    graphics.DrawImage(bustup_section, 100, 0, bustup_section.Width, bustup_section.Height);
                }
                // Check for the case that the sprite has eye frames AND mouth frames.
                else if (eye_frame_count > 0 && mouth_frame_count > 0)
                {
                    // Create a bustup section that accounts for frames of both types present on the sheet.
                    Bitmap bustup_section = Create_Base_Bustup_Bitmap((Bitmap)System.Drawing.Image.FromFile($"{set_path_preview}//{base_sprite_filename}.png"), set_data, 2);

                    // Draw the bustup section to the base template.
                    graphics.DrawImage(bustup_section, 100, 0, bustup_section.Width, bustup_section.Height);

                    // Create two lists of bitmaps to contain the frames for the sprite.
                    List<Bitmap> eye_frame_list = new List<Bitmap>();
                    List<Bitmap> mouth_frame_list = new List<Bitmap>();

                    // Add each eye frame for the sprite to the list.
                    // This assumes the file names are correct while iterating to add them instantly.
                    for (int i = 0; i < eye_frame_count; i++)
                    {
                        if (File.Exists($"{eye_frame_path_preview}//{frame_filename_specific}_e{i + 1}.png"))
                        {
                            eye_frame_list.Add((Bitmap)System.Drawing.Image.FromFile($"{eye_frame_path_preview}//{frame_filename_specific}_e{i + 1}.png"));
                        }
                        else if (File.Exists($"{eye_frame_path_preview}//{frame_filename_generic}_e{i + 1}.png"))
                        {
                            eye_frame_list.Add((Bitmap)System.Drawing.Image.FromFile($"{eye_frame_path_preview}//{frame_filename_generic}_e{i + 1}.png"));
                        }
                    }

                    // Add each mouth frame for the sprite to the list.
                    // This assumes the file names are correct while iterating to add them instantly.
                    for (int i = 0; i < mouth_frame_count; i++)
                    {
                        if (File.Exists($"{mouth_frame_path_preview}//{frame_filename_specific}_m{i + 1}.png"))
                        {
                            mouth_frame_list.Add((Bitmap)System.Drawing.Image.FromFile($"{mouth_frame_path_preview}//{frame_filename_specific}_m{i + 1}.png"));
                        }
                        else if (File.Exists($"{mouth_frame_path_preview}//{frame_filename_generic}_m{i + 1}.png"))
                        {
                            mouth_frame_list.Add((Bitmap)System.Drawing.Image.FromFile($"{mouth_frame_path_preview}//{frame_filename_generic}_m{i + 1}.png"));
                        }
                    }

                    // Create frame bitmaps for the eye and mouth sections.
                    Bitmap eye_frame_section = Create_Standard_Frame_Bitmap(eye_frame_list.ToArray());
                    Bitmap mouth_frame_section = Create_Standard_Frame_Bitmap(mouth_frame_list.ToArray());

                    // Draw the frame bitmaps to the base template.
                    graphics.DrawImage(eye_frame_section, 100, 400, eye_frame_section.Width, eye_frame_section.Height);
                    graphics.DrawImage(mouth_frame_section, 100, 700, mouth_frame_section.Width, mouth_frame_section.Height);
                }
                // Check for the case that the sprite either has eye frames but no mouth frames, or mouth frames but no eye frames.
                else if (eye_frame_count > 0 || mouth_frame_count > 0)
                {
                    // Create a bustup section that accounts for one type of frame present on the sheet.
                    Bitmap bustup_section = Create_Base_Bustup_Bitmap((Bitmap)System.Drawing.Image.FromFile($"{set_path_preview}//{base_sprite_filename}.png"), set_data, 1);
                    graphics.DrawImage(bustup_section, 100, 0, bustup_section.Width, bustup_section.Height);

                    // Now, let's check for which frame type does exist for the sprite.
                    // If any eye frames exist, create an eye frame panel.
                    if (eye_frame_count > 0)
                    {
                        List<Bitmap> eye_frame_list = new List<Bitmap>();

                        for (int i = 0; i < eye_frame_count; i++)
                        {
                            if (File.Exists($"{eye_frame_path_preview}//{frame_filename_specific}_e{i + 1}.png"))
                            {
                                eye_frame_list.Add((Bitmap)System.Drawing.Image.FromFile($"{eye_frame_path_preview}//{frame_filename_specific}_e{i + 1}.png"));
                            }
                            else if (File.Exists($"{eye_frame_path_preview}//{frame_filename_generic}_e{i + 1}.png"))
                            {
                                eye_frame_list.Add((Bitmap)System.Drawing.Image.FromFile($"{eye_frame_path_preview}//{frame_filename_generic}_e{i + 1}.png"));
                            }
                        }

                        Bitmap eye_frame_section = Create_Standard_Frame_Bitmap(eye_frame_list.ToArray());
                        graphics.DrawImage(eye_frame_section, 100, 700, eye_frame_section.Width, eye_frame_section.Height);
                    }
                    // If any mouth frames exist, create a mouth frame panel.
                    else if (mouth_frame_count > 0)
                    {
                        List<Bitmap> mouth_frame_list = new List<Bitmap>();

                        for (int i = 0; i < mouth_frame_count; i++)
                        {
                            if (File.Exists($"{mouth_frame_path_preview}//{frame_filename_specific}_m{i + 1}.png"))
                            {
                                mouth_frame_list.Add((Bitmap)System.Drawing.Image.FromFile($"{mouth_frame_path_preview}//{frame_filename_specific}_m{i + 1}.png"));
                            }
                            else if (File.Exists($"{mouth_frame_path_preview}//{frame_filename_generic}_m{i + 1}.png"))
                            {
                                mouth_frame_list.Add((Bitmap)System.Drawing.Image.FromFile($"{mouth_frame_path_preview}//{frame_filename_generic}_m{i + 1}.png"));
                            }
                        }

                        Bitmap mouth_frame_section = Create_Standard_Frame_Bitmap(mouth_frame_list.ToArray());
                        graphics.DrawImage(mouth_frame_section, 100, 700, mouth_frame_section.Width, mouth_frame_section.Height);
                    }
                }

                // Now, we should create an overlay that will assist the user's viewing of the frame sheet.
                // Create a "black bar" bitmap that will contain information on the side of the frame sheet.
                Bitmap black_bar = new Bitmap(100, 1000);

                // We'll also want to create a "white bar" bitmap to separate sections on the frame sheet.
                Bitmap white_bar = new Bitmap(1000, 6);

                // Fill the black_bar bitmap with the color black.
                using (Graphics overlay_object = Graphics.FromImage(black_bar))
                {
                    overlay_object.Clear(System.Drawing.Color.Black);
                }

                // Fill the white_bar bitmap with the color white.
                using (Graphics overlay_object = Graphics.FromImage(white_bar))
                {
                    overlay_object.Clear(System.Drawing.Color.White);
                }

                // Draw the black bar to the base template. We'll handle drawing the white bars later.
                graphics.DrawImage(black_bar, 0, 0, black_bar.Width, black_bar.Height);

                // Now, let's start rendering text for user readability.
                //Set text rendering to have antialiasing.
                graphics.TextRenderingHint = TextRenderingHint.AntiAlias;

                // Create three rectangle variables to represent text boxes that may be rendered to the template.
                // Depending on the frames available, only one or two of these variables may be used.
                Rectangle base_text_box = new Rectangle(0, 0, 0, 0);
                Rectangle eyes_text_box = new Rectangle(0, 0, 0, 0);
                Rectangle mouth_text_box = new Rectangle(0, 0, 0, 0);

                // Create a font object to draw text to the base template.
                using (Font frame_font = new Font("Eurostar Black Extended", 35))
                {
                    // Format strings so that their placement is at the center of the text box.
                    StringFormat stringFormat = new StringFormat();
                    stringFormat.Alignment = StringAlignment.Center;
                    stringFormat.LineAlignment = StringAlignment.Center;

                    // Check if the number of eye and mouth frames present is zero.
                    if (eye_frame_count == 0 && mouth_frame_count == 0)
                    {
                        // Rotate the graphics object by -90 degrees so that the text will appear on its side.
                        graphics.RotateTransform(-90);

                        // Redefine the text box for the base sprite to take up the entire sheet since there will be no frames to show and draw the text to the template.
                        // The X coordinate starts at -1000 to compensate for the -90 degree rotation.
                        base_text_box = new Rectangle(-1000 + 0, 0, 1000, 100);
                        graphics.DrawString("NO FRAMES", frame_font, System.Drawing.Brushes.White, base_text_box, stringFormat);
                    }
                    // Check if there are both eye frames and mouth frames present.
                    else if (eye_frame_count > 0 && mouth_frame_count > 0)
                    {
                        // Draw two white bars to the template so they will appear as dividers for each section.
                        graphics.DrawImage(white_bar, 0, 397, white_bar.Width, white_bar.Height);
                        graphics.DrawImage(white_bar, 0, 697, white_bar.Width, white_bar.Height);

                        // Rotate the graphics object by -90 degrees so that the text will appear on its side.
                        graphics.RotateTransform(-90);

                        // Redefine the text boxes for the base, eye, and mouth sections and draw their text to the template.
                        // The X coordinates start at -1000 to compensate for the -90 degree rotation.
                        base_text_box = new Rectangle(-1000 + 600, 0, 400, 100);
                        graphics.DrawString("BASE", frame_font, System.Drawing.Brushes.White, base_text_box, stringFormat);

                        eyes_text_box = new Rectangle(-1000 + 300, 0, 300, 100);
                        graphics.DrawString("EYES", frame_font, System.Drawing.Brushes.White, eyes_text_box, stringFormat);

                        mouth_text_box = new Rectangle(-1000 + 0, 0, 300, 100);
                        graphics.DrawString("MOUTH", frame_font, System.Drawing.Brushes.White, mouth_text_box, stringFormat);
                    }
                    // Check if there are either eye frames OR mouth frames present.
                    else if (eye_frame_count > 0 || mouth_frame_count > 0)
                    {
                        // Draw a white bar to the template so it will appear as a divider for each section.
                        graphics.DrawImage(white_bar, 0, 697, white_bar.Width, white_bar.Height);

                        // Rotate the graphics object by -90 degrees so that the text will appear on its side.
                        graphics.RotateTransform(-90);

                        // Redefine the text box for the base sprite and draw the text for it to the template.
                        // The X coordinates start at -1000 to compensate for the -90 degree rotation.
                        base_text_box = new Rectangle(-1000 + 300, 0, 700, 100);
                        graphics.DrawString("BASE", frame_font, System.Drawing.Brushes.White, base_text_box, stringFormat);

                        // Since we've confirmed there are either eye frames or mouth frames present, do a comparison to see which type it is and draw the appropriate text to the template.
                        // The X coordinates start at -1000 to compensate for the -90 degree rotation.
                        if (eye_frame_count > 0)
                        {
                            eyes_text_box = new Rectangle(-1000 + 0, 0, 300, 100);
                            graphics.DrawString("EYES", frame_font, System.Drawing.Brushes.White, eyes_text_box, stringFormat);
                        }
                        else if (mouth_frame_count > 0)
                        {
                            mouth_text_box = new Rectangle(-1000 + 0, 0, 300, 100);
                            graphics.DrawString("MOUTH", frame_font, System.Drawing.Brushes.White, mouth_text_box, stringFormat);
                        }
                    }
                }
            }

            // Return the base template.
            return base_template;
        }

        // Animation frame messages
        public static async Task P1_PS1_Bustup_Frame_Sheet(SocialLinkerCommand sl_command, OfficialSetData set_data, MakerCommandData maker_command_data)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = sl_command.User;
            SocketTextChannel channel = (SocketTextChannel)sl_command.Channel;

            // Send a loading message to the channel while the sprite sheet is being made.
            RestUserMessage loader = await channel.SendMessageAsync("", false, P1_PS1_Loading_Message().Build());

            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = $"{set_data.Name}'s Animation Frames - Portrait #{maker_command_data.Character_Data_1.Base_Sprite}",
                IconUrl = EmbedSettings.Get_Game_Logo("P1-PS1")
            };

            embed.WithAuthor(author);

            // Set the color and thumbnail for the embeded message.
            embed.WithColor(EmbedSettings.Get_Game_Color("P1-PS1", null));

            // Create a footer based on the user's settings.
            var footer = new EmbedFooterBuilder
            {
                Text = Create_Sprite_Sheet_Footer(account, set_data)
            };

            // Add the footer to the embed.
            embed.WithFooter(footer);

            // Attach a locally generated image to the embed. This image hasn't been created yet, so the filename is just a placeholder for now.
            embed.WithImageUrl($"attachment://preview.png");

            // Create a new stream. We'll use this to create the locally generated image.
            MemoryStream memoryStream = new MemoryStream();

            // Generate a bitmap comprised of the base sprite chosen and any animation frames it may have.
            Bitmap sprite_set_preview = Generate_Standard_Bustup_Frame_Sheet(sl_command, set_data, maker_command_data.Character_Data_1);

            // Save the sprite set preview bitmap to the stream as a PNG.
            sprite_set_preview.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);

            // Ensure the stream is set to the beginning of itself.
            memoryStream.Seek(0, SeekOrigin.Begin);

            // Send the embeded message to the channel.
            try
            {
                await sl_command.Channel.SendFileAsync(memoryStream, "preview.png", "", false, embed.Build());
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
        }

        public static async Task P1_PSP_Bustup_Frame_Sheet(SocialLinkerCommand sl_command, OfficialSetData set_data, MakerCommandData maker_command_data)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = sl_command.User;
            SocketTextChannel channel = (SocketTextChannel)sl_command.Channel;

            // Send a loading message to the channel while the sprite sheet is being made.
            RestUserMessage loader = await channel.SendMessageAsync("", false, P1_PSP_Loading_Message().Build());

            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = $"{set_data.Name}'s Animation Frames - Portrait #{maker_command_data.Character_Data_1.Base_Sprite}",
                IconUrl = EmbedSettings.Get_Game_Logo("P1-PSP")
            };

            embed.WithAuthor(author);

            // Set the color and thumbnail for the embeded message.
            embed.WithColor(EmbedSettings.Get_Game_Color("P1-PSP", null));

            // Create a footer based on the user's settings.
            var footer = new EmbedFooterBuilder
            {
                Text = Create_Sprite_Sheet_Footer(account, set_data)
            };

            // Add the footer to the embed.
            embed.WithFooter(footer);

            // Attach a locally generated image to the embed. This image hasn't been created yet, so the filename is just a placeholder for now.
            embed.WithImageUrl($"attachment://preview.png");

            // Create a new stream. We'll use this to create the locally generated image.
            MemoryStream memoryStream = new MemoryStream();

            // Generate a bitmap comprised of the base sprite chosen and any animation frames it may have.
            Bitmap sprite_set_preview = Generate_Standard_Bustup_Frame_Sheet(sl_command, set_data, maker_command_data.Character_Data_1);

            // Save the sprite set preview bitmap to the stream as a PNG.
            sprite_set_preview.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);

            // Ensure the stream is set to the beginning of itself.
            memoryStream.Seek(0, SeekOrigin.Begin);

            // Send the embeded message to the channel.
            try
            {
                await sl_command.Channel.SendFileAsync(memoryStream, "preview.png", "", false, embed.Build());
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
        }

        public static async Task P2IS_PS1_Bustup_Frame_Sheet(SocialLinkerCommand sl_command, OfficialSetData set_data, MakerCommandData maker_command_data)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = sl_command.User;
            SocketTextChannel channel = (SocketTextChannel)sl_command.Channel;

            // Send a loading message to the channel while the sprite sheet is being made.
            RestUserMessage loader = await channel.SendMessageAsync("", false, P2IS_PS1_Loading_Message().Build());

            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = $"{set_data.Name}'s Animation Frames - Portrait #{maker_command_data.Character_Data_1.Base_Sprite}",
                IconUrl = EmbedSettings.Get_Game_Logo("P2IS-PS1")
            };

            embed.WithAuthor(author);

            // Set the color for the embeded message.
            embed.WithColor(EmbedSettings.Get_Game_Color("P2IS-PS1", null));

            // Create a footer based on the user's settings.
            var footer = new EmbedFooterBuilder
            {
                Text = Create_Sprite_Sheet_Footer(account, set_data)
            };

            // Add the footer to the embed.
            embed.WithFooter(footer);

            // Attach a locally generated image to the embed. This image hasn't been created yet, so the filename is just a placeholder for now.
            embed.WithImageUrl($"attachment://preview.png");

            // Create a new stream. We'll use this to create the locally generated image.
            MemoryStream memoryStream = new MemoryStream();

            // Generate a bitmap comprised of the base sprite chosen and any animation frames it may have.
            Bitmap sprite_set_preview = Generate_Standard_Bustup_Frame_Sheet(sl_command, set_data, maker_command_data.Character_Data_1);

            // Save the sprite set preview bitmap to the stream as a PNG.
            sprite_set_preview.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);

            // Ensure the stream is set to the beginning of itself.
            memoryStream.Seek(0, SeekOrigin.Begin);

            // Send the embeded message to the channel.
            try
            {
                await sl_command.Channel.SendFileAsync(memoryStream, "preview.png", "", false, embed.Build());
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
        }

        public static async Task P2IS_PSP_Bustup_Frame_Sheet(SocialLinkerCommand sl_command, OfficialSetData set_data, MakerCommandData maker_command_data)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = sl_command.User;
            SocketTextChannel channel = (SocketTextChannel)sl_command.Channel;

            // Send a loading message to the channel while the sprite sheet is being made.
            RestUserMessage loader = await channel.SendMessageAsync("", false, P2IS_PSP_Loading_Message().Build());

            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = $"{set_data.Name}'s Animation Frames - Portrait #{maker_command_data.Character_Data_1.Base_Sprite}",
                IconUrl = EmbedSettings.Get_Game_Logo("P2IS-PSP")
            };

            embed.WithAuthor(author);

            // Set the color for the embeded message.
            embed.WithColor(EmbedSettings.Get_Game_Color("P2IS-PSP", null));

            // Create a footer based on the user's settings.
            var footer = new EmbedFooterBuilder
            {
                Text = Create_Sprite_Sheet_Footer(account, set_data)
            };

            // Add the footer to the embed.
            embed.WithFooter(footer);

            // Attach a locally generated image to the embed. This image hasn't been created yet, so the filename is just a placeholder for now.
            embed.WithImageUrl($"attachment://preview.png");

            // Create a new stream. We'll use this to create the locally generated image.
            MemoryStream memoryStream = new MemoryStream();

            // Generate a bitmap comprised of the base sprite chosen and any animation frames it may have.
            Bitmap sprite_set_preview = Generate_Standard_Bustup_Frame_Sheet(sl_command, set_data, maker_command_data.Character_Data_1);

            // Save the sprite set preview bitmap to the stream as a PNG.
            sprite_set_preview.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);

            // Ensure the stream is set to the beginning of itself.
            memoryStream.Seek(0, SeekOrigin.Begin);

            // Send the embeded message to the channel.
            try
            {
                await sl_command.Channel.SendFileAsync(memoryStream, "preview.png", "", false, embed.Build());
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
        }

        public static async Task P2EP_PS1_Bustup_Frame_Sheet(SocialLinkerCommand sl_command, OfficialSetData set_data, MakerCommandData maker_command_data)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = sl_command.User;
            SocketTextChannel channel = (SocketTextChannel)sl_command.Channel;

            // Send a loading message to the channel while the sprite sheet is being made.
            RestUserMessage loader = await channel.SendMessageAsync("", false, P2EP_PS1_Loading_Message().Build());

            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = $"{set_data.Name}'s Animation Frames - Portrait #{maker_command_data.Character_Data_1.Base_Sprite}",
                IconUrl = EmbedSettings.Get_Game_Logo("P2EP-PS1")
            };

            embed.WithAuthor(author);

            // Set the color for the embeded message.
            embed.WithColor(EmbedSettings.Get_Game_Color("P2EP-PS1", null));

            // Create a footer based on the user's settings.
            var footer = new EmbedFooterBuilder
            {
                Text = Create_Sprite_Sheet_Footer(account, set_data)
            };

            // Add the footer to the embed.
            embed.WithFooter(footer);

            // Attach a locally generated image to the embed. This image hasn't been created yet, so the filename is just a placeholder for now.
            embed.WithImageUrl($"attachment://preview.png");

            // Create a new stream. We'll use this to create the locally generated image.
            MemoryStream memoryStream = new MemoryStream();

            // Generate a bitmap comprised of the base sprite chosen and any animation frames it may have.
            Bitmap sprite_set_preview = Generate_Standard_Bustup_Frame_Sheet(sl_command, set_data, maker_command_data.Character_Data_1);

            // Save the sprite set preview bitmap to the stream as a PNG.
            sprite_set_preview.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);

            // Ensure the stream is set to the beginning of itself.
            memoryStream.Seek(0, SeekOrigin.Begin);

            // Send the embeded message to the channel.
            try
            {
                await sl_command.Channel.SendFileAsync(memoryStream, "preview.png", "", false, embed.Build());
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
        }

        public static async Task P2EP_PSP_Bustup_Frame_Sheet(SocialLinkerCommand sl_command, OfficialSetData set_data, MakerCommandData maker_command_data)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = sl_command.User;
            SocketTextChannel channel = (SocketTextChannel)sl_command.Channel;

            // Send a loading message to the channel while the sprite sheet is being made.
            RestUserMessage loader = await channel.SendMessageAsync("", false, P2EP_PSP_Loading_Message().Build());

            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = $"{set_data.Name}'s Animation Frames - Portrait #{maker_command_data.Character_Data_1.Base_Sprite}",
                IconUrl = EmbedSettings.Get_Game_Logo("P2EP-PSP")
            };

            embed.WithAuthor(author);

            // Set the color for the embeded message.
            embed.WithColor(EmbedSettings.Get_Game_Color("P2EP-PSP", null));

            // Create a footer based on the user's settings.
            var footer = new EmbedFooterBuilder
            {
                Text = Create_Sprite_Sheet_Footer(account, set_data)
            };

            // Add the footer to the embed.
            embed.WithFooter(footer);

            // Attach a locally generated image to the embed. This image hasn't been created yet, so the filename is just a placeholder for now.
            embed.WithImageUrl($"attachment://preview.png");

            // Create a new stream. We'll use this to create the locally generated image.
            MemoryStream memoryStream = new MemoryStream();

            // Generate a bitmap comprised of the base sprite chosen and any animation frames it may have.
            Bitmap sprite_set_preview = Generate_Standard_Bustup_Frame_Sheet(sl_command, set_data, maker_command_data.Character_Data_1);

            // Save the sprite set preview bitmap to the stream as a PNG.
            sprite_set_preview.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);

            // Ensure the stream is set to the beginning of itself.
            memoryStream.Seek(0, SeekOrigin.Begin);

            // Send the embeded message to the channel.
            try
            {
                await sl_command.Channel.SendFileAsync(memoryStream, "preview.png", "", false, embed.Build());
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
        }

        public static async Task P3F_Bustup_Frame_Sheet(SocialLinkerCommand sl_command, OfficialSetData set_data, MakerCommandData maker_command_data)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = sl_command.User;
            SocketTextChannel channel = (SocketTextChannel)sl_command.Channel;

            // Send a loading message to the channel while the sprite sheet is being made.
            RestUserMessage loader = await channel.SendMessageAsync("", false, P3F_Loading_Message().Build());

            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = $"{set_data.Name}'s Animation Frames - Portrait #{maker_command_data.Character_Data_1.Base_Sprite}",
                IconUrl = EmbedSettings.Get_Game_Logo("P3F")
            };

            embed.WithAuthor(author);

            // Set the color and thumbnail for the embeded message.
            embed.WithColor(EmbedSettings.Get_Game_Color("P3F", null));

            // Create a footer based on the user's settings.
            var footer = new EmbedFooterBuilder
            {
                Text = Create_Sprite_Sheet_Footer(account, set_data)
            };

            // Add the footer to the embed.
            embed.WithFooter(footer);

            // Attach a locally generated image to the embed. This image hasn't been created yet, so the filename is just a placeholder for now.
            embed.WithImageUrl($"attachment://preview.png");

            // Create a new stream. We'll use this to create the locally generated image.
            MemoryStream memoryStream = new MemoryStream();

            // Generate a bitmap comprised of the base sprite chosen and any animation frames it may have.
            Bitmap sprite_set_preview = Generate_Standard_Bustup_Frame_Sheet(sl_command, set_data, maker_command_data.Character_Data_1);

            // Save the sprite set preview bitmap to the stream as a PNG.
            sprite_set_preview.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);

            // Ensure the stream is set to the beginning of itself.
            memoryStream.Seek(0, SeekOrigin.Begin);

            // Send the embeded message to the channel.
            try
            {
                await sl_command.Channel.SendFileAsync(memoryStream, "preview.png", "", false, embed.Build());
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
        }

        public static async Task P3R_Bustup_Frame_Sheet(SocialLinkerCommand sl_command, OfficialSetData set_data, MakerCommandData maker_command_data)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = sl_command.User;
            SocketTextChannel channel = (SocketTextChannel)sl_command.Channel;

            // Send a loading message to the channel while the sprite sheet is being made.
            //RestUserMessage loader = await channel.SendMessageAsync("", false, P3F_Loading_Message().Build());
            RestUserMessage loader = await channel.SendMessageAsync("Loading...");

            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = $"{set_data.Name}'s Animation Frames - Portrait #{maker_command_data.Character_Data_1.Base_Sprite}",
                IconUrl = EmbedSettings.Get_Game_Logo("P3R")
            };

            embed.WithAuthor(author);

            // Set the color and thumbnail for the embeded message.
            embed.WithColor(EmbedSettings.Get_Game_Color("P3R", null));

            // Create a footer based on the user's settings.
            var footer = new EmbedFooterBuilder
            {
                Text = Create_Sprite_Sheet_Footer(account, set_data)
            };

            // Add the footer to the embed.
            embed.WithFooter(footer);

            // Attach a locally generated image to the embed. This image hasn't been created yet, so the filename is just a placeholder for now.
            embed.WithImageUrl($"attachment://preview.png");

            // Create a new stream. We'll use this to create the locally generated image.
            MemoryStream memoryStream = new MemoryStream();

            // Generate a bitmap comprised of the base sprite chosen and any animation frames it may have.
            Bitmap sprite_set_preview = Generate_P3R_Bustup_Frame_Sheet(sl_command, set_data, maker_command_data);

            // Save the sprite set preview bitmap to the stream as a PNG.
            sprite_set_preview.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);

            // Ensure the stream is set to the beginning of itself.
            memoryStream.Seek(0, SeekOrigin.Begin);

            // Send the embeded message to the channel.
            try
            {
                await sl_command.Channel.SendFileAsync(memoryStream, "preview.png", "", false, embed.Build());
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
        }

        public static async Task P3P_Bustup_Frame_Sheet(SocialLinkerCommand sl_command, OfficialSetData set_data, MakerCommandData maker_command_data)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = sl_command.User;
            SocketTextChannel channel = (SocketTextChannel)sl_command.Channel;

            // Send a loading message to the channel while the sprite sheet is being made.
            RestUserMessage loader = await channel.SendMessageAsync("", false, P3P_Loading_Message(sl_command).Build());

            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = $"{set_data.Name}'s Animation Frames - Portrait #{maker_command_data.Character_Data_1.Base_Sprite}",
                IconUrl = EmbedSettings.Get_Game_Logo("P3P")
            };

            embed.WithAuthor(author);

            // Assign a color based on the user's color setting for the P3P template.
            embed.WithColor(EmbedSettings.Get_Game_Color("P3P", account));

            // Create a footer based on the user's settings.
            var footer = new EmbedFooterBuilder
            {
                Text = Create_Sprite_Sheet_Footer(account, set_data)
            };

            // Add the footer to the embed.
            embed.WithFooter(footer);

            // Attach a locally generated image to the embed. This image hasn't been created yet, so the filename is just a placeholder for now.
            embed.WithImageUrl($"attachment://preview.png");

            // Create a new stream. We'll use this to create the locally generated image.
            MemoryStream memoryStream = new MemoryStream();

            // Generate a bitmap comprised of the base sprite chosen and any animation frames it may have.
            Bitmap sprite_set_preview = Generate_Standard_Bustup_Frame_Sheet(sl_command, set_data, maker_command_data.Character_Data_1);

            // Save the sprite set preview bitmap to the stream as a PNG.
            sprite_set_preview.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);

            // Ensure the stream is set to the beginning of itself.
            memoryStream.Seek(0, SeekOrigin.Begin);

            // Send the embeded message to the channel.
            try
            {
                await sl_command.Channel.SendFileAsync(memoryStream, "preview.png", "", false, embed.Build());
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
        }

        public static async Task P4_PS2_Bustup_Frame_Sheet(SocialLinkerCommand sl_command, OfficialSetData set_data, MakerCommandData maker_command_data)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = sl_command.User;
            SocketTextChannel channel = (SocketTextChannel)sl_command.Channel;

            // Send a loading message to the channel while the sprite sheet is being made.
            RestUserMessage loader = await channel.SendMessageAsync("", false, P4_PS2_Loading_Message().Build());

            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = $"{set_data.Name}'s Animation Frames - Portrait #{maker_command_data.Character_Data_1.Base_Sprite}",
                IconUrl = EmbedSettings.Get_Game_Logo("P4-PS2")
            };

            embed.WithAuthor(author);

            // Set the color and thumbnail for the embeded message.
            embed.WithColor(EmbedSettings.Get_Game_Color("P4-PS2", null));

            // Create a footer based on the user's settings.
            var footer = new EmbedFooterBuilder
            {
                Text = Create_Sprite_Sheet_Footer(account, set_data)
            };

            // Add the footer to the embed.
            embed.WithFooter(footer);

            // Attach a locally generated image to the embed. This image hasn't been created yet, so the filename is just a placeholder for now.
            embed.WithImageUrl($"attachment://preview.png");

            // Create a new stream. We'll use this to create the locally generated image.
            MemoryStream memoryStream = new MemoryStream();

            // Generate a bitmap comprised of the base sprite chosen and any animation frames it may have.
            Bitmap sprite_set_preview = Generate_Standard_Bustup_Frame_Sheet(sl_command, set_data, maker_command_data.Character_Data_1);

            // Save the sprite set preview bitmap to the stream as a PNG.
            sprite_set_preview.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);

            // Ensure the stream is set to the beginning of itself.
            memoryStream.Seek(0, SeekOrigin.Begin);

            // Send the embeded message to the channel.
            try
            {
                await sl_command.Channel.SendFileAsync(memoryStream, "preview.png", "", false, embed.Build());
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
        }

        public static async Task P4G_Bustup_Frame_Sheet(SocialLinkerCommand sl_command, OfficialSetData set_data, MakerCommandData maker_command_data)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = sl_command.User;
            SocketTextChannel channel = (SocketTextChannel)sl_command.Channel;

            // Send a loading message to the channel while the sprite sheet is being made.
            RestUserMessage loader = await channel.SendMessageAsync("", false, P4G_Loading_Message().Build());

            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = $"{set_data.Name}'s Animation Frames - Portrait #{maker_command_data.Character_Data_1.Base_Sprite}",
                IconUrl = EmbedSettings.Get_Game_Logo("P4G")
            };

            embed.WithAuthor(author);

            // Set the color and thumbnail for the embeded message.
            embed.WithColor(EmbedSettings.Get_Game_Color("P4G", null));

            // Create a footer based on the user's settings.
            var footer = new EmbedFooterBuilder
            {
                Text = Create_Sprite_Sheet_Footer(account, set_data)
            };

            // Add the footer to the embed.
            embed.WithFooter(footer);

            // Attach a locally generated image to the embed. This image hasn't been created yet, so the filename is just a placeholder for now.
            embed.WithImageUrl($"attachment://preview.png");

            // Create a new stream. We'll use this to create the locally generated image.
            MemoryStream memoryStream = new MemoryStream();

            // Generate a bitmap comprised of the base sprite chosen and any animation frames it may have.
            Bitmap sprite_set_preview = Generate_Standard_Bustup_Frame_Sheet(sl_command, set_data, maker_command_data.Character_Data_1);

            // Save the sprite set preview bitmap to the stream as a PNG.
            sprite_set_preview.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);

            // Ensure the stream is set to the beginning of itself.
            memoryStream.Seek(0, SeekOrigin.Begin);

            // Send the embeded message to the channel.
            try
            {
                await sl_command.Channel.SendFileAsync(memoryStream, "preview.png", "", false, embed.Build());
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
        }

        public static async Task P4D_Bustup_Frame_Sheet(SocialLinkerCommand sl_command, OfficialSetData set_data, MakerCommandData maker_command_data)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = sl_command.User;
            SocketTextChannel channel = (SocketTextChannel)sl_command.Channel;

            // Send a loading message to the channel while the sprite sheet is being made.
            RestUserMessage loader = await channel.SendMessageAsync("", false, P4D_Loading_Message().Build());

            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = $"{set_data.Name}'s Animation Frames - Portrait #{maker_command_data.Character_Data_1.Base_Sprite}",
                IconUrl = EmbedSettings.Get_Game_Logo("P4D")
            };

            embed.WithAuthor(author);

            // Set the color and thumbnail for the embeded message.
            embed.WithColor(EmbedSettings.Get_Game_Color("P4D", null));

            // Create a footer based on the user's settings.
            var footer = new EmbedFooterBuilder
            {
                Text = Create_Sprite_Sheet_Footer(account, set_data)
            };

            // Add the footer to the embed.
            embed.WithFooter(footer);

            // Attach a locally generated image to the embed. This image hasn't been created yet, so the filename is just a placeholder for now.
            embed.WithImageUrl($"attachment://preview.png");

            // Create a new stream. We'll use this to create the locally generated image.
            MemoryStream memoryStream = new MemoryStream();

            // Generate a bitmap comprised of the base sprite chosen and any animation frames it may have.
            Bitmap sprite_set_preview = Generate_P4D_Bustup_Frame_Sheet(sl_command, set_data, maker_command_data);

            // Save the sprite set preview bitmap to the stream as a PNG.
            sprite_set_preview.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);

            // Ensure the stream is set to the beginning of itself.
            memoryStream.Seek(0, SeekOrigin.Begin);

            // Send the embeded message to the channel.
            try
            {
                await sl_command.Channel.SendFileAsync(memoryStream, "preview.png", "", false, embed.Build());
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
        }

        public static async Task P4AU_Bustup_Frame_Sheet(SocialLinkerCommand sl_command, OfficialSetData set_data, MakerCommandData maker_command_data)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = sl_command.User;
            SocketTextChannel channel = (SocketTextChannel)sl_command.Channel;

            // Send a loading message to the channel while the sprite sheet is being made.
            RestUserMessage loader = await channel.SendMessageAsync("", false, P4AU_Loading_Message().Build());

            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = $"{set_data.Name}'s Animation Frames - Portrait #{maker_command_data.Character_Data_1.Base_Sprite}",
                IconUrl = EmbedSettings.Get_Game_Logo("P4AU")
            };

            embed.WithAuthor(author);

            // Set the color and thumbnail for the embeded message.
            embed.WithColor(EmbedSettings.Get_Game_Color("P4AU", null));

            // Create a footer based on the user's settings.
            var footer = new EmbedFooterBuilder
            {
                Text = Create_Sprite_Sheet_Footer(account, set_data)
            };

            // Add the footer to the embed.
            embed.WithFooter(footer);

            // Attach a locally generated image to the embed. This image hasn't been created yet, so the filename is just a placeholder for now.
            embed.WithImageUrl($"attachment://preview.png");

            // Create a new stream. We'll use this to create the locally generated image.
            MemoryStream memoryStream = new MemoryStream();

            // Generate a bitmap comprised of the base sprite chosen and any animation frames it may have.
            Bitmap sprite_set_preview = Generate_P4D_Bustup_Frame_Sheet(sl_command, set_data, maker_command_data);

            // Save the sprite set preview bitmap to the stream as a PNG.
            sprite_set_preview.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);

            // Ensure the stream is set to the beginning of itself.
            memoryStream.Seek(0, SeekOrigin.Begin);

            // Send the embeded message to the channel.
            try
            {
                await sl_command.Channel.SendFileAsync(memoryStream, "preview.png", "", false, embed.Build());
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
        }

        public static async Task P5_PS4_Bustup_Frame_Sheet(SocialLinkerCommand sl_command, OfficialSetData set_data, MakerCommandData maker_command_data)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = sl_command.User;
            SocketTextChannel channel = (SocketTextChannel)sl_command.Channel;

            // Send a loading message to the channel while the sprite sheet is being made.
            RestUserMessage loader = await channel.SendMessageAsync("", false, P5_PS4_Loading_Message().Build());

            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = $"{set_data.Name}'s Animation Frames - Portrait #{maker_command_data.Character_Data_1.Base_Sprite}",
                IconUrl = EmbedSettings.Get_Game_Logo("P5-PS4")
            };

            embed.WithAuthor(author);

            // Set the color and thumbnail for the embeded message.
            embed.WithColor(EmbedSettings.Get_Game_Color("P5-PS4", null));

            // Create a footer based on the user's settings.
            var footer = new EmbedFooterBuilder
            {
                Text = Create_Sprite_Sheet_Footer(account, set_data)
            };

            // Add the footer to the embed.
            embed.WithFooter(footer);

            // Attach a locally generated image to the embed. This image hasn't been created yet, so the filename is just a placeholder for now.
            embed.WithImageUrl($"attachment://preview.png");

            // Create a new stream. We'll use this to create the locally generated image.
            MemoryStream memoryStream = new MemoryStream();

            // Generate a bitmap comprised of the base sprite chosen and any animation frames it may have.
            Bitmap sprite_set_preview = Generate_Standard_Bustup_Frame_Sheet(sl_command, set_data, maker_command_data.Character_Data_1);

            // Save the sprite set preview bitmap to the stream as a PNG.
            sprite_set_preview.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);

            // Ensure the stream is set to the beginning of itself.
            memoryStream.Seek(0, SeekOrigin.Begin);

            // Send the embeded message to the channel.
            try
            {
                await sl_command.Channel.SendFileAsync(memoryStream, "preview.png", "", false, embed.Build());
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
        }

        public static async Task P5R_Bustup_Frame_Sheet(SocialLinkerCommand sl_command, OfficialSetData set_data, MakerCommandData maker_command_data)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = sl_command.User;
            SocketTextChannel channel = (SocketTextChannel)sl_command.Channel;

            // Send a loading message to the channel while the sprite sheet is being made.
            RestUserMessage loader = await channel.SendMessageAsync("", false, P5R_Loading_Message().Build());

            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = $"{set_data.Name}'s Animation Frames - Portrait #{maker_command_data.Character_Data_1.Base_Sprite}",
                IconUrl = EmbedSettings.Get_Game_Logo("P5R")
            };

            embed.WithAuthor(author);

            // Set the color and thumbnail for the embeded message.
            embed.WithColor(EmbedSettings.Get_Game_Color("P5R", null));

            // Create a footer based on the user's settings.
            var footer = new EmbedFooterBuilder
            {
                Text = Create_Sprite_Sheet_Footer(account, set_data)
            };

            // Add the footer to the embed.
            embed.WithFooter(footer);

            // Attach a locally generated image to the embed. This image hasn't been created yet, so the filename is just a placeholder for now.
            embed.WithImageUrl($"attachment://preview.png");

            // Create a new stream. We'll use this to create the locally generated image.
            MemoryStream memoryStream = new MemoryStream();

            // Generate a bitmap comprised of the base sprite chosen and any animation frames it may have.
            Bitmap sprite_set_preview = Generate_Standard_Bustup_Frame_Sheet(sl_command, set_data, maker_command_data.Character_Data_1);

            // Save the sprite set preview bitmap to the stream as a PNG.
            sprite_set_preview.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);

            // Ensure the stream is set to the beginning of itself.
            memoryStream.Seek(0, SeekOrigin.Begin);

            // Send the embeded message to the channel.
            try
            {
                await sl_command.Channel.SendFileAsync(memoryStream, "preview.png", "", false, embed.Build());
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
        }

        public static async Task P5S_Bustup_Frame_Sheet(SocialLinkerCommand sl_command, OfficialSetData set_data, MakerCommandData maker_command_data)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = sl_command.User;
            SocketTextChannel channel = (SocketTextChannel)sl_command.Channel;

            // Send a loading message to the channel while the sprite sheet is being made.
            RestUserMessage loader = await channel.SendMessageAsync("", false, P5S_Loading_Message().Build());

            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = $"{set_data.Name}'s Animation Frames - Portrait #{maker_command_data.Character_Data_1.Base_Sprite}",
                IconUrl = EmbedSettings.Get_Game_Logo("P5S")
            };

            embed.WithAuthor(author);

            // Set the color and thumbnail for the embeded message.
            embed.WithColor(EmbedSettings.Get_Game_Color("P5S", null));

            // Create a footer based on the user's settings.
            var footer = new EmbedFooterBuilder
            {
                Text = Create_Sprite_Sheet_Footer(account, set_data)
            };

            // Add the footer to the embed.
            embed.WithFooter(footer);

            // Attach a locally generated image to the embed. This image hasn't been created yet, so the filename is just a placeholder for now.
            embed.WithImageUrl($"attachment://preview.png");

            // Create a new stream. We'll use this to create the locally generated image.
            MemoryStream memoryStream = new MemoryStream();

            // Generate a bitmap comprised of the base sprite chosen and any animation frames it may have.
            Bitmap sprite_set_preview = Generate_Standard_Bustup_Frame_Sheet(sl_command, set_data, maker_command_data.Character_Data_1);

            // Save the sprite set preview bitmap to the stream as a PNG.
            sprite_set_preview.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);

            // Ensure the stream is set to the beginning of itself.
            memoryStream.Seek(0, SeekOrigin.Begin);

            // Send the embeded message to the channel.
            try
            {
                await sl_command.Channel.SendFileAsync(memoryStream, "preview.png", "", false, embed.Build());
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
        }

        public static async Task BBTAG_Bustup_Frame_Sheet(SocialLinkerCommand sl_command, OfficialSetData set_data, MakerCommandData maker_command_data)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = sl_command.User;
            SocketTextChannel channel = (SocketTextChannel)sl_command.Channel;

            // Send a loading message to the channel while the sprite sheet is being made.
            RestUserMessage loader = await channel.SendMessageAsync("", false, BBTAG_Loading_Message(set_data).Build());

            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = $"{set_data.Name}'s Animation Frames - Portrait #{maker_command_data.Character_Data_1.Base_Sprite}",
                IconUrl = EmbedSettings.Get_Game_Logo("BBTAG")
            };

            embed.WithAuthor(author);

            // Determine the embeded message's color based on the origin series of the sprite set.
            embed.WithColor(EmbedSettings.Get_BBTAG_Series_Color(set_data.Series));

            // Create a footer based on the user's settings.
            var footer = new EmbedFooterBuilder
            {
                Text = Create_Sprite_Sheet_Footer(account, set_data)
            };

            // Add the footer to the embed.
            embed.WithFooter(footer);

            // Attach a locally generated image to the embed. This image hasn't been created yet, so the filename is just a placeholder for now.
            embed.WithImageUrl($"attachment://preview.png");

            // Create a new stream. We'll use this to create the locally generated image.
            MemoryStream memoryStream = new MemoryStream();

            // Generate a bitmap comprised of the base sprite chosen and any animation frames it may have.
            Bitmap sprite_set_preview = Generate_Standard_Bustup_Frame_Sheet(sl_command, set_data, maker_command_data.Character_Data_1);

            // Save the sprite set preview bitmap to the stream as a PNG.
            sprite_set_preview.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);

            // Ensure the stream is set to the beginning of itself.
            memoryStream.Seek(0, SeekOrigin.Begin);

            // Send the embeded message to the channel.
            try
            {
                await sl_command.Channel.SendFileAsync(memoryStream, "preview.png", "", false, embed.Build());
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
        }

        // Loading messages
        public static EmbedBuilder P1_PS1_Loading_Message()
        {
            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Generating Frame Panels...",
                IconUrl = EmbedSettings.Get_Game_Logo("P1-PS1")
            };

            embed.WithAuthor(author);
            embed.WithColor(EmbedSettings.Get_Game_Color("P1-PS1", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Loading_Icon("P1-PS1", null));
            embed.WithDescription("This may take a few seconds!");

            return embed;
        }

        public static EmbedBuilder P1_PSP_Loading_Message()
        {
            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Generating Frame Panels...",
                IconUrl = EmbedSettings.Get_Game_Logo("P1-PSP")
            };

            embed.WithAuthor(author);
            embed.WithColor(EmbedSettings.Get_Game_Color("P1-PSP", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Loading_Icon("P1-PSP", null));
            embed.WithDescription("This may take a few seconds!");

            return embed;
        }

        public static EmbedBuilder P2IS_PS1_Loading_Message()
        {
            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = $"Generating Frame Panels...",
                IconUrl = EmbedSettings.Get_Game_Logo("P2IS-PS1")
            };

            embed.WithAuthor(author);
            embed.WithColor(EmbedSettings.Get_Game_Color("P2IS-PS1", null));
            embed.WithDescription("This may take a few seconds!");

            return embed;
        }

        public static EmbedBuilder P2IS_PSP_Loading_Message()
        {
            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = $"Generating Frame Panels...",
                IconUrl = EmbedSettings.Get_Game_Logo("P2IS-PSP")
            };

            embed.WithAuthor(author);
            embed.WithColor(EmbedSettings.Get_Game_Color("P2IS-PSP", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Loading_Icon("P2IS-PSP", null));
            embed.WithDescription("This may take a few seconds!");

            return embed;
        }

        public static EmbedBuilder P2EP_PS1_Loading_Message()
        {
            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = $"Generating Frame Panels...",
                IconUrl = EmbedSettings.Get_Game_Logo("P2EP-PS1")
            };

            embed.WithAuthor(author);
            embed.WithColor(EmbedSettings.Get_Game_Color("P2EP-PSP", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Loading_Icon("P2EP-PSP", null));
            embed.WithDescription("This may take a few seconds!");

            return embed;
        }

        public static EmbedBuilder P2EP_PSP_Loading_Message()
        {
            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = $"Generating Frame Panels...",
                IconUrl = EmbedSettings.Get_Game_Logo("P2EP-PSP")
            };

            embed.WithAuthor(author);
            embed.WithColor(EmbedSettings.Get_Game_Color("P2EP-PSP", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Loading_Icon("P2EP-PSP", null));
            embed.WithDescription("This may take a few seconds!");

            return embed;
        }

        public static EmbedBuilder P3F_Loading_Message()
        {
            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Generating Frame Panels...",
                IconUrl = EmbedSettings.Get_Game_Logo("P3F")
            };

            embed.WithAuthor(author);
            embed.WithColor(EmbedSettings.Get_Game_Color("P3F", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Loading_Icon("P3F", null));
            embed.WithDescription("This may take a few seconds!");

            return embed;
        }

        public static EmbedBuilder P3P_Loading_Message(SocialLinkerCommand sl_command)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = sl_command.User;

            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Generating Frame Panels...",
                IconUrl = EmbedSettings.Get_Game_Logo("P3P")
            };

            embed.WithAuthor(author);

            // Assign a color based on the user's color setting for the P3P template.
            embed.WithColor(EmbedSettings.Get_Game_Color("P3P", account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Loading_Icon("P3P", account));
            embed.WithDescription("This may take a few seconds!");

            return embed;
        }

        public static EmbedBuilder P4_PS2_Loading_Message()
        {
            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Generating Frame Panels...",
                IconUrl = EmbedSettings.Get_Game_Logo("P4-PS2")
            };

            embed.WithAuthor(author);
            embed.WithColor(EmbedSettings.Get_Game_Color("P4-PS2", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Loading_Icon("P4-PS2", null));
            embed.WithDescription("This may take a few seconds!");

            return embed;
        }

        public static EmbedBuilder P4G_Loading_Message()
        {
            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Generating Frame Panels...",
                IconUrl = EmbedSettings.Get_Game_Logo("P4G")
            };

            embed.WithAuthor(author);
            embed.WithColor(EmbedSettings.Get_Game_Color("P4G", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Loading_Icon("P4G", null));
            embed.WithDescription("This may take a few seconds!");

            return embed;
        }

        public static EmbedBuilder P4AU_Loading_Message()
        {
            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Generating Frame Panels...",
                IconUrl = EmbedSettings.Get_Game_Logo("P4AU")
            };

            embed.WithAuthor(author);
            embed.WithColor(EmbedSettings.Get_Game_Color("P4AU", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Loading_Icon("P4AU", null));
            embed.WithDescription("This may take a few seconds!");

            return embed;
        }

        public static EmbedBuilder P4D_Loading_Message()
        {
            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Generating Frame Panels...",
                IconUrl = EmbedSettings.Get_Game_Logo("P4D")
            };

            embed.WithAuthor(author);
            embed.WithColor(EmbedSettings.Get_Game_Color("P4D", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Loading_Icon("P4D", null));
            embed.WithDescription("This may take a few seconds!");

            return embed;
        }

        public static EmbedBuilder P5_PS4_Loading_Message()
        {
            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Generating Frame Panels...",
                IconUrl = EmbedSettings.Get_Game_Logo("P5-PS4")
            };

            embed.WithAuthor(author);
            embed.WithColor(EmbedSettings.Get_Game_Color("P5-PS4", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Loading_Icon("P5-PS4", null));
            embed.WithDescription("This may take a few seconds!");

            return embed;
        }

        public static EmbedBuilder P5R_Loading_Message()
        {
            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Generating Frame Panels...",
                IconUrl = EmbedSettings.Get_Game_Logo("P5R")
            };

            embed.WithAuthor(author);
            embed.WithColor(EmbedSettings.Get_Game_Color("P5R", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Loading_Icon("P5R", null));
            embed.WithDescription("This may take a few seconds!");

            return embed;
        }

        public static EmbedBuilder P5S_Loading_Message()
        {
            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Generating Frame Panels...",
                IconUrl = EmbedSettings.Get_Game_Logo("P5S")
            };

            embed.WithAuthor(author);
            embed.WithColor(EmbedSettings.Get_Game_Color("P5S", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Loading_Icon("P5S", null));
            embed.WithDescription("This may take a few seconds!");

            return embed;
        }

        public static EmbedBuilder BBTAG_Loading_Message(OfficialSetData set_data)
        {
            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Loading Frame Panels...",
                IconUrl = EmbedSettings.Get_Game_Logo("BBTAG")
            };

            embed.WithAuthor(author);

            embed.WithColor(EmbedSettings.Get_BBTAG_Series_Color(set_data.Series));
            embed.WithThumbnailUrl(EmbedSettings.Get_Loading_Icon("BBTAG", null));
            embed.WithDescription("This may take a few seconds!");

            return embed;
        }
    }
}
