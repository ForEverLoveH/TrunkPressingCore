using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrunkPressingCore.SQLite;

namespace TrunkPressingCore.Window
{
    public partial class TurnsSetForm : Form
    {
        public TurnsSetForm()
        {
            InitializeComponent();
        }
        public string projectId = "";
        public string projectName = "";

        public SQLiteHelper sQLiteHelper = null;


        private void TurnsSetForm_Load(object sender, EventArgs e)
        {
            var ds = sQLiteHelper.ExecuteReaderList($"SELECT Name,TurnsNumber0,TurnsNumber1 FROM SportProjectInfos WHERE Id='{projectId}';");
            for (int i = 0; i < ds.Count; i++)
            {
                Dictionary<string, string> data = ds[i];
                uiTextBox1.Text = data["Name"];
                textBox2.Text = data["TurnsNumber0"];
                textBox3.Text = data["TurnsNumber1"];
                break;
            }
        }

        private void uiButton1_Click(object sender, EventArgs e)
        {
            sQLiteHelper.ExecuteNonQuery($"UPDATE SportProjectInfos SET TurnsNumber0={textBox2.Text},TurnsNumber1={textBox3.Text} WHERE Id='{projectId}';");
            DialogResult = DialogResult.OK;
        }

        private void uiButton2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
