using System.Collections.Generic;
using GeneralClass;
using GeneralClass.Prescription.MedicalBaseClass;
using PharmacySystemInfrastructure.Institution;

namespace PharmacySystem.ApplicationServiceLayer
{
    public static class InstitutionServiceProvider
    {
        public static readonly InstitutionApplicationService Service =
            InstitutionApplicationServiceFactory.GetInstitutionApplicationService();
    }

    public static class InstitutionApplicationServiceFactory
    {
        public static InstitutionApplicationService GetInstitutionApplicationService()
        {
            var institutionRepository = new InstitutionDatabaseRepository();
            return new InstitutionApplicationService(institutionRepository);
        }
    }

    public class InstitutionApplicationService
    {
        private readonly IInstitutionRepository repository;

        public InstitutionApplicationService(IInstitutionRepository institutionRepository)
        {
            repository = institutionRepository;
        }

        public List<Institution> GetInstitutionById(string id)
        {
            return repository.GetInstitutionByID(id);
        }

        public List<Institution> GetInstitutionsByMultipleId(IEnumerable<string> idList)
        {
            return repository.GetInstitutionsByMultipleId(idList);
        }

        public List<Institution> GetInstitutionByName(string name)
        {
            return repository.GetInstitutionByName(name);
        }

        public ServiceResult SearchConditionValidate(InstitutionSearchCondition condition, Institution searchObject)
        {
            InstitutionSearchPattern searchPattern;
            if (searchObject is null)
                return new ServiceResult { Success = false, ErrorMessage = "無查詢條件" };
            switch (condition)
            {
                case InstitutionSearchCondition.ID:
                    if (string.IsNullOrEmpty(searchObject.ID) || searchObject.ID.Length < 5)
                        return new ServiceResult { Success = false, ErrorMessage = "查詢代碼字數不可少於5" };
                    searchPattern = new InstitutionSearchPattern(condition, searchObject.ID);
                    return new ServiceResult { Success = true, Result = searchPattern };
                case InstitutionSearchCondition.Name:
                    if (string.IsNullOrEmpty(searchObject.Name))
                        return new ServiceResult { Success = false, ErrorMessage = "查詢名稱不可為空值" };
                    searchPattern = new InstitutionSearchPattern(condition, searchObject.Name);
                    return new ServiceResult { Success = true, Result = searchPattern };
                default:
                    return new ServiceResult { Success = false, ErrorMessage = "查詢條件異常" };
            }
        }
    }
}
