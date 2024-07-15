using System;

namespace AHRS
{
    public class Matrix4x3
    {
        public double[,] Data { get; private set; }

        public Matrix4x3(double[,] data)
        {
            if (data.GetLength(0) != 4 || data.GetLength(1) != 3)
            {
                throw new ArgumentException("Matrix must be 4x3.");
            }

            Data = data;
        }

        public static Matrix4x3 operator *(Matrix4x3 m1, Matrix4x3 m2)
        {
            double[,] result = new double[4, 3];
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    result[i, j] = 0;
                    for (int k = 0; k < 3; k++)
                    {
                        result[i, j] += m1.Data[i, k] * m2.Data[k, j];
                    }
                }
            }
            return new Matrix4x3(result);
        }

        public static Matrix4x3 operator +(Matrix4x3 m1, Matrix4x3 m2)
        {
            double[,] result = new double[4, 3];
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    result[i, j] = m1.Data[i, j] + m2.Data[i, j];
                }
            }
            return new Matrix4x3(result);
        }

        public static Matrix4x3 operator -(Matrix4x3 m1, Matrix4x3 m2)
        {
            double[,] result = new double[4, 3];
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    result[i, j] = m1.Data[i, j] - m2.Data[i, j];
                }
            }
            return new Matrix4x3(result);
        }

        public static Vector4 operator *(Matrix4x3 m, Vector3 v)
        {
            double x = m.Data[0, 0] * v.x + m.Data[0, 1] * v.y + m.Data[0, 2] * v.z;
            double y = m.Data[1, 0] * v.x + m.Data[1, 1] * v.y + m.Data[1, 2] * v.z;
            double z = m.Data[2, 0] * v.x + m.Data[2, 1] * v.y + m.Data[2, 2] * v.z;
            double w = m.Data[3, 0] * v.x + m.Data[3, 1] * v.y + m.Data[3, 2] * v.z;

            return new Vector4(x, y, z, w);
        }
        public double this[int i, int j]
        {
            get { return Data[i, j]; }
            set { Data[i, j] = value; }
        }
        public static Matrix4x3 Identity()
        {
            double[,] identity = new double[4, 3]
            {
                { 1, 0, 0 },
                { 0, 1, 0 },
                { 0, 0, 1 },
                { 0, 0, 0 }
            };
            return new Matrix4x3(identity);
        }
    }
}