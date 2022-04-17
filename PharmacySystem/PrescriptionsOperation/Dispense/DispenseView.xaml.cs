using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Data;
using Dragablz;
using PharmacySystem.ViewModel;

namespace PharmacySystem.PrescriptionsOperation.Dispense
{
    /// <summary>
    /// DispenseView.xaml 的互動邏輯
    /// </summary>
    public partial class DispenseView : System.Windows.Controls.UserControl
    {
        public DispenseView()
        {
            InitializeComponent();
            ((DispenseViewModel) DataContext).NewPrescriptionCommand.Execute(null);
        }
    }

    public class MultiMarginConverter : IValueConverter
    {
        public object Convert(object values, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(values is ObservableCollection<HeaderedItemViewModel> tabs)) return Binding.DoNothing;
            var margin = tabs.Count * 120 + 15;
            return new Thickness(System.Convert.ToDouble(margin), 7, 5, 5);

        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
