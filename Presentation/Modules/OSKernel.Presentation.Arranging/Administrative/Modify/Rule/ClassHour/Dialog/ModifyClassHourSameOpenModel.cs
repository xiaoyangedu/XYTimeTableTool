using OSKernel.Presentation.Core;
using OSKernel.Presentation.Core.ViewModel;
using OSKernel.Presentation.CustomControl;
using OSKernel.Presentation.Models.Administrative;
using OSKernel.Presentation.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Unity;

namespace OSKernel.Presentation.Arranging.Administrative.Modify.Rule.ClassHour.Dialog
{
    public class ModifyClassHourSameOpenModel : CommonViewModel, IInitilize
    {
        private List<UIClass> _classes;

        /// <summary>
        /// 保存命令
        /// </summary>
        public ICommand SaveCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<object>(saveCommand);
            }
        }

        /// <summary>
        /// 取消命令
        /// </summary>
        public ICommand CancelCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<object>(cancelCommand);
            }
        }

        /// <summary>
        /// 班级列表
        /// </summary>
        public List<UIClass> Classes
        {
            get
            {
                return _classes;
            }

            set
            {
                _classes = value;
                RaisePropertyChanged(() => Classes);
            }
        }

        public ModifyClassHourSameOpenModel()
        {

        }

        [InjectionMethod]
        public void Initilize()
        {

        }

        void saveCommand(object obj)
        {
            ModifyClassHourSameOpen win = obj as ModifyClassHourSameOpen;

            var groups = (from c in this.Classes from cc in c.HourIndexs where !c.ID.Equals("0") select cc)?.GroupBy(cc => cc.Index);
            if (groups != null)
            {
                // 如果存在少于两个的选项则提示信息
                var has = groups.Any(g => g.ToList().Count(gg => gg.IsChecked) == 1);
                if (has)
                {
                    this.ShowDialog("提示信息", "只有选择两个以上才有效!", CustomControl.Enums.DialogSettingType.OnlyOkButton, CustomControl.Enums.DialogType.Warning);
                    return;
                }
            }

            // 1.移除一行记录
            this.Classes.RemoveAll(c => c.ID.Equals("0"));

            var allUnSelected = this.Classes.All(c => c.HourIndexs.All(h => h.IsChecked == false));
            if (allUnSelected)
            {
                this.ShowDialog("提示信息", "至少要选择两个课时！", CustomControl.Enums.DialogSettingType.OnlyOkButton, CustomControl.Enums.DialogType.Warning);
                return;
            }

            // 2.选择班级
            win.Classes = this.Classes;
            // 3.关闭当前窗口
            win.DialogResult = true;
        }

        void cancelCommand(object obj)
        {
            ModifyClassHourSameOpen win = obj as ModifyClassHourSameOpen;
            win.DialogResult = false;
        }

        public void SetClasses(List<UIClass> classes)
        {
            UIClass header = new UIClass()
            {
                ID = "0",
                Name = string.Empty,
                HourIndexs = new List<Models.Administrative.UIClassHourIndex>(),
            };

            var maxIndex = classes.Max(c => c.HourIndexs.Count);
            for (int i = 1; i <= maxIndex; i++)
            {
                var classHour = new Models.Administrative.UIClassHourIndex()
                {
                    Index = i
                };
                header.HourIndexs.Add(classHour);

                classHour.PropertyChanged += ClassHour_PropertyChanged;
            }

            // 临时班级
            List<UIClass> tempClasses = new List<UIClass>();
            tempClasses.AddRange(classes.Select(c => c.DeepClone()));
            tempClasses.Insert(0, header);

            // 班级列表
            this.Classes = tempClasses;

            // 绑定头部选中状态
            var totalIndex = header.HourIndexs.Count;

            header.HourIndexs.ForEach(hi =>
            {
                var indexes = new List<UIClassHourIndex>();

                this.Classes.Where(c => !c.ID.Equals("0"))?.ToList().ForEach(c =>
                {
                    if (c.HourIndexs.Count >= hi.Index)
                    {
                        indexes.Add(c.HourIndexs[hi.Index - 1]);
                    }
                });

                if (indexes.Count >= 2)
                {
                    var all = indexes.All(i => i.IsChecked);
                    if (all)
                    {
                        hi.IsChecked = true;
                    }
                }
                else
                {
                    hi.IsChecked = false;
                }
            });
        }

        private void ClassHour_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            UIClassHourIndex hourIndex = sender as UIClassHourIndex;
            if (e.PropertyName.Equals(nameof(hourIndex.IsChecked)))
            {
                this.Classes.ForEach(c =>
                {
                    if (!c.ID.Equals("0"))
                    {
                        if (c.HourIndexs.Count >= hourIndex.Index)
                        {
                            c.HourIndexs[hourIndex.Index - 1].IsChecked = hourIndex.IsChecked;
                        }
                    }
                });
            }
        }
    }
}
