using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using SocialLinker.Core.LocalStorageTables;

namespace SocialLinker.Core.Menus.Settings.Main.SceneMaker.DisplayNames
{
    internal class Display_Names_Character_Select_Menu
    {
        public static async Task Display_Names_Character_Select_Main(MenuIdStructure menuSession)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var new_name_data = Global.DisplayNameTempList.SingleOrDefault(x => x.User_ID == $"{menuSession.User.Id}");

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Choose a Character",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            embed.WithColor(EmbedSettings.Get_Game_Color(new_name_data.Game, account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo(new_name_data.Game));

            embed.WithDescription("" +
                $"Choose [Enter Sprite Set Name] and type in the name of the {AcronymToTitle(new_name_data.Game)} sprite set you'd like to change the display name of.\n");

            menuSession.CurrentMenu = "Display_Names_Character_Select_Main";

            var component = new ComponentBuilder()
                .WithButton("Enter Sprite Set Name", customId: "display-names-character-select-modal-open", ButtonStyle.Primary)
                .WithButton("↩️ Return", customId: "return", ButtonStyle.Secondary);

            await Utility.CleanMessage(menuSession, embed, component, true);
            Utility.NewTimer(menuSession);
        }

        public static async Task Display_Names_Character_Select_Modal(SocketMessageComponent component)
        {
            if (component.Data.CustomId == "display-names-character-select-modal-open")
            {
                try
                {
                    var modal = new ModalBuilder()
                    .WithTitle("Sprite Set Entry")
                    .WithCustomId("display-names-character-select-modal-submit")
                    .AddTextInput("Name of Sprite Set", "name");

                    await component.RespondWithModalAsync(modal.Build());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public static async Task Display_Names_Character_Select_Error(MenuIdStructure menuSession, string user_input)
        {
            var account = menuSession.Account;
            var user = menuSession.User;
            var message = menuSession.MenuMessage;

            var naming_session = Global.DisplayNameTempList.SingleOrDefault(x => x.User_ID == $"{menuSession.User.Id}");

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = $"Sprite Set Not Found",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Profile_Config_Thumbnail(account));

            embed.WithDescription("" +
                $"There doesn’t seem to be a sprite set with the keyword \"{user_input}\" in {OfficialSetMethods.AcronymToFullTitle(naming_session.Game)}.\n" +
                $"\n" +
                $"Make sure the character’s keyword is typed correctly and try again.\n");

            menuSession.CurrentMenu = "Display_Names_Character_Select_Error";

            var component = new ComponentBuilder()
                .WithButton("↩️ Retry", customId: "return", ButtonStyle.Secondary);

            await Utility.CleanMessage(menuSession, embed, component, true);
            Utility.NewTimer(menuSession);
        }

        // Utility
        public static string AcronymToTitle(string acronym)
        {
            switch (acronym)
            {
                case "P1-PS1":
                    return "Revelations: Persona";

                case "P1-PSP":
                    return "Persona (PSP®️)";

                case "P2IS-PS1":
                    return "Persona 2: Innocent Sin (PlayStation®️)";

                case "P2IS-PSP":
                    return "Persona 2: Innocent Sin (PSP®️)";

                case "P2EP-PS1":
                    return "Persona 2: Eternal Punishment (PlayStation®️)";

                case "P2EP-PSP":
                    return "Persona 2: Eternal Punishment (PSP®️)";

                case "P3F":
                    return "Persona 3 FES";

                case "P3P":
                    return "Persona 3 Portable";

                case "P4-PS2":
                    return "Persona 4 (PlayStation®️ 2)";

                case "P4G":
                    return "Persona 4 Golden";

                case "P4AU":
                    return "Persona 4 Arena Ultimax";

                case "P4D":
                    return "Persona 4: Dancing All Night";

                case "P5-PS4":
                    return "Persona 5 (PlayStation®️ 4)";

                case "P5R":
                    return "Persona 5 Royal";

                case "BBTAG":
                    return "BlazBlue: Cross Tag Battle";

                case "P5S":
                    return "Persona 5 Strikers";

                default:
                    return "---";
            }
        }
    }
}
