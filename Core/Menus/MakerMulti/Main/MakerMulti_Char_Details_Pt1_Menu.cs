using Discord;
using Discord.Rest;
using Discord.WebSocket;
using SocialLinker.Config;
using SocialLinker.Core.CloudStorageTables;
using SocialLinker.Core.LocalStorageTables;
using SocialLinker.Core.SceneMaker;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace SocialLinker.Core.Menus.MakerMulti.Main
{
    class MakerMulti_Char_Details_Pt1_Menu
    {
        private static string back_to_1char_detail_menu = "back-to-makermulti-1char-details";
        private static string back_to_2char_detail_menu = "back-to-makermulti-2char-details";
        //private static string back_to_bbtag_layout_menu = "back-to-makermulti-bbtag-layout-select";

        public static string Get_Back_Button_Id(SocialLinkerCommand multimaker_session)
        {
            switch (multimaker_session.MakerCommand.Template)
            {
                case "BBTAG":
                    switch (multimaker_session.MakerCommand.Expected_Characters)
                    {
                        case 1:
                            return back_to_1char_detail_menu;

                        default:
                            return back_to_2char_detail_menu;
                    }
            }

            return back_to_2char_detail_menu;
        }

        // Main
        public static async Task MakerMulti_1Char_Details_Main(MenuIdStructure menuSession)
        {
            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(menuSession.User);
            var message = menuSession.MenuMessage;
            var user = menuSession.User;

            var multimaker_session = Global.MultiMaker_Session_List.SingleOrDefault(x => x.User.Id == menuSession.User.Id);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Character Details",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            // Determine the color and thumbnail for the embeded message
            embed.WithColor(EmbedSettings.Get_Game_Color(multimaker_session.MakerCommand.Template, account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo(multimaker_session.MakerCommand.Template));

            embed.WithDescription("" +
                $"Let's get started with your scene! Select [Enter Character Details] to specify your character and their sprite number.");

            switch (multimaker_session.MakerCommand.Template)
            {
                case "BBTAG":
                    switch (multimaker_session.MakerCommand.BBTAG_Specific_Data.Layout)
                    {
                        case "1":
                            embed.WithImageUrl("https://i.imgur.com/7qN7IXv.png");
                            break;

                        case "2":
                            embed.WithImageUrl("https://i.imgur.com/l3ZG5Gq.png");
                            break;

                        case "3":
                            embed.WithImageUrl("https://i.imgur.com/BnDIfWC.png");
                            break;
                    }    
                    break;
            }

            var component = new ComponentBuilder()
                .WithButton("Enter Character Details", customId: "makermulti-1char-details-modal-open", ButtonStyle.Primary)
                .WithButton("↩️ Return", customId: "back-to-makermulti-bbtag-layout-select", ButtonStyle.Secondary);

            // Attempt editing the message if it hasn't been deleted by the user yet.
            // If it has, catch the exception, remove the menu entry from the global list, and return.
            try
            {
                // Remove all reactions from the current message.
                await message.RemoveAllReactionsAsync();

                // Edit the current active message by replacing it with the recently created embed.
                await message.ModifyAsync(x => {
                    x.Embed = embed.Build();
                    x.Components = component.Build();
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                await message.DeleteAsync();
                await ErrorHandling.PermissionCheck(message);

                // Remove the menu entry from the global list.
                Global.MenuIdList.Remove(menuSession);

                return;
            }

            // Edit the menu session according to the current message.
            menuSession.CurrentMenu = "MakerMulti_1Char_Details_Main";
            menuSession.MenuTimer = new Timer()
            {
                // Create a timer that expires as a "time out" duration for the user.
                Interval = MenuConfig.menu.timerDuration,
                AutoReset = false,
                Enabled = true
            };

            // If the menu timer runs out, activate a function.
            menuSession.MenuTimer.Elapsed += (sender, e) => Utility.MenuTimer_Elapsed(sender, e, menuSession);
        }

        public static async Task MakerMulti_2Char_Details_Main(MenuIdStructure menuSession)
        {
            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(menuSession.User);
            var message = menuSession.MenuMessage;
            var user = menuSession.User;

            var multimaker_session = Global.MultiMaker_Session_List.SingleOrDefault(x => x.User.Id == menuSession.User.Id);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Character Details",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            // Determine the color and thumbnail for the embeded message
            embed.WithColor(EmbedSettings.Get_Game_Color(multimaker_session.MakerCommand.Template, account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo(multimaker_session.MakerCommand.Template));

            string back_button_id = "";

            switch (multimaker_session.MakerCommand.Template)
            {
                case "P2IS-PS1":
                case "P2IS-PSP":
                    back_button_id = "back-to-makermulti-p2is-vc-select";
                    break;

                case "P2EP-PS1":
                case "P2EP-PSP":
                    back_button_id = "back-to-makermulti-p2ep-vc-select";
                    break;

                case "BBTAG":
                    back_button_id = "back-to-makermulti-bbtag-layout-select";
                    break;

                default:
                    back_button_id = "back-to-makermulti-title-select";
                    break;
            }

            embed.WithDescription("" +
                $"Let's get started with your scene! Select [Enter Character Details] to specify Characters #1 & #2 with their sprite numbers.");

            switch (multimaker_session.MakerCommand.Template)
            {
                case "P2IS-PS1":
                    embed.WithImageUrl("https://i.imgur.com/vfmrprZ.png");
                    break;

                case "P2IS-PSP":
                    embed.WithImageUrl("https://i.imgur.com/ossp5cl.png");
                    break;

                case "P2EP-PS1":
                    embed.WithImageUrl("https://i.imgur.com/85Z4cIc.png");
                    break;

                case "P2EP-PSP":
                    embed.WithImageUrl("https://i.imgur.com/oITaVYl.png");
                    break;

                case "P3P":
                    embed.WithImageUrl("https://i.imgur.com/wn1P3sy.png");
                    break;

                case "P4AU":
                    embed.WithImageUrl("https://i.imgur.com/xL7MWrn.png");
                    break;

                case "P4D":
                    embed.WithImageUrl("https://i.imgur.com/mx7hq8T.png");
                    break;

                case "BBTAG":
                    switch (multimaker_session.MakerCommand.BBTAG_Specific_Data.Layout)
                    {
                        case "4":
                            embed.WithImageUrl("https://i.imgur.com/B2V7VjZ.png");
                            break;

                        case "5":
                            embed.WithImageUrl("https://i.imgur.com/pdHXTrI.png");
                            break;

                        case "6":
                            embed.WithImageUrl("https://i.imgur.com/Ca9Eqp6.png");
                            break;

                        case "7":
                            embed.WithImageUrl("https://i.imgur.com/lsfpQDh.png");
                            break;

                        case "8":
                            embed.WithImageUrl("https://i.imgur.com/InejQFE.png");
                            break;

                        case "9":
                            embed.WithImageUrl("https://i.imgur.com/GkAPNhN.png");
                            break;

                        case "10":
                            embed.WithImageUrl("https://i.imgur.com/eHAGpqL.png");
                            break;
                    }
                    break;
            }

            var component = new ComponentBuilder()
                .WithButton("Enter Character Details", customId: "makermulti-2char-details-modal-open", ButtonStyle.Primary)
                .WithButton("↩️ Return", customId: back_button_id, ButtonStyle.Secondary);

            // Attempt editing the message if it hasn't been deleted by the user yet.
            // If it has, catch the exception, remove the menu entry from the global list, and return.
            try
            {
                // Remove all reactions from the current message.
                await message.RemoveAllReactionsAsync();

                // Edit the current active message by replacing it with the recently created embed.
                await message.ModifyAsync(x => {
                    x.Embed = embed.Build();
                    x.Components = component.Build();
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                await message.DeleteAsync();
                await ErrorHandling.PermissionCheck(message);

                // Remove the menu entry from the global list.
                Global.MenuIdList.Remove(menuSession);

                return;
            }

            // Edit the menu session according to the current message.
            menuSession.CurrentMenu = "MakerMulti_2Char_Details_Main";
            menuSession.MenuTimer = new Timer()
            {
                // Create a timer that expires as a "time out" duration for the user.
                Interval = MenuConfig.menu.timerDuration,
                AutoReset = false,
                Enabled = true
            };

            // If the menu timer runs out, activate a function.
            menuSession.MenuTimer.Elapsed += (sender, e) => Utility.MenuTimer_Elapsed(sender, e, menuSession);
        }

        // Modal
        public static async Task MakerMulti_1Char_Details_Modal(SocketMessageComponent component)
        {
            // Get the account information of the command's user.
            if (component.Data.CustomId == "makermulti-1char-details-modal-open")
            {
                try
                {
                    var modal = new ModalBuilder()
                    .WithTitle("Character Details")
                    .WithCustomId("makermulti-1char-details-modal-submit")
                    .AddTextInput("Character", "character_1")
                    .AddTextInput("Sprite number", "sprite_1");

                    await component.RespondWithModalAsync(modal.Build());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public static async Task MakerMulti_2Char_Details_Modal(SocketMessageComponent component)
        {
            // Get the account information of the command's user.
            if (component.Data.CustomId == "makermulti-2char-details-modal-open")
            {
                try
                {
                    var modal = new ModalBuilder()
                    .WithTitle("Character Details")
                    .WithCustomId("makermulti-2char-details-modal-submit")
                    .AddTextInput("Character #1", "character_1")
                    .AddTextInput("Sprite number for Character #1", "sprite_1")
                    .AddTextInput("Character #2", "character_2")
                    .AddTextInput("Sprite number for Character #2", "sprite_2");

                    await component.RespondWithModalAsync(modal.Build());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        // Error Handling
        public static async Task MakerMulti_Char_Details_Pt1_Invalid_Character(SocketGuildUser user, RestUserMessage message, string user_input)
        {
            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            // Find the menu session associated with the current user.
            var menuSession = Global.MenuIdList.SingleOrDefault(x => x.User.Id == user.Id);
            var multimaker_session = Global.MultiMaker_Session_List.SingleOrDefault(x => x.User.Id == menuSession.User.Id);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Invalid Character",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            // Determine the color and thumbnail for the embeded message
            embed.WithColor(EmbedSettings.Get_Game_Color(multimaker_session.MakerCommand.Template, null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo(multimaker_session.MakerCommand.Template));

            embed.WithDescription("" +
                $"There doesn’t seem to be a sprite set with the keyword \"{user_input}\" in {OfficialSetMethods.AcronymToFullTitle(multimaker_session.MakerCommand.Template)}.\n");
            embed.AddField("Tips", "" +
                $"Make sure the character’s keyword is typed correctly and react with ↩️ to try again.");

            var component = new ComponentBuilder()
                .WithButton("↩️ Retry", customId: Get_Back_Button_Id(multimaker_session), ButtonStyle.Secondary);

            // Attempt editing the message if it hasn't been deleted by the user yet.
            // If it has, catch the exception, remove the menu entry from the global list, and return.
            try
            {
                // Remove all reactions from the current message.
                await message.RemoveAllReactionsAsync();

                // Edit the current active message by replacing it with the recently created embed.
                await message.ModifyAsync(x => {
                    x.Embed = embed.Build();
                    x.Components = component.Build();
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                await message.DeleteAsync();
                await ErrorHandling.PermissionCheck(message);

                // Remove the menu entry from the global list.
                Global.MenuIdList.Remove(menuSession);

                return;
            }

            // Edit the menu session according to the current message.
            menuSession.CurrentMenu = "MakerMulti_Char_Details_Pt1_Invalid_Character";
            menuSession.MenuTimer = new Timer()
            {
                // Create a timer that expires as a "time out" duration for the user.
                Interval = MenuConfig.menu.timerDuration,
                AutoReset = false,
                Enabled = true
            };

            // If the menu timer runs out, activate a function.
            menuSession.MenuTimer.Elapsed += (sender, e) => Utility.MenuTimer_Elapsed(sender, e, menuSession);
        }

        public static async Task MakerMulti_Char_Details_Pt1_Invalid_Base_Sprite(SocketGuildUser user, RestUserMessage message, MakerCharacterData character_data)
        {
            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            // Find the menu session associated with the current user.
            var menuSession = Global.MenuIdList.SingleOrDefault(x => x.User.Id == user.Id);
            var multimaker_session = Global.MultiMaker_Session_List.SingleOrDefault(x => x.User.Id == menuSession.User.Id);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Invalid Sprite Number",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            // Determine the color and thumbnail for the embeded message
            embed.WithColor(EmbedSettings.Get_Game_Color(multimaker_session.MakerCommand.Template, null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo(multimaker_session.MakerCommand.Template));

            embed.WithDescription("" +
                $"That sprite number doesn’t seem to be in {character_data.Set_Data.Name}'s sprite set from {OfficialSetMethods.AcronymToFullTitle(multimaker_session.MakerCommand.Template)}.");

            embed.AddField("Tips", "" +
                $"Use the slash command **`maker_sheet`** to view which character sprites are available.");

            var component = new ComponentBuilder()
                .WithButton("↩️ Retry", customId: Get_Back_Button_Id(multimaker_session), ButtonStyle.Secondary);

            // Attempt editing the message if it hasn't been deleted by the user yet.
            // If it has, catch the exception, remove the menu entry from the global list, and return.
            try
            {
                // Remove all reactions from the current message.
                await message.RemoveAllReactionsAsync();

                // Edit the current active message by replacing it with the recently created embed.
                await message.ModifyAsync(x => {
                    x.Embed = embed.Build();
                    x.Components = component.Build();
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                await message.DeleteAsync();
                await ErrorHandling.PermissionCheck(message);

                // Remove the menu entry from the global list.
                Global.MenuIdList.Remove(menuSession);

                return;
            }

            // Edit the menu session according to the current message.
            menuSession.CurrentMenu = "MakerMulti_Char_Details_Pt1_Invalid_Base_Sprite";
            menuSession.MenuTimer = new Timer()
            {
                // Create a timer that expires as a "time out" duration for the user.
                Interval = MenuConfig.menu.timerDuration,
                AutoReset = false,
                Enabled = true
            };

            // If the menu timer runs out, activate a function.
            menuSession.MenuTimer.Elapsed += (sender, e) => Utility.MenuTimer_Elapsed(sender, e, menuSession);
        }

        public static async Task MakerMulti_Char_Details_Pt1_Sprite_Select_Too_Many_Animation_Frames(SocketGuildUser user, RestUserMessage message, MakerCharacterData character_data)
        {
            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            // Find the menu session associated with the current user.
            var menuSession = Global.MenuIdList.SingleOrDefault(x => x.User.Id == user.Id);
            var multimaker_session = Global.MultiMaker_Session_List.SingleOrDefault(x => x.User.Id == menuSession.User.Id);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Too Many Animation Frames",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            // Determine the color and thumbnail for the embeded message
            embed.WithColor(EmbedSettings.Get_Game_Color(multimaker_session.MakerCommand.Template, null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo(multimaker_session.MakerCommand.Template));

            embed.WithDescription($"There seems to be more digits than needed for specifying animation frames\n");
            embed.AddField("Tips", "" +
                "Start with the base sprite number first, then connect an eye frame number to it with a hyphen. If the character sprite also has mouth frames, connect it after the eye frame number with a hyphen, too.");

            var component = new ComponentBuilder()
                .WithButton("↩️ Retry", customId: Get_Back_Button_Id(multimaker_session), ButtonStyle.Secondary);

            // Attempt editing the message if it hasn't been deleted by the user yet.
            // If it has, catch the exception, remove the menu entry from the global list, and return.
            try
            {
                // Remove all reactions from the current message.
                await message.RemoveAllReactionsAsync();

                // Edit the current active message by replacing it with the recently created embed.
                await message.ModifyAsync(x => {
                    x.Embed = embed.Build();
                    x.Components = component.Build();
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                await message.DeleteAsync();
                await ErrorHandling.PermissionCheck(message);

                // Remove the menu entry from the global list.
                Global.MenuIdList.Remove(menuSession);

                return;
            }

            // Edit the menu session according to the current message.
            menuSession.CurrentMenu = "MakerMulti_Char_Details_Pt1_Sprite_Select_Too_Many_Animation_Frames";
            menuSession.MenuTimer = new Timer()
            {
                // Create a timer that expires as a "time out" duration for the user.
                Interval = MenuConfig.menu.timerDuration,
                AutoReset = false,
                Enabled = true
            };

            // If the menu timer runs out, activate a function.
            menuSession.MenuTimer.Elapsed += (sender, e) => Utility.MenuTimer_Elapsed(sender, e, menuSession);
        }

        public static async Task MakerMulti_Char_Details_Pt1_Sprite_Select_Non_Digit_In_Sprite_Number(SocketGuildUser user, RestUserMessage message, MakerCharacterData character_data)
        {
            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            // Find the menu session associated with the current user.
            var menuSession = Global.MenuIdList.SingleOrDefault(x => x.User.Id == user.Id);
            var multimaker_session = Global.MultiMaker_Session_List.SingleOrDefault(x => x.User.Id == menuSession.User.Id);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Incorrect Syntax",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            // Determine the color and thumbnail for the embeded message
            embed.WithColor(EmbedSettings.Get_Game_Color(multimaker_session.MakerCommand.Template, null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo(multimaker_session.MakerCommand.Template));

            embed.WithDescription($"A non-digit was found when specifying the animation frames for {character_data.Set_Data.Name}.");
            embed.AddField("Tips", "" +
                "Start with the base sprite number first, then connect an eye frame number to it with a hyphen. If the character sprite also has mouth frames, connect it after the eye frame number with a hyphen, too.");

            var component = new ComponentBuilder()
                .WithButton("↩️ Retry", customId: Get_Back_Button_Id(multimaker_session), ButtonStyle.Secondary);

            // Attempt editing the message if it hasn't been deleted by the user yet.
            // If it has, catch the exception, remove the menu entry from the global list, and return.
            try
            {
                // Remove all reactions from the current message.
                await message.RemoveAllReactionsAsync();

                // Edit the current active message by replacing it with the recently created embed.
                await message.ModifyAsync(x => {
                    x.Embed = embed.Build();
                    x.Components = component.Build();
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                await message.DeleteAsync();
                await ErrorHandling.PermissionCheck(message);

                // Remove the menu entry from the global list.
                Global.MenuIdList.Remove(menuSession);

                return;
            }

            // Edit the menu session according to the current message.
            menuSession.CurrentMenu = "MakerMulti_Char_Details_Pt1_Sprite_Select_Non_Digit_In_Sprite_Number";
            menuSession.MenuTimer = new Timer()
            {
                // Create a timer that expires as a "time out" duration for the user.
                Interval = MenuConfig.menu.timerDuration,
                AutoReset = false,
                Enabled = true
            };

            // If the menu timer runs out, activate a function.
            menuSession.MenuTimer.Elapsed += (sender, e) => Utility.MenuTimer_Elapsed(sender, e, menuSession);
        }

        public static async Task MakerMulti_Char_Details_Pt1_Sprite_Select_Animation_Frame_With_Blank_Sprite(SocketGuildUser user, RestUserMessage message, MakerCharacterData character_data)
        {
            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            // Find the menu session associated with the current user.
            var menuSession = Global.MenuIdList.SingleOrDefault(x => x.User.Id == user.Id);
            var multimaker_session = Global.MultiMaker_Session_List.SingleOrDefault(x => x.User.Id == menuSession.User.Id);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Incorrect Syntax",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            // Determine the color and thumbnail for the embeded message
            embed.WithColor(EmbedSettings.Get_Game_Color(multimaker_session.MakerCommand.Template, null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo(multimaker_session.MakerCommand.Template));

            embed.WithDescription($"Animation frames can’t be used for {character_data.Set_Data.Name} if the sprite number is 0.");
            embed.AddField("Tips", "" +
                $"Check which animation frames are available for the character and react with ↩️ to try again.\n");

            var component = new ComponentBuilder()
                .WithButton("↩️ Retry", customId: Get_Back_Button_Id(multimaker_session), ButtonStyle.Secondary);

            // Attempt editing the message if it hasn't been deleted by the user yet.
            // If it has, catch the exception, remove the menu entry from the global list, and return.
            try
            {
                // Remove all reactions from the current message.
                await message.RemoveAllReactionsAsync();

                // Edit the current active message by replacing it with the recently created embed.
                await message.ModifyAsync(x => {
                    x.Embed = embed.Build();
                    x.Components = component.Build();
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                await message.DeleteAsync();
                await ErrorHandling.PermissionCheck(message);

                // Remove the menu entry from the global list.
                Global.MenuIdList.Remove(menuSession);

                return;
            }

            // Edit the menu session according to the current message.
            menuSession.CurrentMenu = "MakerMulti_Char_Details_Pt1_Sprite_Select_Animation_Frame_With_Blank_Sprite";
            menuSession.MenuTimer = new Timer()
            {
                // Create a timer that expires as a "time out" duration for the user.
                Interval = MenuConfig.menu.timerDuration,
                AutoReset = false,
                Enabled = true
            };

            // If the menu timer runs out, activate a function.
            menuSession.MenuTimer.Elapsed += (sender, e) => Utility.MenuTimer_Elapsed(sender, e, menuSession);
        }

        public static async Task MakerMulti_Char_Details_Pt1_Sprite_Select_Eye_Frame_Not_Found(SocketGuildUser user, RestUserMessage message, MakerCharacterData character_data)
        {
            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            // Find the menu session associated with the current user.
            var menuSession = Global.MenuIdList.SingleOrDefault(x => x.User.Id == user.Id);
            var multimaker_session = Global.MultiMaker_Session_List.SingleOrDefault(x => x.User.Id == menuSession.User.Id);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Eye Frame Not Found",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            // Determine the color and thumbnail for the embeded message
            embed.WithColor(EmbedSettings.Get_Game_Color(multimaker_session.MakerCommand.Template, null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo(multimaker_session.MakerCommand.Template));

            embed.WithDescription($"That eye frame doesn’t seem to be part of {character_data.Bustup_Data.Default_Name_EN}'s {character_data.Base_Sprite}{SceneMaker.ErrorHandling.Number_Suffix(character_data.Base_Sprite)} {OfficialSetMethods.AcronymToFullTitle(character_data.Set_Data.Origin)} sprite.");
            embed.AddField("Tips", "" +
                $"Check which animation frames are available for the character and react with ↩️ to try again.\n");

            var component = new ComponentBuilder()
                .WithButton("↩️ Retry", customId: Get_Back_Button_Id(multimaker_session), ButtonStyle.Secondary);

            // Attempt editing the message if it hasn't been deleted by the user yet.
            // If it has, catch the exception, remove the menu entry from the global list, and return.
            try
            {
                // Remove all reactions from the current message.
                await message.RemoveAllReactionsAsync();

                // Edit the current active message by replacing it with the recently created embed.
                await message.ModifyAsync(x => {
                    x.Embed = embed.Build();
                    x.Components = component.Build();
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                await message.DeleteAsync();
                await ErrorHandling.PermissionCheck(message);

                // Remove the menu entry from the global list.
                Global.MenuIdList.Remove(menuSession);

                return;
            }

            // Edit the menu session according to the current message.
            menuSession.CurrentMenu = "MakerMulti_Char_Details_Pt1_Sprite_Select_Eye_Frame_Not_Found";
            menuSession.MenuTimer = new Timer()
            {
                // Create a timer that expires as a "time out" duration for the user.
                Interval = MenuConfig.menu.timerDuration,
                AutoReset = false,
                Enabled = true
            };

            // If the menu timer runs out, activate a function.
            menuSession.MenuTimer.Elapsed += (sender, e) => Utility.MenuTimer_Elapsed(sender, e, menuSession);
        }

        public static async Task MakerMulti_Char_Details_Pt1_Sprite_Select_Mouth_Frame_Not_Found(SocketGuildUser user, RestUserMessage message, MakerCharacterData character_data)
        {
            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            // Find the menu session associated with the current user.
            var menuSession = Global.MenuIdList.SingleOrDefault(x => x.User.Id == user.Id);
            var multimaker_session = Global.MultiMaker_Session_List.SingleOrDefault(x => x.User.Id == menuSession.User.Id);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Mouth Frame Not Found",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            // Determine the color and thumbnail for the embeded message
            embed.WithColor(EmbedSettings.Get_Game_Color(multimaker_session.MakerCommand.Template, null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo(multimaker_session.MakerCommand.Template));

            embed.WithDescription($"That mouth frame doesn’t seem to be part of {character_data.Bustup_Data.Default_Name_EN}'s {character_data.Base_Sprite}{SceneMaker.ErrorHandling.Number_Suffix(character_data.Base_Sprite)} {OfficialSetMethods.AcronymToFullTitle(character_data.Set_Data.Origin)} sprite.");
            embed.AddField("Tips", "" +
                $"Check which animation frames are available for the character and react with ↩️ to try again.\n");

            var component = new ComponentBuilder()
                .WithButton("↩️ Retry", customId: Get_Back_Button_Id(multimaker_session), ButtonStyle.Secondary);

            // Attempt editing the message if it hasn't been deleted by the user yet.
            // If it has, catch the exception, remove the menu entry from the global list, and return.
            try
            {
                // Remove all reactions from the current message.
                await message.RemoveAllReactionsAsync();

                // Edit the current active message by replacing it with the recently created embed.
                await message.ModifyAsync(x => {
                    x.Embed = embed.Build();
                    x.Components = component.Build();
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                await message.DeleteAsync();
                await ErrorHandling.PermissionCheck(message);

                // Remove the menu entry from the global list.
                Global.MenuIdList.Remove(menuSession);

                return;
            }

            // Edit the menu session according to the current message.
            menuSession.CurrentMenu = "MakerMulti_Char_Details_Pt1_Sprite_Select_Mouth_Frame_Not_Found";
            menuSession.MenuTimer = new Timer()
            {
                // Create a timer that expires as a "time out" duration for the user.
                Interval = MenuConfig.menu.timerDuration,
                AutoReset = false,
                Enabled = true
            };

            // If the menu timer runs out, activate a function.
            menuSession.MenuTimer.Elapsed += (sender, e) => Utility.MenuTimer_Elapsed(sender, e, menuSession);
        }
    }
}
