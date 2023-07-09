using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using SocialLinker.Config;
using SocialLinker.Core.CloudStorageTables;
using Discord.Rest;

namespace SocialLinker.Core.Menus.Help.Main
{
    class Credits_Menu
    {
        public static async Task Credits_Page_1(SocketGuildUser user, RestUserMessage message)
        {
            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            // Find the menu session associated with the current user.
            var menuSession = Global.MenuIdList.SingleOrDefault(x => x.User.Id == user.Id);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Credits",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            var footer = new EmbedFooterBuilder
            {
                Text = "↩️ Return to Help Menu | ▶️ Next Page\n" +
                "Page 1 / 2"
            };

            embed.WithFooter(footer);

            // Determine the color and thumbnail for the embeded message
            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Profile_Help_Thumbnail(account));

            embed.AddField("Programming & Design", "" +
                "[Microjack5](https://discord.com/users/222504679878164481/)\n");

            embed.AddField("Quality Assurance Advisors", "" +
                "[Arkane](https://discord.com/users/208779984276291585/)\n" +
                "[Ash!!](https://discord.com/users/442253411077849100/)\n" +
                "[astronights](https://discord.com/users/315671417679118337/)\n" +
                "[Azure](https://discord.com/users/328963190966714369/)\n" +
                "[Camz](https://discord.com/users/345577063295614977/)\n" +
                "[genesisdreams](https://discord.com/users/349683994222395393/)\n" +
                "[Naanos](https://discord.com/users/690720929214496819/)\n" +
                "[無限 | Nate](https://discord.com/users/140846765275348993/)\n" +
                "[poi](https://discord.com/users/800614229865922570/)\n" +
                "[quiche](https://discord.com/users/707398527575130162/)\n" +
                "[RomIsALemon](https://discord.com/users/239519485822894080/)\n" +
                "[Shadow Kawa](https://discord.com/users/210080634498973696/)\n" +
                "[SlimePupAribaba](https://discord.com/users/418035664085450755/)\n" +
                "[Squishy](https://discord.com/users/284351113984081922/)\n" +
                "[tairitsu](https://discord.com/users/560255071749668867/)\n" +
                "[Thena](https://discord.com/users/434019013572427778/)\n" +
                "[WaffleBandito](https://discord.com/users/407300235065425921/)\n");

            embed.AddField("Asset Advisors", "" +
                "[80constant](https://discord.com/users/593323748883562496/)\n" +
                "[Arkane](https://discord.com/users/208779984276291585/)\n" +
                "[Canasniimehugh](https://www.vg-resource.com/user-17021.html)\n" +
                "[Eiowlta](https://discord.com/users/126051543794450432/)\n" +
                "[EsperKnight](https://twitter.com/esperknight)\n" +
                "[Geordan9](https://github.com/Geordan9)\n" +
                "[Oliviayellowcat](https://discord.com/users/366986062599290883/)\n");

            embed.AddField("Status Décor Designers", "" +
                "[Microjack5](https://discord.com/users/222504679878164481/)\n" +
                "[無限 | Nate](https://discord.com/users/140846765275348993/)\n");

            // Attempt editing the message if it hasn't been deleted by the user yet.
            // If it has, catch the exception, remove the menu entry from the global list, and return.
            try
            {
                // Remove all reactions from the current message.
                await message.RemoveAllReactionsAsync();

                // Edit the current active message by replacing it with the recently created embed.
                await message.ModifyAsync(x => {
                    x.Embed = embed.Build();
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
            menuSession.CurrentMenu = "Credits_Page_1";
            menuSession.MenuTimer = new Timer()
            {
                // Create a timer that expires as a "time out" duration for the user.
                Interval = MenuConfig.menu.timerDuration,
                AutoReset = false,
                Enabled = true
            };

            // If the menu timer runs out, activate a function.
            menuSession.MenuTimer.Elapsed += (sender, e) => MenuTimer_Elapsed(sender, e, menuSession);

            // Create an empty list for reactions.
            List<IEmote> reaction_list = new List<IEmote> { };

            // Add needed emote reactions for the menu.
            reaction_list.Add(new Emoji("↩️"));
            reaction_list.Add(new Emoji("▶️"));

            // Add the reactions to the message.
            _ = ReactionHandling.AddReactionsToMenu(message, reaction_list);
        }

        public static async Task Credits_Page_2(SocketGuildUser user, RestUserMessage message)
        {
            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            // Find the menu session associated with the current user.
            var menuSession = Global.MenuIdList.SingleOrDefault(x => x.User.Id == user.Id);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Credits",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            var footer = new EmbedFooterBuilder
            {
                Text = "" +
                "◀️ Previous Page | 💠 Return to Help Menu\n" +
                "Page 2 / 2"
            };

            embed.WithFooter(footer);

            // Determine the color and thumbnail for the embeded message
            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Profile_Help_Thumbnail(account));

            embed.AddField("Gameplay Footage", "" +
                "[BuffMaister](https://www.youtube.com/channel/UCks_VIIleZT2iDWNipPglUg)\n" +
                "[Faz](https://www.youtube.com/channel/UCEevYX4rCcfF0ZrxmnnONXA)\n" +
                "[Ignis](https://www.youtube.com/channel/UCHViTnm0pNN3BwvOwGqlPgQ)\n" +
                "[JohneAwesome](https://www.youtube.com/user/JohneAwesome)\n" +
                "[Literally Satan GAMING](https://www.youtube.com/channel/UCfdQp9SVfAMQEtD3jQAoXLg)\n" +
                "[Noire Blue](https://www.youtube.com/channel/UCUZpzh41JoA4bbgfQL1hx7A)\n" +
                "[PuppiStation](https://www.youtube.com/channel/UCv3PDRDC9cRw9Yzgb_NzgYg)\n" +
                "[RandomPl0x](https://www.youtube.com/c/RandomChannelPlox)\n" +
                "[Shirrako](https://www.youtube.com/channel/UC7eAfUjR9gdIjoaoQaS0W-A)\n");

            embed.AddField("Services", "" +
                "[Amazon Web Services](https://aws.amazon.com/)\n" +
                "[Microsoft Azure](https://azure.microsoft.com/)\n" +
                "[Weather API](https://www.weatherapi.com/)\n");

            embed.AddField("Special Thanks", "" +
                "[Joseph Navarro](https://github.com/josephnavarro)\n" +
                "[Meloman19](https://github.com/Meloman19)\n" +
                "[Petr Sedláček](https://github.com/petrspelos)\n" +
                "[ShrineFox](https://shrinefox.com/)\n");

            // Attempt editing the message if it hasn't been deleted by the user yet.
            // If it has, catch the exception, remove the menu entry from the global list, and return.
            try
            {
                // Remove all reactions from the current message.
                await message.RemoveAllReactionsAsync();

                // Edit the current active message by replacing it with the recently created embed.
                await message.ModifyAsync(x => {
                    x.Embed = embed.Build();
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
            menuSession.CurrentMenu = "Credits_Page_2";
            menuSession.MenuTimer = new Timer()
            {
                // Create a timer that expires as a "time out" duration for the user.
                Interval = MenuConfig.menu.timerDuration,
                AutoReset = false,
                Enabled = true
            };

            // If the menu timer runs out, activate a function.
            menuSession.MenuTimer.Elapsed += (sender, e) => MenuTimer_Elapsed(sender, e, menuSession);

            // Create an empty list for reactions.
            List<IEmote> reaction_list = new List<IEmote> { };

            // Add needed emote reactions for the menu.
            reaction_list.Add(new Emoji("◀️"));
            reaction_list.Add(new Emoji("💠"));

            // Add the reactions to the message.
            _ = ReactionHandling.AddReactionsToMenu(message, reaction_list);
        }

        private static async void MenuTimer_Elapsed(object sender, ElapsedEventArgs e, MenuIdStructure menuSession)
        {
            // Assign the menu session's message to another variable.
            var message = menuSession.MenuMessage;

            // Attempt editing the message if it hasn't been deleted by the user yet.
            // If it has, catch the exception, remove the menu entry from the global list, and return.
            try
            {
                // Remove all reactions from the current message.
                await message.RemoveAllReactionsAsync();

                // Edit the current active message by replacing it with the recently created embed.
                await message.ModifyAsync(x => {
                    x.Embed = MenuTimedOut(menuSession.User).Build();
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                // Remove the menu entry from the global list.
                Global.MenuIdList.Remove(menuSession);

                return;
            }

            // Remove the menu entry from the global list.
            Global.MenuIdList.Remove(menuSession);
        }

        public static EmbedBuilder MenuTimedOut(SocketGuildUser user)
        {
            // Get the account information of the command's target
            var account = UserInfoClasses.GetAccount(user);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Inactive Menu",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            // Determine the color and thumbnail for the embeded message
            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Profile_Help_Thumbnail(account));

            embed.WithDescription($"You can access the help menu at any time with the **`{BotConfig.bot.cmdPrefix}help`** command.");
            return embed;
        }
    }
}
