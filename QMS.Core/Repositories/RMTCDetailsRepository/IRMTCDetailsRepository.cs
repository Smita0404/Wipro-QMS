using QMS.Core.DatabaseContext;
using QMS.Core.Models;

namespace QMS.Core.Repositories.RMTCDetailsRepository
{
    public interface IRMTCDetailsRepository
    {
        Task<List<RMTCDetailsViewModel>> GetListAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<RMTCDetails?> GetByIdAsync(int id);
        Task<OperationResult> CreateAsync(RMTCDetails entity, bool returnCreatedRecord = false);
        Task<OperationResult> UpdateAsync(RMTCDetails entity, bool returnUpdatedRecord = false);
        Task<OperationResult> DeleteAsync(int id);
        Task<List<DropdownOptionViewModel>> GetVendorDropdownAsync();
        Task<List<ProductCodeDetailViewModel>> GetCodeSearchAsync(string search = "");
    }
}
