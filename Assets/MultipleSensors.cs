using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using UnityEditor;
using UnityEngine;
using static TreeEditor.TreeGroup;

public class MultipleSensors : MonoBehaviour
{
    Quaternion ConvertVector(Quaternion v, Quaternion preV)
    {
        //return 0.9f*v + 0.1f*preV;
        Quaternion result = v;
        return v;
    }
    SerialPort serialPort;
    [SerializeField] string portName = "COM7"; // 포트 이름
    [SerializeField] int baudRate = 115200; // 보드레이트

    [SerializeField] Transform RH;
    [SerializeField] Transform RLA;
    [SerializeField] Transform RAA;

    [SerializeField] Quaternion RHs;
    [SerializeField] Quaternion RLAs;
    [SerializeField] Quaternion RLA2s;
    [SerializeField] Quaternion RAAs;
    [SerializeField] Quaternion RHms;
    [SerializeField] Quaternion RLAms;
    [SerializeField] Quaternion RAAms;
    [SerializeField] bool[] StartFlag = new bool[4];
    Quaternion preHand;
    Quaternion preRLA;
    Quaternion preRLA2;
    Quaternion preRAA;

    // Start is called before the first frame update
    void Start()
    {
        // 아두이노와의 시리얼 통신을 위한 포트 초기화
        serialPort = new SerialPort(portName, baudRate);
        serialPort.Handshake = Handshake.None;
        serialPort.DataReceived += new SerialDataReceivedEventHandler(sp_DataReceived);
        serialPort.Open(); // 포트 열기
        serialPort.ReadTimeout = 8000;
        //serialPort.time
        serialPort.RtsEnable = true; //안하면 안됨!!
        serialPort.DtrEnable = true;
        RHms = RH.rotation;
        RLAms = RLA.rotation;
        RAAms = RAA.rotation;
        StartCoroutine(ReadSerialData()); // 시리얼 데이터를 읽는 코루틴 시작
    }
    [SerializeField] bool WorkFlag = false;
    void sp_DataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        WorkFlag = true;
    }
    IEnumerator ReadSerialData()
    {
        bool isDeviceReady = false;
        yield return new WaitForSeconds(1);
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
                    //EditorApplication.isPlaying = false;
                }
            }

            try
            {
                Debug.Log(serialData);
                if (serialData.Contains("OK") && isDeviceReady == false)
                {
                    isDeviceReady = true;
                }

                if (isDeviceReady == false)
                {
                    yield return null;
                    continue;
                }
            }
            finally
            {

            }



            try
            {
                string[] datas = serialData.Split('#');
                //Debug.Log(datas.Length);
                if (datas.Length >= 3)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        string[] data = datas[i].Split(',');
                        int tag = int.Parse(data[0]);
                        float x = float.Parse(data[1]);
                        float y = float.Parse(data[2]);
                        float z = float.Parse(data[3]);
                        float w = float.Parse(data[4]);
                        Quaternion rot = new Quaternion(x, y, z, w);
                        Vector3 v = rot.eulerAngles;
                        float v_x = v.x;
                        float v_y = v.y;
                        float v_z = v.z;

                        v.x = -v_y;
                        v.y = -v_z;
                        v.z= v_x;

                        rot = Quaternion.Euler(v);

                        //Vector3 rot = //q.eulerAngles;
                        //    new Vector3(
                        //        float.Parse(data[1]),
                        //        float.Parse(data[2]),
                        //        float.Parse(data[3]));
                        //float x = -rot.y;
                        //float y = rot.z;
                        //float z = -rot.x;

                        //int tag = int.Parse(data[0]);
                        if (tag == 1)
                        {
                            if (StartFlag[i] == false)
                            {
                                RHs = rot;
                                StartFlag[i] = true;
                                continue;
                            }
                            //rot.x = x;
                            //rot.y = y;
                            //rot.z = z;
                            RH.rotation = (ConvertVector((rot), preHand) * Quaternion.Inverse(RHs)) * RHms;
                            preHand = rot;
                        }
                        else if (tag == 2)
                        {
                            if (StartFlag[i] == false)
                            {
                                RLAs = rot;
                                StartFlag[i] = true;
                                continue;
                            }

                            //rot.x = x;
                            //rot.y = y;
                            //rot.z = z;
                            RLA.rotation = (ConvertVector(rot, preRLA) * Quaternion.Inverse(RLAs)) * RLAms;
                            //RLA.eulerAngles = (ConvertVector(rot2, preRLA2) - RLA2s) + RLAms;
                            preRLA = rot;
                        }
                        else if (tag == 3)
                        {
                            if (StartFlag[i] == false)
                            {
                                RAAs = rot;
                                StartFlag[i] = true;
                                continue;
                            }
                            //rot.x = x;
                            //rot.y = y;
                            //rot.z = z;
                            RAA.rotation = (ConvertVector(rot, preRAA) * Quaternion.Inverse(RAAs)) * RAAms;
                            preRAA = rot;
                        }
                        }
                }
            }
            catch
            {
                //serialPort.
            }
            WorkFlag = false;
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
