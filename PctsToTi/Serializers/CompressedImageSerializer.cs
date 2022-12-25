using System;
using System.Collections.Generic;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace PctsToTi.Serializers;

/// <summary>
/// Black and white image serializer with compression. 
/// </summary>
public class CompressedImageSerializer : IImageSerializer
{   
    public byte[] Serialize(Image<Rgba32> image)
    {
        Rgba32[] pixelArray = new Rgba32[image.Width * image.Height];
        image.CopyPixelDataTo(pixelArray);

        var byteList = new List<byte>();
        
        byte rowCount = 0;
        bool pixelColor = false;
        bool previewPixelColor = false;

        var pixel = pixelArray[0];
    
        // Make pixel colors equal on loop start
        pixelColor = isWhite(pixel);
        previewPixelColor = isWhite(pixel);

        for (int i = 0; i < pixelArray.Length; i++)
        {
            pixel = pixelArray[i];

            pixelColor = isWhite(pixel);

            if (pixelColor == previewPixelColor && rowCount != 127)
            {
                rowCount++;
            }
            else
            {
                byte frame = pixelColor ? (byte)0b10000000 : (byte)0b00000000;
                frame = (byte)(frame | rowCount);

                byteList.Add(frame);
                rowCount = 1;
            }

            previewPixelColor = pixelColor;

        }

        int resultLength = byteList.Count + 4;
        byte[] intBytes = BitConverter.GetBytes(resultLength);
        Array.Reverse(intBytes);
        
        byte[] result = new byte[resultLength];
        result[0] = intBytes[0];
        result[1] = intBytes[1];
        result[2] = intBytes[2];
        result[3] = intBytes[3];

        for (int i = 4; i < resultLength; i++)
            result[i] = byteList[i - 4];
        
        return result;
    }

    public byte GetSerializerId() => 2;

    private bool isWhite(Rgba32 color) => color.R == 255;
}