namespace TrunkPressingCore.Window
{
    partial class ProjectSetForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ContextMenuStrip1 = new Sunny.UI.UIContextMenuStrip();
            this.插入项目ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.删除项目ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uiTitlePanel1 = new Sunny.UI.UITitlePanel();
            this.ucPanelTitle1 = new HZH_Controls.Controls.UCPanelTitle();
            this.treeViewEx1 = new Sunny.UI.UITreeView();
            this.ucPanelTitle4 = new HZH_Controls.Controls.UCPanelTitle();
            this.ucDataGridView1 = new HZH_Controls.Controls.UCDataGridView();
            this.ucPanelTitle3 = new HZH_Controls.Controls.UCPanelTitle();
            this.txt_GroupName = new Sunny.UI.UIComboBox();
            this.button6 = new Sunny.UI.UIButton();
            this.button5 = new Sunny.UI.UIButton();
            this.uiLabel7 = new Sunny.UI.UILabel();
            this.ucPanelTitle2 = new HZH_Controls.Controls.UCPanelTitle();
            this.txt_FloatType = new Sunny.UI.UIComboBox();
            this.uiLabel6 = new Sunny.UI.UILabel();
            this.txt_TestMethod = new Sunny.UI.UIComboBox();
            this.uiLabel5 = new Sunny.UI.UILabel();
            this.txt_BestScoreMode = new Sunny.UI.UIComboBox();
            this.uiLabel4 = new Sunny.UI.UILabel();
            this.txt_RoundCount = new Sunny.UI.UIComboBox();
            this.uiLabel3 = new Sunny.UI.UILabel();
            this.txt_Type = new Sunny.UI.UIComboBox();
            this.txt_projectName = new Sunny.UI.UITextBox();
            this.uiLabel2 = new Sunny.UI.UILabel();
            this.uiLabel1 = new Sunny.UI.UILabel();
            this.button4 = new Sunny.UI.UIButton();
            this.button3 = new Sunny.UI.UIButton();
            this.button2 = new Sunny.UI.UIButton();
            this.ContextMenuStrip1.SuspendLayout();
            this.uiTitlePanel1.SuspendLayout();
            this.ucPanelTitle1.SuspendLayout();
            this.ucPanelTitle4.SuspendLayout();
            this.ucPanelTitle3.SuspendLayout();
            this.ucPanelTitle2.SuspendLayout();
            this.SuspendLayout();
            // 
            // ContextMenuStrip1
            // 
            this.ContextMenuStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(249)))), ((int)(((byte)(255)))));
            this.ContextMenuStrip1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ContextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.插入项目ToolStripMenuItem,
            this.删除项目ToolStripMenuItem});
            this.ContextMenuStrip1.Name = "ContextMenuStrip1";
            this.ContextMenuStrip1.Size = new System.Drawing.Size(145, 56);
            this.ContextMenuStrip1.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            // 
            // 插入项目ToolStripMenuItem
            // 
            this.插入项目ToolStripMenuItem.Name = "插入项目ToolStripMenuItem";
            this.插入项目ToolStripMenuItem.Size = new System.Drawing.Size(144, 26);
            this.插入项目ToolStripMenuItem.Text = "插入项目";
            this.插入项目ToolStripMenuItem.Click += new System.EventHandler(this.插入项目ToolStripMenuItem_Click);
            // 
            // 删除项目ToolStripMenuItem
            // 
            this.删除项目ToolStripMenuItem.Name = "删除项目ToolStripMenuItem";
            this.删除项目ToolStripMenuItem.Size = new System.Drawing.Size(144, 26);
            this.删除项目ToolStripMenuItem.Text = "删除项目";
            this.删除项目ToolStripMenuItem.Click += new System.EventHandler(this.删除项目ToolStripMenuItem_Click);
            // 
            // uiTitlePanel1
            // 
            this.uiTitlePanel1.Controls.Add(this.ucPanelTitle1);
            this.uiTitlePanel1.Controls.Add(this.ucPanelTitle4);
            this.uiTitlePanel1.Controls.Add(this.ucPanelTitle3);
            this.uiTitlePanel1.Controls.Add(this.ucPanelTitle2);
            this.uiTitlePanel1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiTitlePanel1.Location = new System.Drawing.Point(4, -2);
            this.uiTitlePanel1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiTitlePanel1.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiTitlePanel1.Name = "uiTitlePanel1";
            this.uiTitlePanel1.Padding = new System.Windows.Forms.Padding(0, 35, 0, 0);
            this.uiTitlePanel1.ShowText = false;
            this.uiTitlePanel1.Size = new System.Drawing.Size(1512, 789);
            this.uiTitlePanel1.TabIndex = 4;
            this.uiTitlePanel1.Text = "德育龙测试页面";
            this.uiTitlePanel1.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.uiTitlePanel1.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            // 
            // ucPanelTitle1
            // 
            this.ucPanelTitle1.BackColor = System.Drawing.Color.Transparent;
            this.ucPanelTitle1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(160)))), ((int)(((byte)(255)))));
            this.ucPanelTitle1.ConerRadius = 10;
            this.ucPanelTitle1.Controls.Add(this.treeViewEx1);
            this.ucPanelTitle1.FillColor = System.Drawing.Color.White;
            this.ucPanelTitle1.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.ucPanelTitle1.IsCanExpand = true;
            this.ucPanelTitle1.IsExpand = false;
            this.ucPanelTitle1.IsRadius = true;
            this.ucPanelTitle1.IsShowRect = true;
            this.ucPanelTitle1.Location = new System.Drawing.Point(4, 40);
            this.ucPanelTitle1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ucPanelTitle1.Name = "ucPanelTitle1";
            this.ucPanelTitle1.Padding = new System.Windows.Forms.Padding(1);
            this.ucPanelTitle1.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(160)))), ((int)(((byte)(255)))));
            this.ucPanelTitle1.RectWidth = 1;
            this.ucPanelTitle1.Size = new System.Drawing.Size(286, 744);
            this.ucPanelTitle1.TabIndex = 5;
            this.ucPanelTitle1.Title = "项目结构";
            // 
            // treeViewEx1
            // 
            this.treeViewEx1.FillColor = System.Drawing.Color.White;
            this.treeViewEx1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.treeViewEx1.Location = new System.Drawing.Point(0, 41);
            this.treeViewEx1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.treeViewEx1.MinimumSize = new System.Drawing.Size(1, 1);
            this.treeViewEx1.Name = "treeViewEx1";
            this.treeViewEx1.ShowText = false;
            this.treeViewEx1.Size = new System.Drawing.Size(281, 697);
            this.treeViewEx1.Style = Sunny.UI.UIStyle.Custom;
            this.treeViewEx1.TabIndex = 1;
            this.treeViewEx1.Text = "uiTreeView1";
            this.treeViewEx1.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.treeViewEx1.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            this.treeViewEx1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.treeViewEx1_MouseDown);
            this.treeViewEx1.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeViewEx1_NodeMouseClick);
            // 
            // ucPanelTitle4
            // 
            this.ucPanelTitle4.BackColor = System.Drawing.Color.Transparent;
            this.ucPanelTitle4.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(160)))), ((int)(((byte)(255)))));
            this.ucPanelTitle4.ConerRadius = 10;
            this.ucPanelTitle4.Controls.Add(this.ucDataGridView1);
            this.ucPanelTitle4.FillColor = System.Drawing.Color.White;
            this.ucPanelTitle4.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.ucPanelTitle4.IsCanExpand = true;
            this.ucPanelTitle4.IsExpand = false;
            this.ucPanelTitle4.IsRadius = true;
            this.ucPanelTitle4.IsShowRect = true;
            this.ucPanelTitle4.Location = new System.Drawing.Point(298, 254);
            this.ucPanelTitle4.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ucPanelTitle4.Name = "ucPanelTitle4";
            this.ucPanelTitle4.Padding = new System.Windows.Forms.Padding(1);
            this.ucPanelTitle4.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(160)))), ((int)(((byte)(255)))));
            this.ucPanelTitle4.RectWidth = 1;
            this.ucPanelTitle4.Size = new System.Drawing.Size(1198, 530);
            this.ucPanelTitle4.TabIndex = 4;
            this.ucPanelTitle4.Title = "名单";
            // 
            // ucDataGridView1
            // 
            this.ucDataGridView1.BackColor = System.Drawing.Color.White;
            this.ucDataGridView1.Columns = null;
            this.ucDataGridView1.DataSource = null;
            this.ucDataGridView1.HeadFont = new System.Drawing.Font("微软雅黑", 12F);
            this.ucDataGridView1.HeadHeight = 40;
            this.ucDataGridView1.HeadPadingLeft = 0;
            this.ucDataGridView1.HeadTextColor = System.Drawing.Color.Black;
            this.ucDataGridView1.IsShowCheckBox = false;
            this.ucDataGridView1.IsShowHead = true;
            this.ucDataGridView1.Location = new System.Drawing.Point(4, 40);
            this.ucDataGridView1.Name = "ucDataGridView1";
            this.ucDataGridView1.Padding = new System.Windows.Forms.Padding(0, 40, 0, 0);
            this.ucDataGridView1.RowHeight = 40;
            this.ucDataGridView1.RowType = typeof(HZH_Controls.Controls.UCDataGridViewRow);
            this.ucDataGridView1.Size = new System.Drawing.Size(1190, 484);
            this.ucDataGridView1.TabIndex = 1;
            // 
            // ucPanelTitle3
            // 
            this.ucPanelTitle3.BackColor = System.Drawing.Color.Transparent;
            this.ucPanelTitle3.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(160)))), ((int)(((byte)(255)))));
            this.ucPanelTitle3.ConerRadius = 10;
            this.ucPanelTitle3.Controls.Add(this.txt_GroupName);
            this.ucPanelTitle3.Controls.Add(this.button6);
            this.ucPanelTitle3.Controls.Add(this.button5);
            this.ucPanelTitle3.Controls.Add(this.uiLabel7);
            this.ucPanelTitle3.FillColor = System.Drawing.Color.White;
            this.ucPanelTitle3.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.ucPanelTitle3.IsCanExpand = true;
            this.ucPanelTitle3.IsExpand = false;
            this.ucPanelTitle3.IsRadius = true;
            this.ucPanelTitle3.IsShowRect = true;
            this.ucPanelTitle3.Location = new System.Drawing.Point(1135, 40);
            this.ucPanelTitle3.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ucPanelTitle3.Name = "ucPanelTitle3";
            this.ucPanelTitle3.Padding = new System.Windows.Forms.Padding(1);
            this.ucPanelTitle3.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(160)))), ((int)(((byte)(255)))));
            this.ucPanelTitle3.RectWidth = 1;
            this.ucPanelTitle3.Size = new System.Drawing.Size(361, 204);
            this.ucPanelTitle3.TabIndex = 3;
            this.ucPanelTitle3.Title = "组别操作";
            // 
            // txt_GroupName
            // 
            this.txt_GroupName.DataSource = null;
            this.txt_GroupName.FillColor = System.Drawing.Color.White;
            this.txt_GroupName.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txt_GroupName.Location = new System.Drawing.Point(90, 40);
            this.txt_GroupName.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txt_GroupName.MinimumSize = new System.Drawing.Size(63, 0);
            this.txt_GroupName.Name = "txt_GroupName";
            this.txt_GroupName.Padding = new System.Windows.Forms.Padding(0, 0, 30, 2);
            this.txt_GroupName.Size = new System.Drawing.Size(246, 29);
            this.txt_GroupName.TabIndex = 5;
            this.txt_GroupName.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.txt_GroupName.Watermark = "";
            this.txt_GroupName.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            // 
            // button6
            // 
            this.button6.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button6.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button6.Location = new System.Drawing.Point(244, 114);
            this.button6.MinimumSize = new System.Drawing.Size(1, 1);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(107, 35);
            this.button6.TabIndex = 4;
            this.button6.Text = "删除本组";
            this.button6.TipsFont = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button6.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // button5
            // 
            this.button5.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button5.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button5.Location = new System.Drawing.Point(11, 113);
            this.button5.MinimumSize = new System.Drawing.Size(1, 1);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(120, 35);
            this.button5.TabIndex = 3;
            this.button5.Text = "删除选择";
            this.button5.TipsFont = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button5.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // uiLabel7
            // 
            this.uiLabel7.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiLabel7.Location = new System.Drawing.Point(4, 40);
            this.uiLabel7.Name = "uiLabel7";
            this.uiLabel7.Size = new System.Drawing.Size(107, 23);
            this.uiLabel7.TabIndex = 1;
            this.uiLabel7.Text = "组别名称：";
            this.uiLabel7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.uiLabel7.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            // 
            // ucPanelTitle2
            // 
            this.ucPanelTitle2.BackColor = System.Drawing.Color.Transparent;
            this.ucPanelTitle2.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(160)))), ((int)(((byte)(255)))));
            this.ucPanelTitle2.ConerRadius = 10;
            this.ucPanelTitle2.Controls.Add(this.txt_FloatType);
            this.ucPanelTitle2.Controls.Add(this.uiLabel6);
            this.ucPanelTitle2.Controls.Add(this.txt_TestMethod);
            this.ucPanelTitle2.Controls.Add(this.uiLabel5);
            this.ucPanelTitle2.Controls.Add(this.txt_BestScoreMode);
            this.ucPanelTitle2.Controls.Add(this.uiLabel4);
            this.ucPanelTitle2.Controls.Add(this.txt_RoundCount);
            this.ucPanelTitle2.Controls.Add(this.uiLabel3);
            this.ucPanelTitle2.Controls.Add(this.txt_Type);
            this.ucPanelTitle2.Controls.Add(this.txt_projectName);
            this.ucPanelTitle2.Controls.Add(this.uiLabel2);
            this.ucPanelTitle2.Controls.Add(this.uiLabel1);
            this.ucPanelTitle2.Controls.Add(this.button4);
            this.ucPanelTitle2.Controls.Add(this.button3);
            this.ucPanelTitle2.Controls.Add(this.button2);
            this.ucPanelTitle2.FillColor = System.Drawing.Color.White;
            this.ucPanelTitle2.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.ucPanelTitle2.IsCanExpand = true;
            this.ucPanelTitle2.IsExpand = false;
            this.ucPanelTitle2.IsRadius = true;
            this.ucPanelTitle2.IsShowRect = true;
            this.ucPanelTitle2.Location = new System.Drawing.Point(298, 40);
            this.ucPanelTitle2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ucPanelTitle2.Name = "ucPanelTitle2";
            this.ucPanelTitle2.Padding = new System.Windows.Forms.Padding(1);
            this.ucPanelTitle2.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(160)))), ((int)(((byte)(255)))));
            this.ucPanelTitle2.RectWidth = 1;
            this.ucPanelTitle2.Size = new System.Drawing.Size(829, 204);
            this.ucPanelTitle2.TabIndex = 2;
            this.ucPanelTitle2.Title = "项目操作";
            // 
            // txt_FloatType
            // 
            this.txt_FloatType.DataSource = null;
            this.txt_FloatType.FillColor = System.Drawing.Color.White;
            this.txt_FloatType.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txt_FloatType.Items.AddRange(new object[] {
            "小数点后0位",
            "小数点后1位",
            "小数点后2位",
            "小数点后3位",
            "小数点后4位"});
            this.txt_FloatType.Location = new System.Drawing.Point(618, 122);
            this.txt_FloatType.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txt_FloatType.MinimumSize = new System.Drawing.Size(63, 0);
            this.txt_FloatType.Name = "txt_FloatType";
            this.txt_FloatType.Padding = new System.Windows.Forms.Padding(0, 0, 30, 2);
            this.txt_FloatType.Size = new System.Drawing.Size(142, 27);
            this.txt_FloatType.TabIndex = 15;
            this.txt_FloatType.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.txt_FloatType.Watermark = "";
            this.txt_FloatType.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            // 
            // uiLabel6
            // 
            this.uiLabel6.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiLabel6.Location = new System.Drawing.Point(538, 118);
            this.uiLabel6.Name = "uiLabel6";
            this.uiLabel6.Size = new System.Drawing.Size(95, 31);
            this.uiLabel6.TabIndex = 14;
            this.uiLabel6.Text = "保留位数：";
            this.uiLabel6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.uiLabel6.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            // 
            // txt_TestMethod
            // 
            this.txt_TestMethod.DataSource = null;
            this.txt_TestMethod.FillColor = System.Drawing.Color.White;
            this.txt_TestMethod.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txt_TestMethod.Items.AddRange(new object[] {
            "自动下一位",
            "自动下一轮"});
            this.txt_TestMethod.Location = new System.Drawing.Point(618, 83);
            this.txt_TestMethod.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txt_TestMethod.MinimumSize = new System.Drawing.Size(63, 0);
            this.txt_TestMethod.Name = "txt_TestMethod";
            this.txt_TestMethod.Padding = new System.Windows.Forms.Padding(0, 0, 30, 2);
            this.txt_TestMethod.Size = new System.Drawing.Size(142, 29);
            this.txt_TestMethod.TabIndex = 13;
            this.txt_TestMethod.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.txt_TestMethod.Watermark = "";
            this.txt_TestMethod.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            // 
            // uiLabel5
            // 
            this.uiLabel5.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiLabel5.Location = new System.Drawing.Point(534, 83);
            this.uiLabel5.Name = "uiLabel5";
            this.uiLabel5.Size = new System.Drawing.Size(99, 23);
            this.uiLabel5.TabIndex = 12;
            this.uiLabel5.Text = "比赛方式：";
            this.uiLabel5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.uiLabel5.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            // 
            // txt_BestScoreMode
            // 
            this.txt_BestScoreMode.DataSource = null;
            this.txt_BestScoreMode.FillColor = System.Drawing.Color.White;
            this.txt_BestScoreMode.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txt_BestScoreMode.Items.AddRange(new object[] {
            "末位删除",
            "非零进一",
            "四舍五入"});
            this.txt_BestScoreMode.Location = new System.Drawing.Point(363, 120);
            this.txt_BestScoreMode.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txt_BestScoreMode.MinimumSize = new System.Drawing.Size(63, 0);
            this.txt_BestScoreMode.Name = "txt_BestScoreMode";
            this.txt_BestScoreMode.Padding = new System.Windows.Forms.Padding(0, 0, 30, 2);
            this.txt_BestScoreMode.Size = new System.Drawing.Size(162, 29);
            this.txt_BestScoreMode.TabIndex = 11;
            this.txt_BestScoreMode.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.txt_BestScoreMode.Watermark = "";
            this.txt_BestScoreMode.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            // 
            // uiLabel4
            // 
            this.uiLabel4.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiLabel4.Location = new System.Drawing.Point(286, 121);
            this.uiLabel4.Name = "uiLabel4";
            this.uiLabel4.Size = new System.Drawing.Size(96, 29);
            this.uiLabel4.TabIndex = 10;
            this.uiLabel4.Text = "成绩取值：";
            this.uiLabel4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.uiLabel4.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            // 
            // txt_RoundCount
            // 
            this.txt_RoundCount.DataSource = null;
            this.txt_RoundCount.FillColor = System.Drawing.Color.White;
            this.txt_RoundCount.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txt_RoundCount.Items.AddRange(new object[] {
            "0",
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9"});
            this.txt_RoundCount.Location = new System.Drawing.Point(363, 83);
            this.txt_RoundCount.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txt_RoundCount.MinimumSize = new System.Drawing.Size(63, 0);
            this.txt_RoundCount.Name = "txt_RoundCount";
            this.txt_RoundCount.Padding = new System.Windows.Forms.Padding(0, 0, 30, 2);
            this.txt_RoundCount.Size = new System.Drawing.Size(163, 29);
            this.txt_RoundCount.TabIndex = 9;
            this.txt_RoundCount.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.txt_RoundCount.Watermark = "";
            this.txt_RoundCount.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            // 
            // uiLabel3
            // 
            this.uiLabel3.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiLabel3.Location = new System.Drawing.Point(282, 83);
            this.uiLabel3.Name = "uiLabel3";
            this.uiLabel3.Size = new System.Drawing.Size(90, 23);
            this.uiLabel3.TabIndex = 8;
            this.uiLabel3.Text = "比赛轮次：";
            this.uiLabel3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.uiLabel3.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            // 
            // txt_Type
            // 
            this.txt_Type.DataSource = null;
            this.txt_Type.FillColor = System.Drawing.Color.White;
            this.txt_Type.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txt_Type.Items.AddRange(new object[] {
            "立定跳远",
            "投掷实心球",
            "坐位体前屈",
            "投掷铅球"});
            this.txt_Type.Location = new System.Drawing.Point(106, 121);
            this.txt_Type.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txt_Type.MinimumSize = new System.Drawing.Size(63, 0);
            this.txt_Type.Name = "txt_Type";
            this.txt_Type.Padding = new System.Windows.Forms.Padding(0, 0, 30, 2);
            this.txt_Type.Size = new System.Drawing.Size(167, 29);
            this.txt_Type.TabIndex = 7;
            this.txt_Type.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.txt_Type.Watermark = "";
            this.txt_Type.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            // 
            // txt_projectName
            // 
            this.txt_projectName.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txt_projectName.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txt_projectName.Location = new System.Drawing.Point(106, 83);
            this.txt_projectName.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txt_projectName.MinimumSize = new System.Drawing.Size(1, 16);
            this.txt_projectName.Name = "txt_projectName";
            this.txt_projectName.ShowText = false;
            this.txt_projectName.Size = new System.Drawing.Size(167, 29);
            this.txt_projectName.TabIndex = 6;
            this.txt_projectName.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.txt_projectName.Watermark = "";
            this.txt_projectName.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            // 
            // uiLabel2
            // 
            this.uiLabel2.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiLabel2.Location = new System.Drawing.Point(12, 119);
            this.uiLabel2.Name = "uiLabel2";
            this.uiLabel2.Size = new System.Drawing.Size(100, 23);
            this.uiLabel2.TabIndex = 5;
            this.uiLabel2.Text = "项目类型：";
            this.uiLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.uiLabel2.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            // 
            // uiLabel1
            // 
            this.uiLabel1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiLabel1.Location = new System.Drawing.Point(16, 83);
            this.uiLabel1.Name = "uiLabel1";
            this.uiLabel1.Size = new System.Drawing.Size(83, 23);
            this.uiLabel1.TabIndex = 4;
            this.uiLabel1.Text = "项目名称:";
            this.uiLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.uiLabel1.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            // 
            // button4
            // 
            this.button4.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button4.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button4.Location = new System.Drawing.Point(260, 40);
            this.button4.MinimumSize = new System.Drawing.Size(1, 1);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(112, 35);
            this.button4.TabIndex = 3;
            this.button4.Text = "保存项目设置";
            this.button4.TipsFont = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button4.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button3
            // 
            this.button3.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button3.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button3.Location = new System.Drawing.Point(137, 40);
            this.button3.MinimumSize = new System.Drawing.Size(1, 1);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(100, 35);
            this.button3.TabIndex = 2;
            this.button3.Text = "模板导出";
            this.button3.TipsFont = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button3.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button2
            // 
            this.button2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button2.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button2.Location = new System.Drawing.Point(16, 41);
            this.button2.MinimumSize = new System.Drawing.Size(1, 1);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(100, 35);
            this.button2.TabIndex = 1;
            this.button2.Text = "名单导入";
            this.button2.TipsFont = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button2.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // ProjectSetForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1518, 786);
            this.Controls.Add(this.uiTitlePanel1);
            this.Name = "ProjectSetForm";
            this.Text = "项目设置";
            this.Load += new System.EventHandler(this.ProjectSetForm_Load);
            this.ContextMenuStrip1.ResumeLayout(false);
            this.uiTitlePanel1.ResumeLayout(false);
            this.ucPanelTitle1.ResumeLayout(false);
            this.ucPanelTitle4.ResumeLayout(false);
            this.ucPanelTitle3.ResumeLayout(false);
            this.ucPanelTitle2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private Sunny.UI.UIContextMenuStrip ContextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 插入项目ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 删除项目ToolStripMenuItem;
        private Sunny.UI.UITitlePanel uiTitlePanel1;
        private HZH_Controls.Controls.UCPanelTitle ucPanelTitle1;
        private Sunny.UI.UITreeView treeViewEx1;
        private HZH_Controls.Controls.UCPanelTitle ucPanelTitle4;
        private HZH_Controls.Controls.UCDataGridView ucDataGridView1;
        private HZH_Controls.Controls.UCPanelTitle ucPanelTitle3;
        private Sunny.UI.UIComboBox txt_GroupName;
        private Sunny.UI.UIButton button6;
        private Sunny.UI.UIButton button5;
        private Sunny.UI.UILabel uiLabel7;
        private HZH_Controls.Controls.UCPanelTitle ucPanelTitle2;
        private Sunny.UI.UIComboBox txt_FloatType;
        private Sunny.UI.UILabel uiLabel6;
        private Sunny.UI.UIComboBox txt_TestMethod;
        private Sunny.UI.UILabel uiLabel5;
        private Sunny.UI.UIComboBox txt_BestScoreMode;
        private Sunny.UI.UILabel uiLabel4;
        private Sunny.UI.UIComboBox txt_RoundCount;
        private Sunny.UI.UILabel uiLabel3;
        private Sunny.UI.UIComboBox txt_Type;
        private Sunny.UI.UITextBox txt_projectName;
        private Sunny.UI.UILabel uiLabel2;
        private Sunny.UI.UILabel uiLabel1;
        private Sunny.UI.UIButton button4;
        private Sunny.UI.UIButton button3;
        private Sunny.UI.UIButton button2;
    }
}