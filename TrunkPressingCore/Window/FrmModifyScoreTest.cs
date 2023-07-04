using NPOI.SS.Formula.Functions;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrunkPressingCore.GameSystem;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace TrunkPressingCore.Window
{
    public partial class FrmModifyScoreTest : Form
    {
        public FrmModifyScoreTest()
        {
            InitializeComponent();
        }
        public  string  projectName { get; set; }
        public  string groupName { get; set; }
        public  string sName { get; set; }
        public  string IdNumber { get;set; }
        public  string status { get;set;}
        public  int rountid { get; set; }   

        private void uiButton1_Click(object sender, EventArgs e)
        {
            DialogResult=DialogResult.OK;
            this.Close();
        }
        AutoWindowSize AutoWindowSize = new AutoWindowSize ();
        private void FrmModifyScoreTest_Load(object sender, EventArgs e)
        {
            AutoWindowSize.ControlInitializeSize(this);
            this.Title.Text = "修改成绩";
            ProjectNameInput.Text = projectName;
            GroupText.Text = groupName;
            NameText.Text = sName;
            comboBox1.Items.Clear();
            for(int i = 0; i < rountid; i++)
            {
                comboBox1.Items.Add($"第{i+1} 轮");
            }
            if(comboBox2.Items.Contains(status))
            {
                comboBox2.SelectedIndex = comboBox2.Items.IndexOf(status);
            }
        }
        public int updaterountId = 0;

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex != -1)
            {
                updaterountId = comboBox1.SelectedIndex + 1;
            }
        }
        public double updateScore = 0;
        private void uiTextBox1_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(uiTextBox1.Text, out updateScore);
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            status = comboBox2.Text.ToString();
        }

        private void uiButton2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void FrmModifyScoreTest_SizeChanged(object sender, EventArgs e)
        {

        }
    }
}
