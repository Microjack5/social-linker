using System.Collections.Generic;
using Discord;
using Discord.WebSocket;

namespace SocialLinker.Commands
{
    public class SocialLinkerCommand
    {
        public string CommandType { get; set; }
        public string CommandName { get; set; }
        public SocketUser Author { get; set; }
        public ISocketMessageChannel Channel { get; set; }
        public string Content { get; set; }
        public IReadOnlyCollection<Attachment> Attachments { get; set; }
    }
}
