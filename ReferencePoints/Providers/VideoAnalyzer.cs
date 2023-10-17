using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ReferencePoints.Helpers;
using ReferencePoints.Interfaces;
using Point = ReferencePoints.Models.Point;

namespace ReferencePoints.Providers;

    public class VideoAnalyzer : IVideoAnalyzer
    {
        private ImageRetriever imageRetriever;
        private ImageProvider imageProvider;
        private GradientMatrixBuilder gradientMatrixBuilder;
        private LinearContraster linearContraster;
        private string mp4Path;
        private string framesPath;
        private string outputPath;

        private SemaphoreSlim semaphore = new SemaphoreSlim(1, 5);

        public VideoAnalyzer(
            ImageRetriever imageRetriever,
            ImageProvider imageProvider,
            GradientMatrixBuilder gradientMatrixBuilder,
            LinearContraster linearContraster,
            string mp4Path,
            string framesPath,
            string outputPath)
        {
            this.imageRetriever = imageRetriever;
            this.imageProvider = imageProvider;
            this.gradientMatrixBuilder = gradientMatrixBuilder;
            this.linearContraster = linearContraster;
            this.mp4Path = mp4Path;
            this.framesPath = framesPath;
            this.outputPath = outputPath;
        }


        public async Task<IEnumerable<Models.Point>> Analyze(string convertedImagesPath, bool verticalOnly = false, bool horizontalOnly = false)
        {
            string[] bmps = Directory.GetFiles(convertedImagesPath);

            List<Point> measuresBlur = new List<Point>();
            List<Task> tasks = new List<Task>();

            int i = 0;

            foreach (var bmp in bmps)
            {
                tasks.Add(Task.Run(() => measuresBlur.Add(new Point(++i + 1, AnalyzeImage(bmp)))));
            }
            await Task.WhenAll(tasks);

            return measuresBlur;
        }

        public double AnalyzeImage(string bmpPath, bool verticalOnly = false, bool horizontalOnly = false)
        {
            semaphore.Wait();

            double blur = double.NaN;
            int[][] pixelsMatrix = imageProvider.GetBitmapPixelsMatrix(bmpPath);

            int[][] gradientMatrix = gradientMatrixBuilder.BuildGradientMatrix(pixelsMatrix, verticalOnly, horizontalOnly);

            int[][] linearContrastMatrix = linearContraster.BuildLinearContrastMatrix(gradientMatrix);

            blur = MeasureFormParameter(linearContrastMatrix);

            semaphore.Release();

            return blur;
        }

        public void BreakIntoImages()
        {
            this.imageRetriever.RetrieveImages(mp4Path, framesPath);
        }

        public async Task<string> ConvertToGrayScale()
        {
            string[] images = Directory.GetFiles(framesPath);

            string grayscaleOutputPath = CreateGrayscaleOutputDirectory(framesPath);

            List<Task> tasks = new List<Task>();

            int i = 0;

            foreach (var image in images)
            {
                string storage = $"{string.Format(Constants.ImageName, grayscaleOutputPath, ++i)}{Constants.BmpExt}";

                tasks.Add(SaveFrame(image, storage));
            }

            await Task.WhenAll(tasks);

            return grayscaleOutputPath;
        }

        private async Task SaveFrame(string imagePath, string storage)
        {
            await semaphore.WaitAsync();
            Bitmap originalBitmap = new Bitmap(imagePath);
            Bitmap bitmap = await Task.Run(() =>
            {
                 return imageProvider.ToGrayscale(originalBitmap, storage);
            });
            //bitmap.Save(storage);

            semaphore.Release();
        }

        private string CreateGrayscaleOutputDirectory(string outputPath)
        {
            return Directory.CreateDirectory($"{outputPath}_Grayscale_{DateTime.Now.ToString("dd_MM_yy_HH_mm_ss")}").FullName;
        }

        private double MeasureFormParameter(int[][] linearContrastMatrix)
        {
            var average = this.linearContraster.CalculateAverage(linearContrastMatrix);
            var dispersion = this.linearContraster.CalculateDispersion(linearContrastMatrix, average);
            var variation = this.linearContraster.CalculateVariationCoefficient(average, dispersion);

            return this.linearContraster.MeasureFormParameter(variation);
        }
    }
