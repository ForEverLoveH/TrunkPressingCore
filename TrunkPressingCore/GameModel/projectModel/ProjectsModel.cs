using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TrunkPressingCore.GameModel
{
    /// <summary>
    /// 项目树形图结构
    /// </summary>
    class projectsModel
    {
        public string projectName { get; set; }
        public List<groupModel> Groups { get; set; }
    }
    class groupModel
    {
        public string GroupName { get; set; }
        public int IsAllTested { get; set; }
    }
}
