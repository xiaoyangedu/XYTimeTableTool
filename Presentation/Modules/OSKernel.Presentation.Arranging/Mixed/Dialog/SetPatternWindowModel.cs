using OSKernel.Presentation.Arranging.Mixed.Pattern;
using OSKernel.Presentation.Arranging.Mixed.Pattern.Model;
using OSKernel.Presentation.Arranging.Mixed.Pattern.Operator;
using OSKernel.Presentation.Core;
using OSKernel.Presentation.Core.ViewModel;
using OSKernel.Presentation.CustomControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Unity;
using XYKernel.Application.OS.Data.MixedClass;
using XYKernel.OS.Common.Models.Pattern.Base;
using XYKernel.Application.OS.DataValidation.Common;
using MixedModel = XYKernel.OS.Common.Models.Mixed;

namespace OSKernel.Presentation.Arranging.Mixed.Dialog
{
    /// <summary>
    /// 设置模式界面
    /// </summary>
    public class SetPatternWindowModel : CommonViewModel, IInitilize
    {
        private UIPattern _selectPattern;

        private UIPatternOperator _selectOperator;

        private List<UIPattern> _patterns;

        private List<UIPatternOperator> sourceOperators;

        private List<UIPatternOperator> _patternOperators;

        private bool _showFirstPanel = true;

        private bool _showSecondPanel;

        public ICommand SaveCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<object>(saveCommand);
            }
        }

        public ICommand CancelCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<object>(cancelCommand);
            }
        }

        public ICommand SetOperationCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<object>(setOperationCommand);
            }
        }

        public ICommand BackCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand(backCommand);
            }

        }

        /// <summary>
        /// 选中模式
        /// </summary>
        public UIPattern SelectPattern
        {
            get
            {
                return _selectPattern;
            }

            set
            {
                _selectPattern = value;
                RaisePropertyChanged(() => SelectPattern);

                if (_selectPattern != null)
                    filterOperators(_selectPattern);
            }
        }

        /// <summary>
        /// 模式
        /// </summary>
        public List<UIPattern> Patterns
        {
            get
            {
                return _patterns;
            }

            set
            {
                _patterns = value;
                RaisePropertyChanged(() => Patterns);
            }
        }

        /// <summary>
        /// 模式对应的操作项
        /// </summary>
        public List<UIPatternOperator> PatternOperators
        {
            get
            {
                return _patternOperators;
            }

            set
            {
                _patternOperators = value;
                RaisePropertyChanged(() => PatternOperators);
            }
        }

        /// <summary>
        /// 是否显示操作()
        /// </summary>
        public bool ShowOperator
        {
            get
            {
                if (PatternOperators.Count == 0)
                    return false;
                else
                    return true;
            }
        }

        /// <summary>
        /// 显示第一个
        /// </summary>
        public bool ShowFirstPanel
        {
            get
            {
                return _showFirstPanel;
            }

            set
            {
                _showFirstPanel = value;
                RaisePropertyChanged(() => ShowFirstPanel);
            }
        }

        /// <summary>
        /// 显示第二个
        /// </summary>
        public bool ShowSecondPanel
        {
            get
            {
                return _showSecondPanel;
            }

            set
            {
                _showSecondPanel = value;
                RaisePropertyChanged(() => ShowSecondPanel);
            }
        }

        /// <summary>
        /// 选择项
        /// </summary>
        public UIPatternOperator SelectOperator
        {
            get
            {
                return _selectOperator;
            }

            set
            {
                _selectOperator = value;
                RaisePropertyChanged(() => SelectOperator);
            }
        }

        public SetPatternWindowModel()
        {
            sourceOperators = new List<UIPatternOperator>()
            {
                 new UIPatternOperator()
                 {
                      Operator= UIPatternOperator.OperatorEnum.RemoveCombination,
                      View= new Pattern.Operator.RemoveCombinationView()
                 },
                 new UIPatternOperator()
                 {
                      Operator= UIPatternOperator.OperatorEnum.ClassCapacity,
                      View= new Pattern.Operator.ClassCapacityView()
                 },
                 new UIPatternOperator()
                 {
                     Operator= UIPatternOperator.OperatorEnum.Position,
                     View= new Pattern.Operator.PositionView()
                 }
            };
        }

        void filterOperators(UIPattern pattern)
        {
            switch (pattern.Pattern)
            {
                case Models.Enums.PatternTypeEnum.Normal:

                    this.PatternOperators = this.sourceOperators;
                    this.RaisePropertyChanged(() => ShowOperator);

                    break;
                case Models.Enums.PatternTypeEnum.Validation:

                    this.PatternOperators = new List<UIPatternOperator>();
                    this.RaisePropertyChanged(() => ShowOperator);

                    break;
                case Models.Enums.PatternTypeEnum.Compression:

                    this.PatternOperators = this.sourceOperators.Where(o => o.Operator != UIPatternOperator.OperatorEnum.Position)?.ToList();
                    this.RaisePropertyChanged(() => ShowOperator);

                    break;
                case Models.Enums.PatternTypeEnum.OptimizedCompression:

                    this.PatternOperators = new List<UIPatternOperator>();
                    this.RaisePropertyChanged(() => ShowOperator);

                    break;
                case Models.Enums.PatternTypeEnum.Extraction:

                    this.PatternOperators = this.sourceOperators.Where(o => o.Operator != UIPatternOperator.OperatorEnum.ClassCapacity)?.ToList();
                    this.RaisePropertyChanged(() => ShowOperator);

                    break;
                case Models.Enums.PatternTypeEnum.OptimizedExtraction:

                    this.PatternOperators = this.sourceOperators.Where(o => o.Operator == UIPatternOperator.OperatorEnum.RemoveCombination)?.ToList();
                    this.RaisePropertyChanged(() => ShowOperator);

                    break;
            }
        }

        void saveCommand(object obj)
        {
            if (this.SelectPattern == null)
            {
                this.ShowDialog("提示信息", "模式为空!", CustomControl.Enums.DialogSettingType.OnlyOkButton, CustomControl.Enums.DialogType.Warning);
                return;
            }

            var clModel = base.LocalID.DeSerializeCL();
            var algoRule = base.LocalID.DeSerializePatternAlgo();
            var rule = base.LocalID.DeSerializeMixedRule();

            // 1.获取操作
            // 1.1 去除组合
            var combinationView = this.sourceOperators.FirstOrDefault(p => p.Operator == UIPatternOperator.OperatorEnum.RemoveCombination);
            var removeCombinations = (combinationView.View.DataContext as RemoveCombinationViewModel).GetRemovedCombination();

            // 1.2 班额调整
            var classView = this.sourceOperators.FirstOrDefault(p => p.Operator == UIPatternOperator.OperatorEnum.ClassCapacity);
            var classCapacitys = (classView.View.DataContext as ClassCapacityViewModel).GetCapacitys();

            // 1.3 课位调整
            var positionView = this.sourceOperators.FirstOrDefault(p => p.Operator == UIPatternOperator.OperatorEnum.Position);
            var positions = (positionView.View.DataContext as PositionViewModel).GetCoursePositions();

            object severialPattern = null;
            DataProcessing dataProcess = new DataProcessing();

            MixedModel.CLCase patternCase = null;
            MixedModel.AlgoRule.AlgoRule patternAlgoRule = null;
            MixedModel.Rule.Rule patternRule = null;
            List<DataValidationResultInfo> validationResultInfo = new List<DataValidationResultInfo>() { };

            switch (this.SelectPattern.Pattern)
            {
                case Models.Enums.PatternTypeEnum.Normal:

                    NormalModel normal = new NormalModel()
                    {
                        ClassCapacity = classCapacitys,
                        Positions = positions,
                        RemovedCombination = removeCombinations
                    };
                    severialPattern = normal;

                    Tuple<MixedModel.CLCase, bool, List<DataValidationResultInfo>> normalTuple = dataProcess.GetModelByNormal(clModel, rule, algoRule, normal);
                    if (normalTuple.Item2)
                    {
                        patternCase = normalTuple.Item1;
                    }
                    else
                    {
                        StringBuilder message = new StringBuilder();
                        normalTuple.Item3?.ForEach(m =>
                        {
                            message.Append($"描述:{m.ErrorCode}-{m.Description}");
                        });
                        this.ShowDialog("错误消息", message.ToString(), CustomControl.Enums.DialogSettingType.OnlyOkButton, CustomControl.Enums.DialogType.Error);
                        return;
                    }

                    break;
                case Models.Enums.PatternTypeEnum.Extraction:

                    ExtractionViewModel value = this.SelectPattern.View.DataContext as ExtractionViewModel;

                    StudentExtractionModel extraction = new StudentExtractionModel()
                    {
                        ExtractionRatio = value.ExtractionRate,
                        IncreasedCapacity = value.ClassCapacity,
                        Positions = positions,
                        RemovedCombination = removeCombinations
                    };
                    severialPattern = extraction;
                    Tuple<MixedModel.CLCase, bool, List<DataValidationResultInfo>> extractionTuple = dataProcess.GetModelByStudentExtraction(clModel, rule, algoRule, extraction);
                    if (extractionTuple.Item2)
                    {
                        patternCase = extractionTuple.Item1;
                    }
                    else
                    {
                        StringBuilder message = new StringBuilder();
                        extractionTuple.Item3?.ForEach(m =>
                        {
                            message.Append($"描述:{m.ErrorCode}-{m.Description}");
                        });
                        this.ShowDialog("错误消息", message.ToString(), CustomControl.Enums.DialogSettingType.OnlyOkButton, CustomControl.Enums.DialogType.Error);
                        return;
                    }

                    break;
                case Models.Enums.PatternTypeEnum.OptimizedExtraction:

                    OptimizedExtractionViewModel optimizedExtractionVM = this.SelectPattern.View.DataContext as OptimizedExtractionViewModel;

                    if (optimizedExtractionVM.SelectResult == null)
                    {
                        this.ShowDialog("提示信息", "没有选择结果", CustomControl.Enums.DialogSettingType.OnlyOkButton, CustomControl.Enums.DialogType.Warning);
                        return;
                    }

                    var result = Core.Http.OSHttpClient.Instance.GetResult(optimizedExtractionVM.SelectResult.TaskID);
                    if (result.Item1)
                    {
                        UIOptimizedExtraction optimizedExtraction = new UIOptimizedExtraction()
                        {
                            ClassCapacity = optimizedExtractionVM.ClassCapacity,
                            Result = result.Item2,
                            RemovedCombination = removeCombinations
                        };
                        severialPattern = optimizedExtraction;

                        var extractionOptimzeTuple = dataProcess.GetModelByFixedClassTimeTable(clModel, rule, algoRule, new NormalModel() { RemovedCombination = removeCombinations }, optimizedExtraction.Result, optimizedExtraction.ClassCapacity);

                        if (extractionOptimzeTuple.Item3)
                        {
                            patternCase = extractionOptimzeTuple.Item1;
                            patternAlgoRule = extractionOptimzeTuple.Item2;
                        }
                        else
                        {
                            StringBuilder message = new StringBuilder();
                            extractionOptimzeTuple.Item4?.ForEach(m =>
                            {
                                message.Append($"描述:{m.ErrorCode}-{m.Description}");
                            });
                            this.ShowDialog("错误消息", message.ToString(), CustomControl.Enums.DialogSettingType.OnlyOkButton, CustomControl.Enums.DialogType.Error);
                            return;
                        }
                    }

                    break;
                case Models.Enums.PatternTypeEnum.Compression:

                    CompressionViewModel compressionVM = this.SelectPattern.View.DataContext as CompressionViewModel;

                    TimeCompressionModel compression = new TimeCompressionModel()
                    {
                        CompressionRatio = compressionVM.Compression,
                        ClassCapacity = classCapacitys,
                        RemovedCombination = removeCombinations
                    };
                    severialPattern = compression;

                    var compressionTuple = dataProcess.GetModelByTimeCompression(clModel, rule, algoRule, compression);

                    if (compressionTuple.Item4)
                    {
                        patternCase = compressionTuple.Item1;
                        patternRule = compressionTuple.Item2;
                        patternAlgoRule = compressionTuple.Item3;
                    }
                    else
                    {
                        StringBuilder message = new StringBuilder();
                        compressionTuple.Item5?.ForEach(m =>
                        {
                            message.Append($"描述:{m.ErrorCode}-{m.Description}");
                        });
                        this.ShowDialog("错误消息", message.ToString(), CustomControl.Enums.DialogSettingType.OnlyOkButton, CustomControl.Enums.DialogType.Error);
                        return;
                    }

                    break;
                case Models.Enums.PatternTypeEnum.OptimizedCompression:

                    OptimizedCompressionViewModel optimizedCompressionVM = this.SelectPattern.View.DataContext as OptimizedCompressionViewModel;
                    severialPattern = optimizedCompressionVM.SelectResult;

                    if (optimizedCompressionVM.SelectResult == null)
                    {
                        this.ShowDialog("提示信息", "没有选择结果", CustomControl.Enums.DialogSettingType.OnlyOkButton, CustomControl.Enums.DialogType.Warning);
                        return;
                    }

                    var optimizedResult = Core.Http.OSHttpClient.Instance.GetResult(optimizedCompressionVM.SelectResult.TaskID);
                    if (optimizedResult != null)
                    {
                        var compressionOptimize = dataProcess.GetModelByStudentsClassificationResult(clModel, rule, algoRule, optimizedResult.Item2);
                        if (compressionOptimize.Item4)
                        {
                            patternCase = compressionOptimize.Item1;
                            patternRule = compressionOptimize.Item2;
                            patternAlgoRule = compressionOptimize.Item3;
                        }
                        else
                        {
                            StringBuilder message = new StringBuilder();
                            compressionOptimize.Item5?.ForEach(m =>
                            {
                                message.Append($"描述:{m.ErrorCode}-{m.Description}");
                            });
                            this.ShowDialog("错误消息", message.ToString(), CustomControl.Enums.DialogSettingType.OnlyOkButton, CustomControl.Enums.DialogType.Error);
                            return;
                        }
                    }

                    break;
                case Models.Enums.PatternTypeEnum.Validation:

                    ValidationViewModel validationVM = this.SelectPattern.View.DataContext as ValidationViewModel;
                    UIValidation validation = new UIValidation()
                    {
                        AssignedStudents = validationVM.IncludeAssinged,
                        OnlyTeacher = validationVM.OnlyTeacher
                    };
                    severialPattern = validation;

                    if (validationVM.IncludeAssinged)
                    {
                        var IncludeAssingedTuple = dataProcess.GetModelWithOnlyStudentsAssignedToClass(clModel, rule, algoRule);
                        if (IncludeAssingedTuple.Item2)
                        {
                            patternCase = IncludeAssingedTuple.Item1;
                        }
                        else
                        {
                            StringBuilder message = new StringBuilder();
                            IncludeAssingedTuple.Item3?.ForEach(m =>
                            {
                                message.Append($"描述:{m.ErrorCode}-{m.Description}");
                            });
                            this.ShowDialog("错误消息", message.ToString(), CustomControl.Enums.DialogSettingType.OnlyOkButton, CustomControl.Enums.DialogType.Error);
                            return;
                        }
                    }
                    else
                    {
                        var OnlyTeacherTuple = dataProcess.GetModelWithoutStudents(clModel, rule, algoRule);
                        if (OnlyTeacherTuple.Item2)
                        {
                            patternCase = OnlyTeacherTuple.Item1;
                        }
                        else
                        {
                            StringBuilder message = new StringBuilder();
                            OnlyTeacherTuple.Item3?.ForEach(m =>
                            {
                                message.Append($"描述:{m.ErrorCode}-{m.Description}");
                            });
                            this.ShowDialog("错误消息", message.ToString(), CustomControl.Enums.DialogSettingType.OnlyOkButton, CustomControl.Enums.DialogType.Error);
                            return;
                        }
                    }

                    break;
            }

            if (patternCase != null)
            {
                var local = CommonDataManager.GetLocalCase(base.LocalID);
                local.Pattern = this.SelectPattern.Pattern;
                local.Serialize();

                base.LocalID.Serialize(severialPattern);

                patternCase.SerializePatternCase(base.LocalID);
                PatternDataManager.AddCase(base.LocalID, patternCase);

                if (patternAlgoRule == null)
                {
                    if (algoRule != null)
                    {
                        algoRule.SerializePatternAlgo(base.LocalID);
                        PatternDataManager.AddAlgoRule(base.LocalID, algoRule);
                    }
                }
                else
                {
                    patternAlgoRule.SerializePatternAlgo(base.LocalID);
                    PatternDataManager.AddAlgoRule(base.LocalID, patternAlgoRule);
                }

                if (patternRule == null)
                {
                    if (rule != null)
                    {
                        rule.SerializePatternRule(base.LocalID);
                        PatternDataManager.AddRule(base.LocalID, rule);
                    }
                }
                else
                {
                    patternRule.SerializePatternRule(base.LocalID);
                    PatternDataManager.AddRule(base.LocalID, patternRule);
                }

                SetPatternWindow win = obj as SetPatternWindow;
                win.PatternType = this.SelectPattern.Pattern;
                win.DialogResult = true;
            }
            else
            {
                this.ShowDialog("提示信息", "模式实体转换失败!", CustomControl.Enums.DialogSettingType.OnlyOkButton, CustomControl.Enums.DialogType.Warning);
            }
        }

        void cancelCommand(object obj)
        {
            SetPatternWindow win = obj as SetPatternWindow;
            win.DialogResult = false;
        }

        void setOperationCommand(object obj)
        {
            this.ShowFirstPanel = false;
            this.ShowSecondPanel = true;

            this.SelectOperator = (obj as UIPatternOperator);
        }

        void backCommand()
        {
            this.ShowFirstPanel = true;
            this.ShowSecondPanel = false;
        }

        [InjectionMethod]
        public void Initilize()
        {
            this.Patterns = new List<UIPattern>()
            {
                 new UIPattern()
                 {
                      Pattern= Models.Enums.PatternTypeEnum.Normal,
                      View=new NormalView()
                 },
                 new UIPattern()
                 {
                      Pattern= Models.Enums.PatternTypeEnum.Validation,
                      View=new ValidationView()
                 },
                 new UIPattern()
                 {
                      Pattern= Models.Enums.PatternTypeEnum.Extraction,
                      View=new ExtractionView()
                 },
                  new UIPattern()
                 {
                      Pattern= Models.Enums.PatternTypeEnum.OptimizedExtraction,
                      View=new OptimizedExtractionView()
                 },
                   new UIPattern()
                 {
                      Pattern= Models.Enums.PatternTypeEnum.Compression,
                      View=new CompressionView()
                 },
                    new UIPattern()
                 {
                      Pattern= Models.Enums.PatternTypeEnum.OptimizedCompression,
                      View=new OptimizedCompressionView()
                 },
            };

            var local = CommonDataManager.GetLocalCase(base.LocalID);

            var firstDefault = this.Patterns.FirstOrDefault(p => p.Pattern == local.Pattern);
            if (firstDefault != null)
            {
                this.SelectPattern = firstDefault;

                switch (this.SelectPattern.Pattern)
                {
                    case Models.Enums.PatternTypeEnum.Normal:

                        break;
                    case Models.Enums.PatternTypeEnum.Validation:

                        var vm = this.SelectPattern.View.DataContext as ValidationViewModel;
                        var value = base.LocalID.DeSerialize<UIValidation>();
                        if (value != null)
                        {
                            vm.IncludeAssinged = value.AssignedStudents;
                            vm.OnlyTeacher = value.OnlyTeacher;
                        }

                        break;
                    case Models.Enums.PatternTypeEnum.Extraction:

                        var extractionVM = this.SelectPattern.View.DataContext as ExtractionViewModel;
                        var extractionModel = base.LocalID.DeSerialize<StudentExtractionModel>();
                        if (extractionModel != null)
                        {
                            extractionVM.ClassCapacity = extractionModel.IncreasedCapacity;
                            extractionVM.ExtractionRate = extractionModel.ExtractionRatio;
                        }

                        break;
                    case Models.Enums.PatternTypeEnum.OptimizedExtraction:
                        // 优化部分的结果绑定TODO
                        break;
                    case Models.Enums.PatternTypeEnum.Compression:

                        var compressionVM = this.SelectPattern.View.DataContext as CompressionViewModel;
                        var compressionModel = base.LocalID.DeSerialize<TimeCompressionModel>();
                        if (compressionModel != null)
                        {
                            compressionVM.Compression = compressionModel.CompressionRatio;
                        }

                        break;
                    case Models.Enums.PatternTypeEnum.OptimizedCompression:
                        // 优化部分的结果绑定TODO
                        break;
                }
            }
            else
            {
                this.SelectPattern = this.Patterns.First();
            }
        }
    }
}
