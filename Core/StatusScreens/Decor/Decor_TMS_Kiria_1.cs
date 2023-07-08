using System;
using System.Drawing;
using System.IO;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using SocialLinker.Config;
using SocialLinker.Core.CloudStorageTables;
using System.Drawing.Text;

namespace SocialLinker.Core.StatusScreens.Decor
{
    public static class Decor_TMS_Kiria_1
    {
        public const int template_width = 1920;
        public const int template_height = 1080;

        public static async void RenderImage(SocketGuildUser user, ISocketMessageChannel channel)
        {
            // Send a loading message while the status screen gets made
            RestUserMessage loader = await channel.SendMessageAsync("", false, LoadingMessage().Build());

            try
            {
                var account = UserInfoClasses.GetAccount(user);

                Bitmap base_template = new Bitmap(template_width, template_height);

                Bitmap chara_bg = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//Profile//StatusScreens//Decor//Decor_TMS_Kiria_1//chara_bg.png");
                Bitmap ui_overlay = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//Profile//StatusScreens//Decor//Decor_TMS_Kiria_1//ui_overlay.png");

                using (Graphics graphics = Graphics.FromImage(base_template))
                {
                    graphics.TextRenderingHint = TextRenderingHint.AntiAlias;

                    graphics.DrawImage(chara_bg, 0, 0, template_width, template_height);
                    graphics.DrawImage(ui_overlay, 0, 0, template_width, template_height);

                    graphics.DrawImage(RenderFont(user, account), 0, 0, template_width, template_height);
                    graphics.DrawImage(RenderLevelProgressBar(user), 0, 0, template_width, template_height);
                    graphics.DrawImage(CombineSocialStatRankBitmaps(account), 0, 0, template_width, template_height);

                    if (account.Level_Resets > 0)
                    {
                        graphics.DrawImage(RenderPrestigeCounter(account.Level_Resets), 0, 0, template_width, template_height);
                    }
                }

                MemoryStream memoryStream = new MemoryStream();
                base_template.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
                memoryStream.Seek(0, SeekOrigin.Begin);

                await channel.SendFileAsync(memoryStream, $"status_{user.Id}_{DateTime.UtcNow}.png");

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

        public static Bitmap RenderFont(SocketGuildUser user, UserInfoFields account)
        {
            Bitmap base_bitmap = new Bitmap(template_width, template_height);
            System.Drawing.Color tms_text_green = System.Drawing.Color.FromArgb(152, 254, 30);
            SolidBrush tms_text_green_brush = new SolidBrush(tms_text_green);

            System.Drawing.Color tms_text_yellow = System.Drawing.Color.FromArgb(255, 255, 3);
            SolidBrush tms_text_yellow_brush = new SolidBrush(tms_text_yellow);

            using (Graphics graphics = Graphics.FromImage(base_bitmap))
            {
                graphics.TextRenderingHint = TextRenderingHint.AntiAlias;

                StringFormat stringFormat = new StringFormat();

                Rectangle class_title_window = new Rectangle(52, 30, 700, 41);

                Rectangle level_window = new Rectangle(1785, 109, 117, 82);

                Rectangle username_window = new Rectangle(221, 1012, 1450, 40);

                Rectangle remaining_exp_window = new Rectangle(85, 236, 268, 48);

                Rectangle pmedal_window = new Rectangle(113, 375, 529, 54);

                Rectangle proficiency_rank_window = new Rectangle(268, 522, 85, 44);
                Rectangle proficiency_next_window = new Rectangle(218, 567, 135, 44);

                Rectangle diligence_rank_window = new Rectangle(268, 676, 85, 44);
                Rectangle diligence_next_window = new Rectangle(238, 721, 115, 44);

                Rectangle expression_rank_window = new Rectangle(268, 830, 85, 44);
                Rectangle expression_next_window = new Rectangle(218, 875, 135, 44);

                using (Font neology_deco = new Font("NeologyDecoW03-Regular", 23))
                {
                    stringFormat.Alignment = StringAlignment.Near;
                    graphics.DrawString($"{user.Nickname ?? user.Username}'s status.", neology_deco, Brushes.White, username_window, stringFormat);
                }

                Font arial_bold = new Font("Arial", 64, FontStyle.Bold);
                if (account.Level < 10)
                {
                    arial_bold = new Font("Arial", 64, FontStyle.Bold);
                }
                else
                {
                    arial_bold = new Font("Arial", 58, FontStyle.Bold);
                }
                using (arial_bold)
                {
                    stringFormat.Alignment = StringAlignment.Center;
                    graphics.DrawString($"{account.Level}", arial_bold, tms_text_green_brush, level_window, stringFormat);
                }

                int next_level_total_exp_required = LevelSystem.Leveling.CalculateExp(account.Level + 1);
                int required_exp = next_level_total_exp_required - account.Total_Exp;

                using (Font lucida_sans = new Font("Lucida Sans Unicode", 30))
                {
                    stringFormat.Alignment = StringAlignment.Far;
                    graphics.DrawString($"{required_exp}", lucida_sans, Brushes.White, remaining_exp_window, stringFormat);
                }

                using (Font lucida_sans = new Font("Lucida Sans Unicode", 30))
                {
                    stringFormat.Alignment = StringAlignment.Near;
                    graphics.DrawString($"{account.P_Medals}", lucida_sans, Brushes.White, pmedal_window, stringFormat);
                }

                // Social stats
                int proficiency_rank = LevelSystem.SocialStats.CalculateProficiencyRank(account.Proficiency);
                int diligence_rank = LevelSystem.SocialStats.CalculateDiligenceRank(account.Diligence);
                int expression_rank = LevelSystem.SocialStats.CalculateExpressionRank(account.Expression);

                int proficiency_left_to_rank_up = Get_Base_Proficiency_Of_Next_Rank(proficiency_rank) - account.Proficiency;
                int diligence_left_to_rank_up = Get_Base_Diligence_Of_Next_Rank(diligence_rank) - account.Diligence;
                int expression_left_to_rank_up = Get_Base_Expression_Of_Next_Rank(expression_rank) - account.Expression;

                using (Font neology_deco = new Font("NeologyDecoW03-Regular", 21))
                {
                    stringFormat.Alignment = StringAlignment.Near;
                    graphics.DrawString($"{Get_Class_Title(proficiency_rank, diligence_rank, expression_rank)}", neology_deco, Brushes.Black, class_title_window, stringFormat);
                }

                using (Font lucida_sans = new Font("Lucida Sans Unicode", 30))
                {
                    stringFormat.Alignment = StringAlignment.Far;

                    // Draw text to screen
                    if (proficiency_rank == 5)
                    {
                        graphics.DrawString($"{proficiency_rank}", lucida_sans, tms_text_yellow_brush, proficiency_rank_window, stringFormat);
                        graphics.DrawString($"{LevelSystem.SocialStats.SocialStatToDecimal(proficiency_left_to_rank_up)}", lucida_sans, tms_text_yellow_brush, proficiency_next_window, stringFormat);
                    }
                    else
                    {
                        graphics.DrawString($"{proficiency_rank}", lucida_sans, Brushes.White, proficiency_rank_window, stringFormat);
                        graphics.DrawString($"{LevelSystem.SocialStats.SocialStatToDecimal(proficiency_left_to_rank_up)}", lucida_sans, Brushes.White, proficiency_next_window, stringFormat);
                    }

                    if (diligence_rank == 5)
                    {
                        graphics.DrawString($"{diligence_rank}", lucida_sans, tms_text_yellow_brush, diligence_rank_window, stringFormat);
                        graphics.DrawString($"{LevelSystem.SocialStats.SocialStatToDecimal(diligence_left_to_rank_up)}", lucida_sans, tms_text_yellow_brush, diligence_next_window, stringFormat);
                    }
                    else
                    {
                        graphics.DrawString($"{diligence_rank}", lucida_sans, Brushes.White, diligence_rank_window, stringFormat);
                        graphics.DrawString($"{LevelSystem.SocialStats.SocialStatToDecimal(diligence_left_to_rank_up)}", lucida_sans, Brushes.White, diligence_next_window, stringFormat);
                    }


                    if (expression_rank == 5)
                    {
                        graphics.DrawString($"{expression_rank}", lucida_sans, tms_text_yellow_brush, expression_rank_window, stringFormat);
                        graphics.DrawString($"{LevelSystem.SocialStats.SocialStatToDecimal(expression_left_to_rank_up)}", lucida_sans, tms_text_yellow_brush, expression_next_window, stringFormat);
                    }
                    else
                    {
                        graphics.DrawString($"{expression_rank}", lucida_sans, Brushes.White, expression_rank_window, stringFormat);
                        graphics.DrawString($"{LevelSystem.SocialStats.SocialStatToDecimal(expression_left_to_rank_up)}", lucida_sans, Brushes.White, expression_next_window, stringFormat);
                    }
                }
            }

            return base_bitmap;
        }

        public static Bitmap RenderLevelProgressBar(SocketUser user)
        {
            var account = UserInfoClasses.GetAccount(user);

            Bitmap new_bitmap = new Bitmap(template_width, template_height);

            int total_exp = account.Total_Exp;

            Rectangle main_bar = new Rectangle(357, 262, 352, 18);
            Rectangle shadow_bar = new Rectangle(357, 278, 352, 2);

            int bar_max_value = 0;
            int bar_filled_value = 0;

            System.Drawing.Color tms_pink = System.Drawing.Color.FromArgb(233, 125, 174);
            System.Drawing.SolidBrush tms_pink_brush = new SolidBrush(tms_pink);

            System.Drawing.Color tms_pink_shadow = System.Drawing.Color.FromArgb(130, 89, 107);
            System.Drawing.SolidBrush tms_pink_shadow_brush = new SolidBrush(tms_pink_shadow);

            // Create a variable for the minimum total EXP requirement of the next level
            int next_level_total_exp_required = Core.LevelSystem.Leveling.CalculateExp(account.Level + 1);

            // Create a variable for the minimum total EXP requirement of the current level
            int current_level_total_exp_required = Core.LevelSystem.Leveling.CalculateExp(account.Level);

            bar_max_value = next_level_total_exp_required - current_level_total_exp_required;

            // Draw a progress bar that has a max value set to the minimum total EXP requirement of the next level minus the minimum total EXP requirement of the current level
            ProgressBar progress_bar = new ProgressBar(tms_pink_brush, main_bar, bar_max_value);
            ProgressBar progress_bar_shadow = new ProgressBar(tms_pink_shadow_brush, shadow_bar, bar_max_value);

            // However, if the user's level is 99, set the progress bar's max value to 1
            if (account.Level == 99)
            {
                bar_max_value = 1;
                progress_bar = new ProgressBar(tms_pink_brush, main_bar, bar_max_value);
                progress_bar_shadow = new ProgressBar(tms_pink_brush, shadow_bar, bar_max_value);
            }

            // Determine how the progress bar should be filled based on the user's level
            if (account.Level != 99)
            {
                // If the level is below 99, fill the progress bar by subtracting the total EXP the user has by the minimum total EXP requirement of the current level
                bar_filled_value = total_exp - current_level_total_exp_required;
                progress_bar.SetCurrent(bar_filled_value);
                progress_bar_shadow.SetCurrent(bar_filled_value);
            }
            else
            {
                // If the level is at 99, fill the progress bar completely up
                bar_filled_value = 1;
                progress_bar.SetCurrent(bar_filled_value);
                progress_bar_shadow.SetCurrent(bar_filled_value);
            }

            // Use a graphics object to edit the bitmap
            using (Graphics graphics = Graphics.FromImage(new_bitmap))
            {
                // Draw the progress bar to the bitmap
                graphics.DrawImage(progress_bar.GiveGraphic(), progress_bar.GiveCorner());
                graphics.DrawImage(progress_bar_shadow.GiveGraphic(), progress_bar_shadow.GiveCorner());
            }

            return new_bitmap;
        }

        public static Bitmap RenderRankBarLayer(UserInfoFields account)
        {
            Bitmap new_bitmap = new Bitmap(template_width, template_height);

            System.Drawing.Color tms_white = System.Drawing.Color.White;
            System.Drawing.Color tms_yellow = System.Drawing.Color.FromArgb(255, 255, 3);
            System.Drawing.Color tms_white_shadow = System.Drawing.Color.FromArgb(93, 93, 93);
            System.Drawing.Color tms_yellow_shadow = System.Drawing.Color.FromArgb(117, 112, 20);

            int proficiency_rank = LevelSystem.SocialStats.CalculateProficiencyRank(account.Proficiency);
            int diligence_rank = LevelSystem.SocialStats.CalculateDiligenceRank(account.Diligence);
            int expression_rank = LevelSystem.SocialStats.CalculateExpressionRank(account.Expression);

            int initial_x_value = 357;
            int initial_y_value = 548;
            int shadow_y_value = 0;
            int x = 0;
            int y = 0;

            int bar_width = 60;
            int bar_height = 18;
            int bar_shadow_height = 2;
            int bar_width_distance = 73;
            int bar_height_distance = 154;

            using (Graphics graphics = Graphics.FromImage(new_bitmap))
            {
                for (int i = 0; i < proficiency_rank; i++)
                {
                    x = initial_x_value + (bar_width_distance * i);
                    y = initial_y_value;
                    shadow_y_value = y + 16;

                    if (proficiency_rank == 5)
                    {
                        graphics.DrawImage(CreateRankBar(bar_width, bar_height, tms_yellow), x, y, bar_width, bar_height);
                        graphics.DrawImage(CreateRankBar(bar_width, bar_shadow_height, tms_yellow_shadow), x, shadow_y_value, bar_width, bar_shadow_height);
                    }
                    else
                    {
                        graphics.DrawImage(CreateRankBar(bar_width, bar_height, tms_white), x, y, bar_width, bar_height);
                        graphics.DrawImage(CreateRankBar(bar_width, bar_shadow_height, tms_white_shadow), x, shadow_y_value, bar_width, bar_shadow_height);
                    }
                }

                for (int i = 0; i < diligence_rank; i++)
                {
                    x = initial_x_value + (bar_width_distance * i);
                    y = initial_y_value + bar_height_distance;
                    shadow_y_value = y + 16;

                    if (diligence_rank == 5)
                    {
                        graphics.DrawImage(CreateRankBar(bar_width, bar_height, tms_yellow), x, y, bar_width, bar_height);
                        graphics.DrawImage(CreateRankBar(bar_width, bar_shadow_height, tms_yellow_shadow), x, shadow_y_value, bar_width, bar_shadow_height);
                    }
                    else
                    {
                        graphics.DrawImage(CreateRankBar(bar_width, bar_height, tms_white), x, y, bar_width, bar_height);
                        graphics.DrawImage(CreateRankBar(bar_width, bar_shadow_height, tms_white_shadow), x, shadow_y_value, bar_width, bar_shadow_height);
                    }
                }

                for (int i = 0; i < expression_rank; i++)
                {
                    x = initial_x_value + (bar_width_distance * i);
                    y = initial_y_value + bar_height_distance + bar_height_distance;
                    shadow_y_value = y + 16;

                    if (expression_rank == 5)
                    {
                        graphics.DrawImage(CreateRankBar(bar_width, bar_height, tms_yellow), x, y, bar_width, bar_height);
                        graphics.DrawImage(CreateRankBar(bar_width, bar_shadow_height, tms_yellow_shadow), x, shadow_y_value, bar_width, bar_shadow_height);
                    }
                    else
                    {
                        graphics.DrawImage(CreateRankBar(bar_width, bar_height, tms_white), x, y, bar_width, bar_height);
                        graphics.DrawImage(CreateRankBar(bar_width, bar_shadow_height, tms_white_shadow), x, shadow_y_value, bar_width, bar_shadow_height);
                    }
                }
            }

            return new_bitmap;
        }

        public static Bitmap RenderRankProgressBar(UserInfoFields account, string social_stat, int x, int y)
        {
            // Create a working space bitmap
            Bitmap new_bitmap = new Bitmap(template_width, template_height);

            int bar_max_value = 0;
            int bar_filled_value = 0;

            Rectangle main_bar = new Rectangle(x, y, 352, 18);
            Rectangle shadow_bar = new Rectangle(x, y + 16, 352, 2);

            System.Drawing.Color tms_white = System.Drawing.Color.White;
            SolidBrush tms_white_brush = new SolidBrush(tms_white);

            System.Drawing.Color tms_yellow = System.Drawing.Color.FromArgb(255, 255, 3);
            SolidBrush tms_yellow_brush = new SolidBrush(tms_yellow);

            System.Drawing.Color tms_yellow_shadow = System.Drawing.Color.FromArgb(117, 112, 21);
            SolidBrush tms_yellow_shadow_brush = new SolidBrush(tms_yellow_shadow);

            int total_points = 0;
            int next_rank_points_required = 0;
            int current_rank_points_required = 0;
            int account_proficiency_rank = LevelSystem.SocialStats.CalculateProficiencyRank(account.Proficiency);
            int account_diligence_rank = LevelSystem.SocialStats.CalculateDiligenceRank(account.Diligence);
            int account_expression_rank = LevelSystem.SocialStats.CalculateExpressionRank(account.Expression);

            switch (social_stat)
            {
                case "Proficiency":
                    total_points = account.Proficiency;
                    current_rank_points_required = Get_Base_Proficiency_Of_Current_Rank(account_proficiency_rank);
                    next_rank_points_required = Get_Base_Proficiency_Of_Next_Rank(account_proficiency_rank);
                    break;

                case "Diligence":
                    total_points = account.Diligence;
                    current_rank_points_required = Get_Base_Diligence_Of_Current_Rank(account_diligence_rank);
                    next_rank_points_required = Get_Base_Diligence_Of_Next_Rank(account_diligence_rank);
                    break;

                case "Expression":
                    total_points = account.Expression;
                    current_rank_points_required = Get_Base_Expression_Of_Current_Rank(account_expression_rank);
                    next_rank_points_required = Get_Base_Expression_Of_Next_Rank(account_expression_rank);
                    break;
            }

            bar_max_value = next_rank_points_required - current_rank_points_required;

            // Draw a progress bar that has a max value set to the minimum total EXP requirement of the next level minus the minimum total EXP requirement of the current level
            ProgressBar progress_bar = new ProgressBar(tms_white_brush, main_bar, bar_max_value);

            if ((social_stat == "Proficiency" && account_proficiency_rank >= 5) ||
                (social_stat == "Diligence" && account_diligence_rank >= 5) ||
                (social_stat == "Expression" && account_expression_rank >= 5))
            {
                //If the user's rank is 5, set the progress bar's max value to 1
                progress_bar = new ProgressBar(tms_yellow_brush, main_bar, 1);

                // Fill the progress bar completely up
                bar_filled_value = 1;
                progress_bar.SetCurrent(bar_filled_value);
            }
            else
            {
                // If the rank is below 5, fill the progress bar by subtracting the total points the user has by the minimum total point requirement of the current rank
                bar_filled_value = total_points - current_rank_points_required;
                progress_bar.SetCurrent(bar_filled_value);
            }

            // Use a graphics object to edit the bitmap
            using (Graphics graphics = Graphics.FromImage(new_bitmap))
            {
                // Draw the progress bar to the bitmap
                graphics.DrawImage(progress_bar.GiveGraphic(), progress_bar.GiveCorner());
            }

            return new_bitmap;
        }

        public static Bitmap CombineSocialStatRankBitmaps(UserInfoFields account)
        {
            Bitmap base_bitmap = new Bitmap(template_width, template_height);
            System.Drawing.Color tms_yellow = System.Drawing.Color.FromArgb(255, 255, 3);

            int area_x = 357;
            int area_y = 824;
            int area_width = 488;
            int area_height = 859;

            using (Graphics graphics = Graphics.FromImage(base_bitmap))
            {
                graphics.DrawImage(RenderRankBarLayer(account), 0, 0, template_width, template_height);
                graphics.DrawImage(RenderRankProgressBar(account, "Proficiency", 357, 589), 0, 0, template_width, template_height);
                graphics.DrawImage(RenderRankProgressBar(account, "Diligence", 357, 743), 0, 0, template_width, template_height);
                graphics.DrawImage(RenderRankProgressBar(account, "Expression", 357, 897), 0, 0, template_width, template_height);

                graphics.DrawImage(KeepOverlapWithMatchedColor(base_bitmap, CreateMaxRankShineBitmap("Proficiency"), tms_yellow, area_x, area_y, area_width, area_height), 0, 0, template_width, template_height);
                graphics.DrawImage(KeepOverlapWithMatchedColor(base_bitmap, CreateMaxRankShineBitmap("Diligence"), tms_yellow, area_x, area_y, area_width, area_height), 0, 0, template_width, template_height);
                graphics.DrawImage(KeepOverlapWithMatchedColor(base_bitmap, CreateMaxRankShineBitmap("Expression"), tms_yellow, area_x, area_y, area_width, area_height), 0, 0, template_width, template_height);
            }

            return base_bitmap;
        }

        // Asset Creation
        public static Bitmap CreateRankBar(int width, int height, System.Drawing.Color color)
        {
            Bitmap new_bitmap = new Bitmap(width, height);

            using (Graphics graphics = Graphics.FromImage(new_bitmap))
            {
                graphics.Clear(color);
            }

            return new_bitmap;
        }

        public static Bitmap CreateMaxRankShineBitmap(string social_stat)
        {
            Bitmap base_bitmap = new Bitmap(template_width, template_height);
            Bitmap stat_shine = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//Profile//StatusScreens//Decor//Decor_TMS_Kiria_1//stat_shine.png");

            Random rnd = new Random();
            int x = rnd.Next(297, 650);
            int y = 0;

            switch (social_stat)
            {
                case "Proficiency":
                    y = 548;
                    break;

                case "Diligence":
                    y = 702;
                    break;

                case "Expression":
                    y = 856;
                    break;
            }

            using (Graphics graphics = Graphics.FromImage(base_bitmap))
            {
                graphics.DrawImage(stat_shine, x, y, stat_shine.Width, stat_shine.Height);
            }

            return base_bitmap;
        }

        // Utility
        public static string Get_Class_Title(int proficiency_rank, int diligence_rank, int expression_rank)
        {
            string title = "";
            int maxed_stat_count = 0;
            int[] rank_array = new int[] { proficiency_rank, diligence_rank, expression_rank };

            foreach (int num in rank_array)
            {
                if (num == 5)
                {
                    maxed_stat_count++;
                }
            }

            switch (maxed_stat_count)
            {
                case 0:
                    title = "Top of the Charts";
                    break;

                case 1:
                    title = "A New Charisma";
                    break;

                case 2:
                    title = "Cool or Cute?";
                    break;

                case 3:
                    title = "A New Kiria";
                    break;
            }

            return title;
        }

        public static int Get_Base_Proficiency_Of_Current_Rank(int rank)
        {
            int required_points = 0;

            switch (rank)
            {
                case 1:
                    required_points = LevelSystem.SocialStatRanks.proficiency_rank_1_min;
                    break;

                case 2:
                    required_points = LevelSystem.SocialStatRanks.proficiency_rank_2_min;
                    break;

                case 3:
                    required_points = LevelSystem.SocialStatRanks.proficiency_rank_3_min;
                    break;

                case 4:
                    required_points = LevelSystem.SocialStatRanks.proficiency_rank_4_min;
                    break;

                case 5:
                    required_points = LevelSystem.SocialStatRanks.proficiency_rank_5_min;
                    break;
            }

            return required_points;
        }

        public static int Get_Base_Diligence_Of_Current_Rank(int rank)
        {
            int required_points = 0;

            switch (rank)
            {
                case 1:
                    required_points = LevelSystem.SocialStatRanks.diligence_rank_1_min;
                    break;

                case 2:
                    required_points = LevelSystem.SocialStatRanks.diligence_rank_2_min;
                    break;

                case 3:
                    required_points = LevelSystem.SocialStatRanks.diligence_rank_3_min;
                    break;

                case 4:
                    required_points = LevelSystem.SocialStatRanks.diligence_rank_4_min;
                    break;

                case 5:
                    required_points = LevelSystem.SocialStatRanks.diligence_rank_5_min;
                    break;
            }

            return required_points;
        }

        public static int Get_Base_Expression_Of_Current_Rank(int rank)
        {
            int required_points = 0;

            switch (rank)
            {
                case 1:
                    required_points = LevelSystem.SocialStatRanks.expression_rank_1_min;
                    break;

                case 2:
                    required_points = LevelSystem.SocialStatRanks.expression_rank_2_min;
                    break;

                case 3:
                    required_points = LevelSystem.SocialStatRanks.expression_rank_3_min;
                    break;

                case 4:
                    required_points = LevelSystem.SocialStatRanks.expression_rank_4_min;
                    break;

                case 5:
                    required_points = LevelSystem.SocialStatRanks.expression_rank_5_min;
                    break;
            }

            return required_points;
        }

        public static int Get_Base_Proficiency_Of_Next_Rank(int rank)
        {
            int required_points = 0;

            switch (rank)
            {
                case 1:
                    required_points = LevelSystem.SocialStatRanks.proficiency_rank_2_min;
                    break;

                case 2:
                    required_points = LevelSystem.SocialStatRanks.proficiency_rank_3_min;
                    break;

                case 3:
                    required_points = LevelSystem.SocialStatRanks.proficiency_rank_4_min;
                    break;

                case 4:
                    required_points = LevelSystem.SocialStatRanks.proficiency_rank_5_min;
                    break;

                case 5:
                    required_points = LevelSystem.SocialStatRanks.proficiency_rank_5_min;
                    break;
            }

            return required_points;
        }

        public static int Get_Base_Diligence_Of_Next_Rank(int rank)
        {
            int required_points = 0;

            switch (rank)
            {
                case 1:
                    required_points = LevelSystem.SocialStatRanks.diligence_rank_2_min;
                    break;

                case 2:
                    required_points = LevelSystem.SocialStatRanks.diligence_rank_4_min;
                    break;

                case 3:
                    required_points = LevelSystem.SocialStatRanks.diligence_rank_4_min;
                    break;

                case 4:
                    required_points = LevelSystem.SocialStatRanks.diligence_rank_5_min;
                    break;

                case 5:
                    required_points = LevelSystem.SocialStatRanks.diligence_rank_5_min;
                    break;
            }

            return required_points;
        }

        public static int Get_Base_Expression_Of_Next_Rank(int rank)
        {
            int required_points = 0;

            switch (rank)
            {
                case 1:
                    required_points = LevelSystem.SocialStatRanks.expression_rank_2_min;
                    break;

                case 2:
                    required_points = LevelSystem.SocialStatRanks.expression_rank_3_min;
                    break;

                case 3:
                    required_points = LevelSystem.SocialStatRanks.expression_rank_4_min;
                    break;

                case 4:
                    required_points = LevelSystem.SocialStatRanks.expression_rank_5_min;
                    break;

                case 5:
                    required_points = LevelSystem.SocialStatRanks.expression_rank_5_min;
                    break;
            }

            return required_points;
        }

        public static Bitmap KeepOverlapWithMatchedColor(Bitmap bottom_bitmap, Bitmap top_bitmap, System.Drawing.Color matching_color, int x_start, int x_end, int y_start, int y_end)
        {
            System.Drawing.Color bottom_pixel_color;
            System.Drawing.Color top_pixel_color;

            // Make an empty bitmap the same size as scrBitmap
            Bitmap newBitmap = new Bitmap(bottom_bitmap.Width, bottom_bitmap.Height);

            // Establish a nested loop that iterates on the X and Y axis of the image within a region
            for (int i = x_start; i < x_end; i++)
            {
                for (int j = y_start; j < y_end; j++)
                {
                    // Get a pixel from the same position on both bitmaps
                    bottom_pixel_color = bottom_bitmap.GetPixel(i, j);
                    top_pixel_color = top_bitmap.GetPixel(i, j);

                    // If both pixels have an alpha value above 0 and overlap, draw the top pixel to the new bitmap
                    if (bottom_pixel_color.A > 0 && top_pixel_color.A > 0 && bottom_pixel_color == matching_color)
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
            Bitmap prestige_overlay = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//Profile//StatusScreens//Decor//Decor_TMS_Kiria_1//prestige_counter.png");

            // Copy the star to mark prestige to a bitmap.
            Bitmap prestige_star = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//Profile//StatusScreens//Decor//Decor_TMS_Kiria_1//star.png");

            // Create a new bitmap.
            Bitmap new_bitmap = new Bitmap(template_width, template_height);

            // Use a graphics object to edit the bitmap.
            using (Graphics graphics = Graphics.FromImage(new_bitmap))
            {
                // Draw the prestige overlay to the template.
                graphics.DrawImage(prestige_overlay, 0, 0, template_width, template_height);

                // Create a new int variable that shares the same value as level_resets.
                int star_counter = level_resets;

                // Draw as many stars to the prestige overlay as needed.
                for (int i = 0; i < star_counter; i++)
                {
                    graphics.DrawImage(prestige_star, 1747 + (i * 48), 18, prestige_star.Width, prestige_star.Height);
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
                IconUrl = "https://i.imgur.com/kNLRxdB.png"
            };

            embed.WithAuthor(author);
            embed.WithColor(194, 219, 6);
            embed.WithThumbnailUrl("https://i.imgur.com/LIrCDC0.gif");
            embed.WithDescription("This may take a few seconds!");

            return embed;
        }
    }
}
