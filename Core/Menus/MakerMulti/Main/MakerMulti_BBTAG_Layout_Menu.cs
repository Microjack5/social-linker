using Discord;
using SocialLinker.Config;
using SocialLinker.Core.CloudStorageTables;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

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
                "Choose how you'd like the characters to be placed in the scene.\n");

            embed.WithImageUrl("https://i.imgur.com/enrWkjK.png");

            var selectMenu = new SelectMenuBuilder()
                    .WithPlaceholder("Select a layout")
                    .WithCustomId("multimaker-bbtag-layout")
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .AddOption("One character left", "1", null, new Emoji("1️⃣"))
                    .AddOption("One character right", "2", null, new Emoji("2️⃣"))
                    .AddOption("One character centered", "3", null, new Emoji("3️⃣"))
                    .AddOption("One character left + One character right", "4", null, new Emoji("4️⃣"))
                    .AddOption("Tag team left", "5", null, new Emoji("5️⃣"))
                    .AddOption("Tag team right", "6", null, new Emoji("6️⃣"))
                    .AddOption("Tag team centered", "7", null, new Emoji("7️⃣"))
                    .AddOption("One character left + Tag team right", "8", null, new Emoji("8️⃣"))
                    .AddOption("Tag team left + One character right", "9", null, new Emoji("9️⃣"))
                    .AddOption("Tag team left + Tag team right", "10", null, new Emoji("🔟"));

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
