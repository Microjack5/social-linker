using System;
using System.Drawing;
using System.Threading.Tasks;
using Discord.Commands;
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
    public class RenderBBTAG : ModuleBase<SocketCommandContext>
    {
        int template_width = 1920;
        int template_height = 1080;

        public async Task Render_Quick_Scene_BBTAG(SocialLinkerCommand sl_command, OfficialSetData set_data, MakerCommandData command_data)
        {
            SocketUser user = sl_command.User;
            SocketTextChannel channel = (SocketTextChannel)sl_command.Channel;

            RestUserMessage loader = await channel.SendMessageAsync("", false, BBTAG_Loading_Message(set_data.Series).Build());

            var account = UserInfoClasses.GetAccount(user);
            BustupData bustup_data = BustupDataMethods.Get_Bustup_Data(account, set_data, command_data);

            // Background rendering
            Bitmap base_template = new Bitmap(template_width, template_height);
            Bitmap colored_background_bitmap = OfficialSetMethods.Render_Colored_Background(account, template_width, template_height);
            Bitmap background = new Bitmap(2, 2);

            try
            {
                background = OfficialSetMethods.Render_Background(sl_command, template_width, template_height);
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

            if (command_data.Base_Sprite != 0)
            {
                bustup = OfficialSetMethods.Bustup_Selection(sl_command, account, set_data, bustup_data, command_data);
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
                string display_name = OfficialSetMethods.GetDisplayName(account, command_data, set_data, bustup_data);
                DateTime user_time = Get_Date(sl_command, account);
                Bitmap header = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//BBTAG//Main//layer_1.png");
                Bitmap nametag = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//BBTAG//Main//Nametag//{Series_To_Nametag(set_data.Series)}.png");
                Bitmap rendered_name = Render_Name(display_name);
                Bitmap chapter_banner = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//BBTAG//Main//Chapter_Banner//{Get_Chapter_Banner(account)}//{Get_Day_Of_Week(account, user_time)}.png");
                Bitmap textbox = new Bitmap(2, 2);

                graphics.DrawImage(colored_background_bitmap, 0, 0, template_width, template_height);
                graphics.DrawImage(background, 0, 0, template_width, template_height);
                graphics.DrawImage(header, 0, 0, template_width, template_height);
                graphics.DrawImage(chapter_banner, 704, 33, 512, 128);

                // Draw the character bust-up to the template if the base sprite number is not '0'.
                if (command_data.Base_Sprite != 0)
                {
                    textbox = Get_Message_Window(account);
                    Bitmap placed_bustup = Set_Bustup_Placement(account, bustup, bustup_data, set_data);
                    graphics.DrawImage(placed_bustup, 0, 0, template_width, template_height);
                }
                else
                {
                    textbox = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//BBTAG//Main//Message_Window//textbox_none.png");
                }

                graphics.DrawImage(textbox, 0, 0, template_width, template_height);
                graphics.DrawImage(nametag, 0, 0, template_width, template_height);
                graphics.DrawImage(rendered_name, 0, 0, template_width, template_height);
                graphics.DrawImage(Render_Dialogue(command_data.Dialogue), 0, 0, template_width, template_height);
            }

            // Save the entire base template to a data stream.
            MemoryStream memoryStream = new MemoryStream();
            base_template.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
            memoryStream.Seek(0, SeekOrigin.Begin);

            try
            {
                await sl_command.Channel.SendFileAsync(memoryStream, $"scene_{sl_command.User.Id}_{DateTime.UtcNow}.png");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                _ = ErrorHandling.Image_Upload_Failed(sl_command);
                memoryStream.Dispose();
                await loader.DeleteAsync();
                return;
            }

            memoryStream.Dispose();
            await loader.DeleteAsync();

            if (account.Auto_Delete_Commands == "On")
            {
                await sl_command.Message.DeleteAsync();
            }
        }

        public async Task Render_System_Message(SocialLinkerCommand sl_command, MakerCommandData command_data)
        {
            SocketUser user = sl_command.User;
            SocketTextChannel channel = (SocketTextChannel)sl_command.Channel;

            RestUserMessage loader = await channel.SendMessageAsync("", false, BBTAG_Loading_Message("BlazBlue").Build());

            var account = UserInfoClasses.GetAccount(user);

            // Background rendering
            Bitmap base_template = new Bitmap(template_width, template_height);
            Bitmap colored_background_bitmap = OfficialSetMethods.Render_Colored_Background(account, template_width, template_height);
            Bitmap background = new Bitmap(2, 2);

            try
            {
                background = OfficialSetMethods.Render_Background(sl_command, template_width, template_height);
            }
            catch (System.ArgumentException e)
            {
                Console.WriteLine(e);
                await loader.DeleteAsync();
                _ = ErrorHandling.Incompatible_File_Type(sl_command);
                return;
            }

            if (account.BBTAG_TS_BG_Blur == "On")
            {
                background = Blur_Background(background);
            }

            // Time to put it all together!
            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                DateTime user_time = Get_Date(sl_command, account);
                Bitmap header = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//BBTAG//Main//layer_1.png");
                Bitmap chapter_banner = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//BBTAG//Main//Chapter_Banner//{Get_Chapter_Banner(account)}//{Get_Day_Of_Week(account, user_time)}.png");
                Bitmap textbox = new Bitmap(2, 2);

                graphics.DrawImage(colored_background_bitmap, 0, 0, template_width, template_height);
                graphics.DrawImage(background, 0, 0, template_width, template_height);
                graphics.DrawImage(header, 0, 0, template_width, template_height);
                graphics.DrawImage(chapter_banner, 704, 33, 512, 128);

                // Draw the character bust-up to the template if the base sprite number is not '0'.
                if (command_data.Base_Sprite == 0)
                {
                    textbox = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//BBTAG//Main//Message_Window//system_1.png");
                }
                else
                {
                    textbox = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//BBTAG//Main//Message_Window//system_2.png");
                }

                graphics.DrawImage(textbox, 0, 0, template_width, template_height);
                graphics.DrawImage(Render_Dialogue(command_data.Dialogue), 0, 0, template_width, template_height);
            }

            // Save the entire base template to a data stream.
            MemoryStream memoryStream = new MemoryStream();
            base_template.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
            memoryStream.Seek(0, SeekOrigin.Begin);

            try
            {
                await sl_command.Channel.SendFileAsync(memoryStream, $"scene_{sl_command.User.Id}_{DateTime.UtcNow}.png");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                _ = ErrorHandling.Image_Upload_Failed(sl_command);
                memoryStream.Dispose();
                await loader.DeleteAsync();
                return;
            }

            memoryStream.Dispose();
            await loader.DeleteAsync();

            if (account.Auto_Delete_Commands == "On")
            {
                await sl_command.Message.DeleteAsync();
            }
        }

        public Bitmap Set_Bustup_Placement(UserInfoFields account, Bitmap bustup, BustupData bustup_data, OfficialSetData set_data)
        {
            // Create a starting base bitmap to render all graphics on.
            Bitmap base_template = new Bitmap(template_width, template_height);

            // Switch the rendering position of the bustup depending on the user's settings.
            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                switch (account.BBTAG_TS_Position)
                {
                    case "Left":
                        graphics.DrawImage(bustup, bustup_data.BBTAG_Left_Coord_X, bustup_data.BBTAG_Left_Coord_Y, bustup_data.BBTAG_Scale_Width, bustup_data.BBTAG_Scale_Height);
                        break;

                    case "Right":
                        graphics.DrawImage(bustup, bustup_data.BBTAG_Right_Coord_X, bustup_data.BBTAG_Right_Coord_Y, bustup_data.BBTAG_Scale_Width, bustup_data.BBTAG_Scale_Height);
                        break;

                    case "Center":
                        graphics.DrawImage(bustup, bustup_data.BBTAG_Enlarged_Coord_X, bustup_data.BBTAG_Enlarged_Coord_Y, bustup_data.BBTAG_Enlarged_Scale_Width, bustup_data.BBTAG_Enlarged_Scale_Height);
                        break;
                }
            }

            return base_template;
        }

        public Bitmap Get_Message_Window(UserInfoFields account)
        {
            // Create a starting base bitmap to render all graphics on.
            Bitmap base_template = new Bitmap(template_width, template_height);

            // Switch the rendering position of the bustup depending on the user's settings.
            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                switch (account.BBTAG_TS_Position)
                {
                    case "Left":
                        return (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//BBTAG//Main//Message_Window//textbox_left.png");

                    case "Right":
                        return (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//BBTAG//Main//Message_Window//textbox_right.png");

                    case "Center":
                        return (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//BBTAG//Main//Message_Window//textbox_center.png");
                }
            }

            return base_template;
        }

        public Bitmap Render_Name(string display_name)
        {
            Bitmap base_template = new Bitmap(template_width, template_height);

            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                using (Font bbtagFont = new Font("URWScenarioW01-Bold", 50, FontStyle.Bold))
                {
                    // Create a GraphicsPath object.
                    GraphicsPath myPath = new GraphicsPath();

                    // Set up all the string parameters.
                    string stringText = display_name;

                    System.Drawing.FontFamily family = new System.Drawing.FontFamily("URWScenarioW01-Bold");
                    int fontStyle = (int)FontStyle.Bold;
                    int emSize = 47;
                    Point origin = new Point(380, 800);

                    StringFormat stringFormat = new StringFormat();
                    stringFormat.Alignment = StringAlignment.Center;
                    stringFormat.LineAlignment = StringAlignment.Center;

                    // Add the string to the path.
                    myPath.AddString(stringText,
                        family,
                        fontStyle,
                        emSize = (int)graphics.DpiY * 28 / 72,
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

            return base_template;
        }

        public Bitmap Blur_Background(Bitmap background)
        {
            Bitmap base_template = new Bitmap(template_width, template_height);
            Bitmap fog_layer = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//BBTAG//Main//story_bg_grad00.png");

            base_template = Blur(background, new Rectangle(0, 0, template_width, template_height), 4);

            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                graphics.DrawImage(fog_layer, 0, 0, template_width, template_height);
            }

            return base_template;
        }

        // Method from https://stackoverflow.com/questions/44827093/how-to-apply-blur-effect-on-a-bitmap-image-in-c
        private unsafe static Bitmap Blur(Bitmap image, Rectangle rectangle, Int32 blurSize)
        {
            Bitmap blurred = new Bitmap(image.Width, image.Height);

            // make an exact copy of the bitmap provided
            using (Graphics graphics = Graphics.FromImage(blurred))
                graphics.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height),
                    new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);

            // Lock the bitmap's bits
            BitmapData blurredData = blurred.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadWrite, blurred.PixelFormat);

            // Get bits per pixel for current PixelFormat
            int bitsPerPixel = System.Drawing.Image.GetPixelFormatSize(blurred.PixelFormat);

            // Get pointer to first line
            byte* scan0 = (byte*)blurredData.Scan0.ToPointer();

            // look at every pixel in the blur rectangle
            for (int xx = rectangle.X; xx < rectangle.X + rectangle.Width; xx++)
            {
                for (int yy = rectangle.Y; yy < rectangle.Y + rectangle.Height; yy++)
                {
                    int avgR = 0, avgG = 0, avgB = 0;
                    int blurPixelCount = 0;

                    // average the color of the red, green and blue for each pixel in the
                    // blur size while making sure you don't go outside the image bounds
                    for (int x = xx; (x < xx + blurSize && x < image.Width); x++)
                    {
                        for (int y = yy; (y < yy + blurSize && y < image.Height); y++)
                        {
                            // Get pointer to RGB
                            byte* data = scan0 + y * blurredData.Stride + x * bitsPerPixel / 8;

                            avgB += data[0]; // Blue
                            avgG += data[1]; // Green
                            avgR += data[2]; // Red

                            blurPixelCount++;
                        }
                    }

                    avgR = avgR / blurPixelCount;
                    avgG = avgG / blurPixelCount;
                    avgB = avgB / blurPixelCount;

                    // now that we know the average for the blur size, set each pixel to that color
                    for (int x = xx; x < xx + blurSize && x < image.Width && x < rectangle.Width; x++)
                    {
                        for (int y = yy; y < yy + blurSize && y < image.Height && y < rectangle.Height; y++)
                        {
                            // Get pointer to RGB
                            byte* data = scan0 + y * blurredData.Stride + x * bitsPerPixel / 8;

                            // Change values
                            data[0] = (byte)avgB;
                            data[1] = (byte)avgG;
                            data[2] = (byte)avgR;
                        }
                    }
                }
            }

            // Unlock the bits
            blurred.UnlockBits(blurredData);

            return blurred;
        }

        // Render_Dialogue method is legacy code from the prototype build
        public Bitmap Render_Dialogue(string dialogue)
        {
            Bitmap base_template = new Bitmap(template_width, template_height);

            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                using (Font bbtagFont = new Font("URWScenarioW01-Bold", 50, FontStyle.Bold))
                {
                    var left = 960;
                    var top = 888;
                    var mySpacing = -40;

                    string line_1 = "";
                    string line_2 = "";
                    string line_3 = "";
                    float counter = 0;

                    GraphicsPath myPath = new GraphicsPath();

                    //create a bmp / graphic to use MeasureString on
                    Bitmap b = new Bitmap(1920, 1080);
                    Graphics g = Graphics.FromImage(b);

                    //measure the string
                    SizeF naturalSizeOfString = g.MeasureString(dialogue, bbtagFont);
                    SizeF sizeOfString = new SizeF();
                    SizeF sizeLine1 = new SizeF();
                    SizeF sizeLine2 = new SizeF();
                    SizeF sizeLine3 = new SizeF();

                    string[] wordArray = dialogue.Split(' ');

                    for (int i = 0; i < wordArray.Length; i++)
                    {
                        sizeOfString = g.MeasureString(wordArray[i] + " ", bbtagFont);
                        counter = counter + sizeOfString.Width;

                        if (wordArray[i] == "|")
                        {
                            if (counter < 2600)
                            {
                                counter = 2600;
                                i++;
                            }
                            else if (counter < 5200)
                            {
                                counter = 5200;
                                i++;
                            }
                        }

                        if (counter < 2600)
                        {
                            line_1 += wordArray[i] + " ";
                        }
                        else if (counter < 5200)
                        {
                            line_2 += wordArray[i] + " ";
                        }
                        else if (counter < 7800)
                        {
                            line_3 += wordArray[i] + " ";
                        }
                    }

                    sizeLine1 = g.MeasureString(line_1, new Font("URWScenarioW01-Bold", 50, FontStyle.Bold, GraphicsUnit.Pixel));
                    sizeLine2 = g.MeasureString(line_2, new Font("URWScenarioW01-Bold", 50, FontStyle.Bold, GraphicsUnit.Pixel));
                    sizeLine3 = g.MeasureString(line_3, new Font("URWScenarioW01-Bold", 50, FontStyle.Bold, GraphicsUnit.Pixel));

                    var myLines = new List<string>
                        {
                            $"{line_1}",
                            $"{line_2}",
                            $"{line_3}",
                        };

                    for (var i = 0; i < myLines.Count; i++)
                    {
                        var lineText = myLines[i];
                        var lineImageSize = graphics.MeasureString(lineText, bbtagFont);

                        System.Drawing.FontFamily family = new System.Drawing.FontFamily("URWScenarioW01-Bold");
                        int fontStyle = (int)FontStyle.Bold;
                        int emSize = 47;
                        Point origin = new Point(left, top + (int)(i * (lineImageSize.Height + mySpacing)));

                        StringFormat stringFormat = new StringFormat();

                        if ((line_2 != "") && (naturalSizeOfString.Width > 2500) &&
                            ((sizeLine1.Width > 1500) || (sizeLine2.Width > 1500) || (sizeLine3.Width > 1500)))
                        {
                            origin = new Point(230, top + (int)(i * (lineImageSize.Height + mySpacing)));
                            stringFormat.Alignment = StringAlignment.Near;
                        }
                        else
                        {
                            stringFormat.Alignment = StringAlignment.Center;
                        }

                        stringFormat.LineAlignment = StringAlignment.Center;

                        // Add the string to the path.
                        myPath.AddString(myLines[i],
                            family,
                            fontStyle,
                            emSize = (int)graphics.DpiY * 28 / 72,
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
            }

            return base_template;
        }

        public static DateTime Get_Date(SocialLinkerCommand sl_command, UserInfoFields account)
        {
            // Create an empty string variable. This is where the API request will be stored.
            string json_location = "";

            // Create a default DateTime variable.
            // We'll use this to store the user's set time later.
            DateTime user_time = default;

            // Encapsulate the API request in a try-catch block. If the user inputs an invalid parameter, an exception will be thrown.
            try
            {
                // Make an API request with the account key and user input as parameters.
                using (WebClient client = new WebClient())
                {
                    json_location = new TimedWebClient { Timeout = Global.API_Timeout }.DownloadString($"http://api.weatherapi.com/v1/current.json?key={WeatherAPIConfig.weather_api_account.accountKey}&q={account.City}");
                }

                // Deserialize the JSON object and store it in a variable.
                var dataObject = JsonConvert.DeserializeObject<dynamic>(json_location);

                // Set the user's time to set location.
                user_time = dataObject.location.localtime;
            }
            catch (Exception ex)
            {
                // Log the error to the console.
                Console.WriteLine(ex);

                // Send a warning message to the user.
                _ = ErrorHandling.API_Timeout(sl_command);

                // Set the user's time to the current UTC time.
                user_time = DateTime.UtcNow;
            }

            return user_time;
        }

        public int Get_Day_Of_Week(UserInfoFields account, DateTime input_time)
        {
            if (account.BBTAG_TS_Header == "Prologue")
            {
                return 1;
            }

            string day_of_week = input_time.ToString("dddd").ToLower();

            switch(day_of_week)
            {
                case "sunday":
                    return 1;

                case "monday":
                    return 2;

                case "tuesday":
                    return 3;

                case "wednesday":
                    return 4;

                case "thursday":
                    return 5;

                case "friday":
                    return 6;

                case "saturday":
                    return 7;

                default:
                    return 1;
            }
        }

        public string Get_Chapter_Banner(UserInfoFields account)
        {
            switch (account.BBTAG_TS_Header)
            {
                case "Prologue":
                    return "Prologue";

                case "Episode BlazBlue":
                    return "Episode_BlazBlue";

                case "Episode P4A":
                    return "Episode_P4A";

                case "Episode Under Night In-Birth":
                    return "Episode_Under_Night_In-Birth";

                case "Episode RWBY":
                    return "Episode_RWBY";

                case "Episode Extra":
                    return "Episode_Extra";

                default:
                    return "Prologue";
            }
        }

        public string Series_To_Nametag(string series)
        {
            switch (series)
            {
                case "BlazBlue":
                    return "blazblue";

                case "Persona 4 Arena":
                    return "persona_4_arena";

                case "Under Night In-Birth":
                    return "under_night_in-birth";

                case "RWBY":
                    return "rwby";

                case "Arcana Heart":
                    return "arcana_heart";

                case "Senran Kagura":
                    return "senran_kagura";

                case "Akatsuki En-Eins":
                    return "akatsuki_en-eins";

                default:
                    return "_blank";
            }
        }

        public static EmbedBuilder BBTAG_Loading_Message(string series)
        {
            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = $"Generating Scene...",
                IconUrl = EmbedSettings.Get_Game_Logo("BBTAG")
            };

            embed.WithAuthor(author);
            embed.WithColor(EmbedSettings.Get_BBTAG_Series_Color(series));
            embed.WithThumbnailUrl(EmbedSettings.Get_Loading_Icon("BBTAG", null));
            embed.WithDescription("This may take a few seconds!");

            return embed;
        }
    }
}
