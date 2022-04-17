namespace PharmacySystem.PrescriptionsOperation.Search
{
    /// <summary>
    /// PrescriptionSearchView.xaml 的互動邏輯
    /// </summary>
    public partial class PrescriptionSearchView : System.Windows.Controls.UserControl
    {
        public PrescriptionSearchView()
        {
            InitializeComponent();
            DataContext = new PrescriptionSearchViewModel();
        }
    }
}
