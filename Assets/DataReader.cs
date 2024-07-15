using AHRS;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[System.Serializable]
public class VectorListWrapper
{
    public List<AHRS.Vector3> data;

    public VectorListWrapper(List<AHRS.Vector3> data)
    {
        this.data = data;
    }   
}
[System.Serializable]
public class DoubleListWrapper
{
    public List<double> data;

    public DoubleListWrapper(List<double> data)
    {
        this.data = data;
    }
}
public class DataReader : MonoBehaviour
{
    [System.Serializable]
    public class GhostSensor
    {
        //public List<AHRS.Vector3> Gyro = new List<AHRS.Vector3>();
        //public List<AHRS.Vector3> Accel = new List<AHRS.Vector3>();
        //public List<AHRS.Vector3> Mag = new List<AHRS.Vector3>();
        //public List<double> Dt = new List<double>();

        public VectorListWrapper Gyro = new VectorListWrapper(new List<AHRS.Vector3>());
        public VectorListWrapper Accel = new VectorListWrapper(new List<AHRS.Vector3>());
        public VectorListWrapper Mag = new VectorListWrapper(new List<AHRS.Vector3>());
        public DoubleListWrapper Dt = new DoubleListWrapper(new List<double>());


        public int DatLength
        {
            get 
            { 
                return Dt.data.Count; 
            }
        }

        public void GetData(int idx, out AHRS.Vector3 gyro, out AHRS.Vector3 accel, out AHRS.Vector3 mag, out double dt)
        {
            if(idx < DatLength)
            {
                gyro = Gyro.data[idx];
                accel = Accel.data[idx];
                mag = Mag.data[idx];
                dt = Dt.data[idx];

                gyro = gyro / 2f;
            }           

            else
            {
                gyro = new AHRS.Vector3(0, 0, 0);
                accel = new AHRS.Vector3(0, 0, 1);
                mag = new AHRS.Vector3(0, 0, 1);
                dt = 0;
            }
        }
    }

    [SerializeField] SensorController CaptureUnit;

    private string filePath;

    GhostSensor RH = new GhostSensor();
    GhostSensor RLA = new GhostSensor();
    GhostSensor RUA = new GhostSensor();
    GhostSensor LH = new GhostSensor();
    GhostSensor LLA = new GhostSensor();
    GhostSensor LUA = new GhostSensor();

    private void Awake()
    {
        // 파일 경로 설정
        filePath = Application.persistentDataPath;
    }
    string DateTag = "150022";
    GhostSensor ReadData(string tag)
    {
        string jsonData = File.ReadAllText(filePath + "/" + tag + DateTag  + ".json");
        return JsonUtility.FromJson<GhostSensor>(jsonData);
    }

    //List<AHRS.Vector3> ReadVectorData(string tag)
    //{
    //    // 파일이 존재하는지 확인
    //    if (File.Exists(filePath + "/" + tag + ".dat"))
    //    {
    //        // BinaryFormatter 생성
    //        BinaryFormatter formatter = new BinaryFormatter();

    //        FileStream file = File.Open(filePath + "/" + tag + ".dat", FileMode.Open);

    //        // 파일 열기

    //        // 파일에서 데이터를 역직렬화하여 읽기
    //        List<AHRS.Vector3> vectorList = (List<AHRS.Vector3>)formatter.Deserialize(file);

    //        // 파일 닫기
    //        file.Close();

    //        return vectorList;
    //    }
    //    else
    //    {
    //        Debug.LogWarning("불러올 파일이 없습니다.");
    //        return new List<AHRS.Vector3>();
    //    }
    //}
    //List<double> ReadDoubleData(string tag)
    //{
    //    // 파일이 존재하는지 확인
    //    if (File.Exists(filePath + "/" + tag + ".dat"))
    //    {
    //        // BinaryFormatter 생성
    //        BinaryFormatter formatter = new BinaryFormatter();

    //        // 파일 열기
    //        FileStream file = File.Open(filePath + "/" + tag + ".dat", FileMode.Open);

    //        // 파일에서 데이터를 역직렬화하여 읽기
    //        List<double> vectorList = (List<double>)formatter.Deserialize(file);

    //        // 파일 닫기
    //        file.Close();

    //        return vectorList;
    //    }
    //    else
    //    {
    //        Debug.LogWarning("불러올 파일이 없습니다.");
    //        return new List<double>();
    //    }
    //}
    public void ReadSesnroData(string tag, GhostSensor sensor)
    {
        var v = ReadData(tag);
        sensor.Gyro = v.Gyro;
        sensor.Accel = v.Accel;
        sensor.Mag = v.Mag;
        sensor.Dt = v.Dt;
        //sensor.Gyro = ReadVectorData(tag + "Gyro");
        //sensor.Accel = ReadVectorData(tag + "Accel");
        //sensor.Mag = ReadVectorData(tag + "Mag");
        //sensor.Dt = ReadDoubleData(tag + "dt");
    }
    private void Start()
    {
        ReadSesnroData("RH", RH);
        ReadSesnroData("RLA", RLA);
        ReadSesnroData("RUA", RUA);
        ReadSesnroData("LH", LH);
        ReadSesnroData("LLA", LLA);
        ReadSesnroData("LUA", LUA);

        StartCoroutine(_Start());
    }

    int MinDataLength()
    {
        int[] numbers = { RH.DatLength };//, RLA.DatLength, RUA.DatLength, LH.DatLength, LLA.DatLength, LUA.DatLength };
        return numbers.Min();
    }
    int MaxDataLength()
    {
        int[] numbers = { RH.DatLength , RLA.DatLength, RUA.DatLength, LH.DatLength, LLA.DatLength, LUA.DatLength };
        return numbers.Max();
    }
    IEnumerator _Start()
    {
        int c = MaxDataLength();
        for (int i = 2; i < c; i++)
        {
            AHRS.Vector3 gyro;
            AHRS.Vector3 accel;
            AHRS.Vector3 mag;
            double dt;
            double maxdt = 0;

            RH.GetData(i, out gyro, out accel, out mag, out dt);
            if (maxdt < dt)
                maxdt = dt;
            CaptureUnit.SetSensorValue("RH", gyro, accel, mag, dt);
            RLA.GetData(i, out gyro, out accel, out mag, out dt);
            if (maxdt < dt)
                maxdt = dt;
            CaptureUnit.SetSensorValue("RLA", gyro, accel, mag, dt);
            RUA.GetData(i, out gyro, out accel, out mag, out dt);
            if (maxdt < dt)
                maxdt = dt;
            CaptureUnit.SetSensorValue("RUA", gyro, accel, mag, dt);

            LH.GetData(i, out gyro, out accel, out mag, out dt);
            if (maxdt < dt)
                maxdt = dt;
            CaptureUnit.SetSensorValue("LH", gyro, accel, mag, dt);
            LLA.GetData(i, out gyro, out accel, out mag, out dt);
            if (maxdt < dt)
                maxdt = dt;
            CaptureUnit.SetSensorValue("LLA", gyro, accel, mag, dt);
            LUA.GetData(i, out gyro, out accel, out mag, out dt);
            if (maxdt < dt)
                maxdt = dt;
            CaptureUnit.SetSensorValue("LUA", gyro, accel, mag, dt);
            yield return new WaitForSeconds((float)maxdt);
        }
    }
}
