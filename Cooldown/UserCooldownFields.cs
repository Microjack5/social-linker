using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading.Tasks;
using Discord.Rest;
using Discord.WebSocket;

namespace SocialLinker.Cooldown
{
    public class UserCooldownFields
    {
        public SocketGuildUser User { get; set; }
        public string CommandType { get; set; }
        public int UsageCount { get; set; }
        public Timer CooldownTimer { get; set; }
        public RestUserMessage CooldownMessage { get; set; }
        public bool MessageSent { get; set; }
        public DateTime ExpirationTime { get; set; }
    }
}
