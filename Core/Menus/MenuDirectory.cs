using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using SocialLinker.Config;
using SocialLinker.Core.Menus.Help.Reactions;
using SocialLinker.Core.Menus.Shop.Reactions;
using SocialLinker.Core.Menus.InitialUsage.Reactions;
using SocialLinker.Core.Menus.Settings.Reactions;
using SocialLinker.Core.Menus.Settings.Reactions.Profile;
using SocialLinker.Core.Menus.Settings.Reactions.SceneMaker;
using SocialLinker.Core.Menus.Settings.Reactions.SceneMaker.TemplateLayout;
using SocialLinker.Core.Menus.Settings.Reactions.SceneMaker.SpriteSheetOrder;
using SocialLinker.Core.Menus.Settings.Reactions.SceneMaker.ResolutionScaling;
using SocialLinker.Core.Menus.Settings.Reactions.SceneMaker.Backgrounds;
using SocialLinker.Core.Menus.Settings.Reactions.SceneMaker.AutoDelete;
using SocialLinker.Core.Menus.Settings.Reactions.SceneMaker.DisplayNames;
using System;
using Discord.Commands;
using SocialLinker.Core.Menus.MakerMulti.Reactions;
using SocialLinker.Core.Menus.MakerMulti.Main;
using SocialLinker.Core.Menus.Settings.Reactions.SceneMaker.CalendarCycles;

namespace SocialLinker.Core.Menus
{
    class MenuDirectory
    {
        public static async Task ReactionAddedIndex(Cacheable<IUserMessage, ulong> cache, Cacheable<IMessageChannel, ulong> channel, SocketReaction reaction)
        {
            try
            {
                // Create a variable for the user that reacted to the message by storing their ID
                var reactedUser = reaction.UserId;

                // Search the global Menu ID List by grabbing any entry that matches the message ID of the given reaction.
                if (Global.MenuIdList.Any(x => x.MenuMessage.Id == reaction.MessageId))
                {
                    // Once a matching entry is found, store it in a variable.
                    var menuSession = Global.MenuIdList.SingleOrDefault(x => x.MenuMessage.Id == reaction.MessageId);

                    // If the reactor is the bot itself, don't do anything.
                    if (reaction.UserId == BotConfig.bot.id)
                    {
                        // Don't do anything
                    }
                    // Else, if the reactor is the menu user, perform an action.
                    else if (reaction.UserId == menuSession.User.Id)
                    {
                        // Next, let's check all the users who reacted with this emote before we perform any actions.
                        // We want to make sure the reaction added is an emote that the bot has also reacted to, or else we want to ignore it to prevent errors.
                        // First, get a list of all users who reacted with the emote. Set the limit to a value of 3 (only two should be here max).
                        var all_reacted_users = await menuSession.MenuMessage.GetReactionUsersAsync(reaction.Emote, 3).FlattenAsync();

                        // Next, send the list to a method that checks if the bot is among the reacted users to the emote. If not, remove the emote and return.
                        if (ReactionCheck(all_reacted_users) == true)
                        {
                            // Do nothing
                        }
                        else if (ReactionCheck(all_reacted_users) == false)
                        {
                            await menuSession.MenuMessage.RemoveReactionAsync(reaction.Emote, reactedUser);
                            return;
                        }

                        // Ensure that the current menu matches a certain state before proceeding.
                        switch (menuSession.CurrentMenu)
                        {
                            // Namespace: SocialLinker.Core.Menus.InitialUsage
                            case "Set_First_Theme_Main":
                                await SetFirstTheme_Reactions.Nav_SetFirstThemeMain(reaction, menuSession);
                                break;

                            case "Set_First_Theme_Confirm":
                                await SetFirstTheme_Reactions.Nav_SetFirstThemeConfirm(reaction, menuSession);
                                break;

                            case "First_Use_Content_Filter_Main":
                                await First_Use_Content_Filter_Reactions.Nav_First_Use_Content_Filter_Main_Added(reaction, menuSession);
                                break;

                            case "First_Use_Content_Filter_VC_P1_Main":
                                await First_Use_Content_Filter_Reactions.Nav_First_Use_Content_Filter_VC_P1_Main_Added(reaction, menuSession);
                                break;

                            case "First_Use_Content_Filter_VC_P2IS_Main":
                                await First_Use_Content_Filter_Reactions.Nav_First_Use_Content_Filter_VC_P2IS_Main_Added(reaction, menuSession);
                                break;

                            case "First_Use_Content_Filter_VC_P2EP_Main":
                                await First_Use_Content_Filter_Reactions.Nav_First_Use_Content_Filter_VC_P2EP_Main_Added(reaction, menuSession);
                                break;

                            case "First_Use_Content_Filter_VC_P3_Main":
                                await First_Use_Content_Filter_Reactions.Nav_First_Use_Content_Filter_VC_P3_Main_Added(reaction, menuSession);
                                break;

                            case "First_Use_Content_Filter_VC_P4_Main":
                                await First_Use_Content_Filter_Reactions.Nav_First_Use_Content_Filter_VC_P4_Main_Added(reaction, menuSession);
                                break;

                            case "First_Use_Content_Filter_VC_P5_Main":
                                await First_Use_Content_Filter_Reactions.Nav_First_Use_Content_Filter_VC_P5_Main_Added(reaction, menuSession);
                                break;

                            case "First_Use_Content_Filter_Confirm":
                                await First_Use_Content_Filter_Reactions.Nav_First_Use_Content_Filter_Confirm(reaction, menuSession);
                                break;

                            // Namespace: SocialLinker.Core.Menus.Help
                            case "Help_Main_Menu":
                                await Help_Reactions.Nav_Help_Main_Menu(reaction, menuSession);
                                break;

                            case "Legal_Notices_Main":
                                await Legal_Notices_Reactions.Nav_Legal_Notices_Main(reaction, menuSession);
                                break;

                            case "Credits_Page_1":
                                await Credits_Reactions.Nav_Credits_Page_1(reaction, menuSession);
                                break;

                            case "Credits_Page_2":
                                await Credits_Reactions.Nav_Credits_Page_2(reaction, menuSession);
                                break;

                            case "Status_Tutorial_Page_1":
                                await Status_Tutorial_Reactions.Nav_Status_Tutorial_Page_1(reaction, menuSession);
                                break;

                            case "Status_Tutorial_Page_2":
                                await Status_Tutorial_Reactions.Nav_Status_Tutorial_Page_2(reaction, menuSession);
                                break;

                            case "Status_Tutorial_Page_3":
                                await Status_Tutorial_Reactions.Nav_Status_Tutorial_Page_3(reaction, menuSession);
                                break;

                            case "Status_Tutorial_Page_4":
                                await Status_Tutorial_Reactions.Nav_Status_Tutorial_Page_4(reaction, menuSession);
                                break;

                            case "Status_Tutorial_Page_5":
                                await Status_Tutorial_Reactions.Nav_Status_Tutorial_Page_5(reaction, menuSession);
                                break;

                            case "SM_Tutorial_Select_Main":
                                await SM_Tutorial_Select_Reactions.Nav_SM_Tutorial_Select_Main(reaction, menuSession);
                                break;

                            case "SM_Tutorial_Basics_Page_1":
                                await SM_Tutorial_Basics_Reactions.Nav_SM_Tutorial_Basics_Page_1(reaction, menuSession);
                                break;

                            case "SM_Tutorial_Basics_Page_2":
                                await SM_Tutorial_Basics_Reactions.Nav_SM_Tutorial_Basics_Page_2(reaction, menuSession);
                                break;

                            case "SM_Tutorial_Basics_Page_3":
                                await SM_Tutorial_Basics_Reactions.Nav_SM_Tutorial_Basics_Page_3(reaction, menuSession);
                                break;

                            case "SM_Tutorial_Basics_Page_4":
                                await SM_Tutorial_Basics_Reactions.Nav_SM_Tutorial_Basics_Page_4(reaction, menuSession);
                                break;

                            case "SM_Tutorial_Basics_Page_5":
                                await SM_Tutorial_Basics_Reactions.Nav_SM_Tutorial_Basics_Page_5(reaction, menuSession);
                                break;

                            case "SM_Tutorial_Basics_Page_6":
                                await SM_Tutorial_Basics_Reactions.Nav_SM_Tutorial_Basics_Page_6(reaction, menuSession);
                                break;

                            case "SM_Tutorial_Select_Advanced":
                                await SM_Tutorial_Select_Reactions.Nav_SM_Tutorial_Select_Advanced(reaction, menuSession);
                                break;

                            case "SM_Tutorial_VC_Main":
                                await SM_Tutorial_VC_Reactions.Nav_SM_Tutorial_VC_Main(reaction, menuSession);
                                break;

                            case "SM_Tutorial_VC_Basic_Page_1":
                                await SM_Tutorial_VC_Reactions.Nav_SM_Tutorial_VC_Basic_Page_1(reaction, menuSession);
                                break;

                            case "SM_Tutorial_VC_Basic_Page_2":
                                await SM_Tutorial_VC_Reactions.Nav_SM_Tutorial_VC_Basic_Page_2(reaction, menuSession);
                                break;

                            case "SM_Tutorial_VC_Auto_Page_1":
                                await SM_Tutorial_VC_Reactions.Nav_SM_Tutorial_VC_Auto_Page_1(reaction, menuSession);
                                break;

                            case "SM_Tutorial_VC_Auto_Page_2":
                                await SM_Tutorial_VC_Reactions.Nav_SM_Tutorial_VC_Auto_Page_2(reaction, menuSession);
                                break;

                            case "SM_Tutorial_VC_Bypass_Page_1":
                                await SM_Tutorial_VC_Reactions.Nav_SM_Tutorial_VC_Bypass_Page_1(reaction, menuSession);
                                break;

                            case "SM_Tutorial_VC_Bypass_Page_2":
                                await SM_Tutorial_VC_Reactions.Nav_SM_Tutorial_VC_Bypass_Page_2(reaction, menuSession);
                                break;

                            case "SM_Tutorial_VC_Cross_Page_1":
                                await SM_Tutorial_VC_Reactions.Nav_SM_Tutorial_VC_Cross_Page_1(reaction, menuSession);
                                break;

                            case "SM_Tutorial_VC_Cross_Page_2":
                                await SM_Tutorial_VC_Reactions.Nav_SM_Tutorial_VC_Cross_Page_2(reaction, menuSession);
                                break;

                            case "SM_Tutorial_VC_Cross_Page_3":
                                await SM_Tutorial_VC_Reactions.Nav_SM_Tutorial_VC_Cross_Page_3(reaction, menuSession);
                                break;

                            case "SM_Tutorial_Spriteless_Main":
                                await SM_Tutorial_Spriteless_Reactions.Nav_SM_Tutorial_Spriteless_Main(reaction, menuSession);
                                break;

                            case "SM_Tutorial_Spriteless_Chara_Page_1":
                                await SM_Tutorial_Spriteless_Reactions.Nav_SM_Tutorial_Spriteless_Chara_Page_1(reaction, menuSession);
                                break;

                            case "SM_Tutorial_Spriteless_Chara_Page_2":
                                await SM_Tutorial_Spriteless_Reactions.Nav_SM_Tutorial_Spriteless_Chara_Page_2(reaction, menuSession);
                                break;

                            case "SM_Tutorial_Spriteless_System_Page_1":
                                await SM_Tutorial_Spriteless_Reactions.Nav_SM_Tutorial_Spriteless_System_Page_1(reaction, menuSession);
                                break;

                            case "SM_Tutorial_Spriteless_System_Page_2":
                                await SM_Tutorial_Spriteless_Reactions.Nav_SM_Tutorial_Spriteless_System_Page_2(reaction, menuSession);
                                break;

                            case "SM_Tutorial_Dual_Chara_Page_1":
                                await SM_Tutorial_Dual_Reactions.Nav_SM_Tutorial_Dual_Chara_Page_1(reaction, menuSession);
                                break;

                            case "SM_Tutorial_Dual_Chara_Page_2":
                                await SM_Tutorial_Dual_Reactions.Nav_SM_Tutorial_Dual_Chara_Page_2(reaction, menuSession);
                                break;

                            case "SM_Tutorial_Anime_Frames_Page_1":
                                await SM_Tutorial_Anime_Frames_Reactions.Nav_SM_Tutorial_Anime_Frames_Page_1(reaction, menuSession);
                                break;

                            case "SM_Tutorial_Anime_Frames_Page_2":
                                await SM_Tutorial_Anime_Frames_Reactions.Nav_SM_Tutorial_Anime_Frames_Page_2(reaction, menuSession);
                                break;

                            case "SM_Tutorial_Anime_Frames_Page_3":
                                await SM_Tutorial_Anime_Frames_Reactions.Nav_SM_Tutorial_Anime_Frames_Page_3(reaction, menuSession);
                                break;

                            case "SM_Tutorial_Line_Breaks_Page_1":
                                await SM_Tutorial_Line_Breaks_Reactions.Nav_SM_Tutorial_Line_Breaks_Page_1(reaction, menuSession);
                                break;

                            case "SM_Tutorial_Cutin_Page_1":
                                await SM_Tutorial_Cutin_Reactions.Nav_SM_Tutorial_Cutin_Page_1(reaction, menuSession);
                                break;

                            case "SM_Tutorial_Cutin_Page_2":
                                await SM_Tutorial_Cutin_Reactions.Nav_SM_Tutorial_Cutin_Page_2(reaction, menuSession);
                                break;

                            case "SM_Tutorial_Cutin_Page_3":
                                await SM_Tutorial_Cutin_Reactions.Nav_SM_Tutorial_Cutin_Page_3(reaction, menuSession);
                                break;

                            case "SM_Tutorial_Cutin_Page_4":
                                await SM_Tutorial_Cutin_Reactions.Nav_SM_Tutorial_Cutin_Page_4(reaction, menuSession);
                                break;

                            // Namespace: SocialLinker.Core.Menus.Shop
                            case "Shop_Main_Menu":
                                await ShopReactions.Nav_ShopMainMenu(reaction, menuSession);
                                break;

                            case "Shop_Decor_Preview":
                                await ShopReactions.Nav_ShopDecorPreview(reaction, menuSession);
                                break;

                            case "Shop_Decor_Purchased":
                                await ShopReactions.Nav_ShopDecorPurchased(reaction, menuSession);
                                break;

                            case "Shop_Decor_Purchase_Set":
                                await ShopReactions.Nav_ShopDecorPurchaseSet(reaction, menuSession);
                                break;

                            case "Shop_Decor_Purchase_Not_Set":
                                await ShopReactions.Nav_ShopDecorPurchaseNotSet(reaction, menuSession);
                                break;

                            case "Shop_Sort":
                                await ShopReactions.Nav_ShopSort(reaction, menuSession);
                                break;

                            case "Shop_Sort_Confirm":
                                await ShopReactions.Nav_ShopSortConfirm(reaction, menuSession);
                                break;

                            // Namespace: SocialLinker.Core.Menus.Settings
                            case "Settings_Main_Menu":
                                await Settings_Reactions.Nav_Settings_Main_Menu(reaction, menuSession);
                                break;

                            case "Time_Weather_Main":
                                await Time_Weather_Reactions.Nav_Time_Weather_Main(reaction, menuSession);
                                break;

                            case "Time_Weather_Error":
                                await Time_Weather_Reactions.Nav_Time_Weather_Error(reaction, menuSession);
                                break;

                            case "Time_Weather_Confirm":
                                await Time_Weather_Reactions.Nav_Time_Weather_Confirm(reaction, menuSession);
                                break;

                            case "Level_Up_Notifications_Main":
                                await Level_Up_Notifications_Reactions.Nav_Level_Up_Notifications_Main(reaction, menuSession);
                                break;

                            case "Level_Up_Notifications_Confirm":
                                await Level_Up_Notifications_Reactions.Nav_Level_Up_Notifications_Confirm(reaction, menuSession);
                                break;

                            case "Rank_Up_Notifications_Main":
                                await Rank_Up_Notifications_Reactions.Nav_Rank_Up_Notifications_Main(reaction, menuSession);
                                break;

                            case "Rank_Up_Notifications_Confirm":
                                await Rank_Up_Notifications_Reactions.Nav_Rank_Up_Notifications_Confirm(reaction, menuSession);
                                break;

                            case "Content_Filter_Main":
                                await Content_Filter_Reactions.Nav_Content_Filter_Main_Added(reaction, menuSession);
                                break;

                            case "Content_Filter_VC_P1_Main":
                                await Content_Filter_Reactions.Nav_Content_Filter_VC_P1_Main_Added(reaction, menuSession);
                                break;

                            case "Content_Filter_VC_P2IS_Main":
                                await Content_Filter_Reactions.Nav_Content_Filter_VC_P2IS_Main_Added(reaction, menuSession);
                                break;

                            case "Content_Filter_VC_P2EP_Main":
                                await Content_Filter_Reactions.Nav_Content_Filter_VC_P2EP_Main_Added(reaction, menuSession);
                                break;

                            case "Content_Filter_VC_P3_Main":
                                await Content_Filter_Reactions.Nav_Content_Filter_VC_P3_Main_Added(reaction, menuSession);
                                break;

                            case "Content_Filter_VC_P4_Main":
                                await Content_Filter_Reactions.Nav_Content_Filter_VC_P4_Main_Added(reaction, menuSession);
                                break;

                            case "Content_Filter_VC_P5_Main":
                                await Content_Filter_Reactions.Nav_Content_Filter_VC_P5_Main_Added(reaction, menuSession);
                                break;

                            case "Content_Filter_Confirm":
                                await Content_Filter_Reactions.Nav_Content_Filter_Confirm(reaction, menuSession);
                                break;

                            // Namespace: SocialLinker.Core.Menus.Settings.Main.Profile
                            case "Profile_Settings_Menu":
                                await Profile_Settings_Reactions.Nav_Profile_Settings_Main(reaction, menuSession);
                                break;

                            case "Profile_Theme_Main":
                                await Profile_Theme_Reactions.Nav_Profile_Theme_Main(reaction, menuSession);
                                break;

                            case "Profile_Theme_Confirm":
                                await Profile_Theme_Reactions.Nav_Profile_Theme_Confirm(reaction, menuSession);
                                break;

                            case "Status_Decor_Main":
                                await Status_Decor_Reactions.Nav_Status_Decor_Main(reaction, menuSession);
                                break;

                            case "Set_Decor_Preview":
                                await Status_Decor_Reactions.Nav_Set_Decor_Preview(reaction, menuSession);
                                break;

                            case "Set_Decor_Confirm":
                                await Status_Decor_Reactions.Nav_Set_Decor_Confirm(reaction, menuSession);
                                break;

                            case "Decor_Sort":
                                await Status_Decor_Reactions.Nav_Decor_Sort(reaction, menuSession);
                                break;

                            case "Decor_Sort_Confirm":
                                await Status_Decor_Reactions.Nav_Decor_Sort_Confirm(reaction, menuSession);
                                break;

                            case "Star_Level_Main":
                                await Star_Level_Reactions.Nav_Star_Level_Main(reaction, menuSession);
                                break;

                            case "Star_Level_Check":
                                await Star_Level_Reactions.Nav_Star_Level_Check(reaction, menuSession);
                                break;

                            case "Star_Level_Confirm":
                                await Star_Level_Reactions.Nav_Star_Level_Confirm(reaction, menuSession);
                                break;

                            // Namespace: SocialLinker.Core.Menus.Settings.Main.SceneMaker
                            case "SM_Settings_Menu":
                                await SM_Settings_Reactions.Nav_SM_Settings_Main(reaction, menuSession);
                                break;

                            case "Version_Control_Main":
                                await Version_Control_Reactions.Nav_Version_Control_Main(reaction, menuSession);
                                break;

                            case "Version_Control_P1":
                                await Version_Control_Reactions.Nav_Version_Control_P1(reaction, menuSession);
                                break;

                            case "Version_Control_P2IS":
                                await Version_Control_Reactions.Nav_Version_Control_P2IS(reaction, menuSession);
                                break;

                            case "Version_Control_P2EP":
                                await Version_Control_Reactions.Nav_Version_Control_P2EP(reaction, menuSession);
                                break;

                            case "Version_Control_P3":
                                await Version_Control_Reactions.Nav_Version_Control_P3(reaction, menuSession);
                                break;

                            case "Version_Control_P4":
                                await Version_Control_Reactions.Nav_Version_Control_P4(reaction, menuSession);
                                break;

                            case "Version_Control_P5":
                                await Version_Control_Reactions.Nav_Version_Control_P5(reaction, menuSession);
                                break;

                            case "Version_Control_P1_Confirm":
                                await Version_Control_Reactions.Nav_Version_Control_P1_Confirm(reaction, menuSession);
                                break;

                            case "Version_Control_P2IS_Confirm":
                                await Version_Control_Reactions.Nav_Version_Control_P2IS_Confirm(reaction, menuSession);
                                break;

                            case "Version_Control_P2EP_Confirm":
                                await Version_Control_Reactions.Nav_Version_Control_P2EP_Confirm(reaction, menuSession);
                                break;

                            case "Version_Control_P3_Confirm":
                                await Version_Control_Reactions.Nav_Version_Control_P3_Confirm(reaction, menuSession);
                                break;

                            case "Version_Control_P4_Confirm":
                                await Version_Control_Reactions.Nav_Version_Control_P4_Confirm(reaction, menuSession);
                                break;

                            case "Version_Control_P5_Confirm":
                                await Version_Control_Reactions.Nav_Version_Control_P5_Confirm(reaction, menuSession);
                                break;

                            case "Template_Layout_Main":
                                await Template_Layout_Reactions.Nav_Template_Layout_Main(reaction, menuSession);
                                break;

                            case "Template_Layout_VC_P1_Main":
                                await Template_Layout_VC_Reactions.Nav_Template_Layout_VC_P1_Main(reaction, menuSession);
                                break;

                            case "Template_Layout_VC_P2IS_Main":
                                await Template_Layout_VC_Reactions.Nav_Template_Layout_VC_P2IS_Main(reaction, menuSession);
                                break;

                            case "Template_Layout_VC_P2EP_Main":
                                await Template_Layout_VC_Reactions.Nav_Template_Layout_VC_P2EP_Main(reaction, menuSession);
                                break;

                            case "Template_Layout_VC_P3_Main":
                                await Template_Layout_VC_Reactions.Nav_Template_Layout_VC_P3_Main(reaction, menuSession);
                                break;

                            case "Template_Layout_VC_P4_Main":
                                await Template_Layout_VC_Reactions.Nav_Template_Layout_VC_P4_Main(reaction, menuSession);
                                break;

                            case "Template_Layout_VC_P5_Main":
                                await Template_Layout_VC_Reactions.Nav_Template_Layout_VC_P5_Main(reaction, menuSession);
                                break;

                            case "Template_Layout_P1_PS1_Main":
                                await Template_Layout_P1_PS1_Reactions.Nav_Template_Layout_P1_PS1_Main(reaction, menuSession);
                                break;

                            case "Template_Layout_P1_PS1_Wallpaper":
                                await Template_Layout_P1_PS1_Reactions.Nav_Template_Layout_P1_PS1_Wallpaper(reaction, menuSession);
                                break;

                            case "Template_Layout_P1_PS1_Moon_Phases":
                                await Template_Layout_P1_PS1_Reactions.Nav_Template_Layout_P1_PS1_Moon_Phases(reaction, menuSession);
                                break;

                            case "Template_Layout_P1_PS1_Placement":
                                await Template_Layout_P1_PS1_Reactions.Nav_Template_Layout_P1_PS1_Placement(reaction, menuSession);
                                break;

                            case "Template_Layout_P1_PS1_BG_Darken":
                                await Template_Layout_P1_PS1_Reactions.Nav_Template_Layout_P1_PS1_BG_Darken(reaction, menuSession);
                                break;

                            case "Template_Layout_P1_PS1_Consistent_Names":
                                await Template_Layout_P1_PS1_Reactions.Nav_Template_Layout_P1_PS1_Consistent_Names(reaction, menuSession);
                                break;

                            case "Template_Layout_P1_PS1_Localized_Names":
                                await Template_Layout_P1_PS1_Reactions.Nav_Template_Layout_P1_PS1_Localized_Names(reaction, menuSession);
                                break;

                            case "Template_Layout_P1_PS1_Wallpaper_Confirm":
                                await Template_Layout_P1_PS1_Reactions.Nav_Template_Layout_P1_PS1_Wallpaper_Confirm(reaction, menuSession);
                                break;

                            case "Template_Layout_P1_PS1_Moon_Phases_Confirm":
                                await Template_Layout_P1_PS1_Reactions.Nav_Template_Layout_P1_PS1_Moon_Phases_Confirm(reaction, menuSession);
                                break;

                            case "Template_Layout_P1_PS1_Placement_Confirm":
                                await Template_Layout_P1_PS1_Reactions.Nav_Template_Layout_P1_PS1_Placement_Confirm(reaction, menuSession);
                                break;

                            case "Template_Layout_P1_PS1_BG_Darken_Confirm":
                                await Template_Layout_P1_PS1_Reactions.Nav_Template_Layout_P1_PS1_BG_Darken_Confirm(reaction, menuSession);
                                break;

                            case "Template_Layout_P1_PS1_Consistent_Names_Confirm":
                                await Template_Layout_P1_PS1_Reactions.Nav_Template_Layout_P1_PS1_Consistent_Names_Confirm(reaction, menuSession);
                                break;

                            case "Template_Layout_P1_PS1_Localized_Names_Confirm":
                                await Template_Layout_P1_PS1_Reactions.Nav_Template_Layout_P1_PS1_Localized_Names_Confirm(reaction, menuSession);
                                break;

                            case "Template_Layout_P1_PSP_Main":
                                await Template_Layout_P1_PSP_Reactions.Nav_Template_Layout_P1_PSP_Main(reaction, menuSession);
                                break;

                            case "Template_Layout_P1_PSP_Moon_Phases":
                                await Template_Layout_P1_PSP_Reactions.Nav_Template_Layout_P1_PSP_Moon_Phases(reaction, menuSession);
                                break;

                            case "Template_Layout_P1_PSP_Placement":
                                await Template_Layout_P1_PSP_Reactions.Nav_Template_Layout_P1_PSP_Placement(reaction, menuSession);
                                break;

                            case "Template_Layout_P1_PSP_BG_Darken":
                                await Template_Layout_P1_PSP_Reactions.Nav_Template_Layout_P1_PSP_BG_Darken(reaction, menuSession);
                                break;

                            case "Template_Layout_P1_PSP_Moon_Phases_Confirm":
                                await Template_Layout_P1_PSP_Reactions.Nav_Template_Layout_P1_PSP_Moon_Phases_Confirm(reaction, menuSession);
                                break;

                            case "Template_Layout_P1_PSP_Placement_Confirm":
                                await Template_Layout_P1_PSP_Reactions.Nav_Template_Layout_P1_PSP_Placement_Confirm(reaction, menuSession);
                                break;

                            case "Template_Layout_P1_PSP_BG_Darken_Confirm":
                                await Template_Layout_P1_PSP_Reactions.Nav_Template_Layout_P1_PSP_BG_Darken_Confirm(reaction, menuSession);
                                break;

                            case "Template_Layout_P2IS_PS1_Main":
                                await Template_Layout_P2IS_PS1_Reactions.Nav_Template_Layout_P2IS_PS1_Main(reaction, menuSession);
                                break;

                            case "Template_Layout_P2IS_PS1_Wallpaper":
                                await Template_Layout_P2IS_PS1_Reactions.Nav_Template_Layout_P2IS_PS1_Wallpaper(reaction, menuSession);
                                break;

                            case "Template_Layout_P2IS_PS1_Inverted_Filter":
                                await Template_Layout_P2IS_PS1_Reactions.Nav_Template_Layout_P2IS_PS1_Inverted_Filter(reaction, menuSession);
                                break;

                            case "Template_Layout_P2IS_PS1_Placement":
                                await Template_Layout_P2IS_PS1_Reactions.Nav_Template_Layout_P2IS_PS1_Placement(reaction, menuSession);
                                break;

                            case "Template_Layout_P2IS_PS1_Sprite_Flip":
                                await Template_Layout_P2IS_PS1_Reactions.Nav_Template_Layout_P2IS_PS1_Sprite_Flip(reaction, menuSession);
                                break;

                            case "Template_Layout_P2IS_PS1_Localized_Names":
                                await Template_Layout_P2IS_PS1_Reactions.Nav_Template_Layout_P2IS_PS1_Localized_Names(reaction, menuSession);
                                break;

                            case "Template_Layout_P2IS_PS1_Wallpaper_Confirm":
                                await Template_Layout_P2IS_PS1_Reactions.Nav_Template_Layout_P2IS_PS1_Wallpaper_Confirm(reaction, menuSession);
                                break;

                            case "Template_Layout_P2IS_PS1_Inverted_Filter_Confirm":
                                await Template_Layout_P2IS_PS1_Reactions.Nav_Template_Layout_P2IS_PS1_Inverted_Filter_Confirm(reaction, menuSession);
                                break;

                            case "Template_Layout_P2IS_PS1_Placement_Confirm":
                                await Template_Layout_P2IS_PS1_Reactions.Nav_Template_Layout_P2IS_PS1_Placement_Confirm(reaction, menuSession);
                                break;

                            case "Template_Layout_P2IS_PS1_Sprite_Flip_Confirm":
                                await Template_Layout_P2IS_PS1_Reactions.Nav_Template_Layout_P2IS_PS1_Sprite_Flip_Confirm(reaction, menuSession);
                                break;

                            case "Template_Layout_P2IS_PS1_Localized_Names_Confirm":
                                await Template_Layout_P2IS_PS1_Reactions.Nav_Template_Layout_P2IS_PS1_Localized_Names_Confirm(reaction, menuSession);
                                break;

                            case "Template_Layout_P2IS_PSP_Main":
                                await Template_Layout_P2IS_PSP_Reactions.Nav_Template_Layout_P2IS_PSP_Main(reaction, menuSession);
                                break;

                            case "Template_Layout_P2IS_PSP_Inverted_Filter":
                                await Template_Layout_P2IS_PSP_Reactions.Nav_Template_Layout_P2IS_PSP_Inverted_Filter(reaction, menuSession);
                                break;

                            case "Template_Layout_P2IS_PSP_Placement":
                                await Template_Layout_P2IS_PSP_Reactions.Nav_Template_Layout_P2IS_PSP_Placement(reaction, menuSession);
                                break;

                            case "Template_Layout_P2IS_PSP_Sprite_Flip":
                                await Template_Layout_P2IS_PSP_Reactions.Nav_Template_Layout_P2IS_PSP_Sprite_Flip(reaction, menuSession);
                                break;

                            case "Template_Layout_P2IS_PSP_Inverted_Filter_Confirm":
                                await Template_Layout_P2IS_PSP_Reactions.Nav_Template_Layout_P2IS_PSP_Inverted_Filter_Confirm(reaction, menuSession);
                                break;

                            case "Template_Layout_P2IS_PSP_Placement_Confirm":
                                await Template_Layout_P2IS_PSP_Reactions.Nav_Template_Layout_P2IS_PSP_Placement_Confirm(reaction, menuSession);
                                break;

                            case "Template_Layout_P2IS_PSP_Sprite_Flip_Confirm":
                                await Template_Layout_P2IS_PSP_Reactions.Nav_Template_Layout_P2IS_PSP_Sprite_Flip_Confirm(reaction, menuSession);
                                break;

                            case "Template_Layout_P2EP_PS1_Main":
                                await Template_Layout_P2EP_PS1_Reactions.Nav_Template_Layout_P2EP_PS1_Main(reaction, menuSession);
                                break;

                            case "Template_Layout_P2EP_PS1_Wallpaper":
                                await Template_Layout_P2EP_PS1_Reactions.Nav_Template_Layout_P2EP_PS1_Wallpaper(reaction, menuSession);
                                break;

                            case "Template_Layout_P2EP_PS1_Inverted_Filter":
                                await Template_Layout_P2EP_PS1_Reactions.Nav_Template_Layout_P2EP_PS1_Inverted_Filter(reaction, menuSession);
                                break;

                            case "Template_Layout_P2EP_PS1_Placement":
                                await Template_Layout_P2EP_PS1_Reactions.Nav_Template_Layout_P2EP_PS1_Placement(reaction, menuSession);
                                break;

                            case "Template_Layout_P2EP_PS1_Sprite_Flip":
                                await Template_Layout_P2EP_PS1_Reactions.Nav_Template_Layout_P2EP_PS1_Sprite_Flip(reaction, menuSession);
                                break;

                            case "Template_Layout_P2EP_PS1_Localized_Names":
                                await Template_Layout_P2EP_PS1_Reactions.Nav_Template_Layout_P2EP_PS1_Localized_Names(reaction, menuSession);
                                break;

                            case "Template_Layout_P2EP_PS1_Wallpaper_Confirm":
                                await Template_Layout_P2EP_PS1_Reactions.Nav_Template_Layout_P2EP_PS1_Wallpaper_Confirm(reaction, menuSession);
                                break;

                            case "Template_Layout_P2EP_PS1_Inverted_Filter_Confirm":
                                await Template_Layout_P2EP_PS1_Reactions.Nav_Template_Layout_P2EP_PS1_Inverted_Filter_Confirm(reaction, menuSession);
                                break;

                            case "Template_Layout_P2EP_PS1_Placement_Confirm":
                                await Template_Layout_P2EP_PS1_Reactions.Nav_Template_Layout_P2EP_PS1_Placement_Confirm(reaction, menuSession);
                                break;

                            case "Template_Layout_P2EP_PS1_Sprite_Flip_Confirm":
                                await Template_Layout_P2EP_PS1_Reactions.Nav_Template_Layout_P2EP_PS1_Sprite_Flip_Confirm(reaction, menuSession);
                                break;

                            case "Template_Layout_P2EP_PS1_Localized_Names_Confirm":
                                await Template_Layout_P2EP_PS1_Reactions.Nav_Template_Layout_P2EP_PS1_Localized_Names_Confirm(reaction, menuSession);
                                break;

                            case "Template_Layout_P2EP_PSP_Main":
                                await Template_Layout_P2EP_PSP_Reactions.Nav_Template_Layout_P2EP_PSP_Main(reaction, menuSession);
                                break;

                            case "Template_Layout_P2EP_PSP_Window_Color":
                                await Template_Layout_P2EP_PSP_Reactions.Nav_Template_Layout_P2EP_PSP_Window_Color(reaction, menuSession);
                                break;

                            case "Template_Layout_P2EP_PSP_Inverted_Filter":
                                await Template_Layout_P2EP_PSP_Reactions.Nav_Template_Layout_P2EP_PSP_Inverted_Filter(reaction, menuSession);
                                break;

                            case "Template_Layout_P2EP_PSP_Placement":
                                await Template_Layout_P2EP_PSP_Reactions.Nav_Template_Layout_P2EP_PSP_Placement(reaction, menuSession);
                                break;

                            case "Template_Layout_P2EP_PSP_Sprite_Flip":
                                await Template_Layout_P2EP_PSP_Reactions.Nav_Template_Layout_P2EP_PSP_Sprite_Flip(reaction, menuSession);
                                break;

                            case "Template_Layout_P2EP_PSP_Window_Color_Confirm":
                                await Template_Layout_P2EP_PSP_Reactions.Nav_Template_Layout_P2EP_PSP_Window_Color_Confirm(reaction, menuSession);
                                break;

                            case "Template_Layout_P2EP_PSP_Inverted_Filter_Confirm":
                                await Template_Layout_P2EP_PSP_Reactions.Nav_Template_Layout_P2EP_PSP_Inverted_Filter_Confirm(reaction, menuSession);
                                break;

                            case "Template_Layout_P2EP_PSP_Placement_Confirm":
                                await Template_Layout_P2EP_PSP_Reactions.Nav_Template_Layout_P2EP_PSP_Placement_Confirm(reaction, menuSession);
                                break;

                            case "Template_Layout_P2EP_PSP_Sprite_Flip_Confirm":
                                await Template_Layout_P2EP_PSP_Reactions.Nav_Template_Layout_P2EP_PSP_Sprite_Flip_Confirm(reaction, menuSession);
                                break;

                            case "Template_Layout_P3F_Main":
                                await Template_Layout_P3F_Reactions.Nav_Template_Layout_P3F_Main(reaction, menuSession);
                                break;

                            case "Template_Layout_P3F_Date_Moon":
                                await Template_Layout_P3F_Reactions.Nav_Template_Layout_P3F_Date_Moon(reaction, menuSession);
                                break;

                            case "Template_Layout_P3F_Navigatior_Window":
                                await Template_Layout_P3F_Reactions.Nav_Template_Layout_P3F_Navigatior_Window(reaction, menuSession);
                                break;

                            case "Template_Layout_P3F_Date_Moon_Confirm":
                                await Template_Layout_P3F_Reactions.Nav_Template_Layout_P3F_Date_Moon_Confirm(reaction, menuSession);
                                break;

                            case "Template_Layout_P3F_Navigatior_Window_Confirm":
                                await Template_Layout_P3F_Reactions.Nav_Template_Layout_P3F_Navigatior_Window_Confirm(reaction, menuSession);
                                break;

                            case "Template_Layout_P3P_Main":
                                await Template_Layout_P3P_Reactions.Nav_Template_Layout_P3P_Main(reaction, menuSession);
                                break;

                            case "Template_Layout_P3P_Color_Scheme":
                                await Template_Layout_P3P_Reactions.Nav_Template_Layout_P3P_Color_Scheme(reaction, menuSession);
                                break;

                            case "Template_Layout_P3P_Date_Moon":
                                await Template_Layout_P3P_Reactions.Nav_Template_Layout_P3P_Date_Moon(reaction, menuSession);
                                break;

                            case "Template_Layout_P3P_Sprite_Placement":
                                await Template_Layout_P3P_Reactions.Nav_Template_Layout_P3P_Sprite_Placement(reaction, menuSession);
                                break;

                            case "Template_Layout_P3P_Dual_Character":
                                await Template_Layout_P3P_Reactions.Nav_Template_Layout_P3P_Dual_Character(reaction, menuSession);
                                break;

                            case "Template_Layout_P3P_Color_Scheme_Confirm":
                                await Template_Layout_P3P_Reactions.Nav_Template_Layout_P3P_Color_Scheme_Confirm(reaction, menuSession);
                                break;

                            case "Template_Layout_P3P_Date_Moon_Confirm":
                                await Template_Layout_P3P_Reactions.Nav_Template_Layout_P3P_Date_Moon_Confirm(reaction, menuSession);
                                break;

                            case "Template_Layout_P3P_Sprite_Placement_Confirm":
                                await Template_Layout_P3P_Reactions.Nav_Template_Layout_P3P_Sprite_Placement_Confirm(reaction, menuSession);
                                break;

                            case "Template_Layout_P3P_Dual_Character_Confirm":
                                await Template_Layout_P3P_Reactions.Nav_Template_Layout_P3P_Dual_Character_Confirm(reaction, menuSession);
                                break;

                            case "Template_Layout_P4_PS2_Main":
                                await Template_Layout_P4_PS2_Reactions.Nav_Template_Layout_P4_PS2_Main(reaction, menuSession);
                                break;

                            case "Template_Layout_P4_PS2_Date_Weather":
                                await Template_Layout_P4_PS2_Reactions.Nav_Template_Layout_P4_PS2_Date_Weather(reaction, menuSession);
                                break;

                            case "Template_Layout_P4_PS2_Date_Weather_Confirm":
                                await Template_Layout_P4_PS2_Reactions.Nav_Template_Layout_P4_PS2_Date_Weather_Confirm(reaction, menuSession);
                                break;

                            case "Template_Layout_P4G_Main":
                                await Template_Layout_P4G_Reactions.Nav_Template_Layout_P4G_Main(reaction, menuSession);
                                break;

                            case "Template_Layout_P4G_Date_Weather":
                                await Template_Layout_P4G_Reactions.Nav_Template_Layout_P4G_Date_Weather(reaction, menuSession);
                                break;

                            case "Template_Layout_P4G_Date_Weather_Confirm":
                                await Template_Layout_P4G_Reactions.Nav_Template_Layout_P4G_Date_Weather_Confirm(reaction, menuSession);
                                break;

                            case "Template_Layout_P4AU_Main":
                                await Template_Layout_P4AU_Reactions.Nav_Template_Layout_P4AU_Main(reaction, menuSession);
                                break;

                            case "Template_Layout_P4AU_Scene_Type":
                                await Template_Layout_P4AU_Reactions.Nav_Template_Layout_P4AU_Scene_Type(reaction, menuSession);
                                break;

                            case "Template_Layout_P4AU_Auto_Advance":
                                await Template_Layout_P4AU_Reactions.Nav_Template_Layout_P4AU_Auto_Advance(reaction, menuSession);
                                break;

                            case "Template_Layout_P4AU_Control_Panel":
                                await Template_Layout_P4AU_Reactions.Nav_Template_Layout_P4AU_Control_Panel(reaction, menuSession);
                                break;

                            case "Template_Layout_P4AU_Sprite_Placement":
                                await Template_Layout_P4AU_Reactions.Nav_Template_Layout_P4AU_Sprite_Placement(reaction, menuSession);
                                break;

                            case "Template_Layout_P4AU_Dual_Character":
                                await Template_Layout_P4AU_Reactions.Nav_Template_Layout_P4AU_Dual_Character(reaction, menuSession);
                                break;

                            case "Template_Layout_P4AU_Navigator_Location":
                                await Template_Layout_P4AU_Reactions.Nav_Template_Layout_P4AU_Navigator_Location(reaction, menuSession);
                                break;

                            case "Template_Layout_P4AU_Caller_Location":
                                await Template_Layout_P4AU_Reactions.Nav_Template_Layout_P4AU_Caller_Location(reaction, menuSession);
                                break;

                            case "Template_Layout_P4AU_Highlight":
                                await Template_Layout_P4AU_Reactions.Nav_Template_Layout_P4AU_Highlight(reaction, menuSession);
                                break;

                            case "Template_Layout_P4AU_Scene_Type_Confirm":
                                await Template_Layout_P4AU_Reactions.Nav_Template_Layout_P4AU_Scene_Type_Confirm(reaction, menuSession);
                                break;

                            case "Template_Layout_P4AU_Auto_Advance_Confirm":
                                await Template_Layout_P4AU_Reactions.Nav_Template_Layout_P4AU_Auto_Advance_Confirm(reaction, menuSession);
                                break;

                            case "Template_Layout_P4AU_Control_Panel_Confirm":
                                await Template_Layout_P4AU_Reactions.Nav_Template_Layout_P4AU_Control_Panel_Confirm(reaction, menuSession);
                                break;

                            case "Template_Layout_P4AU_Sprite_Placement_Confirm":
                                await Template_Layout_P4AU_Reactions.Nav_Template_Layout_P4AU_Sprite_Placement_Confirm(reaction, menuSession);
                                break;

                            case "Template_Layout_P4AU_Dual_Character_Default_Confirm":
                                await Template_Layout_P4AU_Reactions.Nav_Template_Layout_P4AU_Dual_Character_Default_Confirm(reaction, menuSession);
                                break;

                            case "Template_Layout_P4AU_Dual_Character_Navigator_Confirm":
                                await Template_Layout_P4AU_Reactions.Nav_Template_Layout_P4AU_Dual_Character_Navigator_Confirm(reaction, menuSession);
                                break;

                            case "Template_Layout_P4AU_Dual_Character_Caller_Confirm":
                                await Template_Layout_P4AU_Reactions.Nav_Template_Layout_P4AU_Dual_Character_Caller_Confirm(reaction, menuSession);
                                break;

                            case "Template_Layout_P4AU_Navigator_Location_Confirm":
                                await Template_Layout_P4AU_Reactions.Nav_Template_Layout_P4AU_Navigator_Location_Confirm(reaction, menuSession);
                                break;

                            case "Template_Layout_P4AU_Caller_Location_Confirm":
                                await Template_Layout_P4AU_Reactions.Nav_Template_Layout_P4AU_Caller_Location_Confirm(reaction, menuSession);
                                break;

                            case "Template_Layout_P4AU_Highlight_Confirm":
                                await Template_Layout_P4AU_Reactions.Nav_Template_Layout_P4AU_Highlight_Confirm(reaction, menuSession);
                                break;

                            case "Template_Layout_P4D_Main":
                                await Template_Layout_P4D_Reactions.Nav_Template_Layout_P4D_Main(reaction, menuSession);
                                break;

                            case "Template_Layout_P4D_Scene_Type":
                                await Template_Layout_P4D_Reactions.Nav_Template_Layout_P4D_Scene_Type(reaction, menuSession);
                                break;

                            case "Template_Layout_P4D_Auto_Advance":
                                await Template_Layout_P4D_Reactions.Nav_Template_Layout_P4D_Auto_Advance(reaction, menuSession);
                                break;

                            case "Template_Layout_P4D_Sprite_Placement":
                                await Template_Layout_P4D_Reactions.Nav_Template_Layout_P4D_Sprite_Placement(reaction, menuSession);
                                break;

                            case "Template_Layout_P4D_Dual_Character":
                                await Template_Layout_P4D_Reactions.Nav_Template_Layout_P4D_Dual_Character(reaction, menuSession);
                                break;

                            case "Template_Layout_P4D_Navigator_Caller_Location":
                                await Template_Layout_P4D_Reactions.Nav_Template_Layout_P4D_Navigator_Caller_Location(reaction, menuSession);
                                break;

                            case "Template_Layout_P4D_Scene_Type_Confirm":
                                await Template_Layout_P4D_Reactions.Nav_Template_Layout_P4D_Scene_Type_Confirm(reaction, menuSession);
                                break;

                            case "Template_Layout_P4D_Auto_Advance_Confirm":
                                await Template_Layout_P4D_Reactions.Nav_Template_Layout_P4D_Auto_Advance_Confirm(reaction, menuSession);
                                break;

                            case "Template_Layout_P4D_Sprite_Placement_Confirm":
                                await Template_Layout_P4D_Reactions.Nav_Template_Layout_P4D_Sprite_Placement_Confirm(reaction, menuSession);
                                break;

                            case "Template_Layout_P4D_Dual_Character_Default_Confirm":
                                await Template_Layout_P4D_Reactions.Nav_Template_Layout_P4D_Dual_Character_Default_Confirm(reaction, menuSession);
                                break;

                            case "Template_Layout_P4D_Dual_Character_Navigator_Caller_Confirm":
                                await Template_Layout_P4D_Reactions.Nav_Template_Layout_P4D_Dual_Character_Navigator_Caller_Confirm(reaction, menuSession);
                                break;

                            case "Template_Layout_P4D_Navigator_Caller_Location_Confirm":
                                await Template_Layout_P4D_Reactions.Nav_Template_Layout_P4D_Navigator_Caller_Location_Confirm(reaction, menuSession);
                                break;

                            case "Template_Layout_P5_PS3_Main":
                                await Template_Layout_P5_PS3_Reactions.Nav_Template_Layout_P5_PS3_Main(reaction, menuSession);
                                break;

                            case "Template_Layout_P5_PS3_Date_Weather":
                                await Template_Layout_P5_PS3_Reactions.Nav_Template_Layout_P5_PS3_Date_Weather(reaction, menuSession);
                                break;

                            case "Template_Layout_P5_PS3_Scene_Border":
                                await Template_Layout_P5_PS3_Reactions.Nav_Template_Layout_P5_PS3_Scene_Border(reaction, menuSession);
                                break;

                            case "Template_Layout_P5_PS3_Cursor_Panel":
                                await Template_Layout_P5_PS3_Reactions.Nav_Template_Layout_P5_PS3_Cursor_Panel(reaction, menuSession);
                                break;

                            case "Template_Layout_P5_PS3_Date_Weather_Confirm":
                                await Template_Layout_P5_PS3_Reactions.Nav_Template_Layout_P5_PS3_Date_Weather_Confirm(reaction, menuSession);
                                break;

                            case "Template_Layout_P5_PS3_Scene_Border_Confirm":
                                await Template_Layout_P5_PS3_Reactions.Nav_Template_Layout_P5_PS3_Scene_Border_Confirm(reaction, menuSession);
                                break;

                            case "Template_Layout_P5_PS3_Cursor_Panel_Confirm":
                                await Template_Layout_P5_PS3_Reactions.Nav_Template_Layout_P5_PS3_Cursor_Panel_Confirm(reaction, menuSession);
                                break;

                            case "Template_Layout_P5R_Main":
                                await Template_Layout_P5R_Reactions.Nav_Template_Layout_P5R_Main(reaction, menuSession);
                                break;

                            case "Template_Layout_P5R_Date_Weather":
                                await Template_Layout_P5R_Reactions.Nav_Template_Layout_P5R_Date_Weather(reaction, menuSession);
                                break;

                            case "Template_Layout_P5R_Scene_Border":
                                await Template_Layout_P5R_Reactions.Nav_Template_Layout_P5R_Scene_Border(reaction, menuSession);
                                break;

                            case "Template_Layout_P5R_Cursor_Panel":
                                await Template_Layout_P5R_Reactions.Nav_Template_Layout_P5R_Cursor_Panel(reaction, menuSession);
                                break;

                            case "Template_Layout_P5R_Phone_Calls_Main":
                                await Template_Layout_P5R_Reactions.Nav_Template_Layout_P5R_Phone_Calls_Main(reaction, menuSession);
                                break;

                            case "Template_Layout_P5R_Phone_Calls_Toggle":
                                await Template_Layout_P5R_Reactions.Nav_Template_Layout_P5R_Phone_Calls_Toggle(reaction, menuSession);
                                break;

                            case "Template_Layout_P5R_Phone_Calls_Location":
                                await Template_Layout_P5R_Reactions.Nav_Template_Layout_P5R_Phone_Calls_Location(reaction, menuSession);
                                break;

                            case "Template_Layout_P5R_Date_Weather_Confirm":
                                await Template_Layout_P5R_Reactions.Nav_Template_Layout_P5R_Date_Weather_Confirm(reaction, menuSession);
                                break;

                            case "Template_Layout_P5R_Scene_Border_Confirm":
                                await Template_Layout_P5R_Reactions.Nav_Template_Layout_P5R_Scene_Border_Confirm(reaction, menuSession);
                                break;

                            case "Template_Layout_P5R_Cursor_Panel_Confirm":
                                await Template_Layout_P5R_Reactions.Nav_Template_Layout_P5R_Cursor_Panel_Confirm(reaction, menuSession);
                                break;

                            case "Template_Layout_P5R_Phone_Calls_Toggle_Confirm":
                                await Template_Layout_P5R_Reactions.Nav_Template_Layout_P5R_Phone_Calls_Toggle_Confirm(reaction, menuSession);
                                break;

                            case "Template_Layout_P5R_Phone_Calls_Location_Confirm":
                                await Template_Layout_P5R_Reactions.Nav_Template_Layout_P5R_Phone_Calls_Location_Confirm(reaction, menuSession);
                                break;

                            case "Template_Layout_P5S_Main":
                                await Template_Layout_P5S_Reactions.Nav_Template_Layout_P5S_Main(reaction, menuSession);
                                break;

                            case "Template_Layout_P5S_Controller_Type":
                                await Template_Layout_P5S_Reactions.Nav_Template_Layout_P5S_Controller_Type(reaction, menuSession);
                                break;

                            case "Template_Layout_P5S_Skip_Button":
                                await Template_Layout_P5S_Reactions.Nav_Template_Layout_P5S_Skip_Button(reaction, menuSession);
                                break;

                            case "Template_Layout_P5S_Auto_Advance":
                                await Template_Layout_P5S_Reactions.Nav_Template_Layout_P5S_Auto_Advance(reaction, menuSession);
                                break;

                            case "Template_Layout_P5S_Scene_Border":
                                await Template_Layout_P5S_Reactions.Nav_Template_Layout_P5S_Scene_Border(reaction, menuSession);
                                break;

                            case "Template_Layout_P5S_Date_Location_Layout":
                                await Template_Layout_P5S_Reactions.Nav_Template_Layout_P5S_Date_Location_Layout(reaction, menuSession);
                                break;

                            case "Template_Layout_P5S_Location_Icon":
                                await Template_Layout_P5S_Reactions.Nav_Template_Layout_P5S_Location_Icon(reaction, menuSession);
                                break;

                            case "Template_Layout_P5S_Watermark":
                                await Template_Layout_P5S_Reactions.Nav_Template_Layout_P5S_Watermark(reaction, menuSession);
                                break;

                            case "Template_Layout_P5S_Controller_Type_Confirm":
                                await Template_Layout_P5S_Reactions.Nav_Template_Layout_P5S_Controller_Type_Confirm(reaction, menuSession);
                                break;

                            case "Template_Layout_P5S_Skip_Button_Confirm":
                                await Template_Layout_P5S_Reactions.Nav_Template_Layout_P5S_Skip_Button_Confirm(reaction, menuSession);
                                break;

                            case "Template_Layout_P5S_Auto_Advance_Confirm":
                                await Template_Layout_P5S_Reactions.Nav_Template_Layout_P5S_Auto_Advance_Confirm(reaction, menuSession);
                                break;

                            case "Template_Layout_P5S_Scene_Border_Confirm":
                                await Template_Layout_P5S_Reactions.Nav_Template_Layout_P5S_Scene_Border_Confirm(reaction, menuSession);
                                break;

                            case "Template_Layout_P5S_Date_Location_Layout_Confirm":
                                await Template_Layout_P5S_Reactions.Nav_Template_Layout_P5S_Date_Location_Layout_Confirm(reaction, menuSession);
                                break;

                            case "Template_Layout_P5S_Location_Icon_Confirm":
                                await Template_Layout_P5S_Reactions.Nav_Template_Layout_P5S_Location_Icon_Confirm(reaction, menuSession);
                                break;

                            case "Template_Layout_P5S_Watermark_Confirm":
                                await Template_Layout_P5S_Reactions.Nav_Template_Layout_P5S_Watermark_Confirm(reaction, menuSession);
                                break;

                            case "Template_Layout_BBTAG_Main":
                                await Template_Layout_BBTAG_Reactions.Nav_Template_Layout_BBTAG_Main(reaction, menuSession);
                                break;

                            case "Template_Layout_BBTAG_Header":
                                await Template_Layout_BBTAG_Reactions.Nav_Template_Layout_BBTAG_Header(reaction, menuSession);
                                break;

                            case "Template_Layout_BBTAG_Sprite_Placement":
                                await Template_Layout_BBTAG_Reactions.Nav_Template_Layout_BBTAG_Sprite_Placement(reaction, menuSession);
                                break;

                            case "Template_Layout_BBTAG_Background_Blur":
                                await Template_Layout_BBTAG_Reactions.Nav_Template_Layout_BBTAG_Background_Blur(reaction, menuSession);
                                break;

                            case "Template_Layout_BBTAG_Header_Confirm":
                                await Template_Layout_BBTAG_Reactions.Nav_Template_Layout_BBTAG_Header_Confirm(reaction, menuSession);
                                break;

                            case "Template_Layout_BBTAG_Sprite_Placement_Confirm":
                                await Template_Layout_BBTAG_Reactions.Nav_Template_Layout_BBTAG_Sprite_Placement_Confirm(reaction, menuSession);
                                break;

                            case "Template_Layout_BBTAG_Background_Blur_Confirm":
                                await Template_Layout_BBTAG_Reactions.Nav_Template_Layout_BBTAG_Background_Blur_Confirm(reaction, menuSession);
                                break;

                            case "Calendar_Cycles_Main":
                                await Calendar_Cycles_Reactions.Nav_Calendar_Cycles_Main(reaction, menuSession);
                                break;

                            //case "Calendar_Cycles_P5S_Main":
                            //    await Calendar_Cycles_Reactions.Nav_Calendar_Cycles_Main(reaction, menuSession);
                            //    break;

                            case "Display_Names_Main":
                                await Display_Names_Reactions.Nav_Display_Names_Main(reaction, menuSession);
                                break;

                            case "Display_Names_Title_Select":
                                await Display_Names_Title_Select_Reactions.Nav_Display_Names_Title_Select(reaction, menuSession);
                                break;

                            case "Display_Names_Title_Select_VC_P1_Main":
                                await Display_Names_Title_Select_Reactions.Nav_Display_Names_Title_Select_VC_P1_Main(reaction, menuSession);
                                break;

                            case "Display_Names_Title_Select_VC_P2IS_Main":
                                await Display_Names_Title_Select_Reactions.Nav_Display_Names_Title_Select_VC_P2IS_Main(reaction, menuSession);
                                break;

                            case "Display_Names_Title_Select_VC_P2EP_Main":
                                await Display_Names_Title_Select_Reactions.Nav_Display_Names_Title_Select_VC_P2EP_Main(reaction, menuSession);
                                break;

                            case "Display_Names_Title_Select_VC_P3_Main":
                                await Display_Names_Title_Select_Reactions.Nav_Display_Names_Title_Select_VC_P3_Main(reaction, menuSession);
                                break;

                            case "Display_Names_Title_Select_VC_P4_Main":
                                await Display_Names_Title_Select_Reactions.Nav_Display_Names_Title_Select_VC_P4_Main(reaction, menuSession);
                                break;

                            case "Display_Names_Title_Select_VC_P5_Main":
                                await Display_Names_Title_Select_Reactions.Nav_Display_Names_Title_Select_VC_P5_Main(reaction, menuSession);
                                break;

                            case "Display_Names_Character_Select_Main":
                                await Display_Names_Character_Select_Reactions.Nav_Display_Names_Character_Select_Main(reaction, menuSession);
                                break;

                            case "Display_Names_Character_Select_Error":
                                await Display_Names_Character_Select_Reactions.Nav_Display_Names_Character_Select_Error(reaction, menuSession);
                                break;

                            case "Display_Names_Sprite_Select_Main":
                                await Display_Names_Sprite_Select_Reactions.Nav_Display_Names_Sprite_Select_Main(reaction, menuSession);
                                break;

                            case "Display_Names_Sprite_Select_Error_1":
                                await Display_Names_Sprite_Select_Reactions.Nav_Display_Names_Sprite_Select_Error_1(reaction, menuSession);
                                break;

                            case "Display_Names_Sprite_Select_Error_2":
                                await Display_Names_Sprite_Select_Reactions.Nav_Display_Names_Sprite_Select_Error_2(reaction, menuSession);
                                break;

                            case "Display_Names_Sprite_Select_Error_3":
                                await Display_Names_Sprite_Select_Reactions.Nav_Display_Names_Sprite_Select_Error_3(reaction, menuSession);
                                break;

                            case "Display_Names_Custom_Input_Main":
                                await Display_Names_Custom_Input_Reactions.Nav_Display_Names_Custom_Input_Main(reaction, menuSession);
                                break;

                            case "Display_Names_Custom_Input_Error":
                                await Display_Names_Custom_Input_Reactions.Nav_Display_Names_Custom_Input_Error(reaction, menuSession);
                                break;

                            case "Display_Names_Confirm_Main":
                                await Display_Name_Confirm_Reactions.Nav_Display_Names_Confirm_Main(reaction, menuSession);
                                break;

                            case "Display_Names_Sort":
                                await Display_Names_Sort_Reactions.Nav_Display_Names_Sort(reaction, menuSession);
                                break;

                            case "Display_Names_Sort_Confirm":
                                await Display_Names_Sort_Reactions.Nav_Display_Names_Sort_Confirm(reaction, menuSession);
                                break;

                            case "Display_Names_Edit_Main":
                                await Display_Names_Edit_Reactions.Nav_Display_Names_Edit_Main(reaction, menuSession);
                                break;

                            case "Display_Names_Delete_Confirmation":
                                await Display_Names_Edit_Reactions.Nav_Display_Names_Delete_Confirmation(reaction, menuSession);
                                break;

                            case "Sheet_Order_Main":
                                await Sheet_Order_Reactions.Nav_Sheet_Order_Main(reaction, menuSession);
                                break;

                            case "Sheet_Order_Confirm":
                                await Sheet_Order_Reactions.Nav_Sheet_Order_Confirm(reaction, menuSession);
                                break;

                            case "Backgrounds_Main":
                                await Backgrounds_Reactions.Nav_Backgrounds_Main(reaction, menuSession);
                                break;

                            case "Backgrounds_Default_Color":
                                await Backgrounds_Reactions.Nav_Backgrounds_Default_Color(reaction, menuSession);
                                break;

                            case "Backgrounds_Upload_Settings":
                                await Backgrounds_Reactions.Nav_Backgrounds_Upload_Settings(reaction, menuSession);
                                break;

                            case "Backgrounds_Default_Color_Error":
                                await Backgrounds_Reactions.Nav_Backgrounds_Default_Color_Error(reaction, menuSession);
                                break;

                            case "Backgrounds_Default_Color_Confirm":
                                await Backgrounds_Reactions.Nav_Backgrounds_Default_Color_Confirm(reaction, menuSession);
                                break;

                            case "Backgrounds_Upload_Settings_Confirm":
                                await Backgrounds_Reactions.Nav_Backgrounds_Upload_Settings_Confirm(reaction, menuSession);
                                break;

                            case "Resolution_Scaling_Main":
                                await Resolution_Scaling_Reactions.Nav_Resolution_Scaling_Main(reaction, menuSession);
                                break;

                            case "Resolution_Scaling_VC_P1_Main":
                                await Resolution_Scaling_VC_Reactions.Nav_Resolution_Scaling_VC_P1_Main(reaction, menuSession);
                                break;

                            case "Resolution_Scaling_VC_P2IS_Main":
                                await Resolution_Scaling_VC_Reactions.Nav_Resolution_Scaling_VC_P2IS_Main(reaction, menuSession);
                                break;

                            case "Resolution_Scaling_VC_P2EP_Main":
                                await Resolution_Scaling_VC_Reactions.Nav_Resolution_Scaling_VC_P2EP_Main(reaction, menuSession);
                                break;

                            case "Resolution_Scaling_VC_P3_Main":
                                await Resolution_Scaling_VC_Reactions.Nav_Resolution_Scaling_VC_P3_Main(reaction, menuSession);
                                break;

                            case "Resolution_Scaling_P1_PS1_Main":
                                await Resolution_Scaling_P1_PS1_Reactions.Nav_Resolution_Scaling_P1_PS1_Main(reaction, menuSession);
                                break;

                            case "Resolution_Scaling_P1_PS1_Output_Resolution":
                                await Resolution_Scaling_P1_PS1_Reactions.Nav_Resolution_Scaling_P1_PS1_Output_Resolution(reaction, menuSession);
                                break;

                            case "Resolution_Scaling_P1_PS1_Scaling_Method":
                                await Resolution_Scaling_P1_PS1_Reactions.Nav_Resolution_Scaling_P1_PS1_Scaling_Method(reaction, menuSession);
                                break;

                            case "Resolution_Scaling_P1_PS1_Output_Resolution_Confirm":
                                await Resolution_Scaling_P1_PS1_Reactions.Nav_Resolution_Scaling_P1_PS1_Output_Resolution_Confirm(reaction, menuSession);
                                break;

                            case "Resolution_Scaling_P1_PS1_Scaling_Method_Confirm":
                                await Resolution_Scaling_P1_PS1_Reactions.Nav_Resolution_Scaling_P1_PS1_Scaling_Method_Confirm(reaction, menuSession);
                                break;

                            case "Resolution_Scaling_P1_PSP_Main":
                                await Resolution_Scaling_P1_PSP_Reactions.Nav_Resolution_Scaling_P1_PSP_Main(reaction, menuSession);
                                break;

                            case "Resolution_Scaling_P1_PSP_Output_Resolution":
                                await Resolution_Scaling_P1_PSP_Reactions.Nav_Resolution_Scaling_P1_PSP_Output_Resolution(reaction, menuSession);
                                break;

                            case "Resolution_Scaling_P1_PSP_Scaling_Method":
                                await Resolution_Scaling_P1_PSP_Reactions.Nav_Resolution_Scaling_P1_PSP_Scaling_Method(reaction, menuSession);
                                break;

                            case "Resolution_Scaling_P1_PSP_Output_Resolution_Confirm":
                                await Resolution_Scaling_P1_PSP_Reactions.Nav_Resolution_Scaling_P1_PSP_Output_Resolution_Confirm(reaction, menuSession);
                                break;

                            case "Resolution_Scaling_P1_PSP_Scaling_Method_Confirm":
                                await Resolution_Scaling_P1_PSP_Reactions.Nav_Resolution_Scaling_P1_PSP_Scaling_Method_Confirm(reaction, menuSession);
                                break;

                            case "Resolution_Scaling_P2IS_PS1_Main":
                                await Resolution_Scaling_P2IS_PS1_Reactions.Nav_Resolution_Scaling_P2IS_PS1_Main(reaction, menuSession);
                                break;

                            case "Resolution_Scaling_P2IS_PS1_Output_Resolution":
                                await Resolution_Scaling_P2IS_PS1_Reactions.Nav_Resolution_Scaling_P2IS_PS1_Output_Resolution(reaction, menuSession);
                                break;

                            case "Resolution_Scaling_P2IS_PS1_Scaling_Method":
                                await Resolution_Scaling_P2IS_PS1_Reactions.Nav_Resolution_Scaling_P2IS_PS1_Scaling_Method(reaction, menuSession);
                                break;

                            case "Resolution_Scaling_P2IS_PS1_Output_Resolution_Confirm":
                                await Resolution_Scaling_P2IS_PS1_Reactions.Nav_Resolution_Scaling_P2IS_PS1_Output_Resolution_Confirm(reaction, menuSession);
                                break;

                            case "Resolution_Scaling_P2IS_PS1_Scaling_Method_Confirm":
                                await Resolution_Scaling_P2IS_PS1_Reactions.Nav_Resolution_Scaling_P2IS_PS1_Scaling_Method_Confirm(reaction, menuSession);
                                break;

                            case "Resolution_Scaling_P2IS_PSP_Main":
                                await Resolution_Scaling_P2IS_PSP_Reactions.Nav_Resolution_Scaling_P2IS_PSP_Main(reaction, menuSession);
                                break;

                            case "Resolution_Scaling_P2IS_PSP_Output_Resolution":
                                await Resolution_Scaling_P2IS_PSP_Reactions.Nav_Resolution_Scaling_P2IS_PSP_Output_Resolution(reaction, menuSession);
                                break;

                            case "Resolution_Scaling_P2IS_PSP_Scaling_Method":
                                await Resolution_Scaling_P2IS_PSP_Reactions.Nav_Resolution_Scaling_P2IS_PSP_Scaling_Method(reaction, menuSession);
                                break;

                            case "Resolution_Scaling_P2IS_PSP_Output_Resolution_Confirm":
                                await Resolution_Scaling_P2IS_PSP_Reactions.Nav_Resolution_Scaling_P2IS_PSP_Output_Resolution_Confirm(reaction, menuSession);
                                break;

                            case "Resolution_Scaling_P2IS_PSP_Scaling_Method_Confirm":
                                await Resolution_Scaling_P2IS_PSP_Reactions.Nav_Resolution_Scaling_P2IS_PSP_Scaling_Method_Confirm(reaction, menuSession);
                                break;

                            case "Resolution_Scaling_P2EP_PS1_Main":
                                await Resolution_Scaling_P2EP_PS1_Reactions.Nav_Resolution_Scaling_P2EP_PS1_Main(reaction, menuSession);
                                break;

                            case "Resolution_Scaling_P2EP_PS1_Output_Resolution":
                                await Resolution_Scaling_P2EP_PS1_Reactions.Nav_Resolution_Scaling_P2EP_PS1_Output_Resolution(reaction, menuSession);
                                break;

                            case "Resolution_Scaling_P2EP_PS1_Scaling_Method":
                                await Resolution_Scaling_P2EP_PS1_Reactions.Nav_Resolution_Scaling_P2EP_PS1_Scaling_Method(reaction, menuSession);
                                break;

                            case "Resolution_Scaling_P2EP_PS1_Output_Resolution_Confirm":
                                await Resolution_Scaling_P2EP_PS1_Reactions.Nav_Resolution_Scaling_P2EP_PS1_Output_Resolution_Confirm(reaction, menuSession);
                                break;

                            case "Resolution_Scaling_P2EP_PS1_Scaling_Method_Confirm":
                                await Resolution_Scaling_P2EP_PS1_Reactions.Nav_Resolution_Scaling_P2EP_PS1_Scaling_Method_Confirm(reaction, menuSession);
                                break;

                            case "Resolution_Scaling_P2EP_PSP_Main":
                                await Resolution_Scaling_P2EP_PSP_Reactions.Nav_Resolution_Scaling_P2EP_PSP_Main(reaction, menuSession);
                                break;

                            case "Resolution_Scaling_P2EP_PSP_Output_Resolution":
                                await Resolution_Scaling_P2EP_PSP_Reactions.Nav_Resolution_Scaling_P2EP_PSP_Output_Resolution(reaction, menuSession);
                                break;

                            case "Resolution_Scaling_P2EP_PSP_Scaling_Method":
                                await Resolution_Scaling_P2EP_PSP_Reactions.Nav_Resolution_Scaling_P2EP_PSP_Scaling_Method(reaction, menuSession);
                                break;

                            case "Resolution_Scaling_P2EP_PSP_Output_Resolution_Confirm":
                                await Resolution_Scaling_P2EP_PSP_Reactions.Nav_Resolution_Scaling_P2EP_PSP_Output_Resolution_Confirm(reaction, menuSession);
                                break;

                            case "Resolution_Scaling_P2EP_PSP_Scaling_Method_Confirm":
                                await Resolution_Scaling_P2EP_PSP_Reactions.Nav_Resolution_Scaling_P2EP_PSP_Scaling_Method_Confirm(reaction, menuSession);
                                break;

                            case "Resolution_Scaling_P3F_Main":
                                await Resolution_Scaling_P3F_Reactions.Nav_Resolution_Scaling_P3F_Main(reaction, menuSession);
                                break;

                            case "Resolution_Scaling_P3F_Output_Resolution":
                                await Resolution_Scaling_P3F_Reactions.Nav_Resolution_Scaling_P3F_Output_Resolution(reaction, menuSession);
                                break;

                            case "Resolution_Scaling_P3F_Scaling_Method":
                                await Resolution_Scaling_P3F_Reactions.Nav_Resolution_Scaling_P3F_Scaling_Method(reaction, menuSession);
                                break;

                            case "Resolution_Scaling_P3F_Output_Resolution_Confirm":
                                await Resolution_Scaling_P3F_Reactions.Nav_Resolution_Scaling_P3F_Output_Resolution_Confirm(reaction, menuSession);
                                break;

                            case "Resolution_Scaling_P3F_Scaling_Method_Confirm":
                                await Resolution_Scaling_P3F_Reactions.Nav_Resolution_Scaling_P3F_Scaling_Method_Confirm(reaction, menuSession);
                                break;

                            case "Resolution_Scaling_P3P_Main":
                                await Resolution_Scaling_P3P_Reactions.Nav_Resolution_Scaling_P3P_Main(reaction, menuSession);
                                break;

                            case "Resolution_Scaling_P3P_Output_Resolution":
                                await Resolution_Scaling_P3P_Reactions.Nav_Resolution_Scaling_P3P_Output_Resolution(reaction, menuSession);
                                break;

                            case "Resolution_Scaling_P3P_Scaling_Method":
                                await Resolution_Scaling_P3P_Reactions.Nav_Resolution_Scaling_P3P_Scaling_Method(reaction, menuSession);
                                break;

                            case "Resolution_Scaling_P3P_Output_Resolution_Confirm":
                                await Resolution_Scaling_P3P_Reactions.Nav_Resolution_Scaling_P3P_Output_Resolution_Confirm(reaction, menuSession);
                                break;

                            case "Resolution_Scaling_P3P_Scaling_Method_Confirm":
                                await Resolution_Scaling_P3P_Reactions.Nav_Resolution_Scaling_P3P_Scaling_Method_Confirm(reaction, menuSession);
                                break;

                            case "Resolution_Scaling_P4_PS2_Main":
                                await Resolution_Scaling_P4_PS2_Reactions.Nav_Resolution_Scaling_P4_PS2_Main(reaction, menuSession);
                                break;

                            case "Resolution_Scaling_P4_PS2_Output_Resolution":
                                await Resolution_Scaling_P4_PS2_Reactions.Nav_Resolution_Scaling_P4_PS2_Output_Resolution(reaction, menuSession);
                                break;

                            case "Resolution_Scaling_P4_PS2_Scaling_Method":
                                await Resolution_Scaling_P4_PS2_Reactions.Nav_Resolution_Scaling_P4_PS2_Scaling_Method(reaction, menuSession);
                                break;

                            case "Resolution_Scaling_P4_PS2_Output_Resolution_Confirm":
                                await Resolution_Scaling_P4_PS2_Reactions.Nav_Resolution_Scaling_P4_PS2_Output_Resolution_Confirm(reaction, menuSession);
                                break;

                            case "Resolution_Scaling_P4_PS2_Scaling_Method_Confirm":
                                await Resolution_Scaling_P4_PS2_Reactions.Nav_Resolution_Scaling_P4_PS2_Scaling_Method_Confirm(reaction, menuSession);
                                break;

                            case "Resolution_Scaling_P4AU_Main":
                                await Resolution_Scaling_P4AU_Reactions.Nav_Resolution_Scaling_P4AU_Main(reaction, menuSession);
                                break;

                            case "Resolution_Scaling_P4AU_Output_Resolution":
                                await Resolution_Scaling_P4AU_Reactions.Nav_Resolution_Scaling_P4AU_Output_Resolution(reaction, menuSession);
                                break;

                            case "Resolution_Scaling_P4AU_Scaling_Method":
                                await Resolution_Scaling_P4AU_Reactions.Nav_Resolution_Scaling_P4AU_Scaling_Method(reaction, menuSession);
                                break;

                            case "Resolution_Scaling_P4AU_Output_Resolution_Confirm":
                                await Resolution_Scaling_P4AU_Reactions.Nav_Resolution_Scaling_P4AU_Output_Resolution_Confirm(reaction, menuSession);
                                break;

                            case "Resolution_Scaling_P4AU_Scaling_Method_Confirm":
                                await Resolution_Scaling_P4AU_Reactions.Nav_Resolution_Scaling_P4AU_Scaling_Method_Confirm(reaction, menuSession);
                                break;

                            case "Auto_Delete_Main":
                                await Auto_Delete_Reactions.Nav_Auto_Delete_Main(reaction, menuSession);
                                break;

                            case "Auto_Delete_Commands":
                                await Auto_Delete_Reactions.Nav_Auto_Delete_Commands(reaction, menuSession);
                                break;

                            case "Auto_Delete_Error_Messages":
                                await Auto_Delete_Reactions.Nav_Auto_Delete_Error_Messages(reaction, menuSession);
                                break;

                            case "Auto_Delete_Commands_Confirm":
                                await Auto_Delete_Reactions.Nav_Auto_Delete_Commands_Confirm(reaction, menuSession);
                                break;

                            case "Auto_Delete_Error_Messages_Confirm":
                                await Auto_Delete_Reactions.Nav_Auto_Delete_Error_Messages_Confirm(reaction, menuSession);
                                break;

                            case "MakerMulti_Main_Menu":
                                await MakerMulti_Title_Select_Reactions.Nav_MakerMulti_Main_Menu(reaction, menuSession);
                                break;

                            case "MakerMulti_VC_P2IS_Main":
                                await MakerMulti_Title_Select_Reactions.Nav_MakerMulti_VC_P2IS_Main(reaction, menuSession);
                                break;

                            case "MakerMulti_VC_P2EP_Main":
                                await MakerMulti_Title_Select_Reactions.Nav_MakerMulti_VC_P2EP_Main(reaction, menuSession);
                                break;

                            case "MakerMulti_Layout_Select_Main":
                                await MakerMulti_Layout_Select_Reactions.Nav_MakerMulti_Layout_Select_Main(reaction, menuSession);
                                break;

                            case "MakerMulti_Character_Select_1_Main":
                                await MakerMulti_Char_Select_1_Reactions.Nav_MakerMulti_Character_Select_1_Main(reaction, menuSession);
                                break;

                            case "MakerMulti_Character_Select_1_Invalid_Character":
                                await MakerMulti_Char_Select_1_Reactions.Nav_MakerMulti_Character_Select_1_Invalid_Character(reaction, menuSession);
                                break;

                            case "MakerMulti_Sprite_Select_1_Main":
                                await MakerMulti_Sprite_Select_1_Reactions.Nav_MakerMulti_Sprite_Select_1_Main(reaction, menuSession);
                                break;

                            case "MakerMulti_Sprite_Select_1_Invalid_Base_Sprite":
                                await MakerMulti_Sprite_Select_1_Reactions.Nav_MakerMulti_Sprite_Select_1_Invalid_Base_Sprite(reaction, menuSession);
                                break;

                            case "MakerMulti_Sprite_Select_1_Error_Too_Many_Animation_Frames":
                                await MakerMulti_Sprite_Select_1_Reactions.Nav_MakerMulti_Sprite_Select_1_Error_Too_Many_Animation_Frames(reaction, menuSession);
                                break;

                            case "MakerMulti_Sprite_Select_1_Error_Non_Digit_In_Sprite_Number":
                                await MakerMulti_Sprite_Select_1_Reactions.Nav_MakerMulti_Sprite_Select_1_Error_Non_Digit_In_Sprite_Number(reaction, menuSession);
                                break;

                            case "MakerMulti_Sprite_Select_1_Error_Animation_Frame_With_Blank_Sprite":
                                await MakerMulti_Sprite_Select_1_Reactions.Nav_MakerMulti_Sprite_Select_1_Error_Animation_Frame_With_Blank_Sprite(reaction, menuSession);
                                break;

                            case "MakerMulti_Sprite_Select_1_Error_Eye_Frame_Not_Found":
                                await MakerMulti_Sprite_Select_1_Reactions.Nav_MakerMulti_Sprite_Select_1_Error_Eye_Frame_Not_Found(reaction, menuSession);
                                break;

                            case "MakerMulti_Sprite_Select_1_Error_Mouth_Frame_Not_Found":
                                await MakerMulti_Sprite_Select_1_Reactions.Nav_MakerMulti_Sprite_Select_1_Error_Mouth_Frame_Not_Found(reaction, menuSession);
                                break;

                            case "MakerMulti_Character_Select_2_Main":
                                await MakerMulti_Char_Select_2_Reactions.Nav_MakerMulti_Character_Select_2_Main(reaction, menuSession);
                                break;

                            case "MakerMulti_Character_Select_2_Invalid_Character":
                                await MakerMulti_Char_Select_2_Reactions.Nav_MakerMulti_Character_Select_2_Invalid_Character(reaction, menuSession);
                                break;

                            case "MakerMulti_Sprite_Select_2_Main":
                                await MakerMulti_Sprite_Select_2_Reactions.Nav_MakerMulti_Sprite_Select_2_Main(reaction, menuSession);
                                break;

                            case "MakerMulti_Sprite_Select_2_Invalid_Base_Sprite":
                                await MakerMulti_Sprite_Select_2_Reactions.Nav_MakerMulti_Sprite_Select_2_Invalid_Base_Sprite(reaction, menuSession);
                                break;

                            case "MakerMulti_Sprite_Select_2_Error_Too_Many_Animation_Frames":
                                await MakerMulti_Sprite_Select_2_Reactions.Nav_MakerMulti_Sprite_Select_2_Error_Too_Many_Animation_Frames(reaction, menuSession);
                                break;

                            case "MakerMulti_Sprite_Select_2_Error_Non_Digit_In_Sprite_Number":
                                await MakerMulti_Sprite_Select_2_Reactions.Nav_MakerMulti_Sprite_Select_2_Error_Non_Digit_In_Sprite_Number(reaction, menuSession);
                                break;

                            case "MakerMulti_Sprite_Select_2_Error_Animation_Frame_With_Blank_Sprite":
                                await MakerMulti_Sprite_Select_2_Reactions.Nav_MakerMulti_Sprite_Select_2_Error_Animation_Frame_With_Blank_Sprite(reaction, menuSession);
                                break;

                            case "MakerMulti_Sprite_Select_2_Error_Eye_Frame_Not_Found":
                                await MakerMulti_Sprite_Select_2_Reactions.Nav_MakerMulti_Sprite_Select_2_Error_Eye_Frame_Not_Found(reaction, menuSession);
                                break;

                            case "MakerMulti_Sprite_Select_2_Error_Mouth_Frame_Not_Found":
                                await MakerMulti_Sprite_Select_2_Reactions.Nav_MakerMulti_Sprite_Select_2_Error_Mouth_Frame_Not_Found(reaction, menuSession);
                                break;

                            case "MakerMulti_Character_Select_3_Main":
                                await MakerMulti_Char_Select_3_Reactions.Nav_MakerMulti_Character_Select_3_Main(reaction, menuSession);
                                break;

                            case "MakerMulti_Character_Select_3_Invalid_Character":
                                await MakerMulti_Char_Select_3_Reactions.Nav_MakerMulti_Character_Select_3_Invalid_Character(reaction, menuSession);
                                break;

                            case "MakerMulti_Sprite_Select_3_Main":
                                await MakerMulti_Sprite_Select_3_Reactions.Nav_MakerMulti_Sprite_Select_3_Main(reaction, menuSession);
                                break;

                            case "MakerMulti_Sprite_Select_3_Invalid_Base_Sprite":
                                await MakerMulti_Sprite_Select_3_Reactions.Nav_MakerMulti_Sprite_Select_3_Invalid_Base_Sprite(reaction, menuSession);
                                break;

                            case "MakerMulti_Sprite_Select_3_Error_Too_Many_Animation_Frames":
                                await MakerMulti_Sprite_Select_3_Reactions.Nav_MakerMulti_Sprite_Select_3_Error_Too_Many_Animation_Frames(reaction, menuSession);
                                break;

                            case "MakerMulti_Sprite_Select_3_Error_Non_Digit_In_Sprite_Number":
                                await MakerMulti_Sprite_Select_3_Reactions.Nav_MakerMulti_Sprite_Select_3_Error_Non_Digit_In_Sprite_Number(reaction, menuSession);
                                break;

                            case "MakerMulti_Sprite_Select_3_Error_Animation_Frame_With_Blank_Sprite":
                                await MakerMulti_Sprite_Select_3_Reactions.Nav_MakerMulti_Sprite_Select_3_Error_Animation_Frame_With_Blank_Sprite(reaction, menuSession);
                                break;

                            case "MakerMulti_Sprite_Select_3_Error_Eye_Frame_Not_Found":
                                await MakerMulti_Sprite_Select_3_Reactions.Nav_MakerMulti_Sprite_Select_3_Error_Eye_Frame_Not_Found(reaction, menuSession);
                                break;

                            case "MakerMulti_Sprite_Select_3_Error_Mouth_Frame_Not_Found":
                                await MakerMulti_Sprite_Select_3_Reactions.Nav_MakerMulti_Sprite_Select_3_Error_Mouth_Frame_Not_Found(reaction, menuSession);
                                break;

                            case "MakerMulti_Character_Select_4_Main":
                                await MakerMulti_Char_Select_4_Reactions.Nav_MakerMulti_Character_Select_4_Main(reaction, menuSession);
                                break;

                            case "MakerMulti_Character_Select_4_Invalid_Character":
                                await MakerMulti_Char_Select_4_Reactions.Nav_MakerMulti_Character_Select_4_Invalid_Character(reaction, menuSession);
                                break;

                            case "MakerMulti_Sprite_Select_4_Main":
                                await MakerMulti_Sprite_Select_4_Reactions.Nav_MakerMulti_Sprite_Select_4_Main(reaction, menuSession);
                                break;

                            case "MakerMulti_Sprite_Select_4_Invalid_Base_Sprite":
                                await MakerMulti_Sprite_Select_4_Reactions.Nav_MakerMulti_Sprite_Select_4_Invalid_Base_Sprite(reaction, menuSession);
                                break;

                            case "MakerMulti_Sprite_Select_4_Error_Too_Many_Animation_Frames":
                                await MakerMulti_Sprite_Select_4_Reactions.Nav_MakerMulti_Sprite_Select_4_Error_Too_Many_Animation_Frames(reaction, menuSession);
                                break;

                            case "MakerMulti_Sprite_Select_4_Error_Non_Digit_In_Sprite_Number":
                                await MakerMulti_Sprite_Select_4_Reactions.Nav_MakerMulti_Sprite_Select_4_Error_Non_Digit_In_Sprite_Number(reaction, menuSession);
                                break;

                            case "MakerMulti_Sprite_Select_4_Error_Animation_Frame_With_Blank_Sprite":
                                await MakerMulti_Sprite_Select_4_Reactions.Nav_MakerMulti_Sprite_Select_4_Error_Animation_Frame_With_Blank_Sprite(reaction, menuSession);
                                break;

                            case "MakerMulti_Sprite_Select_4_Error_Eye_Frame_Not_Found":
                                await MakerMulti_Sprite_Select_4_Reactions.Nav_MakerMulti_Sprite_Select_4_Error_Eye_Frame_Not_Found(reaction, menuSession);
                                break;

                            case "MakerMulti_Sprite_Select_4_Error_Mouth_Frame_Not_Found":
                                await MakerMulti_Sprite_Select_4_Reactions.Nav_MakerMulti_Sprite_Select_4_Error_Mouth_Frame_Not_Found(reaction, menuSession);
                                break;

                            case "MakerMulti_Speaker_Select_Main":
                                await MakerMulti_Speaker_Select_Reactions.Nav_MakerMulti_Speaker_Select_Main(reaction, menuSession);
                                break;

                            case "MakerMulti_Dialogue_Select_Main":
                                await MakerMulti_Dialogue_Select_Reactions.Nav_MakerMulti_Dialogue_Select_Main(reaction, menuSession);
                                break;

                            case "MakerMulti_Confirm_Details_Main":
                                await MakerMulti_Confirm_Details_Reactions.Nav_MakerMulti_Confirm_Details_Main(reaction, menuSession);
                                break;
                        }
                    }
                    // If the reactor is neither the bot nor the menu user, remove the reaction.
                    else
                    {
                        await menuSession.MenuMessage.RemoveReactionAsync(reaction.Emote, reactedUser);
                    }
                }
                // If the reactor's ID doesn't match a current menu session, let's test if it matches a scene maker image instead. They might want to delete something.
                else
                {
                    await DeleteSceneMakerImage(cache, reaction);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            
        }

        public static async Task ReactionRemovedIndex(Cacheable<IUserMessage, ulong> cache, Cacheable<IMessageChannel, ulong> channel, SocketReaction reaction)
        {
            // Create a variable for the user that reacted to the message by storing their ID
            var reactedUser = reaction.UserId;

            // Search the global Menu ID List by grabbing any entry that matches the message ID of the given reaction.
            if (Global.MenuIdList.Any(x => x.MenuMessage.Id == reaction.MessageId))
            {
                // Once a matching entry is found, store it in a variable.
                var menuSession = Global.MenuIdList.SingleOrDefault(x => x.MenuMessage.Id == reaction.MessageId);

                // If the reactor is the bot itself, don't do anything.
                if (reaction.UserId == BotConfig.bot.id)
                {
                    // Don't do anything
                }
                // Else, if the reactor is the menu user, perform an action.
                else if (reaction.UserId == menuSession.User.Id)
                {
                    // Next, let's check all the users who reacted with this emote before we perform any actions.
                    // We want to make sure the reaction added is an emote that the bot has also reacted to, or else we want to ignore it to prevent errors.
                    // First, get a list of all users who reacted with the emote. Set the limit to a value of 3 (only two should be here max).
                    var all_reacted_users = await menuSession.MenuMessage.GetReactionUsersAsync(reaction.Emote, 3).FlattenAsync();

                    // Next, send the list to a method that checks if the bot is among the reacted users to the emote. If not, remove the emote and return.
                    if (ReactionCheck(all_reacted_users) == true)
                    {
                        // Do nothing
                    }
                    else if (ReactionCheck(all_reacted_users) == false)
                    {
                        await menuSession.MenuMessage.RemoveReactionAsync(reaction.Emote, reactedUser);
                        return;
                    }

                    // Ensure that the current menu matches a certain state before proceeding.
                    switch (menuSession.CurrentMenu)
                    {
                        // Namespace: SocialLinker.Core.Menus.InitialUsage
                        case "First_Use_Content_Filter_Main":
                            await First_Use_Content_Filter_Reactions.Nav_First_Use_Content_Filter_Main_Removed(reaction, menuSession);
                            break;

                        case "First_Use_Content_Filter_VC_P1_Main":
                            await First_Use_Content_Filter_Reactions.Nav_First_Use_Content_Filter_VC_P1_Main_Removed(reaction, menuSession);
                            break;

                        case "First_Use_Content_Filter_VC_P2IS_Main":
                            await First_Use_Content_Filter_Reactions.Nav_First_Use_Content_Filter_VC_P2IS_Main_Removed(reaction, menuSession);
                            break;

                        case "First_Use_Content_Filter_VC_P2EP_Main":
                            await First_Use_Content_Filter_Reactions.Nav_First_Use_Content_Filter_VC_P2EP_Main_Removed(reaction, menuSession);
                            break;

                        case "First_Use_Content_Filter_VC_P3_Main":
                            await First_Use_Content_Filter_Reactions.Nav_First_Use_Content_Filter_VC_P3_Main_Removed(reaction, menuSession);
                            break;

                        case "First_Use_Content_Filter_VC_P4_Main":
                            await First_Use_Content_Filter_Reactions.Nav_First_Use_Content_Filter_VC_P4_Main_Removed(reaction, menuSession);
                            break;

                        case "First_Use_Content_Filter_VC_P5_Main":
                            await First_Use_Content_Filter_Reactions.Nav_First_Use_Content_Filter_VC_P5_Main_Removed(reaction, menuSession);
                            break;

                        // Namespace: SocialLinker.Core.Menus.Settings.Reactions.General
                        case "Content_Filter_Main":
                            await Content_Filter_Reactions.Nav_Content_Filter_Main_Removed(reaction, menuSession);
                            break;

                        case "Content_Filter_VC_P1_Main":
                            await Content_Filter_Reactions.Nav_Content_Filter_VC_P1_Main_Removed(reaction, menuSession);
                            break;

                        case "Content_Filter_VC_P2IS_Main":
                            await Content_Filter_Reactions.Nav_Content_Filter_VC_P2IS_Main_Removed(reaction, menuSession);
                            break;

                        case "Content_Filter_VC_P2EP_Main":
                            await Content_Filter_Reactions.Nav_Content_Filter_VC_P2EP_Main_Removed(reaction, menuSession);
                            break;

                        case "Content_Filter_VC_P3_Main":
                            await Content_Filter_Reactions.Nav_Content_Filter_VC_P3_Main_Removed(reaction, menuSession);
                            break;

                        case "Content_Filter_VC_P4_Main":
                            await Content_Filter_Reactions.Nav_Content_Filter_VC_P4_Main_Removed(reaction, menuSession);
                            break;

                        case "Content_Filter_VC_P5_Main":
                            await Content_Filter_Reactions.Nav_Content_Filter_VC_P5_Main_Removed(reaction, menuSession);
                            break;
                    }
                }
                // If the reactor is neither the bot nor the menu user, remove the reaction.
                else
                {
                    await menuSession.MenuMessage.RemoveReactionAsync(reaction.Emote, reactedUser);
                }
            }
        }

        public static async Task DeleteSceneMakerImage(Cacheable<IUserMessage, ulong> cache, SocketReaction reaction)
        {
            // Create a variable for the user that reacted to the message by storing their ID.
            var reactedUser = reaction.UserId;

            // Get the message from the cache.
            var message = await cache.GetOrDownloadAsync();

            // Check if the author of the message is the bot by matching up their user IDs.
            if (message.Author.Id == BotConfig.bot.id)
            {
                // Create a variable for the message attachment.
                var attachments = message.Attachments;

                // Create an empty string variable to hold the filename of the attachment.
                string filename = "";

                // If there are no attachments on the message, set the filename string to "None".
                if (attachments.LongCount() == 0)
                {
                    filename = "None";
                }
                // Else, assign the filename of the attachment to the string.
                else
                {
                    filename = attachments.ElementAt(0).Filename;
                }

                // If a filename for a message attachment exists, let's start analyzing it.
                if (filename != "None")
                {
                    // Create a string list to hold parts of the filename. We're about to split it up to obtain certain data.
                    List<string> split_filename;

                    // Likewise, assign delimiter characters to split the filename apart. All scenes should have an underscore separating their areas of info.
                    char[] delimiters = { '_' };

                    // Take the filename and split it by the delimiter specified.
                    // This will be assigned to the newly created string list.
                    split_filename = filename.Split(delimiters).ToList();

                    // Check that the first index of the split filename is "scene". This is important because we only want to delete scene maker images.
                    if (split_filename[0] == "scene")
                    {
                        // Next, check if the emote used is an ❌. This is the symbol we'll use to delete images.
                        if (reaction.Emote.Name == "❌")
                        {
                            // Finally, compare the second index of the split filename to the reactor's user ID. If they match, the image is from their own command.
                            if (split_filename[1] == reaction.UserId.ToString())
                            {
                                // Delete the scene image.
                                await message.DeleteAsync();
                            }
                            else
                            {
                                // If the IDs didn't match, the scene was not created by the reacter. Remove their ❌ emote to avoid confusion.
                                await message.RemoveReactionAsync(reaction.Emote, reactedUser);
                            }
                        }
                    }
                }
            }
        }

        public static async Task MessageReceivedIndex(DiscordShardedClient client, SocketMessage message)
        {
            int argPos = 0;
            SocketUserMessage msg = message as SocketUserMessage;

            try
            {
                if (!msg.HasMentionPrefix(client.CurrentUser, ref argPos))
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            // Search the global Menu ID List by grabbing any entry that matches the channel ID of the sent message.
            if (Global.MenuIdList.Any(x => x.MenuMessage.Channel.Id == message.Channel.Id))
            {
                // Once a matching entry is found, store it in a variable.
                var menuSession = Global.MenuIdList.SingleOrDefault(x => x.MenuMessage.Channel.Id == message.Channel.Id);

                // If the message author is the menu user, perform an action.
                if ((message.Author.Id == menuSession.User.Id) && (!string.IsNullOrEmpty(message.Content)))
                {
                    // Ensure that the current menu matches a certain state before proceeding.
                    switch (menuSession.CurrentMenu)
                    {
                        // Namespace: SocialLinker.Core.Menus.Settings.Reactions.General
                        case "Backgrounds_Default_Color":
                            await Backgrounds_Reactions.Nav_Backgrounds_Default_Color_Message_Received(message, menuSession);
                            break;

                        case "Time_Weather_Main":
                            await Time_Weather_Reactions.Nav_Time_Weather_Main_Received(message, menuSession);
                            break;

                        case "Display_Names_Character_Select_Main":
                            await Display_Names_Character_Select_Reactions.Nav_Display_Names_Character_Select_Main_Received(message, menuSession);
                            break;

                        case "Display_Names_Sprite_Select_Main":
                            await Display_Names_Sprite_Select_Reactions.Nav_Display_Names_Sprite_Select_Main_Received(message, menuSession);
                            break;

                        case "Display_Names_Custom_Input_Main":
                            await Display_Names_Custom_Input_Reactions.Nav_Display_Names_Custom_Input_Main_Received(message, menuSession);
                            break;

                        case "MakerMulti_Character_Select_1_Main":
                            await MakerMulti_Char_Select_1_Reactions.Nav_MakerMulti_Character_Select_1_Main_Received(message, menuSession);
                            break;

                        case "MakerMulti_Sprite_Select_1_Main":
                            await MakerMulti_Sprite_Select_1_Reactions.Nav_MakerMulti_Sprite_Select_1_Main_Received(message, menuSession);
                            break;

                        case "MakerMulti_Character_Select_2_Main":
                            await MakerMulti_Char_Select_2_Reactions.Nav_MakerMulti_Character_Select_2_Main_Received(message, menuSession);
                            break;

                        case "MakerMulti_Sprite_Select_2_Main":
                            await MakerMulti_Sprite_Select_2_Reactions.Nav_MakerMulti_Sprite_Select_2_Main_Received(message, menuSession);
                            break;

                        case "MakerMulti_Character_Select_3_Main":
                            await MakerMulti_Char_Select_3_Reactions.Nav_MakerMulti_Character_Select_3_Main_Received(message, menuSession);
                            break;

                        case "MakerMulti_Sprite_Select_3_Main":
                            await MakerMulti_Sprite_Select_3_Reactions.Nav_MakerMulti_Sprite_Select_3_Main_Received(message, menuSession);
                            break;

                        case "MakerMulti_Character_Select_4_Main":
                            await MakerMulti_Char_Select_4_Reactions.Nav_MakerMulti_Character_Select_4_Main_Received(message, menuSession);
                            break;

                        case "MakerMulti_Sprite_Select_4_Main":
                            await MakerMulti_Sprite_Select_4_Reactions.Nav_MakerMulti_Sprite_Select_4_Main_Received(message, menuSession);
                            break;

                        case "MakerMulti_Speaker_Select_Main":
                            await MakerMulti_Speaker_Select_Reactions.Nav_MakerMulti_Speaker_Select_Main_Received(message, menuSession);
                            break;

                        case "MakerMulti_Dialogue_Select_Main":
                            await MakerMulti_Dialogue_Select_Reactions.Nav_MakerMulti_Dialogue_Select_Main_Received(message, menuSession);
                            break;
                    }
                }
            }
        }

        public static bool ReactionCheck(IEnumerable<IUser> reactor_list)
        {
            foreach (IUser a in reactor_list)
            {
                if (a.Id == BotConfig.bot.id)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
