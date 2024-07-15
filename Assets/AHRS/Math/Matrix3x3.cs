using System;

namespace AHRS
{
    public struct Matrix3x3
    {
        private double[,] elements;// = new double[3, 3];

        public Matrix3x3(double[,] values)
        {
            elements = new double[3, 3];
            if (values.GetLength(0) != 3 || values.GetLength(1) != 3)
            {
                throw new ArgumentException("Matrix must be 3x3.");
            }

            elements = values;
        }

        private static Matrix3x3 _Identity =
            new Matrix3x3(new double[,]{
                {1, 0, 0},
                {0, 1, 0},
                {0, 0, 1}
            });
        public static Matrix3x3 Identity
        {
            get
            {
                return _Identity;
            }
        }

        // Çà·Ä µ¡¼À
        public static Matrix3x3 operator +(Matrix3x3 m1, Matrix3x3 m2)
        {
            double[,] result = new double[3, 3];
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    result[i, j] = m1[i, j] + m2[i, j];
                }
            }
            return new Matrix3x3(result);
        }

        // Çà·Ä »¬¼À
        public static Matrix3x3 operator -(Matrix3x3 m1, Matrix3x3 m2)
        {
            double[,] result = new double[3, 3];
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    result[i, j] = m1[i, j] - m2[i, j];
                }
            }
            return new Matrix3x3(result);
        }

        // Çà·Ä °ö¼À
        public static Matrix3x3 operator *(Matrix3x3 m1, Matrix3x3 m2)
        {
            double[,] result = new double[3, 3];
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        result[i, j] += m1[i, k] * m2[k, j];
                    }
                }
            }
            return new Matrix3x3(result);
        }

        // Çà·Ä ½Ç¼ö¹è
        public static Matrix3x3 operator *(double scalar, Matrix3x3 m)
        {
            double[,] result = new double[3, 3];
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    result[i, j] = scalar * m[i, j];
                }
            }
            return new Matrix3x3(result);
        }

        // Çà·Ä ³ª´°¼À
        public static Matrix3x3 operator /(Matrix3x3 m, double scalar)
        {
            if (scalar == 0)
            {
                throw new ArgumentException("Division by zero.");
            }
            double[,] result = new double[3, 3];
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    result[i, j] = m[i, j] / scalar;
                }
            }
            return new Matrix3x3(result);
        }

        // ÀüÄ¡Çà·Ä °è»ê
        public Matrix3x3 Transpose()
        {
            double[,] result = new double[3, 3];
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    result[i, j] = elements[j, i];
                }
            }
            return new Matrix3x3(result);
        }

        // ¿ªÇà·Ä °è»ê
        public Matrix3x3 Inverse()
        {
            double det = Determinant();
            if (det == 0)
            {
                throw new InvalidOperationException("Matrix is singular and has no inverse.");
            }

            double[,] result = new double[3, 3];

            result[0, 0] = (elements[1, 1] * elements[2, 2] - elements[1, 2] * elements[2, 1]) / det;
            result[0, 1] = (elements[0, 2] * elements[2, 1] - elements[0, 1] * elements[2, 2]) / det;
            result[0, 2] = (elements[0, 1] * elements[1, 2] - elements[0, 2] * elements[1, 1]) / det;
            result[1, 0] = (elements[1, 2] * elements[2, 0] - elements[1, 0] * elements[2, 2]) / det;
            result[1, 1] = (elements[0, 0] * elements[2, 2] - elements[0, 2] * elements[2, 0]) / det;
            result[1, 2] = (elements[0, 2] * elements[1, 0] - elements[0, 0] * elements[1, 2]) / det;
            result[2, 0] = (elements[1, 0] * elements[2, 1] - elements[1, 1] * elements[2, 0]) / det;
            result[2, 1] = (elements[0, 1] * elements[2, 0] - elements[0, 0] * elements[2, 1]) / det;
            result[2, 2] = (elements[0, 0] * elements[1, 1] - elements[0, 1] * elements[1, 0]) / det;

            return new Matrix3x3(result);
        }

        // Çà·Ä½Ä °è»ê
        public double Determinant()
        {
            double det =
                elements[0, 0] * (elements[1, 1] * elements[2, 2] - elements[1, 2] * elements[2, 1]) -
                elements[0, 1] * (elements[1, 0] * elements[2, 2] - elements[1, 2] * elements[2, 0]) +
                elements[0, 2] * (elements[1, 0] * elements[2, 1] - elements[1, 1] * elements[2, 0]);

            return det;
        }
        public double this[int i, int j]
        {
            get { return elements[i, j]; }
            set { elements[i, j] = value; }
        }
        public static Vector3 operator *(Matrix3x3 m, Vector3 v)
        {
            double x = m[0, 0] * v.x + m[0, 1] * v.y + m[0, 2] * v.z;
            double y = m[1, 0] * v.x + m[1, 1] * v.y + m[1, 2] * v.z;
            double z = m[2, 0] * v.x + m[2, 1] * v.y + m[2, 2] * v.z;

            return new Vector3(x, y, z);
        }
        public override string ToString()
        {
            string result = "";
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    result += elements[i, j] + " ";
                }
                result += "\n";
            }
            return result;
        }
    }
}