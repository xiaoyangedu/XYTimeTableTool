using OSKernel.Presentation.Core;
using OSKernel.Presentation.Core.ViewModel;
using OSKernel.Presentation.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Unity;

namespace OSKernel.Presentation.Arranging.Dialog
{
    public class ChooseCourseWindowModel : CommonViewModel, IInitilize
    {
        private bool _checkedAll;

        private List<Models.Base.UICourse> _courses;

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

                this.Courses.ForEach(c =>
                {
                    c.IsChecked = _checkedAll;
                });
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

        public ChooseCourseWindowModel()
        {
            this.Courses = new List<UICourse>();
        }

        [InjectionMethod]
        public void Initilize()
        {

        }

        public void BindData(List<UICourse> current, List<UICourse> all)
        {
            this.Courses = all;

            current.ForEach(c =>
            {
                var course = this.Courses.FirstOrDefault(cc => cc.ID.Equals(c.ID));
                if (course != null)
                {
                    course.IsEnable = true;
                }
            });
        }

        void save(object obj)
        {
            ChooseCourseWindow window = obj as ChooseCourseWindow;
            window.Courses = this.Courses.Where(c => !c.IsEnable && c.IsChecked)?.ToList();
            window.DialogResult = true;
        }

        void cancel(object obj)
        {
            ChooseCourseWindow window = obj as ChooseCourseWindow;
            window.DialogResult = false;
        }
    }
}
