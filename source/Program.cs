using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhaserSpriteSheetUnpacker
{
    class Program
    {
        static void Main(string[] args)
        {
            //local variables
            dynamic sprites;
            string jsonText;
            Bitmap bitmap;
            //
            Console.WriteLine("Phaser Sprite Sheet Unpacker v1.0 - (C)2016 Marcelo Lv Cabral");
            Console.WriteLine("Extracts each frame from the sprite sheet as a separare png");
            Console.WriteLine("");
            if (args.Length >= 3)
            {
                try
                {
                    jsonText = System.IO.File.ReadAllText(args[0]);
                    sprites = JsonConvert.DeserializeObject(jsonText);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error reading/deserializing the json file: {0}", ex.Message);
                    return;
                }
                try
                {
                    bitmap = Image.FromFile(args[1]) as Bitmap;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error reading the png file: {0}", ex.Message);
                    return;
                }
                if (!Directory.Exists(args[2]))
                {
                    Console.WriteLine("Output folder does not exist!");
                    return;
                }

                try
                {
                    foreach (var item in sprites.frames)
                    {
                        Console.WriteLine(item.Name);
                        var frame = item.Value.frame;
                        Rectangle cropRect = new Rectangle((int)frame.x.Value, (int)frame.y.Value, (int)frame.w.Value, (int)frame.h.Value);
                        Bitmap sprite = CropImage(bitmap, cropRect);
                        if (item.Name.Substring(item.Name.Length-4) == ".png")
                        {
                            sprite.Save(Path.Combine(args[2],item.Name));
                        }
                        else
                        {
                            sprite.Save(Path.Combine(args[2], item.Name + ".png"));
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error unpacking the frames: {0}", ex.Message);
                    return;
                }
            }
            else
            {
                help();
            }
        }

        private static Bitmap CropImage(Bitmap src, Rectangle cropRect)
        {
            Bitmap target = new Bitmap(cropRect.Width, cropRect.Height);

            using(Graphics g = Graphics.FromImage(target))
            {
               g.DrawImage(src, new Rectangle(0, 0, target.Width, target.Height), 
                                cropRect,                        
                                GraphicsUnit.Pixel);
            }
            return target;
        }
        private static void help()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("pssu <json path> <png path> <output folder>");
            Console.WriteLine("");
        }
    }
}
