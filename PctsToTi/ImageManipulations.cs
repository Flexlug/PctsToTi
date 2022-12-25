using System;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace PctsToTi;

public static class ImageManipulations
{
    public static void ResizeImage(Image image, int width, int height)
    {
        image.Mutate(x => x.Resize(width, height));
    }

    public static void ToBlackAndWhite(Image<Rgba32> image)
    {
        image.ProcessPixelRows(accessor =>
        {
            Rgba32 black = Color.Black;
            Rgba32 white = Color.White;
        
            for (int y = 0; y < accessor.Height; y++)
            {
                Span<Rgba32> pixelRow = accessor.GetRowSpan(y);

                for (int x = 0; x < pixelRow.Length; x++)
                {
                    ref Rgba32 pixel = ref pixelRow[x];
                    if (pixel.R == 255)
                    {
                        // Overwrite the pixel referenced by 'ref Rgba32 pixel':
                        pixel = white;
                    }
                    else
                    {
                        pixel = black;
                    }
                }
            }
        });
    }
}