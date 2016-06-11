using GA;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace incenterGA
{
    class calcGA : GAFrame
    {

        Stack<Point> pointStk;
        Stack<Point> undoStk;
        Graphics g;
        private double bestfit;
        public double best
        {
            get
            {
                return bestfit;
            }
        }
        protected override int m { get; set; }

        protected override int len { get; set; }

        public calcGA(Graphics _g)
        {
            pointStk = new Stack<Point>();
            undoStk = new Stack<Point>();
            g = _g;
            bestfit = double.MinValue;
            m = 4;
            len = 24;
            pc = 50;
            pm = 0.05;
            DNA = new byte[m][];
            DNA[0] = para2DNA(10, 10);
            DNA[1] = para2DNA(10, 100);
            DNA[2] = para2DNA(100, 10);
            DNA[3] = para2DNA(100, 100);
        }

        public void pushPoint(Point p)
        {
            undoStk.Clear();
            pointStk.Push(p);
        }

        public Point undo()
        {
            Point p = pointStk.Pop();
            undoStk.Push(p);
            return p;
        }

        public Point redo()
        {
            Point p = undoStk.Pop();
            pointStk.Push(p);
            return p;
        }

        public bool pointIsEmpty()
        {
            return pointStk.Count == 0;
        }

        public bool undoIsEmpty()
        {
            return undoStk.Count == 0;
        }

        public int pointSize()
        {
            return pointStk.Count;
        }

        public void clear()
        {
            pointStk.Clear();
            undoStk.Clear();
            bestfit = 0;
        }

        public override double fitness(byte[] DNA)
        {
            int x, y;
            DNA2para(DNA, out x, out y);
            searchPt(x, y);
            //System.Windows.Forms.MessageBox.Show(x.ToString() + " " + y.ToString());
            double deg = 0;
            var ptList = pointStk.ToArray();
            double fit = double.MaxValue;
            for (int i = 1; i <= ptList.Length; i++)
            {
                double dist = ptLineDist(x, y, ptList[i - 1].X, ptList[i - 1].Y, ptList[i % ptList.Length].X, ptList[i % ptList.Length].Y);
                deg += degree(x, y, ptList[i - 1].X, ptList[i - 1].Y, ptList[i % ptList.Length].X, ptList[i % ptList.Length].Y);
                fit = Math.Min(fit, Math.Abs(dist));
            }
            if (Math.Abs(deg) < 1e-6 || double.IsNaN(deg)) fit *= -1;
            else if (fit > bestfit)
            {
                drawCircle(x, y, fit);
                bestfit = fit;
            }
            return fit;
        }

        public override void crosspop(byte[] DNA1, byte[] DNA2, out byte[] DNA1_, out byte[] DNA2_)
        {
            DNA1_ = DNA1;
            DNA2_ = DNA2;
            for (int i = 10; i < len / 2; i++)
            {
                swap(DNA1_[i], DNA2_[i]);
                swap(DNA1_[i + len / 2], DNA2_[i + len / 2]);
            }

        }

        private void swap(byte a, byte b)
        {
            a ^= b ^= a ^= b;
        }

        /// <summary>
        /// 求(u,v)到由(x0,y0)(x1,y1)组成的直线的距离
        /// </summary>
        /// <param name="u"></param>
        /// <param name="v"></param>
        /// <param name="x0"></param>
        /// <param name="y0"></param>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <returns></returns>
        private double ptLineDist(int u, int v, int x0, int y0, int x1, int y1)
        {
            if (!ptInline(u, v, x0, y0, x1, y1))
            {
                return Math.Min(eulerDist(u, v, x0, y0), eulerDist(u, v, x1, y1));
            }
            int a = y1 - y0;
            int b = x0 - x1;
            int c = x1 * y0 - x0 * y1;
            int up = a * u + b * v + c;
            double down = Math.Sqrt(a * a + b * b);
            return up / down;
        }

        /// <summary>
        /// 判断(u,v)到由(x0,y0)(x1,y1)组成的直线的垂心是否落在这条线段上
        /// </summary>
        /// <param name="u"></param>
        /// <param name="v"></param>
        /// <param name="x0"></param>
        /// <param name="y0"></param>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <returns></returns>
        private bool ptInline(int u, int v, int x0, int y0, int x1, int y1)
        {
            double ab = eulerDist(u, v, x0, y0);
            double ac = eulerDist(u, v, x1, y1);
            double bc = eulerDist(x0, y0, x1, y1);
            double b = Math.Acos((ab * ab + bc * bc - ac * ac) / (2 * ab * bc));
            double c = Math.Acos((bc * bc + ac * ac - ab * ab) / (2 * bc * ac));
            return b <= Math.PI / 2 && c <= Math.PI / 2;
        }

        /// <summary>
        /// 将DNA转化为参数
        /// </summary>
        /// <param name="DNA"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void DNA2para(byte[] DNA, out int x, out int y)
        {
            x = 0;
            y = 0;
            for (int i = 0; i < len / 2; i++)
            {
                x <<= 1;
                x |= DNA[i];
            }
            for (int i = len / 2; i < len; i++)
            {
                y <<= 1;
                y |= DNA[i];
            }
        }

        /// <summary>
        /// 将参数转化为DNA
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private byte[] para2DNA(int x, int y)
        {
            byte[] dna = new byte[len];
            for (int i = 0; i < len / 2; i++)
            {
                dna[len / 2 - i - 1] = (byte)(x & 1);
                x >>= 1;
            }
            for (int i = len - 1; i >= len / 2; i--)
            {
                dna[i] = (byte)(y & 1);
                y >>= 1;
            }
            return dna;
        }

        private double degree(int u, int v, int x0, int y0, int x1, int y1)
        {
            double a = eulerDist(u, v, x0, y0);
            double b = eulerDist(u, v, x1, y1);
            double c = eulerDist(x0, y0, x1, y1);
            return Math.Acos((a * a + b * b - c * c) / (2 * a * b)) * direction(u, v, x0, y0, x1, y1);
        }

        private double eulerDist(int x0, int y0, int x1, int y1)
        {
            int dx = x0 - x1;
            int dy = y0 - y1;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        private int direction(int x0, int y0, int x1, int y1, int x2, int y2)
        {
            return (x2 - x0) * (y1 - y0) - (x1 - x0) * (y2 - y0) < 0 ? 1 : -1;
        }

        private void drawCircle(int x, int y, double r)
        {
            int rr = (int)r;
            g.FillEllipse(new SolidBrush(Color.Red), new Rectangle(x - 4, y - 4, 6, 6));
            g.DrawEllipse(new Pen(Color.Blue), new Rectangle(x - rr, y - rr, rr * 2, rr * 2));
        }

        private void searchPt(int x, int y)
        {
            g.DrawEllipse(new Pen(Color.Gray), new Rectangle(x - 1, y - 1, 2, 2));
        }
    }
}
