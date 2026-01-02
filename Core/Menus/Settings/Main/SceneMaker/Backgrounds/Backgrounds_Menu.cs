using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace SocialLinker.Core.Menus.Settings.Main.SceneMaker.Backgrounds
{
    class Backgrounds_Menu
    {
        public static async Task Backgrounds_Main(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Backgrounds",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Profile_Config_Thumbnail(account));

            embed.WithDescription("" +
                "Select a setting to edit.\n" +
                "\n" +
                ":one: Default Background Color\n" +
                ":two: Background Upload Settings\n");

            menuSession.CurrentMenu = "Backgrounds_Main";

            var selectMenu = new SelectMenuBuilder()
                    .WithPlaceholder("Select an option")
                    .WithCustomId(menuSession.CurrentMenu)
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .AddOption("Default Background Color", "1", null, new Emoji("1️⃣"))
                    .AddOption("Background Upload Settings", "2", null, new Emoji("2️⃣"))
                    .AddOption("Return to Scene Maker Settings", "return", null, new Emoji("↩️"));

            var component = new ComponentBuilder()
                .WithSelectMenu(selectMenu);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Backgrounds_Default_Color(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Default Background Color",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Profile_Config_Thumbnail(account));

            embed.WithDescription("" +
                "Choose a default solid color to use for scene maker backgrounds by entering a hex color code.\n" +
                "\n" +
                $"⚙️ **Current setting:** **`{account.Setting_BG_Color}`**\n");

            menuSession.CurrentMenu = "Backgrounds_Default_Color";

            var component = new ComponentBuilder()
                .WithButton("Enter Color Code", customId: "color-code-modal-open", ButtonStyle.Primary)
                .WithButton("Reset Background", customId: "reset-background", ButtonStyle.Secondary)
                .WithButton("↩️ Return", customId: "back-to-background-settings", ButtonStyle.Secondary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Backgrounds_Default_Color_Modal(SocketMessageComponent component)
        {
            if (component.Data.CustomId == "color-code-modal-open")
            {
                try
                {
                    var modal = new ModalBuilder()
                    .WithTitle("Default Background Color")
                    .WithCustomId("color-code-modal-submit")
                    .AddTextInput("Color Code", "color_code");

                    await component.RespondWithModalAsync(modal.Build());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public static async Task Backgrounds_Upload_Settings(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Background Upload Settings",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Profile_Config_Thumbnail(account));

            embed.WithDescription("" +
                "Choose how background images are rendered when images are uploaded with scene maker commands.\n" +
                "\n" +
                $"⚙️ **Current setting:** **`{account.Setting_BG_Upload}`**\n" +
                "\n" +
                ":one: Scale to Width - *Scale backgrounds to span from the left side of a scene to the right side.*\n" +
                ":two: Scale to Height - *Scale backgrounds to span from the top of a scene to the bottom.*\n" +
                ":three: Scale to Fit - *Scale backgrounds to view their entirety within a scene.*\n" +
                ":four: Scale to Fill - *Scale backgrounds to fill the entire scene, maintaining aspect ratio.*\n" +
                ":five: Stretch to Fill - *Stretch backgrounds to fill the entire scene, disregarding aspect ratio.*\n");

            menuSession.CurrentMenu = "Backgrounds_Upload_Settings";

            var selectMenu = new SelectMenuBuilder()
                    .WithPlaceholder("Select an option")
                    .WithCustomId(menuSession.CurrentMenu)
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .AddOption("Scale to Width", "1", null, new Emoji("1️⃣"))
                    .AddOption("Scale to Height", "2", null, new Emoji("2️⃣"))
                    .AddOption("Scale to Fit", "3", null, new Emoji("3️⃣"))
                    .AddOption("Scale to Fill", "4", null, new Emoji("4️⃣"))
                    .AddOption("Stretch to Fill", "5", null, new Emoji("5️⃣"))
                    .AddOption("Return to Backgrounds Menu", "return", null, new Emoji("↩️"));

            var component = new ComponentBuilder()
                .WithSelectMenu(selectMenu);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Backgrounds_Default_Color_Error(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Invalid Hex Color Code",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Profile_Config_Thumbnail(account));

            embed.WithDescription("" +
                "It looks like an invalid hex code was typed in.\n");

            embed.AddField("Tips", "" +
                "Hex codes are the same ones used to set colors for Discord roles. Try using the role color picker if you need reference for a color's code.");

            menuSession.CurrentMenu = "Backgrounds_Default_Color_Error";

            var component = new ComponentBuilder()
                .WithButton("↩️ Retry", customId: "retry", ButtonStyle.Primary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Backgrounds_Default_Color_Confirm(MenuIdStructure menuSession)
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

            embed.WithDescription("" +
                $"The default background color has changed to **`{account.Setting_BG_Color}`**.\n");

            menuSession.CurrentMenu = "Backgrounds_Default_Color_Confirm";

            var component = new ComponentBuilder()
                .WithButton("💠 Background Settings", customId: "back-to-background-settings", ButtonStyle.Primary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Backgrounds_Upload_Settings_Confirm(MenuIdStructure menuSession)
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

            embed.WithDescription("" +
                $"Your background rendering settings have changed to **`{account.Setting_BG_Upload}`**.\n");

            menuSession.CurrentMenu = "Backgrounds_Upload_Settings_Confirm";

            var component = new ComponentBuilder()
                .WithButton("💠 Background Settings", customId: "back-to-background-settings", ButtonStyle.Primary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }
    }
}
