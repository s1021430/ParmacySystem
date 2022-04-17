using System;
using System.Data.SqlClient;

namespace GeneralClass.Common
{
    public interface IHolidayRepository
    {
        int GetHolidayCountBetweenDays(DateTime startDate, DateTime endDate);
    }

    public class Holiday
    {
    }
}
