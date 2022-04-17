using System.IO;
using System.Linq;
using GeneralClass;
using GeneralClass.SystemInfo;
using PharmacySystemInfrastructure;

namespace PharmacySystem.ApplicationServiceLayer
{
    public class InitialSettingApplicationService
    {
        private InitialSettingRepository InitialSettingRepository;

        public InitialSettingApplicationService()
        {
            InitialSettingRepository = new InitialSettingRepository();
        }

        public bool IsNeedInit()
        {
            string filePath = "C:\\PharmacySystem\\sysInfo.ini";
            string dirPath = "C:\\PharmacySystem";
            if (File.Exists(filePath) == false)
            {
                if (Directory.Exists(dirPath) == false)
                    Directory.CreateDirectory(dirPath);

                File.Create(filePath);

                return true;
            }
            string[] lines = File.ReadAllLines(filePath);
            if (lines.Count() == 0)
                return true;

            DBInvoker.ConnectionString = $@"Database=Main;Server={lines[0]};User Id=PS_Admin;Password=city1234;";

            return InitialSettingRepository.IsNeedInit();
        }
        public bool RegisterData(InitialSettingData data)
        {
            string filePath = "C:\\PharmacySystem\\sysInfo.ini";

            if (InitialSettingRepository.RegisterData(data))
            {
                using (StreamWriter sw = File.CreateText(filePath))
                {
                    sw.WriteLine(data.IP);
                }
                return true;
            }

            return false;
        }
    }
}
