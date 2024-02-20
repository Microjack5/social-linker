using SocialLinker.Core.CloudStorageTables;
using Discord.WebSocket;
using System.Threading.Tasks;
using Discord;
using SocialLinker.Core.LocalStorageTables;
using System;
using SocialLinker.Core.Menus;

namespace SocialLinker.Core.SceneMaker
{
    public class OfficialSetLists
    {
        public static async Task P1_PS1_Set_List(SocialLinkerCommand sl_command)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = sl_command.User;
            SocketTextChannel channel = (SocketTextChannel)sl_command.Channel;

            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Revelations: Persona Conversation Portrait Sets",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            // Set the color and thumbnail for the embeded message.
            embed.WithColor(EmbedSettings.Get_Game_Color("P1-PS1", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P1-PS1"));

            // Create a description with the list of sprite sets available for the title.
            embed.WithDescription($"{OfficialSetMethods.Generate_P1_PS1_Set_List()}");

            // Create a footer based on the game version the list is from.
            var footer = new EmbedFooterBuilder
            {
                Text = "Version: P1-PS1"
            };

            // Add the footer to the embed.
            embed.WithFooter(footer);

            // Send the embeded message to the channel.
            await channel.SendMessageAsync("", false, embed.Build());
        }

        public static async Task P1_PSP_Set_List(SocialLinkerCommand sl_command)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = sl_command.User;
            SocketTextChannel channel = (SocketTextChannel)sl_command.Channel;

            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Persona (Remake) Conversation Portrait Sets",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            // Set the color and thumbnail for the embeded message.
            embed.WithColor(EmbedSettings.Get_Game_Color("P1-PSP", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P1-PSP"));

            // Create a description with the list of sprite sets available for the title.
            embed.WithDescription($"{OfficialSetMethods.Generate_Normal_Set_List("P1-PSP")}");

            // Create a footer based on the game version the list is from.
            var footer = new EmbedFooterBuilder
            {
                Text = "Version: P1-PSP"
            };

            // Add the footer to the embed.
            embed.WithFooter(footer);

            // Send the embeded message to the channel.
            await channel.SendMessageAsync("", false, embed.Build());
        }

        public static async Task P2IS_PS1_Set_List(SocialLinkerCommand sl_command)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = sl_command.User;
            SocketTextChannel channel = (SocketTextChannel)sl_command.Channel;

            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Persona 2: Innocent Sin (PlayStation®️) Conversation Portrait Sets",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            // Set the color and thumbnail for the embeded message.
            embed.WithColor(EmbedSettings.Get_Game_Color("P2IS-PS1", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P2IS-PS1"));

            // Create a description with the list of sprite sets available for the title.
            embed.WithDescription($"{OfficialSetMethods.Generate_Normal_Set_List("P2IS-PS1")}");

            // Create a footer based on the game version the list is from.
            var footer = new EmbedFooterBuilder
            {
                Text = "Version: P2IS-PS1"
            };

            // Add the footer to the embed.
            embed.WithFooter(footer);

            // Send the embeded message to the channel.
            await channel.SendMessageAsync("", false, embed.Build());
        }

        public static async Task P2IS_PSP_Set_List(SocialLinkerCommand sl_command)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = sl_command.User;
            SocketTextChannel channel = (SocketTextChannel)sl_command.Channel;

            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Persona 2: Innocent Sin (Remake) Conversation Portrait Sets",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            // Set the color and thumbnail for the embeded message.
            embed.WithColor(EmbedSettings.Get_Game_Color("P2IS-PSP", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P2IS-PSP"));

            // Create a description with the list of sprite sets available for the title.
            embed.WithDescription($"{OfficialSetMethods.Generate_P2IS_PSP_Set_List()}");

            // Create a footer based on the game version the list is from.
            var footer = new EmbedFooterBuilder
            {
                Text = "Version: P2IS-PSP"
            };

            // Add the footer to the embed.
            embed.WithFooter(footer);

            // Send the embeded message to the channel.
            await channel.SendMessageAsync("", false, embed.Build());
        }

        public static async Task P2EP_PS1_Set_List(SocialLinkerCommand sl_command)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = sl_command.User;
            SocketTextChannel channel = (SocketTextChannel)sl_command.Channel;

            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Persona 2: Eternal Punishment (PlayStation®️) Conversation Portrait Sets",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            // Set the color and thumbnail for the embeded message.
            embed.WithColor(EmbedSettings.Get_Game_Color("P2EP-PS1", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P2EP-PS1"));

            // Create a description with the list of sprite sets available for the title.
            embed.WithDescription($"{OfficialSetMethods.Generate_Normal_Set_List("P2EP-PS1")}");

            // Create a footer based on the game version the list is from.
            var footer = new EmbedFooterBuilder
            {
                Text = "Version: P2EP-PS1"
            };

            // Add the footer to the embed.
            embed.WithFooter(footer);

            // Send the embeded message to the channel.
            await channel.SendMessageAsync("", false, embed.Build());
        }

        public static async Task P2EP_PSP_Set_List(SocialLinkerCommand sl_command)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = sl_command.User;
            SocketTextChannel channel = (SocketTextChannel)sl_command.Channel;

            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Persona 2: Eternal Punishment (Remake) Conversation Portrait Sets",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            // Set the color and thumbnail for the embeded message.
            embed.WithColor(EmbedSettings.Get_Game_Color("P2EP-PSP", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P2EP-PSP"));

            // Create a description with the list of sprite sets available for the title.
            embed.WithDescription($"{OfficialSetMethods.Generate_P2EP_PSP_Set_List()}");

            // Create a footer based on the game version the list is from.
            var footer = new EmbedFooterBuilder
            {
                Text = "Version: P2EP-PSP"
            };

            // Add the footer to the embed.
            embed.WithFooter(footer);

            // Send the embeded message to the channel.
            await channel.SendMessageAsync("", false, embed.Build());
        }

        public static async Task P3F_Set_List(SocialLinkerCommand sl_command)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = sl_command.User;
            SocketTextChannel channel = (SocketTextChannel)sl_command.Channel;

            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Persona 3 FES Conversation Portrait Sets",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            // Assign a color and thumbnail to the embeded message based on the title.
            embed.WithColor(EmbedSettings.Get_Game_Color("P3F", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P3F"));

            // Create a description with the list of sprite sets available for the title.
            embed.WithDescription($"{OfficialSetMethods.Generate_Normal_Set_List("P3F")}");

            // Create a footer based on the game version the list is from.
            var footer = new EmbedFooterBuilder
            {
                Text = "Version: P3F"
            };

            // Add the footer to the embed.
            embed.WithFooter(footer);

            // Send the embeded message to the channel.
            await channel.SendMessageAsync("", false, embed.Build());
        }

        public static async Task P3P_Set_List(SocialLinkerCommand sl_command)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = sl_command.User;
            SocketTextChannel channel = (SocketTextChannel)sl_command.Channel;

            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Persona 3 Portable Conversation Portrait Sets",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            // Assign a color based on the user's color setting for the P3P template.
            embed.WithColor(EmbedSettings.Get_Game_Color("P3P", account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P3P"));

            // Create a description with the list of sprite sets available for the title.
            embed.WithDescription($"{OfficialSetMethods.Generate_Normal_Set_List("P3P")}");

            // Create a footer based on the game version the list is from.
            var footer = new EmbedFooterBuilder
            {
                Text = "Version: P3P"
            };

            // Add the footer to the embed.
            embed.WithFooter(footer);

            // Send the embeded message to the channel.
            await channel.SendMessageAsync("", false, embed.Build());
        }

        public static async Task P3R_Set_List(SocialLinkerCommand sl_command)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = sl_command.User;
            SocketTextChannel channel = (SocketTextChannel)sl_command.Channel;

            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Persona 3 Reload Conversation Portrait Sets",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            // Assign a color and thumbnail to the embeded message based on the title.
            embed.WithColor(EmbedSettings.Get_Game_Color("P3R", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P3R"));

            // Create a description with the list of sprite sets available for the title.
            embed.WithDescription($"{OfficialSetMethods.Generate_Normal_Set_List("P3R")}");

            // Create a footer based on the game version the list is from.
            var footer = new EmbedFooterBuilder
            {
                Text = "Version: P3R"
            };

            // Add the footer to the embed.
            embed.WithFooter(footer);

            // Send the embeded message to the channel.
            await channel.SendMessageAsync("", false, embed.Build());
        }

        public static async Task P4_PS2_Set_List(SocialLinkerCommand sl_command)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = sl_command.User;
            SocketTextChannel channel = (SocketTextChannel)sl_command.Channel;

            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Persona 4 Conversation Portrait Sets",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            // Set the color and thumbnail for the embeded message.
            embed.WithColor(EmbedSettings.Get_Game_Color("P4-PS2", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P4-PS2"));

            // Create a description with the list of sprite sets available for the title.
            embed.WithDescription($"{OfficialSetMethods.Generate_Normal_Set_List("P4-PS2")}");

            // Create a footer based on the game version the list is from.
            var footer = new EmbedFooterBuilder
            {
                Text = "Version: P4-PS2"
            };

            // Add the footer to the embed.
            embed.WithFooter(footer);

            // Send the embeded message to the channel.
            await channel.SendMessageAsync("", false, embed.Build());
        }

        public static async Task P4G_Set_List(SocialLinkerCommand sl_command)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = sl_command.User;
            SocketTextChannel channel = (SocketTextChannel)sl_command.Channel;

            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Persona 4 Golden Conversation Portrait Sets",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            // Set the color and thumbnail for the embeded message.
            embed.WithColor(EmbedSettings.Get_Game_Color("P4G", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P4G"));

            // Create a description with the list of sprite sets available for the title.
            embed.WithDescription($"{OfficialSetMethods.Generate_Normal_Set_List("P4G")}");

            // Create a footer based on the game version the list is from.
            var footer = new EmbedFooterBuilder
            {
                Text = "Version: P4G"
            };

            // Add the footer to the embed.
            embed.WithFooter(footer);

            // Send the embeded message to the channel.
            await channel.SendMessageAsync("", false, embed.Build());
        }

        public static async Task P4AU_Set_List(SocialLinkerCommand sl_command)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = sl_command.User;
            SocketTextChannel channel = (SocketTextChannel)sl_command.Channel;

            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Persona 4 Arena Ultimax Conversation Portrait Sets",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            // Set the color and thumbnail for the embeded message.
            embed.WithColor(EmbedSettings.Get_Game_Color("P4AU", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P4AU"));

            // Create a description with the list of sprite sets available for the title.
            embed.WithDescription($"{OfficialSetMethods.Generate_Normal_Set_List("P4AU")}");

            // Send the embeded message to the channel.
            await channel.SendMessageAsync("", false, embed.Build());
        }

        public static async Task P4D_Set_List(SocialLinkerCommand sl_command)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = sl_command.User;
            SocketTextChannel channel = (SocketTextChannel)sl_command.Channel;

            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Persona 4: Dancing All Night Conversation Portrait Sets",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            // Set the color and thumbnail for the embeded message.
            embed.WithColor(EmbedSettings.Get_Game_Color("P4D", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P4D"));

            // Create a description with the list of sprite sets available for the title.
            embed.WithDescription($"{OfficialSetMethods.Generate_Normal_Set_List("P4D")}");

            // Send the embeded message to the channel.
            await channel.SendMessageAsync("", false, embed.Build());
        }

        public static async Task P5_PS4_Set_List(SocialLinkerCommand sl_command)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = sl_command.User;
            SocketTextChannel channel = (SocketTextChannel)sl_command.Channel;

            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Persona 5 Conversation Portrait Sets",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            // Set the color and thumbnail for the embeded message.
            embed.WithColor(EmbedSettings.Get_Game_Color("P5-PS4", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P5-PS4"));

            // Create a description with the list of sprite sets available for the title.
            embed.WithDescription($"{OfficialSetMethods.Generate_Normal_Set_List("P5-PS4")}");

            // Create a footer based on the game version the list is from.
            var footer = new EmbedFooterBuilder
            {
                Text = "Version: P5-PS4"
            };

            // Add the footer to the embed.
            embed.WithFooter(footer);

            // Send the embeded message to the channel.
            await channel.SendMessageAsync("", false, embed.Build());
        }

        public static async Task P5R_Set_List(SocialLinkerCommand sl_command)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = sl_command.User;
            SocketTextChannel channel = (SocketTextChannel)sl_command.Channel;

            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Persona 5 Royal Conversation Portrait Sets",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            // Set the color and thumbnail for the embeded message.
            embed.WithColor(EmbedSettings.Get_Game_Color("P5R", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P5R"));

            // Create a description with the list of sprite sets available for the title.
            embed.WithDescription($"{OfficialSetMethods.Generate_Normal_Set_List("P5R")}");

            // Create a footer based on the game version the list is from.
            var footer = new EmbedFooterBuilder
            {
                Text = "Version: P5R"
            };

            // Add the footer to the embed.
            embed.WithFooter(footer);

            // Send the embeded message to the channel.
            await channel.SendMessageAsync("", false, embed.Build());
        }

        public static async Task P5S_Set_List(SocialLinkerCommand sl_command)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = sl_command.User;
            SocketTextChannel channel = (SocketTextChannel)sl_command.Channel;

            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Persona 5 Strikers Conversation Portrait Sets",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            // Set the color and thumbnail for the embeded message.
            embed.WithColor(EmbedSettings.Get_Game_Color("P5S", null));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("P5S"));

            // Create a description with the list of sprite sets available for the title.
            embed.WithDescription($"{OfficialSetMethods.Generate_Normal_Set_List("P5S")}");

            // Send the embeded message to the channel.
            await channel.SendMessageAsync("", false, embed.Build());
        }

        public static async Task BBTAG_Set_List(SocialLinkerCommand sl_command)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = sl_command.User;
            SocketTextChannel channel = (SocketTextChannel)sl_command.Channel;

            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "BlazBlue: Cross Tag Battle Sprite Sets",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            // Assign an embed color and thumbnail based on the user's episode header setting for the BBTAG template.
            embed.WithColor(EmbedSettings.Get_Game_Color("BBTAG", account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo("BBTAG"));

            // Create a description with the list of sprite sets available for the title.
            embed.WithDescription($"{OfficialSetMethods.Generate_BBTAG_Set_List()}");

            // Send the embeded message to the channel.
            await channel.SendMessageAsync("", false, embed.Build());
        }
    }
}
