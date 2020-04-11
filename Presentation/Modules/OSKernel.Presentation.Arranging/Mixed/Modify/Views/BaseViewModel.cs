using OSKernel.Presentation.Core;
using OSKernel.Presentation.Core.ViewModel;
using OSKernel.Presentation.CustomControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Unity;
using XYKernel.OS.Common.Enums;

namespace OSKernel.Presentation.Arranging.Mixed.Modify.Views
{
    public class BaseViewModel : CommonViewModel, IInitilize
    {
        private string _version;
        private string _school;
        private Dictionary<string, LearningPeriod> _periods;
        private LearningPeriod _selectLearningPeriod;

        public ICommand SaveCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand(Save);
            }
        }

        public BaseViewModel()
        {
            this.Periods = new Dictionary<string, LearningPeriod>()
            {
                { "幼儿园（建设中，默认为小学，不影响排课）",LearningPeriod.Kindergarten},
                {"小学",LearningPeriod.PrimarySchool},
                {"初中",LearningPeriod.JuniorMiddleSchool},
                {"高中",LearningPeriod.SeniorMiddleSchool},
            };
        }

        private void Save()
        {
            if (string.IsNullOrEmpty(this.School))
            {
                this.ShowDialog("提示信息", "学校名称不能为空!", CustomControl.Enums.DialogSettingType.OnlyOkButton, CustomControl.Enums.DialogType.Warning);
                return;
            }

            var cl = CommonDataManager.GetCLCase(base.LocalID);
            cl.LearningPeriod = this.SelectLearningPeriod == LearningPeriod.Kindergarten ? LearningPeriod.PrimarySchool : this.SelectLearningPeriod;
            cl.SchoolName = this.School;
            cl.Version = this.Version;
            cl.Serialize(base.LocalID);

            this.ShowDialog("提示信息", "保存成功!", CustomControl.Enums.DialogSettingType.NoButton, CustomControl.Enums.DialogType.None);
        }

        /// <summary>
        /// 版本信息
        /// </summary>
        public string Version
        {
            get
            {
                return _version;
            }

            set
            {
                _version = value;
                RaisePropertyChanged(() => Version);
            }
        }

        /// <summary>
        /// 学校
        /// </summary>
        public string School
        {
            get
            {
                return _school;
            }

            set
            {
                _school = value;
                RaisePropertyChanged(() => School);
            }
        }

        /// <summary>
        /// 学段
        /// </summary>
        public Dictionary<string, LearningPeriod> Periods
        {
            get
            {
                return _periods;
            }

            set
            {
                _periods = value;
                RaisePropertyChanged(() => Periods);
            }
        }

        /// <summary>
        /// 当前选择的学段
        /// </summary>
        public LearningPeriod SelectLearningPeriod
        {
            get
            {
                return _selectLearningPeriod;
            }

            set
            {
                _selectLearningPeriod = value;
                RaisePropertyChanged(() => SelectLearningPeriod);
            }
        }

        [InjectionMethod]
        public void Initilize()
        {
            // 获取走班
            var cl = CommonDataManager.GetCLCase(base.LocalID);

            this.SelectLearningPeriod = cl.LearningPeriod;
            this.School = cl.SchoolName;
            this.Version = cl.Version;

        }
    }
}
