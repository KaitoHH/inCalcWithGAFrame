using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GA
{
    abstract class GAFrame
    {
        private Random ran;

        /// <summary>
        /// 交叉率
        /// </summary>
        protected double pc { get; set; }

        /// <summary>
        /// 变异率
        /// </summary>
        protected double pm { get; set; }

        /// <summary>
        /// 种群规模
        /// </summary>
        abstract protected int m { get; set; }

        /// <summary>
        /// DNA长度
        /// </summary>
        abstract protected int len { get; set; }

        /// <summary>
        /// 目标适应度
        /// </summary>
        protected double targetFitness { get; set; }

        protected byte[][] DNA;

        protected double[] f;

        public GAFrame()
        {
            ran = new Random();
            pc = 0.5;
            pm = 0.001;
        }

        public abstract double fitness(byte[] DNA);

        public abstract void crosspop(byte[] DNA1, byte[] DNA2, out byte[] DNA1_, out byte[] DNA2_);

        /// <summary>
        /// 一次演变计算
        /// </summary>
        public void envolve()
        {
            double sum = 0;
            double best = double.MinValue;
            double negmax = 0;
            int bp = -1;

            f = new double[m];
            // 计算适应度
            for (int i = 0; i < m; i++)
            {
                f[i] = fitness(DNA[i]);
                if (f[i] < 0)
                {
                    negmax = Math.Min(negmax, f[i]);
                }
                sum += f[i];
                if (f[i] > best)
                {
                    best = f[i];
                    bp = i;
                }
            }
            // 负值修正
            if (negmax < 0)
            {
                sum = 0;
                for (int i = 0; i < m; i++)
                {
                    f[i] -= negmax;
                    sum += f[i];
                }
            }

            // 累计概率
            double[] q = new double[m];
            q[0] = f[0] / sum;
            for (int i = 1; i < m; i++)
            {
                q[i] = q[i - 1] + f[i] / sum;
            }

            // 选择复制
            byte[][] _DNA = new byte[m][];
            for (int i = 0; i < m; i++)
            {
                _DNA[i] = new byte[len];
                double r = ran.NextDouble();
                int loc = indexOf(q, r);
                //_DNA[i] = DNA[loc];
                Array.Copy(DNA[loc], _DNA[i], len);
            }

            // 交叉
            for (int i = 0; i < m * pc; i++)
            {
                int x = ran.Next(m);
                int y = ran.Next(m);
                byte[] d1, d2;
                crosspop(_DNA[x], _DNA[y], out d1, out d2);
                _DNA[x] = d1;
                _DNA[y] = d2;
            }

            // 变异
            for (int i = 0; i < m * len * pm; i++)
            {
                int x;
                do
                {
                    x = ran.Next(m);
                } while (x == bp);
                int y = ran.Next(len);
                _DNA[x][y] = (byte)(1 - _DNA[x][y]);
            }
            Array.Copy(DNA[bp], _DNA[bp], len); // 保留最佳
            DNA = _DNA;
        }

        private int indexOf(double[] array, double r)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] > r)
                {
                    return i;
                }
            }
            return 0;
        }
    }
}
