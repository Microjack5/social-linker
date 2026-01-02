using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace SocialLinker.Core.Menus.Settings.Main.SceneMaker.DisplayNames
{
    internal class Display_Names_Custom_Input_Menu
    {
        public static async Task Display_Names_Custom_Input_Main(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var new_name_data = Global.DisplayNameTempList.SingleOrDefault(x => x.User_ID == $"{menuSession.User.Id}");

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = $"Custom Name Input",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            embed.WithColor(EmbedSettings.Get_Game_Color(new_name_data.Game, account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo(new_name_data.Game));

            embed.WithDescription("" +
                $"Type in the new display name you'd like for the selected sprites.\n" +
                $"Please keep the new name within 32 characters.\n");

            menuSession.CurrentMenu = "Display_Names_Custom_Input_Main";

            var component = new ComponentBuilder()
                .WithButton("Enter Display Name", customId: "display-names-custom-input-modal-open", ButtonStyle.Primary)
                .WithButton("↩️ Return", customId: "return", ButtonStyle.Secondary);

            await Utility.CleanMessage(menuSession, embed, component, true);
            Utility.NewTimer(menuSession);
        }

        public static async Task Display_Names_Custom_Input_Modal(SocketMessageComponent component)
        {
            if (component.Data.CustomId == "display-names-custom-input-modal-open")
            {
                try
                {
                    var modal = new ModalBuilder()
                    .WithTitle("Name Entry")
                    .WithCustomId("display-names-custom-input-modal-submit")
                    .AddTextInput("Display Name", "name");

                    await component.RespondWithModalAsync(modal.Build());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public static async Task Display_Names_Custom_Input_Error_1(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var new_name_data = Global.DisplayNameTempList.SingleOrDefault(x => x.User_ID == $"{menuSession.User.Id}");

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = $"Display Name Too Long",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            embed.WithColor(EmbedSettings.Get_Game_Color(new_name_data.Game, account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo(new_name_data.Game));

            embed.WithDescription("" +
                $"The display name \"{new_name_data.Display_Name}\" is {new_name_data.Display_Name.Length} characters long. Please keep the name within 32 characters and try again.");

            menuSession.CurrentMenu = "Display_Names_Custom_Input_Error_1";

            var component = new ComponentBuilder()
                .WithButton("↩️ Retry", customId: "return", ButtonStyle.Secondary);

            await Utility.CleanMessage(menuSession, embed, component, true);
            Utility.NewTimer(menuSession);
        }
    }
}
