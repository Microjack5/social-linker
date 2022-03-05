using System.Collections.Generic;
using System.Linq;
using System.IO;
using Newtonsoft.Json;
using SocialLinker.Core.CloudStorageTables;
using SocialLinker.Core.SceneMaker;
using Discord.WebSocket;
using System.Threading.Tasks;
using System.Drawing;
using System;
using SocialLinker.Config;
using SocialLinker.Core.SceneMaker.TemplateRenders.QuickScenes;
using SocialLinker.Core.SceneMaker.Data.Bustup;

namespace SocialLinker.Core.LocalStorageTables
{
    public class OfficialSetMethods
    {
        private static List<OfficialSetData> sprite_set_list = null;

        private const string configFolder = "Resources";
        private const string configFile = "Set_Data.json";

        static OfficialSetMethods()
        {
            // If the file folder doesn't exist, create it.
            if (!Directory.Exists(configFolder))
            {
                Directory.CreateDirectory(configFolder);
            }

            // If the file exists, load its contents.
            if (File.Exists(configFolder + "/" + configFile))
            {
                sprite_set_list = LoadSpriteSetList(configFolder + "/" + configFile).ToList();
            }
            // If the file does not exist, create an empty one.
            else
            {
                sprite_set_list = new List<OfficialSetData>();
                SaveSpriteSetList(sprite_set_list, configFolder + "/" + configFile);
            }
        }

        public static void SaveSpriteSetList(IEnumerable<OfficialSetData> decor_list, string filePath)
        {
            string json = JsonConvert.SerializeObject(decor_list, Formatting.Indented, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore
            });
            File.WriteAllText(filePath, json);
        }

        public static IEnumerable<OfficialSetData> LoadSpriteSetList(string filePath)
        {
            if (!File.Exists(filePath)) return null;
            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<OfficialSetData>>(json);
        }

        public static OfficialSetData GetSpriteSetInfo(UserInfoFields account, MakerCommandData command_data)
        {
            // To get the proper sprite set data, we have to take the user's account settings and parsed command into consideration.
            // First, let's analyze the official sprite set list.
            // Iterate through each entry of the official sprite set list.
            foreach (OfficialSetData s in sprite_set_list)
            {
                // Deserialize the Set_Keywords field of the current iterated object into a string array.
                string[] generic_char_keywords = Newtonsoft.Json.JsonConvert.DeserializeObject<string[]>(s.Keywords);

                // Iterate over each index of the string array.
                for (int i = 0; i < generic_char_keywords.Length; i++)
                {
                    // If the contents of the current index match the lowercase form of the user's input, we found a potential candidate!
                    if (generic_char_keywords[i] == command_data.Character_Keyword.ToLower())
                    {
                        // Check to see if the user specified a sprite set version in their command.
                        // First, let's process the case that they didn't.
                        // We'll want to return a sprite set from the character's debut title that matches the user's desired version.
                        if (command_data.Sprite_Set_Version == "")
                        {
                            // Check if the current set is from a title that has multiple versions to it.
                            // If so, the Version_Control_Check method will not return empty and instead return the user's version control settings for that title.
                            if (Version_Control_Check(account, s) != "")
                            {
                                // If the title does have multiple versions, check if the character itself the set contains appears in all versions of the title.
                                if (Appears_In_All_Versions_Check(s) == true)
                                {
                                    // Also check if the sprite set is from the character's debut title and the set's origin matches the user's version control settings.
                                    if (s.Character_Debut == "Yes" && s.Origin == Version_Control_Check(account, s))
                                    {
                                        // If we made it this far, all our checks are complete! Return the current set.
                                        return s;
                                    }
                                }
                                // If the character doesn't appear in multiple versions and the set is from their debut title...
                                if (Appears_In_All_Versions_Check(s) == false && s.Character_Debut == "Yes")
                                {
                                    // All our checks are complete! Return the current set.
                                    return s;
                                }
                            }
                            // If not, check if the sprite set is from the character's debut title.
                            else if (s.Character_Debut == "Yes")
                            {
                                // If we made it this far, all our checks are complete! Return the current set.
                                return s;
                            }
                        }
                        // If the user did specify a sprite set version in their command, let's make sure we get the right set!
                        else if (command_data.Sprite_Set_Version != "")
                        {
                            // First, convert the user's input title into one we can use.
                            string input_template = InputToTemplate(account, command_data.Sprite_Set_Version);

                            // Check if the set's origin matches the user's input template.
                            if (s.Origin == input_template)
                            {
                                // If we made it this far, all our checks are complete! Return the current set.
                                return s;
                            }
                        }
                    }
                }
            }

            return null;
        }

        public static string Version_Control_Check(UserInfoFields account, OfficialSetData input_set)
        {
            // This method is for taking in a set's data and returning the user's associated version control setting for that title.
            // If a title that doesn't have multiple versions is taken in as input, the output is an empty string.

            // Deserialize the Title_Appearances field of the current iterated object into a string array.
            string[] title_appearances = Newtonsoft.Json.JsonConvert.DeserializeObject<string[]>(input_set.Title_Appearances);

            if ((input_set.Origin == "P1-PS1" || input_set.Origin == "P1-PSP") && title_appearances.Contains("P1-PS1") && title_appearances.Contains("P1-PSP"))
            {
                return account.VC_P1;
            }
            else if ((input_set.Origin == "P2IS-PS1" || input_set.Origin == "P2IS-PSP") && title_appearances.Contains("P2IS-PS1") && title_appearances.Contains("P2IS-PSP"))
            {
                return account.VC_P2IS;
            }
            else if ((input_set.Origin == "P2EP-PS1" || input_set.Origin == "P2EP-PSP") && title_appearances.Contains("P2EP-PS1") && title_appearances.Contains("P2EP-PSP"))
            {
                return account.VC_P2EP;
            }
            else if ((input_set.Origin == "P3F" || input_set.Origin == "P3P") && title_appearances.Contains("P3F") && title_appearances.Contains("P3P"))
            {
                return account.VC_P3;
            }
            else if ((input_set.Origin == "P4-PS2" || input_set.Origin == "P4G") && title_appearances.Contains("P4-PS2") && title_appearances.Contains("P4G"))
            {
                return account.VC_P4;
            }
            else if ((input_set.Origin == "P5-PS4" || input_set.Origin == "P5R") && title_appearances.Contains("P5-PS4") && title_appearances.Contains("P5R"))
            {
                return account.VC_P5;
            }

            return "";
        }

        public static bool Appears_In_All_Versions_Check(OfficialSetData set_data)
        {
            // Deserialize the Title_Appearances field of the set data object into a string array.
            string[] appearances = Newtonsoft.Json.JsonConvert.DeserializeObject<string[]>(set_data.Title_Appearances);

            // Check if the set originates from either the PS1 or PSP versions of Persona.
            if (set_data.Origin == "P1-PS1" || set_data.Origin == "P1-PSP")
            {
                // If so, check to see if the character in the set appears in both versions of the title.
                if (appearances.Contains("P1-PS1") && appearances.Contains("P1-PSP"))
                {
                    // If both statements are true, return true.
                    return true;
                }
            }

            // Check if the set originates from either the PS1 or PSP versions of Persona 2: Innocent Sin.
            else if (set_data.Origin == "P2IS-PS1" || set_data.Origin == "P2IS-PSP")
            {
                // If so, check to see if the character in the set appears in both versions of the title.
                if (appearances.Contains("P2IS-PS1") && appearances.Contains("P2IS-PSP"))
                {
                    // If both statements are true, return true.
                    return true;
                }
            }

            // Check if the set originates from either the PS1 or PSP versions of Persona 2: Eternal Punishment.
            else if (set_data.Origin == "P2EP-PS1" || set_data.Origin == "P2EP-PSP")
            {
                // If so, check to see if the character in the set appears in both versions of the title.
                if (appearances.Contains("P2EP-PS1") && appearances.Contains("P2EP-PSP"))
                {
                    // If both statements are true, return true.
                    return true;
                }
            }

            // Check if the set originates from either the PS2 or PSP versions of Persona 3.
            else if (set_data.Origin == "P3F" || set_data.Origin == "P3P")
            {
                // If so, check to see if the character in the set appears in both versions of the title.
                if (appearances.Contains("P3F") && appearances.Contains("P3P"))
                {
                    // If both statements are true, return true.
                    return true;
                }
            }

            // Check if the set originates from either the original or Golden versions of Persona 4.
            else if (set_data.Origin == "P4-PS2" || set_data.Origin == "P4G")
            {
                // If so, check to see if the character in the set appears in both versions of the title.
                if (appearances.Contains("P4-PS2") && appearances.Contains("P4G"))
                {
                    // If both statements are true, return true.
                    return true;
                }
            }

            // Check if the set originates from either the original or Royal versions of Persona 5.
            else if (set_data.Origin == "P5-PS4" || set_data.Origin == "P5R")
            {
                // If so, check to see if the character in the set appears in both versions of the title.
                if (appearances.Contains("P5-PS4") && appearances.Contains("P5R"))
                {
                    // If both statements are true, return true.
                    return true;
                }
            }

            // If none of the statements above successfully return true, return false.
            return false;
        }

        public static string InputToTemplate(UserInfoFields account, string input_template)
        {
            // This method converts the user's input template keyword into a usable abbreviation based on their version control settings and the input itself.
            // First, convert the user's input to entirely uppercase.
            input_template = input_template.ToUpper();

            // Use if statements to handle version-specific keywords.
            if (input_template == "P1-PS1" || input_template == "P1-PSX")
            {
                return "P1-PS1";
            }
            else if (input_template == "P1-PSP" || input_template == "P1P")
            {
                return "P1-PSP";
            }
            else if (input_template == "P2IS-PS1" || input_template == "P2IS-PSX")
            {
                return "P2IS-PS1";
            }
            else if (input_template == "P2IS-PSP" || input_template == "P2ISP")
            {
                return "P2IS-PSP";
            }
            else if (input_template == "P2EP-PS1" || input_template == "P2EP-PSX")
            {
                return "P2EP-PS1";
            }
            else if (input_template == "P2EP-PSP" || input_template == "P2EPP")
            {
                return "P2EP-PSP";
            }
            else if (input_template == "P3F" || input_template == "FES" || input_template == "P3FES" || input_template == "P3-PS2" || input_template == "P3F-PS2" || input_template == "FES-PS2" || input_template == "P3FES-PS2")
            {
                return "P3F";
            }
            else if (input_template == "P3P" || input_template == "P3-PSP")
            {
                return "P3P";
            }
            else if (input_template == "P4-PS2")
            {
                return "P4-PS2";
            }
            else if (input_template == "P4G")
            {
                return "P4G";
            }
            else if (input_template == "P5-PS3" || input_template == "P5-PS4")
            {
                return "P5-PS4";
            }
            else if (input_template == "P5R" || input_template == "P5R-PS4")
            {
                return "P5R";
            }

            // Generic template keywords are handled by the user's version control settings.
            // Since there are multiple inputs that can lead to the desired template, if statements are used to decide the proper abbreviation to return.
            if (input_template == "P1")
            {
                return account.VC_P1;
            }
            else if (input_template == "P2" || input_template == "P2IS")
            {
                return account.VC_P2IS;
            }
            else if (input_template == "P2EP")
            {
                return account.VC_P2EP;
            }
            else if (input_template == "P3")
            {
                return account.VC_P3;
            }
            else if (input_template == "P4")
            {
                return account.VC_P4;
            }
            else if (input_template == "P4A" || input_template == "P4AU" || input_template == "P4U" || input_template == "P4U2")
            {
                return "P4AU";
            }
            else if (input_template == "P4D")
            {
                return "P4D";
            }
            else if (input_template == "P5")
            {
                return account.VC_P5;
            }
            else if (input_template == "P5S")
            {
                return "P5S";
            }
            else if (input_template == "BBTAG")
            {
                return "BBTAG";
            }

            return "";
        }

        public static string AcronymToFullTitle(string acronym)
        {
            // This method takes in a string abbreviation for a scene maker template and converts it to the abbreviation's full proper title.
            switch (acronym)
            {
                case "P1-PS1":
                    return "Revelations: Persona";

                case "P1-PSP":
                    return "Persona (Remake)";

                case "P2IS-PS1":
                    return "Persona 2: Innocent Sin (PlayStation®️)";

                case "P2IS-PSP":
                    return "Persona 2: Innocent Sin (Remake)";

                case "P2EP-PS1":
                    return "Persona 2: Eternal Punishment (PlayStation®️)";

                case "P2EP-PSP":
                    return "Persona 2: Eternal Punishment (Remake)";

                case "P3F":
                    return "Persona 3 FES";

                case "P3P":
                    return "Persona 3 Portable";

                case "P4-PS2":
                    return "Persona 4 (PlayStation®️ 2)";

                case "P4G":
                    return "Persona 4 Golden";

                case "P4AU":
                    return "Persona 4 Arena Ultimax";

                case "P4D":
                    return "Persona 4: Dancing All Night";

                case "P5-PS4":
                    return "Persona 5 (PlayStation®️ 4)";

                case "P5R":
                    return "Persona 5 Royal";

                case "P5S":
                    return "Persona 5 Strikers";

                case "BBTAG":
                    return "BlazBlue: Cross Tag Battle";
            }

            return "";
        }

        // Main methods to assist the functions of the scene maker.
        public static async Task Set_List_Message_Directory(SocketMessage message, string title)
        {
            switch (title)
            {
                case "P1-PS1":
                    await OfficialSetLists.P1_PS1_Set_List(message);
                    return;

                case "P1-PSP":
                    await OfficialSetLists.P1_PSP_Set_List(message);
                    return;

                case "P2IS-PS1":
                    await OfficialSetLists.P2IS_PS1_Set_List(message);
                    return;

                case "P2IS-PSP":
                    await OfficialSetLists.P2IS_PSP_Set_List(message);
                    return;

                case "P2EP-PS1":
                    await OfficialSetLists.P2EP_PS1_Set_List(message);
                    return;

                case "P2EP-PSP":
                    await OfficialSetLists.P2EP_PSP_Set_List(message);
                    return;

                case "P3F":
                    await OfficialSetLists.P3F_Set_List(message);
                    return;

                case "P3P":
                    await OfficialSetLists.P3P_Set_List(message);
                    return;

                case "P4-PS2":
                    await OfficialSetLists.P4_PS2_Set_List(message);
                    return;

                case "P4G":
                    await OfficialSetLists.P4G_Set_List(message);
                    return;

                case "P4AU":
                    await OfficialSetLists.P4AU_Set_List(message);
                    return;

                case "P4D":
                    await OfficialSetLists.P4D_Set_List(message);
                    return;

                case "P5-PS4":
                    await OfficialSetLists.P5_PS4_Set_List(message);
                    return;

                case "P5R":
                    await OfficialSetLists.P5R_Set_List(message);
                    return;

                case "P5S":
                    await OfficialSetLists.P5S_Set_List(message);
                    return;

                case "BBTAG":
                    await OfficialSetLists.BBTAG_Set_List(message);
                    return;
            }
            return;
        }

        public static async Task Sprite_Sheet_Message_Directory(SocketMessage message, OfficialSetData sprite_set_info)
        {
            switch (sprite_set_info.Origin)
            {
                case "P1-PS1":
                    await OfficialSetSheets.P1_PS1_Sprite_Sheet(message, sprite_set_info);
                    return;

                case "P1-PSP":
                    return;

                case "P2IS-PS1":
                    await OfficialSetSheets.P2IS_PS1_Sprite_Sheet(message, sprite_set_info);
                    return;

                case "P2IS-PSP":
                    return;

                case "P2EP-PS1":
                    await OfficialSetSheets.P2EP_PS1_Sprite_Sheet(message, sprite_set_info);
                    return;

                case "P2EP-PSP":
                    return;

                case "P3F":
                    await OfficialSetSheets.P3F_Sprite_Sheet(message, sprite_set_info);
                    return;

                case "P3P":
                    await OfficialSetSheets.P3P_Sprite_Sheet(message, sprite_set_info);
                    return;

                case "P4-PS2":
                    await OfficialSetSheets.P4_PS2_Sprite_Sheet(message, sprite_set_info);
                    return;

                case "P4G":
                    await OfficialSetSheets.P4G_Sprite_Sheet(message, sprite_set_info);
                    return;

                case "P4AU":
                    return;

                case "P4D":
                    await OfficialSetSheets.P4D_Sprite_Sheet(message, sprite_set_info);
                    return;

                case "P5-PS4":
                    await OfficialSetSheets.P5_PS4_Sprite_Sheet(message, sprite_set_info);
                    return;

                case "P5R":
                    await OfficialSetSheets.P5R_Sprite_Sheet(message, sprite_set_info);
                    return;

                case "P5S":
                    await OfficialSetSheets.P5S_Sprite_Sheet(message, sprite_set_info);
                    return;

                case "BBTAG":
                    await OfficialSetSheets.BBTAG_Sprite_Sheet(message, sprite_set_info);
                    return;
            }
            return;
        }

        public static async void Base_Sprite_Validity_Check(SocketMessage message, OfficialSetData set_data, MakerCommandData command_data)
        {
            // Establish the directory of the specified sprite set.
            string set_path = $@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//{set_data.Origin}//Bustup//{set_data.ID}";

            // Get a count of how many files are in the sprite set's directory.
            int filecount = AttachmentCountItemDirectory(set_path);

            // Now that we have a filecount for the set, let's see if the inputted sprite number is valid before we continue.
            // If not, send an error message and cancel the request.
            if (command_data.Base_Sprite > filecount)
            {
                _ = ErrorHandling.Sprite_Number_Not_Found(message, set_data.Name, set_data.Origin);
            }
            // If so, continue with creating the frame sheet!
            else
            {
                await Bustup_Frame_Sheet_Message_Directory(message, set_data, command_data);
            }

            return;
        }

        public static async Task Bustup_Frame_Sheet_Message_Directory(SocketMessage message, OfficialSetData set_data, MakerCommandData command_data)
        {
            switch (set_data.Origin)
            {
                case "P1-PS1":
                    await BustupFrameSheets.P1_PS1_Bustup_Frame_Sheet(message, set_data, command_data);
                    return;

                case "P1-PSP":
                    return;

                case "P2IS-PS1":
                    await BustupFrameSheets.P2IS_PS1_Bustup_Frame_Sheet(message, set_data, command_data);
                    return;

                case "P2IS-PSP":
                    return;

                case "P2EP-PS1":
                    return;

                case "P2EP-PSP":
                    return;

                case "P3F":
                    await BustupFrameSheets.P3F_Bustup_Frame_Sheet(message, set_data, command_data);
                    return;

                case "P3P":
                    await BustupFrameSheets.P3P_Bustup_Frame_Sheet(message, set_data, command_data);
                    return;

                case "P4-PS2":
                    await BustupFrameSheets.P4_PS2_Bustup_Frame_Sheet(message, set_data, command_data);
                    return;

                case "P4G":
                    await BustupFrameSheets.P4G_Bustup_Frame_Sheet(message, set_data, command_data);
                    return;

                case "P4AU":
                    return;

                case "P4D":
                    await BustupFrameSheets.P4D_Bustup_Frame_Sheet(message, set_data, command_data);
                    return;

                case "P5-PS4":
                    await BustupFrameSheets.P5_PS4_Bustup_Frame_Sheet(message, set_data, command_data);
                    return;

                case "P5R":
                    await BustupFrameSheets.P5R_Bustup_Frame_Sheet(message, set_data, command_data);
                    return;

                case "P5S":
                    await BustupFrameSheets.P5S_Bustup_Frame_Sheet(message, set_data, command_data);
                    return;

                case "BBTAG":
                    await BustupFrameSheets.BBTAG_Bustup_Frame_Sheet(message, set_data, command_data);
                    return;
            }
            return;
        }

        public static string Generate_Normal_Set_List(string title)
        {
            // Create an empty string list.
            List<string> specified_set_list = new List<string>();

            // Iterate through each entry of the official sprite set list.
            foreach (OfficialSetData s in sprite_set_list)
            {
                // If the current iterated sprite set comes from the title specified by the user, add it to the string list.
                if (s.Origin == title)
                {
                    specified_set_list.Add(s.Name);
                }
            }

            // Order the newly formed string list by alphabetical order.
            specified_set_list = specified_set_list.OrderBy(s => s).ToList();

            // Create an empty string variable.
            string sorted_string = "";

            // Iterate through each entry of the string list and add it to the newly created string variable in a format the user can read.
            foreach (string s in specified_set_list)
            {
                sorted_string += $"- {s}\n";
            }

            // Return the string variable.
            return sorted_string;
        }

        public static string Generate_P1_PS1_Set_List()
        {
            // Create an empty string list.
            List<string> specified_set_list = new List<string>();

            // Iterate through each entry of the official sprite set list.
            foreach (OfficialSetData s in sprite_set_list)
            {
                // If the current iterated sprite set comes from the title specified by the user, add it to the string list.
                if (s.Origin == "P1-PS1" && (s.ID != "B29" && s.ID != "B30" && s.ID != "B31" && s.ID != "B32" && s.ID != "B33"))
                {
                    specified_set_list.Add(s.Name);
                }
            }

            // Order the newly formed string list by alphabetical order.
            specified_set_list = specified_set_list.OrderBy(s => s).ToList();

            // Create an empty string variable.
            string sorted_string = "";

            // Iterate through each entry of the string list and add it to the newly created string variable in a format the user can read.
            foreach (string s in specified_set_list)
            {
                sorted_string += $"- {s}\n";
            }

            // For this list, we want to seperate the listed characters by which franchise they belong to.
            sorted_string += "\n";
            sorted_string += "**__Snow Queen Quest__**\n";

            // Iterate through each entry of the official sprite set list.
            foreach (OfficialSetData s in sprite_set_list)
            {
                // If the current iterated sprite set comes from BBTAG and is a BlazBlue character, add it to the string list.
                if (s.Origin == "P1-PS1" && (s.ID == "B29" || s.ID == "B30" || s.ID == "B31" || s.ID == "B32" || s.ID == "B33"))
                {
                    sorted_string += $"- {s.Name}\n";
                }
            }

            // Return the string variable.
            return sorted_string;
        }

        public static string Generate_BBTAG_Set_List()
        {
            // Create an empty string variable.
            string output_string = "";

            // For this list, we want to seperate the listed characters by which franchise they belong to.
            // First, let's start with BlazBlue.
            output_string += "<:BlazBlue:657430195967492098> **__BlazBlue__**\n";

            // Iterate through each entry of the official sprite set list.
            foreach (OfficialSetData s in sprite_set_list)
            {
                // If the current iterated sprite set comes from BBTAG and is a BlazBlue character, add it to the string list.
                if (s.Origin == "BBTAG" && s.Series == "BlazBlue")
                {
                    output_string += $"- {s.Name}\n";
                }
            }

            // Next, Persona 4 Arena characters.
            output_string += "\n";
            output_string += "<:Persona4Arena:657430197699739669> **__Persona 4 Arena__**\n";


            // Iterate through each entry of the official sprite set list.
            foreach (OfficialSetData s in sprite_set_list)
            {
                // If the current iterated sprite set comes from BBTAG and is a Persona 4 Arena character, add it to the string list.
                if (s.Origin == "BBTAG" && s.Series == "Persona 4 Arena")
                {
                    output_string += $"- {s.Name}\n";
                }
            }

            // Next, Under Night In-Birth characters.
            output_string += "\n";
            output_string += "<:UnderNightInBirth:657430196755890217> **__Under Night In-Birth__**\n";

            // Iterate through each entry of the official sprite set list.
            foreach (OfficialSetData s in sprite_set_list)
            {
                // If the current iterated sprite set comes from BBTAG and is a Under Night In-Birth character, add it to the string list.
                if (s.Origin == "BBTAG" && s.Series == "Under Night In-Birth")
                {
                    output_string += $"- {s.Name}\n";
                }
            }

            // Next, RWBY characters.
            output_string += "\n";
            output_string += "<:RWBY:657428740594204692> **__RWBY__**\n";

            // Iterate through each entry of the official sprite set list.
            foreach (OfficialSetData s in sprite_set_list)
            {
                // If the current iterated sprite set comes from BBTAG and is a RWBY character, add it to the string list.
                if (s.Origin == "BBTAG" && s.Series == "RWBY")
                {
                    output_string += $"- {s.Name}\n";
                }
            }

            // Next, Arcana Heart characters.
            output_string += "\n";
            output_string += "<:ArcanaHeart:657428744222539787> **__Arcana Heart__**\n";

            // Iterate through each entry of the official sprite set list.
            foreach (OfficialSetData s in sprite_set_list)
            {
                // If the current iterated sprite set comes from BBTAG and is an Arcana Heart character, add it to the string list.
                if (s.Origin == "BBTAG" && s.Series == "Arcana Heart")
                {
                    output_string += $"- {s.Name}\n";
                }
            }

            // Next, Senran Kagura characters.
            output_string += "\n";
            output_string += "<:SenranKagura:657428641281474570> **__Senran Kagura__**\n";

            // Iterate through each entry of the official sprite set list.
            foreach (OfficialSetData s in sprite_set_list)
            {
                // If the current iterated sprite set comes from BBTAG and is a Senran Kagura character, add it to the string list.
                if (s.Origin == "BBTAG" && s.Series == "Senran Kagura")
                {
                    output_string += $"- {s.Name}\n";
                }
            }

            // Next, Akatsuki En-Eins characters.
            output_string += "\n";
            output_string += "<:AkatsukiEnEins:657428743845052427> **__Akatsuki En-Eins__**\n";

            // Iterate through each entry of the official sprite set list.
            foreach (OfficialSetData s in sprite_set_list)
            {
                // If the current iterated sprite set comes from BBTAG and is an Akatsuki En-Eins character, add it to the string list.
                if (s.Origin == "BBTAG" && s.Series == "Akatsuki En-Eins")
                {
                    output_string += $"- {s.Name}\n";
                }
            }

            // Return the string variable.
            return output_string;
        }

        public static async Task Quick_Scene_Directory(SocketMessage message, OfficialSetData set_data, MakerCommandData command_data)
        {
            if (command_data.Template == "")
            {
                switch (set_data.Origin)
                {
                    case "P1-PS1":
                        await RenderP1_PS1.Render_Quick_Scene_P1_PS1(message, set_data, command_data);
                        return;

                    case "P1-PSP":
                        return;

                    case "P2IS-PS1":
                        await RenderP2IS_PS1.Render_Quick_Scene_P2IS_PS1(message, set_data, command_data);
                        return;

                    case "P2IS-PSP":
                        return;

                    case "P2EP-PS1":
                        await RenderP2EP_PS1.Render_Quick_Scene_P2EP_PS1(message, set_data, command_data);
                        return;

                    case "P2EP-PSP":
                        return;

                    case "P3F":
                        await RenderP3F.Render_Quick_Scene_P3F(message, set_data, command_data);
                        return;

                    case "P3P":
                        await RenderP3P.Render_Quick_Scene_P3P(message, set_data, command_data);
                        return;

                    case "P4-PS2":
                        await RenderP4_PS2.Render_Quick_Scene_P4_PS2(message, set_data, command_data);
                        return;

                    case "P4G":
                        await RenderP4G.Render_Quick_Scene_P4G(message, set_data, command_data);
                        return;

                    case "P4AU":
                        return;

                    case "P4D":
                        return;

                    case "P5-PS4":
                        return;

                    case "P5R":
                        await RenderP5R.Render_Quick_Scene_P5R(message, set_data, command_data);
                        return;

                    case "P5S":
                        await RenderP5S.Render_Quick_Scene_P5S(message, set_data, command_data);
                        return;

                    case "BBTAG":
                        return;
                }
            }
            
            return;
        }

        public static async Task System_Message_Directory(SocketMessage message, MakerCommandData command_data)
        {
            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(message.Author);

            // Convert the user's template input into one usable depending on their version control settings.
            string template = InputToTemplate(account, command_data.Sprite_Set_Version);

            switch (template)
            {
                case "P1-PS1":
                    return;

                case "P1-PSP":
                    return;

                case "P2IS-PS1":
                    return;

                case "P2IS-PSP":
                    return;

                case "P2EP-PS1":
                    return;

                case "P2EP-PSP":
                    return;

                case "P3F":
                    await RenderP3F.Render_System_Message(message, command_data);
                    return;

                case "P3P":
                    return;

                case "P4-PS2":
                    await RenderP4_PS2.Render_System_Message(message, command_data);
                    return;

                case "P4G":
                    await RenderP4G.Render_System_Message(message, command_data);
                    return;

                case "P4AU":
                    return;

                case "P4D":
                    return;

                case "P5-PS4":
                    return;

                case "P5R":
                    return;

                case "P5S":
                    return;

                case "BBTAG":
                    return;
            }

            return;
        }

        // Bustup construction
        public static Bitmap Bustup_Selection(SocketMessage message, UserInfoFields account, OfficialSetData set_data, BustupData bustup_data, MakerCommandData command_data)
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
                int base_sprite_number = command_data.Base_Sprite;

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
            if (command_data.Eye_Frame == default && command_data.Mouth_Frame == default)
            {
                Bitmap base_sprite = (Bitmap)System.Drawing.Image.FromFile($@"{set_path}//{base_sprite_filename}.png");
                return base_sprite;
            }
            else
            {
                Bitmap base_sprite = (Bitmap)System.Drawing.Image.FromFile($@"{set_path}//{base_sprite_filename}.png");
                Bitmap bustup_with_frames = Construct_Bustup_With_Frames(message, set_data, bustup_data, command_data, base_sprite);
                return bustup_with_frames;
            }
        }

        public static Bitmap Construct_Bustup_With_Frames(SocketMessage message, OfficialSetData set_data, BustupData bustup_data, MakerCommandData command_data, Bitmap bustup)
        {
            // Create a copy of the bitmap taken in.
            // This is the version we'll be editing and returning.
            Bitmap edited_bustup = bustup;

            if (command_data.Eye_Frame != default && command_data.Eye_Frame != 0)
            {
                // Establish the eye frame directory for the current sprite set.
                string eye_frame_path = $@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//{set_data.Origin}//Bustup//{set_data.ID}//Eyes";

                // Get the eye frame data of the frame specified in the user's command.
                FrameData eye_frame_data = BustupDataMethods.Get_Eye_Frame_Data(set_data, bustup_data, command_data);

                // Ensure that the returned eye frame data is not null.
                if (eye_frame_data != null)
                {
                    // Check that the eye frame path exists.
                    if (File.Exists($"{eye_frame_path}//{eye_frame_data.Filename}"))
                    {
                        // Save the eye frame to a bitmap variable.
                        Bitmap eye_frame_sprite = (Bitmap)System.Drawing.Image.FromFile($@"{eye_frame_path}//{eye_frame_data.Filename}");

                        // Depending on the bustup's game origin, a section of the base bustup may need to be cropped out to make the eye frame properly fit in.
                        if (set_data.Origin == "P5-PS4" || set_data.Origin == "P5R" || set_data.Origin == "P5S")
                        {
                            Rectangle crop_region_eyes = new Rectangle(eye_frame_data.Coord_X, eye_frame_data.Coord_Y, eye_frame_data.Scale_Width, eye_frame_data.Scale_Height);
                            edited_bustup = Crop_Rectangle_From_Bitmap(edited_bustup, crop_region_eyes);
                        }

                        // Draw the eye frame to the base bustup.
                        using (Graphics graphics = Graphics.FromImage(edited_bustup))
                        {
                            graphics.DrawImage(eye_frame_sprite, eye_frame_data.Coord_X, eye_frame_data.Coord_Y, eye_frame_data.Scale_Width, eye_frame_data.Scale_Height);
                        }
                    }
                }
                // If the frame data is null, send an error message and return null as well.
                else
                {
                    _ = ErrorHandling.Eye_Frame_Not_Found(message, command_data, set_data.Name, set_data.Origin);
                    return null;
                }
            }

            // Check if the user's command specifies a mouth frame as well.
            // If so, let's work on the mouth frame.
            if (command_data.Mouth_Frame != default && command_data.Mouth_Frame != 0)
            {
                // Establish the mouth frame directory for the current sprite set.
                string mouth_frame_path = $@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//{set_data.Origin}//Bustup//{set_data.ID}//Mouth";

                // Get the mouth frame data of the frame specified in the user's command.
                FrameData mouth_frame_data = BustupDataMethods.Get_Mouth_Frame_Data(set_data, bustup_data, command_data);

                // Ensure that the returned mouth frame data is not null.
                if (mouth_frame_data != null)
                {
                    // Check that the mouth frame path exists.
                    if (File.Exists($"{mouth_frame_path}//{mouth_frame_data.Filename}"))
                    {
                        // Save the mouth frame to a bitmap variable.
                        Bitmap mouth_frame_sprite = (Bitmap)System.Drawing.Image.FromFile($@"{mouth_frame_path}//{mouth_frame_data.Filename}");

                        // Depending on the bustup's game origin, a section of the base bustup may need to be cropped out to make the eye frame properly fit in.
                        if (set_data.Origin == "P5-PS4" || set_data.Origin == "P5R" || set_data.Origin == "P5S")
                        {
                            Rectangle crop_region_mouth = new Rectangle(mouth_frame_data.Coord_X, mouth_frame_data.Coord_Y, mouth_frame_data.Scale_Width, mouth_frame_data.Scale_Height);
                            edited_bustup = Crop_Rectangle_From_Bitmap(edited_bustup, crop_region_mouth);
                        }

                        // Draw the mouth frame to the base bustup.
                        using (Graphics graphics = Graphics.FromImage(edited_bustup))
                        {
                            graphics.DrawImage(mouth_frame_sprite, mouth_frame_data.Coord_X, mouth_frame_data.Coord_Y, mouth_frame_data.Scale_Width, mouth_frame_data.Scale_Height);
                        }
                    }
                }
                // If the frame data is null, send an error message and return null as well.
                else
                {
                    _ = ErrorHandling.Mouth_Frame_Not_Found(message, command_data, set_data.Name, set_data.Origin);
                    return null;
                }
            }

            // Finally, return the final edited bitmap.
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

        // Method from https://stackoverflow.com/questions/47695942/wrong-number-of-file-count-being-returned-from-directory
        // Neccesary due to Windows giving the incorrect file count at times with Directory.GetFiles(path).Length
        public static int AttachmentCountItemDirectory(string directoryPath)
        {
            string[] attExt = { ".png" };
            return Directory.EnumerateFiles(directoryPath)
              .Count(f => attExt.Contains(Path.GetExtension(f), StringComparer.InvariantCultureIgnoreCase));
        }
    }
}
