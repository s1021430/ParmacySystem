using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace PharmacySystem.Product
{
    public class ProductViewModel : ObservableObject
    {
        protected string id;
        public string ID
        {
            get => id;
            set => Set(ref id, value);
        }

        protected string name;
        public string Name
        {
            get => name;
            set => Set(ref name, value);
        }
    }
}
