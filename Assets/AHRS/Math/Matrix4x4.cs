using System;

namespace AHRS
{
    public struct Matrix4x4
    {
        private double[,] elements;// = new double[4, 4];

        private static Matrix4x4 _Identity =
            new Matrix4x4(new double[,]{
                {1, 0, 0, 0},
                {0, 1, 0, 0},
                {0, 0, 1, 0},
                {0, 0, 0, 1}
            });
        public static Matrix4x4 Identity
        {
            get
            {
                return _Identity;
            }
        }

        public Matrix4x4(double[,] values)
        {
            elements = new double[4, 4];
            if (values.GetLength(0) != 4 || values.GetLength(1) != 4)
            {
                throw new ArgumentException("Matrix must be 4x4.");
            }

            elements = values;
        }

        public double this[int i, int j]
        {
            get { return elements[i, j]; }
            set { elements[i, j] = value; }
        }

        public static Matrix4x4 operator +(Matrix4x4 m1, Matrix4x4 m2)
        {
            double[,] result = new double[4, 4];
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    result[i, j] = m1[i, j] + m2[i, j];
                }
            }
            return new Matrix4x4(result);
        }

        public static Matrix4x4 operator -(Matrix4x4 m1, Matrix4x4 m2)
        {
            double[,] result = new double[4, 4];
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    result[i, j] = m1[i, j] - m2[i, j];
                }
            }
            return new Matrix4x4(result);
        }

        public static Matrix4x4 operator *(Matrix4x4 m1, Matrix4x4 m2)
        {
            double[,] result = new double[4, 4];
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    for (int k = 0; k < 4; k++)
                    {
                        result[i, j] += m1[i, k] * m2[k, j];
                    }
                }
            }
            return new Matrix4x4(result);
        }

        public static Matrix4x4 operator *(double scalar, Matrix4x4 m)
        {
            double[,] result = new double[4, 4];
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    result[i, j] = scalar * m[i, j];
                }
            }
            return new Matrix4x4(result);
        }

        public static Matrix4x4 operator /(Matrix4x4 m, double scalar)
        {
            if (scalar == 0)
            {
                throw new ArgumentException("Division by zero.");
            }
            double[,] result = new double[4, 4];
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    result[i, j] = m[i, j] / scalar;
                }
            }
            return new Matrix4x4(result);
        }

        public Matrix4x4 Transpose()
        {
            double[,] result = new double[4, 4];
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    result[i, j] = elements[j, i];
                }
            }
            return new Matrix4x4(result);
        }

        public double Determinant()
        {
            double det =
                elements[0, 0] * (elements[1, 1] * (elements[2, 2] * elements[3, 3] - elements[2, 3] * elements[3, 2]) -
                                  elements[1, 2] * (elements[2, 1] * elements[3, 3] - elements[2, 3] * elements[3, 1]) +
                                  elements[1, 3] * (elements[2, 1] * elements[3, 2] - elements[2, 2] * elements[3, 1])) -
                elements[0, 1] * (elements[1, 0] * (elements[2, 2] * elements[3, 3] - elements[2, 3] * elements[3, 2]) -
                                  elements[1, 2] * (elements[2, 0] * elements[3, 3] - elements[2, 3] * elements[3, 0]) +
                                  elements[1, 3] * (elements[2, 0] * elements[3, 2] - elements[2, 2] * elements[3, 0])) +
                elements[0, 2] * (elements[1, 0] * (elements[2, 1] * elements[3, 3] - elements[2, 3] * elements[3, 1]) -
                                  elements[1, 1] * (elements[2, 0] * elements[3, 3] - elements[2, 3] * elements[3, 0]) +
                                  elements[1, 3] * (elements[2, 0] * elements[3, 1] - elements[2, 1] * elements[3, 0])) -
                elements[0, 3] * (elements[1, 0] * (elements[2, 1] * elements[3, 2] - elements[2, 2] * elements[3, 1]) -
                                  elements[1, 1] * (elements[2, 0] * elements[3, 2] - elements[2, 2] * elements[3, 0]) +
                                  elements[1, 2] * (elements[2, 0] * elements[3, 1] - elements[2, 1] * elements[3, 0]));

            return det;
        }

        public Matrix4x4 Inverse()
        {
            double det = Determinant();
            if (det == 0)
            {
                throw new InvalidOperationException("Matrix is singular and has no inverse.");
            }

            double[,] result = new double[4, 4];

            result[0, 0] = (elements[1, 1] * (elements[2, 2] * elements[3, 3] - elements[2, 3] * elements[3, 2]) -
                            elements[1, 2] * (elements[2, 1] * elements[3, 3] - elements[2, 3] * elements[3, 1]) +
                            elements[1, 3] * (elements[2, 1] * elements[3, 2] - elements[2, 2] * elements[3, 1])) / det;
            result[0, 1] = (elements[0, 2] * (elements[2, 3] * elements[3, 1] - elements[2, 1] * elements[3, 3]) -
                            elements[0, 1] * (elements[2, 3] * elements[3, 2] - elements[2, 2] * elements[3, 3]) +
                            elements[0, 3] * (elements[2, 1] * elements[3, 2] - elements[2, 2] * elements[3, 1])) / det;
            result[0, 2] = (elements[0, 1] * (elements[1, 3] * elements[3, 2] - elements[1, 2] * elements[3, 3]) -
                            elements[0, 2] * (elements[1, 1] * elements[3, 3] - elements[1, 3] * elements[3, 1]) +
                            elements[0, 3] * (elements[1, 2] * elements[3, 1] - elements[1, 1] * elements[3, 2])) / det;
            result[0, 3] = (elements[0, 2] * (elements[1, 1] * elements[2, 3] - elements[1, 3] * elements[2, 1]) -
                            elements[0, 1] * (elements[1, 2] * elements[2, 3] - elements[1, 3] * elements[2, 2]) +
                            elements[0, 3] * (elements[1, 1] * elements[2, 2] - elements[1, 2] * elements[2, 1])) / det;

            result[1, 0] = (elements[1, 2] * (elements[2, 3] * elements[3, 0] - elements[2, 0] * elements[3, 3]) -
                            elements[1, 0] * (elements[2, 3] * elements[3, 2] - elements[2, 2] * elements[3, 3]) +
                            elements[1, 3] * (elements[2, 0] * elements[3, 2] - elements[2, 2] * elements[3, 0])) / det;
            result[1, 1] = (elements[0, 0] * (elements[2, 3] * elements[3, 2] - elements[2, 2] * elements[3, 3]) -
                            elements[0, 2] * (elements[2, 3] * elements[3, 0] - elements[2, 0] * elements[3, 3]) +
                            elements[0, 3] * (elements[2, 2] * elements[3, 0] - elements[2, 0] * elements[3, 2])) / det;
            result[1, 2] = (elements[0, 2] * (elements[1, 3] * elements[3, 0] - elements[1, 0] * elements[3, 3]) -
                            elements[0, 0] * (elements[1, 2] * elements[3, 3] - elements[1, 3] * elements[3, 2]) +
                            elements[0, 3] * (elements[1, 0] * elements[3, 2] - elements[1, 2] * elements[3, 0])) / det;
            result[1, 3] = (elements[0, 0] * (elements[1, 2] * elements[2, 3] - elements[1, 3] * elements[2, 2]) -
                            elements[0, 2] * (elements[1, 0] * elements[2, 3] - elements[1, 3] * elements[2, 0]) +
                            elements[0, 3] * (elements[1, 0] * elements[2, 2] - elements[1, 2] * elements[2, 0])) / det;

            result[2, 0] = (elements[1, 0] * (elements[2, 1] * elements[3, 3] - elements[2, 3] * elements[3, 1]) -
                            elements[1, 1] * (elements[2, 0] * elements[3, 3] - elements[2, 3] * elements[3, 0]) +
                            elements[1, 3] * (elements[2, 0] * elements[3, 1] - elements[2, 1] * elements[3, 0])) / det;
            result[2, 1] = (elements[0, 1] * (elements[2, 3] * elements[3, 0] - elements[2, 0] * elements[3, 3]) -
                            elements[0, 0] * (elements[2, 3] * elements[3, 1] - elements[2, 1] * elements[3, 3]) +
                            elements[0, 3] * (elements[2, 0] * elements[3, 1] - elements[2, 1] * elements[3, 0])) / det;
            result[2, 2] = (elements[0, 0] * (elements[1, 1] * elements[3, 3] - elements[1, 3] * elements[3, 1]) -
                            elements[0, 1] * (elements[1, 0] * elements[3, 3] - elements[1, 3] * elements[3, 0]) +
                            elements[0, 3] * (elements[1, 0] * elements[3, 1] - elements[1, 1] * elements[3, 0])) / det;
            result[2, 3] = (elements[0, 1] * (elements[1, 0] * elements[2, 3] - elements[1, 3] * elements[2, 0]) -
                            elements[0, 0] * (elements[1, 1] * elements[2, 3] - elements[1, 3] * elements[2, 1]) +
                            elements[0, 3] * (elements[1, 0] * elements[2, 1] - elements[1, 1] * elements[2, 0])) / det;

            result[3, 0] = (elements[1, 1] * (elements[2, 2] * elements[3, 0] - elements[2, 0] * elements[3, 2]) -
                            elements[1, 0] * (elements[2, 2] * elements[3, 1] - elements[2, 1] * elements[3, 2]) +
                            elements[1, 2] * (elements[2, 0] * elements[3, 1] - elements[2, 1] * elements[3, 0])) / det;
            result[3, 1] = (elements[0, 0] * (elements[2, 2] * elements[3, 1] - elements[2, 1] * elements[3, 2]) -
                            elements[0, 1] * (elements[2, 2] * elements[3, 0] - elements[2, 0] * elements[3, 2]) +
                            elements[0, 2] * (elements[2, 0] * elements[3, 1] - elements[2, 1] * elements[3, 0])) / det;
            result[3, 2] = (elements[0, 1] * (elements[1, 2] * elements[3, 0] - elements[1, 0] * elements[3, 2]) -
                            elements[0, 0] * (elements[1, 2] * elements[3, 1] - elements[1, 1] * elements[3, 2]) +
                            elements[0, 2] * (elements[1, 0] * elements[3, 1] - elements[1, 1] * elements[3, 0])) / det;
            result[3, 3] = (elements[0, 0] * (elements[1, 1] * elements[2, 2] - elements[1, 2] * elements[2, 1]) -
                            elements[0, 1] * (elements[1, 0] * elements[2, 2] - elements[1, 2] * elements[2, 0]) +
                            elements[0, 2] * (elements[1, 0] * elements[2, 1] - elements[1, 1] * elements[2, 0])) / det;

            return new Matrix4x4(result);
        }

        public static Vector4 operator *(Matrix4x4 m, Vector4 v)
        {
            double x = m[0, 0] * v.x + m[0, 1] * v.y + m[0, 2] * v.z + m[0, 3] * v.w;
            double y = m[1, 0] * v.x + m[1, 1] * v.y + m[1, 2] * v.z + m[1, 3] * v.w;
            double z = m[2, 0] * v.x + m[2, 1] * v.y + m[2, 2] * v.z + m[2, 3] * v.w;
            double w = m[3, 0] * v.x + m[3, 1] * v.y + m[3, 2] * v.z + m[3, 3] * v.w;

            return new Vector4(x, y, z, w);
        }

        public override string ToString()
        {
            string result = "";
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    result += elements[i, j] + " ";
                }
                result += "\n";
            }
            return result;
        }
    }
}