using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrunkPressingCore.GameConst;
using TrunkPressingCore.GameModel;
using TrunkPressingCore.SQLite;

namespace TrunkPressingCore.GameSystem
{
    public class UpLoadStudentGrade
    {
        /// <summary>
        /// 上传学生成绩的多线程的方式
        /// </summary>
        /// <param name="fsp"></param>
        /// <param name="helper"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public  string UploadStudentThreadFun(Object obj, SQLiteHelper helper, string projectName)
        {
            try
            {
                List<Dictionary<string, string>> Sucesslist = new List<Dictionary<string, string>>();
                List<Dictionary<string, string>> ErrorList = new List<Dictionary<string, string>>();
                Dictionary<string, string> localInfos = new Dictionary<string, string>();
                List<Dictionary<string, string>> fileInfos = helper.ExecuteReaderList("SELECT * FROM localInfos");
                foreach (var fileInfo in fileInfos)
                {
                    localInfos.Add(fileInfo["key"], fileInfo["value"]);

                }
                int.TryParse(localInfos["UploadUnit"], out int UploadUnit);
                string[] S = obj as string[];
                string name = string.Empty;
                string groupname = string.Empty;
                if (S.Length > 0)
                    name = S[0];
                if (S.Length > 1)
                    groupname = S[1];
                var st = helper.ExecuteReaderOne($"SELECT Id,Type,RoundCount,BestScoreMode,TestMethod," +
                     $"FloatType,TurnsNumber0,TurnsNumber1 FROM SportProjectInfos WHERE Name='{projectName}'");
                string sl = $"SELECT Id,ProjectId,Name FROM DbGroupInfos WHERE ProjectId='{st["Id"]}' ";
                if (!string.IsNullOrEmpty(groupname))
                {
                    sl += $"AND Name='{groupname}'";
                }
                List<Dictionary<string, string>> list = helper.ExecuteReaderList(sl);
                UploadResultsRequestParameter ress = new UploadResultsRequestParameter();
                ress.AdminUserName = localInfos["AdminUserName"];
                ress.TestManUserName = localInfos["TestManUserName"];
                ress.TestManPassword = localInfos["TestManPassword"];
                string code = localInfos["MachineCode"];
                if (code.IndexOf("_") != -1)
                {
                    code = code.Substring(code.IndexOf("_") + 1);
                }
                StringBuilder stringBuilder = new StringBuilder();
                StringBuilder builder = new StringBuilder();
                //按组上传
                foreach (var sqlRes in list)
                {
                    string sql = $"SELECT Id,GroupName,Name,IdNumber,SchoolName,GradeName,ClassNumber,State,Sex,BeginTime,FinalScore,uploadState FROM DbPersonInfos" +
                   $" WHERE ProjectId='{st["Id"]}' AND GroupName = '{st["Name"]}'";
                    var ls = helper.ExecuteReaderList(sql);
                    int rs = 0;
                    if (ls.Count > 0)
                    {
                        Dictionary<string, string> stu = ls[0];
                        int.TryParse(st["RoundCount"], out rs);
                        ress.MachineCode = code;
                    }
                    else
                    {
                        continue;
                    }
                    List<StudentData> studentData = new List<StudentData>();
                    Dictionary<string, string> map = new Dictionary<string, string>();
                    foreach (var row in ls)
                    {
                        // 没有参加考试的跳过
                        if (row["State"] == "0" && row["FinalScore"] == "-1") continue;
                        if (row["uploadState"] == "1" || row["uploadState"] == "-1") continue;
                        List<RoundsItem> roundsItems = new List<RoundsItem>();
                        List<Dictionary<string, string>> result = helper.ExecuteReaderList(
                      $"SELECT Id,CreateTime,RoundId,State,uploadState,Result FROM ResultInfos WHERE PersonId='{row["Id"]}' And IsRemoved=0 LIMIT {rs}");
                        #region 查询文件
                        //成绩根目录
                        Dictionary<string, string> dic_images = new Dictionary<string, string>();
                        Dictionary<string, string> dic_viedos = new Dictionary<string, string>();
                        Dictionary<string, string> dic_texts = new Dictionary<string, string>();
                        //string scoreRoot = Application.StartupPath + $"\\Scores\\{projectName}\\{stu["GroupName"]}\\";

                        #endregion
                        foreach (var item in result)
                        {
                            if (item["uploadState"] != "0") continue;
                            ///
                            DateTime.TryParse(item["CreateTime"], out DateTime dtBeginTime);
                            string dateStr = dtBeginTime.ToString("yyyyMMdd");
                            string GroupNo = $"{dateStr}_{row["GroupName"]}_{row["IdNumber"]}_{item["RoundId"]}";
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
                        if (roundsItems.Count == 0) continue;
                        StudentData student = new StudentData();
                        student.SchoolName = row["SchoolName"];
                        student.GradeName = row["GradeName"];
                        student.Name = row["Name"];
                        student.IdNumber = row["IdNumber"];
                        student.Rounds = roundsItems;
                        studentData.Add(student);
                        map.Add(row["IdNumber"], row["Id"]);
                    }
                    ress.studentDatas = studentData;
                    string jsonData = JsonConvert.SerializeObject(ress);
                    string url = localInfos["Platform"] + RequestUrl.UploadResults;
                    var httpUpLoad = new HttpUpload();
                    var datas = new List<FromDataItemModel>();
                    datas.Add(new FromDataItemModel()
                    {
                        key = "data",
                        value = jsonData
                    });
                    builder.AppendLine();
                    builder.AppendLine();
                    builder.AppendLine(jsonData);
                    //s上传学生成绩
                    string res = HttpUpload.PostFromData(url, datas);
                    UpLoadResult upLoadResult = JsonConvert.DeserializeObject<UpLoadResult>(res);
                    string errorStr = string.Empty;
                    List<Dictionary<string, int>> resList = upLoadResult.Results;

                    foreach (var its in studentData)
                    {
                        Dictionary<string, string> dic = new Dictionary<string, string>();
                        dic.Add("Id", map[its.IdNumber]);
                        dic.Add("IdNumber", its.IdNumber);
                        dic.Add("Name", its.Name);
                        dic.Add("uploadGroup", its.Rounds[0].GroupNo);
                        int val = 0;
                        resList.Find(a => a.TryGetValue(its.IdNumber, out val));
                        if (val == 1)
                        {
                            Sucesslist.Add(dic);
                        }
                        else if (val != 0)
                        {
                            errorStr = UpLoadResults.Match(val);
                            dic.Add("error", errorStr);
                            ErrorList.Add(dic);
                            stringBuilder.Append($"{sqlRes["Name"]}组 考号:{its.IdNumber} 姓名:{its.Name}上传失败,错误内容:{errorStr}");
                        }

                    }
                }
                LoggerHelper.Monitor(builder.ToString());
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"成功:{Sucesslist.Count},失败:{ErrorList.Count}");
                sb.AppendLine("****************error***********************");

                foreach (var item in ErrorList)
                {
                    sb.AppendLine($"考号:{item["IdNumber"]} 姓名:{item["Name"]} 错误:{item["error"]}");
                }
                sb.AppendLine("*****************error**********************");

                var sqls = helper.BeginTransaction();
                sb.AppendLine("*****************success**********************");
                foreach (var item in Sucesslist)
                {
                    string sql1 = $"UPDATE DbPersonInfos SET uploadState=1,uploadGroup='{item["uploadGroup"]}' WHERE Id={item["Id"]}";
                    helper.ExecuteNonQuery(sql1);
                    // 更新成绩上传状态
                    sql1 = $"UPDATE ResultInfos SET uploadState=1 WHERE PersonId={item["Id"]}";
                    helper.ExecuteNonQuery(sql1);
                    sb.AppendLine($"考号:{item["IdNumber"]} 姓名:{item["Name"]}");
                }
                helper.CommitTransAction(sqls);
                sb.AppendLine("*****************success**********************");
                string txtpath = Application.StartupPath + $"\\Log\\upload\\";
                if (!Directory.Exists(txtpath))
                {
                    Directory.CreateDirectory(txtpath);
                }
                if (Sucesslist.Count != 0 || ErrorList.Count != 0)
                {
                    txtpath = Path.Combine(txtpath, $"upload_{DateTime.Now.ToString("yyyyMMddHHmmss")}.txt");
                    File.WriteAllText(txtpath, sb.ToString());
                }
                string outpitMessage = stringBuilder.ToString();
                return outpitMessage;
            }
            catch (Exception ex)
            {
                LoggerHelper.Debug(ex);
                return ex.Message;
            }

        }
        /// <summary>
        ///  上传单人成绩
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="helper"></param>
        /// <param name="v"></param>
        /// <returns></returns>

        public  string UploadStudentByName(string projectName, SQLiteHelper helper, int nums = 100)
        {

            try
            {
                List<Dictionary<string, string>> successList = new List<Dictionary<string, string>>();
                List<Dictionary<string, string>> errorList = new List<Dictionary<string, string>>();
                Dictionary<string, string> localInfos = new Dictionary<string, string>();
                List<Dictionary<string, string>> list0 = helper.ExecuteReaderList("SELECT * FROM localInfos");
                foreach (var item in list0)
                {
                    localInfos.Add(item["key"], item["value"]);
                }
                //查询项目数据信息
                Dictionary<string, string> SportProjectDic = helper.ExecuteReaderOne($"SELECT Id,Type,RoundCount,BestScoreMode,TestMethod," +
                         $"FloatType,TurnsNumber0,TurnsNumber1 FROM SportProjectInfos WHERE Name='{projectName}'");
                UploadResultsRequestParameter urrp = new UploadResultsRequestParameter();
                urrp.AdminUserName = localInfos["AdminUserName"];
                urrp.TestManUserName = localInfos["TestManUserName"];
                urrp.TestManPassword = localInfos["TestManPassword"];
                string MachineCode = localInfos["MachineCode"];
                if (MachineCode.IndexOf('_') != -1)
                {
                    MachineCode = MachineCode.Substring(MachineCode.IndexOf('_') + 1);
                }
                //轮次
                int turn = 0;
                if (SportProjectDic.Count > 0)
                {
                    int.TryParse(SportProjectDic["RoundCount"], out turn);
                    urrp.MachineCode = MachineCode;
                }
                else return "未找到项目";

                StringBuilder messageSb = new StringBuilder();
                StringBuilder logWirte = new StringBuilder();
                string sql0 = $"SELECT Id,GroupName,Name,IdNumber,SchoolName,GradeName,ClassNumber,State,Sex,BeginTime,FinalScore,uploadState FROM DbPersonInfos" +
                            $" WHERE ProjectId='{SportProjectDic["Id"]}' and FinalScore=1 ";
                List<Dictionary<string, string>> sqlStuResults = helper.ExecuteReaderList(sql0);
                //?  计次
                int numStep = 0;
                int maxNums = sqlStuResults.Count;
                List<StudentData> sudentsItems = new List<StudentData>();
                //IdNumber 对应Id
                Dictionary<string, string> map = new Dictionary<string, string>();
                //遍历学生
                foreach (var stu in sqlStuResults)
                {
                    numStep++;
                    //未参加考试的跳过
                    if (stu["State"] == "0" && stu["FinalScore"] == "-1")
                    {
                        continue;
                    }
                    /* //已上传的跳过
                     if (stu["uploadState"] == "1" || stu["uploadState"] == "-1")
                     {
                         continue;
                     }*/
                    List<RoundsItem> roundsItems = new List<RoundsItem>();
                    ///成绩
                    List<Dictionary<string, string>> resultScoreList1 = helper.ExecuteReaderList(
                        $"SELECT Id,CreateTime,RoundId,State,uploadState,Result FROM ResultInfos WHERE PersonId='{stu["Id"]}' And IsRemoved=0 LIMIT {turn}");
                    #region 查询文件
                    //成绩根目录
                    Dictionary<string, string> dic_images = new Dictionary<string, string>();
                    Dictionary<string, string> dic_viedos = new Dictionary<string, string>();
                    Dictionary<string, string> dic_texts = new Dictionary<string, string>();
                    //string scoreRoot = Application.StartupPath + $"\\Scores\\{projectName}\\{stu["GroupName"]}\\";
                    #endregion

                    //? 遍历成绩
                    foreach (var item in resultScoreList1)
                    {
                        if (item["uploadState"] != "0") continue;
                        ///
                        DateTime.TryParse(item["CreateTime"], out DateTime dtBeginTime);
                        string dateStr = dtBeginTime.ToString("yyyyMMdd");
                        string GroupNo = $"{stu["GroupName"]}_{stu["IdNumber"]}_{item["RoundId"]}";
                        //轮次成绩
                        RoundsItem rdi = new RoundsItem();
                        rdi.RoundId = Convert.ToInt32(item["RoundId"]);
                        rdi.State = ResultState.ResultState2Str(item["State"]);
                        rdi.Time = item["CreateTime"];
                        double.TryParse(item["Result"], out double score);
                        rdi.Result = score;
                        //string.Format("{0:D2}:{1:D2}", ts.Minutes, ts.Seconds);
                        rdi.GroupNo = GroupNo;
                        rdi.Text = dic_texts;
                        rdi.Images = dic_images;
                        rdi.Videos = dic_viedos;
                        roundsItems.Add(rdi);
                    }
                    if (roundsItems.Count == 0) continue;

                    StudentData ssi = new StudentData();
                    ssi.SchoolName = stu["SchoolName"];
                    ssi.GradeName = stu["GradeName"];
                    ssi.ClassNumber = stu["ClassNumber"];
                    ssi.Name = stu["Name"];
                    ssi.IdNumber = stu["IdNumber"];
                    ssi.Rounds = roundsItems;
                    sudentsItems.Add(ssi);
                    map.Add(stu["IdNumber"], stu["Id"]);

                    //超过 限制数量就上传或者最后一人
                    if (sudentsItems.Count >= nums || numStep >= maxNums)
                    {
                        urrp.studentDatas = sudentsItems;
                        //序列化json
                        string JsonStr = Newtonsoft.Json.JsonConvert.SerializeObject(urrp);
                        string url = localInfos["Platform"] + RequestUrl.UploadResults;
                        var httpUpload = new HttpUpload();
                        var formDatas = new List<FromDataItemModel>();
                        //添加其他字段
                        formDatas.Add(new FromDataItemModel()
                        {
                            key = "data",
                            value = JsonStr
                        });
                        logWirte.AppendLine();
                        logWirte.AppendLine();
                        logWirte.AppendLine(JsonStr);
                        //上传学生成绩
                        string result = HttpUpload.PostFromData(url, formDatas);
                        var upload_Result = Newtonsoft.Json.JsonConvert.DeserializeObject<UpLoadResult>(result);
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
                                messageSb.AppendLine($"考号:{item.IdNumber} 姓名:{item.Name}上传失败,错误内容:{errorStr}");
                            }
                        }
                        map.Clear();
                        sudentsItems.Clear();
                    }
                }
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"成功:{successList.Count},失败:{errorList.Count}");
                sb.AppendLine("****************error***********************");

                foreach (var item in errorList)
                {
                    sb.AppendLine($"考号:{item["IdNumber"]} 姓名:{item["Name"]} 错误:{item["error"]}");
                }
                sb.AppendLine("*****************error**********************");


                System.Data.SQLite.SQLiteTransaction sQLiteTransaction = helper.BeginTransaction();
                sb.AppendLine("******************success*********************");
                foreach (var item in successList)
                {
                    string sql1 = $"UPDATE DbPersonInfos SET uploadState=1,uploadGroup='{item["uploadGroup"]}' WHERE Id={item["Id"]}";
                    helper.ExecuteNonQuery(sql1);
                    //更新成绩上传状态
                    sql1 = $"UPDATE ResultInfos SET uploadState=1 WHERE PersonId={item["Id"]}";
                    helper.ExecuteNonQuery(sql1);
                    sb.AppendLine($"考号:{item["IdNumber"]} 姓名:{item["Name"]}");
                }
                helper.CommitTransAction(sQLiteTransaction);
                sb.AppendLine("*******************success********************");

                string txtpath = Application.StartupPath + $"\\Log\\upload\\";
                if (!Directory.Exists(txtpath))
                {
                    Directory.CreateDirectory(txtpath);
                }
                if (successList.Count != 0 || errorList.Count != 0)
                {
                    txtpath = Path.Combine(txtpath, $"upload_{DateTime.Now.ToString("yyyyMMddHHmmss")}.txt");
                    File.WriteAllText(txtpath, sb.ToString());
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
    }
}
