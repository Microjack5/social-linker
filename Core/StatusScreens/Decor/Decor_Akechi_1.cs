using System;
using System.Drawing;
using System.IO;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using SocialLinker.Config;
using SocialLinker.Core.CloudStorageTables;
using System.Drawing.Text;
using System.Net;
using System.Drawing.Drawing2D;

namespace SocialLinker.Core.StatusScreens.Decor
{
    internal class Decor_Akechi_1
    {
        public static async void RenderImage(SocketUser user, ISocketMessageChannel channel)
        {
            // Send a loading message while the status screen gets made
            RestUserMessage loader = await channel.SendMessageAsync("", false, LoadingMessage().Build());

            // Place the bulk of the function in a try-catch block in case something fails and an error message needs to be sent
            try
            {
                // Grab the user's account information
                var account = UserInfoClasses.GetAccount(user);

                // Establish variables to write on the template
                string username = "";

                // If the username is over 16 characters, replace the last parts with an ellipsis
                if (user.Username.Length > 20)
                {
                    username = $"{username.Substring(0, 20)}...";
                }
                else
                {
                    username = $"{user.Username}";
                }

                // Establish other variables of the user's data
                string level = $"{account.Level}";
                int total_exp = account.Total_Exp;
                string profile_picture_url = user.GetAvatarUrl();
                string pmedals = $"{account.P_Medals}";
                string proficiency_title = Core.LevelSystem.SocialStats.ProficiencyRankTitle(account.Proficiency_Rank);
                string diligence_title = Core.LevelSystem.SocialStats.DiligenceRankTitle(account.Diligence_Rank);
                string expression_title = Core.LevelSystem.SocialStats.ExpressionRankTitle(account.Expression_Rank);

                // Determine the Next Exp value
                int next_exp = 0;
                if (account.Level != 99)
                {
                    next_exp = Core.LevelSystem.Leveling.CalculateExp(account.Level + 1) - account.Total_Exp;
                }

                // If the user doesn't have a profile picture, use a default one
                if (profile_picture_url == null)
                {
                    profile_picture_url = "https://i.imgur.com/T0AjCLh.png";
                }

                // Create a base bitmap to render all the elements on
                Bitmap p5_template = new Bitmap(1920, 1080);

                // Copy the P5 status background to a bitmap
                Bitmap base_layer = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//Profile//StatusScreens//Decor//Decor_Akechi_1//layer_1.png");

                // Copy the game textures to a bitmap
                Bitmap game_textures = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//Profile//StatusScreens//Decor//Decor_Akechi_1//layer_2.png");

                // Copy the graph decoration layer to a bitmap
                Bitmap graph_decoration = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//Profile//StatusScreens//Decor//Decor_Akechi_1//layer_3.png");

                // Copy the center of the graph to a bitmap
                Bitmap graph_center = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//Profile//StatusScreens//Decor//Decor_Akechi_1//layer_4.png");

                // Copy the star shading overlay to a bitmap
                Bitmap shading_overlay = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//Profile//StatusScreens//Decor//Decor_Akechi_1//layer_5.png");

                // Copy the plot point overlay to a bitmap
                Bitmap plot_point_overlay = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//Profile//StatusScreens//Decor//Decor_Akechi_1//layer_6.png");

                // Confetti field
                Bitmap confetti_field = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//Profile//StatusScreens//Decor//Decor_Akechi_1//layer_7.png");

                // Create a random variable and rotate poster a random amount between -12 and 0 degrees
                Random rnd = new Random();
                //Bitmap rotated_poster = RotateImage(wanted_poster, rnd.Next(-12, 1));

                // Use a graphics object to edit the bitmap
                using (Graphics graphics = Graphics.FromImage(p5_template))
                {
                    // Set text rendering to have antialiasing
                    graphics.TextRenderingHint = TextRenderingHint.AntiAlias;

                    // Color the entire bitmap with a bright red color
                    //graphics.Clear(System.Drawing.Color.FromArgb(213, 27, 4));

                    // Draw the first bitmap layer to the template
                    graphics.DrawImage(base_layer, 0, 0, base_layer.Width, base_layer.Height);

                    // Draw the profile picture and its vectorized background to the template
                    graphics.DrawImage(RenderProfilePictureLayer((SocketGuildUser)user), 0, 0, 1920, 1080);

                    // Draw the decoration layer to the template
                    graphics.DrawImage(graph_decoration, 0, 0, 1920, 1080);

                    // Render the user info box that will contain the user's username, level, and Next Level information
                    graphics.DrawImage(RenderUserInfoBox(), -69, -2, 1920, 1080);

                    // Render the vector that contains the word "Stats" to the template
                    //graphics.DrawImage(RenderStatsVector(), 0, 0, 1920, 1080);

                    // Render the vector that contains the user's P-Medal value to the template
                    graphics.DrawImage(RenderPMedalVector(), 0, 0, 1920, 1080);

                    // Render textures taken directly from the game such as the Lv, Next Level, P-Medal icon, and Stats text to the template
                    graphics.DrawImage(game_textures, 0, 0, 1920, 1080);

                    // Create a new bitmap that contains the user's username
                    Bitmap username_box = RenderUsername(username);

                    // Rotate the username bitmap by 3 degrees and save it to a new bitmap
                    Bitmap rotated_username_box = RotateImage(username_box, 3);

                    // Draw the rotated bitmap to the template
                    graphics.DrawImage(rotated_username_box, -10, 23, rotated_username_box.Width, rotated_username_box.Height);

                    // Create a text box to place the user's P-Medal value in
                    Rectangle pmedal_box = new Rectangle(779, 29, 144, 73);

                    // Create a new bitmap that will contain the user's level
                    Bitmap level_box = RenderLevel(level);

                    // Draw the level_box bitmap to the template
                    graphics.DrawImage(level_box, 416, 202, level_box.Width, level_box.Height);

                    // Next, create a new bitmap that contains the user's next EXP value
                    Bitmap next_exp_box = RenderNextExpBitmap(next_exp.ToString());

                    // Rotate the next EXP bitmap by -3 degrees and save it to a new bitmap
                    Bitmap rotated_next_exp_box = RotateImage(next_exp_box, -6);

                    // Draw the rotated next EXP bitmap to the template
                    graphics.DrawImage(rotated_next_exp_box, 594, 166, rotated_next_exp_box.Width, rotated_next_exp_box.Height);

                    // Using a font object, draw the user's P-Medal value to the template
                    using (Font p5r_stats_font = new Font("P5R Stats", 37))
                    {
                        // Format the string so that its placement is on the right side of the text box
                        StringFormat stringFormat = new StringFormat();
                        stringFormat.Alignment = StringAlignment.Center;
                        stringFormat.LineAlignment = StringAlignment.Center;

                        graphics.DrawString(pmedals, p5r_stats_font, System.Drawing.Brushes.Black, pmedal_box, stringFormat);
                    }

                    // Render the Total EXP bitmap
                    Bitmap total_exp_bitmap = RenderTotalExpBitmap(TotalExpToString(total_exp), total_exp.ToString().Length);
                    total_exp_bitmap = RotateImage(total_exp_bitmap, -13);
                    graphics.DrawImage(total_exp_bitmap, 226, 884, 476, 161);

                    // Draw the social stat icons to the template
                    Bitmap stat_icons = RenderStatIcons(account.Proficiency_Rank, account.Diligence_Rank, account.Expression_Rank);
                    Bitmap stat_icons_drop_shadow = BitmapToColor(stat_icons, 0, 0, 0);
                    graphics.DrawImage(stat_icons_drop_shadow, 5, 5, 1920, 1080);
                    graphics.DrawImage(stat_icons, 0, 0, 1920, 1080);

                    // Draw the radar chart to the template
                    Bitmap radar_chart = RenderRadarChart(account.Proficiency_Rank, account.Diligence_Rank, account.Expression_Rank);
                    graphics.DrawImage(radar_chart, 0, 0, 1920, 1080);
                    graphics.DrawImage(graph_center, 0, 0, 1920, 1080);

                    // Draw the star shading overlay to the template
                    Bitmap cropped_shading_overlay = KeepPixelOverlap(radar_chart, shading_overlay);
                    graphics.DrawImage(cropped_shading_overlay, 0, 0, 1920, 1080);

                    // Draw the plot point overlay to the template
                    graphics.DrawImage(plot_point_overlay, 0, 0, 1920, 1080);

                    // Create a bitmap where the user's three social rank titles are displayed
                    Bitmap rank_titles_layer = RenderAllRankTitles(account.Proficiency_Rank, account.Diligence_Rank, account.Expression_Rank);
                    Bitmap rank_titles_layer_drop_shadow = BitmapToColor(rank_titles_layer, 0, 0, 0);

                    // Draw the rank title bitmap to the template
                    graphics.DrawImage(rank_titles_layer_drop_shadow, 3, 3, rank_titles_layer.Width, rank_titles_layer.Height);
                    graphics.DrawImage(rank_titles_layer, 0, 0, rank_titles_layer.Width, rank_titles_layer.Height);

                    // If the user has ever reset their level, render a prestige counter to the template
                    if (account.Level_Resets > 0)
                    {
                        graphics.DrawImage(RenderPrestigeCounter(account.Level_Resets), 0, 0, 1920, 1080);
                    }

                    graphics.DrawImage(confetti_field, 0, 0, 1920, 1080);
                }

                // Save the bitmap to a data stream
                MemoryStream memoryStream = new MemoryStream();
                //RotateImage(RenderTotalExpBitmap(TotalExpToString(total_exp), total_exp.ToString().Length), -13).Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
                p5_template.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
                memoryStream.Seek(0, SeekOrigin.Begin);

                // Send the image
                await channel.SendFileAsync(memoryStream, $"status_{user.Id}_{DateTime.UtcNow}.png");

                // Delete the loading message
                await loader.DeleteAsync();
            }
            catch (Exception ex)
            {
                _ = ErrorHandling.Scene_Upload_Failed(user, channel);
                Console.WriteLine(ex);

                //Delete the loading message
                await loader.DeleteAsync();

                return;
            }
        }

        public static Point ProficiencyGraphPoint(int rank)
        {
            Point graph_point = new Point(0, 0);

            //Set chart point for Proficiency rank
            if (rank == 1)
            {
                graph_point = new Point(1307, 443);
            }
            else if (rank == 2)
            {
                graph_point = new Point(1313, 395);
            }
            else if (rank == 3)
            {
                graph_point = new Point(1320, 348);
            }
            else if (rank == 4)
            {
                graph_point = new Point(1324, 303);
            }
            else if (rank == 5)
            {
                graph_point = new Point(1331, 254);
            }

            return graph_point;
        }

        public static Point DiligenceGraphPoint(int rank)
        {
            Point graph_point = new Point(0, 0);

            //Set chart point for Diligence rank
            if (rank == 1)
            {
                graph_point = new Point(1213, 633);
            }
            else if (rank == 2)
            {
                graph_point = new Point(1175, 677);
            }
            else if (rank == 3)
            {
                graph_point = new Point(1136, 718);
            }
            else if (rank == 4)
            {
                graph_point = new Point(1098, 764);
            }
            else if (rank == 5)
            {
                graph_point = new Point(1058, 808);
            }

            return graph_point;
        }

        public static Point ExpressionGraphPoint(int rank)
        {
            Point graph_point = new Point(0, 0);

            //Set chart point for Expression rank
            if (rank == 1)
            {
                graph_point = new Point(1357, 644);
            }
            else if (rank == 2)
            {
                graph_point = new Point(1386, 691);
            }
            else if (rank == 3)
            {
                graph_point = new Point(1414, 736);
            }
            else if (rank == 4)
            {
                graph_point = new Point(1443, 785);
            }
            else if (rank == 5)
            {
                graph_point = new Point(1471, 833);
            }

            return graph_point;
        }

        public static Point MidsectionGraphPoint(int diligence_rank, int expression_rank)
        {
            Point graph_point = new Point(0, 0);

            //Set chart point for midsection point between Diligence and Expression
            if (diligence_rank == 5 || expression_rank == 5)
            {
                graph_point = new Point(1278, 668);
            }
            else if (diligence_rank >= 4 || expression_rank >= 4)
            {
                graph_point = new Point(1281, 646);
            }
            else if (diligence_rank >= 3 || expression_rank >= 3)
            {
                graph_point = new Point(1284, 627);
            }
            else if (diligence_rank >= 2 || expression_rank >= 2)
            {
                graph_point = new Point(1286, 608);
            }
            else if (diligence_rank == 1 || expression_rank == 1)
            {
                graph_point = new Point(1288, 589);
            }

            return graph_point;
        }

        public static Bitmap RenderUserInfoBox()
        {
            // Make a new bitmap large enough for a working space.
            Bitmap new_bitmap = new Bitmap(1920, 1080);

            // Begin editing the new_bitmap with a graphics object.
            using (Graphics graphics = Graphics.FromImage(new_bitmap))
            {
                // Set the graphics rendering to have antialiasing.
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                // Create a brush for the color white.
                SolidBrush blackBrush = new SolidBrush(System.Drawing.Color.Black);

                // Create a variable and brush for the color light pink.
                System.Drawing.Color beige = System.Drawing.Color.FromArgb(247, 225, 178);
                SolidBrush pinkBrush = new SolidBrush(beige);

                // Create a new random variable.
                Random rnd = new Random();

                // Create multiple variables for the potential min and max values of the four points of the black username box.
                int name_black_point_1_x_min = 105;
                int name_black_point_1_x_max = 154;
                int name_black_point_1_y_min = -14;
                int name_black_point_1_y_max = 20;

                int name_black_point_2_x_min = 701;
                int name_black_point_2_x_max = 752;
                int name_black_point_2_y_min = 12;
                int name_black_point_2_y_max = 76;

                int name_black_point_3_x_min = 721;
                int name_black_point_3_x_max = 750;
                int name_black_point_3_y_min = 173;
                int name_black_point_3_y_max = 191;

                int name_black_point_4_x_min = 141;
                int name_black_point_4_x_max = 183;
                int name_black_point_4_y_min = 170;
                int name_black_point_4_y_max = 217;

                // Randomly set the X and Y values of the four points of the black username box using the min and max values.
                int name_black_point_1_x = rnd.Next(name_black_point_1_x_min, name_black_point_1_x_max);
                int name_black_point_1_y = rnd.Next(name_black_point_1_y_min, name_black_point_1_y_max);

                int name_black_point_2_x = rnd.Next(name_black_point_2_x_min, name_black_point_2_x_max);
                int name_black_point_2_y = rnd.Next(name_black_point_2_y_min, name_black_point_2_y_max);

                int name_black_point_3_x = rnd.Next(name_black_point_3_x_min, name_black_point_3_x_max);
                int name_black_point_3_y = rnd.Next(name_black_point_3_y_min, name_black_point_3_y_max);

                int name_black_point_4_x = rnd.Next(name_black_point_4_x_min, name_black_point_4_x_max);
                int name_black_point_4_y = rnd.Next(name_black_point_4_y_min, name_black_point_4_y_max);

                // Randomly set the X and Y values of the four points of the white username box based on the set black username box X & Y values.
                int name_white_point_1_x = rnd.Next(name_black_point_1_x + 8, name_black_point_1_x + 20);
                int name_white_point_1_y = rnd.Next(name_black_point_1_y + 8, name_black_point_1_y + 20);

                int name_white_point_2_x = rnd.Next(name_black_point_2_x - 20, name_black_point_2_x - 8);
                int name_white_point_2_y = rnd.Next(name_black_point_2_y + 8, name_black_point_2_y + 20);

                int name_white_point_3_x = rnd.Next(name_black_point_3_x - 20, name_black_point_3_x - 8);
                int name_white_point_3_y = rnd.Next(name_black_point_3_y - 20, name_black_point_3_y - 8);

                int name_white_point_4_x = rnd.Next(name_black_point_4_x + 8, name_black_point_4_x + 20);
                int name_white_point_4_y = rnd.Next(name_black_point_4_y - 20, name_black_point_4_y - 8);

                // Create the four points of the black username box from the randomly chosen values.
                Point name_black_point_1 = new Point(name_black_point_1_x, name_black_point_1_y);
                Point name_black_point_2 = new Point(name_black_point_2_x, name_black_point_2_y);
                Point name_black_point_3 = new Point(name_black_point_3_x, name_black_point_3_y);
                Point name_black_point_4 = new Point(name_black_point_4_x, name_black_point_4_y);

                // Create the four points of the white username box from the randomly chosen values.
                Point name_white_point_1 = new Point(name_white_point_1_x, name_white_point_1_y);
                Point name_white_point_2 = new Point(name_white_point_2_x, name_white_point_2_y);
                Point name_white_point_3 = new Point(name_white_point_3_x, name_white_point_3_y);
                Point name_white_point_4 = new Point(name_white_point_4_x, name_white_point_4_y);

                // Add all the points for the black username box into a point array.
                Point[] name_black_poly_points = { name_black_point_1, name_black_point_2, name_black_point_3, name_black_point_4 };

                // Add all the points for the white username box into a point array.
                Point[] name_white_poly_points = { name_white_point_1, name_white_point_2, name_white_point_3, name_white_point_4 };

                // Use the name_black_poly_points array to create a polygon and fill it with black color.
                graphics.FillPolygon(blackBrush, name_black_poly_points);

                // Create points for the outer black box the Level and Next Level information goes into.
                Point lv_black_point_1 = new Point(251, 169);
                Point lv_black_point_2 = new Point(934, 154);
                Point lv_black_point_3 = new Point(944, 222);
                Point lv_black_point_4 = new Point(279, 332);

                // Create points for the inner white box the Level and Next Level information goes into.
                Point lv_white_point_1 = new Point(311, 175);
                Point lv_white_point_2 = new Point(915, 160);
                Point lv_white_point_3 = new Point(932, 218);
                Point lv_white_point_4 = new Point(312, 310);

                // Add all the points for the black Level box into a point array.
                Point[] lv_black_poly_points = { lv_black_point_1, lv_black_point_2, lv_black_point_3, lv_black_point_4 };

                // Add all the points for the white Level box into a point array.
                Point[] lv_white_poly_points = { lv_white_point_1, lv_white_point_2, lv_white_point_3, lv_white_point_4 };

                // Use the lv_black_poly_points array to create a polygon and fill it with black color.
                graphics.FillPolygon(blackBrush, lv_black_poly_points);

                // Use the name_white_poly_points array to create a polygon and fill it with light pink color.
                graphics.FillPolygon(pinkBrush, name_white_poly_points);

                // Use the lv_white_poly_points array to create a polygon and fill it with light pink color.
                graphics.FillPolygon(pinkBrush, lv_white_poly_points);
            }

            // Return the new bitmap.
            return new_bitmap;
        }

        public static Bitmap RenderUsername(string username)
        {
            //Create a new bitmap the same size as the username area of the template
            Bitmap username_bitmap = new Bitmap(514, 116);

            //Use a graphics object to edit the bitmap
            using (Graphics graphics = Graphics.FromImage(username_bitmap))
            {
                //Set text rendering to have antialiasing
                graphics.TextRenderingHint = TextRenderingHint.AntiAlias;

                //Create text boxes to place the user's username in
                Rectangle username_box = new Rectangle(0, 0, 514, 116);
                Rectangle username_shadow_box = new Rectangle(4, 4, 514, 116);

                //Create a new color to place a drop shadow behind the username
                System.Drawing.Color username_shadow_color = System.Drawing.Color.FromArgb(196, 164, 54);

                //Create a new brushe so that the colors can be used on a font object
                SolidBrush username_shadow_brush = new SolidBrush(username_shadow_color);

                //Using a font object, draw the user's username value to the template
                using (Font p5r_font = new Font("Optima nova LT Black", 28))
                {
                    //Format the string so that its placement is at the center of the text box
                    StringFormat stringFormat = new StringFormat();
                    stringFormat.Alignment = StringAlignment.Center;
                    stringFormat.LineAlignment = StringAlignment.Center;

                    //Draw the username text and its shadow to the username_bitmap
                    graphics.DrawString(username, p5r_font, username_shadow_brush, username_shadow_box, stringFormat);
                    graphics.DrawString(username, p5r_font, System.Drawing.Brushes.Black, username_box, stringFormat);
                }
            }

            return username_bitmap;
        }

        public static Bitmap RenderLevel(string level)
        {
            // Create a new bitmap the same size as the username area of the template
            Bitmap level_bitmap = new Bitmap(210, 100);

            // Use a graphics object to edit the bitmap
            using (Graphics graphics = Graphics.FromImage(level_bitmap))
            {
                // Set text rendering to have antialiasing
                graphics.TextRenderingHint = TextRenderingHint.AntiAlias;

                // Create a text box to place the user's level in
                Rectangle level_box = new Rectangle(0, 0, 210, 100);

                // Using a font object, draw the user's level value to the template
                using (Font p5r_stats_font = new Font("P5R Stats", 50))
                {
                    // Format the string so that its placement is on the right side of the text box
                    StringFormat stringFormat = new StringFormat();
                    stringFormat.Alignment = StringAlignment.Near;

                    // Draw the level text to the level_bitmap
                    graphics.DrawString(level, p5r_stats_font, System.Drawing.Brushes.Black, level_box, stringFormat);
                }
            }

            return level_bitmap;
        }

        public static Bitmap RenderNextExpBitmap(string next_exp)
        {
            // Create a new bitmap the same size as the username area of the template
            Bitmap next_exp_bitmap = new Bitmap(210, 92);

            // Use a graphics object to edit the bitmap
            using (Graphics graphics = Graphics.FromImage(next_exp_bitmap))
            {
                // Set text rendering to have antialiasing
                graphics.TextRenderingHint = TextRenderingHint.AntiAlias;

                // Create a text box to place the user's level in
                Rectangle next_exp_box = new Rectangle(0, 0, 210, 92);

                // Using a font object, draw the user's level value to the template
                using (Font p5r_stats_font = new Font("P5R Stats", 30))
                {
                    // Format the string so that its placement is on the right side of the text box
                    StringFormat stringFormat = new StringFormat();
                    stringFormat.Alignment = StringAlignment.Near;

                    // Draw the next EXP text to the next_exp_bitmap
                    graphics.DrawString(next_exp, p5r_stats_font, System.Drawing.Brushes.Black, next_exp_box, stringFormat);
                }
            }

            return next_exp_bitmap;
        }

        public static Bitmap RenderPMedalVector()
        {
            // Make a new bitmap large enough for a working space.
            Bitmap new_bitmap = new Bitmap(1920, 1080);

            // Begin editing the new_bitmap with a graphics object.
            using (Graphics graphics = Graphics.FromImage(new_bitmap))
            {
                // Set the graphics rendering to have antialiasing.
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                // Create a brush for the color white.
                SolidBrush blackBrush = new SolidBrush(System.Drawing.Color.Black);

                // Create a variable and brush for the color light pink.
                System.Drawing.Color beige = System.Drawing.Color.FromArgb(247, 225, 178);
                SolidBrush pinkBrush = new SolidBrush(beige);

                // Create a new random variable.
                Random rnd = new Random();

                // Create multiple variables for the potential min and max values of the four points of the black P-Medal vector box.
                int black_pmedal_vector_point_1_x_min = 673;
                int black_pmedal_vector_point_1_x_max = 673 + 6;
                int black_pmedal_vector_point_1_y_min = 23;
                int black_pmedal_vector_point_1_y_max = 23 + 6;

                int black_pmedal_vector_point_2_x_min = 948;
                int black_pmedal_vector_point_2_x_max = 948 + 6;
                int black_pmedal_vector_point_2_y_min = 16;
                int black_pmedal_vector_point_2_y_max = 16 + 6;

                int black_pmedal_vector_point_3_x_min = 937;
                int black_pmedal_vector_point_3_x_max = 937 + 6;
                int black_pmedal_vector_point_3_y_min = 108;
                int black_pmedal_vector_point_3_y_max = 108 + 6;

                int black_pmedal_vector_point_4_x_min = 691;
                int black_pmedal_vector_point_4_x_max = 691 + 6;
                int black_pmedal_vector_point_4_y_min = 111;
                int black_pmedal_vector_point_4_y_max = 111 + 6;

                // Randomly set the X and Y values of the four points of the black P-Medal vector box using the min and max values.
                int black_pmedal_vector_point_1_x = rnd.Next(black_pmedal_vector_point_1_x_min, black_pmedal_vector_point_1_x_max);
                int black_pmedal_vector_point_1_y = rnd.Next(black_pmedal_vector_point_1_y_min, black_pmedal_vector_point_1_y_max);

                int black_pmedal_vector_point_2_x = rnd.Next(black_pmedal_vector_point_2_x_min, black_pmedal_vector_point_2_x_max);
                int black_pmedal_vector_point_2_y = rnd.Next(black_pmedal_vector_point_2_y_min, black_pmedal_vector_point_2_y_max);

                int black_pmedal_vector_point_3_x = rnd.Next(black_pmedal_vector_point_3_x_min, black_pmedal_vector_point_3_x_max);
                int black_pmedals_vector_point_3_y = rnd.Next(black_pmedal_vector_point_3_y_min, black_pmedal_vector_point_3_y_max);

                int black_pmedal_vector_point_4_x = rnd.Next(black_pmedal_vector_point_4_x_min, black_pmedal_vector_point_4_x_max);
                int black_pmedal_vector_point_4_y = rnd.Next(black_pmedal_vector_point_4_y_min, black_pmedal_vector_point_4_y_max);

                // Create the four points of the black P-Medal vector box from the randomly chosen values.
                Point black_pmedal_vector_point_1 = new Point(black_pmedal_vector_point_1_x, black_pmedal_vector_point_1_y);
                Point black_pmedal_vector_point_2 = new Point(black_pmedal_vector_point_2_x, black_pmedal_vector_point_2_y);
                Point black_pmedal_vector_point_3 = new Point(black_pmedal_vector_point_3_x, black_pmedals_vector_point_3_y);
                Point black_pmedal_vector_point_4 = new Point(black_pmedal_vector_point_4_x, black_pmedal_vector_point_4_y);

                // Add all the points for the black P-Medal vector box into a point array.
                Point[] black_pmedal_vector_poly_points = { black_pmedal_vector_point_1, black_pmedal_vector_point_2, black_pmedal_vector_point_3, black_pmedal_vector_point_4 };

                // Next, work on constructing the inner white area of the P-Medal vector box.
                // Create constant variables for the random shifts on the X and Y values.
                int min_addon = 6;
                int max_addon = 10;

                // Randomly set the X and Y values of the four points of the white P-Medal vector box based on the set black P-Medal vector box X & Y values.
                int white_pmedal_vector_point_1_x = rnd.Next(black_pmedal_vector_point_1_x + min_addon, black_pmedal_vector_point_1_x + max_addon);
                int white_pmedal_vector_point_1_y = rnd.Next(black_pmedal_vector_point_1_y + min_addon, black_pmedal_vector_point_1_y + max_addon);

                int white_pmedal_vector_point_2_x = rnd.Next(black_pmedal_vector_point_2_x - max_addon, black_pmedal_vector_point_2_x - min_addon);
                int white_pmedal_vector_point_2_y = rnd.Next(black_pmedal_vector_point_2_y + min_addon, black_pmedal_vector_point_2_y + max_addon);

                int white_pmedal_vector_point_3_x = rnd.Next(black_pmedal_vector_point_3_x - max_addon, black_pmedal_vector_point_3_x - min_addon);
                int white_pmedal_vector_point_3_y = rnd.Next(black_pmedals_vector_point_3_y - max_addon, black_pmedals_vector_point_3_y - min_addon);

                int white_pmedal_vector_point_4_x = rnd.Next(black_pmedal_vector_point_4_x + min_addon, black_pmedal_vector_point_4_x + max_addon);
                int white_pmedal_vector_point_4_y = rnd.Next(black_pmedal_vector_point_4_y - max_addon, black_pmedal_vector_point_4_y - min_addon);

                // Create the four points of the white P-Medal vector box from the randomly chosen values.
                Point white_pmedal_vector_point_1 = new Point(white_pmedal_vector_point_1_x, white_pmedal_vector_point_1_y);
                Point white_pmedal_vector_point_2 = new Point(white_pmedal_vector_point_2_x, white_pmedal_vector_point_2_y);
                Point white_pmedal_vector_point_3 = new Point(white_pmedal_vector_point_3_x, white_pmedal_vector_point_3_y);
                Point white_pmedal_vector_point_4 = new Point(white_pmedal_vector_point_4_x, white_pmedal_vector_point_4_y);

                // Add all the points for the white P-Medal vector box into a point array.
                Point[] white_pmedal_vector_poly_points = { white_pmedal_vector_point_1, white_pmedal_vector_point_2, white_pmedal_vector_point_3, white_pmedal_vector_point_4 };

                //Finally, let's draw the created polygons to the bitmap.
                // Use the black_pmedal_vector_poly_points array to create a polygon and fill it with black color.
                graphics.FillPolygon(blackBrush, black_pmedal_vector_poly_points);

                // Use the white_pmedal_vector_poly_points array to create a polygon and fill it with light pink color.
                graphics.FillPolygon(pinkBrush, white_pmedal_vector_poly_points);
            }

            // Return the new bitmap.
            return new_bitmap;
        }

        public static Bitmap RenderVectorizedProfilePicture(SocketGuildUser user)
        {
            // Make a new bitmap large enough for a working space.
            Bitmap new_bitmap = new Bitmap(600, 600);

            //Create a variable for the user's profile picture
            string profile_picture_url = user.GetAvatarUrl();

            //If the user doesn't have a profile picture, use a default one
            if (profile_picture_url == null)
            {
                profile_picture_url = "https://i.imgur.com/T0AjCLh.png";
            }

            //Use a graphics object to edit the new bitmap
            using (Graphics graphics = Graphics.FromImage(new_bitmap))
            {
                // Set the graphics rendering to have antialiasing.
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                // Create a variable and brush for the color light pink.
                System.Drawing.Color transparent_black = System.Drawing.Color.FromArgb(128, 0, 0, 0);
                SolidBrush pinkBrush = new SolidBrush(transparent_black);

                // Create a new random variable.
                Random rnd = new Random();

                // Create multiple variables for the potential min and max values of the four points of the white stats vector box.
                int pink_vector_point_1_x_min = 50;
                int pink_vector_point_1_x_max = 100 - 15;
                int pink_vector_point_1_y_min = 50;
                int pink_vector_point_1_y_max = 100 - 15;

                int pink_vector_point_2_x_min = 500 + 15;
                int pink_vector_point_2_x_max = 550;
                int pink_vector_point_2_y_min = 50;
                int pink_vector_point_2_y_max = 100 - 15;

                int pink_vector_point_3_x_min = 500 + 15;
                int pink_vector_point_3_x_max = 550;
                int pink_vector_point_3_y_min = 500 + 15;
                int pink_vector_point_3_y_max = 550;

                int pink_vector_point_4_x_min = 50;
                int pink_vector_point_4_x_max = 100 - 15;
                int pink_vector_point_4_y_min = 500 + 15;
                int pink_vector_point_4_y_max = 550;

                // Randomly set the X and Y values of the four points of the pink vector box using the min and max values.
                int pink_vector_point_1_x = rnd.Next(pink_vector_point_1_x_min, pink_vector_point_1_x_max);
                int pink_vector_point_1_y = rnd.Next(pink_vector_point_1_y_min, pink_vector_point_1_y_max);

                int pink_vector_point_2_x = rnd.Next(pink_vector_point_2_x_min, pink_vector_point_2_x_max);
                int pink_vector_point_2_y = rnd.Next(pink_vector_point_2_y_min, pink_vector_point_2_y_max);

                int pink_vector_point_3_x = rnd.Next(pink_vector_point_3_x_min, pink_vector_point_3_x_max);
                int pink_vector_point_3_y = rnd.Next(pink_vector_point_3_y_min, pink_vector_point_3_y_max);

                int pink_vector_point_4_x = rnd.Next(pink_vector_point_4_x_min, pink_vector_point_4_x_max);
                int pink_vector_point_4_y = rnd.Next(pink_vector_point_4_y_min, pink_vector_point_4_y_max);

                // Create the four points of the pink vector box from the randomly chosen values.
                Point pink_vector_point_1 = new Point(pink_vector_point_1_x, pink_vector_point_1_y);
                Point pink_vector_point_2 = new Point(pink_vector_point_2_x, pink_vector_point_2_y);
                Point pink_vector_point_3 = new Point(pink_vector_point_3_x, pink_vector_point_3_y);
                Point pink_vector_point_4 = new Point(pink_vector_point_4_x, pink_vector_point_4_y);

                // Add all the points for the white stats vector box into a point array.
                Point[] pink_vector_poly_points = { pink_vector_point_1, pink_vector_point_2, pink_vector_point_3, pink_vector_point_4 };

                // Use the pink_vector_poly_points array to create a polygon and fill it with light pink color.
                graphics.FillPolygon(pinkBrush, pink_vector_poly_points);

                //Use a web client to download the user's profile picture and draw it to the template
                using (var wc = new WebClient())
                {
                    using (var imgStream = new MemoryStream(wc.DownloadData(profile_picture_url)))
                    {
                        using (var objImage = System.Drawing.Image.FromStream(imgStream))
                        {
                            Bitmap profile_picture = (Bitmap)objImage;

                            graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                            graphics.DrawImage(profile_picture, 100, 100, 400, 400);
                        }
                    }
                }
            }

            return new_bitmap;
        }

        public static Bitmap RenderProfilePictureLayer(SocketGuildUser user)
        {
            // Make a new bitmap large enough for a working space.
            Bitmap new_bitmap = new Bitmap(1920, 1080);

            //Use a graphics object to edit the new bitmap
            using (Graphics graphics = Graphics.FromImage(new_bitmap))
            {
                // Set the graphics rendering to have antialiasing.
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                // Create a bitmap for the profile picture and its vectorized background
                Bitmap profile_picture = RenderVectorizedProfilePicture((SocketGuildUser)user);

                // Rotate the profile picture by -12 degrees
                Bitmap rotated_profile_picture = RotateImage(profile_picture, -12);

                // Draw the rotated profile picture to the new bitmap
                graphics.DrawImage(rotated_profile_picture, -130, 210, rotated_profile_picture.Width, rotated_profile_picture.Height);
            }

            return new_bitmap;
        }

        public static string TotalExpToString(int total_exp)
        {
            // Create a string variable that will be the represented output of the user's total EXP
            string total_exp_string = "";

            // Determine if the user's total EXP value is less than seven digits
            if (total_exp.ToString().Length < 7)
            {
                // If so, create a variable that contains the amount of digits left to make the value have seven
                int empty_spaces = 7 - total_exp.ToString().Length;

                // For every needed digit in empty_spaces, add a leading zero to total_exp_string
                for (int i = 0; i < empty_spaces; i++)
                {
                    total_exp_string += "0";
                }
            }

            // Concatenate the user's total EXP value to the end of total_exp_string as a string
            total_exp_string += total_exp.ToString();

            // Return the final result
            return total_exp_string;
        }

        public static Bitmap RenderTotalExpBitmap(string total_exp_string, int total_exp_digit_count)
        {
            // Create a new bitmap the same size as the username area of the template. (For this decor, add 50 pixels to the height so the RotateImage method can rotate this bitmap properly.)
            Bitmap total_exp_bitmap = new Bitmap(378, 108 + 20);

            // Create a char array from the input total_exp_string. The array length should always be seven.
            char[] total_exp_char_array = total_exp_string.ToCharArray();

            // Create an int to determine at what point in the array the actual total EXP value starts
            int actual_value_start_index = 7 - total_exp_digit_count;

            // Use a graphics object to edit the bitmap
            using (Graphics graphics = Graphics.FromImage(total_exp_bitmap))
            {
                // Create a variable to keep track of the X position of an (X, Y) coordinate pair
                int x = 0;

                // Iterate through the char array
                for (int i = 0; i < 7; i++)
                {
                    // Copy the needed digit image to a bitmap
                    Bitmap current_digit = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//Profile//StatusScreens//Decor//Decor_Kasumi_1//TotalExpDigits//{total_exp_char_array[i]}.png");

                    // If the iterator 'i' is less than the start index of the actual value, color the digit dark gold
                    if (i < actual_value_start_index)
                    {
                        current_digit = BitmapToColor(current_digit, 77, 64, 42);
                    }
                    // Else, if the iterator 'i' is at or more than the start index of the actual value, color the digit light gold
                    else if (i >= actual_value_start_index)
                    {
                        current_digit = BitmapToColor(current_digit, 242, 210, 147);
                    }

                    // Draw the current digit to the total_exp_bitmap
                    graphics.DrawImage(current_digit, x, 0, current_digit.Width, current_digit.Height);

                    // Increment the X coordinate by the current digit's width so that the next digit is placed correctly
                    x += current_digit.Width;

                    if (i == 0 || i == 3)
                    {
                        // Copy the dot image to a bitmap
                        current_digit = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//Profile//StatusScreens//Decor//Decor_Kasumi_1//TotalExpDigits//dot.png");

                        // If the iterator 'i' is less than the start index of the actual value, color the dot dark gold
                        if (i < actual_value_start_index)
                        {
                            current_digit = BitmapToColor(current_digit, 77, 64, 42);
                        }
                        //Else, if the iterator 'i' is at or more than the start index of the actual value, color the dot light gold
                        else if (i >= actual_value_start_index)
                        {
                            current_digit = BitmapToColor(current_digit, 242, 210, 147);
                        }

                        // Draw the dot to the total_exp_bitmap
                        graphics.DrawImage(current_digit, x, 0, current_digit.Width, current_digit.Height);

                        // Increment the X coordinate by the current digit's width so that the next digit is placed correctly
                        x += current_digit.Width;
                    }
                }
            }

            return total_exp_bitmap;
        }

        public static Bitmap RenderRadarChart(int proficiency_rank, int diligence_rank, int expression_rank)
        {
            //Make a new bitmap the same size as input_bitmap
            Bitmap new_bitmap = new Bitmap(1920, 1080);

            //Use a graphics object to edit the bitmap
            using (Graphics graphics = Graphics.FromImage(new_bitmap))
            {
                // Set the graphics rendering to have antialiasing.
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                //Create points that define radar chart
                Point proficiency_point = ProficiencyGraphPoint(proficiency_rank);
                Point diligence_point = DiligenceGraphPoint(diligence_rank);
                Point expression_point = ExpressionGraphPoint(expression_rank);
                Point midsection_point = MidsectionGraphPoint(diligence_rank, expression_rank);

                //Create a color for the radar chart
                SolidBrush yellowBrush = new SolidBrush(System.Drawing.Color.Orange);

                //Bind radar chart points
                Point[] curvePoints = { proficiency_point, diligence_point, midsection_point, expression_point };

                //Draw radar chart to screen
                graphics.FillPolygon(yellowBrush, curvePoints);
            }

            //Return the new bitmap
            return new_bitmap;
        }

        public static Bitmap RenderStatIcons(int proficiency_rank, int diligence_rank, int expression_rank)
        {
            //Make a new bitmap to place the icons on
            Bitmap new_bitmap = new Bitmap(1920, 1080);

            //Copy the icon backgrounds to a bitmap
            Bitmap proficiency_icon_bg = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//Profile//StatusScreens//Default//P5//ChartStats//proficiency_bg.png");
            Bitmap diligence_icon_bg = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//Profile//StatusScreens//Default//P5//ChartStats//diligence_bg.png");
            Bitmap expression_icon_bg = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//Profile//StatusScreens//Default//P5//ChartStats//expression_bg.png");

            //Copy the icon text layers to a bitmap
            Bitmap proficiency_icon_text = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//Profile//StatusScreens//Default//P5//ChartStats//proficiency_text.png");
            Bitmap diligence_icon_text = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//Profile//StatusScreens//Default//P5//ChartStats//diligence_text.png");
            Bitmap expression_icon_text = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//Profile//StatusScreens//Default//P5//ChartStats//expression_text.png");

            //Copy the rank number backgrounds to a bitmap
            Bitmap proficiency_rank_number_bg = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//Profile//StatusScreens//Default//P5//ChartStats//{proficiency_rank}_bg.png");
            Bitmap diligence_rank_number_bg = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//Profile//StatusScreens//Default//P5//ChartStats//{diligence_rank}_bg.png");
            Bitmap expression_rank_number_bg = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//Profile//StatusScreens//Default//P5//ChartStats//{expression_rank}_bg.png");

            //Copy the rank number text to a bitmap
            Bitmap proficiency_rank_number_text = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//Profile//StatusScreens//Default//P5//ChartStats//{proficiency_rank}_text.png");
            Bitmap diligence_rank_number_text = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//Profile//StatusScreens//Default//P5//ChartStats//{diligence_rank}_text.png");
            Bitmap expression_rank_number_text = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//Profile//StatusScreens//Default//P5//ChartStats//{expression_rank}_text.png");

            //Use a graphics object to edit the bitmap
            using (Graphics graphics = Graphics.FromImage(new_bitmap))
            {
                //Determine whether to color the background layers dark yellow or bright yellow based on the user's Proficiency rank
                if (proficiency_rank < 5)
                {
                    proficiency_icon_bg = BitmapToColor(proficiency_icon_bg, 255, 180, 0);
                    proficiency_rank_number_bg = BitmapToColor(proficiency_rank_number_bg, 255, 180, 0);
                }
                else
                {
                    proficiency_icon_bg = BitmapToColor(proficiency_icon_bg, 255, 255, 32);
                    proficiency_rank_number_bg = BitmapToColor(proficiency_rank_number_bg, 255, 255, 32);
                }

                //Determine whether to color the background layers dark yellow or bright yellow based on the user's Diligence rank
                if (diligence_rank < 5)
                {
                    diligence_icon_bg = BitmapToColor(diligence_icon_bg, 255, 180, 0);
                    diligence_rank_number_bg = BitmapToColor(diligence_rank_number_bg, 255, 180, 0);
                }
                else
                {
                    diligence_icon_bg = BitmapToColor(diligence_icon_bg, 255, 255, 32);
                    diligence_rank_number_bg = BitmapToColor(diligence_rank_number_bg, 255, 255, 32);
                }

                //Determine whether to color the background layers dark yellow or bright yellow based on the user's Expression rank
                if (expression_rank < 5)
                {
                    expression_icon_bg = BitmapToColor(expression_icon_bg, 255, 180, 0);
                    expression_rank_number_bg = BitmapToColor(expression_rank_number_bg, 255, 180, 0);
                }
                else
                {
                    expression_icon_bg = BitmapToColor(expression_icon_bg, 255, 255, 32);
                    expression_rank_number_bg = BitmapToColor(expression_rank_number_bg, 255, 255, 32);
                }

                //Set a default value for the height of rank number bitmaps on the template
                int proficiency_rank_number_height = 153;
                int diligence_rank_number_height = 870;
                int expression_rank_number_height = 889;

                //If any of the social ranks are at the max value, move the rank number bitmap up by 30 pixels
                if (proficiency_rank == 5)
                {
                    proficiency_rank_number_height -= 30;
                }
                if (diligence_rank == 5)
                {
                    diligence_rank_number_height -= 30;
                }
                if (expression_rank == 5)
                {
                    expression_rank_number_height -= 30;
                }

                //Render the background layers for the Proficiency icon and rank number
                graphics.DrawImage(proficiency_icon_bg, 1245, 98, 140, 102);
                graphics.DrawImage(proficiency_rank_number_bg, 1375, proficiency_rank_number_height, proficiency_rank_number_bg.Width, proficiency_rank_number_bg.Height);

                //Render the text layers for the Proficiency icon and rank number
                graphics.DrawImage(proficiency_icon_text, 1245, 98, 140, 102);
                graphics.DrawImage(proficiency_rank_number_text, 1375, proficiency_rank_number_height, proficiency_rank_number_text.Width, proficiency_rank_number_text.Height);

                //Render the background layers for the Diligence icon and rank number
                graphics.DrawImage(diligence_icon_bg, 951, 824, 147, 95);
                graphics.DrawImage(diligence_rank_number_bg, 1087, diligence_rank_number_height, diligence_rank_number_bg.Width, diligence_rank_number_bg.Height);

                //Render the text layers for the Diligence icon and rank number
                graphics.DrawImage(diligence_icon_text, 951, 824, 147, 95);
                graphics.DrawImage(diligence_rank_number_text, 1087, diligence_rank_number_height, diligence_rank_number_text.Width, diligence_rank_number_text.Height);

                //Render the background layers for the Expression icon and rank number
                graphics.DrawImage(expression_icon_bg, 1417, 847, 171, 103);
                graphics.DrawImage(expression_rank_number_bg, 1577, expression_rank_number_height, expression_rank_number_text.Width, expression_rank_number_text.Height);

                //Render the text layers for the Expression icon and rank number
                graphics.DrawImage(expression_icon_text, 1417, 847, 171, 103);
                graphics.DrawImage(expression_rank_number_text, 1577, expression_rank_number_height, expression_rank_number_text.Width, expression_rank_number_text.Height);
            }

            //Return the new bitmap
            return new_bitmap;
        }

        public static Bitmap RenderAllRankTitles(int proficiency_rank, int diligence_rank, int expression_rank)
        {
            // Create a new bitmap to place the title on the template
            Bitmap title_bitmap = new Bitmap(1920, 1080);

            // Create bitmaps for all three rank titles
            Bitmap proficiency_title = RenderRankTitleBitmap(Core.LevelSystem.SocialStats.ProficiencyRankTitle(proficiency_rank));
            Bitmap diligence_title = RenderRankTitleBitmap(Core.LevelSystem.SocialStats.DiligenceRankTitle(diligence_rank));
            Bitmap expression_title = RenderRankTitleBitmap(Core.LevelSystem.SocialStats.ExpressionRankTitle(expression_rank));

            // Rotate the title bitmaps by -7 degrees
            proficiency_title = RotateImage(proficiency_title, -7);
            diligence_title = RotateImage(diligence_title, -7);
            expression_title = RotateImage(expression_title, -7);

            // Use a graphics object to edit the bitmap
            using (Graphics graphics = Graphics.FromImage(title_bitmap))
            {
                // Draw the title bitmaps onto the template
                graphics.DrawImage(proficiency_title, 1067, 186, proficiency_title.Width, proficiency_title.Height);
                graphics.DrawImage(diligence_title, 776, 917, diligence_title.Width, diligence_title.Height);
                graphics.DrawImage(expression_title, 1250, 935, expression_title.Width, expression_title.Height);
            }

            // Return the full bitmap
            return title_bitmap;
        }

        public static Bitmap RenderRankTitleBitmap(string title)
        {
            // Create a new bitmap to place the title on the template
            Bitmap title_bitmap = new Bitmap(350, 50);

            // Use a graphics object to edit the bitmap
            using (Graphics graphics = Graphics.FromImage(title_bitmap))
            {
                // Set text rendering to have antialiasing
                graphics.TextRenderingHint = TextRenderingHint.AntiAlias;

                // Create a text box to place the rank title in
                Rectangle title_box = new Rectangle(0, 0, 350, 50);

                // Create a font object to draw the string to the template
                using (Font p5r_font = new Font("Optima nova LT Black", 22))
                {
                    //F ormat the string so that its placement is at the center of the text box
                    StringFormat stringFormat = new StringFormat();
                    stringFormat.Alignment = StringAlignment.Center;
                    stringFormat.LineAlignment = StringAlignment.Center;

                    // Draw the title text to the bitmap
                    graphics.DrawString(title, p5r_font, System.Drawing.Brushes.White, title_box, stringFormat);
                }
            }

            // Return the bitmap
            return title_bitmap;
        }

        public static Bitmap RenderPrestigeCounter(int level_resets)
        {
            // Copy the prestige counter overlay to a bitmap.
            Bitmap prestige_overlay = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//Profile//StatusScreens//Decor//Decor_Akechi_1//prestige_counter.png");

            // Copy the star to mark prestige to a bitmap.
            Bitmap prestige_star = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//Profile//StatusScreens//Decor//Decor_Akechi_1//star.png");

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
                    graphics.DrawImage(prestige_star, 1506 + (i * 50), 9, 50, 50);
                }
            }

            return new_bitmap;
        }

        public static Bitmap KeepPixelOverlap(Bitmap radar_chart, Bitmap shading_overlay)
        {
            //Create variables to store pixel colors from both bitmaps in
            System.Drawing.Color radar_chart_color;
            System.Drawing.Color shading_overlay_color;

            //Make an empty bitmap the same size as the template
            Bitmap newBitmap = new Bitmap(1920, 1080);

            //Create a nested loop that iterates only on the neccesary areas of both bitmaps where they would overlap
            for (int i = 1056; i < 1472; i++)
            {
                for (int j = 252; j < 834; j++)
                {
                    //Get the pixel colors from both bitmaps
                    radar_chart_color = radar_chart.GetPixel(i, j);
                    shading_overlay_color = shading_overlay.GetPixel(i, j);

                    //If the opacity of the pixel on both bitmaps is greater than 0, copy the shading overlay's pixel to the new bitmap
                    if (radar_chart_color.A > 0 && shading_overlay_color.A > 0)
                    {
                        newBitmap.SetPixel(i, j, shading_overlay_color);
                    }
                }
            }

            return newBitmap;
        }

        public static Bitmap RotateImage(Bitmap rotateMe, float angle)
        {
            //First, re-center the image in a larger image that has a margin/frame
            //to compensate for the rotated image's increased size

            var bmp = new Bitmap(rotateMe.Width + (rotateMe.Width / 2), rotateMe.Height + (rotateMe.Height / 2));

            using (Graphics g = Graphics.FromImage(bmp))
                g.DrawImage(rotateMe, (rotateMe.Width / 4), (rotateMe.Height / 4), rotateMe.Width, rotateMe.Height);

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
                IconUrl = "https://i.imgur.com/1jk1MZw.png"
            };

            embed.WithAuthor(author);
            embed.WithColor(213, 27, 4);
            embed.WithThumbnailUrl("https://i.imgur.com/PYMB6XG.gif");
            embed.WithDescription("This may take a few seconds!");

            return embed;
        }
    }
}
