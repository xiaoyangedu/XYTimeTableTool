using OSKernel.Presentation.Arranging.Mixed.Dialog;
using OSKernel.Presentation.Core;
using OSKernel.Presentation.Core.ViewModel;
using OSKernel.Presentation.CustomControl;
using OSKernel.Presentation.Models.Mixed;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Unity;
using XYKernel.OS.Common.Models.Mixed;

namespace OSKernel.Presentation.Arranging.Mixed.Modify.Views
{
    public class TagViewModel : CommonViewModel, IInitilize, IRefresh
    {
        private bool _allChecked;

        private ObservableCollection<UITag> _tags;

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

        /// <summary>
        /// 选中全部
        /// </summary>
        public bool AllChecked
        {
            get
            {
                return _allChecked;
            }

            set
            {
                _allChecked = value;
                RaisePropertyChanged(() => AllChecked);

                foreach (var t in Tags)
                {
                    t.IsChecked = value;
                }
            }
        }

        /// <summary>
        /// 标签
        /// </summary>
        public ObservableCollection<UITag> Tags
        {
            get
            {
                return _tags;
            }

            set
            {
                _tags = value;
                RaisePropertyChanged(() => Tags);
            }
        }

        public TagViewModel()
        {
            this.Tags = new ObservableCollection<UITag>();
        }

        [InjectionMethod]
        public void Initilize()
        {
            var cl = base.GetClCase(base.LocalID);

            var tags = (from s in cl.Tags
                        select new UITag()
                        {
                            ID = s.ID,
                            Name = s.Name,
                        })?.ToList();

            if (tags != null)
            {
                this.Tags = new ObservableCollection<UITag>(tags);
            }
        }

        void createCommand()
        {
            CreateTagWindow window = new CreateTagWindow();
            window.Closed += (s, args) =>
            {
                if (window.DialogResult.Value)
                {
                    var has = this.Tags.Any(c =>
                    {
                        return window.Tags.Any(cc => cc.Equals(c.Name));
                    });

                    if (has)
                    {
                        var result = this.ShowDialog("提示信息", "存在相同标签,是否继续添加", CustomControl.Enums.DialogSettingType.OkAndCancel, CustomControl.Enums.DialogType.Warning);
                        if (result != CustomControl.Enums.DialogResultType.OK)
                        {
                            return;
                        }
                    }

                    var cl = base.GetClCase(base.LocalID);

                    foreach (var t in window.Tags)
                    {
                        var tagID = this.Tags.Count == 0 ? 0 : this.Tags.Max(tt => Convert.ToInt64(tt.ID));

                        // 创建
                        TagModel tag = new TagModel()
                        {
                            ID = (tagID + 1).ToString(),
                            Name = t,
                        };

                        // 更新UI
                        this.Tags.Add(new UITag
                        {
                            ID = tag.ID,
                            Name = tag.Name,
                        });

                        // 更新缓存
                        cl.Tags.Add(tag);
                    }

                    base.Serialize(cl, LocalID);
                }
            };
            window.ShowDialog();
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        void batchDeleteCommand()
        {
            var dialog = this.ShowDialog("提示信息", "确认删除?", CustomControl.Enums.DialogSettingType.OkAndCancel, CustomControl.Enums.DialogType.Warning);
            if (dialog == CustomControl.Enums.DialogResultType.OK)
            {
                var cl = base.GetClCase(base.LocalID);

                var todeletes = this.Tags.Where(t => t.IsChecked)?.ToList();
                if (todeletes != null)
                {
                    foreach (var t in todeletes)
                    {
                        // 更新UI
                        this.Tags.Remove(t);
                        // 移除教师
                        cl.Tags.RemoveAll(tag => tag.ID.Equals(t.ID));
                    }

                    // 保存
                    base.Serialize(cl, LocalID);
                }
            }
        }

        public void Refresh()
        {
            this.Initilize();
        }
    }
}
