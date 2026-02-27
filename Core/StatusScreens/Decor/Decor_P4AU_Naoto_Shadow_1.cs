using Discord.WebSocket;

namespace SocialLinker.Core.StatusScreens.Decor
{
    public static class Decor_P4AU_Naoto_Shadow_1
    {
        public const string decor_id = "Decor_P4AU_Naoto_Shadow_1";

        public static async void RenderImage(SocketUser user, ISocketMessageChannel channel)
        {
            Shared_P4AU_Methods.RenderImage(user, channel, decor_id);
        }
    }
}
