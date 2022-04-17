using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using PharmacySystem.ApplicationServiceLayer;

namespace PharmacySystem.ViewModel
{
    public enum ProductSearchType
    {
        Medicine,
        Product,
        SpecialMaterial
    }

    public class ProductSearchViewModel : ViewModelBase
    {
        private readonly ProductApplicationService productApplicationService;
        private ProductSearchType productSearchType;
        public ProductSearchType ProductSearchType
        {
            get => productSearchType;
            set
            {
                Set(() =>ProductSearchType,ref productSearchType,value);
            }
        }

        private string productID;
        public string ProductID
        {
            get => productID;
            set
            {
                Set(() => ProductID, ref productID, value);
            }
        }

        private string productName;
        public string ProductName
        {
            get => productName;
            set
            {
                Set(() => ProductName, ref productName, value);
            }
        }

        public ICommand SearchCommand { get; }
        public ProductSearchViewModel()
        {
            productApplicationService = ProductApplicationServiceFactory.GetProductApplicationService();
            SearchCommand = new RelayCommand(Search);
        }

        private static void Search()
        {

        }
    }


}
