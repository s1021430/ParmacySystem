namespace PharmacySystem.OrdersOperation
{
    /// <summary>
    /// StoreOrderManagementView.xaml 的互動邏輯
    /// </summary>
    public partial class StoreOrderManagementView : System.Windows.Controls.UserControl
    {
        public StoreOrderManagementView()
        {
            InitializeComponent();
            this.DataContext = new StoreOrderManagementViewModel();
        }
    }
}
