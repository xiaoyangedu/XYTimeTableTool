using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSKernel.Presentation.Models.Enums
{
    /// <summary>
    /// 排课任务状态枚举
    /// </summary>
    public enum MissionStateEnum : short
    {
        /// <summary>
        /// 创建中（程序用，不会固化到数据库）
        /// </summary>
        [Description("创建中")]
        Creating = -1,
        /// <summary>
        /// 未知（程序用，不会固化到数据库）
        /// </summary>
        [Description("未知")]
        Unknown = 0,
        /// <summary>
        /// 排队中
        /// </summary>
        [Description("排队中")]
        Waiting = 1,
        /// <summary>
        /// 已取消
        /// </summary>
        [Description("已取消")]
        Cancelled,
        /// <summary>
        /// 排课中
        /// </summary>
        [Description("排课中")]
        Started = 10,
        /// <summary>
        /// 重试中
        /// </summary>
        [Description("重试中")]
        Reloading,
        /// <summary>
        /// 已停止
        /// </summary>
        [Description("已停止")]
        Stopped,
        /// <summary>
        /// 已完成
        /// </summary>
        [Description("已完成")]
        Completed = 20,
        /// <summary>
        /// 已失败
        /// </summary>
        [Description("已失败")]
        Failed,
        /// <summary>
        /// 已中断
        /// </summary>
        [Description("已中断")]
        Aborted,
        /// <summary>
        /// 已过期：表示任务重启或等待超过系统时间，排队超时，非排课超时
        /// </summary>
        [Description("已过期")]
        Expired
    }
}
