using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Discord.Addons.Interactive;
using SocialLinker.Core.CloudStorageTables;
using SocialLinker.Cooldown;
using SocialLinker.Core.Menus.InitialUsage.Main;
using SocialLinker.Core.SceneMaker;

namespace SocialLinker.Commands
{
    public class Maker : InteractiveBase<SocketCommandContext>
    {
        [Command("maker", RunMode = RunMode.Async)]
        public async Task MakerCommandParser([Remainder] string param = "")
        {
            // If there is a cooldown session active for the command type "scene", return the method immediately.
            if (await UserCooldownMethods.IsCooldownActive(Context.Message, "scene") == true)
            {
                return;
            }

            // Get the account information of the command's user.
            var command_user_account = UserInfoClasses.GetAccount(Context.User);

            // Check if the user's account has been activated. If not, send them to the initial usage setup menu.
            if (command_user_account.Account_Activated == "No")
            {
                await First_Use_Content_Filter_Menu.First_Use_Content_Filter_Start((SocketTextChannel)Context.Channel, (SocketGuildUser)Context.User);
                return;
            }

            // End of initial usage and cooldown checks.

            SocketMessage message = Context.Message;

            await CommandParser.Parser(message, param);
        }
    }
}
