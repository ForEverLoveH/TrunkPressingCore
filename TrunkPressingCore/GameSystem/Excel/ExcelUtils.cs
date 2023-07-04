﻿using ExcelDataReader;
using MiniExcelLibs;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrunkPressingCore.GameModel;

namespace TrunkPressingCore 
{
    public class ExcelUtils
    { /// <summary>
      /// excel字段转换成键值对
      /// </summary>
      /// <param name="filePath"></param>
      /// <returns></returns>
        public static List<Dictionary<string, string>> ReadExcel(string filePath)
        {
            List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                // Auto-detect format, supports:
                //  - Binary Excel files (2.0-2003 format; *.xls)
                //  - OpenXml Excel files (2007 format; *.xlsx, *.xlsb)
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var configuration = new ExcelDataSetConfiguration { ConfigureDataTable = tableReader => new ExcelDataTableConfiguration { UseHeaderRow = true } };
                    DataSet result = reader.AsDataSet(configuration);
                    for (int i = 0; i < result.Tables[0].Rows.Count; i++)
                    {
                        Dictionary<string, string> dict = new Dictionary<string, string>();
                        for (int j = 0; j < result.Tables[0].Columns.Count; j++)
                        {
                            dict.Add(result.Tables[0].Columns[j].ColumnName, result.Tables[0].Rows[i][j].ToString());
                        }
                        list.Add(dict);
                    }
                }
            }
            return list;
        }
        public static List<ExportStudentData>ReadExcel_Mini(string  path)
        {
            return  MiniExcel.Query<ExportStudentData>(path).ToList();
        }
        private IEnumerable<Dictionary<string, object>> GetIEnumberAble(ExportStudentData id)
        {
            var newCompanyPrepareds = new Dictionary<string, object>();
            newCompanyPrepareds.Add("序号", id.Id);
            newCompanyPrepareds.Add("学校", id.School);
            newCompanyPrepareds.Add("年级", id.GradeName);
            newCompanyPrepareds.Add("班级", id.ClassName);
            newCompanyPrepareds.Add("姓名", id.Name);

            newCompanyPrepareds.Add("性别", id.Sex);
            newCompanyPrepareds.Add("准考证号", id.IdNumber);
            newCompanyPrepareds.Add("组别名称", id.GroupName);

            yield return newCompanyPrepareds;
        }

        private IEnumerable<Dictionary<string, object>> GetOrders(List<ExportStudentData> test)
        {
            foreach (var item in test)
            {
                var newCompanyPrepareds = new Dictionary<string, object>();
                newCompanyPrepareds.Add("序号", item.Id);
                newCompanyPrepareds.Add("学校", item.School);
                newCompanyPrepareds.Add("年级", item.GradeName);
                newCompanyPrepareds.Add("班级", item.ClassName);
                newCompanyPrepareds.Add("姓名", item.Name);

                newCompanyPrepareds.Add("性别", item.Sex);
                newCompanyPrepareds.Add("准考证号", item.IdNumber);
                newCompanyPrepareds.Add("组别名称", item.GroupName);

                yield return newCompanyPrepareds;

            }
        }

        /// <summary>
        /// 导出成绩名单
        /// </summary>
        /// <param name="ldic"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool OutPutExcel(List<Dictionary<string, string>> ldic, string path)
        {
            bool result = false;
            if (ldic.Count == 0) return result;

            HSSFWorkbook workbook = new HSSFWorkbook();
            HSSFSheet sheet = (HSSFSheet)workbook.CreateSheet("Sheet1");//创建一个名称为Sheet0的表;
            HSSFRow row = (HSSFRow)sheet.CreateRow(0);//（第一行写标题)
            Dictionary<string, string> dict = ldic[0];
            int coloum = 0;
            foreach (var key in dict.Keys)
            {
                row.CreateCell(coloum).SetCellValue(key);//第一列标题，
                coloum++;
            }
            for (int i = 0; i < ldic.Count; i++)
            {
                row = (HSSFRow)sheet.CreateRow(i + 1);
                coloum = 0;
                dict = ldic[i];
                foreach (var key in dict.Keys)
                {
                    row.CreateCell(coloum).SetCellValue(dict[key]);//第一列标题，
                    coloum++;
                }
            }
            //文件写入的位置
            using (FileStream fs = File.OpenWrite(path))
            {
                workbook.Write(fs);//向打开的这个xls文件中写入数据  
                result = true;
            }
            return result;
        }
        /// <summary>
        /// MiniExcel导出格式
        /// </summary>
        /// <param name="path"></param>
        /// <param name="value"></param>
        public static bool MiniExcel_OutPutExcel(string path, object value)
        {
            try
            {
                MiniExcel.SaveAs(path, value);
            }
            catch (Exception ex)
            {

                return false;
            }
            return true;
        }


        #region 读取Excel数据
        /// <summary>
        /// 将excel中的数据导入到DataTable中
        /// </summary>
        /// <param name="fileName">文件路径</param>
        /// <param name="sheetName">excel工作薄sheet的名称</param>
        /// <param name="isFirstRowColumn">第一行是否是DataTable的列名，true是</param>
        /// <returns>返回的DataTable</returns>
        public static DataTable ExcelToDatatable(string fileName, string sheetName, bool isFirstRowColumn)
        {
            ISheet sheet = null;
            DataTable data = new DataTable();
            int startRow = 0;
            FileStream fs;
            IWorkbook workbook = null;
            int cellCount = 0;//列数
            int rowCount = 0;//行数
            try
            {
                fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                if (fileName.IndexOf(".xlsx") > 0) // 2007版本
                {
                    workbook = new XSSFWorkbook(fs);
                }
                else if (fileName.IndexOf(".xls") > 0) // 2003版本
                {
                    workbook = new HSSFWorkbook(fs);
                }
                if (sheetName != null)
                {
                    sheet = workbook.GetSheet(sheetName);//根据给定的sheet名称获取数据
                }
                else
                {
                    //也可以根据sheet编号来获取数据
                    sheet = workbook.GetSheetAt(0);//获取第几个sheet表（此处表示如果没有给定sheet名称，默认是第一个sheet表）  
                }
                if (sheet != null)
                {
                    IRow firstRow = sheet.GetRow(0);
                    cellCount = firstRow.LastCellNum; //第一行最后一个cell的编号 即总的列数
                    if (isFirstRowColumn)//如果第一行是标题行
                    {
                        for (int i = firstRow.FirstCellNum; i < cellCount; ++i)//第一行列数循环
                        {
                            DataColumn column = new DataColumn(firstRow.GetCell(i).StringCellValue);//获取标题
                            data.Columns.Add(column);//添加列
                        }
                        startRow = sheet.FirstRowNum + 1;//1（即第二行，第一行0从开始）
                    }
                    else
                    {
                        startRow = sheet.FirstRowNum;//0
                    }
                    //最后一行的标号
                    rowCount = sheet.LastRowNum;
                    for (int i = startRow; i <= rowCount; ++i)//循环遍历所有行
                    {
                        IRow row = sheet.GetRow(i);//第几行
                        if (row == null)
                        {
                            continue; //没有数据的行默认是null;
                        }
                        //将excel表每一行的数据添加到datatable的行中
                        DataRow dataRow = data.NewRow();
                        for (int j = row.FirstCellNum; j < cellCount; ++j)
                        {
                            if (row.GetCell(j) != null) //同理，没有数据的单元格都默认是null
                            {
                                dataRow[j] = row.GetCell(j).ToString();
                            }
                        }
                        data.Rows.Add(dataRow);
                    }
                }
                return data;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
                return null;
            }
        }
        #endregion


        //此处是将list集合写入excel表，Supply也是自己定义的类，每一个字段对应需要写入excel表的每一列的数据
        //一次最多能写65535行数据，超过需将list集合拆分，分多次写入
        #region 写入excel
        public static bool ListToExcel(List<Supply> list)
        {
            bool result = false;
            IWorkbook workbook = new HSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("Sheet1");//创建一个名称为Sheet0的表;
            IRow row = sheet.CreateRow(0);//（第一行写标题)
            row.CreateCell(0).SetCellValue("标题1");//第一列标题，以此类推
            row.CreateCell(1).SetCellValue("标题2");
            row.CreateCell(2).SetCellValue("标题3");
            int count = list.Count;//
            int max = 65535;//最大行数限制
            if (count < max)
            {
                //每一行依次写入
                for (int i = 0; i < list.Count; i++)
                {
                    row = sheet.CreateRow(i + 1);//i+1:从第二行开始写入(第一行可同理写标题)，i从第一行写入
                    row.CreateCell(0).SetCellValue(list[i].Value1);//第一列的值
                    row.CreateCell(1).SetCellValue(list[i].Value2);//第二列的值
                    row.CreateCell(2).SetCellValue(list[i].Value3);
                }
                //文件写入的位置
                using (FileStream fs = File.OpenWrite(@"C:\Users\20882\Desktop\结果.xls"))
                {
                    workbook.Write(fs);//向打开的这个xls文件中写入数据  
                    result = true;
                }
            }
            else
            {
                Console.WriteLine("超过行数限制！");
                result = false;
            }

            return result;
        }


        #endregion
        public class Supply
        {
            public string Value1 { get; set; }
            public string Value2 { get; set; }
            public string Value3 { get; set; }
        }
    }
}

