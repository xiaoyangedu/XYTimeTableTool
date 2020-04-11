using System.Windows;
using System.Windows.Controls;

namespace OSKernel.Presentation.CustomControl.Loading
{
    /// <summary>
    /// A control featuring a range of loading indicating animations.
    /// </summary>
    [TemplatePart(Name = "Border", Type = typeof(Border))]
    public class LoadingIndicator : Control
    {
        /// <summary>
        /// Identifies the <see cref="LoadingIndicators.WPF.LoadingIndicator.SpeedRatio"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SpeedRatioProperty =
            DependencyProperty.Register("SpeedRatio", typeof(double), typeof(LoadingIndicator), new PropertyMetadata(1d, (o, e) =>
            {
                LoadingIndicator li = (LoadingIndicator)o;
                if (li.PART_Border == null || li.IsActive == false)
                {
                    return;
                }

                foreach (VisualStateGroup group in VisualStateManager.GetVisualStateGroups(li.PART_Border))
                {
                    if (group.Name == "ActiveStates")
                    {
                        foreach (VisualState state in group.States)
                        {
                            if (state.Name == "Active")
                            {
                                state.Storyboard.SetSpeedRatio(li.PART_Border, (double)e.NewValue);
                            }
                        }
                    }
                }
            }));

        /// <summary>
        /// 可控制显示与隐藏
        /// Identifies the <see cref="LoadingIndicators.WPF.LoadingIndicator.IsActive"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.Register("IsActive", typeof(bool), typeof(LoadingIndicator), new PropertyMetadata(true, (o, e) =>
            {
                LoadingIndicator li = (LoadingIndicator)o;

                if (li.PART_Border == null)
                {
                    return;
                }

                if ((bool)e.NewValue == false)
                {
                    VisualStateManager.GoToElementState(li.PART_Border, "Inactive", false);
                    li.Visibility = Visibility.Collapsed;
                }
                else
                {
                    VisualStateManager.GoToElementState(li.PART_Border, "Active", false);
                    li.Visibility = Visibility.Visible;
                    foreach (VisualStateGroup group in VisualStateManager.GetVisualStateGroups(li.PART_Border))
                    {
                        if (group.Name == "ActiveStates")
                        {
                            foreach (VisualState state in group.States)
                            {
                                if (state.Name == "Active")
                                {
                                    state.Storyboard.SetSpeedRatio(li.PART_Border, li.SpeedRatio);
                                }
                            }
                        }
                    }
                }
            }));

        // Variables
        protected Border PART_Border;

        /// <summary>
        /// 
        /// 获取或者设置动画的播放速度
        /// </summary>
        public double SpeedRatio
        {
            get { return (double)GetValue(SpeedRatioProperty); }
            set { SetValue(SpeedRatioProperty, value); }
        }

        /// <summary>
        /// 获取或者设置是否是激活的
        /// </summary>
        public bool IsActive
        {
            get { return (bool)GetValue(IsActiveProperty); }
            set { SetValue(IsActiveProperty, value); }
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code
        /// or internal processes call System.Windows.FrameworkElement.ApplyTemplate().
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            PART_Border = (Border)GetTemplateChild("PART_Border");

            if (PART_Border != null)
            {
                VisualStateManager.GoToElementState(PART_Border, (this.IsActive ? "Active" : "Inactive"), false);
                foreach (VisualStateGroup group in VisualStateManager.GetVisualStateGroups(PART_Border))
                {
                    if (group.Name == "ActiveStates")
                    {
                        foreach (VisualState state in group.States)
                        {
                            if (state.Name == "Active")
                            {
                                state.Storyboard.SetSpeedRatio(PART_Border, this.SpeedRatio);
                            }
                        }
                    }
                }

                PART_Border.Visibility = (this.IsActive ? Visibility.Visible : Visibility.Collapsed);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadingIndicators.WPF.LoadingIndicator"/> class.
        /// </summary>
        public LoadingIndicator()
        {
        }
    }
}
