using System;

namespace AHRS
{
    [System.Serializable]
    public struct Vector2
    {
        public double x;
        public double y;

        public Vector2(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public static Vector2 operator +(Vector2 a, Vector2 b)
        {
            return new Vector2(a.x + b.x, a.y + b.y);
        }

        public static Vector2 operator -(Vector2 a, Vector2 b)
        {
            return new Vector2(a.x - b.x, a.y - b.y);
        }

        public static Vector2 operator *(Vector2 a, double scalar)
        {
            return new Vector2(a.x * scalar, a.y * scalar);
        }
        public static Vector2 operator *(double scalar, Vector2 a)
        {
            return scalar * a;
        }
        public static Vector2 operator /(Vector2 a, double scalar)
        {
            if (scalar == 0)
            {
                throw new DivideByZeroException("Division by zero");
            }

            return new Vector2(a.x / scalar, a.y / scalar);
        }

        public double Magnitude()
        {
            return Math.Sqrt(x * x + y * y);
        }

        public Vector2 Normalize()
        {
            double mag = Magnitude();
            if (mag == 0)
            {
                throw new DivideByZeroException("Vector has zero magnitude");
            }

            return new Vector2(x / mag, y / mag);
        }

        public Matrix1x2 Transpose()
        {
            return new Matrix1x2(x, y);
        }

        public override string ToString()
        {
            return $"({x}, {y})";
        }
    }
}