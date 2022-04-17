using GeneralClass.SystemInfo;
using System.Windows;
using PharmacySystem.ApplicationServiceLayer;

namespace PharmacySystem
{
    /// <summary>
    /// InitialSettingWindow.xaml 的互動邏輯
    /// </summary>
    public partial class InitialSettingWindow : Window
    {
        private InitialSettingApplicationService initialSettingService = new InitialSettingApplicationService();
        public InitialSettingWindow()
        {
            InitializeComponent();
             
            if(initialSettingService.IsNeedInit() == false)
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                Owner = mainWindow;
                Close();
            }
            
        }

        private void Btn_Register_Click(object sender, RoutedEventArgs e)
        {
            var ip = txt_IP.Text.Trim();
            var code = txt_Code.Text.Trim();
            InitialSettingData initialSettingData = new InitialSettingData() { IP = ip, PharmacyCode = code };
            if (initialSettingService.RegisterData(initialSettingData))
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                Owner = mainWindow;
                Close();
            }
            else
            {
                MessageBox.Show("IP 錯誤");
            }
        }

         
    }
}
