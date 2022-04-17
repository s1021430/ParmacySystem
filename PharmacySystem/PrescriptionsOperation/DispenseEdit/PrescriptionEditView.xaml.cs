using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using GeneralClass.Prescription;
using GeneralClass.Prescription.EntityIndex;
using PharmacySystem.UIBehavior;

namespace PharmacySystem.PrescriptionsOperation.DispenseEdit
{
    /// <summary>
    /// PrescriptionEditView.xaml 的互動邏輯
    /// </summary>
    public partial class PrescriptionEditView : System.Windows.Controls.UserControl
    {
        public readonly PrescriptionID ID;
        private readonly Messenger prescriptionEditViewMessenger;
        private readonly string messengerName;
        public PrescriptionEditView(PrescriptionData prescription)
        {
            InitializeComponent();
            ID = prescription.ID;
            messengerName = $"PrescriptionEditViewModelMessenger{ID}";
            DataContext = new PrescriptionEditViewModel(prescription);
            prescriptionEditViewMessenger = SimpleIoc.Default.GetInstance<Messenger>(messengerName);
            prescriptionEditViewMessenger.Register<NotificationMessage>(this, FocusNext);
            prescriptionEditViewMessenger.Register<NotificationMessage<int>>(this, FocusDosage);
        }

        private void FocusDosage(NotificationMessage<int> message)
        {
            var rowIndex = message.Content;
            var row = (DataGridRow)MedicineDataGrid.ItemContainerGenerator.ContainerFromIndex(rowIndex);
            if (row == null) return;
            VisualTreeExtension.FocusSpecificCheckBoxCell(MedicineDataGrid, rowIndex, 2, typeof(TextBox));
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
            if (e.Key != Key.Enter) return;
            e.Handled = true;
            var targetIndex = int.Parse(((Control)sender).Tag.ToString());
            var type = targetIndex == 7 ? typeof(CheckBox) : typeof(TextBox);
            var rowIndex = MedicineDataGrid.SelectedIndex;
            if (targetIndex == 0)
                rowIndex++;
            VisualTreeExtension.FocusSpecificCheckBoxCell(MedicineDataGrid, rowIndex, targetIndex, type);
        }

        private void PrescriptionEditView_OnUnloaded(object sender, RoutedEventArgs e)
        {
            prescriptionEditViewMessenger.Unregister(this);
            SimpleIoc.Default.Unregister<Messenger>(messengerName);
        }
    }
}
