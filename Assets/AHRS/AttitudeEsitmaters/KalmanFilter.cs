using System;

namespace AHRS
{
    public class KalmanFilter
    {
        private double Q_angle, Q_gyro, R_measure;
        private double angle, bias;
        private double[,] P = new double[2, 2];
        private double[] K = new double[2];

        public KalmanFilter(double angle, double gyro, double measure)
        {
            Q_angle = angle;
            Q_gyro = gyro;
            R_measure = measure;

            this.angle = 0;
            bias = 0;

            P[0, 0] = 0;
            P[0, 1] = 0;
            P[1, 0] = 0;
            P[1, 1] = 0;
        }

        public double Update(double newValue, double newRate, double dt)
        {
            //double dt = (double)(DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond) / 1000;

            double K_rate = newRate - bias;
            angle += dt * K_rate;

            P[0, 0] += dt * (dt * P[1,1] - P[0,1] - P[1,0] + Q_angle);
            //P[0,0] += dt * (P[1,1] + P[0,1] + Q_angle);
            P[0, 1] -= dt * P[1, 1];
            P[1, 0] -= dt * P[1, 1];
            P[1, 1] += Q_gyro * dt;

            double S = P[0, 0] + R_measure;
            K[0] = P[0, 0] / S;
            K[1] = P[1, 0] / S;

            double y = newValue - angle;
            angle += K[0] * y;
            bias += K[1] * y;

            P[0, 0] -= K[0] * P[0, 0];
            P[0, 1] -= K[0] * P[0, 1];
            P[1, 0] -= K[1] * P[0, 0];
            P[1, 1] -= K[1] * P[0, 1];

            return angle;
        }

        public void SetAngle(double newAngle)
        {
            angle = newAngle;
        }

        public double GetAngle()
        {
            return angle;
        }

        public double GetBias()
        {
            return bias;
        }

        public void SetQAngle(double newQ_angle)
        {
            Q_angle = newQ_angle;
        }

        public void SetQGyro(double newQ_gyro)
        {
            Q_gyro = newQ_gyro;
        }

        public void SetRMeasure(double newR_measure)
        {
            R_measure = newR_measure;
        }

        public double GetQAngle()
        {
            return Q_angle;
        }

        public double GetQGyro()
        {
            return Q_gyro;
        }

        public double GetRMeasure()
        {
            return R_measure;
        }
    }
}