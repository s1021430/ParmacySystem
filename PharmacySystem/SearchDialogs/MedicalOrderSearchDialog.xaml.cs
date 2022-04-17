using System.Linq;
using System.Windows;
using PharmacySystem.ViewModel;

namespace PharmacySystem.SearchDialogs
{
    /// <summary>
    /// MedicalOrderSearchDialog.xaml 的互動邏輯
    /// </summary>
    public partial class MedicalOrderSearchDialog : Window
    {
        public MedicalOrderSearchDialog()
        {
            InitializeComponent();
        }

        private void CancelSearchButtonOnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void OkButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        public bool? ProcessSearchResult(MedicalOrderSearchDialogViewModel dialogViewModel)
        {
            switch (dialogViewModel.Result.Count)
            {
                case 0:
                    return false;
                case 1:
                    dialogViewModel.SelectedOrder = dialogViewModel.Result.First();
                    return true;
                default:
                    DataContext = dialogViewModel;
                    return ShowDialog();
            }
        }
    }
}
