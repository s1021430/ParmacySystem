using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace PharmacySystem.WareHouse
{
    public class WareHouseViewModel : ObservableObject
    {
        public static implicit operator WareHouseViewModel(GeneralClass.WareHouse.WareHouse wareHouse) => new WareHouseViewModel(wareHouse);

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

        private WareHouseViewModel(GeneralClass.WareHouse.WareHouse wareHouse)
        {
            id = wareHouse.ID.ID;
            name = wareHouse.Name;
        }
    }
}
