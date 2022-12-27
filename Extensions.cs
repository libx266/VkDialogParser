using Newtonsoft.Json;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace VkDialogParser
{
    internal static class Extensions
    {
        internal static bool Log(this Exception ex)
        {
            Console.WriteLine(JsonConvert.SerializeObject(ex, Formatting.Indented));
            return false;
        }

        internal static byte[] SaveImage(this Stream input)
        {
            using (var image = Image.Load<Rgb24>(input))
            {
                if (image.Height > 720)
                {
                    double coef = (double)720 / (double)image.Height;

                    image.Mutate(ctx => ctx.Resize((int)((double)image.Width * coef), 720));  
                }

                using (var ms = new MemoryStream())
                {
                    image.SaveAsJpeg(ms, new JpegEncoder { Quality = 60 });
                    return ms.ToArray();
                }
            }
        }
    }
}
