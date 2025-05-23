using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using QMS.Core.DatabaseContext;
using QMS.Core.Models;

namespace QMS.Core.Repositories.KaizenTrackerRepository
{
    public interface IKaizenTrackerRepository
    {
        Task<List<KaizenTrackerViewModel>> GetListAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<KaizenTracker?> GetByIdAsync(int id);
        Task<OperationResult> CreateAsync(KaizenTracker entity, bool returnCreatedRecord = false);
        Task<OperationResult> UpdateAsync(KaizenTracker entity, bool returnUpdatedRecord = false);
        Task<OperationResult> DeleteAsync(int id);
        Task<List<DropdownOptionViewModel>> GetVendorDropdownAsync();
    }
}
