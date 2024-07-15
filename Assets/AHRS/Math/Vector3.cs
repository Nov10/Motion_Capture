using System;

namespace AHRS
{
    [System.Serializable]
    public struct Vector3
    {
        public double x;// { get; set; }
        public double y;// { get; set; }
        public double z;// { get; set; }

        public UnityEngine.Vector3 Convert()
        {
            return new UnityEngine.Vector3((float)x, (float)y, (float)z);
        }
        public Vector3(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        // º¤ÅÍ µ¡¼À
        public static Vector3 operator +(Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z);
        }

        // º¤ÅÍ »¬¼À
        public static Vector3 operator -(Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z);
        }

        // ½Ç¼ö¹è
        public static Vector3 operator *(double scalar, Vector3 v)
        {
            return new Vector3(scalar * v.x, scalar * v.y, scalar * v.z);
        }
        public static Vector3 operator *(Vector3 v, double scalar)
        {
            return scalar * v;
        }
        // ³ª´©±â
        public static Vector3 operator /(Vector3 v, double scalar)
        {
            if (scalar == 0)
            {
                throw new ArgumentException("Division by zero.");
            }
            return new Vector3(v.x / scalar, v.y / scalar, v.z / scalar);
        }

        public Vector3 Normalize()
        {
            return this / Magnitude();
        }

        // ¿ÜÀû (Cross Product)
        public static Vector3 Cross(Vector3 v1, Vector3 v2)
        {
            double x = v1.y * v2.z - v1.z * v2.y;
            double y = v1.z * v2.x - v1.x * v2.z;
            double z = v1.x * v2.y - v1.y * v2.x;
            return new Vector3(x, y, z);
        }

        // ³»Àû (Dot Product)
        public static double Dot(Vector3 v1, Vector3 v2)
        {
            return v1.x * v2.x + v1.y * v2.y + v1.z * v2.z;
        }

        // º¤ÅÍÀÇ Å©±â
        public double Magnitude()
        {
            return Math.Sqrt(x * x + y * y + z * z);
        }

        public override string ToString()
        {
            return $"({x}, {y}, {z})";
        }
    }
}