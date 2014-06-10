using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace PresenceTracker
{
    [ValueConversion(typeof(object), typeof(object))]
    public class LookupConverter : MarkupExtension, IValueConverter
    {
        public LookupConverter() { }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter == null)
                return DependencyProperty.UnsetValue;

            SortedList l = parameter as SortedList;
            if (l == null)
                return DependencyProperty.UnsetValue;

            if (value == null)
                return null;

            if (!l.ContainsKey(value))
                return DependencyProperty.UnsetValue;

            return l[value];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
