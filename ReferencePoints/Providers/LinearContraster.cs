using System;
using System.Linq;

namespace ReferencePoints.Providers;

public class LinearContraster
{
    public double CalculateAverage(int[][] matrix)
    {
        int sum = 0;

        for (int i = 0; i < matrix.Length; i++)
        {
            for (int j = 0; j < matrix[i].Length; j++)
            {
                if (matrix[i][j] < 0)
                {
                    Console.WriteLine("ERROR");
                    Console.WriteLine("row = " + i);
                    Console.WriteLine("column = " + j);
                    Console.WriteLine("x = " + matrix[i][j]);
                }

                sum += matrix[i][j];
            }
        }

        return sum / (matrix.Length * matrix[0].Length);
    }

    public double CalculateDispersion(int[][] matrix, double average)
    {
        double sum = 0;

        for (int i = 0; i < matrix.Length; i++)
        {
            for (int j = 0; j < matrix[i].Length; j++)
            {
                if (matrix[i][j] < 0)
                {
                    Console.WriteLine("ERROR");
                    Console.WriteLine("row = " + i);
                    Console.WriteLine("column = " + j);
                    Console.WriteLine("x = " + matrix[i][j]);
                }

                sum += (matrix[i][j] - average) * (matrix[i][j] - average);
            }
        }

        return sum / (matrix.Length * matrix[0].Length);
    }

    public double CalculateVariationCoefficient(double average, double dispersion)
    {
        return Math.Sqrt(dispersion) / average;
    }

    public T MinVariationCoefficient<T>(T[][] matrix)
    {
        T[] linear = MatrixHelper.GetArrayLinear(matrix);

        return linear.Min();
    }

    public T MaxVariationCoefficient<T>(T[][] matrix)
    {
        T[] linear = MatrixHelper.GetArrayLinear(matrix);

        return linear.Max();
    }

    public (T min, T max) GetMinAndMaxCoefficients<T>(T[][] matrix)
    {
        T[] linear = MatrixHelper.GetArrayLinear(matrix);

        return (linear.Min(), linear.Max());
    }

    public int CalculateLinearContrast(double min, double max, int x)
    {
        return min == max ? 255 : Convert.ToInt32((x - min) * 255 / (max - min));
    }

    public int[][] BuildLinearContrastMatrix(int[][] source)
    {
        int[][] linearContrastMatrix = new int[source.Length][];

        var variation = GetMinAndMaxCoefficients(source);

        for (int i = 0; i < source.Length; ++i)
        {
            linearContrastMatrix[i] = new int[source[i].Length];

            for (int j = 0; j < source[i].Length; ++j)
            {
                linearContrastMatrix[i][j] = CalculateLinearContrast(variation.min, variation.max, source[i][j]);
                if (linearContrastMatrix[i][j] < 0)
                {
                    Console.WriteLine("ERROR");
                    Console.WriteLine("min = " + variation.min);
                    Console.WriteLine("max = " + variation.max);
                    Console.WriteLine("x = " + source[i][j]);
                }
            }
        }

        return linearContrastMatrix;
    }

    //eta
    public double MeasureFormParameter(double variation)
    {
        return Math.Pow(variation, -1.086);
    }

    //lambda
    public double MeasureScaleParameter(double average, double dispersion)
    {
        var sqrtDispersion = Math.Sqrt(dispersion);
        return Math.Pow(average, 1.086) / Math.Pow(sqrtDispersion, 0.086);
    }

    //W^2
    public double CalculateClosure(double variation1, double variation2, double scale1, double scale2)
    {
        return (Math.Min(variation1, variation2) * Math.Min(scale1, scale2)) /
               (Math.Max(variation1, variation2) * Math.Max(scale1, scale2));
    }
}