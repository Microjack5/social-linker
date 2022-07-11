using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Immutable;
using Discord.WebSocket;
using SocialLinker.Core.LocalStorageTables;
using SocialLinker.Core.CloudStorageTables;
using SocialLinker.Config;

namespace SocialLinker.Core.SceneMaker
{
    public class CommandParser
    {
        public static async Task Type_Directory(SocialLinkerCommand sl_command)
        {
            switch (sl_command.CommandType)
            {
                case "Context":
                    // Split the command prefix and name from the content
                    List<string> input_substring;
                    char[] delimiterChars = { ' ' };
                    input_substring = sl_command.Message.Content.Split(delimiterChars).ToList();

                    if (input_substring.Count > 1)
                    {
                        input_substring.RemoveAt(0);
                    }

                    string message_content = String_List_To_String(input_substring);

                    await CommandParser.Parser(sl_command, message_content);
                    break;

                case "Slash":
                    var account = UserInfoClasses.GetAccount(sl_command.User);
                    MakerCommandData maker_command = sl_command.MakerCommand;
                    OfficialSetData sprite_set_info = null;

                    switch (sl_command.CommandName)
                    {
                        case "maker_list":
                            if (maker_command.Template != "" && maker_command.Character_Keyword == "")
                            {
                                await OfficialSetMethods.Set_List_Message_Directory(sl_command);
                            }
                            break;

                        case "maker_view":
                            // If the base sprite is not at the default value but the eye frames and mouth frames are, we have a successful command! Generate an image viewing the details for the specified character sprite.
                            if (maker_command.Base_Sprite != default)
                            {
                                // Test for zero entries in the sprite specifiers.
                                // If the base sprite was read in as zero, send an error message and return.
                                // The base sprite being zero indicates the lack of a sprite, so sprite details are impossible to view.
                                if (maker_command.Base_Sprite == 0)
                                {
                                    await ErrorHandling.Viewing_Sprite_Details_With_Blank_Sprite(sl_command);
                                    return;
                                }

                                // Get the information of the chosen sprite set.
                                sprite_set_info = OfficialSetMethods.GetSpriteSetInfo(account, maker_command);

                                // If the sprite set info is not null, start creating a sprite sheet detailing the chosen sprite's frames.
                                // The first step of this is checking the validity of the user's inputted base sprite in relation to the chosen set.
                                if (sprite_set_info != null)
                                {
                                    OfficialSetMethods.Base_Sprite_Validity_Check(sl_command, sprite_set_info, maker_command);
                                    return;
                                }
                                // If the sprite set info is null, send an error message. The sprite set doesn't exist.
                                else
                                {
                                    await ErrorHandling.Sprite_Set_Not_Found_Generic(sl_command, maker_command.Character_Keyword);
                                    return;
                                }
                            }

                            else if (maker_command.Character_Keyword != "" && maker_command.Sprite_Set_Version == "")
                            {
                                // Get the information of the chosen sprite set.
                                sprite_set_info = OfficialSetMethods.GetSpriteSetInfo(account, maker_command);

                                // If the sprite set info is not null, decide how to generate the embeded message.
                                if (sprite_set_info != null)
                                {
                                    await OfficialSetMethods.Sprite_Sheet_Message_Directory(sl_command, sprite_set_info);
                                    return;
                                }
                                // If the sprite set info is null, send an error message. The sprite set doesn't exist.
                                else
                                {
                                    await ErrorHandling.Sprite_Set_Not_Found_Generic(sl_command, maker_command.Character_Keyword);
                                    return;
                                }
                            }
                            // If both the character keyword and the sprite sheet specifier are not empty, we have a successful command! Generate a character sprite sheet from the specified title.
                            else if (maker_command.Character_Keyword != "" && maker_command.Sprite_Set_Version != "")
                            {
                                // Get the information of the chosen sprite set.
                                sprite_set_info = OfficialSetMethods.GetSpriteSetInfo(account, maker_command);

                                // If the sprite set info is not null, decide how to generate the embeded message.
                                if (sprite_set_info != null)
                                {
                                    await OfficialSetMethods.Sprite_Sheet_Message_Directory(sl_command, sprite_set_info);
                                    return;
                                }
                                // If the sprite set info is null, send an error message. The sprite set doesn't exist.
                                else
                                {
                                    await ErrorHandling.Sprite_Set_Not_Found_Generic(sl_command, maker_command.Character_Keyword);
                                    return;
                                }
                            }

                            
                            break;

                        case "maker_create":
                            if (maker_command.Character_Keyword.ToLower() == "system")
                            {
                                //await System_Message_Parser(sl_command, sl_command.Content);
                                return;
                            }
                            else if (maker_command.Character_Keyword.ToLower() == "dual")
                            {
                                // Redirect to dual scene maker menu
                                return;
                            }
                            else if (maker_command.Character_Keyword.ToLower() == "help")
                            {
                                // Redirect to scene maker help menu
                                return;
                            }

                            // If the base sprite was read in as zero and the eye frame or mouth frame values are not empty, send an error message and return.
                            // Zero can be read in as the base sprite, but eye and mouth frames can't be specified after it.
                            else if ((maker_command.Base_Sprite == 0) && ((maker_command.Eye_Frame != default) || (maker_command.Mouth_Frame != default)))
                            {
                                await ErrorHandling.Animation_Frame_With_Blank_Sprite(sl_command);
                                return;
                            }

                            // Get the information of the chosen sprite set.
                            sprite_set_info = OfficialSetMethods.GetSpriteSetInfo(account, maker_command);

                            Console.WriteLine(maker_command.Character_Keyword);
                            Console.WriteLine(maker_command.Sprite_Set_Version);

                            // If the sprite set's info returns null, it means the character keyword the user typed doesn't exist in the files.
                            // If this happens and the user didn't specify a template, send a generic "set not found" error message.
                            if (sprite_set_info == null && maker_command.Sprite_Set_Version == "")
                            {
                                await ErrorHandling.Sprite_Set_Not_Found_Generic(sl_command, maker_command.Character_Keyword);
                                return;
                            }
                            // Else, if this happens and the user did specify a template, send a "set not found" error message specifying the template.
                            else if (sprite_set_info == null && maker_command.Sprite_Set_Version != "")
                            {
                                await ErrorHandling.Sprite_Set_Not_Found_In_Template(sl_command, maker_command.Character_Keyword, OfficialSetMethods.InputToTemplate(account, maker_command.Sprite_Set_Version));
                                return;
                            }
                            // If the sprite set did not return null, the command was successful!
                            else if (sprite_set_info != null)
                            {
                                await OfficialSetMethods.Quick_Scene_Directory(sl_command, sprite_set_info, maker_command);
                            }
                            break;
                    }
                    break;
            }
        }

        public static async Task Parser(SocialLinkerCommand sl_command, string message_content)
        {
            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(sl_command.User);

            var message = sl_command.Message;

            Console.WriteLine(message.Content.ToString());

            // Create an OfficialSetData variable and set it to null.
            // We'll be assigning a proper object to it depending on how far we progress through the method.
            OfficialSetData sprite_set_info = null;

            // Create a new MakerCommandData object.
            // Throughout the function, we'll be filling the parameters out according to what is parsed from the user's command.
            MakerCommandData command_data = new MakerCommandData()
            {
                Template = "",
                Character_Keyword = "",
                Sprite_Set_Version = "",
                Base_Sprite = default,
                Eye_Frame = default,
                Mouth_Frame = default,
                Dialogue = ""
            };

            // Declare other variables that will be needed throughout the method.
            int iterator = 0;
            string current_string = "";

            // Create an empty string list. This is where the user's input will go.
            List<string> input_substring;

            // Create a char array that contains a whitespace.
            char[] delimiterChars = { ' ' };

            // Using the char array, split the user's input into indicies of the string list by seperating each one per whitespace.
            input_substring = message_content.Split(delimiterChars).ToList();

            // Iterate through the list to ensure no index is a whitespace.
            for (int i = input_substring.Count - 1; i >= 0; i--)
            {
                // If there is, remove the index.
                if (input_substring[i] == "")
                {
                    input_substring.RemoveAt(i);
                }
            }

            // If there are no indicies in the input_substring string list, we have a successful command! Generate a tutorial menu and return.
            if (input_substring.Count == 0)
            {
                await sl_command.Channel.SendMessageAsync(":white_check_mark: **Parsing successful.** A tutorial menu for the scene maker will be displayed.");
                return;
            }

            // Create two individual string arrays for template keywords: One containing generic keywords, and one containing version keywords.
            string[] generic_keywords = { "p1", "p2", "p2is", "p2ep", "p3", "p4", "p4a", "p4au", "p4u", "p4u2", "p4d", "p5", "p5s", "bbtag" };
            string[] version_keywords = { "p1-ps1", "p1-psx", "p1-psp", "p1p", "p2is-ps1", "p2is-psx", "p2is-psp", "p2isp", "p2ep-ps1", "p2ep-psx", "p2ep-psp", "p2epp", "p3f", "fes", "p3fes", "p3-ps2", "p3f-ps2", "fes-ps2", "p3fes-ps2", "p3p", "p3-psp", "p4-ps2", "p4g", "p4a", "p4au", "p4u", "p4u2", "p4d", "p5-ps3", "p5-ps4", "p5r", "p5r-ps4", "p5s", "bbtag" };

            // Assign the first word of the user's input after the "maker" prefix to the empty "current_string" variable.
            // The int variable "iterator" is currently set at 0, so this will retrieve the first index of the string list containing the user's processed input.
            current_string += input_substring[iterator];

            // First, let's assume we're looking for a generic theme keyword.
            // Iterate through every index in the generic_keywords array to check if the current string is a match.
            for (int i = 0; i < generic_keywords.Length; i++)
            {
                // If a match is found, the user specified a template! Assign the current string to the empty "template" string variable.
                // Revome the current string from the input substring list afterwards, bringing the next string to index 0.
                if (current_string.ToLower() == generic_keywords[i])
                {
                    command_data.Template = current_string;
                    input_substring.RemoveAt(iterator);
                    break;
                }
            }

            // Let's assume a generic keyword was not found during the last step. 
            // If the "template" string is empty, a generic keyword wasn't assigned to it. Let's try searching for a version keyword next.
            if (command_data.Template == "")
            {
                // Iterate through every index in the version_keywords array to check if the current string is a match.
                for (int i = 0; i < version_keywords.Length; i++)
                {
                    // If a match is found, the user specified a template! Assign the current string to the empty "template" string variable.
                    // Revome the current string from the input substring list afterwards, bringing the next string to index 0.
                    if (current_string.ToLower() == version_keywords[i])
                    {
                        command_data.Template = current_string;
                        input_substring.RemoveAt(iterator);
                        break;
                    }
                }
            }

            // Count the current number of elements in the input_substring list.
            // If the count is "0", there are no more strings to process.
            if (input_substring.Count == 0)
            {
                // If the template string is no longer empty and the character keyword string is, the user wants to generate a character list.
                if (command_data.Template != "" && command_data.Character_Keyword == "")
                {
                    // Convert the user's input template into a usable form that follows their version control settings.
                    command_data.Template = OfficialSetMethods.InputToTemplate(account, command_data.Template);

                    // Generate the sprite set list for the user's selected title.
                    await OfficialSetMethods.Set_List_Message_Directory(sl_command);
                }
                return;
            }

            // Reset current_string variable so that it is empty to take the next string, which should be the character keyword.
            current_string = "";

            // Next, let's start searching for a character keyword.
            // Ensure that the current index of the input_substring list (which should be 0 at this point) is not null.
            if (input_substring[iterator] != null)
            {
                // Iterate through the input_substring list. This is an iteration seperate from the "iterator" int variable.
                for (int i = 0; i < input_substring.Count; i++)
                {
                    // Confirm that the first character of the current index is not a digit.
                    // This is how we detect coming across the command's possible sprite number.
                    if (Char.IsDigit(input_substring[i], 0) == false)
                    {
                        // If it's not a digit, this is likely part of a character keyword.
                        // Add a space to the current_string variable if the iterator's value is greater than 0.
                        // This means that more than one string in the list matches this condition.
                        if (i > 0)
                        {
                            current_string += " ";
                        }

                        // Add the contents of the current index to the current_string variable.
                        current_string += input_substring[i];
                    }
                    // If the first character of the current index is a digit, we want to perform a few checks here first.
                    
                    if (Char.IsDigit(input_substring[i], 0) == true)
                    {
                        // If we've reached the end of the user input OR the next index in the substring array contains a quotation mark,
                        // assign the "iterator" variable to the index stopped at and break the loop.
                        // We have likely encountered the start of the sprite number.
                        if (i == input_substring.Count - 1 || input_substring[i + 1].Contains("\""))
                        {
                            iterator = i;
                            break;
                        }
                        // If neither of those conditions have been reached, the number we've encountered is likely meant to be part of the character keyword.
                        else
                        {
                            // Add a space to the current_string variable if the iterator's value is greater than 0.
                            // This means that more than one string in the list matches this condition.
                            if (i > 0)
                            {
                                current_string += " ";
                            }

                            // Add the contents of the current index to the current_string variable.
                            current_string += input_substring[i];
                        }
                    }
                }
            }

            // Now, let's analyze the character keyword we have and see if there's a theme specifier at the end.
            // Create an empty string list.
            List<string> char_temp;

            // Split the current_string variable by any whitespaces and assign all parts to the newly created string list.
            char_temp = current_string.Split(delimiterChars).ToList();

            // Check if the char_temp list has more than one index.
            if (char_temp.Count > 1)
            {
                // If so, start iterating through the generic_keywords string list. There may be a template keyword present at the end of the character keyword.
                for (int i = 0; i < generic_keywords.Length; i++)
                {
                    // Take the last index of the char_temp list and compare it against the current generic_keywords index iteration.
                    if (char_temp[char_temp.Count - 1].ToLower() == generic_keywords[i])
                    {
                        // If they match, a generic keyword specifying which game to pull the character's sprites from is present.
                        // Assign the last index of the char_temp list to the "character_sheet" string variable.
                        // Afterwards, remove the last index of char_temp from the list and break the loop. All that should remain is the character keyword.
                        command_data.Sprite_Set_Version = char_temp[char_temp.Count - 1];
                        char_temp.RemoveAt(char_temp.Count - 1);
                        break;
                    }
                }

                // Check if the Sprite_Set_Version variable is still empty. If so, this means the char_temp list did not contain a generic keyword.
                if (command_data.Sprite_Set_Version == "")
                {
                    // Next, let's start checking for version keywords.
                    // Start iterating through the version_keywords string list. There may be a template keyword present at the end of the character keyword.
                    for (int i = 0; i < version_keywords.Length; i++)
                    {
                        // Take the last index of the char_temp list and compare it against the current version_keywords index iteration.
                        if (char_temp[char_temp.Count - 1].ToLower() == version_keywords[i])
                        {
                            // If they match, a version keyword specifying which game to pull the character's sprites from is present.
                            // Assign the last index of the char_temp list to the "Sprite_Set_Version" string variable.
                            // Afterwards, remove the last index of char_temp from the list and break the loop. All that should remain is the character keyword.
                            command_data.Sprite_Set_Version = char_temp[char_temp.Count - 1];
                            char_temp.RemoveAt(char_temp.Count - 1);
                            break;
                        }
                    }
                }
            }

            // Take the remaining entries in char_temp and place them in a single string declared earlier.
            command_data.Character_Keyword = string.Join(" ", char_temp.ToArray());

            // Before we go any further, we'll want to check if the user entered any special keywords in the character keyword's place.
            // These keywords will trigger special scene maker functions and cannot be used as part of a character's access keyword.
            if (command_data.Character_Keyword.ToLower() == "system")
            {
                await System_Message_Parser(sl_command, message.Content);
                return;
            }
            else if (command_data.Character_Keyword.ToLower() == "dual")
            {
                // Redirect to dual scene maker menu
                return;
            }
            else if (command_data.Character_Keyword.ToLower() == "help")
            {
                // Redirect to scene maker help menu
                return;
            }

            // At this point, we want to ensure the character keyword is not a template keyword we accidentally took in.
            // First, iterate through the generic_keywords string list.
            for (int i = 0; i < generic_keywords.Length; i++)
            {
                // Take the character_keyword variable and compare it against the current generic_keywords index iteration.
                if (command_data.Character_Keyword.ToLower() == generic_keywords[i])
                {
                    // If a match is found, send an error message. The command was improperly input.
                    await ErrorHandling.Char_Keyword_Not_Found(sl_command);
                    return;
                }
            }

            // Next, check the version keywords by iterating through the version_keywords string list.
            for (int i = 0; i < version_keywords.Length; i++)
            {
                // Take the character_keyword variable and compare it against the current version_keywords index iteration.
                if (command_data.Character_Keyword.ToLower() == version_keywords[i])
                {
                    // If a match is found, send an error message. The command was improperly input.
                    await ErrorHandling.Char_Keyword_Not_Found(sl_command);
                    return;
                }
            }

            // If there is a sprite number present, the iterator will be placed there now. If not, we have our needed keywords at this point.
            // Confirm that the first character of the input_substring index we left off at is not a digit.
            // If a sprite number is present, this is expected to return true. If not, this is expected to return false.
            if (Char.IsDigit(input_substring[iterator], 0) == false)
            {
                // Decide the case that the user accidentally forgot to enter a sprite number.
                // Create a char array for different types of quotation marks.
                char[] quotation_check = { '\u0022', '\u201C', '\u201D' };

                // Iterate through the quotation_check char array.
                for (int i = 0; i < quotation_check.Length; i++)
                {
                    // Compare the entirety of the character_keyword string against the current iteration of the quotation_check char array to check if a match exists.
                    // If so, the user entered a quotation mark prematurely. Quotation marks only come after a sprite number, so output an error message and return.
                    if (command_data.Character_Keyword.Contains(quotation_check[i]))
                    {
                        await ErrorHandling.Sprite_Number_Missing(sl_command);
                        return;
                    }
                }

                // Here, we want to check for other possible parsing conditions and account for other errors or command types.
                // Reminder: At this point, only the possible template keyword and character keyword should be taken in.
                // If both the template keyword and character keyword are not empty, send an error message. A template keyword without sprite number and dialogue is incorrect syntax.
                if (command_data.Template != "" && command_data.Character_Keyword != "")
                {
                    await ErrorHandling.Sprite_Number_And_Dialogue_Missing(sl_command);
                }
                // If the template keyword is not empty and the character keyword is empty, we have a successful command! Generate a character list from the specified title.
                else if (command_data.Template != "" && command_data.Character_Keyword == "")
                {
                    await OfficialSetMethods.Set_List_Message_Directory(sl_command);
                }
                // If the character keyword is not empty and the sprite sheet specifier is empty, we have a successful command! Generate a sprite sheet from the character's game of origin.
                else if (command_data.Character_Keyword != "" && command_data.Sprite_Set_Version == "")
                {
                    // Get the information of the chosen sprite set.
                    sprite_set_info = OfficialSetMethods.GetSpriteSetInfo(account, command_data);

                    // If the sprite set info is not null, decide how to generate the embeded message.
                    if (sprite_set_info != null)
                    {
                        await OfficialSetMethods.Sprite_Sheet_Message_Directory(sl_command, sprite_set_info);
                    }
                    // If the sprite set info is null, send an error message. The sprite set doesn't exist.
                    else
                    {
                        await ErrorHandling.Sprite_Set_Not_Found_Generic(sl_command, command_data.Character_Keyword);
                        return;
                    }
                }
                // If both the character keyword and the sprite sheet specifier are not empty, we have a successful command! Generate a character sprite sheet from the specified title.
                else if (command_data.Character_Keyword != "" && command_data.Sprite_Set_Version != "")
                {
                    // Get the information of the chosen sprite set.
                    sprite_set_info = OfficialSetMethods.GetSpriteSetInfo(account, command_data);

                    // If the sprite set info is not null, decide how to generate the embeded message.
                    if (sprite_set_info != null)
                    {
                        await OfficialSetMethods.Sprite_Sheet_Message_Directory(sl_command, sprite_set_info);
                    }
                    // If the sprite set info is null, send an error message. The sprite set doesn't exist.
                    else
                    {
                        await ErrorHandling.Sprite_Set_Not_Found_Generic(sl_command, command_data.Character_Keyword);
                        return;
                    }
                }
                return;
            }

            // If we've made it this far, there's still more command to process!
            // Remove everything before the current iterator point in the input_substring list.
            for (int i = 0; i < iterator; i++)
            {
                // Always remove the first index, since the index numbers of the elements will shift by one after a removal.
                input_substring.RemoveAt(0);
            }

            // Reset iterator to 0.
            iterator = 0;

            // Next, let's parse what's supposed to be the sprite number.
            // Create a char array containing characters that could be found in the sprite specifier.
            char[] sprite_number_delimiters = { '-', '_', '.', ',' };

            // Create an empty string list. This is where the sprite specifier will be temporarily held and processed.
            List<string> sprite_number_temp;

            // Take the contents of the current iterated index of input_substring and split it by the characters specified in the char array just created.
            // This will be assigned to the newly created string list.
            sprite_number_temp = input_substring[iterator].Split(sprite_number_delimiters).ToList();

            // Ensure there are only three numbers for the base sprite, eye frame, and mouth frame.
            // If there are more than three indices in the sprite_number_temp string list, send an error message and return.
            if (sprite_number_temp.Count > 3)
            {
                await ErrorHandling.Too_Many_Animation_Frames(sl_command);
                return;
            }

            // Iterate through the entries of the sprite_number_temp string list.
            // Here, we'll test if each one is an integer.
            for (int i = 0; i < sprite_number_temp.Count; i++)
            {
                // Create an int variable and initialize it to zero.
                // We'll need this int to confirm the type of the values in sprite_number_temp we're about to process.
                int integer_test = 0;

                // If the contents of the index currently being iterated on can be successfully converted to an integer, do nothing.
                if (int.TryParse(sprite_number_temp[i], out integer_test) == true)
                {
                    // If it is a digit, do nothing.
                }
                // If not, send an error message and return. We only want integer values at this step.
                else
                {
                    await ErrorHandling.Non_Digit_In_Sprite_Number(sl_command);
                    return;
                }
            }

            // Now that their validity as integers are confirmed, assign them to the proper variables depending on how many indices are available.
            // If the number of indices present is one, the user only specified the base sprite.
            if (sprite_number_temp.Count == 1)
            {
                command_data.Base_Sprite = Int32.Parse(sprite_number_temp[0]);
            }
            // If the number of indices present is two, the user specified both the base sprite and the eye frame.
            else if (sprite_number_temp.Count == 2)
            {
                command_data.Base_Sprite = Int32.Parse(sprite_number_temp[0]);
                command_data.Eye_Frame = Int32.Parse(sprite_number_temp[1]);
            }
            // If the number of indices present is three, the user specified the base sprite, eye frame, and mouth frame.
            else if (sprite_number_temp.Count == 3)
            {
                command_data.Base_Sprite = Int32.Parse(sprite_number_temp[0]);
                command_data.Eye_Frame = Int32.Parse(sprite_number_temp[1]);
                command_data.Mouth_Frame = Int32.Parse(sprite_number_temp[2]);
            }

            // The command input can potentially end here, so let's first test the case that there's still more input remaining.
            if (iterator != input_substring.Count - 1)
            {
                // If the user makes it this far, there should be a character keyword present since we've already taken in the sprite number.
                // If the character keyword is empty, send an error message and return.
                if (command_data.Character_Keyword == "")
                {
                    await ErrorHandling.Sprite_Number_Before_Char_Keyword(sl_command);
                    return;
                }
                // If the base sprite was read in as zero and the eye frame or mouth frame values are not empty, send an error message and return.
                // Zero can be read in as the base sprite, but eye and mouth frames can't be specified after it.
                else if ((command_data.Base_Sprite == 0) && ((command_data.Eye_Frame != default) || (command_data.Mouth_Frame != default)))
                {
                    await ErrorHandling.Animation_Frame_With_Blank_Sprite(sl_command);
                    return;
                }

                // Increase the iterator by one, placing it at the next index of the main input_substring string list.
                iterator++;
            }
            // If the iterator is on the last index of the input_substring string list, there is no more input to take in. Time to examine what we have!
            else
            {
                // Test for zero entries in the sprite specifiers.
                // If the base sprite was read in as zero and the eye frame or mouth frame values are both empty, send an error message and return.
                // The base sprite being zero indicates the lack of a sprite, so sprite details are impossible to view.
                if ((command_data.Base_Sprite == 0) && ((command_data.Eye_Frame == default) || (command_data.Mouth_Frame == default)))
                {
                    await ErrorHandling.Viewing_Sprite_Details_With_Blank_Sprite(sl_command);
                }
                // If the base sprite was read in as zero and the eye frame or mouth frame values are not empty, send an error message and return.
                // Zero can be read in as the base sprite, but eye and mouth frames can't be specified after it.
                // Eye frames and mouth frames being specified without dialouge afterwards is also incorrect syntax.
                else if ((command_data.Base_Sprite == 0) && ((command_data.Eye_Frame != default) || (command_data.Mouth_Frame != default)))
                {
                    await ErrorHandling.Animation_Frame_With_Blank_Sprite_And_Without_Dialogue(sl_command);
                }
                // If the base sprite is not at the default value and neither is the eye frame or mouth frame, send an error message and return.
                // Eye frames and mouth frames being specified without dialouge afterwards is incorrect syntax.
                else if ((command_data.Base_Sprite != default) && ((command_data.Eye_Frame != default) || (command_data.Mouth_Frame != default)))
                {
                    await ErrorHandling.Animation_Frames_Without_Dialogue(sl_command);
                }
                // If the character keyword is empty but the base sprite value is not, send an error message and return.
                // A character keyword must always come before the sprite number.
                else if ((command_data.Character_Keyword == "") && (command_data.Base_Sprite != default))
                {
                    await ErrorHandling.Char_Keyword_Not_Found(sl_command);
                }
                // If the base sprite is not at the default value but the eye frames and mouth frames are, we have a successful command! Generate an image viewing the details for the specified character sprite.
                else if ((command_data.Base_Sprite != default) && (command_data.Eye_Frame == default) && (command_data.Mouth_Frame == default))
                {
                    // Get the information of the chosen sprite set.
                    sprite_set_info = OfficialSetMethods.GetSpriteSetInfo(account, command_data);

                    // If the sprite set info is not null, start creating a sprite sheet detailing the chosen sprite's frames.
                    // The first step of this is checking the validity of the user's inputted base sprite in relation to the chosen set.
                    if (sprite_set_info != null)
                    {
                        OfficialSetMethods.Base_Sprite_Validity_Check(sl_command, sprite_set_info, command_data);
                    }
                    // If the sprite set info is null, send an error message. The sprite set doesn't exist.
                    else
                    {
                        await ErrorHandling.Sprite_Set_Not_Found_Generic(sl_command, command_data.Character_Keyword);
                        return;
                    }
                }
                return;
            }

            // Remove everything before the current iterator point in the input_substring list.
            // After this point, only the dialogue should remain.
            for (int i = 0; i < iterator; i++)
            {
                // Always remove the first index, since the index numbers of the elements will shift by one after a removal.
                input_substring.RemoveAt(0);
            }

            // Finally, let's search for the dialogue to be rendered.
            // Create an empty char list.
            List<char> dialogue_temp;

            // Convert the input_substring string list into a char array.
            char[] charArr = string.Join(" ", input_substring.ToArray()).ToCharArray();

            // Convert the char array to a char list and assign it to the empty char list variable recently created.
            dialogue_temp = charArr.ToList();

            // Ensure that the first index and the last index of the dialogue_temp char array contain quotation marks.
            // If they do, remove the first and last indices from the list.
            if ((dialogue_temp[0] == '\u0022' || dialogue_temp[0] == '\u201C') && (dialogue_temp[dialogue_temp.Count - 1] == '\u0022' || dialogue_temp[dialogue_temp.Count - 1] == '\u201D' || dialogue_temp[dialogue_temp.Count - 1] == '\u201E'))
            {
                dialogue_temp.RemoveAt(0);
                dialogue_temp.RemoveAt(dialogue_temp.Count - 1);
            }
            // If not, send an error message and return. Dialogue should always be placed between quotation marks.
            else
            {
                await ErrorHandling.Text_After_Sprite_Number_Not_Quoted(sl_command);
                return;
            }

            // Add each entry of the dialogue_temp char list to the dialogue string variable.
            foreach (char ch in dialogue_temp)
            {
                command_data.Dialogue += ch;
            }

            // Check if the dialogue variable is empty or full of whitespace. It should be filled with something.
            // Start off whitespace_check at 2 to account for the mandatory quotation marks.
            int whitespace_check = 2;

            for (int i = 0; i < charArr.Length; i++)
            {
                if (charArr[i] == ' ')
                {
                    whitespace_check++;
                }
            }

            // If the dialogue is filled with whitespace, replace it with ellipses.
            if (whitespace_check == charArr.Length)
            {
                command_data.Dialogue = "......";
            }

            // Get the information of the chosen sprite set.
            sprite_set_info = OfficialSetMethods.GetSpriteSetInfo(account, command_data);

            // If the sprite set's info returns null, it means the character keyword the user typed doesn't exist in the files.
            // If this happens and the user didn't specify a template, send a generic "set not found" error message.
            if (sprite_set_info == null && command_data.Sprite_Set_Version == "")
            {
                await ErrorHandling.Sprite_Set_Not_Found_Generic(sl_command, command_data.Character_Keyword);
                return;
            }
            // Else, if this happens and the user did specify a template, send a "set not found" error message specifying the template.
            else if (sprite_set_info == null && command_data.Sprite_Set_Version != "")
            {
                await ErrorHandling.Sprite_Set_Not_Found_In_Template(sl_command, command_data.Character_Keyword, OfficialSetMethods.InputToTemplate(account, command_data.Sprite_Set_Version));
                return;
            }
            // If the sprite set did not return null, the command was successful!
            else if (sprite_set_info != null)
            {
                await OfficialSetMethods.Quick_Scene_Directory(sl_command, sprite_set_info, command_data);
                //await message.Channel.SendMessageAsync(":white_check_mark: **Parsing successful.** Full command has been parsed and sprite set has been found.");
            }
        }

        public static async Task System_Message_Parser(SocialLinkerCommand sl_command, string arg)
        {
            try
            {
                // Get the account information of the command's user.
                var account = UserInfoClasses.GetAccount(sl_command.User);

                // Create an OfficialSetData variable and set it to null.
                // We'll be assigning a proper object to it depending on how far we progress through the method.
                //OfficialSetData sprite_set_info = null;

                // Create a new MakerCommandData object.
                // Throughout the function, we'll be filling the parameters out according to what is parsed from the user's command.
                MakerCommandData command_data = new MakerCommandData()
                {
                    Template = "",
                    Character_Keyword = "",
                    Sprite_Set_Version = "",
                    Base_Sprite = default,
                    Eye_Frame = default,
                    Mouth_Frame = default,
                    Dialogue = ""
                };

                // Declare other variables that will be needed throughout the method.
                int iterator = 0;
                string current_string = "";

                // Create an empty string list. This is where the user's input will go.
                List<string> input_substring;

                // Create a char array that contains a whitespace.
                char[] delimiterChars = { ' ' };

                // Using the char array, split the user's input into indicies of the string list by seperating each one per whitespace.
                input_substring = arg.Split(delimiterChars).ToList();

                // Iterate through the list to ensure no index is a whitespace.
                for (int i = input_substring.Count - 1; i >= 0; i--)
                {
                    // If there is, remove the index.
                    if (input_substring[i] == "")
                    {
                        input_substring.RemoveAt(i);
                    }
                }

                // If there are no indicies in the input_substring string list, we have a successful command! Generate a tutorial menu and return.
                if (input_substring.Count == 0)
                {
                    await sl_command.Channel.SendMessageAsync(":white_check_mark: **Parsing successful.** A tutorial menu for the scene maker will be displayed.");
                    return;
                }

                // Create two individual string arrays for template keywords: One containing generic keywords, and one containing version keywords.
                string[] generic_keywords = { "p1", "p2", "p2is", "p2ep", "p3", "p4", "p4a", "p4au", "p4u", "p4u2", "p4d", "p5", "p5s", "bbtag" };
                string[] version_keywords = { "p1-ps1", "p1-psx", "p1-psp", "p1p", "p2is-ps1", "p2is-psx", "p2is-psp", "p2isp", "p2ep-ps1", "p2ep-psx", "p2ep-psp", "p2epp", "p3f", "fes", "p3fes", "p3-ps2", "p3f-ps2", "fes-ps2", "p3fes-ps2", "p3p", "p3-psp", "p4-ps2", "p4g", "p4a", "p4au", "p4u", "p4u2", "p4d", "p5-ps3", "p5-ps4", "p5r", "p5r-ps4", "p5s", "bbtag" };

                // Assign the first word of the user's input after the "maker" prefix to the empty "current_string" variable.
                // The int variable "iterator" is currently set at 0, so this will retrieve the first index of the string list containing the user's processed input.
                current_string += input_substring[iterator];

                // First, let's assume we're looking for a generic theme keyword.
                // Iterate through every index in the generic_keywords array to check if the current string is a match.
                for (int i = 0; i < generic_keywords.Length; i++)
                {
                    // If a match is found, the user specified a template first before the "System" keyword. Send an error message and return.
                    if (current_string.ToLower() == generic_keywords[i])
                    {
                        await ErrorHandling.Template_Specified_First_On_System_Message(sl_command);
                        return;
                    }
                }

                // Let's assume a generic keyword was not found during the last step. 
                // If the "template" string is empty, a generic keyword wasn't assigned to it. Let's try searching for a version keyword next.
                if (command_data.Template == "")
                {
                    // Iterate through every index in the version_keywords array to check if the current string is a match.
                    for (int i = 0; i < version_keywords.Length; i++)
                    {
                        // If a match is found, the user specified a template first before the "System" keyword. Send an error message and return.
                        if (current_string.ToLower() == version_keywords[i])
                        {
                            await ErrorHandling.Template_Specified_First_On_System_Message(sl_command);
                            return;
                        }
                    }
                }

                // Reset current_string variable so that it is empty to take the next string, which should be the character keyword.
                current_string = "";

                // Next, let's start searching for a character keyword.
                // Ensure that the current index of the input_substring list (which should be 0 at this point) is not null.
                if (input_substring[iterator] != null)
                {
                    // Iterate through the input_substring list. This is an iteration seperate from the "iterator" int variable.
                    for (int i = 0; i < input_substring.Count; i++)
                    {
                        // Confirm that the first character of the current index is not a digit.
                        // This is how we detect coming across the command's possible sprite number.
                        if (Char.IsDigit(input_substring[i], 0) == false)
                        {
                            // If it's not a digit, this is likely part of a character keyword.
                            // Add a space to the current_string variable if the iterator's value is greater than 0.
                            // This means that more than one string in the list matches this condition.
                            if (i > 0)
                            {
                                current_string += " ";
                            }

                            // Add the contents of the current index to the current_string variable.
                            current_string += input_substring[i];
                        }
                        // If the first character of the current index is a digit, we want to perform a few checks here first.

                        if (Char.IsDigit(input_substring[i], 0) == true)
                        {
                            // If we've reached the end of the user input OR the next index in the substring array contains a quotation mark,
                            // assign the "iterator" variable to the index stopped at and break the loop.
                            // We have likely encountered the start of the sprite number.
                            if (i == input_substring.Count - 1 || input_substring[i + 1].Contains("\""))
                            {
                                iterator = i;
                                break;
                            }
                            // If neither of those conditions have been reached, the number we've encountered is likely meant to be part of the character keyword.
                            else
                            {
                                // Add a space to the current_string variable if the iterator's value is greater than 0.
                                // This means that more than one string in the list matches this condition.
                                if (i > 0)
                                {
                                    current_string += " ";
                                }

                                // Add the contents of the current index to the current_string variable.
                                current_string += input_substring[i];
                            }
                        }
                    }
                }

                // Now, let's analyze the character keyword we have and see if there's a theme specifier at the end.
                // Create an empty string list.
                List<string> char_temp;

                // Split the current_string variable by any whitespaces and assign all parts to the newly created string list.
                char_temp = current_string.Split(delimiterChars).ToList();

                // Check if the char_temp list has more than one index.
                if (char_temp.Count > 1)
                {
                    // If so, start iterating through the generic_keywords string list. There may be a template keyword present at the end of the character keyword.
                    for (int i = 0; i < generic_keywords.Length; i++)
                    {
                        // Take the last index of the char_temp list and compare it against the current generic_keywords index iteration.
                        if (char_temp[char_temp.Count - 1].ToLower() == generic_keywords[i])
                        {
                            // If they match, a generic keyword specifying which game to pull the character's sprites from is present.
                            // Assign the last index of the char_temp list to the "character_sheet" string variable.
                            // Afterwards, remove the last index of char_temp from the list and break the loop. All that should remain is the character keyword.
                            command_data.Sprite_Set_Version = char_temp[char_temp.Count - 1];
                            char_temp.RemoveAt(char_temp.Count - 1);
                            break;
                        }
                    }

                    // Check if the Sprite_Set_Version variable is still empty. If so, this means the char_temp list did not contain a generic keyword.
                    if (command_data.Sprite_Set_Version == "")
                    {
                        // Next, let's start checking for version keywords.
                        // Start iterating through the version_keywords string list. There may be a template keyword present at the end of the character keyword.
                        for (int i = 0; i < version_keywords.Length; i++)
                        {
                            // Take the last index of the char_temp list and compare it against the current version_keywords index iteration.
                            if (char_temp[char_temp.Count - 1].ToLower() == version_keywords[i])
                            {
                                // If they match, a version keyword specifying which game to pull the character's sprites from is present.
                                // Assign the last index of the char_temp list to the "Sprite_Set_Version" string variable.
                                // Afterwards, remove the last index of char_temp from the list and break the loop. All that should remain is the character keyword.
                                command_data.Sprite_Set_Version = char_temp[char_temp.Count - 1];
                                char_temp.RemoveAt(char_temp.Count - 1);
                                break;
                            }
                        }
                    }
                }

                // Take the remaining entries in char_temp and place them in a single string declared earlier.
                command_data.Character_Keyword = string.Join(" ", char_temp.ToArray());

                // If there is a sprite number present, the iterator will be placed there now. If not, we should output an appropriate error message.
                // Confirm that the first character of the input_substring index we left off at is not a digit.
                // If a sprite number is present, this is expected to return true. If not, this is expected to return false.
                if (Char.IsDigit(input_substring[iterator], 0) == false)
                {
                    // Decide the case that the user accidentally forgot to enter a sprite number.
                    // Create a char array for different types of quotation marks.
                    char[] quotation_check = { '\u0022', '\u201C', '\u201D' };

                    // Iterate through the quotation_check char array.
                    for (int i = 0; i < quotation_check.Length; i++)
                    {
                        // Compare the entirety of the character_keyword string against the current iteration of the quotation_check char array to check if a match exists.
                        // If so, the user entered a quotation mark prematurely. Quotation marks only come after a sprite number, so output an error message and return.
                        if (command_data.Character_Keyword.Contains(quotation_check[i]))
                        {
                            await ErrorHandling.Sprite_Number_Missing(sl_command);
                            return;
                        }
                    }

                    // Here, we want to check for other possible parsing conditions and account for other errors or command types.
                    // Reminder: At this point, only the character keyword should be taken in.
                    // If the character keyword is not empty, send an error message. A character keyword without sprite number and dialogue is incorrect syntax.
                    if (command_data.Character_Keyword != "")
                    {
                        await ErrorHandling.Sprite_Number_And_Dialogue_Missing_On_System_Message(sl_command);
                    }
                    return;
                }

                // If we've made it this far, there's still more command to process!
                // Remove everything before the current iterator point in the input_substring list.
                for (int i = 0; i < iterator; i++)
                {
                    // Always remove the first index, since the index numbers of the elements will shift by one after a removal.
                    input_substring.RemoveAt(0);
                }

                // Reset iterator to 0.
                iterator = 0;

                // Next, let's parse what's supposed to be the sprite number.
                // Create a char array containing characters that could be found in the sprite specifier.
                char[] sprite_number_delimiters = { '-', '_', '.', ',' };

                // Create an empty string list. This is where the sprite specifier will be temporarily held and processed.
                List<string> sprite_number_temp;

                // Take the contents of the current iterated index of input_substring and split it by the characters specified in the char array just created.
                // This will be assigned to the newly created string list.
                sprite_number_temp = input_substring[iterator].Split(sprite_number_delimiters).ToList();

                // Animation frames cannot be used in system messages, so ensure there is only one number for the base sprite.
                // If there is more than one index in the sprite_number_temp string list, send an error message and return.
                if (sprite_number_temp.Count > 1)
                {
                    await ErrorHandling.Animation_Frames_Specified_On_System_Message(sl_command);
                    return;
                }

                // Iterate through the entries of the sprite_number_temp string list.
                // Here, we'll test if the value taken is an integer
                for (int i = 0; i < sprite_number_temp.Count; i++)
                {
                    // Create an int variable and initialize it to zero.
                    // We'll need this int to confirm the type of the values in sprite_number_temp we're about to process.
                    int integer_test = 0;

                    // If the contents of the index currently being iterated on can be successfully converted to an integer, do nothing.
                    if (int.TryParse(sprite_number_temp[i], out integer_test) == true)
                    {
                        // If it is a digit, do nothing.
                    }
                    // If not, send an error message and return. We only want integer values at this step.
                    else
                    {
                        await ErrorHandling.Non_Digit_In_Sprite_Number(sl_command);
                        return;
                    }
                }

                // Now that the sprite's validity as an integer is confirmed, assign it to the proper base sprite variable.
                command_data.Base_Sprite = Int32.Parse(sprite_number_temp[0]);

                // The command input can potentially end here, so let's first test the case that there's still more input remaining.
                if (iterator != input_substring.Count - 1)
                {
                    // If the user makes it this far, there should be a character keyword present since we've already taken in the sprite number.
                    // If the character keyword is empty, send an error message and return.
                    if (command_data.Character_Keyword == "")
                    {
                        await ErrorHandling.Sprite_Number_Before_Char_Keyword(sl_command);
                        return;
                    }

                    // Increase the iterator by one, placing it at the next index of the main input_substring string list.
                    iterator++;
                }
                // If the iterator is on the last index of the input_substring string list, there is no more input to take in.
                // In this case, send an error message. Dialogue should always be present when forming a system message.
                else
                {
                    await ErrorHandling.Missing_Dialogue_On_System_Message(sl_command);
                    return;
                }

                // Remove everything before the current iterator point in the input_substring list.
                // After this point, only the dialogue should remain.
                for (int i = 0; i < iterator; i++)
                {
                    // Always remove the first index, since the index numbers of the elements will shift by one after a removal.
                    input_substring.RemoveAt(0);
                }

                // Finally, let's search for the dialogue to be rendered.
                // Create an empty char list.
                List<char> dialogue_temp;

                // Convert the input_substring string list into a char array.
                char[] charArr = string.Join(" ", input_substring.ToArray()).ToCharArray();

                // Convert the char array to a char list and assign it to the empty char list variable recently created.
                dialogue_temp = charArr.ToList();

                // Ensure that the first index and the last index of the dialogue_temp char array contain quotation marks.
                // If they do, remove the first and last indices from the list.
                if ((dialogue_temp[0] == '\u0022' || dialogue_temp[0] == '\u201C') && (dialogue_temp[dialogue_temp.Count - 1] == '\u0022' || dialogue_temp[dialogue_temp.Count - 1] == '\u201D' || dialogue_temp[dialogue_temp.Count - 1] == '\u201E'))
                {
                    dialogue_temp.RemoveAt(0);
                    dialogue_temp.RemoveAt(dialogue_temp.Count - 1);
                }
                // If not, send an error message and return. Dialogue should always be placed between quotation marks.
                else
                {
                    await ErrorHandling.Text_After_Sprite_Number_Not_Quoted(sl_command);
                    return;
                }

                // Add each entry of the dialogue_temp char list to the dialogue string variable.
                foreach (char ch in dialogue_temp)
                {
                    command_data.Dialogue += ch;
                }

                // Check if the dialogue variable is empty or full of whitespace. It should be filled with something.
                // Start off whitespace_check at 2 to account for the mandatory quotation marks.
                int whitespace_check = 2;

                for (int i = 0; i < charArr.Length; i++)
                {
                    if (charArr[i] == ' ')
                    {
                        whitespace_check++;
                    }
                }

                // If the dialogue is filled with whitespace, replace it with ellipses.
                if (whitespace_check == charArr.Length)
                {
                    command_data.Dialogue = "......";
                }

                // With that, we've reached the end of the parser! Use the completed command data to render the appropriate template.
                await OfficialSetMethods.System_Message_Directory(sl_command, command_data);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static string String_List_To_String(List<string> input_list)
        {
            // Create an empty string variable.
            string output_string = "";

            // Iterate through each index of the list and add it to the string variable.
            for (int i = 0; i < input_list.Count; i++)
            {
                output_string += input_list[i] + " ";
            }

            // Return the string variable.
            return output_string;
        }
    }

    public class MakerCommandData
    {
        public string Template { get; set; }
        public string Character_Keyword { get; set; }
        public string Sprite_Set_Version { get; set; }
        public int Base_Sprite { get; set; }
        public int Eye_Frame { get; set; }
        public int Mouth_Frame { get; set; }
        public string Dialogue { get; set; }
    }
}
