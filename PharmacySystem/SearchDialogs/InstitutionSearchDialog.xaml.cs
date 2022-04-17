using System.Linq;
using System.Windows;
using PharmacySystem.ViewModel;

namespace PharmacySystem.SearchDialogs
{
    /// <summary>
    /// InstitutionSearchDialog.xaml 的互動邏輯
    /// </summary>
    public partial class InstitutionSearchDialog : Window
    {
        public InstitutionSearchDialog()
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

        public bool? ProcessSearchResult(InstitutionSearchDialogViewModel dialogViewModel)
        {
            switch (dialogViewModel.Result.Count)
            {
                case 0:
                    return false;
                case 1:
                    dialogViewModel.SelectedInstitution = dialogViewModel.Result.First();
                    return true;
                default:
                    DataContext = dialogViewModel;
                    return ShowDialog();
            }
        }
    }
}
