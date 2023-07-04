using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace TrunkPressingCore 
{
     
        public delegate void ReciveDataCallback(byte[] btAryReceiveData);
        public delegate void SendDataCallback(byte[] btArySendData);
        public delegate void AnalyDataCallback(byte[] btAryAnalyData);
    public class ScreenSerialReader
    {
        private SerialPort iSerialPort;
        private int m_nType = -1;
        public ReciveDataCallback ReceiveCallback;
        public SendDataCallback SendCallback;
        public AnalyDataCallback AnalyCallback;

        private System.Timers.Timer waitTimer;
        /// <summary>
        /// 缓存数据
        /// </summary>
        byte[] s232Buffer = new byte[2048];
        int s232Buffersp = 0; 
        public ScreenSerialReader()
        {
            iSerialPort = new SerialPort();
            iSerialPort.DataReceived += new SerialDataReceivedEventHandler(ReceivedComData);
        }
        /// <summary>
        ///  kai chuangko
        /// </summary>
        /// <param name="strport"></param>
        /// <param name="nbaudrate"></param>
        /// <param name="strException"></param>
        public  int OpenConnect(string  strport, int nbaudrate, out string strException)
        {
            strException = string.Empty;
            if( iSerialPort.IsOpen)
            {
                iSerialPort.Close();

            }
            try
            {
                iSerialPort.PortName = strport;
                iSerialPort.BaudRate = nbaudrate;
                iSerialPort.StopBits = StopBits.One;
                iSerialPort.Parity = Parity.None;
                iSerialPort.ReadTimeout = 10;
                iSerialPort.WriteTimeout = 1000;
                iSerialPort.ReadBufferSize = 4096 * 10;
                iSerialPort.Open();

                // 建立定时器处理数据
                waitTimer = new System.Timers.Timer(1000    );
                waitTimer.Elapsed += new System.Timers.ElapsedEventHandler(AnalyReceivedData);
                waitTimer.AutoReset = true;//设置是执行一次（false）还是一直执行(true)
                waitTimer.Enabled = true;
                waitTimer.Start();//是否执行System.Timers.Timer.Elapsed事件；

            }
            catch(Exception ex)
            {
                Console.WriteLine(ex. Message);
                strException = ex.Message;
                return -1;
            }
            m_nType = 0;
            return  0;
        }
        /// <summary>
        ///  关闭串口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void CloseConnect()
        {
            if( iSerialPort.IsOpen)
            {
                iSerialPort.Close();

            }
            m_nType = -1;
        }
        /// <summary>
        /// 串口是u否连接
        /// </summary>
        /// <returns></returns>
        public  bool IsConnectOpen()
        {
            try
            {
                 return iSerialPort.IsOpen;
            }
            catch (Exception)
            {

                return false;
            }
        }
        /// <summary>
        /// 定时器处理数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AnalyReceivedData(object sender, ElapsedEventArgs e)
        {
            if (waitTimer != null) waitTimer.Stop();
            if(s232Buffersp != 0)
            {
                byte[] buffer = new byte[s232Buffersp];
                Array.Copy(s232Buffer, 0, buffer, 0, s232Buffersp);
                Array.Clear (s232Buffer ,0, s232Buffersp);  
                s232Buffersp = 0;
                RunReceieveDataCallback(buffer);
            }
            if(waitTimer != null) waitTimer.Start();
             
        }

        private void RunReceieveDataCallback(byte[] buffer)
        {
            try
            {
                if (ReceiveCallback != null)
                {
                    ReceiveCallback(buffer);
                }
                int X = buffer.Length;
                if(AnalyCallback != null)
                {
                    AnalyCallback(buffer);
                }
            }
            catch (Exception ex )
            {
                Console.WriteLine(ex. Message);
            }
        }

        public  int SendMessage(byte[] bytes)
        {
            if (m_nType == 0)
            {
                if (!iSerialPort.IsOpen)
                {
                    return -1;

                }
                iSerialPort.Write(bytes, 0, bytes.Length);
                if (SendCallback != null)
                {
                    SendCallback(bytes);
                }
                return 0;
            }
            return -1;
        }

        /// <summary>
        /// 串口接受数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReceivedComData(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                int count = iSerialPort.BytesToRead;
                   
                if(count==0)
                {
                    return;
                }
                byte[] data = new byte[count];
                iSerialPort.Read(data, 0, count);
                for(int i = 0; i < count; i++)
                {
                    s232Buffer[s232Buffersp]  = data[i];
                    if (s232Buffersp < (s232Buffer.Length - 2))
                    {
                        s232Buffersp++;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
    }

}
