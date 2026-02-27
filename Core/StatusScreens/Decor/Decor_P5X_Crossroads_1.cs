using Discord.WebSocket;

namespace SocialLinker.Core.StatusScreens.Decor
{
    public static class Decor_P5X_Crossroads_1
    {
        public const string decor_id = "Decor_P5X_Crossroads_1";

        public static async void RenderImage(SocketUser user, ISocketMessageChannel channel)
        {
            Shared_P5X_Crossroads_Methods.RenderImage(user, channel, decor_id);
        }
    }
}
