using AHRS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;

public class SerialDataReader : MonoBehaviour
{
    SerialPort serialPort;
    [SerializeField] string portName = "COM7"; // 포트 이름
    [SerializeField] int baudRate = 115200; // 보드레이트
    [SerializeField] SensorController SensorMaster;
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
        StartCoroutine(ReadSerialData()); // 시리얼 데이터를 읽는 코루틴 시작
    }

    IEnumerator ReadSerialData()
    {
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                //SensorMaster.ResetSensors();
            }

            string serialData = string.Empty;
            if (serialPort.IsOpen)
            {
                try
                {
                    // 시리얼 데이터 읽기
                    serialData = serialPort.ReadLine();
                    Debug.Log(serialData);
                }
                catch (System.Exception e)
                {
                    Debug.LogError("Error reading serial data: " + e.Message);
                }

                try
                {
                    string[] dataForSensor = serialData.Split('#');
                    if (int.TryParse(dataForSensor[0], out int nowTime))
                        {
                        float dt = (nowTime - preTime) / 1000f;
                        preTime = nowTime;
                        for (int i = 1; i < dataForSensor.Length; i++)
                        {
                            //Debug.Log(dataForSensor[i].Length);
                            //if (dataForSensor[i].Length != 11)
                            //    continue;
                            //Debug.Log(dataForSensor[i]);
                            //SensorMaster.SetSensorValueQuat(dataForSensor[i]);
                            SensorMaster.SetSensorValue(dataForSensor[i], dt);
                        }
                    }
                    else
                    {
                    }

                }
                catch (System.FormatException e)
                {
                    Debug.Log("Error reading serial data: " + e.Message);
                }
            }


            yield return null;
        }
    }
    int preTime = 0;
    void OnApplicationQuit()
    {
        // 어플리케이션이 종료될 때 포트 닫기
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
        }
    }
}
