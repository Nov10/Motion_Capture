using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System;

public class SingleSensor : MonoBehaviour
{
    SerialPort serialPort;
    [SerializeField] string portName = "COM7"; // ��Ʈ �̸�
    [SerializeField] int baudRate = 115200; // ���巹��Ʈ
    [SerializeField] Transform TargetObject;
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
        startEuler = TargetObject.eulerAngles;
        StartCoroutine(ReadSerialData()); // �ø��� �����͸� �д� �ڷ�ƾ ����
    }

    Vector3 startEuler = Vector3.zero;

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
                    // �ø��� ������ �б�
                    serialData = serialPort.ReadLine();
                }
                catch (System.Exception e)
                {
                    Debug.LogError("Error reading serial data: " + e.Message);
                }
            }

            try
            {
                //Debug.Log(serialData);
                //if (serialData.Contains("OK") && isDeviceReady == false)
                //{
                //    isDeviceReady = true;
                //}

                //if (isDeviceReady == false)
                //{
                //    yield return null;
                //    continue;
                //}
            }
            finally
            {

            }



            try
            {
                string[] data = serialData.Split(',');
                //Quaternion q = new Quaternion(
                //    float.Parse(data[1]),
                //    float.Parse(data[2]),
                //    float.Parse(data[3]),
                //    float.Parse(data[0]));

                //Vector3 rot = q.eulerAngles;
                //float y = rot.y;
                //float z = rot.z;
                //rot.y = -z;
                //rot.z = y;
                float[] f_datas = new float[data.Length];
                for(int i = 0; i < data.Length; i++)
                {
                    f_datas[i] = float.Parse(data[i]);
                }
                Vector3 rot = new Vector3(f_datas[2], f_datas[0], f_datas[1]);

                TargetObject.eulerAngles = startEuler + rot;
            }
            catch
            {
                //serialPort.
            }


            yield return null;
        }
    }
    void OnApplicationQuit()
    {
        // ���ø����̼��� ����� �� ��Ʈ �ݱ�
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
        }
    }
}
