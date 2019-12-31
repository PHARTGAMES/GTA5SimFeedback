using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTAVTelemetry
{
    public class NoiseFilter
    {
        private float[] samples;
        private int maxSampleCount;
        private int liveSampleCount;
        private int currSample = 0;

        public NoiseFilter(int _maxSampleCount)
        {
            maxSampleCount = Math.Max(1, _maxSampleCount);
            samples = new float[maxSampleCount];
        }

        public float Filter(float sample)
        {
            samples[currSample] = sample;

            liveSampleCount = (liveSampleCount + 1) >= maxSampleCount ? maxSampleCount : liveSampleCount + 1;


            float total = 0.0f;
            int index = (currSample - (liveSampleCount-1)) < 0 ? maxSampleCount - Math.Abs(currSample - liveSampleCount) : currSample - (liveSampleCount - 1);
            for (int i = 0; i < liveSampleCount; ++i)
            {
                total += samples[index];

                index = (index + 1) >= maxSampleCount ? 0 : index + 1;
            }

            currSample = (currSample + 1) >= maxSampleCount ? 0 : currSample + 1;

            return total / liveSampleCount;
        }

        public void Reset()
        {
            currSample = 0;
            liveSampleCount = 0;
        }
    }

    public class KalmanFilter
    {
        private float A, H, Q, R, P, x;

        public KalmanFilter(float A, float H, float Q, float R, float initial_P, float initial_x)
        {
            this.A = A;
            this.H = H;
            this.Q = Q;
            this.R = R;
            this.P = initial_P;
            this.x = initial_x;
        }

        public float Filter(float input)
        {
            // time update - prediction
            x = A * x;
            P = A * P * A + Q;

            // measurement update - correction
            float K = P * H / (H * P * H + R);
            x = x + K * (input - H * x);
            P = (1 - K * H) * P;

            return x;
        }
    }
}
