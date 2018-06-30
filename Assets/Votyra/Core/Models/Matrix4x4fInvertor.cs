using System;

namespace Votyra.Core.Models
{
    //https://msdn.microsoft.com/en-us/magazine/mt736457.aspx?f=255&MSPPError=-2147217396
    //http://quaetrix.com/Matrix/code.html
    internal static class Matrix4x4fInvertor
    {
        public static Matrix4x4f Invert(this Matrix4x4f mat4x4)
        {
            var matArrays = MatrixCreate(4, 4);
            matArrays[0][0] = mat4x4.m00;
            matArrays[1][0] = mat4x4.m10;
            matArrays[2][0] = mat4x4.m20;
            matArrays[3][0] = mat4x4.m30;
            matArrays[0][1] = mat4x4.m01;
            matArrays[1][1] = mat4x4.m11;
            matArrays[2][1] = mat4x4.m21;
            matArrays[3][1] = mat4x4.m31;
            matArrays[0][2] = mat4x4.m02;
            matArrays[1][2] = mat4x4.m12;
            matArrays[2][2] = mat4x4.m22;
            matArrays[3][2] = mat4x4.m32;
            matArrays[0][3] = mat4x4.m03;
            matArrays[1][3] = mat4x4.m13;
            matArrays[2][3] = mat4x4.m23;
            matArrays[3][3] = mat4x4.m33;

            var matInvertedArrays = MatrixInverse(matArrays);

            return new Matrix4x4f
                (
                    (float)matInvertedArrays[0][0],
                    (float)matInvertedArrays[0][1],
                    (float)matInvertedArrays[0][2],
                    (float)matInvertedArrays[0][3],
                    (float)matInvertedArrays[1][0],
                    (float)matInvertedArrays[1][1],
                    (float)matInvertedArrays[1][2],
                    (float)matInvertedArrays[1][3],
                    (float)matInvertedArrays[2][0],
                    (float)matInvertedArrays[2][1],
                    (float)matInvertedArrays[2][2],
                    (float)matInvertedArrays[2][3],
                    (float)matInvertedArrays[3][0],
                    (float)matInvertedArrays[3][1],
                    (float)matInvertedArrays[3][2],
                    (float)matInvertedArrays[3][3]
                );
        }

        private static double[][] MatrixCreate(int rows, int cols)
        {
            double[][] result = new double[rows][];
            for (int i = 0; i < rows; ++i)
                result[i] = new double[cols];
            return result;
        }

        private static double[][] MatrixInverse(double[][] matrix)
        {
            // assumes determinant is not 0
            // that is, the matrix does have an inverse
            int n = matrix.Length;
            double[][] result = MatrixCreate(n, n); // make a copy of matrix
            for (int i = 0; i < n; ++i)
                for (int j = 0; j < n; ++j)
                    result[i][j] = matrix[i][j];

            double[][] lum; // combined lower & upper
            int[] perm;
            MatrixDecompose(matrix, out lum, out perm);

            double[] b = new double[n];
            for (int i = 0; i < n; ++i)
            {
                for (int j = 0; j < n; ++j)
                    if (i == perm[j])
                        b[j] = 1.0;
                    else
                        b[j] = 0.0;

                double[] x = Helper(lum, b); //
                for (int j = 0; j < n; ++j)
                    result[j][i] = x[j];
            }
            return result;
        } // MatrixInverse

        private static int MatrixDecompose(double[][] m, out double[][] lum, out int[] perm)
        {
            // Crout's LU decomposition for matrix determinant and inverse
            // stores combined lower & upper in lum[][]
            // stores row permuations into perm[]
            // returns +1 or -1 according to even or odd number of row permutations
            // lower gets dummy 1.0s on diagonal (0.0s above)
            // upper gets lum values on diagonal (0.0s below)

            int toggle = +1; // even (+1) or odd (-1) row permutatuions
            int n = m.Length;

            // make a copy of m[][] into result lu[][]
            lum = MatrixCreate(n, n);
            for (int i = 0; i < n; ++i)
                for (int j = 0; j < n; ++j)
                    lum[i][j] = m[i][j];

            // make perm[]
            perm = new int[n];
            for (int i = 0; i < n; ++i)
                perm[i] = i;

            for (int j = 0; j < n - 1; ++j) // process by column. note n-1
            {
                double max = Math.Abs(lum[j][j]);
                int piv = j;

                for (int i = j + 1; i < n; ++i) // find pivot index
                {
                    double xij = Math.Abs(lum[i][j]);
                    if (xij > max)
                    {
                        max = xij;
                        piv = i;
                    }
                } // i

                if (piv != j)
                {
                    double[] tmp = lum[piv]; // swap rows j, piv
                    lum[piv] = lum[j];
                    lum[j] = tmp;

                    int t = perm[piv]; // swap perm elements
                    perm[piv] = perm[j];
                    perm[j] = t;

                    toggle = -toggle;
                }

                double xjj = lum[j][j];
                if (xjj != 0.0)
                {
                    for (int i = j + 1; i < n; ++i)
                    {
                        double xij = lum[i][j] / xjj;
                        lum[i][j] = xij;
                        for (int k = j + 1; k < n; ++k)
                            lum[i][k] -= xij * lum[j][k];
                    }
                }
            } // j

            return toggle;
        }

        private static double[] Helper(double[][] luMatrix, double[] b) // helper
        {
            int n = luMatrix.Length;
            double[] x = new double[n];
            b.CopyTo(x, 0);

            for (int i = 1; i < n; ++i)
            {
                double sum = x[i];
                for (int j = 0; j < i; ++j)
                    sum -= luMatrix[i][j] * x[j];
                x[i] = sum;
            }

            x[n - 1] /= luMatrix[n - 1][n - 1];
            for (int i = n - 2; i >= 0; --i)
            {
                double sum = x[i];
                for (int j = i + 1; j < n; ++j)
                    sum -= luMatrix[i][j] * x[j];
                x[i] = sum / luMatrix[i][i];
            }

            return x;
        } // Helper
    }
}