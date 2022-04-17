using System;
using System.Data.SqlClient;
using GeneralClass;
using GeneralClass.Common;
using PharmacySystemInfrastructure.Holiday;

namespace PharmacySystem.ApplicationServiceLayer
{
    public class HolidayApplicationService
    {
        private readonly IHolidayRepository holidayRepository;
        public HolidayApplicationService()
        {
            holidayRepository = new HolidayDatabaseRepository();
        }

        public int GetHolidayCountBetweenDays(DateTime startDate, DateTime endDate)
        {
            if (startDate.Date == endDate.Date) return 0;
            using var conn = new SqlConnection(DBInvoker.ConnectionString);
            var result = holidayRepository.GetHolidayCountBetweenDays(startDate, endDate);
            return result;
        }
    }
}
