using System;
using System.IO;
using PctsToTi.Serializers;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace PctsToTi;

/// <summary>
/// Intended for writing data to file, wich is suitable for Ti-89 transfer via Ti Connect
/// </summary>
public class TiFileWriter : IDisposable
{
    private MemoryStream _dataStream = new();

    private string _varName = string.Empty;

    private BinaryWriter _outStream;
    private IImageSerializer _serializer;

    private bool _isFlushed = false;

    /// <summary>
    /// Creates an instance of Ti-89 writer
    /// </summary>
    /// <param name="fileName">File name. Also known as "variable name". Can be up to 8 characters long.</param>
    /// <param name="serializer">Binary serializer for images</param>
    public TiFileWriter(string fileName, IImageSerializer serializer)
    {
        if (fileName.Length > 8)
            throw new ArgumentException("File name can not be longer then 8 characters");

        _varName = fileName;

        // .89l - Ti-89 data file extension
        var file = File.Create(_varName + ".89l");
        _outStream = new(file);
        _serializer = serializer;
    }

    public void WriteImage(Image<Rgba32> image)
    {
        var result = _serializer.Serialize(image);
        _dataStream.Write(result);
    }

    /// <summary>
    /// Write all data in file
    /// </summary>
    public void Flush()
    {
        // Data format explanation: http://merthsoft.com/linkguide/ti89/fformat.html#varheader

        // **********
        // HEADER
        // **********

        // Offset: 0
        // Length: 8 bytes
        // 8-character signature. The signature is always "**TI89**".
        _outStream.Write("**TI89**".ToCharArray());

        // Offset: 8
        // Length: 2 bytes
        // 2-byte further signature. These two bytes always contain {01h, 00h} = {1, 0}.
        _outStream.Write((byte)(1));
        _outStream.Write((byte)(0));

        // Offset: 10
        // Length: 8 bytes
        // Default folder name (zero terminated unless 8 characters long).
        _outStream.Write("main".ToCharArray());   // string is 4 bytes length
        for (int i = 1; i <= 4; i++)
            _outStream.Write((byte)0);

        // Offset: 18
        // Length: 40 bytes
        // Comment. The comment is either zero-terminated or padded on the right with space characters.
        for (int i = 1; i <= 40; i++)
            _outStream.Write((byte)0);

        // Offset: 58
        // Length: 2 bytes
        // Number of variable and folder entries in the variable table
        _outStream.Write((byte)1);
        _outStream.Write((byte)0);

        // **********
        // VARIABLE
        // **********

        // Offset: 60 + 0
        // Length: 4 bytes
        // Offset to the data for this variable from the beginning of the file.
        _outStream.Write((byte)82);
        _outStream.Write((byte)0);
        _outStream.Write((byte)0);
        _outStream.Write((byte)0);

        // Offset: 60 + 4
        // Length: 8 bytes
        // Name of variable (zero terminated if not 8 characters long).
        _outStream.Write("badapple".ToCharArray());

        // Offset: 60 + 12
        // Length: 1 byte
        // Type ID of the variable. 
        // List of type IDs: http://merthsoft.com/linkguide/ti89/packet.html#varheader

        // - 04h(4)  - list
        // - 0Ch(12) - string
        // - 1Ch(28) - other file
        // - 21h(33) - assembly program

        _outStream.Write((byte)4); 

        // Offset: 60 + 13
        // Length: 1 byte
        // Attribute (0: none, 1: locked, 2: archived)
        _outStream.Write((byte)0);

        // Offset: 60 + 14
        // Length: 2 bytes
        // Unused - each byte has a value of 0.
        _outStream.Write((byte)0);
        _outStream.Write((byte)0);

        // Offset: 60 + 16
        // Length: 4 bytes
        // The file size, in bytes.
        // (+82 is because of the file header)
        // (+2 is because we're adding elements length. List specific stuff)
        // (+4 is because of gap before the data section)
        // (+2 is because of the checksum in the end)
        _outStream.Write((uint)_dataStream.Length + 82 + 2 + 4 + 2);

        // Offset: 60 + 20
        // Length: 2 bytes
        // A 2-byte signature. These two bytes always contain {A5h, 5Ah}.
        _outStream.Write((byte)165);
        _outStream.Write((byte)90);

        // Offset: 60 + 22
        // Length: n bytes
        // The variable data. This field includes all bytes transferred in data packets. The first four bytes usually all have a value of 0.

        // 4 bytes gap in the start of data section
        for (int i = 1; i <= 4; i++)
        {
            _outStream.Write((byte)0);
        }
        
        var data = _dataStream.ToArray();
        var listElementsCount = (ushort)(data.Length / 10);
        
        // LIST SPECIFIC STUFF
        // Offset: 60 + 22 
        // Length: 2 bytes
        // Number of elements in the list. Each element is 10 bytes length
        _outStream.Write(listElementsCount);
        
        // Offset: 60 + 24 (?????)
        // Length: n
        // Element values, one by one, first to last. If the list is a real list, then each element is a 10-byte real number.
        // If the list is a complex list, then each element is a 20-byte complex number.
        _outStream.Write(data);

        // Fill end gap to make collection fit to 10-byte elements list
        var endGapLength = 10 - (data.Length - listElementsCount * 10);
        if (endGapLength != 10)
        {
            for (int i = 1; i <= endGapLength; i++)
            {
                _outStream.Write((byte)0);
            }
        }
        
        // Offset: 60 + 24 + n
        // Length: 2 bytes
        // Checksum. This is the lower 16 bits of the sum of all bytes in the variable data.
        var checksum = calculateChecksum(data);
        _outStream.Write(checksum);

        _isFlushed = true;
    }

    // Checksum realization from: http://merthsoft.com/linkguide/ti89/packet.html#varheader
    private ushort calculateChecksum(byte[] data)
    {
        ushort checksum = 0;
        for (int i = 0; i < data.Length; i++)
        {
            checksum += data[i];
        }
        return checksum;
    }

    public void Dispose()
    {
        if (!_isFlushed)
            Flush();

        _outStream.Flush();
        _outStream.Close();
        _outStream.Dispose();
    }
}