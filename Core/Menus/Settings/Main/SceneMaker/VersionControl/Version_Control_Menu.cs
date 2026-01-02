using System.Threading.Tasks;
using Discord;

namespace SocialLinker.Core.Menus.Settings.Main.SceneMaker
{
    class Version_Control_Menu
    {
        public static async Task Version_Control_Main(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var message = menuSession.MenuMessage;
            var user = menuSession.User;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Version Control",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Profile_Config_Thumbnail(account));

            embed.WithDescription("" +
                "Choose a title to view the version control settings for.\n");

            var selectMenu = new SelectMenuBuilder()
                    .WithPlaceholder("Select a title")
                    .WithCustomId("version-control-main")
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .AddOption("Persona", "p1", emote: Emote.Parse("<:P1:751133115531133112>"))
                    .AddOption("Persona 2: Innocent Sin", "p2is", emote: Emote.Parse("<:P2IS:788950080396328990>"))
                    .AddOption("Persona 2: Eternal Punishment", "p2ep", emote: Emote.Parse("<:P2EP:788950163363463172>"))
                    .AddOption("Persona 3", "p3", emote: Emote.Parse("<:P3:751133114918633483>"))
                    .AddOption("Persona 4", "p4", emote: Emote.Parse("<:P4:751133120530612274>"))
                    .AddOption("Persona 5", "p5", emote: Emote.Parse("<:P5:751133123861020742>"))
                    .AddOption("Return to Scene Maker Settings", "return", null, new Emoji("↩️"));

            var component = new ComponentBuilder()
                .WithSelectMenu(selectMenu);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);

            menuSession.CurrentMenu = "Version_Control_Main";
        }

        public static async Task Version_Control_P1(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var message = menuSession.MenuMessage;
            var user = menuSession.User;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Version Control - Persona",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            embed.WithColor(EmbedSettings.Get_Game_Color("P1-PSP", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P1-PSP"));

            string version_title = "";

            if (account.VC_P1 == "P1-PS1")
            {
                version_title = "Revelations: Persona";
            }
            else if (account.VC_P1 == "P1-PSP")
            {
                version_title = "Persona (PSP®️)";
            }

            embed.WithDescription("" +
                "Select the default version you would like to use.\n" +
                "\n" +
                $"⚙️ **Current Setting:** **`{version_title}`**\n");

            embed.WithImageUrl("https://i.imgur.com/bCWThuf.png");

            var selectMenu = new SelectMenuBuilder()
                    .WithPlaceholder("Select a version")
                    .WithCustomId("version-control-p1")
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .AddOption("Revelations: Persona", "p1-ps1", emote: Emote.Parse(Global.GetGameEmote("P1-PS1")))
                    .AddOption("Persona (PSP®️)", "p1-psp", emote: Emote.Parse(Global.GetGameEmote("P1-PSP")))
                    .AddOption("Return to Version Control Settings", "return", null, new Emoji("↩️"));

            var component = new ComponentBuilder()
                .WithSelectMenu(selectMenu);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);

            menuSession.CurrentMenu = "Version_Control_P1";
        }

        public static async Task Version_Control_P2IS(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var message = menuSession.MenuMessage;
            var user = menuSession.User;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Version Control - Persona 2: Innocent Sin",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            embed.WithColor(EmbedSettings.Get_Game_Color("P2IS-PSP", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P2IS-PSP"));

            string version_title = "";

            if (account.VC_P2IS == "P2IS-PS1")
            {
                version_title = "Persona 2: Innocent Sin (PlayStation®️)";
            }
            else if (account.VC_P2IS == "P2IS-PSP")
            {
                version_title = "Persona 2: Innocent Sin (PSP®️)";
            }

            embed.WithDescription("" +
                "Select the default version you would like to use.\n" +
                "\n" +
                $"⚙️ **Current Setting:** **`{version_title}`**\n");

            embed.WithImageUrl("https://i.imgur.com/6Utgced.png");

            var selectMenu = new SelectMenuBuilder()
                    .WithPlaceholder("Select a version")
                    .WithCustomId("version-control-p2is")
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .AddOption("Persona 2: Innocent Sin (PlayStation®️)", "p2is-ps1", emote: Emote.Parse(Global.GetGameEmote("P2IS-PS1")))
                    .AddOption("Persona 2: Innocent Sin (PSP®️)", "p2is-psp", emote: Emote.Parse(Global.GetGameEmote("P2IS-PSP")))
                    .AddOption("Return to Version Control Settings", "return", null, new Emoji("↩️"));

            var component = new ComponentBuilder()
                .WithSelectMenu(selectMenu);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);

            menuSession.CurrentMenu = "Version_Control_P2IS";
        }

        public static async Task Version_Control_P2EP(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var message = menuSession.MenuMessage;
            var user = menuSession.User;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Version Control - Persona 2: Eternal Punishment",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            embed.WithColor(EmbedSettings.Get_Game_Color("P2EP-PSP", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P2EP-PSP"));

            string version_title = "";

            if (account.VC_P2EP == "P2EP-PS1")
            {
                version_title = "Persona 2: Eternal Punishment (PlayStation®️)";
            }
            else if (account.VC_P2EP == "P2EP-PSP")
            {
                version_title = "Persona 2: Eternal Punishment (PSP®️)";
            }

            embed.WithDescription("" +
                "Select the default version you would like to use.\n" +
                "\n" +
                $"⚙️ **Current Setting:** **`{version_title}`**\n");

            embed.WithImageUrl("https://i.imgur.com/JAZN3dP.png");

            var selectMenu = new SelectMenuBuilder()
                    .WithPlaceholder("Select a version")
                    .WithCustomId("version-control-p2ep")
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .AddOption("Persona 2: Eternal Punishment (PlayStation®️)", "p2ep-ps1", emote: Emote.Parse(Global.GetGameEmote("P2EP-PS1")))
                    .AddOption("Persona 2: Eternal Punishment (PSP®️)", "p2ep-psp", emote: Emote.Parse(Global.GetGameEmote("P2EP-PSP")))
                    .AddOption("Return to Version Control Settings", "return", null, new Emoji("↩️"));

            var component = new ComponentBuilder()
                .WithSelectMenu(selectMenu);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);

            menuSession.CurrentMenu = "Version_Control_P2EP";
        }

        public static async Task Version_Control_P3(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var message = menuSession.MenuMessage;
            var user = menuSession.User;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Version Control - Persona 3",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            var footer = new EmbedFooterBuilder
            {
                Text = "↩️ Return to Version Control Settings"
            };

            embed.WithFooter(footer);

            embed.WithColor(EmbedSettings.Get_Game_Color("P3F", null));
            embed.WithThumbnailUrl("https://i.imgur.com/trtPflx.png");

            string version_title = "";

            if (account.VC_P3 == "P3F")
            {
                version_title = "Persona 3 FES";
            }
            else if (account.VC_P3 == "P3P")
            {
                version_title = "Persona 3 Portable";
            }

            embed.WithDescription("" +
                "Select the default version you would like to use.\n" +
                "\n" +
                $"⚙️ **Current Setting:** **`{version_title}`**\n");

            embed.WithImageUrl("https://i.imgur.com/hZJTcx4.png");

            var selectMenu = new SelectMenuBuilder()
                    .WithPlaceholder("Select a version")
                    .WithCustomId("version-control-p3")
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .AddOption("Persona 3 FES", "p3f", emote: Emote.Parse(Global.GetGameEmote("P3F")))
                    .AddOption("Persona 3 Portable", "p3p", emote: Emote.Parse(Global.GetGameEmote("P3P")))
                    .AddOption("Return to Version Control Settings", "return", null, new Emoji("↩️"));

            var component = new ComponentBuilder()
                .WithSelectMenu(selectMenu);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);

            menuSession.CurrentMenu = "Version_Control_P3";
        }

        public static async Task Version_Control_P4(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var message = menuSession.MenuMessage;
            var user = menuSession.User;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Version Control - Persona 4",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            embed.WithColor(EmbedSettings.Get_Game_Color("P4-PS2", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P4-PS2"));

            string version_title = "";

            if (account.VC_P4 == "P4-PS2")
            {
                version_title = "Persona 4 (PlayStation®️ 2)";
            }
            else if (account.VC_P4 == "P4G")
            {
                version_title = "Persona 4 Golden";
            }

            embed.WithDescription("" +
                "Select the default version you would like to use.\n" +
                "\n" +
                $"⚙️ **Current Setting:** **`{version_title}`**\n");

            embed.WithImageUrl("https://i.imgur.com/ZVldBKO.png");

            var selectMenu = new SelectMenuBuilder()
                    .WithPlaceholder("Select a version")
                    .WithCustomId("version-control-p4")
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .AddOption("Persona 4 (PlayStation®️ 2)", "p4-ps2", emote: Emote.Parse(Global.GetGameEmote("P4-PS2")))
                    .AddOption("Persona 4 Golden", "p4g", emote: Emote.Parse(Global.GetGameEmote("P4G")))
                    .AddOption("Return to Version Control Settings", "return", null, new Emoji("↩️"));

            var component = new ComponentBuilder()
                .WithSelectMenu(selectMenu);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);

            menuSession.CurrentMenu = "Version_Control_P4";
        }

        public static async Task Version_Control_P5(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var message = menuSession.MenuMessage;
            var user = menuSession.User;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Version Control - Persona 5",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            embed.WithColor(EmbedSettings.Get_Game_Color("P5-PS4", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P5-PS4"));

            string version_title = "";

            if (account.VC_P5 == "P5-PS4")
            {
                version_title = "Persona 5 (PlayStation®️ 4)";
            }
            else if (account.VC_P5 == "P5R")
            {
                version_title = "Persona 5 Royal";
            }

            embed.WithDescription("" +
                "Select the default version you would like to use.\n" +
                "\n" +
                $"⚙️ **Current Setting:** **`{version_title}`**\n");

            embed.WithImageUrl("https://i.imgur.com/7PMim5v.png");

            var selectMenu = new SelectMenuBuilder()
                    .WithPlaceholder("Select a version")
                    .WithCustomId("version-control-p5")
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .AddOption("Persona 5 (PlayStation®️ 4)", "p5-ps4", emote: Emote.Parse(Global.GetGameEmote("P5-PS4")))
                    .AddOption("Persona 5 Royal", "p5r", emote: Emote.Parse(Global.GetGameEmote("P5R")))
                    .AddOption("Return to Version Control Settings", "return-to-vc-settings", null, new Emoji("↩️"));

            var component = new ComponentBuilder()
                .WithSelectMenu(selectMenu);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);

            menuSession.CurrentMenu = "Version_Control_P5";
        }

        public static async Task Version_Control_P1_Confirm(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var message = menuSession.MenuMessage;
            var user = menuSession.User;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Settings Saved",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            embed.WithColor(EmbedSettings.Get_Game_Color("P1-PSP", null));

            string version_title = "";

            if (account.VC_P1 == "P1-PS1")
            {
                version_title = "Revelations: Persona";
                embed.WithThumbnailUrl("https://i.imgur.com/VDFzfIt.jpg");
            }
            else if (account.VC_P1 == "P1-PSP")
            {
                version_title = "Persona (PSP®️)";
                embed.WithThumbnailUrl("https://i.imgur.com/V3GQl38.jpg");
            }

            embed.WithDescription("" +
                $"Your version control settings for Persona has been set to **`{version_title}`**.");

            var component = new ComponentBuilder()
                .WithButton("Return to Version Control Settings", customId: "return", ButtonStyle.Primary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);

            menuSession.CurrentMenu = "Version_Control_P1_Confirm";
        }

        public static async Task Version_Control_P2IS_Confirm(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var message = menuSession.MenuMessage;
            var user = menuSession.User;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Settings Saved",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            embed.WithColor(EmbedSettings.Get_Game_Color("P2IS-PSP", null));

            string version_title = "";

            if (account.VC_P2IS == "P2IS-PS1")
            {
                version_title = "Persona 2: Innocent Sin (PlayStation®️)";
                embed.WithThumbnailUrl("https://i.imgur.com/PZAMFPy.jpg");
            }
            else if (account.VC_P2IS == "P2IS-PSP")
            {
                version_title = "Persona 2: Innocent Sin (PSP®️)";
                embed.WithThumbnailUrl("https://i.imgur.com/yMcba0F.jpg");
            }

            embed.WithDescription("" +
                $"Your version control settings for Persona 2: Innocent Sin has been set to **`{version_title}`**.");

            var component = new ComponentBuilder()
                .WithButton("Return to Version Control Settings", customId: "return", ButtonStyle.Primary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);

            menuSession.CurrentMenu = "Version_Control_P2IS_Confirm";
        }

        public static async Task Version_Control_P2EP_Confirm(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var message = menuSession.MenuMessage;
            var user = menuSession.User;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Settings Saved",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            embed.WithColor(EmbedSettings.Get_Game_Color("P2EP-PSP", null));

            string version_title = "";

            if (account.VC_P2EP == "P2EP-PS1")
            {
                version_title = "Persona 2: Eternal Punishment (PlayStation®️)";
                embed.WithThumbnailUrl("https://i.imgur.com/KVj8Tqt.jpg");
            }
            else if (account.VC_P2EP == "P2EP-PSP")
            {
                version_title = "Persona 2: Eternal Punishment (PSP®️)";
                embed.WithThumbnailUrl("https://i.imgur.com/x04Ldby.jpg");
            }

            embed.WithDescription("" +
                $"Your version control settings for Persona 2: Eternal Punishment has been set to **`{version_title}`**.");

            var component = new ComponentBuilder()
                .WithButton("Return to Version Control Settings", customId: "return", ButtonStyle.Primary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);

            menuSession.CurrentMenu = "Version_Control_P2EP_Confirm";
        }

        public static async Task Version_Control_P3_Confirm(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var message = menuSession.MenuMessage;
            var user = menuSession.User;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Settings Saved",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            string version_title = "";

            if (account.VC_P3 == "P3F")
            {
                version_title = "Persona 3 FES";
                embed.WithThumbnailUrl("https://i.imgur.com/LX1PSeP.jpg");
                embed.WithColor(EmbedSettings.Get_Game_Color("P3F", null));
            }
            else if (account.VC_P3 == "P3P")
            {
                version_title = "Persona 3 Portable";
                embed.WithThumbnailUrl("https://i.imgur.com/sNrtgFX.jpg");
                embed.WithColor(EmbedSettings.Get_Game_Color("P3P", account));
            }

            embed.WithDescription("" +
                $"Your version control settings for Persona 3 has been set to **`{version_title}`**.");

            var component = new ComponentBuilder()
                .WithButton("Return to Version Control Settings", customId: "return", ButtonStyle.Primary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);

            menuSession.CurrentMenu = "Version_Control_P3_Confirm";
        }

        public static async Task Version_Control_P4_Confirm(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var message = menuSession.MenuMessage;
            var user = menuSession.User;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Settings Saved",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);


            embed.WithColor(EmbedSettings.Get_Game_Color("P4G", null));

            string version_title = "";

            if (account.VC_P4 == "P4-PS2")
            {
                version_title = "Persona 4 (PlayStation®️ 2)";
                embed.WithThumbnailUrl("https://i.imgur.com/mXi0b8j.jpg");
            }
            else if (account.VC_P4 == "P4G")
            {
                version_title = "Persona 4 Golden";
                embed.WithThumbnailUrl("https://i.imgur.com/bVbgkST.png");
            }

            embed.WithDescription("" +
                $"Your version control settings for Persona 4 has been set to **`{version_title}`**.");

            var component = new ComponentBuilder()
                .WithButton("Return to Version Control Settings", customId: "return", ButtonStyle.Primary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);

            menuSession.CurrentMenu = "Version_Control_P4_Confirm";
        }

        public static async Task Version_Control_P5_Confirm(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var message = menuSession.MenuMessage;
            var user = menuSession.User;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Settings Saved",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            embed.WithColor(EmbedSettings.Get_Game_Color("P5-PS4", null));

            string version_title = "";

            if (account.VC_P5 == "P5-PS4")
            {
                version_title = "Persona 5 (PlayStation®️ 4)";
                embed.WithThumbnailUrl("https://i.imgur.com/K78my5p.jpg");
            }
            else if (account.VC_P5 == "P5R")
            {
                version_title = "Persona 5 Royal";
                embed.WithThumbnailUrl("https://i.imgur.com/auKURsG.jpg");
            }

            embed.WithDescription("" +
                $"Your version control settings for Persona 5 has been set to **`{version_title}`**.");

            var component = new ComponentBuilder()
                .WithButton("Return to Version Control Settings", customId: "return", ButtonStyle.Primary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);

            menuSession.CurrentMenu = "Version_Control_P5_Confirm";
        }
    }
}
