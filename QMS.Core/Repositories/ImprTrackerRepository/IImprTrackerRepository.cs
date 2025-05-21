using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using QMS.Core.DatabaseContext;
using QMS.Core.Models;

namespace QMS.Core.Repositories.ImprTrackerRepository
{
    public interface IImprTrackerRepository
    {
        Task<List<ImprovementTrackerViewModel>> GetListAsync();
        Task<OperationResult> CreateAsync(ImprovementTracker improvementTracker, bool returnCreatedRecord = false);
        Task<OperationResult> UpdateAsync(ImprovementTracker improvementTracker, bool returnUpdatedRecord = false);
        Task<OperationResult> DeleteAsync(int id);
        Task<ImprovementTrackerViewModel?> GetByIdAsync(int id);
    }
}
