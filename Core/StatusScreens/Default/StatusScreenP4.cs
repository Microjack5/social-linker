using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Rest;
using Discord.Commands;
using Discord.WebSocket;
using Fergun.Interactive;
using SocialLinker.Config;
using SocialLinker.Core.CloudStorageTables;
using System.Drawing.Text;
using System.Net;
using System.Drawing.Drawing2D;

namespace SocialLinker.Core.StatusScreens
{
    internal static class StatusScreenP4
    {
        internal static async void RenderImage(SocketUser user, ISocketMessageChannel channel)
        {
            RestUserMessage loader = await channel.SendMessageAsync("", false, LoadingMessage().Build());

            // Place the bulk of the function in a try-catch block in case something fails and an error message needs to be sent
            try
            {
                // Grab the user's account information
                var account = UserInfoClasses.GetAccount(user);

                //Establish variables to write on the template
                string username = "";

                //If the username is over 12 characters, replace the last parts with an ellipsis
                if (user.Username.Length > 12)
                {
                    username = $"{username.Substring(0, 12)}...";
                }
                else
                {
                    username = $"{user.Username}";
                }

                //Establish other variables of the user's data
                string level = $"{account.Level}";
                string profile_picture = user.GetAvatarUrl();
                string pmedals = $"{account.P_Medals}";
                string proficiency_title = Core.LevelSystem.SocialStats.ProficiencyRankTitle(account.Proficiency_Rank);
                string diligence_title = Core.LevelSystem.SocialStats.DiligenceRankTitle(account.Diligence_Rank);
                string expression_title = Core.LevelSystem.SocialStats.ExpressionRankTitle(account.Expression_Rank);

                //Determine the Next Exp value
                int next_exp = 0;
                if (account.Level != 99)
                {
                    next_exp = Core.LevelSystem.Leveling.CalculateExp(account.Level + 1) - account.Total_Exp;
                }

                //If the user doesn't have a profile picture, use a default one
                if (profile_picture == null)
                {
                    profile_picture = "https://i.imgur.com/T0AjCLh.png";
                }

                // Create a base bitmap to render all the elements on
                Bitmap p4_template = new Bitmap(1920, 1080);

                //Copy the P4 status background to a bitmap
                Bitmap base_layer = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//Profile//StatusScreens//Default//P4//layer_1.png");

                //Copy the P4 status info fields to a bitmap
                Bitmap info_layer = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//Profile//StatusScreens//Default//P4//layer_2.png");

                //Copy the plot point overlay to a bitmap
                Bitmap graph_overlay = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//Profile//StatusScreens//Default//P4//layer_3.png");

                //Copy the P-Medal icon to a bitmap
                Bitmap pmedal_icon = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//Profile//StatusScreens//Default//P4//pmedal_icon.png");

                //Use a graphics object to edit the bitmap
                using (Graphics graphics = Graphics.FromImage(p4_template))
                {
                    //Set text rendering to have antialiasing
                    graphics.TextRenderingHint = TextRenderingHint.AntiAlias;

                    // Draw the first bitmap layer to the template
                    graphics.DrawImage(base_layer, 0, 0, base_layer.Width, base_layer.Height);

                    //Draw the randomized elements to the template first
                    graphics.DrawImage(RandomizeWaveAndWindow(), 0, 0, 1920, 1080);

                    //Draw the information fields
                    graphics.DrawImage(info_layer, 0, 0, 1920, 1080);

                    //Using a font object, draw the user's username value to the template
                    using (Font p4g_font = new Font("P4G", 37))
                    {
                        graphics.DrawString(username, p4g_font, System.Drawing.Brushes.Black, new Point(276, 139));
                    }

                    //Create text boxes to place the user's level and P-Medal values in
                    Rectangle level_box = new Rectangle(70, 84, 210, 100);
                    Rectangle pmedal_box = new Rectangle(50, 180, 210, 100);
                    Rectangle next_exp_box = new Rectangle(575, 222, 1000, 100);

                    //Create new colors to shade the level and P-Medal values with
                    System.Drawing.Color level_color = System.Drawing.Color.FromArgb(255, 247, 130);
                    System.Drawing.Color pmedal_color = System.Drawing.Color.FromArgb(225, 108, 1);

                    //Create new brushes so that the colors can be used on a font object
                    SolidBrush level_brush = new SolidBrush(level_color);
                    SolidBrush pmedal_brush = new SolidBrush(pmedal_color);

                    //Using a font object, draw the user's level value to the template
                    using (Font p4g_stats_font = new Font("P4G Stats", 68))
                    {
                        //Format the string so that its placement is on the right side of the text box
                        StringFormat stringFormat = new StringFormat();
                        stringFormat.Alignment = StringAlignment.Far;

                        graphics.DrawString(level, p4g_stats_font, level_brush, level_box, stringFormat);
                    }

                    //Draw P-Medal text and graphic information within this font object
                    using (Font p4g_stats_font = new Font("P4G Stats", 48))
                    {
                        //Format the string so that its placement is on the right side of the text box
                        StringFormat stringFormat = new StringFormat();
                        stringFormat.Alignment = StringAlignment.Far;

                        //If the user's P-Medal count has 2 or less digits, draw the P-Medal icon in a specific place
                        if (pmedals.Length <= 2)
                        {
                            graphics.DrawImage(pmedal_icon, 85, 189, 72, 52);
                        }
                        //Else, if the user's P-Medal count has 3 digits, draw the P-Medal icon in a different place
                        else if (pmedals.Length == 3)
                        {
                            graphics.DrawImage(pmedal_icon, 55, 189, 72, 52);
                        }

                        //Draw the user's P-Medal value to the screen
                        graphics.DrawString(pmedals, p4g_stats_font, pmedal_brush, pmedal_box, stringFormat);
                    }

                    //Draw the user's next EXP value to the template within this font object
                    using (Font p4g_stats_font = new Font("P4G Stats", 47))
                    {
                        //Format the string so that its placement is on the left side of the text box
                        StringFormat stringFormat = new StringFormat();
                        stringFormat.Alignment = StringAlignment.Near;

                        graphics.DrawString($"{next_exp}", p4g_stats_font, System.Drawing.Brushes.Black, next_exp_box, stringFormat);
                    }

                    //Create text boxes to draw the social link rank titles
                    Rectangle proficiency_title_box = new Rectangle(1163, 233, 309, 53);
                    Rectangle diligence_title_box = new Rectangle(834, 860, 309, 53);
                    Rectangle expression_title_box = new Rectangle(1493, 859, 309, 53);

                    //Draw the rank titles onto the template
                    using (Font p4g_font = new Font("P4G", 31))
                    {
                        //Format the strings so that their placements are at the center of the text boxes
                        StringFormat stringFormat = new StringFormat();
                        stringFormat.Alignment = StringAlignment.Center;

                        graphics.DrawString(proficiency_title, p4g_font, System.Drawing.Brushes.White, proficiency_title_box, stringFormat);
                        graphics.DrawString(diligence_title, p4g_font, System.Drawing.Brushes.White, diligence_title_box, stringFormat);
                        graphics.DrawString(expression_title, p4g_font, System.Drawing.Brushes.White, expression_title_box, stringFormat);
                    }

                    //Create points that define radar chart
                    Point proficiency_point = ProficiencyGraphPoint(account.Proficiency_Rank);
                    Point diligence_point = DiligenceGraphPoint(account.Diligence_Rank);
                    Point expression_point = ExpressionGraphPoint(account.Expression_Rank);

                    //Create rectangles for large endpoints that emphasize where the radar points are
                    Rectangle proficiency_endpoint = ProficiencyEndpoint(account.Proficiency_Rank);
                    Rectangle diligence_endpoint = DiligenceEndpoint(account.Diligence_Rank);
                    Rectangle expression_endpoint = ExpressionEndpoint(account.Expression_Rank);

                    //Create a color for the radar chart
                    SolidBrush orangeBrush = new SolidBrush(System.Drawing.Color.Orange);

                    //Bind radar chart points
                    Point[] curvePoints = { proficiency_point, diligence_point, expression_point };

                    //Draw radar chart to screen
                    graphics.FillPolygon(orangeBrush, curvePoints);

                    //Draw the plot point overlay to the template
                    graphics.DrawImage(graph_overlay, 0, 0, 1920, 1080);

                    //Create a black pen to draw large endpoints over the plot point overlay
                    System.Drawing.Pen blackPen = new System.Drawing.Pen(System.Drawing.Color.Black, 10);

                    //Draw the large endpoints to the template
                    graphics.DrawEllipse(blackPen, proficiency_endpoint);
                    graphics.DrawEllipse(blackPen, diligence_endpoint);
                    graphics.DrawEllipse(blackPen, expression_endpoint);

                    //Use a web client to download the user's profile picture and draw it to the template
                    using (var wc = new WebClient())
                    {
                        using (var imgStream = new MemoryStream(wc.DownloadData(profile_picture)))
                        {
                            using (var objImage = System.Drawing.Image.FromStream(imgStream))
                            {
                                graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                                graphics.DrawImage(objImage, 250, 447, 440, 440);
                            }
                        }
                    }

                    // If the user has ever reset their level, render a prestige counter to the template
                    if (account.Level_Resets > 0)
                    {
                        graphics.DrawImage(RenderPrestigeCounter(account.Level_Resets), 0, 0, 1920, 1080);
                    }
                }

                //Save the bitmap to a data stream
                MemoryStream memoryStream = new MemoryStream();
                p4_template.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
                memoryStream.Seek(0, SeekOrigin.Begin);

                //Send the image
                await channel.SendFileAsync(memoryStream, $"status_{user.Id}_{DateTime.UtcNow}.png");

                //Delete the loading message
                await loader.DeleteAsync();
            }
            catch (Exception ex)
            {
                //Send an error message to the user
                _ = ErrorHandling.Scene_Upload_Failed(user, channel);
                Console.WriteLine(ex);

                //Delete the loading message
                await loader.DeleteAsync();

                return;
            }
        }

        public static Bitmap RandomizeWaveAndWindow()
        {
            //Create a new bitmap to return at the end
            Bitmap randomized_layer = new Bitmap(1920, 1080);

            //Assign the wave shape to a bitmap
            Bitmap wave = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//Profile//StatusScreens//Default//P4//wave.png");

            //Assign the orange wave to a bitmap
            Bitmap wave_color = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//Profile//StatusScreens//Default//P4//wave_color.png");

            //Assign the window to a bitmap
            Bitmap window = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//Profile//StatusScreens//Default//P4//window.png");

            //Create a random variable
            Random rnd = new Random();

            //Create a new bitmap to place the wave shape on before changing its color
            Bitmap placed_wave = new Bitmap(1920, 1080);

            //Using a graphics object, draw the wave on the placed_wave bitmap
            using (Graphics graphics = Graphics.FromImage(placed_wave))
            {
                graphics.DrawImage(wave, 128, rnd.Next(-712, 0), 19, 1792);
            }

            //Overlap the proper gradient color with the placed_wave layer and assign it to the randomized_layer bitmap
            randomized_layer = KeepPixelOverlap(placed_wave, wave_color);

            //Use a graphics object to randomize the window placement
            using (Graphics graphics = Graphics.FromImage(randomized_layer))
            {
                //Create a nested loop to randomly draw the window.
                //"i" equals the width of the window plus the spacing
                for (int i = 736; i < 1920; i += (113 + 6))
                {
                    //"j" equals the height of the window plus the spacing
                    for (int j = 0; j < 1080; j += (94 + 10))
                    {
                        // Create a randomized int for one of two options
                        int color_options = rnd.Next(1, 3);

                        // Depending on the number in color_options, render the window bitmap a different shade of yellow. This number is changed every time the loop iterates.
                        if (color_options == 1)
                        {
                            window = BitmapToColor(window, 253, 255, 42);
                        }
                        else if (color_options == 2)
                        {
                            window = BitmapToColor(window, 250, 251, 92);
                        }

                        //Create an appearance rate integer randomized between 0 and 9
                        int appearance_rate = rnd.Next(0, 10);

                        //If the appearance rate is even, draw the window at the current position. Remove one of the accepted results to reduce the draw rate even further.
                        if ((appearance_rate % 2) == 0 && (appearance_rate != 0))
                        {
                            graphics.DrawImage(window, i, j, 113, 94);
                        }
                    }
                }
            }

            return randomized_layer;
        }

        public static Bitmap KeepPixelOverlap(Bitmap bottom_bitmap, Bitmap top_bitmap)
        {
            System.Drawing.Color bottom_pixel_color;
            System.Drawing.Color top_pixel_color;

            //Make an empty bitmap the same size as scrBitmap
            Bitmap newBitmap = new Bitmap(bottom_bitmap.Width, bottom_bitmap.Height);

            for (int i = 128; i < 168; i++)
            {
                for (int j = 0; j < 1080; j++)
                {
                    //Get the pixel from the scrBitmap image
                    bottom_pixel_color = bottom_bitmap.GetPixel(i, j);
                    top_pixel_color = top_bitmap.GetPixel(i, j);

                    //Create a new color with the transparency of the bottom layer and the color of the top layer
                    System.Drawing.Color new_color = System.Drawing.Color.FromArgb(bottom_pixel_color.A, top_pixel_color.R, top_pixel_color.G, top_pixel_color.B);

                    //Draw the new color's pixel if both the top and bottom layers overlap
                    newBitmap.SetPixel(i, j, new_color);
                }
            }

            return newBitmap;
        }

        public static Point ProficiencyGraphPoint(int rank)
        {
            Point graph_point = new Point(0, 0);

            //Set chart point for Proficiency rank
            if (rank == 1)
            {
                graph_point = new Point(1318, 561);
            }
            else if (rank == 2)
            {
                graph_point = new Point(1318, 505);
            }
            else if (rank == 3)
            {
                graph_point = new Point(1318, 449);
            }
            else if (rank == 4)
            {
                graph_point = new Point(1318, 393);
            }
            else if (rank == 5)
            {
                graph_point = new Point(1318, 335);
            }

            return graph_point;
        }

        public static Point DiligenceGraphPoint(int rank)
        {
            Point graph_point = new Point(0, 0);

            //Set chart point for Diligence rank
            if (rank == 1)
            {
                graph_point = new Point(1271, 645);
            }
            else if (rank == 2)
            {
                graph_point = new Point(1222, 673);
            }
            else if (rank == 3)
            {
                graph_point = new Point(1174, 700);
            }
            else if (rank == 4)
            {
                graph_point = new Point(1125, 729);
            }
            else if (rank == 5)
            {
                graph_point = new Point(1075, 757);
            }

            return graph_point;
        }

        public static Point ExpressionGraphPoint(int rank)
        {
            Point graph_point = new Point(0, 0);

            //Set chart point for Expression rank
            if (rank == 1)
            {
                graph_point = new Point(1367, 645);
            }
            else if (rank == 2)
            {
                graph_point = new Point(1415, 673);
            }
            else if (rank == 3)
            {
                graph_point = new Point(1463, 700);
            }
            else if (rank == 4)
            {
                graph_point = new Point(1512, 729);
            }
            else if (rank == 5)
            {
                graph_point = new Point(1562, 757);
            }

            return graph_point;
        }

        public static Rectangle ProficiencyEndpoint(int rank)
        {
            //Create a variable for the width and height of the rectangle
            int size = 11;

            //Create a variable to center the rectangle at the point by subtracting this value from it
            int center_displacement = 6;

            //Create an empty rectangle to start with and return at the end
            Rectangle endpoint = new Rectangle(0, 0, size, size);

            //Set endpoint depending on Proficiency rank
            if (rank == 1)
            {
                endpoint = new Rectangle(1318 - center_displacement, 561 - center_displacement, size, size);
            }
            else if (rank == 2)
            {
                endpoint = new Rectangle(1318 - center_displacement, 505 - center_displacement, size, size);
            }
            else if (rank == 3)
            {
                endpoint = new Rectangle(1318 - center_displacement, 449 - center_displacement, size, size);
            }
            else if (rank == 4)
            {
                endpoint = new Rectangle(1318 - center_displacement, 393 - center_displacement, size, size);
            }
            else if (rank == 5)
            {
                endpoint = new Rectangle(1318 - center_displacement, 335 - center_displacement, size, size);
            }

            return endpoint;
        }

        public static Rectangle DiligenceEndpoint(int rank)
        {
            //Create a variable for the width and height of the rectangle
            int size = 11;

            //Create a variable to center the rectangle at the point by subtracting this value from it
            int center_displacement = 6;

            //Create an empty rectangle to start with and return at the end
            Rectangle endpoint = new Rectangle(0, 0, size, size);

            //Set endpoint depending on Diligence rank
            if (rank == 1)
            {
                endpoint = new Rectangle(1271 - center_displacement, 645 - center_displacement, size, size);
            }
            else if (rank == 2)
            {
                endpoint = new Rectangle(1222 - center_displacement, 673 - center_displacement, size, size);
            }
            else if (rank == 3)
            {
                endpoint = new Rectangle(1174 - center_displacement, 700 - center_displacement, size, size);
            }
            else if (rank == 4)
            {
                endpoint = new Rectangle(1125 - center_displacement, 729 - center_displacement, size, size);
            }
            else if (rank == 5)
            {
                endpoint = new Rectangle(1075 - center_displacement, 757 - center_displacement, size, size);
            }

            return endpoint;
        }

        public static Rectangle ExpressionEndpoint(int rank)
        {
            //Create a variable for the width and height of the rectangle
            int size = 11;

            //Create a variable to center the rectangle at the point by subtracting this value from it
            int center_displacement = 6;

            //Create an empty rectangle to start with and return at the end
            Rectangle endpoint = new Rectangle(0, 0, size, size);

            //Set endpoint depending on Expression rank
            if (rank == 1)
            {
                endpoint = new Rectangle(1367 - center_displacement, 645 - center_displacement, size, size);
            }
            else if (rank == 2)
            {
                endpoint = new Rectangle(1415 - center_displacement, 673 - center_displacement, size, size);
            }
            else if (rank == 3)
            {
                endpoint = new Rectangle(1463 - center_displacement, 700 - center_displacement, size, size);
            }
            else if (rank == 4)
            {
                endpoint = new Rectangle(1512 - center_displacement, 729 - center_displacement, size, size);
            }
            else if (rank == 5)
            {
                endpoint = new Rectangle(1562 - center_displacement, 757 - center_displacement, size, size);
            }

            return endpoint;
        }

        public static Bitmap RenderPrestigeCounter(int level_resets)
        {
            // Copy the prestige counter overlay to a bitmap.
            Bitmap prestige_overlay = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//Profile//StatusScreens//Default//P4//prestige_counter.png");

            // Copy the star to mark prestige to a bitmap.
            Bitmap prestige_star = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//Profile//StatusScreens//Default//P4//star.png");

            // Color the star to a custom gray color.
            prestige_star = BitmapToColor(prestige_star, 45, 45, 45);

            // Create a new bitmap.
            Bitmap new_bitmap = new Bitmap(1920, 1080);

            // Use a graphics object to edit the bitmap.
            using (Graphics graphics = Graphics.FromImage(new_bitmap))
            {
                // Draw the prestige overlay to the template.
                graphics.DrawImage(prestige_overlay, 0, 0, 1920, 1080);

                // Create a new int variable that shares the same value as level_resets.
                int star_counter = level_resets;

                // If the value of star_counter is over 3, set it back to 3.
                if (star_counter > 3)
                {
                    star_counter = 3;
                }

                // Draw as many stars to the prestige overlay as needed.
                for (int i = 0; i < star_counter; i++)
                {
                    graphics.DrawImage(prestige_star, 1580 + (i * 80), 160, 50, 50);
                }
            }

            return new_bitmap;
        }

        public static Bitmap BitmapToColor(Bitmap input_bitmap, int r_value, int g_value, int b_value)
        {
            //Create a color variable to represent the color of a pixel on the input bitmap
            System.Drawing.Color actual_color;

            //Create a color variable to represent a new created color
            System.Drawing.Color new_color;

            //Make a new bitmap the same size as input_bitmap
            Bitmap new_bitmap = new Bitmap(input_bitmap.Width, input_bitmap.Height);

            //Create a nested loop that iterates over every pixel of the input bitmap
            for (int i = 0; i < input_bitmap.Width; i++)
            {
                for (int j = 0; j < input_bitmap.Height; j++)
                {
                    //Get a pixel from the input_bitmap image
                    actual_color = input_bitmap.GetPixel(i, j);

                    //Assign the new_color variable to the pixel's transparency value, but change the color itself to the specified input values
                    new_color = System.Drawing.Color.FromArgb(actual_color.A, r_value, g_value, b_value);

                    //Draw the new colored pixel to the new bitmap
                    new_bitmap.SetPixel(i, j, new_color);
                }
            }

            //Return the new bitmap
            return new_bitmap;
        }

        public static EmbedBuilder LoadingMessage()
        {
            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Loading Status...",
                IconUrl = "https://i.imgur.com/8Qs9g1d.png"
            };

            embed.WithAuthor(author);
            embed.WithColor(255, 229, 49);
            embed.WithThumbnailUrl("https://i.imgur.com/Nr5mEap.gif");
            embed.WithDescription("This may take a few seconds!");

            return embed;
        }
    }
}
