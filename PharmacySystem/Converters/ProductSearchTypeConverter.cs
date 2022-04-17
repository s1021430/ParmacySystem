using System;
using System.Globalization;
using System.Windows.Data;
using PharmacySystem.ViewModel;

namespace PharmacySystem.Converters
{
    public class ProductSearchTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null || parameter is null || !int.TryParse(parameter.ToString(), out var searchType)) 
                return Binding.DoNothing;
            switch (searchType)
            {
                case 0:
                    return ProductSearchType.Medicine;
                case 1:
                    return ProductSearchType.Product;
                case 2:
                    return ProductSearchType.SpecialMaterial;
            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
