using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.WebSocket;
using SocialLinker.Core.CloudStorageTables;
using SocialLinker.Core.Menus.Settings.Main.Profile;
using System.Collections.Generic;
using System.Reflection;

namespace SocialLinker.Core.Menus.Settings.Reactions.Profile
{
    class Content_Filter_Reactions
    {
        // Methods that activate on the ReactionAdded event.
        public static Task Nav_Content_Filter_Main_Added(SocketReaction reaction, MenuIdStructure menuSession)
        {
            // Search for a content filter list that corresponds to the user's ID.
            var filterSession = Global.ContentFilterList.SingleOrDefault(x => x.User.Id == reaction.UserId);

            if (reaction.Emote.Name == "↩️")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Profile_Settings_Menu.Profile_Settings_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }
            else if (reaction.Emote.Name == "P1")
            {
                // If this option is selected, change the value to true in the filter list.
                filterSession.P1_Select = true;
                return Task.CompletedTask;
            }
            else if (reaction.Emote.Name == "P2IS")
            {
                // If this option is selected, change the value to true in the filter list.
                filterSession.P2IS_Select = true;
                return Task.CompletedTask;
            }
            else if (reaction.Emote.Name == "P2EP")
            {
                // If this option is selected, change the value to true in the filter list.
                filterSession.P2EP_Select = true;
                return Task.CompletedTask;
            }
            else if (reaction.Emote.Name == "P3")
            {
                // If this option is selected, change the value to true in the filter list.
                filterSession.P3_Select = true;
                return Task.CompletedTask;
            }
            else if (reaction.Emote.Name == "P4")
            {
                // If this option is selected, change the value to true in the filter list.
                filterSession.P4_Select = true;
                return Task.CompletedTask;
            }
            else if (reaction.Emote.Name == "P4AU")
            {
                // If this option is selected, change the value to true in the filter list.
                filterSession.P4AU_Select = true;
                return Task.CompletedTask;
            }
            else if (reaction.Emote.Name == "P4D")
            {
                // If this option is selected, change the value to true in the filter list.
                filterSession.P4D_Select = true;
                return Task.CompletedTask;
            }
            else if (reaction.Emote.Name == "P5")
            {
                // If this option is selected, change the value to true in the filter list.
                filterSession.P5_Select = true;
                return Task.CompletedTask;
            }
            else if (reaction.Emote.Name == "P5S")
            {
                // If this option is selected, change the value to true in the filter list.
                filterSession.P5S_Select = true;
                return Task.CompletedTask;
            }
            else if (reaction.Emote.Name == "BBTAG")
            {
                // If this option is selected, change the value to true in the filter list.
                filterSession.BBTAG_Select = true;
                return Task.CompletedTask;
            }
            else if (reaction.Emote.Name == "✅")
            {
                filterSession.Menu_List = CreateMenuList(reaction);

                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Decide what menu to go to next.
                _ = VersionControlMenuDirectory(reaction, menuSession);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Content_Filter_VC_P1_Main_Added(SocketReaction reaction, MenuIdStructure menuSession)
        {
            // Search for a content filter list that corresponds to the user's ID.
            var filterSession = Global.ContentFilterList.SingleOrDefault(x => x.User.Id == reaction.UserId);

            if (reaction.Emote.Name == "↩️")
            {
                // Change the options on this menu to false.
                filterSession.P1_VC_PSX_Select = false;
                filterSession.P1_VC_PSP_Select = false;

                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Create a new instance of this class in order to call the non-static ReturnToPreviousMenu method.
                Content_Filter_Reactions content_nav = new Content_Filter_Reactions();

                // Call the ReturnToPreviousMenu method to return to the previous menu.
                content_nav.ReturnToPreviousMenu(reaction, menuSession);

                return Task.CompletedTask;
            }

            // Keycap One
            else if (reaction.Emote.Name == "\u0031\ufe0f\u20e3")
            {
                // If this option is selected, change the value to true in the filter list.
                filterSession.P1_VC_PSX_Select = true;
                return Task.CompletedTask;
            }
            // Keycap Two
            else if (reaction.Emote.Name == "\u0032\ufe0f\u20e3")
            {
                // If this option is selected, change the value to true in the filter list.
                filterSession.P1_VC_PSP_Select = true;
                return Task.CompletedTask;
            }
            else if (reaction.Emote.Name == "✅")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Decide which menu to go to next.
                _ = VersionControlMenuDirectory(reaction, menuSession);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Content_Filter_VC_P2IS_Main_Added(SocketReaction reaction, MenuIdStructure menuSession)
        {
            // Search for a content filter list that corresponds to the user's ID.
            var filterSession = Global.ContentFilterList.SingleOrDefault(x => x.User.Id == reaction.UserId);

            if (reaction.Emote.Name == "↩️")
            {
                // Change the options on this menu to false.
                filterSession.P2IS_VC_PSX_Select = false;
                filterSession.P2IS_VC_PSP_Select = false;

                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Create a new instance of this class in order to call the non-static ReturnToPreviousMenu method.
                Content_Filter_Reactions content_nav = new Content_Filter_Reactions();

                // Call the ReturnToPreviousMenu method to return to the previous menu.
                content_nav.ReturnToPreviousMenu(reaction, menuSession);

                return Task.CompletedTask;
            }

            // Keycap One
            else if (reaction.Emote.Name == "\u0031\ufe0f\u20e3")
            {
                // If this option is selected, change the value to true in the filter list.
                filterSession.P2IS_VC_PSX_Select = true;
                return Task.CompletedTask;
            }
            // Keycap Two
            else if (reaction.Emote.Name == "\u0032\ufe0f\u20e3")
            {
                // If this option is selected, change the value to true in the filter list.
                filterSession.P2IS_VC_PSP_Select = true;
                return Task.CompletedTask;
            }
            else if (reaction.Emote.Name == "✅")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Decide which menu to go to next.
                _ = VersionControlMenuDirectory(reaction, menuSession);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Content_Filter_VC_P2EP_Main_Added(SocketReaction reaction, MenuIdStructure menuSession)
        {
            // Search for a content filter list that corresponds to the user's ID.
            var filterSession = Global.ContentFilterList.SingleOrDefault(x => x.User.Id == reaction.UserId);

            if (reaction.Emote.Name == "↩️")
            {
                // Change the options on this menu to false.
                filterSession.P2EP_VC_PSX_Select = false;
                filterSession.P2EP_VC_PSP_Select = false;

                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Create a new instance of this class in order to call the non-static ReturnToPreviousMenu method.
                Content_Filter_Reactions content_nav = new Content_Filter_Reactions();

                // Call the ReturnToPreviousMenu method to return to the previous menu.
                content_nav.ReturnToPreviousMenu(reaction, menuSession);

                return Task.CompletedTask;
            }

            // Keycap One
            else if (reaction.Emote.Name == "\u0031\ufe0f\u20e3")
            {
                // If this option is selected, change the value to true in the filter list.
                filterSession.P2EP_VC_PSX_Select = true;
                return Task.CompletedTask;
            }
            // Keycap Two
            else if (reaction.Emote.Name == "\u0032\ufe0f\u20e3")
            {
                // If this option is selected, change the value to true in the filter list.
                filterSession.P2EP_VC_PSP_Select = true;
                return Task.CompletedTask;
            }
            else if (reaction.Emote.Name == "✅")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Decide which menu to go to next.
                _ = VersionControlMenuDirectory(reaction, menuSession);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Content_Filter_VC_P3_Main_Added(SocketReaction reaction, MenuIdStructure menuSession)
        {
            // Search for a content filter list that corresponds to the user's ID.
            var filterSession = Global.ContentFilterList.SingleOrDefault(x => x.User.Id == reaction.UserId);

            if (reaction.Emote.Name == "↩️")
            {
                // Change the options on this menu to false.
                filterSession.P3_VC_P3F_Select = false;
                filterSession.P3_VC_P3P_Select = false;

                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Create a new instance of this class in order to call the non-static ReturnToPreviousMenu method.
                Content_Filter_Reactions content_nav = new Content_Filter_Reactions();

                // Call the ReturnToPreviousMenu method to return to the previous menu.
                content_nav.ReturnToPreviousMenu(reaction, menuSession);

                return Task.CompletedTask;
            }

            // Keycap One
            else if (reaction.Emote.Name == "\u0031\ufe0f\u20e3")
            {
                // If this option is selected, change the value to true in the filter list.
                filterSession.P3_VC_P3F_Select = true;
                return Task.CompletedTask;
            }
            // Keycap Two
            else if (reaction.Emote.Name == "\u0032\ufe0f\u20e3")
            {
                // If this option is selected, change the value to true in the filter list.
                filterSession.P3_VC_P3P_Select = true;
                return Task.CompletedTask;
            }
            else if (reaction.Emote.Name == "✅")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Decide which menu to go to next.
                _ = VersionControlMenuDirectory(reaction, menuSession);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Content_Filter_VC_P4_Main_Added(SocketReaction reaction, MenuIdStructure menuSession)
        {
            // Search for a content filter list that corresponds to the user's ID.
            var filterSession = Global.ContentFilterList.SingleOrDefault(x => x.User.Id == reaction.UserId);

            if (reaction.Emote.Name == "↩️")
            {
                // Change the options on this menu to false.
                filterSession.P4_VC_PS2_Select = false;
                filterSession.P4_VC_P4G_Select = false;

                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Create a new instance of this class in order to call the non-static ReturnToPreviousMenu method.
                Content_Filter_Reactions content_nav = new Content_Filter_Reactions();

                // Call the ReturnToPreviousMenu method to return to the previous menu.
                content_nav.ReturnToPreviousMenu(reaction, menuSession);

                return Task.CompletedTask;
            }

            // Keycap One
            else if (reaction.Emote.Name == "\u0031\ufe0f\u20e3")
            {
                // If this option is selected, change the value to true in the filter list.
                filterSession.P4_VC_PS2_Select = true;
                return Task.CompletedTask;
            }
            // Keycap Two
            else if (reaction.Emote.Name == "\u0032\ufe0f\u20e3")
            {
                // If this option is selected, change the value to true in the filter list.
                filterSession.P4_VC_P4G_Select = true;
                return Task.CompletedTask;
            }
            else if (reaction.Emote.Name == "✅")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Decide which menu to go to next.
                _ = VersionControlMenuDirectory(reaction, menuSession);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Content_Filter_VC_P5_Main_Added(SocketReaction reaction, MenuIdStructure menuSession)
        {
            // Search for a content filter list that corresponds to the user's ID.
            var filterSession = Global.ContentFilterList.SingleOrDefault(x => x.User.Id == reaction.UserId);

            if (reaction.Emote.Name == "↩️")
            {
                // Change the options on this menu to false.
                filterSession.P5_VC_PS4_Select = false;
                filterSession.P5_VC_P5R_Select = false;

                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Create a new instance of this class in order to call the non-static ReturnToPreviousMenu method.
                Content_Filter_Reactions content_nav = new Content_Filter_Reactions();

                // Call the ReturnToPreviousMenu method to return to the previous menu.
                content_nav.ReturnToPreviousMenu(reaction, menuSession);

                return Task.CompletedTask;
            }

            // Keycap One
            else if (reaction.Emote.Name == "\u0031\ufe0f\u20e3")
            {
                // If this option is selected, change the value to true in the filter list.
                filterSession.P5_VC_PS4_Select = true;
                return Task.CompletedTask;
            }
            // Keycap Two
            else if (reaction.Emote.Name == "\u0032\ufe0f\u20e3")
            {
                // If this option is selected, change the value to true in the filter list.
                filterSession.P5_VC_P5R_Select = true;
                return Task.CompletedTask;
            }
            else if (reaction.Emote.Name == "✅")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Decide which menu to go to next.
                _ = VersionControlMenuDirectory(reaction, menuSession);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Content_Filter_Confirm(SocketReaction reaction, MenuIdStructure menuSession)
        {
            // Search for a content filter list that corresponds to the user's ID.
            var filterSession = Global.ContentFilterList.SingleOrDefault(x => x.User.Id == reaction.UserId);

            if (reaction.Emote.Name == "💠")
            {
                // Remove the content filter entry from the global list.
                Global.ContentFilterList.Remove(filterSession);

                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = Profile_Settings_Menu.Profile_Settings_Main(menuSession.User, menuSession.MenuMessage);
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

                // If the menu session is not null, remove it and the filter session from the global list.
                if (menuSession != null)
                {
                    Global.MenuIdList.Remove(menuSession);
                    Global.ContentFilterList.Remove(filterSession);
                }
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        // Methods that activate on the ReactionRemoved event.
        public static Task Nav_Content_Filter_Main_Removed(SocketReaction reaction, MenuIdStructure menuSession)
        {
            // Search for a content filter list that corresponds to the user's ID.
            var filterSession = Global.ContentFilterList.SingleOrDefault(x => x.User.Id == reaction.UserId);

            if (reaction.Emote.Name == "P1")
            {
                // If this option is selected, change the value to false in the filter list.
                filterSession.P1_Select = false;

                return Task.CompletedTask;
            }
            else if (reaction.Emote.Name == "P2IS")
            {
                // If this option is selected, change the value to false in the filter list.
                filterSession.P2IS_Select = false;

                return Task.CompletedTask;
            }
            else if (reaction.Emote.Name == "P2EP")
            {
                // If this option is selected, change the value to false in the filter list.
                filterSession.P2EP_Select = false;

                return Task.CompletedTask;
            }
            else if (reaction.Emote.Name == "P3")
            {
                // If this option is selected, change the value to false in the filter list.
                filterSession.P3_Select = false;

                return Task.CompletedTask;
            }
            else if (reaction.Emote.Name == "P4")
            {
                // If this option is selected, change the value to false in the filter list.
                filterSession.P4_Select = false;

                return Task.CompletedTask;
            }
            else if (reaction.Emote.Name == "P4AU")
            {
                // If this option is selected, change the value to false in the filter list.
                filterSession.P4AU_Select = false;

                return Task.CompletedTask;
            }
            else if (reaction.Emote.Name == "P4D")
            {
                // If this option is selected, change the value to false in the filter list.
                filterSession.P4D_Select = false;

                return Task.CompletedTask;
            }
            else if (reaction.Emote.Name == "P5")
            {
                // If this option is selected, change the value to false in the filter list.
                filterSession.P5_Select = false;

                return Task.CompletedTask;
            }
            else if (reaction.Emote.Name == "BBTAG")
            {
                // If this option is selected, change the value to false in the filter list.
                filterSession.BBTAG_Select = false;

                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Content_Filter_VC_P1_Main_Removed(SocketReaction reaction, MenuIdStructure menuSession)
        {
            // Search for a content filter list that corresponds to the user's ID.
            var filterSession = Global.ContentFilterList.SingleOrDefault(x => x.User.Id == reaction.UserId);

            // Keycap One
            if (reaction.Emote.Name == "\u0031\ufe0f\u20e3")
            {
                // If this option is selected, change the value to false in the filter list.
                filterSession.P1_VC_PSX_Select = false;
                return Task.CompletedTask;
            }
            // Keycap Two
            else if (reaction.Emote.Name == "\u0032\ufe0f\u20e3")
            {
                // If this option is selected, change the value to false in the filter list.
                filterSession.P1_VC_PSP_Select = false;
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Content_Filter_VC_P2IS_Main_Removed(SocketReaction reaction, MenuIdStructure menuSession)
        {
            // Search for a content filter list that corresponds to the user's ID.
            var filterSession = Global.ContentFilterList.SingleOrDefault(x => x.User.Id == reaction.UserId);

            // Keycap One
            if (reaction.Emote.Name == "\u0031\ufe0f\u20e3")
            {
                // If this option is selected, change the value to false in the filter list.
                filterSession.P2IS_VC_PSX_Select = false;
                return Task.CompletedTask;
            }
            // Keycap Two
            else if (reaction.Emote.Name == "\u0032\ufe0f\u20e3")
            {
                // If this option is selected, change the value to false in the filter list.
                filterSession.P2IS_VC_PSP_Select = false;
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Content_Filter_VC_P2EP_Main_Removed(SocketReaction reaction, MenuIdStructure menuSession)
        {
            // Search for a content filter list that corresponds to the user's ID.
            var filterSession = Global.ContentFilterList.SingleOrDefault(x => x.User.Id == reaction.UserId);

            // Keycap One
            if (reaction.Emote.Name == "\u0031\ufe0f\u20e3")
            {
                // If this option is selected, change the value to false in the filter list.
                filterSession.P2EP_VC_PSX_Select = false;
                return Task.CompletedTask;
            }
            // Keycap Two
            else if (reaction.Emote.Name == "\u0032\ufe0f\u20e3")
            {
                // If this option is selected, change the value to false in the filter list.
                filterSession.P2EP_VC_PSP_Select = false;
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Content_Filter_VC_P3_Main_Removed(SocketReaction reaction, MenuIdStructure menuSession)
        {
            // Search for a content filter list that corresponds to the user's ID.
            var filterSession = Global.ContentFilterList.SingleOrDefault(x => x.User.Id == reaction.UserId);

            // Keycap One
            if (reaction.Emote.Name == "\u0031\ufe0f\u20e3")
            {
                // If this option is selected, change the value to false in the filter list.
                filterSession.P3_VC_P3F_Select = false;
                return Task.CompletedTask;
            }
            // Keycap Two
            else if (reaction.Emote.Name == "\u0032\ufe0f\u20e3")
            {
                // If this option is selected, change the value to false in the filter list.
                filterSession.P3_VC_P3P_Select = false;
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Content_Filter_VC_P4_Main_Removed(SocketReaction reaction, MenuIdStructure menuSession)
        {
            // Search for a content filter list that corresponds to the user's ID.
            var filterSession = Global.ContentFilterList.SingleOrDefault(x => x.User.Id == reaction.UserId);

            // Keycap One
            if (reaction.Emote.Name == "\u0031\ufe0f\u20e3")
            {
                // If this option is selected, change the value to false in the filter list.
                filterSession.P4_VC_PS2_Select = false;
                return Task.CompletedTask;
            }
            // Keycap Two
            else if (reaction.Emote.Name == "\u0032\ufe0f\u20e3")
            {
                // If this option is selected, change the value to false in the filter list.
                filterSession.P4_VC_P4G_Select = false;
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        public static Task Nav_Content_Filter_VC_P5_Main_Removed(SocketReaction reaction, MenuIdStructure menuSession)
        {
            // Search for a content filter list that corresponds to the user's ID.
            var filterSession = Global.ContentFilterList.SingleOrDefault(x => x.User.Id == reaction.UserId);

            // Keycap One
            if (reaction.Emote.Name == "\u0031\ufe0f\u20e3")
            {
                // If this option is selected, change the value to false in the filter list.
                filterSession.P5_VC_PS4_Select = false;
                return Task.CompletedTask;
            }
            // Keycap Two
            else if (reaction.Emote.Name == "\u0032\ufe0f\u20e3")
            {
                // If this option is selected, change the value to false in the filter list.
                filterSession.P5_VC_P5R_Select = false;
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        // Methods that suppliment the functionality of the menus.
        public static Task VersionControlMenuDirectory(SocketReaction reaction, MenuIdStructure menuSession)
        {
            // Search for a content filter list that corresponds to the user's ID.
            var filterSession = Global.ContentFilterList.SingleOrDefault(x => x.User.Id == reaction.UserId);

            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(menuSession.User);

            //Check each field of titles with alternate versions to see if they have been chosen.
            // Persona
            if (filterSession.P1_Select == true && (filterSession.P1_VC_PSX_Select == false && filterSession.P1_VC_PSP_Select == false))
            {
                // Go to a new menu.
                _ = Content_Filter_Menu.Content_Filter_VC_P1_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }
            // Persona 2: Innocent Sin
            else if (filterSession.P2IS_Select == true && (filterSession.P2IS_VC_PSX_Select == false && filterSession.P2IS_VC_PSP_Select == false))
            {
                // Go to a new menu.
                _ = Content_Filter_Menu.Content_Filter_VC_P2IS_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }
            // Persona 2: Eternal Punishment
            else if (filterSession.P2EP_Select == true && (filterSession.P2EP_VC_PSX_Select == false && filterSession.P2EP_VC_PSP_Select == false))
            {
                // Go to a new menu.
                _ = Content_Filter_Menu.Content_Filter_VC_P2EP_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }
            // Persona 3
            else if (filterSession.P3_Select == true && (filterSession.P3_VC_P3F_Select == false && filterSession.P3_VC_P3P_Select == false))
            {
                // Go to a new menu.
                _ = Content_Filter_Menu.Content_Filter_VC_P3_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }
            // Persona 4
            else if (filterSession.P4_Select == true && (filterSession.P4_VC_PS2_Select == false && filterSession.P4_VC_P4G_Select == false))
            {
                // Go to a new menu.
                _ = Content_Filter_Menu.Content_Filter_VC_P4_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }
            // Persona 5
            else if (filterSession.P5_Select == true && (filterSession.P5_VC_PS4_Select == false && filterSession.P5_VC_P5R_Select == false))
            {
                // Go to a new menu.
                _ = Content_Filter_Menu.Content_Filter_VC_P5_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            // Now, assemble the user's content filter string.
            // Create an empty string variable that will represent the user's content filter string.
            string user_filter = "";

            // Based on the titles the user selected, construct a content filter string for them.
            // Revelations: Persona
            if (filterSession.P1_VC_PSX_Select == true)
            {
                user_filter += "P1-PS1;";
            }

            // Persona (Remake)
            if (filterSession.P1_VC_PSP_Select == true)
            {
                user_filter += "P1-PSP;";
            }

            // Persona 2: Innocent Sin (PlayStation)
            if (filterSession.P2IS_VC_PSX_Select == true)
            {
                user_filter += "P2IS-PS1;";
            }

            // Persona 2: Innocent Sin (Remake)
            if (filterSession.P2IS_VC_PSP_Select == true)
            {
                user_filter += "P2IS-PSP;";
            }

            // Persona 2: Eternal Punishment (PlayStation)
            if (filterSession.P2EP_VC_PSX_Select == true)
            {
                user_filter += "P2EP-PS1;";
            }

            // Persona 2: Eternal Punishment (Remake)
            if (filterSession.P2EP_VC_PSP_Select == true)
            {
                user_filter += "P2EP-PSP;";
            }

            // Persona 3 FES
            if (filterSession.P3_VC_P3F_Select == true)
            {
                user_filter += "P3F;";
            }

            // Persona 3 Portable
            if (filterSession.P3_VC_P3P_Select == true)
            {
                user_filter += "P3P;";
            }

            // Persona 4 (PlayStation 2)
            if (filterSession.P4_VC_PS2_Select == true)
            {
                user_filter += "P4-PS2;";
            }

            // Persona 4 Golden
            if (filterSession.P4_VC_P4G_Select == true)
            {
                user_filter += "P4G;";
            }

            // Persona 4 Arena Ultimax
            if (filterSession.P4AU_Select == true)
            {
                user_filter += "P4AU;";
            }

            // Persona 4: Dancing All Night
            if (filterSession.P4D_Select == true)
            {
                user_filter += "P4D;";
            }

            // Persona 5 (PlayStation 4)
            if (filterSession.P5_VC_PS4_Select == true)
            {
                user_filter += "P5-PS4;";
            }

            // Persona 5 Royal
            if (filterSession.P5_VC_P5R_Select == true)
            {
                user_filter += "P5R;";
            }

            // Persona 5 Strikers
            if (filterSession.P5S_Select == true)
            {
                user_filter += "P5S;";
            }

            // BlazBlue: Cross Tag Battle
            if (filterSession.BBTAG_Select == true)
            {
                user_filter += "BBTAG;";
            }

            // Assign the created content filter list to the user's account.
            account.Content_Filter = user_filter;

            // Next, check if the user completely filtered out any titles that match with their currently set profile theme. If so, set their profile theme to none.
            if ((account.Profile_Theme == "P3" && filterSession.P3_VC_P3F_Select == true && filterSession.P3_VC_P3P_Select == true) ||
                (account.Profile_Theme == "P4" && filterSession.P4_VC_PS2_Select == true && filterSession.P4_VC_P4G_Select == true) ||
                (account.Profile_Theme == "P5" && filterSession.P5_VC_PS4_Select == true && filterSession.P5_VC_P5R_Select == true))
            {
                account.Profile_Theme = "";
            }

            account = Change_Version_Control_Based_On_Content_Filter(account, user_filter);

            // Update the user's account with new data.
            UserInfoClasses.UpdateAccount(account);

            // Go to the confirmation menu.
            _ = Content_Filter_Menu.Content_Filter_Confirm(menuSession.User, menuSession.MenuMessage);

            return Task.CompletedTask;
        }

        public static UserInfoFields Change_Version_Control_Based_On_Content_Filter(UserInfoFields account, string user_filter)
        {
            List<string> user_filter_list = String_To_String_List(user_filter);

            if (account.VC_P1 == "P1-PS1" || account.VC_P1 == "P1-PSP")
            {
                if (user_filter_list.Contains("P1-PS1") && !user_filter_list.Contains("P1-PSP"))
                {
                    account.VC_P1 = "P1-PSP";
                }
                else if (!user_filter_list.Contains("P1-PS1") && user_filter_list.Contains("P1-PSP"))
                {
                    account.VC_P1 = "P1-PS1";
                }
                else if (user_filter_list.Contains("P1-PS1") && user_filter_list.Contains("P1-PSP"))
                {
                    // If both are in the content filter, do nothing and keep the settings the same. They will be blocked no matter what is currently set.
                }
                else
                {
                    // If neither are in the content filter, do nothing and keep the settings the same.
                }
            }

            if (account.VC_P2IS == "P2IS-PS1" || account.VC_P2IS == "P2IS-PSP")
            {
                if (user_filter_list.Contains("P2IS-PS1") && !user_filter_list.Contains("P2IS-PSP"))
                {
                    account.VC_P2IS = "P2IS-PSP";
                }
                else if (!user_filter_list.Contains("P2IS-PS1") && user_filter_list.Contains("P2IS-PSP"))
                {
                    account.VC_P2IS = "P2IS-PS1";
                }
                else if (user_filter_list.Contains("P2IS-PS1") && user_filter_list.Contains("P2IS-PSP"))
                {
                    // If both are in the content filter, do nothing and keep the settings the same. They will be blocked no matter what is currently set.
                }
                else
                {
                    // If neither are in the content filter, do nothing and keep the settings the same.
                }
            }

            if (account.VC_P2EP == "P2EP-PS1" || account.VC_P2EP == "P2EP-PSP")
            {
                if (user_filter_list.Contains("P2EP-PS1") && !user_filter_list.Contains("P2EP-PSP"))
                {
                    account.VC_P2EP = "P2EP-PSP";
                }
                else if (!user_filter_list.Contains("P2EP-PS1") && user_filter_list.Contains("P2EP-PSP"))
                {
                    account.VC_P2EP = "P2EP-PS1";
                }
                else if (user_filter_list.Contains("P2EP-PS1") && user_filter_list.Contains("P2EP-PSP"))
                {
                    // If both are in the content filter, do nothing and keep the settings the same. They will be blocked no matter what is currently set.
                }
                else
                {
                    // If neither are in the content filter, do nothing and keep the settings the same.
                }
            }

            if (account.VC_P3 == "P3F" || account.VC_P3 == "P3P")
            {
                if (user_filter_list.Contains("P3F") && !user_filter_list.Contains("P3P"))
                {
                    account.VC_P3 = "P3P";
                }
                else if (!user_filter_list.Contains("P3F") && user_filter_list.Contains("P3P"))
                {
                    account.VC_P3 = "P3F";
                }
                else if (user_filter_list.Contains("P3F") && user_filter_list.Contains("P3P"))
                {
                    // If both are in the content filter, do nothing and keep the settings the same. They will be blocked no matter what is currently set.
                }
                else
                {
                    // If neither are in the content filter, do nothing and keep the settings the same.
                }
            }

            if (account.VC_P4 == "P4-PS2" || account.VC_P4 == "P4G")
            {
                if (user_filter_list.Contains("P4-PS2") && !user_filter_list.Contains("P4G"))
                {
                    account.VC_P4 = "P4G";
                }
                else if (!user_filter_list.Contains("P4-PS2") && user_filter_list.Contains("P4G"))
                {
                    account.VC_P4 = "P4-PS2";
                }
                else if (user_filter_list.Contains("P4-PS2") && user_filter_list.Contains("P4G"))
                {
                    // If both are in the content filter, do nothing and keep the settings the same. They will be blocked no matter what is currently set.
                }
                else
                {
                    // If neither are in the content filter, do nothing and keep the settings the same.
                }
            }

            if (account.VC_P5 == "P5-PS4" || account.VC_P5 == "P5R")
            {
                if (user_filter_list.Contains("P5-PS4") && !user_filter_list.Contains("P5R"))
                {
                    account.VC_P5 = "P5R";
                }
                else if (!user_filter_list.Contains("P5-PS4") && user_filter_list.Contains("P5R"))
                {
                    account.VC_P5 = "P5-PS4";
                }
                else if (user_filter_list.Contains("P5-PS4") && user_filter_list.Contains("P5R"))
                {
                    // If both are in the content filter, do nothing and keep the settings the same. They will be blocked no matter what is currently set.
                }
                else
                {
                    // If neither are in the content filter, do nothing and keep the settings the same.
                }
            }

            return account;
        }

        public void ReturnToPreviousMenu(SocketReaction reaction, MenuIdStructure menuSession)
        {
            // Search for a content filter list that corresponds to the user's ID.
            var filterSession = Global.ContentFilterList.SingleOrDefault(x => x.User.Id == reaction.UserId);

            // Use the session's menu list to find the index of the current menu.
            int index = filterSession.Menu_List.IndexOf(menuSession.CurrentMenu);

            try
            {
                // Assign the namespace of the content filter menu methods as a string.
                Type type = Type.GetType($"SocialLinker.Core.Menus.Settings.Main.General.Content_Filter_Menu");

                // Specify the method of whatever menu class is chosen to invoke.
                // Since we want to backtrack through a menu, the menu keyword will be in the index before the current one.
                MethodInfo methodInfo = type.GetMethod($"{filterSession.Menu_List[index - 1]}");

                // Store the typical parameters for a menu method within an object array.
                object[] parametersArray = new object[] { menuSession.User, menuSession.MenuMessage };

                // Call the method to jump to the previous menu.
                methodInfo.Invoke(this, parametersArray);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return;
            }
        }

        public static List<string> CreateMenuList(SocketReaction reaction)
        {
            // Search for a content filter list that corresponds to the user's ID.
            var filterSession = Global.ContentFilterList.SingleOrDefault(x => x.User.Id == reaction.UserId);

            // Create an empty string list variable to store the menu list in.
            List<string> menu_list = new List<string>();

            // As the first list item, add the keyword for the first message in the menu.
            menu_list.Add("Content_Filter_Main");

            // Depending on the user's choices in the first menu message, form a string list of menu keywords they will go through.
            // Persona
            if (filterSession.P1_Select == true)
            {
                menu_list.Add("Content_Filter_VC_P1_Main");
            }

            // Persona 2: Innocent Sin
            if (filterSession.P2IS_Select == true)
            {
                menu_list.Add("Content_Filter_VC_P2IS_Main");
            }

            // Persona 2: Eternal Punishment
            if (filterSession.P2EP_Select == true)
            {
                menu_list.Add("Content_Filter_VC_P2EP_Main");
            }

            // Persona 3
            if (filterSession.P3_Select == true)
            {
                menu_list.Add("Content_Filter_VC_P3_Main");
            }

            // Persona 4
            if (filterSession.P4_Select == true)
            {
                menu_list.Add("Content_Filter_VC_P4_Main");
            }

            // Persona 5
            if (filterSession.P5_Select == true)
            {
                menu_list.Add("Content_Filter_VC_P5_Main");
            }

            // Add the last possible menu message to the string list.
            menu_list.Add("Content_Filter_Confirm");

            return menu_list;
        }

        public static List<string> String_To_String_List(string input_string)
        {
            char[] delimiterChars = { ';' };
            List<string> string_list = input_string.Split(delimiterChars).ToList();
            string_list.RemoveAll(x => x.Length == 0); // Get rid of empty spaces in created list
            return string_list;
        }
    }
}