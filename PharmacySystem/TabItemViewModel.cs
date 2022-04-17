using GalaSoft.MvvmLight;

namespace PharmacySystem.ViewModel
{
    public class TabItemViewModel : ObservableObject
    {
        private string title;
        public string Title
        {
            get => title;
            private set
            {
                Set(() => Title, ref title, value);
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

        public TabItemViewModel(string title, bool closable = true)
        {
            Title = title;
            Closable = closable;
        }
    }
}
