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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialLinker.Core.SceneMaker
{
    class OfficialSetSheets
    {
        // Sprite sheet formation
        public static Bitmap Generate_Standard_Bustup_Sprite_Sheet(SocketMessage message, OfficialSetData set_data)
        {
            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(message.Author);

            // Establish the directory of the specified sprite set.
            // This string structure should direct to any set containing bust-ups once the proper variables are input.
            string set_path = $@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//{set_data.Origin}//Bustup//{set_data.ID}";

            // Create a filename for the bitmap that will be generated.
            var fileName = $"{message.Author.Id}_{DateTime.UtcNow.ToString("yyyyMMdd_HH_mm_ss_fff")}.png";

            // Get a count of how many files are in the sprite set's directory.
            int filecount = OfficialSetMethods.AttachmentCountItemDirectory(set_path);

            // Determine how the items will be rendered on the bitmap based on the size of sublist_length.
            // This is done by taking the square root of the input sublist_length variable. Notice that the result will be a double at times and not a full integer.
            double sq_count = Math.Sqrt(filecount);

            // Determine how many columns and rows will be displayed on the bitmap.
            // Columns are determined by calculating the ceiling of the sq_count double variable.
            // Rows are calculated by converting the sq_count double straight to an int.
            int columns = (int)Math.Ceiling(sq_count);
            int rows = Convert.ToInt32(sq_count);

            // Create variables for the desired width and height of each sprite to be drawn on the template.
            int item_width = 256;
            int item_height = 256;

            // Set the width and height of the bitmap based on the desired dimensions of each sprite and the expected amounts of columns and rows.
            int template_width = (columns * item_width);
            int template_height = (rows * item_height);

            // Declare a bitmap variable.
            Bitmap base_template;

            // If either the template_width or template_height values end up being 0 (a result of no files in the directory), create a default size of 2 x 2.
            if (template_width == 0 || template_height == 0)
            {
                base_template = new Bitmap(2, 2);
            }
            // Else, create a bitmap working space from the calculated new_width and new_height values.
            else
            {
                base_template = new Bitmap(template_width, template_height);
            }

            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                if (set_data.Origin == "P1-PS1" ||
                    set_data.Origin == "P1-PS2" ||
                    set_data.Origin == "P2IS-PS1" ||
                    set_data.Origin == "P2IS-PSP" ||
                    set_data.Origin == "P2EP-PS1" ||
                    set_data.Origin == "P2EP-PSP" ||
                    set_data.Origin == "P3P" ||
                    set_data.Origin == "BBTAG")
                {
                    // Set the scaling mode for any rendered images to nearest neighbor.
                    graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                }

                // Create two int variables that represent the X and Y values on the base_template bitmap.
                int x = 0;
                int y = 0;

                // Depending on the user's settings, render the sprite sheet in two different ways.
                // First, "Order by Outfit".
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
                                // If the file path does exist, copy the file to an image variable.
                                System.Drawing.Image current_sprite = System.Drawing.Image.FromFile($"{set_path}//{set_data.ID.ToLower()}_{expression}_{outfit}.png");

                                // Draw the sprite to the template at the current X and Y coordinates.
                                graphics.DrawImage(current_sprite, x, y, item_width, item_height);

                                // Move the current X coordinate over to the right so the next sprite can be rendered right next to the current one.
                                x = x + item_width;

                                // Check if the newly positioned X coordinate is greater than or equal to the template's max width.
                                // If so, move the X coordinate to 0 and the Y coordinate down an entire sprite's length.
                                if (x >= template_width)
                                {
                                    x = 0;
                                    y = y + item_height;
                                }
                            }
                        }
                    }
                }
                // Here, we check if the user's settings is on "Order by Expression".
                else if (account.Setting_Sheet_Order == "Order by Expression")
                {
                    // Create a loop starting at 1 meant to iterate though every file in the directory.
                    // This loop is searching for expressions, which start at 1.
                    for (int expression = 1; expression <= filecount; expression++)
                    {
                        // Inside, create a secondary loop also meant to iterate though every file in the directory.
                        // Outfit numbers always start at 1, so we'll begin there.
                        for (int outfit = 1; outfit <= filecount; outfit++)
                        {
                            // Here, we're going to create a file path that could potentially exist given the combination of expression and outfit numbers.
                            // Check if the created file path string exists.
                            if (File.Exists($"{set_path}//{set_data.ID.ToLower()}_{expression}_{outfit}.png"))
                            {
                                // If the file path does exist, copy the file to an image variable.
                                System.Drawing.Image current_sprite = System.Drawing.Image.FromFile($"{set_path}//{set_data.ID.ToLower()}_{expression}_{outfit}.png");

                                // Draw the sprite to the template at the current X and Y coordinates.
                                graphics.DrawImage(current_sprite, x, y, item_width, item_height);

                                // Move the current X coordinate over to the right so the next sprite can be rendered right next to the current one.
                                x = x + item_width;

                                // Check if the newly positioned X coordinate is greater than or equal to the template's max width.
                                // If so, move the X coordinate to 0 and the Y coordinate down an entire sprite's length.
                                if (x >= template_width)
                                {
                                    x = 0;
                                    y = y + item_height;
                                }
                            }
                        }
                    }
                }

                // Reset the X and Y coordinates to 0.
                // Now that we're done assembling the sprites on the template, it's time to label them.
                x = 0;
                y = 0;

                // Create a single loop to iterate for as many times as there are files in the directory.
                for (int i = 1; i <= filecount; i++)
                {
                    using (Font rockwell = new Font("Rockwell", 45, FontStyle.Bold))
                    {
                        // Create a GraphicsPath object.
                        GraphicsPath myPath = new GraphicsPath();

                        // Set up all the string parameters.
                        string stringText = $"{i}";

                        System.Drawing.FontFamily family = new System.Drawing.FontFamily("Rockwell");
                        int fontStyle = (int)FontStyle.Bold;
                        int emSize = 23;
                        Point origin = new Point(x + 128, y + 222);

                        StringFormat stringFormat = new StringFormat();
                        stringFormat.Alignment = StringAlignment.Center;
                        stringFormat.LineAlignment = StringAlignment.Center;

                        // Add the string to the path.
                        myPath.AddString(stringText,
                            family,
                            fontStyle,
                            emSize = (int)graphics.DpiY * 48 / 72, //47 pt
                            origin,
                            stringFormat);

                        //Draw the path to the screen.
                        graphics.SmoothingMode = SmoothingMode.AntiAlias;
                        graphics.FillPath(System.Drawing.Brushes.White, myPath);
                        graphics.DrawPath(new System.Drawing.Pen(System.Drawing.Brushes.Black, 4), myPath);
                        graphics.FillPath(System.Drawing.Brushes.White, myPath);
                        graphics.DrawPath(new System.Drawing.Pen(System.Drawing.Brushes.Black, 0), myPath);
                    }

                    // Move the current X coordinate over to the right so the next sprite can be rendered right next to the current one.
                    x = x + item_width;

                    // Check if the newly positioned X coordinate is greater than or equal to the template's max width.
                    // If so, move the X coordinate to 0 and the Y coordinate down an entire sprite's length.
                    if (x >= template_width)
                    {
                        x = 0;
                        y = y + item_height;
                    }
                }
            }

            return base_template;
        }

        public static Bitmap Generate_P3F_Bustup_Sprite_Sheet(SocketMessage message, OfficialSetData set_data)
        {
            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(message.Author);

            // Establish the directory of the specified sprite set.
            // This string structure should direct to any set containing bust-ups once the proper variables are input.
            string set_path = $@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//{set_data.Origin}//Bustup//{set_data.ID}";

            // Create a filename for the bitmap that will be generated.
            var fileName = $"{message.Author.Id}_{DateTime.UtcNow.ToString("yyyyMMdd_HH_mm_ss_fff")}.png";

            // Get a count of how many files are in the sprite set's directory.
            int filecount = OfficialSetMethods.AttachmentCountItemDirectory(set_path);

            // Determine how the items will be rendered on the bitmap based on the size of sublist_length.
            // This is done by taking the square root of the input sublist_length variable. Notice that the result will be a double at times and not a full integer.
            double sq_count = Math.Sqrt(filecount);

            // Determine how many columns and rows will be displayed on the bitmap.
            // Columns are determined by calculating the ceiling of the sq_count double variable.
            // Rows are calculated by converting the sq_count double straight to an int.
            int columns = (int)Math.Ceiling(sq_count);
            int rows = Convert.ToInt32(sq_count);

            // Create variables for the desired width and height of each sprite to be drawn on the template.
            int item_width = 512;
            int item_height = 512;

            // Set the width and height of the bitmap based on the desired dimensions of each sprite and the expected amounts of columns and rows.
            int template_width = (columns * item_width);
            int template_height = (rows * item_height);

            // Declare a bitmap variable.
            Bitmap base_template;

            // If either the template_width or template_height values end up being 0 (a result of no files in the directory), create a default size of 2 x 2.
            if (template_width == 0 || template_height == 0)
            {
                base_template = new Bitmap(2, 2);
            }
            // Else, create a bitmap working space from the calculated new_width and new_height values.
            else
            {
                base_template = new Bitmap(template_width, template_height);
            }

            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                // Create two int variables that represent the X and Y values on the base_template bitmap.
                int x = 0;
                int y = 0;

                // Depending on the user's settings, render the sprite sheet in two different ways.
                // First, "Order by Outfit".
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
                                // If the file path does exist, copy the file to an image variable.
                                System.Drawing.Image current_sprite = System.Drawing.Image.FromFile($"{set_path}//{set_data.ID.ToLower()}_{expression}_{outfit}.png");

                                // Draw the sprite to the template at the current X and Y coordinates.
                                graphics.DrawImage(current_sprite, x, y, item_width, item_height);

                                // Move the current X coordinate over to the right so the next sprite can be rendered right next to the current one.
                                x = x + item_width;

                                // Check if the newly positioned X coordinate is greater than or equal to the template's max width.
                                // If so, move the X coordinate to 0 and the Y coordinate down an entire sprite's length.
                                if (x >= template_width)
                                {
                                    x = 0;
                                    y = y + item_height;
                                }
                            }
                        }
                    }
                }
                // Here, we check if the user's settings is on "Order by Expression".
                else if (account.Setting_Sheet_Order == "Order by Expression")
                {
                    // Create a loop starting at 1 meant to iterate though every file in the directory.
                    // This loop is searching for expressions, which start at 1.
                    for (int expression = 1; expression <= filecount; expression++)
                    {
                        // Inside, create a secondary loop also meant to iterate though every file in the directory.
                        // Outfit numbers always start at 1, so we'll begin there.
                        for (int outfit = 1; outfit <= filecount; outfit++)
                        {
                            // Here, we're going to create a file path that could potentially exist given the combination of expression and outfit numbers.
                            // Check if the created file path string exists.
                            if (File.Exists($"{set_path}//{set_data.ID.ToLower()}_{expression}_{outfit}.png"))
                            {
                                // If the file path does exist, copy the file to an image variable.
                                System.Drawing.Image current_sprite = System.Drawing.Image.FromFile($"{set_path}//{set_data.ID.ToLower()}_{expression}_{outfit}.png");

                                // Draw the sprite to the template at the current X and Y coordinates.
                                graphics.DrawImage(current_sprite, x, y, item_width, item_height);

                                // Move the current X coordinate over to the right so the next sprite can be rendered right next to the current one.
                                x = x + item_width;

                                // Check if the newly positioned X coordinate is greater than or equal to the template's max width.
                                // If so, move the X coordinate to 0 and the Y coordinate down an entire sprite's length.
                                if (x >= template_width)
                                {
                                    x = 0;
                                    y = y + item_height;
                                }
                            }
                        }
                    }
                }

                // Reset the X and Y coordinates to 0.
                // Now that we're done assembling the sprites on the template, it's time to label them.
                x = 0;
                y = 0;

                // Create a single loop to iterate for as many times as there are files in the directory.
                for (int i = 1; i <= filecount; i++)
                {
                    using (Font rockwell = new Font("Rockwell", 45, FontStyle.Bold))
                    {
                        // Create a GraphicsPath object.
                        GraphicsPath myPath = new GraphicsPath();

                        // Set up all the string parameters.
                        string stringText = $"{i}";

                        System.Drawing.FontFamily family = new System.Drawing.FontFamily("Rockwell");
                        int fontStyle = (int)FontStyle.Bold;
                        int emSize = 23;
                        Point origin = new Point(x + 256, y + 444);

                        StringFormat stringFormat = new StringFormat();
                        stringFormat.Alignment = StringAlignment.Center;
                        stringFormat.LineAlignment = StringAlignment.Center;

                        // Add the string to the path.
                        myPath.AddString(stringText,
                            family,
                            fontStyle,
                            emSize = (int)graphics.DpiY * 96 / 72,
                            origin,
                            stringFormat);

                        //Draw the path to the screen.
                        graphics.SmoothingMode = SmoothingMode.AntiAlias;
                        graphics.FillPath(System.Drawing.Brushes.White, myPath);
                        graphics.DrawPath(new System.Drawing.Pen(System.Drawing.Brushes.Black, 6), myPath);
                        graphics.FillPath(System.Drawing.Brushes.White, myPath);
                        graphics.DrawPath(new System.Drawing.Pen(System.Drawing.Brushes.Black, 0), myPath);
                    }

                    // Move the current X coordinate over to the right so the next sprite can be rendered right next to the current one.
                    x = x + item_width;

                    // Check if the newly positioned X coordinate is greater than or equal to the template's max width.
                    // If so, move the X coordinate to 0 and the Y coordinate down an entire sprite's length.
                    if (x >= template_width)
                    {
                        x = 0;
                        y = y + item_height;
                    }
                }
            }

            return base_template;
        }

        public static Bitmap Generate_P4_PS2_Bustup_Sprite_Sheet(SocketMessage message, OfficialSetData set_data)
        {
            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(message.Author);

            // Establish the directory of the specified sprite set.
            // This string structure should direct to any set containing bust-ups once the proper variables are input.
            string set_path = $@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//{set_data.Origin}//Bustup//{set_data.ID}";

            // Create a filename for the bitmap that will be generated.
            var fileName = $"{message.Author.Id}_{DateTime.UtcNow.ToString("yyyyMMdd_HH_mm_ss_fff")}.png";

            // Get a count of how many files are in the sprite set's directory.
            int filecount = OfficialSetMethods.AttachmentCountItemDirectory(set_path);

            // Determine how the items will be rendered on the bitmap based on the size of sublist_length.
            // This is done by taking the square root of the input sublist_length variable. Notice that the result will be a double at times and not a full integer.
            double sq_count = Math.Sqrt(filecount);

            // Determine how many columns and rows will be displayed on the bitmap.
            // Columns are determined by calculating the ceiling of the sq_count double variable.
            // Rows are calculated by converting the sq_count double straight to an int.
            int columns = (int)Math.Ceiling(sq_count);
            int rows = Convert.ToInt32(sq_count);

            // Create variables for the desired width and height of each sprite to be drawn on the template.
            int item_width = 256;
            int item_height = 128;

            // Set the width and height of the bitmap based on the desired dimensions of each sprite and the expected amounts of columns and rows.
            int template_width = (columns * item_width);
            int template_height = (rows * item_height);

            // Create a bitmap working space from the calculated new_width and new_height values.
            Bitmap base_template = new Bitmap(template_width, template_height);

            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                // Create two int variables that represent the X and Y values on the base_template bitmap.
                int x = 0;
                int y = 0;

                // Depending on the user's settings, render the sprite sheet in two different ways.
                // First, "Order by Outfit".
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
                                // If the file path does exist, copy the file to an image variable.
                                System.Drawing.Image current_sprite = System.Drawing.Image.FromFile($"{set_path}//{set_data.ID.ToLower()}_{expression}_{outfit}.png");

                                // Draw the sprite to the template at the current X and Y coordinates.
                                graphics.DrawImage(current_sprite, x, y, item_width, item_height);

                                // Move the current X coordinate over to the right so the next sprite can be rendered right next to the current one.
                                x = x + item_width;

                                // Check if the newly positioned X coordinate is greater than or equal to the template's max width.
                                // If so, move the X coordinate to 0 and the Y coordinate down an entire sprite's length.
                                if (x >= template_width)
                                {
                                    x = 0;
                                    y = y + item_height;
                                }
                            }
                        }
                    }
                }
                // Here, we check if the user's settings is on "Order by Expression".
                else if (account.Setting_Sheet_Order == "Order by Expression")
                {
                    // Create a loop starting at 1 meant to iterate though every file in the directory.
                    // This loop is searching for expressions, which start at 1.
                    for (int expression = 1; expression <= filecount; expression++)
                    {
                        // Inside, create a secondary loop also meant to iterate though every file in the directory.
                        // Outfit numbers always start at 1, so we'll begin there.
                        for (int outfit = 1; outfit <= filecount; outfit++)
                        {
                            // Here, we're going to create a file path that could potentially exist given the combination of expression and outfit numbers.
                            // Check if the created file path string exists.
                            if (File.Exists($"{set_path}//{set_data.ID.ToLower()}_{expression}_{outfit}.png"))
                            {
                                // If the file path does exist, copy the file to an image variable.
                                System.Drawing.Image current_sprite = System.Drawing.Image.FromFile($"{set_path}//{set_data.ID.ToLower()}_{expression}_{outfit}.png");

                                // Draw the sprite to the template at the current X and Y coordinates.
                                graphics.DrawImage(current_sprite, x, y, item_width, item_height);

                                // Move the current X coordinate over to the right so the next sprite can be rendered right next to the current one.
                                x = x + item_width;

                                // Check if the newly positioned X coordinate is greater than or equal to the template's max width.
                                // If so, move the X coordinate to 0 and the Y coordinate down an entire sprite's length.
                                if (x >= template_width)
                                {
                                    x = 0;
                                    y = y + item_height;
                                }
                            }
                        }
                    }
                }

                // Reset the X and Y coordinates to 0.
                // Now that we're done assembling the sprites on the template, it's time to label them.
                x = 0;
                y = 0;

                // Create a single loop to iterate for as many times as there are files in the directory.
                for (int i = 1; i <= filecount; i++)
                {
                    using (Font rockwell = new Font("Rockwell", 45, FontStyle.Bold))
                    {
                        // Create a GraphicsPath object.
                        GraphicsPath myPath = new GraphicsPath();

                        // Set up all the string parameters.
                        string stringText = $"{i}";

                        System.Drawing.FontFamily family = new System.Drawing.FontFamily("Rockwell");
                        int fontStyle = (int)FontStyle.Bold;
                        int emSize = 23;
                        Point origin = new Point(x, y + 111);

                        StringFormat stringFormat = new StringFormat();
                        stringFormat.Alignment = StringAlignment.Near;
                        stringFormat.LineAlignment = StringAlignment.Center;

                        // Add the string to the path.
                        myPath.AddString(stringText,
                            family,
                            fontStyle,
                            emSize = (int)graphics.DpiY * 35 / 72,
                            origin,
                            stringFormat);

                        //Draw the path to the screen.
                        graphics.SmoothingMode = SmoothingMode.AntiAlias;
                        graphics.FillPath(System.Drawing.Brushes.White, myPath);
                        graphics.DrawPath(new System.Drawing.Pen(System.Drawing.Brushes.Black, 3), myPath);
                        graphics.FillPath(System.Drawing.Brushes.White, myPath);
                        graphics.DrawPath(new System.Drawing.Pen(System.Drawing.Brushes.Black, 0), myPath);
                    }

                    // Move the current X coordinate over to the right so the next sprite can be rendered right next to the current one.
                    x = x + item_width;

                    // Check if the newly positioned X coordinate is greater than or equal to the template's max width.
                    // If so, move the X coordinate to 0 and the Y coordinate down an entire sprite's length.
                    if (x >= template_width)
                    {
                        x = 0;
                        y = y + item_height;
                    }
                }
            }

            return base_template;
        }

        public static Bitmap Generate_P4D_Bustup_Sprite_Sheet(SocketMessage message, OfficialSetData set_data)
        {
            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(message.Author);

            // Establish the directory of the specified sprite set.
            // This string structure should direct to any set containing bust-ups once the proper variables are input.
            string set_path = $@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//{set_data.Origin}//Bustup//{set_data.ID}";

            // Create a filename for the bitmap that will be generated.
            var fileName = $"{message.Author.Id}_{DateTime.UtcNow.ToString("yyyyMMdd_HH_mm_ss_fff")}.png";

            // Get a count of how many files are in the sprite set's directory.
            int filecount = OfficialSetMethods.AttachmentCountItemDirectory(set_path);

            // Determine how the items will be rendered on the bitmap based on the size of sublist_length.
            // This is done by taking the square root of the input sublist_length variable. Notice that the result will be a double at times and not a full integer.
            double sq_count = Math.Sqrt(filecount);

            // Determine how many columns and rows will be displayed on the bitmap.
            // Columns are determined by calculating the ceiling of the sq_count double variable.
            // Rows are calculated by converting the sq_count double straight to an int.
            int columns = (int)Math.Ceiling(sq_count);
            int rows = Convert.ToInt32(sq_count);

            // Create variables for the desired width and height of each sprite to be drawn on the template.
            int item_width = 512;
            int item_height = 512;

            // Set the width and height of the bitmap based on the desired dimensions of each sprite and the expected amounts of columns and rows.
            int template_width = (columns * item_width);
            int template_height = (rows * item_height);

            // Declare a bitmap variable.
            Bitmap base_template;

            // If either the template_width or template_height values end up being 0 (a result of no files in the directory), create a default size of 2 x 2.
            if (template_width == 0 || template_height == 0)
            {
                base_template = new Bitmap(2, 2);
            }
            // Else, create a bitmap working space from the calculated new_width and new_height values.
            else
            {
                base_template = new Bitmap(template_width, template_height);
            }

            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                // Create two int variables that represent the X and Y values on the base_template bitmap.
                int x = 0;
                int y = 0;

                // Depending on the user's settings, render the sprite sheet in two different ways.
                // First, "Order by Outfit".
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
                                // If the file path does exist, copy the file to an image variable.
                                System.Drawing.Image current_sprite = System.Drawing.Image.FromFile($"{set_path}//{set_data.ID.ToLower()}_{expression}_{outfit}.png");

                                // Check if the image is not a square and the height is greater than 1024 (the average height of a P4D bustup).
                                if ((current_sprite.Width != current_sprite.Height) && (current_sprite.Height > 1024))
                                {
                                    // If so, we'll have to crop the bustup for the sprite sheet.
                                    // Create a new bitmap at a 1024 x 1024 resolution.
                                    Bitmap cropped_image = new Bitmap(1024, 1024);

                                    // Using a new graphics object, draw the bustup to the newly created bitmap, effectively cropping it.
                                    using (Graphics bitmap_edit = Graphics.FromImage(cropped_image))
                                    {
                                        bitmap_edit.DrawImage(current_sprite, 0, -(current_sprite.Height - 1024), current_sprite.Width, current_sprite.Height);
                                    }

                                    // Copy the cropped_image bitmap to the current_sprite variable.
                                    current_sprite = cropped_image;
                                }

                                // Draw the sprite to the template at the current X and Y coordinates.
                                graphics.DrawImage(current_sprite, x, y, item_width, item_height);

                                // Move the current X coordinate over to the right so the next sprite can be rendered right next to the current one.
                                x = x + item_width;

                                // Check if the newly positioned X coordinate is greater than or equal to the template's max width.
                                // If so, move the X coordinate to 0 and the Y coordinate down an entire sprite's length.
                                if (x >= template_width)
                                {
                                    x = 0;
                                    y = y + item_height;
                                }
                            }
                        }
                    }
                }
                // Here, we check if the user's settings is on "Order by Expression".
                else if (account.Setting_Sheet_Order == "Order by Expression")
                {
                    // Create a loop starting at 1 meant to iterate though every file in the directory.
                    // This loop is searching for expressions, which start at 1.
                    for (int expression = 1; expression <= filecount; expression++)
                    {
                        // Inside, create a secondary loop also meant to iterate though every file in the directory.
                        // Outfit numbers always start at 1, so we'll begin there.
                        for (int outfit = 1; outfit <= filecount; outfit++)
                        {
                            // Here, we're going to create a file path that could potentially exist given the combination of expression and outfit numbers.
                            // Check if the created file path string exists.
                            if (File.Exists($"{set_path}//{set_data.ID.ToLower()}_{expression}_{outfit}.png"))
                            {
                                // If the file path does exist, copy the file to an image variable.
                                System.Drawing.Image current_sprite = System.Drawing.Image.FromFile($"{set_path}//{set_data.ID.ToLower()}_{expression}_{outfit}.png");

                                // Check if the image is not a square and the height is greater than 1024 (the average height of a P4D bustup).
                                if ((current_sprite.Width != current_sprite.Height) && (current_sprite.Height > 1024))
                                {
                                    // If so, we'll have to crop the bustup for the sprite sheet.
                                    // Create a new bitmap at a 1024 x 1024 resolution.
                                    Bitmap cropped_image = new Bitmap(1024, 1024);

                                    // Using a new graphics object, draw the bustup to the newly created bitmap, effectively cropping it.
                                    using (Graphics bitmap_edit = Graphics.FromImage(cropped_image))
                                    {
                                        bitmap_edit.DrawImage(current_sprite, 0, -(current_sprite.Height - 1024), current_sprite.Width, current_sprite.Height);
                                    }

                                    // Copy the cropped_image bitmap to the current_sprite variable.
                                    current_sprite = cropped_image;
                                }

                                // Draw the sprite to the template at the current X and Y coordinates.
                                graphics.DrawImage(current_sprite, x, y, item_width, item_height);

                                // Move the current X coordinate over to the right so the next sprite can be rendered right next to the current one.
                                x = x + item_width;

                                // Check if the newly positioned X coordinate is greater than or equal to the template's max width.
                                // If so, move the X coordinate to 0 and the Y coordinate down an entire sprite's length.
                                if (x >= template_width)
                                {
                                    x = 0;
                                    y = y + item_height;
                                }
                            }
                        }
                    }
                }

                // Reset the X and Y coordinates to 0.
                // Now that we're done assembling the sprites on the template, it's time to label them.
                x = 0;
                y = 0;

                // Create a single loop to iterate for as many times as there are files in the directory.
                for (int i = 1; i <= filecount; i++)
                {
                    using (Font rockwell = new Font("Rockwell", 45, FontStyle.Bold))
                    {
                        // Create a GraphicsPath object.
                        GraphicsPath myPath = new GraphicsPath();

                        // Set up all the string parameters.
                        string stringText = $"{i}";

                        System.Drawing.FontFamily family = new System.Drawing.FontFamily("Rockwell");
                        int fontStyle = (int)FontStyle.Bold;
                        int emSize = 23;
                        Point origin = new Point(x + 256, y + 444);

                        StringFormat stringFormat = new StringFormat();
                        stringFormat.Alignment = StringAlignment.Center;
                        stringFormat.LineAlignment = StringAlignment.Center;

                        // Add the string to the path.
                        myPath.AddString(stringText,
                            family,
                            fontStyle,
                            emSize = (int)graphics.DpiY * 96 / 72,
                            origin,
                            stringFormat);

                        //Draw the path to the screen.
                        graphics.SmoothingMode = SmoothingMode.AntiAlias;
                        graphics.FillPath(System.Drawing.Brushes.White, myPath);
                        graphics.DrawPath(new System.Drawing.Pen(System.Drawing.Brushes.Black, 6), myPath);
                        graphics.FillPath(System.Drawing.Brushes.White, myPath);
                        graphics.DrawPath(new System.Drawing.Pen(System.Drawing.Brushes.Black, 0), myPath);
                    }

                    // Move the current X coordinate over to the right so the next sprite can be rendered right next to the current one.
                    x = x + item_width;

                    // Check if the newly positioned X coordinate is greater than or equal to the template's max width.
                    // If so, move the X coordinate to 0 and the Y coordinate down an entire sprite's length.
                    if (x >= template_width)
                    {
                        x = 0;
                        y = y + item_height;
                    }
                }
            }

            return base_template;
        }

        public static Bitmap Generate_P5S_Bustup_Sprite_Sheet(SocketMessage message, OfficialSetData set_data)
        {
            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(message.Author);

            // Establish the directory of the specified sprite set.
            // This string structure should direct to any set containing bust-ups once the proper variables are input.
            string set_path = $@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//{set_data.Origin}//Bustup//{set_data.ID}";

            // Create a filename for the bitmap that will be generated.
            var fileName = $"{message.Author.Id}_{DateTime.UtcNow.ToString("yyyyMMdd_HH_mm_ss_fff")}.png";

            // Get a count of how many files are in the sprite set's directory.
            int filecount = OfficialSetMethods.AttachmentCountItemDirectory(set_path);

            // Determine how the items will be rendered on the bitmap based on the size of sublist_length.
            // This is done by taking the square root of the input sublist_length variable. Notice that the result will be a double at times and not a full integer.
            double sq_count = Math.Sqrt(filecount);

            // Determine how many columns and rows will be displayed on the bitmap.
            // Columns are determined by calculating the ceiling of the sq_count double variable.
            // Rows are calculated by converting the sq_count double straight to an int.
            int columns = (int)Math.Ceiling(sq_count);
            int rows = Convert.ToInt32(sq_count);

            // Create variables for the desired width and height of each sprite to be drawn on the template.
            int item_width = 281;
            int item_height = 256;

            // Set the width and height of the bitmap based on the desired dimensions of each sprite and the expected amounts of columns and rows.
            int template_width = (columns * item_width);
            int template_height = (rows * item_height);

            // Declare a bitmap variable.
            Bitmap base_template;

            // If either the template_width or template_height values end up being 0 (a result of no files in the directory), create a default size of 2 x 2.
            if (template_width == 0 || template_height == 0)
            {
                base_template = new Bitmap(2, 2);
            }
            // Else, create a bitmap working space from the calculated new_width and new_height values.
            else
            {
                base_template = new Bitmap(template_width, template_height);
            }

            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                // Create two int variables that represent the X and Y values on the base_template bitmap.
                int x = 0;
                int y = 0;

                // Depending on the user's settings, render the sprite sheet in two different ways.
                // First, "Order by Outfit".
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
                                // If the file path does exist, copy the file to an image variable.
                                System.Drawing.Image current_sprite = System.Drawing.Image.FromFile($"{set_path}//{set_data.ID.ToLower()}_{expression}_{outfit}.png");

                                // Draw the sprite to the template at the current X and Y coordinates.
                                graphics.DrawImage(current_sprite, x, y, item_width, item_height);

                                // Move the current X coordinate over to the right so the next sprite can be rendered right next to the current one.
                                x = x + item_width;

                                // Check if the newly positioned X coordinate is greater than or equal to the template's max width.
                                // If so, move the X coordinate to 0 and the Y coordinate down an entire sprite's length.
                                if (x >= template_width)
                                {
                                    x = 0;
                                    y = y + item_height;
                                }
                            }
                        }
                    }
                }
                // Here, we check if the user's settings is on "Order by Expression".
                else if (account.Setting_Sheet_Order == "Order by Expression")
                {
                    // Create a loop starting at 1 meant to iterate though every file in the directory.
                    // This loop is searching for expressions, which start at 1.
                    for (int expression = 1; expression <= filecount; expression++)
                    {
                        // Inside, create a secondary loop also meant to iterate though every file in the directory.
                        // Outfit numbers always start at 1, so we'll begin there.
                        for (int outfit = 1; outfit <= filecount; outfit++)
                        {
                            // Here, we're going to create a file path that could potentially exist given the combination of expression and outfit numbers.
                            // Check if the created file path string exists.
                            if (File.Exists($"{set_path}//{set_data.ID.ToLower()}_{expression}_{outfit}.png"))
                            {
                                // If the file path does exist, copy the file to an image variable.
                                System.Drawing.Image current_sprite = System.Drawing.Image.FromFile($"{set_path}//{set_data.ID.ToLower()}_{expression}_{outfit}.png");

                                // Draw the sprite to the template at the current X and Y coordinates.
                                graphics.DrawImage(current_sprite, x, y, item_width, item_height);

                                // Move the current X coordinate over to the right so the next sprite can be rendered right next to the current one.
                                x = x + item_width;

                                // Check if the newly positioned X coordinate is greater than or equal to the template's max width.
                                // If so, move the X coordinate to 0 and the Y coordinate down an entire sprite's length.
                                if (x >= template_width)
                                {
                                    x = 0;
                                    y = y + item_height;
                                }
                            }
                        }
                    }
                }

                // Reset the X and Y coordinates to 0.
                // Now that we're done assembling the sprites on the template, it's time to label them.
                x = 0;
                y = 0;

                // Create a single loop to iterate for as many times as there are files in the directory.
                for (int i = 1; i <= filecount; i++)
                {
                    using (Font rockwell = new Font("Rockwell", 45, FontStyle.Bold))
                    {
                        // Create a GraphicsPath object.
                        GraphicsPath myPath = new GraphicsPath();

                        // Set up all the string parameters.
                        string stringText = $"{i}";

                        System.Drawing.FontFamily family = new System.Drawing.FontFamily("Rockwell");
                        int fontStyle = (int)FontStyle.Bold;
                        int emSize = 23;
                        Point origin = new Point(x, y + 222); //new Point(x + 128, y + 222);

                        StringFormat stringFormat = new StringFormat();
                        stringFormat.Alignment = StringAlignment.Near;
                        stringFormat.LineAlignment = StringAlignment.Center;

                        // Add the string to the path.
                        myPath.AddString(stringText,
                            family,
                            fontStyle,
                            emSize = (int)graphics.DpiY * 48 / 72,
                            origin,
                            stringFormat);

                        //Draw the path to the screen.
                        graphics.SmoothingMode = SmoothingMode.AntiAlias;
                        graphics.FillPath(System.Drawing.Brushes.White, myPath);
                        graphics.DrawPath(new System.Drawing.Pen(System.Drawing.Brushes.Black, 4), myPath);
                        graphics.FillPath(System.Drawing.Brushes.White, myPath);
                        graphics.DrawPath(new System.Drawing.Pen(System.Drawing.Brushes.Black, 0), myPath);
                    }

                    // Move the current X coordinate over to the right so the next sprite can be rendered right next to the current one.
                    x = x + item_width;

                    // Check if the newly positioned X coordinate is greater than or equal to the template's max width.
                    // If so, move the X coordinate to 0 and the Y coordinate down an entire sprite's length.
                    if (x >= template_width)
                    {
                        x = 0;
                        y = y + item_height;
                    }
                }
            }

            return base_template;
        }

        public static Bitmap Generate_BBTAG_Bustup_Sprite_Sheet(SocketMessage message, OfficialSetData set_data)
        {
            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(message.Author);

            // Establish the directory of the specified sprite set.
            // This string structure should direct to any set containing bust-ups once the proper variables are input.
            string set_path = $@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//{set_data.Origin}//Bustup_Preview//{set_data.ID}";

            // Create a filename for the bitmap that will be generated.
            var fileName = $"{message.Author.Id}_{DateTime.UtcNow.ToString("yyyyMMdd_HH_mm_ss_fff")}.png";

            // Get a count of how many files are in the sprite set's directory.
            int filecount = OfficialSetMethods.AttachmentCountItemDirectory(set_path);

            // Determine how the items will be rendered on the bitmap based on the size of sublist_length.
            // This is done by taking the square root of the input sublist_length variable. Notice that the result will be a double at times and not a full integer.
            double sq_count = Math.Sqrt(filecount);

            // Determine how many columns and rows will be displayed on the bitmap.
            // Columns are determined by calculating the ceiling of the sq_count double variable.
            // Rows are calculated by converting the sq_count double straight to an int.
            int columns = (int)Math.Ceiling(sq_count);
            int rows = Convert.ToInt32(sq_count);

            // Create variables for the desired width and height of each sprite to be drawn on the template.
            int item_width = 256;
            int item_height = 256;

            // Set the width and height of the bitmap based on the desired dimensions of each sprite and the expected amounts of columns and rows.
            int template_width = (columns * item_width);
            int template_height = (rows * item_height);

            // Create a bitmap working space from the calculated new_width and new_height values.
            Bitmap base_template = new Bitmap(template_width, template_height);

            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                // Create two int variables that represent the X and Y values on the base_template bitmap.
                int x = 0;
                int y = 0;

                // Depending on the user's settings, render the sprite sheet in two different ways.
                // First, "Order by Outfit".
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
                                // If the file path does exist, copy the file to an image variable.
                                System.Drawing.Image current_sprite = System.Drawing.Image.FromFile($"{set_path}//{set_data.ID.ToLower()}_{expression}_{outfit}.png");

                                // Draw the sprite to the template at the current X and Y coordinates.
                                graphics.DrawImage(current_sprite, x, y, item_width, item_height);

                                // Move the current X coordinate over to the right so the next sprite can be rendered right next to the current one.
                                x = x + item_width;

                                // Check if the newly positioned X coordinate is greater than or equal to the template's max width.
                                // If so, move the X coordinate to 0 and the Y coordinate down an entire sprite's length.
                                if (x >= template_width)
                                {
                                    x = 0;
                                    y = y + item_height;
                                }
                            }
                        }
                    }
                }
                // Here, we check if the user's settings is on "Order by Expression".
                else if (account.Setting_Sheet_Order == "Order by Expression")
                {
                    // Create a loop starting at 1 meant to iterate though every file in the directory.
                    // This loop is searching for expressions, which start at 1.
                    for (int expression = 1; expression <= filecount; expression++)
                    {
                        // Inside, create a secondary loop also meant to iterate though every file in the directory.
                        // Outfit numbers always start at 1, so we'll begin there.
                        for (int outfit = 1; outfit <= filecount; outfit++)
                        {
                            // Here, we're going to create a file path that could potentially exist given the combination of expression and outfit numbers.
                            // Check if the created file path string exists.
                            if (File.Exists($"{set_path}//{set_data.ID.ToLower()}_{expression}_{outfit}.png"))
                            {
                                // If the file path does exist, copy the file to an image variable.
                                System.Drawing.Image current_sprite = System.Drawing.Image.FromFile($"{set_path}//{set_data.ID.ToLower()}_{expression}_{outfit}.png");

                                // Draw the sprite to the template at the current X and Y coordinates.
                                graphics.DrawImage(current_sprite, x, y, item_width, item_height);

                                // Move the current X coordinate over to the right so the next sprite can be rendered right next to the current one.
                                x = x + item_width;

                                // Check if the newly positioned X coordinate is greater than or equal to the template's max width.
                                // If so, move the X coordinate to 0 and the Y coordinate down an entire sprite's length.
                                if (x >= template_width)
                                {
                                    x = 0;
                                    y = y + item_height;
                                }
                            }
                        }
                    }
                }

                // Reset the X and Y coordinates to 0.
                // Now that we're done assembling the sprites on the template, it's time to label them.
                x = 0;
                y = 0;

                // Create a single loop to iterate for as many times as there are files in the directory.
                for (int i = 1; i <= filecount; i++)
                {
                    using (Font rockwell = new Font("Rockwell", 45, FontStyle.Bold))
                    {
                        // Create a GraphicsPath object.
                        GraphicsPath myPath = new GraphicsPath();

                        // Set up all the string parameters.
                        string stringText = $"{i}";

                        System.Drawing.FontFamily family = new System.Drawing.FontFamily("Rockwell");
                        int fontStyle = (int)FontStyle.Bold;
                        int emSize = 23;
                        Point origin = new Point(x, y + 222);

                        StringFormat stringFormat = new StringFormat();
                        stringFormat.Alignment = StringAlignment.Near;
                        stringFormat.LineAlignment = StringAlignment.Center;

                        // Add the string to the path.
                        myPath.AddString(stringText,
                            family,
                            fontStyle,
                            emSize = (int)graphics.DpiY * 48 / 72, //47 pt
                            origin,
                            stringFormat);

                        //Draw the path to the screen.
                        graphics.SmoothingMode = SmoothingMode.AntiAlias;
                        graphics.FillPath(System.Drawing.Brushes.White, myPath);
                        graphics.DrawPath(new System.Drawing.Pen(System.Drawing.Brushes.Black, 4), myPath);
                        graphics.FillPath(System.Drawing.Brushes.White, myPath);
                        graphics.DrawPath(new System.Drawing.Pen(System.Drawing.Brushes.Black, 0), myPath);
                    }

                    // Move the current X coordinate over to the right so the next sprite can be rendered right next to the current one.
                    x = x + item_width;

                    // Check if the newly positioned X coordinate is greater than or equal to the template's max width.
                    // If so, move the X coordinate to 0 and the Y coordinate down an entire sprite's length.
                    if (x >= template_width)
                    {
                        x = 0;
                        y = y + item_height;
                    }
                }
            }

            return base_template;
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

            // Now, let's create a section detailing how the sprites on the sheet are sorted.
            // It wouldn't make much sense if a sort section appeared for a set with only one sprite or one type of outfit or expression, so first we need to iterate through the set.
            // Like how sets are normally sorted, we'll use the filenames to determine if there is more than one type of outfit and expression in the set.

            // Establish the directory of the specified sprite set.
            // This string structure should direct to any set containing bust-ups once the proper variables are input.
            string set_path = $@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//{set_data.Origin}//Bustup//{set_data.ID}";

            // Get a count of how many files are in the sprite set's directory.
            int filecount = OfficialSetMethods.AttachmentCountItemDirectory(set_path);

            // Create an int initialized at 0.
            // This is to check whether or not the number of outfits and expressions in the set are more than 1 without looping through the entire set.
            // Once it detects there are at least two types of outfits and expressions, we can break the iteration loop.
            int variance_check = 0;

            // Create two int variables initialized at 1.
            // These are for storing the highest value of outfits and expressions iterated over in the set.
            int highest_outfit = 1;
            int highest_expression = 1;

            // Create a loop starting at 1 meant to iterate though every file in the directory.
            // Outfit numbers always start at 1, so we'll begin there.
            for (int outfit = 1; outfit <= filecount; outfit++)
            {
                // Inside, create a secondary loop also meant to iterate though every file in the directory.
                // This loop is searching for expressions, which start at 1.
                for (int expression = 1; expression <= filecount; expression++)
                {
                    // If the current outfit value is greater than the value stored in highest_outfit, copy the current outfit value to highest_outfit.
                    if (outfit > highest_outfit)
                    {
                        highest_outfit = outfit;
                    }
                    // If the current expression value is greater than the value stored in highest_expression, copy the current expression value to highest_expression.
                    if (expression > highest_expression)
                    {
                        highest_expression = expression;
                    }

                    // Here, we're going to create a file path that could potentially exist given the combination of expression and outfit numbers.
                    // Check if the created file path string exists, the values of highest_outfit and highest_expression are both greater than 1, and the set's filecount is greater than 2.
                    if (File.Exists($"{set_path}//{set_data.ID.ToLower()}_{expression}_{outfit}.png") && highest_outfit > 1 && highest_expression > 1 && filecount > 2)
                    {
                        // Add a section to the string variable detailing the order the sprites have been organized in based on the user's settings.
                        switch (account.Setting_Sheet_Order)
                        {
                            case "Order by Outfit":
                                footer_text += $"Order: Outfit\n";
                                break;

                            case "Order by Expression":
                                footer_text += $"Order: Expression\n";
                                break;
                        }

                        variance_check++;
                        break;
                    }
                }

                if (variance_check > 0)
                {
                    break;
                }
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
                if (set_data.Origin != "P3F" && set_data.Origin != "P3P")
                {
                    if (appearances.Contains("P3F") && appearances.Contains("P3P"))
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

        // Sprite sheet messages
        public static async Task P1_PS1_Sprite_Sheet(SocketMessage message, OfficialSetData set_data)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = message.Author;
            SocketTextChannel channel = (SocketTextChannel)message.Channel;

            // Send a loading message to the channel while the sprite sheet is being made.
            RestUserMessage loader = await channel.SendMessageAsync("", false, P1_PS1_Loading_Message().Build());

            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = $"{set_data.Name}'s Conversation {Noun_Form_Of_Portrait(set_data)}",
                IconUrl = EmbedSettings.Get_Game_Thumbnail("P1-PS1")
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

            // Generate a bitmap comprised of thumbnail previews of the décor being listed on the current page.
            Bitmap sprite_set_preview = Generate_Standard_Bustup_Sprite_Sheet(message, set_data);

            // Save the sprite set preview bitmap to the stream as a PNG.
            sprite_set_preview.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);

            // Ensure the stream is set to the beginning of itself.
            memoryStream.Seek(0, SeekOrigin.Begin);

            // Send the embeded message to the channel.
            await message.Channel.SendFileAsync(memoryStream, "preview.png", "", false, embed.Build());

            // Delete the loading message.
            await loader.DeleteAsync();
        }

        public static async Task P2IS_PS1_Sprite_Sheet(SocketMessage message, OfficialSetData set_data)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = message.Author;
            SocketTextChannel channel = (SocketTextChannel)message.Channel;

            // Send a loading message to the channel while the sprite sheet is being made.
            RestUserMessage loader = await channel.SendMessageAsync("", false, P2IS_PS1_Loading_Message().Build());

            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = $"{set_data.Name}'s Conversation {Noun_Form_Of_Portrait(set_data)}",
                IconUrl = EmbedSettings.Get_Game_Thumbnail("P2IS-PS1")
            };

            embed.WithAuthor(author);

            // Set the color and thumbnail for the embeded message.
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

            // Generate a bitmap comprised of thumbnail previews of the décor being listed on the current page.
            Bitmap sprite_set_preview = Generate_Standard_Bustup_Sprite_Sheet(message, set_data);

            // Save the sprite set preview bitmap to the stream as a PNG.
            sprite_set_preview.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);

            // Ensure the stream is set to the beginning of itself.
            memoryStream.Seek(0, SeekOrigin.Begin);

            // Send the embeded message to the channel.
            await message.Channel.SendFileAsync(memoryStream, "preview.png", "", false, embed.Build());

            // Delete the loading message.
            await loader.DeleteAsync();
        }

        public static async Task P2EP_PS1_Sprite_Sheet(SocketMessage message, OfficialSetData set_data)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = message.Author;
            SocketTextChannel channel = (SocketTextChannel)message.Channel;

            // Send a loading message to the channel while the sprite sheet is being made.
            RestUserMessage loader = await channel.SendMessageAsync("", false, P2EP_PS1_Loading_Message().Build());

            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = $"{set_data.Name}'s Conversation {Noun_Form_Of_Portrait(set_data)}",
                IconUrl = EmbedSettings.Get_Game_Thumbnail("P2EP-PS1")
            };

            embed.WithAuthor(author);

            // Set the color and thumbnail for the embeded message.
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

            // Generate a bitmap comprised of thumbnail previews of the décor being listed on the current page.
            Bitmap sprite_set_preview = Generate_Standard_Bustup_Sprite_Sheet(message, set_data);

            // Save the sprite set preview bitmap to the stream as a PNG.
            sprite_set_preview.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);

            // Ensure the stream is set to the beginning of itself.
            memoryStream.Seek(0, SeekOrigin.Begin);

            // Send the embeded message to the channel.
            await message.Channel.SendFileAsync(memoryStream, "preview.png", "", false, embed.Build());

            // Delete the loading message.
            await loader.DeleteAsync();
        }

        public static async Task P3F_Sprite_Sheet(SocketMessage message, OfficialSetData set_data)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = message.Author;
            SocketTextChannel channel = (SocketTextChannel)message.Channel;

            // Send a loading message to the channel while the sprite sheet is being made.
            RestUserMessage loader = await channel.SendMessageAsync("", false, P3F_Loading_Message().Build());

            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = $"{set_data.Name}'s Conversation {Noun_Form_Of_Portrait(set_data)}",
                IconUrl = EmbedSettings.Get_Game_Thumbnail("P3F")
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

            // Generate a bitmap comprised of thumbnail previews of the décor being listed on the current page.
            Bitmap sprite_set_preview = Generate_Standard_Bustup_Sprite_Sheet(message, set_data);

            // Save the sprite set preview bitmap to the stream as a PNG.
            sprite_set_preview.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);

            // Ensure the stream is set to the beginning of itself.
            memoryStream.Seek(0, SeekOrigin.Begin);

            // Send the embeded message to the channel.
            await message.Channel.SendFileAsync(memoryStream, "preview.png", "", false, embed.Build());

            // Delete the loading message.
            await loader.DeleteAsync();
        }

        public static async Task P3P_Sprite_Sheet(SocketMessage message, OfficialSetData set_data)
        {
            try
            {
                // Create two variables for the command user and the command channel, derived from the message object taken in.
                SocketUser user = message.Author;
                SocketTextChannel channel = (SocketTextChannel)message.Channel;

                // Send a loading message to the channel while the sprite sheet is being made.
                RestUserMessage loader = await channel.SendMessageAsync("", false, P3P_Loading_Message(message).Build());

                // Get the account information of the command's user.
                var account = UserInfoClasses.GetAccount(user);

                var embed = new EmbedBuilder();
                var author = new EmbedAuthorBuilder
                {
                    Name = $"{set_data.Name}'s Conversation {Noun_Form_Of_Portrait(set_data)}",
                    IconUrl = EmbedSettings.Get_Game_Thumbnail("P3P")
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

                // Generate a bitmap comprised of thumbnail previews of the décor being listed on the current page.
                Bitmap sprite_set_preview = Generate_Standard_Bustup_Sprite_Sheet(message, set_data);

                // Save the sprite set preview bitmap to the stream as a PNG.
                sprite_set_preview.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);

                // Ensure the stream is set to the beginning of itself.
                memoryStream.Seek(0, SeekOrigin.Begin);

                // Send the embeded message to the channel.
                await message.Channel.SendFileAsync(memoryStream, "preview.png", "", false, embed.Build());

                // Delete the loading message.
                await loader.DeleteAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            
        }

        public static async Task P4_PS2_Sprite_Sheet(SocketMessage message, OfficialSetData set_data)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = message.Author;
            SocketTextChannel channel = (SocketTextChannel)message.Channel;

            // Send a loading message to the channel while the sprite sheet is being made.
            RestUserMessage loader = await channel.SendMessageAsync("", false, P4_PS2_Loading_Message().Build());

            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = $"{set_data.Name}'s Conversation {Noun_Form_Of_Portrait(set_data)}",
                IconUrl = EmbedSettings.Get_Game_Thumbnail("P4-PS2")
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

            // Generate a bitmap comprised of thumbnail previews of the décor being listed on the current page.
            Bitmap sprite_set_preview = Generate_P4_PS2_Bustup_Sprite_Sheet(message, set_data);

            // Save the sprite set preview bitmap to the stream as a PNG.
            sprite_set_preview.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);

            // Ensure the stream is set to the beginning of itself.
            memoryStream.Seek(0, SeekOrigin.Begin);

            // Send the embeded message to the channel.
            await message.Channel.SendFileAsync(memoryStream, "preview.png", "", false, embed.Build());

            // Delete the loading message.
            await loader.DeleteAsync();
        }

        public static async Task P4G_Sprite_Sheet(SocketMessage message, OfficialSetData set_data)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = message.Author;
            SocketTextChannel channel = (SocketTextChannel)message.Channel;

            // Send a loading message to the channel while the sprite sheet is being made.
            RestUserMessage loader = await channel.SendMessageAsync("", false, P4G_Loading_Message().Build());

            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = $"{set_data.Name}'s Conversation {Noun_Form_Of_Portrait(set_data)}",
                IconUrl = EmbedSettings.Get_Game_Thumbnail("P4G")
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

            // Generate a bitmap comprised of thumbnail previews of the décor being listed on the current page.
            Bitmap sprite_set_preview = Generate_Standard_Bustup_Sprite_Sheet(message, set_data);

            // Save the sprite set preview bitmap to the stream as a PNG.
            sprite_set_preview.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);

            // Ensure the stream is set to the beginning of itself.
            memoryStream.Seek(0, SeekOrigin.Begin);

            // Send the embeded message to the channel.
            await message.Channel.SendFileAsync(memoryStream, "preview.png", "", false, embed.Build());

            // Delete the loading message.
            await loader.DeleteAsync();
        }

        public static async Task P4D_Sprite_Sheet(SocketMessage message, OfficialSetData set_data)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = message.Author;
            SocketTextChannel channel = (SocketTextChannel)message.Channel;

            // Send a loading message to the channel while the sprite sheet is being made.
            RestUserMessage loader = await channel.SendMessageAsync("", false, P4D_Loading_Message().Build());

            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = $"{set_data.Name}'s Conversation {Noun_Form_Of_Portrait(set_data)}",
                IconUrl = EmbedSettings.Get_Game_Thumbnail("P4D")
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

            // Generate a bitmap comprised of thumbnail previews of the décor being listed on the current page.
            Bitmap sprite_set_preview = Generate_P4D_Bustup_Sprite_Sheet(message, set_data);

            // Save the sprite set preview bitmap to the stream as a PNG.
            sprite_set_preview.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);

            // Ensure the stream is set to the beginning of itself.
            memoryStream.Seek(0, SeekOrigin.Begin);

            // Send the embeded message to the channel.
            await message.Channel.SendFileAsync(memoryStream, "preview.png", "", false, embed.Build());

            // Delete the loading message.
            await loader.DeleteAsync();
        }

        public static async Task P5_PS4_Sprite_Sheet(SocketMessage message, OfficialSetData set_data)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = message.Author;
            SocketTextChannel channel = (SocketTextChannel)message.Channel;

            // Send a loading message to the channel while the sprite sheet is being made.
            RestUserMessage loader = await channel.SendMessageAsync("", false, P5_PS4_Loading_Message().Build());

            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = $"{set_data.Name}'s Conversation {Noun_Form_Of_Portrait(set_data)}",
                IconUrl = EmbedSettings.Get_Game_Thumbnail("P5-PS4")
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

            // Generate a bitmap comprised of thumbnail previews of the décor being listed on the current page.
            Bitmap sprite_set_preview = Generate_Standard_Bustup_Sprite_Sheet(message, set_data);

            // Save the sprite set preview bitmap to the stream as a PNG.
            sprite_set_preview.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);

            // Ensure the stream is set to the beginning of itself.
            memoryStream.Seek(0, SeekOrigin.Begin);

            // Send the embeded message to the channel.
            await message.Channel.SendFileAsync(memoryStream, "preview.png", "", false, embed.Build());

            // Delete the loading message.
            await loader.DeleteAsync();
        }

        public static async Task P5R_Sprite_Sheet(SocketMessage message, OfficialSetData set_data)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = message.Author;
            SocketTextChannel channel = (SocketTextChannel)message.Channel;

            // Send a loading message to the channel while the sprite sheet is being made.
            RestUserMessage loader = await channel.SendMessageAsync("", false, P5R_Loading_Message().Build());

            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = $"{set_data.Name}'s Conversation {Noun_Form_Of_Portrait(set_data)}",
                IconUrl = EmbedSettings.Get_Game_Thumbnail("P5R")
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

            // Generate a bitmap comprised of thumbnail previews of the décor being listed on the current page.
            Bitmap sprite_set_preview = Generate_Standard_Bustup_Sprite_Sheet(message, set_data);

            // Save the sprite set preview bitmap to the stream as a PNG.
            sprite_set_preview.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);

            // Ensure the stream is set to the beginning of itself.
            memoryStream.Seek(0, SeekOrigin.Begin);

            // Send the embeded message to the channel.
            await message.Channel.SendFileAsync(memoryStream, "preview.png", "", false, embed.Build());

            // Delete the loading message.
            await loader.DeleteAsync();
        }

        public static async Task P5S_Sprite_Sheet(SocketMessage message, OfficialSetData set_data)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = message.Author;
            SocketTextChannel channel = (SocketTextChannel)message.Channel;

            // Send a loading message to the channel while the sprite sheet is being made.
            RestUserMessage loader = await channel.SendMessageAsync("", false, P5S_Loading_Message().Build());

            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = $"{set_data.Name}'s Conversation {Noun_Form_Of_Portrait(set_data)}",
                IconUrl = EmbedSettings.Get_Game_Thumbnail("P5S")
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

            // Generate a bitmap comprised of thumbnail previews of the décor being listed on the current page.
            Bitmap sprite_set_preview = Generate_P5S_Bustup_Sprite_Sheet(message, set_data);

            // Save the sprite set preview bitmap to the stream as a PNG.
            sprite_set_preview.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);

            // Ensure the stream is set to the beginning of itself.
            memoryStream.Seek(0, SeekOrigin.Begin);

            // Send the embeded message to the channel.
            await message.Channel.SendFileAsync(memoryStream, "preview.png", "", false, embed.Build());

            // Delete the loading message.
            await loader.DeleteAsync();
        }

        public static async Task BBTAG_Sprite_Sheet(SocketMessage message, OfficialSetData set_data)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = message.Author;
            SocketTextChannel channel = (SocketTextChannel)message.Channel;

            // Send a loading message to the channel while the sprite sheet is being made.
            RestUserMessage loader = await channel.SendMessageAsync("", false, BBTAG_Loading_Message(set_data).Build());

            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = $"{set_data.Name}'s Conversation {Noun_Form_Of_Portrait(set_data)}",
                IconUrl = EmbedSettings.Get_Game_Thumbnail("BBTAG")
            };

            embed.WithAuthor(author);

            // Determine the embeded message's color based on the origin series of the sprite set.
            embed.WithColor(EmbedSettings.Get_Game_Color("BBTAG", account));

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

            // Generate a bitmap comprised of thumbnail previews of the décor being listed on the current page.
            Bitmap sprite_set_preview = Generate_BBTAG_Bustup_Sprite_Sheet(message, set_data);

            // Save the sprite set preview bitmap to the stream as a PNG.
            sprite_set_preview.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);

            // Ensure the stream is set to the beginning of itself.
            memoryStream.Seek(0, SeekOrigin.Begin);

            // Send the embeded message to the channel.
            await message.Channel.SendFileAsync(memoryStream, "preview.png", "", false, embed.Build());

            // Delete the loading message.
            await loader.DeleteAsync();
        }

        // Loading messages
        public static EmbedBuilder P1_PS1_Loading_Message()
        {
            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Generating Sprite Sheet...",
                IconUrl = EmbedSettings.Get_Game_Thumbnail("P1-PS1")
            };

            embed.WithAuthor(author);
            embed.WithColor(EmbedSettings.Get_Game_Color("P1-PS1", null));
            embed.WithThumbnailUrl("https://i.imgur.com/Lv794ze.png");
            embed.WithDescription("This may take a few seconds!");

            return embed;
        }

        public static EmbedBuilder P2IS_PS1_Loading_Message()
        {
            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = $"Generating Sprite Sheet...",
                IconUrl = EmbedSettings.Get_Game_Thumbnail("P2IS-PS1")
            };

            embed.WithAuthor(author);
            embed.WithColor(EmbedSettings.Get_Game_Color("P2IS-PS1", null));
            embed.WithDescription("This may take a few seconds!");

            return embed;
        }

        public static EmbedBuilder P2EP_PS1_Loading_Message()
        {
            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = $"Generating Sprite Sheet...",
                IconUrl = EmbedSettings.Get_Game_Thumbnail("P2EP-PS1")
            };

            embed.WithAuthor(author);
            embed.WithColor(EmbedSettings.Get_Game_Color("P2EP-PS1", null));
            embed.WithThumbnailUrl("https://i.imgur.com/KXcVCmG.png");
            embed.WithDescription("This may take a few seconds!");

            return embed;
        }

        public static EmbedBuilder P3F_Loading_Message()
        {
            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Generating Sprite Sheet...",
                IconUrl = EmbedSettings.Get_Game_Thumbnail("P3F")
            };

            embed.WithAuthor(author);
            embed.WithColor(EmbedSettings.Get_Game_Color("P3F", null));
            embed.WithThumbnailUrl("https://i.imgur.com/VwI3i20.gif");
            embed.WithDescription("This may take a few seconds!");

            return embed;
        }

        public static EmbedBuilder P3P_Loading_Message(SocketMessage message)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = message.Author;

            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Generating Sprite Sheet...",
                IconUrl = EmbedSettings.Get_Game_Thumbnail("P3P")
            };

            embed.WithAuthor(author);

            // Assign a color based on the user's color setting for the P3P template.
            embed.WithColor(EmbedSettings.Get_Game_Color("P3P", account));

            embed.WithDescription("This may take a few seconds!");

            return embed;
        }

        public static EmbedBuilder P4_PS2_Loading_Message()
        {
            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Generating Sprite Sheet...",
                IconUrl = EmbedSettings.Get_Game_Thumbnail("P4-PS2")
            };

            embed.WithAuthor(author);
            embed.WithColor(EmbedSettings.Get_Game_Color("P4-PS2", null));
            embed.WithThumbnailUrl("https://i.imgur.com/Nr5mEap.gif");
            embed.WithDescription("This may take a few seconds!");

            return embed;
        }

        public static EmbedBuilder P4G_Loading_Message()
        {
            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Generating Sprite Sheet...",
                IconUrl = EmbedSettings.Get_Game_Thumbnail("P4G")
            };

            embed.WithAuthor(author);
            embed.WithColor(EmbedSettings.Get_Game_Color("P4G", null));
            embed.WithThumbnailUrl("https://i.imgur.com/8FOF81K.gif");
            embed.WithDescription("This may take a few seconds!");

            return embed;
        }

        public static EmbedBuilder P4D_Loading_Message()
        {
            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Generating Sprite Sheet...",
                IconUrl = EmbedSettings.Get_Game_Thumbnail("P4D")
            };

            embed.WithAuthor(author);
            embed.WithColor(EmbedSettings.Get_Game_Color("P4D", null));
            embed.WithThumbnailUrl(Randomize_P4D_Gif());
            embed.WithDescription("This may take a few seconds!");

            return embed;
        }

        public static EmbedBuilder P5_PS4_Loading_Message()
        {
            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Generating Sprite Sheet...",
                IconUrl = EmbedSettings.Get_Game_Thumbnail("P5-PS4")
            };

            embed.WithAuthor(author);
            embed.WithColor(EmbedSettings.Get_Game_Color("P5-PS4", null));
            embed.WithThumbnailUrl("https://i.imgur.com/PYMB6XG.gif");
            embed.WithDescription("This may take a few seconds!");

            return embed;
        }

        public static EmbedBuilder P5R_Loading_Message()
        {
            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Generating Sprite Sheet...",
                IconUrl = EmbedSettings.Get_Game_Thumbnail("P5R")
            };

            embed.WithAuthor(author);
            embed.WithColor(EmbedSettings.Get_Game_Color("P5R", null));
            embed.WithThumbnailUrl("https://i.imgur.com/PYMB6XG.gif");
            embed.WithDescription("This may take a few seconds!");

            return embed;
        }

        public static EmbedBuilder P5S_Loading_Message()
        {
            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Generating Sprite Sheet...",
                IconUrl = EmbedSettings.Get_Game_Thumbnail("P5S")
            };

            embed.WithAuthor(author);
            embed.WithColor(EmbedSettings.Get_Game_Color("P5S", null));
            //embed.WithThumbnailUrl("https://i.imgur.com/YA9WUNA.gif");
            embed.WithThumbnailUrl("https://i.imgur.com/IkrlV0c.gif");
            embed.WithDescription("This may take a few seconds!");

            return embed;
        }

        public static EmbedBuilder BBTAG_Loading_Message(OfficialSetData set_data)
        {
            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Loading Sprite Sheet...",
                IconUrl = EmbedSettings.Get_Game_Thumbnail("BBTAG")
            };

            embed.WithAuthor(author);

            // Determine the embeded message's color based on the origin series of the sprite set.
            switch (set_data.Series)
            {
                case "BlazBlue":
                    embed.WithColor(66, 119, 255);
                    break;

                case "Persona 4 Arena":
                    embed.WithColor(250, 238, 50);
                    break;

                case "Under Night In-Birth":
                    embed.WithColor(141, 72, 249);
                    break;

                case "RWBY":
                    embed.WithColor(250, 50, 85);
                    break;

                case "Arcana Heart":
                    embed.WithColor(255, 69, 175);
                    break;

                case "Senran Kagura":
                    embed.WithColor(203, 223, 255);
                    break;

                case "Akatsuki En-Eins":
                    embed.WithColor(188, 170, 141);
                    break;

                default:
                    embed.WithColor(250, 238, 50);
                    break;
            }

            embed.WithThumbnailUrl("https://i.imgur.com/f6dSxc1.gif");
            embed.WithDescription("This may take a few seconds!");

            return embed;
        }

        // Supplimental methods
        public static string Noun_Form_Of_Portrait(OfficialSetData set_data)
        {
            string noun_form = "";

            // Establish the directory of the specified sprite set.
            // This string structure should direct to any set containing bust-ups once the proper variables are input.
            string set_path = $@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//{set_data.Origin}//Bustup//{set_data.ID}";

            // Get a count of how many files are in the sprite set's directory.
            int filecount = OfficialSetMethods.AttachmentCountItemDirectory(set_path);

            if (filecount > 1)
            {
                noun_form = "Portraits";
            }
            else
            {
                noun_form = "Portrait";
            }

            return noun_form;
        }

        public static string Randomize_P4D_Gif()
        {
            // Create a random variable.
            Random r = new Random();

            // Create an empty string variable that will return as the final answer.
            string imgurl = "";

            // P3F GIFs are scenes that exclusively apply to the FES version of P3.
            string[] p4d_loading_icons = new string[]
            {
                "https://i.imgur.com/sfn7xIQ.gif",
                "https://i.imgur.com/oCT0vi7.gif",
                "https://i.imgur.com/itKoJAD.gif",
                "https://i.imgur.com/psRGa2G.gif",
                "https://i.imgur.com/hIOykos.gif",
                "https://i.imgur.com/7tkBnC8.gif",
                "https://i.imgur.com/3W8PaMZ.gif",
                "https://i.imgur.com/c0pwkMY.gif",
                "https://i.imgur.com/7drzbNI.gif",
                "https://i.imgur.com/rGmuwTd.gif",
                "https://i.imgur.com/EgZYZbc.gif"
            };

            imgurl = p4d_loading_icons[r.Next(0, p4d_loading_icons.Length)];

            return imgurl;
        }
    }
}
