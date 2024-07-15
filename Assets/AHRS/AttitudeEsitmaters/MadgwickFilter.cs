using System;

namespace AHRS
{
    public class MadgwickFilter
    {
        // Quaternion production
        public static double[] QuaternProd(double[] a, double[] b)
        {
            double[] row = new double[4];

            double a1 = a[0];
            double a2 = a[1];
            double a3 = a[2];
            double a4 = a[3];

            double b1 = b[0];
            double b2 = b[1];
            double b3 = b[2];
            double b4 = b[3];

            row[0] = a1 * b1 - a2 * b2 - a3 * b3 - a4 * b4;
            row[1] = a1 * b2 + a2 * b1 + a3 * b4 - a4 * b3;
            row[2] = a1 * b3 - a2 * b4 + a3 * b1 + a4 * b2;
            row[3] = a1 * b4 + a2 * b3 - a3 * b2 + a4 * b1;

            return row;
        }

        // Quaternion conjugation
        public static double[] QuaternConj(double[] q)
        {
            double[] q_conj = new double[4];

            q_conj[0] = q[0];
            q_conj[1] = -q[1];
            q_conj[2] = -q[2];
            q_conj[3] = -q[3];

            return q_conj;
        }

        // Gradient descent loss function
        public static double[,] func_F(double[] SE_q, double[] E_d, double[] S_s)
        {
            double[,] row = new double[3, 1];

            double dx = E_d[0];
            double dy = E_d[1];
            double dz = E_d[2];

            double sx = S_s[0];
            double sy = S_s[1];
            double sz = S_s[2];

            double q1 = SE_q[0];
            double q2 = SE_q[1];
            double q3 = SE_q[2];
            double q4 = SE_q[3];

            row[0, 0] = 2.0 * dx * (0.5 - q3 * q3 - q4 * q4) + 2.0 * dy * (q1 * q4 + q2 * q3) + 2.0 * dz * (q2 * q4 - q1 * q3) - sx;
            row[1, 0] = 2.0 * dx * (q2 * q3 - q1 * q4) + 2.0 * dy * (0.5 - q2 * q2 - q4 * q4) + 2.0 * dz * (q1 * q2 + q3 * q4) - sy;
            row[2, 0] = 2.0 * dx * (q1 * q3 + q2 * q4) + 2.0 * dy * (q3 * q4 - q1 * q2) + 2.0 * dz * (0.5 - q2 * q2 - q3 * q3) - sz;

            return row;
        }
        public static Vector3 func_F(Quaternion q, Vector3 g, Vector3 a)
        {
            double[,] row = new double[3, 1];

            double dx = g.x;
            double dy = g.y;
            double dz = g.z;

            double sx = a.x;
            double sy = a.y;
            double sz = a.z;

            double q1 = q.w;
            double q2 = q.x;
            double q3 = q.y;
            double q4 = q.z;

            Vector3 result = new Vector3(
                2.0 * dx * (0.5 - q3 * q3 - q4 * q4) + 2.0 * dy * (q1 * q4 + q2 * q3) + 2.0 * dz * (q2 * q4 - q1 * q3) - sx,
                2.0 * dx * (q2 * q3 - q1 * q4) + 2.0 * dy * (0.5 - q2 * q2 - q4 * q4) + 2.0 * dz * (q1 * q2 + q3 * q4) - sy,
                2.0 * dx * (q1 * q3 + q2 * q4) + 2.0 * dy * (q3 * q4 - q1 * q2) + 2.0 * dz * (0.5 - q2 * q2 - q3 * q3) - sz
            );
            return result;
        }
        public static Matrix3x4 J(Quaternion q, Vector3 g)
        {
            double[,] row = new double[3, 4];

            double q1 = q.w;
            double q2 = q.x;
            double q3 = q.y;
            double q4 = q.z;

            double dx = g.x;
            double dy = g.y;
            double dz = g.z;

            row[0, 0] = 2.0 * dy * q4 - 2.0 * dz * q3;
            row[0, 1] = 2.0 * dy * q3 + 2.0 * dz * q4;
            row[0, 2] = -4.0 * dx * q3 + 2.0 * dy * q2 - 2.0 * dz * q1;
            row[0, 3] = -4.0 * dx * q4 + 2.0 * dy * q1 + 2.0 * dz * q2;

            row[1, 0] = -2.0 * dx * q4 + 2.0 * dz * q2;
            row[1, 1] = 2.0 * dx * q3 - 4.0 * dy * q2 + 2.0 * dz * q1;
            row[1, 2] = 2.0 * dx * q2 + 2.0 * dz * q4;
            row[1, 3] = -2.0 * dx * q1 - 4.0 * dy * q4 + 2.0 * dz * q3;

            row[2, 0] = 2.0 * dx * q3 - 2.0 * dy * q2;
            row[2, 1] = 2.0 * dx * q4 - 2.0 * dy * q1 - 4.0 * dz * q2;
            row[2, 2] = 2.0 * dx * q1 + 2.0 * dy * q4 - 4.0 * dz * q3;
            row[2, 3] = 2.0 * dx * q2 + 2.0 * dy * q3;

            return new Matrix3x4(row);
            //return row;
        }
        // Jacobian of loss function
        public static double[,] J(double[] SE_q, double[] E_d)
        {
            double[,] row = new double[3, 4];

            double q1 = SE_q[0];
            double q2 = SE_q[1];
            double q3 = SE_q[2];
            double q4 = SE_q[3];

            double dx = E_d[0];
            double dy = E_d[1];
            double dz = E_d[2];

            row[0, 0] = 2.0 * dy * q4 - 2.0 * dz * q3;
            row[0, 1] = 2.0 * dy * q3 + 2.0 * dz * q4;
            row[0, 2] = -4.0 * dx * q3 + 2.0 * dy * q2 - 2.0 * dz * q1;
            row[0, 3] = -4.0 * dx * q4 + 2.0 * dy * q1 + 2.0 * dz * q2;

            row[1, 0] = -2.0 * dx * q4 + 2.0 * dz * q2;
            row[1, 1] = 2.0 * dx * q3 - 4.0 * dy * q2 + 2.0 * dz * q1;
            row[1, 2] = 2.0 * dx * q2 + 2.0 * dz * q4;
            row[1, 3] = -2.0 * dx * q1 - 4.0 * dy * q4 + 2.0 * dz * q3;

            row[2, 0] = 2.0 * dx * q3 - 2.0 * dy * q2;
            row[2, 1] = 2.0 * dx * q4 - 2.0 * dy * q1 - 4.0 * dz * q2;
            row[2, 2] = 2.0 * dx * q1 + 2.0 * dy * q4 - 4.0 * dz * q3;
            row[2, 3] = 2.0 * dx * q2 + 2.0 * dy * q3;

            return row;
        }

        // Madgwick AHRS using IMU data only
public static Quaternion Madgwick_IMU(Vector3 accel, Vector3 gyro, Quaternion nowRotation, double beta, double dt)
{
    accel = accel.Normalize();
    //지구 중력 벡터
    Vector3 g = new Vector3(0, 0, 1);

    //오차 계산
    Vector3 f = func_F(nowRotation, g, accel);
    Matrix3x4 jacb = J(nowRotation, g);

    // 경사 계산
    Vector4 gda_tmp = jacb.Transpose() * f;
    //정규화
    Quaternion gda = new Quaternion(gda_tmp.w, gda_tmp.x, gda_tmp.y, gda_tmp.z).Normalize();

    //보장할 오차 계산
    Quaternion rotation = gda * dt * beta;

    //Madgwick 필터 계산
    Quaternion updated_quac = Gyro.Update_Trigonometric(nowRotation, gyro, dt) + rotation;
    updated_quac = updated_quac.Normalize();

    return updated_quac;
}

        // Madgwick AHRS using IMU data + Magnetometer
        public static double[] Madgwick_IMU_Magnetometer(double[] Accelerometer, double[] Gyroscope, double[] Magnetometer, double[] q, double beta, double SamplePeriod)
        {
            double Accel_norm = Math.Sqrt(Accelerometer[0] * Accelerometer[0] + Accelerometer[1] * Accelerometer[1] + Accelerometer[2] * Accelerometer[2]);
            double[] Accel_normalized = { Accelerometer[0] / Accel_norm, Accelerometer[1] / Accel_norm, Accelerometer[2] / Accel_norm };

            double[] g_n = { 0, 0, 1 }; // Reference vector
            double[] E_d = g_n;
            double[] S_s = Accel_normalized;

            double[,] F_Accel = func_F(q, E_d, S_s);
            double[,] Delta_Accel = J(q, E_d);

            // Normalize Magnetometer Measurement      
            double Magnet_norm = Math.Sqrt(Magnetometer[0] * Magnetometer[0] + Magnetometer[1] * Magnetometer[1] + Magnetometer[2] * Magnetometer[2]);
            double[] Magnet_normalized = { Magnetometer[0] / Magnet_norm, Magnetometer[1] / Magnet_norm, Magnetometer[2] / Magnet_norm };

            // Reference direction of Earth's magnetic field
            // Rotation from body to inertial frame using quaternion
            double[] h = QuaternProd(q, QuaternProd(new double[] { 0, Magnet_normalized[0], Magnet_normalized[1], Magnet_normalized[2] }, QuaternConj(q)));
            double[] b = { 0, Math.Sqrt(h[1] * h[1] + h[2] * h[2]), 0, h[3] }; // Reference vector

            E_d = new double[] { b[1], b[2], b[3] };
            S_s = Magnet_normalized;

            double[,] F_Magnet = func_F(q, E_d, S_s);
            double[,] Delta_Magnet = J(q, E_d);

            double[,] F = MatrixConcatenate(F_Accel, F_Magnet);
            double[,] Delta = MatrixConcatenate(Delta_Accel, Delta_Magnet);

            // Measurement step calculation
            double[,] step = (MatrixMultiply(MatrixTranspose(Delta), F));
            double normStep = Math.Sqrt(step[0, 0] * step[0, 0] + step[1, 0] * step[1, 0] + step[2, 0] * step[2, 0]);
            for (int i = 0; i < 3; i++)
            {
                step[i, 0] /= normStep;
            }
            //var tstep = MatrixTranspose(step);
            double[] step2 = new double[4];
            step2[0] = step[0, 0];
            step2[1] = step[1, 0];
            step2[2] = step[2, 0];

            // Quaternion update
            double[] qDot = VectorSubtract(VectorScale(QuaternProd(q, new double[] { 0, Gyroscope[0], Gyroscope[1], Gyroscope[2] }), 0.5),
                VectorScale(step2, beta));

            // Measurement update
            for (int i = 0; i < 4; i++)
            {
                q[i] += qDot[i] * SamplePeriod;
            }

            // Normalization
            double qNorm = Math.Sqrt(q[0] * q[0] + q[1] * q[1] + q[2] * q[2] + q[3] * q[3]);
            for (int i = 0; i < 4; i++)
            {
                q[i] /= qNorm;
            }

            return q;
        }

        // Helper functions for matrix and vector operations
        public static double[,] MatrixMultiply(double[,] a, double[,] b)
        {
            int rowsA = a.GetLength(0);
            int colsA = a.GetLength(1);
            int colsB = b.GetLength(1);
            double[,] result = new double[rowsA, colsB];

            for (int i = 0; i < rowsA; ++i)
            {
                for (int j = 0; j < colsB; ++j)
                {
                    for (int k = 0; k < colsA; ++k)
                    {
                        result[i, j] += a[i, k] * b[k, j];
                    }
                }
            }

            return result;
        }

        public static double[,] MatrixAdd(double[,] a, double[,] b)
        {
            int rowsA = a.GetLength(0);
            int colsA = a.GetLength(1);
            double[,] result = new double[rowsA, colsA];

            for (int i = 0; i < rowsA; ++i)
            {
                for (int j = 0; j < colsA; ++j)
                {
                    result[i, j] = a[i, j] + b[i, j];
                }
            }

            return result;
        }
        public static double[,] MatrixScale(double[,] a, double s)
        {
            int rowsA = a.GetLength(0);
            int colsA = a.GetLength(1);
            double[,] result = new double[rowsA, colsA];

            for (int i = 0; i < rowsA; ++i)
            {
                for (int j = 0; j < colsA; ++j)
                {
                    result[i, j] = a[i, j] * s;
                }
            }

            return result;
        }

        public static double[,] MatrixTranspose(double[,] matrix)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);
            double[,] result = new double[cols, rows];

            for (int i = 0; i < cols; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    result[i, j] = matrix[j, i];
                }
            }

            return result;
        }

        public static double[,] MatrixConcatenate(double[,] a, double[,] b)
        {
            int rowsA = a.GetLength(0);
            int colsA = a.GetLength(1);
            int rowsB = b.GetLength(0);
            int colsB = b.GetLength(1);

            if (rowsA != rowsB)
            {
                throw new ArgumentException("Rows of matrices do not match.");
            }

            double[,] result = new double[rowsA, colsA + colsB];

            for (int i = 0; i < rowsA; ++i)
            {
                for (int j = 0; j < colsA + colsB; ++j)
                {
                    if (j < colsA)
                    {
                        result[i, j] = a[i, j];
                    }
                    else
                    {
                        result[i, j] = b[i, j - colsA];
                    }
                }
            }

            return result;
        }

        public static double[] VectorScale(double[] vector, double scalar)
        {
            int length = vector.Length;
            double[] result = new double[length];

            for (int i = 0; i < length; ++i)
            {
                result[i] = vector[i] * scalar;
            }

            return result;
        }

        public static double[] VectorSubtract(double[] a, double[] b)
        {
            int lengthA = a.Length;
            int lengthB = b.Length;
            if (lengthA != lengthB)
            {
                throw new ArgumentException("Vector dimensions do not match.");
            }

            double[] result = new double[lengthA];
            for (int i = 0; i < lengthA; ++i)
            {
                result[i] = a[i] - b[i];
            }

            return result;
        }
    }
}