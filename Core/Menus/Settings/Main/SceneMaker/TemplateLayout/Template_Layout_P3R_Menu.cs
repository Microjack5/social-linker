using Discord;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace SocialLinker.Core.Menus.Settings.Main.SceneMaker.TemplateLayout
{
    class Template_Layout_P3R_Menu
    {
        public static async Task Template_Layout_P3R_Main(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Template Settings - Persona 3 Reload",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            embed.WithColor(EmbedSettings.Get_Game_Color("P3R", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P3R"));

            embed.WithDescription("" +
                "Select a setting to edit.\n" +
                "\n" +
                ":one: Date & Moon Phases\n" +
                ":two: Character Portrait Lighting\n" +
                ":three: Control Panel\n" +
                ":four: Auto Advance\n" +
                ":five: Low Latency Mode" +
                "");

            menuSession.CurrentMenu = "Template_Layout_P3R_Main";

            var selectMenu = new SelectMenuBuilder()
                    .WithPlaceholder("Select an option")
                    .WithCustomId(menuSession.CurrentMenu)
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .AddOption("Date & Moon Phases", "1", null, new Emoji("1️⃣"))
                    .AddOption("Character Portrait Lighting", "2", null, new Emoji("2️⃣"))
                    .AddOption("Control Panel", "3", null, new Emoji("3️⃣"))
                    .AddOption("Auto Advance", "4", null, new Emoji("4️⃣"))
                    .AddOption("Low Latency Mode", "5", null, new Emoji("5️⃣"))
                    .AddOption("Return to Version Select", "return", null, new Emoji("↩️"));

            var component = new ComponentBuilder()
                .WithSelectMenu(selectMenu);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_P3R_Date_Moon(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Date & Moon Phases",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            embed.WithColor(EmbedSettings.Get_Game_Color("P3R", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P3R"));

            embed.WithDescription("" +
                "Toggle parts of the date & moon HUD on and off.\n" +
                "\n" +
                $"⚙️ **Current setting:** **`{account.P3R_TS_HUD}`**\n" +
                "\n" +
                ":one: Display All\n" +
                ":two: Countdown Off\n" +
                ":three: None");

            embed.WithImageUrl("https://i.imgur.com/kHsF2fj.png");

            menuSession.CurrentMenu = "Template_Layout_P3R_Date_Moon";

            var selectMenu = new SelectMenuBuilder()
                    .WithPlaceholder("Select an option")
                    .WithCustomId(menuSession.CurrentMenu)
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .AddOption("Display All", "1", null, new Emoji("1️⃣"))
                    .AddOption("Countdown Off", "2", null, new Emoji("2️⃣"))
                    .AddOption("None", "3", null, new Emoji("3️⃣"))
                    .AddOption("Return to P3R Template Settings", "return", null, new Emoji("↩️"));

            var component = new ComponentBuilder()
                .WithSelectMenu(selectMenu);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_P3R_Portrait_Lighting(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Character Portrait Lighting",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            embed.WithColor(EmbedSettings.Get_Game_Color("P3R", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P3R"));

            embed.WithDescription("" +
                "Set how lighting effects should be applied to character portraits.\n" +
                "\n" +
                $"⚙️ **Current setting:** **`{account.P3R_TS_Portrait_Lighting_Type}`**\n" +
                "\n" +
                ":one: Default\n" +
                ":two: Background-Based - *Dynamically change lighting depending on the background.*\n" +
                ":three: Custom\n" +
                ":four: None - *Enables Low Latency Mode.*");

            embed.WithImageUrl("https://i.imgur.com/JRICsD7.png");

            menuSession.CurrentMenu = "Template_Layout_P3R_Portrait_Lighting";

            var selectMenu = new SelectMenuBuilder()
                    .WithPlaceholder("Select an option")
                    .WithCustomId(menuSession.CurrentMenu)
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .AddOption("Default", "1", null, new Emoji("1️⃣"))
                    .AddOption("Background-Based", "2", null, new Emoji("2️⃣"))
                    .AddOption("Custom", "3", null, new Emoji("3️⃣"))
                    .AddOption("None", "4", null, new Emoji("4️⃣"))
                    .AddOption("Return to P3R Template Settings", "return", null, new Emoji("↩️"));

            var component = new ComponentBuilder()
                .WithSelectMenu(selectMenu);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_P3R_Custom_Lighting(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Custom Lighting",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            embed.WithColor(EmbedSettings.Get_Game_Color("P3R", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P3R"));

            embed.WithDescription("" +
                "Select a setting to edit.\n" +
                "\n" +
                ":one: Custom Lighting Toggle\n" +
                ":two: Custom Lighting Colors");

            menuSession.CurrentMenu = "Template_Layout_P3R_Custom_Lighting";

            var selectMenu = new SelectMenuBuilder()
                    .WithPlaceholder("Select an option")
                    .WithCustomId(menuSession.CurrentMenu)
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .AddOption("Custom Lighting Toggle", "1", null, new Emoji("1️⃣"))
                    .AddOption("Custom Lighting Colors", "2", null, new Emoji("2️⃣"))
                    .AddOption("Return to Character Portrait Lighting", "return", null, new Emoji("↩️"));

            var component = new ComponentBuilder()
                .WithSelectMenu(selectMenu);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_P3R_Custom_Lighting_Toggle(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Custom Lighting Toggle",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            var footer = new EmbedFooterBuilder
            {
                Text = "↩️ Return to Character Portrait Lighting"
            };

            embed.WithFooter(footer);

            embed.WithColor(EmbedSettings.Get_Game_Color("P3R", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P3R"));

            string customLightingSetting = 
                account.P3R_TS_Portrait_Lighting_Type == "Custom" ? "On" : "Off";

            embed.WithDescription("" +
                "Toggle custom lighting for character portraits on and off.\n" +
                "\n" +
                $"⚙️ **Current setting:** **`{customLightingSetting}`**\n");

            menuSession.CurrentMenu = "Template_Layout_P3R_Custom_Lighting_Toggle";

            var component = new ComponentBuilder()
                .WithButton("↩️", customId: "return", ButtonStyle.Secondary)
                .WithButton("✅ On", customId: "on", ButtonStyle.Secondary)
                .WithButton("❌ Off", customId: "off", ButtonStyle.Secondary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_P3R_Custom_Lighting_Colors(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Custom Lighting Colors",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Profile_Config_Thumbnail(account));

            embed.WithDescription("" +
                "Manually set the base lighting and rim lighting colors of character portraits by entering hex color codes.\n" +
                "\n" +
                $"⚙️ **Current settings:**\n" +
                $"\n" +
                $"**Base Lighting:** **`{account.P3R_TS_Portrait_Lighting_Custom_Base}`**\n" +
                $"**Rim Lighting:** **`{account.P3R_TS_Portrait_Lighting_Custom_Rim}`**");

            menuSession.CurrentMenu = "Template_Layout_P3R_Custom_Lighting_Colors";

            var component = new ComponentBuilder()
                .WithButton("Enter Base Light Color Code", customId: "base-color-code-modal-open", ButtonStyle.Primary)
                .WithButton("Enter Rim Light Color Code", customId: "rim-color-code-modal-open", ButtonStyle.Primary)
                .WithButton("Reset Colors", customId: "reset-colors", ButtonStyle.Secondary)
                .WithButton("↩️ Return", customId: "back-to-p3r-custom-lighting", ButtonStyle.Secondary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_P3R_Custom_Lighting_Colors_Base_Modal(SocketMessageComponent component)
        {
            if (component.Data.CustomId == "base-color-code-modal-open")
            {
                try
                {
                    var modal = new ModalBuilder()
                    .WithTitle("Base Color Lighting")
                    .WithCustomId("base-color-code-modal-submit")
                    .AddTextInput("Color Code", "base_color_code");

                    await component.RespondWithModalAsync(modal.Build());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public static async Task Template_Layout_P3R_Custom_Lighting_Colors_Rim_Modal(SocketMessageComponent component)
        {
            if (component.Data.CustomId == "rim-color-code-modal-open")
            {
                try
                {
                    var modal = new ModalBuilder()
                    .WithTitle("Rim Color Lighting")
                    .WithCustomId("rim-color-code-modal-submit")
                    .AddTextInput("Color Code", "rim_color_code");

                    await component.RespondWithModalAsync(modal.Build());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public static async Task Template_Layout_P3R_Custom_Lighting_Colors_Error(MenuIdStructure menuSession)
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

            menuSession.CurrentMenu = "Template_Layout_P3R_Custom_Lighting_Colors_Error";

            var component = new ComponentBuilder()
                .WithButton("↩️ Retry", customId: "retry", ButtonStyle.Primary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_P3R_Control_Panel(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Control Panel",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            embed.WithColor(EmbedSettings.Get_Game_Color("P3R", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P3R"));

            embed.WithDescription("" +
                "Toggle between multiple control guide layouts.\n" +
                "\n" +
                $"⚙️ **Current setting:** **`{account.P3R_TS_Panel}`**\n" +
                "\n" +
                ":one: Xbox Series X|S\n" +
                ":two: PlayStation®️ 5\n" +
                ":three: PlayStation®️ 4\n" +
                ":four: Nintendo Switch 2\n" +
                ":five: Keyboard\n" +
                ":six: None");

            embed.WithImageUrl("https://i.imgur.com/LMCoXNc.png");

            menuSession.CurrentMenu = "Template_Layout_P3R_Control_Panel";

            var selectMenu = new SelectMenuBuilder()
                    .WithPlaceholder("Select an option")
                    .WithCustomId(menuSession.CurrentMenu)
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .AddOption("Xbox Series X|S", "1", null, new Emoji("1️⃣"))
                    .AddOption("PlayStation®️ 5", "2", null, new Emoji("2️⃣"))
                    .AddOption("PlayStation®️ 4", "3", null, new Emoji("3️⃣"))
                    .AddOption("Nintendo Switch 2", "4", null, new Emoji("4️⃣"))
                    .AddOption("Keyboard", "5", null, new Emoji("5️⃣"))
                    .AddOption("None", "6", null, new Emoji("6️⃣"))
                    .AddOption("Return to P3R Template Settings", "return", null, new Emoji("↩️"));

            var component = new ComponentBuilder()
                .WithSelectMenu(selectMenu);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_P3R_Auto_Advance(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Auto Advance",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            var footer = new EmbedFooterBuilder
            {
                Text = "↩️ Return to P3R Template Settings"
            };

            embed.WithFooter(footer);

            embed.WithColor(EmbedSettings.Get_Game_Color("P3R", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P3R"));

            embed.WithDescription("" +
                "Toggle the auto advance icon of the control panel on and off. Also toggles the message window's cursor.\n" +
                "\n" +
                $"⚙️ **Current setting:** **`{account.P3R_TS_Auto_Advance}`**\n");

            embed.WithImageUrl("https://i.imgur.com/OOlbbGW.png");

            menuSession.CurrentMenu = "Template_Layout_P3R_Auto_Advance";

            var component = new ComponentBuilder()
                .WithButton("↩️", customId: "return", ButtonStyle.Secondary)
                .WithButton("✅ On", customId: "on", ButtonStyle.Secondary)
                .WithButton("❌ Off", customId: "off", ButtonStyle.Secondary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_P3R_Low_Latency(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Low Latency Mode",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            var footer = new EmbedFooterBuilder
            {
                Text = "↩️ Return to P3R Template Settings"
            };

            embed.WithFooter(footer);

            embed.WithColor(EmbedSettings.Get_Game_Color("P3R", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P3R"));

            embed.WithDescription("" +
                "Enables faster scene generation time, but disables lighting for character portraits.\n" +
                "\n" +
                $"⚙️ **Current setting:** **`{account.P3R_TS_Low_Latency}`**\n");

            menuSession.CurrentMenu = "Template_Layout_P3R_Low_Latency";

            var component = new ComponentBuilder()
                .WithButton("↩️", customId: "return", ButtonStyle.Secondary)
                .WithButton("✅ On", customId: "on", ButtonStyle.Secondary)
                .WithButton("❌ Off", customId: "off", ButtonStyle.Secondary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_P3R_Date_Moon_Confirm(MenuIdStructure menuSession)
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

            embed.WithColor(EmbedSettings.Get_Game_Color("P3R", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P3R"));

            embed.WithDescription("" +
                $"Date & moon phases have been set to **`{account.P3R_TS_HUD}`**.\n");

            menuSession.CurrentMenu = "Template_Layout_P3R_Date_Moon_Confirm";

            var component = new ComponentBuilder()
                .WithButton("💠 P3R Template Settings", customId: "back-to-p3r-template-settings", ButtonStyle.Primary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_P3R_Portrait_Lighting_Confirm(MenuIdStructure menuSession)
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

            embed.WithColor(EmbedSettings.Get_Game_Color("P3R", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P3R"));

            embed.WithDescription("" +
                $"Character portrait lighting has been set to **`{account.P3R_TS_Portrait_Lighting_Type}`**.\n");

            menuSession.CurrentMenu = "Template_Layout_P3R_Portrait_Lighting_Confirm";

            var component = new ComponentBuilder()
                .WithButton("💠 P3R Template Settings", customId: "back-to-p3r-template-settings", ButtonStyle.Primary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_P3R_Custom_Lighting_Toggle_Confirm(MenuIdStructure menuSession)
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

            embed.WithColor(EmbedSettings.Get_Game_Color("P3R", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P3R"));

            string customLightingSetting =
                account.P3R_TS_Portrait_Lighting_Type == "Custom" ? "On" : "Off";

            embed.WithDescription("" +
                $"Custom lighting has been set to **`{customLightingSetting}`**.\n");

            menuSession.CurrentMenu = "Template_Layout_P3R_Custom_Lighting_Toggle_Confirm";

            var component = new ComponentBuilder()
                .WithButton("💠 P3R Custom Lighting", customId: "back-to-p3r-custom-lighting", ButtonStyle.Primary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_P3R_Custom_Lighting_Colors_Base_Confirm(MenuIdStructure menuSession)
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

            embed.WithColor(EmbedSettings.Get_Game_Color("P3R", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P3R"));

            embed.WithDescription("" +
                $"Base lighting for character portraits has been set to **`{account.P3R_TS_Portrait_Lighting_Custom_Base}`**.\n");

            menuSession.CurrentMenu = "Template_Layout_P3R_Custom_Lighting_Colors_Base_Confirm";

            var component = new ComponentBuilder()
                .WithButton("💠 Custom Lighting Colors", customId: "back-to-p3r-custom-lighting-colors", ButtonStyle.Primary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_P3R_Custom_Lighting_Colors_Rim_Confirm(MenuIdStructure menuSession)
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

            embed.WithColor(EmbedSettings.Get_Game_Color("P3R", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P3R"));

            embed.WithDescription("" +
                $"Rim lighting for character portraits has been set to **`{account.P3R_TS_Portrait_Lighting_Custom_Rim}`**.\n");

            menuSession.CurrentMenu = "Template_Layout_P3R_Custom_Lighting_Colors_Rim_Confirm";

            var component = new ComponentBuilder()
                .WithButton("💠 Custom Lighting Colors", customId: "back-to-p3r-custom-lighting-colors", ButtonStyle.Primary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_P3R_Custom_Lighting_Colors_Reset_Confirm(MenuIdStructure menuSession)
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

            embed.WithColor(EmbedSettings.Get_Game_Color("P3R", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P3R"));

            embed.WithDescription("" +
                $"Base lighting and rim lighting for character portraits have been set to default values.\n");

            menuSession.CurrentMenu = "Template_Layout_P3R_Custom_Lighting_Colors_Reset_Confirm";

            var component = new ComponentBuilder()
                .WithButton("💠 Custom Lighting Colors", customId: "back-to-p3r-custom-lighting-colors", ButtonStyle.Primary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_P3R_Control_Panel_Confirm(MenuIdStructure menuSession)
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

            embed.WithColor(EmbedSettings.Get_Game_Color("P3R", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P3R"));

            embed.WithDescription("" +
                $"The control panel has been set to **`{account.P3R_TS_Panel}`**.\n");

            menuSession.CurrentMenu = "Template_Layout_P3R_Control_Panel_Confirm";

            var component = new ComponentBuilder()
                .WithButton("💠 P3R Template Settings", customId: "back-to-p3r-template-settings", ButtonStyle.Primary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_P3R_Auto_Advance_Confirm(MenuIdStructure menuSession)
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

            embed.WithColor(EmbedSettings.Get_Game_Color("P3R", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P3R"));

            embed.WithDescription("" +
                $"The auto read cursor has been set to **`{account.P3R_TS_Auto_Advance}`**.\n");

            menuSession.CurrentMenu = "Template_Layout_P3R_Auto_Advance_Confirm";

            var component = new ComponentBuilder()
                .WithButton("💠 P3R Template Settings", customId: "back-to-p3r-template-settings", ButtonStyle.Primary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }

        public static async Task Template_Layout_P3R_Low_Latency_Confirm(MenuIdStructure menuSession)
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

            embed.WithColor(EmbedSettings.Get_Game_Color("P3R", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P3R"));

            embed.WithDescription("" +
                $"Low Latency Mode has been set to **`{account.P3R_TS_Low_Latency}`**.\n");

            menuSession.CurrentMenu = "Template_Layout_P3R_Low_Latency_Confirm";

            var component = new ComponentBuilder()
                .WithButton("💠 P3R Template Settings", customId: "back-to-p3r-template-settings", ButtonStyle.Primary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }
    }
}
