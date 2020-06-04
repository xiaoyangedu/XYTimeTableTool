using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace OSKernel.Presentation.Arranging.Utilities
{
    public static class TextTrmmingShowToolTip
    {
        public static readonly DependencyProperty IsToolTipProperty = DependencyProperty.RegisterAttached(
           "IsToolTip", typeof(bool), typeof(TextTrmmingShowToolTip),
           new PropertyMetadata(default(bool), TextPropertyChangedCallback));

        public static void SetIsToolTip(DependencyObject element, bool value)
        {
            element.SetValue(IsToolTipProperty, value);
        }

        public static bool GetIsToolTip(DependencyObject element)
        {
            return (bool)element.GetValue(IsToolTipProperty);
        }

        private static void TextPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var tb = d as TextBlock;
            if (tb == null) return;
            tb.SizeChanged -= TbOnSizeChanged;
            if (!(bool)e.NewValue) return;
            tb.SizeChanged += TbOnSizeChanged;
        }

        private static void TbOnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var tb = sender as TextBlock;
            if (tb == null) return;
            SetToolTip(tb);
        }

        private static void SetToolTip(TextBlock tb)
        {
            if (string.IsNullOrEmpty(tb.Text))
            {
                tb.ToolTip = null;
                return;
            }

            var isTrim = IsTextTrimmed(tb);
            tb.ToolTip = isTrim ? tb.Text : null;
        }

        private static bool IsTextTrimmed(TextBlock textBlock)
        {
            Typeface typeface = new Typeface(
                textBlock.FontFamily,
                textBlock.FontStyle,
                textBlock.FontWeight,
                textBlock.FontStretch);
            FormattedText formattedText = new FormattedText(
                textBlock.Text,
                System.Threading.Thread.CurrentThread.CurrentCulture,
                textBlock.FlowDirection,
                typeface,
                textBlock.FontSize,
                textBlock.Foreground);
            bool isTrimmed = formattedText.Width <= textBlock.ActualWidth;
            return !isTrimmed;
        }
    }
}
