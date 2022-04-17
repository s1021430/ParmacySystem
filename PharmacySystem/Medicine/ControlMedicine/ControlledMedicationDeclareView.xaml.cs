namespace PharmacySystem.Medicine.ControlMedicine
{
    /// <summary>
    /// ControlledMedicationDeclareView.xaml 的互動邏輯
    /// </summary>
    public partial class ControlledMedicationDeclareView : System.Windows.Controls.UserControl
    {
        public ControlledMedicationDeclareView()
        {
            InitializeComponent();
            DataContext = new ControlledMedicationDeclareViewModel();
        }
    }
}
