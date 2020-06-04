using GalaSoft.MvvmLight;
using OSKernel.Presentation.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OSKernel.Presentation.Login.Summary
{
    public class UserInfoViewModel : ViewModelBase
    {
        private long _id;
        private string _name;
        private string _type;

        public ICommand ModifyCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand(Modify);
            }
        }

        /// <summary>
        /// ID
        /// </summary>
        public long Id
        {
            get
            {
                return _id;
            }

            set
            {
                _id = value;
                RaisePropertyChanged(() => Id);
            }
        }

        /// <summary>
        /// 姓名
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                _name = value;
                RaisePropertyChanged(() => Name);
            }
        }

        public string Type
        {
            get
            {
                return _type;
            }

            set
            {
                _type = value;
                RaisePropertyChanged(() => Type);
            }
        }

        public UserInfoViewModel()
        {
            this.Id = CacheManager.Instance.LoginUser.ID;
            this.Name = CacheManager.Instance.LoginUser.UserName;
            this.Type = CacheManager.Instance.LoginUser.IsAnnual == true ? "年费" : "预付费";
        }

        public void Modify()
        {
            Configuration cfa = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            var adress = cfa.AppSettings.Settings["xy:login.address"].Value;
            var port = cfa.AppSettings.Settings["xy:login.port"].Value;

            var url = $@"http://{adress}:{port}/home/login";
            System.Diagnostics.Process.Start(url);
        }
    }
}
