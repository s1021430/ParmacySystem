using GalaSoft.MvvmLight;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PharmacySystem.Class;
using PharmacySystem.View.CommonSetting;

namespace PharmacySystem.ViewModel
{
    public class CommonSettingViewModel : ViewModelBase
    {
        public ObservableCollection<CommonSettingItem> SettingItems { get; }

        private CommonSettingItem? selectedItem;
        public CommonSettingItem? SelectedItem
        {
            get => selectedItem;
            set => Set(()=> SelectedItem,ref selectedItem, value);
        }
        
        private int selectedIndex;
        public int SelectedIndex
        {
            get => selectedIndex;
            set => Set(() => SelectedIndex, ref selectedIndex, value);
        }

        public CommonSettingViewModel()
        {
            SettingItems = new ObservableCollection<CommonSettingItem>();
            foreach (var item in GenerateDemoItems().OrderBy(i => i.Name))
            {
                SettingItems.Add(item);
            }
        }
        private static IEnumerable<CommonSettingItem> GenerateDemoItems()
        {
            yield return new CommonSettingItem(
                "申報檔設定",
                typeof(DeclareSetting));
        }
    }
}
