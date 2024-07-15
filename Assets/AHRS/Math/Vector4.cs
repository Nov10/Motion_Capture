using System;

namespace AHRS
{
    public struct Vector4
    {
        public double x { get; set; }
        public double y { get; set; }
        public double z { get; set; }
        public double w { get; set; }
        public Vector4 Normalize()
        {
            return this / Magnitude();
        }
        public double Magnitude()
        {
            return Math.Sqrt(x * x + y * y + z * z + w*w);
        }
        public Vector4(double x, double y, double z, double w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        // º¤ÅÍ µ¡¼À
        public static Vector4 operator +(Vector4 v1, Vector4 v2)
        {
            return new Vector4(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z, v1.w + v2.w);
        }

        // º¤ÅÍ »¬¼À
        public static Vector4 operator -(Vector4 v1, Vector4 v2)
        {
            return new Vector4(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z, v1.w - v2.w);
        }

        // ½Ç¼ö¹è
        public static Vector4 operator *(double scalar, Vector4 v)
        {
            return new Vector4(scalar * v.x, scalar * v.y, scalar * v.z, scalar * v.w);
        }
        public static Vector4 operator *(Vector4 v, double scalar)
        {
            return scalar * v;
        }
        // ³ª´©±â
        public static Vector4 operator /(Vector4 v, double scalar)
        {
            if (scalar == 0)
            {
                throw new ArgumentException("Division by zero.");
            }
            return new Vector4(v.x / scalar, v.y / scalar, v.z / scalar, v.w / scalar);
        }

        // ³»Àû (Dot Product)
        public static double Dot(Vector4 v1, Vector4 v2)
        {
            return v1.x * v2.x + v1.y * v2.y + v1.z * v2.z + v1.w * v2.w;
        }

        public override string ToString()
        {
            return $"({x}, {y}, {z}, {w})";
        }
    }
}