using GalaSoft.MvvmLight.Messaging;
using OSKernel.Presentation.Core;
using OSKernel.Presentation.Core.ViewModel;
using OSKernel.Presentation.CustomControl;
using OSKernel.Presentation.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Unity;

namespace OSKernel.Presentation.Arranging.Mixed.Modify.Rule.Room
{
    /// <summary>
    /// 可用教室数
    /// </summary>
    public class AvaliableRoomViewModel : CommonViewModel, IInitilize
    {
        private int _roomLimit;

        /// <summary>
        /// 可用教室
        /// </summary>
        public int RoomLimit
        {
            get
            {
                return _roomLimit;
            }

            set
            {
                _roomLimit = value;
                RaisePropertyChanged(() => RoomLimit);
            }
        }

        public ICommand UserControlCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<string>(userControlCommand);
            }
        }

        [InjectionMethod]
        public void Initilize()
        {
            var cl = base.GetClCase(base.LocalID);
            this.RoomLimit = cl.RoomLimit;

            this.Comments = CommonDataManager.GetMixedRuleComments(MixedRuleEnum.AvailableRoom);
        }

        public AvaliableRoomViewModel()
        {
            Messenger.Default.Register<HostView>(this, save);
        }

        void userControlCommand(string parms)
        {
            if (parms.Equals("loaded"))
            {

            }
            else if (parms.Equals("unloaded"))
            {
                Messenger.Default.Unregister<HostView>(this, save);
            }
        }

        void save(HostView host)
        {
            var cl = base.GetClCase(base.LocalID);
            cl.RoomLimit = this.RoomLimit;

            base.Serialize(cl, base.LocalID);

            this.ShowDialog("提示信息", "保存成功!", CustomControl.Enums.DialogSettingType.NoButton, CustomControl.Enums.DialogType.None);
        }
    }
}
