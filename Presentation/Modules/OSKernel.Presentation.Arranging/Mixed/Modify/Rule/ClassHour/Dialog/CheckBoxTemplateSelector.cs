using OSKernel.Presentation.Models.Mixed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace OSKernel.Presentation.Arranging.Mixed.Modify.Rule.ClassHour.Dialog
{
    public class CheckBoxTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var element = container as FrameworkElement;
            var model = element.DataContext as UIClass;
            DataTemplate template = null;

            if (model != null && element != null)
            {
                if (model.ID.Equals("0"))
                {
                    template = element.FindResource("HeaderCheckBox") as DataTemplate;
                }
                else
                {
                    template = element.FindResource("GeneralCheckBox") as DataTemplate;
                }
            }

            return template;
        }
    }
}
