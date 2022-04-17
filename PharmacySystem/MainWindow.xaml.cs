using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace PharmacySystem
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        
    }

    [ValueConversion(typeof(string), typeof(string))]
    public class TabControlContentHeightConverter : MarkupExtension, IValueConverter
    {
        private static TabControlContentHeightConverter _instance;

        public TabControlContentHeightConverter() { }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var size = System.Convert.ToDouble(value) - 55 - 25;
            return size.ToString("G0", CultureInfo.InvariantCulture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _instance ??= new TabControlContentHeightConverter();
        }

    }
}
