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

namespace TrunkPressingCore.Window
{
    public partial class FrmCreateNewProject : HZH_Controls.Forms.FrmWithOKCancel1
    {
        public string ProjectName = "";
        AutoWindowSize AutoWindowSize = new AutoWindowSize();
        public FrmCreateNewProject()
        {
            InitializeComponent();
        }

        private void FrmCreateNewProject_Load(object sender, EventArgs e)
        {
            this.Title = "创建项目";
            AutoWindowSize.ControlInitializeSize(this);
        }
        public void FrmCreateNewProject_SizeChange(object sender, EventArgs e)
        {
            //AutoWindowSize.ControlAutoSize (this);
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            ProjectName = textBox1.Text;
        }
    }
}
