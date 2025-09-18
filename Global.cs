using System.Collections.Generic;
using System.Linq;
using SocialLinker.Cooldown;
using SocialLinker.Core.CloudStorageTables;
using SocialLinker.Core.Menus;
using SocialLinker.Core.SceneMaker.TemplateRenders.QuickScenes;

namespace SocialLinker
{
    internal static class Global
    {
        internal static List<MenuIdStructure> MenuIdList { get; set; } = new List<MenuIdStructure>();
        internal static List<ItemListIterator> ItemIdList { get; set; } = new List<ItemListIterator>();
        internal static List<UserCooldownFields> CooldownList { get; set; } = new List<UserCooldownFields>();
        internal static List<ContentFilter> ContentFilterList { get; set; } = new List<ContentFilter>();
        internal static List<DisplayNameTableData> DisplayNameTableList { get; set; } = new List<DisplayNameTableData>();
        internal static List<DisplayNameInternalData> DisplayNameTempList { get; set; } = new List<DisplayNameInternalData>();
        internal static List<ContextSwitchData> P1_PS1_Usage_List { get; set; } = new List<ContextSwitchData>();
        internal static List<PlacementSwitchData> P1_PSP_Usage_List { get; set; } = new List<PlacementSwitchData>();
        internal static List<SocialLinkerCommand> MultiMaker_Session_List { get; set; } = new List<SocialLinkerCommand>();

        public static readonly Dictionary<string, string> Game_Titles = new Dictionary<string, string>
        {
            { "P1-PS1", "Revelations: Persona" },
            { "P1-PSP", "Persona (PSP®️)" },
            { "P2IS-PS1", "Persona 2: Innocent Sin (PlayStation®️)" },
            { "P2IS-PSP", "Persona 2: Innocent Sin (PSP®️)" },
            { "P2EP-PS1", "Persona 2: Eternal Punishment (PlayStation®️)" },
            { "P2EP-PSP", "Persona 2: Eternal Punishment (PSP®️)" },
            { "P3F", "Persona 3 FES" },
            { "P3P", "Persona 3 Portable" },
            { "P4-PS2", "Persona 4 (PlayStation®️ 2)" },
            { "P4G", "Persona 4 Golden" },
            { "P4AU", "Persona 4 Arena Ultimax" },
            { "P4D", "Persona 4: Dancing All Night" },
            { "P5-PS4", "Persona 5 (PlayStation®️ 4)" },
            { "P5R", "Persona 5 Royal" },
            { "P5S", "Persona 5 Strikers" },
            { "BBTAG", "BlazBlue: Cross Tag Battle" }
        };

        public static string GetGameTitle(string keyword) =>
        Game_Titles.TryGetValue(keyword, out var fullTitle)
            ? fullTitle
            : "Unknown Game";

        public static readonly Dictionary<string, string> Game_Emotes = new Dictionary<string, string>
        {
            { "P1", "<:P1:751133115531133112>" },
            { "P1-PS1", "<:P1:751133115531133112>" },
            { "P1-PSP", "<:P1:751133115531133112>" },
            { "P2IS", "<:P2IS:788950080396328990>" },
            { "P2IS-PS1", "<:P2IS:788950080396328990>" },
            { "P2IS-PSP", "<:P2IS:788950080396328990>" },
            { "P2EP", "<:P2EP:788950163363463172>" },
            { "P2EP-PS1", "<:P2EP:788950163363463172>" },
            { "P2EP-PSP", "<:P2EP:788950163363463172>" },
            { "P3", "<:P3:751133114918633483>" },
            { "P3F", "<:P3:751133114918633483>" },
            { "P3P", "<:P3P:1096338602046267392>" },
            { "P4", "<:P4:751133120530612274>" },
            { "P4-PS2", "<:P4:751133120530612274>" },
            { "P4G", "<:P4G:751133123479207956>" },
            { "P4AU", "<:P4AU:751133122342420572>" },
            { "P4D", "<:P4D:751133120346062859>" },
            { "P5", "<:P5:751133123861020742>" },
            { "P5-PS4", "<:P5:751133123861020742>" },
            { "P5R", "<:P5R:751133123617488937>" },
            { "P5S", "<:P5S:852644176188669972>" },
            { "BBTAG", "<:BBTAG:751133123013771617>" }
        };

        public static string GetGameEmote(string keyword) =>
        Game_Emotes.TryGetValue(keyword, out var fullEmote)
            ? fullEmote
            : "";

        internal static string[] p1_ps1_version_keywords = { "P1-PS1", "P1-PSX", "P1PS1", "P1PSX" };
        internal static string[] p1_psp_version_keywords = { "P1-PSP", "P1PSP", "P1-P", "P1P" };
        internal static string[] p2is_ps1_version_keywords = { "P2IS-PS1", "P2IS-PSX", "P2ISPS1", "P2ISPSX", "P2-PS1", "P2-PSX", "P2PS1", "P2PSX", "IS-PS1", "IS-PSX", "ISPS1", "ISPSX" };
        internal static string[] p2is_psp_version_keywords = { "P2IS-PSP", "P2ISPSP", "P2IS-P", "IS-PSP", "IS-P", "P2ISP", "ISPSP", "ISP" };
        internal static string[] p2ep_ps1_version_keywords = { "P2EP-PS1", "P2EP-PSX", "P2EPPS1", "P2EPPSX", "EP-PS1", "EP-PSX", "EPPS1", "EPPSX" };
        internal static string[] p2ep_psp_version_keywords = { "P2EP-PSP", "P2EPPSP", "P2EP-P", "EP-PSP", "EP-P", "P2EPP", "EPPSP", "EPP" };
        internal static string[] p3f_version_keywords = { "P3F", "FES", "P3FES", "P3-PS2", "P3F-PS2", "FES-PS2", "P3FES-PS2", "P3-FES", "P3PS2", "P3FPS2", "FESPS2", "P3FESPS2" };
        internal static string[] p3p_version_keywords = { "P3P", "P3-PSP", "P3PSP" };
        //internal static string[] p3r_version_keywords = { "P3R", "P3RE" };
        internal static string[] p4_ps2_version_keywords = { "P4-PS2", "P4PS2" };
        internal static string[] p4g_version_keywords = { "P4G" };
        internal static string[] p5_ps4_version_keywords = { "P5-PS3", "P5-PS4", "P5PS3", "P5PS4" };
        internal static string[] p5r_version_keywords = { "P5R", "P5R-PS4", "P5RPS4" };

        internal static string[] p1_generic_keywords = { "P1" };
        internal static string[] p2is_generic_keywords = { "P2IS", "P2", "IS" };
        internal static string[] p2ep_generic_keywords = { "P2EP", "EP" };
        internal static string[] p3_generic_keywords = { "P3" };
        internal static string[] p4_generic_keywords = { "P4" };
        internal static string[] p4au_generic_keywords = { "P4A", "P4AU", "P4U", "P4U2" };
        internal static string[] p4d_generic_keywords = { "P4D" };
        internal static string[] p5_generic_keywords = { "P5" };
        internal static string[] p5s_generic_keywords = { "P5S" };
        internal static string[] bbtag_generic_keywords = { "BBTAG" };

        internal static char[] p3r_poses = { 'a', 'b', 'c', 'd', 'p' };

        internal static int error_duration = 60000;
        internal static int API_Timeout = 5000;
        internal static int Max_PMedals = 999;
        internal static int Max_Level = 99;

        internal static string SlashCommandEmote = "<:SlashCommand:1032644966851281016>";
        internal static string MessageCommandEmote = "<:MessageCommand:1141804603906736271>";
        internal static string MentionNotice = ":information_source: **For Social Linker to read your message, prefix the message by mentioning the bot.**";

        public static string RemoveBotMention(string message_command)
        {
            string altered_command = "";

            List<string> listed_string;

            char[] delimiterChars = { ' ' };

            listed_string = message_command.Split(delimiterChars).ToList();

            listed_string.RemoveAt(0);

            altered_command = String_List_To_String(listed_string);

            return altered_command;
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
}
