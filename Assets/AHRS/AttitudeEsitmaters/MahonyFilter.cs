using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;

namespace AHRS
{
    public class MahonyFilter
    {
            Vector3 ix = new Vector3(), iy = new Vector3(), iz = new Vector3();
public Quaternion Update_GyroAccel(Quaternion quac, Vector3 gyro, Vector3 accel, float dt, float Kp = 2.0f, float Ki = 0.1f)
{
    Vector3 scaled_gyro = new Vector3(gyro.x, gyro.y, gyro.z);

    if (accel.Magnitude() > 0.0001)
    {
        accel = accel.Normalize();

        Vector3 estimated_gravity = Quaternion.RotateVector(quac.Conjugate(), new Vector3(0, 0, 1));
        Vector3 error = Vector3.Cross(estimated_gravity, accel);

        if (Ki > 0.0f) {
            ix = ix + (Ki * dt * error);
            scaled_gyro = gyro + ix;
        }

        scaled_gyro += Kp * error;
    }

    quac = Gyro.Update_Trigonometric(quac, gyro, dt);

    quac = quac.Normalize();

    return quac;
}
    }
}