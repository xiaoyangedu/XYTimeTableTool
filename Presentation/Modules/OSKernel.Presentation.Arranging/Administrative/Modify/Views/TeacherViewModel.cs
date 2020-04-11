using OSKernel.Presentation.Arranging.Dialog;
using OSKernel.Presentation.Core;
using OSKernel.Presentation.Core.ViewModel;
using OSKernel.Presentation.CustomControl;
using OSKernel.Presentation.Models.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using Unity;
using XYKernel.OS.Common.Models.Administrative;

namespace OSKernel.Presentation.Arranging.Administrative.Modify.Views
{
    /// <summary>
    /// 教师模型
    /// </summary>
    public class TeacherViewModel : CommonViewModel, IInitilize
    {
        private bool _allChecked;

        private CPCase _cpcase;

        private ListCollectionView _teacherCollectionView;

        private string _searchTeacher;

        private ObservableCollection<UITeacher> _teachers;

        public ObservableCollection<UITeacher> Teachers
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

        public ICommand CreateCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand(createCommand);
            }
        }

        public ICommand BatchDeleteCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand(batchDeleteCommand);
            }
        }

        public ICommand SaveCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand(saveCommand);
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

        public TeacherViewModel()
        {
            this.Teachers = new ObservableCollection<UITeacher>();
        }

        [InjectionMethod]
        public void Initilize()
        {
            var cp = CommonDataManager.GetCPCase(base.LocalID);
            if (cp != null)
            {
                _cpcase = cp;

                if (cp.Teachers != null)
                {
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
                    this.Teachers = new ObservableCollection<UITeacher>(teachers);

                    _teacherCollectionView = (ListCollectionView)CollectionViewSource.GetDefaultView(this.Teachers);
                    _teacherCollectionView.Filter = TeacherContains;
                }
            }
            else
            {
                _cpcase = new CPCase();
            }
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

        void createCommand()
        {
            CreateTeacherWindow window = new CreateTeacherWindow();
            window.Closed += (s, args) =>
            {
                if (window.DialogResult.Value)
                {
                    var has = this.Teachers.Any(c =>
                    {
                        return window.Teachers.Any(cc => cc.Equals(c.Name));
                    });

                    if (has)
                    {
                        var result = this.ShowDialog("提示信息", "存在相同教师,是否继续添加?", CustomControl.Enums.DialogSettingType.OkAndCancel, CustomControl.Enums.DialogType.Warning);
                        if (result != CustomControl.Enums.DialogResultType.OK)
                        {
                            return;
                        }
                    }

                    foreach (var t in window.Teachers)
                    {
                        var tid = this.Teachers.Count == 0 ? 0 : this.Teachers.Max(tt => Convert.ToInt64(tt.ID));

                        // 创建教师
                        TeacherModel teacher = new TeacherModel()
                        {
                            ID = (tid + 1).ToString(),
                            Name = t
                        };

                        // 更新UI
                        this.Teachers.Add(new UITeacher
                        {
                            ID = teacher.ID,
                            Name = teacher.Name
                        });

                        // 更新缓存
                        _cpcase.Teachers.Add(teacher);
                    }

                    _cpcase.Serialize(base.LocalID);
                }
            };
            window.ShowDialog();
        }

        void batchDeleteCommand()
        {
            var dialog = this.ShowDialog("提示信息", "确认删除?", CustomControl.Enums.DialogSettingType.OkAndCancel, CustomControl.Enums.DialogType.Warning);
            if (dialog == CustomControl.Enums.DialogResultType.OK)
            {
                // 验证是否确认删除?
                var toDeleteTeachers = this.Teachers.Where(t => t.IsChecked)?.ToList();

                if (toDeleteTeachers != null)
                {
                    foreach (var t in toDeleteTeachers)
                    {
                        // 更新UI
                        this.Teachers.Remove(t);
                        // 移除教师
                        _cpcase.Teachers.RemoveAll(teacher => teacher.ID.Equals(t.ID));

                    AdministrativeDataHelper.TeacherChanged(t, base.LocalID, CommonDataManager);

                    // 移除教师
                    GalaSoft.MvvmLight.Messaging.Messenger.Default.Send<UITeacher>(t);
                }

                    // 保存
                    _cpcase.Serialize(base.LocalID);
                }
            }
        }

        void saveCommand()
        {

            var local = CommonDataManager.GetLocalCase(base.LocalID);
            local.Serialize();

            _cpcase.Serialize(base.LocalID);

            this.ShowDialog("提示信息", "保存成功", CustomControl.Enums.DialogSettingType.NoButton, CustomControl.Enums.DialogType.None);
        }
    }
}
