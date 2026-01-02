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
using SocialLinker.Core.LocalStorageTables;

namespace SocialLinker.Core.Menus.Settings.Main.SceneMaker.DisplayNames
{
    internal class Display_Names_Confirm_Menu
    {
        public static async Task Display_Name_Confirm_Main(SocketGuildUser user, RestUserMessage message)
        {
            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);
            
            // Find the menu and item sessions associated with the current user.
            var menuSession = Global.MenuIdList.SingleOrDefault(x => x.User.Id == user.Id);
            var new_name_data = Global.DisplayNameTempList.SingleOrDefault(x => x.User_ID == $"{menuSession.User.Id}");

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = $"Display Name Added",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            // Assign a color and thumbnail to the embeded message based on the title being edited.
            embed.WithColor(EmbedSettings.Get_Game_Color(new_name_data.Game, account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo(new_name_data.Game));

            OfficialSetData set_data = new_name_data.Sprite_Set;

            embed.WithDescription("" +
                $"All set! The following display name has been added:\n" +
                $"\n" +
                $"**Display Name:** {new_name_data.Display_Name}\n" +
                $"**Character:** {new_name_data.Sprite_Set.Name}\n" +
                $"**Game:** {new_name_data.Game}\n" +
                $"**Sprite Numbers Affected:** {DisplayNameLogging.String_Range_To_Int_Range(account, set_data, DisplayNameLogging.String_To_String_List(new_name_data.Sprites_Affected), new_name_data)}\n" +
                $"**Spriteless Affected:** {new_name_data.Spriteless_Included}\n" +
                $"\n");

            // Edit the menu session according to the current message.
            menuSession.CurrentMenu = "Display_Names_Confirm_Main";

            var component = new ComponentBuilder()
                .WithButton("💠 Display Names", customId: "display-names", ButtonStyle.Secondary);

            await Utility.CleanMessage(menuSession, embed, component);
            Utility.NewTimer(menuSession);
        }
    }
}
