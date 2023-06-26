using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Discord.WebSocket;
using SocialLinker.Config;
using SocialLinker.Core.CloudStorageTables;
using SocialLinker.Core.LocalStorageTables;
using SocialLinker.Core.Menus.Settings.Main.SceneMaker.DisplayNames;

namespace SocialLinker.Core.Menus.Settings.Reactions.SceneMaker.DisplayNames
{
    internal class Display_Names_Sprite_Select_Reactions
    {
        public static Task Nav_Display_Names_Sprite_Select_Main(SocketReaction reaction, MenuIdStructure menuSession)
        {
            if (reaction.Emote.Name == "↩️")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Display_Names_Title_Select_Menu.Display_Names_Title_Select(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            else if (reaction.Emote.Name == "🔄")
            {
                try
                {
                    var account = UserInfoClasses.GetAccount((SocketUser)reaction.User);
                    var naming_session = Global.DisplayNameTempList.SingleOrDefault(x => x.User_ID == $"{menuSession.User.Id}");
                    List<int> int_range = Select_Entire_Sprite_Range(naming_session);

                    naming_session.Sprites_Affected = Int_Range_To_String_Range(account, naming_session.Sprite_Set, int_range);
                    naming_session.Spriteless_Included = "Yes";

                    if (DisplayNameLogging.Check_If_Sprites_Overlap(naming_session) == true)
                    {
                        // Stop the timeout timer associated with the menu.
                        menuSession.MenuTimer.Stop();

                        // Go to a new menu.
                        _ = Display_Names_Sprite_Select_Menu.Display_Names_Sprite_Select_Error_3(menuSession.User, menuSession.MenuMessage);
                        return Task.CompletedTask;
                    }

                    // Stop the timeout timer associated with the menu.
                    menuSession.MenuTimer.Stop();

                    // Go to a new menu.
                    _ = Display_Names_Custom_Input_Menu.Display_Names_Custom_Input_Main(menuSession.User, menuSession.MenuMessage);
                    return Task.CompletedTask;

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Display_Names_Sprite_Select_Error_1(SocketReaction reaction, MenuIdStructure menuSession)
        {
            if (reaction.Emote.Name == "↩️")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Display_Names_Sprite_Select_Menu.Display_Names_Sprite_Select_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Display_Names_Sprite_Select_Error_2(SocketReaction reaction, MenuIdStructure menuSession)
        {
            if (reaction.Emote.Name == "↩️")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Display_Names_Sprite_Select_Menu.Display_Names_Sprite_Select_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Display_Names_Sprite_Select_Error_3(SocketReaction reaction, MenuIdStructure menuSession)
        {
            if (reaction.Emote.Name == "↩️")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Display_Names_Sprite_Select_Menu.Display_Names_Sprite_Select_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        // Methods that activate on the MessageReceived event.
        public static Task Nav_Display_Names_Sprite_Select_Main_Received(SocketMessage message, MenuIdStructure menuSession)
        {
            try
            {
                var new_name_data = Global.DisplayNameTempList.SingleOrDefault(x => x.User_ID == $"{menuSession.User.Id}");

                var account = UserInfoClasses.GetAccount(message.Author);
                string input_string = message.Content;

                List<int> int_range = Input_Range_To_List(input_string);

                // Check if reading sprite input failed
                if (int_range.Count == 0)
                {
                    // Send error message
                    menuSession.MenuTimer.Stop();
                    _ = Display_Names_Sprite_Select_Menu.Display_Names_Sprite_Select_Error_1(menuSession.User, menuSession.MenuMessage);
                    return Task.CompletedTask;
                }

                int_range = Spriteless_Check(int_range, new_name_data);

                bool range_check = true;

                // Account for spriteless only options
                if (int_range.Count != 0)
                {
                    range_check = Sprite_Range_Validity_Check(new_name_data.Sprite_Set, int_range[int_range.Count - 1]);
                }

                if (range_check == false)
                {
                    // Stop the timeout timer associated with the menu.
                    menuSession.MenuTimer.Stop();

                    // Go to a new menu.
                    _ = Display_Names_Sprite_Select_Menu.Display_Names_Sprite_Select_Error_2(menuSession.User, menuSession.MenuMessage);
                    return Task.CompletedTask;
                }

                new_name_data.Sprites_Affected = Int_Range_To_String_Range(account, new_name_data.Sprite_Set, int_range);

                if (DisplayNameLogging.Check_If_Sprites_Overlap(new_name_data) == true)
                {
                    // Stop the timeout timer associated with the menu.
                    menuSession.MenuTimer.Stop();

                    // Go to a new menu.
                    _ = Display_Names_Sprite_Select_Menu.Display_Names_Sprite_Select_Error_3(menuSession.User, menuSession.MenuMessage);
                    return Task.CompletedTask;
                }

                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Display_Names_Custom_Input_Menu.Display_Names_Custom_Input_Main(menuSession.User, menuSession.MenuMessage);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return Task.CompletedTask;
        }

        // Utility
        public static List<int> Input_Range_To_List(string input_range)
        {
            int base_int = 0;
            int range_start = 0;
            int range_end = 0;
            bool can_convert = false;
            bool range_in_progress = false;
            string current_string = "";

            char[] input_array = input_range.ToArray();

            List<int> sprite_range = new List<int>();

            for (int i = 0; i < input_array.Length; i++)
            {
                can_convert = int.TryParse(char.ToString(input_array[i]), out base_int);

                if (can_convert == true)
                {
                    current_string += input_array[i];
                }
                else if (can_convert == false)
                {
                    if (current_string != "")
                    {
                        sprite_range.Add(int.Parse(current_string));
                    }

                    switch (input_array[i])
                    {
                        case '-':
                            if (current_string != "")
                            {
                                range_start = int.Parse(current_string);
                                range_in_progress = true;
                                sprite_range.Add(range_start);
                                current_string = "";
                            }
                            break;

                        case ' ':
                            if (current_string != "")
                            {
                                if (range_in_progress == true)
                                {
                                    range_end = int.Parse(current_string);

                                    for (int j = range_start; j < range_end; j++)
                                    {
                                        sprite_range.Add(j);
                                    }

                                    current_string = "";
                                    range_start = 0;
                                    range_end = 0;
                                    range_in_progress = false;
                                }
                                else if (current_string != "")
                                {
                                    sprite_range.Add(int.Parse(current_string));
                                    current_string = "";
                                }
                            }
                            break;

                        case ',':
                            if (range_in_progress == true)
                            {
                                range_end = int.Parse(current_string);

                                for (int j = range_start; j < range_end; j++)
                                {
                                    sprite_range.Add(j);
                                }

                                current_string = "";
                                range_start = 0;
                                range_end = 0;
                                range_in_progress = false;
                            }
                            else if (current_string != "")
                            {
                                sprite_range.Add(int.Parse(current_string));
                                current_string = "";
                            }
                            break;

                        default:
                            // Do nothing
                            break;
                    }
                }
            }

            // Empty last of the input
            if (current_string != "")
            {
                sprite_range.Add(int.Parse(current_string));
            }

            sprite_range = sprite_range.Distinct().ToList();
            sprite_range.Sort();

            return sprite_range;
        }

        public static List<int> Spriteless_Check(List<int> int_range, DisplayNameInternalData new_name_data)
        {
            if (int_range.Contains(0))
            {
                new_name_data.Spriteless_Included = "Yes";
                int_range.Remove(0);
            }
            else
            {
                new_name_data.Spriteless_Included = "No";
            }

            return int_range;
        }

        public static string Int_Range_To_String_Range(UserInfoFields account, OfficialSetData set_data, List<int> int_range)
        {
            string bustup_string = "";

            for (int i = 0; i < int_range.Count; i++)
            {
                bustup_string += Number_To_Bustup_Filename(int_range[i], account, set_data) + ";";
            }

            return bustup_string;
        }

        public static string Number_To_Bustup_Filename(int sprite_number, UserInfoFields account, OfficialSetData set_data)
        {
            // Establish the directory of the specified sprite set.
            string set_path = $@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//{set_data.Origin}//Bustup//{set_data.ID}";

            // Get a count of how many files are in the sprite set's directory.
            int filecount = OfficialSetMethods.AttachmentCountItemDirectory(set_path);

            // Create a variable for the base sprite's filename. We'll go searching for it in a few moments.
            string base_sprite_filename = "";

            // Check if the sprite set's directory exists.
            if (Directory.Exists(set_path))
            {
                // If so, it's time to find the filename for the user's selected sprite so we can retrieve the frames associated with it.
                // We can do this by creating a counter starting from zero that will increment by one until it reaches the sprite numer the user specified.
                // Once it reaches that number, the iterated filename will be saved and we can use that to find its associated frames.
                int counter = 0;

                // The manner of iteration will change based on the user's settings.
                // First, Order by Outfit.
                if (account.Setting_Sheet_Order == "Order by Outfit")
                {
                    // Create a loop starting at 1 meant to iterate though every file in the directory.
                    // Outfit numbers always start at 1, so we'll begin there.
                    for (int outfit = 1; outfit <= filecount; outfit++)
                    {
                        // Inside, create a secondary loop also meant to iterate though every file in the directory.
                        // This loop is searching for expressions, which start at 1.
                        for (int expression = 1; expression <= filecount; expression++)
                        {
                            // Here, we're going to create a file path that could potentially exist given the combination of expression and outfit numbers.
                            // Check if the created file path string exists.
                            if (File.Exists($"{set_path}//{set_data.ID.ToLower()}_{expression}_{outfit}.png"))
                            {
                                // If the file does exist, increment the counter by one.
                                counter++;

                                // Check if the counter matches the same number of the chosen sprite number.
                                if (counter == sprite_number)
                                {
                                    // If it does, we found our sprite! Save the filename to the variable created earlier so we can reference it later.
                                    base_sprite_filename = $"{set_data.ID.ToLower()}_{expression}_{outfit}";

                                    // Break out of the current loop.
                                    break;
                                }
                            }
                        }

                        // Check if the filename variable for the base sprite is not empty.
                        if (base_sprite_filename != "")
                        {
                            // If so, we already found our filename! Break out of the outer loop.
                            break;
                        }
                    }
                }
                // Second case, Order by Expression.
                else if (account.Setting_Sheet_Order == "Order by Expression")
                {
                    // Create a loop starting at 1 meant to iterate though every file in the directory.
                    // Expression numbers always start at 1, so we'll begin there.
                    for (int expression = 1; expression <= filecount; expression++)
                    {
                        // Inside, create a secondary loop also meant to iterate though every file in the directory.
                        // This loop is searching for outfits, which start at 1.
                        for (int outfit = 1; outfit <= filecount; outfit++)
                        {
                            // Here, we're going to create a file path that could potentially exist given the combination of expression and outfit numbers.
                            // Check if the created file path string exists.
                            if (File.Exists($"{set_path}//{set_data.ID.ToLower()}_{expression}_{outfit}.png"))
                            {
                                // If the file does exist, increment the counter by one.
                                counter++;

                                // Check if the counter matches the same number of the chosen sprite number.
                                if (counter == sprite_number)
                                {
                                    // If it does, we found our sprite! Save the filename to the variable created earlier so we can reference it later.
                                    base_sprite_filename = $"{set_data.ID.ToLower()}_{expression}_{outfit}";

                                    // Break out of the current loop.
                                    break;
                                }
                            }
                        }

                        // Check if the filename variable for the base sprite is not empty.
                        if (base_sprite_filename != "")
                        {
                            // If so, we already found our filename! Break out of the outer loop.
                            break;
                        }
                    }
                }
            }

            return $"{base_sprite_filename}.png";
        }

        public static bool Sprite_Range_Validity_Check(OfficialSetData set_data, int largest_sprite)
        {
            // Establish the directory of the specified sprite set.
            string set_path = $@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//{set_data.Origin}//Bustup//{set_data.ID}";

            // Get a count of how many files are in the sprite set's directory.
            int filecount = AttachmentCountItemDirectory(set_path);

            // Now that we have a filecount for the set, let's see if the inputted sprite number is valid before we continue.
            // If not, send an error message and cancel the request.
            if (largest_sprite > filecount)
            {
                return false;
            }

            return true;
        }

        public static List<int> Select_Entire_Sprite_Range(DisplayNameInternalData new_name_data)
        {
            // Establish the directory of the specified sprite set.
            string set_path = $@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//{new_name_data.Sprite_Set.Origin}//Bustup//{new_name_data.Sprite_Set.ID}";

            // Get a count of how many files are in the sprite set's directory.
            int filecount = AttachmentCountItemDirectory(set_path);

            new_name_data.Sprite_Count = filecount;

            List<int> entire_list = new List<int>();

            // Start count from 1 since we are starting from the first sprite
            for (int i = 1; i <= filecount; i++)
            {
                entire_list.Add(i);
            }

            return entire_list;
        }

        private static int AttachmentCountItemDirectory(string set_path)
        {
            string[] attExt = { ".png" };
            return Directory.EnumerateFiles(set_path)
              .Count(f => attExt.Contains(Path.GetExtension(f), StringComparer.InvariantCultureIgnoreCase));
        }
    }
}
