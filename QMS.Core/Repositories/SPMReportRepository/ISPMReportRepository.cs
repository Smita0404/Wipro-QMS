using QMS.Core.Models;
using QMS.Core.DatabaseContext;
using QMS.Core.Repositories.Shared;

namespace QMS.Core.Repositories.SPMReportRepository
{
    public interface ISPMReportRepository
    {
        Task<List<SPMReportViewModel>> GetListAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<SPMReport?> GetByIdAsync(int id);
        Task<OperationResult> CreateAsync(SPMReport entity);
        Task<OperationResult> UpdateAsync(SPMReport entity);
        Task<OperationResult> DeleteAsync(int id);
        Task<List<DropdownOptionViewModel>> GetVendorDropdownAsync();
    }
}
