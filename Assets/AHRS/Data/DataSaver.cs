using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class DataSaver : MonoBehaviour
{
    private string filePath;

    private void Awake()
    {
        // ���� ��� ����
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

        // ���� ���� �Ǵ� ����
        File.WriteAllText(filePath + "/" + tag  + System.DateTime.Now.Day.ToString() + System.DateTime.Now.Hour.ToString() + System.DateTime.Now.Minute.ToString() + System.DateTime.Now.Second.ToString() + ".json", jsonData);
    }
    public void SaveData(List<AHRS.Vector3> dataList, string tag)
    {
        string jsonData = JsonUtility.ToJson(new VectorListWrapper(dataList));

        // ���� ���� �Ǵ� ����
        File.WriteAllText(filePath + "/" + tag + ".json", jsonData);
    }
    public void SaveData(List<double> dataList, string tag)
    {
        string jsonData = JsonUtility.ToJson(new DoubleListWrapper(dataList));

        // ���� ���� �Ǵ� ����
        File.WriteAllText(filePath + "/" + tag + ".json", jsonData);
    }
    //public void SaveData(List<AHRS.Vector3> vectorList, string tag)
    //{
    //    // BinaryFormatter ����
    //    BinaryFormatter formatter = new BinaryFormatter();

    //    // ���� ���� �Ǵ� ����
    //    FileStream file = File.Open(filePath + "/" + tag + ".dat", FileMode.OpenOrCreate);
    //    Debug.Log(filePath + "/" + tag + ".dat");
    //    // �����͸� ����ȭ�Ͽ� ���Ͽ� ����
    //    formatter.Serialize(file, vectorList);

    //    // ���� �ݱ�
    //    file.Close();
    //}
    //public void SaveData(List<double> vectorList, string tag)
    //{
    //    // BinaryFormatter ����
    //    BinaryFormatter formatter = new BinaryFormatter();

    //    // ���� ���� �Ǵ� ����
    //    FileStream file = File.Open(filePath + "/" + tag + ".dat", FileMode.OpenOrCreate);

    //    // �����͸� ����ȭ�Ͽ� ���Ͽ� ����
    //    formatter.Serialize(file, vectorList);

    //    // ���� �ݱ�
    //    file.Close();
    //}
}
