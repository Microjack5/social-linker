using SocialLinker.Core.LocalStorageTables;
using Newtonsoft.Json;
using SocialLinker.Config;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SocialLinker.Core.CloudStorageTables;
using System;
using System.Data.SqlClient;

namespace SocialLinker.Core.SceneMaker.Data.Bustup
{
    class BustupDataMethods
    {
        // Create a variable to store the entire list of bustup data for a sprite set.
        private static List<BustupData> bustup_data_list;
        private static List<FrameData> frame_data_list;

        public static BustupData Get_Bustup_Data(UserInfoFields account, OfficialSetData set_data, MakerCharacterData maker_character_data)
        {
            // Create variables for the folder and JSON that contains the data for the set.
            string data_folder = $@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Data//Template_Data//{set_data.Origin}//Bustup//{set_data.ID}";
            string data_sheet = "bustup_data.json";

            // If the file folder doesn't exist, create it.
            if (!Directory.Exists(data_folder))
            {
                Directory.CreateDirectory(data_folder);
            }

            // If the file exists at the specified directory, load its contents.
            if (File.Exists(data_folder + "/" + data_sheet))
            {
                // Load the data sheet for the selected sprite set.
                bustup_data_list = Load_Bustup_Data_List(data_folder + "/" + data_sheet).ToList();

                // START
                //Console.WriteLine("Copying data...");

                //string data_path = $@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Data//Template_Data//{set_data.Origin}//Bustup//{set_data.ID}//bustup_data.json";

                //foreach (BustupData s in bustup_data_list)
                //{
                //    s.P1_PSP_Scale_Width = 104;
                //    s.P1_PSP_Scale_Height = 112;
                //    s.P1_PSP_Left_Coord_X = 39;
                //    s.P1_PSP_Left_Coord_Y = 84;
                //    s.P1_PSP_Center_Coord_X = 184;
                //    s.P1_PSP_Center_Coord_Y = 84;
                //    s.P1_PSP_Right_Coord_X = 329;
                //    s.P1_PSP_Right_Coord_Y = 84;
                //}

                //try
                //{
                //    string json = JsonConvert.SerializeObject(bustup_data_list, Formatting.Indented);
                //    File.WriteAllText(data_path, json);
                //}
                //catch (Exception e)
                //{
                //    Console.WriteLine($"'{e}'");
                //}

                //Console.WriteLine("Copy complete!");

                // ====================================

                //Console.WriteLine("Copying data...");

                //string data_path = $@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Data//Template_Data//{set_data.Origin}//Bustup//{set_data.ID}//bustup_data.json";

                //foreach (BustupData s in bustup_data_list)
                //{
                //    if (set_data.Origin == "P5R" &&
                //        (s.Filename.Contains("_1.png")))
                //    {
                //        s.P5R_Phone_Coord_X = -12;
                //        s.P5R_Phone_Coord_Y = 347;
                //    }
                //    else if (set_data.Origin == "P5R" &&
                //        (s.Filename.Contains("_2.png")))
                //    {
                //        s.P5R_Phone_Coord_X = 48;
                //        s.P5R_Phone_Coord_Y = 407;
                //    }
                //}

                //try
                //{
                //    string json = JsonConvert.SerializeObject(bustup_data_list, Formatting.Indented);
                //    File.WriteAllText(data_path, json);
                //}
                //catch (Exception e)
                //{
                //    Console.WriteLine($"'{e}'");
                //}

                //Console.WriteLine("Copy complete!");
                // END

                string bustup_filename = "";

                // Find the filename of the bustup that the user has selected.
                if (maker_character_data.Base_Sprite == 0)
                {
                    maker_character_data.Base_Sprite = 1;
                    bustup_filename = Get_Bustup_Filename(account, set_data, maker_character_data);
                    maker_character_data.Base_Sprite = 0;
                }
                else
                {
                    bustup_filename = Get_Bustup_Filename(account, set_data, maker_character_data);
                }

                // Return the bustup data info by using its filename to search for its entry.
                return Bustup_Data_From_Filename(bustup_filename);
            }
            else
            {
                if (set_data.Origin == "P3R")
                {
                    Create_P3R_Bustup_Data_List(set_data);
                }
                else
                {
                    Create_Bustup_Data_List(set_data);
                }
                
                return Get_Bustup_Data(account, set_data, maker_character_data);
            }
        }

        public static BustupData Get_P3R_Bustup_Data(UserInfoFields account, SocialLinkerCommand sl_command, OfficialSetData set_data, MakerCharacterData maker_character_data)
        {
            // Create variables for the folder and JSON that contains the data for the set.
            string data_folder = $@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Data//Template_Data//{set_data.Origin}//Bustup//{set_data.ID}";
            string data_sheet = "bustup_data.json";

            // If the file folder doesn't exist, create it.
            if (!Directory.Exists(data_folder))
            {
                Directory.CreateDirectory(data_folder);
            }

            // If the file exists at the specified directory, load its contents.
            if (File.Exists(data_folder + "/" + data_sheet))
            {
                // Load the data sheet for the selected sprite set.
                bustup_data_list = Load_Bustup_Data_List(data_folder + "/" + data_sheet).ToList();

                string bustup_filename = "";

                // Find the filename of the bustup that the user has selected.
                if (maker_character_data.Base_Sprite == 0)
                {
                    maker_character_data.Base_Sprite = 1;
                    bustup_filename = OfficialSetMethods.Get_P3R_Bustup_Filename_From_Sprite_Number(sl_command, set_data, maker_character_data, false);
                    maker_character_data.Base_Sprite = 0;
                }
                else
                {
                    bustup_filename = OfficialSetMethods.Get_P3R_Bustup_Filename_From_Sprite_Number(sl_command, set_data, maker_character_data, false);
                }

                // Return the bustup data info by using its filename to search for its entry.
                return Bustup_Data_From_Filename($"{bustup_filename}.png");
            }
            else
            {
                Create_P3R_Bustup_Data_List(set_data);
                return Get_P3R_Bustup_Data(account, sl_command, set_data, maker_character_data);
            }
        }

        public static FrameData Get_Eye_Frame_Data(OfficialSetData set_data, BustupData bustup_data, MakerCharacterData maker_character_data)
        {
            // Create variables for the folder and JSON that contains the data for the set.
            string data_folder = $@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Data//Template_Data//{set_data.Origin}//Bustup//{set_data.ID}";
            string data_sheet = "eye_frame_data.json";

            // If the file folder doesn't exist, create it.
            if (!Directory.Exists(data_folder))
            {
                Directory.CreateDirectory(data_folder);
            }

            // If the file exists at the specified directory, load its contents.
            if (File.Exists(data_folder + "/" + data_sheet))
            {
                // Load the data sheet for the selected sprite set.
                frame_data_list = Load_Frame_Data_List(data_folder + "/" + data_sheet).ToList();

                // To create the filename for the frame, delete the file extention at the end of the base bustup's filename first.
                string bustup_filename = bustup_data.Filename.Substring(0, bustup_data.Filename.Length - 4);

                // Combine the base bustup filename substring with the needed frame suffix to create the frame filename.
                string frame_filename = $"{bustup_filename}_e{maker_character_data.Eye_Frame}.png";

                // Return the frame data info by using its filename to search for its entry.
                return Frame_Data_From_Filename(frame_filename);
            }
            // If the file doesn't exist, create it.
            else
            {
                if (Create_Eye_Frame_Data_List(set_data) != default)
                {
                    return Get_Eye_Frame_Data(set_data, bustup_data, maker_character_data);
                }
                else
                {
                    return null;
                }
            } 
        }

        public static FrameData Get_P3R_Eye_Frame_Data(OfficialSetData set_data, BustupData bustup_data, MakerCharacterData maker_character_data, string base_sprite_filename_raw, string base_sprite_filename_preview)
        {
            string set_path_raw = $@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//{set_data.Origin}//Bustup//{set_data.ID}";
            string eye_frame_path_raw = $@"{set_path_raw}//Eyes";

            string data_folder = $@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Data//Template_Data//{set_data.Origin}//Bustup//{set_data.ID}";
            string data_sheet = "eye_frame_data.json";

            if (!Directory.Exists(data_folder))
            {
                Directory.CreateDirectory(data_folder);
            }

            // If the file exists at the specified directory, load its contents.
            if (File.Exists(data_folder + "/" + data_sheet))
            {
                // Load the data sheet for the selected sprite set.
                frame_data_list = Load_Frame_Data_List(data_folder + "/" + data_sheet).ToList();

                string bustup_filename = base_sprite_filename_preview;

                // Parse filename here
                List<string> parsed_filename = bustup_filename.Split(new char[] { '_' }).ToList();

                string char_id = parsed_filename[0];
                string expression = parsed_filename[1];
                string outfit_and_pose = parsed_filename[2];

                string outfit = outfit_and_pose.Substring(0, outfit_and_pose.Length - 1);
                string pose = outfit_and_pose.Substring(outfit_and_pose.Length - 1);

                string frame_filename_specific = bustup_filename;
                string frame_filename_generic = $"{char_id}_{expression}_0{pose}";
                string frame_filename_chosen = "";

                // Combine the base bustup filename substring with the needed frame suffix to create the frame filename.
                if (File.Exists($"{eye_frame_path_raw}//{frame_filename_specific}_e{maker_character_data.Eye_Frame}.png"))
                {
                    frame_filename_chosen = $"{frame_filename_specific}_e{maker_character_data.Eye_Frame}.png";
                }
                else if (File.Exists($"{eye_frame_path_raw}//{frame_filename_generic}_e{maker_character_data.Eye_Frame}.png"))
                {
                    frame_filename_chosen = $"{frame_filename_generic}_e{maker_character_data.Eye_Frame}.png";
                }

                // Return the frame data info by using its filename to search for its entry.
                return Frame_Data_From_Filename(frame_filename_chosen);
            }
            // If the file doesn't exist, create it.
            else
            {
                if (Create_P3R_Eye_Frame_Data_List(set_data) != default)
                {
                    return Get_P3R_Eye_Frame_Data(set_data, bustup_data, maker_character_data, base_sprite_filename_raw, base_sprite_filename_preview);
                }
                else
                {
                    return null;
                }
            }
        }

        public static FrameData Get_P3R_Mouth_Frame_Data(OfficialSetData set_data, BustupData bustup_data, MakerCharacterData maker_character_data, string base_sprite_filename_raw, string base_sprite_filename_preview)
        {
            string set_path_raw = $@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//{set_data.Origin}//Bustup//{set_data.ID}";
            string mouth_frame_path_raw = $@"{set_path_raw}//Mouth";

            string data_folder = $@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Data//Template_Data//{set_data.Origin}//Bustup//{set_data.ID}";
            string data_sheet = "mouth_frame_data.json";

            if (!Directory.Exists(data_folder))
            {
                Directory.CreateDirectory(data_folder);
            }

            // If the file exists at the specified directory, load its contents.
            if (File.Exists(data_folder + "/" + data_sheet))
            {
                // Load the data sheet for the selected sprite set.
                frame_data_list = Load_Frame_Data_List(data_folder + "/" + data_sheet).ToList();

                string bustup_filename = base_sprite_filename_preview;

                // Parse filename here
                List<string> parsed_filename = bustup_filename.Split(new char[] { '_' }).ToList();

                string char_id = parsed_filename[0];
                string expression = parsed_filename[1];
                string outfit_and_pose = parsed_filename[2];

                string outfit = outfit_and_pose.Substring(0, outfit_and_pose.Length - 1);
                string pose = outfit_and_pose.Substring(outfit_and_pose.Length - 1);

                string frame_filename_specific = bustup_filename;
                string frame_filename_generic = $"{char_id}_{expression}_0{pose}";
                string frame_filename_chosen = "";

                // Combine the base bustup filename substring with the needed frame suffix to create the frame filename.
                if (File.Exists($"{mouth_frame_path_raw}//{frame_filename_specific}_m{maker_character_data.Mouth_Frame}.png"))
                {
                    frame_filename_chosen = $"{frame_filename_specific}_m{maker_character_data.Mouth_Frame}.png";
                }
                else if (File.Exists($"{mouth_frame_path_raw}//{frame_filename_generic}_m{maker_character_data.Mouth_Frame}.png"))
                {
                    frame_filename_chosen = $"{frame_filename_generic}_m{maker_character_data.Mouth_Frame}.png";
                }

                // Return the frame data info by using its filename to search for its entry.
                return Frame_Data_From_Filename(frame_filename_chosen);
            }
            // If the file doesn't exist, create it.
            else
            {
                if (Create_P3R_Mouth_Frame_Data_List(set_data) != default)
                {
                    return Get_P3R_Mouth_Frame_Data(set_data, bustup_data, maker_character_data, base_sprite_filename_raw, base_sprite_filename_preview);
                }
                else
                {
                    return null;
                }
            }
        }

        public static FrameData Get_Mouth_Frame_Data(OfficialSetData set_data, BustupData bustup_data, MakerCharacterData maker_character_data)
        {
            // Create variables for the folder and JSON that contains the data for the set.
            string data_folder = $@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Data//Template_Data//{set_data.Origin}//Bustup//{set_data.ID}";
            string data_sheet = "mouth_frame_data.json";

            // If the file folder doesn't exist, create it.
            if (!Directory.Exists(data_folder))
            {
                Directory.CreateDirectory(data_folder);
            }

            // If the file exists at the specified directory, load its contents.
            if (File.Exists(data_folder + "/" + data_sheet))
            {
                // Load the data sheet for the selected sprite set.
                frame_data_list = Load_Frame_Data_List(data_folder + "/" + data_sheet).ToList();

                // To create the filename for the frame, delete the file extention at the end of the base bustup's filename first.
                string bustup_filename = bustup_data.Filename.Substring(0, bustup_data.Filename.Length - 4);

                // Combine the base bustup filename substring with the needed frame suffix to create the frame filename.
                string frame_filename = $"{bustup_filename}_m{maker_character_data.Mouth_Frame}.png";

                // Return the frame data info by using its filename to search for its entry.
                return Frame_Data_From_Filename(frame_filename);
            }
            // If the file doesn't exist, create it.
            if (Create_Mouth_Frame_Data_List(set_data) != default)
            {
                return Get_Mouth_Frame_Data(set_data, bustup_data, maker_character_data);
            }
            else
            {
                return null;
            }
        }

        public static List<BustupData> Create_Bustup_Data_List(OfficialSetData set_data) // For dev purposes only
        {
            var new_list = new List<BustupData>();

            string bustup_path = $@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//{set_data.Origin}//Bustup//{set_data.ID}";
            string data_path = $@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Data//Template_Data//{set_data.Origin}//Bustup//{set_data.ID}//bustup_data.json";

            // Get a count of how many files are in the sprite set's directory.
            int filecount = OfficialSetMethods.AttachmentCountItemDirectory(bustup_path);

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
                    if (File.Exists($"{bustup_path}//{set_data.ID.ToLower()}_{expression}_{outfit}.png"))
                    {
                        if (expression <= 8)
                        {
                            var new_bustup_data = new BustupData()
                            {
                                Filename = $"{set_data.ID.ToLower()}_{expression}_{outfit}.png",
                                Default_Name_EN = "Shido",
                                Default_Name_JPN = "---",
                                P5_PS4_Scale_Width = 768,
                                P5_PS4_Scale_Height = 768,
                                P5_PS4_Coord_X = -27,
                                P5_PS4_Coord_Y = 327,
                            };

                            new_list.Add(new_bustup_data);
                        }
                        else
                        {
                            var new_bustup_data = new BustupData()
                            {
                                Filename = $"{set_data.ID.ToLower()}_{expression}_{outfit}.png",
                                Default_Name_EN = "Shadow Shido",
                                Default_Name_JPN = "---",
                                P5_PS4_Scale_Width = 768,
                                P5_PS4_Scale_Height = 768,
                                P5_PS4_Coord_X = -48,
                                P5_PS4_Coord_Y = 327,
                            };

                            new_list.Add(new_bustup_data);
                        }

                        //var new_bustup_data = new BustupData()
                        //{
                        //    Filename = $"{set_data.ID.ToLower()}_{expression}_{outfit}.png",
                        //    Default_Name_EN = "Igor",
                        //    Default_Name_JPN = "---",
                        //    P5_PS4_Scale_Width = 768,
                        //    P5_PS4_Scale_Height = 768,
                        //    P5_PS4_Coord_X = -10,
                        //    P5_PS4_Coord_Y = 349,
                        //};

                        //new_list.Add(new_bustup_data);
                    }
                }
            }

            try
            {
                string json = JsonConvert.SerializeObject(new_list, Formatting.Indented);
                File.WriteAllText(data_path, json);
            }
            catch (Exception e)
            {
                Console.WriteLine($"'{e}'");
            }

            return new_list;
        }

        public static List<BustupData> Create_P3R_Bustup_Data_List(OfficialSetData set_data) // For dev purposes only
        {
            var new_list = new List<BustupData>();

            string bustup_path = $@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//{set_data.Origin}//Bustup//{set_data.ID}";
            string data_path = $@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Data//Template_Data//{set_data.Origin}//Bustup//{set_data.ID}//bustup_data.json";

            // Get a count of how many files are in the sprite set's directory.
            int filecount = OfficialSetMethods.AttachmentCountItemDirectory(bustup_path);

            // Create a loop starting at 1 meant to iterate though every file in the directory.
            // Expression numbers always start at 1, so we'll begin there.
            for (int outfit = 1; outfit <= filecount; outfit++)
            {
                // Inside, create a secondary loop also meant to iterate though every file in the directory.
                // This loop is searching for outfits, which start at 1.
                for (int pose_index = 0; pose_index < Global.p3r_poses.Length; pose_index++)
                {
                    char current_pose = Global.p3r_poses[pose_index];

                    // Here, we're going to create a file path that could potentially exist given the combination of expression and outfit numbers.
                    // Check if the created file path string exists.
                    if (File.Exists($"{bustup_path}//{set_data.ID.ToLower()}_0_{outfit}{current_pose}.png"))
                    {
                        if (current_pose == 'a')
                        {
                            var new_bustup_data = new BustupData()
                            {
                                Filename = $"{set_data.ID.ToLower()}_0_{outfit}{current_pose}.png",
                                Default_Name_EN = "Mitsuru Kirijo",
                                Default_Name_JPN = "---",
                                P3R_Scale_Width = 2048,
                                P3R_Scale_Height = 2048,
                                P3R_Coord_X = -494,
                                P3R_Coord_Y = 250 + 0,
                            };

                            new_list.Add(new_bustup_data);
                        }
                        else if (current_pose == 'b')
                        {
                            var new_bustup_data = new BustupData()
                            {
                                Filename = $"{set_data.ID.ToLower()}_0_{outfit}{current_pose}.png",
                                Default_Name_EN = "Mitsuru Kirijo",
                                Default_Name_JPN = "---",
                                P3R_Scale_Width = 2048,
                                P3R_Scale_Height = 2048,
                                P3R_Coord_X = -486,
                                P3R_Coord_Y = 250 + 0,
                            };

                            new_list.Add(new_bustup_data);
                        }
                        else if (current_pose == 'c')
                        {
                            var new_bustup_data = new BustupData()
                            {
                                Filename = $"{set_data.ID.ToLower()}_0_{outfit}{current_pose}.png",
                                Default_Name_EN = "Mitsuru Kirijo",
                                Default_Name_JPN = "---",
                                P3R_Scale_Width = 2048,
                                P3R_Scale_Height = 2048,
                                P3R_Coord_X = -434 + -150,
                                P3R_Coord_Y = 250 + 0,
                            };

                            new_list.Add(new_bustup_data);
                        }
                        else if (current_pose == 'd')
                        {
                            var new_bustup_data = new BustupData()
                            {
                                Filename = $"{set_data.ID.ToLower()}_0_{outfit}{current_pose}.png",
                                Default_Name_EN = "Mitsuru Kirijo",
                                Default_Name_JPN = "---",
                                P3R_Scale_Width = 2048,
                                P3R_Scale_Height = 2048,
                                P3R_Coord_X = -434 + -30,
                                P3R_Coord_Y = 250 + 0,
                            };

                            new_list.Add(new_bustup_data);
                        }

                        //var new_bustup_data = new BustupData()
                        //{
                        //    Filename = $"{set_data.ID.ToLower()}_{expression}_{outfit}.png",
                        //    Default_Name_EN = "Igor",
                        //    Default_Name_JPN = "---",
                        //    P5_PS4_Scale_Width = 768,
                        //    P5_PS4_Scale_Height = 768,
                        //    P5_PS4_Coord_X = -10,
                        //    P5_PS4_Coord_Y = 349,
                        //};

                        //new_list.Add(new_bustup_data);
                    }
                }
            }

            try
            {
                string json = JsonConvert.SerializeObject(new_list, Formatting.Indented);
                File.WriteAllText(data_path, json);
            }
            catch (Exception e)
            {
                Console.WriteLine($"'{e}'");
            }

            return new_list;
        }

        public static List<FrameData> Create_Eye_Frame_Data_List(OfficialSetData set_data) // For dev purposes only
        {
            // Create a new empty frame data list.
            // This is what we'll be using to create frame data for each image.
            var new_list = new List<FrameData>();

            // Establish the paths for the directory where the eye frames are held, as well as the directory for where the data sheet is held.
            string frame_path = $@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//{set_data.Origin}//Bustup//{set_data.ID}//Eyes";
            string data_path = $@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Data//Template_Data//{set_data.Origin}//Bustup//{set_data.ID}//eye_frame_data.json";
            
            // Initialize a new int variable to zero.
            // We'll need this to store how many frames are in the eye frame directory.
            int filecount = 0;

            // Check if the eye frame directory established exists.
            // If so, change the filecount int to how many images are in the eye frame directory.
            if (Directory.Exists(frame_path))
            {
                filecount = OfficialSetMethods.AttachmentCountItemDirectory(frame_path);
            }
            // If not, return null.
            else
            {
                return null;
            }

            // Create a loop starting at 1 meant to iterate though every file in the directory.
            // Expression numbers always start at 1, so we'll begin there.
            for (int expression = 1; expression <= filecount; expression++)
            {
                // Inside, create a secondary loop also meant to iterate though every file in the directory.
                // This loop is searching for outfits, which start at 1.
                for (int outfit = 1; outfit <= filecount; outfit++)
                {
                    // Get the current filename generated from the for loops.
                    // This name isn't guaranteed to exist, but we will test it soon to find out.
                    string current_filename = $"{set_data.ID.ToLower()}_{expression}_{outfit}";

                    // Let's get the file count for any images in the directory that share the same base sprite filename substring as our current generated name. 
                    // We do that by forming an array of all filenames that match.
                    string[] allFiles = Directory.GetFiles(frame_path, $"{current_filename}_e*.png");

                    // Take the length of the array and assign it to a new int variable.
                    int base_sprite_eye_frame_count = allFiles.Length;

                    // We're already within two loops to generate the base filename, but now we need to use another loop to iterate through potentially multiple frames for the same sprite.
                    for (int i = 1; i <= base_sprite_eye_frame_count; i++)
                    {
                        // Here, we're going to create a file path that could potentially exist given the combination of expression and outfit numbers.
                        // Check if the created file path string exists.
                        if (File.Exists($"{frame_path}//{set_data.ID.ToLower()}_{expression}_{outfit}_e{i}.png"))
                        {
                            // EXPERIMENT START
                            FrameData new_frame_data = new FrameData();

                            if (expression >= 8)
                            {
                                new_frame_data = new FrameData()
                                {
                                    Filename = $"{set_data.ID.ToLower()}_{expression}_{outfit}_e{i}.png",
                                    Scale_Width = 512,
                                    Scale_Height = 256,
                                    Coord_X = 843,
                                    Coord_Y = 996
                                };
                            }
                            else
                            {
                                new_frame_data = new FrameData()
                                {
                                    Filename = $"{set_data.ID.ToLower()}_{expression}_{outfit}_e{i}.png",
                                    Scale_Width = 512,
                                    Scale_Height = 256,
                                    Coord_X = 885,
                                    Coord_Y = 995
                                };
                            }

                            new_list.Add(new_frame_data);
                            // EXPERIMENT END

                            // If so, generate a new frame data object for the frame.
                            //var new_frame_data = new FrameData()
                            //{
                            //    Filename = $"{set_data.ID.ToLower()}_{expression}_{outfit}_e{i}.png",
                            //    Scale_Width = 384,
                            //    Scale_Height = 192,
                            //    Coord_X = 9,
                            //    Coord_Y = 249
                            //};
                            //new_list.Add(new_frame_data);
                        }
                    }
                }
            }

            // Write all text to the data file.
            try
            {
                string json = JsonConvert.SerializeObject(new_list, Formatting.Indented);
                File.WriteAllText(data_path, json);
            }
            catch (Exception e)
            {
                Console.WriteLine($"'{e}'");
            }

            return new_list;
        }

        public static List<FrameData> Create_Mouth_Frame_Data_List(OfficialSetData set_data) // For dev purposes only
        {
            // Create a new empty frame data list.
            // This is what we'll be using to create frame data for each image.
            var new_list = new List<FrameData>();

            // Establish the paths for the directory where the eye frames are held, as well as the directory for where the data sheet is held.
            string frame_path = $@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//{set_data.Origin}//Bustup//{set_data.ID}//Mouth";
            string data_path = $@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Data//Template_Data//{set_data.Origin}//Bustup//{set_data.ID}//mouth_frame_data.json";

            // Initialize a new int variable to zero.
            // We'll need this to store how many frames are in the eye frame directory.
            int filecount = 0;

            // Check if the eye frame directory established exists.
            // If so, change the filecount int to how many images are in the eye frame directory.
            if (Directory.Exists(frame_path))
            {
                filecount = OfficialSetMethods.AttachmentCountItemDirectory(frame_path);
            }
            // If not, return null.
            else
            {
                return null;
            }

            // Create a loop starting at 1 meant to iterate though every file in the directory.
            // Expression numbers always start at 1, so we'll begin there.
            for (int expression = 1; expression <= filecount; expression++)
            {
                // Inside, create a secondary loop also meant to iterate though every file in the directory.
                // This loop is searching for outfits, which start at 1.
                for (int outfit = 1; outfit <= filecount; outfit++)
                {
                    // Get the current filename generated from the for loops.
                    // This name isn't guaranteed to exist, but we will test it soon to find out.
                    string current_filename = $"{set_data.ID.ToLower()}_{expression}_{outfit}";

                    // Let's get the file count for any images in the directory that share the same base sprite filename substring as our current generated name. 
                    // We do that by forming an array of all filenames that match.
                    string[] allFiles = Directory.GetFiles(frame_path, $"{current_filename}_m*.png");

                    // Take the length of the array and assign it to a new int variable.
                    int base_sprite_eye_frame_count = allFiles.Length;

                    // We're already within two loops to generate the base filename, but now we need to use another loop to iterate through potentially multiple frames for the same sprite.
                    for (int i = 1; i <= base_sprite_eye_frame_count; i++)
                    {
                        // Here, we're going to create a file path that could potentially exist given the combination of expression and outfit numbers.
                        // Check if the created file path string exists.
                        if (File.Exists($"{frame_path}//{set_data.ID.ToLower()}_{expression}_{outfit}_m{i}.png"))
                        {
                            // EXPERIMENT START
                            FrameData new_frame_data = new FrameData();

                            if (expression >= 8)
                            {
                                new_frame_data = new FrameData()
                                {
                                    Filename = $"{set_data.ID.ToLower()}_{expression}_{outfit}_m{i}.png",
                                    Scale_Width = 512,
                                    Scale_Height = 256,
                                    Coord_X = 843,
                                    Coord_Y = 1244
                                };
                            }
                            else
                            {
                                new_frame_data = new FrameData()
                                {
                                    Filename = $"{set_data.ID.ToLower()}_{expression}_{outfit}_m{i}.png",
                                    Scale_Width = 512,
                                    Scale_Height = 256,
                                    Coord_X = 885,
                                    Coord_Y = 1243
                                };
                            }

                            new_list.Add(new_frame_data);
                            // EXPERIMENT END

                            // If so, generate a new frame data object for the frame.
                            //var new_frame_data = new FrameData()
                            //{
                            //    Filename = $"{set_data.ID.ToLower()}_{expression}_{outfit}_m{i}.png",
                            //    Scale_Width = 384,
                            //    Scale_Height = 192,
                            //    Coord_X = 214,
                            //    Coord_Y = 387
                            //};
                            //new_list.Add(new_frame_data);
                        }
                    }
                }
            }

            // Write all text to the data file.
            try
            {
                string json = JsonConvert.SerializeObject(new_list, Formatting.Indented);
                File.WriteAllText(data_path, json);
            }
            catch (Exception e)
            {
                Console.WriteLine($"'{e}'");
            }

            return new_list;
        }

        public static List<FrameData> Create_P3R_Eye_Frame_Data_List(OfficialSetData set_data) // For dev purposes only
        {
            // Create a new empty frame data list.
            // This is what we'll be using to create frame data for each image.
            var new_list = new List<FrameData>();

            // Establish the paths for the directory where the eye frames are held, as well as the directory for where the data sheet is held.
            string base_path = $@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//{set_data.Origin}//Bustup//{set_data.ID}";
            string eye_frame_path = $@"{base_path}//Eyes";

            string data_path = $@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Data//Template_Data//{set_data.Origin}//Bustup//{set_data.ID}//eye_frame_data.json";

            // Initialize a new int variable to zero.
            // We'll need this to store how many frames are in the eye frame directory.
            int base_bustup_filecount = 0;
            int eye_frame_filecount = 0;

            // Check if the eye frame directory established exists.
            // If so, change the filecount int to how many images are in the eye frame directory.
            if (Directory.Exists(base_path))
            {
                base_bustup_filecount = OfficialSetMethods.AttachmentCountItemDirectory(base_path);
            }
            // If not, return null.
            else
            {
                return null;
            }

            if (Directory.Exists(eye_frame_path))
            {
                eye_frame_filecount = OfficialSetMethods.AttachmentCountItemDirectory(eye_frame_path);
            }
            // If not, return null.
            else
            {
                return null;
            }

            char[] poses = Global.p3r_poses;

            // Create a loop starting at 1 meant to iterate though every file in the directory.
            // Expression numbers always start at 1, so we'll begin there.
            for (int expression = 1; expression <= eye_frame_filecount; expression++)
            {
                // Inside, create a secondary loop also meant to iterate though every file in the directory.
                // This loop is searching for outfits, which start at 0 in P3R.
                for (int outfit = 0; outfit < base_bustup_filecount; outfit++)
                {
                    // Third loop for pose index
                    for (int pose_index = 0; pose_index < poses.Length; pose_index++)
                    {
                        // Get the current filename generated from the for loops.
                        // This name isn't guaranteed to exist, but we will test it soon to find out.
                        string current_filename = $"{set_data.ID.ToLower()}_{expression}_{outfit}{poses[pose_index]}";

                        // Let's get the file count for any images in the directory that share the same base sprite filename substring as our current generated name. 
                        // We do that by forming an array of all filenames that match.
                        string[] allFiles = Directory.GetFiles(eye_frame_path, $"{current_filename}_e*.png");

                        // Take the length of the array and assign it to a new int variable.
                        int base_sprite_eye_frame_count = allFiles.Length;

                        // We're already within two loops to generate the base filename, but now we need to use another loop to iterate through potentially multiple frames for the same sprite.
                        for (int i = 1; i <= base_sprite_eye_frame_count; i++)
                        {
                            // Here, we're going to create a file path that could potentially exist given the combination of expression and outfit numbers.
                            // Check if the created file path string exists.
                            if (File.Exists($"{eye_frame_path}//{set_data.ID.ToLower()}_{expression}_{outfit}{poses[pose_index]}_e{i}.png"))
                            {
                                // EXPERIMENT START
                                FrameData new_frame_data = new FrameData();

                                if (expression == 8 || expression == 9 || expression == 10)
                                {
                                    new_frame_data = new FrameData()
                                    {
                                        Filename = $"{set_data.ID.ToLower()}_{expression}_{outfit}{poses[pose_index]}_e{i}.png",
                                        Scale_Width = 512,
                                        Scale_Height = 256,
                                        Coord_X = 843,
                                        Coord_Y = 996
                                    };
                                }
                                else if (expression == 11 || expression == 12)
                                {
                                    new_frame_data = new FrameData()
                                    {
                                        Filename = $"{set_data.ID.ToLower()}_{expression}_{outfit}{poses[pose_index]}_e{i}.png",
                                        Scale_Width = 512,
                                        Scale_Height = 256,
                                        Coord_X = 840,
                                        Coord_Y = 1059
                                    };
                                }
                                else if (expression == 13 || expression == 14 || expression == 15)
                                {
                                    new_frame_data = new FrameData()
                                    {
                                        Filename = $"{set_data.ID.ToLower()}_{expression}_{outfit}{poses[pose_index]}_e{i}.png",
                                        Scale_Width = 512,
                                        Scale_Height = 256,
                                        Coord_X = 826,
                                        Coord_Y = 1088
                                    };
                                }
                                else
                                {
                                    new_frame_data = new FrameData()
                                    {
                                        Filename = $"{set_data.ID.ToLower()}_{expression}_{outfit}{poses[pose_index]}_e{i}.png",
                                        Scale_Width = 512,
                                        Scale_Height = 256,
                                        Coord_X = 885,
                                        Coord_Y = 995
                                    };
                                }

                                //if (poses[pose_index] == 'a')
                                //{
                                //    new_frame_data = new FrameData()
                                //    {
                                //        Filename = $"{set_data.ID.ToLower()}_{expression}_{outfit}{poses[pose_index]}_e{i}.png",
                                //        Scale_Width = 512,
                                //        Scale_Height = 256,
                                //        Coord_X = 673,
                                //        Coord_Y = 1083
                                //    };
                                //}

                                new_list.Add(new_frame_data);
                                // EXPERIMENT END
                            }
                        }
                    }
                }
            }

            // Write all text to the data file.
            try
            {
                string json = JsonConvert.SerializeObject(new_list, Formatting.Indented);
                File.WriteAllText(data_path, json);
            }
            catch (Exception e)
            {
                Console.WriteLine($"'{e}'");
            }

            return new_list;
        }

        public static List<FrameData> Create_P3R_Mouth_Frame_Data_List(OfficialSetData set_data) // For dev purposes only
        {
            // Create a new empty frame data list.
            // This is what we'll be using to create frame data for each image.
            var new_list = new List<FrameData>();

            // Establish the paths for the directory where the mouth frames are held, as well as the directory for where the data sheet is held.
            string base_path = $@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//{set_data.Origin}//Bustup//{set_data.ID}";
            string mouth_frame_path = $@"{base_path}//Mouth";

            string data_path = $@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Data//Template_Data//{set_data.Origin}//Bustup//{set_data.ID}//mouth_frame_data.json";

            // Initialize a new int variable to zero.
            // We'll need this to store how many frames are in the mouth frame directory.
            int base_bustup_filecount = 0;
            int mouth_frame_filecount = 0;

            // Check if the mouth frame directory established exists.
            // If so, change the filecount int to how many images are in the mouth frame directory.
            if (Directory.Exists(base_path))
            {
                base_bustup_filecount = OfficialSetMethods.AttachmentCountItemDirectory(base_path);
            }
            // If not, return null.
            else
            {
                return null;
            }

            if (Directory.Exists(mouth_frame_path))
            {
                mouth_frame_filecount = OfficialSetMethods.AttachmentCountItemDirectory(mouth_frame_path);
            }
            // If not, return null.
            else
            {
                return null;
            }

            char[] poses = Global.p3r_poses;

            // Create a loop starting at 1 meant to iterate though every file in the directory.
            // Expression numbers always start at 1, so we'll begin there.
            for (int expression = 1; expression <= mouth_frame_filecount; expression++)
            {
                // Inside, create a secondary loop also meant to iterate though every file in the directory.
                // This loop is searching for outfits, which start at 0 in P3R.
                for (int outfit = 0; outfit <= base_bustup_filecount; outfit++)
                {
                    // Third loop for pose index
                    for (int pose_index = 0; pose_index < poses.Length; pose_index++)
                    {
                        // Get the current filename generated from the for loops.
                        // This name isn't guaranteed to exist, but we will test it soon to find out.
                        string current_filename = $"{set_data.ID.ToLower()}_{expression}_{outfit}{poses[pose_index]}";

                        // Let's get the file count for any images in the directory that share the same base sprite filename substring as our current generated name. 
                        // We do that by forming an array of all filenames that match.
                        string[] allFiles = Directory.GetFiles(mouth_frame_path, $"{current_filename}_m*.png");

                        // Take the length of the array and assign it to a new int variable.
                        int base_sprite_mouth_frame_count = allFiles.Length;

                        // We're already within two loops to generate the base filename, but now we need to use another loop to iterate through potentially multiple frames for the same sprite.
                        for (int i = 1; i <= base_sprite_mouth_frame_count; i++)
                        {
                            // Here, we're going to create a file path that could potentially exist given the combination of expression and outfit numbers.
                            // Check if the created file path string exists.
                            if (File.Exists($"{mouth_frame_path}//{set_data.ID.ToLower()}_{expression}_{outfit}{poses[pose_index]}_m{i}.png"))
                            {
                                // EXPERIMENT START
                                FrameData new_frame_data = new FrameData();

                                if (expression == 8 || expression == 9 || expression == 10)
                                {
                                    new_frame_data = new FrameData()
                                    {
                                        Filename = $"{set_data.ID.ToLower()}_{expression}_{outfit}{poses[pose_index]}_m{i}.png",
                                        Scale_Width = 512,
                                        Scale_Height = 256,
                                        Coord_X = 843,
                                        Coord_Y = 1244
                                    };
                                }
                                else if (expression == 11 || expression == 12)
                                {
                                    new_frame_data = new FrameData()
                                    {
                                        Filename = $"{set_data.ID.ToLower()}_{expression}_{outfit}{poses[pose_index]}_m{i}.png",
                                        Scale_Width = 512,
                                        Scale_Height = 256,
                                        Coord_X = 840,
                                        Coord_Y = 1307
                                    };
                                }
                                else if (expression == 13 || expression == 14 || expression == 15)
                                {
                                    new_frame_data = new FrameData()
                                    {
                                        Filename = $"{set_data.ID.ToLower()}_{expression}_{outfit}{poses[pose_index]}_m{i}.png",
                                        Scale_Width = 512,
                                        Scale_Height = 256,
                                        Coord_X = 826,
                                        Coord_Y = 1336
                                    };
                                }
                                else
                                {
                                    new_frame_data = new FrameData()
                                    {
                                        Filename = $"{set_data.ID.ToLower()}_{expression}_{outfit}{poses[pose_index]}_m{i}.png",
                                        Scale_Width = 512,
                                        Scale_Height = 256,
                                        Coord_X = 885,
                                        Coord_Y = 1243
                                    };
                                }

                                //if (poses[pose_index] == 'a')
                                //{
                                //    new_frame_data = new FrameData()
                                //    {
                                //        Filename = $"{set_data.ID.ToLower()}_{expression}_{outfit}{poses[pose_index]}_m{i}.png",
                                //        Scale_Width = 512,
                                //        Scale_Height = 256,
                                //        Coord_X = 673,
                                //        Coord_Y = 1320
                                //    };
                                //}

                                new_list.Add(new_frame_data);
                                // EXPERIMENT END
                            }
                        }
                    }
                }
            }

            // Write all text to the data file.
            try
            {
                string json = JsonConvert.SerializeObject(new_list, Formatting.Indented);
                File.WriteAllText(data_path, json);
            }
            catch (Exception e)
            {
                Console.WriteLine($"'{e}'");
            }

            return new_list;
        }

        public static IEnumerable<BustupData> Load_Bustup_Data_List(string filePath)
        {
            // If the path specified doesn't exist, return null.
            if (!File.Exists(filePath)) return null;

            // Otherwise, deserialize the list within the JSON file and return.
            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<BustupData>>(json);
        }

        public static IEnumerable<FrameData> Load_Frame_Data_List(string filePath)
        {
            // If the path specified doesn't exist, return null.
            if (!File.Exists(filePath)) return null;

            // Otherwise, deserialize the list within the JSON file and return.
            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<FrameData>>(json);
        }

        public static string Get_Bustup_Filename(UserInfoFields account, OfficialSetData set_data, MakerCharacterData maker_character_data)
        {
            // Establish the directory of the specified sprite set.
            string set_path = $@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//{set_data.Origin}//Bustup//{set_data.ID}";

            // Get a count of how many files are in the sprite set's directory.
            int filecount = OfficialSetMethods.AttachmentCountItemDirectory(set_path);

            // Create a variable for the base sprite's filename. We'll go searching for it in a few moments.
            string base_sprite_filename = "";

            // Create variables that keep track of how many frames of each frame type are present for the base sprite.
            // This will help us perform the math needed to form the sprite sheet.
            //int eye_frame_count = 0;
            //int mouth_frame_count = 0;

            // Check if the sprite set's directory exists.
            if (Directory.Exists(set_path))
            {
                // If so, it's time to find the filename for the user's selected sprite so we can retrieve the frames associated with it.
                // We can do this by creating a counter starting from zero that will increment by one until it reaches the sprite numer the user specified.
                // Once it reaches that number, the iterated filename will be saved and we can use that to find its associated frames.
                int counter = 0;
                int base_sprite_number = maker_character_data.Base_Sprite;

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
                                if (counter == base_sprite_number)
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
                                if (counter == base_sprite_number)
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

        public static BustupData Bustup_Data_From_Filename(string input_filename)
        {
            // Iterate through each entry of the data list for the set's bustups.
            foreach (BustupData s in bustup_data_list)
            {
                // If the filename for an entry matches the input filename, return that entry.
                if (s.Filename == input_filename)
                {
                    return s;
                }
            }

            return null;
        }

        public static FrameData Frame_Data_From_Filename(string input_filename)
        {
            // Iterate through each entry of the data list for the set's bustups.
            foreach (FrameData s in frame_data_list)
            {
                // If the filename for an entry matches the input filename, return that entry.
                if (s.Filename == input_filename)
                {
                    return s;
                }
            }

            return null;
        }
    }
}
