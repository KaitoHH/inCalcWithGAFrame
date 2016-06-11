using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace incenterGA
{
    public partial class Form1 : Form
    {
        calcGA GA;
        Point lastPt;
        Point firstPt;
        Thread calc;
        int iterCnt;
        int showCnt = 0;
        bool threadStart;
        bool candraw = true;
        private static Color[] colorList = new Color[]
        {
            Color.Tomato,Color.SpringGreen,Color.DarkViolet
        };
        public Form1()
        {
            InitializeComponent();
            GA = new calcGA(board.CreateGraphics());
            updStat();
            iterCnt = 0;
        }

        private void addPoint(object sender, EventArgs e)
        {
            if (!candraw)
            {
                return;
            }
            MouseEventArgs mouse = (MouseEventArgs)e;
            Point mpt = mouse.Location;
            var g = board.CreateGraphics();
            if (GA.pointSize() > 2 && manDist(firstPt, mpt) <= 10)
            {
                MessageBox.Show("画图结束\n点击开始计算以开始迭代");
                candraw = false;
                mpt = firstPt;
            }
            else
            {
                GA.pushPoint(mouse.Location);
                updStat();
                g.FillEllipse(new SolidBrush(Color.Black), new Rectangle(mpt.X - 4, mpt.Y - 4, 6, 6));
            }
            if (GA.pointSize() > 1)
            {
                g.DrawLine(new Pen(Color.Black), lastPt, mpt);
            }
            else
            {
                firstPt = mpt;
            }
            lastPt = mpt;

        }

        private void updStat()
        {
            labelCnt.Text = GA.pointSize().ToString();
            bestfit.Text = GA.best.ToString();
            iteration.Text = iterCnt.ToString();
            ptBest.Text = "X: " + GA.bestX + " Y: " + GA.bestY;
            improveCnt.Text = GA.improvCnt.ToString();
        }

        private void undo(object sender, EventArgs e)
        {
            GA.undo();
            updStat();
        }

        private void redo(object sender, EventArgs e)
        {
            GA.redo();
            updStat();
        }

        private void clear(object sender, EventArgs e)
        {
            threadStart = false;
            board.CreateGraphics().Clear(Color.White);
            GA = new calcGA(board.CreateGraphics());
            iterCnt = 0;
            showCnt = 0;
            candraw = true;
            updStat();
        }

        private void start(object sender, EventArgs e)
        {
            calc = new Thread(start_envolve);
            threadStart = true;
            calc.Start();
        }

        private void start_envolve()
        {
            while (threadStart)
            {
                iterCnt++;
                GA.envolve();
                Invoke(new MethodInvoker(delegate ()
                {
                    updStat();
                }));
            }
        }
        private int manDist(Point x, Point y)
        {
            return Math.Abs(x.X - y.X) + Math.Abs(x.Y - y.Y);
        }

        private void stop(object sender, EventArgs e)
        {
            threadStart = false;
        }

        private void board_Resize(object sender, EventArgs e)
        {
            clear(sender, e);
        }

        private void showPt(object sender, EventArgs e)
        {
            var g = board.CreateGraphics();
            g.DrawEllipse(new Pen(colorList[showCnt++ % colorList.Length], 5),
                new Rectangle((int)(GA.bestX - GA.best), (int)(GA.bestY - GA.best), (int)(GA.best * 2), (int)(GA.best * 2)));
        }

        private void about(object sender, EventArgs e)
        {
            about form = new about();
            form.ShowDialog();
        }
    }
}
