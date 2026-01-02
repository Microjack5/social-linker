using Discord;
using Discord.Rest;
using Discord.WebSocket;
using SocialLinker.Config;
using SocialLinker.Core.CloudStorageTables;
using System;
using System.Threading.Tasks;
using System.Timers;

namespace SocialLinker.Core.Menus.Help.Main
{
    class Help_Menu
    {
        public static async Task Help_Start(SocketTextChannel channel, SocketGuildUser user)
        {
            //Get the account information of the command's target
            var account = UserInfoClasses.GetAccount(user);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Now Loading...",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            //Determine color for embeded message
            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));

            // Create a null variable for the message.
            RestUserMessage message = null;

            // Try to send a message to the channel. If the bot lacks permissions, catch the exception and return.
            try
            {
                message = await channel.SendMessageAsync("", false, embed.Build());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return;
            }

            // Create a new menu identifier entry for this current message and user to keep track of the overall menu status.
            var menuSession = new MenuIdStructure()
            {
                User = user,
                Account = account,
                MenuMessage = message,
                CurrentMenu = "Help_Start",
                MenuTimer = new Timer()
                {
                    // Create a timer that expires as a "time out" duration for the user.
                    Interval = MenuConfig.menu.timerDuration,
                    AutoReset = false,
                    Enabled = true
                },
                InactiveMessage = "You can access the help menu at any time with the **`help`** command."
            };

            // Add the menu entry to the global list.
            Global.MenuIdList.Add(menuSession);

            // Create a new menu in the current channel.
            await Help_Main_Menu(menuSession);
        }

        public static async Task Help_Main_Menu(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Social Linker Help",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            var footer = new EmbedFooterBuilder
            {
                Text = "React with ❌ to close any menu"
            };

            embed.WithFooter(footer);

            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Profile_Help_Thumbnail(account));

            embed.WithDescription(
                "> **General Commands**\n" +
                $"`help`\n" +
                $"`settings`\n" +
                "\n" +
                "> **Social Commands**\n" +
                $"`hug [user]`\n" +
                $"`pat [user]`\n" +
                $"`slap [user]`\n" +
                $"`punch [user]`\n");
            embed.AddField("Links",
                "[Terms of Use](https://sites.google.com/view/social-linker-docs/terms-of-service)\n" +
                "[Privacy Policy](https://sites.google.com/view/social-linker-docs/privacy-policy)\n" +
                "[Social Linker Support](https://discord.gg/ZbEeZRjVvU)\n" +
                "[💗 Donate](https://ko-fi.com/microjack5)\n" +
                "");

            var component = new ComponentBuilder()
                .WithButton("📊 Status Screen Tutorial", customId: "status-screen-tutorial", ButtonStyle.Secondary)
                .WithButton("🛠️ Scene Maker Tutorial", customId: "scene-maker-tutorial", ButtonStyle.Secondary)
                .WithButton("⚖️ Legal Notices", customId: "legal-notices", ButtonStyle.Secondary)
                .WithButton("📄 Credits", customId: "credits", ButtonStyle.Secondary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);

            menuSession.CurrentMenu = "Help_Main_Menu";
        }
    }
}
