using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading.Tasks;
using Discord.Rest;
using Discord.WebSocket;
using SocialLinker.Core.CloudStorageTables;

namespace SocialLinker.Core.Menus
{
    public class MenuIdStructure
    {
        public SocketGuildUser User { get; set; }
        public UserInfoFields Account { get; set; }
        public RestUserMessage MenuMessage { get; set; }
        public string CurrentMenu { get; set; }
        public Timer MenuTimer { get; set; }
        public string InactiveMessage { get; set; }
    }
}
