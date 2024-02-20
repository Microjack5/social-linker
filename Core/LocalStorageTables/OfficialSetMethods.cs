using System.Collections.Generic;
using System.Linq;
using System.IO;
using Newtonsoft.Json;
using SocialLinker.Core.CloudStorageTables;
using SocialLinker.Core.SceneMaker;
using System.Threading.Tasks;
using System.Drawing;
using System;
using SocialLinker.Config;
using SocialLinker.Core.SceneMaker.TemplateRenders.QuickScenes;
using SocialLinker.Core.SceneMaker.Data.Bustup;
using System.Drawing.Drawing2D;
using SocialLinker.Core.SceneMaker.GlyphParsing;
using SocialLinker.Core.SceneMaker.Data.Calendar;

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

        public static OfficialSetData GetSpriteSetInfo(UserInfoFields account, MakerCommandData maker_command_data)
        {
            OfficialSetData last_matching_set = null;

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
                    if (generic_char_keywords[i] == maker_command_data.Character_Data_1.Character_Keyword.ToLower())
                    {
                        // Check to see if the user specified a sprite set version in their command.
                        // First, let's process the case that they didn't.
                        // We'll want to return a sprite set from the character's debut title that matches the user's desired version.
                        if (maker_command_data.Character_Data_1.Sprite_Set_Version == "")
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
                                        // If we made it this far, all our checks are complete! Return the current set if it passes the user's content filter.
                                        if (Passes_Content_Filter(account, s.Origin))
                                        {
                                            return s;
                                        }
                                        // If not, mark it as the last matched set.
                                        else
                                        {
                                            last_matching_set = s;
                                        }
                                    }
                                }
                                // If the character doesn't appear in multiple versions and the set is from their debut title...
                                if (Appears_In_All_Versions_Check(s) == false && s.Character_Debut == "Yes")
                                {
                                    // All our checks are complete! Return the current set if it passes the user's content filter.
                                    if (Passes_Content_Filter(account, s.Origin))
                                    {
                                        return s;
                                    }
                                    // If not, mark it as the last matched set.
                                    else
                                    {
                                        last_matching_set = s;
                                    }
                                }
                            }
                            // If not, check if the sprite set is from the character's debut title.
                            else if (s.Character_Debut == "Yes")
                            {
                                // If we made it this far, all our checks are complete! Return the current set if it passes the user's content filter.
                                if (Passes_Content_Filter(account, s.Origin))
                                {
                                    return s;
                                }
                                // If not, mark it as the last matched set.
                                else
                                {
                                    last_matching_set = s;
                                }
                            }
                        }
                        // If the user did specify a sprite set version in their command, let's make sure we get the right set!
                        else if (maker_command_data.Character_Data_1.Sprite_Set_Version != "")
                        {
                            // First, convert the user's input title into one we can use.
                            string input_template = InputToTemplate(account, maker_command_data.Character_Data_1.Sprite_Set_Version);

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

            // If the last matched set is not null, return it.
            // This set is likely blocked by the user.
            if (last_matching_set != null)
            {
                return last_matching_set;
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
            if (Global.p1_ps1_version_keywords.Contains(input_template))
            {
                return "P1-PS1";
            }
            else if (Global.p1_psp_version_keywords.Contains(input_template))
            {
                return "P1-PSP";
            }
            else if (Global.p2is_ps1_version_keywords.Contains(input_template))
            {
                return "P2IS-PS1";
            }
            else if (Global.p2is_psp_version_keywords.Contains(input_template))
            {
                return "P2IS-PSP";
            }
            else if (Global.p2ep_ps1_version_keywords.Contains(input_template))
            {
                return "P2EP-PS1";
            }
            else if (Global.p2ep_psp_version_keywords.Contains(input_template))
            {
                return "P2EP-PSP";
            }
            else if (Global.p3f_version_keywords.Contains(input_template))
            {
                return "P3F";
            }
            else if (Global.p3p_version_keywords.Contains(input_template))
            {
                return "P3P";
            }
            else if (Global.p3r_version_keywords.Contains(input_template))
            {
                return "P3R";
            }
            else if (Global.p4_ps2_version_keywords.Contains(input_template))
            {
                return "P4-PS2";
            }
            else if (Global.p4g_version_keywords.Contains(input_template))
            {
                return "P4G";
            }
            else if (Global.p5_ps4_version_keywords.Contains(input_template))
            {
                return "P5-PS4";
            }
            else if (Global.p5r_version_keywords.Contains(input_template))
            {
                return "P5R";
            }

            // Generic template keywords are handled by the user's version control settings.
            // Since there are multiple inputs that can lead to the desired template, if statements are used to decide the proper abbreviation to return.
            if (Global.p1_generic_keywords.Contains(input_template))
            {
                return account.VC_P1;
            }
            else if (Global.p2is_generic_keywords.Contains(input_template))
            {
                return account.VC_P2IS;
            }
            else if (Global.p2ep_generic_keywords.Contains(input_template))
            {
                return account.VC_P2EP;
            }
            else if (Global.p3_generic_keywords.Contains(input_template))
            {
                return account.VC_P3;
            }
            else if (Global.p4_generic_keywords.Contains(input_template))
            {
                return account.VC_P4;
            }
            else if (Global.p4au_generic_keywords.Contains(input_template))
            {
                return "P4AU";
            }
            else if (Global.p4d_generic_keywords.Contains(input_template))
            {
                return "P4D";
            }
            else if (Global.p5_generic_keywords.Contains(input_template))
            {
                return account.VC_P5;
            }
            else if (Global.p5s_generic_keywords.Contains(input_template))
            {
                return "P5S";
            }
            else if (Global.bbtag_generic_keywords.Contains(input_template))
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

                case "P3R":
                    return "Persona 3 Reload";

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

        public static string GetDisplayName(UserInfoFields account, MakerCharacterData maker_character_data)
        {
            OfficialSetData set_data = maker_character_data.Set_Data;
            BustupData bustup_data = maker_character_data.Bustup_Data;
            ulong user_id = Convert.ToUInt64(account.User_ID);
            string default_name = bustup_data.Default_Name_EN;
            
            switch (set_data.Origin)
            {
                case "P1-PS1":
                    if (account.P1_PSX_TS_Localized_Revelations_Names == "Off" && bustup_data.Revelations_Char_Original_Name_EN != default)
                    {
                        default_name = bustup_data.Revelations_Char_Original_Name_EN;
                    }
                    break;

                case "P2IS-PS1":
                    if (account.P2IS_PSX_TS_Localized_Revelations_Names == "Off" && bustup_data.Revelations_Char_Original_Name_EN != default)
                    {
                        default_name = bustup_data.Revelations_Char_Original_Name_EN;
                    }
                    break;

                case "P2EP-PS1":
                    if (account.P2EP_PSX_TS_Localized_Revelations_Names == "Off" && bustup_data.Revelations_Char_Original_Name_EN != default)
                    {
                        default_name = bustup_data.Revelations_Char_Original_Name_EN;
                    }
                    break;

                default:
                    // Do nothing
                    break;
            }

            DisplayNameTableData custom_name_data = DisplayNameLogging.GetCustomName(user_id, maker_character_data);

            if (custom_name_data == null)
            {
                if (maker_character_data.Base_Sprite == 0)
                {
                    maker_character_data.Base_Sprite = 1;
                    bustup_data = BustupDataMethods.Get_Bustup_Data(account, set_data, maker_character_data);
                    return default_name;
                }
                else
                {
                    return default_name;
                }
            }
            else
            {
                if (maker_character_data.Base_Sprite == 0)
                {
                    if (custom_name_data.Spriteless_Included == "Yes")
                    {
                        return custom_name_data.Display_Name;
                    }
                    else
                    {
                        maker_character_data.Base_Sprite = 1;
                        bustup_data = BustupDataMethods.Get_Bustup_Data(account, set_data, maker_character_data);
                        return default_name;
                    }
                }
                else
                {
                    return custom_name_data.Display_Name;
                }
            }    
        }

        // Main methods to assist the functions of the scene maker.
        public static async Task Set_List_Message_Directory(SocialLinkerCommand sl_command)
        {
            MakerCommandLogging.LogData(sl_command);

            if (Passes_Content_Filter_With_Fail_Error(sl_command, sl_command.MakerCommand.Template) == false)
            {
                return;
            }

            switch (sl_command.MakerCommand.Template)
            {
                case "P1-PS1":
                    await OfficialSetLists.P1_PS1_Set_List(sl_command);
                    return;

                case "P1-PSP":
                    await OfficialSetLists.P1_PSP_Set_List(sl_command);
                    return;

                case "P2IS-PS1":
                    await OfficialSetLists.P2IS_PS1_Set_List(sl_command);
                    return;

                case "P2IS-PSP":
                    await OfficialSetLists.P2IS_PSP_Set_List(sl_command);
                    return;

                case "P2EP-PS1":
                    await OfficialSetLists.P2EP_PS1_Set_List(sl_command);
                    return;

                case "P2EP-PSP":
                    await OfficialSetLists.P2EP_PSP_Set_List(sl_command);
                    return;

                case "P3F":
                    await OfficialSetLists.P3F_Set_List(sl_command);
                    return;

                case "P3P":
                    await OfficialSetLists.P3P_Set_List(sl_command);
                    return;

                case "P3R":
                    await OfficialSetLists.P3R_Set_List(sl_command);
                    return;

                case "P4-PS2":
                    await OfficialSetLists.P4_PS2_Set_List(sl_command);
                    return;

                case "P4G":
                    await OfficialSetLists.P4G_Set_List(sl_command);
                    return;

                case "P4AU":
                    await OfficialSetLists.P4AU_Set_List(sl_command);
                    return;

                case "P4D":
                    await OfficialSetLists.P4D_Set_List(sl_command);
                    return;

                case "P5-PS4":
                    await OfficialSetLists.P5_PS4_Set_List(sl_command);
                    return;

                case "P5R":
                    await OfficialSetLists.P5R_Set_List(sl_command);
                    return;

                case "P5S":
                    await OfficialSetLists.P5S_Set_List(sl_command);
                    return;

                case "BBTAG":
                    await OfficialSetLists.BBTAG_Set_List(sl_command);
                    return;
            }

            return;
        }

        public static async Task Sprite_Sheet_Message_Directory(SocialLinkerCommand sl_command, OfficialSetData sprite_set_info)
        {
            MakerCommandLogging.LogData(sl_command);

            if (Passes_Content_Filter_With_Fail_Error(sl_command, sprite_set_info.Origin) == false)
            {
                return;
            }

            switch (sprite_set_info.Origin)
            {
                case "P1-PS1":
                    await OfficialSetSheets.P1_PS1_Sprite_Sheet(sl_command, sprite_set_info);
                    return;

                case "P1-PSP":
                    await OfficialSetSheets.P1_PSP_Sprite_Sheet(sl_command, sprite_set_info);
                    return;

                case "P2IS-PS1":
                    await OfficialSetSheets.P2IS_PS1_Sprite_Sheet(sl_command, sprite_set_info);
                    return;

                case "P2IS-PSP":
                    await OfficialSetSheets.P2IS_PSP_Sprite_Sheet(sl_command, sprite_set_info);
                    return;

                case "P2EP-PS1":
                    await OfficialSetSheets.P2EP_PS1_Sprite_Sheet(sl_command, sprite_set_info);
                    return;

                case "P2EP-PSP":
                    await OfficialSetSheets.P2EP_PSP_Sprite_Sheet(sl_command, sprite_set_info);
                    return;

                case "P3F":
                    await OfficialSetSheets.P3F_Sprite_Sheet(sl_command, sprite_set_info);
                    return;

                case "P3P":
                    await OfficialSetSheets.P3P_Sprite_Sheet(sl_command, sprite_set_info);
                    return;

                case "P3R":
                    await OfficialSetSheets.P3R_Sprite_Sheet(sl_command, sprite_set_info);
                    return;

                case "P4-PS2":
                    await OfficialSetSheets.P4_PS2_Sprite_Sheet(sl_command, sprite_set_info);
                    return;

                case "P4G":
                    await OfficialSetSheets.P4G_Sprite_Sheet(sl_command, sprite_set_info);
                    return;

                case "P4AU":
                    await OfficialSetSheets.P4AU_Sprite_Sheet(sl_command, sprite_set_info);
                    return;

                case "P4D":
                    await OfficialSetSheets.P4D_Sprite_Sheet(sl_command, sprite_set_info);
                    return;

                case "P5-PS4":
                    await OfficialSetSheets.P5_PS4_Sprite_Sheet(sl_command, sprite_set_info);
                    return;

                case "P5R":
                    await OfficialSetSheets.P5R_Sprite_Sheet(sl_command, sprite_set_info);
                    return;

                case "P5S":
                    await OfficialSetSheets.P5S_Sprite_Sheet(sl_command, sprite_set_info);
                    return;

                case "BBTAG":
                    await OfficialSetSheets.BBTAG_Sprite_Sheet(sl_command, sprite_set_info);
                    return;
            }
            return;
        }

        public static bool Base_Sprite_Validity_Check(SocialLinkerCommand sl_command, OfficialSetData set_data, MakerCommandData maker_command_data)
        {
            // Establish the directory of the specified sprite set.
            string set_path = $@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//{set_data.Origin}//Bustup//{set_data.ID}";

            // Get a count of how many files are in the sprite set's directory.
            int filecount = AttachmentCountItemDirectory(set_path);

            // Now that we have a filecount for the set, let's see if the inputted sprite number is valid before we continue.
            // If not, send an error message and cancel the request.
            if (maker_command_data.Character_Data_1.Base_Sprite > filecount)
            {
                _ = ErrorHandling.Sprite_Number_Not_Found(sl_command, set_data.Name, AcronymToFullTitle(set_data.Origin));
                return false;
            }

            return true;
        }

        public static async Task Bustup_Frame_Sheet_Message_Directory(SocialLinkerCommand sl_command, OfficialSetData set_data, MakerCommandData command_data)
        {
            MakerCommandLogging.LogData(sl_command);

            if (Passes_Content_Filter_With_Fail_Error(sl_command, set_data.Origin) == false)
            {
                return;
            }

            switch (set_data.Origin)
            {
                case "P1-PS1":
                    await BustupFrameSheets.P1_PS1_Bustup_Frame_Sheet(sl_command, set_data, command_data);
                    return;

                case "P1-PSP":
                    await BustupFrameSheets.P1_PSP_Bustup_Frame_Sheet(sl_command, set_data, command_data);
                    return;

                case "P2IS-PS1":
                    await BustupFrameSheets.P2IS_PS1_Bustup_Frame_Sheet(sl_command, set_data, command_data);
                    return;

                case "P2IS-PSP":
                    await BustupFrameSheets.P2IS_PSP_Bustup_Frame_Sheet(sl_command, set_data, command_data);
                    return;

                case "P2EP-PS1":
                    await BustupFrameSheets.P2EP_PS1_Bustup_Frame_Sheet(sl_command, set_data, command_data);
                    return;

                case "P2EP-PSP":
                    await BustupFrameSheets.P2EP_PSP_Bustup_Frame_Sheet(sl_command, set_data, command_data);
                    return;

                case "P3F":
                    await BustupFrameSheets.P3F_Bustup_Frame_Sheet(sl_command, set_data, command_data);
                    return;

                case "P3P":
                    await BustupFrameSheets.P3P_Bustup_Frame_Sheet(sl_command, set_data, command_data);
                    return;

                case "P4-PS2":
                    await BustupFrameSheets.P4_PS2_Bustup_Frame_Sheet(sl_command, set_data, command_data);
                    return;

                case "P4G":
                    await BustupFrameSheets.P4G_Bustup_Frame_Sheet(sl_command, set_data, command_data);
                    return;

                case "P4AU":
                    await BustupFrameSheets.P4AU_Bustup_Frame_Sheet(sl_command, set_data, command_data);
                    return;

                case "P4D":
                    await BustupFrameSheets.P4D_Bustup_Frame_Sheet(sl_command, set_data, command_data);
                    return;

                case "P5-PS4":
                    await BustupFrameSheets.P5_PS4_Bustup_Frame_Sheet(sl_command, set_data, command_data);
                    return;

                case "P5R":
                    await BustupFrameSheets.P5R_Bustup_Frame_Sheet(sl_command, set_data, command_data);
                    return;

                case "P5S":
                    await BustupFrameSheets.P5S_Bustup_Frame_Sheet(sl_command, set_data, command_data);
                    return;

                case "BBTAG":
                    await BustupFrameSheets.BBTAG_Bustup_Frame_Sheet(sl_command, set_data, command_data);
                    return;
            }
            return;
        }

        public static OfficialSetData Search_By_Title_And_ID(string title, string id)
        {
            foreach (OfficialSetData s in sprite_set_list)
            {
                if (s.Origin == title && s.ID == id)
                {
                    return s;
                }
            }

            return null;
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
            List<string> specified_set_list = new List<string>();

            foreach (OfficialSetData s in sprite_set_list)
            {
                if (s.Origin == "P1-PS1" && (s.ID != "B29" && s.ID != "B30" && s.ID != "B31" && s.ID != "B32" && s.ID != "B33"))
                {
                    specified_set_list.Add(s.Name);
                }
            }

            specified_set_list = specified_set_list.OrderBy(s => s).ToList();

            string sorted_string = "";

            foreach (string s in specified_set_list)
            {
                sorted_string += $"- {s}\n";
            }

            sorted_string += "\n";
            sorted_string += "**__Snow Queen Quest__**\n";

            List<string> secondary_set_list = new List<string>();

            foreach (OfficialSetData s in sprite_set_list)
            {
                if (s.Origin == "P1-PS1" && (s.ID == "B29" || s.ID == "B30" || s.ID == "B31" || s.ID == "B32" || s.ID == "B33"))
                {
                    secondary_set_list.Add(s.Name);
                }
            }

            secondary_set_list = secondary_set_list.OrderBy(s => s).ToList();

            foreach (string s in secondary_set_list)
            {
                sorted_string += $"- {s}\n";
            }

            // Return the string variable.
            return sorted_string;
        }

        public static string Generate_P2IS_PSP_Set_List()
        {
            // Create an empty string list.
            List<string> specified_set_list = new List<string>();

            int current_id = 0;

            foreach (OfficialSetData s in sprite_set_list)
            {
                current_id = Int32.Parse(s.ID.Substring(1));

                if (s.Origin == "P2IS-PSP" && (current_id < 74))
                {
                    specified_set_list.Add(s.Name);
                }
            }

            specified_set_list = specified_set_list.OrderBy(s => s).ToList();

            string sorted_string = "";

            foreach (string s in specified_set_list)
            {
                sorted_string += $"- {s}\n";
            }

            sorted_string += "\n";
            sorted_string += "**__Climax Theater__**\n";

            List<string> secondary_set_list = new List<string>();

            foreach (OfficialSetData s in sprite_set_list)
            {
                current_id = Int32.Parse(s.ID.Substring(1));

                if (s.Origin == "P2IS-PSP" && (current_id >= 74))
                {
                    secondary_set_list.Add(s.Name);
                }
            }

            secondary_set_list = secondary_set_list.OrderBy(s => s).ToList();

            foreach (string s in secondary_set_list)
            {
                sorted_string += $"- {s}\n";
            }

            return sorted_string;
        }

        public static string Generate_P2EP_PSP_Set_List()
        {
            List<string> specified_set_list = new List<string>();

            int current_id = 0;

            foreach (OfficialSetData s in sprite_set_list)
            {
                current_id = Int32.Parse(s.ID.Substring(1));

                if (s.Origin == "P2EP-PSP" && (current_id < 82))
                {
                    specified_set_list.Add(s.Name);
                }
            }

            specified_set_list = specified_set_list.OrderBy(s => s).ToList();

            string sorted_string = "";

            foreach (string s in specified_set_list)
            {
                sorted_string += $"- {s}\n";
            }

            sorted_string += "\n";
            sorted_string += "**__Additional Scenario__**\n";

            List<string> secondary_set_list = new List<string>();

            foreach (OfficialSetData s in sprite_set_list)
            {
                current_id = Int32.Parse(s.ID.Substring(1));

                if (s.Origin == "P2EP-PSP" && (current_id >= 82))
                {
                    secondary_set_list.Add(s.Name);
                }
            }

            secondary_set_list = secondary_set_list.OrderBy(s => s).ToList();

            foreach (string s in secondary_set_list)
            {
                sorted_string += $"- {s}\n";
            }

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

        public static async Task Quick_Scene_Directory(SocialLinkerCommand sl_command)
        {
            MakerCommandLogging.LogData(sl_command);

            var set_data = sl_command.MakerCommand.Character_Data_1.Set_Data;

            if (sl_command.MakerCommand.Template == "")
            {
                if (Passes_Content_Filter_With_Fail_Error(sl_command, set_data.Origin) == false)
                {
                    return;
                }

                switch (set_data.Origin)
                {
                    case "P1-PS1":
                        RenderP1_PS1 p1_ps1_render = new RenderP1_PS1();
                        await p1_ps1_render.Render_Quick_Scene_P1_PS1(sl_command);
                        return;

                    case "P1-PSP":
                        RenderP1_PSP p1_psp_render = new RenderP1_PSP();
                        await p1_psp_render.Render_Quick_Scene_P1_PSP(sl_command);
                        return;

                    case "P2IS-PS1":
                        RenderP2IS_PS1 p2is_ps1_render = new RenderP2IS_PS1();
                        await p2is_ps1_render.Render_Quick_Scene_P2IS_PS1(sl_command);
                        return;

                    case "P2IS-PSP":
                        RenderP2IS_PSP p2is_psp_render = new RenderP2IS_PSP();
                        await p2is_psp_render.Render_Quick_Scene_P2IS_PSP(sl_command);
                        return;

                    case "P2EP-PS1":
                        RenderP2EP_PS1 p2ep_ps1_render = new RenderP2EP_PS1();
                        await p2ep_ps1_render.Render_Quick_Scene_P2EP_PS1(sl_command);
                        return;

                    case "P2EP-PSP":
                        RenderP2EP_PSP p2ep_psp_render = new RenderP2EP_PSP();
                        await p2ep_psp_render.Render_Quick_Scene_P2EP_PSP(sl_command);
                        return;

                    case "P3F":
                        RenderP3F p3f_render = new RenderP3F();
                        await p3f_render.Render_Quick_Scene_P3F(sl_command);
                        return;

                    case "P3P":
                        RenderP3P p3p_render = new RenderP3P();
                        await p3p_render.Render_Quick_Scene_P3P(sl_command);
                        return;

                    case "P4-PS2":
                        RenderP4_PS2 p4_ps2_render = new RenderP4_PS2();
                        await p4_ps2_render.Render_Quick_Scene_P4_PS2(sl_command);
                        return;

                    case "P4G":
                        await RenderP4G.Render_Quick_Scene_P4G(sl_command);
                        return;

                    case "P4AU":
                        RenderP4AU p4au_render = new RenderP4AU();
                        await p4au_render.Render_Quick_Scene_P4AU(sl_command);
                        return;

                    case "P4D":
                        RenderP4D p4d_render = new RenderP4D();
                        await p4d_render.Render_Quick_Scene_P4D(sl_command);
                        return;

                    case "P5-PS4":
                        RenderP5_PS4 p5_ps4_render = new RenderP5_PS4();
                        await p5_ps4_render.Render_Quick_Scene_P5_PS4(sl_command);
                        return;

                    case "P5R":
                        RenderP5R p5r_render = new RenderP5R();
                        await p5r_render.Render_Quick_Scene_P5R(sl_command);
                        return;

                    case "P5S":
                        RenderP5S p5s_render = new RenderP5S();
                        await p5s_render.Render_Quick_Scene_P5S(sl_command);
                        return;

                    case "BBTAG":
                        RenderBBTAG bbtag_render = new RenderBBTAG();
                        await bbtag_render.Render_Quick_Scene_BBTAG(sl_command);
                        return;
                }
            }
            
            return;
        }

        public static async Task System_Message_Directory(SocialLinkerCommand sl_command)
        {
            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(sl_command.User);
            MakerCommandData maker_command_data = sl_command.MakerCommand;

            // Convert the user's template input into one usable depending on their version control settings.
            string template = InputToTemplate(account, maker_command_data.Character_Data_1.Sprite_Set_Version);

            MakerCommandLogging.LogData(sl_command);

            if (Passes_Content_Filter_With_Fail_Error(sl_command, template) == false)
            {
                return;
            }

            switch (template)
            {
                case "P1-PS1":
                    RenderP1_PS1 p1_ps1_render = new RenderP1_PS1();
                    await p1_ps1_render.Render_System_Message(sl_command, maker_command_data);
                    return;

                case "P1-PSP":
                    RenderP1_PSP p1_psp_render = new RenderP1_PSP();
                    await p1_psp_render.Render_System_Message(sl_command, maker_command_data);
                    return;

                case "P2IS-PS1":
                    RenderP2IS_PS1 p2is_ps1_render = new RenderP2IS_PS1();
                    await p2is_ps1_render.Render_System_Message(sl_command, maker_command_data);
                    return;

                case "P2IS-PSP":
                    RenderP2IS_PSP p2is_psp_render = new RenderP2IS_PSP();
                    await p2is_psp_render.Render_System_Message(sl_command, maker_command_data);
                    return;

                case "P2EP-PS1":
                    RenderP2EP_PS1 p2ep_ps1_render = new RenderP2EP_PS1();
                    await p2ep_ps1_render.Render_System_Message(sl_command, maker_command_data);
                    return;

                case "P2EP-PSP":
                    RenderP2EP_PSP p2ep_psp_render = new RenderP2EP_PSP();
                    await p2ep_psp_render.Render_System_Message(sl_command, maker_command_data);
                    return;

                case "P3F":
                    RenderP3F p3f_render = new RenderP3F();
                    await p3f_render.Render_System_Message(sl_command, maker_command_data);
                    return;

                case "P3P":
                    RenderP3P p3p_render = new RenderP3P();
                    await p3p_render.Render_System_Message(sl_command, maker_command_data);
                    return;

                case "P4-PS2":
                    RenderP4_PS2 p4_ps2_render = new RenderP4_PS2();
                    await p4_ps2_render.Render_System_Message(sl_command, maker_command_data);
                    return;

                case "P4G":
                    await RenderP4G.Render_System_Message(sl_command, maker_command_data);
                    return;

                case "P4AU":
                    RenderP4AU p4au_render = new RenderP4AU();
                    await p4au_render.Render_System_Message(sl_command, maker_command_data);
                    return;

                case "P4D":
                    RenderP4D p4d_render = new RenderP4D();
                    await p4d_render.Render_System_Message(sl_command, maker_command_data);
                    return;

                case "P5-PS4":
                    RenderP5_PS4 p5_ps4_render = new RenderP5_PS4();
                    await p5_ps4_render.Render_System_Message(sl_command);
                    return;

                case "P5R":
                    RenderP5R p5r_render = new RenderP5R();
                    await p5r_render.Render_System_Message(sl_command);
                    return;

                case "P5S":
                    RenderP5S p5s_render = new RenderP5S();
                    await p5s_render.Render_System_Message(sl_command, maker_command_data);
                    return;

                case "BBTAG":
                    RenderBBTAG bbtag_render = new RenderBBTAG();
                    await bbtag_render.Render_System_Message(sl_command, maker_command_data);
                    return;
            }

            return;
        }

        // Commonly Shared Methods
        public static List<string>[] Line_Parser(SocialLinkerCommand sl_command, string template, string dialogue, int max_line_count, int max_line_length)
        {
            // The number of pixels in a line remaining. This will gradually decrease as the pixel length of characters are subtracted from it.
            int line_length_remaining = max_line_length;

            // Completed word string. Characters will be added to this string one-by-one until a space, line break, or end-of-input is encountered.
            string completed_word = "";

            // Create an array of string lists and initialize them.
            // These are where our dialogue input will be organized.
            List<string>[] dialogue_list = new List<string>[max_line_count];

            for (int i = 0; i < max_line_count; i++)
            {
                dialogue_list[i] = new List<string>();
            }

            // Now that we have our string lists created, we need a variable to dynamically change which line we're currently on.
            // For that, create an int variable and initialize it to zero for starting on the first line.
            int current_line = 0;

            // Take the input dialogue and convert it into a char array. This is how we'll iterate through the dialogue character-by-character.
            char[] dialogue_array = dialogue.ToCharArray();

            bool add_text = true;

            // Create a for loop meant to iterate through the dialogue array.
            for (int i = 0; i < dialogue_array.Length; i++)
            {
                if (add_text == true)
                {
                    // Check if the completed word string is empty, the remaining pixel length of the current line is at the max value, and if the current iterated character is a space.
                    if ((completed_word == "") && (line_length_remaining == max_line_length) && (dialogue_array[i] == ' '))
                    {
                        // We want to skip any spaces that appear at the start of a line, so do nothing here.
                    }
                    // Check if the contents of the current index is not a space, not a line break, and not the end of the array.
                    else if ((dialogue_array[i] != ' ') && (dialogue_array[i] != '\u000a') && (i != dialogue_array.Length - 1))
                    {
                        // If so, add the currently iterated char to the completed word string.
                        completed_word += dialogue_array[i];
                    }
                    // Next, check if the contents of the current index IS a space, IS a line break, or IS the end of the array.
                    else if ((dialogue_array[i] == ' ') || (dialogue_array[i] == '\u000a') || (i == dialogue_array.Length - 1))
                    {
                        // If so, add the currently iterated char to the completed word string.
                        completed_word += dialogue_array[i];

                        // Now that we have our word, measure the pixel length of the completed string.
                        int completed_word_length = Measure_Word_Pixel_Length_Redirect(sl_command, template, completed_word);

                        // Check if the completed word is under the current line's allowed length.
                        // This is done by subtracting the completed word string's length from the remaining length of the line.
                        // If the result is greater than zero, it's a perfect fit.
                        if ((line_length_remaining - completed_word_length >= 0) && (dialogue_array[i] != '\u000a'))
                        {
                            // Subtract the completed word's pixel length from the remaining pixel length of the current line.
                            line_length_remaining = line_length_remaining - completed_word_length;

                            // Add the completed word to the current line.
                            dialogue_list[current_line].Add(completed_word);

                            // Reset the completed word variable to an empty string.
                            completed_word = "";
                        }

                        // Else, check if all three of the following conditions are met:
                        // If there is no more room to add the completed word to the current line.
                        // The completed word's length is less than or equal to a line itself.
                        // The current iterated character is NOT a line break.
                        else if ((line_length_remaining - completed_word_length < 0) && (completed_word_length <= max_line_length) && (dialogue_array[i] != '\u000a'))
                        {
                            // Check if the current line number is less than the max number of lines available.
                            if (current_line < max_line_count - 1)
                            {
                                // Increase the current line number.
                                current_line++;

                                // Add the completed word string to the current line.
                                dialogue_list[current_line].Add(completed_word);

                                // Reset the remaining pixel length variable to the start and subtract the pixel length of the completed word string.
                                // This is done because we moved to a new line.
                                line_length_remaining = max_line_length - completed_word_length;

                                // Reset the completed word variable to an empty string.
                                completed_word = "";
                            }
                            // Else, check if the current line number is greater than or equal to the max number of lines available.
                            else if (current_line >= max_line_count - 1)
                            {
                                // If so, there is no more room to render text.
                                // Break from the for loop.
                                add_text = false;
                                break;
                            }
                        }

                        // Else, check if all three of the following conditions are met:
                        // If there IS room to add the completed word to the current line.
                        // The completed word's length is less than or equal to the length of a line itself.
                        // The current iterated character IS a line break.
                        else if ((line_length_remaining - completed_word_length >= 0) && (completed_word_length <= max_line_length) && (dialogue_array[i] == '\u000a'))
                        {
                            // Check if the current line number is less than the max number of lines available.
                            if (current_line < max_line_count - 1)
                            {
                                // Since there is room, add the completed word string to the current line.
                                dialogue_list[current_line].Add(completed_word);

                                // Increase the current line number.
                                current_line++;

                                // Reset the remaining pixel length variable to the max value.
                                // This is done because we moved to a new line.
                                line_length_remaining = max_line_length;

                                // Reset the completed word variable to an empty string.
                                completed_word = "";
                            }
                            // Else, check if the current line number is greater than or equal to the max number of lines available.
                            else if (current_line >= max_line_count - 1)
                            {
                                // If so, there is no more room to render text.
                                // Break from the for loop.
                                add_text = false;
                                break;
                            }
                        }

                        // Else, check if there is no more room to add the completed word to the current line AND the completed word's length is greater than the length of a line itself.
                        // This means that we'll need to split the string up on different lines.
                        else if (line_length_remaining - completed_word_length < 0 && completed_word_length > max_line_length)
                        {
                            // Take the completed word and turn it into a char array.
                            // We'll use this to iterate through the word character-by-character to decide where to split the string.
                            char[] completed_word_array = completed_word.ToCharArray();

                            // Create a new string variable and initialize it to an empty string.
                            // Similar to the completed word variable, this string will contain characters that will fit on a single line.
                            // Because we know the word will be split into multiple lines, this will only contain part of the full string at any given time, hence "substring".
                            string substring = "";

                            // Create an int variable and initialize it to zero.
                            // This will contain the pixel length of our substring variable once we measure it.
                            int substring_length = 0;

                            // Create a for loop to iterate through the completed word array.
                            for (int j = 0; j < completed_word_array.Length; j++)
                            {
                                //Console.WriteLine($"Current char: {completed_word_array[j]}");

                                // Add the currently iterated character to the substring.
                                substring += completed_word_array[j];

                                // Measure the pixel length of the substring so far.
                                substring_length = Measure_Word_Pixel_Length_Redirect(sl_command, template, substring);

                                if (current_line > max_line_count - 1)
                                {
                                    add_text = false;
                                    break;
                                }
                                // Check if there is no more room to add another character to the current line, OR if the current character is a line break.
                                // Since we are iterating through the string character-by-character, this should trigger the moment the length hits the line boundary.
                                else if ((line_length_remaining - substring_length <= 0) || (completed_word_array[j] == '\u000a'))
                                {
                                    // Check if the current line number is less than the max number of lines available.
                                    if (current_line <= max_line_count - 1)
                                    {
                                        // Add the substring to the current line.
                                        dialogue_list[current_line].Add(substring);

                                        // Since there is absolutely no more room on the current line left, increase the current line value.
                                        current_line++;

                                        // Reset the remaining pixel length variable to the max value.
                                        // This is done because we moved to a new line.
                                        line_length_remaining = max_line_length;

                                        // Reset the substring variable to an empty string.
                                        substring = "";
                                    }
                                }
                                // Else, check if the last index of the completed word array has been reached.
                                else if (j == completed_word_array.Length - 1)
                                {
                                    // Add the substring to the current line.
                                    dialogue_list[current_line].Add(substring);

                                    // Subtract the completed word's pixel length from the remaining pixel length of the current line.
                                    line_length_remaining = line_length_remaining - substring_length;

                                    // Reset the substring variable to an empty string.
                                    substring = "";
                                }
                            }

                            // Reset the completed word string to an empty string.
                            completed_word = "";
                        }
                    }
                }
            }

            return dialogue_list;
        }

        public static string Validate_Input(SocialLinkerCommand sl_command, string title, string input_type, string input)
        {
            List<char> char_array = input.ToCharArray().ToList();
            string return_value = "";
            ParsingFields glyph_info;

            char[] hearts = { '♥', '♡', '❣', '❤' };
            char[] ba_gua = { '☰', '☱', '☲', '☳', '☴', '☵', '☶', '☷' };

            for (int i = 0; i < char_array.Count; i++)
            {
                switch (title)
                {
                    case "P1-PS1":
                        glyph_info = ParsingMethods.Get_P1_PS1_Glyph(char_array[i]);
                        break;

                    case "P1-PSP":
                        glyph_info = ParsingMethods.Get_P1_PSP_Glyph(char_array[i]);
                        break;

                    case "P2IS-PS1":
                        glyph_info = ParsingMethods.Get_P2IS_PS1_Glyph(char_array[i]);

                        if (glyph_info == null && hearts.Contains(char_array[i]))
                        {
                            glyph_info = new ParsingFields();
                        }
                        else if (glyph_info == null && ba_gua.Contains(char_array[i]))
                        {
                            glyph_info = new ParsingFields();
                        }
                        break;

                    case "P2IS-PSP":
                        glyph_info = ParsingMethods.Get_P2IS_PSP_Glyph(char_array[i]);
                        break;

                    case "P2EP-PS1":
                        glyph_info = ParsingMethods.Get_P2EP_PS1_Glyph(char_array[i]);

                        if (glyph_info == null && hearts.Contains(char_array[i]))
                        {
                            glyph_info = new ParsingFields();
                        }
                        else if (glyph_info == null && ba_gua.Contains(char_array[i]))
                        {
                            glyph_info = new ParsingFields();
                        }
                        break;

                    case "P2EP-PSP":
                        glyph_info = ParsingMethods.Get_P2EP_PSP_Glyph(char_array[i]);
                        break;

                    case "P3F":
                        glyph_info = ParsingMethods.Get_P3F_Glyph(char_array[i]);
                        break;

                    case "P3P":
                        glyph_info = ParsingMethods.Get_P3P_Glyph(char_array[i]);
                        break;

                    case "P4-PS2":
                        glyph_info = ParsingMethods.Get_P4_PS2_Glyph(char_array[i]);
                        break;

                    case "P4G":
                        glyph_info = ParsingMethods.Get_P4G_Glyph(char_array[i]);
                        break;

                    case "P4AU":
                        glyph_info = ParsingMethods.Get_P4AU_Glyph(char_array[i]);
                        break;

                    case "P4D":
                        glyph_info = ParsingMethods.Get_P4D_Glyph(char_array[i]);
                        break;

                    case "P5-PS4":
                        glyph_info = ParsingMethods.Get_P5_PS4_Glyph(char_array[i]);

                        if (glyph_info == null && hearts.Contains(char_array[i]))
                        {
                            glyph_info = new ParsingFields();
                        }
                        break;

                    case "P5R":
                        glyph_info = ParsingMethods.Get_P5R_Glyph(char_array[i]);

                        if (glyph_info == null && hearts.Contains(char_array[i]))
                        {
                            glyph_info = new ParsingFields();
                        }
                        break;

                    case "P5S":
                        glyph_info = ParsingMethods.Get_P5S_Glyph(char_array[i]);

                        if (glyph_info == null && hearts.Contains(char_array[i]))
                        {
                            glyph_info = new ParsingFields();
                        }
                        break;

                    case "BBTAG":
                        glyph_info = null;
                        break;

                    default:
                        glyph_info = null;
                        break;
                }

                if (glyph_info != null)
                {
                    return_value += char_array[i];
                }
                else if (char_array[i] == '\ufe0f')
                {
                    // Do nothing, emoji variation selector
                }
                else
                {
                    switch (input_type)
                    {
                        case "Dialogue":
                            sl_command.MakerCommand.Dialogue_Has_Invalid_Char = true;
                            break;

                        case "Name":
                            sl_command.MakerCommand.Display_Name_Has_Invalid_Char = true;
                            break;
                    }
                }
            }

            switch (input_type)
            {
                case "Dialogue":
                    if (return_value == "")
                    {
                        return_value = "......";
                    }

                    if (sl_command.MakerCommand.Dialogue_Has_Invalid_Char)
                    {
                        _ = ErrorHandling.Unsupported_Character_In_Dialogue(sl_command);
                    }
                    break;

                case "Name":
                    if (return_value == "")
                    {
                        return_value = "???";
                    }

                    if (sl_command.MakerCommand.Display_Name_Has_Invalid_Char)
                    {
                        _ = ErrorHandling.Unsupported_Character_In_Display_Name(sl_command);
                    }
                    break;
            }

            return return_value;
        }

        public static int Measure_Word_Pixel_Length_Redirect(SocialLinkerCommand sl_command, string template, string input_word)
        {
            switch (template)
            {
                case "P1-PS1":
                    return RenderP1_PS1.Measure_String_Pixel_Length(sl_command, input_word);

                case "P1-PSP":
                    return RenderP1_PSP.Measure_String_Pixel_Length(sl_command, input_word);

                case "P2IS-PS1":
                    RenderP2IS_PS1 p2is_ps1_measure = new RenderP2IS_PS1();
                    return p2is_ps1_measure.Measure_Word_Pixel_Length(sl_command, input_word);

                case "P2IS-PSP":
                    return RenderP2IS_PSP.Measure_Word_Pixel_Length(sl_command, input_word);

                case "P2EP-PS1":
                    RenderP2EP_PS1 p2ep_ps1_measure = new RenderP2EP_PS1();
                    return p2ep_ps1_measure.Measure_Word_Pixel_Length(sl_command, input_word);

                case "P2EP-PSP":
                    return RenderP2EP_PSP.Measure_Word_Pixel_Length(sl_command, input_word);

                case "P3F":
                    return RenderP3F.Measure_Word_Pixel_Length(sl_command, input_word);

                case "P3P":
                    RenderP3P p3p_measure = new RenderP3P();
                    return p3p_measure.Measure_Word_Pixel_Length(sl_command, input_word);

                case "P4-PS2":
                    return RenderP4_PS2.Measure_Word_Pixel_Length(sl_command, input_word);

                case "P4G":
                    return RenderP4G.Measure_Word_Pixel_Length(sl_command, input_word);

                case "P4AU":
                    return RenderP4AU.Measure_Word_Pixel_Length(sl_command, input_word);

                case "P4D":
                    return RenderP4D.Measure_Word_Pixel_Length(sl_command, input_word);

                case "P5-PS4":
                    return RenderP5_PS4.Measure_Word_Pixel_Length(sl_command, input_word);

                case "P5R":
                    return RenderP5R.Measure_Word_Pixel_Length(sl_command, input_word);

                case "P5S":
                    return RenderP5S.Measure_String_Pixel_Length(sl_command, input_word);

                case "BBTAG":
                    return 0;

                default:
                    return 0;
            }
        }

        public static Bitmap Render_Background(SocialLinkerCommand sl_command, int template_width, int template_height)
        {
            var account = UserInfoClasses.GetAccount(sl_command.User);
            var attachment = sl_command.MakerCommand.Background;

            Bitmap background = new Bitmap(2, 2);

            if (attachment != null)
            {
                // Here, we'll want to try and retrieve the user's input image.
                try
                {
                    // Declare variables for a web request to retrieve the image.
                    System.Net.HttpWebRequest webRequest = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(attachment.Url);
                    webRequest.AllowWriteStreamBuffering = true;
                    webRequest.Timeout = 30000;

                    // Create a stream and download the image to it.
                    System.Net.WebResponse webResponse = webRequest.GetResponse();
                    System.IO.Stream stream = webResponse.GetResponseStream();

                    // Copy the stream's contents to the background bitmap variable.
                    background = (Bitmap)System.Drawing.Image.FromStream(stream);

                    webResponse.Close();
                }
                // If an exception occurs here, the filetype is likely incompatible.
                // Send an error message, delete the loading message, and return.
                catch (System.ArgumentException e)
                {
                    Console.WriteLine(e);
                    throw new ArgumentException();
                }
            }

            switch (account.Setting_BG_Upload)
            {
                case "Scale to Width":
                    background = Scale_To_Width(background, template_width, template_height);
                    break;

                case "Scale to Height":
                    background = Scale_To_Height(background, template_width, template_height);
                    break;

                case "Scale to Fit":
                    background = Scale_To_Fit(background, template_width, template_height);
                    break;

                case "Scale to Fill":
                    background = Scale_To_Fill(background, template_width, template_height);
                    break;

                case "Stretch to Fill":
                    background = Stretch_To_Fill(background, template_width, template_height);
                    break;
            }

            return background;
        }

        public static Bitmap Render_Colored_Background(UserInfoFields account, int template_width, int template_height)
        {
            Bitmap colored_background_bitmap = new Bitmap(template_width, template_height);

            // The user may have a custom mono-colored background designated in their settings. Let's handle that now.
            // Check if the user's background color setting is set to something other than "Transparent".
            // If so, we have a color to render for the background!
            if (account.Setting_BG_Color != "Transparent")
            {
                // Convert the user's HTML color setting to one we can use and assign it to a color variable.
                System.Drawing.Color user_background_color = System.Drawing.ColorTranslator.FromHtml(account.Setting_BG_Color);

                // Color the entirety of the background bitmap the user's selected color.
                using (Graphics graphics = Graphics.FromImage(colored_background_bitmap))
                {
                    graphics.Clear(user_background_color);
                }
            }

            return colored_background_bitmap;
        }

        public static List<string> ParseContentFilter(UserInfoFields account)
        {
            //Create a list variable to return
            List<string> input_substring;

            //Specify the characters to divide the incoming string by
            char[] delimiterChars = { ';' };

            //Assign the return value to the input account's content filter string with its entries split into a list
            input_substring = account.Content_Filter.Split(delimiterChars).ToList();

            return input_substring;
        }

        public static bool Passes_Content_Filter(UserInfoFields account, string template)
        {
            var content_filter = ParseContentFilter(account);

            if (content_filter.Contains(template))
            {
                return false;
            }

            return true;
        }

        public static bool Passes_Content_Filter_With_Fail_Error(SocialLinkerCommand sl_command, string template)
        {
            var account = UserInfoClasses.GetAccount(sl_command.User);

            if (!Passes_Content_Filter(account, template))
            {
                _ = ErrorHandling.Content_Filter_Enabled(sl_command, template);
                return false;
            }

            return true;
        }

        public static Bitmap Scale_To_Width(Bitmap scrBitmap, int template_width, int template_height)
        {
            float width = template_width;
            float height = template_height;

            var image = new Bitmap(scrBitmap);

            float scale = width / image.Width;

            var bmp = new Bitmap((int)width, (int)height);
            var graph = Graphics.FromImage(bmp);

            // uncomment for higher quality output
            graph.InterpolationMode = InterpolationMode.High;
            graph.CompositingQuality = CompositingQuality.HighQuality;
            graph.SmoothingMode = SmoothingMode.AntiAlias;

            bmp.SetResolution(96, 96);

            var scaleWidth = (int)(image.Width * scale);
            var scaleHeight = (int)(image.Height * scale);

            graph.DrawImage(image, ((int)width - scaleWidth) / 2, ((int)height - scaleHeight) / 2, scaleWidth, scaleHeight);

            return bmp;
        }

        public static Bitmap Scale_To_Height(Bitmap scrBitmap, int template_width, int template_height)
        {
            float width = template_width;
            float height = template_height;

            var image = new Bitmap(scrBitmap);

            float scale = height / image.Height;

            var bmp = new Bitmap((int)width, (int)height);
            var graph = Graphics.FromImage(bmp);

            // uncomment for higher quality output
            graph.InterpolationMode = InterpolationMode.High;
            graph.CompositingQuality = CompositingQuality.HighQuality;
            graph.SmoothingMode = SmoothingMode.AntiAlias;

            bmp.SetResolution(96, 96);

            var scaleWidth = (int)(image.Width * scale);
            var scaleHeight = (int)(image.Height * scale);

            graph.DrawImage(image, ((int)width - scaleWidth) / 2, ((int)height - scaleHeight) / 2, scaleWidth, scaleHeight);

            return bmp;
        }

        public static Bitmap Scale_To_Fit(Bitmap scrBitmap, int template_width, int template_height)
        {
            float width = template_width;
            float height = template_height;

            var image = new Bitmap(scrBitmap);

            float scale = Math.Min(width / image.Width, height / image.Height);

            var bmp = new Bitmap((int)width, (int)height);
            var graph = Graphics.FromImage(bmp);

            // uncomment for higher quality output
            graph.InterpolationMode = InterpolationMode.High;
            graph.CompositingQuality = CompositingQuality.HighQuality;
            graph.SmoothingMode = SmoothingMode.AntiAlias;

            bmp.SetResolution(96, 96);

            var scaleWidth = (int)(image.Width * scale);
            var scaleHeight = (int)(image.Height * scale);

            graph.DrawImage(image, ((int)width - scaleWidth) / 2, ((int)height - scaleHeight) / 2, scaleWidth, scaleHeight);

            return bmp;
        }

        public static Bitmap Scale_To_Fill(Bitmap input_bitmap, int template_width, int template_height)
        {
            float width = template_width;
            float height = template_height;

            var image = new Bitmap(input_bitmap);

            float scale = Math.Max(width / image.Width, height / image.Height);

            var bmp = new Bitmap((int)width, (int)height);
            var graph = Graphics.FromImage(bmp);

            // uncomment for higher quality output
            graph.InterpolationMode = InterpolationMode.High;
            graph.CompositingQuality = CompositingQuality.HighQuality;
            graph.SmoothingMode = SmoothingMode.AntiAlias;

            bmp.SetResolution(96, 96);

            var scaleWidth = (int)(image.Width * scale);
            var scaleHeight = (int)(image.Height * scale);

            graph.DrawImage(image, ((int)width - scaleWidth) / 2, ((int)height - scaleHeight) / 2, scaleWidth, scaleHeight);

            return bmp;
        }

        public static Bitmap Stretch_To_Fill(Bitmap input_bitmap, int template_width, int template_height)
        {
            // Set the width and height of the bitmap to be created
            float width = template_width;
            float height = template_height;

            // Copy the input bitmap to a new variable.
            var bitmap_copy = new Bitmap(input_bitmap);

            // Create a brand new bitmap with the specified dimensions from earlier.
            var new_bitmap = new Bitmap((int)width, (int)height);

            // Create a graphics object so we can edit this new bitmap.
            var graphics = Graphics.FromImage(new_bitmap);

            // uncomment for higher quality output
            //graph.InterpolationMode = InterpolationMode.High;
            //graph.CompositingQuality = CompositingQuality.HighQuality;
            //graph.SmoothingMode = SmoothingMode.AntiAlias;
            new_bitmap.SetResolution(96, 96);

            // Draw the copy of the input bitmap to the new bitmap.
            graphics.DrawImage(bitmap_copy, 0, 0, width, height);

            return new_bitmap;
        }

        public static bool Is_Holiday(DateTime user_time)
        {
            HolidayDataMethods holiday_methods = new HolidayDataMethods(user_time);

            if (holiday_methods.Is_Holiday(user_time))
            {
                return true;
            }

            return false;
        }

        public static bool Is_School_Term(DateTime user_time)
        {
            AcademicDataMethods academic_methods = new AcademicDataMethods(user_time);

            if (academic_methods.Is_School_Term(user_time))
            {
                return true;
            }

            return false;
        }

        // Bustup construction
        public static Bitmap Bustup_Selection(SocialLinkerCommand sl_command, UserInfoFields account, MakerCharacterData maker_character_data)
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
                Bitmap bustup_with_frames = Construct_Bustup_With_Frames(sl_command, maker_character_data, base_sprite, false);
                return bustup_with_frames;
            }
        }

        public static Bitmap Reverse_Bustup_Selection(SocialLinkerCommand sl_command, UserInfoFields account, MakerCharacterData maker_character_data, Bitmap bustup)
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
                    Bitmap bustup_with_frames = Construct_Bustup_With_Frames(sl_command, maker_character_data, base_sprite, true);
                    return bustup_with_frames;
                }
            }
            else
            {
                return bustup;
            }
        }

        public static Bitmap Construct_Bustup_With_Frames(SocialLinkerCommand sl_command, MakerCharacterData maker_character_data, Bitmap bustup, bool reverse_file_exists)
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
                    _ = ErrorHandling.Eye_Frame_Not_Found(sl_command, maker_character_data, set_data.Name, AcronymToFullTitle(set_data.Origin));
                    return null;
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
                    _ = ErrorHandling.Mouth_Frame_Not_Found(sl_command, maker_character_data, set_data.Name, AcronymToFullTitle(set_data.Origin));
                    return null;
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
