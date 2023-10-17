using System;
using MediaToolkit;
using MediaToolkit.Model;
using MediaToolkit.Options;
using ReferencePoints.Helpers;

namespace ReferencePoints.Providers;

public class ImageRetriever
{
    private const int millisecondsInMinute = 1000;

    public void RetrieveImages(string mp4FilePath, string outputPath, int framesPerSec = 1, TimeSpan? from = null,
        TimeSpan? to = null)
    {
        using (var engine = new Engine())
        {
            var mp4 = new MediaFile { Filename = mp4FilePath };

            var start = from.HasValue ? Convert.ToInt32(from.Value.TotalMilliseconds) : 0;
            var end = to?.TotalMilliseconds ?? mp4.Metadata.Duration.TotalMilliseconds;

            engine.GetMetadata(mp4);

            for (int i = start; i < end; i += millisecondsInMinute / framesPerSec)
            {
                var options = new ConversionOptions { Seek = TimeSpan.FromMilliseconds(i) };
                var outputFile = new MediaFile
                {
                    Filename = string.Format($"{Constants.ImageName}{Constants.JpgExt}", outputPath,
                        $"{i / millisecondsInMinute}_{i % millisecondsInMinute}")
                };
                engine.GetThumbnail(mp4, outputFile, options);
            }
        }
    }
}