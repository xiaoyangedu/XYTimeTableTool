using GalaSoft.MvvmLight.Messaging;
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
using System.Windows.Data;
using System.Windows.Input;
using Unity;

namespace OSKernel.Presentation.Arranging.Administrative.Modify.Rule.Teacher
{
    public class AmPmNoContinuesViewModel : CommonViewModel, IInitilize
    {
        private bool _allChecked;

        private string _searchTeacher;

        private List<UITeacher> _teachers;

        private ListCollectionView _teacherCollectionView;

        public List<UITeacher> Teachers
        {
            get
            {
                return _teachers;
            }

            set
            {
                _teachers = value;
                RaisePropertyChanged(() => Teachers);
            }
        }

        public ICommand UserControlCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<string>(userControlCommand);
            }
        }

        /// <summary>
        /// 选中全部
        /// </summary>
        public bool AllChecked
        {
            get
            {
                return _allChecked;
            }

            set
            {
                _allChecked = value;
                RaisePropertyChanged(() => AllChecked);

                foreach (var t in Teachers)
                {
                    t.IsChecked = value;
                }
            }
        }

        /// <summary>
        /// 搜索教师
        /// </summary>
        public string SearchTeacher
        {
            get
            {
                return _searchTeacher;
            }

            set
            {
                _searchTeacher = value;
                RaisePropertyChanged(() => SearchTeacher);

                _teacherCollectionView.Refresh();
            }
        }

        public AmPmNoContinuesViewModel()
        {
            this.Teachers = new List<UITeacher>();

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
            Messenger.Default.Register<HostView>(this, save);

            this.Comments = CommonDataManager.GetAdminRuleComments(AdministrativeRuleEnum.TeacherAmPmNoContinues);

            #region 加载教师

            var cp = CommonDataManager.GetCPCase(base.LocalID);

            List<UITeacher> teachers = new List<UITeacher>();
            cp.Teachers.ForEach(t =>
            {
                UITeacher teacher = new UITeacher()
                {
                    ID = t.ID,
                    Name = t.Name,
                };
                teachers.Add(teacher);
            });
            this.Teachers = teachers;

            _teacherCollectionView = (ListCollectionView)CollectionViewSource.GetDefaultView(this.Teachers);
            _teacherCollectionView.Filter = TeacherContains;

            #endregion

            #region 绑定

            var rule = CommonDataManager.GetAminRule(base.LocalID);
            if (rule != null)
            {
                rule.AmPmNoContinues.ForEach(rc =>
                {
                    var teacher = this.Teachers.FirstOrDefault(t => t.ID.Equals(rc.TeacherID));

                    if (teacher != null)
                    {
                        teacher.IsChecked = true;
                        teacher.Weight = (WeightTypeEnum)rc.Weight;
                    }
                });
            }

            #endregion
        }

        bool TeacherContains(object contain)
        {
            UITeacher teacher = contain as UITeacher;
            if (string.IsNullOrEmpty(this.SearchTeacher))
                return true;

            if (teacher.Name.IndexOf(this.SearchTeacher) != -1)
            {
                return true;
            }
            else
                return false;
        }

        void save(HostView host)
        {
            var rule = CommonDataManager.GetAminRule(base.LocalID);

            // 1.清楚之前的结果
            rule.AmPmNoContinues.Clear();

            // 2.教师列表（过滤教师）
            this.Teachers.Where(t => t.IsChecked)?.ToList()?.ForEach(t =>
            {
                rule.AmPmNoContinues.Add(new XYKernel.OS.Common.Models.Administrative.Rule.TeacherAmPmNoContinousRule()
                {
                    TeacherID = t.ID,
                    Weight = (int)t.Weight

                });
            });

            // 3.弹出提示（保存成功）
            rule.Serialize(base.LocalID);
            this.ShowDialog("提示信息", "保存成功", CustomControl.Enums.DialogSettingType.NoButton, CustomControl.Enums.DialogType.None);
        }

        void userControlCommand(string parms)
        {
            if (parms.Equals("loaded"))
            {

            }
            else if (parms.Equals("unloaded"))
            {
                Messenger.Default.Unregister<HostView>(this, save);
            }
        }

        public override void BatchSetWeight(WeightTypeEnum weightEnum)
        {
            this.Teachers.ForEach(t =>
            {
                t.Weight = weightEnum;
            });
        }
    }
}
