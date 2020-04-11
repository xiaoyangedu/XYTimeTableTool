using OSKernel.Presentation.Arranging.Administrative.Dialog;
using OSKernel.Presentation.Arranging.Dialog;
using OSKernel.Presentation.Core;
using OSKernel.Presentation.Core.ViewModel;
using OSKernel.Presentation.CustomControl;
using OSKernel.Presentation.Models.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using Unity;
using XYKernel.OS.Common.Models.Administrative;

namespace OSKernel.Presentation.Arranging.Administrative.Modify.Views
{
    public class CourseViewModel : CommonViewModel, IInitilize
    {
        private bool _allChecked;

        private CPCase _cpcase;

        private ListCollectionView _courseCollectionView;

        private string _searchCourse;

        private ObservableCollection<UICourse> _courses;

        public ObservableCollection<UICourse> Courses
        {
            get
            {
                return _courses;
            }

            set
            {
                _courses = value;
                RaisePropertyChanged(() => Courses);
            }
        }

        public ICommand CreateCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand(createCommand);
            }
        }

        public ICommand ModifyLevelCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand(modifyLevelCommand);
            }
        }

        public ICommand BatchDeleteCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand(batchDeleteCommand);
            }
        }

        public ICommand SystemCourseCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand(systemCourseCommand);
            }
        }

        public ICommand SetColorCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<object>(setColorCommand);
            }
        }

        public ICommand SaveCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand(saveCommand);
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

                foreach (var t in Courses)
                {
                    t.IsChecked = value;
                }
            }
        }

        /// <summary>
        /// 搜索课程
        /// </summary>
        public string SearchCourse
        {
            get
            {
                return _searchCourse;
            }

            set
            {
                _searchCourse = value;
                RaisePropertyChanged(() => SearchCourse);

                _courseCollectionView.Refresh();
            }
        }

        public CourseViewModel()
        {
            this.Courses = new ObservableCollection<UICourse>();

            _courseCollectionView = (ListCollectionView)CollectionViewSource.GetDefaultView(this.Courses);
            _courseCollectionView.Filter = CourseContains;
        }

        [InjectionMethod]
        public void Initilize()
        {
            var cp = CommonDataManager.GetCPCase(base.LocalID);
            var local = CommonDataManager.GetLocalCase(base.LocalID);

            if (cp != null)
            {
                _cpcase = cp;

                if (cp.Teachers != null)
                {
                    List<UICourse> courses = new List<UICourse>();
                    cp.Courses.ForEach(t =>
                    {
                        UICourse course = new UICourse()
                        {
                            ID = t.ID,
                            Name = t.Name,
                        };

                        var has = local.CourseColors.ContainsKey(t.ID);
                        if (has)
                        {
                            course.ColorString = local.CourseColors[t.ID];
                        }

                        courses.Add(course);
                    });
                    this.Courses = new ObservableCollection<UICourse>(courses);

                    _courseCollectionView = (ListCollectionView)CollectionViewSource.GetDefaultView(this.Courses);
                    _courseCollectionView.Filter = CourseContains;
                }
            }
            else
            {
                _cpcase = new CPCase();
            }
        }

        bool CourseContains(object contain)
        {
            UICourse course = contain as UICourse;
            if (string.IsNullOrEmpty(this.SearchCourse))
                return true;

            if (course.Name.IndexOf(this.SearchCourse) != -1)
            {
                return true;
            }
            else
                return false;
        }

        void createCommand()
        {
            CreateCourseWindow window = new CreateCourseWindow();
            window.Closed += (s, args) =>
            {
                if (window.DialogResult.Value)
                {
                    var has = this.Courses.Any(c =>
                      {
                          return window.Courses.Any(cc => cc.Equals(c.Name));
                      });

                    if (has)
                    {
                        var result = this.ShowDialog("提示信息", "存在相同科目,是否继续添加", CustomControl.Enums.DialogSettingType.OkAndCancel, CustomControl.Enums.DialogType.Warning);
                        if (result != CustomControl.Enums.DialogResultType.OK)
                        {
                            return;
                        }
                    }

                    var local = CommonDataManager.GetLocalCase(base.LocalID);

                    window.Courses.ForEach(c =>
                    {
                        var any = this.Courses.Any(cc => cc.Name.Equals(c));
                        if (!any)
                        {
                            var courseID = this.Courses.Count == 0 ? 0 : this.Courses.Max(cs => Convert.ToInt64(cs.ID));
                            CourseModel course = new CourseModel()
                            {
                                ID = (courseID + 1).ToString(),
                                Name = c
                            };

                            // 更新UI
                            this.Courses.Add(new UICourse
                            {
                                ID = course.ID,
                                Name = course.Name,
                                ColorString = "#000000"
                            });

                            var hasColor = local.CourseColors.ContainsKey(course.ID);
                            if (!hasColor)
                            {
                                local.CourseColors.Add(course.ID, "#000000");
                            }
                            else
                            {
                                local.CourseColors[course.ID] = "#000000";
                            }

                            // 更新缓存
                            _cpcase.Courses.Add(course);
                        }


                    });

                }
            };
            window.ShowDialog();
        }

        void modifyLevelCommand()
        {

        }

        void batchDeleteCommand()
        {
            var confirm = this.ShowDialog("提示信息", "确定删除选中科目?", CustomControl.Enums.DialogSettingType.OkAndCancel, CustomControl.Enums.DialogType.Warning);
            if (confirm == CustomControl.Enums.DialogResultType.OK)
            {
                // 验证是否确认删除?
                var toDeletes = this.Courses.Where(t => t.IsChecked)?.ToList();
                if (toDeletes != null)
                {
                    var local = CommonDataManager.GetLocalCase(base.LocalID);

                    foreach (var t in toDeletes)
                    {
                        // 更新UI
                        this.Courses.Remove(t);

                        _cpcase.Courses.RemoveAll(c => c.ID.Equals(t.ID));
                        _cpcase.Classes?.ForEach(c =>
                        {
                            c.Settings?.RemoveAll(cs => cs.CourseID.Equals(t.ID));
                        });

                        // 移除颜色
                        local.CourseColors.Remove(t.ID);

                        // 移除课程
                        GalaSoft.MvvmLight.Messaging.Messenger.Default.Send<UICourse>(t);

                        AdministrativeDataHelper.CourseChanged(t, base.LocalID, CommonDataManager);
                    }
                    // 保存
                    _cpcase.Serialize(base.LocalID);
                    local.Serialize();
                }
            }
        }

        void systemCourseCommand()
        {
            SystemCourseWindow window = new SystemCourseWindow();
            window.Closed += (s, arg) =>
            {
                if (window.DialogResult.Value)
                {
                    var has = this.Courses.Any(c =>
                    {
                        return window.Courses.Any(cc => cc.Name.Equals(c.Name));
                    });

                    if (has)
                    {
                        var result = this.ShowDialog("提示信息", "存在相同科目,是否继续添加", CustomControl.Enums.DialogSettingType.OkAndCancel, CustomControl.Enums.DialogType.Warning);
                        if (result != CustomControl.Enums.DialogResultType.OK)
                        {
                            return;
                        }
                    }

                    var local = CommonDataManager.GetLocalCase(base.LocalID);

                    window.Courses.ForEach(c =>
                    {
                        var any = this.Courses.Any(cc => cc.Name.Equals(c.Name));
                        if (!any)
                        {
                            var courseID = this.Courses.Count == 0 ? 0 : this.Courses.Max(cs => Convert.ToInt64(cs.ID));
                            CourseModel course = new CourseModel()
                            {
                                ID = (courseID + 1).ToString(),
                                Name = c.Name
                            };

                            // 更新UI
                            this.Courses.Add(new UICourse
                            {
                                ID = course.ID,
                                Name = course.Name,
                                ColorString = c.ColorString
                            });

                            // 更新缓存
                            _cpcase.Courses.Add(course);

                            var hasColor = local.CourseColors.ContainsKey(course.ID);
                            if (!hasColor)
                            {
                                local.CourseColors.Add(course.ID, c.ColorString);
                            }
                            else
                            {
                                local.CourseColors[course.ID] = c.ColorString;
                            }
                        }
                    });

                    local.Serialize();
                    _cpcase.Serialize(base.LocalID);
                }
            };
            window.ShowDialog();
        }

        void setColorCommand(object obj)
        {
            UICourse model = obj as UICourse;
            var local = CommonDataManager.GetLocalCase(base.LocalID);

            ColorDialog colorDialog = new ColorDialog();
            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                SolidBrush sb = new SolidBrush(colorDialog.Color);
                SolidColorBrush solidColorBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(sb.Color.A, sb.Color.R, sb.Color.G, sb.Color.B));

                var colorString = solidColorBrush.ToString();

                model.ColorString = colorString;

                var has = local.CourseColors.ContainsKey(model.ID);
                if (!has)
                {
                    local.CourseColors.Add(model.ID, colorString);
                }
                else
                {
                    local.CourseColors[model.ID] = colorString;
                }

                // 保存方案
                local.Serialize();
            }
        }

        void saveCommand()
        {

            var local = CommonDataManager.GetLocalCase(base.LocalID);
            local.Serialize();

            _cpcase.Serialize(base.LocalID);

            this.ShowDialog("提示信息", "保存成功", CustomControl.Enums.DialogSettingType.NoButton, CustomControl.Enums.DialogType.None);
        }
    }
}
