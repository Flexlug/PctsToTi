using System;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using PctsToTi;
using PctsToTi.Serializers;
using ShellProgressBar;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;


char input;
do
{
    Console.Write("Specify image serialization mechanism (1: default, 2: with compressing): -> ");
    input = Console.ReadKey().KeyChar;
    Console.WriteLine();
} while (input < 49 || input > 50);

IImageSerializer imageSerializer = input switch
{
    '1' => new ImageSerializer(),
    '2' => new CompressedImageSerializer(),
    _ => throw new ArgumentException()
};

if (!Directory.Exists("scaled"))
    Directory.CreateDirectory("scaled");

Console.Write("Enter filename: -> ");
string fileName = Console.ReadLine();

if (string.IsNullOrEmpty(fileName))
{
    Console.WriteLine("Empty filename specified. Exiting...");
    return;
}

var worker = new Worker();
worker.Start(fileName, imageSerializer);
