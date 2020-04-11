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
using XYKernel.OS.Common.Models.Mixed;

namespace OSKernel.Presentation.Arranging.Mixed.Modify.Views
{
    public class TeacherViewModel : CommonViewModel, IInitilize, IRefresh
    {
        private bool _allChecked;

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
            var cl = base.GetClCase(base.LocalID);

            List<UITeacher> teachers = new List<UITeacher>();
            cl.Teachers.ForEach(t =>
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

                    var cl = base.GetClCase(base.LocalID);

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
                        cl.Teachers.Add(teacher);
                    }

                    base.Serialize(cl, LocalID);
                }
            };
            window.ShowDialog();
        }

        void batchDeleteCommand()
        {
            var dialog = this.ShowDialog("提示信息", "确认删除?", CustomControl.Enums.DialogSettingType.OkAndCancel, CustomControl.Enums.DialogType.Warning);
            if (dialog == CustomControl.Enums.DialogResultType.OK)
            {
                var cl = base.GetClCase(base.LocalID);

                // 验证是否确认删除?
                var toDeleteTeachers = this.Teachers.Where(t => t.IsChecked)?.ToList();
                if (toDeleteTeachers != null)
                {
                    foreach (var t in toDeleteTeachers)
                    {
                        // 更新UI
                        this.Teachers.Remove(t);
                        // 移除教师
                        cl.Teachers.RemoveAll(teacher => teacher.ID.Equals(t.ID));
                        // 删除基础数据
                        MixedDataHelper.TeacherChanged(t, base.LocalID, CommonDataManager);

                        // 发送消息
                        GalaSoft.MvvmLight.Messaging.Messenger.Default.Send<UITeacher>(t);
                    }

                    base.Serialize(cl, LocalID);
                }
            }

        }

        public void Refresh()
        {
            this.Initilize();
        }
    }
}
