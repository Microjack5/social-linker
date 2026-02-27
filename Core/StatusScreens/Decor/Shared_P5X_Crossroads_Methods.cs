using Discord;
using Discord.Rest;
using Discord.WebSocket;
using SocialLinker.Config;
using SocialLinker.Core.CloudStorageTables;
using System;
using System.Drawing;
using System.Drawing.Text;
using System.IO;

namespace SocialLinker.Core.StatusScreens.Decor
{
    public class Shared_P5X_Crossroads_Methods
    {
        public const int template_width = 1920;
        public const int template_height = 1080;

        public static async void RenderImage(SocketUser user, ISocketMessageChannel channel, string decor_id)
        {
            RestUserMessage loader = await channel.SendMessageAsync("", false, LoadingMessage().Build());

            try
            {
                var account = UserInfoClasses.GetAccount(user);

                Bitmap base_template = new Bitmap(template_width, template_height);

                Bitmap layer_1 = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//Profile//StatusScreens//Decor//{decor_id}//layer_1.png");

                using (Graphics graphics = Graphics.FromImage(base_template))
                {
                    Bitmap text_layer = CreateTextLayer(user);

                    graphics.DrawImage(layer_1, 0, 0, layer_1.Width, layer_1.Height);

                    graphics.DrawImage(text_layer, 0, 0, text_layer.Width, text_layer.Height);
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

                await loader.DeleteAsync();

                return;
            }
        }

        public static Bitmap CreateTextLayer(SocketUser user)
        {
            var account = UserInfoClasses.GetAccount(user);

            var guild_user = user as SocketGuildUser;
            string display_name = guild_user?.DisplayName ?? user.Username;

            if (display_name.Length > 17)
            {
                display_name = $"{display_name.Substring(0, 14)}...";
            }

            Bitmap base_template = new Bitmap(template_width, template_height);

            SolidBrush white_brush = new SolidBrush(System.Drawing.Color.White);
            SolidBrush black_brush = new SolidBrush(System.Drawing.Color.Black);
            SolidBrush gray_brush = new SolidBrush(System.Drawing.Color.FromArgb(89, 89, 89));

            int next_exp = 0;
            if (account.Level != 99)
            {
                next_exp = Core.LevelSystem.Leveling.CalculateExp(account.Level + 1) - account.Total_Exp;
            }

            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                graphics.TextRenderingHint = TextRenderingHint.AntiAlias;

                // Username
                using (Font username_font = new Font("Nirmala Text", 30, FontStyle.Bold))
                {
                    Rectangle username_box = new Rectangle(1270, 255, 398, 60);
                    StringFormat stringFormat = new StringFormat();
                    stringFormat.Alignment = StringAlignment.Center;
                    graphics.DrawString($"{display_name}", username_font, white_brush, username_box, stringFormat);
                }

                // Level
                using (Font level_font = new Font("Nirmala Text", 45, FontStyle.Bold))
                {
                    Rectangle level_white_box = new Rectangle(1760, 241, 150, 60);
                    Rectangle level_gray_box = new Rectangle(1764, 245, 150, 60);
                    StringFormat stringFormat = new StringFormat();
                    stringFormat.Alignment = StringAlignment.Near;
                    graphics.DrawString($"{account.Level}", level_font, gray_brush, level_gray_box, stringFormat);
                    graphics.DrawString($"{account.Level}", level_font, white_brush, level_white_box, stringFormat);
                }

                // Next Level
                using (Font p5r_font = new Font("Optima nova LT Black", 20))
                {
                    Rectangle next_level_box = new Rectangle(1546, 393, 200, 35);
                    StringFormat stringFormat = new StringFormat();
                    stringFormat.Alignment = StringAlignment.Far;
                    graphics.DrawString($"{next_exp}", p5r_font, black_brush, next_level_box, stringFormat);
                }

                // Proficiency
                using (Font p5r_font = new Font("Optima nova LT Black", 20))
                {
                    Rectangle proficiency_box = new Rectangle(1625, 467, 200, 35);
                    StringFormat stringFormat = new StringFormat();
                    stringFormat.Alignment = StringAlignment.Center;
                    graphics.DrawString($"{account.Proficiency_Rank}", p5r_font, black_brush, proficiency_box, stringFormat);
                }

                // Dilligence
                using (Font p5r_font = new Font("Optima nova LT Black", 20))
                {
                    Rectangle dilligence_box = new Rectangle(1637, 544, 200, 35);
                    StringFormat stringFormat = new StringFormat();
                    stringFormat.Alignment = StringAlignment.Center;
                    graphics.DrawString($"{account.Diligence_Rank}", p5r_font, black_brush, dilligence_box, stringFormat);
                }

                // Expression
                using (Font p5r_font = new Font("Optima nova LT Black", 20))
                {
                    Rectangle expression_box = new Rectangle(1654, 620, 200, 35);
                    StringFormat stringFormat = new StringFormat();
                    stringFormat.Alignment = StringAlignment.Center;
                    graphics.DrawString($"{account.Expression_Rank}", p5r_font, black_brush, expression_box, stringFormat);
                }

                // P-Medals
                using (Font pmedal_font = new Font("Geometris Round Bold Semi-Condensed", 24, FontStyle.Italic))
                {
                    Rectangle pmedal_white_box = new Rectangle(1336, 748, 100, 35);
                    Rectangle pmedal_black_box = new Rectangle(1338, 750, 100, 35);
                    StringFormat stringFormat = new StringFormat();
                    stringFormat.Alignment = StringAlignment.Far;
                    stringFormat.LineAlignment = StringAlignment.Far;
                    graphics.DrawString($"{account.P_Medals}", pmedal_font, black_brush, pmedal_black_box, stringFormat);
                    graphics.DrawString($"{account.P_Medals}", pmedal_font, white_brush, pmedal_white_box, stringFormat);
                }

                // User ID
                using (Font user_id_font = new Font("Optima nova LT Black", 15))
                {
                    Rectangle user_id_box = new Rectangle(1636, 1051, 275, 25);
                    StringFormat stringFormat = new StringFormat();
                    stringFormat.Alignment = StringAlignment.Near;
                    graphics.DrawString($"ID:{user.Id}", user_id_font, white_brush, user_id_box, stringFormat);
                }
            }

            return base_template;
        }

        public static EmbedBuilder LoadingMessage()
        {
            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Loading Status...",
                IconUrl = "https://i.imgur.com/iwNMmyQ.png"
            };

            embed.WithAuthor(author);
            embed.WithColor(213, 27, 4);
            embed.WithThumbnailUrl("https://i.imgur.com/kWx4K6h.gif");
            embed.WithDescription("This may take a few seconds!");

            return embed;
        }
    }
}
