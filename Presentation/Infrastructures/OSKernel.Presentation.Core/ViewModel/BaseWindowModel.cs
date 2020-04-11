using Unity;
using GalaSoft.MvvmLight;
using OSKernel.Presentation.Core.DataManager;
using OSKernel.Presentation.Models.Enums;
using XYKernel.OS.Common.Models.Administrative;
using OSKernel.Presentation.Models.Mixed;
using System.Linq;

namespace OSKernel.Presentation.Core.ViewModel
{
    public class BaseWindowModel : CommonViewModel
    {
        private int _weight = 100;
        private bool _isActive = true;
        private string _titleString;

        private XYKernel.OS.Common.Models.Administrative.TeacherModel _selectTeacher;
        private XYKernel.OS.Common.Models.Administrative.CourseModel _selectCourse;
        private XYKernel.OS.Common.Models.Administrative.ClassModel _selectClass;

        private XYKernel.OS.Common.Models.Mixed.TeacherModel _selectMixedTeacher;
        private XYKernel.OS.Common.Models.Mixed.CourseModel _selectMixedCourse;
        private UIClass _selectMixedClass;
        private XYKernel.OS.Common.Models.Mixed.TagModel _selectMixedTag;


        public OperatorEnum OpratorEnum { get; set; }

        public string UID { get; set; }

        /// <summary>
        /// 如果是添加则显示
        /// </summary>
        public bool ShowEdit
        {
            get
            {
                if (OpratorEnum == OperatorEnum.Add)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// 如果是编辑则显示
        /// </summary>
        public bool ShowRead
        {
            get
            {
                if (OpratorEnum == OperatorEnum.Modify)
                    return true;
                else
                    return false;
            }
        }

        public string TitleString
        {
            get
            {
                return _titleString;
            }

            set
            {
                _titleString = value;
                RaisePropertyChanged(() => TitleString);
            }
        }

        public int Weight
        {
            get
            {
                return _weight;
            }

            set
            {
                if (value <= 0) return;
                if (value > 100) return;

                _weight = value;
                RaisePropertyChanged(() => Weight);
            }
        }

        public bool IsActive
        {
            get
            {
                return _isActive;
            }

            set
            {
                _isActive = value;
                RaisePropertyChanged(() => IsActive);
            }
        }

        public int MaxHour
        {
            get
            {
                var local = CommonDataManager.GetLocalCase(CommonDataManager.LocalID);
                if (local.CaseType == CaseTypeEnum.Administrative)
                {
                    var cp = CommonDataManager.GetCPCase(local.LocalID);
                    var filter = cp?.Positions?.Where(p => p.Position == XYKernel.OS.Common.Enums.Position.AM
                    || p.Position == XYKernel.OS.Common.Enums.Position.PM
                    || p.Position == XYKernel.OS.Common.Enums.Position.MS
                    || p.Position == XYKernel.OS.Common.Enums.Position.ES);

                    if (filter != null)
                    {
                        return filter.GroupBy(f => f.PositionOrder).Count();
                    }
                    else
                        return 0;
                }
                else if (local.CaseType == CaseTypeEnum.Mixed)
                {
                    var cl = CommonDataManager.GetCLCase(local.LocalID);
                    var filter = cl?.Positions?.Where(p => p.Position == XYKernel.OS.Common.Enums.Position.AM
                    || p.Position == XYKernel.OS.Common.Enums.Position.PM
                    || p.Position == XYKernel.OS.Common.Enums.Position.MS
                    || p.Position == XYKernel.OS.Common.Enums.Position.ES);

                    if (filter != null)
                    {
                        return filter.GroupBy(f => f.PositionOrder).Count();
                    }
                    else
                        return 0;
                }
                else
                    return 0;
            }
        }

        public TeacherModel SelectTeacher
        {
            get
            {
                return _selectTeacher;
            }

            set
            {
                _selectTeacher = value;
                RaisePropertyChanged(() => SelectTeacher);

                if (_selectTeacher != null)
                    Search();
            }
        }

        public CourseModel SelectCourse
        {
            get
            {
                return _selectCourse;
            }

            set
            {
                _selectCourse = value;
                RaisePropertyChanged(() => SelectCourse);

                if (_selectCourse != null)
                    Search();
            }
        }

        public ClassModel SelectClass
        {
            get
            {
                return _selectClass;
            }

            set
            {
                _selectClass = value;
                RaisePropertyChanged(() => SelectClass);

                if (_selectClass != null)
                    Search();
            }
        }

        public XYKernel.OS.Common.Models.Mixed.TeacherModel SelectMixedTeacher
        {
            get
            {
                return _selectMixedTeacher;
            }

            set
            {
                _selectMixedTeacher = value;
                RaisePropertyChanged(() => SelectMixedTeacher);
                if (_selectMixedTeacher != null)
                    Search();
            }
        }

        public XYKernel.OS.Common.Models.Mixed.CourseModel SelectMixedCourse
        {
            get
            {
                return _selectMixedCourse;
            }

            set
            {
                _selectMixedCourse = value;
                RaisePropertyChanged(() => SelectMixedCourse);
                if (_selectMixedCourse != null)
                    Search();
            }
        }

        public UIClass SelectMixedClass
        {
            get
            {
                return _selectMixedClass;
            }

            set
            {
                _selectMixedClass = value;
                RaisePropertyChanged(() => SelectMixedClass);
                if (_selectMixedClass != null)
                    Search();
            }
        }

        public XYKernel.OS.Common.Models.Mixed.TagModel SelectMixedTag
        {
            get
            {
                return _selectMixedTag;
            }

            set
            {
                _selectMixedTag = value;
                RaisePropertyChanged(() => SelectMixedTag);
                if (_selectMixedTag != null)
                    Search();
            }
        }

        public virtual void Search()
        {

        }
    }
}
