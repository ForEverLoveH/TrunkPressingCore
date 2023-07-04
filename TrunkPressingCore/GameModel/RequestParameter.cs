using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrunkPressingCore.GameSystem
{
    public class RequestParameter
    {
        /// <summary>
        /// 注册软件生成的机器码
        /// </summary>
        public string MachineCode { get; set; }
        /// <summary>
        /// 管理员账号
        /// </summary>
        public string AdminUserName { get; set; }
        /// <summary>
        /// 裁判员账号
        /// </summary>
        public string TestManUserName { get; set; }
        /// <summary>
        /// 裁判员密码
        /// </summary>
        public string TestManPassword { get; set; }
        /// <summary>
        /// 考试id
        /// </summary>
        public string ExamId { get; set; }

        /// <summary>
        /// 组id
        /// </summary>
        public string GroupId { get; set; }

        /// <summary>
        /// 准考证号
        /// </summary>
        public string IdNumber { get; set; }

        /// <summary>
        /// 要下载的组数
        /// </summary>
        public string GroupNums { get; set; }
    }
    //学校列表
    public class GetExamList
    {
        public List<GetExamListResults> Results { get; set; }

        public String Error { get; set; }

    }
    public class GetExamListResults
    {
        public String exam_id;

        public String title;
    }
    // 机器码
    public class GetMachineCodeList
    {
        public List<GetMachineCodeListResults> Results;

        public String Error;
    }
    public class GetMachineCodeListResults
    {
        public String title;

        public String MachineCode;
    }

}
