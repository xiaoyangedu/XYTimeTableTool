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

namespace OSKernel.Presentation.Arranging.Administrative.Dialog
{
    /// <summary>
    /// 学生信息实体模型
    /// </summary>
    public class StudentWindowModel : CommonViewModel, IInitilize
    {
        private bool _allChecked;

        private string _searchStudent;

        private CPCase _cpcase;

        private ListCollectionView _studentCollectionView;

        private ObservableCollection<UIStudent> _students;

        /// <summary>
        /// 搜索学生名称
        /// </summary>
        public string SearchStudent
        {
            get
            {
                return _searchStudent;
            }

            set
            {
                _searchStudent = value;
                RaisePropertyChanged(() => SearchStudent);

                _studentCollectionView.Refresh();
            }
        }

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

                foreach (var s in Students)
                {
                    s.IsChecked = value;
                }
            }
        }

        public ICommand BatchDeleteCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand(batchDeleteCommand);
            }
        }

        public ICommand CreateCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand(createCommand);
            }
        }

        public ObservableCollection<UIStudent> Students
        {
            get
            {
                return _students;
            }

            set
            {
                _students = value;
            }
        }

        public StudentWindowModel()
        {
            this.Students = new ObservableCollection<UIStudent>();
        }

        [InjectionMethod]
        public void Initilize() { }

        bool StudentContains(object contain)
        {
            UIStudent student = contain as UIStudent;
            if (string.IsNullOrEmpty(this.SearchStudent))
                return true;

            if (student.Name.IndexOf(this.SearchStudent) != -1)
            {
                return true;
            }
            else
                return false;
        }

        void createCommand()
        {
            CreateStudentWindow window = new CreateStudentWindow();
            window.Closed += (s, args) =>
            {
                if (window.DialogResult.Value)
                {
                    var has = this.Students.Any(c =>
                    {
                        return window.Students.Any(cc => cc.Equals(c.Name));
                    });

                    if (has)
                    {
                        var result = this.ShowDialog("提示信息", "存在相同学生,是否继续添加?", CustomControl.Enums.DialogSettingType.OkAndCancel, CustomControl.Enums.DialogType.Warning);
                        if (result != CustomControl.Enums.DialogResultType.OK)
                        {
                            return;
                        }
                    }

                    foreach (var t in window.Students)
                    {
                        var tid = this.Students.Count == 0 ? 0 : this.Students.Max(tt => Convert.ToInt64(tt.ID));

                        // 创建学生
                        StudentModel student = new StudentModel()
                        {
                            ID = (tid + 1).ToString(),
                            Name = t
                        };

                        // 更新UI
                        this.Students.Add(new UIStudent
                        {
                            ID = student.ID,
                            Name = student.Name
                        });

                        // 更新缓存
                        _class.Students.Add(student);
                    }

                    _cpcase.Serialize(base.LocalID);
                }
            };
            window.ShowDialog();
        }

        void batchDeleteCommand()
        {
            // 验证是否确认删除?
            var toDeleteStudents = this.Students.Where(t => t.IsChecked)?.ToList();

            if (toDeleteStudents != null)
            {
                foreach (var t in toDeleteStudents)
                {
                    // 更新UI
                    this.Students.Remove(t);
                    // 移除学生
                    _class.Students.RemoveAll(student => student.ID.Equals(t.ID));
                }

                // 保存
                _cpcase.Serialize(base.LocalID);
            }
        }

        private ClassModel _class;
        public void GetData(UIClass classModel)
        {
            var cp = CommonDataManager.GetCPCase(base.LocalID);
            if (cp != null)
            {
                _cpcase = cp;

                List<UIStudent> students = new List<UIStudent>();
                _class = cp.Classes.FirstOrDefault(c => c.ID.Equals(classModel.ID));
                _class.Students?.ForEach(s =>
                {
                    UIStudent student = new UIStudent()
                    {
                        ID = s.ID,
                        Name = s.Name
                    };
                    students.Add(student);
                });

                this.Students = new ObservableCollection<UIStudent>(students);

                _studentCollectionView = (ListCollectionView)CollectionViewSource.GetDefaultView(this.Students);
                _studentCollectionView.Filter = StudentContains;
            }
        }
    }
}
