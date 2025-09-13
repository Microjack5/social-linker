using Discord;
using SocialLinker.Config;
using SocialLinker.Core.CloudStorageTables;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace SocialLinker.Core.Menus.MakerMulti.Main
{
    class MakerMulti_BBTAG_Speaker_Menu
    {
        public static async Task MakerMulti_BBTAG_Speaker_Main(MenuIdStructure menuSession)
        {
            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(menuSession.User);
            var message = menuSession.MenuMessage;
            var user = menuSession.User;

            var multimaker_session = Global.MultiMaker_Session_List.SingleOrDefault(x => x.User.Id == menuSession.User.Id);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Speaker Select",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            embed.WithColor(EmbedSettings.Get_Game_Color("BBTAG", account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("BBTAG"));

            embed.WithDescription("" +
                "Who's speaking in the scene?\n");

            var selectMenu = new SelectMenuBuilder()
                    .WithPlaceholder("Select a speaker")
                    .WithCustomId("multimaker-bbtag-speaker")
                    .WithMinValues(1)
                    .WithMaxValues(1);

            try
            {
                if (multimaker_session.MakerCommand.Expected_Characters >= 1)
                {
                    selectMenu.AddOption($"" +
                        $"{multimaker_session.MakerCommand.Character_Data_1.Set_Data.Name}", "char_1",
                        emote: Emote.Parse(Utility.BBTAG_Series_To_Emote(multimaker_session.MakerCommand.Character_Data_1.Set_Data.Series)));
                }

                if (multimaker_session.MakerCommand.Expected_Characters >= 2)
                {
                    selectMenu.AddOption($"" +
                        $"{multimaker_session.MakerCommand.Character_Data_2.Set_Data.Name}", "char_2",
                        emote: Emote.Parse(Utility.BBTAG_Series_To_Emote(multimaker_session.MakerCommand.Character_Data_2.Set_Data.Series)));
                }

                if (multimaker_session.MakerCommand.Expected_Characters >= 3)
                {
                    selectMenu.AddOption($"" +
                        $"{multimaker_session.MakerCommand.Character_Data_3.Set_Data.Name}", "char_3",
                        emote: Emote.Parse(Utility.BBTAG_Series_To_Emote(multimaker_session.MakerCommand.Character_Data_3.Set_Data.Series)));
                }

                if (multimaker_session.MakerCommand.Expected_Characters >= 4)
                {
                    selectMenu.AddOption($"" +
                        $"{multimaker_session.MakerCommand.Character_Data_4.Set_Data.Name}", "char_4",
                        emote: Emote.Parse(Utility.BBTAG_Series_To_Emote(multimaker_session.MakerCommand.Character_Data_4.Set_Data.Series)));
                }

                selectMenu
                    .AddOption("System XX", "system_1", null, new Emoji("🔹"))
                    .AddOption("System XX (Sentient)", "system_2", null, new Emoji("🔹"))
                    .AddOption("Someone off-screen", "offscreen", null, new Emoji("❔"));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            var component = new ComponentBuilder()
                .WithSelectMenu(selectMenu);

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
            menuSession.CurrentMenu = "MakerMulti_BBTAG_Speaker_Main";
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

        public static async Task MakerMulti_BBTAG_Offscreen_Speaker_Main(MenuIdStructure menuSession)
        {
            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(menuSession.User);
            var message = menuSession.MenuMessage;
            var user = menuSession.User;

            var multimaker_session = Global.MultiMaker_Session_List.SingleOrDefault(x => x.User.Id == menuSession.User.Id);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Off-screen Speaker",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            embed.WithColor(EmbedSettings.Get_Game_Color("BBTAG", account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("BBTAG"));

            embed.WithDescription("" +
                "Which series does the off-screen speaker come from? We'll use this to color their nametag.\n");

            var selectMenu = new SelectMenuBuilder()
                    .WithPlaceholder("Select a series")
                    .WithCustomId("multimaker-bbtag-offscreen-speaker-series")
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .AddOption("BlazBlue", "BlazBlue", emote: Emote.Parse(Utility.BBTAG_Series_To_Emote("BlazBlue")))
                    .AddOption("Persona 4 Arena", "Persona 4 Arena", emote: Emote.Parse(Utility.BBTAG_Series_To_Emote("Persona 4 Arena")))
                    .AddOption("Under Night In-Birth", "Under Night In-Birth", emote: Emote.Parse(Utility.BBTAG_Series_To_Emote("Under Night In-Birth")))
                    .AddOption("RWBY", "RWBY", emote: Emote.Parse(Utility.BBTAG_Series_To_Emote("RWBY")))
                    .AddOption("Arcana Heart", "Arcana Heart", emote: Emote.Parse(Utility.BBTAG_Series_To_Emote("Arcana Heart")))
                    .AddOption("Senran Kagura", "Senran Kagura", emote: Emote.Parse(Utility.BBTAG_Series_To_Emote("Senran Kagura")))
                    .AddOption("Akatsuki En-Eins", "Akatsuki En-Eins", emote: Emote.Parse(Utility.BBTAG_Series_To_Emote("Akatsuki En-Eins")))
                    .AddOption("None", "none", null, new Emoji("❔"));

            var component = new ComponentBuilder()
                .WithSelectMenu(selectMenu);

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
            menuSession.CurrentMenu = "MakerMulti_BBTAG_Offscreen_Speaker_Main";
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
