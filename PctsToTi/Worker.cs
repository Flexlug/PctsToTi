using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using PctsToTi.Serializers;
using ShellProgressBar;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace PctsToTi;

public class Worker
{
    public void Start(string s, IImageSerializer imageSerializer1) 
    {
        var imgFilesNames = Directory.GetFiles("pictures")
            .OrderBy(x => x).ToList();
    
        if (imgFilesNames is null || imgFilesNames.Count == 0)
        {
            Console.WriteLine("No images in \"pictures\" folder");
            return;
        }
    
        int imageNum = 1;
        const int LCD_WIDTH = 160;
        const int LCD_HEIGHT = 100;
    
        var programStopwatch = new Stopwatch();
        programStopwatch.Start();
    
        var overallProgress = new ProgressBar(2, "Overall progress");
    
        var prepareProgress = overallProgress.Spawn(imgFilesNames.Count, "Preparing images");
        prepareProgress.Tick();
        Parallel.For(0, imgFilesNames.Count, (i) =>
        {
            var image = Image.Load<Rgba32>(imgFilesNames[i]);
    
            ImageManipulations.ResizeImage(image, LCD_WIDTH, LCD_HEIGHT);
            ImageManipulations.ToBlackAndWhite(image);
    
            image.Save($"scaled/{i:d4}.jpg");
    
            prepareProgress.Tick();
        });
    
        overallProgress.Tick();
    
        var writer = new TiFileWriter(s, imageSerializer1);
    
        var writeProgress = overallProgress.Spawn(imgFilesNames.Count, $"Writing images to {s}");
        writeProgress.Tick();
    
        var scaledImgFilesNames = Directory.GetFiles("scaled")
            .OrderBy(x => x).ToList();
    
        for (int i = 0; i < scaledImgFilesNames.Count; i++)
        {
            var image = Image.Load<Rgba32>(scaledImgFilesNames[i]);
            writer.WriteImage(image);
            writeProgress.Tick();
        }

        writer.Dispose();

        overallProgress.Tick();
    
        programStopwatch.Stop();
        overallProgress.WriteLine($"Done! {programStopwatch.Elapsed}");
        Console.ReadKey();
    }
}