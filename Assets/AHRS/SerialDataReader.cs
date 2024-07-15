using AHRS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;

public class SerialDataReader : MonoBehaviour
{
    SerialPort serialPort;
    [SerializeField] string portName = "COM7"; // ��Ʈ �̸�
    [SerializeField] int baudRate = 115200; // ���巹��Ʈ
    [SerializeField] SensorController SensorMaster;
    // Start is called before the first frame update
    void Start()
    {
        // �Ƶ��̳���� �ø��� ����� ���� ��Ʈ �ʱ�ȭ
        serialPort = new SerialPort(portName, baudRate);
        serialPort.Open(); // ��Ʈ ����
        serialPort.ReadTimeout = 5000;

        //serialPort.time
        serialPort.RtsEnable = true; //���ϸ� �ȵ�!!
        serialPort.DtrEnable = true;
        StartCoroutine(ReadSerialData()); // �ø��� �����͸� �д� �ڷ�ƾ ����
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
                    // �ø��� ������ �б�
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
        // ���ø����̼��� ����� �� ��Ʈ �ݱ�
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
        }
    }
}
