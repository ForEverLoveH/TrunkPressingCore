using AForge.Math.Metrics;
using HZH_Controls;
using HZH_Controls.Controls;
using HZH_Controls.Forms;
using Newtonsoft.Json;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.ModelBinding;
using System.Windows.Forms;
using TrunkPressingCore.GameConst;
using TrunkPressingCore.GameModel;
using TrunkPressingCore.GameSystem;
using TrunkPressingCore.SQLite;

namespace TrunkPressingCore.Window
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        AutoWindowSize AutoWindowSize  = new AutoWindowSize ();
        List<projectsModel > projects = new List<projectsModel>();
        SQLiteHelper sQLiteHelper = null;
        string projectId = "";
        string projectName = "";
        string treeGroupTxt = "";
        UpLoadStudentGrade UpLoadStudentGrade = new UpLoadStudentGrade();
        private void GroupChooseView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            projectName = string.Empty;
            projectId= string.Empty;    
            if (e.Node.Level == 1)
            {
                if(e.Button== MouseButtons.Left)
                {
                     
                    String fullpath = e.Node.FullPath;
                    string[] fsp = fullpath.Split('\\');
                    if(fsp.Length > 0)
                    {
                       projectName = fsp[0];    

                    }
                    treeGroupTxt = e.Node.Text;
                    /// 更新表格数据
                    DataGridView1Update(e.Node.Text);

                }
            }
            else if(e.Node.Level == 0)
            {
                if(e.Button == MouseButtons.Left)
                {
                    projectName = e.Node.Text;
                    mylistView.Items.Clear();

                }
            }
        }
         
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void MainWindow_Load(object sender, EventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;

            string code = "程序集版本：" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            string code1 = "文件版本：" + Application.ProductVersion.ToString();
            toolStripStatusLabel1.Text = code;
            AutoWindowSize.ControlInitializeSize(this);
            sQLiteHelper = new SQLiteHelper();
            ProjectTreeUpDate();
        }

        /// <summary>
        /// 项目树形 图更新
        /// </summary>

        private void ProjectTreeUpDate()
        {
            projects.Clear();
            this.treeView1.Nodes.Clear();
            var ds0 = sQLiteHelper.ExecuteReader($"SELECT Id,Name FROM SportProjectInfos");
            while (ds0.Read())
            {
                string ProjectId = ds0.GetValue(0).ToString();
                string ProjectName = ds0.GetString(1);
                var ds = sQLiteHelper.ExecuteReader($"SELECT Name,IsAllTested FROM DbGroupInfos WHERE ProjectId='{ProjectId}'");
                projects.Add(new projectsModel { projectName = ProjectName, Groups = new List<groupModel>() });
                while (ds.Read())
                {
                    string GroupName = ds.GetString(0);
                    int IsAllTested = ds.GetInt32(1);
                    projectsModel pp = projects.FirstOrDefault(a => a.projectName == ProjectName);
                    if (pp != null)
                    {
                        pp.Groups.Add(new groupModel { GroupName = GroupName, IsAllTested = IsAllTested });
                    }
                    else
                    {
                        projects.Add(new projectsModel
                        {
                            Groups = new List<groupModel>
                            { new groupModel { GroupName = GroupName, IsAllTested = IsAllTested } },
                            projectName = ProjectName
                        });
                    }
                }
            }


            for (int i = 0; i < projects.Count; i++)
            {
                TreeNode tn1 = new TreeNode(projects[i].projectName);
                List<groupModel> list = projects[i].Groups;
                for (int j = 0; j < list.Count; j++)
                {
                    tn1.Nodes.Add(list[j].GroupName);

                }
                this.treeView1.Nodes.Add(tn1);
                //全部测试完成显示绿色
                for (int j = 0; j < list.Count; j++)
                {
                    if (list[j].IsAllTested != 0)
                    {
                        this.treeView1.Nodes[i].Nodes[j].BackColor = Color.MediumSpringGreen;
                    }
                }


            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ucNavigationMenu1_ClickItemed(object sender, EventArgs e)
        {
            string txt = ucNavigationMenu1.SelectItem.Text;
            if (txt == "启动测试")
            {
                var ls = treeView1.SelectedNode;
                if (ls != null)
                {
                        string path = treeView1.SelectedNode.FullPath;
                        string[] fsp = path.Split('\\');
                        if (fsp.Length > 0)
                        {
                            List<Dictionary<string, string>> list = sQLiteHelper.ExecuteReaderList($"SELECT Id,Type,RoundCount,BestScoreMode,TestMethod," +
                            $"FloatType,TurnsNumber0,TurnsNumber1 FROM SportProjectInfos WHERE Name='{fsp[0]}'");
                            if (list.Count == 1)
                            {
                                try
                                {
                                    Dictionary<string, string> dict = list[0];
                                    int.TryParse(dict["Type"], out int state);
                                    RunTestWindow runTestWindow = new RunTestWindow();
                                    runTestWindow._ProjectId = fsp[0];
                                    runTestWindow.sQLiteHelper = sQLiteHelper;
                                    runTestWindow._ProjectName = fsp[0];
                                    if (fsp.Length > 1)
                                    {
                                        runTestWindow._GroupName = fsp[1];

                                    }
                                    runTestWindow._ProjectId = dict["Id"];
                                    runTestWindow._Type = dict["Type"];
                                    runTestWindow._RoundCount = Convert.ToInt32(dict["RoundCount"]);
                                    runTestWindow._BestScoreMode = Convert.ToInt32(dict["BestScoreMode"]);
                                    runTestWindow._TestMethod = Convert.ToInt32(dict["TestMethod"]);
                                    runTestWindow._FloatType = Convert.ToInt32(dict["FloatType"]);
                                    runTestWindow.formTitle = string.Format("考试项目:{0}", fsp[0]);
                                    this.Hide();
                                    runTestWindow.ShowDialog();
                                    if (!string.IsNullOrEmpty(projectName))
                                    {
                                        HZH_Controls.ControlHelper.ThreadInvokerControl(this, () =>
                                        {
                                            DataGridView1Update(treeGroupTxt);
                                        });
                                    }

                                }
                                catch (Exception ex)
                                {
                                    LoggerHelper.Debug(ex);
                                }
                                finally
                                {
                                    this.Show();
                                }
                            }
                        }
                    
                }
                else
                {
                    UIMessageBox.ShowWarning("请先选择项目组信息！！");
                    return;
                }

            }
            else if (txt == "修改成绩")
            {
                if (ModifyScore())
                {
                    FrmTips.ShowTipsSuccess(this, "修改成绩成功");
                }
                else
                {
                    FrmTips.ShowTipsSuccess(this, "修改成绩失败");
                }
            }
            else if (txt == "导入成绩")
            {
                /*if (InputScore())
                {
                    DataGridView1Update(treeGroupTxt);
                    FrmTips.ShowTipsError(this, "导入成绩成功");
                }
                else
                {
                    FrmTips.ShowTipsError(this, "导入成绩失败");
                }*/
                FrmTips.ShowTipsError(this, "导入成绩失败");
            }
            else if (txt == "清除成绩")
            {
                if (ClearGradeScore())
                {
                    FrmTips.ShowTipsSuccess(this, "清除成绩成功");
                }
                else
                {
                    FrmTips.ShowTipsSuccess(this, "清除成绩失败");
                }
            }
            else if (txt == "导出成绩")
            {
                ExportGrade();
            }
            else if (txt == "项目设置")
            {
                ProjectSetForm psf = new ProjectSetForm();
                psf.sQLiteHelper = sQLiteHelper;
                psf.ShowDialog();
                ProjectTreeUpDate();
            }
            else if (txt == "系统参数设置")
            {

            }
            else if (txt == "初始化数据库")
            {
                sQLiteHelper.InitSQLiteDB();
                HZH_Controls.ControlHelper.ThreadInvokerControl(this, () =>
                {
                    ProjectTreeUpDate();
                    treeGroupTxt = "";
                    DataGridView1Update(treeGroupTxt);
                });
                FrmTips.ShowTipsSuccess(this, "初始化数据库成功");
            }
            else if (txt == "数据库备份")
            {
                sQLiteHelper.ExportSQLiteDb();
                FrmTips.ShowTipsSuccess(this, "数据库备份成功");
            }
            else if (txt == "导入成绩模板")
            {
                string path = Application.StartupPath + "\\模板\\导入成绩模板.xls";
                if (File.Exists(path))
                {
                    System.Diagnostics.Process.Start(path);
                }
            }
            else if (txt == "导入名单模板")
            {
                string path = Application.StartupPath + "\\模板\\导入名单模板.xls";
                if (File.Exists(path))
                {
                    System.Diagnostics.Process.Start(path);
                }
            }
            else if (txt == "平台设备码")
            {
                EquipmentCodeForm ecf = new EquipmentCodeForm();
                ecf.sQLiteHelper = sQLiteHelper;
                ecf.ShowDialog();
            }
            else if (txt == "退出")
            {
                this.Close();

            }
            else if (txt == "上传成绩")
            {
                ParameterizedThreadStart method = new ParameterizedThreadStart(UpdateLoadScore);
                Thread threadRead = new Thread(method);
                threadRead.IsBackground = true;
                threadRead.Start();
            }
             
             
        }
        /// <summary>
        ///  上传成绩
        /// </summary>
        /// <param name="obj"></param>
        private void UpdateLoadScore(object obj)
        {
            if (treeView1.SelectedNode == null)
            { 
                UIMessageBox.ShowError("请选择项目数据！！");
                return;
            }
            String path = treeView1.SelectedNode.FullPath;
            if (!String.IsNullOrEmpty(path))
            {
                String[] fsp = path.Split('\\');
                string projectName = string.Empty;
                if (fsp.Length > 0)
                    projectName = fsp[0];
                if (string.IsNullOrEmpty(projectName))
                {
                    FrmTips.ShowTipsError(this, "请选择上传成绩的项目");
                    return;
                }
                string outMessage = string.Empty;
                if (fsp.Length > 1)
                {
                    outMessage = UploadStudentThreadFun(fsp, sQLiteHelper );
                }
                else
                {
                    outMessage =  UploadStudentByName(projectName, sQLiteHelper, 200);
                }
                if (string.IsNullOrEmpty(outMessage))
                {
                    FrmTips.ShowTipsInfo(this, "上传结束");
                }
                else
                {
                    MessageBox.Show(outMessage);
                }
                if (!string.IsNullOrEmpty(projectName))
                {
                    HZH_Controls.ControlHelper.ThreadInvokerControl(this, () =>
                    {
                        DataGridView1Update(treeGroupTxt);
                    });
                }
            }
            else
            {
                MessageBox.Show("请先选择项目数据！！");
                return;
            }
        }

        private string UploadStudentByName(string projectName, SQLiteHelper sQLiteHelper, int num=100)
        {
            try
            {
                List<Dictionary<string, string>> successList = new List<Dictionary<string, string>>();
                List<Dictionary<string, string>> errorList = new List<Dictionary<string, string>>();
                Dictionary<string, string> localInfos = new Dictionary<string, string>();
                List<Dictionary<string, string>> list0 = sQLiteHelper.ExecuteReaderList("SELECT * FROM localInfos");
                foreach (var item in list0)
                {
                    localInfos.Add(item["key"], item["value"]);
                }
                //查询项目数据信息
                Dictionary<string, string> SportProjectDic = sQLiteHelper.ExecuteReaderOne($"SELECT Id,Type,RoundCount,BestScoreMode,TestMethod," +
                         $"FloatType,TurnsNumber0,TurnsNumber1 FROM SportProjectInfos WHERE Name='{projectName}'");
               UploadResultsRequestParameter urrp = new UploadResultsRequestParameter();
                urrp.AdminUserName = localInfos["AdminUserName"];
                urrp.TestManUserName = localInfos["TestManUserName"];
                urrp.TestManPassword = localInfos["TestManPassword"];
                string MachineCode = localInfos["MachineCode"];
                if (MachineCode.IndexOf('_') != -1)
                {
                    MachineCode = MachineCode.Substring(MachineCode.IndexOf('_') + 1);
                }
                //轮次
                int turn = 0;
                if (SportProjectDic.Count > 0)
                {
                    int.TryParse(SportProjectDic["RoundCount"], out turn);
                    urrp.MachineCode = MachineCode;
                }
                else return "未找到项目";

                StringBuilder messageSb = new StringBuilder();
                StringBuilder logWirte = new StringBuilder();
                string sql0 = $"SELECT Id,GroupName,Name,IdNumber,SchoolName,GradeName,ClassNumber,State,Sex,BeginTime,FinalScore,uploadState FROM DbPersonInfos" +
                            $" WHERE ProjectId='{SportProjectDic["Id"]}' and FinalScore=1 ";
                List<Dictionary<string, string>> sqlStuResults = sQLiteHelper.ExecuteReaderList(sql0);
                //?  计次
                int numStep = 0;
                int maxNums = sqlStuResults.Count;
                List<StudentData> sudentsItems = new List<StudentData>();
                //IdNumber 对应Id
                Dictionary<string, string> map = new Dictionary<string, string>();
                //遍历学生
                foreach (var stu in sqlStuResults)
                {
                    numStep++;
                    //未参加考试的跳过
                    if (stu["State"] == "0" && stu["FinalScore"] == "-1")
                    {
                        continue;
                    }
                    /* //已上传的跳过
                     if (stu["uploadState"] == "1" || stu["uploadState"] == "-1")
                     {
                         continue;
                     }*/
                    List<RoundsItem> roundsItems = new List<RoundsItem>();
                    ///成绩
                    List<Dictionary<string, string>> resultScoreList1 = sQLiteHelper.ExecuteReaderList(
                        $"SELECT Id,CreateTime,RoundId,State,uploadState,Result FROM ResultInfos WHERE PersonId='{stu["Id"]}' And IsRemoved=0 LIMIT {turn}");
                    #region 查询文件
                    //成绩根目录
                    Dictionary<string, string> dic_images = new Dictionary<string, string>();
                    Dictionary<string, string> dic_viedos = new Dictionary<string, string>();
                    Dictionary<string, string> dic_texts = new Dictionary<string, string>();
                    //string scoreRoot = Application.StartupPath + $"\\Scores\\{projectName}\\{stu["GroupName"]}\\";
                    #endregion

                    //? 遍历成绩
                    foreach (var item in resultScoreList1)
                    {
                        if (item["uploadState"] != "0") continue;
                        ///
                        DateTime.TryParse(item["CreateTime"], out DateTime dtBeginTime);
                        string dateStr = dtBeginTime.ToString("yyyyMMdd");
                        string GroupNo = $"{stu["GroupName"]}_{stu["IdNumber"]}_{item["RoundId"]}";
                        //轮次成绩
                         RoundsItem rdi = new RoundsItem();
                        rdi.RoundId = Convert.ToInt32(item["RoundId"]);
                        rdi.State = ResultState.ResultState2Str(item["State"]);
                        rdi.Time = item["CreateTime"];
                        double.TryParse(item["Result"], out double score);
                        rdi.Result = score;
                        //string.Format("{0:D2}:{1:D2}", ts.Minutes, ts.Seconds);
                        rdi.GroupNo = GroupNo;
                        rdi.Text = dic_texts;
                        rdi.Images = dic_images;
                        rdi.Videos = dic_viedos;
                        roundsItems.Add(rdi);
                    }
                    if (roundsItems.Count == 0) continue;

                    StudentData ssi = new StudentData();
                    ssi.SchoolName = stu["SchoolName"];
                    ssi.GradeName = stu["GradeName"];
                    ssi.ClassNumber = stu["ClassNumber"];
                    ssi.Name = stu["Name"];
                    ssi.IdNumber = stu["IdNumber"];
                    ssi.Rounds = roundsItems;
                    sudentsItems.Add(ssi);
                    map.Add(stu["IdNumber"], stu["Id"]);

                    //超过 限制数量就上传或者最后一人
                    if (sudentsItems.Count >= num || numStep >= maxNums)
                    {
                        urrp.studentDatas = sudentsItems;
                        //序列化json
                        string JsonStr = Newtonsoft.Json.JsonConvert.SerializeObject(urrp);
                        string url = localInfos["Platform"] +  RequestUrl.UploadResults;
                        var httpUpload = new HttpUpload();
                        var formDatas = new List<FromDataItemModel>();
                        //添加其他字段
                        formDatas.Add(new FromDataItemModel()
                        {
                           key = "data",
                           value = JsonStr
                        });
                        logWirte.AppendLine();
                        logWirte.AppendLine();
                        logWirte.AppendLine(JsonStr);
                        //上传学生成绩
                        string result = HttpUpload.PostFromData(url, formDatas);
                        UpLoadResult upload_Result = Newtonsoft.Json.JsonConvert.DeserializeObject<UpLoadResult>(result);
                        if(upload_Result.Error!=null) { return upload_Result.Error; }
                        string errorStr = "null";
                        List<Dictionary<string, int>> result1 = upload_Result.Results;
                        foreach (var item in sudentsItems)
                        {
                            Dictionary<string, string> dic = new Dictionary<string, string>();
                            //map
                            dic.Add("Id", map[item.IdNumber]);
                            dic.Add("IdNumber", item.IdNumber);
                            dic.Add("Name", item.Name);
                            dic.Add("uploadGroup", item.Rounds[0].GroupNo);
                            var value = 0;
                            result1.Find(a => a.TryGetValue(item.IdNumber, out value));
                            if (value == 1)
                            {
                                successList.Add(dic);
                            }
                            else if (value != 0)
                            {
                                errorStr = UpLoadResults.Match(value);
                                dic.Add("error", errorStr);
                                errorList.Add(dic);
                                messageSb.AppendLine($"考号:{item.IdNumber} 姓名:{item.Name}上传失败,错误内容:{errorStr}");
                            }
                        }
                        map.Clear();
                        sudentsItems.Clear();
                    }
                }
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"成功:{successList.Count},失败:{errorList.Count}");
                sb.AppendLine("****************error***********************");

                foreach (var item in errorList)
                {
                    sb.AppendLine($"考号:{item["IdNumber"]} 姓名:{item["Name"]} 错误:{item["error"]}");
                }
                sb.AppendLine("*****************error**********************");


                System.Data.SQLite.SQLiteTransaction sQLiteTransaction = sQLiteHelper.BeginTransaction();
                sb.AppendLine("******************success*********************");
                foreach (var item in successList)
                {
                    string sql1 = $"UPDATE DbPersonInfos SET uploadState=1,uploadGroup='{item["uploadGroup"]}' WHERE Id={item["Id"]}";
                    sQLiteHelper.ExecuteNonQuery(sql1);
                    //更新成绩上传状态
                    sql1 = $"UPDATE ResultInfos SET uploadState=1 WHERE PersonId={item["Id"]}";
                    sQLiteHelper.ExecuteNonQuery(sql1);
                    sb.AppendLine($"考号:{item["IdNumber"]} 姓名:{item["Name"]}");
                }
                sQLiteHelper.CommitTransAction(sQLiteTransaction);
                sb.AppendLine("*******************success********************");

                string txtpath = Application.StartupPath + $"\\Log\\upload\\";
                if (!Directory.Exists(txtpath))
                {
                    Directory.CreateDirectory(txtpath);
                }
                if (successList.Count != 0 || errorList.Count != 0)
                {
                    txtpath = Path.Combine(txtpath, $"upload_{DateTime.Now.ToString("yyyyMMddHHmmss")}.txt");
                    File.WriteAllText(txtpath, sb.ToString());
                }
                string outpitMessage = messageSb.ToString();
                return outpitMessage;
            }
            catch (Exception ex)
            {
                LoggerHelper.Debug(ex);
                return ex.Message;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="sQLiteHelper"></param>
        /// <returns></returns>
        private string UploadStudentThreadFun(Object obj, SQLiteHelper sQLiteHelper)
        {
            try
            {
                List<Dictionary<string, string>> successList = new List<Dictionary<string, string>>();
                List<Dictionary<string, string>> errorList = new List<Dictionary<string, string>>();
                Dictionary<string, string> localInfos = new Dictionary<string, string>();
                List<Dictionary<string, string>> list0 = sQLiteHelper.ExecuteReaderList("SELECT * FROM localInfos");
                foreach (var item in list0)
                {
                    localInfos.Add(item["key"], item["value"]);
                }
                int.TryParse(localInfos["UploadUnit"], out int UploadUnit);
                string[] fusp = obj as string[];
                ///项目名称
                string projectName = string.Empty;
                //组
                string groupName = string.Empty;
                if (fusp.Length > 0)
                    projectName = fusp[0];
                if (fusp.Length > 1)
                    groupName = fusp[1];
                Dictionary<string, string> SportProjectDic = sQLiteHelper.ExecuteReaderOne($"SELECT Id,Type,RoundCount,BestScoreMode,TestMethod," +
                         $"FloatType,TurnsNumber0,TurnsNumber1 FROM SportProjectInfos WHERE Name='{projectName}'");
                string sql0 = $"SELECT Id,ProjectId,Name FROM DbGroupInfos WHERE ProjectId='{SportProjectDic["Id"]}' ";
                ///查询本项目已考组
                if (!string.IsNullOrEmpty(groupName))
                {
                    sql0 += $" AND Name = '{groupName}'";
                }
                List<Dictionary<string, string>> sqlGroupsResults = sQLiteHelper.ExecuteReaderList(sql0);
                 UploadResultsRequestParameter urrp = new  UploadResultsRequestParameter();
                urrp.AdminUserName = localInfos["AdminUserName"];
                urrp.TestManUserName = localInfos["TestManUserName"];
                urrp.TestManPassword = localInfos["TestManPassword"];
                string MachineCode = localInfos["MachineCode"];

                if (MachineCode.IndexOf('_') != -1)
                {
                    MachineCode = MachineCode.Substring(MachineCode.IndexOf('_') + 1);
                }

                StringBuilder messageSb = new StringBuilder();
                StringBuilder logWirte = new StringBuilder();
                ///按组上传
                foreach (var sqlGroupsResult in sqlGroupsResults)
                {
                    string sql = $"SELECT Id,GroupName,Name,IdNumber,SchoolName,GradeName,ClassNumber,State,Sex,BeginTime,FinalScore,uploadState FROM DbPersonInfos" +
                        $" WHERE ProjectId='{SportProjectDic["Id"]}' AND GroupName = '{sqlGroupsResult["Name"]}'";
                    List<Dictionary<string, string>> list = sQLiteHelper.ExecuteReaderList(sql);
                    //轮次
                    int turn = 0;
                    if (list.Count > 0)
                    {
                        Dictionary<string, string> stu = list[0];
                        int.TryParse(SportProjectDic["RoundCount"], out turn);
                        urrp.MachineCode = MachineCode;
                    }
                    else
                    {
                        continue;
                    }

                    List<StudentData> sudentsItems = new List<StudentData>();
                    //IdNumber 对应Id
                    Dictionary<string, string> map = new Dictionary<string, string>();

                    foreach (var stu in list)
                    {
                        //未参加考试的跳过
                        if (stu["State"] == "0" && stu["FinalScore"] == "-1") continue;

                        //已上传的跳过
                        if (stu["uploadState"] == "1" || stu["uploadState"] == "-1")
                        {
                            continue;
                        }
                        List<RoundsItem> roundsItems = new List<RoundsItem>();
                        ///成绩
                        List<Dictionary<string, string>> resultScoreList1 = sQLiteHelper.ExecuteReaderList(
                            $"SELECT Id,CreateTime,RoundId,State,uploadState,Result FROM ResultInfos WHERE PersonId='{stu["Id"]}' And IsRemoved=0 LIMIT {turn}");
                        #region 查询文件
                        //成绩根目录
                        Dictionary<string, string> dic_images = new Dictionary<string, string>();
                        Dictionary<string, string> dic_viedos = new Dictionary<string, string>();
                        Dictionary<string, string> dic_texts = new Dictionary<string, string>();
                        //string scoreRoot = Application.StartupPath + $"\\Scores\\{projectName}\\{stu["GroupName"]}\\";

                        #endregion

                        foreach (var item in resultScoreList1)
                        {
                            if (item["uploadState"] != "0") continue;
                            ///
                            DateTime.TryParse(item["CreateTime"], out DateTime dtBeginTime);
                            string dateStr = dtBeginTime.ToString("yyyyMMdd");
                            string GroupNo = $"{dateStr}_{stu["GroupName"]}_{stu["IdNumber"]}_{item["RoundId"]}";
                            //轮次成绩
                            RoundsItem rdi = new  RoundsItem();
                            rdi.RoundId = Convert.ToInt32(item["RoundId"]);
                            rdi.State = ResultState.ResultState2Str(item["State"]);
                            rdi.Time = item["CreateTime"];
                            double.TryParse(item["Result"], out double score);
                            if (UploadUnit == 1)
                            {
                                score *= 100;
                            }
                            rdi.Result = score;
                            //string.Format("{0:D2}:{1:D2}", ts.Minutes, ts.Seconds);
                            rdi.GroupNo = GroupNo;
                            rdi.Text = dic_texts;
                            rdi.Images = dic_images;
                            rdi.Videos = dic_viedos;
                            roundsItems.Add(rdi);

                        }

                        if (roundsItems.Count == 0) continue;

                        StudentData ssi = new StudentData();
                        ssi.SchoolName = stu["SchoolName"];
                        ssi.GradeName = stu["GradeName"];
                        ssi.ClassNumber = stu["ClassNumber"];
                        ssi.Name = stu["Name"];
                        ssi.IdNumber = stu["IdNumber"];
                        ssi.Rounds = roundsItems;
                        sudentsItems.Add(ssi);
                        map.Add(stu["IdNumber"], stu["Id"]);

                    }
                    urrp.studentDatas = sudentsItems;
                    //序列化json
                    string JsonStr = Newtonsoft.Json.JsonConvert.SerializeObject(urrp);
                    string url = localInfos["Platform"]+RequestUrl.UploadResults;
                    var httpUpload = new  HttpUpload();
                    var formDatas = new List<FromDataItemModel>();
                    //添加其他字段
                    formDatas.Add(new FromDataItemModel()
                    {
                        key = "data",
                        value = JsonStr
                    });

                    logWirte.AppendLine();
                    logWirte.AppendLine();
                    logWirte.AppendLine(JsonStr);

                    //上传学生成绩
                    string result = HttpUpload.PostFromData(url, formDatas);
                    UpLoadResult upload_Result = Newtonsoft.Json.JsonConvert.DeserializeObject<UpLoadResult>(result);
                    if (upload_Result.Error != null)
                    {
                        return upload_Result.Error;
                    }
                    string errorStr = "null";
                    List<Dictionary<string, int>> result1 = upload_Result.Results;
                    foreach (var item in sudentsItems)
                    {
                        Dictionary<string, string> dic = new Dictionary<string, string>();
                        //map
                        dic.Add("Id", map[item.IdNumber]);
                        dic.Add("IdNumber", item.IdNumber);
                        dic.Add("Name", item.Name);
                        dic.Add("uploadGroup", item.Rounds[0].GroupNo);
                        var value = 0;
                        result1.Find(a => a.TryGetValue(item.IdNumber, out value));
                        if (value == 1)
                        {
                            successList.Add(dic);
                        }
                        else if (value != 0)
                        {
                            errorStr = UpLoadResults.Match(value);
                            dic.Add("error", errorStr);
                            errorList.Add(dic);
                            messageSb.AppendLine($"{sqlGroupsResult["Name"]}组 考号:{item.IdNumber} 姓名:{item.Name}上传失败,错误内容:{errorStr}");
                        }
                    }
                }
                LoggerHelper.Monitor(logWirte.ToString());

                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"成功:{successList.Count},失败:{errorList.Count}");
                sb.AppendLine("****************error***********************");

                foreach (var item in errorList)
                {
                    sb.AppendLine($"考号:{item["IdNumber"]} 姓名:{item["Name"]} 错误:{item["error"]}");
                }
                sb.AppendLine("*****************error**********************");


                System.Data.SQLite.SQLiteTransaction sQLiteTransaction = sQLiteHelper.BeginTransaction();
                sb.AppendLine("******************success*********************");
                foreach (var item in successList)
                {
                    string sql1 = $"UPDATE DbPersonInfos SET uploadState=1,uploadGroup='{item["uploadGroup"]}' WHERE Id={item["Id"]}";
                    sQLiteHelper.ExecuteNonQuery(sql1);
                    //更新成绩上传状态
                    sql1 = $"UPDATE ResultInfos SET uploadState=1 WHERE PersonId={item["Id"]}";
                    sQLiteHelper.ExecuteNonQuery(sql1);
                    sb.AppendLine($"考号:{item["IdNumber"]} 姓名:{item["Name"]}");
                }
                sQLiteHelper.CommitTransAction(sQLiteTransaction);
                sb.AppendLine("*******************success********************");

                string txtpath = Application.StartupPath + $"\\Log\\upload\\";
                if (!Directory.Exists(txtpath))
                {
                    Directory.CreateDirectory(txtpath);
                }
                if (successList.Count != 0 || errorList.Count != 0)
                {
                    txtpath = Path.Combine(txtpath, $"upload_{DateTime.Now.ToString("yyyyMMddHHmmss")}.txt");
                    File.WriteAllText(txtpath, sb.ToString());
                }
                string outpitMessage = messageSb.ToString();
                return outpitMessage;
            }
            catch (Exception ex)
            {
                LoggerHelper.Debug(ex);
                return ex.Message;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool ExportGrade()
        {
            if (!string.IsNullOrEmpty(projectId) && !string.IsNullOrEmpty(projectName))
            {
                try
                {
                    OutPutExcelScoreForm oesf = new OutPutExcelScoreForm();
                    oesf.helper = sQLiteHelper;
                    oesf._projectId = projectId;
                    oesf._groupName = treeGroupTxt;
                    oesf._projectName = projectName;
                    oesf.ShowDialog();
                }
                catch (Exception ex)
                {
                    LoggerHelper.Debug(ex);
                    return false;
                }
                return true;
            }
            else
            {
                UIMessageBox.ShowWarning("请先确定项目信息！！");
                return false;
            }
        }

        /// <summary>
        ///清除
        /// </summary>
        /// <returns></returns>
        private bool ClearGradeScore()
        {
            try
            {
                if (mylistView.SelectedItems.Count == 0)
                {
                    FrmTips.ShowTipsError(this, "请选择一个学生");
                    return false;
                }
                string Name = mylistView.SelectedItems[0].SubItems[3].Text;
                string PersonIdNumber = mylistView.SelectedItems[0].SubItems[4].Text;
                if (FrmDialog.ShowDialog(this, $"是否清空学生:{Name}的成绩？", "提示", true) == System.Windows.Forms.DialogResult.OK)
                {
                    string sql = $"DELETE FROM ResultInfos WHERE PersonIdNumber = '{PersonIdNumber}'";
                    int result = sQLiteHelper.ExecuteNonQuery(sql);
                    sql = $"update DbPersonInfos SET State=0 where IdNumber='{PersonIdNumber}'";
                    int result1 = sQLiteHelper.ExecuteNonQuery(sql);
                    if (result1 == 1 && result > 0)
                    {
                        DataGridView1Update(treeGroupTxt);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                LoggerHelper.Debug(ex);
                return false;
            }
        }
            
        /// <summary>
        /// 导入成绩
        /// </summary>
        private bool InputScore()
        {
            try
            {
                if (!(FrmDialog.ShowDialog(this, $"导入成绩会清空考生成绩\n是否导入?", "提示", true) == System.Windows.Forms.DialogResult.OK))
                {
                    return false;
                }

                string path = "";
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Multiselect = true;      //该值确定是否可以选择多个文件
                dialog.Title = "请选择文件";     //弹窗的标题
                dialog.InitialDirectory = Application.StartupPath + "\\";       //默认打开的文件夹的位置
                dialog.Filter = "MicroSoft Excel文件(*.xls;*.xlsx)|*.xls;*.xlsx";       //筛选文件
                dialog.ShowHelp = true;     //是否显示“帮助”按钮
                dialog.RestoreDirectory = true;
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    path = dialog.FileName;
                    if (!string.IsNullOrEmpty(path))
                    {
                        List<Dictionary<string, string>> exceldic = ExcelUtils.ReadExcel(path);
                        for (int i = 0; i < exceldic.Count; i++)
                        {
                            Dictionary<string, string> dics = exceldic[i];
                            string PersonIdNumber = dics["准考证号"];
                            //清空所有成绩
                            string sql = $"DELETE FROM ResultInfos WHERE PersonIdNumber = '{PersonIdNumber}'";
                            int result = sQLiteHelper.ExecuteNonQuery(sql);
                            //读取唯一编号和准考证号
                            var stu = sQLiteHelper.ExecuteReader($"SELECT Id,Name FROM DbPersonInfos WHERE IdNumber='{PersonIdNumber}'");
                            string perid = "";
                            string perName = "";
                            while (stu.Read())
                            {
                                perid = stu.GetString(0);
                                perName = stu.GetString(1);
                            }
                            dics.Remove("准考证号");
                            dics.Remove("姓名");
                            //写入成绩
                            foreach (var dic in dics)
                            {
                                string rounid = dic.Key.TrimStart('第').TrimEnd('轮');

                                sql = $"INSERT INTO ResultInfos(CreateTime,SortId,IsRemoved,PersonId,SportItemType,PersonName,PersonIdNumber,RoundId,Result,State) " +
                                    $"VALUES (datetime(CURRENT_TIMESTAMP, 'localtime') ,(SELECT MAX(SortId)+1 FROM ResultInfos),0," +
                                     $"'{perid}',0,'{perName}','{PersonIdNumber}',{rounid},{dic.Value},1)";
                                int result0 = sQLiteHelper.ExecuteNonQuery(sql);
                            }
                            //状态设置为已测试
                            sql = $"update DbPersonInfos SET State=1 where IdNumber='{PersonIdNumber}'";
                            int result1 = sQLiteHelper.ExecuteNonQuery(sql);
                        }

                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                LoggerHelper.Debug(ex);
                return false;
            }
        }
            /// <summary>
            ///  修改成绩
            /// </summary>
            /// <returns></returns>
        private bool ModifyScore()
        {
            try
            {
                if (mylistView.SelectedItems.Count == 0)
                {
                    FrmTips.ShowTipsError(this, "请选择一个考生");
                    return false;
                }
                int index = mylistView.SelectedItems[0].Index;
                string projectName = mylistView.SelectedItems[0].SubItems[1].Text;
                string groupName = mylistView.SelectedItems[0].SubItems[2].Text;
                string Name = mylistView.SelectedItems[0].SubItems[3].Text;
                string IdNumber = mylistView.SelectedItems[0].SubItems[4].Text;
                string status = mylistView.SelectedItems[0].SubItems[5].Text;
                int rountid = 0;
                Dictionary<string, string> SportProjectDic = sQLiteHelper.ExecuteReaderOne($"SELECT Id,Type,RoundCount,BestScoreMode,TestMethod," +
                    $"FloatType,TurnsNumber0,TurnsNumber1 FROM SportProjectInfos WHERE Name='{projectName}'");
                int FloatType = 0;
                if (SportProjectDic.Count > 0)
                {
                    FloatType = Convert.ToInt32(SportProjectDic["FloatType"]);
                    rountid = Convert.ToInt32(SportProjectDic["RoundCount"]);
                }
                FrmModifyScoreTest frm = new FrmModifyScoreTest();
                frm.projectName = projectName;
                frm.groupName = groupName;
                frm.sName = Name;
                frm.IdNumber = IdNumber;
                frm.status = status;
                frm.rountid = rountid;
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    int roundid = frm.updaterountId;
                    double updateScore = frm.updateScore;
                    decimal.Round(decimal.Parse(updateScore.ToString("0.0000")), FloatType).ToString();

                    string updatestatus = frm.status;
                    int Resultinfo_State = ResultState.ResultState2Int(updatestatus);
                    string perid = "";
                    var ds0 = sQLiteHelper.ExecuteReaderOne($"SELECT Id FROM DbPersonInfos WHERE IdNumber='{IdNumber}' and ProjectId='{projectId}'");
                    if (ds0 == null || ds0.Count == 0) return false;
                    perid = ds0["Id"];

                    string sql = $"UPDATE ResultInfos SET Result={updateScore},State={Resultinfo_State} WHERE PersonId='{perid}' AND RoundId={roundid}";
                    int result = sQLiteHelper.ExecuteNonQuery(sql);
                    if (result == 0)
                    {
                        if (string.IsNullOrEmpty(perid))
                        {
                            return false;
                        }
                        sql = $"INSERT INTO ResultInfos(CreateTime,SortId,IsRemoved,PersonId,SportItemType,PersonName,PersonIdNumber,RoundId,Result,State) " +
                                    $"VALUES (datetime(CURRENT_TIMESTAMP, 'localtime') ,(SELECT MAX(SortId)+1 FROM ResultInfos),0," +
                                     $"'{perid}',0,'{Name}','{IdNumber}',{rountid},{updateScore},{Resultinfo_State})";
                        int result0 = sQLiteHelper.ExecuteNonQuery(sql);

                    }
                    else if (result > 1)
                    {
                        return false;
                    }
                    if (!string.IsNullOrEmpty(projectName))
                    {
                        HZH_Controls.ControlHelper.ThreadInvokerControl(this, () =>
                        {
                            DataGridView1Update(treeGroupTxt);
                        });
                    }
                    return true;
                }
                else
                {
                    return false;
                }


            }
            catch (Exception ex)
            {
                LoggerHelper.Debug(ex);
                return false;
            }


        }
             
        /// <summary>
        ///  更新表格数据
        /// </summary>
        /// <param name="name"></param>

        private void DataGridView1Update(string  name)
        {
            mylistView.Items.Clear();
            if (string.IsNullOrEmpty(name)) return;
            var ds = sQLiteHelper.ExecuteReader($"SELECT b.Id,b.RoundCount,b.FloatType,b.Type " +
             $"FROM DbGroupInfos AS a,SportProjectInfos AS b WHERE a.ProjectId=b.Id AND a.Name='{name}' AND b.Name='{projectName}'");
            // 轮次
            int roundCount = 0;
            int FloatType = 0;
            int type0 = 0;
            while (ds.Read())
            {
                projectId = ds.GetValue(0).ToString();
                roundCount = ds.GetInt16(1);FloatType = ds.GetInt16(2);
                type0 = ds.GetInt16(3);

            }
            ds = sQLiteHelper.ExecuteReader($"SELECT dpi.GroupName,dpi.Name,dpi.IdNumber,dpi.State,dpi.FinalScore,dpi.Id" +
             $" FROM DbPersonInfos as dpi WHERE dpi.GroupName='{name}' AND dpi.ProjectId='{projectId}'");
            int i = 1;
            mylistView.BeginUpdate();
            //初始化标题
            InitListViewHeadTitle(roundCount);
            mylistView.Items.Clear();
            while (ds.Read())
            {
                string num = ds.GetString(2);
                int state = ds.GetInt16(3);
                string personid = ds.GetValue(5).ToString();
                ListViewItem item = new ListViewItem();
                item.UseItemStyleForSubItems = false;
                item.Text = i.ToString();
                item.SubItems.Add(projectName);
                item.SubItems.Add(ds.GetString(0));
                item.SubItems.Add(ds.GetString(1));
                item.SubItems.Add(num);
                if(state == 1)
                {
                    item.SubItems.Add("已测试");
                    item.SubItems[item.SubItems.Count-1].BackColor = Color.MediumSpringGreen;
                }
                else
                {
                    item.SubItems.Add("未测试");

                }
                double maxscore = 1000;
                var res = sQLiteHelper.ExecuteReaderList($"SELECT SortId,RoundId,Result,State,CreateTime,uploadState FROM ResultInfos WHERE PersonId='{personid}'");
                int k=0;  
                List<double> list = new List<double>();
                foreach (var dic in res)
                {
                    int.TryParse(dic["RoundId"], out int RoundId);
                    double.TryParse(dic["Result"], out double Result);
                    list.Add(Result);
                    string restr = GameConst.ResultState.ResultState2Str(dic["State"]);
                    if (restr == "已测试")
                    {
                        if (maxscore > Result)
                        {
                            maxscore = Result;
                        }
                        restr = decimal.Round(decimal.Parse(Result.ToString("0.0000")), FloatType).ToString();
                        item.SubItems.Add(restr);

                    }
                    else
                    {
                        item.SubItems.Add(restr);
                        item.SubItems[item.SubItems.Count-1].ForeColor = Color.Red;

                    }
                    item.SubItems[item.SubItems.Count - 1].Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);

                    if (dic["uploadState"] == "0")
                    {
                        item.SubItems.Add("未上传");
                        item.SubItems[item.SubItems.Count - 1].ForeColor = Color.Red;
                    }
                    else
                    {
                        item.SubItems.Add("已上传");
                        item.SubItems[item.SubItems.Count - 1].Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
                        item.SubItems[item.SubItems.Count - 1].ForeColor = Color.Green;
                    }
                    k++;
                }
                for (int j = k; j < roundCount; j++)
                {
                    item.SubItems.Add("无成绩");
                    item.SubItems.Add("未上传");
                }
                if (maxscore != 1000)
                {
                    if (list.Count > 0)
                    {
                        for(int j=0; j <list.Count-1; j++)
                        {
                            var s = (int)list[j];
                            var p = (int)list[j + 1];
                            if (s > p)
                            {
                                maxscore = list[j];
                            }
                            else
                            {
                                maxscore = list[j + 1];
                            }
                        }
                    }
                    item.SubItems.Add(decimal.Round(decimal.Parse(maxscore.ToString("0.0000")), FloatType).ToString());
                    item.SubItems[item.SubItems.Count - 1].Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
                }
                else
                {
                    item.SubItems.Add("无成绩");
                }
                mylistView.Items.Insert(mylistView.Items.Count, item);
                i++;
            }
            //自动列宽
            MyUtils MyUtils = new MyUtils();
            MyUtils.AutoResizeColumnWidth(mylistView);
            mylistView.EndUpdate();

        }
    
            
        /// <summary>
        ///  初始化标题
        /// </summary>
        /// <param name="roundCount"></param>
        private void InitListViewHeadTitle(int roundCount)
        {
            mylistView.View = View.Details;
            ColumnHeader[] Header = new ColumnHeader[100];
            int sp = 0;
            Header[sp] = new ColumnHeader();
            Header[sp].Text = "序号";
            Header[sp].Width = 40;
            sp++;

            Header[sp] = new ColumnHeader();
            Header[sp].Text = "项目名称";
            Header[sp].Width = 80;
            sp++;

            Header[sp] = new ColumnHeader();
            Header[sp].Text = "组别名称";
            Header[sp].Width = 40;

            sp++;
            Header[sp] = new ColumnHeader();
            Header[sp].Text = "姓名";
            Header[sp].Width = 100;
            sp++;

            Header[sp] = new ColumnHeader();
            Header[sp].Text = "准考证号";
            Header[sp].Width = 100;
            sp++;
            Header[sp] = new ColumnHeader();
            Header[sp].Text = "考试状态";
            Header[sp].Width = 40;
            sp++;
            for (int i = 1; i <= roundCount; i++)
            {
                Header[sp] = new ColumnHeader();
                Header[sp].Text = $"第{i}轮";
                Header[sp].Width = 40;
                sp++;

                Header[sp] = new ColumnHeader();
                Header[sp].Text = $"上传状态";
                Header[sp].Width = 80;
                sp++;
            }

            Header[sp] = new ColumnHeader();
            Header[sp].Text = "最好成绩";
            Header[sp].Width = 60;
            sp++;

            ColumnHeader[] Header1 = new ColumnHeader[sp];
            mylistView.Columns.Clear();
            for (int i = 0; i < Header1.Length; i++)
            {
                Header1[i] = Header[i];
            }
            mylistView.Columns.AddRange(Header1);

        }

        private void MainWindow_SizeChanged(object sender, EventArgs e)
        {
            //AutoWindowSize.ControlAutoSize(this);
        }

        private void treeView1_MouseDown(object sender, MouseEventArgs e)
        {
            if(e.Button== MouseButtons.Left)
            {
                TreeNode treeNode = treeView1.GetNodeAt(e.X, e.Y);
                if (treeNode != null)
                {
                    treeView1.SelectedNode = treeNode;
                }
            }
        }
    }
}
