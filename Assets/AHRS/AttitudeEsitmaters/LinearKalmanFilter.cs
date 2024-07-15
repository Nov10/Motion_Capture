using AHRS;

namespace AHRS
{
    [System.Serializable ]
    public class LinearKalmanFilter
    {
        public Vector2 x = new Vector2();
        public Matrix2x2 A;// = new Matrix2x2();
        public Vector2 B;

        public Matrix2x2 P = new Matrix2x2(0, 0, 0, 0);
        public Matrix2x2 Q = new Matrix2x2(0.001, 0, 
                                           0, 0.003);

        public Matrix1x2 H = new Matrix1x2(1, 0);


        public double R = 0.03;

        public LinearKalmanFilter(double angle, double bias, double r)
        {
            Q = new Matrix2x2(angle, 0, 0, bias);
            R = r;
        }

        public double Update(double z, double gyro, double dt)
        {
            A = new Matrix2x2(1, -dt, 
                              0, 1);
            B = new Vector2(dt, 0);

            //����
            Vector2 x_estimated = A * x + B * gyro;
            Matrix2x2 P_estimated = A * P * A.Transpose() + (Q * dt);

            //������(���ӵ� ����)���� ����
            double y = z - H * x_estimated;
            double S = H * P_estimated * H.Transpose() + R;
            //Į�� ���� ���
            Vector2 K = P_estimated * H.Transpose() / S;

            //����ġ ���
            x = x_estimated + K * y;
            //���� ���л� ���
            P = (Matrix2x2.Identity() - new Matrix2x2(K.x, 0, K.y, 0)) * P_estimated;

            return x.x;
        }
        
    }
}