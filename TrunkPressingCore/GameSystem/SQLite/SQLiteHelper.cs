using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrunkPressingCore. SQLite
{
    public  class SQLiteHelper
    {
        public static  Dictionary<string, SQLiteHelper> SQLiteHelperTable = new Dictionary<string, SQLiteHelper>();
        private  static SQLiteConnection liteConnection;
        /// <summary>
        /// 数据库地址
        /// </summary>
        public string DataSourcePath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        public SQLiteHelper(string filename = null)
        {
            if (string.IsNullOrEmpty(filename))
            {
                filename = GameConst.GameConst.DbPath;
            }
            DataSourcePath  = filename;
            GetSQLiteConnection();
        }
        /// <summary>
        ///  创建数据库
        /// </summary>
        public void CreateSQLiteDataBase()
        {
            string psth = Path.GetDirectoryName(DataSourcePath);
            if ((!string.IsNullOrWhiteSpace(psth)) && (!Directory.Exists(psth)))
            {
                Directory.CreateDirectory(psth);
            }
            if (!File.Exists(DataSourcePath))
            {
                SQLiteConnection.CreateFile(DataSourcePath);
            }
        }
        /// <summary>
        ///  开启事务
        /// </summary>
        /// <returns></returns>
        public SQLiteTransaction BeginTransaction()
        {
            
            try
            {
                 var sqlTransaction = liteConnection.BeginTransaction();
                 return sqlTransaction;
            }
            catch(Exception ex)
            {
                return  null;
            }
        }
        /// <summary>
        ///  提交事务
        /// </summary>
        /// <param name="liteTransaction"></param>
        public void  CommitTransAction(SQLiteTransaction liteTransaction)
        {
           
            try
            {
                if (liteTransaction != null)
                {
                    liteTransaction.Commit();
                }
            }
            catch(Exception ex)
            {
                LoggerHelper.Debug(ex);
            }
        }
        /// <summary>
        /// 准备操作命令参数
        /// </summary>
        /// <param name="command"></param>
        /// <param name="connection"></param>
        /// <param name="sqlText"></param>
        /// <param name="data"></param>
        public static void PrepareCommand(SQLiteCommand command, SQLiteConnection connection,string sqlText, Dictionary<String, String> data)
        {
            if (connection.State != ConnectionState.Open)
                connection.Open();
            command.Parameters.Clear();
            command.Connection=connection;
            command.CommandText=sqlText;
            command.CommandType = System.Data.CommandType.Text;
            command.CommandTimeout = 30;
            if(data!=null&&data.Count>=1)
            {
                foreach(KeyValuePair<String, String> val in data)
                {
                    command.Parameters.AddWithValue(val.Key,val.Value);
                }
            }
        }
        /// <summary>
        /// 查询返回dataset
        /// 
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public DataSet ExecuteDataSet(string cmdText, Dictionary<string, string> data = null)
        {
            try
            {
                SureConnectSql();
                DataSet ds = new DataSet();
                var command = new SQLiteCommand();
                PrepareCommand(command, liteConnection, cmdText, data);
                var da= new SQLiteDataAdapter(command);
                da.Fill(ds);
                return ds;
            }
            catch(Exception ex)
            {
                LoggerHelper .Debug(ex);
                return null;
            }
        }
        /// <summary>
        ///  查询数据返回datatable对象
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public  DataTable ExecuteDataTable(string  cmdText ,Dictionary<string, string> data)
        {
            try
            {
                SureConnectSql();
                var dt = new DataTable();
                var command = new SQLiteCommand();
                PrepareCommand(command,liteConnection, cmdText, data);
                SQLiteDataReader reader = command.ExecuteReader();
                dt.Load(reader);
                return dt;
            }
            catch(Exception ex)
            {
                LoggerHelper.Debug(ex);
                return null;
            }
        }
        /// <summary>
        /// 返回一行数据
        /// </summary>
        /// <param name="cmdText">Sql命令文本</param>
        /// <param name="data">参数数组</param>
        /// <returns>DataRow</returns>
        public DataRow ExecuteDataRow(string cmdText, Dictionary<string, string> data = null)
        {
            DataSet ds = ExecuteDataSet(cmdText, data);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0].Rows[0];
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(string cmdText, Dictionary<string, string> data = null)
        {
            try
            {
                LoggerHelper.Fatal($"数据库操作:{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}\n");
                LoggerHelper.Fatal(cmdText + "\n");
                int result = 0;
                var command = new SQLiteCommand();
                PrepareCommand(command, liteConnection, cmdText, data);
                result = command.ExecuteNonQuery();
                return result;
            }
            catch(Exception e)
            {
                LoggerHelper.Debug(e);
                return 0;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public SQLiteDataReader ExecuteReader(string cmdText, Dictionary<string, string> data = null)
        {
            var command = new SQLiteCommand();
            SureConnectSql();
            try
            {
                PrepareCommand(command, liteConnection, cmdText, data);
                SQLiteDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection);
                return reader;
            }
            catch (Exception e)
            {
                command.Dispose();
                throw;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmdText"></param>
        /// <returns></returns>
        public List<Dictionary<string, string>> ExecuteReaderList(string cmdText)
        {
            try
            {
                List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
                var ds = ExecuteReader(cmdText);
                int columcount = ds.FieldCount;
                while (ds.Read())
                {
                    Dictionary<string, string> dic = new Dictionary<string, string>();
                    for (int i = 0; i < columcount; i++)
                    {
                        object obj = ds.GetValue(i);
                        if (obj == null)
                        {
                            dic.Add(ds.GetName(i), "");
                        }
                        else
                        {
                            dic.Add(ds.GetName(i), obj.ToString());
                        }
                    }
                    list.Add(dic);
                }
                return list;
            }
            catch(Exception e)
            {
                LoggerHelper.Debug(e);
                return null;
            }
        }
        /// <summary>
        /// 返回结果集中的第一行第一列，忽略其他行或列
        /// </summary>
        /// <param name="cmdText">Sql命令文本</param>
        /// <param name="data">传入的参数</param>
        /// <returns>object</returns>
        public object ExecuteScalar(string cmdText, Dictionary<string, string> data = null)
        {
            var cmd = new SQLiteCommand();
            PrepareCommand(cmd,  liteConnection, cmdText, data);
            return cmd.ExecuteScalar();
        }
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="recordCount">总记录数</param>
        /// <param name="pageIndex">页牵引</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="cmdText">Sql命令文本</param>
        /// <param name="countText">查询总记录数的Sql文本</param>
        /// <param name="data">命令参数</param>
        /// <returns>DataSet</returns>
        public DataSet ExecutePager(ref int recordCount, int pageIndex, int pageSize, string cmdText, string countText, Dictionary<string, string> data = null)
        {
            if (recordCount < 0)
                recordCount = int.Parse(ExecuteScalar(countText, data).ToString());
            var ds = new DataSet();
            var command = new SQLiteCommand();
            PrepareCommand(command, liteConnection, cmdText, data);
            var da = new SQLiteDataAdapter(command);
            da.Fill(ds, (pageIndex - 1) * pageSize, pageSize, "result");
            return ds;
        }
        /// <summary>
        /// 重新组织数据库：VACUUM 将会从头重新组织数据库
        /// </summary>
        public void ResetDataBass(SQLiteConnection conn)
        {
            var cmd = new SQLiteCommand();
            if (conn.State != ConnectionState.Open)
                conn.Open();
            cmd.Parameters.Clear();
            cmd.Connection = conn;
            cmd.CommandText = "vacuum";
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 30;
            cmd.ExecuteNonQuery();
        }
        /// <summary>
        /// 关闭数据库
        /// </summary>
        public void CloseDb()
        {
            if (liteConnection.State == ConnectionState.Open)
                liteConnection.Close();
            liteConnection= null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmdText"></param>
        /// <returns></returns>
        public Dictionary<string, string> ExecuteReaderOne(string cmdText)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            var ds = ExecuteReader(cmdText);
            int columcount = ds.FieldCount;
            while (ds.Read())
            {
                for (int i = 0; i < columcount; i++)
                {
                    object obj = ds.GetValue(i);
                    if (obj == null)
                    {
                        dic.Add(ds.GetName(i), "");
                    }
                    else
                    {
                        dic.Add(ds.GetName(i), obj.ToString());
                    }
                }
                break;
            }
            return dic;
        }
        /// <summary>
        ///  备份数据库
        /// </summary>
        public   void ExportSQLiteDb()
        {
            string path  = Path.GetFileNameWithoutExtension(DataSourcePath);
            File.Copy( DataSourcePath, $"./db/backup/{path}_{DateTime.Now.ToString("yyyyMMddHHmmss")}.db");

        }
        /// <summary>
        ///  初始化数据库 
        /// </summary>
        public void InitSQLiteDB()
        {
            var dss = ExecuteReaderList("SELECT name,seq FROM sqlite_sequence");
            var  trs  =BeginTransaction();
            foreach(var  ds in dss)
            {
                string name = ds["name"];
                if (name == "localInfos")
                {
                    continue;
                }
                ExecuteNonQuery($"DELETE FROM {name}");
                ExecuteNonQuery($"UPDATE sqlite_sequence SET seq=0 where name='{name}'");
            }
            CommitTransAction(trs);
        }
        /// <summary>
        /// 连接数据库
        /// </summary>
        private void GetSQLiteConnection()
        {
            string connStr = string.Format("Data Source={0};Version=3;Max Pool Size=10;Journal Mode=Off;", DataSourcePath);
            liteConnection= new SQLiteConnection(connStr);
        }
        /// <summary>
        ///  确保连接
        /// </summary>
        private  void SureConnectSql()
        {
            if (!string.IsNullOrEmpty(DataSourcePath))
            {
                if (liteConnection.State != System.Data.ConnectionState.Open)
                {
                    GetSQLiteConnection();
                }
            }
            
        }
    }
}
