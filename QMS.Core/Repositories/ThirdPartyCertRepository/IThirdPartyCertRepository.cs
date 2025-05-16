using QMS.Core.DatabaseContext;
using QMS.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QMS.Core.Repositories.ThirdPartyCertRepository
{
    public interface IThirdPartyCertRepository
    {
       Task<List<ThirdPartyCertificateMasterViewModel>> GetCertificatesAsync();
        Task<ThirdPartyCertificateMasterViewModel?> GetCertificateByIdAsync(int certificateId);
        Task<OperationResult> CreateCertificateAsync(ThirdPartyCertificateMaster model, bool checkDuplicate);
        Task<OperationResult> DeleteAsync(int Id,string updatedby);
        Task<bool> CheckDuplicate(string searchText, int Id);
        Task<OperationResult> UpdateCertificateAsync(ThirdPartyCertificateMasterViewModel model);
   
    }
}
