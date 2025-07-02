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
using System.Net;
using Newtonsoft.Json;
using System.Collections.Generic;
using SocialLinker.Core.SceneMaker.Data.Bustup;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using SocialLinker.Core.Menus;

namespace SocialLinker.Core.SceneMaker.TemplateRenders.QuickScenes
{
    public class RenderP3R
    {
        public int template_width_4k = 3840;
        public int template_height_4k = 2160;

        public int template_width = 1920;
        public int template_height = 1080;

        public int max_line_length = 480;

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
            Bitmap base_template = new Bitmap(template_width_4k, template_height_4k);
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
            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                // Draw the character bust-up to the template if the base sprite number is not '0'.
                if (maker_command_data.Character_Data_1.Base_Sprite != 0)
                {
                    Bitmap drop_shadow = Create_Bustup_Drop_Shadow(bustup);
                    graphics.DrawImage(drop_shadow, bustup_data.P3R_Coord_X - 44, bustup_data.P3R_Coord_Y + 28, bustup_data.P3R_Scale_Width, bustup_data.P3R_Scale_Height);
                    graphics.DrawImage(bustup, bustup_data.P3R_Coord_X, bustup_data.P3R_Coord_Y, bustup_data.P3R_Scale_Width, bustup_data.P3R_Scale_Height);
                }
            }

            base_template = Scale_Template(account, base_template);

            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                graphics.DrawImage(Render_Calendar_HUD_2(account), 0, 0, template_width, template_height);
                //graphics.DrawImage(Render_Calendar_HUD(account), 0, 0, template_width, template_height);
                graphics.DrawImage(Render_Moon_HUD(account), 0, 0, template_width, template_height);
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

            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                graphics.DrawImage(limit, 0, 0, template_width, template_height);
            }

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
}
