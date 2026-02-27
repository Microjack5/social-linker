using Discord.WebSocket;
using System.Collections.Generic;

namespace SocialLinker.Core.StatusScreens.Decor
{
    public static class Decor_TMS_Yashiro_1
    {
        public const string decor_id = "Decor_TMS_Yashiro_1";

        public static List<string> rank_titles = new List<string>
        {
            "Cold-Hearted",
            "Proud Gourmet",
            "Newly Bonded",
            "Best of the Best"
        };

        public static async void RenderImage(SocketUser user, ISocketMessageChannel channel)
        {
            Shared_TMS_Methods.RenderImage(user, channel, decor_id, rank_titles);
        }
    }
}
