using Discord;
using Discord.Rest;
using Discord.WebSocket;
using SocialLinker.Config;
using SocialLinker.Core.CloudStorageTables;
using System;
using System.Threading.Tasks;
using System.Timers;

namespace SocialLinker.Core.Menus.Settings.Main
{
    class Settings_Menu
    {
        public static async Task Settings_Start(SocketTextChannel channel, SocketGuildUser user)
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
                CurrentMenu = "Settings_Start",
                MenuTimer = new Timer()
                {
                    // Create a timer that expires as a "time out" duration for the user.
                    Interval = MenuConfig.menu.timerDuration,
                    AutoReset = false,
                    Enabled = true
                },
                InactiveMessage = $"You can view and change your user settings at any time with the **`settings`** command."
            };

            // Add the menu entry to the global list.
            Global.MenuIdList.Add(menuSession);

            // Create a new menu in the current channel.
            await Settings_Main_Menu(menuSession);
        }

        public static async Task Settings_Main_Menu(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "User Settings",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            var footer = new EmbedFooterBuilder
            {
                Text = "React with ❌ to close any menu"
            };

            embed.WithFooter(footer);

            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Profile_Config_Thumbnail(account));

            embed.AddField("🔹 Profile Settings",
                "Configure various profile settings.");
            embed.AddField("🔹 Scene Maker Settings",
                "Change general scene maker settings.");

            var component = new ComponentBuilder()
                .WithButton("Profile Settings", customId: "profile-settings", ButtonStyle.Secondary)
                .WithButton("Scene Maker Settings", customId: "scene-maker-settings", ButtonStyle.Secondary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);

            menuSession.CurrentMenu = "Settings_Main_Menu";
        }
    }
}
