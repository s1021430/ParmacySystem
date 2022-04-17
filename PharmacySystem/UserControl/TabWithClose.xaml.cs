using System.Windows.Media;
using GalaSoft.MvvmLight;
using PharmacySystem.Class.Features;

namespace PharmacySystem.UserControl
{
    /// <summary>
    /// TabWithClose.xaml 的互動邏輯
    /// </summary>
    public partial class TabWithClose : System.Windows.Controls.UserControl
    {
        public TabWithClose()
        {
            InitializeComponent();
        }
    }

    public class HeaderWithCloseViewModel : ObservableObject
    {
        private string header;
        public string Header
        {
            get => header;
            private set
            {
                Set(() => Header, ref header, value);
            }
        }
        private ImageSource icon;
        public ImageSource Icon
        {
            get => icon;
            private set
            {
                Set(() => Icon, ref icon, value);
            }
        }
        private bool closable;
        public bool Closable
        {
            get => closable;
            set
            {
                Set(() => Closable, ref closable, value);
            }
        }

        public HeaderWithCloseViewModel(FeaturesEnum header, ImageSource icon, bool closable = true)
        {
            Header = header.ToString();
            Icon = icon;
            Closable = closable;
        }
    }
}
