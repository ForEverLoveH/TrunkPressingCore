namespace TrunkPressingCore
{
    public class RequestUrl
    {
        /// <summary>
        /// 设备端 获取体测学校列表
        /// </summary>
        public static string GetExamListUrl = "api/GetExamList/";

        //GetExamList
        /// <summary>
        /// 设备端 获取机器码
        /// </summary>
        public static string GetMachineCodeListUrl = "api/GetMachineCodeList/";

        /// <summary>
        /// 设备端 获取组列表
        /// </summary>
        public static string GetGroupsUrl = "api/GetGroup/";


        /// <summary>
        /// 设备端 获取学生列表
        /// </summary>
        public static string FetchStudentsUrl = "api/FetchStudents/";

        /// <summary>
        /// 设备端 按组数获取学生
        /// </summary>
        public static string GetGroupStudentUrl = "api/GetGroupStudent/";

        /// <summary>
        /// 上报成绩接口
        /// </summary>

        public static string UploadResults = "api/UploadResults/";
    }
    /// <summary>
    /// 表格显示标题
    /// </summary>
    class DataGridViewModel
    {
        public string ID { get; set; }
        public string GroupName { get; set; }
        public string School { get; set; }
        public string Grade { get; set; }
        public string Class { get; set; }
        public string Name { get; set; }
        public int Sex { get; set; }
        public string IdNumber { get; set; }
    }
}