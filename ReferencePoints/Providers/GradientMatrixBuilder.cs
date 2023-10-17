using System;

namespace ReferencePoints.Providers;

    public class GradientMatrixBuilder
    {
        public int CalculateVerticalGradient(int[] local3x3Elements)
        {
            return -local3x3Elements[0] - 2 * local3x3Elements[1] - local3x3Elements[2] + local3x3Elements[6] + 2 * local3x3Elements[7] + local3x3Elements[8];
        }

        public int CalculateHorizontalGradient(int[] local3x3Elements)
        {
            return -local3x3Elements[0] - 2 * local3x3Elements[3] - local3x3Elements[6] + local3x3Elements[2] + 2 * local3x3Elements[5] + local3x3Elements[8];
        }

        public double CalculateMagnitude(int horizontalGradient, int verticalGradient)
        {
            return Math.Sqrt(horizontalGradient * horizontalGradient + verticalGradient * verticalGradient);
        }

        public int[][] BuildGradientMatrix(int[][] source, bool verticalOnly = false, bool horizontalOnly = false)
        {
            int[][] cuttedMatrix = MatrixHelper.CutMatrix(source);

            int[][] gradientMatrix = new int[cuttedMatrix.Length - 2][];

            for (int i = 0; i < cuttedMatrix.Length; i++)
            {
                if (i + 2 >= cuttedMatrix.Length) break;

                gradientMatrix[i] = new int[cuttedMatrix[i].Length - 2];
            }

            for (int i = 0; i < cuttedMatrix.Length - 2; ++i)
            {
                for (int j = 0; j < cuttedMatrix[i].Length; ++j)
                {
                    if (j + 2 >= cuttedMatrix[i].Length) break;

                    var subMatrix3x3 = MatrixHelper.Retrieve3x3SubMatrix(i, j, cuttedMatrix);
                    int verticalGradient = CalculateVerticalGradient(MatrixHelper.GetArrayLinear(subMatrix3x3));
                    int horizontalGradient = CalculateHorizontalGradient(MatrixHelper.GetArrayLinear(subMatrix3x3));

                    if (verticalOnly)
                    {
                        gradientMatrix[i][j] = verticalGradient;
                    }
                    else if (horizontalOnly)
                    {
                        gradientMatrix[i][j] = horizontalGradient;
                    }
                    else
                    {
                        gradientMatrix[i][j] = Convert.ToInt32(CalculateMagnitude(verticalGradient, horizontalGradient));
                    }
                }
            }

            return gradientMatrix;
        }    
        
        public int[][] BuildGradientMatrix9x9(int[][] source, bool verticalOnly = false, bool horizontalOnly = false)
        {
            int[][] cutMatrix = MatrixHelper.CutMatrix(source, 9);
            int[][] gradientMatrix = new int[cutMatrix.Length - 2][];

            for (int i = 0; i < cutMatrix.Length; i++)
            {
                if (i + 2 >= cutMatrix.Length) break;

                gradientMatrix[i] = new int[cutMatrix[i].Length - 2];
            }

            for (int i = 0; i < cutMatrix.Length - 2; ++i)
            {
                for (int j = 0; j < cutMatrix[i].Length; ++j)
                {
                    if (j + 2 >= cutMatrix[i].Length) break;

                    var subMatrix3x3 = MatrixHelper.Retrieve9x9SubMatrix(i, j, cutMatrix);
                    int verticalGradient = CalculateVerticalGradient(MatrixHelper.GetArrayLinear(subMatrix3x3));
                    int horizontalGradient = CalculateHorizontalGradient(MatrixHelper.GetArrayLinear(subMatrix3x3));

                    if (verticalOnly)
                    {
                        gradientMatrix[i][j] = verticalGradient;
                    }
                    else if (horizontalOnly)
                    {
                        gradientMatrix[i][j] = horizontalGradient;
                    }
                    else
                    {
                        gradientMatrix[i][j] = Convert.ToInt32(CalculateMagnitude(verticalGradient, horizontalGradient));
                    }
                }
            }

            return gradientMatrix;
        }       

    }
