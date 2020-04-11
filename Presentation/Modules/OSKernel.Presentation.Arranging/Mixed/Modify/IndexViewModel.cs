using OSKernel.Presentation.Arranging.Mixed.Dialog;
using OSKernel.Presentation.Arranging.Mixed.Modify.Views;
using OSKernel.Presentation.Core;
using OSKernel.Presentation.Core.ViewModel;
using OSKernel.Presentation.CustomControl;
using OSKernel.Presentation.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Unity;

namespace OSKernel.Presentation.Arranging.Mixed.Modify
{
    public class IndexViewModel : CommonViewModel, IInitilize
    {
        private TimeViewModel _timeVM;
        private TeacherViewModel _teacherVM;
        private TagViewModel _tagVM;
        private CourseViewModel _courseVM;
        private StudentViewModel _studentVM;
        private ClassViewModel _classVM;
        private RuleViewModel _ruleVM;


        /// <summary>
        /// 当前模式类型
        /// </summary>
        private Models.Enums.PatternTypeEnum _patternType;

        private bool _hasPattern;

        /// <summary>
        /// 是否存在模式
        /// </summary>
        public bool HasPattern
        {
            get
            {
                return _hasPattern;
            }

            set
            {
                _hasPattern = value;
                RaisePropertyChanged(() => HasPattern);
            }
        }

        public ICommand SettingPatternCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand(settingPattern);
            }
        }

        public ICommand DeleteCurrentPatternCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand(deleteCurrentPattern);
            }
        }

        public string PatternName
        {
            get
            {
                return _patternType.GetLocalDescription();
            }
        }

        public TimeViewModel TimeVM
        {
            get
            {
                return _timeVM;
            }

            set
            {
                _timeVM = value;
                RaisePropertyChanged(() => TimeVM);
            }
        }

        public TeacherViewModel TeacherVM
        {
            get
            {
                return _teacherVM;
            }

            set
            {
                _teacherVM = value;
                RaisePropertyChanged(() => TeacherVM);
            }
        }

        public TagViewModel TagVM
        {
            get
            {
                return _tagVM;
            }

            set
            {
                _tagVM = value;
                RaisePropertyChanged(() => TagVM);
            }
        }

        public CourseViewModel CourseVM
        {
            get
            {
                return _courseVM;
            }

            set
            {
                _courseVM = value;
                RaisePropertyChanged(() => CourseVM);
            }
        }

        public StudentViewModel StudentVM
        {
            get
            {
                return _studentVM;
            }

            set
            {
                _studentVM = value;
                RaisePropertyChanged(() => StudentVM);
            }
        }

        public ClassViewModel ClassVM
        {
            get
            {
                return _classVM;
            }

            set
            {
                _classVM = value;
                RaisePropertyChanged(() => ClassVM);
            }
        }

        public RuleViewModel RuleVM
        {
            get
            {
                return _ruleVM;
            }

            set
            {
                _ruleVM = value;
                RaisePropertyChanged(() => RuleVM);
            }
        }

        public IndexViewModel()
        {

        }

        [InjectionMethod]
        public void Initilize()
        {
            var local = CommonDataManager.GetLocalCase(base.LocalID);
            if (local.Pattern != Models.Enums.PatternTypeEnum.None)
            {
                this._patternType = local.Pattern;
                this.RaisePropertyChanged(() => PatternName);
                this.HasPattern = true;
            }

            this.ClassVM = CacheManager.Instance.UnityContainer.Resolve<ClassViewModel>();
            this.CourseVM = CacheManager.Instance.UnityContainer.Resolve<CourseViewModel>();
            this.RuleVM = CacheManager.Instance.UnityContainer.Resolve<RuleViewModel>();
            this.StudentVM = CacheManager.Instance.UnityContainer.Resolve<StudentViewModel>();
            this.TagVM = CacheManager.Instance.UnityContainer.Resolve<TagViewModel>();
            this.TeacherVM = CacheManager.Instance.UnityContainer.Resolve<TeacherViewModel>();
            this.TimeVM = CacheManager.Instance.UnityContainer.Resolve<TimeViewModel>();
        }


        /// <summary>
        /// 设置模式
        /// </summary>
        void settingPattern()
        {
            SetPatternWindow window = new SetPatternWindow();
            window.Closed += (s, arg) =>
            {
                if (window.DialogResult.Value)
                {
                    this._patternType = window.PatternType;
                    this.RaisePropertyChanged(() => PatternName);

                    if (this._patternType != Models.Enums.PatternTypeEnum.None)
                        this.HasPattern = true;
                    else
                        this.HasPattern = false;

                    this.refreshPattern();
                }
            };
            window.ShowDialog();
        }

        void deleteCurrentPattern()
        {
            var result = this.ShowDialog("提示信息", "确认删除当前模式?", CustomControl.Enums.DialogSettingType.OkAndCancel, CustomControl.Enums.DialogType.Warning);
            if (result == CustomControl.Enums.DialogResultType.OK)
            {
                // 保存本地方案模式状态
                var local = CommonDataManager.GetLocalCase(base.LocalID);
                local.Pattern = Models.Enums.PatternTypeEnum.None;
                local.Serialize();

                // 删除所有模式
                base.LocalID.DeleteAllPatternData();
                base.PatternDataManager.RemoveAll(base.LocalID);
                this.HasPattern = false;

                this.refreshPattern();
            }
        }

        void refreshPattern()
        {
            // 刷新界面编辑(TODO)
            ClassVM.Refresh();

            CourseVM.Refresh();

            StudentVM.Refresh();

            TagVM.Refresh();

            TeacherVM.Refresh();

            TimeVM.Refresh();

            RuleVM.Refresh();
        }
    }
}
