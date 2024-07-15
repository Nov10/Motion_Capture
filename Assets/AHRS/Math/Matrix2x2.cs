using System;

namespace AHRS
{
    [System.Serializable]
    public struct Matrix2x2
    {
        public double[,] elements;// = new double[2, 2];
        public double this[int i, int j]
        {
            get { return elements[i, j]; }
            set { elements[i, j] = value; }
        }
        public Matrix2x2(double m00, double m01, double m10, double m11)
        {
            elements = new double[2, 2];
            elements[0, 0] = m00;
            elements[0, 1] = m01;
            elements[1, 0] = m10;
            elements[1, 1] = m11;
        }

        public static Matrix2x2 operator +(Matrix2x2 a, Matrix2x2 b)
        {
            Matrix2x2 result = new Matrix2x2(0, 0, 0, 0);
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    result.elements[i, j] = a.elements[i, j] + b.elements[i, j];
                }
            }
            return result;
        }

        public static Matrix2x2 operator -(Matrix2x2 a, Matrix2x2 b)
        {
            Matrix2x2 result = new Matrix2x2(0, 0, 0, 0);
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    result.elements[i, j] = a.elements[i, j] - b.elements[i, j];
                }
            }
            return result;
        }

        public static Matrix2x2 operator *(Matrix2x2 a, Matrix2x2 b)
        {
            Matrix2x2 result = new Matrix2x2(0, 0, 0, 0);
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    for (int k = 0; k < 2; k++)
                    {
                        result.elements[i, j] += a.elements[i, k] * b.elements[k, j];
                    }
                }
            }
            return result;
        }

        public static Matrix2x2 operator *(Matrix2x2 a, double scalar)
        {
            Matrix2x2 result = new Matrix2x2(0, 0, 0, 0);
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    result.elements[i, j] = a.elements[i, j] * scalar;
                }
            }
            return result;
        }


        public static Matrix2x2 operator *(double scalar, Matrix2x2 a)
        {
            return a * scalar;
        }
        public static Matrix2x2 operator /(Matrix2x2 a, double scalar)
        {
            if (scalar == 0)
            {
                throw new DivideByZeroException("Division by zero");
            }

            Matrix2x2 result = new Matrix2x2(0, 0, 0, 0);
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    result.elements[i, j] = a.elements[i, j] / scalar;
                }
            }
            return result;
        }

        public Matrix2x2 Transpose()
        {
            Matrix2x2 result = new Matrix2x2(0, 0, 0, 0);
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    result.elements[i, j] = elements[j, i];
                }
            }
            return result;
        }

        public static Vector2 operator *(Matrix2x2 matrix, Vector2 vector)
        {
            double x = matrix.elements[0, 0] * vector.x + matrix.elements[0, 1] * vector.y;
            double y = matrix.elements[1, 0] * vector.x + matrix.elements[1, 1] * vector.y;
            return new Vector2(x, y);
        }

        public static Matrix2x2 Identity()
        {
            return new Matrix2x2(1, 0, 0, 1);
        }

        public override string ToString()
        {
            return $"[{elements[0, 0]}, {elements[0, 1]}\n {elements[1, 0]}, {elements[1, 1]}]";
        }
    }

}