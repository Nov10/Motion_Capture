using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;
using System;
using static UnityEngine.GraphicsBuffer;

public class SensorVS : MonoBehaviour
{
    SerialPort serialPort;
    [SerializeField] string portName = "COM7"; // 포트 이름
    [SerializeField] int baudRate = 115200; // 보드레이트
    [SerializeField] Transform TargetObject1;
    [SerializeField] Transform TargetObject2;
    Quaternion start1;
    Quaternion start2;

    Quaternion sStart1;
    Quaternion sStart2;
    // Start is called before the first frame update
    void Start()
    {
        // 아두이노와의 시리얼 통신을 위한 포트 초기화
        serialPort = new SerialPort(portName, baudRate);
        serialPort.Open(); // 포트 열기
        serialPort.ReadTimeout = 5000;
        //serialPort.time
        serialPort.RtsEnable = true; //안하면 안됨!!
        serialPort.DtrEnable = true;
        start1 = TargetObject1.rotation;
        start2 = TargetObject2.rotation;
        StartCoroutine(ReadSerialData()); // 시리얼 데이터를 읽는 코루틴 시작
    }

    Vector3 startEuler = Vector3.zero;
    float preT;
    Vector3 preG;
    Quaternion target1 = Quaternion.identity;
    Quaternion target2 = Quaternion.identity;

    AHRS.Vector3 startGyro = new AHRS.Vector3(0,0,0);

    AHRS.KalmanFilter KalmanFilterX = new AHRS.KalmanFilter(0.002, 0.004, 0.02);
    AHRS.KalmanFilter KalmanFilterY = new AHRS.KalmanFilter(0.002, 0.004, 0.02);


    AHRS.KalmanFilter KalmanFilterQX = new AHRS.KalmanFilter(0.002, 0.004, 0.02);
    AHRS.KalmanFilter KalmanFilterQY = new AHRS.KalmanFilter(0.002, 0.004, 0.02);
    AHRS.KalmanFilter KalmanFilterQZ = new AHRS.KalmanFilter(0.002, 0.004, 0.02);
    AHRS.KalmanFilter KalmanFilterQW = new AHRS.KalmanFilter(0.002, 0.004, 0.02);

    IEnumerator ReadSerialData()
    {
        bool isDeviceReady = false;
        bool startFlag = false;
        while (true)
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                startFlag = false;
            }

            string serialData = string.Empty;
            if (serialPort.IsOpen)
            {
                try
                {
                    // 시리얼 데이터 읽기
                    serialData = serialPort.ReadLine();
                }
                catch (System.Exception e)
                {
                    Debug.LogError("Error reading serial data: " + e.Message);
                }
            }



            try
            {
                string[] data = serialData.Split(',');
                float k = 1;
                float gx = float.Parse(data[0]) / 131 / k;
                float gy = float.Parse(data[1]) / 131 / k;
                float gz = float.Parse(data[2]) / 131 / k;
                float ax = float.Parse(data[3]) / k;
                float ay = float.Parse(data[4]) / k;
                float az = float.Parse(data[5]) / k;
                float mx = float.Parse(data[6]) /  k;
                float my = float.Parse(data[7]) / k;
                float mz = float.Parse(data[8]) / k;
                float dt = float.Parse(data[9]);
                //float dt = 0.02f;
                AHRS.Quaternion q10 = new AHRS.Quaternion(target1.w, target1.x, target1.y, target1.z);
                AHRS.Quaternion q20 = new AHRS.Quaternion(target2.w, target2.x, target2.y, target2.z);

                AHRS.Vector3 accelerometer =new AHRS.Vector3( ax, ay, az ); // Example accelerometer data
                AHRS.Vector3 gyroscope = new AHRS.Vector3(gx, gy, gz); // Example gyroscope data
                AHRS.Vector3 magnetometer = new AHRS.Vector3(mx, my, mz ); // Example magnetometer data
                //AHRS.Quaternion q_g = AHRS.Quaternion.FromEulerAngles(gyroscope);
                // Apply Madgwick filter using IMU data only
                //q = MadgwickFilter.Madgwick_IMU(accelerometer, gyroscope, q, beta, samplePeriod);
                // AHRS.Vector3 v1 = AHRS.ComplementaryFilter.Step(v10, gyroscope, accelerometer, magnetometer, dt, 0.9f);// * Mathf.Rad2Deg;
                accelerometer = accelerometer.Normalize();

                AHRS.Quaternion q1 = AHRS.ComplementaryFilter.Update_GyroAccelMag(q10, gyroscope, accelerometer, magnetometer, dt, 0.97f);
                AHRS.Quaternion q2 = AHRS.
                    Gyro.Update_Trigonometric(q20, gyroscope, dt);

                // Apply Madgwick filter using IMU and Magnetometer data
                //var v = MadgwickFilter.Madgwick_IMU_Magnetometer(accelerometer, gyroscope, magnetometer, q, beta, 1/dt);

                double accXangle = (Math.Atan2(accelerometer.y, accelerometer.z)) * UnityEngine.Mathf.Rad2Deg;
                double accYangle = (Math.Atan2(-accelerometer.x, Math.Sqrt(accelerometer.z * accelerometer.z + accelerometer.y * accelerometer.y))) *UnityEngine.Mathf.Rad2Deg;
                if(startFlag == false)
                {
                    //KalmanFilterX.setAngle(accXangle);
                    //KalmanFilterY.setAngle(accYangle);
                }
                AHRS.Quaternion tmp = AHRS.Quaternion.FromEulerAngles_RAD(accXangle, accYangle, q2.ToEulerAngles_RAD().z);

                //double qx = KalmanFilterQX.Update(tmp.x, q_g.x, dt);
                //double qy = KalmanFilterQY.Update(tmp.y, q_g.y, dt);
                //double qz = KalmanFilterQZ.Update(tmp.z, q_g.z, dt);
                //double qw = KalmanFilterQW.Update(tmp.w, q_g.w, dt);

                double k_x = KalmanFilterX.Update(accXangle, gyroscope.x, dt) * Mathf.Deg2Rad;
                double k_y = KalmanFilterY.Update(accYangle, gyroscope.y, dt) * Mathf.Deg2Rad;

                //q1 = AHRS.Quaternion.FromEulerAngles(k_x, k_y, q2.ToEulerAngles().z);
                //q1 = new AHRS.Quaternion(qw, qx, qy, qz);

                target1 = new UnityEngine.Quaternion((float)q1.x, (float)q1.y, (float)q1.z, (float)q1.w);
                //target1 = UnityEngine.Quaternion.Euler((float)v1.x, (float)v1.y, (float)v1.z).normalized;
                target2 = new UnityEngine.Quaternion((float)q2.x, (float)q2.y, (float)q2.z, (float)q2.w);
                //Debug.Log(target);
                if (startFlag == false)
                {
                    startFlag = true;
                    sStart1 = target1;
                    sStart2 = target2;
                    startGyro = gyroscope;
                }

                TargetObject1.rotation = start1 * target1 * Quaternion.Inverse(sStart1);
                TargetObject2.rotation = start2 * target2 * Quaternion.Inverse(sStart2);
            }
            catch(Exception e)
            {
                Debug.Log(e);
                //serialPort.
            }

            preT = Time.time;
            yield return null;
        }
    }
    void OnApplicationQuit()
    {
        // 어플리케이션이 종료될 때 포트 닫기
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
        }
    }
}
