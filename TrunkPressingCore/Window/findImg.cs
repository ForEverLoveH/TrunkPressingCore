﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrunkPressingCore.GameModel;
using TrunkPressingCore.GameSystem;

namespace TrunkPressingCore.Window
{
    public partial class findImg : Form
    {
        public findImg()
        {
            InitializeComponent();
        }
        AutoWindowSize AutoWindowSize = new AutoWindowSize();
        public string fileName = "";
        public string nowTestDir = "";
        FlowLayoutPanel flp = new FlowLayoutPanel();
        //public List<Bitmap> BmpGroup;
        // public Bitmap[] BmpGroup;
        public List<RunTestWindow.imgMsS> imgMs = new List<RunTestWindow.imgMsS>();
        private void findImg_Load(object sender, EventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            AutoWindowSize.ControlInitializeSize(this);
            if(imgMs.Count <1)
            {
                MessageBox.Show("录制不到相片");
                return;
            }
            Controls.Add(flp);
            flp.Dock = DockStyle.Fill;
            flp.AutoScroll = true;


            int len = 0;
            if (imgMs.Count > 0)
            {
                flp.SuspendLayout();
                int width = flp.Width / 5 - 10;
                int height = flp.Height / 5 - 10;
                int n = imgMs.Count;
                int interval = n / 20;
                PictureBox[] pics = new PictureBox[imgMs.Count];
                int count = 0;
                for (int i = pics.Length - 1; i > 0; i--)
                {
                    if (i % interval != 0) continue;
                    pics[i] = new PictureBox();
                    pics[i].Image = imgMs[i].img;//Image.FromHbitmap(BmpGroup[i].GetHbitmap()); //global::SkipExec.Properties.Resources.face; //BmpGroup[i];
                    pics[i].Size = new System.Drawing.Size(width, height);
                    pics[i].SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;

                    pics[i].Name = i + "";
                    pics[i].Click += new System.EventHandler(this.pictureBox1_Click);
                    count++;
                    if (count >= 20) break;

                }

                flp.Controls.AddRange(pics);
                flp.ResumeLayout();
            }


        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            fileName = (sender as PictureBox).Name;
            this.DialogResult = DialogResult.OK;

        }

        private void findImg_SizeChanged(object sender, EventArgs e)
        {
            AutoWindowSize.ControlAutoSize(this);
        }
    }
    
}
