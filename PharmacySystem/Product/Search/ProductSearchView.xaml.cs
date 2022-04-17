using PharmacySystem.PrescriptionsOperation.Search;
using PharmacySystem.ViewModel;

namespace PharmacySystem.View
{
    /// <summary>
    /// ProductSearchView.xaml 的互動邏輯
    /// </summary>
    public partial class ProductSearchView : System.Windows.Controls.UserControl
    {
        public ProductSearchView()
        {
            InitializeComponent();
            DataContext = new PrescriptionSearchViewModel();
        }
    }
}
