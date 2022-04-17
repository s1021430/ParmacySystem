using System.Linq;
using System.Windows;
using PharmacySystem.ViewModel;

namespace PharmacySystem.SearchDialogs
{
    /// <summary>
    /// CustomerSearchDialog.xaml 的互動邏輯
    /// </summary>
    public partial class CustomerSearchDialog : Window
    {
        public CustomerSearchDialog()
        {
            InitializeComponent();
        }

        public bool? ProcessSearchResult(CustomerSearchDialogViewModel dialogViewModel)
        {
            switch (dialogViewModel.Result.Count)
            {
                case 0:
                    return false;
                case 1:
                    dialogViewModel.SelectedCustomer = dialogViewModel.Result.First();
                    return true;
                default:
                    DataContext = dialogViewModel;
                    return ShowDialog();
}
        }

        private void CancelCustomerSearchButtonOnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void OkButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
