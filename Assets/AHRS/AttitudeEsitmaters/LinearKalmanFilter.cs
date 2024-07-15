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

            //예측
            Vector2 x_estimated = A * x + B * gyro;
            Matrix2x2 P_estimated = A * P * A.Transpose() + (Q * dt);

            //축정값(가속도 센서)과의 잔차
            double y = z - H * x_estimated;
            double S = H * P_estimated * H.Transpose() + R;
            //칼만 게인 계산
            Vector2 K = P_estimated * H.Transpose() / S;

            //추정치 계산
            x = x_estimated + K * y;
            //오차 공분산 계산
            P = (Matrix2x2.Identity() - new Matrix2x2(K.x, 0, K.y, 0)) * P_estimated;

            return x.x;
        }
        
    }
}