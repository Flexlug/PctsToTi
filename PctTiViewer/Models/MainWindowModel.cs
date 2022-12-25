using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
using PctTiViewer.ViewModels;

namespace PctTiViewer.Models;

public class MainWindowModel
{
    public ReadOnlyObservableCollection<string> FileAttributes { get; }
    private ObservableCollection<string> _fileAttributes;

    public MainWindowModel()
    {
        _fileAttributes = new();
        FileAttributes = new(_fileAttributes);
    }

    public async Task LoadFile(string path)
    {
        _fileAttributes.Clear();
        
        FileStream file = null;
        try
        {
            file = File.Open(path, FileMode.Open);
        }
        catch (ArgumentNullException)
        {
            _fileAttributes.Add("Couldn't open file. No path provided");
        }
        catch (DirectoryNotFoundException)
        {
            _fileAttributes.Add("Couldn't find specified directory");
        }
        catch (UnauthorizedAccessException)
        {
            _fileAttributes.Add("No access to specified file");
        }
        catch (IOException)
        {
            _fileAttributes.Add("Couldn't open such file. Reading error");
        }

        var fileReader = new BinaryReader(file);

        _fileAttributes.Add($"File name: {file.Name}");
        
        _fileAttributes.Add($"File length: {fileReader.BaseStream.Length}");
        
        // Excluding header and checksum
        var dataLength = fileReader.BaseStream.Length - 82 - 2;
        _fileAttributes.Add($"Data length: {dataLength}");
        
        if (fileReader.BaseStream.Length < 84)
        {
            return;
        }
        
        var signature = new string(fileReader.ReadChars(8));
        _fileAttributes.Add($"00: Signature: {signature}");

        var first2byteSignature = fileReader.ReadByte();
        var second2byteSignature = fileReader.ReadByte();
        _fileAttributes.Add($"08: 2-byte signature {{01, 00}}: {{{first2byteSignature},{second2byteSignature}}}");

        var parentFolder = new string(fileReader.ReadChars(8));
        _fileAttributes.Add($"10: Parent folder: {parentFolder}");

        var fileComment = new string(fileReader.ReadChars(40));
        _fileAttributes.Add($"18: File comment: {fileComment}");

        var byte6Signature = string.Empty;
        byte6Signature += string.Join(',', fileReader.ReadBytes(6));
        _fileAttributes.Add($"58: 6-byte signature {{1, 0, 82, 0, 0, 0}}: {{{byte6Signature}}}");

        var variableName = new string(fileReader.ReadChars(8));
        _fileAttributes.Add($"64: Variable name: {variableName}");

        var typeId = fileReader.ReadByte();
        _fileAttributes.Add($"72: Type ID: {typeId}");
        
        var byte3ZeroSignature = string.Join(',', fileReader.ReadBytes(3));
        _fileAttributes.Add($"73: 3 zero bytes: {{{byte3ZeroSignature}}}");

        var fileSize = fileReader.ReadUInt32();
        _fileAttributes.Add($"76: File size: {fileSize}");
        
        var byte2Signature = string.Join(',', fileReader.ReadBytes(2));
        _fileAttributes.Add($"80: 2-byte signature {{165, 90}}: {{{byte2Signature}}}");
        
        fileReader.BaseStream.Position = fileReader.BaseStream.Length - 2;
        var checksumFirstByte = fileReader.ReadByte().ToString("X");
        var checksumSecondByte = fileReader.ReadByte().ToString("X");
        _fileAttributes.Add($"82+n: Checksum: {{{checksumFirstByte}, {checksumSecondByte}}}");
        
        file.Close();
    }
}