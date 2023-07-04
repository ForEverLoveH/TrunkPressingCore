using AForge.Controls;
using AForge.Video;
using AForge.Video.DirectShow;
using CustomControl;
using HZH_Controls;
using HZH_Controls.Forms;
using NPOI.HSSF.Record.Chart;
using NPOI.OpenXmlFormats.Spreadsheet;
using NPOI.OpenXmlFormats.Wordprocessing;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.XSSF.Streaming.Values;
using OpenCvSharp;
using SpeechLib;
using Sunny.UI;
using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Spire.Xls;
using TrunkPressingCore.GameConst;
using TrunkPressingCore.GameModel;
using TrunkPressingCore.GameSystem;
using TrunkPressingCore.SQLite;
 

namespace TrunkPressingCore.Window
{
    public partial class RunTestWindow : Form
    {
        public RunTestWindow()
        {
            InitializeComponent();
        }
        public static string strMainModule = System.AppDomain.CurrentDomain.BaseDirectory + "data\\";

        //项目编号
        public string _ProjectId = "";

        public string formTitle = "";

        public int _TestMethod = 0;
        //组名
        public string _GroupName = "";
        //项目名
        public string _ProjectName = "";
        //测试最大次数
        public int _RoundCount = 1;
        //当前测试轮次
        public int RoundCount0 = 1;
        //项目模式
        public string _Type = "";
        //最好成绩模式 0:取大值 1:取小值
        /// <summary>
        /// 0:末位删除 1:非零进一 2:四舍五入
        /// </summary>
        public int _BestScoreMode = 0;
        //测试模式 0:自动下一位 1:自动下一轮
        public int _TestMode = 0;
        //保留位数
        public int _FloatType = 0;
        //数据库
        public SQLiteHelper sQLiteHelper = null;

        //考试组分配信息
        RaceStudentData nowRaceStudentData = new RaceStudentData();

        string dangwei = "米";
        int pBox1Width = 0;
        int pBox1Height = 0;
        /// <summary>
        /// 存储考生数据
        /// </summary>
        class RaceStudentData
        {
            public RaceStudentData()
            {
                id = String.Empty;
                idNumber = String.Empty;
                name = String.Empty;
                groupName = String.Empty;
                score = 0;
                RoundId = 1;
                state = 0;
            }
            public string id;//编号
            public string idNumber;//考号
            public string name;//姓名
            public double score;//成绩
            public int RoundId;//轮次
                               //状态 0:未测试 1:已测试 2:中退 3:缺考 4:犯规 5:弃权
            public int state;//状态
            public string groupName;
        }
        public double MeasureLen = 0;//测量长度
        public double MeasureLenX = 0;//测量水平长度
        public double MeasureLenY = 0;//测量垂直长度
        Boolean Measure = true;//测量长度状态
        string markPointFile = "markPoint.dat";
        string nowFileName = "";//当前文件名
        string nowTestDir = String.Empty;//当前文件目录
        string ScoreDir = string.Empty;
        int recTimeR0 = 0;//计时时间
        int frameSum = 0;
        private Thread threadVideo2;
        private void RunTest_Load(object sender, EventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            string code = "程序集版本：" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            string code1 = "文件版本：" + Application.ProductVersion.ToString();
            VersionLabel.Text = code;
            AsyncMethodCaller caller = new AsyncMethodCaller(TestMethodAsync);
            var workTask = Task.Run(() => caller.Invoke());
            RunTestLoadInit();
            UpdateListView(_ProjectId, _GroupName, 1);
            ParameterizedThreadStart method = new ParameterizedThreadStart(ImagePredictLabelQueues2ThreadFun);
            threadVideo2 = new Thread(method);
            threadVideo2.IsBackground = true;
            threadVideo2.Start();
            serialInit();
            CameraInit();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RunTest_FormClosed(object sender, FormClosedEventArgs e)
        {
            CloseCamera();
        }

        /// <summary>
        /// 委托必须和要调用的异步方法有相同的签名
        /// </summary>
        /// <param name="callDuration">sleep时间</param>
        /// <param name="threadId">当前线程id</param>
        /// <returns></returns>
        public delegate string AsyncMethodCaller();
        /// 与委托对应的方法
        /// </summary>
        /// <param name="callDuration"></param>
        /// <param name="threadId"></param>
        /// <returns></returns>
        string TestMethodAsync()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();


            loadJumpInit();
            startMeasure();
            sw.Stop();
            string useTimeStr = string.Format("耗时{0}ms.", sw.ElapsedMilliseconds.ToString());
            return useTimeStr;
        }

        #region 初始化函数

       public void loadJumpInit()
        {
            ScoreDir = Path.Combine(strMainModule, "Score");
            if (!Directory.Exists(ScoreDir)) Directory.CreateDirectory(ScoreDir);
            string[] strg = TxtFile.Instance.Read(strMainModule + markPointFile);
            if (strg != null)
            {
                try
                {
                    if (strg.Length > 0)
                    {
                        foreach (var s in strg)
                        {
                            string[] ss1 = s.Split(';');
                            if (ss1.Length == 4)
                            {
                                System.Drawing.Point p = XYString2Point(ss1[1]);
                                bool bl = true;
                                if (ss1[3] == "0")
                                {
                                    bl = false;
                                }
                                targetPoints.Add(new TargetPoint()
                                {
                                    x = p.X,
                                    y = p.Y,
                                    name = ss1[0],
                                    dis = str2int(ss1[2]),//cm
                                    status = bl
                                });
                            }

                        }
                        updateTargetListView(true);
                    }
                }
                catch (Exception ex)
                {

                    LoggerHelper.Debug(ex);
                }
            }

            if (_FloatType == 0)
            {
                ReservedDigitsTxt = "0";
            }
            else
            {
                ReservedDigitsTxt = "0.";
                for (int i = 0; i < _FloatType; i++)
                {
                    ReservedDigitsTxt += "0";
                }
            }

            //strg = TxtFile.Instance.read(strMainModule + projectTypeCbxPath);
            /* if (strg.Length > 0 && strg[0] != "")
             {
                 //? 立定跳远
                 //? 投掷实心球
                 //? 坐位体前屈
                 //? 投掷铅球
                 if (strg[0] == "立定跳远")
                 {
                     projectTypeCbx.SelectedIndex = 0;
                 }
                 else if (strg[0] == "投掷实心球")
                 {
                     projectTypeCbx.SelectedIndex = 1;

                 }
                 else if (strg[0] == "坐位体前屈")
                 {
                     projectTypeCbx.SelectedIndex = 2;
                 }
                 else if (strg[0] == "投掷铅球")
                 {
                     projectTypeCbx.SelectedIndex = 3;
                 }
             }*/

        }
        /// <summary>
        /// 
        /// </summary>
        void RunTestLoadInit()
        {
            try
            {
                updateGroupCombox();
                updateRoundCountCombox();
                RoundCountCbx.SelectedIndex = 0;

                ProjectNameCbx.Text = _ProjectName;
                ProjectNameCbx.ReadOnly = true;
            }
            catch (Exception ex)
            {

                LoggerHelper.Debug(ex);
            }
        }
        #endregion


        #region 公用函数

        public double str2double(string str)
        {

            double i = 0;
            if (null == str)
                return 0;
            double.TryParse(str, out i);
            return i;
        }
        public int str2int(string str)
        {
            int i = 0;
            if (null == str)
                return 0;
            int.TryParse(str, out i);
            return i;
        }

        public System.Drawing.Point XYString2Point(string str)
        {
            string[] strg = str.Split(',');
            if (null == strg)
            {
                return new System.Drawing.Point(0, 0);
            }
            if (strg.Length == 1)
            {
                return new System.Drawing.Point(str2int(strg[0]), 0);
            }

            System.Drawing.Point p = new System.Drawing.Point(str2int(strg[0]), str2int(strg[1]));
            return p;
        }
        public void drawPointCross(Graphics g, System.Drawing.Point markerTop1, Pen pen)
        {

            g.DrawLine(pen, markerTop1.X - 15, markerTop1.Y, markerTop1.X + 15, markerTop1.Y);
            g.DrawLine(pen, markerTop1.X, markerTop1.Y - 15, markerTop1.X, markerTop1.Y + 15);
        }


        public void drawPointText(Graphics g, String text, System.Drawing.Point point, Font drawFont, SolidBrush drawBrush,int directionx, int distancex, int directiony, int distancey)
        {
            //directiony 0 1 上下
            //directionx 0 1 左右

            int x = point.X;
            int y = point.Y;
            switch (directionx)
            {
                case 0:
                    x -= distancex;
                    break;
                case 1:
                    x += distancex;
                    break;
                default:
                    break;
            }

            switch (directiony)
            {
                case 0:
                    y -= distancey;
                    break;
                case 1:
                    y += distancey;
                    break;

                default:
                    break;
            }

            g.DrawString(text, drawFont, drawBrush, x, y);
        }


        #endregion

        #region 定时器模块
        private void timer1_Tick(object sender, EventArgs e)
        {
            double v = MemoryTool.GetProcessUsedMemory();
            if (v > 100)
            {
                MemoryTool.ClearMemory();
            }

            frameSpeed_txt.Text = "fps:" + frameRecSum;
            frameRecSum = 0;
        }
        private void timer2_Tick(object sender, EventArgs e)
        {
            if (RecordEnd == 1)
            {
                if (remainderImgSum > 0)
                {
                    imgProgressBar.Maximum = remainderImgSum;
                    imgProgressBar.Value = remainderImgCount;
                }

            }
            else if (RecordEnd == 2)
            {
                if (remainderImgSum > 0)
                {
                    imgProgressBar.Maximum = remainderImgSum;
                    imgProgressBar.Value = remainderImgSum;
                }

                RecordEnd = 0;
            }
            if (recTimeR0 > 0)
            {
                recTimeR0--;
                if (recTimeR0 == 0)
                {
                    try
                    {

                        rgbVideoSourceStop();
                        nowRaceStudentData.state = 1;
                        ControlHelper.ThreadInvokerControl(this, () =>
                        {
                            button12.Text = "开始录像(空格)";
                            button12.BackColor = Color.White;
                        });
                        recTime.Text = "文件数:" + frameSum + "";
                        videoSourceRuningR0 = 0;//播放中的心跳包
                                                //录像结束进行操作
                                                //打开图片集
                        RecordEnd = 1;
                        ReleaseVideoOutPut();
                        if (isShowImgList)
                        {
                            openImgList();
                        }
                        else
                        {
                            if (isSitting)
                            {
                                int nlen = imgMs.Count;
                                int maxW = 0;
                                int maxH = 0;
                                int index = 0;
                                int minWidth = conPoints0[0].X;
                                int maxWidth = conPoints0[3].X;
                                int minHeigh = conPoints0[0].Y;
                                int maxHeigh = conPoints0[3].Y;
                                int anaylistLen = 10;
                                int[] anaylist = new int[anaylistLen];
                                OpenCvSharp.Point[] anaPoint = new OpenCvSharp.Point[nlen];
                                for (int k = 0; k < nlen; k++)
                                {
                                    bool[][] isHand = imgMs[k].isHand;
                                    //maxW = 0;
                                    //maxH = 0;
                                    int maxwT = 0;
                                    int maxhT = 0;
                                    if (isHand == null) continue;
                                    for (int i = minWidth; i < maxWidth; i++)
                                    {
                                        for (int j = minHeigh; j < maxHeigh; j++)
                                        {
                                            if (isHand[i][j])
                                            {
                                                int nl = i - 5;
                                                if (nl < 0) nl = 0;
                                                bool flag0 = true;
                                                for (int i1 = nl; i1 < i; i1++)
                                                {
                                                    if (!isHand[i1][j])
                                                    {
                                                        flag0 = false; break;
                                                    }
                                                }
                                                if (!flag0) continue;
                                                //当前图片最大值
                                                maxwT = i;
                                                maxhT = j;
                                                //全局最大值
                                                if (i > maxW)
                                                {
                                                    maxW = i;
                                                    maxH = j;
                                                    index = k;
                                                }
                                            }
                                        }
                                    }
                                    anaPoint[k] = new OpenCvSharp.Point(maxwT, maxhT);
                                }
                                string Log = $"最大值:index:{index},X:{maxW}  y:{maxH}";
                                WriteLog(lrtxtLog, Log, 0);
                                int iFori = -1;
                                int iFlag = 0;
                                for (int i = index; i > 0; i--)
                                {
                                    bool isBreakForj = true;
                                    int ax = anaPoint[i].X;
                                    int end = (index + 2) > nlen - 1 ? nlen - 1 : index + 2;
                                    int begin = (index - 2) < 0 ? 0 : index - 2;
                                    for (int j = begin; j < end; j++)
                                    {
                                        int ipx = Math.Abs(ax - anaPoint[j].X);
                                        if (ipx > 3) isBreakForj = false; break;
                                    }
                                    if (isBreakForj)
                                    {
                                        iFori = i; break;
                                    }
                                }
                                /* for (int i = 0; i < nlen; i++)
                                 {
                                    int ax = anaPoint[i].X;
                                    if (iFlag >= anaylistLen)
                                    {
                                       bool isBreakForj = true;
                                       for (int j = 0; j < anaylistLen - 1; j++)
                                       {
                                          anaylist[j] = anaylist[j + 1];
                                          if (isBreakForj)
                                          {
                                             int ipx = Math.Abs(ax - anaylist[j]);
                                             if (ipx > 3) isBreakForj = false;
                                          }
                                       }
                                       if (isBreakForj)
                                       {
                                          iFori = i; break;
                                       }
                                    }
                                    anaylist[anaylistLen - 1] = ax;
                                    if (ax > 0) iFlag++;
                                 }*/
                                if (iFori != -1)
                                {
                                    int maxwT = anaPoint[iFori].X;
                                    int maxhT = anaPoint[iFori].Y;
                                    if ((maxwT < maxWidth && maxwT > minWidth) && maxhT < maxHeigh && maxhT > minHeigh)
                                    {
                                        maxW = maxwT;
                                        maxH = maxhT;
                                        index = iFori;
                                    }
                                }
                                Log = $"筛选值:index:{index},X:{maxW}  y:{maxH}";
                                WriteLog(lrtxtLog, Log, 0);

                                if (imgMs[index] != null && imgMs[index].img != null)
                                {
                                    nowFileName = index + "";
                                    ControlHelper.ThreadInvokerControl(this, () =>
                                    {
                                        pictureBox1.Image = imgMs[index].img;
                                        recImgIndex.Text = $"图片索引:{nowFileName}";
                                        pictureBox1.Refresh();

                                    });

                                }
                                startMeasure();
                                dispJumpLength1(maxW, maxH);
                                pictureBox1.Refresh();
                                stopMeasure();

                            }
                            else
                            {
                                setHScrollBarValue(imgMs.Count - 1);
                            }

                        }
                    }
                    catch (Exception ex)
                    {

                        LoggerHelper.Debug(ex);
                    }

                }
                else
                {
                    recTime.Text = "REC " + (recTimeR0 + 10) / 10;
                }
            }
        }


        #endregion

        bool UpdateListViewRun = false;
        /// <summary>
        /// 更新名单视图
        /// </summary>
        /// <param name="ProjectId"></param>
        /// <param name="GroupName"></param>
        /// <param name="roundId"></param>
        void UpdateListView(string ProjectId, string GroupName, int roundId)
        {

            if (UpdateListViewRun) return;
            UpdateListViewRun = true;
            try
            {
                stuView.Rows.Clear();
                var ds = sQLiteHelper.ExecuteReaderList($"SELECT Id,Name,IdNumber FROM DbPersonInfos WHERE ProjectId='{ProjectId}' and GroupName='{GroupName}'");
                int listviewCount = 1;
                foreach (var dic in ds)
                {
                    int k = stuView.Rows.Add(new DataGridViewRow());
                    string stuId = dic["Id"];
                    string stuName = dic["Name"];
                    string idNumber = dic["IdNumber"];
                    var ds1 = sQLiteHelper.ExecuteReaderList($"SELECT PersonName,Result,State,uploadState FROM ResultInfos WHERE PersonId='{stuId}' AND RoundId={roundId}");
                    bool flag = true;
                    stuView.Rows[k].Cells[0].Value = listviewCount.ToString();
                    stuView.Rows[k].Cells[1].Value = idNumber;
                    stuView.Rows[k].Cells[2].Value = stuName;
                    stuView.Rows[k].Cells[6].Value = stuId;

                    //序号 考号 姓名 成绩 考试状态 上传状态 唯一编号
                    foreach (var item in ds1)
                    {
                        flag = false;
                        string PersonName0 = item["PersonName"];
                        double.TryParse(item["Result"], out double Result0);
                        int.TryParse(item["State"], out int State0);
                        int.TryParse(item["uploadState"], out int uploadState0);
                        string sstate = ResultState.ResultState2Str(State0);
                        if (State0 > 1)
                        {
                            //犯规异常操作

                            stuView.Rows[k].Cells[3].Value = 0;
                            stuView.Rows[k].Cells[4].Value = sstate;
                            stuView.Rows[k].Cells[4].Style.ForeColor = Color.Red;
                        }
                        else if (State0 != 0)
                        {
                            stuView.Rows[k].Cells[3].Value = Result0.ToString();
                            stuView.Rows[k].Cells[4].Value = sstate;
                            stuView.Rows[k].DefaultCellStyle.BackColor = Color.MediumSpringGreen;
                        }
                        if (uploadState0 > 0)
                        {
                            stuView.Rows[k].Cells[5].Value = "已上传";
                        }
                        else
                        {
                            stuView.Rows[k].Cells[5].Value = "未上传";
                            stuView.Rows[k].Cells[5].Style.ForeColor = Color.Red;
                        }
                        break;
                    }

                    if (flag)
                    {
                        stuView.Rows[k].Cells[3].Value = "未测试";
                        stuView.Rows[k].Cells[4].Value = "未测试";
                        stuView.Rows[k].Cells[5].Value = "未上传";
                        stuView.Rows[k].Cells[5].Style.ForeColor = Color.Red;
                    }
                    listviewCount++;
                }
            }
            catch (Exception ex)
            {
                LoggerHelper.Debug(ex);
            }
            finally
            {
                stuView.ClearSelection();
                UpdateListViewRun = false;
            }

        }
        /// <summary>
        /// 更新本项目轮次次数
        /// </summary>
        void updateRoundCountCombox()
        {
            try
            {
                RoundCountCbx.Items.Clear();
                for (int i = 1; i <= _RoundCount; i++)
                {
                    RoundCountCbx.Items.Add(i.ToString());
                }
            }
            catch (Exception ex)
            {

                LoggerHelper.Debug(ex);
            }
        }

        /// <summary>
        /// 更新组信息
        /// </summary>
        /// <param name="groupname"></param>
        void updateGroupCombox(string groupname = "")
        {
            try
            {
                groupCbx.Items.Clear();
                groupCbx.Text = "";
                var ds = sQLiteHelper.ExecuteReader($"SELECT Name FROM DbGroupInfos WHERE Name LIKE'%{groupname}%' AND ProjectId='{_ProjectId}'");
                while (ds.Read())
                {
                    groupCbx.Items.Add(ds.GetString(0));
                }
                int index = -1;
                groupCbx.SelectedIndex = index;
                groupCbx.Text = "";
                if (string.IsNullOrEmpty(_GroupName) && groupCbx.Items.Count > 0)
                {
                    _GroupName = groupCbx.Items[0].ToString();
                    groupCbx.SelectedIndex = 0;
                }
                else
                {
                    if ((index = groupCbx.Items.IndexOf(_GroupName)) >= 0)
                    {
                        groupCbx.SelectedIndex = index;
                    }
                }
            }
            catch (Exception ex)
            {

                LoggerHelper.Debug(ex);
            }
        }

        /// <summary>
        /// 筛选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            updateGroupCombox(textBox1.Text);
        }

        /// <summary>
        /// 刷新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            _GroupName = "";
            updateGroupCombox();

        }

        /// <summary>
        /// 选择轮次触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RoundCountCbx_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (RoundCountCbx.SelectedIndex != -1)
            {
                RoundCount0 = RoundCountCbx.SelectedIndex + 1;
                UpdateListView(_ProjectId, _GroupName, RoundCount0);
                label7.Text = RoundCountCbx.Text;
                nowRaceStudentData = new RaceStudentData();
            }
        }


        /// <summary>
        /// 选择组触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void groupCbx_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ControlHelper.ThreadInvokerControl(this, () =>
                {
                    _GroupName = groupCbx.Text;
                    label13.Text = "当前测试组:" + _GroupName;
                    if (RoundCount0 > 0)
                    {
                        UpdateListView(_ProjectId, _GroupName, RoundCount0);
                        nowRaceStudentData = new RaceStudentData();
                    }
                });

            }
            catch (Exception ex)
            {

                LoggerHelper.Debug(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void refreshStuViewBtn_Click(object sender, EventArgs e)
        {
            if (RoundCount0 > 0)
            {
                UpdateListView(_ProjectId, _GroupName, RoundCount0);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        void setHScrollBarValue(int value)
        {
            ControlHelper.ThreadInvokerControl(this, () =>
            {
                hScrollBar1.Value = 0;
                hScrollBar1.Maximum = imgMs.Count - 1;
                if (value > imgMs.Count - 1)
                {
                    value = imgMs.Count - 1;
                }
                if (value < 0)
                {
                    value = 0;
                }
                hScrollBar1.Value = value;
            });

        }
        #region 测距模块
        /// <summary>
        /// 开始测距
        /// </summary>
        void startMeasure()
        {
            Measure = true;//测量长度
            ControlHelper.ThreadInvokerControl(this, () =>
            {
                button13.Text = "测量中(S)";
                button13.BackColor = Color.Red;
            });
        }

        void stopMeasure()
        {
            Measure = false;//测量长度
            ControlHelper.ThreadInvokerControl(this, () =>
            {
                button13.Text = "测量(S)";
                button13.BackColor = Color.White;
            });
        }
        /// <summary>
        /// 
        /// </summary>
        void MeasureFun()
        {
            if (Measure)
            {
                stopMeasure();
            }
            else
            {
                startMeasure();
            }
        }
        /// <summary>
        /// 计算角度
        /// </summary>
        /// <param name="cen"></param>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public double Angle(System.Drawing.Point cen, System.Drawing.Point first, System.Drawing.Point second)
        {
            const double M_PI = 3.1415926535897;
            double ma_x = first.X - cen.X;
            double ma_y = first.Y - cen.Y;
            double mb_x = second.X - cen.X;
            double mb_y = second.Y - cen.Y;

            double v1 = (ma_x * mb_x) + (ma_y * mb_y);

            double ma_val = Math.Sqrt(ma_x * ma_x + ma_y * ma_y);
            double mb_val = Math.Sqrt(mb_x * mb_x + mb_y * mb_y);
            double cosM = v1 / (ma_val * mb_val);
            double angleAMB = Math.Acos(cosM) * 180 / M_PI;

            return angleAMB;
        }
        int falseR0 = 0;

        /// <summary>
        /// 根据长度,求两坐标之间的点坐标
        /// </summary>
        /// <param name="len"></param>
        /// <param name="baseStartP"></param>
        /// <param name="baseEndP"></param>
        /// <returns></returns>
        System.Drawing.Point len2xy(double len, System.Drawing.Point baseStartP, System.Drawing.Point baseEndP)
        {
            System.Drawing.Point pout = new System.Drawing.Point();

            double dy = baseEndP.Y - baseStartP.Y;
            double dx = baseEndP.X - baseStartP.X;

            // double top1k = dy / dx;
            double len1 = len;// topLength1;
            double xt1 = Math.Sqrt(len1 * len1 / (dy * dy / (dx * dx) + 1));
            double yt1 = dy * xt1 / dx;

            pout.X = baseStartP.X + (int)(xt1 + 0.5);
            pout.Y = baseStartP.Y + (int)(yt1 + 0.5);

            return pout;
        }
        System.Drawing.Point len2yx(double len, System.Drawing.Point baseStartP, System.Drawing.Point baseEndP)
        {
            System.Drawing.Point pout = new System.Drawing.Point();

            double dx = baseEndP.X - baseStartP.X;
            double dy = baseEndP.Y - baseStartP.Y;
            double len1 = len;// topLength1;

            double yt1 = len1 / Math.Sqrt((dx * dx) / (dy * dy) + 1);
            double xt1 = (dx * yt1) / dy;


            pout.X = baseStartP.X + (int)(xt1 + 0.5);
            pout.Y = baseStartP.Y + (int)(yt1 + 0.5);

            return pout;
        }

        /// <summary>
        /// 求两点长度
        /// 
        /// </summary>
        /// <param name="StartP"></param>
        /// <param name="EndP"></param>
        /// <returns></returns>
        double getLenForm2Point(System.Drawing.Point StartP, System.Drawing.Point EndP)
        {
            double len = Math.Sqrt((StartP.X - EndP.X) * (StartP.X - EndP.X) + (StartP.Y - EndP.Y) * (StartP.Y - EndP.Y));
            return len;
        }


        int judgeSide(System.Drawing.Point P1, System.Drawing.Point P2, System.Drawing.Point point)
        {
            return ((P2.Y - P1.Y) * point.X + (P1.X - P2.X) * point.Y + (P2.X * P1.Y - P1.X * P2.Y));
        }


        System.Drawing.Point mousePoint;

        void dispJumpLength1(int x3, int y3)
        {
            if (Measure == false) return;//不测量
            if (recTimeR0 > 0) return;
            mousePoint.X = x3;
            mousePoint.Y = y3;
            for (int i = 0; i < gfencePnts.Count; i++)
            {
                int angle2_x1 = judgeSide(gfencePnts[i][0], gfencePnts[i][2], mousePoint);
                int angle2_x2 = judgeSide(gfencePnts[i][1], gfencePnts[i][3], mousePoint);
                //在左边界右,在右边界左
                if (angle2_x1 > 0 && angle2_x2 < 0)
                {
                    double pixLength1 = (gfencePntsDisValue[i][1] - gfencePntsDisValue[i][0]) * 10;
                    double baseLen = Convert.ToDouble(gfencePntsDisValue[i][0]);
                    drawMousePointLine1(mousePoint, gfencePnts[i][0], gfencePnts[i][1], gfencePnts[i][2], gfencePnts[i][3],
                        ref markerTopJump, ref markerBottomJump, baseLen, pixLength1, ref MeasureLenX);
                    break;
                }
            }
            try
            {
                VerticalDistance(gfencePnts, mousePoint, ref markerTopJumpY, ref markerBottomJumpY, ref MeasureLenY);

            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }

            try
            {
                MeasureLen = MeasureLenX;
                //是否铅球
                if (isShotPut)
                {
                    double distancey = 0;
                    double distancex = MeasureLenX;
                    int personY = cenMarkPoint.Y;
                    if (mousePoint.Y <= personY)
                    {
                        //distancey = (str2double("0.4") / 2) * 1000 - MeasureLenY;
                        distancey = 2000 - MeasureLenY;
                    }
                    else
                    {
                        //distancey = MeasureLenY - (str2double("0.4") / 2) * 1000;
                        distancey = MeasureLenY - 2000;
                    }
                    MeasureLen = Math.Sqrt((distancey * distancey) + (distancex * distancex));
                }

            }
            catch (Exception ex)
            {
                LoggerHelper.Debug(ex);
            }

            return;
        }

        /// <summary>
        /// 垂直距离
        /// </summary>
        /// <param name="gPnts"></param>
        /// <param name="mousePoint"></param>
        /// <param name="outTopP"></param>
        /// <param name="outBottomP"></param>
        /// <param name="outLen"></param>
        void VerticalDistance(List<System.Drawing.Point[]> gPnts, System.Drawing.Point mousePoint,
            ref System.Drawing.Point outTopP, ref System.Drawing.Point outBottomP, ref double outLen)
        {
            int nLen = gPnts.Count;
            System.Drawing.Point topStartP = gPnts[nLen - 1][1];
            System.Drawing.Point topEndP = gPnts[nLen - 1][3];
            System.Drawing.Point bottomStartP = gPnts[0][0];
            System.Drawing.Point bottomEndP = gPnts[0][2];
            //两点距离
            double fullLenPix = (gfencePntsDisValue[0][1] - gfencePntsDisValue[0][0]) * 10;
            //最小距离
            double baseLen = 0;
            //计算上标点和下标点到鼠标点的角度,180度成直线,
            //上下长度不同应该走不同的step长度
            //超过区域边界长度就break
            //或者上下两点的水平x值都超过鼠标点的xbreak
            double lenq = 0;
            double nowLen = 0;
            double div1 = 0.0f;

            System.Drawing.Point AngleMaxtopP = new System.Drawing.Point();
            System.Drawing.Point AngleMaxbotP = new System.Drawing.Point();
            //顶部长度
            double topLength1a = getLenForm2Point(topStartP, topEndP);
            //底部长度
            double bottomLength1a = getLenForm2Point(bottomStartP, bottomEndP);
            //实际长度和像素长度比值
            fullLenPix /= bottomLength1a;
            double AngleMin = 0;//最小角度
            double AngleMax = 180;
            double divStep = 0.00001f;//粗算
            double angleP12MTemp = 0;

            while (!((AngleMaxtopP.Y >= mousePoint.Y) && (AngleMaxbotP.Y >= mousePoint.Y)) && div1 <= 1)
            {

                try
                {
                    double angleP12M = 0;
                    int angleDecrementCount = 0;
                    div1 = div1 + divStep;
                    //上下标定线不停增加0.1f，直到角度>=0度
                    double topLen = topLength1a * div1;
                    double bottomLen = bottomLength1a * div1;
                    //求出顶部x，y值
                    System.Drawing.Point topP = len2yx(topLen, topStartP, topEndP);
                    //求出底部xy
                    System.Drawing.Point botP = len2yx(bottomLen, bottomStartP, bottomEndP);

                    angleP12M = Angle(mousePoint, topP, botP);

                    if (angleP12M < 120)
                    {
                        divStep = 0.05f;//精算
                    }
                    else if (angleP12M < 160)
                    {
                        divStep = 0.005f;//精算
                    }
                    else if (angleP12M < 170)
                    {
                        divStep = 0.001f;//精算
                    }
                    else if (angleP12M < 180)
                    {
                        divStep = 0.0001f;//精算
                    }
                    if (angleP12MTemp > angleP12M)
                    {
                        angleDecrementCount++;
                        if (angleDecrementCount > 10)
                        {
                            break;
                        }
                    }
                    else
                    {
                        angleDecrementCount = 0;
                    }
                    angleP12MTemp = angleP12M;
                    if (angleP12M <= AngleMax)
                    {
                        if (angleP12M > AngleMin)
                        {
                            AngleMin = angleP12M;
                            AngleMaxtopP = topP;
                            AngleMaxbotP = botP;
                        }
                    }
                    if (179 <= AngleMin && AngleMin <= 180)
                    {
                        outTopP = AngleMaxtopP;
                        outBottomP = AngleMaxbotP;
                        //计算距离
                        lenq = getLenForm2Point(bottomStartP, outBottomP);
                        nowLen = lenq * fullLenPix;
                        nowLen += (baseLen * 10);

                        outLen = nowLen;//测量长度
                                        //pictureBox1.Refresh();
                        return;
                    }
                }
                catch (Exception ex)
                {
                    LoggerHelper.Debug(ex);
                }

            }

            falseR0++;
            outTopP = AngleMaxtopP;
            outBottomP = AngleMaxbotP;
            //计算距离
            lenq = getLenForm2Point(bottomStartP, outBottomP);
            nowLen = lenq * fullLenPix;
            nowLen += (baseLen * 10);
            outLen = nowLen;//测量长度


        }

        /// <summary>
        /// 测距新方法
        /// </summary>
        /// <param name="mousePoint"></param>
        /// <param name="topStartP"></param>
        /// <param name="topEndP"></param>
        /// <param name="bottomStartP"></param>
        /// <param name="bottomEndP"></param>
        /// <param name="outTopP"></param>
        /// <param name="outBottomP"></param>
        /// <param name="baseLen"></param>
        /// <param name="fullLenPix"></param>
        /// <param name="outLen"></param>
        void drawMousePointLine1(System.Drawing.Point mousePoint, System.Drawing.Point topStartP, System.Drawing.Point topEndP, System.Drawing.Point bottomStartP, System.Drawing.Point bottomEndP,
        ref System.Drawing.Point outTopP, ref System.Drawing.Point outBottomP, double baseLen, double fullLenPix, ref double outLen)
        {
            //计算上标点和下标点到鼠标点的角度,180度成直线,
            //上下长度不同应该走不同的step长度
            //超过区域边界长度就break
            //或者上下两点的水平x值都超过鼠标点的xbreak
            double lenq = 0;
            double nowLen = 0;
            double div1 = 0.0f;

            System.Drawing.Point AngleMaxtopP = new System.Drawing.Point();
            System.Drawing.Point AngleMaxbotP = new System.Drawing.Point();
            //顶部长度
            double topLength1a = getLenForm2Point(topStartP, topEndP);
            //底部长度
            double bottomLength1a = getLenForm2Point(bottomStartP, bottomEndP);
            //实际长度和像素长度比值
            fullLenPix /= bottomLength1a;
            double AngleMin = 0;//最小角度
            double AngleMax = 180;
            double divStep = 0.00001f;//粗算
            double angleP12MTemp = 0;
            while (!((AngleMaxtopP.X >= mousePoint.X) && (AngleMaxbotP.X >= mousePoint.X)) && div1 <= 1)
            {

                try
                {
                    double angleP12M = 0;
                    int angleDecrementCount = 0;
                    div1 = div1 + divStep;
                    //上下标定线不停增加0.1f，直到角度>=0度
                    double topLen = topLength1a * div1;
                    double bottomLen = bottomLength1a * div1;
                    //求出顶部x，y值
                    System.Drawing.Point topP = len2xy(topLen, topStartP, topEndP);
                    //求出底部xy
                    System.Drawing.Point botP = len2xy(bottomLen, bottomStartP, bottomEndP);

                    angleP12M = Angle(mousePoint, topP, botP);

                    if (angleP12M < 120)
                    {
                        divStep = 0.05f;//精算
                    }
                    else if (angleP12M < 160)
                    {
                        divStep = 0.005f;//精算
                    }
                    else if (angleP12M < 170)
                    {
                        divStep = 0.001f;//精算
                    }
                    else if (angleP12M < 180)
                    {
                        divStep = 0.0001f;//精算
                    }
                    if (angleP12MTemp > angleP12M)
                    {
                        angleDecrementCount++;
                        if (angleDecrementCount > 10)
                        {
                            break;
                        }
                    }
                    else
                    {
                        angleDecrementCount = 0;
                    }
                    angleP12MTemp = angleP12M;
                    if (angleP12M <= AngleMax)
                    {
                        if (angleP12M > AngleMin)
                        {
                            AngleMin = angleP12M;
                            AngleMaxtopP = topP;
                            AngleMaxbotP = botP;
                        }
                    }
                    if (179 <= AngleMin && AngleMin <= 180)
                    {
                        outTopP = AngleMaxtopP;
                        outBottomP = AngleMaxbotP;
                        //计算距离
                        lenq = getLenForm2Point(bottomStartP, outBottomP);
                        nowLen = lenq * fullLenPix;
                        nowLen += (baseLen * 10);
                        outLen = nowLen;//测量长度

                        return;
                    }
                }
                catch (Exception ex)
                {
                    LoggerHelper.Debug(ex);

                }

            }

            falseR0++;
            outTopP = AngleMaxtopP;
            outBottomP = AngleMaxbotP;
            //计算距离
            lenq = getLenForm2Point(bottomStartP, outBottomP);
            nowLen = lenq * fullLenPix;
            nowLen += (baseLen * 10);
            outLen = nowLen;//测量长度



        }


        #endregion

        
        #region 成绩上传
        /// <summary>
        /// 本次成绩上传
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            ParameterizedThreadStart method = new ParameterizedThreadStart(uploadScoreForNowGroup);
            Thread threadRead = new Thread(method);
            threadRead.IsBackground = true;
            threadRead.Start();
        }
        void uploadScoreForNowGroup(object obj)
        {
            ControlHelper.ThreadInvokerControl(this, () =>
            {
                button2.Text = "上传中";
                button2.BackColor = Color.Red;

            });
            string[] fusp = new string[2];
            fusp[0] = _ProjectName;
            fusp[1] = _GroupName;
            string outmessage = uploadStudentThreadFun(fusp, sQLiteHelper, RoundCount0);
            //string outmessage=uploadStudentThreadFun1(fusp, sQLiteHelper);
            if (string.IsNullOrEmpty(outmessage))
            {
                FrmTips.ShowTipsInfo(this, "上传结束");
            }
            else
            {
                MessageBox.Show(outmessage);
            }

            ControlHelper.ThreadInvokerControl(this, () =>
            {
                button2.Text = "成绩上传";
                button2.BackColor = System.Drawing.SystemColors.Control;
            });
        }

        /// <summary>
        /// 上传学生的多线程方法 多人
        /// 先不上传视频
        /// </summary>
        /// <param name="obj"></param>
        public string uploadStudentThreadFun(Object obj, SQLiteHelper sQLiteHelper, int nowRound)
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
                string sql0 = $"SELECT Id,ProjectId,Name FROM DbGroupInfos WHERE ProjectId='{SportProjectDic["Id"]}'";
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
                    urrp.MachineCode = MachineCode;
                    if (list.Count == 0)
                    {
                        continue;
                    }

                    StringBuilder resultSb = new StringBuilder();
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
                            $"SELECT Id,CreateTime,RoundId,State,uploadState,Result FROM ResultInfos WHERE PersonId='{stu["Id"]}' And IsRemoved=0 And RoundId={nowRound} LIMIT 1");
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
                            RoundsItem rdi = new RoundsItem();
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
                    string url = localInfos["Platform"] + RequestUrl.UploadResults;

                    var httpUpload = new  HttpUpload();
                    var formDatas = new List<FromDataItemModel>();
                    //添加其他字段
                    formDatas.Add(new FromDataItemModel()
                    {
                        key= "data",
                        value = JsonStr
                    });

                    logWirte.AppendLine();
                    logWirte.AppendLine();
                    logWirte.AppendLine(JsonStr);

                    //上传学生成绩
                    string result =  HttpUpload.PostFromData(url, formDatas);
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
                    //更新成绩上传状态
                    string sql1 = $"UPDATE ResultInfos SET uploadState=1 WHERE PersonId={item["Id"]} and RoundId={nowRound}";
                    sQLiteHelper.ExecuteNonQuery(sql1);
                    sb.AppendLine($"考号:{item["IdNumber"]} 姓名:{item["Name"]}");
                }
                sQLiteHelper.CommitTransAction(sQLiteTransaction);
                sb.AppendLine("*******************success********************");

                try
                {
                    string txtpath = Application.StartupPath + $"\\Log\\upload\\";
                    if (!Directory.Exists(txtpath))
                    {
                        Directory.CreateDirectory(txtpath);
                    }
                    if (successList.Count != 0 || errorList.Count != 0)
                    {
                        txtpath = Path.Combine(txtpath, $"upload_{DateTime.Now.ToString("yyyyMMddHHmmss")}.txt");
                        System.IO.File.WriteAllText(txtpath, sb.ToString());
                    }
                }
                catch (Exception ex)
                {

                    LoggerHelper.Debug(ex);
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



        #endregion



        #region 图像显示
        OpenCvSharp.VideoWriter VideoOutPut;
        private object bmpObj = new object();
        private Bitmap bmpSoure = new Bitmap(1, 1);
        private int _width;
        private int _height;
        class TargetPoint
        {
            public string name;
            public int x;
            public int y;
            public int dis;
            public bool status;
        }
        string jpName = "";
        string jpDis = "";
        bool jpStatus = true;//true是上定点,false是下定点
        bool isMirrorMode = false;
        //记录定标数据
        List<TargetPoint> targetPoints = new List<TargetPoint>();
        PointForm1 pf1 = new PointForm1();//? 画点界面
        List<TargetPoint> TopListSort = new List<TargetPoint>();//? 顶部坐标列表
        List<TargetPoint> BottomListSort = new List<TargetPoint>();//? 底部坐标列表
        System.Drawing.Point cenMarkPoint;//? 铅球中心点
        System.Drawing.Point markerTopJumpY, markerBottomJumpY, markerTopJump, markerBottomJump, markerTmp, mouseMovePoint;
        List<System.Drawing.Point[]> gfencePnts = new List<System.Drawing.Point[]>();
        List<int[]> gfencePntsDisValue = new List<int[]>();
        int GCCounta = 0;
        int videoSourceRuningR0 = 0;
        int frameRecSum = 0;//计算帧数
        /// <summary>
        /// 定标参数
        /// </summary>
        public int colum = 0;
        public int initDis = 0;
        public int distance = 0;
        public int nowColum = 0;
        int formShowStatus = 0;

        public struct ImageAndIndex
        {
            //图片顺序索引
            public int index;
            public Bitmap image;
        }
        private object ImageQueuesLock = new object();
        private ConcurrentQueue<ImageAndIndex> ImageQueues = new ConcurrentQueue<ImageAndIndex>();
        Bitmap backBp = null;
        Bitmap resultBp = null;
        //? 是否实心球模式
        bool isBallCheckBoxb = false;
        //? 是否展示选择
        bool isShowImgList = true;
        //? 是不是铅球
        bool isShotPut = false;
        //? 是不是坐位体前屈
        bool isSitting = false;

        public class imgMsS
        {
            public int imgIndex = -1;
            public Bitmap img;
            public DateTime dt;
            public bool isDown;
            public string name;
            public bool[][] isHand;
        }
        public List<imgMsS> imgMs = new List<imgMsS>();
        string ReservedDigitsTxt = "0.000";
        /// <summary>
        /// 视频输入设备信息
        /// </summary>
        private FilterInfoCollection filterInfoCollection;
        /// <summary>
        /// RGB摄像头设备
        /// </summary>
        private VideoCaptureDevice rgbDeviceVideo;
        FuseImage fuseImg = null;
        int skipFrameDispR0 = 0;
        int cameraSkip = 0;
        OpenCvSharp.Point[] conPoints0 = new OpenCvSharp.Point[4];
        /// <summary>
        /// 更新标点显示
        /// </summary>
        void updateTargetListView(bool flag = false)
        {
            //gfencePnts
            gfencePnts.Clear();
            gfencePntsDisValue.Clear();
            List<TargetPoint> TopList = targetPoints.FindAll(a => a.status);
            List<TargetPoint> BottomList = targetPoints.FindAll(a => !a.status);
            TopListSort.Clear();
            BottomListSort.Clear();
            TopListSort = TopList.OrderBy(a => a.x).ToList();
            BottomListSort = BottomList.OrderBy(a => a.x).ToList();

            int min = TopListSort.Count > BottomListSort.Count ? BottomListSort.Count : TopListSort.Count;
            int max = TopListSort.Count > BottomListSort.Count ? TopListSort.Count : BottomListSort.Count;
            for (int i = 0; i < min; i++)
            {
                if (i <= min - 2)
                {
                    System.Drawing.Point[] fpnts = new System.Drawing.Point[4];
                    int[] DisValue = new int[2];
                    System.Drawing.Point p = new System.Drawing.Point(TopListSort[i].x, TopListSort[i].y);
                    System.Drawing.Point p1 = new System.Drawing.Point(TopListSort[i + 1].x, TopListSort[i + 1].y);
                    System.Drawing.Point p2 = new System.Drawing.Point(BottomListSort[i].x, BottomListSort[i].y);
                    System.Drawing.Point p3 = new System.Drawing.Point(BottomListSort[i + 1].x, BottomListSort[i + 1].y);
                    int maxv = 0;
                    int minv = 0;
                    minv = Math.Min(BottomListSort[i].dis, BottomListSort[i + 1].dis);
                    maxv = Math.Max(BottomListSort[i].dis, BottomListSort[i + 1].dis);
                    DisValue[0] = minv;
                    DisValue[1] = maxv;
                    fpnts[0] = p;
                    fpnts[1] = p1;
                    fpnts[2] = p2;
                    fpnts[3] = p3;
                    gfencePnts.Add(fpnts);
                    gfencePntsDisValue.Add(DisValue);
                }
            }
            if (gfencePnts.Count > 0 && gfencePnts[0].Length > 2)
            {
                System.Drawing.Point bottomStartP = gfencePnts[0][0];
                System.Drawing.Point bottomEndP = gfencePnts[0][2];
                int x = (bottomStartP.X + bottomEndP.X) / 2;
                int y = (bottomStartP.Y + bottomEndP.Y) / 2;
                cenMarkPoint = new System.Drawing.Point(x, y);
                Console.WriteLine();
            }
            if (flag)
            {
                //gfencePnts
                int igfInde = gfencePnts.Count - 1;
                conPoints0[0] = new OpenCvSharp.Point(gfencePnts[0][0].X, gfencePnts[0][0].Y);
                conPoints0[1] = new OpenCvSharp.Point(gfencePnts[0][2].X, gfencePnts[0][2].Y);
                conPoints0[2] = new OpenCvSharp.Point(gfencePnts[igfInde][1].X, gfencePnts[igfInde][1].Y);
                conPoints0[3] = new OpenCvSharp.Point(gfencePnts[igfInde][3].X, gfencePnts[igfInde][3].Y);
                //OpenCvSharp.Rect rect = Cv2.BoundingRect(contours[i]);
                OpenCvSharp.Rect rect0 = Cv2.BoundingRect(conPoints0);
                conPoints0[0] = rect0.TopLeft;
                conPoints0[1] = new OpenCvSharp.Point(rect0.X + rect0.Width, rect0.Y);
                conPoints0[2] = new OpenCvSharp.Point(rect0.X, rect0.Y + rect0.Height);
                conPoints0[3] = new OpenCvSharp.Point(rect0.X + rect0.Width, rect0.Y + rect0.Height);
            }

        }
        /// <summary>
        /// 
        /// </summary>
        private void CameraInit()
        {
            _width = 1280;
            _height = 720;
            openCameraSetting();

            //ReloadCameraList();
        }

        /// <summary>
        /// 载入摄像头
        /// </summary>
        /// <param name="name"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public bool LoadCamera(string name)
        {
            bool flag = false;

            try
            {
                if (rgbVideoSource.IsRunning)
                {
                    rgbVideoSource.SignalToStop();
                    //rgbVideoSource.Hide();
                }
                Boolean findIt = false;
                foreach (FilterInfo device in filterInfoCollection)
                {
                    if (device.Name == name)
                    {
                        rgbDeviceVideo = new VideoCaptureDevice(device.MonikerString);
                        if (rgbDeviceVideo.VideoCapabilities.Length == 1)
                        {
                            rgbDeviceVideo.VideoResolution = rgbDeviceVideo.VideoCapabilities[0];
                            findIt = true;
                        }
                        else
                        {
                            for (int i = 0; i < rgbDeviceVideo.VideoCapabilities.Length; i++)
                            {
                                if (rgbDeviceVideo.VideoCapabilities[i].FrameSize.Width == _width
                                    && rgbDeviceVideo.VideoCapabilities[i].FrameSize.Height == _height
                                    && rgbDeviceVideo.VideoCapabilities[i].AverageFrameRate == maxFps)
                                {
                                    rgbDeviceVideo.VideoResolution = rgbDeviceVideo.VideoCapabilities[i];
                                    findIt = true;
                                    break;
                                }
                            }
                        }
                        break;
                    }
                }
                if (findIt)
                {
                    rgbVideoSource.VideoSource = rgbDeviceVideo;
                    rgbVideoSource.Start();
                    //rgbVideoSource.Show();
                    rgbVideoSource.SendToBack();
                    flag = true;
                }
            }
            catch (Exception ex)
            {
                flag = false;
                LoggerHelper.Debug(ex);
            }
            return flag;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool rgbVideoSourceStart()
        {
            bool flag = false;
            if (rgbVideoSource.IsRunning) return true;
            if (rgbVideoSource != null && !rgbVideoSource.IsRunning)
            {
                rgbVideoSource.Start();
                //rgbVideoSource.Show();
                return true;
            }
            if (string.IsNullOrEmpty(cameraName))
            {
                FrmTips.ShowTipsError(this, "请选择摄像头!");
                return false;
            }
            flag = openCamearaFun(cameraName);
            return flag;
        }
        public void rgbVideoSourceStop()
        {
            CloseCamera();
        }

        /// <summary>
        /// 打开写入本地流
        /// </summary>
        /// <param name="outPath"></param>
        public void OpenVideoOutPut(string outPath)
        {
            try
            {
                if (VideoOutPut != null && VideoOutPut.IsOpened())
                {
                    VideoOutPut.Release();
                }
                VideoOutPut = new OpenCvSharp.VideoWriter(outPath, OpenCvSharp.FourCC.XVID, 60, new OpenCvSharp.Size(_width, _height));
            }
            catch (Exception ex)
            {
                LoggerHelper.Debug(ex);
                Console.WriteLine(ex.Message);
            }

        }
        /// <summary>
        /// 释放写入本地流
        /// </summary>
        public void ReleaseVideoOutPut()
        {
            try
            {
                if (VideoOutPut != null && VideoOutPut.IsOpened())
                {
                    VideoOutPut.Release();
                }
            }
            catch (Exception ex)
            {
                LoggerHelper.Debug(ex);
            }
        }
        public void CloseCamera()
        {
            try
            {
                if (!rgbVideoSource.IsRunning) return;
                if (rgbVideoSource != null && rgbVideoSource.IsRunning)
                {
                    rgbVideoSource.SignalToStop();
                    //rgbVideoSource.Hide();
                }
                openCameraBtn.Text = "打开摄像头";
                openCameraBtn.BackColor = Color.White;
            }
            catch (Exception ex)
            {
                LoggerHelper.Debug(ex);
            }
        }

        bool openCamearaFun(string cameraName)
        {
            bool flag = LoadCamera(cameraName);
            if (!flag)
            {
                openCameraBtn.Text = "打开摄像头";
                openCameraBtn.BackColor = Color.White;
                FrmTips.ShowTipsError(this, "打开摄像头失败!");
            }
            else
            {
                openCameraBtn.Text = "关闭摄像头";
                openCameraBtn.BackColor = Color.Red;
            }
            return flag;
        }
        private void rgbVideoSource_SizeChanged(object sender, EventArgs e)
        {
            pBox1Width = rgbVideoSource.Width;
            pBox1Height = rgbVideoSource.Height;
        }

        private void rgbVideoSource_MouseDown(object sender, MouseEventArgs e)
        {

        }
        void saveMarkSetting()
        {
            StringBuilder builder = new StringBuilder();
            foreach (var t in targetPoints)
            {
                builder.Append(t.name + ";");
                builder.Append(t.x + ",");
                builder.Append(t.y + ";");
                builder.Append(t.dis + ";");
                if (t.status)
                {
                    builder.Append("1\r\n");
                }
                else
                {
                    builder.Append("0\r\n");
                }
            }
            TxtFile.Instance.Write(strMainModule + markPointFile, builder.ToString());
        }

        private void rgbVideoSource_MouseMove(object sender, MouseEventArgs e)
        {

        }
        /// <summary>
        /// 刷新视频显示
        /// </summary>
        void rgbVideoSourceRePaint()
        {
            pictureBox1.Refresh();
            if (!rgbVideoSource.IsRunning)
                rgbVideoSource.Refresh();
        }

        bool rgbVideoSourcePaintFlag = false;
        private void rgbVideoSource_Paint(object sender, PaintEventArgs e)
        {
            skipFrameDispR0++;
            if (rgbVideoSourcePaintFlag) return;
            rgbVideoSourcePaintFlag = true;
            //float offsetX = pBox1Width * 1f / bmp.Width;
            //float offsetY = pBox1Height * 1f / bmp.Height; 
            try
            {
                if (skipFrameDispR0 < cameraSkip)
                {
                    return;
                }
                skipFrameDispR0 = 0;
                if (rgbVideoSource.IsRunning)
                {
                    //得到当前RGB摄像头下的图片
                    Bitmap bmp = rgbVideoSource.GetCurrentVideoFrame();
                    if (bmp == null)
                    {
                        return;
                    }
                    //处理镜像
                    if (isMirrorMode)
                    {
                        bmp.RotateFlip(RotateFlipType.RotateNoneFlipX);
                    }
                    //是否写入
                    if (VideoOutPut != null && VideoOutPut.IsOpened())
                    {
                        Bitmap bitmap = ImageHelper.DeepCopyBitmap(bmp);
                        OpenCvSharp.Mat mat = ImageHelper.Bitmap2Mat(bitmap);
                        VideoOutPut.Write(mat);
                    }
                    ///处理录像数据
                    backBp = ImageHelper.DeepCopyBitmap(bmp);
                    FuseBitmap fb = null;
                    Bitmap dstBitmap = null;
                    if (isSitting)
                    {
                        fb = new FuseBitmap(bmp);
                        fb.SetRect(conPoints0);
                        fb.FuseColorImg(isShowHandFlag);
                        fb.Dispose();
                        dstBitmap = fb.dstBitmap;
                    }
                    if (recTimeR0 > 0)
                    {
                        if (isBallCheckBoxb)
                        {
                            Bitmap bp = ImageHelper.DeepCopyBitmap(bmp);
                            ImageAndIndex iai = new ImageAndIndex();
                            iai.index = frameSum;
                            iai.image = bp;
                            lock (ImageQueuesLock)
                            {
                                ImageQueues.Enqueue(iai);
                            }
                        }
                        imgMsS mss = new imgMsS();
                        mss.dt = DateTime.Now;
                        mss.name = "img" + imgMs.Count;
                        //坐位体前屈
                        if (isSitting && fb != null && dstBitmap != null)
                        {
                            mss.img = dstBitmap;
                            mss.isHand = fb.isHand;
                        }
                        else
                        {
                            Bitmap bitmap = ImageHelper.DeepCopyBitmap(bmp);
                            mss.img = bitmap;
                        }
                        imgMs.Add(mss);
                        frameSum++;
                    }
                    if (isSitting && fb != null && dstBitmap != null)
                    {
                        bmp = ImageHelper.DeepCopyBitmap(dstBitmap);
                    }
                    frameRecSum++;//计算帧速用

                    //显示图片
                    pictureBox1.Image = bmp;
                }
            }
            catch (Exception ex)
            {
                LoggerHelper.Debug(ex);
            }
            finally
            {
                GCCounta++;
                if (GCCounta > 10)
                {
                    GCCounta = 0;
                    GC.Collect();
                }
                rgbVideoSourcePaintFlag = false;
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            markerTmp.X = e.X;
            markerTmp.Y = e.Y;
            mouseMovePoint.X = e.X;
            mouseMovePoint.Y = e.Y;
            if (formShowStatus > 0)
            {
                bool flag = false;
                //添加定标
                targetPoints.Add(new TargetPoint()
                {
                    x = markerTmp.X,
                    y = markerTmp.Y,
                    name = jpName,
                    dis = str2int(jpDis),//cm
                    status = jpStatus
                });
                if (targetPoints.Count == colum * 2)
                {
                    formShowStatus = 0;
                    pf1.Hide();
                    saveMarkSetting();
                    GC.Collect();
                    flag = true;
                }
                else
                {
                    pf1.UpdateFlp(++nowColum);
                    jpName = nowColum + "";
                    if (nowColum % 2 != 0)
                    {
                        initDis += distance;
                    }
                    jpDis = initDis + "";
                    jpStatus = !jpStatus;
                    System.Drawing.Point ptc = this.PointToScreen(new System.Drawing.Point(e.X, e.Y - 100));
                    pf1.Location = ptc;
                }
                updateTargetListView(flag);
            }
            dispJumpLength1(e.X, e.Y);
            rgbVideoSourceRePaint();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            mouseMovePoint.X = e.X;
            mouseMovePoint.Y = e.Y;
            if (formShowStatus > 0)
            {
                Task.Run(() =>
                {
                    System.Drawing.Point ptc = this.PointToScreen(new System.Drawing.Point(e.X, e.Y - 100));
                    pf1.Location = ptc;
                });
            }
            else
            {
                dispJumpLength1(e.X, e.Y);
            }
            rgbVideoSourceRePaint();
        }
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            #region 画图

            Pen pen = new Pen(Color.MediumSpringGreen, 1);
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            Font drawFont = new Font("Arial", 14);
            SolidBrush drawBrush = new SolidBrush(Color.MediumSpringGreen);
            List<TargetPoint> TopList = targetPoints.FindAll(a => a.status);
            List<TargetPoint> BottomList = targetPoints.FindAll(a => !a.status);
            List<TargetPoint> TopListSort = TopList.OrderBy(a => a.x).ToList();
            List<TargetPoint> BottomListSort = BottomList.OrderBy(a => a.x).ToList();
            //框点十字
            int left = 0;
            foreach (var mark in TopListSort)
            {
                System.Drawing.Point p = new System.Drawing.Point(mark.x, mark.y);
                if (left <= TopListSort.Count - 2)
                {
                    System.Drawing.Point p1 = new System.Drawing.Point(TopListSort[left + 1].x, TopListSort[left + 1].y);
                    e.Graphics.DrawLine(pen, p, p1);
                    left++;
                }
                drawPointCross(e.Graphics, p, pen);
                drawPointText(e.Graphics, $"({mark.name}){mark.dis}cm", p, drawFont, drawBrush, 0, 20, 0, 30);
            }
            left = 0;
            foreach (var mark in BottomListSort)
            {
                System.Drawing.Point p = new System.Drawing.Point(mark.x, mark.y);

                if (left <= BottomListSort.Count - 2)
                {
                    System.Drawing.Point p1 = new System.Drawing.Point(BottomListSort[left + 1].x, BottomListSort[left + 1].y);
                    e.Graphics.DrawLine(pen, p, p1);
                    left++;
                }
                drawPointCross(e.Graphics, p, pen);
                drawPointText(e.Graphics, $"({mark.name}){mark.dis}cm", p, drawFont, drawBrush, 0, 20, 1, 10);
            }
            //中间框点连线
            int min = TopListSort.Count > BottomListSort.Count ? BottomListSort.Count : TopListSort.Count;
            for (int i = 0; i < min; i++)
            {
                if (i != 0 && i != min - 1)
                {
                    continue;
                }
                TargetPoint mark = TopListSort[i];
                TargetPoint mark1 = BottomListSort[i];
                System.Drawing.Point p = new System.Drawing.Point(mark.x, mark.y);
                System.Drawing.Point p1 = new System.Drawing.Point(mark1.x, mark1.y);
                e.Graphics.DrawLine(pen, p, p1);
            }
            //鼠标画十字
            Pen pen1 = new Pen(Color.Red, 1);
            drawPointCross(e.Graphics, mouseMovePoint, pen1);
            double fenmu = 1000;
            if (isSitting)
            {
                fenmu = 10;
            }
            string LenX = (MeasureLenX / fenmu).ToString(ReservedDigitsTxt);
            string LenY = (MeasureLenY / fenmu).ToString(ReservedDigitsTxt);
            string Len = (MeasureLen / fenmu).ToString(ReservedDigitsTxt);

            pen.Color = Color.Red;
            e.Graphics.DrawLine(pen, markerTopJump, markerBottomJump);
            drawPointCross(e.Graphics, mousePoint, pen1);
            drawFont = new Font("微软雅黑", 28, FontStyle.Bold);
            drawBrush = new SolidBrush(Color.Red);// Create point for upper-left corner of drawing.
            drawPointText(e.Graphics, $"成绩:{Len}{dangwei}", new System.Drawing.Point(10, 10), drawFont, drawBrush, 1, 10, 1, 0);
            e.Graphics.FillRectangle(new SolidBrush(Color.White), new Rectangle(new System.Drawing.Point(350, 0), new System.Drawing.Size(1000, 50)));
            //时间 
            e.Graphics.DrawString($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}", new Font("微软雅黑", 14), drawBrush, 350, 15);
            //考生姓名和学号
            if (isHaveStudent(false))
            {
                e.Graphics.DrawString($"组号:{nowRaceStudentData.groupName} 考号:{nowRaceStudentData.idNumber} 姓名:{nowRaceStudentData.name}", new Font("微软雅黑", 14), drawBrush, 550, 15);
            }

            e.Graphics.DrawString($"X:{LenX},Y:{LenY}", new Font("微软雅黑", 14, FontStyle.Bold), new SolidBrush(Color.Blue), 1100, 15);
            if (isShotPut)
            {
                pen.DashPattern = new float[] { 5f, 10f };
                e.Graphics.DrawLine(pen, cenMarkPoint, mousePoint);
            }
            #region 方框
            /*bool m_flag = true;
            Array.ForEach(conPoints0, (a) =>
            {
               if (a == null) m_flag = false;
            });
            if (m_flag)
            {
               int minWidth = conPoints0[0].X;
               int maxWidth = conPoints0[3].X;
               int minHeigh = conPoints0[0].Y;
               int maxHeigh = conPoints0[3].Y;
               if (minWidth != 0 &&
                  minWidth != 0 &&
                  minWidth != 0 &&
                  minWidth != 0)
               {
                  pen = new Pen(Color.MediumSpringGreen, 3);
                  e.Graphics.DrawRectangle(pen, minWidth, minHeigh, maxWidth- minWidth, maxHeigh- minHeigh);
               }
            }*/

            #endregion
            #endregion

        }
        int sleepCount = 0;
        int RecordEnd = 0;
        //录像结束剩余图片总数
        int remainderImgSum = 0;
        //录像结束 未处理图片数
        int remainderImgCount = 0;
        /// <summary>
        /// 处理实心球合成图
        /// </summary>
        /// <param name="obj"></param>
        void ImagePredictLabelQueues2ThreadFun(object obj)
        {
            while (true)
            {
                if (ImageQueues.Count == 0)
                {
                    Thread.Sleep(10);
                    sleepCount++;
                    if (sleepCount > 10)
                    {
                        Thread.Sleep(100);
                    }
                    if (RecordEnd == 1)
                    {
                        //剩余图片处理结束
                        RecordEnd = 2;
                        fuseImg.Dispose();
                        Bitmap dstcopy0 = FuseImage.DeepCopyBitmap(fuseImg.dstBitmap);
                        imgMsS mss = new imgMsS();
                        mss.img = dstcopy0;
                        mss.dt = DateTime.Now;
                        mss.name = "img" + imgMs.Count;
                        imgMs.Add(mss);
                        setHScrollBarValue(imgMs.Count - 1);
                    }
                }
                else
                {
                    sleepCount = 0;
                    //处理剩余图片
                    if (RecordEnd == 1)
                    {
                        remainderImgSum = ImageQueues.Count;
                        remainderImgCount = 0;
                        bool flag = false;
                        ImageAndIndex iplr = new ImageAndIndex();
                        lock (ImageQueuesLock)
                        {
                            flag = ImageQueues.TryDequeue(out iplr);
                        }
                        while (flag)
                        {
                            remainderImgCount++;
                            fuseImg.FuseColorImg1(iplr.image);
                            lock (ImageQueuesLock)
                            {
                                flag = ImageQueues.TryDequeue(out iplr);
                            }
                        }
                        //剩余图片处理结束
                        RecordEnd = 2;
                        fuseImg.Dispose();
                        Bitmap dstcopy0 = FuseImage.DeepCopyBitmap(fuseImg.dstBitmap);

                        imgMsS mss = new imgMsS();
                        mss.img = dstcopy0;
                        mss.dt = DateTime.Now;
                        mss.name = "img" + imgMs.Count;
                        imgMs.Add(mss);
                        setHScrollBarValue(imgMs.Count - 1);
                        startMeasure();
                    }
                    else
                    {
                        bool flag = false;
                        ImageAndIndex iplr = new ImageAndIndex();
                        lock (ImageQueuesLock)
                        {
                            flag = ImageQueues.TryDequeue(out iplr);
                        }
                        if (flag)
                        {
                            fuseImg.FuseColorImg1(iplr.image);
                        }
                    }

                }
            }
        }

        /// <summary>
        /// 打开界面选择录像图片
        /// </summary>
        void openImgList()
        {
            if (!isHaveStudent())
            {
                uiTabControl1.SelectedIndex = 0;
                return;
            }

            findImg fi = new findImg();
            fi.imgMs = imgMs;
            //fi.nowTestDir = nowTestDir;
            if (fi.ShowDialog() == DialogResult.OK)
            {
                nowFileName = fi.fileName;
                //string fileName = nowTestDir + "\\" + fi.fileName + ".jpg";
                int sp = str2int(nowFileName);
                if (sp < imgMs.Count - 1)
                {
                    setHScrollBarValue(sp);
                    pictureBox1.Image = imgMs[sp].img;

                }
            }
            else
            {
                int sp = imgMs.Count - 5;
                nowFileName = sp + "";
                if (sp < imgMs.Count)
                {
                    pictureBox1.Image = imgMs[sp].img;

                }
                else
                {
                    sp = imgMs.Count - 1;
                    nowFileName = sp + "";
                    pictureBox1.Image = imgMs[sp].img;

                }
            }
            recImgIndex.Text = $"图片索引:{nowFileName}";
            startMeasure();
        }

        /// <summary>
        /// 开始录像
        /// </summary>
        void BeginRec()
        {
            if (recTimeR0 > 0)//停止录像
            {
                recTimeR0 = 1;
                return;
            }
            bool flag = true;
            if (MeasureLen != 0 || recTimeR0 != 0)
            {
                flag = beginTest();
            }
            if (flag)
            {
                startRec();
            }

        }
        /// <summary>
        /// 开始计时
        /// </summary>
        void startRec()
        {
            if (!isHaveStudent())
            {

                return;
            }
            recTimeR0 = 0;
            RecordEnd = 0;
            fuseImg = new FuseImage(backBp);
            ControlHelper.ThreadInvokerControl(this, () =>
            {
                button12.Text = "录像中...";
                button12.BackColor = Color.Red;
                button13.Text = "测量中...";
                button13.BackColor = Color.Red;
            });

            stopMeasure();
            nowTestDir = $"\\{_ProjectName}\\{_GroupName}\\{nowRaceStudentData.idNumber}_{nowRaceStudentData.name}\\第{nowRaceStudentData.RoundId}轮\\";
            nowTestDir = ScoreDir + nowTestDir;
            if (!Directory.Exists(nowTestDir))
            {
                DirectoryInfo dir = new DirectoryInfo(nowTestDir);
                dir.Create();//自行判断一下是否存在。
            }
            string avipath = Path.Combine(nowTestDir,
                $"{nowRaceStudentData.idNumber}_{nowRaceStudentData.name}_第{nowRaceStudentData.RoundId}轮.mp4");
            if (System.IO.File.Exists(avipath))
            {
                System.IO.File.Delete(avipath);
            }
            OpenVideoOutPut(avipath);
            //recFileSp = 1;//录像文件顺序号
            frameSum = 0;
            recTimeR0 = str2int(recTimeTxt.Text) * 10;
            Task.Run(() => voiceOut0("考生开始考试"));
            SendScore(nowRaceStudentData.name, "开始考试", "");
        }

        #endregion



        #region 控制面板

        void voiceOut0(string str, int rate = 3)
        {
            Task.Run(() =>
            {
                SpVoice voice = new SpVoice();
                ISpeechObjectTokens obj = voice.GetVoices();
                voice.Voice = obj.Item(0);
                voice.Rate = rate;
                voice.Speak(str, SpeechVoiceSpeakFlags.SVSFIsXML | SpeechVoiceSpeakFlags.SVSFDefault);
            });
        }


        /// <summary>
        /// 写入成绩
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnWriteScore_Click(object sender, EventArgs e)
        {

            WriteScore2Db();
        }

        /// <summary>
        /// 写入成绩
        /// </summary>
        void WriteScore2Db()
        {
            if (recTimeR0 > 0)
            {
                FrmTips.ShowTipsError(this, "考试中请勿进行此操作");
                return;
            }
            //updateRaceStudentDataListsScore();
            if (!isHaveStudent())
            {
                FrmTips.ShowTipsError(this, "数据异常");
                return;
            }
            //写入成绩
            if (imgMs.Count > 0)
            {
                //弹窗显示成绩是否写入
                DetermineGrades dmg = new DetermineGrades();
                double fenmu = 1000;
                if (isSitting)
                {
                    fenmu = 10;
                }
                dmg.score = MeasureLen / fenmu;
                dmg.dangwei = dangwei;
                if (dmg.ShowDialog() == DialogResult.OK)
                {
                    ///成绩修改写入日志
                    if (dmg.checkScore != 0)
                    {
                        MeasureLen = dmg.checkScore * fenmu;
                        //测试项目
                        string projectTypeCbxtxt = ProjectNameCbx.Text;
                        string txt_Grouptxt = _GroupName;
                        string txt_GNametxt = nowRaceStudentData.name;
                        double score1 = dmg.score;


                        string scoreContent = string.Format("时间:{0,-20},项目:{1,-20},组别:{2,-10},准考证号:{3,-20},姓名{4,-5},第{5}次成绩:修改成绩{6,-5}为{7,-5}, 状态:{8,-5}",
                              DateTime.Now.ToString("yyyy年MM月dd日HH:mm:ss"),
                              projectTypeCbxtxt,
                              txt_Grouptxt,
                              nowRaceStudentData.idNumber,
                              txt_GNametxt,
                              RoundCount0,
                              score1.ToString(ReservedDigitsTxt),
                              dmg.checkScore,
                              "已测试");
                        System.IO.File.AppendAllText(@"./成绩日志.txt", scoreContent + "\n");
                    }
                    string score = (MeasureLen / fenmu).ToString(ReservedDigitsTxt);
                    Task.Run(() => voiceOut0($"成绩:{score}{dangwei}", 3));
                    bool sendScoreReturn = SendScore(nowRaceStudentData.name, score);
                    //写入成绩
                    input2Result();
                }
            }
            else
            {
                FrmTips.ShowTipsError(this, "未录像");
            }
        }

        #region 右键列表选择处理
        private void 缺考ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetErrorState("缺考");
        }
        private void 中退ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetErrorState("中退");
        }

        private void 犯规ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetErrorState("犯规");
        }

        private void 弃权ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetErrorState("弃权");
        }
        private void 成绩查询ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (stuView.SelectedRows.Count == 0)
                {
                    FrmTips.ShowTipsError(this, "未选择考生");
                    return;
                }
                string idNumber = stuView.SelectedRows[0].Cells[1].Value.ToString();
                string name = stuView.SelectedRows[0].Cells[2].Value.ToString();
                string nowTestDir1 = $"\\{_ProjectName}\\{_GroupName}\\{idNumber}_{name}\\第{RoundCount0}轮";
                nowTestDir1 = ScoreDir + nowTestDir1;
                if (Directory.Exists(nowTestDir1))
                {
                    System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo("Explorer.exe");
                    psi.Arguments = "/e,/select," + nowTestDir1;
                    System.Diagnostics.Process.Start(psi);
                }
                else
                {
                    FrmTips.ShowTipsError(this, "未找到文件夹");
                }
            }
            catch (Exception ex)
            {

                LoggerHelper.Debug(ex);
            }

        }

        /// <summary>
        /// 设置异常状态
        /// </summary>
        /// <param name="state"></param>
        void SetErrorState(string state)
        {
            if (stuView.SelectedRows.Count == 0)
            {
                FrmTips.ShowTipsError(this, "未选择考生");
                return;
            }
            int state0 = ResultState.ResultState2Int(state);
            string idNumber = stuView.SelectedRows[0].Cells[1].Value.ToString();
            string name = stuView.SelectedRows[0].Cells[2].Value.ToString();
            string id = stuView.SelectedRows[0].Cells[6].Value.ToString();
            string sql = $"UPDATE DbPersonInfos SET State=1,FinalScore=1 WHERE Id='{id}'";
            int result = sQLiteHelper.ExecuteNonQuery(sql);
            sql = $"UPDATE ResultInfos SET State={state0} WHERE PersonId='{id}' AND RoundId={RoundCount0} AND IsRemoved=0";
            result = sQLiteHelper.ExecuteNonQuery(sql);
            //更新没有该成绩 插入
            if (result == 0)
            {
                var sortid = sQLiteHelper.ExecuteScalar($"SELECT MAX(SortId) + 1 FROM ResultInfos");
                string sortid0 = "1";
                if (sortid != null && sortid.ToString() != "") sortid0 = sortid.ToString();

                sql = $"INSERT INTO ResultInfos(CreateTime,SortId,IsRemoved,PersonId,SportItemType,PersonName,PersonIdNumber,RoundId,Result,State) " +
                         $"VALUES('{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}', {sortid0}, 0, '{id}',0,'{name}','{idNumber}',{RoundCount0},{0},{state0})";
                //处理写入数据库
                sQLiteHelper.ExecuteNonQuery(sql);
            }
            if (RoundCount0 > 0)
            {
                UpdateListView(_ProjectId, _GroupName, RoundCount0);

            }
        }
        #endregion


        /// <summary>
        /// 当前是否有考生
        /// </summary>
        /// <returns></returns>
        bool isHaveStudent(bool flag = true)
        {
            if (nowRaceStudentData == null || string.IsNullOrEmpty(nowRaceStudentData.id))
            {
                if (flag)
                    FrmTips.ShowTipsError(this, "请选择考生");
                return false;
            }
            return true;
        }

        /// <summary>
        /// 定标设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button27_Click(object sender, EventArgs e)
        {
            if (recTimeR0 > 0)
            {
                FrmTips.ShowTipsError(this, "考试中请勿进行此操作");
                return;
            }
            SelectPunctuationRule sptr = new SelectPunctuationRule();
            if (sptr.ShowDialog() == DialogResult.OK)
            {
                targetPoints.Clear();
                colum = sptr.colum;
                initDis = sptr.initDis;
                distance = sptr.distance;
                pf1.col = colum;
                nowColum = 1;
                jpName = nowColum + "";
                jpDis = initDis + "";
                jpStatus = true;
                pf1.UpdateFlp(nowColum);
                formShowStatus = 1;
                pf1.Show();
            }

        }
        /// <summary>
        /// 浏览图像
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button10_Click(object sender, EventArgs e)
        {
            if (recTimeR0 > 0)
            {
                FrmTips.ShowTipsError(this, "考试中请勿进行此操作");
                return;
            }
            openImgList();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OutPutScore();
        }
        //导出excel
        private bool OutPutScore(bool flag = false)
        {
            bool result = false;
            try
            {
                string path = Application.StartupPath + $"\\data\\excel\\";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                path += $"output{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";
                Dictionary<string, string> dict0 = sQLiteHelper.ExecuteReaderOne($"SELECT RoundCount FROM SportProjectInfos WHERE Id='{_ProjectId}' ");
                if (dict0.Count == 0)
                {
                    FrmTips.ShowTipsError(this, "数据错误");
                    return false;
                }
                int.TryParse(dict0["RoundCount"], out int RoundCount);
                List<Dictionary<string, string>> ldic = new List<Dictionary<string, string>>();
                //序号 项目名称    组别名称 姓名  准考证号 考试状态    第1轮 第2轮 最好成绩
                string sql = $"SELECT BeginTime, Id, GroupName, Name, IdNumber,State,Sex FROM DbPersonInfos WHERE ProjectId='{_ProjectId}' ";
                if (!flag)
                {
                    sql += $" AND GroupName = '{_GroupName}'";
                }
                List<Dictionary<string, string>> list1 = sQLiteHelper.ExecuteReaderList(sql);
                int step = 1;
                foreach (var item1 in list1)
                {
                    Dictionary<string, string> dic = new Dictionary<string, string>();
                    dic.Add("序号", step + "");
                    dic.Add("项目名称", _ProjectName);
                    dic.Add("组别名称", item1["GroupName"]);
                    dic.Add("姓名", item1["Name"]);
                    dic.Add("准考证号", item1["IdNumber"]);
                    dic.Add("性别", item1["Sex"] == "0" ? "男" : "女");
                    string state0 = ResultState.ResultState2Str(item1["State"]);
                    dic.Add("考试状态", state0);
                    List<Dictionary<string, string>> list2 = sQLiteHelper.ExecuteReaderList(
                        $"SELECT * FROM ResultInfos WHERE PersonId='{item1["Id"]}' And IsRemoved=0 ORDER BY RoundId ASC LIMIT {RoundCount}");
                    int step2 = 1;
                    double maxScore = 0;
                    foreach (var item2 in list2)
                    {
                        double.TryParse(item2["Result"], out double result0);
                        int.TryParse(item2["State"], out int state1);
                        if (result0 > maxScore) maxScore = result0;
                        if (state1 == 1)
                        {
                            dic.Add($"第{step2}轮", result0 + "");
                        }
                        else
                        {
                            dic.Add($"第{step2}轮", ResultState.ResultState2Str(state1));
                        }
                        step2++;
                    }
                    for (int i = step2; i <= RoundCount; i++)
                    {
                        dic.Add($"第{i}轮", "");
                    }
                    if (step2 > 1)
                    {
                        dic.Add($"最终成绩", maxScore + "");
                    }
                    else
                    {
                        dic.Add($"最终成绩", "");
                    }

                    ldic.Add(dic);
                    step++;
                }
                result = ExcelUtils.MiniExcel_OutPutExcel(path, ldic);
                //result = ExcelUtils.OutPutExcel(ldic, path);
                if (result)
                {
                    /* System.Diagnostics.Process p = new System.Diagnostics.Process();
                     p.StartInfo.CreateNoWindow = true;
                     p.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                     p.StartInfo.UseShellExecute = true;
                     p.StartInfo.FileName = path;
                     p.StartInfo.Verb = "print";
                     p.Start();*/
                   /* Workbook workbook = new Workbook();
                    workbook.LoadFromFile("Sample.xlsx");
                    Worksheet sheet = workbook.Worksheets[0];
                    sheet.PageSetup.PrintArea = "A7:T8";
                    sheet.PageSetup.PrintTitleRows = "$1:$1";
                    sheet.PageSetup.FitToPagesWide = 1;
                    sheet.PageSetup.FitToPagesTall = 1;
                    //sheet.PageSetup.Orientation =PageOrientationType.Landscape;
                    //sheet.PageSetup.PaperSize = PaperSizeType.PaperA3;
                    PrintDialog dialog = newPrintDialog();
                    dialog.AllowPrintToFile = true;
                    dialog.AllowCurrentPage = true;
                    dialog.AllowSomePages = true;
                    dialog.AllowSelection = true;
                    dialog.UseEXDialog = true;
                    dialog.PrinterSettings.Duplex = Duplex.Simplex;
                    dialog.PrinterSettings.FromPage = 0;
                    dialog.PrinterSettings.ToPage = 8;
                    dialog.PrinterSettings.PrintRange = PrintRange.SomePages;
                    workbook.PrintDialog = dialog;
                    PrintDocument pd = workbook.PrintDocument;
                    if (dialog.ShowDialog() == DialogResult.OK)
                    { pd.Print(); }*/
                    Workbook workbook = new Workbook();
                    workbook.LoadFromFile(path);
                    Worksheet sheet = workbook.Worksheets[0];
                    //设置打印纸张大小
                    sheet.PageSetup.PaperSize = PaperSizeType.PaperA4;
                   // sheet.PageSetup.PrintArea = "B2:F8";
                    PrintDialog dialog = new PrintDialog();
                    dialog.AllowPrintToFile = true;
                    dialog.AllowCurrentPage = true;
                    dialog.AllowSomePages = true;
                    dialog.AllowSelection = true;
                    dialog.UseEXDialog = true;
                    dialog.PrinterSettings.Duplex = Duplex.Simplex;
                    dialog.PrinterSettings.FromPage = 0;
                    dialog.PrinterSettings.ToPage = 8;
                    dialog.PrinterSettings.PrintRange = PrintRange.SomePages;
                    workbook.PrintDialog = dialog;
                    PrintDocument pd = workbook.PrintDocument;
                    if (dialog.ShowDialog() == DialogResult.OK)
                    { pd.Print(); }
                   // var sl = SetExcelToWorld(path);
                  
                }
                return result;

            }
            catch (Exception ex)
            {
                LoggerHelper.Debug(ex);
                return false;
            }
        }
         
        /// <summary>
        /// 测量
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button13_Click(object sender, EventArgs e)
        {
            MeasureFun();
        }
        /// <summary>
        /// 上一张
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button8_Click(object sender, EventArgs e)
        {
            if (recTimeR0 > 0)
            {
                FrmTips.ShowTipsError(this, "考试中请勿进行此操作");
                return;
            }
            dispDecPic();
        }
        void dispDecPic()
        {
            if (!isHaveStudent())
            {
                uiTabControl1.SelectedIndex = 0;
                return;
            }
            if (imgMs.Count == 0)
            {
                MessageBox.Show("请录像");
                return;
            }
            CloseCamera();
            int i = str2int(nowFileName);

            if (i == 0)
            {
                MessageBox.Show("到尽头了");
                return;
            }
            i--;
            if (imgMs.Count < i)
                i = 1;

            nowFileName = i + "";
            if (null != imgMs[i])
            {
                //setHScrollBarValue(i);
                pictureBox1.Image = imgMs[i].img;
                recImgIndex.Text = $"图片索引:{nowFileName}";
            }
            rgbVideoSourceRePaint();
        }
        /// <summary>
        /// 下一张
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button9_Click(object sender, EventArgs e)
        {
            if (recTimeR0 > 0)
            {
                FrmTips.ShowTipsError(this, "考试中请勿进行此操作");
                return;
            }
            dispIncPic();
        }

        void dispIncPic()
        {
            if (!isHaveStudent())
            {

                uiTabControl1.SelectedIndex = 0;
                return;
            }
            if (imgMs.Count == 0)
            {
                MessageBox.Show("请录像");
                return;
            }
            CloseCamera();
            int i = str2int(nowFileName);
            i++;

            if (i >= imgMs.Count)
            {
                i = imgMs.Count - 1;
                MessageBox.Show("到尽头了");
                return;
            }
            nowFileName = i + "";
            if (null != imgMs[i])
            {
                //setHScrollBarValue(i);
                pictureBox1.Image = imgMs[i].img;
                recImgIndex.Text = $"图片索引:{nowFileName}";
            }

            rgbVideoSourceRePaint();

        }

        private bool beginTest()
        {
            //检查成绩要测第几次
            MeasureLen = 0;//测量长度
            bool IhvaStu = isHaveStudent();
            if (!IhvaStu)
            {
                return IhvaStu;
            }
            nowRaceStudentData.state = 0;
            recTimeR0 = 0;
            imgMs.Clear();
            GC.Collect();
            try
            {
                rgbVideoSourceStart();
                return true;
            }
            catch (Exception ex)
            {
                FrmTips.ShowTipsError(this, "摄像头未开启");
                return false;
            }

        }
        bool isActive=false;
        /// <summary>
        /// 选择考生
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void stuView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            nowRaceStudentData = new RaceStudentData();
            if (stuView.SelectedRows.Count == 0)
            {
                return;
            }
            try
            {
                DataGridViewRow dataGridViewRow = stuView.SelectedRows[0];
                //序号 考号 姓名 成绩 考试状态 上传状态 唯一编号
                nowRaceStudentData.id = dataGridViewRow.Cells[6].Value.ToString();
                nowRaceStudentData.name = dataGridViewRow.Cells[2].Value.ToString();
                nowRaceStudentData.idNumber = dataGridViewRow.Cells[1].Value.ToString();
                String stateStr = dataGridViewRow.Cells[4].Value.ToString();
                int stateInt = ResultState.ResultState2Int(stateStr);
                nowRaceStudentData.state = stateInt;
                nowRaceStudentData.groupName = _GroupName;
                nowRaceStudentData.RoundId = RoundCount0;
                SendScore(nowRaceStudentData.name, "准备考试", "");
                stuView.ReadOnly=true;
            }
            catch (Exception ex)
            {
                nowRaceStudentData = new RaceStudentData();
                LoggerHelper.Debug(ex);
            }
            finally
            {
                pictureBox1.Refresh();
            }

        }

        /// <summary>
        /// 开始录像按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button12_Click(object sender, EventArgs e)
        {
            BeginRec();
        }
        /// <summary>
        /// 测量最大值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button7_Click(object sender, EventArgs e)
        {
            if (isSitting)
            {
                if (imgMs.Count > 0)
                {
                    int nlen = imgMs.Count;
                    int maxW = 0;
                    int maxH = 0;
                    int index = 0;
                    int minWidth = conPoints0[0].X;
                    int maxWidth = conPoints0[3].X;
                    int minHeigh = conPoints0[0].Y;
                    int maxHeigh = conPoints0[3].Y;
                    int anaylistLen = 10;
                    int[] anaylist = new int[anaylistLen];
                    OpenCvSharp.Point[] anaPoint = new OpenCvSharp.Point[nlen];
                    for (int k = 0; k < nlen; k++)
                    {
                        if (k == 54)
                            Console.WriteLine();
                        bool[][] isHand = imgMs[k].isHand;
                        //maxW = 0;
                        //maxH = 0;
                        int maxwT = 0;
                        int maxhT = 0;
                        if (isHand == null) continue;
                        for (int i = minWidth; i < maxWidth; i++)
                        {
                            for (int j = minHeigh; j < maxHeigh; j++)
                            {
                                if (i == 1135 && j == 143)
                                    Console.WriteLine();
                                if (isHand[i][j])
                                {
                                    int nl = i - 5;
                                    if (nl < 0) nl = 0;
                                    bool flag0 = true;
                                    for (int i1 = nl; i1 < i; i1++)
                                    {
                                        if (!isHand[i1][j])
                                        {
                                            flag0 = false; break;
                                        }
                                    }
                                    if (!flag0) continue;
                                    //当前图片最大值
                                    maxwT = i;
                                    maxhT = j;
                                    //全局最大值
                                    if (i > maxW)
                                    {
                                        maxW = i;
                                        maxH = j;
                                        index = k;
                                    }
                                }
                            }
                        }
                        anaPoint[k] = new OpenCvSharp.Point(maxwT, maxhT);
                    }
                    string Log = $"最大值:index:{index},X:{maxW}  y:{maxH}";
                    WriteLog(lrtxtLog, Log, 0);


                    if (imgMs[index] != null && imgMs[index].img != null)
                    {
                        nowFileName = index + "";
                        ControlHelper.ThreadInvokerControl(this, () =>
                        {
                            pictureBox1.Image = imgMs[index].img;
                            recImgIndex.Text = $"图片索引:{nowFileName}";
                            pictureBox1.Refresh();
                        });
                    }
                    startMeasure();
                    dispJumpLength1(maxW, maxH);
                    pictureBox1.Refresh();
                    stopMeasure();

                }
                else
                {
                    UIMessageBox.ShowError("请先开始测试,得到测试图像后在点击");
                    return;
                }
            }

        }
        /// <summary>
        /// 测量停顿值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void button11_Click(object sender, EventArgs e)
        {
            if (isSitting)
            {
                if (imgMs.Count > 0)
                {
                    int nlen = imgMs.Count;
                    int maxW = 0;
                    int maxH = 0;
                    int index = 0;
                    int minWidth = conPoints0[0].X;
                    int maxWidth = conPoints0[3].X;
                    int minHeigh = conPoints0[0].Y;
                    int maxHeigh = conPoints0[3].Y;
                    int anaylistLen = 10;
                    int[] anaylist = new int[anaylistLen];
                    OpenCvSharp.Point[] anaPoint = new OpenCvSharp.Point[nlen];
                    for (int k = 0; k < nlen; k++)
                    {
                        bool[][] isHand = imgMs[k].isHand;
                        //maxW = 0;
                        //maxH = 0;
                        int maxwT = 0;
                        int maxhT = 0;
                        if (isHand == null) continue;
                        for (int i = minWidth; i < maxWidth; i++)
                        {
                            for (int j = minHeigh; j < maxHeigh; j++)
                            {
                                if (isHand[i][j])
                                {
                                    int nl = i - 5;
                                    if (nl < 0) nl = 0;
                                    bool flag0 = true;
                                    for (int i1 = nl; i1 < i; i1++)
                                    {
                                        if (!isHand[i1][j])
                                        {
                                            flag0 = false; break;
                                        }
                                    }
                                    if (!flag0) continue;
                                    //当前图片最大值
                                    maxwT = i;
                                    maxhT = j;
                                    //全局最大值
                                    if (i > maxW)
                                    {
                                        maxW = i;
                                        maxH = j;
                                        index = k;
                                    }
                                }
                            }
                        }
                        anaPoint[k] = new OpenCvSharp.Point(maxwT, maxhT);
                    }
                    string Log = $"最大值:index:{index},X:{maxW}  y:{maxH}";
                    WriteLog(lrtxtLog, Log, 0);
                    int iFori = -1;
                    int iFlag = 0;
                    for (int i = index; i > 0; i--)
                    {
                        bool isBreakForj = true;
                        int ax = anaPoint[i].X;
                        int end = (index + 1) > nlen - 1 ? nlen - 1 : index + 2;
                        int begin = (index - 10) < 0 ? 0 : index - 2;
                        for (int j = begin; j < end; j++)
                        {
                            int ipx = Math.Abs(ax - anaPoint[j].X);
                            if (ipx > 5) isBreakForj = false; break;
                        }
                        if (isBreakForj)
                        {
                            iFori = i; break;
                        }
                    }

                    if (iFori != -1)
                    {
                        int maxwT = anaPoint[iFori].X;
                        int maxhT = anaPoint[iFori].Y;
                        if ((maxwT < maxWidth && maxwT > minWidth) && maxhT < maxHeigh && maxhT > minHeigh)
                        {
                            maxW = maxwT;
                            maxH = maxhT;
                            index = iFori;
                        }
                    }
                    Log = $"筛选值:index:{index},X:{maxW}  y:{maxH}";
                    WriteLog(lrtxtLog, Log, 0);

                    if (imgMs[index] != null && imgMs[index].img != null)
                    {
                        nowFileName = index + "";
                        ControlHelper.ThreadInvokerControl(this, () =>
                        {
                            pictureBox1.Image = imgMs[index].img;
                            recImgIndex.Text = $"图片索引:{nowFileName}";
                            pictureBox1.Refresh();

                        });

                    }
                    startMeasure();
                    dispJumpLength1(maxW, maxH);
                    pictureBox1.Refresh();
                    stopMeasure();
                }
                else
                {
                    UIMessageBox.ShowError("请先开始测试,得到测试图像后在点击");
                    return;
                }

            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openCameraBtn_Click(object sender, EventArgs e)
        {
            if (recTimeR0 > 0)
            {
                FrmTips.ShowTipsError(this, "考试中请勿进行此操作");
                return;
            }
            if (openCameraBtn.Text == "关闭摄像头")
            {
                CloseCamera();
                //openCameraBtn.Text = "打开摄像头";
                return;
            }
            if (string.IsNullOrEmpty(cameraName))
            {
                FrmTips.ShowTipsError(this, "请选择摄像头!");
                return;
            }
            openCamearaFun(cameraName);
        }
        /// <summary>
        /// 摄像头属性设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button6_Click(object sender, EventArgs e)
        {
            if (recTimeR0 > 0)
            {
                FrmTips.ShowTipsError(this, "考试中请勿进行此操作");
                return;
            }
            openCameraSetting();
        }
        string cameraName = String.Empty;
        int maxFps = 0;
        int Fps = 0;
        /// <summary>
        /// 打开摄像头设置
        /// </summary>
        public void openCameraSetting()
        {
            filterInfoCollection = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            FrmCameraProperty frmc = new FrmCameraProperty();
            frmc.filterInfoCollection = filterInfoCollection;
            if (frmc.ShowDialog() == DialogResult.OK)
            {
                cameraName = frmc.cameraName;
                maxFps = frmc.maxFps;
                Fps = frmc.Fps;
                if (Fps == 0)
                {
                    cameraSkip = maxFps;
                }
                else
                {
                    cameraSkip = maxFps / Fps;
                }

                if (!string.IsNullOrEmpty(cameraName))
                {
                    openCamearaFun(cameraName);
                }
            }
        }

        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            if (imgMs.Count == 0)
            {
                hScrollBar1.Value = 0;
                hScrollBar1.Maximum = 0;
                return;
            }
            int sp = hScrollBar1.Value;
            int imgcount = imgMs.Count - 1;
            hScrollBar1.Maximum = imgcount;

            if (sp > imgcount)
            {
                sp = imgcount;
            }
            if (null != imgMs[sp])
            {
                nowFileName = sp + "";
                pictureBox1.Image = imgMs[sp].img;
                recImgIndex.Text = $"图片索引:{nowFileName}";
                rgbVideoSourceRePaint();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProjectNameCbx_TextChanged(object sender, EventArgs e)
        {
            if (ProjectNameCbx.Text.Trim() == "立定跳远")
            {
                isBallCheckBoxb = false;
                isShowImgList = true;
                isShotPut = false;
                isSitting = false;
                checkBox2.Visible = false;
                button7.Visible = false;
                button11.Visible = false;
            }
            else if (ProjectNameCbx.Text.Trim() == "投掷实心球")
            {
                isBallCheckBoxb = true;
                isShowImgList = false;
                isShotPut = false;
                isSitting = false;
                checkBox2.Visible = false;
                button7.Visible = false;
                button11.Visible = false;
            }
            else if (ProjectNameCbx.Text.Trim() == "坐位体前屈")
            {
                isBallCheckBoxb = false;
                isShowImgList = false;
                isShotPut = false;
                isSitting = true;
                checkBox2.Visible = true;
                checkBox2.Checked = true;
                button7.Visible = true;
                button11.Visible = true;
                dangwei = "厘米";
            }
            else if (ProjectNameCbx.Text.Trim() == "投掷铅球")
            {
                isBallCheckBoxb = true;
                isShowImgList = false;
                isShotPut = true;
                isSitting = false;
                checkBox2.Visible = false;
                button7.Visible = false;
                button11.Visible = false;
            }
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            isMirrorMode = checkBox1.Checked;
        }
        
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RunTest_KeyDown(object sender, KeyEventArgs e)
        {
            if (uiTabControl1.SelectedIndex != 0) return;
            if (e.KeyCode == Keys.Space)
            {
                BeginRec();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.A)
            {
                if (recTimeR0 > 0) return;
                //上一张
                dispDecPic();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.D)
            {
                //下一张
                dispIncPic();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.S)
            {
                if (recTimeR0 > 0) return;
                //测量
                MeasureFun();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.W)
            {
                WriteScore2Db();

                e.Handled = true;
            }
        }

        private void stuView_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (e.RowIndex >= 0)
                {
                    stuView.ClearSelection();
                    stuView.Rows[e.RowIndex].Selected = true;
                    stuView.CurrentCell = stuView.Rows[e.RowIndex].Cells[e.ColumnIndex];
                    contextMenuStrip1.Show(MousePosition.X, MousePosition.Y);
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (recTimeR0 > 0)
            {
                FrmTips.ShowTipsError(this, "考试中请勿进行此操作");
                return;
            }
            ParameterizedThreadStart method = new ParameterizedThreadStart((obj) =>
            {
                EquipmentCodeForm ecf = new EquipmentCodeForm();
                ecf.sQLiteHelper = sQLiteHelper;
                ecf.ShowDialog();
            });
            Thread threadRead = new Thread(method);
            threadRead.IsBackground = true;
            threadRead.Start();

        }

        bool isShowHandFlag = false;
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            isShowHandFlag = checkBox2.Checked;
        }




        /// <summary>
        /// 写入结果集
        /// </summary>
        private void input2Result()
        {
            if (!isHaveStudent())
            {
                uiTabControl1.SelectedIndex = 0;
                return;
            }
            if (imgMs.Count < 1)
            {
                FrmTips.ShowTipsError(this, "先录像");
                return;
            }
            makeEndResult();
            saveOkFileOut();
            startMeasure();
            ///自动上传成绩
            //if (isAutoUpload.Checked)
            //uploadStudentScore();
            //自动下一个
            //nextMode();
            rgbVideoSourceStart();
        }
        void makeEndResult()
        {
            double fenmu = 1000;
            if (isSitting)
            {
                fenmu = 10;
            }
            string score = (MeasureLen / fenmu).ToString(ReservedDigitsTxt);
            nowRaceStudentData.score = Convert.ToDouble(score);
            string projectTypeCbxtxt = ProjectNameCbx.Text;
            string txt_Grouptxt = _GroupName;
            string txt_GNametxt = nowRaceStudentData.name;
            string ScoreStatus = ResultState.ResultState2Str(nowRaceStudentData.state);
            /*  string scoreContent = $"时间:{DateTime.Now.ToString("yyyy年MM月dd日HH:mm:ss")} , " +
                  $"项目:{projectTypeCbxtxt} , 组别:{txt_Grouptxt} , 准考证号:{nowRaceStudentData.idNumber} ,  姓名：{txt_GNametxt} ,  " +
                  $"第{RoundCount0}次成绩:{score}, 状态:{ScoreStatus}";*/
            string scoreContent = string.Format("时间:{0,-20},项目:{1,-20},组别:{2,-10},准考证号:{3,-20},姓名{4,-10},第{5}次成绩:{6,-5}, 状态:{7,-5}",
                DateTime.Now.ToString("yyyy年MM月dd日HH: mm:ss"),
                projectTypeCbxtxt,
                txt_Grouptxt,
                nowRaceStudentData.idNumber,
                txt_GNametxt,
                RoundCount0,
                score,
                ScoreStatus);
            System.IO.File.AppendAllText(@"./成绩日志.txt", scoreContent + "\n");
            updateJumpLen();
        }
        /// <summary>
        /// 写入数据库
        /// </summary>
        void updateJumpLen()
        {
            System.Data.SQLite.SQLiteTransaction sQLiteTransaction = sQLiteHelper.BeginTransaction();
            try
            {
                var sortid = sQLiteHelper.ExecuteScalar($"SELECT MAX(SortId) + 1 FROM ResultInfos");
                string sortid0 = "1";
                if (sortid != null && sortid.ToString() != "") sortid0 = sortid.ToString();
                int state0 = nowRaceStudentData.state == 0 ? 1 : nowRaceStudentData.state;
                string sql = $"INSERT INTO ResultInfos(CreateTime,SortId,IsRemoved,PersonId,SportItemType,PersonName,PersonIdNumber,RoundId,Result,State) " +
                    $"VALUES('{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}', {sortid0}, 0, '{nowRaceStudentData.id}',0,'{nowRaceStudentData.name}'," +
                    $"'{nowRaceStudentData.idNumber}',{RoundCount0},{nowRaceStudentData.score},{state0})";
                //处理写入数据库
                sQLiteHelper.ExecuteNonQuery(sql);
                //更新状态为已考
                sQLiteHelper.ExecuteNonQuery($"UPDATE DbPersonInfos SET State = 1, FinalScore = 1 WHERE Id = '{nowRaceStudentData.id}'");
                FrmTips.ShowTipsSuccess(this, "写入成功");
            }
            catch (Exception ex)
            {
                FrmTips.ShowTipsError(this, ex.Message);
                LoggerHelper.Debug(ex);
            }
            sQLiteHelper.CommitTransAction(sQLiteTransaction);
            if (RoundCount0 > 0)
            {
                UpdateListView(_ProjectId, _GroupName, RoundCount0);
            }
        }

        /// <summary>
        /// 保存文件
        /// </summary>
        void saveOkFileOut()
        {
            //nowTestDir
            string savePath = Path.Combine(nowTestDir, $"落地_{nowRaceStudentData.idNumber}.jpg");
            if (pictureBox1.Image != null)
            {
                imgJpgSave(pictureBox1.Image, savePath);
                Image newImage = pictureBox1.Image;
                {
                    if (null != newImage)
                    {
                        using (var graphic = Graphics.FromImage(newImage))
                        {
                            // 核心参数啊，感觉相当于PS保存时间的质量设置参数
                            Int64 qualityLevel = 80L;
                            Pen pen = new Pen(Color.MediumSpringGreen, 1);
                            Font drawFont = new Font("Arial", 14);
                            SolidBrush drawBrush = new SolidBrush(Color.MediumSpringGreen);
                            System.Drawing.Point markerTopJumpT = new System.Drawing.Point(0, 0);
                            System.Drawing.Point markerBottomJumpT = new System.Drawing.Point(0, 0);
                            List<TargetPoint> TopList = new List<TargetPoint>();
                            List<TargetPoint> BottomList = new List<TargetPoint>();
                            markerTopJumpT = markerTopJump;
                            markerBottomJumpT = markerBottomJump;
                            TopList = targetPoints.FindAll(a => a.status);
                            BottomList = targetPoints.FindAll(a => !a.status);
                            List<TargetPoint> TopListSort = TopList.OrderBy(a => a.x).ToList();
                            List<TargetPoint> BottomListSort = BottomList.OrderBy(a => a.x).ToList();
                            // 高质量
                            graphic.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                            graphic.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                            graphic.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                            qualityLevel = 100L;
                            System.Drawing.Imaging.ImageCodecInfo codec = System.Drawing.Imaging.ImageCodecInfo.GetImageEncoders()[1];
                            System.Drawing.Imaging.EncoderParameters eParams = new System.Drawing.Imaging.EncoderParameters(1);
                            eParams.Param[0] = new System.Drawing.Imaging.EncoderParameter(System.Drawing.Imaging.Encoder.Quality, qualityLevel);
                            {
                                //框点十字
                                int left = 0;
                                //画顶标
                                foreach (var mark in TopListSort)
                                {
                                    System.Drawing.Point p = new System.Drawing.Point(mark.x, mark.y);

                                    if (left <= TopListSort.Count - 2)
                                    {
                                        System.Drawing.Point p1 = new System.Drawing.Point(TopListSort[left + 1].x, TopListSort[left + 1].y);
                                        graphic.DrawLine(pen, p, p1);
                                        left++;
                                    }
                                    drawPointCross(graphic, p, pen);
                                    drawPointText(graphic, $"({mark.name}){mark.dis}cm", p, drawFont, drawBrush, 0, 60, 0, 30);
                                }
                                left = 0;
                                //画底标
                                foreach (var mark in BottomListSort)
                                {
                                    System.Drawing.Point p = new System.Drawing.Point(mark.x, mark.y);

                                    if (left <= BottomListSort.Count - 2)
                                    {
                                        System.Drawing.Point p1 = new System.Drawing.Point(BottomListSort[left + 1].x, BottomListSort[left + 1].y);
                                        graphic.DrawLine(pen, p, p1);
                                        left++;
                                    }
                                    drawPointCross(graphic, p, pen);
                                    drawPointText(graphic, $"({mark.name}){mark.dis}m", p, drawFont, drawBrush, 0, 60, 1, 10);
                                }
                                //中间框点连线
                                int min = TopListSort.Count > BottomListSort.Count ? BottomListSort.Count : TopListSort.Count;
                                for (int i = 0; i < min; i++)
                                {
                                    TargetPoint mark = TopListSort[i];
                                    TargetPoint mark1 = BottomListSort[i];
                                    System.Drawing.Point p = new System.Drawing.Point(mark.x, mark.y);
                                    System.Drawing.Point p1 = new System.Drawing.Point(mark1.x, mark1.y);
                                    graphic.DrawLine(pen, p, p1);

                                }
                            }

                            pen.Color = Color.Red;
                            drawFont = new Font("Arial", 32);
                            graphic.DrawLine(pen, markerTopJumpT, markerBottomJumpT);
                            drawBrush = new SolidBrush(Color.Red);// Create point for upper-left corner of drawing.
                            if (isSitting)
                            {
                                graphic.DrawString((MeasureLen / 10).ToString(ReservedDigitsTxt) + dangwei, drawFont, drawBrush, markerBottomJumpT.X - 70, markerBottomJumpT.Y);

                            }
                            else
                            {
                                graphic.DrawString((MeasureLen / 1000).ToString(ReservedDigitsTxt) + dangwei, drawFont, drawBrush, markerBottomJumpT.X - 70, markerBottomJumpT.Y);

                            }

                            //时间 
                            graphic.FillRectangle(new SolidBrush(Color.White), new Rectangle(new System.Drawing.Point(350, 0), new System.Drawing.Size(300, 50)));
                            graphic.DrawString($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}", new Font("Arial", 20), drawBrush, 360, 10);

                            //考生姓名和学号
                            graphic.FillRectangle(new SolidBrush(Color.White), new Rectangle(new System.Drawing.Point(700, 0), new System.Drawing.Size(540, 50)));
                            graphic.DrawString($"组号:{nowRaceStudentData.groupName} 考号:{nowRaceStudentData.idNumber} 姓名:{nowRaceStudentData.name}", new Font("Arial", 18), drawBrush, 710, 10);
                            if (isShotPut)
                            {
                                pen.DashPattern = new float[] { 5f, 10f };
                                graphic.DrawLine(pen, cenMarkPoint, mousePoint);
                            }
                            string imgOkFileName = Path.Combine(nowTestDir, $"{nowRaceStudentData.idNumber}.jpg");// = 0;//第几次考试
                            if (System.IO.File.Exists(imgOkFileName))
                                System.IO.File.Delete(imgOkFileName);

                            newImage.Save(imgOkFileName, codec, eParams);
                        }
                    }
                }
            }
            //当前图片索引
            int nowFileName1 = str2int(nowFileName);
            if (isBallCheckBoxb)
            {
                if (nowFileName1 == 0 || nowFileName1 > imgMs.Count - 1)
                    return;
            }
            //保存前面第5帧
            int sum = 5;
            for (int i = 0; i < sum; i++)
            {
                if (i < imgMs.Count)
                {
                    savePath = Path.Combine(nowTestDir, $"{nowRaceStudentData.idNumber}_开始{i}.jpg");
                    imgJpgSave(imgMs[i].img, savePath);
                }
            }
            //落地前5帧
            int sp = 5;
            int fsum = nowFileName1 - sp;
            if (fsum < 0)
            {
                fsum = 0;
            }
            for (int i = fsum; i < nowFileName1; i++)
            {
                if (i < imgMs.Count)
                {
                    savePath = Path.Combine(nowTestDir, $"{nowRaceStudentData.idNumber}_落地前{i}.jpg");
                    imgJpgSave(imgMs[i].img, savePath);
                }
            }
            //落地后
            int backsum = nowFileName1 + sp;
            if (backsum >= imgMs.Count)
            {
                backsum = imgMs.Count;
            }
            for (int i = nowFileName1; i < backsum; i++)
            {
                if (i < imgMs.Count)
                {
                    savePath = Path.Combine(nowTestDir, $"{nowRaceStudentData.idNumber}_落地后{i}.jpg");
                    imgJpgSave(imgMs[i].img, savePath);
                }
            }
        }

        void imgJpgSave(Image img, string path)
        {
            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);
            img.Save(path);
        }


        #endregion 控制面板




        #region 单人显示屏

        ScreenSerialReader sReader = null;

        private void AnalyData(byte[] btData)
        {



        }

        private void ReceiveData(byte[] btData)
        {



        }


        private void SendData(byte[] btData)
        {



        }
        /// <summary>
        /// 初始化串口
        /// </summary>
        /// <returns></returns>
        bool serialInit()
        {
            bool flag = true;
            try
            {
                sReader = new ScreenSerialReader();
                sReader.AnalyCallback = AnalyData;
                sReader.ReceiveCallback = ReceiveData;
                sReader.SendCallback = SendData;

                SerialTool.init();
                if (RefreshComPorts())
                {
                    //serialInit();
                    ConnectPort();
                }

            }
            catch (Exception)
            {

                flag = false;
            }
            return flag;
        }

        private void recTimeTxt_SelectedIndexChanged(object sender, EventArgs e)
        {

        }





        /// <summary>
        /// 刷新串口
        /// </summary>
        /// <returns></returns>
        bool RefreshComPorts()
        {
            bool flag = false;
            try
            {
                string portFind = portNameSearch.Text;
                string[] portNames = SerialTool.getPortDeviceName(portFind);
                portNamesList.Items.Clear();
                foreach (var item in portNames)
                {
                    portNamesList.Items.Add(item);
                }
                if (portNames.Length > 0)
                {
                    flag = true;
                    portNamesList.SelectedIndex = 0;
                }
            }
            catch (Exception)
            {

                flag = false;
            }

            return flag;
        }


        /// <summary>
        /// 串口连接
        /// </summary>
        void ConnectPort()
        {
            if (sReader.IsConnectOpen())
            {
                //处理串口断开连接读写器
                sReader.CloseConnect();
                openSerialPortBtn.Text = "打开串口";
                FrmTips.ShowTipsInfo(this, "关闭串口");
            }
            else
            {
                try
                {
                    string strPort = SerialTool.PortDeviceName2PortName(portNamesList.Text);
                    if (string.IsNullOrEmpty(strPort))
                    {
                        FrmTips.ShowTipsError(this, "选择单人显示屏串口");
                        return;
                    }

                    int.TryParse(tb_nBaudrate.Text, out int nBaudrate);
                    string strException = string.Empty;
                    int nRet = sReader.OpenConnect(strPort, nBaudrate, out strException);
                    if (nRet == -1)
                    {
                        openSerialPortBtn.Text = "打开串口";
                        FrmTips.ShowTipsError(this, "打开单人显示屏失败");
                    }
                    else
                    {
                        openSerialPortBtn.Text = "关闭串口";
                        FrmTips.ShowTipsInfo(this, "打开单人显示屏串口成功");
                    }
                }
                catch (Exception ex)
                {

                    LoggerHelper.Debug(ex);
                }

            }

        }

        /// <summary>
        /// 发送成绩至小屏
        /// </summary>
        /// <param name="name"></param>
        /// <param name="Score"></param>
        bool SendScore(string name, string Score, string txt3 = "米")
        {
            if (!sReader.IsConnectOpen()) return false;
            try
            {
                int c1 = 0;//红
                int c2 = 1;//绿
                int c3 = 2;//蓝
                byte[] result = SerialTool.PushText_BL2(name, c1, Score, c2, txt3, c3);
                sReader.SendMessage(result);
            }
            catch (Exception ex)
            {
                LoggerHelper.Debug(ex);
                return false;
            }

            return true;
        }

        private void lrtxtLog_DoubleClick(object sender, EventArgs e)
        {
            lrtxtLog.BeginInvoke(new ThreadStart((MethodInvoker)delegate ()
            {
                lrtxtLog.Clear();
            }));
        }
        public void WriteLog(CustomControl.LogRichTextBox logRichTxt, string strLog, int nType)
        {
            try
            {
                logRichTxt.BeginInvoke(new ThreadStart((MethodInvoker)delegate ()
                {
                    if (logRichTxt.Lines.Length > 100)
                    {
                        logRichTxt.Clear();
                    }
                    if (nType == 0)
                    {
                        logRichTxt.AppendTextEx(strLog, Color.Indigo);
                    }
                    else if (nType == 2)
                    {
                        logRichTxt.AppendTextEx(strLog, Color.Blue);
                    }
                    else if (nType == 1)
                    {
                        logRichTxt.AppendTextEx(strLog, Color.Red);
                    }
                    else if (nType == 3)
                    {
                        logRichTxt.AppendTextEx(strLog, Color.DarkGreen);
                    }

                    logRichTxt.Select(logRichTxt.TextLength, 0);
                    logRichTxt.ScrollToCaret();
                }));
            }
            catch (Exception ex)
            {

                LoggerHelper.Debug(ex);
            }
        }

        #endregion


    }
}
                                                                                                            