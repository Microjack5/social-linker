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

namespace SocialLinker.Core.StatusScreens.Decor
{
    public static class Decor_P3P_1
    {
        public static async void RenderImage(SocketUser user, ISocketMessageChannel channel)
        {
            // Send a loading message while the status screen gets made
            RestUserMessage loader = await channel.SendMessageAsync("", false, LoadingMessage().Build());

            // Place the bulk of the function in a try-catch block in case something fails and an error message needs to be sent
            try
            {
                //Grab the user's account information
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
                int total_exp = account.Total_Exp;
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
                Bitmap p3_template = new Bitmap(1920, 1080);

                //Copy the P3 status template to a bitmap
                Bitmap base_layer = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//Profile//StatusScreens//Decor//Decor_P3P_1//p3p_template.png");

                //Copy the EXP bar to a bitmap
                Bitmap exp_bar = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//Profile//StatusScreens//Decor//Decor_P3P_1//exp_bar.png");

                //Copy the rank bar to a bitmap
                Bitmap rank_bar = RankBarBitmap();

                //Use a graphics object to edit the bitmap
                using (Graphics graphics = Graphics.FromImage(p3_template))
                {
                    //Set text rendering to have antialiasing
                    graphics.TextRenderingHint = TextRenderingHint.AntiAlias;

                    // Create color and brush
                    System.Drawing.Color dark_red = System.Drawing.Color.FromArgb(107, 1, 10);
                    System.Drawing.Color light_pink = System.Drawing.Color.FromArgb(221, 29, 93);
                    SolidBrush dark_red_brush = new SolidBrush(dark_red);
                    SolidBrush light_pink_brush = new SolidBrush(light_pink);

                    // Draw the first bitmap layer to the template
                    graphics.DrawImage(base_layer, 0, 0, base_layer.Width, base_layer.Height);

                    // Draw the rank titles to the bitmap.
                    using (Font p4g_font = new Font("P4G", 37))
                    {
                        graphics.DrawString(proficiency_title, p4g_font, System.Drawing.Brushes.White, new Point(348, 141));
                        graphics.DrawString(diligence_title, p4g_font, System.Drawing.Brushes.White, new Point(348, 280));
                        graphics.DrawString(expression_title, p4g_font, System.Drawing.Brushes.White, new Point(348, 411));
                    }

                    //Draw PLV text
                    using (Font street = new Font("Street Corner Extend", 25))
                    {
                        graphics.DrawString("PLV", street, dark_red_brush, new Point(190, 613));
                    }

                    //Draw level number
                    using (Font street = new Font("Street Corner Extend", 32))
                    {
                        graphics.DrawString(level, street, dark_red_brush, new Point(307, 613));
                    }

                    //Draw username
                    using (Font p4g_font = new Font("P4G", 34))
                    {
                        graphics.DrawString(username, p4g_font, dark_red_brush, new Point(492, 610));
                    }

                    //Draw "Total EXP" & "Next EXP" text, plus total EXP value
                    using (Font street = new Font("Street Corner Extend", 28))
                    {
                        graphics.DrawString("TOTAL EXP", street, dark_red_brush, new Point(237, 817));
                        graphics.DrawString("NEXT EXP", street, dark_red_brush, new Point(190, 681));

                        Rectangle box1 = new Rectangle(550, 817, 362, 46);
                        StringFormat stringFormat = new StringFormat();
                        stringFormat.Alignment = StringAlignment.Far;
                        stringFormat.LineAlignment = StringAlignment.Far;
                        graphics.DrawString($"{total_exp}", street, dark_red_brush, box1, stringFormat);
                    }

                    //Draw P-Medal value
                    using (Font street = new Font("Street Corner Extend", 31))
                    {
                        Rectangle box1 = new Rectangle(817, 743, 190, 42);
                        StringFormat stringFormat = new StringFormat();
                        stringFormat.Alignment = StringAlignment.Center;
                        stringFormat.LineAlignment = StringAlignment.Center;
                        graphics.DrawString(pmedals, street, light_pink_brush, box1, stringFormat);
                    }

                    //Draw ranks for Proficiency
                    for (int i = 0; i < account.Proficiency_Rank; i++)
                    {
                        graphics.DrawImage(rank_bar, 355 + (94 * i), 123, 87, 14);
                    }

                    //Draw ranks for Diligence
                    for (int i = 0; i < account.Diligence_Rank; i++)
                    {
                        graphics.DrawImage(rank_bar, 355 + (94 * i), 261, 87, 14);
                    }

                    //Draw ranks for Expression
                    for (int i = 0; i < account.Expression_Rank; i++)
                    {
                        graphics.DrawImage(rank_bar, 355 + (94 * i), 393, 87, 14);
                    }

                    // Draw a bitmap where the proper Next EXP progress bar overlaps with the exp_bar decorative image
                    graphics.DrawImage(KeepPixelOverlap(RenderProgressBar(user), exp_bar), 0, 0, 1920, 1080);

                    //Use a web client to download the user's profile picture and draw it to the template
                    using (var wc = new WebClient())
                    {
                        using (var imgStream = new MemoryStream(wc.DownloadData(profile_picture)))
                        {
                            using (var objImage = System.Drawing.Image.FromStream(imgStream))
                            {
                                graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                                graphics.DrawImage(objImage, 1239, 240, 600, 600);
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
                p3_template.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
                memoryStream.Seek(0, SeekOrigin.Begin);

                //Send the image
                await channel.SendFileAsync(memoryStream, $"status_{user.Id}_{DateTime.UtcNow}.png");

                //Delete the loading message
                await loader.DeleteAsync();
            }
            catch (Exception ex)
            {
                //Send an error message to the user
                await channel.SendMessageAsync($":warning: Oops! It looks like something went wrong here. Please try again.");
                Console.WriteLine(ex);

                //Delete the loading message
                await loader.DeleteAsync();

                return;
            }
        }

        public static Bitmap RankBarBitmap()
        {
            // Create a bitmap to render the color on
            Bitmap new_bitmap = new Bitmap(87, 14);

            // Use a graphics object to edit the bitmap
            using (Graphics graphics = Graphics.FromImage(new_bitmap))
            {
                // Color the entire bitmap with a yellow color
                graphics.Clear(System.Drawing.Color.FromArgb(255, 221, 0));
            }

            return new_bitmap;
        }

        public static Bitmap RenderProgressBar(SocketUser user)
        {
            // Grab the user's account information
            var account = UserInfoClasses.GetAccount(user);

            // Establish other variables of the user's data
            int total_exp = account.Total_Exp;

            // Create a working space bitmap
            Bitmap new_bitmap = new Bitmap(1920, 1080);

            // Use a graphics object to edit the bitmap
            using (Graphics graphics = Graphics.FromImage(new_bitmap))
            {
                // Create a variable for the minimum total EXP requirement of the next level
                int next_level_total_exp_required = Core.LevelSystem.Leveling.CalculateExp(account.Level + 1);

                // Create a variable for the minimum total EXP requirement of the current level
                int current_level_total_exp_required = Core.LevelSystem.Leveling.CalculateExp(account.Level);

                // Draw a progress bar that has a max value set to the minimum total EXP requirement of the next level minus the minimum total EXP requirement of the current level
                ProgressBar progress_bar = new ProgressBar(System.Drawing.Brushes.Yellow, new Rectangle(496, 684, 416, 37), next_level_total_exp_required - current_level_total_exp_required);

                // However, if the user's level is 99, set the progress bar's max value to 1
                if (account.Level == 99)
                {
                    progress_bar = new ProgressBar(System.Drawing.Brushes.Yellow, new Rectangle(496, 684, 416, 37), 1);
                }

                // Determine how the progress bar should be filled based on the user's level
                if (account.Level != 99)
                {
                    // If the level is below 99, fill the progress bar by subtracting the total EXP the user has by the minimum total EXP requirement of the current level
                    progress_bar.SetCurrent(total_exp - current_level_total_exp_required);
                }
                else
                {
                    // If the level is at 99, fill the progress bar completely up
                    progress_bar.SetCurrent(1);
                }

                // Draw the progress bar to the bitmap
                graphics.DrawImage(progress_bar.GiveGraphic(), progress_bar.GiveCorner());
            }

            return new_bitmap;
        }

        public static Bitmap KeepPixelOverlap(Bitmap bottom_bitmap, Bitmap top_bitmap)
        {
            System.Drawing.Color bottom_pixel_color;
            System.Drawing.Color top_pixel_color;

            // Make an empty bitmap the same size as scrBitmap
            Bitmap newBitmap = new Bitmap(bottom_bitmap.Width, bottom_bitmap.Height);

            // Establish a nested loop that iterates on the X and Y axis of the image within a region
            for (int i = 496; i < 914; i++)
            {
                for (int j = 684; j < 721; j++)
                {
                    // Get a pixel from the same position on both bitmaps
                    bottom_pixel_color = bottom_bitmap.GetPixel(i, j);
                    top_pixel_color = top_bitmap.GetPixel(i, j);

                    // If both pixels have an alpha value above 0 and overlap, draw the top pixel to the new bitmap
                    if (bottom_pixel_color.A > 0 && top_pixel_color.A > 0)
                    {
                        newBitmap.SetPixel(i, j, top_pixel_color);
                    }

                }
            }

            return newBitmap;
        }

        public static Bitmap RenderPrestigeCounter(int level_resets)
        {
            // Copy the prestige counter overlay to a bitmap.
            Bitmap prestige_overlay = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//Profile//StatusScreens//Decor//Decor_P3P_1//prestige_counter.png");

            // Copy the star to mark prestige to a bitmap.
            Bitmap prestige_star = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//Profile//StatusScreens//Decor//Decor_P3P_1//star.png");

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
                    graphics.DrawImage(prestige_star, 79, 705 - (i * 59), 50, 50);
                }
            }

            return new_bitmap;
        }

        public static EmbedBuilder LoadingMessage()
        {
            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Loading Status...",
                IconUrl = "https://i.imgur.com/y7x9wce.png"
            };

            embed.WithAuthor(author);
            embed.WithColor(255, 117, 154);
            embed.WithThumbnailUrl("https://i.imgur.com/pkb4OJa.gif");
            embed.WithDescription("This may take a few seconds!");

            return embed;
        }
    }
}
