using GalaSoft.MvvmLight;
using OSKernel.Presentation.Core.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace OSKernel.Presentation.Login.Summary
{
    public class IndexViewModel : ViewModelBase
    {
        private bool _isUserChecked;
        private bool _isVersionChecked;

        private ContentControl _currentContent;

        public IndexViewModel()
        {

        }

        public bool IsUserChecked
        {
            get
            {
                return _isUserChecked;
            }

            set
            {
                _isUserChecked = value;
                RaisePropertyChanged(() => IsUserChecked);

                if (_isUserChecked)
                    this.CurrentContent = new UserInfoView();
            }
        }

        public bool IsVersionChecked
        {
            get
            {
                return _isVersionChecked;
            }

            set
            {
                _isVersionChecked = value;
                RaisePropertyChanged(() => IsVersionChecked);

                if (_isVersionChecked)
                    this.CurrentContent = new VersionView();
            }
        }

        public ContentControl CurrentContent
        {
            get
            {
                return _currentContent;
            }

            set
            {
                _currentContent = value;
                RaisePropertyChanged(() => CurrentContent);
            }
        }

        public void SelecteItem(string param)
        {
            switch (param)
            {
                case "userInfo":
                    this.IsUserChecked = true;
                    break;
                case "version":
                    this.IsVersionChecked = true;
                    break;
            }
        }
    }
}
