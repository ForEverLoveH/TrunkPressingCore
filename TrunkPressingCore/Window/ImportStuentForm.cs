using HZH_Controls.Forms;
using MiniExcelLibs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrunkPressingCore.GameModel;
using TrunkPressingCore.SQLite;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using TrunkPressingCore.GameSystem;
using System.Threading;
using Sunny.UI;

namespace TrunkPressingCore.Window
{
    public partial class ImportStuentForm : Form
    {
        public ImportStuentForm()
        {
            InitializeComponent();
        }
        public SQLiteHelper sQLiteHelper;
        public List<Dictionary<string, string>> localInfos  = null;
        public Dictionary<string, string> localValues = new Dictionary<string, string>();
        public string _projectName = string.Empty;
        int proVal = 0;
        int proMax = 0;
        /// <summary>
        /// 数据库备份
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton1_Click(object sender, EventArgs e)
        {
            sQLiteHelper.ExportSQLiteDb();
            FrmTips.ShowTipsSuccess(this, "数据库备份成功");
        }

        private void ImportStuentForm_Load(object sender, EventArgs e)
        {
            UpDateLocalInfo();
        }

        private void UpDateLocalInfo()
        {
            localInfos = new List<Dictionary<string, string>>();
            localInfos = sQLiteHelper.ExecuteReaderList("SELECT * FROM localInfos");
            if (localInfos.Count > 0)
            {
                localValues = new Dictionary<string, string>();
                foreach (var item in localInfos)
                {
                    localValues.Add(item["key"], item["value"]);
                }
            };
        }
        /// <summary>
        ///  数据库清空
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton2_Click(object sender, EventArgs e)
        {
            sQLiteHelper.InitSQLiteDB();
            FrmTips.ShowTipsSuccess(this, "初始化数据库成功");
        }
        /// <summary>
        /// 模板导出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton3_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveImageDialog = new SaveFileDialog();
            saveImageDialog.Filter = "xls files(*.xls)|*.xls|xlsx file(*.xlsx)|*.xlsx|All files(*.*)|*.*";
            saveImageDialog.RestoreDirectory = true;
            saveImageDialog.FileName = $"导出模板{DateTime.Now.ToString("yyyyMMddHHmmss")}.xls";
            string path = Application.StartupPath + "\\excel\\output.xlsx";

            if (saveImageDialog.ShowDialog() == DialogResult.OK)
            {
                path = saveImageDialog.FileName;
                File.Copy(@"./模板/导入名单模板.xlsx", path);
                FrmTips.ShowTipsSuccess(this, "导出成功");
            }
        }
        /// <summary>
        /// 本地名单导入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton4_Click(object sender, EventArgs e)
        {
            string path = OpenXlsxDialog();
            ExcelListInput(path);

        }

        private void ExcelListInput(object obj)
        {
            bool IsResult = false;
            try
            {
                string path = obj as string;
                if (!string.IsNullOrEmpty(path))
                {
                    if (!string.IsNullOrEmpty(_projectName))
                    {
                        string projectName = _projectName;
                        string projectid = sQLiteHelper.ExecuteScalar($"select Id from SportProjectInfos where name='{projectName}'").ToString();
                        var rows = MiniExcel.Query<ExportStudentData>(path).ToList();
                        proVal = 0;
                        proMax = rows.Count;
                        ///序号
                        ///学校
                        ///年纪
                        ///班级
                        ///姓名
                        ///性别
                        ///准考证号
                        ///组别名称
                        ///
                        HashSet<String> set = new HashSet<String>();
                        for (int i = 0; i < rows.Count; i++)
                        {
                            set.Add(rows[i].GroupName);
                        }
                        List<String> rolesMarketList = new List<string>();
                        rolesMarketList.AddRange(set);

                        System.Data.SQLite.SQLiteTransaction sQLiteTransaction = sQLiteHelper.BeginTransaction();
                        for (int i = 0; i < rolesMarketList.Count; i++)
                        {
                            string GroupName = rolesMarketList[i];
                            string countstr = sQLiteHelper.ExecuteScalar($"SELECT COUNT(*) FROM DbGroupInfos where ProjectId='{projectid}' and Name='{GroupName}'").ToString();
                            int.TryParse(countstr, out int count);
                            if (count == 0)
                            {
                                string groupsortidstr = sQLiteHelper.ExecuteScalar("select MAX( SortId ) + 1 from DbGroupInfos").ToString();
                                int groupsortid = 1;
                                int.TryParse(groupsortidstr, out groupsortid);
                                string insertsql = $"INSERT INTO DbGroupInfos(CreateTime,SortId,IsRemoved,ProjectId,Name,IsAllTested) " +
                                    $"VALUES(datetime(CURRENT_TIMESTAMP, 'localtime'),{groupsortid},0,'{projectid}','{GroupName}',0)";
                                //插入组
                                sQLiteHelper.ExecuteNonQuery(insertsql);
                            }
                        }
                        for (int i = 0; i < rows.Count; i++)
                        {
                            ExportStudentData idata = rows[i];
                            string PersonIdNumber = idata.IdNumber;
                            string name = idata.Name;
                            int Sex = idata.Sex == "男" ? 0 : 1;
                            string SchoolName = idata.School;
                            string GradeName = idata.GradeName;
                            string classNumber = idata.ClassName;
                            string GroupName = idata.GroupName;
                            string countstr = sQLiteHelper.ExecuteScalar($"SELECT COUNT(*) FROM DbPersonInfos WHERE ProjectId='{projectid}' AND IdNumber='{PersonIdNumber}'").ToString();
                            int.TryParse(countstr, out int count);
                            if (count == 0)
                            {
                                int personsortid = 1;
                                string personsortidstr = sQLiteHelper.ExecuteScalar("select MAX( SortId ) + 1 from DbPersonInfos").ToString();
                                int.TryParse(personsortidstr, out personsortid);
                                string insertsql = $"INSERT INTO DbPersonInfos(CreateTime,SortId,IsRemoved,ProjectId,SchoolName,GradeName,ClassNumber,GroupName,Name,IdNumber,Sex,State,FinalScore,uploadState) " +
                                    $"VALUES(datetime(CURRENT_TIMESTAMP, 'localtime'),{personsortid},0,'{projectid}','{SchoolName}','{GradeName}','{classNumber}','{GroupName}'," +
                                    $"'{name}','{PersonIdNumber}',{Sex},0,-1,0)";
                                sQLiteHelper.ExecuteNonQuery(insertsql);
                            }
                            proVal++;
                        }
                        sQLiteHelper.CommitTransAction(sQLiteTransaction);
                        if (rows.Count == 0)
                        {
                            IsResult = false;
                        }
                        else
                        {
                            IsResult = true;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("请先选择 项目！！");
                    return;
                }
            }
            catch (Exception ex)
            {
                LoggerHelper.Debug(ex);
            }
            if (IsResult)
            {
                this.DialogResult = DialogResult.OK;
                FrmTips.ShowTipsSuccess(this, "导入成功");
            }
            else
            {
                this.DialogResult = DialogResult.Cancel;
                FrmTips.ShowTipsError(this, "导入失败");
            }
        
        }

        /// <summary>
        /// 打开本地xlsx 文件
        /// </summary>
        /// <returns></returns>
        private string OpenXlsxDialog()
        {
            string path = string.Empty;
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = true;      //该值确定是否可以选择多个文件
            dialog.Title = "请选择文件";     //弹窗的标题
            dialog.InitialDirectory = Application.StartupPath + "\\";    //默认打开的文件夹的位置
            dialog.Filter = "MicroSoft Excel文件(*.xlsx)|*.xlsx";       //筛选文件
            dialog.ShowHelp = true;     //是否显示“帮助”按钮
            dialog.RestoreDirectory = true;
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                path = dialog.FileName;
            }
            return path;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (proMax != 0)
            {
                uiProcessBar1.Maximum = proMax;
                if (proVal > proMax)
                {
                    proVal = proMax;
                    timer1.Stop();
                }
                uiProcessBar1.Value = proVal;
            }
        }

        private void uiButton5_Click(object sender, EventArgs e)
        {
            EquipmentCodeForm ecf = new EquipmentCodeForm();
            ecf.sQLiteHelper = sQLiteHelper;
            ecf.ShowDialog();
            UpDateLocalInfo();
        }

        private void uiButton6_Click(object sender, EventArgs e)
        {
            //funTest1();
            //funTest2();
            //funTest3();
            string groupNums = textBox1.Text;
            if (!String.IsNullOrEmpty(groupNums))
                FunTest5(groupNums);
            else
            {
                UIMessageBox.ShowError("请先输入你需要导入的组数");
                return;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupNums"></param>
        private void FunTest5(string groupNums)
        {
            RequestParameter RequestParameter = new RequestParameter();
            RequestParameter.AdminUserName = localValues["AdminUserName"];
            RequestParameter.TestManUserName = localValues["TestManUserName"];
            RequestParameter.TestManPassword = localValues["TestManPassword"];
            string ExamId0 = localValues["ExamId"];
            ExamId0 = ExamId0.Substring(ExamId0.IndexOf('_') + 1);
            string MachineCode0 = localValues["MachineCode"];
            MachineCode0 = MachineCode0.Substring(MachineCode0.IndexOf('_') + 1);
            RequestParameter.ExamId = ExamId0;
            RequestParameter.MachineCode = MachineCode0;
            RequestParameter.GroupNums = groupNums + "";
            //序列化
            string JsonStr = Newtonsoft.Json.JsonConvert.SerializeObject(RequestParameter);
            string url = localValues["Platform"] + RequestUrl.GetGroupStudentUrl;

            //? 下载男
            var formDatas = new List<FromDataItemModel> ();
            //添加其他字段
            formDatas.Add(new FromDataItemModel()
            {
                key = "data",
                value = JsonStr
            });
            var httpUpload = new HttpUpload();
            string result = HttpUpload.PostFromData(url, formDatas);
            GetGroupStudent upload_Result = JsonConvert.DeserializeObject<GetGroupStudent>(result);
            GetGroupStudent studentList = new GetGroupStudent();
            studentList.Results = new Results();
            studentList.Results.groups = new List<GroupsItem>();

            if (upload_Result.Results.groups.Count == 0)
            {
                FrmTips.ShowTipsError(this, $"男生组提交错误,错误码:[{upload_Result.Error}]");
            }
            else
            {
                studentList.Results.groups.AddRange(upload_Result.Results.groups);
            }

            if (studentList.Results.groups.Count > 0)
            {
                DownlistOutputExcel1(studentList);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="studentList"></param>
        private void DownlistOutputExcel1(GetGroupStudent studentList)
        {
            List<GroupsItem> Groups = studentList.Results.groups;
            List<ExportStudentData> doc = new List<ExportStudentData>();
            int step = 1;
            proVal = 0;
            proMax = 0;
            //序号	学校	 年级	班级 	姓名	 性别	准考证号	 组别名称
            foreach (var Group in Groups)
            {
                string groupId = Group.GroupId;
                string groupName = Group.GroupName;
                foreach (var StudentInfo in Group.StudentInfos)
                {
                    ExportStudentData idata = new  ExportStudentData();
                    idata.Id = step;
                    idata.School = StudentInfo.SchoolName;
                    idata.GradeName = StudentInfo.GradeName;
                    idata.ClassName = StudentInfo.ClassName;
                    idata.Name = StudentInfo.Name;
                    idata.Sex = StudentInfo.Sex;
                    idata.IdNumber = StudentInfo.IdNumber;
                    idata.GroupName = groupId;
                    doc.Add(idata);
                    step++;
                }
            }
            string path = Application.StartupPath + $"\\模板\\下载名单\\downList{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";
            ExcelUtils.MiniExcel_OutPutExcel(path, doc);
            ParameterizedThreadStart ParStart = new ParameterizedThreadStart(ExcelListInput);
            Thread t = new Thread(ParStart);
            t.IsBackground = true;
            t.Start(path);
        }
    }
}
