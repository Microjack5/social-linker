using System;
using System.Drawing;
using System.Threading.Tasks;
using System.IO;
using Discord.WebSocket;
using SocialLinker.Config;
using SocialLinker.Core.LocalStorageTables;
using Discord.Rest;
using SocialLinker.Core.CloudStorageTables;
using System.Collections.Generic;
using SocialLinker.Core.SceneMaker.Data.Bustup;

namespace SocialLinker.Core.SceneMaker.TemplateRenders.MakerMulti
{
    public class RenderP3P
    {
        QuickScenes.RenderP3P base_p3p_rendering = new QuickScenes.RenderP3P();
        int template_width = 480;
        int template_height = 272;
        int max_line_length = 360;
        int error_counter = 0;

        public async Task Render_Quick_Scene_P3P(SocialLinkerCommand sl_command)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = sl_command.User;
            SocketTextChannel channel = (SocketTextChannel)sl_command.Channel;

            MakerCommandData maker_command_data = sl_command.MakerCommand;

            // Get the account information of the command's user.
            var account = UserInfoClasses.GetAccount(user);

            // Send a loading message to the channel while the sprite sheet is being made.
            RestUserMessage loader = await channel.SendMessageAsync("", false, base_p3p_rendering.P3P_Loading_Message(account).Build());

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
                // Create and assign bitmap variables for the assets needed.
                Bitmap message_window = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3P//Main//message_window.png");
                Bitmap cursor = (Bitmap)System.Drawing.Image.FromFile($@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3P//Main//cursor.png");

                // Draw the layer with the user's colored default background if it exists.
                graphics.DrawImage(colored_background_bitmap, 0, 0, template_width, template_height);

                // Draw the user's background to the base template.
                graphics.DrawImage(background, 0, 0, template_width, template_height);

                // Draw the character bust-up to the template if the base sprite number is not '0'.
                //Bustup 1
                if (maker_command_data.Character_Data_1.Base_Sprite != 0)
                {
                    Bitmap placed_bustup = Set_Bustup_Placement(account, bustup_1, bustup_data_1, set_data_1, 1);
                    graphics.DrawImage(placed_bustup, 0, 0, template_width, template_height);
                }
                //Bustup 2
                if (maker_command_data.Character_Data_2.Base_Sprite != 0)
                {
                    Bitmap placed_bustup = Set_Bustup_Placement(account, bustup_2, bustup_data_2, set_data_2, 2);
                    graphics.DrawImage(placed_bustup, 0, 0, template_width, template_height);
                }

                // Draw the message window layer to the base template.
                message_window = base_p3p_rendering.Tint_Message_Window(message_window);
                graphics.DrawImage(message_window, 0, 0, template_width, template_height);

                // Draw the cursor layer to the base template.
                cursor = base_p3p_rendering.Color_Cursor(cursor, account.P3P_TS_Color);
                graphics.DrawImage(cursor, 0, 0, template_width, template_height);

                // If the user has the HUD enabled, render it to the template as well.
                if (account.P3P_TS_HUD != "None")
                {
                    graphics.DrawImage(base_p3p_rendering.Render_Calendar_HUD(account), 0, 0, template_width, template_height);
                    graphics.DrawImage(base_p3p_rendering.Render_Moon_HUD(account), 0, 0, template_width, template_height);
                }
            }

            // Create another graphics object for the base template.
            // We'll start rendering our needed text here.
            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                System.Drawing.Color name_dark_blue = System.Drawing.Color.FromArgb(29, 0, 92);
                Rectangle name_area = new Rectangle(0, 190, 480, 30);

                string display_name = sl_command.MakerCommand.Display_Name;
                display_name = OfficialSetMethods.Validate_Input(sl_command, "P3P", "Name", display_name);

                Bitmap rendered_name = base_p3p_rendering.Render_Name(display_name);
                Bitmap colored_rendered_name = base_p3p_rendering.Bitmap_To_Color(rendered_name, name_dark_blue, name_area);
                graphics.DrawImage(colored_rendered_name, 0, 0, template_width, template_height);

                System.Drawing.Color dialogue_gray = System.Drawing.Color.FromArgb(72, 72, 72);
                Rectangle dialogue_area = new Rectangle(0, 190, 480, 82);

                maker_command_data.Dialogue = OfficialSetMethods.Validate_Input(sl_command, "P3P", "Dialogue", maker_command_data.Dialogue);
                List<string>[] parsed_lines = OfficialSetMethods.Line_Parser(sl_command, "P3P", maker_command_data.Dialogue, 3, max_line_length);

                Bitmap rendered_dialogue = base_p3p_rendering.Render_Dialogue(parsed_lines);
                Bitmap colored_dialogue = base_p3p_rendering.Bitmap_To_Color(rendered_dialogue, dialogue_gray, dialogue_area);

                // Draw the input dialogue to the template.
                graphics.DrawImage(colored_dialogue, 0, 0, template_width, template_height);
            }

            base_template = base_p3p_rendering.Scale_Template(account, base_template);

            if (error_counter > 0)
            {
                _ = ErrorHandling.API_Timeout(sl_command);
            }

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
        }

        public Bitmap Set_Bustup_Placement(UserInfoFields account, Bitmap bustup, BustupData bustup_data, OfficialSetData set_data, int char_number)
        {
            // Create a starting base bitmap to render all graphics on.
            Bitmap base_template = new Bitmap(template_width, template_height);

            // Switch the rendering position of the bustup depending on the user's settings.
            using (Graphics graphics = Graphics.FromImage(base_template))
            {
                switch (char_number)
                {
                    case 1:
                        graphics.DrawImage(bustup, bustup_data.P3P_Left_Coord_X, bustup_data.P3P_Left_Coord_Y, bustup_data.P3P_Scale_Width, bustup_data.P3P_Scale_Height);
                        break;

                    case 2:
                        graphics.DrawImage(bustup, bustup_data.P3P_Right_Coord_X, bustup_data.P3P_Right_Coord_Y, bustup_data.P3P_Scale_Width, bustup_data.P3P_Scale_Height);
                        break;
                }
            }

            return base_template;
        }
    }
}
