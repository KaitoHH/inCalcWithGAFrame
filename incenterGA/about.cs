using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace incenterGA
{
    public partial class about : Form
    {
        public about()
        {
            InitializeComponent();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            DialogResult dr = MessageBox.Show(this, "即将打开源项目链接", "提示", MessageBoxButtons.OKCancel);
            if (dr == DialogResult.OK)
            {
                System.Diagnostics.Process.Start("https://github.com/KaitoHH/inCalcWithGAFrame");
            }
        }
    }
}
