using OSKernel.Presentation.Core.ViewModel;
using OSKernel.Presentation.Models.Administrative;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OSKernel.Presentation.Arranging.Dialog
{
    public class ClassHourSettingWindowModel : CommonViewModel
    {
        public ICommand ChooseTeacherCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<object>(chooseClassHour);
            }
        }

        public ClassHourSettingWindowModel()
        {

        }

        void chooseClassHour(object obj)
        {
            UIClassHour classHour = obj as UIClassHour;
        }
    }
}
