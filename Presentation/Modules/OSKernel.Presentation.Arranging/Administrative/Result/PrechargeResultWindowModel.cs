using OSKernel.Presentation.Core;
using OSKernel.Presentation.Core.ViewModel;
using OSKernel.Presentation.CustomControl;
using OSKernel.Presentation.Models.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Unity;

namespace OSKernel.Presentation.Arranging.Administrative.Result
{
    /// <summary>
    /// 预充值模式结果
    /// </summary>
    public class PrechargeResultWindowModel : CommonViewModel
    {
        private List<UIPrechargeResult> _precharges;

        /// <summary>
        /// 预览
        /// </summary>
        public List<UIPrechargeResult> Precharges
        {
            get
            {
                return _precharges;
            }

            set
            {
                _precharges = value;
                RaisePropertyChanged(() => Precharges);
            }
        }

        /// <summary>
        /// 确定命令
        /// </summary>
        public ICommand ConfirmCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<object>(Confirm);
            }
        }

        /// <summary>
        /// 放弃命令
        /// </summary>
        public ICommand AbandonCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<object>(Abandon);
            }
        }

        /// <summary>
        /// 关闭命令
        /// </summary>
        public ICommand CloseCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<object>(close);
            }
        }

        public PrechargeResultWindowModel()
        {
            this.Precharges = new List<UIPrechargeResult>();
        }

        public void Initilize(List<string> urls)
        {
            List<UIPrechargeResult> uiPrecharges = new List<UIPrechargeResult>();
            urls.ForEach(url =>
            {
                var lastIndex = url.LastIndexOf('/');
                var total = url.Length;
                var name = url.Substring(lastIndex, total - lastIndex).Replace("/", "").Replace(".jpg", "");

                UIPrechargeResult file = new UIPrechargeResult()
                {
                    Name = name,
                    URL = url
                };
                uiPrecharges.Add(file);
            });

            this.Precharges = uiPrecharges?.OrderBy(p => p.Name)?.ToList();
        }

        /// <summary>
        /// 确定
        /// </summary>
        void Confirm(object obj)
        {
            var confirm = this.ShowDialog("提示信息", "确认使用课表?", CustomControl.Enums.DialogSettingType.OkAndCancel, CustomControl.Enums.DialogType.Warning);

            if (confirm == CustomControl.Enums.DialogResultType.OK)
            {
                PrechargeResultWindow window = obj as PrechargeResultWindow;
                window.IsUseResult = true;
                window.DialogResult = true;
            }
        }

        /// <summary>
        /// 放弃
        /// </summary>
        void Abandon(object obj)
        {
            var confirm = this.ShowDialog("提示信息", "确认放弃使用课表?", CustomControl.Enums.DialogSettingType.OkAndCancel, CustomControl.Enums.DialogType.Warning);

            if (confirm == CustomControl.Enums.DialogResultType.OK)
            {
                PrechargeResultWindow window = obj as PrechargeResultWindow;
                window.IsUseResult = false;
                window.DialogResult = true;
            }
        }

        /// <summary>
        /// 关闭
        /// </summary>
        /// <param name="obj"></param>
        void close(object obj)
        {
            PrechargeResultWindow window = obj as PrechargeResultWindow;
            window.DialogResult = false;
        }
    }
}
