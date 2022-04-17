using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GeneralClass.Manufactory;

namespace PharmacySystem.Manufactory
{
    public class ManufactoryViewModel : ObservableObject
    {
        public static implicit operator ManufactoryViewModel(GeneralClass.Manufactory.Manufactory manufactory) => new ManufactoryViewModel(manufactory);

        private int id;
        public int ID
        {
            get => id;
            set => Set(ref id, value);
        }

        private string name;
        public string Name
        {
            get => name;
            set => Set(ref name, value);
        }
        
        private ManufactoryViewModel(GeneralClass.Manufactory.Manufactory manufactory)
        {
            id = manufactory.ID.ID;
            name = manufactory.Name;
        }
    }
}
