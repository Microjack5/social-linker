using Discord.WebSocket;
using SocialLinker.Core.CloudStorageTables;
using SocialLinker.Core.Menus.Settings.Main.SceneMaker.DisplayNames;
using SocialLinker.Core.Menus.Shop.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialLinker.Core.Menus.Settings.Reactions.SceneMaker.DisplayNames
{
    internal class Display_Names_Sort_Reactions
    {
        public static Task Nav_Display_Names_Sort(SocketReaction reaction, MenuIdStructure menuSession)
        {
            if (reaction.Emote.Name == "↩️")
            {
                // Stop the timeout timer associated with the menu
                menuSession.MenuTimer.Stop();

                // Go to a new menu
                _ = Display_Names_Sort_Menu.Display_Names_Sort_Confirm(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }
            else if (reaction.Emote.Name == "\u0031\ufe0f\u20e3") // Keycap one
            {
                var account = UserInfoClasses.GetAccount(menuSession.User);

                account.Display_Names_Sort = "entry_old_new";

                UserInfoClasses.UpdateAccount(account);

                menuSession.MenuTimer.Stop();

                _ = Display_Names_Sort_Menu.Display_Names_Sort_Confirm(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }
            else if (reaction.Emote.Name == "\u0032\ufe0f\u20e3") // Keycap two
            {
                var account = UserInfoClasses.GetAccount(menuSession.User);

                account.Display_Names_Sort = "entry_new_old";

                UserInfoClasses.UpdateAccount(account);

                menuSession.MenuTimer.Stop();

                _ = Display_Names_Sort_Menu.Display_Names_Sort_Confirm(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }
            else if (reaction.Emote.Name == "\u0033\ufe0f\u20e3") // Keycap three
            {
                var account = UserInfoClasses.GetAccount(menuSession.User);

                account.Display_Names_Sort = "name_a_z";

                UserInfoClasses.UpdateAccount(account);

                menuSession.MenuTimer.Stop();

                _ = Display_Names_Sort_Menu.Display_Names_Sort_Confirm(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }
            else if (reaction.Emote.Name == "\u0034\ufe0f\u20e3") // Keycap four
            {
                var account = UserInfoClasses.GetAccount(menuSession.User);

                account.Display_Names_Sort = "name_z_a";

                UserInfoClasses.UpdateAccount(account);

                menuSession.MenuTimer.Stop();

                _ = Display_Names_Sort_Menu.Display_Names_Sort_Confirm(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }
            else if (reaction.Emote.Name == "\u0035\ufe0f\u20e3") // Keycap five
            {
                var account = UserInfoClasses.GetAccount(menuSession.User);

                account.Display_Names_Sort = "by_title";

                UserInfoClasses.UpdateAccount(account);

                menuSession.MenuTimer.Stop();

                _ = Display_Names_Sort_Menu.Display_Names_Sort_Confirm(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Display_Names_Sort_Confirm(SocketReaction reaction, MenuIdStructure menuSession)
        {
            if (reaction.Emote.Name == "💠")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Display_Names_Menu.Display_Names_Start(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            else if (reaction.Emote.Name == "❌")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Attempt to delete the menu message from the channel if it hasn't been deleted by the user yet. If this fails, catch the exception.
                try
                {
                    _ = menuSession.MenuMessage.DeleteAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                // If the menu session is not null, remove it from the global list.
                if (menuSession != null)
                {
                    Global.MenuIdList.Remove(menuSession);
                }
                return Task.CompletedTask;
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
