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
        int dual_offset = 150;

        public async Task Render_Multi_Character_Scene_BBTAG(SocialLinkerCommand sl_command)
        {
            SocketUser user = sl_command.User;
            SocketTextChannel channel = (SocketTextChannel)sl_command.Channel;
            MakerCommandData maker_command_data = sl_command.MakerCommand;

            var account = UserInfoClasses.GetAccount(user);

            OfficialSetData set_data_1 = null;
            OfficialSetData set_data_2 = null;
            OfficialSetData set_data_3 = null;
            OfficialSetData set_data_4 = null;

            BustupData bustup_data_1 = null;
            BustupData bustup_data_2 = null;
            BustupData bustup_data_3 = null;
            BustupData bustup_data_4 = null;

            Bitmap bustup_1 = new Bitmap(2, 2);
            Bitmap bustup_2 = new Bitmap(2, 2);
            Bitmap bustup_3 = new Bitmap(2, 2);
            Bitmap bustup_4 = new Bitmap(2, 2);

            switch (sl_command.MakerCommand.Expected_Characters)
            {
                case 4:
                    set_data_4 = sl_command.MakerCommand.Character_Data_4.Set_Data;
                    sl_command.MakerCommand.Character_Data_4.Bustup_Data = BustupDataMethods.Get_Bustup_Data(account, set_data_4, maker_command_data.Character_Data_4);
                    bustup_data_4 = sl_command.MakerCommand.Character_Data_4.Bustup_Data;

                    if (maker_command_data.Character_Data_4.Base_Sprite != 0)
                    {
                        bustup_4 = OfficialSetMethods.Bustup_Selection(sl_command, account, maker_command_data.Character_Data_4);
                    }
                    goto case 3;

                case 3:
                    set_data_3 = sl_command.MakerCommand.Character_Data_3.Set_Data;
                    sl_command.MakerCommand.Character_Data_3.Bustup_Data = BustupDataMethods.Get_Bustup_Data(account, set_data_3, maker_command_data.Character_Data_3);
                    bustup_data_3 = sl_command.MakerCommand.Character_Data_3.Bustup_Data;

                    if (maker_command_data.Character_Data_3.Base_Sprite != 0)
                    {
                        bustup_3 = OfficialSetMethods.Bustup_Selection(sl_command, account, maker_command_data.Character_Data_3);
                    }
                    goto case 2;

                case 2:
                    set_data_2 = sl_command.MakerCommand.Character_Data_2.Set_Data;
                    sl_command.MakerCommand.Character_Data_2.Bustup_Data = BustupDataMethods.Get_Bustup_Data(account, set_data_2, maker_command_data.Character_Data_2);
                    bustup_data_2 = sl_command.MakerCommand.Character_Data_2.Bustup_Data;

                    if (maker_command_data.Character_Data_2.Base_Sprite != 0)
                    {
                        bustup_2 = OfficialSetMethods.Bustup_Selection(sl_command, account, maker_command_data.Character_Data_2);
                    }
                    goto case 1;

                case 1:
                    set_data_1 = sl_command.MakerCommand.Character_Data_1.Set_Data;
                    sl_command.MakerCommand.Character_Data_1.Bustup_Data = BustupDataMethods.Get_Bustup_Data(account, set_data_1, maker_command_data.Character_Data_1);
                    bustup_data_1 = sl_command.MakerCommand.Character_Data_1.Bustup_Data;

                    if (maker_command_data.Character_Data_1.Base_Sprite != 0)
                    {
                        bustup_1 = OfficialSetMethods.Bustup_Selection(sl_command, account, maker_command_data.Character_Data_1);
                    }
                    break;
            }

            RestUserMessage loader = await channel.SendMessageAsync("", false, base_bbtag_rendering.BBTAG_Loading_Message(set_data_1.Series).Build());

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

            if (account.BBTAG_TS_BG_Blur == "On" && sl_command.MakerCommand.Background != null)
            {
                background = base_bbtag_rendering.Blur_Background(background);
            }

            try
            {
                // Time to put it all together!
                using (Graphics graphics = Graphics.FromImage(base_template))
                {
                    string display_name = sl_command.MakerCommand.Display_Name;
                    DateTime user_time = base_bbtag_rendering.Get_Date(sl_command, account);
                    Bitmap header = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//BBTAG//Main//layer_1.png");
                    Bitmap rendered_name = base_bbtag_rendering.Render_Name(display_name ?? "");
                    Bitmap chapter_banner = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//BBTAG//Main//Chapter_Banner//{base_bbtag_rendering.Get_Chapter_Banner(account)}//{base_bbtag_rendering.Get_Day_Of_Week(account, user_time)}.png");
                    Bitmap nametag = new Bitmap(2, 2);
                    Bitmap textbox = new Bitmap(2, 2);

                    graphics.DrawImage(colored_background_bitmap, 0, 0, template_width, template_height);
                    graphics.DrawImage(background, 0, 0, template_width, template_height);
                    graphics.DrawImage(header, 0, 0, template_width, template_height);
                    graphics.DrawImage(chapter_banner, 704, 33, 512, 128);

                    Bitmap bustup_layer = new Bitmap(1920, 1080);
                    int selected_bbtag_layout = Int32.Parse(maker_command_data.BBTAG_Specific_Data.Layout);

                    Bitmap bustup_layer_1 = Set_Bustup_Placement(account, bustup_1, bustup_data_1, selected_bbtag_layout, 1);
                    Bitmap bustup_layer_2 = Set_Bustup_Placement(account, bustup_2, bustup_data_2, selected_bbtag_layout, 2);
                    Bitmap bustup_layer_3 = Set_Bustup_Placement(account, bustup_3, bustup_data_3, selected_bbtag_layout, 3);
                    Bitmap bustup_layer_4 = Set_Bustup_Placement(account, bustup_4, bustup_data_4, selected_bbtag_layout, 4);

                    graphics.DrawImage(bustup_layer_1, 0, 0, bustup_layer_1.Width, bustup_layer_1.Height);
                    graphics.DrawImage(bustup_layer_2, 0, 0, bustup_layer_2.Width, bustup_layer_2.Height);
                    graphics.DrawImage(bustup_layer_4, 0, 0, bustup_layer_4.Width, bustup_layer_4.Height);
                    graphics.DrawImage(bustup_layer_3, 0, 0, bustup_layer_3.Width, bustup_layer_3.Height);

                    string textbox_type = "";

                    // Nametag
                    if (maker_command_data.BBTAG_Specific_Data.Speaker_Series != default)
                    {
                        nametag = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//BBTAG//Main//Nametag//{base_bbtag_rendering.Series_To_Nametag(maker_command_data.BBTAG_Specific_Data.Speaker_Series)}.png");
                    }

                    // Textbox
                    if (maker_command_data.BBTAG_Specific_Data.Speaker_Is_Spriteless)
                    {
                        textbox_type = "textbox_none";
                    }
                    else
                    {
                        switch (maker_command_data.BBTAG_Specific_Data.Speaker)
                        {
                            case "system_1":
                                textbox_type = "system_1";
                                break;

                            case "system_2":
                                textbox_type = "system_2";
                                break;

                            case "offscreen":
                                textbox_type = "textbox_none";
                                break;

                            default:
                                switch (maker_command_data.BBTAG_Specific_Data.Layout)
                                {
                                    case "1":
                                        textbox_type = "textbox_left";
                                        break;

                                    case "2":
                                        textbox_type = "textbox_right";
                                        break;

                                    case "3":
                                        textbox_type = "textbox_center";
                                        break;

                                    case "4":
                                        switch (maker_command_data.BBTAG_Specific_Data.Speaker)
                                        {
                                            case "char_1":
                                                textbox_type = "textbox_left";
                                                break;

                                            default:
                                                textbox_type = "textbox_right";
                                                break;
                                        }
                                        break;

                                    case "5":
                                        textbox_type = "textbox_left";
                                        break;

                                    case "6":
                                        textbox_type = "textbox_right";
                                        break;

                                    case "7":
                                        textbox_type = "textbox_center";
                                        break;

                                    case "8":
                                        switch (maker_command_data.BBTAG_Specific_Data.Speaker)
                                        {
                                            case "char_1":
                                                textbox_type = "textbox_left";
                                                break;

                                            default:
                                                textbox_type = "textbox_right";
                                                break;
                                        }
                                        break;

                                    case "9":
                                        switch (maker_command_data.BBTAG_Specific_Data.Speaker)
                                        {
                                            case "char_3":
                                                textbox_type = "textbox_right";
                                                break;

                                            default:
                                                textbox_type = "textbox_left";
                                                break;
                                        }
                                        break;

                                    case "10":
                                        switch (maker_command_data.BBTAG_Specific_Data.Speaker)
                                        {
                                            case "char_1":
                                            case "char_2":
                                                textbox_type = "textbox_left";
                                                break;

                                            default:
                                                textbox_type = "textbox_right";
                                                break;
                                        }
                                        break;

                                    default:
                                        textbox_type = "textbox_none";
                                        break;
                                }
                                break;
                        }
                    }

                    textbox = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//BBTAG//Main//Message_Window//{textbox_type}.png");

                    graphics.DrawImage(textbox, 0, 0, template_width, template_height);
                    graphics.DrawImage(nametag, 0, 0, template_width, template_height);
                    graphics.DrawImage(rendered_name, 0, 0, template_width, template_height);
                    graphics.DrawImage(base_bbtag_rendering.Render_Dialogue(maker_command_data.Dialogue), 0, 0, template_width, template_height);
                }
            }
            catch (Exception err)
            {
                Console.WriteLine(err);
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
            if (bustup_data == null)
            {
                return new Bitmap(2, 2);
            }

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
                        }
                        break;

                    case 2:
                        switch (char_number)
                        {
                            case 1:
                                graphics.DrawImage(bustup, bustup_data.BBTAG_Right_Coord_X, bustup_data.BBTAG_Right_Coord_Y, bustup_data.BBTAG_Scale_Width, bustup_data.BBTAG_Scale_Height);
                                break;
                        }
                        break;

                    case 3:
                        switch (char_number)
                        {
                            case 1:
                                graphics.DrawImage(bustup, bustup_data.BBTAG_Enlarged_Coord_X, bustup_data.BBTAG_Enlarged_Coord_Y, bustup_data.BBTAG_Enlarged_Scale_Width, bustup_data.BBTAG_Enlarged_Scale_Height);
                                break;
                        }
                        break;

                    case 4:
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

                    case 5:
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

                    case 6:
                        switch (char_number)
                        {
                            case 1:
                                graphics.DrawImage(bustup, bustup_data.BBTAG_Right_Coord_X - dual_offset, bustup_data.BBTAG_Right_Coord_Y, bustup_data.BBTAG_Scale_Width, bustup_data.BBTAG_Scale_Height);
                                break;

                            case 2:
                                graphics.DrawImage(bustup, bustup_data.BBTAG_Right_Coord_X + dual_offset, bustup_data.BBTAG_Right_Coord_Y, bustup_data.BBTAG_Scale_Width, bustup_data.BBTAG_Scale_Height);
                                break;
                        }
                        break;

                    case 7:
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

                    case 8:
                        switch (char_number)
                        {
                            case 1:
                                graphics.DrawImage(bustup, bustup_data.BBTAG_Left_Coord_X, bustup_data.BBTAG_Left_Coord_Y, bustup_data.BBTAG_Scale_Width, bustup_data.BBTAG_Scale_Height);
                                break;

                            case 2:
                                graphics.DrawImage(bustup, bustup_data.BBTAG_Right_Coord_X - dual_offset, bustup_data.BBTAG_Right_Coord_Y, bustup_data.BBTAG_Scale_Width, bustup_data.BBTAG_Scale_Height);
                                break;

                            case 3:
                                graphics.DrawImage(bustup, bustup_data.BBTAG_Right_Coord_X + dual_offset, bustup_data.BBTAG_Right_Coord_Y, bustup_data.BBTAG_Scale_Width, bustup_data.BBTAG_Scale_Height);
                                break;
                        }
                        break;

                    case 9:
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

                    case 10:
                        switch (char_number)
                        {
                            case 1:
                                graphics.DrawImage(bustup, bustup_data.BBTAG_Left_Coord_X - dual_offset, bustup_data.BBTAG_Left_Coord_Y, bustup_data.BBTAG_Scale_Width, bustup_data.BBTAG_Scale_Height);
                                break;

                            case 2:
                                graphics.DrawImage(bustup, bustup_data.BBTAG_Left_Coord_X + dual_offset, bustup_data.BBTAG_Left_Coord_Y, bustup_data.BBTAG_Scale_Width, bustup_data.BBTAG_Scale_Height);
                                break;

                            case 3:
                                graphics.DrawImage(bustup, bustup_data.BBTAG_Right_Coord_X - dual_offset, bustup_data.BBTAG_Right_Coord_Y, bustup_data.BBTAG_Scale_Width, bustup_data.BBTAG_Scale_Height);
                                break;

                            case 4:
                                graphics.DrawImage(bustup, bustup_data.BBTAG_Right_Coord_X + dual_offset, bustup_data.BBTAG_Right_Coord_Y, bustup_data.BBTAG_Scale_Width, bustup_data.BBTAG_Scale_Height);
                                break;
                        }
                        break;
                }
            }

            return base_template;
        }
    }
}
