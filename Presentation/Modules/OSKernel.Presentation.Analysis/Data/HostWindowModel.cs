using GalaSoft.MvvmLight;
using OSKernel.Presentation.Core;
using OSKernel.Presentation.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSKernel.Presentation.Analysis.Data
{
    /// <summary>
    /// 窗口模型
    /// </summary>
    public class HostWindowModel : ViewModelBase
    {
        private bool _showAdministrative = false;
        private bool _showMixed = false;
        private bool _showDivide = false;

        public bool ShowAdministrative
        {
            get
            {
                return _showAdministrative;
            }

            set
            {
                _showAdministrative = value;
                RaisePropertyChanged(() => ShowAdministrative);
            }
        }

        public bool ShowMixed
        {
            get
            {
                return _showMixed;
            }

            set
            {
                _showMixed = value;
                RaisePropertyChanged(() => ShowMixed);
            }
        }

        public bool ShowDivide
        {
            get
            {
                return _showDivide;
            }

            set
            {
                _showDivide = value;
                RaisePropertyChanged(() => ShowDivide);
            }
        }

        public void Initilize(CaseTypeEnum caseType)
        {
            if (caseType == CaseTypeEnum.Administrative)
            {
                this.ShowAdministrative = true;
            }
            else if (caseType == CaseTypeEnum.Mixed)
            {
                this.ShowMixed = true;
            }
            else if (caseType == CaseTypeEnum.Divide)
            {
                this.ShowDivide = true;
            }
        }
    }
}
