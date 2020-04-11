using OSKernel.Presentation.Core;
using OSKernel.Presentation.Core.ViewModel;
using OSKernel.Presentation.Models.Mixed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Unity;
using XYKernel.OS.Common.Models.Mixed;
using XYKernel.OS.Common.Models.Pattern.Extend;

namespace OSKernel.Presentation.Arranging.Mixed.Pattern.Operator
{
    public class ClassCapacityViewModel : CommonViewModel, IInitilize
    {
        private int _no = 0;
        private int _fromCapacity;
        private int _toCapacity;
        private int _nowCapacity;
        private int _allCapacity;

        private List<UIClass> _classes;

        public int FromCapacity
        {
            get
            {
                return _fromCapacity;
            }

            set
            {
                if (value < 0)
                {
                    _fromCapacity = 0;
                    RaisePropertyChanged(() => FromCapacity);
                    return;
                }

                if (value > 9999)
                {
                    _fromCapacity = 9999;
                    RaisePropertyChanged(() => FromCapacity);
                    return;
                }

                _fromCapacity = value;
                RaisePropertyChanged(() => FromCapacity);
            }
        }

        public int ToCapacity
        {
            get
            {
                return _toCapacity;
            }

            set
            {
                if (value < 0)
                {
                    _toCapacity = 0;
                    RaisePropertyChanged(() => ToCapacity);
                    return;
                }

                if (value > 9999)
                {
                    _toCapacity = 9999;
                    RaisePropertyChanged(() => ToCapacity);
                    return;
                }

                _toCapacity = value;
                RaisePropertyChanged(() => ToCapacity);
            }
        }

        public int NowCapacity
        {
            get
            {
                return _nowCapacity;
            }

            set
            {
                if (value < 0)
                {
                    _nowCapacity = 0;
                    RaisePropertyChanged(() => NowCapacity);
                    return;
                }

                if (value > 9999)
                {
                    _nowCapacity = 9999;
                    RaisePropertyChanged(() => NowCapacity);
                    return;
                }

                _nowCapacity = value;
                RaisePropertyChanged(() => NowCapacity);
            }
        }

        public int AllCapacity
        {
            get
            {
                return _allCapacity;
            }

            set
            {
                if (value < 0)
                {
                    _allCapacity = 0;
                    RaisePropertyChanged(() => AllCapacity);
                    return;
                }

                if (value > 9999)
                {
                    _allCapacity = 9999;
                    RaisePropertyChanged(() => AllCapacity);
                    return;
                }

                _allCapacity = value;
                RaisePropertyChanged(() => AllCapacity);
            }
        }

        /// <summary>
        /// 班级
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

        public ICommand SettingCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<object>(setting);
            }
        }

        public ClassCapacityViewModel()
        {
            this.Classes = new List<UIClass>();
        }

        private void setting(object type)
        {
            var value = Convert.ToInt32(type);

            if (value == 1)
            {
                var lessThan = this.Classes.Where(r => r.Capacity < FromCapacity);
                if (lessThan != null)
                {
                    foreach (var less in lessThan)
                    {
                        less.Capacity = ToCapacity;
                    }
                }

            }
            else if (value == 2)
            {
                this.Classes?.ForEach(r => r.Capacity += NowCapacity);
            }
            else if (value == 3)
            {
                this.Classes?.ForEach(r => r.Capacity = AllCapacity);
            }
        }

        [InjectionMethod]
        public void Initilize()
        {
            CLCase clModel = CommonDataManager.GetCLCase(base.LocalID);

            List<UIClass> classes = new List<UIClass>();
            clModel.Courses.ForEach(c =>
            {
                var values = clModel.GetClasses(c.ID);
                foreach (var v in values)
                {
                    v.NO = ++_no;
                    classes.Add(v);
                }
            });

            this.Classes = classes;
        }

        /// <summary>
        /// 获取班级额度
        /// </summary>
        /// <returns></returns>
        public List<ClassCapacityModel> GetCapacitys()
        {
            return this.Classes.Select(c =>
             {
                 ClassCapacityModel classCapacity = new ClassCapacityModel()
                 {
                     Capacity = c.Capacity,
                     ClassId = c.ID
                 };
                 return classCapacity;
             })?.ToList();
        }
    }
}
