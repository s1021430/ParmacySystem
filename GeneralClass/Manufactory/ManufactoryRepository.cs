using System.Collections.Generic;
using GeneralClass.Manufactory.EntityIndex;
using GeneralClass.Manufactory.SearchCondition;

namespace GeneralClass.Manufactory
{
    public interface IManufactoryRepository
    {
        List<Manufactory> GetAllManufactories();
        bool Save(Manufactory manufactory);
        bool Create(Manufactory manufactory);
        bool Delete(ManufactoryID id);
        List<Manufactory> GetManufactories(ManufactorySearchCondition searchCondition);
    }
}
