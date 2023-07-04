using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrunkPressingCore .GameConst
{
    public  class GameConst
    {
        /// <summary>
        /// 数据库路径
        /// </summary>
        //public static string DbPath = @"./db/SportsDB.db";
        public static string DbPath = @"./db/RunSportsDB.db";
    }
    public class ResultState
    {
        public static int NoTest = 0;//未测试
        public static int Test = 1;//已测试
        public static int Withdrawal = 2;//中退
        public static int MissTest = 3;//缺考
        public static int Foul = 4;//犯规 
        public static int Waiver = 5;//弃权 
        public static string ResultState2Str(int state)
        {
            switch (state)
            {
                case 0:
                    return "未测试";
                case 1:
                    return "已测试";
                case 2:
                    return "中退";
                case 3:
                    return "缺考";
                case 4:
                    return "犯规";
                case 5:
                    return "弃权";
                default:
                    return "";
            }
        }
        public static string ResultState2Str(string state0)
        {
            int.TryParse(state0, out int state);
            return ResultState2Str(state);

        }

        public static int ResultState2Int(string state)
        {
            switch (state)
            {
                case "未测试":
                    return ResultState.NoTest;
                case "已测试":
                    return ResultState.Test;
                case "中退":
                    return ResultState.Withdrawal;
                case "缺考":
                    return ResultState.MissTest;
                case "犯规":
                    return ResultState.Foul;
                case "弃权":
                    return ResultState.Waiver;
                default:
                    return 0;
            }
        }
    }
        public class ProjectState
        {
            /*立定跳远
            投掷实心球
            坐位体前屈
            投掷铅球*/
            public static int Type1 = 0;//立定跳远
            public static int Type2 = 1;//投掷实心球
            public static int Type3 = 2;//坐位体前屈
            public static int Type4 = 3;//投掷铅球

            public static int ProjectStateType2Int(string state)
            {
                switch (state)
                {
                    case "立定跳远":
                        return Type1;
                    case "投掷实心球":
                        return Type2;
                    case "坐位体前屈":
                        return Type3;
                    case "投掷铅球":
                        return Type4;
                    default:
                        return 0;
                }
            }
            public static string ProjectState2Str(int state)
            {
                switch (state)
                {
                    case 0:
                        return "立定跳远";
                    case 1:
                        return "投掷实心球";
                    case 2:
                        return "坐位体前屈";
                    case 3:
                        return "投掷铅球";
                    default:
                        return "立定跳远";
                }
            }
            public static string ProjectStatee2Str(string state0)
            {
                int.TryParse(state0, out int state);
                return ProjectState2Str(state);
            }

        }

        public class TestModeState
        {
            public static int TestMode1 = 0;//自动下一位
            public static int TestMode2 = 1;//自动下一轮
            /// <summary>
            /// 测试模式
            /// </summary>
            /// <param name="state"></param>
            /// <returns></returns>
            public static int TestModeStateType2Int(string state)
            {
                switch (state)
                {
                    case "自动下一位":
                        return TestMode1;
                    case "自动下一轮":
                        return TestMode2;
                    default:
                        return -1;
                }
            }
        }

        public class BestScoreModeState
        {
            public static int BestScoreMode1 = 0;//自动下一位
            public static int BestScoreMode2 = 1;//自动下一轮
            /// <summary>
            /// 最好成绩取值
            /// </summary>
            /// <param name="state"></param>
            /// <returns></returns>
            public static int BestScoreModeStateType2Int(string state)
            {
                switch (state)
                {
                    case "数值最大最优":
                        return BestScoreMode1;
                    case "数值最小最优":
                        return BestScoreMode2;

                    default:
                        return -1;
                }
            }
        }


}


