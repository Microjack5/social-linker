using System.Threading.Tasks;
using Discord;

namespace SocialLinker.Core.Menus.Settings.Main.Profile
{
    class Level_Up_Notifications_Menu
    {
        public static async Task Level_Up_Notifications_Main(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Level Up Notifications",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Profile_Config_Thumbnail(account));

            embed.WithDescription("" +
                "You can toggle notification messages on or off for whenever your user level has increased.\n" +
                "\n" +
                $"⚙️ **Current Setting:** **`{account.Level_Up_Notifications}`**\n");

            menuSession.CurrentMenu = "Level_Up_Notifications_Main";

            var component = new ComponentBuilder()
                .WithButton("↩️", customId: "return", ButtonStyle.Secondary)
                .WithButton("✅ On", customId: "on", ButtonStyle.Secondary)
                .WithButton("❌ Off", customId: "off", ButtonStyle.Secondary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Level_Up_Notifications_Confirm(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Settings Saved",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Profile_Config_Thumbnail(account));

            embed.WithDescription($"Level up notifications have been set to **`{account.Level_Up_Notifications}`**.");

            menuSession.CurrentMenu = "Level_Up_Notifications_Confirm";

            var component = new ComponentBuilder()
                .WithButton("💠 General Settings", customId: "back-to-general-settings", ButtonStyle.Primary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }
    }
}
