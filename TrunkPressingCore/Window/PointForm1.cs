using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TrunkPressingCore
{
    public partial class PointForm1 : Form
    {
        public PointForm1()
        {
            InitializeComponent();
        }
        //GetActiveWindow返回线程的活动窗口，而不是系统的活动窗口。如果要得到用户正在激活的窗口，应该使用 GetForegroundWindow
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll ")]
        //设置窗体置顶
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        //窗体句柄
        private IntPtr handle = IntPtr.Zero;
        public IntPtr Handle { get => handle; set => handle = value; }

        #region   窗体在最前
        [DllImport("user32")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32")]
        private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
        #endregion

        public int col = 5;
        PictureBox[] ps1;
        PictureBox[] ps2;   
        private void PointForm1_Load(object sender, EventArgs e)
        {
            this.TopMost = true;
            InitPic();
        }

        private void InitPic()
        {
            int wid = flp1.Width / col - 10;
            int hei = flp1.Height   - 10;
            flp1.SuspendLayout();
            flp2 .SuspendLayout();
            flp1.Controls.Clear();
            flp2.Controls.Clear();
            ps1= new PictureBox[col];
            ps2 = new PictureBox[col];
            for(int i = 0; i < col; i++)
            {
                ps1[i] = new PictureBox();
                ps1[i].Size = new System .Drawing.Size(wid, hei);
                ps1[i].BackColor = Color.Blue;
                ps2[i] = new PictureBox();
                ps2[i].Size = new System.Drawing.Size(wid, hei);
                ps2[i].BackColor = Color.Blue;

            }
            flp1.Controls .AddRange (ps1);
            flp2.Controls .AddRange (ps2);
            flp1 .ResumeLayout (false);
            flp2.ResumeLayout (false);
        }
        public void  UpdateFlp(int index)
        {
            InitPic();
            if (index % 2 == 0)
            {
                //下标点
               int indexs  = (index-2 )/ 2;
                ps2[indexs].BackColor = Color.Red;

            }
            else
            {
                int indexs = (index - 1) / 2;
                ps1 [indexs].BackColor = Color.Red;

            }
            flp1.Controls.AddRange(ps1);
            flp2.Controls.AddRange(ps2);
            flp1.ResumeLayout(false);
            flp2.ResumeLayout(false);
        }
    }
}
