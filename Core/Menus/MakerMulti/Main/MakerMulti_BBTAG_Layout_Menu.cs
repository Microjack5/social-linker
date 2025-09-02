using Discord.Rest;
using Discord;
using Discord.WebSocket;
using SocialLinker.Config;
using SocialLinker.Core.CloudStorageTables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using SocialLinker.Core.SceneMaker;

namespace SocialLinker.Core.Menus.MakerMulti.Main
{
    class MakerMulti_BBTAG_Layout_Menu
    {
        public static async Task MakerMulti_BBTAG_Layout_Main(MenuIdStructure menuSession)
        {
            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(menuSession.User);
            var message = menuSession.MenuMessage;
            var user = menuSession.User;

            var multimaker_session = Global.MultiMaker_Session_List.SingleOrDefault(x => x.User.Id == menuSession.User.Id);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Scene Layout",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            embed.WithColor(EmbedSettings.Get_Game_Color("BBTAG", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("BBTAG"));

            embed.WithDescription("" +
                "Choose the layout of the scene.\n");

            embed.WithImageUrl("https://i.imgur.com/H1dJ2aU");

            var selectMenu = new SelectMenuBuilder()
                    .WithPlaceholder("Select a layout")
                    .WithCustomId("multimaker-bbtag-layout")
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .AddOption("Layout 1", "1")
                    .AddOption("Layout 2", "2")
                    .AddOption("Layout 3", "3")
                    .AddOption("Layout 4", "4")
                    .AddOption("Layout 5", "5")
                    .AddOption("Layout 6", "6")
                    .AddOption("Layout 7", "7")
                    .AddOption("Layout 8", "8")
                    .AddOption("Layout 9", "9");

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
            menuSession.CurrentMenu = "MakerMulti_BBTAG_Layout_Main";
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
