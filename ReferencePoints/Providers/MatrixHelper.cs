using System;
using System.IO;
using System.Threading.Tasks;

namespace ReferencePoints.Providers;

public static class MatrixHelper
{
    /// <summary>
    /// Retrieve submatrix 
    /// </summary>
    /// <param name="i">Submatrix top left element index</param>
    /// <param name="j">Submatrix top left element index</param>
    /// <returns></returns>
    public static T[][] Retrieve3x3SubMatrix<T>(int i, int j, T[][] matrix)
    {
        T[][] subMatrix = new T[3][];

        for (int k = 0; k < 3; ++k)
        {
            subMatrix[k] = new T[3];

            for (int l = 0; l < 3; ++l)
            {
                subMatrix[k][l] = matrix[i + k][j + l];
            }
        }

        return subMatrix;
    }
    
    public static T[][] Retrieve9x9SubMatrix<T>(int i, int j, T[][] matrix)
    {
        T[][] subMatrix = new T[9][];

        for (int k = 0; k < 9; ++k)
        {
            subMatrix[k] = new T[9];

            for (int l = 0; l < 9; ++l)
            {
                subMatrix[k][l] = matrix[i + k][j + l];
            }
        }

        return subMatrix;
    }

    public static T[][] RetrieveSubMatrix<T>(int i, int j, T[][] matrix, int blockSize)
    {
        T[][] subMatrix = new T[blockSize][];

        for (int k = 0; k < blockSize; ++k)
        {
            subMatrix[k] = new T[blockSize];

            for (int l = 0; l < blockSize; ++l)
            {
                subMatrix[k][l] = matrix[i + k][j + l];
            }
        }

        return subMatrix;
    }

    public static T[][] CutMatrix<T>(T[][] source, int subMatrixSize = 3)
    {
        int verticalMultiplicity = source.Length % subMatrixSize;

        var matrix = new T[source.Length - verticalMultiplicity][];

        for (int i = 0; i < source.Length - verticalMultiplicity; ++i)
        {
            var horizontalMultiplicity = source[i].Length % subMatrixSize;

            matrix[i] = new T[source[i].Length - horizontalMultiplicity];

            Array.Copy(source[i], matrix[i], source[i].Length - horizontalMultiplicity);
        }

        return matrix;
    }

    public static Task ExportMatrixAsync<T>(T[][] source, Stream stream)
    {
        return Task.Run(() => { ExportMatrix(source, stream); });
    }

    public static void ExportMatrix<T>(T[][] source, Stream stream)
    {
        using (StreamWriter outputFile = new StreamWriter(stream))
        {
            outputFile.Flush();

            for (int i = 0; i < source.Length; i++)
            {
                for (int j = 0; j < source[i].Length; j++)
                {
                    outputFile.Write(source[i][j].ToString());
                    outputFile.Write(" ");
                }

                outputFile.Write(Environment.NewLine);
            }
        }
    }

    public static void ExportMatrixParameters<T>(T[][] source, Stream stream)
    {
        int height = source.Length;
        int width = source[0].Length;

        using (StreamWriter outputFile = new StreamWriter(stream))
        {
            outputFile.Flush();

            outputFile.Write(height);
            outputFile.Write(" ");
            outputFile.Write(width);
        }
    }

    public static T[] GetArrayLinear<T>(T[][] source)
    {
        T[] array = new T[source[0].Length * source.Length];

        for (int i = 0; i < source.Length; i++)
        {
            for (int j = 0; j < source[i].Length; j++)
            {
                array[i * source[i].Length + j] = source[i][j];
            }
        }

        return array;
    }

    private static string Format3Position(string value)
    {
        if (value.Length == 1)
        {
            return $"  {value}";
        }

        if (value.Length == 2)
        {
            return $" {value}";
        }

        return value;
    }
}