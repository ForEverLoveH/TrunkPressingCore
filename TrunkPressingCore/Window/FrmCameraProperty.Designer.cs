namespace TrunkPressingCore.Window
{
    partial class FrmCameraProperty
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
            this.uiLabel1 = new Sunny.UI.UILabel();
            this.uiLabel2 = new Sunny.UI.UILabel();
            this.uiLabel3 = new Sunny.UI.UILabel();
            this.comboBox_camera = new Sunny.UI.UIComboBox();
            this.comboBox2 = new Sunny.UI.UIComboBox();
            this.comboBox1 = new Sunny.UI.UIComboBox();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.comboBox1);
            this.panel3.Controls.Add(this.comboBox2);
            this.panel3.Controls.Add(this.comboBox_camera);
            this.panel3.Controls.Add(this.uiLabel3);
            this.panel3.Controls.Add(this.uiLabel2);
            this.panel3.Controls.Add(this.uiLabel1);
            this.panel3.Size = new System.Drawing.Size(584, 284);
            // 
            // uiLabel1
            // 
            this.uiLabel1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiLabel1.Location = new System.Drawing.Point(141, 28);
            this.uiLabel1.Name = "uiLabel1";
            this.uiLabel1.Size = new System.Drawing.Size(101, 23);
            this.uiLabel1.TabIndex = 0;
            this.uiLabel1.Text = "相机选择：";
            this.uiLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.uiLabel1.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            // 
            // uiLabel2
            // 
            this.uiLabel2.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiLabel2.Location = new System.Drawing.Point(150, 103);
            this.uiLabel2.Name = "uiLabel2";
            this.uiLabel2.Size = new System.Drawing.Size(77, 34);
            this.uiLabel2.TabIndex = 1;
            this.uiLabel2.Text = "分辨率：";
            this.uiLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.uiLabel2.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            // 
            // uiLabel3
            // 
            this.uiLabel3.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiLabel3.Location = new System.Drawing.Point(150, 165);
            this.uiLabel3.Name = "uiLabel3";
            this.uiLabel3.Size = new System.Drawing.Size(100, 30);
            this.uiLabel3.TabIndex = 2;
            this.uiLabel3.Text = "帧数：";
            this.uiLabel3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.uiLabel3.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            // 
            // comboBox_camera
            // 
            this.comboBox_camera.DataSource = null;
            this.comboBox_camera.FillColor = System.Drawing.Color.White;
            this.comboBox_camera.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.comboBox_camera.Location = new System.Drawing.Point(249, 28);
            this.comboBox_camera.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.comboBox_camera.MinimumSize = new System.Drawing.Size(63, 0);
            this.comboBox_camera.Name = "comboBox_camera";
            this.comboBox_camera.Padding = new System.Windows.Forms.Padding(0, 0, 30, 2);
            this.comboBox_camera.Size = new System.Drawing.Size(244, 29);
            this.comboBox_camera.TabIndex = 3;
            this.comboBox_camera.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.comboBox_camera.Watermark = "";
            this.comboBox_camera.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            this.comboBox_camera.SelectedIndexChanged += new System.EventHandler(this.comboBox_camera_SelectedIndexChanged);
            // 
            // comboBox2
            // 
            this.comboBox2.DataSource = null;
            this.comboBox2.FillColor = System.Drawing.Color.White;
            this.comboBox2.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.comboBox2.Items.AddRange(new object[] {
            "1280*720"});
            this.comboBox2.Location = new System.Drawing.Point(249, 103);
            this.comboBox2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.comboBox2.MinimumSize = new System.Drawing.Size(63, 0);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Padding = new System.Windows.Forms.Padding(0, 0, 30, 2);
            this.comboBox2.Size = new System.Drawing.Size(244, 34);
            this.comboBox2.TabIndex = 4;
            this.comboBox2.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.comboBox2.Watermark = "";
            this.comboBox2.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            this.comboBox2.SelectedIndexChanged += new System.EventHandler(this.comboBox2_SelectedIndexChanged);
            // 
            // comboBox1
            // 
            this.comboBox1.DataSource = null;
            this.comboBox1.FillColor = System.Drawing.Color.White;
            this.comboBox1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.comboBox1.Location = new System.Drawing.Point(249, 165);
            this.comboBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.comboBox1.MinimumSize = new System.Drawing.Size(63, 0);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Padding = new System.Windows.Forms.Padding(0, 0, 30, 2);
            this.comboBox1.Size = new System.Drawing.Size(244, 30);
            this.comboBox1.TabIndex = 5;
            this.comboBox1.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.comboBox1.Watermark = "";
            this.comboBox1.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // FrmCameraProperty
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 408);
            this.Name = "FrmCameraProperty";
            this.Text = "FrmCameraProperty";
            this.Load += new System.EventHandler(this.FrmCameraProperty_Load);
            this.SizeChanged += new System.EventHandler(this.FrmCameraProperty_SizeChanged);
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Sunny.UI.UILabel uiLabel1;
        private Sunny.UI.UIComboBox comboBox1;
        private Sunny.UI.UIComboBox comboBox2;
        private Sunny.UI.UIComboBox comboBox_camera;
        private Sunny.UI.UILabel uiLabel3;
        private Sunny.UI.UILabel uiLabel2;
    }
}