using System.Windows;

namespace PharmacySystem.PurchaseRequisition
{
    /// <summary>
    /// PurchaseRequisitionSearchDialog.xaml 的互動邏輯
    /// </summary>
    public partial class PurchaseRequisitionSearchDialog : Window
    {
        public PurchaseRequisitionSearchDialog()
        {
            InitializeComponent();

            var viewModel = new PurchaseRequisitionSearchDialogViewModel(CloseWindow);
            DataContext = viewModel;
        }

        private void CloseWindow()
        {
            DialogResult = true;
        }
    }
}
