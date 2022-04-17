using System;
using System.Globalization;
using System.Windows.Data;

namespace PharmacySystem.Converters
{
    public class DoubleStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is double d)
                return d.ToString(CultureInfo.CurrentCulture);
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string s)
                return double.TryParse(s, out var result) ? result : Binding.DoNothing;
            return Binding.DoNothing;
        }
    }

    public class TaiwanCalendarYearConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int i)
                return (i - 1911).ToString(CultureInfo.CurrentCulture);
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
