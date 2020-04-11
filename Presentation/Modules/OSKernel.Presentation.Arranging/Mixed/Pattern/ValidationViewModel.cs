using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OSKernel.Presentation.Arranging.Mixed.Pattern
{
    public class ValidationViewModel : GalaSoft.MvvmLight.ObservableObject
    {
        private bool _onlyTeacher = true;

        private bool _includeAssinged;

        /// <summary>
        /// 只包含教师
        /// </summary>
        public bool OnlyTeacher
        {
            get
            {
                return _onlyTeacher;
            }

            set
            {
                _onlyTeacher = value;
                RaisePropertyChanged(() => OnlyTeacher);
            }
        }

        /// <summary>
        /// 包含所有已经分配的
        /// </summary>
        public bool IncludeAssinged
        {
            get
            {
                return _includeAssinged;
            }

            set
            {
                _includeAssinged = value;
                RaisePropertyChanged(() => IncludeAssinged);
            }
        }

        public ICommand ChooseCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<string>(chooseCommand);
            }
        }

        public ValidationViewModel()
        {

        }

        void chooseCommand(string type)
        {
            if (type.Equals("1"))
            {
                this.OnlyTeacher = true;
            }
            else if (type.Equals("2"))
            {
                this.IncludeAssinged = true;
            }
        }
    }
}
