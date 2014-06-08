using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace PresenceTracker
{
    public enum State
    {
        Shutdown,
        Logon,
        Logoff,
        Lock,
        Unlock,
        Unknown,
        AppStart,
        Resume,
        Suspend
    }

    [ValueConversion(typeof(State), typeof(SolidColorBrush))]
    public class StateToColorConverter : MarkupExtension, IValueConverter
    {
        public StateToColorConverter() { }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (value != null && value is State)
            {
                switch ((State)value)
                {
                    case State.AppStart:
                        return new SolidColorBrush(Colors.ForestGreen);
                    case State.Lock:
                        return new SolidColorBrush(Colors.Yellow);
                    case State.Logoff:
                        return new SolidColorBrush(Colors.Red);
                    case State.Logon:
                        return new SolidColorBrush(Colors.Green);
                    case State.Shutdown:
                        return new SolidColorBrush(Colors.Red);
                    case State.Unknown:
                        return new SolidColorBrush(Colors.SaddleBrown);
                    case State.Unlock:
                        return new SolidColorBrush(Colors.Green);
                    default:
                        return new SolidColorBrush(Colors.Transparent);
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
