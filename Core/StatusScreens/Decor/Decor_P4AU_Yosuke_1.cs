using System;
using System.Drawing;
using System.IO;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using SocialLinker.Config;
using SocialLinker.Core.CloudStorageTables;
using System.Drawing.Text;
using SocialLinker.Core.Menus;

namespace SocialLinker.Core.StatusScreens.Decor
{
    public static class Decor_P4AU_Yosuke_1
    {
        public const int template_width = 1280;
        public const int template_height = 720;
        public const string decor_id = "Decor_P4AU_Yosuke_1";

        public static async void RenderImage(SocketUser user, ISocketMessageChannel channel)
        {
            // Send a loading message while the status screen gets made
            RestUserMessage loader = await channel.SendMessageAsync("", false, LoadingMessage().Build());

            // Place the bulk of the function in a try-catch block in case something fails and an error message needs to be sent
            try
            {
                var account = UserInfoClasses.GetAccount(user);

                Bitmap base_template = new Bitmap(template_width, template_height);

                Bitmap layer_1 = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//Profile//StatusScreens//Decor//{decor_id}//layer_1.png");
                Bitmap layer_2 = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//Profile//StatusScreens//Decor//{decor_id}//layer_2.png");

                Bitmap rotated_graph = RotateImage(RenderRadarChart(account), -4);
                Bitmap rotated_graph_drop_shadow = Bitmap_To_Color(rotated_graph, System.Drawing.Color.FromArgb(43, 43, 43), new Rectangle(0, 0, rotated_graph.Width, rotated_graph.Height));

                Bitmap text_layer = RotateImage(RenderNextExpAndPMedalStats(account), -4);

                Bitmap username_layer = RenderUsername(user.Username);

                Bitmap level_layer = RenderLevel(account);

                using (Graphics graphics = Graphics.FromImage(base_template))
                {
                    graphics.DrawImage(layer_1, 0, 0, layer_1.Width, layer_1.Height);

                    graphics.DrawImage(rotated_graph_drop_shadow, 727 + 24, -65 + 12, rotated_graph.Width, rotated_graph.Height);
                    graphics.DrawImage(rotated_graph, 727, -65, rotated_graph.Width, rotated_graph.Height);

                    graphics.DrawImage(layer_2, 0, 0, layer_2.Width, layer_2.Height);

                    graphics.DrawImage(text_layer, -322, -168, text_layer.Width, text_layer.Height);

                    graphics.DrawImage(username_layer, 0, 0, username_layer.Width, username_layer.Height);

                    graphics.DrawImage(level_layer, 0, 0, level_layer.Width, level_layer.Height);

                    if (account.Level_Resets > 0)
                    {
                        graphics.DrawImage(RenderPrestigeCounter(account.Level_Resets), 0, 0, template_width, template_height);
                    }
                }

                // Save the bitmap to a data stream
                MemoryStream memoryStream = new MemoryStream();
                base_template.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
                memoryStream.Seek(0, SeekOrigin.Begin);

                // Send the image
                await channel.SendFileAsync(memoryStream, $"status_{user.Id}_{DateTime.UtcNow}.png");

                // Delete the loading message
                await loader.DeleteAsync();
            }
            catch (Exception ex)
            {
                _ = ErrorHandling.Image_Upload_Failed(user, channel);
                Console.WriteLine(ex);

                //Delete the loading message
                await loader.DeleteAsync();

                return;
            }
        }

        public static Bitmap RenderRadarChart(UserInfoFields account)
        {
            Bitmap graph_main = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//Profile//StatusScreens//Decor//{decor_id}//Graph//graph_1.png");
            Bitmap white_overlay = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//Profile//StatusScreens//Decor//{decor_id}//Graph//graph_2.png");
            Bitmap plot_markers = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//Profile//StatusScreens//Decor//{decor_id}//Graph//graph_3.png");
            Bitmap base_template = new Bitmap(graph_main.Width, graph_main.Height);

            string proficiency_title = LevelSystem.SocialStats.ProficiencyRankTitle(account.Proficiency_Rank);
            string diligence_title = LevelSystem.SocialStats.DiligenceRankTitle(account.Diligence_Rank);
            string expression_title = LevelSystem.SocialStats.ExpressionRankTitle(account.Expression_Rank);

            Rectangle proficiency_title_box = new Rectangle(141, 75, 134, 26);
            Rectangle diligence_title_box = new Rectangle(0, 344, 134, 26);
            Rectangle expression_title_box = new Rectangle(283, 344, 134, 26);

            System.Drawing.Color dark_blue = System.Drawing.Color.FromArgb(0, 53, 236);
            SolidBrush dark_blue_brush = new SolidBrush(dark_blue);

            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                // Set text rendering to have antialiasing
                graphics.TextRenderingHint = TextRenderingHint.AntiAlias;

                graphics.DrawImage(graph_main, 0, 0, graph_main.Width, graph_main.Height);

                // Draw the rank titles to the bitmap.
                using (Font p4g_font = new Font("P4G", 13, FontStyle.Bold))
                {
                    StringFormat stringFormat = new StringFormat();
                    stringFormat.Alignment = StringAlignment.Center;

                    graphics.DrawString(proficiency_title, p4g_font, dark_blue_brush, proficiency_title_box, stringFormat);
                    graphics.DrawString(diligence_title, p4g_font, dark_blue_brush, diligence_title_box, stringFormat);
                    graphics.DrawString(expression_title, p4g_font, dark_blue_brush, expression_title_box, stringFormat);
                }

                graphics.DrawImage(white_overlay, 0, 0, white_overlay.Width, white_overlay.Height);
            }

            // Render user stats
            Point proficiency_point = ProficiencyGraphPoint(account.Proficiency_Rank);
            Point diligence_point = DiligenceGraphPoint(account.Diligence_Rank);
            Point expression_point = ExpressionGraphPoint(account.Expression_Rank);

            System.Drawing.Color graph_yellow = System.Drawing.Color.FromArgb(255, 243, 0);
            SolidBrush graph_yellow_brush = new SolidBrush(graph_yellow);

            Point[] curvePoints = { proficiency_point, diligence_point, expression_point };

            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                graphics.FillPolygon(graph_yellow_brush, curvePoints);
                graphics.DrawImage(plot_markers, 0, 0, plot_markers.Width, plot_markers.Height);
            }

            return base_template;
        }

        public static Bitmap RenderUsername(string username)
        {
            Bitmap base_template = new Bitmap(template_width, template_height);

            Rectangle username_box = new Rectangle(1084, 10, 197, 45);

            SolidBrush white_brush = new SolidBrush(System.Drawing.Color.White);

            if (username.Length > 14)
            {
                username = $"{username.Substring(0, 11)}...";
            }

            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                graphics.TextRenderingHint = TextRenderingHint.AntiAlias;

                using (Font username_font = new Font("FOT-スキップ Std B", 15))
                {
                    StringFormat stringFormat = new StringFormat();
                    stringFormat.Alignment = StringAlignment.Center;

                    graphics.DrawString(username, username_font, white_brush, username_box, stringFormat);
                }
            }

            return base_template;
        }

        public static Bitmap RenderNextExpAndPMedalStats(UserInfoFields account)
        {
            Bitmap base_template = new Bitmap(template_width, template_height);

            string pmedals = $"{account.P_Medals}";

            int next_exp = 0;
            if (account.Level != 99)
            {
                next_exp = Core.LevelSystem.Leveling.CalculateExp(account.Level + 1) - account.Total_Exp;
            }

            System.Drawing.Color stat_yellow = System.Drawing.Color.FromArgb(255, 255, 36);
            SolidBrush stat_yellow_brush = new SolidBrush(stat_yellow);

            Rectangle pmedal_box = new Rectangle(661, 400, 175, 45);
            Rectangle next_exp_box = new Rectangle(661 - 63, 500 - 28, 175, 45);

            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                graphics.TextRenderingHint = TextRenderingHint.AntiAlias;

                using (Font p4g_stats_font = new Font("P4G Stats", 34))
                {
                    StringFormat stringFormat = new StringFormat();
                    stringFormat.Alignment = StringAlignment.Near;

                    graphics.DrawString(pmedals, p4g_stats_font, stat_yellow_brush, pmedal_box, stringFormat);
                    graphics.DrawString(next_exp.ToString(), p4g_stats_font, stat_yellow_brush, next_exp_box, stringFormat);
                }
            }

            return base_template;
        }

        public static Bitmap RenderLevel(UserInfoFields account)
        {
            Bitmap base_template = new Bitmap(template_width, template_height);
            string level = $"{account.Level}";

            SolidBrush white_brush = new SolidBrush(System.Drawing.Color.White);

            Rectangle level_box = new Rectangle(785, 509, 320, 211);

            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                //Set text rendering to have antialiasing
                graphics.TextRenderingHint = TextRenderingHint.AntiAlias;

                // Draw the rank titles to the bitmap.
                using (Font p4g_stats_font = new Font("Edo SZ", 150))
                {
                    StringFormat stringFormat = new StringFormat();
                    stringFormat.Alignment = StringAlignment.Center;

                    graphics.DrawString(level, p4g_stats_font, white_brush, level_box, stringFormat);
                }
            }

            Random rnd = new Random();

            int random_chance = rnd.Next(1, 101);
            int metal_select = 0;

            if (random_chance <= 60)
            {
                metal_select = 1;
            }
            else if (random_chance <= 90)
            {
                metal_select = 2;
            }
            else if (random_chance <= 100)
            {
                metal_select = 3;
            }

            int x_placement = rnd.Next(80, 800);
            int y_placement = rnd.Next(-280, 509);

            Bitmap metal_layer = new Bitmap(template_width, template_height);
            Bitmap metallic_cover = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//Profile//StatusScreens//Decor//{decor_id}//Metals//{metal_select}.png");

            using (Graphics graphics = Graphics.FromImage(metal_layer))
            {
                graphics.DrawImage(metallic_cover, x_placement, y_placement, metallic_cover.Width, metallic_cover.Height);
            }

            base_template = Keep_Pixel_Overlap(base_template, metal_layer, level_box, false);

            Bitmap fade = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//Profile//StatusScreens//Decor//{decor_id}//fade.png");

            fade = Keep_Pixel_Overlap(base_template, fade, level_box, true);

            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                graphics.DrawImage(fade, 0, 0, fade.Width, fade.Height);
            }

            return base_template;
        }

        public static Point ProficiencyGraphPoint(int rank)
        {
            Point graph_point = new Point(0, 0);

            if (rank == 1)
            {
                graph_point = new Point(208, 213);
            }
            else if (rank == 2)
            {
                graph_point = new Point(208, 185);
            }
            else if (rank == 3)
            {
                graph_point = new Point(208, 157);
            }
            else if (rank == 4)
            {
                graph_point = new Point(208, 129);
            }
            else if (rank == 5)
            {
                graph_point = new Point(208, 101);
            }

            return graph_point;
        }

        public static Point DiligenceGraphPoint(int rank)
        {
            Point graph_point = new Point(0, 0);

            if (rank == 1)
            {
                graph_point = new Point(183, 254);
            }
            else if (rank == 2)
            {
                graph_point = new Point(159, 268);
            }
            else if (rank == 3)
            {
                graph_point = new Point(134, 282);
            }
            else if (rank == 4)
            {
                graph_point = new Point(110, 296);
            }
            else if (rank == 5)
            {
                graph_point = new Point(90, 308);
            }

            return graph_point;
        }

        public static Point ExpressionGraphPoint(int rank)
        {
            Point graph_point = new Point(0, 0);

            if (rank == 1)
            {
                graph_point = new Point(232, 254);
            }
            else if (rank == 2)
            {
                graph_point = new Point(257, 268);
            }
            else if (rank == 3)
            {
                graph_point = new Point(281, 282);
            }
            else if (rank == 4)
            {
                graph_point = new Point(306, 296);
            }
            else if (rank == 5)
            {
                graph_point = new Point(326, 308);
            }

            return graph_point;
        }

        public static Bitmap RenderPrestigeCounter(int level_resets)
        {
            // Copy the prestige counter overlay to a bitmap.
            Bitmap prestige_overlay = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//Profile//StatusScreens//Decor//{decor_id}//Prestige//prestige_counter.png");

            // Copy the star to mark prestige to a bitmap.
            Bitmap prestige_star = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//Profile//StatusScreens//Decor//{decor_id}//Prestige//star.png");

            // Create a new bitmap.
            Bitmap new_bitmap = new Bitmap(template_width, template_height);

            // Use a graphics object to edit the bitmap.
            using (Graphics graphics = Graphics.FromImage(new_bitmap))
            {
                // Draw the prestige overlay to the template.
                graphics.DrawImage(prestige_overlay, 0, 0, template_width, template_height);

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
                    graphics.DrawImage(prestige_star, 21 + (i * 42), 646, 40, 40);
                }
            }

            return new_bitmap;
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

        public static Bitmap Keep_Pixel_Overlap(Bitmap bottom_bitmap, Bitmap top_bitmap, Rectangle level_box, bool Is_Fade_Layer)
        {
            System.Drawing.Color bottom_pixel_color;
            System.Drawing.Color top_pixel_color;
            System.Drawing.Color new_color;

            Bitmap output_bitmap = new Bitmap(bottom_bitmap.Width, bottom_bitmap.Height);

            for (int x = level_box.X; x < (level_box.X + level_box.Width); x++)
            {
                for (int y = level_box.Y; y < (level_box.Y + level_box.Height); y++)
                {
                    bottom_pixel_color = bottom_bitmap.GetPixel(x, y);
                    top_pixel_color = top_bitmap.GetPixel(x, y);

                    if (bottom_pixel_color.A > 0 && top_pixel_color.A > 0)
                    {
                        switch (Is_Fade_Layer)
                        {
                            case true:
                                if (bottom_pixel_color.A < 150)
                                {
                                    new_color = System.Drawing.Color.FromArgb(bottom_pixel_color.A, top_pixel_color.R, top_pixel_color.G, top_pixel_color.B);
                                }
                                else
                                {
                                    new_color = System.Drawing.Color.FromArgb(top_pixel_color.A, top_pixel_color.R, top_pixel_color.G, top_pixel_color.B);
                                }
                                break;

                            case false:
                                new_color = System.Drawing.Color.FromArgb(bottom_pixel_color.A, top_pixel_color.R, top_pixel_color.G, top_pixel_color.B);
                                break;
                        }

                        output_bitmap.SetPixel(x, y, new_color);
                    }
                }
            }

            return output_bitmap;
        }

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

        public static EmbedBuilder LoadingMessage()
        {
            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Loading Status...",
                IconUrl = EmbedSettings.Get_Game_Logo("P4AU")
            };

            embed.WithAuthor(author);
            embed.WithColor(EmbedSettings.Get_Game_Color("P4AU", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Loading_Icon("P4AU", null));
            embed.WithDescription("This may take a few seconds!");

            return embed;
        }
    }
}
