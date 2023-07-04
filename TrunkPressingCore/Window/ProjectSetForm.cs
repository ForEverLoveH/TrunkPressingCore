using HZH_Controls;
using HZH_Controls.Controls;
using HZH_Controls.Forms;
using MiniExcelLibs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using TrunkPressingCore.GameModel;
using TrunkPressingCore.SQLite;

namespace TrunkPressingCore.Window
{
    public partial class ProjectSetForm : Form
    {
        public ProjectSetForm()
        {
            InitializeComponent();
        }
        //数据库
        public SQLiteHelper sQLiteHelper = null;
        string projectId = "";
        private void ProjectSetForm_Load(object sender, EventArgs e)
        {
            projectTreeUpdate();
        }
        /// <summary>
        /// 项目树形图结构
        /// </summary>
        class projectsModel
        {
            public string projectName { get; set; }
            public List<string> Groups { get; set; }
        }
        List<projectsModel> projects = new List<projectsModel>();
        /// <summary>
        /// 项目树形结构更新
        /// </summary>
        void projectTreeUpdate()
        {
            projects.Clear();
            this.treeViewEx1.Nodes.Clear();
            var ds0 = sQLiteHelper.ExecuteReader($"SELECT Id,Name FROM SportProjectInfos");
            while (ds0.Read())
            {
                string ProjectId = ds0.GetValue(0).ToString();
                string ProjectName = ds0.GetString(1);
                var ds = sQLiteHelper.ExecuteReader($"SELECT Name,IsAllTested FROM DbGroupInfos WHERE ProjectId='{ProjectId}'");
                projects.Add(new projectsModel { projectName = ProjectName, Groups = new List<string>() });
                while (ds.Read())
                {
                    string GroupName = ds.GetString(0);
                    int IsAllTested = ds.GetInt32(1);
                    projectsModel pp = projects.FirstOrDefault(a => a.projectName == ProjectName);
                    if (pp != null)
                    {
                        pp.Groups.Add(GroupName);
                    }
                    else
                    {
                        projects.Add(new projectsModel { Groups = new List<string> { GroupName }, projectName = ProjectName });
                    }
                }
            }


            for (int i = 0; i < projects.Count; i++)
            {
                TreeNode tn = new TreeNode(projects[i].projectName);
                List<string> list = projects[i].Groups;
                for (int j = 0; j < list.Count; j++)
                {
                    tn.Nodes.Add(list[j]);
                }
                this.treeViewEx1.Nodes.Add(tn);
            }
        }

        /// <summary>
        /// 更新表格数据
        /// </summary>
        /// <param name="groupName"></param>
        void ucDataGridView1Update(string groupName)
        {
            txt_GroupName.Text = groupName;
            List<DataGridViewColumnEntity> lstCulumns = new List<DataGridViewColumnEntity>();
            lstCulumns.Add(new DataGridViewColumnEntity() { DataField = "ID", HeadText = "序号", Width = 5, WidthType = SizeType.AutoSize });
            lstCulumns.Add(new DataGridViewColumnEntity() { DataField = "GroupName", HeadText = "组别名称", Width = 20, WidthType = SizeType.AutoSize });
            lstCulumns.Add(new DataGridViewColumnEntity() { DataField = "School", HeadText = "学校", Width = 20, WidthType = SizeType.AutoSize });
            lstCulumns.Add(new DataGridViewColumnEntity() { DataField = "Grade", HeadText = "年级", Width = 5, WidthType = SizeType.AutoSize });
            lstCulumns.Add(new DataGridViewColumnEntity() { DataField = "Class", HeadText = "班级", Width = 5, WidthType = SizeType.AutoSize });
            lstCulumns.Add(new DataGridViewColumnEntity() { DataField = "Name", HeadText = "姓名", Width = 20, WidthType = SizeType.AutoSize });
            lstCulumns.Add(new DataGridViewColumnEntity() { DataField = "Sex", HeadText = "性别", Width = 5, WidthType = SizeType.AutoSize, Format = (a) => { return ((int)a) == 0 ? "男" : "女"; } });
            lstCulumns.Add(new DataGridViewColumnEntity() { DataField = "IdNumber", HeadText = "准考证号", Width = 20, WidthType = SizeType.AutoSize });
            this.ucDataGridView1.Columns = lstCulumns;
            this.ucDataGridView1.IsShowCheckBox = true;
            List<object> lstSource = new List<object>();

            var ds = sQLiteHelper.ExecuteReaderList($"SELECT d.GroupName,d.SchoolName,d.GradeName,d.ClassNumber,d.Name,d.Sex,d.IdNumber " +
               $"FROM DbPersonInfos AS d WHERE d.GroupName='{groupName}' AND d.ProjectId='{projectId}'");
            int i = 1;
            foreach (var item in ds)
            {
                DataGridViewModel model = new DataGridViewModel()
                {
                    ID = i.ToString(),
                    GroupName = item["GroupName"],
                    School = item["SchoolName"],
                    Grade = item["GradeName"],
                    Class = item["ClassNumber"] + "班",
                    Name = item["Name"],
                    Sex = Convert.ToInt32(item["Sex"]),
                    IdNumber = item["IdNumber"],

                };
                lstSource.Add(model);
                i++;
            }
            this.ucDataGridView1.DataSource = lstSource;
            this.ucDataGridView1.ReloadSource();
            //this.ucDataGridView1.First();
        }

        /// <summary>
        /// 更新项目属性
        /// </summary>
        /// <param name="ProjectName"></param>
        void ProjectAttributeUpdate(string ProjectName)
        {
            var ds = sQLiteHelper.ExecuteReader("SELECT spi.Name,spi.Type,spi.RoundCount,spi.BestScoreMode,spi.TestMethod,spi.FloatType,spi.Id " +
                $"FROM SportProjectInfos AS spi WHERE spi.Name='{ProjectName}'");

            while (ds.Read())
            {
                string Name = ds.GetString(0);
                int Type = ds.GetInt16(1);
                int RoundCount = ds.GetInt16(2);
                int BestScoreMode = ds.GetInt16(3);
                int TestMethod = ds.GetInt16(4);
                int FloatType = ds.GetInt16(5);
                projectId = ds.GetValue(6).ToString();
                txt_projectName.Text = Name;
                txt_Type.SelectedIndex = Type;
                txt_RoundCount.SelectedIndex = RoundCount;
                txt_BestScoreMode.SelectedIndex = BestScoreMode;
                txt_TestMethod.SelectedIndex = TestMethod;
                txt_FloatType.SelectedIndex = FloatType;

                break;
            }

        }

        /// <summary>
        /// 表格显示标题
        /// </summary>
        class DataGridViewModel
        {
            public string ID { get; set; }
            public string GroupName { get; set; }
            public string School { get; set; }
            public string Grade { get; set; }
            public string Class { get; set; }
            public string Name { get; set; }
            public int Sex { get; set; }
            public string IdNumber { get; set; }
        }


        private void treeViewEx1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {

            if (e.Node.Level == 0)
            {
                if (e.Button == MouseButtons.Left)
                {
                    //父节点
                    ProjectAttributeUpdate(e.Node.Text);
                    /*panel5.Enabled = true;
                    panel7.Enabled = false;*/
                }
            }
            else if (e.Node.Level == 1)
            {
                if (e.Button == MouseButtons.Left)
                {
                   // panel5.Enabled = false;
                   // panel7.Enabled = true;
                    //子节点
                    //更新表格数据
                    ucDataGridView1Update(e.Node.Text);

                }
            }
        }

        /// <summary>
        /// 保存项目属性设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                if (treeViewEx1.SelectedNode != null)
                {
                    string name0 = treeViewEx1.SelectedNode.Text;
                    string Name = txt_projectName.Text;
                    int Type = txt_Type.SelectedIndex;
                    int RoundCount = txt_RoundCount.SelectedIndex;
                    int BestScoreMode = txt_BestScoreMode.SelectedIndex;
                    int TestMethod = txt_TestMethod.SelectedIndex;
                    int FloatType = txt_FloatType.SelectedIndex;
                    string projectID = sQLiteHelper.ExecuteScalar($"select Id from SportProjectInfos where Name='{name0}'").ToString();

                    string sql = $"UPDATE SportProjectInfos SET Name='{Name}', Type={Type},RoundCount={RoundCount},BestScoreMode={BestScoreMode},TestMethod={TestMethod},FloatType={FloatType} where Id='{projectID}'";
                    int result = sQLiteHelper.ExecuteNonQuery(sql);
                    if (result == 1)
                    {
                        projectTreeUpdate();
                        FrmTips.ShowTipsSuccess(this, "修改成功");
                    }
                    else
                    {
                        FrmTips.ShowTipsError(this, "修改失败");
                    }
                }
                else
                {
                    MessageBox.Show("请先新建或者选择项目数据！！");
                    return;
                }
            }
            catch (Exception ex)
            {
                FrmTips.ShowTipsError(this, "请选择项目修改属性");
                LoggerHelper.Debug(ex);
            }
        }
        /// <summary>
        /// 名单导入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            var sl = treeViewEx1.SelectedNode;
            if (sl != null)
            {
                
                    string projectName = txt_projectName.Text;
                    ImportStuentForm isf = new ImportStuentForm();
                    isf.sQLiteHelper = sQLiteHelper;
                    isf._projectName = projectName;

                    if (isf.ShowDialog() == DialogResult.OK)
                    {
                        FrmTips.ShowTipsSuccess(this, "导入成功");
                    }
                    else
                    {
                        FrmTips.ShowTipsError(this, "导入失败");
                    }

                    ControlHelper.ThreadInvokerControl(this, () =>
                    {
                        projectTreeUpdate();
                        //FrmTips.ShowTipsSuccess(this, "导入成功");
                    });
                
            }
            else
            {
                MessageBox.Show("请先选择或者新建考试项目！！");
                return;
            }


        }
        private bool ExcelListInput()
        {
            string path = "";
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
                if (!string.IsNullOrEmpty(path))
                {
                    string projectName = txt_projectName.Text;
                    string projectid = sQLiteHelper.ExecuteScalar($"select Id from SportProjectInfos where name='{projectName}'").ToString();
                    var rows = MiniExcel.Query<ExportStudentData>(path).ToList();
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
                            string insertsql = $"INSERT INTO DbPersonInfos(CreateTime,SortId,IsRemoved,ProjectId,SchoolName,GradeName,ClassNumber,GroupName,Name,IdNumber,Sex,State,FinalScore) " +
                                $"VALUES(datetime(CURRENT_TIMESTAMP, 'localtime'),{personsortid},0,'{projectid}','{SchoolName}','{GradeName}','{classNumber}','{GroupName}'," +
                                $"'{name}','{PersonIdNumber}',{Sex},0,-1)";
                            sQLiteHelper.ExecuteNonQuery(insertsql);
                        }

                    }
                    sQLiteHelper.CommitTransAction(sQLiteTransaction);

                    ControlHelper.ThreadInvokerControl(this, () =>
                    {
                        projectTreeUpdate();
                        FrmTips.ShowTipsSuccess(this, "导入成功");
                    });


                    return true;
                }

            }
            return false;
        }

        /// <summary>
        /// 名单模板导出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
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
        /// 圈数设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            TurnsSetForm turnsSetForm = new TurnsSetForm();
            turnsSetForm.projectId = projectId;
            turnsSetForm.sQLiteHelper = sQLiteHelper;
            if (turnsSetForm.ShowDialog() == DialogResult.OK)
            {
                FrmTips.ShowTipsSuccess(this, "设置圈数成功");
            }
        }
        /// <summary>
        /// 删除选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            deleteChoosePerson();
        }
        /// <summary>
        /// 删除本组
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button6_Click(object sender, EventArgs e)
        {
            //删除
            deleteAllGroup();
        }



        private void deleteChoosePerson()
        {
            int count = ucDataGridView1.SelectRows.Count;
            if (count > 0)
            {
                var value = sQLiteHelper.ExecuteScalar($"SELECT Id FROM SportProjectInfos WHERE Name='{txt_projectName.Text}'");
                string projectId = value.ToString();
                if (!string.IsNullOrEmpty(projectId))
                {
                    for (int i = 0; i < count; i++)
                    {
                        DataGridViewModel osoure = (DataGridViewModel)ucDataGridView1.SelectRows[i].DataSource;
                        var vpersonId = sQLiteHelper.ExecuteScalar($"SELECT  Id FROM DbPersonInfos WHERE ProjectId='{projectId}' and Name='{osoure.Name}' and IdNumber='{osoure.IdNumber}'");
                        //删除人
                        sQLiteHelper.ExecuteNonQuery($"DELETE FROM DbPersonInfos WHERE Id='{vpersonId}'");
                        //删除成绩
                        sQLiteHelper.ExecuteNonQuery($"DELETE FROM ResultInfos WHERE PersonId='{vpersonId}'");

                    }
                    ucDataGridView1Update(txt_GroupName.Text);
                    FrmTips.ShowTipsSuccess(this, "删除成功");
                }


            }
        }



        private void deleteAllGroup()
        {
            string delGroupName = txt_GroupName.Text;
            var value = sQLiteHelper.ExecuteScalar($"SELECT Id FROM SportProjectInfos WHERE Name='{txt_projectName.Text}'");
            string projectId = value.ToString();
            if (!string.IsNullOrEmpty(projectId))
            {
                //删除组
                sQLiteHelper.ExecuteNonQuery($"DELETE FROM DbGroupInfos WHERE ProjectId='{projectId}' and Name='{delGroupName}'");
                var ds = sQLiteHelper.ExecuteReader($"SELECT Id FROM DbPersonInfos WHERE ProjectId='{projectId}' AND GroupName='{delGroupName}'");
                while (ds.Read())
                {
                    var vpersonId = ds.GetValue(0).ToString(); ;
                    //删除成绩
                    sQLiteHelper.ExecuteNonQuery($"DELETE FROM ResultInfos WHERE PersonId='{vpersonId}'");
                }
                //删除人
                sQLiteHelper.ExecuteNonQuery($"DELETE FROM DbPersonInfos WHERE ProjectId='{projectId}' AND GroupName='{delGroupName}'");
                projectTreeUpdate();
                ucDataGridView1Update(txt_GroupName.Text);
                FrmTips.ShowTipsSuccess(this, "删除成功");

            }
        }


        private void treeViewEx1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ContextMenuStrip1.Show(treeViewEx1, e.Location);
            }
        }

        private void 插入项目ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateNewProject();
        }

        private void CreateNewProject()
        {
            FrmCreateNewProject frm = new FrmCreateNewProject();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                String NewProjectName = frm.ProjectName;
                string sql = $"select Id from SportProjectInfos where Name='{NewProjectName}' LIMIT 1";
                var existProject = sQLiteHelper.ExecuteScalar(sql);
                int si = 1;
                var ds = sQLiteHelper.ExecuteScalar($"SELECT MAX(SortId) + 1 FROM SportProjectInfos").ToString();
                int.TryParse(ds, out si);

                if (existProject != null)
                {
                    FrmTips.ShowTipsWarning(this, $"项目:{NewProjectName}已存在");
                }
                else
                {

                    sql = $"INSERT INTO SportProjectInfos (CreateTime, SortId, IsRemoved, Name, Type, RoundCount, BestScoreMode, TestMethod, FloatType ) " +
                        $"VALUES(datetime(CURRENT_TIMESTAMP, 'localtime'),{si}," +
                        $"0,'{NewProjectName}',0,2,0,0,2)";
                    int result = sQLiteHelper.ExecuteNonQuery(sql);
                    if (result == 1)
                    {
                        FrmTips.ShowTipsSuccess(this, $"添加成功");
                        projectTreeUpdate();
                    }
                }
            }

        }

        private void 删除项目ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeViewEx1.SelectedNode.Level != 0)
            {
                FrmTips.ShowTipsInfo(this, "请选择一个项目");
            }
            else
            {
                if (FrmDialog.ShowDialog(this, $"是否确认删除{treeViewEx1.SelectedNode.Text}项目？", "删除确认", true) == System.Windows.Forms.DialogResult.OK)
                {
                    DeleteProject(treeViewEx1.SelectedNode.Text);
                }


            }
        }

        private void DeleteProject(string projectName)
        {
            var value = sQLiteHelper.ExecuteScalar($"SELECT Id FROM SportProjectInfos WHERE Name='{projectName}'");
            string projectId = value.ToString();

            int result = sQLiteHelper.ExecuteNonQuery($"DELETE FROM SportProjectInfos WHERE Id = '{projectId}'");
            if (result == 1)
            {
                sQLiteHelper.ExecuteNonQuery($"DELETE FROM DbGroupInfos WHERE ProjectId = '{projectId}'");
                sQLiteHelper.ExecuteNonQuery($"DELETE FROM DbPersonInfos WHERE ProjectId = '{projectId}'");
                projectTreeUpdate();
            }
            FrmTips.ShowTipsSuccess(this, $"删除{projectName}");
        }

        private void ProjectSetForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //sQLiteHelper.CloseDb();
        }

        private void txt_Type_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}