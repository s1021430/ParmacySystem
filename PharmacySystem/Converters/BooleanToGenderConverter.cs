using System;
using System.Globalization;
using System.Windows.Data;
using GeneralClass.Customer;

namespace PharmacySystem.Converters
{
    public class BooleanToGenderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || !bool.TryParse(value.ToString(), out var result))
                return Binding.DoNothing;
            return result ? Gender.男.ToString() : Gender.女.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
