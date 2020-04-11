using GalaSoft.MvvmLight.Messaging;
using OSKernel.Presentation.Arranging.Administrative.Modify.Rule.Teacher.Dialog;
using OSKernel.Presentation.Arranging.Administrative.Modify.Rule.Teacher.Model;
using OSKernel.Presentation.Core;
using OSKernel.Presentation.Core.ViewModel;
using OSKernel.Presentation.CustomControl;
using OSKernel.Presentation.Models.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Unity;

namespace OSKernel.Presentation.Arranging.Administrative.Modify.Rule.Teacher
{
    public class MasterApprentticeViewModel : CommonViewModel, IInitilize
    {
        private ObservableCollection<UIMasterApprenttice> _rules;

        private bool _isCheckedAll;

        public ICommand UserControlCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<string>(userControlCommand);
            }
        }

        public ICommand CreateCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand(createCommand);
            }
        }

        public ICommand BatchDeleteCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand(batchDeleteCommand);
            }
        }

        public bool IsCheckedAll
        {
            get
            {
                return _isCheckedAll;
            }

            set
            {
                _isCheckedAll = value;
                RaisePropertyChanged(() => IsCheckedAll);

                foreach (var r in this.Rules)
                {
                    r.IsChecked = value;
                }
            }
        }

        public ObservableCollection<UIMasterApprenttice> Rules
        {
            get
            {
                return _rules;
            }

            set
            {
                _rules = value;
                RaisePropertyChanged(() => Rules);
            }
        }

        public MasterApprentticeViewModel()
        {
            this.Rules = new ObservableCollection<UIMasterApprenttice>();

            base.Weights = new Dictionary<string, WeightTypeEnum>()
            {
                { "高", WeightTypeEnum.Hight},
                { "中", WeightTypeEnum.Medium},
                { "低", WeightTypeEnum.Low},
            };

            base.SelectWeight = WeightTypeEnum.Hight;
        }

        [InjectionMethod]
        public void Initilize()
        {
            this.Comments = CommonDataManager.GetAdminRuleComments(AdministrativeRuleEnum.MasterApprenttice);

            Messenger.Default.Register<HostView>(this, save);

            var cp = CommonDataManager.GetCPCase(base.LocalID);
            var rule = CommonDataManager.GetAminRule(base.LocalID);

            rule.MasterApprenttices.ForEach(m =>
            {
                var course = cp.Courses.FirstOrDefault(c => c.ID.Equals(m.CourseID));
                var master = cp.Teachers.FirstOrDefault(t => t.ID.Equals(m.MasterID));

                UIMasterApprenttice uirule = new UIMasterApprenttice();
                uirule.CourseID = m.CourseID;
                uirule.MasterID = m.MasterID;
                uirule.Master = master?.Name;
                uirule.Course = course?.Name;
                uirule.ApprentticeID = cp.GetTeachersByIds(m.ApprenticeIDs);
                uirule.Weight = (Models.Enums.WeightTypeEnum)m.Weight;

                this.Rules.Add(uirule);
            });
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
            var rule = CommonDataManager.GetAminRule(base.LocalID);

            // 清除徒弟集合
            rule.MasterApprenttices.Clear();

            foreach (var r in Rules)
            {
                var ma = new XYKernel.OS.Common.Models.Administrative.Rule.MasterApprenticeRule();
                ma.CourseID = r.CourseID;
                ma.MasterID = r.MasterID;
                ma.Weight = (int)r.Weight;
                ma.ApprenticeIDs = r.ApprentticeID?.Select(a => a.ID)?.ToList();

                rule.MasterApprenttices.Add(ma);
            }

            rule.Serialize(base.LocalID);
            this.ShowDialog("提示信息", "保存成功", CustomControl.Enums.DialogSettingType.NoButton, CustomControl.Enums.DialogType.None);
        }

        void createCommand()
        {
            CreateMasterApprentticeWindow masterWindow = new CreateMasterApprentticeWindow();
            masterWindow.Closed += (s, arg) =>
            {
                if (masterWindow.DialogResult.Value)
                {
                    var master = masterWindow.Rule;
                    if (this.Rules.Any(r => r.MasterID.Equals(master.MasterID)))
                    {
                        this.ShowDialog("提示信息", "存在该教师", CustomControl.Enums.DialogSettingType.NoButton, CustomControl.Enums.DialogType.None);
                        return;
                    }
                    else
                    {
                        this.Rules.Add(master);
                    }
                }
            };
            masterWindow.ShowDialog();
        }

        void batchDeleteCommand()
        {
            var selectedRules = this.Rules.Where(r => r.IsChecked)?.ToList();

            if (selectedRules?.Count == 0)
                return;

            var rule = CommonDataManager.GetAminRule(base.LocalID);

            selectedRules?.ForEach(r =>
            {
                rule.MasterApprenttices.RemoveAll(ma => ma.MasterID.Equals(r.MasterID));
                this.Rules.Remove(r);
            });

            rule.Serialize(base.LocalID);
            this.ShowDialog("提示信息", "删除成功", CustomControl.Enums.DialogSettingType.NoButton, CustomControl.Enums.DialogType.None);
        }

        public override void BatchSetWeight(WeightTypeEnum weightEnum)
        {
            foreach (var r in this.Rules)
            {
                r.Weight = weightEnum;
            }
        }
    }
}
