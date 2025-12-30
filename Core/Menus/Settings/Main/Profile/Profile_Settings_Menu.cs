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

namespace SocialLinker.Core.Menus.Settings.Main.Profile
{
    class Profile_Settings_Menu
    {
        public static async Task Profile_Settings_Main(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Profile Settings",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Profile_Config_Thumbnail(account));

            embed.AddField(":one: Profile Theme",
                "Adjust profile theme settings.");
            embed.AddField(":two: Status Screen Décor",
                "Adjust status screen décor settings.");
            embed.AddField(":three: Time Zone & Weather",
                "Adjust time zone and weather settings.");
            embed.AddField(":four: Level Up Notifications",
                "Adjust level up notification settings.");
            embed.AddField(":five: Rank Up Notifications",
                "Adjust rank up notification settings.");
            embed.AddField(":six: Content Filter",
                "Adjust content filter settings.");

            // If the user is at level 99 and has reset their level less than three times, add a "Star Level" option to the menu.
            if (account.Level == 99 && account.Level_Resets == 0)
            {
                embed.AddField(":star: Star Level",
                "Reach a new profile rank and continue earning P-Medals.");
            }
            else if (account.Level == 99 && account.Level_Resets == 1)
            {
                embed.AddField(":star2: Star Level",
                "Reach Star Level Rank 2 and continue earning P-Medals!");
            }
            else if (account.Level == 99 && account.Level_Resets == 2)
            {
                embed.AddField(":sparkles: Star Level",
                "Reach the final Star Level rank!");
            }

            menuSession.CurrentMenu = "Profile_Settings_Menu";

            var selectMenu = new SelectMenuBuilder()
                    .WithPlaceholder("Select an option")
                    .WithCustomId("profile-settings-menu-select")
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .AddOption("Profile Theme", "1", null, new Emoji("1️⃣"))
                    .AddOption("Status Screen Décor", "2", null, new Emoji("2️⃣"))
                    .AddOption("Time Zone & Weather", "3", null, new Emoji("3️⃣"))
                    .AddOption("Level Up Notifications", "4", null, new Emoji("4️⃣"))
                    .AddOption("Rank Up Notifications", "5", null, new Emoji("5️⃣"))
                    .AddOption("Content Filter", "6", null, new Emoji("6️⃣"));

            // If the user is at level 99, add a star reaction to the menu depending on how many times they reset their level.
            if (account.Level == 99 && account.Level_Resets == 0)
            {
                selectMenu.AddOption("Star Level", "star1", null, new Emoji("⭐"));
            }
            else if (account.Level == 99 && account.Level_Resets == 1)
            {
                selectMenu.AddOption("Star Level", "star2", null, new Emoji("🌟"));
            }
            else if (account.Level == 99 && account.Level_Resets == 2)
            {
                selectMenu.AddOption("Star Level", "star3", null, new Emoji("✨"));
            }

            selectMenu.AddOption("Return to Settings Main Menu", "return", null, new Emoji("↩️"));

            var component = new ComponentBuilder()
                .WithSelectMenu(selectMenu);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }
    }
}
