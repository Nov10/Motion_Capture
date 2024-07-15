using System;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

namespace AHRS
{
    public class ComplementaryFilter
    {
        public static Vector3 Step(Vector3 r, Vector3 gyro, Vector3 accel, Vector3 mag, float dt, float alpha)
        {
            Vector3 r_gyro = Gyro.Update_Integration(r, gyro, dt);
            double accXangle = Math.Atan2(accel.y, Math.Sqrt(accel.x * accel.x + accel.z * accel.z));
            double accYangle = Math.Atan2(accel.x, Math.Sqrt(accel.y * accel.y + accel.z * accel.z));

            Vector3 r0 = new Vector3(accXangle, accYangle, r_gyro.z);



            Vector3 result = ((1 - alpha) * r0) + (alpha * r_gyro);
            return result;
        }
        static Vector3 preAccel = new Vector3(0,0,0);
        public static Quaternion Update_GyroAccelMag(Quaternion r, Vector3 gyro, Vector3 accel, Vector3 mag, double dt, double alpha)
        {
            //if ((preAccel - accel).Magnitude() > 0.9)
                //alpha = 1;
            Vector3 origiAccel = new Vector3(accel.x, accel.y, accel.z);
            accel = accel.Normalize();
            //Quaternion q_acc;
            //if(accel.z >= 0)
            //{
            //    q_acc = new Quaternion(
            //        Math.Sqrt((accel.z + 1) / 2),
            //        -accel.y / Math.Sqrt(2 * (accel.z + 1)),
            //        accel.x / Math.Sqrt(2 * (accel.z + 1)),
            //        0);
            //}
            //else
            //{
            //    q_acc = new Quaternion(
            //        -accel.y / Math.Sqrt(2 * (1-accel.z)),
            //        Math.Sqrt((1-accel.z)/2),
            //        0,
            //        accel.x / Math.Sqrt(2 * (1-accel.z))
            //        );
            //}
            //return q_acc;
            //Vector3 r_gyro = r.ToEulerAngles() * UnityEngine.Mathf.Rad2Deg + (gyro * dt);
            Quaternion q_gyro = Gyro.Update_Derivative(r, gyro, dt);

            double accXangle = (Math.Atan2(accel.y, accel.z));
            double accYangle = (Math.Atan2(-accel.x, Math.Sqrt(accel.z * accel.z + accel.y * accel.y)));
            double x = accXangle;
            double y = accYangle;
            var m = mag;
            double anlgeZ = Math.Atan2(m.z * Math.Sin(y) - m.y * Math.Cos(y), m.x * Math.Cos(x) + Math.Sin(x) * (m.y * Math.Sin(y) + m.z * Math.Cos(y)));

            Quaternion q_acc = Quaternion.FromEulerAngles_RAD(accXangle, accYangle, anlgeZ);

            return q_gyro * alpha + (1 - alpha) * q_acc; //Lerp
        }
        static Vector3 preV = new Vector3(0, 0, 0);
public static Quaternion Update_GyroAccel(Quaternion r, Vector3 gyro, Vector3 accel, double dt, double alpha)
{
    accel = accel.Normalize();

    //자이로 센서의 값으로부터 각 계산
    Quaternion q_gyro = Gyro.Update_Derivative(r, gyro, dt);

    //가속도 센서의 값으로부터 각 계산
    double accXangle = Math.Atan2(accel.y, Math.Sqrt(accel.x * accel.x + accel.z * accel.z));
    double accYangle = Math.Atan2(accel.x, Math.Sqrt(accel.y * accel.y + accel.z * accel.z));

    //센서가 뒤집히면 자이로 값으로 반환
    if (accel.z < 0) {
        return q_gyro;
    }
    accXangle = accXangle * Mathf.Rad2Deg;
    accYangle = accYangle * Mathf.Rad2Deg;

    Quaternion q_acc = Quaternion.FromEulerAngles_DEG(accXangle, accYangle, q_gyro.ToEulerAngles_DEG().z);

    // 상보 필터 적용
    return Quaternion.Slerp(q_gyro, q_acc, alpha);
}
            /*
        {
            //if ((preAccel - accel).Magnitude() > 0.9)
            //alpha = 1;
            Vector3 r_q = r.ToEulerAngles_DEG();
            Vector3 origiAccel = new Vector3(accel.x, accel.y, accel.z);
            accel = accel.Normalize();

            //Quaternion q_acc;
            //if(accel.z >= 0)
            //{
            //    q_acc = new Quaternion(
            //        Math.Sqrt((accel.z + 1) / 2),
            //        -accel.y / Math.Sqrt(2 * (accel.z + 1)),
            //        accel.x / Math.Sqrt(2 * (accel.z + 1)),
            //        0);
            //}
            //else
            //{
            //    q_acc = new Quaternion(
            //        -accel.y / Math.Sqrt(2 * (1-accel.z)),
            //        Math.Sqrt((1-accel.z)/2),
            //        0,
            //        accel.x / Math.Sqrt(2 * (1-accel.z))
            //        );
            //}
            //return q_acc;
            //Vector3 r_gyro = r.ToEulerAngles() * UnityEngine.Mathf.Rad2Deg + (gyro * dt);
            //Quaternion q_gyro = Gyro.Update_Derivative(r, gyro, dt);

            //Vector3 v_gyro = Gyro.Update_Derivative(r, gyro, dt).ToEulerAngles_DEG();
            Quaternion q_gyro = Gyro.Update_Derivative(r, gyro, dt);
            double accXangle = Math.Atan2(accel.y, Math.Sqrt(accel.z * accel.z + accel.x * accel.x));
            double accYangle = Math.Atan2(-accel.x, Math.Sqrt(accel.z * accel.z + accel.y * accel.y));
            //accXangle = Math.Atan(accel.y / accel.z) * Mathf.Rad2Deg;
            //accYangle = Math.Asin(accel.x / 9.81f) * Mathf.Rad2Deg;

            if (accel.z > 0)
            {
                accYangle = (accYangle > 0) ? (180 - accYangle) : (-180 - accYangle);
                accXangle = -accXangle;
            }

            //if (accel.z < 0)
            //{
            //    alpha = 1;
            //}
            //else
            //{
            //    //Vector3 t = new Vector3(accXangle, accYangle, 0);
            //    //t = preV * 0.95 + t * 0.05;
            //    //accXangle = t.x;
            //    //accYangle = t.y;
            //    //preV = t;
            //}

                accXangle = accXangle * Mathf.Rad2Deg;
                accYangle = accYangle * Mathf.Rad2Deg;
            //Vector3 v_acc = new Vector3(accXangle, accYangle, v_gyro.z);
            Quaternion q_acc = Quaternion.FromEulerAngles_DEG(accXangle, accYangle, q_gyro.ToEulerAngles_DEG().z);

            return (q_gyro * alpha + (1-alpha)* q_acc); //Lerp
        }
            */
    }
}