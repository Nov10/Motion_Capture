using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AHRS
{
    public enum eSensorType
    {
        GyroOnly = 0,
        ComplementaryFilter_GyroAccel,
        ComplementaryFilter_GyroAccelMag,
        KalmanFilter_XY,
        MadgwickFilter_GyroAccel,
        MadgwickFilter_GyroAccelMag,
        MahonyFilter_GyroAccel,
        MAX
    }
    [System.Serializable]
    public class Sensor : MonoBehaviour
    {
        public enum eSensorQMode
        {
            WXYZ= 0,
            WXZY,
            WYXZ,
            WYZX,
            WZXY,
            WZYX,
            W_iXYZ,
            W_iXZY,
            W_iYXZ,
            W_iYZX,
            W_iZXY,
            W_iZYX,

            WX_iYZ,
            WX_iZY,
            WY_iXZ,
            WY_iZX,
            WZ_iXY,
            WZ_iYX,

            WXY_iZ,
            WXZ_iY,
            WYX_iZ,
            WYZ_iX,
            WZX_iY,
            WZY_iX,

            W_iX_iYZ,
            W_iX_iZY,
            W_iY_iXZ,
            W_iY_iZX,
            W_iZ_iXY,
            W_iZ_iYX,

            W_iXY_iZ,
            W_iXZ_iY,
            W_iYX_iZ,
            W_iYZ_iX,
            W_iZX_iY,
            W_iZY_iX,

            WX_iY_iZ,
            WX_iZ_iY,
            WY_iX_iZ,
            WY_iZ_iX,
            WZ_iX_iY,
            WZ_iY_iX,

            W_iX_iY_iZ,
            W_iX_iZ_iY,
            W_iY_iX_iZ,
            W_iY_iZ_iX,
            W_iZ_iX_iY,
            W_iZ_iY_iX,
        }
        public eSensorQMode SensorQuaternionMode;
        public bool FlipAngle = false;
        public eSensorType SensorType;
        [Range(0, 1)] public double ComplementaryFilterAlpha = 0.95;
        [Range(0, 1)] public double MadgwickFilterBeta = 0.95;
        [Range(0, 0.1f)] public double KalmanFilterAngle = 0.002;
        [Range(0, 0.1f)] public double KalmanFilterGyro = 0.004;
        [Range(0, 0.3f)] public double KalmanFilterMeasure = 0.02;
        AHRS.Quaternion NowRotation = new Quaternion(1, 0, 0, 0);
        public List<AHRS.Vector3> GyroValues = new List<AHRS.Vector3>();
        public List<AHRS.Vector3> AccelValues = new List<AHRS.Vector3>();
        public List<AHRS.Vector3> MagValues = new List<AHRS.Vector3>();
        public List<double> DeltaTimes = new List<double>();

        [SerializeField] bool COMPARESENSOR;
        Transform[] Sensors = new Transform[(int)eSensorType.MAX];

        AHRS.Quaternion StartRotation_Sensor;
        UnityEngine.Quaternion StartRotation_Model;

        public bool StartFlag = false;
        bool InitializerRunning = false;

        [SerializeField] Transform Model;

        AHRS.LinearKalmanFilter KalmanFilterX;
        AHRS.LinearKalmanFilter KalmanFilterY;

        private void Awake()
        {
            KalmanFilterX = new LinearKalmanFilter(KalmanFilterAngle, KalmanFilterGyro, KalmanFilterMeasure);
            KalmanFilterY = new LinearKalmanFilter(KalmanFilterAngle, KalmanFilterGyro, KalmanFilterMeasure);

            //StartRotation_Model = new Quaternion(Model.rotation.w, Model.rotation.x, Model.rotation.y, Model.rotation.z);
            StartRotation_Model = Model.rotation;

            if(COMPARESENSOR == true)
            {
                for(int i = 0; i < Sensors.Length; i++)
                {
                    Sensors[i] = Instantiate(Model);
                    Sensors[i].name = ((eSensorType)i).ToString();
                    Sensors[i].position = new UnityEngine.Vector3(i, 0, 0);
                }
            }
        }

        public void UpdateSensorValue(AHRS.Vector3 gyro, AHRS.Vector3 accel, AHRS.Vector3 mag, double dt)
        {
            if (accel.Magnitude() < 0.001)
                return;
            transform.position = Model.transform.position;
            GyroValues.Add(gyro);
            AccelValues.Add(accel);
            MagValues.Add(mag);
            DeltaTimes.Add(dt);
            Debug.DrawRay(transform.position, new UnityEngine.Vector3((float)accel.x, (float)accel.z, (float)accel.y).normalized, Color.white);

            if(COMPARESENSOR == false)
            {
                var new_q = UpdateSensor(NowRotation);
                NowRotation = new_q.Normalize();
                UnityEngine.Quaternion nowSensorRotation = new UnityEngine.Quaternion((float)NowRotation.x, (float)NowRotation.y, (float)NowRotation.z, (float)NowRotation.w);
                UnityEngine.Quaternion startSensorRotation = new UnityEngine.Quaternion((float)StartRotation_Sensor.x, (float)StartRotation_Sensor.y, (float)StartRotation_Sensor.z, (float)StartRotation_Sensor.w);

                nowSensorRotation = new UnityEngine.Quaternion(-nowSensorRotation.y, nowSensorRotation.x, -nowSensorRotation.z, nowSensorRotation.w);
                UnityEngine.Quaternion q = StartRotation_Model * UnityEngine.Quaternion.Inverse(nowSensorRotation) * UnityEngine.Quaternion.Inverse(startSensorRotation);

                Model.rotation = q;
            }
            else
            {
                for(int i = 0; i < Sensors.Length; i++)
                {
                    SensorType = (eSensorType)i;
                    var new_q = UpdateSensor(CompareQs[i]);
                    CompareQs[i] = new_q;
                    UnityEngine.Quaternion nowSensorRotation = new UnityEngine.Quaternion((float)new_q.x, (float)new_q.y, (float)new_q.z, (float)new_q.w);
                    UnityEngine.Quaternion startSensorRotation = new UnityEngine.Quaternion((float)CompareStartQs[i].x, (float)CompareStartQs[i].y, (float)CompareStartQs[i].z, (float)CompareStartQs[i].w);

                    nowSensorRotation = new UnityEngine.Quaternion(-nowSensorRotation.y, nowSensorRotation.x, -nowSensorRotation.z, nowSensorRotation.w);
                    UnityEngine.Quaternion q = StartRotation_Model * nowSensorRotation * UnityEngine.Quaternion.Inverse(startSensorRotation);

                    Sensors[i].rotation = q;
                }
            }

            //Debug.Log("A");
        }
        AHRS.Quaternion[] CompareQs = new Quaternion[(int)eSensorType.MAX];
        AHRS.Quaternion[] CompareStartQs = new Quaternion[(int)eSensorType.MAX];

        UnityEngine.Quaternion startQuat;
        public void SetRotation(Quaternion rotation)
        {
            UnityEngine.Quaternion q_i = new UnityEngine.Quaternion((float)rotation.x, (float)rotation.y, (float)rotation.z, (float)rotation.w);
            float x = (float)q_i.x;
            float y = (float)q_i.y;
            float z = (float)q_i.z;
            float w = (float)q_i.w;
            if(FlipAngle == true)
            {
                w = w * -1;
            }
            switch (SensorQuaternionMode)
            {
                case eSensorQMode.WXYZ:
                    q_i = new UnityEngine.Quaternion(-x, -y, -z, -w);
                    break;
                case eSensorQMode.WXZY:
                    q_i = new UnityEngine.Quaternion(x, z, y, w);
                    break;
                case eSensorQMode.WYXZ:
                    q_i = new UnityEngine.Quaternion(y, x, z, -w);
                    break;
                case eSensorQMode.WYZX:
                    q_i = new UnityEngine.Quaternion(y, z, x, w);
                    break;
                case eSensorQMode.WZXY:
                    q_i = new UnityEngine.Quaternion(z, x, y, w);
                    break;
                case eSensorQMode.WZYX:
                    q_i = new UnityEngine.Quaternion(z, y, x, w);
                    break;

                case eSensorQMode.W_iXYZ:
                    q_i = new UnityEngine.Quaternion(-x, y, z, w);
                    break;
                case eSensorQMode.W_iXZY:
                    q_i = new UnityEngine.Quaternion(-x, z, y, w);
                    break;
                case eSensorQMode.W_iYXZ:
                    q_i = new UnityEngine.Quaternion(-y, x, z, w);
                    break;
                case eSensorQMode.W_iYZX:
                    q_i = new UnityEngine.Quaternion(-y, z, x, w);
                    break;
                case eSensorQMode.W_iZXY:
                    q_i = new UnityEngine.Quaternion(-z, x, y, w);
                    break;
                case eSensorQMode.W_iZYX:
                    q_i = new UnityEngine.Quaternion(-z, y, x, w);
                    break;


                case eSensorQMode.WX_iYZ:
                    q_i = new UnityEngine.Quaternion(x, -y, z, w);
                    break;
                case eSensorQMode.WX_iZY:
                    q_i = new UnityEngine.Quaternion(x, -z, y, w);
                    break;
                case eSensorQMode.WY_iXZ:
                    q_i = new UnityEngine.Quaternion(y, -x, z, w);
                    break;
                case eSensorQMode.WY_iZX:
                    q_i = new UnityEngine.Quaternion(y, -z, x, w);
                    break;
                case eSensorQMode.WZ_iXY:
                    q_i = new UnityEngine.Quaternion(z, -x, y, w);
                    break;
                case eSensorQMode.WZ_iYX:
                    q_i = new UnityEngine.Quaternion(z, -y, x, w);
                    break;



                case eSensorQMode.WXY_iZ:
                    q_i = new UnityEngine.Quaternion(x, y, -z, w);
                    break;
                case eSensorQMode.WXZ_iY:
                    q_i = new UnityEngine.Quaternion(x, z, -y, w);
                    break;
                case eSensorQMode.WYX_iZ:
                    q_i = new UnityEngine.Quaternion(y, x, -z, w);
                    break;
                case eSensorQMode.WYZ_iX:
                    q_i = new UnityEngine.Quaternion(y, z, -x, w);
                    break;
                case eSensorQMode.WZX_iY:
                    q_i = new UnityEngine.Quaternion(z, x, -y, w);
                    break;
                case eSensorQMode.WZY_iX:
                    q_i = new UnityEngine.Quaternion(z, y, -x, w);
                    break;


                case eSensorQMode.W_iX_iYZ:
                    q_i = new UnityEngine.Quaternion(-x, -y, z, w);
                    break;
                case eSensorQMode.W_iX_iZY:
                    q_i = new UnityEngine.Quaternion(-x, -x, y, w);
                    break;
                case eSensorQMode.W_iY_iXZ:
                    q_i = new UnityEngine.Quaternion(-y,-x,z, w);
                    break;
                case eSensorQMode.W_iY_iZX:
                    q_i = new UnityEngine.Quaternion(-y,-z,x, w);
                    break;
                case eSensorQMode.W_iZ_iXY:
                    q_i = new UnityEngine.Quaternion(-z,-x,y, w);
                    break;
                case eSensorQMode.W_iZ_iYX:
                    q_i = new UnityEngine.Quaternion(-z,-y,x, w);
                    break;

                case eSensorQMode.W_iXY_iZ:
                    q_i = new UnityEngine.Quaternion(-x, y, -z, w);
                    break;
                case eSensorQMode.W_iXZ_iY:
                    q_i = new UnityEngine.Quaternion(-x, z, y, w);
                    break;
                case eSensorQMode.W_iYX_iZ:
                    q_i = new UnityEngine.Quaternion(-y, x, -z, w);
                    break;
                case eSensorQMode.W_iYZ_iX:
                    q_i = new UnityEngine.Quaternion(-y, z, -x, w);
                    break;
                case eSensorQMode.W_iZX_iY:
                    q_i = new UnityEngine.Quaternion(-z, x, -y, w);
                    break;
                case eSensorQMode.W_iZY_iX:
                    q_i = new UnityEngine.Quaternion(-z, y, -x, w);
                    break;

                case eSensorQMode.WX_iY_iZ:
                    q_i = new UnityEngine.Quaternion(x, -y, -z, w);
                    break;
                case eSensorQMode.WX_iZ_iY:
                    q_i = new UnityEngine.Quaternion(x, -y, -z, w);
                    break;
                case eSensorQMode.WY_iX_iZ:
                    q_i = new UnityEngine.Quaternion(y, -x, -z, w);
                    break;
                case eSensorQMode.WY_iZ_iX:
                    q_i = new UnityEngine.Quaternion(y, -z, -x, w);
                    break;
                case eSensorQMode.WZ_iX_iY:
                    q_i = new UnityEngine.Quaternion(z, -x, -y, w);
                    break;
                case eSensorQMode.WZ_iY_iX:
                    q_i = new UnityEngine.Quaternion(z, -y, -x, w);
                    break;


                case eSensorQMode.W_iX_iY_iZ:
                    q_i = new UnityEngine.Quaternion(-x, -y, -z, w);
                    break;
                case eSensorQMode.W_iX_iZ_iY:
                    q_i = new UnityEngine.Quaternion(-x, -z, -y, w);
                    break;
                case eSensorQMode.W_iY_iX_iZ:
                    q_i = new UnityEngine.Quaternion(-y, -x, -z, w);
                    break;
                case eSensorQMode.W_iY_iZ_iX:
                    q_i = new UnityEngine.Quaternion(-y, -z, -x, w);
                    break;
                case eSensorQMode.W_iZ_iX_iY:
                    q_i = new UnityEngine.Quaternion(-z, -x, -y, w);
                    break;
                case eSensorQMode.W_iZ_iY_iX:
                    q_i = new UnityEngine.Quaternion(-z, -y, -x, w);
                    break;
            }
            if (StartFlag == false)
            {
                StartFlag  = true;
                startQuat = q_i;
            }
            UnityEngine.Quaternion q = StartRotation_Model * q_i * UnityEngine.Quaternion.Inverse(startQuat);
            if (q.w * q.w <= 0.0000001f)
                return;
            Model.rotation = UnityEngine.Quaternion.Lerp(preQ, q, 0.8f);
            preQ = q;
        }
        UnityEngine.Quaternion preQ = UnityEngine.Quaternion.identity;
        AHRS.Vector3 LastGyro
        {
            get { return GyroValues[GyroValues.Count - 1]; }
        }
        AHRS.Vector3 LastAccel
        {
            get { return AccelValues[AccelValues.Count - 1]; }
        }
        AHRS.Vector3 LastMag
        {
            get { return MagValues[MagValues.Count - 1]; }
        }
        double LastDeltaTime
        {
            get { return DeltaTimes[DeltaTimes.Count - 1]; }
        }
        Vector3 preA = new Vector3();
        public AHRS.Quaternion UpdateSensor(AHRS.Quaternion nowR)
        {
            AHRS.Quaternion target = new Quaternion(1, 0, 0, 0);

            switch (SensorType)
            {
                case eSensorType.GyroOnly:
                    target = AHRS.Gyro.Update_Trigonometric(nowR, LastGyro, LastDeltaTime);
                    break;
                case eSensorType.ComplementaryFilter_GyroAccel:
                    target = AHRS.ComplementaryFilter.Update_GyroAccel(nowR, LastGyro, LastAccel * -1, LastDeltaTime, ComplementaryFilterAlpha);
                    break;
                case eSensorType.ComplementaryFilter_GyroAccelMag:
                    target = AHRS.ComplementaryFilter.Update_GyroAccelMag(nowR, LastGyro, LastAccel, LastMag, LastDeltaTime, ComplementaryFilterAlpha);
                    break;
                case eSensorType.KalmanFilter_XY:
                    //var anlgeZ = AHRS.ComplementaryFilter.Update_GyroAccel(nowR, LastGyro, LastAccel, LastDeltaTime, ComplementaryFilterAlpha).ToEulerAngles_DEG().z;
                    var anlgeZ = AHRS.Gyro.Update_Derivative(nowR, LastGyro, LastDeltaTime).ToEulerAngles_DEG().z;
                    var a = LastAccel;
                    var g = LastGyro;
                    var m = LastMag;
                    var dt = LastDeltaTime;

double accXangle = Math.Atan2(a.y, Math.Sqrt(a.z * a.z + a.x * a.x));
double accYangle = Math.Atan2(a.x, Math.Sqrt(a.z * a.z + a.y * a.y));
                    accXangle = (accXangle * 0.3 + preA.x * 0.7);
                    accYangle = (accYangle * 0.3 + preA.y * 0.7);
                    preA = new Vector3(accXangle, accYangle, 0);
                    //if (a.z > -3)
                    //{
                    //    target = AHRS.Gyro.Update_Derivative(nowR, LastGyro, LastDeltaTime);
                    //}
                    //else
                    //{
double k_x = KalmanFilterX.Update(accXangle * UnityEngine.Mathf.Rad2Deg, g.x, dt);
double k_y = KalmanFilterY.Update(accYangle * UnityEngine.Mathf.Rad2Deg, g.y, dt);
double x = accXangle;
double y = accYangle;
target = AHRS.Quaternion.FromEulerAngles_DEG(k_x, k_y, anlgeZ);
                   // }

                    break;
                case eSensorType.MadgwickFilter_GyroAccelMag:
                    var v = AHRS.MadgwickFilter.Madgwick_IMU_Magnetometer(new double[] { LastAccel.x, LastAccel.y, LastAccel.z},
                        new double[] { LastGyro.x, LastGyro.y, LastGyro.z },
                        new double[] { LastMag.x, LastMag.y, LastMag.z },
                        new double[] { nowR.w, nowR.x, nowR.y, nowR.z }, MadgwickFilterBeta, LastDeltaTime);
                    target = new Quaternion(v[0], v[1], v[2], v[3]);
                    break;
                case eSensorType.MadgwickFilter_GyroAccel:
                    //var v2 = AHRS.MadgwickFilter.Madgwick_IMU(new double[] { -LastAccel.x, -LastAccel.y, -LastAccel.z },
                    //    new double[] { LastGyro.x, LastGyro.y, LastGyro.z },
                    //    new double[] { nowR.w, nowR.x, nowR.y, nowR.z }, MadgwickFilterBeta, LastDeltaTime);
                    target = AHRS.MadgwickFilter.Madgwick_IMU(LastAccel * -1, LastGyro, nowR, MadgwickFilterBeta, LastDeltaTime);
                    //target = new Quaternion(v2[0], v2[1], v2[2], v2[3]);
                    break;

                case eSensorType.MahonyFilter_GyroAccel:
                    var q = M.Update_GyroAccel(nowR, LastGyro, -1 * LastAccel, (float)LastDeltaTime);
                    target = q;
                    break;
            }

            if(StartFlag == false && InitializerRunning == false)
            {
                StartCoroutine(_StartInitializer());
            }

            if(StartFlag == false)
            {
                target = new Quaternion(1, 0, 0, 0);
            }

            return target.Normalize();
        }
        MahonyFilter M = new MahonyFilter();
        public void ResetSensor()
        {
            StartFlag = false;
            //StartRotation_Sensor = NowRotation;
        }
        AHRS.Vector3 StartGyro = new Vector3(0,0,0);
        IEnumerator _StartInitializer()
        {
            InitializerRunning = true;
            yield return new WaitForSeconds(2);

            StartRotation_Sensor = NowRotation;
            //StartRotation_Sensor = new Quaternion(1, 0, 0, 0);
            for(int i = 0; i< CompareStartQs.Length; i++)
            {
                CompareStartQs[i] = CompareQs[i];
            }
            StartFlag = true;
            StartGyro = LastGyro;
            yield return new WaitForSeconds(1);
            InitializerRunning = false;
        }
    }
}