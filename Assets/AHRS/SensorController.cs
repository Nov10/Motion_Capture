using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AHRS
{
    public enum eSensorPart
    {
        RightHand = 0,
        RightLowerArm,
        RightUpperArm,
        LeftHand,
        LeftLowerArm,
        LeftUpperArm,
        LowerSpine,
        CenterSpine,
        UpperSpine
    }
    public class SensorController : MonoBehaviour
    {
        [SerializeField] Sensor RightHand;
        [SerializeField] Sensor RightLowerArm;
        [SerializeField] Sensor RightUpperArm;


        [SerializeField] Sensor LeftHand;
        [SerializeField] Sensor LeftLowerArm;
        [SerializeField] Sensor LeftUpperArm;

        [SerializeField] Sensor LowerSpine;
        [SerializeField] Sensor CenterSpine;
        [SerializeField] Sensor UpperSpine;

        public DataSaver dataSaver;

        public void SaveSensor(Sensor sensor, string tag)
        {
            dataSaver.SaveData(sensor.GyroValues, sensor.AccelValues, sensor.MagValues, sensor.DeltaTimes, tag);
            //dataSaver.SaveData(sensor.GyroValues, tag + "Gyro");
            //dataSaver.SaveData(sensor.AccelValues, tag + "Accel");
            //dataSaver.SaveData(sensor.MagValues, tag + "Mag");
            //dataSaver.SaveData(sensor.DeltaTimes, tag + "dt");
        }
        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("Saving...");
                SaveSensor(RightHand, "RH");
                SaveSensor(RightLowerArm, "RLA");
                SaveSensor(RightUpperArm, "RUA");


                SaveSensor(LeftHand, "LH");
                SaveSensor(LeftLowerArm, "LLA");
                SaveSensor(LeftUpperArm, "LUA");

                Debug.Log("End!");
            }

            if(Input.GetKeyDown(KeyCode.R))
            {
                ResetSensors();
            }
        }
        [SerializeField] float Scale;
        public void SetSensorValue(string dataLine, float dt)
        {
            string[] data = dataLine.Split(',');
            string sensorTag = data[0];
            double ax = float.Parse(data[1]);
            double ay = float.Parse(data[2]);
            double az = float.Parse(data[3]);
            double gx = float.Parse(data[4]);
            double gy = float.Parse(data[5]);
            double gz = float.Parse(data[6]);
            double mx = float.Parse(data[7]);
            double my = float.Parse(data[8]);
            double mz = float.Parse(data[9]);

            AHRS.Vector3 accelerometer = new AHRS.Vector3(ax, ay, az) ;
            AHRS.Vector3 gyroscope = new AHRS.Vector3(gx, gy, gz) / Scale;// * 1.7;
            AHRS.Vector3 magnetometer = new AHRS.Vector3(mx, my, mz);
            //Debug.Log(sensorTag);
            switch (sensorTag)
            {
                case "RH":
                    RightHand.UpdateSensorValue(gyroscope, accelerometer, magnetometer, dt);
                    break;
                case "RLA":
                    RightLowerArm.UpdateSensorValue(gyroscope, accelerometer, magnetometer, dt);
                    break;
                case "RHA":
                    RightUpperArm.UpdateSensorValue(gyroscope, accelerometer, magnetometer, dt);
                    break;
                case "LH":
                    LeftHand.UpdateSensorValue(gyroscope, accelerometer, magnetometer, dt);
                    break;
                case "LLA":
                    LeftLowerArm.UpdateSensorValue(gyroscope, accelerometer, magnetometer, dt);
                    break;
                case "LHA":
                    LeftUpperArm.UpdateSensorValue(gyroscope, accelerometer, magnetometer, dt);
                    break;
                case "LS":
                    LowerSpine.UpdateSensorValue(gyroscope, accelerometer, magnetometer, dt);
                    break;
                case "CS":
                    CenterSpine.UpdateSensorValue(gyroscope, accelerometer, magnetometer, dt);
                    break;
                case "US":
                    UpperSpine.UpdateSensorValue(gyroscope, accelerometer, magnetometer, dt);
                    break;
            }
        }
        public void SetSensorValueQuat(string dataLine)
        {
            string[] data = dataLine.Split(',');
            string sensorTag = data[0];
            double w = float.Parse(data[1]);
            double x = float.Parse(data[2]);
            double y = float.Parse(data[3]);
            double z = float.Parse(data[4]);

            Quaternion q = new Quaternion(w, x, y, z);
            switch (sensorTag)
            {
                case "RH":
                    RightHand.SetRotation(q);
                    break;
                case "RLA":
                    RightLowerArm.SetRotation(q);
                    break;
                case "RHA":
                    RightUpperArm.SetRotation(q);
                    break;
                case "LH":
                    LeftHand.SetRotation(q);
                    break;
                case "LLA":
                    LeftLowerArm.SetRotation(q);
                    break;
                case "LHA":
                    LeftUpperArm.SetRotation(q);
                    break;
                case "LS":
                    LowerSpine.SetRotation(q);
                    break;
                case "CS":
                    CenterSpine.SetRotation(q);
                    break;
                case "US":
                    UpperSpine.SetRotation(q);
                    break;
            }
        }
        public void SetSensorValue(string tag, AHRS.Vector3 gyro, AHRS.Vector3 accel, AHRS.Vector3 mag, double dt)
        {
            //if (accel.Magnitude() < double.Epsilon * 1000)
            //    return;
            AHRS.Vector3 gyroscope = gyro;
            AHRS.Vector3 accelerometer = accel;
            AHRS.Vector3 magnetometer = mag;
            //Debug.Log(sensorTag);
            switch (tag)
            {
                case "RH":
                    RightHand.UpdateSensorValue(gyroscope, accelerometer, magnetometer, dt);
                    break;
                case "RLA":
                    RightLowerArm.UpdateSensorValue(gyroscope, accelerometer, magnetometer, dt);
                    break;
                case "RUA":
                    RightUpperArm.UpdateSensorValue(gyroscope, accelerometer, magnetometer, dt);
                    break;
                case "LH":
                    LeftHand.UpdateSensorValue(gyroscope, accelerometer, magnetometer, dt);
                    break;
                case "LLA":
                    LeftLowerArm.UpdateSensorValue(gyroscope, accelerometer, magnetometer, dt);
                    break;
                case "LUA":
                    LeftUpperArm.UpdateSensorValue(gyroscope, accelerometer, magnetometer, dt);
                    break;
                case "LS":
                    LowerSpine.UpdateSensorValue(gyroscope, accelerometer, magnetometer, dt);
                    break;
                case "CS":
                    CenterSpine.UpdateSensorValue(gyroscope, accelerometer, magnetometer, dt);
                    break;
                case "US":
                    UpperSpine.UpdateSensorValue(gyroscope, accelerometer, magnetometer, dt);
                    break;
            }
        }


        public void UpdateSensorValues(eSensorPart part, AHRS.Vector3 gyro, AHRS.Vector3 accel, AHRS.Vector3 mag, double dt)
        {
            GetSensor(part).UpdateSensorValue(gyro, accel, mag, dt);
        }
        IEnumerator _ResetSensors()
        {
            yield return new WaitForSeconds(2);
            RightHand.ResetSensor();
            RightLowerArm.ResetSensor();
            RightUpperArm.ResetSensor();


            LeftHand.ResetSensor();
            LeftLowerArm.ResetSensor();
            LeftUpperArm.ResetSensor();
        }
        public void ResetSensors()
        {
            StartCoroutine(_ResetSensors());

            //LowerSpine.ResetSensor();
            //CenterSpine.ResetSensor();
            //UpperSpine.ResetSensor();
        }


        Sensor GetSensor(eSensorPart part)
        {
            switch (part)
            {
                case eSensorPart.RightHand:
                    return RightHand;
                case eSensorPart.RightLowerArm:
                    return RightLowerArm;
                case eSensorPart.RightUpperArm:
                    return RightUpperArm;
                case eSensorPart.LeftHand:
                    return LeftHand;
                case eSensorPart.LeftLowerArm:
                    return LeftLowerArm;
                case eSensorPart.LeftUpperArm:
                    return LeftUpperArm;
                case eSensorPart.LowerSpine:
                    return LowerSpine;
                case eSensorPart.CenterSpine:
                    return CenterSpine;
                case eSensorPart.UpperSpine:
                    return UpperSpine;
            }
            return null;
        }
    }
}