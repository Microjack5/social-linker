using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading.Tasks;
using Discord.Rest;
using Discord.WebSocket;

namespace SocialLinker.Core.Menus
{
    public class MenuIdStructure
    {
        public SocketGuildUser User { get; set; }
        public RestUserMessage MenuMessage { get; set; }
        public string CurrentMenu { get; set; }
        public Timer MenuTimer { get; set; }
    }
}
