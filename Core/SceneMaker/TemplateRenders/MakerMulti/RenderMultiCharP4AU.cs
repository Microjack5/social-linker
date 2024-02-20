using System;
using System.Drawing;
using System.Threading.Tasks;
using System.IO;
using Discord.WebSocket;
using SocialLinker.Config;
using SocialLinker.Core.LocalStorageTables;
using Discord.Rest;
using SocialLinker.Core.CloudStorageTables;
using SocialLinker.Core.SceneMaker.Data.Bustup;

namespace SocialLinker.Core.SceneMaker.TemplateRenders.MakerMulti
{
    internal class RenderMultiCharP4AU
    {
        QuickScenes.RenderP4AU base_p4au_rendering = new QuickScenes.RenderP4AU();
        int template_width = 1280;
        int template_height = 720;

        public async Task Render_Multi_Character_Scene_P4AU(SocialLinkerCommand sl_command)
        {
            SocketUser user = sl_command.User;
            SocketTextChannel channel = (SocketTextChannel)sl_command.Channel;

            OfficialSetData set_data = sl_command.MakerCommand.Character_Data_1.Set_Data;
            MakerCommandData maker_command_data = sl_command.MakerCommand;

            RestUserMessage loader = await channel.SendMessageAsync("", false, base_p4au_rendering.P4AU_Loading_Message().Build());

            var account = UserInfoClasses.GetAccount(user);

            OfficialSetData set_data_1 = sl_command.MakerCommand.Character_Data_1.Set_Data;
            OfficialSetData set_data_2 = sl_command.MakerCommand.Character_Data_2.Set_Data;

            sl_command.MakerCommand.Character_Data_1.Bustup_Data = BustupDataMethods.Get_Bustup_Data(account, set_data_1, maker_command_data.Character_Data_1);
            sl_command.MakerCommand.Character_Data_2.Bustup_Data = BustupDataMethods.Get_Bustup_Data(account, set_data_2, maker_command_data.Character_Data_2);
            BustupData bustup_data_1 = sl_command.MakerCommand.Character_Data_1.Bustup_Data;
            BustupData bustup_data_2 = sl_command.MakerCommand.Character_Data_2.Bustup_Data;

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
            Bitmap bustup_1 = new Bitmap(2, 2);
            Bitmap bustup_2 = new Bitmap(2, 2);

            // Check if the base sprite number is something other than zero.
            // If it is zero, we have nothing to render. Otherwise, retrieve the bustup.
            if (maker_command_data.Character_Data_1.Base_Sprite != 0)
            {
                bustup_1 = OfficialSetMethods.Bustup_Selection(sl_command, account, maker_command_data.Character_Data_1);
            }

            if (maker_command_data.Character_Data_2.Base_Sprite != 0)
            {
                bustup_2 = OfficialSetMethods.Bustup_Selection(sl_command, account, maker_command_data.Character_Data_2);
            }

            // Time to put it all together!
            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                Bitmap layer_1 = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P4AU//Main//layer_1.png");
                Bitmap layer_2 = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P4AU//Main//layer_2.png");

                graphics.DrawImage(colored_background_bitmap, 0, 0, template_width, template_height);
                graphics.DrawImage(background, 0, 0, template_width, template_height);

                // Draw the character bust-up to the template if the base sprite number is not '0'.
                //Bustup 1
                if (maker_command_data.Character_Data_1.Base_Sprite != 0)
                {
                    Bitmap placed_bustup = Set_Bustup_Placement(sl_command, account, bustup_1, bustup_data_1, 1);
                    graphics.DrawImage(placed_bustup, 0, 0, template_width, template_height);
                }
                //Bustup 2
                if (maker_command_data.Character_Data_2.Base_Sprite != 0)
                {
                    Bitmap placed_bustup = Set_Bustup_Placement(sl_command, account, bustup_1, bustup_data_2, 2);
                    graphics.DrawImage(placed_bustup, 0, 0, template_width, template_height);
                }

                Bitmap text_overlay = new Bitmap(2, 2);

                maker_command_data.Dialogue = OfficialSetMethods.Validate_Input(sl_command, "P4AU", "Dialogue", maker_command_data.Dialogue);

                switch (account.P4AU_TS_Scene_Type)
                {
                    case "Dialogue":
                        string display_name = sl_command.MakerCommand.Display_Name;
                        text_overlay = base_p4au_rendering.Render_Dialogue_Overlay(sl_command, account, maker_command_data, display_name);
                        break;

                    case "Narration":
                        text_overlay = base_p4au_rendering.Render_Narration_Overlay(sl_command, account, maker_command_data);
                        break;
                }

                graphics.DrawImage(text_overlay, 0, 0, template_width, template_height);

                Bitmap control_guide = base_p4au_rendering.Render_Control_Guide(account);

                graphics.DrawImage(control_guide, 0, 0, control_guide.Width, control_guide.Height);
            }

            base_template = base_p4au_rendering.Scale_Template(account, base_template);

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

        public Bitmap Set_Bustup_Placement(SocialLinkerCommand sl_command, UserInfoFields account, Bitmap bustup, BustupData bustup_data, int char_number)
        {
            // Create a starting base bitmap to render all graphics on.
            Bitmap base_template = new Bitmap(template_width, template_height);

            // Switch the rendering position of the bustup depending on the user's settings.
            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                switch (char_number)
                {
                    case 1:
                        Bitmap bustup_yellow = base_p4au_rendering.Bitmap_To_Color(bustup, System.Drawing.Color.FromArgb(240, 253, 39), new Rectangle(0, 0, bustup.Width, bustup.Height));
                        Bitmap bustup_white = base_p4au_rendering.Bitmap_To_Color(bustup, System.Drawing.Color.White, new Rectangle(0, 0, bustup.Width, bustup.Height));

                        if ((account.P4AU_TS_Scene_Type == "Dialogue") && (account.P4AU_TS_Highlight == "On") && (sl_command.MakerCommand.P4AU_Multi_Char_Protag_Highlight_Toggle == true))
                        {
                            graphics.DrawImage(bustup_yellow, bustup_data.P4AU_Right_Coord_X - 16, bustup_data.P4AU_Right_Coord_Y, bustup_data.P4AU_Scale_Width, bustup_data.P4AU_Scale_Height);
                            graphics.DrawImage(bustup_white, bustup_data.P4AU_Right_Coord_X - 6, bustup_data.P4AU_Right_Coord_Y, bustup_data.P4AU_Scale_Width, bustup_data.P4AU_Scale_Height);
                        }

                        graphics.DrawImage(bustup, bustup_data.P4AU_Right_Coord_X, bustup_data.P4AU_Right_Coord_Y, bustup_data.P4AU_Scale_Width, bustup_data.P4AU_Scale_Height);
                        break;
                    case 2:
                        bustup = OfficialSetMethods.Reverse_Bustup_Selection(sl_command, account, sl_command.MakerCommand.Character_Data_2, bustup);

                        if (bustup_data.P4AU_Dual_Flip == true)
                        {
                            bustup.RotateFlip(RotateFlipType.RotateNoneFlipX);
                        }

                        graphics.DrawImage(bustup, bustup_data.P4AU_Left_Coord_X, bustup_data.P4AU_Left_Coord_Y, bustup_data.P4AU_Scale_Width, bustup_data.P4AU_Scale_Height);
                        break;
                }
            }

            return base_template;
        }
    }
}
