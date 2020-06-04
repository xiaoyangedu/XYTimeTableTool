using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace OSKernel.Presentation.Models.Result
{
    /// <summary>
    /// 结果界面
    /// </summary>
    public class UIResult : GalaSoft.MvvmLight.ObservableObject
    {
        /// <summary>
        /// 序号
        /// </summary>
        public int NO { get; set; }

        /// <summary>
        /// 本地方案ID
        /// </summary>
        public string LocalID { get; set; }

        /// <summary>
        /// 任务ID
        /// </summary>
        public long TaskID { get; set; }

        /// <summary>
        /// 方案名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 是否已上传
        /// </summary>
        public bool IsUploaded { get; set; }

        /// <summary>
        /// 是否应用
        /// </summary>
        public bool IsUsed { get; set; }

        /// <summary>
        /// 显示常规操作
        /// </summary>
        public bool ShowNormalOption
        {
            get
            {
                return IsUsed;
            }
            set { }
        }

        /// <summary>
        /// 显示结果预览
        /// </summary>
        public bool ShowPrecharge
        {
            get
            {
                return !IsUsed;
            }
            set
            {

            }
        }

        public void RaiseChanged()
        {
            this.RaisePropertyChanged(() => ShowNormalOption);
            this.RaisePropertyChanged(() => ShowPrecharge);
        }
    }
}
