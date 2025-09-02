using Discord.Rest;
using Discord.WebSocket;
using SocialLinker.Config;
using SocialLinker.Core.CloudStorageTables;
using SocialLinker.Core.LocalStorageTables;
using SocialLinker.Core.SceneMaker.Data.Bustup;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialLinker.Core.SceneMaker.TemplateRenders.MakerMulti
{
    class RenderMultiCharP2IS_PSP
    {
        QuickScenes.RenderP2IS_PSP base_p2is_psp_rendering = new QuickScenes.RenderP2IS_PSP();
        int template_width = 480;
        int template_height = 272;

        public async Task Render_Multi_Character_Scene_P2IS_PSP(SocialLinkerCommand sl_command)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = sl_command.User;
            SocketTextChannel channel = (SocketTextChannel)sl_command.Channel;

            OfficialSetData set_data = sl_command.MakerCommand.Character_Data_1.Set_Data;
            MakerCommandData maker_command_data = sl_command.MakerCommand;

            // Send a loading message to the channel while the sprite sheet is being made.
            RestUserMessage loader = await channel.SendMessageAsync("", false, base_p2is_psp_rendering.P2IS_PSP_Loading_Message().Build());

            // Get the account information of the command's user.
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

            Bitmap bustup_1 = new Bitmap(2, 2);
            Bitmap bustup_2 = new Bitmap(2, 2);

            if (maker_command_data.Character_Data_1.Base_Sprite != 0)
            {
                bustup_1 = OfficialSetMethods.Bustup_Selection(sl_command, account, maker_command_data.Character_Data_1);
            }

            if (maker_command_data.Character_Data_2.Base_Sprite != 0)
            {
                bustup_2 = OfficialSetMethods.Bustup_Selection(sl_command, account, maker_command_data.Character_Data_2);
            }

            if (bustup_1 == null || bustup_2 == null)
            {
                await loader.DeleteAsync();
                return;
            }

            // Time to put it all together!
            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                // Create and assign bitmap variables for the assets needed.
                Bitmap layer_1 = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P2IS-PSP//layer_1.png");

                // Draw the layer with the user's colored default background if it exists.
                graphics.DrawImage(colored_background_bitmap, 0, 0, template_width, template_height);

                // Draw the user's background to the base template.
                graphics.DrawImage(background, 0, 0, template_width, template_height);

                // Draw the first textbox layer to the base template.
                graphics.DrawImage(layer_1, 0, 0, template_width, template_height);

                //Bustup 1
                if (maker_command_data.Character_Data_1.Base_Sprite != 0)
                {
                    Bitmap placed_bustup = Set_Bustup_Placement(sl_command, account, bustup_1, bustup_data_1, 1);
                    graphics.DrawImage(placed_bustup, 0, 0, template_width, template_height);
                }
                //Bustup 2
                if (maker_command_data.Character_Data_2.Base_Sprite != 0)
                {
                    Bitmap placed_bustup = Set_Bustup_Placement(sl_command, account, bustup_2, bustup_data_2, 2);
                    graphics.DrawImage(placed_bustup, 0, 0, template_width, template_height);
                }
            }

            System.Drawing.Color display_name_color = System.Drawing.Color.FromArgb(166, 222, 69);

            string display_name = OfficialSetMethods.GetDisplayName(account, sl_command.MakerCommand.Character_Data_1);
            display_name = OfficialSetMethods.Validate_Input(sl_command, "P2IS-PSP", "Name", display_name);

            Bitmap display_name_layer = base_p2is_psp_rendering.Render_Name(display_name);
            Rectangle display_name_area = new Rectangle(25, 200, 455, 16);

            // Create another graphics object for the base template.
            // We'll start rendering our needed text here.
            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                // Draw the input dialogue to the template.
                maker_command_data.Dialogue = OfficialSetMethods.Validate_Input(sl_command, "P2IS-PSP", "Dialogue", maker_command_data.Dialogue);
                List<string>[] dialogue_lines = OfficialSetMethods.Line_Parser(sl_command, "P2IS-PSP", maker_command_data.Dialogue, 3, 370);
                graphics.DrawImage(base_p2is_psp_rendering.Render_Dialogue(dialogue_lines, false), 0, 0, template_width, template_height);

                Bitmap cursor = base_p2is_psp_rendering.Render_Cursor();
                graphics.DrawImage(cursor, 0, 0, cursor.Width, cursor.Height);
            }

            base_template = base_p2is_psp_rendering.Scale_Template(account, base_template);

            // Save the entire base template to a data stream.
            MemoryStream memoryStream = new MemoryStream();
            base_template.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
            memoryStream.Seek(0, SeekOrigin.Begin);

            try
            {
                // Send the image.
                await sl_command.Channel.SendFileAsync(memoryStream, $"scene_{sl_command.User.Id}_{DateTime.UtcNow}.png");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

                // Send an error message to the user if the image upload fails.
                _ = ErrorHandling.Image_Upload_Failed(sl_command);

                // Clean up resources used by the stream, delete the loading message, and return.
                memoryStream.Dispose();
                await loader.DeleteAsync();
                return;
            }

            // Clean up resources used by the stream and delete the loading message.
            memoryStream.Dispose();
            await loader.DeleteAsync();

            // If the user has auto-delete for their commands set to on, delete their command as well.
            if (account.Auto_Delete_Commands == "On")
            {
                await sl_command.Message.DeleteAsync();
            }
        }

        public Bitmap Set_Bustup_Placement(SocialLinkerCommand sl_command, UserInfoFields account, Bitmap bustup, BustupData bustup_data, int char_number)
        {
            Bitmap base_template = new Bitmap(template_width, template_height);

            if (account.P2IS_PSP_TS_Invert == "On")
            {
                bustup = base_p2is_psp_rendering.Invert_Bitmap(bustup);
            }

            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                switch (char_number)
                {
                    case 1:
                        graphics.DrawImage(bustup, bustup_data.P2IS_PSP_Left_Coord_X, bustup_data.P2IS_PSP_Left_Coord_Y, bustup_data.P2IS_PSP_Scale_Width, bustup_data.P2IS_PSP_Scale_Height);
                        break;
                    case 2:
                        //bustup.RotateFlip(RotateFlipType.Rotate180FlipY);
                        graphics.DrawImage(bustup, bustup_data.P2IS_PSP_Right_Coord_X, bustup_data.P2IS_PSP_Right_Coord_Y, bustup_data.P2IS_PSP_Scale_Width, bustup_data.P2IS_PSP_Scale_Height);
                        break;
                }
            }

            return base_template;
        }
    }
}
