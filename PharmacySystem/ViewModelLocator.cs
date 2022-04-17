using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;
using PharmacySystem.Medicine.Management;
using PharmacySystem.PrescriptionsOperation.Dispense;
using PharmacySystem.PrescriptionsOperation.DispenseEdit;

namespace PharmacySystem.ViewModel
{
    public class ViewModelLocator
    {
        public static bool IsInDesign => true;
        public static ViewModelLocator Locator;
        public ViewModelLocator()
        {
            Locator = this;
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<DispenseViewModel>();
            SimpleIoc.Default.Register<MedicineManagementViewModel>();
            SimpleIoc.Default.Register<DispenseEditViewModel>();
        }

        public MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();
        public DispenseViewModel Dispense => ServiceLocator.Current.GetInstance<DispenseViewModel>();
        public DispenseEditViewModel DispenseEdit => ServiceLocator.Current.GetInstance<DispenseEditViewModel>();
        public MedicineManagementViewModel MedicineManagement => ServiceLocator.Current.GetInstance<MedicineManagementViewModel>();
        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}
