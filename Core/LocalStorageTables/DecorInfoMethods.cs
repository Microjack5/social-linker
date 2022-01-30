using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Newtonsoft.Json;
using System.Drawing;
using SocialLinker.Core.Menus;
using SocialLinker.Config;
using System.Drawing.Drawing2D;
using SocialLinker.Core.CloudStorageTables;
using Discord.WebSocket;

namespace SocialLinker.Core.LocalStorageTables
{
    public static class DecorInfoMethods
    {
        private static List<DecorInfoFields> decor_info_list = null;

        private const string configFolder = "Resources";
        private const string configFile = "DecorInfo.json";

        static DecorInfoMethods()
        {
            // If the file folder doesn't exist, create it.
            if (!Directory.Exists(configFolder))
            {
                Directory.CreateDirectory(configFolder);
            }

            // If the file exists, load its contents.
            if (File.Exists(configFolder + "/" + configFile))
            {
                decor_info_list = LoadDecorList(configFolder + "/" + configFile).ToList();
            }
            // If the file does not exist, create an empty one.
            else
            {
                decor_info_list = new List<DecorInfoFields>();
                SaveDecorList(decor_info_list, configFolder + "/" + configFile);
            }
        }

        public static void SaveDecorList(IEnumerable<DecorInfoFields> decor_list, string filePath)
        {
            string json = JsonConvert.SerializeObject(decor_list, Formatting.Indented, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore
            });
            File.WriteAllText(filePath, json);
        }

        public static IEnumerable<DecorInfoFields> LoadDecorList(string filePath)
        {
            if (!File.Exists(filePath)) return null;
            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<DecorInfoFields>>(json);
        }

        public static DecorInfoFields GetDecorInfo(string id)
        {
            var result = from a in decor_info_list
                         where a.Decor_ID == id
                         select a;

            var decor_info = result.FirstOrDefault();
            return decor_info;
        }

        public static List<string> Shop_Decor_By_Title_A_Z()
        {
            // Create an empty string list to return at the end of the method.
            List<string> return_list = new List<string>();

            // Create an empty décor list.
            List<DecorInfoFields> organized_decor_list = new List<DecorInfoFields>();

            // For each décor entry that exists in the JSON, add it to the empty décor list.
            foreach (DecorInfoFields s in decor_info_list)
            {
                organized_decor_list.Add(s);
            }

            // Order the newly formed décor list by title in alphabetical order.
            organized_decor_list = organized_decor_list.OrderBy(s => s.Title).ToList();

            // Finally, add the IDs of the décor in the organized décor list to the empty string list.
            foreach (DecorInfoFields s in organized_decor_list)
            {
                return_list.Add(s.Decor_ID);
            }

            // Return the string list.
            return return_list;
        }

        public static List<string> Shop_Decor_By_Title_Z_A()
        {
            // Create an empty string list to return at the end of the method.
            List<string> return_list = new List<string>();

            // Create an empty décor list.
            List<DecorInfoFields> organized_decor_list = new List<DecorInfoFields>();

            // For each décor entry that exists in the JSON, add it to the empty décor list.
            foreach (DecorInfoFields s in decor_info_list)
            {
                organized_decor_list.Add(s);
            }

            // Order the newly formed décor list by game title in reverse alphabetical order.
            organized_decor_list = organized_decor_list.OrderByDescending(s => s.Title).ToList();

            // Finally, add the IDs of the décor in the organized décor list to the empty string list.
            foreach (DecorInfoFields s in organized_decor_list)
            {
                return_list.Add(s.Decor_ID);
            }

            // Return the string list.
            return return_list;
        }

        public static List<string> Shop_Decor_By_Cost_Low_High()
        {
            // Create an empty string list to return at the end of the method.
            List<string> return_list = new List<string>();

            // Create an empty décor list.
            List<DecorInfoFields> organized_decor_list = new List<DecorInfoFields>();

            // For each décor entry that exists in the JSON, add it to the empty décor list.
            foreach (DecorInfoFields s in decor_info_list)
            {
                organized_decor_list.Add(s);
            }

            // Order the newly formed décor list by price from least expensive to most expensive.
            organized_decor_list = organized_decor_list.OrderBy(s => s.Price).ToList();

            // Finally, add the IDs of the décor in the organized décor list to the empty string list.
            foreach (DecorInfoFields s in organized_decor_list)
            {
                return_list.Add(s.Decor_ID);
            }

            // Return the string list.
            return return_list;
        }

        public static List<string> Shop_Decor_By_Cost_High_Low()
        {
            // Create an empty string list to return at the end of the method.
            List<string> return_list = new List<string>();

            // Create an empty décor list.
            List<DecorInfoFields> organized_decor_list = new List<DecorInfoFields>();

            // For each décor entry that exists in the JSON, add it to the empty décor list.
            foreach (DecorInfoFields s in decor_info_list)
            {
                organized_decor_list.Add(s);
            }

            // Order the newly formed décor list by price from most expensive to least expensive.
            organized_decor_list = organized_decor_list.OrderByDescending(s => s.Price).ToList();

            // Finally, add the IDs of the décor in the organized décor list to the empty string list.
            foreach (DecorInfoFields s in organized_decor_list)
            {
                return_list.Add(s.Decor_ID);
            }

            // Return the string list.
            return return_list;
        }

        public static List<string> Shop_Decor_By_Order_Added_Old_New()
        {
            // Create an empty string list to return at the end of the method.
            List<string> return_list = new List<string>();

            // For each décor entry that exists in the JSON, add it to the empty string list.
            foreach (DecorInfoFields s in decor_info_list)
            {
                return_list.Add(s.Decor_ID);
            }

            // Return the string list.
            return return_list;
        }

        public static List<string> Shop_Decor_By_Order_Added_New_Old()
        {
            // Create an empty string list to return at the end of the method.
            List<string> return_list = new List<string>();

            // For each décor entry that exists in the JSON, add it to the empty string list.
            foreach (DecorInfoFields s in decor_info_list)
            {
                return_list.Add(s.Decor_ID);
            }

            // Reverse the elements of the string list.
            return_list.Reverse();

            // Return the string list.
            return return_list;
        }

        public static Bitmap DecorPreviews(ItemListIterator itemSession, int sublist_length)
        {
            // First, determine how the items will be rendered on the bitmap based on the size of sublist_length.
            // This is done by taking the square root of the input sublist_length variable. Notice that the result will be a double at times and not a full integer.
            double sq_count = Math.Sqrt(sublist_length);

            // Determine how many columns and rows will be displayed on the bitmap.
            // Columns are determined by calculating the ceiling of the sq_count double variable.
            // Rows are calculated by converting the sq_count double straight to an int.
            int columns = (int)Math.Ceiling(sq_count);
            int rows = Convert.ToInt32(sq_count);

            // Create variables for the desired width and height of each décor item to be drawn on the template.
            int item_width = 475;
            int item_height = 268;

            // Set the width and height of the bitmap based on the desired dimensions of each décor thumbnail and the expected amounts of columns and rows.
            int newWidth = (columns * item_width);
            int newHeight = (rows * item_height);

            // Create a bitmap working space from the calculated newWidth and newHeight values.
            Bitmap base_template = new Bitmap(newWidth, newHeight);

            // Attempt to edit the bitmap with a graphics object. These operations could fail at times, so make sure it's in a try-catch block.
            try
            {
                // Use a graphics object to edit the bitmap
                using (Graphics graphics = Graphics.FromImage(base_template))
                {
                    // Create two int variables that represent the X and Y values on the base_template bitmap.
                    int x = 0;
                    int y = 0;

                    // Color the entire bitmap with a black color
                    graphics.Clear(System.Drawing.Color.FromArgb(0, 0, 0));

                    // Create an int to properly display the needed emotes when iterating through the item list.
                    int displayed_list_counter = 0;

                    // Iterate through the item list starting from the ItemBaseIndex and up until sublist_length.
                    for (int i = itemSession.ItemIndexBase; i < (itemSession.ItemIndexBase + sublist_length); i++)
                    {
                        // Increase the displayed_list_counter by one.
                        displayed_list_counter += 1;

                        // Get the information of the current décor iteration.
                        var decor_info = DecorInfoMethods.GetDecorInfo(itemSession.ItemList[i]);

                        // Create a bitmap variable.
                        Bitmap thumbnail = new Bitmap(2, 2);

                        // Attempt to gather the thumbnail associated with the current décor being iterated on and assign it to the bitmap variable.
                        try
                        {
                            thumbnail = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//Profile//StatusScreens//Decor//{decor_info.Decor_ID}//_Thumbnails//preview_1.png");
                        }
                        // If this fails, catch the exception and continue.
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }

                        // Draw the thumbnail to the base template at the current X and Y coordinate values.
                        graphics.DrawImage(thumbnail, x, y, item_width, item_height);

                        using (Font rockwell = new Font("Rockwell", 45, FontStyle.Bold))
                        {
                            // Create a GraphicsPath object.
                            GraphicsPath myPath = new GraphicsPath();

                            // Set up all the string parameters.
                            string stringText = $"{displayed_list_counter}";

                            System.Drawing.FontFamily family = new System.Drawing.FontFamily("Rockwell");
                            int fontStyle = (int)FontStyle.Bold;
                            int emSize = 23;
                            Point origin = new Point(x + 236, y + 237);

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

                        x = x + item_width + 5;

                        if (x >= newWidth)
                        {
                            x = 0;
                            y = y + item_height + 5;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return base_template;
        }

        public static List<string> CreateSortSettingList(string user_setting)
        {
            // Create an empty string list to return at the end of the method.
            List<string> return_list = new List<string>();

            if (user_setting == "title_a_z")
            {
                return_list = Shop_Decor_By_Title_A_Z();
            }
            else if (user_setting == "title_z_a")
            {
                return_list = Shop_Decor_By_Title_Z_A();
            }
            else if (user_setting == "cost_low_high")
            {
                return_list = Shop_Decor_By_Cost_Low_High();
            }
            else if (user_setting == "cost_high_low")
            {
                return_list = Shop_Decor_By_Cost_High_Low();
            }
            else if (user_setting == "release_old_new")
            {
                return_list = Shop_Decor_By_Order_Added_Old_New();
            }
            else if (user_setting == "release_new_old")
            {
                return_list = Shop_Decor_By_Order_Added_New_Old();
            }

            return return_list;
        }

        public static string SortSettingToString(string user_setting)
        {
            string return_value = "";

            if (user_setting == "title_a_z")
            {
                return_value = "By Title (A - Z)";
            }
            else if (user_setting == "title_z_a")
            {
                return_value = "By Title (Z - A)";
            }
            else if (user_setting == "cost_low_high")
            {
                return_value = "By Cost (Low - High)";
            }
            else if (user_setting == "cost_high_low")
            {
                return_value = "By Cost (High - Low)";
            }
            else if (user_setting == "release_old_new")
            {
                return_value = "By Release Order (Old - New)";
            }
            else if (user_setting == "release_new_old")
            {
                return_value = "By Release Order (New - Old)";
            }

            return return_value;
        }

        public static string GetDecorTitle(SocketUser user)
        {
            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            // Create an empty string variable. This will hold the title of the user's currently set décor.
            string set_decor_title = "";

            // Determine what the title will be based on the user's set décor and profile theme.
            // First, check if the user has a décor currently set.
            if (account.Decor_Setting != "")
            {
                // Get the information of the currently set décor.
                var decor_info = DecorInfoMethods.GetDecorInfo(account.Decor_Setting);

                // Set the string variable to the title of the user's set décor.
                set_decor_title = decor_info.Title;
            }
            // Else, check if the user has a profile theme currently set.
            else if (account.Profile_Theme != "")
            {
                // Create another empty string variable.
                string game_title = "";

                // Form a full title based on the user's profile theme and assign it to the string variable.
                if (account.Profile_Theme == "P3")
                {
                    game_title = "Persona 3";
                }
                else if (account.Profile_Theme == "P4")
                {
                    game_title = "Persona 4";
                }
                else if (account.Profile_Theme == "P5")
                {
                    game_title = "Persona 5";
                }

                // Use the initial empty string variable created and assign the full game title to it, alongside describing it as a décor.
                set_decor_title = $"{game_title} Décor";
            }
            // If the user neither has a set décor or profile theme, the title will be "None".
            else
            {
                set_decor_title = "None";
            }

            return set_decor_title;
        }

        public static List<string> RemoveBlockedContentFromList(List<string> decor_list, List<string> user_settings)
        {
            // Create a copy of the decor_list.
            List<string> edited_decor_list = decor_list;

            // Iterate through the decor_list. Here, we're going to attempt to remove any entries that match with the user's content filter.
            foreach (string decor_id in decor_list.ToList())
            {
                // Get the information of the current iterated décor.
                var decor_info = DecorInfoMethods.GetDecorInfo(decor_id);

                // Create an empty string list.
                List<string> decor_content = new List<string>();

                // Check if the Content field of the current décor is null.
                if (decor_info.Content != null)
                {
                    // If not, convert that field into a list and store it in the decor_content variable we just created.
                    decor_content = Newtonsoft.Json.JsonConvert.DeserializeObject<string[]>(decor_info.Content).ToList();
                }

                // Check if the decor_content list is null.
                if (decor_content == null)
                {
                    // If it is null, do nothing. We arent interested in editing it!
                }
                else
                {
                    // If it isn't null, it's time to check what's inside it and compare its contents to the user's content filter.
                    // Start by creating a loop that iterates through decor_content.
                    for (int i = 0; i < decor_content.Count; i++)
                    {
                        // Because we want to compare both the decor_content and user_content_filter lists at the same time, create a nested loop.
                        // This second loop will be for iterating through user_content_filter.
                        // For every entry in decor_content, they'll be compared to the user's entire content filter parameters.
                        for (int j = 0; j < user_settings.Count; j++)
                        {
                            // Check if the current iteration of décor content matches with an entry in the user's content filter.
                            if (decor_content[i] == user_settings[j])
                            {
                                // If so, remove the décor entry being iterated on from the edited_decor_list.
                                edited_decor_list.Remove(decor_id);
                                break;
                            }
                        }
                    }
                }
            }

            // Copy the edited_decor_list to the decor_list variable.
            // Any entries that conflicted with the user's content filter should now be removed.
            decor_list = edited_decor_list;

            // Return the decor_list variable.
            return decor_list;
        }

        public static List<string> StringToStringArray(string input_string)
        {
            // Create an empty string list variable.
            List<string> output_list;

            // Create a char array of characters the input_string should be split by.
            char[] delimiterChars = { ';' };

            // Split the input_string into a list dictated by the delimiterChars specified.
            output_list = input_string.Split(delimiterChars).ToList();

            // There may be times when an empty string is parsed into the list, so make sure they are removed before returning.
            output_list.RemoveAll(s => s == "");

            // Return the output_list variable.
            return output_list;
        }

        public static string NumberToKeycapEmoji(int number)
        {
            string return_value = "";

            switch(number)
            {
                case 0:
                    return_value = "\u0030\ufe0f\u20e3";
                    break;

                case 1:
                    return_value = "\u0031\ufe0f\u20e3";
                    break;

                case 2:
                    return_value = "\u0032\ufe0f\u20e3";
                    break;

                case 3:
                    return_value = "\u0033\ufe0f\u20e3";
                    break;

                case 4:
                    return_value = "\u0034\ufe0f\u20e3";
                    break;

                case 5:
                    return_value = "\u0035\ufe0f\u20e3";
                    break;

                case 6:
                    return_value = "\u0036\ufe0f\u20e3";
                    break;

                case 7:
                    return_value = "\u0037\ufe0f\u20e3";
                    break;

                case 8:
                    return_value = "\u0038\ufe0f\u20e3";
                    break;

                case 9:
                    return_value = "\u0039\ufe0f\u20e3";
                    break;

                case 10:
                    return_value = "🔟";
                    break;
            }

            return return_value;
        }

        // Method from https://stackoverflow.com/questions/2729752/converting-numbers-in-to-words-c-sharp
        public static string NumberToWords(int number)
        {
            if (number == 0)
                return "zero";

            if (number < 0)
                return "minus " + NumberToWords(Math.Abs(number));

            string words = "";

            if ((number / 1000000) > 0)
            {
                words += NumberToWords(number / 1000000) + " million ";
                number %= 1000000;
            }

            if ((number / 1000) > 0)
            {
                words += NumberToWords(number / 1000) + " thousand ";
                number %= 1000;
            }

            if ((number / 100) > 0)
            {
                words += NumberToWords(number / 100) + " hundred ";
                number %= 100;
            }

            if (number > 0)
            {
                if (words != "")
                    words += "and ";

                var unitsMap = new[] { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
                var tensMap = new[] { "zero", "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };

                if (number < 20)
                    words += unitsMap[number];
                else
                {
                    words += tensMap[number / 10];
                    if ((number % 10) > 0)
                        words += "-" + unitsMap[number % 10];
                }
            }

            return words;
        }
    }
}
