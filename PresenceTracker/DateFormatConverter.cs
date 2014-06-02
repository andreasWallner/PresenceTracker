using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;

namespace PresenceTracker
{
    [ValueConversion(typeof(DateTime), typeof(string))]
    public class DateFormatConverter : MarkupExtension, IValueConverter
    {
        public string format { get; set; }

        public DateFormatConverter() { }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (value != null && value is DateTime)
            {
                DateTime dt = (DateTime)value;
                if(parameter!=null && parameter is string)
                    return dt.ToString((string)parameter);
                if (format != null)
                    return dt.ToString(format);
                else
                    return dt.ToString();
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
