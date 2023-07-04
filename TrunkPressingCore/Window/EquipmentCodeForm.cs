using HZH_Controls.Forms;
using Newtonsoft.Json;
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
using TrunkPressingCore.GameModel;
using TrunkPressingCore.GameSystem;
using TrunkPressingCore.SQLite;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace TrunkPressingCore.Window
{
    public partial class EquipmentCodeForm : Form
    {
        public EquipmentCodeForm()
        {
            InitializeComponent();
        }
        AutoWindowSize AutoWindowSize = new AutoWindowSize();

        public Dictionary<string, string> localValues = new Dictionary<string, string>();
        //数据库
        public SQLiteHelper sQLiteHelper = null;
        public List<Dictionary<string, string>> localInfos = new List<Dictionary<string, string>>();
        private void EquipmentCodeForm_Load(object sender, EventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            AutoWindowSize.ControlInitializeSize(this);
            localInfos = sQLiteHelper.ExecuteReaderList("SELECT * FROM localInfos");
            if (localInfos.Count > 0)
            {
                string MachineCode = String.Empty;
                string ExamId = String.Empty;
                string Platform = String.Empty;
                string Platforms = String.Empty;
                int UploadUnit = 0;
                foreach (var item in localInfos)
                {
                    localValues.Add(item["key"], item["value"]);
                    switch (item["key"])
                    {
                        case "MachineCode":
                            MachineCode = item["value"];
                            break;
                        case "ExamId":
                            ExamId = item["value"];
                            break;
                        case "Platform":
                            Platform = item["value"];
                            break;
                        case "Platforms":
                            Platforms = item["value"];
                            break;
                        case "UploadUnit":
                            int.TryParse(item["value"], out UploadUnit);
                            break;
                    }
                }

                if (string.IsNullOrEmpty(MachineCode))
                {
                    FrmTips.ShowTipsError(this, "设备码为空");
                }
                else
                {
                    comboBox1.Text = MachineCode;
                }


                if (string.IsNullOrEmpty(ExamId))
                {
                    FrmTips.ShowTipsError(this, "考试id为空");
                }
                else
                {
                    comboBox3.Text = ExamId;
                }
                if (string.IsNullOrEmpty(Platforms))
                {
                    FrmTips.ShowTipsError(this, "平台码为空");
                }
                else
                {
                    string[] Platformss = Platforms.Split(';');
                    comboBox2.Items.Clear();
                    foreach (var item in Platformss)
                    {
                        comboBox2.Items.Add(item);
                    }

                }
                if (string.IsNullOrEmpty(Platform))
                {
                    FrmTips.ShowTipsError(this, "平台码为空");
                }
                else
                {
                    comboBox2.Text = Platform;
                }
                comboBox4.SelectedIndex = UploadUnit;

            }
        }
        /// <summary>
        /// 获取考试id
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton1_Click(object sender, EventArgs e)
        {
            comboBox3.Items.Clear();
            string url = comboBox2.Text;
            if (string.IsNullOrEmpty(url))
            {
                FrmTips.ShowTipsError(this, "网址为空!");
                return;
            }
            url += RequestUrl.GetExamListUrl;
            RequestParameter RequestParameter = new RequestParameter();
            RequestParameter.AdminUserName = localValues["AdminUserName"];
            RequestParameter.TestManUserName = localValues["TestManUserName"];
            RequestParameter.TestManPassword = localValues["TestManPassword"];

            //序列化
            string JsonStr = Newtonsoft.Json.JsonConvert.SerializeObject(RequestParameter);

            // string url = localValues["Platform"] + RequestUrl.GetExamListUrl;

            var formDatas = new List<FromDataItemModel>();
            //添加其他字段
            formDatas.Add(new FromDataItemModel()
            {
                key = "data",
                value= JsonStr
            });
            var httpUpload = new HttpUpload();
            string result = HttpUpload.PostFromData(url, formDatas);
            GetExamList upload_Result = JsonConvert.DeserializeObject<GetExamList>(result);

            if (upload_Result.Results.Count == 0)
            {
                FrmTips.ShowTipsError(this, $"提交错误,错误码:[{upload_Result.Error}]");
                return;
            }

            foreach (var item in upload_Result.Results)
            {
                string str = $"{item.title}_{item.exam_id}";
                comboBox3.Items.Add(str);
            }
            UIMessageBox.ShowSuccess("获取成功！！");
            comboBox3.SelectedIndex = 0;
        }
        /// <summary>
        /// 获取机器码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton2_Click(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();

            string examId = comboBox3.Text;
            if (string.IsNullOrEmpty(examId))
            {
                FrmTips.ShowTipsError(this, "考试id为空!");
                return;
            }
            if (examId.IndexOf('_') != -1)
            {
                examId = examId.Substring(examId.IndexOf('_') + 1);
            }
            string url = comboBox2.Text;
            if (string.IsNullOrEmpty(url))
            {
                FrmTips.ShowTipsError(this, "网址为空!");
                return;
            }
            url += RequestUrl.GetMachineCodeListUrl;

            RequestParameter RequestParameter = new RequestParameter();
            RequestParameter.AdminUserName = localValues["AdminUserName"];
            RequestParameter.TestManUserName = localValues["TestManUserName"];
            RequestParameter.TestManPassword = localValues["TestManPassword"];
            RequestParameter.ExamId = examId;
            //序列化
            string JsonStr = Newtonsoft.Json.JsonConvert.SerializeObject(RequestParameter);

            var formDatas = new List<FromDataItemModel>();
            //添加其他字段
            formDatas.Add(new FromDataItemModel()
            {
                key = "data",
                value= JsonStr
            });
            var httpUpload = new HttpUpload();
            string result = HttpUpload.PostFromData(url, formDatas);
            GetMachineCodeList upload_Result = JsonConvert.DeserializeObject<GetMachineCodeList>(result);
            if (upload_Result.Results.Count == 0)
            {
                FrmTips.ShowTipsError(this, $"提交错误,错误码:[{upload_Result.Error}]");
                return;
            }

            foreach (var item in upload_Result.Results)
            {
                string str = $"{item.title}_{item.MachineCode}";
                comboBox1.Items.Add(str);

            }
            UIMessageBox.ShowSuccess("获取成功！！");
            comboBox1.SelectedIndex = 0;
        }

        private void uiButton3_Click(object sender, EventArgs e)
        {
            try
            {
                string Platform = comboBox2.Text;
                string ExamId = comboBox3.Text;
                string MachineCode = comboBox1.Text;
                int UploadUnit = comboBox4.SelectedIndex;
                System.Data.SQLite.SQLiteTransaction sQLiteTransaction = sQLiteHelper.BeginTransaction();
                sQLiteHelper.ExecuteNonQuery($"UPDATE localInfos SET value = '{Platform}' WHERE key = 'Platform'");
                sQLiteHelper.ExecuteNonQuery($"UPDATE localInfos SET value = '{ExamId}' WHERE key = 'ExamId'");
                sQLiteHelper.ExecuteNonQuery($"UPDATE localInfos SET value = '{MachineCode}' WHERE key = 'MachineCode'");
                sQLiteHelper.ExecuteNonQuery($"UPDATE localInfos SET value = '{UploadUnit}' WHERE key = 'UploadUnit'");
                sQLiteHelper. CommitTransAction(sQLiteTransaction);
                FrmTips.ShowTipsSuccess(this, "保存成功");
                this.Close();
            }
            catch (Exception ex)
            {

                LoggerHelper.Debug(ex);
                FrmTips.ShowTipsError(this, "保存失败");
            }
            this.Close();
        }

        private void uiButton4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void EquipmentCodeForm_SizeChanged(object sender, EventArgs e)
        {
            AutoWindowSize.ControlAutoSize(this);
        }
    }
}
