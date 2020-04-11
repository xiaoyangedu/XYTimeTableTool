using GalaSoft.MvvmLight;
using OSKernel.Presentation.Core;
using OSKernel.Presentation.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSKernel.Presentation.Login.Summary
{
    public class VersionViewModel : ViewModelBase
    {
        private string _version;

        public VersionViewModel()
        {
            this.Version = $"{CacheManager.Instance.Version.Version}{CacheManager.Instance.Version.VersionType.GetLocalDescription()}";
        }

        public string Version
        {
            get
            {
                return _version;
            }

            set
            {
                _version = value;
                RaisePropertyChanged(() => Version);
            }
        }
    }
}
