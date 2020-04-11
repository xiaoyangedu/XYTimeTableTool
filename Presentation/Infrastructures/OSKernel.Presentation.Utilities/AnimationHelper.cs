using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace OSKernel.Presentation.Utilities
{
    /// <summary>
    /// 动画帮助类
    /// </summary>
    public static class AnimationHelper
    {
        /// <summary>
        /// 渐入
        /// </summary>
        /// <param name="element"></param>
        public static void FedIn(this UIElement element)
        {
            DoubleAnimation dLoginFadeIn = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromSeconds(1)));
            element.BeginAnimation(UIElement.OpacityProperty, dLoginFadeIn);
            element.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// 渐出
        /// </summary>
        /// <param name="element"></param>
        public static void FedOut(this UIElement element)
        {
            DoubleAnimation dLoginFadeOut = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromSeconds(1)));
            element.BeginAnimation(UIElement.OpacityProperty, dLoginFadeOut);
            element.Visibility = Visibility.Collapsed;
        }

        public static void Show(this Grid element)
        {
            DoubleAnimation dLoginFadeIn = new DoubleAnimation(element.Height, element.MaxHeight, new Duration(TimeSpan.FromSeconds(1)));
            dLoginFadeIn.EasingFunction = new CircleEase { EasingMode = EasingMode.EaseOut };
            element.BeginAnimation(Grid.HeightProperty, dLoginFadeIn);
        }

        public static void Hidden(this Grid element)
        {
            DoubleAnimation dLoginFadeOut = new DoubleAnimation(element.MaxHeight, 15, new Duration(TimeSpan.FromSeconds(1)));
            dLoginFadeOut.EasingFunction = new CircleEase { EasingMode = EasingMode.EaseOut };
            element.BeginAnimation(Grid.HeightProperty, dLoginFadeOut);
        }
    }
}
