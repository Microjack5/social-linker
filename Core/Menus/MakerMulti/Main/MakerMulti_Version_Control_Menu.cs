using Discord;
using SocialLinker.Config;
using SocialLinker.Core.CloudStorageTables;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace SocialLinker.Core.Menus.MakerMulti.Main
{
    class MakerMulti_Version_Control_Menu
    {
        public static async Task MakerMulti_VC_P2IS_Main(MenuIdStructure menuSession)
        {
            var account = UserInfoClasses.GetAccount(menuSession.User);
            var message = menuSession.MenuMessage;
            var user = menuSession.User;

            var multimaker_session = Global.MultiMaker_Session_List.SingleOrDefault(x => x.User.Id == menuSession.User.Id);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Version Control",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);
            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));

            embed.WithDescription("" +
                "Which version of Persona 2: Innocent Sin would you like to use?\n");

            embed.WithImageUrl("https://i.imgur.com/6Utgced.png");

            // Attempt editing the message if it hasn't been deleted by the user yet.
            // If it has, catch the exception, remove the menu entry from the global list, and return.
            try
            {
                // Remove all reactions from the current message.
                await message.RemoveAllReactionsAsync();

                // Build the select menu
                var selectMenu = new SelectMenuBuilder()
                    .WithPlaceholder("Select a version")
                    .WithCustomId("multimaker-p2is-vc")
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .AddOption("Persona 2: Innocent Sin (PlayStation®️)", "P2IS-PS1", emote: Emote.Parse("<:P2IS:788950080396328990>"))
                    .AddOption("Persona 2: Innocent Sin (PSP®️)", "P2IS-PSP", emote: Emote.Parse("<:P2IS:788950080396328990>"));

                var component = new ComponentBuilder()
                    .WithSelectMenu(selectMenu);

                // Modify the message with the embed and the select menu
                await message.ModifyAsync(msg =>
                {
                    msg.Embed = embed.Build();
                    msg.Components = component.Build();
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
            menuSession.CurrentMenu = "MakerMulti_VC_P2IS_Main";
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

        public static async Task MakerMulti_VC_P2EP_Main(MenuIdStructure menuSession)
        {
            var account = UserInfoClasses.GetAccount(menuSession.User);
            var message = menuSession.MenuMessage;
            var user = menuSession.User;

            var multimaker_session = Global.MultiMaker_Session_List.SingleOrDefault(x => x.User.Id == menuSession.User.Id);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Version Control",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);
            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));

            embed.WithDescription("" +
                "Which version of Persona 2: Eternal Punishment would you like to use?\n");

            embed.WithImageUrl("https://i.imgur.com/JAZN3dP.png");

            // Attempt editing the message if it hasn't been deleted by the user yet.
            // If it has, catch the exception, remove the menu entry from the global list, and return.
            try
            {
                // Remove all reactions from the current message.
                await message.RemoveAllReactionsAsync();

                // Build the select menu
                var selectMenu = new SelectMenuBuilder()
                    .WithPlaceholder("Select a version")
                    .WithCustomId("multimaker-p2ep-vc")
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .AddOption("Persona 2: Eternal Punishment (PlayStation®️)", "P2EP-PS1", emote: Emote.Parse("<:P2EP:788950163363463172>"))
                    .AddOption("Persona 2: Eternal Punishment (PSP®️)", "P2EP-PSP", emote: Emote.Parse("<:P2EP:788950163363463172>"));

                var component = new ComponentBuilder()
                    .WithSelectMenu(selectMenu);

                // Modify the message with the embed and the select menu
                await message.ModifyAsync(msg =>
                {
                    msg.Embed = embed.Build();
                    msg.Components = component.Build();
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
            menuSession.CurrentMenu = "MakerMulti_VC_P2EP_Main";
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
