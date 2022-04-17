using System.Windows;

namespace PharmacySystem.View.CommonSetting
{
    /// <summary>
    /// CommonSettingDialog.xaml 的互動邏輯
    /// </summary>
    public partial class CommonSettingDialog : System.Windows.Controls.UserControl
    {
        public CommonSettingDialog()
        {
            InitializeComponent();
        }

        private void MenuToggleButton_OnClick(object sender, RoutedEventArgs e)
        {
            SettingListBox.Focus();
        }

        private void OnSelectedItemChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            SettingItemsScrollViewer.ScrollToHome();
        }
    }
}
