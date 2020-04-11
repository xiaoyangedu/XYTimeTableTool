using OSKernel.Presentation.Arranging.Mixed.Modify.Rule.Teacher.Model;
using OSKernel.Presentation.Core;
using OSKernel.Presentation.Core.ViewModel;
using OSKernel.Presentation.CustomControl;
using OSKernel.Presentation.Models.Mixed;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Unity;
using XYKernel.OS.Common.Enums;

namespace OSKernel.Presentation.Arranging.Mixed.Modify.Rule.Teacher.Dialog
{
    public class CreateClassHourSameOpenViewModel : CommonViewModel, IInitilize
    {
        private List<UICourse> _courses;

        private ObservableCollection<UIClass> _classes;

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

        public ICommand UserControlCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<string>(userControlCommand);
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

        public ObservableCollection<UIClass> Classes
        {
            get
            {
                return _classes;
            }

            set
            {
                _classes = value;
                RaisePropertyChanged(() => Classes);
            }
        }

        public CreateClassHourSameOpenViewModel()
        {
            this.Courses = new List<UICourse>();
            this.Classes = new ObservableCollection<UIClass>();
        }

        [InjectionMethod]
        public void Initilize()
        {
            // 绑定课程
            var rule = base.GetClRule(base.LocalID);

            var cl = base.GetClCase(base.LocalID);

            this.Courses = (from c in cl.Courses
                            select new UICourse
                            {
                                ID = c.ID,
                                Name = c.Name
                            })?.ToList();

            this.Courses.ForEach(c =>
            {
                c.PropertyChanged += C_PropertyChanged;
            });
        }

        private void C_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            UICourse course = sender as UICourse;

            if (e.PropertyName.Equals(nameof(course.IsChecked)))
            {
                // 1.先清除
                var classes = this.Classes.Where(c => c.CourseID == course.ID)?.ToList();
                if (classes != null)
                {
                    classes.ForEach(c =>
                    {
                        this.Classes.Remove(c);
                    });
                }

                // 2.再添加
                if (course.IsChecked)
                {
                    var cl = CommonDataManager.GetCLCase(base.LocalID);

                    var results = cl.GetClasses(course.ID);
                    results?.ForEach(r =>
                    {
                        r.IsChecked = true;
                        this.Classes.Add(r);
                    });
                }
            }
        }

        void userControlCommand(string parms)
        {
            if (parms.Equals("loaded"))
            {

            }
            else if (parms.Equals("unloaded"))
            {
                this.Courses.ForEach(c =>
                {
                    c.PropertyChanged -= C_PropertyChanged;
                });
            }
        }

        void saveCommand(object obj)
        {
            var win = obj as CreateClassHourSameOpenView;

            var selectClasses = this.Classes.Where(c => c.IsChecked).ToList();
            if (selectClasses?.Count < 2)
            {
                this.ShowDialog("提示信息", "至少选择两个班级！", CustomControl.Enums.DialogSettingType.OnlyOkButton, CustomControl.Enums.DialogType.Warning);
                return;
            }

            win.Classes = selectClasses;
            win.DialogResult = true;
            //win.IsSave = true;
            //this.ShowDialog("提示信息", "保存成功！", CustomControl.Enums.DialogSettingType.NoButton, CustomControl.Enums.DialogType.None);
        }

        void cancelCommand(object obj)
        {
            var win = obj as CreateClassHourSameOpenView;
            win.DialogResult = false;
        }
    }
}
