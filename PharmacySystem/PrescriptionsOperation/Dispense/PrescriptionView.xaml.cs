using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using PharmacySystem.UIBehavior;
using PharmacySystem.ViewModel;

namespace PharmacySystem.PrescriptionsOperation.Dispense
{
    /// <summary>
    /// PrescriptionView.xaml 的互動邏輯
    /// </summary>
    public partial class PrescriptionView : System.Windows.Controls.UserControl
    {
        private readonly Messenger prescriptionViewMessenger;
        private readonly string messengerName;
        public PrescriptionView()
        {
            InitializeComponent();
            messengerName = $"PrescriptionViewModelMessenger{ViewModelLocator.Locator.Dispense.PrescriptionsTabs.Count}";
            DataContext = new PrescriptionViewModel(messengerName);
            prescriptionViewMessenger = SimpleIoc.Default.GetInstance<Messenger>(messengerName);
            prescriptionViewMessenger.Register<NotificationMessage>(this, FocusNext);
            prescriptionViewMessenger.Register<NotificationMessage<int>>(this, FocusDosage);
        }

        private void FocusDosage(NotificationMessage<int> message)
        {
            var rowIndex = message.Content;
            var row = (DataGridRow)MedicineDataGrid.ItemContainerGenerator.ContainerFromIndex(rowIndex);
            if (row == null) return;
            VisualTreeExtension.FocusSpecificCheckBoxCell(MedicineDataGrid,rowIndex,2,typeof(TextBox));
        }

        private void FocusNext(NotificationMessage message)
        {
            switch (message.Notification)
            {
                case "GetInstitution":
                    DivisionComboBox.Focus();
                    break;
                case "GetMainDisease":
                    SubDisease.Focus();
                    break;
                case "GetSubDisease":
                    PaymentCategoryComboBox.Focus();
                    break;
            }
        }

        private void MedicineDataGridFocusNext(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                var targetIndex = int.Parse(((Control)sender).Tag.ToString());
                var type = targetIndex == 7 ? typeof(CheckBox) : typeof(TextBox);
                var rowIndex = MedicineDataGrid.SelectedIndex;
                if (targetIndex == 0)
                    rowIndex++;
                VisualTreeExtension.FocusSpecificCheckBoxCell(MedicineDataGrid, rowIndex, targetIndex, type);
            }
        }

        private void PrescriptionView_OnUnloaded(object sender, RoutedEventArgs e)
        {
            prescriptionViewMessenger.Unregister(this);
            SimpleIoc.Default.Unregister<Messenger>(messengerName);
        }
    }
}
