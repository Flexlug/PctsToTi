using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace PctsToTi.Serializers;

/// <summary>
/// Black and white image serializer without compression
/// </summary>
public class ImageSerializer : IImageSerializer
{
    /// <summary>
    /// Serialize black and white image to byte array. Each byte will represent an 8 pixels row fragment
    /// </summary>
    /// <param name="image">Serializing image</param>
    public byte[] Serialize(Image<Rgba32> image)
    {
        Rgba32[] pixelArray = new Rgba32[image.Width * image.Height];
        image.CopyPixelDataTo(pixelArray);
        
        int lcdWidth = image.Width;
        int lcdHeight = image.Height;
        
        byte[] result = new byte[lcdWidth / 8 * lcdHeight];
        int result_i = 0;
        
        bool[] boolFrame = new bool[8];
        int frame_i = 0;

        for (int i = 0; i < pixelArray.Length; i++)
        {
            var pixel = pixelArray[i];

            // Each pixel will be a bit in byte stream where each byte is called "frame"

            // LCD screen is black and white only
            // Define if pixel is black or white. Picking any channel
            if (pixel.R == 255)
            {
                // Pixel is white. Set bit to true
                boolFrame[frame_i++] = true;
            }
            else
            {
                // Pixel is black. Set bit to false
                boolFrame[frame_i++] = false;
            }

            // When end of frame is reached we will write it to file
            if (frame_i == 8)
            {
                // Equals to 0x1000000
                // Works as writing "caret" which will be moved by bit arithmetics
                byte writingBit = 128;

                // Equals to 0x0000000
                byte frame = 0;
                for (int bit = 0; bit < 8; bit++)
                {
                    if (boolFrame[bit])
                    {
                        frame = (byte)(frame | writingBit);
                        boolFrame[bit] = false;
                    }

                    // Moving "caret" to right by 1 bit
                    writingBit = (byte)(writingBit >> 1);
                }

                // Write result frame to file
                result[result_i] = frame;

                frame_i = 0;
            }

        }

        return result;
    }

    public byte GetSerializerId() => 1;
}