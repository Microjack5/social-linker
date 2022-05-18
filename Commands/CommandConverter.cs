using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using SocialLinker.Config;

namespace SocialLinker.Commands
{
    public class SocialLinkerCommand
    {
        public string CommandType { get; set; }
        public string CommandName { get; set; }
        public SocketUser Author { get; set; }
        public ISocketMessageChannel Channel { get; set; }
        public SocketUser MentionedUser { get; set; }
        public string Content { get; set; }
        public IReadOnlyCollection<Attachment> Attachments { get; set; }
    }

    public class CommandConverter : InteractionModuleBase<SocketInteractionContext>
    {
        private async Task SlashCommandConverter(SocketSlashCommand command)
        {
            SocketUser data_to_mentioneduser;
            string data_to_string;

            if ((command.Data != null)) // Maker, help, settings, shop, status
            {
                data_to_mentioneduser = (SocketUser)command.Data.Options.First().Value;
                data_to_string = null;
            }
            else
            {
                data_to_mentioneduser = null;
                data_to_string = command.Data.Options.ToString();
            }

            SocialLinkerCommand slash_to_command = new SocialLinkerCommand
            {
                CommandType = "Slash",
                CommandName = command.CommandName,
                Author = command.User,
                Channel = command.Channel,
                MentionedUser = data_to_mentioneduser,
                Content = data_to_string
            };
        }

        private async Task ContextCommandConverter(SocketMessage message)
        {
            List<string> input_substring;

            char[] delimiterChars = { ' ' };

            input_substring = message.Content.Split(delimiterChars).ToList();

            int prefix_length = $"{BotConfig.bot.cmdPrefix}".Length;

            string parsed_command_name = input_substring[0].Substring(prefix_length);

            if (input_substring.Count > 1)
            {
                input_substring.RemoveAt(0);
            }

            string parsed_content = String_List_To_String(input_substring);

            SocialLinkerCommand context_to_command = new SocialLinkerCommand
            {
                CommandType = "Context",
                CommandName = parsed_command_name,
                Author = message.Author,
                Channel = message.Channel,
                MentionedUser = message.MentionedUsers.First(),
                Content = parsed_content,
                Attachments = message.Attachments
            };
        }

        public static string String_List_To_String(List<string> input_list)
        {
            // Create an empty string variable.
            string output_string = "";

            // Iterate through each index of the list and add it to the string variable.
            for (int i = 0; i < input_list.Count; i++)
            {
                output_string += input_list[i];
            }

            // Return the string variable.
            return output_string;
        }
    }
}
