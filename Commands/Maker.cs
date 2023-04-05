using System.Threading.Tasks;
using Discord.Commands;
using SocialLinker.Core.CloudStorageTables;
using SocialLinker.Cooldown;
using SocialLinker.Core.Menus.InitialUsage.Main;
using SocialLinker.Core.SceneMaker;

namespace SocialLinker.Commands
{
    public class Maker : ModuleBase<SocketCommandContext>
    {
        public static async Task MakerCommandParser(SocialLinkerCommand sl_command)
        {
            // If there is a cooldown session active for the command type "scene", return the method immediately.
            if (await UserCooldownMethods.IsCooldownActive(sl_command, "scene") == true)
            {
                return;
            }

            // Get the account information of the command's user.
            var command_user_account = UserInfoClasses.GetAccount(sl_command.User);

            // Check if the user's account has been activated. If not, send them to the initial usage setup menu.
            if (command_user_account.Account_Activated == "No")
            {
                await First_Use_Content_Filter_Menu.First_Use_Content_Filter_Initialize(sl_command);
                return;
            }

            // End of initial usage and cooldown checks.

            CommandParser command_parser = new CommandParser();
            await command_parser.Type_Directory(sl_command);
        }
    }
}
