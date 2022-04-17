using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using Dragablz;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GeneralClass.Prescription;
using HisApiBase;
using PharmacySystem.Class.Features;
using PharmacySystem.PrescriptionsOperation.DispenseEdit;
using PharmacySystem.UserControl;
using PharmacySystem.View;
using PharmacySystem.View.CommonSetting;

namespace PharmacySystem.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public ObservableCollection<HeaderedItemViewModel> OpenedTabs { get; }
        private System.Windows.Controls.UserControl dialogContent;
        public System.Windows.Controls.UserControl DialogContent
        {
            get => dialogContent;
            set
            {
                Set(() => DialogContent, ref dialogContent, value);
            }
        }
        private bool isDialogOpen;
        public bool IsDialogOpen
        {
            get => isDialogOpen;
            set
            {
                Set(() => IsDialogOpen, ref isDialogOpen, value);
            }
        }
        private HeaderedItemViewModel selectedTab;
        public HeaderedItemViewModel SelectedTab
        {
            get => selectedTab;
            set
            {
                if (selectedTab == value)
                    return;
                Set(() => SelectedTab, ref selectedTab, value);
            }
        }

        private readonly DispatcherTimer hisAPITimer = new(DispatcherPriority.Render) { Interval = TimeSpan.FromSeconds(0.1) };

        private string cardReaderStatus;

        public string CardReaderStatus
        {
            get => cardReaderStatus;
            set
            {
                if (cardReaderStatus == value)
                    return;
                cardReaderStatus = value;
                RaisePropertyChanged(nameof(CardReaderStatus));
            }
        }
        public RelayCommand OpenCommonSettingDialogCommand { get; }
        public RelayCommand LoginSuccessCommand { get; }
        public RelayCommand<FeaturesEnum> OpenFeatureCommand { get; }
        public RelayCommand<PrescriptionData> OpenPrescriptionEditCommand { get; }
        public MainViewModel()
        {
            OpenedTabs = new ObservableCollection<HeaderedItemViewModel>
            {
                new()
                {
                    Header = FeaturesFactory.GetFeatureHeader(FeaturesEnum.首頁),
                    Content = FeaturesFactory.GetFeatureContent(FeaturesEnum.首頁)
                },
                new()
                {
                    Header = FeaturesFactory.GetFeatureHeader(FeaturesEnum.調劑),
                    Content = FeaturesFactory.GetFeatureContent(FeaturesEnum.調劑)
                }
            };
            hisAPITimer.Tick += GetCardReaderStatus;
            hisAPITimer.Start();
            OpenCommonSettingDialogCommand = new RelayCommand(OpenCommandSettingWindow);
            OpenFeatureCommand = new RelayCommand<FeaturesEnum>(OpenFeature);
            LoginSuccessCommand = new RelayCommand(LoginSuccess);
            OpenPrescriptionEditCommand = new RelayCommand<PrescriptionData>(OpenPrescriptionEdit);
            DialogContent = new LoginView();
            IsDialogOpen = true;
        }

        private async void OpenCommandSettingWindow()
        {
            DialogContent = new CommonSettingDialog();
            IsDialogOpen = true;
        }

        private void GetCardReaderStatus(object sender, EventArgs e)
        {
            CardReaderStatus = HisAPI.GetErrorCodeDescription();
        }

        private void LoginSuccess()
        {
            if(CheckLoginSuccess())
                IsDialogOpen = false;
        }

        private bool CheckLoginSuccess()
        {
            return true;
        }

        private void OpenFeature(FeaturesEnum feature)
        {
            var featureTab = new HeaderedItemViewModel
            {
                Header = FeaturesFactory.GetFeatureHeader(feature),
                Content = FeaturesFactory.GetFeatureContent(feature)
            };
            var existTab =
                OpenedTabs.SingleOrDefault(t => ((HeaderWithCloseViewModel)t.Header).Header
                    .Equals(((HeaderWithCloseViewModel)featureTab.Header).Header));
            if (existTab != null)
            {
                SelectedTab = existTab;
                return;
            }
            OpenedTabs.Add(featureTab);
            SelectedTab = OpenedTabs.Single(tab => tab == featureTab);
        }

        private void OpenPrescriptionEdit(PrescriptionData editedPrescription)
        {
            var dispenseEdit = ViewModelLocator.Locator.DispenseEdit;
            var header = FeaturesFactory.GetFeatureHeader(FeaturesEnum.處方編輯);
            var existTab =
                OpenedTabs.SingleOrDefault(t => ((HeaderWithCloseViewModel)t.Header).Header
                    .Equals(header.Header));
            if (existTab != null)
            {
                if (dispenseEdit.CanAddPrescription)
                    dispenseEdit.AddPrescription(editedPrescription);
                else
                    MessageBox.Show("編輯中的處方已達上限，請完成部分處方後再試");
                SelectedTab = existTab;
                return;
            }
            var featureTab = new HeaderedItemViewModel
            {
                Header = header,
                Content = new DispenseEditView()
            };
            OpenedTabs.Add(featureTab);
            dispenseEdit.AddPrescription(editedPrescription);
            SelectedTab = OpenedTabs.Single(tab => tab == featureTab);
        }
    }
}
