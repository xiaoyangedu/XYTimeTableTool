namespace OSKernel.Presentation.Arranging.Mixed.Pattern.Data
{
    public class DataValidationResultInfo
    {
        /// <summary>
        /// 1xx 基本数据错误
        /// 2xx 批量规则错误
        /// 3xx 高级规则错误
        /// </summary>
        public string ErrorCode { get; set; }

        public string Description { get; set; }
    }

    public sealed class CaseError
    {
        private readonly string name;

        /// <summary>
        /// 基本数据模型为空!
        /// </summary>
        public static readonly CaseError XYErr100 = new CaseError("100");

        /// <summary>
        /// 课位信息
        /// </summary>
        public static readonly CaseError XYErr101 = new CaseError("101");

        /// <summary>
        /// 课程信息
        /// </summary>
        public static readonly CaseError XYErr102 = new CaseError("102");

        /// <summary>
        /// 教师信息
        /// </summary>
        public static readonly CaseError XYErr103 = new CaseError("103");

        /// <summary>
        /// 学生信息
        /// </summary>
        public static readonly CaseError XYErr104 = new CaseError("104");

        /// <summary>
        /// 课时信息
        /// </summary>
        public static readonly CaseError XYErr105 = new CaseError("105");

        /// <summary>
        /// 班额信息
        /// </summary>
        public static readonly CaseError XYErr106 = new CaseError("106");

        private CaseError(string name)
        {
            this.name = name;
        }

        public override string ToString()
        {
            return name;
        }
    }

    public sealed class RuleError
    {
        private readonly string name;

        /// <summary>
        /// 课时分散
        /// </summary>
        public static readonly RuleError XYErr201 = new RuleError("201");
        /// <summary>
        /// 教案齐头
        /// </summary>
        public static readonly RuleError XYErr202 = new RuleError("202");
        /// <summary>
        /// 教案平头
        /// </summary>
        public static readonly RuleError XYErr203 = new RuleError("203");
        /// <summary>
        /// 连排齐头
        /// </summary>
        public static readonly RuleError XYErr204 = new RuleError("204");
        /// <summary>
        /// 教师排课时间
        /// </summary>
        public static readonly RuleError XYErr205 = new RuleError("205");
        /// <summary>
        /// 教师每天最大课时数
        /// </summary>
        public static readonly RuleError XYErr206 = new RuleError("206");
        /// <summary>
        /// 教师每周最大工作天数
        /// </summary>
        public static readonly RuleError XYErr207 = new RuleError("207");
        /// <summary>
        /// 教师每天最大课时间隔
        /// </summary>
        public static readonly RuleError XYErr208 = new RuleError("208");
        /// <summary>
        /// 教师上下午不连排
        /// </summary>
        public static readonly RuleError XYErr209 = new RuleError("209");
        /// <summary>
        /// 教师半天上课
        /// </summary>
        public static readonly RuleError XYErr210 = new RuleError("210");
        /// <summary>
        /// 师徒跟随
        /// </summary>
        public static readonly RuleError XYErr211 = new RuleError("211");
        /// <summary>
        /// 同时开课
        /// </summary>
        public static readonly RuleError XYErr212 = new RuleError("212");
        /// <summary>
        /// 课程互斥
        /// </summary>
        public static readonly RuleError XYErr213 = new RuleError("213");
        /// <summary>
        /// 合班上课
        /// </summary>
        public static readonly RuleError XYErr214 = new RuleError("214");
        /// <summary>
        /// 上下午课时限制
        /// </summary>
        public static readonly RuleError XYErr215 = new RuleError("215");
        /// <summary>
        /// 单双周
        /// </summary>
        public static readonly RuleError XYErr216 = new RuleError("216");
        /// <summary>
        /// 课程连排
        /// </summary>
        public static readonly RuleError XYErr217 = new RuleError("217");
        /// <summary>
        /// 课程排课时间
        /// </summary>
        public static readonly RuleError XYErr218 = new RuleError("218");
        /// <summary>
        /// 同时开课限制
        /// </summary>
        public static readonly RuleError XYErr219 = new RuleError("219");
        /// <summary>
        /// 主要课程在特定课位占比
        /// </summary>
        public static readonly RuleError XYErr220 = new RuleError("220");
        /// <summary>
        /// 次要课程回避的课位
        /// </summary>
        public static readonly RuleError XYErr221 = new RuleError("221");
        /// <summary>
        /// 教师在特殊课位上的占比控制
        /// </summary>
        public static readonly RuleError XYErr222 = new RuleError("222");
        /// <summary>
        /// 每个班级第一节不重复
        /// </summary>
        public static readonly RuleError XYErr223 = new RuleError("223");
        /// <summary>
        /// 课表锁定
        /// </summary>
        public static readonly RuleError XYErr224 = new RuleError("224");

        private RuleError(string name)
        {
            this.name = name;
        }

        public override string ToString()
        {
            return name;
        }
    }

    public sealed class AlgoRuleError
    {
        private readonly string name;

        /// <summary>
        /// 单个课时有一个优先开始时间
        /// </summary>
        public static readonly AlgoRuleError XYErr301 = new AlgoRuleError("301");
        /// <summary>
        /// 单个课时有多个优先开始时间
        /// </summary>
        public static readonly AlgoRuleError XYErr302 = new AlgoRuleError("302");
        /// <summary>
        /// 单个课时有多个优先课位
        /// </summary>
        public static readonly AlgoRuleError XYErr303 = new AlgoRuleError("303");
        /// <summary>
        /// 在选定课位中设置多个课时的最大同时开课数量
        /// </summary>
        public static readonly AlgoRuleError XYErr304 = new AlgoRuleError("304");
        /// <summary>
        /// 多个课时不同时开课
        /// </summary>
        public static readonly AlgoRuleError XYErr305 = new AlgoRuleError("305");
        /// <summary>
        /// 多个课时在选定课位中占用的最大数量
        /// </summary>
        public static readonly AlgoRuleError XYErr306 = new AlgoRuleError("306");
        /// <summary>
        /// 多个课时有多个优先开始时间
        /// </summary>
        public static readonly AlgoRuleError XYErr307 = new AlgoRuleError("307");
        /// <summary>
        /// 多个课时有多个优先课位
        /// </summary>
        public static readonly AlgoRuleError XYErr308 = new AlgoRuleError("308");
        /// <summary>
        /// 多个课时有相同的开始日期（日期）
        /// </summary>
        public static readonly AlgoRuleError XYErr309 = new AlgoRuleError("309");
        /// <summary>
        /// 多个课时有相同的开始课位（时间）
        /// </summary>
        public static readonly AlgoRuleError XYErr310 = new AlgoRuleError("310");
        /// <summary>
        /// 多个课时有相同的开始时间（日期+时间）
        /// </summary>
        public static readonly AlgoRuleError XYErr311 = new AlgoRuleError("311");
        /// <summary>
        /// 多个课时之间的最大间隔天数
        /// </summary>
        public static readonly AlgoRuleError XYErr312 = new AlgoRuleError("312");
        /// <summary>
        /// 多个课时间最小课程间隔天数
        /// </summary>
        public static readonly AlgoRuleError XYErr313 = new AlgoRuleError("313");
        /// <summary>
        /// 教师每周最大工作天数
        /// </summary>
        public static readonly AlgoRuleError XYErr314 = new AlgoRuleError("314");
        /// <summary>
        /// 教师每天最大课程间隔
        /// </summary>
        public static readonly AlgoRuleError XYErr315 = new AlgoRuleError("315");
        /// <summary>
        /// 教师每天最大课时数
        /// </summary>
        public static readonly AlgoRuleError XYErr316 = new AlgoRuleError("316");
        /// <summary>
        /// 教师的不可用时间
        /// </summary>
        public static readonly AlgoRuleError XYErr317 = new AlgoRuleError("317");
        /// <summary>
        /// 所有教师每周最大工作天数
        /// </summary>
        public static readonly AlgoRuleError XYErr318 = new AlgoRuleError("318");
        /// <summary>
        /// 所有教师每天最大课程间隔
        /// </summary>
        public static readonly AlgoRuleError XYErr319 = new AlgoRuleError("319");
        /// <summary>
        /// 所有教师每天最大课时数
        /// </summary>
        public static readonly AlgoRuleError XYErr320 = new AlgoRuleError("320");
        /// <summary>
        /// 给3个课时分组
        /// </summary>
        public static readonly AlgoRuleError XYErr321 = new AlgoRuleError("321");
        /// <summary>
        /// 对2个课时设置连排
        /// </summary>
        public static readonly AlgoRuleError XYErr322 = new AlgoRuleError("322");
        /// <summary>
        /// 给2个课时分组
        /// </summary>
        public static readonly AlgoRuleError XYErr323 = new AlgoRuleError("323");
        /// <summary>
        /// 对2个课时排序
        /// </summary>
        public static readonly AlgoRuleError XYErr324 = new AlgoRuleError("324");

        private AlgoRuleError(string name)
        {
            this.name = name;
        }

        public override string ToString()
        {
            return name;
        }
    }

    public static class ValidationUtility
    {
        public static string GetTimeSlotInfo(XYKernel.OS.Common.Models.DayPeriodModel dayPeriod)
        {
            string dayPeriodString = string.Empty;

            if (dayPeriod != null)
            {
                dayPeriodString = $"Day={dayPeriod.Day},Period={dayPeriod.Period},PeriodName={dayPeriod.PeriodName ?? string.Empty}";
            }

            return dayPeriodString;
        }
    }
}
