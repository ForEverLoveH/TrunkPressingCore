using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TrunkPressingCore
{
    public class MyUtils
    {
        /// <summary>
        /// 自动调整ListView的列宽的方法
        /// </summary>
        /// <param name="lv"></param>
        public void AutoResizeColumnWidth(ListView lv)
        {
            int allWidth = lv.Width;
            int count = lv.Columns.Count;
            int MaxWidth = 0;
            Graphics graphics = lv.CreateGraphics();
            int width;
            lv.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            if (count == 0) return;
            for (int i = 0; i < count; i++)
            {
                string str = lv.Columns[i].Text;
                //MaxWidth = lv.Columns[i].Width;
                MaxWidth = 0;

                foreach (ListViewItem item in lv.Items)
                {
                    try
                    {
                        str = item.SubItems[i].Text;
                        width = (int)graphics.MeasureString(str, lv.Font).Width;
                        if (width > MaxWidth)
                        {
                            MaxWidth = width;
                        }
                    }
                    catch (Exception)
                    {

                        break;
                    }

                }
                lv.Columns[i].Width = MaxWidth;
                allWidth -= MaxWidth;
            }
            if (allWidth > count && count != 0)
            {
                allWidth /= count;
                for (int i = 0; i < count; i++)
                {
                    lv.Columns[i].Width += allWidth;
                }
            }

        }

    }
    public class MemoryTool
    {
        // 获得为该进程(程序)分配的内存. 做一个计时器,就可以时时查看程序占用系统资源
        public static double GetProcessUsedMemory()

        {

            double usedMemory = 0;

            usedMemory = Process.GetCurrentProcess().WorkingSet64 / 1024.0 / 1024.0;

            return usedMemory;

        }

        #region 内存回收
        [DllImport("kernel32.dll", EntryPoint = "SetProcessWorkingSetSize")]
        public static extern int SetProcessWorkingSetSize(IntPtr process, int minSize, int maxSize);

        /// <summary>
        /// 释放内存
        /// </summary>
        public static void ClearMemory()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                SetProcessWorkingSetSize(System.Diagnostics.Process.GetCurrentProcess().Handle, -1, -1);
            }
        }
        #endregion
    }
}

