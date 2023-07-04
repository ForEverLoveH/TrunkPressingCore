using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Text;
using System.Threading ;
using System.Windows.Forms;
using System.Xml.Linq;

namespace TrunkPressingCore.Window
{
    public partial class LoginWindow : Form
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void LoginWindow_Load(object sender, EventArgs e)
        {
            serialInit();
          
        }
        private void LoginWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (reader != null && reader.IsConnectOpen())
            {
                //处理串口断开连接读写器
                reader.CloseConnect();
            }
        }

        List<string> portNamesList = new List<string>();
        ScreenSerialReader reader;

        private bool encrypt()
        {
            try
            {
                string path = Application.StartupPath + "\\encryptCode.txt";
                string cpuid = cpuHelper.GetCpuID();
                if (!File.Exists(path))
                {
                    File.WriteAllText(path, cpuid);
                    return false;
                }
                string value = File.ReadAllText(path);
                string v = cpuHelper.Encrypt(cpuid);
                if (value != v)
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                // LoggerHelper.Debug(ex);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 刷新串口
        /// </summary>
        /// <returns></returns>
        bool RefreshComPorts()
        {
            return true;
            bool flag = false;
            try
            {
                if (reader.IsConnectOpen())
                {
                    //处理串口断开连接读写器
                    reader.CloseConnect();
                }
                loadSuccess = false;
                string portFind = "USB 串行设备";
                string[] portNames = getPortDeviceName(portFind);
                portNamesList.Clear();
                foreach (var item in portNames)
                {
                    portNamesList.Add(item);
                }
                if (portNames.Length > 0)
                {
                    flag = true;
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
            if (reader.IsConnectOpen())
            {
                //处理串口断开连接读写器
                reader.CloseConnect();
            }
            else
            {
                string strPort = PortDeviceName2PortName(portNamesList[0]);
                if (string.IsNullOrEmpty(strPort))
                {
                    return;
                }
                int nBaudrate = 9600;
                string strException = string.Empty;
                int nRet = reader.OpenConnect(strPort, nBaudrate, out strException);

            }

        }

        /// <summary>
        /// 初始化串口
        /// </summary>
        /// <returns></returns>
        bool serialInit()
        {
            bool flag = false;
            try
            {
                reader = new ScreenSerialReader();
                reader.AnalyCallback = AnalyData;
                reader.ReceiveCallback = ReceiveData;
                reader.SendCallback = SendData;

            }
            catch (Exception)
            {

                flag = false;
            }
            return flag;
        }
        /// <summary>
        /// 获取串口信息
        /// </summary>
        /// <returns></returns>
        public static string[] getPortDeviceName(string comName)
        {
            List<string> strs = new List<string>();
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from Win32_PnPEntity where Name like '%(COM%'"))
            {
                var hardInfos = searcher.Get();
                foreach (var hardInfo in hardInfos)
                {
                    if (hardInfo.Properties["Name"].Value != null)
                    {
                        string deviceName = hardInfo.Properties["Name"].Value.ToString();
                        if (deviceName.Contains(comName))
                        {
                            strs.Add(deviceName);
                        }
                    }
                }
            }
            return strs.ToArray();
        }


        public static string PortDeviceName2PortName(string deviceName)
        {
            try
            {
                int a = deviceName.IndexOf("(COM") + 1;//a会等于1
                string str = deviceName.Substring(a, deviceName.Length - a);
                a = str.IndexOf(")");//a会等于1
                str = str.Substring(0, a);

                return str;
            }
            catch (Exception)
            {

                return "";
            }

        }

        bool beginVer = false;
        bool loadSuccess = false;
        bool loadSuccess1 = false;
        private void AnalyData(byte[] btData)
        {
            if (beginVer)
            {
                bool flag = checkByte1(btData);
                if (!flag)
                {
                     uiLabel1.Text = "请连接摄像头";
                    beginVer = true;
                }
                else
                {
                    loadSuccess = true;
                }
            }
        }

        void SendVerData()
        {
            beginVer = true;
            return;
            if (reader != null && reader.IsConnectOpen())
            {
                uiLabel1.Text = "验证摄像头";
                byte[] bytes = test03();
                reader.SendMessage(bytes);
                beginVer = true;
            }
            else
            {
                uiLabel1.Text = "请连接摄像头";
            }
        }

        private void ReceiveData(byte[] btData)
        {
            /* string code = ByteArrayToString(btData, 0, btData.Length);
             string strLog = "Receive:" + hexTextSpace(code);
             WriteLog(strLog);*/
        }
        private void SendData(byte[] btData)
        {
            /*string code = ByteArrayToString(btData, 0, btData.Length);
             string strLog = "Send:" + hexTextSpace(code);
             WriteLog(strLog);*/
        }
        private byte[] test03()
        {
            // opencamera1234567890
            string opencamera = $"opencamera{DateTime.Now.ToString("ddHHmmssff")}";
            byte[] opencameraByte = Encoding.UTF8.GetBytes(opencamera);
            int nlen = opencameraByte.Length;
            Random random = new Random();
            int sp = 0;
            byte[] temp = new byte[100];
            byte[] vlaueTemp = new byte[4];
            vlaueTemp[sp++] = 0x27;
            vlaueTemp[sp++] = (byte)random.Next(255);
            vlaueTemp[sp++] = (byte)random.Next(255);
            vlaueTemp[sp++] = (byte)(vlaueTemp[1] | vlaueTemp[2]);
            byte[] vlaueTemp0 = new byte[4 + nlen];
            Array.Copy(vlaueTemp, vlaueTemp0, 0);
            Array.Copy(opencameraByte, 0, vlaueTemp0, 4, nlen);
            Array.Copy(vlaueTemp, 0, temp, 0, 4);
            sp = 4;
            for (int i = 0; i < 10; i++)
            {
                temp[sp++] = (byte)(vlaueTemp0[i + 4] ^ vlaueTemp0[i + 4 + 10]);
            }
            for (int i = 0; i < 10; i++)
            {
                temp[sp++] = vlaueTemp0[i + 14];
            }
            byte cheack = 0xaa;
            for (int i = 0; i < opencameraByte.Length; i++)
            {
                cheack ^= opencameraByte[i];
            }
            temp[sp++] = cheack;
            byte[] dst = new byte[sp];
            Array.Copy(temp, dst, sp);
            return dst;
        }
        bool checkByte1(byte[] btData)
        {
            beginVer = false;
            int sp = 0;
            bool flag = true;
            bool flag2 = false;
            return true;
            while (flag)
            {
                int[] result = findHeadIndex(btData, sp);
                if (result[0] == -1)
                {
                    flag = false;
                    continue;
                }
                byte cheack = 0xaa;
                for (int i = result[0]; i < result[1]; i++)
                {
                    cheack = (byte)(cheack ^ btData[i]);
                }
                if (cheack == btData[result[1]])
                {
                    flag = false;
                    flag2 = true;
                    break;
                }
            }

            return flag2;
        }

        int[] findHeadIndex(byte[] btData, int begin)
        {
            int[] result = new int[2];
            int sp = -1;
            int nlen = btData.Length;
            int templegth = 0;
            for (int i = begin; i < nlen; i++)
            {
                if (btData[i] == 0x27)
                {
                    sp = i; break;
                }
            }
            if (sp + 4 > nlen - 1)
            {
                sp = -1;
            }
            else
            {
                byte PCcheack = btData[sp + 4];
                if (PCcheack % 5 == 0)
                {
                    templegth = 15;
                }
                else if (PCcheack % 3 == 0)
                {
                    templegth = 20;
                }
                else if (PCcheack % 2 == 0)
                {
                    templegth = 22;
                }
                else
                {
                    templegth = 24;
                }
                if (sp + templegth > nlen - 1)
                {
                    sp = -1;
                }
            }
            result[0] = sp;
            result[1] = sp + templegth;

            return result;
        }

        private void uiLabel1_Click(object sender, EventArgs e)
        {

        }

        private void uiLabel1_DoubleClick(object sender, EventArgs e)
        {
            if (RefreshComPorts())
            {
                ConnectPort();
                SendVerData();
            }
            else
            {
                uiLabel1.Text = "请连接摄像头";
            }
        }

        int beginVerCount = 0;
        int unSuccesssum = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (beginVer)
            {
                beginVerCount++;
                if (beginVerCount > 30)
                {
                    beginVerCount = 0;
                    beginVer = false;
                    uiLabel1.Text = "请连接摄像头";
                }
            }
            loadSuccess = true;
            if (loadSuccess)
            {
                unSuccesssum = 0;
                loadSuccess = false;
                loadSuccess1 = true;
                timer1.Stop();
                this.Hide();
                MainWindow mainWindow = new MainWindow();
                mainWindow.ShowDialog();
                this.Close();
            }
            else
            {
                unSuccesssum++;
                //if (unSuccesssum > 10)
                //{
                //   unSuccesssum = 0;
                //   if (beginVer) return;
                //   if (loadSuccess1) return;
                //   if (!reader.IsComOpen())
                //   {
                //      if (RefreshComPorts())
                //      {
                //         ConnectPort();
                //         SendVerData();
                //      }
                //      else
                //      {
                //         label1.Text = "请连接培林摄像头";
                //      }
                //   }
                //   else
                //   {
                //      SendVerData();
                //   }
                //}
            }
        }
    }
}