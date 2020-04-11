using OSKernel.Presentation.Arranging.Mixed.Pattern.Model;
using OSKernel.Presentation.Core;
using OSKernel.Presentation.Core.ViewModel;
using OSKernel.Presentation.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using XYKernel.OS.Common.Models.Mixed;
using XYKernel.OS.Common.Models.Pattern.Extend;

namespace OSKernel.Presentation.Arranging.Mixed.Pattern.Operator
{
    public class RemoveCombinationViewModel : CommonViewModel, IInitilize
    {
        private bool _isCheckedAll;

        private List<UICombination> _combinations;

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

                this.Combinations.ForEach(c =>
                {
                    c.IsChecked = _isCheckedAll;
                });
            }
        }

        public RemoveCombinationViewModel()
        {
            this.Combinations = new List<UICombination>();
        }

        public List<UICombination> Combinations
        {
            get
            {
                return _combinations;
            }

            set
            {
                _combinations = value;
                RaisePropertyChanged(() => Combinations);
            }
        }

        [InjectionMethod]
        public void Initilize()
        {
            CLCase caseModel = CommonDataManager.GetCLCase(base.LocalID);

            var courseLevels = (from c in caseModel.Courses
                                from cl in c.Levels
                                select new
                                {
                                    CourseID = c.ID,
                                    Course = c.Name,
                                    LevelID = cl.ID,
                                    Level = cl.Name,
                                    Display = $"{c.Name}|{cl.Name}"
                                });

            var groups = caseModel.Students.GroupBy(s =>
              {
                  return (from sp in s.Preselections
                          from cl in courseLevels
                          where cl.LevelID.Equals(sp.LevelID) && cl.CourseID.Equals(sp.CourseID)
                          select new
                          {
                              display = $"{cl.Course}{cl.Level}"
                          }).OrderBy(d => d.display).Select(d => d.display).Parse();
              });

            if (groups != null)
            {
                List<UICombination> combinations = new List<UICombination>();
                int no = 0;
                foreach (var g in groups)
                {
                    if (g.All(gg => gg.Preselections.Count == 0))
                        continue;


                    UICombination combination = new UICombination();
                    combination.NO = ++no;
                    combination.Combination = g.Key;
                    combination.Students = g.Count();

                    var selection = g.First();
                    combination.Selections = selection.Preselections.Select(sp =>
                    {
                        return new XYKernel.OS.Common.Models.Pattern.Extend.SelectionModel()
                        {
                            CourseId = sp.CourseID,
                            LevelId = sp.LevelID
                        };
                    })?.ToList();

                    combinations.Add(combination);
                };

                // 绑定去除组合
                this.Combinations = combinations;
            }
        }

        /// <summary>
        /// 获取要删除的组合
        /// </summary>
        /// <returns>要删除的组合</returns>
        public List<SelectionCombinationModel> GetRemovedCombination()
        {
            return this.Combinations.Where(c => c.IsChecked)?.Select(s =>
               {
                   SelectionCombinationModel combination = new SelectionCombinationModel()
                   {
                       SelectionCombination = s.Selections.Select(ss =>
                       {
                           return new SelectionModel()
                           {
                               CourseId = ss.CourseId,
                               LevelId = ss.LevelId
                           };
                       })?.ToList()
                   };
                   return combination;
               })?.ToList();
        }
    }
}
