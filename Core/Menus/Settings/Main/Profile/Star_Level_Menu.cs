using System;
using System.Threading.Tasks;
using Discord;

namespace SocialLinker.Core.Menus.Settings.Main.Profile
{
    class Star_Level_Menu
    {
        public static async Task Star_Level_Main(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Star Level",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            // Determine the color and thumbnail for the embeded message
            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Profile_Config_Thumbnail(account));

            string description_text = "";

            if (account.Level_Resets == 0)
            {
                description_text = "" +
                    "Star Level is a special rank that lets you keep earning P-Medals past Level 99!\n" +
                    "Your profile will be reset to Level 1, but a signature mark will appear on your status screen and all your social stats stay intact for P-Medal bonuses.\n" +
                    "\n" +
                    "Would you like to reach to Star Level Rank 1?";
            }
            else if (account.Level_Resets == 1)
            {
                description_text = "" +
                    "You've maxed out your level twice!\n" +
                    "You can reach Star Level Rank 2 by resetting your level again while gaining another star mark and keeping your social stats intact.\n" +
                    "\n" +
                    "Would you like to reach Star Level Rank 2?";
            }
            else if (account.Level_Resets == 2)
            {
                description_text = "" +
                    "You've made it! This is the end goal of Social Linker.\n" +
                    "This time, there are no more level resets. All levels and social stats remain completely intact going forward.\n" +
                    "\n" +
                    "Would you like to reach Star Level Rank 3?";
            }

            embed.WithDescription(description_text);

            menuSession.CurrentMenu = "Star_Level_Main";

            var component = new ComponentBuilder()
                .WithButton("↩️ Return", customId: "return", ButtonStyle.Secondary)
                .WithButton("✅ Confirm", customId: "confirm", ButtonStyle.Secondary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Star_Level_Check(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Are You Sure?",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            // Determine the color and thumbnail for the embeded message
            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Profile_Config_Thumbnail(account));

            if (account.Level_Resets == 0)
            {
                embed.WithDescription($"Are you sure you want to gain Star Level rank and reset your level? This cannot be undone.");
            }
            else if (account.Level_Resets == 1)
            {
                embed.WithDescription($"Are you sure you want to gain another Star Level rank and reset your level? This cannot be undone.");
            }
            else if (account.Level_Resets == 2)
            {
                embed.WithDescription($"Are you sure? No turning back!");
            }

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

                // Remove the menu entry from the global list.
                Global.MenuIdList.Remove(menuSession);

                return;
            }

            // Edit the menu session according to the current message.
            menuSession.CurrentMenu = "Star_Level_Check";

            var component = new ComponentBuilder()
                .WithButton("💠 Profile Settings", customId: "profile-settings", ButtonStyle.Secondary);

            // Add a star reaction to the menu depending on how many times the user's level has been reset.
            if (account.Level == 99 && account.Level_Resets == 0)
            {
                component.WithButton("⭐ Confirm", customId: "confirm", ButtonStyle.Secondary);
            }
            else if (account.Level == 99 && account.Level_Resets == 1)
            {
                component.WithButton("🌟 Confirm", customId: "confirm", ButtonStyle.Secondary);
            }
            else if (account.Level == 99 && account.Level_Resets == 2)
            {
                component.WithButton("✨ Confirm", customId: "confirm", ButtonStyle.Secondary);
            }

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Star_Level_Confirm(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();

            string header_text = "";
            string description_text = "";

            if (account.Level_Resets == 1)
            {
                header_text = "Welcome to Star Level!";
                description_text = "" +
                    "Your Star Level is now at Rank 1! A rank above the rest!\n" +
                    "\n" +
                    "There are three possible ranks you can reach in Star Level. Maybe a special surprise awaits at the final one...?\n" +
                    "\n" +
                    "Only one way to find out! Go out and show off your new rank with pride!";
            }
            else if (account.Level_Resets == 2)
            {
                header_text = "Star Level Rank Up!!";
                description_text = "" +
                    "Your Star Level is now at Rank 2! You've come an amazingly long way. Congratulations!";
            }
            else if (account.Level_Resets == 3)
            {
                header_text = "STAR LEVEL RANK MAX!!";
                description_text = "" +
                    "You've completely maxed out your Star Level! There are no other greater accomplishments past this point.\n" +
                    "\n" +
                    "As a special gift, all décor in the Décor Shop are now completely free for you from this point onwards. Thank you for being active and using Social Linker so much over the past few years!";
            }

            var author = new EmbedAuthorBuilder
            {
                Name = header_text,
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            // Determine the color and thumbnail for the embeded message
            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Profile_Config_Thumbnail(account));

            embed.WithDescription(description_text);

            menuSession.CurrentMenu = "Star_Level_Confirm";

            var component = new ComponentBuilder()
                .WithButton("💠 Profile Settings", customId: "profile-settings", ButtonStyle.Secondary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }
    }
}
