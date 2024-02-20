using System;
using System.Drawing;
using System.Threading.Tasks;
using Discord.Commands;
using System.IO;
using Discord.WebSocket;
using SocialLinker.Config;
using SocialLinker.Core.LocalStorageTables;
using Discord;
using Discord.Rest;
using SocialLinker.Core.CloudStorageTables;
using System.Net;
using Newtonsoft.Json;
using System.Collections.Generic;
using SocialLinker.Core.SceneMaker.Data.Bustup;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using SocialLinker.Core.Menus;

namespace SocialLinker.Core.SceneMaker.TemplateRenders.MakerMulti
{
    internal class RenderMultiCharBBTAG
    {
        QuickScenes.RenderBBTAG base_bbtag_rendering = new QuickScenes.RenderBBTAG();
        int template_width = 1920;
        int template_height = 1080;
        bool is_spriteless = false;

        int screen_center = 964;
        int dual_offset = 150;

        public async Task Render_Multi_Character_Scene_BBTAG(SocialLinkerCommand sl_command)
        {
            SocketUser user = sl_command.User;
            SocketTextChannel channel = (SocketTextChannel)sl_command.Channel;

            OfficialSetData set_data = sl_command.MakerCommand.Character_Data_1.Set_Data;
            MakerCommandData maker_command_data = sl_command.MakerCommand;

            RestUserMessage loader = await channel.SendMessageAsync("", false, base_bbtag_rendering.BBTAG_Loading_Message(set_data.Series).Build());

            var account = UserInfoClasses.GetAccount(user);
            sl_command.MakerCommand.Character_Data_1.Bustup_Data = BustupDataMethods.Get_Bustup_Data(account, set_data, sl_command.MakerCommand.Character_Data_1);
            BustupData bustup_data = sl_command.MakerCommand.Character_Data_1.Bustup_Data;

            if (maker_command_data.Character_Data_1.Base_Sprite == 0)
            {
                is_spriteless = true;
            }

            // Background rendering
            Bitmap base_template = new Bitmap(template_width, template_height);
            Bitmap colored_background_bitmap = OfficialSetMethods.Render_Colored_Background(account, template_width, template_height);
            Bitmap background = new Bitmap(2, 2);

            try
            {
                background = OfficialSetMethods.Render_Background(sl_command, template_width, template_height);
            }
            catch (System.ArgumentException e)
            {
                Console.WriteLine(e);
                await loader.DeleteAsync();
                _ = ErrorHandling.Incompatible_File_Type(sl_command);
                return;
            }

            // Next, time for the conversation portrait! Create and initialize a new bitmap variable for it.
            Bitmap bustup = new Bitmap(2, 2);

            if (is_spriteless == false)
            {
                bustup = OfficialSetMethods.Bustup_Selection(sl_command, account, sl_command.MakerCommand.Character_Data_1);
            }

            // If the bustup returns as null, however, something went wrong with rendering the animation frames.
            // An error message has already been sent in the frame rendering method, so delete the loading message and return.
            if (bustup == null)
            {
                await loader.DeleteAsync();
                return;
            }

            // Time to put it all together!
            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                string display_name = OfficialSetMethods.GetDisplayName(account, sl_command.MakerCommand.Character_Data_1);
                DateTime user_time = base_bbtag_rendering.Get_Date(sl_command, account);
                Bitmap header = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//BBTAG//Main//layer_1.png");
                Bitmap nametag = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//BBTAG//Main//Nametag//{base_bbtag_rendering.Series_To_Nametag(set_data.Series)}.png");
                Bitmap rendered_name = base_bbtag_rendering.Render_Name(display_name);
                Bitmap chapter_banner = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//BBTAG//Main//Chapter_Banner//{base_bbtag_rendering.Get_Chapter_Banner(account)}//{base_bbtag_rendering.Get_Day_Of_Week(account, user_time)}.png");
                Bitmap textbox = new Bitmap(2, 2);

                graphics.DrawImage(colored_background_bitmap, 0, 0, template_width, template_height);
                graphics.DrawImage(background, 0, 0, template_width, template_height);
                graphics.DrawImage(header, 0, 0, template_width, template_height);
                graphics.DrawImage(chapter_banner, 704, 33, 512, 128);

                // Draw the character bust-up to the template if the base sprite number is not '0'.
                if (is_spriteless == false)
                {
                    textbox = base_bbtag_rendering.Get_Message_Window(account);
                    //Bitmap placed_bustup = Set_Bustup_Placement(account, bustup, bustup_data, set_data);
                    //graphics.DrawImage(placed_bustup, 0, 0, template_width, template_height);
                }
                else
                {
                    textbox = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//BBTAG//Main//Message_Window//textbox_none.png");
                }

                graphics.DrawImage(textbox, 0, 0, template_width, template_height);
                graphics.DrawImage(nametag, 0, 0, template_width, template_height);
                graphics.DrawImage(rendered_name, 0, 0, template_width, template_height);
                graphics.DrawImage(base_bbtag_rendering.Render_Dialogue(maker_command_data.Dialogue), 0, 0, template_width, template_height);
            }

            // Save the entire base template to a data stream.
            MemoryStream memoryStream = new MemoryStream();
            base_template.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
            memoryStream.Seek(0, SeekOrigin.Begin);

            try
            {
                await sl_command.Channel.SendFileAsync(memoryStream, $"scene_{sl_command.User.Id}_{DateTime.UtcNow}.png");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                _ = ErrorHandling.Image_Upload_Failed(sl_command);
                memoryStream.Dispose();
                await loader.DeleteAsync();
                return;
            }

            memoryStream.Dispose();
            await loader.DeleteAsync();

            if (account.Auto_Delete_Commands == "On")
            {
                await sl_command.Message.DeleteAsync();
            }
        }

        public Bitmap Set_Bustup_Placement(UserInfoFields account, Bitmap bustup, BustupData bustup_data, int layout_number, int char_number)
        {
            // Create a starting base bitmap to render all graphics on.
            Bitmap base_template = new Bitmap(template_width, template_height);

            // Switch the rendering position of the bustup depending on the user's settings.
            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                switch (layout_number)
                {
                    case 1:
                        switch (char_number)
                        {
                            case 1:
                                graphics.DrawImage(bustup, bustup_data.BBTAG_Left_Coord_X, bustup_data.BBTAG_Left_Coord_Y, bustup_data.BBTAG_Scale_Width, bustup_data.BBTAG_Scale_Height);
                                break;

                            case 2:
                                graphics.DrawImage(bustup, bustup_data.BBTAG_Right_Coord_X, bustup_data.BBTAG_Right_Coord_Y, bustup_data.BBTAG_Scale_Width, bustup_data.BBTAG_Scale_Height);
                                break;
                        }
                        break;

                    case 2:
                        switch (char_number)
                        {
                            case 1:
                                graphics.DrawImage(bustup, bustup_data.BBTAG_Left_Coord_X - dual_offset, bustup_data.BBTAG_Left_Coord_Y, bustup_data.BBTAG_Scale_Width, bustup_data.BBTAG_Scale_Height);
                                break;

                            case 2:
                                graphics.DrawImage(bustup, bustup_data.BBTAG_Left_Coord_X + dual_offset, bustup_data.BBTAG_Left_Coord_Y, bustup_data.BBTAG_Scale_Width, bustup_data.BBTAG_Scale_Height);
                                break;
                        }
                        break;

                    case 3:
                        switch (char_number)
                        {
                            case 1:
                                graphics.DrawImage(bustup, bustup_data.BBTAG_Right_Coord_X + screen_center - dual_offset, bustup_data.BBTAG_Right_Coord_X, bustup_data.BBTAG_Scale_Width, bustup_data.BBTAG_Scale_Height);
                                break;

                            case 2:
                                graphics.DrawImage(bustup, bustup_data.BBTAG_Right_Coord_X + screen_center + dual_offset, bustup_data.BBTAG_Right_Coord_X, bustup_data.BBTAG_Scale_Width, bustup_data.BBTAG_Scale_Height);
                                break;
                        }
                        break;

                    case 4:
                        switch (char_number)
                        {
                            case 1:
                                graphics.DrawImage(bustup, bustup_data.BBTAG_Enlarged_Coord_X - bustup_data.BBTAG_Team_Center_Dist, bustup_data.BBTAG_Enlarged_Coord_Y, bustup_data.BBTAG_Enlarged_Scale_Width, bustup_data.BBTAG_Enlarged_Scale_Height);
                                break;

                            case 2:
                                graphics.DrawImage(bustup, bustup_data.BBTAG_Enlarged_Coord_X + bustup_data.BBTAG_Team_Center_Dist, bustup_data.BBTAG_Enlarged_Coord_Y, bustup_data.BBTAG_Enlarged_Scale_Width, bustup_data.BBTAG_Enlarged_Scale_Height);
                                break;
                        }
                        break;

                    case 5:
                        switch (char_number)
                        {
                            case 1:
                                graphics.DrawImage(bustup, bustup_data.BBTAG_Left_Coord_X, bustup_data.BBTAG_Left_Coord_Y, bustup_data.BBTAG_Scale_Width, bustup_data.BBTAG_Scale_Height);
                                break;

                            case 2:
                                graphics.DrawImage(bustup, bustup_data.BBTAG_Right_Coord_X + screen_center - dual_offset, bustup_data.BBTAG_Right_Coord_X, bustup_data.BBTAG_Scale_Width, bustup_data.BBTAG_Scale_Height);
                                break;

                            case 3:
                                graphics.DrawImage(bustup, bustup_data.BBTAG_Right_Coord_X + screen_center + dual_offset, bustup_data.BBTAG_Right_Coord_X, bustup_data.BBTAG_Scale_Width, bustup_data.BBTAG_Scale_Height);
                                break;
                        }
                        break;

                    case 6:
                        switch (char_number)
                        {
                            case 1:
                                graphics.DrawImage(bustup, bustup_data.BBTAG_Left_Coord_X - dual_offset, bustup_data.BBTAG_Left_Coord_Y, bustup_data.BBTAG_Scale_Width, bustup_data.BBTAG_Scale_Height);
                                break;

                            case 2:
                                graphics.DrawImage(bustup, bustup_data.BBTAG_Left_Coord_X + dual_offset, bustup_data.BBTAG_Left_Coord_Y, bustup_data.BBTAG_Scale_Width, bustup_data.BBTAG_Scale_Height);
                                break;

                            case 3:
                                graphics.DrawImage(bustup, bustup_data.BBTAG_Right_Coord_X, bustup_data.BBTAG_Right_Coord_Y, bustup_data.BBTAG_Scale_Width, bustup_data.BBTAG_Scale_Height);
                                break;
                        }
                        break;

                    case 7:
                        switch (char_number)
                        {
                            case 1:
                                graphics.DrawImage(bustup, bustup_data.BBTAG_Left_Coord_X - dual_offset, bustup_data.BBTAG_Left_Coord_Y, bustup_data.BBTAG_Scale_Width, bustup_data.BBTAG_Scale_Height);
                                break;

                            case 2:
                                graphics.DrawImage(bustup, bustup_data.BBTAG_Left_Coord_X + dual_offset, bustup_data.BBTAG_Left_Coord_Y, bustup_data.BBTAG_Scale_Width, bustup_data.BBTAG_Scale_Height);
                                break;

                            case 3:
                                graphics.DrawImage(bustup, bustup_data.BBTAG_Right_Coord_X + screen_center - dual_offset, bustup_data.BBTAG_Right_Coord_X, bustup_data.BBTAG_Scale_Width, bustup_data.BBTAG_Scale_Height);
                                break;

                            case 4:
                                graphics.DrawImage(bustup, bustup_data.BBTAG_Right_Coord_X + screen_center + dual_offset, bustup_data.BBTAG_Right_Coord_X, bustup_data.BBTAG_Scale_Width, bustup_data.BBTAG_Scale_Height);
                                break;
                        }
                        break;
                }
            }

            return base_template;
        }
    }
}
