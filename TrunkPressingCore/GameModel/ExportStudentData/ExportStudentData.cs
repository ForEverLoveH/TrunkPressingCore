﻿using MiniExcelLibs.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrunkPressingCore.GameModel
{
    public class ExportStudentData
    {
        [ExcelColumnName("序号")]
        public int Id { get; set; }
        [ExcelColumnName("学校")]
        public string School { get; set; }
        [ExcelColumnName("年级")]
        public string GradeName { get; set; }
        [ExcelColumnName("班级")]
        public string ClassName { get; set; }
        [ExcelColumnName("姓名")]
        public string Name { get; set; }
        [ExcelColumnName("性别")]
        public string Sex { get; set; }
        [ExcelColumnName("准考证号")]
        public string IdNumber { get; set; }
        [ExcelColumnName("组别名称")]
        public string GroupName { get; set; }
    }
    public class InputChipData
    {
        [ExcelColumnName("序号")]
        public int Id { get; set; }
        [ExcelColumnName("芯片标签号码")]
        public string ChipLabel { get; set; }
        [ExcelColumnName("芯片内部编号")]
        public string ChipNO { get; set; }
        [ExcelColumnName("组号")]
        public string GroupName { get; set; }

    }
}
