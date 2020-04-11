using OSKernel.Presentation.Arranging.Administrative.Modify.Rule.Teacher.Model;
using OSKernel.Presentation.Core;
using OSKernel.Presentation.Core.ViewModel;
using OSKernel.Presentation.CustomControl;
using OSKernel.Presentation.Models.Base;
using OSKernel.Presentation.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Unity;
using XYKernel.OS.Common.Enums;

namespace OSKernel.Presentation.Arranging.Administrative.Modify.Rule.Teacher.Dialog
{
    public class CreateMasterApprentticeWindowModel : CommonViewModel, IInitilize
    {
        private Dictionary<string, MasterApprenticeFollow> _masterFollows;

        private MasterApprenticeFollow _selectMasterType;

        private UICourse _selectCourse;

        private UITeacher _selectMaster;

        private List<UICourse> _courses;

        private List<UITeacher> _masters;

        private List<UITeacher> _apprentices;

        /// <summary>
        /// 科目教师
        /// </summary>
        public List<UITeacher> _courseTeachers { get; set; }

        public Dictionary<string, MasterApprenticeFollow> MasterFollows
        {
            get
            {
                return _masterFollows;
            }

            set
            {
                _masterFollows = value;
                RaisePropertyChanged(() => MasterFollows);
            }
        }

        public MasterApprenticeFollow SelectMasterType
        {
            get
            {
                return _selectMasterType;
            }

            set
            {
                _selectMasterType = value;
                RaisePropertyChanged(() => SelectMasterType);
            }
        }

        public ICommand SaveCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<object>(saveCommand);
            }
        }

        public ICommand CancelCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<object>(cancelCommand);
            }
        }

        public List<UICourse> Courses
        {
            get
            {
                return _courses;
            }

            set
            {
                _courses = value;
                RaisePropertyChanged(() => Courses);
            }
        }

        public List<UITeacher> Apprentices
        {
            get
            {
                return _apprentices;
            }

            set
            {
                _apprentices = value;
                RaisePropertyChanged(() => Apprentices);
            }
        }

        public UICourse SelectCourse
        {
            get
            {
                return _selectCourse;
            }

            set
            {
                _selectCourse = value;
                RaisePropertyChanged(() => SelectCourse);

                // 选中课程
                this.getTechers(value);
            }
        }

        public UITeacher SelectMaster
        {
            get
            {
                return _selectMaster;
            }

            set
            {
                _selectMaster = value;
                RaisePropertyChanged(() => SelectMaster);

                if (SelectMaster != null)
                {
                    // 移除当前选中教师目标里面
                    this.Apprentices = this._courseTeachers.Where(t => !t.ID.Equals(SelectMaster.ID))?.ToList();
                }
            }
        }

        public List<UITeacher> Masters
        {
            get
            {
                return _masters;
            }

            set
            {
                _masters = value;
                RaisePropertyChanged(() => Masters);
            }
        }

        public CreateMasterApprentticeWindowModel()
        {
            this.Apprentices = new List<UITeacher>();
            this.Courses = new List<UICourse>();
            this.Masters = new List<UITeacher>();

            //this.MasterFollows = new Dictionary<string, MasterApprenticeFollow>()
            //{
            //    {"早于", MasterApprenticeFollow.Before },
            //    {"晚于", MasterApprenticeFollow.After  }
            //};

            //this.SelectMasterType = MasterApprenticeFollow.Before;

            base.Weights = new Dictionary<string, WeightTypeEnum>()
            {
                { "高", WeightTypeEnum.Hight},
                { "中", WeightTypeEnum.Medium},
                { "低", WeightTypeEnum.Low},
            };
            base.SelectWeight = WeightTypeEnum.Hight;
        }

        [InjectionMethod]
        public void Initilize()
        {
            // 绑定课程
            var rule = CommonDataManager.GetAminRule(base.LocalID);

            var cp = CommonDataManager.GetCPCase(base.LocalID);

            var courses = (from c in cp.Courses
                           select new UICourse
                           {
                               ID = c.ID,
                               Name = c.Name
                           })?.ToList();
            this.Courses = courses;
        }

        void saveCommand(object obj)
        {
            var win = obj as CreateMasterApprentticeWindow;

            var has = this.Apprentices.Any(a => a.IsChecked);
            if (!has)
            {
                this.ShowDialog("提示信息", "没有选中徒弟!", CustomControl.Enums.DialogSettingType.OnlyOkButton, CustomControl.Enums.DialogType.Warning);
                return;
            }

            var results = new List<UIMasterApprenttice>();
            var checkedApprenttics = this.Apprentices.Where(a => a.IsChecked)?.ToList();

            win.Rule = new UIMasterApprenttice()
            {
                Master = this.SelectMaster.Name,
                MasterID = this.SelectMaster.ID,
                Course = this.SelectCourse.Name,
                CourseID = this.SelectCourse.ID,
                Weight = base.SelectWeight,
                ApprentticeID = checkedApprenttics?.Select(s =>
                {
                    return new XYKernel.OS.Common.Models.Administrative.TeacherModel()
                    {
                        ID = s.ID,
                        Name = s.Name
                    };
                })?.ToList(),
            };
            win.WeightType = base.SelectWeight;
            win.DialogResult = true;
            //win.IsSave = true;
            //this.ShowDialog("提示信息", "保存成功!", CustomControl.Enums.DialogSettingType.NoButton, CustomControl.Enums.DialogType.None);
        }

        void getTechers(UICourse course)
        {
            // 获取方案
            var cp = CommonDataManager.GetCPCase(base.LocalID);

            // 获取教师
            var teachers = cp.GetTeachers(course.ID);
            // 1.1 当前教师数据源
            this._courseTeachers = teachers;
            // 1.2 师傅
            this.Masters = teachers;
            // 1.3 徒弟
            this.Apprentices = teachers;
        }

        void cancelCommand(object obj)
        {
            var win = obj as CreateMasterApprentticeWindow;
            win.DialogResult = false;
            //win.DialogResult = win.IsSave;
        }
    }
}
