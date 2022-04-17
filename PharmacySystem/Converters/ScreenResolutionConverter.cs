using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;

namespace PharmacySystem.Converters
{

    [ValueConversion(typeof(string), typeof(string))]
    public class ScreenResolutionConverter : MarkupExtension, IValueConverter
    {
        private static ScreenResolutionConverter _instance;

        public ScreenResolutionConverter() { }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double size = System.Convert.ToDouble(value) * System.Convert.ToDouble(parameter, CultureInfo.InvariantCulture);
            return size.ToString("G0", CultureInfo.InvariantCulture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _instance ??= new ScreenResolutionConverter();
        }

    }
}
