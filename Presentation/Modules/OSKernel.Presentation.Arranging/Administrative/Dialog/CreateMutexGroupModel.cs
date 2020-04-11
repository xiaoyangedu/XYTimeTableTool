using OSKernel.Presentation.Core;
using OSKernel.Presentation.Core.ViewModel;
using OSKernel.Presentation.CustomControl;
using OSKernel.Presentation.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Unity;

namespace OSKernel.Presentation.Arranging.Administrative.Dialog
{
    public class CreateMutexGroupModel : CommonViewModel, IInitilize
    {
        private List<UICourse> _courses;

        private int _weight = 100;

        /// <summary>
        /// 课程
        /// </summary>
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

        public int Weight
        {
            get
            {
                return _weight;
            }

            set
            {
                _weight = value;
                RaisePropertyChanged(() => Weight);
            }
        }

        public CreateMutexGroupModel()
        {
            this.Courses = new List<UICourse>();
        }

        [InjectionMethod]
        public void Initilize()
        {
            var cp = CommonDataManager.GetCPCase(base.LocalID);
            if (cp == null)
                return;

            this.Courses = cp.Courses.Select(s =>
              {
                  return new UICourse()
                  {
                      ID = s.ID,
                      Name = s.Name,
                  };
              })?.ToList();

        }

        void save(object obj)
        {
            var has = this.Courses.Count(c => c.IsChecked)>1;
            if (!has)
            {
                this.ShowDialog("提示信息", "至少选两门课程！", CustomControl.Enums.DialogSettingType.OnlyOkButton, CustomControl.Enums.DialogType.Warning);
                return;
            }

            CreateMutexGroup win = obj as CreateMutexGroup;
            win.Courses = this.Courses.Where(c => c.IsChecked)?.ToList();
            win.Weight = this.Weight;
            win.DialogResult = true;
        }

        void cancel(object obj)
        {
            CreateMutexGroup win = obj as CreateMutexGroup;
            win.DialogResult = false;
        }
    }

}
