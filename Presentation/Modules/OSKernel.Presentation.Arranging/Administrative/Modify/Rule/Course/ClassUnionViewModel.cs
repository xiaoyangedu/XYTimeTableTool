using GalaSoft.MvvmLight.Messaging;
using OSKernel.Presentation.Arranging.Administrative.Dialog;
using OSKernel.Presentation.Arranging.Administrative.Modify.Rule.Course.Model;
using OSKernel.Presentation.Core;
using OSKernel.Presentation.Core.ViewModel;
using OSKernel.Presentation.CustomControl;
using OSKernel.Presentation.Models.Base;
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
    public class ClassUnionViewModel : CommonViewModel, IInitilize
    {
        ObservableCollection<UIClassUnion> _rules;

        public ICommand CreateCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand(createCommand);
            }
        }

        public ICommand UserControlCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<string>(userControlCommand);
            }
        }

        public ICommand DeleteRuleCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<object>(deleteRule);
            }
        }

        public ObservableCollection<UIClassUnion> Rules
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

        public ClassUnionViewModel()
        {
            this.Rules = new ObservableCollection<UIClassUnion>();
        }

        [InjectionMethod]
        public void Initilize()
        {
            Messenger.Default.Register<HostView>(this, save);

            this.Comments = CommonDataManager.GetAdminRuleComments(AdministrativeRuleEnum.ClassUnion);


            var cp = CommonDataManager.GetCPCase(base.LocalID);
            var rule = CommonDataManager.GetAminRule(base.LocalID);

            rule.ClassUnions.ForEach(u =>
            {
                UIClassUnion model = new UIClassUnion();
                var course = cp.Courses.FirstOrDefault(c => c.ID.Equals(u.CourseID));
                if (course != null)
                {
                    model.CourseID = course.ID;
                    model.CourseName = course.Name;
                }

                var classes = (from c in cp.Classes
                               from cc in u.ClassIDs
                               where c.ID.Equals(cc)
                               select new UIClass()
                               {
                                   ID = c.ID,
                                   Name = c.Name,
                               })?.ToList();
                model.Classes = classes;

                this.Rules.Add(model);
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

        void createCommand()
        {
            CreateClassUnion win = new CreateClassUnion();
            win.Closed += (s, arg) =>
            {
                if (win.DialogResult.Value)
                {
                    UIClassUnion model = new UIClassUnion()
                    {
                        Classes = win.SelectClasses,
                        CourseID = win.SelectCourse.ID,
                        CourseName = win.SelectCourse.Name,
                    };
                    this.Rules.Add(model);
                }
            };
            win.ShowDialog();
        }

        void deleteRule(object obj)
        {
            var result = this.ShowDialog("提示信息", "确认删除?", CustomControl.Enums.DialogSettingType.OkAndCancel, CustomControl.Enums.DialogType.Warning);
            if (result == CustomControl.Enums.DialogResultType.OK)
            {
                UIClassUnion rule = obj as UIClassUnion;
                this.Rules.Remove(rule);
            }
        }

        void save(HostView host)
        {
            var rule = CommonDataManager.GetAminRule(base.LocalID);
            var cp = CommonDataManager.GetCPCase(base.LocalID);

            rule.ClassUnions.Clear();

            foreach (var r in Rules)
            {
                var classUnion = new XYKernel.OS.Common.Models.Administrative.Rule.ClassUnionRule()
                {
                    ClassIDs = r.Classes?.Select(c => c.ID)?.ToList(),
                    CourseID = r.CourseID
                };

                rule.ClassUnions.Add(classUnion);
            }

            rule.Serialize(base.LocalID);
            this.ShowDialog("提示信息", "保存成功", CustomControl.Enums.DialogSettingType.NoButton, CustomControl.Enums.DialogType.None);
        }
    }
}
