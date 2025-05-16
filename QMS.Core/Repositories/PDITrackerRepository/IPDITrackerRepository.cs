using QMS.Core.DatabaseContext;
using QMS.Core.Models;

namespace QMS.Core.Repositories.PDITrackerRepository
{
    public interface IPDITrackerRepository
    {
        Task<List<PDITrackerViewModel>> GetListAsync();
        Task<PDITracker?> GetByIdAsync(int id);
        Task<OperationResult> CreateAsync(PDITracker entity, bool returnCreatedRecord = false);
        Task<OperationResult> UpdateAsync(PDITracker entity, bool returnUpdatedRecord = false);
        Task<OperationResult> DeleteAsync(int id);
        Task<List<ProductCodeDetailViewModel>> GetCodeSearchAsync(string search = "");
        Task<List<ProductCodeDetailViewModel>> GetCodeSelect2OptionsAsync(string search = "");

    }
}
