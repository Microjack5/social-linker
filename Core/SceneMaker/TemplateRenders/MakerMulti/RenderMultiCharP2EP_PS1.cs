using Discord.Rest;
using Discord.WebSocket;
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
    class RenderMultiCharP2EP_PS1
    {
        QuickScenes.RenderP2EP_PS1 base_p2ep_ps1_rendering = new QuickScenes.RenderP2EP_PS1();
        int template_width = 320;
        int template_height = 240;

        public async Task Render_Multi_Character_Scene_P2EP_PS1(SocialLinkerCommand sl_command)
        {
            SocketUser user = sl_command.User;
            SocketTextChannel channel = (SocketTextChannel)sl_command.Channel;

            OfficialSetData set_data = sl_command.MakerCommand.Character_Data_1.Set_Data;
            MakerCommandData maker_command_data = sl_command.MakerCommand;

            RestUserMessage loader = await channel.SendMessageAsync("", false, base_p2ep_ps1_rendering.P2EP_PS1_Loading_Message().Build());

            var account = UserInfoClasses.GetAccount(user);

            OfficialSetData set_data_1 = sl_command.MakerCommand.Character_Data_1.Set_Data;
            OfficialSetData set_data_2 = sl_command.MakerCommand.Character_Data_2.Set_Data;

            sl_command.MakerCommand.Character_Data_1.Bustup_Data = BustupDataMethods.Get_Bustup_Data(account, set_data_1, maker_command_data.Character_Data_1);
            sl_command.MakerCommand.Character_Data_2.Bustup_Data = BustupDataMethods.Get_Bustup_Data(account, set_data_2, maker_command_data.Character_Data_2);
            BustupData bustup_data_1 = sl_command.MakerCommand.Character_Data_1.Bustup_Data;
            BustupData bustup_data_2 = sl_command.MakerCommand.Character_Data_2.Bustup_Data;

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
                graphics.DrawImage(colored_background_bitmap, 0, 0, template_width, template_height);
                graphics.DrawImage(background, 0, 0, template_width, template_height);

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

                graphics.DrawImage(base_p2ep_ps1_rendering.Render_Message_Window(account), 0, 0, template_width, template_height);

                string display_name = sl_command.MakerCommand.Display_Name;
                display_name = OfficialSetMethods.Validate_Input(sl_command, "P2EP-PS1", "Name", display_name);

                maker_command_data.Dialogue = OfficialSetMethods.Validate_Input(sl_command, "P2EP-PS1", "Dialogue", maker_command_data.Dialogue);
                List<string>[] dialogue_lines = OfficialSetMethods.Line_Parser(sl_command, "P2EP-PS1", maker_command_data.Dialogue, 3, 230);

                graphics.DrawImage(base_p2ep_ps1_rendering.Combined_Text_Layers(display_name, dialogue_lines), 0, 0, template_width, template_height);
                graphics.DrawImage(base_p2ep_ps1_rendering.Render_Cursor(), 0, 0, template_width, template_height);
            }

            base_template = base_p2ep_ps1_rendering.Scale_Template(account, base_template);

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
            Bitmap base_template = new Bitmap(template_width, template_height);

            if (account.P2EP_PSX_TS_Invert == "On")
            {
                bustup = base_p2ep_ps1_rendering.Invert_Bitmap(bustup);
            }

            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                switch (char_number)
                {
                    case 1:
                        graphics.DrawImage(bustup, bustup_data.P2EP_PSX_Left_Coord_X, bustup_data.P2EP_PSX_Left_Coord_Y, bustup_data.P2EP_PSX_Scale_Width, bustup_data.P2EP_PSX_Scale_Height);
                        break;
                    case 2:
                        //bustup.RotateFlip(RotateFlipType.Rotate180FlipY);
                        graphics.DrawImage(bustup, bustup_data.P2EP_PSX_Right_Coord_X, bustup_data.P2EP_PSX_Right_Coord_Y, bustup_data.P2EP_PSX_Scale_Width, bustup_data.P2EP_PSX_Scale_Height);
                        break;
                }
            }

            return base_template;
        }
    }
}
