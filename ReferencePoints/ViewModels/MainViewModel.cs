using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediaToolkit;
using MediaToolkit.Model;
using Microsoft.Win32;
using ReferencePoints.Helpers;
using ReferencePoints.Models;
using ReferencePoints.Providers;

namespace ReferencePoints.ViewModels;

public class MainViewModel : ObservableRecipient
{
    private const double defaultComparer = 0.3;
    private const string regexPattern = "[\\d]+[.][\\d]+";
    private readonly LinearContraster _linearContraster = null!;
    private readonly ImageProvider _imageProvider;
    private readonly GradientMatrixBuilder _gradientMatrixBuilder;
    private readonly ImageRetriever _imageRetriever;
    private string mp4Path;
    private string imagePath;
    private int[][] cutMatrix;
    private double comparer = defaultComparer;
    private bool isConvertedToGrayscale = false;
    private List<Coordinate> referencePoints = new List<Coordinate>();
    private bool isBusy;
    private RelayCommand uploadVideoCommand;
    private RelayCommand breakIntoImagesCommand;
    private RelayCommand uploadImageCommand;
    private AsyncRelayCommand convertToGrayscaleCommand;
    private AsyncRelayCommand analyzeImageCommand;
    private AsyncRelayCommand exportResultCommand;

    public MainViewModel()
    {
        _imageProvider = new ImageProvider();
        _gradientMatrixBuilder = new GradientMatrixBuilder();
        _linearContraster = new LinearContraster();
        _imageRetriever = new ImageRetriever();
    }
    
    public MediaFile Mp4 { get; set; }

    public string Mp4Path
    {
        get => mp4Path;
        set => SetProperty(ref mp4Path, value);
    }
    
    public string ImagePath
    {
        get => imagePath;
        set => SetProperty(ref imagePath, value);
    }
    
    public bool IsBusy 
    {
        get => isBusy;
        set => SetProperty(ref isBusy, value);
    }

    public string Comparer
    {
        get => comparer.ToString(CultureInfo.InvariantCulture);
        set => SetProperty(ref comparer, Regex.IsMatch(value, regexPattern) ? Convert.ToDouble(value) : defaultComparer);
    }

    public RelayCommand UploadVideoCommand =>
        uploadVideoCommand ?? new RelayCommand(UploadVideoCommandExecuted);

    public RelayCommand UploadImageCommand =>
        uploadImageCommand ?? new RelayCommand(UploadImageCommandExecuted);

    public AsyncRelayCommand ConvertToGrayscaleCommand =>
        convertToGrayscaleCommand ?? new AsyncRelayCommand(ConvertToGrayscaleCommandExecuted);

    public AsyncRelayCommand AnalyzeImageCommand =>
        analyzeImageCommand ?? new AsyncRelayCommand(() => AnalyzeImage(ImagePath));

    public AsyncRelayCommand ExportResultCommand =>
        exportResultCommand ?? new AsyncRelayCommand(ExportResultCommandExecuted);
    
    private void UploadImageCommandExecuted()
    {
        Reset();
        
        OpenFileDialog fileDialog = new OpenFileDialog();
        fileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);
        fileDialog.Multiselect = false;
        fileDialog.Filter = Constants.ImageFilter;
        fileDialog.ShowDialog();

        if (fileDialog.FileNames.Any())
        {
            ImagePath = fileDialog.FileNames[0];
        }
        
        //ConvertToGrayscaleCommand.NotifyCanExecuteChanged();
    }

    private void Reset()
    {
        Mp4 = null;
        Mp4Path = null;
        ImagePath = null;
        isConvertedToGrayscale = false;
        referencePoints.Clear();
        cutMatrix = null;
    }
    
    private async Task ConvertToGrayscaleCommandExecuted()
    {
        if (string.IsNullOrWhiteSpace(ImagePath))
        {
            MessageBox.Show("Upload Image.", "Upload Image", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            return;
        }
        
        var directoryDialog = ShowSaveFileDialog(Constants.BmpFilter, Constants.BmpExtPattern);

        if (string.IsNullOrEmpty(directoryDialog.FileName)) return;

        try
        {
            IsBusy = true;

            string bitmapPath = directoryDialog.FileName;
            await _imageProvider.CreateGrayscale8Async(ImagePath, bitmapPath);
            ImagePath = bitmapPath;
            isConvertedToGrayscale = true;
        }
        finally
        {
            //AnalyzeImageCommand.NotifyCanExecuteChanged();
            //ExportResultCommand.NotifyCanExecuteChanged();
            IsBusy = false;
        }
    }
    
    public SaveFileDialog ShowSaveFileDialog(string filter = null, string defaultExt = null, string title = null)
    {
        SaveFileDialog dialog = new SaveFileDialog();
        if(!string.IsNullOrWhiteSpace(title))
        {
            dialog.Title = title;
        }
        dialog.Filter = filter;
        dialog.DefaultExt = defaultExt;
        dialog.ShowDialog();

        return dialog;
    }

    private async Task AnalyzeImage(string selectedImagePath)
    {
        if (string.IsNullOrWhiteSpace(ImagePath) || !isConvertedToGrayscale)
        {
            MessageBox.Show("Convert to Grayscale.", "Convert to Grayscale", MessageBoxButton.OK, MessageBoxImage.Exclamation);

            return;
        }
        
        IsBusy = true;
        
        try
        {
            await Task.Run(() =>
            {
                var pixelsMatrix = _imageProvider.GetBitmapPixelsMatrix(selectedImagePath);
                var gradientMatrix = _gradientMatrixBuilder.BuildGradientMatrix(pixelsMatrix);
                var linearContrastMatrix = _linearContraster.BuildLinearContrastMatrix(gradientMatrix);
                cutMatrix = MatrixHelper.CutMatrix(linearContrastMatrix, 9);

                for (int i = 0; i < cutMatrix.Length - 9; ++i)
                {
                    for (int j = 0; j < cutMatrix[i].Length - 9; ++j)
                    {
                        var centralSubMatrix = MatrixHelper.Retrieve3x3SubMatrix(i + 3, j + 3, cutMatrix);
                        var w2s = new List<double>();

                        var average = _linearContraster.CalculateAverage(centralSubMatrix);
                        var dispersion = _linearContraster.CalculateDispersion(centralSubMatrix, average);
                        var variation = _linearContraster.CalculateVariationCoefficient(average, dispersion);

                        var formParameter = _linearContraster.MeasureFormParameter(variation);
                        var scaleParameter = _linearContraster.MeasureScaleParameter(average, dispersion);
                        for (int k = 0; k < 9; k+=3)
                        {
                            for (int l = 0; l < 9; l+=3)
                            {
                                if (k == 3 && l == 3) continue;
                                
                                var subMatrix3x3 = MatrixHelper.Retrieve3x3SubMatrix(i + k, j + l, cutMatrix);
                                var average_1 = _linearContraster.CalculateAverage(subMatrix3x3);
                                var dispersion_1 = _linearContraster.CalculateDispersion(subMatrix3x3, average_1);
                                var variation_1 = _linearContraster.CalculateVariationCoefficient(average_1, dispersion_1);

                                var formParameter_1 = _linearContraster.MeasureFormParameter(variation_1);
                                var scaleParameter_1 = _linearContraster.MeasureScaleParameter(average_1, dispersion_1);

                                var w2 = _linearContraster.CalculateClosure(formParameter, formParameter_1, scaleParameter,
                                    scaleParameter_1);
                                w2s.Add(w2);
                                //Console.WriteLine($"i: i+{k}, j: j+{l}");
                            }
                        }

                        if (w2s.All(x => x < comparer))
                        {
                            referencePoints.Add(new Coordinate(i, j));
                        }
                    }
                }
            });
        }
        finally
        {
            //ExportResultCommand.NotifyCanExecuteChanged();
            IsBusy = false;
        }
        
        var result = MessageBox.Show("Do you want to save results?.", "Save Results", MessageBoxButton.YesNo, MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes)
        {
            await ExportResultCommandExecuted();
        }
    }
    
    private async Task ExportResultCommandExecuted()
    {
        if (string.IsNullOrWhiteSpace(ImagePath) || cutMatrix == null || !referencePoints.Any())
        {
            MessageBox.Show("Analyze Image.", "Analyze Image", MessageBoxButton.OK, MessageBoxImage.Exclamation);

            return;
        }
        
        var saveDialog = ShowSaveFileDialog(Constants.BmpFilter);

        if (string.IsNullOrEmpty(saveDialog.FileName)) return;

        IsBusy = true;
        try
        {
            await Task.Run(() =>
            {
                foreach (var point in referencePoints)
                {
                    cutMatrix[point.X][point.Y] = -1;
                }

                _imageProvider.CreateBitmapFromPixelMatrixAndReferencePoints(cutMatrix).Save(saveDialog.FileName);
            });
        }
        finally
        {
            IsBusy = false;
        }

        ImagePath = saveDialog.FileName;
    }
    
    private void UploadVideoCommandExecuted()
    {
        OpenFileDialog fileDialog = new OpenFileDialog();
        fileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);
        fileDialog.Multiselect = false;
        fileDialog.Filter = Constants.Mp4Filter;
        fileDialog.ShowDialog();

        if (fileDialog.FileNames.Any())
        {
            Mp4Path = fileDialog.FileName;
            using (var engine = new Engine())
            {
                Mp4 = new MediaFile { Filename = Mp4Path };
                engine.GetMetadata(Mp4);
            }
        }
    }
}