namespace TrunkPressingCore.Window
{
    partial class OutPutExcelScoreForm
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
            this.ExportAllGradeBtn = new Sunny.UI.UIButton();
            this.ExportCurrentGroupBtn = new Sunny.UI.UIButton();
            this.uiLabel1 = new Sunny.UI.UILabel();
            this.SuspendLayout();
            // 
            // ExportAllGradeBtn
            // 
            this.ExportAllGradeBtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ExportAllGradeBtn.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ExportAllGradeBtn.Location = new System.Drawing.Point(35, 331);
            this.ExportAllGradeBtn.MinimumSize = new System.Drawing.Size(1, 1);
            this.ExportAllGradeBtn.Name = "ExportAllGradeBtn";
            this.ExportAllGradeBtn.Size = new System.Drawing.Size(164, 35);
            this.ExportAllGradeBtn.TabIndex = 0;
            this.ExportAllGradeBtn.Text = "导出全部成绩";
            this.ExportAllGradeBtn.TipsFont = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ExportAllGradeBtn.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            this.ExportAllGradeBtn.Click += new System.EventHandler(this.ExportAllGradeBtn_Click);
            // 
            // ExportCurrentGroupBtn
            // 
            this.ExportCurrentGroupBtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ExportCurrentGroupBtn.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ExportCurrentGroupBtn.Location = new System.Drawing.Point(469, 331);
            this.ExportCurrentGroupBtn.MinimumSize = new System.Drawing.Size(1, 1);
            this.ExportCurrentGroupBtn.Name = "ExportCurrentGroupBtn";
            this.ExportCurrentGroupBtn.Size = new System.Drawing.Size(151, 35);
            this.ExportCurrentGroupBtn.TabIndex = 1;
            this.ExportCurrentGroupBtn.Text = "导出当前组成绩";
            this.ExportCurrentGroupBtn.TipsFont = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ExportCurrentGroupBtn.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            this.ExportCurrentGroupBtn.Click += new System.EventHandler(this.ExportCurrentGroupBtn_Click);
            // 
            // uiLabel1
            // 
            this.uiLabel1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiLabel1.Location = new System.Drawing.Point(35, 103);
            this.uiLabel1.Name = "uiLabel1";
            this.uiLabel1.Size = new System.Drawing.Size(597, 100);
            this.uiLabel1.TabIndex = 2;
            this.uiLabel1.Text = "uiLabel1";
            this.uiLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.uiLabel1.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            // 
            // OutPutExcelScoreForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.uiLabel1);
            this.Controls.Add(this.ExportCurrentGroupBtn);
            this.Controls.Add(this.ExportAllGradeBtn);
            this.Name = "OutPutExcelScoreForm";
            this.Text = "导出成绩";
            this.Load += new System.EventHandler(this.OutPutExcelScoreForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private Sunny.UI.UIButton ExportAllGradeBtn;
        private Sunny.UI.UIButton ExportCurrentGroupBtn;
        private Sunny.UI.UILabel uiLabel1;
    }
}