using Discord.WebSocket;
using SocialLinker.Config;
using SocialLinker.Core.CloudStorageTables;
using SocialLinker.Core.LocalStorageTables;
using SocialLinker.Core.Menus.MakerMulti.Main;
using SocialLinker.Core.SceneMaker;
using SocialLinker.Core.SceneMaker.Data.Bustup;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialLinker.Core.Menus.MakerMulti.Reactions
{
    class Utility
    {
        public static string Sprite_Number_Parser(string sprite_number, SocialLinkerCommand sl_command, int current_character)
        {
            char[] sprite_number_delimiters = { '-', '_', '.', ',' };

            // Create an empty string list. This is where the sprite specifier will be temporarily held and processed.
            List<string> sprite_number_temp;

            // Take the contents of the current iterated index of input_substring and split it by the characters specified in the char array just created.
            // This will be assigned to the newly created string list.
            sprite_number_temp = sprite_number.Split(sprite_number_delimiters).ToList();

            // Ensure there are only three numbers for the base sprite, eye frame, and mouth frame.
            // If there are more than three indices in the sprite_number_temp string list, send an error message and return.
            if (sprite_number_temp.Count > 3)
            {
                return "Too_Many_Animation_Frames";
            }

            // Iterate through the entries of the sprite_number_temp string list.
            // Here, we'll test if each one is an integer.
            for (int i = 0; i < sprite_number_temp.Count; i++)
            {
                // If the contents of the index currently being iterated on can be successfully converted to an integer, do nothing.
                if (int.TryParse(sprite_number_temp[i], out int integer_test) == true)
                {
                    // If it is a digit, do nothing.
                }
                // If not, send an error message and return. We only want integer values at this step.
                else
                {
                    return "Non_Digit_In_Sprite_Number";
                }
            }

            Assign_Sprite_Number_To_Character(sprite_number_temp, sl_command, current_character);

            return $"Success";
        }

        public static void Assign_Sprite_Number_To_Character(List<string> sprite_number, SocialLinkerCommand sl_command, int current_character)
        {
            MakerCharacterData temp_character_data = new MakerCharacterData();

            // If the number of indices present is one, the user only specified the base sprite.
            if (sprite_number.Count == 1)
            {
                temp_character_data.Base_Sprite = Int32.Parse(sprite_number[0]);
            }
            // If the number of indices present is two, the user specified both the base sprite and the eye frame.
            else if (sprite_number.Count == 2)
            {
                temp_character_data.Base_Sprite = Int32.Parse(sprite_number[0]);
                temp_character_data.Eye_Frame = Int32.Parse(sprite_number[1]);
            }
            // If the number of indices present is three, the user specified the base sprite, eye frame, and mouth frame.
            else if (sprite_number.Count == 3)
            {
                temp_character_data.Base_Sprite = Int32.Parse(sprite_number[0]);
                temp_character_data.Eye_Frame = Int32.Parse(sprite_number[1]);
                temp_character_data.Mouth_Frame = Int32.Parse(sprite_number[2]);
            }

            switch (current_character)
            {
                case 1:
                    sl_command.MakerCommand.Character_Data_1.Base_Sprite = temp_character_data.Base_Sprite;
                    sl_command.MakerCommand.Character_Data_1.Eye_Frame = temp_character_data.Eye_Frame;
                    sl_command.MakerCommand.Character_Data_1.Mouth_Frame = temp_character_data.Mouth_Frame;
                    break;

                case 2:
                    sl_command.MakerCommand.Character_Data_2.Base_Sprite = temp_character_data.Base_Sprite;
                    sl_command.MakerCommand.Character_Data_2.Eye_Frame = temp_character_data.Eye_Frame;
                    sl_command.MakerCommand.Character_Data_2.Mouth_Frame = temp_character_data.Mouth_Frame;
                    break;

                case 3:
                    sl_command.MakerCommand.Character_Data_3.Base_Sprite = temp_character_data.Base_Sprite;
                    sl_command.MakerCommand.Character_Data_3.Eye_Frame = temp_character_data.Eye_Frame;
                    sl_command.MakerCommand.Character_Data_3.Mouth_Frame = temp_character_data.Mouth_Frame;
                    break;

                case 4:
                    sl_command.MakerCommand.Character_Data_4.Base_Sprite = temp_character_data.Base_Sprite;
                    sl_command.MakerCommand.Character_Data_4.Eye_Frame = temp_character_data.Eye_Frame;
                    sl_command.MakerCommand.Character_Data_4.Mouth_Frame = temp_character_data.Mouth_Frame;
                    break;
            }
        }

        public static bool Base_Sprite_Validity_Check(MakerCharacterData character_data)
        {
            var set_data = character_data.Set_Data;

            // Establish the directory of the specified sprite set.
            string set_path = $@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//{set_data.Origin}//Bustup//{set_data.ID}";

            // Get a count of how many files are in the sprite set's directory.
            int filecount = OfficialSetMethods.AttachmentCountItemDirectory(set_path);

            // Now that we have a filecount for the set, let's see if the inputted sprite number is valid before we continue.
            // If not, send an error message and cancel the request.
            if (character_data.Base_Sprite > filecount)
            {
                return false;
            }

            return true;
        }

        public static OfficialSetData ValidateCharacter(SocialLinkerCommand multimaker_session, UserInfoFields account, string input_string)
        {
            MakerCommandData maker_command = new MakerCommandData()
            {
                Character_Data_1 = new MakerCharacterData()
                {
                    Sprite_Set_Version = multimaker_session.MakerCommand.Template,
                    Character_Keyword = input_string
                }
            };

            return OfficialSetMethods.GetSpriteSetInfo(account, maker_command);
        }


        // Bustup construction (Maker-Multi)
        public static Bitmap Bustup_Selection(MenuIdStructure menuSession, UserInfoFields account, MakerCharacterData maker_character_data, int current_character)
        {
            OfficialSetData set_data = maker_character_data.Set_Data;

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

            // If eye frames and mouth frames were not specified, return the base sprite.
            if (maker_character_data.Eye_Frame == default && maker_character_data.Mouth_Frame == default)
            {
                Bitmap base_sprite = (Bitmap)System.Drawing.Image.FromFile($@"{set_path}//{base_sprite_filename}.png");
                return base_sprite;
            }
            else
            {
                Bitmap base_sprite = (Bitmap)System.Drawing.Image.FromFile($@"{set_path}//{base_sprite_filename}.png");

                // We need a temporary bustup_data object here to check for associated animation frames
                maker_character_data.Bustup_Data = BustupDataMethods.Get_Bustup_Data(account, set_data, maker_character_data);

                Console.WriteLine("Are we going here?");

                Bitmap bustup_with_frames = Construct_Bustup_With_Frames(menuSession, maker_character_data, base_sprite, false);
                return bustup_with_frames;
            }
        }

        public static Bitmap Reverse_Bustup_Selection(MenuIdStructure menuSession, MakerCharacterData maker_character_data, Bitmap bustup)
        {
            OfficialSetData set_data = maker_character_data.Set_Data;
            BustupData bustup_data = maker_character_data.Bustup_Data;

            string reverse_path = $@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//{set_data.Origin}//Bustup//{set_data.ID}//Reverse";
            string base_sprite_filename = $"r{bustup_data.Filename.Substring(1)}";

            if (File.Exists($"{reverse_path}//{base_sprite_filename}"))
            {
                Bitmap base_sprite = (Bitmap)System.Drawing.Image.FromFile($@"{reverse_path}//{base_sprite_filename}");

                // Do something
                if (maker_character_data.Eye_Frame == default && maker_character_data.Mouth_Frame == default)
                {
                    return base_sprite;
                }
                else
                {
                    Bitmap bustup_with_frames = Construct_Bustup_With_Frames(menuSession, maker_character_data, base_sprite, true);
                    return bustup_with_frames;
                }
            }
            else
            {
                return bustup;
            }
        }

        public static Bitmap Construct_Bustup_With_Frames(MenuIdStructure menuSession, MakerCharacterData maker_character_data, Bitmap bustup, bool reverse_file_exists)
        {
            Bitmap edited_bustup = bustup;

            FrameData eye_frame_data = default;
            FrameData mouth_frame_data = default;
            Bitmap eye_frame_sprite = default;
            Bitmap mouth_frame_sprite = default;

            OfficialSetData set_data = maker_character_data.Set_Data;
            BustupData bustup_data = maker_character_data.Bustup_Data;

            if (maker_character_data.Eye_Frame != default && maker_character_data.Eye_Frame != 0)
            {
                // Establish the eye frame directory for the current sprite set.
                string eye_frame_path = "";

                switch (reverse_file_exists)
                {
                    case true:
                        eye_frame_path = $@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//{set_data.Origin}//Bustup//{set_data.ID}//Reverse//Eyes";
                        break;

                    case false:
                        eye_frame_path = $@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//{set_data.Origin}//Bustup//{set_data.ID}//Eyes";
                        break;
                }

                // Get the eye frame data of the frame specified in the user's command.
                eye_frame_data = BustupDataMethods.Get_Eye_Frame_Data(set_data, bustup_data, maker_character_data);

                // Ensure that the returned eye frame data is not null.
                if (eye_frame_data != null)
                {
                    string eye_frame_filename = "";

                    switch (reverse_file_exists)
                    {
                        case true:
                            eye_frame_filename = $"r{eye_frame_data.Filename.Substring(1)}";
                            break;

                        case false:
                            eye_frame_filename = eye_frame_data.Filename;
                            break;
                    }

                    // Check that the eye frame path exists.
                    if (File.Exists($"{eye_frame_path}//{eye_frame_filename}"))
                    {
                        // Save the eye frame to a bitmap variable.
                        eye_frame_sprite = (Bitmap)System.Drawing.Image.FromFile($@"{eye_frame_path}//{eye_frame_filename}");

                        // Depending on the bustup's game origin, a section of the base bustup may need to be cropped out to make the eye frame properly fit in.
                        if (set_data.Origin == "P5-PS4" || set_data.Origin == "P5R" || set_data.Origin == "P5S")
                        {
                            Rectangle crop_region_eyes = new Rectangle(eye_frame_data.Coord_X, eye_frame_data.Coord_Y, eye_frame_data.Scale_Width, eye_frame_data.Scale_Height);

                            edited_bustup = Crop_Rectangle_From_Bitmap(edited_bustup, crop_region_eyes);
                        }
                    }
                }
                // If the frame data is null, send an error message and return null as well.
                else
                {
                    menuSession.MenuTimer.Stop();
                    throw new EyeFrameNotFoundException();
                }
            }

            if (maker_character_data.Mouth_Frame != default && maker_character_data.Mouth_Frame != 0)
            {
                // Establish the mouth frame directory for the current sprite set.
                string mouth_frame_path = "";

                switch (reverse_file_exists)
                {
                    case true:
                        mouth_frame_path = $@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//{set_data.Origin}//Bustup//{set_data.ID}//Reverse//Mouth";
                        break;

                    case false:
                        mouth_frame_path = $@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//{set_data.Origin}//Bustup//{set_data.ID}//Mouth";
                        break;
                }

                // Get the mouth frame data of the frame specified in the user's command.
                mouth_frame_data = BustupDataMethods.Get_Mouth_Frame_Data(set_data, bustup_data, maker_character_data);

                // Ensure that the returned mouth frame data is not null.
                if (mouth_frame_data != null)
                {
                    string mouth_frame_filename = "";

                    switch (reverse_file_exists)
                    {
                        case true:
                            mouth_frame_filename = $"r{mouth_frame_data.Filename.Substring(1)}";
                            break;

                        case false:
                            mouth_frame_filename = mouth_frame_data.Filename;
                            break;
                    }

                    // Check that the mouth frame path exists.
                    if (File.Exists($"{mouth_frame_path}//{mouth_frame_filename}"))
                    {
                        // Save the mouth frame to a bitmap variable.
                        mouth_frame_sprite = (Bitmap)System.Drawing.Image.FromFile($@"{mouth_frame_path}//{mouth_frame_filename}");

                        // Depending on the bustup's game origin, a section of the base bustup may need to be cropped out to make the eye frame properly fit in.
                        if (set_data.Origin == "P5-PS4" || set_data.Origin == "P5R" || set_data.Origin == "P5S")
                        {
                            Rectangle crop_region_mouth = new Rectangle(mouth_frame_data.Coord_X, mouth_frame_data.Coord_Y, mouth_frame_data.Scale_Width, mouth_frame_data.Scale_Height);

                            edited_bustup = Crop_Rectangle_From_Bitmap(edited_bustup, crop_region_mouth);
                        }
                    }
                }
                // If the frame data is null, send an error message and return null as well.
                else
                {
                    menuSession.MenuTimer.Stop();
                    throw new MouthFrameNotFoundException();
                }
            }

            // Draw the frames to the cropped bustup.
            using (Graphics graphics = Graphics.FromImage(edited_bustup))
            {
                if (mouth_frame_sprite != default && mouth_frame_data != default)
                {
                    graphics.DrawImage(mouth_frame_sprite, mouth_frame_data.Coord_X, mouth_frame_data.Coord_Y, mouth_frame_data.Scale_Width, mouth_frame_data.Scale_Height);
                }

                if (eye_frame_sprite != default && eye_frame_data != default)
                {
                    graphics.DrawImage(eye_frame_sprite, eye_frame_data.Coord_X, eye_frame_data.Coord_Y, eye_frame_data.Scale_Width, eye_frame_data.Scale_Height);
                }
            }

            return edited_bustup;
        }

        public static Bitmap Crop_Rectangle_From_Bitmap(Bitmap input_bitmap, Rectangle crop_region)
        {
            // Create a color variable. We'll use this to iterate through the input bitmap and store each pixel's color values here.
            System.Drawing.Color actual_color;

            // Make an empty bitmap the same size as the input bitmap.
            Bitmap new_bitmap = new Bitmap(input_bitmap.Width, input_bitmap.Height);

            // Create a for loop to iterate over the X values of the bitmap to be changed.
            for (int x = 0; x < input_bitmap.Width; x++)
            {
                // Create a nested for loop to iterate over the Y values of the bitmap to be changed.
                for (int y = 0; y < input_bitmap.Height; y++)
                {
                    // Get the current pixel from the input bitmap.
                    actual_color = input_bitmap.GetPixel(x, y);

                    if ((x > crop_region.X && x < (crop_region.X + crop_region.Width)) && (y > crop_region.Y && y < (crop_region.Y + crop_region.Height)))
                    {
                        // Do nothing
                    }
                    else
                    {
                        new_bitmap.SetPixel(x, y, actual_color);
                    }
                }
            }

            return new_bitmap;
        }

        public static Task<bool> Process_Character(SocialLinkerCommand multimaker_session, MenuIdStructure menuSession, UserInfoFields account, SocketModal modal, string character_input, string sprite_input, int current_character)
        {
            var character_set_data = Utility.ValidateCharacter(multimaker_session, account, character_input);

            if (character_set_data == null)
            {
                menuSession.MenuTimer.Stop();
                modal.DeferAsync(ephemeral: true);

                switch (current_character)
                {
                    case 1:
                    case 2:
                        _ = MakerMulti_Char_Details_Pt1_Menu.MakerMulti_Char_Details_Pt1_Invalid_Character(menuSession.User, menuSession.MenuMessage, character_input);
                        break;

                    case 3:
                    case 4:
                        //_ = MakerMulti_Char_Details_Pt2_Menu.MakerMulti_Char_Entry_2_Invalid_Character(menuSession.User, menuSession.MenuMessage, character_input);
                        break;
                }

                return Task.FromResult(false);
            }

            MakerCharacterData general_character_data = null;

            switch (current_character)
            {
                case 1:
                    multimaker_session.MakerCommand.Character_Data_1.Set_Data = character_set_data;
                    general_character_data = multimaker_session.MakerCommand.Character_Data_1;
                    break;

                case 2:
                    multimaker_session.MakerCommand.Character_Data_2.Set_Data = character_set_data;
                    general_character_data = multimaker_session.MakerCommand.Character_Data_2;
                    break;

                case 3:
                    multimaker_session.MakerCommand.Character_Data_3.Set_Data = character_set_data;
                    general_character_data = multimaker_session.MakerCommand.Character_Data_3;
                    break;

                case 4:
                    multimaker_session.MakerCommand.Character_Data_4.Set_Data = character_set_data;
                    general_character_data = multimaker_session.MakerCommand.Character_Data_4;
                    break;
            }

            // Process Character Sprite Number
            string parsed_sprite_number = Utility.Sprite_Number_Parser(sprite_input, multimaker_session, current_character);

            switch (parsed_sprite_number)
            {
                case "Success":
                    break;

                case "Too_Many_Animation_Frames":
                    menuSession.MenuTimer.Stop();
                    modal.DeferAsync(ephemeral: true);
                    switch (current_character)
                    {
                        case 1:
                        case 2:
                            _ = MakerMulti_Char_Details_Pt1_Menu.MakerMulti_Char_Details_Pt1_Sprite_Select_Too_Many_Animation_Frames(menuSession.User, menuSession.MenuMessage, general_character_data);
                            break;

                        case 3:
                        case 4:
                            //_ = MakerMulti_Char_Details_Pt2_Menu.MakerMulti_Char_Details_Pt2_Sprite_Select_Too_Many_Animation_Frames(menuSession.User, menuSession.MenuMessage, general_character_data);
                            break;
                    }
                    return Task.FromResult(false);

                case "Non_Digit_In_Sprite_Number":
                    menuSession.MenuTimer.Stop();
                    modal.DeferAsync(ephemeral: true);
                    _ = MakerMulti_Char_Details_Pt1_Menu.MakerMulti_Char_Details_Pt1_Sprite_Select_Non_Digit_In_Sprite_Number(menuSession.User, menuSession.MenuMessage, general_character_data);
                    return Task.FromResult(false);
            }

            if (Utility.Base_Sprite_Validity_Check(general_character_data) == false)
            {
                menuSession.MenuTimer.Stop();
                modal.DeferAsync(ephemeral: true);
                _ = MakerMulti_Char_Details_Pt1_Menu.MakerMulti_Char_Details_Pt1_Invalid_Base_Sprite(menuSession.User, menuSession.MenuMessage, general_character_data);
                return Task.FromResult(false);
            }
            if ((general_character_data.Base_Sprite == 0) && ((general_character_data.Eye_Frame != default) || (general_character_data.Mouth_Frame != default)))
            {
                menuSession.MenuTimer.Stop();
                modal.DeferAsync(ephemeral: true);
                _ = MakerMulti_Char_Details_Pt1_Menu.MakerMulti_Char_Details_Pt1_Sprite_Select_Animation_Frame_With_Blank_Sprite(menuSession.User, menuSession.MenuMessage, general_character_data);
                return Task.FromResult(false);
            }

            if (general_character_data.Base_Sprite != 0)
            {
                try
                {
                    switch (current_character)
                    {
                        case 1:
                            multimaker_session.MakerCommand.Character_Data_1.Bustup_Data = BustupDataMethods.Get_Bustup_Data(account, general_character_data.Set_Data, general_character_data);
                            break;

                        case 2:
                            multimaker_session.MakerCommand.Character_Data_2.Bustup_Data = BustupDataMethods.Get_Bustup_Data(account, general_character_data.Set_Data, general_character_data);
                            break;

                        case 3:
                            multimaker_session.MakerCommand.Character_Data_3.Bustup_Data = BustupDataMethods.Get_Bustup_Data(account, general_character_data.Set_Data, general_character_data);
                            break;

                        case 4:
                            multimaker_session.MakerCommand.Character_Data_4.Bustup_Data = BustupDataMethods.Get_Bustup_Data(account, general_character_data.Set_Data, general_character_data);
                            break;
                    }
                }
                catch (EyeFrameNotFoundException)
                {
                    menuSession.MenuTimer.Stop();
                    modal.DeferAsync(ephemeral: true);

                    switch (current_character)
                    {
                        case 1:
                        case 2:
                            _ = MakerMulti_Char_Details_Pt1_Menu.MakerMulti_Char_Details_Pt1_Sprite_Select_Eye_Frame_Not_Found(menuSession.User, menuSession.MenuMessage, general_character_data);
                            break;

                        case 3:
                        case 4:
                            //_ = MakerMulti_Char_Entry_2_Menu.MakerMulti_Char_Entry_2_Sprite_Select_Eye_Frame_Not_Found(menuSession.User, menuSession.MenuMessage, general_character_data);
                            break;
                    }

                    return Task.FromResult(false);
                }
                catch (MouthFrameNotFoundException)
                {
                    menuSession.MenuTimer.Stop();
                    modal.DeferAsync(ephemeral: true);

                    switch (current_character)
                    {
                        case 1:
                        case 2:
                            _ = MakerMulti_Char_Details_Pt1_Menu.MakerMulti_Char_Details_Pt1_Sprite_Select_Mouth_Frame_Not_Found(menuSession.User, menuSession.MenuMessage, general_character_data);
                            break;

                        case 3:
                        case 4:
                            //_ = MakerMulti_Char_Entry_2_Menu.MakerMulti_Char_Entry_2_Sprite_Select_Mouth_Frame_Not_Found(menuSession.User, menuSession.MenuMessage, general_character_data);
                            break;
                    }

                    return Task.FromResult(false);
                }
            }

            return Task.FromResult(true);
        }

        public class EyeFrameNotFoundException : Exception
        {
            public EyeFrameNotFoundException() : base("Eye frame not found") { }
        }

        public class MouthFrameNotFoundException : Exception
        {
            public MouthFrameNotFoundException() : base("Mouth frame not found") { }
        }
    }
}
