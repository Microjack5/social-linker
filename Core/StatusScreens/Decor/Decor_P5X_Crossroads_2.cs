using Discord.WebSocket;

namespace SocialLinker.Core.StatusScreens.Decor
{
    public class Decor_P5X_Crossroads_2
    {
        public const string decor_id = "Decor_P5X_Crossroads_2";

        public static async void RenderImage(SocketUser user, ISocketMessageChannel channel)
        {
            Shared_P5X_Crossroads_Methods.RenderImage(user, channel, decor_id);
        }
    }
}
