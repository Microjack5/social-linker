using System.Collections.Generic;
using System.Linq;
using Discord.WebSocket;
using SocialLinker.Core.CloudStorageTables;

namespace SocialLinker.Core.Menus
{
    public class ContentFilter
    {
        public SocketGuildUser User { get; set; }
        public List<string> Menu_List { get; set; }
        public bool P1_Select { get; set; }
        public bool P1_VC_PSX_Select { get; set; }
        public bool P1_VC_PSP_Select { get; set; }
        public bool P2IS_Select { get; set; }
        public bool P2IS_VC_PSX_Select { get; set; }
        public bool P2IS_VC_PSP_Select { get; set; }
        public bool P2EP_Select { get; set; }
        public bool P2EP_VC_PSX_Select { get; set; }
        public bool P2EP_VC_PSP_Select { get; set; }
        public bool P3_Select { get; set; }
        public bool P3_VC_P3F_Select { get; set; }
        public bool P3_VC_P3P_Select { get; set; }
        public bool P4_Select { get; set; }
        public bool P4_VC_PS2_Select { get; set; }
        public bool P4_VC_P4G_Select { get; set; }
        public bool P4AU_Select { get; set; }
        public bool P4D_Select { get; set; }
        public bool P5_Select { get; set; }
        public bool P5_VC_PS4_Select { get; set; }
        public bool P5_VC_P5R_Select { get; set; }
        public bool BBTAG_Select { get; set; }
        public bool P5S_Select { get; set; }
    }

    public class ContentFilterMethods
    {
        public static List<string> ParseContentFilter(UserInfoFields account)
        {
            //Create a list variable to return
            List<string> input_substring;

            //Specify the characters to divide the incoming string by
            char[] delimiterChars = { ';' };

            //Assign the return value to the input account's content filter string with its entries split into a list
            input_substring = account.Content_Filter.Split(delimiterChars).ToList();

            // There may be times when an empty string is parsed into the list, so make sure they are removed before returning.
            input_substring.RemoveAll(s => s == "");

            return input_substring;
        }

        public static List<string> AcronymToTitle(List<string> filter_list)
        {
            //Create a list variable to return
            List<string> title_list = new List<string>();

            if (filter_list.Contains("P1-PS1"))
            {
                title_list.Add("Revelations: Persona");
            }

            if (filter_list.Contains("P1-PSP"))
            {
                title_list.Add("Persona (PSP™)");
            }

            if (filter_list.Contains("P2IS-PS1"))
            {
                title_list.Add("Persona 2: Innocent Sin (PlayStation®️)");
            }

            if (filter_list.Contains("P2IS-PSP"))
            {
                title_list.Add("Persona 2: Innocent Sin (PSP™)");
            }

            if (filter_list.Contains("P2EP-PS1"))
            {
                title_list.Add("Persona 2: Eternal Punishment (PlayStation®️)");
            }

            if (filter_list.Contains("P2EP-PSP"))
            {
                title_list.Add("Persona 2: Eternal Punishment (PSP™)");
            }

            if (filter_list.Contains("P3F"))
            {
                title_list.Add("Persona 3 FES");
            }

            if (filter_list.Contains("P3P"))
            {
                title_list.Add("Persona 3 Portable");
            }

            if (filter_list.Contains("P4-PS2"))
            {
                title_list.Add("Persona 4 (PlayStation®️ 2)");
            }

            if (filter_list.Contains("P4G"))
            {
                title_list.Add("Persona 4 Golden");
            }

            if (filter_list.Contains("P4AU"))
            {
                title_list.Add("Persona 4 Arena Ultimax");
            }

            if (filter_list.Contains("P4D"))
            {
                title_list.Add("Persona 4: Dancing All Night");
            }

            if (filter_list.Contains("P5-PS4"))
            {
                title_list.Add("Persona 5 (PlayStation®️ 4)");
            }

            if (filter_list.Contains("P5R"))
            {
                title_list.Add("Persona 5 Royal");
            }

            if (filter_list.Contains("BBTAG"))
            {
                title_list.Add("BlazBlue: Cross Tag Battle");
            }

            if (filter_list.Contains("P5S"))
            {
                title_list.Add("Persona 5 Strikers");
            }

            return title_list;
        }
    }
}
