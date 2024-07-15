using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System;

public class Connector : MonoBehaviour
{
    SerialPort serialPort;
    string portName = "COM7"; // 포트 이름
    int baudRate = 115200; // 보드레이트

    Vector3 ConvertVector(Vector3 v, Vector3 preV)
    {
        Vector3 result = v;
        Vector3 diff = v - preV;
        if(Math.Abs(diff.x) < 2.1f)
        {
            result.x = preV.x;
        }
        if (Math.Abs(diff.y) < 2.1f)
        {
            result.y = preV.y;
        }
        if (Math.Abs(diff.z) < 2.1f)
        {
            result.z = preV.z;
        }
        return result;
    }

    void Start()
    {
        // 아두이노와의 시리얼 통신을 위한 포트 초기화
        serialPort = new SerialPort(portName, baudRate);
        serialPort.Open(); // 포트 열기
        serialPort.ReadTimeout = 8000;
        //serialPort.time
        serialPort.RtsEnable = true; //안하면 안됨!!
        serialPort.DtrEnable = true;
        StartCoroutine(ReadSerialData()); // 시리얼 데이터를 읽는 코루틴 시작
        preAngle = new Vector3[10];
        baseGyro = new Vector3[10];
        preGyro = new Vector3[10];
        preGyroAngle = new Vector3[10];

        for (int i = 0; i < 10; i++)
        {
            preAngle[i] = Vector3.zero;
            baseGyro[i] = Vector3.zero;
            preGyro[i] = Vector3.zero;
            preGyroAngle[i] = Vector3.zero;
        }
        RHms = RH.localEulerAngles;
        RLAms = RLA.localEulerAngles;
        RAAms = RAA.localEulerAngles;
    }
    [SerializeField] Transform Target;
    [SerializeField] Transform RH;
    [SerializeField] Transform RLA;
    [SerializeField] Transform RAA;

    [SerializeField] Vector3 RHs;
    [SerializeField] Vector3 RLAs;
    [SerializeField] Vector3 RAAs;
    [SerializeField] Vector3 RHms;
    [SerializeField] Vector3 RLAms;
    [SerializeField] Vector3 RAAms;
    [SerializeField] bool[] StartFlag = new bool[3];
    [SerializeField] bool f = false;

    Vector3 preHand;
    Vector3 preRLA;
    Vector3 preRAA;

    private void ReInit()
    {
        for(int i = 0; i < StartFlag.Length; i++)
        {
            StartFlag[i] = false;
        }


    }

    IEnumerator ReadSerialData()
    {
        bool isDeviceReady = false;
        while (true)
        {
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
            Debug.Log(serialData);
                if (serialData.Contains("OK") && f == false)
                {
                    f = true;
                }

                if (f == false)
                {

                    yield return null;
                    continue;
                }
            }
            finally
            {
                
            }
                //if (isDeviceReady == false)
                //{
                //    if (serialData.Contains("OK\n"))
                //    {
                //        isDeviceReady = true;
                //        Debug.Log("Received 'OK' from Arduino. Proceeding...");
                //    }
                //    yield return null;
                //}

                //    Debug.Log(serialData);
                //try
                //{
                //    string[] datas = serialData.Split(',');
                //    //Vector3 v = new Vector3();
                //    //v = new Vector3(
                //    //    float.Parse(datas[1]),
                //    //    float.Parse(datas[0]),
                //    //    -float.Parse(datas[2]));
                //    Quaternion q = new Quaternion(
                //        -float.Parse(datas[2]),
                //        -float.Parse(datas[3]),
                //        float.Parse(datas[1]),
                //        float.Parse(datas[0]));
                //    Target.rotation = q;
                //    //Target.eulerAngles = v;

                //}
                //catch
                //{

                //}


                string[] datas = serialData.Split('#');
            if (datas.Length >= 3)
            {
            for (int i = 0; i < 3; i++) 
            {
                try
                {
                    string[] data = datas[i].Split(',');
                        Quaternion q = new Quaternion(
                            -float.Parse(data[4]),
                            -float.Parse(data[3]),
                            float.Parse(data[2]),
                            float.Parse(data[1]));
                        int tag = int.Parse(data[0]);
                        Vector3 e = new Quaternion(
        float.Parse(data[2]),
        float.Parse(data[3]),
        float.Parse(data[4]),
        float.Parse(data[1])).eulerAngles;
                        e.x *= -1;
                        //int tag = int.Parse(data[0]);
                        if (tag == 1)
                        {
                            if (StartFlag[i] == false)
                            {
                                RHs = e;
                                StartFlag[i] = true;
                                continue;
                            }
                            RH.localEulerAngles = (ConvertVector(e, preHand) - RHs) + RHms;
                            preHand = e;
                            //RH.rotation = q * Quaternion.Inverse(RHs) * RHms;
                        }
                        else if (tag == 2)
                        {
                            if (StartFlag[i] == false)
                            {
                                RLAs = e;
                                StartFlag[i] = true;
                                continue;
                            }
                            RLA.localEulerAngles = (ConvertVector(e, preRLA) - RLAs) + RLAms;
                            preRLA = e;
                            //RLA.rotation = q * Quaternion.Inverse(RLAs) * RLAms;
                        }
                        else if (tag == 3)
                        {
                            if (StartFlag[i] == false)
                            {
                                RAAs = e;
                                StartFlag[i] = true;
                                continue;
                            }
                            RAA.localEulerAngles = (ConvertVector(e, preRAA) - RAAs) + RAAms;
                            preRAA = e;
                            //RAA.rotation = q * Quaternion.Inverse(RAAs) * RAAms;
                        }
                        //Cube[i].transform.eulerAngles = v;
                    }
                catch
                {
                    Debug.Log(i);
                        //serialPort.
                }
                //try
                //{
                //    float dt = Time.deltaTime;
                //    string[] data = datas[i].Split(',');
                //    int ax = int.Parse(data[1]);
                //    int ay = int.Parse(data[2]);
                //    int az = int.Parse(data[3]);
                //    int gx = int.Parse(data[4]);
                //    int gy = int.Parse(data[5]);
                //    int gz = int.Parse(data[6]);
                //    Vector3 nowAngle = Cube[i].transform.eulerAngles;
                //    // Convert gyro values to degrees/sec
                //    float FS_SEL = 131;
                //    Vector3 raw_gyro = new Vector3(gx, gy, gz);
                //    Vector3 gyro = (raw_gyro - baseGyro[i]) / FS_SEL;

                //    // Compute the (filtered) gyro angles
                //    Vector3 gyroAngle = gyro * dt + preAngle[i];

                //    // Compute the drifting gyro angles
                //    //Vector3 unfiltered_gyro = gyro * dt + preGyroAngle[i];

                //    // Get raw acceleration values
                //    //float G_CONVERT = 16384;
                //    Vector3 accel = new Vector3(ax, ay, az);

                //    // Get angle values from accelerometer
                //    // float accel_vector_length = sqrt(pow(accel_x,2) + pow(accel_y,2) + pow(accel_z,2));
                //    //float accel_angle_y = Mathf.Atan(-1 * accel.x / Mathf.Sqrt(Mathf.Pow(accel.y, 2) + Mathf.Pow(accel.z, 2))) * Mathf.Rad2Deg;
                //    //float accel_angle_x = Mathf.Atan(accel.y / Mathf.Sqrt(Mathf.Pow(accel.x, 2) + Mathf.Pow(accel.z, 2))) * Mathf.Rad2Deg;
                //    //float accel_angle_z = 0;

                //    float alpha = 0.96f;
                //    float angle_x = alpha * gyroAngle.x + (1.0f - alpha) * accel.x;
                //    float angle_y = alpha * gyroAngle.y + (1.0f - alpha) * accel.y;
                //    float angle_z = gyroAngle.z; // Accelerometer doesn't give z-angle


                //    Cube[i].transform.eulerAngles = new Vector3(angle_x, angle_y, angle_z) * M;
                //    Cube[i].transform.eulerAngles = accel * M / 16384f;
                //    preGyro[i] = gyro;
                //    preGyroAngle[i] = gyroAngle;
                //    preAngle[i] = Cube[i].transform.eulerAngles;


                //    //Debug.Log(Mathf.Deg2Rad * new Vector3(x, z, y));
                //}
                //catch
                //{

                //}
            }

            }
            //Debug.Log(datas.Length);


            yield return null;
        }
    }
    Vector3[] preAngle;
    Vector3[] baseGyro;
    Vector3[] preGyro;
    Vector3[] preGyroAngle;
    [SerializeField] float M;
    [SerializeField] GameObject[] Cube;
    void OnApplicationQuit()
    {
        // 어플리케이션이 종료될 때 포트 닫기
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            ReInit();
    }
}
