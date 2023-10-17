using System.Collections.Generic;
using System.Threading.Tasks;
using ReferencePoints.Models;

namespace ReferencePoints.Interfaces;

public interface IVideoAnalyzer
{
    Task<IEnumerable<Point>> Analyze(string convertedImagesPath, bool verticalOnly = false, bool horizontalOnly = false);

    void BreakIntoImages();

    Task<string> ConvertToGrayScale();

    double AnalyzeImage(string bmpPath, bool verticalOnly = false, bool horizontalOnly = false);
}