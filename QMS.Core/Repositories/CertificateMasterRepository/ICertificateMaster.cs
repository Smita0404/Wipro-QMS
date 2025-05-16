using QMS.Core.DatabaseContext;
using QMS.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QMS.Core.Repositories.CertificateMasterRepository
{
    public interface ICertificateMasterRepository
    {
       Task<List<CertificateMasterViewModel>> GetCertificatesAsync();
        Task<CertificateMasterViewModel?> GetCertificateByIdAsync(int certificateId);
        Task<OperationResult> CreateCertificateAsync(CertificateMaster model, bool checkDuplicate);
        Task<OperationResult> DeleteAsync(int Id, string updatedby);
        Task<bool> CheckDuplicate(string searchText, int Id);
        Task<OperationResult> UpdateCertificateAsync(CertificateMasterViewModel model);
   
    }
}
