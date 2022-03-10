using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Rest;
using Discord.Commands;
using Discord.WebSocket;
using Fergun.Interactive;
using SocialLinker.Config;
using SocialLinker.Core.CloudStorageTables;
using SocialLinker.Cooldown;
using SocialLinker.Core.Menus.InitialUsage.Main;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;

namespace SocialLinker.Commands
{
    public class Multiply : ModuleBase<SocketCommandContext>
    {
        public static System.Drawing.Image SetImageOpacity(System.Drawing.Image image, float opacity)
        {
            try
            {
                //create a Bitmap the size of the image provided  
                Bitmap bmp = new Bitmap(image.Width, image.Height);

                //create a graphics object from the image  
                using (Graphics gfx = Graphics.FromImage(bmp))
                {

                    //create a color matrix object  
                    ColorMatrix matrix = new ColorMatrix();

                    //set the opacity  
                    matrix.Matrix33 = opacity;

                    //create image attributes  
                    ImageAttributes attributes = new ImageAttributes();

                    //set the color(opacity) of the image  
                    attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                    //now draw the image  
                    gfx.DrawImage(image, new Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attributes);
                }
                return bmp;
            }
            catch
            {
                return image;
            }
        }

        private System.Drawing.Image MultiplyBitmap(Bitmap scrBitmap)
        {
            System.Drawing.Color actualColor;
            //Make an empty bitmap the same size as scrBitmap
            Bitmap newBitmap = new Bitmap(scrBitmap.Width, scrBitmap.Height);
            for (int i = 0; i < scrBitmap.Width; i++)
            {
                for (int j = 0; j < scrBitmap.Height; j++)
                {
                    //Get the pixel from the scrBitmap image
                    actualColor = scrBitmap.GetPixel(i, j);

                    var int_newColorR = ((actualColor.R * actualColor.R) / 255);
                    var int_newColorG = ((actualColor.G * actualColor.G) / 255);
                    var int_newColorB = ((actualColor.B * actualColor.B) / 255);

                    if (actualColor.A <= 5)
                    {
                        //Don't draw the pixel; it needs to be transparent
                    }
                    /* else if (int_newColorR + int_newColorG + int_newColorB >= 765)
                     {
                         newBitmap.SetPixel(i, j, System.Drawing.Color.White);
                     }
                     else if (int_newColorR + int_newColorG + int_newColorB <= 0)
                     {
                         newBitmap.SetPixel(i, j, System.Drawing.Color.Black);
                     } */
                    else
                    {
                        var newColorR = (byte)((actualColor.R * actualColor.R) / 255);
                        var newColorG = (byte)((actualColor.G * actualColor.G) / 255);
                        var newColorB = (byte)((actualColor.B * actualColor.B) / 255);

                        System.Drawing.Color newColor = System.Drawing.Color.FromArgb(actualColor.A, newColorR, newColorG, newColorB);
                        newBitmap.SetPixel(i, j, newColor);
                    }
                }
            }

            using (Graphics graphics = Graphics.FromImage(newBitmap))
            {
                Bitmap thirdLayer = new Bitmap(newBitmap.Width, newBitmap.Height);

                for (int i = 0; i < newBitmap.Width; i++)
                {
                    for (int j = 0; j < newBitmap.Height; j++)
                    {
                        //Get the pixel from the scrBitmap image
                        actualColor = newBitmap.GetPixel(i, j);

                        var int_newColorR = ((actualColor.R * actualColor.R) / 255);
                        var int_newColorG = ((actualColor.G * actualColor.G) / 255);
                        var int_newColorB = ((actualColor.B * actualColor.B) / 255);

                        if (actualColor.A <= 5)
                        {
                            //Don't draw the pixel; it needs to be transparent
                        }
                        /* else if (int_newColorR + int_newColorG + int_newColorB >= 765)
                         {
                             thirdLayer.SetPixel(i, j, System.Drawing.Color.White);
                         }
                         else if (int_newColorR + int_newColorG + int_newColorB <= 0)
                         {
                             thirdLayer.SetPixel(i, j, System.Drawing.Color.Black);
                         } */
                        else
                        {
                            var newColorR = (byte)((actualColor.R * actualColor.R) / 255);
                            var newColorG = (byte)((actualColor.G * actualColor.G) / 255);
                            var newColorB = (byte)((actualColor.B * actualColor.B) / 255);

                            System.Drawing.Color newColor = System.Drawing.Color.FromArgb(actualColor.A, newColorR, newColorG, newColorB);
                            thirdLayer.SetPixel(i, j, newColor);
                        }
                    }
                }

                Bitmap bmp = (Bitmap)SetImageOpacity(thirdLayer, (float)0.18);
                graphics.DrawImage(bmp, 0, 0, newBitmap.Width, newBitmap.Height);
            }

            return newBitmap;
        }

        [Command("fix")]
        public async Task FixP5()
        {
            RestUserMessage loader = await Context.Channel.SendMessageAsync("Normalizing sprite colors...");

            try
            {
                foreach (var file in Directory.EnumerateFiles($@"C:\Users\Microjack5\Desktop\Old", "*.png"))
                {
                    string filename = Path.GetFileName(file);
                    Bitmap bitmap = (Bitmap)System.Drawing.Image.FromFile(file);
                    Bitmap newFile = (Bitmap)MultiplyBitmap(bitmap);
                    newFile.Save($@"C:\Users\Microjack5\Desktop\Fix\{filename}", System.Drawing.Imaging.ImageFormat.Png);
                }
            }
            catch (Exception e)
            {
                await Context.Channel.SendMessageAsync("Error encountered. Please see console for details.");
                Console.WriteLine($"{e}");
                await loader.DeleteAsync();
                return;
            }

            await Context.Channel.SendMessageAsync("Sprite colors returned to normal. See source folder for results.");
            await loader.DeleteAsync();

            Console.WriteLine($">>Normalization command called by {Context.User.Username} in #{Context.Channel.Name}\n");
        }
    }
}
