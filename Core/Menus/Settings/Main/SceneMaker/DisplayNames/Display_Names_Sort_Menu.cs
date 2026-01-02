using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Rest;
using SocialLinker.Core.Menus.Settings.Reactions.SceneMaker.DisplayNames;

namespace SocialLinker.Core.Menus.Settings.Main.SceneMaker.DisplayNames
{
    internal class Display_Names_Sort_Menu
    {
        public static async Task Display_Names_Sort(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var itemSession = Global.ItemIdList.SingleOrDefault(x => x.User.Id == user.Id);

            // Create a new embed that will be displayed in the message.
            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Sort Display Names",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            embed.WithDescription("" +
                "**Choose a method to sort display names entries by.**\n" +
                "\n"+
                $"⚙️ **Current Setting:** **`{Display_Names_Sort_Reactions.SortSettingToString(account.Display_Names_Sort)}`**\n" +
                $"\n" +
                $":one: By Oldest to Newest\n" +
                $":two: By Newest to Oldest\n" +
                $":three: By Display Name (A - Z)\n" +
                $":four: By Display Name (Z - A)\n" +
                $":five: By Title\n");

            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));

            // Attempt deleting the message if it hasn't been deleted by the user yet.
            try
            {
                // Delete the current message from the channel.
                await message.DeleteAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            // If the bot lacks permission to send messages, catch the exception and return.
            try
            {
                // Reassign the menu session's message to a new message generated from the created embed.
                menuSession.MenuMessage = (RestUserMessage)await message.Channel.SendMessageAsync("", false, embed.Build());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return;
            }

            // Set the "message" variable to the menu session's message.
            message = menuSession.MenuMessage;

            // Edit the menu session according to the current message.
            menuSession.CurrentMenu = "Display_Names_Sort";

            var selectMenu = new SelectMenuBuilder()
                    .WithPlaceholder("Select an option")
                    .WithCustomId("shop-sort-select")
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .AddOption("By Oldest to Newest", "1", null, new Emoji("1️⃣"))
                    .AddOption("By Newest to Oldest", "2", null, new Emoji("2️⃣"))
                    .AddOption("By Display Name (A - Z)", "3", null, new Emoji("3️⃣"))
                    .AddOption("By Display Name (Z - A)", "4", null, new Emoji("4️⃣"))
                    .AddOption("By Title", "5", null, new Emoji("5️⃣"));

            var component = new ComponentBuilder()
                .WithSelectMenu(selectMenu)
                .WithButton("↩️ Return", customId: "return", ButtonStyle.Secondary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Display_Names_Sort_Confirm(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var itemSession = Global.ItemIdList.SingleOrDefault(x => x.User.Id == user.Id);

            // Create a new embed that will be displayed in the message.
            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Settings Saved",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            embed.WithDescription($"Display names will now be sorted **`{Display_Names_Sort_Reactions.SortSettingToString(account.Display_Names_Sort)}`**.");

            // Determine the color and thumbnail for the embeded message.
            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));

            menuSession.CurrentMenu = "Display_Names_Sort_Confirm";

            var component = new ComponentBuilder()
                .WithButton("💠 Display Names", customId: "display-names", ButtonStyle.Secondary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }
    }
}
