using GalaSoft.MvvmLight.Messaging;
using OSKernel.Presentation.Arranging.Administrative.Modify.Rule.Course.Model;
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

namespace OSKernel.Presentation.Arranging.Administrative.Modify.Rule.Course
{
    public class OddDualWeekViewModel : CommonViewModel, IInitilize
    {
        private ObservableCollection<UIOddDualWeek> _rules;

        public ICommand CreateCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand(createCommand);
            }
        }

        public ICommand DeleteRuleCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<object>(deleteRule);
            }
        }

        public ICommand UserControlCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<string>(userControlCommand);
            }
        }

        public ObservableCollection<UIOddDualWeek> Rules
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

        public OddDualWeekViewModel()
        {
            this.Rules = new ObservableCollection<UIOddDualWeek>();
        }

        [InjectionMethod]
        public void Initilize()
        {
            Messenger.Default.Register<HostView>(this, save);

            this.Comments = CommonDataManager.GetAdminRuleComments(AdministrativeRuleEnum.OddDualWeek);

            var cp = CommonDataManager.GetCPCase(base.LocalID);
            var rule = CommonDataManager.GetAminRule(base.LocalID);

            rule.OddDualWeeks.ForEach(m =>
            {
                UIOddDualWeek oddDual = new UIOddDualWeek();
                oddDual.OddCourseID = m.OddWeekCourseID;
                oddDual.DualCourseID = m.DualWeekCourseID;
                oddDual.ClassID = m.ClassID;

                var firstClass = cp.Classes.FirstOrDefault(c => c.ID.Equals(m.ClassID));
                oddDual.ClassName = firstClass?.Name;

                var oddCourse = cp.Courses.FirstOrDefault(c => c.ID.Equals(m.OddWeekCourseID));
                oddDual.OddCourse = oddCourse?.Name;

                var dualCourse = cp.Courses.FirstOrDefault(c => c.ID.Equals(m.DualWeekCourseID));
                oddDual.DualCourse = dualCourse?.Name;

                this.Rules.Add(oddDual);
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

        void deleteRule(object obj)
        {
            var result = this.ShowDialog("提示信息", "确认删除?", CustomControl.Enums.DialogSettingType.OkAndCancel, CustomControl.Enums.DialogType.Warning);
            if (result == CustomControl.Enums.DialogResultType.OK)
            {
                UIOddDualWeek rule = obj as UIOddDualWeek;
                this.Rules.Remove(rule);
            }
        }

        void createCommand()
        {
            Dialog.CreateOddDualWeek createWin = new Dialog.CreateOddDualWeek();
            createWin.Closed += (s, arg) =>
            {
                if (createWin.DialogResult.Value)
                {
                    var last = this.Rules.LastOrDefault();
                    var index = last == null ? 0 : last.NO;

                    var cp = CommonDataManager.GetCPCase(base.LocalID);

                    var classes = createWin.SelectClasses?.ToList();
                    var odd = createWin.OddCourse;
                    var dual = createWin.DualCourse;

                    classes.ForEach(c =>
                    {
                        var model = new UIOddDualWeek()
                        {
                            NO = ++index,
                            ClassID = c.ID,
                            ClassName = c.Name,
                            OddCourseID = odd?.ID,
                            DualCourseID = dual?.ID,
                        };

                        var dualCourse = cp.Courses.FirstOrDefault(d => d.ID.Equals(model.DualCourseID));
                        var oddCourse = cp.Courses.FirstOrDefault(d => d.ID.Equals(model.OddCourseID));

                        model.OddCourse = oddCourse?.Name;
                        model.DualCourse = dualCourse?.Name;

                        this.Rules.Add(model);
                    });
                }
            };
            createWin.ShowDialog();
        }

        void save(HostView host)
        {
            var rule = CommonDataManager.GetAminRule(base.LocalID);
            var cp = CommonDataManager.GetCPCase(base.LocalID);

            rule.OddDualWeeks.Clear();

            foreach (var r in Rules)
            {
                var oddDualWeek = new XYKernel.OS.Common.Models.Administrative.Rule.OddDualWeekRule()
                {
                    ClassID = r.ClassID,
                    DualWeekCourseID = r.DualCourseID,
                    OddWeekCourseID = r.OddCourseID
                };

                rule.OddDualWeeks.Add(oddDualWeek);
            }

            rule.Serialize(base.LocalID);
            this.ShowDialog("提示信息", "保存成功", CustomControl.Enums.DialogSettingType.NoButton, CustomControl.Enums.DialogType.None);
        }
    }
}
