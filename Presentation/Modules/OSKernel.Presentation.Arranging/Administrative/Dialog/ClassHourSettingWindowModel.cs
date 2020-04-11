using GalaSoft.MvvmLight;
using OSKernel.Presentation.Arranging.Dialog;
using OSKernel.Presentation.Core.DataManager;
using OSKernel.Presentation.Core.ViewModel;
using OSKernel.Presentation.Models.Administrative;
using OSKernel.Presentation.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OSKernel.Presentation.Arranging.Administrative.Dialog
{
    public class ClassHourSettingWindowModel : CommonViewModel
    {
        private List<UIClassHour> _classHours;

        public List<UIClassHour> ClassHours
        {
            get
            {
                return _classHours;
            }

            set
            {
                _classHours = value;
                RaisePropertyChanged(() => ClassHours);
            }
        }

        public ICommand SetTeacherCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<object>(setTeacherCommand);
            }
        }

        public ClassHourSettingWindowModel()
        {
            this.ClassHours = new List<UIClassHour>();
        }

        void setTeacherCommand(object obj)
        {
            UIClassHour classHour = obj as UIClassHour;

            var cpCase = CommonDataManager.GetCPCase(base.LocalID);

            var currentTeachers = classHour.Teachers.Select(s =>
            {
                return new UITeacher
                {
                    ID = s.ID,
                    Name = s.Name
                };
            })?.ToList();

            var allTeachers = cpCase.Teachers.Select(s =>
            {
                return new UITeacher
                {
                    ID = s.ID,
                    Name = s.Name
                };
            })?.ToList();

            ChooseTeacherWindow window = new ChooseTeacherWindow(currentTeachers, allTeachers);
            window.Closed += (s, arg) =>
            {
                if (window.DialogResult.Value)
                {
                    // 清空教师
                    //classHour.Teachers.Clear();
                    if (window.Teachers.Count > 0)
                    {
                        var selectedTeachers = window.Teachers.Select(st =>
                        {
                            return new XYKernel.OS.Common.Models.Administrative.TeacherModel()
                            {
                                ID = st.ID,
                                Name = st.Name
                            };
                        })?.ToList();
                        classHour.Teachers = selectedTeachers;
                    }
                    else
                    {
                        classHour.Teachers.Clear();
                        classHour.TeacherString = string.Empty;
                    }
                    classHour.RaiseChanged();
                }
            };
            window.ShowDialog();
        }
    }
}
