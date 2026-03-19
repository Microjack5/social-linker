using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Newtonsoft.Json;
using SocialLinker.Config;
using SocialLinker.Core.CloudStorageTables;
using SocialLinker.Core.LocalStorageTables;
using SocialLinker.Core.Menus;
using SocialLinker.Core.SceneMaker.Data.Bustup;
using SocialLinker.Core.SceneMaker.GlyphParsing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;

namespace SocialLinker.Core.SceneMaker.TemplateRenders.QuickScenes
{
    public class RenderP3R
    {
        public int template_width_4k = 3840;
        public int template_height_4k = 2160;

        public int template_width = 1920;
        public int template_height = 1080;

        public int max_line_length = 480;

        public System.Drawing.Color nametag_color = System.Drawing.Color.FromArgb(11, 239, 239);

        public Rectangle calendar_area = new Rectangle(1295, 0, 625, 150);

        System.Drawing.Color time_of_day_dark_hour_color = System.Drawing.Color.FromArgb(53, 255, 121);
        System.Drawing.Color type_of_day_dark_hour_color = System.Drawing.Color.FromArgb(5, 90, 20);

        System.Drawing.Color time_of_day_default_color = System.Drawing.Color.FromArgb(2, 253, 255);
        System.Drawing.Color type_of_day_default_color = System.Drawing.Color.FromArgb(22, 32, 103);

        public async Task Render_Quick_Scene_P3R(SocialLinkerCommand sl_command)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = sl_command.User;
            SocketTextChannel channel = (SocketTextChannel)sl_command.Channel;

            OfficialSetData set_data = sl_command.MakerCommand.Character_Data_1.Set_Data;
            MakerCommandData maker_command_data = sl_command.MakerCommand;

            // Send a loading message to the channel while the sprite sheet is being made.
            RestUserMessage loader = await channel.SendMessageAsync("", false, P3R_Loading_Message().Build());

            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            sl_command.MakerCommand.Character_Data_1.Bustup_Data = BustupDataMethods.Get_P3R_Bustup_Data(account, sl_command, set_data, sl_command.MakerCommand.Character_Data_1);
            BustupData bustup_data = sl_command.MakerCommand.Character_Data_1.Bustup_Data;

            // Background rendering
            Bitmap base_template = new Bitmap(template_width, template_height);
            Bitmap uhd_layer = new Bitmap(template_width_4k, template_height_4k);
            Bitmap colored_background_bitmap = OfficialSetMethods.Render_Colored_Background(account, template_width_4k, template_height_4k);
            Bitmap background = new Bitmap(2, 2);

            try
            {
                background = OfficialSetMethods.Render_Background(sl_command, template_width_4k, template_height_4k);
            }
            catch (System.ArgumentException e)
            {
                Console.WriteLine(e);
                await loader.DeleteAsync();
                _ = ErrorHandling.Incompatible_File_Type(sl_command);
                return;
            }

            // Next, time for the conversation portrait! Create and initialize a new bitmap variable for it.
            Bitmap bustup = new Bitmap(2, 2);

            // Check if the base sprite number is something other than zero.
            // If it is zero, we have nothing to render. Otherwise, retrieve the bustup.
            if (maker_command_data.Character_Data_1.Base_Sprite != 0)
            {
                bustup = OfficialSetMethods.Bustup_Selection(sl_command, account, sl_command.MakerCommand.Character_Data_1);
            }

            // If the bustup returns as null, however, something went wrong with rendering the animation frames.
            // An error message has already been sent in the frame rendering method, so delete the loading message and return.
            if (bustup == null)
            {
                await loader.DeleteAsync();
                return;
            }

            // Time to put it all together!
            using (Graphics graphics = Graphics.FromImage(uhd_layer))
            {
                // Draw the character bust-up to the template if the base sprite number is not '0'.
                if (maker_command_data.Character_Data_1.Base_Sprite != 0)
                {
                    Bitmap drop_shadow = Create_Bustup_Drop_Shadow(bustup);
                    graphics.DrawImage(drop_shadow, bustup_data.P3R_Coord_X - 44, bustup_data.P3R_Coord_Y + 28, bustup_data.P3R_Scale_Width, bustup_data.P3R_Scale_Height);
                    graphics.DrawImage(bustup, bustup_data.P3R_Coord_X, bustup_data.P3R_Coord_Y, bustup_data.P3R_Scale_Width, bustup_data.P3R_Scale_Height);
                }
            }

            uhd_layer = Scale_Template(account, uhd_layer);

            DialogueRenderer renderer = new DialogueRenderer();

            DialogueRenderResult result = renderer.RenderDialogueAdvanced(
                dialogue: sl_command.MakerCommand.Dialogue,
                bitmapWidth: 1920,
                bitmapHeight: 1080,
                startX: 637f + 192f,
                startY: 869f,
                letterSpacing: 12f, //0.1
                spaceScale: 1f,
                lineSpacing: -19f,
                drawOutline: false,
                fillColor: System.Drawing.Color.White,
                outlineColor: System.Drawing.Color.Black,
                outlineWidth: 2.5f
            );

            Bitmap dialogue_bitmap = result.Bitmap;
            int lineCount = result.LineCount;
            float longestLine = result.LongestLineWidth;

            Bitmap nametag_layer = renderer.RenderName(
                name: OfficialSetMethods.GetDisplayName(account, sl_command.MakerCommand.Character_Data_1),
                bitmapWidth: 1920,
                bitmapHeight: 1080,
                x: 548f,
                y: 803f,
                letterSpacing: -1.5f,
                spaceScale: 0.5f,
                drawOutline: false,
                fillColor: nametag_color,
                outlineColor: System.Drawing.Color.Black,
                outlineWidth: 2.5f
            );

            Bitmap message_bg = RenderMessageWindow(result);
            Bitmap control_panel = RenderControlPanel(account);

            Bitmap bustup_bg = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3R//Main//bustup_bg.png");

            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                graphics.DrawImage(background, 0, 0, template_width, template_height);

                graphics.DrawImage(bustup_bg, 0, 0, template_width, template_height);

                graphics.DrawImage(uhd_layer, 0, 0, uhd_layer.Width, uhd_layer.Height);
                graphics.DrawImage(Render_Calendar_HUD_2(account), 0, 0, template_width, template_height);
                graphics.DrawImage(Render_Calendar_HUD(account), 0, 0, template_width, template_height);
                graphics.DrawImage(Render_Moon_HUD(account), 0, 0, template_width, template_height);
                graphics.DrawImage(message_bg, 0, 0, message_bg.Width, message_bg.Height);
                graphics.DrawImage(dialogue_bitmap, 0, 0, dialogue_bitmap.Width, dialogue_bitmap.Height);
                graphics.DrawImage(nametag_layer, 0, 0, nametag_layer.Width, nametag_layer.Height);
                graphics.DrawImage(control_panel, 0, 0, template_width, template_height);
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
                _ = ErrorHandling.Image_Upload_Failed(sl_command);

                // Clean up resources used by the stream, delete the loading message, and return.
                memoryStream.Dispose();
                await loader.DeleteAsync();
                return;
            }

            // Delete the loading message.
            await loader.DeleteAsync();

            // If the user has auto-delete for their commands set to on, delete their command as well.
            if (account.Auto_Delete_Commands == "On")
            {
                await sl_command.Message.DeleteAsync();
            }
        }

        public Bitmap RenderMessageWindow(DialogueRenderResult result)
        {
            Bitmap message_window = new Bitmap(1920, 1080);

            Bitmap message_background = new Bitmap(1920, 1080);
            Bitmap message_main = new Bitmap(1920, 1080);
            Bitmap nametag_layer = new Bitmap(1920, 1080);
            Bitmap advance_layer = new Bitmap(1920, 1080);

            Bitmap talk_layer = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3R//Main//talk.png");
            talk_layer = Bitmap_To_Color(talk_layer, nametag_color, new Rectangle(551, 842, 58, 8));

            float base_box_x = 500f;
            float base_box_y = 784f;
            float base_box_width = 798f; // Med = 127f, Large = 264f
            float base_box_height = 210f;

            float box_y_growth_medium = -17;
            float box_height_growth_medium = 32;
            float box_width_growth_medium = 127;

            float box_y_growth_large = -26;
            float box_height_growth_large = 41;
            float box_width_growth_large = 264;

            float box_x = base_box_x;
            float box_y = base_box_y;
            float box_width = base_box_width;
            float box_height = base_box_height;

            int line_small_max_width = 500;
            int line_medium_max_width = 650;
            //int line_large_max_width = 820;

            int msg_bg_x = 15;
            int msg_bg_y = -14;

            if (result.LongestLineWidth <= line_small_max_width)
            {
                // Do nothing
            }
            else if (result.LongestLineWidth <= line_medium_max_width)
            {
                box_width += box_width_growth_medium;

                msg_bg_x = 19;

            }
            else
            {
                box_width += box_width_growth_large;

                msg_bg_x = 18;
            }

            switch (result.LineCount)
            {
                case 1:
                    box_y = 784f;
                    box_height = 210f;
                    break;

                case 2:
                    box_y = box_y + box_y_growth_medium;
                    box_height = box_height + box_height_growth_medium;
                    break;

                case 3:
                default:
                    box_y = box_y + box_y_growth_large;
                    box_height = box_height + box_height_growth_large;
                    msg_bg_y += 9;
                    break;
            }

            Bitmap background_tilt = new Bitmap(message_window.Width, message_window.Height);

            using (Graphics graphics = Graphics.FromImage(background_tilt))
            using (SolidBrush cyanBrush = new SolidBrush(System.Drawing.Color.FromArgb(185, 0, 80, 255)))
            {
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphics.CompositingQuality = CompositingQuality.HighQuality;

                Persona3ReloadMessageInnerResizable.FillMessageInnerRotated(
                    graphics,
                    cyanBrush,
                    box_x + 5f,
                    box_y + 5f,
                    box_x + box_width,
                    box_y + box_height,
                    -4f,   // slight tilt
                    0.5f,    // rotate around center X
                    0.5f     // rotate around center Y
                );
            }

            using (Graphics graphics = Graphics.FromImage(message_background))
            {
                
                graphics.DrawImage(background_tilt, msg_bg_x, msg_bg_y, background_tilt.Width, background_tilt.Height);
            }

            int scale = 4;

            // Create a high-resolution temporary surface
            Bitmap message_supersample = new Bitmap(
                message_main.Width * scale,
                message_main.Height * scale,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            // Scale all coordinates
            float s_box_x = box_x * scale;
            float s_box_y = box_y * scale;
            float s_box_width = box_width * scale;
            float s_box_height = box_height * scale;

            float innerX = s_box_x + (5f * scale);
            float innerY = s_box_y + (5f * scale);
            float innerWidth = s_box_width - (10f * scale);
            float innerHeight = s_box_height - (10f * scale);
            float cutoutInset = 1f * scale;

            // メッセージ下地
            using (Graphics graphics = Graphics.FromImage(message_supersample))
            using (SolidBrush msgOutlineBrush = new SolidBrush(System.Drawing.Color.FromArgb(255, 22, 36, 99)))
            {
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphics.CompositingQuality = CompositingQuality.HighQuality;

                Persona3ReloadMessageBaseResizable.FillMessageBaseBySize(
                    graphics,
                    msgOutlineBrush,
                    s_box_x,
                    s_box_y,
                    s_box_width,
                    s_box_height
                );
            }

            // Smaller transparent cutout
            using (Graphics graphics = Graphics.FromImage(message_supersample))
            using (GraphicsPath cutoutPath = Persona3ReloadMessageInnerResizable.BuildScaledPathBySize(
                innerX + cutoutInset,
                innerY + cutoutInset,
                innerWidth - (cutoutInset * 2f),
                innerHeight - (cutoutInset * 2f)))
            using (SolidBrush transparentBrush = new SolidBrush(System.Drawing.Color.Transparent))
            {
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphics.CompositingQuality = CompositingQuality.HighQuality;

                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.FillPath(transparentBrush, cutoutPath);
                graphics.CompositingMode = CompositingMode.SourceOver;
            }

            // メッセージ中身
            using (Graphics graphics = Graphics.FromImage(message_supersample))
            {
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphics.CompositingQuality = CompositingQuality.HighQuality;

                //Persona3ReloadMessageInnerResizable.FillMessageInnerWithHorizontalGradientBySize(
                //    graphics,
                //    innerX,
                //    innerY,
                //    innerWidth,
                //    innerHeight
                //);

                Persona3ReloadMessageInnerResizable.FillMessageInnerWithHorizontalGradientDitheredBySize(
                    graphics,
                    innerX,
                    innerY,
                    innerWidth,
                    innerHeight,
                    3
                );
            }

            // Downscale back to the final surface
            using (Graphics graphics = Graphics.FromImage(message_main))
            {
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphics.CompositingQuality = CompositingQuality.HighQuality;

                graphics.DrawImage(
                    message_supersample,
                    new Rectangle(0, 0, message_main.Width, message_main.Height),
                    new Rectangle(0, 0, message_supersample.Width, message_supersample.Height),
                    GraphicsUnit.Pixel);
            }

            // Cleanup
            message_supersample.Dispose();

            float nametag_y = 0f;

            switch (result.LineCount)
            {
                case 1:
                    // Do nothing
                    break;

                case 2:
                    nametag_y -= 11f;
                    break;

                case 3:
                    nametag_y -= 23f;
                    break;

                default:
                    nametag_y = nametag_y - ((box_height - base_box_height) / 2);
                    break;
            }

            // 話者名下地　バストアップあり (Tail)
            using (Graphics graphics = Graphics.FromImage(nametag_layer))
            using (SolidBrush tailBrush = new SolidBrush(System.Drawing.Color.FromArgb(22, 36, 99)))
            {
                Point tail_point_1 = new Point(452, 842);
                Point tail_point_2 = new Point(502, 859);
                Point tail_point_3 = new Point(505, 829);

                Point[] tail_points = {
                    tail_point_1,
                    tail_point_2,
                    tail_point_3
                };

                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                graphics.FillPolygon(tailBrush, tail_points);
            }

            // 話者名下地　バストアップあり
            using (Graphics graphics = Graphics.FromImage(nametag_layer))
            using (SolidBrush nametagBrush = new SolidBrush(System.Drawing.Color.FromArgb(23, 0, 254)))
            {
                Persona3ReloadSpeakerNameBaseBustupResizable.FillSpeakerNameBaseBustupBySize(
                    graphics,
                    nametagBrush,
                    431f,   // x
                    778f,   // y
                    236f,  // width
                    92f    // height
                );
            }

            // 話者名下地　バストアップなし
            //using (Graphics graphics = Graphics.FromImage(nametag_layer))
            //using (SolidBrush pinkBrush = new SolidBrush(System.Drawing.Color.Pink))
            //{
            //    Persona3ReloadSpeakerNameBaseNoBustupResizable.FillSpeakerNameBaseNoBustupBySize(
            //        graphics,
            //        pinkBrush,
            //        431f,   // x
            //        778f,   // y
            //        236f,  // width
            //        92f    // height
            //    );
            //}

            // 話者名しっぽ下地　バストアップ
            //using (Graphics graphics = Graphics.FromImage(message_bg))
            //using (SolidBrush pinkBrush = new SolidBrush(System.Drawing.Color.Orange))
            //{
            //    Persona3ReloadSpeakerNameTailBaseBustupResizable.FillSpeakerNameTailBaseBustupBySize(
            //        graphics,
            //        pinkBrush,
            //        431f,   // x
            //        778f,   // y
            //        236f,  // width
            //        92f    // height
            //    );
            //}

            // 文字送り (2)
            using (Graphics graphics = Graphics.FromImage(advance_layer))
            using (SolidBrush advanceBaseBrush = new SolidBrush(System.Drawing.Color.FromArgb(43, 45, 254)))
            {
                Persona3ReloadTextAdvance.VariantB.FillBySize(
                    graphics,
                    advanceBaseBrush,
                    1205f + (box_width - base_box_width),   // x
                    926f + ((box_height - base_box_height) / 2),   // y
                    90f,  // width
                    42f    // height
                );
            }

            // 文字送り (1)
            using (Graphics graphics = Graphics.FromImage(advance_layer))
            using (SolidBrush advanceArrowBrush = new SolidBrush(System.Drawing.Color.White))
            {
                Persona3ReloadTextAdvance.VariantA.FillBySize(
                    graphics,
                    advanceArrowBrush,
                    1212f + (box_width - base_box_width),   // x
                    930f + ((box_height - base_box_height) / 2),   // y
                    71f,  // width
                    34f    // height
                );
            }

            using (Graphics graphics = Graphics.FromImage(message_window))
            {
                graphics.DrawImage(message_background, 0, 0, message_background.Width, message_background.Height);
                graphics.DrawImage(message_main, 0, 0, message_main.Width, message_main.Height);
                graphics.DrawImage(nametag_layer, 0, nametag_y, nametag_layer.Width, nametag_layer.Height);
                graphics.DrawImage(talk_layer, 0, nametag_y, talk_layer.Width, talk_layer.Height);
                graphics.DrawImage(advance_layer, 0, 0, advance_layer.Width, advance_layer.Height);
            }

            return message_window;
        }

        public Bitmap RenderControlPanel(UserInfoFields account)
        {
            Bitmap buttons_ps5_1 = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3R//Main//Control_Panel//buttons_ps5_1.png");

            return buttons_ps5_1;
        }

        public Bitmap Render_Calendar_HUD(UserInfoFields account)
        {
            Bitmap base_template = new Bitmap(template_width, template_height);
            DateTime user_time = Get_Date(account);

            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                string time_of_day = Get_Time_of_Day(user_time);

                Bitmap month_layer = new Bitmap(template_width, template_height);

                Bitmap day_tens_layer = new Bitmap(template_width, template_height);
                Bitmap day_ones_layer = new Bitmap(template_width, template_height);

                Bitmap day_of_week_layer = new Bitmap(template_width, template_height);
                Bitmap time_of_day_layer = new Bitmap(template_width, template_height);
                Bitmap day_triangle_layer = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3R//Main//Calendar//Weekday//_triangle.png");

                month_layer = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3R//Main//Calendar//Month//{user_time.Month}.png");

                // Get the user's current day and convert it to a char array.
                char[] day = user_time.ToString("dd").ToCharArray();

                // If the day is not a single digit, get the appropriate bitmap for the tens place of the day.
                if (day[0] != '0')
                {
                    day_tens_layer = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3R//Main//Calendar//Day//Tens_Place//{day[0]}.png");
                }

                // Regardless, get the appropriate bitmap for the ones place of the day.
                day_ones_layer = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3R//Main//Calendar//Day//Ones_Place//{day[1]}.png");

                // Get the appropriate bitmaps for the weekday and time of day for the user.
                day_of_week_layer = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3R//Main//Calendar//Weekday//{user_time.ToString("dddd").ToLower()}.png");
                time_of_day_layer = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3R//Main//Calendar//Time_of_Day//{time_of_day}.png");

                if (time_of_day == "dark_hour")
                {
                    time_of_day_layer = Partial_Bitmap_To_Color(time_of_day_layer, System.Drawing.Color.FromArgb(49, 255, 119), calendar_area, 90);
                }
                else
                {
                    time_of_day_layer = Partial_Bitmap_To_Color(time_of_day_layer, System.Drawing.Color.FromArgb(3, 254, 255), calendar_area, 90);
                }

                System.Drawing.Color triangle_color = default;

                switch (user_time.DayOfWeek)
                {
                    case DayOfWeek.Saturday:
                        triangle_color = System.Drawing.Color.FromArgb(64, 188, 240);
                        break;

                    case DayOfWeek.Sunday:
                        triangle_color = System.Drawing.Color.FromArgb(231, 62, 31);
                        break;

                    default:
                        triangle_color = System.Drawing.Color.FromArgb(7, 11, 38);
                        break;
                }

                day_triangle_layer = Bitmap_To_Color(day_triangle_layer, triangle_color, calendar_area);

                if (day[0] == '0')
                {
                    graphics.DrawImage(month_layer, 0, 0, template_width, template_height);
                }
                else // If day is double digit, move a little to the left
                {
                    graphics.DrawImage(month_layer, -36, 0, template_width, template_height);
                }

                graphics.DrawImage(day_tens_layer, 0, 0, template_width, template_height);
                graphics.DrawImage(day_ones_layer, 0, 0, template_width, template_height);

                graphics.DrawImage(day_triangle_layer, 0, 0, template_width, template_height);
                graphics.DrawImage(day_of_week_layer, 0, 0, template_width, template_height);
                graphics.DrawImage(time_of_day_layer, 0, 0, template_width, template_height);
            }

            return base_template;
        }

        public Bitmap Render_Calendar_HUD_2(UserInfoFields account)
        {
            Bitmap base_template = new Bitmap(template_width, template_height);
            DateTime user_time = Get_Date(account);

            Bitmap type_of_day = new Bitmap(template_width, template_height);

            string time_of_day = Get_Time_of_Day(user_time);

            if (time_of_day == "dark_hour")
            {
                type_of_day = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3R//Main//Calendar//Type_of_Day//dark_hour.png");
                type_of_day = Bitmap_To_Color(type_of_day, System.Drawing.Color.FromArgb(5, 91, 20), calendar_area);
            }
            else if (user_time.DayOfWeek == DayOfWeek.Saturday || user_time.DayOfWeek == DayOfWeek.Sunday) 
            {
                type_of_day = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3R//Main//Calendar//Type_of_Day//weekend.png");
                type_of_day = Bitmap_To_Color(type_of_day, System.Drawing.Color.FromArgb(22, 32, 103), calendar_area);
            }
            else
            {
                type_of_day = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3R//Main//Calendar//Type_of_Day//weekday.png");
                type_of_day = Bitmap_To_Color(type_of_day, System.Drawing.Color.FromArgb(22, 32, 103), calendar_area);
            }

            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                graphics.DrawImage(type_of_day, 0, 0, template_width, template_height);
            }

            return base_template;
        }

        public Bitmap Render_Moon_HUD(UserInfoFields account)
        {
            Bitmap base_template = new Bitmap(template_width, template_height);

            Bitmap countdown_text_main = new Bitmap(template_width, template_height);
            Bitmap countdown_text_special = new Bitmap(template_width, template_height);

            Bitmap countdown_tens = new Bitmap(template_width, template_height);
            Bitmap countdown_ones = new Bitmap(template_width, template_height);

            Bitmap moon_phase = new Bitmap(template_width, template_height);

            DateTime user_time = Get_Date(account);

            Bitmap limit = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3R//Main//Moon//Countdown//limit.png");

            limit = Bitmap_To_Color(limit, System.Drawing.Color.FromArgb(246, 252, 156), calendar_area);

            // AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
            // AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA

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

            // Convert the full moon countdown value to a two-index char array.
            char[] countdown_array = full_moon_countdown.ToString("00").ToCharArray();

            // Check if the first index is not a zero. If it is, the countdown digit is a single number and we can ignore the tens place.
            // Else, we need to assign a proper value to the tens place bitmap variable.
            if (countdown_array[0] != '0')
            {
                countdown_tens = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3R//Main//Moon//Countdown//Digits//Tens_Place//{countdown_array[0]}.png");
            }
            // There will always be a digit in the ones place unless the moon is full, so assign a proper value here too.
            countdown_ones = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3R//Main//Moon//Countdown//Digits//Ones_Place//{countdown_array[1]}.png");

            System.Drawing.Color countdown_color = System.Drawing.Color.FromArgb(252, 252, 6);
            countdown_tens = Bitmap_To_Color(countdown_tens, countdown_color, calendar_area);
            countdown_ones = Bitmap_To_Color(countdown_ones, countdown_color, calendar_area);

            // Here is where the calculation on which moon phase to display begins.
            // The cycle begins with a new moon, so we'll use the current cycle's age and divide it into two halfs to determine whether it's waxing or waning.
            // Waxing phases
            if (cycle_age <= 14.76)
            {
                // New Moon
                if ((illumination >= 0) && (illumination < 6.25))
                {
                    limit = new Bitmap(template_width, template_height);
                    moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3R//Main//Moon//Phases//1_new.png");
                    countdown_text_special = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3R//Main//Moon//Countdown//Text//new.png");
                }
                // Waxing Crescent 1
                else if ((illumination >= 6.25) && (illumination < 12.5))
                {
                    moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3R//Main//Moon//Phases//2_waxing_crescent.png");
                }
                // Waxing Crescent 2
                else if ((illumination >= 12.5) && (illumination < 18.75))
                {
                    moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3R//Main//Moon//Phases//3_waxing_crescent.png");
                }
                // Waxing Crescent 3
                else if ((illumination >= 18.75) && (illumination < 25))
                {
                    moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3R//Main//Moon//Phases//4_waxing_crescent.png");
                }
                // Waxing Crescent 4
                else if ((illumination >= 25) && (illumination < 31.25))
                {
                    moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3R//Main//Moon//Phases//5_waxing_crescent.png");
                }
                // Waxing Crescent 5
                else if ((illumination >= 31.25) && (illumination < 37.5))
                {
                    moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3R//Main//Moon//Phases//6_waxing_crescent.png");
                }
                // Waxing Crescent 6
                else if ((illumination >= 37.5) && (illumination < 43.75))
                {
                    moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3R//Main//Moon//Phases//7_waxing_crescent.png");
                }
                // Half
                else if ((illumination >= 43.75) && (illumination < 50))
                {
                    limit = new Bitmap(template_width, template_height);
                    moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3R//Main//Moon//Phases//8_half.png");
                    countdown_text_special = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3R//Main//Moon//Countdown//Text//half.png");
                }
                // Half
                else if ((illumination >= 50) && (illumination < 56.25))
                {
                    limit = new Bitmap(template_width, template_height);
                    moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3R//Main//Moon//Phases//8_half.png");
                    countdown_text_special = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3R//Main//Moon//Countdown//Text//half.png");
                }
                // Waxing Gibbous 1
                else if ((illumination >= 56.25) && (illumination < 62.5))
                {
                    moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3R//Main//Moon//Phases//9_waxing_gibbous.png");
                }
                // Waxing Gibbous 2
                else if ((illumination >= 62.5) && (illumination < 68.75))
                {
                    moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3R//Main//Moon//Phases//10_waxing_gibbous.png");
                }
                // Waxing Gibbous 3
                else if ((illumination >= 68.75) && (illumination < 75))
                {
                    moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3R//Main//Moon//Phases//11_waxing_gibbous.png");
                }
                // Waxing Gibbous 4
                else if ((illumination >= 75) && (illumination < 81.25))
                {
                    moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3R//Main//Moon//Phases//12_waxing_gibbous.png");
                }
                // Waxing Gibbous 5
                else if ((illumination >= 81.25) && (illumination < 87.5))
                {
                    moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3R//Main//Moon//Phases//13_waxing_gibbous.png");
                }
                // Waxing Gibbous 6
                else if ((illumination >= 87.5) && (illumination < 93.75))
                {
                    moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3R//Main//Moon//Phases//14_waxing_gibbous.png");
                }
                // Waxing Gibbous 7
                else if ((illumination >= 93.75) && (illumination < 100))
                {
                    moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3R//Main//Moon//Phases//15_waxing_gibbous.png");
                }
                // Full Moon
                else if (illumination >= 100)
                {
                    limit = new Bitmap(template_width, template_height);
                    moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3R//Main//Moon//Phases//16_full.png");
                    countdown_text_special = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3R//Main//Moon//Countdown//Text//full.png");
                }
            }
            // Waning Phases
            else if (cycle_age > 14.76)
            {
                // Full moon
                if (illumination >= 100)
                {
                    limit = new Bitmap(template_width, template_height);
                    moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3R//Main//Moon//Phases//16_full.png");
                    countdown_text_special = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3R//Main//Moon//Countdown//Text//full.png");
                }
                // Waning Gibbous 7
                else if ((illumination >= 93.75) && (illumination < 100))
                {
                    moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3R//Main//Moon//Phases//17_waning_gibbous.png");
                }
                // Waning Gibbous 6
                else if ((illumination >= 87.5) && (illumination < 93.75))
                {
                    moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3R//Main//Moon//Phases//18_waning_gibbous.png");
                }
                // Waning Gibbous 5
                else if ((illumination >= 81.25) && (illumination < 87.5))
                {
                    moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3R//Main//Moon//Phases//19_waning_gibbous.png");
                }
                // Waning Gibbous 4
                else if ((illumination >= 75) && (illumination < 81.25))
                {
                    moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3R//Main//Moon//Phases//20_waning_gibbous.png");
                }
                // Waning Gibbous 3
                else if ((illumination >= 68.75) && (illumination < 75))
                {
                    moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3R//Main//Moon//Phases//21_waning_gibbous.png");
                }
                // Waning Gibbous 2
                else if ((illumination >= 62.5) && (illumination < 68.75))
                {
                    moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3R//Main//Moon//Phases//22_waning_gibbous.png");
                }
                // Waning Gibbous 1
                else if ((illumination >= 56.25) && (illumination < 62.5))
                {
                    moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3R//Main//Moon//Phases//23_waning_gibbous.png");
                }
                // Half
                else if ((illumination >= 50) && (illumination < 56.25))
                {
                    limit = new Bitmap(template_width, template_height);
                    moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3R//Main//Moon//Phases//24_half.png");
                    countdown_text_special = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3R//Main//Moon//Countdown//Text//half.png");
                }
                // Half
                else if ((illumination >= 43.75) && (illumination < 50))
                {
                    limit = new Bitmap(template_width, template_height);
                    moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3R//Main//Moon//Phases//24_half.png");
                    countdown_text_special = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3R//Main//Moon//Countdown//Text//half.png");
                }
                // Waxing Crescent 6
                else if ((illumination >= 37.5) && (illumination < 43.75))
                {
                    moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3R//Main//Moon//Phases//25_waning_crescent.png");
                }
                // Waxing Crescent 5
                else if ((illumination >= 31.25) && (illumination < 37.5))
                {
                    moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3R//Main//Moon//Phases//26_waning_crescent.png");
                }
                // Waxing Crescent 4
                else if ((illumination >= 25) && (illumination < 31.25))
                {
                    moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3R//Main//Moon//Phases//27_waning_crescent.png");
                }
                // Waxing Crescent 3
                else if ((illumination >= 18.75) && (illumination < 25))
                {
                    moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3R//Main//Moon//Phases//28_waning_crescent.png");
                }
                // Waxing Crescent 2
                else if ((illumination >= 12.5) && (illumination < 18.75))
                {
                    moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3R//Main//Moon//Phases//29_waning_crescent.png");
                }
                // Waxing Crescent 1
                else if ((illumination >= 6.25) && (illumination < 12.5))
                {
                    moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3R//Main//Moon//Phases//30_waning_crescent.png");
                }
                // New Moon
                if ((illumination >= 0) && (illumination < 6.25))
                {
                    limit = new Bitmap(template_width, template_height);
                    moon_phase = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3R//Main//Moon//Phases//1_new.png");
                    countdown_text_special = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3R//Main//Moon//Countdown//Text//new.png");
                }
            }

            countdown_text_special = Bitmap_To_Color(countdown_text_special, countdown_color, calendar_area);

            // Now, let's use a graphics object to draw to the base template.
            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                graphics.DrawImage(limit, 0, 0, template_width, template_height);
                graphics.DrawImage(countdown_text_special, 0, 0, template_width, template_height);
                graphics.DrawImage(countdown_tens, 0, 0, template_width, template_height);
                graphics.DrawImage(countdown_ones, 0, 0, template_width, template_height);
                graphics.DrawImage(moon_phase, 0, 0, template_width, template_height);
            }

            return base_template;
        }

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
                    System.Drawing.Color new_color = System.Drawing.Color.FromArgb(actual_color.A, 0, 30, 154);
                    new_bitmap.SetPixel(x, y, new_color);
                }
            }

            return new_bitmap;
        }

        //public static Bitmap Bitmap_To_Color(Bitmap input_bitmap, System.Drawing.Color input_color, Rectangle edit_area)
        //{
        //    Bitmap base_bitmap = new Bitmap(input_bitmap.Width, input_bitmap.Height);

        //    for (int x = edit_area.X; x < edit_area.Right; x++)
        //    {
        //        for (int y = edit_area.Y; y < edit_area.Bottom; y++)
        //        {
        //            System.Drawing.Color original_color = input_bitmap.GetPixel(x, y);
        //            System.Drawing.Color new_color = System.Drawing.Color.FromArgb(original_color.A, input_color.R, input_color.G, input_color.B);

        //            base_bitmap.SetPixel(x, y, new_color);
        //        }
        //    }

        //    return base_bitmap;
        //}

        public static Bitmap Partial_Bitmap_To_Color(Bitmap input_bitmap, System.Drawing.Color input_color, Rectangle edit_area, int alpha_threshold)
        {
            Bitmap base_bitmap = new Bitmap(input_bitmap.Width, input_bitmap.Height);

            for (int x = edit_area.X; x < edit_area.Right; x++)
            {
                for (int y = edit_area.Y; y < edit_area.Bottom; y++)
                {
                    System.Drawing.Color original_color = input_bitmap.GetPixel(x, y);

                    if (original_color.A >= alpha_threshold)
                    {
                        System.Drawing.Color new_color = System.Drawing.Color.FromArgb(original_color.A, input_color.R, input_color.G, input_color.B);
                        base_bitmap.SetPixel(x, y, new_color);
                    }
                    else
                    {
                        base_bitmap.SetPixel(x, y, original_color);
                    }
                }
            }

            return base_bitmap;
        }

        public Bitmap Scale_Template(UserInfoFields account, Bitmap input_template)
        {
            var scaled_bitmap = new Bitmap(2, 2);

            var copied_input = new Bitmap(input_template);
            scaled_bitmap = new Bitmap(template_width, template_height);

            using (Graphics graphics = Graphics.FromImage(scaled_bitmap))
            {
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.DrawImage(copied_input, 0, 0, template_width, template_height);
            }

            input_template = scaled_bitmap;

            return input_template;
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
                    json_result = new TimedWebClient { Timeout = Global.API_Timeout }.DownloadString($"http://api.weatherapi.com/v1/current.json?key={WeatherAPIConfig.weather_api_account.accountKey}&q={account.City}");
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
            // If it's a Sunday or outside of a school term, set it to Daytime.
            // If it's a weekday, set it to After School.
            else if (current_hour < evening && current_hour >= after_school)
            {
                if (DateTime.Now.ToString("ddd") == "Sun" || !OfficialSetMethods.Is_School_Term(input_time))
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
            // If it's a Sunday or outside of a school term, set it to Daytime.
            // If it's a weekday, set it to Lunchtime.
            else if (current_hour < afternoon && current_hour >= lunchtime)
            {
                if ((DateTime.Now.ToString("ddd") == "Sun") || !OfficialSetMethods.Is_School_Term(input_time))
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
                    json_result = new TimedWebClient { Timeout = Global.API_Timeout }.DownloadString($"http://api.weatherapi.com/v1/current.json?key={WeatherAPIConfig.weather_api_account.accountKey}&q={account.City}");
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

        public static Bitmap Bitmap_To_Color(Bitmap inputBitmap, System.Drawing.Color inputColor, Rectangle editArea)
        {
            int width = inputBitmap.Width;
            int height = inputBitmap.Height;

            Rectangle bounds = new Rectangle(0, 0, width, height);
            Rectangle clippedArea = Rectangle.Intersect(editArea, bounds);

            Bitmap source = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            using (Graphics g = Graphics.FromImage(source))
            {
                g.DrawImage(inputBitmap, 0, 0, width, height);
            }

            Bitmap output = new Bitmap(width, height, PixelFormat.Format32bppArgb);

            BitmapData srcData = source.LockBits(bounds, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData dstData = output.LockBits(bounds, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            try
            {
                int srcStride = srcData.Stride;
                int dstStride = dstData.Stride;
                int bytes = Math.Abs(srcStride) * height;

                byte[] srcBuffer = new byte[bytes];
                byte[] dstBuffer = new byte[bytes];

                Marshal.Copy(srcData.Scan0, srcBuffer, 0, bytes);

                for (int y = clippedArea.Top; y < clippedArea.Bottom; y++)
                {
                    int srcRow = y * srcStride;
                    int dstRow = y * dstStride;

                    for (int x = clippedArea.Left; x < clippedArea.Right; x++)
                    {
                        int i = srcRow + (x * 4);

                        byte a = srcBuffer[i + 3];

                        dstBuffer[dstRow + (x * 4) + 0] = inputColor.B;
                        dstBuffer[dstRow + (x * 4) + 1] = inputColor.G;
                        dstBuffer[dstRow + (x * 4) + 2] = inputColor.R;
                        dstBuffer[dstRow + (x * 4) + 3] = a;
                    }
                }

                Marshal.Copy(dstBuffer, 0, dstData.Scan0, bytes);
            }
            finally
            {
                source.UnlockBits(srcData);
                output.UnlockBits(dstData);
                source.Dispose();
            }

            return output;
        }

        public static EmbedBuilder P3R_Loading_Message()
        {
            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Generating Scene...",
                IconUrl = EmbedSettings.Get_Game_Logo("P3R")
            };

            embed.WithAuthor(author);
            embed.WithColor(EmbedSettings.Get_Game_Color("P3R", null));
            //embed.WithThumbnailUrl(EmbedSettings.Get_Loading_Icon("P3F", null));
            embed.WithDescription("This may take a few seconds!");

            return embed;
        }
    }

    public static class Persona3ReloadMessage
    {
        // Extracted from メッセージ下地 in PLG_UI_Message_00.json
        public static readonly PointF[] MessageBaseVertices = new PointF[]
        {
            new PointF(1056.536100f, -95.241240f),
            new PointF(1056.382100f, -97.262010f),
            new PointF(1055.940000f, -99.144150f),
            new PointF(1055.240500f, -100.847336f),
            new PointF(1054.314100f, -102.331280f),
            new PointF(1053.191200f, -103.555630f),
            new PointF(1051.902300f, -104.480090f),
            new PointF(1050.478300f, -105.064330f),
            new PointF(1048.949200f, -105.268036f),
            new PointF(1038.659400f, -106.059040f),
            new PointF(1009.246700f, -108.144380f),
            new PointF(962.896850f, -111.092636f),
            new PointF(901.795040f, -114.472350f),
            new PointF(828.126830f, -117.852066f),
            new PointF(744.077640f, -120.800320f),
            new PointF(651.833070f, -122.885666f),
            new PointF(553.578370f, -123.676670f),
            new PointF(453.325200f, -122.885666f),
            new PointF(356.198900f, -120.800310f),
            new PointF(265.355830f, -117.852066f),
            new PointF(183.952150f, -114.472350f),
            new PointF(115.144040f, -111.092636f),
            new PointF(62.087708f, -108.144380f),
            new PointF(27.939331f, -106.059020f),
            new PointF(15.855164f, -105.268036f),
            new PointF(14.326050f, -105.064330f),
            new PointF(12.901978f, -104.480090f),
            new PointF(11.613159f, -103.555630f),
            new PointF(10.490234f, -102.331300f),
            new PointF(9.563782f, -100.847350f),
            new PointF(8.864258f, -99.144165f),
            new PointF(8.422119f, -97.262010f),
            new PointF(8.268005f, -95.241240f),
            new PointF(7.977173f, -93.185210f),
            new PointF(7.210449f, -87.367950f),
            new PointF(6.126587f, -78.315674f),
            new PointF(4.884033f, -66.554660f),
            new PointF(3.641479f, -52.611115f),
            new PointF(2.557495f, -37.011322f),
            new PointF(1.790833f, -20.281494f),
            new PointF(1.500000f, -2.947846f),
            new PointF(1.790771f, 15.170319f),
            new PointF(2.557495f, 32.817383f),
            new PointF(3.641479f, 49.393950f),
            new PointF(4.884033f, 64.300630f),
            new PointF(6.126587f, 76.938080f),
            new PointF(7.210510f, 86.706940f),
            new PointF(7.977173f, 93.007780f),
            new PointF(8.268005f, 95.241240f),
            new PointF(8.422119f, 97.261990f),
            new PointF(8.864258f, 99.144104f),
            new PointF(9.563843f, 100.847320f),
            new PointF(10.490234f, 102.331240f),
            new PointF(11.613159f, 103.555600f),
            new PointF(12.901978f, 104.480070f),
            new PointF(14.326111f, 105.064330f),
            new PointF(15.855164f, 105.268036f),
            new PointF(27.939331f, 106.059050f),
            new PointF(62.087708f, 108.144380f),
            new PointF(115.144040f, 111.092620f),
            new PointF(183.952150f, 114.472350f),
            new PointF(265.355830f, 117.852080f),
            new PointF(356.198970f, 120.800320f),
            new PointF(453.325200f, 122.885650f),
            new PointF(553.578370f, 123.676670f),
            new PointF(651.833070f, 122.885680f),
            new PointF(744.077640f, 120.800320f),
            new PointF(828.126830f, 117.852080f),
            new PointF(901.795040f, 114.472350f),
            new PointF(962.896700f, 111.092650f),
            new PointF(1009.246700f, 108.144380f),
            new PointF(1038.659400f, 106.059050f),
            new PointF(1048.949200f, 105.268036f),
            new PointF(1050.478300f, 105.064330f),
            new PointF(1051.902300f, 104.480070f),
            new PointF(1053.191200f, 103.555600f),
            new PointF(1054.314000f, 102.331210f),
            new PointF(1055.240500f, 100.847320f),
            new PointF(1055.940000f, 99.144070f),
            new PointF(1056.382100f, 97.261990f),
            new PointF(1056.536100f, 95.241240f),
            new PointF(1056.822900f, 93.007750f),
            new PointF(1057.578900f, 86.706910f),
            new PointF(1058.647600f, 76.938080f),
            new PointF(1059.872800f, 64.300630f),
            new PointF(1061.097900f, 49.393950f),
            new PointF(1062.166700f, 32.817383f),
            new PointF(1062.922700f, 15.170319f),
            new PointF(1063.209500f, -2.947846f),
            new PointF(1062.922700f, -20.281494f),
            new PointF(1062.166700f, -37.011322f),
            new PointF(1061.097900f, -52.611115f),
            new PointF(1059.872800f, -66.554660f),
            new PointF(1058.647600f, -78.315674f),
            new PointF(1057.578900f, -87.367950f),
            new PointF(1056.822900f, -93.185210f),
            new PointF(1058.027600f, -95.401924f),
            new PointF(1057.864500f, -97.491220f),
            new PointF(1057.368400f, -99.602030f),
            new PointF(1056.575200f, -101.531845f),
            new PointF(1055.508000f, -103.239235f),
            new PointF(1054.186300f, -104.677980f),
            new PointF(1052.629200f, -105.792280f),
            new PointF(1050.865200f, -106.513570f),
            new PointF(1049.105800f, -106.759840f),
            new PointF(1038.770000f, -107.554960f),
            new PointF(1009.347350f, -109.641000f),
            new PointF(962.985840f, -112.589990f),
            new PointF(901.870850f, -115.970436f),
            new PointF(828.187500f, -119.350840f),
            new PointF(744.120850f, -122.299700f),
            new PointF(651.856100f, -124.385490f),
            new PointF(553.578500f, -125.176670f),
            new PointF(453.303160f, -124.385506f),
            new PointF(356.158500f, -122.299770f),
            new PointF(265.300380f, -119.351040f),
            new PointF(183.884250f, -115.970810f),
            new PointF(115.065640f, -112.590580f),
            new PointF(62.000366f, -109.641840f),
            new PointF(27.844635f, -107.556030f),
            new PointF(15.707031f, -106.760704f),
            new PointF(13.939148f, -106.513570f),
            new PointF(12.175232f, -105.792280f),
            new PointF(10.617981f, -104.677980f),
            new PointF(9.296265f, -103.239270f),
            new PointF(8.229065f, -101.531900f),
            new PointF(7.435852f, -99.602066f),
            new PointF(6.939758f, -97.491240f),
            new PointF(6.776794f, -95.403404f),
            new PointF(6.490967f, -93.388260f),
            new PointF(5.722168f, -87.555120f),
            new PointF(4.636047f, -78.483640f),
            new PointF(3.391113f, -66.700035f),
            new PointF(2.146179f, -52.729683f),
            new PointF(1.059998f, -37.097652f),
            new PointF(0.291565f, -20.328415f),
            new PointF(0.000000f, -2.948393f),
            new PointF(0.291443f, 15.214912f),
            new PointF(1.059692f, 32.898884f),
            new PointF(2.145630f, 49.505196f),
            new PointF(3.390198f, 64.436325f),
            new PointF(4.634705f, 77.094185f),
            new PointF(5.720581f, 86.880240f),
            new PointF(6.488953f, 93.195210f),
            new PointF(6.775940f, 95.395170f),
            new PointF(6.939758f, 97.491230f),
            new PointF(7.435852f, 99.602030f),
            new PointF(8.229187f, 101.531876f),
            new PointF(9.296265f, 103.239200f),
            new PointF(10.617981f, 104.677940f),
            new PointF(12.175232f, 105.792270f),
            new PointF(13.939209f, 106.513570f),
            new PointF(15.707031f, 106.760704f),
            new PointF(27.844635f, 107.556060f),
            new PointF(62.000366f, 109.641840f),
            new PointF(115.065640f, 112.590570f),
            new PointF(183.884250f, 115.970810f),
            new PointF(265.300380f, 119.351060f),
            new PointF(356.158570f, 122.299780f),
            new PointF(453.303160f, 124.385490f),
            new PointF(553.578500f, 125.176670f),
            new PointF(651.856100f, 124.385506f),
            new PointF(744.120850f, 122.299700f),
            new PointF(828.187500f, 119.350850f),
            new PointF(901.870850f, 115.970436f),
            new PointF(962.985700f, 112.590004f),
            new PointF(1009.347350f, 109.641000f),
            new PointF(1038.770000f, 107.554980f),
            new PointF(1049.105800f, 106.759840f),
            new PointF(1050.865200f, 106.513570f),
            new PointF(1052.629200f, 105.792260f),
            new PointF(1054.186300f, 104.677910f),
            new PointF(1055.507900f, 103.239170f),
            new PointF(1056.575200f, 101.531876f),
            new PointF(1057.368400f, 99.601950f),
            new PointF(1057.864500f, 97.491210f),
            new PointF(1058.028300f, 95.393810f),
            new PointF(1058.311500f, 93.192604f),
            new PointF(1059.069100f, 86.877820f),
            new PointF(1060.139600f, 77.092020f),
            new PointF(1061.366800f, 64.434440f),
            new PointF(1062.593900f, 49.503647f),
            new PointF(1063.664600f, 32.897747f),
            new PointF(1064.422100f, 15.214291f),
            new PointF(1064.709500f, -2.948383f),
            new PointF(1064.422000f, -20.327760f),
            new PointF(1063.664300f, -37.096450f),
            new PointF(1062.593300f, -52.728030f),
            new PointF(1061.366000f, -66.698010f),
            new PointF(1060.138400f, -78.481320f),
            new PointF(1059.067400f, -87.552540f),
            new PointF(1058.309400f, -93.385460f),
        };

        public static readonly int[] MessageBaseIndices = new int[]
        {
            0, 1, 8, 1, 2, 8, 2, 3, 8, 3, 4, 8,
            4, 5, 8, 5, 6, 8, 6, 7, 8, 0, 8, 9,
            24, 25, 26, 24, 26, 27, 24, 27, 28, 24, 28, 29,
            24, 29, 30, 24, 30, 31, 24, 31, 32, 23, 33, 34,
            22, 23, 35, 23, 34, 35, 22, 35, 36, 22, 36, 37,
            22, 37, 38, 22, 38, 39, 47, 48, 56, 48, 49, 56,
            49, 50, 56, 50, 51, 56, 51, 52, 56, 52, 53, 56,
            53, 54, 56, 54, 55, 56, 45, 46, 57, 46, 47, 57,
            47, 56, 57, 44, 45, 58, 45, 57, 58, 20, 21, 59,
            41, 42, 59, 42, 58, 59, 21, 40, 59, 40, 41, 59,
            19, 20, 60, 20, 59, 60, 18, 19, 61, 19, 60, 61,
            18, 61, 62, 16, 17, 63, 15, 16, 64, 16, 63, 64,
            15, 64, 65, 12, 13, 68, 13, 67, 68, 72, 73, 74,
            72, 74, 75, 72, 75, 76, 72, 76, 77, 72, 77, 78,
            72, 78, 79, 71, 72, 80, 72, 79, 80, 71, 80, 81,
            71, 81, 82, 70, 71, 83, 71, 82, 83, 70, 83, 84,
            70, 84, 85, 69, 70, 86, 70, 85, 86, 69, 86, 87,
            11, 69, 88, 69, 87, 88, 11, 88, 89, 10, 11, 90,
            11, 89, 90, 10, 90, 91, 10, 91, 92, 9, 10, 93,
            10, 92, 93, 9, 93, 94, 0, 9, 95, 9, 94, 95,
            23, 24, 32, 23, 32, 33, 21, 22, 39, 21, 39, 40,
            42, 43, 58, 43, 44, 58, 17, 18, 63, 18, 62, 63,
            13, 14, 66, 13, 66, 67, 14, 15, 66, 15, 65, 66,
            11, 12, 69, 12, 68, 69, 95, 96, 0, 0, 96, 1,
            1, 97, 2, 96, 97, 1, 2, 98, 3, 97, 98, 2,
            3, 99, 4, 98, 99, 3, 4, 100, 5, 99, 100, 4,
            5, 101, 6, 100, 101, 5, 6, 102, 7, 101, 102, 6,
            7, 103, 8, 102, 103, 7, 8, 104, 9, 103, 104, 8,
            9, 105, 10, 104, 105, 9, 23, 120, 24, 24, 120, 25,
            25, 121, 26, 120, 121, 25, 26, 122, 27, 121, 122, 26,
            27, 123, 28, 122, 123, 27, 28, 124, 29, 123, 124, 28,
            29, 125, 30, 124, 125, 29, 30, 126, 31, 125, 126, 30,
            31, 127, 32, 126, 127, 31, 32, 128, 33, 127, 128, 32,
            128, 129, 33, 33, 129, 34, 34, 130, 35, 129, 130, 34,
            21, 118, 22, 22, 118, 23, 23, 119, 120, 118, 119, 23,
            35, 131, 36, 130, 131, 35, 36, 132, 37, 131, 132, 36,
            37, 133, 38, 132, 133, 37, 38, 134, 39, 133, 134, 38,
            39, 135, 40, 134, 135, 39, 46, 143, 47, 47, 143, 48,
            48, 144, 49, 143, 144, 48, 49, 145, 50, 144, 145, 49,
            50, 146, 51, 145, 146, 50, 51, 147, 52, 146, 147, 51,
            52, 148, 53, 147, 148, 52, 53, 149, 54, 148, 149, 53,
            54, 150, 55, 149, 150, 54, 55, 151, 56, 150, 151, 55,
            56, 152, 57, 151, 152, 56, 44, 141, 45, 45, 141, 46,
            46, 142, 143, 141, 142, 46, 57, 153, 58, 152, 153, 57,
            43, 140, 44, 44, 140, 141, 58, 154, 59, 153, 154, 58,
            19, 116, 20, 20, 116, 21, 21, 117, 118, 116, 117, 21,
            40, 137, 41, 41, 137, 42, 42, 138, 43, 137, 138, 42,
            59, 155, 60, 154, 155, 59, 135, 136, 40, 40, 136, 137,
            18, 115, 19, 19, 115, 116, 60, 156, 61, 155, 156, 60,
            17, 114, 18, 18, 114, 115, 61, 157, 62, 156, 157, 61,
            62, 158, 63, 157, 158, 62, 15, 112, 16, 16, 112, 17,
            17, 113, 114, 112, 113, 17, 14, 111, 15, 15, 111, 112,
            158, 159, 63, 63, 159, 64, 64, 160, 65, 159, 160, 64,
            65, 161, 66, 160, 161, 65, 11, 108, 12, 12, 108, 13,
            13, 109, 14, 108, 109, 13, 66, 163, 67, 67, 163, 68,
            68, 164, 69, 163, 164, 68, 71, 168, 72, 72, 168, 73,
            73, 169, 74, 168, 169, 73, 74, 170, 75, 169, 170, 74,
            75, 171, 76, 170, 171, 75, 76, 172, 77, 171, 172, 76,
            77, 173, 78, 172, 173, 77, 78, 174, 79, 173, 174, 78,
            79, 175, 80, 174, 175, 79, 70, 167, 71, 71, 167, 168,
            80, 176, 81, 175, 176, 80, 81, 177, 82, 176, 177, 81,
            82, 178, 83, 177, 178, 82, 69, 166, 70, 70, 166, 167,
            83, 179, 84, 178, 179, 83, 84, 180, 85, 179, 180, 84,
            85, 181, 86, 180, 181, 85, 164, 165, 69, 69, 165, 166,
            86, 182, 87, 181, 182, 86, 87, 183, 88, 182, 183, 87,
            88, 184, 89, 183, 184, 88, 89, 185, 90, 184, 185, 89,
            105, 106, 10, 10, 106, 11, 11, 107, 108, 106, 107, 11,
            90, 186, 91, 185, 186, 90, 91, 187, 92, 186, 187, 91,
            92, 188, 93, 187, 188, 92, 93, 189, 94, 188, 189, 93,
            94, 190, 95, 189, 190, 94, 190, 191, 95, 95, 191, 96,
            43, 139, 140, 138, 139, 43, 14, 110, 111, 109, 110, 14,
            161, 162, 66, 66, 162, 163,
        };

        //public static void FillMessageBaseByTriangles(Graphics graphics, Brush brush, float offsetX = 0f, float offsetY = 130f)
        //{
        //    if (graphics == null) throw new ArgumentNullException(nameof(graphics));
        //    if (brush == null) throw new ArgumentNullException(nameof(brush));

        //    graphics.SmoothingMode = SmoothingMode.AntiAlias;

        //    for (int i = 0; i < MessageBaseIndices.Length; i += 3)
        //    {
        //        PointF[] triangle = new PointF[]
        //        {
        //        Translate(MessageBaseVertices[MessageBaseIndices[i]], offsetX, offsetY),
        //        Translate(MessageBaseVertices[MessageBaseIndices[i + 1]], offsetX, offsetY),
        //        Translate(MessageBaseVertices[MessageBaseIndices[i + 2]], offsetX, offsetY)
        //        };

        //        graphics.FillPolygon(brush, triangle);
        //    }
        //}

        public static void FillMessageBaseByTrianglesAsOnePath(
            Graphics graphics,
            Brush brush,
            float offsetX = 0f,
            float offsetY = 0f)
        {
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            graphics.CompositingQuality = CompositingQuality.HighQuality;

            using (GraphicsPath path = new GraphicsPath(FillMode.Winding))
            {
                for (int i = 0; i < MessageBaseIndices.Length; i += 3)
                {
                    PointF p1 = Translate(MessageBaseVertices[MessageBaseIndices[i]], offsetX, offsetY);
                    PointF p2 = Translate(MessageBaseVertices[MessageBaseIndices[i + 1]], offsetX, offsetY);
                    PointF p3 = Translate(MessageBaseVertices[MessageBaseIndices[i + 2]], offsetX, offsetY);

                    path.AddPolygon(new PointF[] { p1, p2, p3 });
                }

                graphics.FillPath(brush, path);
            }
        }

        public static Bitmap RenderMessageBaseByTriangles(int bitmapWidth = 1100, int bitmapHeight = 300, float offsetX = 10f, float offsetY = 145f)
        {
            Bitmap bitmap = new Bitmap(bitmapWidth, bitmapHeight);
            using (Graphics graphics = Graphics.FromImage(bitmap))
            using (SolidBrush brush = new SolidBrush(System.Drawing.Color.Black))
            {
                graphics.Clear(System.Drawing.Color.Transparent);
                FillMessageBaseByTrianglesAsOnePath(graphics, brush, offsetX, offsetY);
            }
            return bitmap;
        }

        private static PointF Translate(PointF point, float offsetX, float offsetY)
        {
            return new PointF(point.X + offsetX, point.Y + offsetY);
        }
    }

    public static class Persona3ReloadMessageBaseResizable
    {
        public readonly struct PlgBounds
        {
            public float MinX { get; }
            public float MinY { get; }
            public float MaxX { get; }
            public float MaxY { get; }

            public float Width => MaxX - MinX;
            public float Height => MaxY - MinY;

            public PlgBounds(float minX, float minY, float maxX, float maxY)
            {
                MinX = minX;
                MinY = minY;
                MaxX = maxX;
                MaxY = maxY;
            }
        }

        public static readonly PlgBounds SourceBounds =
            new PlgBounds(0.0f, -125.17667f, 1064.7095f, 125.17667f);

        public static readonly PointF[] MessageBaseVertices =
        {
            new PointF(1056.5361f, -95.24124f),
            new PointF(1056.3821f, -97.26201f),
            new PointF(1055.94f, -99.14415f),
            new PointF(1055.2405f, -100.847336f),
            new PointF(1054.3141f, -102.33128f),
            new PointF(1053.1912f, -103.55563f),
            new PointF(1051.9023f, -104.48009f),
            new PointF(1050.4783f, -105.06433f),
            new PointF(1048.9492f, -105.268036f),
            new PointF(1038.6594f, -106.05904f),
            new PointF(1009.2467f, -108.14438f),
            new PointF(962.89685f, -111.092636f),
            new PointF(901.79504f, -114.47235f),
            new PointF(828.12683f, -117.852066f),
            new PointF(744.07764f, -120.80032f),
            new PointF(651.83307f, -122.885666f),
            new PointF(553.57837f, -123.67667f),
            new PointF(453.3252f, -122.885666f),
            new PointF(356.1989f, -120.80031f),
            new PointF(265.35583f, -117.852066f),
            new PointF(183.95215f, -114.47235f),
            new PointF(115.14404f, -111.092636f),
            new PointF(62.087708f, -108.14438f),
            new PointF(27.939331f, -106.05902f),
            new PointF(15.855164f, -105.268036f),
            new PointF(14.32605f, -105.06433f),
            new PointF(12.901978f, -104.48009f),
            new PointF(11.613159f, -103.55563f),
            new PointF(10.490234f, -102.3313f),
            new PointF(9.563782f, -100.84735f),
            new PointF(8.864258f, -99.144165f),
            new PointF(8.422119f, -97.26201f),
            new PointF(8.268005f, -95.24124f),
            new PointF(7.977173f, -93.18521f),
            new PointF(7.210449f, -87.36795f),
            new PointF(6.126587f, -78.315674f),
            new PointF(4.884033f, -66.55466f),
            new PointF(3.641479f, -52.611115f),
            new PointF(2.557495f, -37.011322f),
            new PointF(1.790833f, -20.281494f),
            new PointF(1.5f, -2.947846f),
            new PointF(1.790771f, 15.170319f),
            new PointF(2.557495f, 32.817383f),
            new PointF(3.641479f, 49.39395f),
            new PointF(4.884033f, 64.30063f),
            new PointF(6.126587f, 76.93808f),
            new PointF(7.21051f, 86.70694f),
            new PointF(7.977173f, 93.00778f),
            new PointF(8.268005f, 95.24124f),
            new PointF(8.422119f, 97.26199f),
            new PointF(8.864258f, 99.144104f),
            new PointF(9.563843f, 100.84732f),
            new PointF(10.490234f, 102.33124f),
            new PointF(11.613159f, 103.5556f),
            new PointF(12.901978f, 104.48007f),
            new PointF(14.326111f, 105.06433f),
            new PointF(15.855164f, 105.268036f),
            new PointF(27.939331f, 106.05905f),
            new PointF(62.087708f, 108.14438f),
            new PointF(115.14404f, 111.09262f),
            new PointF(183.95215f, 114.47235f),
            new PointF(265.35583f, 117.85208f),
            new PointF(356.19897f, 120.80032f),
            new PointF(453.3252f, 122.88565f),
            new PointF(553.57837f, 123.67667f),
            new PointF(651.83307f, 122.88568f),
            new PointF(744.07764f, 120.80032f),
            new PointF(828.12683f, 117.85208f),
            new PointF(901.79504f, 114.47235f),
            new PointF(962.8967f, 111.09265f),
            new PointF(1009.2467f, 108.14438f),
            new PointF(1038.6594f, 106.05905f),
            new PointF(1048.9492f, 105.268036f),
            new PointF(1050.4783f, 105.06433f),
            new PointF(1051.9023f, 104.48007f),
            new PointF(1053.1912f, 103.5556f),
            new PointF(1054.314f, 102.33121f),
            new PointF(1055.2405f, 100.84732f),
            new PointF(1055.94f, 99.14407f),
            new PointF(1056.3821f, 97.26199f),
            new PointF(1056.5361f, 95.24124f),
            new PointF(1056.8229f, 93.00775f),
            new PointF(1057.5789f, 86.70691f),
            new PointF(1058.6476f, 76.93808f),
            new PointF(1059.8728f, 64.30063f),
            new PointF(1061.0979f, 49.39395f),
            new PointF(1062.1667f, 32.817383f),
            new PointF(1062.9227f, 15.170319f),
            new PointF(1063.2095f, -2.947846f),
            new PointF(1062.9227f, -20.281494f),
            new PointF(1062.1667f, -37.011322f),
            new PointF(1061.0979f, -52.611115f),
            new PointF(1059.8728f, -66.55466f),
            new PointF(1058.6476f, -78.315674f),
            new PointF(1057.5789f, -87.36795f),
            new PointF(1056.8229f, -93.18521f),
            new PointF(1058.0276f, -95.401924f),
            new PointF(1057.8645f, -97.49122f),
            new PointF(1057.3684f, -99.60203f),
            new PointF(1056.5752f, -101.531845f),
            new PointF(1055.508f, -103.239235f),
            new PointF(1054.1863f, -104.67798f),
            new PointF(1052.6292f, -105.79228f),
            new PointF(1050.8652f, -106.51357f),
            new PointF(1049.1058f, -106.75984f),
            new PointF(1038.77f, -107.55496f),
            new PointF(1009.34735f, -109.641f),
            new PointF(962.98584f, -112.58999f),
            new PointF(901.87085f, -115.970436f),
            new PointF(828.1875f, -119.35084f),
            new PointF(744.12085f, -122.2997f),
            new PointF(651.8561f, -124.38549f),
            new PointF(553.5785f, -125.17667f),
            new PointF(453.30316f, -124.385506f),
            new PointF(356.1585f, -122.29977f),
            new PointF(265.30038f, -119.35104f),
            new PointF(183.88425f, -115.97081f),
            new PointF(115.06564f, -112.59058f),
            new PointF(62.000366f, -109.64184f),
            new PointF(27.844635f, -107.55603f),
            new PointF(15.707031f, -106.760704f),
            new PointF(13.939148f, -106.51357f),
            new PointF(12.175232f, -105.79228f),
            new PointF(10.617981f, -104.67798f),
            new PointF(9.296265f, -103.23927f),
            new PointF(8.229065f, -101.5319f),
            new PointF(7.435852f, -99.602066f),
            new PointF(6.939758f, -97.49124f),
            new PointF(6.776794f, -95.403404f),
            new PointF(6.490967f, -93.38826f),
            new PointF(5.722168f, -87.55512f),
            new PointF(4.636047f, -78.48364f),
            new PointF(3.391113f, -66.700035f),
            new PointF(2.146179f, -52.729683f),
            new PointF(1.059998f, -37.097652f),
            new PointF(0.291565f, -20.328415f),
            new PointF(0.0f, -2.948393f),
            new PointF(0.291443f, 15.214912f),
            new PointF(1.059692f, 32.898884f),
            new PointF(2.14563f, 49.505196f),
            new PointF(3.390198f, 64.436325f),
            new PointF(4.634705f, 77.094185f),
            new PointF(5.720581f, 86.88024f),
            new PointF(6.488953f, 93.19521f),
            new PointF(6.77594f, 95.39517f),
            new PointF(6.939758f, 97.49123f),
            new PointF(7.435852f, 99.60203f),
            new PointF(8.229187f, 101.531876f),
            new PointF(9.296265f, 103.2392f),
            new PointF(10.617981f, 104.67794f),
            new PointF(12.175232f, 105.79227f),
            new PointF(13.939209f, 106.51357f),
            new PointF(15.707031f, 106.760704f),
            new PointF(27.844635f, 107.55606f),
            new PointF(62.000366f, 109.64184f),
            new PointF(115.06564f, 112.59057f),
            new PointF(183.88425f, 115.97081f),
            new PointF(265.30038f, 119.35106f),
            new PointF(356.15857f, 122.29978f),
            new PointF(453.30316f, 124.38549f),
            new PointF(553.5785f, 125.17667f),
            new PointF(651.8561f, 124.385506f),
            new PointF(744.12085f, 122.2997f),
            new PointF(828.1875f, 119.35085f),
            new PointF(901.87085f, 115.970436f),
            new PointF(962.9857f, 112.590004f),
            new PointF(1009.34735f, 109.641f),
            new PointF(1038.77f, 107.55498f),
            new PointF(1049.1058f, 106.75984f),
            new PointF(1050.8652f, 106.51357f),
            new PointF(1052.6292f, 105.79226f),
            new PointF(1054.1863f, 104.67791f),
            new PointF(1055.5079f, 103.23917f),
            new PointF(1056.5752f, 101.531876f),
            new PointF(1057.3684f, 99.60195f),
            new PointF(1057.8645f, 97.49121f),
            new PointF(1058.0283f, 95.39381f),
            new PointF(1058.3115f, 93.192604f),
            new PointF(1059.0691f, 86.87782f),
            new PointF(1060.1396f, 77.09202f),
            new PointF(1061.3668f, 64.43444f),
            new PointF(1062.5939f, 49.503647f),
            new PointF(1063.6646f, 32.897747f),
            new PointF(1064.4221f, 15.214291f),
            new PointF(1064.7095f, -2.948383f),
            new PointF(1064.422f, -20.32776f),
            new PointF(1063.6643f, -37.09645f),
            new PointF(1062.5933f, -52.72803f),
            new PointF(1061.366f, -66.69801f),
            new PointF(1060.1384f, -78.48132f),
            new PointF(1059.0674f, -87.55254f),
            new PointF(1058.3094f, -93.38546f),
        };

        public static readonly int[] MessageBaseIndices =
        {
            0,
            1,
            8,
            1,
            2,
            8,
            2,
            3,
            8,
            3,
            4,
            8,
            4,
            5,
            8,
            5,
            6,
            8,
            6,
            7,
            8,
            0,
            8,
            9,
            24,
            25,
            26,
            24,
            26,
            27,
            24,
            27,
            28,
            24,
            28,
            29,
            24,
            29,
            30,
            24,
            30,
            31,
            24,
            31,
            32,
            23,
            33,
            34,
            22,
            23,
            35,
            23,
            34,
            35,
            22,
            35,
            36,
            22,
            36,
            37,
            22,
            37,
            38,
            22,
            38,
            39,
            47,
            48,
            56,
            48,
            49,
            56,
            49,
            50,
            56,
            50,
            51,
            56,
            51,
            52,
            56,
            52,
            53,
            56,
            53,
            54,
            56,
            54,
            55,
            56,
            45,
            46,
            57,
            46,
            47,
            57,
            47,
            56,
            57,
            44,
            45,
            58,
            45,
            57,
            58,
            20,
            21,
            59,
            41,
            42,
            59,
            42,
            58,
            59,
            21,
            40,
            59,
            40,
            41,
            59,
            19,
            20,
            60,
            20,
            59,
            60,
            18,
            19,
            61,
            19,
            60,
            61,
            18,
            61,
            62,
            16,
            17,
            63,
            15,
            16,
            64,
            16,
            63,
            64,
            15,
            64,
            65,
            12,
            13,
            68,
            13,
            67,
            68,
            72,
            73,
            74,
            72,
            74,
            75,
            72,
            75,
            76,
            72,
            76,
            77,
            72,
            77,
            78,
            72,
            78,
            79,
            71,
            72,
            80,
            72,
            79,
            80,
            71,
            80,
            81,
            71,
            81,
            82,
            70,
            71,
            83,
            71,
            82,
            83,
            70,
            83,
            84,
            70,
            84,
            85,
            69,
            70,
            86,
            70,
            85,
            86,
            69,
            86,
            87,
            11,
            69,
            88,
            69,
            87,
            88,
            11,
            88,
            89,
            10,
            11,
            90,
            11,
            89,
            90,
            10,
            90,
            91,
            10,
            91,
            92,
            9,
            10,
            93,
            10,
            92,
            93,
            9,
            93,
            94,
            0,
            9,
            95,
            9,
            94,
            95,
            23,
            24,
            32,
            23,
            32,
            33,
            21,
            22,
            39,
            21,
            39,
            40,
            42,
            43,
            58,
            43,
            44,
            58,
            17,
            18,
            63,
            18,
            62,
            63,
            13,
            14,
            66,
            13,
            66,
            67,
            14,
            15,
            66,
            15,
            65,
            66,
            11,
            12,
            69,
            12,
            68,
            69,
            95,
            96,
            0,
            0,
            96,
            1,
            1,
            97,
            2,
            96,
            97,
            1,
            2,
            98,
            3,
            97,
            98,
            2,
            3,
            99,
            4,
            98,
            99,
            3,
            4,
            100,
            5,
            99,
            100,
            4,
            5,
            101,
            6,
            100,
            101,
            5,
            6,
            102,
            7,
            101,
            102,
            6,
            7,
            103,
            8,
            102,
            103,
            7,
            8,
            104,
            9,
            103,
            104,
            8,
            9,
            105,
            10,
            104,
            105,
            9,
            23,
            120,
            24,
            24,
            120,
            25,
            25,
            121,
            26,
            120,
            121,
            25,
            26,
            122,
            27,
            121,
            122,
            26,
            27,
            123,
            28,
            122,
            123,
            27,
            28,
            124,
            29,
            123,
            124,
            28,
            29,
            125,
            30,
            124,
            125,
            29,
            30,
            126,
            31,
            125,
            126,
            30,
            31,
            127,
            32,
            126,
            127,
            31,
            32,
            128,
            33,
            127,
            128,
            32,
            128,
            129,
            33,
            33,
            129,
            34,
            34,
            130,
            35,
            129,
            130,
            34,
            21,
            118,
            22,
            22,
            118,
            23,
            23,
            119,
            120,
            118,
            119,
            23,
            35,
            131,
            36,
            130,
            131,
            35,
            36,
            132,
            37,
            131,
            132,
            36,
            37,
            133,
            38,
            132,
            133,
            37,
            38,
            134,
            39,
            133,
            134,
            38,
            39,
            135,
            40,
            134,
            135,
            39,
            46,
            143,
            47,
            47,
            143,
            48,
            48,
            144,
            49,
            143,
            144,
            48,
            49,
            145,
            50,
            144,
            145,
            49,
            50,
            146,
            51,
            145,
            146,
            50,
            51,
            147,
            52,
            146,
            147,
            51,
            52,
            148,
            53,
            147,
            148,
            52,
            53,
            149,
            54,
            148,
            149,
            53,
            54,
            150,
            55,
            149,
            150,
            54,
            55,
            151,
            56,
            150,
            151,
            55,
            56,
            152,
            57,
            151,
            152,
            56,
            44,
            141,
            45,
            45,
            141,
            46,
            46,
            142,
            143,
            141,
            142,
            46,
            57,
            153,
            58,
            152,
            153,
            57,
            43,
            140,
            44,
            44,
            140,
            141,
            58,
            154,
            59,
            153,
            154,
            58,
            19,
            116,
            20,
            20,
            116,
            21,
            21,
            117,
            118,
            116,
            117,
            21,
            40,
            137,
            41,
            41,
            137,
            42,
            42,
            138,
            43,
            137,
            138,
            42,
            59,
            155,
            60,
            154,
            155,
            59,
            135,
            136,
            40,
            40,
            136,
            137,
            18,
            115,
            19,
            19,
            115,
            116,
            60,
            156,
            61,
            155,
            156,
            60,
            17,
            114,
            18,
            18,
            114,
            115,
            61,
            157,
            62,
            156,
            157,
            61,
            62,
            158,
            63,
            157,
            158,
            62,
            15,
            112,
            16,
            16,
            112,
            17,
            17,
            113,
            114,
            112,
            113,
            17,
            14,
            111,
            15,
            15,
            111,
            112,
            158,
            159,
            63,
            63,
            159,
            64,
            64,
            160,
            65,
            159,
            160,
            64,
            65,
            161,
            66,
            160,
            161,
            65,
            11,
            108,
            12,
            12,
            108,
            13,
            13,
            109,
            14,
            108,
            109,
            13,
            66,
            163,
            67,
            67,
            163,
            68,
            68,
            164,
            69,
            163,
            164,
            68,
            71,
            168,
            72,
            72,
            168,
            73,
            73,
            169,
            74,
            168,
            169,
            73,
            74,
            170,
            75,
            169,
            170,
            74,
            75,
            171,
            76,
            170,
            171,
            75,
            76,
            172,
            77,
            171,
            172,
            76,
            77,
            173,
            78,
            172,
            173,
            77,
            78,
            174,
            79,
            173,
            174,
            78,
            79,
            175,
            80,
            174,
            175,
            79,
            70,
            167,
            71,
            71,
            167,
            168,
            80,
            176,
            81,
            175,
            176,
            80,
            81,
            177,
            82,
            176,
            177,
            81,
            82,
            178,
            83,
            177,
            178,
            82,
            69,
            166,
            70,
            70,
            166,
            167,
            83,
            179,
            84,
            178,
            179,
            83,
            84,
            180,
            85,
            179,
            180,
            84,
            85,
            181,
            86,
            180,
            181,
            85,
            164,
            165,
            69,
            69,
            165,
            166,
            86,
            182,
            87,
            181,
            182,
            86,
            87,
            183,
            88,
            182,
            183,
            87,
            88,
            184,
            89,
            183,
            184,
            88,
            89,
            185,
            90,
            184,
            185,
            89,
            105,
            106,
            10,
            10,
            106,
            11,
            11,
            107,
            108,
            106,
            107,
            11,
            90,
            186,
            91,
            185,
            186,
            90,
            91,
            187,
            92,
            186,
            187,
            91,
            92,
            188,
            93,
            187,
            188,
            92,
            93,
            189,
            94,
            188,
            189,
            93,
            94,
            190,
            95,
            189,
            190,
            94,
            190,
            191,
            95,
            95,
            191,
            96,
            43,
            139,
            140,
            138,
            139,
            43,
            14,
            110,
            111,
            109,
            110,
            14,
            161,
            162,
            66,
            66,
            162,
            163
        };

        public static PointF RemapPointToBounds(PointF point, PlgBounds source, PlgBounds target)
        {
            if (source.Width == 0f)
                throw new InvalidOperationException("Source width cannot be zero.");

            if (source.Height == 0f)
                throw new InvalidOperationException("Source height cannot be zero.");

            float normalizedX = (point.X - source.MinX) / source.Width;
            float normalizedY = (point.Y - source.MinY) / source.Height;

            return new PointF(
                target.MinX + normalizedX * target.Width,
                target.MinY + normalizedY * target.Height);
        }

        public static PointF[] GetScaledVertices(PlgBounds targetBounds)
        {
            PointF[] scaled = new PointF[MessageBaseVertices.Length];

            for (int i = 0; i < MessageBaseVertices.Length; i++)
            {
                scaled[i] = RemapPointToBounds(MessageBaseVertices[i], SourceBounds, targetBounds);
            }

            return scaled;
        }

        //public static PointF[] GetScaledVertices(float minX, float minY, float maxX, float maxY)
        //{
        //    return GetScaledVertices(new PlgBounds(minX, minY, maxX, maxY));
        //}

        //public static PointF[] GetScaledVerticesBySize(float x, float y, float width, float height)
        //{
        //    return GetScaledVertices(new PlgBounds(x, y, x + width, y + height));
        //}

        public static GraphicsPath BuildScaledPath(PlgBounds targetBounds)
        {
            PointF[] scaledVertices = GetScaledVertices(targetBounds);
            GraphicsPath path = new GraphicsPath(FillMode.Winding);

            for (int i = 0; i < MessageBaseIndices.Length; i += 3)
            {
                path.AddPolygon(new[]
                {
                scaledVertices[MessageBaseIndices[i]],
                scaledVertices[MessageBaseIndices[i + 1]],
                scaledVertices[MessageBaseIndices[i + 2]]
            });
            }

            return path;
        }

        //public static GraphicsPath BuildScaledPath(float minX, float minY, float maxX, float maxY)
        //{
        //    return BuildScaledPath(new PlgBounds(minX, minY, maxX, maxY));
        //}

        //public static GraphicsPath BuildScaledPathBySize(float x, float y, float width, float height)
        //{
        //    return BuildScaledPath(new PlgBounds(x, y, x + width, y + height));
        //}

        public static void FillMessageBase(
            Graphics graphics,
            Brush brush,
            PlgBounds targetBounds,
            SmoothingMode smoothingMode = SmoothingMode.AntiAlias)
        {
            graphics.SmoothingMode = smoothingMode;

            using (GraphicsPath path = BuildScaledPath(targetBounds))
            {
                graphics.FillPath(brush, path);
            }
        }

        //public static void FillMessageBase(
        //    Graphics graphics,
        //    Brush brush,
        //    float minX,
        //    float minY,
        //    float maxX,
        //    float maxY,
        //    SmoothingMode smoothingMode = SmoothingMode.AntiAlias)
        //{
        //    FillMessageBase(
        //        graphics,
        //        brush,
        //        new PlgBounds(minX, minY, maxX, maxY),
        //        smoothingMode);
        //}

        public static void FillMessageBaseBySize(
            Graphics graphics,
            Brush brush,
            float x,
            float y,
            float width,
            float height,
            SmoothingMode smoothingMode = SmoothingMode.AntiAlias)
        {
            FillMessageBase(
                graphics,
                brush,
                new PlgBounds(x, y, x + width, y + height),
                smoothingMode);
        }

        //public static Bitmap RenderMessageBase(
        //    int bitmapWidth,
        //    int bitmapHeight,
        //    System.Drawing.Color fillColor,
        //    PlgBounds targetBounds,
        //    System.Drawing.Color? backgroundColor = null,
        //    SmoothingMode smoothingMode = SmoothingMode.AntiAlias)
        //{
        //    Bitmap bitmap = new Bitmap(bitmapWidth, bitmapHeight);

        //    using (Graphics graphics = Graphics.FromImage(bitmap))
        //    using (SolidBrush fillBrush = new SolidBrush(fillColor))
        //    {
        //        graphics.SmoothingMode = smoothingMode;

        //        if (backgroundColor.HasValue)
        //        {
        //            graphics.Clear(backgroundColor.Value);
        //        }

        //        FillMessageBase(graphics, fillBrush, targetBounds, smoothingMode);
        //    }

        //    return bitmap;
        //}

        //public static Bitmap RenderMessageBaseBySize(
        //    int bitmapWidth,
        //    int bitmapHeight,
        //    System.Drawing.Color fillColor,
        //    float x,
        //    float y,
        //    float width,
        //    float height,
        //    System.Drawing.Color? backgroundColor = null,
        //    SmoothingMode smoothingMode = SmoothingMode.AntiAlias)
        //{
        //    return RenderMessageBase(
        //        bitmapWidth,
        //        bitmapHeight,
        //        fillColor,
        //        new PlgBounds(x, y, x + width, y + height),
        //        backgroundColor,
        //        smoothingMode);
        //}
    }

    public static class Persona3ReloadMessageInnerResizable
    {
        public readonly struct PlgBounds
        {
            public float MinX { get; }
            public float MinY { get; }
            public float MaxX { get; }
            public float MaxY { get; }

            public float Width => MaxX - MinX;
            public float Height => MaxY - MinY;

            public PlgBounds(float minX, float minY, float maxX, float maxY)
            {
                MinX = minX;
                MinY = minY;
                MaxX = maxX;
                MaxY = maxY;
            }
        }

        public static readonly PlgBounds SourceBounds =
            new PlgBounds(0.0f, -121.47421f, 1056.516f, 121.47421f);

        public static readonly PointF[] MessageInnerVertices =
        {
            new PointF(1048.3942f, -94.389984f),
            new PointF(1048.0836f, -96.21179f),
            new PointF(1047.7548f, -97.7713f),
            new PointF(1047.3622f, -99.07739f),
            new PointF(1046.8611f, -100.13895f),
            new PointF(1046.2059f, -100.96478f),
            new PointF(1045.3517f, -101.56378f),
            new PointF(1044.2534f, -101.944824f),
            new PointF(1042.8657f, -102.11673f),
            new PointF(1032.5693f, -102.88405f),
            new PointF(1003.1571f, -104.90695f),
            new PointF(956.8446f, -107.76694f),
            new PointF(895.84717f, -111.04547f),
            new PointF(822.38025f, -114.324005f),
            new PointF(738.65955f, -117.18399f),
            new PointF(646.90027f, -119.206894f),
            new PointF(549.31793f, -119.97421f),
            new PointF(449.7525f, -119.16394f),
            new PointF(353.14923f, -117.027725f),
            new PointF(262.6869f, -114.0076f),
            new PointF(181.54419f, -110.54547f),
            new PointF(112.89987f, -107.08336f),
            new PointF(59.93268f, -104.06322f),
            new PointF(25.82129f, -101.92702f),
            new PointF(13.744446f, -101.11673f),
            new PointF(12.206238f, -100.92296f),
            new PointF(10.983582f, -100.640854f),
            new PointF(10.039795f, -100.2222f),
            new PointF(9.338074f, -99.61868f),
            new PointF(8.841614f, -98.78198f),
            new PointF(8.513611f, -97.66382f),
            new PointF(8.317322f, -96.21591f),
            new PointF(8.21582f, -94.389984f),
            new PointF(7.927307f, -92.30957f),
            new PointF(7.166443f, -86.439896f),
            new PointF(6.0908813f, -77.33835f),
            new PointF(4.85791f, -65.562225f),
            new PointF(3.624939f, -51.66896f),
            new PointF(2.5493164f, -36.21585f),
            new PointF(1.7885742f, -19.760284f),
            new PointF(1.5f, -2.8596497f),
            new PointF(1.7886353f, 14.8020935f),
            new PointF(2.5493774f, 32.1474f),
            new PointF(3.624939f, 48.548035f),
            new PointF(4.85791f, 63.3757f),
            new PointF(6.0908813f, 76.002045f),
            new PointF(7.166443f, 85.79874f),
            new PointF(7.927246f, 92.13754f),
            new PointF(8.21582f, 94.39011f),
            new PointF(8.5112915f, 96.29703f),
            new PointF(8.830627f, 97.90091f),
            new PointF(9.218445f, 99.21881f),
            new PointF(9.719299f, 100.26785f),
            new PointF(10.377747f, 101.065094f),
            new PointF(11.238403f, 101.627594f),
            new PointF(12.345764f, 101.97244f),
            new PointF(13.744446f, 102.11673f),
            new PointF(25.821259f, 102.88403f),
            new PointF(59.93268f, 104.90695f),
            new PointF(112.89987f, 107.76697f),
            new PointF(181.54419f, 111.04547f),
            new PointF(262.6869f, 114.323975f),
            new PointF(353.14917f, 117.18399f),
            new PointF(449.7525f, 119.20691f),
            new PointF(549.31793f, 119.97421f),
            new PointF(646.85736f, 119.20688f),
            new PointF(738.5033f, 117.18399f),
            new PointF(822.06384f, 114.323975f),
            new PointF(895.34717f, 111.04544f),
            new PointF(956.161f, 107.76694f),
            new PointF(1002.31335f, 104.90695f),
            new PointF(1031.6123f, 102.88403f),
            new PointF(1041.8657f, 102.11673f),
            new PointF(1043.4357f, 101.90076f),
            new PointF(1044.7532f, 101.52176f),
            new PointF(1045.8368f, 100.954285f),
            new PointF(1046.7051f, 100.172455f),
            new PointF(1047.3765f, 99.15085f),
            new PointF(1047.8694f, 97.86362f),
            new PointF(1048.2025f, 96.28528f),
            new PointF(1048.3942f, 94.39011f),
            new PointF(1048.6787f, 92.13757f),
            new PointF(1049.4288f, 85.79874f),
            new PointF(1050.4893f, 76.002014f),
            new PointF(1051.705f, 63.37567f),
            new PointF(1052.9208f, 48.548065f),
            new PointF(1053.9813f, 32.1474f),
            new PointF(1054.7314f, 14.802063f),
            new PointF(1055.016f, -2.8596497f),
            new PointF(1054.7314f, -19.760284f),
            new PointF(1053.9813f, -36.21585f),
            new PointF(1052.9208f, -51.668945f),
            new PointF(1051.7051f, -65.562225f),
            new PointF(1050.4894f, -77.33832f),
            new PointF(1049.4288f, -86.43991f),
            new PointF(1048.6787f, -92.30957f),
            new PointF(1049.8767f, -94.617676f),
            new PointF(1049.5571f, -96.49262f),
            new PointF(1049.2083f, -98.14227f),
            new PointF(1048.7626f, -99.61493f),
            new PointF(1048.1353f, -100.930435f),
            new PointF(1047.2349f, -102.05633f),
            new PointF(1046.0347f, -102.89927f),
            new PointF(1044.5933f, -103.4058f),
            new PointF(1043.0137f, -103.60941f),
            new PointF(1032.6765f, -104.38021f),
            new PointF(1003.25476f, -106.40377f),
            new PointF(956.9311f, -109.26444f),
            new PointF(895.9209f, -112.54366f),
            new PointF(822.43933f, -115.822845f),
            new PointF(738.70166f, -118.683395f),
            new PointF(646.9227f, -120.706726f),
            new PointF(549.31775f, -121.47421f),
            new PointF(449.72983f, -120.66377f),
            new PointF(353.1076f, -118.527145f),
            new PointF(262.62988f, -115.506516f),
            new PointF(181.47443f, -112.043846f),
            new PointF(112.8194f, -108.5812f),
            new PointF(59.84311f, -105.56054f),
            new PointF(25.724213f, -103.423874f),
            new PointF(13.600464f, -102.6098f),
            new PointF(11.9435425f, -102.39978f),
            new PointF(10.508667f, -102.0637f),
            new PointF(9.238037f, -101.48996f),
            new PointF(8.188904f, -100.58272f),
            new PointF(7.4661865f, -99.380486f),
            new PointF(7.04657f, -97.97657f),
            new PointF(6.8240967f, -96.358406f),
            new PointF(6.7228394f, -94.53476f),
            new PointF(6.440613f, -92.50901f),
            new PointF(5.6777954f, -86.62433f),
            new PointF(4.6000977f, -77.50447f),
            new PointF(3.3648682f, -65.70663f),
            new PointF(2.1296387f, -51.787342f),
            new PointF(1.0518188f, -36.30257f),
            new PointF(0.28930664f, -19.80773f),
            new PointF(0.0f, -2.8601987f),
            new PointF(0.28930664f, 14.847216f),
            new PointF(1.0516357f, 32.229347f),
            new PointF(2.1290894f, 48.659267f),
            new PointF(3.3640137f, 63.510746f),
            new PointF(4.598877f, 76.15679f),
            new PointF(5.6762695f, 85.96996f),
            new PointF(6.4386597f, 92.32222f),
            new PointF(6.730591f, 94.600266f),
            new PointF(7.0342407f, 96.55838f),
            new PointF(7.3740845f, 98.259445f),
            new PointF(7.817688f, 99.75538f),
            new PointF(8.454712f, 101.07463f),
            new PointF(9.377808f, 102.18319f),
            new PointF(10.598938f, 102.984474f),
            new PointF(12.044312f, 103.44184f),
            new PointF(13.619934f, 103.61155f),
            new PointF(25.72931f, 104.38121f),
            new PointF(59.84784f, 106.40455f),
            new PointF(112.82364f, 109.26503f),
            new PointF(181.47812f, 112.544014f),
            new PointF(262.6329f, 115.823006f),
            new PointF(353.10977f, 118.68347f),
            new PointF(449.73102f, 120.70676f),
            new PointF(549.31805f, 121.47421f),
            new PointF(646.8798f, 120.70671f),
            new PointF(738.54553f, 118.683395f),
            new PointF(822.12305f, 115.82281f),
            new PointF(895.421f, 112.54362f),
            new PointF(956.2478f, 109.26443f),
            new PointF(1002.4114f, 106.40375f),
            new PointF(1031.72f, 104.380165f),
            new PointF(1042.0239f, 103.60836f),
            new PointF(1043.7461f, 103.3683f),
            new PointF(1045.3113f, 102.914055f),
            new PointF(1046.6934f, 102.1857f),
            new PointF(1047.843f, 101.14973f),
            new PointF(1048.7114f, 99.83493f),
            new PointF(1049.3081f, 98.288025f),
            new PointF(1049.6847f, 96.51595f),
            new PointF(1049.8845f, 94.55957f),
            new PointF(1050.1676f, 92.31971f),
            new PointF(1050.9193f, 85.96759f),
            new PointF(1051.9814f, 76.15461f),
            new PointF(1053.199f, 63.508846f),
            new PointF(1054.4167f, 48.65776f),
            new PointF(1055.4791f, 32.228207f),
            new PointF(1056.2307f, 14.846553f),
            new PointF(1056.516f, -2.8601935f),
            new PointF(1056.2307f, -19.807068f),
            new PointF(1055.4789f, -36.30136f),
            new PointF(1054.4163f, -51.78568f),
            new PointF(1053.1982f, -65.70462f),
            new PointF(1051.9805f, -77.502144f),
            new PointF(1050.9177f, -86.621796f),
            new PointF(1050.1658f, -92.50628f),
        };

        public static readonly int[] MessageInnerIndices =
        {
            4, 5, 6, 3, 4, 7, 4, 6, 7, 0, 1, 8,
            1, 2, 8, 2, 3, 8, 3, 7, 8, 0, 8, 9,
            26, 27, 29, 27, 28, 29, 25, 26, 30, 26, 29, 30,
            24, 25, 31, 25, 30, 31, 24, 31, 32, 23, 24, 33,
            24, 32, 33, 23, 33, 34, 23, 34, 35, 22, 23, 36,
            23, 35, 36, 22, 36, 37, 22, 37, 38, 21, 22, 39,
            22, 38, 39, 21, 39, 40, 52, 53, 54, 51, 52, 55,
            52, 54, 55, 47, 48, 56, 48, 49, 56, 49, 50, 56,
            50, 51, 56, 51, 55, 56, 45, 46, 57, 46, 47, 57,
            47, 56, 57, 42, 43, 58, 43, 44, 58, 44, 45, 58,
            45, 57, 58, 41, 42, 59, 42, 58, 59, 21, 40, 59,
            40, 41, 59, 20, 21, 60, 21, 59, 60, 18, 19, 62,
            19, 61, 62, 17, 18, 63, 18, 62, 63, 16, 17, 64,
            17, 63, 64, 15, 16, 65, 16, 64, 65, 14, 15, 66,
            15, 65, 66, 12, 13, 68, 13, 67, 68, 11, 12, 69,
            12, 68, 69, 74, 75, 77, 75, 76, 77, 73, 74, 78,
            74, 77, 78, 72, 73, 79, 73, 78, 79, 72, 79, 80,
            71, 72, 81, 72, 80, 81, 71, 81, 82, 70, 71, 83,
            71, 82, 83, 70, 83, 84, 70, 84, 85, 69, 70, 86,
            70, 85, 86, 69, 86, 87, 11, 69, 88, 69, 87, 88,
            11, 88, 89, 10, 11, 90, 11, 89, 90, 10, 90, 91,
            10, 91, 92, 9, 10, 93, 10, 92, 93, 9, 93, 94,
            0, 9, 95, 9, 94, 95, 19, 20, 61, 20, 60, 61,
            13, 14, 67, 14, 66, 67, 3, 100, 4, 4, 100, 5,
            5, 101, 6, 100, 101, 5, 6, 102, 7, 101, 102, 6,
            2, 99, 3, 3, 99, 100, 7, 103, 8, 102, 103, 7,
            95, 96, 0, 0, 96, 1, 1, 97, 2, 96, 97, 1,
            2, 98, 99, 97, 98, 2, 8, 104, 9, 103, 104, 8,
            9, 105, 10, 104, 105, 9, 25, 122, 26, 26, 122, 27,
            27, 123, 28, 122, 123, 27, 28, 124, 29, 123, 124, 28,
            29, 125, 30, 124, 125, 29, 24, 121, 25, 25, 121, 122,
            30, 126, 31, 125, 126, 30, 23, 120, 24, 24, 120, 121,
            31, 127, 32, 126, 127, 31, 32, 128, 33, 127, 128, 32,
            22, 119, 23, 23, 119, 120, 33, 129, 34, 128, 129, 33,
            34, 130, 35, 129, 130, 34, 35, 131, 36, 130, 131, 35,
            21, 118, 22, 22, 118, 119, 36, 132, 37, 131, 132, 36,
            37, 133, 38, 132, 133, 37, 38, 134, 39, 133, 134, 38,
            20, 117, 21, 21, 117, 118, 39, 135, 40, 134, 135, 39,
            40, 136, 41, 135, 136, 40, 51, 148, 52, 52, 148, 53,
            53, 149, 54, 148, 149, 53, 54, 150, 55, 149, 150, 54,
            50, 147, 51, 51, 147, 148, 55, 151, 56, 150, 151, 55,
            46, 143, 47, 47, 143, 48, 48, 144, 49, 143, 144, 48,
            49, 145, 50, 144, 145, 49, 50, 146, 147, 145, 146, 50,
            56, 152, 57, 151, 152, 56, 44, 141, 45, 45, 141, 46,
            46, 142, 143, 141, 142, 46, 57, 153, 58, 152, 153, 57,
            41, 138, 42, 42, 138, 43, 43, 139, 44, 138, 139, 43,
            44, 140, 141, 139, 140, 44, 58, 154, 59, 153, 154, 58,
            136, 137, 41, 41, 137, 138, 59, 155, 60, 154, 155, 59,
            19, 116, 20, 20, 116, 117, 60, 156, 61, 155, 156, 60,
            17, 114, 18, 18, 114, 19, 19, 115, 116, 114, 115, 19,
            156, 157, 61, 61, 157, 62, 62, 158, 63, 157, 158, 62,
            16, 113, 17, 17, 113, 114, 63, 159, 64, 158, 159, 63,
            15, 112, 16, 16, 112, 113, 64, 160, 65, 159, 160, 64,
            14, 111, 15, 15, 111, 112, 65, 161, 66, 160, 161, 65,
            13, 110, 14, 14, 110, 111, 66, 162, 67, 161, 162, 66,
            11, 108, 12, 12, 108, 13, 13, 109, 110, 108, 109, 13,
            162, 163, 67, 67, 163, 68, 68, 164, 69, 163, 164, 68,
            10, 107, 11, 11, 107, 108, 69, 165, 70, 164, 165, 69,
            73, 170, 74, 74, 170, 75, 75, 171, 76, 170, 171, 75,
            76, 172, 77, 171, 172, 76, 77, 173, 78, 172, 173, 77,
            72, 169, 73, 73, 169, 170, 78, 174, 79, 173, 174, 78,
            71, 168, 72, 72, 168, 169, 79, 175, 80, 174, 175, 79,
            80, 176, 81, 175, 176, 80, 70, 167, 71, 71, 167, 168,
            81, 177, 82, 176, 177, 81, 82, 178, 83, 177, 178, 82,
            165, 166, 70, 70, 166, 167, 83, 179, 84, 178, 179, 83,
            84, 180, 85, 179, 180, 84, 85, 181, 86, 180, 181, 85,
            86, 182, 87, 181, 182, 86, 87, 183, 88, 182, 183, 87,
            88, 184, 89, 183, 184, 88, 89, 185, 90, 184, 185, 89,
            105, 106, 10, 10, 106, 107, 90, 186, 91, 185, 186, 90,
            91, 187, 92, 186, 187, 91, 92, 188, 93, 187, 188, 92,
            93, 189, 94, 188, 189, 93, 94, 190, 95, 189, 190, 94,
            190, 191, 95, 95, 191, 96,
        };

        public static PointF RemapPointToBounds(PointF point, PlgBounds sourceBounds, PlgBounds targetBounds)
        {
            float normalizedX = (point.X - sourceBounds.MinX) / sourceBounds.Width;
            float normalizedY = (point.Y - sourceBounds.MinY) / sourceBounds.Height;

            return new PointF(
                targetBounds.MinX + (normalizedX * targetBounds.Width),
                targetBounds.MinY + (normalizedY * targetBounds.Height));
        }

        public static PointF[] GetScaledVertices(PlgBounds targetBounds)
        {
            PointF[] scaledVertices = new PointF[MessageInnerVertices.Length];

            for (int i = 0; i < MessageInnerVertices.Length; i++)
            {
                scaledVertices[i] = RemapPointToBounds(MessageInnerVertices[i], SourceBounds, targetBounds);
            }

            return scaledVertices;
        }

        //public static PointF[] GetScaledVertices(float minX, float minY, float maxX, float maxY)
        //{
        //    return GetScaledVertices(new PlgBounds(minX, minY, maxX, maxY));
        //}

        //public static PointF[] GetScaledVerticesBySize(float x, float y, float width, float height)
        //{
        //    return GetScaledVertices(new PlgBounds(x, y, x + width, y + height));
        //}

        public static GraphicsPath BuildScaledPath(PlgBounds targetBounds)
        {
            PointF[] scaledVertices = GetScaledVertices(targetBounds);
            GraphicsPath path = new GraphicsPath(FillMode.Winding);

            for (int i = 0; i < MessageInnerIndices.Length; i += 3)
            {
                path.AddPolygon(new[]
                {
                scaledVertices[MessageInnerIndices[i]],
                scaledVertices[MessageInnerIndices[i + 1]],
                scaledVertices[MessageInnerIndices[i + 2]]
            });
            }

            return path;
        }

        public static GraphicsPath BuildScaledPath(float minX, float minY, float maxX, float maxY)
        {
            return BuildScaledPath(new PlgBounds(minX, minY, maxX, maxY));
        }

        public static GraphicsPath BuildScaledPathBySize(float x, float y, float width, float height)
        {
            return BuildScaledPath(new PlgBounds(x, y, x + width, y + height));
        }

        public static void FillMessageInner(
            Graphics graphics,
            Brush brush,
            PlgBounds targetBounds,
            SmoothingMode smoothingMode = SmoothingMode.AntiAlias)
        {
            graphics.SmoothingMode = smoothingMode;

            using (GraphicsPath path = BuildScaledPath(targetBounds))
            {
                graphics.FillPath(brush, path);
            }
        }

        //public static void FillMessageInner(
        //    Graphics graphics,
        //    Brush brush,
        //    float minX,
        //    float minY,
        //    float maxX,
        //    float maxY,
        //    SmoothingMode smoothingMode = SmoothingMode.AntiAlias)
        //{
        //    FillMessageInner(
        //        graphics,
        //        brush,
        //        new PlgBounds(minX, minY, maxX, maxY),
        //        smoothingMode);
        //}

        public static void FillMessageInnerBySize(
            Graphics graphics,
            Brush brush,
            float x,
            float y,
            float width,
            float height,
            SmoothingMode smoothingMode = SmoothingMode.AntiAlias)
        {
            FillMessageInner(
                graphics,
                brush,
                new PlgBounds(x, y, x + width, y + height),
                smoothingMode);
        }

        //public static Bitmap RenderMessageInner(
        //    int bitmapWidth,
        //    int bitmapHeight,
        //    System.Drawing.Color fillColor,
        //    PlgBounds targetBounds,
        //    System.Drawing.Color? backgroundColor = null,
        //    SmoothingMode smoothingMode = SmoothingMode.AntiAlias)
        //{
        //    Bitmap bitmap = new Bitmap(bitmapWidth, bitmapHeight);

        //    using (Graphics graphics = Graphics.FromImage(bitmap))
        //    using (SolidBrush fillBrush = new SolidBrush(fillColor))
        //    {
        //        graphics.SmoothingMode = smoothingMode;

        //        if (backgroundColor.HasValue)
        //        {
        //            graphics.Clear(backgroundColor.Value);
        //        }

        //        FillMessageInner(graphics, fillBrush, targetBounds, smoothingMode);
        //    }

        //    return bitmap;
        //}

        //public static Bitmap RenderMessageInnerBySize(
        //    int bitmapWidth,
        //    int bitmapHeight,
        //    System.Drawing.Color fillColor,
        //    float x,
        //    float y,
        //    float width,
        //    float height,
        //    System.Drawing.Color? backgroundColor = null,
        //    SmoothingMode smoothingMode = SmoothingMode.AntiAlias)
        //{
        //    return RenderMessageInner(
        //        bitmapWidth,
        //        bitmapHeight,
        //        fillColor,
        //        new PlgBounds(x, y, x + width, y + height),
        //        backgroundColor,
        //        smoothingMode);
        //}

        public static GraphicsPath BuildScaledAndRotatedPath(
            float minX,
            float minY,
            float maxX,
            float maxY,
            float angleDegrees,
            float pivotNormalizedX = 0.5f,
            float pivotNormalizedY = 0.5f)
        {
            GraphicsPath path = BuildScaledPath(minX, minY, maxX, maxY);

            float width = maxX - minX;
            float height = maxY - minY;

            float pivotX = minX + (width * pivotNormalizedX);
            float pivotY = minY + (height * pivotNormalizedY);

            using (Matrix matrix = new Matrix())
            {
                matrix.RotateAt(angleDegrees, new PointF(pivotX, pivotY));
                path.Transform(matrix);
            }

            return path;
        }

        public static void FillMessageInnerRotated(
            Graphics graphics,
            Brush brush,
            float minX,
            float minY,
            float maxX,
            float maxY,
            float angleDegrees,
            float pivotNormalizedX = 0.5f,
            float pivotNormalizedY = 0.5f)
        {
            using (GraphicsPath path = BuildScaledAndRotatedPath(
                minX, minY, maxX, maxY,
                angleDegrees,
                pivotNormalizedX, pivotNormalizedY))
            {
                graphics.FillPath(brush, path);
            }
        }

        public static void FillMessageInnerWithHorizontalGradient(
            Graphics graphics,
            float minX,
            float minY,
            float maxX,
            float maxY)
        {
            using (GraphicsPath path = BuildScaledPath(minX, minY, maxX, maxY))
            {
                RectangleF bounds = path.GetBounds();

                using (LinearGradientBrush gradientBrush = new LinearGradientBrush(
                    new PointF(bounds.Left, bounds.Top),
                    new PointF(bounds.Right, bounds.Top),
                    System.Drawing.Color.Black,
                    System.Drawing.Color.Black))
                {
                    const int stopCount = 50;

                    System.Drawing.Color[] colors = new System.Drawing.Color[stopCount];
                    float[] positions = new float[stopCount];

                    float peakPosition = 0.30f;
                    float leftSpread = 0.30f;
                    float rightSpread = 0.70f;

                    // Low and highlight colors
                    int lowR = 0;
                    int lowG = 5;
                    int lowB = 56;

                    int highR = 23;
                    int highG = 26;
                    int highB = 133;

                    int alpha = 220;

                    for (int i = 0; i < stopCount; i++)
                    {
                        float t = (float)i / (stopCount - 1);
                        positions[i] = t;

                        float local;
                        if (t <= peakPosition)
                        {
                            local = (t - peakPosition) / leftSpread;
                        }
                        else
                        {
                            local = (t - peakPosition) / rightSpread;
                        }

                        float highlight = 1f - (float)Math.Pow(Math.Abs(local), 2.0); //2.6, 2.2
                        highlight = Math.Max(0f, highlight);

                        int r = (int)(lowR + highlight * (highR - lowR));
                        int g = (int)(lowG + highlight * (highG - lowG));
                        int b = (int)(lowB + highlight * (highB - lowB));

                        colors[i] = System.Drawing.Color.FromArgb(alpha, r, g, b);
                    }

                    gradientBrush.InterpolationColors = new ColorBlend
                    {
                        Colors = colors,
                        Positions = positions
                    };

                    graphics.FillPath(gradientBrush, path);
                }
            }
        }

        public static void FillMessageInnerWithHorizontalGradientBySize(
            Graphics graphics,
            float x,
            float y,
            float width,
            float height)
        {
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            graphics.CompositingQuality = CompositingQuality.HighQuality;

            FillMessageInnerWithHorizontalGradient(
                graphics,
                x,
                y,
                x + width,
                y + height
            );
        }

        private static int ClampInt(int value, int min, int max)
        {
            if (value < min)
                return min;

            if (value > max)
                return max;

            return value;
        }

        public static void ApplySubtleNoise(Bitmap bitmap, int strength = 2)
        {
            Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            BitmapData data = bitmap.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            int byteCount = Math.Abs(data.Stride) * bitmap.Height;
            byte[] pixels = new byte[byteCount];
            Marshal.Copy(data.Scan0, pixels, 0, byteCount);

            Random rnd = new Random();

            for (int y = 0; y < bitmap.Height; y++)
            {
                int row = y * data.Stride;

                for (int x = 0; x < bitmap.Width; x++)
                {
                    int i = row + (x * 4);

                    byte b = pixels[i + 0];
                    byte g = pixels[i + 1];
                    byte r = pixels[i + 2];
                    byte a = pixels[i + 3];

                    // Skip fully transparent pixels
                    if (a == 0)
                        continue;

                    int noise = rnd.Next(-strength, strength + 1);

                    int newR = ClampInt(r + noise, 0, 255);
                    int newG = ClampInt(g + noise, 0, 255);
                    int newB = ClampInt(b + noise, 0, 255);

                    pixels[i + 0] = (byte)newB;
                    pixels[i + 1] = (byte)newG;
                    pixels[i + 2] = (byte)newR;
                    pixels[i + 3] = a;
                }
            }

            Marshal.Copy(pixels, 0, data.Scan0, byteCount);
            bitmap.UnlockBits(data);
        }

        public static void FillMessageInnerWithHorizontalGradientDitheredBySize(
            Graphics graphics,
            float x,
            float y,
            float width,
            float height,
            int noiseStrength = 2)
        {
            using Bitmap temp = new Bitmap(
                (int)Math.Ceiling(width),
                (int)Math.Ceiling(height),
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            using (Graphics tempGraphics = Graphics.FromImage(temp))
            {
                tempGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                tempGraphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                tempGraphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

                // Draw the gradient into local 0,0 space
                FillMessageInnerWithHorizontalGradient(
                    tempGraphics,
                    0f,
                    0f,
                    width,
                    height
                );
            }

            ApplySubtleNoise(temp, noiseStrength);

            graphics.DrawImage(temp, x, y, width, height);
        }
    }

    public static class Persona3ReloadSpeakerNameBaseBustupResizable
    {
        public struct PlgBounds
        {
            public float MinX;
            public float MinY;
            public float MaxX;
            public float MaxY;

            public float Width => MaxX - MinX;
            public float Height => MaxY - MinY;

            public PlgBounds(float minX, float minY, float maxX, float maxY)
            {
                MinX = minX;
                MinY = minY;
                MaxX = maxX;
                MaxY = maxY;
            }
        }

        public static readonly PlgBounds SourceBounds =
            new PlgBounds(-120.96772f, -46.67346f, 116.97429f, 46.207413f);

        public static readonly PointF[] SpeakerNameBaseBustupVertices = new PointF[]
        {
            new PointF(-114.0f, 4.5f),
            new PointF(63.0f, 44.5f),
            new PointF(114.0f, -44.5f),
            new PointF(-120.96772f, 4.4631996f),
            new PointF(63.750416f, 46.207413f),
            new PointF(116.97429f, -46.67346f)
        };

        public static readonly int[] SpeakerNameBaseBustupIndices = new int[]
        {
        0, 1, 2,
        2, 3, 0,
        0, 3, 1,
        1, 4, 2,
        3, 4, 1,
        2, 5, 3,
        4, 5, 2
        };

        public static PointF RemapPointToBounds(PointF point, PlgBounds source, PlgBounds target)
        {
            float normalizedX = (point.X - source.MinX) / source.Width;
            float normalizedY = (point.Y - source.MinY) / source.Height;

            return new PointF(
                target.MinX + normalizedX * target.Width,
                target.MinY + normalizedY * target.Height
            );
        }

        public static PointF[] GetScaledVertices(float targetMinX, float targetMinY, float targetMaxX, float targetMaxY)
        {
            PlgBounds targetBounds = new PlgBounds(targetMinX, targetMinY, targetMaxX, targetMaxY);
            PointF[] scaled = new PointF[SpeakerNameBaseBustupVertices.Length];

            for (int i = 0; i < SpeakerNameBaseBustupVertices.Length; i++)
            {
                scaled[i] = RemapPointToBounds(SpeakerNameBaseBustupVertices[i], SourceBounds, targetBounds);
            }

            return scaled;
        }

        public static GraphicsPath BuildScaledPath(float targetMinX, float targetMinY, float targetMaxX, float targetMaxY)
        {
            PointF[] scaledVertices = GetScaledVertices(targetMinX, targetMinY, targetMaxX, targetMaxY);
            GraphicsPath path = new GraphicsPath(FillMode.Winding);

            for (int i = 0; i < SpeakerNameBaseBustupIndices.Length; i += 3)
            {
                path.AddPolygon(new PointF[]
                {
                scaledVertices[SpeakerNameBaseBustupIndices[i]],
                scaledVertices[SpeakerNameBaseBustupIndices[i + 1]],
                scaledVertices[SpeakerNameBaseBustupIndices[i + 2]]
                });
            }

            return path;
        }

        public static void FillSpeakerNameBaseBustup(Graphics graphics, Brush brush, float targetMinX, float targetMinY, float targetMaxX, float targetMaxY)
        {
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            graphics.CompositingQuality = CompositingQuality.HighQuality;

            using (GraphicsPath path = BuildScaledPath(targetMinX, targetMinY, targetMaxX, targetMaxY))
            {
                graphics.FillPath(brush, path);
            }
        }

        public static void FillSpeakerNameBaseBustupBySize(Graphics graphics, Brush brush, float x, float y, float width, float height)
        {
            FillSpeakerNameBaseBustup(graphics, brush, x, y, x + width, y + height);
        }

        //public static Bitmap RenderSpeakerNameBaseBustup(int bitmapWidth, int bitmapHeight, System.Drawing.Color fillColor, float targetMinX, float targetMinY, float targetMaxX, float targetMaxY)
        //{
        //    Bitmap bitmap = new Bitmap(bitmapWidth, bitmapHeight);

        //    using (Graphics graphics = Graphics.FromImage(bitmap))
        //    using (SolidBrush brush = new SolidBrush(fillColor))
        //    {
        //        FillSpeakerNameBaseBustup(graphics, brush, targetMinX, targetMinY, targetMaxX, targetMaxY);
        //    }

        //    return bitmap;
        //}

        //public static Bitmap RenderSpeakerNameBaseBustupBySize(int bitmapWidth, int bitmapHeight, System.Drawing.Color fillColor, float x, float y, float width, float height)
        //{
        //    return RenderSpeakerNameBaseBustup(bitmapWidth, bitmapHeight, fillColor, x, y, x + width, y + height);
        //}
    }

    public static class Persona3ReloadSpeakerNameBaseNoBustupResizable
    {
        public struct PlgBounds
        {
            public float MinX;
            public float MinY;
            public float MaxX;
            public float MaxY;

            public float Width => MaxX - MinX;
            public float Height => MaxY - MinY;

            public PlgBounds(float minX, float minY, float maxX, float maxY)
            {
                MinX = minX;
                MinY = minY;
                MaxX = maxX;
                MaxY = maxY;
            }
        }

        public static readonly PlgBounds SourceBounds =
            new PlgBounds(-127.00302f, -69.32443f, 101.53408f, 45.4659f);

        public static readonly PointF[] SpeakerNameBaseNoBustupVertices =
        {
            new PointF(-123.43965f, -16.421963f),
            new PointF(95.56035f, -66.42197f),
            new PointF(-40.43965f, 43.578037f),
            new PointF(-127.00302f, -17.147005f),
            new PointF(101.53408f, -69.32443f),
            new PointF(-40.388504f, 45.4659f)
        };

        public static readonly int[] SpeakerNameBaseNoBustupIndices =
        {
            0, 1, 2, 2, 3, 0, 0, 3, 1, 1, 4, 2, 3, 4, 1, 2, 5, 3, 4, 5, 2
        };

        private static PointF RemapPointToBounds(PointF point, PlgBounds source, PlgBounds target)
        {
            float normalizedX = (point.X - source.MinX) / source.Width;
            float normalizedY = (point.Y - source.MinY) / source.Height;

            return new PointF(
                target.MinX + normalizedX * target.Width,
                target.MinY + normalizedY * target.Height
            );
        }

        public static PointF[] GetScaledVertices(float targetMinX, float targetMinY, float targetMaxX, float targetMaxY)
        {
            PlgBounds targetBounds = new PlgBounds(targetMinX, targetMinY, targetMaxX, targetMaxY);
            PointF[] scaled = new PointF[SpeakerNameBaseNoBustupVertices.Length];

            for (int i = 0; i < SpeakerNameBaseNoBustupVertices.Length; i++)
            {
                scaled[i] = RemapPointToBounds(SpeakerNameBaseNoBustupVertices[i], SourceBounds, targetBounds);
            }

            return scaled;
        }

        public static GraphicsPath BuildScaledPath(float targetMinX, float targetMinY, float targetMaxX, float targetMaxY)
        {
            PointF[] scaledVertices = GetScaledVertices(targetMinX, targetMinY, targetMaxX, targetMaxY);
            GraphicsPath path = new GraphicsPath(FillMode.Winding);

            for (int i = 0; i < SpeakerNameBaseNoBustupIndices.Length; i += 3)
            {
                path.AddPolygon(new PointF[]
                {
                scaledVertices[SpeakerNameBaseNoBustupIndices[i]],
                scaledVertices[SpeakerNameBaseNoBustupIndices[i + 1]],
                scaledVertices[SpeakerNameBaseNoBustupIndices[i + 2]]
                });
            }

            return path;
        }

        public static void FillSpeakerNameBaseNoBustup(
            Graphics graphics,
            Brush brush,
            float targetMinX,
            float targetMinY,
            float targetMaxX,
            float targetMaxY)
        {
            if (graphics == null) throw new ArgumentNullException(nameof(graphics));
            if (brush == null) throw new ArgumentNullException(nameof(brush));

            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            graphics.CompositingQuality = CompositingQuality.HighQuality;

            using (GraphicsPath path = BuildScaledPath(targetMinX, targetMinY, targetMaxX, targetMaxY))
            {
                graphics.FillPath(brush, path);
            }
        }

        public static void FillSpeakerNameBaseNoBustupBySize(
            Graphics graphics,
            Brush brush,
            float x,
            float y,
            float width,
            float height)
        {
            FillSpeakerNameBaseNoBustup(graphics, brush, x, y, x + width, y + height);
        }

        //public static Bitmap RenderSpeakerNameBaseNoBustup(
        //    int bitmapWidth,
        //    int bitmapHeight,
        //    System.Drawing.Color fillColor,
        //    float targetMinX,
        //    float targetMinY,
        //    float targetMaxX,
        //    float targetMaxY)
        //{
        //    Bitmap bitmap = new Bitmap(bitmapWidth, bitmapHeight);

        //    using (Graphics graphics = Graphics.FromImage(bitmap))
        //    using (SolidBrush brush = new SolidBrush(fillColor))
        //    {
        //        FillSpeakerNameBaseNoBustup(graphics, brush, targetMinX, targetMinY, targetMaxX, targetMaxY);
        //    }

        //    return bitmap;
        //}

        //public static Bitmap RenderSpeakerNameBaseNoBustupBySize(
        //    int bitmapWidth,
        //    int bitmapHeight,
        //    System.Drawing.Color fillColor,
        //    float x,
        //    float y,
        //    float width,
        //    float height)
        //{
        //    return RenderSpeakerNameBaseNoBustup(bitmapWidth, bitmapHeight, fillColor, x, y, x + width, y + height);
        //}
    }

    public static class Persona3ReloadSpeakerNameTailBaseBustupResizable
    {
        public struct PlgBounds
        {
            public float MinX;
            public float MinY;
            public float MaxX;
            public float MaxY;

            public float Width => MaxX - MinX;
            public float Height => MaxY - MinY;

            public PlgBounds(float minX, float minY, float maxX, float maxY)
            {
                MinX = minX;
                MinY = minY;
                MaxX = maxX;
                MaxY = maxY;
            }
        }

        public static readonly PlgBounds SourceBounds =
            new PlgBounds(-49.393047f, -27.421165f, 45.5f, 27.57367f);

        public static readonly PointF[] SpeakerNameTailBaseBustupVertices =
        {
            new PointF(-44.0f, -3.5f),
            new PointF(44.0f, 25.5f),
            new PointF(44.0f, -25.5f),
            new PointF(-49.393047f, -3.6979027f),
            new PointF(45.5f, 27.57367f),
            new PointF(45.5f, -27.421165f)
        };

        public static readonly int[] SpeakerNameTailBaseBustupIndices =
        {
            0, 1, 2,
            2, 3, 0,
            0, 3, 1,
            1, 4, 2,
            3, 4, 1,
            2, 5, 3,
            4, 5, 2
        };

        public static PointF RemapPointToBounds(PointF point, PlgBounds source, PlgBounds target)
        {
            float normalizedX = (point.X - source.MinX) / source.Width;
            float normalizedY = (point.Y - source.MinY) / source.Height;

            return new PointF(
                target.MinX + normalizedX * target.Width,
                target.MinY + normalizedY * target.Height
            );
        }

        public static PointF[] GetScaledVertices(float targetMinX, float targetMinY, float targetMaxX, float targetMaxY)
        {
            PlgBounds target = new PlgBounds(targetMinX, targetMinY, targetMaxX, targetMaxY);
            PointF[] scaled = new PointF[SpeakerNameTailBaseBustupVertices.Length];

            for (int i = 0; i < SpeakerNameTailBaseBustupVertices.Length; i++)
            {
                scaled[i] = RemapPointToBounds(SpeakerNameTailBaseBustupVertices[i], SourceBounds, target);
            }

            return scaled;
        }

        public static GraphicsPath BuildScaledPath(float targetMinX, float targetMinY, float targetMaxX, float targetMaxY)
        {
            PointF[] scaledVertices = GetScaledVertices(targetMinX, targetMinY, targetMaxX, targetMaxY);

            GraphicsPath path = new GraphicsPath(FillMode.Winding);

            for (int i = 0; i < SpeakerNameTailBaseBustupIndices.Length; i += 3)
            {
                path.AddPolygon(new PointF[]
                {
                scaledVertices[SpeakerNameTailBaseBustupIndices[i]],
                scaledVertices[SpeakerNameTailBaseBustupIndices[i + 1]],
                scaledVertices[SpeakerNameTailBaseBustupIndices[i + 2]]
                });
            }

            return path;
        }

        public static void FillSpeakerNameTailBaseBustup(Graphics graphics, Brush brush, float targetMinX, float targetMinY, float targetMaxX, float targetMaxY)
        {
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            graphics.CompositingQuality = CompositingQuality.HighQuality;

            using (GraphicsPath path = BuildScaledPath(targetMinX, targetMinY, targetMaxX, targetMaxY))
            {
                graphics.FillPath(brush, path);
            }
        }

        public static void FillSpeakerNameTailBaseBustupBySize(Graphics graphics, Brush brush, float x, float y, float width, float height)
        {
            FillSpeakerNameTailBaseBustup(graphics, brush, x, y, x + width, y + height);
        }

        //public static Bitmap RenderSpeakerNameTailBaseBustup(int bitmapWidth = 256, int bitmapHeight = 256, float targetMinX = 50f, float targetMinY = 100f, float targetMaxX = 200f, float targetMaxY = 180f, System.Drawing.Color? fillColor = null)
        //{
        //    Bitmap bmp = new Bitmap(bitmapWidth, bitmapHeight);

        //    using (Graphics graphics = Graphics.FromImage(bmp))
        //    using (SolidBrush brush = new SolidBrush(fillColor ?? System.Drawing.Color.Black))
        //    {
        //        graphics.Clear(System.Drawing.Color.Transparent);
        //        FillSpeakerNameTailBaseBustup(graphics, brush, targetMinX, targetMinY, targetMaxX, targetMaxY);
        //    }

        //    return bmp;
        //}

        //public static Bitmap RenderSpeakerNameTailBaseBustupBySize(int bitmapWidth = 256, int bitmapHeight = 256, float x = 50f, float y = 100f, float width = 150f, float height = 80f, System.Drawing.Color? fillColor = null)
        //{
        //    return RenderSpeakerNameTailBaseBustup(bitmapWidth, bitmapHeight, x, y, x + width, y + height, fillColor);
        //}
    }

    public struct PlgBounds
    {
        public float MinX;
        public float MinY;
        public float MaxX;
        public float MaxY;

        public float Width => MaxX - MinX;
        public float Height => MaxY - MinY;

        public PlgBounds(float minX, float minY, float maxX, float maxY)
        {
            MinX = minX;
            MinY = minY;
            MaxX = maxX;
            MaxY = maxY;
        }
    }

    /// <summary>
    /// Resizable helpers for the two different PLG entries named 文字送り.
    /// VariantA is the 12-vertex version with bounds
    /// MinX = -36.188763, MinY = -17.001892, MaxX = 36.188763, MaxY = 17.576553.
    /// VariantB is the 6-vertex version with bounds
    /// MinX = -46.35325, MinY = -21.013353, MaxX = 46.35324, MaxY = 21.549795.
    /// </summary>
    public static class Persona3ReloadTextAdvance
    {

        public static class VariantA
        {
            public static readonly PlgBounds SourceBounds =
                new PlgBounds(-36.188763f, -17.001892f, 36.188763f, 17.576553f);

            public static readonly PointF[] Vertices = new PointF[]
            {
            new PointF(26.861694f, -15.501892f),
            new PointF(0.0f, 10.164612f),
            new PointF(-26.861694f, -15.501892f),
            new PointF(-32.44763f, -15.501892f),
            new PointF(0.0f, 15.501892f),
            new PointF(32.44763f, -15.501892f),
            new PointF(0.0f, 8.089948f),
            new PointF(-26.260271f, -17.001892f),
            new PointF(-36.188763f, -17.001892f),
            new PointF(0.0f, 17.576553f),
            new PointF(26.260271f, -17.001892f),
            new PointF(36.188763f, -17.001892f)
            };

            public static readonly int[] Indices = new int[]
            {
            1, 2, 4, 2, 3, 4, 0, 1, 4, 0, 4, 5, 0, 6, 1, 1, 6, 2, 2, 7, 3, 6, 7, 2, 3, 8, 4, 7, 8, 3, 4, 9, 5, 8, 9, 4, 5, 10, 0, 0, 10, 6, 5, 11, 10, 9, 11, 5
            };

            private static PointF RemapPointToBounds(PointF point, PlgBounds source, PlgBounds target)
            {
                float normalizedX = (point.X - source.MinX) / source.Width;
                float normalizedY = (point.Y - source.MinY) / source.Height;

                return new PointF(
                    target.MinX + normalizedX * target.Width,
                    target.MinY + normalizedY * target.Height
                );
            }

            public static PointF[] GetScaledVertices(float minX, float minY, float maxX, float maxY)
            {
                PlgBounds targetBounds = new PlgBounds(minX, minY, maxX, maxY);
                PointF[] scaled = new PointF[Vertices.Length];

                for (int i = 0; i < Vertices.Length; i++)
                {
                    scaled[i] = RemapPointToBounds(Vertices[i], SourceBounds, targetBounds);
                }

                return scaled;
            }

            public static GraphicsPath BuildScaledPath(float minX, float minY, float maxX, float maxY)
            {
                PointF[] scaledVertices = GetScaledVertices(minX, minY, maxX, maxY);
                GraphicsPath path = new GraphicsPath(FillMode.Winding);

                for (int i = 0; i < Indices.Length; i += 3)
                {
                    path.AddPolygon(new PointF[]
                    {
                    scaledVertices[Indices[i]],
                    scaledVertices[Indices[i + 1]],
                    scaledVertices[Indices[i + 2]]
                    });
                }

                return path;
            }

            public static void Fill(Graphics graphics, Brush brush, float minX, float minY, float maxX, float maxY)
            {
                using (GraphicsPath path = BuildScaledPath(minX, minY, maxX, maxY))
                {
                    graphics.FillPath(brush, path);
                }
            }

            public static void FillBySize(Graphics graphics, Brush brush, float x, float y, float width, float height)
            {
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphics.CompositingQuality = CompositingQuality.HighQuality;

                Fill(graphics, brush, x, y, x + width, y + height);
            }

            //public static Bitmap Render(float minX, float minY, float maxX, float maxY, int bitmapWidth, int bitmapHeight, System.Drawing.Color fillColor)
            //{
            //    Bitmap bitmap = new Bitmap(bitmapWidth, bitmapHeight);

            //    using (Graphics graphics = Graphics.FromImage(bitmap))
            //    using (SolidBrush brush = new SolidBrush(fillColor))
            //    {
            //        graphics.SmoothingMode = SmoothingMode.AntiAlias;
            //        graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            //        graphics.CompositingQuality = CompositingQuality.HighQuality;
            //        Fill(graphics, brush, minX, minY, maxX, maxY);
            //    }

            //    return bitmap;
            //}

            //public static Bitmap RenderBySize(float x, float y, float width, float height, int bitmapWidth, int bitmapHeight, System.Drawing.Color fillColor)
            //{
            //    return Render(x, y, x + width, y + height, bitmapWidth, bitmapHeight, fillColor);
            //}
        }


        public static class VariantB
        {
            public static readonly PlgBounds SourceBounds =
                new PlgBounds(-46.35325f, -21.013353f, 46.35324f, 21.549795f);

            public static readonly PointF[] Vertices = new PointF[]
            {
            new PointF(42.501892f, -19.513351f),
            new PointF(0.000061035f, 19.513351f),
            new PointF(-42.501892f, -19.513351f),
            new PointF(46.35324f, -21.013353f),
            new PointF(0.000062466f, 21.549795f),
            new PointF(-46.35325f, -21.013351f)
            };

            public static readonly int[] Indices = new int[]
            {
            0, 1, 2, 2, 3, 0, 0, 3, 1, 1, 4, 2, 3, 4, 1, 2, 5, 3, 4, 5, 2
            };

            private static PointF RemapPointToBounds(PointF point, PlgBounds source, PlgBounds target)
            {
                float normalizedX = (point.X - source.MinX) / source.Width;
                float normalizedY = (point.Y - source.MinY) / source.Height;

                return new PointF(
                    target.MinX + normalizedX * target.Width,
                    target.MinY + normalizedY * target.Height
                );
            }

            public static PointF[] GetScaledVertices(float minX, float minY, float maxX, float maxY)
            {
                PlgBounds targetBounds = new PlgBounds(minX, minY, maxX, maxY);
                PointF[] scaled = new PointF[Vertices.Length];

                for (int i = 0; i < Vertices.Length; i++)
                {
                    scaled[i] = RemapPointToBounds(Vertices[i], SourceBounds, targetBounds);
                }

                return scaled;
            }

            public static GraphicsPath BuildScaledPath(float minX, float minY, float maxX, float maxY)
            {
                PointF[] scaledVertices = GetScaledVertices(minX, minY, maxX, maxY);
                GraphicsPath path = new GraphicsPath(FillMode.Winding);

                for (int i = 0; i < Indices.Length; i += 3)
                {
                    path.AddPolygon(new PointF[]
                    {
                    scaledVertices[Indices[i]],
                    scaledVertices[Indices[i + 1]],
                    scaledVertices[Indices[i + 2]]
                    });
                }

                return path;
            }

            public static void Fill(Graphics graphics, Brush brush, float minX, float minY, float maxX, float maxY)
            {
                using (GraphicsPath path = BuildScaledPath(minX, minY, maxX, maxY))
                {
                    graphics.FillPath(brush, path);
                }
            }

            public static void FillBySize(Graphics graphics, Brush brush, float x, float y, float width, float height)
            {
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphics.CompositingQuality = CompositingQuality.HighQuality;

                Fill(graphics, brush, x, y, x + width, y + height);
            }

            //public static Bitmap Render(float minX, float minY, float maxX, float maxY, int bitmapWidth, int bitmapHeight, System.Drawing.Color fillColor)
            //{
            //    Bitmap bitmap = new Bitmap(bitmapWidth, bitmapHeight);

            //    using (Graphics graphics = Graphics.FromImage(bitmap))
            //    using (SolidBrush brush = new SolidBrush(fillColor))
            //    {
            //        graphics.SmoothingMode = SmoothingMode.AntiAlias;
            //        graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            //        graphics.CompositingQuality = CompositingQuality.HighQuality;
            //        Fill(graphics, brush, minX, minY, maxX, maxY);
            //    }

            //    return bitmap;
            //}

            //public static Bitmap RenderBySize(float x, float y, float width, float height, int bitmapWidth, int bitmapHeight, System.Drawing.Color fillColor)
            //{
            //    return Render(x, y, x + width, y + height, bitmapWidth, bitmapHeight, fillColor);
            //}
        }

    }

    public class DialogueRenderer
    {
        // Adjust these as needed.
        private const string FontName = "FOT-スキップ Pro E"; //FOT-スキップ Pro E //FOT-スキップ Std B
        private const float FontSize = 33f;
        private const float MaxLineWidth = 900f;
        private const int MaxLines = 3;

        public DialogueRenderResult RenderDialogueAdvanced(
            string dialogue,
            int bitmapWidth,
            int bitmapHeight,
            float startX,
            float startY,
            float letterSpacing = 0f,
            float spaceScale = 0.7f,
            float lineSpacing = 6f,
            bool drawOutline = false,
            System.Drawing.Color? fillColor = null,
            System.Drawing.Color? outlineColor = null,
            float outlineWidth = 2f)
        {
            Bitmap output = new Bitmap(bitmapWidth, bitmapHeight);

            using (Graphics graphics = Graphics.FromImage(output))
            using (Font font = new Font(FontName, FontSize, FontStyle.Regular, GraphicsUnit.Pixel))
            using (SolidBrush fillBrush = new SolidBrush(fillColor ?? System.Drawing.Color.Red))
            using (Pen outlinePen = new Pen(outlineColor ?? System.Drawing.Color.Black, outlineWidth)
            {
                LineJoin = LineJoin.Round
            })
            {
                graphics.Clear(System.Drawing.Color.Transparent);
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

                float defaultSpaceWidth = MeasureSpaceAdvance(graphics, font);
                float customSpaceWidth = defaultSpaceWidth * spaceScale;

                DialogueLayoutResult layout = WrapTextToLinesAdvanced(
                    graphics,
                    dialogue,
                    font,
                    MaxLineWidth,
                    MaxLines,
                    letterSpacing,
                    customSpaceWidth);

                float y = startY;

                switch (layout.LineCount)
                {
                    case 1:
                        y += 0f;
                        break;

                    case 2:
                        y -= 18f;
                        break;

                    case 3:
                        y -= 41f;
                        break;
                }

                foreach (string line in layout.Lines)
                {
                    DrawLineWithCustomSpacing(
                        graphics,
                        line,
                        font,
                        fillBrush,
                        outlinePen,
                        startX,
                        y,
                        letterSpacing,
                        customSpaceWidth,
                        drawOutline);

                    y += GetLineHeight(graphics, font) + lineSpacing;
                }

                return new DialogueRenderResult
                {
                    Bitmap = output,
                    Lines = new List<string>(layout.Lines),
                    LineWidths = new List<float>(layout.LineWidths)
                };
            }
        }

        private DialogueLayoutResult WrapTextToLinesAdvanced(
            Graphics graphics,
            string input,
            Font font,
            float maxLineWidth,
            int maxLines,
            float letterSpacing,
            float customSpaceWidth)
        {
            DialogueLayoutResult result = new DialogueLayoutResult();

            if (string.IsNullOrWhiteSpace(input))
            {
                return result;
            }

            string normalized = input
                .Replace("\r\n", "\n")
                .Replace('\r', '\n')
                .Replace("|", "\n");

            string[] manualLines = normalized.Split('\n');

            foreach (string manualLine in manualLines)
            {
                if (result.LineCount >= maxLines)
                {
                    break;
                }

                if (string.IsNullOrWhiteSpace(manualLine))
                {
                    result.Lines.Add(string.Empty);
                    result.LineWidths.Add(0f);

                    if (result.LineCount >= maxLines)
                    {
                        break;
                    }

                    continue;
                }

                string[] words = Regex.Split(manualLine.Trim(), @"\s+");
                string currentLine = string.Empty;

                foreach (string word in words)
                {
                    if (result.LineCount >= maxLines)
                    {
                        break;
                    }

                    string testLine = string.IsNullOrEmpty(currentLine)
                        ? word
                        : currentLine + " " + word;

                    float testWidth = MeasureStringAdvance(
                        graphics,
                        font,
                        testLine,
                        letterSpacing,
                        customSpaceWidth);

                    if (testWidth <= maxLineWidth)
                    {
                        currentLine = testLine;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(currentLine))
                        {
                            float currentWidth = MeasureStringAdvance(
                                graphics,
                                font,
                                currentLine,
                                letterSpacing,
                                customSpaceWidth);

                            result.Lines.Add(currentLine);
                            result.LineWidths.Add(currentWidth);

                            if (result.LineCount >= maxLines)
                            {
                                break;
                            }

                            currentLine = word;
                        }
                        else
                        {
                            List<string> splitPieces = HardSplitWord(
                                graphics,
                                font,
                                word,
                                maxLineWidth,
                                letterSpacing,
                                customSpaceWidth);

                            for (int i = 0; i < splitPieces.Count; i++)
                            {
                                if (result.LineCount >= maxLines)
                                {
                                    break;
                                }

                                string piece = splitPieces[i];
                                float pieceWidth = MeasureStringAdvance(
                                    graphics,
                                    font,
                                    piece,
                                    letterSpacing,
                                    customSpaceWidth);

                                bool isLastPiece = i == splitPieces.Count - 1;

                                if (!isLastPiece)
                                {
                                    result.Lines.Add(piece);
                                    result.LineWidths.Add(pieceWidth);
                                }
                                else
                                {
                                    currentLine = piece;
                                }
                            }
                        }
                    }
                }

                if (result.LineCount >= maxLines)
                {
                    break;
                }

                if (!string.IsNullOrEmpty(currentLine))
                {
                    float currentWidth = MeasureStringAdvance(
                        graphics,
                        font,
                        currentLine,
                        letterSpacing,
                        customSpaceWidth);

                    result.Lines.Add(currentLine);
                    result.LineWidths.Add(currentWidth);
                }
            }

            if (result.Lines.Count > maxLines)
            {
                result.Lines = result.Lines.Take(maxLines).ToList();
                result.LineWidths = result.LineWidths.Take(maxLines).ToList();
            }

            return result;
        }

        private List<string> HardSplitWord(
            Graphics graphics,
            Font font,
            string word,
            float maxLineWidth,
            float letterSpacing,
            float customSpaceWidth)
        {
            List<string> pieces = new List<string>();

            if (string.IsNullOrEmpty(word))
            {
                return pieces;
            }

            StringBuilder current = new StringBuilder();

            foreach (char c in word)
            {
                string test = current.ToString() + c;

                float width = MeasureStringAdvance(
                    graphics,
                    font,
                    test,
                    letterSpacing,
                    customSpaceWidth);

                if (width <= maxLineWidth || current.Length == 0)
                {
                    current.Append(c);
                }
                else
                {
                    pieces.Add(current.ToString());
                    current.Clear();
                    current.Append(c);
                }
            }

            if (current.Length > 0)
            {
                pieces.Add(current.ToString());
            }

            return pieces;
        }

        //private void DrawLineWithCustomSpacing(
        //    Graphics graphics,
        //    string text,
        //    Font font,
        //    Brush fillBrush,
        //    Pen outlinePen,
        //    float startX,
        //    float startY,
        //    float letterSpacing,
        //    float customSpaceWidth,
        //    bool drawOutline)
        //{
        //    float x = startX;

        //    foreach (char c in text)
        //    {
        //        if (c == ' ')
        //        {
        //            x += customSpaceWidth;
        //            continue;
        //        }

        //        string s = c.ToString();
        //        float advance = MeasureCharacterAdvance(graphics, font, c);

        //        if (drawOutline)
        //        {
        //            using (GraphicsPath path = new GraphicsPath())
        //            {
        //                float emSize = font.SizeInPoints * graphics.DpiY / 72f;

        //                path.AddString(
        //                    s,
        //                    font.FontFamily,
        //                    (int)font.Style,
        //                    emSize,
        //                    new PointF(x, startY),
        //                    StringFormat.GenericTypographic);

        //                graphics.DrawPath(outlinePen, path);
        //                graphics.FillPath(fillBrush, path);
        //            }
        //        }
        //        else
        //        {
        //            graphics.DrawString(
        //                s,
        //                font,
        //                fillBrush,
        //                new PointF(x, startY),
        //                StringFormat.GenericTypographic);
        //        }

        //        x += advance + letterSpacing;
        //    }
        //}

        private void DrawLineWithCustomSpacing(
            Graphics graphics,
            string text,
            Font font,
            Brush fillBrush,
            Pen outlinePen,
            float startX,
            float startY,
            float letterSpacing,
            float customSpaceWidth,
            bool drawOutline,
            float widthScale = 0.85f)
        {
            float x = startX;

            foreach (char c in text)
            {
                if (c == ' ')
                {
                    x += customSpaceWidth;
                    continue;
                }

                string s = c.ToString();
                float advance = MeasureCharacterAdvance(graphics, font, c) * widthScale;

                using (GraphicsPath path = new GraphicsPath())
                {
                    float emSize = font.SizeInPoints * graphics.DpiY / 72f;

                    path.AddString(
                        s,
                        font.FontFamily,
                        (int)font.Style,
                        emSize,
                        new PointF(x, startY),
                        StringFormat.GenericTypographic);

                    if (Math.Abs(widthScale - 1.0f) > 0.0001f)
                    {
                        using (Matrix matrix = new Matrix())
                        {
                            matrix.Translate(-x, 0f);
                            matrix.Scale(widthScale, 1.0f);
                            matrix.Translate(x, 0f);
                            path.Transform(matrix);
                        }
                    }

                    if (drawOutline)
                    {
                        graphics.DrawPath(outlinePen, path);
                    }

                    graphics.FillPath(fillBrush, path);
                }

                x += advance + letterSpacing;
            }
        }

        private float MeasureStringAdvance(
            Graphics graphics,
            Font font,
            string text,
            float letterSpacing,
            float customSpaceWidth)
        {
            float width = 0f;

            foreach (char c in text)
            {
                if (c == ' ')
                {
                    width += customSpaceWidth;
                }
                else
                {
                    width += MeasureCharacterAdvance(graphics, font, c) + letterSpacing;
                }
            }

            // Remove trailing letterSpacing so the last character doesn't add extra width.
            if (text.Length > 0 && text[text.Length - 1] != ' ')
            {
                width -= letterSpacing;
            }

            return width;
        }

        private float MeasureCharacterAdvance(Graphics graphics, Font font, char c)
        {
            string s = c.ToString();

            // Measure using GenericTypographic to reduce GDI+ padding.
            SizeF size = graphics.MeasureString(
                s,
                font,
                new PointF(0, 0),
                StringFormat.GenericTypographic);

            return size.Width;
        }

        private float GetLineHeight(Graphics graphics, Font font)
        {
            return font.GetHeight(graphics);
        }

        private float MeasureSpaceAdvance(Graphics graphics, Font font)
        {
            float withSpace = MeasureRawTextWidth(graphics, font, "A A");
            float withoutSpace = MeasureRawTextWidth(graphics, font, "AA");
            return withSpace - withoutSpace;
        }

        private float MeasureRawTextWidth(Graphics graphics, Font font, string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return 0f;
            }

            SizeF size = graphics.MeasureString(
                text,
                font,
                int.MaxValue,
                StringFormat.GenericTypographic);

            return size.Width;
        }

        public Bitmap RenderName(
            string name,
            int bitmapWidth,
            int bitmapHeight,
            float x,
            float y,
            float letterSpacing = 0f,
            float spaceScale = 0.7f,
            bool drawOutline = false,
            System.Drawing.Color? fillColor = null,
            System.Drawing.Color? outlineColor = null,
            float outlineWidth = 2f)
        {
            Bitmap output = new Bitmap(bitmapWidth, bitmapHeight);

            using (Graphics graphics = Graphics.FromImage(output))
            using (Font font = new Font(FontName, 24f, FontStyle.Regular, GraphicsUnit.Pixel)) // smaller font
            using (SolidBrush fillBrush = new SolidBrush(fillColor ?? System.Drawing.Color.White))
            using (Pen outlinePen = new Pen(outlineColor ?? System.Drawing.Color.Black, outlineWidth)
            {
                LineJoin = LineJoin.Round
            })
            {
                graphics.Clear(System.Drawing.Color.Transparent);
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

                float defaultSpaceWidth = MeasureSpaceAdvance(graphics, font);
                float customSpaceWidth = defaultSpaceWidth * spaceScale;

                DrawLineWithCustomSpacing(
                    graphics,
                    name,
                    font,
                    fillBrush,
                    outlinePen,
                    x,
                    y,
                    letterSpacing,
                    customSpaceWidth,
                    drawOutline);

                return output;
            }
        }
    }

    public class DialogueLayoutResult
    {
        public List<string> Lines { get; set; } = new List<string>();
        public List<float> LineWidths { get; set; } = new List<float>();

        public int LineCount => Lines.Count;
        public float LongestLineWidth => LineWidths.Count == 0 ? 0f : LineWidths.Max();
    }

    public class DialogueRenderResult
    {
        public Bitmap Bitmap { get; set; }
        public List<string> Lines { get; set; } = new List<string>();
        public List<float> LineWidths { get; set; } = new List<float>();
        public int LineCount => Lines.Count;
        public float LongestLineWidth => LineWidths.Count == 0 ? 0f : LineWidths.Max();
    }
}
