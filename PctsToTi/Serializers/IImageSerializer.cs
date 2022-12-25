using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace PctsToTi.Serializers;

public interface IImageSerializer
{
    public byte[] Serialize(Image<Rgba32> image);
    public byte GetSerializerId();
}