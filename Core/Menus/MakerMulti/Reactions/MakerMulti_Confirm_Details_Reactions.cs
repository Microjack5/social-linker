using Discord.WebSocket;
using SocialLinker.Core.Menus.MakerMulti.Main;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SocialLinker.Core.Menus.MakerMulti.Reactions
{
    class MakerMulti_Confirm_Details_Reactions
    {
        public static Task Nav_MakerMulti_Confirm_Details_Main(SocketReaction reaction, MenuIdStructure menuSession)
        {
            if (reaction.Emote.Name == "↩️")
            {
                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Go to a new menu.
                _ = MakerMulti_Speaker_Select_Menu.MakerMulti_Speaker_Select_Main(menuSession.User, menuSession.MenuMessage);
                return Task.CompletedTask;
            }

            else if (reaction.Emote.Name == "✅")
            {
                var multimaker_session = Global.MultiMaker_Session_List.SingleOrDefault(x => x.User.Id == menuSession.User.Id);

                // Stop the timeout timer associated with the menu.
                menuSession.MenuTimer.Stop();

                // Attempt to delete the menu message from the channel if it hasn't been deleted by the user yet. If this fails, catch the exception.
                try
                {
                    _ = menuSession.MenuMessage.DeleteAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                // If the menu session is not null, remove it from the global list.
                if (menuSession != null)
                {
                    Global.MenuIdList.Remove(menuSession);
                }

                // Go to a new menu.
                switch (multimaker_session.MakerCommand.Template)
                {
                    case "P3P":
                        SceneMaker.TemplateRenders.MakerMulti.RenderMultiCharP3P p3p_render = new SceneMaker.TemplateRenders.MakerMulti.RenderMultiCharP3P();
                        _ = p3p_render.Render_Multi_Character_Scene_P3P(multimaker_session);
                        break;

                    case "P4AU":
                        SceneMaker.TemplateRenders.MakerMulti.RenderMultiCharP4AU p4au_render = new SceneMaker.TemplateRenders.MakerMulti.RenderMultiCharP4AU();
                        _ = p4au_render.Render_Multi_Character_Scene_P4AU(multimaker_session);
                        break;

                    case "P4D":
                        SceneMaker.TemplateRenders.MakerMulti.RenderMultiCharP4D p4d_render = new SceneMaker.TemplateRenders.MakerMulti.RenderMultiCharP4D();
                        _ = p4d_render.Render_Multi_Character_Scene_P4D(multimaker_session);
                        break;

                    case "BBTAG":
                        SceneMaker.TemplateRenders.MakerMulti.RenderMultiCharBBTAG bbtag_render = new SceneMaker.TemplateRenders.MakerMulti.RenderMultiCharBBTAG();
                        _ = bbtag_render.Render_Multi_Character_Scene_BBTAG(multimaker_session);
                        break;
                }

                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }
    }
}
