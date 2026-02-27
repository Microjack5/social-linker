using Discord.WebSocket;
using System.Collections.Generic;

namespace SocialLinker.Core.StatusScreens.Decor
{
    public static class Decor_TMS_Barry_1
    {
        public const string decor_id = "Decor_TMS_Barry_1";

        public static List<string> rank_titles = new List<string>
        {
            "Trainer/Fanboy",
            "Mamori Maniac",
            "Drill Sergeant",
            "Shredding Again"
        };

        public static async void RenderImage(SocketUser user, ISocketMessageChannel channel)
        {
            Shared_TMS_Methods.RenderImage(user, channel, decor_id, rank_titles);
        }
    }
}
