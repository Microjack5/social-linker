using Discord.WebSocket;
using SocialLinker.Core.CloudStorageTables;
using SocialLinker.Core.Menus.Settings.Main.SceneMaker.DisplayNames;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialLinker.Core.Menus.Settings.Reactions.SceneMaker.DisplayNames
{
    internal class Display_Names_Sort_Reactions
    {
        public static Task Nav_Display_Names_Sort(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            var account = menuSession.Account;

            switch (component.Data.CustomId)
            {
                case "return":
                    _ = Display_Names_Sort_Menu.Display_Names_Sort_Confirm(menuSession);
                    break;
            }

            string selected = component.Data.Values.First();

            switch (selected)
            {
                case "1":
                    account.Display_Names_Sort = "entry_old_new";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Display_Names_Sort_Menu.Display_Names_Sort_Confirm(menuSession);
                    break;

                case "2":
                    account.Display_Names_Sort = "entry_new_old";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Display_Names_Sort_Menu.Display_Names_Sort_Confirm(menuSession);
                    break;

                case "3":
                    account.Display_Names_Sort = "name_a_z";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Display_Names_Sort_Menu.Display_Names_Sort_Confirm(menuSession);
                    break;

                case "4":
                    account.Display_Names_Sort = "name_z_a";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Display_Names_Sort_Menu.Display_Names_Sort_Confirm(menuSession);
                    break;

                case "5":
                    account.Display_Names_Sort = "by_title";
                    UserInfoClasses.UpdateAccount(account);

                    menuSession.Account = account;
                    _ = Display_Names_Sort_Menu.Display_Names_Sort_Confirm(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Display_Names_Sort_Confirm(SocketMessageComponent component, MenuIdStructure menuSession)
        {
            switch (component.Data.CustomId)
            {
                case "display-names":
                    _ = Display_Names_Menu.Display_Names_Start(menuSession);
                    break;
            }

            return Task.CompletedTask;
        }

        // Utility
        public static string SortSettingToString(string user_setting)
        {
            switch (user_setting)
            {
                case "entry_old_new":
                    return "By Oldest to Newest";

                case "entry_new_old":
                    return "By Newest to Oldest";

                case "name_a_z":
                    return "By Display Name (A - Z)";

                case "name_z_a":
                    return "By Display Name (Z - A)";

                case "by_title":
                    return "By Title";

                default:
                    return "By Newest to Oldest";
            }
        }

        public static List<DisplayNameTableData> CreateSortSettingList(List<DisplayNameTableData> display_name_list, string user_setting)
        {
            switch (user_setting)
            {
                case "entry_old_new":
                    return Display_Names_By_Entry_Old_New(display_name_list);

                case "entry_new_old":
                    return Display_Names_By_Entry_New_Old(display_name_list);

                case "name_a_z":
                    return Display_Names_By_Name_A_Z(display_name_list);

                case "name_z_a":
                    return Display_Names_By_Name_Z_A(display_name_list);

                case "by_title":
                    return Display_Names_By_Title(display_name_list);

                default:
                    return Display_Names_By_Entry_New_Old(display_name_list);
            }
        }

        public static List<DisplayNameTableData> Display_Names_By_Entry_Old_New(List<DisplayNameTableData> display_name_list)
        {
            List<DisplayNameTableData> return_list = new List<DisplayNameTableData>();

            foreach (DisplayNameTableData s in display_name_list)
            {
                return_list.Add(s);
            }

            return_list = return_list.OrderBy(s => s.Timestamp).ToList();

            return return_list;
        }

        public static List<DisplayNameTableData> Display_Names_By_Entry_New_Old(List<DisplayNameTableData> display_name_list)
        {
            List<DisplayNameTableData> return_list = new List<DisplayNameTableData>();

            foreach (DisplayNameTableData s in display_name_list)
            {
                return_list.Add(s);
            }

            return_list = return_list.OrderByDescending(s => s.Timestamp).ToList();

            return return_list;
        }

        public static List<DisplayNameTableData> Display_Names_By_Name_A_Z(List<DisplayNameTableData> display_name_list)
        {
            List<DisplayNameTableData> return_list = new List<DisplayNameTableData>();

            foreach (DisplayNameTableData s in display_name_list)
            {
                return_list.Add(s);
            }

            return_list = return_list.OrderBy(s => s.Display_Name).ToList();

            return return_list;
        }

        public static List<DisplayNameTableData> Display_Names_By_Name_Z_A(List<DisplayNameTableData> display_name_list)
        {
            List<DisplayNameTableData> return_list = new List<DisplayNameTableData>();

            foreach (DisplayNameTableData s in display_name_list)
            {
                return_list.Add(s);
            }

            return_list = return_list.OrderByDescending(s => s.Display_Name).ToList();

            return return_list;
        }

        public static List<DisplayNameTableData> Display_Names_By_Title(List<DisplayNameTableData> display_name_list)
        {
            List<DisplayNameTableData> return_list = new List<DisplayNameTableData>();

            foreach (DisplayNameTableData s in display_name_list)
            {
                return_list.Add(s);
            }

            return_list = return_list.OrderBy(s => s.RowKey).ToList();

            return return_list;
        }
    }
}
