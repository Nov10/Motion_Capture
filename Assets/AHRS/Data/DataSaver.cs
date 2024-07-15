using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class DataSaver : MonoBehaviour
{
    private string filePath;

    private void Awake()
    {
        // 파일 경로 설정
        filePath = Application.persistentDataPath;// + "/data.dat";
    }
    public void SaveData(List<AHRS.Vector3> gyro, List<AHRS.Vector3> accel, List<AHRS.Vector3> mag, List<double> dt, string tag)
    {
        DataReader.GhostSensor s = new DataReader.GhostSensor();
        s.Gyro.data = gyro;
        s.Accel.data = accel;
        s.Mag.data = mag;
        s.Dt.data = dt;
        string jsonData = JsonUtility.ToJson(s);

        // 파일 생성 또는 열기
        File.WriteAllText(filePath + "/" + tag  + System.DateTime.Now.Day.ToString() + System.DateTime.Now.Hour.ToString() + System.DateTime.Now.Minute.ToString() + System.DateTime.Now.Second.ToString() + ".json", jsonData);
    }
    public void SaveData(List<AHRS.Vector3> dataList, string tag)
    {
        string jsonData = JsonUtility.ToJson(new VectorListWrapper(dataList));

        // 파일 생성 또는 열기
        File.WriteAllText(filePath + "/" + tag + ".json", jsonData);
    }
    public void SaveData(List<double> dataList, string tag)
    {
        string jsonData = JsonUtility.ToJson(new DoubleListWrapper(dataList));

        // 파일 생성 또는 열기
        File.WriteAllText(filePath + "/" + tag + ".json", jsonData);
    }
    //public void SaveData(List<AHRS.Vector3> vectorList, string tag)
    //{
    //    // BinaryFormatter 생성
    //    BinaryFormatter formatter = new BinaryFormatter();

    //    // 파일 생성 또는 열기
    //    FileStream file = File.Open(filePath + "/" + tag + ".dat", FileMode.OpenOrCreate);
    //    Debug.Log(filePath + "/" + tag + ".dat");
    //    // 데이터를 직렬화하여 파일에 쓰기
    //    formatter.Serialize(file, vectorList);

    //    // 파일 닫기
    //    file.Close();
    //}
    //public void SaveData(List<double> vectorList, string tag)
    //{
    //    // BinaryFormatter 생성
    //    BinaryFormatter formatter = new BinaryFormatter();

    //    // 파일 생성 또는 열기
    //    FileStream file = File.Open(filePath + "/" + tag + ".dat", FileMode.OpenOrCreate);

    //    // 데이터를 직렬화하여 파일에 쓰기
    //    formatter.Serialize(file, vectorList);

    //    // 파일 닫기
    //    file.Close();
    //}
}
