using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Security.Cryptography;
using UnityEngine;

[System.Serializable]
public class FingerSensor
{
    public Transform Model;
    public Quaternion StartRotation;
    public int Max;
    public int Min;
    public bool MinMaxInitialized;

    public void Initialize()
    {
        StartRotation = Model.localRotation;
    }

    public void SetMax(int max)
    { Max = max; }
    public void SetMin(int min)
    {
        Min = min;
    }

    public float GetNormalizedValue(int value)
    {
        return 1 - (float)(value - Min) / (float)(Max - Min);
    }
}
public class Hand : MonoBehaviour
{
    SerialPort serialPort;
    [SerializeField] string portName = "COM7"; // 포트 이름
    [SerializeField] int baudRate = 115200; // 보드레이트

    [SerializeField] FingerSensor R0_0;
    [SerializeField] FingerSensor R1_0;
    [SerializeField] FingerSensor R2_0;
    [SerializeField] FingerSensor R3_0;
    [SerializeField] FingerSensor R4_0;
    [SerializeField] bool[] StartFlag = new bool[4];

    // Start is called before the first frame update
    void Start()
    {
        // 아두이노와의 시리얼 통신을 위한 포트 초기화
        serialPort = new SerialPort(portName, baudRate);
        serialPort.Handshake = Handshake.None;
        //serialPort.DataReceived += new SerialDataReceivedEventHandler(sp_DataReceived);
        serialPort.Open(); // 포트 열기
        serialPort.ReadTimeout = 8000;
        //serialPort.time
        serialPort.RtsEnable = true; //안하면 안됨!!
        serialPort.DtrEnable = true;
        R0_0.Initialize();
        R1_0.Initialize();
        R2_0.Initialize();
        R3_0.Initialize();
        R4_0.Initialize();
        StartCoroutine(ReadSerialData()); // 시리얼 데이터를 읽는 코루틴 시작
    }
    [SerializeField] bool WorkFlag = false;
    IEnumerator ReadSerialData()
    {
        string serialData = string.Empty;
        bool isDeviceReady = false;
        while (true)
        {
            if (serialPort.IsOpen)
            {
                serialData = serialPort.ReadLine();
                //if (serialData.Contains("OK") && isDeviceReady == false)
                {
                    isDeviceReady = true;
                    break;
                }
            }
        }

        yield return new WaitForSeconds(0.1f);
        Debug.Log("손 펴!");
        yield return new WaitForSeconds(1);
        float elapsedTime = 0;
        float targetTime = 3;
        while (true)
        {
            elapsedTime += Time.deltaTime;
            serialData = serialPort.ReadLine();
            string[] datas = serialData.Split('#');

            for (int i = 0; i < datas.Length; i++)
            {
                int value = int.Parse(datas[i]);
                switch (i + 1)
                {
                    case 0:
                        R0_0.SetMax(value);
                        break;
                    case 1:
                        R1_0.SetMax(value);
                        break;
                    case 2:
                        R2_0.SetMax(value);
                        break;
                    case 3:
                        R3_0.SetMax(value);
                        break;
                    case 4:
                        R4_0.SetMax(value);
                        break;
                }
            }
            if (targetTime <= elapsedTime)
            {
                break;
            }
            yield return null;
        }


        yield return new WaitForSeconds(0.1f);
        Debug.Log("손 주먹!");
        yield return new WaitForSeconds(1);
        elapsedTime = 0;
        targetTime = 3;
        while (true)
        {
            elapsedTime += Time.deltaTime;
            serialData = serialPort.ReadLine();
            string[] datas = serialData.Split('#');
            for (int i = 0; i < datas.Length; i++)
            {
                int value = int.Parse(datas[i]);
                switch (i + 1)
                {
                    case 0:
                        R0_0.SetMin(value);
                        break;
                    case 1:
                        R1_0.SetMin(value);
                        break;
                    case 2:
                        R2_0.SetMin(value);
                        break;
                    case 3:
                        R3_0.SetMin(value);
                        break;
                    case 4:
                        R4_0.SetMin(value);
                        break;
                }
            }
            if (targetTime <= elapsedTime)
            {
                break;
            }
            yield return null;
        }

        while (true)
        {
            if (serialPort.IsOpen)
            {
                try
                {
                    serialData = serialPort.ReadLine();
                }
                catch (System.Exception e)
                {
                    Debug.LogError("Error reading serial data: " + e.Message);
                }
            }

            try
            {
                string[] datas = serialData.Split('#');
                for (int i = 0; i < datas.Length; i++)
                {
                    int value = int.Parse(datas[i]);
                    float n_value = value;
                    switch (i+1)
                    {
                        case 0:
                            n_value = R0_0.GetNormalizedValue(value);
                            R0_0.Model.localRotation = R0_0.StartRotation * Quaternion.Euler(n_value * 80f, 0, 0);
                            break;
                        case 1:
                            n_value = R1_0.GetNormalizedValue(value);
                            Debug.Log(n_value);
                            R1_0.Model.localRotation = R1_0.StartRotation * Quaternion.Euler(n_value * 80f, 0, 0);
                            break;
                        case 2:
                            n_value = R2_0.GetNormalizedValue(value);
                            R2_0.Model.localRotation = R2_0.StartRotation * Quaternion.Euler(n_value * 80f, 0, 0);
                            break;
                        case 3:
                            n_value = R3_0.GetNormalizedValue(value);
                            R3_0.Model.localRotation = R3_0.StartRotation * Quaternion.Euler(n_value * 80f, 0, 0);
                            break;
                        case 4:
                            n_value = R4_0.GetNormalizedValue(value);
                            R4_0.Model.localRotation = R4_0.StartRotation * Quaternion.Euler(n_value * 80f, 0, 0);
                            break;
                    }
                }
            }
            catch
            {
                //serialPort.
            }
            yield return null;
            yield return new WaitForSeconds(0.01f);
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
