using System.Collections;
using System.Collections.Generic;
//using UnityEngine;
using AHRS;

namespace AHRS
{
    public class Gyro
    {
public static Quaternion Update_Trigonometric(Quaternion q, Vector3 w, double dt)
{
    if (w.Magnitude() < 0.000001)
        return q;
    Vector3 u = w / w.Magnitude();
    double w_m = w.Magnitude();
    Quaternion dq = new Quaternion(System.Math.Cos(w_m * dt / 2), System.Math.Sin(w_m * dt / 2) * u.x, System.Math.Sin(w_m * dt / 2) * u.y, System.Math.Sin(w_m * dt / 2) * u.z);
    return q * dq;
}
        public static Vector3 Update_Integration(Vector3 v, Vector3 w, double dt)
        {
            return v + (w * dt);
        }
        public static Quaternion Update_Derivative(Quaternion q, Vector3 w, double dt)
        {
            Matrix4x4 omega = Omega(w);
            Quaternion result = (Matrix4x4.Identity + (0.5f * dt * omega)) * q;
            return result;
        }
        public static Quaternion Get_Derivative(Quaternion q, Vector3 w, double dt)
        {
            Matrix4x4 omega = Omega(w);
            Quaternion result = ((0.5f * dt * omega)) * q;
            return result;
        }
        public static Matrix4x4 Omega(Vector3 w)
        {
            return new Matrix4x4(new double[,]
            {
            {0, -w.x, -w.y, -w.z },
            {w.x, 0, w.z, -w.y },
            {w.y, -w.z, 0, w.x },
            {w.z, w.y, -w.x, 0 }
            });
        }
    }
}