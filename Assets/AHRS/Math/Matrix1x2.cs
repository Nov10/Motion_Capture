using System;
using Unity.VisualScripting;

namespace AHRS
{
    //(x, y)
    public struct Matrix1x2
    {
        public static Matrix1x2 One
        {
            get { return new Matrix1x2(1, 1); }
        }
        public double[,] elements;
        public double this[int i, int j]
        {
            get { return elements[i, j]; }
            set { elements[i, j] = value; }
        }
        public Matrix1x2(double m00, double m01)
        {
            elements = new double[1, 2];
            elements[0, 0] = m00;
            elements[0, 1] = m01;
        }

        public static Matrix1x2 operator +(Matrix1x2 a, Matrix1x2 b)
        {
            Matrix1x2 result = new Matrix1x2(0, 0);
            for (int i = 0; i < 1; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    result.elements[i, j] = a.elements[i, j] + b.elements[i, j];
                }
            }
            return result;
        }

        public static Matrix1x2 operator -(Matrix1x2 a, Matrix1x2 b)
        {
            Matrix1x2 result = new Matrix1x2(0, 0);
            for (int i = 0; i < 1; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    result.elements[i, j] = a.elements[i, j] - b.elements[i, j];
                }
            }
            return result;
        }

        public static Matrix1x2 operator *(Matrix1x2 a, double scalar)
        {
            Matrix1x2 result = new Matrix1x2(0, 0);
            for (int i = 0; i < 1; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    result.elements[i, j] = a.elements[i, j] * scalar;
                }
            }
            return result;
        }

        public static Matrix1x2 operator *(double scalar, Matrix1x2 a)
        {
            return a * scalar;
        }

        public static Matrix1x2 operator /(Matrix1x2 a, double scalar)
        {
            if (scalar == 0)
            {
                throw new DivideByZeroException("Division by zero");
            }

            Matrix1x2 result = new Matrix1x2(0, 0);
            for (int i = 0; i < 1; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    result.elements[i, j] = a.elements[i, j] / scalar;
                }
            }
            return result;
        }

        public Vector2 Transpose()
        {
            return new Vector2(elements[0, 0], elements[0, 1]);
        }
        public static Matrix1x2 operator *(Matrix1x2 matrix, Matrix2x2 matrix2x2)
        {
            double x = matrix.elements[0, 0] * matrix2x2.elements[0, 0] + matrix.elements[0, 1] * matrix2x2.elements[1, 0];
            double y = matrix.elements[0, 0] * matrix2x2.elements[0, 1] + matrix.elements[0, 1] * matrix2x2.elements[1, 1];
            return new Matrix1x2(x, y);
        }
        public static double operator *(Matrix1x2 matrix, Vector2 vector)
        {
            double x = matrix.elements[0, 0] * vector.x + matrix.elements[0, 1] * vector.y;
            return x;
        }

        public static Matrix2x2 operator *(Vector2 vector, Matrix1x2 matrix)
        {
            //double x = matrix.elements[0, 0] * vector.x + matrix.elements[0, 1] * vector.y;
            return new Matrix2x2(vector.x * matrix[0, 0], vector.x * matrix[0, 1],
                                 vector.y * matrix[0, 0], vector.y * matrix[0, 1]);
        }
        public override string ToString()
        {
            return $"[{elements[0, 0]}, {elements[0, 1]}]";
        }
    }
}
