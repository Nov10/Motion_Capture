using System;
using UnityEngine;

namespace AHRS
{
    public struct Quaternion
    {
        public double x { get; set; }
        public double y { get; set; }
        public double z { get; set; }
        public double w { get; set; }

        public Quaternion(double w, double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        // ÄõÅÍ´Ï¾ð µ¡¼À
        public static Quaternion operator +(Quaternion q1, Quaternion q2)
        {
            return new Quaternion(q1.w + q2.w, q1.x + q2.x, q1.y + q2.y, q1.z + q2.z);
        }

        // ÄõÅÍ´Ï¾ð »¬¼À
        public static Quaternion operator -(Quaternion q1, Quaternion q2)
        {
            return new Quaternion(q1.w - q2.w, q1.x - q2.x, q1.y - q2.y, q1.z - q2.z);
        }

        // ÄõÅÍ´Ï¾ð °ö¼À
        public static Quaternion operator *(Quaternion q1, Quaternion q2)
        {
            double w = q1.w * q2.w - q1.x * q2.x - q1.y * q2.y - q1.z * q2.z;
            double x = q1.w * q2.x + q1.x * q2.w + q1.y * q2.z - q1.z * q2.y;
            double y = q1.w * q2.y + q1.y * q2.w + q1.z * q2.x - q1.x * q2.z;
            double z = q1.w * q2.z + q1.z * q2.w + q1.x * q2.y - q1.y * q2.x;

            return new Quaternion(w, x, y, z);
        }

        // ÄõÅÍ´Ï¾ð°ú ½ºÄ®¶óÀÇ °ö¼À
        public static Quaternion operator *(Quaternion q, double scalar)
        {
            return new Quaternion(q.w * scalar, q.x * scalar, q.y * scalar, q.z * scalar);
        }
        public static Quaternion operator *(double scalar, Quaternion q)
        {
            return q * scalar;
        }
        // ÄõÅÍ´Ï¾ð°ú ½ºÄ®¶óÀÇ ³ª´°¼À
        public static Quaternion operator /(Quaternion q, double scalar)
        {
            if (scalar == 0)
            {
                throw new ArgumentException("Division by zero.");
            }

            return new Quaternion(q.w / scalar, q.x / scalar, q.y / scalar, q.z / scalar);
        }

        // ÄõÅÍ´Ï¾ðÀÇ Å©±â
        public double Magnitude()
        {
            return Math.Sqrt(x * x + y * y + z * z + w * w);
        }

        // ÄõÅÍ´Ï¾ð Á¤±ÔÈ­
        public Quaternion Normalize()
        {
            double magnitude = Magnitude();
            return new Quaternion(w / magnitude, x / magnitude, y / magnitude, z / magnitude);
        }

        // ÄõÅÍ´Ï¾ðÀÇ ÄÓ·¹
        public Quaternion Conjugate()
        {
            return new Quaternion(w , - x, -y, -z);
        }

        // ÄõÅÍ´Ï¾ð°ú ÄõÅÍ´Ï¾ðÀÇ ³»Àû
        public static double Dot(Quaternion q1, Quaternion q2)
        {
            return q1.x * q2.x + q1.y * q2.y + q1.z * q2.z + q1.w * q2.w;
        }

        // ÄõÅÍ´Ï¾ð°ú º¤ÅÍÀÇ È¸Àü
        public static Vector3 RotateVector(Quaternion q, Vector3 v)
        {
            double q0 = q.w;
            double q1 = q.x;
            double q2 = q.y;
            double q3 = q.z;

            double v1 = v.x;
            double v2 = v.y;
            double v3 = v.z;

            // Calculate the rotation matrix elements
            double[,] M = new double[3, 3];

            M[0, 0] = q0 * q0 + q1 * q1 - q2 * q2 - q3 * q3;
            M[0, 1] = 2 * (q1 * q2 - q0 * q3);
            M[0, 2] = 2 * (q0 * q2 + q1 * q3);

            M[1, 0] = 2 * (q1 * q2 + q0 * q3);
            M[1, 1] = q0 * q0 - q1 * q1 + q2 * q2 - q3 * q3;
            M[1, 2] = 2 * (q2 * q3 - q0 * q1);

            M[2, 0] = 2 * (q1 * q3 - q0 * q2);
            M[2, 1] = 2 * (q0 * q1 + q2 * q3);
            M[2, 2] = q0 * q0 - q1 * q1 - q2 * q2 + q3 * q3;

            // Apply the rotation matrix to the vector
            Vector3 rotatedVector = new Vector3(
                M[0, 0] * v1 + M[0, 1] * v2 + M[0, 2] * v3,
                M[1, 0] * v1 + M[1, 1] * v2 + M[1, 2] * v3,
                M[2, 0] * v1 + M[2, 1] * v2 + M[2, 2] * v3
            );

            return rotatedVector;
        }

        // ÄõÅÍ´Ï¾ð°ú 4x4 Çà·ÄÀÇ °ö¼À
        public static Quaternion operator *(Matrix4x4 m, Quaternion q)
        {
            double[] v = { q.w, q.x, q.y, q.z };

            double[] result = new double[4];
            for (int i = 0; i < 4; i++)
            {
                result[i] = 0;
                for (int j = 0; j < 4; j++)
                {
                    result[i] += m[i, j] * v[j];
                }
            }

            return new Quaternion(result[0], result[1], result[2], result[3]);
        }
        public static Quaternion FromEulerAngles_RAD(double roll, double pitch, double yaw)
        {
            double cy = Math.Cos(yaw * 0.5);
            double sy = Math.Sin(yaw * 0.5);
            double cp = Math.Cos(pitch * 0.5);
            double sp = Math.Sin(pitch * 0.5);
            double cr = Math.Cos(roll * 0.5);
            double sr = Math.Sin(roll * 0.5);

            double w = cr * cp * cy + sr * sp * sy;
            double x = sr * cp * cy - cr * sp * sy;
            double y = cr * sp * cy + sr * cp * sy;
            double z = cr * cp * sy - sr * sp * cy;

            return new Quaternion(w, x, y, z);
        }
        public static Quaternion FromEulerAngles_DEG(Vector3 v)
        {
            return FromEulerAngles_RAD(v.x * UnityEngine.Mathf.Deg2Rad, v.y * UnityEngine.Mathf.Deg2Rad, v.z * UnityEngine.Mathf.Deg2Rad);
        }
        public static Quaternion FromEulerAngles_DEG(double roll, double pitch, double yaw)
        {
            return FromEulerAngles_RAD(roll * UnityEngine.Mathf.Deg2Rad, pitch * UnityEngine.Mathf.Deg2Rad, yaw * UnityEngine.Mathf.Deg2Rad);
        }
        public static Quaternion FromEulerAngles_RAD(Vector3 v)
        {
            return FromEulerAngles_RAD(v.x, v.y, v.z);
        }
        public Vector3 ToEulerAngles_RAD()
        {
            double sinr_cosp = 2 * (w * x + y * z);
            double cosr_cosp = 1 - 2 * (x * x + y * y);
            double roll = Math.Atan2(sinr_cosp, cosr_cosp);

            double sinp = 2 * (w * y - z * x);
            double pitch = Math.Abs(sinp) >= 1 ? Math.PI / 2 * Math.Sign(sinp) : Math.Asin(sinp);

            double siny_cosp = 2 * (w * z + x * y);
            double cosy_cosp = 1 - 2 * (y * y + z * z);
            double yaw = Math.Atan2(siny_cosp, cosy_cosp);

            return new Vector3(roll, pitch, yaw);
        }
        public Vector3 ToEulerAngles_DEG()
        {
            return ToEulerAngles_RAD() * Mathf.Rad2Deg;
        }
        public static Quaternion Lerp(Quaternion q1, Quaternion q2, double t)
        {
            // Ensure the quaternions are normalized
            q1.Normalize();
            q2.Normalize();

            Vector3 e1 = q1.ToEulerAngles_DEG();
            Vector3 e2 = q2.ToEulerAngles_DEG();

            return Quaternion.FromEulerAngles_DEG(t * e1 + (1 - t) * e2);

        }
        public static Quaternion Slerp(Quaternion q1, Quaternion q2, double t)
        {
            // Ensure the quaternions are normalized
            q1.Normalize();
            q2.Normalize();

            double dot = Dot(q1, q2);

            if (dot < 0)
            {
                q1 = -1 * q1;
                dot = -dot;
            }

            const double threshold = 0.9995;
            if (dot > threshold)
            {
                Quaternion result = q1 + (q2 - q1) * t;
                result.Normalize();
                return result;
            }

            double theta_0 = Math.Acos(dot);
            double theta = theta_0 * t;
            double sin_theta = Math.Sin(theta);
            double sin_theta_0 = Math.Sin(theta_0);

            double s0 = Math.Cos(theta) - dot * sin_theta / sin_theta_0;
            double s1 = sin_theta / sin_theta_0;

            return (q1 * s0) + (q2 * s1);
        }
        public override string ToString()
        {
            return $"({w}, {x}, {y}, {z})";
        }
    }
}