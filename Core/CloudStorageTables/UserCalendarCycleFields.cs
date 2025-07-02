using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialLinker.Core.CloudStorageTables
{
    internal class UserCalendarCycleFields : TableEntity
    {
        public string Platform => PartitionKey;
        public string User_ID => RowKey;
        public string P1_PSX_Calendar_Cycle_Override { get; set; }
        public string P1_PSX_Calendar_Cycle_Moon_Phase { get; set; }
        public string P1_PSP_Calendar_Cycle_Override { get; set; }
        public string P1_PSP_Calendar_Cycle_Moon_Phase { get; set; }
        public string P3F_Calendar_Cycle_Override { get; set; }
        public string P3F_Calendar_Cycle_Moon_Phase { get; set; }
        public string P3P_Calendar_Cycle_Override { get; set; }
        public string P3R_Calendar_Cycle_Override { get; set; }
        public string P4_PS2_Calendar_Cycle_Override { get; set; }
        public string P4G_Calendar_Cycle_Override { get; set; }
        public string P5_PS3_Calendar_Cycle_Override { get; set; }
        public string P5R_Calendar_Cycle_Override { get; set; }
        public string P5R_Calendar_Cycle_Month { get; set; }
        public string P5R_Calendar_Cycle_Day { get; set; }
        public string P5R_Calendar_Cycle_Day_of_Week { get; set; }
        public string P5R_Calendar_Cycle_Time_of_Day { get; set; }
        public string P5S_Calendar_Cycle_Cycle_Override { get; set; }
        public string P5S_Calendar_Cycle_Month { get; set; }
        public string P5S_Calendar_Cycle_Day { get; set; }
        public string P5S_Calendar_Cycle_Day_of_Week { get; set; }
        public string P5S_Calendar_Cycle_Time_of_Day { get; set; }
    }
}
