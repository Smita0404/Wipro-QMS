using QMS.Core.DatabaseContext;
using QMS.Core.Models;
using QMS.Core.Repositories.Shared;

namespace QMS.Core.Repositories.DNTrackerRepository
{
    public interface IDNTrackerRepository
    {
        Task<List<DNTrackerViewModel>> GetListAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<DNTracker?> GetByIdAsync(int id);
        Task<OperationResult> CreateAsync(DNTracker entity);
        Task<OperationResult> UpdateAsync(DNTracker entity);
        Task<OperationResult> DeleteAsync(int id);
        Task<List<DropdownOptionViewModel>> GetProductCodeAsync();
        Task<List<DropdownOptionViewModel>> GetVendorDropdownAsync();
    }
}
