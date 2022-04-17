using System.Collections.Generic;
using GeneralClass.WareHouse.EntityIndex;

namespace GeneralClass.WareHouse
{
    public interface IWareHouseRepository
    {
        WareHouse GetWareHouseByID(WareHouseID wareHouseID);
        bool Save(WareHouse wareHouse);
        bool Create(WareHouse wareHouse);
        bool Delete(WareHouseID wareHouseID);
        public List<WareHouse> GetAllWareHouses();
    }
}
