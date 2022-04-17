using System;
using System.Globalization;
using System.Windows.Data;

namespace PharmacySystem.Converters
{
    public class TaiwanCalendarConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not DateTime date || date.Equals(DateTime.MinValue))
                return Binding.DoNothing;
            return date.ToString("yyy/MM/dd", culture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(DateTime.TryParse(value.ToString(),culture,DateTimeStyles.AssumeLocal, out var dt))
                return dt;
            return Binding.DoNothing;
        }
    }
}
