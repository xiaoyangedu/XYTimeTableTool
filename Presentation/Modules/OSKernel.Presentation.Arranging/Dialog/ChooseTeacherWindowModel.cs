using OSKernel.Presentation.Core;
using OSKernel.Presentation.Core.ViewModel;
using OSKernel.Presentation.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using Unity;

namespace OSKernel.Presentation.Arranging.Dialog
{
    public class ChooseTeacherWindowModel : CommonViewModel, IInitilize
    {
        private bool _checkedAll;

        private List<Models.Base.UITeacher> _teachers;

        private ListCollectionView _teacherCollectionView;

        private string _searchTeacher;

        public bool CheckedAll
        {
            get
            {
                return _checkedAll;
            }

            set
            {
                _checkedAll = value;
                RaisePropertyChanged(() => CheckedAll);

                this.Teachers.ForEach(t => t.IsChecked = _checkedAll);
            }
        }

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

        public ICommand SaveCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<object>(save);
            }
        }

        public ICommand CancelCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<object>(cancel);
            }
        }

        public ChooseTeacherWindowModel()
        {
            this.Teachers = new List<UITeacher>();
        }

        [InjectionMethod]
        public void Initilize()
        {
        }

        public void BindData(List<Models.Base.UITeacher> currentTeachers, List<Models.Base.UITeacher> allTeachers)
        {
            this.Teachers = allTeachers;

            currentTeachers.ForEach(ct =>
            {
                var teacher = this.Teachers.First(t => t.ID.Equals(ct.ID));
                if (teacher != null)
                {
                    teacher.IsChecked = true;
                }
            });

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


        void save(object obj)
        {
            ChooseTeacherWindow window = obj as ChooseTeacherWindow;
            window.Teachers = this.Teachers.Where(t => t.IsChecked)?.ToList();
            window.DialogResult = true;
        }

        void cancel(object obj)
        {
            ChooseTeacherWindow window = obj as ChooseTeacherWindow;
            window.DialogResult = false;
        }
    }
}
